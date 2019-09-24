var retroAccountDialog = function () {

    var URLs = {
        impactedfolderVersions: '/FolderVersion/GetImpactedFolderVersionList?folderId={folderId}&effectiveDate={effectiveDate}&tenantId=1',
        folderVersionRetro: '/FolderVersion/FolderVersionRetro',
        isValidRetroEffectiveDate: '/FolderVersion/IsValidRetroEffectiveDate?folderId={folderId}&tenantId={tenantId}&retroEffectiveDate={retroEffectiveDate}',
        //Gets the Category List
        categoryList: '/Settings/GetFolderVersionCategoryForDropdown?tenantId=1&isPortfolio=null&folderVersionID={folderVersionID}'
    };
    var elementIDs = {
        retroEffectiveDate: '#retroEffectiveDate',
        retroAccountDialog: '#retroAccountDialog',
        impactedFoldersJQGrid: '#impactedFoldersGrid',
        impactedFoldersDiv: '#impactedFoldersdiv',
        searchBtn: '#searchImpactedFolders',
        cancelBtn: '#cancelSearch',
        retroEffectiveDateSpan: '#retroeffectiveDateSpan',
        folderNameDiv: '#folderNameDiv',
        effectiveDateDiv: '#effectiveDateDiv',
        backBtn: '#backBtn',
        saveBtn: '#saveBtn',
        majorVersionGridJQ: '#majorVersion',
        showRetroFoldersMsg: '#showRetroFoldersMsg',
        categoryNamesDDLJQ: '#retro_CategoryDDL',
        categoryNamesAutoCompltDDLJQ: '#retro_CategoryNamesAutoCompltDDL',
        retroCategoryDDLSpan: "#retro_CategoryDDLSpan",
        selectCatLabelJQ: '#selectCatLabel',
        selectCategoryIDLabelJQ: '#selectCategoryIDLabel',
        catIDJQ: '#retro_CatIDTXT'
    };

    function folderSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            $(elementIDs.retroAccountDialog).dialog("close");
            messageDialog.show(FolderVersion.folderVersionRetroSuccess);
            versionHistory.loadVersionHistory();
            versionHistoryML.loadVersionHistory();
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
    }

    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    function loadImpactedFoldersGrid(isLoaded, folderId, effectiveDate) {
        //set column list for grid
        var colArray = ['', 'Folder Version', 'EffectiveDate', 'Comments',''];
        var url;
        //set column models
        var colModel = [];
        colModel.push({ name: 'FolderVersionId', index: 'FolderVersionId', key: true, editable: false, hidden: true });
        colModel.push({ name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: false, align: 'right' });
        colModel.push({
            name: 'EffectiveDate', index: 'EffectiveDate', editable: false, formatter: 'date',
            formatoptions: JQGridSettings.DateFormatterOptions, align: 'center'
        });
        colModel.push({ name: 'Comments', index: 'Comments', key: true, editable: false });
        colModel.push({ name: 'VersionNumber', index: 'VersionNumber', key: true, editable: false, hidden: true });

        //clean up the grid first - only table element remains after this
        $(elementIDs.impactedFoldersJQGrid).jqGrid('GridUnload');

        //load majorfolderversion grid only for release functionality 
        if (isLoaded == false) {
            url = "";
        } else {
            url = URLs.impactedfolderVersions.replace(/\{folderId\}/g, folderId).replace(/\{effectiveDate}/g, effectiveDate);
        }

        //adding the pager element
        $(elementIDs.impactedFoldersJQGrid).parent().append("<div id='p" + elementIDs.impactedFoldersJQGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.impactedFoldersJQGrid).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Impacted Folders',
            height: '200',
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            shrinkToFit: false,
            viewrecords: true,
            hidegrid: true,
            altRows: true,
            pager: '#p' + elementIDs.impactedFoldersJQGrid,
            altclass: 'alternate',
            gridComplete: function () {
                $(elementIDs.saveBtn).removeAttr('disabled', 'disabled');
                var allRowsData = $(elementIDs.impactedFoldersJQGrid).getRowData();
                if (allRowsData.length > 0) {
                    $.each(allRowsData, function (i, item) {
                        if (new Date(item.EffectiveDate) <= new Date(effectiveDate)) {
                            $(elementIDs.retroAccountDialog).find('#' + item.FolderVersionId + '').hide();
                        }
                    });
                }
            }
        });

        var pagerElement = '#p' + elementIDs.impactedFoldersJQGrid;
        //remove default buttons
        $(elementIDs.impactedFoldersJQGrid).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove scroll bar
        //$(elementIDs.impactedFoldersJQGrid).parent().parent().css('overflow-x', 'hidden');
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        // add filter toolbar to the top
        $(elementIDs.impactedFoldersJQGrid).jqGrid('filterToolbar', {
            stringResult: true,
            searchOnEnter: false,
            defaultSearch: "cn"
        });
    }

    function loadControlsForRetroWizard() {
        $(elementIDs.impactedFoldersDiv).hide();
        $(elementIDs.backBtn).hide();
        $(elementIDs.saveBtn).hide();
        // $(elementIDs.folderNameDiv).show();
        $(elementIDs.effectiveDateDiv).show();
        $(elementIDs.searchBtn).show();
        $(elementIDs.cancelBtn).show();
        $(elementIDs.retroEffectiveDate).val('');
        $(elementIDs.showRetroFoldersMsg).hide();
    }

    function loadControlsForImpactedFolders() {
        $(elementIDs.folderNameDiv).hide();
        $(elementIDs.effectiveDateDiv).hide();
        $(elementIDs.searchBtn).hide();
        $(elementIDs.cancelBtn).hide();
        $(elementIDs.impactedFoldersDiv).show();
        $(elementIDs.backBtn).show();
        $(elementIDs.saveBtn).show();
        $(elementIDs.showRetroFoldersMsg).show();
    }

    $(function () {
        $(elementIDs.categoryNamesDDLJQ).autoCompleteDropDown({ ID: 'retro_CategoryNamesAutoCompltDDL' });
        $(elementIDs.categoryNamesDDLJQ).click(function () {
            $(elementIDs.categoryNamesDDLJQ).toggle();
        });
    });

    //to fill drop down list of Category names
    function fillCategoryDDL(selectedIndex) {
        var categoryName;
        var defaultCategoryText = "-- Select Category --";
        //To refresh dropdown of Consortium names
        $(elementIDs.categoryNamesDDLJQ).empty();
        $(elementIDs.retroCategoryDDLSpan).text('');
        $(elementIDs.categoryNamesDDLJQ).append("<option value=0>" + defaultCategoryText + "</option>");
        //ajax call for drop down list of account names
        var majorVersions = $(elementIDs.majorVersionGridJQ).jqGrid('getGridParam', 'data');
        var folderVersionID = majorVersions[0].FolderVersionId;
        var url = URLs.categoryList.replace(/{folderVersionID}/g, folderVersionID);
        var promise = ajaxWrapper.getJSON(url);
        promise.done(function (names) {
            for (i = 0; i < names.length; i++) {
                // For creating New Version the group NewGroup and JAA NewGroup should not be include , Refer ANT-13 and ANT-91
                //if (names[i].FolderVersionCategoryName != AntWFCategoriesGroups.NewGroup && names[i].FolderVersionCategoryName != AntWFCategoriesGroups.JAANewGroup) {
                    // If Role is TPA Analyst then include only JAA New Group, else for all other roles (SuperUser,EBA Analyst) should include all
                    //if (vbRole == Role.TPAAnalyst) {
                        //if (names[i].FolderVersionCategoryName == AntWFCategoriesGroups.JAAChangeRequest) {
                    //        $(elementIDs.categoryNamesDDLJQ).append("<option value=" + names[i].FolderVersionCategoryID + ">" + names[i].FolderVersionCategoryName + "</option>");
                        //}
                    //}
                    //else {
                        $(elementIDs.categoryNamesDDLJQ).append("<option value=" + names[i].FolderVersionCategoryID + ">" + names[i].FolderVersionCategoryName + "</option>");
                    //}
                //}
                if (selectedIndex == names[i].FolderVersionCategoryID)
                    FolderVersionCategoryName = names[i].FolderVersionCategoryName;
            }
            $(elementIDs.categoryNamesDDLJQ).val(selectedIndex);
            if (selectedIndex > 0)
                $(elementIDs.categoryNamesAutoCompltDDLJQ).val(FolderVersionCategoryName);
        });
        promise.fail(showError);
    }

    function init() {
        //register dialog for grid row add/edit
        $(elementIDs.retroAccountDialog).dialog({
            autoOpen: false,
            resizable: false,
            closeOnEscape: true,
            height: 'auto',
            width: 500,
            modal: true,
            position: 'center'
        });

        //To display date picker for folder effective date.
        $(elementIDs.retroEffectiveDate).datepicker({
            dateFormat: 'mm/dd/yy',
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-61:c+20',
            showOn: "both",
            buttonImage: Icons.CalenderIcon,
            buttonImageOnly: true
        }).parent().find('img').css('margin-bottom', '8px');

        $(elementIDs.searchBtn).on('click', function () {
            var effectiveDate = $(elementIDs.retroEffectiveDate).val();
            //To remove red border of text box when folder effective date is specified
            var category = $(elementIDs.categoryNamesDDLJQ).val();
            if (effectiveDate != "" && category != "0") {
                var effectiveDateMessage = isValidEffectiveDate(effectiveDate);
                if (effectiveDateMessage == "") {
                    $(elementIDs.retroEffectiveDate).removeClass('form-control');
                    $(elementIDs.retroEffectiveDateSpan).text('');

                    $(elementIDs.categoryNamesDDLJQ).removeClass('form-control');
                    $(elementIDs.retroCategoryDDLSpan).text('');

                    var url = URLs.isValidRetroEffectiveDate.replace(/\{folderId\}/g, folderData.folderId)
                                    .replace(/\{tenantId\}/g, folderData.tenantId)
                                    .replace(/\{retroEffectiveDate\}/g, effectiveDate);
                    var promise = ajaxWrapper.getJSON(url);

                    promise.done(function (isValid) {
                        if (isValid.Result == ServiceResult.SUCCESS) {
                            loadControlsForImpactedFolders();
                            loadImpactedFoldersGrid(true, folderData.folderId, effectiveDate);
                        } else {
                            $(elementIDs.retroAccountDialog + ' div').addClass('has-error');
                            $(elementIDs.retroEffectiveDate).addClass('form-control');
                            $(elementIDs.retroEffectiveDateSpan).text(FolderVersion.retroisNotEffective);
                        }
                    });
                }
                else {
                    $(elementIDs.retroAccountDialog + ' div').addClass('has-error');
                    $(elementIDs.retroEffectiveDate).addClass('form-control');
                    $(elementIDs.retroEffectiveDateSpan).text(effectiveDateMessage);
                }
            }
            else {
                if (effectiveDate == "") {
                    $(elementIDs.retroAccountDialog + ' div').addClass('has-error');
                    $(elementIDs.retroEffectiveDate).addClass('form-control');
                    $(elementIDs.retroEffectiveDateSpan).text(Common.folderEffectiveDateRequiredMsg);
                }
                if (category == "0") {
                    $(elementIDs.retroAccountDialog + ' div').addClass('has-error');
                    $(elementIDs.categoryNamesDDLJQ).addClass('form-control')
                    $(elementIDs.retroCategoryDDLSpan).text("Category is required!");
                }
            }
        });

        $(elementIDs.cancelBtn).on('click', function () {
            $(elementIDs.retroAccountDialog).dialog("close");
        });

        $(elementIDs.backBtn).on('click', function () {
            loadControlsForRetroWizard();
        });

        $(elementIDs.saveBtn).on('click', function () {
            var retroDate = $(elementIDs.retroEffectiveDate).val();
            var categoryId = $(elementIDs.categoryNamesDDLJQ).val();
            var catId = $(elementIDs.catIDJQ).val();
            $(elementIDs.retroCategoryDDLSpan).text('');
            if (categoryId == 0) {
                $(elementIDs.retroAccountDialog + ' div').addClass('has-error');
                $(elementIDs.retroCategoryDDLSpan).text("Category is required!");
            }
            else {
                var impactedRows = $(elementIDs.impactedFoldersJQGrid).getRowData();
                var retroChangesList = [];
                var isVisited = false;
                $.each(impactedRows, function (i, item) {
                    var retroChange = {
                        FolderVersionId: item.FolderVersionId,
                        EffectiveDate: retroDate,
                        IsCopyRetro: true,
                        FolderVersionNumber: item.FolderVersionNumber,
                        VersionNumber: item.VersionNumber,
                    };
                    if ((new Date(item.EffectiveDate) <= new Date(retroDate)) && !isVisited) {
                        retroChange.IsCopyRetro = false;
                        isVisited = true;
                    }
                    retroChangesList.push(retroChange);
                });
                var parameters = {
                    retroChangesList: retroChangesList,
                    tenantId: folderData.tenantId,
                    folderId: folderData.folderId,
                    retroEffectiveDate: retroDate,
                    categoryID: categoryId,
                    catID: catId
                };
                var url = URLs.folderVersionRetro;
                var promise = ajaxWrapper.postJSONCustom(url, parameters);
                //register ajax success callback
                promise.done(folderSuccess);
                //register ajax failure callback
                promise.fail(showError);
            }
            });
        
        
    }

    //initialize the dialog after this js is loaded
    $(document).ready(function () {
        //initialize the dialog after this js are loaded
        init();
        
    });

    return {
        show: function (minorVersionData) {
            
            loadControlsForRetroWizard();
            $('#impactedFoldersdiv').css({ 'margin-top': '-50px' });

            retroAccountDialog.minorVersionDataInstance(minorVersionData);
            $(elementIDs.retroAccountDialog).dialog('option', 'title', 'Retro Wizard');
            $(elementIDs.retroAccountDialog).css('overflow', 'hidden');
            $(elementIDs.retroAccountDialog).dialog("open");

            $(elementIDs.retroEffectiveDate).removeClass('form-control');
            $(elementIDs.retroEffectiveDate).removeClass('has-error');
            $(elementIDs.retroEffectiveDateSpan).text('');
            if (folderData.folderType != "MasterList") {
                fillCategoryDDL("0");
                $(elementIDs.categoryNamesAutoCompltDDLJQ).show();
                $(elementIDs.selectCatLabelJQ).show();
            }
            else {
                $(elementIDs.categoryNamesAutoCompltDDLJQ).hide();
                $(elementIDs.selectCatLabelJQ).hide();
            }
        },
        // set minorVersionData property
        minorVersionDataInstance: function (minorVersionData) {
            this.minorVersionData = minorVersionData;
        }
    };
}();
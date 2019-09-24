var folderVersionBaseline = function () {
    
    //urls to be accessed  
    var URLs = {
        getMinorFolderVersionList: '/FolderVersion/GetMinorFolderVersionList?folderId={folderId}&tenantId=1&ownerName={ownerName}&isBaseline={isBaseline}',
        getMajorFolderVersionList: '/FolderVersion/GetMajorFolderVersionList?folderId={folderId}&tenantId=1&userName={userName}&versionNumber={versionNumber}&effectiveDate={effectiveDate}',
        baseLineFolderVersion: '/FolderVersion/BaseLineFolderVersion',
        folderVersionIndex: '/FolderVersion/Index?tenantId=1&folderVersionId={folderVersionId}&folderId={folderId}&foldeViewMode={foldeViewMode}&mode=true'
    };

    //Grid Ids for major and minor version
    var elementIDs = {
        //for dialog for BaseLine
        baseLineButton: '#btnBaseline',
        baseLineDialogue: "#baseLineDialogue",
        doneButtonId: "#updateBaseLine",
        majorVersionGrid: "majorFolderVersion",
        majorVersionGridJQ: "#majorFolderVersion",
        minorVersionGrid: "minorFolderVersion",
        minorVersionGridJQ: "#minorFolderVersion",
        updatestatus: "#updatestatus"

    };

    //ajax success callback
    function addSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            //reload Folder Version Index page
            var url = URLs.folderVersionIndex.replace(/\{folderVersionId\}/g, parseInt(xhr.Items[0].Messages[0])).replace(/\{folderId\}/g, folderData.folderId).replace(/{foldeViewMode}/g, folderData.FolderViewMode);
            $(elementIDs.baseLineDialogue).dialog("close");
            window.location.href = url;
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
       
    }

    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    // Load Major Folder Version Grid
    function loadMajorVersionGrid(isBaseLine, ownerName) {
        //set column list for grid
        var colArray = ['', 'Major Version', 'Effective Date', 'Release Date', 'User', 'Comments'];
        var url;
        //set column models
        var colModel = [];
        colModel.push({ name: 'FolderVersionId', index: 'FolderVersionId', key: true, editable: false, hidden: true });
        colModel.push({ name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: false, align: 'right', width: '60' });
        colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center', width: '60' });
        colModel.push({ name: 'ReleaseDate', index: 'ReleaseDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center', width: '60' });
        colModel.push({ name: 'UserName', index: 'UserName', editable: false, align: 'left', width: '60' });
        colModel.push({ name: 'Comments', index: 'Comments', editable: true, formatter: formatColumns, unformat: unformatColumns, sortable: false });

        //clean up the grid first - only table element remains after this
        $(elementIDs.majorVersionGridJQ).jqGrid('GridUnload');

        //load majorfolderversion grid only for release functionality 
        if (isBaseLine == true) {
            url = "";
        } else {
            url = URLs.getMajorFolderVersionList.replace(/\{folderId\}/g, folderData.folderId)
                                                .replace(/\{userName\}/g, ownerName)
                                                .replace(/\{versionNumber\}/g, folderData.versionNumber)
                                                .replace(/\{effectiveDate\}/g, folderData.effectiveDate);
        }

        //adding the pager element
        $(elementIDs.majorVersionGridJQ).parent().append("<div id='p" + elementIDs.majorVersionGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.majorVersionGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Major Version List',
            height: 100,
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: true,
            altRows: true,
            pager: '#p' + elementIDs.majorVersionGrid,
            altclass: 'alternate',
            gridComplete: function () {
                var majorTDLength = $(elementIDs.majorVersionGridJQ + " tr:nth-child(2)").children().length;
                $(elementIDs.majorVersionGridJQ + " tr:nth-child(2) td:nth-child(" + majorTDLength + ")").children().resizable();
            }
        
        });

        var pagerElement = '#p' + elementIDs.majorVersionGrid;
        //remove default buttons
        $(elementIDs.majorVersionGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false, refresh: false });
        //remove scroll bar
        $(elementIDs.majorVersionGridJQ).parent().parent().css('overflow-x', 'hidden');
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        // add filter toolbar to the top
        $(elementIDs.majorVersionGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });
    }

    // Load Minor Folder Version Grid
    function loadMinorVersionGrid(isBaseLine, ownerName) {
        //set column list for grid
        var colArray = ['', 'Minor Version', 'Baseline Date', 'User', 'Comments'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'FolderVersionId', index: 'FolderVersionId', key: true, editable: false, hidden: true });
        colModel.push({ name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: false, align: 'right', width: '60' });
        colModel.push({ name: 'BaseLineDate', index: 'BaseLineDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center', width: '60' });
        colModel.push({ name: 'UserName', index: 'UserName', editable: false, align: 'left', width: '60' });
        colModel.push({ name: 'Comments', index: 'Comments', editable: true, formatter: formatColumns, unformat: unformatColumns });

        //clean up the grid first - only table element remains after this
        $(elementIDs.minorVersionGridJQ).jqGrid('GridUnload');

        var url = URLs.getMinorFolderVersionList.replace(/\{folderId\}/g, folderData.folderId).replace(/\{ownerName\}/g, ownerName).replace(/\{isBaseline\}/g, isBaseLine);
        //adding the pager element
        $(elementIDs.minorVersionGridJQ).parent().append("<div id='p" + elementIDs.minorVersionGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        var lastsel2;
        $(elementIDs.minorVersionGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Minor Version List',
            height: 150,
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: true,
            altRows: true,
            pager: '#p' + elementIDs.minorVersionGrid,
            viewsortcols: true,
            altclass: 'alternate',
            gridComplete: function () {
                var minorTDLength = $(elementIDs.minorVersionGridJQ + " tr:nth-child(2)").children().length;
                $(elementIDs.minorVersionGridJQ + " tr:nth-child(2) td:nth-child(" + minorTDLength + ")").children().resizable();
            }
        });
        var pagerElement = '#p' + elementIDs.minorVersionGrid;
        //remove default buttons
        $(elementIDs.minorVersionGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false, refresh: false });
        //remove scroll bar
        $(elementIDs.minorVersionGridJQ).parent().parent().css('overflow-x', 'hidden');

       
      
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        // add filter toolbar to the top
        $(elementIDs.minorVersionGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });
    }
    
   
    // Use TextArea formatter for comments column
    var formatColumns = function (cellValue, options, rowObject) {
        var result;
        if (cellValue === undefined || cellValue === null) {
            cellValue = '';
        }
        if (options.rowId == 0) {
            result = '<textarea class="form-control" id="' + options.colModel.index + '" style="width:90%"  maxlength="2000" rows="5" cols="10" value="' + cellValue + '"/>';
        } else {
            result = cellValue;
        }
        return result;
    }

    // Use unformatter for comments column
    var unformatColumns = function (cellValue, options) {
        var result;
        result = $(this).find('#' + options.rowId).find('#' + options.colModel.index).val();
        return result;
    }


    //this function is called below soon after this JS file is loaded   
    function init() {
        //register dialog for grid row add/edit
        $(elementIDs.baseLineDialogue).dialog({
            autoOpen: false,
            resizable: false,
            closeOnEscape: true,
            height: 'auto',
            width: 850,
            modal: true,
            position: ['top', 30]
        });

              
        //show baseline dialog on baseline button click 
        $(elementIDs.baseLineButton).on('click', function () {
            folderVersionBaseline.show(true);
        });


        $(elementIDs.doneButtonId).on('click', function () {
            //if isRelease false then call only success method
            var rowMinor = $(elementIDs.minorVersionGridJQ).getRowData(0);
            var rowMajor = $(elementIDs.majorVersionGridJQ).getRowData(0);
            var majorTDLength = $(elementIDs.majorVersionGridJQ + " tr:nth-child(2)").children().length;
            var minorTDLength = $(elementIDs.minorVersionGridJQ + " tr:nth-child(2)").children().length;      

            if (folderVersionBaseline.isRelease == undefined || folderVersionBaseline.isRelease == false) {
                var url = URLs.baseLineFolderVersion;                
                if (rowMinor.Comments === "" || rowMinor.Comments === null) {
                    $(elementIDs.minorVersionGridJQ + " tr:nth-child(2) td:nth-child(" + minorTDLength + ")").children().find('textarea').addClass('comment-has-error');
                    messageDialog.show("Minor-Version Comment is Required.");
                } else {
                    $(elementIDs.minorVersionGridJQ + " tr:nth-child(2) td:nth-child(" + minorTDLength + ")").children().find('textarea').removeClass('comment-has-error');
                    var minorVersionToCreate = {
                        tenantId: folderData.tenantId,
                        folderId: folderData.folderId,
                        folderVersionId: folderData.folderVersionId,
                        comments: rowMinor.Comments,
                        versionNumber: rowMinor.FolderVersionNumber,
                        isRelease: false
                    };
                    var promise = ajaxWrapper.postJSON(url, minorVersionToCreate);
                    promise.done(addSuccess);
                    promise.fail(showError);
                }
            } else {
                if (rowMajor.Comments === "" || rowMajor.Comments === null) {
                    $(elementIDs.majorVersionGridJQ + " tr:nth-child(2) td:nth-child(" + majorTDLength + ")").children().find('textarea').addClass('comment-has-error');
                    messageDialog.show("Major-Version Comment is Required.");
                }else {
                    $(elementIDs.majorVersionGridJQ + " tr:nth-child(2) td:nth-child(" + majorTDLength + ")").children().find('textarea').removeClass('comment-has-error');
                    var rowMajor = $(elementIDs.majorVersionGridJQ).getRowData(0);

                    //trigger click on save button of statusupdatedialog and pass all newly generated row data  
                    $(elementIDs.updatestatus).trigger('click', [true, rowMajor.Comments, rowMajor.FolderVersionNumber]);
                    $(elementIDs.baseLineDialogue).dialog("close");
                }
            }
        });


    }

    $(document).ready(function () {
        //initialize the dialog after this js are loaded
        init();
    });

    //these are the properties that can be called by using BaselineDialog.<Property>
    //pass true or false value for isBaseline parameter
    return {
        show: function (isBaseline,ownerName) {
            //open the dialog - uses jqueryui dialog:            
            $(elementIDs.baseLineDialogue).dialog('option', 'title', folderData.folderName);
            if (isBaseline) {
                folderVersionBaseline.setRelease(false);
            } else {
                folderVersionBaseline.setRelease(true);
            }
            $(elementIDs.baseLineDialogue).dialog("open");
            loadMajorVersionGrid(isBaseline, ownerName);
            loadMinorVersionGrid(isBaseline, ownerName);
        },
               
        // set isRelease property to true or false
        setRelease: function (isRelease) {
            this.isRelease = isRelease;
        },

        isRelease: true
    }
}();


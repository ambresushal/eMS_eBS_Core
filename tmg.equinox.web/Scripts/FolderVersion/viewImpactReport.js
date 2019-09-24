var pbpViewAction = function (formInstance) {

    var formInstancebuilder = formInstance;

    var elementIDs = {
        actionGridJQ: "#actionGrid",
        btnPBPImpactLog: "#btnPBPImpactLog",
        btnExportImpactGrid: "#btnExportImpactGrid"
    }
    var URLs = {
        getPBPViewImpactData: '/PBPViewAction/GetPBPViewImpactData',
        folderVersionDetails: '/FolderVersion/Index?tenantId={tenantId}&folderVersionId={folderVersionId}&folderId={folderId}&foldeViewMode={foldeViewMode}&mode={mode}',
        getDefaultViewFormInstanceList: '/FolderVersion/GetFormInstancesList',
        setTempDataForNavigation: '/PBPViewAction/SetTempDataForNavigation',
        exportPBPViewImpactData: '/PBPViewAction/ExportImpactLogData',
    }

    function dateTimeFormatter(cellvalue, options, rowObject) {
        return $.fn.fmatter.call(this, "date", cellvalue, options, rowObject);
    }

    function linkFormatter(cellvalue, options, rowObject) {
        return '<a href="#' + '">' + cellvalue + '</a>';
    }

    function setFormInstanceBuiler(formInstance) {
        this.formInstancebuilder = formInstance;
    }


    function buildGrid() {
        $(elementIDs.actionGridJQ).jqGrid('GridUnload');

        $(elementIDs.actionGridJQ).jqGrid({
            datatype: "local",
            colNames: ['ID', 'FormInstanceId', 'ElementPath', 'SOT/Medicare Field Path', 'Element ID', 'Element Name', 'Field', 'Key', 'Field Change', 'Updated By', 'Last Updated', ''],
            colModel: [{ name: 'ID', index: 'ID', key: true, hidden: true },
                       { name: 'FormInstanceId', index: 'FormInstanceId', hidden: true },
                       { name: 'ElementPath', index: 'ElementPath', hidden: true },
                       { name: 'ElementPathName', index: 'ElementPathName', width: '300' },
                       { name: 'ElementID', index: 'ElementID', hidden: true },
                       { name: 'ElementName', index: 'ElementName', hidden: true },
                       { name: 'ElementLabel', index: 'ElementLabel' },
                       { name: 'Key', index: 'Key', width: '80', align: 'left', hidden: true },
                       { name: 'Description', index: 'Description' },
                       { name: 'UpdatedBy', index: 'UpdatedBy' },
                       { name: 'UpdatedDate', index: 'UpdatedDate', formatter: dateTimeFormatter, formatoptions: JQGridSettings.DateTimeFormatterOptions, align: 'center' },
                       { name: 'ImpactedElements', index: 'ImpactedElements', hidden: true }],
            pager: '#pactionGrid',
            autowidth: true,
            shrinkToFit: false,
            height: '150',
            caption: '<div id="impactLogDiv"><button id ="btnPBPImpactLog" type="button" class="btn pull-right" style="padding:1px 5px;">View Impact History</button></div> <span id="headerImpactLog" class="ui-jqgrid-title">Impact Log</span>',
            altRows: true,
            altclass: 'alternate',
            rownumbers: true,
            rownumWidth: 40,
            rowNum: 20,
            //pgbuttons: false,
            //pgtext: null,
            loadonce: true,
            hiddengrid: false,
            hidegrid: false,
            //viewrecords: true,
            altRows: true,
            //sortname: 'ID',
            subGrid: true,
            subGridRowExpanded: function (subgrid_id, rowid) {
                var subgrid_table_id, pager_id;
                subgrid_table_id = subgrid_id + "_t";
                pager_id = "p_" + subgrid_table_id;
                $("#" + subgrid_id).html("<table id='" + subgrid_table_id + "' class='scroll'></table><div id='" + pager_id + "' class='scroll'></div>");
                var row = $(elementIDs.actionGridJQ).jqGrid("getLocalRow", rowid);
                var subGridData = row.ImpactedElements;

                $("#" + subgrid_table_id).jqGrid({
                    datatype: "local",
                    data: subGridData,
                    colNames: ['ID', 'FormInstanceID', 'FormDesignVersionID', '', '', 'ElementPath', 'PBP View Field Path', '', 'ElementID', 'ElementName', 'Field', 'Update Type', 'Field Value Update', 'PBP View Action'],
                    colModel: [{ name: 'ID', index: 'ID', key: true, hidden: true },
                               { name: 'FormInstanceID', index: 'FormInstanceID', key: false, hidden: true },
                               { name: 'FormDesignVersionID', index: 'FormDesignVersionID', hidden: true },
                               { name: 'SectionName', index: 'SectionName', hidden: true },
                               { name: 'SectionGeneratedName', index: 'SectionGeneratedName', hidden: true },
                               { name: 'ElementPath', index: 'ElementPath', hidden: true },
                               { name: 'ElementPathLabel', index: 'ElementPathLabel' },
                               { name: 'ElementGeneratedName', index: 'ElementGeneratedName', hidden: true },
                               { name: 'ElementID', index: 'ElementID', key: false, hidden: true },
                               { name: 'ElementName', index: 'ElementName', hidden: true },
                               { name: 'ElementLabel', index: 'ElementLabel' },
                               { name: 'UpdateType', index: 'UpdateType' },
                               { name: 'Description', index: 'Description' },
                               { name: 'Action', index: 'Action' }],
                    rowNum: 500000,
                    height: '100%',
                    autowidth: true,
                    shrinkToFit: true,
                    altRows: true,
                    altclass: 'alternate',
                    rownumWidth: 40,
                    pgbuttons: false,
                    hiddengrid: false,
                    hidegrid: false,
                    onSelectRow: function (id) {
                        var objRow = $(this).jqGrid('getRowData', id);
                        if (formInstancebuilder instanceof sotFormInstanceBuilder == false) {
                            handleClassicViewNavigation(objRow);
                        }
                        else if (formInstancebuilder instanceof sotFormInstanceBuilder == true) {
                            handleSOTViewNavigation(objRow);
                        }
                    }
                });
            }
        });
        $(elementIDs.btnPBPImpactLog).off('click').on('click', function () {
            getPBPViewImpactData(formInstancebuilder.anchorFormInstanceID);
        });

        $(elementIDs.actionGridJQ).jqGrid('navGrid', "#pactionGrid", { edit: false, add: false, del: false, refresh: true, search: false }, {}, {}, {});
        $(elementIDs.actionGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

        $(elementIDs.actionGridJQ).jqGrid('navButtonAdd', "#pactionGrid",
             {
                 caption: '', buttonicon: 'ui-icon-arrowstop-1-s', title: 'Export to Excel', id: 'btnExportImpactGrid', position: "first",
                 onClickButton: function () {
                     downLoadImpactLogData();
                 },
             });
        $(elementIDs.btnExportImpactGrid).css({ "position": "relative", "left": "-50px" });
    }

    function downLoadImpactLogData() {
        var impactLogDataURL = URLs.exportPBPViewImpactData;
        var stringData = "formInstanceID=" + formInstancebuilder.formInstanceId;
        stringData += "<&formInstanceName=" + formInstancebuilder.formName;
        $.downloadNew(impactLogDataURL, stringData, 'post');
    }

    function getPBPViewImpactData(formInstanceId) {
        var url = URLs.getPBPViewImpactData;
        var data = {
            formInstanceId: parseInt(formInstanceId),
        }
        var promise = ajaxWrapper.postJSONCustom(url, data, false);
        promise.done(function (xhr) {
            if (xhr.Result == ServiceResult.FAILURE) {
                messageDialog.show("Error occured while loading 'View Impact Log data");
            }
            else {
                loadGridData(xhr);
            }
        });
        promise.fail(showError);
    }

    function handleClassicViewNavigation(objRow) {
        //create callback in case of document or section is not loaded
        args = [objRow, formInstancebuilder];
        var callbackMetaData = { callback: formInstancebuilder.pbpViewAction.highlightPBPElement, callbackArgs: args };
        //get the selected document
        var formInstanceIDs = $(formInstancebuilder.elementIDs.sectionMenuContainerJQ + " select.view-menu")[0].value.split('_');

        if (formInstanceIDs[0] != objRow.FormInstanceID) {
            var viewid = objRow.FormInstanceID + "_" + objRow.FormDesignVersionID;
            $(formInstancebuilder.elementIDs.sectionMenuContainerJQ + " select.view-menu").val(viewid).trigger("change", [{ callback: callbackMetaData, sectionName: objRow.SectionName, sectionGeneratedName: objRow.SectionGeneratedName }]);
        } else {
            if (formInstancebuilder.selectedSection != objRow.SectionName) {
                formInstancebuilder.form.saveSectionData(formInstancebuilder.selectedSection, objRow.SectionName, callbackMetaData);
            } else {
                highlightElement(args);
            }
        }
    }

    function handleSOTViewNavigation(objRow) {
        if (formInstancebuilder != null && formInstancebuilder != undefined && formInstancebuilder.form.hasChanges() && !formInstancebuilder.isfolderReleased) {
            formInstancebuilder.bottomMenu.closeBottomMenu();
            yesNoConfirmDialog.show("Do you want to save the changes", function (e) {
                yesNoConfirmDialog.hide();
                if (e) {
                    formInstancebuilder.form.saveFormInstanceData(true);
                }
                else {
                    FolderLockAction.ISREPEATERACTION = 1;
                    var url = URLs.setTempDataForNavigation;
                    var data = {};
                    data.anchorFormInstanceId = formInstancebuilder.formInstanceId;
                    //data.folderVersionId = formInstancebuilder.folderData.folderVersionId;
                    data.folderVersionId = formInstancebuilder.folderVersionId;
                    data.sectionName = objRow.SectionName;
                    data.sectionGeneratedName = objRow.SectionGeneratedName;
                    var promise = ajaxWrapper.postJSON(url, data);
                    //register ajax success callback
                    promise.done(function (xhr) {
                        //show appropriate message
                        if (xhr.Result == ServiceResult.SUCCESS) {
                            var mode = formInstancebuilder.isfolderReleased == false ? true : false;
                            var redirectUrl = URLs.folderVersionDetails.replace(/{tenantId}/g, formInstancebuilder.folderData.tenantId).replace(/{folderVersionId}/g, formInstancebuilder.folderData.folderVersionId).replace(/{folderId}/g, formInstancebuilder.folderData.folderId).replace(/{foldeViewMode}/g, FolderViewMode.DefaultView);
                            redirectUrl = redirectUrl.replace(/{mode}/g, mode);
                            window.location = redirectUrl;
                        }
                    });
                    promise.fail(showError);
                }
            });
        }
        else {
            FolderLockAction.ISREPEATERACTION = 1;
            var url = URLs.setTempDataForNavigation;
            var data = {};
            data.anchorFormInstanceId = formInstancebuilder.formInstanceId;
            //data.folderVersionId = formInstancebuilder.folderData.folderVersionId;
            data.folderVersionId = formInstancebuilder.folderVersionId;
            data.sectionName = objRow.SectionName;
            data.sectionGeneratedName = objRow.SectionGeneratedName;
            var promise = ajaxWrapper.postJSON(url, data);
            //register ajax success callback
            promise.done(function (xhr) {
                //show appropriate message
                if (xhr.Result == ServiceResult.SUCCESS) {
                    var mode = formInstancebuilder.isfolderReleased == false ? true : false;
                    var redirectUrl = URLs.folderVersionDetails.replace(/{tenantId}/g, formInstancebuilder.folderData.tenantId).replace(/{folderVersionId}/g, formInstancebuilder.folderData.folderVersionId).replace(/{folderId}/g, formInstancebuilder.folderData.folderId).replace(/{foldeViewMode}/g, FolderViewMode.DefaultView);
                    redirectUrl = redirectUrl.replace(/{mode}/g, mode);
                    window.location = redirectUrl;
                }
            });
            promise.fail(showError);
        }
       
    }
    
    function highlightElement(args) {
        var gridRow = args[0];
        try {
            $("#" + "tab" + formInstancebuilder.anchorFormInstanceID + "#" + gridRow.SectionName + " #" + gridRow.ElementName + gridRow.FormInstanceID).focus();
            $('html, body').animate({
                scrollTop: $('#' + gridRow.ElementName + gridRow.FormInstanceID).offset().top
            }, 300);
        } catch (e) { }
        formInstancebuilder.bottomMenu.closeBottomMenu();
    }

    function resizeGrid() {
        $(elementIDs.actionGridJQ).jqGrid("setGridWidth", $(elementIDs.actionGridJQ).parent().width(), true);
    }

    function loadGridData(data) {
        $(elementIDs.actionGridJQ).jqGrid('clearGridData');

        if (data != undefined) {
            for (var i = 0; i < data.length; i++) {
                var item = data[i];
                $(elementIDs.actionGridJQ).jqGrid('addRowData', i, data[i]);
            }
            $(elementIDs.actionGridJQ).trigger("reloadGrid");
        }
        formInstancebuilder.bottomMenu.showPBPViewActionTab();
        $(elementIDs.actionGridJQ).jqGrid("setGridWidth", $(elementIDs.actionGridJQ).parent().width(), true);
    }

    function showError(xhr) {
        if (xhr.status == 999)
            window.location.href = '/Account/LogOn';
        else
            messageDialog.show(Common.errorMsg);
    }
    return {
        buildImpactGrid: function () {
            buildGrid();
        },
        fillImpactGrid: function (data) {
            loadGridData(data);
        },
        resizeGrid: function () {
            resizeGrid();
        },
        setFormInstanceBuilder: function (formInstance) {
            setFormInstanceBuiler(formInstance);
        },
        highlightPBPElement: function (args) {
            highlightElement(args);
        }
    }
}
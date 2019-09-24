var openDocumentDialog = function () {
    //var folderVersionId;
    var isInitialized = false;
    var isOpenDocumentGridLoaded = false;
    var viewDetails = [];
    //urls to be accessed for create/copy form.
    var URLs = {

        getAnchorDocumentList: '/FolderVersion/GetAncherFormInstanceList?tenantId=1&folderVersionId={folderVersionId}&folderViewMode={folderViewMode}',
        getUpdatedDocumentList: '/FolderVersion/GetUpdatedDocumentList?tenantId=1&folderVersionId={folderVersionId}',
        getDocumentViewList: '/FolderVersion/GetDocumentViewList?tenantId=1&anchorFormInstanceId={anchorFormInstanceId}',
        getDefaultViewFormInstanceList: '/FolderVersion/GetFormInstancesList',
        getSOTViewFormInstanceList: '/FolderVersion/GetSOTViewFormInstancesList',
        getDocumentLockStatus: '/FolderVersion/GetDocumentLockStatus?tenantId=1&folderId={folderId}',
        overrideDocumentLock: '/FolderVersion/OverrideDocumentLock?tenantId=1&folderId={folderId}',
    };

    //element ID's required for create/copy form.
    //added in Views/FolderVersion/Index.cshtml
    var elementIDs = {
        openDocumentDialogJQ: '#openDocumentDialog',
        openDocumentDialog: 'openDocumentDialog',
        openDocumentGrid: 'openDocumentGrid',
        openDocumentGridJQ: '#openDocumentGrid',
        openDocumentSelectAllCheckbox: 'selectallcheckbox',
        openDocumentSelectAllCheckboxJQ: '#selectallcheckbox',
        openDocumentAllIsIncludeCheckboxJQ: '[id^=selectcheckbox_]'

    };

    //this function is called below soon after this JS file is loaded
    function init() {
        $(document).ready(function () {
            if (isInitialized == false) {
                $(elementIDs.openDocumentDialogJQ).dialog({ autoOpen: false, zIndex: 100007, width: 990, height: 400, modal: true });
                isInitialized = true;
            }
        });
    }
    init();

    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    function loadopenDocumentGrid(folderVersionId, currentFolderInstance) {

        //if (isOpenDocumentGridLoaded == false) {

        var getAnchorDocumentListURL = URLs.getAnchorDocumentList.replace(/\{folderVersionId\}/g, folderVersionId).replace(/\{folderViewMode\}/g, currentFolderInstance.folderViewMode)
        var colArray;
        var colModel = [];
        var selectAllCheckbox = '<div style = "text-align: center;z-index:9999999"><input type="checkbox" class="check-all" id="selectallcheckbox" /></div>';
        var isSuperUser = false;
        //set column list for grid   
        if (GLOBAL.applicationName.toLowerCase() != 'ebenefitsync') {
        colArray = [selectAllCheckbox, '', '', '', '', '', '', '', '', '', '', ''];
        }else
        {
            colArray = [selectAllCheckbox, '', '', '', '', '', '', '', '', ''];
        }

        if (currentFolderInstance.folderData.RoleId == Role.TMGSuperUser || currentFolderInstance.folderData.RoleId == Role.WCSuperUser || currentFolderInstance.folderData.RoleId == Role.ClientSuperUser)
            isSuperUser = true;
        //set column models
        colModel.push({ name: 'IsIncluded', sortable: false, index: 'IsIncluded', hidden: false, align: 'center', formatter: formatterDesign, unformat: unFormatIncludedColumnDesign, width: 80, resizable: true });
        colModel.push({ name: 'AnchorFormInstanceId', index: 'AnchorFormInstanceId', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'DocId', index: 'DocId', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'FormName', index: 'FormName', editable: false, align: 'left', hidden: false });
        if (GLOBAL.applicationName.toLowerCase() != 'ebenefitsync') {
        colModel.push({ name: 'ContractCode', index: 'ContractCode', editable: false, align: 'left', hidden: false });
        colModel.push({ name: 'PlanNumber', index: 'PlanNumber', editable: false, align: 'left', hidden: false });
        }
        colModel.push({ name: 'FolderVersions', index: 'FolderVersions', editable: false, align: 'center', formatter: formatterDesign, unformat: unFormatIncludedColumnDesign, width: 120, resizable: true, hidden: currentFolderInstance.folderViewMode == FolderViewMode.DefaultView ? true : false });
        colModel.push({ name: 'DocumentViews', index: 'DocumentViews', editable: false, align: 'center', formatter: formatterDesign, unformat: unFormatIncludedColumnDesign, width: 120, resizable: true });
        colModel.push({ name: 'FormDesignVersionID', index: 'FormDesignVersionID', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'IsDocumentLocked', index: 'IsDocumentLocked', editable: true, align: 'center', hidden: true, });
        colModel.push({ name: 'UnlockDocument', index: 'UnlockDocument', sortable: false, align: 'center', formatter: formatterDesign, unformat: unFormatIncludedColumnDesign, width: 70, resizable: true, align: 'center', hidden: !isSuperUser });
        colModel.push({ name: 'LockedBy', index: 'LockedBy', editable: false, align: 'left', width: 80, hidden: false });
        //clean up the grid first - only table element remains after this

        $(elementIDs.openDocumentGridJQ).jqGrid('GridUnload');

        //adding the pager element
        $(elementIDs.openDocumentGridJQ).parent().append("<div id='p" + elementIDs.openDocumentGrid + "'></div>");

        $(elementIDs.openDocumentGridJQ).jqGrid({
            datatype: 'json',
            url: getAnchorDocumentListURL,//openDocumentDialog.URLs.getAnchorDocumentList,
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Open Document',
            pager: '#p' + elementIDs.openDocumentGrid,
            height: '200',
            rowheader: true,
            loadonce: true,
            rowNum: 10000,
            autowidth: true,
            gridview: true,
            viewrecords: true,
            altRows: true,
            altclass: 'alternate',
            gridComplete: function () {

                setOpenDocumentGridSelection();
                initializeCheckAll();
                //For select all checkbox for open document
                $(elementIDs.openDocumentSelectAllCheckboxJQ).change(function () {
                    var val = $(this).is(':checked');
                    $(elementIDs.openDocumentAllIsIncludeCheckboxJQ).filter(':not(:disabled)').prop('checked', val);
                    setSelectAllCheckbox();
                });

                $(elementIDs.openDocumentAllIsIncludeCheckboxJQ).change(function () {
                    setSelectAllCheckbox();
                });

                //To highlight Cancelled Folder versions
                var rows = $(this).getDataIDs();
                for (i = 0; i < rows.length; i++) {
                    var row = $(elementIDs.openDocumentGridJQ).getRowData(rows[i]);
                    $(row).each(function () {
                        if (row.IsDocumentLocked == "true") {
                            $('#' + rows[i]).closest('tr').find('td').css("background", "#F4D9E0");
                        }
                    });
                }
            },
            loadComplete: function () {
                $(".UNLOCKDOC").unbind('click');
                $(".UNLOCKDOC").change(function () {
                    var cellvalue = $(this).attr("id");
                    var text = $('option:selected', this).text();
                    var detailsObj = $.grep(viewDetails, function (element, index) {
                        return element.VewName == text;
                    });
                    if (detailsObj != null && detailsObj != undefined && detailsObj[0].isSectionLock == true) {
                        var id = 'unlockDocument_' + cellvalue.substring(cellvalue.indexOf('_') + 1, cellvalue.length);
                        $(id).attr('disabled', 'disabled');
                    }
                });
            }
        });

        //This GroupHeader is used for check-all View/Edit checkbox
        $(elementIDs.openDocumentGridJQ).jqGrid('setGroupHeaders', {
            useColSpanStyle: false,
            groupHeaders: [
              { startColumnName: 'IsIncluded', numberOfColumns: 1, titleText: 'Select' },
              { startColumnName: 'FormName', numberOfColumns: 1, titleText: 'Document Name' },
              { startColumnName: 'ContractCode', numberOfColumns: 1, titleText: 'Contract Number' },
              { startColumnName: 'DocId', numberOfColumns: 1, titleText: 'Doc Id' },
              { startColumnName: 'PlanNumber', numberOfColumns: 1, titleText: 'Plan Name' },
              { startColumnName: 'DocumentViews', numberOfColumns: 1, titleText: 'Document View' },
               { startColumnName: 'FolderVersions', numberOfColumns: 1, titleText: 'Folder Versions' },
               { startColumnName: 'FormDesignVersionID', numberOfColumns: 1, titleText: 'FormDesignVersionID' },
               { startColumnName: 'IsDocumentLocked', numberOfColumns: 1, titleText: 'IsDocumentLocked' },
               { startColumnName: 'UnlockDocument', numberOfColumns: 1, titleText: 'Unlock' },
               { startColumnName: 'LockedBy', numberOfColumns: 1, titleText: 'Locked By' }
            ]
        });
        jQuery("tr.ui-jqgrid-labels th:eq(0)").attr("title", "Select").tooltip({ container: 'body' });
        var pagerElement = '#p' + elementIDs.openDocumentGrid;
        //remove default buttons
        $(elementIDs.openDocumentGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false,refresh:false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();

        //$(elementIDs.openDocumentGridJQ).jqGrid('navButtonAdd', pagerElement,
        //       {
        //           caption: 'Select Updated Document',
        //           onClickButton: function () {
        //               var promise = ajaxWrapper.getJSON(URLs.getUpdatedDocumentList.replace(/{folderVersionId}/g, folderVersionId));

        //               promise.done(function (list) {
        //                   if (list.length != 0) {
        //                       for (i = 0; i < list.length; i++) {
        //                           $(elementIDs.openDocumentGridJQ).find('[name^=selectcheckbox_' + list[i].AnchorDocumentID + ']').prop('checked', true).trigger('change');
        //                           $(elementIDs.openDocumentGridJQ).find('[name^=documentviewdropdown_' + list[i].AnchorDocumentID + ']').val(list[i].FormInstanceID);
        //                       }
        //                   }
        //               });
        //               promise.fail(showError);

        //           }
        //       });

        $(elementIDs.openDocumentGridJQ).jqGrid('navButtonAdd', pagerElement,
                {
                    caption: 'Open',
                    onClickButton: function () {
                        loadOpenDocuments(currentFolderInstance);
                    }
                });


        function formatterDesign(cellValue, options, rowObject) {
            var result = cellValue;
            switch (options.colModel.index) {
                case "IsIncluded":
                    result = "<input type='checkbox' id='selectcheckbox_" + options.rowId + "' name= 'selectcheckbox_" + rowObject.AnchorFormInstanceId + "' />";
                    break;
                case "DocumentViews":

                    var Options = "";// = "<option value='0'>Select One</option>";

                    if (cellValue != null) {
                        var fillViewDetails = false;
                        if (viewDetails == null || viewDetails == undefined || viewDetails.length == 0) {
                            fillViewDetails = true;
                        }
                        for (i = 0; i < cellValue.length; i++) {
                            //if (currentFolderInstance.checkForViewAccessPermissions(cellValue[i])) {

                            // Options = Options + "<option value=" + cellValue[i].FormInstanceId + ">" + cellValue[i].FormDesignDisplayName + "</option>";

                            if (cellValue[i].FormDesignDisplayName == FormDesignName.FormDesignMedicare) {
                                Options = Options + "<option value=" + cellValue[i].FormInstanceId + " selected>" + cellValue[i].FormDesignDisplayName + "</option>";
                            }
                            else {
                                Options = Options + "<option value=" + cellValue[i].FormInstanceId + ">" + cellValue[i].FormDesignDisplayName + "</option>";
                            }

                            if (fillViewDetails) {
                                var details = { VewName: cellValue[i].FormDesignDisplayName, isSectionLock: cellValue[i].IsSectionLockEnabled };
                                viewDetails.push(details);
                            }
                        }
                        //}
                        //else {
                        //    Options = Options + "<option value=" + cellValue[i].FormInstanceId + " disabled='disabled'>" + cellValue[i].FormDesignName + "</option>";
                        //}
                    }
                    result = "<Select class='UNLOCKDOC' id='documentviewdropdown_" + options.rowId + "' name= 'documentviewdropdown_" + rowObject.AnchorFormInstanceId + "' >" + Options + "</select>";
                    //$(result).val(rowObject.AnchorFormInstanceId);
                    break;
                case "FolderVersions":
                    var Options = "";// = "<option value='0'>Select One</option>";
                    if (cellValue != null) {
                        for (i = 0; i < cellValue.length; i++) {
                            //if (currentFolderInstance.checkForViewAccessPermissions(cellValue[i])) {

                            Options = Options + "<option value=" + cellValue[i].FolderVersionId + ">" + cellValue[i].FolderVersionNumber + "</option>";
                            //}
                            //else {
                            //    Options = Options + "<option value=" + cellValue[i].FormInstanceId + " disabled='disabled'>" + cellValue[i].FormDesignName + "</option>";
                            //}
                        }
                    }

                    result = "<Select id='folderversionviewdropdown_" + options.rowId + "' name= 'folderversionviewdropdown_" + rowObject.DocId + "' >" + Options + "</select>";
                    //$(result).val(rowObject.AnchorFormInstanceId);
                    break;
                case "UnlockDocument":
                    var isSuperUser = false;
                    if (currentFolderInstance.folderData.RoleId == Role.TMGSuperUser || currentFolderInstance.folderData.RoleId == Role.WCSuperUser)
                        isSuperUser = true;
                    if (rowObject.IsDocumentLocked == true && currentFolderInstance.isEditable == true && isSuperUser == true) // Override option will be available only for SuperUser
                        result = "<input type='checkbox' id='unlockDocument_" + options.rowId + "' name= 'unlockDocument_" + rowObject.AnchorFormInstanceId + "' />";
                    else
                        result = "<input type='checkbox' id='unlockDocument_" + options.rowId + "' name= 'unlockDocument_" + rowObject.AnchorFormInstanceId + "' disabled='disabled'/>";
                    break;
            }
            return result;

        }

        //unformat the grid column based on element property
        function unFormatIncludedColumnDesign(cellValue, options, rowObject) {
            var result;

            switch (options.colModel.index) {
                case "IsIncluded":
                    result = $(this).find('#selectcheckbox_' + options.rowId).prop('checked');
                    break;
                case "DocumentViews":
                    result = $(this).find('#documentviewdropdown_' + options.rowId).val();
                    break;
                case "FolderVersions":
                    result = $(this).find('#folderversionviewdropdown_' + options.rowId).val();
                    break;
                case "UnlockDocument":
                    result = $(this).find('#unlockDocument_' + options.rowId).prop('checked');
            }

            return result;
        }
        //To initialize all check-all checkboxes
        function initializeCheckAll() {
            $(elementIDs.openDocumentSelectAllCheckboxJQ).closest(".ui-jqgrid-sortable").removeClass("ui-jqgrid-sortable");
            setSelectAllCheckbox();
        }

        function setSelectAllCheckbox() {
            if ($(elementIDs.openDocumentAllIsIncludeCheckboxJQ).filter(':not(:checked):not(:disabled)').length > 0)
                $(elementIDs.openDocumentSelectAllCheckboxJQ).prop('checked', false);
            else
                $(elementIDs.openDocumentSelectAllCheckboxJQ).prop('checked', true);
        }

        function setOpenDocumentGridSelection() {

            var openDocumentGridData = $(elementIDs.openDocumentGridJQ).getRowData();
            if (openDocumentGridData != undefined && openDocumentGridData.length > 0) {
                $.each(openDocumentGridData, function (i, e) {
                    try {
                        if (currentFolderInstance.folderViewMode == FolderViewMode.DefaultView) {
                            var openDocument = currentFolderInstance.openDocuments[e.AnchorFormInstanceId];
                            if (openDocument != undefined && openDocument.Status == OpenDocumentStatus.Open) {
                                $(elementIDs.openDocumentGridJQ).find('[name^=selectcheckbox_' + openDocument.AnchorDocumentID + ']').prop('checked', true).prop('disabled', true);
                                $(elementIDs.openDocumentGridJQ).find('[name^=documentviewdropdown_' + openDocument.AnchorDocumentID + ']').val(openDocument.SelectedViewID).prop('disabled', true);
                                $(elementIDs.openDocumentGridJQ).find('[name^=folderversionviewdropdown_' + e.DocId + ']').val(e.FolderVersions).prop('disabled', true);
                                $(elementIDs.openDocumentGridJQ).find('[name^=unlockDocument_' + openDocument.AnchorDocumentID + ']').prop('disabled', true);
                            }
                        }
                        else if (currentFolderInstance.folderViewMode == FolderViewMode.SOTView) {
                            var formInstaces = currentFolderInstance.openDocuments.filter(function (item, idx) {
                                return item.FormInstance.FormDesignName == e.FormName
                                       && item.FormInstance.DocID == e.DocId && item.FormInstance.FormDesignVersionID == e.FormDesignVersionID
                            });
                            if (formInstaces != null && formInstaces.length > 0) {
                                $.each(formInstaces, function (indx, val) {
                                    var opt = $(elementIDs.openDocumentGridJQ).find('[name^=folderversionviewdropdown_' + val.FormInstance.DocID + ']').find("option[value='" + val.FormInstance.FolderVersionID + "']");
                                    if (opt != undefined && opt != null && opt.length > 0) {
                                        if (val.Status == OpenDocumentStatus.Open)
                                            $(opt).prop('disabled', true)

                                        else
                                            $(opt).prop('disabled', false)
                                    }
                                });
                                var optCount = $(elementIDs.openDocumentGridJQ).find('[name^=folderversionviewdropdown_' + e.DocId + ']').find("option").length;
                                var disableCount = $(elementIDs.openDocumentGridJQ).find('[name^=folderversionviewdropdown_' + e.DocId + ']').find('option[disabled]').length;
                                if (optCount == disableCount) {
                                    $(elementIDs.openDocumentGridJQ).find('[name^=selectcheckbox_' + e.AnchorFormInstanceId + ']').prop('checked', true).prop('disabled', true);
                                }
                            }
                        }
                    } catch (e) {
                    }
                });
            }
        }

    }
    //format the grid column based on element property

    function loadOpenDocuments(currentFolderInstance) {
        if ($(elementIDs.openDocumentAllIsIncludeCheckboxJQ).filter(':checked:not(:disabled)').length >= 1) {
            var data = {};
            var openDocumentList = [];
            if (currentFolderInstance.folderViewMode == FolderViewMode.DefaultView) {
                openDocumentList = $(elementIDs.openDocumentGridJQ).getRowData().filter(function (row) {
                    return row.IsIncluded == true && row.DocumentViews > 0
                    && (currentFolderInstance.openDocuments == undefined
                        || currentFolderInstance.openDocuments[row.AnchorFormInstanceId] == undefined
                        || currentFolderInstance.openDocuments[row.AnchorFormInstanceId].Status == OpenDocumentStatus.Close
                        )
                });
            }
            else {
                openDocumentList = $(elementIDs.openDocumentGridJQ).getRowData().filter(function (row) {
                    return row.IsIncluded == true && row.DocumentViews > 0
                });
            }
            if (openDocumentList.length != 0) {
                var url = "";
                if (currentFolderInstance.folderViewMode == FolderViewMode.DefaultView) {
                    data = getFolderViewRequestData(openDocumentList, currentFolderInstance.folderId);
                    url = URLs.getDefaultViewFormInstanceList;
                }
                else if (currentFolderInstance.folderViewMode == FolderViewMode.SOTView) {
                    data = getSOTViewRequestData(openDocumentList, currentFolderInstance.folderId);
                    url = URLs.getSOTViewFormInstanceList;
                }
                if (data != null && data != undefined) {
                    if (currentFolderInstance.isEditable == true) {
                        var docsToOverride = getDocumentsToOverride(openDocumentList);
                        var userListToSendUnlockMsg = getDocumentsToOverrideUserList(openDocumentList);
                        if (docsToOverride.formInstanceIDs != undefined && docsToOverride.formInstanceIDs.length > 0) {
                            var overrideLockUrl = URLs.overrideDocumentLock.replace(/{folderId}/g, currentFolderInstance.folderId);
                            var promise = ajaxWrapper.postAsyncJSONCustom(overrideLockUrl, docsToOverride);
                            $.connection.hub.start().done(function () {
                                console.log("Hub Connected!");
                                $.connection.notificationHub.server.sendDocumentOverriddenBySuperUserMessage(userListToSendUnlockMsg);

                            })
                            promise.done(function (result) {
                                console.log("Overriden Document Lock");
                            });
                        }

                        var docsToLock = getDocumentsToLock(openDocumentList);
                        if (docsToLock.formInstanceIDs != undefined && docsToLock.formInstanceIDs.length > 0) {
                            var getDocStatusUrl = URLs.getDocumentLockStatus.replace(/{folderId}/g, currentFolderInstance.folderId);
                            var promise = ajaxWrapper.postAsyncJSONCustom(getDocStatusUrl, docsToLock);
                            promise.done(function (list) {
                                console.log("Document Locked!")
                            });
                        }
                    }

                    var promise = ajaxWrapper.postAsyncJSONCustom(url, data);
                    promise.done(function (list) {
                        if (list.length != 0) {
                            folderManager.getInstance().loadFolderTabs(list, false);
                        }
                    });
                    promise.fail(showError);
                    currentFolderInstance.openDocumentList = $(elementIDs.openDocumentGridJQ).getRowData();

                    $(elementIDs.openDocumentDialogJQ).dialog('close');
                    //}
                    //});
                }
            }
            else {
                messageDialog.show(DocumentDesign.openDocumentValidationMsg);
            }

        }
        else {
            messageDialog.show(DocumentDesign.openDocumentValidationMsg);
        }
    }

    function getFolderViewRequestData(openDocumentList) {
        var formInstanceIds = new Array();
        var requestData = {};
        var unlockDocumentSelections = new Array();
        var unlockDocumentStatus = {};
        for (i = 0; i < openDocumentList.length; i++) {
            formInstanceIds.push(parseInt(openDocumentList[i].DocumentViews));
            unlockDocumentStatus = { FormInstanceId: parseInt(openDocumentList[i].DocumentViews), IsOverrideDocument: openDocumentList[i].UnlockDocument, IsDocumentLocked: openDocumentList[i].IsDocumentLocked }
            unlockDocumentSelections.push(unlockDocumentStatus);
        }
        if (formInstanceIds.length != 0) {
            requestData.tenantId = 1;
            requestData.formInstanceIDs = formInstanceIds;
            requestData.unlockDocumentSelections = unlockDocumentSelections;
        }
        return requestData;
    }

    function getSOTViewRequestData(openDocumentList) {
        var documentList = [];
        var requestData = {};
        var unlockDocumentSelections = new Array();
        var unlockDocumentStatus = {};
        for (i = 0; i < openDocumentList.length; i++) {
            var data = {
                formName: openDocumentList[i].FormName,
                docID: openDocumentList[i].DocId,
                folderVersionId: openDocumentList[i].FolderVersions,
                documentViews: (parseInt(openDocumentList[i].DocumentViews)),
                formDesignVersionId: (parseInt(openDocumentList[i].FormDesignVersionID)),
                unlockDocumentStatus: { FormInstanceId: parseInt(openDocumentList[i].DocumentViews), IsOverrideDocument: openDocumentList[i].UnlockDocument, IsDocumentLocked: openDocumentList[i].IsDocumentLocked },
            };
            documentList.push(data);
            unlockDocumentSelections.push(data.unlockDocumentStatus);
        }
        if (documentList.length != 0) {
            requestData.tenantId = 1;
            requestData.documentList = documentList;
            requestData.unlockDocumentSelections = unlockDocumentSelections;
        }
        return requestData;
    }

    function getDocumentsToOverride(openDocumentList) {
        var formInstanceIds = new Array();
        var requestData = {};
        for (i = 0; i < openDocumentList.length; i++) {
            if (openDocumentList[i].UnlockDocument == true && openDocumentList[i].IsDocumentLocked == "true")
                formInstanceIds.push(parseInt(openDocumentList[i].DocumentViews));
        }
        if (formInstanceIds.length != 0) {
            requestData.tenantId = 1;
            requestData.formInstanceIDs = formInstanceIds;
        }
        return requestData;
    }
    function getDocumentsToOverrideUserList(openDocumentList) {

        var userListToSendUnlockMsg = [];
        for (i = 0; i < openDocumentList.length; i++) {
            if (openDocumentList[i].UnlockDocument == true && openDocumentList[i].IsDocumentLocked == "true")

                var notificationData = {
                    toUser: openDocumentList[i].LockedBy,
                    viewName: openDocumentList[i].PlanNumber,
                    sectionName: ""
                };
            userListToSendUnlockMsg.push(notificationData);
        }

        return userListToSendUnlockMsg;
    }

    function getDocumentsToLock(openDocumentList) {
        var formInstanceIds = new Array();
        var requestData = {};
        for (i = 0; i < openDocumentList.length; i++) {
            if (openDocumentList[i].UnlockDocument == false && openDocumentList[i].IsDocumentLocked == "false")
                formInstanceIds.push(parseInt(openDocumentList[i].DocumentViews));
        }
        if (formInstanceIds.length != 0) {
            requestData.tenantId = 1;
            requestData.formInstanceIDs = formInstanceIds;
        }
        return requestData;
    }

    function recenterDialogBox(d) {
        if (typeof d !== "object") {
            d = $(d);
        }
        d.dialog("option", "position", {
            my: "center",
            at: "center",
            of: window
        });
    }
    recenterDialogBox();

    return {
        show: function (folderVersionId, currentFolderInstance) {
            //this.folderVersionId = folderVersionId;
            $(elementIDs.openDocumentDialogJQ + ' div').removeClass('has-error');
            $(elementIDs.openDocumentDialogJQ + ' .help-block').text('');

            //refresh form name 
            $(elementIDs.openDocumentDialogJQ).dialog('option', 'title', 'Open Document');
            $(elementIDs.openDocumentDialogJQ).dialog({
                position: {
                    my: 'center',
                    at: 'center'
                },
            });
            $(elementIDs.openDocumentDialogJQ).dialog('open');
            loadopenDocumentGrid(folderVersionId, currentFolderInstance);

        },

        folderInstance: undefined
    }
}();

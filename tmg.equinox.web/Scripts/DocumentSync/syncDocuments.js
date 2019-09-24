function csSyncDocuments(currentIndex, newIndex, documentSyncContainer, documentSyncData) {
    this.elementIDs = {
        tableSyncDocuments: "syncDocumentsList",
        tableSyncDocumentsJS: "#syncDocumentsList",
        tableSyncDocumentsPagerJS: "#psyncDocumentsList"
    };

    this.targetDocuments = {
    };

    this.selectedRows = [];

    this.documentSyncContainer = documentSyncContainer;
    this.documentSyncData = documentSyncData;

    this.URLs = {
        syncDocuments: '/DocumentSync/SyncDocuments',
    };
}

csSyncDocuments.prototype.process = function () {
    //cleanup if required or second visit onwards
    this.loadSyncDocumentsGrid();
}

csSyncDocuments.prototype.loadSyncDocumentsGrid = function () {
    this.documentSyncData.IsSyncInLastVisit = false;
    var currentInstance = this;

    var sourceData = currentInstance.documentSyncData.SelectedTargetDocuments.filter(function (item) {
        return item.FolderVersionStateName == "In Progress";
    });
    $.each(sourceData, function (index, item) {
        if (item.IsSelected == undefined) {
            item.IsSelected = false;
        }
    });
    var colArray = ['Account', 'Folder', '','Compare Type', 'Folder Status', 'Folder Version Number', 'Effective Date', 'Document Name', '', '', '', '', '', 'CompCol'];
    var colModel = [];
    colModel.push({ name: 'AccountName', index: 'AccountName', editable: false, align: 'left', hidden: true });
    colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left', hidden: true });
    colModel.push({ name: 'IsSelected', index: 'IsSelected', editable: true, align: 'center', formatter: currentInstance.checkboxformat, editoptions: { value: 'true:false' }, formatoptions: { disabled: false } });
    colModel.push({
        name: 'CompareType', index: 'CompareType', editable: true, align: 'center', search: false, formatter: currentInstance.comparetypeformat, unformat: currentInstance.comparetypeunformat, formatoptions: { disabled: false }
    });
    colModel.push({ name: 'FolderVersionStateName', index: 'FolderVersionStateName', editable: false, align: 'left' });
    colModel.push({ name: 'VersionNumber', index: 'VersionNumber', editable: false, align: 'right' });
    colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, align: 'right' });
    colModel.push({ name: 'FormInstanceName', index: 'FormInstanceName', editable: false, align: 'left' });
    colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true });
    colModel.push({ name: 'AccountID', index: 'AccountID', hidden: true });
    colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true });
    colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', hidden: true });
    colModel.push({ name: 'FormInstanceID', index: 'FormInstanceID', hidden: true, key: true });
    colModel.push({ name: 'CompCol', index: 'CompCol' });
    $(currentInstance.elementIDs.tableSyncDocumentsJS).jqGrid('GridUnload');
    $(currentInstance.elementIDs.tableSyncDocumentsJS).parent().append("<div id='p" + currentInstance.elementIDs.tableSyncDocuments + "'></div>");
    $(currentInstance.elementIDs.tableSyncDocumentsJS).jqGrid({
        datatype: 'local',
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Selected Documents',
        height: '300',
        data: sourceData,
        width: 800,
        rowNum: 50,
        rowList: [50],
        ignoreCase: true,
        autowidth: true,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        pager: currentInstance.elementIDs.tableSyncDocumentsPagerJS,
        sortname: 'FormInstanceName',
        altclass: 'alternate',
        grouping: true,
        groupingView: {
            groupField: ['CompCol'],
            groupColumnShow: [false]
        },
        gridComplete: function () {
            $(currentInstance.elementIDs.tableSyncDocumentsJS + " input:checkbox").click(function () {
                var rowID = this.id;
                if ($(this).is(":checked") == true) {
                    var rowData = $(currentInstance.elementIDs.tableSyncDocumentsJS).jqGrid('getRowData', rowID);
                    rowData.IsSelected = true;
                    currentInstance.selectedRows.push(rowData);
                }
                else {
                    currentInstance.selectedRows = currentInstance.selectedRows.filter(function (item) {
                        return item.FormInstanceID != rowID;
                    });
                }
            });
        }
    });
    var pagerElement = currentInstance.elementIDs.tableSyncDocumentsPagerJS;
    //remove default buttons
    $(currentInstance.elementIDs.tableSyncDocumentsJS).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    $(currentInstance.elementIDs.tableSyncDocumentsJS).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
    $(currentInstance.elementIDs.tableSyncDocumentsJS).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-newwin', title: 'Sync Selected Documents',
            onClickButton: function () {
                if (currentInstance.selectedRows.length > 0) {
                    currentInstance.syncSelectedDocuments(currentInstance.selectedRows);
                }
                else {
                    messageDialog.show("Please select documents to Sync.");
                }
            }
        });
}

csSyncDocuments.prototype.checkboxformat = function (cellValue, options, rowObject) {
    var checked = cellValue == true ? "checked='checked' " : "";
    return "<input type='checkbox' " + checked + " value='" + cellValue + "' id='" + rowObject.FormInstanceID + "'/>";
}

csSyncDocuments.prototype.syncSelectedDocuments = function (selectedRows) {
    var currentInstance = this;
    var sourceData = this.documentSyncData.SourceDocument;
    var targetData = selectedRows;
    var macroData = this.documentSyncData.SelectedMacro.Template;
    var macroId = this.documentSyncData.SelectedMacro.MacroID;
    var tenantId = 1;
    var jsonData = {
        tenantId: tenantId,
        source: JSON.stringify(sourceData),
        targets: JSON.stringify(targetData),
        macro: JSON.stringify(macroData),
        macroId: macroId
    };
    var promise = ajaxWrapper.postJSON(this.URLs.syncDocuments, jsonData);
    promise.done(function (xhr) {
        currentInstance.showSyncMessage(xhr);
        currentInstance.updateSelectedTargetDocuments(xhr);
        currentInstance.loadSyncDocumentsGrid();
        currentInstance.documentSyncData.IsSyncInLastVisit = true;
    });
}



csSyncDocuments.prototype.showSyncMessage = function (result) {
    var resultContent = "";
    $.each(result, function (index, folderVersion) {
        resultContent = resultContent + "<table class = 'table'>";
        var accountName = folderVersion.AccountName;
        var folderName = folderVersion.FolderName;
        var folderVersionNumber = folderVersion.VersionNumber;
        resultContent = resultContent + "<tr><th>Account Name : " + accountName + ",   Folder Name : " + folderName + ",   Folder Version : " + folderVersionNumber + "</th></tr>";
        if (folderVersion.IsFolderLock == true) {
            resultContent = resultContent + "<tr><td>" + "Not synchronized as the Folder is Locked for use." + "</td></tr>";
        }
        else {
            if (folderVersion.SyncStatus == "NoSync") {
                resultContent = resultContent + "<tr><td>" + "Not synchronized as Documents are already in Sync for the criteria selected." + "</td></tr>";
            }
            else {
                if (folderVersion.FormInstances != null && folderVersion.FormInstances.length > 0) {
                    $.each(folderVersion.FormInstances, function (index, formInstance) {
                        if (formInstance.SyncStatus == "Sync") {
                            resultContent = resultContent + "<tr><td>Sync for Document <b>" + formInstance.FormInstanceName + "</b>" + " has been processed.</td></tr>"
                        }
                        else {
                            resultContent = resultContent + "<tr><td>Document <b>" + formInstance.FormInstanceName + "</b>" + " is already in Sync for the Sync criteria selected.</td></tr>"
                        }
                    });
                }
            }
        }
        resultContent = resultContent + "</table>";
    });
    selectSyncDocumentsDialog.show(this, resultContent);
}

csSyncDocuments.prototype.comparetypeformat = function (cellValue, options, rowObject) {
    var selectBox = "<select id='CompareType_" + rowObject.FormInstanceID + "'>";
    selectBox = selectBox + "<option>Common Sync</option>";
    selectBox = selectBox + "<option>Full Sync</option>";
    selectBox = selectBox + "</select>";
    return selectBox;
}

csSyncDocuments.prototype.comparetypeunformat = function (cellValue, options, cell) {
    var selected = $('#' + 'CompareType_' + options.rowId).val();
    return selected;
}

csSyncDocuments.prototype.updateSelectedTargetDocuments = function (result) {
    var currentInstance = this;
    $.each(result, function (index, folderVersion) {
        if (folderVersion.FormInstances != null && folderVersion.FormInstances.length > 0) {
            $.each(folderVersion.FormInstances, function (ind, formInstance) {
                var selRow = currentInstance.documentSyncData.SelectedTargetDocuments.filter(function (data) {
                    return data.FormInstanceID == formInstance.FormInstanceID;
                });
                if (formInstance.SyncStatus == "Sync") {
                    selRow[0].FolderVersionStateName = "Baselined";
                }
            });
        }
    });
}

var selectSyncDocumentsDialog = function () {
    var elementIDs = {
        syncDialogJQ: "#syncDocumentsDialog"
    }

    function init() {
        //register dialog 
        $(elementIDs.syncDialogJQ).dialog({
            autoOpen: false,
            height: 400,
            width: 600,
            modal: true
        });
    }
    //initialize the dialog after this js is loaded
    init();

    return {
        show: function (dialogParent, syncResult) {
            $(elementIDs.syncDialogJQ).dialog('option', 'title', "Document Sync Results");
            $(elementIDs.syncDialogJQ).html(syncResult);
            $(elementIDs.syncDialogJQ).dialog("open");
        }
    }
}();

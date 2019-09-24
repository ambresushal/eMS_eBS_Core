function csSelectTargetDocuments(currentIndex, newIndex, documentSyncData) {
    this.elementIDs = {
        tableTargetDocuments: "targetDocumentList",
        tableSelectedTargetDocuments: "targetDocumentSelectedList",
        tableTargetDocumentsJS: "#targetDocumentList",
        tableSelectedTargetDocumentsJS: "#targetDocumentSelectedList",
        tableTargetDocumentsPagerJS: "#ptargetDocumentList",
        tableSelectedTargetDocumentsPagerJS: "#ptargetDocumentSelectedList"
    };

    this.documentSyncData = documentSyncData;
    this.targetDocuments = {
    };

    this.URLs = {
        docSearch: '/ConsumerAccount/GetDocumentsList?tenantId=1&formDesignID={formDesignID}&formInstanceId={formInstanceId}'
    };
};

csSelectTargetDocuments.prototype.process = function () {
    //cleanup if required or second visit onwards
    console.log("Process Select Target Documents");
    if (this.documentSyncData.SelectedTargetDocuments == null) {
        this.loadTargetDocumentsGrid();
        this.loadTargetSelectedDocumentsGrid();
    }
    else {
        this.loadTargetSelectedDocumentsGrid();
    }
}

csSelectTargetDocuments.prototype.loadTargetDocumentsGrid = function () {
    var currentInstance = this;
    var colArray = ['Account', 'Folder', 'Folder Version Number', 'Folder Status', 'Effective Date', 'Document Name', '', '', '', '', ''];
    var colModel = [];
    colModel.push({ name: 'AccountName', index: 'AccountName', editable: false, align: 'left' });
    colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left' });
    colModel.push({ name: 'VersionNumber', index: 'VersionNumber', editable: false, align: 'right' });
    colModel.push({ name: 'FolderVersionStateName', index: 'FolderVersionStateName', editable: false, align: 'left' });
    colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' });
    colModel.push({ name: 'FormInstanceName', index: 'FormInstanceName', editable: false, align: 'left' });
    colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true });
    colModel.push({ name: 'AccountID', index: 'AccountID', hidden: true });
    colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true });
    colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', hidden: true });
    colModel.push({ name: 'FormInstanceID', index: 'FormInstanceID', hidden: true, key: true });

    $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid('GridUnload');
    $(currentInstance.elementIDs.tableTargetDocumentsJS).parent().append("<div id='p" + currentInstance.elementIDs.tableTargetDocuments + "'></div>");
    var url = currentInstance.URLs.docSearch;
    url = url.replace(/\{formDesignID\}/g, this.documentSyncData.SourceDocument.FormDesignID).replace(/\{formInstanceId\}/g, this.documentSyncData.SourceDocument.FormInstanceID);
    $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid({
        url: url,
        datatype: 'json',
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Select Documents',
        height: '150',
        width: 800,
        rowNum: 50,
        rowList: [50],
        ignoreCase: true,
        autowidth: true,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        pager: currentInstance.elementIDs.tableTargetDocumentsPagerJS,
        sortname: 'FormInstanceName',
        altclass: 'alternate',
        gridComplete: function () {
            //To set first row of grid as selected.
            console.log("grid complete");

        }
    });
    var pagerElement = currentInstance.elementIDs.tableTargetDocumentsPagerJS;
    //remove default buttons
    $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
    $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-plus', title: 'Add',
            onClickButton: function () {
                var selectedRowId = $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid('getGridParam', 'selrow');
                var rowData = $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid('getRowData', selectedRowId);
                if (selectedRowId == null) {
                    messageDialog.show("Please select a document to sync.");
                    return;
                }
                var rowIDs = $(currentInstance.elementIDs.tableSelectedTargetDocumentsJS).jqGrid("getDataIDs");
                if (rowIDs.length >= 10) {
                    messageDialog.show("Only 10 documents can be added for Sync at a time.");
                    return;
                }
                var matchID = rowIDs.filter(function (rowID) {
                    return selectedRowId == rowID;
                });
                if (matchID.length > 0) {
                    messageDialog.show(rowData.FormInstanceName + " for Folder Version Number " + rowData.VersionNumber + " is already added for Sync.");
                    return;
                }
                else {
                    rowData.CompCol = "Account Name : " + rowData.AccountName.trim() + " , Folder Name : " + rowData.FolderName + " , Folder Version : " + rowData.VersionNumber;
                    if (currentInstance.documentSyncData.SelectedTargetDocuments == null) {
                        currentInstance.documentSyncData.SelectedTargetDocuments = [];
                    }
                    currentInstance.documentSyncData.SelectedTargetDocuments.push(rowData);
                    currentInstance.loadTargetSelectedDocumentsGrid();
                }
            }
        });
}

csSelectTargetDocuments.prototype.loadTargetSelectedDocumentsGrid = function () {
    var currentInstance = this;
    if (currentInstance.documentSyncData.SelectedTargetDocuments != null) {
        var colArray = ['Account', 'Folder', 'Folder Version Number', 'Folder Status', 'Effective Date', 'Document Name', '', '', '', '', '', 'CompCol'];
        var colModel = [];
        colModel.push({ name: 'AccountName', index: 'AccountName', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'VersionNumber', index: 'VersionNumber', editable: false, align: 'right', hidden: true });
        colModel.push({ name: 'FolderVersionStateName', index: 'FolderVersionStateName', editable: false, align: 'left' });
        colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, align: 'center' });
        colModel.push({ name: 'FormInstanceName', index: 'FormInstanceName', editable: false, align: 'left' });
        colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true });
        colModel.push({ name: 'AccountID', index: 'AccountID', hidden: true });
        colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true });
        colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', hidden: true });
        colModel.push({ name: 'FormInstanceID', index: 'FormInstanceID', hidden: true, key: true });
        colModel.push({ name: 'CompCol', index: 'CompCol' });

        $(currentInstance.elementIDs.tableSelectedTargetDocumentsJS).jqGrid('GridUnload');
        $(currentInstance.elementIDs.tableSelectedTargetDocumentsJS).parent().append("<div id='p" + currentInstance.elementIDs.tableSelectedTargetDocuments + "'></div>");
        $(currentInstance.elementIDs.tableSelectedTargetDocumentsJS).jqGrid({
            datatype: 'local',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Selected Documents',
            height: '150',
            data: currentInstance.documentSyncData.SelectedTargetDocuments,
            width: 800,
            rowNum: 50,
            rowList: [50],
            ignoreCase: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: currentInstance.elementIDs.tableSelectedTargetDocumentsPagerJS,
            sortname: 'FormInstanceName',
            altclass: 'alternate',
            grouping: true,
            groupingView: {
                groupField: ['CompCol'],
                groupColumnShow: [false]
            },
            gridComplete: function () {
                //To set first row of grid as selected.
                console.log("grid complete");
            }
        });
        var pagerElement = currentInstance.elementIDs.tableSelectedTargetDocumentsPagerJS;
        //remove default buttons
        $(currentInstance.elementIDs.tableSelectedTargetDocumentsJS).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(currentInstance.elementIDs.tableSelectedTargetDocumentsJS).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
        $(currentInstance.elementIDs.tableSelectedTargetDocumentsJS).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon-minus', title: 'Remove',
                onClickButton: function () {
                    var selectedRowId = $(currentInstance.elementIDs.tableSelectedTargetDocumentsJS).jqGrid('getGridParam', 'selrow');
                    var rowData = $(currentInstance.elementIDs.tableSelectedTargetDocumentsJS).jqGrid('getRowData', selectedRowId);
                    if (selectedRowId == null) {
                        messageDialog.show("Please select a document to remove from Sync.");
                        return;
                    }
                    var rowIDs = $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid("getDataIDs");
                    var matchID = rowIDs.filter(function (rowID) {
                        return selectedRowId == rowID;
                    });
                    $(currentInstance.elementIDs.tableSelectedTargetDocumentsJS).delRowData(selectedRowId);
                    if (matchID.length == 0) {
                        $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid('addRowData', selectedRowId, rowData);
                    }
                    currentInstance.documentSyncData.SelectedTargetDocuments = $(currentInstance.elementIDs.tableSelectedTargetDocumentsJS).jqGrid('getRowData');
                    $(currentInstance.elementIDs.tableSelectedTargetDocumentsJS).trigger("reloadGrid");
                }
            });
    }
}


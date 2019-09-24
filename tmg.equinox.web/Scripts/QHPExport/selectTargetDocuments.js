function selectDocuments() {
    this.elementIDs = {
        tableTargetDocuments: "targetDocumentList",
        tableTargetDocumentsJS: "#targetDocumentList",
        tableTargetDocumentsPagerJS: "#ptargetDocumentList",

        tableSelectedTargetDocuments: "targetDocumentSelectedList",
        tableSelectedTargetDocumentsJS: "#targetDocumentSelectedList",
        tableSelectedTargetDocumentsPagerJS: "#ptargetDocumentSelectedList",

        tableReportQueue: "reportQueue",
        tableReportQueueJS: "#reportQueue",
        tableReportQueuePagerJS: "#preportQueue",

        chkOffExchangeOnly: "#chkOffExchangeOnly"

    };

    this.idsOfSelectedRows = [];
    this.selectedDocuments = [];
    this.targetDocuments = {
    };

    this.URLs = {
        docSearch: '/QHPExport/GetDocumentsList?tenantId=1',
        generateQHP: '/QHPExport/GenerateQHPReport',
        updateQHPReportQueue: '/QHPExport/UpdateQHPReportQueue',
        getQHPReportQueue: '/QHPExport/GetQHPReportQueue?tenantId=1',
    };

    refreshInterval = setInterval(function () { AutoRefreshMigrationQueue("#reportQueue", "#spnTimer"); }, 1000);
};

selectDocuments.prototype.process = function () {
    //cleanup if required or second visit onwards
    console.log("Process Select Target Documents");
    this.loadTargetDocumentsGrid();
    this.loadQHPReportQueueGrid();
}

selectDocuments.prototype.loadTargetDocumentsGrid = function () {
    var currentInstance = this;
    var colArray = ['Document Name', 'Account', 'Folder Name', 'Version Number', 'Status', 'Effective Date', 'Market Type', 'State', 'Variant', '', '', '', '', '', ''];
    var colModel = [];
    colModel.push({ name: 'FormInstanceName', index: 'FormInstanceName', editable: false, align: 'left' });
    colModel.push({ name: 'AccountName', index: 'AccountName', hidden: true, editable: false, align: 'left' });
    colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left' });
    colModel.push({ name: 'VersionNumber', index: 'VersionNumber', editable: false, align: 'center', width: 90 });
    colModel.push({ name: 'FolderVersionStateName', index: 'FolderVersionStateName', editable: false, align: 'left', width: 60 });
    colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, width: 90, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' });
    colModel.push({ name: 'MarketType', index: 'MarketType', editable: false, align: 'left', width: 80 });
    colModel.push({ name: 'State', index: 'State', editable: false, align: 'center', width: 60 });
    colModel.push({ name: 'CSRVariationType', index: 'CSRVariationType', editable: false, align: 'left' });
    colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true });
    colModel.push({ name: 'AccountID', index: 'AccountID', hidden: true });
    colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true });
    colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', hidden: true });
    colModel.push({ name: 'FormInstanceID', index: 'FormInstanceID', hidden: true, key: true });
    colModel.push({ name: 'DocumentId', index: 'DocumentId', hidden: true });

    $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid('GridUnload');
    $(currentInstance.elementIDs.tableTargetDocumentsJS).parent().append("<div id='p" + currentInstance.elementIDs.tableTargetDocuments + "'></div>");
    var url = currentInstance.URLs.docSearch;
    var selectedRows = {};
    $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid({
        url: url,
        datatype: 'json',
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Select Documents',
        height: '300',
        rowNum: 50,
        rowList: [50, 100, 150, 200],
        ignoreCase: true,
        shrinkToFit: true,
        autowidth: true,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        multiselect: true,
        pager: currentInstance.elementIDs.tableTargetDocumentsPagerJS,
        sortname: 'FormInstanceName',
        altclass: 'alternate',
        gridComplete: function () {
            for (var i = 0; i < currentInstance.idsOfSelectedRows.length; i++) {
                $(currentInstance.elementIDs.tableTargetDocumentsJS).setSelection(currentInstance.idsOfSelectedRows[i], true);
            }
        },
        onSelectRow: function (id, status, event) {
            var index = $.inArray(id, currentInstance.idsOfSelectedRows);
            if (status == true) {
                var isvalid = currentInstance.validateSelection(id, status);
                if (!isvalid) {
                    $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid('setSelection', id);
                } else {
                    if (index < 0) {
                        currentInstance.idsOfSelectedRows.push(id);
                        var rowData = $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid('getRowData', id);
                        currentInstance.selectedDocuments.push(rowData);
                    }
                }
            } else {
                if (index >= 0) {
                    currentInstance.idsOfSelectedRows.splice(index, 1);
                    currentInstance.selectedDocuments.splice(index, 1);
                }
            }
        }
    });
    var pagerElement = currentInstance.elementIDs.tableTargetDocumentsPagerJS;
    //remove default buttons
    $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

    /*  Need to work on the following code. commenting line for CFG push*/
    $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid('navButtonAdd', pagerElement,
        {

            caption: '', buttonicon: 'ui-icon-arrowthick-1-s', title: 'Queue Report',
            onClickButton: function () {
                var canQueue = true;
                var hasOffExchangePlans = true;
                var offExchangeOnlyFlag = $("#chkOffExchangeOnly").prop("checked");
                //var ids = $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid('getGridParam', 'selarrrow');
                var selectedRows = [];
                if (currentInstance.idsOfSelectedRows.length > 0) {
                    if (offExchangeOnlyFlag == true) {
                        hasOffExchangePlans = false;
                        for (var j = 0; j < currentInstance.idsOfSelectedRows.length; j++) {
                            var rowData = currentInstance.selectedDocuments[j];
                            if (rowData.CSRVariationType == "Off the exchange" || rowData.CSRVariationType == "Both (Display as On/Off Exchange)") {
                                hasOffExchangePlans = true;
                                break;
                            }
                        }
                    }
                    if (hasOffExchangePlans) {
                        for (var j = 0; j < currentInstance.idsOfSelectedRows.length; j++) {
                            if (canQueue == true) {
                                var rowData = currentInstance.selectedDocuments[j];
                                rowData.CompCol = "Provider Name : " + rowData.FolderName + " , Version : " + rowData.VersionNumber;
                                selectedRows.push(rowData);
                            }
                        }
                        if (canQueue) {
                            currentInstance.updateQHPReportQueue(selectedRows);
                            $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid('resetSelection');
                            selectedRows.push(rowData);
                        }
                    } else {
                        messageDialog.show("Please select at least one document with Off Exchange variant to queue report.");
                    }
                }
                else {
                    messageDialog.show("Please select document(s) to queue report.");
                }
                location:;
            }
        });

    //$(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid('navButtonAdd', pagerElement,
    //    {
    //        caption: '', buttonicon: 'ui-icon-arrowthick-1-s', title: 'Download',
    //        onClickButton: function () {
    //            //var ids = $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid('getGridParam', 'selarrrow');
    //            var selectedRows = [];
    //            if (currentInstance.idsOfSelectedRows.length > 0) {
    //                for (var j = 0; j < currentInstance.idsOfSelectedRows.length; j++) {
    //                    var rowData = currentInstance.selectedDocuments[j];
    //                    selectedRows.push(rowData);
    //                }
    //                currentInstance.generateQHP(selectedRows);
    //            }
    //            else {
    //                messageDialog.show("Please select documents to export.");
    //            }
    //        }
    //    });
}

selectDocuments.prototype.validateSelection = function (id, status) {
    var currentInstance = this;
    var isValid = true;
    //get list of selected rows
    var selectedRows = currentInstance.selectedDocuments;

    //get the current row
    var currentRow = $(currentInstance.elementIDs.tableTargetDocumentsJS).jqGrid('getRowData', id);

    for (var index = 0; index < selectedRows.length; index++) {
        var data = selectedRows[index];
        if (currentRow["MarketType"] != data["MarketType"]) {
            messageDialog.show("Documents having different Market Type cannot be combined together.");
            isValid = false;
            break;
        }

        if (currentRow["DocumentId"] == data["DocumentId"] && currentRow["FormInstanceID"] != data["FormInstanceID"]) {
            messageDialog.show("Same document from different versions cannot be combined together.");
            isValid = false;
            break;
        }

        if (currentRow["VersionNumber"].substring(0, 3) != data["VersionNumber"].substring(0, 3)) {
            messageDialog.show("Documents from different years cannot be combined together.");
            isValid = false;
            break;
        }

        if (currentRow["State"].substring(0, 3) != data["State"].substring(0, 3)) {
            messageDialog.show("Documents from different states cannot be combined together.");
            isValid = false;
            break;
        }
    }

    return isValid;
}

selectDocuments.prototype.generateQHP = function (selectedRows) {
    var currentInstance = this;
    var targetData = JSON.stringify(selectedRows);
    var stringData = "tenantId=" + 1;
    stringData += "<&targets=" + encodeURIComponent(targetData);
    stringData += "<&offExchangeOnly=" + $(currentInstance.elementIDs.chkOffExchangeOnly).is(':checked');
    $.downloadNew(currentInstance.URLs.generateQHP, stringData, 'post');
}

selectDocuments.prototype.updateQHPReportQueue = function (selectedRows) {
    var currentInstance = this;
    var targetData = JSON.stringify(selectedRows);

    var data = {
        tenantId: 1,
        targets: encodeURIComponent(targetData),
        offExchangeOnly: $(currentInstance.elementIDs.chkOffExchangeOnly).is(':checked'),
    }

    var promise = ajaxWrapper.postJSON(currentInstance.URLs.updateQHPReportQueue, data);
    promise.done(function (xhr) {
        if (xhr.Result == ServiceResult.SUCCESS) {
            $(currentInstance.elementIDs.tableReportQueueJS).trigger("reloadGrid");
            $(currentInstance.elementIDs.chkOffExchangeOnly).prop("checked", false);
            messageDialog.show("QHP Report queue updated sucessfully!");
        } else {
            messageDialog.show("Error occured while updating QHP Report queue");
        }
    });
}

selectDocuments.prototype.loadQHPReportQueueGrid = function () {
    var currentInstance = this;

    var colArray = null;
    colArray = ['Queue ID', 'Queued Date', 'Added By', 'Added Date', 'Download', 'Status'];

    var colModel = [];
    colModel.push({ key: true, hidden: false, name: 'QueueID', index: 'QueueID', width: 80, align: 'left', hidden: false, editable: false });
    colModel.push({ key: false, name: 'QueuedDate', index: 'QueuedDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateTimeFormatterOptions });
    colModel.push({ key: false, name: 'AddedBy', index: 'AddedBy', editable: false });
    colModel.push({ key: false, name: 'AddedDate', index: 'AddedDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateTimeFormatterOptions });
    colModel.push({ key: false, name: 'DocumentLocation', index: 'DocumentLocation', align: 'center', width: 50, search: false, editable: false, formatter: addDownloadDocumentLink });
    colModel.push({ key: false, name: 'Status', index: 'Status', editable: false, formatter: processStatusFormmater });

    $(currentInstance.elementIDs.tableReportQueueJS).jqGrid('GridUnload');
    $(currentInstance.elementIDs.tableReportQueueJS).parent().append("<div id='p" + currentInstance.elementIDs.tableReportQueue + "'></div>");

    $(currentInstance.elementIDs.tableReportQueueJS).jqGrid({
        url: currentInstance.URLs.getQHPReportQueue,
        datatype: 'json',
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Queue Status',
        height: '300',
        rowNum: 50,
        rowList: [50],
        loadonce: false,
        ignoreCase: true,
        headertitles: true,
        shrinkToFit: true,
        autowidth: true,
        viewrecords: true,
        sortname: 'QueueID',
        sortorder: 'desc',
        pager: currentInstance.elementIDs.tableReportQueuePagerJS,
        gridComplete: function () {

        },
    });

    var pagerElement = currentInstance.elementIDs.tableReportQueuePagerJS;

    //remove default buttons
    $(currentInstance.elementIDs.tableReportQueueJS).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    $(currentInstance.elementIDs.tableReportQueueJS).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

    function addDownloadDocumentLink(cellvalue, options, rowObject) {
        var lnkDownloadDocument = '';
        if (cellvalue != undefined && cellvalue != '' && cellvalue != -1) {
            lnkDownloadDocument = "<span class='ui-icon ui-icon-document' style='cursor: pointer;justify-content: center;margin-left:40%;padding:0px' onclick=\"downloadQHPReport(" + rowObject.QueueID + ");\"></span>";
            //lnkDownloadDocument = "<span class='ui-icon ui-icon-document' style='cursor: pointer;justify-content: center;' onclick=\"downloadQHPReport(" + rowObject.QueueID + ");\"></span>";
        }
        return lnkDownloadDocument;
    }
}

function downloadQHPReport(QueueID) {
    var currentInstance = this;
    var stringData = "QueueID=" + QueueID;
    $.downloadNew('/QHPExport/DownloadQHPReport', stringData, 'post');
}

function DisplayErrorMessage(QueueID, popupTitle) {
    if (QueueID != undefined && QueueID != '' && QueueID != -1) {
        var url = '/QHPExport/GetErrorDescription?QueueID=' + QueueID;
        var promise = ajaxWrapper.getJSONSync(url);
        promise.done(function (result) {
            if (result != null) {
                $('#messagedialog').dialog({
                    title: popupTitle,
                    height: 125,
                    width: 500,
                    zIndex: 99999
                });
                messageDialog.show(result.Message)
            }
        });
    }
}

function processStatusFormmater(cellvalue, options, rowObject) {
    switch (cellvalue) {
        case 'New':
            cellvalue = '<span style="">' + 'Queued' + '</span>';
            break;
        case 'InProgress':
            cellvalue = '<span style="color:blue">' + 'In Progress' + '</span>';
            break;
        case 'Fail':
            cellvalue = '<a href="javascript:DisplayErrorMessage(' + rowObject.QueueID + ',\'Error Details\');"><span style="color:red"><b>' + 'Errored' + '</b></span></a>';
            break;
        case 'Info':
            cellvalue = '<a href="javascript:DisplayErrorMessage(' + rowObject.QueueID + ',\'Queue Information\');"><span style="color:orange"><b>' + 'Information' + '</b></span></a>';
            break;
        case 'Completed':
            cellvalue = '<span style="color:green">' + 'Complete' + '</span>';
            break;
    }
    return cellvalue;
}
function csCompareDocuments(currentIndex, newIndex, documentSyncData) {
    this.elementIDs = {
        tableCompareDocuments: "compareDocumentsList",
        tableCompareDocumentsJS: "#compareDocumentsList",
        tableCompareDocumentsPagerJS: "#pcompareDocumentsList"
    };

    this.targetDocuments = {
    };

    this.documentSyncData = documentSyncData;

    this.URLs = {
        compareDocuments: '/DocumentSync/CompareReport',
    };
}

csCompareDocuments.prototype.process = function () {
    //cleanup if required or second visit onwards
    console.log("Process Compare Documents");
    this.loadSelectedDocumentsGrid();
}

csCompareDocuments.prototype.loadSelectedDocumentsGrid = function () {
    var currentInstance = this;
    var colArray = ['Account', 'Folder', 'Folder Status', 'Compare Filter','Folder Version Number', 'Effective Date', 'Document Name', '', '', '', '', '', 'CompCol'];
    var colModel = [];
    colModel.push({ name: 'AccountName', index: 'AccountName', editable: false, align: 'left', hidden: true });
    colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left', hidden: true });
    colModel.push({ name: 'FolderVersionStateName', index: 'FolderVersionStateName', editable: false, align: 'left' });
    colModel.push({ name: 'CompareFilter', index: 'CompareFilter', editable: true, align: 'center', search: false,formatter: currentInstance.comparefilterboxformat, editoptions: { value: 'Matches Only:Mismatches Only;Both' }, formatoptions: { disabled: false } });
    colModel.push({ name: 'VersionNumber', index: 'VersionNumber', editable: false, align: 'right' });
    colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, align: 'right' });
    colModel.push({ name: 'FormInstanceName', index: 'FormInstanceName', editable: false, align: 'left' });
    colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true });
    colModel.push({ name: 'AccountID', index: 'AccountID', hidden: true });
    colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true });
    colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', hidden: true });
    colModel.push({ name: 'FormInstanceID', index: 'FormInstanceID', hidden: true, key: true });
    colModel.push({ name: 'CompCol', index: 'CompCol' });

    $.each(currentInstance.documentSyncData.SelectedTargetDocuments, function (index, item) {
        if (item.CompareFilter == null) {
            item.CompareFilter = "Mismatches Only";
        }
    });
    $(currentInstance.elementIDs.tableCompareDocumentsJS).jqGrid('GridUnload');
    $(currentInstance.elementIDs.tableCompareDocumentsJS).parent().append("<div id='p" + currentInstance.elementIDs.tableCompareDocuments + "'></div>");
    $(currentInstance.elementIDs.tableCompareDocumentsJS).jqGrid({
        datatype: 'local',
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Selected Documents',
        height: '300',
        data: currentInstance.documentSyncData.SelectedTargetDocuments,
        width: 800,
        rowNum: 50,
        rowList: [50],
        ignoreCase: true,
        autowidth: true,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        pager: currentInstance.elementIDs.tableCompareDocumentsPagerJS,
        sortname: 'FormInstanceName',
        altclass: 'alternate',
        grouping: true,
        groupingView: {
            groupField: ['CompCol'],
            groupColumnShow: [false]
        },
        gridComplete: function () {
            $(currentInstance.elementIDs.tableCompareDocumentsJS + " select").click(function () {
                var id = this.id.replace("CompareFilter", "");
                $(currentInstance.elementIDs.tableCompareDocumentsJS).setSelection(id, false);
            });
        }
    });
    var pagerElement = currentInstance.elementIDs.tableCompareDocumentsPagerJS;
    //remove default buttons
    $(currentInstance.elementIDs.tableCompareDocumentsJS).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    $(currentInstance.elementIDs.tableCompareDocumentsJS).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
    $(currentInstance.elementIDs.tableCompareDocumentsJS).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-arrowstop-1-s', title: 'Download Report',
            onClickButton: function () {
                var selectedRowId = $(currentInstance.elementIDs.tableCompareDocumentsJS).jqGrid('getGridParam', 'selrow');
                var rowData = $(currentInstance.elementIDs.tableCompareDocumentsJS).jqGrid('getRowData', selectedRowId);
                if (selectedRowId == null) {
                    messageDialog.show("Please select a document to Compare.");
                }
                else {
                    var selectElement = $(currentInstance.elementIDs.tableCompareDocumentsJS + " #CompareFilter" + selectedRowId);
                    if (selectElement != null && selectElement.length > 0) {
                        var selElem = selectElement[0];
                        if (selElem.selectedIndex == 0) {
                            rowData.CompareFilter =  "Mismatches only";
                        }
                        else if (selElem.selectedIndex == 1) {
                            rowData.CompareFilter = "Matches only";
                        }
                        else {
                            rowData.CompareFilter = "Both";
                        }
                    }
                    currentInstance.generateReport(rowData);
                }
            }
        });
}

csCompareDocuments.prototype.comparefilterboxformat = function (cellValue, options, rowObject) {
    var selectBox = "<div class='select-wrapper block'><select id='CompareFilter" + rowObject.FormInstanceID + "'>";
    selectBox = selectBox + "<option>Mismatches only</option>";
    selectBox = selectBox + "<option>Matches only</option>";
    selectBox = selectBox + "<option>Both</option>";
    selectBox = selectBox + "</select></div>";
    return selectBox;
}

csCompareDocuments.prototype.generateReport = function (targetRowData) {
    var sourceData = JSON.stringify(this.documentSyncData.SourceDocument);
    var targetData = JSON.stringify(targetRowData);
    var macroData = JSON.stringify(this.documentSyncData.SelectedMacro.Template);
    var stringData = "tenantId=" + 1;
    stringData += "<&source=" + encodeURIComponent(sourceData);
    stringData += "<&target=" + encodeURIComponent(targetData);
    stringData += "<&macro=" + encodeURIComponent(macroData);
    $.downloadNew(this.URLs.compareDocuments, stringData, 'post');
}
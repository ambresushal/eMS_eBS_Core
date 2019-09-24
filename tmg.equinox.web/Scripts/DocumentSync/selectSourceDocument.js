function csSelectSourceDocument(documentSyncData) {
    this.elementIDs = {
        //id for select source document button
        sourceDocumentButtonJQ: "#cmdSelectSourceDocument",
        //id for selected document display div
        sourceDocumentJQ: "#sourceDocument",
        thSourceDocumentJQ: "#thSourceDcument",
        tdAccountNameJQ: "#tdAccountName",
        tdFolderNameJQ: "#tdFolderName",
        tdFolderVersionNumberJQ: "#tdFolderVersionNumber",
        tdEffectiveDateJQ: "#tdEffectiveDate"
    };

    this.documentSyncData = documentSyncData;
    this.sourceDocument = {};
};



csSelectSourceDocument.prototype.process = function () {
    //cleanup if required or second visit onwards
    console.log("Process Select Source Documents");
    var currentInstance = this;
    $(this.elementIDs.sourceDocumentButtonJQ).click(function () {
        var res = selectSourceDialog.show(currentInstance);
    });
}

csSelectSourceDocument.prototype.setSourceDocumentValues = function () {
    $(this.elementIDs.thSourceDocumentJQ).text(this.sourceDocument.FormInstanceName);
    $(this.elementIDs.tdAccountNameJQ).text(this.sourceDocument.AccountName);
    $(this.elementIDs.tdFolderNameJQ).text(this.sourceDocument.FolderName);
    $(this.elementIDs.tdFolderVersionNumberJQ).text(this.sourceDocument.VersionNumber);
    $(this.elementIDs.tdEffectiveDateJQ).text(this.sourceDocument.EffectiveDate);
    $(this.elementIDs.sourceDocumentJQ).css({ display: "block" });
    this.documentSyncData.SourceDocument = this.sourceDocument;
}

var selectSourceDialog = function () {
    var URLs = {
        docSearch: '/ConsumerAccount/GetDocumentsList?tenantId=1&formDesignID=0&formInstanceId=0'
    };
    var elementIDs = {
        //id for dialog div
        sourceDialogJQ: "#sourceDocumentDialog",
        docSearchGrid: "docSearch",
        docSearchGridJS: "#docSearch",
        docSearchGridPagerJQ: "#pdocSearch"
    }

    function init() {
        //register dialog 
        $(elementIDs.sourceDialogJQ).dialog({
            autoOpen: false,
            height: 600,
            width: 800,
            modal: true
        });
    }
    //initialize the dialog after this js is loaded
    init();

    function loadSelectGrid(dialogParent) {
        var currentInstance = this;
        var colArray = ['Account', 'Folder', 'Consortium', 'Folder Version Number', 'Effective Date', 'Document Name', 'Document Type','', '', '', '', '',''];
        var colModel = [];
        colModel.push({ name: 'AccountName', index: 'AccountName', editable: false, align: 'left', width: 200 });
        colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left' });
        colModel.push({ name: 'ConsortiumName', index: 'ConsortiumName', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'VersionNumber', index: 'VersionNumber', editable: false, align: 'left' });
        colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' });
        colModel.push({ name: 'FormInstanceName', index: 'FormInstanceName', editable: false, align: 'left' });
        colModel.push({ name: 'DesignType', index: 'DesignType', editable: false, align: 'left' });
        colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true });
        colModel.push({ name: 'AccountID', index: 'AccountID', hidden: true });
        colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true });
        colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', hidden: true });
        colModel.push({ name: 'FormInstanceID', index: 'FormInstanceID', hidden: true });
        colModel.push({ name: 'FormDesignID', index: 'FormDesignID', hidden: true });


        $(elementIDs.docSearchGridJS).jqGrid('GridUnload');
        $(elementIDs.docSearchGridJS).parent().append("<div id='p" + elementIDs.docSearchGrid + "'></div>");
        var url = URLs.docSearch;
        $(elementIDs.docSearchGridJS).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Select Documents',
            height: '400',
            rowNum: 20,
            rowList: [10, 20, 30],
            ignoreCase: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: elementIDs.docSearchGridPagerJQ,
            sortname: 'FormInstanceName',
            altclass: 'alternate',
            ondblClickRow: function () {
                setDocumentSelection(dialogParent);
            },
            gridComplete: function () {
                //To set first row of grid as selected.
                console.log("grid complete");
            }
        });
        var pagerElement = elementIDs.docSearchGridPagerJQ;
        //remove default buttons
        $(elementIDs.docSearchGridJS).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.docSearchGridJS).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
        $(elementIDs.docSearchGridJS).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-check', title: 'Select',
               onClickButton: function () {
                   setDocumentSelection(dialogParent);
               }
           });
    }

    function setDocumentSelection(dialogParent) {
        var selectedRowId = $(elementIDs.docSearchGridJS).jqGrid('getGridParam', 'selrow');
        if (selectedRowId != null && selectedRowId != undefined) {
            var rowData = $(elementIDs.docSearchGridJS).jqGrid('getRowData', selectedRowId);
            dialogParent.sourceDocument = rowData;
            dialogParent.setSourceDocumentValues();
            $(elementIDs.sourceDialogJQ).dialog("close");
        }
        else {
            messageDialog.show("Please select a row.");
        }
    }

    return {
        show: function (dialogParent) {
            $(elementIDs.sourceDialogJQ).dialog('option', 'title', "Select Source Document");
            $(elementIDs.sourceDialogJQ).dialog("open");
            loadSelectGrid(dialogParent);
            $(elementIDs.sourceDialogJQ).dialog({
                position: {
                    my: 'center',
                    at: 'center'
                },
            });

        }
    }
}();

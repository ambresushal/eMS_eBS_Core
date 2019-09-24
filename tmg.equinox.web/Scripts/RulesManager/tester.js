var ruletest = (function () {
    var _formInstanceId;
    var _formDesignId;
    var _formDesignVersionId;
    var _elementList;
    var _expressionControl;

    var elementIDs = {
        testerTypeJQ: 'input[name=optradio]',
        testerTypeValueJQ: "input[name='optradio']:checked",
        btnProductJQ: '#btnSelect',
        divContainerJQ: '.tester-container'
    };

    function _init(formDesignId, formDesignVersionId, elementList, expressionControl) {
        _formDesignId = formDesignId;
        _formDesignVersionId = formDesignVersionId;
        _elementList = elementList;
        _expressionControl = expressionControl;
    }

    function _registerEvents() {
        $(elementIDs.btnProductJQ).off('click');
        $(elementIDs.btnProductJQ).click(function () {
            var res = selectSourceDialog.show(ruletest, _formDesignId);
        });

        $(elementIDs.testerTypeJQ).off('change');
        $(elementIDs.testerTypeJQ).on('change', function () {
            if (this.value == 'manual') {
                $(elementIDs.btnProductJQ).css('visibility', 'hidden');
                _process();
            }
            if (this.value == 'product') {
                $(elementIDs.btnProductJQ).css('visibility', 'visible');
            }
        });
    }

    function _process() {
        //var testType = $(elementIDs.testerTypeValueJQ).val();
        //if (testType == 'manual') {
        //    _processManual(_formDesignVersionId, _elementList, _expressionControl);
        //}

        //if (testType == 'product') {
        //    _processProduct(_formDesignVersionId, _elementList, _expressionControl);
        //}
        _processManual(_formDesignVersionId, _elementList, _expressionControl,_formDesignId);
    }

    function _processManual(formDesignVersionId, elementList, expressionControl, formDesignId) {
        $(elementIDs.divContainerJQ).show();
        var tester = new rulesTesterDialog(elementList[0], formDesignVersionId, expressionControl, elementList, [], formDesignId);
        tester.loadDesignRulesTesterData();
    }

    function _processProduct(formDesignVersionId, elementList, expressionControl, formInstanceId) {
        $(elementIDs.divContainerJQ).show();
        var tester = new rulesTesterDialog(elementList[0], formDesignVersionId, expressionControl, elementList, []);
        tester.loadDesignRulesTesterDataFromProduct(formInstanceId);
    }

    function _setSourceDocument(formInstanceId) {
        _formInstanceId = formInstanceId;
        _processProduct(_formDesignVersionId, _elementList, _expressionControl, formInstanceId);
    }

    return {
        process: function (formDesignId, formDesignVersionId, elementList, expressionControl) {
            _init(formDesignId, formDesignVersionId, elementList, expressionControl);
            _registerEvents();
            _process();
        },
        setSourceDocumentValues: function (sourceDocument) {
            _setSourceDocument(sourceDocument.FormInstanceID);
        }
    };
}());

var selectSourceDialog = function () {
    var URLs = {
        docSearch: '/ConsumerAccount/GetDocumentsListNew?tenantId=1&formDesignID={formDesignID}'
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
            height: 400,
            width: 900,
            modal: true
        });
    }
    //initialize the dialog after this js is loaded
    init();

    function loadSelectGrid(dialogParent, formDesignId) {
        var currentInstance = this;
        var colArray = ['Account', 'Folder', 'Consortium', 'Folder Version Number', 'Effective Date', 'Document Name', 'Document Type', '', '', '', '', '', ''];
        var colModel = [];
        colModel.push({ name: 'AccountName', index: 'AccountName', editable: false, align: 'left', width: 200 });
        colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left' });
        colModel.push({ name: 'ConsortiumName', index: 'ConsortiumName', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'VersionNumber', index: 'VersionNumber', editable: false, align: 'left' });
        colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' });
        colModel.push({ name: 'FormInstanceName', index: 'FormInstanceName', editable: false, align: 'left' });
        colModel.push({ name: 'DesignType', index: 'DesignType', hidden: true, editable: false, align: 'left' });
        colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true });
        colModel.push({ name: 'AccountID', index: 'AccountID', hidden: true });
        colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true });
        colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', hidden: true });
        colModel.push({ name: 'FormInstanceID', index: 'FormInstanceID', hidden: true });
        colModel.push({ name: 'FormDesignID', index: 'FormDesignID', hidden: true });


        $(elementIDs.docSearchGridJS).jqGrid('GridUnload');
        $(elementIDs.docSearchGridJS).parent().append("<div id='p" + elementIDs.docSearchGrid + "'></div>");
        var url = URLs.docSearch.replace(/\{formDesignID\}/g, formDesignId);
        $(elementIDs.docSearchGridJS).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Products',
            height: '250',
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
                $(elementIDs.sourceDialogJQ).dialog({ position: { my: 'center', at: 'center' } });
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
            dialogParent.setSourceDocumentValues(rowData);
            $(elementIDs.sourceDialogJQ).dialog("close");
        }
        else {
            messageDialog.show("Please select a row.");
        }
    }

    return {
        show: function (dialogParent, formDesignId) {
            $(elementIDs.sourceDialogJQ).dialog('option', 'title', "Select Product");
            $(elementIDs.sourceDialogJQ).dialog("open");
            loadSelectGrid(dialogParent, formDesignId);
        }
    }
}();
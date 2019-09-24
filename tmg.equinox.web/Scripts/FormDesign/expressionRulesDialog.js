function expressionRulesDialog(uiElement, formDesignId, formDesignVersionId, status) {
    this.uiElement = uiElement;
    this.uiElementType = '';
    this.formDesignId = formDesignId;
    this.formDesignVersionId = formDesignVersionId;
    this.statustext = status;
    this.ruleRowId = 0;
    this.docruleData = [];
}
expressionRulesDialog.ruleType = '';

expressionRulesDialog.elementIDs = {
    expressionDialog: '#expressionRuledialog',
    expressionDialogNew: '#expressionRuledialogNew',
    expressionGrid: 'expressionRulegrid',
    expressionGridJQ: '#expressionRulegrid',
    jsonData: '#jsonData'
}

expressionRulesDialog.URLs = {
    documentRuleList: '/UIElement/DocumentRule?uiElementId={uiElementId}&formDesignId={formDesignId}&formDesignVersionId={formDesignVersionId}',
    getdocumentRuleType: '/UIElement/GetDocumentRuleType',
    savedocumentRule: '/UIElement/SaveDocumentRule',
    deletedocumentRule: '/UIElement/DeleteDocumentRule?dRulesID={docRulesID}'
}

expressionRulesDialog._isInitialized = false;

expressionRulesDialog.prototype.show = function () {
    $(expressionRulesDialog.elementIDs.expressionDialogNew).hide();
    $(expressionRulesDialog.elementIDs.expressionDialog).dialog('option', 'title', 'Expression Builder Dialog - ' + this.uiElement.Label);
    $(expressionRulesDialog.elementIDs.expressionDialog).dialog('open');
    this.uiElementType = this.uiElement.ElementType;
    this.getRuleType(0);
}

expressionRulesDialog.init = function () {
    var currenInstance = this;
    if (expressionRulesDialog._isInitialized == false) {
        $(expressionRulesDialog.elementIDs.expressionDialog).dialog({
            autoOpen: false,
            height: '550',
            width: '60%',
            modal: true,
            close: function () {
                $(expressionRulesDialog.elementIDs.jsonData).val('');
            }
        });
        expressionRulesDialog._isInitialized = true;
    }
}

expressionRulesDialog.init();

expressionRulesDialog.prototype.loadDocumentRuleGrid = function (focusOn) {
    var setFocus = focusOn;
    var currentInstance = this;
    var colArray;
    var colModel = [];

    colArray = ['DocumentRuleID', 'DocumentRuleTypeID', 'Rule Type', 'Description', 'Rule JSON'];

    colModel.push({ name: 'DocumentRuleID', index: 'DocumentRuleID', key: true, hidden: true, search: false });
    colModel.push({ name: 'DocumentRuleTypeID', index: 'DocumentRuleTypeID', hidden: true, sortable: false });
    colModel.push({
        name: 'RuleType', index: 'RuleType',  sortable: false,editable:false, align: 'left',
        formatter: currentInstance.formatColumns,
        unformat: currentInstance.unformatColumns
    });
    colModel.push({
        name: 'Description', index: 'Description', sortable: false, sortable: false, editable: false, align: 'left',
        formatter: currentInstance.formatColumns,
        unformat: currentInstance.unformatColumns
    });
    colModel.push({ name: 'RuleJSON', index: 'RuleJSON', hidden: true, search: false });
    
    var expressionRuleURL = expressionRulesDialog.URLs.documentRuleList
                               .replace(/\{uiElementId\}/g, this.uiElement.UIElementID)
                               .replace(/\{formDesignId\}/g, this.formDesignId)
                               .replace(/\{formDesignVersionId\}/g, this.formDesignVersionId);

    $(expressionRulesDialog.elementIDs.expressionGridJQ).jqGrid('GridUnload');

    $(expressionRulesDialog.elementIDs.expressionGridJQ).parent().append("<div id='p" + expressionRulesDialog.elementIDs.expressionGrid + "'></div>");

    $(expressionRulesDialog.elementIDs.expressionGridJQ).jqGrid({
        datatype: 'json',
        url: expressionRuleURL,
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: '',
        pager: '#p' + expressionRulesDialog.elementIDs.expressionGrid,
        height: '150',
        rowheader: true,
        loadonce: true,
        autowidth: true,
        viewrecords: true,
        altRows: true,
        altclass: 'alternate',
        hidegrid: false,
        onSelectRow: function (id, e) {
            var row = $(this).getRowData(id);
            $(expressionRulesDialog.elementIDs.jsonData).val(row.RuleJSON);
            $(expressionRulesDialog.elementIDs.expressionDialog + ' .help-block').text('');
            $(expressionRulesDialog.elementIDs.jsonData).parent().removeClass('has-error');
        },
        gridComplete: function () {
            var rowIds = $(expressionRulesDialog.elementIDs.expressionGridJQ).getDataIDs();
            if (rowIds.length > 0) {
                if (setFocus == 0) {
                    $(expressionRulesDialog.elementIDs.expressionGridJQ).jqGrid('setSelection', rowIds[setFocus]);
                }
                for (i = 0; i <= rowIds.length; i++) {
                    if (rowIds[i] == setFocus) {
                        $(expressionRulesDialog.elementIDs.expressionGridJQ).jqGrid('setSelection', rowIds[i]);
                        return;
                    }
                }

            }
            else {
                $(expressionRulesDialog.elementIDs.jsonData).val('');
            }

            if (ClientRolesForFormDesignAccess.indexOf(vbRole) >= 0 && currentInstance.uiElement.IsStandard == "true") {
                $("#p" + expressionRulesDialog.elementIDs.expressionGrid).hide();//.addClass('divdisabled');
                $(expressionRulesDialog.elementIDs.expressionGridJQ).find("select").attr('disabled', 'disabled');
                $(expressionRulesDialog.elementIDs.expressionGridJQ).find("textarea").attr('disabled', 'disabled');
                $(expressionRulesDialog.elementIDs.jsonData).attr('disabled', 'disabled');
            } else {
                $("#p" + expressionRulesDialog.elementIDs.expressionGrid).show();
                $(expressionRulesDialog.elementIDs.expressionGridJQ).find("select").removeAttr('disabled');
                $(expressionRulesDialog.elementIDs.expressionGridJQ).find("textarea").removeAttr('disabled');
                $(expressionRulesDialog.elementIDs.jsonData).removeAttr('disabled');
            }
        }
    });

    var pagerElement = '#p' + expressionRulesDialog.elementIDs.expressionGrid;

    $(expressionRulesDialog.elementIDs.expressionGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    //if (this.statustext != 'Finalized') {
        $(expressionRulesDialog.elementIDs.expressionGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-plus',
            onClickButton: function () {
                var rowId = currentInstance.ruleRowId - 1;
                currentInstance.ruleRowId = currentInstance.ruleRowId - 1;
                
                var docrule = {
                    DocumentRuleID: rowId, DocumentRuleTypeID: 0, RuleType: 4 ,Description: '', RuleJSON: ''
                };
                $(expressionRulesDialog.elementIDs.expressionGridJQ).jqGrid('addRowData', rowId, docrule);
                currentInstance.docruleData.push(docrule);
                $(expressionRulesDialog.elementIDs.expressionGridJQ).jqGrid('setSelection', rowId);
            }
        });
        $(expressionRulesDialog.elementIDs.expressionGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-trash',
            onClickButton: function () {
                var rowId = $(expressionRulesDialog.elementIDs.expressionGridJQ).getGridParam('selrow');
                if (rowId != null) {
                    $(expressionRulesDialog.elementIDs.expressionGridJQ).jqGrid('delRowData', rowId);
                    DeleteExpressionBRule(rowId);
                }
            }
        });
        var currentInstance = this;
        $(expressionRulesDialog.elementIDs.expressionGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: 'Save',
            onClickButton: function () {

                var selectedRowId = $(expressionRulesDialog.elementIDs.expressionGridJQ).getGridParam('selrow');
                var expList = [];

                var allData = $(expressionRulesDialog.elementIDs.expressionGridJQ).jqGrid('getGridParam', 'data');
                
                if (selectedRowId != 0 && selectedRowId != null) {
                    var selectedRowData = $(expressionRulesDialog.elementIDs.expressionGridJQ).getRowData(selectedRowId);

                    var valTocompare = selectedRowData.DocumentRuleTypeID + ' ' + $('#Description' + selectedRowId).val().trim();

                    var jsonDataStr = $(expressionRulesDialog.elementIDs.jsonData).val();
                    if ($(expressionRulesDialog.elementIDs.jsonData).val().trim().length > 0 && isJSON(jsonDataStr)) {
                        var getdescription = $('#Description' + selectedRowId);
                        var getdropdownval = $('#Select_' + selectedRowId);
                        var valTocompare = getdropdownval.val() + ' ' + getdescription.val().trim();
                        var isDuplicate = checkDuplicate(selectedRowId, valTocompare, allData)
                        if (!isDuplicate) {
                            expList.push({
                                DocumentRuleID: selectedRowData.DocumentRuleID,
                                DisplayText: getdescription.val(),
                                Description: getdescription.val(),
                                DocumentRuleTypeID: getdropdownval.val(),
                                RuleJSON: $(expressionRulesDialog.elementIDs.jsonData).val(),
                                FormDesignID: currentInstance.formDesignId,
                                FormDesignVersionID: currentInstance.formDesignVersionId,
                                TargetUIElementID: currentInstance.uiElement.UIElementID
                            });
                            SaveExpressionBRule(currentInstance.uiElement.UIElementID, currentInstance.uiElement.ElementType, currentInstance.formDesignId, currentInstance.formDesignVersionId, expList);
                        }
                        else {
                            messageDialog.show(DocumentDesign.expDuplicateMsg);
                        }
                    }
                    else {
                        $(expressionRulesDialog.elementIDs.expressionDialog + ' .help-block').text(DocumentDesign.expEnterJsonMsg);
                        $(expressionRulesDialog.elementIDs.jsonData).parent().addClass('has-error');
                    }

                }
                else {
                    messageDialog.show(DocumentDesign.dataSourceRowSelectionMsg);
                }
            }
            
        });
   // }

    function SaveExpressionBRule(uiElementId, uiElementType, formDesignId, formDesignVersionId, expRuleList ) {

        var rowListlen = expRuleList.length;
        var updateExpRule = new Array(rowListlen);

        for (var i = 0; i < rowListlen; i++) {
            var updateExpressionRecord = new Object();

            updateExpressionRecord.DocumentRuleID = expRuleList[i].DocumentRuleID;
            updateExpressionRecord.DisplayText = expRuleList[i].DisplayText;
            updateExpressionRecord.Description = expRuleList[i].Description;
            updateExpressionRecord.DocumentRuleTypeID = expRuleList[i].DocumentRuleTypeID;
            updateExpressionRecord.RuleJSON = expRuleList[i].RuleJSON;
            updateExpressionRecord.FormDesignID = expRuleList[i].FormDesignID;
            updateExpressionRecord.FormDesignVersionID = expRuleList[i].FormDesignVersionID;
            updateExpressionRecord.TargetUIElementID = expRuleList[i].TargetUIElementID;

            updateExpRule[i] = updateExpressionRecord;
        }


        var expressionRuleList = {
            
            uiElementId: uiElementId,
            uiElementType: uiElementType,
            formDesignId: formDesignId,
            formDesignVersionId: formDesignVersionId,
            nRules: updateExpRule
        };

        var url = expressionRulesDialog.URLs.savedocumentRule;
        var promise = ajaxWrapper.postJSONCustom(url, expressionRuleList);
        promise.done(expressionRuleSuccess); 
        promise.fail(showError);
    }

    function expressionRuleSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(DocumentDesign.expSaveElementMsg);
            setfocusOn = xhr.Items[0].Messages[0];
            currentInstance.getRuleType(setfocusOn);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
    }

    function DeleteExpressionBRule(documentRuleID) {

        var docRulesID = documentRuleID;
        var url = expressionRulesDialog.URLs.deletedocumentRule.replace(/\{docRulesID\}/g, docRulesID);
        var promise = ajaxWrapper.getJSON(url);

        promise.done(function (xhr) {
            if (xhr.Result === ServiceResult.SUCCESS) {
                currentInstance.getRuleType(0);
            }
            else {
                messageDialog.show(Common.errorMsg);
            }
        });

        promise.fail(showError);
    }

    function checkDuplicate (rowId, valTocompare, data) {

        var hasError = false;
        $.each(data, function (i, item) {
            if (item.DocumentRuleID != rowId) {
                var compareStr = item.DocumentRuleTypeID + ' ' + (item.Description != null?item.Description.trim():'');
                if (compareStr == valTocompare) {
                    HighlightDuplicate(item.DocumentRuleID);
                    hasError = true;
                }

            }
        });
        return hasError;
    }

    function HighlightDuplicate(rowId) {
        //remove previous class if any 
        $(expressionRulesDialog.elementIDs.expressionGridJQ).find('td').removeClass("repeater-has-error");

        //add class for required
        var ind = $(expressionRulesDialog.elementIDs.expressionGridJQ).jqGrid('getGridRowById', rowId);
        if (ind) {
            var tcell_RuleType = $("td:eq(2)", ind);
            var tcell_Description = $("td:eq(3)", ind);
            $(tcell_RuleType).addClass("repeater-has-error");
            $(tcell_Description).addClass("repeater-has-error");
        }
    }

}

expressionRulesDialog.prototype.getRuleType = function (focusOn) {
    var currentInstance = this;

    var promise = ajaxWrapper.getJSON(expressionRulesDialog.URLs.getdocumentRuleType);
    promise.done(function (data) {
        expressionRulesDialog.ruleType = data;
    });

    promise.fail(showError);

    this.loadDocumentRuleGrid(focusOn);
}

function showError(xhr) {
    if (xhr.status == 999)
        this.location = '/Account/LogOn';
    else
        messageDialog.show(JSON.stringify(xhr));
}

expressionRulesDialog.prototype.formatColumns = function (cellValue, options, rowObject) {
    var result;
    if (cellValue === undefined || cellValue === null) {
        cellValue = '';
    }
    switch (options.colModel.index) {
        case 'RuleType':
            var result = "<Select name = 'RuleType' data-rowID=" + options.rowId + " id = Select_" + options.rowId + " class = 'form-control'>";
            if (expressionRulesDialog.ruleType != null && expressionRulesDialog.ruleType.length > 0) {
                var ruleType = expressionRulesDialog.ruleType;
                for (var i = 0; i < ruleType.length; i++) {
                    result += "<option id="
                                + ruleType[i].DocumentRuleTypeID
                                + " value=" + ruleType[i].DocumentRuleTypeID
                                + (rowObject.DocumentRuleTypeID === ruleType[i].DocumentRuleTypeID ? "  selected" : "")
                                + ">"
                                + ruleType[i].DisplayText
                                + "</option>";
                }
            }
            result = result + "</select>";
            break;
        case 'Description':
            var resultId = "Description" + rowObject.DocumentRuleID;
            result = "<textarea type='text' id='" + resultId + "'maxlength='1000' >" + cellValue + "</textarea>";
            break;
    }
    return result;
}

expressionRulesDialog.prototype.unformatColumns = function (cellValue, options, rowObject) {
    var result;
    if (cellValue === undefined || cellValue === null) {
        cellValue = '';
    }
    switch (options.colModel.index) {
        case 'RuleType':
            var result = "<Select name = 'RuleType' data-rowID=" + options.rowId + " id = Select_" + options.rowId + " class = 'form-control'>";
            if (expressionRulesDialog.ruleType != null && expressionRulesDialog.ruleType.length > 0) {
                var ruleType = expressionRulesDialog.ruleType;
                for (var i = 0; i < ruleType.length; i++) {
                    result += "<option id="
                                + ruleType[i].DocumentRuleTypeID
                                + " value=" + ruleType[i].DocumentRuleTypeID
                                + (rowObject.DocumentRuleTypeID === ruleType[i].DocumentRuleTypeID ? "  selected" : "")
                                + ">"
                                + ruleType[i].DisplayText
                                + "</option>";
                }
            }
            result = result + "</select>";
            break;
        case 'Description':
            var resultId = "Description" + rowObject.DocumentRuleID;
            result = "<textarea type='text' id='" + resultId + "'maxlength='1000' >" + cellValue + "</textarea>";
            break;
    }
    return result;
}

$(expressionRulesDialog.elementIDs.jsonData ).on('change', function () {
    if($(expressionRulesDialog.elementIDs.jsonData).val().trim().length > 0)
    {
        $(expressionRulesDialog.elementIDs.expressionDialog + ' .help-block').text('');
        $(expressionRulesDialog.elementIDs.jsonData).parent().removeClass('has-error');
    }
    else {

        $(expressionRulesDialog.elementIDs.expressionDialog + ' .help-block').text(DocumentDesign.expEnterJsonMsg);
        $(expressionRulesDialog.elementIDs.jsonData).parent().addClass('has-error');
    }

});




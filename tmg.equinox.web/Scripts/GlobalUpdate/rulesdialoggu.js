var rulesDialogGu = function (uiElement, formDesignVersionId, elementGridData, globalUpdateId) {
    this.uiElement = uiElement;
    this.elementGridData = elementGridData;
    this.formDesignVersionId = formDesignVersionId;
    this.expressionBuilders = [];
    this.expressionBuildersGu = [];  //TODO - Confirmed for Gu
    this.currentExpressionBuilder = undefined;
    this.currentExpressionBuilderGu = undefined; //TODO - Confirmed for Gu
    this.elementDropDownList = undefined;
    this.rulesData = undefined;
    this.readOnlyRulesData = [];
    this.rulesRowId = 0;
    //this.statustext = status;
    this.logicalOperators = [];
    this.operators = [];
    this.targetProperties = [];
    this.isExist = false;
    this.globalUpdateId = globalUpdateId;
}

rulesDialogGu.URLs = {
    rulesList: '/GlobalUpdate/Rules?tenantId=1&formDesignVersionId={formDesignVersionId}&uiElementId={uiElementId}&globalUpdateId={globalUpdateId}',
    getLogicalOperatorList: '/UIElement/GetLogicalOperatorList?tenantId=1',
    getOperatorList: '/UIElement/GetOperatorList?tenantId=1',
    getTargetPropertyList: '/UIElement/GetTargetPropertyList?tenantId=1'
}

rulesDialogGu.TargetProperties = {
    Enabled: 1,
    RunValidation: 2,
    Value: 3,
    Visible: 4,
    IsRequired: 5,
    Error: 6,
    ExpressionValue: 7
}


rulesDialogGu._currentInstance = undefined;
rulesDialogGu._isInitialized = false;

rulesDialogGu.elementIDs = {
    rulesDialog: '#rulesdialogGu',
    rulesGrid: 'rulesgrid',
    rulesGridJQ: '#rulesgrid',
    expressionBuilder: 'expressionbuilder',
    expressionBuilderJQ: '#expressionbuilder',
    labelExpression: '#lblExpression',
    removeRule: '#guRemoveRule',
    addRule: '#guAddRule'

}

rulesDialogGu.showerror = function (xhr) {
    if (xhr.status == 999)
        this.location = '/Account/LogOn';
    else
        messageDialog.show(JSON.stringify(xhr));
}

//init dialog
rulesDialogGu.init = function () {
    if (rulesDialogGu._isInitialized == false) {
        $(rulesDialogGu.elementIDs.rulesDialog).dialog({
            autoOpen: false,
            height: '600',
            width: '80%',
            modal: true
        });
        rulesDialogGu._isInitialized = true;
    }
}();

rulesDialogGu.prototype.getCurrentElementId = function () {
    return this.uiElement.UIElementID;
}
rulesDialogGu.prototype.show = function () {
    rulesDialogGu._currentInstance = this;
    var dialogContent = this.getHtmlTemplate();
    $(rulesDialogGu.elementIDs.rulesDialog).empty();
    $(rulesDialogGu.elementIDs.rulesDialog).append(dialogContent);
    $(rulesDialogGu.elementIDs.rulesDialog).dialog('option', 'title', 'Update Value - ' + this.uiElement.Label)
    $(rulesDialogGu.elementIDs.rulesDialog).dialog('open');

    var currentInstance = this;
    $.when(ajaxWrapper.getJSON(rulesDialogGu.URLs.getLogicalOperatorList),
           ajaxWrapper.getJSON(rulesDialogGu.URLs.getOperatorList),
           ajaxWrapper.getJSON(rulesDialogGu.URLs.getTargetPropertyList)
           ).done(function (logicalOpResult, operatorResult, targetPropResult) {
               currentInstance.logicalOperators = logicalOpResult[0];
               currentInstance.operators = operatorResult[0];
               //var list = targetPropResult[0];

               //TODO : Confirmed for Global Update
               var list = targetPropResult[0].filter(function (t) {
                   return t.TargetPropertyName == 'Expression Value' || t.TargetPropertyName == 'Value';
               });


               currentInstance.targetProperties = list;
               currentInstance.loadRulesGrid();
           }).fail(rulesDialogGu.showerror);
}

rulesDialogGu.prototype.open = function () {
    $(rulesDialogGu.elementIDs.rulesDialog).dialog('open');
}

rulesDialogGu.prototype.getHtmlTemplate = function () {
    var rulesDialogHtml = "<div class='panel-body'>" +
                            "<div>" +
                            "<div class='row-fluid'>" +
                            "<div class='col-xs-12 col-md-12 col-lg-12 col-sm-12'>" +
                            "<label id='lblExpression' class='expressionlabel'>Add Expressions</label>" +
                            "<div id='expressionbuilder' class='rulecontainer'>" +
                            "</div>" +
                            "</div>" +
                            "<div class='row-fluid'>" +
                            "<div class='col-xs-12 col-md-12 col-lg-12 col-sm-12'></div>" +
                            "</div>" +
                            "<div class='row-fluid'>" +
                            "<div class='col-xs-12 col-md-12 col-lg-12 col-sm-12'>" +
                            "<table id='rulesgrid'></table>" +
                            "<span class='help-block'>Changes will be Saved locally. They will be Saved to the system only if the Save button on the Properties Grid is clicked.</span>" +
                            "</div>" +
                            "</div>" +
                            "</div>" +
                            "</div>";
    return rulesDialogHtml;
}

rulesDialogGu.prototype.loadRulesGrid = function () {
    var mode = getQueryString("mode");
    var currentInstance = this;
    $(rulesDialogGu.elementIDs.expressionBuilderJQ).empty();

    //set column list
    var colArray = ['RuleId', 'PropertyRuleMapID', 'Filter Type', 'New Value', 'If Expression is False', 'Validation Message'];
    //set column models
    var colModel = [];
    colModel.push({ name: 'RuleId', index: 'RuleId', key: true, hidden: true, search: false });
    colModel.push({ name: 'PropertyRuleMapID', index: 'PropertyRuleMapID', hidden: true });
    colModel.push({ name: 'TargetPropertyId', index: 'TargetPropertyId', align: 'center', editable: false, formatter: currentInstance.formatColumns, unformat: currentInstance.unformatColumns, targetProperties: currentInstance.targetProperties, sortable: false });
    colModel.push({ name: 'ResultSuccess', index: 'ResultSuccess', align: 'center', editable: false, formatter: currentInstance.formatColumns, unformat: currentInstance.unformatColumns, sortable: false });
    colModel.push({ name: 'ResultFailure', index: 'ResultFailure', align: 'center', editable: true, hidden: true, formatter: currentInstance.formatColumns, unformat: currentInstance.unformatColumns, sortable: false });
    //colModel.push({ name: 'Message', index: 'Message', editable: true, formatter: currentInstance.formatColumns, unformat: currentInstance.unformatColumns, sortable: false });
    colModel.push({ name: 'Message', index: 'Message', hidden: true });

    //get URL for grid
    var rulesURL = rulesDialogGu.URLs.rulesList.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId).replace(/\{uiElementId\}/g, this.uiElement.UIElementID).replace(/\{globalUpdateId\}/g, this.globalUpdateId);
    //unload previous grid values
    $(rulesDialogGu.elementIDs.rulesGridJQ).jqGrid('GridUnload');

    $(rulesDialogGu.elementIDs.rulesGridJQ).parent().append("<div id='p" + rulesDialogGu.elementIDs.rulesGrid + "'></div>");

    $(rulesDialogGu.elementIDs.rulesGridJQ).jqGrid({
        datatype: 'local',
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Set Value',
        pager: '#p' + rulesDialogGu.elementIDs.rulesGrid,
        height: '140',
        rowheader: true,
        loadonce: true,
        autowidth: true,
        viewrecords: true,
        altRows: true,
        altclass: 'alternate',
        hidegrid: false,
        afterInsertRow: function (rowId, rowData, rowElem) {
            currentInstance.enableDisplayExressionGrid(rowData.TargetPropertyId);
            $(rulesDialogGu.elementIDs.rulesGridJQ).find('#TargetProperty' + rowId).on('change', function () {
                var targetPropertyId = $(this).val();
                currentInstance.enableDisplayExressionGrid(targetPropertyId);

                var rowObject = {
                    RuleId: rowId,
                    TargetPropertyId: targetPropertyId,
                    ResultSuccess: '',
                    ResultFailure: '',
                    IsResultSuccessElement: '',
                    IsResultSuccessFailure: '',
                    Message: ''
                };
                var rowElement = $(rulesDialogGu.elementIDs.rulesGridJQ).find('#' + rowId);
                var resultSuccessCell = $(rowElement).find('td[aria-describedby="rulesgrid_ResultSuccess"]');
                var resultFailureCell = $(rowElement).find('td[aria-describedby="rulesgrid_ResultFailure"]');
                var messageCell = $(rowElement).find('td[aria-describedby="rulesgrid_Message"]');
                var options = {};
                var model = colModel.filter(function (model) {
                    return model.name == 'ResultSuccess';
                });
                options.colModel = model[0];
                resultSuccessCell.html(currentInstance.formatColumns('', options, rowObject));
                var model = colModel.filter(function (model) {
                    return model.name == 'ResultFailure';
                });
                options.colModel = model[0];
                resultFailureCell.html(currentInstance.formatColumns('', options, rowObject));
                var model = colModel.filter(function (model) {
                    return model.name == 'Message';
                });
                options.colModel = model[0];
                messageCell.html(currentInstance.formatColumns('', options, rowObject));
                currentInstance.registerRulesRowEvents(rowId, rowObject);
                currentInstance.isErrorRuleExists(rowId, rowObject);
            });
            currentInstance.registerRulesRowEvents(rowId, rowData);
        },


        //Check for Value Expression label
        gridComplete: function () {
            var allRowsInGrid = $(this).getRowData();
            if (allRowsInGrid.length > 0) {
                $(rulesDialogGu.elementIDs.addRule).hide();

            }
            else {
                $(rulesDialogGu.elementIDs.addRule).show();
            }
        },
        loadComplete: function () {
            if (mode == 'view') {
                $(this.p.pager).find('td').addClass('hide');
            }
        },
        onSelectRow: function (rowId) {
            $(rulesDialogGu.elementIDs.expressionBuilderJQ).empty();
            if (currentInstance.currentExpressionBuilder != null) {
                currentInstance.currentExpressionBuilder.getRuleData();
            }
            var filteredRules = currentInstance.rulesData.filter(function (control) {
                return control.RuleId == rowId;
            });
            var filteredRule = {};
            if (filteredRules == null || filteredRules.length == 0) {
                filteredRule = {};
                $.extend(filteredRule, $(rulesDialogGu.elementIDs.rulesGridJQ).jqGrid('getRowData', rowId));
                currentInstance.rulesData.push(filteredRule);
            }
            else {
                filteredRule = filteredRules[0];
            }
            var filteredExpression = currentInstance.expressionBuilders.filter(function (control) {
                return control.RuleID == rowId;
            });
            var expressionControl = null;

            if (filteredExpression == null || filteredExpression.length == 0) {
                if (filteredRule.hasOwnProperty('RootExpression')) {

                    expressionControl = new expressionBuilder(currentInstance.uiElement, filteredRule, rulesDialogGu.elementIDs.expressionBuilder,
                        currentInstance.getGridForExpression(), currentInstance.logicalOperators, currentInstance.operators);
                }
                else {
                    filteredRule.RootExpression = null;
                    expressionControl = new expressionBuilder(currentInstance.uiElement, filteredRule, rulesDialogGu.elementIDs.expressionBuilder,
                    currentInstance.getGridForExpression(), currentInstance.logicalOperators, currentInstance.operators);
                }
                currentInstance.expressionBuilders.push({ RuleID: rowId, ExpressionBuilder: expressionControl });
            }
            else {
                expressionControl = filteredExpression[0].ExpressionBuilder;
            }
            expressionControl.render();
            currentInstance.currentExpressionBuilder = expressionControl;
        }
    });

    var pagerElement = '#p' + rulesDialogGu.elementIDs.rulesGrid;
    //remove default buttons
    $(rulesDialogGu.elementIDs.rulesGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    //remove paging
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    //  if (this.statustext != 'Finalized') {
    $(rulesDialogGu.elementIDs.rulesGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-plus', id: 'guAddRule',
        onClickButton: function () {
            var rowId = currentInstance.rulesRowId - 1;
            currentInstance.rulesRowId = currentInstance.rulesRowId - 1;
            var targetPropertyId = rulesDialogGu.TargetProperties.ExpressionValue;
            var rule = { RuleId: rowId, PropertyRuleMapID: 0, TargetPropertyId: targetPropertyId, ResultSuccess: '', ResultFailure: '', IsResultSuccessElement: '', IsResultFailureElement: '', Message: '' };
            $(rulesDialogGu.elementIDs.rulesGridJQ).jqGrid('addRowData', rowId, rule);
            currentInstance.rulesData.push(rule);
            $(rulesDialogGu.elementIDs.rulesGridJQ).jqGrid('setSelection', rowId);
        }
    });

    $(rulesDialogGu.elementIDs.rulesGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-trash', id: 'guRemoveRule',
        onClickButton: function () {
            var rowId = $(rulesDialogGu.elementIDs.rulesGridJQ).getGridParam('selrow');
            if (rowId != null) {
                $(rulesDialogGu.elementIDs.rulesGridJQ).jqGrid('delRowData', rowId);
                //remove from rulesData and expressionBuilders
                for (var idx = 0; idx < currentInstance.rulesData.length; idx++) {
                    if (currentInstance.rulesData[idx].RuleId == rowId) {
                        currentInstance.rulesData.splice(idx, 1);
                        break;
                    }
                }
                for (var idxExp = 0; idxExp < currentInstance.expressionBuilders.length; idxExp++) {
                    if (currentInstance.expressionBuilders[idxExp].RuleID == rowId) {
                        currentInstance.expressionBuilders.splice(idxExp, 1);
                        break;
                    }
                }
                currentInstance.currentExpressionBuilder.close();
                currentInstance.currentExpressionBuilder = undefined;
                var rowIds = $(rulesDialogGu.elementIDs.rulesGridJQ).getDataIDs();
                if (rowIds.length > 0) {
                    $(rulesDialogGu.elementIDs.rulesGridJQ).jqGrid('setSelection', rowIds[0]);
                }
            }
        }
    });
    // }
    var promise = ajaxWrapper.getJSON(rulesURL);
    promise.done(function (xhr) {
        currentInstance.rulesData = xhr;
        var elementData = currentInstance.getGridForExpression();
        $.extend(true, currentInstance.readOnlyRulesData, currentInstance.rulesData);
        for (var index = 0; index < currentInstance.rulesData.length; index++) {
            var item = currentInstance.rulesData[index];
            var elem = elementData.filter(function (el) {
                return el.UIElementName == item.ResultSuccess;
            });
            if (elem != null && elem.length > 0) {
                item.ResultSuccessLabel = elem[0].Element;
            }
            var elem = elementData.filter(function (el) {
                return el.UIElementName == item.ResultFailure;
            });
            if (elem != null && elem.length > 0) {
                item.ResultFailureLabel = elem[0].Element;
            }
            $(rulesDialogGu.elementIDs.rulesGridJQ).jqGrid('addRowData', item.RuleId, item);

        }
        if (currentInstance.rulesData.length > 0) {
            var rowIds = $(rulesDialogGu.elementIDs.rulesGridJQ).getDataIDs();
            $(rulesDialogGu.elementIDs.rulesGridJQ).jqGrid('setSelection', rowIds[0]);
        }
    });
    promise.fail(rulesDialogGu.showerror);
}

rulesDialogGu.prototype.registerRulesRowEvents = function (rowId, rowData) {
    if (rowData.TargetPropertyId == rulesDialogGu.TargetProperties.ExpressionValue || rowData.TargetPropertyId == rulesDialogGu.TargetProperties.Value) {
        this.registerValueElementEvents('Success', rowId);
        //this.registerValueElementEvents('Failure', rowId);
    }
}

rulesDialogGu.prototype.isErrorRuleExists = function (rowId, rowData) {
    var currentInstance = this;
    if (rowData.TargetPropertyId == rulesDialogGu.TargetProperties.Error) {
        if (currentInstance.isExist) {
            $(rulesDialogGu.elementIDs.rulesGridJQ).jqGrid('delRowData', rowId);
            //remove from rulesData
            for (var idx = 0; idx < currentInstance.rulesData.length; idx++) {
                if (currentInstance.rulesData[idx].RuleId == rowId) {
                    currentInstance.rulesData.splice(idx, 1);
                    break;
                }
            }
            if (currentInstance.rulesData.length > 0) {
                var rowIds = $(rulesDialogGu.elementIDs.rulesGridJQ).getDataIDs();
                $(rulesDialogGu.elementIDs.rulesGridJQ).jqGrid('setSelection', rowIds[0]);
            }
            messageDialog.show('Error Rule cannot be applied multiple times on same control.');
        }
        else if (currentInstance.rulesData.length > 1) {
            currentInstance.rulesData.filter(function (ct) {
                if (ct.TargetPropertyId == rulesDialogGu.TargetProperties.Error) {
                    currentInstance.isExist = true;
                }
            });
            if (currentInstance.isExist) {
                $(rulesDialogGu.elementIDs.rulesGridJQ).jqGrid('delRowData', rowId);
                //remove from rulesData
                for (var idx = 0; idx < currentInstance.rulesData.length; idx++) {
                    if (currentInstance.rulesData[idx].RuleId == rowId) {
                        currentInstance.rulesData.splice(idx, 1);
                        break;
                    }
                }
                if (currentInstance.rulesData.length > 0) {
                    var rowIds = $(rulesDialogGu.elementIDs.rulesGridJQ).getDataIDs();
                    $(rulesDialogGu.elementIDs.rulesGridJQ).jqGrid('setSelection', rowIds[0]);
                }
                messageDialog.show('Error Rule cannot be applied multiple times on same control.');
            }
            else {
                currentInstance.isExist = true;
            }
        }
        else {
            currentInstance.isExist = true;
        }
    }
}

rulesDialogGu.prototype.registerValueElementEvents = function (valueType, rowId) {
    var currentInstance = this;
    var row = $(rulesDialogGu.elementIDs.rulesGridJQ).find('#' + rowId);
    var resultCheckElement = row.find('#IsResult' + valueType + 'Element' + rowId);
    var resultLabelElement = row.find('#Result' + valueType + 'Label' + rowId);
    var resultElement = row.find('#Result' + valueType + rowId);
    var searchElement = row.find('#Search' + valueType + 'Element' + rowId);
    searchElement.on('click', function () {
        var searchDialog = new expressionBuilderSearchDialog(currentInstance.getGridForExpression(), resultElement, resultLabelElement);
        searchDialog.show();
    });

    resultCheckElement.on('change', function () {
        if ($(this)[0].checked == true) {
            $(searchElement).css('visibility', 'visible');
            $(resultLabelElement).attr('readonly', true);
            $(resultLabelElement).val('');
            $(resultElement).val('');
            $(resultLabelElement).on('keydown', function (event) {
                if (event.key == 'Backspace') {
                    event.stopPropagation();
                    return false;
                }
            });
        }
        else {
            $(searchElement).css('visibility', 'hidden');
            $(resultLabelElement).attr('readonly', false);
            $(resultLabelElement).val('');
            $(resultElement).val('');
            $(resultLabelElement).off('keydown');
        }
    });
    if ($(resultCheckElement)[0].checked == true) {
        $(resultLabelElement).on('keydown', function (event) {
            if (event.key == 'Backspace') {
                event.stopPropagation();
                return false;
            }
        });
    }
    else {
        $(resultLabelElement).off('keydown');

    }

}

rulesDialogGu.prototype.formatColumns = function (cellValue, options, rowObject) {
    var result;
    if (cellValue === undefined || cellValue === null) {
        cellValue = '';
    }
    switch (options.colModel.index) {
        case 'TargetPropertyId':
            result = "<select id='TargetProperty" + rowObject.RuleId + "'>";
            if (options.colModel.targetProperties != null && options.colModel.targetProperties.length > 0) {
                var targetProperties = options.colModel.targetProperties;
                for (var idx = 0; idx < targetProperties.length; idx++) {
                    if (cellValue == targetProperties[idx].TargetPropertyId) {
                        result = result + "<option value='" + targetProperties[idx].TargetPropertyId + "' selected>" + targetProperties[idx].TargetPropertyName + "</option>";
                    }
                    else {
                        result = result + "<option value='" + targetProperties[idx].TargetPropertyId + "'>" + targetProperties[idx].TargetPropertyName + "</option>";
                    }
                }
            }
            result = result + "</select>";
            break;
        case 'ResultSuccess':
        case 'ResultFailure':
            if (rowObject.TargetPropertyId == rulesDialogGu.TargetProperties.ExpressionValue || rowObject.TargetPropertyId == rulesDialogGu.TargetProperties.Value) {
                var resultCheckId;
                var resultLabelId;
                var resultId;
                var searchId;
                var val;
                var label;
                var isCheck;
                var visible;
                var readOnly = '';
                if (options.colModel.index == "ResultSuccess") {
                    resultId = 'ResultSuccess' + rowObject.RuleId;
                    resultLabelId = 'ResultSuccessLabel' + rowObject.RuleId;
                    resultCheckId = 'IsResultSuccessElement' + rowObject.RuleId;
                    searchId = 'SearchSuccessElement' + rowObject.RuleId;
                    val = rowObject.ResultSuccess;
                    if (rowObject.IsResultSuccessElement == true) {
                        isCheck = 'checked';
                        visible = 'visible';
                        label = rowObject.ResultSuccessLabel;
                        readOnly = "readonly='true'";
                    }
                    else {
                        label = val;
                    }
                }
                else {
                    resultId = 'ResultFailure' + rowObject.RuleId;
                    resultLabelId = 'ResultFailureLabel' + rowObject.RuleId;
                    resultCheckId = 'IsResultFailureElement' + rowObject.RuleId;
                    searchId = 'SearchFailureElement' + rowObject.RuleId;
                    val = rowObject.ResultFailure;
                    label = rowObject.ResultFailureLabel;
                    if (rowObject.IsResultFailureElement == true) {
                        isCheck = 'checked';
                        visible = 'visible';
                        label = rowObject.ResultFailureLabel;
                        readOnly = "readonly='true'";
                    }
                    else {
                        label = val;
                    }
                }
                if (val == null) {
                    val = '';
                }
                if (label == null) {
                    label = '';
                }
                if (isCheck == null) {
                    isCheck = '';
                }
                if (visible == null) {
                    visible = 'hidden';
                }
                result = "<div class='jqgridcelldiv'>";
                result = result + "<input type='text' id='" + resultLabelId + "' value='" + label + "'" + readOnly + "/>";
                result = result + "<input type='hidden' id='" + resultId + "' value='" + val + "'/>";
                result = result + "<input type='checkbox' id='" + resultCheckId + "' " + isCheck + "/>";
                result = result + "<span class='ui-icon ui-icon-search' id='" + searchId + "' style='display: inline-block;visibility:" + visible + "'></span></div>";
            }


            else {
                result = cellValue;
            }
            break;
        case 'Message':
            if (rowObject.TargetPropertyId == rulesDialogGu.TargetProperties.IsRequired || rowObject.TargetPropertyId == rulesDialogGu.TargetProperties.RunValidation) {
                var resultId = "Message" + rowObject.RuleId;
                result = "<textarea type='text' id='" + resultId + "'maxlength='1000' >" + cellValue + "</textarea>";
            }
            else {
                result = cellValue;
            }
            break;
        default:
            result = cellValue;
            break;
    }
    return result;
}

rulesDialogGu.prototype.unformatColumns = function (cellValue, options) {
    var result;
    result = $(this).find('#' + options.rowId).find('#' + options.colModel.index).val();
    return result;
}

rulesDialogGu.prototype.getRulesData = function () {
    this.updateDataFromGrid();//AF

    if (this.rulesData[0].TargetPropertyId == rulesDialogGu.TargetProperties.Value) { //AF
        this.rulesData[0].RootExpression = [];
        this.rulesData[0].Expressions = [];
    }
    else if (this.rulesData[0].TargetPropertyId == rulesDialogGu.TargetProperties.ExpressionValue) { //AF
        for (var idx = 0; idx < this.expressionBuilders.length; idx++) {
            this.expressionBuilders[idx].ExpressionBuilder.getRuleData();
        }
    }

    return this.rulesData;
}

rulesDialogGu.prototype.enableDisplayExressionGrid = function (targetPropertyId) {
    if (targetPropertyId == rulesDialogGu.TargetProperties.Value) {
        $(rulesDialogGu.elementIDs.rulesDialog).height('300')
        $(rulesDialogGu.elementIDs.labelExpression).addClass('expressionlabel');
        $(rulesDialogGu.elementIDs.expressionBuilderJQ).hide();
        

    }
    else if (targetPropertyId == rulesDialogGu.TargetProperties.ExpressionValue) {
        $(rulesDialogGu.elementIDs.rulesDialog).height('565')
        $(rulesDialogGu.elementIDs.labelExpression).removeClass('expressionlabel');
        $(rulesDialogGu.elementIDs.expressionBuilderJQ).show();
    }
}

rulesDialogGu.prototype.updateDataFromGrid = function () {
    var rowIds = $(rulesDialogGu.elementIDs.rulesGridJQ).getDataIDs();
    if (rowIds.length > 0) {
        for (var idx = 0; idx < rowIds.length; idx++) {
            var rowId = rowIds[idx];
            var row = $(rulesDialogGu.elementIDs.rulesGridJQ).find('#' + rowIds[idx]);
            var rules = this.rulesData.filter(function (rl) {
                return rl.RuleId == rowId;
            });
            if (rules != null && rules.length > 0) {
                var rule = rules[0];
                var elem = row.find('#TargetProperty' + rowId);
                rule.TargetPropertyId = elem[0].value;

                var list = this.targetProperties.filter(function (t) {
                    if (t.TargetPropertyId == rule.TargetPropertyId)
                        return t;
                });

                rule.TargetProperty = list[0].TargetPropertyName;

                if (rule.TargetPropertyId == rulesDialogGu.TargetProperties.ExpressionValue || rule.TargetPropertyId == rulesDialogGu.TargetProperties.Value) {
                    elem = row.find('#Message' + rowId);
                    if (elem != null && elem.length > 0) {
                        rule.Message = elem[0].value;
                    }
                }
                if (rule.TargetPropertyId == rulesDialogGu.TargetProperties.ExpressionValue || rule.TargetPropertyId == rulesDialogGu.TargetProperties.Value) {
                    elem = row.find('#IsResultSuccessElement' + rowId);
                    rule.IsResultSuccessElement = elem[0].checked;
                    if (rule.IsResultSuccessElement == true) {
                        elem = row.find('#ResultSuccess' + rowId);
                        rule.ResultSuccess = elem[0].value;
                    }
                    else {
                        elem = row.find('#ResultSuccessLabel' + rowId);
                        rule.ResultSuccess = elem[0].value;
                    }
                    elem = row.find('#IsResultFailureElement' + rowId);
                    rule.IsResultFailureElement = elem[0].checked;
                    if (rule.IsResultFailureElement == true) {
                        elem = row.find('#ResultFailure' + rowId);
                        rule.ResultFailure = elem[0].value;
                    }
                    else {
                        elem = row.find('#ResultFailureLabel' + rowId);
                        rule.ResultFailure = elem[0].value;
                    }
                }
            }
        }
    }
}

rulesDialogGu.prototype.isUpdated = function () {
    var isUpdated = false;
    //check for rule changes
    var strOriginal = JSON.stringify(this.readOnlyRulesData);
    var strModified = JSON.stringify(this.rulesData);
    if (strOriginal != strModified) {
        //add a more detailed check, if required
        isUpdated = true;
    }
    return isUpdated;
}

rulesDialogGu.prototype.getElementsForExpression = function () {
    //get parent element type
    var parentId = this.uiElement.parent;
    var parentElement = this.elementGridData.filter(function (element) {
        return element.parent == parentId;
    });
    var parentElementType;
    if (parentElement != null && parentElement.length > 0) {
        parentElementType = parentElement[0].ElementType;
    }
    var elements = [];
    for (var idx = 0; idx < this.elementGridData.length; idx++) {
        var elem = this.elementGridData[idx];
        if (elem.ElementType != 'Tab' && elem.ElementType != 'Section' && elem.ElementType != 'Repeater') {
            var parent = this.elementGridData.filter(function (element) {
                return element.UIElementID == elem.parent;
            });
            if (parent != null && parent.length > 0) {
                parent = parent[0];
                if (parent.ElementType == 'Section') {
                    elements.push(elem.Label);
                }
                if (parent.ElementType == 'Repeater' && elem.parent == parentId) {
                    elements.push(elem.Label);
                }
            }
        }
    }
    return elements;
}

rulesDialogGu.prototype.getGridForExpression = function () {
    var gridElements = [];
    var tabElements = this.elementGridData.filter(function (element) {
        return element.ElementType == 'Tab';
    });
    var tabElementId = 0;
    if (tabElements != null && tabElements.length > 0) {
        tabElementId = tabElements[0].UIElementID;
    }
    var sectionElements = this.elementGridData.filter(function (element) {
        return element.ElementType == 'Section' && element.parent == tabElementId;
    });
    for (var idx = 0; idx < sectionElements.length; idx++) {
        this.getSectionElementsForExpression('', sectionElements[idx], gridElements);
    }
    return gridElements;
}

rulesDialogGu.prototype.getSectionElementsForExpression = function (prependParent, section, gridElements) {
    var elements = this.elementGridData.filter(function (element) {
        return element.parent == section.UIElementID && element.ElementType != '[Blank]';
    });
    var currentParent = prependParent + section.Label;
    if (prependParent == '') {
        prependParent = section.Label + ' > ';
    }
    else {
        prependParent = prependParent + section.Label;
    }
    if (elements != null && elements.length > 0) {
        for (var idx = 0; idx < elements.length; idx++) {
            var element = elements[idx];
            if (element.ElementType == 'Repeater') {
                this.getRepeaterElementsForExpression(currentParent, element, gridElements);
            }
            else if (element.ElementType == 'Section') {
                this.getSectionElementsForExpression(prependParent, element, gridElements);
            }
            else {
                gridElements.push({ Section: currentParent, Element: element.Label, UIElementID: element.UIElementID, UIElementName: element.UIElementName, Parent: element.parent });
            }
        }
    }
}

rulesDialogGu.prototype.getRepeaterElementsForExpression = function (prependParent, repeater, gridElements) {
    if (this.uiElement.parent == repeater.UIElementID) {
        var elements = this.elementGridData.filter(function (element) {
            return element.parent == repeater.UIElementID;
        });
        if (elements != null && elements.length > 0) {
            for (var idx = 0; idx < elements.length; idx++) {
                var element = elements[idx];
                gridElements.push({ Section: prependParent, Element: element.Label, UIElementID: element.UIElementID, UIElementName: element.UIElementName, Parent: element.parent });
            }
        }
    }
}

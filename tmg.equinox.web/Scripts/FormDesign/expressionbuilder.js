function expressionBuilder(uiElement, ruleObj, containerElementId, searchGridData, logicalOperators, operators) {
    this.uiElement = uiElement;
    this.ruleObj = ruleObj;
    this.containerElementID = containerElementId;
    this.searchGridData = searchGridData;
    this.logicalOperators = logicalOperators;
    this.operators = operators;
    this.expressionsList = null;
    this.tenantId = 1;
    this.ruleId = -1;
    this.newId = -1;
    this.rootExpression = null;
    this.isCurrent = true;
    this.isParentRepeater = false;
}

expressionBuilder.prototype.buildrules = function () {
    if (this.ruleObj.RootExpression == null) {
        this.ruleObj.RootExpression = this.createNewExpression(null, null);
    }
    var rootExp = this.ruleObj.RootExpression;
    this.tenantId = rootExp.TenantId;
    this.ruleId = this.ruleObj.RuleId;
    this.expressionsList = [];
    var rules = {};
    this.addlogicalrule(rootExp, rules);

    return rules;
}

expressionBuilder.prototype.addlogicalrule = function (nodeExpression, parentObj) {
    this.expressionsList.push(nodeExpression);
    if (nodeExpression.LogicalOperatorTypeId == 1 || nodeExpression.LogicalOperatorTypeId == null) {
        parentObj.condition = "AND";
    }
    else {
        parentObj.condition = "OR";
    }
    parentObj.data = { id: nodeExpression.ExpressionId, isRightOperandElement: false, rightOperandElementName: '' };
    parentObj.rules = [];
    var currentInstance = this;
    if (nodeExpression.Expressions != null && nodeExpression.Expressions.length > 0) {
        $.each(nodeExpression.Expressions, function (idx, val) {
            var emptyObj = {};
            parentObj.rules.push(emptyObj);
            if (val.ExpressionTypeId == 1) {
                currentInstance.addlogicalrule(val, emptyObj);
            }
            else {
                currentInstance.addexpressionrule(val, emptyObj);
            }
        });
    }
}

expressionBuilder.prototype.addexpressionrule = function (expression, parentObj) {
    var currentInstance = this;
    this.expressionsList.push(expression);
    parentObj.id = expression.LeftOperand;
    parentObj.field = expression.LeftOperand;
    parentObj.type = "string";
    parentObj.data = {
        id: expression.ExpressionId,
        isRightOperandElement: expression.IsRightOperandElement,
        rightOperandElementName: expression.RightOperand,
        ltKeyFilter: expression.LeftKeyFilter ? expression.LeftKeyFilter : null,
        rtKeyFilter: expression.RightKeyFilter ? expression.RightKeyFilter : null
    };
    parentObj.input = "text";
    parentObj.operator = this.getoperator(expression.OperatorTypeId);
    if (expression.IsRightOperandElement == true) {
        var result = $.grep(this.searchGridData, function (val, idx) {
            return val.UIElementName == expression.RightOperand;
        });
        if (result != null && result.length > 0) {
            parentObj.value = result[0].Element;
        }
    }
    else {
        parentObj.value = expression.RightOperand;
    }

    if (expression.CompOp == null) {
        expression.CompOp = { Factor: '', FactorValue: '' };
    }

    if (currentInstance.isParentRepeater == true) {
        if ($.inArray(expression.OperatorTypeId, [4, 8, 9, 10]) >= 0) {
            parentObj.value = parentObj.value;
        }
        else {
            parentObj.value = [expression.CompOp.FactorValue, expression.CompOp.Factor, parentObj.value];
        }
    }
}

expressionBuilder.prototype.getoperator = function (operatorTypeId) {
    var operatorKey = "";
    switch (operatorTypeId) {
        case 1:
            operatorKey = 'ebs_equals';
            break;
        case 2:
            operatorKey = 'ebs_gt';
            break;
        case 3:
            operatorKey = 'ebs_lt';
            break;
        case 4:
            operatorKey = 'ebs_contains';
            break;
        case 5:
            operatorKey = 'ebs_ne';
            break;
        case 6:
            operatorKey = 'ebs_gte';
            break;
        case 7:
            operatorKey = 'ebs_lte';
            break;
        case 8:
            operatorKey = 'ebs_isnull';
            break;
        case 9:
            operatorKey = 'ebs_custom';
            break;
        case 10:
            operatorKey = 'ebs_notcontains';
            break;
        default:
            operatorKey = 'ebs_equals';
            break;
    }
    return operatorKey;
}

expressionBuilder.prototype.buildfilters = function (operators) {
    var filters = [];
    var operators = ['ebs_equals', 'ebs_gt', 'ebs_lt', 'ebs_contains', 'ebs_ne', 'ebs_gte', 'ebs_lte', 'ebs_isnull', 'ebs_custom', 'ebs_notcontains'];
    $.each(this.searchGridData, function (idx, val) {
        var entry = {};
        entry.id = val.UIElementName;
        entry.label = val.Element;
        entry.type = "string";
        entry.operators = operators;
        entry.validation = { allow_empty_value: true };
        filters.push(entry);
    });
    return filters;
}

expressionBuilder.prototype.render = function () {
    var currentInstance = this;
    currentInstance.setParentRepeater();
    var rules = this.buildrules();
    var filters = this.buildfilters();
    var opCount = currentInstance.isParentRepeater == true ? 3 : 1;
    var containerElementJQ = "#" + this.containerElementID;
    $(containerElementJQ).queryBuilder('destroy');
    $(containerElementJQ).queryBuilder({

        operators: $.fn.queryBuilder.constructor.DEFAULTS.operators.concat([
			    { type: 'ebs_equals', nb_inputs: opCount, multiple: false, apply_to: ['string'] },
			    { type: 'ebs_gt', nb_inputs: opCount, multiple: false, apply_to: ['string'] },
			    { type: 'ebs_lt', nb_inputs: opCount, multiple: false, apply_to: ['string'] },
			    { type: 'ebs_contains', nb_inputs: 1, multiple: false, apply_to: ['string'] },
			    { type: 'ebs_ne', nb_inputs: opCount, multiple: false, apply_to: ['string'] },
			    { type: 'ebs_gte', nb_inputs: opCount, multiple: false, apply_to: ['string'] },
			    { type: 'ebs_lte', nb_inputs: opCount, multiple: false, apply_to: ['string'] },
			    { type: 'ebs_isnull', nb_inputs: 1, multiple: false, apply_to: ['string'] },
			    { type: 'ebs_custom', nb_inputs: 1, multiple: false, apply_to: ['string'] },
			    { type: 'ebs_notcontains', nb_inputs: 1, multiple: false, apply_to: ['string'] }
        ]),

        lang: {
            operators: {
                ebs_equals: 'Equals ( = )',
                ebs_gt: 'Greater Than ( > )',
                ebs_lt: 'Less Than ( < )',
                ebs_contains: 'Contains',
                ebs_ne: 'Not Equals ( ! )',
                ebs_gte: 'Greater Than OR Equal To ( >=)',
                ebs_lte: 'Less Than OR Equal To ( <=)',
                ebs_isnull: 'Is NULL',
                ebs_custom: 'Custom',
                ebs_notcontains: 'Not Contains'
            }
        },
        allow_empty: true,

        plugins: ['ebs-elementpicker'],

        filters: filters,

        rules: rules,

        initiator: currentInstance,

        inputs_separator: ' ',

    });

    if (currentInstance.ruleObj.RuleDescription) {
        var containeRuleDesc = "#ruleDesc";
        $(containeRuleDesc).text(currentInstance.ruleObj.RuleDescription);
        $("#selectedRule").text(currentInstance.ruleObj.TargetProperty);
    }
}

expressionBuilder.prototype.setParentRepeater = function () {
    var currentInstance = this;

    if (currentInstance.uiElement.UIElementName.indexOf('Repeater') >= 0) {
        currentInstance.isParentRepeater = true;
    }
}


expressionBuilder.prototype.setCurrent = function () {
    this.isCurrent = true;
}

expressionBuilder.prototype.saveRule = function () {
    var returnVal = true;
    if (this.isCurrent == true) {
        var containerElementJQ = "#" + this.containerElementID;
        var res = $(containerElementJQ).queryBuilder('getRules');
        if (res != null) {
            var convertedRes = this.getNodeExpression(null, res);
            this.ruleObj.RootExpression = convertedRes;
        }
        else {
            returnVal = false;
        }
    }
    return returnVal;
}

expressionBuilder.prototype.getNodeExpression = function (parent, node) {
    var currentInstance = this;
    var currentNode = node;

    var matchExp = this.getOriginalExpression(currentNode);
    if (matchExp == null) {
        matchExp = this.createNewExpression(parent, node);
    }
    else {
        matchExp = this.updateExistingExpression(matchExp, parent, node);
    }
    $.each(node.rules, function (idx, rule) {
        if (matchExp.Expressions == null) {
            matchExp.Expressions = [];
        }
        if (rule.rules != null && rule.rules.length > 0) {
            var nodeExp = currentInstance.getNodeExpression(currentNode, rule);
            matchExp.Expressions.push(nodeExp);
        }
        else {
            var leafExp = currentInstance.getLeafExpression(currentNode, rule);
            matchExp.Expressions.push(leafExp);
        }
    });
    return matchExp;
}

expressionBuilder.prototype.getLeafExpression = function (parent, exp) {
    var matchExp = this.getOriginalExpression(exp);
    if (matchExp == null) {
        matchExp = this.createNewExpression(parent, exp);
    }
    else {
        matchExp = this.updateExistingExpression(matchExp, parent, exp);
    }
    return matchExp;
}

expressionBuilder.prototype.getOriginalExpression = function (rule) {
    var result = null;
    var filter = rule.data.id;
    if (filter > 0) {
        var res = $.grep(this.expressionsList, function (val, idx) {
            return val.ExpressionId == filter;
        });
        if (res != null) {
            result = res[0];
        }
    }
    return result;
}

expressionBuilder.prototype.createNewExpression = function (parent, exp) {
    var result = {
        TenantId: 1,
        UIElementId: 0,
        RuleId: 18748,
        ExpressionId: -1,
        LeftOperand: "",
        LeftOperandName: null,
        RightOperand: '',
        RightOperandName: null,
        OperatorTypeId: 1,
        LogicalOperatorTypeId: 1,
        IsRightOperandElement: false,
        Expressions: null,
        ParentExpressionId: -1,
        ExpressionTypeId: -1
    };
    if (exp != null) {
        if (exp.data != null) {
            result.ExpressionId = exp.data.id;
        }
        result = this.setExpressionValues(result, parent, exp);
    }
    return result;
}

expressionBuilder.prototype.updateExistingExpression = function (matchExp, parent, exp) {
    matchExp.Expressions = [];
    matchExp = this.setExpressionValues(matchExp, parent, exp);
    return matchExp;
}

expressionBuilder.prototype.setExpressionValues = function (matchExp, parent, exp) {
    var currentInstance = this;
    //TenantId
    matchExp.TenantId = this.tenantId;
    //RuleId
    matchExp.RuleId = this.ruleId;
    //LeftOperand - resolve value
    if (exp.id != undefined) {
        matchExp.LeftOperand = exp.id;
    }
    //OperatorTypeId - resolve value
    matchExp.OperatorTypeId = this.getoperatortypeid(exp.operator);
    //LogicalOperatorTypeId - resolve value
    matchExp.LogicalOperatorTypeId = 1;
    if (exp.condition == "AND") {
        matchExp.LogicalOperatorTypeId = 1;
    }
    if (exp.condition == "OR") {
        matchExp.LogicalOperatorTypeId = 2;
    }
    //IsRightOperandElement - resolve value
    if (exp.data != null && exp.data.isRightOperandElement != undefined) {
        matchExp.IsRightOperandElement = exp.data.isRightOperandElement;
    }
    else {
        matchExp.IsRightOperandElement = false;
    }
    //RightOperand - resolve value
    if (exp.id != undefined) {
        if (exp.data.isRightOperandElement == false) {
            matchExp.RightOperand = exp.value;
        }
        else {
            matchExp.RightOperand = exp.data.rightOperandElementName;
        }
    }

    //Get complex operator values
    if (currentInstance.isParentRepeater == true && $.isArray(exp.value)) {
        if ($.inArray(matchExp.OperatorTypeId, [4, 8, 9, 10]) >= 0) {
            matchExp.RightOperand = matchExp.RightOperand;
        }
        else {
            matchExp.RightOperand = exp.data.isRightOperandElement == false ? exp.value[2] : matchExp.RightOperand;
            matchExp.CompOp = { Factor: exp.value[1], FactorValue: exp.value[0] };
        }
    }

    //ParentExpressionId - resolve value
    if (parent != null && parent.data != null) {
        matchExp.ParentExpressionId = parent.data.id;
    }
    else {
        matchExp.ParentExpressionId = null;
    }
    //ExpressionTypeId - resolve value
    if (exp.rules != null) {
        matchExp.ExpressionTypeId = 1;
    }
    else {
        matchExp.ExpressionTypeId = 2;
    }

    if (exp.rules == null || exp.rules.length == 0) {
        matchExp.Expressions = null;
    }

    if (exp.data != null && exp.data.ltKeyFilter != null) {
        matchExp.LeftKeyFilter = exp.data.ltKeyFilter;
    }
    if (exp.data != null && exp.data.rtKeyFilter != null) {
        matchExp.RightKeyFilter = exp.data.rtKeyFilter;
    }
    return matchExp;
}

expressionBuilder.prototype.getoperatortypeid = function (operator) {
    var operatorKey = 1;
    switch (operator) {
        case 'ebs_equals':
            operatorKey = 1;
            break;
        case 'ebs_gt':
            operatorKey = 2;
            break;
        case 'ebs_lt':
            operatorKey = 3;
            break;
        case 'ebs_contains':
            operatorKey = 4;
            break;
        case 'ebs_ne':
            operatorKey = 5;
            break;
        case 'ebs_gte':
            operatorKey = 6;
            break;
        case 'ebs_lte':
            operatorKey = 7;
            break;
        case 'ebs_isnull':
            operatorKey = 8;
            break;
        case 'ebs_custom':
            operatorKey = 9;
            break;
        case 'ebs_notcontains':
            operatorKey = 10;
            break;
        default:
            operatorKey = 1;
            break;
    }
    return operatorKey;
}


expressionBuilder.prototype.close = function () {
    this.isCurrent = false;
    $('#' + this.containerElementID).empty();
}

var expressionBuilderSearchDialog = function (searchGridData, element, elementName, isRightOperand, ruleData) {
    this.searchGridData = searchGridData;
    this.element = element;
    this.elementName = elementName;
    this.isRightOperand = isRightOperand;
    this.ruleData = ruleData;
    this.containerElementJQ = '#expressionBuilderSearch';
    this.gridElementJQ = '#expressionBuilderSearchGrid';
    this.gridElement = 'expressionBuilderSearchGrid';
}

//this function is called below soon after clicking on
//pencil icon for custom rule on uielement property grid.
expressionBuilderSearchDialog.init = function () {
    $("<div id='expressionBuilderSearch'><div class='grid-wrapper'><table id='expressionBuilderSearchGrid'></div></table></div>").appendTo("body").dialog({
        autoOpen: false,
        width: 900,
        height: 500,
        modal: true
    });
}();

expressionBuilderSearchDialog.prototype.show = function () {
    $(this.containerElementJQ).dialog('option', 'title', 'Search Operand');
    $(this.containerElementJQ).dialog('open');
    this.loadGrid();
}


expressionBuilderSearchDialog.prototype.loadGrid = function () {
    //set column list
    var colArray = ['UIElementID', 'UIElementName', 'Section', 'Element'];
    //set column models
    var colModel = [];
    colModel.push({ name: 'UIElementID', index: 'UIElementID', key: true, hidden: true, search: false });
    colModel.push({ name: 'UIElementName', index: 'UIElementName', hidden: true });
    colModel.push({ name: 'Section', index: 'Section', align: 'left', editable: false });
    colModel.push({ name: 'Element', index: 'Element', align: 'left', editable: false });
    var currentInstance = this;
    //unload previous grid values
    $(this.gridElementJQ).jqGrid('GridUnload');

    $(this.gridElementJQ).parent().append("<div id='p" + currentInstance.gridElement + "'></div>");
    $(this.gridElementJQ).jqGrid({
        datatype: 'local',
        data: currentInstance.searchGridData,
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Elements',
        height: '300px',
        pager: '#p' + currentInstance.gridElement,
        //this is added for pagination.
        rowList: [10, 20, 30],
        rowNumber: 10,
        loadonce: true,
        autowidth: true,
        viewrecords: true,
        altRows: true,
        altclass: 'alternate',
        ignoreCase: true,
        hidegrid: false
    });
    $(this.gridElementJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });
    var pagerElement = '#p' + this.gridElement;
    $('#p' + this.gridElement).find('input').css('height', '20px');
    //remove default buttons
    $(this.gridElementJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

    $(this.gridElementJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-check',
        onClickButton: function () {
            var rowId = $(currentInstance.gridElementJQ).getGridParam('selrow');
            var selectedElement = $(currentInstance.gridElementJQ).jqGrid('getRowData', rowId);
            if (currentInstance.ruleData != null) {
                if (currentInstance.isRightOperand == true) {
                    currentInstance.ruleData.rightOperandElementName = selectedElement.UIElementName;
                    currentInstance.element.val(selectedElement.Element);
                    $(currentInstance.element).prop('title', selectedElement.Element);
                }
                else {
                    currentInstance.element.val(selectedElement.UIElementName);
                    $(currentInstance.element).prop('title', selectedElement.UIElementName);
                }
            }
            else {
                currentInstance.elementName.val(selectedElement.Element);
                currentInstance.element.val(selectedElement.UIElementName);
                $(currentInstance.element).prop('title', selectedElement.UIElementName);
            }
            currentInstance.element.change();
            currentInstance.close();

        }
    });
}

expressionBuilderSearchDialog.prototype.close = function () {
    $(this.containerElementJQ).dialog('close');
}


var repeaterKeyFilter = function (uiElement, searchGridData, target, data) {
    this.uiElement = uiElement;
    this.searchGridData = searchGridData;
    this.target = target;
    this.data = data;
    this.lastsel2;
}

repeaterKeyFilter._isInitialized = false;

repeaterKeyFilter.elementIDs = {
    rptKeyDialog: '#repeaterKeydialog',
    rptKeysGrid: 'repeaterkeygrid',
    rptKeysGridJQ: '#repeaterkeygrid',
}

repeaterKeyFilter.URLs = {
    leftKeyList: '/UIElement/GetLeftRepeaterKeys?tenantId=1&uiElementId={uiElementId}&expressionId={expressionId}',
    rightKeyList: '/UIElement/GetRightRepeaterKeys?tenantId=1&uiElementId={uiElementId}&expressionId={expressionId}',
    targetKeyList: '/UIElement/GetTargetRepeaterKeys?tenantId=1&uiElementId={uiElementId}&ruleId={ruleId}',
}

repeaterKeyFilter.showerror = function (xhr) {
    if (xhr.status == 999)
        this.location = '/Account/LogOn';
    else
        messageDialog.show(JSON.stringify(xhr));
}

repeaterKeyFilter.init = function () {
    if (repeaterKeyFilter._isInitialized == false) {
        $(repeaterKeyFilter.elementIDs.rptKeyDialog).dialog({
            autoOpen: false,
            height: '300',
            width: '40%',
            modal: true
        });
        repeaterKeyFilter._isInitialized = true;
    }
}();

repeaterKeyFilter.prototype.show = function () {
    var currentInstance = this;
    var getKeyList = "";

    $(repeaterKeyFilter.elementIDs.rptKeyDialog).off("dialogclose");
    $(repeaterKeyFilter.elementIDs.rptKeyDialog).on("dialogclose", function (event, ui) {
        currentInstance.close();
    });

    $(repeaterKeyFilter.elementIDs.rptKeyDialog).dialog('option', 'title', 'Repeater Key Filter - ' + currentInstance.target);
    $(repeaterKeyFilter.elementIDs.rptKeyDialog).dialog('open');
    if (currentInstance.target == 'Left-Operand') {
        if (currentInstance.data != null && currentInstance.data.ltKeyFilter != null && currentInstance.data.ltKeyFilter.length > 0) {
            currentInstance.loadKeyFilterGrid(currentInstance.data.ltKeyFilter);
            return;
        }
        else {
            getKeyList = repeaterKeyFilter.URLs.leftKeyList.replace(/\{uiElementId\}/g, currentInstance.uiElement.parent)
                                                           .replace(/\{expressionId\}/g, currentInstance.data.id);
        }
    }

    if (currentInstance.target == 'Right-Operand') {
        if (currentInstance.data != null && currentInstance.data.rtKeyFilter != null && currentInstance.data.rtKeyFilter.length > 0) {
            currentInstance.loadKeyFilterGrid(currentInstance.data.rtKeyFilter);
            return;
        }
        else {
            getKeyList = repeaterKeyFilter.URLs.rightKeyList.replace(/\{uiElementId\}/g, currentInstance.uiElement.parent)
                                                            .replace(/\{expressionId\}/g, currentInstance.data.id);
        }
    }

    if (currentInstance.target == 'Target') {
        if (currentInstance.data != null && currentInstance.data.TargetKeyFilter != null && currentInstance.data.TargetKeyFilter.length > 0) {
            currentInstance.loadKeyFilterGrid(currentInstance.data.TargetKeyFilter);
            return;
        }
        else {
            getKeyList = repeaterKeyFilter.URLs.targetKeyList.replace(/\{uiElementId\}/g, currentInstance.uiElement.parent)
                                                             .replace(/\{ruleId\}/g, currentInstance.data.RuleId);
        }
    }

    var promise = ajaxWrapper.getJSON(getKeyList);
    promise.done(function (xhr) {
        currentInstance.loadKeyFilterGrid(xhr);
    });
    promise.fail(repeaterKeyFilter.showerror);

}

repeaterKeyFilter.prototype.loadKeyFilterGrid = function (rptKeyData) {
    var currentInstance = this;


    var colArray = ['UIElementID', 'Key', 'UIElementPath', 'Apply', 'Value'];
    var colModel = [];
    colModel.push({ name: 'UIElementID', index: 'UIElementID', key: true, hidden: true, editable: false, align: 'left' });
    colModel.push({ name: 'Label', index: 'Label', editable: false, align: 'left' });
    colModel.push({ name: 'UIElementPath', index: 'UIElementPath', hidden: true });
    colModel.push({ name: 'isChecked', index: 'isChecked', width: 40, align: 'center', hidden: true, editable: false, edittype: 'checkbox', formatter: currentInstance.chkValueImageFmatter, unformat: currentInstance.chkValueImageUnFormat, editoptions: { value: 'true:false' }, formatoptions: { disabled: false } });
    colModel.push({ name: 'FilterValue', index: 'FilterValue', editable: true, align: 'left' });

    $(repeaterKeyFilter.elementIDs.rptKeysGridJQ).jqGrid('GridUnload');
    $(repeaterKeyFilter.elementIDs.rptKeysGridJQ).jqGrid({
        datatype: 'local',
        data: rptKeyData,
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Key Filter',
        height: '230',
        loadonce: true,
        rowNum: 20,
        rowList: [10, 20, 30],
        ignoreCase: true,
        autowidth: true,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        altclass: 'alternate',
        loadComplete: function () {

        },
        onSelectRow: function (id) {
            if (id && id !== currentInstance.lastsel2) {
                $(repeaterKeyFilter.elementIDs.rptKeysGridJQ).jqGrid('restoreRow', currentInstance.lastsel2);
                $(repeaterKeyFilter.elementIDs.rptKeysGridJQ).jqGrid('editRow', id, true);
                currentInstance.lastsel2 = id;
            }
        },
        beforeSelectRow: function (id, e) {
            if (id) {
                if (id != currentInstance.lastsel2)
                    $(repeaterKeyFilter.elementIDs.rptKeysGridJQ).jqGrid('saveRow', currentInstance.lastsel2, false);
            }
            return true;
        }
    });
}

repeaterKeyFilter.prototype.chkValueImageFmatter = function (cellvalue, options, rowObject) {
    if (cellvalue == "true" || cellvalue == true) {
        return '<span align="center" class="ui-icon ui-icon-check" style="margin:auto !important;" ></span>';
    }
    else {
        return '<span align="center" class="ui-icon ui-icon-close" style="margin:auto !important;" ></span>';
    }
}

repeaterKeyFilter.prototype.chkValueImageUnFormat = function (cellvalue, options, cell) {

    var checked = $(cell).children('span').attr('class');

    if (checked == "ui-icon ui-icon-check")
        return 'true';
    else
        return 'false';
}

repeaterKeyFilter.prototype.close = function () {
    var currentInstance = this;
    $(repeaterKeyFilter.elementIDs.rptKeysGridJQ).jqGrid('saveRow', currentInstance.lastsel2, false);
    var filterData = $(repeaterKeyFilter.elementIDs.rptKeysGridJQ).getRowData();

    if (currentInstance.validate(filterData) == true) {
        if (currentInstance.target == 'Left-Operand') {
            currentInstance.data.ltKeyFilter = filterData;
        }
        if (currentInstance.target == 'Right-Operand') {
            currentInstance.data.rtKeyFilter = filterData;
        }
        if (currentInstance.target == 'Target') {
            currentInstance.data.TargetKeyFilter = filterData;
        }
    }
    $(repeaterKeyFilter.elementIDs.rptKeyDialog).dialog('close');
}


repeaterKeyFilter.prototype.validate = function (data) {
    var result = false;
    $.each(data, function (idx, row) {
        if (row["FilterValue"] != "") {
            result = true;
        }
    });

    return result;
}
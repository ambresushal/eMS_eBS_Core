var repeaterRuleProcessor = function (formData, formRuleProcessor, repeaterBuilder) {
    this.formData = formData;
    this.formRuleProcessor = formRuleProcessor;
    this.repeaterBuilder = repeaterBuilder;
};

repeaterRuleProcessor.prototype.processRulesForChildRows = function (rule, containerName, rowData, childRowId) {
    var results = [];
    var parentRow = $.extend({}, rowData);
    var childName = rule.UIElementFullName.replace(containerName + '.', '');
    if (childName.indexOf('.') > 0) {
        childName = childName.split('.')[0];
    }
    var data = rowData[childName];

    if (data != null && data != undefined) {
        if (childRowId != null) {
            if (rule.ParentRepeaterType == 'In Line') {
                data = [data[childRowId]];
            }
            else {
                data = data.filter(function (dt) {
                    return dt.RowIDProperty == childRowId;
                });
            }
        }
        for (var idx = 0; idx < data.length; idx++) {
            var childRowData = data[idx];
            parentRow[childName] = childRowData;
            var result = this.processNode(rule.RootExpression, containerName, parentRow);
            results.push({ rowId: idx, result: result });
        }
    }
    return results;
}

repeaterRuleProcessor.prototype.processRule = function (rule, parentRowId, childRowId, repeaterDialogObj) {
    var elementName = rule.UIElementFullName;
    var repeaterData = this.getRepeaterDataFromElementName(elementName);
    var data = repeaterData.data;
    if (Array.isArray(data)) {
        if (rule != null) {
            if (rule.IsParentRepeater == true) { rule.RunForRow = true; };
        }
        if (parentRowId != null) {
            data = data.filter(function (dt) {
                return dt.RowIDProperty == parentRowId;
            });
        } //else {
        //    if (this.repeaterBuilder.gridOptions != undefined || this.repeaterBuilder.gridOptions != null) {
        //        data = this.getDisplayedRow(this.repeaterBuilder.gridOptions);
        //    }
        //}
        var repeaterName = repeaterData.repeaterName;
        if (rule.ParentRepeaterType == null || rule.ParentRepeaterType == "Dropdown") {
            if (rule.RunForRow == true) {
                if (data != null && data.length > 0) {
                    for (var idx = 0; idx < data.length; idx++) {
                        var targetRow = data[idx];
                        var result = false;

                        if (rule.TargetKeyFilters != null && rule.TargetKeyFilters != undefined && rule.TargetKeyFilters.length > 0) {
                            var isValidRow = this.isValidRowForComplexRule(rule, repeaterName, targetRow);
                            if (isValidRow)
                                targetRow = this.getKeyFilterRowData(rule.TargetKeyFilters, repeaterName);
                            else
                                continue;
                        }
                        result = this.processRuleForRow(rule, repeaterName, targetRow);
                        this.setTargetForRow(rule, targetRow, repeaterName, repeaterDialogObj, result);
                    }
                }
            }
            else {
                var result = this.formRuleProcessor.processRule(rule);
                if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Visible) {
                    if (repeaterDialogObj == undefined) {
                        if (this.repeaterBuilder.fullName == rule.UIElementFullName) {
                            this.repeaterBuilder.visibleRuleResultCallBack(rule, result);
                        } else {
                            var targetedRepeaterObj = currentInstance.formInstanceBuilder.repeaterBuilders.filter(function (elemName) {
                                return elemName.design.FullName == rule.UIElementFullName;
                            });

                            if (targetedRepeaterObj.length > 0) {
                                targetedRepeaterObj[0].visibleRuleResultCallBack(rule, result);
                            }
                        }
                    }
                    else {
                        repeaterDialogObj.visibleRuleResultCallBack(rule, result);
                    }
                }
                else {
                    for (var idx = 0; idx < data.length; idx++) {
                        this.setTargetForRow(rule, data[idx], repeaterName, repeaterDialogObj, result);
                    }
                }

            }
        }
        else if (rule.ParentRepeaterType == 'In Line' || rule.ParentRepeaterType == 'Child') {
            if (rule.RunForRow == true || rule.RunForParentRow == true) {
                if (data != null && data.length > 0) {
                    for (var idx = 0; idx < data.length; idx++) {
                        var results = this.processRulesForChildRows(rule, repeaterName, data[idx], childRowId);
                        this.setTargetForChildRows(data[idx], rule, repeaterName, parentRowId, childRowId, repeaterDialogObj, results);
                    }
                }
            }
            else {
                var result = this.formRuleProcessor.processRule(rule);
                if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Visible) {
                    if (repeaterDialogObj == undefined) {
                        this.repeaterBuilder.visibleRuleResultCallBack(rule, result);
                    }
                    else {
                        repeaterDialogObj.visibleRuleResultCallBack(rule, result);
                    }
                }
                else {
                    if (data != null && data.length > 0) {
                        for (var idx = 0; idx < data.length; idx++) {
                            this.setTargetForChildRows(data[idx], rule, repeaterName, parentRowId, childRowId, repeaterDialogObj, result);
                        }
                    }
                }
            }
        }
    }
}

repeaterRuleProcessor.prototype.getDisplayedRow = function (gridOptions) {
    var result = [];
    var rowCount = gridOptions.api.getDisplayedRowCount();
    var lastGridIndex = rowCount - 1;
    var currentPage = gridOptions.api.paginationGetCurrentPage();
    var pageSize = gridOptions.api.paginationGetPageSize();
    var startPageIndex = currentPage * pageSize;
    var endPageIndex = ((currentPage + 1) * pageSize) - 1;

    if (endPageIndex > lastGridIndex) {
        endPageIndex = lastGridIndex;
    }

    for (var i = startPageIndex; i <= endPageIndex; i++) {
        var rowNode = gridOptions.api.getDisplayedRowAtIndex(i);
        result.push(rowNode.data);
    }
    return result;
}

repeaterRuleProcessor.prototype.processFormRule = function (rule) {
    var result = this.formRuleProcessor.processRule(rule);
    return result;
}

repeaterRuleProcessor.prototype.processRuleForRow = function (rule, containerName, rowData) {
    var result = this.processNode(rule.RootExpression, containerName, rowData, rule);
    return result;
}

repeaterRuleProcessor.prototype.isValidRowForComplexRule = function (rule, containerName, rowData) {
    var validRowData = false;
    var validRow = this.getKeyFilterRowData(rule.TargetKeyFilters, containerName);
    if (rowData.RowIDProperty == validRow.RowIDProperty) {
        validRowData = true;
    } else {
        if (rule != null) {
            var expression = rule.RootExpression;
            if (expression.Expressions != null && expression.Expressions.length > 0) {
                for (var idx = 0; idx < expression.Expressions.length; idx++) {
                    var exp = expression.Expressions[idx];
                    if (rule.TargetKeyFilters != null && rule.TargetKeyFilters.length > 0) {
                        var leftContainerName = this.getRepeaterNameByDesignName(exp.LeftOperand.split('_')[0]);
                        var leftRow = this.getKeyFilterRowData(exp.LeftKeyFilters, leftContainerName);
                        if (leftRow.RowIDProperty == rowData.RowIDProperty) {
                            validRowData = true;
                            break;
                        } else {
                            if (exp.IsRightOperandElement == true) {
                                var rightContainerName = this.getRepeaterNameByDesignName(exp.RightOperand.split('_')[0]);
                                rightRow = this.getKeyFilterRowData(exp.RightKeyFilters, rightContainerName);
                                if (rightRow.RowIDProperty == rowData.RowIDProperty) {
                                    validRowData = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    return validRowData;
}

repeaterRuleProcessor.prototype.getKeyFilterRowData = function (keyFilters, repeaterName) {
    var targetInstances = currentInstance.formInstanceBuilder.repeaterBuilders.filter(function (elemName) {
        return elemName.fullName == repeaterName;
    });
    var repeaterDataRows;
    if (targetInstances.length > 0) {
        repeaterDataRows = targetInstances[0].data;
        for (var keyIndex = 0; keyIndex < keyFilters.length; keyIndex++) {
            var keyNameParts = keyFilters[keyIndex].RepeaterKeyName.split('.');
            var keyName = keyNameParts[(keyNameParts.length) - 1];
            repeaterDataRows = repeaterDataRows.filter(function (row) { return row[keyName] == keyFilters[keyIndex].RepeaterKeyValue; });
        }
    }
    return repeaterDataRows.length > 0 ? repeaterDataRows[0] : null;
}

repeaterRuleProcessor.prototype.getRepeaterNameByDesignName = function (designName) {
    var repeaterName = "";
    var targetInstances = currentInstance.formInstanceBuilder.repeaterBuilders.filter(function (elemName) {
        return elemName.design.Name == designName;
    });
    if (targetInstances.length > 0) {
        repeaterName = targetInstances[0].fullName;
    }
    return repeaterName;
}

repeaterRuleProcessor.prototype.processComplexLeaf = function (expression, containerName, leftcontainerName, leftKeyFilterRow, rightcontainerName, rightKeyFilterRow) {
    var leftOperand = this.getOperandValue(expression.LeftOperandName, expression.LeftOperand, leftcontainerName, leftKeyFilterRow, null, null);
    var operator = expression.OperatorTypeId;
    var rightOperand = expression.RightOperand;
    if (expression.IsRightOperandElement == true) {
        rightOperand = this.getOperandValue(expression.RightOperandName, expression.RightOperand, rightcontainerName, rightKeyFilterRow, null, null);
    }
    rightOperand = this.evaluateComplexOperator(expression, rightOperand);
    var result = this.evaluateExpression(leftOperand, operator, rightOperand, expression.IsRightOperandElement, expression.LeftOperand);
    return result;
}

repeaterRuleProcessor.prototype.evaluateComplexOperator = function (expression, operandValue) {
    var currentInstance = this;
    var result = operandValue;
    var prefix = operandValue.indexOf('$');
    var suffix = operandValue.indexOf('%');
    var complexOp = (expression.complexOp != undefined && expression.complexOp != null) ? expression.complexOp : null;
    if (complexOp != null) {
        var factor = (expression.complexOp.Factor != undefined && expression.complexOp.Factor != null) ? expression.complexOp.Factor.toLowerCase() : null;
        var factorValue = (expression.complexOp.FactorValue != undefined && expression.complexOp.FactorValue != null) ? expression.complexOp.FactorValue : null;
        operandValue = operandValue.replace('%', '').replace('$', '').replace(',', '');

        if (factor != null && factorValue != null) {
            switch (factor) {
                case '%':
                    result = ((operandValue * factorValue) / 100);
                    break;
                case '*':
                    result = operandValue * factorValue;
                    break;
                case '+':
                    result = parseFloat(operandValue) + factorValue;
                    break;
                case '-':
                    result = parseFloat(operandValue) - factorValue;
                    break;
                default:
                    result = currentInstance.processRightOperand(expression.OperatorTypeId, operandValue, factorValue);
                    break;
            }
        }
        if (prefix >= 0) { result = '$' + result; }
        if (suffix >= 0) { result = result + '%'; }
    }
    return result;
}

repeaterRuleProcessor.prototype.processRightOperand = function (operatorTypeId, operandValue, factorValue) {
    if (operatorTypeId == 3 || operatorTypeId == 7) {
        operandValue = parseFloat(operandValue) - factorValue;
    }

    if (operatorTypeId == 2 || operatorTypeId == 6) {
        operandValue = parseFloat(operandValue) + factorValue;
    }

    return operandValue;
}

repeaterRuleProcessor.prototype.processNode = function (expression, containerName, rowData, rule) {
    var isSuccess;
    var logicalOperatorTypeId = expression.LogicalOperatorTypeId;
    if (logicalOperatorTypeId == ruleProcessor.LogicalOperatorTypes.AND) {
        isSuccess = true;
    }
    else {
        isSuccess = false;
    }

    if (expression.Expressions != null && expression.Expressions.length > 0 && this.repeaterBuilder.colModel != undefined) {
        for (var idx = 0; idx < expression.Expressions.length; idx++) {
            var exp = expression.Expressions[idx];
            var sourceRepeaterName = null;
            var sourceDataRow = null;
            var leftKeyFilterRow = null;
            var leftKeyContainerName = null;
            var rightKeyFilterRow = null;
            var rightKeyContainerName = null;

            if (rule != null) {
                if (rule.IsParentRepeater == true && exp.LeftOperand != null && exp.LeftOperand != "") {
                    if (rule.TargetKeyFilters != null && rule.TargetKeyFilters.length > 0) {
                        var leftKeyContainerName = this.getRepeaterNameByDesignName(exp.LeftOperand.split('_')[0]);
                        leftKeyFilterRow = this.getKeyFilterRowData(exp.LeftKeyFilters, leftKeyContainerName);
                        if (exp.IsRightOperandElement == true) {
                            var rightKeyContainerName = this.getRepeaterNameByDesignName(exp.RightOperand.split('_')[0]);
                            rightKeyFilterRow = this.getKeyFilterRowData(exp.RightKeyFilters, rightKeyContainerName);
                        }
                    } else {
                        var targerDataKeyColumn = this.repeaterBuilder.colModel.filter(function (a) { return a.iskey == true });
                        var targerDataKeys = [];
                        for (var keyIndex = 0; keyIndex < targerDataKeyColumn.length; keyIndex++) {
                            targerDataKeys.push(targerDataKeyColumn[keyIndex].dataIndx);
                        }

                        var names = exp.LeftOperand.split('_');
                        var repeaterName = names[0];
                        var targetCurrentInstances = currentInstance.formInstanceBuilder.repeaterBuilders.filter(function (elemName) {
                            return elemName.design.Name == repeaterName;
                        });
                        if (targetCurrentInstances.length > 0) {
                            var sourceDataRows = targetCurrentInstances[0].data;
                            sourceRepeaterName = targetCurrentInstances[0].fullName;
                            var sourceDataKeyColumn = targetCurrentInstances[0].colModel.filter(function (a) { return a.iskey == true });
                            var sourceDataKeys = [];
                            for (var keyIndex = 0; keyIndex < targerDataKeyColumn.length; keyIndex++) {
                                sourceDataKeys.push(sourceDataKeyColumn[keyIndex].dataIndx);
                            }
                            for (var dataRowIdx = 0; dataRowIdx < sourceDataKeys.length; dataRowIdx++) {
                                sourceDataRows = sourceDataRows.filter(function (row) { return row[sourceDataKeys[dataRowIdx]] == rowData[targerDataKeys[dataRowIdx]]; });
                            }
                            sourceDataRow = sourceDataRows[0];
                        }
                    }
                }
            }

            var result;
            if (exp.ExpressionTypeId == ruleProcessor.ExpressionTypes.NODE) {
                result = this.processNode(exp, containerName, rowData, rule);
            }
            else {
                if (rule.TargetKeyFilters != null && rule.TargetKeyFilters.length > 0) {
                    result = leftKeyFilterRow != null ? this.processComplexLeaf(exp, containerName, leftKeyContainerName, leftKeyFilterRow, rightKeyContainerName, rightKeyFilterRow) : false;
                } else {
                    result = this.processLeaf(exp, containerName, rowData, sourceRepeaterName, sourceDataRow);
                }
            }
            if (logicalOperatorTypeId == ruleProcessor.LogicalOperatorTypes.AND) {
                if (result == false) {
                    isSuccess = false;
                    break;
                }
            }
            else {
                if (result == true) {
                    isSuccess = true;
                    break;
                }
            }
        }
    }
    return isSuccess;
}

repeaterRuleProcessor.prototype.processLeaf = function (expression, containerName, rowData, sourceRepeaterName, sourceDataRow) {
    var leftOperand = this.getOperandValue(expression.LeftOperandName, expression.LeftOperand, containerName, rowData, sourceRepeaterName, sourceDataRow);
    var operator = expression.OperatorTypeId;
    var rightOperand = expression.RightOperand;
    if (expression.IsRightOperandElement == true) {
        rightOperand = this.getOperandValue(expression.RightOperandName, expression.RightOperand, containerName, rowData, sourceRepeaterName, sourceDataRow);
    }
    var result = this.evaluateExpression(leftOperand, operator, rightOperand, expression.IsRightOperandElement, expression.LeftOperand);


    return result;
}

repeaterRuleProcessor.prototype.evaluateExpression = function (leftOperand, operator, rightOperand, isRightOperandElement, uiElementId) {
    var result = false;
    if (operator == ruleProcessor.OperatorTypes.Equals) {
        if (!isNaN(leftOperand) && !isNaN(rightOperand)) {
            var leftOperandNumber = Number(leftOperand);
            var rightOperandNumber = Number(rightOperand);
            result = leftOperandNumber == rightOperandNumber;
        }
        else {
            if (this.isCostShare(leftOperand, rightOperand)) {
                result = parseFloat(leftOperand.replace(/[^\d\.]/, '').replace(",", "")) == parseFloat(rightOperand.replace(/[^\d\.]/, '').replace(",", ""));
            }
            else {
                result = leftOperand == rightOperand;
            }
        }
    }
    else if (operator == ruleProcessor.OperatorTypes.GreaterThan) {
        if (uiElementId.indexOf('Calendar') > -1) {
            leftOperandDate = leftOperand.split('/');
            rightOperandDate = rightOperand.split('/');
            if (leftOperandDate[2] != undefined && rightOperandDate[2] != undefined) {
                if (leftOperandDate[2] == rightOperandDate[2]) {
                    if (leftOperandDate[0] == rightOperandDate[0]) {
                        result = leftOperandDate[1] > rightOperandDate[1];
                    }
                    else {
                        result = leftOperandDate[0] > rightOperandDate[0];
                    }
                }
                else {
                    result = leftOperandDate[2] > rightOperandDate[2];
                }
            }
            else {
                result = leftOperand > rightOperand;
            }
        }
        else if (!isNaN(leftOperand) && !isNaN(rightOperand)) {
            var leftOperandNumber = Number(leftOperand);
            var rightOperandNumber = Number(rightOperand);
            result = leftOperandNumber > rightOperandNumber;
        }
        else {
            if (this.isCostShare(leftOperand, rightOperand)) {
                result = parseFloat(leftOperand.replace(/[^\d\.]/, '').replace(",", "")) > parseFloat(rightOperand.replace(/[^\d\.]/, '').replace(",", ""));
            }
            else {
                result = leftOperand > rightOperand;
            }
        }
    }
    else if (operator == ruleProcessor.OperatorTypes.LessThan) {
        if (uiElementId.indexOf('Calendar') > -1) {
            leftOperandDate = leftOperand.split('/');
            rightOperandDate = rightOperand.split('/');
            if (leftOperandDate[2] != undefined && rightOperandDate[2] != undefined) {
                if (leftOperandDate[2] == rightOperandDate[2]) {
                    if (leftOperandDate[0] == rightOperandDate[0]) {
                        result = leftOperandDate[1] < rightOperandDate[1];
                    }
                    else {
                        result = leftOperandDate[0] < rightOperandDate[0];
                    }
                }
                else {
                    result = leftOperandDate[2] < rightOperandDate[2];
                }
            }
            else {
                result = leftOperand < rightOperand;
            }
        }
        else if (!isNaN(leftOperand) && !isNaN(rightOperand)) {
            var leftOperandNumber = Number(leftOperand);
            var rightOperandNumber = Number(rightOperand);
            result = leftOperandNumber < rightOperandNumber;
        }
        else {
            if (this.isCostShare(leftOperand, rightOperand)) {
                result = parseFloat(leftOperand.replace(/[^\d\.]/, '').replace(",", "")) < parseFloat(rightOperand.replace(/[^\d\.]/, '').replace(",", ""));
            }
            else {
                result = leftOperand < rightOperand;
            }
        }
    }
    else if (operator == ruleProcessor.OperatorTypes.Contains) {
        if (uiElementId.indexOf('DropDown') > -1) {
            if (leftOperand == Validation.selectOne) {
                leftOperand = "";
            }
        }
        result = leftOperand.indexOf(rightOperand) > -1;
    }
    else if (operator == ruleProcessor.OperatorTypes.NotEquals) {
        if (!isNaN(leftOperand) && !isNaN(rightOperand)) {
            var leftOperandNumber = Number(leftOperand);
            var rightOperandNumber = Number(rightOperand);
            result = leftOperandNumber != rightOperandNumber;
        }
        else {
            if (this.isCostShare(leftOperand, rightOperand)) {
                result = parseFloat(leftOperand.replace(/[^\d\.]/, '').replace(",", "")) != parseFloat(rightOperand.replace(/[^\d\.]/, '').replace(",", ""));
            } else {
                result = leftOperand != rightOperand;
            }
        }
    }
    else if (operator == ruleProcessor.OperatorTypes.GreaterThanOrEqualTo) {
        if (uiElementId.indexOf('Calendar') > -1) {
            leftOperandDate = leftOperand.split('/');
            rightOperandDate = rightOperand.split('/');
            if (leftOperandDate[2] != undefined && rightOperandDate[2] != undefined) {
                if (leftOperandDate[2] == rightOperandDate[2]) {
                    if (leftOperandDate[0] == rightOperandDate[0]) {
                        result = leftOperandDate[1] >= rightOperandDate[1];
                    }
                    else {
                        result = leftOperandDate[0] >= rightOperandDate[0];
                    }
                }
                else {
                    result = leftOperandDate[2] >= rightOperandDate[2];
                }
            }
        }
        else if (!isNaN(leftOperand) && !isNaN(rightOperand)) {
            var leftOperandNumber = Number(leftOperand);
            var rightOperandNumber = Number(rightOperand);
            result = leftOperandNumber >= rightOperandNumber;
        }
        else {
            if (this.isCostShare(leftOperand, rightOperand)) {
                result = parseFloat(leftOperand.replace(/[^\d\.]/, '').replace(",", "")) >= parseFloat(rightOperand.replace(/[^\d\.]/, '').replace(",", ""));
            } else {
                result = leftOperand >= rightOperand;
            }
        }
    }
    else if (operator == ruleProcessor.OperatorTypes.LessThanOrEqualTo) {
        if (uiElementId.indexOf('Calendar') > -1) {
            leftOperandDate = leftOperand.split('/');
            rightOperandDate = rightOperand.split('/');
            if (leftOperandDate[2] != undefined && rightOperandDate[2] != undefined) {
                if (leftOperandDate[2] == rightOperandDate[2]) {
                    if (leftOperandDate[0] == rightOperandDate[0]) {
                        result = leftOperandDate[1] <= rightOperandDate[1];
                    }
                    else {
                        result = leftOperandDate[0] <= rightOperandDate[0];
                    }
                }
                else {
                    result = leftOperandDate[2] <= rightOperandDate[2];
                }
            }
        }
        else if (!isNaN(leftOperand) && !isNaN(rightOperand)) {
            var leftOperandNumber = Number(leftOperand);
            var rightOperandNumber = Number(rightOperand);
            result = leftOperandNumber <= rightOperandNumber;
        }
        else {
            if (this.isCostShare(leftOperand, rightOperand)) {
                result = parseFloat(leftOperand.replace(/[^\d\.]/, '').replace(",", "")) <= parseFloat(rightOperand.replace(/[^\d\.]/, '').replace(",", ""));
            }
            else {
                result = leftOperand <= rightOperand;
            }
        }
    }
    else if (operator == ruleProcessor.OperatorTypes.IsNull) {
        if (uiElementId.indexOf('Radio') > -1 || uiElementId.indexOf('CheckBox') > -1) {
            if (leftOperand == "No") {
                result = true;
            }
            else {
                result = false;
            }
        }
        else if (uiElementId.indexOf('DropDown') > -1) {
            if (leftOperand == undefined || leftOperand == null || leftOperand == '' || leftOperand == "" || leftOperand == Validation.selectOne) {
                result = true;
            }
            else {
                result = false;
            }
        }
        else if (leftOperand == undefined || leftOperand == null || leftOperand == '' || leftOperand == "") {
            result = true;
        }
        else {
            result = false;
        }
    }
    return result;
}

repeaterRuleProcessor.prototype.isCostShare = function (leftOperand, rightOperand) {
    var isCostShare = false;
    try {
        if (leftOperand != undefined && rightOperand != undefined) {
            if (leftOperand.indexOf('$') > -1 && rightOperand.indexOf('$') > -1) isCostShare = true;
            else if (leftOperand.indexOf('%') > -1 && rightOperand.indexOf('%') > -1) isCostShare = true;
        }
    }
    catch (e) {
        isCostShare = false;
    }
    return isCostShare;
}

repeaterRuleProcessor.prototype.getOperandValue = function (operandFullName, operandElementName, containerName, rowData, sourceRepeaterName, sourceDataRow) {
    var operandValue;
    if (operandFullName.indexOf(containerName) == 0) {
        var elementName = operandFullName.replace(containerName + '.', '');
        var elementNameParts = elementName.split('.');
        var elemPart = '';
        for (var idx = 0; idx < elementNameParts.length; idx++) {
            if (idx == 0) {
                if (rowData != null && rowData.hasOwnProperty(elementNameParts[idx])) {
                    elemPart = rowData[elementNameParts[idx]];
                }
            }
            else if (idx == (elementNameParts.length - 1)) {
                if (rowData != null && rowData.hasOwnProperty(elementNameParts[idx])) {
                    elemPart = rowData[elementNameParts[idx]];
                }
            }
            else {
                elemPart = elemPart[elementNameParts[idx]];
            }
        }
        operandValue = elemPart;
    } else if (sourceRepeaterName != null & operandFullName.indexOf(sourceRepeaterName) == 0 && sourceRepeaterName !== "") {
        var elementName = operandFullName.replace(sourceRepeaterName + '.', '');
        var elementNameParts = elementName.split('.');
        var elemPart = '';
        for (var idx = 0; idx < elementNameParts.length; idx++) {
            if (sourceDataRow != undefined) {
                if (idx == 0) {
                    if (sourceDataRow.hasOwnProperty(elementNameParts[idx])) {
                        elemPart = sourceDataRow[elementNameParts[idx]];
                    }
                }
                else if (idx == (elementNameParts.length - 1)) {
                    if (sourceDataRow.hasOwnProperty(elementNameParts[idx])) {
                        elemPart = sourceDataRow[elementNameParts[idx]];
                    }
                }
                else {
                    elemPart = elemPart[elementNameParts[idx]];
                }
            }
        }
        operandValue = elemPart;
    }
    else {
        var nameParts = operandFullName.split(".");
        var dataPart = '';
        for (var idx = 0; idx < nameParts.length; idx++) {
            if (idx == 0) {
                dataPart = this.formData[nameParts[idx]];
            }
            else {
                dataPart = dataPart[nameParts[idx]];
            }
        }
        operandValue = dataPart;
    }
    if (operandElementName != null) {
        if (operandElementName.indexOf('CheckBox') > 0 || operandElementName.indexOf('Radio') > 0) {
            if (operandValue == true || operandValue == false || operandValue == 'true' || operandValue == 'false') {
                if (operandValue == true || operandValue == 'true') {
                    operandValue = 'Yes';
                }
                else {
                    operandValue = 'No';
                }
            }
        }
    }

    if (operandValue == null)
        operandValue = '';
    return operandValue;
}

repeaterRuleProcessor.prototype.getRepeaterDataFromElementName = function (elementName) {
    var dataPart;
    var repeaterName;
    var nameParts = elementName.split(".");
    var dataPart;
    for (var idx = 0; idx < nameParts.length; idx++) {
        if (idx == 0) {
            dataPart = this.formData[nameParts[idx]];
            repeaterName = nameParts[idx];
        }
        else {
            if (dataPart[nameParts[idx]]) {
                dataPart = dataPart[nameParts[idx]];
                repeaterName = repeaterName + '.' + nameParts[idx];
            }
        }
        if (Array.isArray(dataPart)) {
            break;
        }
    }
    return { data: dataPart, repeaterName: repeaterName };
}

repeaterRuleProcessor.prototype.setValueForRuleTarget = function (rule, result, rowData, repeaterName, rowToFind) {
    var retVal;
    if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Value) {
        if (result == true) {
            if (rule.IsResultSuccessElement == true) {
                if (rowToFind == null) {
                    rowData[rule.UIElementName] = this.getOperandValue(rule.SuccessValueFullName, '', repeaterName, rowData);
                }
                else {
                    rowData[rule.UIElementName] = this.getOperandValue(rule.SuccessValueFullName, '', repeaterName, rowToFind);
                }
            }
            else {
                rowData[rule.UIElementName] = rule.SuccessValue;
            }
        }
        else {
            if (rule.IsResultFailureElement == true) {
                if (rowToFind == null) {
                    rowData[rule.UIElementName] = this.getOperandValue(rule.FailureValueFullName, '', repeaterName, rowData);
                }
                else {
                    rowData[rule.UIElementName] = this.getOperandValue(rule.FailureValueFullName, '', repeaterName, rowToFind);
                }
            }
            else {
                rowData[rule.UIElementName] = rule.FailureValue;
            }
        }
    }
    retVal = rowData[rule.UIElementName];
    if (retVal == '') {
        retVal = undefined;
    }
    return retVal;
}

repeaterRuleProcessor.prototype.setTargetForRow = function (rule, row, repeaterName, repeaterDialogObj, result) {
    if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Value) {
        var retVal = this.setValueForRuleTarget(rule, result, row, repeaterName, null);
        if (this.repeaterBuilder != null) {
            if (repeaterDialogObj == undefined) {
                this.repeaterBuilder.ruleResultCallBack(rule, row, retVal);
            }
            else {
                repeaterDialogObj.ruleResultCallBack(rule, row, retVal);
            }
        }
    }
    else {
        if (this.repeaterBuilder != null) {
            if (repeaterDialogObj == undefined) {
                this.repeaterBuilder.ruleResultCallBack(rule, row, result);
            }
            else {
                repeaterDialogObj.ruleResultCallBack(rule, row, result);
            }
        }
    }
}

repeaterRuleProcessor.prototype.setTargetForChildRows = function (row, rule, repeaterName, parentRowId, childRowId, repeaterDialogObj, results) {
    var parentRow = $.extend({}, row);
    var childName = rule.UIElementFullName.replace(repeaterName + '.', '');
    if (childName.indexOf('.') > 0) {
        childName = childName.split('.')[0];
    }
    var data = row[childName];
    if (data != null && data != undefined) {
        if (childRowId != null) {
            if (rule.ParentRepeaterType == 'In Line') {
                data = [data[childRowId]];
            }
            else {
                data = data.filter(function (dt) {
                    return dt.RowIDProperty == childRowId;
                });
            }
        }
        for (var childIdx = 0; childIdx < data.length; childIdx++) {
            parentRow[childName] = data[childIdx];
            var result;
            if (Array.isArray(results) == true) {
                result = results[childIdx].result;
            }
            else {
                result = results;
            }
            if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Value) {
                var retVal = this.setValueForRuleTarget(rule, result, data[childIdx], repeaterName, parentRow);
                if (this.repeaterBuilder != null) {
                    if (childRowId != null && data.length == 1) {
                        if (repeaterDialogObj == undefined) {
                            this.repeaterBuilder.ruleResultCallBack(rule, parentRow, retVal, childName, childRowId);
                        }
                        else {
                            repeaterDialogObj.ruleResultCallBack(rule, parentRow, retVal, childName, childRowId);
                        }
                    }
                    else {
                        if (repeaterDialogObj == undefined) {
                            this.repeaterBuilder.ruleResultCallBack(rule, parentRow, retVal, childName, childIdx);
                        }
                        else {
                            repeaterDialogObj.ruleResultCallBack(rule, parentRow, retVal, childName, childIdx);
                        }
                    }
                }
            }
            else {
                if (this.repeaterBuilder != null) {
                    if (childRowId != null && data.length == 1) {
                        if (repeaterDialogObj == undefined) {
                            this.repeaterBuilder.ruleResultCallBack(rule, parentRow, result, childName, childRowId);
                        }
                        else {
                            repeaterDialogObj.ruleResultCallBack(rule, parentRow, result, childName, childRowId);
                        }
                    }
                    else {
                        if (repeaterDialogObj == undefined) {
                            this.repeaterBuilder.ruleResultCallBack(rule, parentRow, result, childName, childIdx);
                        }
                        else {
                            repeaterDialogObj.ruleResultCallBack(rule, parentRow, result, childName, childIdx);
                        }
                    }
                }
            }
        }
    }

}
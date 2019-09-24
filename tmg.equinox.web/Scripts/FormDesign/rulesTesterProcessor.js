var rulesTesterProcessor = function (testData) {
    this.testData = testData;
};

rulesTesterProcessor.OperatorTypes = {
    Equals: 1,
    GreaterThan: 2,
    LessThan: 3,
    Contains: 4,
    NotEquals: 5,
    GreaterThanOrEqualTo: 6,
    LessThanOrEqualTo: 7,
    IsNull: 8,
    Custom: 9,
    NotContains: 10,

}

rulesTesterProcessor.LogicalOperatorTypes = {
    AND: 1,
    OR: 2
}

rulesTesterProcessor.ExpressionTypes = {
    NODE: 1,
    LEAF: 2
}

rulesTesterProcessor.TargetPropertyTypes = {
    Enabled: 1,
    RunValidation: 2,
    Value: 3,
    Visible: 4,
    IsRequired: 5,
    Error: 6,
    Highlight: 8,
    Dialog: 9,
    CustomRule: 10
}

rulesTesterProcessor.prototype.processRule = function (rule) {
    var result = this.processNode(rule.RootExpression);
    return result;
}

rulesTesterProcessor.prototype.processNode = function (expression) {
    var isSuccess;
    var logicalOperatorTypeId = expression.LogicalOperatorTypeId;
    if (logicalOperatorTypeId == rulesTesterProcessor.LogicalOperatorTypes.AND) {
        isSuccess = true;
    }
    else {
        isSuccess = false;
    }
    if (expression.Expressions != null && expression.Expressions.length > 0) {
        for (var idx = 0; idx < expression.Expressions.length; idx++) {
            var exp = expression.Expressions[idx];
            var result;
            if (exp.ExpressionTypeId == rulesTesterProcessor.ExpressionTypes.NODE) {
                result = this.processNode(exp);
            }
            else {
                result = this.processLeaf(exp);
            }
            if (logicalOperatorTypeId == rulesTesterProcessor.LogicalOperatorTypes.AND) {
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

rulesTesterProcessor.prototype.processLeaf = function (expression) {
    var leftOperand = this.getOperandValue(expression.LeftOperandName, expression.LeftOperand);
    var operator = expression.OperatorTypeId;
    var rightOperand = expression.RightOperand;
    if (expression.IsRightOperandElement == true) {
        rightOperand = this.getOperandValue(expression.RightOperandName, expression.RightOperand);
    }
    var result = this.evaluateExpression(leftOperand, operator, rightOperand, expression.IsRightOperandElement, expression.LeftOperand);
    return result;
}

rulesTesterProcessor.prototype.evaluateExpression = function (leftOperand, operator, rightOperand, isRightOperandElement, uiElementId) {
    var result = false;
    if (operator == rulesTesterProcessor.OperatorTypes.Equals) {
        if (!isNaN(leftOperand) && !isNaN(rightOperand)) {
            var leftOperandNumber = Number(leftOperand);
            var rightOperandNumber = Number(rightOperand);
            result = leftOperandNumber == rightOperandNumber;
        }
        else {
            result = leftOperand == rightOperand;
        }
    }
    else if (operator == rulesTesterProcessor.OperatorTypes.GreaterThan) {
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
        else {
            try {
                var isArray = this.isArray(leftOperand);
                if (isArray == true) {
                    var length = leftOperand.length;
                    result = length > parseFloat(rightOperand);
                }
                else if (this.isCostShare(leftOperand, rightOperand)) {
                    result = parseFloat(leftOperand.replace(/[^\d\.]/, '')) > parseFloat(rightOperand.replace(/[^\d\.]/, ''));
                }
                else {
                    result = parseFloat(leftOperand) > parseFloat(rightOperand);
                }
            }
            catch (ex) {
                result = leftOperand > rightOperand;
            }
        }
    }
    else if (operator == rulesTesterProcessor.OperatorTypes.LessThan) {
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
        else {
            try {
                var isArray = this.isArray(leftOperand);
                if (isArray == true) {
                    var length = leftOperand.length;
                    result = length < parseFloat(rightOperand);
                }
                else if (this.isCostShare(leftOperand, rightOperand)) {
                    result = parseFloat(leftOperand.replace(/[^\d\.]/, '')) < parseFloat(rightOperand.replace(/[^\d\.]/, ''));
                }
                else {
                    result = parseFloat(leftOperand) < parseFloat(rightOperand);
                }
            }
            catch (ex) {
                result = leftOperand < rightOperand;
            }
        }
    }
    else if (operator == rulesTesterProcessor.OperatorTypes.Contains) {
        if (uiElementId.indexOf('DropDown') > -1) {
            if (leftOperand == Validation.selectOne) {
                leftOperand = "";
            }
        }
        result = leftOperand.indexOf(rightOperand) > -1;
    }
    else if (operator == rulesTesterProcessor.OperatorTypes.NotContains) {
        if (uiElementId.indexOf('DropDown') > -1) {
            if (leftOperand == Validation.selectOne) {
                leftOperand = "";
            }
        }
        result = leftOperand.indexOf(rightOperand) < 0;
    }
    else if (operator == rulesTesterProcessor.OperatorTypes.NotEquals) {
        if (!isNaN(leftOperand) && !isNaN(rightOperand) && leftOperand != '' && rightOperand != '' && leftOperand != "" && rightOperand != "") {
            var leftOperandNumber = Number(leftOperand);
            var rightOperandNumber = Number(rightOperand);
            result = leftOperandNumber != rightOperandNumber;
        }
        else {
            result = leftOperand != rightOperand;
        }
    }
    else if (operator == rulesTesterProcessor.OperatorTypes.GreaterThanOrEqualTo) {
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
            else {
                result = leftOperand >= rightOperand;
            }
        }
        else {
            try {
                var isArray = this.isArray(leftOperand);
                if (isArray == true) {
                    var length = leftOperand.length;
                    result = length >= parseFloat(rightOperand);
                }
                else if (this.isCostShare(leftOperand, rightOperand)) {
                    result = parseFloat(leftOperand.replace(/[^\d\.]/, '')) >= parseFloat(rightOperand.replace(/[^\d\.]/, ''));
                }
                else {
                    result = parseFloat(leftOperand) >= parseFloat(rightOperand);
                }
            }
            catch (ex) {
                result = leftOperand >= rightOperand;
            }
        }
    }
    else if (operator == rulesTesterProcessor.OperatorTypes.LessThanOrEqualTo) {
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
            else {
                result = leftOperand <= rightOperand;
            }
        }
        else {
            try {
                var isArray = this.isArray(leftOperand);
                if (isArray == true) {
                    var length = leftOperand.length;
                    result = length <= parseFloat(rightOperand);
                }
                else if (this.isCostShare(leftOperand, rightOperand)) {
                    result = parseFloat(leftOperand.replace(/[^\d\.]/, '')) <= parseFloat(rightOperand.replace(/[^\d\.]/, ''));
                }
                else {
                    result = parseFloat(leftOperand) <= parseFloat(rightOperand);
                }
            }
            catch (ex) {
                result = leftOperand <= rightOperand;
            }
        }
    }
    else if (operator == rulesTesterProcessor.OperatorTypes.IsNull) {
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
    else if (operator == rulesTesterProcessor.OperatorTypes.Custom) {
        result = true;
    }
    return result;
}

rulesTesterProcessor.prototype.isArray = function (leftOperand) {
    var isArray = false;
    try {
        if (leftOperand != undefined) {
            isArray = leftOperand instanceof Array;
        }
    } catch (e) {
        isArray = false;
    }
    return isArray;
}

rulesTesterProcessor.prototype.isCostShare = function (leftOperand, rightOperand) {
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

rulesTesterProcessor.prototype.getOperandValue = function (operandFullName, operandElementName) {
    var dataPart;
    var nameParts;
    //var nameParts = operandFullName.split(".");
    //for (var idx = 0; idx < nameParts.length; idx++) {
    //    if (idx == 0) {
    //        dataPart = this.testData[nameParts[idx]];
    //    }
    //    else {
    //        if (dataPart != undefined || dataPart != null) {
    //            dataPart = dataPart[nameParts[idx]];
    //        }
    //    }
    //}
    for (var property in this.testData) {
        var endIndex = property.indexOf("#!");
        if (endIndex > 0) {
            nameParts = property.substr(0, endIndex);
            if (operandElementName == nameParts) {
                nameParts = property;
                break;
            }
        }
    }

    dataPart = this.testData[nameParts];
    if (dataPart == null) {
        dataPart = '';
    }
    if (operandElementName != null) {
        if (operandElementName.indexOf('CheckBox') > 0 || operandElementName.indexOf('Radio') > 0) {
            if (dataPart == true || dataPart == false || dataPart == 'true' || dataPart == 'false' || dataPart == 'True' || dataPart == 'False') {
                if (dataPart == true || dataPart == 'true' || dataPart == 'True') {
                    dataPart = 'Yes';
                }
                else {
                    dataPart = 'No';
                }
            }
        }
    }

    //Handle RichTextBox Html
    if (isHTML(dataPart)) {
        var divElement = document.createElement('div');
        divElement.innerHTML = dataPart;
        dataPart = $(divElement).text();
    }
    return dataPart;
}

rulesTesterProcessor.prototype.setPropertyValue = function (elementFullName, value) {
    var dataPart;
    var nameParts;
    //var nameParts = elementFullName.split(".");
    //for (var idx = 0; idx < nameParts.length - 1; idx++) {
    //    if (idx == 0) {
    //        dataPart = this.testData[nameParts[idx]];
    //    }
    //    else {
    //        dataPart = dataPart[nameParts[idx]];
    //    }
    //}
    for (var property in this.testData) {
        var endIndex = property.indexOf("#!");
        if (endIndex > 0) {
            nameParts = property.substr(0, endIndex);
            if (operandElementName == nameParts) {
                nameParts = property;
                break;
            }
        }
    }

    dataPart = this.testData[nameParts];
    if (value == null) {
        value = '';
    }
    dataPart[nameParts[nameParts.length - 1]] = value;
}



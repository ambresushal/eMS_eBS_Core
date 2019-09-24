var ruleProcessor = function (formData) {
    this.formData = formData;
};

ruleProcessor.OperatorTypes = {
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

ruleProcessor.LogicalOperatorTypes = {
    AND: 1,
    OR: 2
}

ruleProcessor.ExpressionTypes = {
    NODE: 1,
    LEAF: 2
}

ruleProcessor.TargetPropertyTypes = {
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

ruleProcessor.prototype.processRule = function (rule) {
    var result = this.processNode(rule.RootExpression);
    return result;
}

ruleProcessor.prototype.processNode = function (expression) {
    var isSuccess;
    var logicalOperatorTypeId = expression.LogicalOperatorTypeId;
    if (logicalOperatorTypeId == ruleProcessor.LogicalOperatorTypes.AND) {
        isSuccess = true;
    }
    else {
        isSuccess = false;
    }
    if (expression.Expressions != null && expression.Expressions.length > 0) {
        for (var idx = 0; idx < expression.Expressions.length; idx++) {
            var exp = expression.Expressions[idx];
            var result;
            if (exp.ExpressionTypeId == ruleProcessor.ExpressionTypes.NODE) {
                result = this.processNode(exp);
            }
            else {
                result = this.processLeaf(exp);
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

ruleProcessor.prototype.processLeaf = function (expression) {
    var leftOperand = this.getOperandValue(expression.LeftOperandName, expression.LeftOperand);
    var operator = expression.OperatorTypeId;
    var rightOperand = expression.RightOperand;
    if (expression.IsRightOperandElement == true) {
        rightOperand = this.getOperandValue(expression.RightOperandName, expression.RightOperand);
    }
    var result = this.evaluateExpression(leftOperand, operator, rightOperand, expression.IsRightOperandElement, expression.LeftOperand);
    return result;
}

ruleProcessor.prototype.evaluateExpression = function (leftOperand, operator, rightOperand, isRightOperandElement, uiElementId) {
    var result = false;
    if (operator == ruleProcessor.OperatorTypes.Equals) {
        if (!isNaN(leftOperand) && !isNaN(rightOperand)) {
            var leftOperandNumber = Number(leftOperand);
            var rightOperandNumber = Number(rightOperand);
            result = leftOperandNumber == rightOperandNumber;
        }
        else {
            result = leftOperand == rightOperand;
        }
    }
    else if (operator == ruleProcessor.OperatorTypes.GreaterThan) {
        if (uiElementId.indexOf('Calendar') > -1) {
            leftOperandDate = leftOperand.split('/');
            rightOperandDate=rightOperand.split('/');
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
                if (isArray == true)
                {
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
    else if (operator == ruleProcessor.OperatorTypes.Contains) {
        if (uiElementId.indexOf('DropDown') > -1) {
            if (leftOperand == Validation.selectOne) {
                leftOperand = "";
            }
        }
        result = leftOperand.indexOf(rightOperand) > -1;
    }
    else if (operator == ruleProcessor.OperatorTypes.NotContains) {
        if (uiElementId.indexOf('DropDown') > -1) {
            if (leftOperand == Validation.selectOne) {
                leftOperand = "";
            }
        }
        result = leftOperand.indexOf(rightOperand) < 0;
    }
    else if (operator == ruleProcessor.OperatorTypes.NotEquals) {
        if (!isNaN(leftOperand) && !isNaN(rightOperand) && leftOperand != '' && rightOperand != '' && leftOperand != "" && rightOperand != "") {
            var leftOperandNumber = Number(leftOperand);
            var rightOperandNumber = Number(rightOperand);
            result = leftOperandNumber != rightOperandNumber;
        }
        else {
            result = leftOperand != rightOperand;
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
        if (rightOperand == "FALSE" || rightOperand == 'FALSE' || rightOperand == false) {
            result = result == true ? false : true;
        }
    }
    else if (operator == ruleProcessor.OperatorTypes.Custom) {
        result = true;
    }
    return result;
}

ruleProcessor.prototype.isArray = function (leftOperand) {
    var isArray = false;
    try
    {
        if (leftOperand != undefined)
        {
            isArray = leftOperand instanceof Array;
        }
    } catch (e) {
        isArray = false;
    }
    return isArray;
}

ruleProcessor.prototype.isCostShare = function (leftOperand, rightOperand) {
    var isCostShare = false;
    try {
        if (leftOperand != undefined && rightOperand!=undefined) 
        {
            if (leftOperand.indexOf('$') > -1 && rightOperand.indexOf('$') > -1) isCostShare = true;
            else if (leftOperand.indexOf('%') > -1 && rightOperand.indexOf('%') > -1) isCostShare = true;
        }
    }
    catch (e) {
        isCostShare = false;
    }
    return isCostShare;
}

ruleProcessor.prototype.getOperandValue = function (operandFullName, operandElementName) {
    var dataPart;
    var nameParts = operandFullName.split(".");
    for (var idx = 0; idx < nameParts.length; idx++) {
        if (idx == 0) {
            dataPart = this.formData[nameParts[idx]];
        }
        else {
            if (dataPart != undefined || dataPart != null) {
                dataPart = dataPart[nameParts[idx]];
            }
        }
    }
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

ruleProcessor.prototype.setPropertyValue = function (elementFullName, value) {
    var dataPart;
    var nameParts = elementFullName.split(".");
    for (var idx = 0; idx < nameParts.length - 1; idx++) {
        if (idx == 0) {
            dataPart = this.formData[nameParts[idx]];
        }
        else {
            dataPart = dataPart[nameParts[idx]];
        }
    }
    if (value == null) {
        value = '';
    }
    dataPart[nameParts[nameParts.length - 1]] = value;
}



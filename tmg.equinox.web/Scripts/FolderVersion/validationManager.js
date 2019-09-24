var validationManager = function () {

    function init() {

    }

    init();

    var currentInstance = this;

    function checkRequiredValidation(validationObject, value, control) {
        if (validationObject) {
            if (validationObject.IsRequired) {
                var tagName = $(control).prop("tagName");
                if (!isDisabled(control) && isVisible(control)) {
                    switch (tagName.toLowerCase()) {
                        case 'select':
                            if (value === Validation.selectOne || value === '') {
                                //has validation error
                                return true;
                            }
                            else {
                                //does not have validation error
                                return false;
                            }
                            break;
                        case 'input':
                            //check if control is radio/checkbox
                            var type = $(control).prop("type")
                            if (new RegExp("radio|checkbox").test(type)) {
                                if ($(control).is(":checked")) {
                                    //has validation error
                                    return false;
                                }
                                else {
                                    //does not have validation error
                                    return true;
                                }
                            }
                            else if (value === '') {
                                //has validation error
                                return true;
                            }
                            else {
                                //does not have validation error
                                return false;
                            }
                            break;
                    }
                }
            }
        }
    }

    function checkMaxLengthValidation(validationObject, value, control) {
        if (validationObject) {
            if (validationObject.HasMaxLength) {
                if (isVisible(control)) {
                    if (value.length > validationObject.MaxLength) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }
        }
    }

    function checkRegexValidation(validationObject, value, control) {
        if (validationObject) {
            if (validationObject.Regex) {
                //check if control is visible/ if not visible refers to control is been disabled by some rule.
                if (isVisible(control)) {
                    //var regex = new RegExp(validationObject.Regex.replace(/[-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g, "\\$&"));
                    var regex = new RegExp(validationObject.Regex);

                    if (!regex.test(value)) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }
        }
    }

    function isDisabled(element) {
        if ($(element).is(":radio") || $(element).is(":checkbox"))
            return $(element).is(":disabled");
        else
            return $(element).is(":disabled") || $(element).is("[readOnly]");
    }

    function isVisible(element) {
        return $(element).css('display') != "none";
    }

    function checkRangeValidation(currentValue, minValue, maxValue, control) {
        if (!isDisabled(control) && isVisible(control)) {
            if ((currentValue != undefined && currentValue != "") && (maxValue != undefined && maxValue != "") && (minValue != undefined || minValue != "")) {
                return (validateMaxValue(currentValue, maxValue) && validateMinValue(currentValue, minValue));
            }
            else
                return true;
        }
    }

    function checkMaxValueValidation(currentValue, maxValue, control) {
        if (!isDisabled(control) && isVisible(control)) {
            if ((currentValue != undefined && currentValue != "") && (maxValue != undefined && maxValue != "")) {
                return validateMaxValue(currentValue, maxValue);
            }
        }
    }

    function checkMinValueValidation(currentValue, minValue, control) {
        if (!isDisabled(control) && isVisible(control)) {
            if ((currentValue != undefined && currentValue != "") && (minValue != undefined || minValue != "")) {
                return validateMinValue(currentValue, minValue);
            }
        }
    }

    function validateMaxValue(currentValue, maxValue) {
        return (maxValue >= currentValue);
    }

    function validateMinValue(currentValue, minValue) {
        return (minValue <= currentValue);
    }

    return {
        applyRequiredValidation: function (validationObject, value, control) {
            return checkRequiredValidation(validationObject, value, control);
        },

        applyMaxLengthValidation: function (validationObject, value, control) {
            return checkMaxLengthValidation(validationObject, value, control);
        },

        applyRegexValidation: function (validationObject, value, control) {
            return checkRegexValidation(validationObject, value, control);
        },

        applyRangeValidation: function (currentValue, minValue, maxValue, control) {
            return checkRangeValidation(currentValue, minValue, maxValue, control);
        },

        applyMaxValueValidation: function (currentValue, maxValue, control) {
            return checkMaxValueValidation(currentValue, maxValue, control);
        },

        applyMinValueValidation: function (currentValue, minValue, control) {
            return checkMinValueValidation(currentValue, minValue, control);
        }
    }
}();
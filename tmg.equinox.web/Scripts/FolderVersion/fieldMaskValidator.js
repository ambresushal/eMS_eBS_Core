var fieldMaskValidator = function () {
    function applyMaskForFields(validations, formInstanceId) {
        var stringUtility = new globalutilities();
        $.each(validations, function (index, validation) {
            if (!stringUtility.stringMethods.isNullOrEmpty(validation.Regex) && !stringUtility.stringMethods.isNullOrEmpty(validation.MaskExpression)) {
                if (!stringUtility.stringMethods.isNullOrEmpty(validation.PlaceHolder)) {
                    placeHolderValue = validation.PlaceHolder;
                }
                else {
                    placeHolderValue = validation.MaskExpression;
                }
                $("#" + validation.UIElementName + formInstanceId).inputmask(validation.MaskExpression, { placeholder: placeHolderValue, greedy: false, rightAlign: false });
                $("#" + validation.UIElementName + formInstanceId).attr('placeholder', placeHolderValue);
            }
            else if (!stringUtility.stringMethods.isNullOrEmpty(validation.Regex)) {
                if (!stringUtility.stringMethods.isNullOrEmpty(validation.LibraryRegexName)) {
                    placeHolderValue = validation.PlaceHolder;
                    $("#" + validation.UIElementName + formInstanceId).inputmask('Regex', { regex: validation.Regex });
                    $("#" + validation.UIElementName + formInstanceId).attr('placeholder', placeHolderValue);
                }
                else if (validation.MaskFlag == true) {
                    $("#" + validation.UIElementName + formInstanceId).inputmask('Regex', { regex: validation.Regex });
                }
            }
        });
    }
    return {
        applyMask: function (validations, formInstanceId) {
            applyMaskForFields(validations, formInstanceId);
        }
    }
}

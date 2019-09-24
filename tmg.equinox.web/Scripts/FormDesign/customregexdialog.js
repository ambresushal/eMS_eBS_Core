//constructor for custom regex dialog.
//params:
//uiElement - uiElement of row selected of the Document Design Version UI ELements Grid
//formDesignVersionId - form design version id of the element

var customRegexDialog = function (uiElement, formDesignVersionId, customRegex, message, maskFlag) {
    this.uiElement = uiElement;
    this.formDesignVersionId = formDesignVersionId;
    this.customRegex = customRegex;
    this.message = message;
    // this.customFieldMask = fieldMask;
    this.maskFlag = maskFlag;

}


customRegexDialog._currentInstance = undefined;

//element ID's required for custom regex
//added in Views/FormDesign/Index.cshtml
customRegexDialog.elementIDs = {
    //id for dialog
    customRegexDialog: '#customRegexDialog',
    //id for textarea custom regex
    customRegex: '#customRegex',
    //id for textarea custom message
    validationMessage: '#validationMessage',
    //id for checkbox MaskFlag
    MaskFlag: '#maskFlag'
}

customRegexDialog._isInitialized = false;



customRegexDialog.init = function () {
    //register dialog
    if (customRegexDialog._isInitialized == false) {
        $(customRegexDialog.elementIDs.customRegexDialog).dialog({
            autoOpen: false,
            width: 500,
            height: 260,
            modal: true
        });
        customRegexDialog._isInitialized = true;
    }
}();


//this function open  the dialog for custom regex.
customRegexDialog.prototype.show = function (statustext) {
    customRegexDialog._currentInstance = this;
    var stringUtility = new globalutilities();
    $(customRegexDialog.elementIDs.customRegexDialog).dialog('option', 'title', DocumentDesign.customRegexDialog + this.uiElement.Label)
    $(customRegexDialog.elementIDs.customRegexDialog).dialog('open');
    if (statustext == "Finalized") {
        $(customRegexDialog.elementIDs.customRegexDialog + ' button').attr("disabled", "disabled");
    }
    if (!stringUtility.stringMethods.isNullOrEmpty(customRegexDialog._currentInstance.customRegex))
        $(customRegexDialog.elementIDs.customRegex).val(customRegexDialog._currentInstance.customRegex);
    else
        $(customRegexDialog.elementIDs.customRegex).val('');

    if (!stringUtility.stringMethods.isNullOrEmpty(customRegexDialog._currentInstance.message))
        $(customRegexDialog.elementIDs.validationMessage).val(customRegexDialog._currentInstance.message);
    else
        $(customRegexDialog.elementIDs.validationMessage).val('');

    if (customRegexDialog._currentInstance.maskFlag !== undefined && customRegexDialog._currentInstance.maskFlag != null) {
        $(customRegexDialog.elementIDs.MaskFlag).prop('checked', customRegexDialog._currentInstance.maskFlag);
    }
    else {
        $(customRegexDialog.elementIDs.MaskFlag).prop('checked', customRegexDialog._currentInstance.maskFlag);
    }

    if (ClientRolesForFormDesignAccess.indexOf(vbRole) >= 0 && this.uiElement.IsStandard == "true") {
        $(customRegexDialog.elementIDs.customRegex).attr('disabled', 'disabled');
        $(customRegexDialog.elementIDs.validationMessage).attr('disabled', 'disabled');
        $(customRegexDialog.elementIDs.MaskFlag).attr('disabled', 'disabled');
    } else {
        $(customRegexDialog.elementIDs.customRegex).removeAttr('disabled');
        $(customRegexDialog.elementIDs.validationMessage).removeAttr('disabled');
        $(customRegexDialog.elementIDs.MaskFlag).removeAttr('disabled');
    }
}

 
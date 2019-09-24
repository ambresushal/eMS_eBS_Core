var formValidationManager = function (formInstancebuilder) {
    var formInstancebuilder = formInstancebuilder;
    var currentInstance = this;
    var validationErrorList = new Array();
    var duplicationList = new Array();


    function handleErrorValidation(path, value, rowNum, columnNum, rowid, Message, fullName) {
        try {
            //get the validation object

            var validationObjectList = formInstancebuilder.designData.Validations.filter(function (ct) {
                return ct.FullName == path;
            });

            if (validationObjectList.length == 0) {
                validationObjectList = formInstancebuilder.designData.Validations.filter(function (ct) {
                    return ct.UIElementName == fullName;
                });
            }
            //changes to remove data source name from full name 
            var datasourceList = formInstancebuilder.designData.DataSources;
            if ((datasourceList != null || datasourceList != undefined) && datasourceList.length != '0') {
                var invalidFullName = undefined;
                var fullPath = undefined;
                var dataSourceName = undefined;

                for (i = 0 ; i < datasourceList.length; i++) {

                    for (index = 0 ; index < validationObjectList.length ; index++) {

                        if (validationObjectList[index].FullName.indexOf(datasourceList[i].DataSourceName) > -1) {
                            invalidFullName = validationObjectList[index].FullName;
                            var isDataSourceNameExistsInFullName = false;
                            var fullNameList = validationObjectList[index].FullName.split('.');
                            for (count = 0; count < fullNameList.length; count++) {
                                if (fullNameList[count].indexOf(datasourceList[i].DataSourceName) > -1) {
                                    //isDataSourceNameExistsInFullName = true; -- commented line as it removes the data source name from FullName in below code 
                                    dataSourceName = datasourceList[i].DataSourceName;

                                }
                            }
                        }
                    }
                }
                // if data source name exists in FullName then remove the data source name from FullName.
                if (isDataSourceNameExistsInFullName == true) {
                    if (invalidFullName != undefined) {
                        var fullPath = invalidFullName.replace(dataSourceName + ".", "");
                    }
                }

            }
            var validationObject = validationObjectList[0];

            if (validationObject) {
                var validationError = {
                    FullName: fullPath == undefined ? path : fullPath,
                    IsRequiredError: false,
                    IsError: false,
                    Message: '',
                    UIElementName: validationObject.UIElementName,
                    RowIdProperty: rowid === undefined ? "" : parseInt(rowid),
                    RowNumber: getRowNumber(rowNum),
                    ColumnNumber: columnNum,
                    GeneratedName: validationObject.FullName.substring(validationObject.FullName.lastIndexOf('.') + 1),
                    hasValidationError: false,
                    value: value
                };
                if (validationObject.PlaceHolder != null || validationObject.PlaceHolder != undefined) {
                    var qualifiedElementNameParts = validationObject.FullName.split(".");
                    var dataPortion;
                    for (var qualifiedElementNamePartsCount = 0; qualifiedElementNamePartsCount < qualifiedElementNameParts.length; qualifiedElementNamePartsCount++) {
                        if (qualifiedElementNamePartsCount == 0) {
                            dataPortion = formInstancebuilder.formData[qualifiedElementNameParts[qualifiedElementNamePartsCount]];
                        }
                        else {
                            dataPortion = dataPortion[qualifiedElementNameParts[qualifiedElementNamePartsCount]];
                            var regex = /^[_.,/\()-]+$/;
                            var stringUtility = new globalutilities();
                            if (dataPortion != null || dataPortion != undefined) {
                                if ((dataPortion == validationObject.PlaceHolder || regex.test(dataPortion)) && dataPortion.length > 0) {
                                    value = stringUtility.stringMethods.setEmptyString(value);
                                }
                            }
                        }

                    }
                }
                if (validationObject.IsRequired && validationObject.IsActive) {
                    //check if required 
                    if (validationObject.UIElementName.indexOf('CheckBox') > -1) {
                        if (value) {
                            //does not have validation error
                            validationError.hasValidationError = false;
                            validationError.IsRequiredError = false;
                        }
                        else {
                            //has validation error
                            //add object details to validationErrorObject
                            validationError.Message = validationObject.ValidationMessage || Validation.requiredMsg;
                            validationError.hasValidationError = true;
                            validationError.IsRequiredError = true;
                        }
                    }
                    else {
                        var stringUtility = new globalutilities();
                        if (stringUtility.stringMethods.isNullOrEmpty(value)) {
                            //has validation error
                            //add object details to validationErrorObject
                            if (validationError.IsError != 'true') {
                                validationError.Message = validationObject.ValidationMessage || Validation.requiredMsg;
                                validationError.hasValidationError = true;
                                validationError.IsRequiredError = true;
                            }
                        }
                        else {
                            //does not have validation error
                            var sectionNamePath = path.substring(0, path.lastIndexOf('.'));
                            if (sectionNamePath.indexOf('.') > 0) {
                                var sectionName = path.substring(0, sectionNamePath.indexOf('.'));
                            }
                            else {
                                var sectionName = sectionNamePath;
                            }
                            var sectionDetails = formInstancebuilder.designData.Sections.filter(function (ct) {
                                return ct.FullName == sectionName;
                            });

                            var elementDetails = getElementDetails(sectionDetails[0], path);
                            if (elementDetails.DataType == "date") {
                                var regexDate = new RegExp(/^(((0?[1-9]|1[012])\/(0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])\/(29|30)|(0?[13578]|1[02])\/31)\/(19|[2-9]\d)\d{2}|0?2\/29\/((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00)))$/);
                                if (!regexDate.test(value) && value != "" && value != null) {
                                    //has validation error
                                    //add object details to validationErrorObject
                                    validationError.Message = "" || Validation.invalidDateMsg;
                                    validationError.hasValidationError = true;
                                    validationError.IsRequiredError = false;
                                }
                                else {
                                    //does not have validation error
                                    validationError.hasValidationError = false;
                                }
                            }
                            else {
                                //does not have validation error
                                validationError.hasValidationError = false;
                                validationError.IsRequiredError = false;
                            }
                        }
                    }
                }
                if (!validationError.hasValidationError) {
                    if (validationObject.Regex != '' && validationObject.Regex != null && validationObject.Regex != undefined && validationObject.IsActive == true && value != '') {
                        var regex = new RegExp(validationObject.Regex);
                        // If datatype=float make value to two decimal fixed 
                        if (validationObject.DataType == "float" && !isNaN(value)) {
                            value = parseFloat(value).toFixed(2);
                            validationError.value = value;
                            formInstancebuilder.rules.setCustomJSRenderProperty(validationObject.FullName, value);
                        }
                        if (!regex.test(value)) {
                            //has validation error
                            //add object details to validationErrorObject
                            validationError.Message = validationObject.ValidationMessage || Validation.regexMsg;
                            validationError.hasValidationError = true;
                            validationError.IsRequiredError = false;
                        }
                        else {
                            if (validationObject.UIElementName.indexOf('DropDown') > -1) {
                                validationObject.MaxLength = validationObject.MaxLength == 0 ? 200 : validationObject.MaxLength;
                            }
                                //if (value.length > validationObject.MaxLength) {
                                //    validationError.Message = validationObject.ValidationMessage || Validation.exceedMaxLen;
                                //    validationError.hasValidationError = true;
                                //    validationError.IsRequiredError = false;
                                //}
                            else {
                                //does not have validation error
                                validationError.hasValidationError = false;
                            }
                        }
                    }
                        //check for the datatype validations 
                    else if (validationObject.DataType != '' && validationObject.DataType != null && validationObject.DataType != undefined && validationObject.IsActive == true) {
                        //check for int value validations
                        if (validationObject.DataType == "int") {
                            if (value != '') {
                                var regx = new RegExp("^[0-9]*$");
                                if (regx.test(value)) {
                                    if (value.length > validationObject.MaxLength) {
                                        validationError.Message = validationObject.ValidationMessage || Validation.exceedMaxLen;
                                        validationError.hasValidationError = true;
                                        validationError.IsRequiredError = false;
                                    }
                                    else {
                                        //does not have validation error
                                        validationError.hasValidationError = false;
                                    }
                                }
                                else {
                                    //has validation error
                                    //add object details to validationErrorObject
                                    validationError.Message = validationObject.ValidationMessage || Validation.invalidIntMsg;
                                    validationError.hasValidationError = true;
                                    validationError.IsRequiredError = false;
                                }
                            }
                        }
                            // check for float value validations
                        else if (validationObject.DataType == "float") {
                            var regexFloat = new RegExp(/^[+-]?\d+(\.\d+)?$/);
                            if (!regexFloat.test(value)) {
                                //has validation error
                                //add object details to validationErrorObject
                                validationError.Message = validationObject.ValidationMessage || Validation.invalidFloatMsg;
                                validationError.hasValidationError = true;
                                validationError.IsRequiredError = false;
                            }
                            else {
                                if (value.length > validationObject.MaxLength) {
                                    validationError.Message = validationObject.ValidationMessage || Validation.exceedMaxLen;
                                    validationError.hasValidationError = true;
                                    validationError.IsRequiredError = false;
                                }
                                else {
                                    //does not have validation error
                                    validationError.hasValidationError = false;
                                }
                            }
                        } else if (validationObject.DataType == "string") {
                            if (value.length > validationObject.MaxLength) {
                                validationError.Message = validationObject.ValidationMessage || Validation.exceedMaxLen;
                                validationError.hasValidationError = true;
                                validationError.IsRequiredError = false;
                            }
                        }
                    }
                }

                if (validationObject.IsError || validationError.hasValidationError) {
                    if (validationError.Message != "") {
                        validationError.Message = validationError.Message;
                    }
                    else if (Message != undefined && Message != "") {
                        validationError.Message = Message;
                    }
                    else {
                        validationError.Message = validationObject.ValidationMessage || Validation.ruleErrorMsg;
                    }
                    validationError.hasValidationError = true;
                    validationError.IsError = true;
                }
                else {
                    validationError.IsError = false;
                    validationError.hasValidationError = false;
                }

                return validationError;
            }
            else if (value == Validation.selectOne) {
                var sectionNamePath = path.substring(0, path.lastIndexOf('.'));
                if (sectionNamePath.indexOf('.') > 0) {
                    var sectionName = path.substring(0, sectionNamePath.indexOf('.'));
                }
                else {
                    var sectionName = sectionNamePath;
                }
                var sectionDetails = formInstancebuilder.designData.Sections.filter(function (ct) {
                    return ct.FullName == sectionName;
                });

                var elementDetails = getElementDetails(sectionDetails[0], path);
                if (elementDetails) {
                    var validationError = {
                        FullName: path,
                        IsRequiredError: false,
                        IsError: false,
                        //Message: Validation.incorrectValueMsg,
                        Message: '',
                        UIElementName: elementDetails.Name,
                        RowNumber: getRowNumber(rowNum),
                        GeneratedName: elementDetails.GeneratedName,
                        //hasValidationError: true,
                        hasValidationError: false,
                        value: value
                    };

                    return validationError;
                }
            }
                // Start - EQN-480 Code for IsDataRequired property
                // Executes when there is no row in the repeater and gives validation message if IsDataRequired property is set true in document design
            else if (value !== null && value.length == 0 && value.length != undefined) {
                var sectionNamePath = path.substring(0, path.lastIndexOf('.'));
                if (sectionNamePath.indexOf('.') > 0) {
                    var sectionName = path.substring(0, sectionNamePath.indexOf('.'));
                }
                else {
                    var sectionName = sectionNamePath;
                }
                var sectionDetails = formInstancebuilder.designData.Sections.filter(function (ct) {
                    return ct.FullName == sectionName;
                });

                var elementDetails = getElementDetails(sectionDetails[0], path);
                if (elementDetails) {
                    var validationError = {
                        FullName: path,
                        IsRequiredError: false,
                        IsError: false,
                        Message: '',
                        UIElementName: elementDetails.Name,
                        RowIdProperty: rowid === undefined ? "" : parseInt(rowid),
                        RowNumber: getRowNumber(rowNum),
                        ColumnNumber: columnNum,
                        GeneratedName: elementDetails.GeneratedName,
                        hasValidationError: false,
                        value: value
                    };
                }
                if (elementDetails.Repeater != null) {
                    if (elementDetails.Repeater.IsDataRequired === true) {
                        validationError.Message = Validation.dataRequiredMsg;
                        validationError.hasValidationError = true;
                        validationError.IsRequiredError = false;
                    }
                    else {
                        validationError.hasValidationError = false;
                    }
                }
                return validationError;
            }
                //End
            else {
                var sectionNamePath = path.substring(0, path.lastIndexOf('.'));
                if (sectionNamePath.indexOf('.') > 0) {
                    var sectionName = path.substring(0, sectionNamePath.indexOf('.'));
                }
                else {
                    var sectionName = sectionNamePath;
                }
                var sectionDetails = formInstancebuilder.designData.Sections.filter(function (ct) {
                    return ct.FullName == sectionName;
                });

                var elementDetails = getElementDetails(sectionDetails[0], path);
                if (elementDetails) {
                    var validationError = {
                        FullName: path,
                        IsRequiredError: false,
                        IsError: false,
                        Message: '',
                        UIElementName: elementDetails.Name,
                        RowIdProperty: rowid === undefined ? "" : parseInt(rowid),
                        RowNumber: getRowNumber(rowNum),
                        ColumnNumber: columnNum,
                        GeneratedName: elementDetails.GeneratedName,
                        hasValidationError: false,
                        value: value
                    };

                    return validationError;
                }
            }
        } catch (e) {
            console.log(e + " function : handleValidation()");
        }
    }

    function handleValidation(path, value, rowNum, columnNum, rowid, Message) {
        try {
            //get the validation object

            var validationObjectList = formInstancebuilder.designData.Validations.filter(function (ct) {
                return ct.FullName == path;
            });
            //changes to remove data source name from full name 
            var datasourceList = formInstancebuilder.designData.DataSources;
            if ((datasourceList != null || datasourceList != undefined) && datasourceList.length != '0') {
                var invalidFullName = undefined;
                var fullPath = undefined;
                var dataSourceName = undefined;

                for (i = 0 ; i < datasourceList.length; i++) {

                    for (index = 0 ; index < validationObjectList.length ; index++) {

                        if (validationObjectList[index].FullName.indexOf(datasourceList[i].DataSourceName) > -1) {
                            invalidFullName = validationObjectList[index].FullName;
                            var isDataSourceNameExistsInFullName = false;
                            var fullNameList = validationObjectList[index].FullName.split('.');
                            for (count = 0; count < fullNameList.length; count++) {
                                if (fullNameList[count].indexOf(datasourceList[i].DataSourceName) > -1) {
                                    //isDataSourceNameExistsInFullName = true; -- commented line as it removes the data source name from FullName in below code 
                                    dataSourceName = datasourceList[i].DataSourceName;

                                }
                            }
                        }
                    }
                }
                // if data source name exists in FullName then remove the data source name from FullName.
                if (isDataSourceNameExistsInFullName == true) {
                    if (invalidFullName != undefined) {
                        var fullPath = invalidFullName.replace(dataSourceName + ".", "");
                    }
                }

            }
            var validationObject = validationObjectList[0];

            if (validationObject) {
                var validationError = {
                    FullName: fullPath == undefined ? path : fullPath,
                    IsRequiredError: false,
                    IsError: false,
                    Message: '',
                    UIElementName: validationObject.UIElementName,
                    RowIdProperty: rowid === undefined ? "" : parseInt(rowid),
                    RowNumber: getRowNumber(rowNum),
                    ColumnNumber: columnNum,
                    GeneratedName: validationObject.FullName.substring(validationObject.FullName.lastIndexOf('.') + 1),
                    hasValidationError: false,
                    value: value
                };
                if (validationObject.PlaceHolder != null || validationObject.PlaceHolder != undefined) {
                    var qualifiedElementNameParts = validationObject.FullName.split(".");
                    var dataPortion;
                    for (var qualifiedElementNamePartsCount = 0; qualifiedElementNamePartsCount < qualifiedElementNameParts.length; qualifiedElementNamePartsCount++) {
                        if (qualifiedElementNamePartsCount == 0) {
                            dataPortion = formInstancebuilder.formData[qualifiedElementNameParts[qualifiedElementNamePartsCount]];
                        }
                        else {
                            dataPortion = dataPortion[qualifiedElementNameParts[qualifiedElementNamePartsCount]];
                            var regex = /^[_.,/\()-]+$/;
                            var stringUtility = new globalutilities();
                            if (dataPortion != null || dataPortion != undefined) {
                                if ((dataPortion == validationObject.PlaceHolder || regex.test(dataPortion)) && dataPortion.length > 0) {
                                    value = stringUtility.stringMethods.setEmptyString(value);
                                }
                            }
                        }

                    }
                }
                if (validationObject.IsRequired && validationObject.IsActive) {
                    //check if required 
                    if (validationObject.UIElementName.indexOf('CheckBox') > -1) {
                        if (value) {
                            //does not have validation error
                            validationError.hasValidationError = false;
                            validationError.IsRequiredError = false;
                        }
                        else {
                            //has validation error
                            //add object details to validationErrorObject
                            validationError.Message = validationObject.ValidationMessage || Validation.requiredMsg;
                            validationError.hasValidationError = true;
                            validationError.IsRequiredError = true;
                        }
                    }
                    else {
                        var stringUtility = new globalutilities();
                        if (stringUtility.stringMethods.isNullOrEmpty(value)) {
                            //has validation error
                            //add object details to validationErrorObject
                            if (validationError.IsError != 'true') {
                                validationError.Message = validationObject.ValidationMessage || Validation.requiredMsg;
                                validationError.hasValidationError = true;
                                validationError.IsRequiredError = true;
                            }
                        }
                        else {
                            //does not have validation error
                            var sectionNamePath = path.substring(0, path.lastIndexOf('.'));
                            if (sectionNamePath.indexOf('.') > 0) {
                                var sectionName = path.substring(0, sectionNamePath.indexOf('.'));
                            }
                            else {
                                var sectionName = sectionNamePath;
                            }
                            var sectionDetails = formInstancebuilder.designData.Sections.filter(function (ct) {
                                return ct.FullName == sectionName;
                            });

                            var elementDetails = getElementDetails(sectionDetails[0], path);
                            if (elementDetails.DataType == "date") {
                                var regexDate = new RegExp(/^(((0?[1-9]|1[012])\/(0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])\/(29|30)|(0?[13578]|1[02])\/31)\/(19|[2-9]\d)\d{2}|0?2\/29\/((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00)))$/);
                                if (!regexDate.test(value) && value != "" && value != null) {
                                    //has validation error
                                    //add object details to validationErrorObject
                                    validationError.Message = "" || Validation.invalidDateMsg;
                                    validationError.hasValidationError = true;
                                    validationError.IsRequiredError = false;
                                }
                                else {
                                    //does not have validation error
                                    validationError.hasValidationError = false;
                                }
                            }
                            else {
                                //does not have validation error
                                validationError.hasValidationError = false;
                                validationError.IsRequiredError = false;
                            }
                        }
                    }
                }
                if (!validationError.hasValidationError) {
                    if (validationObject.Regex != '' && validationObject.Regex != null && validationObject.Regex != undefined && validationObject.IsActive == true && value != '') {
                        var regex = new RegExp(validationObject.Regex);
                        // If datatype=float make value to two decimal fixed 
                        if (validationObject.DataType == "float" && !isNaN(value)) {
                            value = parseFloat(value).toFixed(2);
                            validationError.value = value;
                            formInstancebuilder.rules.setCustomJSRenderProperty(validationObject.FullName, value);
                        }
                        if (!regex.test(value)) {
                            //has validation error
                            //add object details to validationErrorObject
                            validationError.Message = validationObject.ValidationMessage || Validation.regexMsg;
                            validationError.hasValidationError = true;
                            validationError.IsRequiredError = false;
                        }
                        else {
                            if (validationObject.UIElementName.indexOf('DropDown') > -1) {
                                validationObject.MaxLength = validationObject.MaxLength == 0 ? 200 : validationObject.MaxLength;
                            }
                                //if (value.length > validationObject.MaxLength) {
                                //    validationError.Message = validationObject.ValidationMessage || Validation.exceedMaxLen;
                                //    validationError.hasValidationError = true;
                                //    validationError.IsRequiredError = false;
                                //}
                            else {
                                //does not have validation error
                                validationError.hasValidationError = false;
                            }
                        }
                    }
                        //check for the datatype validations 
                    else if (validationObject.DataType != '' && validationObject.DataType != null && validationObject.DataType != undefined && validationObject.IsActive == true) {
                        //check for int value validations
                        if (validationObject.DataType == "int") {
                            if (value != '') {
                                var regx = new RegExp("^[0-9]*$");
                                if (regx.test(value)) {
                                    if (value.length > validationObject.MaxLength) {
                                        validationError.Message = validationObject.ValidationMessage || Validation.exceedMaxLen;
                                        validationError.hasValidationError = true;
                                        validationError.IsRequiredError = false;
                                    }
                                    else {
                                        //does not have validation error
                                        validationError.hasValidationError = false;
                                    }
                                }
                                else {
                                    //has validation error
                                    //add object details to validationErrorObject
                                    validationError.Message = validationObject.ValidationMessage || Validation.invalidIntMsg;
                                    validationError.hasValidationError = true;
                                    validationError.IsRequiredError = false;
                                }
                            }
                        }
                            // check for float value validations
                        else if (validationObject.DataType == "float") {
                            var regexFloat = new RegExp(/^[+-]?\d+(\.\d+)?$/);
                            if (!regexFloat.test(value)) {
                                //has validation error
                                //add object details to validationErrorObject
                                validationError.Message = validationObject.ValidationMessage || Validation.invalidFloatMsg;
                                validationError.hasValidationError = true;
                                validationError.IsRequiredError = false;
                            }
                            else {
                                if (value.length > validationObject.MaxLength) {
                                    validationError.Message = validationObject.ValidationMessage || Validation.exceedMaxLen;
                                    validationError.hasValidationError = true;
                                    validationError.IsRequiredError = false;
                                }
                                else {
                                    //does not have validation error
                                    validationError.hasValidationError = false;
                                }
                            }
                        } else if (validationObject.DataType == "string") {
                            if (value.length > validationObject.MaxLength) {
                                validationError.Message = validationObject.ValidationMessage || Validation.exceedMaxLen;
                                validationError.hasValidationError = true;
                                validationError.IsRequiredError = false;
                            }
                        }
                    }
                }

                if (validationObject.IsError || validationError.hasValidationError) {
                    if (validationError.Message != "") {
                        validationError.Message = validationError.Message;
                    }
                    else if (Message != undefined && Message != "") {
                        validationError.Message = Message;
                    }
                    else {
                        validationError.Message = validationObject.ValidationMessage || Validation.ruleErrorMsg;
                    }
                    validationError.hasValidationError = true;
                    validationError.IsError = true;
                }
                else {
                    validationError.IsError = false;
                    validationError.hasValidationError = false;
                }

                return validationError;
            }
            else if (value == Validation.selectOne) {
                var sectionNamePath = path.substring(0, path.lastIndexOf('.'));
                if (sectionNamePath.indexOf('.') > 0) {
                    var sectionName = path.substring(0, sectionNamePath.indexOf('.'));
                }
                else {
                    var sectionName = sectionNamePath;
                }
                var sectionDetails = formInstancebuilder.designData.Sections.filter(function (ct) {
                    return ct.FullName == sectionName;
                });

                var elementDetails = getElementDetails(sectionDetails[0], path);
                if (elementDetails) {
                    var validationError = {
                        FullName: path,
                        IsRequiredError: false,
                        IsError: false,
                        //Message: Validation.incorrectValueMsg,
                        Message: '',
                        UIElementName: elementDetails.Name,
                        RowNumber: getRowNumber(rowNum),
                        GeneratedName: elementDetails.GeneratedName,
                        //hasValidationError: true,
                        hasValidationError: false,
                        value: value
                    };

                    return validationError;
                }
            }
                // Start - EQN-480 Code for IsDataRequired property
                // Executes when there is no row in the repeater and gives validation message if IsDataRequired property is set true in document design
            else if (value !== null && value.length == 0 && value.length != undefined) {
                var sectionNamePath = path.substring(0, path.lastIndexOf('.'));
                if (sectionNamePath.indexOf('.') > 0) {
                    var sectionName = path.substring(0, sectionNamePath.indexOf('.'));
                }
                else {
                    var sectionName = sectionNamePath;
                }
                var sectionDetails = formInstancebuilder.designData.Sections.filter(function (ct) {
                    return ct.FullName == sectionName;
                });

                var elementDetails = getElementDetails(sectionDetails[0], path);
                if (elementDetails) {
                    var validationError = {
                        FullName: path,
                        IsRequiredError: false,
                        IsError: false,
                        Message: '',
                        UIElementName: elementDetails.Name,
                        RowIdProperty: rowid === undefined ? "" : parseInt(rowid),
                        RowNumber: getRowNumber(rowNum),
                        ColumnNumber: columnNum,
                        GeneratedName: elementDetails.GeneratedName,
                        hasValidationError: false,
                        value: value
                    };
                }
                if (elementDetails.Repeater != null) {
                    if (elementDetails.Repeater.IsDataRequired === true) {
                        validationError.Message = Validation.dataRequiredMsg;
                        validationError.hasValidationError = true;
                        validationError.IsRequiredError = false;
                    }
                    else {
                        validationError.hasValidationError = false;
                    }
                }
                return validationError;
            }
                //End
            else {
                var sectionNamePath = path.substring(0, path.lastIndexOf('.'));
                if (sectionNamePath.indexOf('.') > 0) {
                    var sectionName = path.substring(0, sectionNamePath.indexOf('.'));
                }
                else {
                    var sectionName = sectionNamePath;
                }
                var sectionDetails = formInstancebuilder.designData.Sections.filter(function (ct) {
                    return ct.FullName == sectionName;
                });

                var elementDetails = getElementDetails(sectionDetails[0], path);
                if (elementDetails) {
                    var validationError = {
                        FullName: path,
                        IsRequiredError: false,
                        IsError: false,
                        Message: '',
                        UIElementName: elementDetails.Name,
                        RowIdProperty: rowid === undefined ? "" : parseInt(rowid),
                        RowNumber: getRowNumber(rowNum),
                        ColumnNumber: columnNum,
                        GeneratedName: elementDetails.GeneratedName,
                        hasValidationError: false,
                        value: value
                    };

                    return validationError;
                }
            }
        } catch (e) {
            console.log(e + " function : handleValidation()");
        }
    }

    function handleDateValidation(path, value, rowNum, columnNum, rowid) {
        var sectionNamePath = path.substring(0, path.lastIndexOf('.'));
        if (sectionNamePath.indexOf('.') > 0) {
            var sectionName = path.substring(0, sectionNamePath.indexOf('.'));
        }
        else {
            var sectionName = sectionNamePath;
        }
        var sectionDetails = formInstancebuilder.designData.Sections.filter(function (ct) {
            return ct.FullName == sectionName;
        });

        var elementDetails = getElementDetails(sectionDetails[0], path);
        if (elementDetails) {
            var validationError = {
                FullName: path,
                IsRequiredError: false,
                IsError: false,
                Message: '',
                UIElementName: elementDetails.Name,
                RowIdProperty: rowid === undefined ? "" : parseInt(rowid),
                RowNumber: getRowNumber(rowNum),
                ColumnNumber: columnNum,
                GeneratedName: elementDetails.GeneratedName,
                hasValidationError: false,
                value: value
            };
            if (elementDetails.DataType === 'date' && value != "") {
                var rxDatePattern = new RegExp(/^(((0?[1-9]|1[012])\/(0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])\/(29|30)|(0?[13578]|1[02])\/31)\/(19|[2-9]\d)\d{2}|0?2\/29\/((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00)))$/);
                //&& value != "" && value != null
                if (!rxDatePattern.test(value)) {
                    //has validation error
                    //add object details to validationErrorObject
                    validationError.Message = "" || Validation.invalidDateMsg;
                    validationError.hasValidationError = true;
                    validationError.IsRequiredError = false;
                }
                else {
                    //does not have validation error
                    validationError.hasValidationError = false;
                }
            }
            return validationError;
        }
    }

    //code to be executed on change event of all input,select
    function handleDuplication(path, value, rowNum, columnNum, isDelete) {
        try {
            var duplicationObjectList = formInstancebuilder.designData.Duplications.filter(function (ct) {
                return ct.ParentUIElementName == path;
            });
            if (duplicationObjectList != undefined) {
                var isBlankRowHasDuplication = false;
                var hashcolumnArray = getRepeaterHashColumnData(value, duplicationObjectList);

                var rowId = parseInt(rowNum);
                var nonDuplicationObjectArray = new Array();
                var duplicationObjectArray = new Array();

                var duplicateRowData = new Array();
                duplicationList.filter(function (ct, idx) {
                    if (ct.Path == path) {
                        duplicateRowData = ct.DuplicateRows;
                    }
                });

                if (isDelete == undefined || !isDelete) {
                    var rowData = undefined;
                    var gridData = new Array();
                    hashcolumnArray.filter(function (data, ct) {
                        var rowIDProperty = parseInt(data.ID);
                        if (rowIDProperty != rowId) {
                            gridData.push(data);
                        }
                        else if (rowIDProperty == rowId) {
                            rowData = data.Data;
                        }
                    });

                    var splitRowData = rowData.split('#');
                    var isBlank = splitRowData.filter(function (dt) {
                        if (dt != "blank" && dt != "[selectone]" && dt != "") {
                            return dt;
                        }
                    });

                    if (isBlank.length == 0) {
                        var sections = formInstancebuilder.errorGridData.filter(function (ct) {
                            return ct.Section == path.split('.')[0];
                        });

                        var row = undefined;
                        if (sections.length > 0) {
                            row = sections[0].ErrorRows.filter(function (ct) {
                                return ct.SubSectionName.replace(/\ => /g, '.').replace(/\s/g, '') == path && ct.RowIdProperty == rowId && ct.Value == "CheckDuplicate";
                            });
                        }

                        if (row != undefined && row.length == 0) {
                            return false;
                        }
                        else {
                            isBlankRowHasDuplication = true;
                        }
                    }

                    var found = 0;
                    gridData.filter(function (data, ct) {
                        if (data.Data === rowData && isBlankRowHasDuplication == false) {
                            found++;
                            var rowIDProperty = parseInt(data.ID);
                            if (duplicationObjectArray.length > 0) {
                                var t = checkElementExists(duplicationObjectArray, rowIDProperty);
                                if (t == 0) {
                                    duplicationObjectArray.push(rowIDProperty);
                                }
                            }
                            else {
                                duplicationObjectArray.push(rowIDProperty);
                            }
                        }
                    });

                    if (found > 0) {
                        duplicationObjectArray.push(rowId);
                    }
                    else {
                        nonDuplicationObjectArray.push(rowId);
                    }
                }

                var changedDuplicateRowData = new Array();
                duplicateRowData.filter(function (ct) {
                    if (ct != rowId) {
                        changedDuplicateRowData.push(ct);
                    }
                });

                var gridRowData = new Array();
                if (changedDuplicateRowData.length > 0) {
                    $.each(value, function (i, el) {
                        changedDuplicateRowData.filter(function (ct, dx) {
                            if (el.RowIDProperty != rowId && el.RowIDProperty == ct) {
                                gridRowData.push(el);
                            }
                        });
                    });
                }

                for (var k = 0 ; k < gridRowData.length; k++) {
                    var rowData = undefined;
                    var gridData = new Array();
                    var hashcolumnArray = getRepeaterHashColumnData(gridRowData, duplicationObjectList);
                    hashcolumnArray.filter(function (data, ct) {
                        if (ct != k) {
                            gridData.push(data.Data);
                        }
                        else if (ct == k) {
                            rowData = data.Data;
                        }
                    });


                    var splitRowData = rowData.split('#');
                    var isBlank = splitRowData.filter(function (dt) {
                        if (dt != "blank" && dt != "[selectone]" && dt != "") {
                            return dt;
                        }
                    });

                    if (isBlank.length == 0) {
                        var sections = formInstancebuilder.errorGridData.filter(function (ct) {
                            return ct.Section == path.split('.')[0];
                        });

                        var row = undefined;
                        if (sections.length > 0) {
                            row = sections[0].ErrorRows.filter(function (ct) {
                                return ct.SubSectionName.replace(/\ => /g, '.').replace(/\s/g, '') == path && ct.RowIdProperty == rowId && ct.Value == "CheckDuplicate";
                            });
                        }

                        if (row != undefined && row.length == 0) {
                            return false;
                        }
                    }

                    var found = 0;
                    for (var i = 0; i < gridData.length; i++) {
                        found = 0;
                        if (gridData[i] === rowData) {
                            found++;
                        }

                        if (found > 0) {
                            var rowIDProperty = parseInt(gridRowData[k].RowIDProperty);
                            if (duplicationObjectArray.length > 0) {
                                var t = checkElementExists(duplicationObjectArray, rowIDProperty);
                                if (t == 0) {
                                    duplicationObjectArray.push(rowIDProperty);
                                }
                            }
                            else {
                                duplicationObjectArray.push(rowIDProperty);
                            }
                        }
                    }
                }

                for (var k = 0; k < gridRowData.length; k++) {
                    var rowIDProperty = parseInt(gridRowData[k].RowIDProperty);
                    if (duplicationObjectArray.length > 0) {
                        var t = checkElementExists(duplicationObjectArray, rowIDProperty);
                        if (t == 0) {
                            nonDuplicationObjectArray.push(rowIDProperty);
                        }
                    }
                    else {
                        nonDuplicationObjectArray.push(rowIDProperty);
                    }
                }

                if (duplicationList.length > 0) {
                    duplicationList.filter(function (ct, idx) {
                        if (ct.Path == path) {
                            ct.DuplicateRows = new Array();
                            ct.DuplicateRows = duplicationObjectArray;
                        }
                    });
                }
                else {
                    var duplicateObjectRow = {
                        ID: duplicationList.length + 1,
                        Path: path,
                        DuplicateRows: duplicationObjectArray,
                    };
                    duplicationList.push(duplicateObjectRow);
                }

                var duplicationErrorArray = new Array();
                for (var i = 0; i < nonDuplicationObjectArray.length; i++) {
                    for (var j = 0; j < duplicationObjectList.length; j++) {
                        var duplicationObject = duplicationObjectList[j];
                        var duplicationError = {
                            FullName: duplicationObject.FullName,
                            IsRequiredError: false,
                            Message: '',
                            UIElementName: duplicationObject.UIElementName,
                            RowIdProperty: parseInt(nonDuplicationObjectArray[i]),
                            RowNumber: getRowNumber((parseInt(nonDuplicationObjectArray[i]) + 1)),
                            ColumnNumber: columnNum,
                            GeneratedName: duplicationObject.FullName.substring(duplicationObject.FullName.lastIndexOf('.') + 1),
                            hasValidationError: false,
                            value: 'CheckDuplicate',
                            CheckDuplicate: true
                        };
                        duplicationErrorArray.push(duplicationError);
                    }
                }

                for (var i = 0; i < duplicationObjectArray.length; i++) {
                    for (var j = 0; j < duplicationObjectList.length; j++) {

                        var duplicationObject = duplicationObjectList[j];
                        var duplicationError = {
                            FullName: duplicationObject.FullName,
                            IsRequiredError: false,
                            Message: Validation.duplicateMsg,
                            UIElementName: duplicationObject.UIElementName,
                            RowIdProperty: parseInt(duplicationObjectArray[i]),
                            RowNumber: getRowNumber((parseInt(duplicationObjectArray[i]) + 1)),
                            ColumnNumber: columnNum,
                            GeneratedName: duplicationObject.FullName.substring(duplicationObject.FullName.lastIndexOf('.') + 1),
                            hasValidationError: true,
                            value: 'CheckDuplicate',
                            CheckDuplicate: true
                        };
                        duplicationErrorArray.push(duplicationError);
                    }
                }
                return duplicationErrorArray;
            }
        } catch (e) {
            console.log(e + " function : handleDuplication()");
        }
    }

    //code to handle duplicate values in grid on Validate Document
    function handleDuplications(path, value, rowNum, columnNum) {
        try {
            var duplicationObjectList = formInstancebuilder.designData.Duplications.filter(function (ct) {
                return ct.ParentUIElementName == path;
            });
            if (duplicationObjectList.length > 0) {
                //var hashcolumnArray = getRepeaterHashColumnData(value, duplicationObjectList);

                var duplicationObjectArray = new Array();
                var gridRowData = new Array();
                $.each(value, function (i, el) {
                    gridRowData.push(el);
                });

                for (var k = 0; k < gridRowData.length; k++) {
                    var rowData = undefined;
                    var gridData = new Array();
                    var hashcolumnArray = getRepeaterHashColumnData(gridRowData, duplicationObjectList);
                    hashcolumnArray.filter(function (data, ct) {
                        if (ct != k) {
                            gridData.push(data.Data);
                        }
                        else if (ct == k) {
                            rowData = data.Data;
                        }
                    });

                    var splitRowData = rowData.split('#');
                    var isBlank = splitRowData.filter(function (dt) {
                        if (dt != "blank" && dt != "[selectone]" && dt != "") {
                            return dt;
                        }
                    });

                    if (isBlank.length == 0) {
                        continue;
                    }

                    var found = 0;
                    for (var i = 0; i < gridData.length; i++) {
                        found = 0;
                        if (gridData[i] === rowData) {
                            found++;
                        }

                        if (found > 0) {
                            if (gridRowData[k].RowIDProperty != undefined) {
                                var rowIDProperty = parseInt(gridRowData[k].RowIDProperty);
                                if (duplicationObjectArray.length > 0) {
                                    var t = checkElementExists(duplicationObjectArray, rowIDProperty);
                                    if (t == 0) {
                                        duplicationObjectArray.push(rowIDProperty);
                                    }
                                }
                                else {
                                    duplicationObjectArray.push(rowIDProperty);
                                }
                            }
                            else {
                                if (duplicationObjectArray.length > 0) {
                                    var t = checkElementExists(duplicationObjectArray, k);
                                    if (t == 0) {
                                        duplicationObjectArray.push(k);
                                    }
                                }
                                else {
                                    duplicationObjectArray.push(k);
                                }
                            }
                        }
                    }
                }

                var duplicateObjectRow = {
                    ID: duplicationList.length + 1,
                    Path: path,
                    DuplicateRows: duplicationObjectArray,
                };
                duplicationList.push(duplicateObjectRow);

                var duplicationErrorArray = new Array();
                if (duplicationObjectArray.length > 0) {
                    for (var i = 0; i < duplicationObjectArray.length; i++) {
                        for (var j = 0; j < duplicationObjectList.length; j++) {
                            var duplicationObject = duplicationObjectList[j];
                            var duplicationError = {
                                FullName: duplicationObject.FullName,
                                IsRequiredError: false,
                                Message: Validation.duplicateMsg,
                                UIElementName: duplicationObject.UIElementName,
                                RowIdProperty: parseInt(duplicationObjectArray[i]),
                                RowNumber: parseInt(duplicationObjectArray[i]) + 1,
                                ColumnNumber: columnNum,
                                GeneratedName: duplicationObject.FullName.substring(duplicationObject.FullName.lastIndexOf('.') + 1),
                                hasValidationError: true,
                                value: 'CheckDuplicate',
                                CheckDuplicate: true
                            };
                            duplicationErrorArray.push(duplicationError);
                        }
                    }
                }
                return duplicationErrorArray;
            }
        } catch (e) {
            console.log(e + " function : handleDuplications()");
        }
    }

    function handleValidations(path, value, rowNum, columnNum, rowid) {
        try {
            var validationError = handleValidation(path, value, rowNum, columnNum, rowid);
            if (validationError != undefined) {
                validationErrorList.push(validationError);
            }
            if (rowNum == undefined) {
                var duplicationErrorArray = handleDuplications(path, value, rowNum, columnNum);

                if (duplicationErrorArray != undefined) {
                    if (duplicationErrorArray.length > 0) {
                        for (var i = 0; i < duplicationErrorArray.length; i++) {
                            validationErrorList.push(duplicationErrorArray[i]);
                        }
                    }
                }
            }
        } catch (e) {
            console.log(e + " function : handleValidations()");
        }

    }

    function showValidatedControls() {
        for (var i = 0; i < formInstancebuilder.errorGridData.length; i++) {
            var row = formInstancebuilder.errorGridData[i];
            for (var j = 0; j < row.ErrorRows.length; j++) {
                applyValidation(row.ErrorRows[j], formInstancebuilder.formInstanceId);
            }
        }
    }

    function applyValidation(row, formInstanceId) {
        if (row.RowNum == "") {
            $(formInstancebuilder.formInstanceDivJQ).find("#" + row.ElementID).parent().addClass("has-error");
            //addAsterisk(row, formInstanceId);
        }
    }

    function removeValidation(uielementName, formInstanceId) {
        $(formInstancebuilder.formInstanceDivJQ).find("#" + uielementName).parent().removeClass("has-error");
    }

    function removeRepeaterValidationErrors(row) {
        var colindex = undefined;
        var loadedPage = undefined;

        var repeaterID = row.ElementID.substring(0, row.ElementID.indexOf("_")) + row.FormInstanceID;
        var col = $("#" + repeaterID).jqGrid("getGridParam", "colModel");

        if (col != undefined) {
            $.each(col, function (idx, ct) {
                if (ct.name == row.GeneratedName) {
                    colindex = idx;
                    return false;
                }
            });

            var ind = $("#" + repeaterID).jqGrid('getGridRowById', row.RowIdProperty);
            if (ind) {
                //var index = $('#' + row.RowIdProperty)[0].rowIndex;
                var tcell = $("td:eq(" + colindex + ")", ind);
                $(tcell).removeClass("repeater-has-error");
            }
        }
    }

    function hasRequiredValidation(fullName) {
        var validationObjectList = formInstancebuilder.designData.Validations.filter(function (ct) {
            return ct.FullName == fullName;
        });
        var validationObject = validationObjectList[0];

        if (validationObject) {
            if (validationObject.IsRequired) {
                return true;
            }
        }
        return false;
    }

    function validateFormInstance() {
        validationErrorList = new Array();
        var obje = flattenObject(formInstancebuilder.formData);
        try {
            for (prop in obje) {
                if (prop.indexOf("jQuery") == -1 && prop.indexOf("RowIDProperty") == -1) {
                    if (Array.isArray(obje[prop])) {
                        // Start - Code change for EQN-480
                        if (obje[prop].length == 0) {
                            handleValidations(prop, obje[prop].length, '', '', rowIdProperty);
                        }
                            //End
                        else {
                            for (var i = 0; i < obje[prop].length; i++) {
                                for (var child in obje[prop][i]) {
                                    var rowIdProperty = obje[prop][i].RowIDProperty;
                                    if (rowIdProperty == undefined) {
                                        rowIdProperty = i;
                                    }
                                    if (child.indexOf("jQuery") == -1 && child.indexOf("RowIDProperty") == -1) {
                                        var path1 = prop + "." + child;
                                        var value2 = obje[prop][i][child];
                                        if (Array.isArray(obje[prop][i][child])) {
                                            for (var j = 0; j < obje[prop][i][child].length; j++) {
                                                for (var child1 in obje[prop][i][child][j]) {
                                                    if (child1.indexOf("jQuery") == -1 && child1.indexOf("RowIDProperty") == -1) {
                                                        var path2 = prop + "." + child1;
                                                        var value3 = obje[prop][i][child][j][child1];
                                                        handleValidations(path2, value3, i, j, rowIdProperty);
                                                    }
                                                }
                                            }
                                        }
                                        else {
                                            handleValidations(path1, value2, i, '', rowIdProperty);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    var path = prop;
                    var value = obje[prop];
                    handleValidations(path, value);
                }
            }
        } catch (e) {
            console.log(e + " function: validationFormInstance()");
        }
        return validationErrorList;
    }

    function isAnchor() {
        return formInstancebuilder.formInstanceId == formInstancebuilder.anchorFormInstanceID;
    }

    function isDisabled(uielement) {
        if ($(uielement).is(":radio") || $(uielement).is(":checkbox"))
            return $(uielement).is(":disabled");
        else
            return ($(uielement).is(":disabled") || $(uielement).is("[readOnly]"));
    }

    function isVisible(uielement) {
        return $(uielement).css('display') != "none" || $(uielement).parent().find('.ddt-textbox').css('display') != "none";
    }

    function removeVisibleAndDisabledControls() {
        if (formInstancebuilder.errorGridData.length > 0) {
            for (var i = 0; i < formInstancebuilder.errorGridData.length; i++) {
                var errorRow = formInstancebuilder.errorGridData[i];
                if (errorRow.Section === formInstancebuilder.selectedSectionName || formInstancebuilder instanceof sotFormInstanceBuilder == true) {
                    //if (formInstancebuilder.sections[errorRow.SectionID].IsLoaded == false) {
                    //    formInstancebuilder.form.renderSection(errorRow.SectionID, false);
                    //}
                    for (var j = 0; j < errorRow.ErrorRows.length; j++) {
                        var row = errorRow.ErrorRows[j];
                        var uielement = $("#" + row.ElementID);
                        if (!isVisible(uielement) || (isDisabled(uielement) && isAnchor())) {
                            removeValidation(row.ElementID, formInstancebuilder.formInstanceId);
                            formInstancebuilder.errorGridData[i].ErrorRows.splice(j, 1);
                            j--;
                            continue;
                        }
                        else {
                            var element = "";
                            var gridCell = "";
                            if (row.RowNum == "") {
                                element = $("#" + row.ElementID);
                            }
                            else {
                                var repeaterID = row.ElementID.substring(0, row.ElementID.indexOf("_")) + row.FormInstanceID;
                                var repeaterelement = $("#repeater" + repeaterID);
                                if (!isVisible(repeaterelement) || ($(repeaterelement).hasClass('disabled') && isAnchor())) {
                                    removeRepeaterValidationErrors(row);
                                    formInstancebuilder.errorGridData[i].ErrorRows.splice(j, 1);
                                    j--;
                                    continue;
                                }
                                else {
                                    //$("#" + repeaterID).jqGrid('setSelection', (row.RowNum));
                                    //$("#" + repeaterID).jqGrid('setSelection', (row.RowNum - 1));
                                    var td;

                                    if (checkIfPQGridLoaded(true))
                                        td = getRepeaterColumnPQ(repeaterID, row.RowNum, row.GeneratedName, row.ColumnNumber);
                                    else
                                        td = getRepeaterColumn(repeaterID, row.RowNum, row.GeneratedName, row.ColumnNumber);

                                    if (td.length > 0) {
                                        var repeaterelement = $(td);
                                        var repeateruielement = $(td).children().first();
                                        if (!isVisible(repeaterelement) || !isVisible(repeateruielement) || (isDisabled(repeaterelement) && isAnchor()) || (isDisabled(repeateruielement) && isAnchor())) {
                                            removeRepeaterValidationErrors(row);
                                            formInstancebuilder.errorGridData[i].ErrorRows.splice(j, 1);
                                            j--;
                                            continue;
                                        }
                                    }
                                    else {

                                        //errorRow.ErrorRows.splice(j, 1);
                                        //j--;
                                        //continue;
                                    }
                                }
                            }
                        }
                    }
                    if (errorRow.ErrorRows.length == 0) {
                        formInstancebuilder.errorGridData.splice(i, 1);
                        i--;
                        continue;
                    }
                }
            }
        }
    }

    function getRowNumber(rowNumber) {
        var rowNum;
        var keyValue;
        if (rowNumber != undefined && rowNumber != null && rowNumber != "") {
            if (rowNumber.toString().indexOf("|") >= 0) {
                rowNum = parseInt(rowNumber) + 1;
                keyValue = rowNumber.substring(rowNumber.indexOf("|") + 1, rowNumber.length + 1);
                return rowNum + "|" + keyValue
            }
            else
                return parseInt(rowNumber) + 1
        }
        else
            return "";
    }

    function addAsterisk(row, formInstanceId) {
        if (formInstanceId != undefined) {
            if (row.Description.indexOf("is required.") > -1) {
                var elementID = row.ElementID.replace(formInstanceId, "");
                var text = $("label[for=" + elementID + "]").text().replace("*", "");
                text = text + " <em>*</em>";
                $("label[for=" + elementID + "]").html("");
                $("label[for=" + elementID + "]").html(text);
                text = "";
            }
        }
    }

    function removeAsterisk(row, formInstanceId) {
        if (formInstanceId != undefined) {
            if (row.Description.indexOf("is required.") > -1) {
                var elementID = row.ElementID.replace(formInstanceId, "");
                var text = $("label[for=" + elementID + "]").text().replace(" <em>*</em>", "");
                $("label[for=" + elementID + "]").html(text);
            }
        }
    }

    return {
        handleValidation: function (path, value, rowNum, columnNum, rowid, Message) {
            return handleValidation(path, value, rowNum, columnNum, rowid, Message);
        },
        handleErrorValidation: function (path, value, rowNum, columnNum, rowid, Message, fullName) {
            return handleErrorValidation(path, value, rowNum, columnNum, rowid, Message, fullName);
        },
        handleDateValidation: function (path, value, rowNum, columnNum, rowid) {
            return handleDateValidation(path, value, rowNum, columnNum, rowid);
        },
        handleDuplication: function (path, value, rowNum, columnNum, isDelete) {
            return handleDuplication(path, value, rowNum, columnNum, isDelete);
        },
        handleDuplications: function (path, value, rowNum, columnNum) {
            return handleDuplications(path, value, rowNum, columnNum);
        },
        validateFormInstance: function () {
            return validateFormInstance();
        },
        showValidatedControls: function () {
            showValidatedControls();
        },
        hasRequiredValidation: function (fullName) {
            return hasRequiredValidation(fullName);
        },
        applyValidation: function (row) {
            applyValidation(row);
        },
        removeValidation: function (uielementName) {
            removeValidation(uielementName);
        },
        removeVisibleAndDisabledControls: function () {
            removeVisibleAndDisabledControls();
        }
    }
}
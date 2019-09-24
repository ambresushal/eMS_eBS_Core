function serviceDefinitionPropertyGrid(serviceDefinitionId, serviceDefinition, serviceDesignId, serviceDesignName, serviceDesignVersionInstance) {
    this.serviceDefinitionId = serviceDefinitionId;
    this.serviceDefinition = serviceDefinition;
    this.serviceDesignId = serviceDesignId;
    this.serviceDesignName = serviceDesignName;
    this.serviceDesignVersionInstance = serviceDesignVersionInstance;
    this.serviceDesignVersionId = this.serviceDesignVersionInstance.serviceDesignVersion.ServiceDesignVersionId;
    this.isFinalized = this.serviceDesignVersionInstance.serviceDesignVersion.IsFinalized;

    this.Urls = {
        getServceDefinitionDetails: '/ServiceDefinition/GetDetails?tenantId=1&serviceDefinitionId=' + this.serviceDefinitionId,
        saveServiceDefinition: '/ServiceDefinition/Update'
    }

    this.elementIDs = {
        propertyGridElement: 'fdvuielemdetail{serviceDesignVersionId}',
        propertyGridElementContainer: 'fdvuielemdetail{serviceDesignVersionId}container'
    }

    this.gridElementId = '#' + this.elementIDs.propertyGridElement.replace(/\{serviceDesignVersionId\}/g, this.serviceDesignVersionId);
    this.gridContainerElementId = '#' + this.elementIDs.propertyGridElementContainer.replace(/\{serviceDesignVersionId\}/g, this.serviceDesignVersionId);
    //without #
    this.gridElementIdNoHash = this.elementIDs.propertyGridElement.replace(/\{serviceDesignVersionId\}/g, this.serviceDesignVersionId);

}

serviceDefinitionPropertyGrid.prototype.loadPropertyGrid = function () {
    var currentInstance = this;
    var url = currentInstance.Urls.getServceDefinitionDetails
    var promise = ajaxWrapper.getJSON(url);
    //callback for ajax request success
    promise.done(function (xhr) {
        currentInstance.uiElementDetail = xhr;
        //generate grid data
        var uiElementProperties = currentInstance.generateGridData(xhr);
        currentInstance.bindToPropertyGrid(uiElementProperties);
        //if (currentInstance.serviceDefinition.UIElementType != 'Tab') {
        currentInstance.showGrid();
        //}
        //else {
        //    currentInstance.hideGrid();
        //}
    });
    //register callback for ajax request failure
    promise.fail(this.showError);

}

serviceDefinitionPropertyGrid.prototype.generateGridData = function (xhr) {
    //get properties that need to be displayed for the element based on type
    var serviceDefinitionProperties = this.getServiceDefinitionProperties(this.serviceDefinition.UIElementType);
    //populate the Value for each property of the element with the data received from the ajax call
    for (var index = 0; index < serviceDefinitionProperties.length; index++) {
        switch (serviceDefinitionProperties[index].IntProperty) {
            case 'UIElementDataTypeID':
                serviceDefinitionProperties[index].Value = xhr.UIElementDataTypeID;
                break;
            case 'IsRequired':
                serviceDefinitionProperties[index].Value = xhr.IsRequired;
                break;
            case 'IsMultiSelect':
                serviceDefinitionProperties[index].Value = xhr.IsMultiSelect;
                break;
            case 'Label':
                serviceDefinitionProperties[index].Value = xhr.Label;
                break;
            case 'DisplayName':
                serviceDefinitionProperties[index].Value = xhr.DisplayName;
                break;
            case 'Sequence':
                serviceDefinitionProperties[index].Value = xhr.Sequence;
                break;
            case 'UIElementTypeID':
                serviceDefinitionProperties[index].Value = xhr.UIElementTypeID;
                break;
            case 'UIElementType':
                serviceDefinitionProperties[index].Value = xhr.UIElementType;
                break;
            case 'IsKey':
                serviceDefinitionProperties[index].Value = xhr.IsKey;
                break;
        }
    }
    return serviceDefinitionProperties;
}
//get the data template required for the element type
//the structure of the objects returned is suitable to be bound to the property grid
serviceDefinitionPropertyGrid.prototype.getServiceDefinitionProperties = function (elementType) {
    var currentInstance = this;
    //IntProperty - maps to a property with the same name in the object recieved from the ajax call
    //Property - property name to be displayed as first column in the grid
    //Value - value of the property
    //ControlTypes -  the control types these properties should be displayed for
    var elementProperties = [
             { IntProperty: 'Label', Property: 'Label', Value: '', ControlTypes: ['Tab', 'Textbox', 'Label', 'Multiline TextBox', 'Repeater', 'Calendar', 'Section', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
             { IntProperty: 'UIElementDataTypeID', Property: 'Data Type', Value: '', ControlTypes: ['Textbox', 'Label', 'Multiline TextBox', 'Calendar', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
             { IntProperty: 'IsRequired', Property: 'IsRequired', Value: '', ControlTypes: ['[Blank]'] },
             { IntProperty: 'SearchParameters', Property: 'Search Parameters', Value: '', ControlTypes: ['Tab'] },
             { IntProperty: 'UIElementType', Property: 'UI Element Type', Value: '', ControlTypes: ['Textbox', 'Label', 'Multiline TextBox', 'Repeater', 'Calendar', 'Section', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
             { IntProperty: 'DisplayName', Property: 'Attribute Name', Value: '', ControlTypes: ['Textbox', 'Label', 'Multiline TextBox', 'Repeater', 'Calendar', 'Section', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
             { IntProperty: 'IsKey', Property: 'IsKey', Value: '', ControlTypes: ['Textbox', 'Label', 'Multiline TextBox', 'Repeater', 'Calendar', 'Section', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
    ];
    //get properties applicable to the element type
    var filter = '';
    var filteredProperties = [];
    for (var index = 0; index < elementProperties.length; index++) {
        var filteredTypes = elementProperties[index].ControlTypes.filter(function (ct) { return ct === elementType || ct === 'ALL' });
        if (filteredTypes !== undefined && filteredTypes.length > 0) {
            filteredProperties.push({ IntProperty: elementProperties[index].IntProperty, Property: elementProperties[index].Property, Value: elementProperties[index].Value });
        }
    }
    return filteredProperties;
}

//bind data to the element property grid
serviceDefinitionPropertyGrid.prototype.bindToPropertyGrid = function (serviceDefinitionProperties) {
    //unload previous grid values
    $(this.gridElementId).jqGrid('GridUnload');
    //set column list
    var colArray = ['IntProperty', 'Property', 'Value'];
    //set column  models
    var colModel = [];
    colModel.push({ name: 'IntProperty', index: 'IntProperty', key: true, hidden: true, search: false });
    colModel.push({ name: 'Property', index: 'Property', align: 'left', editable: false });
    colModel.push({ name: 'Value', index: 'Value', align: 'center', editable: false, formatter: this.formatColumn, unformat: this.unFormatColumn });
    //add grid footer
    $(this.gridElementId).parent().append("<div id='p" + this.gridElementIdNoHash + "'></div>");
    //this captured in currentInstance variable so that the currentInstance can be referenced
    //in event handlers(where this is set to the respective element for which the event handler is written
    var currentInstance = this;
    //load the grid
    $(this.gridElementId).jqGrid({
        datatype: 'local',
        colNames: colArray,
        colModel: colModel,
        autowidth: true,
        caption: this.serviceDefinition.DisplayName,
        pager: '#p' + currentInstance.gridElementIdNoHash,
        hidegrid: false,
        height: '360',
        altRows: true,
        altclass: 'alternate',
        //register event handler for row insert
        afterInsertRow: function (rowId, rowData, rowElem) {
            //set values in dropdowns
            if (rowId == 'UIElementType' && rowData.Value == 'Calendar') {
                $(this).find('#UIElementDataTypeID').find('select').find('option').remove();
                $(this).find('#UIElementDataTypeID').find('select').append('<option value="3">Date</option>').val(3);
            }
            if (rowId == 'UIElementDataTypeID') {
                $(this).find('#UIElementDataTypeID').find('select').val(currentInstance.serviceDefinition.UIElementDataTypeID);

                var selectlist = $(this).find("#" + rowId).find('select');
                $(selectlist).bind("focusout", function () {
                    currentInstance.validateRequired(rowId, selectlist);
                });
            }

            if (rowId == 'DisplayName') {
                var displayName = $(this).find("#" + rowId).find('textarea');
                $(displayName).bind("focusout", function () {
                    currentInstance.validateRequired(rowId, displayName);
                });
            }
        },
        gridComplete: function () {
            //TODO: Add authorization code
            //bellow changes to handle authorisation for Dynamic Grid [EQN:99] 
            //authorizePropertyGrid($(this), URLs.formDesignVersionList);
        }
    });
    //insert rows in the grid
    for (var index = 0; index < serviceDefinitionProperties.length; index++) {
        $(this.gridElementId).jqGrid('addRowData', serviceDefinitionProperties[index].IntProperty, serviceDefinitionProperties[index]);
    }
    //set footer
    var pagerElement = '#p' + this.gridElementIdNoHash;
    $(this.gridElementId).jqGrid('navGrid', pagerElement, { refresh: false, edit: false, add: false, del: false, search: false });
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    $(this.gridElementId).find('#SearchParameters').find('.ui-icon-pencil').click(function () {
        var dialog = new serviceParameterDialog(currentInstance.serviceDesignId, currentInstance.serviceDesignName, currentInstance.serviceDesignVersionId, currentInstance.isFinalized, currentInstance.serviceDesignVersionInstance.formDesignId, currentInstance.serviceDesignVersionInstance.formDesignVersionId);
        dialog.show();
    });

    var editFlag = true;
    //TODO: Add permissions
    //editFlag = checkUserPermissionForEditAndDataSource(URLs.formDesignVersionList);
    if (editFlag) {
        //Determine if formversion is finalized    
        if (this.isFinalized != "true") {
            $(this.gridElementId).jqGrid('navButtonAdd', pagerElement,
            {
                caption: 'Save', id: '#PropertyGridSaveButton',
                onClickButton: function () {
                    var grid = this;
                    currentInstance.saveServiceDefinition(grid);
                }
            });
        }
    }
}

serviceDefinitionPropertyGrid.prototype.saveServiceDefinition = function (grid) {
    var currentInstance = this;
    //check if there are validation errors on the form 
    if (!this.hasValidationError($(grid))) {
        var data = this.readGridData();
        data.ServiceDesignVersionID = this.serviceDesignVersionId;

        var promise = ajaxWrapper.postJSON(this.Urls.saveServiceDefinition, data);
        //success callback
        promise.done(function (xhr) {
            if (xhr.Result === ServiceResult.SUCCESS) {
                messageDialog.show('Changes saved succesfully.');
                currentInstance.serviceDesignVersionInstance.loadServiceDefinitionGrid(data.ServiceDefinitionID);
            }
            else {
                messageDialog.show(Common.errorMsg);
            }
        });
        //failure callback
        promise.fail(this.showError);
    }
    else {
        messageDialog.show(Common.validationErrorMsg);
    }
}

//format the grid column based on element Property
//used in formatter in colModel for the Value column : bindToPropertyGrid method
serviceDefinitionPropertyGrid.prototype.formatColumn = function (cellValue, options, rowObject) {
    var result;
    switch (rowObject.IntProperty) {
        case 'UIElementDataTypeID':     //dropdown for Element Data Types
            //<option value="3">Date</option> commented as per discussion with Sushrut on 9/5/2014
            //since Calendar control is available for date, no need to have date datatype for text boxes.
            result = '<select style="width:100%" class="form-control"><option value="1">Integer</option><option value="2">String</option><option value="5">Float</option></select>';
            break;
        case 'IsRequired':
            //set checkbox in column for these properties
            if (cellValue == true) {
                result = '<input type="checkbox" checked/>';
            }
            else {
                result = '<input type="checkbox" />';
            }
            break;
        case 'IsKey':
            //set checkbox in column for these properties
            if (cellValue == true) {
                result = '<input type="checkbox" disabled="disabled" checked/>';
            }
            else {
                result = '<input type="checkbox" disabled="disabled"/>';
            }
            break;
        case 'Label':
        case 'UIElementType':
            result = cellValue;
            break;
        case 'DisplayName':
            //set textarea in column for these properties
            if (cellValue === null || cellValue === undefined) {
                cellValue = '';
            }
            result = '<textarea style="width:100%;" >' + cellValue + '</textarea>';
            break;
        case 'SearchParameters':
            //set edit icon which will be used to load dialogs
            return '<div style="float:right;width:55%"><span class="ui-icon ui-icon-pencil" title = "Manage ' + rowObject.Property + '" style = "cursor: pointer;"></span></div>';
            break;
        case 'IsMultiSelect':
            //set checkbox in column for these properties
            if (cellValue == true) {
                result = '<input type="checkbox" checked/>';
            }
            else {
                result = '<input type="checkbox" />';
            }
            break;
        default:
            if (cellValue === undefined || cellValue === null) {
                result = '';
            }
            else {
                result = cellValue;
            }
            break;
    }
    return result;
}

//unformat the grid column based on element property
//used in unformat in colModel for the Value column : bindToPropertyGrid method
serviceDefinitionPropertyGrid.prototype.unFormatColumn = function (cellValue, options) {
    var result;
    switch (options.rowId) {
        case 'UIElementDataTypeID':
            //extract value from the drop down
            result = $(this).find('#' + options.rowId).find('select').val();
            break;
        case 'IsRequired':
        case 'IsKey':
        case 'IsMultiSelect':
            //extract value from the checkbox
            result = $(this).find('#' + options.rowId).find('input').prop('checked');
            break;
        case 'Label':
        case 'UIElementType':
            result = cellValue;
            break;
        case 'DisplayName':
            //extract value from textarea
            result = $(this).find('#' + options.rowId).find('textarea').val();
            break;
        default:
            result = '';
            break;
    }
    return result;
}

//handler for ajax errors
serviceDefinitionPropertyGrid.prototype.showError = function (xhr) {
    if (xhr.status == 999)
        window.location.href = '/Account/LogOn';
    else
        alert(JSON.stringify(xhr));
}

serviceDefinitionPropertyGrid.prototype.showGrid = function () {
    $(this.gridContainerElementId).show();
}

serviceDefinitionPropertyGrid.prototype.hideGrid = function () {
    $(this.gridContainerElementId).hide();
}

serviceDefinitionPropertyGrid.prototype.hasValidationError = function (grid) {
    //first remove all the validations on the grid
    $(grid).find('.input-validation-error').each(function (val, idx) {
        $(this).removeClass("input-validation-error");
        $(this).attr("data-original-title", "");
    });
    //trigger the validations
    //as all validations are applied on focusout event of control, we need to trigger the focusout event explicitly 
    $(grid).find('input').trigger('focusout');
    $(grid).find('textarea').trigger('focusout');
    $(grid).find('select').trigger('focusout');
    return $(grid).find('.input-validation-error').length > 0;
}

//read the data in the property grid
serviceDefinitionPropertyGrid.prototype.readGridData = function () {
    var serviceDefinitionProperties = this.getServiceDefinitionProperties(this.serviceDefinition.UIElementType);
    var updateElement = {};
    updateElement.ElementType = this.serviceDefinition.UIElementType;
    updateElement.UIElementTypeID = this.serviceDefinition.UIElementTypeID;
    updateElement.UIElementID = this.serviceDefinition.UIElementID;
    updateElement.ParentServiceDefinitionID = this.serviceDefinition.ParentServiceDefinitionID;
    updateElement.ServiceDefinitionID = this.serviceDefinition.ServiceDefinitionID;
    //iterate through each element and set the values in the updateElement object
    for (var index = 0; index < serviceDefinitionProperties.length; index++) {
        switch (serviceDefinitionProperties[index].IntProperty) {
            default:
                updateElement[serviceDefinitionProperties[index].IntProperty] = $(this.gridElementId).getRowData(serviceDefinitionProperties[index].IntProperty).Value;
                break;
        }
    }
    return updateElement;
}

serviceDefinitionPropertyGrid.prototype.destroy = function () {
    $(this.gridElementId).jqGrid('GridUnload');
}

serviceDefinitionPropertyGrid.prototype.validateRequired = function (rowId, control) {
    var currentInstance = this;
    var value = $(control).val();
    var controlLength = value != undefined || value != '' ? value.length : 0;
    var startWithAlphabet = new RegExp(CustomRegexValidation.STARTWITHAPLHABETS);

    if (controlLength == 0) {
        $(control).addClass("input-validation-error");
        $(control).attr("data-original-title", rowId + " is required.");
        $(control).attr("data-toggle", "tooltip");
        $(control).tooltip({
            placement: "left",
            trigger: "hover",
        });
    }
    else if (!(value.match(startWithAlphabet))) {
        $(control).addClass("input-validation-error");
        $(control).attr("data-original-title", rowId + " " + ServiceDesign.nameValidateMsg);
        $(control).attr("data-toggle", "tooltip");
        $(control).tooltip({
            placement: "left",
            trigger: "hover",
        });
    }
    else {
        $(control).removeClass("input-validation-error");
        $(control).attr("data-original-title", "");
    }
}
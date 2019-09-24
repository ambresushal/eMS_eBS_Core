//constructor for property grid of the UI Elements
//params:
//uiElement - uiElement of row selected of the Document Design Version UI ELements Grid
//formDesignVersionId - form design version id of the element
//elementGridData - data from the Document Design Version UI ELements Grid - required for Rules dropdown in rules dialog
function uiGuElementPropertyGrid(uiElement, formDesignId, formDesignVersionId, elementGridData, globalUpdateId) {
    this.uiElement = uiElement;
    this.formDesignId = formDesignId;
    this.tenantId = 1;
    this.formDesignVersionId = formDesignVersionId;
    this.uiElementDetail = null;
    //this.elementGridData = $(elementGridData).getRowData();
    this.elementGridData = elementGridData;//$(elementGridData).jqGrid('getGridParam', 'data');
    this.globalUpdateId = globalUpdateId;
    //this.updateValueConfirmationDetails = confirmationDetails
    //added the call back for loading heirarchical UIElementGrid
    //  this.formDesignVersionInstance = formDesignVersionInstance;



    this.URLs = {
        //to get textbox element details
        textBoxDetail: '/UIElement/TextBox?tenantId=1&formDesignVersionId={formDesignVersionId}&uiElementId={uiElementId}',
        //to get calendar element details
        calendarDetail: '/UIElement/Calendar?tenantId=1&formDesignVersionId={formDesignVersionId}&uiElementId={uiElementId}',
        //to get dropdown element details
        dropDownDetail: '/UIElement/DropDown?tenantId=1&formDesignVersionId={formDesignVersionId}&uiElementId={uiElementId}',
        //to get checkbox element details
        checkBoxDetail: '/UIElement/CheckBox?tenantId=1&formDesignVersionId={formDesignVersionId}&uiElementId={uiElementId}',
        //to get radio button element details
        radioButtonDetail: '/UIElement/RadioButton?tenantId=1&formDesignVersionId={formDesignVersionId}&uiElementId={uiElementId}',
        //to get repeater element details
        repeaterDetail: '/UIElement/Repeater?tenantId=1&formDesignVersionId={formDesignVersionId}&uiElementId={uiElementId}',
        //to get section element details
        sectionDetail: '/UIElement/Section?tenantId=1&formDesignVersionId={formDesignVersionId}&uiElementId={uiElementId}',
        //to get tab element details
        tabDetail: '/UIElement/Tab?tenantId=1&uiElementId={uiElementId}',
        //to update any element
        updateElement: '/GlobalUpdate/UpdateUIElementValue',
        //to check if DataSourceName is unique
        isUniqueDataSourceName: '/UIElement/IsDataSourceNameUnique?tenantId=1&formDesignVersionId={formDesignVersionId}&dataSourceName={dataSourceName}&uiElementId={uiElementId}&uiElementType={uiElementType}',
        //getUpdate Value Confirmation Info
        getConfirmationInfo: '/GlobalUpdate/UpdateValueConfirmationDetails?uiElementId={uiElementId}&formDesignVersionId={formDesignVersionId}&globalUpdateId={globalUpdateId}',
    }

    //generate dynamic grid element id
    this.gridElementId = '#guUpdateValuePropertyGrid'
    //without #
    this.gridElementIdNoHash = 'guUpdateValuePropertyGrid'
    //variable for dropDownItemsDialog - required only for Drop Downs
    //this.dropDownItemsDialog = undefined;
    this.rulesDialogGu = undefined;
    //confirmDialog
    this.confirmDialog = "#confirmUpdateValueDialog";
    //selected Element Grid
    this.selectedElementGrid = '#guSelectedDesignVersionsElements';

    //get  selectedDocument version UiElements
    this.selectedDocumentDesignVersionElementsJQ = '#guSelectedDesignVersionsElements',
    //rule Grid
    this.rulesGridJQ = '#rulesgrid',
    //validationMessage
    this.ErrorMessage=null;

    this.elementHeaderId = 'txtElementHeader';

    this.confirmUpdateValueDetails = undefined;
}

//load property grid
uiGuElementPropertyGrid.prototype.loadPropertyGrid = function () {
    var url;
    //get the url to fetch data for the property grid - for the element selected in the UI Elements grid
    switch (this.uiElement.ElementType) {
        case 'Textbox':
        case 'Multiline TextBox':
        case '[Blank]':
        case 'Label':
            url = this.URLs.textBoxDetail.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId).replace(/\{uiElementId\}/g, this.uiElement.UIElementID);
            break;
        case 'Calendar':
            url = this.URLs.calendarDetail.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId).replace(/\{uiElementId\}/g, this.uiElement.UIElementID);
            break;
        case 'Dropdown List':
        case 'Dropdown TextBox':
            url = this.URLs.dropDownDetail.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId).replace(/\{uiElementId\}/g, this.uiElement.UIElementID);
            break;
        case 'Radio Button':
            url = this.URLs.radioButtonDetail.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId).replace(/\{uiElementId\}/g, this.uiElement.UIElementID);
            break;
        case 'Checkbox':
            url = this.URLs.checkBoxDetail.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId).replace(/\{uiElementId\}/g, this.uiElement.UIElementID);
            break;
        case 'Section':
            url = this.URLs.sectionDetail.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId).replace(/\{uiElementId\}/g, this.uiElement.UIElementID);
            break;
        case 'Repeater':
            url = this.URLs.repeaterDetail.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId).replace(/\{uiElementId\}/g, this.uiElement.UIElementID);
            break;

    }
    //make the ajax call to get the data
    if (url !== undefined) {
        var promise = ajaxWrapper.getJSON(url);
        //need to set current instance to a variable since this defaults to 
        //the callback object for the ajax call
        var currentInstance = this;
        //callback for ajax request success
        promise.done(function (xhr) {
            currentInstance.uiElementDetail = xhr;
            //generate grid data
            var uiElementProperties = currentInstance.generateGridData(xhr);
            currentInstance.bindToPropertyGrid(uiElementProperties);
        });
        //register callback for ajax request failure
        promise.fail(this.showError);
    }

}
//generate data for the grid
//this method is called when the Ajax call to retrieve the element data is successsful
//param xhr contains the data received from the ajax call
uiGuElementPropertyGrid.prototype.generateGridData = function (xhr) {
    //get properties that need to be displayed for the element based on type
    var uiElementProperties = this.getUIElementProperties(this.uiElement.ElementType);
    //populate the Value for each property of the element with the data received from the ajax call
    for (var index = 0; index < uiElementProperties.length; index++) {
        switch (uiElementProperties[index].IntProperty) {
            case 'UIElementDataTypeID':
                uiElementProperties[index].Value = xhr.UIElementDataTypeID;
                break;
            case 'DefaultValue':
                uiElementProperties[index].Value = xhr.DefaultValue === undefined ? xhr.SelectedValue : xhr.DefaultValue;
                break;
            case 'Enabled':
                uiElementProperties[index].Value = xhr.Enabled;
                break;
            case 'CustomRule':
                uiElementProperties[index].Value = xhr.CustomRule;
                this.customRule = xhr.CustomRule;
                break;
            case 'HelpText':
                uiElementProperties[index].Value = xhr.HelpText;
                break;
            case 'IsLabel':
                uiElementProperties[index].Value = xhr.IsLabel;
                break;
            case 'IsMultiLine':
                uiElementProperties[index].Value = xhr.IsMultiLine;
                break;
            case 'IsRequired':
                uiElementProperties[index].Value = xhr.IsRequired;
                break;
            case 'Label':
                uiElementProperties[index].Value = xhr.Label;
                break;
            case 'MaxLength':
                uiElementProperties[index].Value = xhr.MaxLength;
                break;
            case 'Sequence':
                uiElementProperties[index].Value = xhr.Sequence;
                break;
            case 'DefaultBoolValue':
                uiElementProperties[index].Value = xhr.DefaultValue;
                break;
            case 'OptionLabel':
                uiElementProperties[index].Value = xhr.OptionLabel;
                break;
            case 'ChildCount':
                uiElementProperties[index].Value = xhr.ChildCount;
                break;
            case 'LayoutTypeID':
                uiElementProperties[index].Value = xhr.LayoutTypeID;
                break;
            case 'SpellCheck':
                uiElementProperties[index].Value = xhr.SpellCheck;
                break;
            case 'Visible':
                uiElementProperties[index].Value = xhr.Visible;
                break;
            case 'IsYesNo':
                uiElementProperties[index].Value = xhr.IsYesNo;
                break;
            case 'OptionLabelNo':
                uiElementProperties[index].Value = xhr.OptionLabelNo;
                break;
            case 'DefaultDate':
                uiElementProperties[index].Value = xhr.DefaultDate;
                break;
            case 'MaxDate':
                uiElementProperties[index].Value = xhr.MaxDate;
                break;
            case 'MinDate':
                uiElementProperties[index].Value = xhr.MinDate;
                break;
            case 'LibraryRegexID':
                uiElementProperties[index].Value = xhr.LibraryRegexID;
                break;
            case 'Regex':
                uiElementProperties[index].Value = xhr.Regex;
                this.customRegex = xhr.Regex;
                this.CustomRegexMessage = xhr.CustomRegexMessage;
                //this.fieldMask = xhr.CustomFieldMask;
                this.MaskFlag = xhr.MaskFlag;
                break;
            case 'IsLibraryRegex':
                uiElementProperties[index].Value = xhr.IsLibraryRegex;
                break;
            case 'IsDataSource':
                uiElementProperties[index].Value = xhr.IsDataSource;
                break;
            case 'DataSourceName':
                uiElementProperties[index].Value = xhr.DataSourceName;
                break;
            case 'DataSourceDescription':
                uiElementProperties[index].Value = xhr.DataSourceDescription;
                break;
            case 'IsDropDownTextBox':
                uiElementProperties[index].Value = xhr.IsDropDownTextBox;
                break;
            case 'DuplicationCheck':
                uiElementProperties[index].Value = xhr.DuplicationCheck;
                break;
            case 'LoadFromServer':
                uiElementProperties[index].Value = xhr.LoadFromServer;
                break;
            case 'IsDataRequired':
                uiElementProperties[index].Value = xhr.IsDataRequired;
                break;
            case 'CustomHtml':
                uiElementProperties[index].Value = xhr.CustomHtml;
                break;
            case 'IsSortRequired':
                uiElementProperties[index].Value = xhr.IsSortRequired;
                break;
            case 'IsMultiSelect':
                uiElementProperties[index].Value = xhr.IsMultiSelect;
                break;
        }
    }
    return uiElementProperties;
}
//get the data template required for the element type
//the structure of the objects returned is suitable to be bound to the property grid
uiGuElementPropertyGrid.prototype.getUIElementProperties = function (elementType) {
    var currentInstance = this;
    //IntProperty - maps to a property with the same name in the object recieved from the ajax call
    //Property - property name to be displayed as first column in the grid
    //Value - value of the property
    //ControlTypes -  the control types these properties should be displayed for
    var elementProperties = [
             { IsVisible: true, IntProperty: 'Label', Property: 'Element Header', Value: '', ControlTypes: ['Textbox', 'Label', 'Multiline TextBox', 'Repeater', 'Calendar', 'Section', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
             { IsVisible: true, IntProperty: 'DataSourceMapping', Property: 'Is Data Source Mapping', Value: '', ControlTypes: ['Textbox', 'Multiline TextBox', 'Label', 'Calendar', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
             { IsVisible: false, IntProperty: 'IsDropDownTextBox', Property: 'Is DropDown TextBox', Value: '', ControlTypes: ['Dropdown List', 'Dropdown TextBox'] },
             { IsVisible: true, IntProperty: 'DefaultDate', Property: 'Default Date', Value: '', ControlTypes: ['Calendar'] },
             { IsVisible: true, IntProperty: 'UIElementDataTypeID', Property: 'Data Type', Value: '', ControlTypes: ['Textbox', 'Dropdown List', 'Dropdown TextBox'] },
             { IsVisible: false, IntProperty: 'Visible', Property: 'Visible', Value: '', ControlTypes: ['Textbox', 'Label', 'Multiline TextBox', 'Repeater', 'Calendar', 'Section', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
             { IsVisible: false, IntProperty: 'Enabled', Property: 'Enabled', Value: '', ControlTypes: ['Textbox', 'Label', 'Multiline TextBox', 'Repeater', 'Calendar', 'Section', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
             { IsVisible: false, IntProperty: 'IsDataRequired', Property: 'Is Data Required', Value: '', ControlTypes: ['Repeater'] },
             { IsVisible: true, IntProperty: 'OptionLabel', Property: 'Option Label', Value: '', ControlTypes: ['Checkbox', 'Radio Button'] },
             { IsVisible: true, IntProperty: 'OptionLabelNo', Property: 'Option Label No', Value: '', ControlTypes: ['Radio Button'] },
             { IsVisible: true, IntProperty: 'MaxLength', Property: 'Max Length', Value: '', ControlTypes: ['Textbox', 'Multiline TextBox'] },
             { IsVisible: false, IntProperty: 'Sections', Property: 'Sections', Value: '', ControlTypes: ['Tab'] },
             { IsVisible: true, IntProperty: 'IsRequired', Property: 'Is Required', Value: '', ControlTypes: ['Textbox', 'Multiline TextBox', 'Calendar', 'Dropdown List', 'Dropdown TextBox', 'Radio Button'] },
             { IsVisible: false, IntProperty: 'IsDataSource', Property: 'Is Data Source', Value: '', ControlTypes: ['Section', 'Repeater'] },
             { IsVisible: true, IntProperty: 'UpdateValue', Property: 'Update Value', Value: '', ControlTypes: ['ALL'] },
    ];
    //get properties applicable to the element type
    var filter = '';
    var filteredProperties = [];
    for (var index = 0; index < elementProperties.length; index++) {
        var filteredTypes = elementProperties[index].ControlTypes.filter(function (ct) { return ct === elementType || ct === 'ALL' });
        if (filteredTypes !== undefined && filteredTypes.length > 0) {
            filteredProperties.push({ IsVisible: elementProperties[index].IsVisible, IntProperty: elementProperties[index].IntProperty, Property: elementProperties[index].Property, Value: elementProperties[index].Value });
        }
    }
    //if (filteredProperties != null) {
    //    if (currentInstance.formDesignId != FormTypes.MASTERLISTFORMID) {
    //        return filteredProperties.filter(function (ct) { return ct.IntProperty != "LoadFromServer" });
    //    }
    //}
    return filteredProperties;
}

uiGuElementPropertyGrid.prototype.ParentLeftOperand = function (rootExpression) {
    var leftOperandEmptyTextBoxes = new Array();
    this.leftOperand(rootExpression, leftOperandEmptyTextBoxes)
    return leftOperandEmptyTextBoxes;
}

uiGuElementPropertyGrid.prototype.leftOperand = function (rootExpression, leftOperandEmptyTextBoxes) {
    var leftOperandValue = '';
    if (rootExpression.Expressions != null) {
        for (var i = 0; i < rootExpression.Expressions.length; i++) {
            this.leftOperand(rootExpression.Expressions[i], leftOperandEmptyTextBoxes);           
        }
    }
    else {
        leftOperandValue = rootExpression.LeftOperand;
        if (leftOperandValue == null || leftOperandValue == '' || leftOperandValue === undefined) {
            var expId = rootExpression.ExpressionId;
            leftOperandEmptyTextBoxes.push(expId);
        }
    }
}

//bind data to the element property grid
uiGuElementPropertyGrid.prototype.bindToPropertyGrid = function (uiElementProperties) {
    var mode = getQueryString("mode");
    var URLs = {
        //get Document Design List
        formDesignList: '/FormDesign/FormDesignList?tenantId=1',
        formDesignVersionList: '/FormDesign/FormDesignVersionList?tenantId=1'
    }
    //unload previous grid values
    $(this.gridElementId).jqGrid('GridUnload');
    //set column list
    var colArray = ['IntProperty', 'Property', 'Value', 'IsVisible'];
    //set column  models
    var colModel = [];
    colModel.push({ name: 'IntProperty', index: 'IntProperty', key: true, hidden: true, search: false });
    colModel.push({ name: 'Property', index: 'Property', align: 'left', editable: false });
    colModel.push({ name: 'Value', index: 'Value', align: 'center', editable: false, formatter: this.formatColumn, unformat: this.unFormatColumn });
    colModel.push({ name: 'IsVisible', index: 'IsVisible', key: true, hidden: true, search: false });

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
        caption: this.uiElement.Label,
        pager: '#p' + currentInstance.gridElementIdNoHash,
        hidegrid: false,
        height: '290',
        altRows: true,
        altclass: 'alternate',
        //register event handler for row insert
        afterInsertRow: function (rowId, rowData, rowElem) {
            if (!rowData.IsVisible) {
                $('#' + rowData.IntProperty, currentInstance.gridElementId).css({ display: "none" });
            }

            if (rowId == "Label") {
                //add required validation for label
                var label = $(this).find("#" + rowId).find('textarea[name="' + currentInstance.elementHeaderId + '"]');
                $(label).bind("focusout", function () {
                    currentInstance.validateRequired(rowId, label);
                });
            }

            if (rowId == 'UIElementDataTypeID') {
                $(this).find('#UIElementDataTypeID').find('select').val(currentInstance.uiElementDetail.UIElementDataTypeID);
                var grid = this;
                //if the selected value for the Data Type is Date ie 3, convert the DefaultValue text area to Datepicker
                if ($(this).find('#UIElementDataTypeID').find('select').val() == 3) {
                    if ($(grid).find("#DefaultValue").find('textarea') != undefined) {
                        //append the td with datepicker control 
                        //$(grid).find("#DefaultValue").find('textarea').resizable('destroy');
                        $(grid).find("#DefaultValue").find('textarea').parent().append('<input type="text" style="float: left; margin-right: 0px; padding-right: 1px;width: 90%;"/>');
                        //$(grid).find("#DefaultValue").find('textarea').parent().append('<span class="input-group-addon" style="width:auto;"><span class="glyphicon glyphicon-calendar"></span></span>');
                        $(grid).find("#DefaultValue").find('textarea').remove();
                    }
                    $(grid).find("#DefaultValue").find('input').datepicker({
                        dateFormat: 'mm/dd/yy',
                        changeMonth: true,
                        changeYear: true,
                        yearRange: 'c-61:c+20',
                        showOn: "both",
                        buttonImage: '/Content/css/custom-theme/images/calendar.gif',
                        buttonImageOnly: true
                    });
                }
            }

            if (rowId == 'DataSourceMapping') {
                var grid = this;
                var MappingIndicatorElement = '#DataSourceMapping td[aria-describedby=' + currentInstance.gridElementIdNoHash + '_Value]';
                if (currentInstance.uiElementDetail.IsDataSourceMapped) {

                    $(grid).find(MappingIndicatorElement).append('<span disabled class="glyphicon glyphicon-ok text-black"></span>');
                    $(grid).find(MappingIndicatorElement).attr('title', "This Field is used in DataSource.");
                    $(grid).find(MappingIndicatorElement).attr('disabled', "");
                    $(grid).find(MappingIndicatorElement).mouseover(function () {
                        $(MappingIndicatorElement).tooltip('show');
                    });
                }
                else {
                    $(grid).find(MappingIndicatorElement).append('<span disabled class="glyphicon glyphicon-remove text-black"></span>');
                }
            }


            if (rowId == 'DefaultDate' || rowId == 'MaxDate' || rowId == 'MinDate') {
                //add the calendar icon for the text box to convert it to date picker control
                $(this).find('#' + rowId).find('input').datepicker({
                    dateFormat: 'mm/dd/yy',
                    changeMonth: true,
                    changeYear: true,
                    yearRange: 'c-61:c+20',
                    showOn: "both",
                    buttonImage: '/Content/css/custom-theme/images/calendar.gif',
                    buttonImageOnly: true
                });
                $(this).find('#' + rowId).find('input').attr("style", "float: left; margin-right: 0px; padding-right: 1px;width: 70%;");
                $(this).find('#' + rowId).find('input').attr("disabled", "");

                if (rowId == 'MaxDate' || rowId == 'MinDate') {
                    var grid = this;
                    var dateControl = $(this).find("#" + rowId).find('input');
                    //adding the validation for Max Date & Min Date as Max date can not be less than Min Date
                    $(dateControl).bind("focusout", function () {
                        currentInstance.validateMinMaxDate(rowId, $(grid).find('#MinDate').find('input'), $(grid).find('#MaxDate').find('input'));
                    });
                }
            }

        },
        loadComplete: function () {
            currentInstance.GetUpdateValueConfirmationInfo(currentInstance.uiElement.UIElementID, currentInstance.formDesignVersionId, currentInstance.globalUpdateId);
            if (mode == 'view') {
                $("#guUpdateValuePropertyGrid").attr('disabled', 'disabled');
                $("#rulesdialogGu").attr('disabled', 'disabled');
                $(this.p.pager).find('td').addClass('hide');
            }          
        }
    });
    //insert rows in the grid
    for (var index = 0; index < uiElementProperties.length; index++) {
        $(this.gridElementId).jqGrid('addRowData', uiElementProperties[index].IntProperty, uiElementProperties[index]);
    }
    //set footer
    var pagerElement = '#p' + this.gridElementIdNoHash;
    $(this.gridElementId).jqGrid('navGrid', pagerElement, { refresh: false, edit: false, add: false, del: false, search: false });
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    //Note : This needs to handled for Update value
    //register event handler to show the rules dialog when edit icon is clicked on the Rules property
    $(this.gridElementId).find('#UpdateValue').find('.ui-icon-pencil').click(function () {

        if (currentInstance.confirmUpdateValueDetails.HasRule || currentInstance.confirmUpdateValueDetails.HasDataSource || currentInstance.confirmUpdateValueDetails.HasValidation || currentInstance.confirmUpdateValueDetails.HasCascadingRules) {
            $(currentInstance.confirmDialog).find('div p').text(currentInstance.confirmUpdateValueDetails.ValidationMessage);
            $(currentInstance.confirmDialog).dialog({
                title: 'Confirm Update Value',
                height: 120,
                buttons: {
                    Yes: function () {
                        currentInstance.initializeRule();
                        $(this).dialog("close");
                    },
                    No: function () {
                        $(this).dialog("close");
                    }
                }
            });
        }
        else {
            currentInstance.initializeRule();
        }
    });


    $(this.gridElementId).jqGrid('navButtonAdd', pagerElement,
         {
             caption: 'Save', id: '#PropertyGridSaveButton',
             onClickButton: function () {
                 var data = currentInstance.readGridData();
                 currentInstance.GetUpdateValueConfirmationInfo(currentInstance.uiElement.UIElementID, currentInstance.formDesignVersionId, currentInstance.globalUpdateId);
                 var ElementHeaderFromDb = currentInstance.confirmUpdateValueDetails.ElementHeaderText;
                 var ElementHeaderFromGrid = data.ElementHeader;
                 if (ElementHeaderFromDb != ElementHeaderFromGrid) {
                     //Set flag to true if Element Header's value is changed
                     data.IsPropertyGridModified = true;
                 }
                 else {
                     data.IsPropertyGridModified = false;
                 }
                 var grid = this;
                 if ((!currentInstance.hasValidationError($(grid))) || data.IsPropertyGridModified == true) {
                     //get Rules
                     if (currentInstance.rulesDialogGu !== undefined) {
                         data.Rules = currentInstance.rulesDialogGu.getRulesData();
                         //Start LeftOperand validation check
                         var rootExpression = data.Rules[0].RootExpression;
                         if (data.Rules[0].TargetPropertyId != 3) {
                             //Check at least one expression required if filter type selected as 'Expression Value'
                             result = currentInstance.checkExpressionExists(rootExpression);
                             if (rootExpression.Expressions.length == 0 || !result) {
                                 currentInstance.rulesDialogGu.open();
                                 messageDialog.show("Please add expression!!");
                                 return false;
                             }

                             leftOperandEmptyTextBoxesLocal = currentInstance.ParentLeftOperand(rootExpression)
                             for (var i = 0; i < leftOperandEmptyTextBoxesLocal.length; i++) {
                                 var id = "input[data-bind-" + leftOperandEmptyTextBoxesLocal[i]+ "='LeftOperandName']"
                                 $(id).addClass("input-validation-error");
                             }
                             if (leftOperandEmptyTextBoxesLocal.length > 0) {
                                 currentInstance.rulesDialogGu.open();
                                 messageDialog.show("Please enter value for Left Operand!!");
                                 return false;
                             }
                         }
                         //End LeftOperand validation check
                         data.AreRulesModified = currentInstance.rulesDialogGu.isUpdated();
                     }
                     else {
                         data.AreRulesModified = false;
                     }

                     //ajax POST of element properties
                     if (data.AreRulesModified || data.IsPropertyGridModified) {
                         var dataStr = {
                             modelStr: JSON.stringify(data)
                         };
                         var promise = ajaxWrapper.postJSONCustom(currentInstance.URLs.updateElement, dataStr);
                         promise.done(function (xhr) {
                             if (xhr.Result === ServiceResult.SUCCESS) {
                                 $('#span_updateSelectionID').remove();
                                 $('#updateSelectionID').append('<span id="span_updateSelectionID" class="glyphicon glyphicon-check workflow-graphic-icon text-green" style="left: 32px;"></span>');
                             } else {
                                 // Failed
                             }
                         });
                         //register ajax failure callback
                         promise.fail(currentInstance.showError);
                     }
                     var rowId = $(currentInstance.selectedElementGrid).getGridParam('selrow');
                     $(currentInstance.selectedElementGrid).jqGrid('setSelection', rowId);

                 }
                 if (currentInstance.ErrorMessage != null && currentInstance.ErrorMessage != '')
                 {
                     currentInstance.rulesDialogGu.open();
                     messageDialog.show(currentInstance.ErrorMessage);                    

                 }
                 // currentInstance.loadPropertyGrid();

             }
         });
}

uiGuElementPropertyGrid.prototype.checkExpressionExists = function (rootExpression) {
    var result = true;
    if (rootExpression.Expressions != null) {
        for (var i = 0; i < rootExpression.Expressions.length; i++) {
            this.checkExpressionExists(rootExpression.Expressions[i]);
            if (rootExpression.Expressions[i].Expressions != null) {
                if (rootExpression.Expressions[i].Expressions.length > 0) {
                    result = true;
                }
                else
                    result = false;
            }
        }
    }
    else {
        result = false;
    }
    return result;
}

uiGuElementPropertyGrid.prototype.readGridData = function () {
    // var uiElementProperties = this.getUIElementProperties(this.uiElement.ElementType);
    var updateElement = {};
    updateElement.ElementType = this.uiElement.ElementType;
    updateElement.UIElementID = this.uiElement.UIElementID;
    updateElement.FormDesignID = this.formDesignId;
    updateElement.FormDesignVersionID = this.formDesignVersionId;
    updateElement.ParentUIElementID = this.uiElement.ParentUIElementID;
    updateElement.TenantID = this.tenantId;
    updateElement.GlobalUpdateId = this.globalUpdateId;
    updateElement.ElementHeader = $(this.gridElementId).find('textarea[name="' + this.elementHeaderId + '"]').val();

    //iterate through each element and set the values in the updateElement object
    //for (var index = 0; index < uiElementProperties.length; index++) {
    //    switch (uiElementProperties[index].IntProperty) {
    //        case "Rules":
    //        case "Validation":
    //        case "Fields":
    //        case "Sections":
    //            break;
    //        default:
    //            updateElement[uiElementProperties[index].IntProperty] = $(this.gridElementId).getRowData(uiElementProperties[index].IntProperty).Value;
    //            break;
    //    }
    //}
    return updateElement;
}

//BK- Removed FormInstance Data
//format the grid column based on element Property
//used in formatter in colModel for the Value column : bindToPropertyGrid method
uiGuElementPropertyGrid.prototype.formatColumn = function (cellValue, options, rowObject) {
    var result;
    switch (rowObject.IntProperty) {
        case 'UIElementDataTypeID':     //dropdown for Element Data Types
            //<option value="3">Date</option> commented as per discussion with Sushrut on 9/5/2014
            //since Calendar control is available for date, no need to have date datatype for text boxes.
            result = '<select disabled style="width:100%" class="form-control"><option value="1">Integer</option><option value="2">String</option><option value="5">Float</option></select>';
            break;

        case 'Label':
            //set textarea in column for these properties
            if (cellValue === null || cellValue === undefined) {
                cellValue = '';
            }
            result = '<textarea id="txtElementHeader" name ="txtElementHeader" style="width:100%;" >' + cellValue + '</textarea>';
            break;
        case 'MaxLength':
            //set input type=text element
            if (cellValue === null) {
                cellValue = '';
            }
            result = '<input  disabled style="width:100%" value="' + cellValue + '" class="form-control"/>';
            break;

        case 'OptionLabel':
        case 'OptionLabelNo':
            //set textarea in column for these properties
            if (cellValue === null || cellValue === undefined) {
                cellValue = '';
            }
            //set penicil icon for custom regex
            if (rowObject.Property == "Custom Regex") {
                return '<div style="float:right;width:55%"><span class="ui-icon ui-icon-pencil" title = "Manage ' + rowObject.Property + '" style = "cursor: pointer;"></span></div>';
            }
            else {
                result = '<textarea disabled style="width:100%;" >' + cellValue + '</textarea>';
            }
            break;
        case 'IsRequired':
            //set checkbox in column for these properties
            if (cellValue == true) {
                result = '<input disabled type="checkbox" checked/>';
            }
            else {
                result = '<input disabled type="checkbox"/>';
            }
            break;
        case 'Sections':
        case 'UpdateValue':
            //set edit icon which will be used to load dialogs
            return '<div style="float:right;width:55%"><span class="ui-icon ui-icon-pencil" title = "Manage ' + rowObject.Property + '" style = "cursor: pointer;"></span></div>';
            break;
        case 'IsMultiSelect':
            //set checkbox in column for these properties
            if (cellValue == true) {
                result = '<input type="checkbox" checked/>';
            }
            else {
                result = '<input type="checkbox"/>';
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
uiGuElementPropertyGrid.prototype.unFormatColumn = function (cellValue, options) {
    var result;
    switch (options.rowId) {
        case 'UIElementDataTypeID':
            //extract value from the drop down
            result = $(this).find('#' + options.rowId).find('select').val();
            break;
        case 'Label':
            //extract value from textarea
            result = $(this).find('#' + options.rowId).find('textarea').val();
            break;
        case 'MaxLength':
            //extract value from the input type=text element
            result = $(this).find('#' + options.rowId).find('input').val();
            break;

        default:
            result = '';
            break;
    }
    return result;
}

//handler for ajax errors
uiGuElementPropertyGrid.prototype.showError = function (xhr) {
    if (xhr.status == 999)
        window.location.href = '/Account/LogOn';
    else
        alert(JSON.stringify(xhr));
}

uiGuElementPropertyGrid.prototype.initializeRule = function () {
    //Call Rule Dialog
    //  $('#rulesdialogGuBh').find('div p').text(message);
    var currentInstance = this;
    if (currentInstance.rulesDialogGu == null || rulesDialogGu._currentInstance == null || currentInstance.rulesDialogGu.getCurrentElementId() != rulesDialogGu._currentInstance.getCurrentElementId()) {
        currentInstance.rulesDialogGu = new rulesDialogGu(currentInstance.uiElement, currentInstance.formDesignVersionId, currentInstance.elementGridData, currentInstance.globalUpdateId);
        currentInstance.rulesDialogGu.show();
    }
    else {
        currentInstance.rulesDialogGu.open();
    }
}

uiGuElementPropertyGrid.prototype.hasValidationError = function (grid) {
    //first remove all the validations on the grid
    var validationMessage = [];
    this.ErrorMessage = null;
    $(grid).find('.input-validation-error').each(function (val, idx) {
        $(this).removeClass("input-validation-error");
        $(this).attr("data-original-title", "");
    });
    //trigger the validations
    //as all validations are applied on focusout event of control, we need to trigger the focusout event explicitly 
    $(grid).find('input').trigger('focusout');
    $(grid).find('textarea').trigger('focusout');
    $(grid).find('select').trigger('focusout');
    //
    var ruleRowIds = $(this.rulesGridJQ).getDataIDs();

    $(this.rulesGridJQ).find('#' + ruleRowIds[0]).find('#ResultSuccessLabel' + ruleRowId).removeClass("input-validation-error")
    if ((!$(grid).find('.input-validation-error').length > 0) && ruleRowIds.length>0) {
        //Start validation
        
        var ruleRowId = ruleRowIds[0];
        var row = $(this.rulesGridJQ).find('#' + ruleRowIds[0]);
        var elem = row.find('#ResultSuccessLabel' + ruleRowId);
        var newValue = elem[0].value;
        var maxControlLength = $(grid).find("#MaxLength").find('input').val()

        //Start validation
        var rowid = $(this.selectedDocumentDesignVersionElementsJQ).jqGrid('getGridParam', 'selrow');
        var rowData = $(this.selectedDocumentDesignVersionElementsJQ).getRowData(rowid);
        var elementType = rowData.ElementType;
        
        var dataTypeValue = $(this.gridElementId).find('#UIElementDataTypeID').find("option:selected").text();
        // var maxControlLength = (maxValue != undefined) ? maxValue : maxValue;

        if (elementType == 'Calendar') {
            var calenderPattern = new RegExp(/^(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d+$/);
            if (!calenderPattern.test(newValue)) {
                validationMessage.push(GlobalUpdateMessages.dataTypeValidation);
            }
        }
        else if (dataTypeValue == "Float") {
            var regexFloat = new RegExp(/^[+-]?\d+(\.\d+)?$/);
            if (!regexFloat.test(newValue)) {
                validationMessage.push(GlobalUpdateMessages.dataTypeValidation)
            }
        }

        else if (dataTypeValue == "Integer") {
            if (!isFinite(newValue) || !Math.floor(newValue) == newValue) {
                validationMessage.push(GlobalUpdateMessages.dataTypeValidation)
            }
        }
        if ((maxControlLength != undefined) && (newValue.length > maxControlLength) && elementType != 'Calendar') {
            validationMessage.push(GlobalUpdateMessages.maxLengthvalidation)
        }

        if (validationMessage.length > 0) {
            this.ErrorMessage = validationMessage.join() + GlobalUpdateMessages.ruleSuccessValuevalidation;
            $(this.rulesGridJQ).find('#' + ruleRowIds[0]).find('#ResultSuccessLabel' + ruleRowId).addClass("input-validation-error")
            return true;
        }
            

    }
    else {
        return true;
    }
     //return $(grid).find('.input-validation-error').length > 0;
    //End validation


}

uiGuElementPropertyGrid.prototype.GetUpdateValueConfirmationInfo = function (uiElementId, formDesignVersionId, globalUpdateId) {
    var currentInstance = this;

    url = this.URLs.getConfirmationInfo.replace(/\{uiElementId\}/g, uiElementId).replace(/\{formDesignVersionId\}/g, formDesignVersionId).replace(/\{globalUpdateId\}/g, globalUpdateId);
    var promise = ajaxWrapper.getJSON(url);
    //register ajax success callback
    promise.done(function (result) {
        currentInstance.confirmUpdateValueDetails = result;
        if (currentInstance.confirmUpdateValueDetails != undefined) {
            if (currentInstance.confirmUpdateValueDetails.ElementHeaderText != null && currentInstance.confirmUpdateValueDetails.ElementHeaderText != '')
                $(currentInstance.gridElementId).find('textarea[name="'+currentInstance.elementHeaderId+'"]').val(currentInstance.confirmUpdateValueDetails.ElementHeaderText); //elementHeaderId
        }
    });
    //register ajax failure callback
    //promise.fail(showError);
}

uiGuElementPropertyGrid.prototype.validateRequired = function (rowId, control) {
    var currentInstance = this;
    var value = $(control).val();
    var controlLength = value != undefined || value != '' ? value.length : 0;
    if (controlLength == 0) {
        $(control).addClass("input-validation-error");
        $(control).attr("data-original-title", rowId + " is required.");
        $(control).attr("data-toggle", "tooltip");
        $(control).tooltip({
            placement: "left",
            trigger: "hover",
        });
    }
    else {

        var isControlNameDuplicateInParent = currentInstance.elementGridData.filter(function (dt) {
            return dt.parent == currentInstance.uiElementDetail.ParentUIElementID && dt.Label.toUpperCase().replace(/\s/g, '') == value.toUpperCase().replace(/\s/g, '') && dt.UIElementID != currentInstance.uiElementDetail.UIElementID;
        });

        var startWithAlphabet = new RegExp(CustomRegexValidation.STARTWITHAPLHABETS);

        if (isControlNameDuplicateInParent.length > 0) {
            $(control).addClass("input-validation-error");
            $(control).attr("data-original-title", rowId + " is duplicate.");
            $(control).attr("data-toggle", "tooltip");
            $(control).tooltip({
                placement: "left",
                trigger: "hover",
            });
        }
        else if (!(value.match(startWithAlphabet))) {
            $(control).addClass("input-validation-error");
            $(control).attr("data-original-title", rowId + " " + DocumentDesign.nameValidateMsg);
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
}


uiGuElementPropertyGrid.prototype.destroy = function () {
    $(this.gridElementId).jqGrid('GridUnload');
}();


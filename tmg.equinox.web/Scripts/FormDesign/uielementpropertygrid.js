//constructor for property grid of the UI Elements
//params:
//uiElement - uiElement of row selected of the Document Design Version UI ELements Grid
//formDesignVersionId - form design version id of the element
//elementGridData - data from the Document Design Version UI ELements Grid - required for Rules dropdown in rules dialog
function uiElementPropertyGrid(uiElement, formDesignId, formDesignVersionId, status, elementGridData, formDesignVersionInstance) {
    this.uiElement = uiElement;
    this.formDesignId = formDesignId;
    this.tenantId = 1;
    this.formDesignVersionId = formDesignVersionId;
    this.uiElementDetail = null;
    //this.elementGridData = $(elementGridData).getRowData();
    this.elementGridData = $(elementGridData).jqGrid('getGridParam', 'data');
    this.statustext = status;
    //added the call back for loading heirarchical UIElementGrid
    this.formDesignVersionInstance = formDesignVersionInstance;
    this.designRulesTesterData = [];

    var isFinalized = false;
    if (Finalized.length > 0) {
        $.each(Finalized, function (index, value) {
            if (value.FORMDESIGNVERSIONID == formDesignVersionId && value.ISFINALIZED == 1) {
                isFinalized = true;
            }
        });
    }
    if (isFinalized == true)
        this.statustext = "Finalized";

    this.UIElementIDs = {
        //multiple instances for different tabs -generate id dynamically for each form design version id
        propertyGridContainerElement: 'fdvuielemdetail{formDesignVersionId}',
        propertyGridContainerElementContainer: 'fdvuielemdetail{formDesignVersionId}container',
        rulesDialog: "#rulesdialog",
    }
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
        updateElement: '/UIElement/UpdateElement',
        //to check if DataSourceName is unique
        isUniqueDataSourceName: '/UIElement/IsDataSourceNameUnique?tenantId=1&formDesignVersionId={formDesignVersionId}&dataSourceName={dataSourceName}&uiElementId={uiElementId}&uiElementType={uiElementType}',
        //to check if mdm is unique
        isUniqueMdmName: '/UIElement/IsMDMNameUnique?tenantId=1&formDesignId={formDesignId}&mdmName={mdmName}&uiElementId={uiElementId}&uiElementType={uiElementType}',
        //Jamir : to add/update the config rule tester data
        saveConfigRulesTesterData: '/UIElement/SaveConfigRulesTesterData',
    }

    //generate dynamic grid element id
    this.gridElementId = '#' + this.UIElementIDs.propertyGridContainerElement.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId);
    this.gridContainerElementId = '#' + this.UIElementIDs.propertyGridContainerElementContainer.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId);
    //without #
    this.gridElementIdNoHash = this.UIElementIDs.propertyGridContainerElement.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId);
    //variable for dropDownItemsDialog - required only for Drop Downs
    this.dropDownItemsDialog = undefined;
    this.rulesDialog = undefined;
    this.advanceConfigurationDialog = undefined;
    this.repeaterTemplateDialog = undefined;
    this.expressionRulesDialog = undefined;
    this.dataSourceDialog = undefined;
    this.customRulesDialog = undefined;
    this.customRegexDialog = undefined;
    //this variable is required to show value on add/edit custom rule dialog.
    this.customRule = undefined;
    //this variable is required to show value on add/edit custom regex dialog.
    this.customRegex = undefined;
    //this variable is required to show value on add/edit message for custom regex dialog.
    this.CustomRegexMessage = undefined;
    //this.fieldMask = undefined;
    this.MaskFlag = undefined;
    this.roleAccessPermissionDialog = undefined;
    this.duplicationCheck = undefined;
    this.duplicationCheckDialog = undefined;
}

//load property grid
uiElementPropertyGrid.prototype.loadPropertyGrid = function () {
    var url;
    //get the url to fetch data for the property grid - for the element selected in the UI Elements grid
    switch (this.uiElement.ElementType) {
        case 'Textbox':
        case 'Multiline TextBox':
        case '[Blank]':
        case 'Label':
        case 'Rich TextBox':
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
    else {
        //get the url to fetch data for the property grid - for the tab element 
        url = this.URLs.tabDetail.replace(/\{uiElementId\}/g, this.uiElement.UIElementID);
        //make the ajax call to get the data
        if (url !== undefined) {
            var promise = ajaxWrapper.getJSON(url);
            var currentInstance = this;
            //callback for ajax request success
            promise.done(function (xhr) {
                currentInstance.uiElementDetail = xhr;
                var tabUIElementProperties = currentInstance.getUIElementProperties(currentInstance.uiElement.ElementType);
                //Adding custom rule fetched from DB to tabUIElementProperties.
                //tabUIElementProperties[1].Value = xhr.CustomRule;
                //currentInstance.customRule = xhr.CustomRule;
                currentInstance.bindToPropertyGrid(tabUIElementProperties);
            });
            //register callback for ajax request failure
            promise.fail(this.showError);
        }
    }
}

uiElementPropertyGrid.prototype.setClientUserDesignPermissions = function (grid) {
    var currentInstance = this;

    //$("#p" + currentInstance.gridElementIdNoHash).hide();
    $(grid).find('#Label').find('textarea').attr('disabled', 'disabled');
    $(grid).find('#IsStandard').find('input:checkbox').attr('disabled', 'disabled');
    //$(grid).find('#ViewType').find('select').attr('disabled', 'disabled');

    $(grid).find('#IsDropDownTextBox').find('input:checkbox').attr('disabled', 'disabled');
    $(grid).find('#IsDropDownFilterable').find('input:checkbox').attr('disabled', 'disabled');
    $(grid).find('#IsLabel').find('input').attr('disabled', 'disabled');
    $(grid).find('#IsMultiLine').find('input').attr('disabled', 'disabled');
    $(grid).find('#DefaultValue').find('textarea').attr('disabled', 'disabled');
    $(grid).find('#UIElementDataTypeID').find('select').attr('disabled', 'disabled');
    $(grid).find('#Visible').find('input:checkbox').attr('disabled', 'disabled');
    $(grid).find('#Enabled').find('input:checkbox').attr('disabled', 'disabled');
    $(grid).find('#IsSortRequired').find('input:checkbox').attr('disabled', 'disabled');
    $(grid).find('#MaxLength').find('input').attr('disabled', 'disabled');
    $(grid).find('#LayoutTypeID').find('select').attr('disabled', 'disabled');
    $(grid).find('#CustomHtml').find('textarea').attr('disabled', 'disabled');
    $(grid).find('#SpellCheck').find('input:checkbox').attr('disabled', 'disabled');
    $(grid).find('#AllowGlobalUpdates').find('input:checkbox').attr('disabled', 'disabled');
    $(grid).find('#HasCustomRule').find('input:checkbox').attr('disabled', 'disabled');
    $(grid).find('#IsRequired').find('input:checkbox').attr('disabled', 'disabled');
    $(grid).find('#IsLibraryRegex').find('input:checkbox').attr('disabled', 'disabled');
    $(grid).find('#LibraryRegexID').find('select').attr("disabled", "disabled");
    $("#btnCustomRegexDialogSave").parent().hide();
    $(grid).find('#HelpText').find('textarea').attr('disabled', 'disabled');
    $(grid).find('#IsDataSource').find('input:checkbox').attr('disabled', 'disabled');
    $(grid).find('#DataSourceName').find('input').attr('disabled', 'disabled');
    $(grid).find('#DataSourceDescription').find('textarea').attr('disabled', 'disabled');
    $(grid).find('#IsMultiSelect').find('input:checkbox').attr('disabled', 'disabled');
    //----------
    $(grid).find('#LoadFromServer').find('input:checkbox').attr('disabled', 'disabled');
    //$(grid).find('#DuplicationCheck').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled").addClass("divdisabled");
    //$(grid).find('#RepeaterTemplates').show();
    $(grid).find('#Fields').find('input').removeAttr("disabled").removeClass("ui-state-disabled");
}

//generate data for the grid
//this method is called when the Ajax call to retrieve the element data is successsful
//param xhr contains the data received from the ajax call
uiElementPropertyGrid.prototype.generateGridData = function (xhr) {
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
                //case 'CustomRule':
                //    uiElementProperties[index].Value = xhr.CustomRule;
                //    this.customRule = xhr.CustomRule;
                //    break;
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
            case 'IsMultiSelect':
                uiElementProperties[index].Value = xhr.IsMultiSelect;
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
            case 'DefaultBoolValueForRadio':
                var defaultValue = '';
                if (xhr.DefaultValue == true) {
                    defaultValue = "1";
                }
                else if (xhr.DefaultValue == false) {
                    defaultValue = "2";
                }else
                defaultValue = "3";
                uiElementProperties[index].Value = defaultValue;
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
            case 'AllowGlobalUpdates':
                uiElementProperties[index].Value = xhr.AllowGlobalUpdates;
                break;
            case 'HasCustomRule':
                uiElementProperties[index].Value = xhr.HasCustomRule;
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
            case 'AllowBulkUpdate':
                uiElementProperties[index].Value = xhr.AllowBulkUpdate;
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
            case 'IsDropDownFilterable':
                uiElementProperties[index].Value = xhr.IsDropDownFilterable;
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
            case 'JSONElement':
                uiElementProperties[index].Value = this.uiElement.GeneratedName;
                break;
            case 'ViewType':
                uiElementProperties[index].Value = xhr.ViewType;
                break;
            case 'IsStandard':
                uiElementProperties[index].Value = xhr.IsStandard;
                break;
            case 'MDMName':
                uiElementProperties[index].Value = xhr.MDMName;
                break;
        }
    }
    return uiElementProperties;
}

//get the data template required for the element type
//the structure of the objects returned is suitable to be bound to the property grid
uiElementPropertyGrid.prototype.getUIElementProperties = function (elementType) {
    var currentInstance = this;
    //IntProperty - maps to a property with the same name in the object recieved from the ajax call
    //Property - property name to be displayed as first column in the grid
    //Value - value of the property
    //ControlTypes -  the control types these properties should be displayed for
    var elementProperties = [
             { IntProperty: 'JSONElement', Property: 'JSON Element', Value: '', ControlTypes: ['Textbox', 'Label', 'Rich TextBox', 'Multiline TextBox', 'Repeater', 'Calendar', 'Section', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
             { IntProperty: 'Label', Property: 'Label', Value: '', ControlTypes: ['Textbox', 'Label', 'Rich TextBox', 'Multiline TextBox', 'Repeater', 'Calendar', 'Section', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
             { IntProperty: 'MDMName', Property: 'MDM Name', Value: '', ControlTypes: ['Textbox', 'Label', 'Rich TextBox', 'Multiline TextBox', 'Repeater', 'Calendar', 'Section', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
             { IntProperty: 'IsStandard', Property: 'Is Standard', Value: '', ControlTypes: ['Textbox', 'Rich TextBox', 'Multiline TextBox', 'Calendar', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox', 'Section','Repeater'] },
             { IntProperty: 'ViewType', Property: 'View Type', Value: '', ControlTypes: ['Textbox', 'Rich TextBox', 'Multiline TextBox', 'Calendar', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox', 'Section'] },
             { IntProperty: 'DataSourceMapping', Property: 'Is Data Source Mapping', Value: '', ControlTypes: ['Textbox', 'Rich TextBox', 'Multiline TextBox', 'Label', 'Calendar', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
             { IntProperty: 'IsDropDownTextBox', Property: 'Is DropDown TextBox', Value: '', ControlTypes: ['Dropdown List', 'Dropdown TextBox'] },
             { IntProperty: 'IsDropDownFilterable', Property: 'Is DropDown Filterable', Value: '', ControlTypes: ['Dropdown List', 'Dropdown TextBox'] },
             { IntProperty: 'IsLabel', Property: 'Is Label', Value: '', ControlTypes: ['Textbox', 'Label'] },
             { IntProperty: 'IsMultiLine', Property: 'Is MultiLine', Value: '', ControlTypes: ['Textbox', 'Multiline TextBox'] },
             { IntProperty: 'DefaultValue', Property: 'Default Value', Value: '', ControlTypes: ['Textbox', 'Rich TextBox', 'Multiline TextBox', 'Dropdown List', 'Dropdown TextBox'] },
             { IntProperty: 'DefaultDate', Property: 'Default Date', Value: '', ControlTypes: ['Calendar'] },
             { IntProperty: 'DefaultBoolValue', Property: 'Default Value', Value: '', ControlTypes: ['Checkbox'] },
             { IntProperty: 'DefaultBoolValueForRadio', Property: 'Default Value', Value: '', ControlTypes: ['Radio Button'] },
             { IntProperty: 'UIElementDataTypeID', Property: 'Data Type', Value: '', ControlTypes: ['Textbox', 'Dropdown List', 'Dropdown TextBox'] },
             { IntProperty: 'Visible', Property: 'Visible', Value: '', ControlTypes: ['Textbox', 'Label', 'Rich TextBox', 'Multiline TextBox', 'Repeater', 'Calendar', 'Section', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
             { IntProperty: 'Enabled', Property: 'Enabled', Value: '', ControlTypes: ['Textbox', 'Label', 'Rich TextBox', 'Multiline TextBox', 'Repeater', 'Calendar', 'Section', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
             { IntProperty: 'IsDataRequired', Property: 'Is Data Required', Value: '', ControlTypes: ['Repeater'] },
             { IntProperty: 'RoleAccessPermission', Property: 'Role Access Permissions', Value: '', ControlTypes: ['Section'] },
             { IntProperty: 'OptionLabel', Property: 'Option Label', Value: '', ControlTypes: ['Checkbox', 'Radio Button'] },
             { IntProperty: 'OptionLabelNo', Property: 'Option Label No', Value: '', ControlTypes: ['Radio Button'] },
             { IntProperty: 'MaxLength', Property: 'Max Length', Value: '', ControlTypes: ['Textbox', 'Multiline TextBox'] },
             { IntProperty: 'Sections', Property: 'Sections', Value: '', ControlTypes: ['Tab'] },
             { IntProperty: 'Fields', Property: 'Fields', Value: '', ControlTypes: ['Section', 'Repeater'] },
             { IntProperty: 'LoadFromServer', Property: 'Load From Server', Value: '', ControlTypes: ['Repeater'] },
             //{ IntProperty: 'DuplicationCheck', Property: 'Duplication Check', Value: '', ControlTypes: ['Repeater'] }, //Removed duplication for as this will be checked for the keys set for Repeater.
             { IntProperty: 'Items', Property: 'Items', Value: '', ControlTypes: ['Dropdown List', 'Dropdown TextBox'] },
             { IntProperty: 'IsSortRequired', Property: 'Is Sort Required', Value: '', ControlTypes: ['Dropdown List', 'Dropdown TextBox'] },
             { IntProperty: 'Rules', Property: 'Rules', Value: 'Rules[]', ControlTypes: ['Textbox', 'Label', 'Rich TextBox', 'Multiline TextBox', 'Repeater', 'Calendar', 'Section', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
             { IntProperty: 'MaxDate', Property: 'Max Date', Value: '', ControlTypes: ['Calendar'] },
             { IntProperty: 'MinDate', Property: 'Min Date', Value: '', ControlTypes: ['Calendar'] },
             //{ IntProperty: 'CustomRule', Property: 'Custom Rule', Value: '', ControlTypes: ['Tab', 'Textbox', 'Label', 'Multiline TextBox', 'Repeater', 'Calendar', 'Section', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
             { IntProperty: 'SpellCheck', Property: 'SpellCheck', Value: '', ControlTypes: ['Textbox', 'Multiline TextBox'] },
             { IntProperty: 'AllowGlobalUpdates', Property: 'Allow Global Update', Value: '', ControlTypes: ['Textbox', 'Rich TextBox', 'Multiline TextBox', 'Calendar', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
             { IntProperty: 'HasCustomRule', Property: 'Has Child Popup', Value: '', ControlTypes: ['Textbox', 'Multiline TextBox', 'Calendar', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
             { IntProperty: 'IsYesNo', Property: 'Is Yes/No', Value: '', ControlTypes: ['Radio Button'] },
             { IntProperty: 'LayoutTypeID', Property: 'Layout Type', Value: '', ControlTypes: ['Repeater', 'Section'] },
             { IntProperty: 'CustomHtml', Property: 'Custom Layout Html', Value: '', ControlTypes: ['Section'] },
             { IntProperty: 'IsRequired', Property: 'Is Required', Value: '', ControlTypes: ['Textbox', 'Rich TextBox', 'Multiline TextBox', 'Calendar', 'Dropdown List', 'Dropdown TextBox', 'Radio Button'] },
             { IntProperty: 'IsLibraryRegex', Property: 'Is Standard Format?', Value: '', ControlTypes: ['Textbox', 'Rich TextBox', 'Multiline TextBox', 'Dropdown TextBox'] },
             { IntProperty: 'LibraryRegexID', Property: 'Select Standard Format', Value: '', ControlTypes: ['Textbox', 'Rich TextBox', 'Multiline TextBox', 'Dropdown TextBox'] },
             { IntProperty: 'Regex', Property: 'Custom Regex', Value: '', ControlTypes: ['Textbox', 'Rich TextBox', 'Multiline TextBox'] },
             { IntProperty: 'HelpText', Property: 'Help Text', Value: '', ControlTypes: ['Textbox', 'Label', 'Rich TextBox', 'Multiline TextBox', 'Repeater', 'Calendar', 'Section', 'Dropdown List', 'Dropdown TextBox', 'Radio Button', 'Checkbox'] },
             { IntProperty: 'IsDataSource', Property: 'Is Data Source', Value: '', ControlTypes: ['Section', 'Repeater'] },
             { IntProperty: 'AllowBulkUpdate', Property: 'Allow Bulk Update', Value: '', ControlTypes: ['Repeater'] },
             { IntProperty: 'DataSourceName', Property: 'Data Source Name', Value: '', ControlTypes: ['Section', 'Repeater'] },
             { IntProperty: 'DataSourceDescription', Property: 'Description', Value: '', ControlTypes: ['Repeater', 'Section'] },
             { IntProperty: 'UseDataSource', Property: 'Use Data Source', Value: '', ControlTypes: ['Repeater', 'Section', 'Dropdown List', 'Dropdown TextBox'] },
             { IntProperty: 'AdvancedConfiguration', Property: 'Advanced Configuration', Value: '', ControlTypes: ['Repeater'] },
             { IntProperty: 'RepeaterTemplates', Property: 'Repeater Template', Value: '', ControlTypes: ['Repeater'] },
             { IntProperty: 'ExpressionRules', Property: 'Expression Rules', Value: '', ControlTypes: ['Section', 'Repeater', 'Textbox', 'Multiline TextBox', 'Rich TextBox', 'Dropdown List', 'Dropdown TextBox', 'Checkbox', 'Radio Button', 'Calendar'] },
             { IntProperty: 'IsMultiSelect', Property: 'Is MultiSelect', Value: '', ControlTypes: ['Dropdown List', 'Dropdown TextBox'] },

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
    if (filteredProperties != null) {
        if (currentInstance.formDesignId != FormTypes.MASTERLISTFORMID) {
            return filteredProperties.filter(function (ct) { return ct.IntProperty != "LoadFromServer" });
        }
    }
    return filteredProperties;
}

//bind data to the element property grid
uiElementPropertyGrid.prototype.bindToPropertyGrid = function (uiElementProperties) {
    var URLs = {
        //get Document Design List
        formDesignList: '/FormDesign/FormDesignList?tenantId=1',
        formDesignVersionList: '/FormDesign/FormDesignVersionList?tenantId=1'
    }
    //unload previous grid values
    $(this.gridElementId).jqGrid('GridUnload');
    //set column list
    var colArray = ['IntProperty', 'Property', 'Value'];
    //set column  models
    var colModel = [];
    colModel.push({ name: 'IntProperty', index: 'IntProperty', key: true, hidden: true, search: false });
    colModel.push({ name: 'Property', index: 'Property', align: 'left', editable: false, width: '195px' });
    colModel.push({ name: 'Value', index: 'Value', align: 'center', editable: false, formatter: this.formatColumn, unformat: this.unFormatColumn, width: '200px' });
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
        autowidth: false,
        caption: this.uiElement.Label,
        pager: '#p' + currentInstance.gridElementIdNoHash,
        hidegrid: false,
        height: '350',
        altRows: true,
        altclass: 'alternate',
        //register event handler for row insert
        afterInsertRow: function (rowId, rowData, rowElem) {
            //makes all textarea resiable
            //the code is commented as tooltips are not working on resizable textareas
            //if ($(this).find('#' + rowId).find('textarea') != undefined) {
            //    $(this).find('#' + rowId).find('textarea').resizable({
            //        handles: "se",
            //        grid: [10000, 1]
            //    });
            //}

            if (rowId == 'AllowGlobalUpdates') {
                //check fist selected element parant is repetear
                var isParentRepeater = false;
                var parentElements = currentInstance.elementGridData.filter(function (elem) {
                    return elem.UIElementID == currentInstance.uiElement.parent;
                });
                for (i = 0; i < parentElements.length; i++) {
                    if (parentElements[i].ElementType == "Repeater") {
                        isParentRepeater = true;
                    }
                }
                //if parent is repeter then only call for iskey check function.
                if (isParentRepeater) {
                    var grid = this;
                    var data = uiElementPropertyGrid.prototype.CheckIsRepeaterColumnKeyElement(currentInstance.uiElement.UIElementID);
                    if (data) {
                        $(grid).find("#AllowGlobalUpdates").find('input').attr("disabled", "disabled");
                        $(grid).find("#AllowGlobalUpdates").find('input').attr('checked', false);
                    }
                    else {
                        $(grid).find("#AllowGlobalUpdates").find('input').removeAttr("disabled");
                    }
                }
            }

            if (rowId == 'DataSourceMapping') {
                var grid = this;
                var MappingIndicatorElement = '#DataSourceMapping td[aria-describedby=' + currentInstance.gridElementIdNoHash + '_Value]';
                if (currentInstance.uiElementDetail.IsDataSourceMapped) {

                    $(grid).find(MappingIndicatorElement).append('<span class="glyphicon glyphicon-ok text-black"></span>');
                    $(grid).find(MappingIndicatorElement).attr('title', "This Field is used in DataSource.");
                    //$(grid).find(MappingIndicatorElement).mouseover(function () {
                    //    $(MappingIndicatorElement).tooltip('show');
                    //});
                }
                else {
                    $(grid).find(MappingIndicatorElement).append('<span class="glyphicon glyphicon-remove text-black"></span>');
                }
            }



            //set values in dropdowns
            if (rowId == 'ViewType') {
                $(this).find('#ViewType').find('select').val(currentInstance.uiElementDetail.ViewType);
            }
            if (rowId == 'DefaultBoolValueForRadio') {
                var rddefaultValue = 3;
                if (currentInstance.uiElementDetail.DefaultValue == true) {
                    rddefaultValue = "1";
                }
                else if (currentInstance.uiElementDetail.DefaultValue == false) {
                    rddefaultValue = "2";
                } else
                    rddefaultValue = "3";
                $(this).find('#DefaultBoolValueForRadio').find('select').val(rddefaultValue);
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
                        buttonImage: '/Content/css/custom-theme/images/calendar-icon.svg',
                        buttonImageOnly: true
                    });
                }
            }
            if (rowId == 'LayoutTypeID') {
                $(this).find('#LayoutTypeID').find('select').val(currentInstance.uiElementDetail.LayoutTypeID);
                if (!(parseInt(currentInstance.uiElementDetail.LayoutTypeID) == LayoutType.CUSTOMLAYOUT)) {
                    $(currentInstance.gridElementId).find("#" + "CustomHtml").hide();
                }
                else {
                    $(currentInstance.gridElementId).find("#" + "CustomHtml").show();
                }
            }
            if (rowId == 'CustomHtml') {
                var control = $(this).find("#" + "CustomHtml").find('textarea');
                $(control).bind("focusout", function () {
                    var layoutType = $(currentInstance.gridElementId).find("#" + "LayoutTypeID").find('select').val();
                    if (parseInt(layoutType) == LayoutType.CUSTOMLAYOUT) {
                        currentInstance.validateRequiredCustomHtml(rowId, control);
                    }
                });
            }
            if (rowId == 'DefaultDate' || rowId == 'MaxDate' || rowId == 'MinDate') {
                //add the calendar icon for the text box to convert it to date picker control
                $(this).find('#' + rowId).find('input').datepicker({
                    dateFormat: 'mm/dd/yy',
                    changeMonth: true,
                    changeYear: true,
                    yearRange: 'c-61:c+20',
                    showOn: "both",
                    buttonImage: '/Content/css/custom-theme/images/calendar-icon.svg',
                    buttonImageOnly: true
                });
                $(this).find('#' + rowId).find('input').attr("style", "float: left; margin-right: 0px; padding-right: 1px;width: 70%;");
                //$(this).find('#' + rowId).find('input').parent().append('<img class="ui-datepicker-trigger" src="/Content/css/custom-theme/images/calendar.gif" style = "margin-left:-40px">');

                if (rowId == 'MaxDate' || rowId == 'MinDate') {
                    var grid = this;
                    var dateControl = $(this).find("#" + rowId).find('input');
                    //adding the validation for Max Date & Min Date as Max date can not be less than Min Date
                    $(dateControl).bind("focusout", function () {
                        currentInstance.validateMinMaxDate(rowId, $(grid).find('#MinDate').find('input'), $(grid).find('#MaxDate').find('input'));
                    });
                }
            }

            if (rowId == "Label") {
                //add required validation for label
                var label = $(this).find("#" + rowId).find('textarea');
                $(label).bind("focusout", function () {
                    currentInstance.validateRequired(rowId, label);
                });
            }

            //Validate Datasource Name only if Control checked as Datasource           
            if (rowId == 'DataSourceName') {
                var control = $(this).find("#" + "IsDataSource").find('input');
                var label = $(this).find("#DataSourceName").find('input');
                var grid = this;

                if ($(control).is(':checked')) {
                    $(grid).find("#DataSourceName").find('input').on("focusout", function () {
                        currentInstance.validateRequired("DataSourceName", label);
                    });
                    //$(grid).find("#DataSourceName").find('input').on("focusout", function () {
                    //    currentInstance.validateUniqueDataSourceName("DataSourceName", label, currentInstance.uiElement.UIElementID);
                    //});
                } else {
                    $(grid).find("#DataSourceName").find('input').off();
                    $(grid).find("#DataSourceName").find('input').removeClass("input-validation-error");
                    $(grid).find("#DataSourceName").find('input').attr("data-original-title", "");

                }

                $(control).on("change", function () {
                    if ($(control).is(':checked')) {
                        $(grid).find("#DataSourceName").find('input').on("focusout", function () {
                            currentInstance.validateRequired("DataSourceName", label);
                        });
                        //$(grid).find("#DataSourceName").find('input').on("focusout", function () {
                        //    currentInstance.validateUniqueDataSourceName("DataSourceName", label, currentInstance.uiElement.UIElementID);
                        //});
                    }
                    else {

                        $(grid).find("#DataSourceName").find('input').off();
                        $(grid).find("#DataSourceName").find('input').removeClass("input-validation-error");
                        $(grid).find("#DataSourceName").find('input').attr("data-original-title", "");

                    }
                });
            }

            //validations for the text box
            if (currentInstance.uiElement.ElementType == "Label" || currentInstance.uiElement.ElementType == "Multiline TextBox" || currentInstance.uiElement.ElementType == "Textbox"
                || currentInstance.uiElement.ElementType == "Section" || currentInstance.uiElement.ElementType == "Repeater" || currentInstance.uiElement.ElementType == "Dropdown TextBox") {
                //validations for text box
                if (rowId == "Label" || rowId == "DefaultValue" || rowId == "Regex" || rowId == "HelpText" || rowId == "DataSourceDescription" || rowId == 'MDMName') {
                    var grid = this;
                    var control = $(this).find("#" + rowId).find('textarea') || $(this).find("#" + rowId).find('input');
                    //add Max length validation to Label, Default Value, Regex & Help Text
                    $(control).bind("focusout", function () {
                        var length = 0;
                        switch (rowId) {
                            case "Label":
                            case "MDMName":
                                length = 500;
                                break;
                            case "DefaultValue":
                                length = 4000;
                                //For a text box type, if max length is set, then use the value from the 
                                //maxlength property to validate the length of the default value content
                                //use specified length otherwise. 
                                var maxLengthControlValue = $(grid).find("#MaxLength").find('input').val();
                                if (maxLengthControlValue != undefined) {
                                    var intValue = parseInt(maxLengthControlValue, 10);
                                    length = intValue == 0 ? length : intValue;
                                }
                                else {
                                    length = 4000;
                                }
                                break;
                            case "HelpText":
                                length = 1000;
                                break;
                            case "Regex":
                                length = 200;
                                break;
                            case "DataSourceDescription":
                                length = 500;
                                break;
                        }
                        currentInstance.validateMaxLength(rowId, control, length);
                    });
                }
                if (rowId == 'IsLabel') {
                    var control = $(this).find("#" + rowId).find('input');
                    var grid = this;
                    $(control).bind("change", function () {
                        //If IsLabel property of the text box is checked, then below controls should be disabled as 
                        //the text box will be converted to the Label Control so no validations are required
                        //also change the data type to 2 which represents string 
                        //remove disable otherwise. 
                        if ($(control).is(':checked')) {
                            $(grid).find("#IsMultiLine").find('input').attr("disabled", "disabled");
                            $(grid).find("#DefaultValue").find('input').attr("disabled", "disabled");
                            $(grid).find("#UIElementDataTypeID").find('input').attr("disabled", "disabled");
                            $(grid).find("#Enabled").find('input').attr("disabled", "disabled");
                            $(grid).find("#UIElementDataTypeID").find('select').attr("disabled", "disabled");
                            $(grid).find("#UIElementDataTypeID").find('select').val(2);            //assuming 2 is for datatype string  
                            $(grid).find("#MaxLength").find('input').attr("disabled", "disabled");
                            $(grid).find("#SpellCheck").find('input').attr("disabled", "disabled");
                            $(grid).find("#IsRequired").find('input').attr("disabled", "disabled");
                            $(grid).find("#IsLibraryRegex").find('input').attr("disabled", "disabled");
                            $(grid).find("#LibraryRegexID").find('select').attr("disabled", "disabled");
                            $(grid).find("#Regex").find('.ui-icon-pencil').attr("disabled", "disabled");
                        }
                        else {
                            $(grid).find("#IsMultiLine").find('input').removeAttr("disabled");
                            $(grid).find("#DefaultValue").find('input').removeAttr("disabled");
                            $(grid).find("#UIElementDataTypeID").find('input').removeAttr("disabled");
                            $(grid).find("#Enabled").find('input').removeAttr("disabled");
                            $(grid).find("#UIElementDataTypeID").find('select').removeAttr("disabled");
                            $(grid).find("#MaxLength").find('input').removeAttr("disabled");
                            $(grid).find("#SpellCheck").find('input').removeAttr("disabled");
                            $(grid).find("#IsRequired").find('input').removeAttr("disabled");
                            $(grid).find("#IsLibraryRegex").find('input').removeAttr("disabled");
                            $(grid).find("#LibraryRegexID").find('select').removeAttr("disabled");
                            $(grid).find('#Regex').find('.ui-icon-pencil').removeAttr("disabled");
                        }
                    });
                }
                if (rowId == 'IsMultiLine') {
                    $(this).find('#' + rowId).find('select').val(2);        //set data type to string 
                }
                if (rowId == 'IsLibraryRegex') {
                    var grid = this;
                    var LibraryregexID;
                    //if IsLibraryRegex is checked, then enable the Select List to pick Regex from library & disable the Custom Regex text area                    
                    $(grid).find("#" + rowId).find('input').on("change", function () {
                        if ($(grid).find("#" + rowId).find('input').is(':checked')) {
                            $(grid).find("#LibraryRegexID").find('select').removeAttr("disabled");
                            $(grid).find("#LibraryRegexID").find('select').prop('selectedIndex', LibraryregexID);
                            $(grid).find('#Regex').find('.ui-icon-pencil').attr("disabled", "disabled").addClass("ui-state-disabled");

                        }
                        else {
                            LibraryregexID = $(grid).find("#LibraryRegexID").find('select').val()
                            $(grid).find('#Regex').find('.ui-icon-pencil').removeAttr("disabled").removeClass("ui-state-disabled");
                            $(grid).find("#LibraryRegexID").find('select').prop('selectedIndex', 0);
                            $(grid).find("#LibraryRegexID").find('select').attr("disabled", "disabled");
                        }
                    });
                }
                if (rowId == "UIElementDataTypeID" && currentInstance.uiElement.ElementType == "Textbox") {
                    var grid = this;
                    $(this).find("#UIElementDataTypeID").find('select').bind("change", function () {

                        //if the selected value for the data type drop down is 3 that is Date
                        //change the default value text box to datepicker
                        //& set the IsLibraryRegex to true & set defualt Date validation in the Select Library regex drop down list.
                        if ($(this).val() == 3) {
                            if ($(grid).find("#DefaultValue").find('textarea') != undefined) {
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
                                buttonImage: '/Content/css/custom-theme/images/calendar-icon.svg',
                                buttonImageOnly: true
                            });

                            //add default library regex for date here. 
                            $(grid).find("#IsLibraryRegex").find('input').attr("checked", "checked");
                            $(grid).find("#LibraryRegexID").find('select').val(1);         // 1 for date regex

                            $(grid).find('#Regex').find('.ui-icon-pencil').attr("disabled", "disabled").addClass("ui-state-disabled");

                            currentInstance.Regex = null;
                            currentInstance.CustomRegexMessage = null;
                            //currentInstance.fieldMask = null;
                            currentInstance.MaskFlag = null;
                        }
                        else {
                            //revert all the changes & make DefaultValue as textarea again
                            if ($(grid).find("#DefaultValue").find('input') != undefined) {
                                $(grid).find("#DefaultValue").find('input').parent().append('<textarea style="width: 100%; "></textarea>');
                                $(grid).find("#DefaultValue").find('input').remove();
                                $(grid).find("#DefaultValue").find('img').remove();
                                $(grid).find('#Regex').find('.ui-icon-pencil').removeAttr("disabled").removeClass("ui-state-disabled");
                                //remove the default library regex
                                $(grid).find("#IsLibraryRegex").removeAttr("checked");
                            }
                        }
                        //if selected data type float set Library Regex for decimal type.
                        if ($(this).val() == 5) {
                            //add default library regex for date here. 
                            $(grid).find("#IsLibraryRegex").find('input').attr("checked", "checked");
                            $(grid).find("#LibraryRegexID").find('select').val(6);         // 6 for decimal regex

                            $(grid).find('#Regex').find('.ui-icon-pencil').attr("disabled", "disabled").addClass("ui-state-disabled");

                            $(customRegexDialog.elementIDs.customRegex).val(null);
                            //$(customRegexDialog.elementIDs.customFieldMask).val(null);
                            $(customRegexDialog.elementIDs.MaskFlag).val(null);
                        }
                        else {
                            //revert all the changes & make DefaultValue as textarea again
                            if ($(grid).find("#DefaultValue").find('input') != undefined) {
                                //remove the default library regex
                                $(grid).find("#IsLibraryRegex").removeAttr("checked");
                                $(grid).find('#Regex').find('.ui-icon-pencil').removeAttr("disabled").removeClass("ui-state-disabled");
                            }
                        }
                    });
                }
                //validations for text box

                if (rowId == 'MDMName') {
                    var control = $(this).find("#" + "MDMName").find('textarea');
                    var label = $(this).find("#MDMName").find('textarea');
                    var grid = this;

                    $(control).bind("focusout", function () {                        
                        currentInstance.validateUniqueMdmName("MDMName", label, currentInstance.uiElement.UIElementID);;
                    });
                }
            }

            //get library regex list for the dropdown
            if (rowId == 'LibraryRegexID') {
                var promise = masterListManager.getLibraryRegexes(currentInstance.tenantId);
                promise.done(function (xhr) {
                    var sel = $(currentInstance.gridElementId).find('#LibraryRegexID select');
                    $.each(xhr, function (i, item) {
                        $(sel).append($('<option>', {
                            value: item.Key,
                            text: item.Value
                        }));
                    });
                    $(sel).val(currentInstance.uiElementDetail.LibraryRegexID);
                });
                promise.fail(currentInstance.showError);

                var grid = this;
                var control = $(grid).find("#" + "IsLibraryRegex").find('input');
                var label = $(grid).find("#LibraryRegexID").find('select');

                if ($(control).is(':checked')) {
                    $(label).on("focusout", function () {
                        currentInstance.validateRequired("LibraryRegexID", label);
                    });
                } else {
                    $(grid).find("#LibraryRegexID").find('select').val(0);
                    $(grid).find("#LibraryRegexID").find('select').off();
                    $(grid).find("#LibraryRegexID").find('select').removeClass("input-validation-error");
                    $(grid).find("#LibraryRegexID").find('select').attr("data-original-title", "");
                }

                $(control).on("change", function () {
                    if ($(control).is(':checked')) {
                        $(label).on("focusout", function () {
                            currentInstance.validateRequired("LibraryRegexID", label);
                        });
                    }
                    else {
                        $(grid).find("#LibraryRegexID").find('select').off();
                        $(grid).find("#LibraryRegexID").find('select').removeClass("input-validation-error");
                        $(grid).find("#LibraryRegexID").find('select').attr("data-original-title", "");

                    }
                });
            }
        },
        gridComplete: function () {
            var grid = this;
            if (!$(grid).find('#IsLibraryRegex').find('input:checkbox').is(':checked')) {
                $(grid).find('#LibraryRegexID').find('select').attr("disabled", "disabled");
                $(this).find('#Regex').find('.ui-icon-pencil').removeAttr("disabled").removeClass("ui-state-disabled");
            }
            else {
                $(this).find('#Regex').find('.ui-icon-pencil').attr("disabled", "disabled").addClass("ui-state-disabled");
                $(this).find('#LibraryRegexID').find('.select').removeAttr("disabled");
            }

            if (currentInstance.uiElement.IsMappedUIElement === "true") {
                $(grid).find('#Label').find('textarea').attr('disabled', 'disabled');
            }
            else {
                $(grid).find('#Label').find('textarea').removeAttr("disabled");
            }

            if (currentInstance.uiElementDetail != undefined && currentInstance.uiElementDetail.IsDataSourceEnabled === false) {
                $(grid).find('#DataSourceName').find('input').attr('disabled', 'disabled');
                $(grid).find('#IsDataSource').find('input:checkbox').attr('disabled', 'disabled');
            }

            var flag1 = $(grid).parent().find('#LoadFromServer').find('input:checkbox').is(':checked');
            var flag2 = $(grid).parent().find('#LoadFromServer').find('input:checkbox').is(':disabled');

            if (currentInstance.uiElementDetail != undefined && (($(grid).parent().find('#LoadFromServer').find('input:checkbox').is(':checked')) || ($(grid).parent().find('#LoadFromServer').find('input:checkbox').is(':disabled')))) {
                $(grid).find('#Rules').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
                //$(grid).find('#CustomRule').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
            }
            if (currentInstance.uiElementDetail != undefined && currentInstance.uiElementDetail.IsLoadFromServerEnabled === false) {
                $(grid).find('#LoadFromServer').find('input:checkbox').attr('disabled', 'disabled');
                $(grid).find('#IsDataSource').find('input:checkbox').attr('disabled', 'disabled');
                $(grid).find('#DataSourceName').find('input').attr('disabled', 'disabled');
                $(grid).find('#DuplicationCheck').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
                $(grid).find('#UseDataSource').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
                $(grid).find('#Rules').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
                //$(grid).find('#CustomRule').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
            }
            if (currentInstance.formDesignVersionInstance.hasParentElementIsLoadFromServer == true) {
                $(grid).find('#Rules').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
                //$(grid).find('#CustomRule').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
            }
            var LayoutType = currentInstance.uiElementDetail.LayoutTypeID;
            if (!(parseInt(LayoutType) == 7)) {
                $(grid).find('#CustomHtml').hide();//.attr('disabled', 'disabled').addClass("ui-state-disabled");
            }
            else {
                $(grid).find('#CustomHtml').show();
            }

            if (!(parseInt(LayoutType) == 7)) {
                $(grid).find('#RepeaterTemplates').hide();
            }
            else {
                $(grid).find('#RepeaterTemplates').show();
            }
            //bellow changes to handle authorisation for Dynamic Grid [EQN:99] 
            authorizePropertyGrid($(this), URLs.formDesignVersionList);

            var parentId = currentInstance.uiElementDetail.ParentUIElementID;
            for (var index = 0; index < currentInstance.elementGridData.length; index++) {
                var parentElementType = currentInstance.elementGridData[index].ElementType;
                if (currentInstance.elementGridData[index].UIElementID == parentId) {
                    if (parentElementType == "Section") {
                        $(grid).find('#ExpressionRules').show();
                    }
                    else {
                        $(grid).find('#ExpressionRules').hide();
                    }
                } 
                if (currentInstance.elementGridData[index].UIElementID == currentInstance.uiElementDetail.UIElementID && parentElementType == "Section") {
                    $(grid).find('#ExpressionRules').show();
                }
            }

            if (currentInstance.uiElement.IsStandard == "true") {
                $(grid).find('#ViewType').find('select').find("option[value='4']").hide();
                if (ClientRolesForFormDesignAccess.indexOf(vbRole) >= 0) {
                    currentInstance.setClientUserDesignPermissions(grid);
                } else {
                    //$("#p" + currentInstance.gridElementIdNoHash).show();
                    $("#btnCustomRegexDialogSave").parent().show();
                }
            } else {
                $(grid).find('#ViewType').find('select').find("option[value='4']").show();
                //$("#p" + currentInstance.gridElementIdNoHash).show()
                $("#btnCustomRegexDialogSave").parent().show();
                //$("#p" + currentInstance.gridElementIdNoHash).show()
                $("#btnCustomRegexDialogSave").parent().show();
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
    //register event handler to load sections add dialog when edi ticon of Sections property is clicked
    //only used for Tab(the root) control of the form
    $(this.gridElementId).find('#Sections').find('.ui-icon-pencil').click(function () {
        var dialog = new sectionListDialog(currentInstance.uiElement, currentInstance.statustext, currentInstance.formDesignId, currentInstance.formDesignVersionId, currentInstance.formDesignVersionInstance);
        dialog.show();
    });
    //register event handler to display fields dialog when edit icon of fields property is clicked
    $(this.gridElementId).find('#Fields').find('.ui-icon-pencil').click(function () {
        var dialog = new fieldListDialog(currentInstance.uiElement, currentInstance.statustext, currentInstance.formDesignVersionId, currentInstance.formDesignVersionInstance, currentInstance.uiElementDetail);
        dialog.show();
    });

    var currentInstance = this;
    $(this.gridElementId).find('#IsStandard').find('input:checkbox').change(function () {
        if (this.checked) {
            $(currentInstance.gridElementId).find('#ViewType').find('select').find("option[value='4']").hide();
        } else {
            $(currentInstance.gridElementId).find('#ViewType').find('select').find("option[value='4']").show();
        }
    });

    $(this.gridElementId).find('#AdvancedConfiguration').find('.ui-icon-pencil').click(function () {
        if (currentInstance.advanceConfigurationDialog === undefined || currentInstance.advanceConfigurationDialog == null) {
            currentInstance.advanceConfigurationDialog = new advancedConfigurationDialog(currentInstance.uiElement, currentInstance.statustext, currentInstance.formDesignVersionId, currentInstance.formDesignVersionInstance, currentInstance.uiElementDetail);
            currentInstance.advanceConfigurationDialog.show(false);
        }
        else {
            currentInstance.advanceConfigurationDialog.show(true);
        }
    });

    $(this.gridElementId).find('#RepeaterTemplates').find('.ui-icon-pencil').click(function () {
        if (currentInstance.repeaterTemplateDialog === undefined || currentInstance.repeaterTemplateDialog == null) {
            currentInstance.repeaterTemplateDialog = new repeaterTemplateDialog(currentInstance.uiElement, currentInstance.statustext, currentInstance.formDesignVersionId, currentInstance.formDesignVersionInstance, currentInstance.uiElementDetail);
            currentInstance.repeaterTemplateDialog.show(false);
        }
        else {
            currentInstance.repeaterTemplateDialog.show(true);
        }
    });

    $(this.gridElementId).find('#ExpressionRules').find('.ui-icon-pencil').click(function () {
        if (currentInstance.expressionRulesDialog === undefined || currentInstance.expressionRulesDialog == null) {
            //currentInstance.expressionRulesDialog = new expressionRulesDialog(currentInstance.uiElement, currentInstance.formDesignId, currentInstance.formDesignVersionId, currentInstance.statustext);
            //currentInstance.expressionRulesDialog.show(false);
            currentInstance.expressionRulesDialog = new expressionRulesDialogNew(currentInstance.uiElement, currentInstance.formDesignId, currentInstance.formDesignVersionId, currentInstance.formDesignVersionInstance.formDesignVersion.EffectiveDate);
            currentInstance.expressionRulesDialog.show();
        }
        else {
            currentInstance.expressionRulesDialog.show();
        }
    });

    //register event handler to show the drop down items dialog when edit icon is clicked on the Items property
    //only for dropdowns
    $(this.gridElementId).find('#Items').find('.ui-icon-pencil').click(function () {
        if (currentInstance.dropDownItemsDialog === undefined) {
            currentInstance.dropDownItemsDialog = new dropdownItemsDialog(currentInstance.uiElementDetail, currentInstance.statustext, currentInstance.formDesignVersionId);
            currentInstance.dropDownItemsDialog.show(false);
        }
        else {
            currentInstance.dropDownItemsDialog.show(true);
        }
    });
    //register event handler to show the rules dialog when edit icon is clicked on the Rules property
    $(this.gridElementId).find('#Rules').find('.ui-icon-pencil').click(function () {
        if (currentInstance.rulesDialog == null || rulesDialog._currentInstance == null || currentInstance.rulesDialog.getCurrentElementId() != rulesDialog._currentInstance.getCurrentElementId()) {
            currentInstance.rulesDialog = new rulesDialog(currentInstance.uiElement, currentInstance.formDesignVersionId, currentInstance.statustext, currentInstance.elementGridData);
            currentInstance.rulesDialog.setRuleTesterData(currentInstance.designRulesTesterData);
            currentInstance.rulesDialog.show();
        }
        else {
            currentInstance.rulesDialog.setRuleTesterData(currentInstance.designRulesTesterData);
            currentInstance.rulesDialog.open();
        }
    });

    $(currentInstance.UIElementIDs.rulesDialog).on("dialogclose", function (event, ui) {
        if (currentInstance.rulesDialog != null || currentInstance.rulesDialog != undefined) {
            currentInstance.designRulesTesterData = currentInstance.rulesDialog.getRuleTesterData();
        }
    });

    //register event handler to show the custom rules dialog when edit icon is clicked on the Custom Rule property
    //$(this.gridElementId).find('#CustomRule').find('.ui-icon-pencil').click(function () {
    //    currentInstance.customRulesDialog = new customRulesDialog(currentInstance.uiElement, currentInstance.formDesignVersionId, currentInstance.statustext, currentInstance.customRule);
    //    currentInstance.customRulesDialog.show();
    //});
    //register event handler to show the Duplication Check dialog when edit icon is clicked on the Duplication Check property
    $(this.gridElementId).find('#DuplicationCheck').find('.ui-icon-pencil').click(function () {
        var grid = this;
        var flag1 = $(currentInstance.gridElementId).parent().find('#LoadFromServer').find('input:checkbox').is(':checked');
        currentInstance.uiElement.hasLoadFromServer = String(flag1);
        currentInstance.duplicationCheckDialog = new duplicationCheckDialog(currentInstance.uiElement, currentInstance.formDesignVersionId, currentInstance.statustext);
        currentInstance.duplicationCheckDialog.show();
    });

    //register event handler to show the custom Regex dialog when edit icon is clicked on the Custom Regex property
    $(this.gridElementId).find('#Regex').find('.ui-icon-pencil').click(function () {
        currentInstance.customRegexDialog = new customRegexDialog(currentInstance.uiElement, currentInstance.formDesignVersionId, currentInstance.customRegex, currentInstance.CustomRegexMessage, currentInstance.MaskFlag);
        currentInstance.customRegexDialog.show(currentInstance.statustext);

    });

    $(this.gridElementId).find('#LayoutTypeID').change(function () {
        var layoutType = $('#LayoutTypeID option:selected').val();
        if (!(parseInt($('#LayoutTypeID option:selected').val()) == 7)) {
            $(currentInstance.gridElementId).find("#" + "CustomHtml").find('textarea').val("");
            $(currentInstance.gridElementId).find('#CustomHtml').hide();
        }
        else {
            $(currentInstance.gridElementId).find('#CustomHtml').show();
        }

        if (parseInt($('#LayoutTypeID option:selected').val()) == 7) {
            $(currentInstance.gridElementId).find('#RepeaterTemplates').show();
        }
        else {
            $(currentInstance.gridElementId).find('#RepeaterTemplates').hide();
        }
    });

    $(this.gridElementId).find('#LayoutTypeID').click(function () {
        if (!(currentInstance.uiElement.ElementType == "Section") && !(currentInstance.uiElement.ElementType == "Repeater")) {
            $("#LayoutTypeID option[value='7']").remove();
        }
    });

    if (currentInstance.uiElement.ElementType == "Repeater") {
        $("#LayoutTypeID option[value='1']").remove();
        $("#LayoutTypeID option[value='2']").remove();
        $("#LayoutTypeID option[value='3']").remove();
    }

    if (currentInstance.uiElement.ElementType == "Section") {
        $("#LayoutTypeID option[value='5']").remove();
    }

    //register eventHandler to show the  datasourcedialog when the edit icon is clicked on the use data source property
    $(this.gridElementId).find('#UseDataSource').find('.ui-icon-pencil').click(function () {
        currentInstance.dataSourceDialog = new dataSourceDialog(currentInstance.uiElement, currentInstance.formDesignId, currentInstance.formDesignVersionId, currentInstance.statustext, currentInstance.elementGridData, currentInstance.formDesignVersionInstance);
        currentInstance.dataSourceDialog.show();
    });

    //register eventHandler to show the  User Access Level when the edit icon is clicked on the User Access Level property
    $(this.gridElementId).find('#RoleAccessPermission').find('.ui-icon-pencil').click(function () {
        currentInstance.roleAccessPermissionDialog = new roleAccessPermissionDialog(currentInstance.uiElement, currentInstance.formDesignVersionId);
        currentInstance.roleAccessPermissionDialog.show(currentInstance.statustext);
    });

    $(customRegexDialog.elementIDs.customRegexDialog + ' button').on('click', function () {
        //custom regex added by user     
        var customRegex = $(customRegexDialog.elementIDs.customRegex).val();
        //custom message added by user     
        var message = $(customRegexDialog.elementIDs.validationMessage).val();
        //MaskFlag checked by user     
        var maskFlag = $(customRegexDialog.elementIDs.MaskFlag).prop('checked');

        currentInstance.customRegex = customRegex;
        currentInstance.CustomRegexMessage = message;
        currentInstance.MaskFlag = maskFlag;

        //used to validate regex entered by user in custom regex dialog box
        if (validateRegex(customRegex)) {
            $(customRegexDialog.elementIDs.customRegexDialog).dialog("close");
        }
        else {
            messageDialog.show(DocumentDesign.invalidRegexMsg);
        }

    });

    if ((currentInstance.formDesignId == FormTypes.MASTERLISTFORMID && currentInstance.uiElementDetail.IsDataSource == true)) {

        $(currentInstance.gridElementId).find('#LoadFromServer').find('input:checkbox').attr('disabled', 'disabled').addClass("ui-state-disabled");
    }
    //register event handler to Save the element properties
    //event to check if the logged in user has a permissions for accessing the data source.
    var editFlag = false;
    editFlag = checkUserPermissionForEditAndDataSource(URLs.formDesignVersionList);
    if (editFlag) {
        //Determine if formversion is finalized    

        // if (this.statustext != 'Finalized') {
        $(this.gridElementId).jqGrid('navButtonAdd', pagerElement,
        {
            caption: 'Save', id: '#PropertyGridSaveButton',
            onClickButton: function () {

                var grid = this;
                //check if there are validation errors on the form 
                if (!currentInstance.hasValidationError($(grid))) {
                    var data = currentInstance.readGridData();

                    if (data.Label != undefined) {
                        data.Label = data.Label.replace(/\s+/g, " ");
                    }

                    //Set UIElementDataTypeID as string for Label and Multiline TextBox
                    if (data.ElementType === 'Label' || data.ElementType === 'Multiline TextBox' || data.ElementType === 'Dropdown TextBox' || data.ElementType === 'Rich TextBox') {
                        data.UIElementDataTypeID = 2;
                    }
                    //get dropdown items
                    if (currentInstance.dropDownItemsDialog !== undefined) {
                        data.Items = currentInstance.dropDownItemsDialog.getDialogData();
                    }
                    else {
                        if (currentInstance.uiElementDetail != undefined)
                            data.Items = currentInstance.uiElementDetail.Items;
                    }
                    //get Rules
                    if (currentInstance.rulesDialog !== undefined) {
                        data.Rules = currentInstance.rulesDialog.getRulesData();
                        data.AreRulesModified = currentInstance.rulesDialog.isUpdated();
                    }
                    else {
                        data.AreRulesModified = false;
                    }

                    // Set AdvanceConfiguration
                    if (currentInstance.advanceConfigurationDialog !== undefined) {
                        data.AdvancedConfiguration = currentInstance.advanceConfigurationDialog.getConfigurationData()[0];
                    }

                    // Set RepeaterTemplates
                    if (currentInstance.repeaterTemplateDialog !== undefined) {
                        data.RepeaterTemplates = currentInstance.repeaterTemplateDialog.getConfigurationData()[0];
                    }

                    //get Custom Rule
                    //if (currentInstance.customRulesDialog !== undefined) {
                    //    data.IsCustomRulesModified = true;
                    //    var customRule = currentInstance.customRulesDialog.getCustomRulesData();
                    //    if (customRule !== undefined && customRule != null && customRule.trim() != '')
                    //        data.HasCustomRule = true;
                    //    else
                    //        data.HasCustomRule = false;
                    //    data.CustomRule = customRule;
                    //    currentInstance.customRule = customRule
                    //}
                    //else {
                    //    data.IsCustomRulesModified = false;
                    //}

                    //get Custom data
                    if (data.IsLibraryRegex == false) {
                        data.Regex = currentInstance.customRegex;
                        data.CustomRegexMessage = currentInstance.CustomRegexMessage;
                        //data.customFieldMask = currentInstance.fieldMask;
                        data.MaskFlag = currentInstance.MaskFlag;
                    }
                    else {
                        data.Regex = null;
                        data.CustomRegexMessage = null;
                        //data.customFieldMask = null;
                        data.MaskFlag = null;
                    }

                    if (data.LoadFromServer && !($(grid).find('#LoadFromServer').find('input:checkbox').is(':disabled'))) {
                        confirmDialog.show((DocumentDesign.allowRepeaterLoadFromServerConfirmSaveMsg), function () {
                            confirmDialog.hide();
                            currentInstance.uiElement.hasLoadFromServer = data.LoadFromServer.toString();
                            if (parseInt(data.LayoutTypeID) == LayoutType.CUSTOMLAYOUT) {
                                currentInstance.allowCustomLayoutForElement(currentInstance, data);
                            }
                            else {
                                currentInstance.postFormInstanceData(data, currentInstance);
                            }
                        });
                    }
                    else {
                        if (parseInt(data.LayoutTypeID) == LayoutType.CUSTOMLAYOUT) {
                            currentInstance.allowCustomLayoutForElement(currentInstance, data);
                        }
                        else {
                            currentInstance.postFormInstanceData(data, currentInstance);
                        }
                    }
                }
                else {
                    messageDialog.show(Common.validationErrorMsg);
                }
            }
        });

        // }
    }
}

uiElementPropertyGrid.prototype.saveDesignRulesTesterData = function () {
    var currentInstance = this;
    var designRulesTesterData = currentInstance.designRulesTesterData.filter(function (data) { return data.ruleId > 0 });

    if (designRulesTesterData.length > 0) {
        var configRulesTesterData = {
            tenantId: currentInstance.tenantId,
            designRulesTesterData: JSON.stringify(designRulesTesterData)
        }

        var url = currentInstance.URLs.saveConfigRulesTesterData;
        var promise = ajaxWrapper.postJSON(url, configRulesTesterData);

        promise.done(function (xhr) {
            if (xhr.Result === ServiceResult.SUCCESS) {
                currentInstance.designRulesTesterData = [];
                //messageDialog.show("Config rule test data saved successfully.");
            }
            else {
                //messageDialog.show("Config rule test data not saved.");
            }
        });
    }
}

uiElementPropertyGrid.prototype.allowCustomLayoutForElement = function (currentInstance, data) {
    var isSubSectionOrRepeaterInsideSection = false;
    var childElements = currentInstance.elementGridData.filter(function (elem) {
        return elem.parent == currentInstance.uiElement.UIElementID;
    });
    for (i = 0; i < childElements.length; i++) {
        if (childElements[i].ElementType == "Repeater" || childElements[i].ElementType == "Section") {
            isSubSectionOrRepeaterInsideSection = true;
        }
    }
    if (!isSubSectionOrRepeaterInsideSection) {
        currentInstance.postFormInstanceData(data, currentInstance);
    }
    else {
        messageDialog.show(DocumentDesign.restrictExistingLayoutToChangeMsg);
        return false;
    }
}
uiElementPropertyGrid.prototype.postFormInstanceData = function (data, currentInstance) {
    //ajax POST of element properties
    var dataStr = {
        modelStr: JSON.stringify(data)
    };
    var promise = ajaxWrapper.postJSONCustom(currentInstance.URLs.updateElement, dataStr);
    promise.done(function (xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            if (xhr.Items.length > 0 && xhr.Items[0].Messages[0] != undefined && xhr.Items[0].Messages[0] != null) {
                if (xhr.Items[0].Messages[1] != undefined && xhr.Items[0].Messages[1] != null) {
                    currentInstance.formDesignVersionInstance.buildGrid(parseInt(xhr.Items[0].Messages[0]), parseInt(xhr.Items[0].Messages[1]));
                } else {
                    currentInstance.formDesignVersionInstance.buildGrid(parseInt(xhr.Items[0].Messages[0]));
                }
            }
            else {
                currentInstance.formDesignVersionInstance.buildGrid(currentInstance.uiElementDetail.ParentUIElementID, currentInstance.uiElementDetail.UIElementID);
            }
            if (currentInstance.uiElement.hasLoadFromServer == "true") {
                $(currentInstance.gridElementId).find('#LoadFromServer').find('input:checkbox').attr('disabled', 'disabled');
                $(currentInstance.gridElementId).find('#IsDataSource').find('input:checkbox').attr('disabled', 'disabled');
                $(currentInstance.gridElementId).find('#DataSourceName').find('input').attr('disabled', 'disabled');
                $(currentInstance.gridElementId).find('#DuplicationCheck').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
                $(currentInstance.gridElementId).find('#UseDataSource').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
                $(currentInstance.gridElementId).find('#Rules').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
                //$(currentInstance.gridElementId).find('#CustomRule').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
            }
            messageDialog.show(DocumentDesign.saveMsg);
            //Jamir : Added this function to add/update the config rule tester data.
            currentInstance.saveDesignRulesTesterData();
        } else {
            if (xhr.Items.length > 0 && xhr.Items[0].Messages[0] != undefined && xhr.Items[0].Messages[0] != null) {
                messageDialog.show(xhr.Items[0].Messages[0]);
                if (xhr.Items[0].Messages[1] != undefined && xhr.Items[0].Messages[1] != null) {
                    currentInstance.loadPropertyGrid();
                }
            }
        }

    });
    //register ajax failure callback
    promise.fail(currentInstance.showError);

}

uiElementPropertyGrid.prototype.hasValidationError = function (grid) {
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

uiElementPropertyGrid.prototype.validateMaxLength = function (rowId, control, length) {
    var controlLength = $(control).val().length;
    if (controlLength > length) {
        $(control).addClass("input-validation-error");
        $(control).attr("data-original-title", DocumentDesign.maxLengthMsg + length + " chars.");
        $(control).attr("data-toggle", "tooltip");
        $(control).tooltip({
            placement: "left",
            trigger: "hover",
        });
    }
}

uiElementPropertyGrid.prototype.CheckIsRepeaterColumnKeyElement = function (uiElementID) {
    var result = false;
    $.ajax({
        type: "POST", url: '/UIElement/CheckIsRepeaterColumnKeyElement?tenantId=1&uiElementId=' + uiElementID,
        async: false,
        success: function (data) {
            result = data;
        }
    });
    return result;
}

uiElementPropertyGrid.prototype.validateUniqueDataSourceName = function (rowId, control, uiElementID) {
    var dataSourceName = $(control).text() || $(control).val();
    if (dataSourceName != undefined && dataSourceName != '') {
        var dataSourceDetails = {
            dataSourceName: dataSourceName
        }
        url = this.URLs.isUniqueDataSourceName.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId).replace(/\{dataSourceName\}/g, dataSourceName).replace(/\{uiElementId\}/g, uiElementID).replace(/\{uiElementType\}/g, this.uiElement.ElementType);

        //ajax call to add/update
        var promise = ajaxWrapper.postJSON(url, dataSourceDetails);
        //register ajax success callback
        promise.done(function (result) {
            if (result == false) {
                $(control).addClass("input-validation-error");
                $(control).attr("data-original-title", DocumentDesign.DataSourceNameUniqueMsg);
                $(control).attr("data-toggle", "tooltip");
                $(control).tooltip({
                    placement: "left",
                    trigger: "hover",
                });
            }
        });
        //register ajax failure callback
        promise.fail(this.showError);
    }
}

uiElementPropertyGrid.prototype.validateUniqueMdmName = function (rowId, control, uiElementID) {
    var startWithAlphabet = new RegExp(CustomRegexValidation.STARTWITHAPLHABETS);
    var specialChar = new RegExp(CustomRegexValidation.AVOIDSPECIALCHARACTER);
    var value = $(control).val();
    var keyValue = ["forminstanceid", "sequence", "id", "rowidproperty"];
    if (value == "" || value == undefined) {
        $(control).removeClass("input-validation-error");
        $(control).attr("data-original-title", "");
    }
   else if (!value.match(startWithAlphabet)) {
        $(control).addClass("input-validation-error");
        $(control).attr("data-original-title", rowId + " " + DocumentDesign.nameValidateMsg);
        $(control).attr("data-toggle", "tooltip");
        $(control).tooltip({
            placement: "left",
            trigger: "hover",
        });
    }
    else if (!(value.match(specialChar))) {
        $(control).addClass("input-validation-error");
        $(control).attr("data-original-title", rowId + " " + DocumentDesign.specialCharNotAllowedMsg);
        $(control).attr("data-toggle", "tooltip");
        $(control).tooltip({
            placement: "left",
            trigger: "hover",
        });
    }
    else if (keyValue.indexOf(value.toString().toLowerCase()) != -1) {
        $(control).addClass("input-validation-error");
        $(control).attr("data-original-title", value + " Is Key value please enter other alias");
        $(control).attr("data-toggle", "tooltip");
        $(control).tooltip({
            placement: "left",
            trigger: "hover",
        });
    }
    else {
        var mdmName = $(control).val();
        if (mdmName != undefined && mdmName != '') {
            var dataSourceDetails = {
                dataSourceName: mdmName
            }
            url = this.URLs.isUniqueMdmName.replace(/\{formDesignId\}/g, this.formDesignId).replace(/\{mdmName\}/g, mdmName).replace(/\{uiElementId\}/g, uiElementID).replace(/\{uiElementType\}/g, this.uiElement.ElementType);

            //ajax call to add/update
            var promise = ajaxWrapper.postAsyncJSONCustom(url, dataSourceDetails);
            //register ajax success callback
            promise.done(function (result) {
                if (result == true) {
                    $(control).addClass("input-validation-error");
                    $(control).attr("data-original-title", DocumentDesign.mdmUniqueMsg);
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
            });
            //register ajax failure callback
            promise.fail(this.showError);
        }
    }
}

uiElementPropertyGrid.prototype.validateRequiredCustomHtml = function (rowId, control) {
    var customHtml = control.val();
    var controlLength = customHtml != undefined || customHtml != '' ? customHtml.length : 0;
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
        $(control).removeClass("input-validation-error");
        $(control).attr("data-original-title", "");
    }
}

uiElementPropertyGrid.prototype.validateRequired = function (rowId, control) {
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

uiElementPropertyGrid.prototype.validateMinMaxDate = function (rowId, minDateControl, maxDateControl) {
    if ($(minDateControl).val() != undefined && $(maxDateControl).val() != undefined) {
        if (new Date($(minDateControl).val()) > new Date($(maxDateControl).val())) {
            $(minDateControl).addClass("input-validation-error");
            $(minDateControl).attr("data-original-title", DocumentDesign.MinMaxDateValidateMsg);
            $(minDateControl).attr("data-toggle", "tooltip");
            $(minDateControl).tooltip({
                placement: "left",
                trigger: "hover"
            });
        }
    }
}
//read the data in the property grid
uiElementPropertyGrid.prototype.readGridData = function () {
    var uiElementProperties = this.getUIElementProperties(this.uiElement.ElementType);
    var updateElement = {};
    updateElement.ElementType = this.uiElement.ElementType;
    updateElement.UIElementID = this.uiElement.UIElementID;
    updateElement.FormDesignID = this.formDesignId;
    updateElement.FormDesignVersionID = this.formDesignVersionId;
    updateElement.ParentUIElementID = this.uiElement.ParentUIElementID;
    updateElement.TenantID = this.tenantId;
    //iterate through each element and set the values in the updateElement object
    for (var index = 0; index < uiElementProperties.length; index++) {
        switch (uiElementProperties[index].IntProperty) {
            case "Rules":
            case "Validation":
            case "Fields":
            case "Sections":
            case "AdvancedConfiguration":
            case "RepeaterTemplates":
            case "ExpressionRules":
                break;
            case "DefaultBoolValueForRadio":
                var defaultValue = null;
                var radioval = $(this.gridElementId).getRowData(uiElementProperties[index].IntProperty).Value;
                if (radioval == "1") {
                    defaultValue = true;
                }
                else if (radioval == "2") {
                    defaultValue = false;
                }
                updateElement[uiElementProperties[index].IntProperty] = defaultValue;
                break;
            default:
                updateElement[uiElementProperties[index].IntProperty] = $(this.gridElementId).getRowData(uiElementProperties[index].IntProperty).Value;
                break;
        }
    }
    return updateElement;
}

//format the grid column based on element Property
//used in formatter in colModel for the Value column : bindToPropertyGrid method
uiElementPropertyGrid.prototype.formatColumn = function (cellValue, options, rowObject) {
    var result;
    switch (rowObject.IntProperty) {
        case 'UIElementDataTypeID':     //dropdown for Element Data Types
            //<option value="3">Date</option> commented as per discussion with Sushrut on 9/5/2014
            //since Calendar control is available for date, no need to have date datatype for text boxes.
            result = '<select style="width:100%" class="form-control"><option value="1">Integer</option><option value="2">String</option><option value="5">Float</option></select>';
            break;
        case 'LayoutTypeID':        //dropdown for Layout Types
            result = '<select style="width:100%" class="form-control"><option value="1">Three-Column Layout</option><option value="2">Two-Column Layout</option><option value="3">Single-Column Layout</option><option value="5">Grid Layout</option><option value="7">Custom Layout</option></select>';
            break;
        case 'ViewType':        //dropdown for Layout Types
            result = '<select style="width:100%" class="form-control"><option value="1">Folder View</option><option value="2">SOT View</option><option value="3">Both</option><option value="4">None</option></select>';
            break;
        case 'LibraryRegexID':   //dropdown for Library Regex ID's
            result = '<select style="width:100%" class="form-control"><option value="">Select Standard Format</option></select>';
            break;
        case 'Enabled':
        case 'HasCustomRule':
        case 'AllowGlobalUpdates':
        case 'IsLabel':
        case 'IsMultiLine':
        case 'AllowBulkUpdate':
        case 'IsRequired':
        case 'IsYesNo':
        case 'SpellCheck':
        case 'Visible':
        case 'DefaultBoolValue':
        case 'IsLibraryRegex':
        case 'IsDropDownFilterable':
        case 'IsMultiSelect':
        case 'IsStandard':
            //set checkbox in column for these properties
            if (cellValue == true) {
                result = '<input type="checkbox" checked/>';
            }
            else {
                result = '<input type="checkbox"/>';
            }
            break;
        case 'DefaultBoolValueForRadio':
            result = '<select style="width:100%" class="form-control"><option value="1">Yes</option><option value="2">No</option><option value="3">None</option></select>';
            break;
        case 'Label':
        case 'HelpText':
        case 'DefaultValue':
        case 'MDMName':
        case 'OptionLabel':
        case 'OptionLabelNo':
        case 'Regex':
            //set textarea in column for these properties
            if (cellValue === null || cellValue === undefined) {
                cellValue = '';
            }
            //set penicil icon for custom regex
            if (rowObject.Property == "Custom Regex") {
                return '<div><span class="ui-icon ui-icon-pencil" title = "Manage ' + rowObject.Property + '" style = "cursor: pointer;"></span></div>';
            }
            else {
                result = '<textarea style="width:100%;" >' + cellValue + '</textarea>';
            }
            break;
        case 'MaxLength':
            //set input type=text element
            if (cellValue === null) {
                cellValue = '';
            }
            result = '<input style="width:100%" value="' + cellValue + '" class="form-control"/>';
            break;
        case 'DefaultDate':
        case 'MaxDate':
        case 'MinDate':
            //set date format and input element for date values
            if (cellValue !== undefined && cellValue !== null) {
                var convertedDate = $.jgrid.parseDate('Y-m-d', cellValue, 'n/j/Y');
                result = '<input style="width:100%" type="text" value="' + convertedDate + '" class="form-control"/>';
            }
            else {
                result = '<input style="width:100%" type="text" class="form-control" />';;
            }
            break;
        case 'Sections':
        case 'Rules':
        case 'Validation':
        case 'Fields':
        case 'AdvancedConfiguration':
        case 'RepeaterTemplates':
        case 'ExpressionRules':
        case 'Items':
        case 'UseDataSource':
            //case 'CustomRule':
        case 'UseInModule':
            //set edit icon which will be used to load dialogs
            return '<div><span class="ui-icon ui-icon-pencil" title = "Manage ' + rowObject.Property + '" style = "cursor: pointer;"></span></div>';
            break;

        case 'IsDataSource':
            //set checkbox in column for these properties
            if (cellValue == true) {
                result = '<input type="checkbox" checked/>';
            }
            else {
                result = '<input type="checkbox"/>';
            }
            break;

        case 'IsDataRequired':
            if (cellValue == true) {
                result = '<input type="checkbox" checked/>';
            }
            else {
                result = '<input type="checkbox"/>';
            }
            break;

        case 'DataSourceName':
            //set input type=text element
            if (cellValue === null) {
                cellValue = '';
            }
            result = '<input style="width:100%" value="' + cellValue + '" class="form-control"/>';
            break;

        case 'DataSourceDescription':
            ///set textarea in column for these properties
            if (cellValue === null) {
                cellValue = '';
            }
            result = '<textarea style="width:100%;" >' + cellValue + '</textarea>';
            break;
        case 'RoleAccessPermission':
            //set edit icon which will be used to load dialogs
            return '<div><span class="ui-icon ui-icon-pencil" title = "Manage ' + rowObject.Property + '" style = "cursor: pointer;"></span></div>';
            break;
        case 'IsDropDownTextBox':
            //set checkbox in column for these properties
            if (cellValue == true) {
                result = '<input type="checkbox" checked/>';
            }
            else {
                result = '<input type="checkbox"/>';
            }
            break;
        case 'DuplicationCheck':
            return '<div><span class="ui-icon ui-icon-pencil" title = "Manage ' + rowObject.Property + '" style = "cursor: pointer;"></span></div>';
            break;

        case 'LoadFromServer':
            //set checkbox in column for these properties
            if (cellValue == true) {
                result = '<input type="checkbox" checked/>';
            }
            else {
                result = '<input type="checkbox"/>';
            }
            break;
        case 'CustomHtml':
            ///set textarea in column for these properties
            if (cellValue === null) {
                cellValue = '';
            }
            result = '<textarea style="width:100%;" >' + cellValue + '</textarea>';
            break;
        case 'IsSortRequired':
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
uiElementPropertyGrid.prototype.unFormatColumn = function (cellValue, options) {
    var result;
    switch (options.rowId) {
        case 'UIElementDataTypeID':
        case 'LayoutTypeID':
        case 'LibraryRegexID':
        case 'ViewType':
            //extract value from the drop down
            result = $(this).find('#' + options.rowId).find('select').val();
            break;
        case 'Enabled':
        case 'HasCustomRule':
        case 'AllowGlobalUpdates':
        case 'JSONElement':
        case 'IsLabel':
        case 'AllowBulkUpdate':
        case 'IsYesNo':
        case 'IsMultiLine':
        case 'IsRequired':
        case 'SpellCheck':
        case 'Visible':
        case 'DefaultBoolValue':
        case 'IsLibraryRegex':
        case 'IsDataSource':
        case 'IsDropDownFilterable':
        case 'IsMultiSelect':
        case 'IsStandard':
            //extract value from the checkbox
            result = $(this).find('#' + options.rowId).find('input').prop('checked');
            break;
        case 'DefaultBoolValueForRadio':
            //extract value from the drop down
            result = $(this).find('#' + options.rowId).find('select').val();
            break;
        case 'IsDataRequired':
            //extract value from the checkbox
            result = $(this).find('#' + options.rowId).find('input').prop('checked');
            break;
        case 'Label':
        case 'HelpText':
        case 'DefaultValue':
        case 'OptionLabel':
        case 'OptionLabelNo':
        case 'Regex':
        case 'MDMName':
            //extract value from textarea
            result = $(this).find('#' + options.rowId).find('textarea').val();
            break;
        case 'MaxLength':
        case 'DefaultDate':
        case 'MaxDate':
        case 'MinDate':
        case 'DataSourceName':
            //extract value from the input type=text element
            result = $(this).find('#' + options.rowId).find('input').val();
            break;
        case 'DataSourceDescription':
            //extract value from the input type=text element
            //extract value from textarea
            result = $(this).find('#' + options.rowId).find('textarea').val();
            break;
        case 'IsDropDownTextBox':
            //extract value from the checkbox
            result = $(this).find('#' + options.rowId).find('input').prop('checked');
            break;
        case 'LoadFromServer':
            //extract value from the checkbox
            result = $(this).find('#' + options.rowId).find('input').prop('checked');
            break;
        case 'CustomHtml':
            //extract value from the input type=text element
            //extract value from textarea
            result = $(this).find('#' + options.rowId).find('textarea').val();
            break;
        case 'IsSortRequired':
            result = $(this).find('#' + options.rowId).find('input').prop('checked');
            break;
        default:
            result = '';
            break;
    }
    return result;
}

//handler for ajax errors
uiElementPropertyGrid.prototype.showError = function (xhr) {
    if (xhr.status == 999)
        window.location.href = '/Account/LogOn';
    else
        alert(JSON.stringify(xhr));
}

uiElementPropertyGrid.prototype.destroy = function () {
    this.dropDownItemsDialog = null;
    this.rulesDialog = null;
    this.dataSourceDialog = null;
    this.customRulesDialog = null;
    this.advanceConfigurationDialog = null;
    this.repeaterTemplateDialog = null;
    this.customRule = null;
    this.sectionListDialog = null;
    $(this.gridElementId).jqGrid('GridUnload');
    this.duplicationCheck = null;
    this.duplicationCheckDialog = null
}

uiElementPropertyGrid.prototype.showGrid = function () {
    $(this.gridContainerElementId).show();
}

uiElementPropertyGrid.prototype.hideGrid = function () {
    $(this.gridContainerElementId).hide();
}

//return methods which can be used to get master lists
var masterListManager = function () {
    var URLs = {
        libraryRegex: '/MasterList/LibraryRegexList?tenantId={tenantId}'
    }

    return {
        //get library regexes
        getLibraryRegexes: function (tenantId) {
            var url = URLs.libraryRegex.replace(/\{tenantId\}/g, tenantId);
            var promise = ajaxWrapper.getJSONCache(url);
            return promise;
        }
    }

}();
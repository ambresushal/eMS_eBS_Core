function reportConfigurationGrid(uiElement, formDesignId, formDesignVersionId, status, elementGridData, formDesignVersionInstance) {
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
        propertyGridContainerElementContainer: 'fdvuielemdetail{formDesignVersionId}container'
    }
    this.URLs = {
        updateProperties: '/WinwardReport/UpdateReportProperties',
        reportPropertiesDetail: '/WinwardReport/GetProperties?tenantId=1&formDesignVersionId={formDesignVersionId}&uiElementId={uiElementId}',
    }

    //generate dynamic grid element id
    this.gridElementId = '#reportMapping';
    this.gridElementIdNoHash = 'reportMapping';
    this.gridContainerElementId = '#' + this.UIElementIDs.propertyGridContainerElementContainer.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId);
    this.roleAccessPermissionDialog = undefined;    
    //variable for dropDownItemsDialog - required only for Drop Downs
    this.dropDownItemsDialog = undefined;
}

//load property grid
reportConfigurationGrid.prototype.loadPropertyGrid = function () {
    url = this.URLs.reportPropertiesDetail.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId).replace(/\{uiElementId\}/g, this.uiElement.UIElementID);

    if (url !== undefined) {
        var promise = ajaxWrapper.getJSON(url);
        var currentInstance = this;
        promise.done(function (xhr) {
            currentInstance.uiElementDetail = xhr;
            //generate grid data
            var reportProperties = currentInstance.generateGridData(xhr);
            currentInstance.bindToPropertyGrid(reportProperties);
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
                tabUIElementProperties[1].Value = xhr.CustomRule;
                currentInstance.customRule = xhr.CustomRule;
                currentInstance.bindToPropertyGrid(tabUIElementProperties);
            });
            //register callback for ajax request failure
            promise.fail(this.showError);
        }
    }
}


reportConfigurationGrid.prototype.generateGridData = function (xhr) {
    //get properties that need to be displayed for the element based on type
    var uiElementProperties = this.getUIElementProperties(this.uiElement.ElementType);
    //populate the Value for each property of the element with the data received from the ajax call
    for (var index = 0; index < uiElementProperties.length; index++) {
        switch (uiElementProperties[index].IntProperty) {
            case 'ReportDescription':
                uiElementProperties[index].Value = xhr[0].ReportDescription;
                break;
            case 'Visible':
                uiElementProperties[index].Value = xhr[0].Visible;
                break;
            case 'Location':
                uiElementProperties[index].Value = xhr[0].Location;
                break;
            case 'RoleAccessPermission':
                uiElementProperties[index].Value = xhr[0].RoleAccessPermission;                
                break;
            case 'Parameters':
                uiElementProperties[index].Value = xhr[0].Parameters;
                break;
            case 'ReportType':
                uiElementProperties[index].Value = xhr[0].ReportType;
                break;
            case 'HelpText':
                uiElementProperties[index].Value = xhr[0].HelpText;
                break;
            case 'IsRelease':
                uiElementProperties[index].Value = xhr[0].IsRelease;
                break;
            case 'Template':
                uiElementProperties[index].Value = xhr[0].Template;
                break;
        }
    }
    return uiElementProperties;
}

reportConfigurationGrid.prototype.getUIElementProperties = function (elementType) {
    var currentInstance = this;
    var elementProperties = [
             { IntProperty: 'ReportDescription', Property: 'Report Description', Value: ''},
             { IntProperty: 'Visible', Property: 'Visible', Value: ''},
             { IntProperty: 'Location', Property: 'Location', Value: '' },
             { IntProperty: 'RoleAccessPermission', Property: 'Role Access Permissions' },
             { IntProperty: 'Parameters', Property: 'Parameters', Value: '' },
             { IntProperty: 'ReportType', Property: 'Report Type', Value: ''},
             { IntProperty: 'HelpText', Property: 'Help Text', Value: ''},
             { IntProperty: 'IsRelease', Property: 'Is Release', Value: ''},
             { IntProperty: 'Template', Property: 'Template', Value: ''},
    ];
    return elementProperties;
}

//bind data to the element property grid
reportConfigurationGrid.prototype.bindToPropertyGrid = function (uiElementProperties) {
    var URLs = {
        //get Document Design List
        formDesignList: '/FormDesign/FormDesignList?tenantId=1',
        formDesignVersionList: '/FormDesign/FormDesignVersionList?tenantId=1'
    }
    //unload previous grid values
    $('#reportMapping').jqGrid('GridUnload');  // this should be mapping Grid
    //set column list
    var colArray = ['IntProperty', 'Property', 'Value'];
    //set column  models
    var colModel = [];
    colModel.push({ name: 'IntProperty', index: 'IntProperty', key: true, hidden: true, search: false });
    colModel.push({ name: 'Property', index: 'Property', align: 'left', editable: false });
    colModel.push({ name: 'Value', index: 'Value', align: 'center', editable: false, formatter: this.formatColumn, unformat: this.unFormatColumn });

    $(this.gridElementId).parent().append("<div id='p" + this.gridElementIdNoHash + "'></div>");
    
    var currentInstance = this;
   
    $(this.gridElementId).jqGrid({
        datatype: 'local',
        colNames: colArray,
        colModel: colModel,
        autowidth: true,
        caption: currentInstance.uiElement.ReportName,
        pager: '#p' + currentInstance.gridElementIdNoHash,
        hidegrid: false,
        height: '400',
        altRows: true,
        altclass: 'alternate',
        //register event handler for row insert
        afterInsertRow: function (rowId, rowData, rowElem) {
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
                $(grid).find('#CustomRule').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
            }
            if (currentInstance.uiElementDetail != undefined && currentInstance.uiElementDetail.IsLoadFromServerEnabled === false) {
                $(grid).find('#LoadFromServer').find('input:checkbox').attr('disabled', 'disabled');
                $(grid).find('#IsDataSource').find('input:checkbox').attr('disabled', 'disabled');
                $(grid).find('#DataSourceName').find('input').attr('disabled', 'disabled');
                $(grid).find('#DuplicationCheck').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
                $(grid).find('#UseDataSource').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
                $(grid).find('#Rules').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
                $(grid).find('#CustomRule').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
            }

            var LayoutType = currentInstance.uiElementDetail.LayoutTypeID;
            if (!(parseInt(LayoutType) == 7)) {
                $(grid).find('#CustomHtml').hide();//.attr('disabled', 'disabled').addClass("ui-state-disabled");
            }
            else {
                $(grid).find('#CustomHtml').show();
            }
        }
    });

    //insert rows in the grid
    for (var index = 0; index < uiElementProperties.length; index++) {
        $('#reportMapping').jqGrid('addRowData', uiElementProperties[index].IntProperty, uiElementProperties[index]);
    }

    var pagerElement = '#p' + currentInstance.gridElementIdNoHash;
    //remove default buttons
    $(currentInstance.gridElementId).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();
    $(pagerElement + '_left').find("tr").children().last().find('div').css('padding-top', '5px');

    $(currentInstance.gridElementId).jqGrid('navButtonAdd', pagerElement,
    {
        caption: 'Save', id: 'templatePropSaveButton',
        onClickButton: function () {
            //var templatePropertiesList = $(this).getRowData();
            var grid = this;
            var data = currentInstance.readGridData();

            currentInstance.postFormInstanceData(data, currentInstance);

        }
    });    

    //register event handler to load sections add dialog when edi ticon of Sections property is clicked
    //only used for Tab(the root) control of the form
    $(this.gridElementId).find('.ui-icon-document').click(function (e) {
        //var dialog = new sectionListDialog(currentInstance.uiElement, currentInstance.statustext, currentInstance.formDesignId, currentInstance.formDesignVersionId, currentInstance.formDesignVersionInstance);
        var url = '/WinwardReport/DownloadDocument';
        var stringData = 'fileName=' + 'App_Data\\QHP\\workbook.xml';
        $.downloadNew(url, stringData, 'post');        
    });
    //register event handler to display fields dialog when edit icon of fields property is clicked
    $(this.gridElementId).find('#Fields').find('.ui-icon-pencil').click(function () {
        var dialog = new fieldListDialog(currentInstance.uiElement, currentInstance.statustext, currentInstance.formDesignVersionId, currentInstance.formDesignVersionInstance, currentInstance.uiElementDetail);
        dialog.show();
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
            currentInstance.rulesDialog.show();
        }
        else {
            currentInstance.rulesDialog.open();
        }
    });

    //register event handler to show the custom rules dialog when edit icon is clicked on the Custom Rule property
    $(this.gridElementId).find('#CustomRule').find('.ui-icon-pencil').click(function () {
        currentInstance.customRulesDialog = new customRulesDialog(currentInstance.uiElement, currentInstance.formDesignVersionId, currentInstance.statustext, currentInstance.customRule);
        currentInstance.customRulesDialog.show();
    });
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
    });

    $(this.gridElementId).find('#LayoutTypeID').click(function () {
        if (!(currentInstance.uiElement.ElementType == "Section")) {
            $("#LayoutTypeID option[value='7']").remove();
        }
    });

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

    
    if ((currentInstance.formDesignId == FormTypes.MASTERLISTFORMID && currentInstance.uiElementDetail.IsDataSource == true)) {

        $(currentInstance.gridElementId).find('#LoadFromServer').find('input:checkbox').attr('disabled', 'disabled').addClass("ui-state-disabled");
    }
}

reportConfigurationGrid.prototype.allowCustomLayoutForElement = function (currentInstance, data) {
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
reportConfigurationGrid.prototype.postFormInstanceData = function (data, currentInstance) {
    //ajax POST of element properties
    var promise = ajaxWrapper.postJSONCustom(currentInstance.URLs.updateProperties, data);
    promise.done(function (xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            if (xhr.Items.length > 0 && xhr.Items[0].Messages[0] != undefined && xhr.Items[0].Messages[0] != null) {
                if (xhr.Items[0].Messages[1] != undefined && xhr.Items[0].Messages[1] != null) {
                    currentInstance.formDesignVersionInstance.loadUIElementGrid(parseInt(xhr.Items[0].Messages[0]), parseInt(xhr.Items[0].Messages[1]));
                } else {
                    currentInstance.formDesignVersionInstance.loadUIElementGrid(parseInt(xhr.Items[0].Messages[0]));
                }
            }
            else {
                currentInstance.formDesignVersionInstance.loadUIElementGrid(currentInstance.uiElementDetail.ParentUIElementID, currentInstance.uiElementDetail.UIElementID);
            }
            if (currentInstance.uiElement.hasLoadFromServer == "true") {
                $(currentInstance.gridElementId).find('#LoadFromServer').find('input:checkbox').attr('disabled', 'disabled');
                $(currentInstance.gridElementId).find('#IsDataSource').find('input:checkbox').attr('disabled', 'disabled');
                $(currentInstance.gridElementId).find('#DataSourceName').find('input').attr('disabled', 'disabled');
                $(currentInstance.gridElementId).find('#DuplicationCheck').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
                $(currentInstance.gridElementId).find('#UseDataSource').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
                $(currentInstance.gridElementId).find('#Rules').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
                $(currentInstance.gridElementId).find('#CustomRule').find('.ui-icon-pencil').attr('disabled', 'disabled').addClass("ui-state-disabled");
            }
            messageDialog.show(DocumentDesign.saveMsg);
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

reportConfigurationGrid.prototype.hasValidationError = function (grid) {
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

reportConfigurationGrid.prototype.validateMaxLength = function (rowId, control, length) {
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

reportConfigurationGrid.prototype.validateUniqueDataSourceName = function (rowId, control, uiElementID) {
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

reportConfigurationGrid.prototype.validateRequiredCustomHtml = function (rowId, control) {
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

reportConfigurationGrid.prototype.validateRequired = function (rowId, control) {
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

reportConfigurationGrid.prototype.validateMinMaxDate = function (rowId, minDateControl, maxDateControl) {
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
reportConfigurationGrid.prototype.readGridData = function () {
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
                updateElement[uiElementProperties[index].IntProperty] = $(this.gridElementId).getRowData(uiElementProperties[index].IntProperty).Value;
        }
    return updateElement;
}

reportConfigurationGrid.prototype.formatColumn = function (cellValue, options, rowObject) {
    var result;
    switch (rowObject.IntProperty) {
        case 'Location':
            result = '<select style="width:100%" class="form-control"><option value="InFolder">In Folder</option><option value="InMenu">In Menu</option></select>';
            break;
        case 'ReportType':
            result = '<select style="width:100%" class="form-control"><option value="Folder">Folder</option><option value="Account">Account</option></select>';
            break;               
        case 'Visible':            
        case 'IsLibraryRegex':
        case 'IsRelease':            
            if (cellValue == true) {
                result = '<input type="checkbox" checked/>';
            }
            else {
                result = '<input type="checkbox"/>';
            }
            break;
        case 'RoleAccessPermission':
        case 'Parameters':                
            if (cellValue === null || cellValue === undefined) {
                cellValue = '';
            }           
            result = '<div style="float:right;width:55%"><span class="ui-icon ui-icon-pencil" title = "Manage ' + rowObject.Property + '" style = "cursor: pointer;"></span></div>';
            break;
        case 'ReportDescription':
        case 'HelpText':
            result = '<textarea style="width:100%;" >' + cellValue + '</textarea>';
            break;
        case 'Template':                
            if (cellValue === null || cellValue === undefined) {
                cellValue = '';
            }
            return '<div style="float:right;width:55%"><span class="ui-icon ui-icon-document view" title = "Download ' + rowObject.Property + '" style = "cursor: pointer;"></span></div>';
    }
    return result;
}

//unformat the grid column based on element property
//used in unformat in colModel for the Value column : bindToPropertyGrid method
reportConfigurationGrid.prototype.unFormatColumn = function (cellValue, options) {
    var result;
    switch (options.rowId) {
        case 'ReportDescription':
        case 'HelpText':
            result = $(this).find('#' + options.rowId).find('textarea').val();
            break;
        case 'Location': 
        case 'ReportType':
            //extract value from the drop down
            result = $(this).find('#' + options.rowId).find('select').val();
            break;
        case 'DataSourceName':
            result = $(this).find('#' + options.rowId).find('input').val();
            break;
        case 'Visible':
        case 'IsRelease':            
            result = $(this).find('#' + options.rowId).find('input').prop('checked');
            break;
        default:
            result = '';
            break;
    }
    return result;
}

//handler for ajax errors
reportConfigurationGrid.prototype.showError = function (xhr) {
    if (xhr.status == 999)
        window.location.href = '/Account/LogOn';
    else
        alert(JSON.stringify(xhr));
}

reportConfigurationGrid.prototype.destroy = function () {
    this.dropDownItemsDialog = null;
    this.rulesDialog = null;
    this.dataSourceDialog = null;
    this.customRulesDialog = null;
    this.customRule = null;
    this.sectionListDialog = null;
    $(this.gridElementId).jqGrid('GridUnload');
    this.duplicationCheck = null;
    this.duplicationCheckDialog = null
}

reportConfigurationGrid.prototype.showGrid = function () {
    $(this.gridContainerElementId).show();
}

reportConfigurationGrid.prototype.hideGrid = function () {
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
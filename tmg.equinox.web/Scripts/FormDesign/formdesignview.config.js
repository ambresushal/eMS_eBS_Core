function formDesignView(formDesignVersion, formDesignId, formDesignName, finalizedstatus) {
    this.tenantId = 1;
    this.formDesignVersion = formDesignVersion;
    this.formDesignId = formDesignId;
    this.formDesignName = formDesignName;
    this.status = finalizedstatus;
    this.data = null;
    this.objFormatter = this.formatter();
    this.objValidator = this.validator();
    this.elementTypes = ['Radio Button', 'Checkbox', '[Blank]','Textbox', 'Multiline TextBox', 'Dropdown List', 'Dropdown TextBox', 'Calendar', 'Repeater', 'Section', 'Label', 'Rich TextBox'];
    this.dataTypes = ['', 'Integer', 'String', 'Float', 'NA'];
    this.formats = ['', 'Date', 'Zip Code', 'Phone', 'Url', 'Email', 'Decimal', 'Comma Separated', 'Social Security Number', 'Hexa Decimal', 'Numeric With 2 Precision', 'Numeric With 2 Precision Or Percent', 'Numeric Only', 'Alphabets Only', 'Time Format'];
    this.secLayout = ['', 'Three-Column Layout', 'Two-Column Layout', 'Single-Column Layout', 'One - Column - Big - Label', 'Custom-Layout'];
    this.repLayout = ['', 'Grid-Layout', 'table-Layout', 'Custom-Layout'];
    this.viewTypes = ['Folder View', 'SOT View', 'Both'];
    this.hotInstance;
    this.currentRowId;
    this.dropDownItemsDialog = undefined;
    this.rulesDialog = undefined;
    this.dataSourceDialog = undefined;
    this.configraionDialog = undefined;
    this.expressionRulesDialog = undefined;
    this.radioOptionDialog = undefined;
    this.columnChooserDialog = undefined;
    this.excelConfigurationDialog = undefined;
    this.roleAccessPermissionDialog = undefined;
    this.colModels = [];
    this.colNames = [];
    this.extendedProperties = [];
    this.hiddenColumns = [1, 2, 8, 9, 13, 24, 25, 26, 27];
    this.tabElement;
    this.designRulesTesterData = [];

    this.elementIDs = {
        viewGrid: 'divfdvuielems' + this.formDesignVersion.FormDesignVersionId,
        viewGridJQ: '#divfdvuielems' + this.formDesignVersion.FormDesignVersionId,
        buttonHolder: 'actionContainer' + this.formDesignVersion.FormDesignVersionId,
        buttonHolderJQ: '#actionContainer' + this.formDesignVersion.FormDesignVersionId,
        dropdownItemsDialogJQ: "#dropdownitemsdialog",
        rulesDialogJQ: "#rulesdialog",
        dataSourceDialogJQ: '#dataSourcedialog',
        configraionDialogJQ: '#advancedConfigraionDialog',
        radioOptionDialogJQ: '#radioButtonOptiondialog',
        roleAccessPermissionDialog: '#roleaccesspermission',
    }

    this.URLs = {
        getElementList: '/UIElement/GetUIElementList?tenantId=1&formDesignVersionId={formDesignVersionId}',
        saveElementList: '/UIElement/SaveFormDesignVersion',
        formDesignVersionList: '/FormDesign/FormDesignVersionList?tenantId=1&formDesignId={formDesignId}',
        formDesignUIElementList: '/UIElement/FormDesignVersionUIElementList?tenantId=1&formDesignVersionId={formDesignVersionId}',
        formDesignUIElementDelete: '/UIElement/DeleteElement?tenantId=1&formDesignVersionId={formDesignVersionId}&elementType={elementType}&uiElementId={uiElementId}',
        formDesignVersionPreview: '/FormDesignPreview/Preview?tenantId=1&formDesignVersionId={formDesignVersionId}&formName={formName}',
        CompileFormDesignVersion: '/FormDesign/CompileFormDesignVersion',
        CompileDocumentRule: '/FormDesign/CompileFormDesignVersionRule',
        downloadViewUrl: '/UIElement/DownloadConfigView',
        CompileImpactedField: '/FormDesign/CompileImpactedField',
        saveConfigRulesTesterData: '/UIElement/SaveConfigRulesTesterData'
    }
}

formDesignView.prototype.init = function () {
    var currentInstance = this;
    currentInstance.getData();
    $(currentInstance.elementIDs.viewGridJQ).parent().show();
    $(currentInstance.elementIDs.buttonHolderJQ).show();

    $(currentInstance.elementIDs.dropdownItemsDialogJQ).on("dialogclose", function (event, ui) {
        var items;
        if (currentInstance.dropDownItemsDialog != null && currentInstance.dropDownItemsDialog != undefined) {
            items = currentInstance.dropDownItemsDialog.getDialogData();
            currentInstance.dropDownItemsDialog = null;
            var selected = currentInstance.hotInstance.getSourceDataAtRow(currentInstance.currentRowId);
            selected["Items"] = items;
        }
    });

    $(currentInstance.elementIDs.rulesDialogJQ).on("dialogopen", function (event, ui) {
        if (currentInstance.rulesDialog != null || currentInstance.rulesDialog != undefined) {
            currentInstance.rulesDialog.setRuleTesterData(currentInstance.designRulesTesterData);
        }
    });

    $(currentInstance.elementIDs.rulesDialogJQ).on("dialogclose", function (event, ui) {
        var rules;
        var isRuleModified = false;
        if (currentInstance.rulesDialog != null && currentInstance.rulesDialog != undefined) {
            rules = currentInstance.rulesDialog.getRulesData();
            currentInstance.designRulesTesterData = currentInstance.rulesDialog.getRuleTesterData();
            isRuleModified = currentInstance.rulesDialog.isUpdated();
            currentInstance.rulesDialog = null;
            var selected = currentInstance.hotInstance.getSourceDataAtRow(currentInstance.currentRowId);
            selected["Rules"] = rules;
            selected["AreRulesModified"] = isRuleModified;
        }
    });

    $(currentInstance.elementIDs.dataSourceDialogJQ).on("dialogclose", function (event, ui) {
        if (currentInstance.dataSourceDialog != null && currentInstance.dataSourceDialog != undefined) {
            currentInstance.dataSourceDialog = null;
        }
    });

    $(currentInstance.elementIDs.configraionDialogJQ).on("dialogclose", function (event, ui) {
        var config;
        if (currentInstance.configraionDialog != null && currentInstance.configraionDialog != undefined) {
            config = currentInstance.configraionDialog.getConfigurationData()[0]
            currentInstance.configraionDialog = null;
            var selected = currentInstance.hotInstance.getSourceDataAtRow(currentInstance.currentRowId);
            selected["AdvancedConfiguration"] = config;
        }
    });

    $(currentInstance.elementIDs.radioOptionDialogJQ).on("dialogclose", function (event, ui) {
        var options;
        if (currentInstance.radioOptionDialog != null && currentInstance.radioOptionDialog != undefined) {
            options = currentInstance.radioOptionDialog.getOptions();
            currentInstance.radioOptionDialog = null;
            var selected = currentInstance.hotInstance.getSourceDataAtRow(currentInstance.currentRowId);
            selected["OptionYes"] = options.OptionYes;
            selected["OptionNo"] = options.OptionNo;
        }
    });

    $(currentInstance.elementIDs.roleAccessPermissionDialog).on("dialogclose", function (event, ui) {
        if (currentInstance.roleAccessPermissionDialog != null && currentInstance.roleAccessPermissionDialog != undefined) {
            currentInstance.roleAccessPermissionDialog = null;
        }
    });
}

formDesignView.prototype.destroy = function () {
    var currentInstance = this;
    currentInstance.hotInstance.destroy();
    $(currentInstance.elementIDs.viewGridJQ).parent().hide();
    currentInstance.dropDownItemsDialog = undefined;
    currentInstance.rulesDialog = undefined;
    currentInstance.dataSourceDialog = undefined;
    currentInstance.configraionDialog = undefined;
    currentInstance.expressionRulesDialog = undefined;
    currentInstance.radioOptionDialog = undefined;
    currentInstance.designRulesTesterData = undefined;
}

formDesignView.prototype.getData = function () {
    var currentInstance = this;
    var url = currentInstance.URLs.getElementList.replace(/\{tenantId\}/g, currentInstance.tenantId).replace(/\{formDesignVersionId\}/g, currentInstance.formDesignVersion.FormDesignVersionId);
    var promise = ajaxWrapper.getJSON(url);

    promise.done(function (result) {
        currentInstance.data = JSON.parse(JSON.stringify(result));
        currentInstance.data.sections = [];
        $.each(result.SectionElements, function (key, value) {
            if (key == 0) { currentInstance.tabElement = value.DisplayText; }
            currentInstance.data.sections.push(value.DisplayText);
        });

        $.each(result.ExtendedProperties, function (key, val) {
            currentInstance.extendedProperties.push({ Header: val.Header, Name: val.Name });
        });

        currentInstance.buildGrid(result.UIElementList, result.Comments);
    });

    promise.fail(currentInstance.showError);
}

formDesignView.prototype.getColNames = function () {
    var currentInstance = this;
    currentInstance.colNames = [];
    currentInstance.colNames.push('Label');
    currentInstance.colNames.push('UIElementID');
    currentInstance.colNames.push('UIElementName');
    currentInstance.colNames.push('Element Type');
    currentInstance.colNames.push('Layout');
    currentInstance.colNames.push('Items');
    currentInstance.colNames.push('Multiselect');
    currentInstance.colNames.push('Data Type');
    currentInstance.colNames.push('ApplicationDataTypeID');
    currentInstance.colNames.push('Sequence');
    currentInstance.colNames.push('Max Length');
    currentInstance.colNames.push('Required');
    currentInstance.colNames.push('Formats');
    currentInstance.colNames.push('LibraryRegexID');
    currentInstance.colNames.push('Parent');
    currentInstance.colNames.push('Rules');
    currentInstance.colNames.push('Exp Rules');
    currentInstance.colNames.push('Source');
    currentInstance.colNames.push('Adv Config');
    currentInstance.colNames.push('Role Access');
    currentInstance.colNames.push('Visible');
    currentInstance.colNames.push('Enable');
    currentInstance.colNames.push('View Type');
    currentInstance.colNames.push('Help Text');
    currentInstance.colNames.push('ParentUIElementId');
    currentInstance.colNames.push('IsContainer');
    currentInstance.colNames.push('TempUIElementId');
    currentInstance.colNames.push('SourceUIElementId');
    currentInstance.colNames.push('Is Key');
    currentInstance.colNames.push('Default Value');
    currentInstance.colNames.push('Allow Bulk Update');
    currentInstance.colNames.push('Custom HTML');

    $.each(currentInstance.extendedProperties, function (i, j) {
        currentInstance.colNames.push(j.Header);
    });

    return currentInstance.colNames;
}

formDesignView.prototype.getColModel = function () {
    var currentInstance = this;
    currentInstance.colModels = [];
    var column = { data: 'Label', validator: currentInstance.objValidator.emptyValidator, width: 300 };
    currentInstance.colModels.push(column);
    column = { data: 'UIElementID', width: 50 };
    currentInstance.colModels.push(column);
    column = { data: 'UIElementName', width: 50 };
    currentInstance.colModels.push(column);
    column = { data: 'ElementType', type: 'dropdown', source: currentInstance.elementTypes, strict: true, allowEmpty: false, width: 120 };
    currentInstance.colModels.push(column);
    column = { data: 'Layout', type: 'dropdown', source: currentInstance.secLayout, width: 140 };
    currentInstance.colModels.push(column);
    column = { data: 'HasOptions', renderer: currentInstance.objFormatter.optionEditRenderer, className: "htCenter htMiddle", width: 60, readOnly: true, };
    currentInstance.colModels.push(column);
    column = { data: 'IsMultiselect', type: 'checkbox', className: "htCenter htMiddle", width: 80 };
    currentInstance.colModels.push(column);
    column = { data: 'DataType', type: 'dropdown', source: currentInstance.dataTypes, width: 80 };
    currentInstance.colModels.push(column);
    column = { data: 'ApplicationDataTypeID', width: 100 };
    currentInstance.colModels.push(column);
    column = { data: 'Sequence', width: 100 };
    currentInstance.colModels.push(column);
    column = { data: 'MaxLength', type: 'numeric', width: 85 };
    currentInstance.colModels.push(column);
    column = { data: 'Required', type: 'checkbox', className: "htCenter htMiddle", width: 70 };
    currentInstance.colModels.push(column);
    column = { data: 'Formats', type: 'dropdown', source: currentInstance.formats, width: 100, className: "wordWrap" };
    currentInstance.colModels.push(column);
    column = { data: 'LibraryRegexID', type: 'numeric', width: 100 };
    currentInstance.colModels.push(column);
    column = { data: 'Parent', type: 'dropdown', source: currentInstance.data.sections, className: "wordWrap", strict: true, allowEmpty: false, width: 250 };
    currentInstance.colModels.push(column);
    column = { data: 'HasRules', renderer: currentInstance.objFormatter.ruleEditRenderer, className: "htCenter htMiddle", width: 60, readOnly: true, };
    currentInstance.colModels.push(column);
    column = { data: 'HasExpRules', renderer: currentInstance.objFormatter.ruleEditRenderer, className: "htCenter htMiddle", width: 80, readOnly: true, };
    currentInstance.colModels.push(column);
    column = { data: 'DataSource', renderer: currentInstance.objFormatter.dataSourceRenderer, className: "htCenter htMiddle", width: 60, readOnly: true, };
    currentInstance.colModels.push(column);
    column = { data: 'HasConfig', renderer: currentInstance.objFormatter.configRenderer, className: "htCenter htMiddle", width: 80, readOnly: true, };
    currentInstance.colModels.push(column);
    column = { data: 'HasRoleAccess', renderer: currentInstance.objFormatter.roleAccessRenderer, className: "htCenter htMiddle", width: 90, readOnly: true, };
    currentInstance.colModels.push(column);
    column = { data: 'IsVisible', type: 'checkbox', className: "htCenter htMiddle", width: 60 };
    currentInstance.colModels.push(column);
    column = { data: 'IsEnable', type: 'checkbox', className: "htCenter htMiddle", width: 60 };
    currentInstance.colModels.push(column);
    column = { data: 'ViewType', width: 100, type: 'dropdown', source: currentInstance.viewTypes, strict: true, allowEmpty: false };
    currentInstance.colModels.push(column)
    column = { data: 'HelpText', width: 250, className: "wordWrap" };
    currentInstance.colModels.push(column);
    column = { data: 'ParentUIElementId', width: 20 };
    currentInstance.colModels.push(column);
    column = { data: 'IsContainer', type: 'checkbox', className: "htCenter htMiddle", width: 20 };
    currentInstance.colModels.push(column);
    column = { data: 'TempUIElementId', width: 20 };
    currentInstance.colModels.push(column);
    column = { data: 'SourceUIElementId', width: 20 };
    currentInstance.colModels.push(column);
    column = { data: 'IsKey', type: 'checkbox', className: "htCenter htMiddle", width: 70 };
    currentInstance.colModels.push(column);
    column = { data: 'DefaultValue', width: 100, className: "wordWrap" };
    currentInstance.colModels.push(column);
    column = { data: 'AllowBulkUpdate', type: 'checkbox', className: "htCenter htMiddle", width: 120 };
    currentInstance.colModels.push(column);
    column = { data: 'CustomHtml', width: 150, className: "wordWrap" };
    currentInstance.colModels.push(column);

    $.each(currentInstance.extendedProperties, function (i, j) {
        currentInstance.colModels.push({ data: 'ExtProp.' + j.Name, width: 200 });
    });

    return currentInstance.colModels;
}

formDesignView.prototype.formatter = function () {
    var currentInstance = this;
    return {
        ruleEditRenderer: function (instance, td, row, col, prop, value, cellProperties) {
            var btn = document.createElement('span');
            btn.className = 'ui-icon ui-icon-pencil';
            btn.style.cursor = 'pointer';
            btn.onclick = function () {
                var uiElement = instance.getSourceDataAtRow(row);
                if (prop == 'HasRules') {
                    if (currentInstance.rulesDialog == undefined) {
                        currentInstance.rulesDialog = new rulesDialog(uiElement, currentInstance.formDesignVersion.FormDesignVersionId, currentInstance.status, currentInstance.data.UIElementList);
                        currentInstance.rulesDialog.show();
                    }
                    else {
                        currentInstance.rulesDialog.open();
                    }
                }
                else {
                    if (uiElement.UIElementID) {
                        if (currentInstance.expressionRulesDialog === undefined || currentInstance.expressionRulesDialog == null) {
                            //currentInstance.expressionRulesDialog = new expressionRulesDialog(uiElement, currentInstance.formDesignId, currentInstance.formDesignVersion.FormDesignVersionId, currentInstance.status);
                            //currentInstance.expressionRulesDialog.show(false);
                            currentInstance.expressionRulesDialog = new expressionRulesDialogNew(uiElement, currentInstance.formDesignId, currentInstance.formDesignVersion.FormDesignVersionId, currentInstance.formDesignVersionInstance.formDesignVersion.EffectiveDate);
                            currentInstance.expressionRulesDialog.show(false);

                        }
                        else {
                            currentInstance.expressionRulesDialog.show(true);
                        }
                    } else {
                        messageDialog.show('Please save to set expression rules for a field.');
                    }
                }
            }
            Handsontable.dom.empty(td);
            td.setAttribute("style", "padding-left: 20px");
            if (value != null && value.indexOf('1') == 1) {
                td.className = 'cont-hilighter'
            }
            td.appendChild(btn);
            return td;
        },
        dataSourceRenderer: function (instance, td, row, col, prop, value, cellProperties) {
            if (value != null && value.indexOf('1') == 0) {
                if (value.charAt(1) == '1') {
                    td.className = 'cont-hilighter'
                }
                var btn = document.createElement('span');
                btn.className = 'ui-icon ui-icon-pencil';
                btn.style.cursor = 'pointer';
                btn.onclick = function () {
                    var uiElement = instance.getSourceDataAtRow(row);
                    if (uiElement.UIElementID) {
                        currentInstance.dataSourceDialog = new dataSourceDialog(uiElement, currentInstance.formDesignId, currentInstance.formDesignVersion.FormDesignVersionId, currentInstance.status, currentInstance.data.UIElementList, undefined);
                        currentInstance.dataSourceDialog.show();
                    } else {
                        messageDialog.show('Please save to use data source for a field.');
                    }
                }
                Handsontable.dom.empty(td);
                td.setAttribute("style", "padding-left: 20px");
                td.appendChild(btn);
            } else {
                if (value != null && value.indexOf('1') == 1) {
                    td.className = 'cont-hilighter'
                }
                Handsontable.dom.empty(td);
            }
            return td;
        },
        optionEditRenderer: function (instance, td, row, col, prop, value, cellProperties) {
            if (value != null && value.indexOf('1') == 0 || value != null && value.indexOf('2') == 0) {
                var btn = document.createElement('span');
                btn.className = 'ui-icon ui-icon-pencil';
                btn.style.cursor = 'pointer';
                btn.onclick = function () {
                    if (value.indexOf('1') == 0) {
                        if (currentInstance.dropDownItemsDialog == undefined) {
                            var uiElement = instance.getSourceDataAtRow(row);
                            currentInstance.dropDownItemsDialog = new dropdownItemsDialog(uiElement, currentInstance.status, currentInstance.formDesignVersion.FormDesignVersionId);
                            currentInstance.dropDownItemsDialog.show(false);
                        }
                        else {
                            currentInstance.dropDownItemsDialog.show(true);
                        }
                    }
                    else {
                        if (currentInstance.radioOptionDialog == undefined) {
                            var uiElement = instance.getSourceDataAtRow(row);
                            currentInstance.radioOptionDialog = new radioButtonOptionDialog(uiElement, currentInstance.status, currentInstance.formDesignVersion.FormDesignVersionId);
                            currentInstance.radioOptionDialog.show(false);
                        }
                        else {
                            currentInstance.radioOptionDialog.show(true);
                        }
                    }
                }
                Handsontable.dom.empty(td);
                td.setAttribute("style", "padding-left: 20px");
                td.appendChild(btn);
            }
            else {
                if (value != null && value.indexOf('1') == 1) {
                    td.className = 'cont-hilighter'
                }
                td.readOnly = true;
                Handsontable.dom.empty(td);
            }
            return td;
        },
        configRenderer: function (instance, td, row, col, prop, value, cellProperties) {
            if (value != null && value.indexOf('1') == 0) {
                var btn = document.createElement('span');
                btn.className = 'ui-icon ui-icon-pencil';
                btn.style.cursor = 'pointer';
                btn.onclick = function () {
                    if (currentInstance.configraionDialog === undefined || currentInstance.configraionDialog == null) {
                        var uiElement = instance.getSourceDataAtRow(row);
                        if (uiElement.UIElementID == undefined || uiElement.UIElementID == null) {
                            uiElement.AdvancedConfiguration = currentInstance.getAdvancedConfigDefault();
                        }
                        currentInstance.configraionDialog = new advancedConfigurationDialog(uiElement, currentInstance.status, currentInstance.formDesignVersion.FormDesignVersionId, undefined, uiElement.AdvancedConfiguration);
                        currentInstance.configraionDialog.show(false);
                    }
                    else {
                        currentInstance.configraionDialog.show(true);
                    }
                }
                Handsontable.dom.empty(td);
                td.className = 'cont-hilighter'
                td.setAttribute("style", "padding-left: 20px");
                td.appendChild(btn);
            } else {
                if (value != null && value.indexOf('1') == 1) {
                    td.className = 'cont-hilighter'
                }
                td.readOnly = true;
                Handsontable.dom.empty(td);
            }
            return td;
        },
        roleAccessRenderer: function (instance, td, row, col, prop, value, cellProperties) {
            if (value != null && value.indexOf('1') == 0) {
                var btn = document.createElement('span');
                btn.className = 'ui-icon ui-icon-pencil';
                btn.style.cursor = 'pointer';
                btn.onclick = function () {
                    var uiElement = instance.getSourceDataAtRow(row);
                    if (uiElement.UIElementID) {
                        if (currentInstance.roleAccessPermissionDialog === undefined || currentInstance.roleAccessPermissionDialog == null) {
                            currentInstance.roleAccessPermissionDialog = new roleAccessPermissionDialog(uiElement, currentInstance.formDesignVersion.FormDesignVersionId);
                            currentInstance.roleAccessPermissionDialog.show(currentInstance.status);
                        }
                        else {
                            currentInstance.roleAccessPermissionDialog.show(currentInstance.status);
                        }
                    } else {
                        messageDialog.show('Please save to set Role Access Permission for this section.');
                    }
                }
                Handsontable.dom.empty(td);
                td.className = 'cont-hilighter'
                td.setAttribute("style", "padding-left: 20px");
                td.appendChild(btn);
            } else {
                if (value != null && value.indexOf('1') == 1) {
                    td.className = 'cont-hilighter'
                }
                td.readOnly = true;
                Handsontable.dom.empty(td);
            }
            return td;
        },
    }
}

formDesignView.prototype.getAdvancedConfigDefault = function () {
    var AdvancedConfiguration = {};
    AdvancedConfiguration.DisplayTopHeader = false;
    AdvancedConfiguration.DisplayTitle = false;
    AdvancedConfiguration.FrozenColCount = 0;
    AdvancedConfiguration.FrozenRowCount = 0;
    AdvancedConfiguration.AllowPaging = true;
    AdvancedConfiguration.RowsPerPage = 20;
    AdvancedConfiguration.AllowExportToExcel = false;
    AdvancedConfiguration.AllowExportToCSV = false;
    AdvancedConfiguration.FilterMode = 'Both';

    return AdvancedConfiguration;
}

formDesignView.prototype.validator = function () {
    var currentInstance = this;
    return {
        emptyValidator: function (value, callback) {
            if (!value || String(value).length === 0) {
                callback(false);
            } else {
                callback(true);
            }
        }
    }
}

formDesignView.prototype.buildGrid = function (elements, comments) {
    var rowsCount = elements.length;
    var currentInstance = this;
    var container = document.getElementById(currentInstance.elementIDs.viewGrid);

    currentInstance.hotInstance = new Handsontable(container, {
        data: elements,
        height: 450,
        allowInsertRow: true,
        allowRemoveRow: true,
        rowHeaders: true,
        colHeaders: currentInstance.getColNames(),
        columns: currentInstance.getColModel(),
        columnSorting: false,
        filters: true,
        dropdownMenu: true,
        licenseKey: '7ae9f-13b20-0c915-e492b-ec649',
        contextMenu: {
            callback: function (key, options) {
                if (key === 'Add_Column') {
                    currentInstance.addColumn();
                }
                else if (key === 'column_chooser') {
                    if (currentInstance.columnChooserDialog) {
                        currentInstance.columnChooserDialog.show(true);
                    }
                    else {
                        currentInstance.columnChooserDialog = new columnChooser(currentInstance);
                        currentInstance.columnChooserDialog.show(false);
                    }
                }
                else if (key === 'excel_settings') {
                    if (currentInstance.excelConfigurationDialog) {
                        currentInstance.excelConfigurationDialog.show(true);
                    }
                    else {
                        currentInstance.excelConfigurationDialog = new excelConfiguration(currentInstance);
                        currentInstance.excelConfigurationDialog.show(false);
                    }
                }
            },
            items: currentInstance.addContextMenu()
        },
        comments: true,
        autoRowSize: true,
        manualColumnMove: false,
        manualColumnResize: true,
        manualRowMove: true,
        autoWrapCol: true,
        autoWrapRow: true,
        minSpareRows: 1,
        fixedColumnsLeft: 4,
        hiddenColumns: {
            columns: currentInstance.hiddenColumns,
            indicators: false
        },
        beforeChange: function (data, source) {
            if (data != null) {
                var row = data[0][0];
                var col = data[0][1];
                var value = data[0][3];

                if (row == 0) {
                    var cellProperties = this.getCellMeta(row, currentInstance.colNames.indexOf('Element Type'));
                    cellProperties.source = ['Section'];
                }

                if (col == 'ElementType') { //enable dropdown options

                    var currentRow = this.getDataAtRow(row);
                    if (currentRow[currentInstance.colNames.indexOf('UIElementID')]) {
                        messageDialog.show('Element type cannot be changed as it may cause data loss. Please remove and add new element.');
                        return false;
                    }

                    if (value == 'Dropdown List' || value == 'Dropdown TextBox' || value == 'Radio Button') {
                        if (value == 'Radio Button') {
                            this.setDataAtRowProp(row, "HasOptions", '20');
                        } else {
                            this.setDataAtRowProp(row, "HasOptions", '10');
                        }
                        this.setDataAtRowProp(row, "DataSource", '10');
                    }

                    if (value == 'Section' || value == 'Repeater') {
                        this.setDataAtRowProp(row, "IsContainer", true);
                        this.setDataAtRowProp(row, "DataSource", '11');
                        if (value == 'Repeater') {
                            this.setDataAtRowProp(row, "HasConfig", '11');
                            this.setDataAtRowProp(row, "Layout", 'Grid-Layout');
                            this.setDataAtRowProp(row, "HasRoleAccess", '01');
                        }

                        if (value == 'Section') {
                            this.setDataAtRowProp(row, "HasConfig", '01');
                            this.setDataAtRowProp(row, "Layout", 'Three-Column Layout');
                            this.setDataAtRowProp(row, "HasRoleAccess", '11');
                        }
                    }
                    else {
                        this.setDataAtRowProp(row, "Layout", '');
                    }

                    var meta = this.getCellMeta(row, currentInstance.colNames.indexOf('Layout'));
                    meta.readOnly = value == 'Section' || value == 'Repeater' ? false : true;
                    meta = this.getCellMeta(row, currentInstance.colNames.indexOf('Multiselect'));
                    meta.readOnly = value == 'Dropdown List' || value == 'Dropdown TextBox' ? false : true;
                    meta = this.getCellMeta(row, currentInstance.colNames.indexOf('Data Type'));
                    meta.readOnly = value == 'Textbox' || value == 'Multiline TextBox' || value == 'Rich TextBox' || value == 'Dropdown List' || value == 'Dropdown TextBox' ? false : true;
                    meta = this.getCellMeta(row, currentInstance.colNames.indexOf('Max Length'));
                    meta.readOnly = value == 'Textbox' || value == 'Multiline TextBox' || value == 'Rich TextBox' ? false : true;
                    meta = this.getCellMeta(row, currentInstance.colNames.indexOf('Formats'));
                    meta.readOnly = value == 'Textbox' || value == 'Multiline TextBox' || value == 'Rich TextBox' || value == 'Dropdown List' || value == 'Dropdown TextBox' ? false : true;
                    meta = this.getCellMeta(row, currentInstance.colNames.indexOf('Is Key'));
                    meta.readOnly = value == 'Repeater' || value == 'Section' ? true : false;
                    meta = this.getCellMeta(row, currentInstance.colNames.indexOf('Allow Bulk Update'));
                    meta.readOnly = value == 'Repeater' ? false : true;
                }

                if (col == 'Parent') { //set Parent Element ID
                    var label = this.getDataAtCell(row, currentInstance.colNames.indexOf('Label'));
                    if (label == value) {
                        messageDialog.show('Cannot select same element as a parent. Please select an appropriate section.');
                        return false;
                    }
                    var elementType = this.getDataAtCell(row, currentInstance.colNames.indexOf('Element Type'));
                    if (elementType != 'Section' && value == currentInstance.tabElement) {
                        messageDialog.show('Other than section element cannot be added to the root. Please add an element under a section.');
                        return false;
                    }

                    var objSection = jQuery.grep(currentInstance.data.SectionElements, function (n, i) {
                        return (n.DisplayText == value);
                    });
                    if (objSection.length > 0) {
                        this.setDataAtRowProp(row, "ParentUIElementId", objSection[0].Value);
                    }

                    var seq = this.getDataAtRow(row - 1);
                    if (seq[currentInstance.colNames.indexOf('Parent')] == value && seq[currentInstance.colNames.indexOf('Sequence')] > 0) {
                        this.setDataAtRowProp(row, "Sequence", seq[currentInstance.colNames.indexOf('Sequence')] + 1);
                    }
                    else {
                        this.setDataAtRowProp(row, "Sequence", 0);
                    }
                }

                if (col == 'DataType') {//set Datatype ID
                    var objSection = jQuery.grep(currentInstance.data.DataTypes, function (n, i) {
                        return (n.DisplayText == value);
                    });
                    if (objSection.length > 0) {
                        this.setDataAtRowProp(row, "ApplicationDataTypeID", objSection[0].Value);
                    }
                }

                if (col == 'Formats') { //set regex library
                    var objSection = jQuery.grep(currentInstance.data.DataFormats, function (n, i) {
                        return (n.DisplayText == value);
                    });
                    if (objSection.length > 0) {
                        this.setDataAtRowProp(row, "LibraryRegexID", objSection[0].Value);
                    }
                }

                if (col == 'Label') { //set sequence of the field
                    if (row >= 0) {
                        this.setDataAtRowProp(row, "IsVisible", true);
                        this.setDataAtRowProp(row, "IsEnable", true);
                        this.setDataAtRowProp(row, "ViewType", currentInstance.viewTypes[2]);
                    }
                }

                if (col == 'Layout') { //set sequence of the field
                    var meta = this.getCellMeta(row, currentInstance.colNames.indexOf('Custom HTML'));
                    meta.readOnly = value == 'Custom-Layout' ? false : true;
                }
            }
        },
        afterChange: function (change, source) {
            if (change != null) {
                var instance = this;
                var row = change[0][0];
                var col = change[0][1];
                var value = change[0][3];
                if (col == 'ElementType') { //refresh dropdown source for parent elements
                    if (value == 'Repeater' || value == 'Section') {
                        var column = instance.getDataAtCell(row, currentInstance.colNames.indexOf('Label'));
                        var isExist = jQuery.grep(currentInstance.data.SectionElements, function (n, i) {
                            return (n.DisplayText == value);
                        });

                        if (isExist <= 0) {
                            var tempId = 'N' + Math.floor((Math.random() * 9999) + 1);
                            currentInstance.data.sections.push(column);
                            this.setDataAtRowProp(row, "TempUIElementId", tempId);
                            var objContainer = {};
                            objContainer['DisplayText'] = column;
                            objContainer['Value'] = tempId;
                            currentInstance.data.SectionElements.push(objContainer);
                        }

                        var meta = instance.getCellMeta(row, currentInstance.colNames.indexOf('Parent'));
                        meta.source = currentInstance.data.sections;

                        var meta = instance.getCellMeta(row, currentInstance.colNames.indexOf('Layout'));
                        if (value == 'Repeater') {
                            meta.source = currentInstance.repLayout;
                        }
                        else {
                            meta.source = currentInstance.secLayout;
                        }

                        for (var i = 0; i < this.countCols() ; i++) {
                            meta = this.getCellMeta(row, i);
                            if (!meta.className) { meta.className = ''; }
                            meta.className = meta.className + ' cont-hilighter';
                        }
                        this.render();
                    }

                }
            }
        },
        afterValidate: function (result, value, row, prop, source) {
            if (!result) {
                //alert('There is an error boy ! - value - ' + value + ' row -' + row + ' prop -' + prop + +' source-' + source);
            }
        },
        afterSelection: function (r, c, r2, c2, preventScrolling, selectionLayerLevel) {
            currentInstance.currentRowId = r;
        },
        afterRowMove: function (rows, target) {
            //TODO: reset sequencing
        },
        beforePaste: function (data, coords) {
            $.each(data, function (indx, row) {
                row[currentInstance.colNames.indexOf('SourceUIElementId')] = row[1];
                row[currentInstance.colNames.indexOf('UIElementID')] = "";
                row[currentInstance.colNames.indexOf('UIElementName')] = "";
                row[currentInstance.colNames.indexOf('Sequence')] = "";
                row[currentInstance.colNames.indexOf('Parent')] = "";
            });
        },
        afterInit: function () {
            for (var rowNumber = 0; rowNumber < this.countRows() ; rowNumber++) {
                var row = this.getDataAtRow(rowNumber);
                for (var column = 0; column < this.countCols() ; column++) {
                    var meta = this.getCellMeta(rowNumber, column);
                    var value = false;
                    var elementType = row[currentInstance.colNames.indexOf('Element Type')];

                    if (!meta.className) { meta.className = ''; }

                    if (elementType == 'Section' || elementType == 'Repeater') {
                        meta.className = meta.className + ' cont-hilighter';
                    }

                    if (column == currentInstance.colNames.indexOf('Layout')) {
                        value = elementType != 'Section' && elementType != 'Repeater';
                        meta.source = elementType == 'Section' ? currentInstance.secLayout : currentInstance.repLayout;
                        meta.readOnly = value;
                    }

                    if (column == currentInstance.colNames.indexOf('Multiselect')) {
                        value = elementType != 'Dropdown List' && elementType != 'Dropdown TextBox';
                        meta.readOnly = value;
                    }

                    if (column == currentInstance.colNames.indexOf('Data Type') || column == currentInstance.colNames.indexOf('Max Length') || column == currentInstance.colNames.indexOf('Formats')) {
                        value = elementType != 'Dropdown List' && elementType != 'Dropdown TextBox' && elementType != 'Textbox' && elementType != 'Multiline TextBox' && elementType != 'Rich TextBox';
                        meta.readOnly = value;
                    }

                    if (column == currentInstance.colNames.indexOf('Is Key')) {
                        value == elementType == 'Section' || elementType == 'Repeater';
                        meta.readOnly = value;
                    }

                    if (column == currentInstance.colNames.indexOf('Allow Bulk Update')) {
                        value = elementType != 'Repeater';
                        meta.readOnly = value;
                    }



                    var comments = $.grep(currentInstance.data.Comments, function (n, i) {
                        return n.row == rowNumber && n.col == column;
                    });

                    if (comments.length > 0) {
                        meta.comment = { value: comments[0].comment.value };
                    }
                }
            }
            this.render();
        }
    });

    currentInstance.addButtons();
}

formDesignView.prototype.addContextMenu = function () {
    var currentInstance = this;
    var contextMenuItems = {}
    contextMenuItems =
    {
        "row_above": {},
        "row_below": {},
        "hsep1": "---------",
        "Add_Column": { name: "Add column" },
        "col_right": {},
        "hsep2": "---------",
        "remove_row": {},
        "hsep3": "---------",
        "undo": {},
        "redo": {},
        "hsep4": "---------",
        "commentsAddEdit": {},
        "commentsRemove": {},
        "hsep5": "---------",
        "column_chooser": { name: "Column Chooser" },
        "excel_settings": { name: "Excel Configuration" },
        "hsep6": "---------",
        "copy": {},
        "cut": {},
        //"paste": { name: "Paste" }
    }

    return contextMenuItems;
}

formDesignView.prototype.hideColumn = function (index) {
    var currentInstance = this;
    currentInstance.hiddenColumns.push(index);
    currentInstance.hotInstance.updateSettings({ hiddenColumns: { columns: currentInstance.hiddenColumns, indicators: false }, });
}

formDesignView.prototype.showColumn = function (index) {
    var currentInstance = this;
    var index = currentInstance.hiddenColumns.indexOf(index);
    if (index > -1) {
        currentInstance.hiddenColumns.splice(index, 1);
    }
    currentInstance.hotInstance.updateSettings({ hiddenColumns: { columns: currentInstance.hiddenColumns, indicators: false }, });
}

formDesignView.prototype.addColumn = function () {
    var currentInstance = this;
    var colName = prompt("Please enter column name", "Ext Prop");

    currentInstance.colNames.push(colName);
    var colDataName = colName.replace(/ +/g, "");
    currentInstance.extendedProperties.push({ Header: colName, Name: colDataName });
    currentInstance.colModels.push({ data: 'ExtProp.' + colDataName, width: 200 });

    currentInstance.hotInstance.updateSettings({
        colHeaders: currentInstance.colNames,
        columns: currentInstance.colModels
    });

    //currentInstance.hotInstance.selectCell(0, currentInstance.colNames.length - 1, 0, currentInstance.colNames.length - 1, true);
}

formDesignView.prototype.addButtons = function () {
    var currentInstance = this;

    var prtDiv = document.createElement('DIV');
    prtDiv.className = 'pull-right btn-padding';

    var rightSection = document.createElement('DIV');
    rightSection.className = 'dropdown pull-right details-custom-pagination';

    //Save button
    var btnSave = document.createElement('BUTTON');
    //btnSave.className = 'btn btn-sm but-align';
    btnSave.title = 'Save';
    btnSave.id = 'btnSave' + currentInstance.formDesignVersion.FormDesignVersionId;
    btnSave.onclick = function () { currentInstance.saveDesign(); }
    btnSave.innerHTML = '<i class="material-icons">&#xE161;</i>';
    rightSection.appendChild(btnSave);

    //Compile Design button
    var btnCompile = document.createElement('BUTTON');
    //btnCompile.className = 'btn btn-sm but-align';
    btnCompile.title = 'Compile Design';
    btnCompile.id = 'btnCompile' + currentInstance.formDesignVersion.FormDesignVersionId;
    btnCompile.innerHTML = '<i class="material-icons">&#xE90C;</i>';
    btnCompile.onclick = function () { currentInstance.compileDesign(); }
    rightSection.appendChild(btnCompile);

    //Compile Rule button
    var btnCompileRule = document.createElement('BUTTON');
    //btnCompileRule.className = 'btn btn-sm but-align';
    btnCompileRule.title = 'Compile Rule';
    btnCompileRule.id = 'btnCompileRule' + currentInstance.formDesignVersion.FormDesignVersionId;
    btnCompileRule.innerHTML = '<i class="material-icons">&#xE86D;</i>';
    btnCompileRule.onclick = function () { currentInstance.compileRules(); }
    rightSection.appendChild(btnCompileRule);

    //Compile PBPViewImpact button
    if (currentInstance.formDesignId == 2367) {
        var btnCompilePbpViewImpact = document.createElement('BUTTON');
        //btnCompilePbpViewImpact.className = 'btn btn-sm but-align';
        btnCompilePbpViewImpact.title = 'Compile Impacted Field';
        btnCompilePbpViewImpact.id = 'btnCompileImpactedField' + currentInstance.formDesignVersion.FormDesignVersionId;
        btnCompilePbpViewImpact.innerHTML = '<i class="material-icons">extension</i>';
        btnCompilePbpViewImpact.onclick = function () { currentInstance.compileImpactedField(); }
        rightSection.appendChild(btnCompilePbpViewImpact);
    }
    //Download excel
    var btnDownload = document.createElement('BUTTON');
    //btnDownload.className = 'btn btn-sm but-align';
    btnDownload.title = 'Download View';
    btnDownload.id = 'btnDownload' + currentInstance.formDesignVersion.FormDesignVersionId;
    btnDownload.innerHTML = '<i class="material-icons">&#xE861;</i>';
    btnDownload.onclick = function () { currentInstance.downloadView(""); }
    rightSection.appendChild(btnDownload);

    prtDiv.appendChild(rightSection);
    $(currentInstance.elementIDs.buttonHolderJQ).empty();
    document.getElementById(currentInstance.elementIDs.buttonHolder).appendChild(prtDiv);
}

formDesignView.prototype.compileDesign = function () {
    var currentInstance = this;

    var url = currentInstance.URLs.CompileFormDesignVersion;
    var data = {
        tenantId: 1,
        formDesignVersionId: currentInstance.formDesignVersion.FormDesignVersionId
    };

    var promise = ajaxWrapper.postJSON(url, data);

    promise.done(function (xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(DocumentDesign.documentCompileMsg);
        }
        else {
            messageDialog.show(DocumentDesign.compileFailureMsg);
        }
    });

    promise.fail(currentInstance.showError);
}

formDesignView.prototype.compileRules = function () {
    var currentInstance = this;

    var url = currentInstance.URLs.CompileDocumentRule;
    var data = {
        tenantId: 1,
        formDesignVersionId: currentInstance.formDesignVersion.FormDesignVersionId
    };

    var promise = ajaxWrapper.postJSON(url, data);

    promise.done(function (xhr) {
        if (xhr.Result === ServiceResult.SUCCESS && xhr.Items.length > 0 && xhr.Items[0].Status === ServiceResult.WARNING) {
            messageDialog.show(xhr.Items[0].Messages);
        }
        else if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(DocumentDesign.documentRuleCompileMsg);
        }
        else {
            if (xhr.Items.length > 0) {
                var msg = xhr.Items[0].Messages + '\n \n' + DocumentDesign.documentRuleCompileFailureMsg
                messageDialog.show(msg);
            }
            else {
                messageDialog.show(DocumentDesign.documentRuleCompileFailureMsg);
            }
        }
    });

    promise.fail(currentInstance.showError);
}

formDesignView.prototype.compileImpactedField = function () {
    var currentInstance = this;
    var url = currentInstance.URLs.CompileImpactedField;
    var data = {
        tenantId: 1,
        formDesignVersionId: currentInstance.formDesignVersion.FormDesignVersionId
    };

    var promise = ajaxWrapper.postJSON(url, data);
    //success callback
    promise.done(function (xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show("Impacted Field compiled successfully.");
        }
        else {
            if (xhr.Items.length > 0) {
                var msg = xhr.Items[0].Messages + '\n \n' + "Impacted Field compiled failed."
                messageDialog.show(msg);
            }
            else {
                messageDialog.show("Impacted Field compiled failed.");
            }
        }
    });
    //failure callback
    promise.fail(currentInstance.showError);
}

formDesignView.prototype.saveDesign = function () {
    var currentInstance = this;

    // Get the data in the cells
    var myTableData = currentInstance.hotInstance.getData();

    // If the last row is empty, remove it before validation
    if (myTableData.length > 1 && currentInstance.hotInstance.isEmptyRow(myTableData.length - 1)) {
        currentInstance.hotInstance.updateSettings({ minSpareRows: 0 });
        // Remove the last row if it's empty
        currentInstance.hotInstance.alter('remove_row', parseInt(myTableData.length - 1), keepEmptyRows = false);
    }

    // Validate the cells and submit the form via ajax or whatever
    currentInstance.hotInstance.validateCells(function (result, obj) {
        if (result == true) {
            var formDesignVersionData = {
                tenantId: currentInstance.tenantId,
                formDesignVersionId: currentInstance.formDesignVersion.FormDesignVersionId,
                formDesignId: currentInstance.formDesignId,
                prevElements: JSON.stringify(currentInstance.data.UIElementList),
                newElements: JSON.stringify(currentInstance.hotInstance.getSourceData()),
                comments: currentInstance.getComments(),
                extendedProperties: JSON.stringify(currentInstance.extendedProperties)
            }

            var url = currentInstance.URLs.saveElementList;
            var promise = ajaxWrapper.postJSON(url, formDesignVersionData);

            promise.done(function (xhr) {
                if (xhr.Result === ServiceResult.SUCCESS) {

                    currentInstance.saveDesignRulesTesterData();

                    messageDialog.show("Form design saved successfully.");
                    var url = currentInstance.URLs.getElementList.replace(/\{tenantId\}/g, currentInstance.tenantId).replace(/\{formDesignVersionId\}/g, currentInstance.formDesignVersion.FormDesignVersionId);
                    var promise = ajaxWrapper.getJSON(url);

                    promise.done(function (result) {
                        currentInstance.data = JSON.parse(JSON.stringify(result));
                        currentInstance.data.sections = [];
                        $.each(result.SectionElements, function (key, value) {
                            currentInstance.data.sections.push(value.DisplayText);
                        });
                        currentInstance.hotInstance.loadData(result.UIElementList);
                        currentInstance.hotInstance.updateSettings({
                            data: result.UIElementList,
                            colHeaders: currentInstance.getColNames(),
                            columns: currentInstance.getColModel(),
                            minSpareRows: 1
                        });
                        currentInstance.hotInstance.init();
                    });
                }
                else {
                    if (Array.isArray(xhr)) {
                        for (var i = 0; i < xhr.length; i++) {
                            var cellMeta = currentInstance.hotInstance.setCellMeta(xhr[i], 0, 'valid', false);
                        }
                        currentInstance.hotInstance.render();
                        currentInstance.hotInstance.updateSettings({ minSpareRows: 1 });
                        messageDialog.show("Please resolve errors and try again.");
                    }
                    else {
                        messageDialog.show("There is an error while saving form design.");
                        currentInstance.hotInstance.updateSettings({ minSpareRows: 1 });
                    }
                }
            });
        }
        else {
            messageDialog.show("Please resolve errors and try again.");
            currentInstance.hotInstance.updateSettings({ minSpareRows: 1 });
        }
    });
}


formDesignView.prototype.saveDesignRulesTesterData = function () {
    var currentInstance = this;
    var designRulesTesterData = currentInstance.designRulesTesterData
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


formDesignView.prototype.getComments = function () {
    var currentInstance = this;
    var data = [];
    var comments = currentInstance.hotInstance.getCellsMeta();
    $.each(comments, function (ind, meta) {
        if (meta && meta.comment) {
            data.push({ row: meta.row, col: meta.col, comment: meta.comment });
        }
    });

    return JSON.stringify(data);
}

formDesignView.prototype.downloadView = function (configuration) {
    var currentInstance = this;

    // Get the data in the cells
    var myTableData = currentInstance.hotInstance.getData();

    // If the last row is empty, remove it before validation
    if (myTableData.length > 1 && currentInstance.hotInstance.isEmptyRow(myTableData.length - 1)) {
        currentInstance.hotInstance.updateSettings({ minSpareRows: 0 });
        // Remove the last row if it's empty
        currentInstance.hotInstance.alter('remove_row', parseInt(myTableData.length - 1), keepEmptyRows = false);
    }

    var impactLogDataURL = currentInstance.URLs.downloadViewUrl;
    var stringData = "formDesignName=" + currentInstance.formDesignName;
    stringData += "<&formDesignId=" + currentInstance.formDesignId;
    stringData += "<&formDesignVersion=" + currentInstance.formDesignVersion.Version;
    stringData += "<&formDesignVersionId=" + currentInstance.formDesignVersion.FormDesignVersionId;
    stringData += "<&data=" + encodeURIComponent(JSON.stringify(currentInstance.hotInstance.getSourceData()));
    stringData += "<&extendedProperties=" + encodeURIComponent(JSON.stringify(currentInstance.extendedProperties));
    stringData += "<&comments=" + encodeURIComponent(currentInstance.getComments());
    stringData += "<&configuration=" + encodeURIComponent(configuration);
    $.downloadNew(impactLogDataURL, stringData, 'post');
    currentInstance.hotInstance.updateSettings({ minSpareRows: 1 });
}

formDesignView.prototype.showError = function (xhr) {
    if (xhr.status == 999)
        this.location = '/Account/LogOn';
    else

        messageDialog.show(JSON.stringify(xhr));
}

function radioButtonOptionDialog(uiElement, status, formDesignVersionId) {
    this.uiElement = uiElement;
    this.formDesignVersionId = formDesignVersionId;
    this.tenantId = 1;
    this.statustext = status;
    this.radioOptions = { 'OptionYes': '', 'OptionNo': '' };
}

radioButtonOptionDialog.elementIDs = {
    radioOptionJQ: '#radioButtonOptiondialog',
    optionYesJQ: '#txtOptionYes',
    optionNoJQ: '#txtOptionNo'
}
radioButtonOptionDialog._isInitialized = false;

radioButtonOptionDialog.init = function () {
    if (radioButtonOptionDialog._isInitialized == false) {
        $(radioButtonOptionDialog.elementIDs.radioOptionJQ).dialog({
            autoOpen: false,
            height: '500',
            width: '60%',
            modal: true
        });
        radioButtonOptionDialog._isInitialized = true;
    }
}

//show dialog
radioButtonOptionDialog.prototype.show = function (isLoaded) {
    $(radioButtonOptionDialog.elementIDs.optionYesJQ).val(this.uiElement.OptionYes);
    $(radioButtonOptionDialog.elementIDs.optionNoJQ).val(this.uiElement.OptionNo);
    if (isLoaded == false) {
        $(radioButtonOptionDialog.elementIDs.radioOptionJQ).dialog('option', 'title', this.uiElement.Label);
        $(radioButtonOptionDialog.elementIDs.radioOptionJQ).dialog('open');
    }
    else {
        //open existing dialog
        $(radioButtonOptionDialog.elementIDs.radioOptionJQ).dialog('open');
    }
    this.registerEvents();
}

radioButtonOptionDialog.prototype.registerEvents = function () {
    var currentInstance = this;
    $(radioButtonOptionDialog.elementIDs.optionYesJQ).off('change').on('change', function () {
        var value = $(radioButtonOptionDialog.elementIDs.optionYesJQ).val();
        currentInstance.radioOptions['OptionYes'] = value;
    });

    $(radioButtonOptionDialog.elementIDs.optionNoJQ).off('change').on('change', function () {
        var value = $(radioButtonOptionDialog.elementIDs.optionNoJQ).val();
        currentInstance.radioOptions['OptionNo'] = value;
    });
}

radioButtonOptionDialog.prototype.getOptions = function () {
    var currentInstance = this;
    return currentInstance.radioOptions;
}

radioButtonOptionDialog.init();

//contains functionality for column chooser in config vuew
function columnChooser(configViewInstance) {
    this.instance = configViewInstance;
    this.hiddenColumns = configViewInstance.hiddenColumns;
    this.requiredColumns = [0, 3, 14];
}

columnChooser.elementIDs = {
    columnChooserDialogJQ: '#columnchooserdialog',
    columnChooserDialogGridJQ: '#columnchooserdialoggrid',
    columnChooserDialogGrid: 'columnchooserdialoggrid'
}

columnChooser._isInitialized = false;

columnChooser.init = function () {
    var currentInstance = this;
    if (columnChooser._isInitialized == false) {
        $(columnChooser.elementIDs.columnChooserDialogJQ).dialog({ autoOpen: false, height: '250', width: '30%', modal: true });
        columnChooser._isInitialized = true;
    }
}

columnChooser.prototype.show = function (isLoaded) {
    var currentInstance = this;
    if (isLoaded == false) {
        this.loadGrid(currentInstance.instance);
    }
    else {
        //open existing dialog
        $(columnChooser.elementIDs.columnChooserDialogJQ).dialog('open');
    }
}

//load the grid of drop down items
columnChooser.prototype.loadGrid = function (instance) {
    var currentInstance = this;

    //set column models
    var colModel = [];
    colModel.push({ name: 'hide', width: 20, align: 'center', index: 'hide', editoptions: { value: "True:False" }, editrules: { required: true }, formatter: "checkbox", formatoptions: { disabled: false }, editable: true });
    colModel.push({ name: 'columnName', index: 'columnName', align: 'left', editable: false, sortable: false, width: 100 });
    colModel.push({ name: 'index', index: 'index', key: true, align: 'left', editable: false, sortable: false, hidden: true });

    $(columnChooser.elementIDs.columnChooserDialogGridJQ).jqGrid('GridUnload');
    $(columnChooser.elementIDs.columnChooserDialogGridJQ).parent().append("<div id='p" + columnChooser.elementIDs.columnChooserDialogGrid + "'></div>");

    $(columnChooser.elementIDs.columnChooserDialogGridJQ).jqGrid({
        datatype: 'local',
        cache: false,
        colNames: ['', 'Column Name', 'Index'],
        colModel: colModel,
        pager: '#p' + columnChooser.elementIDs.columnChooserDialogGrid,
        height: '220',
        autowidth: true,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        altclass: 'alternate',
        beforeSelectRow: function (rowid, e) {
            var $self = $(this),
                iCol = $.jgrid.getCellIndex($(e.target).closest("td")[0]),
                cm = $self.jqGrid("getGridParam", "colModel"),
                localData = $self.jqGrid("getLocalRow", rowid);
            if (cm[iCol].name === "hide" && e.target.tagName.toUpperCase() === "INPUT") {
                if ($(e.target).is(":checked")) {
                    instance.hideColumn(localData.index);
                }
                else {
                    instance.showColumn(localData.index);
                }
            }
            return true;
        },
        rowattr: function (item) {
            if (currentInstance.requiredColumns.indexOf(item.index) >= 0) {
                return { "class": "ui-state-disabled" };
            }
        },
        gridComplete: function () {
            $(columnChooser.elementIDs.columnChooserDialogJQ).dialog('option', 'title', 'Choose columns');
            $(columnChooser.elementIDs.columnChooserDialogJQ).dialog({ position: { my: 'center', at: 'center' }, });
            $(columnChooser.elementIDs.columnChooserDialogJQ).dialog('open');
        }
    });

    var pagerElement = '#p' + columnChooser.elementIDs.columnChooserDialogGrid;
    $(columnChooser.elementIDs.columnChooserDialogGridJQ).jqGrid('navGrid', pagerElement, { refresh: false, edit: false, add: false, del: false, search: false });
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    var columns = instance.getColNames();
    for (var i = 0; i < columns.length; i++) {
        if (currentInstance.requiredColumns.indexOf(i) < 0) {
            if (currentInstance.hiddenColumns.indexOf(i) < 0) {
                var obj = {};
                obj.columnName = columns[i];
                obj.index = i;
                $(columnChooser.elementIDs.columnChooserDialogGridJQ).jqGrid('addRowData', i + 1, obj);
            }
        }
    }
}

columnChooser.init();

//contains functionality for excel configuration in config vuew
function excelConfiguration(configViewInstance) {
    this.instance = configViewInstance;
    this.hiddenColumns = configViewInstance.hiddenColumns;
    this.buttonColumns = ['Source', 'Adv Config', 'Role Access'];
}

excelConfiguration.elementIDs = {
    configExportDialogJQ: '#excelconfigurationdialog',
    configExportDialogGridJQ: '#configurationdialoggrid',
    configExportDialogGrid: 'configurationdialoggrid'
}

excelConfiguration._isInitialized = false;

excelConfiguration.init = function () {
    var currentInstance = this;
    if (excelConfiguration._isInitialized == false) {
        $(excelConfiguration.elementIDs.configExportDialogJQ).dialog({ autoOpen: false, height: '400', width: '60%', modal: true });
        excelConfiguration._isInitialized = true;
    }
}

excelConfiguration.prototype.show = function (isLoaded) {
    var currentInstance = this;
    if (isLoaded == false) {
        currentInstance.getData(currentInstance.instance);
    }
    else {
        //open existing dialog
        currentInstance.getData(currentInstance.instance);
        $(excelConfiguration.elementIDs.configExportDialogJQ).dialog('open');
    }
}

excelConfiguration.prototype.getData = function (instance) {
    var currentInstance = this;
    var url = '/UIElement/GetConfigVieExcelConfiguration?formDesignVersionId={formDesignVersionId}'.replace(/\{formDesignVersionId\}/g, instance.formDesignVersion.FormDesignVersionId);
    var promise = ajaxWrapper.getJSON(url);

    promise.done(function (result) {
        result = currentInstance.getDefaultConfiguration(result, instance);
        currentInstance.loadGrid(result, instance);
    });
}

excelConfiguration.prototype.hide = function () {
    var currentInstance = this;
    $(excelConfiguration.elementIDs.configExportDialogJQ).dialog('close');
}

//load the grid of drop down items
excelConfiguration.prototype.loadGrid = function (data, instance) {
    var currentInstance = this;
    var lastSel;
    var colArray = ['ColumnIndex', 'Header', 'Column Name', 'Sequence', 'Include', 'Output Column'];
    var colModel = [];
    colModel.push({ name: 'ColumnIndex', align: 'center', width: 50, sortable: true, key: true, hidden: true });
    colModel.push({ name: 'ColumnHeader', index: 'ColumnHeader', width: 120 });
    colModel.push({ name: 'ColumnName', index: 'ColumnName', hidden: true, width: 120 });
    colModel.push({ name: 'Sequence', index: 'Sequence', width: '40', align: 'center', hidden: true, editable: false, sortable: false });
    colModel.push({ name: 'Include', index: 'Include', width: '40', align: 'center', edittype: 'checkbox', editoptions: { value: "True:False" }, editrules: { required: true }, formatter: "checkbox", formatoptions: { disabled: false }, editable: true });
    colModel.push({ name: 'OutputColumnName', index: 'OutputColumnName', width: 120, editable: true });

    //unload previous grid values
    $(excelConfiguration.elementIDs.configExportDialogGridJQ).jqGrid('GridUnload');
    $(excelConfiguration.elementIDs.configExportDialogGridJQ).parent().append("<div id='p" + excelConfiguration.elementIDs.configExportDialogGrid + "'></div>");

    //load grid
    $(excelConfiguration.elementIDs.configExportDialogGridJQ).jqGrid({
        datatype: 'local',
        data: data,
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Excel Export Configuration',
        pager: '#p' + excelConfiguration.elementIDs.configExportDialogGrid,
        height: '350',
        rowNum: 100,
        rownumbers: true,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        altclass: 'alternate',
        gridComplete: function () {
            $(excelConfiguration.elementIDs.configExportDialogJQ).dialog('option', 'title', 'Export Configuration');
            $(excelConfiguration.elementIDs.configExportDialogJQ).dialog({
                position: { my: 'center', at: 'center' },
            });
            $(excelConfiguration.elementIDs.configExportDialogJQ).dialog("open");
        },
        onSelectRow: function (id) {
            if (id && id !== lastSel) {
                $(excelConfiguration.elementIDs.configExportDialogGridJQ).jqGrid('saveRow', lastSel);
                lastSel = id;
            }
            $(excelConfiguration.elementIDs.configExportDialogGridJQ).editRow(id, true);
        }
    });
    $(excelConfiguration.elementIDs.configExportDialogGridJQ).jqGrid('sortableRows');
    var pagerElement = '#p' + excelConfiguration.elementIDs.configExportDialogGrid;
    //remove default buttons
    $(excelConfiguration.elementIDs.configExportDialogGridJQ).jqGrid('navGrid', pagerElement, { refresh: false, edit: false, add: false, del: false, search: false });
    //remove paging
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();
    $(excelConfiguration.elementIDs.configExportDialogGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: 'Save',
        onClickButton: function () {
            $(excelConfiguration.elementIDs.configExportDialogGridJQ).jqGrid('saveRow', lastSel);
            var data = $(excelConfiguration.elementIDs.configExportDialogGridJQ).jqGrid('getRowData');
            instance.downloadView(JSON.stringify(data));
            currentInstance.hide();
        }
    });
}

excelConfiguration.prototype.getDefaultConfiguration = function (result, instance) {
    var currentInstance = this;
    var data = [];
    var columnNames = instance.getColNames();
    var columns = instance.getColModel();
    for (var j = 0; j < columnNames.length; j++) {
        if (currentInstance.hiddenColumns.indexOf(j) < 0) {
            if (currentInstance.buttonColumns.indexOf(columnNames[j]) < 0) {
                var exists = $.grep(result, function (n) {
                    return (n.ColumnHeader == columnNames[j]);
                });
                if (exists.length > 0) {
                    data.push(exists[0]);
                } else {
                    var rowData = {};
                    rowData["ColumnIndex"] = j;
                    rowData["ColumnHeader"] = columnNames[j];
                    rowData["ColumnName"] = columns[j].data;
                    rowData["Sequence"] = j;
                    rowData["Include"] = true;
                    rowData["OutputColumnName"] = "";
                    data.push(rowData);
                }
            }
        }
    }
    return data;
}

excelConfiguration.init();
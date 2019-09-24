var dataSourceDialog = function (uiElement, formDesignId, formDesignVersionId, status, elementGridData, formDesignVersionInstance) {
    this.uiElement = uiElement;
    this.uiElementType = uiElement.ElementType;
    this.uiElementId = uiElement.UIElementID;
    this.UIElementName = uiElement.UIElementName;
    this.elementGridData = elementGridData;
    this.formDesignId = formDesignId;
    this.formDesignVersionId = formDesignVersionId;
    this.elementDropDownList = undefined;
    this.rulesData = undefined;
    this.rulesRowId = 0;
    this.expressionsRowId = 0;
    this.expressionsData = [];
    this.statustext = status;
    this.dataSourceElementsDialog = undefined;
    this.uiElementIdMappCollection = undefined;
    this.mappedUIElementId = 0;
    this.prevSelectedUIItems = undefined;
    this.displayModeItems = '';
    this.copyModeItems = '';
    this.formDesignVersionInstance = formDesignVersionInstance;
    this.dropDownDataSourceId = 0;
    this.tenantId = 1;
    this.filterOperatorList = '';
    this.displayDataSourceModeItems = '';
}


dataSourceDialog.URLs = {
    datasourceList: '/UIElement/DataSources?tenantId={tenantId}&uiElementId={uiElementId}&uiElementType={uiElementType}&formDesignId={formDesignId}&formDesignVersionId={formDesignVersionId}',
    datasourceuiElementsList: '/UIElement/DataSourceUIElements?tenantId={tenantId}&uiElementId={uiElementId}&uiElementType={uiElementType}&dataSourceId={dataSourceId}&formDesignId={formDesignId}&formDesignVersionId={formDesignVersionId}',
    updateDataSource: '/UIElement/UpdateDataSource',
    targetContainerElementList: '/UIElement/DataSourceFieldList?tenantId={tenantId}&formDesignId={formDesignId}&formDesignVersionId={formDesignVersionId}&parentUIElementId={parentUIElementId}&isKey={isKey}',
    addDataSourceMapping: '/UIElement/AddDataSourceUIElementMapping',
    dataSourceDisplayModeList: '/UIElement/GetDataSourceElementDisplayMode',
    checkElementMapping: '/UIElement/IsUIElementMappingExists',
    copyModeList: '/UIElement/GetDataCopyMode',
    formDesignList: '/FormDesign/FormDesignList?tenantId=1',
    formDesignVersionList: '/FormDesign/FormDesignVersionList?tenantId={tenantId}',
    dataSourceFilterOperatorList: '/UIElement/GetAllDataSourceFilterOperatorList',
    dataSourceModeList: '/UIElement/GetDataSourceDisplayMode',
}

dataSourceDialog._currentInstance = undefined;

dataSourceDialog.elementIDs = {
    dataSourceDialog: '#dataSourcedialog',
    dataSourceGrid: 'dataSourcegrid',
    dataSourceGridJQ: '#dataSourcegrid',
    dataSourceElementsGrid: 'dataSourceElementsgrid',
    dataSourceElementsGridJQ: '#dataSourceElementsgrid',
    dataCopyItems: ''
}

dataSourceDialog._isInitialized = false;
dataSourceDialog.repeaterKeyElements = [];



dataSourceDialog.showerror = function (xhr) {
    if (xhr.status == 999)
        this.location = '/Account/LogOn';
    else

        messageDialog.show(JSON.stringify(xhr));
}

dataSourceDialog.displayModeSuccess = function (data) {
    this.displayModeItems = data;
    //for (var i = 0; i < data.length; i++) {
    //    dataSourceDialog.displayModeItems = dataSourceDialog.displayModeItems + "<option id=" + data[i].DataSourceElementDisplayModeID + " value=" + data[i].DataSourceElementDisplayModeID + ">" + data[i].DisplayMode + "</option>";
    //}
}
//For Manual Data SourceMode DropDown
dataSourceDialog.dataSourceModeSuccess = function (data) {
    dataSourceDialog.displayDataSourceModeItems = "<option id='0' value='0'>Not Applicable</option>"
    for (var i = 0; i < data.length; i++) {
        dataSourceDialog.displayDataSourceModeItems = dataSourceDialog.displayDataSourceModeItems + "<option id=" + data[i].DataSourceModeID + " value=" + data[i].DataSourceModeID + ">" + data[i].DataSourceModeType + "</option>";
    }

}

function dataSourceCopyModeList() {
    //ajax call to add/update
    var promise = ajaxWrapper.getJSON(dataSourceDialog.URLs.copyModeList);
    //register ajax success callback
    promise.done(loadCopyModelist);
    //register ajax failure callback
    promise.fail(dataSourceDialog.showerror);
}

function loadCopyModelist(data) {
    dataSourceDialog.elementIDs.dataCopyItems = '';
    for (var i = 0; i < data.length; i++) {
        dataSourceDialog.elementIDs.dataCopyItems = dataSourceDialog.elementIDs.dataCopyItems + "<option value=" + data[i].Key + ">" + data[i].Value + "</option>";
    }
}

function dataSourceFilterOperators() {
    //ajax call to add/update
    var promise = ajaxWrapper.getJSON(dataSourceDialog.URLs.dataSourceFilterOperatorList);
    //register ajax success callback
    promise.done(loadFilterOperatorList);
    //register ajax failure callback
    promise.fail(dataSourceDialog.showerror);
}

function loadFilterOperatorList(data) {
    dataSourceDialog.filterOperatorList = '';
    for (var i = 0; i < data.length; i++) {
        dataSourceDialog.filterOperatorList = dataSourceDialog.filterOperatorList + "<option id=" + data[i].Key + " value=" + data[i].Key + ">" + data[i].Value + "</option>";
    }
}


dataSourceDialog.prototype.getDisplayModeDropDown = function () {
    var currentInstance = this;
    //ajax call to Get
    var promise = ajaxWrapper.getJSON(dataSourceDialog.URLs.dataSourceDisplayModeList);
    //register ajax success callback
    promise.done(function (data) {
        currentInstance.displayModeItems = data;
        //for (var i = 0; i < data.length; i++) {
        //    dataSourceDialog.displayModeItems = dataSourceDialog.displayModeItems + "<option id=" + data[i].DataSourceElementDisplayModeID + " value=" + data[i].DataSourceElementDisplayModeID + ">" + data[i].DisplayMode + "</option>";
        //}
    });
    //register ajax failure callback
    promise.fail(dataSourceDialog.showError);

    this.loadDataSourceGrid(this.uiElementType, this.formDesignId, this.formDesignVersionId);
}

//For Data Source Mode (auto/Manual)
dataSourceDialog.prototype.getDataSourceModeDropDown = function () {
    //ajax call to Get
    var promise = ajaxWrapper.getJSON(dataSourceDialog.URLs.dataSourceModeList);
    //register ajax success callback
    promise.done(dataSourceDialog.dataSourceModeSuccess);
    //register ajax failure callback
    promise.fail(dataSourceDialog.showError);
}
//init dialog
dataSourceDialog.init = function () {
    if (dataSourceDialog._isInitialized == false) {
        $(dataSourceDialog.elementIDs.dataSourceDialog).dialog({
            autoOpen: false,
            height: '600',
            width: '80%',
            modal: true,
            close: function () {
                dataSourceDialog.uiElementIdMappCollection = undefined;
                dataSourceDialog.prevSelectedUIItems = undefined;
            }
        });
        dataSourceDialog._isInitialized = true;

    }
}();

dataSourceDialog.prototype.show = function () {
    dataSourceDialog._currentInstance = this;
    $(dataSourceDialog.elementIDs.dataSourceDialog).dialog('option', 'title', 'DataSource - ' + this.uiElement.Label)
    $(dataSourceDialog.elementIDs.dataSourceDialog).dialog('open');
    if (this.uiElementType == "Repeater") {
        this.getDataSourceModeDropDown();
        this.getDisplayModeDropDown();

    }
    else
        this.loadDataSourceGrid(this.uiElementType, this.formDesignId, this.formDesignVersionId);
}

dataSourceDialog.prototype.loadDataSourceGrid = function (type, formDesignId, formDesignVersionId) {
    var currentInstance = this;

    var colArray;
    var colModel = [];
    //set column list
    if (type == "Section") {
        colArray = ['Include', 'DocumentName', 'DataSourceId', 'DataSourceName', 'Description', ''];
        //set column models
        colModel.push({
            name: 'Include', index: 'IsCurrentDS', align: 'center', width: 60, editable: false, formatter: checkBoxFormatter, unformatter: currentInstance.unFormatIncludedColumn, sortable: false, editType: 'checkbox',
            editoptions: { value: 'Yes:No', defaultValue: 'No' },
            stype: 'select',
            searchoptions: {
                sopt: ['eq', 'ne'],
                value: ':Any;Y:Yes;N:No',
            }
        });
        colModel.push({ name: 'DocumentName', index: 'DocumentName', align: 'left' });
        colModel.push({ name: 'DataSourceId', index: 'DataSourceId', key: true, hidden: true, search: false });
        colModel.push({ name: 'DataSourceName', index: 'DataSourceName', hidden: false, align: 'left' });
        colModel.push({ name: 'DataSourceDescription', index: 'DataSourceDescription', align: 'left', editable: false, sortable: false, width: 60 });
        colModel.push({ name: 'IsCurrentDS', index: 'IsCurrentDS', hidden: true, align: 'center' });
    }
    else if ((type == "Repeater")) {
        colArray = ['Include', 'DocumentName', 'DataSourceId', 'DataSourceName', 'Description', 'DisplayMode', 'Is Primary', 'DataSourceMode', '', '', '', ''];
        //set column models

        colModel.push({
            name: 'Include', index: 'IsCurrentDS', align: 'center', width: 60, editable: false, formatter: checkBoxFormatter, unformatter: currentInstance.unFormatIncludedColumn, sortable: false,
            editType: 'checkbox',
            editoptions: { value: 'Yes:No', defaultValue: 'No' },
            stype: 'select',
            searchoptions: {
                sopt: ['eq', 'ne'],
                value: ':Any;Y:Yes;N:No',
            }
        });
        colModel.push({ name: 'DocumentName', index: 'DocumentName', align: 'left' });
        colModel.push({ name: 'DataSourceId', index: 'DataSourceId', key: true, hidden: true, search: false });
        colModel.push({ name: 'DataSourceName', index: 'DataSourceName', hidden: false, align: 'left' });
        colModel.push({ name: 'DataSourceDescription', index: 'DataSourceDescription', align: 'left', editable: false, sortable: false, width: 60 });
        colModel.push({
            name: 'DisplayModeData',
            hidden: false, search: false,
            align: 'center',
            formatter: function (cellValue, options, rowObject) {
                var result = "<Select name = 'SelectMode' data-rowID=" + options.rowId + " id = Select_" + options.rowId + " class = 'form-control'>";
                for (var i = 0; i < currentInstance.displayModeItems.length; i++) {
                    result += "<option id="
                                + currentInstance.displayModeItems[i].DataSourceElementDisplayModeID
                                + " value=" + currentInstance.displayModeItems[i].DataSourceElementDisplayModeID
                                + (rowObject.DisplayMode === currentInstance.displayModeItems[i].DataSourceElementDisplayModeID ? "selected" : "")
                                + ">"
                                + currentInstance.displayModeItems[i].DisplayMode
                                + "</option>";
                }
                return result;
            },
            unformatter: function (cellValue, options, rowObject) {
                var result = "<Select name = 'SelectMode' data-rowID=" + options.rowId + " id = Select_" + options.rowId + " class = 'form-control'>";
                for (var i = 0; i < currentInstance.displayModeItems.length; i++) {
                    result += "<option id="
                                + currentInstance.displayModeItems[i].DataSourceElementDisplayModeID
                                + " value=" + currentInstance.displayModeItems[i].DataSourceElementDisplayModeID
                                + (rowObject.DisplayMode === currentInstance.displayModeItems[i].DataSourceElementDisplayModeID ? "selected" : "")
                                + ">"
                                + currentInstance.displayModeItems[i].DisplayMode
                                + "</option>";
                }
                return result;
            },
        });
        colModel.push({
            name: 'IsPrimaryData', index: 'IsPrimary', align: 'center', width: 60, editable: false, formatter: radioButtonFormatter, unFormmater: radioButtonFormatter,
            editType: 'radiobutton',
            editoptions: { value: 'Yes:No', defaultValue: 'No' },
            stype: 'select',
            searchoptions: {
                sopt: ['eq', 'ne'],
                value: ':Any;Y:Yes;N:No',
            }
        });
        colModel.push({ name: 'DataSourceMode', hidden: false, search: false, align: 'center', formatter: displayDataSourceModeDropDownFormatter, unformatter: displayDataSourceModeDropDownFormatter });
        colModel.push({ name: 'IsCurrentDS', index: 'IsCurrentDS', hidden: true, align: 'center' });
        colModel.push({ name: 'IsPrimary', index: 'IsPrimary', align: 'center', hidden: true, });
        colModel.push({ name: 'DisplayMode', index: 'DisplayMode', align: 'center', hidden: true });
        colModel.push({ name: 'DispalyDataSourceMode', index: 'DisplayMode', align: 'center', hidden: true });
    }
    else {
        colArray = ['Include', 'DocumentName', 'DataSourceId', 'DataSourceName', 'Description', ''];
        //set column models

        colModel.push({
            name: 'Include', index: 'IsCurrentDS', align: 'center', width: 60, editable: false, formatter: radioButtonDropDownFormatter, unformatter: radioButtonDropDownFormatter, sortable: false,
            editType: 'radiobutton',
            editoptions: { value: 'Yes:No', defaultValue: 'No' },
            stype: 'select',
            searchoptions: {
                sopt: ['eq', 'ne'],
                value: ':Any;Y:Yes;N:No',
            }
        });
        colModel.push({ name: 'DocumentName', index: 'DocumentName', align: 'left' });
        colModel.push({ name: 'DataSourceId', index: 'DataSourceId', key: true, hidden: true, search: false });
        colModel.push({ name: 'DataSourceName', index: 'DataSourceName', hidden: false, align: 'left' });
        colModel.push({ name: 'DataSourceDescription', index: 'DataSourceDescription', align: 'left', editable: false, sortable: false, width: 60 });
        colModel.push({ name: 'IsCurrentDS', index: 'IsCurrentDS', hidden: true, align: 'center' });
    }


    //get URL for grid    
    var datasourceURL = dataSourceDialog.URLs.datasourceList
   .replace(/\{tenantId\}/g, currentInstance.tenantId)
   .replace(/\{uiElementId\}/g, this.uiElementId)
   .replace(/\{uiElementType\}/g, this.uiElementType)
   .replace(/\{formDesignId\}/g, this.formDesignId)
   .replace(/\{formDesignVersionId\}/g, this.formDesignVersionId);

    //unload previous grid values
    $(dataSourceDialog.elementIDs.dataSourceGridJQ).jqGrid('GridUnload');
    $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid('GridUnload');


    $(dataSourceDialog.elementIDs.dataSourceGridJQ).parent().append("<div id='p" + dataSourceDialog.elementIDs.dataSourceGrid + "'></div>");
    allRecords = null;
    $(dataSourceDialog.elementIDs.dataSourceGridJQ).jqGrid({
        datatype: 'json',
        url: datasourceURL,
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Data Sources',
        pager: '#p' + dataSourceDialog.elementIDs.dataSourceGrid,
        height: '80',
        rowheader: true,
        loadonce: true,
        ignoreCase: true,
        rowNum: 10000,
        autowidth: true,
        gridview: true,
        scrollrows: true,
        viewrecords: true,
        altRows: true,
        sortable: true,
        sortname: 'IsCurrentDS',
        sortorder: 'desc',
        altclass: 'alternate',
        gridComplete: function () {
            dataSourceCopyModeList();
            var rows = $(this).getDataIDs();
            var newId;

            for (index = 0; index < rows.length; index++) {
                row = $(this).getRowData(rows[index]);

                if (type == "Dropdown List" || type == "Dropdown TextBox") {
                    if (row.IsCurrentDS == "Y") {
                        currentInstance.dropDownDataSourceId = row.DataSourceId;
                    }
                }
                if (type == "Section" || type == "Dropdown List" || type == "Dropdown TextBox") {
                    if (row.IsCurrentDS == "Y") {
                        $(row.Include).prop('checked', true);
                        newId = row.DataSourceId;
                        break;
                    }
                }
                else if (row.IsPrimary == "Y") {
                    $(row.Include).prop('checked', true);
                    newId = row.DataSourceId;
                    $('#Select_' + row.DataSourceId).attr("disabled", "disabled");
                    break;
                }
                else {
                    newId = rows[0];
                }
            }

            $(this).jqGrid('setSelection', newId);
            var grid = this;

            if (type == "Repeater") {
                $(this).find("input[name='radioDataSorce']").each(function () {
                    //set all the drop downs to be disabled initially, as the check box will decide which one to be used. 
                    $(this).attr("disabled", "disabled");
                });
            }

            $(grid).find("input:checkbox").each(function (idx, control) {

                $(control).click(function () {

                    var selectedRowId = $(dataSourceDialog.elementIDs.dataSourceGridJQ).getGridParam('selrow');
                    var selectedRowData = $(dataSourceDialog.elementIDs.dataSourceGridJQ).getRowData(selectedRowId);

                    var checkedRowData = $(dataSourceDialog.elementIDs.dataSourceGridJQ).getRowData(rows[idx]);
                    if (selectedRowData.DataSourceId === checkedRowData.DataSourceId) {
                        if ($(control).is(":checked")) {
                            if (selectedRowData.IsPrimary == 'Y') {
                                $('#btnAddDataSourceUnMapped').hide();
                            } else {
                                $('#btnAddDataSourceUnMapped').show();
                            }
                            setTimeout(function () {
                                $($(control).closest("tr")).find("input[name='radioDataSorce']").removeAttr("disabled", "disabled");
                                $(grid).jqGrid('setSelection', checkedRowData.DataSourceId);
                                $(dataSourceDialog.elementIDs.dataSourceGridJQ).find("#checkbox_" + checkedRowData.DataSourceId).prop("checked", true);
                            }, 10);
                        }
                        else {
                            $(grid).jqGrid('setSelection', checkedRowData.DataSourceId);
                            $('#btnAddDataSourceUnMapped').show();
                            $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).find("input:checkbox").each(function () {
                                $(this).prop("checked", false);
                            });

                            if (dataSourceDialog.uiElementIdMappCollection != undefined)
                                $(dataSourceDialog.uiElementIdMappCollection.removeAllFromDataSourceId(checkedRowData.DataSourceId));
                            $($(control).closest("tr")).find("select[name='SelectMode']").prop("selectedIndex", 0);
                            $($(control).closest("tr")).find("select[name='SelectMode']").removeAttr("disabled");
                            $($(control).closest("tr")).find("input[name='radioDataSorce']").attr("disabled", "disabled");
                            $($(control).closest("tr")).find("input[name='radioDataSorce']").attr("checked", false);
                        }
                    } else {
                        messageDialog.show(DocumentDesign.dataSourceRowSelectionMsg);
                    }
                });


                if (dataSourceDialog._currentInstance.uiElementType == "Repeater") {

                    if ($(control).is(":checked")) {
                        $($(control).closest("tr")).find("input[name='radioDataSorce']").removeAttr("disabled", "disabled");
                    }
                    var row = $(dataSourceDialog.elementIDs.dataSourceGridJQ).getRowData(rows[idx]);
                    if (row.IsPrimary == "Y") {
                        $($(control).closest("tr")).find("input[name='radioDataSorce']").prop("checked", true);
                        $('#btnAddDataSourceUnMapped').hide();
                    }

                    //if (row.DisplayMode == 3)
                    //    $($(control).closest("tr")).find("select[name='SelectMode']").prop("selectedIndex", 2);
                    //else if (row.DisplayMode == 2)
                    //    $($(control).closest("tr")).find("select[name='SelectMode']").prop("selectedIndex", 1);
                    //else
                    //    $($(control).closest("tr")).find("select[name='SelectMode']").prop("selectedIndex", 0);
                    if (row.DisplayMode != '')
                        $($(control).closest("tr")).find("select[name='SelectMode']").prop("selectedIndex", parseInt(row.DisplayMode) - parseInt(1));

                    if (row.DisplayMode == 3 || row.DisplayMode == 2 || row.DisplayMode == 3) {
                        if (row.DispalyDataSourceMode == 2)
                            $($(control).closest("tr")).find("select[name='SelectDataSourceMode']").prop("selectedIndex", 2);
                        else if (row.DispalyDataSourceMode == 3) {
                            $($(control).closest("tr")).find("select[name='SelectDataSourceMode']").prop("selectedIndex", 3);
                        }
                        else {
                            $($(control).closest("tr")).find("select[name='SelectDataSourceMode']").prop("selectedIndex", 1);
                        }
                        $($(control).closest("tr")).find("select[name='SelectDataSourceMode']").removeAttr("disabled");
                    }
                    else {
                        $($(control).closest("tr")).find("select[name='SelectDataSourceMode']").prop("selectedIndex", 0);
                        $($(control).closest("tr")).find("select[name='SelectDataSourceMode']").prop("disabled", "disabled");
                    }
                }

            });


            $(grid).find("input:radio").each(function (idx, control) {
                if ($(control).is(":checked")) {
                    $('#btnAddDataSourceUnMapped').hide();
                }
                $(control).click(function () {
                    if ($(control).is(":checked")) {
                        $('#btnAddDataSourceUnMapped').hide();

                        setTimeout(function () {
                            if (type == "Repeater") {
                                $(grid).find("select[name='SelectMode']").prop("selectedIndex", 0);
                                $(grid).find("select[name='SelectMode']").removeAttr("disabled");
                                $(grid).find("select[name='SelectDataSourceMode']").prop("selectedIndex", 0);
                                //$(grid).find("select[name='SelectDataSourceMode']").prop("disabled", "disabled");
                                var row = $(dataSourceDialog.elementIDs.dataSourceGridJQ).getRowData(rows[idx]);
                                $('#Select_' + row.DataSourceId).prop("disabled", "disabled");
                                $('#Select_' + row.DataSourceId).prop("selectedIndex", 2);
                                $('#SelectMode_' + row.DataSourceId).removeAttr("disabled");

                                $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).find("input[name='IsKeycheckbox']").each(function () {
                                    $(this).removeAttr("disabled", "disabled");
                                });
                            }
                            else if (type == "Dropdown List" || type == "Dropdown TextBox") {
                                //select grid on radio selection
                                var row = $(dataSourceDialog.elementIDs.dataSourceGridJQ).getRowData(rows[idx]);
                                $(grid).jqGrid('setSelection', row.DataSourceId);
                                $(dataSourceDialog.elementIDs.dataSourceGridJQ).find("#radiobutton_" + row.DataSourceId).prop("checked", true);
                            }

                        }, 10);
                    }


                });

            });
            $(dataSourceDialog.elementIDs.dataSourceGridJQ).find("select[name='SelectMode']").each(function () {
                $(this).change(function () {
                    var rowId = $(this).attr("data-rowID");
                    $(dataSourceDialog.elementIDs.dataSourceGridJQ).jqGrid('setSelection', rowId);
                    if ($(this).val() == 3 || $(this).val() == 2) {
                        $('#SelectMode_' + rowId).removeAttr("disabled");
                    }
                    else {
                        $('#SelectMode_' + rowId).prop("disabled", "disabled");
                        $('#SelectMode_' + rowId).prop("selectedIndex", 0)
                    }

                })
            });

            if (ClientRolesForFormDesignAccess.indexOf(vbRole) >= 0 && currentInstance.uiElement.IsStandard == "true") {
                $(grid).find("input:checkbox").attr('disabled', 'disabled');
                $(grid).find("input:radio").attr('disabled', 'disabled');
                $(".ui-search-toolbar").addClass("divdisabled");
                $(dataSourceDialog.elementIDs.dataSourceGridJQ).jqGrid('setGridParam', { onSelectRow: function (id) { return false; } });
            } else {
                $(".ui-search-toolbar").removeClass("divdisabled");
                $(grid).find("input:checkbox").removeAttr('disabled');
                $(grid).find("input:radio").removeAttr('disabled');
            }
        },
        onSelectRow: function (id, e) {
            var row = $(this).getRowData(id);


            if ($("#checkbox_" + row.DataSourceId).is(':checked')) {
                currentInstance.loadUIElementsGrid(currentInstance.tenantId, row.DataSourceId, formDesignId, formDesignVersionId);
            }

            if (type == "Dropdown List" || type == "Dropdown TextBox") {
                $(this).find("#radiobutton_" + row.DataSourceId).prop("checked", true);
            }
            if ($("#radiobutton_" + row.DataSourceId).is(':checked')) {
                currentInstance.loadUIElementsGrid(currentInstance.tenantId, row.DataSourceId, formDesignId, formDesignVersionId);
            }

            var isPrimaryExists = false;


            if (allRecords == null) {
                $(dataSourceDialog.elementIDs.dataSourceGridJQ).find("input:radio").each(function (idx, radioControl) {
                    if ($(radioControl).is(":checked") && radioControl.id == row.DataSourceId) {
                        $('#btnAddDataSourceUnMapped').hide();
                    }
                    else if ($(radioControl).is(":checked")) {
                        isPrimaryExists = true;
                    }
                });
                if (isPrimaryExists == false) {
                    $('#btnAddDataSourceUnMapped').hide();
                }
            }
            else if (row.IsPrimary == "Y") {
                $('#btnAddDataSourceUnMapped').hide();
            }


        },

        //loadComplete: function () {
        //    var $this = $(this),
        //        datatype = $this.getGridParam('datatype');

        //    if (datatype === "xml" || datatype === "json") {
        //        setTimeout(function () {
        //            $this.trigger("reloadGrid");
        //        }, 100);
        //    }
        //}
    });

    var pagerElement = '#p' + dataSourceDialog.elementIDs.dataSourceGrid;
    //remove default buttons
    $(dataSourceDialog.elementIDs.dataSourceGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    $(dataSourceDialog.elementIDs.dataSourceGridJQ).jqGrid('filterToolbar', {
        stringResult: true, searchOnEnter: false, defaultSearch: "cn", beforeSearch: function () {
            allRecords = $(dataSourceDialog.elementIDs.dataSourceGridJQ).getDataIDs();
            var selectedDataSourceID = $(this).getGridParam('selrow');
            var isPrimaryExists = false;
            $(dataSourceDialog.elementIDs.dataSourceGridJQ).find("input:radio").each(function (idx, radioControl) {
                if ($(radioControl).is(":checked") && radioControl.id == selectedDataSourceID) {
                    $('#btnAddDataSourceUnMapped').hide();
                }
                else if ($(radioControl).is(":checked")) {
                    isPrimaryExists = true;
                }
                if (isPrimaryExists == false) {
                    $('#btnAddDataSourceUnMapped').hide();
                }
            });
            return false;
        }
    });

    //remove paging
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    if (type == "Dropdown List" || type == "Dropdown TextBox") {
        //Clear button in footer of grid that clears selected Radiobutton Check
        $(dataSourceDialog.elementIDs.dataSourceGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: 'Clear All',
            onClickButton: function () {

                if (dataSourceDialog.prevSelectedUIItems === undefined)
                    dataSourceDialog.prevSelectedUIItems = new createCollection("uiElementCollection");

                dataSourceDialog.prevSelectedUIItems.add(new uiElementIdMapper(formDesignId, formDesignVersionId, currentInstance.dropDownDataSourceId, 0, dataSourceDialog._currentInstance.uiElementId));

                $("input:radio").attr("checked", false);
            }
        });
    }
    function displayDataSourceModeDropDownFormatter(cellValue, options, rowObject) {
        var result = "<Select name = 'SelectDataSourceMode' data-rowID=" + options.rowId + " id = SelectMode_" + options.rowId + " class = 'form-control'>";
        result = result + dataSourceDialog.displayDataSourceModeItems;
        return result;
    }

}

//format the grid column based on element property 
dataSourceDialog.prototype.formatIncludedColumn = function (cellValue, options, rowObject) {
    var result;
    if ((dataSourceDialog._currentInstance.uiElementType == 'Textbox' ||
        dataSourceDialog._currentInstance.uiElementType == 'Radio Button' ||
        dataSourceDialog._currentInstance.uiElementType == 'Multiline TextBox' ||
        dataSourceDialog._currentInstance.uiElementType == 'Calendar')) {
        result = '<input type="radio"' + 'name=' + dataSourceDialog._currentInstance.UIElementName + ' id=' + options.rowId;
    }
    else if ((dataSourceDialog._currentInstance.uiElementType == 'Section' ||
        dataSourceDialog._currentInstance.uiElementType == 'Repeater' ||
        dataSourceDialog._currentInstance.uiElementType == 'Dropdown List' ||
        dataSourceDialog._currentInstance.uiElementType == "Dropdown TextBox")) {
        result = "<input type='checkbox' id='" + options.rowId + "' name= '" + dataSourceDialog._currentInstance.UIElementName
    }

    if (rowObject.loaded == true)
        result = result + "' checked />";
    else
        result = result + "' unchecked />";
    return result;
}

dataSourceDialog.prototype.checkBoxColumnFormatter = function (cellValue, options, rowObject) {
    if (rowObject.IsKey) {
        return "<input type='checkbox' id='IsKey_" + options.rowId + "' name= 'IsKeycheckbox' checked />";
    } else {
        return "<input type='checkbox' id='IsKey_" + options.rowId + "' name= 'IsKeycheckbox' unchecked/>";
    }
}

dataSourceDialog.prototype.checkBoxColumnUnFormatter = function (cellValue, options, rowObject) {
    var result;
    result = $(this).find('#IsKey_' + options.rowId).prop('checked');
    return result;
}


dataSourceDialog.prototype.radioButtonForDropDownFormatter = function (cellValue, options, rowObject) {
    return "<input type='radio' id='radio_" + options.rowId + "' data-rowId=" + rowObject.UIElementID + " name= 'radio' />";
}



function radioButtonDropDownFormatter(cellValue, options, rowObject) {
    return "<input type='radio' id='radiobutton_" + options.rowId + "' data-rowId=" + rowObject.DataSourceId + " name= 'radioDataSorce' />";
}

function radioButtonFormatter(cellValue, options, rowObject) {
    return "<input type='radio' id='" + options.rowId + "' data-rowId=" + rowObject.DataSourceId + " name= 'radioDataSorce' />";
}

function radioButton(cellValue, options, rowObject) {
    return "<input type='radio' id='radio_" + options.rowId + "' data-rowId=" + rowObject.UIElementID + " name= 'radio' />";
}


function checkBoxFormatter(cellValue, options, rowObject) {
    if (rowObject.IsCurrentDS == "Y")
        return "<input type='checkbox' id='checkbox_" + options.rowId + "' name= 'checkbox_" + options.gid + "' checked />";
    else
        return "<input type='checkbox' id='checkbox_" + options.rowId + "' name= 'checkbox_" + options.gid + "' unchecked/>";
}

//unformat the grid column based on element property
dataSourceDialog.prototype.unFormatIncludedColumn = function (cellValue, options) {
    var result;
    result = $(this).find('#' + options.rowId).find('input').prop('checked');
    return result;
}

dataSourceDialog.prototype.loadUIElementsGrid = function (tenantId, dataSourceId, formDesignId, formDesignVersionId) {
    //set column list
    var colArray = [];
    //set column models
    var colModel = [];


    if (this.uiElementType == "Repeater") {
        colArray = ['Include', 'loaded', 'UIElementID', 'Label', 'Element Type', 'Mapped Element',
            'MappedUIElementId', 'MappedUIElementType', 'Operator', 'Filter Value', 'IsKey', 'DataCopyMode', 'Action', 'DataCopyModeID', 'DataSourceFilterOperatorID', 'IsRepeaterKey'];
        colModel.push({
            name: 'Include', index: 'loaded', width: 60, align: 'center', editable: false, formatter: this.formatIncludedColumn, unformat: this.unFormatIncludedColumn, sortable: false, search: false
        });
        colModel.push({ name: 'loaded', index: 'loaded', hidden: true, search: false });
        colModel.push({ name: 'UIElementID', key: true, index: 'UIElemenentId', hidden: true, search: false });
        colModel.push({ name: 'Label', index: 'Label', align: 'left' });
        colModel.push({ name: 'ElementType', index: 'ElementType', align: 'left' });
        colModel.push({ name: 'MappedUIElementName', index: 'MappedUIElementName', align: 'left', editable: true, sortable: false });
        colModel.push({ name: 'MappedUIElementId', index: 'MappedUIElementId', hidden: true, align: 'left', editable: false, sortable: false });
        colModel.push({ name: 'MappedUIElementType', index: 'MappedUIElementType', hidden: true, align: 'left', editable: false, sortable: false });
        colModel.push({ name: 'DataSourceFilterOperator', index: 'DataSourceFilterOperator', align: 'left', search: false, editable: true, formatter: dataSourceFilterOperatorFormatter, unformatter: dataSourceFilterOperatorFormatter });
        colModel.push({ name: 'DataSourceFilterValue', index: 'DataSourceFilterValue', align: 'left', editable: true, formatter: formatColumns, unformat: unformatColumns });
        colModel.push({
            name: 'IsKey', index: 'IsKey', width: 60, align: 'center', editable: true, formatter: this.checkBoxColumnFormatter, unformat: this.checkBoxColumnUnFormatter, sortable: false,
            editType: 'checkbox',
            editoptions: { value: 'Yes:No', defaultValue: 'No' },
            stype: 'select',
            searchoptions: {
                sopt: ['eq', 'ne'],
                value: ':Any;true:Yes;false:No',
            }
        });
        colModel.push({ name: 'DataCopyMode', index: 'DataCopyMode', align: 'center', search: false, formatter: dataSourceCopyModeDropDown, unformatter: dataSourceCopyModeDropDown });
        colModel.push({ name: 'Action', index: 'Action', search: false, align: 'center', formatter: dataSourceActionDropDown, unformatter: dataSourceActionDropDown });
        colModel.push({ name: 'DataCopyModeID', index: 'DataCopyModeID', align: 'center', hidden: true });
        colModel.push({ name: 'DataSourceFilterOperatorID', index: 'DataSourceFilterOperatorID', align: 'center', hidden: true });
        colModel.push({ name: 'IsRepeaterKey', index: 'IsRepeaterKey', align: 'center', hidden: true });
    }
    else if (this.uiElementType == "Section") {
        colArray = ['Include', 'loaded', 'UIElementID', 'Label', 'Element Type', 'Mapped Element', 'MappedUIElementId', 'DataCopyMode', 'Action', 'DataCopyModeID'];
        colModel.push({
            name: 'Include', index: 'loaded', width: 60, align: 'center', editable: false, formatter: this.formatIncludedColumn, unformat: this.unFormatIncludedColumn, sortable: false, search: false
        });
        colModel.push({ name: 'loaded', index: 'loaded', hidden: true, search: false });
        colModel.push({ name: 'UIElementID', key: true, index: 'UIElemenentId', hidden: true, search: false });
        colModel.push({ name: 'Label', index: 'Label', align: 'left' });
        colModel.push({ name: 'ElementType', index: 'ElementType', align: 'left' });
        colModel.push({ name: 'MappedUIElementName', index: 'MappedUIElementName', align: 'left', editable: false, sortable: false });
        colModel.push({ name: 'MappedUIElementId', index: 'MappedUIElementId', hidden: true, align: 'left', editable: false, sortable: false });
        colModel.push({ name: 'DataCopyMode', index: 'DataCopyMode', search: false, align: 'center', formatter: dataSourceCopyModeDropDown, unformatter: dataSourceCopyModeDropDown });
        colModel.push({ name: 'Action', index: 'Action', search: false, align: 'center', formatter: dataSourceActionDropDown, unformatter: dataSourceActionDropDown });
        colModel.push({ name: 'DataCopyModeID', index: 'DataCopyModeID', align: 'center', hidden: true });

    }
    else {
        colArray = ['Include', 'loaded', 'UIElementID', 'Label', 'Element Type', ''];

        colModel.push({
            name: 'Include', index: 'loaded', width: 60, align: 'center', editable: false, formatter: this.radioButtonForDropDownFormatter, unformat: this.radioButtonForDropDownFormatter, sortable: false, search: false
        });
        colModel.push({ name: 'loaded', index: 'loaded', hidden: true, search: false });
        colModel.push({ name: 'UIElementID', key: true, index: 'UIElemenentId', hidden: true, search: false });
        colModel.push({ name: 'Label', index: 'Label', align: 'left' });
        colModel.push({ name: 'ElementType', index: 'ElementType', align: 'left' });
        colModel.push({ name: 'MappedUIElementId', index: 'MappedUIElementId', hidden: true, align: 'left', editable: false, sortable: false });
    }

    //get URL for grid    
    var datasourceuielementListURL = dataSourceDialog.URLs.datasourceuiElementsList
                                                 .replace(/\{tenantId\}/g, tenantId)
                                                 .replace(/\{uiElementId\}/g, this.uiElementId)
                                                 .replace(/\{uiElementType\}/g, this.uiElementType)
                                                 .replace(/\{dataSourceId\}/g, dataSourceId)
                                                 .replace(/\{formDesignId\}/g, this.formDesignId)
                                                 .replace(/\{formDesignVersionId\}/g, this.formDesignVersionId);

    $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid('GridUnload');

    $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).parent().append("<div id='p" + dataSourceDialog.elementIDs.dataSourceElementsGrid + "'></div>");



    var currentInstance = this;

    var isPrimaryExist = false;
    this.sourceUIElements = [];

    if (currentInstance.uiElementType == "Repeater") {
        if (dataSourceDialog.uiElementIdMappCollection === undefined)
            dataSourceDialog.uiElementIdMappCollection = new createCollection("uiElementCollection");

        var url = dataSourceDialog.URLs.targetContainerElementList.replace(/\{tenantId\}/g, currentInstance.tenantId).replace(/\{formDesignId\}/g, currentInstance.formDesignId)
                        .replace(/\{formDesignVersionId\}/g, currentInstance.formDesignVersionId).replace(/\{parentUIElementId\}/g, currentInstance.uiElement.UIElementID).replace(/\{isKey\}/g, false);;

        var promise = ajaxWrapper.getJSON(url);
        promise.done(function (xhr) {
            currentInstance.sourceUIElements = xhr;
            console.log('Elements');
        });
        dataSourceFilterOperators();
    }

    $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid({
        datatype: 'json',
        url: datasourceuielementListURL,
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'UIElements',
        loadonce: true,
        ignoreCase: true,
        pager: '#p' + dataSourceDialog.elementIDs.dataSourceElementsGrid,
        height: '150',
        autowidth: true,
        viewrecords: true,
        rowNum: 10000,
        hidegrid: false,
        sortable: true,
        sortname: 'loaded',
        sortorder: 'desc',
        gridComplete: function () {
            var grid = this;
            //attach the change event to the DropDown list in Action column
            //this drop down will server as the source for which element the data source should map to 
            $(this).find("select[name='SelectAction']").each(function () {
                //set all the drop downs to be disabled initially, as the check box will decide which one to be used. 
                $(this).attr("disabled", "disabled");

                $(this).change(function () {
                    //if the Value selected is "Create New" then show popup to create new element within the section/repeater
                    if ($(this).val() == 1) {
                        var elementType = $(grid).getRowData($(this).attr("data-rowId")).ElementType;
                        var selectedUiElmentId = $(grid).getRowData($(this).attr("data-rowId")).UIElementID;
                        var isKey = $(grid).getRowData($(this).attr("data-rowId")).IsKey;
                        var rowId = $(this).attr("data-rowID");
                        $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid("setSelection", rowId);
                        addDataSourceFieldDialog.show('', 'add', 1, currentInstance.formDesignId, currentInstance.formDesignVersionId, currentInstance.uiElement.UIElementID, null, elementType, dataSourceDialog._currentInstance, selectedUiElmentId, isKey, dataSourceId);
                        $("#select_" + selectedUiElmentId).val(0);
                    }
                        //if the Value selected is "Use Existing" then show popup. this pop up will list down all the elements in current section/repeater
                    else if ($(this).val() == 2) {
                        var rowId = $(this).attr("data-rowID");

                        var elementType = $(grid).getRowData($(this).attr("data-rowId")).ElementType;
                        var rowData = $(grid).getRowData($(this).attr("data-rowId"));

                        //save the updated form design list attached to the formgroup
                        var url = dataSourceDialog.URLs.targetContainerElementList.replace(/\{tenantId\}/g, currentInstance.tenantId).replace(/\{formDesignId\}/g, currentInstance.formDesignId)
                                .replace(/\{formDesignVersionId\}/g, currentInstance.formDesignVersionId).replace(/\{parentUIElementId\}/g, currentInstance.uiElement.UIElementID);
                        if (rowData.IsKey === undefined) {
                            url = url.replace(/\{isKey\}/g, false);
                        } else {
                            url = url.replace(/\{isKey\}/g, rowData.IsKey);
                        }
                        var promise = ajaxWrapper.getJSON(url);
                        promise.done(function (xhr) {
                            currentInstance.dataSourceElementsDialog = new dataSourceElementsDialog(currentInstance.uiElement, currentInstance.formDesignVersionId, currentInstance.statustext, rowId, xhr, elementType);
                            $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid("setSelection", rowId);
                            currentInstance.dataSourceElementsDialog.show();
                            currentInstance.dataSourceElementsDialog.loadTargetElementsGrid(rowData, dataSourceId, formDesignId, formDesignVersionId);
                            $("#select_" + rowData.UIElementID).val(0);
                        });
                        promise.fail(showError);
                    }
                    else {
                        //DO NOTHING HERE.
                    }
                });
            });

            //set all the text to be disabled initially, as the drop down will decide which one to be used. 
            $(this).find('input:text').each(function () {
                $(this).attr("disabled", "disabled");
            });
            //On select drop down Value its remove the disable attribute 
            $(this).find("select[name='SelectOperator']").each(function () {
                // remove disabled which dropdown is selected 
                $(this).change(function () {
                    $($(this).closest("tr")).find("input:text").removeAttr("disabled", "disabled");

                    if ($(this).val() == 0 || $(this).val() == 7) {
                        $($(this).closest("tr")).find("input:text").attr("disabled", "disabled");
                        $($(this).closest("tr")).find("input:text").val('');
                    }
                });

                //Remove disable text box  when already select drop down and its initaily load
                $(this).find("option:selected").each(function () {
                    var rows = $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid().getDataIDs();
                    $.each(rows, function (i, item) {
                        var rowData = $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid().getRowData(item);
                        if (rowData.DataSourceFilterValue != '' && rowData.Include == true) {
                            $("#DataSourceFilter_" + rowData.UIElementID).removeAttr("disabled", "disabled");
                        }
                    });
                });
            });

            //Attach a click event to the Include checkbox which will toggle the enable/disable attributes of Action DropDownList
            $(this).find("input:checkbox[name=" + dataSourceDialog._currentInstance.UIElementName + "]").each(function (idx) {

                var rows = $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid().getDataIDs();

                $.each(rows, function (i, item) {
                    var rowData = $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid().getRowData(item);

                    if (rowData.UIElementID == rowData.MappedUIElementId && rowData.Include == true) {
                        $(this).find("#checkbox_" + rowData.UIElementID).prop("checked", true);
                    }
                });

                $(this).click(function () {
                    if ($(this).is(":checked")) {
                        //enable the drop down list that is remove the disabled attribute.
                        $($(this).closest("tr")).find("select[name='SelectAction']").removeAttr("disabled", "disabled");

                        if (dataSourceDialog.uiElementIdMappCollection === undefined)
                            dataSourceDialog.uiElementIdMappCollection = new createCollection("uiElementCollection");


                        if (!dataSourceDialog.uiElementIdMappCollection.isExists(dataSourceId, $(this).attr("id"), formDesignId, formDesignVersionId)) {

                            var rows = $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid().getDataIDs();
                            var mappId = 0;
                            for (index = 0; index < rows.length; index++) {
                                row = $($(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid()).getRowData(rows[index]);
                                if (row.UIElementID == $(this).attr("id")) {
                                    mappId = row.MappedUIElementId;
                                }
                            }

                            dataSourceDialog.uiElementIdMappCollection.add(new uiElementIdMapper(formDesignId, formDesignVersionId, dataSourceId, $(this).attr("id"), mappId));
                        }

                    }
                    else {
                        //disable the drop down list that is add the disabled attribute.
                        $($(this).closest("tr")).find("select[name='SelectAction']").attr("disabled", "disabled");
                        //set the first element as selected value ie. Select
                        $($(this).closest("tr")).find("select[name='SelectAction']").val(0);


                        if (dataSourceDialog.uiElementIdMappCollection != undefined && dataSourceDialog.uiElementIdMappCollection.length > 0) {
                            dataSourceDialog.uiElementIdMappCollection.remove(dataSourceId, $(this).attr("id"), formDesignId, formDesignVersionId);
                        }
                    }
                });


                if ($(this).is(":checked")) {

                    $($(this).closest("tr")).find("select[name='SelectAction']").removeAttr("disabled", "disabled");


                    if (dataSourceDialog.uiElementIdMappCollection === undefined)
                        dataSourceDialog.uiElementIdMappCollection = new createCollection("uiElementCollection");

                    if (!dataSourceDialog.uiElementIdMappCollection.isExists(dataSourceId, $(this).attr("id"), formDesignId, formDesignVersionId)) {

                        var mappId = 0;
                        var dataCopyModeId = 0;
                        var filterValue = '';
                        var operatorId = 0;
                        var isKey = false;
                        var isPrimary = false;
                        for (index = 0; index < rows.length; index++) {
                            row = $($(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid()).getRowData(rows[index]);
                            if (row.UIElementID == $(this).attr("id")) {
                                mappId = row.MappedUIElementId;
                                dataCopyModeId = row.DataCopyModeID;
                                if (currentInstance.uiElementType == "Repeater") {
                                    filterValue = row.DataSourceFilterValue;
                                    operatorId = row.DataSourceFilterOperatorID;
                                    isKey = row.IsKey;
                                    targetElementType = row.MappedUIElementType;
                                    var upperRow = $(dataSourceDialog.elementIDs.dataSourceGridJQ).getRowData(dataSourceId);
                                    isPrimary = upperRow.IsPrimary;
                                }
                            }
                        }
                        if (currentInstance.uiElementType == "Repeater") {
                            dataSourceDialog.uiElementIdMappCollection.add(new uiElementIdMapper(formDesignId, formDesignVersionId, dataSourceId,
                                $(this).attr("id"), mappId, dataCopyModeId, operatorId, filterValue, isKey, targetElementType, isPrimary));
                        } else {
                            dataSourceDialog.uiElementIdMappCollection.add(new uiElementIdMapper(formDesignId, formDesignVersionId, dataSourceId,
                                $(this).attr("id"), mappId, dataCopyModeId));
                        }
                    }

                    if (dataSourceDialog.prevSelectedUIItems != undefined) {
                        if (!dataSourceDialog.uiElementIdMappCollection.isExists(dataSourceId, $(this).attr("id"), formDesignId, formDesignVersionId)) {
                            $($(this).closest("tr")).find("select[name='SelectAction']").attr("disabled", "disabled");
                            //set the first element as selected value ie. Select
                            $($(this).closest("tr")).find("select[name='SelectAction']").val(0);
                            $(this).prop("checked", false);
                            return;
                        }
                        else {
                            $($(this).closest("tr")).find("select[name='SelectAction']").removeAttr("disabled", "disabled");
                        }

                    }

                    if (dataSourceDialog.prevSelectedUIItems === undefined)
                        dataSourceDialog.prevSelectedUIItems = new createCollection("uiElementCollection");

                    if (!dataSourceDialog.prevSelectedUIItems.isExists(dataSourceId, $(this).attr("id"), formDesignId, formDesignVersionId))
                        dataSourceDialog.prevSelectedUIItems.add(new uiElementIdMapper(formDesignId, formDesignVersionId, dataSourceId, $(this).attr("id"), mappId));

                    var row = $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid().getRowData(rows[idx]);

                    if (row.DataCopyModeID == 1 || row.DataCopyModeID == "")
                        $($(this).closest("tr")).find("select[name='SelectCopyMode']").prop("selectedIndex", 0);
                    else
                        $($(this).closest("tr")).find("select[name='SelectCopyMode']").prop("selectedIndex", 1);

                    if (currentInstance.uiElementType == "Repeater") {
                        var filterOperatorLength = $($(this).closest("tr")).find("select[name='SelectOperator']")[0].length;
                        for (var i = 0; i < filterOperatorLength; i++) {
                            if (row.DataSourceFilterOperatorID == i) {
                                $($(this).closest("tr")).find("select[name='SelectOperator']").prop("selectedIndex", i);
                            }
                        }
                    }


                }
                else if (dataSourceDialog.uiElementIdMappCollection != undefined && dataSourceDialog.uiElementIdMappCollection.isExists(dataSourceId, $(this).attr("id"), formDesignId, formDesignVersionId)) {
                    $(this).prop("checked", true);
                    $($(this).closest("tr")).find("select[name='SelectAction']").removeAttr("disabled", "disabled");
                    return;
                }

            });

            $(this).find("input:radio").each(function (idx, control) {
                $(control).click(function () {

                    if ($(control).is(":checked")) {

                        if (dataSourceDialog.uiElementIdMappCollection === undefined)
                            dataSourceDialog.uiElementIdMappCollection = new createCollection("uiElementCollection");

                        var rows = $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid().getDataIDs();
                        var row = $($(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid()).getRowData(rows[idx]);
                        var mappId = row.UIElementID;

                        dataSourceDialog.uiElementIdMappCollection.removeAll();
                        dataSourceDialog.uiElementIdMappCollection.add(new uiElementIdMapper(formDesignId, formDesignVersionId, dataSourceId, mappId, dataSourceDialog._currentInstance.uiElementId));

                    }

                });

                var rows = $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid().getDataIDs();
                var row = $($(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid()).getRowData(rows[idx]);
                var mappId = row.MappedUIElementId;

                if (mappId == dataSourceDialog._currentInstance.uiElementId)
                    $(this).prop("checked", true);
            });

            if (currentInstance.uiElementType == "Repeater") {
                dataSourceDialog.repeaterKeyElements = [];
                var rows = $(grid).jqGrid().getDataIDs();
                $.each(rows, function (i, item) {
                    var rowData = $(grid).jqGrid().getRowData(item);
                    if (rowData.UIElementID == rowData.MappedUIElementId) {
                        $('#filterOperator_' + rowData.UIElementID).attr('disabled', 'disabled');
                        $('#DataSourceFilter_' + rowData.UIElementID).attr('disabled', 'disabled');
                        $('#IsKey_' + rowData.UIElementID).attr('disabled', 'disabled');
                        $('#selectcopy_' + rowData.UIElementID).attr('disabled', 'disabled');
                        $('#select_' + rowData.UIElementID).attr('disabled', 'disabled');
                    }
                    if (rowData.IsRepeaterKey == "true") {
                        var isKeyColumnMapped = false;
                        //set Mappings if same name key exists
                        $.each(currentInstance.sourceUIElements, function (i, existingElement) {
                            if (existingElement.Label == rowData.Label) {
                                dataSourceDialog.uiElementIdMappCollection.add(new uiElementIdMapper(formDesignId, formDesignVersionId, dataSourceId, rowData.UIElementID, existingElement.UiElementId));
                                dataSourceDialog.uiElementIdMappCollection.setMappingId(dataSourceId, rowData.UIElementID, existingElement.UIElementID, existingElement.ElementType, formDesignId, formDesignVersionId);
                                rowData.MappedUIElementName = rowData.Label;
                                rowData.IsKey = true;
                                rowData.loaded = true;
                                isKeyColumnMapped = true;
                                $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid('setRowData', item, rowData);
                            }
                        });
                        if (!isKeyColumnMapped) {
                            rowData.MappedUIElementName = rowData.Label;
                            rowData.IsKey = true;
                            rowData.loaded = true;  // this property is used to decide whether element is mapped or not. which is cascaded to Include flag in formatter

                            $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid('setRowData', item, rowData);
                            dataSourceDialog.uiElementIdMappCollection.add(new uiElementIdMapper(formDesignId, formDesignVersionId, dataSourceId, rowData.UIElementID, 0, rowData.copyModeId, undefined, undefined, true, 'Label', undefined, undefined));
                        }
                    }

                });
            }

            //below changes to handle authorization for Dynamic Grid UIElements[EQN-252]
            authorizePropertyGrid($(this), dataSourceDialog.URLs.formDesignVersionList.replace(/\{tenantId\}/g, currentInstance.tenantId));

            var selectedDataSourceRow = $(dataSourceDialog.elementIDs.dataSourceGridJQ).getRowData(dataSourceId);

            $(dataSourceDialog.elementIDs.dataSourceGridJQ).find("input:radio").each(function (idx, control) {
                if (control.id == dataSourceId && control.checked) {
                    $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).find("input[name='IsKeycheckbox']").each(function () {
                        $(this).removeAttr("disabled", "disabled");
                    });
                }
            });

            if (ClientRolesForFormDesignAccess.indexOf(vbRole) >= 0 && currentInstance.uiElement.IsStandard == "true") {
                $('#p' + dataSourceDialog.elementIDs.dataSourceElementsGrid).hide();//.addClass('divdisabled');
                $(this).find("input:radio").attr('disabled', 'disabled');
            } else {
                $('#p' + dataSourceDialog.elementIDs.dataSourceElementsGrid).show();
                $(this).find("input:radio").removeAttr('disabled');
            }
        },
        //loadComplete: function () {
        //    var $this = $(this),
        //        datatype = $this.getGridParam('datatype');

        //    if (datatype === "xml" || datatype === "json") {
        //        setTimeout(function () {
        //            $this.trigger("reloadGrid");
        //        }, 100);
        //    }
        //}
    });
    if (this.uiElementType == "Repeater") {
        $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid('setGroupHeaders', {
            useColSpanStyle: false,
            groupHeaders: [
                { startColumnName: 'DataSourceFilterOperator', numberOfColumns: 2, titleText: '<div class="ui-jqgrid-sortable">DataSource Filter</div>' }
            ]

        });
    }
    var pagerElement = '#p' + dataSourceDialog.elementIDs.dataSourceElementsGrid;
    //remove default buttons
    $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

    //remove paging
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();
    //event to handle permissions for data sources.
    var editFlag = false;



    editFlag = checkUserPermissionForEditAndDataSource(dataSourceDialog.URLs.formDesignVersionList.replace(/\{tenantId\}/g, currentInstance.tenantId));
    if (editFlag) {
        //if (this.statustext != 'Finalized') {
        //TODO: the below condition which is always false, remove after completing the implementation of DataSource UnMapped (EQN- 368)
        if (this.uiElementType == "Repeater") {
            $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid('navButtonAdd', pagerElement,
         {
             caption: '', buttonicon: 'ui-icon-plus',
             title: 'Add UnMapped Elements', id: 'btnAddDataSourceUnMapped',
             onClickButton: function () {
                 //on Add Button Click Load UnMapped Element for selected Container Element                            
                 var selectedIDs = $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid('getGridParam', 'data');
                 //save the updated form design list atached to the formgroup
                 var url = dataSourceDialog.URLs.targetContainerElementList.replace(/\{tenantId\}/g, currentInstance.tenantId)
                      .replace(/\{formDesignId\}/g, currentInstance.formDesignId)
                      .replace(/\{formDesignVersionId\}/g, currentInstance.formDesignVersionId)
                      .replace(/\{parentUIElementId\}/g, currentInstance.uiElement.UIElementID)
                      .replace(/\{isKey\}/g, false);

                 var filteredList = [];
                 var promise = ajaxWrapper.getJSON(url);
                 promise.done(function (xhr) {
                     //To filter unmapped elements list if we already selected unmapped elements. 
                     var k = 0;
                     for (i = 0; i < xhr.length ; i++) {
                         var isAlreadySelected = false;
                         for (j = 0; j < selectedIDs.length; j++) {
                             if (xhr[i].UIElementID == selectedIDs[j].UIElementID)
                                 isAlreadySelected = true;
                         }
                         if (isAlreadySelected == false) {
                             filteredList[k++] = xhr[i];
                         }
                     }
                     currentInstance.unMappedElementsDialog = new unMappedElementsDialog(currentInstance.uiElement, currentInstance.formDesignVersionId, currentInstance.statustext, filteredList);
                     currentInstance.unMappedElementsDialog.show();
                     currentInstance.unMappedElementsDialog.loadTargetElementsGrid(dataSourceId, formDesignId, formDesignVersionId);
                 });
                 promise.fail(showError);
             }
         });
        }

        $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: 'Save',
                onClickButton: function () {
                    var selectedDataSourceID = $(dataSourceDialog.elementIDs.dataSourceGridJQ).getGridParam('selrow');
                    var uiElementsList = $(this).getRowData();
                    var flag = false;
                    var isLoded = false;

                    for (i = 0 ; i < uiElementsList.length ; i++) {
                        if (uiElementsList[i].Include == true) {
                            flag = true;
                        }
                        if (uiElementsList[i].loaded == "true") {
                            isLoded = true;
                        }
                    }

                    if ($("#checkbox_" + selectedDataSourceID).is(":checked") && flag == false) {
                        messageDialog.show("Please select map element");
                    }
                    else {

                        if (dataSourceDialog._currentInstance.uiElementType !== "Dropdown TextBox" && dataSourceDialog.uiElementIdMappCollection != undefined && dataSourceDialog.uiElementIdMappCollection.length > 0) {
                            setCopyModeForDataSource(uiElementsList, dataSourceId);
                        }
                        if (dataSourceDialog._currentInstance.uiElementType !== "Dropdown List" && dataSourceDialog.uiElementIdMappCollection != undefined && dataSourceDialog.uiElementIdMappCollection.length > 0) {
                            setCopyModeForDataSource(uiElementsList, dataSourceId);
                        }

                        if (dataSourceDialog._currentInstance.uiElementType == "Repeater" && dataSourceDialog.uiElementIdMappCollection != undefined && dataSourceDialog.uiElementIdMappCollection.length > 0) {
                            setDisplayModeAndIsPrimaryForRepeater();
                            setFilterForDataSource(uiElementsList, dataSourceId);
                            setIsKeyForDataSource(uiElementsList, dataSourceId);
                            if (!isValid())
                                return;
                        }

                        if (dataSourceDialog.uiElementIdMappCollection != undefined && dataSourceDialog.uiElementIdMappCollection.length > 0) {
                            if (checkForMappedElement(dataSourceDialog.uiElementIdMappCollection) && checkIsKeyhasLabelMapping(dataSourceDialog.uiElementIdMappCollection) && checkForFilterElement(dataSourceDialog.uiElementIdMappCollection)) {

                                //show confirm dialog
                                confirmDialog.show(DocumentDesign.datasourceElementSaveConfirmMsg, function () {
                                    confirmDialog.hide();
                                    if (dataSourceDialog.prevSelectedUIItems != undefined && dataSourceDialog.prevSelectedUIItems.length > 0) {
                                        var existingDSIds = dataSourceDialog.prevSelectedUIItems.getAlreadyExistingDataSourceId();
                                        UpdateDatasource(dataSourceDialog.uiElementIdMappCollection, dataSourceId, dataSourceDialog._currentInstance.uiElementId, false, formDesignId, formDesignVersionId, existingDSIds);
                                    } else {
                                        UpdateDatasource(dataSourceDialog.uiElementIdMappCollection, dataSourceId, dataSourceDialog._currentInstance.uiElementId, false, formDesignId, formDesignVersionId, null);
                                    }

                                })
                            }
                        }
                        else if (dataSourceDialog.prevSelectedUIItems != undefined && dataSourceDialog.prevSelectedUIItems.length > 0) {
                            //show confirm dialog
                            confirmDialog.show(DocumentDesign.datasourceElementSaveConfirmMsg, function () {
                                confirmDialog.hide();
                                UpdateDatasource(dataSourceDialog.prevSelectedUIItems, dataSourceId, dataSourceDialog._currentInstance.uiElementId, true, formDesignId, formDesignVersionId, null);
                            })
                        }
                        else {
                            if (dataSourceDialog._currentInstance.uiElementType == "Dropdown List" && isLoded == true) {
                                messageDialog.show(DocumentDesign.mapElementAlreadySavedMsg);
                            }
                            else {
                                messageDialog.show(DocumentDesign.mapElementSelectionMsg);
                            }
                        }

                    }
                }
            });
        //}
    }

    function isValid() {
        if (dataSourceDialog.uiElementIdMappCollection != undefined && dataSourceDialog.uiElementIdMappCollection.length > 0) {
            if (dataSourceDialog.uiElementIdMappCollection.isPrimaryExists()) {
                var isValid = false;
                if (!dataSourceDialog.uiElementIdMappCollection.isKeyExists(true)) {
                    messageDialog.show(DocumentDesign.isKeyMsg);
                    return false;
                }
            }
            if (dataSourceDialog.uiElementIdMappCollection.isChildExists()) {
                var isValid = false;
                if (!dataSourceDialog.uiElementIdMappCollection.isKeyExists(false)) {
                    messageDialog.show(DocumentDesign.isKeyMsg);
                    return false;
                }
            }
            return true;
        }
        else {
            messageDialog.show(DocumentDesign.primarySelectionMsg);
            return false;
        }
    }

    function setDisplayModeAndIsPrimaryForRepeater() {

        $(dataSourceDialog.elementIDs.dataSourceGridJQ).find("input:radio").each(function () {
            var rowDataSourceId = $(this).attr("id");

            if ($(this).is(":checked")) {
                var selectedDataSourceMode = ($(this).closest("tr")).find("select[name='SelectDataSourceMode']").find("option:selected").attr('id');
                dataSourceDialog.uiElementIdMappCollection.setDisplayModeAndPrimaryValues(rowDataSourceId, true, selectedDataSourceMode, 1)
            }
            else {
                var selectedDataSourceMode = ($(this).closest("tr")).find("select[name='SelectDataSourceMode']").find("option:selected").attr('id');
                var selectedDisplayMode = ($(this).closest("tr")).find('option:selected').attr('id');
                if (selectedDisplayMode == "2")
                    dataSourceDialog.uiElementIdMappCollection.setDisplayModeAndPrimaryValues(rowDataSourceId, false, selectedDataSourceMode, selectedDisplayMode)
                else
                    dataSourceDialog.uiElementIdMappCollection.setDisplayModeAndPrimaryValues(rowDataSourceId, false, 1, selectedDisplayMode)
            }
        });
    }

    function setCopyModeForDataSource(uiElementsList, dsId) {
        dataSourceDialog.uiElementIdMappCollection.setCopyMode(uiElementsList, dsId)
    }

    function setFilterForDataSource(uiElementsList, dsId) {
        dataSourceDialog.uiElementIdMappCollection.setFiltersValues(uiElementsList, dsId)
    }

    function setIsKeyForDataSource(uiElementsList, dsId) {
        dataSourceDialog.uiElementIdMappCollection.setIsKeyValues(uiElementsList, dsId)
    }

    function checkForMappedElement(uiElementsList) {
        var rowListlen = uiElementsList.length;
        for (var i = 0; i < rowListlen; i++) {

            if ((uiElementsList[i].targetElementId == 0 || uiElementsList[i].targetElementId == undefined) && (uiElementsList[i].isKey == false)) {
                messageDialog.show(DocumentDesign.mapElementSelectionMsg);
                return false;
            }
            if (uiElementsList[i].DispalyDataSourceMode == 0 && uiElementsList[i].isPrimary == true) {
                messageDialog.show(DocumentDesign.dataSourcemodeSelectionMsg);
                return false;
            }
            if (uiElementsList[i].DispalyDataSourceMode == 0 && uiElementsList[i].displayMode == "2") {
                messageDialog.show(DocumentDesign.dataSourcemodeSelectionMsg);
                return false;
            }
            if (uiElementsList[i].displayMode == "3" && uiElementsList[i].isPrimary == false) {
                messageDialog.show(DocumentDesign.primarySelectionMsg);
                return false;
            }

            //Check whether datasource 'Child or Inline' contains isKey or not.
            //DisplayMode '1' represents Inline and displayMode '2' represents Child.
            //if ((uiElementsList[i].displayMode == "1" || uiElementsList[i].displayMode == "2") && uiElementsList[i].isPrimary == false && uiElementsList[i].isKey == true) {
            //    return true;
            //}
            //if ((uiElementsList[i].displayMode == "1" || uiElementsList[i].displayMode == "2") && uiElementsList[i].isPrimary == false) {
            //    messageDialog.show(DocumentDesign.isKeyMsg);
            //    return false;
            //}
        }

        return true;
    }

    function checkForFilterElement(uiElementsList) {
        var rowListlen = uiElementsList.length;
        for (var i = 0; i < rowListlen; i++) {
            if (uiElementsList[i].dataSourceFilterOperator == 7) {
                $('#DataSourceFilter_' + uiElementsList[i].sourceElementId).val('NULL');
                uiElementsList[i].dataSourceFilterValue = 'NULL';
            }
            if (uiElementsList[i].dataSourceFilterOperator > 0 && (uiElementsList[i].dataSourceFilterValue.length == 0)) {
                messageDialog.show(DocumentDesign.filterElementSelectionMsg);
                return false;
            }
        }
        return true;
    }

    function checkIsKeyhasLabelMapping(uiElementsList) {
        var rowListlen = uiElementsList.length;
        for (var i = 0; i < rowListlen; i++) {

            if (uiElementsList[i].isKey) {
                if (uiElementsList[i].targetElementType !== 'Label') {
                    messageDialog.show(DocumentDesign.mapIsKeyWithLabelType);
                    return false;
                }
            }
        }
        return true;
    }

    //Update DataSource function on save button click
    function UpdateDatasource(uiElementsList, dataSourceId, uiElementId, isEmptyDelete, formDesignId, formDesignVersionId, existingDSIds) {
        var rowListlen = uiElementsList.length;

        var updateElements = new Array(rowListlen);

        for (var i = 0; i < rowListlen; i++) {
            var uiElementRecord = new Object();
            uiElementRecord.DataSourceId = uiElementsList[i].dataSourceId;

            uiElementRecord.UiElementId = uiElementsList[i].targetElementId;
            uiElementRecord.MappedUiElementId = uiElementsList[i].sourceElementId;
            uiElementRecord.IsPrimary = uiElementsList[i].isPrimary;
            uiElementRecord.DataSourceElementDisplayModeID = uiElementsList[i].displayMode;
            uiElementRecord.DataSourceModeID = uiElementsList[i].DispalyDataSourceMode;
            uiElementRecord.DataSourceMappingOperatorID = uiElementsList[i].dataSourceFilterOperator;
            uiElementRecord.DataSourceFilter = uiElementsList[i].dataSourceFilterValue;
            uiElementRecord.DataCopyModeID = uiElementsList[i].copyModeId;
            //uiElementRecord.IsKey = uiElementsList[i].isPrimary ? uiElementsList[i].isKey : false;
            uiElementRecord.IsKey = uiElementsList[i].isKey;

            updateElements[i] = uiElementRecord
        }


        var dataSourceUIElements = {
            uiElementRows: updateElements,
            uiElementId: uiElementId,
            tenantId: currentInstance.tenantId,
            isEmptyDelete: isEmptyDelete,
            uiElementType: dataSourceDialog._currentInstance.uiElementType,
            formDesignId: formDesignId,
            formDesignVersionId: formDesignVersionId,
            ParentUIElementID: currentInstance.uiElement.UIElementID,
            existingDataSourceIdList: existingDSIds
        };

        //save the updated form design list atached to the formgroup
        var url = dataSourceDialog.URLs.updateDataSource;
        var promise = ajaxWrapper.postJSONCustom(url, dataSourceUIElements);
        promise.done(dataSourceSuccess);
        promise.fail(showError);
    }

    function dataSourceSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(DocumentDesign.saveElementMsg);
            if (xhr.Items.length > 0) {
                if (currentInstance.uiElementType == "Dropdown List" || currentInstance.uiElementType == "Dropdown TextBox") {
                    currentInstance.dropDownDataSourceId = parseInt(xhr.Items[0].Messages[0]);
                }
            }
        }
        else if (xhr.Result === ServiceResult.FAILURE) {
            messageDialog.show(xhr.Items[0].Messages[0]);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
        currentInstance.loadDataSourceGrid(currentInstance.uiElementType, formDesignId, formDesignVersionId);
        $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).trigger('reloadGrid');
        if (currentInstance.formDesignVersionInstance != null && currentInstance.formDesignVersionInstance != undefined) {
            currentInstance.formDesignVersionInstance.buildGrid(currentInstance.uiElementId);
        }
    }

    //handler for ajax errors
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    function dataSourceActionDropDown(cellValue, options, rowObject) {
        return "<select name='SelectAction' data-rowID=" + options.rowId + " id='select_" + options.rowId + "' class='form-control'><option value='0'>Select</option><option value='1'>Create New</option><option value='2'>Use Existing</option></select>";
    }

    function dataSourceCopyModeDropDown(cellValue, options, rowObject) {
        var result = "<select name='SelectCopyMode' data-rowID=" + options.rowId + " id='selectcopy_" + options.rowId + "' class='form-control'>";
        result = result + dataSourceDialog.elementIDs.dataCopyItems;
        return result;
    }

    function unformatColumns(cellValue, options) {
        var result;
        result = $(this).find('#DataSourceFilter_' + options.rowId).val();
        return result;
    }

    function formatColumns(cellValue, options, rowObject) {
        var result;
        if (cellValue === undefined || cellValue === null) {
            cellValue = '';
        }
        result = '<input class="form-control" id="DataSourceFilter_' + options.rowId + '" style="width:90%" type="text" value="' + cellValue + '"/>';

        return result;
    }

    function dataSourceFilterOperatorFormatter(cellValue, options, rowObject) {
        var result = "<select name='SelectOperator' data-rowID=" + options.rowId + " id='filterOperator_" + options.rowId + "' class='form-control'><option value='0'>Select Operator</option>";
        result = result + dataSourceDialog.filterOperatorList;
        return result;
    }
}


dataSourceDialog.prototype.SetMappElementForSave = function (dataSourceId, selctedUiId, uiElementId, uiElementType, formDesignId, formDesignVersionId) {
    dataSourceDialog.uiElementIdMappCollection.setMappingId(dataSourceId, selctedUiId, uiElementId, uiElementType, formDesignId, formDesignVersionId)
}


var dataSourceElementsDialog = function (uiElement, formDesignVersionId, status, selectedDataSourceId, elementsData, elementType) {
    this.uiElement = uiElement;
    this.uiElementType = uiElement.ElementType;
    this.uiElementId = uiElement.UIElementID;
    this.UIElementName = uiElement.UIElementName;
    this.formDesignVersionId = formDesignVersionId;
    this.selectedDataSourceId = selectedDataSourceId;
    this.ElementData = elementsData;
    this.uiElementType = elementType;
}

dataSourceElementsDialog.elementIDs = {
    dataSourceElementsDialog: "#dataSourceElementsDialog",
    targetElementsGrid: "targetElementsGrid",
    targetElementsGridJQ: "#targetElementsGrid"
}
dataSourceElementsDialog.AllowedElementList = [
        { UIElemetType: "Textbox", MapsToList: ['Textbox', 'Multiline TextBox', 'Dropdown List', 'Dropdown TextBox', 'Label'] },
        { UIElemetType: "Checkbox", MapsToList: ['Checkbox', 'Radio Button', 'Label'] },
        { UIElemetType: "Dropdown List", MapsToList: ['Dropdown List', 'Dropdown TextBox', 'Textbox', 'Label'] },
        { UIElemetType: "Calendar", MapsToList: ['Calendar', 'Label'] },
        { UIElemetType: "Multiline TextBox", MapsToList: ['Textbox', 'Multiline TextBox', 'Label'] },
        { UIElemetType: "Dropdown TextBox", MapsToList: ['Dropdown List', 'Dropdown TextBox', 'Textbox', 'Label'] },
        { UIElemetType: "Label", MapsToList: ['Label'] },
];
dataSourceElementsDialog._isInitialized = false;

dataSourceElementsDialog._isInitialized = false;

dataSourceElementsDialog.prototype.init = function () {
    if (dataSourceElementsDialog._isInitialized == false || dataSourceElementsDialog._isInitialized == undefined) {
        $(dataSourceElementsDialog.elementIDs.dataSourceElementsDialog).dialog({
            autoOpen: false,
            width: '50%',
            modal: true
        });
        dataSourceElementsDialog._isInitialized = true;
    }
}();

dataSourceElementsDialog.prototype.show = function () {
    $(dataSourceElementsDialog.elementIDs.dataSourceElementsDialog).dialog('open');
    $(dataSourceElementsDialog.elementIDs.dataSourceElementsDialog).dialog('option', 'title', 'DataSource - ' + this.uiElement.Label + ' - Select Elements')
}

dataSourceElementsDialog.prototype.loadTargetElementsGrid = function (uiElement, dataSourceId, formDesignId, formDesignVersionId) {
    var currentInstance = this;
    $(dataSourceElementsDialog.elementIDs.targetElementsGridJQ).jqGrid('GridUnload');
    $(dataSourceElementsDialog.elementIDs.targetElementsGridJQ).parent().append("<div id='p" + dataSourceElementsDialog.elementIDs.targetElementsGrid + "'></div>");

    //set column list
    var colArray = ['', 'Include', 'Field Name', 'Element Type'];

    //set column models
    var colModel = [];
    colModel.push({ name: 'UIElementID', index: 'UIElementID', key: true, hidden: true, search: false, });
    colModel.push({ name: 'Include', index: 'Include', width: '50', align: 'center', editable: false, search: false, sortable: false, formatter: radioButton });
    colModel.push({ name: 'Label', index: 'Label', align: 'left', editable: false, sortable: false });
    colModel.push({ name: 'ElementType', index: 'ElementType', align: 'left', editable: false, sortable: false });


    //load grid
    $(dataSourceElementsDialog.elementIDs.targetElementsGridJQ).jqGrid({
        datatype: "local",
        data: currentInstance.ElementData,
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Field List',
        pager: '#p' + dataSourceElementsDialog.elementIDs.targetElementsGrid,
        rowNum: 10000,
        autowidth: true,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        altclass: 'alternate',
        gridComplete: function () {
            var rows = $(this).jqGrid('getGridParam', 'data');
            var newId;
            for (var row in rows) {
                if (rows[row].UIElementID == uiElement.MappedUIElementId) {
                    newId = rows[row].UIElementID;
                    break;
                }
            }
            $(this).jqGrid('setSelection', newId);
            var grid = this;
            $(grid).find("input:radio").each(function (idx, control) {
                $(control).click(function () {
                    if ($(control).is(":checked")) {
                        var row = $(control).attr("data-rowId");
                        $(grid).jqGrid('setSelection', row);
                    }
                });
            });
        },
        loadonce: true,
        ignoreCase: true,
        afterInsertRow: function (rowId, rowData, rowElement) {
            //get the list to check what all are the allowed items for the given element types
            var filteredList = dataSourceElementsDialog.AllowedElementList.filter(function (ct) { return ct.UIElemetType === currentInstance.uiElementType });

            //check if inserted type is allowed in the Allowed element type list
            var allowed = filteredList[0].MapsToList.filter(function (ct) { return ct == rowElement.ElementType });

            if (allowed == undefined || allowed.length == 0) {
                //if there are no entries for this, delete the records, as we can not set a data source for the destined field.
                //for example a Textbox can only be mapped to either Textbox or Dropdown List
                $(this).delRowData(rowId);
            }
        },
        onSelectRow: function (id, e) {
            var row = $(this).getRowData(id);

            $(this).find("input:radio").prop("checked", false);
            $(this).find("#radio_" + row.UIElementID).prop("checked", true);
        }

    });

    var pagerElement = '#p' + dataSourceElementsDialog.elementIDs.targetElementsGrid;
    //remove default buttons
    $(dataSourceElementsDialog.elementIDs.targetElementsGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    $(dataSourceElementsDialog.elementIDs.targetElementsGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

    //remove paging
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    $(dataSourceElementsDialog.elementIDs.targetElementsGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: 'Save',
        onClickButton: function () {
            var rowId = $(this).getGridParam('selrow');
            var row = $(this).getRowData(rowId);
            dataSourceDialog._currentInstance.mappedUIElementId = row.UIElementID;
            dataSourceDialog.uiElementIdMappCollection.setMappingId(dataSourceId, uiElement.UIElementID, dataSourceDialog._currentInstance.mappedUIElementId, row.ElementType, formDesignId, formDesignVersionId);

            //For refreshing Mapped element in 'UIElements' grid
            var parentRowID = $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).getGridParam('selrow');
            var parentrow = $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).getRowData(parentRowID);

            $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid('setCell', parentRowID, 'MappedUIElementName', row.Label);
            $("#select_" + parentrow.UIElementID).val(0);

            $(dataSourceElementsDialog.elementIDs.dataSourceElementsDialog).dialog('close');
        }
    });

    function dataSourceMappingSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(DocumentDesign.saveMappingMsg);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
    }

    //handler for success
    function erroExists(xhr) {
        alert(JSON.stringify(xhr));
    }

    //handler for ajax errors
    function successExists(xhr) {

    }

    //handler for ajax errors
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
}

var addDataSourceFieldDialog = function () {
    var URLs = {
        fieldAdd: '/UIElement/AddElement',
        fieldUpdate: '/UIElement/UpdateFieldName'
    }

    var elementIDs = {
        fieldListDialog: '#addDataSourceFieldDialog',
        fieldListDialogGridJQ: '#addDataSourceFieldDialogGrid',
        fieldListDialogGrid: 'addDataSourceFieldDialogGrid',
        fieldListDropDown: '#addDataSourceFieldlist'
    };

    var fieldDialogState;
    var formDesignVersionId;
    var tenantId;
    var parentUIElementId;
    var parentObject;
    var uiElementType;
    var selectedUiId;
    var dsId;
    var dataSourceInstance;
    var formDesignId;

    function fieldSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(DocumentDesign.saveFieldMsg);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
        dataSourceInstance.SetMappElementForSave(dsId, selectedUiId, xhr.Items[0].Messages[0], xhr.Items[1].Messages[0], formDesignId, formDesignVersionId);
        if (parentObject != undefined)
            parentObject.loadGrid();
        $(elementIDs.fieldListDialog + ' div').removeClass('has-error');
        $(elementIDs.fieldListDialog + ' .help-block').text(Common.fieldUniqueNameMsg);
        $(elementIDs.fieldListDialog).dialog('close');
    }

    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    function init() {
        //register dialog for grid row add/edit
        $(elementIDs.fieldListDialog).dialog({
            autoOpen: false,
            width: 450,
            modal: true
        });
        $(elementIDs.fieldListDialog + ' button').on('click', function () {
            //check if name is already used
            var elementType = $(elementIDs.fieldListDialog + ' select').val();
            var newName = $(elementIDs.fieldListDialog + ' textarea').val();
            var startWithAlphabet = new RegExp(CustomRegexValidation.STARTWITHAPLHABETS);
            var fieldList = $(elementIDs.fieldListDialogGridJQ).getRowData();
            var filterList;
            if (elementType != '[Blank]') {
                filterList = fieldList.filter(function (ct) {
                    return compareStrings(ct.Label, newName, true);
                });
            }

            else {
                newName = 'Blank';
            }
            //For refreshing Mapped element in 'UIElements' grid
            var parentRowID = $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).getGridParam('selrow');
            var parentrow = $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).getRowData(parentRowID);
            $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).jqGrid('setCell', parentRowID, 'MappedUIElementName', newName);
            $("#select_" + parentrow.UIElementID).val(0);

            //validate form name
            if (filterList !== undefined && filterList.length > 0) {
                $(elementIDs.fieldListDialog + ' div').addClass('has-error');
                $(elementIDs.fieldListDialog + ' .help-block').text(Common.fieldNameExistsMsg);
            }
            else if (newName == '' && elementType != '[Blank]') {
                $(elementIDs.fieldListDialog + ' div').addClass('has-error');
                $(elementIDs.fieldListDialog + ' .help-block').text(Common.fieldNameRequiredMsg);
            }
            else if (!(newName.match(startWithAlphabet))) {
                $(elementIDs.fieldListDialog + ' div').addClass('has-error');
                $(elementIDs.fieldListDialog + ' .help-block').text(DocumentDesign.nameValidateMsg);

            }
            else {
                var rowId = $(elementIDs.fieldListDialogGridJQ).getGridParam('selrow');
                var fieldAdd = {
                    TenantID: tenantId,
                    FormDesignVersionID: formDesignVersionId,
                    ParentUIElementID: parentUIElementId,
                    UIElementID: rowId,
                    ElementType: elementType,
                    Label: newName
                };
                var url;
                if (fieldDialogState === 'add') {
                    url = URLs.fieldAdd;
                }
                else {
                    url = URLs.fieldUpdate;
                }
                var promise = ajaxWrapper.postJSONCustom(url, fieldAdd);
                promise.done(fieldSuccess);
                promise.fail(showError);
            }
        });
    }
    init();

    return {
        show: function (fieldName, action, tenantID, formDesignID, formDesignVersionID, parentUIElementID, parent, elementType, dataSourceDialogInstance, selectedUiElementId, isKey, dataSourceId) {
            tenantId = tenantID;
            formDesignId = formDesignID;
            formDesignVersionId = formDesignVersionID;
            parentUIElementId = parentUIElementID;
            fieldDialogState = action;
            parentObject = parent;
            uiElementType = elementType;
            dataSourceInstance = dataSourceDialogInstance;
            selectedUiId = selectedUiElementId;
            dsId = dataSourceId;
            //clears all elements in field list drop down
            $(elementIDs.fieldListDropDown).empty();
            if (isKey) {
                $(elementIDs.fieldListDropDown).append("<option>" + 'Label' + "</option>");
            }
            else {
                var elemnetList = [
                            { UIElemetType: "Textbox", MapsToList: ['Textbox', 'Multiline TextBox', 'Dropdown List', 'Dropdown TextBox', 'Label'] },
                            { UIElemetType: "Radio Button", MapsToList: ['Radio Button', 'Checkbox', 'Label'] },
                            { UIElemetType: "Checkbox", MapsToList: ['Checkbox', 'Radio Button', 'Label'] },
                            { UIElemetType: "Dropdown List", MapsToList: ['Dropdown List', 'Dropdown TextBox', 'Textbox', 'Label'] },
                            { UIElemetType: "Calendar", MapsToList: ['Calendar', 'Label'] },
                            { UIElemetType: "Multiline TextBox", MapsToList: ['Textbox', 'Multiline TextBox', 'Dropdown List', 'Dropdown TextBox', 'Label'] },
                            { UIElemetType: "Dropdown TextBox", MapsToList: ['Dropdown List', 'Dropdown TextBox', 'Textbox', 'Label'] },
                            { UIElemetType: "Label", MapsToList: ['Label'] },
                ];

                var filteredList = elemnetList.filter(function (ct) { return ct.UIElemetType === elementType });

                for (var i = 0; i < filteredList[0].MapsToList.length; i++) {
                    $(elementIDs.fieldListDropDown).append("<option>" + filteredList[0].MapsToList[i] + "</option>");
                }
            }

            $(elementIDs.fieldListDialog + ' textarea').each(function () {
                $(this).val(fieldName);
            });
            $(elementIDs.fieldListDialog + ' div').removeClass('has-error');
            if (fieldDialogState == 'add') {
                $(elementIDs.fieldListDialog).dialog('option', 'title', 'Add Field');
                $(elementIDs.fieldListDialog + ' .help-block').text(Common.fieldUniqueNameMsg);
                $(elementIDs.fieldListDialog + ' button').text('Add');
            }
            else {
                $(elementIDs.fieldListDialog).dialog('option', 'title', 'Edit Field');
                $(elementIDs.fieldListDialog + ' .help-block').text(Common.fieldEditNameValidateMsg);
                $(elementIDs.fieldListDialog + ' button').text('Edit');
            }
            $(elementIDs.fieldListDialog).dialog("open");
        }
    }
}();

var uiElementIdMapper = function (formDesignId, formDesignVersionId, dsId, uiElementId, mappingElementId, copyModeId, dataSourceFilterOperator,
    dataSourceFilterValue, isKey, targetElementType, isPrimary, displayMode) {
    this.dataSourceId = dsId;
    this.sourceElementId = uiElementId;
    this.targetElementId = mappingElementId;
    this.displayMode = displayMode;
    this.isPrimary = isPrimary;
    this.formDesignId = formDesignId;
    this.formDesignVersionId = formDesignVersionId;
    this.copyModeId = copyModeId;
    this.dataSourceFilterOperator = dataSourceFilterOperator;
    this.dataSourceFilterValue = dataSourceFilterValue;
    this.isKey = isKey;
    this.targetElementType = targetElementType;
}

function createCollection(ClassName) {
    var obj = new Array();
    eval("var t=new " + ClassName + "()");
    for (_item in t) {
        eval("obj." + _item + "=t." + _item);
    }
    return obj;
}

function uiElementCollection() {
    this.count = 0;

    this.add = function (obj) {
        this.push(obj);
        return ++this.count
    }

    this.remove = function (dataSourceId, uiElementId, formDesignId, formDesignVersionId) {

        for (var i = 0; i < this.length; i++) {
            if (this[i].dataSourceId == dataSourceId && this[i].sourceElementId == uiElementId && this[i].formDesignId == formDesignId && this[i].formDesignVersionId == formDesignVersionId) {
                this.splice(i, 1);
                return --this.count
            }
        }
    }

    this.removeAll = function () {
        for (var i = 0; i < this.length; i++) {
            this.splice(i, 1);
        }
        return this.count = 0;
    }

    this.removeAllFromDataSourceId = function (dataSourceId) {
        var uiElementMappedList = this;

        for (var i = 0, j = 0; i <= this.length ; i++, j++) {
            if (this[j]) {
                if (this[j].dataSourceId == dataSourceId) {
                    //removes an item here
                    uiElementMappedList.splice(j, 1);
                    j = j - 1;
                }

            }
        }
        return this;
    }

    this.setMappingId = function (dataSourceId, uiElementId, mappingId, mappingElementType, formDesignId, formDesignVersionId) {
        for (var i = 0; i < this.length; i++) {
            if (this[i].dataSourceId == dataSourceId && this[i].sourceElementId == uiElementId && this[i].formDesignId == formDesignId && this[i].formDesignVersionId == formDesignVersionId) {
                this[i].targetElementId = mappingId;
                this[i].targetElementType = mappingElementType;
            }
        }
    }

    this.isExists = function (dataSourceId, uiElementId, formDesignId, formDesignVersionId) {
        for (var i = 0; i < this.length; i++) {
            if (this[i].dataSourceId == dataSourceId && this[i].sourceElementId == uiElementId && this[i].formDesignId == formDesignId && this[i].formDesignVersionId == formDesignVersionId)
                return true;
        }

        return false;
    }

    this.getAlreadyExistingDataSourceId = function () {
        var alreadyExistingDataSourceIds = new Array();
        for (var i = 0; i < this.length; i++) {
            if (alreadyExistingDataSourceIds.indexOf(this[i].dataSourceId) < 0)
                alreadyExistingDataSourceIds.push(this[i].dataSourceId);

        }
        return alreadyExistingDataSourceIds;
    }

    this.isPrimaryExists = function () {
        for (var i = 0; i < this.length; i++) {
            if (this[i].displayMode == "3" && (this[i].isPrimary == "N" || this[i].isPrimary == 1))
                return true;
        }
        return false;
    }
    this.isChildExists = function () {
        for (var i = 0; i < this.length; i++) {
            if (this[i].displayMode == "2" && (this[i].isPrimary == "N" || this[i].isPrimary == false))
                return true;
        }
        return false;
    }

    this.isKeyExists = function (displayMode) {
        for (var i = 0; i < this.length; i++) {
            if ((this[i].isKey == "Y" || this[i].isKey == 1) && this[i].isPrimary == displayMode)
                return true;
        }
        return false;
    }

    this.setDisplayModeAndPrimaryValues = function (dsId, isPrimaryVal, dataSourceMode, mode) {
        for (var i = 0; i < this.length; i++) {
            if (this[i].dataSourceId == dsId) {
                this[i].isPrimary = isPrimaryVal;
                this[i].DispalyDataSourceMode = dataSourceMode;
                if (isPrimaryVal == 1)
                    this[i].displayMode = 3;
                else
                    this[i].displayMode = mode;

            }
        }

    }

    this.setCopyMode = function (uielement, dsId) {
        for (var i = 0; i < this.length; i++) {
            for (var j = 0; j < uielement.length; j++) {
                if (this[i].sourceElementId == uielement[j].UIElementID && uielement[j].Include == true) {
                    this[i].copyModeId = $('#selectcopy_' + uielement[j].UIElementID + ' option:selected').val();
                }
            }
        }
    }

    this.setFiltersValues = function (uielement, dsId) {
        for (var i = 0; i < this.length; i++) {
            for (var j = 0; j < uielement.length; j++) {
                if (this[i].sourceElementId == uielement[j].UIElementID && uielement[j].Include == true) {
                    this[i].dataSourceFilterOperator = $('#filterOperator_' + uielement[j].UIElementID + ' option:selected').val();
                    this[i].dataSourceFilterValue = uielement[j].DataSourceFilterValue;
                }
            }
        }
    }

    this.setIsKeyValues = function (uielement, dsId) {
        for (var i = 0; i < this.length; i++) {
            for (var j = 0; j < uielement.length; j++) {
                if (this[i].sourceElementId == uielement[j].UIElementID && uielement[j].Include == true) {
                    this[i].isKey = $('#IsKey_' + uielement[j].UIElementID).prop('checked');
                }
            }
        }
    }


}


var unMappedElementsDialog = function (uiElement, formDesignVersionId, status, elementsData) {
    this.uiElement = uiElement;
    this.uiElementType = uiElement.ElementType;
    this.uiElementId = uiElement.UIElementID;
    this.UIElementName = uiElement.UIElementName;
    this.formDesignVersionId = formDesignVersionId;
    this.ElementData = elementsData;
}


unMappedElementsDialog.elementIDs = {
    dsUnMappedElementsDialog: "#dataSourceElementsDialog",
    dsUnMappedElementsGrid: "targetElementsGrid",
    dsUnMappedElementsGridJQ: "#targetElementsGrid"
}

//unformat the grid column based on element property
unMappedElementsDialog.prototype.unFormatIncludedColumn = function (cellValue, options) {
    var result;
    result = $(this).find('#' + options.rowId).find('input').prop('checked');
    return result;
}

unMappedElementsDialog._isInitialized == false;

unMappedElementsDialog.prototype.init = function () {
    if (unMappedElementsDialog._isInitialized == false || unMappedElementsDialog._isInitialized == undefined) {
        $(unMappedElementsDialog.elementIDs.dsUnMappedElementsDialog).dialog({
            autoOpen: false,
            width: '50%',
            modal: true
        });
        unMappedElementsDialog._isInitialized = true;
    }
}();

unMappedElementsDialog.prototype.show = function () {
    $(unMappedElementsDialog.elementIDs.dsUnMappedElementsDialog).dialog('open');
    $(unMappedElementsDialog.elementIDs.dsUnMappedElementsDialog).dialog('option',
        'title', 'DataSource UnMapped- ' + this.uiElement.Label + ' - Select Elements');
}

unMappedElementsDialog.prototype.loadTargetElementsGrid = function (dataSourceId, formDesignId, formDesignVersionId) {
    var currentInstance = this;
    $(unMappedElementsDialog.elementIDs.dsUnMappedElementsGridJQ).jqGrid('GridUnload');
    $(unMappedElementsDialog.elementIDs.dsUnMappedElementsGridJQ).parent().append("<div id='p" + unMappedElementsDialog.elementIDs.dsUnMappedElementsGrid + "'></div>");

    //set column list
    var colArray = ['', 'Include', 'Field Name', 'Element Type'];

    //set column models
    var colModel = [];
    colModel.push({ name: 'UIElementID', index: 'UIElementID', key: true, hidden: true, search: false, });
    colModel.push({
        name: 'Include', index: 'IsCurrentDS', width: '50', align: 'center', editable: false, sortable: false, formatter: checkBoxFormatter, unformat: currentInstance.unFormatIncludedColumn,
        editType: 'checkbox',
        editoptions: { value: 'Yes:No', defaultValue: 'No' },
        //stype: 'select',
        //searchoptions: {
        //    sopt: ['eq', 'ne'],
        //    value: ':Any;Y:Yes;N:No',
        //}
    });
    colModel.push({ name: 'Label', index: 'Label', align: 'left', editable: false, sortable: false });
    colModel.push({ name: 'ElementType', index: 'ElementType', align: 'left', editable: false, sortable: false });


    //load grid
    $(unMappedElementsDialog.elementIDs.dsUnMappedElementsGridJQ).jqGrid({
        datatype: "local",
        data: currentInstance.ElementData,
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Field List',
        pager: '#p' + unMappedElementsDialog.elementIDs.dsUnMappedElementsGrid,
        rowNum: 10000,
        autowidth: true,
        viewrecords: true,
        ignoreCase: true,
        hidegrid: false,
        altRows: true,
        altclass: 'alternate',
        gridComplete: function () {
            var rows = $(this).jqGrid('getGridParam', 'data');

            var grid = this;

            $(grid).find("input:checkbox").each(function (idx, control) {
                $(control).click(function () {
                    if ($(control).is(":checked")) {
                        var row = $(control).attr("id");
                        $(grid).jqGrid('setSelection', row);
                    }
                });
            });


            //$(grid).find("input:radio").each(function (idx, control) {
            //    $(control).click(function () {
            //        if ($(control).is(":checked")) {
            //            var row = $(control).attr("data-rowId");
            //            $(grid).jqGrid('setSelection', row);
            //        }
            //    });
            //});
        },

        //afterInsertRow: function (rowId, rowData, rowElement) {
        //    //get the list to check what all are the allowed items for the given element types
        //    var filteredList = unMappedElementsDialog.AllowedElementList.filter(function (ct) { return ct.UIElemetType === currentInstance.uiElementType });

        //    //check if inserted type is allowed in the Allowed element type list
        //    var allowed = filteredList[0].MapsToList.filter(function (ct) { return ct == rowElement.ElementType });

        //    if (allowed == undefined || allowed.length == 0) {
        //        //if there are no entries for this, delete the records, as we can not set a data source for the destined field.
        //        //for example a Textbox can only be mapped to either Textbox or Dropdown List
        //        $(this).delRowData(rowId);
        //    }
        //},
        onSelectRow: function (id, e) {
            var row = $(this).getRowData(id);

            $(this).find("input:checkbox").prop("checked", false);
            $(this).find("#radio_" + row.UIElementID).prop("checked", true);
        }

    });

    var pagerElement = '#p' + unMappedElementsDialog.elementIDs.dsUnMappedElementsGrid;
    //remove default buttons
    $(unMappedElementsDialog.elementIDs.dsUnMappedElementsGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    $(unMappedElementsDialog.elementIDs.dsUnMappedElementsGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

    //remove paging
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    $(unMappedElementsDialog.elementIDs.dsUnMappedElementsGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: 'Save',
        onClickButton: function () {
            var allRowsData = $(this).getRowData();
            $.each(allRowsData, function (i, item) {
                if (item.Include == true) {
                    if (dataSourceDialog.uiElementIdMappCollection === undefined)
                        dataSourceDialog.uiElementIdMappCollection = new createCollection("uiElementCollection");

                    //check if Main grid already added the UIElementID, if not added then only add to main grid
                    var elementsData = $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).getRowData(item.UIElementID);

                    if (elementsData != null && elementsData.UIElementID === undefined) {

                        dataSourceDialog.uiElementIdMappCollection.add(new uiElementIdMapper(formDesignId, formDesignVersionId, dataSourceId, item.UIElementID, item.UIElementID));

                        dataSourceDialog.uiElementIdMappCollection.setMappingId(dataSourceId, item.UIElementID,
                            item.UIElementID, item.ElementType, formDesignId, formDesignVersionId);

                        $(dataSourceDialog.elementIDs.dataSourceElementsGridJQ).addRowData(item.UIElementID,
                                { UIElementID: item.UIElementID, Include: true, Label: item.Label, ElementType: item.ElementType, MappedUIElementId: item.UIElementID, MappedUIElementName: item.Label, DataCopyModeID: 1 });

                    }
                }
            });
            $(unMappedElementsDialog.elementIDs.dsUnMappedElementsDialog).dialog('close');
        }
    });

    function dataSourceMappingSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(DocumentDesign.saveMappingMsg);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
    }

    //handler for success
    function erroExists(xhr) {
        alert(JSON.stringify(xhr));
    }

    //handler for ajax errors
    function successExists(xhr) {

    }

    //handler for ajax errors
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
}







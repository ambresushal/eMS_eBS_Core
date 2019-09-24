var controType =
    {
        textBox: "textBox",
        checkBox: "checkBox",
        dropDown: "dropDown",
        multiTextBox: "multiTextBox",
        dropDownText: "dropDownText",
        multiSelect: "multiSelect",
        calender: "calender",
        richTextBox: "richTextBox"
    };


var GridApi = function () {

    var columnDefintion = function () {
        return columnDef;
    };
    var gridGlobalSettingOption = function () {
        return new gridGlobalSetting();
    };
    var getFilteredGridData = function (currentInstance) {
        var currentGridInstance = {};
        var gridInstance = $(currentInstance.gridElementIdJQ)[0];
        currentGridInstance.gridOptions = gridInstance.gridOptions;
        currentGridInstance.colModel = gridInstance.colModel;
        var filteredData = gridInstance.gridOptions.api.getModel().rowsToDisplay;
        var rowData = [];
        $.each(filteredData, function (i, row) {
            rowData.push(row.data);
        });
        currentGridInstance.filteredRows = rowData;
        return currentGridInstance;
    };


    var getCurrentGridInstance = function (currentInstance) {
        var currentGridInstance = {};
        var gridInstance = $(currentInstance.gridElementIdJQ)[0];
        currentGridInstance.gridOptions = gridInstance.gridOptions;
        currentGridInstance.colModel = gridInstance.colModel;
        return currentGridInstance;
    };
    var getCurrentGridInstanceById = function (repeaterID) {
        var currentGridInstance = {};
        var gridInstance;

        if (repeaterID.indexOf('#') != -1) {
            gridInstance = $(repeaterID)[0]
        }
        else {
            gridInstance = $("#" + repeaterID)[0];
        }
        currentGridInstance.gridOptions = gridInstance.gridOptions;
        currentGridInstance.colModel = gridInstance.colModel;
        return currentGridInstance;
    };

    var setCellFocus = function (currentGridInstance, rowNodeFound, colName, colindex) {

        currentGridInstance.gridOptions.cellStyle = { rowIndex: rowNodeFound.rowIndex, colId: colName }

        currentGridInstance.gridOptions.api.setFocusedCell(rowNodeFound.rowIndex, colName, null);
        currentGridInstance.gridOptions.api.refreshCells();

        var pageSize = currentGridInstance.gridOptions.api.paginationGetPageSize();
        var goToPageNo = 0;
        if (rowNodeFound.rowIndex > 0) {
            var goToPageNo = (rowNodeFound.rowIndex / pageSize);
            goToPageNo = parseFloat(goToPageNo.toString().split(".")[0]);
        }
        currentGridInstance.gridOptions.api.paginationGoToPage(goToPageNo);
        //   rowNodeFound.setSelected(true, true);
        currentGridInstance.gridOptions.api.ensureIndexVisible(rowNodeFound.rowIndex);
        currentGridInstance.gridOptions.api.ensureNodeVisible(rowNodeFound, colindex);


    }
    var setRowSectionOnNavigation = function (currentInstance, isNext) {
        var gridOptions = getCurrentGridInstance(currentInstance).gridOptions;

        var rowCount = gridOptions.api.getDisplayedRowCount() - 1;

        var selectedNodes = gridOptions.api.getSelectedNodes();

        if (selectedNodes.length > 0) {
            var currentIndex = selectedNodes[0].rowIndex;

            var lastGridIndex = rowCount - 1;
            var currentPage = gridOptions.api.paginationGetCurrentPage();
            var pageSize = gridOptions.api.paginationGetPageSize();
            var startPageIndex = currentPage * pageSize;

            var currentPg = currentPage;

            if (isNext == true) {
                currentPg++;
            }

            var endPageIndex = (currentPg * pageSize) - 1;

            if (isNext == true) {
                if (currentIndex <= rowCount) {
                    endPageIndex++;
                    currentIndex++;
                }
            }
            else

                if (currentIndex > 0) {
                    currentIndex--;
                }
            var nextRowData = gridOptions.api.getDisplayedRowAtIndex(currentIndex)


            if (endPageIndex == currentIndex) {

                if (isNext == true) {
                    currentPage++;
                }
                else {
                    currentPage--;
                }
                gridOptions.api.paginationGoToPage(currentPage);
            }

            nextRowData.setSelected(true, true);
            gridOptions.api.ensureIndexVisible(currentIndex);
            return nextRowData;
        }
    }

    var setPaginationOption = function (pageSize, size) {
        var selected = "selected";
        var optionHtml = '';
        if (pageSize == size) {
            optionHtml = '<option value="' + size + '" ' + selected + ' >' + size + '</option>'
        }
        else {
            optionHtml = '<option value="' + size + '">' + size + '</option>'
        }
        return optionHtml
    }



    var addPaginationPanel = function (gridOptions, row) {
        var currentInstance = this;

        td = document.createElement("TD");
        cell = document.createElement("SPAN");
        cell.innerText = "Page Size";

        var div = document.createElement("select");

        var pageSize = gridOptions.api.paginationGetPageSize();

        var optionHtml = '';
        div.innerHTML = setPaginationOption(pageSize, 10) +
                        setPaginationOption(pageSize, 20) +
                        setPaginationOption(pageSize, 50) +
                        setPaginationOption(pageSize, 100)



        div.onchange = function () {
            gridOptions.api.paginationSetPageSize(Number(this.value));
        };

        cell.appendChild(div);

        td.appendChild(cell);
        row.appendChild(td);
    }


    var renderGridHeaderRow = function (gridElementIdID, headerLabel, gridOptions) {
        var table = document.createElement("TABLE");
        table.className = "ag-ltr";//"customPager";
        var row = document.createElement("TR");
        row.className = "customPager";
        var td = document.createElement("TD");
        var cell = document.createElement("SPAN");

        td.appendChild(cell);
        row.appendChild(td);

        var currentInstance = this;
        td = document.createElement("TD");
        cell.title = headerLabel;
        cell = document.createElement("SPAN");
        cell.className = "pq-grid-title ui-corner-top";
        cell.innerText = headerLabel;
        td.appendChild(cell);
        row.appendChild(td);

        addPaginationPanel(gridOptions, row);

        table.appendChild(row);

        $(gridElementIdID).find('.ag-paging-panel').prepend(table);

    }
    return {
        columnDef: columnDefintion,
        getCurrentGridInstance: getCurrentGridInstance,
        getCurrentGridInstanceById: getCurrentGridInstanceById,
        setRowSectionOnNavigation: setRowSectionOnNavigation,
        setCellFocus: setCellFocus,
        gridGlobalSetting: gridGlobalSettingOption,
        addPaginationPanel: addPaginationPanel,
        renderGridHeaderRow: renderGridHeaderRow,
        getFilteredGridData: getFilteredGridData
    };
}();

var columnDef = (function () {
    this.filterMode;
    this.userInfo;

    var prepare = function (columnSettings, filterMode, userInfo) {
        this.filterMode = filterMode;
        this.userInfo = userInfo;

        var columnDefs = [firstColumn()];
        columnDefs.push()
        $.each(columnSettings, function (key, setting) {
            var item = setHeader(setting);
            columnDefs.push(item);
        });
        return columnDefs;
    };

    var isNull = function (value) {
        if (value == null || value === 'undefined')
            return true
        else
            return false;
    }

    var firstColumn = function () {
        return {
            headerName: "", //width: "50px",
            width: 60,
            maxWidth: 70,
            // it is important to have node.id here, so that when the id changes (which happens
            // when the row is loaded) then the cell is refreshed.
            valueGetter: 'node.rowIndex',
            suppressFilter: true,
            suppressMenu: true,
            pinned: 'left',
            resizable: false,
            suppressSizeToFit: true,
            cellRenderer: function (params) {
                if (params.rowIndex !== undefined) {
                    return (params.rowIndex + 1).toString();
                } else {
                    return '<img src="../images/loading.gif">'
                }
            }
        }
    }
    var getOptionValuesWithCommaSep = function (jsonOptionsValue) {
        var optionsValue = {};
        $.each(jsonOptionsValue, function (i, item) {
            for (var key in item) {
                if (key == '[Select One]')
                { optionsValue[0] = key }
                else {
                    optionsValue[key] = item[key];
                }
            }
        });
        return optionsValue;


        var optionsValue = {};
        /*
 
         $.each(jsonOptionsValue, function (i, item) {
             for (var key in item) {
                 if (key !== '[Select One]')
                 {
                     optionsValue[key] = item[key];
                 }
             }
         });
         if (jsonOptionsValue.length > 0) {
             if (Object.keys(jsonOptionsValue[0]).length > 0) {
                 var key = (Object.keys(jsonOptionsValue[0])[0]);
 
                 if ((key == "[Select One]")) {
                     optionsValue[key] = key
                 }
             }
         }*/
        return optionsValue
    }
    var setHeader = function (columnSetting) {
        //console.log(columnSetting);
        var item = {};
        item["headerName"] = columnSetting.title;
        item["field"] = columnSetting.dataIndx;
        item["headerTooltip"] = columnSetting.toolTip;
        if (!isNull(columnSetting.width)) {
            item["width"] = columnSetting.width;
        }

        item["hide"] = columnSetting.hidden;
        //item["suppressMenu"] = true;

        item["editable"] = columnSetting.editable;

        item["cellStyle"] = {
            'white-space': 'normal'
        };

        item["cellClassRules"] = {
            'has-error': function (params) {
                var cellStyle = params.node.gridOptionsWrapper.gridOptions.cellStyle;

                if (cellStyle != undefined) {
                    if (cellStyle != null) {
                        if (params.data[cellStyle.colId] == params.value
                            && params.rowIndex == cellStyle.rowIndex) {
                            params.node.gridOptionsWrapper.gridOptions.cellStyle = null;

                            return true;
                        }
                    }
                }
                return false;
            },
            'repeater-has-error': function (params) {
                var returnValue = false;
                var cellStyle = params.node.gridOptionsWrapper.gridOptions.cellErrorStyle;

                if (cellStyle != undefined) {
                    if (cellStyle != null) {
                        $.each(cellStyle, function (i, el) {
                            if (this.colId === params.colDef.field
                                && params.node.data.RowIDProperty === this.rowIndex) {
                                // params.node.gridOptionsWrapper.gridOptions.cellErrorStyle = null;

                                returnValue = true;
                            }
                        });
                    }
                }
                return returnValue;
            },
            'ag-cellHighlight': function (params) {
                var returnValue = false;
                var cellStyle = params.node.gridOptionsWrapper.gridOptions.cellHighlightStyle;

                if (cellStyle != undefined) {
                    if (cellStyle != null) {
                        $.each(cellStyle, function (i, el) {
                            if (this.colId === params.colDef.field
                                && params.node.data.RowIDProperty === this.rowIndex) {
                                returnValue = true;
                            }
                        });
                    }
                }
                return returnValue;
            },
            'ag-cellDisable': function (params) {
                var returnValue = false;
                var cellStyles = params.node.gridOptionsWrapper.gridOptions.currentInstance.cellRules;


                if (cellStyles != undefined) {
                    var cellStyle = cellStyles.cellEnableStyle;
                    if (cellStyle != null) {
                        $.each(cellStyle, function (i, el) {
                            if (this.colId === params.colDef.field
                                && params.node.data.RowIDProperty === this.rowIndex) {
                                // params.node.gridOptionsWrapper.gridOptions.cellErrorStyle = null;
                                returnValue = true;
                            }
                        });
                    }
                }
                return returnValue;
            }
            /*,
            'rag-amber-outer': function (params)
            {
                var value = params.colDef.context === 'error';
                params.colDef.context = null;
                return value;
            },
            'rag-red-outer': function (params)
            {
                var value = params.colDef.context === 'error';
                params.colDef.context = null;
                return value;
            }*/
        };


        if (columnSetting.dataIndx == "expandCollapse" ||
            ((currentInstance.fullName == "BenefitReview.BenefitReviewGrid"
            ||
            currentInstance.fullName == "BenefitReview.BenefitReviewSlidingCostShare.BenefitReviewGridSlidingCostShare"
            ) && columnSetting.dataIndx == "Limits"))
            {
            item["cellRenderer"] = columnSetting.render;
        }
        else {
            item["cellRenderer"] = function (params) {
                var toolTip = $('<div/>').html(params.value).text();
                if (params.value === undefined) {
                    return "";
                }
                else {
                    return '<span title="' + toolTip + '" style="width:80px;">' + params.value + '</span>';
                }
            }
        }

        if (columnSetting.edittype != undefined && columnSetting.editor.cls != undefined) {
            if (columnSetting.editor.cls == "ddt-dropdown") {
                {
                    item["cellStyle"] = highlightDropdownTextBox;
                }
            }
        }


        if (columnSetting.dataType == 'bool') {
            item["cellRenderer"] = 'booleanCellRenderer';
            item["cellStyle"] = { "text-align": "center" };
            item["comparator"] = booleanComparator;
            item["floatCell"] = true;
        }
        else if (columnSetting.dataType == 'float') {
            item["comparator"] = floatComparator;
        }

        //   console.log(columnSetting);
        if (!isNull(columnSetting.editor)) {
            if (!isNull(columnSetting.edittype) && (columnSetting.editable == true || (columnSetting.editable == false && columnSetting.edittype == 'select'))) {

                var editType = columnSetting.edittype;
                if (columnSetting.dataType == 'date')
                    editType = 'calendar';


                switch (editType) {
                    case 'select':
                        var mappingValues = getOptionValuesWithCommaSep(columnSetting.editor.options);
                        if (columnSetting.editor.attr == "multiple='multiple'") {
                            item["cellEditor"] = getMultiSelect();
                            item["cellEditorParams"] = {
                                values: columnSetting.editor.options
                            };
                        }
                        else {

                            item["cellEditor"] = getDropdown();//"agSelectCellEditor";

                            if (columnSetting.editor.cls == "ddt-dropdown") {
                                item["cellEditor"] = getDropdownTextbox();
                            }
                            item["cellEditorParams"] = {
                                values: columnSetting.editor.options
                            };
                        }

                        item["valueSetter"] = function (params) {
                            var newValueId = lookupKey(mappingValues, params.newValue, params.colDef.cellEditor);
                            params.data[params.column.colId] = newValueId;
                        };

                        item["valueGetter"] = function (params) {
                            var val = lookupValue(mappingValues, params.data[params.column.colId], params.colDef.cellEditor);
                            return val;
                        };

                        item["valueFormatter"] = function (params) {
                            var val = lookupValue(mappingValues, params.value, params.colDef.cellEditor);
                            return val;
                        };
                        item["valueParser"] = function (params) {
                            return lookupKey(mappingValues, params.newValue, params.colDef.cellEditor);
                        };
                        break;
                    case 'text':

                        if (columnSetting.editor.IsRichTextBox == true) {

                            /*  item["cellRendererSelector"] = function (params) {
                                  return {
                                      component: 'richTextBoxCellRenderer'
                                  };
                              };*/
                            item["cellRenderer"] = 'richTextBoxCellRenderer';
                            item["cellEditor"] = getRichTextBox();
                            item["cellEditorParams"] = {
                                userInfo: columnDef.userInfo
                            };
                            //item["editable"] = false;
                        }
                        else if (columnSetting.Multiline == true) {
                            item["cellEditor"] = getLargeTextCellEditor();//"agLargeTextCellEditor";
                        }
                        else {
                            item["cellEditor"] = getTextBox();//"agTextCellEditor";
                            item["cellEditorParams"] = {
                                dataType: columnSetting.dataType
                            }
                        }
                        break;
                    case 'textarea':
                        item["cellEditor"] = getLargeTextCellEditor();// "agLargeTextCellEditor";
                        item["cellRenderer"] = 'richTextBoxCellRenderer';
                        break;
                    case 'calendar':
                        item["cellEditor"] = getDatePicker();
                        break;
                    case 'checkbox':
                        item["cellEditor"] = getCheckBoxPicker();
                        break;
                }
            }
        }
        item["filter"] = 'agSetColumnFilter'
        var filterParams = {};


        item["filterParams"] = { selectAllOnMiniFilter: true, clearButton: true, applyButton: true, newRowsAction: 'keep' };

        //  if (currentcolumnDef.currentInstance.design.FilterMode.toLowerCase() == "local filtering" || currentcolumnDef.currentInstance.design.FilterMode.toLowerCase() == "both") {
        if (columnDef.filterMode == null) {
            columnDef.filterMode = "";
        }

        if (columnSetting.lockPinned !== undefined) {
            item["pinned"] = columnSetting.pinned,
            item["lockPinned"] = columnSetting.lockPinned,
            item["cellClass"] = columnSetting.cellClass
        }
        if (columnSetting.userSettingData !== undefined || columnSetting.userSettingConst !== undefined) {

            if (columnSetting.userSettingData[columnSetting.userSettingConst.filter] == columnSetting.userSettingConst.FilterByCondition) {
                columnDef.filterMode = "local filtering";
            }
            else if (columnSetting.userSettingData[columnSetting.userSettingConst.filter] == columnSetting.userSettingConst.FilterByWildcard) {
                columnDef.filterMode = "both";
                item["floatingFilterComponentParams"] = { suppressFilterButton: true }
            }
            else {
                columnDef.filterMode = "";
            }

            item["filterParams"].caseSensitive = booleanCleaner(columnSetting.userSettingData[columnSetting.userSettingConst.casesensitive]);

            item["suppressMenu"] = !booleanCleaner(columnSetting.userSettingData[columnSetting.userSettingConst.menu]);
        }
        if (columnDef.filterMode.toLowerCase() == "local filtering" || columnDef.filterMode.toLowerCase() == "both") {

            var filter = 'agTextColumnFilter';

            if (columnSetting.dataType == 'int' || columnSetting.dataType == 'float') {
                filter = 'agNumberColumnFilter';

            }
            if (columnSetting.dataType == 'date') {
                filter = 'agDateColumnFilter';
                item["filterParams"] = {
                    comparator: function (filterLocalDate, cellValue) {
                        /* var dateParts = cellValue.split("/");
                         var day = Number(dateParts[1]);
                         var month = Number(dateParts[0]) - 1;
                         var year = Number(dateParts[2]);
                         var cellDate = new Date(day, month, year);*/
                        var cellDate = new Date(cellValue);

                        // Now that both parameters are Date objects, we can compare
                        if (cellDate < filterLocalDate) {
                            return -1;
                        } else if (cellDate > filterLocalDate) {
                            return 1;
                        } else {
                            return 0;
                        }

                    }, clearButton: true, applyButton: true
                };
            }
            item["filter"] = filter;
        }

        if (columnSetting.colModel != undefined && columnSetting.colModel.length > 0) {
            var childitem = new Array();
            for (var i = 0; i < columnSetting.colModel.length; i++) {
                childitem.push(setHeader(columnSetting.colModel[i]));
            }
            item["children"] = childitem;
        }
        if (GLOBAL.applicationName.toLowerCase() == 'ebenefitsync') {
            if (columnSetting.dataIndx.indexOf('Allowable') != -1 || columnSetting.dataIndx.indexOf('Standard') != -1)
                item["maxWidth"] = 250;
        }
        return item;
    }
    return {
        prepare: prepare,
    };
})();

var gridGlobalSetting = function () {


    var defaultGridOption = function () {
        var enablePivotButton = true;
        var GridOption = {
            enableCellChangeFlash: true,
            rowDragManaged: true,
            floatingFilter: true,
            //  rowGroupPanelShow: false,
            //showToolPanel: true,
            //toolPanelSuppressPivots: true,
            // toolPanelSuppressPivotMode: true,
            enterMovesDownAfterEdit: true,
            animateRows: true,
            enableColResize: true, //one of [true, false]
            enableSorting: true, //one of [true, false]
            enableFilter: true, //one of [true, false]
            enableStatusBar: true,
            enableRangeSelection: false,
            rowSelection: "multiple", // one of ['single','multiple'], leave blank for no selection
            //rowDeselection: true,
            quickFilterText: null,
            //     groupSelectsChildren: false,
            //  suppressRowClickSelection: false,
            //   debug: true,
            pagination: true,
            paginationAutoPageSize: false,
            paginationPageSize: 10,// pageSize,
            paginationNumberFormatter: function (params) {
                return params.value.toLocaleString();
            },
            singleClickEdit: true,
            onRowSelected: null,
            components: {
                agColumnHeader: CustomHeader,
                richTextBoxCellRenderer: RichTextBoxCellRenderer,
                agDateInput: CustomDateComponent,
                booleanCellRenderer: booleanCellRenderer
            },
            // pivotMode: true,
            //  enableColResize: true,
            pivotPanelShow: true,
            pivotTotals: true,
            showToolPanel: false,
            suppressContextMenu: true,
            /*
            toolPanelSuppressRowGroups:true;
            toolPanelSuppressValues
            toolPanelSuppressPivots:
            toolPanelSuppressPivotMode: 
            toolPanelSuppressSideButtons: 
            toolPanelSuppressColumnFilter: 
            toolPanelSuppressColumnSelectAll:
                toolPanelSuppressColumnExpandAll: */

            defaultColDef: {
                //menuTabs: ['generalMenuTab', 'gibberishMenuTab', 'columnsMenuTab'],
                menuTabs: ['filterMenuTab', 'generalMenuTab'],
                editable: false,
                enableValue: true,
                // allow every column to be grouped
                enableRowGroup: true,
                // allow every column to be pivoted
                enablePivot: true,
                headerComponentParams: {
                    menuIcon: 'fa-bars'
                },
            },
            //throttleScroll: true,
            // suppressMenu: true,
            columnDefs: null,
            rowData: null,
            onCellValueChanged: null,
            onCellClicked: null,
            onRowSelected: null
        };
        return GridOption;
    }



    return {
        defaultGridOption: defaultGridOption
    };

};


var constantKey =
{
    KEY_LEFT: 37,
    KEY_UP: 38,
    KEY_RIGHT: 39,
    KEY_DOWN: 40,
    KEY_PAGE_UP: 33,
    KEY_PAGE_DOWN: 34,
    KEY_PAGE_HOME: 36,
    KEY_PAGE_END: 35,
    KEY_ENTER: 13,
    KEY_TAB: 9
}


applyRule = function (control, params, eInput)
{
    var isExists = false;

    if (params.api.gridOptionsWrapper.gridOptions.currentInstance.cellRules == undefined)
    {
        return;
    }
    var enableRules = params.api.gridOptionsWrapper.gridOptions.currentInstance.cellRules.cellEnableStyle;
    var colId = params.column.colId;
    var rowIndex = params.node.data.RowIDProperty;

    $.each(enableRules, function (i, el) {
        if (this.colId == colId && this.rowIndex == rowIndex) {
            isExists = true;
            if (controType.checkBox === control || controType.dropDownText === control || controType.dropDown === control || controType.multiSelect === control)
            {
                eInput.setAttribute("disabled", true);
            }
            if (controType.textBox === control || controType.calender === control || controType.multiTextBox === control) {
                eInput.setAttribute("readonly", "true");
            }
            eInput.setAttribute("title", "Rule applied, Not Editable");
            params.node.gridOptionsWrapper.gridOptions.api.stopEditing();
        }
    });
    return isExists;
}
executeRule = function (eInput, param,KEY, control )
{
    currentInstance = param.api.gridOptionsWrapper.gridOptions.currentInstance;

    eInput.addEventListener('keydown', function (event) {


        var keyCode = event.keyCode;
        if (keyCode === KEY) {
            rowNode = param.node;
            var value = '';
            if (controType.checkBox === control)
            {
                value = $(this)[0].checked;
                if (value===false)
                {
                    value = "No";
                }
                else
                {
                    value = "Yes";
                }
            }
            else if (controType.textBox === control || controType.calender === control || controType.multiTextBox === control)
            {
                value = $(this).val();
            }
            else if (controType.multiSelect === control  || controType.dropDownText === control || controType.dropDown === control )
            {
                value = $(this).find("option:selected").text();
            }
            rowNode.setDataValue(param.column.colId, value);
            //currentInstance.runRuleOnChange(param.column.colId, param.node.data.RowIDProperty, value, currentInstance,false);
        }
    });

}
function getTextBox() {
    // function to act as a class

    function TextBox() {

    }

    // gets called once before the renderer is used
    TextBox.prototype.init = function (params) {
        // create the cell

        var param = params;

        currentInstance = params.api.gridOptionsWrapper.gridOptions.currentInstance;

        currentInstance.lastSelectedRow = params.node.data.RowIDProperty;
        this.eInput = document.createElement('input');
        if (params.value == undefined || params.value == 'undefined')
        {
            params.value = '';
        }
        this.eInput.value = params.value;
        if (params.dataType == 'float' && params.value != "" && params.value != undefined && params.value != null && !isNaN(params.value)) {
            this.eInput.value = parseFloat(this.eInput.value).toFixed(2);
        }

        this.isDisable = applyRule(controType.textBox, params, this.eInput)
        this.onBlur = this.onBlur.bind(this, params);
        this.eInput.addEventListener('blur', this.onBlur);

        executeRule(this.eInput, param, constantKey.KEY_TAB, controType.textBox);
    };

    TextBox.prototype.onBlur = function (params) {

        var gridOptions = params.api.gridOptionsWrapper.gridOptions;
        rowNode = params.node;
        if (params.dataType == 'float' && this.eInput.value != "" && this.eInput.value != undefined && this.eInput.value != null && !isNaN(this.eInput.value)) {
            this.eInput.value = parseFloat(this.eInput.value).toFixed(2);
        }
        rowNode.setDataValue(params.column.colId, this.eInput.value);
        gridOptions.api.clearFocusedCell();
    }

    // gets called once when grid ready to insert the element
    TextBox.prototype.getGui = function () {
        return this.eInput;
    };

    // focus and select can be done after the gui is attached
    TextBox.prototype.afterGuiAttached = function () {
        this.eInput.focus();
        this.eInput.select();
    };

    // returns the new value after editing
    TextBox.prototype.getValue = function () {

        return this.eInput.value;
    };

    TextBox.prototype.isCancelBeforeStart = function () {
        return this.cancelBeforeStart;
    };

    // any cleanup we need to be done here
    TextBox.prototype.destroy = function () {
        // but this example is simple, no cleanup, we could
        // even leave this method out as it's optional
    };

    // if true, then this editor will appear in a popup
    TextBox.prototype.isPopup = function () {
        // and we could leave this method out also, false is the default
        return false;
    };

    return TextBox;
}

function getLargeTextCellEditor() {
    function LargeTextCellEditor() {

    }
    LargeTextCellEditor.prototype.init = function (params) {
        var template =
        // tab index is needed so we can focus, which is needed for keyboard events
        '<div class="ag-large-text" tabindex="0">' +
            '<div class="ag-large-textarea"></div>' +
            '</div>';
        this.eInput = document.createElement('div');
        this.eInput.innerHTML = template;

        this.params = params;
        this.focusAfterAttached = params.cellStartedEdit;
        this.textarea = document.createElement("textarea");
        this.textarea.maxLength = params.maxLength ? params.maxLength : "200";
        this.textarea.cols = params.cols ? params.cols : "60";
        this.textarea.rows = params.rows ? params.rows : "10";
        if (params.value !== '') {
            this.textarea.value = params.value.toString();
        }
        this.getGui().querySelector('.ag-large-textarea').appendChild(this.textarea);
        this.textarea.addEventListener('keydown', this.onKeyDown.bind(this));
        this.textarea.attributes
        this.textarea.setAttribute("title", "Press Shift + Enter key for nextline");
        this.isDisable = applyRule(controType.multiTextBox, params, this.textarea)
    };
    LargeTextCellEditor.prototype.getGui = function () {
        return this.eInput.firstChild;
    };
    LargeTextCellEditor.prototype.onKeyDown = function (event) {
        var KEY_LEFT = 37;
        var KEY_UP = 38;
        var KEY_RIGHT = 39;
        var KEY_DOWN = 40;
        var KEY_PAGE_UP = 33;
        var KEY_PAGE_DOWN = 34;
        var KEY_PAGE_HOME = 36;
        var KEY_PAGE_END = 35;
        var KEY_ENTER = 13;
        var key = event.which || event.keyCode;
        if (key == KEY_LEFT ||
            key == KEY_UP ||
            key == KEY_RIGHT ||
            key == KEY_DOWN ||
            (event.shiftKey && key == KEY_ENTER)) { // shift+enter allows for newlines
            event.stopPropagation();
        }
    };
    LargeTextCellEditor.prototype.afterGuiAttached = function () {
        if (this.focusAfterAttached) {
            this.textarea.focus();
        }
    };
    LargeTextCellEditor.prototype.getValue = function () {
        return this.params.parseValue(this.textarea.value);
    };
    LargeTextCellEditor.prototype.isPopup = function () {
        return true;
    };


    return LargeTextCellEditor;
}
function getDatePicker() {
    // function to act as a class
    function Datepicker() { }

    // gets called once before the renderer is used
    Datepicker.prototype.init = function (params) {
        // create the cell

        this.eInput = document.createElement('input');
        this.eInput.value = params.value;
        this.eInput.className = "form-control calendar";
        this.eInput.setAttribute("style", "'width:76%!important;");

        this.isDisable = applyRule(controType.calender, params, this.eInput)

        var minDate = "", maxDate = "", defaultDate = "";
        this.onBlur = this.onBlur.bind(this);
        this.eInput.addEventListener('blur', this.onBlur);
        executeRule(this.eInput, params, constantKey.KEY_TAB, controType.calender);
        if (this.isDisable === false) {
            $(this.eInput).datepicker({
                dateFormat: "mm/dd/yy",
                changeMonth: true,
                changeYear: true,
                yearRange: 'c-61:c+20',
                showOn: "both",
                minDate: minDate == '' || minDate == null ? null : new Date(minDate),
                maxDate: maxDate == '' || maxDate == null ? null : new Date(maxDate),
                buttonImage: Icons.CalenderIcon,
                buttonImageOnly: true
            });
        }
    };

    Datepicker.prototype.onBlur = function () {
        var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
        gridOptions.api.clearFocusedCell();
    }

    // gets called once when grid ready to insert the element
    Datepicker.prototype.getGui = function () {
        return this.eInput;
    };

    // focus and select can be done after the gui is attached
    Datepicker.prototype.afterGuiAttached = function () {
        this.eInput.focus();
        this.eInput.select();
    };

    // returns the new value after editing
    Datepicker.prototype.getValue = function () {
        return this.eInput.value;
    };

    // any cleanup we need to be done here
    Datepicker.prototype.destroy = function () {
        // but this example is simple, no cleanup, we could
        // even leave this method out as it's optional
    };

    // if true, then this editor will appear in a popup
    Datepicker.prototype.isPopup = function () {
        // and we could leave this method out also, false is the default
        return false;
    };

    return Datepicker;
}

function getCheckBoxPicker() {
    // function to act as a class
    function CheckBoxPicker() { }

    // gets called once before the renderer is used
    CheckBoxPicker.prototype.init = function (params) {
        // create the cell
        var currentInst = this;
        currentInst.params = params;

        this.eInput = document.createElement('input');
        this.eInput.value = params.value;
        var checked = '';

        this.isDisable = applyRule(controType.checkBox, params, this.eInput)

        if (params.value == 'Yes' || params.value == 'yes') {
            $(this.eInput).attr({ type: 'checkbox', name: 'chk', 'checked': '' });
        }
        else {
            $(this.eInput).attr({ type: 'checkbox', name: 'chk' });
        }

        this.onBlur = this.onBlur.bind(this);
        this.eInput.addEventListener('blur', this.onBlur);
        executeRule(this.eInput, params, constantKey.KEY_TAB, controType.checkBox);
    };

    CheckBoxPicker.prototype.onBlur = function () {
        var gridOptions = this.params.api.gridOptionsWrapper.gridOptions;
        //rowNode = this.params.node;

        //var value = "No";

        //if (this.eInput.value == "true" || this.eInput.value == "Yes") {
        //    value = "Yes";
        //}
        gridOptions.api.clearFocusedCell();
        //rowNode.setDataValue(this.params.column.colId, value);
    }
    // gets called once when grid ready to insert the element
    CheckBoxPicker.prototype.getGui = function () {
        return this.eInput;
    };

    // focus and select can be done after the gui is attached
    CheckBoxPicker.prototype.afterGuiAttached = function () {
        this.eInput.focus();
        this.eInput.select();
    };

    // returns the new value after editing
    CheckBoxPicker.prototype.getValue = function () {
        if (this.eInput.checked == true) {
            return 'Yes';
        }
        return 'No';
    };

    // any cleanup we need to be done here
    CheckBoxPicker.prototype.destroy = function () {
        // but this example is simple, no cleanup, we could
        // even leave this method out as it's optional
    };

    // if true, then this editor will appear in a popup
    CheckBoxPicker.prototype.isPopup = function () {
        // and we could leave this method out also, false is the default
        return false;
    };

    return CheckBoxPicker;
}

function getDropdownTextbox() {
    // function to act as a class
    function DropdownTextbox() { }

    // gets called once before the renderer is used
    DropdownTextbox.prototype.init = function (params) {
        // create the cell
        this.eDiv = document.createElement('DIV');
        this.eDiv.className = "DropdownTextbox";
        this.oldValue = params.value;

        this.eSelect = document.createElement('select');
        var currentDDLInstance = this;
        var ddlId = 'id' + params.column.colDef.field + "_" + params.rowIndex.toString();
        this.eSelect.setAttribute('id', ddlId);
        this.isDisable = applyRule(controType.dropDownText, params, this.eSelect)

        var data = params.values;
        var isItemFound = false;

        /*for (var val in data) {
            var eOption = document.createElement("option");
            eOption.text = data[val];
            eOption.value = val;
            if (params.value == data[val]) {
                eOption.selected = true;
                isItemFound = true;
            }
            this.eSelect.appendChild(eOption);
        };*/

        var eselect = this.eSelect;
        var optionsValue = {};
        $.each(params.values, function (i, item) {
            if (typeof (item) == "object") {
                for (var key in item) {
                    var eOption = document.createElement("option");
                    eOption.text = item[key];
                    eOption.value = key;
                    if ((params.value == item[key]) || (params.value == eOption.text)) {
                        eOption.selected = true;
                        isItemFound = true;
                    }
                    eselect.appendChild(eOption);
                }
            }
            else {
                var eOption = document.createElement("option");
                eOption.text = item;
                eOption.value = item;
                if ((params.value == item) || (params.value == eOption.text)) {
                    eOption.selected = true;
                    isItemFound = true;
                }
                eselect.appendChild(eOption);
            }
        });

        if (isItemFound == false) {
            var eOption = document.createElement("option");
            if (params.value != undefined && params.value != undefined) {
                eOption.text = params.value;
                eOption.value = params.value;
                eOption.selected = true;
            }
            //  eOption.setAttribute('style', "background-color: #ffc2c3;");
            eOption.setAttribute('class', "non-standard-optn");
            this.eSelect.appendChild(eOption);
        }

        this.eText = document.createElement('input');
        this.eText.setAttribute('type', "text");

        this.eDiv.appendChild(this.eSelect);
        this.eDiv.appendChild(this.eText);

        this.eText.setAttribute('type', "text");

        if(this.isDisable)
        {
            this.eText.setAttribute('readonly', "true");
        }

        this.onBlur = this.onBlur.bind(this);
        this.eSelect.addEventListener('blur', this.onBlur);

        executeRule(this.eSelect, params, constantKey.KEY_TAB, controType.dropDownText);
        executeRule(this.eText, params, constantKey.KEY_TAB, controType.textBox);

    };

    DropdownTextbox.prototype.onBlur = function () {
        var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
        gridOptions.api.clearFocusedCell();
    }

    // gets called once when grid ready to insert the element
    DropdownTextbox.prototype.getGui = function () {
        return this.eDiv;
    };

    // focus and select can be done after the gui is attached
    DropdownTextbox.prototype.afterGuiAttached = function () {
        this.eDiv.focus();

    };


    // returns the new value after editing
    DropdownTextbox.prototype.getValue = function () {


        if (this.eText.value != "") {
            return this.eText.value;

        }

        var selectedValue = getvalueFromDropdown(this.eSelect, this.oldValue);

        /*   if (this.eSelect.selectedOptions != undefined) {
               if (this.eSelect.selectedOptions.length > 0) {
   
                   if (this.eSelect.selectedOptions[0].text.toLowerCase() != "[select one]") {
                       selectedValue = this.eSelect.selectedOptions[0].text;
                   }
                   else {
                       selectedValue = this.oldValue;
                   }
               }
            
           }
           else {
               selectedValue = this.eSelect.options[this.eSelect.selectedIndex].text;
               if (selectedValue == "[select one]")
                   selectedValue = '';
           }
           */
        return selectedValue;
    };

    // any cleanup we need to be done here
    DropdownTextbox.prototype.destroy = function () {
        // but this example is simple, no cleanup, we could
        // even leave this method out as it's optional
    };

    // if true, then this editor will appear in a popup
    DropdownTextbox.prototype.isPopup = function () {
        // and we could leave this method out also, false is the default
        return true;
    };

    return DropdownTextbox;
}


function getDropdown() {
    this.url = '';
    this.colName = '';

    // function to act as a class
    function Dropdown(url, colName) {
        this.url = url;
        this.colName = colName;
    }

    // gets called once before the renderer is used
    Dropdown.prototype.init = function (params) {
        // create the cell
        var hasRangeGuardrails = "";
        if (GLOBAL.applicationName.toLowerCase() == 'ebenefitsync') {
            var cellInsatnce = this;
            //this.eDiv = document.createElement('DIV');
            //this.eDiv.className = "Dropdown";
            currentInstance = params.api.gridOptionsWrapper.gridOptions.currentInstance;
            currentInstance.columnName = params.column.colId;
            currentInstance.lastSelectedRow = params.node.data.RowIDProperty;
            if (currentInstance.expressionBuilder != undefined && currentInstance.expressionBuilder != null) {
                if (currentInstance.expressionBuilder.hasExpressionBuilderRule(currentInstance.formInstanceBuilder.formDesignId)
                    && currentInstance.expressionBuilder.hasCustomRuleColumn(currentInstance.formInstanceBuilder.formDesignId, currentInstance.fullName, currentInstance.columnName)) {
                    var result = currentInstance.expressionBuilder.onCellClick(currentInstance, params, cellInsatnce);
                    if (result != undefined) {
                        params.values = result.Values == undefined ? [] : result.Values;
                        var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
                        var col = gridOptions.columnApi.getColumn(currentInstance.columnName);
                        if (col.colDef != undefined && col.colDef.cellEditorParams != undefined && col.colDef.cellEditorParams.values != undefined) {
                            col.colDef.cellEditorParams.values = params.values;
                        }
                    }
                }
                hasRangeGuardrails = currentInstance.expressionBuilder.expressionBuilderExtension.getGuardrailType(currentInstance, params.node.data, false);
            }
        }
        this.eDiv = document.createElement('DIV');
        this.eDiv.className = "Dropdown";
        this.oldValue = params.value;

        this.eSelect = document.createElement('select');
        var currentDDLInstance = this;
        var ddlId = 'id' + params.column.colDef.field + "_" + params.rowIndex.toString();
        this.eSelect.setAttribute('id', ddlId);
        this.isDisable = applyRule(controType.dropDownText, params, this.eSelect)
        if (hasRangeGuardrails == "Range") {
            params.values = [];
        }
        var data = params.values;
        var isItemFound = false;

        /*for (var val in data) {
            var eOption = document.createElement("option");
            eOption.text = data[val];
            eOption.value = val;
            if (params.value == data[val]) {
                eOption.selected = true;
                isItemFound = true;
            }
            this.eSelect.appendChild(eOption);
        };*/

        var eselect = this.eSelect;
        var optionsValue = {};
        $.each(params.values, function (i, item) {
            if (typeof (item) == "object") {
                for (var key in item) {
                    var eOption = document.createElement("option");
                    eOption.text = item[key];
                    eOption.value = key;
                    if ((params.value == item[key]) || (params.value == eOption.text)) {
                        eOption.selected = true;
                        isItemFound = true;
                    }
                    eselect.appendChild(eOption);
                }
            }
            else {
                var eOption = document.createElement("option");
                eOption.text = item;
                eOption.value = item;
                if ((params.value == item) || (params.value == eOption.text)) {
                    eOption.selected = true;
                    isItemFound = true;
                }
                eselect.appendChild(eOption);
            }
        });


        this.eDiv.appendChild(this.eSelect);

        this.onBlur = this.onBlur.bind(this);
        this.eSelect.addEventListener('blur', this.onBlur);

        executeRule(this.eSelect, params, constantKey.KEY_TAB, controType.dropDown);
    };

    Dropdown.prototype.onBlur = function () {
        var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
        gridOptions.api.clearFocusedCell();
    }

    // gets called once when grid ready to insert the element
    Dropdown.prototype.getGui = function () {
        return this.eDiv;
    };

    // focus and select can be done after the gui is attached
    Dropdown.prototype.afterGuiAttached = function () {
        this.eDiv.focus();

    };

    // returns the new value after editing
    Dropdown.prototype.getValue = function () {
        var selectedValue = getvalueFromDropdown(this.eSelect, this.oldValue);


        /*
        if (this.eSelect.selectedOptions.length > 0) {

            if (this.eSelect.selectedOptions[0].text.toLowerCase() != "[select one]") {
                selectedValue = this.eSelect.selectedOptions[0].text;
            }
            else {
                selectedValue = this.oldValue;
            }
        }*/

        return selectedValue;
    };

    // any cleanup we need to be done here
    Dropdown.prototype.destroy = function () {
        // but this example is simple, no cleanup, we could
        // even leave this method out as it's optional
    };

    // if true, then this editor will appear in a popup
    Dropdown.prototype.isPopup = function () {
        // and we could leave this method out also, false is the default
        return true;
    };

    return Dropdown;
}

function getvalueFromDropdown(selectELement, oldvalue) {
    var selectedValue = '';
    if (selectELement.selectedOptions != undefined) {
        if (selectELement.selectedOptions.length > 0) {

            if (selectELement.selectedOptions[0].text.toLowerCase() != "[select one]") {
                selectedValue = selectELement.selectedOptions[0].text;
            }
            else if (selectELement.selectedOptions[0].text.toLowerCase() == "[select one]")
            {
                selectedValue = "";
            }
            else {
                selectedValue = oldvalue;
            }
        }

    }
    else {
        selectedValue = selectELement.options[selectELement.selectedIndex].text;
        if (selectedValue.toLowerCase() == "[select one]")
            selectedValue = oldvalue;
    }
    return selectedValue;
}

function getMultiSelect() {
    // function to act as a class
    function MultiSelect() { }

    // gets called once before the renderer is used
    MultiSelect.prototype.init = function (params) {
        this.params;
        if (GLOBAL.applicationName.toLowerCase() == 'ebenefitsync') {
            var cellInsatnce = this;
            //this.eDiv = document.createElement('DIV');
            //this.eDiv.className = "Dropdown";
            currentInstance = params.api.gridOptionsWrapper.gridOptions.currentInstance;
            currentInstance.columnName = params.column.colId;
            currentInstance.lastSelectedRow = params.node.data.RowIDProperty;
            if (currentInstance.expressionBuilder.hasExpressionBuilderRule(currentInstance.formInstanceBuilder.formDesignId)
                && currentInstance.expressionBuilder.hasCustomRuleColumn(currentInstance.formInstanceBuilder.formDesignId, currentInstance.fullName, currentInstance.columnName)) {
                var result = currentInstance.expressionBuilder.onCellClick(currentInstance, params, cellInsatnce);
                if (result != undefined) {
                    params.values = result.Values == undefined ? [] : result.Values;
                    var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
                    var col = gridOptions.columnApi.getColumn(currentInstance.columnName);
                    if (col.colDef != undefined && col.colDef.cellEditorParams != undefined && col.colDef.cellEditorParams.values != undefined) {
                        col.colDef.cellEditorParams.values = params.values;
                        if (result.SelectedValues != undefined) {
                            params.value = result.SelectedValues;
                        }
                    }
                }
            }
        }

        // create the cell

        this.eInput = document.createElement('div');
        var currentDDLInstance = this;
        var ddlId = 'ddl' + params.rowIndex.toString();

        //$(this.eInput).attr({ class: 'ag-cell-edit-input' });

        var data = params.values;

        var html = '<select id="' + ddlId + '" multiple="multiple">';


        var valueArry = [];
        if (params.value != '' && params.value != undefined) {
            if (params.value instanceof Array) {
                valueArry = params.value;
            }
            else {
                if (GLOBAL.applicationName.toLowerCase() == 'ebenefitsync') {
                    if (currentInstance.expressionBuilderRulesExt != undefined && currentInstance.expressionBuilderRulesExt != null) {
                        valueArry = currentInstance.expressionBuilderRulesExt.getMultiSelectKeyArray(params.value);
                    }
                }
                else {
                    valueArry = params.value.split(",");
                }
            }
        }
        /*
        for (var val in data) {
            // $('<option />', { value: val, text: data[val] }).appendTo($(this.eInput));

            var selected = '';
            $.each(valueArry, function (index, keyData) {
                if (data[val] == keyData)
                {
                    selected = ' selected ';
                }
            });
            html = html + '<option ' + selected  + ' value="' + val + '">' + data[val] + '</option>'
            selected = '';
        };
        html = html + ' </select>';
        */
        var optionsValue = {};
        $.each(params.values, function (i, item) {

            for (var key in item) {
                var selected = '';
                /*if (item[key] == params.value) {
                    selected = ' selected ';
                }*/
                $.each(valueArry, function (index, keyData) {
                    if (item[key] == keyData) {
                        selected = ' selected ';
                    }
                });

                if (key == '[Select One]') {
                    html = html + '<option value=0>' + item[key] + '</option>'
                }
                else {
                    html = html + '<option ' + selected + ' value="' + key + '">' + item[key] + '</option>'
                }
            }
        });
        html = html + ' </select>';


        this.eInput.innerHTML = html;
        this.select = $(this.eInput).find('#' + ddlId);

        if (this.select.length > 0)
            this.isDisable = applyRule(controType.multiSelect, params, this.select[0])

        this.onBlur = this.onBlur.bind(this);
        this.eInput.addEventListener('blur', this.onBlur);

        executeRule(this.eInput, params, constantKey.KEY_TAB, controType.multiSelect);

        this.select.multiselect({
            nonSelectedText: 'Select',
            enableFiltering: true,
            enableCaseInsensitiveFiltering: true,
            buttonWidth: GLOBAL.applicationName.toLowerCase() == 'ebenefitsync' ? params.eGridCell.clientWidth + "px" : "215px"
        });
    };

    MultiSelect.prototype.onBlur = function () {
        var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
        gridOptions.api.clearFocusedCell();
    }

    // gets called once when grid ready to insert the element
    MultiSelect.prototype.getGui = function () {
        return this.eInput;
    };

    // focus and select can be done after the gui is attached
    MultiSelect.prototype.afterGuiAttached = function () {
        this.eInput.focus();

    };

    // returns the new value after editing
    MultiSelect.prototype.getValue = function () {
        var selectedValues = '';
        this.select.find(':selected').each(function () {
            //alert($(this).text() + ' ' + $(this).val());
            var text = $(this).text();
            if (text.toLowerCase().indexOf("[select one]") == -1) {
                if (selectedValues == '') {
                    selectedValues = text;
                }
                else {
                    selectedValues = selectedValues + ',' + text;
                }
            }
        });

        return selectedValues;
    };

    // any cleanup we need to be done here
    MultiSelect.prototype.destroy = function () {
        // but this example is simple, no cleanup, we could
        // even leave this method out as it's optional
    };

    // if true, then this editor will appear in a popup
    MultiSelect.prototype.isPopup = function () {
        // and we could leave this method out also, false is the default
        return true;
    };

    return MultiSelect;
}

/*backup working
function getMultiSelect() {
    // function to act as a class
    function MultiSelect() { }

    // gets called once before the renderer is used
    MultiSelect.prototype.init = function (params) {
        // create the cell
        this.eInput = document.createElement('div');
        this.hiddenValue = document.createElement('div');
        var currentDDLInstance = this;
        var ddlId = 'ddl' + params.rowIndex.toString();
        $(this.hiddenValue).attr({ id: 'hid' + ddlId, type: 'hidden'});

        //$(this.eInput).attr({ class: 'ag-cell-edit-input' });

        var data = params.values;

        var html = '<select id="' + ddlId + '" multiple="multiple">';

        for (var val in data) {
           // $('<option />', { value: val, text: data[val] }).appendTo($(this.eInput));
            html = html + '<option value="' + val + '">' + data[val] + '</option>'
        };       
        html = html + ' </select>';       

        this.eInput.innerHTML = html;
        this.select = $(this.eInput).find('#' + ddlId);


        if (params.value != '')
        {
            if (params.value instanceof Array)
            {
                this.select.val(params.value);
            }
            else {
                this.select.val(params.value.split(","));
            }
        }
        $(currentDDLInstance.hiddenValue).val(params.value);

        this.select.multiselect({
            nonSelectedText: 'Select',
            enableFiltering: true,
            enableCaseInsensitiveFiltering: true,
            buttonWidth: '400px',
            onChange: function () {
                $(currentDDLInstance.hiddenValue).val($('#' + ddlId).val().join());
                console.log($('#' + ddlId).val().join());
            }
        }); 
    };

    // gets called once when grid ready to insert the element
    MultiSelect.prototype.getGui = function () {
        return this.eInput;
    };

    // focus and select can be done after the gui is attached
    MultiSelect.prototype.afterGuiAttached = function () {
        this.eInput.focus();
     
    };

    // returns the new value after editing
    MultiSelect.prototype.getValue = function () {
        var selectedValues = '';
        this.select.find(':selected').each(function () {
            //alert($(this).text() + ' ' + $(this).val());
            var text = $(this).val();
            if (text.toLowerCase().indexOf("[select one]") == -1) {
                if (selectedValues == '') {
                    selectedValues = text;
                }
                else {
                    selectedValues = selectedValues + ',' + text;
                }
            }
        });
        return selectedValues;
      //  return $(this.hiddenValue).val();
    };

    // any cleanup we need to be done here
    MultiSelect.prototype.destroy = function () {
        // but this example is simple, no cleanup, we could
        // even leave this method out as it's optional
    };

    // if true, then this editor will appear in a popup
    MultiSelect.prototype.isPopup = function () {
        // and we could leave this method out also, false is the default
        return true;
    };

    return MultiSelect;
}
*/

//force 
function getRichTextBox() {

    // function to act as a class
    function RichTextBox() {
    }

    // gets called once before the renderer is used
    RichTextBox.prototype.init = function (params) {
        this.params = params;
        // create the cell
        this.eInput = document.createElement('SPAN');
        this.eInput.setAttribute('id', 'div' + params.column.colId + params.rowIndex);
        this.eInput.setAttribute('style', 'width:600px;height:500px');
        this.eInput.setAttribute('z-index', '0');
        // this.eInput.className = "ag-popup-editor";

        //        this.eBulkDropDownTextBox = this.eGui.querySelector(".ddt-textbox");
        //console.log(this.eBulkDropDownTextBox);
        //  this.onClick = this.eInput.bind(this, params.api.gridOptionsWrapper.gridOptions.currentInstance);
        //this.eInput.addEventListener('onclick', this.onClick);
        this.isDisable = applyRule(controType.richTextBox, params, this.eInput)


        this.value = '';
        if (this.isDisable === false) {
            this.eInput.onclick = this.onClick(this);
        }
    };


    // gets called once when grid ready to insert the element
    RichTextBox.prototype.getGui = function () {
        //this.richTextbox.tinymce({
        //  this.loadTinyMCE(this.etextArea);
        return this.eInput;
    };


    RichTextBox.prototype.onClick = function (instance) {
        this.dialog = new repeaterCellDialog(instance.params, RichTextBoxRenderingFrom.Cell);
        this.dialog.show();
    }
    // focus and select can be done after the gui is attached
    RichTextBox.prototype.afterGuiAttached = function () {
        //this.eInput.focus();

    };

    // returns the new value after editing
    RichTextBox.prototype.getValue = function () {

        /*  var val = $('#txt' + this.params.column.colId + this.params.rowIndex).val();
          console.log('val>>' + val);
  
          console.log('getValue>>' + this.dialog.getContainerValue());
  
          return this.dialog.getContainerValue();*/
        return this.params.node.data[this.params.column.colId];

    };

    // any cleanup we need to be done here
    RichTextBox.prototype.destroy = function () {
        // but this example is simple, no cleanup, we could
        // even leave this method out as it's optional
    };

    // if true, then this editor will appear in a popup
    RichTextBox.prototype.isPopup = function () {
        // and we could leave this method out also, false is the default
        return true;
    };

    return RichTextBox;
}
//uaable to load rich textbox on textrea as document is not loaded 100%
/* 
function getRichTextBox() {
    // function to act as a class
    function RichTextBox() { }

    // gets called once before the renderer is used
    RichTextBox.prototype.init = function (params) {
        // create the cell
        // this.eInput = document.createElement('div');
        this.eInput = $('#reachTextBoxLayout');
        this.eInput[0].setAttribute('style', 'width:600px;height:500px');
    //    this.eInput.setAttribute('id', 'div' + params.column.colId + params.rowIndex);
        //this.eInput.className = "col-xs-9 col-md-9 col-lg-9 col-sm-9";

        // this.eGui.setAttribute('class', 'col-xs-9 col-md-9 col-lg-9 col-sm-9');
        this.eInput.html('');

        var textAreaHtml = "<textarea id='" + params.column.colId + params.rowIndex + "' wrap='hard' cols='25' rows='5' class='form-control'>" + params.value + "</textarea>";
        
        this.eInput.html(textAreaHtml);
       // this.eInput.innerHTML = textAreaHtml;

        this.etextArea = this.eInput[0].querySelector(".form-control");

        
        //  this.loadTinyMCE($('#test'));
         this.loadTinyMCE($('#' +  params.column.colId + params.rowIndex));
    };

    RichTextBox.prototype.loadTinyMCE = function (id) {
        tinymce.remove(id.id);
        $(id).tinymce({
            statusbar: false,
            theme: 'modern',
            forced_root_block: "",
            force_br_newlines: true,
            force_p_newlines: false,
            plugins: [
              'advlist autolink lists charmap print preview hr pagebreak',
              'searchreplace wordcount visualblocks visualchars code',
              'insertdatetime save table directionality',
              'emoticons template powerpaste textcolor colorpicker textpattern imagetools codesample toc',
              'image'
            ],
            menubar: "file edit insert view format table tools",
            toolbar1: 'undo redo | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent',
            toolbar2: 'preview forecolor backcolor | fontselect |  fontsizeselect | CenterAlign | RightAlign',
            image_advtab: true,
            powerpaste_word_import: 'prompt',
            powerpaste_html_import: 'prompt',
            templates: [
              { title: 'Test template 1', content: 'Test 1' },
              { title: 'Test template 2', content: 'Test 2' }
            ],
            image_list: [
                       { title: 'Apple', value: '/Content/tinyMce/Apple.png' },
                       { title: 'Question Mark', value: '/Content/tinyMce/Question Mark.png' },
                       { title: 'Tick Mark', value: '/Content/tinyMce/Tick Mark.png' },
                       { title: 'Square Bullet', value: '/Content/tinyMce/Square Bullet.png' },
                       { title: 'ID Card No Rx - Back', value: '/Content/tinyMce/ID Card No Rx - Back.png' },
                       { title: 'ID Card No Rx - Front', value: '/Content/tinyMce/ID Card No Rx - Front.png' },
                       { title: 'Medicare Rx Membership Card', value: '/Content/tinyMce/Medicare Rx Membership Card.png' },
                       { title: 'Back Membership Card', value: '/Content/tinyMce/Back Membership Card.png' },
                       { title: 'Tick [Large]', value: '/Content/tinyMce/Tick [Large].png' },
                       { title: 'Tick Bullets', value: '/Content/tinyMce/Tick Bullets.png' },
                       { title: 'Tick Not Valid', value: '/Content/tinyMce/Tick Not Valid.png' }
            ],
            removed_menuitems: 'pastetext bold italic',
            style_formats: [
                               { title: 'Heading 1', block: 'h1' },
                               { title: 'Heading 2', block: 'h2' },
                               { title: 'Heading 3', block: 'h3' },
                               { title: 'Heading 4', block: 'h4' },
                               { title: 'Heading 5', block: 'h5' },
                               { title: 'Heading 6', block: 'h6' },
            ],
            //content_css: [
            //  '//fonts.googleapis.com/css?family=Lato:300,300i,400,400i',
            //  '//www.tinymce.com/css/codepen.min.css'
            //],
            setup: function (editor) {
                editor.addButton('CenterAlign', {
                    text: 'Center',
                    icon: false,
                    onclick: function () {
                        editor.insertContent('<div align="center">Enter Table Here<p style="margin: 0in 0in 10pt; line-height: 115%; font-size: 11pt; font-family: Calibri, sans-serif;">&nbsp;</p></div>');
                    }
                });
                editor.addButton('RightAlign', {
                    text: 'Right',
                    icon: false,
                    onclick: function () {
                        editor.insertContent('<div align="right">Enter Table Here<p style="margin: 0in 0in 10pt; line-height: 115%; font-size: 11pt; font-family: Calibri, sans-serif;">&nbsp;</p></div>');
                    }
                });
            }
        });

    }
    // gets called once when grid ready to insert the element
    RichTextBox.prototype.getGui = function () {
        //this.richTextbox.tinymce({
      //  this.loadTinyMCE(this.etextArea);
        return this.eInput[0];
    };

    // focus and select can be done after the gui is attached
    RichTextBox.prototype.afterGuiAttached = function () {
      //  this.eInput.focus();

    };

    // returns the new value after editing
    RichTextBox.prototype.getValue = function () {
       return  this.etextArea.value;
    };

    // any cleanup we need to be done here
    RichTextBox.prototype.destroy = function () {
        // but this example is simple, no cleanup, we could
        // even leave this method out as it's optional
    };

    // if true, then this editor will appear in a popup
    RichTextBox.prototype.isPopup = function () {
        // and we could leave this method out also, false is the default
        return true;
    };

    return RichTextBox;
}
*/

function NumericCellEditor() {
}

// gets called once before the renderer is used
NumericCellEditor.prototype.init = function (params) {
    // create the cell
    this.eInput = document.createElement('input');

    this.isDisable = applyRule(controType.textBox, params, this.eInput)


    if (isCharNumeric(params.charPress)) {
        this.eInput.value = params.charPress;
    } else {
        if (params.value !== undefined && params.value !== null) {
            this.eInput.value = params.value;
        }
    }

    var that = this;
    this.eInput.addEventListener('keypress', function (event) {
        if (!isKeyPressedNumeric(event)) {
            that.eInput.focus();
            if (event.preventDefault) event.preventDefault();
        } else if (that.isKeyPressedNavigation(event)) {
            event.stopPropagation();
        }
    });

    // only start edit if key pressed is a number, not a letter
    var charPressIsNotANumber = params.charPress && ('1234567890'.indexOf(params.charPress) < 0);
    this.cancelBeforeStart = charPressIsNotANumber;
};

NumericCellEditor.prototype.isKeyPressedNavigation = function (event) {
    return event.keyCode === 39
        || event.keyCode === 37;
};


// gets called once when grid ready to insert the element
NumericCellEditor.prototype.getGui = function () {
    return this.eInput;
};

// focus and select can be done after the gui is attached
NumericCellEditor.prototype.afterGuiAttached = function () {
    this.eInput.focus();
};

// returns the new value after editing
NumericCellEditor.prototype.isCancelBeforeStart = function () {
    return this.cancelBeforeStart;
};

// example - will reject the number if it contains the value 007
// - not very practical, but demonstrates the method.
NumericCellEditor.prototype.isCancelAfterEnd = function () {
    var value = this.getValue();
    return value.indexOf('007') >= 0;
};

// returns the new value after editing
NumericCellEditor.prototype.getValue = function () {
    return this.eInput.value;
};

// any cleanup we need to be done here
NumericCellEditor.prototype.destroy = function () {
    // but this example is simple, no cleanup, we could  even leave this method out as it's optional
};

// if true, then this editor will appear in a popup 
NumericCellEditor.prototype.isPopup = function () {
    // and we could leave this method out also, false is the default
    return false;
};


BulkUpdateData = function (bulkUpdateDropdown, agParams) {
    this.bulkUpdateDropdown = bulkUpdateDropdown;
    this.agParams = agParams;
    this.currentBulkInstance = this;
}
BulkUpdateData.prototype.getInstance = function () {
    return this;
}

BulkUpdateData.prototype.fillDropDown = function (node, index) {
    //console.log(index + ' -> group: ' + node.data.gold);
    //var ddl = document.querySelector('#ddl')

    var newOption = document.createElement("option");
    newOption.text = node.data[currentInstance.agParams.column.colId]
    newOption.value = index;
    currentInstance.bulkUpdateDropdown.appendChild(newOption);
}

CustomHeader = function () {
    this.bulkUpdateDropdown = "";
    this.agParams = {};
}

CustomHeader.prototype.init = function (agParams) {
    var currentHeaderInstance = this;
    currentHeaderInstance.bulkUpdateDropdown = "";
    currentHeaderInstance.agParams = agParams;

    this.eGui = document.createElement('div');
    this.eGui.className = "customTable";

    if (this.agParams.column.colDef.valueGetter == "node.rowIndex") {
        currentHeaderInstance.agParams.enableMenu = false;
        currentHeaderInstance.agParams.enableSorting = false;
        return;
    }
    this.eGui.innerHTML = '';
    if (currentHeaderInstance.agParams.enableMenu) {
        this.eGui.innerHTML = this.eGui.innerHTML + ' <div class="customHeaderMenuButton"><div class="fa ' + currentHeaderInstance.agParams.menuIcon + '"></div></div>';
    }
    // '<div class="customHeaderLabel">' + this.agParams.displayName + '</div>' +
    this.eGui.innerHTML = this.eGui.innerHTML +
      // '<div class="customHeaderMenuButton"><i class="fa ' + currentHeaderInstance.agParams.menuIcon + '"></i></div>' +
         '<div class="customHeaderLabel">' + currentHeaderInstance.agParams.displayName + '</div>' +
        '<div class="customSortDownLabel inactive"><div class="fa fa-long-arrow-down"></div></div>' +
        '<div class="customSortUpLabel inactive"><div class="fa fa-long-arrow-up"></div></div>' +
        '<div class="customSortRemoveLabel inactive"><div class="fa fa-times"></div></div>'


    var isbulkUpdateEnable = false;
    if (agParams.api.gridOptionsWrapper.gridOptions.currentInstance !== undefined) {
        isbulkUpdateEnable = agParams.api.gridOptionsWrapper.gridOptions.currentInstance.design.AllowBulkUpdate;
    }

    if (isbulkUpdateEnable == true && this.agParams.column.colId != "0") {


        var bulkControlHtml = '';
        var colModels = agParams.api.gridOptionsWrapper.gridOptions.currentInstance.columnModels;
        $.each(colModels, function (ind, value) {
            if (value.dataIndx == currentHeaderInstance.agParams.column.colId) {
                //  var prevControl = $(agParams.api.gridOptionsWrapper.gridOptions.currentInstance.gridElementIdJQ).find('[id*=bu_' + this.dataIndx + ']');
                //   var prevValue = agParams.api.gridOptionsWrapper.gridOptions.currentInstance.getBulkUpdateControlsElementValue(prevControl);
                var contlHTml = agParams.api.gridOptionsWrapper.gridOptions.currentInstance.getHeaderElement(this);
                if (contlHTml != undefined) {
                    bulkControlHtml = agParams.api.gridOptionsWrapper.gridOptions.currentInstance.getHeaderElement(this);
                }
            }
        });

        var tableHtml = '<div class= "customTableRow"> ' +
                                        '<div class="customTableCell">' + bulkControlHtml + '</div></div>' +
                                "<div class= 'customTableRow'> " +
                                        "<div class='customTableCell'>" + this.eGui.innerHTML + "</div></div>";


        this.eGui.innerHTML = tableHtml;
        // this.eGui.innerHTML = bulkControlHtml + this.eGui.innerHTML;


        this.eBulkTextBox = this.eGui.querySelector(".pq-grid-hd-search-field");
        this.onTextChange = this.onBulkTextChange.bind(this, agParams.api.gridOptionsWrapper.gridOptions.currentInstance);

        if (this.eBulkTextBox != null || this.eBulkTextBox != undefined) {
            this.eBulkTextBox.addEventListener('change', this.onTextChange);
        }

        this.eBulkRichTextbox = this.eGui.querySelector(".clsRichTextbox");
        if (this.eBulkRichTextbox != null || this.eBulkRichTextbox != undefined) {
            this.onRichTextboxClick = this.onBulkRichTextClick.bind(this, currentHeaderInstance, false);
            this.eBulkRichTextbox.addEventListener('click', this.onRichTextboxClick);
        }
        this.eBulkMultiTextbox = this.eGui.querySelector(".clsMultiTextbox");
        if (this.eBulkMultiTextbox != null || this.eBulkMultiTextbox != undefined) {
            this.onMultiTextboxClick = this.onBulkRichTextClick.bind(this, currentHeaderInstance, true);
            this.eBulkMultiTextbox.addEventListener('click', this.onMultiTextboxClick);
        }
        this.eBulkDropDownTextBox = this.eGui.querySelector(".ddt-textbox");
        if (this.eBulkDropDownTextBox != null || this.eBulkDropDownTextBox != undefined) {
            //console.log(this.eBulkDropDownTextBox);
            this.onDropDownTextChange = this.onBulkDroptDownTextChange.bind(this, agParams.api.gridOptionsWrapper.gridOptions.currentInstance);
            this.eBulkDropDownTextBox.addEventListener('change', this.onDropDownTextChange);
        }
    }
    else {
        var tableHtml = "<div class= 'customTableRow'> " +
                                      "<div class='customTableCell'>" + this.eGui.innerHTML + "</div></div>";


        this.eGui.innerHTML = tableHtml;
    }


    /*
    if (this.agParams.column.colId != "0") {
        if (agParams.column.colDef.editable == true && agParams.column.colDef.cellEditor == "agSelectCellEditor") {
            var dropdownId = 'ddl' + currentHeaderInstance.agParams.column.colId;
            dropDownHtml = '<select  class = "customDropDown" id="' + dropdownId + '"></select>';
            this.eGui.innerHTML = dropDownHtml + this.eGui.innerHTML;
            currentHeaderInstance.bulkUpdateDropdown = this.eGui.querySelector("#" + dropdownId);
            //  var bulkUpdateData = new BulkUpdateData(currentInstance.bulkUpdateDropdown, currentInstance.agParams);

           // var data = agParams.api.gridOptionsWrapper.gridOptions.rowData;
                var newOption = document.createElement("option");

              

              //  var text = row[currentHeaderInstance.agParams.column.colId];

                agParams.api.gridOptionsWrapper.gridOptions.currentInstance.design.Elements.forEach(function (column) {
                    if (column.GeneratedName == currentHeaderInstance.agParams.column.colId)
                    {
                        var items = column.Items;
                        for (i = 0; i < items.length; i++) {
                            newOption.text = items[i].ItemText;
                            newOption.value = items[i].ItemValue;
                            currentHeaderInstance.bulkUpdateDropdown.appendChild(newOption);
                        };
                    }
                });

            
            var value = text;
                var valueExistsinOption = false;
            
                var options = currentHeaderInstance.bulkUpdateDropdown.options;
                //todo for int 
                for(i =0; i<options.length; i++){
                    if (options[i].value == text) {
                        valueExistsinOption = true;
                    }
                };
                
                if (valueExistsinOption == false) {
                    if (agParams.column.colDef.cellEditorParams != undefined && text != "") {
                        text = agParams.column.colDef.cellEditorParams.values[text];
                    }
                    newOption.text = text;
                    newOption.value = value;
                    currentHeaderInstance.bulkUpdateDropdown.appendChild(newOption);
                }
                
        }
      }
    */
    this.eMenuButton = this.eGui.querySelector(".customHeaderMenuButton");
    this.eSortDownButton = this.eGui.querySelector(".customSortDownLabel");
    this.eSortUpButton = this.eGui.querySelector(".customSortUpLabel");
    this.eSortRemoveButton = this.eGui.querySelector(".customSortRemoveLabel");



    if (currentHeaderInstance.agParams.enableMenu) {
        this.onMenuClickListener = this.onMenuClick.bind(this);
        this.eMenuButton.addEventListener('click', this.onMenuClickListener);
    }

    if (currentHeaderInstance.agParams.enableSorting) {
        this.onSortAscRequestedListener = this.onSortRequested.bind(this, 'asc');
        if (this.eSortDownButton != null) {
            this.eSortDownButton.addEventListener('click', this.onSortAscRequestedListener);
            this.onSortDescRequestedListener = this.onSortRequested.bind(this, 'desc');
        }

        if (this.eSortUpButton != null) {
            this.eSortUpButton.addEventListener('click', this.onSortDescRequestedListener);
        }
        this.onRemoveSortListener = this.onSortRequested.bind(this, '');

        if (this.eSortRemoveButton != null) {
            this.eSortRemoveButton.addEventListener('click', this.onRemoveSortListener);
        }


        this.onSortChangedListener = this.onSortChanged.bind(this);
        if (this.agParams != null && this.agParams.column != null) {
            this.agParams.column.addEventListener('sortChanged', this.onSortChangedListener);
        }
        this.onSortChanged();
    } else {
        /*this.eGui.removeChild(this.eSortDownButton);
        this.eGui.removeChild(this.eSortUpButton);
        this.eGui.removeChild(this.eSortRemoveButton);*/
    }


};
CustomHeader.prototype.RemoveChild = function (element) {
    if (element.remove != undefined) {
        element.remove();
    }
}
CustomHeader.prototype.onSortChanged = function () {
    function deactivate(toDeactivateItems) {
        toDeactivateItems.forEach(function (toDeactivate) {
            toDeactivate.className = toDeactivate.className.split(' ')[0]
        });
    }

    function activate(toActivate) {
        toActivate.className = toActivate.className + " active";
    }

    if (this.agParams.column.isSortAscending()) {
        deactivate([this.eSortUpButton, this.eSortRemoveButton]);
        activate(this.eSortDownButton)
    } else if (this.agParams.column.isSortDescending()) {
        deactivate([this.eSortDownButton, this.eSortRemoveButton]);
        activate(this.eSortUpButton)
    } else {
        deactivate([this.eSortUpButton, this.eSortDownButton]);
        activate(this.eSortRemoveButton)
    }
};

CustomHeader.prototype.getGui = function () {
    return this.eGui;
};

CustomHeader.prototype.onBulkRichTextClick = function (params, isMultTextbox) {
    dialog = new repeaterCellDialog(this, RichTextBoxRenderingFrom.Column, isMultTextbox);
    dialog.show();
}
//stores the previous value of each column Id : on grid scroll column is redraw then retrive back and set value to control and also used in bulk update 
CustomHeader.prototype.onBulkTextChange = function (currentGridInstance) {


    var item = {
        id: this.eBulkTextBox.id,
        value: this.eBulkTextBox.value
    };

    if (this.eBulkDropDownTextBox != null) {
        if (item.value == "Enter Unique Response") {
            this.eBulkDropDownTextBox.setAttribute("style", "display:block");
        }
        else {
            this.eBulkDropDownTextBox.setAttribute("style", "display:none");
        }
    }
    //console.log(item);
    this.addBulkItems(item, currentGridInstance);
};
CustomHeader.prototype.onBulkDroptDownTextChange = function (currentGridInstance) {


    var item = {
        id: this.eBulkDropDownTextBox.id,
        value: this.eBulkDropDownTextBox.value
    };
    //console.log(item);
    this.addBulkItems(item, currentGridInstance);
};
CustomHeader.prototype.addBulkItems = function (item, currentGridInstance) {

    //console.log("id : " + item.id + " Value: " + item.value);
    var gridOptions = GridApi.getCurrentGridInstance(currentGridInstance).gridOptions;
    gridOptions.api.clearFocusedCell();
    var indexFound = -1, len = currentGridInstance.bulkData.length;
    for (var index = 0; index < len; index++) {
        if (item.id == currentGridInstance.bulkData[index].id) {
            indexFound = index;
            break;
        }
    }

    if (indexFound == -1) {
        currentGridInstance.bulkData.push(item);
    }
    else {
        currentGridInstance.bulkData[indexFound] = item;
    }

}
CustomHeader.prototype.onMenuClick = function () {
    this.agParams.showColumnMenu(this.eMenuButton);
};

CustomHeader.prototype.onSortRequested = function (order, event) {
    this.agParams.setSort(order, event.shiftKey);
};

CustomHeader.prototype.destroy = function () {
    if (this.onMenuClickListener) {
        this.eMenuButton.removeEventListener('click', this.onMenuClickListener)
    }
    if (this.onSortRequestedListener) {
        this.eSortDownButton.removeEventListener('click', this.onSortRequestedListener);
        this.eSortUpButton.removeEventListener('click', this.onSortRequestedListener);
        this.eSortRemoveButton.removeEventListener('click', this.onSortRequestedListener);
    }
    if (this.onSortChangedListener) {
        this.agParams.column.removeEventListener('sortChanged', this.onSortChangedListener);
    }
};




function RichTextBoxCellRenderer() {
}

RichTextBoxCellRenderer.prototype.init = function (params) {
    this.eGui = document.createElement('DIV');
    //this.eGui = params.eGridCell;

    if (params.value !== "" && params.value !== undefined && params.value !== null) {
        this.eGui.innerHTML = '<span>' + params.value + '</span>';
    }
    else {
        this.eGui.innerHTML = "";
    }
    this.eGui.className = "cellRichTextbox";
    params.eGridCell.appendChild(this.eGui);
};

RichTextBoxCellRenderer.prototype.getGui = function () {
    return this.eGui;
};

function extractValues(mappings) {
    return Object.keys(mappings);
}


function lookupValue(mappings, key, cellEditor) {
    if (cellEditor.name != undefined) {
        if (cellEditor.name == 'MultiSelect') {
            var valuesWithCommaSep = '';
            var keyArry = [];
            if (key instanceof Array) {
                keyArry = key;
                if (key.length == 0) {
                    return "";
                }
            }
            else {
                if (key == "") {
                    return "";
                };
                if (key != null) {
                    if (GLOBAL.applicationName.toLowerCase() == 'ebenefitsync') {
                        if (key != undefined || key != "undefined") {
                            return key;
                        }
                        else {

                            return '';
                        }
                    }
                    else {
                        keyArry = key.split(',');
                    }
                }
            }
            $.each(keyArry, function (index, key) {
                if (valuesWithCommaSep == '') {
                    valuesWithCommaSep = mappings[key];
                }
                else {
                    valuesWithCommaSep = valuesWithCommaSep + "," + mappings[key];
                }
            });

            return valuesWithCommaSep;
        }
    }
    if (mappings[key] == undefined) {
        return key;
    }
    else {
        return mappings[key];
    }
}

function lookupKey(mappings, name, cellEditor) {
    if (cellEditor.name != undefined) {
        if (cellEditor.name == 'MultiSelect') {
            var valuesWithCommaSep = '';
            var keyArry = [];
            if (name instanceof Array) {
                keyArry = name;
                if (name.length == 0) {
                    return "";
                }
            }
            else {
                if (name == "") {
                    return "";
                };
                if (name != null) {
                    if (GLOBAL.applicationName.toLowerCase() == 'ebenefitsync') {
                        if (name != undefined || name != "undefined") {
                            return name;
                        }
                        else {

                            return '';
                        }
                    }
                    else {
                        keyArry = name.split(',');
                    }
                }
            }
            var mapping = mappings;
            $.each(keyArry, function (index, key) {
                $.each(mapping, function (indexM, keyM) {
                    if(keyM == key)
                    {
                        if (valuesWithCommaSep == '') {
                            valuesWithCommaSep = indexM;
                        }
                        else {
                            valuesWithCommaSep = valuesWithCommaSep + "," + indexM;
                        }
                    }
                });
            });

            return valuesWithCommaSep;
        }
    }

    for (var key in mappings) {
        if (mappings.hasOwnProperty(key)) {
            if (name === mappings[key]) {
                return key;
            }
        }
    }
    return name;
}


function getRowHeight(params) {
    // assuming 50 characters per line, working how how many lines we need
    var maxLen = 50;
    var maxlenProp = "";
    for (var prop in params.data) {
        if (params.data.hasOwnProperty(prop)) {

            if (params.data[prop] !== undefined && params.data[prop] !== null) {
                var len = params.data[prop].length;
                if (len > maxLen) {
                    maxLen = len;
                    maxlenProp = prop;
                }
            }
        }
    }

    if (maxLen > 1000) {
        //console.log(maxLen + "  "  + (18 * (Math.floor(maxLen / 45) + 1)).toString());

        if (GLOBAL.applicationName.toLowerCase() == 'ebenefitsync') {
            if (maxlenProp.indexOf('Standard') != -1 || maxlenProp.indexOf('Allowable') != -1) {
                return 25;
            }
            else {
                return 420;
            }
        }
        else {
            return 420;
        }
    }
    else if (maxLen > 100) {
        if (GLOBAL.applicationName.toLowerCase() == 'ebenefitsync') {
            if (maxlenProp.indexOf('Standard') != -1 || maxlenProp.indexOf('Allowable') != -1) {
                return 25;
            }
            else {
                return 18 * (Math.floor(maxLen / 45) + 1)
            }
        }
        else {
            return 18 * (Math.floor(maxLen / 45) + 1)
        }
    }
    else {
        return 25;
    }


    if (maxLen > 50) {
        console.log(maxLen);
        //console.log(maxLen + "  "  + (18 * (Math.floor(maxLen / 45) + 1)).toString());
        return 455;
    }
    else {
        return 25;
    }
}


function autoSizeAll(gridOptions) {
    var allColumnIds = [];
    gridOptions.columnApi.getAllColumns().forEach(function (column) {
        allColumnIds.push(column.colId);
    });
    gridOptions.columnApi.autoSizeColumns(allColumnIds);
}


function filter(array, element) {
    //   return array.filter(e => e.value != element);
    //  return $(array).filter(function (x) { return x.value !== element });

    var filterArry = [];

    array.forEach(function (item) {
        if (element !== item.value) {
            filterArry.push(item);
        }
    });


    return filterArry;


    /*var result = $.grep(array, function (item) {
        return item.value !== element;
    });
    return result;*/
}

function CustomDateComponent() {
}

CustomDateComponent.prototype.init = function (params) {
    var template =
        "   <span class='reset'>x</span>" +
        "   <input class='mm' placeholder='mm' maxLength='2'/>" +
        "   <span class='divider'>/</span>" +
        "   <input class='dd' placeholder='dd'maxLength='2'/>" +
        "   <span class='divider'>/</span>" +
        "   <input class='yyyy' placeholder='yyyy' maxLength='4'/>";

    this.params = params;

    this.eGui = document.createElement('div');
    this.eGui.className = 'filter';
    this.eGui.innerHTML = template;

    this.eReset = this.eGui.querySelector('.reset');
    this.eDD = this.eGui.querySelector('.dd');
    this.eMM = this.eGui.querySelector('.mm');
    this.eYYYY = this.eGui.querySelector('.yyyy');

    this.eReset.addEventListener('click', this.onResetDate.bind(this));
    this.eDD.addEventListener('input', this.onDateChanged.bind(this, 'dd'));
    this.eMM.addEventListener('input', this.onDateChanged.bind(this, 'mm'));
    this.eYYYY.addEventListener('input', this.onDateChanged.bind(this, 'yyyy'));

    this.date = null;
};

CustomDateComponent.prototype.getGui = function () {
    return this.eGui;
};


CustomDateComponent.prototype.onResetDate = function () {
    this.setDate(null);
    this.params.onDateChanged();
};

CustomDateComponent.prototype.onDateChanged = function (on, newValue) {
    this.date = this.parseDate(
        this.eDD.value,
        this.eMM.value,
        this.eYYYY.value
    );
    this.params.onDateChanged();
};

CustomDateComponent.prototype.parseDate = function (dd, mm, yyyy) {
    //If any of the three input date fields are empty, stop and return null
    if (dd.trim() === '' || mm.trim() === '' || yyyy.trim() === '') {
        return null;
    }

    let day = Number(dd);
    let month = Number(mm);
    let year = Number(yyyy);

    let date = new Date(year, month - 1, day);

    //If the date is not valid
    if (isNaN(date.getTime())) {
        return null;
    }

    //Given that new Date takes any garbage in, it is possible for the user to specify a new Date
    //like this (-1, 35, 1) and it will return a valid javascript date. In this example, it will
    //return Sat Dec 01    1 00:00:00 GMT+0000 (GMT) - Go figure...
    //To ensure that we are not letting non sensical dates to go through we check that the resultant
    //javascript date parts (month, year and day) match the given date fields provided as parameters.
    //If the javascript date parts don't match the provided fields, we assume that the input is nonsensical
    // ... ie: Day=-1 or month=14, if this is the case, we return null
    //This also protects us from non sensical dates like dd=31, mm=2 of any year
    if (date.getDate() !== day || date.getMonth() + 1 !== month || date.getFullYear() !== year) {
        return null;
    }

    return date;
};
CustomDateComponent.prototype.getDate = function () {
    return this.date;
};

CustomDateComponent.prototype.setDate = function (date) {
    if (date === null) {
        this.eDD.value = '';
        this.eMM.value = '';
        this.eYYYY.value = '';
        this.date = null;
    } else {
        this.eDD.value = date.getDate() + '';
        this.eMM.value = (date.getMonth() + 1) + '';
        this.eYYYY.value = date.getFullYear() + '';
        this.date = date;
    }
};


function booleanCleaner(value) {
    if (value === null || value == undefined) {
        return false;
    }
    if (value.toLowerCase() === "true" || value === true || value === 1 || value.toLowerCase() === 'yes') {
        return true;
    } else if (value.toLowerCase() === "false" || value === false || value === 0 || value.toLowerCase() === 'no') {
        return false;
    }
    else if (value === "") {
        return false;
    }
    else {
        return null;
    }

}


function booleanCellRenderer(params) {
    if (params.value === undefined) {
        return "";
    }
    var valueCleaned = booleanCleaner(params.value);
    if (valueCleaned === true) {
        return "<span title='true' class='ag-icon ag-icon-tick content-icon'></span>";
    } else if (valueCleaned === false) {
        return "<span title='false' class='ag-icon ag-icon-cross content-icon'></span>";
    } else if (params.value !== null && params.value !== undefined) {
        return params.value.toString();
    } else {
        return "";
    }
}

function booleanComparator(value1, value2) {
    var value1Cleaned = booleanCleaner(value1);
    var value2Cleaned = booleanCleaner(value2);
    var value1Ordinal = value1Cleaned === true ? 0 : (value1Cleaned === false ? 1 : 2);
    var value2Ordinal = value2Cleaned === true ? 0 : (value2Cleaned === false ? 1 : 2);
    return value1Ordinal - value2Ordinal;
}

function floatComparator(value1, value2) {
    var value1Number = parseFloat(value1);
    var value2Number = parseFloat(value2);

    if (value1Number === null && value2Number === null) {
        return 0;
    }
    if (value1Number === null) {
        return -1;
    }
    if (value2Number === null) {
        return 1;
    }

    return value1 - value2;
}

function highlightDropdownTextBox(params) {
    var backgroundColor = { 'background-color': '' };
    var currentInstance = params.api.gridOptionsWrapper.gridOptions.currentInstance;

    var elementDesign = currentInstance.design.Elements.filter(function (dt) {
        if (dt.GeneratedName == params.column.colDef.field) {
            return dt;
        }
    })
    var isDropdownOption = false;
    if (elementDesign != null && elementDesign.length > 0) {
        $.each(elementDesign[0].Items, function (idx, dt) {
            if (dt.ItemText == params.value) {
                isDropdownOption = true;
                return true;
            }
        })
    }
    if (isDropdownOption) {
        return { 'background-color': '' };
    }
    else {
        if (params.value != "") {
            return { 'background-color': '#ffffb3' };
        }
    }
}

/*filter sample
  {headerName: "Sport", field: "sport", width: 110, suppressMenu:true, filter: 'agTextColumnFilter'},
    {headerName: "Gold", field: "gold", width: 100, filter: 'agNumberColumnFilter', filterParams:{applyButton:true}, suppressMenu:true},
    {headerName: "Silver", field: "silver", width: 100, filter: 'agNumberColumnFilter', floatingFilterComponentParams:{suppressFilterButton:true}},
    {headerName: "Bronze", field: "bronze", width: 100, filter: 'agNumberColumnFilter', floatingFilterComponentParams:{suppressFilterButton:true}},
    {headerName: "Total", field: "total", width: 100, filter: 'agNumberColumnFilter', suppressFilter: true}
];


*/


function setJSRenderProperty(formData, elementFullName, value) {
    var obj = {};

    obj[elementFullName] = value;
    $.observable(formData).setProperty(obj);
}



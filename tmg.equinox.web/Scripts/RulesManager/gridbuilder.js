document.addEventListener('DOMContentLoaded', function () {
    agGrid.LicenseManager.setLicenseKey(license.agGrid);
    gridDiv = document.querySelector('#myGrid');
    //new agGrid.Grid(gridDiv, gridOptions);
    //createData();

    //api = gridOptions.api;
    //columnApi = gridOptions.columnApi;
});

// for easy access in the dev console, we put api and columnApi into global variables
var isSmall = false;
var api, columnApi;
var gridDiv;
var booleanValues = [true, "true", false, "false"];
var size = 'fill'; // model for size select
var width = '100%'; // the div gets its width and height from here
var height = '100%';
var rowSelection = 'checkbox';

var gridOptions = {
    statusBar: {
        statusPanels: [
            { statusPanel: 'agTotalAndFilteredRowCountComponent', key: 'totalAndFilter', align: 'left' },
            { statusPanel: 'agSelectedRowCountComponent', align: 'left' },
            { statusPanel: 'agAggregationComponent', align: 'right' }
        ]
    },
    components: {
        personFilter: PersonFilter,
        booleanCellRenderer: booleanCellRenderer,
        booleanFilterCellRenderer: booleanFilterCellRenderer
    },
    defaultExportParams: {
        columnGroups: true,
        headerRowHeight: 30,
        rowHeight: 22
    },
    defaultColDef: {
        minWidth: 50,
        sortable: true,
        filter: true,
        resizable: true
    },
    enableCellChangeFlash: true,
    rowDragManaged: true,
    popupParent: document.body,
    floatingFilter: !isSmall,
    rowGroupPanelShow: isSmall ? undefined : 'always', // on of ['always','onlyWhenGrouping']
    suppressMenuHide: isSmall,
    pivotPanelShow: 'always', // on of ['always','onlyWhenPivoting']
    pivotColumnGroupTotals: 'before',
    pivotRowTotals: 'before',
    enterMovesDownAfterEdit: true,
    enterMovesDown: true,
    enableCharts: true,
    multiSortKey: 'ctrl',
    animateRows: true,
    enableRangeSelection: true,
    enableRangeHandle: false,
    enableFillHandle: false,
    rowSelection: "multiple", // one of ['single','multiple'], leave blank for no selection
    rowDeselection: true,
    quickFilterText: null,
    groupSelectsChildren: true, // one of [true, false]
    // pagination: true,
    // paginationPageSize: 20,
    // groupSelectsFiltered: true,
    suppressRowClickSelection: true, // if true, clicking rows doesn't select (useful for checkbox selection)
    //autoGroupColumnDef: groupColumn,
    sideBar: {
        toolPanels: [
            {id: 'columns',labelDefault: 'Columns',labelKey: 'columns',iconKey: 'columns',toolPanel: 'agColumnsToolPanel',},
            {id: 'filters',labelDefault: 'Filters',labelKey: 'filters',iconKey: 'filter',toolPanel: 'agFiltersToolPanel',}
        ],
        defaultToolPanel: 'columns',
        hiddenByDefault: isSmall
    },
    onRowSelected: rowSelected, //callback when row selected
    onSelectionChanged: selectionChanged, //callback when selection changed,
    aggFuncs: {'zero': function () {return 0;}},
    getBusinessKeyForNode: function (node) {
        if (node.data) {
            return node.data.name;
        } else {
            return '';
        }
    },
    defaultGroupSortComparator: function (nodeA, nodeB) {
        if (nodeA.key < nodeB.key) {
            return -1;
        } else if (nodeA.key > nodeB.key) {
            return 1;
        } else {
            return 0;
        }
    },
    onRowClicked: function (params) {
        // console.log("Callback onRowClicked: " + (params.data?params.data.name:null) + " - " + params.event);
    },
    onRowDoubleClicked: function (params) {
        // console.log("Callback onRowDoubleClicked: " + params.data.name + " - " + params.event);
    },
    onGridSizeChanged: function (params) {
        console.log("Callback onGridSizeChanged: ", params);
    },
    onCellClicked: function (params) {
        // console.log("Callback onCellClicked: " + params.value + " - " + params.colDef.field + ' - ' + params.event);
    },
    onCellDoubleClicked: function (params) {
        // console.log("Callback onCellDoubleClicked: " + params.value + " - " + params.colDef.field + ' - ' + params.event);
    },
    onGridReady: function (event) {
        console.log('Callback onGridReady: api = ' + event.api);
    },
    onRowGroupOpened: function (event) {
        console.log('Callback onRowGroupOpened: node = ' + event.node.key + ', ' + event.node.expanded);
    }
};

var ruleDefaultCols = [
    {
        headerName: 'Rule Definition',
        children: [
            {
                headerName: 'Rule Code',rowDrag: true,field: 'RuleCode',editable: false,enableRowGroup: true,filter: 'personFilter',cellClass: 'vAlign',
                checkboxSelection: function (params) {
                    // we put checkbox on the name if we are not doing grouping
                    return params.columnApi.getRowGroupColumns().length === 0;
                },
                headerCheckboxSelection: function (params) {
                    // we put checkbox on the name if we are not doing grouping
                    return params.columnApi.getRowGroupColumns().length === 0;
                },
                headerCheckboxSelectionFilteredOnly: true
            },
            {headerName: "Rule Name", field: "RuleName", width: 200, editable: false, filter: 'agSetColumnFilter',cellClass: 'vAlign',enableRowGroup: true,enablePivot: true,filterParams: {selectAllOnMiniFilter: true,newRowsAction: 'keep',clearButton: true}},
            {headerName: "Rule Description", field: "Description", width: 300, editable: false, filter: 'agSetColumnFilter',cellClass: 'vAlign',enableRowGroup: true,enablePivot: true,filterParams: {selectAllOnMiniFilter: true,newRowsAction: 'keep',clearButton: true}},
            {headerName: "Rule Type", field: "Type", width: 150, editable: false,filter: 'agSetColumnFilter',enableRowGroup: true,cellClass: 'vAlign',enablePivot: true,filterParams: {newRowsAction: 'keep',selectAllOnMiniFilter: true,clearButton: true}}
        ]
    },
    {
        headerName: 'Source Elements',
        children: [
            {headerName: "Source(s)", field: "SourceElement", width: 300, editable: false, filter: 'agSetColumnFilter',enableRowGroup: true,enablePivot: true,filterParams: {newRowsAction: 'keep',selectAllOnMiniFilter: true,clearButton: true}},
            {headerName: "Key Filter", field: "KeyFilter", filter: 'agSetColumnFilter', editable: true, width: 150,enableRowGroup: true,enablePivot: true,cellClass: 'booleanType',cellRenderer: 'booleanCellRenderer', cellStyle: {"text-align": "center"}, comparator: booleanComparator,floatCell: true,filterParams: {cellRenderer: 'booleanFilterCellRenderer',selectAllOnMiniFilter: true,newRowsAction: 'keep',clearButton: true}}
        ]
    },
    {
        headerName: 'Target Elements',
        children: [
            {headerName: "Target(s)", field: "TargetElement", width: 300, editable: false, filter: 'agSetColumnFilter',enableRowGroup: true,enablePivot: true,filterParams: {newRowsAction: 'keep',selectAllOnMiniFilter: true,clearButton: true}},
            {headerName: "Key Filter", field: "KeyFilter", filter: 'agSetColumnFilter', editable: true, width: 150,enableRowGroup: true,enablePivot: true,cellClass: 'booleanType',cellRenderer: 'booleanCellRenderer', cellStyle: {"text-align": "center"}, comparator: booleanComparator,floatCell: true,filterParams: {cellRenderer: 'booleanFilterCellRenderer',selectAllOnMiniFilter: true,newRowsAction: 'keep',clearButton: true}}
        ]
    }
];

var defaultCols;
var defaultColCount;
defaultCols = ruleDefaultCols;
defaultColCount = 8;

function filterDoubleClicked(event) {
    setInterval(function () {
        gridOptions.api.ensureIndexVisible(Math.floor(Math.random() * 100000));
    }, 4000);
}

function toggleToolPanel() {
    var showing = gridOptions.api.isToolPanelShowing();
    gridOptions.api.showToolPanel(!showing);
}

function getRowCount() {
    return 100;
}

var loadInstance = 0;

function createData() {
    loadInstance++;

    var loadInstanceCopy = loadInstance;
    gridOptions.api.showLoadingOverlay();

    var colDefs = defaultCols;
    var rowCount = getRowCount();
    var colCount = 8;

    var row = 0;
    var data = [];

    var intervalId = setInterval(function () {
        if (loadInstanceCopy != loadInstance) {
            clearInterval(intervalId);
            return;
        }

        for (var i = 0; i < 1000; i++) {
            if (row < rowCount) {
                var rowItem = createRowItem(row, colCount);
                data.push(rowItem);
                row++;
            }
        }

        if (row >= rowCount) {
            clearInterval(intervalId);
            window.setTimeout(function () {
                gridOptions.api.setColumnDefs(colDefs);
                gridOptions.api.setRowData(data);
            }, 0);
        }

    }, 0);
}

function createRowItem(row, colCount) {
    var rowItem = {};
    rowItem.RuleCode = "VM001";
    rowItem.RuleName = "Coinsurance Validations";
    rowItem.Description = "Coinsurance of INN should be 2 times to ONN";
    rowItem.Type = "validation";
    rowItem.SourceElement = "abc";
    rowItem.KeyFilter = "abc=pqr";
    rowItem.TargetElement="pqr";
    rowItem.KeyFilter = "npop=miiiiii";

    return rowItem;
}

function selectionChanged(event) {
    console.log('Callback selectionChanged: selection count = ' + gridOptions.api.getSelectedNodes().length);
}

function rowSelected(event) {
    // the number of rows selected could be huge, if the user is grouping and selects a group, so
    // to stop the console from clogging up, we only print if in the first 10 (by chance we know
    // the node id's are assigned from 0 upwards)
    if (event.node.id < 10) {
        var valueToPrint = event.node.group ? 'group (' + event.node.key + ')' : event.node.data.name;
        console.log("Callback rowSelected: " + valueToPrint + " - " + event.node.isSelected());
    }
}

var filterCount = 0;

function onFilterChanged(newFilter) {
    filterCount++;
    var filterCountCopy = filterCount;
    window.setTimeout(function () {
        if (filterCount === filterCountCopy) {
            gridOptions.api.setQuickFilter(newFilter);
        }
    }, 300);
}

function PersonFilter() {
}

PersonFilter.prototype.init = function (params) {
    this.valueGetter = params.valueGetter;
    this.filterText = null;
    this.params = params;
    this.setupGui();
};

// not called by ag-Grid, just for us to help setup
PersonFilter.prototype.setupGui = function () {
    this.gui = document.createElement('div');
    this.gui.innerHTML =
        '<div style="padding: 4px;">' +
        '<div style="font-weight: bold;">Custom Athlete Filter</div>' +
        '<div class="ag-input-wrapper"><input style="margin: 4px 0px 4px 0px;" type="text" id="filterText" placeholder="Full name search..."/></div>' +
        '<div style="margin-top: 20px; width: 200px;">This filter does partial word search on multiple words, e.g. "mich phel" still brings back Michael Phelps.</div>' +
        '<div style="margin-top: 20px; width: 200px;">Just to illustrate that anything can go in here, here is an image:</div>' +
        '<div><img src="images/ag-Grid2-200.png" style="width: 150px; text-align: center; padding: 10px; margin: 10px; border: 1px solid lightgrey;"/></div>' +
        '</div>';

    this.eFilterText = this.gui.querySelector('#filterText');
    this.eFilterText.addEventListener("input", this.onFilterChanged.bind(this));
};

PersonFilter.prototype.setFromFloatingFilter = function (filter) {
    this.eFilterText.value = filter;
    this.onFilterChanged();
};

PersonFilter.prototype.onFilterChanged = function () {
    this.extractFilterText();
    this.params.filterChangedCallback();
};

PersonFilter.prototype.extractFilterText = function () {
    this.filterText = this.eFilterText.value;
};

PersonFilter.prototype.getGui = function () {
    return this.gui;
};

PersonFilter.prototype.doesFilterPass = function (params) {
    // make sure each word passes separately, ie search for firstname, lastname
    var passed = true;
    var valueGetter = this.valueGetter;
    this.filterText.toLowerCase().split(" ").forEach(function (filterWord) {
        var value = valueGetter(params);
        if (value.toString().toLowerCase().indexOf(filterWord) < 0) {
            passed = false;
        }
    });

    return passed;
};

PersonFilter.prototype.isFilterActive = function () {
    var isActive = this.filterText !== null && this.filterText !== undefined && this.filterText !== '';
    return isActive;
};

PersonFilter.prototype.getModelAsString = function (model) {
    return model ? model : '';
};

PersonFilter.prototype.getModel = function () {
    return this.eFilterText.value;
};

// lazy, the example doesn't use setModel()
PersonFilter.prototype.setModel = function (model) {
    this.eFilterText.value = model;
    this.extractFilterText();
};

PersonFilter.prototype.destroy = function () {
    this.eFilterText.removeEventListener("input", this.onFilterChanged);
};

function currencyCssFunc(params) {
    if (params.value !== null && params.value !== undefined && params.value < 0) {
        return {"color": "red", "font-weight": "bold"};
    } else {
        return {};
    }
}

function booleanComparator(value1, value2) {
    var value1Cleaned = booleanCleaner(value1);
    var value2Cleaned = booleanCleaner(value2);
    var value1Ordinal = value1Cleaned === true ? 0 : (value1Cleaned === false ? 1 : 2);
    var value2Ordinal = value2Cleaned === true ? 0 : (value2Cleaned === false ? 1 : 2);
    return value1Ordinal - value2Ordinal;
}

var count = 0;

function booleanCellRenderer(params) {
    count++;
    if (count <= 1) {
        // params.api.onRowHeightChanged();
    }

    var valueCleaned = booleanCleaner(params.value);
    if (valueCleaned === true) {
        return "<span title='true' class='ag-icon ag-icon-tick content-icon'></span>";
    } else if (valueCleaned === false) {
        return "<span title='false' class='ag-icon ag-icon-cross content-icon'></span>";
    } else if (params.value !== null && params.value !== undefined) {
        return params.value.toString();
    } else {
        return null;
    }
}

function booleanFilterCellRenderer(params) {
    var valueCleaned = booleanCleaner(params.value);
    if (valueCleaned === true) {
        return "<span title='true' class='ag-icon ag-icon-tick content-icon'></span>";
    } else if (valueCleaned === false) {
        return "<span title='false' class='ag-icon ag-icon-cross content-icon'></span>";
    } else {
        return "(empty)";
    }
}

function booleanCleaner(value) {
    if (value === "true" || value === true || value === 1) {
        return true;
    } else if (value === "false" || value === false || value === 0) {
        return false;
    } else {
        return null;
    }
}
var provisionDetails = function () {
    var tableInfo = JSON.parse(data);
    var test = document.getElementById("tableDetaillink");
    var URLs = {
        getTranslationQueueList: "",
    }
    var elementIDs = {
        provisionDetail: 'provisionDetail',
        provisionDetailJQ: "#provisionDetail"

    };
    function init() {
        $(document).ready(function () {
            for (var i = 0; i < tableInfo.length; i++) {
                if (i != 0) {
                    var div = document.createElement('div');
                    div.className = 'row';
                    div.style.height = "50px"
                    document.getElementById(elementIDs.provisionDetail).appendChild(div);
                }
                loadprovisionDetail(tableInfo[i]);
            }

        });
    }
    init();

    function loadprovisionDetail(data) {

        var colmInfo = JSON.parse(data["ColumnJSON"]);
        var rowData = JSON.parse(data["TableDataJSON"]);

        var gridGlobalSettingOption = new GridApi.gridGlobalSetting();

        var columnDefs = [];

        for (var i = 0; i < colmInfo.length; i++) {
            var obj = { headerName: colmInfo[i]["COLUMN_NAME"], field: colmInfo[i]["COLUMN_FIELD"] };
            columnDefs.push(obj);
        }


        var gridOptions = gridGlobalSettingOption.defaultGridOption();
        //gridOptions.pivotPanelShow = false;
        //gridOptions.pivotTotals = false;
        //gridOptions.showToolPanel = false;
        //gridOptions.defaultColDef.enablePivot = false;
        gridOptions.defaultColDef.enableValue = false;
        gridOptions.defaultColDef.enableRowGroup = false;
        gridOptions.defaultColDef.headerComponentParams.enableMenu = false;
        gridOptions.toolPanelSuppressSideButtons = true
        gridOptions.columnDefs = columnDefs;
        gridOptions.rowData = rowData;

        agGrid.LicenseManager.setLicenseKey(license.agGrid);

        var div = document.createElement('div');
        div.className = 'ag-theme-fresh';
        div.id = data["TableName"].replace(/ /g, '_');
        document.getElementById(elementIDs.provisionDetail).appendChild(div);
        var gridId = "#" + div.id;
        var eGridDiv = document.querySelector(gridId);
        new agGrid.Grid(eGridDiv, gridOptions);
        GridApi.renderGridHeaderRow(gridId, data["TableName"], gridOptions);

              
        var pagetoolBar = $(gridId).find(' .customPager');

        td = document.createElement("TD");
        cell = document.createElement("SPAN");
        cell.className = "ui-icon ui-icon-arrowstop-1-s";
        cell.title = 'Export To Excel';
        cell.setAttribute('data-toggle', 'tooltip');
        cell.onclick = function (param) {
            exportToExcel(currentGrid, currentGridData);
        }

        td.appendChild(cell);

        pagetoolBar[0].appendChild(td);

        var currentGrid = gridId;
        var currentGridData = gridOptions;
    }

    function exportToExcel(currentGrid, currentGridData) {
        params = {
            fileName: currentGrid.replace("#", ""),
            sheetName: "Sheet1",
            //Type: ".xlsx"
        };
        currentGridData.api.exportDataAsExcel(params);
    }
}
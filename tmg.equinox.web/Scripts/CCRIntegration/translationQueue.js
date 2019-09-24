var translationQueue = function () {

    var URLs = {
        getTranslationQueueList: "/CCRIntegration/GetTranslationQueue",
        getTranslationLog: '/CCRIntegration/GetTranslationLog/?processGovernance1Up=',
    }
    var elementIDs = {
        translationQueueGrid: "translationQueueGrid",
        translationQueueGridJQ: "#translationQueueGrid",

    };
    function init() {
        $(document).ready(function () {
            loadTranslationGridgrid();
        });
    }
    init();

    function loadTranslationGridgrid() {

        var gridGlobalSettingOption = new GridApi.gridGlobalSetting();

        var columnDefs = [
           {
               headerName: "Process Governance", field: "ProcessGovernance1Up", width: 50, headerTooltip: "Process Governance", filterParams: { selectAllOnMiniFilter: true, clearButton: true, applyButton: true }
               , cellRenderer: function (params) {
                   return "<a href='" + URLs.getTranslationLog + params.data.ProcessGovernance1Up + "' style='text-decoration: underline' + target='_blank'>" + params.value + "</a>"
               }
           },
        { headerName: "Product ID", field: "ProductID", width: 120, headerTooltip: "Product ID", filterParams: { selectAllOnMiniFilter: true, clearButton: true, applyButton: true } },
        { headerName: "Plan Name", field: "ProductName", width: 150, headerTooltip: "Plan Name", filterParams: { selectAllOnMiniFilter: true, clearButton: true, applyButton: true } },
        { headerName: "Account Name", field: "AccountName", headerTooltip: "Account Name", width: 155, filterParams: { selectAllOnMiniFilter: true, clearButton: true, applyButton: true } },
        { headerName: "Folder Name", field: "FolderName", headerTooltip: "Folder Name", width: 150, filterParams: { selectAllOnMiniFilter: true, clearButton: true, applyButton: true } },
        { headerName: "Product Type", field: "ProductType", headerTooltip: "Product Type", width: 120, filterParams: { selectAllOnMiniFilter: true, clearButton: true, applyButton: true } },
        { headerName: "Effective Date", field: "EffectiveDate", headerTooltip: "Effective Date", width: 150, valueFormatter: dateFormatter, filterParams: { selectAllOnMiniFilter: true, clearButton: true, applyButton: true } },
        { headerName: "Status", field: "ProcessStatus1Up", headerTooltip: "Status", width: 130, valueFormatter: statusFormatter, filterParams: { selectAllOnMiniFilter: true, clearButton: true, applyButton: true } },
        { headerName: "Start Date", field: "StartTime", headerTooltip: "Start Date", width: 120, valueFormatter: dateFormatter, filterParams: { selectAllOnMiniFilter: true, clearButton: true, applyButton: true } },
        { headerName: "Processing Time", field: "ProcessTime", headerTooltip: "Processing Time", width: 120, filterParams: { selectAllOnMiniFilter: true, clearButton: true, applyButton: true } },
        ];

        var gridOptions = gridGlobalSettingOption.defaultGridOption();

        gridOptions.pivotPanelShow = false;
        gridOptions.pivotTotals = false;
        gridOptions.showToolPanel = false;
        gridOptions.defaultColDef.enablePivot = false;
        gridOptions.defaultColDef.enableValue = false;
        gridOptions.defaultColDef.enableRowGroup = false;
        gridOptions.defaultColDef.headerComponentParams.enableMenu = false;
        gridOptions.toolPanelSuppressSideButtons = true
        gridOptions.columnDefs = columnDefs;
        gridOptions.rowData = data;

        agGrid.LicenseManager.setLicenseKey(license.agGrid);

        var eGridDiv = document.querySelector(elementIDs.translationQueueGridJQ);
        new agGrid.Grid(eGridDiv, gridOptions);

        GridApi.renderGridHeaderRow(elementIDs.translationQueueGridJQ, "Translation Queue List", gridOptions);

    }

    function dateFormatter(params) {
        if (params.value != null) {
            return moment(params.value).format('MM/DD/YYYY');
        }
    }

    function statusFormatter(params) {
        if (params.value != null) {
            if (params.value == 1) {
                return "Queued";
            }
            else if (params.value == 2) {
                return "In Progress";
            }
            else if (params.value == 3) {
                return "Errored";
            }
            else if (params.value == 4) {
                return "Complete";
            }
            else if (params.value == 5) {
                return "Rollback";
            }
            else if (params.value == 6) {
                return "Finalyzed";
            }
            else if (params.value == 7) {
                return "Not Scheduled";
            }
            else if (params.value == 8) {
                return "Scheduled";
            }
            else if (params.value == 9) {
                return "DataSync Pending";
            }
        }
    }
}
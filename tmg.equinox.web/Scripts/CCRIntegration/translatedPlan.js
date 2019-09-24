var translatedPlan = function () {

    var URLs = {
        getTranslationQueueList: "",
    }
    var elementIDs = {
        translatedListGrid: "translatedListGrid",
        translatedListGridJQ: "#translatedListGrid",
        tableDetaillinkJQ: "#tableDetaillink",
        reportDetaillinkJQ: "#reportDetaillink"
    };
    function init() {
        $(elementIDs.tableDetaillinkJQ).attr('href', "/CCRIntegration/TableDetails/?ProductId=defualt");
        $(elementIDs.reportDetaillinkJQ).attr('href', "/CCRIntegration/ProvisionDetails/?ProductId=defualt");
        $(document).ready(function () {
            loadTranslatedgrid();
        });
    }
    init();

    function loadTranslatedgrid() {

        var gridGlobalSettingOption = new GridApi.gridGlobalSetting();

        var columnDefs = [
           { headerName: "Product ID", field: "ProductID", width: 150, headerTooltip: "Product ID", filterParams: { selectAllOnMiniFilter: true, clearButton: true, applyButton: true } },
           { headerName: "Plan Name", field: "ProductName", width: 150, headerTooltip: "Plan Name", filterParams: { selectAllOnMiniFilter: true, clearButton: true, applyButton: true } },
           { headerName: "Account Name", field: "AccountName", width: 184, headerTooltip: "Account Name", filterParams: { selectAllOnMiniFilter: true, clearButton: true, applyButton: true } },
           { headerName: "Folder Name", field: "FolderName", width: 184, headerTooltip: "Folder Name", filterParams: { selectAllOnMiniFilter: true, clearButton: true, applyButton: true } },
           { headerName: "Product Type", field: "ProductType", width: 150, headerTooltip: "Product Type", filterParams: { selectAllOnMiniFilter: true, clearButton: true, applyButton: true } },
           { headerName: "Effective Date", field: "EffectiveDate", width: 150, headerTooltip: "Effective Date", valueFormatter: dateFormatter, filterParams: { selectAllOnMiniFilter: true, clearButton: true, applyButton: true } },
           { headerName: "Folder Version Number", field: "FolderVersionNumber", width: 150, headerTooltip: "Folder Version Number", filterParams: { selectAllOnMiniFilter: true, clearButton: true, applyButton: true } },
           { headerName: "Processed on", field: "EndTime", width: 150, headerTooltip: "Processed on", valueFormatter: dateFormatter, filterParams: { selectAllOnMiniFilter: true, clearButton: true, applyButton: true } },
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
        gridOptions.rowData = JSON.parse(data);
        gridOptions.onRowSelected = function (e) {
            if (e.node.selected == true) {
                var productId = e.node.data.ProductID;
                if (productId != "" && productId != null) {
                    $(elementIDs.tableDetaillinkJQ).attr('href', "/CCRIntegration/TableDetails/?ProductId=" + productId + "");
                    $(elementIDs.reportDetaillinkJQ).attr('href', "/CCRIntegration/ProvisionDetails/?ProductId=" + productId + "");
                    $("#researchTab ul:first li:eq(1) a").text(productId + " - Table Details");
                    $("#researchTab ul:first li:eq(2) a").text(productId + " - Provisions");
                }
                else {
                    $(elementIDs.tableDetaillinkJQ).attr('href', "/CCRIntegration/TableDetails/?ProductId=defualt");
                    $(elementIDs.reportDetaillinkJQ).attr('href', "/CCRIntegration/ProvisionDetails/?ProductId=defualt");
                    $("#researchTab ul:first li:eq(1) a").text("Table Details");
                    $("#researchTab ul:first li:eq(2) a").text("Provisions");
                }

            }
        };

        agGrid.LicenseManager.setLicenseKey(license.agGrid);

        var eGridDiv = document.querySelector(elementIDs.translatedListGridJQ);
        new agGrid.Grid(eGridDiv, gridOptions);

        GridApi.renderGridHeaderRow(elementIDs.translatedListGridJQ, "Translated Product List", gridOptions);
    }

    function dateFormatter(params) {
        if (params.value != null) {
            return moment(params.value).format('MM/DD/YYYY');
        }
    }
}
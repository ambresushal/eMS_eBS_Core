function advancedConfigurationDialog(uiElement, status, formDesignVersionId, formDesignVersionInstance, uiElementDetail) {
    this.uiElement = uiElement;
    this.formDesignVersionId = formDesignVersionId;
    this.tenantId = 1;
    this.statustext = status;
    this.formDesignVersionInstance = formDesignVersionInstance;
    this.uiElementDetail = uiElementDetail;
    this.configrationData = [];
}

advancedConfigurationDialog.elementIDs = {
    configraionDialog: '#advancedConfigraionDialog',
    accordianConfigraionDialog: '#accordianConfigraionDialog',
    displayTopHeader: '#displayTopHeader',
    displayTitle: '#displayTitle',
    allowPaging: '#allowPaging',
    rowsPerPageDiv: '#rowsPerPageDiv',
    rowsPerPage: '#rowsPerPage',
    freezeColNo: '#freezeColNo',
    ddlFilterMode: '#ddlFilterMode',
    freezeRowNo: '#freezeRowNo',
    exportToExcel: '#exportToExcel',
    exportToCSV: '#exportToCSV',
    rowsPerPageSpan: "#rowsPerPageSpan",
}

advancedConfigurationDialog._isInitialized = false;

//init dialog
advancedConfigurationDialog.init = function () {
    var currentInstance = this;
    if (advancedConfigurationDialog._isInitialized == false) {
        $(advancedConfigurationDialog.elementIDs.configraionDialog).dialog({
            autoOpen: false,
            width: '40%',
            modal: true,
            open: function () {
                $(advancedConfigurationDialog.elementIDs.accordianConfigraionDialog).accordion({ autoHeight: true });
            }
        });
        advancedConfigurationDialog._isInitialized = true;
    }
}

advancedConfigurationDialog.init();

//show dialog
advancedConfigurationDialog.prototype.show = function (isLoaded) {
    if (!isLoaded) {
        $(advancedConfigurationDialog.elementIDs.configraionDialog).dialog('option', 'title', 'Advanced Configuration Settings - ' + this.uiElement.Label);
        $(advancedConfigurationDialog.elementIDs.configraionDialog).dialog('open');
        // register change event of all controls 
        this.registerEvents();
        //load grid
        this.loadConfigration();
    }
    else {
        $(advancedConfigurationDialog.elementIDs.configraionDialog).dialog('open');
    }
}

advancedConfigurationDialog.prototype.loadConfigration = function () {
    var currentInstance = this;
    if (currentInstance.uiElementDetail != undefined && currentInstance.uiElementDetail != null) {
        $(advancedConfigurationDialog.elementIDs.displayTopHeader).prop('checked', currentInstance.uiElementDetail.DisplayTopHeader);
        $(advancedConfigurationDialog.elementIDs.displayTitle).prop('checked', currentInstance.uiElementDetail.DisplayTitle);
        $(advancedConfigurationDialog.elementIDs.freezeColNo).val(currentInstance.uiElementDetail.FrozenColCount);
        $(advancedConfigurationDialog.elementIDs.freezeRowNo).val(currentInstance.uiElementDetail.FrozenRowCount);
        $(advancedConfigurationDialog.elementIDs.allowPaging).prop('checked', currentInstance.uiElementDetail.AllowPaging);
        $(advancedConfigurationDialog.elementIDs.exportToExcel).prop('checked', currentInstance.uiElementDetail.AllowExportToExcel);
        $(advancedConfigurationDialog.elementIDs.exportToCSV).prop('checked', currentInstance.uiElementDetail.AllowExportToCSV);
        $(advancedConfigurationDialog.elementIDs.ddlFilterMode).val(currentInstance.uiElementDetail.FilterMode);

        $(advancedConfigurationDialog.elementIDs.rowsPerPage).val(currentInstance.uiElementDetail.RowsPerPage);

        if (currentInstance.uiElementDetail.AllowPaging) {
            $(advancedConfigurationDialog.elementIDs.rowsPerPageDiv).css("visibility", "visible");
            $(advancedConfigurationDialog.elementIDs.rowsPerPage).val(currentInstance.uiElementDetail.RowsPerPage);
            $(advancedConfigurationDialog.elementIDs.rowsPerPageSpan).text('')
            $(advancedConfigurationDialog.elementIDs.rowsPerPage).parent().removeClass('has-error');
            $(advancedConfigurationDialog.elementIDs.rowsPerPage).removeClass('form-control');
        }
        else {
            $(advancedConfigurationDialog.elementIDs.rowsPerPageDiv).css("visibility", "hidden");
            $(advancedConfigurationDialog.elementIDs.rowsPerPage).val("");
        }
    }
}

// Get updated values for configuration
advancedConfigurationDialog.prototype.getConfigurationData = function () {
    var currentInstance = this;
    currentInstance.configrationData.push({
        DisplayTopHeader: currentInstance.uiElementDetail.DisplayTopHeader,
        DisplayTitle: currentInstance.uiElementDetail.DisplayTitle,
        FrozenColCount: currentInstance.uiElementDetail.FrozenColCount,
        FrozenRowCount: currentInstance.uiElementDetail.FrozenRowCount,
        AllowPaging: currentInstance.uiElementDetail.AllowPaging,
        RowsPerPage: currentInstance.uiElementDetail.RowsPerPage,
        AllowExportToExcel: currentInstance.uiElementDetail.AllowExportToExcel,
        AllowExportToCSV: currentInstance.uiElementDetail.AllowExportToCSV,
        FilterMode: currentInstance.uiElementDetail.FilterMode
    });

    if (currentInstance.configrationData[0].AllowPaging) {
        if (currentInstance.configrationData[0].RowsPerPage == "" ||
            currentInstance.configrationData[0].RowsPerPage == null ||
            currentInstance.configrationData[0].RowsPerPage <= 0 ||
            currentInstance.configrationData[0].RowsPerPage == undefined) {
            currentInstance.configrationData[0].RowsPerPage = 20;
        }
    }

    return currentInstance.configrationData;
}

advancedConfigurationDialog.prototype.registerEvents = function () {
    var currentInstance = this;
    var elementDetails = currentInstance.uiElementDetail;
    // DisplayTopHeader
    $(advancedConfigurationDialog.elementIDs.displayTopHeader).off('change').on('change', function () {
        var hasTopHeader = $(advancedConfigurationDialog.elementIDs.displayTopHeader).is(':checked');
        elementDetails.DisplayTopHeader = hasTopHeader;
    });
    // DisplayTitle
    $(advancedConfigurationDialog.elementIDs.displayTitle).off('change').on('change', function () {
        var hasTitle = $(advancedConfigurationDialog.elementIDs.displayTitle).is(':checked');
        elementDetails.DisplayTitle = hasTitle;
    });
    // AllowPaging
    $(advancedConfigurationDialog.elementIDs.allowPaging).off('change').on('change', function () {
        var hasPaging = $(advancedConfigurationDialog.elementIDs.allowPaging).is(':checked');
        elementDetails.AllowPaging = hasPaging;
        if (hasPaging) {
            $(advancedConfigurationDialog.elementIDs.rowsPerPageDiv).css("visibility", "visible");
            var count = currentInstance.uiElementDetail.RowsPerPage;
            if (count == null || count == undefined || count == "" || count <= 0) {
                elementDetails.RowsPerPage = 20;
                $(advancedConfigurationDialog.elementIDs.rowsPerPage).val(20);
            }
            else {
                $(advancedConfigurationDialog.elementIDs.rowsPerPage).val(currentInstance.uiElementDetail.RowsPerPage);
            }
            $(advancedConfigurationDialog.elementIDs.rowsPerPageSpan).text('')
            $(advancedConfigurationDialog.elementIDs.rowsPerPage).parent().removeClass('has-error');
            $(advancedConfigurationDialog.elementIDs.rowsPerPage).removeClass('form-control');
        }
        else {
            $(advancedConfigurationDialog.elementIDs.rowsPerPageDiv).css("visibility", "hidden");
        }
    });
    // RowsPerPage
    $(advancedConfigurationDialog.elementIDs.rowsPerPage).off('change').on('change', function () {
        var rowCount = $(advancedConfigurationDialog.elementIDs.rowsPerPage).val();
        if (rowCount == "" || rowCount == undefined || rowCount == null) {
            elementDetails.RowsPerPage = rowCount;
            $(advancedConfigurationDialog.elementIDs.rowsPerPage).parent().addClass('has-error');
            $(advancedConfigurationDialog.elementIDs.rowsPerPage).addClass('form-control');
            $(advancedConfigurationDialog.elementIDs.rowsPerPageSpan).text(DocumentDesign.rowsPerPageRequiredMsg);
        }
        else if (rowCount != "" && rowCount != undefined && rowCount != null && rowCount <= 0) {
            elementDetails.RowsPerPage = rowCount;
            $(advancedConfigurationDialog.elementIDs.rowsPerPage).parent().addClass('has-error');
            $(advancedConfigurationDialog.elementIDs.rowsPerPage).addClass('form-control');
            $(advancedConfigurationDialog.elementIDs.rowsPerPageSpan).text(DocumentDesign.rowsPerPageGreaterThanZeroMsg);
        }
        else {
            elementDetails.RowsPerPage = rowCount;
            $(advancedConfigurationDialog.elementIDs.rowsPerPageSpan).text('')
            $(advancedConfigurationDialog.elementIDs.rowsPerPage).parent().removeClass('has-error');
            $(advancedConfigurationDialog.elementIDs.rowsPerPage).removeClass('form-control');
        }

    });
    //FreezwColCount
    $(advancedConfigurationDialog.elementIDs.freezeColNo).off('change').on('change', function () {
        var freezeColNo = $(advancedConfigurationDialog.elementIDs.freezeColNo).val();
        elementDetails.FrozenColCount = freezeColNo;
    });
    //FreezwRowCount
    $(advancedConfigurationDialog.elementIDs.freezeRowNo).off('change').on('change', function () {
        var freezeRowNo = $(advancedConfigurationDialog.elementIDs.freezeRowNo).val();
        elementDetails.FrozenRowCount = freezeRowNo;
    });
    //FilterMode
    $(advancedConfigurationDialog.elementIDs.ddlFilterMode).off('change').on('change', function () {
        var filterMode = $(advancedConfigurationDialog.elementIDs.ddlFilterMode).val();
        elementDetails.FilterMode = filterMode;
    });
    //ExportToExcel 
    $(advancedConfigurationDialog.elementIDs.exportToExcel).off('change').on('change', function () {
        var hasExportToExcel = $(advancedConfigurationDialog.elementIDs.exportToExcel).is(':checked');
        elementDetails.AllowExportToExcel = hasExportToExcel;
    });
    //ExportToCSV
    $(advancedConfigurationDialog.elementIDs.exportToCSV).off('change').on('change', function () {
        var hasExportToCSV = $(advancedConfigurationDialog.elementIDs.exportToCSV).is(':checked');
        elementDetails.AllowExportToCSV = hasExportToCSV;
    });
    //$(advancedConfigurationDialog.elementIDs.configraionDialog).off('dialogclose').on('dialogclose', function (event) {
    //    if (elementDetails.AllowPaging) {
    //        if (elementDetails.RowsPerPage == "" || elementDetails.RowsPerPage == null || elementDetails.RowsPerPage <= 0 || elementDetails.RowsPerPage == undefined) {
    //            elementDetails.RowsPerPage = 20;
    //        }
    //    }
    //});
}

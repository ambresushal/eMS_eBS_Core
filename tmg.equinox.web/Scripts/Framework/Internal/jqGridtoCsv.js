
var JQGridtoCsv = function (gridTableJQ, autoExpandChildGrid, repeaterBuilder) {
    this.gridTableJQ = gridTableJQ;
    this.exportOption = null;
    this.JQGridData = null;
    this.isSubgrid = false;
    this.isGroupHeader = false;
    this.noofGroupedColumns = 0;
    this.allHiddenColumn = null;
    this.allDisplayColumn = null;
    this.allDisplayColumnCount = 0;
    this.groupHeadersOptions = [];
    this.csvData = '';
    this.colNames = null;
    this.colModel = null;
    this.autoExpandChildGrid = autoExpandChildGrid;
    this.repeaterBuilder = repeaterBuilder;
}

JQGridtoCsv.prototype.init = function () {

    this.exportOption = {};

    this.isSubgrid = this.repeaterBuilder.displayChildGridAsPopup ? this.repeaterBuilder.hasChildGrids : $(this.gridTableJQ).jqGrid("getGridParam").subGrid;

    if (this.gridTableJQ == "#userListGrid" || this.gridTableJQ == "#cmTransErrorsGrid" || this.gridTableJQ.selector == "#activityLogGrid" || this.gridTableJQ == "#workQueue" || this.gridTableJQ == "#rulestestergrid") {
        this.colModel = $(this.gridTableJQ).jqGrid("getGridParam", "colModel");
        this.JQGridData = $(this.gridTableJQ).jqGrid('getGridParam', 'data');;
        this.setJQGridDisplayColumns(this.gridTableJQ);
    }
    else {
        if (this.repeaterBuilder.groupHeaders && this.repeaterBuilder.groupHeaders !== null)
            this.groupHeadersOptions = this.repeaterBuilder.groupHeaders;
        if (($(this.gridTableJQ).hasClass("ui-jqgrid-btable") == false)) {
            this.JQGridData = $(this.gridTableJQ).pqGrid("option", "dataModel.data");
        }
        else {
            this.JQGridData = $(this.gridTableJQ).jqGrid('getGridParam', 'data');;
        }
        this.colModel = this.repeaterBuilder.columnModels;
        if (!this.colModel || this.colModel == null || this.colModel.length == 0)
            this.colModel = $(this.gridTableJQ).jqGrid('getGridParam', 'colModel');
        this.setDisplayColumns(this.gridTableJQ);
    }
    if (this.groupHeadersOptions.length > 0) {
        this.isGroupHeader = true;
    }

    //this.colNames = this.repeaterBuilder.columnNames;


    this.setHiddenColumns(this.gridTableJQ);


}

JQGridtoCsv.prototype.getGridData = function (gridTableID) {
    currentInstance = this;
    var rowNum = $(gridTableID).getGridParam('rowNum');
    var allRecords = $(gridTableID).getGridParam('records');
    var totalPages = parseInt((allRecords / rowNum) + 1);
    var repeaterData = [];

   // if (currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId == FormTypes.MASTERLISTFORMID) {
     //   return $(gridTableID).jqGrid('getRowData');
   // }
   // else {
    if (totalPages == 1)
        return $(gridTableID).jqGrid('getRowData');

    currentPage = $(gridTableID).getGridParam('page');

    for (p = 1; p <= totalPages; p++) {

        $(gridTableID).trigger("reloadGrid", [{ page: p }]);

        var pageData = $(gridTableID).jqGrid("getRowData");

        for (var i = 0; i < pageData.length; i++) {
            repeaterData.push(pageData[i]);
        }
        if (p == totalPages) {
            $(gridTableID).trigger("reloadGrid", [{ page: currentPage }]);
        }
    }
    return repeaterData;
    //}
}

JQGridtoCsv.prototype.setHiddenColumns = function () {
    var currentInstance = this;
    currentInstance.allHiddenColumn = {};
    for (key in currentInstance.colModel) {
        if (currentInstance.colModel[key].hidden)
            currentInstance.allHiddenColumn[key] = currentInstance.colModel[key];
    }
}

JQGridtoCsv.prototype.setDisplayColumns = function () {
    var currentInstance = this;
    currentInstance.allDisplayColumn = {};
    for (key in currentInstance.colModel) {
        if ((checkIfPQGridLoaded(false))) {

            if (currentInstance.colModel[key].dataIndx != undefined && !currentInstance.colModel[key].hidden && currentInstance.colModel[key].dataIndx != "expandCollapse") {
                currentInstance.allDisplayColumn[currentInstance.colModel[key].dataIndx] = currentInstance.colModel[key].name;//Enumerable.From(currentInstance.repeaterBuilder.design.Elements).Where(function (element) { return element.GeneratedName == currentInstance.colModel[key].index }).FirstOrDefault().Label;
                currentInstance.allDisplayColumnCount++;                                                           
            }
        }
        else {
            if (currentInstance.colModel[key].index != undefined && !currentInstance.colModel[key].hidden && currentInstance.colModel[key].index != "expandCollapse") {
                currentInstance.allDisplayColumn[currentInstance.colModel[key].index] = currentInstance.colModel[key].index;//Enumerable.From(currentInstance.repeaterBuilder.design.Elements).Where(function (element) { return element.GeneratedName == currentInstance.colModel[key].index }).FirstOrDefault().Label;
                currentInstance.allDisplayColumnCount++;
            }
        }
    }
}

JQGridtoCsv.prototype.setJQGridDisplayColumns = function () {
    var currentInstance = this;
    currentInstance.allDisplayColumn = {};
    for (key in currentInstance.colModel) {
        if (currentInstance.colModel[key].index != undefined && !currentInstance.colModel[key].hidden && currentInstance.colModel[key].index != "expandCollapse") {
            currentInstance.allDisplayColumn[currentInstance.colModel[key].index] = currentInstance.colModel[key].index;//Enumerable.From(currentInstance.repeaterBuilder.design.Elements).Where(function (element) { return element.GeneratedName == currentInstance.colModel[key].index }).FirstOrDefault().Label;
            currentInstance.allDisplayColumnCount++;
        }
    }
}

JQGridtoCsv.prototype.setGroupHeaderToCSV = function () {
    var currentInstance = this;

    var cntMin = 0;
    var cntMax = 0;

    for (key in currentInstance.groupHeadersOptions) {
        if (key == "0") {

            var startIndex = currentInstance.groupHeadersOptions[key].startIndex;

            for (hidCol in currentInstance.allHiddenColumn) {
                if (hidCol < startIndex) {
                    cntMin = cntMin + 1;
                }
                else {
                    cntMax = cntMax + 1;
                }
            }
            startIndex = startIndex - cntMin;
            for (var k = 0; k < startIndex; k++) {
                currentInstance.csvData += "\t";
            }
        }
        currentInstance.csvData += currentInstance.groupHeadersOptions[key].titleText + "\t"
        if (currentInstance.groupHeadersOptions.length == 1) {
            currentInstance.noofGroupedColumns = currentInstance.allDisplayColumnCount - startIndex;
        } else {
            currentInstance.noofGroupedColumns = Math.round(currentInstance.groupHeadersOptions[key].numberOfColumns - (cntMax / currentInstance.groupHeadersOptions.length));
        }

        for (var k = 1; k < currentInstance.noofGroupedColumns; k++) {
            currentInstance.csvData += "\t";
        }
    }
    currentInstance.csvData = currentInstance.removeLastChar(currentInstance.csvData) + "\r\n";
}

JQGridtoCsv.prototype.setColumnsHeaderToCSV = function () {
    var currentInstance = this;
    for (k in currentInstance.allDisplayColumn) {
        currentInstance.csvData += currentInstance.allDisplayColumn[k].replace('<font color=red>*</font>', '').replace("<span class='primaryKey'/>", '') + "\t";
    }
    currentInstance.csvData = currentInstance.removeLastChar(currentInstance.csvData) + "\r\n";
}

JQGridtoCsv.prototype.setRowDatatoCSV = function () {
    var currentInstance = this;
    var cellValue = '';
    if (currentInstance.gridTableJQ.selector == "#activityLogGrid")
        currentInstance.JQGridData = $("#activityLogGrid").jqGrid('getRowData');
    for (i = 0; i < currentInstance.JQGridData.length; i++) {

        var data = currentInstance.JQGridData[i];

        for (col in currentInstance.allDisplayColumn) {
            // Fetch one jqGrid cell's data, but make sure it's a string
            cellValue = '' + data[col];

            if (cellValue == null)
                currentInstance.csvData += "\t";
            else {
                if (cellValue.indexOf("a href") > -1) {
                    //  Some of my cells have a jqGrid cell with a formatter in them, making them hyperlinks.
                    //  We don't want to export the "<a href..> </a>" tags to our Excel file, just the cell's text.
                    cellValue = $(cellValue).text();
                }
                //  Make sure we are able to POST data containing apostrophes in it
                cellValue = cellValue.replace(/'/g, "&apos;");
                cellValue = cellValue.replace(/\//g, "&apos;");
                cellValue = cellValue.replace(/"/g, "&apos;");
                //cellValue = cellValue.replace(/\n/g, "&apos;");

                currentInstance.csvData += cellValue + "\t";
            }
        }
        currentInstance.csvData = currentInstance.removeLastChar(currentInstance.csvData) + "\r\n";

        if (currentInstance.isSubgrid) {
            currentInstance.appendChildGrid(i + 1);
            currentInstance.csvData = currentInstance.removeLastChar(currentInstance.csvData) + "\r\n";
        }
    }
}

JQGridtoCsv.prototype.appendChildGrid = function (parentRowId) {

    if (this.autoExpandChildGrid) {
        this.expandAndAppendChildGrid(parentRowId);
    }
    //else {
    //    this.appendExpandedChildGrid(parentRowId);
    //}
}

JQGridtoCsv.prototype.expandAndAppendChildGrid = function (parentRowId) {
    var currentInstance = this;
    var childDesign = $.extend({}, currentInstance.repeaterBuilder.design);
    var dataSourceName = currentInstance.repeaterBuilder.design.ChildDataSources[0].DataSourceName;
    var childGridData = currentInstance.repeaterBuilder.data[parentRowId - 1][dataSourceName];
    var colNames = [];
    var colModel = [];
    $.each(childDesign.Elements, function (index, element) {
        if (element.Label != null && element.IsPrimary == false && element.Visible) {
            colNames.push(element.Label);
            colModel.push(element.GeneratedName);
        }
    });
    var childGridColumn = {};
    childGridColumn[""] = "";
    for (key in colModel) {
        if (colModel[key] != undefined && colModel[key] != 'RowIDProperty')
            childGridColumn[colModel[key]] = colNames[key];
    }
    currentInstance.setChildGridRowToCSV(childGridColumn, childGridData);
}

JQGridtoCsv.prototype.appendExpandedChildGrid = function (parentRowId) {
    var currentInstance = this;
    var childId = currentInstance.gridTableJQ + '_' + parentRowId
    if ($(childId).closest('div')[0] != undefined) {
        var subgridId = $(childId).closest('div')[0].firstChild.id.replace(/gbox_/g, '');

        var subGridJQ = '#' + subgridId;

        var childGridData = $(subGridJQ).jqGrid('getGridParam', 'data');

        var colNames = $(subGridJQ).jqGrid('getGridParam', 'colNames');
        var colModel = $(subGridJQ).jqGrid('getGridParam', 'colModel');

        var childGridColumn = {};
        childGridColumn["dummyColumn"] = "dummyColumn";
        for (key in colModel) {
            if (colModel[key].index != undefined && colModel[key].index != 'RowIDProperty')
                childGridColumn[colModel[key].index] = colNames[key];
        }
        currentInstance.setChildGridRowToCSV(childGridColumn, childGridData);
    }
}

JQGridtoCsv.prototype.setChildGridRowToCSV = function (childGridColumn, childGridData) {
    var currentInstance = this;
    //  First, let's append the header row...
    for (k in childGridColumn) {
        currentInstance.csvData += childGridColumn[k] + "\t";
    }

    currentInstance.csvData = currentInstance.removeLastChar(currentInstance.csvData) + "\r\n";

    //  ..then each row of data to be exported.
    var cellValue = '';
    if (childGridData != null) {
        for (l = 0; l < childGridData.length; l++) {

            var data = childGridData[l];

            for (j in childGridColumn) {

                // Fetch one jqGrid cell's data, but make sure it's a string
                cellValue = '' + data[j];

                if (cellValue == null || cellValue == "undefined")
                    currentInstance.csvData += "\t";
                else {
                    if (cellValue.indexOf("a href") > -1) {
                        //  Some of my cells have a jqGrid cell with a formatter in them, making them hyperlinks.
                        //  We don't want to export the "<a href..> </a>" tags to our Excel file, just the cell's text.
                        cellValue = $(cellValue).text();
                    }
                    //  Make sure we are able to POST data containing apostrophes in it
                    cellValue = cellValue.replace(/'/g, "&apos;");

                    currentInstance.csvData += cellValue + "\t";
                }
            }
            currentInstance.csvData = currentInstance.removeLastChar(currentInstance.csvData) + "\r\n";
        }
    }
}

JQGridtoCsv.prototype.buildExportOptions = function () {

    this.init();
    if (this.isGroupHeader)
        this.setGroupHeaderToCSV();
    this.setColumnsHeaderToCSV();
    this.setRowDatatoCSV();
}

JQGridtoCsv.prototype.expandChildGrids = function () {
    var currentInstance = this;
    $body = $(currentInstance.gridTableJQ).closest(".ui-jqgrid-view").find(">.ui-jqgrid-bdiv>div>.ui-jqgrid-btable>tbody");
    $body.find(">tr.jqgrow>td.sgcollapsed").click();
}

JQGridtoCsv.prototype.collapseChildGrids = function () {
    var currentInstance = this;
    $body = $(currentInstance.gridTableJQ).closest(".ui-jqgrid-view").find(">.ui-jqgrid-bdiv>div>.ui-jqgrid-btable>tbody");
    $body.find(">tr.jqgrow>td.sgexpanded").click();
}

JQGridtoCsv.prototype.removeLastChar = function (str) {
    //  Remove the last character from a string
    return str.substring(0, str.length - 1);
}





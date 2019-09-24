var activitylogger = function () {
    var isLoggingActive = true;
    var isCurrentActivity = true;
    this.formInstanceId = null;
    this.formName = null;
    this.activityLogFormInstanceData = new Object();
    var elementIDs = {
        activityLog: "activitylog",
        activityLogJQ: "#activityLog",
        activityGridLog: "activityLogGrid",
        activityLogGridJQ: "#activityLogGrid",
        btnActivityLog: "#btnActivityLog",
        headerActivityLog: "#headerActivityLog",
    }

    function activateLogging(activate) {
        isLoggingActive = activate;
    }

    function init() {
        if (isLoggingActive) {
            // sent false to keep the grid empty initially as URL will not be sent from the grid.
            loadActivityGrid(false);
        }
    }
    function getFormInstanceId(currentFormInstanceId) {
        this.formInstanceId = currentFormInstanceId;
    }
    function getFormName(formName) {
        this.formName = formName;
    }

    function loadActivityLogGrid() {
        $(elementIDs.btnActivityLog).text("View Activity History");
        $(elementIDs.headerActivityLog).text("Current Activity Log");
        $(elementIDs.headerActivityLog).append(' - <b>' + this.formName + '</b>');
        var loadDataGrid = undefined;
        if (this.activityLogFormInstanceData != undefined) {
            if (this.activityLogFormInstanceData[formInstanceId]) {
                loadDataGrid = new Array();
                loadDataGrid = this.activityLogFormInstanceData[formInstanceId].slice();
                loadDataGrid.reverse();
            }
        }
        $(elementIDs.activityLogGridJQ).jqGrid('clearGridData');
        $(elementIDs.activityLogGridJQ).parent().append("<div id='p" + elementIDs.activityLogGridJQ + "'></div>");
        if (loadDataGrid != undefined) {
            var log = {};
            log.page = 1;
            log.rows = new Array();
            if (!$.isEmptyObject(activityLogFormInstanceData[parseInt(formInstanceId)])) {
                var activityLogdata = activityLogFormInstanceData[parseInt(formInstanceId)].filter(function (ct) {
                    return ct.IsNewRecord == true;
                });
                if (activityLogdata != undefined) {
                    activityLogdata = activityLogdata.reverse();
                    var rowNum = $(elementIDs.activityLogGridJQ).getGridParam('rowNum');
                    log.rows = activityLogdata.slice((log.page * rowNum) - rowNum, log.page * rowNum);
                    log.records = activityLogdata.length;
                    var totalPages = Math.ceil((log.records / rowNum));
                    log.total = totalPages;
                }
                var gridRows = $(elementIDs.activityLogGridJQ)[0];
                gridRows.addJSONData(log);
            }
        }
        isCurrentActivity = true;
    }
    function loadActivityGrid(viewActivityLogFlag) {
        var currentInstance = this;
        var loadDataGrid;
        if (this.activityLogFormInstanceData != undefined) {
            if (!this.activityLogFormInstanceData[formInstanceId]) {
                loadDataGrid = this.activityLogFormInstanceData[formInstanceId];
            }
        }
        $(elementIDs.activityLogGridJQ).jqGrid('GridUnload');
        var pagerElement = "#p_" + elementIDs.activityGridLog;
        var url = "";
        var dataType;
        if (viewActivityLogFlag) {
            url = '/FormInstance/GetActivityLogData?formInstnaceId={formInstnaceId}';
            url = url.replace(/{formInstnaceId}/g, this.formInstanceId);
            dataType = "json";
            isCurrentActivity = false;
        }
        else {
            dataType = "local";
            isCurrentActivity = true;
            sortable: true;
        }
        $(elementIDs.activityLogGridJQ).parent().append("<div id='p_" + elementIDs.activityGridLog + "'></div>");
        $(elementIDs.activityLogGridJQ).jqGrid({
            url: url,
            datatype: dataType,
            rowList: [10, 20, 30],
            data: loadDataGrid,
            colNames: ['ID', 'FormInstanceId', 'Element Path', 'Field', 'Row', 'Description', 'Version', 'Updated By', 'Updated Last', 'Is New Record'],
            colModel: [{ name: 'ID', index: 'ID', key: true, hidden: true },
                       { name: 'FormInstanceId', index: 'FormInstanceId', key: true, hidden: true },
                       { name: 'ElementPath', index: 'ElementPath', width: '300' },
                       { name: 'Field', index: 'Field', width: '100' },
                       { name: 'RowNum', index: 'RowNum', width: '80', align: 'left' },
                       { name: 'Description', index: 'Description', width: '400', formatter: descriptionColumnFormatter },
                       { name: 'FolderVersionName', index: 'FolderVersionName', width: '100', align: 'left' },
                       { name: 'UpdatedBy', index: 'UpdatedBy', width: '200', formatter: updatedByColumnFormatter },
                       { name: 'UpdatedLast', index: 'UpdatedLast', width: '200', formatter: dateTimeFormatter, formatoptions: JQGridSettings.DateTimeFormatterOptions, align: 'center' },
                       { name: 'IsNewRecord', index: 'IsNewRecord', key: true, hidden: true }],

            pager: pagerElement,
            altRows: true,
            cache: false,
            altclass: 'alternate',
            height: '160',
            hidegrid: false,
            rowNum: 50,
            autowidth: true,
            shrinkToFit: false,
            sortname: 'UpdatedLast',
            viewrecords: true,
            sortorder: "desc",
            caption: '<div id="activityLogDiv"><button id ="btnActivityLog" type="button" class="btn pull-right" style="padding:1px 5px;">View Activity History</button></div> <span id="headerActivityLog" class="ui-jqgrid-title">Current Activity Log</span>',
            gridComplete: function () {
                $(elementIDs.activityLogGridJQ).jqGrid('setGridWidth', $(window).width() - (($(window).width() * 4) / 100), true);
            },
            loadComplete: function (log) {
                if (isCurrentActivity) {
                    if (!$.isEmptyObject(activityLogFormInstanceData[parseInt(formInstanceId)])) {
                        var activityLogdata = activityLogFormInstanceData[parseInt(formInstanceId)].filter(function (ct) {
                            return ct.IsNewRecord == true;
                        });
                        if (activityLogdata != undefined) {
                            activityLogdata = activityLogdata.reverse();
                            var rowNum = $(elementIDs.activityLogGridJQ).getGridParam('rowNum');
                            log.rows = activityLogdata.slice((log.page * rowNum) - rowNum, log.page * rowNum);
                            log.records = activityLogdata.length;
                            var totalPages = Math.ceil((log.records / rowNum));
                            log.total = totalPages;
                        }
                        var gridRows = $(elementIDs.activityLogGridJQ)[0];
                        gridRows.addJSONData(log);
                    }
                }
            },
        });
        $(elementIDs.activityLogGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
        $(elementIDs.activityLogGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, refresh: false, search: false }, {}, {}, {});
        $(elementIDs.btnActivityLog).bind('click', function () {
            if ($(elementIDs.btnActivityLog).text() == "View Current Activity") {
                isCurrentActivity = true;
                loadActivityGrid(false);
                $(elementIDs.btnActivityLog).text("View Activity History");
                $(elementIDs.headerActivityLog).text("Current Activity Log");
                $(elementIDs.headerActivityLog).append(' - <b>' + currentInstance.formName + '</b>');
            }
            else {
                getActivityLogData();
                isCurrentActivity = false;
                $(elementIDs.btnActivityLog).text("View Current Activity");
                $(elementIDs.headerActivityLog).text("History Activity Log");
                $(elementIDs.headerActivityLog).append(' - <b>' + currentInstance.formName + '</b>');
            }
        });
        //To make pager at center position.
        $(pagerElement + "_left").css("width", "");
        //add button in footer of grid that pops up the add form design dialog
        //$(elementIDs.activityLogGridJQ).jqGrid('navButtonAdd', pagerElement,
        //{
        //    caption: '', buttonicon: 'ui-icon-trash',
        //    title: 'Clear Log',
        //    onClickButton: function () {
        //        clearLog();
        //    }
        //});

        $(elementIDs.activityLogGridJQ).jqGrid('navButtonAdd', pagerElement,
       {
           caption: '', buttonicon: 'ui-icon-arrowstop-1-s', title: 'Download ActivityLog To Excel', id: 'btnActivityLogExportToExcel',

           onClickButton: function () {
               exportActivityLogToExcel(false);
           }
       });

    }

    function dateTimeFormatter(cellvalue, options, rowObject) {
        if (rowObject.IsNewRecord == true) {
            return "<b>" + $.fn.fmatter.call(this, "date", cellvalue, options, rowObject); + "</b>";
        }
        else {
            return $.fn.fmatter.call(this, "date", cellvalue, options, rowObject);
        }
    }

    function updatedByColumnFormatter(cellvalue, options, rowObject) {
        if (rowObject.IsNewRecord == true) {
            return cellvalue;
        }
        else {
            return cellvalue.replace(/<b>/g, " ").replace(/<\/b>/g, " ");
        }
    }

    function descriptionColumnFormatter(cellvalue, options, rowObject) {
        if (rowObject.IsNewRecord == true) {
            return cellvalue;
        }
        else {
            //return cellvalue.replace(/<b>/g, " ").replace(/<\/b>/g, " ");
            return cellvalue;
        }
    }



    function logActivity(propertyName, previousValue, changedValue, fullName, currentUserName, lastUpdatedDate, currentFormInstanceId) {
        propertyName = propertyName.replace(/([a-z])([A-Z])/g, '$1 $2')
                                    .replace(/\b([A-Z]+)([A-Z])([a-z])/, '$1 $2$3')
                                    .replace(/^./, function (str) { return str.toUpperCase(); });

        var msg = "Value of <b>" + propertyName + "</b> is changed from <b>" + (previousValue === "" ? "null" : previousValue) + "</b> to <b>" + changedValue + "</b>.";
        var upadatedByMsg = "<b>" + currentUserName + "</b>";

        var rowCount = $(elementIDs.activityLogGridJQ).getGridParam("reccount");
        var row = {
            ID: (rowCount + 1),
            FormInstanceId: currentFormInstanceId,
            ElementPath: fullName,
            Field: propertyName,
            RowNum: '', Description: msg,
            FolderVersionName: this.folderData.versionNumber,
            UpdatedBy: upadatedByMsg,
            UpdatedLast: lastUpdatedDate,
            IsNewRecord: true
        }
        if (!this.activityLogFormInstanceData[currentFormInstanceId]) {
            this.activityLogFormInstanceData[currentFormInstanceId] = new Object(new Array());
            this.activityLogFormInstanceData[currentFormInstanceId].push(row);
        } else {

            this.activityLogFormInstanceData[currentFormInstanceId].push(row);
        }
        var allRecords = $(elementIDs.activityLogGridJQ).getGridParam('records');
        var rowNum = $(elementIDs.activityLogGridJQ).getGridParam('rowNum');
        var totalPages = parseInt((allRecords / rowNum) + 1);

        var currentPage = $(elementIDs.activityLogGridJQ).getGridParam('page');
        var p;
        for (p = 1; p <= totalPages; p++) {
            if (p == totalPages) {
                $(elementIDs.activityLogGridJQ).jqGrid('addRowData', (rowCount + 1), row, "first");
                $(elementIDs.activityLogGridJQ).trigger("reloadGrid", [{ page: currentPage }]);
            }
        }
        isCurrentActivity = true;
        $(elementIDs.btnActivityLog).text("View Activity History");
        $(elementIDs.headerActivityLog).text("Current Activity Log");
        $(elementIDs.headerActivityLog).append(' - <b>' + this.formName + '</b>');
    }

    function logRepeaterActivity(fullName, field, rowNumber, operation, currentUserName, lastUpdatedDate, repeaterName, currentFormInstanceId, oldValue, newValue, buFilterCriteria, repeaterDesign, repeaterID, keyValue, customMsg) {
        //if (rowNumber) {
        if (oldValue != undefined) {
            if (!$.isArray(oldValue)) {
                if (oldValue.toString().trim() === '') {
                    oldValue = "''";
                }
            }
        }
        if (newValue != undefined) {
            if (!$.isArray(newValue)) {
                if (newValue.toString().trim() === '') {
                    newValue = "''";
                }
            }
        }

        var operationVerb = "";
        switch (operation) {
            case 0:
                operationVerb = " Added";
                break;
            case 1:
                operationVerb = " Changed";
                break;
            case 2:
                operationVerb = " Deleted";
                break;
            case 3:
                operationVerb = " Copied";
                break;
            default:

        }
        var buSearchParam = null;
        var cnt = 0;
        var msg = null;
        if (buFilterCriteria != undefined) {
            $.each(buFilterCriteria, function (ind, prop) {
                var inlColName = null;
                if (prop.field.substring(0, 4) == 'INL_') {
                    var propArr = prop.field.split('_');
                    if (propArr.length == 4) {
                        inlColName = propArr[3];
                    }
                }
                var filterField = (inlColName == null ? prop.field : inlColName);
                if (cnt == 0)
                    buSearchParam = "Bulk Update Filter Criteria[" + filterField + ": " + prop.data + "]";
                else
                    buSearchParam = buSearchParam + "[" + filterField + ": " + prop.data + "]";
                cnt++;
            });
        }
        var upadatedByMsg = "<b>" + currentUserName + "</b>";
        var rowCount = $(elementIDs.activityLogGridJQ).getGridParam("reccount");

        //var keyValue = "";
        // If operation is delete then we get no data of selected row as row is delete before activitylog so we use oldvalue to hold repeater key data before row delete
        if (operation == 2 && oldValue != undefined) {
            keyValue = oldValue;
        }
        //else {
        //    var selectedRowId = $("#" + repeaterID).jqGrid('getGridParam', 'selrow');
        //    $.each(repeaterDesign.Elements, function (index, element) {
        //        if (element.IsKey == true) {
        //            var value = $("#" + repeaterID).jqGrid('getCell', selectedRowId, element.GeneratedName);
        //            keyValue += value + " ";
        //        }
        //    });
        //}
        if (customMsg == undefined) {
            if (keyValue !== "") {
                if (cnt == 0) {
                    if (operationVerb == " Deleted" || operationVerb == " Added")
                        msg = "<b>" + repeaterName + "</b> was" + operationVerb;
                    else
                        msg = "<b>" + repeaterName + "</b> was" + operationVerb + " from " + oldValue + " to " + newValue;
                }
                else
                    msg = buSearchParam + " <b>" + repeaterName + "</b> was" + operationVerb + " from " + oldValue + " to " + newValue;
            }
            else {
                if (cnt == 0) {
                    if (operationVerb == " Deleted" || operationVerb == " Added")
                        msg = "Row " + (parseInt(rowNumber) + 1) + " in <b>" + repeaterName + "</b> was" + operationVerb;
                    else
                        msg = "Row " + (parseInt(rowNumber) + 1) + " in <b>" + repeaterName + "</b> was" + operationVerb + " from " + oldValue + " to " + newValue;
                }
                else
                    msg = buSearchParam + " Row " + (parseInt(rowNumber) + 1) + " in <b>" + repeaterName + "</b> was" + operationVerb + " from " + oldValue + " to " + newValue;
            }
        }
        else {
            msg = customMsg;
        }

        var rowValue = "";

        if (keyValue != "")
            rowValue = keyValue;
        else {
            if (rowNumber != undefined) {
                rowValue = (parseInt(rowNumber) + 1);
            }
            else {
                rowValue = "";
            }
        }

        var row = {
            ID: (rowCount + 1),
            FormInstanceId: currentFormInstanceId,
            ElementPath: fullName,
            Field: field,
            RowNum: rowValue,
            FolderVersionName: this.folderData.versionNumber,
            Description: msg,
            UpdatedBy: upadatedByMsg,
            UpdatedLast: lastUpdatedDate,
            IsNewRecord: true
        }

        if (!this.activityLogFormInstanceData[currentFormInstanceId]) {
            this.activityLogFormInstanceData[currentFormInstanceId] = new Object(new Array());
            this.activityLogFormInstanceData[currentFormInstanceId].push(row);
        } else {
            this.activityLogFormInstanceData[currentFormInstanceId].push(row);
        }
        if (buFilterCriteria == undefined) {
            var allRecords = $(elementIDs.activityLogGridJQ).getGridParam('records');
            var rowNum = $(elementIDs.activityLogGridJQ).getGridParam('rowNum');
            var totalPages = parseInt((allRecords / rowNum) + 1);

            var currentPage = $(elementIDs.activityLogGridJQ).getGridParam('page');
            var p;
            for (p = 1; p <= totalPages; p++) {
                if (p == totalPages) {
                    $(elementIDs.activityLogGridJQ).jqGrid('addRowData', (rowCount + 1), row, "first");
                    $(elementIDs.activityLogGridJQ).trigger("reloadGrid", [{ page: currentPage }]);
                }
            }
            isCurrentActivity = true;
            $(elementIDs.btnActivityLog).text("View Activity History");
            $(elementIDs.headerActivityLog).text("Current Activity Log");
            $(elementIDs.headerActivityLog).append(' - <b>' + this.formName + '</b>');
        }
    }


    function logRepeaterActivityPQ(fullName, field, rowNumber, operation, currentUserName, lastUpdatedDate, repeaterName, currentFormInstanceId, oldValue, newValue, buFilterCriteria, repeaterDesign, repeaterID, keyValue, customMsg) {
        //if (rowNumber) {

        var operationVerb = "";
        switch (operation) {
            case 0:
                operationVerb = " Added";
                break;
            case 1:
                operationVerb = " Changed";
                break;
            case 2:
                operationVerb = " Deleted";
                break;
            case 3:
                operationVerb = " Copied";
                break;
            default:

        }
        var buSearchParam = null;
        var cnt = 0;
        var msg = null;
        if (buFilterCriteria != undefined) {
            $.each(buFilterCriteria[0], function (ind, prop) {
                var inlColName = null;
                if (prop.dataIndx.toString().substring(0, 4) == 'INL_') {
                    var propArr = prop.dataIndx.split('_');
                    if (propArr.length == 4) {
                        inlColName = propArr[3];
                    }
                }
                var filterField = (inlColName == null ? prop.dataIndx : inlColName);
                if (cnt == 0)
                    buSearchParam = "Bulk Update Filter Criteria[" + filterField + ": " + prop.value + "]";
                else
                    buSearchParam = buSearchParam + "[" + filterField + ": " + prop.value + "]";
                cnt++;
            });
        }
        var upadatedByMsg = "<b>" + currentUserName + "</b>";
        var rowCount = $(elementIDs.activityLogGridJQ).getGridParam("reccount");

        //var keyValue = "";
        // If operation is delete then we get no data of selected row as row is delete before activitylog so we use oldvalue to hold repeater key data before row delete
        if (operation == 2 && oldValue != undefined) {
            keyValue = oldValue;
        }
        //else {
        //    var selectedRowId = $("#" + repeaterID).jqGrid('getGridParam', 'selrow');
        //    $.each(repeaterDesign.Elements, function (index, element) {
        //        if (element.IsKey == true) {
        //            var value = $("#" + repeaterID).jqGrid('getCell', selectedRowId, element.GeneratedName);
        //            keyValue += value + " ";
        //        }
        //    });
        //}
        if (oldValue != undefined) {
            if (!$.isArray(oldValue)) {
                if (oldValue.toString().trim() === '') {
                    oldValue = "''";
                }
            }
        }
        if (newValue != undefined) {
            if (!$.isArray(newValue)) {
                if (newValue.toString().trim() === '') {
                    newValue = "''";
                }
            }
        }
        if (customMsg == undefined) {
            if (keyValue !== "") {
                if (cnt == 0) {
                    if (operationVerb == " Deleted" || operationVerb == " Added")
                        msg = "<b>" + repeaterName + "</b> was" + operationVerb;
                    else
                        msg = "<b>" + repeaterName + "</b> was" + operationVerb + " from " + oldValue + " to " + newValue;
                }
                else
                    msg = buSearchParam + " <b>" + repeaterName + "</b> was" + operationVerb + " from " + oldValue + " to " + newValue;
            }
            else {
                if (cnt == 0) {
                    if (operationVerb == " Deleted" || operationVerb == " Added")
                        msg = "Row " + (parseInt(rowNumber) + 1) + " in <b>" + repeaterName + "</b> was" + operationVerb;
                    else
                        msg = "Row " + (parseInt(rowNumber) + 1) + " in <b>" + repeaterName + "</b> was" + operationVerb + " from " + oldValue + " to " + newValue;
                }
                else
                    msg = buSearchParam + " Row " + (parseInt(rowNumber) + 1) + " in <b>" + repeaterName + "</b> was" + operationVerb + " from " + oldValue + " to " + newValue;
            }
        }
        else {
            msg = customMsg;
        }

        var rowValue = "";

        if (keyValue != "")
            rowValue = keyValue;
        else {
            if (rowNumber != undefined) {
                rowValue = (parseInt(rowNumber) + 1);
            }
            else {
                rowValue = "";
            }
        }

        var row = {
            ID: (rowCount + 1),
            FormInstanceId: currentFormInstanceId,
            ElementPath: fullName,
            Field: field,
            RowNum: rowValue,
            FolderVersionName: this.folderData.versionNumber,
            Description: msg,
            UpdatedBy: upadatedByMsg,
            UpdatedLast: lastUpdatedDate,
            IsNewRecord: true
        }

        if (!this.activityLogFormInstanceData[currentFormInstanceId]) {
            this.activityLogFormInstanceData[currentFormInstanceId] = new Object(new Array());
            this.activityLogFormInstanceData[currentFormInstanceId].push(row);
        } else {
            this.activityLogFormInstanceData[currentFormInstanceId].push(row);
        }
        if (buFilterCriteria == undefined) {
            var allRecords = $(elementIDs.activityLogGridJQ).getGridParam('records');
            var rowNum = $(elementIDs.activityLogGridJQ).getGridParam('rowNum');
            var totalPages = parseInt((allRecords / rowNum) + 1);

            var currentPage = $(elementIDs.activityLogGridJQ).getGridParam('page');
            var p;
            for (p = 1; p <= totalPages; p++) {
                if (p == totalPages) {
                    $(elementIDs.activityLogGridJQ).jqGrid('addRowData', (rowCount + 1), row, "first");
                    $(elementIDs.activityLogGridJQ).trigger("reloadGrid", [{ page: currentPage }]);
                }
            }
            isCurrentActivity = true;
            $(elementIDs.btnActivityLog).text("View Activity History");
            $(elementIDs.headerActivityLog).text("Current Activity Log");
            $(elementIDs.headerActivityLog).append(' - <b>' + this.formName + '</b>');
        }
    }

    function getLabelText(targetID) {
        var text = $("label[for='" + targetID + "']").text();
        return text.replace(":", "").replace("*", "");
    }

    function clearLog() {
        if (!$.isEmptyObject(activityLogFormInstanceData[parseInt(formInstanceId)])) {
            activityLogFormInstanceData[this.formInstanceId] = new Object(new Array());
        }
    }

    function haveChangesToForm(formName) {
        var data = $(elementIDs.activityLogGridJQ).jqGrid('getGridParam', 'data');
        var hasFormData = false;

        if (data.length > 0) {
            for (var i = 0; i < data.length; i++) {
                if (data[i].Form === formName) {
                    hasFormData = true;
                    break;
                }
            }
        }
        return hasFormData;
    }

    function getActivityLogFormInstanceData(formId) {
        var activityLogData = undefined;
        if (activityLogFormInstanceData != undefined) {
            if (activityLogFormInstanceData[formId]) {
                activityLogData = this.activityLogFormInstanceData[formId];
            }
        }
        return activityLogData;
    }

    //export to excel
    function exportActivityLogToExcel(autoExpandChild) {
        FolderLockAction.ISREPEATERACTION = 1;
        var currentInstance = this;

        // currentInstance.saveRow();
        var jqGridtoCsv = new JQGridtoCsv($(elementIDs.activityLogGridJQ), autoExpandChild, currentInstance);
        jqGridtoCsv.buildExportOptions();

        var forminstancelisturl = '/FormInstance/ExportToExcel';

        var csvData = jqGridtoCsv.csvData.replace(/(<([^>]+)>)|\n|>/ig, "");
        csvData = csvData.replace(/=/g, "->");
        var stringData = "csv=" + csvData;
        stringData += "<&isGroupHeader=" + jqGridtoCsv.isGroupHeader;
        stringData += "<&noOfColInGroup=" + jqGridtoCsv.noofGroupedColumns;
        stringData += "<&isChildGrid=" + false;
        stringData += "<&repeaterName=" + "Activity Log";
        stringData += "<&formName=" + jqGridtoCsv.repeaterBuilder.formName;
        stringData += "<&folderVersionId=" + currentInstance.folderData.folderVersionId;
        stringData += "<&folderId=" + currentInstance.folderData.folderId;
        stringData += "<&tenantId=" + 1;
      
        
        $.download(forminstancelisturl, stringData, 'post');
    }
    function getActivityLogData() {
        var currentInstance = this;
        var viewActivityLogFlag = true;
        loadActivityGrid(viewActivityLogFlag);
    }

    return {
        init: function () {
            init();
        },
        getFormInstanceId: function (formInstanceId) {
            getFormInstanceId(formInstanceId);
        },
        getFormName: function (fromName) {
            getFormName(fromName);
        },

        log: function (propertyName, previousValue, changedValue, fullName, currentUserName, lastUpdatedDate, currentFormInstanceId) {
            logActivity(propertyName, previousValue, changedValue, fullName, currentUserName, lastUpdatedDate, currentFormInstanceId);
        },
        logRepeater: function (fullName, field, rowNumber, operation, currentUserName, lastUpdatedBy, repeaterName, currentFormInstanceId, oldValue, newValue, buFilterCriteria, repeaterDesign, repeaterID, keyValue, customMsg) {
            logRepeaterActivity(fullName, field, rowNumber, operation, currentUserName, lastUpdatedBy, repeaterName, currentFormInstanceId, oldValue, newValue, buFilterCriteria, repeaterDesign, repeaterID, keyValue, customMsg);
        },

        logRepeaterPQ: function (fullName, field, rowNumber, operation, currentUserName, lastUpdatedBy, repeaterName, currentFormInstanceId, oldValue, newValue, buFilterCriteria, repeaterDesign, repeaterID, keyValue, customMsg) {
            logRepeaterActivityPQ(fullName, field, rowNumber, operation, currentUserName, lastUpdatedBy, repeaterName, currentFormInstanceId, oldValue, newValue, buFilterCriteria, repeaterDesign, repeaterID, keyValue, customMsg);
        },

        clear: function () {
            clearLog();
        },
        activate: function (active) {
            activateLogging(active);
        },
        haveChangesToForm: function (formName) {
            return haveChangesToForm(formName);
        },

        loadActivityLogGrid: function () {
            return loadActivityLogGrid();
        },
        getActivityLogFormInstanceData: function (formInstanceId) {
            return getActivityLogFormInstanceData(formInstanceId);
        }
    }
}();


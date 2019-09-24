var viewQueuedPBPExports = function () {
    var URLs = {
        getQueuedPBPExportList: "/PBPExport/GetQueuedPBPExportList",
        exportToExcel: '/PBPExport/ExportPBPExport',
        downloadFile: '/PBPExport/PBPExportDownload?PBPExportQueueID=',
        GetCascadeMLProcessingOrQueued: "/FolderVersion/GetCascadeMLProcessingOrQueued",
        GetQueuedOrProcessingPBPImport: "/PBPImport/GetQueuedOrProcessingPBPImport",
        GetQueuedOrProcessingPBPExport: "/PBPExport/GetQueuedOrProcessingPBPExport"
    };

    var elementIDs = {
        queuedPBPExportGrid: "queuedPBPExportGrid",
        queuedPBPExportGridJQ: "#queuedPBPExportGrid",
        btnExportPBP: "#btnExportPBP",
    };

    var timer = 60;
    this.refreshInterval;

    function init() {
        $(document).ready(function () {
            loadQueuedPBPExportGrid();
        });
    }
    init();


    function loadQueuedPBPExportGrid() {
        var colArray = null;
        colArray = ['Export Id', 'Export Name', 'Description', 'Database Name', 'Plan Year', 'Exported Date & Time', 'Exported By', 'Status', 'Export Status', 'Download PBP File', 'ErrorMessage'];

        var colModel = [];
        colModel.push({ key: true, hidden: false, name: 'PBPExportQueueID', index: 'PBPExportQueueID', align: 'left', editable: true });
        colModel.push({ key: false, name: 'FileName', index: 'FileName', editable: true, edittype: 'select', align: 'left', width: '150' });
        colModel.push({ key: false, name: 'Description', index: 'Description', editable: false, width: '150' });
        colModel.push({ key: false, name: 'PBPDatabase', index: 'PBPDatabase', editable: true, align: 'left', width: '150' });
        colModel.push({ key: false, name: 'PlanYear', index: 'PlanYear', editable: true, align: 'left', width: '100' });
        colModel.push({ key: false, name: 'ExportedDate', index: 'ExportedDate', formatter: 'date', formatoptions: JQGridSettings.DateTimeFormatterOptions, editable: true, edittype: 'select', align: 'left', width: '200' });
        colModel.push({ key: false, name: 'ExportedBy', index: 'ExportedBy', editable: true, align: 'left', width: '150' });
        colModel.push({ key: false, name: 'Status', index: 'Status', hidden: true });
        colModel.push({ key: false, name: 'StatusText', index: 'StatusText', editable: false, align: 'left', formatter: processStatusFormmater });
        colModel.push({ key: false, name: 'PBPExportQueueID', index: 'PBPExportQueueID', search: false, editable: false, width: '120', formatter: downloadFileFormmater });
        colModel.push({ key: false, name: 'ErrorMessage', index: 'ErrorMessage', hidden: true });

        $(elementIDs.queuedPBPExportGridJQ).jqGrid('GridUnload');
        $(elementIDs.queuedPBPExportGridJQ).parent().append("<div id='p" + elementIDs.queuedPBPExportGrid + "'></div>");

        $(elementIDs.queuedPBPExportGridJQ).jqGrid({
            url: URLs.getQueuedPBPExportList,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            height: '350',
            rowNum: 20,
            rowList: [20, 40, 60],
            headertitles: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortname: 'ExportedDate',
            sortorder: 'desc',
            pager: '#p' + elementIDs.queuedPBPExportGrid
        });

        var pagerElement = '#p' + elementIDs.queuedPBPExportGrid;
        //$('#p' + elementIDs.queuedPBPExportGrid).find('input').css('height', '20px');
        //remove default buttons
        $(elementIDs.queuedPBPExportGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.queuedPBPExportGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
        $(elementIDs.queuedPBPExportGridJQ).jqGrid('navButtonAdd', pagerElement,
         {
             caption: '', buttonicon: 'ui-icon-arrowstop-1-s', title: 'Export to Excel', id: 'btnExportPBPExportAudit',
             onClickButton: function () {
                 var currentInstance = this;
                 var jqGridtoCsv = new JQGridtoCsv(elementIDs.queuedPBPExportGridJQ, false, currentInstance);
                 jqGridtoCsv.buildExportOptions();
                 var stringData = "csv=" + jqGridtoCsv.csvData;
                 stringData += "<&isGroupHeader=" + jqGridtoCsv.isGroupHeader;
                 stringData += "<&noOfColInGroup=" + jqGridtoCsv.noofGroupedColumns;
                 stringData += "<&repeaterName=" + elementIDs.userListGrid;
                 $.download(URLs.exportToExcel, stringData, 'post');
             }
         });

        refreshInterval = setInterval(AutoRefreshPBPExportQueued, 1000);
    }

    function AutoRefreshPBPExportQueued() {
        timer--;
        if (timer == 0) {
            $(elementIDs.queuedPBPExportGridJQ).trigger("reloadGrid");
            timer = 60;
        }
        $("#spnTimer").text(timer + " seconds.").css("font-weight", "Bold");
    }

    function downloadFileFormmater(cellvalue, options, rowObject) {
        if (rowObject.Status === 3) {//Errored
            if (rowObject.ErrorMessage != null) {
                var errorMsg = "onclick=javascript:errorLogDialog.show('";
                GLOBAL.applicationName.toLowerCase() !== 'ebenefitsync' ? errorMsg += rowObject.ErrorMessage + "  Please contact eMS support at emssupport@simplifyhealthcare.com".replace(/["']/g, "&quot"): 
                errorMsg += rowObject.ErrorMessage + "  Please contact eBS support at ebssupport@simplifyhealthcare.com".replace(/["']/g, "&quot");                
                errorMsg += "');";
                errorMsg = errorMsg.replaceAll(" ", "&nbsp;");
                cellvalue = "<a style='text-decoration: underline' href='javascript:void(0);' " + errorMsg + ">" + cellvalue + "</a>";
            }
        }
        else if (rowObject.Status == 4)
            cellvalue = "<a style='text-decoration: underline' href='" + URLs.downloadFile + rowObject.PBPExportQueueID + "' >" + cellvalue + "</a>";
        return cellvalue;
    }

    String.prototype.replaceAll = function (search, replacement) {
        var target = this;
        return target.split(search).join(replacement);
    };

    function processStatusFormmater(cellvalue, options, rowObject) {
        switch (cellvalue) {

            case 'Queued':
                // If status is Queued then display status text in back color 
                cellvalue = '<span style="">' + 'Queued' + '</span>';

                break;
            case 'In Progress':
                // If status is Queued then display status text in back color 
                cellvalue = '<span style="color:blue">' + 'In Progress' + '</span>';

                break;
            case 'Failed':
                // If status is Queued then display status text in back color 
                cellvalue = '<span style="color:red">' + 'Failed' + '</span>';

                break;
            case 'Complete':
                // If status is Queued then display status text in back color 
                cellvalue = '<span style="color:green">' + 'Complete' + '</span>';

                break;
            case 'Finalized':
                // If status is Queued then display status text in back color 
                cellvalue = '<span style="color:blue">' + 'Finalized' + '</span>';

                break;
            case 'Not Scheduled':
                // If status is Queued then display status text in back color 
                cellvalue = '<span style="color:blue">' + 'Not Scheduled' + '</span>';

                break;
            case 'Scheduled':
                // If status is Queued then display status text in back color 
                cellvalue = '<span style="color:blue">' + 'Scheduled' + '</span>';

                break;
            case 'In Review':
                // If status is Queued then display status text in back color 
                cellvalue = '<span style="color:#ffb833"><b><u>' + 'In Review' + '</u></b></span>';

                break;
            case 'Cancel':
                // If status is Queued then display status text in back color 
                cellvalue = '<span style="color:blue">' + 'Cancel' + '</span>';
                break;
        }
        return cellvalue;
    }

    function showError(msg) {
        messageDialog.show(JSON.stringify(msg));
    }


    $(elementIDs.btnExportPBP).on('click', function () {
        //check for ML Cascade and Import
        var urlML = URLs.GetCascadeMLProcessingOrQueued;
        var promiseML = ajaxWrapper.getJSON(urlML);
        var urlImport = URLs.GetQueuedOrProcessingPBPImport;
        var promiseImport = ajaxWrapper.getJSON(urlImport);
        $.when(promiseML, promiseImport).done(function (xhrML, xhrImport) {
            var ml = xhrML[0];
            var imp = xhrImport[0];
            if (ml == true) {
                messageDialog.show("A Master List Cascade is already queued or being Processed. Please try again later. Visit the Master List Cascade status screen to check for Status.");
            }
            else if (imp == true) {
                messageDialog.show("A PBP Import is already queued or being Processed. Please try again later. Visit the PBP Import Screen to check the status of the Import that is queued or being Processed.");
            }
            if (ml == false && imp == false) {
                var queueDialog = new showExportPBPQueueDialog();
                queueDialog.show();
            }
        });

    });
}

var showExportPBPQueueDialog = function () {
    var URLs = {
        queuePBPExport: "/PBPExport/QueuePBPExport?exportName={exportName}&description={description}&DBName={DBName}&pbpDatabase1Up={PBPDatabase1Up}",
        getPBPDatabaseNames: "/PBPExport/GetPBPDatabaseNames",
        getPBPDatabaseDetails: "/PBPExport/GetPBPDatabaseDetails?PBPDatabase1Up={PBPDatabase1Up}",
        checkExportFolderIsLocked: "/PBPExport/CheckExportFolderIsLocked"

    };

    var elementIDs = {
        exportPBPQueueDialog: "#exportPBPQueueDialog",
        exportPBPQueueTable: "#exportPBPQueueTable",
        description: "#description",
        exportName: "#exportName",
        btnPBPQueue: "#btnPBPQueue",
        folderId: '#folderId',
        folderVersionId: '#folderVersionId',
        folderVersionEffectiveDate: '#folderVersionEffectiveDate',
        PBPDatabaseNamesGrid: "PBPDatabaseNamesGrid",
        PBPDatabaseNamesGridJQ: "#PBPDatabaseNamesGrid",
        PBPDatabaseDetailsGrid: 'PBPDatabaseDetailsGrid',
        PBPDatabaseDetailsGridJQ: '#PBPDatabaseDetailsGrid',
        PBPDatabaseDetailsDialog: '#exportPBPDatabaseDetailsDialog',
        queuedPBPExportGridJQ: "#queuedPBPExportGrid",
        PBPDatabaseDetailsGrid: 'PBPDatabaseDetailsGrid',
    };

    function init() {
        $(elementIDs.exportPBPQueueDialog).dialog({
            autoOpen: false,
            height: 500,
            width: 650,
            modal: true
        });

        $(elementIDs.exportPBPQueueDialog + ' button').unbind().on('click', function () {
            var exportName = $(elementIDs.exportName).val();
            var description = $(elementIDs.description).val();
            var rowId = $(elementIDs.PBPDatabaseNamesGridJQ).jqGrid("getGridParam", "selrow");
            var selDatabaseRow = $(elementIDs.PBPDatabaseNamesGridJQ).jqGrid("getRowData", rowId);
            if (selDatabaseRow != undefined && selDatabaseRow != null) {
                var databaseName = selDatabaseRow["PBPDataBase"];
                var PBPDatabase1Up = selDatabaseRow["PBPDatabase1Up"];
            }
            if (databaseName != "" && databaseName != undefined) {
                if (exportName != "" && description != "") {
                    // Ajax Call To Check If Any Folder Is Locked for selected PBPDatabase1Up
                    var link = URLs.checkExportFolderIsLocked;
                    var input = {
                        pbpDatabase1Up: PBPDatabase1Up
                    }
                    var promise = ajaxWrapper.postJSON(link, input);
                    promise.done(function (result) {
                        if (result == undefined || result == null || result.length == 0) {
                            var url = URLs.queuePBPExport.replace(/\{exportName\}/g, exportName).replace(/\{description\}/g, description).replace(/\{DBName\}/g, databaseName).replace(/\{PBPDatabase1Up\}/g, PBPDatabase1Up);
                            var promise = ajaxWrapper.getJSON(url);
                            promise.done(function (result) {
                                if (result == true) {
                                    $(elementIDs.queuedPBPExportGridJQ).trigger('reloadGrid');
                                    messageDialog.show("PBP Export queued successfully");
                                    $(elementIDs.exportPBPQueueDialog).dialog("close");
                                }
                            });
                        }
                        else if (result != undefined && result != null && result.length > 0) {
                            var message = "Following folder(s) are in use <br/><br/>";
                            $.each(result, function (idx, value) {
                                message += "Folder : " + value.Folder.Name + "<br/> Used By : " + value.LockedUserName + (idx !== (result.length - 1) ? "<br/>" : "");
                            })
                            message += "<br/><br/> Running export may lead to data discrepancy.Do you want to continue export?"
                            yesNoConfirmDialog.showFormatted(message, function (e) {
                                yesNoConfirmDialog.hide();
                                if (e) {
                                    var url = URLs.queuePBPExport.replace(/\{exportName\}/g, exportName).replace(/\{description\}/g, description).replace(/\{DBName\}/g, databaseName).replace(/\{PBPDatabase1Up\}/g, PBPDatabase1Up);
                                    var promise = ajaxWrapper.getJSON(url);
                                    promise.done(function (result) {
                                        if (result == true) {
                                            $(elementIDs.queuedPBPExportGridJQ).trigger('reloadGrid');
                                            messageDialog.show("PBP Export queued successfully");
                                            $(elementIDs.exportPBPQueueDialog).dialog("close");
                                        }
                                    });
                                }
                            });
                        }
                    });
                }
                else
                    messageDialog.show("Please enter export name & description.");
            }
            else
                messageDialog.show("Please select database name.");
        });

        var selectedRowId = 0;
        var colArray = null;
        colArray = ['', 'Database Name', 'PBPDatabase1Up'];

        var colModel = [];
        colModel.push({ key: false, name: 'PBPDatabase1Up', index: 'PBPDatabase1Up', search: false, editable: false, formatter: DBDetailsFormmater, width: '8px', align: 'center !important' });
        colModel.push({ key: false, name: 'PBPDataBase', index: 'PBPDataBase', editable: true, sortable: true, align: 'left' });
        colModel.push({ key: false, name: 'PBPDatabase1Up', index: 'PBPDatabase1Up', hidden: true, editable: false });

        $(elementIDs.PBPDatabaseNamesGridJQ).jqGrid('GridUnload');
        $(elementIDs.PBPDatabaseNamesGridJQ).parent().append("<div id='p" + elementIDs.PBPDatabaseNamesGrid + "'></div>");

        $(elementIDs.PBPDatabaseNamesGridJQ).jqGrid({
            url: URLs.getPBPDatabaseNames,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'PBP Databases for Export',
            //height: '140',
            rowNum: 20,
            rowList: [20, 40, 60],
            headertitles: true,
            viewrecords: true,
            sortname: 'PBPDataBase',
            pager: '#p' + elementIDs.PBPDatabaseNamesGrid,
            gridComplete: function () {
                $(".DownloadAccessFile").on('click', function () {
                    var parameters = $(this).attr("data-Id").split('ö');
                    var PBPDataBase1Up = parameters[0];
                    var PBPDataBaseName = parameters[1];
                    showDatabaseDetails(PBPDataBase1Up, PBPDataBaseName);
                });
            }
        });

        var pagerElement = '#p' + elementIDs.PBPDatabaseNamesGrid;
        //$(pagerElement).find('input').css('height', '20px');
        $(elementIDs.PBPDatabaseNamesGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.PBPDatabaseNamesGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
    }

    function DBDetailsFormmater(cellvalue, options, rowObject) {
        cellvalue = "<a class='ASSIGNMENT' href='#'><span title='Details' id='spnDBDetails' class='DownloadAccessFile ui-icon ui-icon-extlink' data-Id='" + rowObject.PBPDatabase1Up + 'ö' + rowObject.PBPDataBase + "'></span></a>";
        return cellvalue;
    }

    function showDatabaseDetails(PBPDataBase1Up, PBPDataBaseName) {
        $(elementIDs.PBPDatabaseDetailsDialog).show();
        $(elementIDs.PBPDatabaseDetailsDialog).dialog({
            autoOpen: false,
            height: 450,
            width: 550,
            modal: true
        });

        $(elementIDs.PBPDatabaseDetailsDialog).dialog('option', 'title', 'Plans in- ' + PBPDataBaseName);
        $(elementIDs.PBPDatabaseDetailsDialog).dialog("open");

        var colArray = null;
        var colModel = [];

        colArray = ['Plan Name', 'Plan Number', 'Folder Version'];
        colModel.push({ name: 'PlanName', index: 'PlanName', align: 'left', editable: false, width: '250' });
        colModel.push({ name: 'PlanNumber', index: 'PlanNumber', search: false, editable: false, width: '150' });
        colModel.push({ name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: false, align: 'left', width: '100' });

        $(elementIDs.PBPDatabaseDetailsGridJQ).jqGrid('GridUnload');
        //adding the pager element
        $(elementIDs.PBPDatabaseDetailsGridJQ).parent().append("<div id='p" + elementIDs.PBPDatabaseDetailsGrid + "'></div>");
        $(elementIDs.PBPDatabaseDetailsGridJQ).jqGrid({
            url: URLs.getPBPDatabaseDetails.replace(/\{PBPDatabase1Up\}/g, PBPDataBase1Up),
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Database Details',
            height: '150',
            rowNum: 20,
            rowList: [20, 40, 60],
            headertitles: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            pager: '#p' + elementIDs.PBPDatabaseDetailsGrid,
            sortname: 'PlanNumber',
            sortorder: 'asc',
        });
        var pagerElement = '#p' + elementIDs.PBPDatabaseDetailsGrid;
        $(elementIDs.PBPDatabaseDetailsGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    }

    init();

    return {
        show: function () {
            $(elementIDs.exportPBPQueueDialog).dialog('option', 'title', "Queue PBP Export");
            $(elementIDs.exportPBPQueueDialog).dialog("open");
            $(elementIDs.description)[0].value = '';
            $(elementIDs.exportName)[0].value = '';
        }
    }
}

var errorLogDialog;
errorLogDialog = errorLogDialog || (function () {
    $(function () {
        $('#errorLogDialog').dialog({
            modal: true,
            autoOpen: false,
            draggable: true,
            resizable: true,
            height: 200,
            zIndex: 1005,
            closeOnEscape: false,
            title: 'Error Details',
            buttons: {
                Close: function () {
                    $(this).dialog("close");
                }
            }
        });
    });
    return {
        show: function (message) {
            $('#errorLogDialog').html(message);
            $('#errorLogDialog').dialog('open');
        },
        hide: function () {
            $('#errorLogDialog').dialog('close');
        },
    };
})();
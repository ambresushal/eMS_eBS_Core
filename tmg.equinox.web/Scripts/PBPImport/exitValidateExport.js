var exitValidateExport = function(){
    
    var elementIDs = {
        exportedExitValidateGrid: 'exportedExitValidateGrid',
        exportedExitValidateGridJQ: '#exportedExitValidateGrid',
        btnExportedExitValidateListListJQ: '#btnExportedExitValidateListList',
        btnExitValidateDownloadJQ: '#btnExitValidateDownload',
    };

    var URLs = {

        getExitValidateExportedList: '/ExitValidate/GetExitValidateExportedList',
        exitValidateExportDownload: '/ExitValidate/ExitValidateExportDownload',
        folderVersionList: '/FolderVersion/Index?tenantId={tenantId}&folderVersionId={folderVersionId}&folderId={folderId}&foldeViewMode={foldeViewMode}&mode={mode}',

    };
    function init() {
        $(document).ready(function () {
            loadExitValidateExportGrid();
        });
    }
    init();


    function loadExitValidateExportGrid() {
    
        var colArray = ['ExitValidateQueueID', 'FolderVersionID', 'FormInstanceID', 'FolderID', 'FormDesignVersionID', 'Folder Name', 'Product ID', 'Folder Version', 'Queued Date Time', 'Completed Date Time', 'Status', 'User Name'];
        var colModel = [];
        colModel.push({ name: 'ExitValidateQueueID', index: 'ExitValidateQueueID', hidden: true, editable: false, align: 'left', search: false });
        colModel.push({ name: 'FormInstanceID', index: 'FormInstanceID', hidden: true, editable: false, align: 'left', search: false });
        colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', hidden: true, editable: false, align: 'left', search: false });
        colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true, editable: false, align: 'left', search: false });
        colModel.push({ name: 'FormDesignVersionID', index: 'FormDesignVersionID', hidden: true, editable: false, align: 'left', search: false });
        colModel.push({ name: 'FolderName', index: 'FolderName', hidden: false, editable: false, align: 'left', search: true, searchoptions: { sopt: ["cn"] } });
        colModel.push({ name: 'ProductID', index: 'ProductID', hidden: false, editable: false, align: 'left', search: true, searchoptions: { sopt: ["cn"] } });
        colModel.push({ name: 'FolderVersion', index: 'FolderVersion', hidden: false, editable: false, align: 'left', search: true, searchoptions: { sopt: ["cn"] } });
        colModel.push({ name: 'QueuedDateTime', index: 'QueuedDateTime', editable: false, align: 'left', search: true, formatter: 'date', sorttype: "date", formatoptions: JQGridSettings.DateTimeFormatterOptions, searchoptions: { sopt: ["cn"] } });
        colModel.push({ name: 'CompletedDateTime', index: 'CompletedDateTime', hidden: false, editable: false, align: 'left', search: true, formatter: 'date', sorttype: "date", formatoptions: JQGridSettings.DateTimeFormatterOptions, searchoptions: { sopt: ["cn"] } });
        colModel.push({ name: 'Status', index: 'Status', hidden: false, editable: false, align: 'left', search: true, searchoptions: { sopt: ["cn"] }, formatter: statusFormatter });
        colModel.push({ name: 'User', index: 'User', hidden: false, editable: false, align: 'left', search: true, searchoptions: { sopt: ["cn"] } });

        //set column models
        $(elementIDs.exportedExitValidateGridJQ).jqGrid('GridUnload');
        var URL = URLs.getExitValidateExportedList;
        $(elementIDs.exportedExitValidateGridJQ).parent().append("<div id='p_" + elementIDs.exportedExitValidateGrid + "'></div>");
        $(elementIDs.exportedExitValidateGridJQ).jqGrid({
            //data: dataJson,
            rowList: [10, 20, 30],
            url: URL,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: "Exit Validate Export",
            pager: '#p_' + elementIDs.exportedExitValidateGrid,
            height: '300',
            rowheader: false,
            loadonce: true,
            rowNum: 30,
            headertitles: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            altclass: 'alternate',
            gridComplete: function () {
                //var pagerElement = '#p_' + elementIDs.exportedExitValidateGrid;
                //$(pagerElement + "_right").hide();
                $(elementIDs.btnExitValidateDownloadJQ).hide();
            }
        });

        var pagerElement = '#p_' + elementIDs.exportedExitValidateGrid;
        $(elementIDs.exportedExitValidateGridJQ).jqGrid('navGrid', pagerElement, { edit: false, view: false, add: false, del: false, search: false, refresh : false });
        $(elementIDs.exportedExitValidateGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
        $(elementIDs.exportedExitValidateGridJQ).jqGrid('navButtonAdd', pagerElement,
          {
              caption: '', buttonicon: 'ui-icon-pencil', title: 'Edit', id: 'PBPImportEdit',
              onClickButton: function () {
                  var rowId = $(elementIDs.exportedExitValidateGridJQ).getGridParam('selrow');
                  if (rowId !== undefined && rowId !== null) {
                      var row = $(elementIDs.exportedExitValidateGridJQ).getRowData(rowId);
                      //var folderVersionListUrl = URLs.folderVersionList.replace(/{tenantId}/g, row.TenantID).replace(/\{folderVersionId\}/g, row.FolderVersionId).replace(/\{folderId\}/g, row.FolderId).replace(/{foldeViewMode}/g, viewMode);
                      var folderVersionListUrl = URLs.folderVersionList.replace(/{tenantId}/g, 1).replace(/\{folderVersionId\}/g, row.FolderVersionID).replace(/\{folderId\}/g, row.FolderID).replace(/{foldeViewMode}/g, "Default");
                      folderVersionListUrl = folderVersionListUrl.replace(/\{mode\}/g, true);//since edit button is clicked mode needs to be true
                      window.location.href = folderVersionListUrl + "&navformInstanceId=" + row.FormInstanceID + "&navformDesignVersionId=" + row.FormDesignVersionID + "&fromEV=true";
                  }
              }
          });
        $(elementIDs.exportedExitValidateGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-arrowstop-1-s', title: 'Download MDB files', id: 'btnExitValidateDownload',
            onClickButton: function () {
                var rowId = $(elementIDs.exportedExitValidateGridJQ).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    var row = $(elementIDs.exportedExitValidateGridJQ).getRowData(rowId);
                    var stringData = "<&exitValidateQueueId=" + row.ExitValidateQueueID;
                    stringData += "<&folderVersion=" + row.FolderVersion;
                    $.download(URLs.exitValidateExportDownload, stringData, 'get');
                }
                else {
                    messageDialog.show("Please select a row from the Exit Validate result grid.");
                }
            }
        });
        $(elementIDs.exportedExitValidateGridJQ).jqGrid('navButtonAdd', pagerElement,
         {
             caption: '', buttonicon: 'ui-icon-refresh', title: 'Reload', id: 'btnExitValidateReload',
             onClickButton: function () {
                 loadExitValidateExportGrid();
             }
         });
    }

    function statusFormatter(cellvalue, options, rowObject) {
        if (rowObject.Status == "Failed") {//Errored
            if (rowObject.ErrorMessage != null) {
                var errorMsg = "onclick=javascript:errorLogDialog.show('";
                errorMsg += rowObject.ErrorMessage.replace(/'/g, '"');
                errorMsg += "');";
                errorMsg = errorMsg.replaceAll(" ", "&nbsp;");
                cellvalue = "<a style='text-decoration:underline; color:red' href='javascript:void(0);' " + errorMsg + ">" + cellvalue + "</a>";
            }
        }
        return cellvalue;
    }

    String.prototype.replaceAll = function (search, replacement) {
        var target = this;
        return target.split(search).join(replacement);
    };

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
    
}();


var masterList = function () {
    this.masterList = [];

    var currentInstance = this;
    var URLs = {
        getMasterListCascadeBatch: '/MasterList/GetMasterListCascadeBatch',
        getMasterListCascadeBatchDetail: '/MasterList/GetMasterListCascadeBatchDetail?masterListCascadeBatchID={masterListCascadeBatchID}'
    };
    var elementIDs = {
        masterListCascadeBatchGrid: "mlcb",
        masterListCascadeBatchGridJQ: "#mlcb",
        masterListCascadeBatchDetailGrid: "mlcbd",
        masterListCascadeBatchDetailGridJQ: "#mlcbd"
    };

    var timer = 60;
    this.refreshInterval;
    function init() {
        $(document).ready(function () {
            loadMasterListCascadeBatchGrid();
        });
    }


    function AutoRefreshMasterListCascadeBatchGrid() {
        timer--;
        if (timer == 0) {
            $('input[id*="gs_"]').val("");
            //$('select[id*="gs_"]').val("ALL");
            $(elementIDs.masterListCascadeBatchGridJQ).jqGrid('setGridParam', { sortname: 'name', sortorder: 'desc', search: false, postData: { "filters": null, "searchField": "" } }).trigger("reloadGrid");

            timer = 60;
            ajaxDialog.showPleaseWait();
            setTimeout(function () {
                ajaxDialog.hidePleaseWait();
            }, 1000);
        }
        $("#spnTimer").text(timer + " seconds.").css("font-weight", "Bold");
    }

    function loadMasterListCascadeBatchDetailGrid(masterListCascadeBatchID) {
        //set column list for grid
        var colArray = ['Folder Name', 'Folder Version Number', 'Form Instance Name', 'Plan Code', 'Type', 'Status', 'Processed Date', 'Message'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'FolderName' });
        colModel.push({ name: 'FolderVersionNumber' });
        colModel.push({ name: 'FormInstanceName' });
        colModel.push({ name: 'PlanCode' });
        colModel.push({ name: 'FormDesignName' });
        colModel.push({ name: 'StatusName' });
        colModel.push({ name: 'ProcessedDate', formatter: 'date', formatoptions: JQGridSettings.DateTimeFormatterOptions });
        colModel.push({ name: 'Message' });

        //clean up the grid first - only table element remains after this
        $(elementIDs.masterListCascadeBatchDetailGridJQ).jqGrid('GridUnload');
        //adding the pager element
        $(elementIDs.masterListCascadeBatchDetailGridJQ).parent().append("<div id='p" + elementIDs.masterListCascadeBatchDetailGrid + "'></div>");
        var url = URLs.getMasterListCascadeBatchDetail.replace(/{masterListCascadeBatchID}/g, masterListCascadeBatchID);
        $(elementIDs.masterListCascadeBatchDetailGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Master List Cascade Batch Detail',
            height: '200',
            rowNum: 20,
            //rowList: [10, 20, 30],
            loadonce: true,
            //autowidth: true,
            //shrinkToFit: true,
            viewrecords: true,
            altRows: true,
            hidegrid: false,

            pager: '#p' + elementIDs.masterListCascadeBatchDetailGrid,
            //sortname: 'MasterListCascadeBatchID',
            altclass: 'alternate',
            //subGrid: true,
            gridComplete: function () {
                var rows = $(this).getDataIDs();
                if (rows.length > 0) {
                    $(this).jqGrid("setSelection", rows[0]);
                }
            },
            //To disable edit button when selected folder version is released and approved.
            onSelectRow: function (rowId) {

            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.masterListCascadeBatchDetailGridJQ);
            }
        });
        var pagerElement = '#p' + elementIDs.masterListCascadeBatchDetailGrid;
        //remove default buttons
        $(elementIDs.masterListCascadeBatchDetailGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false, refresh: false });
        $(elementIDs.masterListCascadeBatchDetailGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnenter: true, defaultSearch: 'cn' });
        $(elementIDs.masterListCascadeBatchDetailGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon-refresh', title: 'refresh', id: 'PBPDatabaseNameAdd',
                onClickButton: function () {
                    $(elementIDs.masterListCascadeBatchDetailGridJQ).jqGrid('setGridParam', { sortname: 'name', sortorder: 'desc' }).trigger("reloadGrid");
                    ajaxDialog.showPleaseWait();
                    setTimeout(function () {
                        ajaxDialog.hidePleaseWait();
                    }, 1000);
                }

            });



    }

    function loadMasterListCascadeBatchGrid() {
        //set column list for grid
        var colArray = ['Master List Name', 'Status', 'Start Date', 'End Date', 'Queued By', 'Message', ''];

        //set column models
        var colModel = [];
        colModel.push({ name: 'MasterListName' });
        colModel.push({ name: 'StatusName' });
        colModel.push({ name: 'StartDate', formatter: 'date', formatoptions: JQGridSettings.DateTimeFormatterOptions });
        colModel.push({ name: 'EndDate', formatter: 'date', formatoptions: JQGridSettings.DateTimeFormatterOptions });
        colModel.push({ name: 'QueuedBy' });
        colModel.push({ name: 'ErrorMessage' });
        colModel.push({ name: 'MasterListCascadeBatchID', hidden: true });

        //clean up the grid first - only table element remains after this
        $(elementIDs.masterListCascadeBatchGridJQ).jqGrid('GridUnload');
        //adding the pager element
        $(elementIDs.masterListCascadeBatchGridJQ).parent().append("<div id='p" + elementIDs.masterListCascadeBatchGrid + "'></div>");
        var url = URLs.getMasterListCascadeBatch;
        $(elementIDs.masterListCascadeBatchGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Master List Cascade Batch',
            height: '200',
            rowNum: 20,
            //rowList: [10, 20, 30],
            loadonce: true,
            autowidth: true,
            shrinkToFit: true,
            viewrecords: true,
            altRows: true,
            hidegrid: false,
            pager: '#p' + elementIDs.masterListCascadeBatchGrid,
            sortname: 'StartDate',
            altclass: 'alternate',
            sortorder: 'desc',
            //subGrid: true,
            gridComplete: function () {
                var rows = $(this).getDataIDs();
                if (rows.length > 0) {
                    $(this).jqGrid("setSelection", rows[0]);
                }
            },
            //To disable edit button when selected folder version is released and approved.
            onSelectRow: function (rowId) {

            },
            ondblClickRow: function (rowId, iRow, iCol, e) {
                if (rowId !== undefined && rowId !== null) {
                    var row = $(this).getRowData(rowId);
                    loadMasterListCascadeBatchDetailGrid(row.MasterListCascadeBatchID);
                }

            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.masterListCascadeBatchGridJQ);
            }
        });
        var pagerElement = '#p' + elementIDs.masterListCascadeBatchGrid;
        //remove default buttons
        $(elementIDs.masterListCascadeBatchGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false, refresh: false });
        $(elementIDs.masterListCascadeBatchGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnenter: true, defaultSearch: 'cn' });
        $(elementIDs.masterListCascadeBatchGridJQ).jqGrid('navButtonAdd', pagerElement,
              {
                  caption: '', buttonicon: 'ui-icon-refresh', title: 'refresh', id: 'PBPDatabaseNameAdd',
                  onClickButton: function () {
                      //$(elementIDs.masterListCascadeBatchGridJQ).jqGrid('clearGridData');
                      //$(elementIDs.masterListCascadeBatchGridJQ).jqGrid('setGridParam', { sortname: 'name', sortorder: 'desc' }).trigger('reloadGrid');
                      $('input[id*="gs_"]').val("");
                      //$('select[id*="gs_"]').val("ALL");
                      $(elementIDs.masterListCascadeBatchGridJQ).jqGrid('setGridParam', { sortname: 'name', sortorder: 'desc', search: false, postData: { "filters": null, "searchField": "" } }).trigger("reloadGrid");

                      //$(elementIDs.masterListCascadeBatchGridJQ).trigger("reloadGrid");                                                                 
                      ajaxDialog.showPleaseWait();
                      setTimeout(function () {
                          ajaxDialog.hidePleaseWait();
                      }, 1000);
                  }

              });

        refreshInterval = setInterval(AutoRefreshMasterListCascadeBatchGrid, 1000);

    }

    init();
    return {
        loadMasterListCascadeBatch: function () {
            loadMasterListCascadeBatchGrid();
        }
    }
}();




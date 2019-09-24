var manageReportSetting = function () {
    //variables required for tab management
    var tabs;
    var tabIndex = 1;
    var tabCount = 0;
    var tabNamePrefix = 'DesignTemplate-';
    this.elementIDs = {
        reportConfigJQ: '#reportConfigtab',
        //table element for the Templte Document  Grid
        reportDesignGrid: 'reportDesign',
        //with hash for use with jQuery
        reportDesignGridJQ: '#reportDesign',
        //table element for the Templte Document  Grid
        reportMappingGrid: 'reportMapping',
        //with hash for use with jQuery
        reportMappingGridJQ: '#reportMapping',
    };
    this.URLs = {
        //get Document Template List
        getReportNameList: '/WinwardReport/GetWinwardReportsName?tenantId=1',
        getTemplateDesginUIElementList: '/WinwardReport/GetWinwardReportsName?tenantId=1',
        setTemplateDesignUIElement: '/PrintTemplate/SetTemplateDesignUIElement',
        designTemplateDelete: '/PrintTemplate/deleteDesignTemplate',
    }

    function init() {
        $(document).ready(function () {
            ////To remove style attribute so that Document Design tab is displayed after loading the  page. 
            $(elementIDs.reportConfigJQ).removeAttr("style");
            ////jqueryui tabs
            tabs = $(elementIDs.reportConfigJQ).tabs();
            ////register event for closing a tab page - refer jquery ui documentation
            ////event will be registered for each tab page loaded
            tabs.delegate('span.ui-icon-close', 'click', function () {
                var panelId = $(this).closest('li').remove().attr('aria-controls');
                $('#' + panelId).remove();
                tabCount--;
                tabIndex = 1;
                tabs.tabs('refresh');
            });
            //load the form design grid
            loadReportTemplateGrid();
        });
    }
    init();

    function loadReportMappingGrid(row) {
        this.row = row;
    }
    
    //load Document Template list in grid
    function loadReportTemplateGrid() {
        var currentInstance = this;
        //set column list for grid
        var colArray = ['', 'Report Name'];
        //set column models
        var colModel = [];
        colModel.push({ name: 'ReportId', index: 'TemplateID', key: true, hidden: true, editable: false });
        colModel.push({ name: 'ReportName', index: 'ReportName', editable: false });

        //clean up the grid first - only table element remains after this
        $(elementIDs.reportDesignGridJQ).jqGrid('GridUnload');
        var url = URLs.getReportNameList;
        //adding the pager element
        $(elementIDs.reportDesignGridJQ).parent().append("<div id='p" + elementIDs.reportDesignGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.reportDesignGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Report Name List',
            height: '350',
            rowNum: 25,
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.reportDesignGrid,
            sortname: 'ReportId',
            altclass: 'alternate',
            //load associated form design version grid on selecting a row
            onSelectRow: function (id) {
                var row = $(this).getRowData(id);
                if (row) {
                    //var propGrid = new reportConfigurationGrid(row, formDesignId, formDesignVersionId, status, currentInstance.getGridData(), currentInstance);
                }
				
                var confGrid = new reportConfigurationGrid(row, currentInstance);
				confGrid.loadPropertyGrid();
                //loadReportMappingGrid(row);
                $("#btnUIelementSave").hide();
            },
            //on adding a new form design, reload the grid and set the row to selected
            gridComplete: function () {
                var allRowsInGrid = $(this).getRowData();
                if (allRowsInGrid.length > 0) {
                    //loadReportMappingGrid(allRowsInGrid[0])
                    $("#btnUIelementSave").hide();
                }
                else {
                    $(elementIDs.templateUIElementGridJQ).jqGrid('GridUnload');
                }
            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.reportDesignGridJQ);
            }
        });
        var pagerElement = '#p' + elementIDs.templateDesignGrid;
        //remove default buttons
        $(elementIDs.reportDesignGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        //add button in footer of grid that pops up the add form design dialog
        $(elementIDs.reportDesignGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-plus',
            title: 'Add', id: 'btnFormDesignAdd',
            onClickButton: function () {
                //pdfConfigurationDialog.show();
            }
        });
        //delete button in footer of grid that pops up the delete form design dialog
        $(elementIDs.reportDesignGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-trash',
            title: 'Delete', id: 'btnDocumentTemplateDelete',
            onClickButton: function () {
                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    //get the formDesignVersion List for current form Design
                    var DocumentTemplateList = $(elementIDs.reportDesignGridJQ).jqGrid("getRowData", rowId);
                    if (DocumentTemplateList !== undefined && DocumentTemplateList.length > 0) {
                        messageDialog.show(DocumentDesign.designDeleteValidationMsg);
                    }
                    else {
                        //load confirm dialogue to asset the operation
                        confirmDialog.show(Common.deleteConfirmationMsg, function () {
                            confirmDialog.hide();
                            //delete the template design
                            var designTemplate = {
                                tenantId: 1,
                                templateId: DocumentTemplateList.TemplateID
                            };
                            var promise = ajaxWrapper.postJSON(URLs.designTemplateDelete, designTemplate);
                            //register ajax success callback
                            promise.done(templateDeleteSuccess);
                            //register ajax failure callback
                            promise.fail(pdfConfigurationDialog.showError);
                        });
                    }
                }
            }
        });
        // add filter toolbar to the top
        $(elementIDs.reportDesignGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });
    }   

}();

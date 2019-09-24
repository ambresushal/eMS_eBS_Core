var serviceDesign = function () {
    //variables required for tab management
    var tabs;
    var tabIndex = 1;
    var tabCount = 0;
    var tabNamePrefix = 'serviceDesign-';

    //urls to be accessed for service design
    var URLs = {
        //get Document Design List
        serviceDesignList: '/ServiceDesign/ServiceDesignList?tenantId=1',
        //get Document Design Version
        serviceDesignVersionList: '/ServiceDesign/ServiceDesignVersionList?tenantId=1&serviceDesignId={serviceDesignId}',
        //Finalize Form Design
        serviceDesignVersionFinalize: '/ServiceDesign/FinalizeVersion',
        //Delete Form Design
        serviceDesignDelete: '/ServiceDesign/Delete',
        //Delete Document Design Version
        serviceDesignVersionDelete: '/ServiceDesign/DeleteVersion',
    };

    //element ID's required for service design
    //added in Views/ServiceDesign/Index.cshtml
    var elementIDs = {
        //table element for the Document Design Grid
        serviceDesignGrid: 'fdg',
        //with hash for use with jQuery
        serviceDesignGridJQ: '#fdg',
        //container element for the service design tabs
        serviceDesignTabs: '#servicedesigntabs',
        //table element for the Document Design Version Grid
        serviceDesignVersionGrid: 'fdvg',
        //with hash for use with jQuery
        serviceDesignVersionGridJQ: '#fdvg',
        //for dialog for add/edit of Form Design
        serviceDesignDialog: "#servicedesigndialog",
        //for dialog for add/edit of Document Design Version
        serviceDesignVersionDialog: "#servicedesignversiondialog",
        btnServiceDesignEdit: "#btnServiceDesignEdit",
        btnServiceDesignDelete: "#btnServiceDesignDelete"
    };

    var customSearchOptions = {
        dataEvents:
            [{
                type: 'keypress',
                fn: function (e) {
                    if (e.keyCode != undefined) {
                        //currentFilterElementID = e.target.id;
                    }
                }
            }]
    };

    function init() {
        $(document).ready(function () {
            //To remove style attribute so that Document Design tab is displayed after loading the  page. 
            $(elementIDs.serviceDesignTabs).removeAttr("style");

            //jqueryui tabs
            tabs = $(elementIDs.serviceDesignTabs).tabs();
            //register event for closing a tab page - refer jquery ui documentation
            //event will be registered for each tab page loaded
            tabs.delegate('span.ui-icon-close', 'click', function () {
                var panelId = $(this).closest('li').remove().attr('aria-controls');
                $('#' + panelId).remove();
                tabCount--;
                tabIndex = 1;
                tabs.tabs('refresh');

                //Handled enabling/disabling of edit and delete icons when Document Design Version tab is closed
                var tabName = $(this).parent().find("a").text();
                if (tabName != null && tabName != undefined) {
                    var tabServiceDesignName = tabName.substring(0, tabName.lastIndexOf("-"));
                }
                var designSelectedRowId = $(elementIDs.serviceDesignGridJQ).getGridParam('selrow');
                var selectedRow = $(elementIDs.serviceDesignGridJQ).getRowData(designSelectedRowId);
                var selectedServiceDesign = selectedRow.DisplayText;

                if (selectedServiceDesign == tabServiceDesignName) {
                    //disableEditDeleteButtons(selectedServiceDesign);
                }

            });
            //load the service design grid
            loadServiceDesignGrid();
        });
    }

    //ajax callback success - reload Form Desing  grid
    function serviceDesignDeleteSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(ServiceDesign.deleteServiceDesign);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
        loadServiceDesignGrid();
    }
    //ajax error callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    //load service design list in grid
    function loadServiceDesignGrid() {
        //set column list for grid
        var colArray = ['', 'Web API Name', 'Web API Method Name', 'Does Return A List'];

        //set column models
        var colModel = [];
        //colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true, search: false, });
        colModel.push({ name: 'ServiceDesignId', index: 'ServiceDesignId', key: true, hidden: true });
        colModel.push({ name: 'ServiceName', index: 'ServiceName', editable: false, width: '305px', searchoptions: customSearchOptions });
        colModel.push({ name: 'ServiceMethodName', index: 'ServiceMethodName', editable: false, width: '305px', searchoptions: customSearchOptions });
        colModel.push({ name: 'DoesReturnAList', index: 'DoesReturnAList', editable: false, width: '200px', searchable: false, formatter: booleanValueImageFormatter, unformat: booleanValueImageUnFormatter });

        //clean up the grid first - only table element remains after this
        $(elementIDs.serviceDesignGridJQ).jqGrid('GridUnload');

        var url = URLs.serviceDesignList;
        //adding the pager element
        $(elementIDs.serviceDesignGridJQ).parent().append("<div id='p" + elementIDs.serviceDesignGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.serviceDesignGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Web API Design List',
            height: '360',
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.serviceDesignGrid,
            sortname: 'ServiceDesignId',
            altclass: 'alternate',
            //load associated service design version grid on selecting a row
            onSelectRow: function (id) {
                var row = $(this).getRowData(id);
                if (row)
                    loadServiceDesignVersionGrid(row.ServiceDesignId, row.ServiceName);
            },
            //on adding a new service design, reload the grid and set the row to selected
            gridComplete: function () {
                var rows = $(this).getDataIDs();
                var rowId;
                if (rows.length > 0) {
                    rowId = rows[0];
                }
                var serviceDesignName = $(elementIDs.serviceDesignDialog + ' input').val();
                var newId;
                for (index = 0; index < rows.length; index++) {
                    row = $(this).getRowData(rows[index]);
                    if (serviceDesignName === row.ServiceName) {
                        newId = row.ServiceId;
                        break;
                    }
                }
                if (newId !== undefined) {
                    $(this).jqGrid('setSelection', newId);
                }
                else {
                    $(this).jqGrid('setSelection', rowId);
                }
                //to check for claims..               
                var objMap = {
                    edit: "#btnServiceDesignEdit",
                    add: "#btnServiceDesignAdd",
                    remove: "#btnServiceDesignDelete"
                };
                /*TODO: Add code for Checking application Claims*/
                //checkApplicationClaims(claims, objMap, URLs.serviceDesignList);

            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.serviceDesignGridJQ);
            }
        });

        var pagerElement = '#p' + elementIDs.serviceDesignGrid;
        //remove default buttons
        $(elementIDs.serviceDesignGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        //add button in footer of grid that pops up the add service design dialog
        $(elementIDs.serviceDesignGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-plus',
            title: 'Add', id: 'btnServiceDesignAdd',
            onClickButton: function () {
                //load Document Design dialog on click - see serviceDesignDialog function below
                serviceDesignDialog.show('', 'add');
            }
        });
        //edit button in footer of grid that pops up the edit service design dialog
        $(elementIDs.serviceDesignGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-pencil',
            title: 'Edit', id: 'btnServiceDesignEdit',
            onClickButton: function () {

                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    var row = $(this).getRowData(rowId);
                    //load Document Design dialog for edit on click - see serviceDesignDialog function below
                    serviceDesignDialog.show(row, 'edit');
                }
            }
        });

        //delete button in footer of grid that pops up the delete service design dialog
        $(elementIDs.serviceDesignGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-trash',
            title: 'Delete', id: 'btnServiceDesignDelete',
            onClickButton: function () {
                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    //get the serviceDesignVersion List for current service Design
                    var serviceDesignVersionList = $(elementIDs.serviceDesignVersionGridJQ).getRowData();

                    if (serviceDesignVersionList !== undefined && serviceDesignVersionList.length > 0) {
                        messageDialog.show(ServiceDesign.designDeleteValidationMsg);
                    }
                    else {
                        //load confirm dialogue to asset the operation
                        confirmDialog.show(Common.deleteConfirmationMsg, function () {
                            confirmDialog.hide();

                            //delete the service design
                            var serviceDesignDelete = {
                                tenantId: 1,
                                serviceDesignId: rowId
                            };
                            var promise = ajaxWrapper.postJSON(URLs.serviceDesignDelete, serviceDesignDelete);
                            //register ajax success callback
                            promise.done(serviceDesignDeleteSuccess);
                            //register ajax failure callback
                            promise.fail(showError);
                        });
                    }
                }

                else {
                    messageDialog.show(ServiceDesign.inProgressDesignSelectionMsg);
                }
            }
        });
        // add filter toolbar to the top
        $(elementIDs.serviceDesignGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });
    }

    //load service design verion in grid
    function loadServiceDesignVersionGrid(serviceDesignId, serviceDesignName) {
        //set column list
        var colArray = ['TenantId', 'Service Design Version ID', 'Web API Effective Date', 'Web API Version', 'FormDesignID', 'Document Name', 'Form Design Version ID', 'Document Version Number', 'Finalized', '', ''];

        //set column models
        var colModel = [];
        colModel.push({ name: 'TenantId', index: 'TenantId', hidden: true, search: false, });
        colModel.push({ name: 'ServiceDesignVersionId', index: 'ServiceDesignVersionId', key: true, hidden: true, width: 100, align: 'center', search: false, });
        colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', width: '130px', align: 'center', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateServiceatterOptions });
        colModel.push({ name: 'VersionNumber', index: 'VersionNumber', width: '120px', align: 'center', editable: false });
        colModel.push({ name: 'FormDesignID', index: 'FormDesignID', align: 'right', editable: false, hidden: true });
        colModel.push({ name: 'FormDesignName', index: 'FormDesignName', width: '200px', align: 'left', editable: false });
        colModel.push({ name: 'FormDesignVersionID', index: 'FormDesignVersionID', align: 'right', editable: false, hidden: true });
        colModel.push({ name: 'FormDesignVersionNumber', index: 'FormDesignVersionNumber', width: '150px', align: 'center', editable: false });
        colModel.push({
            name: 'IsFinalized', index: 'IsFinalized', width: '100px', align: 'center', editable: false, formatter: function (cellValue, options, rowObject) {
                if (rowObject.IsFinalized) {
                    return "Yes";
                }
                else {
                    return "No";
                }
            }
        });
        colModel.push({ name: 'IsFinalized', index: 'IsFinalized', width: '315px', align: 'center', editable: false, hidden: true });
        colModel.push({ name: 'Action', index: 'Action', width: '50px', align: 'center', sortable: false, search: false, editable: false, formatter: actionFormatter });

        //get URL for grid
        var serviceDesignVersionListUrl = URLs.serviceDesignVersionList.replace(/\{serviceDesignId\}/g, serviceDesignId);

        //unload previous grid values
        $(elementIDs.serviceDesignVersionGridJQ).jqGrid('GridUnload');
        //add pager element
        $(elementIDs.serviceDesignVersionGridJQ).parent().append("<div id='p" + elementIDs.serviceDesignVersionGrid + "'></div>");
        //load grid - refer jqGrid documentation for details
        $(elementIDs.serviceDesignVersionGridJQ).jqGrid({
            url: serviceDesignVersionListUrl,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Web API Design Version List - ' + serviceDesignName,
            forceFit: true,
            height: '360',
            autowidth: false,
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
            ExpandColumn: 'Label',
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.serviceDesignVersionGrid,
            sortname: 'ServiceDesignVersionId',
            altclass: 'alternate',
            //register events to load the service design version in a new tab page
            //this is on click of the span which has the image in the last column
            gridComplete: function () {
                var rows = $(this).getDataIDs();
                var reverse = rows.reverse();
                for (index = 0; index < rows.length; index++) {
                    row = $(this).getRowData(rows[index]);
                    $('#fvds' + row.ServiceDesignVersionId).click(function () {
                        var serviceDesignVersionId = this.id.replace(/fvds/g, '');
                        //launch a new tab here for this service design version
                        //TODO: pass TenantId too
                        var currentRow = $(elementIDs.serviceDesignVersionGridJQ).getRowData(serviceDesignVersionId);
                        //increment to manage adding /deletesof tab pages
                        var tabName = serviceDesignName + '-' + currentRow.VersionNumber;
                        $(elementIDs.btnServiceDesignEdit).addClass('ui-state-disabled');
                        $(elementIDs.btnServiceDesignDelete).addClass('ui-state-disabled');
                        //if service design version is already loaded, do not load again but make it active
                        var foundIndex;
                        if (tabs != undefined) {
                            tabs.find('.ui-tabs-anchor').each(function (index, element) {
                                if ($(this).text() == tabName) {
                                    foundIndex = index;
                                }
                            });
                        }
                        else {
                            foundIndex = 0;
                        }
                        if (foundIndex > 0) {
                            for (var i = 0; i < Finalized.length ; i++) {
                                if (Finalized[i].FORMDESIGNVERSIONID == serviceDesignVersionId && Finalized[i].ISFINALIZED == 1) {
                                    $(".ui-tabs-anchor").trigger("click");
                                }
                            }
                        }
                        if (foundIndex > 0) {
                            tabs.tabs('option', 'active', foundIndex);
                        }
                        else {
                            tabIndex++;
                            tabCount++;
                            //create serviceDesignVersion instance - load tab     
                            var sdVersion = new serviceDesignVersion(currentRow, serviceDesignId, serviceDesignName, tabNamePrefix + tabIndex, tabIndex - 1, tabCount, tabs);
                            sdVersion.loadTabPage();
                        }
                    });
                }
                //disableEditDeleteButtons(serviceDesignName);
                var rowId;
                if (rows.length > 0) {
                    rowId = rows[0];
                }
                $(this).jqGrid('setSelection', rowId);

                //to check for claims..  
                var objMap = {
                    edit: "#btnServiceDesignVersionEdit",
                    add: "#btnServiceDesignVersionAdd",
                    remove: "#btnServiceDesignVersionDelete",
                    finalized: "#btnServiceDesignVersionFinalized",
                    preview: "#btnServiceDesignVersionPreview",
                };
                /*TODO: Add code for Checking application Claims*/
                //checkApplicationClaims(claims, objMap, URLs.serviceDesignVersionList);
                //authorizeServiceDesignVersionList($(this), URLs.serviceDesignVersionList);

            },
            onSelectRow: function (rowID) {
                if (rowID !== undefined && rowID !== null) {
                    var row = $(this).getRowData(rowID);
                    if (row.IsFinalized == true) {
                        $('#btnServiceDesignVersionEdit').addClass('ui-icon-hide');
                        $('#btnServiceDesignVersionDelete').addClass('ui-icon-hide');
                        $('#btnServiceDesignVersionFinalized').addClass('ui-icon-hide');
                    }
                    else {
                        $('#btnServiceDesignVersionEdit').removeClass('ui-icon-hide');
                        $('#btnServiceDesignVersionDelete').removeClass('ui-icon-hide');
                        $('#btnServiceDesignVersionFinalized').removeClass('ui-icon-hide');
                    }
                }
            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.serviceDesignGridJQ);
            }
        });
        var pagerElement = '#p' + elementIDs.serviceDesignVersionGrid;
        //remove default buttons
        $(elementIDs.serviceDesignVersionGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        //button in footer of grid for add 
        $(elementIDs.serviceDesignVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-plus', title: 'Add', id: 'btnServiceDesignVersionAdd',
            onClickButton: function () {
                //show dialog only if all versions are finalized
                var rows = $(this).getDataIDs();
                //$(elementIDs.serviceDesignVersionDialog + ' select').empty();
                var showDialog = true;

                var rowsAll = $(this).getRowData();

                var filterList = rowsAll.filter(function (ct) {
                    if (ct.IsFinalized == "false") {
                        showDialog = false;
                    }
                });

                if (showDialog === true) {
                    var versionData = $(elementIDs.serviceDesignVersionGridJQ).jqGrid('getGridParam', 'data');
                    if (versionData) {
                        serviceDesignVersionDialog.show(versionData[0], 'add', serviceDesignId);
                    }
                    else {
                        serviceDesignVersionDialog.show(null, 'add', serviceDesignId);
                    }
                }
                else {
                    //show message dialog
                    messageDialog.show(ServiceDesign.inProgressDesignValidationMsg);
                }
            }
        });
        //button in footer of grid for edit
        $(elementIDs.serviceDesignVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-pencil', title: 'Edit', id: 'btnServiceDesignVersionEdit',
            onClickButton: function () {
                //allow edit only if currently selected row is in progress
                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    var row = $(this).getRowData(rowId);
                    if (row.IsFinalized == "false") {
                        serviceDesignVersionDialog.show(row, 'edit', serviceDesignId);
                    }
                    else {
                        messageDialog.show(ServiceDesign.designModificationValidationMsg);
                    }
                }
                else {
                    messageDialog.show(Common.selectRowMsg);
                }
            }
        });
        //button in footer of grid for delete
        $(elementIDs.serviceDesignVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-trash', title: 'Delete', id: 'btnServiceDesignVersionDelete',
            onClickButton: function () {
                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    var row = $(this).getRowData(rowId);
                    var row = $(elementIDs.serviceDesignVersionGridJQ).getRowData(rowId);
                    if (row.IsFinalized == "false") {
                        //load Document Design dialog for delete on click - see serviceDesignDialog function below
                        confirmDialog.show(Common.deleteConfirmationMsg, function () {
                            confirmDialog.hide();
                            var serviceDesignVersionDelete = {
                                tenantId: 1,
                                serviceDesignVersionId: rowId,
                                serviceDesignId: serviceDesignId
                            };
                            var promise = ajaxWrapper.postJSON(URLs.serviceDesignVersionDelete, serviceDesignVersionDelete);
                            //register ajax success callback
                            promise.done(serviceDesignSuccess);
                            //register ajax failure callback
                            promise.fail(showError);

                        });
                    }
                    else {
                        messageDialog.show(ServiceDesign.deleteDesignValidationMsg);
                    }
                }
                else {
                    messageDialog.show(ServiceDesign.inProgressDesignSelectionMsg);
                }
            }
        });
        //button in footer of grid for finalizing a Document Design Version
        $(elementIDs.serviceDesignVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-locked', title: 'Finalized', id: 'btnServiceDesignVersionFinalized',
            onClickButton: function () {
                var rowId = $(elementIDs.serviceDesignVersionGridJQ).getGridParam('selrow');
                if (rowId != null) {
                    var row = $(elementIDs.serviceDesignVersionGridJQ).getRowData(rowId);
                    if (row.IsFinalized == "false") {
                        //show confirm dialog
                        confirmDialog.show(ServiceDesign.confirmFinalizationMsg, function () {
                            confirmDialog.hide();
                            finalizeServiceDesignVersion(row.ServiceDesignVersionId);
                        })
                    }
                    else {
                        messageDialog.show(ServiceDesign.inProgressDesignSelectionMsg);
                    }
                }
                else {
                    messageDialog.show(ServiceDesign.inProgressDesignSelectionMsg);
                }
            }
        });
        //button in footer of grid for finalizing a Document Design Version
        $(elementIDs.serviceDesignVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-clipboard', title: 'Preview', id: 'btnServiceDesignVersionPreview',
            onClickButton: function () {
                var rowId = $(elementIDs.serviceDesignVersionGridJQ).getGridParam('selrow');
                if (rowId != null) {
                    var row = $(elementIDs.serviceDesignVersionGridJQ).getRowData(rowId);
                    if (row) {
                        servicedesignversionoutputdialog.show(serviceDesignId, row.ServiceDesignVersionId, serviceDesignName, row.VersionNumber);
                    }
                    else {
                        messageDialog.show(Common.errorMsg);
                    }
                }
                else {
                    messageDialog.show(Common.pleaseSelectRowMsg);
                }
            }
        });
        //add filter toolbar at the top of grid
        $(elementIDs.serviceDesignVersionGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });

    }

    function finalizeServiceDesignVersion(serviceDesignVersionId) {
        var serviceDesignVersionFinalize = {
            tenantId: 1,
            serviceDesignVersionId: serviceDesignVersionId,
        };
        //submit using ajax for finalization
        var promise = ajaxWrapper.postJSON(URLs.serviceDesignVersionFinalize, serviceDesignVersionFinalize);
        //To close comments dialog.
        $(elementIDs.commentsDialog).dialog("close");
        //success callback
        promise.done(function (xhr) {
            if (xhr.Result === ServiceResult.SUCCESS) {
                messageDialog.show(ServiceDesign.finalizationSuccessMsg);
            }
            else {
                messageDialog.show(Common.errorMsg);
            }
            serviceDesign.loadServiceDesignVersion();
        });
        //failure callback
        promise.fail(showError);
    }

    //Edit and delete document design buttons are disabled if document is already opened in edit mode
    function disableEditDeleteButtons(serviceDesignName) {
        $(elementIDs.btnServiceDesignEdit).removeClass('ui-state-disabled');
        $(elementIDs.btnServiceDesignDelete).removeClass('ui-state-disabled');
        var rows = $(elementIDs.serviceDesignVersionGridJQ).getDataIDs();
        for (var i = 0; i < rows.length; i++) {
            var rowData = $(elementIDs.serviceDesignVersionGridJQ).getRowData(rows[i]);
            var tabName = serviceDesignName + "-" + rowData.Version;
            var isDisabled = false;
            tabs.find('.ui-tabs-anchor').each(function (index, element) {
                if (this.id == tabName) {
                    $(elementIDs.btnServiceDesignEdit).addClass('ui-state-disabled');
                    $(elementIDs.btnServiceDesignDelete).addClass('ui-state-disabled');
                    isDisabled = true;
                    return false;
                }
            });
            if (isDisabled) break;
        }
    }

    //add custom functionality to action column
    //used for custom serviceatting columns- see  jqGrid colmodel (formatter property) of Document Design Version grid 
    //sets the icons based on status of the Document Design Version column
    function actionFormatter(cellValue, options, rowObject) {
        if (rowObject.IsFinalized == true) {
            return "<span id = 'fvds" + rowObject.ServiceDesignVersionId + "' class='ui-icon ui-icon-document view' title = 'View Design' style = 'cursor: pointer'/>";
        }
        else {
            return "<span id = 'fvds" + rowObject.ServiceDesignVersionId + "' class='ui-icon ui-icon-pencil edit' title = 'Modify Design' style = 'cursor: pointer'/>";
        }
    }

    //ajax callback success - reload Form Desing Version grid
    function serviceDesignSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(ServiceDesign.designVersionDeleteMsg);
        }
        else {
            messageDialog.show(xhr.Items[0].Messages[0]);
        }
        serviceDesign.loadServiceDesignVersion();
    }

    //initialization of the Document Design when the serviceDesign function is loaded in browser and invoked
    init();

    function booleanValueImageFormatter(cellvalue, options, rowObject) {
        if (cellvalue) {
            return '<span align="center" class="ui-icon ui-icon-check" style="margin:auto"></span>';
        }
        else {
            return '<span align="center" class="ui-icon ui-icon-close" style="margin:auto" ></span>';
        }
    }

    function booleanValueImageUnFormatter(cellvalue, options, rowObject) {
        return rowObject.DoesReturnAList;
    }
    //
    return {
        loadServiceDesign: function () {
            loadServiceDesignGrid();
        },
        loadServiceDesignVersion: function () {
            //load service design version for currently selected row of Document Design grid
            var rowId = $(elementIDs.serviceDesignGridJQ).getGridParam('selrow');
            var row = $(elementIDs.serviceDesignGridJQ).getRowData(rowId);
            loadServiceDesignVersionGrid(rowId, row.ServiceName);
        }
    }
}();

//contains functionality for the Document Design add/edit dialog
var serviceDesignDialog = function () {
    var URLs = {
        //url for Add Service Design
        serviceDesignAdd: '/ServiceDesign/Add',
        //url for Update Service Design
        serviceDesignUpdate: '/ServiceDesign/Update'
    }

    //see element id's in Views\ServiceDesign\Index.cshtml
    var elementIDs = {
        //service design grid element
        serviceDesignGrid: 'fdg',
        //with hash for jquery
        serviceDesignGridJQ: '#fdg',
        //service design dialog element
        serviceDesignDialog: "#servicedesigndialog",

        serviceDesignNameJQ: '#servicedesignname',
        serviceDesignMethodNameJQ: '#servicemethodname',
        doesReturnAListJQ: '#doesreturnalist',
    };

    //maintains dialog state - add or edit
    var serviceDesignDialogState;

    //ajax success callback - for add/edit
    function serviceDesignSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            if (serviceDesignDialogState === 'edit')
                messageDialog.show(ServiceDesign.designUpdateSuccessMsg);
            else
                messageDialog.show(ServiceDesign.designAddSuccessMsg);

        }
        else {
            messageDialog.show(Common.errorMsg);
        }
        //reload service design grid 
        serviceDesign.loadServiceDesign();
        //reset dialog elements
        $(elementIDs.serviceDesignDialog + ' div').removeClass('has-error');
        $(elementIDs.serviceDesignNameJQ).next(' .help-block').text(ServiceDesign.addNewDesignNameMsg);
        $(elementIDs.serviceDesignMethodNameJQ).next(' .help-block').text(ServiceDesign.addNewMethodNameMsg);
        $(elementIDs.serviceDesignDialog).dialog('close');
    }
    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
    //init dialog on load of page
    function init() {
        //register dialog for grid row add/edit
        $(elementIDs.serviceDesignDialog).dialog({
            autoOpen: false,
            height: 350,
            width: 450,
            modal: true
        });
        //register event for Add/Edit button click on dialog
        $(elementIDs.serviceDesignDialog + ' button').on('click', function () {
            //check if name is already used
            var serviceDesignName = $(elementIDs.serviceDesignNameJQ).val();
            var serviceMethodName = $(elementIDs.serviceDesignMethodNameJQ).val();
            var doesReturnAList = $(elementIDs.doesReturnAListJQ).is(":checked");
            var serviceDesignList = $(elementIDs.serviceDesignGridJQ).getRowData();

            if (serviceDesignDialogState == 'edit') {
                var selectedRowId = $(elementIDs.serviceDesignGridJQ).getGridParam('selrow');
                serviceDesignList = serviceDesignList.filter(function (r) {
                    return r.ServiceDesignId != selectedRowId;
                })
            }

            var filterList = serviceDesignList.filter(function (ct) {
                return compareStrings(ct.ServiceName, serviceDesignName, true);
            });

            var serviceMethodFilterList = serviceDesignList.filter(function (ct) {
                return compareStrings(ct.ServiceMethodName, serviceMethodName, true);
            });
            var isValid = true;
            //validate service name
            if (filterList !== undefined && filterList.length > 0) {
                $(elementIDs.serviceDesignNameJQ).parent(' div').addClass('has-error');
                $(elementIDs.serviceDesignNameJQ).next(' .help-block').text(ServiceDesign.designNameAlreadyExistsMsg);
                isValid = false;
            }
            else if (serviceDesignName == '') {
                $(elementIDs.serviceDesignNameJQ).parent(' div').addClass('has-error');
                $(elementIDs.serviceDesignNameJQ).next(' .help-block').text(ServiceDesign.designNameRequiredMsg);
                isValid = false;
            }

            if (serviceMethodFilterList !== undefined && serviceMethodFilterList.length > 0) {
                $(elementIDs.serviceDesignMethodNameJQ).parent(' div').addClass('has-error');
                $(elementIDs.serviceDesignMethodNameJQ).next(' .help-block').text(ServiceDesign.methodNameAlreadyExistsMsg);
                isValid = false;
            }
            else if (serviceMethodName == '') {
                $(elementIDs.serviceDesignMethodNameJQ).parent(' div').addClass('has-error');
                isValid = false;
                $(elementIDs.serviceDesignMethodNameJQ).next(' .help-block').text(ServiceDesign.methodNameRequiredMsg);
            }

            if (isValid) {
                //save the new service design
                var rowId = $(elementIDs.serviceDesignGridJQ).getGridParam('selrow');
                var serviceDesignAdd = {
                    tenantId: 1,
                    serviceDesignId: rowId,
                    serviceName: serviceDesignName,
                    serviceMethodName: serviceMethodName,
                    doesReturnAList: doesReturnAList,
                };
                var url;
                if (serviceDesignDialogState === 'add') {
                    url = URLs.serviceDesignAdd;
                }
                else {
                    url = URLs.serviceDesignUpdate;
                }
                //ajax call to add/update
                var promise = ajaxWrapper.postJSON(url, serviceDesignAdd);
                //register ajax success callback
                promise.done(serviceDesignSuccess);
                //register ajax failure callback
                promise.fail(showError);
            }
        });
    }
    //initialize the dialog after this js are loaded
    init();

    //these are the properties that can be called by using serviceDesignDialog.<Property>
    //eg. serviceDesignDialog.show('name','add');
    return {
        show: function (row, action) {
            serviceDesignDialogState = action;
            //set eleemnts based on whether the dialog is opened in add or edit mode
            $(elementIDs.serviceDesignDialog + ' div').removeClass('has-error');
            if (serviceDesignDialogState == 'add') {
                $(elementIDs.serviceDesignDialog).dialog('option', 'title', ServiceDesign.addServiceDesign);
                $(elementIDs.serviceDesignNameJQ).next(' .help-block').text(ServiceDesign.addNewDesignNameMsg);
                $(elementIDs.serviceDesignMethodNameJQ).next(' .help-block').text(ServiceDesign.addNewMethodNameMsg);
                $(elementIDs.doesReturnAListJQ).next(' .help-block').text(ServiceDesign.doesReturnAListMsg);
                $(elementIDs.doesReturnAListJQ).removeAttr('disabled');

                $(elementIDs.serviceDesignNameJQ).val('');
                $(elementIDs.serviceDesignMethodNameJQ).val('');
                $(elementIDs.doesReturnAListJQ).removeAttr('checked');
            }
            else {
                $(elementIDs.serviceDesignDialog).dialog('option', 'title', ServiceDesign.editServiceDesign);
                $(elementIDs.serviceDesignNameJQ).next(' .help-block').text(ServiceDesign.editNewDesignNameMsg);
                $(elementIDs.serviceDesignMethodNameJQ).next(' .help-block').text(ServiceDesign.editNewMethodNameMsg);
                $(elementIDs.serviceDesignMethodNameJQ).next(' .help-block').text(ServiceDesign.doesReturnAListMsg);
                $(elementIDs.serviceDesignNameJQ).val(row.ServiceName);
                $(elementIDs.serviceDesignMethodNameJQ).val(row.ServiceMethodName);
                row.DoesReturnAList == "true" ? $(elementIDs.doesReturnAListJQ).attr("checked", "checked") : $(elementIDs.doesReturnAListJQ).removeAttr("checked");
                $(elementIDs.doesReturnAListJQ).attr('disabled', true);
            }
            //open the dialog - uses jqueryui dialog
            $(elementIDs.serviceDesignDialog).dialog("open");
        }
    }
}(); //invoked soon after loading

//contains functionality for the Document Design Version add/edit dialog
var serviceDesignVersionDialog = function () {
    var URLs = {
        //url for adding Document Design Version
        serviceDesignVersionAdd: '/ServiceDesign/AddVersion',
        //url for updating Document Design Version
        serviceDesignVersionUpdate: '/ServiceDesign/UpdateVersion',
        //url for loading FormDesigns
        getFormDesignList: '/FormDesign/FormDesignList?tenantId=1',
        //url for loading FormDesign versions
        getFormDesignVersionList: '/FormDesign/FormDesignVersionList?tenantId=1&formDesignId={formDesignId}',
    }

    var elementIDs = {
        //service design version grid
        serviceDesignVersionGrid: 'fdvg',
        //with hash for jquery
        serviceDesignVersionGridJQ: '#fdvg',
        //container for service design version add/edit dialog
        serviceDesignVersionDialog: "#servicedesignversiondialog",

        serviceDesignVersionEffectiveDateJQ: '#sdveffectivedate',
        serviceDesignVersionEffectiveDateHelpBlockJQ: '#sdveffectivedateHelpblock',
        serviceDesignVersionFormDesignIDJQ: '#sdvformdesignid',
        serviceDesignVersionFormDesignIDHelpBlockJQ: '#sdvformdesignidHelpblock',
        serviceDesignVersionFormDesignVersionIDJQ: '#sdvformdesignversion',
        serviceDesignVersionFormDesignVersionIDHelpBlockJQ: '#sdvformdesignversionHelpblock',
    };

    //dialog state - add or edit
    var serviceDesignVersionDialogState;
    //current Document Design ID
    var currentServiceDesignId;
    var currentServiceDesignVersionId;
    var currentFormDesignId;
    //ajax success callback called after adding/editing Document Design version
    function serviceDesignVersionSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            if (serviceDesignVersionDialogState === 'edit')
                messageDialog.show(ServiceDesign.designVersionUpdateSuccessMsg);
            else
                messageDialog.show(ServiceDesign.designVersionAddSuccessMsg);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
        //reload the grid
        serviceDesign.loadServiceDesignVersion();

        //close the dialog - jqueryui dialog used
        $(elementIDs.serviceDesignVersionDialog).dialog('close');
    }
    //ajax error callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    ////Validate day for fabruary month 
    function daysInFebruary(year) {
        // February has 29 days in any year evenly divisible by four,
        // EXCEPT for centurial years which are not also divisible by 400.
        return (((year % 4 == 0) && ((!(year % 100 == 0)) || (year % 400 == 0))) ? 29 : 28);
    }

    //Calculate number of day in month
    function getDaysInMonth(n) {
        for (var i = 1; i <= n; i++) {
            this[i] = 31
            if (i == 4 || i == 6 || i == 9 || i == 11) { this[i] = 30 }
            if (i == 2) { this[i] = 29 }
        }
        return this
    }

    function getMinimumEffectiveDate(serviceDesignVersionList) {
        var min = serviceDesignVersionList.sort(function (a, b) {
            return new Date(a.EffectiveDate) - new Date(b.EffectiveDate);
        });
        return min[0].EffectiveDate;
    }

    //init dialog on load of page
    function init() {
        //register dialog for grid row add
        $(elementIDs.serviceDesignVersionDialog).dialog({
            autoOpen: false,
            height: 320,
            width: 380,
            modal: true
        });
        //select datapicker - jqueryui
        $(elementIDs.serviceDesignVersionEffectiveDateJQ).datepicker({
            dateServiceat: 'mm/dd/yy',
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-61:c+20',
            showOn: "both",
            //CalenderIcon path declare in golbalvariable.js
            buttonImage: Icons.CalenderIcon,
            buttonImageOnly: true
        });

        loadFormDesign();

        $(elementIDs.serviceDesignVersionFormDesignIDHelpBlockJQ).text(ServiceDesign.formDesignHelpBlockMsg);
        $(elementIDs.serviceDesignVersionFormDesignVersionIDHelpBlockJQ).text(ServiceDesign.formDesignVersionHelpBlockMsg);
        $(elementIDs.serviceDesignVersionEffectiveDateHelpBlockJQ).text(ServiceDesign.effectiveDateHelpblockMsg);

        //register event for button click to add 
        $(elementIDs.serviceDesignVersionDialog + ' button').on('click', function () {
            if (validate()) {
                save();
                serviceDesign.loadServiceDesignVersion();
            }
        });
    }

    init();
    //initialize the dialog soon after this js is loaded   
    //these are the properties that can be called by using serviceDesignVersionDialog.<Property>
    //eg. serviceDesignVersionDialog.show('name','add',serviceDesignId);

    function save() {
        //check if a version with the same effective date is already present
        var serviceDesignVersionId = 0;
        var newEffectiveDate = $(elementIDs.serviceDesignVersionEffectiveDateJQ).val();
        //save the new service design
        var rows = $(elementIDs.serviceDesignVersionGridJQ).getDataIDs();
        var rowId = $(elementIDs.serviceDesignVersionGridJQ).getGridParam('selrow');

        var isFirstVersion = false;
        if (rows.length == 0)
            isFirstVersion = true;
        else {
        }
        //create object to be posted
        var serviceDesignVersion = {};
        var url;
        //add or edit based on the current mode the dialog is opened in
        if (serviceDesignVersionDialogState === 'add') {
            serviceDesignVersion = {
                tenantId: 1,
                serviceDesignId: currentServiceDesignId,
                serviceDesignVersionId: serviceDesignVersionId,
                isFirstVersion: isFirstVersion,
                effectiveDate: newEffectiveDate,
                formDesignID: $(elementIDs.serviceDesignVersionFormDesignIDJQ).val(),
                formDesignVersionID: $(elementIDs.serviceDesignVersionFormDesignVersionIDJQ).val(),
            };
            url = URLs.serviceDesignVersionAdd;
        }
        else {
            var rowId = $(elementIDs.serviceDesignVersionGridJQ).getDataIDs();
            serviceDesignVersion = {
                tenantId: 1,
                serviceDesignVersionId: currentServiceDesignVersionId,
                formDesignVersionId: $(elementIDs.serviceDesignVersionFormDesignVersionIDJQ).val(),
                effectiveDate: newEffectiveDate,
            }
            url = URLs.serviceDesignVersionUpdate;
        }
        //ajax call for post
        var promise = ajaxWrapper.postJSON(url, serviceDesignVersion);
        //register success callback
        promise.done(serviceDesignVersionSuccess);
        //register failure callback
        promise.fail(showError);
    }

    function loadFormDesign() {
        $(elementIDs.serviceDesignVersionFormDesignIDJQ).empty();
        $(elementIDs.serviceDesignVersionFormDesignIDJQ).append("<option value='0'>" + "--Select--" + "</option>");

        //ajax call for drop down list of account names
        var promise = ajaxWrapper.getJSON(URLs.getFormDesignList);

        promise.done(function (list) {
            if (list.length != 0) {
                for (i = 0; i < list.length; i++) {
                    $(elementIDs.serviceDesignVersionFormDesignIDJQ).append("<option value=" + list[i].FormDesignId + ">" + list[i].FormDesignName + "</option>");
                }
            }
            $(elementIDs.serviceDesignVersionFormDesignIDJQ).val(0);
            $(elementIDs.serviceDesignVersionFormDesignVersionIDJQ).empty();
            $(elementIDs.serviceDesignVersionFormDesignVersionIDJQ).addClass("disabled").attr('disabled', 'disabled');

            $(elementIDs.serviceDesignVersionFormDesignIDJQ).on('change', function (e) {
                loadFormDesignVersion($(elementIDs.serviceDesignVersionFormDesignIDJQ).val());
            });
        });
        promise.fail(showError);
    }

    function loadFormDesignVersion(formDesignId, selectedFormDesignVersionId) {
        if (formDesignId != 0) {
            $(elementIDs.serviceDesignVersionFormDesignVersionIDJQ).empty();
            $(elementIDs.serviceDesignVersionFormDesignVersionIDJQ).append("<option value='0'>" + "--Select--" + "</option>");
            $(elementIDs.serviceDesignVersionFormDesignVersionIDJQ).removeClass("disabled").removeAttr('disabled');

            //ajax call for drop down list of account names
            var promise = ajaxWrapper.getJSON(URLs.getFormDesignVersionList.replace('{formDesignId}', formDesignId));

            promise.done(function (list) {
                if (list.length != 0) {
                    for (i = 0; i < list.length; i++) {
                        //TODO: Add a condition here to allow users to select only FormDesignVersions which are greater than the current Selected Version
                        //Need to verify this from Phil
                        //if (selectedFormDesignVersionId) {
                        //    if (list[i].FormDesignVersionId >= selectedFormDesignVersionId) {
                        //        $(elementIDs.serviceDesignVersionFormDesignVersionIDJQ).append("<option value=" + list[i].FormDesignVersionId + ">" + list[i].Version + "</option>");
                        //    }
                        //}
                        //else
                        $(elementIDs.serviceDesignVersionFormDesignVersionIDJQ).append("<option value=" + list[i].FormDesignVersionId + ">" + list[i].Version + "</option>");
                    }
                }
                if (selectedFormDesignVersionId)
                    $(elementIDs.serviceDesignVersionFormDesignVersionIDJQ).val(selectedFormDesignVersionId);
                else
                    $(elementIDs.serviceDesignVersionFormDesignVersionIDJQ).val(0);

                $(elementIDs.serviceDesignVersionFormDesignVersionIDJQ).focus();
            });
            promise.fail(showError);
        }
    }

    function validate() {
        var effectivDate = $(elementIDs.serviceDesignVersionEffectiveDateJQ).val();
        var formDesignId = $(elementIDs.serviceDesignVersionFormDesignIDJQ).val();
        var formDesignVersionId = $(elementIDs.serviceDesignVersionFormDesignVersionIDJQ).val();
        var isValid = false;

        if (effectivDate == '' || effectivDate == undefined || effectivDate == null) {
            isValid = false;
            $(elementIDs.serviceDesignVersionEffectiveDateJQ).parent('div').addClass('has-error');
            $(elementIDs.serviceDesignVersionEffectiveDateHelpBlockJQ).text(ServiceDesign.effectiveDateIsRequiredMsg);
            return isValid;
        }
        else {
            isValid = true;
            $(elementIDs.serviceDesignVersionEffectiveDateJQ).parent('div').removeClass('has-error');
            $(elementIDs.serviceDesignVersionEffectiveDateHelpBlockJQ).text(ServiceDesign.effectiveDateHelpblockMsg);
        }
        if (formDesignId == 0 || formDesignId == '' || formDesignId == null) {
            isValid = false;
            $(elementIDs.serviceDesignVersionFormDesignIDJQ).parent('div').addClass('has-error');
            $(elementIDs.serviceDesignVersionFormDesignIDHelpBlockJQ).text(ServiceDesign.formDesignIDIsRequiredMsg);

            return isValid;
        }
        else {
            isValid = true;
            $(elementIDs.serviceDesignVersionFormDesignIDJQ).parent('div').removeClass('has-error');
            $(elementIDs.serviceDesignVersionFormDesignIDHelpBlockJQ).text(ServiceDesign.formDesignHelpBlockMsg);
        }
        if (formDesignVersionId == 0 || formDesignVersionId == '' || formDesignVersionId == null) {
            isValid = false;
            $(elementIDs.serviceDesignVersionFormDesignVersionIDJQ).parent('div').addClass('has-error');
            $(elementIDs.serviceDesignVersionFormDesignVersionIDHelpBlockJQ).text(ServiceDesign.formDesignVersionIDIsRequiredMsg);

            return isValid;
        }
        else {
            isValid = true;
            $(elementIDs.serviceDesignVersionFormDesignVersionIDJQ).parent('div').removeClass('has-error');
            $(elementIDs.serviceDesignVersionFormDesignVersionIDHelpBlockJQ).text(ServiceDesign.formDesignVersionHelpBlockMsg);
        }

        //get Form Version List
        var serviceDesignList = $(elementIDs.serviceDesignVersionGridJQ).getRowData();
        var newEffectiveDate = $(elementIDs.serviceDesignVersionEffectiveDateJQ).val();

        var checkEffectiveDate = serviceDesignList.length > 0 && (new Date(newEffectiveDate) < new Date(getMinimumEffectiveDate(serviceDesignList)));

        // validation for Effective Date field
        if (newEffectiveDate == '') {
            $(elementIDs.serviceDesignVersionEffectiveDateJQ).parent('div').addClass('has-error');
            $(elementIDs.serviceDesignVersionEffectiveDateHelpBlockJQ).text(Common.effectiveDateRequiredMsg);
            isValid = false;
            return isValid;
        }
        else if (checkEffectiveDate === true && serviceDesignList.length > 1) {
            $(elementIDs.serviceDesignVersionEffectiveDateJQ).parent('div').addClass('has-error');
            $(elementIDs.serviceDesignVersionEffectiveDateHelpBlockJQ).text(ServiceDesign.effectiveDateGreaterThanVersionMsg);
            isValid = false;
            return isValid;
        }
        else {
            var effectiveDateMessage = isValidEffectiveDate(newEffectiveDate);
            if (effectiveDateMessage == "") {
                isValid = true;
                $(elementIDs.serviceDesignVersionEffectiveDateJQ).parent('div').removeClass('has-error');
                $(elementIDs.serviceDesignVersionEffectiveDateHelpBlockJQ).text(ServiceDesign.effectiveDateHelpblockMsg);
            }
            else {
                $(elementIDs.serviceDesignVersionEffectiveDateJQ).parent('div').addClass('has-error');
                $(elementIDs.serviceDesignVersionEffectiveDateHelpBlockJQ).text(effectiveDateMessage);
                isValid = false;
                return isValid;
            }
        }

        return isValid;
    }
    return {
        //open the dialog

        show: function (row, action, serviceDesignId) {
            serviceDesignVersionDialogState = action;
            currentServiceDesignId = serviceDesignId;

            //set based elements for add or edit based on the mode the dialog is opened in
            $(elementIDs.serviceDesignVersionDialog + ' div').removeClass('has-error');

            if (serviceDesignVersionDialogState == "add") {
                $(elementIDs.serviceDesignVersionDialog).dialog('option', 'title', ServiceDesign.addVersionDialogTitle);
                $(elementIDs.serviceDesignVersionFormDesignIDJQ).removeAttr('disabled');

                $(elementIDs.serviceDesignVersionFormDesignIDJQ).val(0);
                $(elementIDs.serviceDesignVersionFormDesignVersionIDJQ).val(0);
                if (row) {
                    $(elementIDs.serviceDesignVersionFormDesignIDJQ).attr('disabled', 'disabled');

                    $(elementIDs.serviceDesignVersionFormDesignIDJQ).val(row.FormDesignID);
                    loadFormDesignVersion(row.FormDesignID, row.FormDesignVersionID);
                }

                //open dialog - uses jqueryui dialog
                $(elementIDs.serviceDesignVersionDialog).dialog("open");
            }
            else {
                $(elementIDs.serviceDesignVersionDialog).dialog('option', 'title', ServiceDesign.editVersionDialogTitle);
                $(elementIDs.serviceDesignVersionFormDesignIDJQ).attr('disabled', 'disabled');

                //open dialog - uses jqueryui dialog
                $(elementIDs.serviceDesignVersionDialog).dialog("open");

                $(elementIDs.serviceDesignVersionEffectiveDateJQ).val(row.EffectiveDate);
                $(elementIDs.serviceDesignVersionFormDesignIDJQ).val(row.FormDesignID);
                loadFormDesignVersion(row.FormDesignID, row.FormDesignVersionID);
                currentFormDesignId = row.FormDesignID;
                currentServiceDesignVersionId = row.ServiceDesignVersionId;
            }
        }
    }
}(); //invoked soon after loading
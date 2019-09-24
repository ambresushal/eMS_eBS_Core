var masterWorkflowSettings = function () {
    tenantId = 1;

    var URLs = {
        getWorkFlowStateMasterList: "/WorkFlow/GetWorkFlowStateMasterList?tenantID=1",
        getWorkFlowStateApprovalTypeMasterList: "/WorkFlow/GetWorkFlowStateApprovalTypeMasterList?tenantID=1",
        deleteWorkflowStateMaster: "/WorkFlow/DeleteWorkFlowStateMaster",
        deleteWorkflowStateApprovalTypeMaster: "/WorkFlow/DeleteWorkFlowStateApprovalTypeMaster"
    };

    var elementIDs = {
        settingsTabJQ: "#settingsTab",
        workFlowStateMasterGrid: "workFlowStateMasterGrid",
        workFlowStateMasterGridJQ: "#workFlowStateMasterGrid",
        workFlowStateApprovalTypeMasterGrid: "workFlowStateApprovalTypeMasterGrid",
        workFlowStateApprovalTypeMasterGridJQ: "#workFlowStateApprovalTypeMasterGrid"
    };
    //this function is called below soon after this JS file is loaded
    function init() {
        $(document).ready(function () {
            registerEvents();
        });
    }

    function loadMasterWorkflowStateGrid() {

        var colArray = ['', 'State'];
        //set column models
        var colModel = [];
        colModel.push({ key: true, name: 'WFStateID', index: 'WFStateID', editable: true, hidden: true });
        colModel.push({ name: 'WFStateName', index: 'WFStateName', width: '990px' });

        //adding the pager element
        $(elementIDs.workFlowStateMasterGridJQ).parent().append("<div id='p" + elementIDs.workFlowStateMasterGrid + "'></div>");


        //clean up the grid first - only table element remains after this
        $(elementIDs.workFlowStateMasterGridJQ).jqGrid('GridUnload');
        $(elementIDs.workFlowStateMasterGridJQ).jqGrid({
            datatype: 'json',
            url: URLs.getWorkFlowStateMasterList,
            cache: false,
            altclass: 'alternate',
            colNames: colArray,
            colModel: colModel,
            caption: '',
            height: '200',
            rowNum: 10,
            loadonce: true,
            //autowidth: true,
            viewrecords: true,
            hidegrid: false,
            hiddengrid: false,
            ignoreCase: true,
            caption: "WorkFlow States",
            sortname: 'WFStateID',
            pager: '#p' + elementIDs.workFlowStateMasterGrid,
            editurl: 'clientArray',
            altRows: true,
            loadComplete: function () {
                //if (roleId == Role.HNESuperUser) {
                //    $("#btnworkFlowStateMasterEdit").addClass('disabled-button');
                //    $("#btnworkFlowStateMasterAdd").addClass('disabled-button');
                //    $("#btnworkFlowStateMasterDelete").addClass('disabled-button');
                //}
                return true;
            }
        });

        var pagerElement = '#p' + elementIDs.workFlowStateMasterGrid;
        $(pagerElement).find('input').css('height', '20px');

        //remove default buttons
        $(elementIDs.workFlowStateMasterGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

        $(elementIDs.workFlowStateMasterGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

        //edit button in footer of grid 
        $(elementIDs.workFlowStateMasterGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-pencil',
            title: 'Edit', id: 'btnworkFlowStateMasterEdit',
            onClickButton: function () {

                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    var row = $(elementIDs.workFlowStateMasterGridJQ).getRowData(rowId);
                    //load Add Category Workflow dialog for edit on click - see categoryWorkflowDesignDialog function below
                    masterWorkflowDesignDialog.show('WFState', row.WFStateName, 'edit');
                } else {
                    messageDialog.show('Please select a row.');
                }
            }
        });

        //add button in footer of grid
        $(elementIDs.workFlowStateMasterGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon-plus',
                title: 'Add', id: 'btnworkFlowStateMasterAdd',
                onClickButton: function () {
                    //load Edit Category Workflow dialog on click - see categoryWorkflowDesignDialog function below
                    masterWorkflowDesignDialog.show('WFState', '', 'add');
                }
            });

        //delete button in footer of grid
        $(elementIDs.workFlowStateMasterGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-trash',
            title: 'Delete', id: 'btnworkFlowStateMasterDelete',
            onClickButton: function () {
                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    //get the categoryWorkflow List for current mapping
                    //var categoryWorkflowList = $(elementIDs.workFlowStateMasterGridJQ).getRowData();

                    //load confirm dialogue to asset the operation
                    confirmDialog.show(Common.deleteConfirmationMsg, function () {
                        confirmDialog.hide();

                        //delete the form design
                        var workflowStateDelete = {
                            wFStateID: rowId
                        };
                        var promise = ajaxWrapper.postJSON(URLs.deleteWorkflowStateMaster, workflowStateDelete);
                        //register ajax success callback
                        promise.done(masterWorkflowStateDeleteSuccess);
                        //register ajax failure callback
                        promise.fail(showError);
                    });
                }
                else {
                    messageDialog.show('Please select a row.');
                }
            }
        });
    }

    //ajax callback success - reload Form Desing  grid
    function masterWorkflowStateDeleteSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(WorkFlowSettingsMessages.wfMasterStateDeleteSuccess);
        }
        else {
            if (xhr.Items[0].Messages.length > 0) messageDialog.show(xhr.Items[0].Messages[0]);
            else messageDialog.show(Common.errorMsg);
        }
        loadMasterWorkflowStateGrid();
    }

    function loadWorkFlowStateApprovalTypeMasterGrid() {

        var colArray = ['', 'Update Option'];
        //set column models
        var colModel = [];
        colModel.push({ key: true, name: 'WorkFlowStateApprovalTypeID', index: 'WorkFlowStateApprovalTypeID', editable: true, hidden: true });
        colModel.push({ name: 'WorkFlowStateApprovalTypeName', index: 'WorkFlowStateApprovalTypeName', width: '990px' });

        //adding the pager element
        $(elementIDs.workFlowStateApprovalTypeMasterGridJQ).parent().append("<div id='p" + elementIDs.workFlowStateApprovalTypeMasterGrid + "'></div>");


        //clean up the grid first - only table element remains after this
        $(elementIDs.workFlowStateApprovalTypeMasterGridJQ).jqGrid('GridUnload');
        $(elementIDs.workFlowStateApprovalTypeMasterGridJQ).jqGrid({
            datatype: 'json',
            url: URLs.getWorkFlowStateApprovalTypeMasterList,
            cache: false,
            altclass: 'alternate',
            colNames: colArray,
            colModel: colModel,
            caption: '',
            height: '200',
            rowNum: 10,
            loadonce: true,
            //autowidth: true,
            viewrecords: true,
            hidegrid: false,
            hiddengrid: false,
            ignoreCase: true,
            caption: "Status Update Option",
            sortname: 'WFStateID',
            pager: '#p' + elementIDs.workFlowStateApprovalTypeMasterGrid,
            editurl: 'clientArray',
            altRows: true,
            loadComplete: function () {
                //if (roleId == Role.HNESuperUser) {
                //    $("#btnworkFlowStateATMasterEdit").addClass('disabled-button');
                //    $("#btnworkFlowStateATMasterAdd").addClass('disabled-button');
                //    $("#btnworkFlowStateATMasterDelete").addClass('disabled-button');
                //}
                return true;
            }
        });

        var pagerElement = '#p' + elementIDs.workFlowStateApprovalTypeMasterGrid;
        $(pagerElement).find('input').css('height', '20px');

        //remove default buttons
        $(elementIDs.workFlowStateApprovalTypeMasterGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

        $(elementIDs.workFlowStateApprovalTypeMasterGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

        //edit button in footer of grid 
        $(elementIDs.workFlowStateApprovalTypeMasterGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-pencil',
            title: 'Edit', id: 'btnworkFlowStateATMasterEdit',
            onClickButton: function () {

                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    var row = $(elementIDs.workFlowStateApprovalTypeMasterGridJQ).getRowData(rowId);
                    //load Add Category Workflow dialog for edit on click - see categoryWorkflowDesignDialog function below
                    masterWorkflowDesignDialog.show('WFApprovalType', row.WorkFlowStateApprovalTypeName, 'edit');
                }
                else {
                    messageDialog.show('Please select a row.');
                }
            }
        });

        //add button in footer of grid
        $(elementIDs.workFlowStateApprovalTypeMasterGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon-plus',
                title: 'Add', id: 'btnworkFlowStateATMasterAdd',
                onClickButton: function () {
                    //load Edit Category Workflow dialog on click - see categoryWorkflowDesignDialog function below
                    masterWorkflowDesignDialog.show('WFApprovalType', '', 'add');
                }
            });

        //delete button in footer of grid
        $(elementIDs.workFlowStateApprovalTypeMasterGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-trash',
            title: 'Delete', id: 'btnworkFlowStateATMasterDelete',
            onClickButton: function () {
                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    //load confirm dialogue to asset the operation
                    confirmDialog.show(Common.deleteConfirmationMsg, function () {
                        confirmDialog.hide();

                        //delete the form design
                        var workflowStateApprovalTypeDelete = {
                            workFlowStateApprovalTypeID: rowId
                        };
                        var promise = ajaxWrapper.postJSON(URLs.deleteWorkflowStateApprovalTypeMaster, workflowStateApprovalTypeDelete);
                        //register ajax success callback
                        promise.done(workFlowStateApprovalTypeMasterDeleteSuccess);
                        //register ajax failure callback
                        promise.fail(showError);
                    });
                }
                else {
                    messageDialog.show('Please select a row.');
                }
            }
        });
    }

    //ajax callback success - reload Form Desing  grid
    function workFlowStateApprovalTypeMasterDeleteSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(WorkFlowSettingsMessages.wfMasterAPPTypeDeleteSuccess);
        }
        else {
            if (xhr.Items[0].Messages.length > 0) messageDialog.show(xhr.Items[0].Messages[0]);
            else messageDialog.show(Common.errorMsg);
        }
        loadWorkFlowStateApprovalTypeMasterGrid()
    }

    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    init();

    //register all the events associated to the view
    function registerEvents() {
        //load grid on the tab switch event
        $(".WFCATCONFIGMASTER").unbind('click');
        $(".WFCATCONFIGMASTER").click(function () {
            loadMasterWorkflowStateGrid();
            loadWorkFlowStateApprovalTypeMasterGrid();
            //show the selected tab
            $(elementIDs.settingsTabJQ).tabs({
                selected: 2
            });
        });
    }

    return {
        loadMasterWorkflowState: function () {
            loadMasterWorkflowStateGrid();
        },
        loadWorkFlowStateApprovalTypeMaster: function () {
            loadWorkFlowStateApprovalTypeMasterGrid();
        }
    }
}();

//contains functionality for the Document Design add/edit dialog
var masterWorkflowDesignDialog = function () {
    var URLs = {
        masterWFStateAdd: '/WorkFlow/AddWorkFlowStateMaster',
        masterWFStateATypeAdd: '/WorkFlow/AddWorkFlowStateApprovalTypeMaster',
        masterWFStateUpdate: '/WorkFlow/UpdateWorkFlowStateMaster',
        masterWFStateATypeUpdate: '/WorkFlow/UpdateWorkFlowStateApprovalTypeMaster',
    }

    //see element id's in Views\FormDesign\Index.cshtml
    var elementIDs = {
        //form design dialog element
        masterWorkflowDesignDialog: "#masterWorkflowDesignDialog",
        masterWorkflowDataText: "#masterWorkflowData",
        workFlowStateMasterGridJQ: "#workFlowStateMasterGrid",
        workFlowStateApprovalTypeMasterGridJQ: "#workFlowStateApprovalTypeMasterGrid"
    };

    //maintains dialog state - add or edit
    var masterWorkflowDesignDialogState, masterWorkflowDesignDataName;

    //ajax success callback - for add/edit
    function masterWorkflowDesignSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            var msg = (masterWorkflowDesignDataName === "WFState") ? WorkFlowSettingsMessages.wfMasterStateUpdateSuccess : WorkFlowSettingsMessages.wfMasterAPPTypeUpdateSuccess;
            messageDialog.show(msg);
        }
        else {
            if (xhr.Items[0].Messages.length > 0) messageDialog.show(xhr.Items[0].Messages[0]);
            else messageDialog.show(Common.errorMsg);
        }
        //reload form design grid 
        if (masterWorkflowDesignDataName === "WFState")
            masterWorkflowSettings.loadMasterWorkflowState();
        else
            masterWorkflowSettings.loadWorkFlowStateApprovalTypeMaster();
        //reset dialog elements
        $(elementIDs.masterWorkflowDesignDialog + ' div').removeClass('has-error');
        $(elementIDs.masterWorkflowDesignDialog + ' .help-block').text('');
        $(elementIDs.masterWorkflowDesignDialog).dialog('close');
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
        $(elementIDs.masterWorkflowDesignDialog).dialog({
            autoOpen: false,
            height: 250,
            width: 450,
            modal: true
        });
        //register event for Add/Edit button click on dialog
        $(elementIDs.masterWorkflowDesignDialog + ' button').on('click', function () {
            //check if name is already used
            var newName = $(elementIDs.masterWorkflowDesignDialog + ' input').val(), filterList;

            var grid = (masterWorkflowDesignDataName === "WFState") ? $(elementIDs.workFlowStateMasterGridJQ) : $(elementIDs.workFlowStateApprovalTypeMasterGridJQ);
            var validationMsgRequired = (masterWorkflowDesignDataName === "WFState") ? WorkFlowSettingsMessages.wfMasterStateRequiredError : WorkFlowSettingsMessages.wfMasterAPPTypeRequiredError
            var validationMsgExists = (masterWorkflowDesignDataName === "WFState") ? WorkFlowSettingsMessages.wfMasterStateExistsError : WorkFlowSettingsMessages.wfMasterAPPTypeExistsError

            var dataList = grid.getRowData();
            if (masterWorkflowDesignDataName === "WFState") {
                filterList = dataList.filter(function (ct) {
                    return compareStrings(ct.WFStateName, newName, true);
                });
            } else {
                filterList = dataList.filter(function (ct) {
                    return compareStrings(ct.WorkFlowStateApprovalTypeName, newName, true);
                });
            }

            //validate form name
            if (filterList !== undefined && filterList.length > 0) {
                $(elementIDs.masterWorkflowDesignDialog + ' div').addClass('has-error');
                $(elementIDs.masterWorkflowDesignDialog + ' .help-block').text(validationMsgExists);
            }
            else if (newName == '') {
                $(elementIDs.masterWorkflowDesignDialog + ' div').addClass('has-error');
                $(elementIDs.masterWorkflowDesignDialog + ' .help-block').text(validationMsgRequired);
            }
            else {
                //save the new form design
                var rowId = grid.getGridParam('selrow');
                var wfStateDesignAdd = {
                    tenantId: 1,
                    wFStateID: rowId,
                    wFStateName: newName,
                    workFlowStateApprovalTypeID: rowId,
                    workFlowStateApprovalTypeName: newName
                };
                var url;
                if (masterWorkflowDesignDialogState == 'add' && masterWorkflowDesignDataName === 'WFState') {
                    url = URLs.masterWFStateAdd;
                }
                else if (masterWorkflowDesignDialogState == 'edit' && masterWorkflowDesignDataName === 'WFState') {
                    url = URLs.masterWFStateUpdate;
                }
                else if (masterWorkflowDesignDialogState == 'add' && masterWorkflowDesignDataName === 'WFApprovalType') {
                    url = URLs.masterWFStateATypeAdd;
                }
                else
                    url = URLs.masterWFStateATypeUpdate;

                //ajax call to add/update
                var promise = ajaxWrapper.postJSON(url, wfStateDesignAdd);
                //register ajax success callback
                promise.done(masterWorkflowDesignSuccess);
                //register ajax failure callback
                promise.fail(showError);
            }
        });
    }
    //initialize the dialog after this js are loaded
    init();
    //these are the properties that can be called by using formDesignDialog.<Property>
    //eg. formDesignDialog.show('name','add');
    return {
        show: function (masterWFDataName, masterWFDataValue, action) {
            masterWorkflowDesignDialogState = action;
            masterWorkflowDesignDataName = masterWFDataName;

            //set eleemnts based on whether the dialog is opened in add or edit mode
            $(elementIDs.masterWorkflowDesignDialog + ' div').removeClass('has-error');

            if (masterWorkflowDesignDialogState == 'add' && masterWFDataName === 'WFState') {
                $(elementIDs.masterWorkflowDesignDialog).dialog('option', 'title', WorkFlowSettingsMessages.wfMasterStateAddTitle);
                $(elementIDs.masterWorkflowDesignDialog + ' .help-block').text(WorkFlowSettingsMessages.wfMasterStateAddHelpText);
                $(elementIDs.masterWorkflowDesignDialog + ' label').text(WorkFlowSettingsMessages.wfMasterStateLabel);
                $(elementIDs.masterWorkflowDataText).val('');
            }
            else if (masterWorkflowDesignDialogState == 'edit' && masterWFDataName === 'WFState') {
                $(elementIDs.masterWorkflowDesignDialog).dialog('option', 'title', WorkFlowSettingsMessages.wfMasterStateUpdateTitle);
                $(elementIDs.masterWorkflowDataText).val(masterWFDataValue);
                $(elementIDs.masterWorkflowDesignDialog + ' .help-block').text(WorkFlowSettingsMessages.wfMasterStateUpdateHelpText);
                $(elementIDs.masterWorkflowDesignDialog + ' label').text(WorkFlowSettingsMessages.wfMasterStateLabel);
            }
            else if (masterWorkflowDesignDialogState == 'add' && masterWFDataName === 'WFApprovalType') {
                $(elementIDs.masterWorkflowDesignDialog).dialog('option', 'title', WorkFlowSettingsMessages.wfMasterAPPTypeAddTitle);
                $(elementIDs.masterWorkflowDesignDialog + ' .help-block').text(WorkFlowSettingsMessages.wfMasterAPPTypeAddHelpText);
                $(elementIDs.masterWorkflowDesignDialog + ' label').text(WorkFlowSettingsMessages.wfMasterAPPTypeLabel);
                $(elementIDs.masterWorkflowDataText).val('');
            }
            else {
                $(elementIDs.masterWorkflowDesignDialog).dialog('option', 'title', WorkFlowSettingsMessages.wfMasterAPPTypeUpdateTitle);
                $(elementIDs.masterWorkflowDataText).val(masterWFDataValue);
                $(elementIDs.masterWorkflowDesignDialog + ' .help-block').text(WorkFlowSettingsMessages.wfMasterAPPTypeUpdateHelpText);
                $(elementIDs.masterWorkflowDesignDialog + ' label').text(WorkFlowSettingsMessages.wfMasterAPPTypeLabel);
            }
            //open the dialog - uses jqueryui dialog
            $(elementIDs.masterWorkflowDesignDialog).dialog("open");
        }
    }
}(); //invoked soon after loading
var wfStateMappingChange = 1, isFinalized = false;
var customWorkflowSettings = function () {
    //isInitialized = false;
    tenantId = 1;

    var URLs = {
        getWorkflowCategoryMappingList: "/WorkFlow/GetWorkFlowCategoryMappingList?tenantID=1",
        deleteWorkflowCategoryMapping: "/WorkFlow/DeleteWorkFlowCategoryMapping",
        getCategoryList: "/Settings/GetFolderVersionCategoryForDropdown?tenantID=1",
        getWorkflowStateMappingList: "/WorkFlow/GetWorkFlowVersionStatesList?workFlowVersionID={workFlowVersionID}",
        getWFStateList: "/WorkFlow/GetWorkFlowStateMasterList?tenantID=1",
        deleteWorkflowStateMapping: "/WorkFlow/DeleteWorkFlowVersionStates?workFlowVersionStatesID={workFlowVersionStatesID}",
        getWFStateAccessMappingList: "/WorkFlow/GetWorkFlowVersionStatesAccessList?workFlowVersionStateID={workFlowVersionStateID}",
        getUserRoleList: "/Settings/GetUserRolesDetails",
        deleteWorkflowStateAccessMapping: "/WorkFlow/DeleteWorkFlowVersionStatesAccess",
        getWFStateApprovalStatusMappingList: "/WorkFlow/GetWFVersionStatesApprovalTypeList?workFlowVersionStateID={workFlowVersionStateID}",
        deleteWFVersionStatesApprovalType: "/WorkFlow/DeleteWFVersionStatesApprovalType",
        getWorkFlowStateApprovalTypeMasterList: "/WorkFlow/GetWorkFlowStateApprovalTypeMasterList?tenantID=1",
        finalizeWorkflowVersion: "/WorkFlow/FinalizeWorkFlowVersion",
        getWFStateApprovalStatusActionsMappingList: "/WorkFlow/GetWFVersionStatesApprovalTypeAction?wFVersionStatesApprovalTypeID={wFVersionStatesApprovalTypeID}"
    };

    var elementIDs = {
        settingsTabJQ: "#settingsTab",
        dopCategoryListJQ: "#categorynamelist",
        categoryNameError: "#categorynameError",
        workflowCategoryMappingGrid: "workflowCategoryMappingGrid",
        workflowCategoryMappingGridJQ: "#workflowCategoryMappingGrid",
        workflowStateMappingGrid: "workflowStateMappingGrid",
        workflowStateMappingGridJQ: "#workflowStateMappingGrid",
        workflowStateMappingDiv: "#workflowStateMappingDiv",
        btnAddWorkflowStates: "#addWorkflowStates",
        workflowStatesAdddialog: "#workflowStatesAdddialog",
        dopWfStatesList: "#wfstatenamelist",
        dopWfStateSequencelist: "#wfstatesequencelist",
        workflowStateAccessMappingGrid: "workflowStateAccessMappingGrid",
        workflowStateAccessMappingGridJQ: "#workflowStateAccessMappingGrid",
        workflowStateAccessMappingDiv: "#workflowStateAccessMappingDiv",
        dopWfUserRoleList: "#userRolelist",
        workflowStateApprovalStatusDiv: "#workflowStateApprovalStatusDiv",
        wfStateApprovalStatusGrid: "wfStateApprovalStatusGrid",
        wfStateApprovalStatusGridJQ: "#wfStateApprovalStatusGrid",
        dopWFApprovalTypeList: "#wfApprovalTypeList",
        wfStateApprovalStatusActionsGrid: "wfStateApprovalStatusActionsGrid",
        wfStateApprovalStatusActionsGridJQ: "#wfStateApprovalStatusActionsGrid",
        wfStateApprovalStatusActionsDiv: "#workflowStateApprovalStatusActionsDiv",
        actionStatesList: "#actionStatesList",
    };

    //this function is called below soon after this JS file is loaded
    function init() {
        $(document).ready(function () {
            registerEvents();
        });
    }

    //-----------------------------------------------Workflow Category Mapping-----------------------------------------------//

    function loadCategoryList() {
        var currentInstance = this;
        var promise = ajaxWrapper.getJSON(URLs.getCategoryList);

        //register ajax success callback
        promise.done(function (items) {
            if (items) {
                $(elementIDs.dopCategoryListJQ).empty();
                $.each(items, function (i, item) {
                    $(elementIDs.dopCategoryListJQ).append(
                        "<option value=" + item.FolderVersionCategoryID + ">" + item.FolderVersionCategoryName + "</option>"
                    );
                });
            }
        });

        //register ajax failure callback
        promise.fail(showError);
        //registerButtonEvents();
    }

    function loadWorkflowCategoryMappingGrid() {
        var colArray = ['', '', '', 'Category Name', 'Account Type', 'WorkFlow Type', '', 'States', 'Order', 'Finalize'];// 'Effective Date', 'Finalized', 'States', 'Order', ''];
        //set column models
        var colModel = [];
        colModel.push({ key: true, name: 'WorkFlowVersionID', index: 'WorkFlowVersionID', editable: true, hidden: true });
        colModel.push({ name: 'IsActive', index: 'IsActive', hidden: true });
        colModel.push({ name: 'FolderVersionCategoryID', index: 'FolderVersionCategoryID', editable: true, hidden: true });
        colModel.push({ name: 'CategoryName', index: 'CategoryName', editable: true, width: '233px' });
        colModel.push({ name: 'AccountTypeName', index: 'AccountTypeName', editable: true, width: '233px' });
        colModel.push({ name: 'WorkFlowtypeName', index: 'WorkFlowtypeName', editable: true, width: '233px' });
        //colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: true, width: '150px' });
        colModel.push({ name: 'IsFinalized', index: 'IsFinalized', editable: true, search: false, align: 'center', formatter: 'checkbox', edittype: 'checkbox', editoptions: { value: 'true : false' }, stype: 'select', width: '98px', hidden: true });
        colModel.push({ name: 'WorkflowStates', index: 'WorkflowStates', editable: true, formatter: expandCollapseFmatterWFStates, width: '98px', search: false });
        colModel.push({ name: 'WorkflowOrder', index: 'WorkflowOrder', editable: true, formatter: expandCollapseFmatterWFOrder, width: '98px', search: false });
        colModel.push({ name: 'Finalize', index: 'Finalize', editable: true, formatter: finalizeWorkFlowFmatter, width: '98px', search: false, sortable: false });
        //adding the pager element
        $(elementIDs.workflowCategoryMappingGridJQ).parent().append("<div id='p" + elementIDs.workflowCategoryMappingGrid + "'></div>");


        //clean up the grid first - only table element remains after this
        $(elementIDs.workflowCategoryMappingGridJQ).jqGrid('GridUnload');
        $(elementIDs.workflowCategoryMappingGridJQ).jqGrid({
            datatype: 'json',
            url: URLs.getWorkflowCategoryMappingList,
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
            caption: "Category WorkFlow Mapping",
            sortname: 'WorkFlowVersionID',
            pager: '#p' + elementIDs.workflowCategoryMappingGrid,
            editurl: 'clientArray',
            loadComplete: function () {
                //if (roleId == Role.HNESuperUser) {
                //    $("#btnCategoryListWSEdit").addClass('disabled-button');
                //    $("#btnCategoryListWSAdd").addClass('disabled-button');
                //    $("#btnCategoryListWSDelete").addClass('disabled-button');
                //    $("#btnCategoryListWSCopy").addClass('disabled-button');
                //}
                registerEvents();
            },
            altRows: true
        });

        var pagerElement = '#p' + elementIDs.workflowCategoryMappingGrid;
        $(pagerElement).find('input').css('height', '20px');

        //remove default buttons
        $(elementIDs.workflowCategoryMappingGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

        $(elementIDs.workflowCategoryMappingGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

        //edit button in footer of grid 
        $(elementIDs.workflowCategoryMappingGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-pencil',
            title: 'Edit', id: 'btnCategoryListWSEdit',
            onClickButton: function () {

                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    var row = $(elementIDs.workflowCategoryMappingGridJQ).getRowData(rowId);
                    //load Add Category Workflow dialog for edit on click - see categoryWorkflowDesignDialog function below
                    wFcategoryDesignDialog.show(row.FolderVersionCategoryID, row.AccountTypeName, row.WorkFlowtypeName, 'edit');
                }
                else {
                    messageDialog.show('Please select a row.');
                }
            }
        });

        //add button in footer of grid
        $(elementIDs.workflowCategoryMappingGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon-plus',
                title: 'Add', id: 'btnCategoryListWSAdd',
                onClickButton: function () {
                    //load Edit Category Workflow dialog on click - see categoryWorkflowDesignDialog function below
                    wFcategoryDesignDialog.show('', '', '', 'add');
                }
            });

        //delete button in footer of grid
        $(elementIDs.workflowCategoryMappingGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-trash',
            title: 'Delete', id: 'btnCategoryListWSDelete',
            onClickButton: function () {
                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    var rowData = $(elementIDs.workflowCategoryMappingGridJQ).getRowData(rowId);
                    if (rowData.IsFinalized.trim() == "false") {
                        //load confirm dialogue to asset the operation
                        confirmDialog.show(Common.deleteConfirmationMsg, function () {
                            confirmDialog.hide();
                            //validate category as 'IsFinalized before deleting.'
                            //delete the workflow Category mapping
                            var workflowCategoryDelete = {
                                WorkFlowVersionID: rowId,
                                tenantID: 1
                            };
                            var promise = ajaxWrapper.postJSON(URLs.deleteWorkflowCategoryMapping, workflowCategoryDelete);
                            //register ajax success callback
                            promise.done(wFCategoryMappingDeleteSuccess);
                            //register ajax failure callback
                            promise.fail(showError);
                        });
                    }
                    else
                        messageDialog.show(WorkFlowSettingsMessages.wfCategoryMappingDeleteError);
                }
                else {
                    messageDialog.show('Please select a row.');
                }
            }
        });

        //copy workflow button in footer of grid
        $(elementIDs.workflowCategoryMappingGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-copy',
            title: 'Copy Workflow', id: 'btnCategoryListWSCopy',
            onClickButton: function () {
                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    //Category Workflow dialog for copy on click - see categoryWorkflowDesignDialog function below
                    wFcategoryDesignDialog.show('', '', '', 'copy');
                }
                else {
                    messageDialog.show('Please select a row.');
                }
            }
        });
    }

    //ajax callback success - reload Workflow Category Mapping Grid
    function wFCategoryMappingDeleteSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(WorkFlowSettingsMessages.wfCatDeleteSuccess);

            //hide all other divs on category mapping delete
            $(elementIDs.workflowStateMappingDiv).hide();
            $(elementIDs.workflowStateAccessMappingDiv).hide();
            $(elementIDs.workflowStateApprovalStatusDiv).hide();
            $(elementIDs.wfStateApprovalStatusActionsDiv).hide();
        }
        else {
            if (xhr.Items[0].Messages.length > 0) messageDialog.show(xhr.Items[0].Messages[0]);
            else messageDialog.show(Common.errorMsg);
        }
        loadWorkflowCategoryMappingGrid();
    }

    var expandCollapseFmatterWFOrder = function (cellvalue, options, rowObject) {
        if (!$.isNumeric(options.rowId)) {
            return '';
        }
        var toolTip = 'ViewStateOrder';
        var link = "<a href='#' class='WFORDER' data-Id='" + rowObject.WorkFlowVersionID + "'><span align='center' class='ui-icon ui-icon-arrowreturnthick-1-s' style='margin:auto' title=" + toolTip.trim() + " ></span></a>";
        return link;
    }

    var expandCollapseFmatterWFStates = function (cellvalue, options, rowObject) {
        if (!$.isNumeric(options.rowId)) {
            return '';
        }
        var toolTip = 'ViewStates';
        var link = "<a href='#' class='WFSTATE' data-Id='" + rowObject.WorkFlowVersionID + "'><span align='center' class='ui-icon ui-icon-arrowreturnthick-1-s' style='margin:auto'title=" + toolTip.trim() + " ></span></a>";
        return link;
    }

    var finalizeWorkFlowFmatter = function (cellvalue, options, rowObject) {
        if (!$.isNumeric(options.rowId)) {
            return '';
        }
        var toolTip = 'FinalizeWorkFlow', link = '';
        if (!rowObject.IsFinalized)
            link = "<a href='#' class='WFFINALIZE' data-Id='" + rowObject.WorkFlowVersionID + "'><span align='center' class='ui-icon ui-icon-unlocked' style='margin:auto' title=" + toolTip + " ></span></a>";
        else {
            toolTip = 'WorkFlowFinalized'
            link = "<a href='#' class='WFFINALIZEDONE' data-Id='" + rowObject.WorkFlowVersionID + "'><span align='center' class='ui-icon ui-icon-locked' style='margin:auto' title=" + toolTip + " ></span></a>";
        }

        return link;
    }

    //-----------------------------------------------Workflow Category Mapping-----------------------------------------------//

    //-----------------------------------------------Workflow States Mapping-------------------------------------------------//

    function loadWorkFlowStatesDetails(workFlowVersionID) {
        $(elementIDs.workflowStateMappingDiv).show();
        $(elementIDs.workflowStateAccessMappingDiv).hide();
        $(elementIDs.workflowStateApprovalStatusDiv).hide();
        $(elementIDs.wfStateApprovalStatusActionsDiv).hide();
        loadWorkFlowStateList();
        wfStateMappingChange = 1;
        loadWorflowStateMappingGrid(workFlowVersionID, wfStateMappingChange);
    }

    function loadWorkFlowStateList() {
        var currentInstance = this;
        var promise = ajaxWrapper.getJSON(URLs.getWFStateList);

        //register ajax success callback
        promise.done(function (items) {
            if (items) {
                $(elementIDs.dopWfStatesList).empty();
                $.each(items, function (i, item) {
                    $(elementIDs.dopWfStatesList).append("<option value=" + item.WFStateID + ">" + item.WFStateName + "</option>");
                });
            }
        });

        //register ajax failure callback
        promise.fail(showError);
        //registerButtonEvents();
    }

    function loadWorflowStateMappingGrid(workFlowVersionID, wfStateMappingChangeID) {

        var getWorkflowStateMapping = URLs.getWorkflowStateMappingList.replace(/\{workFlowVersionID\}/g, workFlowVersionID);

        var colArray = ['', '', '', 'State', 'Sequence', 'Editable By', 'Approval Types with Action '];
        //set column models
        var colModel = [];
        colModel.push({ key: true, name: 'WorkFlowVersionStateID', index: 'WorkFlowVersionStateID', editable: true, hidden: true });
        colModel.push({ name: 'IsActive', index: 'IsActive', hidden: true });
        colModel.push({ name: 'WFStateID', index: 'WFStateID', editable: true, hidden: true });
        colModel.push({ name: 'WFStateName', index: 'WFStateName', editable: true, width: '450px' });
        colModel.push({ name: 'Sequence', index: 'Sequence', editable: true, width: '540px', hidden: true });
        colModel.push({ name: 'WorkFlowStateAccess', index: 'WorkFlowStateAccess', editable: true, formatter: expandCollapseFmatterWFStateAccess, width: '270px', hidden: true, search: false });
        colModel.push({ name: 'WorkFlowStateAction', index: 'WorkFlowStateAction', editable: true, formatter: expandCollapseFmatterWFApplicableStatus, width: '270px', hidden: true, search: false });

        //adding the pager element
        $(elementIDs.workflowStateMappingGridJQ).parent().append("<div id='p" + elementIDs.workflowStateMappingGrid + "'></div>");

        //clean up the grid first - only table element remains after this
        $(elementIDs.workflowStateMappingGridJQ).jqGrid('GridUnload');
        $(elementIDs.workflowStateMappingGridJQ).jqGrid({
            datatype: 'json',
            url: getWorkflowStateMapping,
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
            caption: "WorkFlow State Mapping",
            sortname: 'WorkFlowVersionStatesID',
            pager: '#p' + elementIDs.workflowStateMappingGrid,
            editurl: 'clientArray',
            loadComplete: function () {
                //Change Nav gird setting on basis of add/Edit state or edit state order/sequence.
                loadWFStateOrderNavGrid(wfStateMappingChangeID);
                //set grid caption
                var data = 'WorkFlow State', caption = 'States for';
                caption = (wfStateMappingChangeID == 1) ? 'States for' : 'State Order for';
                var rowID = $(elementIDs.workflowCategoryMappingGridJQ).getGridParam('selrow');
                if (rowID !== undefined && rowID !== null)
                    var row = $(elementIDs.workflowCategoryMappingGridJQ).getRowData(rowID);
                if (row.length != 0)
                    data = " " + row.AccountTypeName + " - " + row.CategoryName;
                $(elementIDs.workflowStateMappingGridJQ).jqGrid('setCaption', caption + data.bold());

                //function to scroll vertically to gird position
                goToByScroll(elementIDs.workflowStateMappingDiv);

                registerEvents();
                //if (roleId == Role.HNESuperUser) {
                //    $("#btnWFStatesMappingAdd").addClass('disabled-button');
                //    $("#btnWFStatesMappingEdit").addClass('disabled-button');
                //    $("#btnWFStatesMappingDelete").addClass('disabled-button');
                //}
            },
            gridComplete: function () {
                //Hide add/edit/delete button from pager when Category mapping is Finalized.
                var rowID = $(elementIDs.workflowCategoryMappingGridJQ).getGridParam('selrow');
                var row = $(elementIDs.workflowCategoryMappingGridJQ).getRowData(rowID);
                isFinalized = (row.IsFinalized == " false") ? false : true;
                if (isFinalized)
                    $('#p' + elementIDs.workflowStateMappingGrid + '_left').find('table').hide();//sprint5-Bug71
            },
            altRows: true
        });

        var pagerElement = '#p' + elementIDs.workflowStateMappingGrid;
        $(pagerElement).find('input').css('height', '20px');

        //remove default buttons
        $(elementIDs.workflowStateMappingGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

        $(elementIDs.workflowStateMappingGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

        //add button in footer of grid
        $(elementIDs.workflowStateMappingGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon-plus',
                title: 'Add', id: 'btnWFStatesMappingAdd',
                onClickButton: function () {
                    //load Edit Category Workflow dialog on click - see categoryWorkflowDesignDialog function below
                    wFStateDesignDialog.show('', 'add');
                }
            });

        //edit button in footer of grid
        $(elementIDs.workflowStateMappingGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon-pencil',
                title: 'Add', id: 'btnWFStatesMappingEdit',
                onClickButton: function () {
                    var rowId = $(this).getGridParam('selrow');
                    if (rowId !== undefined && rowId !== null) {
                        var row = $(elementIDs.workflowStateMappingGridJQ).getRowData(rowId);
                        wFStateDesignDialog.show(row.WFStateID, 'edit');
                    }
                    else {
                        messageDialog.show('Please select a row.');
                    }
                }
            });

        //delete button in footer of grid
        $(elementIDs.workflowStateMappingGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-trash',
            title: 'Delete', id: 'btnWFStatesMappingDelete',
            onClickButton: function () {
                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {

                    //load confirm dialogue to asset the operation
                    confirmDialog.show(Common.deleteConfirmationMsg, function () {
                        confirmDialog.hide();

                        //delete the form design
                        var workflowStateDelete = {
                            workFlowVersionStatesID: rowId
                        };
                        var promise = ajaxWrapper.postJSON(URLs.deleteWorkflowStateMapping, workflowStateDelete);
                        //register ajax success callback
                        promise.done(worflowStateMappingDeleteSuccess);
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

    function loadWFStateOrderNavGrid(wfStateMappingChangeID) {
        if (wfStateMappingChangeID != 1) {
            //show/hide columns in order view
            $(elementIDs.workflowStateMappingGridJQ).hideCol("WorkFlowStateAccess");
            $(elementIDs.workflowStateMappingGridJQ).hideCol("WorkFlowStateAction");
            $(elementIDs.workflowStateMappingGridJQ).showCol("Sequence");

            //show/hide pager button in order view
            $("#btnWFStatesMappingAdd").hide();
            $("#btnWFStatesMappingDelete").hide();
            $("#btnWFStatesMappingEdit").show();
        }
        else {
            //show/hide columns in order view
            $(elementIDs.workflowStateMappingGridJQ).showCol("WorkFlowStateAccess");
            $(elementIDs.workflowStateMappingGridJQ).showCol("WorkFlowStateAction");
            $(elementIDs.workflowStateMappingGridJQ).hideCol("Sequence");

            //show/hide pager button in order view
            $("#btnWFStatesMappingAdd").show();
            $("#btnWFStatesMappingDelete").show();
            $("#btnWFStatesMappingEdit").hide();
        }
    }

    //ajax callback success - reload Worflow State Mapping Grid
    function worflowStateMappingDeleteSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(WorkFlowSettingsMessages.wfVersionStateDeleteSuccess);

            //hide other divs on deletion
            $(elementIDs.workflowStateAccessMappingDiv).hide();
            $(elementIDs.workflowStateApprovalStatusDiv).hide();
            $(elementIDs.wfStateApprovalStatusActionsDiv).hide();
        }
        else {
            if (xhr.Items[0].Messages.length > 0) messageDialog.show(xhr.Items[0].Messages[0]);
            else messageDialog.show(Common.errorMsg);
        }
        var workFlowVersionID = $(elementIDs.workflowCategoryMappingGridJQ).getGridParam('selrow');
        loadWorflowStateMappingGrid(workFlowVersionID, wfStateMappingChange)
    }

    var expandCollapseFmatterWFStateAccess = function (cellvalue, options, rowObject) {
        if (!$.isNumeric(options.rowId)) {
            return '';
        }
        var link = "<a href='#' class='WFSTATEACCESS' data-Id='" + rowObject.WorkFlowVersionStateID + "'><span align='center' class='ui-icon ui-icon-arrowreturnthick-1-s' style='margin:auto' title='ViewUserRoles' ></span></a>";
        return link;
    }

    var expandCollapseFmatterWFApplicableStatus = function (cellvalue, options, rowObject) {
        if (!$.isNumeric(options.rowId)) {
            return '';
        }
        var link = "<a href='#' class='WFSTATUS' data-Id='" + rowObject.WorkFlowVersionStateID + "'><span align='center' class='ui-icon ui-icon-arrowreturnthick-1-s' style='margin:auto' title='ViewStatus'></span></a>";
        return link;
    }

    //-----------------------------------------------Workflow States Mapping-------------------------------------------------//

    //-----------------------------------------------Workflow Order Mapping--------------------------------------------------//

    function loadWorkFlowOrderDetails(workFlowVersionID) {
        $(elementIDs.workflowStateMappingDiv).show();
        $(elementIDs.workflowStateAccessMappingDiv).hide();
        $(elementIDs.workflowStateApprovalStatusDiv).hide();
        $(elementIDs.wfStateApprovalStatusActionsDiv).hide();
        wfStateMappingChange = 2;
        loadWorkFlowStateList();
        loadWorflowStateMappingGrid(workFlowVersionID, wfStateMappingChange);
    }

    //-----------------------------------------------Workflow Order Mapping--------------------------------------------------//

    //-----------------------------------------------Workflow States Access Mapping------------------------------------------//

    function loadWFStateAccessMappingDetails(workFlowVersionStateID) {
        $(elementIDs.workflowStateAccessMappingDiv).show();
        $(elementIDs.workflowStateApprovalStatusDiv).hide();
        $(elementIDs.wfStateApprovalStatusActionsDiv).hide();
        populateUserRoleDropDown();
        loadWFStateAccessMappingGrid(workFlowVersionStateID);
    }

    function populateUserRoleDropDown() {
        $(elementIDs.dopWfUserRoleList).empty();
        $(elementIDs.dopWfUserRoleList).append("<option value='0'>" + "Select User Role" + "</option>");

        var promise = ajaxWrapper.getJSON(URLs.getUserRoleList);
        promise.done(function (list) {
            for (i = 0; i < list.length; i++) {
                $(elementIDs.dopWfUserRoleList).append("<option value=" + list[i]['RoleId'] + ">" + list[i]['UserRole'] + "</option>");
            }
        });
        promise.fail(showError);

    }

    function loadWFStateAccessMappingGrid(workFlowVersionStateID) {

        var getWFStateAccessMapping = URLs.getWFStateAccessMappingList.replace(/\{workFlowVersionStateID\}/g, workFlowVersionStateID);
        //var getWFStateAccessMapping = '';

        var colArray = ['', '', '', 'User Role'];//, 'Access', 'Owner'];
        //set column models
        var colModel = [];
        colModel.push({ key: true, name: 'WorkFlowVersionStatesAccessID', index: 'WorkFlowVersionStatesAccessID', editable: true, hidden: true });
        colModel.push({ name: 'WorkFlowVersionStateID', index: 'WorkFlowVersionStateID', hidden: true });
        colModel.push({ name: 'RoleID', index: 'RoleID', editable: true, hidden: true });
        colModel.push({ name: 'RoleName', index: 'RoleName', editable: true, width: '990px' });

        //adding the pager element
        $(elementIDs.workflowStateAccessMappingGridJQ).parent().append("<div id='p" + elementIDs.workflowStateAccessMappingGrid + "'></div>");

        //clean up the grid first - only table element remains after this
        $(elementIDs.workflowStateAccessMappingGridJQ).jqGrid('GridUnload');
        $(elementIDs.workflowStateAccessMappingGridJQ).jqGrid({
            datatype: 'json',
            url: getWFStateAccessMapping,
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
            caption: "WorkFlow State Access Mapping",
            sortname: 'WorkFlowVersionStatesID',
            pager: '#p' + elementIDs.workflowStateAccessMappingGrid,
            editurl: 'clientArray',
            loadComplete: function () {
                //set grid caption
                var data = 'WorkFlow State Access';
                var rowIDW = $(elementIDs.workflowCategoryMappingGridJQ).getGridParam('selrow');
                var rowIDS = $(elementIDs.workflowStateMappingGridJQ).getGridParam('selrow');
                //var caption = 'User Role Access for State: ';
                var caption = 'Editable by User Role(s) for State: '; //EQN - 1816
                if (rowIDS !== undefined && rowIDS !== null)
                    var state = $(elementIDs.workflowStateMappingGridJQ).getRowData(rowIDS);
                if (rowIDS !== undefined && rowIDS !== null)
                    var category = $(elementIDs.workflowCategoryMappingGridJQ).getRowData(rowIDW);
                if (category.length != 0 && state.length != 0) {
                    var categoryName = " " + category.AccountTypeName + " - " + category.CategoryName;
                    data = categoryName + " -> " + state.WFStateName;
                }
                $(elementIDs.workflowStateAccessMappingGridJQ).jqGrid('setCaption', caption + data.bold());

                //function to scroll vertically to gird position
                goToByScroll(elementIDs.workflowStateAccessMappingDiv);
                //if (roleId == Role.HNESuperUser) {
                //    $("#btnWFStateAccessMappingAdd").addClass('disabled-button');
                //    $("#btnWFStateAccessMappingEdit").addClass('disabled-button');
                //    $("#btnWFStateAccessMappingDelete").addClass('disabled-button');
                //}
            },
            gridComplete: function () {
                //hide add/edit/delete buttons if category mapping is Finalized
                if (isFinalized)
                    $('#p' + elementIDs.workflowStateAccessMappingGrid + '_left').find('table').hide();//sprint5-Bug71
            },
            altRows: true
        });

        var pagerElement = '#p' + elementIDs.workflowStateAccessMappingGrid;
        $(pagerElement).find('input').css('height', '20px');

        //remove default buttons
        $(elementIDs.workflowStateAccessMappingGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

        $(elementIDs.workflowStateAccessMappingGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

        //add button in footer of grid
        $(elementIDs.workflowStateAccessMappingGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon-plus',
                title: 'Add', id: 'btnWFStateAccessMappingAdd',
                onClickButton: function () {
                    //load Edit Category Workflow dialog on click - see categoryWorkflowDesignDialog function below
                    wFStateAccessDesignDialog.show('', 'add');
                }
            });

        //edit button in footer of grid
        $(elementIDs.workflowStateAccessMappingGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon-pencil',
                title: 'Edit', id: 'btnWFStateAccessMappingEdit',
                onClickButton: function () {
                    var rowId = $(this).getGridParam('selrow');
                    if (rowId !== undefined && rowId !== null) {
                        var row = $(elementIDs.workflowStateAccessMappingGridJQ).getRowData(rowId);
                        wFStateAccessDesignDialog.show(row.RoleID, 'edit');
                    }
                    else {
                        messageDialog.show('Please select a row.');
                    }
                }
            });

        //delete button in footer of grid
        $(elementIDs.workflowStateAccessMappingGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-trash',
            title: 'Delete', id: 'btnWFStateAccessMappingDelete',
            onClickButton: function () {
                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {

                    //load confirm dialogue to asset the operation
                    confirmDialog.show(Common.deleteConfirmationMsg, function () {
                        confirmDialog.hide();

                        //delete the form design
                        var workflowStateAccessDelete = {
                            workFlowVersionStatesAccessID: rowId
                        };
                        var promise = ajaxWrapper.postJSON(URLs.deleteWorkflowStateAccessMapping, workflowStateAccessDelete);
                        //register ajax success callback
                        promise.done(worflowStateAccessMappingDeleteSuccess);
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

    //ajax callback success - reload WF State Access Mapping Grid
    function worflowStateAccessMappingDeleteSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(WorkFlowSettingsMessages.wfVerStateAccessDeleteSuccess);
        }
        else {
            if (xhr.Items[0].Messages.length > 0) messageDialog.show(xhr.Items[0].Messages[0]);
            else messageDialog.show(Common.errorMsg);
        }
        var workFlowVersionStateID = $(elementIDs.workflowStateMappingGridJQ).getGridParam('selrow');
        loadWFStateAccessMappingGrid(workFlowVersionStateID)
    }

    //-----------------------------------------------Workflow States Access Mapping------------------------------------------//

    //-----------------------------------------------Workflow States Approval Status Mapping---------------------------------//

    function loadWFStateApprovalStatusMappingDetails(workFlowVersionStateID) {
        $(elementIDs.workflowStateAccessMappingDiv).hide();
        $(elementIDs.wfStateApprovalStatusActionsDiv).hide();
        $(elementIDs.workflowStateApprovalStatusDiv).show();
        loadWFStateApprovalTypeList();
        loadWFStateApprovalStatusMappingGrid(workFlowVersionStateID);
    }


    function loadWFStateApprovalTypeList() {
        $(elementIDs.dopWFApprovalTypeList).empty();
        $(elementIDs.dopWFApprovalTypeList).append("<option value='0'>" + "Select Approval Type" + "</option>");

        var promise = ajaxWrapper.getJSON(URLs.getWorkFlowStateApprovalTypeMasterList);
        //register ajax success callback
        promise.done(function (items) {
            if (items) {
                $.each(items, function (i, item) {
                    $(elementIDs.dopWFApprovalTypeList).append("<option value=" + item.WorkFlowStateApprovalTypeID + ">" + item.WorkFlowStateApprovalTypeName + "</option>");
                });
            }
        });
        promise.fail(showError);

    }

    function loadWFStateApprovalStatusMappingGrid(workFlowVersionStateID) {

        var getWFStateApprovalStatusMapping = URLs.getWFStateApprovalStatusMappingList.replace(/\{workFlowVersionStateID\}/g, workFlowVersionStateID);
        //var getWFStateAccessMapping = '';

        var colArray = ['', '', '', 'Approval Type', 'Actions'];
        //set column models
        var colModel = [];
        colModel.push({ key: true, name: 'WFVersionStatesApprovalTypeID', index: 'WFVersionStatesApprovalTypeID', editable: true, hidden: true });
        colModel.push({ name: 'WorkFlowVersionStateID', index: 'WorkFlowVersionStateID', hidden: true });
        colModel.push({ name: 'WorkFlowStateApprovalTypeID', index: 'WorkFlowStateApprovalTypeID', hidden: true });
        colModel.push({ name: 'WorkFlowStateApprovalTypeName', index: 'WorkFlowStateApprovalTypeName', editable: true, width: '720px' });
        colModel.push({ name: 'ApprovalTypeActions', index: 'ApprovalTypeActions', editable: true, formatter: fmatterWFApplicableStatusAction, width: '270px', search: false, sortable: false });

        //adding the pager element
        $(elementIDs.wfStateApprovalStatusGridJQ).parent().append("<div id='p" + elementIDs.wfStateApprovalStatusGrid + "'></div>");

        //clean up the grid first - only table element remains after this
        $(elementIDs.wfStateApprovalStatusGridJQ).jqGrid('GridUnload');
        $(elementIDs.wfStateApprovalStatusGridJQ).jqGrid({
            datatype: 'json',
            url: getWFStateApprovalStatusMapping,
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
            caption: "WorkFlow State Approval Type Mapping",
            sortname: 'WFVersionStatesApprovalTypeID',
            pager: '#p' + elementIDs.wfStateApprovalStatusGrid,
            editurl: 'clientArray',
            loadComplete: function () {
                //set grid caption
                var data = 'WorkFlow State Approval Type Mapping';
                var rowIDW = $(elementIDs.workflowCategoryMappingGridJQ).getGridParam('selrow');
                var rowIDS = $(elementIDs.workflowStateMappingGridJQ).getGridParam('selrow'); var caption = 'Approval Types with Actions for: ';
                if (rowIDS !== undefined && rowIDS !== null)
                    var state = $(elementIDs.workflowStateMappingGridJQ).getRowData(rowIDS);
                if (rowIDS !== undefined && rowIDS !== null)
                    var category = $(elementIDs.workflowCategoryMappingGridJQ).getRowData(rowIDW);
                if (category.length != 0 && state.length != 0) {
                    var categoryName = " " + category.AccountTypeName + " - " + category.CategoryName;
                    data = categoryName + " -> " + state.WFStateName;
                }
                $(elementIDs.wfStateApprovalStatusGridJQ).jqGrid('setCaption', caption + data.bold());

                //function to scroll vertically to gird position
                goToByScroll(elementIDs.workflowStateApprovalStatusDiv);

                registerEvents();
                //if (roleId == Role.HNESuperUser) {
                //    $("#btnWFStateApprovalStatusAdd").addClass('disabled-button');
                //    $("#btnWFStateApprovalStatusDelete").addClass('disabled-button');
                //    $("#btnWFStateApprovalStatusDelete").addClass('disabled-button');
                //}
            },
            gridComplete: function () {
                //hide add/edit/delete buttons if category mapping is Finalized
                if (isFinalized)
                    $('#p' + elementIDs.wfStateApprovalStatusGrid + '_left').find('table').hide();//sprint5-Bug71
            },
            altRows: true
        });

        var pagerElement = '#p' + elementIDs.wfStateApprovalStatusGrid;
        $(pagerElement).find('input').css('height', '20px');

        //remove default buttons
        $(elementIDs.wfStateApprovalStatusGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

        $(elementIDs.wfStateApprovalStatusGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

        //add button in footer of grid
        $(elementIDs.wfStateApprovalStatusGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon-plus',
                title: 'Add', id: 'btnWFStateApprovalStatusAdd',
                onClickButton: function () {
                    //load Edit Category Workflow dialog on click - see categoryWorkflowDesignDialog function below
                    wFStateApprovalStatusDesignDialog.show('', 'add');
                }
            });

        //delete button in footer of grid
        $(elementIDs.wfStateApprovalStatusGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-trash',
            title: 'Delete', id: 'btnWFStateApprovalStatusDelete',
            onClickButton: function () {
                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {

                    //load confirm dialogue to asset the operation
                    confirmDialog.show(Common.deleteConfirmationMsg, function () {
                        confirmDialog.hide();

                        //delete the form design
                        var workflowStateApprovalStatusDelete = {
                            wFVersionStatesApprovalTypeID: rowId
                        };
                        var promise = ajaxWrapper.postJSON(URLs.deleteWFVersionStatesApprovalType, workflowStateApprovalStatusDelete);
                        //register ajax success callback
                        promise.done(worflowStateApprovalStatusDeleteSuccess);
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

    var fmatterWFApplicableStatusAction = function (cellvalue, options, rowObject) {
        if (!$.isNumeric(options.rowId)) {
            return '';
        }
        var link = "<a href='#' class='WFSTATUSACTION' data-Id='" + rowObject.WFVersionStatesApprovalTypeID + "'><span align='center' class='ui-icon ui-icon-arrowreturnthick-1-s' style='margin:auto' title='ViewActions'></span></a>";
        return link;
    }


    //ajax callback success - reload WF State Approval Status Mapping Grid
    function worflowStateApprovalStatusDeleteSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(WorkFlowSettingsMessages.wfVerStateAPStatusDeleteSuccess);

            //hide actions div on approval type delete.
            $(elementIDs.wfStateApprovalStatusActionsDiv).hide();
        }
        else {
            if (xhr.Items[0].Messages.length > 0) messageDialog.show(xhr.Items[0].Messages[0]);
            else messageDialog.show(Common.errorMsg);
        }
        var workFlowVersionStateID = $(elementIDs.workflowStateMappingGridJQ).getGridParam('selrow');
        loadWFStateApprovalStatusMappingGrid(workFlowVersionStateID)
    }

    //-----------------------------------------------Workflow States Approval Status Mapping---------------------------------//

    //-----------------------------------------------Workflow Approval Status Actions ---------------------------------------//

    function loadWFStateApprovalStatusActionsDetails(wfStateApprovalTypeID) {
        $(elementIDs.wfStateApprovalStatusActionsDiv).show();
        loadWFStateApprovalStatusActionsGrid(wfStateApprovalTypeID);
    }

    function loadWFStateApprovalStatusActionsGrid(wfStateApprovalTypeID) {

        var getWFStateApprovalStatusActionsMapping = URLs.getWFStateApprovalStatusActionsMappingList.replace(/\{wFVersionStatesApprovalTypeID\}/g, wfStateApprovalTypeID);

        var colArray = ['', '', '', 'Action Type', 'Action Details'];
        //set column models
        var colModel = [];
        colModel.push({ key: true, name: 'WFStatesApprovalTypeActionID', index: 'WFStatesApprovalTypeActionID', editable: true, hidden: true });
        colModel.push({ name: 'ActionID', index: 'ActionID', editable: true, hidden: true });
        colModel.push({ name: 'WFVersionStatesApprovalTypeID', index: 'WFVersionStatesApprovalTypeID', hidden: true, editable: true });
        colModel.push({ name: 'ActionName', index: 'ActionName', editable: true, width: '400px' });
        colModel.push({ name: 'ActionResponse', index: 'ActionResponse', editable: true, width: '590px' });


        //adding the pager element
        $(elementIDs.wfStateApprovalStatusActionsGridJQ).parent().append("<div id='p" + elementIDs.wfStateApprovalStatusActionsGrid + "'></div>");

        //clean up the grid first - only table element remains after this
        $(elementIDs.wfStateApprovalStatusActionsGridJQ).jqGrid('GridUnload');
        $(elementIDs.wfStateApprovalStatusActionsGridJQ).jqGrid({
            datatype: 'json',
            url: getWFStateApprovalStatusActionsMapping,
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
            caption: "WorkFlow State Action",
            sortname: 'WFStatesApprovalTypeActionID',
            pager: '#p' + elementIDs.wfStateApprovalStatusActionsGrid,
            editurl: 'clientArray',
            loadComplete: function () {
                //set grid caption
                var data = 'WorkFlow State Action';
                var rowIDW = $(elementIDs.workflowCategoryMappingGridJQ).getGridParam('selrow');
                var rowIDS = $(elementIDs.workflowStateMappingGridJQ).getGridParam('selrow');
                var rowIDAT = $(elementIDs.wfStateApprovalStatusGridJQ).getGridParam('selrow');
                var caption = 'Actions for: ';
                if (rowIDS !== undefined && rowIDS !== null)
                    var state = $(elementIDs.workflowStateMappingGridJQ).getRowData(rowIDS);
                if (rowIDW !== undefined && rowIDW !== null)
                    var category = $(elementIDs.workflowCategoryMappingGridJQ).getRowData(rowIDW);
                if (rowIDAT !== undefined && rowIDAT !== null)
                    var approvalType = $(elementIDs.wfStateApprovalStatusGridJQ).getRowData(rowIDAT);
                if (category != undefined && state != undefined && approvalType != undefined) {
                    var categoryName = " " + category.AccountTypeName + " - " + category.CategoryName;
                    data = categoryName + " -> " + state.WFStateName + " -> " + approvalType.WorkFlowStateApprovalTypeName;
                }

                $(elementIDs.wfStateApprovalStatusActionsGridJQ).jqGrid('setCaption', caption + data.bold());

                //Populate state dropdown for action design
                var workFlowVersionID = $(elementIDs.workflowCategoryMappingGridJQ).getGridParam('selrow');
                loadActionstateList(workFlowVersionID);
                //function to scroll vertically to gird position
                goToByScroll(elementIDs.wfStateApprovalStatusActionsDiv);
                //if (roleId == Role.HNESuperUser) {
                //    $("#btnWFStateApprovalStatusEdit").addClass('disabled-button');
                //}
            },
            gridComplete: function () {
                //hide add/edit/delete buttons if category mapping is Finalized
                //if (isFinalized)
                //    $('#p' + elementIDs.wfStateApprovalStatusActionsGrid + '_left').hide();
            },
            altRows: true
        });

        var pagerElement = '#p' + elementIDs.wfStateApprovalStatusActionsGrid;
        $(pagerElement).find('input').css('height', '20px');

        //remove default buttons
        $(elementIDs.wfStateApprovalStatusActionsGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

        $(elementIDs.wfStateApprovalStatusActionsGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });


        //edit button in footer of grid
        $(elementIDs.wfStateApprovalStatusActionsGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon-pencil',
                title: 'Edit', id: 'btnWFStateApprovalStatusEdit',
                onClickButton: function () {
                    var rowId = $(this).getGridParam('selrow');
                    if (rowId !== undefined && rowId !== null) {
                        var row = $(elementIDs.wfStateApprovalStatusActionsGridJQ).getRowData(rowId);
                        wFStateApprovalStatusActionDesignDialog.show(row.ActionID, row.ActionResponse, 'edit');
                    }
                    else {
                        messageDialog.show('Please select a row.');
                    }
                }
            });

    }

    function loadActionstateList(workFlowVersionID) {
        $(elementIDs.actionStatesList).empty();
        $(elementIDs.actionStatesList).append("<option value='0'>" + "Select State" + "</option>");

        var url = URLs.getWorkflowStateMappingList.replace(/\{workFlowVersionID\}/g, workFlowVersionID);
        var promise = ajaxWrapper.getJSON(url);
        //register ajax success callback
        promise.done(function (items) {
            if (items) {
                $.each(items, function (i, item) {
                    $(elementIDs.actionStatesList).append("<option value=" + item.WorkFlowVersionStateID + ">" + item.WFStateName + "</option>");
                });
            }
        });
        promise.fail(showError);

    }

    //-----------------------------------------------Workflow Approval Status Actions----------------------------------------//

    function goToByScroll(id) {
        // Reove "link" from the ID
        id = id.replace("#", "");
        // Scroll
        $('html,body').animate({
            scrollTop: $("#" + id).offset().top
        },
            'fast');
    }

    function registerEvents() {
        $(".WFSTATE").unbind('click');
        $(".WFSTATE").click(function () {
            var workFlowVersionID = $(this).attr("data-Id");
            loadWorkFlowStatesDetails(workFlowVersionID);
        });

        $(".WFORDER").unbind('click');
        $(".WFORDER").click(function () {
            var workFlowVersionID = $(this).attr("data-Id");
            loadWorkFlowOrderDetails(workFlowVersionID);
        });

        $(".WFSTATEACCESS").unbind('click');
        $(".WFSTATEACCESS").click(function () {
            var workFlowVersionStateID = $(this).attr("data-Id");
            loadWFStateAccessMappingDetails(workFlowVersionStateID);
        });

        $(".WFSTATUS").unbind('click');
        $(".WFSTATUS").click(function () {
            var workFlowVersionStateID = $(this).attr("data-Id");
            loadWFStateApprovalStatusMappingDetails(workFlowVersionStateID);
        });
        //if (roleId != Role.HNESuperUser) {
            $(".WFFINALIZE").unbind('click');
            $(".WFFINALIZE").click(function () {
                var workFlowVersionID = $(this).attr("data-Id");
                finalizeWorkFlowVersion(workFlowVersionID);
            });

            $(".WFFINALIZEDONE").unbind('click');
            $(".WFFINALIZEDONE").click(function () {
                messageDialog.show(WorkFlowSettingsMessages.wfCategoryFinalizedDone);
            });
        //}
        $(".WFSTATUSACTION").unbind('click');
        $(".WFSTATUSACTION").click(function () {
            var wfStateApprovalTypeID = $(this).attr("data-Id");
            loadWFStateApprovalStatusActionsDetails(wfStateApprovalTypeID)
        });

        $(".WFCATCONFIG").unbind('click');
        $(".WFCATCONFIG").click(function () {
            loadCategoryList();
            loadWorkflowCategoryMappingGrid();
            $(elementIDs.settingsTabJQ).tabs({
                selected: 3
            });
            //hide all divs on tab switch.
            $(elementIDs.workflowStateMappingDiv).hide();
            $(elementIDs.workflowStateAccessMappingDiv).hide();
            $(elementIDs.workflowStateApprovalStatusDiv).hide();
            $(elementIDs.wfStateApprovalStatusActionsDiv).hide();
        });
    }

    function finalizeWorkFlowVersion(workFlowVersionID) {
        var workFlowVersionObj =
            { workFlowVersionID: workFlowVersionID };
        //ajax call to add/update
        var promise = ajaxWrapper.postJSON(URLs.finalizeWorkflowVersion, workFlowVersionObj);
        //register ajax success callback
        promise.done(finalizeWorkFlowVersionSuccess);
        //register ajax failure callback
        promise.fail(showError);
    }

    function finalizeWorkFlowVersionSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            var msg = 'WorkFlow Version Finalized Successfully.';
            messageDialog.show(msg);
            //hide all divs on finalize.
            $(elementIDs.workflowStateMappingDiv).hide();
            $(elementIDs.workflowStateAccessMappingDiv).hide();
            $(elementIDs.workflowStateApprovalStatusDiv).hide();
            $(elementIDs.wfStateApprovalStatusActionsDiv).hide();
        }
        else {
            if (xhr.Items[0].Messages.length > 0) messageDialog.show(xhr.Items[0].Messages[0]);
            else messageDialog.show(Common.errorMsg);
        }
        //reload category workflow grid         
        customWorkflowSettings.loadWorkflowCategoryMapping();
    }

    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    init();

    return {
        loadWorkflowCategoryMapping: function () {
            loadWorkflowCategoryMappingGrid();
        },
        loadWorflowStateMapping: function (workFlowVersionID, wfStateMappingChangeID) {
            loadWorflowStateMappingGrid(workFlowVersionID, wfStateMappingChangeID);
        },
        loadWFStateAccessMapping: function (workFlowVersionStateID) {
            loadWFStateAccessMappingGrid(workFlowVersionStateID);
        },
        loadWFStateApprovalStatusMapping: function (workFlowVersionStateID) {
            loadWFStateApprovalStatusMappingGrid(workFlowVersionStateID);
        },
        loadWFStateApprovalStatusActions: function (wfStateApprovalTypeID) {
            loadWFStateApprovalStatusActionsGrid(wfStateApprovalTypeID);
        }
    }

}();

//contains functionality for the Category Workflow Mapping add/edit dialog
var wFcategoryDesignDialog = function () {
    var URLs = {
        //url for Add Category Workflow Mapping
        wFCategoryDesignAdd: '/WorkFlow/AddWorkFlowCategoryMapping',
        //url for Update Category Workflow Mapping
        wFCategoryDesignUpdate: '/WorkFlow/UpdateWorkFlowCategoryMapping',
        //url for copying workflow for one category to another
        wFCategoryDesignCopy: '/WorkFlow/CopyWorkFlowCategoryMapping'
    }

    //see element id's in Views\Settings\WorkFlowSettings.cshtml
    var elementIDs = {

        //category workflow mapping form elements
        categorynameDiv: '#categoryname',
        categorynamelist: '#categorynamelist',
        accounttypelist: '#accounttypelist',
        categoryNameError: '#categorynameError',
        accountTypeError: '#accounttypeError',
        workflowtypelist: '#workflowtypelist',
        workflowtypeError: '#workflowtypeError',
        effectiveDate: "#wfCategoryEffectivedate",
        effectiveDateSpan: "#wfCategoryEffectivedateHelpblock",
        accountTypeDiv: "#accountType",
        workflowTypeDiv: "#workflowType",
        //category list grid element
        workflowCategoryMappingGrid: 'workflowCategoryMappingGrid',
        //with hash for jquery
        workflowCategoryMappingGridJQ: '#workflowCategoryMappingGrid',
        //category list dialog element
        wFCategoryDesignDialog: "#workflowcategorydesigndialog"
    };

    //maintains dialog state - add or edit
    var wFCategoryDesignDialogState;

    //ajax success callback - for add/edit
    function categoryWFSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            var msg = "";
            if (wFCategoryDesignDialogState == "copy")
                msg = WorkFlowSettingsMessages.wfCatCopySuccess;
            else if (wFCategoryDesignDialogState == "add")
                msg = WorkFlowSettingsMessages.wfCatAddSuccess;
            else
                msg = WorkFlowSettingsMessages.wfCatUpdateSuccess;
            messageDialog.show(msg);
        }
        else {
            if (xhr.Items[0].Messages.length > 0) messageDialog.show(xhr.Items[0].Messages[0]);
            else messageDialog.show(Common.errorMsg);
        }
        //reload form design grid 
        customWorkflowSettings.loadWorkflowCategoryMapping();
        //reset dialog elements
        $(elementIDs.wFCategoryDesignDialog + ' div').removeClass('has-error');
        $(elementIDs.wFCategoryDesignDialog + ' .help-block').text('');
        $(elementIDs.wFCategoryDesignDialog).dialog('close');
    }

    //To check effective date is specified or not
    function validEffectiveDate() {
        validEffectiveDateFlag = false;
        var newWFEffectiveDate = $(elementIDs.effectiveDate).val();
        if (newWFEffectiveDate == '') {
            $(elementIDs.effectiveDate).parent().addClass('has-error');
            $(elementIDs.effectiveDate).addClass('form-control');

            $(elementIDs.effectiveDateSpan).parent().addClass('has-error');
            $(elementIDs.effectiveDateSpan).text('Effective Date Required.');
            validEffectiveDateFlag = false
        }
        else {
            var effectiveDateMessage = isValidEffectiveDate(newWFEffectiveDate);
            if (effectiveDateMessage == "") {
                $(elementIDs.effectiveDate).removeClass('form-control');
                $(elementIDs.effectiveDate).parent().addClass('has-error');

                $(elementIDs.effectiveDateSpan).parent().removeClass('has-error');
                $(elementIDs.effectiveDateSpan).text('');
                validEffectiveDateFlag = true;
            }
            else {
                $(elementIDs.effectiveDate).parent().addClass('has-error');
                $(elementIDs.effectiveDate).addClass('form-control');
                $(elementIDs.effectiveDateSpan).parent().addClass('has-error');
                $(elementIDs.effectiveDateSpan).text(effectiveDateMessage);
                validEffectiveDateFlag = false
            }
        }
        return validEffectiveDateFlag;
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
        $(elementIDs.wFCategoryDesignDialog).dialog({
            autoOpen: false,
            height: 250,
            width: 450,
            modal: true
        });

        //To display datepicker for category workflow effective date.
        //$(elementIDs.effectiveDate).datepicker({
        //    dateFormat: 'mm/dd/yy',
        //    changeMonth: true,
        //    changeYear: true,
        //    yearRange: 'c-61:c+20',
        //    showOn: "both",
        //    //CalenderIcon path declare in golbalvariable.js
        //    buttonImage: Icons.CalenderIcon,
        //    buttonImageOnly: true,
        //}).parent().find('img').css('margin-bottom', '7px');

        //register event for Add/Edit button click on dialog
        $(elementIDs.wFCategoryDesignDialog + ' button').on('click', function () {
            //check if name is already used
            var newCategoryName = $(elementIDs.categorynamelist).val();
            var newAccountType = $(elementIDs.accounttypelist).val();
            var newWorkflowType = $(elementIDs.workflowtypelist).val();
            var categoryList = $(elementIDs.workflowCategoryMappingGridJQ).getRowData();
            var newWFEffectiveDate = $(elementIDs.effectiveDate).val();


            //validate category name
            if (newCategoryName == null) {
                $(elementIDs.categorynamelist).parent().addClass('has-error');
                $(elementIDs.categoryNameError).text(WorkFlowSettingsMessages.wfCatNameRequiredError);
                $(elementIDs.categoryNameError).parent().addClass('has-error');
            }
            else if (newAccountType == null && wFCategoryDesignDialogState !== "copy") {
                $(elementIDs.accounttypelist).parent().addClass('has-error');
                $(elementIDs.accountTypeError).text(WorkFlowSettingsMessages.wfAccTypeRequiredError);
                $(elementIDs.accountTypeError).parent().addClass('has-error');
            } else if (newWorkflowType == null && wFCategoryDesignDialogState !== "copy") {
                $(elementIDs.workflowtypelist).parent().addClass('has-error');
                $(elementIDs.workflowtypeError).text(WorkFlowSettingsMessages.wfWFTypeRequiredError);
                $(elementIDs.workflowtypeError).parent().addClass('has-error');
            } else {

                //validate effective Date
                //var isValidEffectiveDate = validEffectiveDate();

                //if (isValidEffectiveDate) {
                //save the new form design
                var rowId = $(elementIDs.workflowCategoryMappingGridJQ).getGridParam('selrow');

                var newWorkflowType = (newWorkflowType === '1') ? 1 : 2;
                var newAccountType = (newAccountType === '1') ? 1 : 2;

                var wFCategoryAdd = {
                    tenantID: 1,
                    workFlowVersionID: rowId,
                    workFlowType: newWorkflowType,
                    accountType: newAccountType,
                    folderVersionCategoryID: newCategoryName
                    //,
                    //effectiveDate: newWFEffectiveDate
                };
                var url;
                if (wFCategoryDesignDialogState === 'add') {
                    url = URLs.wFCategoryDesignAdd;
                }
                else if (wFCategoryDesignDialogState === 'copy') {
                    url = URLs.wFCategoryDesignCopy;
                } else {
                    url = URLs.wFCategoryDesignUpdate;
                }
                //ajax call to add/update
                var promise = ajaxWrapper.postJSON(url, wFCategoryAdd);
                //register ajax success callback
                promise.done(categoryWFSuccess);
                //register ajax failure callback
                promise.fail(showError);
            }
            // }
        });
    }
    //initialize the dialog after this js are loaded
    init();

    //these are the properties that can be called by using formDesignDialog.<Property>
    //eg. formDesignDialog.show('name','add');
    return {
        show: function (FolderVersionCategoryID, accountType, workflowType, action) {
            wFCategoryDesignDialogState = action;

            //set eleemnts based on whether the dialog is opened in add or edit mode
            $(elementIDs.categorynamelist).parent().removeClass('has-error');
            $(elementIDs.categoryNameError).text('');
            $(elementIDs.accounttypelist).parent().removeClass('has-error');
            $(elementIDs.accountTypeError).text('');
            $(elementIDs.workflowtypelist).parent().removeClass('has-error');
            $(elementIDs.workflowtypeError).text('');

            //set all dropdowns enabled on load of popup
            $(elementIDs.categorynamelist).prop("disabled", false);
            $(elementIDs.accounttypelist).prop("disabled", false);
            $(elementIDs.workflowtypelist).prop("disabled", false);
            $(elementIDs.wFCategoryDesignDialog + ' button').prop("disabled", false);

            //get if finalized param
            var rowID = $(elementIDs.workflowCategoryMappingGridJQ).getGridParam('selrow');
            var rowData = $(elementIDs.workflowCategoryMappingGridJQ).getRowData(rowID), isFinalized = false;
            //sprint5-Bug19
            if (rowData.IsFinalized != undefined) {
                if (rowData.IsFinalized.trim() == "false") isFinalized = false; else isFinalized = true;
            }

            if (wFCategoryDesignDialogState === "edit") {

                //hide/show dropdown on basis of add-edit-copy
                $(elementIDs.accountTypeDiv).show();
                $(elementIDs.workflowTypeDiv).show();

                //set values of the of the dropdown
                $(elementIDs.categorynamelist).val(FolderVersionCategoryID);
                $(elementIDs.accounttypelist).val(accountType = (accountType === 'PORTFOLIO') ? 1 : 2);
                $(elementIDs.workflowtypelist).val(workflowType = (workflowType === 'SEQUENTIAL') ? 1 : 2);

                //set title of modal popup
                $(elementIDs.wFCategoryDesignDialog).dialog('option', 'title', WorkFlowSettingsMessages.wfCategoryUpdateTitle);

                //disable category dropdown for edit
                $(elementIDs.categorynamelist).prop("disabled", true);

                //disable/enable dropdowns on edit when mapping is finalized
                if (isFinalized) {
                    $(elementIDs.accounttypelist).prop("disabled", true);
                    $(elementIDs.workflowtypelist).prop("disabled", true);
                    $(elementIDs.wFCategoryDesignDialog + ' button').prop("disabled", true);
                }
                else {
                    $(elementIDs.accounttypelist).prop("disabled", false);
                    $(elementIDs.workflowtypelist).prop("disabled", false);
                    $(elementIDs.wFCategoryDesignDialog + ' button').prop("disabled", false);
                }
            }
            else if (wFCategoryDesignDialogState === "copy") {
                //show only category dropdown when copying a category
                $(elementIDs.categorynamelist).val(0);
                $(elementIDs.accountTypeDiv).hide();
                $(elementIDs.workflowTypeDiv).hide();
                $(elementIDs.wFCategoryDesignDialog).dialog('option', 'title', "Copy Workflow");
            } else {
                //show all dropdown when adding a category
                $(elementIDs.accountTypeDiv).show();
                $(elementIDs.workflowTypeDiv).show();
                $(elementIDs.categorynamelist).val(0);
                $(elementIDs.accounttypelist).val(0);
                $(elementIDs.workflowtypelist).val(0);

                //set title of modal popup
                $(elementIDs.wFCategoryDesignDialog).dialog('option', 'title', WorkFlowSettingsMessages.wfCategoryAddTitle);
                //enable category dropdown for add
                $(elementIDs.categorynamelist).prop("disabled", false);
            }

            //open the dialog - uses jqueryui dialog
            $(elementIDs.wFCategoryDesignDialog).dialog("open");
        }
    }
}(); //invoked soon after loading

//contains functionality for the Workflow States Mapping add/edit dialog
var wFStateDesignDialog = function () {
    var URLs = {
        //url for Add States Workflow Mapping
        wFStateDesignAdd: '/WorkFlow/AddWorkFlowVersionStates',
        //url for Edit States Workflow Mapping
        wFStateDesignEdit: '/WorkFlow/UpdateWorkFlowVersionStates'
    }

    //see element id's in Views\Settings\WorkFlowSettings.cshtml
    var elementIDs = {

        //States workflow mapping form elements        
        wfstatenamelist: '#wfstatenamelist',
        wfstatesequencelist: '#wfstatesequencelist',
        wfstatesequenceDiv: "#wfstatesequenceDiv",
        wfstatesequenceError: "#wfstatesequenceError",

        workflowStateMappingGrid: "workflowStateMappingGrid",
        workflowStateMappingGridJQ: "#workflowStateMappingGrid",
        workflowCategoryMappingGridJQ: "#workflowCategoryMappingGrid",
        //States list dialog element
        workflowStatesDesignDialog: "#workflowstatesdesigndialog"
    };

    //maintains dialog state - add or edit
    var workflowStatesDesignDialogState;

    //ajax success callback - for add/edit
    function workflowStatesSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            var msg = (workflowStatesDesignDialogState === "add") ? WorkFlowSettingsMessages.wfVersionStateAddSuccess : WorkFlowSettingsMessages.wfVersionOrderUpdateSuccess;
            messageDialog.show(msg);
        }
        else {
            if (xhr.Items[0].Messages.length > 0) messageDialog.show(xhr.Items[0].Messages[0]);
            else messageDialog.show(Common.errorMsg);
        }
        //reload workflow version state grid 
        var workFlowVersionID = $(elementIDs.workflowCategoryMappingGridJQ).getGridParam('selrow');

        customWorkflowSettings.loadWorflowStateMapping(workFlowVersionID, wfStateMappingChange);
        //reset dialog elements
        $(elementIDs.workflowStatesDesignDialog + ' div').removeClass('has-error');
        $(elementIDs.workflowStatesDesignDialog + ' .help-block').text('');
        $(elementIDs.workflowStatesDesignDialog).dialog('close');
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
        $(elementIDs.workflowStatesDesignDialog).dialog({
            autoOpen: false,
            height: 250,
            width: 250,
            modal: true
        });
        //register event for Add/Edit button click on dialog
        $(elementIDs.workflowStatesDesignDialog + ' button').on('click', function () {
            //check if name is already used
            var newStateName = $(elementIDs.wfstatenamelist).val();
            var newStateSequence = $(elementIDs.wfstatesequencelist).val();

            newStateSequence = (newStateSequence == null) ? 0 : newStateSequence;

            //validate state name
            if (newStateName == null) {
                $(elementIDs.workflowStatesDesignDialog + ' div').addClass('has-error');
                $(elementIDs.workflowStatesDesignDialog + ' .help-block').text(WorkFlowSettingsMessages.wfVersionStateRequiredError);
            }
                //Sprint5-Bug57
                //else if (newStateSequence == "0" && workflowStatesDesignDialogState == "edit") {
                //    $(elementIDs.wfstatesequenceDiv).addClass('has-error');
                //    $(elementIDs.wfstatesequenceError).text(WorkFlowSettingsMessages.wfVersionStateOrderRequiredError);
                //}
            else {
                if (workflowStatesDesignDialogState == "edit" && !validateSequence(newStateSequence)) {
                    $(elementIDs.wfstatesequenceDiv).addClass('has-error');
                    $(elementIDs.wfstatesequenceError).text(WorkFlowSettingsMessages.wfVersionStateOrderRepeatError);
                } else {
                    //save the new state mapping
                    var workFlowVersionID = $(elementIDs.workflowCategoryMappingGridJQ).getGridParam('selrow');
                    var workFlowVersionStatesID = $(elementIDs.workflowStateMappingGridJQ).getGridParam('selrow');

                    var wFStateAdd = {
                        tenantID: 1,
                        workFlowVersionStatesID: workFlowVersionStatesID,
                        workFlowVersionID: workFlowVersionID,
                        wFStateID: newStateName,
                        sequence: newStateSequence
                    };

                    var url;
                    if (workflowStatesDesignDialogState === 'add') {
                        url = URLs.wFStateDesignAdd;
                    }
                    else {
                        url = URLs.wFStateDesignEdit;
                    }

                    //ajax call to add/update
                    var promise = ajaxWrapper.postJSON(url, wFStateAdd);
                    //register ajax success callback
                    promise.done(workflowStatesSuccess);
                    //register ajax failure callback
                    promise.fail(showError);
                }
            }
        });
    }

    function validateSequence(newStateSequence) {
        var isValidSequence = false;

        var rowId = $(elementIDs.workflowCategoryMappingGridJQ).getGridParam('selrow');
        var workflowType = $(elementIDs.workflowCategoryMappingGridJQ).getRowData(rowId);
        if (workflowType.WorkFlowtypeName == "SEQUENTIAL") {
            var stateSequenceGridData = $(elementIDs.workflowStateMappingGridJQ).jqGrid("getRowData");
            var duplicateSequence = $.grep(stateSequenceGridData, function (e) {
                return e.Sequence == newStateSequence;
            });

            if (duplicateSequence.length == 0) isValidSequence = true; else isValidSequence = false;
        }
        else isValidSequence = true;

        return isValidSequence;
    }

    //initialize the dialog after this js are loaded
    init();

    //these are the properties that can be called by using formDesignDialog.<Property>
    //eg. formDesignDialog.show('name','add');
    return {
        show: function (stateNameID, action) {
            workflowStatesDesignDialogState = action;

            //populate with exsisting values
            $(elementIDs.wfstatenamelist).val(stateNameID);

            //set eleemnts based on whether the dialog is opened in add or edit mode
            $(elementIDs.workflowStatesDesignDialog + ' div').removeClass('has-error');
            $(elementIDs.workflowStatesDesignDialog + ' .help-block').text('');

            $(elementIDs.wfstatesequenceDiv).removeClass('has-error');
            $(elementIDs.wfstatesequenceError).text('');

            $(elementIDs.wfstatesequencelist).val('0');


            if (workflowStatesDesignDialogState === "add") {
                $(elementIDs.wfstatenamelist).prop("disabled", false);
                $(elementIDs.wfstatesequenceDiv).hide();
                $(elementIDs.workflowStatesDesignDialog).dialog('option', 'title', WorkFlowSettingsMessages.wfStateAddTitle);
            }
            else {
                $(elementIDs.wfstatenamelist).prop("disabled", true);
                $(elementIDs.wfstatesequenceDiv).show();
                $(elementIDs.workflowStatesDesignDialog).dialog('option', 'title', WorkFlowSettingsMessages.wfStateUpdateTitle);

                //populate sequence dropdown
                var wfStateMappingGridCount = jQuery(elementIDs.workflowStateMappingGridJQ).jqGrid('getGridParam', 'records');
                if (wfStateMappingGridCount != 0) {
                    var i = 1;
                    $(elementIDs.wfstatesequencelist).empty();
                    $(elementIDs.wfstatesequencelist).append("<option value=" + 0 + ">" + 0 + "</option>");
                    while (wfStateMappingGridCount != 0) {
                        $(elementIDs.wfstatesequencelist).append("<option value=" + i + ">" + i + "</option>");
                        i++;
                        wfStateMappingGridCount--;
                    }
                }
            }

            //open the dialog - uses jqueryui dialog
            $(elementIDs.workflowStatesDesignDialog).dialog("open");
        }
    }
}(); //invoked soon after loading

//contains functionality for the Workflow StateAccess add/edit dialog
var wFStateAccessDesignDialog = function () {
    var URLs = {
        //url for Add States Workflow Mapping
        wFStateAccessDesignAdd: '/WorkFlow/AddWorkFlowVersionStatesAccess',
        //url for Edit States Workflow Mapping
        wFStateAccessDesignEdit: '/WorkFlow/UpdateWorkFlowVersionStatesAccess'
    }

    //see element id's in Views\Settings\WorkFlowSettings.cshtml
    var elementIDs = {

        //States workflow mapping form elements        
        userRolelist: '#userRolelist',
        userRoleError: "#userRoleError",

        workflowStateAccessMappingGridJQ: "#workflowStateAccessMappingGrid",
        workflowStateMappingGridJQ: "#workflowStateMappingGrid",
        //States list dialog element
        workflowStateAccessDesignDialog: "#workflowStateAccessDesignDialog"
    };

    //maintains dialog state - add or edit
    var wFStateAccessDesignDialogState;

    //ajax success callback - for add/edit
    function workflowStateAccessSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            var msg = (wFStateAccessDesignDialogState === "add") ? WorkFlowSettingsMessages.wfVerStateAccessAddSuccess : WorkFlowSettingsMessages.wfVerStateAccessUpdateSuccess;
            messageDialog.show(msg);
        }
        else {
            if (xhr.Items[0].Messages.length > 0) messageDialog.show(xhr.Items[0].Messages[0]);
            else messageDialog.show(Common.errorMsg);
        }
        //reload workflow State Access grid 
        var workFlowVersionStateID = $(elementIDs.workflowStateMappingGridJQ).getGridParam('selrow');
        customWorkflowSettings.loadWFStateAccessMapping(workFlowVersionStateID);
        //reset dialog elements
        $(elementIDs.workflowStateAccessDesignDialog + ' div').removeClass('has-error');
        $(elementIDs.workflowStateAccessDesignDialog + ' .help-block').text('');
        $(elementIDs.workflowStateAccessDesignDialog).dialog('close');
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
        $(elementIDs.workflowStateAccessDesignDialog).dialog({
            autoOpen: false,
            height: 250,
            width: 250,
            modal: true
        });
        //register event for Add/Edit button click on dialog
        $(elementIDs.workflowStateAccessDesignDialog + ' button').on('click', function () {
            //check if name is already used
            var newUserRoleID = $(elementIDs.userRolelist).val();

            //validate state name
            if (newUserRoleID == "0") {
                $(elementIDs.userRolelist).parent().addClass('has-error');
                $(elementIDs.userRoleError).text(WorkFlowSettingsMessages.wfVerStateUserRoleRequiredError);
            } else {
                //save the new state mapping
                var workFlowVersionStatesAccessID = $(elementIDs.workflowStateAccessMappingGridJQ).getGridParam('selrow');
                var workFlowVersionStateID = $(elementIDs.workflowStateMappingGridJQ).getGridParam('selrow');

                var wFStateAccessAdd = {
                    workFlowVersionStateID: workFlowVersionStateID,
                    roleID: newUserRoleID,
                    workFlowVersionStatesAccessID: workFlowVersionStatesAccessID
                };

                var url;
                if (wFStateAccessDesignDialogState === 'add') {
                    url = URLs.wFStateAccessDesignAdd;
                }
                else {
                    url = URLs.wFStateAccessDesignEdit;
                }

                //ajax call to add/update
                var promise = ajaxWrapper.postJSON(url, wFStateAccessAdd);
                //register ajax success callback
                promise.done(workflowStateAccessSuccess);
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
        show: function (userRoleID, action) {
            wFStateAccessDesignDialogState = action;

            if (wFStateAccessDesignDialogState === "edit") {
                $(elementIDs.userRolelist).val(userRoleID);
                $(elementIDs.workflowStateAccessDesignDialog).dialog('option', 'title', WorkFlowSettingsMessages.wfStateAccessUpdateTitle);
            }
            else {
                $(elementIDs.userRolelist).val(0);
                $(elementIDs.workflowStateAccessDesignDialog).dialog('option', 'title', WorkFlowSettingsMessages.wfStateAccessAddTitle);
            }

            //set eleemnts based on whether the dialog is opened in add or edit mode
            $(elementIDs.userRolelist).parent().removeClass('has-error');
            $(elementIDs.userRoleError).text('');

            //open the dialog - uses jqueryui dialog
            $(elementIDs.workflowStateAccessDesignDialog).dialog("open");
        }
    }
}(); //invoked soon after loading

//contains functionality for the Workflow StateApprovalStatus add/edit dialog
var wFStateApprovalStatusDesignDialog = function () {
    var URLs = {
        //url for Add States Workflow Mapping
        wFStateApprovalStatusDesignAdd: '/WorkFlow/AddWFVersionStatesApprovalType',
        //url for Edit States Workflow Mapping
        wFStateApprovalStatusDesignEdit: '/WorkFlow/UpdateWFVersionStatesApprovalType'
    }

    //see element id's in Views\Settings\WorkFlowSettings.cshtml
    var elementIDs = {

        //States workflow mapping form elements        
        wfApprovalTypeList: '#wfApprovalTypeList',
        wfApprovalTypeError: "#wfApprovalTypeError",

        wfStateApprovalStatusGridJQ: "#wfStateApprovalStatusGrid",
        workflowStateMappingGridJQ: "#workflowStateMappingGrid",
        //States list dialog element
        wfStateApprovalStatusDesignDialog: "#wfStateApprovalStatusDesignDialog"
    };

    //maintains dialog state - add or edit
    var wfStateApprovalStatusDesignDialogState;

    //ajax success callback - for add/edit
    function workflowStateApprovalTypeSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            var msg = (wfStateApprovalStatusDesignDialogState === "add") ? WorkFlowSettingsMessages.wfVerStateAPStatusAddSuccess : WorkFlowSettingsMessages.wfVerStateAPStatusUpdateSuccess;
            messageDialog.show(msg);
        }
        else {
            messageDialog.show(xhr.Items[0].Messages[0]);
        }
        //reload form design grid 
        var workFlowVersionStateID = $(elementIDs.workflowStateMappingGridJQ).getGridParam('selrow');
        customWorkflowSettings.loadWFStateApprovalStatusMapping(workFlowVersionStateID);
        //reset dialog elements
        $(elementIDs.wfStateApprovalStatusDesignDialog + ' div').removeClass('has-error');
        $(elementIDs.wfStateApprovalStatusDesignDialog + ' .help-block').text('');
        $(elementIDs.wfStateApprovalStatusDesignDialog).dialog('close');
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
        $(elementIDs.wfStateApprovalStatusDesignDialog).dialog({
            autoOpen: false,
            height: 250,
            width: 250,
            modal: true
        });
        //register event for Add/Edit button click on dialog
        $(elementIDs.wfStateApprovalStatusDesignDialog + ' button').on('click', function () {
            //check if name is already used
            var newApprovalTypeID = $(elementIDs.wfApprovalTypeList).val();

            //validate state name
            if (newApprovalTypeID == "0") {
                $(elementIDs.wfApprovalTypeList).parent().addClass('has-error');
                $(elementIDs.wfStateApprovalStatusDesignDialog + ' .help-block').text(WorkFlowSettingsMessages.wfVersionStateAPStatusRequiredError);
            } else {
                //save the new state mapping
                var wFVersionStatesApprovalTypeID = $(elementIDs.wfStateApprovalStatusGridJQ).getGridParam('selrow');
                var workFlowVersionStateID = $(elementIDs.workflowStateMappingGridJQ).getGridParam('selrow');

                var wFStateApprovalTypeAdd = {
                    wFVersionStatesApprovalTypeID: wFVersionStatesApprovalTypeID,
                    workFlowStateApprovalTypeID: newApprovalTypeID,
                    workFlowVersionStateID: workFlowVersionStateID
                };

                var url;
                if (wfStateApprovalStatusDesignDialogState === 'add') {
                    url = URLs.wFStateApprovalStatusDesignAdd;
                }
                else {
                    url = URLs.wFStateApprovalStatusDesignEdit;
                }

                //ajax call to add/update
                var promise = ajaxWrapper.postJSON(url, wFStateApprovalTypeAdd);
                //register ajax success callback
                promise.done(workflowStateApprovalTypeSuccess);
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
        show: function (approvalTypeID, action) {
            wfStateApprovalStatusDesignDialogState = action;

            if (wfStateApprovalStatusDesignDialogState === "edit") {
                $(elementIDs.wfApprovalTypeList).val(approvalTypeID);
                $(elementIDs.wfStateApprovalStatusDesignDialog).dialog('option', 'title', WorkFlowSettingsMessages.wfStateAPPTypeUpdateTitle);
            }
            else {
                $(elementIDs.wfApprovalTypeList).val(0);
                $(elementIDs.wfStateApprovalStatusDesignDialog).dialog('option', 'title', WorkFlowSettingsMessages.wfStateAPPTypeAddTitle);
            }

            //set eleemnts based on whether the dialog is opened in add or edit mode
            $(elementIDs.wfApprovalTypeList).parent().removeClass('has-error');
            $(elementIDs.wfApprovalTypeError).text('');

            //open the dialog - uses jqueryui dialog
            $(elementIDs.wfStateApprovalStatusDesignDialog).dialog("open");
        }
    }
}(); //invoked soon after loading

//Validate Effective Date
function isValidEffectiveDate(effectiveDate) {

    var dtCh = "/";
    var daysInMonth = getDaysInMonth(12);
    var pos1 = effectiveDate.indexOf(dtCh);
    var pos2 = effectiveDate.indexOf(dtCh, pos1 + 1);
    var strMonth = effectiveDate.substring(0, pos1);
    var strDay = effectiveDate.substring(pos1 + 1, pos2);
    var strYear = effectiveDate.substring(pos2 + 1);
    strYr = strYear;
    if (strDay.charAt(0) == "0" && strDay.length > 1) strDay = strDay.substring(1);
    if (strMonth.charAt(0) == "0" && strMonth.length > 1) strMonth = strMonth.substring(1);
    for (var i = 1; i <= 3; i++) {
        if (strYr.charAt(0) == "0" && strYr.length > 1) strYr = strYr.substring(1);
    }
    month = parseInt(strMonth);
    day = parseInt(strDay);
    year = parseInt(strYr);
    if (pos1 == -1 || pos2 == -1) {
        return Common.effectiveDateValidMsg;
    }
    if (strMonth.length < 1 || month < 1 || month > 12) {
        return Common.effectiveDateValidateMonthMsg;
    }
    if (strDay.length < 1 || day < 1 || day > 31 || (month == 2 && day > daysInFebruary(year)) || day > daysInMonth[month]) {
        return Common.effectiveDateValidateDayMsg;
    }
    if (strYear.length != 4 || year == 0) {
        return Common.effectiveDateValidateYearMsg;
    }
    return "";
}

//contains functionality for the Workflow StateApprovalStatus Actions add/edit dialog
var wFStateApprovalStatusActionDesignDialog = function () {
    var URLs = {
        //url for Edit States Workflow Mapping
        wFStateApprovalStatusActionDesignEdit: '/WorkFlow/UpdateWFVersionStatesApprovalTypeAction'
    }

    //see element id's in Views\Settings\WorkFlowSettings.cshtml
    var elementIDs = {
        actionEmailDiv: "#actionEmail",
        actionStatesDiv: "#actionStates",

        //Action design elements        
        actionEmailText: "#actionEmailText",
        actionEmailError: "#actionEmailError",
        actionStatesLabel: "#actionStatesLabel",
        actionStatesList: "#actionStatesList",
        actionStatesError: "#actionStatesError",

        workflowCategoryMappingGridJQ: "#workflowCategoryMappingGrid",
        workflowStateAccessMappingGridJQ: "#workflowStateAccessMappingGrid",
        wfStateApprovalStatusGridJQ: "#wfStateApprovalStatusGrid",
        wfStateApprovalStatusActionsGridJQ: "#wfStateApprovalStatusActionsGrid",
        wfStateApprovalStatusActionsDesignDialog: "#wfStateApprovalStatusActionsDesignDialog"
    };
    //initialize the action Type
    var wFStateApprovalStatusActionID;

    //ajax success callback - for add/edit
    function workflowStateApprovalTypeActionSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            var msg = WorkFlowSettingsMessages.wfVersionStateAPStatusActionADDSuccess;
            messageDialog.show(msg);
        }
        else {
            if (xhr.Items[0].Messages.length > 0) messageDialog.show(xhr.Items[0].Messages[0]);
            else messageDialog.show(Common.errorMsg);
        }
        //reload State Approval Status Actions grid 
        var wfStateApprovalTypeID = $(elementIDs.wfStateApprovalStatusGridJQ).getGridParam('selrow');
        customWorkflowSettings.loadWFStateApprovalStatusActions(wfStateApprovalTypeID);
        //reset dialog elements
        $(elementIDs.wfStateApprovalStatusActionsDesignDialog + ' div').removeClass('has-error');
        $(elementIDs.wfStateApprovalStatusActionsDesignDialog + ' .help-block').text('');
        $(elementIDs.wfStateApprovalStatusActionsDesignDialog).dialog('close');
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
        $(elementIDs.wfStateApprovalStatusActionsDesignDialog).dialog({
            autoOpen: false,
            height: 250,
            width: 450,
            modal: true
        });
        //register event for Add/Edit button click on dialog
        $(elementIDs.wfStateApprovalStatusActionsDesignDialog + ' button').on('click', function () {
            var ele;
            if (wFStateApprovalStatusActionID == 1) ele = $(elementIDs.actionEmailText); else ele = $(elementIDs.actionStatesList);

            var newActionResponse = ele.val();

            //validate state name
            if (wFStateApprovalStatusActionID == 1 && newActionResponse == "") {
                $(elementIDs.actionEmailText).parent().addClass('has-error');
                $(elementIDs.actionEmailError).parent().addClass('has-error');
                $(elementIDs.actionEmailError).text(WorkFlowSettingsMessages.wfVersionActionEmailRequiredError);
            } else if (wFStateApprovalStatusActionID == 1 && !validateEmail(newActionResponse)) {
                $(elementIDs.actionEmailText).parent().addClass('has-error');
                $(elementIDs.actionEmailError).parent().addClass('has-error');
                $(elementIDs.actionEmailError).text(WorkFlowSettingsMessages.wfVersionActionValidEmailError);
            } else if (newActionResponse == null || newActionResponse == "0") {
                $(elementIDs.actionStatesList).parent().addClass('has-error');
                $(elementIDs.actionStatesError).parent().addClass('has-error');
                $(elementIDs.actionStatesError).text(WorkFlowSettingsMessages.wfVersionActionStateRequiredError);
            }
            else {
                //save the new state mapping
                var wFStatesApprovalTypeActionID = $(elementIDs.wfStateApprovalStatusActionsGridJQ).getGridParam('selrow');

                var wFStateApprovalTypeActionObj = {
                    wFStatesApprovalTypeActionID: wFStatesApprovalTypeActionID,
                    actionResponse: newActionResponse
                };

                //ajax call to add/update
                var promise = ajaxWrapper.postJSON(URLs.wFStateApprovalStatusActionDesignEdit, wFStateApprovalTypeActionObj);
                //register ajax success callback
                promise.done(workflowStateApprovalTypeActionSuccess);
                //register ajax failure callback
                promise.fail(showError);
            }
        });
    }

    //function to validate comma seperated email ids
    function validateEmail(newActionResponse) {
        var isValid = true;
        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        var emailIds = newActionResponse.split(',');
        $(emailIds).each(function (index, email) {
            if (!re.test(email)) {
                isValid = false;
            }
        });
        return isValid;
    }

    //initialize the dialog after this js are loaded
    init();

    //these are the properties that can be called by using formDesignDialog.<Property>
    //eg. formDesignDialog.show('name','add');
    return {
        show: function (actionID, actionResponse, action) {

            wFStateApprovalStatusActionID = actionID;

            var selRow = $(elementIDs.workflowCategoryMappingGridJQ).getGridParam("selrow");
            var rowData = $(elementIDs.workflowCategoryMappingGridJQ).getRowData(selRow);
            var mappingIsFinalized = (rowData.IsFinalized.trim() == "false") ? false : true;

            if (actionID == 1) {
                $(elementIDs.actionStatesDiv).hide(); $(elementIDs.actionEmailDiv).show();
                //set eleemnts based on whether the dialog is opened in add or edit mode
                $(elementIDs.actionEmailText).parent().removeClass('has-error');
                $(elementIDs.actionEmailError).parent().removeClass('has-error');
                $(elementIDs.actionEmailError).text(WorkFlowSettingsMessages.wfVersionActionEmailHelpText);
                $(elementIDs.wfStateApprovalStatusActionsDesignDialog).dialog('option', 'title', WorkFlowSettingsMessages.wfStateAPPActionEmailTitle);
                $(elementIDs.wfStateApprovalStatusActionsDesignDialog + ' button').prop("disabled", false);

                //set element values
                $(elementIDs.actionEmailText).val(actionResponse);
            }
            else {
                $(elementIDs.actionEmailDiv).hide(); $(elementIDs.actionStatesDiv).show();
                //set eleemnts based on whether the dialog is opened in add or edit mode
                $(elementIDs.actionStatesList).parent().removeClass('has-error');
                $(elementIDs.actionStatesError).text('');
                $(elementIDs.wfStateApprovalStatusActionsDesignDialog).dialog('option', 'title', WorkFlowSettingsMessages.wfStateAPPActionStateTitle);

                //disable/enable dropdown on basis of mapping isFinalized
                if (mappingIsFinalized) {
                    $(elementIDs.actionStatesList).prop("disabled", true);
                    $(elementIDs.wfStateApprovalStatusActionsDesignDialog + ' button').prop("disabled", true);
                } else {
                    $(elementIDs.actionStatesList).prop("disabled", false);
                    $(elementIDs.wfStateApprovalStatusActionsDesignDialog + ' button').prop("disabled", false);
                }

                //set element values                
                $(elementIDs.actionStatesList + " option").each(function (a, b) {
                    if ($(this).html() == actionResponse) $(this).attr("selected", "selected");
                });
            }
            if (actionID == 2) $(elementIDs.actionStatesLabel).text(WorkFlowSettingsMessages.wfVersionActionLabelForSuccessState); else $(elementIDs.actionStatesLabel).text(WorkFlowSettingsMessages.wfVersionActionLabelForFailureState);

            //open the dialog - uses jqueryui dialog
            $(elementIDs.wfStateApprovalStatusActionsDesignDialog).dialog("open");
        }
    }
}(); //invoked soon after loading
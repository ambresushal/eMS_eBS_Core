var folderMember = function () {
    var URLs = {
        updateUserRoles: '/WorkFlow/UpdateFolderVersionWorkflowStateUser',
    };
    var elementIDs = {
        assignFolderMemberbutton: '#assignFolderMemberbutton',
        //for dialog for user role assignment popup            
        userRoleDialog: "#UserRoleDialog",

        userAssignmentEditJQ: "userAssignmentEdit",
        watchGridJQ: "#watch",
        btnSaveUserAssignment: "#saveUserAssignment",
        assignFolderMemberbutton: '#assignFolderMemberbutton',
    };

    //maintains dialog state - add or edit
    var userAssignmentDialogState;

    function init() {
        $(elementIDs.userRoleDialog).dialog({
            autoOpen: false,
            resizable: false,
            closeOnEscape: true,
            height: 'auto',
            width: '830px',
            modal: true,
            position: 'center'
        });

        $(elementIDs.assignFolderMemberbutton).on('click', function () {
            folderMember.show("add");
        });

        //register event for Add/Edit button click on dialog
        $(elementIDs.userRoleDialog + ' button').on('click', function () {

            var selRowIds = jQuery('#' + elementIDs.userAssignmentEditJQ).jqGrid('getGridParam', 'selarrrow'), userList = new Array();
            if (selRowIds.length > 0) {
                $(selRowIds).each(function (i, id) {
                    var row = jQuery('#' + elementIDs.userAssignmentEditJQ).getRowData(id);

                    var userListRow = new Object();
                    userListRow.ApplicableTeamUserMapID = row.ApplicableTeamUserMapID;
                    userListRow.ApplicableTeamID = row.ApplicableTeamID;
                    userListRow.ApplicableTeamName = row.ApplicableTeamName;
                    userListRow.UserRoleID = row.UserRoleID;
                    userListRow.UserRoleName = row.UserRoleName;
                    userListRow.UserID = row.UserID;
                    userListRow.UserName = row.UserName;
                    userListRow.WFStateUserMapID = row.WFStateUserMapID;

                    userList[i] = userListRow;
                });

                // Check for TPA Analyst is included, if yes then remove it
                var removedTPAuserList = new Array();
                var isPortfolioFolder = false;
                var isTPAAnalystPresent = false;
                var tpaIdx = 0;
                for (var idx = 0; idx < userList.length; idx++) {
                    if (folderData.isPortfolio == "True") {
                        isPortfolioFolder = true;
                        //if (userList[idx].UserRoleID == 22)
                        //{ isTPAAnalystPresent = true; }
                        //else
                        //{
                            removedTPAuserList[tpaIdx] = userList[idx];
                            tpaIdx = tpaIdx + 1;
                        //}
                    }
                    else {
                        removedTPAuserList[tpaIdx] = userList[idx];
                        tpaIdx = tpaIdx + 1;
                    }
                   
                }
                var userRowsToAdd =
                   {
                       tenantId: 1,
                       assignedUserList: removedTPAuserList,
                       folderVersionId: folderData.folderVersionId
                   };
                //if (isPortfolioFolder && isTPAAnalystPresent && userList.length == 1) {
                //    messageDialog.show("Portfolio Folder cannot be assigned to users having TPA Analyst user role.");
                //}
                //else {
                   // if (isPortfolioFolder && isTPAAnalystPresent) {
                   //     messageDialog.show("Portfolio Folder cannot be assigned to users having TPA Analyst user role from the selected list.");
                   // }
                    ////ajax call to add/update
                    var promise = ajaxWrapper.postJSONCustom(URLs.updateUserRoles, userRowsToAdd);
                    ////register ajax success callback
                    promise.done(userRoleSuccess);
                    ////register ajax failure callback
                    promise.fail(showError);
                //}
            }
            else {
                messageDialog.show('Please select a row.');
            }
        });

    }

    //ajax success callback - for add/edit
    function userRoleSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            if (userAssignmentDialogState == "add") {
                messageDialog.show(DashboardMessages.userAssignedSuccess);
                isAddedUserAssigned = true;
            }
            else
                messageDialog.show(DashboardMessages.userUnAssignedSuccess);
        }
        else {
            if (xhr.Items != undefined) {
                var errorMsg = xhr.Items[0].Messages[0];
                if (errorMsg.indexOf(',') > 0) {
                    messageDialog.show(errorMsg.slice(0, -1) + " - " + DashboardMessages.userAssignedValidation);
                }
                else
                    messageDialog.show(errorMsg);
            }
            else messageDialog.show(Common.errorMsg);
        }

        //reload User Assignment grid        
        loadUserAssignmentGrid(folderData.folderVersionId);
        //reset dialog elements
        $(elementIDs.userRoleDialog + ' div').removeClass('has-error');
        $(elementIDs.userRoleDialog + ' .help-block').text('');
        $(elementIDs.userRoleDialog).dialog('close');
    }
    
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }


    $(document).ready(function () {
        //initialize the dialog after this js are loaded
        init();
    });

    return {
        show: function (action) {          
            userAssignmentDialogState = action;
            $(elementIDs.userRoleDialog).dialog('option', 'title', DashboardMessages.userDialogAssignment);
            loadUserAssignmentGrid(folderData.folderVersionId, userAssignmentDialogState);

            $(elementIDs.userRoleDialog).dialog("open");
        }
    }
}();

var URLs = {
    userAssignmentList: '/DashBoard/GetUserRoleAssignment?folderVersionID={folderVersionID}&userAssignmentDialogState={userAssignmentDialogState}',
     getDocumentLockStatus: '/FolderVersion/GetDocumentLockStatus?tenantId=1&folderId={folderId}'
};

var elementIDs = {
    userAssignmentEditGridJQ: "#userAssignmentEdit",
    userAssignmentEditGrid: "userAssignmentEdit"
};

function loadUserAssignmentGrid(folderVersionID, userAssignmentDialogState) {

    var getUserAssignmentList = URLs.userAssignmentList.replace(/\{folderVersionID\}/g, folderVersionID).replace(/\{userAssignmentDialogState\}/g, userAssignmentDialogState);

    //set column list for grid
    var colArray = ['', 'UserRole', '', 'User', 'ApplicableTeamID', 'Team', '', ''];

    //set column models
    var colModel = [];
    colModel.push({ name: 'UserRoleID', index: 'UserRoleID', editable: false, width: '265', hidden: true });
    colModel.push({ name: 'UserRoleName', index: 'UserRoleName', editable: false, width: '250' });
    colModel.push({ name: 'UserID', index: 'UserID', editable: true, cellEdit: true, width: '200', hidden: true });
    colModel.push({ name: 'UserName', index: 'UserName', editable: true, width: '250' });
    colModel.push({ name: 'ApplicableTeamID', index: 'ApplicableTeamID', editable: false, width: '265', hidden: true });
    colModel.push({ name: 'ApplicableTeamName', index: 'ApplicableTeamName', editable: false, width: '250' });
    colModel.push({ name: 'ApplicableTeamUserMapID', index: 'ApplicableTeamUserMapID', editable: false, width: '265', hidden: true });
    colModel.push({ name: 'WFStateUserMapID', index: 'WFStateUserMapID', editable: false, width: '265', hidden: true });

    //clean up the grid first - only table element remains after this
    $(elementIDs.userAssignmentEditGridJQ).jqGrid('GridUnload');

    //adding the pager element
    $(elementIDs.userAssignmentEditGridJQ).parent().append("<div id='p" + elementIDs.userAssignmentEditGrid + "'></div>");

    $(elementIDs.userAssignmentEditGridJQ).jqGrid({
        url: getUserAssignmentList,
        datatype: 'json',
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: '',
        height: 'auto',
        rowNum: 10,
        ignoreCase: true,
        loadonce: false,
        sortname: 'UserID',
        //autowidth: true,
        viewrecords: true,
        hidegrid: true,
        hiddengrid: false,
        altRows: true,
        multiselect: true,
        pager: '#p' + elementIDs.userAssignmentEditGrid,
        altclass: 'alternate',
        editurl: 'clientArray',
        //rowList: [10, 20, 30],
        loadComplete: function () {
            //if (isGridEdit)
            //    $('#p' + elementIDs.userAssignmentEditGrid + '_left').find('table').hide();
        },
        resizeStop: function (width, index) {
            autoResizing(elementIDs.userAssignmentGridJQ);
        },
        onSelectRow: function (id) {
        }
    });

    $(elementIDs.userAssignmentEditGridJQ + "_cb").css("width", "35px");


    var pagerElement = '#p' + elementIDs.userAssignmentEditGrid;
    $('#p' + elementIDs.userAssignmentEditGrid).find('input').css('height', '20px');

    $(elementIDs.userAssignmentEditGridJQ).jqGrid('filterToolbar', {
        stringResult: true, searchOnEnter: true,
        defaultSearch: "cn",
    });

    //remove default buttons
    $(elementIDs.userAssignmentEditGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
}
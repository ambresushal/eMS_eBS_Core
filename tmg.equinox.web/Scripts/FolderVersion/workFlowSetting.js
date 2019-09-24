var selectedUsersList = function () {
    this.userListData = null;
    var URLs = {
        applicableTeamList: '/Settings/GetApplicableTeamList?tenantId=1',
        userList: '/MasterList/GetOwnerList?tenantId=1',
        updateApplicableTeamUsers: '/Settings/UpdateApplicableTeamUserMap',
        getApplicableTeamUserList: '/Settings/GetApplicableTeamUserList?tenantId=1&teamId={teamId}'
    };

    var elementIDs = {
        addUserButton: "#addUser",
        removeUserButton: "#removeUser",
        selectedUserListGridJQ: "#selectedUserListGrid",
        userListGridJQ: "#tblUserList",
        applicableTeamId: "#applicableTeam",
        btnSaveApplicableTeamUsers: "#btnSaveApplicableTeamUsers"
    };

    function init() {
        $(document).ready(function () {
            $(elementIDs.addUserButton).addClass('disabled-button');
            $(elementIDs.removeUserButton).addClass('disabled-button');
            $(elementIDs.btnSaveApplicableTeamUsers).addClass('disabled-button');
            getApplicableTeamList();
            buildUserList();
            loadSelectedUserListGrid();
            //disableEnableManagerAssignment();
            $(elementIDs.applicableTeamId).find('option:first').prop('selected', 'selected');

        });
    }
    $(elementIDs.applicableTeamId).on('change', function () {
        $(elementIDs.selectedUserListGridJQ).jqGrid("clearGridData");
        $(elementIDs.userListGridJQ).jqGrid("clearGridData");
        //check role access permissions.
        checkWorkflowSettingsClaims(elementIDs, claims);
        var applicableTeamId = parseInt($(elementIDs.applicableTeamId).val());
        var url = URLs.getApplicableTeamUserList.replace(/\{teamId\}/g, applicableTeamId);
        var promise = ajaxWrapper.getJSON(url);
        //register ajax success callback
        promise.done(function (result) {
            if (result.length > 0) {
                for (i = 0 ; i < result.length ; i++) {
                    $(elementIDs.selectedUserListGridJQ).jqGrid('addRowData', (i), { UsersList: result[i].Username, IsManager: result[i].IsManager, UserID: parseInt(result[i].UserId) });
                }
            }
            var selectedUserList = $(elementIDs.selectedUserListGridJQ).getRowData();
            var userIds = $.map(selectedUserList, function (item) { return item.UserID; });
            //if (userIds.length > 0) {
            if ((applicableTeamId === 0 && result !== null && !result && result.length !== 0) || applicableTeamId !== 0)
                getUsersList(userIds);
            //}
        });

        //register ajax failure callback
        promise.fail(showError);

        //if (roleId == Role.HNESuperUser) {
        //    $(elementIDs.btnSaveApplicableTeamUsers).addClass('disabled-button');
        //}
    });

    $(elementIDs.btnSaveApplicableTeamUsers).on('click', function (event) {
        var applicableTeamId = parseInt($(elementIDs.applicableTeamId).val());
        var selectedUserList = $(elementIDs.selectedUserListGridJQ).getRowData();
        if (applicableTeamId == 0) {
            messageDialog.show(WorkFlowStateMessages.selectApplicableTeamMsg);
            return false;
        }
        if (selectedUserList.length == 0) {
            messageDialog.show(WorkFlowStateMessages.selectedUserListEmptyMsg);
            return false;
        }
        var checkedAtLeastOne = false;
        $('input[type="checkbox"]').each(function () {
            if (($(this).is(":checked")) && ($(this)[0].id.indexOf('tblUserList') == -1)) {
                checkedAtLeastOne = true;
            }
        });
        if (!checkedAtLeastOne) {
            messageDialog.show(WorkFlowStateMessages.atleastOneTeamManagerMsg);
            return false;
        }

        confirmDialog.show(Common.saveConfirmationMsg, function () {
            confirmDialog.hide();
            var saveData = {
                tenantId: 1,
                teamId: applicableTeamId,
                selectedUserListData: JSON.stringify(selectedUserList),
            };
            var promise = ajaxWrapper.postJSON(URLs.updateApplicableTeamUsers, saveData);
            promise.done(function (xhr) {
                if (xhr.Result === ServiceResult.SUCCESS) {
                    messageDialog.show(WorkFlowStateMessages.saveSuccessMsg);
                }
            });
            promise.fail(showError);
        });
    });

    $(elementIDs.addUserButton).on('click', function (event) {
        var ids = $(elementIDs.userListGridJQ).jqGrid('getGridParam', 'selarrrow');
        var selectedRows = $.extend(true, [], ids);
        if (ids.length > 0) {
            //Get the index of existing rows
            var index = $(elementIDs.selectedUserListGridJQ).jqGrid('getGridParam', 'records');
            index = index == 0 ? 1 : (index + 1)

            for (var i = 0; i < selectedRows.length; i++) {
                var row = $(elementIDs.userListGridJQ).jqGrid('getRowData', selectedRows[i]);
                if (row != null && row != undefined) {
                    $(elementIDs.selectedUserListGridJQ).jqGrid('addRowData', (index), { UsersList: row.UserName, IsManager: false, UserID: row.UserID });
                    $(elementIDs.userListGridJQ).jqGrid('delRowData', selectedRows[i]);
                    index++;
                }
            }
        }
        else {
            messageDialog.show(WorkFlowStateMessages.userSelectionToAddMsg);
        }
    });

    $(elementIDs.removeUserButton).on('click', function (event) {
        var selectedRow = $(elementIDs.selectedUserListGridJQ).jqGrid('getGridParam', 'selrow');

        if (selectedRow != null) {
            var selectedRowData = $(elementIDs.selectedUserListGridJQ).getRowData(selectedRow);
            var userId = selectedRowData['UserID'];
            var userName = selectedRowData['UsersList'];
            var isManager = selectedRowData['IsManager'];

            var index = $(elementIDs.userListGridJQ).jqGrid('getGridParam', 'records');
            index = index == 0 ? 1 : (index + 1)
            $(elementIDs.userListGridJQ).jqGrid('addRowData', index, { UserID: userId, UserName: userName });

            $(elementIDs.selectedUserListGridJQ).jqGrid('delRowData', selectedRow);

            if (isManager) {
                $(elementIDs.selectedUserListGridJQ).find("input:checkbox").each(function () {
                    $(this).prop("disabled", false);
                });
            }

        }
        else {
            messageDialog.show(WorkFlowStateMessages.userSelectionToRemoveMsg);
        }
    });

    function getApplicableTeamList() {
        var url = URLs.applicableTeamList;
        var promise = ajaxWrapper.getJSON(url);
        //register ajax success callback
        promise.done(loadAppliacbleTeamList);
        //register ajax failure callback
        promise.fail(showError);
    }

    function getUsersList(userIds) {
        //ajax call to add/update
        var promise = ajaxWrapper.getJSON(URLs.userList);
        //register ajax success callback

        promise.done(function (result) {
            loaduserlist(result, userIds);
        });
        //register ajax failure callback
        promise.fail(showError);
    }
    function loadAppliacbleTeamList(data) {
        for (var i = 0; i < data.length; i++) {
            $(elementIDs.applicableTeamId).append("<option value=" + data[i].Key + ">" + data[i].Value + "</option>");
        }
    }

    function loaduserlist(data, userIds) {
        var index = 0;
        $.grep(data, function (el) {
            if (!($.inArray((el.Key).toString(), userIds) !== -1)) {
                $(elementIDs.userListGridJQ).jqGrid('addRowData', index, { UserID: el.Key, UserName: el.Value });
                index++;
            }
        });
    }

    function buildUserList() {
        var colArray = ['User ID', 'User Name'];

        var colModel = [];
        colModel.push({ name: 'UserID', index: 'UserID', hidden: true });
        colModel.push({ name: 'UserName', index: 'UserName', editable: false, width: '300', align: 'left' });

        //clean up the grid first - only table element remains after this
        $(elementIDs.userListGridJQ).jqGrid('GridUnload');
        $(elementIDs.userListGridJQ).jqGrid({
            datatype: 'json',
            cache: false,
            altclass: 'alternate',
            colNames: colArray,
            colModel: colModel,
            caption: '',
            height: '320',
            rowNum: 10,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            hiddengrid: false,
            ignoreCase: true,
            altRows: true,
            multiselect: true,
        });
        $("#tblUserList_cb").css("width", "35px")
    }

    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
    function loadSelectedUserListGrid() {
        var colArray = ['Users List', 'Is Manager', 'UserID'];
        //set column models
        var colModel = [];
        colModel.push({ name: 'UsersList', index: 'UsersList', editable: false, width: '200', align: 'left' });
        colModel.push({
            name: 'IsManager', index: 'IsManager', align: 'left', editable: false, width: '100', align: 'center', edittype: 'checkbox',
            stype: 'select',
            searchoptions: {
                sopt: ['eq', 'ne'],
                value: ':Any;true:Yes;false:No'
            }, formatter: this.formatIsManagerColumn, unformat: this.unFormatColumn
        });
        colModel.push({ name: 'UserID', index: 'UserID', hidden: true });

        //clean up the grid first - only table element remains after this
        $(elementIDs.selectedUserListGridJQ).jqGrid('GridUnload');
        $(elementIDs.selectedUserListGridJQ).jqGrid({
            datatype: 'json',
            cache: false,
            altclass: 'alternate',
            colNames: colArray,
            colModel: colModel,
            caption: '',
            height: '320',
            rowNum: 10,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            hiddengrid: false,
            ignoreCase: true,
            altRows: true
        });
    }
    formatIsManagerColumn = function (cellValue, options, rowObject) {
        var result;
        if (rowObject.IsManager == true)
            result = '<input type="checkbox"' + ' class="chk-ismanager" id = manager' + rowObject.UserID + ' name=' + rowObject.UserID + 'manager' + ' checked />';
        else
            result = '<input type="checkbox"' + ' class="chk-ismanager" id = manager' + rowObject.UserID + ' name=' + rowObject.UserID + 'manager' + '/>';
        return result;
    }
    unFormatColumn = function (cellValue, options) {
        var result;
        result = $(this).find('#' + options.rowId).find('input').prop('checked');
        return result;
    }
    init();

    function disableEnableManagerAssignment() {
        $(document).on("change", ".chk-ismanager", function (event) {
            var selectedCheckBoxId = $(this).attr('id');
            if (this.checked) {
                $(elementIDs.selectedUserListGridJQ).find("input:checkbox").each(function () {
                    if ($(this).attr('id') != selectedCheckBoxId) {
                        $(this).prop("disabled", true);
                        if ($(this).prop("checked") == true) {
                            $(this).prop('checked', false);
                        }
                    }
                });
            }
            else {
                $(elementIDs.selectedUserListGridJQ).find("input:checkbox").each(function () {
                    $(this).prop("disabled", false);
                });
            }
        });
    }
}();
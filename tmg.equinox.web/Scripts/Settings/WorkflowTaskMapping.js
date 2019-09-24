var selectedWorkflowList = function () {
    this.userListData = null;
    var URLs = {
        getWorkFlowStateList: '/WorkFlowTaskMapping/getworkflowList?tenantId=1',
        taskList: '/WorkFlowTaskMapping/GetNonSelectedTaskList?tenantID=1',
        updateApplicableWftask: '/WorkFlowTaskMapping/UpdateApplicableWfTaskrMap',
        getApplicableWfTaskList: '/WorkFlowTaskMapping/GetApplicableWfTaskList?tenantId=1&WfstateId={WfstateId}'
    };

    var elementIDs = {
        addTaskButton: "#addTask",
        removeTaskButton: "#removeTask",
        selectedTaskListGridJQ: "#selectedTaskListGrid",
        TaskListGridJQ: "#tblTaskList",
        applicableWorkflowStateId: "#applicableWorkflowState",
        btnSaveApplicableTask: "#btnSaveApplicableTask"
    };

    function init() {
        $(document).ready(function () {
            //$(elementIDs.addTaskButton).addClass('disabled-button');
            //$(elementIDs.removeTaskButton).addClass('disabled-button');
            //$(elementIDs.btnSaveApplicableTask).addClass('disabled-button');
            getWorkflowList();
            buildTaskList();
            loadSelectedTaskListGrid();
            //$(elementIDs.applicableWorkflowStateId).find('option:first').prop('selected', 'selected');
        });
    }
    $(elementIDs.applicableWorkflowStateId).on('change', function () {
        $(elementIDs.selectedTaskListGridJQ).jqGrid("clearGridData");
        $(elementIDs.TaskListGridJQ).jqGrid("clearGridData");
        //check role access permissions.
        checkWorkflowSettingsClaims(elementIDs, claims);
        var applicableWorkflowStateId = parseInt($(elementIDs.applicableWorkflowStateId).val());
        var url = URLs.getApplicableWfTaskList.replace(/\{WfstateId\}/g, applicableWorkflowStateId);
        var promise = ajaxWrapper.getJSON(url);
        //  register ajax success callback
        promise.done(function (result) {
            if (result.length > 0) {
                for (i = 0 ; i < result.length ; i++) {
                    $(elementIDs.selectedTaskListGridJQ).jqGrid('addRowData', (i), { TaskList: result[i].Value, TaskID: result[i].Key });
                }
            }
            var selectedTaskList = $(elementIDs.selectedTaskListGridJQ).getRowData();
            var taskIds = $.map(selectedTaskList, function (item) { return item.TaskID; });
            if ((applicableWorkflowStateId === 0 && result !== null && !result && result.length !== 0) || applicableWorkflowStateId !== 0)
                getTasksList(taskIds);
        });

        //register ajax failure callback
        promise.fail(showError);
    });

    $(elementIDs.btnSaveApplicableTask).on('click', function (event) {
        var applicableWorkflowStateId = parseInt($(elementIDs.applicableWorkflowStateId).val());
        var selectedTaskList = $(elementIDs.selectedTaskListGridJQ).getRowData();
        if (applicableWorkflowStateId == 0) {
            messageDialog.show(WorkFlowStateMessages.selectWorkflowStateMsg);
            return false;
        }
        if (selectedTaskList.length == 0) {
            messageDialog.show(WorkFlowStateMessages.selectedTaskListEmptyMsg);
            return false;
        }

        confirmDialog.show(Common.saveConfirmationMsg, function () {
            confirmDialog.hide();
            var saveData = {
                tenantId: 1,
                WfstateId: applicableWorkflowStateId,
                selectedTaskListData: JSON.stringify(selectedTaskList),
            };
            var promise = ajaxWrapper.postJSON(URLs.updateApplicableWftask, saveData);
            promise.done(function (xhr) {
                if (xhr.Result === ServiceResult.SUCCESS) {
                    messageDialog.show(WorkFlowTaskMapMessages.WorkFlowTaskMapsaveSuccessMsg);
                }
            });
            promise.fail(showError);
        });
    });

    $(elementIDs.addTaskButton).on('click', function (event) {
        var ids = $(elementIDs.TaskListGridJQ).jqGrid('getGridParam', 'selarrrow');
        var selectedRows = $.extend(true, [], ids);
        if (ids.length > 0) {
            //Get the index of existing rows
            var index = $(elementIDs.selectedTaskListGridJQ).jqGrid('getGridParam', 'records');
            index = index == 0 ? 1 : (index + 1)

            for (var i = 0; i < selectedRows.length; i++) {
                var row = $(elementIDs.TaskListGridJQ).jqGrid('getRowData', selectedRows[i]);
                if (row != null && row != undefined) {

                    $(elementIDs.selectedTaskListGridJQ).jqGrid('addRowData', (index), { TaskList: row.TaskDescription, TaskID: row.TaskID });
                    $(elementIDs.TaskListGridJQ).jqGrid('delRowData', selectedRows[i]);
                    index++;
                }
            }
        }
        else {
            messageDialog.show(WorkFlowTaskMapMessages.taskSelectionToAddMsg);
        }
    });

    $(elementIDs.removeTaskButton).on('click', function (event) {
        var ids = $(elementIDs.selectedTaskListGridJQ).jqGrid('getGridParam', 'selarrrow');
        var selectedRow = $(elementIDs.selectedTaskListGridJQ).jqGrid('getGridParam', 'selarrrow');
        var selectedRows = $.extend(true, [], ids);
        if (ids.length > 0) {
            if (selectedRow != null) {
                var index = $(elementIDs.TaskListGridJQ).jqGrid('getGridParam', 'records');
                index = index == 0 ? 1 : (index + 1)
                for (var i = 0; i < selectedRows.length; selectedRows.length--) {
                    var selectedRowData = $(elementIDs.selectedTaskListGridJQ).getRowData(selectedRow[i]);
                    var taskId = selectedRowData['TaskID'];
                    var taskdescription = selectedRowData['TaskList'];
                    var row = $(elementIDs.selectedTaskListGridJQ).jqGrid('getRowData', selectedRows[i]);
                    $(elementIDs.TaskListGridJQ).jqGrid('addRowData', (index), { TaskID: taskId, TaskDescription: taskdescription });

                    $(elementIDs.selectedTaskListGridJQ).jqGrid('delRowData', selectedRow[i]);
                    index++;
                }
            }

        }
        else {
            messageDialog.show(WorkFlowTaskMapMessages.taskSelectionToRemoveMsg);
        }
    });

    function getWorkflowList() {
        var url = URLs.getWorkFlowStateList;
        var promise = ajaxWrapper.getJSON(url);
        //register ajax success callback
        promise.done(loadWorkflowList);
        //register ajax failure callback
        promise.fail(showError);
    }

    function getTasksList(taskIds) {
        //ajax call to add/update
        var promise = ajaxWrapper.getJSON(URLs.taskList);
        //register ajax success callback

        promise.done(function (result) {
            loadtasklist(result, taskIds);
        });
        //register ajax failure callback
        promise.fail(showError);
    }
    function loadWorkflowList(data) {
        $(elementIDs.applicableWorkflowStateId).html('');
        $(elementIDs.applicableWorkflowStateId).append("<option value='0'>Select WorkFlow State</option>");
        for (var i = 0; i < data.length; i++) {
            $(elementIDs.applicableWorkflowStateId).append("<option value=" + data[i].Key + ">" + data[i].Value + "</option>");
        }
    }

    function loadtasklist(data, taskIds) {
        var index = 0;
        $.grep(data, function (el) {
            if (!($.inArray((el.Key).toString(), taskIds) !== -1)) {
                $(elementIDs.TaskListGridJQ).jqGrid('addRowData', index, { TaskID: el.Key, TaskDescription: el.Value });
                index++;
            }
        });
    }
    function loadselectedtasklist(data, taskId) {
        var index = 0;
        $.grep(data, function (el) {
            if (!($.inArray((el.Key).toString(), taskId) !== -1)) {
                $(elementIDs.selectedTaskListGridJQ).jqGrid('addRowData', index, { TaskID: el.Key, TaskDescription: el.Value });
                index++;
            }
        });
    }
    function buildTaskList() {
        var colArray = ['Task ID', 'Task Description'];

        var colModel = [];
        colModel.push({ name: 'TaskID', index: 'TaskID', hidden: true });
        colModel.push({ name: 'TaskDescription', index: 'TaskDescription', editable: false, width: '300', align: 'left' });

        //clean up the grid first - only table element remains after this
        $(elementIDs.TaskListGridJQ).jqGrid('GridUnload');
        $(elementIDs.TaskListGridJQ).jqGrid({
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
        $("#tblTaskList_cb").css("width", "35px")
    }

    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
    function loadSelectedTaskListGrid() {
        var colArray = ['Task Description', 'WFStateTaskID'];
        //set column models
        var colModel = [];
        colModel.push({ name: 'TaskList', index: 'TaskList', editable: false, width: '200', align: 'left' });
        colModel.push({ name: 'TaskID', index: 'TaskID', hidden: true });

        //clean up the grid first - only table element remains after this
        $(elementIDs.selectedTaskListGridJQ).jqGrid('GridUnload');
        $(elementIDs.selectedTaskListGridJQ).jqGrid({
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
            multiselect: true
        });
        $("#selectedTaskListGrid_cb").css("width", "35px")
    }

    $(".WORKFLOWTASKMAPPING").unbind('click');
    $(".WORKFLOWTASKMAPPING").click(function () {
        init();
        //show the selected tab
        $(elementIDs.settingsTabJQ).tabs({
            selected: 6
        });
    });
}();
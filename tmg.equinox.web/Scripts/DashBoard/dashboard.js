
var initTabs = function () {
    var dasInstance = this;
    var taskGridViewMode = 'Open';
    var activeTab = 'workQueue';
    var selectedTaskFolderversionWorkFlowState = '';
    var selectedTaskstate = '';
    $(document).ready(function () {


        function formUpdates() {

            var URLs = {
                formUpdatesList: '/DashBoard/GetFormUpdatesList'
            };

            var elementIDs = {
                //table element for the Form Update Grid 
                formUpdatesGrid: 'formUpdates',
                //with hash for use with jQuery
                formUpdatesGridJQ: '#formUpdates',
            };

            function init() {
                $(document).ready(function () {
                    //load the Document update grid
                    loadFormUpdatesGrid();
                });
            }

            function loadFormUpdatesGrid() {
                //set column list for grid
                var colArray = ['Document Design Name', 'Version Number', 'Effective Date', 'Release Date', 'Comments'];

                //set column models
                var colModel = [];
                colModel.push({ name: 'FormName', index: 'FormName', editable: false, align: 'left' });
                colModel.push({ name: 'VersionNumber', index: 'VersionNumber', editable: false, align: 'right' });
                colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, align: 'center', formatter: 'date', sorttype: "date", searchoptions: { sopt: ["eq"] }, formatoptions: JQGridSettings.DateFormatterOptions });
                colModel.push({ name: 'ReleaseDate', index: 'ReleaseDate', editable: false, align: 'center', formatter: 'date', sorttype: "date", searchoptions: { sopt: ["eq"] }, formatoptions: JQGridSettings.DateFormatterOptions });
                colModel.push({ name: 'Comments', index: 'Comments', editable: false, align: 'left' });

                //clean up the grid first - only table element remains after this
                $(elementIDs.formUpdatesGridJQ).jqGrid('GridUnload');

                //adding the pager element
                $(elementIDs.formUpdatesGridJQ).parent().append("<div id='p" + elementIDs.formUpdatesGrid + "'></div>");

                var url = URLs.formUpdatesList;

                $(elementIDs.formUpdatesGridJQ).jqGrid({
                    url: url,
                    datatype: 'json',
                    cache: false,
                    colNames: colArray,
                    colModel: colModel,
                    caption: 'Document Design Updates',
                    rowNum: 10,
                    height: '150',
                    autowidth: true,
                    loadonce: true,
                    viewrecords: true,
                    ignoreCase: true,
                    hidegrid: true,
                    hiddengrid: false,
                    altRows: true,
                    pager: '#p' + elementIDs.formUpdatesGrid,
                    altclass: 'alternate',
                    //this is added for pagination.
                    rowList: [10, 20, 30],
                    onPaging: function (pgButton) {
                        if (pgButton === "user" && !IsEnteredPageExist(elementIDs.formUpdatesGrid)) {
                            return "stop";
                        }
                    },
                    resizeStop: function (width, index) {
                        autoResizing(elementIDs.formUpdatesGridJQ);
                    },
                    jsonReader: {
                        page: function (obj) { return obj.records == 0 || obj.records == undefined ? "0" : obj.page; },
                    }
                });
                var pagerElement = '#p' + elementIDs.formUpdatesGrid;
                $('#p' + elementIDs.formUpdatesGrid).find('input').css('height', '20px');

                $(elementIDs.formUpdatesGridJQ).jqGrid('filterToolbar', {
                    stringResult: true, searchOnEnter: true,
                    defaultSearch: "cn",
                });

                //remove default buttons
                $(elementIDs.formUpdatesGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

            }

            //initialization of the DashBoard Grid when the Form Update function is loaded in browser and invoked
            init();
            return {
                loadFormUpdates: function () {
                    loadFormUpdatesGrid();
                }
            }
        }
        //workQueue();

        $(".nav-pills a").click(function () {
            if ($(this).attr("href") == "#workQueueTab") {
                $(this).tab('show');
                activeTab = 'workQueue';
                workQueue();
                $("#workQueue").setGridWidth($(".tab-content")[0].offsetWidth);
            }
            if ($(this).attr("href") == "#watchListTab") {
                $(this).tab('show');
                activeTab = 'watchList';
                watch();
                $("#watch").setGridWidth($(".tab-content")[0].offsetWidth);
            }
            if ($(this).attr("href") == "#notificationTab") {
                $(this).tab('show');
                formUpdates();
                $("#formUpdates").setGridWidth($(".tab-content")[0].offsetWidth);
            }
        });

        $('input:radio[name=viewTasks]').on('click', function () {
            taskGridViewMode = $(this).val();
            if (activeTab == "workQueue")
                workQueue();
            else
                watch();
        });
        var url = window.location.href;
        if (url.toLowerCase().includes("dashboard")) {
            $("#btnRoutingTask").hide();
            workQueue();
            watch();
        }

    });

    function watch() {
        var taskFolderVersionId = WorkFlowState.TaskFolderVersionId;
        var URLs = {
            watchList: '/DashBoard/GetWatchList?isViewInterested={isViewInterested}&viewMode={viewMode}&taskFolderVersionId={taskFolderVersionId}',
            folderVersionList: '/FolderVersion/Index?tenantId={tenantId}&folderVersionId={folderVersionId}&folderId={folderId}&foldeViewMode={foldeViewMode}&mode={mode}',
            LockStatusUrl: '/FolderVersion/GetFolderLockStatus?tenantId={tenantId}&folderId={folderId}',
            OverrideLockUrl: '/FolderVersion/OverrideFolderLock?tenantId={tenantId}&folderId={folderId}',


            exportWatchListToExcel: '/DashBoard/ExportWatchListToExcel?isViewInterested={isViewInterested}&viewMode={viewMode}',
            saveInterstedFolderVersion: '/DashBoard/SaveInterstedFolderVersion?folderVersionId={folderVersionId}&currentUserName={currentUserName}',
            deleteInterstedFolderVersion: '/DashBoard/DeleteInterestedFolderVersion?folderVersionId={folderVersionId}',

            userAssignmentList: '/DashBoard/GetUserRoleAssignment?folderVersionID={folderVersionID}&userAssignmentDialogState={userAssignmentDialogState}',
            updateUserRoles: '/WorkFlow/UpdateFolderVersionWorkflowStateUser',
            deleteUserRoles: '/WorkFlow/DeleteFolderVersionWorkflowStateUser',
            checkIsManager: '/DashBoard/CheckIsManager',
            getFolderVersionList: '/FolderVersion/GetFolderVersionList',

            saveInterestedTask: '/DashBoard/SaveInterstedTask?id={id}',
            saveInterestedAllTask: '/DashBoard/SaveInterstedAllTask?isViewInterested={isViewInterested}&viewMode={viewMode}&value={value}',
            removeInterestedTask: '/DashBoard/RemoveInterstedTask?id={id}',
            getRepeateUserSettings: '/Settings/GetFormDesignUserSetting?formDesignVersionId={formDesignVersionId}',
            SubwatchQueueList: '/DashBoard/GetSubwatchQueueList?',
            commentHistory: '/DashBoard/GetCommentList?Taskid={Taskid}'

        };

        var elementIDs = {
            //table element for the Watch Grid 
            watchGrid: 'watch',
            //with hash for use with jQuery
            watchGridJQ: '#watch',
            btnUnlockJQ: '#btnUnlock',
            markInterestedCheckboxJQ: '[id^=selectcheckbox_]',
            markInterstedCheckbox: '#selectcheckbox',

            markInterstedSelectAllCheckbox: 'selectallcheckbox',
            markInterstedSelectAllCheckboxJQ: '#selectallcheckbox',

            //elements for user role assignment
            viewAssignedUsersGrid: "viewAssignedUsers",
            viewAssignedUsersGridJQ: "#viewAssignedUsers",
            userAssignmentUpdateGrid: "userAssignmentUpdate",
            userAssignmentUpdateGridJQ: "#userAssignmentUpdate",

            userAssignUrl: ".ASSIGNMENT",
            btnSaveUserAssignment: "#assignUser",

            userAssignUnassignDialog: "#UserAssignUnassignDialog",
            folderViewDialog: '#folderViewDialog',
            btnClassicFolderView: '#btnClassicFolderView1',
            btnSOTView: '#btnSOTView1',

            taskAssignmentWatchlist: "#taskAssignmentWatchlist",
            taskAssigWatchFolderList: "#taskAssigWatchFolderList",
            taskAssigWatchVersionList: "#taskAssigWatchVersionList",
            taskAssigWatchWorkFlowStatusList: "#taskAssigWatchWorkFlowStatusList",
            taskAssigWatchPlansList: "#taskAssigWatchPlansList",
            taskAssigWatchTasksList: "#taskAssigWatchTasksList",
            taskAssigWatchAssignUserList: "#taskAssigWatchAssignUserList",
            taskAssigWatchTaskStatus: "#taskAssigWatchTaskStatus",
            taskAssigWatchOrder: "#taskAssigWatchOrder",
            taskAssigWatchDuration: "#taskAssigWatchDuration",
            taskAssigWatchDurationSpan: "#taskAssigWatchDurationSpan",
            taskAssigWatchStartdate: "#taskAssigWatchStartdate",
            taskAssigWatchDuedate: "#taskAssigWatchDuedate",
            taskAssigWatchSaveBtn: "#taskAssigWatchSaveBtn",
            taskAssigWatchCancelBtn: "#taskAssigWatchCancelBtn",
            createNewTasksBtn: "#createNewTasksBtn",
            newTaskCreationDialog: "#newTaskCreationDialog",
            newTaskDescription: "#newTaskDescription",
            newTaskCreationCancelBtn: "#newTaskCreationCancelBtn",
            newTaskCreationSaveBtn: "#newTaskCreationSaveBtn",
            taskAssigWatchViewList: "#taskAssigWatchViewList",
            taskAssigWatchSectionList: "#taskAssigWatchSectionList",

            taskAssigWatchViewListSpan: "#taskAssigWatchViewListSpan",
            taskAssigWatchSectionListSpan: "#taskAssigWatchSectionListSpan",
            taskAssigWatchFolderListSpan: "#taskAssigWatchFolderListSpan",
            taskAssigWatchVersionListSpan: "#taskAssigWatchVersionListSpan",
            taskAssigWatchWorkFlowStatusListSpan: "#taskAssigWatchWorkFlowStatusListSpan",
            taskAssigWatchPlansListSpan: "#taskAssigWatchPlansListSpan",
            taskAssigWatchTasksListSpan: "#taskAssigWatchTasksListSpan",
            createNewTasksBtnSpan: "#createNewTasksBtnSpan",
            taskAssigWatchAssignUserListSpan: "#taskAssigWatchAssignUserListSpan",
            taskAssigWatchTaskStatusSpan: "#taskAssigWatchTaskStatusSpan",
            taskAssigWatchDuedateSpan: "#taskAssigWatchDuedateSpan",
            taskAssigWatchStartdateSpan: "#taskAssigWatchStartdateSpan",
            newTaskDescriptionSpan: "#newTaskDescriptionSpan",
            PlanTaskMode: "#PlanTaskMode",
            HiddenParameter: "#HiddenParameter",
            taskAssigWatchTaskStatusLabel: "#taskAssigWatchTaskStatusLabel",
            taskAssigWatchTaskStatus: "#taskAssigWatchTaskStatus",
            taskAssigWatchTaskStatusRow: "#taskAssigWatchTaskStatusRow",
            cancelWatchDflag: "#cancelWatchDflag",
            NewTaskSaved: "#NewTaskSaved",
            IsTaskInfoModifed: "#IsTaskInfoModifed",
            MappingRowIDForTaskHDVal: "#MappingRowIDForTaskHDVal",
            ChangeEventTriggerByCode: "#ChangeEventTriggerByCode",
            IsTaskLateStatus: "#IsTaskLateStatus",
            taskAssigWatchCommentsJQ: "#taskAssigWatchComments",
            FolderVersionsListHDVal: "#FolderVersionsListHDVal",
            newWatchcommentDialog: "#newWatchcommentDialog",
            newWatchcommentJQ: "#newWatchcomment",
            btnsaveWatchcommentJQ: "#btnsaveWatchcomment",
            Documentname: '#Documentname',
            taskAssigWatchOrder: "#taskAssigWatchOrder",
            taskAssigWatchAttachment: "#uploadWatchAttachment",
            taskAssigWatchChangeAttachment: "#uploadWatchAttachmentBtn",
            taskAssigWatchLabelAttachment: "#watchAttachmentName",
            optradioWatchPortfolio: "#optradioWatchPortfolio",
            taskAssigWatchAccountListLabelDiv: "#taskAssigWatchAccountListLabelDiv",
            taskAssigWatchAccountListDiv: "#taskAssigWatchAccountListDiv",
            taskAssigWatchAccountList: "#taskAssigWatchAccountList",
            watchAttachmentName: "#watchAttachmentName",
            taskAssigWatchAttachmentChangeDiv: "#taskAssigWatchAttachmentChangeDiv",
            taskAssigWatchEstTime: "#taskAssigWatchEstTime",
            taskAssigWatchActualTime: "#taskAssigWatchActualTime",
            taskAssigWatchEstTimeSpan: "#taskAssigWatchEstTimeSpan",
            taskAssigWatchActualTimeSpan: "#taskAssigWatchActualTimeSpan",
            uploadWatchCommentAttachment: "#uploadWatchCommentAttachment",
            taskCommentsDailog: "#taskCommentsDailog",
            taskCommentsGrid: "#taskCommentsGrid",
            newWatchcommentSpan: "#newWatchcommentSpan",
            taskAssigWatchAccountListSpan: "#taskAssigWatchAccountListSpan"
        };
        var FolderVersionsList;
        var MappingRowIDForTask;
        var IsPlanNewTaskUpdate;
        var UPTFolderID;
        var UPTFolderVersionID;
        var UPTWFStateID;
        var UPTFormInstanceId;
        var UPTTaskID;
        var UPAttachment;
        var ALLPlans = "All Plans";
        var ALLViews = "All Views";
        var ALLSections = "All Sections";
        var SaveOperationDoneForNewTask = false;
        var IsPlanTaskInfoUpdated = false;
        var UPViewID;
        var UPSectionID;
        var UPOrder;

        function showError(xhr) {
            if (xhr.status == 999)
                this.location = '/Account/LogOn';
            else
                messageDialog.show(JSON.stringify(xhr));
        }

        function init() {
            // Select Folder View Dialog
            $(elementIDs.folderViewDialog).dialog({
                autoOpen: false,
                height: 600,
                width: 400,
                modal: true,
            });

            $(elementIDs.taskAssignmentWatchlist).dialog({
                autoOpen: false,
                height: 500,
                width: 850,
                modal: true,
            });
            $(elementIDs.newTaskCreationDialog).dialog({
                autoOpen: false,
                height: 500,
                width: 450,
                modal: true,
            });
            $(elementIDs.taskCommentsDailog).dialog({
                autoOpen: false,
                height: 450,
                width: 700,
                modal: true,
            });
            $(elementIDs.newWatchcommentDialog).dialog({
                autoOpen: false,
                height: 500,
                width: 450,
                modal: true,
            });
            $(elementIDs.taskAssigWatchStartdate).datepicker({
                dateFormat: 'mm/dd/yy',
                changeMonth: true,
                changeYear: true,
                yearRange: 'c-61:c+20',
                showOn: "both",
                //CalenderIcon path declare in golbalvariable.js
                buttonImage: Icons.CalenderIcon,
                buttonImageOnly: true,
            }).parent().find('img').css('margin-top', '-25px').css('margin-right', '5px');

            $(elementIDs.taskAssigWatchDuedate).datepicker({
                dateFormat: 'mm/dd/yy',
                changeMonth: true,
                changeYear: true,
                yearRange: 'c-61:c+20',
                showOn: "both",
                //CalenderIcon path declare in golbalvariable.js
                buttonImage: Icons.CalenderIcon,
                buttonImageOnly: true,
            }).parent().find('img').css('margin-top', '-25px').css('margin-right', '5px');

            $('div' + elementIDs.taskAssignmentWatchlist).on('dialogclose', function (event) {
                var isModifed = $(elementIDs.IsTaskInfoModifed).text();
                var isCancelDone = $(elementIDs.cancelWatchDflag).text();
                //if (isModifed == "TaskInfoModifed" && isCancelDone == "CancelUnDone")
                //{
                //    yesNoConfirmDialog.show(DashBoard.confirmTaskAssignmentSaveMsg, function (e) {
                //        if (e) {
                //            yesNoConfirmDialog.hide();
                //            $(elementIDs.taskAssignmentWatchlist).dialog("open");
                //            SaveTaskAssignmentOperation();
                //        }
                //        else {
                //            yesNoConfirmDialog.hide();
                //        }
                //    });
                //}
            });


            $(elementIDs.taskAssigWatchCancelBtn).click(function () {
                taskAssignmentWatchlistDialogClose();
            });

            taskAssignmentWatchlistDialogClose = function () {
                //yesNoConfirmDialog.show(Common.closeConfirmationMsg, function (e) {
                //    if (e) {
                //        yesNoConfirmDialog.hide();
                //        $(elementIDs.taskAssignmentWatchlist).dialog("close");
                //    }
                //    else {
                //        yesNoConfirmDialog.hide();
                //    }
                //});
            }

            $(elementIDs.taskAssignmentWatchlist + ' button').off('click').on('click', function () {
                if (this.id == "taskAssigWatchSaveBtn") {
                    SaveTaskAssignmentOperation();
                    //if (selectedTaskstate != $(elementIDs.taskAssigWatchTaskStatus + ' option:selected').text()) {
                    //if (selectedTaskFolderversionWorkFlowState == null || selectedTaskFolderversionWorkFlowState == $(elementIDs.taskAssigWatchWorkFlowStatusList + ' option:selected').text()) {
                    //    SaveTaskAssignmentOperation();
                    //} else {
                    //    messageDialog.show("Folder version workflow state does not match with task workflow state, so you can not update the task status.");
                    //}
                    //}
                }
                else if (this.id == "createNewTasksBtn") {
                    SaveOperationDoneForNewTask = false;
                    $(elementIDs.NewTaskSaved).text("SaveUnDone");
                    $(elementIDs.newTaskDescription).parent().addClass('has-error');
                    $(elementIDs.newTaskDescription).removeClass('form-control');
                    $(elementIDs.newTaskDescriptionSpan).text('');
                    $(elementIDs.newTaskDescription).val('');
                    $(elementIDs.newTaskCreationDialog).dialog('option', 'title', "New Task Creation");
                    $(elementIDs.newTaskCreationDialog).dialog("open");
                }
                else {
                    $(elementIDs.taskAssignmentWatchlist).dialog("close");
                    selectedTaskstate = '';
                    $(elementIDs.cancelWatchDflag).text("CancelDone");
                }
            });
            $(elementIDs.newTaskCreationDialog + ' button').on('click', function () {
                if (this.id == "newTaskCreationSaveBtn") {
                    var taskSavedFlag = $(elementIDs.NewTaskSaved).text();

                    if (taskSavedFlag == "SaveUnDone") {
                        $(elementIDs.NewTaskSaved).text("SaveDone");
                        var taskDesc = $(elementIDs.newTaskDescription).val();
                        var wfStateId = $(elementIDs.taskAssigWatchWorkFlowStatusList).val();
                        SaveOperationDoneForNewTask = true;
                        var urlCheckTaskAlreadyExist = "/Task/CheckTasksAlreadyExists?strTaskName={strTaskName}"
                        var promiseCheckTaskAlreadyExist = ajaxWrapper.postJSON(urlCheckTaskAlreadyExist.replace(/{strTaskName}/g, taskDesc));
                        promiseCheckTaskAlreadyExist.done(function (tasklist) {
                            if (tasklist == true) {
                                //    alert(TasksListMessages.taskExistsError);
                                $(elementIDs.newTaskDescription).parent().addClass('has-error');
                                $(elementIDs.newTaskDescription).addClass('form-control');
                                $(elementIDs.newTaskDescriptionSpan).text(TasksListMessages.taskExistsError);
                                $(elementIDs.NewTaskSaved).text("SaveUnDone");
                            }
                            else {
                                var performOperation = true;
                                //   var strErrorList = "Fields required for :";
                                if (wfStateId == null || wfStateId == undefined || wfStateId == "0") {
                                    $(elementIDs.taskAssigWatchWorkFlowStatusList).parent().addClass('has-error');
                                    $(elementIDs.taskAssigWatchWorkFlowStatusList).addClass('form-control');
                                    $(elementIDs.taskAssigWatchWorkFlowStatusListSpan).text(DashBoard.selectWorkFlowStateRequiredMsg);
                                    messageDialog.show(DashBoard.selectWorkFlowStateRequiredMsg);
                                    //     strErrorList = strErrorList + " WorkflowStatus";
                                    performOperation = false;
                                }
                                else {
                                    $(elementIDs.taskAssigWatchWorkFlowStatusList).parent().addClass('has-error');
                                    $(elementIDs.taskAssigWatchWorkFlowStatusList).removeClass('form-control');
                                    $(elementIDs.taskAssigWatchWorkFlowStatusListSpan).text('');
                                    performOperation = true;

                                    var taskDesc = $(elementIDs.newTaskDescription).val();
                                    if (taskDesc == null || taskDesc == undefined || taskDesc == "0" || taskDesc == "") {
                                        $(elementIDs.newTaskDescription).parent().addClass('has-error');
                                        $(elementIDs.newTaskDescription).addClass('form-control');
                                        $(elementIDs.newTaskDescriptionSpan).text(DashBoard.entertaskdescriptionMsg);
                                        //       strErrorList = strErrorList + ", Tasks";
                                        performOperation = false;
                                    }
                                    else {
                                        $(elementIDs.newTaskDescription).parent().addClass('has-error');
                                        $(elementIDs.newTaskDescription).removeClass('form-control');
                                        $(elementIDs.newTaskDescriptionSpan).text('');
                                        performOperation = true;
                                    }
                                }

                                if (performOperation == true) {
                                    var input = {
                                        wfStateId: wfStateId,
                                        taskDescription: taskDesc
                                    }

                                    var url = '/WorkFlowTaskMapping/SaveTaskAndWFStateTaskMapping';
                                    var promise = ajaxWrapper.postJSON(url, input);
                                    promise.done(function (list) {
                                        $(elementIDs.newTaskDescription).val("").change();
                                        fillWatchTasksDropDown();

                                        //alert("Saving New Task Creation done");
                                    });

                                    promise.fail(showError);
                                    $(elementIDs.newTaskCreationDialog).dialog("close");
                                }
                                else {
                                    $(elementIDs.NewTaskSaved).text("SaveUnDone");
                                    //messageDialog.show(strErrorList);
                                }
                            }
                        });
                        promiseCheckTaskAlreadyExist.fail(showError);
                    }
                }
                else {
                    $(elementIDs.newTaskCreationDialog).dialog("close");
                }
            });
            $($(elementIDs.newTaskDescription)).change(function () {
                SaveOperationDoneForNewTask = false;
            });
            $(elementIDs.taskAssigWatchFolderList).off('change').change(function () {
                var eventTrigResult = $(elementIDs.ChangeEventTriggerByCode).text();
                if (eventTrigResult == false || eventTrigResult == "false") {
                    IsPlanTaskInfoUpdated = true;
                    $(elementIDs.IsTaskInfoModifed).text("TaskInfoModifed");
                    $(elementIDs.taskAssigWatchSectionList).empty();
                    $(elementIDs.taskAssigWatchPlansList).empty();
                    $(elementIDs.taskAssigWatchTasksList).empty();
                    fillTaskFolderVersionsDropDown();
                }
            });
            $(elementIDs.taskAssigWatchVersionList).change(function () {
                var eventTrigResult = $(elementIDs.ChangeEventTriggerByCode).text();
                if (eventTrigResult == false || eventTrigResult == "false") {
                    IsPlanTaskInfoUpdated = true;
                    fillWatchPlansDropDown();
                    //fillWatchViewDropDown();
                }

            });
            $(elementIDs.taskAssigWatchViewList).change(function () {
                var eventTrigResult = $(elementIDs.ChangeEventTriggerByCode).text();
                if (eventTrigResult == false || eventTrigResult == "false") {
                    IsPlanTaskInfoUpdated = true;
                    fillWatchSectionsDropdown();
                }

            });
            $(elementIDs.taskAssigWatchPlansList).change(function () {
                var eventTrigResult = $(elementIDs.ChangeEventTriggerByCode).text();
                if (eventTrigResult == false || eventTrigResult == "false") {
                    IsPlanTaskInfoUpdated = true;
                    fillWatchViewDropDown();
                }
            });

            $('input[type=radio][name=optradioWatchFolderType]').off('change').change(function () {
                if (this.id == 'optradioWatchAccount') {
                    $(elementIDs.taskAssigWatchAccountListLabelDiv).css('display', 'block');
                    $(elementIDs.taskAssigWatchAccountListDiv).css('display', 'block');
                    fillWatchAccountsDropDown();
                    ClearTaskAssignmentControlsData(true);
                    //$("#optradioWatchAccount").prop('checked', 'checked');
                }
                else {
                    $(elementIDs.taskAssigWatchAccountListLabelDiv).css('display', 'none');
                    $(elementIDs.taskAssigWatchAccountListDiv).css('display', 'none');
                    $(elementIDs.taskAssigWatchAccountList).val("");
                    fillTaskFoldersDropDown();
                    // $("#optradioWatchPortfolio").prop('checked', 'checked');
                }
            });

            $(elementIDs.taskAssigWatchAccountList).change(function () {
                var eventTrigResult = $(elementIDs.ChangeEventTriggerByCode).text();
                if (eventTrigResult == false || eventTrigResult == "false") {
                    IsPlanTaskInfoUpdated = true;
                    fillTaskFoldersDropDown();
                }
            });
            //$(elementIDs.taskAssigWatchSectionList).change(function () {
            //    var eventTrigResult = $(elementIDs.ChangeEventTriggerByCode).text();
            //    if (eventTrigResult == false || eventTrigResult == "false") {
            //        IsPlanTaskInfoUpdated = true;
            //        fillWatchPlansDropDown();
            //    }

            //});
            $(elementIDs.taskAssigWatchWorkFlowStatusList).change(function () {
                var eventTrigResult = $(elementIDs.ChangeEventTriggerByCode).text();
                if (eventTrigResult == false || eventTrigResult == "false") {
                    IsPlanTaskInfoUpdated = true;
                    fillWatchTasksDropDown();
                }
            });

            $(elementIDs.btnClassicFolderView).off('click').on('click', function () {
                if (isEditable == true) {
                    var rowId = $(elementIDs.watchGridJQ).getGridParam('selrow');
                    LoadWatchListFolderVersion(rowId, FolderViewMode.DefaultView);
                }
                else if (isEditable == false) {
                    viewWatchListFolderVersion(rowId, FolderViewMode.DefaultView);
                }
                $(elementIDs.folderViewDialog).dialog("close");
            });

            $(elementIDs.btnSOTView).off('click').on('click', function () {
                if (isEditable == true) {
                    var rowId = $(elementIDs.watchGridJQ).getGridParam('selrow');
                    LoadWatchListFolderVersion(rowId, FolderViewMode.SOTView);
                }
                else if (isEditable == false) {
                    viewWatchListFolderVersion(rowId, FolderViewMode.SOTView);
                }
                $(elementIDs.folderViewDialog).dialog("close");
            });

            $(document).ready(function () {
                var currentInstance = this;

                //get currentUser details           
                var promise = ajaxWrapper.getJSONSync(URLs.checkIsManager);
                promise.done(function (result) {
                    if (result) {
                        isManager = result;
                    }
                });
                loadWatchGrid(false);
            });
        }

        function clearValidationMsg() {
            $(elementIDs.taskAssigWatchFolderList).parent().addClass('has-error');
            $(elementIDs.taskAssigWatchFolderList).removeClass('form-control');
            $(elementIDs.taskAssigWatchFolderListSpan).text('');
            $(elementIDs.taskAssigWatchVersionList).parent().addClass('has-error');
            $(elementIDs.taskAssigWatchVersionList).removeClass('form-control');
            $(elementIDs.taskAssigWatchVersionListSpan).text('');
            $(elementIDs.taskAssigWatchWorkFlowStatusList).parent().addClass('has-error');
            $(elementIDs.taskAssigWatchWorkFlowStatusList).removeClass('form-control');
            $(elementIDs.taskAssigWatchWorkFlowStatusListSpan).text('');
            $(elementIDs.taskAssigWatchPlansList).parent().addClass('has-error');
            $(elementIDs.taskAssigWatchPlansList).removeClass('form-control');
            $(elementIDs.taskAssigWatchPlansListSpan).text('');
            $(elementIDs.taskAssigWatchTasksList).parent().addClass('has-error');
            $(elementIDs.taskAssigWatchTasksList).removeClass('form-control');
            $(elementIDs.taskAssigWatchTasksListSpan).text('');
            $(elementIDs.taskAssigWatchAssignUserList).parent().addClass('has-error');
            $(elementIDs.taskAssigWatchAssignUserList).removeClass('form-control');
            $(elementIDs.taskAssigWatchAssignUserListSpan).text('');
            $(elementIDs.taskAssigWatchTaskStatus).parent().addClass('has-error');
            $(elementIDs.taskAssigWatchTaskStatus).removeClass('form-control');
            $(elementIDs.taskAssigWatchTaskStatusSpan).text('');
            $(elementIDs.taskAssigWatchStartdate).parent().removeClass('has-error');
            $(elementIDs.taskAssigWatchStartdate).removeClass('form-control');
            $(elementIDs.taskAssigWatchStartdateSpan).text('');
            $(elementIDs.taskAssigWatchDuedate).parent().removeClass('has-error');
            $(elementIDs.taskAssigWatchDuedate).removeClass('form-control');
            $(elementIDs.taskAssigWatchDuedateSpan).text('');
            $(elementIDs.taskAssigWatchActualTime).parent().removeClass('has-error');
            $(elementIDs.taskAssigWatchActualTime).removeClass('form-control');
            $(elementIDs.taskAssigWatchActualTimeSpan).text('');
            $(elementIDs.taskAssigWatchEstTime).parent().removeClass('has-error');
            $(elementIDs.taskAssigWatchEstTime).removeClass('form-control');
            $(elementIDs.taskAssigWatchEstTimeSpan).text('');
        }

        //function LoadFolderViewDialog() {
        //    $(elementIDs.folderViewDialog).dialog('option', 'title', "Select Folder View");
        //    $(elementIDs.folderViewDialog).dialog("open");
        //}

        function LoadWatchListFolderVersion(rowId, viewMode) {
            var rowId = $(elementIDs.watchGridJQ).getGridParam('selrow');
            if (rowId !== undefined && rowId !== null) {
                var row = $(elementIDs.watchGridJQ).getRowData(rowId);

                if (isFolderLockEnable == false)
                {
                    //to forward the edit request to FolderVersion.
                    var folderVersionListUrl = URLs.folderVersionList.replace(/{tenantId}/g, row.TenantID).replace(/\{folderVersionId\}/g, row.FolderVersionId).replace(/\{folderId\}/g, row.FolderId).replace(/{foldeViewMode}/g, viewMode);
                    folderVersionListUrl = folderVersionListUrl.replace(/\{mode\}/g, true);//since edit button is clicked mode needs to be true
                    window.location.href = folderVersionListUrl + "&planTaskId=" + row.MappingRowID;
                    return;

                }
                var promise = ajaxWrapper.getJSON(URLs.LockStatusUrl.replace(/{tenantId}/g, row.TenantID).replace(/\{folderId\}/g, row.FolderId));
                promise.done(function (result) {
                    if (result != "") {
                        var userWarning = FolderLock.userWarning.replace("{0}", result);
                //        checkUnlockFolderClaims(elementIDs, claims);
                        folderLockWarningDialog.show(userWarning, function () {
                //            //unlock Folder Version
                //            var promise1 = ajaxWrapper.getJSON(URLs.OverrideLockUrl.replace(/{tenantId}/g, row.TenantID).replace(/\{folderId\}/g, row.FolderId));
                //            promise1.done(function (xhr) {
                //                if (xhr.Result === ServiceResult.SUCCESS) {
                //                    var folderVersionListUrl = URLs.folderVersionList.replace(/{tenantId}/g, row.TenantID).replace(/\{folderVersionId\}/g, row.FolderVersionId).replace(/\{folderId\}/g, row.FolderId).replace(/{foldeViewMode}/g, viewMode);
                //                    folderVersionListUrl = folderVersionListUrl.replace(/\{mode\}/g, true);   //since we are clicking on the edit button, mode needs to be true
                //                    window.location.href = folderVersionListUrl;
                //                }
                //                else if (xhr.Result === ServiceResult.FAILURE) {
                //                    //messageDialog.show(xhr.Items[0].Messages);
                //                }
                //                else {
                //                    messageDialog.show(Common.errorMsg);
                //                }
                //            });
                        }, function () {
                            //View Folder Version
                            var folderVersionListUrl = URLs.folderVersionList.replace(/{tenantId}/g, row.TenantID).replace(/\{folderVersionId\}/g, row.FolderVersionId).replace(/\{folderId\}/g, row.FolderId).replace(/{foldeViewMode}/g, viewMode);
                            folderVersionListUrl = folderVersionListUrl.replace(/\{mode\}/g, false);   //since we are clicking on the edit button, mode needs to be true
                             window.location.href = folderVersionListUrl + "&planTaskId=" + row.MappingRowID;
                            //window.location.href = folderVersionListUrl;
                        });
                        return;
                    }
                    else {
                var folderVersionListUrl = URLs.folderVersionList.replace(/{tenantId}/g, row.TenantID).replace(/\{folderVersionId\}/g, row.FolderVersionId).replace(/\{folderId\}/g, row.FolderId).replace(/{foldeViewMode}/g, viewMode);
                folderVersionListUrl = folderVersionListUrl.replace(/\{mode\}/g, true);   //since we are clicking on the edit button, mode needs to be true
                window.location.href = folderVersionListUrl + "&planTaskId=" + row.MappingRowID;
                //window.location.href = folderVersionListUrl;
                    }
                });
                promise.fail(showError);
            } else {
                $('#messagedialog').dialog({
                    title: 'Watch List',
                    height: 120
                });
                messageDialog.show(Common.selectRowMsg);
            }
        }

        function viewWatchListFolderVersion(gridID, viewMode) {
            var rowId = $(elementIDs.watchGridJQ).getGridParam('selrow');
            if (rowId !== undefined && rowId !== null) {
                var row = $(elementIDs.watchGridJQ).getRowData(rowId);
                //to forward the edit request to FolderVersion.                     
                var folderVersionListUrl = URLs.folderVersionList.replace(/{tenantId}/g, row.TenantID).replace(/\{folderVersionId\}/g, row.FolderVersionId).replace(/\{folderId\}/g, row.FolderId).replace(/{foldeViewMode}/g, viewMode);
                folderVersionListUrl = folderVersionListUrl.replace(/\{mode\}/g, false);   //since we are clicking on the view button, mode needs to be false
                window.location.href = folderVersionListUrl + "&planTaskId=" + row.MappingRowID;

            } else {
                $('#messagedialog').dialog({
                    title: 'Watch List',
                    height: 120
                });
                messageDialog.show(DashBoard.selectRowToViewFolderMsg);
            }
        }

        var isloaded = false, ischeckViewAll = false, ischeckViewInserted = false; isAddedUserAssigned = false, isManager = false;

        function loadWatchGrid(checkinterested) {
            //set column list for grid
            var colArray = ['Task #', 'Account', 'TenantID', 'FolderId', 'FolderVersionId', 'Folder', 'Folder Version', 'Effective Date', 'Workflow', 'Plan', 'View', 'Section', 'Task', 'Assignment', 'Status', 'Start Date', 'Due Date', 'Priority','Attachments', 'Completed', 'Status Date', 'Owner', 'ApprovalStatusID', 'CategoryID', 'Category', 'User Assignment', 'Estimated Time', 'Actual Time', 'Interested', 'FolderVersionWFStateID', 'TaskWFStateID'];

            //set column models
            var colModel = [];
            colModel.push({ name: 'MappingRowID', index: 'MappingRowID', editable: false, align: 'left', width: 75 });
            colModel.push({ name: 'Account', index: 'Account', hidden: true, editable: false, align: 'left' });
            colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true, search: false, });
            colModel.push({ name: 'FolderId', index: 'FolderId', hidden: true, search: false, });
            colModel.push({ name: 'FolderVersionId', index: 'FolderVersionId', hidden: true, search: false, });
            colModel.push({ name: 'Folder', index: 'Folder', editable: false, align: 'left' });
            colModel.push({ name: 'FolderVersion', index: 'FolderVersion', editable: false, align: 'left', hidden: true, });
            colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, hidden: true, align: 'center', formatter: 'date', sorttype: "date", searchoptions: { sopt: ["eq"] }, formatoptions: JQGridSettings.DateFormatterOptions });
            colModel.push({ name: 'Workflow', index: 'Workflow', editable: false, align: 'left', classes: 'jqgrid-cell-wrap', hidden: true });
            colModel.push({ name: 'Plan', index: 'Plan', editable: false, align: 'left' });
            colModel.push({ name: 'View', index: 'View', editable: false, align: 'left' });
            colModel.push({ name: 'Section', index: 'Section', editable: false, align: 'left', hidden: true });
            colModel.push({ name: 'Task', index: 'Task', editable: false, align: 'left', width: 250, classes: 'jqgrid-cell-wrap' });
            
            colModel.push({ name: 'Assignment', index: 'Assignment', editable: false, align: 'left' });
            colModel.push({ name: 'Status', index: 'Status', editable: false, align: 'center' });
            colModel.push({ name: 'StartDate', index: 'StartDate', editable: false, align: 'center', formatter: 'date', sorttype: "date", searchoptions: { sopt: ["eq"] }, formatoptions: JQGridSettings.DateFormatterOptions });
            colModel.push({ name: 'DueDate', index: 'DueDate', editable: false, align: 'center', formatter: 'date', sorttype: "date", searchoptions: { sopt: ["eq"] }, formatoptions: JQGridSettings.DateFormatterOptions });
            colModel.push({ name: 'Priority', index: 'Priority', editable: false, align: 'left' });
            colModel.push({ name: 'Comments', index: 'Comments', editable: true, align: 'center', formatter: commentsGridFormatterWQ });
            colModel.push({ name: 'Completed', index: 'Completed', hidden: (taskGridViewMode == 'Open' ? true : false), editable: false, align: 'center', formatter: 'date', sorttype: "date", searchoptions: { sopt: ["eq"] }, formatoptions: JQGridSettings.DateFormatterOptions });
            colModel.push({ name: 'StatusDate', index: 'StatusDate', hidden: true, editable: false, align: 'center', formatter: 'date', sorttype: "date", searchoptions: { sopt: ["eq"] }, formatoptions: JQGridSettings.DateFormatterOptions });
            colModel.push({ name: 'Owner', index: 'Owner', editable: false, hidden: true, align: 'center' });
            colModel.push({ name: 'ApprovalStatusID', index: 'ApprovalStatusID', hidden: true });
            colModel.push({ name: 'CategoryId', index: 'CategoryId', hidden: true });
            colModel.push({ name: 'CategoryName', index: 'CategoryName', hidden: true });
            colModel.push({ name: 'Assignment', index: 'Assignment', hidden: true, align: 'center', formatter: expandCollapseFmatterAssignment, search: false, sortable: false });
            colModel.push({ name: 'EstimatedTime', index: 'EstimatedTime', editable: false, align: 'left', hidden: true });
            colModel.push({ name: 'ActualTime', index: 'ActualTime', editable: false, align: 'left', hidden: true });
            colModel.push({ name: 'MarkInterested', index: 'MarkInterested', align: 'center', formatter: markinterestedFmatter, search: false, sortable: false });
            colModel.push({ name: 'FolderVersionWFStateID', index: 'FolderVersionWFStateID', editable: false, align: 'left', hidden: true });
            colModel.push({ name: 'TaskWFStateID', index: 'TaskWFStateID', editable: false, align: 'left', hidden: true });
            //clean up the grid first - only table element remains after this
            $(elementIDs.watchGridJQ).jqGrid('GridUnload');

            //adding the pager element
            //var elem = document.getElementById("p" + elementIDs.watchGrid);
            //elem.parentNode.removeChild(elem);
            $(elementIDs.watchGridJQ).parent().append("<div id='p" + elementIDs.watchGrid + "'></div>");
            var url = URLs.watchList.replace(/{isViewInterested}/g, checkinterested).replace('{viewMode}', taskGridViewMode);
            url = url.replace('{taskFolderVersionId}', taskFolderVersionId);
            $(elementIDs.watchGridJQ).jqGrid({
                url: url,
                datatype: 'json',
                cache: false,
                colNames: colArray,
                colModel: colModel,
                caption: 'Watch List',
                height: '220',
                rowNum: 10,
                ignoreCase: true,
                loadonce: false,
                autowidth: true,
                viewrecords: true,
                hidegrid: false,
                hiddengrid: false,
                altRows: true,
                sortname: 'MappingRowID',
                sortorder: 'desc',
                pager: '#p' + elementIDs.watchGrid,
                altclass: 'alternate',
                rowList: [10, 20, 30],
                gridComplete: function () {
                    //to check for claims.               
                    var objMap = {
                        edit: "#edit_watch_top",
                        view: "#btnWatchView"
                    };
                    checkApplicationClaims(claims, objMap, URLs.watchList);

                    //Add Assignee call
                    $(elementIDs.userAssignUrl).on('click', function (e) {
                        var folderVersionID = $(this).attr("data-Id");
                        if (folderVersionID != null) {
                            e.preventDefault();
                            viewAssignedUserDialog.show(folderVersionID);
                        }
                    });

                    $(elementIDs.markInterestedCheckboxJQ).on('click', function (e) {
                        var ischecked = $(this).prop('checked');
                        var rowId = $(this).prop('id').split('_')[1];
                        //setInterstedFolderVersion(ischecked, rowId);
                        saveInterstedTask(ischecked, rowId);
                    });

                    if (jQuery(elementIDs.watchGridJQ).jqGrid('getGridParam', 'records') > 0) {
                        if ($('.ui-search-toolbar').children().last().children().first().children().length <= 0) {
                            $('.ui-search-toolbar').children().last().children().first().append("<input type='checkbox' class='check-all' id='selectallcheckbox'/>");
                            $('.ui-search-toolbar').children().last().children().first().css('text-align', 'center');
                        }

                        if (checkinterested.toString() == "true")
                            $(elementIDs.markInterstedSelectAllCheckboxJQ).prop('checked', true);
                        else
                            $(elementIDs.markInterstedSelectAllCheckboxJQ).prop('checked', false);

                        $(elementIDs.markInterstedSelectAllCheckboxJQ).change(function () {
                            var ischecked = $(this).prop('checked');
                            $(elementIDs.markInterestedCheckboxJQ).filter(':not(:disabled)').prop('checked', ischecked);
                            saveInterstedAllTask(ischecked, checkinterested.toString() == "true" ? true : false);
                        });
                    }

                    $('#gview_watch').children().first().find('.showwatchcontrol').remove();
                    if (!isloaded) {
                        $('#gview_watch').children().first().append("<div class='showwatchcontrol'><input type='radio' id='chkviewall' name='viewwatchlist' value=false />View All <input type='radio' id='chkviewinterested' name='viewwatchlist' value=true /> View Interested </div>");
                        $('#chkviewall').prop('checked', 'checked');
                        ischeckViewAll = true;
                    }
                    else {
                        $('#gview_watch').children().first().append("<div class='showwatchcontrol'><input type='radio' id='chkviewall' name='viewwatchlist' value=false />View All <input type='radio' id='chkviewinterested' name='viewwatchlist' value=true /> View Interested </div>");
                        $('#chkviewinterested').prop('checked', ischeckViewInserted);
                        $('#chkviewall').prop('checked', ischeckViewAll);
                    }
                    $('input:radio[name=viewwatchlist]').on('click', function () {
                        ischeckViewInserted = $(this).val();
                        ischeckViewAll = ischeckViewInserted == "true" ? false : true;
                        loadWatchGrid(ischeckViewInserted);
                    });
                    isloaded = true;
                    if (taskGridViewMode == 'Completed') {
                        $('#chkCompletedTasks').prop('checked', 'checked');
                    } else {
                        $('#chkOpenTasks').prop('checked', 'checked');
                    }

                    $(".WQCOMMENTS").unbind('click');
                    $(".WQCOMMENTS").click(function () {
                        var mappingTaskID = $(this).attr("id");
                        loadCommentsDailog(mappingTaskID);
                    });

                    var rowIds = $(elementIDs.watchGridJQ).getDataIDs();
                    for (var i = 0 ; i <= rowIds.length; i++) {
                        var rowData = $(elementIDs.watchGridJQ).getRowData(rowIds[i]);
                        if (rowData.FolderVersionWFStateID == rowData.TaskWFStateID) {
                            var trElement = $("#" + rowIds[i], elementIDs.watchGridJQ);
                            trElement.find("td:first").css("color", "#57BD99");
                        }
                    }
                },
                //To disable edit button when selected folder is released and approved.
                onSelectRow: function (rowId) {
                    //to check if user has permissions to disable the Release Folder.(eg. Viewer)                                 
                    var editFlag = checkUserPermissionForEditable(URLs.watchList);
                    if (rowId !== undefined && rowId !== null) {
                        var row = $(this).getRowData(rowId);
                        //if (editFlag) {
                        //    if (vbRole != undefined) {
                        //if ((vbRole != Role.Audit && row.Status == "Test/Audit") && (vbRole != Role.Superuser && row.Status == "Test/Audit")
                        //    && (vbRole != Role.ProductAudit && row.Status == "Test/Audit")) {
                        //    $("#btnWatchEdit").addClass('ui-state-disabled');
                        //}
                        //else if (row.Status == 'Facets Prod' && row.ApprovalStatusID !== null && row.ApprovalStatusID == 1) {
                        //    $("#btnWatchEdit").addClass('ui-state-disabled');
                        //}
                        //else {
                        $("#edit_watch_top").removeClass('ui-state-disabled');
                        $("#btnWatchEdit").removeClass('ui-state-disabled');
                        $("#btnWatchView").removeClass('ui-state-disabled');
                        //}
                        // }
                        // }
                    }
                },
                resizeStop: function (width, index) {
                    autoResizing(elementIDs.watchGridJQ);
                },
                jsonReader: {
                    //page: function (obj) {
                    //    return obj.records == 0 || obj.records == undefined ? "0" : obj.page;
                    //},
                    //total: function (obj) {
                    //    var lastPageIndex = 0;
                    //    if (obj != null && obj != undefined && obj[0] != null && obj[0] != undefined
                    //        && obj[0].LastPageNoForGrid != null && obj[0].LastPageNoForGrid != undefined) {
                    //        lastPageIndex = obj[0].LastPageNoForGrid;
                    //    }
                    //    return lastPageIndex;
                    //},
                }

            });
            var pagerElement = '#p' + elementIDs.watchGrid;
            //$('#p' + elementIDs.watchGrid).find('input').css('height', '20px');

            $(elementIDs.watchGridJQ).jqGrid('filterToolbar', {
                stringResult: true, searchOnEnter: true,
                defaultSearch: "cn",
            });

            //remove default buttons
            formDesignVersionId = 2364;

            allowTaskAssignmentToAllFlag = false;

            var url = URLs.getRepeateUserSettings.replace(/\{formDesignVersionId\}/g, formDesignVersionId);
            var promise = ajaxWrapper.getJSONSync(url);
            promise.done(function (data) {
                if (data != null) {

                    n = data.length;
                    for (i = 0; i < n; i++) {
                        if (data[i].Key == "AllowTaskAssignmentToALl") {
                            allowTaskAssignmentToAllFlag = data[i].Data == "True" ? true : false;
                        }

                    }
                }
            });
            promise.fail(showError);

            $(elementIDs.watchGridJQ).jqGrid('navGrid', pagerElement, { edit: false, view: false, add: false, del: false, search: false });

            $(elementIDs.watchGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '',
                buttonicon: 'ui-icon-plus',
                title: 'New Task Assignment',
                id: 'btnAssignNewTask',
                disabled: 'disabled',
                onClickButton: function () {
                    if (isManager == true) {
                        clearValidationMsg();
                        openTaskAssignmentWatchListDialog('Add');
                    }
                    else {
                        if (allowTaskAssignmentToAllFlag == true) {
                            clearValidationMsg();
                            openTaskAssignmentWatchListDialog('Add');
                        } else {
                            messageDialog.show(DashBoard.UseraddrestrictedMsg);
                        }
                    }
                }
            });

            $(elementIDs.watchGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '',
                buttonicon: 'ui-icon-pencil',
                title: 'Edit Task Assignment',
                id: 'btnAssignEditTask',
                disabled: 'disabled',
                onClickButton: function () {
                    if (isManager == true) {
                        clearValidationMsg();
                        openTaskAssignmentWatchListDialog('Edit');
                    }
                    else {
                        messageDialog.show(DashBoard.UsereditrestrictedMsg);
                    }
                }
            });

            if (taskGridViewMode == 'Completed') {
                //$("#btnAssignEditTask").addClass("ui-state-disabled");
                $("#btnAssignNewTask").addClass("ui-state-disabled");

            } else {
                if (allowTaskAssignmentToAllFlag) {
                    $("#btnAssignNewTask").removeClass("ui-state-disabled");
                    $("#btnAssignEditTask").removeClass("ui-state-disabled");
                }
                else {
                    if (vbRole == 23) {
                        $("#btnAssignEditTask").addClass("ui-state-disabled");
                        $("#btnAssignNewTask").addClass("ui-state-disabled");
                    }
                    else {
                        $("#btnAssignNewTask").removeClass("ui-state-disabled");
                        $("#btnAssignEditTask").removeClass("ui-state-disabled");
                    }
                }
            }

            $(elementIDs.watchGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '',
               buttonicon: 'ui-icon-document-b',
               title: 'View Task Assignment',
               id: 'btnAssignViewTask',
               disabled: 'disabled',
               onClickButton: function () {
                   clearValidationMsg();
                   openTaskAssignmentWatchListDialog('View');
               }
           });

            //edit button in footer of grid that pops up the edit form design dialog
            $(elementIDs.watchGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '',
                buttonicon: 'ui-icon-script',
                title: 'Edit Folder',
                id: 'edit_watch_top',
                disabled: 'disabled',
                onClickButton: function () {
                    isEditable = true;
                    var rowId = $(elementIDs.watchGridJQ).getGridParam('selrow');
                    LoadWatchListFolderVersion(rowId, FolderViewMode.DefaultView);

                }
            });

            //View button in footer of grid 
            $(elementIDs.watchGridJQ).jqGrid('navButtonAdd', pagerElement,
               {
                   caption: '', id: 'btnWatchView', buttonicon: 'ui-icon-copy', title: 'View Folder',
                   onClickButton: function () {
                       var rowId = $(elementIDs.watchGridJQ).getGridParam('selrow');
                       viewWatchListFolderVersion(rowId, FolderViewMode.DefaultView);
                   }
               });
            //Download Excel file
            $(elementIDs.watchGridJQ).jqGrid('navButtonAdd', pagerElement,
              {
                  caption: '', buttonicon: 'ui-icon-arrowstop-1-s', title: 'Download Watch List', id: 'btnWatchListExportToExcel',
                  onClickButton: function () {
                      $.download(URLs.exportWatchListToExcel.replace(/{isViewInterested}/g, checkinterested).replace('{viewMode}', taskGridViewMode), "test", 'post');
                  }
              });
        }

        function commentsGridFormatterWQ(cellvalue, options, rowObject) {
            if (cellvalue == undefined || cellvalue.trim() == "" || cellvalue == null) {
                return '';
            }
            var link = "<a href='#' class='WQCOMMENTS' id='" + rowObject.MappingRowID + "'><span align='center' class='ui-icon-extlink' style='margin:auto' title='View Attachments' ></span></a>";
            return link;
        }

        var expandCollapseFmatterAssignment = function (cellvalue, options, rowObject) {
            if (!$.isNumeric(options.rowId)) {
                return '';
            }
            var text = "";
            //var divEle = "<div class='row'><div class='col-sm-4'><span title='Add Assignee' class='ui-icon ui-icon-extlink' style='margin: auto;' align='center'></span></div><div class='col-sm-4'><span class='badge' title='No. of Users Assigned'>" + rowObject.AssignedUserCount + "</span></div></div>"
            if (rowObject != null) {
                if (isManager) {
                    text = "<a class='ASSIGNMENT' href='#' data-Id='" + rowObject.FolderVersionId + "'><div style='float: left;'><span title='Add Assignee' class='ui-icon ui-icon-extlink' style='margin: auto;' align='center'></span></div><span class='badge' title='No. of Users Assigned'>" + rowObject.AssignedUserCount + "</span></a>";
                }
                else {
                    text = "<span class='badge' title='No. of Users Assigned'>" + rowObject.AssignedUserCount + "</span>";
                }
            }
            return text;
            promise.fail(showError);
        }

        var markinterestedFmatter = function (cellvalue, options, rowObject) {
            if (cellvalue == "Yes" || cellvalue == "True" || cellvalue == "Y-True" || cellvalue == true) {
                result = "<input type='checkbox' checked='checked' id='selectcheckbox_" + options.rowId + "' name= 'selectcheckbox_' />";
            }
            else {
                result = "<input type='checkbox' id='selectcheckbox_" + options.rowId + "' name= 'selectcheckbox_' />";
            }
            return result;
        }

        function loadCommentsDailog(mappingTaskID) {
            $(elementIDs.taskCommentsDailog).dialog({
                autoOpen: false,
                height: 500,
                width: 700,
                modal: true,
            });
            $(elementIDs.taskCommentsDailog).dialog('option', 'title', 'View Task Attachments');
            taskAssigWatchComments(mappingTaskID, "View", elementIDs.taskCommentsGrid);
            $(elementIDs.taskCommentsDailog).dialog("open");
        };

        function loadUserAssignmentGrid(folderVersionID, userAssignmentDialogState) {

            var getUserAssignmentList = URLs.userAssignmentList.replace(/\{folderVersionID\}/g, folderVersionID).replace(/\{userAssignmentDialogState\}/g, userAssignmentDialogState);


            //set column list for grid
            var colArray = ['', 'User Role', '', 'User', 'ApplicableTeamID', 'Team', '', ''];

            //set column models
            var colModel = [];
            colModel.push({ name: 'UserRoleID', index: 'UserRoleID', editable: false, width: '265', hidden: true });
            colModel.push({ name: 'UserRoleName', index: 'UserRoleName', editable: false, width: '250' });
            colModel.push({ name: 'UserID', index: 'UserID', editable: true, cellEdit: true, width: '200', hidden: true });
            colModel.push({ name: 'UserName', index: 'UserName', editable: true, width: '250' });
            colModel.push({ name: 'ApplicableTeamID', index: 'ApplicableTeamID', editable: false, width: '265', hidden: true });
            colModel.push({ name: 'ApplicableTeamName', index: 'ApplicableTeamName', editable: false, width: '200' });
            colModel.push({ name: 'ApplicableTeamUserMapID', index: 'ApplicableTeamUserMapID', editable: false, width: '265', hidden: true });
            colModel.push({ name: 'WFStateUserMapID', index: 'WFStateUserMapID', editable: false, width: '265', hidden: true });

            var gridJQ = (userAssignmentDialogState != undefined) ? elementIDs.userAssignmentUpdateGridJQ : elementIDs.viewAssignedUsersGridJQ;
            var grid = (userAssignmentDialogState != undefined) ? elementIDs.userAssignmentUpdateGrid : elementIDs.viewAssignedUsersGrid;
            isGridEdit = (userAssignmentDialogState != undefined) ? true : false;

            //clean up the grid first - only table element remains after this
            $(gridJQ).jqGrid('GridUnload');

            //adding the pager element
            $(gridJQ).parent().append("<div id='p" + grid + "'></div>");

            $(gridJQ).jqGrid({
                url: getUserAssignmentList,
                datatype: 'json',
                cache: false,
                colNames: colArray,
                colModel: colModel,
                caption: '',
                height: '260',
                pgbuttons: false,
                pginput: false,
                pgtext: "",
                //rowNum: 10,
                ignoreCase: true,
                loadonce: false,
                sortname: 'UserID',
                //autowidth: true,
                viewrecords: true,
                hidegrid: true,
                hiddengrid: false,
                altRows: true,
                multiselect: isGridEdit,
                pager: '#p' + grid,
                altclass: 'alternate',
                editurl: 'clientArray',
                //rowList: [10, 20, 30],
                loadComplete: function () {
                    if (isGridEdit) {
                        var btnAssign = $('#p' + grid + '_left').find(elementIDs.btnSaveUserAssignment);
                        if (btnAssign.length == 0) {
                            $('#p' + grid + '_left').append('<td><button type="button" id="assignUser" class="btn pull-right"></button></td>');
                            if (userAssignmentDialogState == "add") {
                                $(elementIDs.userAssignUnassignDialog).dialog('option', 'title', DashboardMessages.userDialogAssignment);
                                $(elementIDs.btnSaveUserAssignment).text('Assign');
                            }
                            else {
                                $(elementIDs.userAssignUnassignDialog).dialog('option', 'title', DashboardMessages.userDialogUnAssignment);
                                $(elementIDs.btnSaveUserAssignment).text('Unassign');
                            }
                        }
                        $('#p' + grid + '_left').find('table').hide();

                        //register events for button click
                        $(elementIDs.btnSaveUserAssignment).unbind('click');
                        $(elementIDs.btnSaveUserAssignment).click(function () {
                            var folderID = $(elementIDs.watchGridJQ).getGridParam("selrow");
                            folderData = $(elementIDs.watchGridJQ).getRowData(folderID);

                            var selRowIds = jQuery(gridJQ).jqGrid('getGridParam', 'selarrrow'), userList = new Array();
                            if (selRowIds.length > 0) {
                                $(selRowIds).each(function (i, id) {
                                    var row = jQuery(gridJQ).getRowData(id);

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



                                var url = (userAssignmentDialogState == "add") ? URLs.updateUserRoles : URLs.deleteUserRoles;

                                //Check for Portfloio folder for TPA Analyst user id  int tenantId, int folderId
                                var isPortfolioFolder = false;
                                var getFolderVersionParameter = { tenantId: folderData.TenantID, folderId: folderData.FolderId };

                                var promisePortfolio = ajaxWrapper.postJSONCustom(URLs.getFolderVersionList, getFolderVersionParameter);
                                promisePortfolio.done(function (resultFolderVersion) {

                                    if (resultFolderVersion.length > 0) {
                                        if (resultFolderVersion[0].IsPortfolio) {
                                            isPortfolioFolder = true;
                                        }
                                    }
                                    // Check for TPA Analyst is included, if yes then remove it
                                    var removedTPAuserList = new Array();
                                    var isTPAAnalystPresent = false;
                                    var tpaIdx = 0;
                                    for (var idx = 0; idx < userList.length; idx++) {
                                        if (isPortfolioFolder) {
                                            if (userList[idx].UserRoleID == 22)
                                            { isTPAAnalystPresent = true; }
                                            else
                                            {
                                                removedTPAuserList[tpaIdx] = userList[idx];
                                                tpaIdx = tpaIdx + 1;
                                            }
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
                                                folderVersionId: folderData.FolderVersionId
                                            };

                                    if (isPortfolioFolder && isTPAAnalystPresent && userList.length == 1) {
                                        messageDialog.show("Portfolio Folder cannot be assigned to users having TPA Analyst user role.");
                                    }
                                    else {
                                        if (isPortfolioFolder && isTPAAnalystPresent) {
                                            messageDialog.show("Portfolio Folder cannot be assigned to users having TPA Analyst user role from the selected list.");
                                        }
                                        ////ajax call to add/update
                                        var promise = ajaxWrapper.postJSONCustom(url, userRowsToAdd);
                                        ////register ajax success callback
                                        promise.done(function (xhr) {
                                            userRoleSuccess(xhr, userAssignmentDialogState);
                                        });


                                        ////register ajax failure callback
                                        promise.fail(showError);
                                    }
                                });
                            }
                            else {
                                messageDialog.show('Please select a row.');
                            }
                        });
                    }
                },
                resizeStop: function (width, index) {
                    autoResizing(elementIDs.userAssignmentGridJQ);
                }
            });
            if (isGridEdit)
                $(gridJQ + "_cb").css("width", "35px");

            var pagerElement = '#p' + grid;
            $('#p' + grid).find('input').css('height', '20px');

            $(gridJQ).jqGrid('filterToolbar', {
                stringResult: true, searchOnEnter: true,
                defaultSearch: "cn",
            });

            //remove default buttons
            $(gridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

            //add button in footer of grid that pops up the edit form design dialog
            $(gridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '',
                buttonicon: 'ui-icon-plus',
                title: 'Add',
                id: 'btnUserAdd',
                //disabled: 'disabled',
                onClickButton: function (e) {
                    e.preventDefault();
                    UserAssignUnassignDialog.show('add');
                }
            });


            //add button in footer of grid that pops up the edit form design dialog
            $(gridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '',
                buttonicon: 'ui-icon-minus',
                title: 'Delete',
                id: 'btnUserDelete',
                //disabled: 'disabled',
                onClickButton: function () {
                    UserAssignUnassignDialog.show('delete');
                }
            });
        }

        function loadTaskAssignmentWatchListDialog(rowId, transactionMode) {
            var watchGriddata = $(elementIDs.watchGridJQ).jqGrid('getRowData', rowId);
            var rowdata = $(elementIDs.watchGridJQ).jqGrid('getRowData', rowId);
            $(elementIDs.HiddenParameter).text("SaveEditUnDone");
            $(elementIDs.taskAssigWatchTaskStatusRow).show();
            IsPlanTaskInfoUpdated = false;
            $(elementIDs.cancelWatchDflag).text("CancelUnDone");
            $(elementIDs.ChangeEventTriggerByCode).text("false");
            selectedTaskFolderversionWorkFlowState = watchGriddata.Workflow;
            selectedTaskstate = rowdata.Status;
            $(elementIDs.IsTaskLateStatus).text("false");
            $(elementIDs.FolderVersionsListHDVal).text("");
            $(elementIDs.optradioWatchPortfolio).prop("checked", true);
            taskAssigWatchAttachment(rowdata.ID);
            fillWatchTaskPrioityList();
            if (transactionMode == 'Add') {
                //$(elementIDs.taskAssigWatchOrder).val('');
                $(elementIDs.taskAssigWatchDuration).val('0');                
                $(elementIDs.taskAssigWatchAttachment).val('');
                $(elementIDs.taskAssigWatchViewList).empty();
                $(elementIDs.taskAssigWatchSectionList).empty();
                $("#etadfloderid").text("Add :" + rowdata.FolderId);
                EnableTaskAssignmentConstrols();
                ClearTaskAssignmentControlsData();
                FolderVersionsList = null;
                MappingRowIDForTask = null;
                IsPlanNewTaskUpdate = null;
                UPTFolderID = null; 
                UPTFolderVersionID = null;
                UPTWFStateID = null;
                UPTFormInstanceId = null;
                UPTTaskID = null;
                $(elementIDs.IsTaskInfoModifed).text("TaskInfoUnModifed");
                $(elementIDs.PlanTaskMode).text("Add");
                $(elementIDs.taskAssigWatchTaskStatusRow).hide();
                if ($("input[name='optradioWatchFolderType']:checked")[0].id == "optradioWatchPortfolio")
                    fillTaskFoldersDropDown();
                else
                    fillWatchAccountsDropDown();
                fillWatchAssignUserList();
                fillWatchTaskStatusList();
           
                taskAssigWatchComments();
            } else if (transactionMode == 'Edit') {
                $("#etadfloderid").text("Edit : " + rowdata.FolderId);
                MappingRowIDForTask = rowdata.MappingRowID;
                $(elementIDs.MappingRowIDForTaskHDVal).text(rowdata.MappingRowID);
                IsPlanNewTaskUpdate = true;
                FolderVersionsList = null;
                UPTFolderID = null;
                UPTFolderVersionID = null;
                UPTWFStateID = null;
                UPTFormInstanceId = null;
                UPTTaskID = null;
                UPViewID = null;
                UPSectionID = null;
                UPOrder = null;
                $(elementIDs.IsTaskInfoModifed).text("TaskInfoUnModifed");
                $(elementIDs.PlanTaskMode).text("Edit");
                EnableTaskAssignmentConstrols();
                ClearTaskAssignmentControlsData();
                loadTaskAssignmentUpdateSettings(rowdata.MappingRowID);
            } else {
                $("#etadfloderid").text("View : " + rowdata.FolderId);
                MappingRowIDForTask = rowdata.MappingRowID;
                $(elementIDs.MappingRowIDForTaskHDVal).text(rowdata.MappingRowID);
                FolderVersionsList = null;
                IsPlanNewTaskUpdate = null;
                UPTFolderID = null;
                UPTFolderVersionID = null;
                UPTWFStateID = null;
                UPTFormInstanceId = null;
                UPTTaskID = null;
                $(elementIDs.PlanTaskMode).text("View")
                $(elementIDs.IsTaskInfoModifed).text("View");
                loadTaskAssignmentViewSettings(rowdata.MappingRowID);
            }
        }
        function loadTaskAssignmentUpdateSettings(rowId) {
            //Populate Folder list             
            ajaxDialog.hidePleaseWait();
            ajaxDialog.showPleaseWait();

            //$(elementIDs.taskAssigWatchFolderList).val("0").autoDropdown("refresh");
            //Populate Assigned user list
            $(elementIDs.taskAssigWatchAssignUserList).empty();
            DisableTaskAssignmentConstrols(true);
            var loginUserName = "";
            var urlUserList = '/PlanTaskUserMapping/GetTasksTeamMemberList?strUserName={strUserName}';
            var promiseUserList = ajaxWrapper.getJSON(urlUserList.replace(/{strUserName}/g, loginUserName));
            promiseUserList.done(function (listUser) {
                for (i = 0; i < listUser.length; i++) {
                    $(elementIDs.taskAssigWatchAssignUserList).append("<option value=" + listUser[i].Key + ">" + listUser[i].Value + "</option>");
                }
                //Populate Task Status
                $(elementIDs.taskAssigWatchTaskStatus).empty();
                $(elementIDs.taskAssigWatchTaskStatus).append("<option value='0'>" + "Select One" + "</option>");
                var urlTaskStatusList = '/PlanTaskUserMapping/GetWatchTaskStatusList';
                var promiseTaskStatusList = ajaxWrapper.getJSON(urlTaskStatusList);
                promiseTaskStatusList.done(function (listTaskStatus) {
                    //Fetch data for particular row id 
                    var PlanTaskUserMappingIdVal = MappingRowIDForTask;
                    if (PlanTaskUserMappingIdVal != null && PlanTaskUserMappingIdVal != undefined) {
                        var urlGetPlanTaskUserMapping = '/PlanTaskUserMapping/GetPlanTaskUserMappingList?PlanTaskUserMappingId={PlanTaskUserMappingId}';
                        var promiseGetPlanTaskUserMapping = ajaxWrapper.getJSON(urlGetPlanTaskUserMapping.replace(/{PlanTaskUserMappingId}/g, PlanTaskUserMappingIdVal));
                        promiseGetPlanTaskUserMapping.done(function (listPlanTaskUserMapping) {
                            if (listPlanTaskUserMapping != null && listPlanTaskUserMapping != undefined && listPlanTaskUserMapping.length > 0) {
                                var accountName = listPlanTaskUserMapping[0].AccountName;
                                var accountID;
                                if (accountName != undefined && accountName != "NA") {
                                    accountID = accountName.split('|')[1];
                                    $(elementIDs.taskAssigWatchAccountListLabelDiv).css('display', 'block');
                                    $(elementIDs.taskAssigWatchAccountListDiv).css('display', 'block');
                                    $("#optradioWatchAccount").prop('checked', 'checked');
                                }
                                else {
                                    $(elementIDs.taskAssigWatchAccountListLabelDiv).css('display', 'none');
                                    $(elementIDs.taskAssigWatchAccountListDiv).css('display', 'none');
                                    $("#optradioWatchPortfolio").prop('checked', 'checked').change();
                                }

                                $(elementIDs.taskAssigWatchAccountList).empty();
                                $(elementIDs.taskAssigWatchAccountList).append("<option value='" + accountID + "'>" + accountName.split('|')[0] + "</option>");
                                $(elementIDs.taskAssigWatchFolderList).empty();
                                $(elementIDs.taskAssigWatchFolderList).append("<option value='" + listPlanTaskUserMapping[0].FolderID + "'>" + listPlanTaskUserMapping[0].FolderName + "</option>");

                                taskAssigWatchComments(rowId);

                                if (listPlanTaskUserMapping[0].Status == "Late") {
                                    $(elementIDs.IsTaskLateStatus).text("true");
                                    $(elementIDs.taskAssigWatchTaskStatus).append("<option value=" + "4" + ">" + "Late" + "</option>");
                                    $(elementIDs.taskAssigWatchTaskStatus).val("4");

                                }

                                for (i = 0; i < listTaskStatus.length; i++) {
                                    if (listPlanTaskUserMapping[0].Status == "Late") {
                                        $(elementIDs.IsTaskLateStatus).text("true");
                                        if (listTaskStatus[i].TaskStatusDescription == "Completed" || listTaskStatus[i].TaskStatusDescription == "Completed - Fail" || listTaskStatus[i].TaskStatusDescription == "Completed - Pass"
                                            || listTaskStatus[i].TaskStatusDescription == "InProgress") {
                                            $(elementIDs.taskAssigWatchTaskStatus).append("<option value=" + listTaskStatus[i].TaskStatusID + ">" + listTaskStatus[i].TaskStatusDescription + "</option>");
                                        }
                                    }
                                    else {
                                        $(elementIDs.taskAssigWatchTaskStatus).append("<option value=" + listTaskStatus[i].TaskStatusID + ">" + listTaskStatus[i].TaskStatusDescription + "</option>");
                                    }
                                }

                                $(elementIDs.taskAssigWatchTaskStatus + " option").each(function () {
                                    var $thisOption = $(this);
                                    var valueToCompare = "4";

                                    if ($thisOption.val() == valueToCompare) {
                                        $thisOption.attr("disabled", "disabled");
                                    }
                                });

                                //$(elementIDs.taskAssigWatchFolderList + " select").val(listPlanTaskUserMapping[0].FolderID);
                                //$('.id_100 option[value=val2]').attr('selected', 'selected');
                                var designDetailsJson = $.parseJSON(listPlanTaskUserMapping[0].PlanTaskUserMappingDetails);
                                UPTFolderID = listPlanTaskUserMapping[0].FolderID;
                                UPTFolderVersionID = listPlanTaskUserMapping[0].FolderVersionID;
                                UPTWFStateID = listPlanTaskUserMapping[0].WFStateID;
                                UPTFormInstanceId = designDetailsJson != null ? designDetailsJson.FormInstanceId : "";
                                UPTTaskID = listPlanTaskUserMapping[0].TaskID;
                                UPViewID = designDetailsJson != null ? designDetailsJson.FormDesignVersionId : "";
                                UPSectionID = designDetailsJson != null ? designDetailsJson.SectionId : "";
                                UPOrder = listPlanTaskUserMapping[0].Order;
                                UPDuration = listPlanTaskUserMapping[0].Duration;
                                UPAttachment = listPlanTaskUserMapping[0].Attachment;
                                $(elementIDs.taskAssigWatchOrder).val(UPOrder);
                                $(elementIDs.taskAssigWatchDuration).val(UPDuration);
                                
                                for (i = 0; i < listUser.length; i++) {
                                    if (listUser[i].Value == listPlanTaskUserMapping[0].AssignedUserName) {
                                        $(elementIDs.taskAssigWatchAssignUserList).val(listUser[i].Key).change();
                                    }
                                }
                                if (listPlanTaskUserMapping[0].AssignedUserName != null && listPlanTaskUserMapping[0].AssignedUserName.indexOf(',') >= 0) {
                                    var ids = listPlanTaskUserMapping[0].AssignedUserName.split(',');
                                    $(ids).each(function (i, e) {
                                        var id = $.grep(listUser, function (element, index) {
                                            return element.Value == e;
                                        });
                                        $(elementIDs.taskAssigWatchAssignUserList + " option[value='" + id[0].Key + "']").prop("selected", true);
                                    });
                                }
                                else {
                                    var id = $.grep(listUser, function (element, index) {
                                        return element.Value == listPlanTaskUserMapping[0].AssignedUserName;
                                    });
                                    if (id.length != 0)
                                        $(elementIDs.taskAssigWatchAssignUserList).val(id[0].Key).change();
                                }
                                for (i = 0; i < listTaskStatus.length; i++) {
                                    if (listTaskStatus[i].TaskStatusDescription == listPlanTaskUserMapping[0].Status) {
                                        $(elementIDs.taskAssigWatchTaskStatus).val(listTaskStatus[i].TaskStatusID).change();
                                    }
                                }
                                var startDate = new Date(listPlanTaskUserMapping[0].StartDate);
                                var startday = ("0" + startDate.getDate()).slice(-2);
                                var startmonth = ("0" + (startDate.getMonth() + 1)).slice(-2);
                                var startDateValue = (startmonth) + "/" + (startday) + "/" + startDate.getFullYear();
                                $(elementIDs.taskAssigWatchStartdate).val(startDateValue);
                                var dueDate = new Date(listPlanTaskUserMapping[0].DueDate);
                                var dueday = ("0" + dueDate.getDate()).slice(-2);
                                var duemonth = ("0" + (dueDate.getMonth() + 1)).slice(-2);
                                var dueDateValue = (duemonth) + "/" + (dueday) + "/" + dueDate.getFullYear();
                                $(elementIDs.taskAssigWatchDuedate).val(dueDateValue);
                                //set ChangeEventTriggerByCode Flag to true so that change event of folderlist combo box will not hit
                                $(elementIDs.ChangeEventTriggerByCode).text("true");
                                //Populate Versions combo box
                                $(elementIDs.taskAssigWatchVersionList).empty();
                                $(elementIDs.taskAssigWatchEstTime).val(listPlanTaskUserMapping[0].EstimatedTime);
                                $(elementIDs.taskAssigWatchActualTime).val(listPlanTaskUserMapping[0].ActualTime);
                                var folderIdValue = $(elementIDs.taskAssigWatchFolderList).val();
                                if (folderIdValue != null && folderIdValue != undefined && folderIdValue != "0") {
                                    var url = '/FolderVersion/GetTasksFolderVersionsList?folderId={folderId}';
                                    var promiseVerList = ajaxWrapper.getJSON(url.replace(/{folderId}/g, folderIdValue));
                                    ajaxDialog.hidePleaseWait();
                                    ajaxDialog.showPleaseWait();

                                    promiseVerList.done(function (Verlist) {
                                        $(elementIDs.taskAssigWatchFolderList).val(listPlanTaskUserMapping[0].FolderID).change();
                                        FolderVersionsList = Verlist;
                                        $(elementIDs.FolderVersionsListHDVal).val(JSON.stringify(Verlist));
                                        $(elementIDs.taskAssigWatchVersionList).empty();
                                        $(elementIDs.taskAssigWatchVersionList).append("<option value='0'>" + "Select One" + "</option>");
                                        for (i = 0; i < Verlist.length; i++) {
                                            $(elementIDs.taskAssigWatchVersionList).append("<option value=" + Verlist[i].FolderVersionId + ">" + Verlist[i].FolderVersionNumber + "</option>");
                                        }
                                        $(elementIDs.taskAssigWatchVersionList).val(UPTFolderVersionID).change();
                                        // Populate Plan combo box
                                        $(elementIDs.taskAssigWatchPlansList).empty();
                                        var folderVersionIdValue = $(elementIDs.taskAssigWatchVersionList).val();
                                        if (folderIdValue != null && folderIdValue != undefined && folderIdValue != "0"
                                            && folderVersionIdValue != null && folderVersionIdValue != undefined && folderVersionIdValue != "0") {
                                            var url = '/FolderVersion/GetTasksFormInstanceListForFolderVersion?folderVersionId={folderVersionId}&folderId={folderId}';
                                            var promisePlanList = ajaxWrapper.getJSON(url.replace(/{folderVersionId}/g, folderVersionIdValue).replace(/{folderId}/g, folderIdValue));
                                            ajaxDialog.hidePleaseWait();
                                            ajaxDialog.showPleaseWait();

                                            promisePlanList.done(function (Planlist) {
                                                $(elementIDs.taskAssigWatchPlansList).empty();
                                                $(elementIDs.taskAssigWatchPlansList).append("<option value='0'>" + ALLPlans + "</option>");
                                                for (i = 0; i < Planlist.length; i++) {
                                                    $(elementIDs.taskAssigWatchPlansList).append("<option value=" + Planlist[i].FormInstanceID + ">" + Planlist[i].FormDesignName + "</option>");
                                                }

                                                UPTFormInstanceId = UPTFormInstanceId.replace(',', '');
                                                $(elementIDs.taskAssigWatchPlansList).val(UPTFormInstanceId).change();
                                                if (UPTFormInstanceId != "")
                                                    $(elementIDs.taskAssigWatchPlansList).attr("disabled", "disabled");
                                                // Populate WorkFlowStatus combo box
                                                $(elementIDs.taskAssigWatchWorkFlowStatusList).empty();

                                                var wfStateId;
                                                for (i = 0; i < FolderVersionsList.length; i++) {
                                                    if (FolderVersionsList[i].FolderVersionId == folderVersionIdValue) {
                                                        wfStateId = FolderVersionsList[i].WFStateID;
                                                    }
                                                }
                                                if (wfStateId != undefined && wfStateId != null) {
                                                    var url = '/WorkFlow/GetTasksWorkFlowStateListGreaterThanSelected?wfStateId={wfStateId}&&folderversionId=' + folderVersionIdValue;
                                                    var promiseWorkFlowStatusList = ajaxWrapper.getJSON(url.replace(/{wfStateId}/g, wfStateId));
                                                    ajaxDialog.hidePleaseWait();
                                                    ajaxDialog.showPleaseWait();

                                                    promiseWorkFlowStatusList.done(function (WorkFlowlist) {
                                                        $(elementIDs.taskAssigWatchWorkFlowStatusList).empty();
                                                        $(elementIDs.taskAssigWatchWorkFlowStatusList).append("<option value='0'>" + "Select One" + "</option>");
                                                        for (i = 0; i < WorkFlowlist.length; i++) {
                                                            $(elementIDs.taskAssigWatchWorkFlowStatusList).append("<option value=" + WorkFlowlist[i].WFStateID + ">" + WorkFlowlist[i].WFStateName + "</option>");
                                                        }
                                                        $(elementIDs.taskAssigWatchWorkFlowStatusList).val(UPTWFStateID).change();
                                                        // Populate Tasks combo box
                                                        var wfStateId = $(elementIDs.taskAssigWatchWorkFlowStatusList).val();
                                                        if (wfStateId != undefined && wfStateId != null) {
                                                            $(elementIDs.taskAssigWatchTasksList).empty();
                                                            var url = '/Task/GetDPFTasksMasterList?wfStateId={wfStateId}';
                                                            var promiseTaskList = ajaxWrapper.getJSON(url.replace(/{wfStateId}/g, wfStateId));
                                                            ajaxDialog.hidePleaseWait();
                                                            ajaxDialog.showPleaseWait();

                                                            promiseTaskList.done(function (WFStatelist) {
                                                                $(elementIDs.taskAssigWatchTasksList).empty();
                                                                $(elementIDs.taskAssigWatchTasksList).append("<option value='0'>" + "Select One" + "</option>");
                                                                for (i = 0; i < WFStatelist.length; i++) {
                                                                    $(elementIDs.taskAssigWatchTasksList).append("<option value=" + WFStatelist[i].TaskID + ">" + WFStatelist[i].TaskDescription + "</option>");
                                                                }
                                                                $(elementIDs.taskAssigWatchTasksList).val(UPTTaskID).change();
                                                                //set ChangeEventTriggerByCode Flag to false when all fileds get populated 
                                                                $(elementIDs.ChangeEventTriggerByCode).text("false");
                                                                ajaxDialog.hidePleaseWait();
                                                            });
                                                            promiseTaskList.fail(showError);
                                                        }

                                                        //PopulateViews
                                                        $(elementIDs.taskAssigWatchViewList).empty();
                                                        //var url = '/PlanTaskUserMapping/GetFormDesignVersionList';
                                                        var url = '/PlanTaskUserMapping/GetFormDesignVersionList?folderVersionId=' + folderVersionIdValue;
                                                        var promiseViewList = ajaxWrapper.getJSON(url);
                                                        ajaxDialog.hidePleaseWait();
                                                        ajaxDialog.showPleaseWait();

                                                        promiseViewList.done(function (list) {
                                                            if (list.length > 1) {
                                                                var allViewIds = "";
                                                                $(list).each(function (index, value) {
                                                                    allViewIds = allViewIds + ',' + value.FormDesignVersionId;
                                                                });
                                                                allViewIds = allViewIds.substring(1, allViewIds.length);
                                                                $(elementIDs.taskAssigWatchViewList).append("<option value='" + allViewIds + "'>" + ALLViews + "</option>");
                                                            }
                                                            for (i = 0; i < list.length; i++) {
                                                                $(elementIDs.taskAssigWatchViewList).append("<option value=" + list[i].FormDesignVersionId + ">" + list[i].FormDesignName + "</option>");
                                                            }
                                                            if (designDetailsJson != null && designDetailsJson.FormDesignVersionLabel != ALLViews && UPViewID != undefined && UPViewID.indexOf(',') >= 0) {
                                                                var ids = UPViewID.split(',');
                                                                $(ids).each(function (i, e) {
                                                                    $(elementIDs.taskAssigWatchViewList + " option[value='" + e + "']").prop("selected", true);
                                                                });
                                                            }
                                                            else
                                                                $(elementIDs.taskAssigWatchViewList).val(UPViewID);//.change();

                                                            //Populate Sections

                                                            if (UPViewID != null && UPViewID != undefined && UPViewID != "") {
                                                                var url = '/PlanTaskUserMapping/GetSectionsList?tenantId={tenantId}&formDesignVersionId={formDesignVersionId}';
                                                                var promiseSectionList = ajaxWrapper.getJSON(url.replace(/{tenantId}/g, 1).replace(/{formDesignVersionId}/g, UPViewID));
                                                                ajaxDialog.hidePleaseWait();
                                                                ajaxDialog.showPleaseWait();

                                                                promiseSectionList.done(function (sections) {
                                                                    $(elementIDs.taskAssigWatchSectionList).empty();
                                                                    if (sections.length > 1) {
                                                                        var allSectionIds = "";
                                                                        $(sections).each(function (index, value) {
                                                                            allSectionIds = allSectionIds + ',' + value.ID;
                                                                        });
                                                                        allSectionIds = allSectionIds.substring(1, allSectionIds.length);
                                                                        $(elementIDs.taskAssigWatchSectionList).append("<option value='" + allSectionIds + "'>" + ALLSections + "</option>");
                                                                    }
                                                                    for (i = 0; i < sections.length; i++) {
                                                                        $(elementIDs.taskAssigWatchSectionList).append("<option value=" + sections[i].ID + ">" + sections[i].Label + "</option>");
                                                                    }
                                                                    if (designDetailsJson != null && designDetailsJson.SectionLabel != ALLSections && UPSectionID != undefined && UPSectionID.indexOf(',') >= 0) {
                                                                        var ids = UPSectionID.split(',');
                                                                        $(ids).each(function (i, e) {
                                                                            $(elementIDs.taskAssigWatchSectionList + " option[value='" + e + "']").prop("selected", true);
                                                                        });
                                                                    }
                                                                    else
                                                                        $(elementIDs.taskAssigWatchSectionList).val(UPSectionID);//.change();

                                                                    $(elementIDs.taskAssigWatchOrder).val(UPOrder);
                                                                    $(elementIDs.taskAssigWatchDuration).val(UPDuration);

                                                                    if (UPAttachment != null) {
                                                                        $(elementIDs.taskAssigWatchAttachmentChangeDiv).css('display', 'block');
                                                                        $("#taskAssigWatchAttachmentUploadDiv").css('display', 'none');
                                                                        $(elementIDs.watchAttachmentName).attr('title', UPAttachment);
                                                                    }
                                                                    else {
                                                                        $("#taskAssigWatchAttachmentUploadDiv").css('display', 'block');
                                                                        $(elementIDs.taskAssigWatchAttachmentChangeDiv).css('display', 'none');
                                                                    }
                                                                    ajaxDialog.hidePleaseWait();
                                                                });
                                                                promiseSectionList.fail(showError);
                                                            } else {
                                                                $(elementIDs.taskAssigWatchSectionList).empty();
                                                            }
                                                        });
                                                        promiseViewList.fail(showError);
                                                    });
                                                    promiseWorkFlowStatusList.fail(showError);
                                                }
                                            });
                                            promisePlanList.fail(showError);
                                        }
                                    });
                                    promiseVerList.fail(showError);
                                }

                                //$(elementIDs.taskAssigWatchAccountList).empty();
                                //$(elementIDs.taskAssigWatchAccountList).append("<option value='" + accountID + "'>" + accountName.split('|')[0] + "</option>");
                            }
                        });
                        promiseGetPlanTaskUserMapping.fail(showError);
                    }
                });
                promiseTaskStatusList.fail(showError);
            });
            promiseUserList.fail(showError);
        }
        function ClearTaskAssignmentControlsData(isAccount) {
            if (!isAccount) {
                $(elementIDs.taskAssigWatchFolderList).empty();
                $(elementIDs.taskAssigWatchVersionList).empty();
                $(elementIDs.taskAssigWatchWorkFlowStatusList).empty();
                $(elementIDs.taskAssigWatchPlansList).empty();
                $(elementIDs.taskAssigWatchTasksList).empty();
                $(elementIDs.taskAssigWatchAssignUserList).empty();
                $(elementIDs.taskAssigWatchTaskStatus).empty();
                $(elementIDs.taskAssigWatchComments).val("");
                $(elementIDs.taskAssigWatchStartdate).empty();
                $(elementIDs.taskAssigWatchDuedate).empty();
                $(elementIDs.taskAssigWatchStartdate).val("");
                $(elementIDs.taskAssigWatchDuedate).val("");
                $(elementIDs.taskAssigWatchEstTime).val("0");
                $(elementIDs.taskAssigWatchActualTime).val("0");
            }
            else {
                $(elementIDs.taskAssigWatchFolderList).empty();
                $(elementIDs.taskAssigWatchVersionList).empty();
                $(elementIDs.taskAssigWatchWorkFlowStatusList).empty();
                $(elementIDs.taskAssigWatchPlansList).empty();
                $(elementIDs.taskAssigWatchTasksList).empty();
            }

            if ($("input[name='optradioWatchFolderType']:checked")[0].id == "optradioWatchPortfolio") {
                $(elementIDs.taskAssigWatchAccountListLabelDiv).css('display', 'none');
                $(elementIDs.taskAssigWatchAccountListDiv).css('display', 'none');
                $(elementIDs.taskAssigWatchAccountList).val("");

            }
            else {
                $(elementIDs.taskAssigWatchAccountListLabelDiv).css('display', 'block');
                $(elementIDs.taskAssigWatchAccountListDiv).css('display', 'block');
            }
        }
        function EnableTaskAssignmentConstrols() {
            $(elementIDs.taskAssigWatchFolderList).removeAttr("disabled");
            $(elementIDs.taskAssigWatchVersionList).removeAttr("disabled");
            $(elementIDs.taskAssigWatchWorkFlowStatusList).removeAttr("disabled");
            $(elementIDs.taskAssigWatchPlansList).removeAttr("disabled");
            $(elementIDs.taskAssigWatchTasksList).removeAttr("disabled");
            $(elementIDs.taskAssigWatchAssignUserList).removeAttr("disabled");
            $(elementIDs.taskAssigWatchTaskStatus).removeAttr("disabled");
            $(elementIDs.taskAssigWatchComments).removeAttr("disabled");
            $(elementIDs.taskAssigWatchStartdate).removeAttr("disabled");
            $(elementIDs.taskAssigWatchDuedate).removeAttr("disabled");
            $(elementIDs.taskAssigWatchSaveBtn).removeAttr("disabled");
            $(elementIDs.createNewTasksBtn).removeAttr("disabled");
            $(elementIDs.taskAssigWatchStartdate).datepicker('enable');
            $(elementIDs.taskAssigWatchDuedate).datepicker('enable');
            $(elementIDs.taskAssigWatchEstTime).removeAttr("disabled");
            $(elementIDs.taskAssigWatchActualTime).removeAttr("disabled");
            $(elementIDs.taskAssigWatchAccountList).removeAttr("disabled");
            $('input[name=optradioWatchFolderType]').removeAttr("disabled");
        }
        function DisableTaskAssignmentConstrols(isUpdate) {
            $(elementIDs.taskAssigWatchFolderList).attr("disabled", "disabled");
            $(elementIDs.taskAssigWatchVersionList).attr("disabled", "disabled");

            $(elementIDs.taskAssigWatchAccountList).attr("disabled", "disabled");
            $('input[name=optradioWatchFolderType]').attr("disabled", "disabled");
            if (!isUpdate) {
                $(elementIDs.taskAssigWatchPlansList).attr("disabled", "disabled");
                $(elementIDs.taskAssigWatchTasksList).attr("disabled", "disabled");
                $(elementIDs.taskAssigWatchAssignUserList).attr("disabled", "disabled");
                $(elementIDs.taskAssigWatchTaskStatus).attr("disabled", "disabled");
                $(elementIDs.taskAssigWatchComments).attr("disabled", "disabled");
                $(elementIDs.taskAssigWatchStartdate).attr("disabled", "disabled");
                $(elementIDs.taskAssigWatchDuedate).attr("disabled", "disabled");
                $(elementIDs.taskAssigWatchSaveBtn).attr("disabled", "disabled");
                $(elementIDs.createNewTasksBtn).attr("disabled", "disabled");
                $(elementIDs.taskAssigWatchStartdate).datepicker('disable');
                $(elementIDs.taskAssigWatchDuedate).datepicker('disable');
                $(elementIDs.taskAssigWatchViewList).attr("disabled", "disabled");
                $(elementIDs.taskAssigWatchSectionList).attr("disabled", "disabled");
                $(elementIDs.taskAssigWatchOrder).attr("disabled", "disabled");
                $(elementIDs.taskAssigWatchDuration).attr("disabled", "disabled");
    
                $(elementIDs.taskAssigWatchAttachment).attr("disabled", "disabled");
                $(elementIDs.taskAssigWatchChangeAttachment).attr("disabled", "disabled");
                $(elementIDs.taskAssigWatchEstTime).attr("disabled", "disabled");
                $(elementIDs.taskAssigWatchActualTime).attr("disabled", "disabled");
                $(elementIDs.taskAssigWatchWorkFlowStatusList).attr("disabled", "disabled");
            }
        }
        function loadTaskAssignmentViewSettings(rowId) {
            var folderIdValue = MappingRowIDForTask;
            if (folderIdValue == null || folderIdValue == undefined) {
                folderIdValue = "3";
            }

            var url = '/PlanTaskUserMapping/GetPlanTaskUserMappingList?PlanTaskUserMappingId={PlanTaskUserMappingId}';
            var promise = ajaxWrapper.getJSON(url.replace(/{PlanTaskUserMappingId}/g, folderIdValue));
            promise.done(function (list) {
                if (list != null && list != undefined && list.length > 0) {
                    var designDetailsJson = $.parseJSON(list[0].PlanTaskUserMappingDetails);
                    var plan = designDetailsJson != null ? designDetailsJson.FormInstanceLabel : "";
                    var accountName = list[0].AccountName;
                    var accountID;
                    if (accountName != undefined && accountName != "NA") {
                        accountID = accountName.split('|')[1];
                        $(elementIDs.taskAssigWatchAccountListLabelDiv).css('display', 'block');
                        $(elementIDs.taskAssigWatchAccountListDiv).css('display', 'block');
                        $("#optradioWatchAccount").prop('checked', 'checked');
                    }
                    else {
                        $(elementIDs.taskAssigWatchAccountListLabelDiv).css('display', 'none');
                        $(elementIDs.taskAssigWatchAccountListDiv).css('display', 'none');
                        $("#optradioWatchPortfolio").prop('checked', 'checked');
                    }
                    if(list[0].AssignedUserName != undefined)
                    {
                        list[0].AssignedUserName = list[0].AssignedUserName.split(',').join(", ");
                    }
                    else
                    {
                        list[0].AssignedUserName = ""; 
                    }
                    $(elementIDs.taskAssigWatchAccountList).empty();
                    $(elementIDs.taskAssigWatchAccountList).append("<option value='0'>" + accountName.split('|')[0] + "</option>");
                    $(elementIDs.taskAssigWatchFolderList).empty();
                    $(elementIDs.taskAssigWatchFolderList).append("<option value='0'>" + list[0].FolderName + "</option>");
                    $(elementIDs.taskAssigWatchVersionList).empty();
                    $(elementIDs.taskAssigWatchVersionList).append("<option value='0'>" + list[0].FolderVersionNumber + "</option>");
                    $(elementIDs.taskAssigWatchWorkFlowStatusList).empty();
                    $(elementIDs.taskAssigWatchWorkFlowStatusList).append("<option value='0'>" + list[0].WorkflowState + "</option>");
                    $(elementIDs.taskAssigWatchPlansList).empty();
                    $(elementIDs.taskAssigWatchPlansList).append("<option value='0'>" + plan + "</option>");
                    $(elementIDs.taskAssigWatchTasksList).empty();
                    $(elementIDs.taskAssigWatchTasksList).append("<option value='0'>" + list[0].TaskDescription + "</option>");
                    $(elementIDs.taskAssigWatchAssignUserList).empty();
                    $(elementIDs.taskAssigWatchAssignUserList).append("<option value='0'>" + list[0].AssignedUserName + "</option>");
                    $(elementIDs.taskAssigWatchTaskStatus).empty();
                    $(elementIDs.taskAssigWatchTaskStatus).append("<option value='0'>" + list[0].Status + "</option>");
                    var designDetailsJson = $.parseJSON(list[0].PlanTaskUserMappingDetails);
                    if (designDetailsJson != undefined)
                    {
                        $(elementIDs.taskAssigWatchViewList).empty();
                        $(elementIDs.taskAssigWatchViewList).append("<option value='0'>" + designDetailsJson.FormDesignVersionLabel + "</option>");
                        $(elementIDs.taskAssigWatchSectionList).empty();
                        $(elementIDs.taskAssigWatchSectionList).append("<option value='0'>" + designDetailsJson.SectionLabel + "</option>");
                    }
                   
                //    $(elementIDs.taskAssigWatchOrder).val('');
                    $(elementIDs.taskAssigWatchOrder).val(list[0].Order);
                    $(elementIDs.taskAssigWatchDuration).val(list[0].Duration);
                    //$(elementIDs.taskAssigWatchComments).val(list[0].Comments);
                    taskAssigWatchComments(rowId, "View");
                    var startDate = new Date(list[0].StartDate);
                    var startday = ("0" + startDate.getDate()).slice(-2);
                    var startmonth = ("0" + (startDate.getMonth() + 1)).slice(-2);
                    var startDateValue = (startmonth) + "/" + (startday) + "/" + startDate.getFullYear();
                    $(elementIDs.taskAssigWatchStartdate).val(startDateValue);
                    var dueDate = new Date(list[0].DueDate);
                    var dueday = ("0" + dueDate.getDate()).slice(-2);
                    var duemonth = ("0" + (dueDate.getMonth() + 1)).slice(-2);
                    var dueDateValue = (duemonth) + "/" + (dueday) + "/" + dueDate.getFullYear();
                    $(elementIDs.taskAssigWatchDuedate).val(dueDateValue);
                    $(elementIDs.taskAssigWatchEstTime).val(list[0].EstimatedTime);
                    $(elementIDs.taskAssigWatchActualTime).val(list[0].ActualTime);
                }
                DisableTaskAssignmentConstrols();
            });
            promise.fail(showError);
        }
        function fillTaskFoldersDropDown() {
            $(elementIDs.taskAssigWatchFolderList).empty();
            var accountID = $(elementIDs.taskAssigWatchAccountList).val();
            var url = '';
            if (accountID != null && accountID != 0) {
                url = '/FolderVersion/GetFolderList?tenantId=1&accountType=2&accountId={accountId}&categoryId=1';
                url = url.replace(/{accountId}/g, accountID);
            }
            else
                url = '/FolderVersion/GetTasksFoldersList';
            var promise = ajaxWrapper.getJSON(url);
            promise.done(function (list) {
                $(elementIDs.taskAssigWatchFolderList).empty();
                $(elementIDs.taskAssigWatchFolderList).append("<option value='0'>" + "Select One" + "</option>");
                if (accountID != null) {
                    var uniqueIds = [];
                    for (i = 0; i < list.length; i++) {
                        if ($.inArray(list[i].FolderId, uniqueIds) == -1) {
                            $(elementIDs.taskAssigWatchFolderList).append("<option value=" + list[i].FolderId + ">" + list[i].FolderName + "</option>");
                            uniqueIds.push(list[i].FolderId);
                        }
                    }
                }
                else {
                    for (i = 0; i < list.length; i++) {
                        $(elementIDs.taskAssigWatchFolderList).append("<option value=" + list[i].DocId + ">" + list[i].FormName + "</option>");
                    }
                }
                //$(elementIDs.taskAssigWatchFolderList).val("0").autoDropdown("refresh");
            });
            promise.fail(showError);
        }
        function fillTaskFolderVersionsDropDown() {
            $(elementIDs.taskAssigWatchVersionList).empty();
            var folderIdValue = $(elementIDs.taskAssigWatchFolderList).val();
            if (folderIdValue != null && folderIdValue != undefined && folderIdValue != "0") {
                var url = '/FolderVersion/GetTasksFolderVersionsList?folderId={folderId}';
                var promise = ajaxWrapper.getJSON(url.replace(/{folderId}/g, folderIdValue));
                promise.done(function (list) {
                    FolderVersionsList = list;
                    $(elementIDs.FolderVersionsListHDVal).val(JSON.stringify(FolderVersionsList));
                    $(elementIDs.taskAssigWatchVersionList).empty();
                    $(elementIDs.taskAssigWatchVersionList).append("<option value='0'>" + "Select One" + "</option>");
                    for (i = 0; i < list.length; i++) {
                        $(elementIDs.taskAssigWatchVersionList).append("<option value=" + list[i].FolderVersionId + ">" + list[i].FolderVersionNumber + "</option>");
                    }

                    if (IsPlanNewTaskUpdate == true) {
                        $(elementIDs.taskAssigWatchVersionList).val(UPTFolderVersionID).change();
                    }
                    else {
                        if (list.length > 0) {
                            //$(elementIDs.ChangeEventTriggerByCode).text("true");
                            $(elementIDs.taskAssigWatchVersionList).val(list[(list.length - 1)].FolderVersionId).change();
                            //$(elementIDs.ChangeEventTriggerByCode).text("false");
                        }
                    }
                    //$(elementIDs.taskAssigWatchVersionList).val("0").autoDropdown("refresh");
                    fillWorkFlowStatusDropDown();
                });
                promise.fail(showError);
            }
        }
        function fillWorkFlowStatusDropDown() {
            $(elementIDs.taskAssigWatchWorkFlowStatusList).empty();
            var folderIdValue = $(elementIDs.taskAssigWatchVersionList).val();
            var wfStateId;
            if (FolderVersionsList != undefined) {
                for (i = 0; i < FolderVersionsList.length; i++) {
                    if (FolderVersionsList[i].FolderVersionId == folderIdValue) {
                        wfStateId = FolderVersionsList[i].WFStateID;
                    }
                }
                if (wfStateId != undefined && wfStateId != null) {
                    var folderVersionIdValue = $(elementIDs.taskAssigWatchVersionList).val();
                    var url = '/WorkFlow/GetTasksWorkFlowStateListGreaterThanSelected?wfStateId={wfStateId}&&folderversionId=' + folderVersionIdValue;
                    var promise = ajaxWrapper.getJSON(url.replace(/{wfStateId}/g, wfStateId));
                    promise.done(function (list) {
                        $(elementIDs.taskAssigWatchWorkFlowStatusList).empty();
                        $(elementIDs.taskAssigWatchWorkFlowStatusList).append("<option value='0'>" + "Select One" + "</option>");
                        for (i = 0; i < list.length; i++) {
                            $(elementIDs.taskAssigWatchWorkFlowStatusList).append("<option value=" + list[i].WFStateID + ">" + list[i].WFStateName + "</option>");
                        }

                        if (IsPlanNewTaskUpdate == true) {
                            $(elementIDs.taskAssigWatchWorkFlowStatusList).val(UPTWFStateID).change();

                        }
                    });
                    promise.fail(showError);
                }
            }
        }
        function fillWatchPlansDropDown() {
            $(elementIDs.taskAssigWatchPlansList).empty();
            var folderIdValue = $(elementIDs.taskAssigWatchFolderList).val();
            var formDesignVersionID = 2364; //Medicare
            var mode = $(elementIDs.PlanTaskMode).text();
            var folderVersionIdValue = $(elementIDs.taskAssigWatchVersionList).val();
            if (folderIdValue != null && folderIdValue != undefined && folderIdValue != "0"
                && folderVersionIdValue != null && folderVersionIdValue != undefined && folderVersionIdValue != "0") {
                var url = '/PlanTaskUserMapping/GetTasksFormInstanceListForFolderVersion?folderVersionId={folderVersionId}&folderId={folderId}&formDesignVersionId={formDesignVersionId}';
                var promise = ajaxWrapper.getJSON(url.replace(/{folderVersionId}/g, folderVersionIdValue).replace(/{folderId}/g, folderIdValue).replace(/{formDesignVersionId}/g, formDesignVersionID));
                promise.done(function (list) {
                    $(elementIDs.taskAssigWatchPlansList).empty();
                    //$(elementIDs.taskAssigWatchPlansList).append("<option value='0'>" + "Select One" + "</option>");
                    if (mode != "Edit") {
                        if (list.length > 1) {
                            //var allPlanIds = "";
                            //$(list).each(function (index, value) {
                            //    allPlanIds = allPlanIds + ',' + value.ID;
                            //});
                            //allPlanIds = allPlanIds.substring(1, allPlanIds.length);
                            $(elementIDs.taskAssigWatchPlansList).append("<option value='0'>" + ALLPlans + "</option>");
                        }
                    }
                    for (i = 0; i < list.length; i++) {
                        $(elementIDs.taskAssigWatchPlansList).append("<option value=" + list[i].FormInstanceID + ">" + list[i].FormDesignName + "</option>");
                    }

                    if (IsPlanNewTaskUpdate == true) {
                        $(elementIDs.taskAssigWatchPlansList).val(UPTFormInstanceId).change();
                    }

                });
                promise.fail(showError);
            }
        }
        function fillWatchSectionsDropdown() {
            $(elementIDs.taskAssigWatchSectionList).empty();
            var formDesignVersionID = $(elementIDs.taskAssigWatchViewList).val();
            if (formDesignVersionID != null && formDesignVersionID != undefined && formDesignVersionID != "0") {
                var url = '/PlanTaskUserMapping/GetSectionsList?tenantId={tenantId}&formDesignVersionId={formDesignVersionId}';
                var promise = ajaxWrapper.getJSON(url.replace(/{tenantId}/g, 1).replace(/{formDesignVersionId}/g, formDesignVersionID));
                promise.done(function (sections) {
                    $(elementIDs.taskAssigWatchSectionList).empty();
                    //$(elementIDs.taskAssigWatchSectionList).append("<option value='0'>" + "Select One" + "</option>");
                    if (sections.length > 1) {
                        var allSectionIds = "";
                        $(sections).each(function (index, value) {
                            allSectionIds = allSectionIds + ',' + value.ID;
                        });
                        allSectionIds = allSectionIds.substring(1, allSectionIds.length);
                        $(elementIDs.taskAssigWatchSectionList).append("<option value='" + allSectionIds + "'>" + ALLSections + "</option>");
                    }
                    for (i = 0; i < sections.length; i++) {
                        $(elementIDs.taskAssigWatchSectionList).append("<option value=" + sections[i].ID + ">" + sections[i].Label + "</option>");
                    }
                    //fillWatchPlansDropDown();
                });
                promise.fail(showError);
            }
        }
        function fillWatchTasksDropDown() {
            var wfStateId = $(elementIDs.taskAssigWatchWorkFlowStatusList).val();
            if (wfStateId != undefined && wfStateId != null) {
                $(elementIDs.taskAssigWatchTasksList).empty();
                var url = '/Task/GetDPFTasksMasterList?wfStateId={wfStateId}';
                var promise = ajaxWrapper.getJSON(url.replace(/{wfStateId}/g, wfStateId));
                promise.done(function (list) {
                    $(elementIDs.taskAssigWatchTasksList).empty();
                    $(elementIDs.taskAssigWatchTasksList).append("<option value='0'>" + "Select One" + "</option>");
                    for (i = 0; i < list.length; i++) {
                        $(elementIDs.taskAssigWatchTasksList).append("<option value=" + list[i].TaskID + ">" + list[i].TaskDescription + "</option>");
                    }
                    //$(elementIDs.taskAssigWatchTasksList).val("0").autoDropdown("refresh");
                    if (IsPlanNewTaskUpdate == true) {
                        $(elementIDs.taskAssigWatchTasksList).val(UPTTaskID).change();
                    }
                });
                promise.fail(showError);
            }
        }
        function fillWatchAssignUserList() {
            $(elementIDs.taskAssigWatchAssignUserList).empty();
            var folderIdValue = $(elementIDs.taskAssigWatchFolderList).val();
            var folderVersionIdValue = $(elementIDs.taskAssigWatchVersionList).val();
            var loginUserName = "";
            var url = '/PlanTaskUserMapping/GetTasksTeamMemberList?strUserName={strUserName}';
            var promise = ajaxWrapper.getJSON(url.replace(/{strUserName}/g, loginUserName));
            promise.done(function (list) {
                $(elementIDs.taskAssigWatchAssignUserList).empty();
                for (i = 0; i < list.length; i++) {
                    $(elementIDs.taskAssigWatchAssignUserList).append("<option value=" + list[i].Key + ">" + list[i].Value + "</option>");
                }

                //$(elementIDs.taskAssigWatchAssignUserList).val("0").autoDropdown("refresh");

            });
            promise.fail(showError);
        }
        function fillWatchTaskStatusList() {
            $(elementIDs.taskAssigWatchTaskStatus).empty();
            var folderIdValue = $(elementIDs.taskAssigWatchFolderList).val();
            var folderVersionIdValue = $(elementIDs.taskAssigWatchVersionList).val();
            var loginUserName = "";
            var url = '/PlanTaskUserMapping/GetWatchTaskStatusList';
            var promise = ajaxWrapper.getJSON(url);
            promise.done(function (list) {
                $(elementIDs.taskAssigWatchTaskStatus).empty();
                $(elementIDs.taskAssigWatchTaskStatus).append("<option value='0'>" + "Select One" + "</option>");
                for (i = 0; i < list.length; i++) {
                    $(elementIDs.taskAssigWatchTaskStatus).append("<option value=" + list[i].TaskStatusID + ">" + list[i].TaskStatusDescription + "</option>");
                }
                $(elementIDs.taskAssigWatchTaskStatus).val(1);
                //$(elementIDs.taskAssigWatchTaskStatus).val("0").autoDropdown("refresh");

            });
            promise.fail(showError);
        }
        function fillWatchTaskPrioityList() {
            $(elementIDs.taskAssigWatchOrder).empty();
            var url = '/PlanTaskUserMapping/GetWatchTaskPriorityList';
            var promise = ajaxWrapper.getJSON(url);
            promise.done(function (list) {
                $(elementIDs.taskAssigWatchOrder).empty();
             //   $(elementIDs.taskAssigWatchOrder).append("<option value='0'>" + "Select One" + "</option>");
                for (i = 0; i < list.length; i++) {
                    $(elementIDs.taskAssigWatchOrder).append("<option value=" + list[i].ID + ">" + list[i].Description + "</option>");
                }
                $(elementIDs.taskAssigWatchOrder).val(1);
                //$(elementIDs.taskAssigWatchTaskStatus).val("0").autoDropdown("refresh");

            });
            promise.fail(showError);
        }
        function fillWatchViewDropDown() {
            var folderVersionIdValue = $(elementIDs.taskAssigWatchVersionList).val();
            var url = '/PlanTaskUserMapping/GetFormDesignVersionList?folderVersionId=' + folderVersionIdValue;
            var promise = ajaxWrapper.getJSON(url);
            promise.done(function (list) {
                $(elementIDs.taskAssigWatchViewList).empty();
                //$(elementIDs.taskAssigWatchViewList).append("<option value='0'>" + "Select One" + "</option>");
                if (list.length > 1) {
                    var allViewIds = "";
                    $(list).each(function (index, value) {
                        allViewIds = allViewIds + ',' + value.FormDesignVersionId;
                    });
                    allViewIds = allViewIds.substring(1, allViewIds.length);
                    $(elementIDs.taskAssigWatchViewList).append("<option value='" + allViewIds + "'>" + ALLViews + "</option>");
                }
                for (i = 0; i < list.length; i++) {
                    $(elementIDs.taskAssigWatchViewList).append("<option value=" + list[i].FormDesignVersionId + ">" + list[i].FormDesignName + "</option>");
                }
                $(elementIDs.taskAssigWatchViewList).val(0);
                //$(elementIDs.taskAssigWatchTaskStatus).val("0").autoDropdown("refresh");

            });
            promise.fail(showError);
        }
        function fillWatchAccountsDropDown() {
            var url = '/ConsumerAccount/GetAccountList?tenantId=1';
            var accountId = $(elementIDs.taskAssigWatchAccountList).val();
            var promise = ajaxWrapper.getJSON(url);
            promise.done(function (list) {
                $(elementIDs.taskAssigWatchAccountList).empty();
                $(elementIDs.taskAssigWatchAccountList).append("<option value='0'>" + "Select One" + "</option>");
                for (i = 0; i < list.length; i++) {
                    $(elementIDs.taskAssigWatchAccountList).append("<option value=" + list[i].AccountID + ">" + list[i].AccountName + "</option>");
                }
                $(elementIDs.taskAssigWatchAccountList).val(0);
            });
            promise.fail(showError);
        }
        function SaveTaskAssignmentOperation() {
            var saveDone = $(elementIDs.HiddenParameter).text();
            if (saveDone == "SaveEditUnDone") {
                var performOperation = true;
                // var strErrorList = "Fields required for :";
                var isLateTaskStatus = $(elementIDs.IsTaskLateStatus).text();
                var mode = $(elementIDs.PlanTaskMode).text();
                if ($("input[name='optradioWatchFolderType']:checked")[0].id != "optradioWatchPortfolio") {
                    var accountID = $(elementIDs.taskAssigWatchAccountList).val();
                    if (accountID == null || accountID == undefined || accountID == "0") {
                        $(elementIDs.taskAssigWatchAccountList).parent().addClass('has-error');
                        $(elementIDs.taskAssigWatchAccountList).addClass('form-control');
                        $(elementIDs.taskAssigWatchAccountListSpan).text(DashBoard.selectAccountRequiredMsg);
                        //  strErrorList = strErrorList + " WorkflowStatus";
                        performOperation = false;
                    }
                    else {
                        $(elementIDs.taskAssigWatchAccountList).parent().addClass('has-error');
                        $(elementIDs.taskAssigWatchAccountList).removeClass('form-control');
                        $(elementIDs.taskAssigWatchAccountListSpan).text('');
                    }
                }
                var foldeName = $(elementIDs.taskAssigWatchFolderList + ' option:selected').text();
                var folderId = $(elementIDs.taskAssigWatchFolderList).val();
                if (folderId == null || folderId == undefined || folderId == "0" || folderName == "Select One") {
                    $(elementIDs.taskAssigWatchFolderList).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchFolderList).addClass('form-control');
                    $(elementIDs.taskAssigWatchFolderListSpan).text(DashBoard.selectFolderRequiredMsg);
                    //  strErrorList = strErrorList + " WorkflowStatus";
                    performOperation = false;
                }
                else {
                    $(elementIDs.taskAssigWatchFolderList).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchFolderList).removeClass('form-control');
                    $(elementIDs.taskAssigWatchFolderListSpan).text('');
                }
                var folderversionName = $(elementIDs.taskAssigWatchVersionList + ' option:selected').text();
                var folderversion = $(elementIDs.taskAssigWatchVersionList).val();
                if (folderversion == null || folderversion == undefined || folderversion == "0" || folderversionName == "Select One") {
                    $(elementIDs.taskAssigWatchVersionList).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchVersionList).addClass('form-control');
                    $(elementIDs.taskAssigWatchVersionListSpan).text(DashBoard.selectFoldeVersionRequiredMsg);
                    //   strErrorList = strErrorList + " WorkflowStatus";
                    performOperation = false;
                }
                else {
                    $(elementIDs.taskAssigWatchVersionList).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchVersionList).removeClass('form-control');
                    $(elementIDs.taskAssigWatchVersionListSpan).text('');
                }
                var wfStateName = $(elementIDs.taskAssigWatchWorkFlowStatusList + ' option:selected').text();
                var wfStateId = $(elementIDs.taskAssigWatchWorkFlowStatusList).val();
                if (wfStateId == null || wfStateId == undefined || wfStateId == "0" || wfStateName == "Select One") {

                    $(elementIDs.taskAssigWatchWorkFlowStatusList).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchWorkFlowStatusList).addClass('form-control');
                    $(elementIDs.taskAssigWatchWorkFlowStatusListSpan).text(DashBoard.selectWorkFlowStateRequiredMsg);
                    //     strErrorList = strErrorList + " WorkflowStatus";
                    performOperation = false;
                }
                else {
                    $(elementIDs.taskAssigWatchWorkFlowStatusList).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchWorkFlowStatusList).removeClass('form-control');
                    $(elementIDs.taskAssigWatchWorkFlowStatusListSpan).text('');
                }
                var formInstanceName = $(elementIDs.taskAssigWatchPlansList + ' option:selected').text();
                var formInstanceId = $(elementIDs.taskAssigWatchPlansList).val();
                if (formInstanceName == "" && $(elementIDs.taskAssigWatchPlansList).is(':enabled')) {
                    $(elementIDs.taskAssigWatchPlansList).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchPlansList).addClass('form-control');
                    $(elementIDs.taskAssigWatchPlansListSpan).text(DashBoard.selectPlanRequiredMsg);
                    //       strErrorList = strErrorList + ", Plans";
                    performOperation = false;
                }
                else {
                    if (formInstanceName.indexOf(ALLPlans) >= 0 && formInstanceName.length > ALLPlans.length) {
                        $(elementIDs.taskAssigWatchPlansList).parent().addClass('has-error');
                        $(elementIDs.taskAssigWatchPlansList).addClass('form-control');
                        $(elementIDs.taskAssigWatchPlansListSpan).text(DashBoard.selectPlanValidationMsg);
                        //       strErrorList = strErrorList + ", Plans";
                        performOperation = false;
                    }
                    else {
                        $(elementIDs.taskAssigWatchPlansList).parent().addClass('has-error');
                        $(elementIDs.taskAssigWatchPlansList).removeClass('form-control');
                        $(elementIDs.taskAssigWatchPlansListSpan).text('');
                    }
                }
                var taskName = $(elementIDs.taskAssigWatchTasksList + ' option:selected').text();
                var taskId = $(elementIDs.taskAssigWatchTasksList).val();
                if (taskId == null || taskId == undefined || taskId == "0" || taskName == "Select One") {
                    $(elementIDs.taskAssigWatchTasksList).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchTasksList).addClass('form-control');
                    $(elementIDs.taskAssigWatchTasksListSpan).text(DashBoard.selectTaskRequiredMsg);
                    //       strErrorList = strErrorList + ", Tasks";
                    performOperation = false;
                }
                else {
                    $(elementIDs.taskAssigWatchTasksList).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchTasksList).removeClass('form-control');
                    $(elementIDs.taskAssigWatchTasksListSpan).text('');
                }
                var assignedUserName = $(elementIDs.taskAssigWatchAssignUserList + ' option:selected').text();
                var assignedUserNameVal = $(elementIDs.taskAssigWatchAssignUserList).val();
                if (assignedUserName == null || assignedUserName == undefined || assignedUserName == "" || assignedUserName == "Select One") {
                    $(elementIDs.taskAssigWatchAssignUserList).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchAssignUserList).addClass('form-control');
                    $(elementIDs.taskAssigWatchAssignUserListSpan).text(DashBoard.selectAssignUserRequiredMsg);
                    //      strErrorList = strErrorList + ", Assign User";
                    performOperation = false;
                }
                else {
                    $(elementIDs.taskAssigWatchAssignUserList).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchAssignUserList).removeClass('form-control');
                    $(elementIDs.taskAssigWatchAssignUserListSpan).text('');
                    assignedUserName = "";
                    $(elementIDs.taskAssigWatchAssignUserList + " > option:selected").each(function () {
                        var selText = $(this).text();
                        if (selText != "") {
                            assignedUserName = assignedUserName + "," + selText;
                        }
                    });
                    assignedUserName = assignedUserName.substring(1, assignedUserName.length);
                }
                var taskStatus = $(elementIDs.taskAssigWatchTaskStatus + ' option:selected').text();
                var taskStatusVal = $(elementIDs.taskAssigWatchTaskStatus).val();
                if (taskStatus == null || taskStatus == undefined || taskStatusVal == "0" || taskStatus == "" || taskStatus == "Select One") {
                    //if (isLateTaskStatus != "true") {
                    $(elementIDs.taskAssigWatchTaskStatus).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchTaskStatus).addClass('form-control');
                    $(elementIDs.taskAssigWatchTaskStatusSpan).text(DashBoard.selectTaskStatusRequiredMsg);
                    //       strErrorList = strErrorList + ", Task Status";
                    performOperation = false;
                    //}
                }
                else {
                    $(elementIDs.taskAssigWatchTaskStatus).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchTaskStatus).removeClass('form-control');
                    $(elementIDs.taskAssigWatchTaskStatusSpan).text('');
                }
                var managerUserName = "mgr";
                var startDate = new Date($(elementIDs.taskAssigWatchStartdate).val());
                var startDF = startDate.getFullYear() + '-' + (startDate.getMonth() + 1) + '-' + startDate.getDate();
                if (startDate == null || startDate == undefined || startDate == "Invalid Date" || startDF == "NaN-NaN-NaN") {
                    $(elementIDs.taskAssigWatchStartdate).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchStartdate).addClass('form-control');
                    $(elementIDs.taskAssigWatchStartdateSpan).text(DashBoard.selectStartDateRequiredMsg);
                    //       strErrorList = strErrorList + ", Start date";
                    performOperation = false;
                }
                else {
                    $(elementIDs.taskAssigWatchStartdate).parent().removeClass('has-error');
                    $(elementIDs.taskAssigWatchStartdate).removeClass('form-control');
                    $(elementIDs.taskAssigWatchStartdateSpan).text('');
                }
                var dueDate = new Date($(elementIDs.taskAssigWatchDuedate).val());
                var dueDateDF = dueDate.getFullYear() + '-' + (dueDate.getMonth() + 1) + '-' + dueDate.getDate();
                if (dueDate == null || dueDate == undefined || dueDate == "Invalid Date" || dueDateDF == "NaN-NaN-NaN") {
                    $(elementIDs.taskAssigWatchDuedate).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchDuedate).addClass('form-control');
                    $(elementIDs.taskAssigWatchDuedateSpan).text(DashBoard.selectDueDateRequiredMsg);
                    //        strErrorList = strErrorList + ", Due date";
                    performOperation = false;
                }
                else {
                    $(elementIDs.taskAssigWatchDuedate).parent().removeClass('has-error');
                    $(elementIDs.taskAssigWatchDuedate).removeClass('form-control');
                    $(elementIDs.taskAssigWatchDuedateSpan).text('');
                }
                var formDesignVersionNames = $(elementIDs.taskAssigWatchViewList + ' option:selected').text();
                var formDesignVersionIds = $(elementIDs.taskAssigWatchViewList).val();
                if (formDesignVersionIds != null && typeof formDesignVersionIds == "object") {
                    formDesignVersionIds = formDesignVersionIds.toString();
                }
                if (formDesignVersionNames.indexOf(ALLViews) >= 0 && formDesignVersionNames.length > ALLViews.length) {
                    $(elementIDs.taskAssigWatchViewList).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchViewList).addClass('form-control');
                    $(elementIDs.taskAssigWatchViewListSpan).text(DashBoard.selectViewValidationMsg);
                    //       strErrorList = strErrorList + ", Plans";
                    performOperation = false;
                }
                else {
                    $(elementIDs.taskAssigWatchViewList).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchViewList).removeClass('form-control');
                    $(elementIDs.taskAssigWatchViewListSpan).text('');
                }
                var sectionNames = $(elementIDs.taskAssigWatchSectionList + ' option:selected').text();
                var sectionNamesIds = $(elementIDs.taskAssigWatchSectionList).val();
                if (sectionNamesIds != null && typeof sectionNamesIds == "object") {
                    sectionNamesIds = sectionNamesIds.toString();
                }
                if (sectionNames.indexOf(ALLSections) >= 0 && sectionNames.length > ALLSections.length) {
                    $(elementIDs.taskAssigWatchSectionList).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchSectionList).addClass('form-control');
                    $(elementIDs.taskAssigWatchSectionListSpan).text(DashBoard.selectSectionValidationMsg);
                    //       strErrorList = strErrorList + ", Plans";
                    performOperation = false;
                }
                else {
                    $(elementIDs.taskAssigWatchSectionList).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchSectionList).removeClass('form-control');
                    $(elementIDs.taskAssigWatchSectionListSpan).text('');
                }
                var taskOrder = $(elementIDs.taskAssigWatchOrder).val();
                var taskDuration = $(elementIDs.taskAssigWatchDuration).val();


                var decimal = /^\d*[0-9]\d*$/;
                var estimatedTime = $(elementIDs.taskAssigWatchEstTime).val();
                if (!$.isNumeric(estimatedTime) || !estimatedTime.toString().match(decimal)) {
                    $(elementIDs.taskAssigWatchEstTime).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchEstTime).addClass('form-control');
                    $(elementIDs.taskAssigWatchEstTimeSpan).text(DashBoard.estimateTimeValidationMsg);
                    //       strErrorList = strErrorList + ", Tasks";
                    performOperation = false;
                }
                else {
                    $(elementIDs.taskAssigWatchEstTime).parent().removeClass('has-error');
                    $(elementIDs.taskAssigWatchEstTime).removeClass('form-control');
                    $(elementIDs.taskAssigWatchEstTimeSpan).text('');
                }

                var actualTime = $(elementIDs.taskAssigWatchActualTime).val();
                if (!$.isNumeric(actualTime) || ! actualTime.match(decimal)) {
                    $(elementIDs.taskAssigWatchActualTime).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchActualTime).addClass('form-control');
                    $(elementIDs.taskAssigWatchActualTimeSpan).text(DashBoard.actualTimeValidationMsg);
                    //       strErrorList = strErrorList + ", Tasks";
                    performOperation = false;
                }
                else {
                    $(elementIDs.taskAssigWatchActualTime).parent().removeClass('has-error');
                    $(elementIDs.taskAssigWatchActualTime).removeClass('form-control');
                    $(elementIDs.taskAssigWatchActualTimeS).text('');
                }

                var durationTime = $(elementIDs.taskAssigWatchDuration).val();
                if (!$.isNumeric(durationTime) || !durationTime.match(decimal)) {
                    $(elementIDs.taskAssigWatchDuration).parent().addClass('has-error');
                    $(elementIDs.taskAssigWatchDuration).addClass('form-control');
                    $(elementIDs.taskAssigWatchDurationSpan).text(DashBoard.durationValidationMsg);
                    //       strErrorList = strErrorList + ", Tasks";
                    performOperation = false;
                }
                else {
                    $(elementIDs.taskAssigWatchDuration).parent().removeClass('has-error');
                    $(elementIDs.taskAssigWatchDuration).removeClass('form-control');
                    $(elementIDs.taskAssigWatchDuration).text('');
                }
                //Checking validation for Late Task Status
                var currentDate = new Date();
                var currDateNew = new Date((currentDate.getMonth() + 1) + "/" + currentDate.getDate() + "/" + currentDate.getFullYear());
                if (mode == "Edit") {
                    if (isLateTaskStatus == "true" && (taskStatus != "Completed" && taskStatus != "Completed - Fail" && taskStatus != "Completed - Pass")) {

                        if (dueDate < currDateNew) {
                            //$(elementIDs.taskAssigWatchDuedate).parent().addClass('has-error');
                            //$(elementIDs.taskAssigWatchDuedate).addClass('form-control');
                            //$(elementIDs.taskAssigWatchDuedateSpan).text(DashBoard.selectDueDateGreaterThanCurrentMsg);
                            //performOperation = false;
                        }
                        else {
                            if (taskStatus != "InProgress") {
                                taskStatus = "Assigned";
                            }
                            $(elementIDs.taskAssigWatchTaskStatus).parent().removeClass('has-error');
                            $(elementIDs.taskAssigWatchTaskStatus).removeClass('form-control');
                            $(elementIDs.taskAssigWatchTaskStatusSpan).text('');

                            $(elementIDs.taskAssigWatchDuedate).parent().removeClass('has-error');
                            $(elementIDs.taskAssigWatchDuedate).removeClass('form-control');
                            $(elementIDs.taskAssigWatchDuedateSpan).text('');
                        }
                    }
                }
                if (taskStatus != "Completed" && taskStatus != "Completed - Fail" && taskStatus != "Completed - Pass") {
                    if (mode == "Edit") {
                        if (dueDate < currDateNew) {
                            //taskStatus = "Late";
                            //$(elementIDs.taskAssigWatchDuedate).parent().addClass('has-error');
                            //$(elementIDs.taskAssigWatchDuedate).addClass('form-control');
                            //$(elementIDs.taskAssigWatchDuedateSpan).text(DashBoard.selectDueDateGreaterThanCurrentMsg);
                            //performOperation = false;
                        }
                        else {
                            $(elementIDs.taskAssigWatchDuedate).parent().removeClass('has-error');
                            $(elementIDs.taskAssigWatchDuedate).removeClass('form-control');
                            $(elementIDs.taskAssigWatchDuedateSpan).text('');
                        }
                    }
                    else {
                        if (dueDate < currDateNew) {
                            taskStatus = "Late";
                        }
                    }
                }
                var day = startDate.getDate();
                var monthIndex = startDate.getMonth();
                var year = startDate.getFullYear();
                var taskDescription = $(elementIDs.taskAssigWatchTasksList + ' option:selected').text();
                var plan = $(elementIDs.taskAssigWatchPlansList + ' option:selected').text();
                var folderName = $(elementIDs.taskAssigWatchFolderList + ' option:selected').text();
                var workflowState = $(elementIDs.taskAssigWatchWorkFlowStatusList + ' option:selected').text();
                var folderVersionIdValue = $(elementIDs.taskAssigWatchVersionList).val()
                var effectiveDateString;
                var effectiveDate;
                var folderVersionNumber;
                MappingRowIDForTask = $(elementIDs.MappingRowIDForTaskHDVal).text();
                var PlanTaskUserMappingID = MappingRowIDForTask;
                FolderVersionsList = null;

                var hiddenArrayvalue = $(elementIDs.FolderVersionsListHDVal).val(); //retrieve array
                FolderVersionsList = JSON.parse(hiddenArrayvalue);

                if (FolderVersionsList != null) {
                    for (i = 0; i < FolderVersionsList.length; i++) {
                        if (FolderVersionsList[i].FolderVersionId == folderVersionIdValue) {
                            var effectiveDate = new Date(FolderVersionsList[i].EffectiveDate);
                            var effectiveDateDF = effectiveDate.getFullYear() + '-' + (effectiveDate.getMonth() + 1) + '-' + effectiveDate.getDate();
                            effectiveDateString = effectiveDateDF;
                            effectiveDate = effectiveDateDF;
                            folderVersionNumber = FolderVersionsList[i].FolderVersionNumber;
                        }
                    }
                }

                var rowIds = $(elementIDs.taskAssigWatchCommentsJQ).getDataIDs();
                var taskCommentsGridData = [];
                for (var i = rowIds.length ; i >= 0; i--) {
                    var rowData = $(elementIDs.taskAssigWatchCommentsJQ).getRowData(i);
                    if (rowData.IsNew == 1) {
                        var attachment = rowData.Attachment != "" ? $.parseHTML(rowData.Attachment)[0].id : ""
                        rowData.Attachment = attachment;
                        taskCommentsGridData.push(rowData);
                        rowData.IsNew = 0;
                    }
                }

                if (performOperation == true) {
                    var currentdate = new Date();
                    if ((new Date(startDate.toDateString()) >= new Date(currentdate.toDateString()) && mode == "Add") || mode == "Edit") {
                        if (dueDate >= startDate) {
                            IsPlanTaskInfoUpdated = false;
                            $(elementIDs.IsTaskInfoModifed).text("TaskInfoUnModifed");
                            var formInstanceList = [];
                            var formInstanceNameList = [];
                            var viewLabel = "";
                            var sectionLabel = "";
                            IsPlanNewTaskUpdate = false;
                            if (mode == "Edit") {
                                IsPlanNewTaskUpdate = true;
                            }
                            var planListSelectedVal = $(elementIDs.taskAssigWatchPlansList + ' option:selected').text();
                            if (planListSelectedVal != ALLPlans) {
                                $(elementIDs.taskAssigWatchPlansList + " > option:selected").each(function () {
                                    var selText = $(this).text();
                                    if (selText != "") {
                                        formInstanceList.push($(this).val());
                                        formInstanceNameList.push(selText);
                                    }
                                });
                            }
                            else {
                                $(elementIDs.taskAssigWatchPlansList + " option").each(function () {
                                    //alert(this.text + ' ' + this.value);
                                    if (this.text != "" && this.text != ALLPlans) {
                                        formInstanceList.push(this.value);
                                        formInstanceNameList.push(this.text);
                                    }
                                });
                                //$(elementIDs.taskAssigWatchPlansList).each(function () {
                                //    var selText = $(this).text();
                                //    if (selText != "" && selText != ALLPlans) {
                                //        formInstanceList.push($(this).val());
                                //        formInstanceNameList.push(selText);
                                //    }
                                //});
                                // formInstanceNameList.push(planListSelectedVal);
                            }

                            var viewListSelectedVal = $(elementIDs.taskAssigWatchViewList + ' option:selected').text();
                            if (viewListSelectedVal != ALLViews) {
                                $(elementIDs.taskAssigWatchViewList + " > option:selected").each(function () {
                                    var selText = $(this).text();
                                    if (selText != "") {
                                        viewLabel = viewLabel + "," + selText;
                                    }
                                });
                                viewLabel = viewLabel.substring(1, viewLabel.length);
                            }
                            else
                                viewLabel = viewListSelectedVal;

                            var sectionListSelectedVal = $(elementIDs.taskAssigWatchSectionList + ' option:selected').text();
                            if (sectionListSelectedVal != ALLSections) {
                                $(elementIDs.taskAssigWatchSectionList + " > option:selected").each(function () {
                                    var selText = $(this).text();
                                    if (selText != "") {
                                        sectionLabel = sectionLabel + "," + selText;
                                    }
                                });
                                sectionLabel = sectionLabel.substring(1, sectionLabel.length);
                            }
                            else
                                sectionLabel = sectionListSelectedVal;

                            var input = {
                                FormInstanceId: formInstanceId,
                                WFStateID: wfStateId,
                                TaskID: taskId,
                                AssignedUserName: assignedUserName,
                                ManagerUserName: managerUserName,
                                StartDateString: startDF,
                                DueDateString: dueDateDF,
                                Status: taskStatus,
                                StartDate: startDF,
                                DueDate: dueDateDF,
                                TaskDescription: taskDescription,
                                Plan: plan,
                                FolderName: folderName,
                                EffectiveDateString: effectiveDateString,
                                EffectiveDate: effectiveDate,
                                WorkflowState: workflowState,
                                FolderVersionNumber: folderVersionNumber,
                                ID: PlanTaskUserMappingID,
                                FormInstanceIdList: formInstanceList,
                                FormInstanceNameList: formInstanceNameList,
                                //Comments: commentsValue,
                                Order: taskOrder,
                                Duration : taskDuration,
                                SectionID: sectionNamesIds,
                                ViewID: formDesignVersionIds,
                                // Attachment: taskAttachmentFilePath,
                                FolderID: folderId,
                                FolderVersionID: folderVersionIdValue,
                                TaskComments: JSON.stringify(taskCommentsGridData),
                                ViewLabels: viewLabel,
                                SectionLabels: sectionLabel,
                                EstimatedTime: $(elementIDs.taskAssigWatchEstTime).val(),
                                ActualTime: $(elementIDs.taskAssigWatchActualTime).val(),
                                AccountName: $(elementIDs.taskAssigWatchAccountList + ' option:selected').text()
                            }
                            var url = null;
                            $(elementIDs.HiddenParameter).text("SaveEditDone");
                            if (IsPlanNewTaskUpdate == true) {
                                url = '/PlanTaskUserMapping/UpdateWatchPlanTaskUserMapping';
                            }
                            else {
                                url = '/PlanTaskUserMapping/SaveWatchPlanTaskUserMapping';
                            }
                            if (url != null) {
                                var promise = ajaxWrapper.postJSON(url, input);
                                promise.done(function (list) {
                                    ClearTaskAssignmentControlsData();
                                    IsPlanTaskInfoUpdated = false;
                                    $(elementIDs.IsTaskInfoModifed).text("TaskInfoUnModifed");
                                    var resultVal = list;
                                    if (resultVal == true || resultVal == "true") {
                                        messageDialog.show(DashBoard.planTaskUserMapSaveMsg);
                                    }
                                    //   $(elementIDs.watchGridJQ).trigger('reloadGrid');
                                    $(elementIDs.watchGridJQ).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
                                });

                                promise.fail(showError);
                            }
                            $(elementIDs.taskAssignmentWatchlist).dialog("close");
                            $(elementIDs.cancelWatchDflag).text("CancelDone");
                        }
                        else {
                            messageDialog.show(DashBoard.startDuedateValidationMsg);

                        }
                    }
                    else {
                        messageDialog.show(DashBoard.currentDateValidationMsg);
                    }
                }
            }
        }
        function userRoleSuccess(xhr, userAssignmentDialogState) {
            if (xhr.Result === ServiceResult.SUCCESS) {
                if (userAssignmentDialogState == "add") {
                    messageDialog.show(DashboardMessages.userAssignedSuccess);
                }
                else {
                    messageDialog.show(DashboardMessages.userUnAssignedSuccess);
                }
                isAddedUserAssigned = true;
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
            $(elementIDs.userAssignmentUpdateGridJQ).trigger('reloadGrid');
            //reset dialog elements
            $(elementIDs.userAssignUnassignDialog + ' div').removeClass('has-error');
            $(elementIDs.userAssignUnassignDialog + ' .help-block').text('');
            $(elementIDs.userAssignUnassignDialog).dialog('close');
        }

        function taskAssigWatchAttachment(mappingRowID) {
            $(elementIDs.taskAssigWatchAttachment).css('display', 'none');
            $(elementIDs.taskAssigWatchLabelAttachment).css('display', 'block');
            $(elementIDs.taskAssigWatchChangeAttachment).css('display', 'block');

            $(document).on("click", elementIDs.taskAssigWatchChangeAttachment, function (event) {
                $(elementIDs.taskAssignmentWatchlist).dialog("open");
                $(elementIDs.taskAssigWatchChangeAttachment).css('display', 'none');
                $(elementIDs.taskAssigWatchLabelAttachment).css('display', 'none');
                $(elementIDs.taskAssigWatchAttachment).css('display', 'normal');
            });



            var url = URLs.GetAttchment + "mappingRowID=" + mappingRowID



        }

        function saveInterstedTask(ischecked, rowId) {
            var message = "";
            if (ischecked) {
                message = message + "Are you sure you want to mark this task as interested task?";
                yesNoConfirmDialog.show(message, function (e) {
                    if (e) {
                        yesNoConfirmDialog.hide();
                        var rowdata = $(elementIDs.watchGridJQ).jqGrid('getRowData', rowId);
                        var promise = ajaxWrapper.getJSON(URLs.saveInterestedTask.replace(/{id}/g, rowdata.MappingRowID));
                        promise.done(function (list) {
                            $(elementIDs.watchGridJQ).trigger('reloadGrid');
                        });
                        promise.fail(showError);
                    }
                    else {
                        $(elementIDs.markInterstedCheckbox + '_' + rowId).prop('checked', false);
                    }
                });
            }
            else {
                message = message + "Are you sure you want to remove this task from interested task list?";
                yesNoConfirmDialog.show(message, function (e) {
                    if (e) {
                        yesNoConfirmDialog.hide();
                        var rowdata = $(elementIDs.watchGridJQ).jqGrid('getRowData', rowId);
                        var promise = ajaxWrapper.getJSON(URLs.removeInterestedTask.replace(/{id}/g, rowdata.MappingRowID));
                        promise.done(function (list) {
                            ischeckViewInserted = true;
                            $(elementIDs.watchGridJQ).trigger('reloadGrid');
                        });
                        promise.fail(showError);
                    }
                    else {
                        $(elementIDs.markInterstedCheckbox + '_' + rowId).prop('checked', true);
                    }
                });
            }
        }

        function saveInterstedAllTask(ischecked, isViewInsteresed) {
            var message = "";
            if (ischecked) {
                message = message + "Are you sure you want to mark all tasks as interested task?";
                yesNoConfirmDialog.show(message, function (e) {
                    if (e) {
                        yesNoConfirmDialog.hide();
                        var promise = ajaxWrapper.getJSON(URLs.saveInterestedAllTask.replace(/{isViewInterested}/g, isViewInsteresed).replace(/{viewMode}/g, taskGridViewMode).replace(/{value}/g, true));
                        promise.done(function (list) {
                            $(elementIDs.watchGridJQ).trigger('reloadGrid');
                        });
                        promise.fail(showError);
                    }
                    else {
                        $(elementIDs.markInterestedCheckboxJQ).filter(':not(:disabled)').prop('checked', false);
                        $(elementIDs.markInterstedSelectAllCheckboxJQ).prop('checked', false);
                    }
                });
            }
            else {
                message = message + "Are you sure you want to remove all tasks from interested task list?";
                yesNoConfirmDialog.show(message, function (e) {
                    if (e) {
                        yesNoConfirmDialog.hide();
                        var promise = ajaxWrapper.getJSON(URLs.saveInterestedAllTask.replace(/{isViewInterested}/g, isViewInsteresed).replace(/{viewMode}/g, taskGridViewMode).replace(/{value}/g, false));
                        promise.done(function (list) {
                            ischeckViewInserted = true;
                            $(elementIDs.watchGridJQ).trigger('reloadGrid');
                        });
                        promise.fail(showError);
                    }
                    else {
                        $(elementIDs.markInterestedCheckboxJQ).filter(':not(:disabled)').prop('checked', true);
                        $(elementIDs.markInterstedSelectAllCheckboxJQ).prop('checked', true);
                    }
                });
            }
        }

        function setInterstedFolderVersion(ischecked, rowId) {
            var message = "";
            if (ischecked) {
                message = message + "Are you sure you want to mark this folder version as interested folder version?";
                yesNoConfirmDialog.show(message, function (e) {
                    if (e) {
                        yesNoConfirmDialog.hide();
                        var rowdata = $(elementIDs.watchGridJQ).jqGrid('getRowData', rowId);
                        var promise = ajaxWrapper.getJSON(URLs.saveInterstedFolderVersion.replace(/{folderVersionId}/g, rowdata.FolderVersionId).replace(/{currentUserName}/g, rowdata.Owner));
                        promise.done(function (list) {

                        });
                        promise.fail(showError);
                    }
                    else {
                        $(elementIDs.markInterstedCheckbox + '_' + rowId).prop('checked', false);
                    }
                });
            }
            else {
                message = message + "Are you sure you want to remove this folder version from interested folder version list?";
                yesNoConfirmDialog.show(message, function (e) {
                    if (e) {
                        yesNoConfirmDialog.hide();
                        var rowdata = $(elementIDs.watchGridJQ).jqGrid('getRowData', rowId);
                        var promise = ajaxWrapper.getJSON(URLs.deleteInterstedFolderVersion.replace(/{folderVersionId}/g, rowdata.FolderVersionId));
                        promise.done(function (list) {
                            ischeckViewInserted = true;
                            $(elementIDs.watchGridJQ).trigger('reloadGrid');
                        });
                        promise.fail(showError);
                    }
                    else {
                        $(elementIDs.markInterstedCheckbox + '_' + rowId).prop('checked', true);
                    }
                });
            }
        }

        //contains functionality to related to Task Assignment
        function openTaskAssignmentWatchListDialog(transactionMode) {
            var rowId = $(elementIDs.watchGridJQ).getGridParam('selrow');
            var Title = 'Add New Task Assignment';
            if (transactionMode == 'Edit' || transactionMode == 'View') {
                if (rowId == undefined && rowId == null) {
                    $('#messagedialog').dialog({
                        title: 'Watch List',
                        height: 120
                    });
                    messageDialog.show(transactionMode == 'Edit' ? DashBoard.selectRowToShowTaskAssignmentEditMsg : DashBoard.selectRowToShowTaskAssignmentViewMsg);
                    return;
                } else {
                    Title = transactionMode == 'Edit' ? 'Edit Task Assignment' : 'View Task Assignment';
                }
            } else {
                rowId = 0;
            }
            $(elementIDs.taskAssignmentWatchlist).dialog({
                autoOpen: false,
                height: 500,
                width: 850,
                modal: true,
            });
            $(elementIDs.taskAssignmentWatchlist).dialog('option', 'title', Title);

            loadTaskAssignmentWatchListDialog(rowId, transactionMode);
            ajaxDialog.hidePleaseWait();
            $(elementIDs.taskAssignmentWatchlist).dialog("open");
        };

        function taskAssigWatchComments(Taskid, transactionMode, gridName) {
            var isAttachmentsOnly = false;
            if (gridName != undefined)
                isAttachmentsOnly = true;
            else
                gridName = elementIDs.taskAssigWatchCommentsJQ;

            //set column list for grid
            var colArray = ['Task Number', 'Comment', 'Status', 'Added Date', 'Added By', 'IsNew', 'Attachment', 'Folder Version', 'FolderVersionID', 'filename'];

            //set column models
            var colModel = [];
            colModel.push({ key: true, name: 'TaskID', index: 'TaskID', width: '60px', editable: false, hidden: true });
            colModel.push({ name: 'Comment', index: 'Comment', editable: false, width: '170px' });
            colModel.push({ name: 'Status', index: 'Status', editable: false, width: '80px' });
            colModel.push({ name: 'Datetimestamp', index: 'Datetimestamp', editable: false, formatter: 'date', width: '100px', formatoptions: JQGridSettings.DateTimeFormatterOptions });
            colModel.push({ name: 'AddedBy', index: 'AddedBy', editable: false, width: '80px' });
            colModel.push({ name: 'IsNew', index: 'IsNew', editable: false, width: '80px', hidden: true });
            colModel.push({ name: 'Attachment', index: 'Attachment', editable: false, align: 'center', formatter: AttachmentWatchFormatter });
            colModel.push({ name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: false, align: 'center' });
            colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', editable: false, align: 'center', hidden: true });
            colModel.push({ name: 'filename', index: 'filename', editable: false, align: 'center', hidden: true });

            ////clean up the grid first - only table element remains after this
            $(gridName).jqGrid('GridUnload');

            ////adding the pager element            
            $(gridName).parent().append("<div id='p" + elementIDs.taskAssigWatchComments + "'></div>");

            var taskId = Taskid;
            if (Taskid == undefined) {
                taskId = 0;
            }
            var url = "/DashBoard/GetCommentList?Taskid={Taskid}&isAttachmentsOnly={isAttachmentsOnly}".replace('{Taskid}', taskId).replace('{isAttachmentsOnly}', isAttachmentsOnly);

            $(gridName).jqGrid({
                url: url,
                datatype: 'json',
                cache: false,
                colNames: colArray,
                colModel: colModel,
                //  caption: 'Task Comments',
                height: 'auto',
                rowNum: 10,
                ignoreCase: true,
                loadonce: true,
                viewrecords: true,
                hidegrid: true,
                altRows: true,
                sortname: 'MappingRowID',
                sortorder: 'desc',
                pager: '#p' + elementIDs.taskAssigWatchComments,
                altclass: 'alternate',
                rowList: [10, 20, 30],
                loadComplete: function () {
                    $(".WFSTATEACCESS").unbind('click');
                    $(".WFSTATEACCESS").click(function () {
                        var cellvalue = $(this).attr("id");
                        var fileName = $(this)[0].parentElement.innerText;
                        DownLoadDocument(cellvalue, fileName);
                    });
                    $(elementIDs.taskCommentsDailog).dialog({
                    position: {
                    my: 'center',
                    at: 'center'
                    },
                });

                }
            });
            var pagerElement = '#p' + elementIDs.taskAssigWatchComments;
            $(gridName).jqGrid('navGrid', pagerElement, { edit: false, view: false, add: false, del: false, search: false });

            ////remove paging
            //$(pagerElement).find(pagerElement + '_center').remove();
            //$(pagerElement).find(pagerElement + '_right').remove();
            if (transactionMode != "View") {
                $(gridName).jqGrid('navButtonAdd', pagerElement,
                 {
                     caption: '',
                     buttonicon: 'ui-icon-plus',
                     title: 'New Task comment',
                     id: 'btnNewWatchcomment',
                     disabled: 'disabled',
                     onClickButton: function () {
                         $(elementIDs.newWatchcommentDialog).dialog('option', 'title', "Comment");
                         $(elementIDs.uploadWatchCommentAttachment).val('');
                         $(elementIDs.newWatchcommentJQ).val('');
                         $(elementIDs.newWatchcommentDialog).dialog("open");
                     }
                 });
            }
            $(elementIDs.btnsaveWatchcommentJQ).off('click').bind("click", (function () {
                var comment = $(elementIDs.newWatchcommentJQ).val();
                var performOperation = true;
                if (comment == "" || comment == null) {
                    $(elementIDs.newWatchcommentJQ).parent().addClass('has-error');
                    $(elementIDs.newWatchcommentJQ).addClass('form-control');
                    $(elementIDs.newWatchcommentSpan).text("Please enter comment");
                    performOperation = false;
                }
                else {
                    $(elementIDs.newWatchcommentJQ).parent().removeClass('has-error');
                    $(elementIDs.newWatchcommentJQ).removeClass('form-control');
                    $(elementIDs.newWatchcommentSpan).text('');
                }
                if (performOperation) {
                    var filename = $(elementIDs.uploadWatchCommentAttachment).val();
                    var fileNameUpload = $(elementIDs.uploadWatchCommentAttachment).get(0);
                    var fileNamefiles = fileNameUpload.files;
                    var fileData = new FormData();
                    // Looping over all files and add it to FormData object  
                    for (var i = 0; i < fileNamefiles.length; i++) {
                        fileData.append(fileNamefiles[i].name, fileNamefiles[i]);
                    }
                    $.ajax({
                        url: '/DashBoard/uploadExcel',
                        type: "POST",
                        contentType: false, // Not to set any content header  
                        processData: false, // Not to process data  
                        data: fileData,
                        success: function (result) {
                            var attachments;
                            if (result != "") {
                                attachments = result;
                            }

                            var comment = $(elementIDs.newWatchcommentJQ).val();
                            var status = $(elementIDs.taskAssigWatchTaskStatus + ' option:selected').text();
                            var folderVersionID = $(elementIDs.taskAssigWatchVersionList).val();
                            var folderVersion = $(elementIDs.taskAssigWatchVersionList).find('option:selected').text();
                            var currentdate = new Date();
                            var file = fileNamefiles.length > 0 ? fileNamefiles[0].name : "";
                            
                            var data = [{ TaskID: "", Datetimestamp: currentdate, Comment: comment, Status: status, Attachment: attachments, filename: file, IsNew: 1, FolderVersionID: folderVersionID, FolderVersionNumber: folderVersion, AddedBy:"" }];
                            var rowCount = $(elementIDs.taskAssigWatchCommentsJQ).jqGrid('getGridParam', 'records');
                            $(elementIDs.taskAssigWatchCommentsJQ).jqGrid('addRowData', rowCount + 1, data[0], "first");
                            $(elementIDs.uploadWatchCommentAttachment).val('');
                            $(elementIDs.newWatchcommentDialog).dialog("close");
                        },
                        error: function (err) {
                            messageDialog.show(err.statusText);
                        }
                    });
                }
            }));
        }

        function AttachmentWatchFormatter(cellvalue, options, rowObject) {
            if (cellvalue == undefined || cellvalue == "" || rowObject.filename == null) {
                return '';
            }
            var link = "<a href='#' class='WFSTATEACCESS' id='" + cellvalue + "'>" + rowObject.filename + "<span align='center' class='ui-icon ui-icon-arrowreturnthick-1-s' style='margin:auto' title='Download Attachment' ></span></a>";
            return link;
        }

        function DownLoadDocument(cellvalue, fileName) {
            $.download('/DashBoard/DownloadDocument?Attachment=' + cellvalue + '&file=' + fileName, "test", 'post');
        }
        //contains functionality for viewing assigned users dialog
        var viewAssignedUserDialog = function () {
            var elementIDs = {
                //for dialog for User assignment update            
                viewAssignedUserDialog: "#viewAssignedUserDialog",
                watchGridJQ: "#watch"
            };

            function init() {
                $(elementIDs.viewAssignedUserDialog).dialog({
                    autoOpen: false,
                    resizable: false,
                    closeOnEscape: true,
                    height: 'auto',
                    width: '750px',
                    modal: true,
                    position: 'relative',
                    close: function () {
                        if (isAddedUserAssigned) {
                            $(elementIDs.watchGridJQ).trigger('reloadGrid');
                            isloaded = false;
                            ischeckViewInserted = $('#chkviewinterested').prop('checked');
                            ischeckViewAll = $('#chkviewall').prop('checked');
                        }
                        isAddedUserAssigned = false;
                    }
                });
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
                show: function (folderVersionID) {
                    $(elementIDs.viewAssignedUserDialog).dialog('option', 'title', 'User Assignment');
                    loadUserAssignmentGrid(folderVersionID);
                    $(elementIDs.viewAssignedUserDialog).dialog("open");
                }
            }
        }();

        //contains functionality to add/Delete user assignment for folder version
        var UserAssignUnassignDialog = function () {
            var elementIDs = {
                userAssignUnassignDialog: "#UserAssignUnassignDialog",
                watchGridJQ: "#watch"
            };

            //maintains dialog state - add or delete
            var userAssignmentDialogState;

            function init() {
                $(elementIDs.userAssignUnassignDialog).dialog({
                    autoOpen: false,
                    resizable: false,
                    closeOnEscape: true,
                    height: 'auto',
                    width: 770,
                    modal: true,
                    close: function () {
                        var folderID = $(elementIDs.watchGridJQ).getGridParam("selrow");
                        folderData = $(elementIDs.watchGridJQ).getRowData(folderID);
                        loadUserAssignmentGrid(folderData.FolderVersionId);
                    }
                });

            }

            function showError(xhr) {
                if (xhr.status == 999)
                    this.location = '/Account/LogOn';
                else
                    messageDialog.show(JSON.stringify(xhr));
            }

            //initialize the dialog after this js are loaded
            init();

            return {
                show: function (action) {

                    userAssignmentDialogState = action;

                    var folderID = $(elementIDs.watchGridJQ).getGridParam("selrow");
                    var folderData = $(elementIDs.watchGridJQ).getRowData(folderID);

                    loadUserAssignmentGrid(folderData.FolderVersionId, userAssignmentDialogState);

                    $(elementIDs.userAssignUnassignDialog).dialog("open");
                }
            }
        }();

        //initialization of the DashBoard Grid when the Watch List function is loaded in browser and invoked
        init();

        return {
            loadWatch: function () {
                loadWatchGrid();
            },
            loadUserAssignment: function (folderVersionID, userAssignmentDialogState) {
                loadUserAssignmentGrid(folderVersionID, userAssignmentDialogState);
            }
        }
    }
    function workQueue() {
        var isEdit = false;
        var taskFolderVersionId = WorkFlowState.TaskFolderVersionId;
        var URLs = {
            workQueueList: '/DashBoard/GetWorkQueueListNotReleasedAndApproved?viewMode={viewMode}&taskFolderVersionId={taskFolderVersionId}',
            folderVersionList: '/FolderVersion/Index?tenantId={tenantId}&folderVersionId={folderVersionId}&folderId={folderId}&foldeViewMode={foldeViewMode}&mode={mode}',
            LockStatusUrl: '/FolderVersion/GetFolderLockStatus?tenantId={tenantId}&folderId={folderId}',
            OverrideLockUrl: '/FolderVersion/OverrideFolderLock?tenantId={tenantId}&folderId={folderId}',
            exportToExcel: '/DashBoard/ExportWorkQueueListToExcel',
            SubworkQueueList: '/DashBoard/GetSubworkQueueList?',
            commentHistory: '/DashBoard/GetCommentList?Taskid={Taskid}',

        };

        var elementIDs = {
            //table element for the Work Queue Grid 
            workQueueGrid: 'workQueue',
            //with hash for use with jQuery
            workQueueGridJQ: '#workQueue',
            btnUnlockJQ: '#btnUnlock',
            fldrViewDialog: '#folderViewDialog',
            btnClassicFolderView: '#btnClassicFolderView1',
            btnSOTFolderView: '#btnSOTView1',
            taskAssignmentWorkQueue: "#taskAssignmentWorkQueue",
            taskAssigWorkQueueFolderList: "#taskAssigWorkQueueFolderList",
            taskAssigWorkQueueVersionList: "#taskAssigWorkQueueVersionList",
            taskAssigWorkQueueWorkFlowStatusList: "#taskAssigWorkQueueWorkFlowStatusList",
            taskAssigWorkQueuePlansList: "#taskAssigWorkQueuePlansList",
            taskAssigWorkQueueTasksList: "#taskAssigWorkQueueTasksList",
            taskAssigWorkQueueAssignUserList: "#taskAssigWorkQueueAssignUserList",
            taskAssigWorkQueueTaskStatus: "#taskAssigWorkQueueTaskStatus",
            taskAssigWorkQueuehCommentsJQ: "#taskAssigWorkQueuehComments",
            taskAssigWorkQueueStartdate: "#taskAssigWorkQueueStartdate",
            taskAssigWorkQueueDuedate: "#taskAssigWorkQueueDuedate",
            taskAssigWorkQueueCancelBtn: "#taskAssigWorkQueueCancelBtn",
            taskAssigWorkQueueSaveBtn: "#taskAssigWorkQueueSaveBtn",
            HiddenParameter: "#HiddenParameter",
            MappingRowIDForTaskHDValQ: "#MappingRowIDForTaskHDValQ",
            newcommentDialog: "#newcommentDialog",
            reportUploadMappedDoc: '#reportUploadMappedDoc',
            fileInputElement: '#uploadAttachmentFile',
            fileInputLabelElement: '#uploadWorkQueueAttachmentLabel',
            fileInputChangeElement: '#uploadWorkQueueAttachment',
            Documentname: '#Documentname',
            btnsavecommentJQ: "#btnsavecomment",
            newcommentJQ: "#newcomment",
            taskAssigWorkQueueViewList: "#taskAssigWorkQueueViewList",
            taskAssigWorkQueueSectionList: "#taskAssigWorkQueueSectionList",
            taskAssigWorkQueueOrder: "#taskAssigWorkQueueOrder",
            taskAssigWorkQueueDuration:"#taskAssigWorkQueueDuration",
            taskAssigWorkQueueAccountListLabelDiv: "#taskAssigWorkQueueAccountListLabelDiv",
            taskAssigWorkQueueAccountListDiv: "#taskAssigWorkQueueAccountListDiv",
            taskAssigWorkQueueEstTime: "#taskAssigWorkQueueEstTime",
            taskAssigWorkQueueActualTime: "#taskAssigWorkQueueActualTime",
            uploadWorkCommentAttachment: "#uploadWorkCommentAttachment",
            taskCommentsDailog: "#taskCommentsDailog",
            taskCommentsGrid: "#taskCommentsGrid",
            newcommentSpan: "#newcommentSpan",
            taskAssigWorkQueueAccountList: "#taskAssigWorkQueueAccountList",
      
        };

        var MappingRowIDForTask;
        var ALLPlans = "All Plans";
        var ALLViews = "All Views";
        var ALLSections = "All Sections";
        var IsPlanTaskQueueInfoUpdated = false;
        var IsSaveWorkQueueProcessDone = false;
        var subGridTableIdJQ;
        if (typeof vbIsFolderLockEnable === "undefined") { this.isFolderLockEnable = false } else {
            this.isFolderLockEnable = vbIsFolderLockEnable
        };

        function showError(xhr) {
            if (xhr.status == 999)
                this.location = '/Account/LogOn';
            else
                messageDialog.show(JSON.stringify(xhr));
        }
        function fillQueueTaskPrioityList(value) {
            $(elementIDs.taskAssigWorkQueueOrder).empty();
            var url = '/PlanTaskUserMapping/GetWatchTaskPriorityList';
            var promise = ajaxWrapper.getJSON(url);
            promise.done(function (list) {
                $(elementIDs.taskAssigWorkQueueOrder).empty();
                //   $(elementIDs.taskAssigWatchOrder).append("<option value='0'>" + "Select One" + "</option>");
                for (i = 0; i < list.length; i++) {
                    $(elementIDs.taskAssigWorkQueueOrder).append("<option value=" + list[i].ID + ">" + list[i].Description + "</option>");
                }
                //$(elementIDs.taskAssigWatchTaskStatus).val("0").autoDropdown("refresh");
                $(elementIDs.taskAssigWorkQueueOrder).val(value);
            });
            promise.fail(showError);
        }
        function init() {
            // Select Folder View Dialog
            $(elementIDs.fldrViewDialog).dialog({
                autoOpen: false,
                height: 120,
                width: 400,
                modal: true,
            });

            $(elementIDs.taskAssignmentWorkQueue).dialog({
                autoOpen: false,
                height: 500,
                width: 850,
                modal: true,
            });
            $(elementIDs.newcommentDialog).dialog({
                autoOpen: false,
                height: 500,
                width: 450,
                modal: true,
            });
            $(elementIDs.taskCommentsDailog).dialog({
                autoOpen: false,
                height: 450,
                width: 700,
                modal: true,
            });
            $(elementIDs.taskAssigWorkQueueStartdate).datepicker({
                dateFormat: 'mm/dd/yy',
                changeMonth: true,
                changeYear: true,
                yearRange: 'c-61:c+20',
                showOn: "both",
                //CalenderIcon path declare in golbalvariable.js
                buttonImage: Icons.CalenderIcon,
                buttonImageOnly: true,
            }).parent().find('img').css('margin-top', '-25px').css('margin-right', '5px');

            $(elementIDs.taskAssigWorkQueueDuedate).datepicker({
                dateFormat: 'mm/dd/yy',
                changeMonth: true,
                changeYear: true,
                yearRange: 'c-61:c+20',
                showOn: "both",
                //CalenderIcon path declare in golbalvariable.js
                buttonImage: Icons.CalenderIcon,
                buttonImageOnly: true,
            }).parent().find('img').css('margin-top', '-25px').css('margin-right', '5px');


            $(elementIDs.taskAssigWorkQueueCancelBtn).click(function () {
                taskAssignmentWorkQueuelistDialogClose();
            });
            taskAssignmentWorkQueuelistDialogClose = function () {
                //yesNoConfirmDialog.show(Common.closeConfirmationMsg, function (e) {
                //    if (e) {
                //        yesNoConfirmDialog.hide();
                //        $(elementIDs.taskAssignmentWorkQueue).dialog("close");
                //    }
                //    else {
                //        yesNoConfirmDialog.hide();
                //    }
                //});

            }

            $(elementIDs.btnClassicFolderView).off('click').on('click', function () {
                if (isEdit == true) {
                    var rowId = $(elementIDs.workQueueGridJQ).getGridParam('selrow');
                    LoadWorkQueueFolderVersion(rowId, FolderViewMode.DefaultView);
                }
                else if (isEdit == false) {
                    ViewWorkQueueFolderVersion(rowId, FolderViewMode.DefaultView);
                }
                $(elementIDs.fldrViewDialog).dialog("close");
            });

            $(elementIDs.btnSOTFolderView).off('click').on('click', function () {
                if (isEdit == true) {
                    var rowId = $(elementIDs.workQueueGridJQ).getGridParam('selrow');
                    LoadWorkQueueFolderVersion(rowId, FolderViewMode.SOTView);
                }
                else if (isEdit == false) {
                    ViewWorkQueueFolderVersion(rowId, FolderViewMode.SOTView);
                }
                $(elementIDs.fldrViewDialog).dialog("close");
            });

            $(document).ready(function () {
                loadWorkQueueGrid();
            });

            $('div' + elementIDs.taskAssignmentWorkQueue).on('dialogclose', function (event) {
                //if (IsPlanTaskQueueInfoUpdated == true) {
                //    yesNoConfirmDialog.show(DashBoard.confirmTaskAssignmentSaveMsg, function (e) {
                //        if (e) {
                //            yesNoConfirmDialog.hide();
                //            $(elementIDs.taskAssignmentWorkQueue).dialog("open");
                //            SaveTaskAssignWorkQuereOperation();
                //        }
                //        else {
                //            yesNoConfirmDialog.hide();
                //        }
                //    });
                //}
            });

            $(elementIDs.taskAssignmentWorkQueue + ' button').on('click', function () {
                if (this.id == "taskAssigWorkQueueSaveBtn") {
                    if (selectedTaskstate != $(elementIDs.taskAssigWorkQueueTaskStatus + ' option:selected').text()) {
                        if (selectedTaskFolderversionWorkFlowState == $(elementIDs.taskAssigWorkQueueWorkFlowStatusList + ' option:selected').text()) {
                            SaveTaskAssignWorkQuereOperation();
                        } else {
                            messageDialog.show("Folder version workflow state does not match with task workflow state, so you can not update the task status.");
                        }

                    } else {
                        SaveTaskAssignWorkQuereOperation();
                    }
                }
                else {
                    $(elementIDs.taskAssignmentWorkQueue).dialog("close");
                }
            });

            $('input[type=radio][name=optradioWorkQueueFolderType]').change(function () {
                if (this.id == 'optradioWorkQueueAccount') {
                    $(elementIDs.taskAssigWorkQueueAccountListLabelDiv).css('display', 'block');
                    $(elementIDs.taskAssigWorkQueueAccountListDiv).css('display', 'block');
                    $("#optradioWorkQueueAccount").prop('checked', 'checked');
                }
                else {
                    $(elementIDs.taskAssigWorkQueueAccountListLabelDiv).css('display', 'none');
                    $(elementIDs.taskAssigWorkQueueAccountListDiv).css('display', 'none');
                    $("#optradioWorkQueuePortfolio").prop('checked', 'checked');
                }
            });
        }
        $(elementIDs.taskAssigWorkQueueTaskStatus).change(function () {
            IsPlanTaskQueueInfoUpdated = true;
        });
        $(elementIDs.taskAssigWorkQueuehComments).change(function () {
            IsPlanTaskQueueInfoUpdated = true;
        });
        //function LoadFolderViewDialog() {
        //    $(elementIDs.fldrViewDialog).dialog({
        //        autoOpen: false,
        //        height: 120,
        //        width: 400,
        //        modal: true,
        //    });
        //    $(elementIDs.fldrViewDialog).dialog('option', 'title', "Select Folder View");
        //    $(elementIDs.fldrViewDialog).dialog("open");
        //}

        function LoadWorkQueueFolderVersion(rowId, viewMode) {
            var rowId = $(elementIDs.workQueueGridJQ).getGridParam('selrow');
            if (rowId !== undefined && rowId !== null) {
                var row = $(elementIDs.workQueueGridJQ).getRowData(rowId);
                if (isFolderLockEnable == false)
                {
                    var folderVersionListUrl = URLs.folderVersionList.replace(/{tenantId}/g, row.TenantID).replace(/\{folderVersionId\}/g, row.FolderVersionId).replace(/\{folderId\}/g, row.FolderId).replace(/{foldeViewMode}/g, viewMode);
                    folderVersionListUrl = folderVersionListUrl.replace(/\{mode\}/g, true);//since edit button is clicked mode needs to be true
                    window.location.href = folderVersionListUrl + "&planTaskId=" + row.MappingRowID;
                    return;
                }

                var promise = ajaxWrapper.getJSON(URLs.LockStatusUrl.replace(/{tenantId}/g, row.TenantID).replace(/\{folderId\}/g, row.FolderId));
                promise.done(function (result) {
                    if (result != "") {
                        var userWarning = FolderLock.userWarning.replace("{0}", result);
                //        checkUnlockFolderClaims(elementIDs, claims);
                        folderLockWarningDialog.show(userWarning, function () {
                //            //unlock Folder Version
                //            var promise1 = ajaxWrapper.getJSON(URLs.OverrideLockUrl.replace(/{tenantId}/g, row.TenantID).replace(/\{folderId\}/g, row.FolderId));
                //            promise1.done(function (xhr) {
                //                if (xhr.Result === ServiceResult.SUCCESS) {
                //                    var folderVersionListUrl = URLs.folderVersionList.replace(/{tenantId}/g, row.TenantID).replace(/\{folderVersionId\}/g, row.FolderVersionId).replace(/\{folderId\}/g, row.FolderId).replace(/{foldeViewMode}/g, viewMode);
                //                    folderVersionListUrl = folderVersionListUrl.replace(/\{mode\}/g, true);//since edit button is clicked mode needs to be true
                //                    window.location.href = folderVersionListUrl;
                //                }
                //                else if (xhr.Result === ServiceResult.FAILURE) {
                //                    messageDialog.show(xhr.Items[0].Messages);
                //                }
                //                else {
                //                    messageDialog.show(Common.errorMsg);
                //                }
                //            });
                        }, function () {
                            //View Folder Version
                            var folderVersionListUrl = URLs.folderVersionList.replace(/{tenantId}/g, row.TenantID).replace(/\{folderVersionId\}/g, row.FolderVersionId).replace(/\{folderId\}/g, row.FolderId).replace(/{foldeViewMode}/g, viewMode);
                            folderVersionListUrl = folderVersionListUrl.replace(/\{mode\}/g, false);//since edit button is clicked mode needs to be true
                            window.location.href = folderVersionListUrl + "&planTaskId=" + row.MappingRowID;
                        });
                        return;
                    }
                    else {
                var folderVersionListUrl = URLs.folderVersionList.replace(/{tenantId}/g, row.TenantID).replace(/\{folderVersionId\}/g, row.FolderVersionId).replace(/\{folderId\}/g, row.FolderId).replace(/{foldeViewMode}/g, viewMode);
                folderVersionListUrl = folderVersionListUrl.replace(/\{mode\}/g, true);//since edit button is clicked mode needs to be true
                window.location.href = folderVersionListUrl + "&planTaskId=" + row.MappingRowID;
                //window.location.href = folderVersionListUrl + "&formInstanceId=" + row.FormInstanceId;;
                    }
                });
               promise.fail(showError);
            } else {
                $('#messagedialog').dialog({
                    title: 'Work Queue',
                    height: 120
                });
                messageDialog.show(Common.selectRowMsg);
            }
        }

        function ViewWorkQueueFolderVersion(rowId, viewMode) {
            var rowId = $(elementIDs.workQueueGridJQ).getGridParam('selrow');
            if (rowId !== undefined && rowId !== null) {
                var row = $(elementIDs.workQueueGridJQ).getRowData(rowId);
                //forward to FolderVersion Page to view the page.                      
                var folderVersionListUrl = URLs.folderVersionList.replace(/{tenantId}/g, row.TenantID).replace(/\{folderVersionId\}/g, row.FolderVersionId).replace(/\{folderId\}/g, row.FolderId).replace(/{foldeViewMode}/g, viewMode);
                folderVersionListUrl = folderVersionListUrl.replace(/\{mode\}/g, false); //since view button is clicked mode needs to be false
                window.location.href = folderVersionListUrl + "&planTaskId=" + row.MappingRowID;
            } else {
                $('#messagedialog').dialog({
                    title: 'Work Queue',
                    height: 120
                });
                messageDialog.show(DashBoard.selectRowToViewFolderMsg);
            }
        }

        function loadWorkQueueGrid() {
            //set column list for grid

            var colArray = ['Task #', 'Account', 'TenantID', 'FolderId', 'FolderVersionId', 'TaskId', 'Folder', 'Folder Version', 'Effective Date', 'Workflow', 'Task', 'Plan', 'View', 'Section', 'Assignment', 'Status', 'Start Date', 'Due Date', 'Priority', 'Completed Date', 'Estimated Time', 'Actual Time', 'Attachments', 'FolderVersionWFStateID', 'TaskWFStateID'];

            //set column models
            var colModel = [];
            colModel.push({ name: 'MappingRowID', index: 'MappingRowID', editable: false, align: 'left', width: 75 });
            colModel.push({ name: 'Account', index: 'Account', hidden: true, editable: false, align: 'left' });
            colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true, search: false });
            colModel.push({ name: 'FolderId', index: 'FolderId', hidden: true, search: false });
            colModel.push({ name: 'FolderVersionId', index: 'FolderVersionId', hidden: true, search: false });
            colModel.push({ name: 'TaskId', index: 'TaskId', hidden: true, search: false });
            colModel.push({ name: 'Folder', index: 'Folder', editable: false, align: 'left' });
            colModel.push({ name: 'FolderVersion', index: 'FolderVersion', editable: false, hidden: true, align: 'right' });
            colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, align: 'center', formatter: 'date', sorttype: "date", searchoptions: { sopt: ["eq"] }, formatoptions: JQGridSettings.DateFormatterOptions });
            colModel.push({ name: 'Workflow', index: 'Workflow', editable: false, align: 'left' });
            colModel.push({ name: 'Task', index: 'Task', editable: false, align: 'left', width: 250 });
            colModel.push({ name: 'Plan', index: 'Plan', editable: false, align: 'left' });
            colModel.push({ name: 'View', index: 'View', editable: false, align: 'left' });
            colModel.push({ name: 'Section', index: 'Section', hidden: true, editable: false, align: 'left' });
            colModel.push({ name: 'Assignment', index: 'Assignment', editable: false, align: 'left', hidden: true });
            colModel.push({ name: 'Status', index: 'Status', editable: false, align: 'left', hidden: false });
            colModel.push({ name: 'StartDate', index: 'StartDate', editable: false, align: 'left', hidden: false, formatter: 'date', sorttype: "date", searchoptions: { sopt: ["eq"] }, formatoptions: JQGridSettings.DateFormatterOptions });
            colModel.push({ name: 'DueDate', index: 'DueDate', editable: false, align: 'left', hidden: false, formatter: 'date', sorttype: "date", searchoptions: { sopt: ["eq"] }, formatoptions: JQGridSettings.DateFormatterOptions });
            colModel.push({ name: 'Priority', index: 'Priority', editable: false, align: 'left' });
            colModel.push({ name: 'Completed', index: 'Completed', hidden: (taskGridViewMode == 'Open' ? true : false), editable: false, align: 'center', formatter: 'date', sorttype: "date", searchoptions: { sopt: ["eq"] }, formatoptions: JQGridSettings.DateFormatterOptions });
            colModel.push({ name: 'EstimatedTime', index: 'EstimatedTime', editable: false, align: 'left', hidden: true });
            colModel.push({ name: 'ActualTime', index: 'ActualTime', editable: false, align: 'left', hidden: true });
            colModel.push({ name: 'Comments', index: 'Comments', editable: true, align: 'center', formatter: commentsGridFormatterWQ });
            colModel.push({ name: 'FolderVersionWFStateID', index: 'FolderVersionWFStateID', editable: false, align: 'left', hidden: true });
            colModel.push({ name: 'TaskWFStateID', index: 'TaskWFStateID', editable: false, align: 'left', hidden: true });

            //clean up the grid first - only table element remains after this
            $(elementIDs.workQueueGridJQ).jqGrid('GridUnload');

            //adding the pager element
            $(elementIDs.workQueueGridJQ).parent().append("<div id='p" + elementIDs.workQueueGrid + "'></div>");

            var url = URLs.workQueueList.replace('{viewMode}', taskGridViewMode);
            url = url.replace('{taskFolderVersionId}', taskFolderVersionId);
            $(elementIDs.workQueueGridJQ).jqGrid({
                url: url,
                datatype: 'json',
                cache: false,
                colNames: colArray,
                colModel: colModel,
                caption: 'Work Queue',
                height: '220',
                //width:'100',
                rowNum: 10,
                autowidth: true,
                ignoreCase: true,
                loadonce: false,
                viewrecords: true,
                hidegrid: false,
                // altRows: true,
                sortname: 'MappingRowID',
                sortorder: 'desc',
                pager: '#p' + elementIDs.workQueueGrid,
                altclass: 'alternate',
                rowList: [10, 20, 30],
                onPaging: function (pgButton) {
                    if (pgButton === "user" && !IsEnteredPageExist(elementIDs.workQueueGrid)) {
                        return "stop";
                    }
                },
                gridComplete: function () {
                    //to check for claims.             
                    var objMap = {
                        edit: "#btnWorkQueueEdit",
                        view: "#btnWorkQueueView"
                    };
                    checkApplicationClaims(claims, objMap, URLs.workQueueList);
                    if (taskGridViewMode == 'Completed') {
                        $('#chkCompletedTasks').prop('checked', 'checked');
                    } else {
                        $('#chkOpenTasks').prop('checked', 'checked');
                    }
                    //    $(elementIDs.workQueueGridJQ).jqGrid('setGridWidth', '1200');
                    $(".WQCOMMENTS").unbind('click');
                    $(".WQCOMMENTS").click(function () {
                        var mappingTaskID = $(this).attr("id");
                        loadCommentsDailog(mappingTaskID);
                    });
                    //var gridData = $(elementIDs.workQueueGridJQ).jqGrid('getGridParam', 'data');
                    //$(gridData).each(function (index, value) {
                    //    if (value.FolderVersionWFStateID == value.TaskWFStateID) {
                    //        var trElement = $("#" + value._id_, elementIDs.workQueueGridJQ);
                    //        trElement.find("td:first").css("color", "#57BD99");
                    //    }
                    //});
                    var rowIds = $(elementIDs.workQueueGridJQ).getDataIDs();
                    for (var i = 0 ; i <= rowIds.length; i++) {
                        var rowData = $(elementIDs.workQueueGridJQ).getRowData(rowIds[i]);
                        if (rowData.FolderVersionWFStateID == rowData.TaskWFStateID) {
                            var trElement = $("#" + rowIds[i], elementIDs.workQueueGridJQ);
                            trElement.find("td:first").css("color", "#57BD99");
                        }
                    }
                },
                onSelectRow: function (rowId) {
                    //to check if user has permissions to disable the Release Folder.(eg. Viewer)                                 
                    var editFlag = checkUserPermissionForEditable(URLs.workQueueList);
                    if (rowId !== undefined && rowId !== null) {
                        var row = $(this).getRowData(rowId);
                        //if (editFlag) {
                        //if (vbRole != undefined) {

                        //if ((vbRole != Role.Audit && row.Status == "Test/Audit") && (vbRole != Role.Superuser && row.Status == "Test/Audit")
                        //    && (vbRole != Role.ProductAudit && row.Status == "Test/Audit")) {
                        //    $("#btnWorkQueueEdit").addClass('ui-state-disabled');
                        //}
                        //else if (row.Status == 'Facets Prod' && row.ApprovalStatusID !== null && row.ApprovalStatusID == 1) {
                        //    $("#btnWorkQueueEdit").addClass('ui-state-disabled');
                        //}
                        //else {
                        $("#btnWorkQueueEdit").removeClass('ui-state-disabled');
                        $("#btnWorkQueueView").removeClass('ui-state-disabled');
                        //}
                        //  }
                        //}
                    }
                },
                resizeStop: function (width, index) {
                    autoResizing(elementIDs.workQueueGridJQ);
                },
                jsonReader: {
                    page: function (obj) { return obj.records == 0 || obj.records == undefined ? "0" : obj.page; },
                }
            });
            var pagerElement = '#p' + elementIDs.workQueueGrid;
            // $('#p' + elementIDs.workQueueGrid).find('input').css('height', '20px');

            $(elementIDs.workQueueGridJQ).jqGrid('filterToolbar', {
                stringResult: true, searchOnEnter: true,
                defaultSearch: "cn",
            });
            //remove default buttons
            $(elementIDs.workQueueGridJQ).jqGrid('navGrid', pagerElement, { edit: false, view: false, add: false, del: false, search: false });

            //Add button in footer of grid that pops up the Edit Task Assignment dialog
            $(elementIDs.workQueueGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '',
                buttonicon: 'ui-icon-pencil',
                title: 'Update Task',
                id: 'btnUpdateTask',
                disabled: 'disabled',
                onClickButton: function () {
                    openTaskAssignmentWorkQueueDialog(true);
                }
            });

            if (taskGridViewMode == 'Completed') {
                $("#btnUpdateTask").addClass("ui-state-disabled");
            } else {
                if (vbRole == 23) {
                    $("#btnUpdateTask").addClass("ui-state-disabled");
                }
                else {
                    $("#btnUpdateTask").removeClass("ui-state-disabled");
                }
            }
            $(elementIDs.workQueueGridJQ).jqGrid('navButtonAdd', pagerElement,
               {
                   caption: '',
                   buttonicon: 'ui-icon-document-b',
                   title: 'View Task',
                   id: 'btnViewTask',
                   onClickButton: function () {
                       openTaskAssignmentWorkQueueDialog(false);
                   }
               });

            //add button in footer of grid that pops up the edit form design dialog
            $(elementIDs.workQueueGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon-script',
                title: 'Edit Folder',
                id: 'btnWorkQueueEdit',
                onClickButton: function () {
                    var rowId = $(elementIDs.workQueueGridJQ).getGridParam('selrow');
                    LoadWorkQueueFolderVersion(rowId, FolderViewMode.DefaultView);
                }
            });

            //View button in footer of grid 
            $(elementIDs.workQueueGridJQ).jqGrid('navButtonAdd', pagerElement,
               {
                   caption: '', buttonicon: 'ui-icon-copy', title: 'View Folder', id: 'btnWorkQueueView',
                   onClickButton: function () {
                       var rowId = $(elementIDs.workQueueGridJQ).getGridParam('selrow');
                       ViewWorkQueueFolderVersion(rowId, FolderViewMode.DefaultView);
                   }
               });

            //Download Excel file
            $(elementIDs.workQueueGridJQ).jqGrid('navButtonAdd', pagerElement,
              {
                  caption: '', buttonicon: 'ui-icon-arrowstop-1-s', title: 'Download Work Queue List', id: 'btnWorkQueueListExportToExcel',

                  onClickButton: function () {
                      var currentInstance = this;
                      var jqGridtoCsv = new JQGridtoCsv(elementIDs.workQueueGridJQ, false, currentInstance);
                      jqGridtoCsv.buildExportOptions();
                      var stringData = "csv=" + jqGridtoCsv.csvData;
                      stringData += "<&isGroupHeader=" + jqGridtoCsv.isGroupHeader;
                      stringData += "<&noOfColInGroup=" + jqGridtoCsv.noofGroupedColumns;
                      stringData += "<&repeaterName=" + elementIDs.userListGrid;

                      //$.download(URLs.exportToExcel, stringData, 'post');

                      var strURLNew = '/DashBoard/ExportWorkQueueListToExcelTable?viewMode={viewMode}';
                      $.download(strURLNew.replace('{viewMode}', taskGridViewMode), "test", 'post');
                  }
              });
            // add filter toolbar to the top
            $(elementIDs.formDesignGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
        }

        function commentsGridFormatterWQ(cellvalue, options, rowObject) {
            if (cellvalue == undefined || cellvalue.trim() == "" || cellvalue == null) {
                return '';
            }
            var link = "<a href='#' class='WQCOMMENTS' id='" + rowObject.MappingRowID + "'><span align='center' class='ui-icon-extlink' style='margin:auto' title='View Attachments' ></span></a>";
            return link;
        }

        //initialization of the DashBoard Grid when the YourWorkQueue function is loaded in browser and invoked
        init();
        function loadCommentsDailog(mappingTaskID) {

            $(elementIDs.taskCommentsDailog).dialog({
                autoOpen: false,
                height: 500,
                width: 850,
                modal: true,
            });
            $(elementIDs.taskCommentsDailog).dialog('option', 'title', 'View Task Attachments');
            taskAssigWorkQueuehComments(mappingTaskID, "View", elementIDs.taskCommentsGrid);
            $(elementIDs.taskCommentsDailog).dialog("open");

        };


        //contains functionality to related to Task Assignment
        function openTaskAssignmentWorkQueueDialog(isUpdate) {
            var rowId = $(elementIDs.workQueueGridJQ).getGridParam('selrow');
            if (rowId == undefined && rowId == null) {
                $('#messagedialog').dialog({
                    title: 'Work Queue',
                    height: 120
                });
                messageDialog.show(isUpdate ? DashBoard.selectRowToShowUpdateTaskMsg : DashBoard.selectRowToShowViewTaskMsg);
            } else {
                $(elementIDs.taskAssignmentWorkQueue).dialog({
                    autoOpen: false,
                    height: 500,
                    width: 850,
                    modal: true,
                });
                $(elementIDs.taskAssignmentWorkQueue).dialog('option', 'title', isUpdate ? 'Update Task Status' : 'View Task Details');
                loadTaskAssignmentWorkQueueDialog(rowId, isUpdate);
                $(elementIDs.taskAssignmentWorkQueue).dialog("open");
            }
        };

        function loadTaskAssignmentWorkQueueDialog(rowId, isUpdate) {
            $("#optradioWorkQueuePortfolio").prop("checked", true);
            var workQueueGriddata = $(elementIDs.workQueueGridJQ).jqGrid('getRowData', rowId);
            var rowdata = $(elementIDs.workQueueGridJQ).jqGrid('getRowData', rowId);
            $("#etadfloderid").text("Mapping Row Id : " + rowdata.MappingRowID);
            MappingRowIDForTask = rowdata.MappingRowID;
            $(elementIDs.MappingRowIDForTaskHDValQ).text(rowdata.MappingRowID);
            IsPlanTaskQueueInfoUpdated = false;

            if ($('input[type=radio][name=optradioWorkQueueFolderType]')[0].id != 'optradioWorkQueuePortfolio') {
                $(elementIDs.taskAssigWorkQueueAccountListLabelDiv).css('display', 'block');
                $(elementIDs.taskAssigWorkQueueAccountListDiv).css('display', 'block');
            }
            else {
                $(elementIDs.taskAssigWorkQueueAccountListLabelDiv).css('display', 'none');
                $(elementIDs.taskAssigWorkQueueAccountListDiv).css('display', 'none');
            }
            //Check for Update if update then load update settings or else load view settings
         
            DisableTaskAssignWorkQueueControl();
            selectedTaskFolderversionWorkFlowState = workQueueGriddata.Workflow;
            selectedTaskstate = rowdata.Status;

            loadDataToTaskAssignWorkQueueDialog(rowdata.MappingRowID);
            taskAssigWorkQueuehComments(rowdata.MappingRowID, isUpdate);
            if (isUpdate == true) {
                IsSaveWorkQueueProcessDone = false;
                $(elementIDs.HiddenParameter).text("SaveUnDone");
                loadTaskAssignWorkQueueUpdateSettings(rowdata.MappingRowID);
            }
            else {
                loadTaskAssignWorkQueueViewSettings(rowdata.MappingRowID);
            }
        }

        function loadTaskAssignWorkQueueUpdateSettings(planTaskUserMapId) {
            EnableTaskAssignWorkQueueControl();
        }
        function loadTaskAssignWorkQueueViewSettings(planTaskUserMapId) {

        }
        function loadDataToTaskAssignWorkQueueDialog(planTaskUserMapId) {
            var url = '/PlanTaskUserMapping/GetPlanTaskUserMappingList?PlanTaskUserMappingId={PlanTaskUserMappingId}';
            var promise = ajaxWrapper.getJSON(url.replace(/{PlanTaskUserMappingId}/g, planTaskUserMapId));
            promise.done(function (list) {
                if (list != null && list != undefined && list.length > 0) {
                    var designDetailsJson = $.parseJSON(list[0].PlanTaskUserMappingDetails);
                    var accountName = list[0].AccountName;
                    var accountID;
                    if (accountName != undefined && accountName != "NA") {
                        accountID = accountName.split('|')[1];
                        $(elementIDs.taskAssigWorkQueueAccountListLabelDiv).css('display', 'block');
                        $(elementIDs.taskAssigWorkQueueAccountListDiv).css('display', 'block');
                        $("#optradioWorkQueueAccount").prop('checked', 'checked');
                    }
                    else {
                        $(elementIDs.taskAssigWorkQueueAccountListLabelDiv).css('display', 'none');
                        $(elementIDs.taskAssigWorkQueueAccountListDiv).css('display', 'none');
                        $("#optradioWorkQueuePortfolio").prop('checked', 'checked');
                    }
                    if (list[0].AssignedUserName != undefined) {
                        list[0].AssignedUserName = list[0].AssignedUserName.split(',').join(", "); 
                    }
                    else {
                        list[0].AssignedUserName = "";
                    }
                    //list[0].AssignedUserName = list[0].AssignedUserName.split(',').join(", ");
                    $(elementIDs.taskAssigWorkQueueAccountList).empty();
                    $(elementIDs.taskAssigWorkQueueAccountList).append("<option value='0'>" + accountName.split('|')[0] + "</option>");
                    $(elementIDs.taskAssigWorkQueueFolderList).empty();
                    $(elementIDs.taskAssigWorkQueueFolderList).append("<option value='0'>" + list[0].FolderName + "</option>");
                    $(elementIDs.taskAssigWorkQueueVersionList).empty();
                    $(elementIDs.taskAssigWorkQueueVersionList).append("<option value='0'>" + list[0].FolderVersionNumber + "</option>");
                    $(elementIDs.taskAssigWorkQueueWorkFlowStatusList).empty();
                    $(elementIDs.taskAssigWorkQueueWorkFlowStatusList).append("<option value='0'>" + list[0].WorkflowState + "</option>");
                    //$(elementIDs.taskAssigWorkQueuePlansList).empty();
                    //$(elementIDs.taskAssigWorkQueuePlansList).append("<option value='0'>" + designDetailsJson.FormInstanceLabel + "</option>");
                    $(elementIDs.taskAssigWorkQueueTasksList).empty();
                    $(elementIDs.taskAssigWorkQueueTasksList).append("<option value='0'>" + list[0].TaskDescription + "</option>");
                    $(elementIDs.taskAssigWorkQueueAssignUserList).empty();
                    $(elementIDs.taskAssigWorkQueueAssignUserList).append("<option value='0'>" + list[0].AssignedUserName + "</option>");
                    $(elementIDs.taskAssigWorkQueueEstTime).val(list[0].EstimatedTime);
                    $(elementIDs.taskAssigWorkQueueActualTime).val(list[0].ActualTime);
                    //$(elementIDs.taskAssigWorkQueueTaskStatus).empty();
                    //$(elementIDs.taskAssigWorkQueueTaskStatus).append("<option value='0'>" + list[0].Status + "</option>");
                    var startDate = new Date(list[0].StartDate);
                    var startday = ("0" + startDate.getDate()).slice(-2);
                    var startmonth = ("0" + (startDate.getMonth() + 1)).slice(-2);
                    var startDateValue = (startmonth) + "/" + (startday) + "/" + startDate.getFullYear();
                    $(elementIDs.taskAssigWorkQueueStartdate).val(startDateValue);
                    var dueDate = new Date(list[0].DueDate);
                    var dueday = ("0" + dueDate.getDate()).slice(-2);
                    var duemonth = ("0" + (dueDate.getMonth() + 1)).slice(-2);
                    var dueDateValue = (duemonth) + "/" + (dueday) + "/" + dueDate.getFullYear();
                    var UPViewID = designDetailsJson != null ? designDetailsJson.FormDesignVersionId : "";
                    var UPSectionID = designDetailsJson != null ? designDetailsJson.SectionId : "";
                    var UPTFormInstanceId = designDetailsJson != null ? designDetailsJson.FormInstanceId : "";

                    var UPOrder = list[0].Order;
                    var UPDuration = list[0].Duration
                    fillQueueTaskPrioityList(UPOrder);
                    //$(elementIDs.taskAssigWorkQueueOrder).val(UPOrder);
                    $(elementIDs.taskAssigWorkQueueDuedate).val(dueDateValue);
                    $(elementIDs.taskAssigWorkQueueDuration).val(UPDuration);

                    MappingRowIDForTask = planTaskUserMapId;
                    $(elementIDs.taskAssigWorkQueuehComments).val(list[0].Comments);
                    var urlTaskStatusList = '/PlanTaskUserMapping/GetWatchTaskStatusList';
                    var promiseTaskStatusList = ajaxWrapper.getJSON(urlTaskStatusList);
                    promiseTaskStatusList.done(function (listTaskStatus) {
                        $(elementIDs.taskAssigWorkQueueTaskStatus).empty();
                        $(elementIDs.taskAssigWorkQueueTaskStatus).append("<option value='0'>" + "Select One" + "</option>");

                        if (list[0].Status == "Late") {
                            $(elementIDs.taskAssigWorkQueueTaskStatus).append("<option value=" + "4" + ">" + "Late" + "</option>");
                            $(elementIDs.taskAssigWorkQueueTaskStatus).val("4");
                        }
                        var statusIdToSet = 0;

                        for (i = 0; i < listTaskStatus.length; i++) {
                            if (list[0].Status == "Late") {
                                if (listTaskStatus[i].TaskStatusDescription == "Completed" || listTaskStatus[i].TaskStatusDescription == "Completed - Fail" || listTaskStatus[i].TaskStatusDescription == "Completed - Pass"
                                    || listTaskStatus[i].TaskStatusDescription == "InProgress") {
                                    $(elementIDs.taskAssigWorkQueueTaskStatus).append("<option value=" + listTaskStatus[i].TaskStatusID + ">" + listTaskStatus[i].TaskStatusDescription + "</option>");
                                    if (listTaskStatus[i].TaskStatusDescription == list[0].Status) {
                                        statusIdToSet = listTaskStatus[i].TaskStatusID;
                                    }
                                }
                            }
                            else {
                                $(elementIDs.taskAssigWorkQueueTaskStatus).append("<option value=" + listTaskStatus[i].TaskStatusID + ">" + listTaskStatus[i].TaskStatusDescription + "</option>");
                                if (listTaskStatus[i].TaskStatusDescription == list[0].Status) {
                                    statusIdToSet = listTaskStatus[i].TaskStatusID;
                                }
                            }
                        }

                        $(elementIDs.taskAssigWorkQueueTaskStatus + " option").each(function () {
                            var $thisOption = $(this);
                            var valueToCompare = "4";

                            if ($thisOption.val() == valueToCompare) {
                                $thisOption.attr("disabled", "disabled");
                            }
                        });

                        //$(elementIDs.taskAssigWorkQueueTaskStatus + " option:selected").text(list[0].Status);
                        if (list[0].Status == "Late") {
                            $(elementIDs.taskAssigWorkQueueTaskStatus).val("4");
                        }
                        else {
                            $(elementIDs.taskAssigWorkQueueTaskStatus).val(statusIdToSet).change();
                        }
                        MappingRowIDForTask = planTaskUserMapId;
                        //populate plans
                        // Populate Plan combo box
                        $(elementIDs.taskAssigWorkQueuePlansList).empty();
                        var folderVersionIdValue = list[0].FolderVersionID
                        var folderIdValue = list[0].FolderID;
                        if (folderIdValue != null && folderIdValue != undefined && folderIdValue != "0"
                            && folderVersionIdValue != null && folderVersionIdValue != undefined && folderVersionIdValue != "0") {
                            var url = '/FolderVersion/GetTasksFormInstanceListForFolderVersion?folderVersionId={folderVersionId}&folderId={folderId}';
                            var promisePlanList = ajaxWrapper.getJSON(url.replace(/{folderVersionId}/g, folderVersionIdValue).replace(/{folderId}/g, folderIdValue));
                            promisePlanList.done(function (Planlist) {
                                $(elementIDs.taskAssigWorkQueuePlansList).empty();
                                $(elementIDs.taskAssigWorkQueuePlansList).append("<option value='0'>" + ALLPlans + "</option>");
                                for (i = 0; i < Planlist.length; i++) {
                                    $(elementIDs.taskAssigWorkQueuePlansList).append("<option value=" + Planlist[i].FormInstanceID + ">" + Planlist[i].FormDesignName + "</option>");
                                }

                                $(elementIDs.taskAssigWorkQueuePlansList).val(UPTFormInstanceId).change();
                                //PopulateViews
                                $(elementIDs.taskAssigWorkQueueViewList).empty();
                                //var url = '/PlanTaskUserMapping/GetFormDesignVersionList';
                                var url = '/PlanTaskUserMapping/GetFormDesignVersionList?folderVersionId=' + folderVersionIdValue;
                                var promiseViewList = ajaxWrapper.getJSON(url);
                                promiseViewList.done(function (list) {
                                    $(elementIDs.taskAssigWorkQueueViewList).append("<option value='0'>" + "Select One" + "</option>");
                                    if (list.length > 1) {
                                        var allViewIds = "";
                                        $(list).each(function (index, value) {
                                            allViewIds = allViewIds + ',' + value.FormDesignVersionId;
                                        });
                                        allViewIds = allViewIds.substring(1, allViewIds.length);
                                        $(elementIDs.taskAssigWorkQueueViewList).append("<option value='" + allViewIds + "'>" + ALLViews + "</option>");
                                    }
                                    for (i = 0; i < list.length; i++) {
                                        $(elementIDs.taskAssigWorkQueueViewList).append("<option value=" + list[i].FormDesignVersionId + ">" + list[i].FormDesignName + "</option>");
                                    }
                                    if (designDetailsJson != null && designDetailsJson.FormDesignVersionLabel != ALLViews && UPViewID != undefined && UPViewID.indexOf(',') >= 0) {
                                        var ids = UPViewID.split(',');
                                        $(ids).each(function (i, e) {
                                            $(elementIDs.taskAssigWorkQueueViewList + " option[value='" + e + "']").prop("selected", true);
                                        });
                                    }
                                    else
                                        $(elementIDs.taskAssigWorkQueueViewList).val(UPViewID).change();

                                    //Populate Sections
                                    $(elementIDs.taskAssigWorkQueueSectionList).empty();
                                    if (UPViewID != null && UPViewID != undefined && UPViewID != "") {
                                        var url = '/PlanTaskUserMapping/GetSectionsList?tenantId={tenantId}&formDesignVersionId={formDesignVersionId}';
                                        var promiseSectionList = ajaxWrapper.getJSON(url.replace(/{tenantId}/g, 1).replace(/{formDesignVersionId}/g, UPViewID));
                                        promiseSectionList.done(function (sections) {
                                            if (sections.length > 1) {
                                                var allSectionIds = "";
                                                $(sections).each(function (index, value) {
                                                    allSectionIds = allSectionIds + ',' + value.ID;
                                                });
                                                allSectionIds = allSectionIds.substring(1, allSectionIds.length);
                                                $(elementIDs.taskAssigWorkQueueSectionList).append("<option value='" + allSectionIds + "'>" + ALLSections + "</option>");
                                            }
                                            for (i = 0; i < sections.length; i++) {
                                                $(elementIDs.taskAssigWorkQueueSectionList).append("<option value=" + sections[i].ID + ">" + sections[i].Label + "</option>");
                                            }
                                            if (designDetailsJson != null && designDetailsJson.SectionLabel != ALLSections && UPSectionID != undefined && UPSectionID.indexOf(',') >= 0) {
                                                var ids = UPSectionID.split(',');
                                                $(ids).each(function (i, e) {
                                                    $(elementIDs.taskAssigWorkQueueSectionList + " option[value='" + e + "']").prop("selected", true);
                                                });
                                            }
                                            else
                                                $(elementIDs.taskAssigWorkQueueSectionList).val(UPSectionID).change();
                                        });
                                        promiseSectionList.fail(showError);
                                    }
                                });
                                promiseViewList.fail(showError);
                            });
                        }
                    });
                    promiseTaskStatusList.fail(showError);
                }
            });
            promise.fail(showError);
        }
        function DisableTaskAssignWorkQueueControl() {
            //$(elementIDs.taskAssigWorkQueueFolderList).addClass("ui-state-disabled");
            //$(elementIDs.taskAssigWorkQueueVersionList).addClass("ui-state-disabled");
            //$(elementIDs.taskAssigWorkQueueWorkFlowStatusList).addClass("ui-state-disabled");
            //$(elementIDs.taskAssigWorkQueuePlansList).addClass("ui-state-disabled");
            //$(elementIDs.taskAssigWorkQueueTasksList).addClass("ui-state-disabled");
            //$(elementIDs.taskAssigWorkQueueAssignUserList).addClass("ui-state-disabled");
            //$(elementIDs.taskAssigWorkQueueTaskStatus).addClass("ui-state-disabled");
            //$(elementIDs.taskAssigWorkQueuehComments).addClass("ui-state-disabled");
            //$(elementIDs.taskAssigWorkQueueStartdate).addClass("ui-state-disabled");
            //$(elementIDs.taskAssigWorkQueueDuedate).addClass("ui-state-disabled");
            //$(elementIDs.taskAssigWorkQueueCancelBtn).addClass("ui-state-disabled");
            //$(elementIDs.taskAssigWorkQueueSaveBtn).addClass("ui-state-disabled");

            $(elementIDs.taskAssigWorkQueueFolderList).attr("disabled", "disabled");
            $(elementIDs.taskAssigWorkQueueVersionList).attr("disabled", "disabled");
            $(elementIDs.taskAssigWorkQueueWorkFlowStatusList).attr("disabled", "disabled");
            $(elementIDs.taskAssigWatchOrder).attr("disabled", "disabled");
            $(elementIDs.taskAssigWatchDuration).attr("disabled", "disabled");
            $(elementIDs.taskAssigWorkQueuePlansList).attr("disabled", "disabled");
            $(elementIDs.taskAssigWorkQueueTasksList).attr("disabled", "disabled");
            $(elementIDs.taskAssigWorkQueueAssignUserList).attr("disabled", "disabled");
            $(elementIDs.taskAssigWorkQueueTaskStatus).attr("disabled", "disabled");
            $(elementIDs.taskAssigWorkQueuehComments).attr("disabled", "disabled");
            $(elementIDs.taskAssigWorkQueueStartdate).attr("disabled", "disabled");
            $(elementIDs.taskAssigWorkQueueDuedate).attr("disabled", "disabled");
            //   $(elementIDs.taskAssigWorkQueueCancelBtn).attr("disabled", "disabled");
            $(elementIDs.taskAssigWorkQueueSaveBtn).attr("disabled", "disabled");
            $(elementIDs.taskAssigWorkQueueStartdate).datepicker('disable');
            $(elementIDs.taskAssigWorkQueueDuedate).datepicker('disable');
            $(elementIDs.taskAssigWorkQueueViewList).attr("disabled", "disabled");
            $(elementIDs.taskAssigWorkQueueSectionList).attr("disabled", "disabled");
            $(elementIDs.taskAssigWorkQueueOrder).attr("disabled", "disabled");
            $(elementIDs.taskAssigWorkQueueDuration).attr("disabled", "disabled");            
            $(elementIDs.taskAssigWorkQueueEstTime).attr("disabled", "disabled");

        }
        function EnableTaskAssignWorkQueueControl() {
            //$(elementIDs.taskAssigWorkQueueTaskStatus).removeClass("ui-state-disabled");
            //$(elementIDs.taskAssigWorkQueuehComments).removeClass("ui-state-disabled");
            //$(elementIDs.taskAssigWorkQueueCancelBtn).removeClass("ui-state-disabled");
            //$(elementIDs.taskAssigWorkQueueSaveBtn).removeClass("ui-state-disabled");

            $(elementIDs.taskAssigWorkQueueTaskStatus).removeAttr("disabled");
            $(elementIDs.taskAssigWorkQueuehComments).removeAttr("disabled");
            $(elementIDs.taskAssigWorkQueueCancelBtn).removeAttr("disabled");
            $(elementIDs.taskAssigWorkQueueSaveBtn).removeAttr("disabled");
        }
        function SaveTaskAssignWorkQuereOperation() {

            var taskStatus = $(elementIDs.taskAssigWorkQueueTaskStatus + ' option:selected').text();
            if (taskStatus == "Completed" || taskStatus == "Completed - Fail" || taskStatus == "Completed - Pass") {
                yesNoConfirmDialog.show(DashBoard.confirmTaskCompleteMsg, function (e) {
                    yesNoConfirmDialog.hide();
                    if (e) {
                        FinalTaskAssignWorkQueueOperation();
                    }
                    else {
                        taskAssignmentWorkQueuelistDialogClose();
                        //  $(elementIDs.taskAssignmentWorkQueue).dialog("close");
                    }
                });
            }
            else {
                FinalTaskAssignWorkQueueOperation();
            }
        }
        function FinalTaskAssignWorkQueueOperation() {
            var saveOperatonDone = $(elementIDs.HiddenParameter).text();
            if (saveOperatonDone == "SaveUnDone") {
                MappingRowIDForTask = $(elementIDs.MappingRowIDForTaskHDValQ).text();
                var taskStatus = $(elementIDs.taskAssigWorkQueueTaskStatus + ' option:selected').text();
                var taskStatusVal = $(elementIDs.taskAssigWorkQueueTaskStatus).val();
                if (taskStatus == null || taskStatus == undefined || taskStatus == "" || taskStatus == "Select One") {
                    messageDialog.show("Please select proper status");
                }
                else {
                    var decimal = /^\d*[0-9]\d*$/;
                    var actualTime = $(elementIDs.taskAssigWorkQueueActualTime).val();
                    if (!$.isNumeric(actualTime) || ! actualTime.match(decimal)) {
                        messageDialog.show("Actual Time must be numeric");
                    }
                    else {
                        var rowIds = $(elementIDs.taskAssigWorkQueuehCommentsJQ).getDataIDs();
                        var taskCommentsGridData = [];
                        var filename;
                        //for (index = 0; index < rowIds.length; index++) {
                        //    var rowData = $(elementIDs.taskAssigWorkQueuehCommentsJQ).getRowData(rowIds[index]);
                        //    if (rowData.IsNew == 1) {
                        //        var attachment = rowData.Attachment != "" ? $.parseHTML(rowData.Attachment)[0].id : ""
                        //        rowData.Attachment = attachment;
                        //        taskCommentsGridData.push(rowData);
                        //        rowData.IsNew = 0;
                        //    }
                        //}
                        for (var i = rowIds.length ; i >= 0; i--) {
                            var rowData = $(elementIDs.taskAssigWorkQueuehCommentsJQ).getRowData(i);
                            if (rowData.IsNew == 1) {
                                var attachment = rowData.Attachment != "" ? $.parseHTML(rowData.Attachment)[0].id : "";
                                rowData.Attachment = attachment;
                                //  rowdata.filename =filename ;
                                taskCommentsGridData.push(rowData);
                                rowData.IsNew = 0;

                            }
                        }
                        var assignedUserName = $(elementIDs.taskAssigWorkQueueAssignUserList + ' option:selected').text();
                        var taskDescription = $(elementIDs.taskAssigWorkQueueTasksList + ' option:selected').text();
                        var input = {
                            Status: taskStatus,
                            ID: MappingRowIDForTask,
                            AssignedUserName: assignedUserName,
                            TaskComments: JSON.stringify(taskCommentsGridData),
                            EstimatedTime: $(elementIDs.taskAssigWorkQueueEstTime).val(),
                            ActualTime: $(elementIDs.taskAssigWorkQueueActualTime).val(),
                            TaskDescription: taskDescription,
                            FolderName: $(elementIDs.taskAssigWorkQueueFolderList + ' option:selected').text(),
                            AccountName: $(elementIDs.taskAssigWorkQueueAccountList + ' option:selected').text()
                        }
                        var url = null;
                        IsPlanTaskQueueInfoUpdated = false;
                        IsSaveWorkQueueProcessDone = true;
                        $(elementIDs.HiddenParameter).text("SaveDone");
                        url = '/PlanTaskUserMapping/UpdateQueuePlanTaskUserMapping';
                        if (url != null) {
                            var promise = ajaxWrapper.postJSON(url, input);
                            promise.done(function (list) {
                                $(elementIDs.taskAssigWorkQueuehComments).val("");
                                IsPlanTaskQueueInfoUpdated = false;
                                IsSaveWorkQueueProcessDone = true;
                                messageDialog.show(DashBoard.planTaskUserMapUpdateMsg);
                                //  $(elementIDs.workQueueGridJQ).trigger('reloadGrid');
                                $(elementIDs.workQueueGridJQ).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
                            });

                            promise.fail(showError);
                            $(elementIDs.taskAssignmentWorkQueue).dialog("close");
                        }
                    }
                }
            }
        }

        function taskAssigWorkQueuehComments(Taskid, transactionMode, gridName) {
            var isAttachmentsOnly = false;
            if (gridName != undefined) {
                isAttachmentsOnly = true;
            }
            else
                gridName = elementIDs.taskAssigWorkQueuehCommentsJQ;
            //set column list for grid
            var colArray = ['Task Number', 'Comment', 'Status', 'Added Date', 'Added By', 'IsNew', 'Attachment', 'Folder Version', 'FolderVersionID', 'filename'];

            //set column models
            var colModel = [];
            colModel.push({ name: 'TaskID', index: 'TaskID', width: '60px', editable: false, hidden: true });
            colModel.push({ name: 'Comment', index: 'Comment', editable: false, width: '150px' });
            colModel.push({ name: 'Status', index: 'Status', editable: false, width: '80px' });
            colModel.push({ name: 'Datetimestamp', index: 'Datetimestamp', editable: false, formatter: 'date', width: '100px', formatoptions: JQGridSettings.DateTimeFormatterOptions });
            colModel.push({ name: 'AddedBy', index: 'AddedBy', editable: false, width: '80px' });
            colModel.push({ name: 'IsNew', index: 'IsNew', editable: false, width: '80px', hidden: true });
            colModel.push({ name: 'Attachment', index: 'Attachment', editable: true, align: 'center', formatter: AttachmentButtonFormatter });
            colModel.push({ name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: false, align: 'center' });
            colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', editable: false, align: 'center', hidden: true });
            colModel.push({ name: 'filename', index: 'filename', editable: false, align: 'center', hidden: true });

            //clean up the grid first - only table element remains after this
            $(gridName).jqGrid('GridUnload');

            ////adding the pager element           
            $(gridName).parent().append("<div id='p" + elementIDs.taskAssigWorkQueuehComments + "'></div>");

            var url = "/DashBoard/GetCommentList?Taskid={Taskid}&isAttachmentsOnly={isAttachmentsOnly}".replace('{Taskid}', Taskid).replace('{isAttachmentsOnly}', isAttachmentsOnly);

            $(gridName).jqGrid({
                url: url,
                datatype: 'json',
                cache: false,
                colNames: colArray,
                colModel: colModel,
                //  caption: 'Task Comments',
                height: 'auto',
                rowNum: 10,
                ignoreCase: true,
                loadonce: true,
                viewrecords: true,
                hidegrid: true,
                altRows: true,
                sortname: 'MappingRowID',
                sortorder: 'desc',
                pager: '#p' + elementIDs.taskAssigWorkQueuehComments,
                altclass: 'alternate',
                rowList: [10, 20, 30],
                loadComplete: function () {
                    $(".WFSTATEACCESS").unbind('click');
                    $(".WFSTATEACCESS").click(function () {
                        var cellvalue = $(this).attr("id");
                        var fileName = $(this)[0].parentElement.innerText;
                        DownLoadDocument(cellvalue, fileName);
                    });
                    $(elementIDs.taskCommentsDailog).dialog({
                        position: {
                            my: 'center',
                            at: 'center'
                        },
                    });

                }
            });

            var pagerElement = '#p' + elementIDs.taskAssigWorkQueuehComments;
            //$('#p' + elementIDs.taskAssigWorkQueuehComments).find('input').css('height', '20px');
            $(gridName).jqGrid('navGrid', pagerElement, { edit: false, view: false, add: false, del: false, search: false, refresh: false });

            ////remove paging
            //$(pagerElement).find(pagerElement + '_center').remove();
            //$(pagerElement).find(pagerElement + '_right').remove();
            $(elementIDs.newcommentDialog).dialog({
                autoOpen: false,
                height: 500,
                width: 450,
                modal: true,
                open: function () {
                    var par = $(elementIDs.newcommentDialog).data('uniqueID');
                    //    $('#UniqueName').val(par);
                }
            });
            if (transactionMode.toString() == 'true') {
                $(gridName).jqGrid('navButtonAdd', pagerElement,
                {
                    caption: '',
                    buttonicon: 'ui-icon-plus',
                    title: 'New Task comment',
                    id: 'btnNewcomment',
                    disabled: 'disabled',
                    onClickButton: function () {
                        $(elementIDs.newcommentDialog).dialog('option', 'title', "Comment");
                        var uniqueID = Math.floor(Math.random() * 26) + Date.now();
                        $(elementIDs.uploadWorkCommentAttachment).val('');
                        $(elementIDs.newcommentDialog).data('uniqueID', uniqueID).dialog("open");
                        $(elementIDs.newcommentDialog).dialog({
                            position: {
                                my: 'center',
                                at: 'center'
                            },
                        });

                    }
                });
            }
            $(elementIDs.btnsavecommentJQ).off('click').click(function () {
                var comment = $(elementIDs.newcommentJQ).val();
                var performOperation = true;
                if (comment == "" || comment == null) {
                    $(elementIDs.newcommentJQ).parent().addClass('has-error');
                    $(elementIDs.newcommentJQ).addClass('form-control');
                    $(elementIDs.newcommentSpan).text("Please enter comment");
                    //       strErrorList = strErrorList + ", Tasks";
                    performOperation = false;
                }
                else {
                    $(elementIDs.newcommentJQ).parent().removeClass('has-error');
                    $(elementIDs.newcommentJQ).removeClass('form-control');
                    $(elementIDs.newcommentSpan).text('');
                }
                if (performOperation) {
                    var filename = $(elementIDs.uploadWorkCommentAttachment).val();
                    var fileNameUpload = $(elementIDs.uploadWorkCommentAttachment).get(0);
                    var fileNamefiles = fileNameUpload.files;
                    var fileData = new FormData();
                    // Looping over all files and add it to FormData object  
                    for (var i = 0; i < fileNamefiles.length; i++) {
                        fileData.append(fileNamefiles[i].name, fileNamefiles[i]);
                    }
                    $.ajax({
                        url: '/DashBoard/uploadExcel',
                        type: "POST",
                        contentType: false, // Not to set any content header  
                        processData: false, // Not to process data  
                        data: fileData,
                        success: function (result) {
                            var attachments;
                            if (result != "") {
                                attachments = result;
                            }
                            var comment = $(elementIDs.newcommentJQ).val();
                            var status = $(elementIDs.taskAssigWorkQueueTaskStatus + ' option:selected').text();
                            var folderVersionID = $(elementIDs.taskAssigWatchVersionList).val();
                            var folderVersion = $(elementIDs.taskAssigWorkQueueVersionList).find('option:selected').text();
                            var currentdate = new Date();
                            var rowCount = $(elementIDs.taskAssigWorkQueuehCommentsJQ).jqGrid('getGridParam', 'records');
                            var file = fileNamefiles.length > 0 ? fileNamefiles[0].name : "";
                              var data = [{ TaskID: "", Datetimestamp: currentdate, Comment: comment, Status: status, Attachment: attachments, filename: file, IsNew: 1, FolderVersionID: folderVersionID, FolderVersionNumber: folderVersion, AddedBy: "" }];
                            $(elementIDs.taskAssigWorkQueuehCommentsJQ).jqGrid('addRowData', rowCount + 1, data[0], "first");
                            $(elementIDs.newcommentJQ).val('');
                            $(elementIDs.uploadWorkCommentAttachment).val('');
                            $(elementIDs.newcommentDialog).dialog("close");
                        },
                        error: function (err) {
                            messageDialog.show(err.statusText);
                        }
                    });
                }
            });
        }

        function AttachmentButtonFormatter(cellvalue, options, rowObject) {
            if (cellvalue == undefined || cellvalue == "" || rowObject.filename == null) {
                return '';
            }
            var link = "<a href='#' class='WFSTATEACCESS' id='" + cellvalue + "'>" + rowObject.filename + "<span align='center' class='ui-icon ui-icon-arrowreturnthick-1-s' style='margin:auto' title='Download Attachment' ></span></a>";

            return link;
        }

        function DownLoadDocument(cellvalue, fileName) {
            $.download('/DashBoard/DownloadDocument?Attachment=' + cellvalue + '&file=' + fileName, "test", 'post');
        }
        return {
            loadWorkQueue: function () {
                loadWorkQueueGrid();
            }
        }
    }
    return {
        loadFormUpdates: function () {
            workQueue();
            watch();
            $("#workQueue").setGridWidth("1238", true);

        }
    }
}();

//initTabs();

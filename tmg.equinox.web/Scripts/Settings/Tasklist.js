var tasklist = function () {

    var URLs = {
        getTaskList: "/Task/GetTaskList?tenantID=1",
        deleteTask: "/Task/DeleteTask",
    };

    var elementIDs = {
        settingsTabJQ: "#settingsTab",
        tasklistGrid: "TasklistGrid",
        tasklistGridJQ: "#TasklistGrid"
    };
    //this function is called below soon after this JS file is loaded
    function init() {
        $(document).ready(function () {
            registerEvents();
        });
    }
    function loadTaskListGrid() {

        var colArray = ['', 'Task', 'Is standard Task'];
        //set column models
        var colModel = [];
        colModel.push({ key: true, name: 'TaskID', index: 'TaskID', editable: true, hidden: true });
        colModel.push({ name: 'TaskDescription', index: 'TaskDescription', width: '790px' });
        colModel.push({ name: 'IsStandardTask', index: 'IsStandardTask', width: '200px', editable: false,desable:true, formatter: checkBoxFormatterDesign, unformat: unFormatIncludedColumnDesign, });
        //adding the pager element
        $(elementIDs.tasklistGridJQ).parent().append("<div id='p" + elementIDs.tasklistGrid + "'></div>");
        //clean up the grid first - only table element remains after this
        $(elementIDs.tasklistGridJQ).jqGrid('GridUnload');
        $(elementIDs.tasklistGridJQ).jqGrid({
            datatype: 'json',
            url: URLs.getTaskList,
            cache: false,
            altclass: 'alternate',
            colNames: colArray,
            colModel: colModel,
            caption: '',
            height: '300',
            rowNum: 10,
            loadonce: true,
            //autowidth: true,
            viewrecords: true,
            hidegrid: false,
            hiddengrid: false,
           // ignoreCase: false,
            caption: "Tasks",
            sortable: true,
            sortname: 'TaskID',
            sortorder: 'desc',
            pager: '#p' + elementIDs.tasklistGrid,
            editurl: 'clientArray',
            altRows: true,
            loadComplete: function ()
            {
              
                return true;
            }

        });

      
       
        var pagerElement = '#p' + elementIDs.tasklistGrid;
        $(pagerElement).find('input').css('height', '20px');

        //remove default buttons
        $(elementIDs.tasklistGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

        $(elementIDs.tasklistGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

        //edit button in footer of grid 
        $(elementIDs.tasklistGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-pencil',
            title: 'Edit', id: 'btntasklistEdit',
            onClickButton: function () {

                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    var row = $(elementIDs.tasklistGridJQ).getRowData(rowId);
                    //load Add Task dialog for edit on click - see categoryWorkflowDesignDialog function below
                    taskDialog.show('Tasks', row.TaskDescription, 'edit');
                    taskDialog.show('Tasks1', row.IsStandardTask, 'edit');
                    
                   
                } else {
                    messageDialog.show('Please select a row.');
                }
            }
        });

        //add button in footer of grid
        $(elementIDs.tasklistGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon-plus',
                title: 'Add', id: 'btntasklistAdd',
                onClickButton: function () {
                    //load Edit Task dialog on click - see categoryWorkflowDesignDialog function below
                    taskDialog.show('Tasks', '', 'add');
                }
            });

        //delete button in footer of grid
        $(elementIDs.tasklistGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-trash',
            title: 'Delete', id: 'btntasklistDelete',
            onClickButton: function () {
                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    //load confirm dialogue to asset the operation
                    confirmDialog.show(Common.deleteConfirmationMsg, function () {
                        confirmDialog.hide();
                        //delete the form design
                        var taskDelete = {
                            TaskID: rowId
                        };
                        var promise = ajaxWrapper.postJSON(URLs.deleteTask, taskDelete);
                        //register ajax success callback
                        promise.done(TaskDeleteSuccess);
                        //register ajax failure callback
                        promise.fail(showError);
                    });
                }
                else {
                    messageDialog.show('Please select a row.');
                }
            }
        });
        function checkBoxFormatterDesign(cellValue, options, rowObject) {
            return "<input id='para_" + options.rowId + (rowObject.IsStandardTask ? '\' checked=\'checked\' ' : '\'') + " type='checkbox' disabled='disabled'/>";
            //return "<select id=\'" + rowObject.RowID + "\'><option value=\"Account\">Account</option><option value=\"FolderVersion\">Folder Version</option><option value=\"Folder\">Folder</option><option value=\"Document\">Document</option></select>";
            //return str;
        }
        function unFormatIncludedColumnDesign(cellValue, options, rowObject) {
            return $('#para_' + options.rowId).prop('checked');
        }

    }
    //ajax callback success - reload Form Desing  grid
    function TaskDeleteSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(TasksListMessages.taskDeleteSuccess);
        }
        else {
            if (xhr.Items[0].Messages.length > 0) messageDialog.show(xhr.Items[0].Messages[0]);
            else messageDialog.show(Common.errorMsg);
        }
        loadTaskListGrid();
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
        $(".TASKLIST").unbind('click');
        $(".TASKLIST").click(function () {
            loadTaskListGrid();
            //show the selected tab
            $(elementIDs.settingsTabJQ).tabs({
                selected: 4
            });
        });
    }

    return {
        loadTaskList: function () {
            loadTaskListGrid();
        }
    }
}();
//contains functionality for the Document Design add/edit dialog
var taskDialog = function () {
    var URLs = {
        taskAdd: '/Task/AddTask',
        taskUpdate: '/Task/UpdateTask',

    }

    //see element id's in Views\FormDesign\Index.cshtml
    var elementIDs = {
        //form design dialog element
        taskDialog: "#taskDialog",
        tasknameText: "#taskname",
        TasklistGridJQ: "#TasklistGrid",
        IsStandardCheckbox:"#IsStandardCheck",
        workFlowStateApprovalTypeMasterGridJQ: "#workFlowStateApprovalTypeMasterGrid"
    };
 
    //maintains dialog state - add or edit
    var taskDesginDialog, taskDesginName;

    //ajax success callback - for add/edit
    function TaskSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            if (taskDesginDialog == 'add')
            {
                var msg = TasksListMessages.taskAddSuccess;

            }
            else {
                var msg = TasksListMessages.taskUpdateSuccess;
            }
            messageDialog.show(msg);
        }
        else {
            if (xhr.Items[0].Messages.length > 0) messageDialog.show(xhr.Items[0].Messages[0]);
            else messageDialog.show(Common.errorMsg);
        }
        //reload form design grid 
        tasklist.loadTaskList();
        //reset dialog elements
        $(elementIDs.taskDialog + ' div').removeClass('has-error');
        $(elementIDs.taskDialog + ' .help-block').text('');
        $(elementIDs.taskDialog).dialog('close');
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
        $(elementIDs.taskDialog).dialog({
            autoOpen: false,
            height: 250,
            width: 450,
            modal: true
        });
        //register event for Add/Edit button click on dialog
        $(elementIDs.taskDialog + ' button').on('click', function () {
            //check if name is already used
            var newName = $(elementIDs.taskDialog + ' input').val(), filterList;
             
            var chekedorNot = $(elementIDs.IsStandardCheckbox).is(":checked");
            var grid = $(elementIDs.TasklistGridJQ);
            var validationMsgRequired = TasksListMessages.taskRequiredError
            var validationMsgExists = TasksListMessages.taskExistsError

            var dataList = grid.getRowData();
            if (taskDesginName === "Tasks")
            {
                filterList = dataList.filter(function (ct)
                {
                    return compareStrings(ct.TaskDescription, newName, true);
                });
            }
            // validate form name
            if (filterList !== undefined && filterList.length > 0) {
                $(elementIDs.taskDialog + ' div').addClass('has-error');
                $(elementIDs.taskDialog + ' .help-block').text(validationMsgExists);
            }
            else if (newName == '') {
                $(elementIDs.taskDialog + ' div').addClass('has-error');
                $(elementIDs.taskDialog + ' .help-block').text(validationMsgRequired);
            }
            else {
                //save the new form design
                var rowId = grid.getGridParam('selrow');
                var TaskDesignAdd = {
                    tenantId: 1,
                    TaskID: rowId,
                    TaskDescription: newName,
                    IsStandardCheckbox: chekedorNot,
                };
                var url;
                if (taskDesginDialog == 'add') {
                    url = URLs.taskAdd;
                }
                else if (taskDesginDialog == 'edit') {
                    url = URLs.taskUpdate;
                }
                //ajax call to add/update
                var promise = ajaxWrapper.postJSON(url, TaskDesignAdd);
                //register ajax success callback
                promise.done(TaskSuccess);
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
        show: function (masterTaskName, taskValue, action) {
            taskDesginDialog = action;
            taskDesginName = masterTaskName;

            //set eleemnts based on whether the dialog is opened in add or edit mode
            $(elementIDs.taskDialog + ' div').removeClass('has-error');

            if (taskDesginDialog == 'add')
            {
                $(elementIDs.taskDialog).dialog('option', 'title', TasksListMessages.taskAddTitle);
                $(elementIDs.taskDialog + ' .help-block').text(TasksListMessages.taskAddHelpText);
                $(elementIDs.taskDialog + ' label').text(TasksListMessages.taskLabel);
                $(elementIDs.tasknameText).val('');

                
                                
            }
            else if (taskDesginDialog == 'edit')
            {
                if (taskDesginName == "Tasks")
                {
                    $(elementIDs.taskDialog).dialog('option', 'title', TasksListMessages.taskUpdateTitle);
                    $(elementIDs.tasknameText).val(taskValue);
                    $(elementIDs.taskDialog + ' .help-block').text(TasksListMessages.taskUpdateHelpText);
                    $(elementIDs.taskDialog + 'label').text(TasksListMessages.taskLabel);
                }
                else
                {
                    $(elementIDs.IsStandardCheckbox).val(taskValue);          
                    if (taskValue ==true)
                    {
                       
                        $(elementIDs.IsStandardCheckbox).prop('checked', true);
                    }
                    else
                    {
                        $(elementIDs.IsStandardCheckbox).prop('checked', false);
                    }
                   
                    $(elementIDs.taskDialog + 'Checkbox').text(TasksListMessages.taskIsstandardCheckboxlabel);

                }
                
               
               
              
            }
            //open the dialog - uses jqueryui dialog
            $(elementIDs.taskDialog).dialog("open");
        }
    }
}(); //invoked soon after loading
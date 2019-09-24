var settings = function () {
    isInitialized = false;
    tenantId = 1;

    var URLs = {
        getSessionTimeOut: "/Settings/GetSessionTimeOut?tenantId={tenantId}",
        saveAutoSaveSettings: "/Settings/SaveAutoSaveSettings",
        getAutoSaveSettingsForTenant: "/Settings/GetAutoSaveSettingsForTenant?tenantId={tenantId}",
        getFolderVersionCategoryList: "/Settings/GetFolderVersionCategoryList?tenantId=1",
        getFolderVersionGroupList: "/Settings/GetFolderVersionGroupList?tenantId=1",
        deleteFolderVersionGroup: "/Settings/DeleteFolderVersionGroup",
        deleteFolderVersionCategory: "/Settings/DeleteFolderVersionCategory",
        getlockDocumentList: "/FolderVersion/GetAllockedDocument",
        SaveUnLockTimeOutSetting: "/Settings/SaveUnLockTimeOutSetting",
        GetUnLockTimeOutSetting: "/Settings/GetUnLockTimeOutSetting"
    };

    var elementIDs = {
        settingsTabJQ: "#settingsTab",
        settingsTabControlsJQ: "#settingsTabControls",
        duationDDLAutoSaveJQ: "#duationDDLAutoSave",
        btnSaveJQ: "#btnSave",
        enableAutoSaveChkJQ: "#enableAutoSaveChk",
        enableAutoSaveChkHelpBlockJQ: "#enableAutoSaveChkHelpBlock",
        categoryListGrid: "categoryListGrid",
        categoryListGridJQ: "#categoryListGrid",
        groupNameListGrid: "groupListGrid",
        groupNameListGridJQ: "#groupListGrid",
        addCategoryListGridJQ: "addCategoryListGrid",
        editCategoryListGridJQ: "editCategoryListGrid",
        deleteCategoryListGridJQ: "deleteCategoryListGrid",

        addGroup: "addGroupListGrid",
        editGrouPName: "editGroupListGrid",
        LockDocumentGridJQ: "#LockDocumentGrid",
        LockDocumentGrid: "LockDocumentGrid",
        btnReleaseJQ: "#btnRelease",
        duationDDLUnlockJQ: "#duationDDLUnlockJQ",
        accelerateStartDateForTask:"#accelerateStartDateForTask"
    };

    //this function is called below soon after this JS file is loaded
    function init() {        
        $(elementIDs.enableAutoSaveChkJQ).click(function (e) {
            setAutoSaveState();
        });

        $(document).ready(function () {
            if (isInitialized == false) {
                $(elementIDs.settingsTabJQ).tabs().tabs("select", 0);
                //get auto saved setting data
                fillDropDownTimeDurations();
                fillDropDownUnlockTimeOutDurations();
                $(elementIDs.btnSaveJQ).click(function (e) {
                    saveSettingsData();
                });

                registerEvents();

                isInitialized = true;
                //if (roleId == Role.HNESuperUser) {
                //    $(elementIDs.btnSaveJQ).addClass('disabled-button');
                //}
            }
            getLockDocumentList();
        });
    }

    function loadCategoryListGrid() {
        var colArray = ['', '', 'Category Name', 'Group Name', ''];
        //set column models
        var colModel = [];
        colModel.push({ name: 'FolderVersionCategoryID', index: 'FolderVersionCategoryID', key: true, editable: false, hidden: true });
        colModel.push({ name: 'IsActive', index: 'IsActive', hidden: true });
        colModel.push({ name: 'FolderVersionCategoryName', index: 'FolderVersionCategoryName', editable: false, width: '495px' });
        colModel.push({ name: 'FolderVersionGroupName', index: 'FolderVersionGroupName', editable: false, width: '495px' });
        colModel.push({ name: 'FolderVersionGroupID', index: 'FolderVersionGroupID', editable: false, hidden: true });
        //adding the pager element
        $(elementIDs.categoryListGridJQ).parent().append("<div id='p" + elementIDs.categoryListGrid + "'></div>");


        //clean up the grid first - only table element remains after this
        $(elementIDs.categoryListGridJQ).jqGrid('GridUnload');
        $(elementIDs.categoryListGridJQ).jqGrid({
            datatype: 'json',
            url: URLs.getFolderVersionCategoryList,
            cache: false,
            altclass: 'alternate',
            colNames: colArray,
            colModel: colModel,
            caption: '',
            height: '200',
            rowNum: 10,
            //loadonce: true,
            //autowidth: true,
            viewrecords: true,
            hidegrid: false,
            hiddengrid: false,
            ignoreCase: true,
            caption: "Category",
            sortname: 'FolderVersionCategoryID',
            pager: '#p' + elementIDs.categoryListGrid,
            altRows: true,
            loadComplete: function () {
                //if (roleId == Role.HNESuperUser) {
                //    $("#" + elementIDs.addCategoryListGridJQ).addClass('disabled-button');
                //    $("#" + elementIDs.editCategoryListGridJQ).addClass('disabled-button');
                //    $("#" + elementIDs.deleteCategoryListGridJQ).addClass('disabled-button');
                //}
                return true;
            }
        });

        var pagerElement = '#p' + elementIDs.categoryListGrid;
        $(pagerElement).find('input').css('height', '20px');

        //remove default buttons
        $(elementIDs.categoryListGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

        $(elementIDs.categoryListGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

        //edit button in footer of grid 
        $(elementIDs.categoryListGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-pencil', title: 'Edit', id: elementIDs.editCategoryListGridJQ,
            onClickButton: function () {
                var categoryID = $(elementIDs.categoryListGridJQ).jqGrid('getGridParam', 'selrow');
                var rowData = $(elementIDs.categoryListGridJQ).getRowData(categoryID);
                if (categoryID != null)
                    consortiumDialog.show(rowData, 'update');
                else
                    messageDialog.show(FolderCategoryMessages.categoryRowSelectionMsg);
            }
        });


        $(elementIDs.categoryListGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon-plus', title: 'Add', id: elementIDs.addCategoryListGridJQ,
                onClickButton: function () {
                    consortiumDialog.show('', 'add');
                }
            });


        $(elementIDs.categoryListGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-trash', title: 'Remove', id: elementIDs.deleteCategoryListGridJQ,
               onClickButton: function () {
                   //remove selected row
                   var categoryID = $(elementIDs.categoryListGridJQ).jqGrid('getGridParam', 'selrow');
                   var rowData = $(elementIDs.categoryListGridJQ).getRowData(categoryID);
                   if (categoryID != null)
                       //consortiumDialog.show(rowData, 'delete');
                       confirmDialog.show('Are you sure want to delete?', function () {
                           confirmDialog.hide();
                           var data = {
                               tenantID: 1,
                               folderVersionCategoryID: rowData.FolderVersionCategoryID,
                               folderVersionCategoryName: rowData.FolderVersionCategoryName
                           }
                           var promise = ajaxWrapper.postJSON(URLs.deleteFolderVersionCategory, data);
                           //callback function for ajax request success.
                           promise.done(function (xhr) {
                               if (xhr.Result === ServiceResult.FAILURE) {
                                   if (xhr.Items[0].Messages.length > 0) messageDialog.show(xhr.Items[0].Messages[0]);
                                   else messageDialog.show(FolderCategoryMessages.deleteCategoryErrorMsg);
                               }
                               else {
                                   messageDialog.show(FolderCategoryMessages.deleteCategoryMsg);
                                   loadCategoryListGrid();
                               }
                           });
                       })
                   else
                       messageDialog.show(FolderCategoryMessages.groupRowDeleteSelectionMsg);
               }
           });


    }

    function loadGroupListGrid() {
        var colArray = ['', '', 'Group Name'];
        //set column models
        var colModel = [];
        colModel.push({ name: 'FolderVersionGroupID', index: 'FolderVersionGroupID', key: true, editable: false, hidden: true });
        colModel.push({ name: 'IsActive', index: 'IsActive', hidden: true });
        colModel.push({ name: 'FolderVersionGroupName', index: 'FolderVersionGroupName', editable: false, width: '990px' });
        //adding the pager element
        $(elementIDs.groupNameListGridJQ).parent().append("<div id='p" + elementIDs.groupNameListGrid + "'></div>");

        //clean up the grid first - only table element remains after this
        $(elementIDs.groupNameListGridJQ).jqGrid('GridUnload');
        $(elementIDs.groupNameListGridJQ).jqGrid({
            datatype: 'json',
            url: URLs.getFolderVersionGroupList,
            cache: false,
            altclass: 'alternate',
            colNames: colArray,
            colModel: colModel,
            caption: '',
            height: '200',
            rowNum: 10,
            //loadonce: true,
            //autowidth: true,
            viewrecords: true,
            hidegrid: false,
            hiddengrid: false,
            ignoreCase: true,
            caption: "Group",
            sortname: 'FolderVersionGroupID',
            pager: '#p' + elementIDs.groupNameListGrid,
            altRows: true,
            loadComplete: function () {
                //if (roleId == Role.HNESuperUser) {
                //    $("#" + elementIDs.editGrouPName).addClass('disabled-button');
                //    $("#" + elementIDs.addGroup).addClass('disabled-button');
                //}
                return true;
            }
        });

        var pagerElement = '#p' + elementIDs.groupNameListGrid;
        $(pagerElement).find('input').css('height', '20px');

        //remove default buttons
        $(elementIDs.groupNameListGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

        $(elementIDs.groupNameListGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

        //edit button in footer of grid 
        $(elementIDs.groupNameListGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-pencil', title: 'Edit', id: elementIDs.editGrouPName,
            onClickButton: function () {
                var groupID = $(elementIDs.groupNameListGridJQ).jqGrid('getGridParam', 'selrow');
                var rowData = $(elementIDs.groupNameListGridJQ).getRowData(groupID);
                if (groupID != null)
                    groupDialog.show(rowData, 'update');
                else
                    messageDialog.show(FolderCategoryMessages.categoryRowSelectionMsg);
            }
        });

        $(elementIDs.groupNameListGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon-plus', title: 'Add', id: elementIDs.addGroup,
                onClickButton: function () {
                    groupDialog.show('', 'add');
                }
            });

        // UnComment To Enable Delete Group Functionality  
        //$(elementIDs.groupNameListGridJQ).jqGrid('navButtonAdd', pagerElement,
        //   {
        //       caption: '', buttonicon: 'ui-icon-trash', title: 'Remove', id: 'btnDelete',
        //       onClickButton: function () {
        //           //remove selected row
        //           var groupID = $(elementIDs.groupNameListGridJQ).jqGrid('getGridParam', 'selrow');
        //           var rowData = $(elementIDs.groupNameListGridJQ).getRowData(groupID);
        //           if (groupID != null) {
        //               // groupDialog.show(rowData, 'delete');
        //               confirmDialog.show('Are you sure want to delete?', function () {
        //                   confirmDialog.hide();
        //                   var data = {
        //                       folderVersionGroupID: groupID,
        //                       folderVersionGroupName: rowData.FolderVersionGroupName
        //                   }
        //                   var promise = ajaxWrapper.postJSON(URLs.deleteFolderVersionGroup, data);
        //                   //callback function for ajax request success.
        //                   promise.done(function (xhr) {
        //                       if (xhr.Result === ServiceResult.FAILURE) {
        //                           messageDialog.show(FolderCategoryMessages.deleteGroupErrorMsg);
        //                       }
        //                       else {
        //                           messageDialog.show(FolderCategoryMessages.deleteGroupMsg);
        //                           $(elementIDs.groupNameListGridJQ).setGridParam({ datatype: 'json', page: 1 }).trigger('reloadGrid');;  //.trigger("reloadGrid");
        //                       }
        //                   });
        //               })
        //           }
        //           else
        //               messageDialog.show(ConsortiumMessages.groupRowDeleteSelectionMsg);
        //       }
        //   });
    }

    function getAutoSaveSettingsData() {
        var currentInstance = this;
        var url = URLs.getAutoSaveSettingsForTenant.replace(/\{tenantId\}/g, currentInstance.tenantId);
        var promise = ajaxWrapper.getJSON(url);
        promise.done(function (data) {
            if (data != null && data.IsAutoSaveEnabled != null && data.Duration != null) {
                if (data.AccelerateStartDateForTask !== undefined)
                {
                    $(elementIDs.accelerateStartDateForTask).attr("checked", data.AccelerateStartDateForTask);
                }
                
                $(elementIDs.enableAutoSaveChkJQ).attr("checked", data.IsAutoSaveEnabled);
                if (data.IsAutoSaveEnabled)
                    $(elementIDs.duationDDLAutoSaveJQ).val(data.Duration);
                else
                    $(elementIDs.duationDDLAutoSaveJQ).val(0)
                setAutoSaveState();
            }
        });
        promise.fail(showError);
    }

    function getUnlockTimeoutSettingsData() {
        var currentInstance = this;
        var url = URLs.GetUnLockTimeOutSetting .replace(/\{tenantId\}/g, currentInstance.tenantId);
        var promise = ajaxWrapper.getJSON(url);
        promise.done(function (data) {
            if (data != null) {
               $(elementIDs.duationDDLUnlockJQ).val(data);
            }
        });
        promise.fail(showError);
    }
 


    //TODO: No need to query on server, the max value can be pulled of from the Server & iterated here.
    function fillDropDownTimeDurations() {
        currentInstance = this;
        var options = "<option value='0'>" + "--Select--" + "</option>";
        var url = URLs.getSessionTimeOut.replace(/\{tenantId\}/g, currentInstance.tenantId);
        var promise = ajaxWrapper.getJSON(url);
        promise.done(function (duration) {
            for (i = 1; i < duration; i = i + 1) {
                options += "<option value=" + i + ">" + i + " min" + "</option>";
            }
            $(elementIDs.duationDDLAutoSaveJQ).append(options);
            getAutoSaveSettingsData();
        });
        promise.fail(showError);
    }
    function fillDropDownUnlockTimeOutDurations() {
        currentInstance = this;
        var options = "";
        
        for (i = 60; i < 240; i = i + 30) {
            options += "<option value=" + i + ">" + i + " min" + "</option>";
        }
        $(elementIDs.duationDDLUnlockJQ).append(options);
        getUnlockTimeoutSettingsData();
    }

    function setAutoSaveState() {
        //fill up the different duration levels for dropdown
        if ($(elementIDs.enableAutoSaveChkJQ).is(':checked')) {
            $(elementIDs.duationDDLAutoSaveJQ).removeAttr('disabled');
        }
        else {
            $(elementIDs.duationDDLAutoSaveJQ).attr('disabled', 'disabled');
            $(elementIDs.duationDDLAutoSaveJQ).val(0)
        }

        //check role access permissions.
        checkSettingsClaims(elementIDs, claims);
    }

    function saveUnLockTimeOutSettingData() {
            var viewModel = getUnlockViewModel();
            var promise = ajaxWrapper.postJSON(URLs.SaveUnLockTimeOutSetting, viewModel);
            //callback function for ajax request success.
            promise.done(function (xhr) {
                if (xhr.Result === ServiceResult.SUCCESS) {
                    messageDialog.show(DocumentDesign.saveMsg);
                }
                else {
                    messageDialog.show(Common.errorMsg);
                }
            });
        
    }


    function saveSettingsData() {
        if (validateSettings()) {
            var viewModel = getAutoSaveViewModel();
            var promise = ajaxWrapper.postJSON(URLs.saveAutoSaveSettings, viewModel);
            //callback function for ajax request success.
            promise.done(function (xhr) {
                if (xhr.Result === ServiceResult.SUCCESS) {
                    saveUnLockTimeOutSettingData();
                }
                else {
                    messageDialog.show(Common.errorMsg);
                }
            });
        }
    }
    function getUnlockViewModel() {
        var unlockSaveData = {
            unlockTimeOutInMin: $(elementIDs.duationDDLUnlockJQ).val(),
            TenantId: tenantId
        }
        return unlockSaveData;
    }

    function getAutoSaveViewModel() {
        var autoSaveData = {
            IsAutoSaveEnabled: $(elementIDs.enableAutoSaveChkJQ).is(":checked"),
            Duration: $(elementIDs.duationDDLAutoSaveJQ).val(),
            AccelerateStartDateForTask: $(elementIDs.accelerateStartDateForTask).is(":checked"),
            TenantID: tenantId
        }
        return autoSaveData;
    }

    function validateSettings() {
        var isValid = false;
        var isAutoSaveEnable = $(elementIDs.enableAutoSaveChkJQ).is(":checked");
        var autoSaveDuration = $(elementIDs.duationDDLAutoSaveJQ).val();
        if (isAutoSaveEnable == true && autoSaveDuration == "0") {
            $(elementIDs.duationDDLAutoSaveJQ).parent().addClass('has-error');
            $(elementIDs.enableAutoSaveChkHelpBlockJQ).text(AutoSave.autoSaveDurationRequiredMsg);
            isValid = false;
        }
        else {
            $(elementIDs.duationDDLAutoSaveJQ).parent().removeClass('has-error');
            $(elementIDs.enableAutoSaveChkHelpBlockJQ).text('');
            isValid = true;
        }
        return isValid;
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
        $(".WFCATLIST").unbind('click');
        $(".WFCATLIST").click(function () {
            loadCategoryListGrid();
            loadGroupListGrid();
            //show the selected tab
            $(elementIDs.settingsTabJQ).tabs({
                selected: 1
            });
        });
    }

    getLockDocumentList = function () {

        //set column list for grid
        var colArray = null;
        colArray = ['Folder', 'Product', 'View', 'Lock By User', 'UserId', 'Release'];

        //set column models
        var colModel = [];
        
        colModel.push({ key: false, name: 'foldername', index: 'foldername', editable: false });
        colModel.push({ key: false, name: 'FormInstanceName', index: 'FormInstanceName', editable: false });
        colModel.push({ key: false, name: 'ViewName', index: 'ViewName', align: 'left' });
        colModel.push({ key: false, name: 'lockByUserName', index: 'lockByUserName', editable: true, edittype: 'select', align: 'left' });
        colModel.push({ key: false, name: 'UserId', index: 'UserId',hidden:true, editable: true, edittype: 'select', align: 'left' });
        colModel.push({ key: false, name: 'IsLock', index: 'IsLock', align: 'left',editable: true,
        edittype: 'checkbox', editoptions: { value: "True:False" }, 
        formatter: "checkbox", formatoptions: { disabled: false}  });
        //clean up the grid first - only table element remains after this
        $(elementIDs.LockDocumentGrid).jqGrid('GridUnload');

        //adding the pager element
        $(elementIDs.LockDocumentGridJQ).parent().append("<div id='p" + elementIDs.LockDocumentGrid + "'></div>");

        $(elementIDs.LockDocumentGridJQ).jqGrid({
            url: URLs.getlockDocumentList,
            datatype: 'local',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: '',
            height: 380,
            rowNum: 20,
            rowList: [20, 40, 60],
            headertitles: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            //sortname: 'Foldername',
            //sortorder: 'desc',
            pager: '#p' + elementIDs.LockDocumentGrid,
        });

        var pagerElement = '#p' + elementIDs.LockDocumentGrid;
        
        $(elementIDs.LockDocumentGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.LockDocumentGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
       
        function showError(index, rowid) {
            var cm = $(elementIDs.LockDocumentGridJQ).jqGrid('getGridParam', 'colModel');
            if (cm[index].name == "StatusText") {
                var rowData = $(elementIDs.LockDocumentGridJQ).getRowData(rowid);
                if (rowData.StatusCode == "3") {
                    messageDialog.show(rowData.ErrorMessage);
                }
            }
        }

        function CheckBoxFormatter(cellValue, options, rowObject) {
            var result;
            switch (options.colModel.index) {
                case 'IsLock':
                    var resultId = "IsLock" + options.rowId;
                    if (rowObject.IsLock == true) {
                        result = "<input type='checkbox' id='" + resultId + "' checked='" + cellValue + "'/>";
                    }
                    else {
                        result = "<input type='checkbox' id='" + resultId + "' unchecked " + "'  disabled='true' />";
                    }
                    break;
                default:
                    result = cellValue;
                    break;
            }
            return result;
        }

        $(elementIDs.btnReleaseJQ).click(function () {
            var GridData = $("#LockDocumentGrid").jqGrid('getGridParam', 'data');
            alert("select Procuct to release");
        });

        function getSelectUserAction()
        {
            var s = 0;
        }
    }

    
}();


//contains functionality for the Consortium add/update dialog
var consortiumDialog = function () {
    var consortiumDialogState;
    var URLs = {
        //Add Consortium
        AddCategory: '/Settings/AddFolderVersionCategory?tenantID=1',
        //Update Consortium
        UpdateCategory: '/Settings/UpdateFolderVersionCategory?&tenantID=1',

        DeleteCategory: '/Settings/DeleteCategory?&tenantID=1',
        //Gets the Group List
        GroupList: '/Settings/GetFolderVersionGroupForDropdown?tenantId=1'
    }
    var elementIDs = {
        consortiumDialog: '#consortiumDialog',
        consortiumNameJQ: '#consortiumName',
        btnsaveConsortiumJQ: '#btnsaveConsortium',
        categoryListGridJQ: "#categoryListGrid",
        consortiumNameSpanJQ: '#consortiumNameSpan',

        groupNameSpanJQ: '#groupNameSpan',
        groupDDLJQ: '#groupDDL',
        groupNamesAutoCompltDDLJQ: '#groupNamesAutoCompltDDL'
    }

    //ajax success callback - for add/edit
    function consortiumSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            if (consortiumDialogState === "add") {
                messageDialog.show(FolderCategoryMessages.addCategoryMsg);
            }
            else {
                messageDialog.show(FolderCategoryMessages.updateCategoryMsg);
            }

            $(elementIDs.categoryListGridJQ).setGridParam({ datatype: 'json', page: 1 }).trigger('reloadGrid');;  //.trigger("reloadGrid");
            $(elementIDs.consortiumDialog).dialog('close');
        }
        else {

            if (consortiumDialogState === "add") {
                if (xhr.Items.length > 0) {
                    messageDialog.show(xhr.Items[0].Messages);
                }
                else {
                    messageDialog.show(FolderCategoryMessages.addCategoryErrorMsg);
                }
            }
            else if (consortiumDialogState === "update") {
                if (xhr.Items.length > 0) {
                    messageDialog.show(xhr.Items[0].Messages);
                }
                else {
                    messageDialog.show(FolderCategoryMessages.updateCategoryErrorMsg);
                }
            }
            else {
                //messageDialog.show(FolderCategoryMessages.updateCategoryErrorMsg);
            }
            $(elementIDs.consortiumNameJQ).parent().removeClass('has-error');
            $(elementIDs.consortiumNameJQ).removeClass('form-control');
            $(elementIDs.groupDDLJQ).parent().removeClass('has-error');
            $(elementIDs.groupDDLJQ).removeClass('form-control');
            $(elementIDs.consortiumNameSpanJQ).text('')
            $(elementIDs.groupNameSpanJQ).text('')
        }
    }
    function init() {
        //register dialog for grid row add/edit
        $(elementIDs.consortiumDialog).dialog({
            autoOpen: false,
            height: 180,
            width: 350,
            modal: true
        });

        //register event for save button click on dialog
        $(elementIDs.consortiumDialog + ' button').on('click', function () {
            var categoryName = $(elementIDs.consortiumNameJQ).val();
            var groupID = $(elementIDs.groupDDLJQ).val();
            if (categoryName == "") {
                $(elementIDs.consortiumNameJQ).parent().addClass('has-error');
                $(elementIDs.consortiumNameJQ).addClass('form-control');
                $(elementIDs.consortiumNameSpanJQ).text(FolderCategoryMessages.categoryNameRequiredMsg);
            }
            else if (groupID == "0" || groupID == null || groupID == undefined || $(elementIDs.groupNamesAutoCompltDDLJQ).val() == "") {
                $(elementIDs.groupNameSpanJQ).parent().addClass('has-error');
                $(elementIDs.groupDDLJQ).addClass('form-control');
                $(elementIDs.groupNameSpanJQ).text(FolderCategoryMessages.groupNameRequiredMsg);
            }
            else {
                var categoryData;
                if (consortiumDialogState == "add") {
                    categoryData = { folderVersionCategoryName: categoryName, folderVersionGroupID: groupID }
                    var url = URLs.AddCategory;
                }
                else {
                    var consortiumID = $(elementIDs.categoryListGridJQ).jqGrid('getGridParam', 'selrow');
                    categoryData = {
                        folderVersionCategoryID: consortiumID,
                        folderVersionCategoryName: categoryName,
                        folderVersionGroupID: groupID,
                    }
                    var url = URLs.UpdateCategory;
                }

                updatecategoryData(url, categoryData, consortiumDialogState)
            }
        });

        //Auto complete Start
        $(function () {
            $(elementIDs.groupDDLJQ).autoCompleteDropDown({ ID: 'groupNamesAutoCompltDDL' });
            $(elementIDs.groupDDLJQ).click(function () {
                $(elementIDs.groupDDLJQ).toggle();
            });
        });
    }


    function updatecategoryData(url, consortiumData, consortiumDialogState) {
        var promise = ajaxWrapper.postJSON(url, consortiumData);

        promise.done(function (xhr) {
            consortiumSuccess(xhr)
        });
    }
    init();

    //to fill drop down list of Group names
    function fillGroupDDL(selectedIndex) {
        var categoryName;
        var defaultGroupText = "-- Select Group --";
        //To refresh dropdown of Consortium names
        $(elementIDs.groupDDLJQ).empty();
        $(elementIDs.groupDDLJQ).append("<option value=0>" + defaultGroupText + "</option>");
        //ajax call for drop down list of account names
        var promise = ajaxWrapper.getJSON(URLs.GroupList);
        promise.done(function (names) {
            for (i = 0; i < names.length; i++) {
                $(elementIDs.groupDDLJQ).append("<option value=" + names[i].FolderVersionGroupID + ">" + names[i].FolderVersionGroupName + "</option>");

                if (selectedIndex == names[i].FolderVersionGroupID)
                    FolderVersionGroupName = names[i].FolderVersionGroupName;
            }
            $(elementIDs.groupDDLJQ).val(selectedIndex);
            if (selectedIndex > 0)
                $(elementIDs.groupNamesAutoCompltDDLJQ).val(FolderVersionGroupName);

        });
    }

    return {
        show: function (rowData, action) {
            $(elementIDs.consortiumDialog + ' input').val("");
            consortiumDialogState = action;

            $(elementIDs.consortiumNameSpanJQ).text('')
            $(elementIDs.groupNameSpanJQ).text('')

            $(elementIDs.consortiumNameJQ).parent().removeClass('has-error');
            $(elementIDs.consortiumNameJQ).removeClass('form-control');

            $(elementIDs.groupDDLJQ).parent().removeClass('has-error');
            $(elementIDs.groupDDLJQ).removeClass('form-control');

            if (consortiumDialogState == "add") {
                fillGroupDDL(0);
                $(elementIDs.consortiumDialog).dialog('option', 'title', 'Add Category');
            }
            else if (consortiumDialogState == "update") {

                //var consortiumID = $(elementIDs.categoryListGridJQ).jqGrid('getGridParam', 'selrow');
                //categoryData = {
                //    consortiumID: consortiumID,
                //    categoryName: categoryName,
                //}
                //var url = URLs.DeleteCategory;

                //updatecategoryData(url, consortiumData, consortiumDialogState)
                fillGroupDDL(rowData.FolderVersionGroupID);
                $(elementIDs.consortiumDialog).dialog('option', 'title', 'Update Category');
                $(elementIDs.consortiumNameJQ).val(rowData.FolderVersionCategoryName);
            }
            else {
                $(elementIDs.consortiumDialog).dialog('option', 'title', 'Delete Category');
                $(elementIDs.consortiumNameJQ).val(rowData.FolderVersionCategoryName);
            }
            //open the dialog - uses jqueryui dialog
            $(elementIDs.consortiumDialog).dialog("open");
        }
    }
}();

var groupDialog = function () {
    var groupDialogState;
    var URLs = {
        //Add SubCategory
        AddGroup: '/Settings/AddFolderVersionGroup?tenantID=1',
        //Update SubCategory
        UpdateGroup: '/Settings/UpdateFolderVersionGroup?&tenantID=1',

        DeleteGroup: '/Settings/DeleteFolderVersionGroup?&tenantID=1',
        //Gets the Group List
        GroupList: '/Settings/GetFolderVersionGroupForDropdown?tenantId=1'
    }
    var elementIDs = {
        groupDialog: '#groupDialog',
        groupNameJQ: '#groupName',
        btnsaveGroupJQ: '#btnsaveGroup',
        groupListGridJQ: "#groupListGrid",
        groupNameSpanJQ: "#groupNameSpanGrpDialog"
    }

    //ajax success callback - for add/edit
    function groupSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            if (groupDialogState === "add") {
                messageDialog.show(FolderCategoryMessages.addGroupMsg);
            }
            else {
                messageDialog.show(FolderCategoryMessages.updateGroupMsg);
            }
            $(elementIDs.groupListGridJQ).setGridParam({ datatype: 'json', page: 1 }).trigger('reloadGrid');;  //.trigger("reloadGrid");
            $(elementIDs.groupDialog).dialog('close');
        }
        else {

            if (groupDialogState === "add") {
                if (xhr.Items.length > 0) {
                    messageDialog.show(xhr.Items[0].Messages);
                    $(elementIDs.groupNameSpanJQ).text('')
                }
                else {
                    messageDialog.show(FolderCategoryMessages.addGroupErrorMsg);
                    $(elementIDs.groupNameSpanJQ).text('')
                }
            }
            else if (groupDialogState === "update") {
                if (xhr.Items.length > 0) {
                    messageDialog.show(xhr.Items[0].Messages);
                    $(elementIDs.groupNameSpanJQ).text('')
                }
                else {
                    messageDialog.show(FolderCategoryMessages.updateGroupErrorMsg);
                    $(elementIDs.groupNameSpanJQ).text('')
                }
            }
            else {
                //messageDialog.show(FolderCategoryMessages.updateSubCategoryErrorMsg);
            }

        }
    }
    function init() {
        // Subcategory dialog for grid row add/edit
        $(elementIDs.groupDialog).dialog({
            autoOpen: false,
            height: 150,
            width: 350,
            modal: true
        });

        //register event for save button click on dialog
        $(elementIDs.groupDialog + ' button').on('click', function () {
            var groupName = $(elementIDs.groupNameJQ).val();
            if (groupName == "") {
                $(elementIDs.groupNameJQ).parent().addClass('has-error');
                $(elementIDs.groupNameJQ).addClass('form-control');
                $(elementIDs.groupNameSpanJQ).text(FolderCategoryMessages.groupNameRequiredMsg);
            }
            else {
                var groupData;
                if (groupDialogState == "add") {
                    groupData = { folderVersionGroupName: groupName }
                    var url = URLs.AddGroup;
                }
                else if (groupDialogState == "delete") {
                    var groupID = $(elementIDs.groupListGridJQ).jqGrid('getGridParam', 'selrow');
                    groupData = {
                        folderVersionGroupID: groupID,
                        folderVersionGroupName: groupName
                    }
                    var url = URLs.DeleteGroup;
                }
                else {
                    var groupID = $(elementIDs.groupListGridJQ).jqGrid('getGridParam', 'selrow');
                    groupData = {
                        folderVersionGroupID: groupID,
                        folderVersionGroupName: groupName
                    }
                    var url = URLs.UpdateGroup;
                }
                updateGroupData(url, groupData, groupDialogState)
            }
        });
    }

    function updateGroupData(url, groupData, groupDialogState) {
        var promise = ajaxWrapper.postJSON(url, groupData);

        promise.done(function (xhr) {
            groupSuccess(xhr)
        });
    }
    init();

    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    //to fill drop down list of Category names
    function fillGroupDDL(selectedIndex) {
        var groupName;
        var defaultGroupText = "-- Select Group --";
        //To refresh dropdown of Consortium names
        $(elementIDs.groupNameJQ).empty();
        $(elementIDs.groupNameJQ).append("<option value=0>" + defaultGroupText + "</option>");
        //ajax call for drop down list of group names
        var promise = ajaxWrapper.getJSON(URLs.GroupList);
        promise.done(function (names) {
            for (i = 0; i < names.length; i++) {
                $(elementIDs.groupNameJQ).append("<option value=" + names[i].FolderVersionGroupID + ">" + names[i].FolderVersionGroupName + "</option>");

                if (selectedIndex == names[i].FolderVersionGroupID)
                    FolderVersionGroupName = names[i].FolderVersionGroupName;
            }
            $(elementIDs.groupNameJQ).val(selectedIndex);
            if (selectedIndex > 0)
                $(elementIDs.groupAutoCompltDDLJQ).val(FolderVersionGroupName);
        });
    }

    return {
        show: function (rowData, action) {
            $(elementIDs.groupDialog + ' input').val("");
            groupDialogState = action;

            $(elementIDs.groupNameSpanJQ).text('')
            $(elementIDs.groupNameJQ).parent().removeClass('has-error');
            $(elementIDs.groupNameJQ).removeClass('form-control');

            if (groupDialogState == "add") {
                //fillGroupDDL("0");
                $(elementIDs.groupDialog).dialog('option', 'title', 'Add Group');
            } else if (groupDialogState == "update") {
                //fillGroupDDL(rowData.FolderVersionGroupID);
                $(elementIDs.groupDialog).dialog('option', 'title', 'Update Group');
                $(elementIDs.groupNameJQ).val(rowData.FolderVersionGroupName);
            }
            //open the dialog - uses jqueryui dialog
            $(elementIDs.groupDialog).dialog("open");
        }
    }
}();




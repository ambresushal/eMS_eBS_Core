var SectionLockPopUp = function () {
    var isInitialized = false;
    var isOpenDocumentGridLoaded = false;
    var SuperuserId = 0;
    //urls to be accessed for create/copy form.
    var URLs = {
        getAlllockSectionDocumentList: '/DashBoard/GetSectionLockStatus',
        masterlisturl: "/FolderVersion/IndexML?tenantId=1&folderVersionId={folderVersionId}&folderId={folderId}",
        folderVersionList: '/FolderVersion/Index?tenantId=1&folderVersionId={folderVersionId}&folderId={folderId}&foldeViewMode={foldeViewMode}&mode={mode}',
        PostSectionReleaseData: '/DashBoard/ReleaseSectionListBySuperUser',
        GetListDataOfSuperUserData: '/Framework/IsSupperUser',
        PostUpdateNotifyuserList: '/DashBoard/UpdateNotifyuserList',
        getUserRole: '/DashBoard/GetIsSuperUser'
    };

    var elementIDs = {
        openDocumentDialogJQ: '#SectionLockNotificationPopUp',
        openDocumentDialog: 'openDocumentDialog1',
        openDocumentGrid: 'openDocumentGrid1',
        openDocumentGridJQ: '#SectionLockPopUpID',
        notifycheckbox: 'NotifyuserFlag',
        openDocumentSelectAllCheckbox: 'selectallcheckbox',
        openDocumentSelectAllCheckboxJQ: '#selectallcheckbox',
        NotifyMeCheckBoxCheckbox: 'NotifyMeCheckBox',
        NotifyMeCheckBoxAllCheckboxJQ1: '#NotifyMeCheckBox',
        openDocumentAllIsIncludeCheckboxJQ: '[id^=IsLocked_]',
        NotifyMeCheckBoxAllIsIncludeCheckboxJQ: '[id^=NotifyuserFlag_]'
    };
    //this function is called below soon after this JS file is loaded
    function init() {
        $(document).ready(function () {
            if (isInitialized == false) {
                $(elementIDs.openDocumentDialogJQ).dialog({ autoOpen: false, zIndex: 100007, width: 1100, height: 600, modal: true });
                isInitialized = true;
            }
        });
    }
    init();
    function loadopenDocumentGrid(isSuperUser) {

        var getAnchorDocumentListURL = URLs.getAlllockSectionDocumentList;
        var colArray;
        var colModel = [];
        var selectAllNotifymeCheckbox = '<input type="checkbox" class="check-all" id="NotifyMeCheckBox" />&nbsp;';
        var selectAllIsLockedCheckbox = '<input type="checkbox" class="check-all" id="selectallcheckbox" />&nbsp;';

        //set column list for grid   
        colArray = ['UserId', 'Folder Name', 'Folder Version', 'Document Name', 'Plan Name', 'View', 'Section', 'Locked By', selectAllNotifymeCheckbox + 'Notify Me', selectAllIsLockedCheckbox + 'Unlock', '', '', '', '', ''];
        //set column models
        colModel.push({ name: 'LockedBy', index: 'LockedBy', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left', hidden: false });
        colModel.push({ name: 'FolderversionNumber', index: 'FolderversionNumber', editable: false, align: 'left', hidden: false });
        colModel.push({ name: 'FormName', index: 'FormName', editable: false, align: 'left', hidden: false });
        colModel.push({ name: 'planName', index: 'planName', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'DisplayViewName', index: 'DisplayViewName', editable: false, align: 'left', width: 100, resizable: true, hidden: false  /*hidden: currentFolderInstance.folderViewMode == FolderViewMode.DefaultView ? true : false*/ });
        colModel.push({ name: 'DisplaySectionName', index: 'DisplaySectionName', editable: false, align: 'left', width: 140, resizable: true, hidden: false })
        colModel.push({ name: 'LockedUserName', index: 'LockedUserName', editable: false, align: 'left', width: 80, hidden: false });
        colModel.push({ name: 'NotifyuserFlag', index: 'NotifyuserFlag', sortable: false, editable: true, align: 'left', width: 85, hidden: false, formatter: formatterDesign, unformat: unFormatIncludedColumnDesign, });
        colModel.push({ name: 'IsLocked', index: 'IsLocked', sortable: false, editable: true, align: 'left', formatter: formatterDesign, unformat: unFormatIncludedColumnDesign, width: 85, resizable: true, hidden: !isSuperUser });
        colModel.push({ name: 'IsMasterList', index: 'IsMasterList', editable: true, align: 'left', hidden: true });
        colModel.push({ name: 'FormInstanceID', index: 'FormInstanceID', editable: true, align: 'left', hidden: true });
        colModel.push({ name: 'FolderID', index: 'FolderID', editable: true, align: 'left', hidden: true });
        colModel.push({ name: 'FolderVersionId', index: 'FolderVersionId', editable: true, align: 'left', hidden: true });
        colModel.push({ name: 'SectionName', index: 'SectionName', sortable: false, align: 'left', formatter: formatterDesign, unformat: unFormatIncludedColumnDesign, width: 70, resizable: true, hidden: true });
        //clean up the grid first - only table element remains after this
        $(elementIDs.openDocumentGridJQ).jqGrid('GridUnload');
        //adding the pager element
        $(elementIDs.openDocumentGridJQ).parent().append("<div id='p" + elementIDs.openDocumentGrid + "'></div>");
        $(elementIDs.openDocumentGridJQ).jqGrid({
            datatype: 'json',
            url: getAnchorDocumentListURL,//openDocumentDialog.URLs.getAnchorDocumentList,
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: '',
            pager: '#p' + elementIDs.openDocumentGrid,
            height: '200',
            rowheader: true,
            loadonce: true,
            rowNum: 10000,
            autowidth: true,
            gridview: true,
            viewrecords: true,
            altRows: true,
            altclass: 'alternate',
            ignoreCase: true,
            gridComplete: function () {
                initializeCheckAll();
                //For select all checkbox for open document
                $(elementIDs.openDocumentSelectAllCheckboxJQ).change(function () {
                    var val = $(this).is(':checked');
                    $(elementIDs.openDocumentAllIsIncludeCheckboxJQ).filter(':not(:disabled)').prop('checked', val);
                    setSelectAllCheckbox();
                });
                $(elementIDs.NotifyMeCheckBoxAllCheckboxJQ1).change(function () {
                    var val = $(this).is(':checked');
                    $(elementIDs.NotifyMeCheckBoxAllIsIncludeCheckboxJQ).filter(':not(:disabled)').prop('checked', val);
                    setSelectAllNotifyMeCheckBox();
                });
                $(elementIDs.openDocumentAllIsIncludeCheckboxJQ).change(function () {
                    setSelectAllCheckbox();
                });
                $(elementIDs.NotifyMeCheckBoxAllIsIncludeCheckboxJQ).change(function () {
                    var val = $(this).is(':checked');
                    var row = $(this)[0].id.split('_');
                    if(row != undefined && row.length > 0)
                    {
                        var rowId = row[1];
                        var data = $(elementIDs.openDocumentGridJQ).jqGrid('getGridParam', 'data');
                        data[rowId - 1]['NotifyuserFlag'] = val;
                    }
                    setSelectAllNotifyMeCheckBox();
                });
            }

        });
        jQuery("tr.ui-jqgrid-labels th:eq(0)").attr("title", "Select").tooltip({ container: 'body' });
        var pagerElement = '#p' + elementIDs.openDocumentGrid;
        //remove default buttons
        $(elementIDs.openDocumentGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false, refresh: false });
        //remove paging              
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        $(elementIDs.openDocumentGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
        $(elementIDs.openDocumentGridJQ).jqGrid('navButtonAdd', pagerElement,
             {
                 caption: '',
                 buttonicon: 'ui-icon-refresh',
                 onClickButton: function () {
                     $(elementIDs.openDocumentGridJQ).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
                 }
             });
        $(elementIDs.openDocumentGridJQ).jqGrid('navButtonAdd', pagerElement,
              {
                  caption: 'Open to View Only',
                  onClickButton: function () {
                      OpenSelectedInstance();
                  }
              });
        $(elementIDs.openDocumentGridJQ).jqGrid('navButtonAdd', pagerElement,
              {
                  caption: 'Notify Me',
                  onClickButton: function () {
                      var NotifyUserListgetRowData = $(elementIDs.openDocumentGridJQ).getRowData();
                      var NotifyUser = new Array();
                      NotifyUser = [];
                      NotifyUserList = NotifyUserListgetRowData.filter(function (obj) {
                          if (obj.NotifyuserFlag == true || obj.NotifyuserFlag == "true")
                              return obj;
                      })
                      if (NotifyUserListgetRowData == null || NotifyUserListgetRowData == undefined || NotifyUserListgetRowData == "" || NotifyUserListgetRowData.length == 0) {
                          messageDialog.show(SectionLockpopUpCommonMessage.SelectRowCheckbox);
                      }
                      else {
                          var resourceLockModel = []
                          NotifyUserList.forEach(function (element) {
                              var resourceLockNotifyUser =
                              {
                                  FormInstanceID: element.FormInstanceID,
                                  SectionName: element.SectionName
                              };
                              resourceLockModel.push(resourceLockNotifyUser);
                          });
                          var promise = ajaxWrapper.postJSONCustom(URLs.PostUpdateNotifyuserList, resourceLockModel);
                          promise.done(function (Result) {
                              messageDialog.show(SectionLockpopUpCommonMessage.NotifyDoneMessage);
                          });

                      }
                  }
              });
        if (isSuperUser) {
            $(elementIDs.openDocumentGridJQ).jqGrid('navButtonAdd', pagerElement,
                      {
                          caption: 'Unlock',
                          id: "unlockBySuperUser",
                          onClickButton: function () {
                              var openDocumentList = $(elementIDs.openDocumentGridJQ).getRowData();
                              openDocumentList = openDocumentList.filter(function (obj) {
                                  if (obj.IsLocked == true || obj.IsLocked == "true")
                                      return obj;
                              });

                              if (openDocumentList == null || openDocumentList == undefined || openDocumentList == "" || openDocumentList.length == 0) {
                                  messageDialog.show(SectionLockpopUpCommonMessage.SelectRowCheckbox);
                              }
                              else {
                                  var resourceLockModel = []

                                  var userList = [];

                                  openDocumentList.forEach(function (element) {
                                      var resourceLockNotifyUser = {
                                          FormInstanceID: element.FormInstanceID,
                                          SectionName: element.SectionName
                                      };
                                      resourceLockModel.push(resourceLockNotifyUser);

                                      var notificationData = {
                                          toUser: element.LockedUserName,
                                          viewName: element.DisplayViewName,
                                          sectionName: element.DisplaySectionName
                                      };

                                      userList.push(notificationData);
                                  });
                                  $.connection.hub.start().done(function () {
                                      console.log("Hub Connected!");
                                      $.connection.notificationHub.server.sendDocumentOverriddenBySuperUserMessage(userList);

                                  })

                                  var promise = ajaxWrapper.postJSONCustom(URLs.PostSectionReleaseData, resourceLockModel);

                                  promise.done(function (Result) {
                                      if (Result == true) {
                                          $(elementIDs.openDocumentGridJQ).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');

                                      }

                                  });
                              }
                          }
                      })
        }
        function formatterDesign(cellValue, options, rowObject) {
            var result = cellValue;
            switch (options.colModel.index) {
                case "NotifyuserFlag":
                    if (rowObject.NotifyuserFlag == true || rowObject.NotifyuserFlag == "true") {
                        result = "<input type='checkbox' id='NotifyuserFlag_" + options.rowId + "' name= 'NotifyuserFlag_" + rowObject.NotifyuserFlag + "' checked=\'checked\'/>";
                    }
                    else {
                        result = "<input type='checkbox' id='NotifyuserFlag_" + options.rowId + "' name= 'NotifyuserFlag_" + rowObject.NotifyuserFlag + "' />";
                    }

                    break;
                case "IsLocked":
                    //var isSuperUser = false;
                    //if (currentFolderInstance.folderData.RoleId == Role.TMGSuperUser || currentFolderInstance.folderData.RoleId == Role.WCSuperUser)
                    //    isSuperUser = true;
                    //if (rowObject.IsDocumentLocked == true && currentFolderInstance.isEditable == true && isSuperUser == true) // Override option will be available only for SuperUser
                    result = "<input type='checkbox' id='IsLocked_" + options.rowId + "' name= 'IsLocked_" + rowObject.IsLocked + "' />";
                    //else
                    //    result = "<input type='checkbox' id='IsLocked_" + options.rowId + "' name= 'IsLocked_" + rowObject.IsLocked + "' disabled='disabled'/>";
                    break;
            }
            return result;

        }
        //unformat the grid column based on element property
        function unFormatIncludedColumnDesign(cellValue, options, rowObject) {
            var result;

            switch (options.colModel.index) {
                case "NotifyuserFlag":
                    result = $(this).find('#NotifyuserFlag_' + options.rowId).prop('checked');
                    break;

                case "IsLocked":
                    result = $(this).find('#IsLocked_' + options.rowId).prop('checked');
            }

            return result;
        }

        function initializeCheckAll() {
            $(elementIDs.openDocumentSelectAllCheckboxJQ).closest(".ui-jqgrid-sortable").removeClass("ui-jqgrid-sortable");
            $(elementIDs.NotifyMeCheckBoxAllCheckboxJQ1).closest(".ui-jqgrid-sortable").removeClass("ui-jqgrid-sortable");
            //setSelectAllCheckbox();
            //setSelectAllNotifyMeCheckBox();
        }

        function setSelectAllCheckbox() {
            if ($(elementIDs.openDocumentAllIsIncludeCheckboxJQ).filter(':not(:checked):not(:disabled)').length > 0)
                $(elementIDs.openDocumentSelectAllCheckboxJQ).prop('checked', false);
            else
                $(elementIDs.openDocumentSelectAllCheckboxJQ).prop('checked', true);


        }

        function setSelectAllNotifyMeCheckBox() {
            if ($(elementIDs.NotifyMeCheckBoxAllIsIncludeCheckboxJQ).filter(':not(:checked):not(:disabled)').length > 0)
                $(elementIDs.NotifyMeCheckBoxAllCheckboxJQ1).prop('checked', false);
            else
                $(elementIDs.NotifyMeCheckBoxAllCheckboxJQ1).prop('checked', true);


        }

        function OpenSelectedInstance() {
            var rowId = $(elementIDs.openDocumentGridJQ).getGridParam('selrow');
            if (rowId == null) {
                messageDialog.show(SectionLockpopUpCommonMessage.OpenDocumentInViewOnlyMode);
                return;
            }
            var row = $(elementIDs.openDocumentGridJQ).getRowData(rowId);
            var formInstanceId = row.FormInstanceID;
            var displaySectionName = row.SectionName;
            var isMasterlist = row.IsMasterList;
            var folderVesrionId = row.FolderVersionId;
            var folderId = row.FolderID;

            if (isMasterlist == 'true') {
                var url = URLs.masterlisturl;// "/FolderVersion/IndexML?tenantId=1&folderVersionId={folderVersionId}&folderId={folderId}";
                url = url.replace('{folderVersionId}', folderVesrionId);
                url = url.replace('{folderId}', folderId);
                window.location.href = url;
                return;
            } else {
                var url = URLs.folderVersionList;  //'/FolderVersion/Index?tenantId=1&folderVersionId={folderVersionId}&folderId={folderId}&foldeViewMode={foldeViewMode}&mode={mode}';
                url = url.replace('{folderVersionId}', folderVesrionId);
                url = url.replace('{folderId}', folderId);
                url = url.replace('{foldeViewMode}', "Default");
                url = url.replace('{mode}', true);
                window.location.href = url + "&lockNavInfo=" + formInstanceId + "_" + displaySectionName;
                return;
            }
        }
    }
    //format the grid column based on element property
    return {
        SectionLockshowNotification: function () {
            //refresh form name 
            $(elementIDs.openDocumentDialogJQ).dialog('option', 'title', 'Document Lock Status');
            
            $(elementIDs.openDocumentDialogJQ).dialog('open');
            var promise = ajaxWrapper.getJSON(URLs.getUserRole);
            //callback function for ajax request success.
            promise.done(function (xhr) {
                var isSuperUser = xhr;
                loadopenDocumentGrid(isSuperUser);
                $(elementIDs.openDocumentDialogJQ).dialog({
                    position: {
                        my: 'center',
                        at: 'center'
                    },
                });
            });

        }
    }
}();


var userManagementSettings = function () {

    var URLs = {
        userList: '/Settings/GetUsersDetails?tenantId=1',
        createuser: '/Settings/CreateUser',
        userRoleList: '/Settings/GetUserRolesDetails',
        exportToExcel: '/Settings/ExportToExcel',
        resetUserPassword: '/Settings/ResetPassword',
        updateRole: '/Settings/UpdateRole',
        deleteUser: '/Settings/DeleteUser',
        sendMailNotification: '/Settings/SendMailNotification',
        activateUser: '/Settings/ActivateUser?userName={userName}',
        unlockUser: '/Settings/UnlockUser'
    }
    var elementIDs = {

        userListGrid: 'userListGrid',
        userListGridJQ: '#userListGrid',
        userdialog: '#userdialog',
        changeRoledialog: '#changeRoledialog',
        usernameJQ: '#txtusername',
        userroleJQ: '#userRoleName',
        emailJQ: '#email',
        firstNameJQ: '#firstName',
        lastNameJQ: '#lastName',
        cnuserroleJQ: '#cnuserrole',
        lusernameJQ: '#lusername',
        lcuserroleJQ: '#lcuserrole',
        userIdForRole: '#userIdForRole',
        userNameErrorJQ: '#userNameError',
        userRoleErrorJQ: '#userRoleError',
        userEmailErrorJQ: '#userEmailError',
        userFirstNameErrorJQ: '#userFirstNameError',
        userLastNameErrorJQ: '#userLastNameError',
        newUserRoleErrorjQ: '#newUserRoleError'
    }

    function intializeControls() {
        $(elementIDs.userdialog + ' input').val("");
        $(elementIDs.userNameErrorJQ).text('');
        $(elementIDs.userRoleErrorJQ).text('');
        $(elementIDs.userEmailErrorJQ).text('');
        $(elementIDs.userFirstNameErrorJQ).text('');
        $(elementIDs.userLastNameErrorJQ).text('');
        $(elementIDs.usernameJQ).parent().removeClass('has-error');
        $(elementIDs.userroleJQ).parent().removeClass('has-error');
        $(elementIDs.emailJQ).parent().removeClass('has-error');
        $(elementIDs.firstNameJQ).parent().removeClass('has-error');
        $(elementIDs.lastNameJQ).parent().removeClass('has-error');

    }

    function init() {
        $(document).ready(function () {
            loadUserMangementGrid();

            intializeControls();
            //register dialog for grid row add/edit
            $(elementIDs.userdialog).dialog({
                autoOpen: false, height: 350, width: 450, modal: true, close: function (e) {
                    $(elementIDs.userdialog + ' input').val("");
                    $(elementIDs.userNameErrorJQ).text('');
                    $(elementIDs.usernameJQ).parent().removeClass('has-error');
                    $(elementIDs.userNameErrorJQ).parent().removeClass('has-error');
                    $(elementIDs.userEmailErrorJQ).text('');
                    $(elementIDs.emailJQ).parent().removeClass('has-error');
                    $(elementIDs.userEmailErrorJQ).parent().removeClass('has-error');
                    $(elementIDs.userFirstNameErrorJQ).text('');
                    $(elementIDs.userFirstNameErrorJQ).parent().removeClass('has-error');
                    $(elementIDs.firstNameJQ).parent().removeClass('has-error');
                    $(elementIDs.userLastNameErrorJQ).text('');
                    $(elementIDs.userLastNameErrorJQ).parent().removeClass('has-error');
                    $(elementIDs.lastNameJQ).parent().removeClass('has-error');
                    $(elementIDs.userRoleErrorJQ).text('');
                    $(elementIDs.userRoleErrorJQ).parent().removeClass('has-error');
                    $(elementIDs.userroleJQ).parent().removeClass('has-error');
                }
            });
            $(elementIDs.deleteUserdialog).dialog({ autoOpen: false, height: 250, width: 450, modal: true });
            $(elementIDs.changeRoledialog).dialog({
                autoOpen: false, height: 250, width: 450, modal: true,
                close: function (e) {
                    $(elementIDs.newUserRoleErrorjQ).text('');
                    $(elementIDs.cnuserroleJQ).parent().removeClass('has-error');
                    $(elementIDs.newUserRoleErrorjQ).parent().removeClass('has-error');
                }
            });
            $(elementIDs.resetPassworddialog).dialog({ autoOpen: false, height: 250, width: 450, modal: true });

            $(elementIDs.changeRoledialog + ' button').on('click', function () {

                userName = $(elementIDs.lusernameJQ).text();
                userRole = $(elementIDs.lcuserroleJQ).text();
                userId = $(elementIDs.userIdForRole).text();
                newUserRole = $(elementIDs.cnuserroleJQ + " :selected").text();

                var valiadteData = false;

                var userRoleData = {
                    userName: userName,
                    userRole: userRole,
                    newUserRole: newUserRole,
                    userId: userId
                }
                if (newUserRole == 'Select User Role') {
                    $(elementIDs.cnuserroleJQ).parent().addClass('has-error');
                    $(elementIDs.newUserRoleErrorjQ).text(UserManagementMessages.userRoleRequiredMsg);
                    $(elementIDs.newUserRoleErrorjQ).parent().addClass('has-error');
                    valiadteData = false;
                }
                else {
                    $(elementIDs.cnuserroleJQ).removeClass('form-control');
                    $(elementIDs.newUserRoleErrorjQ).text('')
                    valiadteData = true;
                }

                if (valiadteData == true) {
                    url = URLs.updateRole;

                    //ajax call to update User Role
                    var promise = ajaxWrapper.postJSON(url, userRoleData);
                    //register ajax success callback
                    promise.done(function (result) {
                        if (result.Result == 0) {
                            messageDialog.show(UserManagementMessages.changeRoleSucessMsg);
                            loadUserMangementGrid();
                        }
                        else {
                            messageDialog.show(UserManagementMessages.changeRoleErrorMsg);
                        }
                    });

                    $(elementIDs.changeRoledialog).dialog('close');
                    //register ajax failure callback
                    promise.fail(showError);
                }
            });

            $(elementIDs.userdialog + ' button').on('click', function () {
                userName = $(elementIDs.usernameJQ).val();
                userRole = $(elementIDs.userroleJQ + " :selected").text();
                email = $(elementIDs.emailJQ).val();
                firstName = $(elementIDs.firstNameJQ).val();
                lastName = $(elementIDs.lastNameJQ).val();

                var userData = {
                    userName: userName,
                    userRole: userRole,
                    email: email,
                    firstName: firstName,
                    lastName: lastName
                }
                var validatedUserData = validateControls(userData);
                if (validatedUserData == true) {
                    url = URLs.createuser;

                    // ajax call to add User
                    var promise = ajaxWrapper.postJSON(url, userData);
                    // register ajax success callback
                    promise.done(function (result) {
                        if (result.Result == 0) {
                            messageDialog.show(UserManagementMessages.createUserSucessMsg);
                            loadUserMangementGrid();
                            $(elementIDs.userdialog).dialog('close');
                        }
                        else {

                            if (result.Items[0].Messages[0] == "User already Exist") {
                                $(elementIDs.userdialog).dialog('close');
                                confirmDialog.show((UserManagementMessages.userActivateConfirmationMsg).replace(/\{#UserName}/g, userName), function () {
                                    // $(elementIDs.userdialog).dialog('close');
                                    confirmDialog.hide();
                                    url = URLs.activateUser.replace(/\{userName\}/g, userName);
                                    var promise = ajaxWrapper.postJSON(url);
                                    //register ajax success callback
                                    promise.done(function (xhr) {
                                        if (xhr.Result === ServiceResult.SUCCESS) {
                                            messageDialog.show(UserManagementMessages.userActivateSuccessMsg);
                                            loadUserMangementGrid();
                                        }
                                        else if (xhr.Result === ServiceResult.WARNING) {
                                            window.location.href = '/Account/LogOn';
                                        }
                                    });
                                });
                            }
                            else if (result.Items[0].Messages[0] == "Email Already Exist") {
                                $(elementIDs.emailJQ).parent().addClass('has-error');
                                $(elementIDs.userEmailErrorJQ).text(UserManagementMessages.emailAlreadyExistMsg);
                                $(elementIDs.userEmailErrorJQ).parent().addClass('has-error');
                            }

                            else {
                                messageDialog.show(UserManagementMessages.createUserErrorMsg);
                            }
                        }
                    });

                    //  register ajax failure callback
                    promise.fail(showError);
                }
            });
        });
    }

    function fillUserRoleDropDown() {
        $(elementIDs.userroleJQ).empty();
        $(elementIDs.userroleJQ).append("<option value='0'>" + "Select User Role" + "</option>");

        var promise = ajaxWrapper.getJSON(URLs.userRoleList);
        promise.done(function (list) {
            for (i = 0; i < list.length; i++) {
                $(elementIDs.userroleJQ).append("<option value=" + list[i]['UserRole'] + ">" + list[i]['UserRole'] + "</option>");
            }
        });
        promise.fail(showError);

    }

    function filNewUserRoleDropDown() {
        $(elementIDs.cnuserroleJQ).empty();
        $(elementIDs.cnuserroleJQ).append("<option value='0'>" + "Select User Role" + "</option>");
        var promise = ajaxWrapper.getJSON(URLs.userRoleList);
        promise.done(function (list) {
            for (i = 0; i < list.length; i++) {
                $(elementIDs.cnuserroleJQ).append("<option value=" + list[i]['UserRole'] + ">" + list[i]['UserRole'] + "</option>");
            }
        });
        promise.fail(showError);
    }

    function validateUserName(userName) {
        if (userName == '') {
            $(elementIDs.usernameJQ).parent().addClass('has-error');
            $(elementIDs.userNameErrorJQ).text(UserManagementMessages.userNameRequiredMsg);
            $(elementIDs.userNameErrorJQ).parent().addClass('has-error');

            validUserDetails = false;
        }
        else {
            $(elementIDs.usernameJQ).parent().removeClass('has-error');
            $(elementIDs.usernameJQ).removeClass('form-control');
            $(elementIDs.userNameErrorJQ).text('')
            validUserDetails = true;
        }
        return validUserDetails;
    }
    function validateUserRole(userRole) {

        if (userRole == 'Select User Role') {
            $(elementIDs.userroleJQ).parent().addClass('has-error');
            $(elementIDs.userRoleErrorJQ).text(UserManagementMessages.userRoleRequiredMsg);
            $(elementIDs.userRoleErrorJQ).parent().addClass('has-error');
            validUserDetails = false;
        }
        else {
            $(elementIDs.userroleJQ).parent().removeClass('has-error');
            $(elementIDs.userroleJQ).removeClass('form-control');
            $(elementIDs.userRoleErrorJQ).text('')
            validUserDetails = true;
        }
        return validUserDetails;
    }

    function validateEmail(email) {
        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        if (email == '') {
            $(elementIDs.emailJQ).parent().addClass('has-error');
            $(elementIDs.userEmailErrorJQ).text(UserManagementMessages.emailRequiredMsg);
            $(elementIDs.userEmailErrorJQ).parent().addClass('has-error');

            validUserDetails = false;
        }
        else if (!(re.test(email))) {
            $(elementIDs.emailJQ).parent().addClass('has-error');
            $(elementIDs.userEmailErrorJQ).text(UserManagementMessages.emailFormatMsg);
            $(elementIDs.userEmailErrorJQ).parent().addClass('has-error');

            validUserDetails = false;
        }
        else {
            $(elementIDs.emailJQ).parent().removeClass('has-error');
            $(elementIDs.emailJQ).removeClass('form-control');
            $(elementIDs.userEmailErrorJQ).text('');
            validUserDetails = true;
        }
        return validUserDetails;
    }

    function validateFirstName(firstName) {
        if (firstName == '') {
            $(elementIDs.firstNameJQ).parent().addClass('has-error');
            $(elementIDs.userFirstNameErrorJQ).text(UserManagementMessages.userFirstNameRequiredMsg);
            $(elementIDs.userFirstNameErrorJQ).parent().addClass('has-error');
            validUserDetails = false;
        }
        else {
            $(elementIDs.firstNameJQ).parent().removeClass('has-error');
            $(elementIDs.firstNameJQ).removeClass('form-control');
            $(elementIDs.userFirstNameErrorJQ).text('');
            validUserDetails = true;
        }
        return validUserDetails;
    }

    function validateLastName(lastName) {

        if (lastName == '') {
            $(elementIDs.lastNameJQ).parent().addClass('has-error');
            $(elementIDs.userLastNameErrorJQ).parent().addClass('has-error');
            $(elementIDs.userLastNameErrorJQ).text(UserManagementMessages.userLastNameRequiredMsg);
            $(elementIDs.userLastNameErrorJQ).parent().addClass('has-error');

            validUserDetails = false;
        }
        else {

            $(elementIDs.lastNameJQ).parent().removeClass('has-error');
            $(elementIDs.lastNameJQ).removeClass('form-control');
            $(elementIDs.userLastNameErrorJQ).text('');
            validUserDetails = true;
        }
        return validUserDetails;
    }

    function validateControls(userData) {
        validUserDetails = true;
        if (validateUserName(userData.userName) != true || validateUserRole(userData.userRole) != true || validateEmail(userData.email) != true || validateFirstName(userData.firstName) != true || validateLastName(userData.lastName) != true) {
            validUserDetails = false;
        }

        return validUserDetails;
    }

    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    function loadUserMangementGrid() {


        var j = 1;
        url = URLs.userList;
        var promise = ajaxWrapper.getJSON(url);
        // register ajax success callback
        promise.done(function (ListData) {
            var userData = [];
            $.each(ListData, function (idx, data) {
                data["ID"] = j++;
                userData.push(data);
            });


            var colArray = ['ID', 'Name', 'User Name', 'User Role', 'Updated By', 'Email', 'Last Updated', 'Send Email Notification', 'UserID', 'RoleId', 'IsCurrentUser'];//'Send Email Notification'
            var colModel = [];
            colModel.push({ name: 'ID', index: 'ID', hidden: true, key: true });
            colModel.push({ name: 'Name', index: 'Name', hidden: true });
            colModel.push({ name: 'UserName', index: 'UserName', editable: false, width: '200', align: 'left' });
            colModel.push({ name: 'UserRole', index: 'UserRole', editable: false, width: '200', align: 'left' });
            colModel.push({ name: 'UpdatedBy', index: 'UpdatedBy', editable: false, width: '200', align: 'left' });
            colModel.push({ name: 'Email', index: 'Email', editable: false, width: '200', hidden: true });
            colModel.push({ name: 'UpdatedDate', index: 'UpdatedDate', editable: false, width: '200', formatter: 'date', formatoptions: JQGridSettings.DateTimeFormatterOptions, align: 'center' });
            colModel.push({
                name: 'SendEmailNotification', index: 'SendEmailNotification', editable: false, width: '200', align: 'center', formatter: checkBoxFormatter, unformat: unFormat,
                edittype: "checkbox", editoptions: { value: "true:false", defaultValue: "false" }, sorttype: function (value, item) {
                    return item.SendEmailNotification == "Yes" ? true : false;
                }
            });
            colModel.push({ name: 'UserId', index: 'UserId', hidden: true });
            colModel.push({ name: 'RoleId', index: 'RoleId', hidden: true });
            colModel.push({ name: 'IsCurrentUser', index: 'IsCurrentUser', hidden: true });

            $(elementIDs.userListGridJQ).jqGrid('GridUnload');
            $(elementIDs.userListGridJQ).parent().append("<div id='p" + elementIDs.userListGrid + "'></div>");
            $(elementIDs.userListGridJQ).jqGrid({
                url: URLs.userList,
                datatype: 'local',
                data: userData,
                cache: false,
                altclass: 'alternate',
                colNames: colArray,
                colModel: colModel,
                caption: '',
                height: '320',
                rowNum: 13,
                loadonce: true,
                autowidth: true,
                viewrecords: true,
                hidegrid: false,
                hiddengrid: false,
                sortname: 'UserName',
                ignoreCase: true,
                altRows: true,
                pager: '#p' + elementIDs.userListGrid,
                loadComplete: function () {
                    //if (roleId == Role.HNESuperUser) {
                    //    $("#btnManageUserAdd").addClass('disabled-button');
                    //    $("#btnManageUserDelete").addClass('disabled-button');
                    //    $("#btnManageUserRole").addClass('disabled-button');
                    //    $("#btnResetPassword").addClass('disabled-button');
                    //    $("#btnManageMail").addClass('disabled-button');
                    //}
                    var p = this.p, data = p.data, item, $this = $(this), index = p._index, rowid;
                    for (rowid in index) {
                        if (index.hasOwnProperty(rowid)) {
                            item = data[index[rowid]];
                            if (item.SendEmailNotification == "false" || item.SendEmailNotification == "false" || item.SendEmailNotification == "true" || item.SendEmailNotification == false || item.SendEmailNotification == true) {
                                $this.jqGrid('setSelection', rowid, false);
                            }
                        }
                    }
                },
            });

            var pagerElement = '#p' + elementIDs.userListGrid;
            //remove default buttons
            $(elementIDs.userListGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

            $(elementIDs.userListGridJQ).jqGrid('filterToolbar', {
                stringResult: true, searchOnEnter: true,
                defaultSearch: "cn",
            });
            $(elementIDs.userListGridJQ).jqGrid('navButtonAdd', pagerElement,
         {
             caption: '', buttonicon: 'ui-icon-arrowstop-1-s', title: 'Download User Details', id: 'btnUserDetailsExportToExcel',

             onClickButton: function () {
                 var currentInstance = this;
                 var jqGridtoCsv = new JQGridtoCsv(elementIDs.userListGridJQ, false, currentInstance);
                 jqGridtoCsv.buildExportOptions();
                 var stringData = "csv=" + jqGridtoCsv.csvData;
                 stringData += "<&isGroupHeader=" + jqGridtoCsv.isGroupHeader;
                 stringData += "<&noOfColInGroup=" + jqGridtoCsv.noofGroupedColumns;
                 stringData += "<&repeaterName=" + elementIDs.userListGrid;

                 $.download(URLs.exportToExcel, stringData, 'post');
             }
         });

            $(elementIDs.userListGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon-plus', title: 'AddNewUser', id: 'btnManageUserAdd',
                onClickButton: function () {

                    $(elementIDs.userdialog).dialog('option', 'title', 'Add New User');
                    $(elementIDs.userdialog).dialog("open");

                    fillUserRoleDropDown();
                }
            });

            $(elementIDs.userListGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-trash', title: 'DeleteUser', id: 'btnManageUserDelete',
               onClickButton: function () {

                   var rowId = $(this).getGridParam('selrow');
                   var rowData = $(elementIDs.userListGridJQ).getRowData(rowId);
                   if (rowId == undefined && rowId == null) {
                       messageDialog.show(UserManagementMessages.userNameSelectMsg);
                   }
                   else if (rowData.IsCurrentUser == "true") {
                       messageDialog.show("Current logged in user is not allowed to delete");
                   }
                   else {
                       //load confirm dialogue to asset the operation
                       confirmDialog.show((UserManagementMessages.deleteConfirmationuserMsg).replace(/\{#userName}/g, rowData.UserName), function () {
                           confirmDialog.hide();

                           var deleteUser = {
                               userName: rowData.UserName,
                               userId: rowData.UserId,
                           };
                           url = URLs.deleteUser;
                           var promise = ajaxWrapper.postJSON(url, deleteUser);
                           // register ajax success callback
                           promise.done(function (xhr) {
                               if (xhr.Result === ServiceResult.SUCCESS) {
                                   messageDialog.show(UserManagementMessages.deleteUserSucessMsg);
                                   loadUserMangementGrid();
                               }
                           });
                       });
                   }
               }
           });

            $(elementIDs.userListGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: ' ui-icon-person', title: 'ChangeRole', id: 'btnManageUserRole',
               onClickButton: function () {

                   var rowId = $(this).getGridParam('selrow');

                   if (rowId !== undefined && rowId == null) {
                       messageDialog.show(UserManagementMessages.userNameSelectMsg);
                   }
                   else {
                       var rowData = $(elementIDs.userListGridJQ).getRowData(rowId);
                       $(elementIDs.lusernameJQ).text(rowData.UserName);
                       $(elementIDs.lcuserroleJQ).text(rowData.UserRole);
                       $(elementIDs.userIdForRole).text(rowData.UserId);


                       $(elementIDs.changeRoledialog).dialog('option', 'title', 'Change Role');
                       $(elementIDs.changeRoledialog).dialog("open");

                       filNewUserRoleDropDown();
                   }
               }
           });
            $(elementIDs.userListGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-key', title: 'ResetPassword', id: 'btnResetPassword',
               onClickButton: function () {

                   var rowId = $(this).getGridParam('selrow');
                   var rowData = $(elementIDs.userListGridJQ).getRowData(rowId);
                   var defaultPassword = ResetPassword.DefaultPassword;
                   if (rowId == undefined && rowId == null) {
                       messageDialog.show(UserManagementMessages.userNameSelectMsg);

                   }
                   else {
                       //load confirm dialogue to asset the operation
                       confirmDialog.show((UserManagementMessages.resetpasswordConfirmationMsg).replace(/\{#userName}/g, rowData.UserName), function () {
                           confirmDialog.hide();

                           var resetPasswordData = {
                               userID: rowData.UserId,
                               userName: rowData.UserName,
                               password: defaultPassword
                           };
                           url = URLs.resetUserPassword;
                           var promise = ajaxWrapper.postJSON(url, resetPasswordData);
                           //register ajax success callback
                           promise.done(function (xhr) {
                               if (xhr.Result === ServiceResult.SUCCESS) {
                                   loadUserMangementGrid();
                                   messageDialog.show(UserManagementMessages.resetPasswordSuccessMsg);
                               }
                               else if (xhr.Result === ServiceResult.WARNING) {
                                   window.location.href = '/Account/LogOn';
                               }
                           });

                       });
                   }
               }
           });

            $(elementIDs.userListGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: ' ui-icon-unlocked', title: 'Unlock User', id: 'btnUnlockUser',
               onClickButton: function () {

                   var rowId = $(this).getGridParam('selrow');

                   if (rowId !== undefined && rowId == null) {
                       messageDialog.show(UserManagementMessages.userNameSelectMsg);
                   }
                   else {
                       var rowData = $(elementIDs.userListGridJQ).getRowData(rowId);
                       var userInfo = {
                           userName: rowData.UserName,
                           userId: rowData.UserId,
                       };
                       url = URLs.unlockUser;
                       var promise = ajaxWrapper.postJSON(url, userInfo);
                       // register ajax success callback
                       promise.done(function (xhr) {
                           if (xhr.Result === ServiceResult.SUCCESS) {
                               messageDialog.show(UserManagementMessages.unlockUserSucessMsg);
                           }
                       });
                   }
               }
           });

            $(elementIDs.userListGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: 'SendMail', title: 'SendMail', id: 'btnManageMail',
               onClickButton: function () {
                   var allRowData = $(elementIDs.userListGridJQ).jqGrid('getGridParam', 'data');
                   var filterList = allRowData.filter(function (ct) {
                       if (ct.SendEmailNotification == 'Yes')
                           return ct.UserName;
                   });

                   if (filterList.length == 0) {
                       messageDialog.show(UserManagementMessages.sentEmailNotifcationrequiredMsg);
                   }
                   else {
                       var UserDetails = JSON.stringify(filterList)
                       url = URLs.sendMailNotification;
                       var Data = {
                           "UserDetails": UserDetails
                       };
                       var promise = ajaxWrapper.postJSON(url, Data);
                       // register ajax success callback
                       promise.done(function (result) {

                           if (result.Result == 0) {
                               messageDialog.show(UserManagementMessages.sentEmailNotifcationSucess);
                               loadUserMangementGrid();
                           }
                           else {
                               messageDialog.show(UserManagementMessages.sentEmailNotifcationFailedMsg);
                           }


                       });
                   }
               }
           });





            $(elementIDs.userListGridJQ).on('change', 'input[name="checkbox_userGrid"]', function (e) {
                var selectedUsers = [];
                var element = $(this).attr("Id");
                var id = element.split('_');
                var cellValue = $(this).is(":checked");
                if (cellValue) {
                    cellValue = "Yes";
                }
                else {
                    cellValue = "No";
                }
                $(elementIDs.userListGridJQ).jqGrid('setCell', id[1], 'SendEmailNotification', cellValue);
                var selectRowData = $(elementIDs.userListGridJQ).jqGrid('getRowData', (id[1]));
                selectRowData.SendEmailNotification = cellValue;
                $(elementIDs.userListGridJQ).jqGrid("saveRow", id[1], selectRowData);
                $(elementIDs.userListGridJQ).editRow(id[1], true);
                //selectedUsers.push(selectRowData);
            });



            //formatter use to get checkbox
            function checkBoxFormatter(cellValue, options, rowObject) {
                var result;
                if (cellValue == "Yes") {
                    result = '<input type="checkbox"' + ' id = checkbox_' + options.rowId + ' name= checkbox_userGrid' + ' checked />';
                }
                else {
                    result = "<input type='checkbox' id='checkbox_" + options.rowId + "' name= 'checkbox_userGrid'/>";
                }
                return result;

            }

            function unFormat(cellValue, options, rowObject) {
                var result;

                result = $(this).find('#' + options.rowId).find('input[name="checkbox_userGrid"]').prop('checked');
                if (result == true || result == "true")
                    return 'Yes';
                else
                    return 'No';

            }

        });
    }


    init();

}();
//This will be a single instance which is invoked soon after it is loaded in the browser
var account = function () {

    //urls to be accessed for account 
    var URLs = {
        //Get Account List
        accountList: '/ConsumerAccount/GetAccountList?tenantId=1',
        //Delete Account
        accountDelete: '/ConsumerAccount/Delete',
        //Create Account Permissions
        accountCreationPermission: '/ConsumerAccount/GetUserAccountCreationPermission',
        //Create Account Permissions
        folderVersionCreationPermission: '/FolderVersion/GetUserFolderVersionCreationPermission?isPortfolioSearch=false'
    };

    //element ID's required for account 
    //added in Views/Account/.cshtml
    var elementIDs = {
        //table element for the Account  Grid
        accountGrid: 'amg',
        //with hash for use with jQuery
        accountGridJQ: '#amg',
        //for dialog for add/edit of Account
        accountDialog: "#accountdialog",
        btnManageAccountAddJQ: '#btnManageAccountAdd',
        btnManageAccountEditJQ: '#btnManageAccountEdit',
        btnManageAccountDeleteJQ: '#btnManageAccountDelete'
    };

    //this function is called below soon after this JS file is loaded 
    //generates the tabs and loads the Account Grid
    function init() {
        $(document).ready(function () {
            //load the account grid
            loadAccountGrid();
            if (location.search == "?accountfolderadd=true")
                loadAccountFolderWizard();
        });
    }
    function loadAccountFolderWizard() {
        var promise = ajaxWrapper.getJSON(URLs.folderVersionCreationPermission);
        promise.done(function (result) {
            if (result == true) {
                accountFolderDialog.show('','add');
            } else {
                messageDialog.show('User do not have permission to create account and folder version.');
            }
        });
        //register ajax failure callback
        promise.fail(showError);
    }
    //load account list in grid
    function loadAccountGrid() {
        //set column list for grid
        var colArray = ['', '', 'Account Name'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true, search: false, });
        colModel.push({ name: 'AccountID', index: 'AccountID', key: true, hidden: true });
        colModel.push({ name: 'AccountName', index: 'AccountName', editable: false });

        //clean up the grid first - only table element remains after this
        $(elementIDs.accountGridJQ).jqGrid('GridUnload');

        var url = URLs.accountList;
        //adding the pager element
        $(elementIDs.accountGridJQ).parent().append("<div id='p" + elementIDs.accountGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.accountGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Accounts',
            height: '350',
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.accountGrid,
            sortname: 'AccountID',
            altclass: 'alternate',
            //on adding a new form design, reload the grid and set the row to selected
            gridComplete: function () {
                var rows = $(this).getDataIDs();
                var rowId;
                if (rows.length > 0) {
                    rowId = rows[0];
                }
                var newName = $(elementIDs.accountDialog + ' input').val();
                var newId;
                for (index = 0; index < rows.length; index++) {
                    row = $(this).getRowData(rows[index]);
                    if (newName === row.DisplayText) {
                        newId = row.AccountID;
                        break;
                    }
                }
                if (newId !== undefined) {
                    $(this).jqGrid('setSelection', newId);
                }
                else {
                    $(this).jqGrid('setSelection', rowId);
                }

                //to check for claims..              
                var objMap = {
                    edit: "#btnManageAccountEdit",
                    add: "#btnManageAccountAdd",
                    remove: "#btnManageAccountDelete",
                };
                checkApplicationClaims(claims, objMap, URLs.accountList);
                checkAccountManageClaims(elementIDs, claims);
            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.accountGridJQ);
            }
        });
        var pagerElement = '#p' + elementIDs.accountGrid;
        //remove default buttons
        $(elementIDs.accountGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        //add button in footer of grid that pops up the add account dialog
        $(elementIDs.accountGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-plus', title: 'Add', id: 'btnManageAccountAdd',
            onClickButton: function () {
                //Check if user has permission to create the Account
                var promise = ajaxWrapper.getJSON(URLs.accountCreationPermission);

                //register ajax success callback
                promise.done(function (result) {
                    if (result == true) {
                        //load Account dialog on click - see accountDialog function below
                        accountDialog.show('', 'add');
                    }else
                    {
                        messageDialog.show('User do not have permission to create an Account.');
                    }
                });
                //register ajax failure callback
                promise.fail(showError);
            }
        });

        //add button in footer of grid that pops up the edit account dialog
        $(elementIDs.accountGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-pencil', title: 'Edit', id: 'btnManageAccountEdit',
            onClickButton: function () {

                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    var row = $(this).getRowData(rowId);
                    //load Account dialog for edit on click - see accountDialog function below
                    accountDialog.show(row.AccountName, 'edit');
                }
            }
        });


        //add button in footer of grid that pops up the delete account dialog
        $(elementIDs.accountGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-trash', title: 'Delete', id: 'btnManageAccountDelete',
            onClickButton: function () {
                var rowId = $(this).getGridParam('selrow');
                var rowData= $(elementIDs.accountGridJQ).getRowData(rowId);
                if (rowId !== undefined && rowId !== null) {
                    //load confirm dialogue to asset the operation
                    confirmDialog.show((ConsumerAccount.deleteConfirmationForAccountMsg).replace(/\{#accountName}/g, rowData.AccountName), function () {
                        confirmDialog.hide();

                        //delete the form design
                        var accountDelete = {
                            tenantId: 1,
                            accountId: rowId
                        };
                        var promise = ajaxWrapper.postJSON(URLs.accountDelete, accountDelete);
                        //register ajax success callback
                        promise.done(loadAccountGrid);
                        //register ajax failure callback
                        promise.fail(showError);

                    });
                }
                else {
                    messageDialog.show(ConsumerAccount.accountNotExistMsg);
                }
            }
        });
        // add filter toolbar to the top
        $(elementIDs.accountGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });
    }

    //handler for ajax errors
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
    
    //initialization of the Account Grid when the Account function is loaded in browser and invoked
    init();

    //
    return {
        loadAccount: function () {
            loadAccountGrid();
        }
    }

}();

//contains functionality for the Account add dialog
var accountDialog = function () {
    var URLs = {
        //Add Account
        accountAdd: '/ConsumerAccount/Add',
        //Update Account
        accountUpdate: '/ConsumerAccount/Update',
    }

    //see element id's in Views\Account\ManageAccount.cshtml
    var elementIDs = {
        //Account grid element
        accountGrid: 'amg',
        //with hash for jquery
        accountGridJQ: '#amg',
        //form design dialog element
        accountDialog: "#accountdialog"
    };

    //maintains dialog state - add or edit
    var accountDialogState;

    //ajax success callback - for add/edit
    function accountSuccess(xhr, actionMode) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            if (actionMode == 'add')
                messageDialog.show(ConsumerAccount.addAccountMsg);
            else if (actionMode == 'edit')
                messageDialog.show(ConsumerAccount.updateAccountMsg);
            //reload form design grid 
            account.loadAccount();
        }
        else {
            messageDialog.show(ConsumerAccount.duplicateAccountMsg);
        }
        //reset dialog elements
        $(elementIDs.accountDialog + ' div').removeClass('has-error');
        $(elementIDs.accountDialog).dialog('close');
    }
    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
    //function for comparing strings
    function compareStrings(string1, string2, ignoreCase, useLocale) {
        if (!!ignoreCase) {
            string1 = string1.toLowerCase();
            string2 = string2.toLowerCase();
        }
        return string1 === string2;
    }
    //init dialog on load of page
    function init() {
        //register dialog for grid row add/edit
        $(elementIDs.accountDialog).dialog({
            autoOpen: false,
            height: 250,
            width: 450,
            modal: true
        });
        //register event for Add/Edit button click on dialog
        $(elementIDs.accountDialog + ' button').on('click', function () {
            //check if name is already used
            var newName = $(elementIDs.accountDialog + ' input').val();
            var accountNamesList = $(elementIDs.accountGridJQ).getRowData();
            var selectedAccountId = $(elementIDs.accountGridJQ).jqGrid('getGridParam', 'selrow');
            var filterList = accountNamesList.filter(function (ct) {
                if (ct.AccountID != selectedAccountId)
                    return compareStrings(ct.AccountName, newName, true);
            });

            //validate Account Name
            if (filterList !== undefined && filterList.length > 0) {
                $(elementIDs.accountDialog + ' div').addClass('has-error');
                $(elementIDs.accountDialog + ' .help-block').text(ConsumerAccount.duplicateAccountMsg);
            }
            else if (newName == '') {
                $(elementIDs.accountDialog + ' div').addClass('has-error');
                $(elementIDs.accountDialog + ' .help-block').text(ConsumerAccount.accountNameRequiredMsg);
            }
            else {
                //save the new account

                var rowId = $(elementIDs.accountGridJQ).getGridParam('selrow');
                var accountData = {
                    tenantId: 1,
                    accountID: rowId,
                    accountName: newName
                };
                var url;
                if (accountDialogState === 'add') {
                    accountData.accountID = 0;
                    url = URLs.accountAdd;
                }
                else {
                    url = URLs.accountUpdate;
                }
                //ajax call to add/update
                var promise = ajaxWrapper.postJSON(url, accountData);
                //register ajax success callback
                promise.done(function (data) { accountSuccess(data, accountDialogState) });
                //register ajax failure callback
                promise.fail(showError);
            }
        });
    }
    //initialize the dialog after this js are loaded
    init();

    //these are the properties that can be called by using AccountDialog.<Property>
    //eg. accountDialog.show('name','add');
    return {
        show: function (accountName, action) {
            accountDialogState = action;
            $(elementIDs.accountDialog + ' input').each(function () {
                $(this).val(accountName);
            });
            //set eleemnts based on whether the dialog is opened in add or edit mode
            $(elementIDs.accountDialog + ' div').removeClass('has-error');
            $(elementIDs.accountDialog + ' .help-block').text('');

            if (accountDialogState == 'add') {
                $(elementIDs.accountDialog).dialog('option', 'title', 'Add Account');
            }
            else {
                $(elementIDs.accountDialog).dialog('option', 'title', 'Edit Account');
            }
            //open the dialog - uses jqueryui dialog
            $(elementIDs.accountDialog).dialog("open");
        }
    }
}(); //invoked soon after loading





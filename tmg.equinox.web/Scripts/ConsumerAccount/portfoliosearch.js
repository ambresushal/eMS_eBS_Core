var portfolioSearch = function () {
    var isEditable = true;
    var isSubGridExpand = false;
    var subGridID = "";

    var URLs = {
        //Get details list of portfolio folders .
        portfolioDetails: '/ConsumerAccount/GetPortfolioBasedAccountList?tenantId=1&documentName={documentName}',
        //Get Folder Version Details List for subgrid
        getFolderVersionDetailsList: '/ConsumerAccount/GetFolderVersionList?tenantId=1&folderId={folderId}&accountName={accountName}&accountID={accountID}&documentName={documentName}',
        //Get Folder Version Details List for view an dedit buttons
        folderVersionDetails: '/FolderVersion/Index?tenantId={tenantId}&folderVersionId={folderVersionId}&folderId={folderId}&foldeViewMode={foldeViewMode}&mode={mode}',
        LockStatusUrl: '/FolderVersion/GetFolderLockStatus?tenantId={tenantId}&folderId={folderId}',
        OverrideLockUrl: '/FolderVersion/OverrideFolderLock?tenantId={tenantId}&folderId={folderId}',
        deletePortfolioBasedFolder: '/FolderVersion/DeleteNonPortfolioBasedFolder?tenantId={tenantId}&folderID={folderId}',
        //Create Account Permissions
        folderVersionCreationPermission: '/FolderVersion/GetUserFolderVersionCreationPermission?isPortfolioSearch=true',
        checkFolderIsQueued: '/FolderVersion/CheckFolderIsQueued'
    };
    var elementIDs = {
        //table element for the portfolio search Grid
        portfolioDetailsGrid: "pdg",
        //with hash for use with jQuery
        portfolioDetailsGridJQ: "#pdg",
        btnUnlockJQ: '#btnUnlock',
        btnDocSearchJQ: '#btnDocSearch',
        searchDocJQ: '#searchDoc',
        btnDocClearJQ: '#btnDocClear',
        btnPortfolioSearchNewJQ: '#btnPortfolioSearchNew',
        btnPortfolioSearchEditJQ: '#btnPortfolioSearchEdit',
        btnPortfolioSearchCopyJQ: '#btnPortfolioSearchCopy',
        btnPortfolioSearchViewJQ: '#btnPortfolioSearchView',
        folderViewDialog: '#folderViewDialog',
        btnClassicFolderView: '#btnClassicFolderView',
        btnSOTView: '#btnSOTView',
        buttonPortfolioRemoveJQ: "#btnPortfolioSearchDelete",
        buttonPortfolioRemove: "btnPortfolioSearchDelete"
    }

    if (typeof vbIsFolderLockEnable === "undefined") { this.isFolderLockEnable = false } else { this.isFolderLockEnable = vbIsFolderLockEnable };

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
            height: 120,
            width: 380,
            modal: true,
        });
        $(elementIDs.btnClassicFolderView).on('click', function () {
            if (isEditable == true) {
                var rowId = $(elementIDs.portfolioDetailsGridJQ).getGridParam('selrow');
                LoadPortfolioFolderVersion(rowId, FolderViewMode.DefaultView);
            }
            else if (isEditable == false) {
                if (isSubGridExpand == false) {
                    viewButtonClick(elementIDs.portfolioDetailsGrid, FolderViewMode.DefaultView);
                }
                else {
                    viewButtonClick(subGridID, FolderViewMode.DefaultView);
                }
            }
            $(elementIDs.folderViewDialog).dialog("close");
        });

        $(elementIDs.btnSOTView).on('click', function () {
            if (isEditable == true) {
                var rowId = $(elementIDs.portfolioDetailsGridJQ).getGridParam('selrow');
                LoadPortfolioFolderVersion(rowId, FolderViewMode.SOTView);
            }
            else if (isEditable == false) {
                if (isSubGridExpand == false) {
                    viewButtonClick(elementIDs.portfolioDetailsGrid, FolderViewMode.SOTView);
                }
                else {
                    viewButtonClick(subGridID, FolderViewMode.SOTView);
                }
            }
            $(elementIDs.folderViewDialog).dialog("close");
        });

        $(document).ready(function () {
            //load the account grid
            loadPortfolioGrid("");
            //Click event for Search button
            $(elementIDs.btnDocSearchJQ).click(function () {
                //Get entered value into the search box. 
                var documentName = $(elementIDs.searchDocJQ).val();
                loadPortfolioGrid(documentName);
            });
            //Click event for Clear button
            $(elementIDs.btnDocClearJQ).click(function () {
                $(elementIDs.searchDocJQ).val("");
                loadPortfolioGrid("");
            });

            $(elementIDs.searchDocJQ).on("keypress", function (e) {
                var code = e.keyCode || e.which;
                if (code == 13) {
                    loadPortfolioGrid(this.value);
                }
            });

        });
    }

    function LoadFolderViewDialog() {
        var rowId = $(elementIDs.portfolioDetailsGridJQ).getGridParam('selrow');
        if (rowId !== undefined && rowId !== null) {
            var row = $(elementIDs.portfolioDetailsGridJQ).getRowData(rowId);
            if (row.FolderID == "")
                messageDialog.show(ConsumerAccount.folderAbsentMsg);
            else {
                if (isEditable == false) {
                    $(elementIDs.folderViewDialog).dialog('option', 'title', "Select Folder View");
                    $(elementIDs.folderViewDialog).dialog("open");
                }
                else {
                    var input = {
                        folderID: row.FolderID,
                        folderVersionID: row.FolderVersionID
                    }
                    var currentInstance = this;
                    var promise = ajaxWrapper.postJSON(URLs.checkFolderIsQueued, input);
                    promise.done(function (xhr) {
                        if (xhr != null && xhr != undefined && xhr.Result == ServiceResult.FAILURE) {
                            $(elementIDs.folderViewDialog).dialog('option', 'title', "Select Folder View");
                            $(elementIDs.folderViewDialog).dialog("open");
                        }
                        else {
                            messageDialog.show(xhr.Items[0].Messages[0]);
                        }
                    });
                }
            }
        }
        else {
            messageDialog.show(FolderVersion.selectRowToView);
        }
    }

    function LoadPortfolioFolderVersion(rowId, viewMode) {
        if (rowId !== undefined && rowId !== null) {
            //var row = $(elementIDs.portfolioDetailsGridJQ).getRowData(rowId);
            var editFlag = checkUserPermissionForEditable(URLs.portfolioDetails);
            var row;
            var isReleasedVersion = false;
            var parentGridRowID = $(elementIDs.portfolioDetailsGridJQ).getGridParam('selrow');
            if (rowId == parentGridRowID)
                row = $(elementIDs.portfolioDetailsGridJQ).getRowData(rowId);
            else {
                var subGridIdJQ = "#pdg_" + parentGridRowID + "_t";
                row = $(subGridIdJQ).getRowData(rowId);
                isReleasedVersion = true;
            }
            if ((row.Status == 'Published' || row.Status == 'Release') && row.ApprovalStatusID !== null && (row.ApprovalStatusID == 1 || row.ApprovalStatusID == 6)) {
                isReleasedVersion = true;
            }
            if (row.FolderID == "")
                messageDialog.show(ConsumerAccount.folderAbsentMsg);
            else {
                if (isFolderLockEnable == false) {
                    var redirectUrl = URLs.folderVersionDetails.replace(/{tenantId}/g, row.TenantID).replace(/{folderVersionId}/g, row.FolderVersionID).replace(/{folderId}/g, row.FolderID).replace(/{foldeViewMode}/g, viewMode);
                    redirectUrl = redirectUrl.replace(/{mode}/g, true); //since Edit button is clicked mode needs to be true
                    window.location.href = redirectUrl;
                    return;
                } else if (isReleasedVersion == true) {
                    var redirectUrl = URLs.folderVersionDetails.replace(/{tenantId}/g, row.TenantID).replace(/{folderVersionId}/g, row.FolderVersionID).replace(/{folderId}/g, row.FolderID).replace(/{foldeViewMode}/g, viewMode);
                    redirectUrl = redirectUrl.replace(/{mode}/g, false);
                    window.location.href = redirectUrl;
                    return;
                }
                var promise = ajaxWrapper.getJSON(URLs.LockStatusUrl.replace(/{tenantId}/g, row.TenantID).replace(/\{folderId\}/g, row.FolderID));
                promise.done(function (result) {
                    if (result != "") {
                        var userWarning = FolderLock.userWarning.replace("{0}", result);
                        //checkUnlockFolderClaims(elementIDs, claims);
                        folderLockWarningDialog.show(userWarning, function () {
                            //unlock Folder Version
                            //var promise1 = ajaxWrapper.getJSON(URLs.OverrideLockUrl.replace(/{tenantId}/g, row.TenantID).replace(/\{folderId\}/g, row.FolderID));
                            //promise1.done(function (xhr) {
                            //    if (xhr.Result === ServiceResult.SUCCESS) {
                            //        var redirectUrl = URLs.folderVersionDetails.replace(/{tenantId}/g, row.TenantID).replace(/{folderVersionId}/g, row.FolderVersionID).replace(/{folderId}/g, row.FolderID).replace(/{foldeViewMode}/g, viewMode);
                            //        redirectUrl = redirectUrl.replace(/{mode}/g, true); //since Edit button is clicked mode needs to be true
                            //        window.location.href = redirectUrl;
                            //    }
                            //    else if (xhr.Result === ServiceResult.FAILURE) {
                            //        messageDialog.show(xhr.Items[0].Messages);
                            //    }
                            //    else {
                            //        messageDialog.show(Common.errorMsg);
                            //    }
                            //});
                        }, function () {
                            //View Folder Version
                            var redirectUrl = URLs.folderVersionDetails.replace(/{tenantId}/g, row.TenantID).replace(/{folderVersionId}/g, row.FolderVersionID).replace(/{folderId}/g, row.FolderID).replace(/{foldeViewMode}/g, viewMode);
                            redirectUrl = redirectUrl.replace(/{mode}/g, false); //since Edit button is clicked mode needs to be true
                            window.location.href = redirectUrl;
                        });
                        return;
                    }
                    else {
                        var redirectUrl = URLs.folderVersionDetails.replace(/{tenantId}/g, row.TenantID).replace(/{folderVersionId}/g, row.FolderVersionID).replace(/{folderId}/g, row.FolderID).replace(/{foldeViewMode}/g, viewMode);
                        if (GLOBAL.applicationName.toLowerCase() == 'ebenefitsync') {
                            redirectUrl = redirectUrl.replace(/{mode}/g, editFlag); //since Edit button is clicked mode needs to be true
                        } else {
                            redirectUrl = redirectUrl.replace(/{mode}/g, true);
                        }
                        window.location.href = redirectUrl;
                    }
                });
                promise.fail(showError);
                //var redirectUrl = URLs.folderVersionDetails.replace(/{tenantId}/g, row.TenantID).replace(/{folderVersionId}/g, row.FolderVersionID).replace(/{folderId}/g, row.FolderID).replace(/{foldeViewMode}/g, viewMode);
                //redirectUrl = redirectUrl.replace(/{mode}/g, true); //since Edit button is clicked mode needs to be true
                //window.location.href = redirectUrl;
            }
        }
    }

    function viewButtonClick(gridID, viewMode) {
        var rowId = $("#" + gridID).getGridParam('selrow');
        if (rowId !== undefined && rowId !== null) {
            var row = $("#" + gridID).getRowData(rowId);
            if (row.FolderID == "")
                messageDialog.show(ConsumerAccount.folderAbsentMsg);
            else {
                var redirectUrl = URLs.folderVersionDetails.replace(/{tenantId}/g, row.TenantID).replace(/{folderVersionId}/g, row.FolderVersionID).replace(/{folderId}/g, row.FolderID).replace(/{foldeViewMode}/g, viewMode);
                redirectUrl = redirectUrl.replace(/{mode}/g, false); //since view button is clicked mode needs to be false
                window.location.href = redirectUrl;
            }
        }
        else
            messageDialog.show(FolderVersion.selectRowToView);
    }

    function loadPortfolioGrid(documentName) {
        //set column list for grid
        var colArray = ['', '', '', '', '', '', '', 'Portfolio Name', 'Folder Version Number', '', 'Category Name', 'Effective Date', 'Status', 'Last Updated', 'Updated By', 'Is Foundation', '', 'Uses', '', ''];

        //set column models
        var colModel = [];
        colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true });
        colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true, key: true });
        colModel.push({ name: 'FolderVersionCount', index: 'FolderVersionCount', hidden: true });
        colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', hidden: true });
        colModel.push({ name: 'AccountID', index: 'AccountID', hidden: true });
        colModel.push({ name: 'MarketSegmentID', index: 'MarketSegmentID', hidden: true });
        colModel.push({ name: 'PrimaryContactID', index: 'PrimaryContactID', hidden: true });
        colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left' });
        colModel.push({ name: 'VersionNumber', index: 'VersionNumber', editable: false, align: 'right' });
        colModel.push({ name: 'CategoryID', index: 'CategoryID', hidden: true, editable: false, align: 'right' });
        colModel.push({ name: 'CategoryName', index: 'CategoryName', editable: false, align: 'left' });
        colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, search: true, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' });
        colModel.push({ name: 'Status', index: 'Status', editable: false, align: 'left' });
        colModel.push({ name: 'UpdatedDate', index: 'UpdatedDate', editable: false, search: true,formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' });
        colModel.push({ name: 'UpdatedBy', index: 'UpdatedBy', editable: false, align: 'left' });
        colModel.push({ name: 'IsFoundation', index: 'IsFoundation', align: 'center', hidden: (GLOBAL.applicationName.toLowerCase() == 'ebenefitsync') ? false : true });
        colModel.push({ name: 'ApprovalStatusID', index: 'ApprovalStatusID', hidden: true });
        colModel.push({ name: 'UsesCount', index: 'UsesCount', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'IsExpanded', index: 'IsExpanded', key: true, hidden: true, editable: false });
        colModel.push({ name: 'Mode', index: 'Mode', hidden: true });


        //clean up the grid first - only table element remains after this
        $(elementIDs.portfolioDetailsGridJQ).jqGrid('GridUnload');
        //adding the pager element
        $(elementIDs.portfolioDetailsGridJQ).parent().append("<div id='p" + elementIDs.portfolioDetailsGrid + "'></div>");
        var url = URLs.portfolioDetails.replace(/{documentName}/g, encodeURIComponent(documentName));
        $(elementIDs.portfolioDetailsGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Portfolio Search',
            height: '400',
            rowNum: 20,
            rowList: [10, 20, 30],
            //loadonce: true,
            autowidth: true,
            shrinkToFit: true,
            viewrecords: true,
            altRows: true,
            hidegrid: false,
            pager: '#p' + elementIDs.portfolioDetailsGrid,
            sortname: 'UpdatedDate',
            sortorder: 'desc',
            altclass: 'alternate',
            subGrid: true,
            //event for subGrid.
            subGridRowExpanded: function (subGridId, rowId) {
                $(this).jqGrid("setSelection", rowId);
                if (rowId !== undefined && rowId !== null) {
                    var row = $(this).getRowData(rowId);
                    var url = URLs.getFolderVersionDetailsList.replace(/{folderId}/g, row.FolderID).replace(/{accountName}/g, "").replace(/{accountID}/g, 0).replace(/{documentName}/g, documentName);
                    var subGridTableId, pager_id;
                    //table id of subgrid.
                    subGridTableId = subGridId + "_t";
                    subGridTableIdJQ = "#" + subGridTableId;
                    pager_id = "p_" + subGridTableId;
                    //Set value as true for IsExpanded column of selected row of main grid. 
                    row.IsExpanded = "true";
                    $(this).jqGrid("setRowData", rowId, row);
                    //created table tag.
                    $("#" + subGridId).html("<table id='" + subGridTableId + "' class='scroll'></table><div id='" + pager_id + "' class='scroll'></div>");
                    $(subGridTableIdJQ).jqGrid({
                        url: url,
                        datatype: 'json',
                        cache: false,
                        colNames: ['Folder', 'Folder Version Number', 'Effective Date', '', '', '', '', '', ''],
                        colModel: [
                            { name: 'FolderName', index: 'FolderName', width: 200, editable: false, align: 'left' },
                            { name: 'VersionNumber', index: 'VersionNumber', width: 200, editable: false, align: 'right' },
                            { name: 'EffectiveDate', index: 'EffectiveDate', width: 200, editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' },
                            { name: 'MarketSegmentID', index: 'MarketSegmentID', editable: false, align: 'right', hidden: true },
                            { name: 'PrimaryContactID', index: 'PrimaryContactID', editable: false, align: 'right', hidden: true },
                            { name: 'TenantID', index: 'TenantID', hidden: true },
                            { name: 'FolderID', index: 'FolderID', hidden: true, editable: false, align: 'left' },
                            { name: 'FolderVersionID', index: 'FolderVersionID', hidden: true, editable: false, align: 'left', Key: true },
                            { name: 'AccountID', index: 'AccountID', hidden: true },
                        ],
                        rowNum: 20,
                        viewrecords: true,
                        hidegrid: false,
                        altRows: true,
                        altclass: 'alternate',
                        height: '100%',
                        shrinkToFit: true,
                        pager: pager_id,
                        gridComplete: function () {

                            var rows = $(this).getDataIDs();
                            if (rows.length > 0) {
                                $(this).jqGrid("setSelection", rows[0]);
                            }
                            //check application claims               
                            var objMap = {
                                view: "#" + subGridTableId + 'view',
                                copy: "#" + subGridTableId + 'copy',
                                remove: "#btnPortfolioSearchDelete"
                            };
                            //checkApplicationClaims(claims, objMap, URLs.portfolioDetails);
                            checkSubGridPortfolioClaims(claims, objMap);
                        }
                    })
                }
                var pagerElement = '#' + pager_id;
                //remove default buttons
                $(subGridTableIdJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
                //remove paging
                $(pagerElement).find(pagerElement + '_center').remove();
                $(pagerElement).find(pagerElement + '_right').remove();

                //View button in footer of grid 
                $(subGridTableIdJQ).jqGrid('navButtonAdd', pagerElement,
                   {
                       caption: '', buttonicon: 'ui-icon-document', title: 'View', id: subGridTableId + "view",
                       onClickButton: function () {
                           isSubGridExpand = true;
                           subGridID = subGridTableId;
                           isEditable = false;
                           LoadFolderViewDialog();
                           //viewButtonClick(subGridTableId);
                       }
                   });
                //Copy button to copy data from selected folder and create new
                $("#" + subGridTableId).jqGrid('navButtonAdd', pagerElement,
                {
                    caption: '', buttonicon: 'ui-icon-copy', title: 'Copy', id: subGridTableId + "copy",
                    onClickButton: function () {
                        copyButtonClick(subGridTableId);
                    }
                });
            },
            subGridRowColapsed: function (subGridId, rowId) {
                isSubGridExpand = false;
                $(this).jqGrid("setSelection", rowId);
            },
            gridComplete: function () {
                var rows = $(this).getDataIDs();
                if (rows.length > 0) {
                    $(this).jqGrid("setSelection", rows[0]);
                }
                //To remove plus sign when subgrid is not present
                for (i = 0; i < rows.length; i++) {
                    var row = $(this).getRowData(rows[i]);
                    //Number of folder versions in one folder.
                    var count = row.FolderVersionCount;
                    if (row.FolderVersionID == "" || row.FolderVersionID == null) {
                        $('#' + rows[i]).children("td.sgcollapsed").unbind().html("");
                    }
                    else {
                        if (count < 2)
                            $('#' + rows[i]).children("td.sgcollapsed").unbind().html("");
                    }
                }
                //check application claims               
                var objMap = {
                    view: "#btnPortfolioSearchView",
                    edit: "#btnPortfolioSearchEdit",
                    add: "#btnPortfolioSearchNew",
                    copy: "#btnPortfolioSearchCopy",
                };
                checkApplicationClaims(claims, objMap, URLs.portfolioDetails);
                checkPortfolioClaims(elementIDs, claims);
            },
            //To disable edit button when selected folder version is released and approved.
            onSelectRow: function (rowId) {
                //to check if user has permissions to disable the Release Folder.(eg. Portfolio Designer.)                                 
                var editFlag = checkUserPermissionForEditable(URLs.portfolioDetails);
                if (rowId !== undefined && rowId !== null) {
                    var row = $(this).getRowData(rowId);
                    if (editFlag) {
                        //6th June : HNE-341 - Edit option is available when the folder is in 'Release' state
                        if ((row.Status == 'Published' || row.Status == 'Release') && row.ApprovalStatusID !== null && row.ApprovalStatusID == 1) {
                            $("#btnPortfolioSearchEdit").addClass('ui-state-disabled');
                        }else if((vbRole == Role.Viewer || vbRole == Role.ReViewer || vbRole == Role.ProductCoreAdminDesigner) && GLOBAL.clientName == 'HMHS')
                        {
                            $("#btnPortfolioSearchEdit").addClass('ui-state-disabled');
                            $("#btnPortfolioSearchDelete").addClass('ui-state-disabled');
                        }
                        else {
                            $("#btnPortfolioSearchEdit").removeClass('ui-state-disabled');
                        }


                        if ((row.Status == "BPD Distribution" && row.ApprovalStatusID == 1) || (row.Mode != undefined && row.Mode.toString() == "false")) {
                            $("#btnPortfolioSearchEdit").addClass('ui-state-disabled');
                        }


                    }else if(GLOBAL.clientName == 'HMHS')
                    {
                        $("#btnPortfolioSearchEdit").addClass('ui-state-disabled');
                        $("#btnPortfolioSearchDelete").addClass('ui-state-disabled');
                    }
                }
                //For selecting first row of expanded sub-grid.
                rows = $(this).getDataIDs();
                for (i = 0 ; i < rows.length ; i++) {
                    row = $(this).getRowData(rows[i]);
                    if (row.IsExpanded == "true")
                        $("#pdg_" + rows[i] + "_t").jqGrid("setSelection", 1);
                }
            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.portfolioDetailsGridJQ);
            },
            ondblClickRow: function (rowid, iRow, iCol, e) {

                LoadPortfolioFolderVersion(rowid, FolderViewMode.DefaultView);
            }
        });
        var pagerElement = '#p' + elementIDs.portfolioDetailsGrid;
        //remove default buttons
        $(elementIDs.portfolioDetailsGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.portfolioDetailsGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });


        //View button in footer of grid 
        $(elementIDs.portfolioDetailsGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-document', title: 'View', id: 'btnPortfolioSearchView',
               onClickButton: function () {

                   isSubGridExpand = false;
                   isEditable = false;
                   //LoadFolderViewDialog();
                   if (GLOBAL.applicationName.toLowerCase() == 'ebenefitsync') {
                       viewButtonClick(elementIDs.portfolioDetailsGrid, FolderViewMode.DefaultView);
                   } else {
                       LoadFolderViewDialog();
                   }
                   //viewButtonClick(elementIDs.portfolioDetailsGrid);
               }
           });
        //Edit button in footer of grid 
        $(elementIDs.portfolioDetailsGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-pencil', title: 'Edit', id: 'btnPortfolioSearchEdit',
            onClickButton: function () {
                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    isEditable = true;
                    if (GLOBAL.applicationName.toLowerCase() == 'ebenefitsync') {
                        LoadPortfolioFolderVersion(rowId, FolderViewMode.DefaultView);
                    } else {
                        LoadFolderViewDialog();
                    }

                    //LoadPortfolioFolderVersion(rowId);
                }

            }
        });
        //Add button to add folder
        $(elementIDs.portfolioDetailsGridJQ).jqGrid('navButtonAdd', pagerElement,
          {
              caption: '', buttonicon: 'ui-icon-plus', title: 'Add', id: 'btnPortfolioSearchNew',
              onClickButton: function () {
                  //Check if user has permission to create the Account
                  var promise = ajaxWrapper.getJSON(URLs.folderVersionCreationPermission);

                  //register ajax success callback
                  promise.done(function (result) {
                      if (result == true) {
                          folderDialog.show(true, '', 'add');
                      } else {
                          messageDialog.show('User do not have permission to create folder version.');
                      }
                  });
                  //register ajax failure callback
                  promise.fail(showError);
              }
          });

        //delete button in footer of grid 
        if (GLOBAL.applicationName.toLowerCase() == 'ebenefitsync' && (vbRole == Role.ClientSuperUser || vbRole == Role.TMGSuperUser)) {
            $(elementIDs.portfolioDetailsGridJQ).jqGrid('navButtonAdd', pagerElement,
               {
                   caption: '', buttonicon: 'ui-icon-trash', title: 'delete', id: elementIDs.buttonPortfolioRemove,
                   onClickButton: function () {

                       var rowID = $(this).getGridParam('selrow');
                       var row = $(elementIDs.portfolioDetailsGridJQ).getRowData(rowID);
                       confirmDialog.show(ConsumerAccount.deleteConfirmationForFolderMsg.replace(/\{#FolderName}/g, row.FolderName.toUpperCase()), function () {
                           confirmDialog.hide();

                           if (row !== undefined && row !== null) {
                               if (row.FolderID == "")
                                   messageDialog.show(ConsumerAccount.folderAbsentMsg);
                               else {
                                   var promise = ajaxWrapper.getJSON(URLs.LockStatusUrl.replace(/{tenantId}/g, row.TenantID).replace(/\{folderId\}/g, row.FolderID));
                                   promise.done(function (result) {
                                       if (result != "") {
                                           var userWarning = ConsumerAccount.userWarning.replace("{0}", result);

                                           messageDialog.show(userWarning);
                                           return;
                                       }
                                       else {
                                           var url = URLs.deletePortfolioBasedFolder.replace(/{tenantId}/g, row.TenantID).replace(/{folderId}/g, row.FolderID);
                                           var promise = ajaxWrapper.getJSON(url);
                                           promise.done(function (result) {
                                               if (result.Result == 2) {
                                                   if (result.Items.length > 0) {
                                                       messageDialog.show(result.Items[0].Messages);
                                                   }
                                                   else {
                                                       messageDialog.show("Failure to Delete Folder");
                                                   }
                                                   return;
                                               }
                                               else {
                                                   $(elementIDs.portfolioDetailsGridJQ).trigger("reloadGrid")
                                                   messageDialog.show(ConsumerAccount.folderDeleteSuccess);
                                               }
                                           });
                                           promise.fail(showError);
                                       }
                                   });
                                   promise.fail(showError);
                               }
                           }

                       });

                   }
               });
        }


        function copyButtonClick(gridID) {
            var rowId = $("#" + gridID).getGridParam('selrow');
            if (rowId !== undefined && rowId !== null) {
                var row = $("#" + gridID).getRowData(rowId);
                if (row.FolderID == "")
                    messageDialog.show(ConsumerAccount.folderAbsentMsg);
                else {
                    folderDialog.show(true, row, 'Copy');
                }
            }
        }
        //Copy button to copy data from selected folder and create new
        $(elementIDs.portfolioDetailsGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-copy', title: 'Copy', id: 'btnPortfolioSearchCopy',
            onClickButton: function () {
                //Check if user has permission to create the Account
                var promise = ajaxWrapper.getJSON(URLs.folderVersionCreationPermission);

                //register ajax success callback
                promise.done(function (result) {
                    if (result == true) {
                        copyButtonClick(elementIDs.portfolioDetailsGrid);
                    } else {
                        messageDialog.show('User do not have permission to copy folder version.');
                    }
                });
                //register ajax failure callback
                promise.fail(showError);
            }
        });
    }
    init();


    return {
        loadPortfolio: function () {
            loadPortfolioGrid();
        }
    }
}();



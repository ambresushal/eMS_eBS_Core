var versionHistory = function () {

    var URLs = {
        majorVersionList: '/FolderVersion/GetVersionHistory?folderId={folderId}&tenantId=1&versionType=Major',
        minorVersionList: '/FolderVersion/GetVersionHistory?folderId={folderId}&tenantId=1&versionType=Minor',
        FolderVersionDetails: '/FolderVersion/Index?tenantId=1&folderVersionId={folderVersionId}&folderId={folderId}&mode={mode}',
        isAnyFolderVersionInProgress: '/FolderVersion/IsAnyFolderVersionInProgress?folderId={folderId}&tenantId=1',
        addMinorFolderVersion: '/FolderVersion/AddMinorFolderVersion',
        folderVersionDetailsPortfolioBasedAccount: '/FolderVersion/Index?tenantId={tenantId}&folderVersionId={folderVersionId}&folderId={folderId}&foldeViewMode={foldeViewMode}&mode={mode}',
        folderVersionDetailsNonPortfolioBasedAccount: '/FolderVersion/GetNonPortfolioBasedAccountFolders?tenantId={tenantId}&folderVersionId={folderVersionId}&folderId={folderId}&folderType={folderType}&mode={mode}&foldeViewMode={FolderViewMode}',
        deleteFolderVersion: '/FolderVersion/DeleteFolderVersion',
        rollbackFolderVersion: '/FolderVersion/RollbackFolderVersion',
        accountSearch: '/ConsumerAccount/AccountSearch',
        portfoliaccountSearch: '/ConsumerAccount/PortfolioSearch',
        isFolderVersionCanRollback: '/FolderVersion/IsFolderVersionCanRollback?tenantId={tenantId}&rollbackFolderVersionNumber={rollbackFolderVersionNumber}&inProgressMinorVersionNumber={inProgressMinorVersionNumber}',
        redirectFolderVersionReport: '/FolderVersionReport/FolderVersionReport',
        LockStatusUrl: '/FolderVersion/GetFolderLockStatus?tenantId=1&folderId={folderId}',
        OverrideLockUrl: '/FolderVersion/OverrideFolderLock?tenantId=1&folderId={folderId}',
        //Add Workflow States
        applicableTeamsAdd: '/FolderVersion/AddApplicableTeams',
        //Gets the Category List
        categoryList: '/Settings/GetFolderVersionCategoryForDropdown?tenantId=1&isPortfolio=null&folderVersionID={folderVersionID}',
        getFolderVersion: '/FolderVersion/GetFolderVersion?folderVersionID={folderVersionID}'
    };

    var isApplicableTeamsRequired = false;
    var elementIDs = {
        versionHistoryDialog: '#vesrionHistoryDialog',
        majorVersionGrid: 'majorVersion',
        majorVersionGridJQ: '#majorVersion',
        minorVersionGrid: 'minorVersion',
        minorVersionGridJQ: '#minorVersion',
        versionHistoryButton: '#versionHistoryButton',
        newButton: '#newVersionHistory',
        retroButton: '#retroVersionHistory',
        changeSummaryButton: '#changeSummary',
        minorEffectiveDateDialog: "#minoreffectivedatedialog",
        minorEffectiveDate: "#effectivedate",
        ctegory: "#categoryDDL",
        minorEffectiveDateButton: "#minoreffectivebtn",
        minorVersionRollbackButton: "#btnMinorVersionRollback",
        minorVersionDeleteButton: "#btnMinorVersionDelete",
        minorVersionViewButton: "#btnMinorVersionView",
        minorVersionEditButton: "#btnMinorVersionEdit",
        btnUnlockJQ: '#btnUnlock',
        checkAllId: '#checkAll',
        categoryDDLJQ: '#categoryDDL',
        categoryDDLContainer: '#categoryDDLContainer',
        applicableTeamsContainer: '#applicableTeamsContainer',
        catIDJQ: '#catIDTXT',
        newversionCategoryDDLSpan: "#newversion_CategoryDDLSpan",
        newversioneffectivedate: "#newversion_effectivedate"
    };

    //ajax success callback
    function addSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            //code to save selected workflow states
            if (isApplicableTeamsRequired == true) {
                var applicableTeamsIDs = new Array();
                $(elementIDs.minorEffectiveDateDialog).find('.applicableTeamsCheckbox').each(function () {
                    if ($(this).is(':checked'))
                        applicableTeamsIDs.push(parseInt($(this).attr('id')));
                });
                if (isApplicableTeamsRequired)
                    var applicableTeamsAddData = {
                        FolderID: parseInt(folderData.folderId),
                        FolderVersionID: parseInt(folderData.folderVersionId),
                        ApplicableTeamsIDList: applicableTeamsIDs,
                    };
                //ajax call to add folder workflow states
                var promise = ajaxWrapper.postJSONCustom(URLs.applicableTeamsAdd, applicableTeamsAddData);
                //register ajax success callback
                promise.done(function (result) {
                    if (result.Result === ServiceResult.FAILURE) {
                        messageDialog.show("Applicable Teams are not saved.");
                        $(elementIDs.minorEffectiveDateDialog).dialog("close");
                        $(elementIDs.versionHistoryDialog).dialog("close");
                        return;
                    }
                    else {
                        $(elementIDs.minorEffectiveDateDialog).dialog("close");
                        if (xhr.Items[0].Messages[0] == "True") {
                            messageDialog.show(FolderVersion.folderVersionRenewalMsg);
                        }
                        else {
                            messageDialog.show(FolderVersion.folderVersionCreatedSuccess);
                        }
                        setInputControlsState();
                        loadMinorVersionGrid();
                    }
                });
                //register ajax failure callback
                promise.fail(showError);
            }
            else {
                $(elementIDs.minorEffectiveDateDialog).dialog("close");
                if (xhr.Items[0].Messages[0] == "True") {
                    messageDialog.show(FolderVersion.folderVersionRenewalMsg);
                }
                else {
                    messageDialog.show(FolderVersion.folderVersionCreatedSuccess);
                }
                setInputControlsState();
                loadMinorVersionGrid();
            }
        }
        else if (xhr.Result === ServiceResult.FAILURE) {
            messageDialog.show(xhr.Items[0].Messages[0]);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
    }

    //ajax success callback
    function deleteSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            $(elementIDs.minorEffectiveDateDialog).dialog("close");
            messageDialog.show(FolderVersion.folderVersionDeletedSuccess);
            setInputControlsState();
            loadMinorVersionGrid();
            //Fixed for ANT-147, If the Folder data is Portfolio then URL should be for Portfoli Search
            var url = "";
            if (folderData.isPortfolio == "True" || folderData.isPortfolio == "true") {
                url = URLs.portfoliaccountSearch;
            }
            else {
                url = URLs.accountSearch;
            }

            xhr.Items.forEach(function (item) {
                if (item.Messages[0] == folderData.folderVersionId) {
                    window.location.href = url;
                }
            });
        }
        else if (xhr.Result === ServiceResult.FAILURE) {
            messageDialog.show(xhr.Items[0].Messages[0]);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
    }

    //ajax success callback
    function rollbackSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            $(elementIDs.minorEffectiveDateDialog).dialog("close");
            messageDialog.show(FolderVersion.folderVersionRollbackSuccess);
            loadMinorVersionGrid();
            setInputControlsState();
            $(elementIDs.versionHistoryDialog).on("dialogclose", function (event, ui) {
                var url = window.location.href.replace('mode=true', 'mode=false');
                window.location.href = url;
            });
        }
        else if (xhr.Result === ServiceResult.FAILURE) {
            messageDialog.show(xhr.Items[0].Messages[0]);
        }
        else {
            messageDialog.show(xhr.Items[0].Messages[0]);
        }
    }

    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    function setInputControlsState() {
        var url = URLs.isAnyFolderVersionInProgress.replace(/\{folderId\}/g, folderData.folderId);
        var promise = ajaxWrapper.getJSON(url);
        promise.done(function (inProgress) {
            //changes to check if user has permissions for newVersion and Retro(eg. Viewer.) 
            var claimsUrl = window.location.pathname;
            if (folderData.folderId == MasterList.MASTERLISTFOLDERID) {
                var sectioName = "/FolderVersion/Index/ML";
                //var editFlag = checkUserPermissionForEditable(sectioName);
                var editFlag = checkUserPermissionNewVersionRetro(sectioName);
            }
            else if (claimsUrl === "/FolderVersion/Index") {
                //var editFlag = checkUserPermissionForEditable(URLs.FolderVersionDetails);
                var editFlag = checkUserPermissionNewVersionRetro(URLs.FolderVersionDetails);
            }
            else {
                //var editFlag = checkUserPermissionForEditable(URLs.folderVersionDetailsNonPortfolioBasedAccount);
                var editFlag = checkUserPermissionNewVersionRetro(URLs.folderVersionDetailsNonPortfolioBasedAccount);
            }

            if (editFlag)
                if (inProgress) {
                    $(elementIDs.minorVersionRollbackButton).addClass('ui-state-disabled');
                    $(elementIDs.newButton).attr('disabled', 'disabled');
                    $(elementIDs.retroButton).attr('disabled', 'disabled');
                } else {
                    $(elementIDs.newButton).removeAttr('disabled', 'disabled');
                    $(elementIDs.retroButton).removeAttr('disabled', 'disabled');
                    $(elementIDs.minorVersionRollbackButton).removeClass('ui-state-disabled');
                }

        });
    }

    function loadMajorVersionGrid() {
        //set column list for grid
        var colArray = ['', '', 'Major Version', 'Folder Effective Date', 'Status', 'Category', '', 'User', 'Comments'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'FolderVersionId', index: 'FolderVersionId', key: true, hidden: true, search: false });
        colModel.push({ name: 'FolderId', index: 'FolderId', hidden: true, search: false });
        colModel.push({ name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: false, width: '90' });
        colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, align: 'center', formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions });
        colModel.push({ name: 'WFStateName', index: 'WFStateName', editable: false });
        colModel.push({ name: 'CategoryName', index: 'CategoryName', editable: false });
        colModel.push({ name: 'CategoryId', index: 'CategoryId', hidden: true, editable: false });
        //colModel.push({ name: 'CatID', index: 'CatID', editable: false });
        colModel.push({ name: 'User', index: 'User', editable: false });
        colModel.push({ name: 'Comments', index: 'Comments', editable: false });

        //clean up the grid first - only table element remains after this
        $(elementIDs.majorVersionGridJQ).jqGrid('GridUnload');
        var url = URLs.majorVersionList.replace(/\{folderId\}/g, folderData.folderId);
        //adding the pager element
        $(elementIDs.majorVersionGridJQ).parent().append("<div id='p" + elementIDs.majorVersionGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.majorVersionGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Major Version List',
            height: '100',
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: true,
            altRows: true,
            pager: '#p' + elementIDs.majorVersionGrid,
            sortname: 'MajorVersion',
            altclass: 'alternate',
            gridComplete: function () {
                var grid = this;
                var allRowsData = $(grid).getRowData();
                if (allRowsData.length <= 0) {
                    $('#btnMajorVersionView').addClass('ui-state-disabled');
                }

                //to check for claims..  
                var objMap = {
                    view: "#btnMajorVersionView"
                };
                var claimsUrl = window.location.pathname;
                //to check for Masterlist permissions..
                var url = window.location.href;
                var check = url.split("&");
                var masterlist = check[2];
                var masterlistFolderId = masterlist.split("=")[1];
                if (masterlistFolderId == MasterList.MASTERLISTFOLDERID) {
                    var sectioName = "/FolderVersion/Index/ML"
                    checkApplicationClaims(claims, objMap, sectioName);
                }
                else if (claimsUrl === "/FolderVersion/Index") {
                    checkApplicationClaims(claims, objMap, URLs.FolderVersionDetails);
                }
                else {
                    checkApplicationClaims(claims, objMap, URLs.folderVersionDetailsNonPortfolioBasedAccount);
                }
            },
        });

        var pagerElement = '#p' + elementIDs.majorVersionGrid;
        //remove default buttons
        $(elementIDs.majorVersionGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false,refresh:false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        // add filter toolbar to the top
        $(elementIDs.majorVersionGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });

        //View button in footer of grid 
        $(elementIDs.majorVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-document', title: 'View', id: 'btnMajorVersionView',
               onClickButton: function () {
                   var rowId = $(this).getGridParam('selrow');
                   if (rowId !== undefined && rowId !== null) {
                       var row = $(this).getRowData(rowId);

                       if (folderData.isPortfolio == "True") {
                           loadPortfolioBasedAccount(row.FolderVersionId, FolderViewMode.DefaultView);
                       }
                       else {
                           loadNonPortfolioBasedAccount(row.FolderVersionId,FolderViewMode.DefaultView);
                       }
                   } else {
                       messageDialog.show(FolderVersion.selectRowToView);
                   }

               }
           });
    }

    function loadMinorVersionGrid() {
        //set column list for grid
        var colArray = ['', '', 'Minor Version', 'Folder Effective Date', 'Baseline Date', 'State', '', 'Category', '', 'User', 'Comments', 'WFStateName'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'FolderVersionId', index: 'FolderVersionId', key: true, hidden: true, search: false });
        colModel.push({ name: 'FolderId', index: 'FolderId', hidden: true, search: false });
        colModel.push({ name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: false, width: '90' });
        colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, align: 'center', formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions });
        colModel.push({ name: 'AddedDate', index: 'AddedDate', editable: false, align: 'center', formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions });
        colModel.push({ name: 'FolderVersionStateName', index: 'FolderVersionStateName', editable: false });
        colModel.push({ name: 'VersionType', index: 'VersionType', editable: false, hidden: true });
        colModel.push({ name: 'CategoryName', index: 'CategoryName', editable: false });
        colModel.push({ name: 'CategoryId', index: 'CategoryId', hidden: true, editable: false });
        //colModel.push({ name: 'CatID', index: 'CatID', editable: false });
        colModel.push({ name: 'User', index: 'User', editable: false });
        colModel.push({ name: 'Comments', index: 'Comments', editable: false });
        colModel.push({ name: 'WFStateName', index: 'WFStateName', editable: false, hidden: true });

        //clean up the grid first - only table element remains after this
        $(elementIDs.minorVersionGridJQ).jqGrid('GridUnload');

        var url = URLs.minorVersionList.replace(/\{folderId\}/g, folderData.folderId);;
        //adding the pager element
        $(elementIDs.minorVersionGridJQ).parent().append("<div id='p" + elementIDs.minorVersionGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.minorVersionGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Minor Version List',
            height: '100',
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: true,
            altRows: true,
            pager: '#p' + elementIDs.minorVersionGrid,
            sortname: 'MinorVersion',
            altclass: 'alternate',
            gridComplete: function () {
                var grid = this;
                var allRowsData = $(grid).getRowData();
                isReleased = true;
                if (allRowsData.length > 0) {
                    $.each(allRowsData, function (i, item) {
                        if (item.FolderVersionStateName == FolderVersion.inProgress && item.VersionType == FolderVersion.new) {
                            isReleased = false;
                        }
                    });
                    if (isReleased) {
                        $(elementIDs.minorVersionRollbackButton).addClass('ui-state-disabled');
                        $(elementIDs.minorVersionDeleteButton).addClass('ui-state-disabled');
                    } else {
                        $(elementIDs.minorVersionRollbackButton).removeClass('ui-state-disabled');
                        $(elementIDs.minorVersionDeleteButton).removeClass('ui-state-disabled');
                    }
                } else {
                    $(elementIDs.minorVersionRollbackButton).addClass('ui-state-disabled');
                    $(elementIDs.minorVersionDeleteButton).addClass('ui-state-disabled');
                    $(elementIDs.minorVersionEditButton).addClass('ui-state-disabled');
                    $(elementIDs.minorVersionViewButton).addClass('ui-state-disabled');
                }

                //to check for claims..  
                var objMap = {
                    view: elementIDs.minorVersionViewButton,
                    edit: elementIDs.minorVersionEditButton,
                    rollback: elementIDs.minorVersionRollbackButton,
                    remove: elementIDs.minorVersionDeleteButton,
                };
                var claimsUrl = window.location.pathname;
                //to check for Masterlist permissions..
                var url = window.location.href;
                var check = url.split("&");
                var masterlist = check[2];
                var masterlistFolderId = masterlist.split("=")[1];
                if (masterlistFolderId == MasterList.MASTERLISTFOLDERID) {
                    var sectioName = "/FolderVersion/Index/ML"
                    checkApplicationClaims(claims, objMap, sectioName);
                }
                else if (claimsUrl === "/FolderVersion/Index") {
                    checkApplicationClaims(claims, objMap, URLs.FolderVersionDetails);
                }
                else {
                    checkApplicationClaims(claims, objMap, URLs.folderVersionDetailsNonPortfolioBasedAccount);
                }
            },
            onSelectRow: function (id) {

                var inProgressVersionNumber = null;
                var allRowsData = $(elementIDs.minorVersionGridJQ).getRowData();
                if (allRowsData.length > 0) {
                    $.each(allRowsData, function (i, item) {
                        if (item.FolderVersionStateName == FolderVersion.inProgress && item.VersionType == FolderVersion.new) {
                            inProgressVersionNumber = item.FolderVersionNumber;
                        }
                    });
                }
                var row = $(this).getRowData(id);

                if (row.FolderVersionStateName == FolderVersion.inProgress) {
                    if (row.VersionType == FolderVersion.new) {
                        $(elementIDs.minorVersionRollbackButton).addClass('ui-state-disabled');
                    }
                    //if (vbRole == Role.Product && row.WFStateName == "Test/Audit") {
                    //    $(elementIDs.minorVersionDeleteButton).addClass('ui-state-disabled');
                    //    $(elementIDs.minorVersionEditButton).addClass('ui-state-disabled');
                    //}
                    //else {
                    $(elementIDs.minorVersionDeleteButton).removeClass('ui-state-disabled');
                    $(elementIDs.minorVersionEditButton).removeClass('ui-state-disabled');
                    //}
                }
                else if (row.FolderVersionStateName == FolderVersion.inProgressBlocked) {
                    $(elementIDs.minorVersionRollbackButton).addClass('ui-state-disabled');
                    $(elementIDs.minorVersionDeleteButton).addClass('ui-state-disabled');
                    $(elementIDs.minorVersionEditButton).addClass('ui-state-disabled');
                }
                else {
                    if (row.FolderVersionStateName == FolderVersion.baselined && inProgressVersionNumber != null) {
                        var url = URLs.isFolderVersionCanRollback.replace(/{tenantId}/g, folderData.tenantId)
                            .replace(/{rollbackFolderVersionNumber}/g, row.FolderVersionNumber)
                            .replace(/{inProgressMinorVersionNumber}/g, inProgressVersionNumber);
                        var promise = ajaxWrapper.getJSON(url);
                        promise.done(function (x) {
                            $(elementIDs.minorVersionDeleteButton).addClass('ui-state-disabled');
                            $(elementIDs.minorVersionEditButton).addClass('ui-state-disabled');
                            if (x.Result == ServiceResult.SUCCESS) {
                                $(elementIDs.minorVersionRollbackButton).removeClass('ui-state-disabled');
                            } else {
                                $(elementIDs.minorVersionRollbackButton).addClass('ui-state-disabled');
                            }
                        });
                    } else {
                        $(elementIDs.minorVersionRollbackButton).addClass('ui-state-disabled');
                        $(elementIDs.minorVersionDeleteButton).addClass('ui-state-disabled');
                        $(elementIDs.minorVersionEditButton).addClass('ui-state-disabled');
                    }
                }
                //to check for claims..  
                var objMap = {
                    view: elementIDs.minorVersionViewButton,
                    edit: elementIDs.minorVersionEditButton,
                    rollback: elementIDs.minorVersionRollbackButton,
                    remove: elementIDs.minorVersionDeleteButton,
                };
                var claimsUrl = window.location.pathname;
                //to check for Masterlist permissions..
                var url = window.location.href;
                var check = url.split("&");
                var masterlist = check[2];
                var masterlistFolderId = masterlist.split("=")[1];
                if (masterlistFolderId == MasterList.MASTERLISTFOLDERID) {
                    var sectioName = "/FolderVersion/Index/ML"
                    checkApplicationClaims(claims, objMap, sectioName);
                }
                else if (claimsUrl === "/FolderVersion/Index") {
                    checkApplicationClaims(claims, objMap, URLs.FolderVersionDetails);
                }
                else {
                    checkApplicationClaims(claims, objMap, URLs.folderVersionDetailsNonPortfolioBasedAccount);
                }
            }
        });
        var pagerElement = '#p' + elementIDs.minorVersionGrid;
        //remove default buttons
        $(elementIDs.minorVersionGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false,refresh:false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        // add filter toolbar to the top
        $(elementIDs.minorVersionGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });

        //Rollback button in footer of grid 
        $(elementIDs.minorVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-arrowreturn-1-w', title: 'Rollback', id: 'btnMinorVersionRollback',
               onClickButton: function () {
                   var rowId = $(this).getGridParam('selrow');
                   if (rowId !== undefined && rowId !== null) {
                       var row = $(this).getRowData(rowId);

                       versionToRollback = {
                           rollbackFolderVersionId: row.FolderVersionId,
                           tenantId: folderData.tenantId,
                           folderId: folderData.folderId,
                           rollbackFolderVersionNumber: row.FolderVersionNumber
                       };
                       url = URLs.rollbackFolderVersion;
                       confirmDialog.show(FolderVersion.confirmRollbackFolderVersion, function () {
                           confirmDialog.hide();
                           var promise = ajaxWrapper.postJSON(url, versionToRollback);
                           //register ajax success callback
                           promise.done(rollbackSuccess);
                           //register ajax failure callback
                           promise.fail(showError);
                       });
                   }
               }
           });

        //View button in footer of grid 
        $(elementIDs.minorVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-document', title: 'View', id: 'btnMinorVersionView',
               onClickButton: function () {
                   var rowId = $(this).getGridParam('selrow');
                   if (rowId !== undefined && rowId !== null) {
                       var row = $(this).getRowData(rowId);

                       if (folderData.isPortfolio == "True") {
                           loadPortfolioBasedAccount(row.FolderVersionId, false,FolderViewMode.DefaultView );
                       } else {
                           loadNonPortfolioBasedAccount(row.FolderVersionId, false, FolderViewMode.DefaultView);
                       }
                   } else {
                       messageDialog.show(FolderVersion.selectRowToView);
                   }
               }
           });

        //Edit button in footer of grid 
        $(elementIDs.minorVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-pencil', title: 'Edit', id: 'btnMinorVersionEdit',
               onClickButton: function () {
                   var rowId = $(this).getGridParam('selrow');
                   if (rowId !== undefined && rowId !== null) {
                       var row = $(this).getRowData(rowId);

                       if (vbIsFolderLockEnable == false) {
                           if (folderData.isPortfolio == "True") {
                               loadPortfolioBasedAccount(row.FolderVersionId, true);
                               return;
                           } else {
                               loadNonPortfolioBasedAccount(row.FolderVersionId, true);
                               return;
                           }
                       }
                       //var promise = ajaxWrapper.getJSON(URLs.LockStatusUrl.replace(/\{folderId\}/g, row.FolderId));
                       //promise.done(function (result) {
                       //    if (result != "") {
                       //        var userWarning = FolderLock.userWarning.replace("{0}", result);
                       //        checkUnlockFolderClaims(elementIDs, claims);
                       //        folderLockWarningDialog.show(userWarning, function () {
                       //            //unlock Folder Version
                       //            var promise1 = ajaxWrapper.getJSON(URLs.OverrideLockUrl.replace(/{tenantId}/g, row.TenantID).replace(/\{folderId\}/g, row.FolderId));
                       //            promise1.done(function (xhr) {
                       //                if (xhr.Result === ServiceResult.SUCCESS) {
                       //                    if (folderData.isPortfolio == "True") {
                       //                        loadPortfolioBasedAccount(row.FolderVersionId, true);
                       //                    } else {
                       //                        loadNonPortfolioBasedAccount(row.FolderVersionId, true);
                       //                    }
                       //                }
                       //                else if (xhr.Result === ServiceResult.FAILURE) {
                       //                    //messageDialog.show(xhr.Items[0].Messages);
                       //                }
                       //                else {
                       //                    messageDialog.show(Common.errorMsg);
                       //                }
                       //            });
                       //        }, function () {
                       //            if (folderData.isPortfolio == "True") {
                       //                loadPortfolioBasedAccount(row.FolderVersionId, false);
                       //            } else {
                       //                loadNonPortfolioBasedAccount(row.FolderVersionId, false);
                       //            }
                       //        });
                       //        return;
                       //    }
                       //    else {
                       if (folderData.isPortfolio == "True") {
                           loadPortfolioBasedAccount(row.FolderVersionId, true);
                       } else {
                           loadNonPortfolioBasedAccount(row.FolderVersionId, true);
                       }
                       //    }
                       //});

                   } else {
                       messageDialog.show(FolderVersion.selectRowToEdit);
                   }
               }
           });

        //Delete button in footer of grid 
        $(elementIDs.minorVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-trash', title: 'Delete', id: 'btnMinorVersionDelete',
               onClickButton: function () {
                   var rowId = $(this).getGridParam('selrow');
                   if (rowId !== undefined && rowId !== null) {
                       var row = $(this).getRowData(rowId);
                       var minorGridData = $(this).getRowData();
                       var majorGridData = $(elementIDs.majorVersionGridJQ).jqGrid('getGridParam', 'data');;
                       if (row.FolderVersionStateName == FolderVersion.inProgress) {

                           if (minorGridData.length == 1 && majorGridData.length < 1) {
                               messageDialog.show(FolderVersion.cannotPerformDelete);
                           }
                               //If the product is in translation or transmission then one should not allow the folder version to delete.
                           else if (folderManager.getInstance().getFolderInstance().checkForStatusUpdateDisableButton()) {
                               messageDialog.show(FolderVersion.cannotPerformDelete);
                           }
                           else {
                               versionToDelete = {
                                   folderVersionId: row.FolderVersionId,
                                   tenantId: folderData.tenantId,
                                   folderId: folderData.folderId,
                                   versionType: row.VersionType
                               };
                               url = URLs.deleteFolderVersion;
                               confirmDialog.show(FolderVersion.confirmDeleteFolderVersion, function () {
                                   confirmDialog.hide();

                                   var promise = ajaxWrapper.postJSON(url, versionToDelete);
                                   //register ajax success callback
                                   promise.done(deleteSuccess);
                                   //register ajax failure callback
                                   promise.fail(showError);
                               });
                           }
                       } else {
                           messageDialog.show(FolderVersion.selectInProgress);
                       }
                   }
                   else {
                       messageDialog.show(FolderVersion.selectRowToDelete);
                   }
               }
           });
    }

    function loadPortfolioBasedAccount(folderVersionId, mode) {
        url = URLs.folderVersionDetailsPortfolioBasedAccount
                    .replace(/{tenantId}/g, folderData.tenantId)
                    .replace(/{folderVersionId}/g, folderVersionId)
                    .replace(/{folderId}/g, folderData.folderId)
                    .replace(/{foldeViewMode}/g, folderData.FolderViewMode);

        //If view button is clicked mode needs to be "false" else "true"
        url = url.replace(/{mode}/g, mode) + "&isPopup=true";
        window.location.href = url;
    }

    function loadNonPortfolioBasedAccount(folderVersionId, mode,FolderViewMode) {

        url = URLs.folderVersionDetailsNonPortfolioBasedAccount
                    .replace(/{tenantId}/g, folderData.tenantId)
                    .replace(/{folderVersionId}/g, folderVersionId)
                    .replace(/{folderId}/g, folderData.folderId)
                    .replace(/{folderType}/g, folderData.folderType)
                    .replace(/{FolderViewMode}/g, folderData.FolderViewMode);


        //If view button is clicked mode needs to be "false" else "true"
        url = url.replace(/{mode}/g, mode) + "&isPopup=true";
        window.location.href = url;
    }

    function getMaxFolderVersion(majorFolderVersionList) {
        var max = majorFolderVersionList.sort(function (a, b) {
            return new Date(b.EffectiveDate) - new Date(a.EffectiveDate);
        });
        //Max effective date
        var latestMajerVersion = max[0];
        //Get Same effective date list
        var sameEffectiveDateList = majorFolderVersionList.filter(function (date) {
            return date.EffectiveDate == latestMajerVersion.EffectiveDate;
        });
        //2017_4.0
        //Get latest version number if More than one effective date present
        if (sameEffectiveDateList.length > 1) {
            latestMajerVersion = sameEffectiveDateList[0];
            var maxVersionNumber = latestMajerVersion.FolderVersionNumber.split('_')[1];
            var maxNumber = parseInt(maxVersionNumber);
            $.each(sameEffectiveDateList, function (idx, data) {
                var versionNumber = data.FolderVersionNumber.split('_')[1];
                var number = parseInt(versionNumber);
                if (number > maxNumber) {
                    latestMajerVersion = data;
                    maxNumber = number;
                }
            });
        }

        return latestMajerVersion;
    }

    function init() {

        //register dialog for grid row add/edit
        $(elementIDs.versionHistoryDialog).dialog({
            autoOpen: false,
            resizable: false,
            closeOnEscape: true,
            height: 'auto',
            width: 850,
            modal: true,
            position: ['middle', 100],
        });


        $(elementIDs.minorEffectiveDateDialog).dialog({
            autoOpen: false,
            resizable: false,
            closeOnEscape: true,
            height: 'auto',
            width: 500,
            modal: true,
            position: 'center'
        });

        $(elementIDs.minorEffectiveDateDialog).find(elementIDs.minorEffectiveDate).datepicker({
            dateFormat: 'mm/dd/yy',
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-61:c+20',
            showOn: "both",
            buttonImage: Icons.CalenderIcon,
            buttonImageOnly: true
        });

        if (isApplicableTeamsRequired == true) {
            $(elementIDs.checkAllId).click(function (event) {
                if (this.checked) {
                    $('.applicableTeamsCheckbox').each(function () {
                        this.checked = true;
                    });
                } else {
                    $('.applicableTeamsCheckbox').each(function () {
                        if (!($(this).is(":disabled"))) {
                            this.checked = false;
                        }
                    });
                }
            });
        }

        $(elementIDs.minorEffectiveDateDialog).find('.applicableTeamsCheckbox').each(function () {
            $(this).click(function (event) {
                if (!$(this).is(':checked')) {
                    $(elementIDs.checkAllId)[0].checked = false;
                }
            });
        });

        $(elementIDs.versionHistoryButton).on('click', function () {
            versionHistory.loadVersionHistory();
        });

        $(elementIDs.newButton).on('click', function () {
            //Get all major version shown in major Version Grid.
            var majorVersions = $(elementIDs.majorVersionGridJQ).jqGrid('getGridParam', 'data');
            var folderVersionID = majorVersions[0].FolderVersionId;
            var categoryId = majorVersions[0].CategoryId;
            var catId = '';//majorVersions[0].CatID;
            versionHistory.showEffectiveDateDialog(categoryId, catId);
        });

        $(elementIDs.retroButton).on('click', function () {
            retroAccountDialog.show($(elementIDs.majorVersionGridJQ).jqGrid('getGridParam', 'data'));
        });

        $(elementIDs.changeSummaryButton).on('click', function () {

            var url = '/FolderVersionReport/ChangeSummaryReport';
            window.location = url;

        });

        $(elementIDs.minorEffectiveDateButton).on('click', function () {
            var newEffectiveDate = $(elementIDs.minorEffectiveDateDialog + ' input').val();
            $(elementIDs.minorEffectiveDateDialog + ' div').removeClass('has-error');
            var majorFolderVersionList = $(elementIDs.majorVersionGridJQ).jqGrid('getGridParam', 'data');
            var maxMajorVersionData = getMaxFolderVersion(majorFolderVersionList);
            var maxMajorVersionEffDate = getFormattedDate(new Date(maxMajorVersionData.EffectiveDate));
            var checkEffectiveDate = majorFolderVersionList != null &&
                                        new Date(newEffectiveDate) < new Date(maxMajorVersionEffDate);
            var ctagory = $(elementIDs.ctegory).val();
            if (newEffectiveDate == '') {
                $(elementIDs.minorEffectiveDateDialog).find(elementIDs.minorEffectiveDate).parent().addClass('has-error');
                $(elementIDs.minorEffectiveDateDialog + ' .help-block').text(Common.effectiveDateRequiredMsg);
            }
            else if (maxMajorVersionData != null && checkEffectiveDate) {
                $(elementIDs.minorEffectiveDateDialog).find(elementIDs.minorEffectiveDate).parent().addClass('has-error');
                //$(elementIDs.minorEffectiveDateDialog + ' .help-block').text('Effective Date should be greater or equal to major Version "' + maxMajorVersionData.FolderVersionNumber + '" Effective Date');
                $(elementIDs.newversioneffectivedate).text('Effective Date should be greater or equal to major Version "' + maxMajorVersionData.FolderVersionNumber + '" Effective Date');
            }
            else if (ctagory == 0) {
                $(elementIDs.minorEffectiveDateDialog).find(elementIDs.ctegory).parent().addClass('has-error');
                //$(elementIDs.minorEffectiveDateDialog + ' .help-block').text(Common.categoryRequiredMsg);
                $(elementIDs.newversionCategoryDDLSpan).text(Common.categoryRequiredMsg);
            }
            else {
                var effectiveDateMessage = isValidEffectiveDate(newEffectiveDate);
                var applicableTeamsIDs = new Array();
                if (isApplicableTeamsRequired == true) {
                    $(elementIDs.minorEffectiveDateDialog).find('.applicableTeamsCheckbox').each(function () {
                        if ($(this).is(':checked'))
                            applicableTeamsIDs.push(parseInt($(this).attr('id')));
                    });
                }
                if (applicableTeamsIDs.length > 0 || isApplicableTeamsRequired == false) {
                    if (maxMajorVersionData != null && effectiveDateMessage == "") {
                        var url = URLs.addMinorFolderVersion;
                        var categoryId = $(elementIDs.categoryDDLJQ).val();
                        var catId = '';//$(elementIDs.catIDJQ).val();

                        var minorVersionToCreate = {
                            tenantId: folderData.tenantId,
                            folderId: folderData.folderId,
                            folderVersionId: maxMajorVersionData.FolderVersionId,
                            comments: maxMajorVersionData.Comments,
                            versionNumber: maxMajorVersionData.FolderVersionNumber,
                            effectiveDate: newEffectiveDate,
                            isRelease: true,
                            consortiumID: null,
                            categoryId: categoryId,
                            catID: catId
                        };

                        var promise = ajaxWrapper.postJSON(url, minorVersionToCreate);
                        promise.done(addSuccess);
                        promise.fail(showError);
                    } else {
                        $(elementIDs.minorEffectiveDateDialog).find(elementIDs.minorEffectiveDate).parent().addClass('has-error');
                        $(elementIDs.minorEffectiveDateDialog + ' .help-block').text(effectiveDateMessage);
                    }
                }
                else {
                    messageDialog.show(WorkFlowStateMessages.atleastOneApplicableTeamMsg);
                    $(elementIDs.minorEffectiveDateDialog).find('.applicableTeamsCheckbox').each(function () {
                        $(this).parent().addClass("has-error");
                    });
                }
            }

        });

    }

    function fillCategoryDDL(selectedIndex) {

        var defaultCategoryText = "-- Select Category --";
        //To refresh dropdown of Consortium names
        $(elementIDs.categoryDDLJQ).empty();
        $(elementIDs.categoryDDLJQ).append("<option value=0>" + defaultCategoryText + "</option>");
        //ajax call for drop down list of account names
        var majorVersions = $(elementIDs.majorVersionGridJQ).jqGrid('getGridParam', 'data');
        var folderVersionID = majorVersions[0].FolderVersionId;
        var url = URLs.categoryList.replace(/{folderVersionID}/g, folderVersionID);
        var promise = ajaxWrapper.getJSON(url);
        //var promise = ajaxWrapper.getJSON(URLs.categoryList);
        promise.done(function (names) {
            var isNewGroup = "false";
            var isJaaNewGroup = "false";
            var changeGroupIndex;
            var JAAChangeGroupIndex;
            for (i = 0; i < names.length; i++) {
                // For creating New Version the group NewGroup and JAA NewGroup should not be include , Refer ANT-13 and ANT-91
                //if (names[i].FolderVersionCategoryName != AntWFCategoriesGroups.NewGroup && names[i].FolderVersionCategoryName != AntWFCategoriesGroups.JAANewGroup) {
                // If Role is TPA Analyst then include only JAA Change Request, else for all other roles (SuperUser,EBA Analyst) should include all
                //if (vbRole == Role.TPAAnalyst) {
                //if (names[i].FolderVersionCategoryName == AntWFCategoriesGroups.JAAChangeRequest) {
                //        $(elementIDs.categoryDDLJQ).append("<option value=" + names[i].FolderVersionCategoryID + ">" + names[i].FolderVersionCategoryName + "</option>");
                //}
                //}
                //else {
                $(elementIDs.categoryDDLJQ).append("<option value=" + names[i].FolderVersionCategoryID + ">" + names[i].FolderVersionCategoryName + "</option>");
                //}
                //}
                //if (selectedIndex == names[i].FolderVersionCategoryID && names[i].FolderVersionCategoryName == AntWFCategoriesGroups.NewGroup) {
                //    isNewGroup = "true";
                //}
                //if (isNewGroup == "true" && names[i].FolderVersionCategoryName == AntWFCategoriesGroups.ChangeRequest) {
                //    changeGroupIndex = names[i].FolderVersionCategoryID;
                //}
                //if (selectedIndex == names[i].FolderVersionCategoryID && names[i].FolderVersionCategoryName == AntWFCategoriesGroups.JAANewGroup) {
                //    isJaaNewGroup = "true";
                //}
                //if (isJaaNewGroup == "true" && names[i].FolderVersionCategoryName == AntWFCategoriesGroups.JAAChangeRequest) {
                //    JAAChangeGroupIndex = names[i].FolderVersionCategoryID;
                //}
            }
            //if (isNewGroup == "true") {
            //    $(elementIDs.categoryDDLJQ).val(changeGroupIndex);
            //}
            //else if (isJaaNewGroup == "true") {
            //    $(elementIDs.categoryDDLJQ).val(JAAChangeGroupIndex);
            //}
            //else {
            $(elementIDs.categoryDDLJQ).val(selectedIndex);
            //}

        });
        promise.fail(showError);
    }

    $(document).ready(function () {
        //initialize the dialog after this js are loaded
        init();
    });

    return {
        loadVersionHistory: function () {
            $(elementIDs.versionHistoryDialog).dialog('option', 'title', 'Version Management');
            $(elementIDs.versionHistoryDialog).dialog("open");
            setInputControlsState();
            loadMajorVersionGrid();
            loadMinorVersionGrid();
        },

        showEffectiveDateDialog: function (CategoryID, CatID) {
            //open the dialog - uses jqueryui dialog:   
            $(elementIDs.minorEffectiveDateDialog + ' div').removeClass('has-error');
            $(elementIDs.minorEffectiveDateDialog).dialog('option', 'title', 'Minor Version');
            if (folderData.folderType == "MasterList") {
                if (masterListEffectiveDate != undefined) {
                    $(elementIDs.minorEffectiveDateDialog + ' .hasDatepicker').datepicker('setDate', masterListEffectiveDate).datepicker('disable');
                    $(elementIDs.minorEffectiveDateDialog + ' .help-block')
                   .text('Effective Date of Master List should always remain 01/01/2014');
                }
            }
            else {
                // $(elementIDs.minorEffectiveDateDialog + ' .help-block')
                //.text(FolderVersion.addMinorVersionWithEffectiveDateValiadteMsg);
                $(elementIDs.newversioneffectivedate).text(FolderVersion.addMinorVersionWithEffectiveDateValiadteMsg);
            }
            $(elementIDs.applicableTeamsContainer).hide();
            $(elementIDs.minorEffectiveDateDialog).dialog("open");
            $(elementIDs.minorEffectiveDate).val('');
            fillCategoryDDL(0);
            //$(elementIDs.catIDJQ).val(CatID);
        }
    };
}();


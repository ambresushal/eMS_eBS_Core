var versionHistoryML = function () {

    var URLs = {
        majorVersionList: '/FolderVersion/GetVersionHistoryML?folderId={folderId}&tenantId=1&versionType=Major',
        minorVersionList: '/FolderVersion/GetVersionHistoryML?folderId={folderId}&tenantId=1&versionType=Minor',
        isAnyFolderVersionInProgress: '/FolderVersion/IsAnyFolderVersionInProgress?folderId={folderId}&tenantId=1',
        addMinorFolderVersion: '/FolderVersion/AddMinorFolderVersion',
        folderVersionDetailsNonPortfolioBasedAccount: '/FolderVersion/GetNonPortfolioBasedAccountFoldersML?tenantId={tenantId}&folderVersionId={folderVersionId}&folderId={folderId}&folderType={folderType}&mode={mode}',
        viewEditNolderVersionDetailsNonPortfolioBasedAccount: '/FolderVersion/ViewEditMLFolderVersion?tenantId={tenantId}&folderVersionId={folderVersionId}&folderId={folderId}&folderType={folderType}&mode={mode}',
        deleteFolderVersion: '/FolderVersion/DeleteFolderVersion',
        rollbackFolderVersion: '/FolderVersion/RollbackFolderVersion',
        accountSearch: '/ConsumerAccount/AccountSearch',
        isFolderVersionCanRollback: '/FolderVersion/IsFolderVersionCanRollback?tenantId={tenantId}&rollbackFolderVersionNumber={rollbackFolderVersionNumber}&inProgressMinorVersionNumber={inProgressMinorVersionNumber}',
        LockStatusUrl: '/FolderVersion/GetFolderLockStatus?tenantId=1&folderId={folderId}',
        DeleteSuccessRender: '/FolderVersion/IndexML?tenantId=1&folderType={folderType}',
        OverrideLockUrl: '/FolderVersion/OverrideFolderLock?tenantId=1&folderId={folderId}',
        ////Add Workflow States
        applicableTeamsAdd: '/FolderVersion/AddApplicableTeams',
        ////Gets the Consortium List
        consortiumList: '/Consortium/GetConsortiumForDropdown?tenantId=1',
        categoryList: '/Settings/GetFolderVersionCategoryForDropdown?tenantId=1&isPortfolio={isPortfolio}&folderVersionID={folderVersionID}',
        getFolderVersion: '/FolderVersion/GetFolderVersion?folderVersionID={folderVersionID}',
        releaseMLVersion: '/FolderVersion/ReleaseMLVersion',
        cascadeReleaseMLVersion: '/FolderVersion/CascadeReleaseMLVersion'
    };

    var elementIDs = {
        vesrionHistoryDialogML: '#vesrionHistoryDialogML',
        majorVersionML: 'majorVersionML',
        majorVersionMLJQ: '#majorVersionML',
        minorVersionML: 'minorVersionML',
        minorVersionMLJQ: '#minorVersionML',
        versionHistoryButtonMLJQ: '#versionHistoryButtonML',
        newVersionHistoryMLJQ: '#newVersionHistoryML',
        retroVersionHistoryMLJQ: '#retroVersionHistoryML',
        changeSummaryMLJQ: '#changeSummaryML',
        minorEffectiveDateDialogML: "#minoreffectivedatedialogML",
        minorEffectiveDateML: "#effectivedateML",
        minorEffectiveDateButtonML: "#minoreffectivebtnML",
        minorVersionRollbackButton: "#btnMinorVersionRollbackML",
        minorVersionDeleteButton: "#btnMinorVersionDeleteML",
        minorVersionReleaseButton: "#btnMinorVersionReleaseML",

        minorVersionViewButton: "#btnMinorVersionViewML",
        minorVersionEditButton: "#btnMinorVersionEditML",
        btnUnlockJQ: '#btnUnlock',
        btnMinorVersionCascadeML: '#btnMinorVersionCascadeML',
        checkAllId: '#checkAll',
        consortiumDDLML: '#consortiumDDLML',
        categoryDDLMLJQ: '#categoryDDLML',
        catIDMLJQ: '#catIDTXTML',
        consortiumAutoCompltDDL: "#consortiumAutoCompltDDL",
        categoryAutoCompltDDL: "#categoryAutoCompltDDL",
        addCommentMLCascadeDialogJS: "#addCommentMLCascadeDialog",
        txtMLCascadeCommentJS: "#txtMLCascadeComment",
    };

    function loadMajorVersionGrid() {
        //set column list for grid
        var colArray = ['', '', 'Major Version', 'Folder Effective Date', 'Status', 'User', 'Comments'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'FolderVersionId', index: 'FolderVersionId', key: true, hidden: true, search: false });
        colModel.push({ name: 'FolderId', index: 'FolderId', hidden: true, search: false });
        colModel.push({ name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: false, width: '90' });
        colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, align: 'center', formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions });
        colModel.push({ name: 'Status', index: 'Status', editable: false });
        colModel.push({ name: 'User', index: 'User', editable: false });
        colModel.push({ name: 'Comments', index: 'Comments', editable: false });

        //clean up the grid first - only table element remains after this
        $(elementIDs.majorVersionMLJQ).jqGrid('GridUnload');
        var url = URLs.majorVersionList.replace(/\{folderId\}/g, folderData.folderId);
        //adding the pager element
        $(elementIDs.majorVersionMLJQ).parent().append("<div id='p" + elementIDs.majorVersionML + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.majorVersionMLJQ).jqGrid({
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
            pager: '#p' + elementIDs.majorVersionML,
            sortname: 'MajorVersion',
            altclass: 'alternate',
            gridComplete: function () {
                var grid = this;
                var allRowsData = $(grid).getRowData();
                if (allRowsData.length <= 0) {
                    $('#btnMajorVersionViewML').addClass('ui-state-disabled');
                }

                //to check for claims..  
                var objMap = {
                    view: "#btnMajorVersionViewML"
                };
                var claimsUrl = window.location.pathname;
                //to check for Masterlist permissions..
                var url = window.location.href;
                var check = url.split("&");
                var sectioName = "/FolderVersion/IndexML"
                checkApplicationClaims(claims, objMap, sectioName);
            },
        });

        var pagerElement = '#p' + elementIDs.majorVersionML;
        //remove default buttons
        $(elementIDs.majorVersionMLJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        // add filter toolbar to the top
        $(elementIDs.majorVersionMLJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });

        //View button in footer of grid 
        $(elementIDs.majorVersionMLJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-document', title: 'View', id: 'btnMajorVersionViewML',
               onClickButton: function () {
                   var rowId = $(this).getGridParam('selrow');
                   if (rowId !== undefined && rowId !== null) {
                       var row = $(this).getRowData(rowId);

                       if (folderData.isPortfolio == "True") {
                           loadPortfolioBasedAccount(row.FolderVersionId, false);
                       }
                       else {
                           loadNonPortfolioBasedAccount(row.FolderVersionId, false);
                       }

                   } else {
                       messageDialog.show(FolderVersion.selectRowToView);
                   }
               }
           });
    }
    var versionToCascadeRelease;
    function loadMinorVersionGrid() {
        //set column list for grid
        var colArray = ['', '', 'Minor Version', 'Folder Effective Date', 'Baseline Date', 'State', '', 'User', 'Comments', 'WFStateName'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'FolderVersionId', index: 'FolderVersionId', key: true, hidden: true, search: false });
        colModel.push({ name: 'FolderId', index: 'FolderId', hidden: true, search: false });
        colModel.push({ name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: false, width: '90' });
        colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, align: 'center', formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions });
        colModel.push({ name: 'AddedDate', index: 'AddedDate', editable: false, align: 'center', formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions });
        colModel.push({ name: 'FolderVersionStateName', index: 'FolderVersionStateName', editable: false });
        colModel.push({ name: 'VersionType', index: 'VersionType', editable: false, hidden: true });
        colModel.push({ name: 'User', index: 'User', editable: false });
        colModel.push({ name: 'Comments', index: 'Comments', editable: false });
        colModel.push({ name: 'WFStateName', index: 'WFStateName', editable: false, hidden: true });

        //clean up the grid first - only table element remains after this
        $(elementIDs.minorVersionMLJQ).jqGrid('GridUnload');
        var currentInstance = this;
        var url = URLs.minorVersionList.replace(/\{folderId\}/g, folderData.folderId);;
        //adding the pager element
        $(elementIDs.minorVersionMLJQ).parent().append("<div id='p" + elementIDs.minorVersionML + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.minorVersionMLJQ).jqGrid({
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
            pager: '#p' + elementIDs.minorVersionML,
            sortname: 'MinorVersion',
            altclass: 'alternate',
            gridComplete: function () {
                //if (folderData.RoleId == 24) {
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
                        $(elementIDs.minorVersionReleaseButton).addClass('ui-state-disabled');
                    } else {
                        $(elementIDs.minorVersionRollbackButton).removeClass('ui-state-disabled');
                        $(elementIDs.minorVersionDeleteButton).removeClass('ui-state-disabled');
                        $(elementIDs.minorVersionReleaseButton).removeClass('ui-state-disabled');
                    }
                } else {
                    $(elementIDs.minorVersionRollbackButton).addClass('ui-state-disabled');
                    $(elementIDs.minorVersionDeleteButton).addClass('ui-state-disabled');
                    $(elementIDs.minorVersionEditButton).addClass('ui-state-disabled');
                    $(elementIDs.minorVersionViewButton).addClass('ui-state-disabled');
                    $(elementIDs.minorVersionReleaseButton).addClass('ui-state-disabled');
                }

                //To check if section is from required list to be transmitted/translated


                //to check for claims..  
                var objMap = {
                    view: elementIDs.minorVersionViewButton,
                    edit: elementIDs.minorVersionEditButton,
                    rollback: elementIDs.minorVersionRollbackButton,
                    remove: elementIDs.minorVersionDeleteButton,
                    release: elementIDs.minorVersionReleaseButton
                };
                var claimsUrl = window.location.pathname;
                //to check for Masterlist permissions..
                var url = window.location.href;
                var check = url.split("&");
                var sectioName = "/FolderVersion/IndexML"
                checkApplicationClaims(claims, objMap, sectioName);
                //}
                //else {
                //    $(elementIDs.minorVersionRollbackButton).addClass('ui-state-disabled');
                //    $(elementIDs.newVersionHistoryMLJQ).addClass('ui-state-disabled');
                //    $(elementIDs.retroVersionHistoryMLJQ).addClass('ui-state-disabled');

                //    $(elementIDs.minorVersionEditButton).addClass('ui-state-disabled');
                //    $(elementIDs.minorVersionDeleteButton).addClass('ui-state-disabled');
                //    $(elementIDs.minorVersionReleaseButton).addClass('ui-state-disabled');
                //    //$(elementIDs.changeSummaryMLJQ).addClass('ui-state-disabled');
                //    $(elementIDs.minorVersionViewButton).removeClass('ui-state-disabled');

                //    $('#btnMajorVersionViewML').removeClass('ui-state-disabled');
                //}
            },
            onSelectRow: function (id) {
                //if (folderData.RoleId == 24) {
                var inProgressVersionNumber = null;
                var allRowsData = $(elementIDs.minorVersionMLJQ).getRowData();
                if (allRowsData.length > 0) {
                    $.each(allRowsData, function (i, item) {
                        if (item.FolderVersionStateName == FolderVersion.inProgress && item.VersionType == FolderVersion.new) {
                            inProgressVersionNumber = item.FolderVersionNumber;
                        }
                    });
                }
                var row = $(this).getRowData(id);

                //if (row.FolderVersionStateName == FolderVersion.inProgress) {
                //    if (row.VersionType == FolderVersion.new) {
                //        $(elementIDs.minorVersionRollbackButton).addClass('ui-state-disabled');
                //    }
                //    $(elementIDs.minorVersionReleaseButton).removeClass('ui-state-disabled');
                //    $(elementIDs.minorVersionDeleteButton).removeClass('ui-state-disabled');
                //    $(elementIDs.minorVersionEditButton).removeClass('ui-state-disabled');

                //}
                //else if (row.FolderVersionStateName == FolderVersion.inProgressBlocked) {
                //    $(elementIDs.minorVersionRollbackButton).addClass('ui-state-disabled');
                //    $(elementIDs.minorVersionReleaseButton).addClass('ui-state-disabled');
                //    $(elementIDs.minorVersionDeleteButton).addClass('ui-state-disabled');
                //    $(elementIDs.minorVersionEditButton).addClass('ui-state-disabled');
                //}
                //else {
                //if (row.FolderVersionStateName == FolderVersion.baselined && inProgressVersionNumber != null) {
                var url = URLs.isFolderVersionCanRollback.replace(/{tenantId}/g, folderData.tenantId)
                    .replace(/{rollbackFolderVersionNumber}/g, row.FolderVersionNumber)
                    .replace(/{inProgressMinorVersionNumber}/g, inProgressVersionNumber);
                var promise = ajaxWrapper.getJSON(url, false);
                promise.done(function (x) {
                    //$(elementIDs.minorVersionReleaseButton).addClass('ui-state-disabled');
                    //$(elementIDs.minorVersionDeleteButton).addClass('ui-state-disabled');
                    //$(elementIDs.minorVersionEditButton).addClass('ui-state-disabled');
                    if (x.Result == ServiceResult.SUCCESS && vbRole != Role.ProductSME && vbRole != Role.ProductDesignerLevel1 && vbRole != Role.ProductDesignerLevel2) {
                        $(elementIDs.minorVersionRollbackButton).removeClass('ui-state-disabled');
                    } else {
                        //messageDialog.show(x.Items[0].Messages[0]);
                        $(elementIDs.minorVersionRollbackButton).addClass('ui-state-disabled');
                    }
                });
                //} else {
                //    $(elementIDs.minorVersionRollbackButton).addClass('ui-state-disabled');
                //    $(elementIDs.minorVersionDeleteButton).addClass('ui-state-disabled');
                //    $(elementIDs.minorVersionReleaseButton).addClass('ui-state-disabled');
                //    $(elementIDs.minorVersionEditButton).addClass('ui-state-disabled');
                //}
                //}



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
                var sectioName = "/FolderVersion/IndexML"
                checkApplicationClaims(claims, objMap, sectioName);
                //}
                //else {
                //    $(elementIDs.minorVersionRollbackButton).addClass('ui-state-disabled');
                //    $(elementIDs.newVersionHistoryMLJQ).addClass('disabled-button');
                //    $(elementIDs.retroVersionHistoryMLJQ).addClass('disabled-button');

                //    $(elementIDs.minorVersionEditButton).addClass('ui-state-disabled');
                //    $(elementIDs.minorVersionDeleteButton).addClass('ui-state-disabled');
                //    $(elementIDs.minorVersionReleaseButton).addClass('ui-state-disabled');
                //    //$(elementIDs.changeSummaryMLJQ).addClass('ui-state-disabled');
                //    $(elementIDs.minorVersionViewButton).removeClass('ui-state-disabled');

                //    $('#btnMajorVersionViewML').removeClass('ui-state-disabled');

                //}
                if (expBuilder.checkMLCascade(currentInstance.folderData.folderType, currentInstance.folderData.folderName) == true) {
                    if (row.FolderVersionStateName == FolderVersion.inProgress) {
                        $(elementIDs.btnMinorVersionCascadeML).removeClass('ui-state-disabled');
                    }
                    else {
                        $(elementIDs.btnMinorVersionCascadeML).addClass('ui-state-disabled');
                    }
                }
            }
        });
        var pagerElement = '#p' + elementIDs.minorVersionML;
        //remove default buttons
        $(elementIDs.minorVersionMLJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        // add filter toolbar to the top
        $(elementIDs.minorVersionMLJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });

        //Rollback button in footer of grid 
        $(elementIDs.minorVersionMLJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-arrowreturn-1-w', title: 'Rollback', id: 'btnMinorVersionRollbackML',
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
        $(elementIDs.minorVersionMLJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-document', title: 'View', id: 'btnMinorVersionViewML',
               onClickButton: function () {
                   var rowId = $(this).getGridParam('selrow');
                   if (rowId !== undefined && rowId !== null) {
                       var row = $(this).getRowData(rowId);

                       if (folderData.isPortfolio == "True") {
                           loadPortfolioBasedAccount(row.FolderVersionId, false);
                       }
                       else {
                           loadNonPortfolioBasedAccount(row.FolderVersionId, false);
                       }
                   } else {
                       messageDialog.show(FolderVersion.selectRowToView);
                   }
               }
           });

        //Edit button in footer of grid 
        $(elementIDs.minorVersionMLJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-pencil', title: 'Edit', id: 'btnMinorVersionEditML',
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
        $(elementIDs.minorVersionMLJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-trash', title: 'Delete', id: 'btnMinorVersionDeleteML',
               onClickButton: function () {
                   var rowId = $(this).getGridParam('selrow');
                   if (rowId !== undefined && rowId !== null) {
                       var row = $(this).getRowData(rowId);
                       var minorGridData = $(this).getRowData();
                       var majorGridData = $(elementIDs.majorVersionMLJQ).jqGrid('getGridParam', 'data');;
                       if (row.FolderVersionStateName == FolderVersion.inProgress) {

                           if (minorGridData.length == 1 && majorGridData.length < 1) {
                               messageDialog.show(FolderVersion.cannotPerformDelete);
                           }
                               //If the product is in translation or transmission then one should not allow the folder version to delete.
                           else if (folderManager.getInstance(null, row.FolderVersionId, row.FolderId, true).getFolderInstance().checkForStatusUpdateDisableButton()) {
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

        //Release In Prgress versions
        $(elementIDs.minorVersionMLJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon ui-icon-locked', title: 'Release', id: 'btnMinorVersionReleaseML',
               onClickButton: function () {
                   var rowId = $(this).getGridParam('selrow');
                   if (rowId !== undefined && rowId !== null) {
                       var row = $(this).getRowData(rowId);
                       var minorGridData = $(this).getRowData();
                       if (row.FolderVersionStateName == FolderVersion.inProgress) {

                           var versionToRelease = {
                               folderVersionId: row.FolderVersionId,
                               tenantId: folderData.tenantId,
                               folderId: folderData.folderId,
                               versionNumber: folderData.versionNumber
                           };
                           
                           confirmDialog.show(FolderVersion.confirmReleaseFolderVersion, function () {
                               confirmDialog.hide();
                               var url = URLs.releaseMLVersion;
                               var promise = ajaxWrapper.postJSON(url, versionToRelease);
                               promise.done(releaseSuccess);
                               promise.fail(showError);
                           });



                       } else {
                           messageDialog.show(FolderVersion.selectInProgress);
                       }
                   }
                   else {
                       messageDialog.show(FolderVersion.selectRowToRelease);
                   }
               }
           });
        var expBuilder = new expressionBuilder();
        
        if (expBuilder.checkMLCascade(currentInstance.folderData.folderType, currentInstance.folderData.folderName) == true) {
            $(elementIDs.minorVersionMLJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon ui-icon-arrowrefresh-1-e', title: 'Cascade', id: 'btnMinorVersionCascadeML',
                onClickButton: function () {

                    var rowId = $(this).getGridParam('selrow');
                    if (rowId !== undefined && rowId !== null) {
                        var row = $(this).getRowData(rowId);
                        var minorGridData = $(this).getRowData();
                        if (row.FolderVersionStateName == FolderVersion.inProgress) {
                            versionToCascadeRelease = null;
                            versionToCascadeRelease = {
                                folderVersionId: row.FolderVersionId,
                                tenantId: folderData.tenantId,
                                folderId: folderData.folderId,
                                versionNumber: folderData.versionNumber,
                                CommentText: ""
                            };

                            $(elementIDs.addCommentMLCascadeDialogJS).dialog('option', 'title', "Add Cascade Comment");
                            $(elementIDs.addCommentMLCascadeDialogJS).dialog("open");
                            $(elementIDs.addCommentMLCascadeDialogJS + ' p').text(FolderVersion.confirmCascadeReleaseFolderVersion);

                            $(elementIDs.addCommentMLCascadeDialogJS + ' button').unbind().on('click', function () {
                                var cascadeCommentText = $(elementIDs.txtMLCascadeCommentJS).val().trim();
                                if (cascadeCommentText == "") {
                                    $(elementIDs.addCommentMLCascadeDialogJS + ' .help-block').text(FolderVersion.cascadeCommentEmptyMsg);
                                }
                                else {
                                    $(elementIDs.addCommentMLCascadeDialogJS + ' .help-block').text('');
                                    var url = URLs.cascadeReleaseMLVersion;
                                    versionToCascadeRelease.CommentText = cascadeCommentText;
                                    var promise = ajaxWrapper.postJSON(url, versionToCascadeRelease);
                                    promise.done(releaseCascadeSuccess);
                                    promise.fail(showError);
                                }
                            });

                        }
                    }


                }
            });
            var expBuilder = new expressionBuilder();
            if (expBuilder.checkMLCascade(currentInstance.folderData.folderType, currentInstance.folderData.folderName) == true) {
                $(elementIDs.btnMinorVersionCascadeML).addClass('ui-state-disabled');
            }
        }
    }

    //ajax success callback
    function addSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {

            $(elementIDs.minorEffectiveDateDialogML).dialog("close");
            messageDialog.show(FolderVersion.folderVersionCreatedSuccess);
            setInputControlsState();
            loadMinorVersionGrid();
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
            $(elementIDs.minorEffectiveDateDialogML).dialog("close");
            messageDialog.show(FolderVersion.folderVersionDeletedSuccess);
            setInputControlsState();
            loadMinorVersionGrid();

            var url = URLs.DeleteSuccessRender.replace(/\{folderType\}/g, folderData.folderName);
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
            $(elementIDs.minorEffectiveDateDialogML).dialog("close");
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

    //ajax success callback
    function releaseSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            //$(elementIDs.vesrionHistoryDialogML).dialog("close");
            messageDialog.show(FolderVersion.folderVersionReleaseSuccess);
            loadMinorVersionGrid();
            loadMajorVersionGrid();
            setInputControlsState();
            $(elementIDs.vesrionHistoryDialogML).on("dialogclose", function (event, ui) {
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

    function releaseCascadeSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(FolderVersion.folderVersionCascadeReleaseSuccess);
            loadMinorVersionGrid();
            loadMajorVersionGrid();
            setInputControlsState();
            $(elementIDs.addCommentMLCascadeDialogJS).dialog("close");
            $(elementIDs.vesrionHistoryDialogML).on("dialogclose", function (event, ui) {
                var url = window.location.href.replace('mode=true', 'mode=false');
                window.location.href = url;
            });
        }
        else if (xhr.Result === ServiceResult.FAILURE) {
            //messageDialog.show(xhr.Items[0].Messages[0]);
            var url = '/FolderVersion/CascadeMLVersion';//URLs.rollbackFolderVersion;
            //if (xhr.Items != undefined && xhr.Items[0].Messages.indexOf('Master List Cascade cannot be queued since one or more Folders are being edited as below') != -1) {
            //    console.log(str2 + " found");
            //}
            confirmDialog.show("Cascaded plans are currently being edited. Are you sure you want to continue?", function () {
                confirmDialog.hide();
                var promise = ajaxWrapper.postJSON(url, versionToCascadeRelease, false);
                $(elementIDs.addCommentMLCascadeDialogJS).dialog("close");
                promise.done(releaseCascadeSuccess);
                promise.fail(showError);
            });
        }
        else {
            if(xhr.Result != undefined)
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
            var sectioName = "/FolderVersion/IndexML";
            var editFlag = checkUserPermissionNewVersionRetro(sectioName);
            var claimsUrl = window.location.pathname;
            if (editFlag) {
                if (inProgress) {
                    $(elementIDs.minorVersionRollbackButton).addClass('ui-state-disabled');
                    $(elementIDs.newVersionHistoryMLJQ).attr('disabled', 'disabled');
                    $(elementIDs.retroVersionHistoryMLJQ).attr('disabled', 'disabled');
                } else {
                    $(elementIDs.newVersionHistoryMLJQ).removeAttr('disabled', 'disabled');
                    $(elementIDs.retroVersionHistoryMLJQ).removeAttr('disabled', 'disabled');
                    $(elementIDs.minorVersionRollbackButton).removeClass('ui-state-disabled');
                }
            }
            else {
                $(elementIDs.minorVersionRollbackButton).addClass('ui-state-disabled');
                $(elementIDs.newVersionHistoryMLJQ).attr('disabled', 'disabled');
                $(elementIDs.retroVersionHistoryMLJQ).attr('disabled', 'disabled');
            }
        });
    }



    function loadPortfolioBasedAccount(folderVersionId, mode) {
        url = URLs.folderVersionDetailsNonPortfolioBasedAccount
                    .replace(/{tenantId}/g, folderData.tenantId)
                    .replace(/{folderVersionId}/g, folderVersionId)
                    .replace(/{folderId}/g, folderData.folderId)
                    .replace(/{folderType}/g, folderData.folderType)
                    .replace(/{mode}/g, folderData.mode);

        //If view button is clicked mode needs to be "false" else "true"
        url = url.replace(/{mode}/g, mode);
        window.location.href = url;
    }

    function viewEditNolderVersionDetailsNonPortfolioBasedAccount(folderVersionId, mode) {

        url = URLs.viewEditNolderVersionDetailsNonPortfolioBasedAccount
                    .replace(/{tenantId}/g, folderData.tenantId)
                    .replace(/{folderVersionId}/g, folderVersionId)
                    .replace(/{folderId}/g, folderData.folderId)
                    .replace(/{folderType}/g, folderData.folderType);

        //If view button is clicked mode needs to be "false" else "true"
        url = url.replace(/{mode}/g, mode);
        window.location.href = url;
    }

    function loadNonPortfolioBasedAccount(folderVersionId, mode) {

        url = URLs.folderVersionDetailsNonPortfolioBasedAccount
                    .replace(/{tenantId}/g, folderData.tenantId)
                    .replace(/{folderVersionId}/g, folderVersionId)
                    .replace(/{folderId}/g, folderData.folderId)
                    .replace(/{folderType}/g, folderData.folderType);

        //If view button is clicked mode needs to be "false" else "true"
        url = url.replace(/{mode}/g, mode);
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
    function fillConsortiumDDL(selectedIndex) {
        var selectedIndexValue;
        //To refresh dropdown of Consortium names
        $(elementIDs.consortiumDDLML).empty();
        $(elementIDs.consortiumDDLML).append("<option value=0>" + "--Select Consortium --" + "</option>");
        //ajax call for drop down list of account names
        var promise = ajaxWrapper.getJSON(URLs.consortiumList);
        promise.done(function (names) {
            for (i = 0; i < names.length; i++) {
                $(elementIDs.consortiumDDLML).append("<option value=" + names[i].ConsortiumID + ">" + names[i].ConsortiumName + "</option>");
                if (selectedIndex == names[i].ConsortiumID)
                    selectedIndexValue = names[i].ConsortiumName;
            }
            $(elementIDs.consortiumDDLML).val(selectedIndex);
            $(elementIDs.consortiumAutoCompltDDL).val(selectedIndexValue);
        });
        promise.fail(showError);
    }

    function fillCategoryDDL(selectedIndex) {
        var selectedIndexValue;
        var defaultCategoryText = "-- Select Category --";
        //To refresh dropdown of Consortium names
        $(elementIDs.categoryDDLMLJQ).empty();
        $(elementIDs.categoryDDLMLJQ).append("<option value=0>" + defaultCategoryText + "</option>");
        //ajax call for drop down list of account names
        var majorVersions = $(elementIDs.majorVersionMLJQ).jqGrid('getGridParam', 'data');
        var folderVersionID = majorVersions[0].FolderVersionId;
        var url = URLs.categoryList.replace(/{folderVersionID}/g, folderVersionID);
        var promise = ajaxWrapper.getJSON(url);
        promise.done(function (names) {
            for (i = 0; i < names.length; i++) {
                $(elementIDs.categoryDDLMLJQ).append("<option value=" + names[i].FolderVersionCategoryID + ">" + names[i].FolderVersionCategoryName + "</option>");
                if (selectedIndex == names[i].FolderVersionCategoryID)
                    selectedIndexValue = names[i].FolderVersionCategoryName;
            }
            $(elementIDs.categoryDDLMLJQ).val(selectedIndex);
            if (selectedIndex > 0)
                $(elementIDs.categoryAutoCompltDDL).val(selectedIndexValue);
        });
        promise.fail(showError);
    }

    $(document).ready(function () {
        //initialize the dialog after this js are loaded
        init();
    });

    function init() {

        //register dialog for grid row add/edit
        $(elementIDs.vesrionHistoryDialogML).dialog({
            autoOpen: false,
            resizable: false,
            closeOnEscape: true,
            height: 'auto',
            width: 850,
            modal: true,
            position: ['middle', 100],
        });

        $(elementIDs.addCommentMLCascadeDialogJS).dialog({
            autoOpen: false,
            resizable: false,
            closeOnEscape: true,
            height: 'auto',
            width: 500,
            modal: true,
            position: ['middle', 100],
        });

        $(elementIDs.minorEffectiveDateDialogML).dialog({
            autoOpen: false,
            resizable: false,
            closeOnEscape: true,
            height: 'auto',
            width: 500,
            modal: true,
            position: 'center'
        });

        $(elementIDs.minorEffectiveDateDialogML).find(elementIDs.minorEffectiveDateML).datepicker({
            dateFormat: 'mm/dd/yy',
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-61:c+20',
            showOn: "both",
            buttonImage: Icons.CalenderIcon,
            buttonImageOnly: true
        });

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

        $(elementIDs.minorEffectiveDateDialogML).find('.applicableTeamsCheckbox').each(function () {
            $(this).click(function (event) {
                if (!$(this).is(':checked')) {
                    $(elementIDs.checkAllId)[0].checked = false;
                }
            });
        });

        $(elementIDs.versionHistoryButtonMLJQ).on('click', function () {
            versionHistoryML.loadVersionHistory();
        });

        $(elementIDs.newVersionHistoryMLJQ).on('click', function () {
            var ConsortiumID; var CatID; var CategoryID;


            //Get all major version shown in major Version Grid.
            var majorVersions = $(elementIDs.majorVersionMLJQ).jqGrid('getGridParam', 'data');
            var folderVersionID = majorVersions[0].FolderVersionId;
            //To get consortium ID of the latest major version.
            //var url = URLs.getConsortium.replace(/{folderVersionID}/g, folderVersionID);
            var url = URLs.getFolderVersion.replace(/{folderVersionID}/g, folderVersionID);
            var promise = ajaxWrapper.getJSONSync(url);
            promise.done(function (data) {
                if (data != undefined || data != null) {
                    ConsortiumID = data.ConsortiumID;
                    CategoryID = data.CategoryID;
                    CatID = data.CatID
                }
            });
            versionHistoryML.showEffectiveDateDialog(ConsortiumID, CategoryID, CatID);
        });

        $(elementIDs.retroVersionHistoryMLJQ).on('click', function () {
            retroAccountDialog.show($(elementIDs.majorVersionMLJQ).jqGrid('getGridParam', 'data'));
        });

        $(elementIDs.changeSummaryMLJQ).on('click', function () {

            var url = '/FolderVersionReport/ChangeSummaryReport';
            window.location = url;

        });

        $(elementIDs.minorEffectiveDateButtonML).on('click', function () {
            var newEffectiveDate = $(elementIDs.minorEffectiveDateDialogML + ' input').val();
            $(elementIDs.minorEffectiveDateDialogML + ' div').removeClass('has-error');
            var majorFolderVersionList = $(elementIDs.majorVersionMLJQ).jqGrid('getGridParam', 'data');
            var maxMajorVersionData = getMaxFolderVersion(majorFolderVersionList);
            var maxMajorVersionEffDate = getFormattedDate(new Date(maxMajorVersionData.EffectiveDate));
            var checkEffectiveDate = majorFolderVersionList != null &&
                                        new Date(newEffectiveDate) < new Date(maxMajorVersionEffDate);
            if (newEffectiveDate == '') {
                $(elementIDs.minorEffectiveDateDialogML).find(elementIDs.minorEffectiveDateML).parent().addClass('has-error');
                $(elementIDs.minorEffectiveDateDialogML + ' .help-block').text(Common.effectiveDateRequiredMsg);
            }
            else if (maxMajorVersionData != null && checkEffectiveDate) {
                $(elementIDs.minorEffectiveDateDialogML).find(elementIDs.minorEffectiveDateML).parent().addClass('has-error');
                $(elementIDs.minorEffectiveDateDialogML + ' .help-block').text('Effective Date should be greater or equal to major Version "' + maxMajorVersionData.FolderVersionNumber + '" Effective Date');
            }
            else {
                var effectiveDateMessage = isValidEffectiveDate(newEffectiveDate);

                if (maxMajorVersionData != null && effectiveDateMessage == "") {
                    var url = URLs.addMinorFolderVersion;
                    //To get selected consortium ID
                    var consortiumId = null;
                    var categoryId = null;
                    var catId = null;

                    var minorVersionToCreate = {
                        tenantId: folderData.tenantId,
                        folderId: folderData.folderId,
                        folderVersionId: maxMajorVersionData.FolderVersionId,
                        comments: maxMajorVersionData.Comments,
                        versionNumber: maxMajorVersionData.FolderVersionNumber,
                        effectiveDate: newEffectiveDate,
                        isRelease: true,
                        consortiumID: consortiumId,
                        categoryId: categoryId,
                        catID: catId
                    };

                    var promise = ajaxWrapper.postJSON(url, minorVersionToCreate);
                    promise.done(addSuccess);
                    promise.fail(showError);
                } else {
                    $(elementIDs.minorEffectiveDateDialogML).find(elementIDs.minorEffectiveDateML).parent().addClass('has-error');
                    $(elementIDs.minorEffectiveDateDialogML + ' .help-block').text(effectiveDateMessage);
                }
            }
        });

        $(function () {
            $(elementIDs.consortiumDDLML).autoCompleteDropDown({ ID: 'consortiumAutoCompltDDL' });
            $(elementIDs.consortiumDDLML).click(function () {
                $(elementIDs.consortiumDDLML).toggle();
            });
            $(elementIDs.categoryDDLMLJQ).autoCompleteDropDown({ ID: 'categoryAutoCompltDDL' });
            $(elementIDs.categoryDDLMLJQ).click(function () {
                $(elementIDs.categoryDDLMLJQ).toggle();
            });
        });
    }
    return {
        loadVersionHistory: function () {
            $(elementIDs.vesrionHistoryDialogML).dialog('option', 'title', 'Version Management');
            $(elementIDs.vesrionHistoryDialogML).dialog("open");
            setInputControlsState();
            loadMajorVersionGrid();
            loadMinorVersionGrid();
        },

        showEffectiveDateDialog: function (ConsortiumID, CategoryID, CatID) {
            folderData
            //set the default value to blank.
            $(elementIDs.minorEffectiveDateDialogML).find('input#effectivedateML').val('');
            //open the dialog - uses jqueryui dialog:   
            $(elementIDs.minorEffectiveDateDialogML + ' div').removeClass('has-error');
            $(elementIDs.minorEffectiveDateDialogML).dialog('option', 'title', 'Minor Version');
            $(elementIDs.minorEffectiveDateDialogML + ' .help-block')
               .text(FolderVersion.addMinorVersionWithEffectiveDateValiadteMsg);
            $(elementIDs.minorEffectiveDateDialogML).dialog("open");
            $(elementIDs.minorEffectiveDate).val('');
            fillConsortiumDDL(ConsortiumID);
            fillCategoryDDL(CategoryID);
            $(elementIDs.catIDMLJQ).val(CatID);
        }
    };
}();


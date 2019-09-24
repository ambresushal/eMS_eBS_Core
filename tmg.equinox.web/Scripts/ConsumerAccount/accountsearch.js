var accountSearch = function () {
    var URLs = {
        //Gets the Portfolio Based Account List
        portfolioBasedAccountDetails: '/ConsumerAccount/GetPortfolioBasedAccountList?tenantId=1',
        //Gets the non-Portfolio Based Account List
        nonPortfolioBasedAccountDetails: '/ConsumerAccount/GetNonPortfolioBasedAccountist?tenantId=1&documentName={documentName}',
        //Get folders list
        getFolderVersionList: '/ConsumerAccount/GetFolderVersionList?tenantId={tenantId}&folderId={folderId}&accountName={accountName}&accountID={accountID}&documentName={documentName}',
        //Folder version details
        folderVersionDetailsPortfolioBasedAccount: '/FolderVersion/Index?tenantId={tenantId}&folderVersionId={folderVersionId}&folderId={folderId}&mode={mode}',

        folderVersionDetailsNonPortfolioBasedAccount: '/FolderVersion/GetNonPortfolioBasedAccountFolders?tenantId={tenantId}&folderVersionId={folderVersionId}&folderId={folderId}&mode={mode}&foldeViewMode={fldviewmode}',

        LockStatusUrl: '/FolderVersion/GetFolderLockStatus?tenantId={tenantId}&folderId={folderId}',

        OverrideLockUrl: '/FolderVersion/OverrideFolderLock?tenantId={tenantId}&folderId={folderId}',

        deleteNonPortfolioBasedFolder: '/FolderVersion/DeleteNonPortfolioBasedFolder?tenantId={tenantId}&folderID={folderId}',

        //Create Account Permissions
        folderVersionCreationPermission: '/FolderVersion/GetUserFolderVersionCreationPermission?isPortfolioSearch=false'

    };
    var elementIDs = {
        //table element for the Non Portfolio Grid 
        nonPortfolioBasedAccountDetailGrid: 'npadg',
        //with hash for use with jQuery
        nonPortfolioBasedAccountDetailGridJQ: '#npadg',
        //html element id for edit button on Non Portfolio grid
        buttonNonPortfolioEdit: 'btnNonPortfolioEdit',
        //with hash for use with jQuery
        buttonNonPortfolioEditJQ: '#btnNonPortfolioEdit',
        //html element id for view button on Non Portfolio grid
        buttonNonPortfolioView: 'btnNonPortfolioView',
        //with hash for use with jQuery
        buttonNonPortfolioViewJQ: '#btnNonPortfolioView',
        //html element id for copy button on Non Portfolio grid
        buttonNonPortfolioCopy: "btnNonPortfolioCopy",
        //with hash for use with jQuery
        buttonNonPortfolioCopyJQ: "#btnNonPortfolioCopy",
        //html element id for add button on NonPortfolio grid
        buttonNonPortfolioAdd: "btnNonPortfolioAdd",
        buttonNonPortfolioRemove: "btnNonPortfolioRemoveFolder",
        //with hash for use with jQuery
        buttonNonPortfolioAddJQ: "#btnNonPortfolioAdd",
        buttonNonPortfolioRemoveJQ: "#btnNonPortfolioRemoveFolder",
        btnUnlockJQ: '#btnUnlock',
        btnDocSearchJQ: '#btnDocSearch',
        searchDocJQ: '#searchDoc',
        btnDocClearJQ: '#btnDocClear',
        btnNonPortfolioAddJQ: '#btnNonPortfolioAdd',
        btnNonPortfolioEditJQ: '#btnNonPortfolioEdit',
        btnNonPortfolioCopyJQ: '#btnNonPortfolioCopy',
        btnNonPortfolioRemoveFolderJQ: '#btnNonPortfolioRemoveFolder'
    };

    if (typeof vbIsFolderLockEnable === "undefined") { this.isFolderLockEnable = false } else { this.isFolderLockEnable = vbIsFolderLockEnable };

    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
    //This is custom formatter for boolean values (i.e. true/false).
    //This will display images instead of true and flase in grid column.
    function booleanValueImageFmatter(cellvalue, options, rowObject) {
        if (cellvalue) {
            return '<span align="center" class="ui-icon ui-icon-check" style="margin:auto"></span>';
        }
        else {
            return '<span align="center" class="ui-icon ui-icon-close" style="margin:auto" ></span>';
        }
    }

    function loadPortfolioBasedAccountDetailGrid() {
        //set column list for grid
        var colArray = ['', '', '', 'Folder', 'Folder Version Number', 'Effective Date', 'Product Types', 'Product Names', 'Status', '', '', 'Market Segment', 'Portfolio', '', '', '', ''];

        //set column models
        var colModel = [];
        colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true });
        colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true });
        colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', key: true, hidden: true });
        colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left' });
        colModel.push({ name: 'VersionNumber', index: 'VersionNumber', editable: false, align: 'left' });
        colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'left' });
        colModel.push({ name: 'ProductType', index: 'ProductType', editable: false, align: 'left' });
        colModel.push({ name: 'ProductName', index: 'ProductName', editable: false, align: 'left' });
        colModel.push({ name: 'Status', index: 'Status', editable: false, align: 'left' });
        colModel.push({ name: 'CategoryID', index: 'CategoryID', hidden: true, editable: false, align: 'left' });
        colModel.push({ name: 'CategoryName', index: 'CategoryName', hidden: true, editable: false, align: 'left' });
        colModel.push({ name: 'MarketSegment', index: 'MarketSegment', editable: false, align: 'left' });
        colModel.push({ name: 'Portfolio', index: 'Portfolio', editable: false, align: 'left', formatter: booleanValueImageFmatter, search: false, sortable: false });
        colModel.push({ name: 'ApprovalStatusID', index: 'ApprovalStatusID', hidden: true });
        colModel.push({ name: 'FolderVersionStateID', index: 'FolderVersionStateID', hidden: true, editable: false });
        colModel.push({ name: 'FolderVersionCount', index: 'FolderVersionCount', hidden: true, editable: false });
        colModel.push({ name: 'IsExpanded', index: 'IsExpanded', hidden: true, editable: false });

        //clean up the grid first - only table element remains after this
        $(elementIDs.portfolioBasedAccountDetailGridJQ).jqGrid('GridUnload');
        //adding the pager element
        $(elementIDs.portfolioBasedAccountDetailGridJQ).parent().append("<div id='p" + elementIDs.portfolioBasedAccountDetailGrid + "'></div>");
        var url = URLs.portfolioBasedAccountDetails;
        $(elementIDs.portfolioBasedAccountDetailGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Portfolio Based Account',
            height: '175',
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.portfolioBasedAccountDetailGrid,
            sortname: 'FolderName',
            altclass: 'alternate',
            subGrid: true,
            subGridRowExpanded: function (subGridId, rowId) {
                $(this).jqGrid("setSelection", rowId);
                if (rowId !== undefined && rowId !== null) {
                    var row = $(this).getRowData(rowId);
                    var url = URLs.getFolderVersionList.replace(/{tenantId}/g, row.TenantID).replace(/{folderId}/g, row.FolderID).replace(/{accountName}/g, "").replace(/{accountID}/g, 0);
                    var subGridTableId, pager_id;
                    //table id of subgrid.
                    subGridTableId = subGridId + "_t";
                    subGridTableIdJQ = "#" + subGridTableId
                    pager_id = "p_" + subGridTableId;
                    //Set value as true for IsExpanded column of selected row of main grid. 
                    row.IsExpanded = true;
                    $(this).jqGrid("setRowData", rowId, row);
                    //created table tag.
                    $("#" + subGridId).html("<table id='" + subGridTableId + "' class='scroll'></table><div id='" + pager_id + "' class='scroll'></div>");
                    $(subGridTableIdJQ).jqGrid({
                        url: url,
                        datatype: 'json',
                        cache: false,
                        colNames: ['', '', '', 'Folder', 'Folder Version Number', 'Effective Date', '', '', '', '', '', ''],
                        colModel: [
                            { name: 'TenantID', index: 'TenantID', editable: false, align: 'left', hidden: true },
                            { name: 'AccountID', index: 'AccountID', editable: false, align: 'left', hidden: true },
                            { name: 'FolderVersionID', index: 'FolderVersionID', hidden: true, editable: false, align: 'left', hidden: true },
                            { name: 'FolderName', index: 'FolderName', width: 200, editable: false, align: 'left' },
                            { name: 'VersionNumber', index: 'VersionNumber', width: 200, editable: false, align: 'left' },
                            { name: 'EffectiveDate', index: 'EffectiveDate', width: 200, editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'left' },
                            { name: 'FolderID', index: 'FolderID', hidden: true },
                            { name: 'ApprovalStatusID', index: 'ApprovalStatusID', editable: false, align: 'left', hidden: true },
                            { name: 'MarketSegmentID', index: 'MarketSegmentID', editable: false, align: 'left', hidden: true },
                            { name: 'PrimaryContactID', index: 'PrimaryContactID', editable: false, align: 'left', hidden: true },
                            { name: 'FolderVersionStateID', index: 'FolderVersionStateID', editable: false, align: 'left', hidden: true },
                            { name: 'FolderVersionCount', index: 'FolderVersionCount', editable: false, align: 'left', hidden: true },
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
                            //To set first row of grid as selected.
                            var rows = $(this).getDataIDs();
                            if (rows.length > 0) {
                                $(this).jqGrid("setSelection", rows[0]);
                            }
                            //check application claims               
                            var objMap = {
                                view: "#" + subGridTableId + 'view',
                            };
                            checkApplicationClaims(claims, objMap, URLs.portfolioBasedAccountDetails);
                        }
                    })
                    var pagerElement = '#' + pager_id;
                    $(subGridTableIdJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
                    //remove paging
                    $(pagerElement).find(pagerElement + '_center').remove();
                    $(pagerElement).find(pagerElement + '_right').remove();

                    $(subGridTableIdJQ).jqGrid('navButtonAdd', pagerElement,
                    {
                        caption: '', buttonicon: 'ui-icon-document', title: 'View', id: subGridTableId + "view",
                        onClickButton: function () {
                            viewButtonClickForPortfolioGrid(subGridTableIdJQ);
                        }
                    });
                }
            },
            subGridRowColapsed: function (subGridId, rowId) {
                $(this).jqGrid("setSelection", rowId);
            },
            gridComplete: function () {
                rows = $(this).getDataIDs();
                if (rows && rows.length > 0)
                    $(this).jqGrid("setSelection", rows[0]);

                //to check for claims..              
                var objMap = {
                    edit: elementIDs.buttonPortfolioEditJQ,
                    view: elementIDs.buttonPortfolioEditJQ,
                };
                checkApplicationClaims(claims, objMap, URLs.portfolioBasedAccountDetails);

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
            },
            //To disable edit button when selected folder is released and approved.
            onSelectRow: function (rowID) {
                //to check if user has permissions to disable the Release Folder.(eg. Portfolio Designer.)                                 
                var editFlag = checkUserPermissionForEditable(URLs.portfolioBasedAccountDetails);
                if (rowID !== undefined && rowID !== null) {
                    var row = $(this).getRowData(rowID);
                    if (editFlag) {
                        if (row.Status == 'Published' && row.ApprovalStatusID !== null && row.ApprovalStatusID == 1) {
                            $(elementIDs.buttonPortfolioEditJQ).addClass('ui-state-disabled');
                        }
                        else {
                            $(elementIDs.buttonPortfolioEditJQ).removeClass('ui-state-disabled');
                        }




                    }
                }
                //For selecting first row of expanded sub-grid.
                rows = $(this).getDataIDs();
                for (i = 0 ; i < rows.length ; i++) {
                    row = $(this).getRowData(rows[i]);
                    if (row.IsExpanded == "true")
                        $("#padg_" + rows[i] + "_t").jqGrid("setSelection", 1);
                }


            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.portfolioBasedAccountDetailGridJQ);
            }
        });
        var pagerElement = '#p' + elementIDs.portfolioBasedAccountDetailGrid;
        //remove default buttons
        $(elementIDs.portfolioBasedAccountDetailGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        $(elementIDs.portfolioBasedAccountDetailGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

        function viewButtonClickForPortfolioGrid(gridID) {
            var rowID = $(gridID).getGridParam('selrow');
            if (rowID !== undefined && rowID !== null) {
                var row = $(gridID).getRowData(rowID);
                var url = URLs.folderVersionDetailsPortfolioBasedAccount.replace(/{tenantId}/g, row.TenantID).replace(/{folderVersionId}/g, row.FolderVersionID).replace(/{folderId}/g, row.FolderID);
                url = url.replace(/{mode}/g, false); //since view button is clicked mode needs to be false
                window.location.href = url;
            }
        }

        //View button in footer of grid 
        $(elementIDs.portfolioBasedAccountDetailGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-document', title: 'View', id: elementIDs.buttonPortfolioView,
               onClickButton: function () {
                   viewButtonClickForPortfolioGrid(elementIDs.portfolioBasedAccountDetailGridJQ);
               }
           });
        //Edit button in footer of grid 
        $(elementIDs.portfolioBasedAccountDetailGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-pencil', title: 'Edit', id: elementIDs.buttonPortfolioEdit,
            onClickButton: function () {
                var rowID = $(this).getGridParam('selrow');
                if (rowID !== undefined && rowID !== null) {
                    var row = $(this).getRowData(rowID);

                    if (isFolderLockEnable == false) {
                        var url = URLs.folderVersionDetailsPortfolioBasedAccount.replace(/{tenantId}/g, row.TenantID).replace(/{folderVersionId}/g, row.FolderVersionID).replace(/{folderId}/g, row.FolderID);
                        url = url.replace(/{mode}/g, true);
                        window.location.href = url;
                        return;
                    }
                    var promise = ajaxWrapper.getJSON(URLs.LockStatusUrl.replace(/{tenantId}/g, row.TenantID).replace(/\{folderId\}/g, row.FolderID));
                    promise.done(function (result) {
                        if (result != "") {
                            var userWarning = FolderLock.userWarning.replace("{0}", result);
                            checkUnlockFolderClaims(elementIDs, claims);

                            folderLockWarningDialog.show(userWarning, function () {
                                //unlock Folder 
                                var promise1 = ajaxWrapper.getJSON(URLs.OverrideLockUrl.replace(/{tenantId}/g, row.TenantID).replace(/\{folderId\}/g, row.FolderID));
                                promise1.done(function (xhr) {
                                    if (xhr.Result === ServiceResult.SUCCESS) {
                                        var url = URLs.folderVersionDetailsPortfolioBasedAccount.replace(/{tenantId}/g, row.TenantID).replace(/{folderVersionId}/g, row.FolderVersionID).replace(/{folderId}/g, row.FolderID);
                                        url = url.replace(/{mode}/g, true);
                                        window.location.href = url;
                                    }
                                    else if (xhr.Result === ServiceResult.FAILURE) {
                                        messageDialog.show(xhr.Items[0].Messages);
                                    }
                                    else {
                                        messageDialog.show(Common.errorMsg);
                                    }
                                });
                            }, function () {
                                //View Folder 
                                var url = URLs.folderVersionDetailsPortfolioBasedAccount.replace(/{tenantId}/g, row.TenantID).replace(/{folderVersionId}/g, row.FolderVersionID).replace(/{folderId}/g, row.FolderID);
                                url = url.replace(/{mode}/g, false);
                                window.location.href = url;
                            });
                            return;
                        }
                        else {
                            var url = URLs.folderVersionDetailsPortfolioBasedAccount.replace(/{tenantId}/g, row.TenantID).replace(/{folderVersionId}/g, row.FolderVersionID).replace(/{folderId}/g, row.FolderID);
                            url = url.replace(/{mode}/g, true);
                            window.location.href = url;
                        }
                    });
                    promise.fail(showError);
                }
            }
        });
    }

    //documentName is the value entered in "Document Name" search box.
    function loadNonPortfolioBasedAccountDetailGrid(documentName) {
        //set column list for grid
        var colArray = ['', '', '', 'Account', 'Folder', 'Folder Version Number', 'Effective Date', 'Status', '', '', 'Updated By', 'Last Updated', '', '', '', '', '', ''];
        //set column models
        var colModel = [];
        colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true });
        colModel.push({ name: 'AccountID', index: 'AccountID', hidden: true });
        colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', hidden: true });
        colModel.push({ name: 'AccountName', index: 'AccountName', editable: false, align: 'left' });
        colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left' });
        colModel.push({ name: 'VersionNumber', index: 'VersionNumber', editable: false, align: 'left' });
        colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, search: true, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'left' });
        colModel.push({ name: 'Status', index: 'Status', editable: false, align: 'left' });
        colModel.push({ name: 'CategoryID', index: 'CategoryID', hidden: true, editable: false, align: 'left' });
        colModel.push({ name: 'CategoryName', index: 'CategoryName', hidden: true, editable: false, align: 'left' });
        colModel.push({ name: 'UpdatedBy', index: 'UpdatedBy', editable: false, align: 'left' });
        colModel.push({ name: 'UpdatedDate', index: 'UpdatedDate', editable: false, search: true, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'left' });
        colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true, key: true });
        colModel.push({ name: 'FolderVersionStateID', index: 'FolderVersionStateID', hidden: true, editable: false });
        colModel.push({ name: 'FolderVersionCount', index: 'FolderVersionCount', hidden: true, editable: false });
        colModel.push({ name: 'RowID', index: 'RowID', hidden: true, editable: false });
        colModel.push({ name: 'IsExpanded', index: 'IsExpanded', hidden: true, editable: false });
        colModel.push({ name: 'ApprovalStatusID', index: 'ApprovalStatusID', hidden: true });
        //clean up the grid first - only table element remains after this
        $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).jqGrid('GridUnload');
        //adding the pager element
        $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).parent().append("<div id='p" + elementIDs.nonPortfolioBasedAccountDetailGrid + "'></div>");
        var url = URLs.nonPortfolioBasedAccountDetails.replace(/{documentName}/g, encodeURIComponent(documentName));
        $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Accounts',
            height: '400',
            rowNum: 20,
            rowList: [10, 20, 30],
            ignoreCase: true,
            //loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.nonPortfolioBasedAccountDetailGrid,
            sortname: 'UpdatedDate',
            sortorder: 'desc',
            altclass: 'alternate',
            subGrid: true,
            subGridRowExpanded: function (subGridId, rowId) {
                $(this).jqGrid("setSelection", rowId);
                if (rowId !== undefined && rowId !== null) {
                    var row = $(this).getRowData(rowId);
                    var url = URLs.getFolderVersionList.replace(/{tenantId}/g, row.TenantID).replace(/{folderId}/g, row.FolderID).replace(/{accountName}/g, row.AccountName).replace(/{accountID}/g, row.AccountID).replace(/{documentName}/g, documentName);
                    var subGridTableId, pager_id;
                    //table id of subgrid.
                    subGridTableId = subGridId + "_t";
                    subGridTableIdJQ = "#" + subGridTableId
                    pager_id = "p_" + subGridTableId;
                    //Set value as true for IsExpanded column of selected row of main grid. 
                    row.IsExpanded = "true";
                    $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).jqGrid("setRowData", rowId, row);
                    //created table tag.
                    $("#" + subGridId).html("<table id='" + subGridTableId + "' class='scroll'></table><div id='" + pager_id + "' class='scroll'></div>");
                    $(subGridTableIdJQ).jqGrid({
                        url: url,
                        datatype: 'json',
                        cache: false,
                        colNames: ['', '', '', 'Folder', 'Folder Version Number', 'Effective Date', '', '', ''],
                        colModel: [
                            { name: 'TenantID', index: 'TenantID', editable: false, align: 'left', hidden: true },
                            { name: 'AccountID', index: 'AccountID', editable: false, align: 'left', hidden: true },
                            { name: 'FolderVersionID', index: 'FolderVersionID', hidden: true, editable: false, align: 'left', hidden: true },
                            { name: 'FolderName', index: 'FolderName', width: 200, editable: false, align: 'left' },
                            { name: 'VersionNumber', index: 'VersionNumber', width: 200, editable: false, align: 'left' },
                            { name: 'EffectiveDate', index: 'EffectiveDate', width: 200, editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'left' },
                            { name: 'FolderID', index: 'FolderID', hidden: true },
                            { name: 'ApprovalStatusID', index: 'ApprovalStatusID', editable: false, align: 'left', hidden: true },
                             { name: 'FolderVersionStateID', index: 'FolderVersionStateID', editable: false, align: 'left', hidden: true, Key: true }
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
                            //To set first row of grid as selected.
                            var rows = $(this).getDataIDs();
                            if (rows.length > 0) {
                                $(this).jqGrid("setSelection", rows[0]);
                            }
                            //check application claims               
                            var objMap = {
                                view: "#" + subGridTableId + 'view',
                                copy: "#" + subGridTableId + 'copy',
                            };
                            checkApplicationClaims(claims, objMap, URLs.nonPortfolioBasedAccountDetails);
                        },
                        onSelectRow: function (rowID) {
                            var row = $(this).getRowData(rowID);
                            if (row.FolderVersionStateID == FolderVersionState.INPROGRESS_BLOCKED) {
                                $(subGridTableIdJQ + 'copy').addClass('ui-state-disabled');
                            }
                            else {
                                $(subGridTableIdJQ + 'copy').removeClass('ui-state-disabled');
                            }


                        }
                    })
                    var pagerElement = '#' + pager_id;
                    $(subGridTableIdJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
                    //remove paging
                    $(pagerElement).find(pagerElement + '_center').remove();
                    $(pagerElement).find(pagerElement + '_right').remove();

                    //View button in footer of grid 
                    $(subGridTableIdJQ).jqGrid('navButtonAdd', pagerElement,
                    {
                        caption: '', buttonicon: 'ui-icon-document', title: 'View', id: subGridTableId + "view",
                        onClickButton: function () {
                            viewButtonClickForNonPortfolioGrid(subGridTableIdJQ);
                        }
                    });

                }

                //Copy button in footer of grid 
                $(subGridTableIdJQ).jqGrid('navButtonAdd', pagerElement,
                {
                    caption: '', buttonicon: 'ui-icon-copy', title: 'Copy', id: subGridTableId + "copy",
                    onClickButton: function () {
                        copyButtonClick(subGridTableIdJQ);
                    }
                });

            },
            subGridRowColapsed: function (subGridId, rowId) {
                $(this).jqGrid("setSelection", rowId);
            },
            gridComplete: function () {
                //To set first row of grid as selected.
                rows = $(this).getDataIDs();
                if (rows && rows.length > 0) {
                    $(this).jqGrid("setSelection", rows[0]);
                }

                //to check for claims..              
                var objMap = {
                    edit: elementIDs.buttonNonPortfolioEditJQ,
                    view: elementIDs.buttonNonPortfolioViewJQ,
                    copy: elementIDs.buttonNonPortfolioCopyJQ,
                    add: elementIDs.buttonNonPortfolioAddJQ,
                    remove: elementIDs.buttonNonPortfolioRemoveJQ
                };
                checkApplicationClaims(claims, objMap, URLs.nonPortfolioBasedAccountDetails);

                if ($(objMap['remove']).hasClass("ui-state-disabled")) {
                    $(objMap['remove']).hide();

                }
                else {
                    $(objMap['remove']).show();
                }

                //To remove plus sign when subgrid is not present
                for (i = 0; i < rows.length; i++) {
                    var row = $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).getRowData(rows[i]);
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

                checkAccountSearchClaims(elementIDs, claims);

            },
            //To disable edit button when selected folder is released and approved.
            onSelectRow: function (rowID) {
                ////to check if user has permissions to disable the Release Folder.(eg. Portfolio Designer.)                                 
                //var editFlag = checkUserPermissionForEditable(URLs.nonPortfolioBasedAccountDetails);
                //if (rowID !== undefined && rowID !== null) {
                //    var row = $(this).getRowData(rowID);
                //    if (editFlag) {
                //        if (vbRole != undefined) {
                //            if ((vbRole != Role.Audit && row.Status == "Test/Audit")
                //                && (vbRole != Role.ProductAudit && row.Status == "Test/Audit") && (vbRole != Role.TMGSuperUser && row.Status == "Test/Audit")) {
                //                $(elementIDs.buttonNonPortfolioEditJQ).addClass('ui-state-disabled');
                //                $(elementIDs.buttonNonPortfolioCopyJQ).removeClass('ui-state-disabled');
                //            }
                //            else if (row.Status == 'Facets Prod' && row.ApprovalStatusID !== null && row.ApprovalStatusID == 1) {
                //                $(elementIDs.buttonNonPortfolioEditJQ).addClass('ui-state-disabled');
                //                $(elementIDs.buttonNonPortfolioCopyJQ).removeClass('ui-state-disabled');
                //            }
                //            else if (row.FolderVersionStateID == FolderVersionState.INPROGRESS_BLOCKED) {
                //                $(elementIDs.buttonNonPortfolioEditJQ).addClass('ui-state-disabled');
                //                $(elementIDs.buttonNonPortfolioCopyJQ).addClass('ui-state-disabled');
                //            }
                //            else {
                //                $(elementIDs.buttonNonPortfolioEditJQ).removeClass('ui-state-disabled');
                //                $(elementIDs.buttonNonPortfolioCopyJQ).removeClass('ui-state-disabled');
                //            }
                //        }

                //        if (row.Status == "BPD Distribution" && row.ApprovalStatusID == 1) {
                //            $(elementIDs.buttonNonPortfolioEditJQ).addClass('ui-state-disabled');
                //        }

                //    }
                //}
                //For selecting first row of expanded sub-grid.
                rows = $(this).getDataIDs();
                for (i = 0; i < rows.length; i++) {
                    var row = $(this).getRowData(rows[i]);
                    if (row.IsExpanded == "true") {
                        $("#npadg_" + rows[i] + "_t").jqGrid("setSelection", 1);
                    }
                }
            },
            ondblClickRow: function (rowid, iRow, iCol, e) {
                LoadFolderVersion(rowid);
            }

        });


        var pagerElement = '#p' + elementIDs.nonPortfolioBasedAccountDetailGrid;
        //remove default buttons
        $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

        function viewButtonClickForNonPortfolioGrid(gridID) {
            var rowID = $(gridID).getGridParam('selrow');
            if (rowID !== undefined && rowID !== null) {
                var row = $(gridID).getRowData(rowID);
                if (row.FolderID == "")
                    messageDialog.show(ConsumerAccount.folderAbsentMsg);
                else {
                    var url = URLs.folderVersionDetailsNonPortfolioBasedAccount.replace(/{tenantId}/g, row.TenantID).replace(/{folderVersionId}/g, row.FolderVersionID).replace(/{folderId}/g, row.FolderID).replace(/{fldviewmode}/g, "Default");
                    url = url.replace(/{mode}/g, false); //since view button is clicked mode needs to be false
                    window.location.href = url;
                }
            }
            else
                messageDialog.show(FolderVersion.selectRowToView);
        }

        //View button in footer of grid 
        $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-document', title: 'View', id: elementIDs.buttonNonPortfolioView,
               onClickButton: function () {
                   viewButtonClickForNonPortfolioGrid(elementIDs.nonPortfolioBasedAccountDetailGridJQ);
               }
           });
        //edit button in footer of grid 
        $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-pencil', title: 'Edit', id: elementIDs.buttonNonPortfolioEdit,
            onClickButton: function () {
                var rowID = $(this).getGridParam('selrow');
                if (rowID !== undefined && rowID !== null) {
                    LoadFolderVersion(rowID);
                }
            }

        });
        function copyButtonClick(gridID) {
            var rowID = $(gridID).getGridParam('selrow');
            if (rowID !== undefined && rowID !== null) {
                var row = $(gridID).getRowData(rowID);
                if (row.FolderID == "")
                    messageDialog.show(ConsumerAccount.folderAbsentMsg);
                else {
                    folderDialog.show(false, row, 'Copy');
                }
            }
        }
        //Copy button in footer of grid 
        $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-copy', title: 'Copy', id: elementIDs.buttonNonPortfolioCopy,
            onClickButton: function () {
                //Check if user has permission to create the Account
                var promise = ajaxWrapper.getJSON(URLs.folderVersionCreationPermission);

                //register ajax success callback
                promise.done(function (result) {
                    if (result == true) {
                        copyButtonClick(elementIDs.nonPortfolioBasedAccountDetailGridJQ);
                    } else {
                        messageDialog.show('User do not have permission to copy folder version.');
                    }
                });
                //register ajax failure callback
                promise.fail(showError);

            }
        });
        //Add button in footer of grid 
        $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-plus', title: 'Add', id: elementIDs.buttonNonPortfolioAdd,
               onClickButton: function () {
                   //Check if user has permission to create the Account
                   var promise = ajaxWrapper.getJSON(URLs.folderVersionCreationPermission);

                   //register ajax success callback
                   promise.done(function (result) {
                       if (result == true) {
                           folderDialog.show(false, '', 'add');
                       } else {
                           messageDialog.show('User do not have permission to create folder version.');
                       }
                   });
                   //register ajax failure callback
                   promise.fail(showError);

               }
           });
        //delete button in footer of grid 
        $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-trash', title: 'delete', id: elementIDs.buttonNonPortfolioRemove,
               onClickButton: function () {

                   var rowID = $(this).getGridParam('selrow');
                   var row = $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).getRowData(rowID);
                   confirmDialog.show(ConsumerAccount.deleteConfirmationForFolderMsg.replace(/\{#FolderName}/g, row.FolderName), function () {
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
                                       var url = URLs.deleteNonPortfolioBasedFolder.replace(/{tenantId}/g, row.TenantID).replace(/{folderId}/g, row.FolderID);
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
                                               $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).trigger("reloadGrid")
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

    function LoadFolderVersion(rowID) {
        //var row = $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).getRowData(rowID);
        if (rowID !== undefined && rowID !== null) {
            var editFlag = checkUserPermissionForEditable(URLs.nonPortfolioBasedAccountDetails);
            var row;
            var isReleasedVersion = false;
            var parentGridRowID = $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).getGridParam('selrow');
            if (rowID == parentGridRowID)
                row = $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).getRowData(rowID);
            else {
                var subGridIdJQ = "#npadg_" + parentGridRowID + "_t";
                row = $(subGridIdJQ).getRowData(rowID);
                isReleasedVersion = true;
            }
            if ((row.Status == 'Published' || row.Status == 'Release') && row.ApprovalStatusID !== null && (row.ApprovalStatusID == 1 || row.ApprovalStatusID == 6)) {
                isReleasedVersion = true;
            }
            if (row.FolderID == "")
                messageDialog.show(ConsumerAccount.folderAbsentMsg);
            else {
                if (isFolderLockEnable == false) {
                    var url = URLs.folderVersionDetailsNonPortfolioBasedAccount.replace(/{tenantId}/g, row.TenantID).replace(/{folderVersionId}/g, row.FolderVersionID).replace(/{folderId}/g, row.FolderID).replace(/{fldviewmode}/g, "Default");;
                    url = url.replace(/{mode}/g, true); //since edit button is clicked mode needs to be true  
                    window.location.href = url;
                    return;
                } else if (isReleasedVersion == true) {
                    var url = URLs.folderVersionDetailsNonPortfolioBasedAccount.replace(/{tenantId}/g, row.TenantID).replace(/{folderVersionId}/g, row.FolderVersionID).replace(/{folderId}/g, row.FolderID).replace(/{fldviewmode}/g, "Default");;
                    url = url.replace(/{mode}/g, false); 
                    window.location.href = url;
                    return;
                }
                var promise = ajaxWrapper.getJSON(URLs.LockStatusUrl.replace(/{tenantId}/g, row.TenantID).replace(/\{folderId\}/g, row.FolderID));
                promise.done(function (result) {
                    if (result != "") {
                        var userWarning = FolderLock.userWarning.replace("{0}", result);
                        checkUnlockFolderClaims(elementIDs, claims);

                        folderLockWarningDialog.show(userWarning, function () {
                            //unlock Folder 
                            var promise1 = ajaxWrapper.getJSON(URLs.OverrideLockUrl.replace(/{tenantId}/g, row.TenantID).replace(/\{folderId\}/g, row.FolderID));
                            promise1.done(function (xhr) {
                                if (xhr.Result === ServiceResult.SUCCESS) {
                                    var url = URLs.folderVersionDetailsNonPortfolioBasedAccount.replace(/{tenantId}/g, row.TenantID).replace(/{folderVersionId}/g, row.FolderVersionID).replace(/{folderId}/g, row.FolderID);
                                    url = url.replace(/{mode}/g, true); //since edit button is clicked mode needs to be true  
                                    window.location.href = url;
                                }
                                else if (xhr.Result === ServiceResult.FAILURE) {
                                    messageDialog.show(xhr.Items[0].Messages);
                                }
                                else {
                                    messageDialog.show(Common.errorMsg);
                                }
                            });
                        }, function () {
                            //View Folder 
                            var url = URLs.folderVersionDetailsNonPortfolioBasedAccount.replace(/{tenantId}/g, row.TenantID).replace(/{folderVersionId}/g, row.FolderVersionID).replace(/{folderId}/g, row.FolderID);
                            url = url.replace(/{mode}/g, false); //since edit button is clicked mode needs to be true  
                            window.location.href = url;
                        });
                        return;
                    }
                    else {
                        var url = URLs.folderVersionDetailsNonPortfolioBasedAccount.replace(/{tenantId}/g, row.TenantID).replace(/{folderVersionId}/g, row.FolderVersionID).replace(/{folderId}/g, row.FolderID).replace(/{fldviewmode}/g, "Default");
                        url = url.replace(/{mode}/g, editFlag); //since edit button is clicked mode needs to be true  
                        window.location.href = url;
                    }
                });
                promise.fail(showError);
            }
        }
    }

    function init() {
        $(document).ready(function () {
            //loadPortfolioBasedAccountDetailGrid();
            loadNonPortfolioBasedAccountDetailGrid("");
            //Click event for Search button
            $(elementIDs.btnDocSearchJQ).click(function () {
                //Get entered value into the search box. 
                var documentName = $(elementIDs.searchDocJQ).val();
                loadNonPortfolioBasedAccountDetailGrid(documentName);
            });
            //Click event for Clear button
            $(elementIDs.btnDocClearJQ).click(function () {
                $(elementIDs.searchDocJQ).val("");
                loadNonPortfolioBasedAccountDetailGrid("");
            });

            $(elementIDs.searchDocJQ).on("keypress", function (e) {
                var code = e.keyCode || e.which;
                if (code == 13) {
                    loadNonPortfolioBasedAccountDetailGrid(this.value);
                }
            });
        });

    }
    init();

    return {
        loadPortfolioBasedAccountDetail: function () {
            loadPortfolioBasedAccountDetailGrid();
        },

        loadNonPortfolioBasedAccountDetail: function () {
            loadNonPortfolioBasedAccountDetailGrid();
        }
    }
}();

var accountFolderDialog = function () {
    var URLs = {
        accountAdd: '/ConsumerAccount/Add',
        categoryListAccount: '/Settings/GetFolderVersionCategoryForDropdown?tenantId=1&isPortfolio=false&folderVersionID=0&isFinalized=true',
        folderAdd: '/ConsumerAccount/AddFolder',
        applicableTeamsAdd: '/FolderVersion/AddApplicableTeams',
        folderVersionIndex: '/FolderVersion/Index?tenantId={tenantId}&folderVersionId={folderVersionId}&folderId={folderId}&mode={mode}',
        folderVersionGetNonPortfolioBasedAccountFolders: '/FolderVersion/GetNonPortfolioBasedAccountFolders?tenantId={tenantId}&folderVersionId={folderVersionId}&folderId={folderId}&mode={mode}',
    }
    var isPortfolioFolder;
    var applicableTeamsRequired = false;
    var elementIDs = {
        accountFolderDialog: "#accountfolderdialog",
        categoryDDL_d: "#categoryDDL_d",
        btnNext: "#btnNext",
        btnSave: '#btnSave',
        effectiveDate_d: '#effectiveDate_d',
        folderName_d: "#folderName_d",
        accountNames_d: "#accountNames_d",
        marketSegment_d: "#marketSegment_d",
        folderNameSpan_d: "#folderNameSpan_d",
        accountfolderdialog: "#accountfolderdialog",
        divAccountDetails: "#divAccountDetails",
        divFolderDetails: "#divFolderDetails",
        nonPortfolioBasedAccountDetailGridJQ: '#npadg',
        categoryNamesDDLJQ: '#categoryDDL_d',
        categorySpan: '#categorySpan_d',
        catIDJQ: '#catIDTXT',
        folderDialogApplicableTeams_d: "#folderDialogApplicableTeams_d",
    }
    function init() {
        //register dialog for grid row add/edit
        $(elementIDs.accountFolderDialog).dialog({
            autoOpen: false,
            height: 230,
            width: 450,
            modal: true
        });
        //To display datepicker for folder effective date.
        $(elementIDs.effectiveDate_d).datepicker({
            dateFormat: 'mm/dd/yy',
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-61:c+20',
            showOn: "both",
            //CalenderIcon path declare in golbalvariable.js
            buttonImage: Icons.CalenderIcon,
            buttonImageOnly: true,
        }).parent().find('img').css('margin-bottom', '7px');

        $(elementIDs.btnNext).on('click', function () {
            //check if name is already used
            var newName = $(elementIDs.accountFolderDialog + ' input').val();
            if (newName == '') {
                $(elementIDs.accountFolderDialog + ' div').addClass('has-error');
                $(elementIDs.accountFolderDialog + ' .help-block').text(ConsumerAccount.accountNameRequiredMsg);
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
                //ajax call to add/update
                var promise = ajaxWrapper.postJSON(url, accountData);
                //register ajax success callback
                promise.done(function (data) { accountFolderSuccess(data, accountDialogState) });
                //register ajax failure callback
                promise.fail(showError);
            }
        });
        $(elementIDs.btnSave).on('click', function () {
            //var rowId = $(elementIDs.accountDetailGridJQ).getGridParam('selrow');
            var newFolderEffectiveDate = $(elementIDs.effectiveDate_d).val();
            var newFolderName = $(elementIDs.folderName_d).val();
            var categoryId = $(elementIDs.categoryDDL_d).val();
            var catId = $(elementIDs.catIDJQ).val();
            if (categoryId == 0) categoryId = null;
            if (catId == undefined) catId = "";
            isPortfolioFolder = false;
            var validateFolderdata = validateFolderData();
            if (validateFolderdata == true) {
                if (applicableTeamsRequired == true) {
                    var applicableTeamsIDs = new Array();
                    $(elementIDs.folderDialog).find('.applicableTeamsCheckbox').each(function () {
                        if ($(this).is(':checked'))
                            applicableTeamsIDs.push(parseInt($(this).attr('id')));
                    });
                }
                if ((applicableTeamsRequired == false) || (applicableTeamsRequired == true && applicableTeamsIDs.length > 0)) {
                    var newFolderName = $(elementIDs.folderName_d).val();
                    var accountId;
                    var url;
                    //var marketSegmentId = $(elementIDs.marketSegment_d).val();
                    var userId = 0;
                    var userName = "";
                    if (isPortfolioFolder == false) {
                        accountId = $(elementIDs.accountNames_d).val();
                    }

                    var folderAddData = {
                        tenantId: 1,
                        accountID: accountId,
                        folderName: newFolderName,
                        folderEffectiveDate: newFolderEffectiveDate,
                        isPortfolio: isPortfolioFolder,
                        userId: userId,
                        userName: userName,
                        marketSegmentId: 1,
                        categoryID: categoryId,
                        catID: catId
                    };
                    url = URLs.folderAdd;
                    
                    //ajax call to add folder
                    var promise = ajaxWrapper.postJSON(url, folderAddData);
                    //register ajax success callback
                    promise.done(folderSuccess);
                    //register ajax failure callback
                    promise.fail(showError);
                }
                else {
                    messageDialog.show(WorkFlowStateMessages.atleastOneApplicableTeamMsg);
                    $(elementIDs.folderDialog).find('.applicableTeamsCheckbox').each(function () {
                        $(this).parent().addClass("has-error");
                    });
                }
            }
        });
    }
    //To check flags from above functions
    function validateFolderData() {
        //var isMarketSegmentValid = validMarketSegment();
        var isAccountNameValid = true;
        //if (isPortfolioFolder == false) {
        //    isAccountNameValid = validAccountName();
        //}
        var isFolderNameValid = validFolderName();
        var isEffectiveDateValid = validEffectiveDate();
        var isCategoryValid = validateCategory();

        if (isAccountNameValid == false ||
        isFolderNameValid == false || isEffectiveDateValid == false || isCategoryValid == false) {
            return false;
        }
        return true;
    }
    function validateCategory() {
        validCategory = false;
        if ($(elementIDs.categoryNamesDDLJQ)[0].selectedIndex == 0 || $(elementIDs.categoryNamesDDLJQ)[0].selectedIndex == -1) {
            $(elementIDs.categoryNamesDDLJQ).addClass('form-control');
            $(elementIDs.categoryNamesDDLJQ).parent().addClass('has-error');
            $(elementIDs.categorySpan).text(ConsumerAccount.categoryRequiredMsg);
            validCategory = false;
        } else {
            $(elementIDs.categoryNamesDDLJQ).removeClass('form-control');
            $(elementIDs.categoryNamesDDLJQ).parent().removeClass('has-error');
            //$(elementIDs.categoryNamesAutoCompltDDLJQ).removeClass('highlightBorder');
            //$(elementIDs.categoryNamesAutoCompltDDLJQ).siblings('a').removeClass('highlightBorder');
            $(elementIDs.categorySpan).text('');
            validCategory = true;
        }
        return validCategory;
    }
    function validEffectiveDate() {
        validEffectiveDateFlag = false;
        var newFolderEffectiveDate = $(elementIDs.effectiveDate_d).val();
        if (newFolderEffectiveDate == '') {
            $(elementIDs.effectiveDate_d).parent().addClass('has-error');
            $(elementIDs.effectiveDate_d).addClass('form-control');
            $(elementIDs.effectiveDateSpan_d).text(Common.folderEffectiveDateRequiredMsg);
            validEffectiveDateFlag = false
        }
        else {
            var effectiveDateMessage = isValidEffectiveDate(newFolderEffectiveDate);
            if (effectiveDateMessage == "") {
                $(elementIDs.effectiveDate_d).removeClass('form-control');
                $(elementIDs.effectiveDate_d).parent().addClass('has-error');
                $(elementIDs.effectiveDateSpan_d).text('');
                validEffectiveDateFlag = true;
            }
            else {
                $(elementIDs.effectiveDate_d).parent().addClass('has-error');
                $(elementIDs.effectiveDate_d).addClass('form-control');
                $(elementIDs.effectiveDateSpan_d).text(effectiveDateMessage);
                validEffectiveDateFlag = false
            }
        }
        return validEffectiveDateFlag;
    }
    //To check all validations for folder name
    function validFolderName() {
        validFolderNameFlag = true;
        var newFolderName = $(elementIDs.folderName_d).val();
        var newAccountName = $(elementIDs.accountNames_d).find('option:selected').text();
        if (newFolderName == '') {
            $(elementIDs.folderName_d).parent().addClass('has-error');
            $(elementIDs.folderName_d).addClass('form-control');
            $(elementIDs.folderNameSpan_d).text(ConsumerAccount.folderNameRequiredMsg);
            validFolderNameFlag = false;
        }
        else {
            $(elementIDs.folderNameSpan_d).text('')
            $(elementIDs.effectiveDateSpan_d).text('');
            $(elementIDs.folderName_d).parent().removeClass('has-error');
            $(elementIDs.folderName_d).removeClass('form-control');

            var rowIds = $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).getDataIDs();
            for (index = 0; index < rowIds.length; index++) {
                var folderData = $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).getRowData(rowIds[index]);
                if (newFolderName === folderData.FolderName_d && newAccountName === folderData.AccountName) {
                    $(elementIDs.folderName_d).parent().addClass('has-error');
                    $(elementIDs.folderName_d).addClass('form-control');
                    $(elementIDs.folderNameSpan_d).text(ConsumerAccount.folderNameExistsMsg);
                    validFolderNameFlag = false;
                }
            }
            //}
            var check = $(elementIDs.folderNameSpan_d).text();
            if (check != ConsumerAccount.folderNameExistsMsg) {
                $(elementIDs.folderName_d).parent().removeClass('has-error');
                $(elementIDs.folderName_d).removeClass('form-control');
                $(elementIDs.folderNameSpan_d).text('')
                validFolderNameFlag = true;
            }
        }
        return validFolderNameFlag;
    }
    function folderSuccess(xhr) {
        if (xhr.Result === ServiceResult.FAILURE || xhr.Result === 'undefined') {
            $(elementIDs.folderName_d).parent().addClass('has-error');
            $(elementIDs.folderName_d).addClass('form-control');
            $(elementIDs.folderNameSpan_d).text(ConsumerAccount.folderNameExistsMsg);
        }
        else {
            //code to save selected workflow states            
            if (applicableTeamsRequired == true) {
                var applicableTeamsIDs = new Array();
                $(elementIDs.folderDialog).find('.applicableTeamsCheckbox').each(function () {
                    if ($(this).is(':checked'))
                        applicableTeamsIDs.push(parseInt($(this).attr('id')));
                });
                var applicableTeamsAddData = {
                    FolderID: parseInt(xhr.Items[0].Messages[2]),
                    FolderVersionID: parseInt(xhr.Items[0].Messages[1]),
                    ApplicableTeamsIDList: applicableTeamsIDs,
                };

                //ajax call to add folder workflow states
                var promise = ajaxWrapper.postJSONCustom(URLs.applicableTeamsAdd, applicableTeamsAddData);
                //register ajax success callback
                promise.done(function (result) {
                    if (result.Result === ServiceResult.FAILURE) {
                        messageDialog.show("Applicable Teams are not saved.");
                        //$(elementIDs.folderDialog).dialog('close');
                        return;
                    }
                    else {
                        redirectToFolder(xhr);
                    }
                });
                //register ajax failure callback
                promise.fail(showError);
            }
            else {
                redirectToFolder(xhr);
            }
        }
    }

    function redirectToFolder(xhr) {
        var Tempurl = URLs.folderVersionIndex;
        if ($(elementIDs.portfolioYes).is(":checked") == false) {
            var Tempurl = URLs.folderVersionGetNonPortfolioBasedAccountFolders;
        }
        $(elementIDs.accountfolderdialog).dialog('close');
        var url = Tempurl
            .replace(/{tenantId}/g, parseInt(xhr.Items[0].Messages[0]))
            .replace(/{folderVersionId}/g, parseInt(xhr.Items[0].Messages[1]))
            .replace(/{folderId}/g, parseInt(xhr.Items[0].Messages[2]))
            .replace(/{mode}/g, true); //since edit button is clicked mode needs to be true
        window.location.href = url;
    }
    function accountFolderSuccess(xhr, actionMode) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            if (actionMode == 'add') {
                messageDialog.show(ConsumerAccount.addAccountMsg);
                $(elementIDs.accountFolderDialog).dialog({
                    autoOpen: false,
                    height: 350,
                    width: 550,
                    modal: true
                });
                $(elementIDs.accountNames_d).append("<option value=" + xhr.Items[0].Messages[0] + ">" + $(elementIDs.accountFolderDialog + ' input').val() + "</option>");
                fillCategoryDDL(0);
                $(elementIDs.divAccountDetails).hide();
                $(elementIDs.divFolderDetails).show();
            }
        }
        else {
            messageDialog.show(ConsumerAccount.duplicateAccountMsg);
        }
        //reset dialog elements
        $(elementIDs.accountFolderDialog + ' div').removeClass('has-error');
        //$(elementIDs.accountFolderDialog).dialog('close');
    }
    function fillCategoryDDL(categoryID) {
        var categoryName;
        $(elementIDs.categoryDDL_d).empty();
        $(elementIDs.categoryDDL_d).append("<option value=0>" + "--Select Category--" + "</option>");
        //ajax call for drop down list of account names
        var promise = ajaxWrapper.getJSON(URLs.categoryListAccount);
        var folderVersionCategoryName = "";
        promise.done(function (names) {
            for (i = 0; i < names.length; i++) {
                // For creating Folder the group Change Request and JAA Change Request should not be include , Refer ANT-13
                if (names[i].FolderVersionCategoryName != AntWFCategoriesGroups.ChangeRequest && names[i].FolderVersionCategoryName != AntWFCategoriesGroups.JAAChangeRequest) {
                    // If Role is TPA Analyst then include only JAA New Group, else for all other roles (SuperUser,EBA Analyst) should include all
                    if (vbRole == Role.TPAAnalyst) {
                        if (names[i].FolderVersionCategoryName == AntWFCategoriesGroups.JAANewGroup) {
                            $(elementIDs.categoryDDL_d).append("<option value=" + names[i].FolderVersionCategoryID + ">" + names[i].FolderVersionCategoryName + "</option>");
                        }
                    }
                    else {
                        $(elementIDs.categoryDDL_d).append("<option value=" + names[i].FolderVersionCategoryID + ">" + names[i].FolderVersionCategoryName + "</option>");
                    }
                }
                if (categoryID == names[i].FolderVersionCategoryID) {
                    folderVersionCategoryName = names[i].FolderVersionCategoryName;
                }
            }
            if (categoryID > 0) {
                $(elementIDs.categoryNamesAutoCompltDDLJQ).val(folderVersionCategoryName);
                $(elementIDs.categoryDDL_d).val(categoryID);
            } else {
                $(elementIDs.categoryNamesAutoCompltDDLJQ).val("--Select Category--");
            }
        });

        promise.fail(showError);
    }
    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
    init();
    return {
        show: function (accountName, action) {
            accountDialogState = action;
            $(elementIDs.accountFolderDialog + ' input').each(function () {
                $(this).val(accountName);
            });
            //set eleemnts based on whether the dialog is opened in add or edit mode
            $(elementIDs.accountFolderDialog + ' div').removeClass('has-error');
            $(elementIDs.accountFolderDialog + ' .help-block').text('');
            if (applicableTeamsRequired == false) {
                $(elementIDs.folderDialogApplicableTeams_d).hide();
            }
            if (accountDialogState == 'add') {
                $(elementIDs.accountFolderDialog).dialog('option', 'title', 'Create New Account and Plans');
            }
            //else {
            //    $(elementIDs.accountFolderDialog).dialog('option', 'title', 'Edit Account');
            //}
            //open the dialog - uses jqueryui dialog
            $(elementIDs.accountFolderDialog).dialog("open");
        }
    }
}();

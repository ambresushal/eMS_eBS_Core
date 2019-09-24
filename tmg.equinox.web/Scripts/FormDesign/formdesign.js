//Document Design and Version Grids
//This will be a single instance which is invoked soon after it is loaded in the browser
var formDesign = function () {
    //variables required for tab management
    var tabs;
    var tabIndex = 1;
    var tabCount = 0;
    var tabNamePrefix = 'formDesign-';
    var designType;
    //urls to be accessed for form design
    var URLs = {
        //get Document Design List
        formDesignListByDocType: '/FormDesign/FormDesignListByDocType?tenantId=1&docDesignType={docDesignType}',
        formDesignList: '/FormDesign/FormDesignList?tenantId=1',
        //get Document Design Version
        formDesignVersionList: '/FormDesign/FormDesignVersionList?tenantId=1&formDesignId={formDesignId}',
        //Finalize Form Design
        formDesignVersionFinalize: '/FormDesign/FinalizeVersion',
        //Delete Form Design
        formDesignDelete: '/FormDesign/Delete',
        //Delete Document Design Version
        formDesignVersionDelete: '/FormDesign/DeleteVersion',
        //get preview for Form Design Version
        formDesignVersionPreview: '/FormDesignPreview/Preview?tenantId=1&formDesignVersionId={formDesignVersionId}&formName={formName}',
        //get Document Design Type List
        fillDocumentDesignType: '/FormDesign/GetDocumentDesignType',
        CompileFormDesignVersion: '/FormDesign/CompileFormDesignVersion',
        CompileDocumentRule: '/FormDesign/CompileFormDesignVersionRule',
        generateRuledescription: '/FormDesign/GenerateRuledescription?formDesignId={formDesignId}&formDesignVersionId={formDesignVersionId}',
    };

    //element ID's required for form design
    //added in Views/FormDesign/Index.cshtml
    var elementIDs = {
        //table element for the Document Design Grid
        formDesignGrid: 'fdg',
        //with hash for use with jQuery
        formDesignGridJQ: '#fdg',
        //container element for the form design tabs
        formDesignTabs: '#formdesigntabs',
        //table element for the Document Design Version Grid
        formDesignVersionGrid: 'fdvg',
        //with hash for use with jQuery
        formDesignVersionGridJQ: '#fdvg',
        //for dialog for add/edit of Form Design
        formDesignDialog: "#formdesigndialog",
        //for dialog for add/edit of Document Design Version
        formDesignVersionDialog: "#formdesignversiondialog",
        //for dropdown element for Document design type
        documentDesignTypeDDLJQ: '#documentDesignTypeDDL',
        btnDocADDJQ: '#btnDocAdd',
        documentDesignTypeDDLHelpBlockJQ: '#documentDesignTypeDDLHelpBlock',
        documentdesigndialog: '#documentdesigndialog'

    };

    //this function is called below soon after this JS file is loaded 
    //generates the tabs and loads the Document Design Grid
    function init() {

        $(document).ready(function () {
            //To remove style attribute so that Document Design tab is displayed after loading the  page. 
            $(elementIDs.formDesignTabs).removeAttr("style");
            //jqueryui tabs
            tabs = $(elementIDs.formDesignTabs).tabs();
            //register event for closing a tab page - refer jquery ui documentation
            //event will be registered for each tab page loaded
            tabs.delegate('span.ui-icon-close', 'click', function () {
                var panelId = $(this).closest('li').remove().attr('aria-controls');
                $('#' + panelId).remove();
                tabCount--;
                if (tabCount == 0) {
                    tabIndex = 1
                }
                tabs.tabs('refresh');
            });
            fillDocumentDesignDLL();
        });
    }

    $(elementIDs.documentDesignTypeDDLJQ).on('change', function () {
        $(elementIDs.documentDesignTypeDDLHelpBlockJQ).text('');
        $(elementIDs.documentDesignTypeDDLJQ).parent().removeClass('has-error');
        var docDesignType = parseInt($(elementIDs.documentDesignTypeDDLJQ).val());
        designType = docDesignType;
        //Remove tabs with previous Design type
        var getallli = document.querySelectorAll('#createtabs li');
        for (var i = 0; li = getallli[i]; i++) {
            if (i > 0) {
                li.parentNode.removeChild(li);
            }

        }
        //Clear document design version list
        $(elementIDs.formDesignVersionGridJQ).jqGrid('GridUnload');
        //Reset Tab count
        tabCount = 0;
        //Load only if document design selected
        if (docDesignType > 0) {
            loadFormDesignGrid(docDesignType);
        }
        else {
            $(elementIDs.formDesignGridJQ).jqGrid('GridUnload');
        }
    });


    $(elementIDs.btnDocADDJQ).click(function () {
        documentdesigndialog.show();

    });
    //fill document design dropdown

    function fillDocumentDesignDLL() {
        $(elementIDs.documentDesignTypeDDLHelpBlockJQ).text('');
        $(elementIDs.documentDesignTypeDDLHelpBlockJQ).removeClass('form-control');
        $(elementIDs.documentDesignTypeDDLJQ).parent().removeClass('has-error');

        var url = URLs.fillDocumentDesignType;

        var promise = ajaxWrapper.getJSON(url);
        //fill the folder list drop down
        promise.done(function (list) {
            for (i = 0; i < list.length; i++) {
                $(elementIDs.documentDesignTypeDDLJQ).append("<option value=" + list[i].DocumentDesignTypeID + ">" + list[i].DocumentDesignName + "</option>");
            }

            if (ClientRolesForFormDesignAccess.indexOf(vbRole) >= 0 && GLOBAL.applicationName.toLowerCase() != 'ebenefitsync' ) {
                SetFormDesignPermissionsToClientUser();
            }
        });

        promise.fail(showError);

    }

    function SetFormDesignPermissionsToClientUser() {
        $("li:contains('Folder')").addClass("disabled").find("a").prop("disabled", true);
        $(elementIDs.documentDesignTypeDDLJQ).prop('selectedIndex', FormDesignTypeID.ANCHOR);
        $(elementIDs.documentDesignTypeDDLJQ).trigger('change');
        $(elementIDs.documentDesignTypeDDLJQ).prop("disabled", true);

        $(elementIDs.formDesignGridJQ).bind('jqGridAfterGridComplete', function (event, data) {
            $("#p" + elementIDs.formDesignGrid).hide();//.addClass('divdisabled');
            $(elementIDs.formDesignVersionGridJQ).bind('jqGridAfterGridComplete', function (event, data) {
                $('#btnFormDesignVersionEdit').addClass('ui-icon-hide');
                $('#btnFormDesignVersionDelete').addClass('ui-icon-hide');
                $('#btnFormDesignVersionAdd').addClass('ui-icon-hide');
            });
        });
    }

    //ajax callback success - reload Form Desing  grid
    function formDesignDeleteSuccess(xhr) {
        var docDesignType = parseInt($(elementIDs.documentDesignTypeDDLJQ).val());
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(DocumentDesign.deleteDocumentDesign);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
        loadFormDesignGrid(docDesignType);
    }
    //ajax error callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else

            messageDialog.show(JSON.stringify(xhr));
    }

    //load form design list in grid
    function loadFormDesignGrid(docDesignType) {
        //set column list for grid
        var colArray = ['', '', 'Document Design', '', '', '', ''];

        //set column models
        var colModel = [];
        colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true, search: false, });
        colModel.push({ name: 'FormDesignId', index: 'FormDesignId', key: true, hidden: true });
        colModel.push({ name: 'DisplayText', index: 'DisplayText', editable: false, width: '300px' });
        colModel.push({ name: 'SourceDesign', index: 'SourceDesign', hidden: true, search: false });
        colModel.push({ name: 'IsAliasDesignMasterList', index: 'IsAliasDesignMasterList', hidden: true, search: false });
        colModel.push({ name: 'UsesAliasDesignMasterList', index: 'UsesAliasDesignMasterList', hidden: true, search: false });
        colModel.push({ name: 'IsSectionLock', index: 'IsSectionLock', hidden: true, editable: false, search: false, formatter: checkBoxFormatterDesign, unformat: unFormatIncludedColumnDesign });

        //clean up the grid first - only table element remains after this
        $(elementIDs.formDesignGridJQ).jqGrid('GridUnload');

        var url = URLs.formDesignListByDocType.replace(/\{docDesignType\}/g, docDesignType);
        //adding the pager element
        $(elementIDs.formDesignGridJQ).parent().append("<div id='p" + elementIDs.formDesignGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.formDesignGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Document Design List',
            height: '350',
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
            autowidth: false,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.formDesignGrid,
            sortname: 'FormDesignId',
            altclass: 'alternate',
            //load associated form design version grid on selecting a row
            onSelectRow: function (id) {
                var row = $(this).getRowData(id);
                loadFormDesignVersionGrid(row.FormDesignId, row.DisplayText);
            },
            //on adding a new form design, reload the grid and set the row to selected
            gridComplete: function () {
                var rows = $(this).getDataIDs();
                var rowId;
                if (rows.length > 0) {
                    rowId = rows[0];
                }
                var newName = $(elementIDs.formDesignDialog + ' input').val();
                var newId;
                for (index = 0; index < rows.length; index++) {
                    row = $(this).getRowData(rows[index]);
                    if (newName === row.DisplayText) {
                        newId = row.FormDesignId;
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
                    edit: "#btnFormDesignEdit",
                    add: "#btnFormDesignAdd",
                    remove: "#btnFormDesignDelete"
                };
                checkApplicationClaims(claims, objMap, URLs.formDesignList);
            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.formDesignGridJQ);
            }
        });

        var pagerElement = '#p' + elementIDs.formDesignGrid;
        //remove default buttons
        $(elementIDs.formDesignGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        //add button in footer of grid that pops up the add form design dialog
        $(elementIDs.formDesignGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-plus',
            title: 'Add', id: 'btnFormDesignAdd',
            onClickButton: function () {
                //load Document Design dialog on click - see formDesignDialog function below
                var designType = $(elementIDs.documentDesignTypeDDLJQ).val();
                var formDesignList = $(elementIDs.formDesignGridJQ).getRowData();
                console.log(JSON.stringify(formDesignList));
                var aliasDesignExists = false;
                var aliasDesignPresent = $.grep(formDesignList, function (n, i) {
                    return n.IsAliasDesignMasterList == "true";

                });
                if (aliasDesignPresent.length > 0) {
                    aliasDesignExists = true;
                }
                console.log(aliasDesignExists);
                formDesignDialog.show('', 'add', 0, designType, false, false, aliasDesignExists);
            }
        });
        //edit button in footer of grid that pops up the edit form design dialog
        $(elementIDs.formDesignGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-pencil',
            title: 'Edit', id: 'btnFormDesignEdit',
            onClickButton: function () {

                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    var formDesignList = $(elementIDs.formDesignGridJQ).getRowData();
                    var aliasDesignExists = false;
                    var aliasDesignPresent = $.grep(formDesignList, function (n, i) {
                        return n.IsAliasDesignMasterList == "true";
                    });
                    if (aliasDesignPresent.length > 0) {
                        aliasDesignExists = true;
                    }
                    var row = $(this).getRowData(rowId);
                    //load Document Design dialog for edit on click - see formDesignDialog function below
                    var isAliasDesignMasterList = false;
                    if (row.IsAliasDesignMasterList == "true") {
                        isAliasDesignMasterList = true;
                    }
                    var usesAliasDesignMasterList = false;
                    if (row.UsesAliasDesignMasterList == "true") {
                        usesAliasDesignMasterList = true;
                    }
                    var isSectionLock = false;
                    if (row.IsSectionLock == true) {
                        isSectionLock = true;

                    }
                    IsSectionLock = isSectionLock;
                    formDesignDialog.show(row.DisplayText, 'edit', row.SourceDesign, designType, isAliasDesignMasterList, usesAliasDesignMasterList, aliasDesignExists, IsSectionLock);
                }
            }
        });

        //delete button in footer of grid that pops up the delete form design dialog
        $(elementIDs.formDesignGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-trash',
            title: 'Delete', id: 'btnFormDesignDelete',
            onClickButton: function () {
                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    //get the formDesignVersion List for current form Design
                    var formDesignVersionList = $(elementIDs.formDesignVersionGridJQ).getRowData();

                    if (formDesignVersionList !== undefined && formDesignVersionList.length > 0) {
                        messageDialog.show(DocumentDesign.designDeleteValidationMsg);
                    }
                    else {
                        //load confirm dialogue to asset the operation
                        confirmDialog.show(Common.deleteConfirmationMsg, function () {
                            confirmDialog.hide();

                            //delete the form design
                            var formDesignDelete = {
                                tenantId: 1,
                                formDesignId: rowId
                            };
                            var promise = ajaxWrapper.postJSON(URLs.formDesignDelete, formDesignDelete);
                            //register ajax success callback
                            promise.done(formDesignDeleteSuccess);
                            //register ajax failure callback
                            promise.fail(showError);
                        });
                    }
                }

                else {
                    messageDialog.show(DocumentDesign.inProgressDesignSelectionMsg);
                }
            }
        });


        function checkBoxFormatterDesign(cellValue, options, rowObject) {
            return "<input id='para_" + options.rowId + (rowObject.IsSectionLock ? '\' checked=\'checked\' ' : '\'') + " type='checkbox' disabled='disabled'/>";
            //return "<select id=\'" + rowObject.RowID + "\'><option value=\"Account\">Account</option><option value=\"FolderVersion\">Folder Version</option><option value=\"Folder\">Folder</option><option value=\"Document\">Document</option></select>";
            //return str;
        }
        function unFormatIncludedColumnDesign(cellValue, options, rowObject) {
            return $('#para_' + options.rowId).prop('checked');
        }
        // add filter toolbar to the top
        $(elementIDs.formDesignGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: true,
            defaultSearch: "cn"
        });
    }

    //load form design verion in grid
    function loadFormDesignVersionGrid(formDesignId, formDesignName) {
        //set column list
        var colArray = ['TenantId', 'FormDesignId', 'Effective Date', 'Version', 'StatusId', 'Status', ''];

        //set column models
        var colModel = [];
        colModel.push({ name: 'TenantId', index: 'TenantId', hidden: true, search: false, });
        colModel.push({ name: 'FormDesignVersionId', index: 'FormDesignVersionId', key: true, hidden: true, width: 100, align: 'center', search: false, });
        colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', width: '240px', align: 'center', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions });
        colModel.push({ name: 'Version', index: 'Version', width: '150px', align: 'center', editable: false });
        colModel.push({ name: 'StatusId', index: 'StatusId', hidden: true, editable: false });
        colModel.push({ name: 'StatusText', index: 'StatusText', width: '150px', align: 'left', editable: false });
        colModel.push({ name: 'Action', index: 'Action', width: '95px', align: 'center', sortable: false, search: false, editable: false, formatter: actionFormatter });

        //get URL for grid
        var formDesignVersionListUrl = URLs.formDesignVersionList.replace(/\{formDesignId\}/g, formDesignId);

        //unload previous grid values
        $(elementIDs.formDesignVersionGridJQ).jqGrid('GridUnload');
        //add pager element
        $(elementIDs.formDesignVersionGridJQ).parent().append("<div id='p" + elementIDs.formDesignVersionGrid + "'></div>");
        //load grid - refer jqGrid documentation for details
        $(elementIDs.formDesignVersionGridJQ).jqGrid({
            url: formDesignVersionListUrl,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Document Design Version List - ' + formDesignName,
            forceFit: true,
            height: '360',
            autowidth: false,
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
            ExpandColumn: 'Label',
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.formDesignVersionGrid,
            sortname: 'FormDesignVersionId',
            sortorder: 'desc',
            altclass: 'alternate',
            //register events to load the form design version in a new tab page
            //this is on click of the span which has the image in the last column
            gridComplete: function () {
                var rows = $(this).getDataIDs();
                var reverse = rows.reverse();
                for (index = 0; index < rows.length; index++) {
                    row = $(this).getRowData(rows[index]);
                    $('#fvds' + row.FormDesignVersionId).click(function () {
                        var formDesignVersionId = this.id.replace(/fvds/g, '');
                        //launch a new tab here for this form design version
                        //TODO: pass TenantId too
                        var currentRow = $(elementIDs.formDesignVersionGridJQ).getRowData(formDesignVersionId);
                        //increment to manage adding /deletesof tab pages
                        var tabName = formDesignName + '-' + currentRow.Version;
                        //if form design version is already loaded, do not load again but make it active
                        var foundIndex;
                        if (tabs != undefined) {
                            tabs.find('.ui-tabs-anchor').each(function (index, element) {
                                if ($(this).text() == tabName) {
                                    foundIndex = index;
                                }
                            });
                        }
                        else {
                            foundIndex = 0;
                        }
                        if (foundIndex > 0) {
                            for (var i = 0; i < Finalized.length ; i++) {
                                if (Finalized[i].FORMDESIGNVERSIONID == formDesignVersionId && Finalized[i].ISFINALIZED == 1) {
                                    $(".ui-tabs-anchor").trigger("click");
                                }
                            }
                        }
                        if (foundIndex > 0) {
                            tabs.tabs('option', 'active', foundIndex);
                        }
                        else {
                            tabIndex++;
                            tabCount++;
                            //create formDesignVersion instance - load tab     

                            //this function will check the user permission for form design(eg. portfolio designer).
                            //var hasPermission = checkUserPermissionForEditable(URLs.formDesignVersionList);
                            //if (hasPermission) {
                            var fdVersion = new formDesignVersion(currentRow, formDesignId, formDesignName, row.StatusText, tabNamePrefix + tabIndex, tabIndex - 1, tabCount, tabs);
                            fdVersion.loadTabPage();
                            //}
                        }
                    });

                }

                var rowId;
                if (rows.length > 0) {
                    rowId = rows[rows.length - 1];
                }
                //get the newly added id for the form design version 
                //var newName = $(elementIDs.formDesignVersionDialog + ' input').val();
                //var newId;
                //for (index = 0; index < rows.length; index++) {
                //    row = $(this).getRowData(rows[index]);
                //    if (newName === row.DisplayText) {
                //        newId = row.FormDesignId;
                //        break;
                //    }
                //}
                //if (newId !== undefined) {
                //    //set newly added row 
                //    $(this).jqGrid('setSelection', newId);
                //}
                //else {
                //    //set first row 
                $(this).jqGrid('setSelection', rowId);
                //}

                //to check for claims..  
                var objMap = {
                    edit: "#btnFormDesignVersionEdit",
                    add: "#btnFormDesignVersionAdd",
                    remove: "#btnFormDesignVersionDelete",
                    finalized: "#btnFormDesignVersionFinalized",
                    preview: "#btnFormDesignVersionPreview",
                };
                checkApplicationClaims(claims, objMap, URLs.formDesignVersionList);
                authorizeDocumentDesignVersionList($(this), URLs.formDesignVersionList);

            },
            onSelectRow: function (rowID) {
                if (rowID !== undefined && rowID !== null) {
                    var row = $(this).getRowData(rowID);
                    if (row.StatusText == 'Finalized') {
                        $('#btnFormDesignVersionEdit').addClass('ui-icon-hide');
                        $('#btnFormDesignVersionDelete').addClass('ui-icon-hide');
                        $('#btnFormDesignVersionFinalized').addClass('ui-icon-hide');
                    }
                    else {
                        $('#btnFormDesignVersionEdit').removeClass('ui-icon-hide');
                        $('#btnFormDesignVersionDelete').removeClass('ui-icon-hide');
                        $('#btnFormDesignVersionFinalized').removeClass('ui-icon-hide');
                    }
                }
                if (ClientRolesForFormDesignAccess.indexOf(vbRole) >= 0) {
                    $('#btnFormDesignVersionEdit').addClass('ui-icon-hide');
                    $('#btnFormDesignVersionAdd').addClass('ui-icon-hide');
                    $('#btnFormDesignVersionDelete').addClass('ui-icon-hide');
                }
            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.formDesignGridJQ);
            }
        });
        var pagerElement = '#p' + elementIDs.formDesignVersionGrid;
        //remove default buttons
        $(elementIDs.formDesignVersionGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        //button in footer of grid for add 

        //Jamir : Added temp button to generate rule description for all elements and update the DB.
        //$(elementIDs.formDesignVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
        //{
        //    caption: '', buttonicon: 'ui-icon-gear', title: 'Update Rule Description', id: 'btnFormDesignVersionRuleDescription',
        //    onClickButton: function () {
        //        var rowId = $(this).getGridParam('selrow');
        //        if (rowId !== undefined && rowId !== null) {
        //            var url = URLs.generateRuledescription.replace("{formDesignId}", formDesignId).replace('{formDesignVersionId}', rowId);
        //            $.ajax({
        //                url: url,
        //                type: "POST",
        //                contentType: false,
        //                processData: false,
        //                async: false,
        //                success: function (data) {
        //                    if (data.Result == ServiceResult.SUCCESS) {
        //                        messageDialog.show('Rule description has been updated successfully.');
        //                    } else {
        //                        messageDialog.show('Something went wrong while updating rule description.');
        //                    }
        //                },
        //                error: function (err) {
        //                    messageDialog.show(err.statusText);
        //                }
        //            });
        //        }
        //    }
        //});

        $(elementIDs.formDesignVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-plus', title: 'Add', id: 'btnFormDesignVersionAdd',
            onClickButton: function () {
                //show dialog only if all versions are finalized
                var rows = $(this).getDataIDs();
                $(elementIDs.formDesignVersionDialog + ' select').empty();
                var showDialog = true;
                for (index = 0; index < rows.length; index++) {
                    row = $(this).getRowData(rows[index]);
                    $(elementIDs.formDesignVersionDialog + ' select').append(new Option(row.EffectiveDate + " - " + row.Version, row.FormDesignVersionId));
                    if (row.StatusId != 3) {
                        showDialog = false;
                        break;
                    }
                }
                if (showDialog === true) {
                    formDesignVersionDialog.show('', 'add', formDesignId, designType);
                }
                else {
                    //show message dialog
                    messageDialog.show(DocumentDesign.inProgressDesignValidationMsg);
                }
            }
        });
        //button in footer of grid for edit
        $(elementIDs.formDesignVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-pencil', title: 'Edit', id: 'btnFormDesignVersionEdit',
            onClickButton: function () {
                //allow edit only if currently selected row is in progress
                var recordCount = $(elementIDs.formDesignVersionGridJQ).getGridParam("reccount");
                if (recordCount > 0) {
                    var rowId = $(this).getGridParam('selrow');
                    if (rowId !== undefined && rowId !== null) {
                        var row = $(this).getRowData(rowId);
                        if (row.StatusId != 3) {
                            formDesignVersionDialog.show(row.EffectiveDate, 'edit', formDesignId);
                        }
                        else {
                            messageDialog.show(DocumentDesign.designModificationValidationMsg);
                        }
                    }
                    else {
                        messageDialog.show(Common.selectRowMsg);
                    }
                }
                else
                    messageDialog.show(Common.noRecordsToDisplay);
            }
        });
        //button in footer of grid for delete
        $(elementIDs.formDesignVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-trash', title: 'Delete', id: 'btnFormDesignVersionDelete',
            onClickButton: function () {
                var recordCount = $(elementIDs.formDesignVersionGridJQ).getGridParam("reccount");
                if (recordCount > 0) {
                    var rowId = $(this).getGridParam('selrow');
                    if (rowId !== undefined && rowId !== null) {
                        var row = $(this).getRowData(rowId);
                        var row = $(elementIDs.formDesignVersionGridJQ).getRowData(rowId);
                        if (row.StatusId != 3) {
                            //load Document Design dialog for delete on click - see formDesignDialog function below
                            confirmDialog.show(Common.deleteConfirmationMsg, function () {
                                confirmDialog.hide();
                                var formDesignVersionDelete = {
                                    tenantId: 1,
                                    formDesignVersionId: rowId,
                                    formDesignId: formDesignId
                                };
                                var promise = ajaxWrapper.postJSON(URLs.formDesignVersionDelete, formDesignVersionDelete);
                                //register ajax success callback
                                promise.done(formDesignSuccess);
                                //register ajax failure callback
                                promise.fail(showError);

                            });
                        }
                        else {
                            messageDialog.show(DocumentDesign.deleteDesignValidationMsg);
                        }
                    }
                    else {
                        messageDialog.show(DocumentDesign.inProgressDesignSelectionMsg);
                    }
                }
                else
                    messageDialog.show(Common.noRecordsToDisplay);
            }
        });
        //button in footer of grid for finalizing a Document Design Version
        $(elementIDs.formDesignVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-locked', title: 'Finalized', id: 'btnFormDesignVersionFinalized',
            onClickButton: function () {
                var rowId = $(elementIDs.formDesignVersionGridJQ).getGridParam('selrow');
                if (rowId != null) {
                    var row = $(elementIDs.formDesignVersionGridJQ).getRowData(rowId);
                    if (row.StatusId != 3) {
                        //show confirm dialog
                        confirmDialog.show(DocumentDesign.confirmFinalizationMsg, function () {
                            confirmDialog.hide();
                            commentsDialog.show(row.Version);
                        })
                    }
                    else {
                        messageDialog.show(DocumentDesign.inProgressDesignSelectionMsg);
                    }
                }
                else {
                    messageDialog.show(DocumentDesign.inProgressDesignSelectionMsg);
                }
            }
        });
        ////button in footer of grid for previewing a Document Design Version
        //$(elementIDs.formDesignVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
        //{
        //    caption: '', buttonicon: 'ui-icon-clipboard', title: 'Preview', id: 'btnFormDesignVersionPreview',
        //    onClickButton: function () {
        //        var rowId = $(this).getGridParam('selrow');
        //        if (rowId !== undefined && rowId !== null) {
        //            var row = $(this).getRowData(rowId);
        //            var formName = formDesignName + " " + row.Version;
        //            var url = URLs.formDesignVersionPreview.replace("{formDesignVersionId}", row.FormDesignVersionId).replace("{formName}", formName);
        //            var windowProperties = "menubar=0, addressbar=no,location=0,toolbar=0,resizable=1,status=1,scrollbars=1,height=600,width=900";
        //            var title = "eBenefit Sync - Document " + formName + " Preview";
        //            window.open(url, title, windowProperties);
        //        }
        //        else {
        //            messageDialog.show(Common.pleaseSelectRowMsg);
        //        }
        //    }
        //});
        //add filter toolbar at the top of grid

        $(elementIDs.formDesignVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon ui-icon-disk', title: 'Design Compile', id: 'btnCompileDesign',
            onClickButton: function () {

                var selectedRowId = $(elementIDs.formDesignVersionGridJQ).jqGrid('getGridParam', 'selrow');

                if (selectedRowId > 0) {
                    var sourceRowData = $(elementIDs.formDesignVersionGridJQ).jqGrid('getRowData', selectedRowId);
                    if (sourceRowData != null)
                        compileDesign(selectedRowId, sourceRowData.FormDesignVersionId);
                }
                else {
                    messageDialog.show("Please select row");
                }

            }
        });

        $(elementIDs.formDesignVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
       {
           caption: '', buttonicon: 'ui-icon ui-icon-script', title: 'Rule Compile', id: 'btnCompileRule',
           onClickButton: function () {
               var selectedRowId = $(elementIDs.formDesignVersionGridJQ).jqGrid('getGridParam', 'selrow');

               if (selectedRowId > 0) {
                   var sourceRowData = $(elementIDs.formDesignVersionGridJQ).jqGrid('getRowData', selectedRowId);
                   if (sourceRowData != null)
                       compileRule(selectedRowId, sourceRowData.FormDesignVersionId);
               }
               else {
                   messageDialog.show("Please select row");
               }

           }
       });

        $(elementIDs.formDesignVersionGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });

    }

    function loadPreviewData(response) {
        var jsonResponse = JSON.parse(response);
        var manager = new repeaterManager();
        $.templates({ frmTmpl: { markup: "#FormActionTemplate", layout: true } });
        $("#FormContainer").html($.render.frmTmpl(jsonResponse, { repeaterManager: manager }));
        manager.renderJQGrids();
        applyHoverOver();
        generateCalendar();

        $("#previewDialog").dialog({
            autoOpen: false,
            height: 500,
            width: 1000,
            modal: true,
            title: "Preview - " + jsonResponse.FormName
        });

        $("#previewDialog").dialog("open");
    }

    //add custom functionality to action column
    //used for custom formatting columns- see  jqGrid colmodel (formatter property) of Document Design Version grid 
    //sets the icons based on status of the Document Design Version column
    function actionFormatter(cellValue, options, rowObject) {
        if (rowObject.StatusId === 3) {
            return "<span id = 'fvds" + rowObject.FormDesignVersionId + "' class='ui-icon ui-icon-document view' title = 'View Design' style = 'cursor: pointer'/>";
        }
        else {
            return "<span id = 'fvds" + rowObject.FormDesignVersionId + "' class='ui-icon ui-icon-pencil edit' title = 'Modify Design' style = 'cursor: pointer'/>";
        }
    }

    //ajax callback success - reload Form Desing Version grid
    function formDesignSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(DocumentDesign.designVersionDeleteMsg);
        }
        else {
            messageDialog.show(xhr.Items[0].Messages[0]);
        }
        formDesign.loadFormDesignVersion();
    }

    function compileDesign(rowId, formDesignVersionId) {
        var url = URLs.CompileFormDesignVersion;
        var data = {
            tenantId: 1,
            formDesignVersionId: formDesignVersionId
        };

        var promise = ajaxWrapper.postJSON(url, data);

        promise.done(function (xhr) {
            if (xhr.Result === ServiceResult.SUCCESS) {
                messageDialog.show(DocumentDesign.documentCompileMsg);
            }
            else {
                messageDialog.show(DocumentDesign.compileFailureMsg);
            }
        });

        promise.fail(showError);
    }

    function compileRule(rowId, formDesignVersionId) {

        var url = URLs.CompileDocumentRule;
        var data = {
            tenantId: 1,
            formDesignVersionId: formDesignVersionId
        };

        var promise = ajaxWrapper.postJSON(url, data);

        promise.done(function (xhr) {
            if (xhr.Result === ServiceResult.SUCCESS) {
                messageDialog.show(DocumentDesign.documentRuleCompileMsg);
            }
            else {
                if (xhr.Items.length > 0) {
                    var msg = xhr.Items[0].Messages + '\n \n' + DocumentDesign.documentRuleCompileFailureMsg
                    messageDialog.show(msg);
                }
                else {
                    messageDialog.show(DocumentDesign.documentRuleCompileFailureMsg);
                }
            }
        });

        promise.fail(showError);
    }

    //initialization of the Document Design when the formDesign function is loaded in browser and invoked
    init();

    //
    return {
        loadFormDesign: function () {
            var docDesignType = parseInt($(elementIDs.documentDesignTypeDDLJQ).val());
            loadFormDesignGrid(docDesignType);
        },
        loadFormDesignVersion: function () {
            //load form design version for currently selected row of Document Design grid
            var rowId = $(elementIDs.formDesignGridJQ).getGridParam('selrow');
            var row = $(elementIDs.formDesignGridJQ).getRowData(rowId);
            loadFormDesignVersionGrid(rowId, row.DisplayText);
        },
        fillDocumentDesignDLL: function () {
            $(elementIDs.documentDesignTypeDDLJQ).empty();
            $(elementIDs.documentDesignTypeDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");
            $(elementIDs.formDesignVersionGridJQ).jqGrid("GridUnload");
            $(elementIDs.formDesignGridJQ).jqGrid("GridUnload");
            //Remove tabs with previous Design type
            var getallli = document.querySelectorAll('#createtabs li');
            for (var i = 0; li = getallli[i]; i++) {
                if (i > 0) {
                    li.parentNode.removeChild(li);
                }

            }
            fillDocumentDesignDLL();
        }
    }

}();

//contains functionality for the Document Design add/edit dialog
var formDesignDialog = function () {
    var URLs = {
        //url for Add Form Design
        formDesignAdd: '/FormDesign/Add',
        //url for Update Form Design
        formDesignUpdate: '/FormDesign/Update',
        fillSourceDesign: '/FormDesign/AnchorDesignList?tenantId=1'
    }

    //see element id's in Views\FormDesign\Index.cshtml
    var elementIDs = {
        //form design grid element
        formDesignGrid: 'fdg',
        //with hash for jquery
        formDesignGridJQ: '#fdg',
        //form design dialog element
        formDesignDialog: "#formdesigndialog",
        isMasterListRow: "#isMasterListRow",
        formDesignIsMasterList: "#ismasterlist",
        masterListlblJQ: "#masterListlbl",
        documentDesignTypeDDLJQ: '#documentDesignTypeDDL',
        sourceDesignDDLJQ: '#sourceDesignDDL',
        sourceDesignDDLRow: "#sourceDesignDDLRow",
        sourcedesignnameHelpBlock: '#sourcedesignnameHelpBlock',
        formDesignNameJQ: '#formdesignname',
        formDesignNameJQHelpBlock: '#formdesignnameHelpBlock',
        formdesignIsAliasDesign: '#formdesignIsAliasDesign',
        labelformdesignIsAliasDesign: '#labelformdesignIsAliasDesign',
        formdesignIsAliasDesignHelpBlock: '#formdesignIsAliasDesignHelpBlock',
        formdesignUsesAliasDesign: '#formdesignUsesAliasDesign',
        formdesignUsesAliasDesignHelpBlock: '#formdesignUsesAliasDesignHelpBlock',
        labelformdesignUsesAliasDesign: '#labelformdesignUsesAliasDesign',
        IsectionLockCheckbox: '#formdesignSectionLockcheckbox',
        sectionlockPanel: '#sectionlockPanel'
    };

    //maintains dialog state - add or edit
    var formDesignDialogState;
    var existingDesignName;
    var formDesignType;
    //ajax success callback - for add/edit
    function formDesignSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(DocumentDesign.designAddSuccessMsg);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
        //reload form design grid 
        formDesign.loadFormDesign();
        //reset dialog elements
        $(elementIDs.formDesignDialog + ' div').removeClass('has-error');
        $(elementIDs.formDesignNameJQHelpBlock).text(DocumentDesign.designAddNewNameValidateMsg);
        $(elementIDs.sourcedesignnameHelpBlock).text(DocumentDesign.anchortotargetlinkMsg);
        $(elementIDs.formDesignDialog).dialog('close');
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
        $(elementIDs.formDesignDialog).dialog({
            autoOpen: false,
            height: 250,
            width: 450,
            modal: true
        });
        //register event for Add/Edit button click on dialog
        $(elementIDs.formDesignDialog + ' button').on('click', function () {
            //check if name is already used
            getDocDesignList();
            var newName = $(elementIDs.formDesignDialog + ' input').val();
            var filterList = [];

            $.each(DocDesignList, function (id, item) {
                if (compareStrings(item.DisplayText, newName, true)) {
                    filterList.push(item.DisplayText);
                }
            });
            noOfDuplicate = 1;
            if (formDesignDialogState === 'add') {
                noOfDuplicate = 0;
            }

            //validate form name
            if (filterList !== undefined && filterList.length > noOfDuplicate) {
                $(elementIDs.formDesignNameJQ).parent().addClass('has-error');
                $(elementIDs.formDesignNameJQHelpBlock).text(DocumentDesign.designNameAlreadyExistsMsg);
            }
            else if (newName.trim() == '') {
                $(elementIDs.formDesignNameJQ).parent().addClass('has-error');
                $(elementIDs.formDesignNameJQHelpBlock).text(DocumentDesign.designNameRequiredMsg);
            }
            else {
                //save the new form design
                var rowId = $(elementIDs.formDesignGridJQ).getGridParam('selrow');
                var doctype = $(elementIDs.documentDesignTypeDDLJQ).val();
                var srcdesign = $(elementIDs.sourceDesignDDLJQ).val();
                var isMasterList = false;
                var isAliasDesign = false;
                var usesAliasDesign = false;
                var IsSectionLock = false;
                if (formDesignType == 2) {
                    isMasterList = true;
                    srcdesign=0;
                    if ($(elementIDs.formdesignIsAliasDesign).is(':checked')) {
                        isAliasDesign = true;
                    }
                    if ($(elementIDs.formdesignUsesAliasDesign).is(':checked')) {
                        usesAliasDesign = true;
                    }
                }
                //2	MasterList or 11	Collateral
                if (formDesignType == 2 || formDesignType == 11) {
                    if ($(elementIDs.IsectionLockCheckbox).is(':checked')) {
                        IsSectionLock = true;
                    }
                }
                var formDesignAdd = {
                    tenantId: 1,
                    formDesignId: rowId,
                    displayText: newName,
                    isMasterList: isMasterList,
                    docType: doctype,
                    srcDesign: srcdesign,
                    isAliasDesign: isAliasDesign,
                    usesAliasDesign: usesAliasDesign,
                    IsSectionLock: IsSectionLock
                };
                var url;
                if (formDesignDialogState === 'add') {
                    url = URLs.formDesignAdd;
                }
                else {
                    url = URLs.formDesignUpdate;
                }
                //ajax call to add/update
                var promise = ajaxWrapper.postJSON(url, formDesignAdd);
                //register ajax success callback
                promise.done(formDesignSuccess);
                //register ajax failure callback
                promise.fail(showError);
            }
        });
    }

    function fillSourceDesignDLL(srcDesign) {

        var url = URLs.fillSourceDesign;
        $(elementIDs.sourceDesignDDLJQ).empty();
        $(elementIDs.sourceDesignDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");
        var promise = ajaxWrapper.getJSON(url);
        //fill the folder list drop down
        promise.done(function (list) {
            for (i = 0; i < list.length; i++) {
                $(elementIDs.sourceDesignDDLJQ).append("<option value=" + list[i].FormDesignId + ">" + list[i].DisplayText + "</option>");
            }
            $(elementIDs.sourceDesignDDLJQ).val(srcDesign);

        });

        promise.fail(showError);

    }


    //initialize the dialog after this js are loaded
    init();

    //these are the properties that can be called by using formDesignDialog.<Property>
    //eg. formDesignDialog.show('name','add');
    return {
        show: function (formDesignName, action, srcDesign, designType, isAliasDesign, usesAliasDesign, aliasDesignExists, IsSectionLock) {
            formDesignType = designType;
            formDesignDialogState = action;
            existingDesignName = formDesignName;
            $(elementIDs.formDesignDialog + ' input').each(function () {
                $(this).val(formDesignName);
            });
            $(elementIDs.isMasterListRow).hide();
            //set eleemnts based on whether the dialog is opened in add or edit mode
            $(elementIDs.formDesignDialog + ' div').removeClass('has-error');
            $(elementIDs.formdesignIsAliasDesign).prop("checked", false);
            $(elementIDs.formdesignUsesAliasDesign).prop("checked", false);

            if ((formDesignType == 2 || formDesignType == 11)) {
                $(elementIDs.sectionlockPanel).show();
                $(elementIDs.IsectionLockCheckbox).prop("checked", IsSectionLock);
            }
            else {
                IsSectionLock == false;
                $(elementIDs.sectionlockPanel).hide();
            }

            //$(elementIDs.IsectionLockCheckbox).prop("checked", false);
            if (aliasDesignExists == true) {
                $(elementIDs.formdesignIsAliasDesign).prop("disabled", true);
            }
            if (formDesignType == 2) {
                if (isAliasDesign == true) {
                    $(elementIDs.formdesignIsAliasDesign).prop("checked", true);
                }
                if (usesAliasDesign == true) {
                    $(elementIDs.formdesignUsesAliasDesign).prop("checked", true);
                }

                $(elementIDs.formdesignIsAliasDesign).show();
                $(elementIDs.labelformdesignIsAliasDesign).show();
                $(elementIDs.formdesignIsAliasDesignHelpBlock).show();
                $(elementIDs.formdesignIsAliasDesignHelpBlock).text(DocumentDesign.isAliasDesignAdd);
                $(elementIDs.formdesignUsesAliasDesign).show();
                $(elementIDs.labelformdesignUsesAliasDesign).show();
                $(elementIDs.formdesignUsesAliasDesignHelpBlock).show();
                $(elementIDs.formdesignUsesAliasDesignHelpBlock).text(DocumentDesign.usesAliasDesignAdd);
            }
            else {
                $(elementIDs.formdesignIsAliasDesign).hide();
                $(elementIDs.labelformdesignIsAliasDesign).hide();
                $(elementIDs.formdesignIsAliasDesignHelpBlock).hide();
                $(elementIDs.formdesignUsesAliasDesign).hide();
                $(elementIDs.labelformdesignUsesAliasDesign).hide();
                $(elementIDs.formdesignUsesAliasDesignHelpBlock).hide();
            }
            if (formDesignDialogState == 'add') {
                $(elementIDs.formDesignDialog).dialog('option', 'title', DocumentDesign.addDocumentDesign);
                $(elementIDs.formDesignNameJQHelpBlock).text(DocumentDesign.designAddNewNameValidateMsg);
                if (formDesignType == 11 || formDesignType == 14) {
                    $(elementIDs.sourceDesignDDLRow).show();
                    $(elementIDs.sourcedesignnameHelpBlock).text(DocumentDesign.anchortotargetlinkMsg)
                    $(elementIDs.sourceDesignDDLJQ).prop("disabled", false);
                    fillSourceDesignDLL(srcDesign);
                }
                else {
                    $(elementIDs.sourceDesignDDLRow).hide();
                    srcDesign = 0;
                }
            }
            else {
                $(elementIDs.formDesignDialog).dialog('option', 'title', DocumentDesign.editDocumentDesign);
                $(elementIDs.formdesignnameHelpBlock).text(DocumentDesign.designEditNameValidateMsg);
                if (formDesignType == 11 || formDesignType == 14) {
                    $(elementIDs.sourceDesignDDLRow).show();
                    $(elementIDs.sourcedesignnameHelpBlock).text(DocumentDesign.anchortotargetlinkMsg)
                    $(elementIDs.sourceDesignDDLJQ).prop("disabled", true);
                    fillSourceDesignDLL(srcDesign);
                }
                else {
                    $(elementIDs.sourceDesignDDLRow).hide();
                    srcDesign = 0;
                }
            }
            //open the dialog - uses jqueryui dialog
            $(elementIDs.formDesignDialog).dialog("open");
        }
    }
}(); //invoked soon after loading

//contains functionality for the Document Design Version add/edit dialog
var formDesignVersionDialog = function () {
    var URLs = {
        //url for adding Document Design Version
        formDesignVersionAdd: '/FormDesign/AddVersion',
        //url for updating Document Design Version
        formDesignVersionUpdate: '/FormDesign/UpdateVersion'
    }

    var elementIDs = {
        //form design version grid
        formDesignVersionGrid: 'fdvg',
        //with hash for jquery
        formDesignVersionGridJQ: '#fdvg',
        //container for form design version add/edit dialog
        formDesignVersionDialog: "#formdesignversiondialog",

        formdesignversionEditdialog: "#formdesignversionEditdialog",

        chkbox: 'CreateFormDesignVersion',
        isCreateFormDesignVersion: 'isCreateFormDesignVersion',
        isCreateFormDesignVersionJQ: '#isCreateFormDesignVersion',
        isCopyFormDesignVersionJQ: '#isCopyFormDesignVersion',
        createFormDesignVersionEffectivedateJQ: '#createFormDesignVersionEffectivedate',
        createFormDesignVersionEffectivedateHelpblockJQ: '#createFormDesignVersionEffectivedateHelpblock',
        effectiveDate: 'FormDesignVersionEffectivedate',
        copyFormDesignVersionCheckboxHelpblock: 'copyFormDesignVersionCheckboxHelpblock',
        copyFormDesignVersionCheckboxHelpblockJQ: '#copyFormDesignVersionCheckboxHelpblock'
    };

    //dialog state - add or edit
    var formDesignVersionDialogState;
    //current Document Design ID
    var currentFormDesignId;
    //ajax success callback called after adding/editing Document Design version
    function formDesignVersionSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(DocumentDesign.designVersionAddSuccessMsg);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
        //reload the grid
        formDesign.loadFormDesignVersion();
        //reset dialog elements
        $(elementIDs.formDesignVersionDialog + ' div').removeClass('has-error');
        $(elementIDs.formDesignVersionDialog + ' .help-block').text(DocumentDesign.designAddNewNameValidateMsg);
        //close the dialog - jqueryui dialog used
        $(elementIDs.formDesignVersionDialog).dialog('close');
    }
    //ajax error callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
    ////Validate day for fabruary month 
    function daysInFebruary(year) {
        // February has 29 days in any year evenly divisible by four,
        // EXCEPT for centurial years which are not also divisible by 400.
        return (((year % 4 == 0) && ((!(year % 100 == 0)) || (year % 400 == 0))) ? 29 : 28);
    }

    //Calculate number of day in month
    function getDaysInMonth(n) {
        for (var i = 1; i <= n; i++) {
            this[i] = 31
            if (i == 4 || i == 6 || i == 9 || i == 11) { this[i] = 30 }
            if (i == 2) { this[i] = 29 }
        }
        return this
    }

    function getMinimumEffectiveDate(formDesignVersionList) {
        var min = formDesignVersionList.sort(function (a, b) {
            return new Date(a.EffectiveDate) - new Date(b.EffectiveDate);
        });
        return min[0].EffectiveDate;
    }

    //init dialog on load of page
    function init() {
        //register dialog for grid row add
        $(elementIDs.formDesignVersionDialog).dialog({
            autoOpen: false,
            height: 320,
            width: 380,
            modal: true
        });
        //select datapicker - jqueryui
        $(elementIDs.createFormDesignVersionEffectivedateJQ).datepicker({
            dateFormat: 'mm/dd/yy',
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-61:c+20',
            showOn: "both",
            //CalenderIcon path declare in golbalvariable.js
            buttonImage: Icons.CalenderIcon,
            buttonImageOnly: true
        });

        $(elementIDs.copyFormDesignVersionEffectivedateJQ).datepicker({
            dateFormat: 'mm/dd/yy',
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-61:c+20',
            showOn: "both",
            //CalenderIcon path declare in golbalvariable.js
            buttonImage: Icons.CalenderIcon,
            buttonImageOnly: true
        });

        //register event for button click to add
        $(elementIDs.formDesignVersionDialog + ' button').on('click', function () {
            //check if a version with the same effective date is already present
            var newEffectiveDate = undefined;
            var formDesignVersionId = 0;

            //get Form Version List
            var formDesignList = $(elementIDs.formDesignVersionGridJQ).getRowData();

            if ($("input[name='" + elementIDs.chkbox + "']").is(":checked")) {
                newEffectiveDate = $(elementIDs.createFormDesignVersionEffectivedateJQ).val();

                if (formDesignList.length != 0)
                    formDesignVersionId = $(elementIDs.formDesignVersionDialog + ' select').val();
            }
            else {
                newEffectiveDate = $(elementIDs.createFormDesignVersionEffectivedateJQ).val();
            }

            var checkEffectiveDate = formDesignList.length > 0 &&
                                   new Date(newEffectiveDate) < new Date(getMinimumEffectiveDate(formDesignList));

            // validation for Effective Date field
            if (newEffectiveDate == '') {
                $(elementIDs.createFormDesignVersionEffectivedateJQ).parent('div').addClass('has-error');
                $(elementIDs.createFormDesignVersionEffectivedateHelpblockJQ).text(Common.effectiveDateRequiredMsg);
            }
            else if (checkEffectiveDate === true && formDesignList.length > 1) {
                $(elementIDs.createFormDesignVersionEffectivedateJQ).parent('div').addClass('has-error');
                $(elementIDs.createFormDesignVersionEffectivedateHelpblockJQ).text(DocumentDesign.effectiveDateGreaterThanVersionMsg);
            }
            else {
                var effectiveDateMessage = isValidEffectiveDate(newEffectiveDate);
                if (effectiveDateMessage == "") {
                    //save the new form design
                    var rows = $(elementIDs.formDesignVersionGridJQ).getDataIDs();
                    var rowId = $(elementIDs.formDesignVersionGridJQ).getGridParam('selrow');

                    var isFirstVersion = false;
                    if (rows.length == 0)
                        isFirstVersion = true;
                    else {
                    }
                    //create object to be posted
                    var formDesignVersionAdd = {
                        tenantId: 1,
                        formDesignId: currentFormDesignId,
                        formDesignVersionId: formDesignVersionId,
                        isFirstVersion: isFirstVersion,
                        effectiveDate: newEffectiveDate
                    };
                    var url;
                    //add or edit based on the current mode the dialog is opened in
                    if (formDesignVersionDialogState === 'add') {
                        url = URLs.formDesignVersionAdd;
                    }
                    else {
                        var rowId = $(elementIDs.formDesignVersionGridJQ).getDataIDs();
                        var formDesignVersionUpdate = {
                            tenantId: 1,
                            formDesignVersionId: currentFormDesignId,
                        }
                        url = URLs.formDesignVersionUpdate;
                    }
                    //ajax call for post
                    var promise = ajaxWrapper.postJSON(url, formDesignVersionAdd);
                    //register success callback
                    promise.done(formDesignVersionSuccess);
                    //register failure callback
                    promise.fail(showError);
                }
                else {
                    $(elementIDs.createFormDesignVersionEffectivedateJQ).parent('div').addClass('has-error');
                    $(elementIDs.createFormDesignVersionEffectivedateHelpblockJQ).text(effectiveDateMessage);
                }
            }
        });

        //register event for Check box change to add a new version
        $("input[name='" + elementIDs.chkbox + "']").on('change', function () {
            if ($(this).is(":checked")) {
                var formDesignList = $(elementIDs.formDesignVersionGridJQ).getRowData();

                if (formDesignList.length == 0) {
                    $(elementIDs.formDesignVersionDialog + ' select').attr('disabled', 'disabled');
                }
                else {
                    //enable copy version drop down list
                    $(elementIDs.formDesignVersionDialog + ' select').removeAttr("disabled");

                    // Set first element in the dropdown list as selected
                    $(elementIDs.formDesignVersionDialog + ' select option:first').attr('selected', 'selected');
                }
            }
            else {
                //disable copy version drop down list
                $(elementIDs.formDesignVersionDialog + ' select').attr('disabled', 'disabled');
            }
        });

        //register dialog for grid edit
        $(elementIDs.formdesignversionEditdialog).dialog({
            autoOpen: false,
            height: 250,
            width: 450,
            modal: true
        });
        //select datapicker - jqueryui
        $(elementIDs.formdesignversionEditdialog + ' input').datepicker({
            dateFormat: 'mm/dd/yy',
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-61:c+20',
            showOn: "both",
            //CalenderIcon path declare in golbalvariable.js
            buttonImage: Icons.CalenderIcon,
            buttonImageOnly: true
        });

        // register event for button click to edit
        $(elementIDs.formdesignversionEditdialog + ' button').on('click', function () {
            //check if a version with the same effective date is already present
            var newEffectiveDate = $(elementIDs.formdesignversionEditdialog + ' input').val();
            var formDesignList = $(elementIDs.formDesignVersionGridJQ).getRowData();

            var checkEffectiveDate = formDesignList.length > 0 &&
                                        new Date(newEffectiveDate) < new Date(getMinimumEffectiveDate(formDesignList));


            if (newEffectiveDate == '') {
                $(elementIDs.formdesignversionEditdialog + ' div').addClass('has-error');
                $(elementIDs.formdesignversionEditdialog + ' .help-block').text(Common.effectiveDateRequiredMsg);
            }
            else if (checkEffectiveDate === true && formDesignList.length > 1) {
                $(elementIDs.formdesignversionEditdialog + ' div').addClass('has-error');
                $(elementIDs.formdesignversionEditdialog + ' .help-block').text(DocumentDesign.effectiveDateGreaterThanVersionMsg);
            }
            else {
                var effectiveDateMessage = isValidEffectiveDate(newEffectiveDate);
                if (effectiveDateMessage == "") {
                    //save the new form design
                    var rows = $(elementIDs.formDesignVersionGridJQ).getDataIDs();
                    var rowId = $(elementIDs.formDesignVersionGridJQ).getGridParam('selrow');

                    var isFirstVersion = false;
                    if (rows.length == 0) {
                        isFirstVersion = true;
                    }
                    else {
                    }
                    //create object to be posted
                    var formDesignVersionAdd = {
                        tenantId: 1,
                        formDesignId: currentFormDesignId,
                        formDesignVersionId: rowId,
                        isFirstVersion: isFirstVersion,
                        effectiveDate: newEffectiveDate
                    };
                    var url;
                    //add or edit based on the current mode the dialog is opened in

                    var rowId = $(elementIDs.formDesignVersionGridJQ).getDataIDs();
                    var formDesignVersionUpdate = {
                        tenantId: 1,
                        formDesignVersionId: currentFormDesignId,
                    }
                    url = URLs.formDesignVersionUpdate;

                    //ajax call for post
                    var promise = ajaxWrapper.postJSON(url, formDesignVersionAdd);
                    //register success callback
                    promise.done(formDesignVersionSuccess);
                    //register failure callback
                    promise.fail(showError);
                }
                else {
                    $(elementIDs.formdesignversionEditdialog + ' div').addClass('has-error');
                    $(elementIDs.formdesignversionEditdialog + ' .help-block').text(effectiveDateMessage);
                }
            }
        });
    }
    init();
    //initialize the dialog soon after this js is loaded   
    //these are the properties that can be called by using formDesignVersionDialog.<Property>
    //eg. formDesignVersionDialog.show('name','add',formDesignId);
    return {
        //open the dialog

        show: function (formDesignName, action, formDesignId) {
            formDesignVersionDialogState = action;
            currentFormDesignId = formDesignId;
            $(elementIDs.formDesignVersionDialog + ' input').each(function () {
                $(this).val(formDesignName);
            });

            //set based elements for add or edit based on the mode the dialog is opened in
            $(elementIDs.formDesignVersionDialog + ' div').removeClass('has-error');

            if (formDesignVersionDialogState == 'add') {
                // $('label[for="' + elementIDs.effectiveDate + '"]').addClass('labelfocus');

                var formDesignList = $(elementIDs.formDesignVersionGridJQ).getRowData();

                // Disable dropdwonlist if Version list if empty
                if (formDesignList.length == 0) {
                    $(elementIDs.formDesignVersionDialog + ' select').attr('disabled', 'disabled');
                }
                else {
                    $(elementIDs.formDesignVersionDialog + ' select').removeAttr('disabled');
                }

                //enable create new document all elements
                $(elementIDs.createFormDesignVersionEffectivedateJQ).removeAttr('disabled');
                $(elementIDs.createFormDesignVersionEffectivedateJQ).siblings().removeAttr('disabled');
                $(elementIDs.formDesignVersionDialog + ' select').attr('disabled', 'disabled');

                //set checkbox value false by default         
                if ($("input[name='" + elementIDs.chkbox + "']").is(":checked")) {
                    $("input[name='" + elementIDs.chkbox + "']").removeAttr('checked');
                }

                $(elementIDs.formDesignVersionDialog).dialog('option', 'title', DocumentDesign.designAddVersionMsg);
                $(elementIDs.formDesignVersionDialog + ' .help-block').text(DocumentDesign.designAddNewVersionValidateEffectiveDateMsg);

                $(elementIDs.copyFormDesignVersionCheckboxHelpblockJQ).text(DocumentDesign.designAddNewVerionValidateCheckBoxMsg);
                //open dialog - uses jqueryui dialog
                $(elementIDs.formDesignVersionDialog).dialog("open");
            }
            else {
                $(elementIDs.formdesignversionEditdialog + ' div').removeClass('has-error');
                $(elementIDs.formdesignversionEditdialog).dialog('option', 'title', DocumentDesign.editDocumentDesignVersion);
                $(elementIDs.formdesignversionEditdialog + ' .help-block').text(DocumentDesign.designEditVersionValidateEffectiveDateMsg);
                //open dialog - uses jqueryui dialog
                $(elementIDs.formdesignversionEditdialog).dialog("open");
            }
        }
    }
}(); //invoked soon after loading

//contains functionality for adding comments before finalizing Document Design version 
var commentsDialog = function () {
    var formDesignVersion;
    var URLs = {
        //Finalize Form Design
        formDesignVersionFinalize: '/FormDesign/FinalizeVersion',
        getVersionNumber: '/FormDesign/GetVersionNumber?tenantId=1&formDesignVersionId={formDesignVersionId}',
    };
    var elementIDs = {
        //id for save button
        saveButton: "saveButton",
        //id for textarea of comments
        comments: "#comments",
        //id for dialog
        commentsDialog: "#commentsDialog",
        //form design version grid
        formDesignVersionGrid: 'fdvg',
        //with hash for jquery
        formDesignVersionGridJQ: '#fdvg',
    }

    //ajax callback success - reload Form Desing Version grid
    function VersionFinalizeSuccess(xhr) {
        if (xhr.Items[0]) {
            var message = '';
            for (var i = 0; i < xhr.Items[0].Messages.length; i++) {
                message += xhr.Items[0].Messages[i] + ", ";
            }
            messageDialog.show(DocumentDesign.finalizationCannotPerformMsg + message);
        }
        else if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(DocumentDesign.finalizationSuccessMsg);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
        formDesign.loadFormDesignVersion();

        //To find formDesignVersionID
        var data = this.data;
        var array = []; var innerArray = [];
        array = data.split("&");
        innerArray = array[1].split("=");
        var formDesignVersionID = innerArray[1];
        //submit using ajax for finalization

        var url = URLs.getVersionNumber.replace(/{formDesignVersionId}/g, formDesignVersionID);
        var promise = ajaxWrapper.getJSON(url);

        promise.done(function (result) {
            //Check if version number is changed or not after finalizing form design version.
            if (result != formDesignVersion) {
                //Close tab for old version number and open tab for new version number.
                var tab = tabs.find('span[data-tabid=' + formDesignVersionID + ']')
                var panelId = $(tab).closest('li').remove().attr('aria-controls');
                $('#' + panelId).remove();
                $('#fvds' + formDesignVersionID).trigger("click");
            }
        });

        $('.ui-icon-close').each(function () {
            //While finalizing form Design Version, if tab of that version is opened then set global variable.
            if ($(this).attr(("data-tabid")) == formDesignVersionID) {
                Finalized.push({
                    FORMDESIGNVERSIONID: formDesignVersionID,
                    ISFINALIZED: 1,
                    ISMESSAGEDISPLAYED: false
                });
            }
        })
    }
    //handler for ajax errors
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    function init() {
        //register dialog 
        $(elementIDs.commentsDialog).dialog({
            autoOpen: false,
            height: 200,
            width: 400,
            modal: true
        });
        $(elementIDs.commentsDialog + ' button').on('click', function () {
            var rowId = $(elementIDs.formDesignVersionGridJQ).getGridParam('selrow');
            var finalizingComments;
            //Comments added by user
            finalizingComments = $(elementIDs.comments).val();

            var formDesignFinalize = {
                tenantId: 1,
                formDesignVersionId: rowId,
                comments: finalizingComments
            };
            //submit using ajax for finalization
            var promise = ajaxWrapper.postJSON(URLs.formDesignVersionFinalize, formDesignFinalize);
            //To close comments dialog.
            $(elementIDs.commentsDialog).dialog("close");
            //success callback
            promise.done(VersionFinalizeSuccess);
            //failure callback
            promise.fail(showError);
        });
    }
    //initialize the dialog after this js is loaded
    init();
    return {
        show: function (version) {
            formDesignVersion = version;
            $(elementIDs.commentsDialog).dialog('option', 'title', DocumentDesign.designAddComments);
            //To refresh textarea of comments
            $(elementIDs.comments).val("");
            //open the dialog - uses jqueryui dialog
            $(elementIDs.commentsDialog).dialog("open");
        }
    }
}();


var documentdesigndialog = function () {
    var URLs = {
        //url for Add Document Design Type
        formDesignAdd: '/FormDesign/AddType'
    }

    //see element id's in Views\FormDesign\Index.cshtml
    var elementIDs = {
        documentdesigndialog: "#documentdesigndialog",
        documentDesignTypeDDLJQ: '#documentDesignTypeDDL',
    };

    //ajax success callback - for add/edit
    function docDesignSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(DocumentDesign.docdesignAddSuccessMsg);
            formDesign.fillDocumentDesignDLL();
        }
        else {
            messageDialog.show(Common.errorMsg);
        }

        //reset dialog elements
        $(elementIDs.documentdesigndialog + ' div').removeClass('has-error');
        $(elementIDs.documentdesigndialog + ' .help-block').text(DocumentDesign.documentdesignAddNewNameValidateMsg);
        $(elementIDs.documentdesigndialog).dialog('close');
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
        $(elementIDs.documentdesigndialog).dialog({
            autoOpen: false,
            height: 150,
            width: 450,
            modal: true
        });
        //register event for Add button click on dialog
        $(elementIDs.documentdesigndialog + ' button').on('click', function () {
            //check if name is already used
            var newName = $(elementIDs.documentdesigndialog + ' input').val();
            var opts = $(elementIDs.documentDesignTypeDDLJQ)[0].options;
            var docTypeList = $.map(opts, function (elem) {
                return (elem.text);
            });

            var filterList = docTypeList.filter(function (ct) {
                return compareStrings(ct, newName, true);
            });

            //validate form name
            if (filterList !== undefined && filterList.length > 0) {
                $(elementIDs.documentdesigndialog + ' div').addClass('has-error');
                $(elementIDs.documentdesigndialog + ' .help-block').text(DocumentDesign.documentdesignNameAlreadyExistsMsg);
            }
            else if (newName == '') {
                $(elementIDs.documentdesigndialog + ' div').addClass('has-error');
                $(elementIDs.documentdesigndialog + ' .help-block').text(DocumentDesign.documentdesignNameRequiredMsg);
            }
            else {
                //save the new form design
                var formDesignAdd = {
                    displayText: newName
                };
                var url;
                url = URLs.formDesignAdd;

                //ajax call to add/update
                var promise = ajaxWrapper.postJSON(url, formDesignAdd);
                //register ajax success callback
                promise.done(docDesignSuccess);
                //register ajax failure callback
                promise.fail(showError);
            }
        });
    }
    //initialize the dialog after this js are loaded
    init();

    return {
        show: function () {

            $(elementIDs.documentdesigndialog + ' div').removeClass('has-error');

            $(elementIDs.documentdesigndialog).dialog('option', 'title', DocumentDesign.addDocumentDesignType);
            $(elementIDs.documentdesigndialog + ' .help-block').text(DocumentDesign.documentdesignAddNewNameValidateMsg);
            $(elementIDs.documentdesigndialog).dialog("open");
        }
    }
}();

function getDocDesignList() {
    var URLs = {
        fillDocDesign: '/FormDesign/FormDesignList?tenantId=1'
    }
    while (DocDesignList.length > 0) {
        DocDesignList.pop();
    }
    var url = URLs.fillDocDesign;
    var promise = ajaxWrapper.getJSONSync(url);
    //fill the folder list drop down
    promise.done(function (list) {
        for (i = 0; i < list.length; i++) {
            DocDesignList.push(list[i]);
        }
    });

    promise.fail(showError);

}
var DocDesignList = [];

var formDesignGroup = function () {
    var tabs;
    var tabIndex = 1;
    var tabCount = 0;
    var tabNamePrefix = 'formDesign-';
    //urls to be accessed for form design
    var URLs = {
        formDesignGroupList: '/FormDesignGroup/FormDesignGroupList?tenantId=1',
        formDesignGroupMappingList: '/FormDesignGroup/FormGroupMappingList?tenantId=1&formGroupId={formGroupId}',
        formGroupSave: '/FormDesignGroup/UpdateFormGroupMapping'
    };
    //element ID's required for form design
    var elementIDs = {
        formDesignGroupGrid: 'fdg',
        formDesignGroupGridJQ: '#fdg',
        formDesignGroupMappingGrid: 'fdvg',
        formDesignGroupMappingGridJQ: '#fdvg',
        formDesignGroupDialog: "#formdesigngroupdialog",
    };

    //init the tab on load of the form design screen
    function init() {
        $(document).ready(function () {
            loadFormDesignGroupGrid();
        });
    }

    //load form design list in grid
    function loadFormDesignGroupGrid() {
        //set column list for grid
        var colArray = ['Tenant', 'Document GroupId', 'Folder Name'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true, search: false, align: 'right' });
        colModel.push({ name: 'FormGroupId', index: 'FormGroupId', key: true, hidden: true, align: 'right' });
        colModel.push({ name: 'FormDesignGroupName', index: 'FormDesignGroupName', editable: false, search: true, align: 'left' });

        $(elementIDs.formDesignGroupGridJQ).jqGrid('GridUnload');

        var url = URLs.formDesignGroupList;
        $(elementIDs.formDesignGroupGridJQ).parent().append("<div id='p" + elementIDs.formDesignGroupGrid + "'></div>");
        //load grid
        $(elementIDs.formDesignGroupGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Document Folder List',
            height: '350',
            rowNum: 10000,
            loadonce: true,
            ignoreCase: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.formDesignGroupGrid,
            sortname: 'FormGroupId',
            altclass: 'alternate',
            onSelectRow: function (id) {
                var row = $(this).getRowData(id);
                loadFormDesignGroupMappingGrid(row.TenantID, row.FormGroupId, row.FormDesignGroupName);
            },
            gridComplete: function () {
                var rows = $(this).getDataIDs();
                var rowId;
                if (rows.length > 0) {
                    rowId = rows[0];
                }
                var newName = $(elementIDs.formDesignGroupDialog + ' input').val();
                var newId;
                for (index = 0; index < rows.length; index++) {
                    row = $(this).getRowData(rows[index]);
                    if (newName === row.FormDesignGroupName) {
                        newId = row.FormGroupId;
                        break;
                    }
                }
                if (newId !== undefined) {
                    $(this).jqGrid('setSelection', newId);
                }
                else {
                    $(this).jqGrid('setSelection', rowId);
                }

                //to check for claims.             
                var objMap = {
                    add: "#btnFormDesignGroupAdd",
                    edit: "#btnFormDesignGroupEdit"
                };
                checkApplicationClaims(claims, objMap, URLs.formDesignGroupList);
            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.formDesignGroupGridJQ);
            }


        });
        var pagerElement = '#p' + elementIDs.formDesignGroupGrid;
        //remove default buttons
        $(elementIDs.formDesignGroupGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        $(elementIDs.formDesignGroupGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-plus', id: 'btnFormDesignGroupAdd',
            onClickButton: function () {
                formDesignGroupDialog.show('', 'add');
            }
        });
        $(elementIDs.formDesignGroupGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-pencil', id: 'btnFormDesignGroupEdit',
            onClickButton: function () {
                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    var row = $(this).getRowData(rowId);
                    formDesignGroupDialog.show(row.FormDesignGroupName, 'edit');
                }
            }
        });

        $(elementIDs.formDesignGroupGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });
    }


    //load form design verion in grid
    function loadFormDesignGroupMappingGrid(tenantId, formGroupId, formDesignGroupName) {
        //set column list
        var colArray = ['TenantId', 'DocumentDesignId', 'Document Design Name', 'Abbreviation', 'Sequence', 'AllowMultipleInstance', 'Multiple Instance', 'IsIncluded', 'Include'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'TenantId', index: 'TenantId', hidden: true, search: false, });
        colModel.push({ name: 'FormDesignId', index: 'FormDesignId', hidden: true, search: false, });
        colModel.push({ name: 'FormDesignName', index: 'FormDesignName', hidden: false, width: 100, align: 'left', });
        colModel.push({ name: 'Abbreviation', index: 'Abbreviation', hidden: false, width: 100, align: 'left' });
        colModel.push({ name: 'Sequence', index: 'Sequence', hidden: true, search: false });
        colModel.push({ name: 'AllowMultipleInstance', index: 'AllowMultipleInstance', hidden: true, align: 'center', search: false });
        colModel.push({
            name: 'MultipleInstance', index: 'AllowMultipleInstance', align: 'left', editable: false, align: 'center', edittype: 'checkbox',
            editoptions: { value: 'Yes:No', defaultValue: 'No' },
            stype: 'select',
            searchoptions: {
                sopt: ['eq', 'ne'],
                value: ':Any;true:Yes;false:No'
            }, formatter: this.formatAllowMultiplenstanceColumn, unformat: this.unFormatAllowMultiplenstanceColumn
        });
        colModel.push({ name: 'IsIncluded', index: 'IsIncluded', hidden: true, search: false });
        colModel.push({
            name: 'Include', index: 'IsIncluded', editable: false, align: 'center', edittype: 'checkbox',
            editoptions: { value: 'Yes:No', defaultValue: 'No' },
            stype: 'select',
            searchoptions: {
                sopt: ['eq', 'ne'],
                value: ':Any;true:Yes;false:No'
            }, formatter: this.formatIncludedColumn, unformat: this.unFormatIncludedColumn
        });

        //get URL for grid
        var formDesignGroupMappingListUrl = URLs.formDesignGroupMappingList.replace(/\{formGroupId\}/g, formGroupId);

        //unload previous grid values
        $(elementIDs.formDesignGroupMappingGridJQ).jqGrid('GridUnload');

        $(elementIDs.formDesignGroupMappingGridJQ).parent().append("<div id='p" + elementIDs.formDesignGroupMappingGrid + "'></div>");
        //load grid
        $(elementIDs.formDesignGroupMappingGridJQ).jqGrid({
            url: formDesignGroupMappingListUrl,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Document Folder List - ' + formDesignGroupName,
            forceFit: true,
            height: '350',
            loadonce: true,
            ignoreCase: true,
            autowidth: true,
            ExpandColumn: 'Label',
            viewrecords: true,
            hidegrid: false,
            rowNum: 10000,
            altRows: true,
            pager: '#p' + elementIDs.formDesignGroupMappingGrid,
            sortname: 'TenantId',
            altclass: 'alternate',
            gridComplete: function () {
                var rows = $(this).getDataIDs();
                for (index = 0; index < rows.length; index++) {
                    row = $(this).getRowData(rows[index]);
                }
                //to check for claims.             
                var objMap = {                  
                    save: "#btnFormDesignGroupListSave"
                };
                checkApplicationClaims(claims, objMap, URLs.formDesignGroupMappingList);
                //authorizePropertyGrid($(this), URLs.formDesignGroupList);
            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.formDesignGroupMappingGridJQ);
        }
        });


        $(elementIDs.formDesignGroupMappingGridJQ).jqGrid('sortableRows');

        $(elementIDs.formDesignGroupMappingGridJQ).on("sortstop", function (event, ui) {
            var rows = $(elementIDs.formDesignGroupMappingGridJQ).getDataIDs();
            for (index = 0; index < rows.length; index++) {
                row = $(elementIDs.formDesignGroupMappingGridJQ).getRowData(rows[index]);
                row.Sequence = index + 1;
                $(elementIDs.formDesignGroupMappingGridJQ).setCell(rows[index], 4, index + 1);
            }
        });

        var pagerElement = '#p' + elementIDs.formDesignGroupMappingGrid;
        //remove default buttons
        $(elementIDs.formDesignGroupMappingGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();

        $(elementIDs.formDesignGroupMappingGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: 'Save', id: 'btnFormDesignGroupListSave',
            onClickButton: function () {
                //var formDesignList = $(this).getRowData();
                var formDesignList = $(this).jqGrid('getGridParam', 'data');
                var selectedFormGroupId = CreateFormDesignList(formDesignList, formGroupId);
            }
        });

        $('#pfdvg_left').find("tr").children().last().find('div').css('padding-top', '5px');

        $(elementIDs.formDesignGroupMappingGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });



        //Update FormDesignSave function on save button click
        function CreateFormDesignList(formDesignList, formGroupId) {
            var rowListlen = formDesignList.length;

            var formDesignRows = new Array(rowListlen);

            for (var i = 0; i < rowListlen; i++) {

                var formesigndRow = new Object();
                formesigndRow.FormDesignId = formDesignList[i].FormDesignId;
                formesigndRow.IsIncluded = $("#include" + formDesignList[i].FormDesignId).is(":checked") || (formDesignList[i].IsIncluded && $("#include" + formDesignList[i].FormDesignId).length == 0);
                formesigndRow.AllowMultipleInstance = $("#allowMultipleSelection" + formDesignList[i].FormDesignId).is(":checked") || (formDesignList[i].AllowMultipleInstance && $("#allowMultipleSelection" + formDesignList[i].FormDesignId).length == 0);
                formesigndRow.Sequence = formDesignList[i].Sequence;
                formDesignRows[i] = formesigndRow
            }

            var formDesignsToAdd = {
                formDesignRows: formDesignRows,
                tenantId: 1,
                formGroupId: formGroupId,
            };

            //save the updated form design list atached to the formgroup
            var url = URLs.formGroupSave;
            var promise = ajaxWrapper.postJSONCustom(url, formDesignsToAdd);
            promise.done(formGroupSuccess);
            promise.fail(showError);
        }

        function formGroupSuccess(xhr) {
            if (xhr.Result === ServiceResult.SUCCESS) {
                messageDialog.show(DocumentGroupDesign.designMapSuccessMsg);
            }
            else {
                messageDialog.show(Common.errorMsg);
            }
            formDesignGroup.loadFormDesignGroupMapping();
        }


    }

    //format the grid column based on element property
    formatAllowMultiplenstanceColumn = function (cellValue, options, rowObject) {
        var result;
        if (rowObject.AllowMultipleInstance == true)
            result = '<input type="checkbox"' + ' id = allowMultipleSelection' + rowObject.FormDesignId + ' name=' + rowObject.FormDesignName + 'AllowMultiple' + ' checked="checked"/>';
        else
            result = '<input type="checkbox"' + ' id = allowMultipleSelection' + rowObject.FormDesignId + ' name=' + rowObject.FormDesignName + 'AllowMultiple' + ' />';
        return result;
    }

    //unformat the grid column based on element property
    unFormatAllowMultiplenstanceColumn = function (cellValue, options, rowObject) {
        var result;
        result = $(this).find('#' + options.rowId).find('input[name=' + rowObject.FormDesignName + 'AllowMultiple]').prop('checked');
        return result;
    }

    //format the grid column based on element property
    formatIncludedColumn = function (cellValue, options, rowObject) {
        var result;
        if (rowObject.IsIncluded == true)
            result = '<input type="checkbox"' + ' id = include' + rowObject.FormDesignId + ' name=' + rowObject.FormDesignName + 'Included' + ' checked/>';
        else
            result = '<input type="checkbox"' + ' id = include' + rowObject.FormDesignId + ' name=' + rowObject.FormDesignName + 'Included' + ' />';
        return result;
    }

    //unformat the grid column based on element property
    unFormatIncludedColumn = function (cellValue, options, rowObject) {
        var result;
        result = $(this).find('#' + options.rowId).find('input[name=' + rowObject.FormDesignName + 'Included]').prop('checked');
        return result;
    }

    //handler for ajax errors
    function showError(xhr) {
        if (xhr.status == 999)
            window.location.href = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
    init();

    return {
        loadFormDesignGroup: function () {
            loadFormDesignGroupGrid();
        },

        loadFormDesignGroupMapping: function () {
            //load form design version for currently selected row of Document Design grid
            var rowId = $(elementIDs.formDesignGroupGridJQ).getGridParam('selrow');
            var row = $(elementIDs.formDesignGroupGridJQ).getRowData(rowId);
            loadFormDesignGroupMappingGrid(row.TenantID, row.FormGroupId, row.FormDesignGroupName);
        }
    }
}();

var formDesignGroupDialog = function () {
    var URLs = {
        formDesignGroupAdd: '/FormDesignGroup/Add',
        formDesignGroupUpdate: '/FormDesignGroup/Update'
    }

    var elementIDs = {
        formDesignGroupGrid: 'fdg',
        formDesignGroupGridJQ: '#fdg',
        formDesignGroupDialog: "#formdesigngroupdialog"
    };

    var formDesignGroupDialogState;

    function formDesignGroupSuccess(xhr) {        
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(DocumentGroupDesign.addFolderSuccessMsg);
        }
        else {
            messageDialog.show(Common.errorMsg); 
        }
        //Reload grid
        formDesignGroup.loadFormDesignGroup();
        $(elementIDs.formDesignGroupDialog + ' div').removeClass('has-error');
        $(elementIDs.formDesignGroupDialog + ' .help-block').text(DocumentDesign.designAddNewNameValidateMsg);
        $(elementIDs.formDesignGroupDialog).dialog('close');
    }

    function showError(xhr) {
        if (xhr.status == 999)
            window.location.href = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    function init() {
        //register dialog for grid row add/edit
        $(elementIDs.formDesignGroupDialog).dialog({
            autoOpen: false,
            height: 250,
            width: 450,
            modal: true
        });
        $(elementIDs.formDesignGroupDialog + ' button').on('click', function () {
            //check if name is already used
            var newName = $(elementIDs.formDesignGroupDialog + ' input').val();
            var formDesignList = $(elementIDs.formDesignGroupGridJQ).getRowData();


            var filterList = formDesignList.filter(function (ct) {
                return compareStrings(ct.FormDesignGroupName, newName, true);
            });

            //validate form name
            if (filterList !== undefined && filterList.length > 0) {
                $(elementIDs.formDesignGroupDialog + ' div').addClass('has-error');
                $(elementIDs.formDesignGroupDialog + ' .help-block').text(DocumentDesign.designNameAlreadyExistsMsg);
            }
            else if (newName == '') {
                $(elementIDs.formDesignGroupDialog + ' div').addClass('has-error');
                $(elementIDs.formDesignGroupDialog + ' .help-block').text(DocumentDesign.designNameRequiredMsg);
            }
            else {
                //save the new form design
                var rowId = $(elementIDs.formDesignGroupGridJQ).getGridParam('selrow');
                var row = $(elementIDs.formDesignGroupGridJQ).getRowData(rowId);
                var formDesignGroupToAdd = {
                    tenantId: 1,
                    formGroupId: row.FormGroupId,
                    groupName: newName
                };
                var url;
                if (formDesignGroupDialogState === 'add') {
                    url = URLs.formDesignGroupAdd;
                }
                else {
                    url = URLs.formDesignGroupUpdate;
                }
                var promise = ajaxWrapper.postJSON(url, formDesignGroupToAdd);
                promise.done(formDesignGroupSuccess);
                promise.fail(showError);
            }
        });
    }
    init();

    return {
        show: function (formDesignName, action) {
            formDesignGroupDialogState = action;
            $(elementIDs.formDesignGroupDialog + ' input').each(function () {
                $(this).val(formDesignName);
            });
            $(elementIDs.formDesignGroupDialog + ' div').removeClass('has-error');
            if (formDesignGroupDialogState == 'add') {
                $(elementIDs.formDesignGroupDialog).dialog('option', 'title', DocumentGroupDesign.addFolderMsg);
                $(elementIDs.formDesignGroupDialog + ' .help-block').text(DocumentGroupDesign.addFolderNameMsg);
            }
            else {
                $(elementIDs.formDesignGroupDialog).dialog('option', 'title', DocumentGroupDesign.editFolderMsg);
                $(elementIDs.formDesignGroupDialog + ' .help-block').text(DocumentGroupDesign.editFolderNameMsg);
            }
            $(elementIDs.formDesignGroupDialog).dialog("open");
        }
    }
}();

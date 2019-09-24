
var roleAccessPermissionDialog = function (UIElement, formDesignVersionId) {
    this.uiElement = UIElement;
    this.formDesignVersionId = formDesignVersionId;

}

roleAccessPermissionDialog.URLs = {
    userRoleList: '/Identity/GetElementAccessPermissionSet?elementId={elementId}',
    roleAccessPermissionSave: '/Identity/AddElementAccessPermissionSet'
}

roleAccessPermissionDialog._currentInstance = undefined;
roleAccessPermissionDialog._isInitialized = false;
roleAccessPermissionDialog.elementIDs = {
    roleAccessPermissionDialog: '#roleaccesspermission',
    roleAccessPermissionGrid: 'roleaccesspermissiongrid',
    roleAccessPermissionJQ: '#roleaccesspermissiongrid',
    roleAccessPermissionAllViewCheckbox: 'input[id^=viewcheckbox_]',
    roleAccessPermissionAllEditCheckbox: 'input[id^=editcheckbox_]'
}

//init dialog
roleAccessPermissionDialog.init = function () {
    if (roleAccessPermissionDialog._isInitialized == false) {
        $(roleAccessPermissionDialog.elementIDs.roleAccessPermissionDialog).dialog({
            autoOpen: false,
            height: 'auto',
            width: '50%',
            modal: true
        });
        roleAccessPermissionDialog._isInitialized = true;
    }
}();

roleAccessPermissionDialog.prototype.show = function (statustext) {
    roleAccessPermissionDialog._currentInstance = this;
    if (statustext == "Finalized") {
        roleAccessPermissionDialog._currentInstance.statustext = statustext;
    }

    $(roleAccessPermissionDialog.elementIDs.roleAccessPermissionDialog).dialog('option', 'title', DocumentDesign.roleAccessPermissionDialog + " - " + this.uiElement.Label)
    $(roleAccessPermissionDialog.elementIDs.roleAccessPermissionDialog).dialog('open');

    this.loadroleAccessPermissionGrid(this.formDesignId, this.formDesignVersionId, this.uiElement.UIElementID);
    $(roleAccessPermissionDialog.elementIDs.roleAccessPermissionDialog).dialog({
        position: {
            my: 'center',
            at: 'center'
        },
    });
}

roleAccessPermissionDialog.prototype.loadroleAccessPermissionGrid = function (formDesignId, formDesignVersionId, UIElementID) {
    var currentInstance = this;
    var getsectionAccessURL = roleAccessPermissionDialog.URLs.userRoleList.replace(/\{elementId\}/g, UIElementID);
    var colArray;
    var colModel = [];
    var viewAllCheckbox = '<input type="checkbox" class="check-all" id="viewallcheckbox" />';
    var editAllCheckbox = '<input type="checkbox" class="check-all" id="editallcheckbox" />';
    //set column list for grid   
    colArray = ['', '', 'ResourceID', viewAllCheckbox, editAllCheckbox];
    //set column models
    colModel.push({ name: 'RoleID', index: 'RoleID', editable: false, align: 'left', hidden: true });
    colModel.push({ name: 'RoleName', index: 'RoleName', editable: false, align: 'left', hidden: false });
    colModel.push({ name: 'ResourceID', index: 'SectionId', editable: false, align: 'left', hidden: true });
    colModel.push({ name: 'IsVisible', sortable: false, index: 'IsVisible', hidden: false, align: 'center', formatter: checkBoxFormatterDesign, unformat: unFormatIncludedColumnDesign, width: 50, resizable: true });
    colModel.push({ name: 'IsEditable', sortable: false, index: 'IsEditable', hidden: false, align: 'center', formatter: checkBoxFormatterDesign, unformat: unFormatIncludedColumnDesign, width: 50, resizable: true });
    //clean up the grid first - only table element remains after this

    $(roleAccessPermissionDialog.elementIDs.roleAccessPermissionJQ).jqGrid('GridUnload');

    //adding the pager element
    $(roleAccessPermissionDialog.elementIDs.roleAccessPermissionJQ).parent().append("<div id='p" + roleAccessPermissionDialog.elementIDs.roleAccessPermissionGrid + "'></div>");

    $(roleAccessPermissionDialog.elementIDs.roleAccessPermissionJQ).jqGrid({
        datatype: 'json',
        url: getsectionAccessURL,//roleAccessPermissionDialog.URLs.userRoleList,
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Role Access Permissions',
        pager: '#p' + roleAccessPermissionDialog.elementIDs.roleAccessPermissionGrid,
        height: '200',
        rowheader: true,
        loadonce: true,
        rowNum: 10000,
        autowidth: true,
        gridview: true,
        viewrecords: true,
        altRows: true,
        altclass: 'alternate',
        gridComplete: function () {
            initializeCheckAll();
            $(roleAccessPermissionDialog.elementIDs.roleAccessPermissionAllEditCheckbox).change(function () {
                if ($(this).is(':checked') == true) {
                    var targetId = $(this).attr('id').replace('editcheckbox_', 'viewcheckbox_');
                    $('#' + targetId).prop('checked', true).trigger('change');
                }
                setEditAllCheckbox();
            });
            $(roleAccessPermissionDialog.elementIDs.roleAccessPermissionAllViewCheckbox).change(function () {
                if ($(this).is(':checked') == false) {
                    var targetId = $(this).attr('id').replace('viewcheckbox_', 'editcheckbox_');
                    $('#' + targetId).prop('checked', false).trigger('change');
                }
                setViewAllCheckbox();
            });

            if (ClientRolesForFormDesignAccess.indexOf(vbRole) >= 0 && currentInstance.uiElement.IsStandard == "true")
            {
                $('#p' + roleAccessPermissionDialog.elementIDs.roleAccessPermissionGrid).hide();//.addClass('divdisabled');
                $(this).find('input:checkbox').attr('disabled', 'disabled');
                $('#viewallcheckbox').attr('disabled', 'disabled');
                $('#editallcheckbox').attr('disabled', 'disabled');
            } else {
                $('#p' + roleAccessPermissionDialog.elementIDs.roleAccessPermissionGrid).show();
                $(this).find('input:checkbox').removeAttr('disabled');
                $('#viewallcheckbox').removeAttr('disabled');
                $('#editallcheckbox').removeAttr('disabled');
            }
        },
        onSelectRow: function (id, e) { }
    });

    //This GroupHeader is used for check-all View/Edit checkbox
    $(roleAccessPermissionDialog.elementIDs.roleAccessPermissionJQ).jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'RoleName', numberOfColumns: 1, titleText: 'Role' },
          { startColumnName: 'IsVisible', numberOfColumns: 1, titleText: 'View' },
          { startColumnName: 'IsEditable', numberOfColumns: 1, titleText: 'Edit' },
        ]
    });
    var pagerElement = '#p' + roleAccessPermissionDialog.elementIDs.roleAccessPermissionGrid;
    //remove default buttons
    $(roleAccessPermissionDialog.elementIDs.roleAccessPermissionJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    //remove paging
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    //if (roleAccessPermissionDialog._currentInstance.statustext != 'Finalized') {
    $(roleAccessPermissionDialog.elementIDs.roleAccessPermissionJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: 'Save',
            id:'btnRoleAccessPermissionSave',
            onClickButton: function () {
                if ($(roleAccessPermissionDialog.elementIDs.roleAccessPermissionAllEditCheckbox).filter(':checked').length >= 1) {
                    var roleAccessPermissionList = $(this).getRowData();
                    CreateSectionWiseAccessList(roleAccessPermissionList, UIElementID);
                }
                else {
                    messageDialog.show(DocumentDesign.roleAccessPermissionValidationMsg);
                }

            }
        });
    //}
    //To initialize all check-all checkboxes
    function initializeCheckAll() {
        $(".check-all").parent(".ui-jqgrid-sortable").removeClass("ui-jqgrid-sortable");
        setViewAllCheckbox();
        setEditAllCheckbox();
    }
    //For select all checkbox for View Permission 
    $('#viewallcheckbox').change(function () {
        var val = $('#viewallcheckbox').is(':checked');
        $(roleAccessPermissionDialog.elementIDs.roleAccessPermissionAllViewCheckbox).filter(':not(:disabled)').prop('checked', val).trigger('change');
    });
    function setViewAllCheckbox() {
        if ($(roleAccessPermissionDialog.elementIDs.roleAccessPermissionAllViewCheckbox).filter(':not(:checked):not(:disabled)').length > 0)
            $('#viewallcheckbox').prop('checked', false);
        else
            $('#viewallcheckbox').prop('checked', true);
    }

    //For select all checkbox for Edit Permission
    $('#editallcheckbox').change(function () {
        var val = $('#editallcheckbox').is(':checked');
        $(roleAccessPermissionDialog.elementIDs.roleAccessPermissionAllEditCheckbox).filter(':not(:disabled)').prop('checked', val).trigger('change');
    });

    function setEditAllCheckbox() {
        if ($(roleAccessPermissionDialog.elementIDs.roleAccessPermissionAllEditCheckbox).filter(':not(:checked):not(:disabled)').length > 0)
            $('#editallcheckbox').prop('checked', false);
        else
            $('#editallcheckbox').prop('checked', true);
    }
}
//InsertSectionWiseUserAccess function on save button click
function CreateSectionWiseAccessList(roleAccessPermissionList, UIElementID) {
    var rowListlen = roleAccessPermissionList.length;

    var accessRows = new Array(rowListlen);

    for (var i = 0; i < rowListlen; i++) {

        var roleListRow = new Object();
        roleListRow.RoleID = roleAccessPermissionList[i].RoleID;
        roleListRow.ResourceID = UIElementID;

        roleListRow.IsEditable = roleAccessPermissionList[i].IsEditable;
        roleListRow.IsVisible = roleAccessPermissionList[i].IsVisible;
        accessRows[i] = roleListRow

    }

    var accessRowsToAdd =
        {
            sectionaccessRows: accessRows

        };

    //save the updated user access list
    var url = roleAccessPermissionDialog.URLs.roleAccessPermissionSave;
    var promise = ajaxWrapper.postJSONCustom(url, accessRowsToAdd);
    promise.done(sectionRoleAccessSuccess);
    promise.fail(showError);
}

function showError(xhr) {
    if (xhr.status == 999)
        window.location.href = '/Account/LogOn';
    else {
        messageDialog.show(DocumentDesign.roleAccessPermissionMapErrorMsg);
    }

}
function sectionRoleAccessSuccess(xhr) {
    if (xhr == 0) {
        messageDialog.show(DocumentDesign.roleAccessPermissionMapSuccessMsg);
    }
    else {
        messageDialog.show(xhr);
    }
    //formDesignGroup.loadFormDesignGroupMapping();
}

//format the grid column based on element property
function checkBoxFormatterDesign(cellValue, options, rowObject) {

    if (options.colModel.index == "IsVisible") {
        if (rowObject.IsVisible == true) {
            if (rowObject.RoleName == "TMG Super User") {
                return "<input type='checkbox' title='TMG Super User should always have a view permission' id='viewcheckbox_" + options.rowId + "' name= ''viewcheckbox_" +

options.gid + "'  disabled checked/>";

            }
            else
                return "<input type='checkbox' id='viewcheckbox_" + options.rowId + "' name= 'viewcheckbox_" + options.gid + "' checked />";
        }

        else
            return "<input type='checkbox' id='viewcheckbox_" + options.rowId + "' name= 'viewcheckbox_" + options.gid + "' />";
    }
    //Disabled edit checkbox for Viewer
    if (rowObject.RoleName == "Viewer") {
        return "<input type='checkbox' title='Viewer does not have an edit permission' id='editcheckbox_" + options.rowId + "' name= 'checkbox_" + options.gid + "'  disabled />";
    }
    else {
        if (rowObject.IsEditable == true) {
            if (rowObject.RoleName == "TMG Super User") {
                return "<input type='checkbox' title='TMG Super User should always have an edit permission' id='editcheckbox_" + options.rowId + "' name= 'editcheckbox_" +

             options.gid + "'  disabled checked/>";

            }
            return "<input type='checkbox' id='editcheckbox_" + options.rowId + "' name= 'editcheckbox_" + options.gid + "' checked />";
        }

        else
            return "<input type='checkbox' id='editcheckbox_" + options.rowId + "' name= 'editcheckbox_" + options.gid + "' />";
    }
    //Disabled edit checkbox for Viewer
    if (rowObject.RoleName == "Viewer") {
        return "<input type='checkbox' title='Viewer does not have an edit permission' id='editcheckbox_" + options.rowId + "' name= 'checkbox_" + options.gid + "'  disabled />";
    }
}

//unformat the grid column based on element property
function unFormatIncludedColumnDesign(cellValue, options, rowObject) {
    var result;
    if (options.colModel.index == "IsVisible") {
        result = $(this).find('#viewcheckbox_' + options.rowId).prop('checked');
    }
    else if (rowObject.Name == "Viewer") {
        result = $(this).find('#viewcheckbox_' + options.rowId).prop('checked');
    }
    else {
        result = $(this).find('#editcheckbox_' + options.rowId).prop('checked');
    }
    return result;
}










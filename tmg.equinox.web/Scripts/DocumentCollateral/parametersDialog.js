/***** Parameters Dialog *****/
var parametersDialog = function (parameterElementID, reportVersionID) {
    this.parameter = {};
    this.parameterElementID = parameterElementID;
    this.reportVersionID = reportVersionID;
}

parametersDialog._currentInstance = undefined;

parametersDialog.URLs = {
    getParametersUrl: '/DocumentCollateral/GetParameters?tenantId={tenantId}&reportVersionID={reportVersionID}',
}
parametersDialog.elementIDs = {
    parametersDialog: '#parametersDialog',
    parametersGrid: '#parametersGrid',
}
parametersDialog._isInitialized = false;

parametersDialog.init = function () {
    if (parametersDialog._isInitialized == false) {
        $(parametersDialog.elementIDs.parametersDialog).dialog({
            autoOpen: false,
            width: 430,
            height: 350,
            modal: true
        });
        parametersDialog._isInitialized = true;
    }
}();

parametersDialog.prototype.show = function () {
    parametersDialog._currentInstance = this;
    $(parametersDialog.elementIDs.parametersDialog).dialog('option', 'title', "Select Parameters");
    $(parametersDialog.elementIDs.parametersDialog).dialog('open');


    this.loadParameters();
}

parametersDialog.prototype.loadParameters = function () {
    var currentInstance = this;
    var url = parametersDialog.URLs.getParametersUrl.replace(/\{tenantId\}/g, 1).replace(/\{reportVersionID\}/, this.reportVersionID);
    var promise = ajaxWrapper.getJSON(url);
    promise.done(function (xhr) {
        //generate grid data        
        currentInstance.loadParametersGrid(xhr);
    });
    //register callback for ajax request failure
    promise.fail(this.showError);
}

parametersDialog.prototype.getCustomRulesData = function () {
    return $(parametersDialog.elementIDs.parametersDialog + ' textarea').val();
}

parametersDialog.prototype.loadParametersGrid = function (parameters) {
    var colArray;
    var colModel = [];
    var currentInstance = this;
    var paramValues = parameters;
    colArray = ['ParameterID', 'Parameters', 'Action'];

    colModel.push({ name: 'ParameterId', sortable: false, key: true, index: 'ParameterId', hidden: true, align: 'left' });
    colModel.push({
        name: 'ParameterName', sortable: false, index: 'ParameterName', hidden: false, align: 'left'
        , resizable: true
    });
    //colModel.push({ name: 'Action', sortable: false, index: 'Action', align: 'center', formatter: checkBoxFormatterDesign, unformat: unFormatIncludedColumnDesign });
    colModel.push({ name: 'IsSelected', sortable: false, index: 'IsSelected', formatter: checkBoxFormatterDesign, unformat: unFormatIncludedColumnDesign, align: 'center' });

    $(parametersDialog.elementIDs.parametersGrid).jqGrid('GridUnload');

    $(parametersDialog.elementIDs.parametersGrid).parent().append("<div id='p" + 'parametersGird' + "'></div>");

    $(parametersDialog.elementIDs.parametersGrid).jqGrid({
        datatype: 'local',
        url: '',
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Parameters List',
        pager: '#p' + 'parametersGird',
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
            $(parametersDialog.elementIDs.parametersGrid).find("select").each(function (idx, control) {
                $(control).change(function () {
                    var element = $(this).attr("Id");
                    var id = element.split('_');
                    $(parametersDialog.elementIDs.parametersGrid).editRow(id[0], true);
                });
            });
        },
        onSelectRow: function (id, e) { }
    });
    //var newrow = {
    //    RowID: 0,
    //    IsVisible: 'Account'
    //};
    //$(parametersDialog.elementIDs.parametersGrid).jqGrid('addRowData', 0, newrow);

    // Adding Rows to Grid
    $.each(parameters, function (i, obj) {
        $(parametersDialog.elementIDs.parametersGrid).jqGrid('addRowData', obj.ParameterId, obj);
    });

    var pagerElement = '#p' + 'parametersGird';

    $(parametersDialog.elementIDs.parametersGrid).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();
    $(parametersDialog.elementIDs.parametersGrid).jqGrid('navButtonAdd', pagerElement,
            {
                caption: 'Save',
                onClickButton: function () {
                    var gfhfh = $(parametersDialog.elementIDs.parametersGrid).find('select').val();
                    var griddata = $(parametersDialog.elementIDs.parametersGrid).jqGrid('getRowData');
                    currentInstance.parameter = griddata;
                    $(currentInstance.parameterElementID).val(currentInstance.parameter);
                    $(parametersDialog.elementIDs.parametersDialog).dialog('close');
                    messageDialog.show('Parameter Saved Locally. Please submit form to persist changes.');
                }
            });
    function checkBoxFormatterDesign(cellValue, options, rowObject) {
        return "<input id='para_" + options.rowId + (rowObject.IsSelected ? '\' checked=\'checked\' ' : '\'') + " type='checkbox' />";
        //return "<select id=\'" + rowObject.RowID + "\'><option value=\"Account\">Account</option><option value=\"FolderVersion\">Folder Version</option><option value=\"Folder\">Folder</option><option value=\"Document\">Document</option></select>";
        //return str;
    }
    function unFormatIncludedColumnDesign(cellValue, options, rowObject) {
        return $('#para_' + options.rowId).prop('checked');
    }
}

/***** Role Access Permission Dialog *****/

var roleAccessPermissionDialog = function (UIElement, formDesignVersionId, templateReportVersiontID) {
    this.uiElement = UIElement;
    this.formDesignVersionId = formDesignVersionId;
    this.templateReportVersiontID = templateReportVersiontID;
}

roleAccessPermissionDialog.URLs = {
    userRoleList: '/DocumentCollateral/GetRoleNames?tenantId={tenantId}&reportVersionID={reportVersionID}',
    roleAccessPermissionSave: '/DocumentCollateral/AddViewPermissionSet'
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

    $(roleAccessPermissionDialog.elementIDs.roleAccessPermissionDialog).dialog('option', 'title', DocumentDesign.roleAccessPermissionDialog + " - ")
    $(roleAccessPermissionDialog.elementIDs.roleAccessPermissionDialog).dialog('open');

    this.loadroleAccessPermissionGrid(this.formDesignId, this.formDesignVersionId, this.templateReportVersiontID);
    $(roleAccessPermissionDialog.elementIDs.roleAccessPermissionDialog).dialog({
        position: {
            my: 'center',
            at: 'center'
        },
    });
}

roleAccessPermissionDialog.prototype.loadroleAccessPermissionGrid = function (formDesignId, formDesignVersionId, templateReportVersiontID) {
    //  var currentInstance = this;
    var getsectionAccessURL = roleAccessPermissionDialog.URLs.userRoleList.replace(/\{tenantId\}/g, 1).replace(/\{reportVersionID\}/g, templateReportVersiontID);
    var colArray;
    var colModel = [];
    var viewAllCheckbox = '<input type="checkbox" class="check-all" id="viewallcheckbox" style="margin-left: 45%"/>';
    var editAllCheckbox = '<input type="checkbox" class="check-all" id="editallcheckbox" />';
    //set column list for grid   
    colArray = ['', '', 'ResourceID', viewAllCheckbox];
    //set column models
    colModel.push({ name: 'RoleID', index: 'RoleID', editable: false, align: 'left', hidden: true });
    colModel.push({ name: 'RoleName', index: 'RoleName', editable: false, align: 'left', hidden: false });
    colModel.push({ name: 'ResourceID', index: 'SectionId', editable: false, align: 'left', hidden: true });
    colModel.push({ name: 'IsVisible', sortable: false, index: 'IsVisible', hidden: false, align: 'left', formatter: checkBoxFormatterDesign, unformat: unFormatIncludedColumnDesign, width: 50, resizable: true });
    //colModel.push({ name: 'IsEditable', sortable: false, index: 'IsEditable', hidden: false, align: 'center', formatter: checkBoxFormatterDesign, unformat: unFormatIncludedColumnDesign, width: 50, resizable: true });
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
        },
        onSelectRow: function (id, e) { }
    });

    //This GroupHeader is used for check-all View/Edit checkbox
    $(roleAccessPermissionDialog.elementIDs.roleAccessPermissionJQ).jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'RoleName', numberOfColumns: 1, titleText: 'Role' },
          { startColumnName: 'IsVisible', numberOfColumns: 1, titleText: 'View' },
          //{ startColumnName: 'IsEditable', numberOfColumns: 1, titleText: 'Edit' },
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
            onClickButton: function () {
                if ($(roleAccessPermissionDialog.elementIDs.roleAccessPermissionAllViewCheckbox).filter(':checked').length >= 1) {
                    var roleAccessPermissionList = $(this).getRowData();
                    CreateSectionWiseAccessList(roleAccessPermissionList, templateReportVersiontID);
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
function CreateSectionWiseAccessList(roleAccessPermissionList, templateReportVersiontID) {
    var currentInstance = this;
    var rowListlen = roleAccessPermissionList.length;

    var accessRows = new Array(rowListlen);

    for (var i = 0; i < rowListlen; i++) {

        var roleListRow = new Object();
        roleListRow.RoleID = roleAccessPermissionList[i].RoleID;
        roleListRow.TemplateReportVersionID = templateReportVersiontID;

        //roleListRow.IsEditable = roleAccessPermissionList[i].IsEditable;
        roleListRow.IsVisible = roleAccessPermissionList[i].IsVisible;
        accessRows[i] = roleListRow;
    }

    var accessRowsToAdd =
        {
            sectionaccessRows: accessRows
        };
    accessRowsToAdd = accessRowsToAdd.sectionaccessRows.filter(function (rows) {
        return rows.IsVisible == true;
    });
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
    if (xhr.Result === ServiceResult.SUCCESS) {
        if (xhr.Items.length > 0 && xhr.Items[0].Messages[0] != undefined && xhr.Items[0].Messages[0] != null) {
            messageDialog.show(xhr.Items[0].Messages[0]);
        }
        $(roleAccessPermissionDialog.elementIDs.roleAccessPermissionDialog).dialog('close');
        messageDialog.show(DocumentDesign.reportRoleAccessPermissionSuccessMsg);
    }
    else {
        messageDialog.show(xhr);
    }
}

//format the grid column based on element property
function checkBoxFormatterDesign(cellValue, options, rowObject) {
    if (options.colModel.index == "IsVisible" && rowObject.IsVisible == true) {
        if (rowObject.RoleName == "Super User") {
            return "<input type='checkbox' id='viewcheckbox_" + options.rowId + "' name= 'viewcheckbox_" + options.gid + "' checked  disabled=disabled/>";
        }
        return "<input type='checkbox' id='viewcheckbox_" + options.rowId + "' name= 'viewcheckbox_" + options.gid + "' checked />";
    }
    else {
        if (rowObject.RoleName == "Super User") {
            return "<input type='checkbox' id='viewcheckbox_" + options.rowId + "' name= 'viewcheckbox_" + options.gid + "' disabled=disabled/>";
        }
        return "<input type='checkbox' id='viewcheckbox_" + options.rowId + "' name= 'viewcheckbox_" + options.gid + "' />";
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












//constructor for duplication check dialog.
//params:
//uiElement - uiElement of row selected of the Document Design Version UI Elements Grid
//formDesignVersionId - form design version id of the element

var duplicationCheckDialog = function (uiElement, formDesignVersionId, status) {
    this.uiElement = uiElement;
    this.formDesignVersionId = formDesignVersionId;
    this.statustext = status;
}

duplicationCheckDialog.URLs = {
    fieldList: '/UIElement/GetRepeaterFieldList?tenantId=1&formDesignVersionId={formDesignVersionId}&parentUIElementId={uiElementId}',
    updateFields: '/UIElement/UpdateFieldCheckDuplicate'
}

duplicationCheckDialog._currentInstance = undefined;

//element ID's required for duplication check
duplicationCheckDialog.elementIDs = {
    //id for dialog
    duplicationCheckDialog: '#duplicationCheckDialog',
    duplicationCheckDialogGridJQ: '#duplicationCheckDialogGrid',
    duplicationCheckDialogGrid: 'duplicationCheckDialogGrid',
    duplicationCheckAllFieldCheckBox: 'input[name="checkboxIndividual"]',
    checkAllElements: "checkAllElements",
    checkAllElementsJQ: "#checkAllElements"
}
duplicationCheckDialog._isInitialized = false;

duplicationCheckDialog.init = function () {
    //register dialog
    if (duplicationCheckDialog._isInitialized == false) {
        $(duplicationCheckDialog.elementIDs.duplicationCheckDialog).dialog({
            autoOpen: false,
            width: 650,
            height: 450,
            modal: true
        });
        duplicationCheckDialog._isInitialized = true;
    }
}();

//this function open  the dialog for custom regex.
duplicationCheckDialog.prototype.show = function () {
    duplicationCheckDialog._currentInstance = this;
    $(duplicationCheckDialog.elementIDs.duplicationCheckDialog).dialog('option', 'title', DuplicationCheck.duplicationCheckDialogMsg + this.uiElement.Label)
    $(duplicationCheckDialog.elementIDs.duplicationCheckDialog).dialog('open');
    this.loadRepeaterFieldsGrid();
}
duplicationCheckDialog.prototype.loadRepeaterFieldsGrid = function () {
    var currentInstance = this;

    var colModel = [];
    if (currentInstance.statustext == 'Finalized') {
        var checkAllCheckbox = '<input type="checkbox" disabled="disabled" class="check-all" id="' + duplicationCheckDialog.elementIDs.checkAllElements + '"/>';
    }
    else {
        var checkAllCheckbox = '<input type="checkbox" class="check-all" id="' + duplicationCheckDialog.elementIDs.checkAllElements + '" />';
    }
    var colArray = ['', '', '', checkAllCheckbox];
    colModel.push({ name: 'UIElementID', index: 'UIElementID', key: true, hidden: true, search: false, });
    colModel.push({ name: 'Label', index: 'Label', align: 'left', editable: false, sortable: false });
    colModel.push({ name: 'ElementType', index: 'ElementType', align: 'left', editable: false, sortable: false });
    colModel.push({
        name: 'CheckDuplicate', index: 'CheckDuplicate', width: '50', align: 'center', sortable: false, formatter: checkBoxFormatterDesign, unformat: unFormatIncludedColumnDesign,
        editType: 'checkbox',
        editoptions: { value: 'Yes:No', defaultValue: 'No' },
    });

    //get URL for grid
    var fieldListUrl = duplicationCheckDialog.URLs.fieldList.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId).replace(/\{uiElementId\}/g, this.uiElement.UIElementID);
    //unload previous grid values
    $(duplicationCheckDialog.elementIDs.duplicationCheckDialogGridJQ).jqGrid('GridUnload');

    $(duplicationCheckDialog.elementIDs.duplicationCheckDialogGridJQ).parent().append("<div id='p" + duplicationCheckDialog.elementIDs.duplicationCheckDialogGrid + "'></div>");

    //load grid
    $(duplicationCheckDialog.elementIDs.duplicationCheckDialogGridJQ).jqGrid({
        url: fieldListUrl,
        datatype: 'json',
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Fields',
        pager: '#p' + duplicationCheckDialog.elementIDs.duplicationCheckDialogGrid,
        height: '250',
        rowNum: 10000,
        loadonce: false,
        autowidth: true,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        altclass: 'alternate',
        gridComplete: function () {
            initializeCheckAll();
        },
        loadComplete: function () {
            var data = $(duplicationCheckDialog.elementIDs.duplicationCheckDialogGridJQ).jqGrid('getRowData');
            for (var i = 0; i < data.length; i++) {
                $('#checkbox' + '_' + data[i].UIElementID).on('change', function () {
                    var elem = this.checked;
                    if (elem == false) {
                        $(duplicationCheckDialog.elementIDs.checkAllElementsJQ).prop('checked', false);
                    }
                    else {
                        var notCheckedCount = $(duplicationCheckDialog.elementIDs.duplicationCheckAllFieldCheckBox).filter(':not(:checked)').length;
                        if (notCheckedCount == 0) {
                            $(duplicationCheckDialog.elementIDs.checkAllElementsJQ).prop('checked', true);
                        }
                    }
                });
            }

            if (ClientRolesForFormDesignAccess.indexOf(vbRole) >= 0 && currentInstance.uiElement.IsStandard == "true") {
                $("#p" + duplicationCheckDialog.elementIDs.duplicationCheckDialogGrid).hide();//.addClass('divdisabled');
            }
        }
    });
    $(duplicationCheckDialog.elementIDs.duplicationCheckDialogGridJQ).jqGrid('sortableRows');

    var pagerElement = '#p' + duplicationCheckDialog.elementIDs.duplicationCheckDialogGrid;
    //remove default buttons
    $(duplicationCheckDialog.elementIDs.duplicationCheckDialogGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    //remove paging
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    //if (currentInstance.statustext != 'Finalized') {
    $(duplicationCheckDialog.elementIDs.duplicationCheckDialogGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: 'Save',
        onClickButton: function () {
            var updatedFields = [];
            var rows = $(this).getDataIDs();
            var count = 0;
            if (rows.length > 0) {
                for (index = 0; index < rows.length; index++) {
                    row = $(this).getRowData(rows[index]);
                    if (row.CheckDuplicate == true)
                        count++;
                    updatedFields.push({
                        FormDesignVersionID: currentInstance.formDesignVersionId,
                        UIElementID: row.UIElementID,
                        CheckDuplicate: row.CheckDuplicate,
                    });
                }
                if (currentInstance.uiElement.hasLoadFromServer == "true" && count == 0) {
                    messageDialog.show(DuplicationCheck.addAtleastOneFieldMsg);
                    return;
                }
                var data = {
                    TenantID: currentInstance.tenantId,
                    FormDesignVersionID: currentInstance.formDesignVersionId,
                    ParentUIElementID: currentInstance.uiElement.UIElementID,
                    Models: updatedFields
                };
                var promise = ajaxWrapper.postJSONCustom(duplicationCheckDialog.URLs.updateFields, { model: data });
                promise.done(function (xhr) {
                    currentInstance.loadRepeaterFieldsGrid();
                    messageDialog.show(DuplicationCheck.saveFieldMsg);
                });
                promise.fail(showError);
            }
            else
                messageDialog.show(DuplicationCheck.addAtleastOneFieldMsg);
        }
    });
    //}
    $(duplicationCheckDialog.elementIDs.duplicationCheckDialogGridJQ).jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'Label', numberOfColumns: 1, titleText: 'Field Name' },
          { startColumnName: 'ElementType', numberOfColumns: 1, titleText: 'Element Type' },
          { startColumnName: 'CheckDuplicate', numberOfColumns: 1, titleText: 'Check Duplicate' },
        ]
    });

    function showError(xhr) {
        if (xhr.status == 999)
            window.location.href = '/Account/LogOn';
        else {
            messageDialog.show(DuplicationCheck.duplicationCheckErrorMsg);
        }
    }
    //To initialize all check-all checkboxes
    function initializeCheckAll() {
        $(".check-all").parent(".ui-jqgrid-sortable").removeClass("ui-jqgrid-sortable");

        $(duplicationCheckDialog.elementIDs.checkAllElementsJQ).unbind("change");
        $(duplicationCheckDialog.elementIDs.checkAllElementsJQ).bind("change", function () {
            setAllCheckbox();
        });
        showAllCheckbox();
    }
    function setAllCheckbox() {
        $(duplicationCheckDialog.elementIDs.duplicationCheckAllFieldCheckBox).prop("checked", $(duplicationCheckDialog.elementIDs.checkAllElementsJQ).is(":checked"));
    }
    function showAllCheckbox() {
        if ($(duplicationCheckDialog.elementIDs.duplicationCheckAllFieldCheckBox).length > 0) {
            if ($(duplicationCheckDialog.elementIDs.duplicationCheckAllFieldCheckBox).filter(':not(:checked)').length > 0) {
                $(duplicationCheckDialog.elementIDs.checkAllElementsJQ).prop('checked', false);
            }
            else {
                $(duplicationCheckDialog.elementIDs.checkAllElementsJQ).prop('checked', true);
            }
        }
        else {
            $(duplicationCheckDialog.elementIDs.checkAllElementsJQ).prop('checked', false);
        }
    }
    //format the grid column based on applicable elements for check
    function checkBoxFormatterDesign(cellValue, options, rowObject) {
        var setCheckedValue;
        if (cellValue == true && (rowObject.ElementType == 'Dropdown List' || rowObject.ElementType == 'Dropdown TextBox' || rowObject.ElementType == 'Textbox' || rowObject.ElementType == 'Multiline TextBox' || rowObject.ElementType == 'Calendar'))
            setCheckedValue = 'checked';
        else if ((rowObject.ElementType == 'Blank' || rowObject.ElementType == 'Label' || rowObject.ElementType == 'Radio Button' || rowObject.ElementType == 'Checkbox'))
            setCheckedValue = 'disabled';
        else
            setCheckedValue = '';

        if (duplicationCheckDialog._currentInstance.statustext == 'Finalized') {
            return "<input type='checkbox' disabled='disabled' id='checkbox_" + options.rowId + "' name= 'checkboxIndividual' " + setCheckedValue + "/>";
        }
        else {
            return "<input type='checkbox' id='checkbox_" + options.rowId + "' name= 'checkboxIndividual' " + setCheckedValue + "/>";
        }
    }
    //unformat the grid column based on applicable elements for check
    function unFormatIncludedColumnDesign(cellValue, options, rowObject) {
        return $(this).find('#checkbox_' + options.rowId).prop('checked');
    }
}
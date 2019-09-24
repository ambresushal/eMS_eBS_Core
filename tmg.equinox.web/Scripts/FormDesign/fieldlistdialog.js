function fieldListDialog(uiElement, status, formDesignVersionId, formDesignVersionInstance, uiElementDetail) {
    this.uiElement = uiElement;
    this.formDesignVersionId = formDesignVersionId;
    this.tenantId = 1;
    this.statustext = status;
    this.formDesignVersionInstance = formDesignVersionInstance;
    this.uiElementDetail = uiElementDetail;
}

fieldListDialog.URLs = {
    fieldList: '/UIElement/FieldList?tenantId=1&formDesignVersionId={formDesignVersionId}&parentUIElementId={uiElementId}',
    updateFields: '/UIElement/UpdateFieldSequences'
}

fieldListDialog.elementIDs = {
    fieldListDialog: '#fieldlistdialog',
    fieldListDialogGridJQ: '#fieldlistdialoggrid',
    fieldListDialogGrid: 'fieldlistdialoggrid'
}

fieldListDialog._isInitialized = false;

//init dialog
fieldListDialog.init = function () {
    if (fieldListDialog._isInitialized == false) {
        $(fieldListDialog.elementIDs.fieldListDialog).dialog({
            autoOpen: false,
            height: '550',
            width: '70%',
            modal: true
        });
        fieldListDialog._isInitialized = true;
    }
}


//show dialog
fieldListDialog.prototype.show = function () {
    //set header
    $(fieldListDialog.elementIDs.fieldListDialog).dialog('option', 'title', 'Manage Fields - ' + this.uiElement.Label);
    //open dialog
    $(fieldListDialog.elementIDs.fieldListDialog).dialog('open');
    //load grid
    this.loadGrid();
    $(fieldListDialog.elementIDs.fieldListDialog).dialog({
        position: {
            my: 'center',
            at: 'center'
        },
    });
}

fieldListDialog.prototype.loadGrid = function () {
    var lastSel;
    var currentInstance = this;
    //set column list
    var colArray = ['', 'Field Name', 'Element Type', 'Old Sequence', 'New Sequence', 'Is Key', 'Is Key Mapped', 'IsElementMapped', 'IsStandard'];

    //set column models
    var colModel = [];
    colModel.push({ name: 'UIElementID', index: 'UIElementID', key: true, hidden: true, search: false, });
    colModel.push({ name: 'Label', index: 'Label', align: 'left', editable: false, sortable: false });
    colModel.push({ name: 'ElementType', index: 'ElementType', align: 'left', editable: false, sortable: false });
    colModel.push({ name: 'Sequence', index: 'Sequence', width: '50', align: 'right', hidden: true, editable: false, sortable: false });
    colModel.push({ name: 'NewSequence', index: 'NewSequence', width: '50', align: 'right', hidden: true, editable: true, sortable: false, formatter: this.formatNewSequence });
    colModel.push({
        name: 'IsKey', index: 'IsKey', width: '30', align: 'center', editable: true, edittype: 'checkbox',
        formatter: this.chkValueImageFmatter, editoptions: { value: 'true:false' },
        unformat: this.chkValueImageUnFormat
    });
    colModel.push({ name: 'IsRepeaterKeyMapped', index: 'IsRepeaterKeyMapped', hidden: true, search: false, });
    colModel.push({ name: 'IsElementMapped', index: 'IsElementMapped', hidden: true, search: false, });
    colModel.push({ name: 'IsStandard', index: 'IsStandard', hidden: true, search: false, });


    //get URL for grid
    var fieldListUrl = fieldListDialog.URLs.fieldList.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId).replace(/\{uiElementId\}/g, this.uiElement.UIElementID);
    //unload previous grid values
    $(fieldListDialog.elementIDs.fieldListDialogGridJQ).jqGrid('GridUnload');

    $(fieldListDialog.elementIDs.fieldListDialogGridJQ).parent().append("<div id='p" + fieldListDialog.elementIDs.fieldListDialogGrid + "'></div>");

    //load grid
    $(fieldListDialog.elementIDs.fieldListDialogGridJQ).jqGrid({
        url: fieldListUrl,
        datatype: 'json',
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Field List',
        pager: '#p' + fieldListDialog.elementIDs.fieldListDialogGrid,
        height: '350',
        rowNum: 10000,
        rowattr: function (rd) {
            if (rd.IsRepeaterKeyMapped) {
                //$(fieldListDialog.elementIDs.fieldListDialogGridJQ).jqGrid('setColProp', 'IsKey', { editable: false });
                return { "class": "ui-state-disabled ui-jqgrid-disablePointerEvents" };
            }
        },
        loadonce: true,
        autowidth: true,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        altclass: 'alternate',
        
        gridComplete: function () {
            if (currentInstance.uiElement.ElementType !== 'Repeater') {
                jQuery(fieldListDialog.elementIDs.fieldListDialogGridJQ).jqGrid('hideCol', ["IsKey"]);
            }
            $('#btn_' + fieldListDialog.elementIDs.fieldListDialogGrid + '_ChangeControl').addClass('ui-icon-hide');
        },
        onSelectRow: function (id) {

            var ctrlChng = ['Textbox', 'Label', 'Multiline TextBox', 'Rich TextBox',
                            'Dropdown List', 'Dropdown TextBox'];
            if (id && id !== lastSel) {
                $(fieldListDialog.elementIDs.fieldListDialogGridJQ).jqGrid('saveRow', lastSel);
                $(fieldListDialog.elementIDs.fieldListDialogGridJQ).jqGrid('editRow', id, true);//, null, null, 'clientArray');
                lastSel = id;
            }

            var row = $(this).getRowData(id);
            if (row) {
                if (ctrlChng.indexOf(row.ElementType) != -1) {
                    $('#btn_' + fieldListDialog.elementIDs.fieldListDialogGrid + '_ChangeControl').removeClass('ui-icon-hide');
                }
                else {
                    $('#btn_' + fieldListDialog.elementIDs.fieldListDialogGrid + '_ChangeControl').addClass('ui-icon-hide');
                }

                if (ClientRolesForFormDesignAccess.indexOf(vbRole) >= 0 && row.IsStandard == "true") {
                    $('#btn_' + fieldListDialog.elementIDs.fieldListDialogGrid + '_ChangeControl').addClass('ui-icon-hide');
                } else {
                    $('#btn_' + fieldListDialog.elementIDs.fieldListDialogGrid + '_ChangeControl').removeClass('ui-icon-hide');
                }
            }
        }
    });
    $(fieldListDialog.elementIDs.fieldListDialogGridJQ).jqGrid('sortableRows');
    $(fieldListDialog.elementIDs.fieldListDialogGridJQ).on("sortstop", function (event, ui) {
        var rows = $(fieldListDialog.elementIDs.fieldListDialogGridJQ).getDataIDs();
        for (index = 0; index < rows.length; index++) {
            row = $(fieldListDialog.elementIDs.fieldListDialogGridJQ).getRowData(rows[index]);
            row.NewSequence = index + 1;
            $(fieldListDialog.elementIDs.fieldListDialogGridJQ).setCell(rows[index], 4, index + 1);
        }
    });

    var pagerElement = '#p' + fieldListDialog.elementIDs.fieldListDialogGrid;
    //remove default buttons
    $(fieldListDialog.elementIDs.fieldListDialogGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    //remove paging
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    //NOTE:!Important Following condition to for adding element in finalized version is enabled temporarily 
    //if (this.statustext != 'Finalized') {
    $(fieldListDialog.elementIDs.fieldListDialogGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-plus',
        onClickButton: function () {
            addFieldDialog.show('', 'add', currentInstance.tenantId, currentInstance.formDesignVersionId, currentInstance.uiElement.UIElementID, currentInstance);
        }
    });
    $(fieldListDialog.elementIDs.fieldListDialogGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-shuffle',
        title: 'Change Control', id: 'btn_' + fieldListDialog.elementIDs.fieldListDialogGrid + '_ChangeControl',
        onClickButton: function () {

            var rowId = $(this).getGridParam('selrow');
            if (rowId !== undefined && rowId !== null) {
                var row = $(this).getRowData(rowId);
                ChangeControlDialog.show(row.Label, row.ElementType, row.UIElementID, row.IsElementMapped, currentInstance.formDesignVersionId, currentInstance);
            }
        }
    });
    var currentInstance = this;
    $(fieldListDialog.elementIDs.fieldListDialogGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: 'Save',
        onClickButton: function () {
            $(fieldListDialog.elementIDs.fieldListDialogGridJQ).jqGrid('saveRow', lastSel)
            var hasKey = false;
            var updatedSequences = [];
            var rows = $(this).getDataIDs();
            for (index = 0; index < rows.length; index++) {
                row = $(this).getRowData(rows[index]);
                if (row.IsKey === "true") {
                    hasKey = true;
                }
                updatedSequences.push({
                    TenantID: currentInstance.tenantId,
                    FormDesignVersionID: currentInstance.formDesignVersionId,
                    UIElementID: row.UIElementID,
                    IsKey: row.IsKey,
                    Sequence: row.NewSequence
                });
            }
            if (!hasKey && currentInstance.uiElement.ElementType === 'Repeater') {
                messageDialog.show(DocumentDesign.repeaterKeyValidationMessage);
                //if (updatedSequences.length > 0) {
                //    updatedSequences[0].IsKey = true;
                //}
            }
            else {
                var data = {
                    TenantID: currentInstance.tenantId,
                    FormDesignVersionID: currentInstance.formDesignVersionId,
                    ParentUIElementID: currentInstance.uiElement.UIElementID,
                    Models: updatedSequences
                };
                var promise = ajaxWrapper.postJSONCustom(fieldListDialog.URLs.updateFields, { model: data });
                promise.done(function (xhr) {
                    //$(fieldListDialog.elementIDs.fieldListDialogGridJQ).trigger('reloadGrid');
                    currentInstance.loadGrid();
                    currentInstance.formDesignVersionInstance.buildGrid(currentInstance.uiElement.UIElementID);
                    messageDialog.show(DocumentDesign.saveFieldMsg);
                });
            }
        }
    });
    //}
}

fieldListDialog.prototype.chkValueImageFmatter = function (cellvalue, options, rowObject) {

    if (cellvalue == true || cellvalue == "true") {

        if (rowObject.IsRepeaterKeyMapped == true || rowObject.IsRepeaterKeyMapped == "true") {
            return '<span align="center" class="ui-icon ui-icon-check" style="margin:auto" ></span>';
        }
        else {
            return '<span align="center" class="ui-icon ui-icon-check" style="margin:auto" ></span>';
        }

    }
    else {
        return '<span align="center" class="ui-icon ui-icon-close" style="margin:auto" ></span>';
    }
}

fieldListDialog.prototype.chkValueImageUnFormat = function (cellvalue, options, cell) {

    var checked = $(cell).children('span').attr('class');

    if (checked == "ui-icon ui-icon-check")
        return 'true';
    else
        return 'false';
}


fieldListDialog.prototype.formatNewSequence = function (cellValue, options, rowObject) {
    if (cellValue === undefined) {
        return rowObject.Sequence;
    }
    else {
        return cellValue;
    }
}


fieldListDialog.init();

var addFieldDialog = function () {
    var URLs = {
        fieldAdd: '/UIElement/AddElement',
        fieldUpdate: '/UIElement/UpdateFieldName'
    }

    var elementIDs = {
        fieldListDialog: '#fieldlistdialogadd',
        fieldListDialogGridJQ: '#fieldlistdialoggrid',
        fieldListDialogGrid: 'fieldlistdialoggrid',
        fieldControlList: '#fieldlist'
    };

    var fieldDialogState;
    var formDesignVersionId;
    var tenantId;
    var parentUIElementId;
    var parentObject;

    function fieldSuccess(xhr) {
        //$(elementIDs.fieldListDialogGridJQ).trigger('reloadGrid');
        if (parentObject != undefined)
            parentObject.loadGrid();
        $(elementIDs.fieldListDialog + ' #fieldname').parent().removeClass('has-error');
        $(elementIDs.fieldListDialog + ' .help-block').text(Common.fieldUniqueNameMsg);
        $(elementIDs.fieldListDialog).dialog('close');
    }

    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else

            messageDialog.show(JSON.stringify(xhr));
    }

    function init() {
        //register dialog for grid row add/edit
        $(elementIDs.fieldListDialog).dialog({
            autoOpen: false,
            width: 450,
            modal: true
        });
        $(elementIDs.fieldListDialog + ' button').on('click', function () {
            //check if name is already used
            var elementType = $(elementIDs.fieldListDialog + ' select').val();
            var newName = $(elementIDs.fieldListDialog + ' textarea').val().replace(/\s+/g, " ");
            var startWithAlphabet = new RegExp(CustomRegexValidation.STARTWITHAPLHABETS);
            var fieldList = $(elementIDs.fieldListDialogGridJQ).getRowData();
            var filterList;
            if (elementType != '[Blank]') {
                filterList = fieldList.filter(function (ct) {
                    return compareStrings(ct.Label.replace(/\s/g, ''), newName, true);
                });
            }
            else {
                newName = 'Blank';
            }

            //validate form name
            if (filterList !== undefined && filterList.length > 0) {
                $(elementIDs.fieldListDialog + ' #fieldname').parent().addClass('has-error');
                $(elementIDs.fieldListDialog + ' .help-block').text(Common.fieldNameExistsMsg);
            }
            else if (newName == '' && elementType != '[Blank]') {
                $(elementIDs.fieldListDialog + ' #fieldname').parent().addClass('has-error');
                $(elementIDs.fieldListDialog + ' .help-block').text(Common.fieldNameRequiredMsg);
            }
            else if (!(newName.match(startWithAlphabet))) {
                $(elementIDs.fieldListDialog + ' #fieldname').parent().addClass('has-error');
                $(elementIDs.fieldListDialog + ' .help-block').text(DocumentDesign.nameValidateMsg);
            }
            else {
                //check for duplicate names within FormDesign version

                if (parentObject.uiElement.Label.replace(/\s/g, "") === newName.replace(/\s/g, "")) {
                    $(elementIDs.fieldListDialog + ' #fieldname').parent().addClass('has-error');
                    $(elementIDs.fieldListDialog + ' .help-block').text(Common.fieldNameExistsMsg);
                } else {
                    //save the new form design
                    var rowId = $(elementIDs.fieldListDialogGridJQ).getGridParam('selrow');
                    var fieldAdd = {
                        TenantID: tenantId,
                        FormDesignVersionID: formDesignVersionId,
                        ParentUIElementID: parentUIElementId,
                        UIElementID: rowId,
                        ElementType: elementType,
                        Label: newName
                    };
                    var url;
                    if (fieldDialogState === 'add') {
                        url = URLs.fieldAdd;
                    } else {
                        url = URLs.fieldUpdate;
                    }
                    var promise = ajaxWrapper.postJSONCustom(url, fieldAdd);
                    promise.done(fieldSuccess);
                    promise.fail(showError);
                }
            }
        });
    }
    init();

    return {
        show: function (fieldName, action, tenantID, formDesignVersionID, parentUIElementID, parent) {
            tenantId = tenantID;
            formDesignVersionId = formDesignVersionID;
            parentUIElementId = parentUIElementID;
            fieldDialogState = action;
            parentObject = parent;
            $(elementIDs.fieldListDialog + ' textarea').each(function () {
                $(this).val(fieldName);

            });

            //This code will disable the dropdown elements which are not required for Repeater.
            $(elementIDs.fieldListDialog + ' #fieldname').parent().removeClass('has-error');
            if (fieldDialogState == 'add') {
                $(elementIDs.fieldListDialog).dialog('option', 'title', 'Add Field');
                $(elementIDs.fieldListDialog + ' .help-block').text(Common.fieldUniqueNameMsg);
                $(elementIDs.fieldListDialog + ' button').text('Add');
                $(elementIDs.fieldControlList + ' option[value="TextBox"]').prop('selected', true);


                if (parent.uiElement.ElementType == "Repeater") {
                    $(elementIDs.fieldControlList + ' option[value="Repeater"]').attr('disabled', 'disabled');
                    $(elementIDs.fieldControlList + ' option[value="Section"]').attr('disabled', 'disabled');
                    $(elementIDs.fieldControlList + ' option[value="Radio Button"]').attr('disabled', 'disabled');
                    $(elementIDs.fieldControlList + ' option[value="[Blank]"]').attr('disabled', 'disabled');
                    //$(elementIDs.fieldControlList + ' option[value="Rich TextBox"]').attr('disabled', 'disabled');
                } else {
                    //If 'Layout Type' is selected as 'Custom Layout' then not allowed users to add Repeater or sub section inside section.
                    if (parseInt(parent.uiElementDetail.LayoutTypeID) == LayoutType.CUSTOMLAYOUT) {
                        if (parent.uiElement.ElementType == "Section") {
                            $(elementIDs.fieldControlList + ' option[value="Repeater"]').attr('disabled', 'disabled');
                            $(elementIDs.fieldControlList + ' option[value="Section"]').attr('disabled', 'disabled');
                            $(elementIDs.fieldControlList + ' option[value="[Blank]"]').removeAttr('disabled');
                            $(elementIDs.fieldControlList + ' option[value="Radio Button"]').removeAttr('disabled', 'disabled');
                        }
                    }
                    else {
                        $(elementIDs.fieldControlList + ' option[value="[Blank]"]').removeAttr('disabled');
                        $(elementIDs.fieldControlList + ' option[value="Section"]').removeAttr('disabled');
                        $(elementIDs.fieldControlList + ' option[value="Radio Button"]').removeAttr('disabled', 'disabled');
                        $(elementIDs.fieldControlList + ' option[value="Repeater"]').removeAttr('disabled');
                    }
                }

            }
            else {
                $(elementIDs.fieldListDialog).dialog('option', 'title', 'Edit Field');
                $(elementIDs.fieldListDialog + ' .help-block').text(Common.fieldEditNameValidateMsg);
                $(elementIDs.fieldListDialog + ' button').text('Edit');
            }
            $(elementIDs.fieldListDialog).dialog("open");
        }
    }
}();

var ChangeControlDialog = function () {
    var URLs = {
        changeControlType: '/UIElement/ChangeControlType?tenantId=1&formDesignVersionId={formDesignVersionId}&newElementType={newElementType}&uiElementId={uiElementId}&newuiElementTypeId={newuiElementTypeId}',
    }

    var elementIDs = {
        ChangeControlDialog: "#changeControldialog",
        ChangeControlDDL: '#changeControlToDDL',
        ElementName: '#elementname',
        CurrentControl: '#currentcontrol'
    };

    var isCurrentControlMapped;
    var formDVId;
    var getUIelementId;
    var parentObject;

    //ajax success callback
    function controlChangeSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            if (parentObject != undefined)
                parentObject.loadGrid();
            messageDialog.show(DocumentDesign.controlChangeSuccessMsg);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
        $(elementIDs.ChangeControlDialog).dialog('close');
    }

    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    function init() {
        $(elementIDs.ChangeControlDialog).dialog({
            autoOpen: false,
            height: 200,
            width: 350,
            modal: true
        });
        $(elementIDs.ChangeControlDialog + ' button').on('click', function () {
            var changetype = $(elementIDs.ChangeControlDDL).val();
            if (changetype == 0) {
                $(elementIDs.ChangeControlDialog + ' .help-block').text(DocumentDesign.dropDowncontrolChangeReqMsg);
                $(elementIDs.ChangeControlDialog + ' #changeControlToDDL').parent().addClass('has-error');
            }
            else {

                if (isCurrentControlMapped == 'true' && isCurrentControlMapped === 'true') {
                    yesNoConfirmDialog.show(DocumentDesign.deleteMappingOnChangeControl, function (e) {
                        yesNoConfirmDialog.hide();
                        if (e) {
                            changeControlType();
                        }
                        else {
                            $(elementIDs.ChangeControlDialog).dialog('close');
                        }
                    })
                }
                else {
                    changeControlType();
                }

            }

        });
    }

    $(elementIDs.ChangeControlDialog).on('change', function () {
        $(elementIDs.ChangeControlDialog + ' .help-block').text('');
        $(elementIDs.ChangeControlDialog + ' #changeControlToDDL').parent().removeClass('has-error');
    });

    init();

    function fillChangeControlDDL(ElementType) {
        $(elementIDs.ChangeControlDDL).empty();
        $(elementIDs.ChangeControlDDL).append("<option value='0'>" + "--Select--" + "</option>");
        if (ElementType == "Textbox") {
            $(elementIDs.ChangeControlDDL).append("<option value=4>Multiline TextBox</option>");
            $(elementIDs.ChangeControlDDL).append("<option value=13>Rich TextBox</option>");
            $(elementIDs.ChangeControlDDL).append("<option value=10>Label</option>");
        }
        else if (ElementType == "Label") {
            $(elementIDs.ChangeControlDDL).append("<option value=4>Multiline TextBox</option>");
            $(elementIDs.ChangeControlDDL).append("<option value=13>Rich TextBox</option>");
            $(elementIDs.ChangeControlDDL).append("<option value=3>Textbox</option>");
        }
        else if (ElementType == "Multiline TextBox") {
            $(elementIDs.ChangeControlDDL).append("<option value=13>Rich TextBox</option>");
        }
        else if (ElementType == "Rich TextBox") {
            $(elementIDs.ChangeControlDDL).append("<option value=4>Multiline TextBox</option>");
        }
        else if (ElementType == "Dropdown List") {
            $(elementIDs.ChangeControlDDL).append("<option value=12>Dropdown TextBox</option>");
        }
        else if (ElementType == "Dropdown TextBox") {
            $(elementIDs.ChangeControlDDL).append("<option value=5>Dropdown List</option>");
        }

    }

    function changeControlType() {
        url = URLs.changeControlType.replace(/\{formDesignVersionId\}/g, formDVId).replace(/\{uiElementId\}/g, getUIelementId);
        url = url.replace(/\{newElementType\}/g, $(elementIDs.ChangeControlDDL).find("option:selected").text());
        url = url.replace(/\{newuiElementTypeId\}/g, $(elementIDs.ChangeControlDDL).val());

        var promise = ajaxWrapper.postJSON(url);
        //register ajax success callback
        promise.done(function (xhr) {
            controlChangeSuccess(xhr);
        });
        //register ajax failure callback
        promise.fail(showError);
        $(elementIDs.ChangeControlDialog).dialog('close');
    }
    return {
        show: function (elementName, elementType, elementId, isMapped, fdvId, parent) {
            $(elementIDs.ElementName).val(elementName);
            $(elementIDs.CurrentControl).val(elementType);
            isCurrentControlMapped = isMapped;
            formDVId = fdvId;
            parentObject = parent;
            getUIelementId = elementId;
            $(elementIDs.ChangeControlDialog + ' div').removeClass('has-error');
            $(elementIDs.ChangeControlDialog + ' .help-block').text("");

            fillChangeControlDDL(elementType);

            $(elementIDs.ChangeControlDialog).dialog('option', 'title', 'Manage Change Control Type');

            $(elementIDs.ChangeControlDialog).dialog("open");
        }
    }
}();
function csSetupCompare(currentIndex, newIndex, documentSyncData) {
    this.isNewMacro = false;
    this.elementIDs = {
        selectMacroButtonJQ: "#cmdSelectMacro",
        addMacroButtonJQ: "#cmdAddMacro",
        elementGrid: 'elementList',
        elementGridJS: '#elementList',
        elementGridPagerJQ: '#pelementList',
        repeaterKeyGrid: 'repeaterKeyList',
        repeaterKeyGridJS: '#repeaterKeyList',
        childElementGrid: 'childElementList',
        childElementGridJS: '#childElementList',
        selectAllFieldsJQ: "#selectAllFields",
        divSelectedMacroJQ: "#dvSelectedMacro",
        tdMacroNameJQ: "#tdMacroName",
        tdMacroNotesJQ: "#tdMacroNotes",
        btnEditMacroJQ: "#existingMacro",
        btnViewMacroJQ: "#viewExistingMacro"
    };
    this.URLs = {
        elementList: '/DocumentSync/DocumentElements?formDesignVersionID={formDesignVersionID}&&macroID={macroID}',
        repeaterKeyList: '/DocumentSync/RepeaterKeys',
        childElementList: '/DocumentSync/ElementChildren',
        saveMacroTemplate: '/DocumentSync/UpdateSyncMacro',
        validateMacroTemplate: '/DocumentSync/HasMacroConfigured'
    };

    this.documentSyncData = documentSyncData;
};

csSetupCompare.prototype.process = function () {
    var that = this;
    //cleanup if required or second visit onwards
    console.log("Process Setup Compare");

    var currentInstance = this;
    $(this.elementIDs.selectMacroButtonJQ).off("click");
    $(this.elementIDs.selectMacroButtonJQ).click(function () {
        var res = selectMacroeDailog.show(currentInstance);
    });

    $(this.elementIDs.btnEditMacroJQ).off("click");
    $(this.elementIDs.btnEditMacroJQ).click(function () {
        that.loadElementGrid(true);
    });

    $(this.elementIDs.btnViewMacroJQ).off("click");
    $(this.elementIDs.btnViewMacroJQ).click(function () {
        that.loadElementGrid(false);
    });
}

csSetupCompare.prototype.setSelectedMacro = function (isEditable) {
    $(this.elementIDs.tdMacroNameJQ).text(this.documentSyncData.SelectedMacro.MacroName);
    $(this.elementIDs.tdMacroNotesJQ).text(this.documentSyncData.SelectedMacro.Notes);
    $(this.elementIDs.divSelectedMacroJQ).css({ display: "block" });
    $(this.elementIDs.elementGridJS).jqGrid('GridUnload');
    $(this.elementIDs.childElementGridJS).jqGrid('GridUnload');
    $(this.elementIDs.repeaterKeyGridJS).jqGrid('GridUnload');
    if (this.isNewMacro) this.loadElementGrid(isEditable);

    if (isEditable) {
        isMacroEditiable = true;
        $(this.elementIDs.btnViewMacroJQ).css('display', 'none');
        $(this.elementIDs.btnEditMacroJQ).css('display', 'block');
    }
    else {
        isMacroEditiable = false;
        $(this.elementIDs.btnViewMacroJQ).css('display', 'block');
        $(this.elementIDs.btnEditMacroJQ).css('display', 'none');
    }
}

csSetupCompare.prototype.updateMacro = function () {

    var that = this;
    var template = JSON.stringify(that.documentSyncData.SelectedMacro.Template)
    //Update template and save to database
    if (template != that.documentSyncData.SelectedMacro.MacroJSON) {
        var objData = {
            macroID: that.documentSyncData.SelectedMacro.MacroID,
            macroJSON: template
        };
        var promise = ajaxWrapper.postJSON(that.URLs.saveMacroTemplate, objData);

        promise.done(function (xhr) {
            console.log("template saved...");
        });

        promise.fail(that.showError);
    }
}

csSetupCompare.prototype.vallidateMacro = function (obj) {
    var hasFields = false;
    var pattern = /"(Field)":"((\\"|[^"])*)"/i;
    if (obj.match(pattern) != null) {
        hasFields = true;
    }
    return hasFields;
}

csSetupCompare.prototype.chkFormatter = function (cellvalue, options, rowObject) {
    if (rowObject.ElementType != "Tab") {
        var checked = cellvalue == true ? "checked='checked' " : "";
        return "<input type='checkbox' " + checked + " value='" + cellvalue + "'/>";
    }
    else {
        return "";
    }
}

csSetupCompare.prototype.chkUnFormat = function (cellvalue, options, cell) {

    var checked = $(cell).is(':checked');
    return checked;
}

csSetupCompare.prototype.loadElementGrid = function (isEditable) {
    try {
        var that = this;
        var colArray = ['UIElementID', 'Name', '', 'Type', ''];
        var colModel = [];
        colModel.push({ name: 'UIElementID', index: 'UIElementID', hidden: true, key: true });
        colModel.push({ name: 'Label', index: 'Label', width: 350, editable: false });
        colModel.push({ name: 'UIElementPath', index: 'UIElementPath', hidden: true });
        colModel.push({ name: 'ElementType', index: 'ElementType', align: "left", editable: false });
        colModel.push({ name: 'isChecked', index: 'isChecked', align: 'center', editable: isEditable, formatter: 'checkbox', editoptions: { value: 'true:false' }, formatoptions: { disabled: !isEditable } });

        $(that.elementIDs.elementGridJS).jqGrid('GridUnload');
        $(that.elementIDs.elementGridJS).parent().append("<div id='p" + that.elementIDs.elementGrid + "'></div>");

        var objData = {
            formDesignVersionID: that.documentSyncData.SelectedMacro.FormDesignVersionID,
            macroID: that.documentSyncData.SelectedMacro.MacroID,
            template: JSON.stringify(that.documentSyncData.SelectedMacro.Template)
        };

        var url = that.URLs.elementList;

        $(that.elementIDs.elementGridJS).jqGrid({
            url: url,
            mtype: 'POST',
            postData: objData,
            datatype: 'json',
            treeGrid: true,
            treeGridModel: 'adjacency',
            colNames: colArray,
            colModel: colModel,
            autowidth: false,
            shrinkToFit: false,
            forceFit: true,
            rowNum: 10000,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            altclass: 'alternate',
            height: '400',
            pager: that.elementIDs.elementGridPagerJQ,
            ExpandColumn: 'Label',
            caption: "Setup Compare",
            onSelectRow: function (id) {
                var row = $(that.elementIDs.elementGridJS).getRowData(id);
                if (row) {
                    if (row.ElementType != 'Tab') {
                        that.getChildElements(row.UIElementID, row.UIElementPath, row.isChecked, isEditable);
                        if (row.ElementType == 'Repeater') {
                            that.loadRepeaterKeyGrid(row.UIElementID, row.UIElementPath);
                        }
                    }
                    else {
                        if ($(that.elementIDs.childElementGridJS)[0].grid) {
                            $(that.elementIDs.childElementGridJS).jqGrid('GridUnload');
                        }
                        if ($(that.elementIDs.repeaterKeyGridJS)[0].grid) {
                            $(that.elementIDs.repeaterKeyGridJS).jqGrid('GridUnload');
                        }
                    }
                }
            },
            loadComplete: function () {
                var iCol = 4, rows = this.rows, i, c = rows.length;
                for (i = 0; i < c; i += 1) {
                    $(rows[i].cells[iCol]).change(function (e) {
                        var id = $(e.target).closest('tr')[0].id, isChecked = $(e.target).is(':checked');
                        $(that.elementIDs.elementGridJS).jqGrid('setSelection', id);
                    });
                }
            }
        });

        var pagerElement = that.elementIDs.elementGridPagerJQ;
        //remove default buttons
        $(that.elementIDs.elementGridJS).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    } catch (e) {

    }
}

csSetupCompare.prototype.loadRepeaterKeyGrid = function (elementID, uiElementPath) {
    var that = this;
    var colArray = ['UIElementID', 'Name', ''];
    var colModel = [];
    colModel.push({ name: 'UIElementID', index: 'UIElementID', width: 100, hidden: true, key: true });
    colModel.push({ name: 'Label', index: 'Label', width: 290 });
    colModel.push({ name: 'UIElementPath', index: 'UIElementPath', hidden: true });

    $(that.elementIDs.repeaterKeyGridJS).jqGrid('GridUnload');
    var url = that.URLs.repeaterKeyList;

    var objData = {
        formDesignVersionID: that.documentSyncData.SelectedMacro.FormDesignVersionID,
        elementID: elementID,
        macroID: that.documentSyncData.SelectedMacro.MacroID,
        elementPath: uiElementPath,
        template: JSON.stringify(that.documentSyncData.SelectedMacro.Template)
    };

    $(that.elementIDs.repeaterKeyGridJS).jqGrid({
        url: url,
        mtype: 'POST',
        postData: objData,
        datatype: 'json',
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Keys',
        rowNum: 20,
        rowList: [10, 20, 30],
        ignoreCase: true,
        autowidth: true,
        shrinkToFit: false,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        sortname: 'UIElementID',
        altclass: 'alternate',
        gridComplete: function () {
            //To set first row of grid as selected.
            console.log("grid complete");
        }
    });
}

csSetupCompare.prototype.setElementSelection = function (row, isChecked) {
    var that = this; var path = row.UIElementPath.split('.'); var fields = null;
    for (var i = 0; i < path.length - 1; i++) {
        if (fields == null) { fields = that.documentSyncData.SelectedMacro.Template; }
        fields = fields[path[i]];
    }

    if (isChecked == true || isChecked == "true") {
        var data = { Field: path[path.length - 1], Label: row.Label };
        fields.Fields.push(data);
    }
    else {
        var index;
        $.each(fields.Fields, function (key, value) {
            if (value.Field === path[path.length - 1]) {
                index = key;
                return false;
            }
        });
        if (index != null && index != undefined) {
            fields.Fields.splice(index, 1);
        }
    }
}

csSetupCompare.prototype.getChildElements = function (elementID, uiElementPath, isChecked, isEditable) {
    var that = this;
    var url = that.URLs.childElementList;
    var objData = {
        formDesignVersionID: that.documentSyncData.SelectedMacro.FormDesignVersionID,
        elementID: elementID,
        macroID: that.documentSyncData.SelectedMacro.MacroID,
        elementPath: uiElementPath,
        template: JSON.stringify(that.documentSyncData.SelectedMacro.Template)
    };

    var elementMacro = undefined;
    if (uiElementPath.indexOf('.') != -1) {
        var path = uiElementPath.split('.');
        $.each(path, function (idx, dt) {
            if (idx == 0) {
                elementMacro = that.documentSyncData.SelectedMacro.Template[dt];
            }
            else {
                elementMacro = elementMacro[dt];
            }
        })

        elementMacro.IsMacroSelected = isChecked;
    }    

    var promise = ajaxWrapper.postJSON(url, objData);

    promise.done(function (xhr) {
        if (xhr.length > 0) {
            if (isEditable) {
                var isChildSelected = that.anyChildSelected(xhr);
                if (isChildSelected && (isChecked == false || isChecked == "false")) {
                    that.toggleChildSelection(xhr, isChecked);
                }
                else if (!isChildSelected && (isChecked == true || isChecked == "true")) {
                    that.toggleChildSelection(xhr, isChecked);
                }
            }
            that.loadChildElementsGrid(xhr, isEditable);
        }
        else {            
            if ($(that.elementIDs.childElementGridJS)[0].grid) {
                $(that.elementIDs.childElementGridJS).jqGrid('GridUnload');
            }
            if (that.elementIDs.repeaterKeyGridJS[0].grid) {
                $(that.elementIDs.repeaterKeyGridJS).jqGrid('GridUnload');
            }
        }
    });

    promise.fail(that.showError);
}

csSetupCompare.prototype.anyChildSelected = function (data) {
    var isSelected = false;
    $.each(data, function (ind, record) {
        if (record.isChecked == true || record.isChecked == "true") {
            isSelected = true;
            return false;
        }
    });

    return isSelected;
}

csSetupCompare.prototype.toggleChildSelection = function (data, isChecked) {
    var that = this;
    $.each(data, function (ind, record) {
        record.isChecked = isChecked;
        that.setElementSelection(record, isChecked);
    });
}

csSetupCompare.prototype.loadChildElementsGrid = function (elements, isEditable) {
    var that = this;
    var colArray = ['UIElementID', 'Name', '', isEditable == false ? "" : '<input type="checkbox" id="selectAllFields"/>'];
    var colModel = [];
    colModel.push({ name: 'UIElementID', index: 'UIElementID', width: 100, hidden: true, key: true });
    colModel.push({ name: 'Label', index: 'Label', width: 250 });
    colModel.push({ name: 'UIElementPath', index: 'UIElementPath', hidden: true });
    colModel.push({ name: 'isChecked', index: 'isChecked', width: 40, align: 'left', formatter: 'checkbox', editoptions: { value: 'true:false' }, formatoptions: { disabled: !isEditable } });

    $(that.elementIDs.childElementGridJS).jqGrid('GridUnload');
    $(that.elementIDs.childElementGridJS).jqGrid({
        data: elements,
        datatype: 'local',
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Field',
        rowNum: 20,
        rowList: [10, 20, 30],
        ignoreCase: true,
        autowidth: true,
        shrinkToFit: false,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        altclass: 'alternate',
        loadComplete: function () {
            var iCol = 3, rows = this.rows, i, c = rows.length;
            //Set SelectAll checkbox value
            var data = $(that.elementIDs.childElementGridJS).jqGrid('getRowData');
            var isAllChildSelected = true;
            $.each(data, function (i, j) {
                if (j.isChecked == false || j.isChecked == "false") { isAllChildSelected = false; return false; }
            });
            $(that.elementIDs.selectAllFieldsJQ).prop('checked', isAllChildSelected);

            //Register checkbox events
            for (i = 0; i < c; i += 1) {
                $(rows[i].cells[iCol]).click(function (e) {
                    var id = $(e.target).closest('tr')[0].id, isChecked = $(e.target).is(':checked');
                    var row = $(that.elementIDs.childElementGridJS).getRowData(id);
                    that.setElementSelection(row, isChecked);
                });
            }
        }
    });

    $(that.elementIDs.selectAllFieldsJQ).click(function (e) {
        e = e || event; e.stopPropagation ? e.stopPropagation() : e.cancelBubble = false;
        var value = $(that.elementIDs.selectAllFieldsJQ).is(':checked');

        var gridData = $(that.elementIDs.childElementGridJS).getRowData();
        $.each(gridData, function (idx, row) {
            if (JSON.parse(row.isChecked) != value) {
                row.isChecked = value;
                that.setElementSelection(row, value);
            }
        });
        $(that.elementIDs.childElementGridJS).jqGrid('setGridParam', { datatype: 'local', data: gridData }).trigger('reloadGrid');
        if (value) {
            $(that.elementIDs.selectAllFieldsJQ).attr('checked', 'checked');
        }
        else {
            $(that.elementIDs.selectAllFieldsJQ).removeAttr('checked');
        }
    });
}

var selectMacroeDailog = function () {
    var parentInstance;
    var URLs = {
        macroSearch: '/DocumentSync/SyncMacroList?formInstanceID={formInstanceID}',
        addMacro: '/DocumentSync/AddSyncMacro?formInstanceID={formInstanceID}',
        deleteMacro: '/DocumentSync/DeleteMacro',
        copyMacro: '/DocumentSync/CopyMacro?macroID={macroID}'
    };
    var elementIDs = {
        //id for dialog div
        macroDailogJQ: "#selectMacroDialog",
        macroGrid: "macroGrid",
        macroGridJS: "#macroGrid",
        macroGridPagerJQ: "#pmacroGrid",
        isPublicJQ: '#IsPublic'
    }

    function init() {
        //register dialog 
        $(elementIDs.macroDailogJQ).dialog({
            autoOpen: false,
            height: 450,
            width: 800,
            modal: true
        });
    }

    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    //initialize the dialog after this js is loaded
    init();

    function loadMacroGrid(dialogParent) {
        var currentInstance = this;
        var colArray = ['MacroID', 'Macro Name', 'Notes', '', '', 'Is Public'];
        var colModel = [];

        colModel.push({ name: 'MacroID', index: 'MacroID', key: true, hidden: true, editable: false, align: 'left' });
        colModel.push({ name: 'MacroName', index: 'MacroName', editable: true, editrules: { required: true }, align: 'left' });
        colModel.push({ name: 'Notes', index: 'Notes', editable: true, editrules: { required: true }, align: 'left' });
        colModel.push({ name: 'FormDesignID', index: 'FormDesignID', hidden: true });
        colModel.push({ name: 'FormDesignVersionID', index: 'FormDesignVersionID', hidden: true });
        colModel.push({
            name: 'IsPublic', index: 'IsPublic', width: 40, align: 'center', hidden: false,
            editable: true, edittype: 'checkbox', formatter: function (cellvalue, options, rowObject) {
                if (cellvalue == true || cellvalue == "true") return '<span align="center" class="ui-icon ui-icon-check" style="margin:auto" ></span>';
                else return '<span align="center" class="ui-icon ui-icon-close" style="margin:auto" ></span>';
            }, unformat: function (cellvalue, options, rowObject) {
                var checked = $(rowObject).children('span').attr('class');
                if (checked == "ui-icon ui-icon-check") return 'true';
                else return 'false';
            }, editoptions: { value: 'true:false' }, formatoptions: { disabled: true }
        });

        $(elementIDs.macroGridJS).jqGrid('GridUnload');
        $(elementIDs.macroGridJS).parent().append("<div id='p" + elementIDs.macroGrid + "'></div>");
        var url = URLs.macroSearch.replace('{formInstanceID}', dialogParent.documentSyncData.SourceDocument.FormInstanceID);
        $(elementIDs.macroGridJS).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Macro',
            height: '230',
            rowNum: 20,
            rowList: [10, 20, 30],
            ignoreCase: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: elementIDs.macroGridPagerJQ,
            sortname: 'MacroID',
            altclass: 'alternate',
            ondblClickRow: function () {
                var selectedRowId = $(elementIDs.macroGridJS).jqGrid('getGridParam', 'selrow');
                if (selectedRowId != null && selectedRowId != undefined) {
                    var rowData = $(elementIDs.macroGridJS).jqGrid('getRowData', selectedRowId);
                    dialogParent.isNewMacro = false;
                    setSelectedMacro(dialogParent, rowData.MacroID, rowData.IsPublic);
                }
            },
            gridComplete: function () {
                //To set first row of grid as selected.
                console.log("grid complete");
            }
        });
        var pagerElement = elementIDs.macroGridPagerJQ;
        //remove default buttons
        $(elementIDs.macroGridJS).jqGrid('navGrid', pagerElement, { edit: false, add: true, del: false, search: false, refresh: false },
            {}, {
                url: URLs.addMacro.replace("{formInstanceID}", dialogParent.documentSyncData.SourceDocument.FormInstanceID),
                closeOnEscape: true,
                reloadAfterSubmit: true,
                closeAfterAdd: true,
                addCaption: "Add Macro",
                beforeShowForm: function (form) {
                    //if (roleID == parseInt(Role.Superuser) || roleID == parseInt(Role.HNESuperUser) || roleID == parseInt(Role.ProductDesigner) ||
                    //    roleID == parseInt(Role.ProductSME)) {
                        $(form.selector).find(elementIDs.isPublicJQ).attr("disabled", false);
                    //}
                    //else {
                    //    $(form.selector).find(elementIDs.isPublicJQ).attr("disabled", true);
                    //}
                },
                afterComplete: function (response, postdata, formid) {
                    if (response.responseText != null && response.responseText != undefined) {
                        var macroID = JSON.parse(response.responseText).Items[0].Messages[0];
                        if (macroID == "Please Enter Unique Macro Name") {
                            messageDialog.show("Please Enter Unique Macro Name.");
                        }
                        else {
                            dialogParent.isNewMacro = true;
                            setSelectedMacro(dialogParent, macroID, false);
                        }
                    }
                    else {
                        messageDialog.show("Error while adding a Macro.");
                    }
                }
            }, {});

        if (roleID != null && roleID != undefined) {
            //if (roleID == parseInt(Role.Superuser) || roleID == parseInt(Role.HNESuperUser) || roleID == parseInt(Role.ProductDesigner) ||
              //  roleID == parseInt(Role.ProductSME)) {
                $(elementIDs.macroGridJS).jqGrid('navButtonAdd', pagerElement, {
                    caption: '', buttonicon: 'ui-icon-trash', title: 'Delete',
                    onClickButton: function () {
                        var gr = $(elementIDs.macroGridJS).jqGrid('getGridParam', 'selrow');
                        if (gr != null) $(elementIDs.macroGridJS).jqGrid('delGridRow', gr, { url: URLs.deleteMacro, addCaption: "Delete Macro", reloadAfterSubmit: true });
                        else messageDialog.show("Please Select Row to delete!");
                    }
                });
            //}
        }

        $(elementIDs.macroGridJS).jqGrid('navButtonAdd', pagerElement, {
            caption: '', buttonicon: 'ui-icon-check', title: 'Select',
            onClickButton: function () {
                var selectedRowId = $(elementIDs.macroGridJS).jqGrid('getGridParam', 'selrow');
                if (selectedRowId != null && selectedRowId != undefined) {
                    var rowData = $(elementIDs.macroGridJS).jqGrid('getRowData', selectedRowId);
                    dialogParent.isNewMacro = false;
                    setSelectedMacro(dialogParent, rowData.MacroID, rowData.IsPublic);
                }
                else {
                    messageDialog.show("Please select a row.");
                }
            }
        });

        $(elementIDs.macroGridJS).jqGrid('navButtonAdd', pagerElement, {
            caption: '', buttonicon: 'ui-icon-copy', title: 'Copy',
            onClickButton: function () {
                var selectedRowId = $(elementIDs.macroGridJS).jqGrid('getGridParam', 'selrow');
                if (selectedRowId != null && selectedRowId != undefined) {
                    var copyUrl = URLs.copyMacro.replace("{macroID}", selectedRowId);
                    $(elementIDs.macroGridJS).jqGrid('editGridRow', "new", {
                        url: copyUrl, addCaption: "Copy Macro", reloadAfterSubmit: false, closeAfterAdd: true, afterComplete: function (response, postdata, formid) {
                            if (response.responseText != null && response.responseText != undefined) {
                                var macroID = JSON.parse(response.responseText).Items[0].Messages[0];
                                dialogParent.isNewMacro = true;
                                setSelectedMacro(dialogParent, macroID, false);
                            }
                        }
                    });
                }
                else {
                    messageDialog.show("Please select a Macro.");
                }
            }
        });

        $(elementIDs.macroGridJS).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: true, defaultSearch: "cn",
            beforeSearch: function (item) {
                var isPublicFilter = false;

                postData = $(elementIDs.macroGridJS).jqGrid('getGridParam', 'postData'),
                filters = $.parseJSON(postData.filters);

                if (filters && typeof filters.rules !== 'undefined' && filters.rules.length > 0) {
                    rules = filters.rules;

                    $.each(rules, function (idx, item) {
                        if (item.field == 'IsPublic' && item.data != "") {
                            item.data = (item.data == "Yes" || item.data == "yes" || item.data == "YES") ? 'true'
                                      : ((item.data == "No" || item.data == "no" || item.data == "NO") ? 'false' : item.data);
                            isPublicFilter = true;
                        }

                    });
                    if (isPublicFilter)
                        postData.filters = JSON.stringify(filters);
                }
            }

        });

        function setSelectedMacro(parentInstance, macroID, isPublic) {
            if (macroID != null && macroID != undefined) {
                var objData = { macroID: macroID };
                var promise = ajaxWrapper.postJSON("/DocumentSync/GetMacroDefinition", objData);

                promise.done(function (xhr) {
                    parentInstance.documentSyncData.SelectedMacro = xhr;
                    $.extend(parentInstance.documentSyncData.SelectedMacro, { Template: JSON.parse(xhr.MacroJSON) });
                    //parentInstance.setSelectedMacro(roleID == parseInt(Role.Superuser) || roleID == parseInt(Role.HNESuperUser) || roleID == parseInt(Role.ProductDesigner) ||
                    //                                roleID == parseInt(Role.ProductSME) || !JSON.parse(isPublic));
                    parentInstance.setSelectedMacro(!JSON.parse(isPublic));
                    $(elementIDs.macroDailogJQ).dialog("close");
                });

                promise.fail(showError);
            }
            else {
                messageDialog.show("Please select a row.");
            }
        }
    }

    return {
        show: function (dialogParent) {
            parentInstance = dialogParent;
            $(elementIDs.macroDailogJQ).dialog('option', 'title', "Select a Macro");
            $(elementIDs.macroDailogJQ).dialog("open");
            loadMacroGrid(dialogParent);
            $(elementIDs.macroDailogJQ).dialog({
                position: {
                    my: 'center',
                    at: 'center'
                },
            });
        }
    }
}();


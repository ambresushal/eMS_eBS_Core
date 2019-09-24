function expressionRulesDialogNew(uiElement, formDesignId, formDesignVersionId, effecttiveDate) {
    this.uiElement = uiElement;
    this.formDesignId = formDesignId;
    this.documentRuleID = -1;
    this.formDesignVersionId = formDesignVersionId;
    this.effecttiveDate = effecttiveDate;
    this.ruleTester = new expressionRuleTester();
    this.currentElementList;
}

var codeMirrorEditor;
expressionRulesDialogNew.ruleType = '';
expressionRulesDialogNew.documentViewList = '';

expressionRulesDialogNew.elementIDs = {
    expressionDialogold: '#expressionRuledialog',
    expressionDialog: '#expressionRuledialogNew',
    jsonData: '#jsonDataNew',
    elementGrid: "elementGrid",
    elementGridJQ: "#elementGrid",
    ruleTypeDropDown: "#ruleTypeDropDown",
    targetElementName: "#targetElementName",
    textDescription: "#txtDescription",
    deleteButton: "#btnDelete",
    expressionRulePanel: '#expressionRulePanel',
    expressionRuleNewHTML: '#expressionRuleNewHTML',
    elementGridHTML: '#elementGridHTML',
    btnPanel: '#btnPanel',
    templateDropDown: "#templateDropDown",
    lbltemplateDropDown: "#lbltemplateDropDown",
    btnValidateRule: "#btnValidateRule",
    btnFormatRuleText: "#btnFormatRuleText",
    btnSaveExpresssionData: "#btnSaveExpresssionData",
    btnDeleteExpresssionRule: "#btnDeleteExpresssionRule",
    libtnFormatRuleText: "#libtnFormatRuleText",
    libtnValidateRule: "#libtnValidateRule",
    libtnSaveExpresssionData: "#libtnSaveExpresssionData",
    libtnDeleteExpresssionRule: "#libtnDeleteExpresssionRule",
    libtnDownloadExpresssionData: "#libtnDownloadExpresssionData",
    btnDownloadExpresssionData: "#btnDownloadExpresssionData"
}

expressionRulesDialogNew.templates = {
    BUILDREPORTSTRING: 'IF(EQUALS(varName1, "true")<>TRUE())\n{\n\t\tBUILDREPORTSTRING();\n}',
    BUILDREPORTTABLE: 'IF(EQUALS(varName1, "true")<>TRUE())\n{\n\t\tSET(varName2,value);BUILDREPORTTABLE(varName2);\n}'
};

expressionRulesDialogNew.URLs = {
    documentRuleList: '/UIElement/DocumentRuleNew?uiElementId={uiElementId}&formDesignId={formDesignId}&formDesignVersionId={formDesignVersionId}&effecttiveDate={effecttiveDate}',
    getdocumentRuleType: '/UIElement/GetDocumentRuleType',
    savedocumentRule: '/UIElement/SaveDocumentRuleNew',
    deletedocumentRule: '/UIElement/DeleteDocumentRule?dRulesID={docRulesID}',
    getdocumentviewlisturl: '/UIElement/GetDocumentViewList?formDesignId={formDesignId}',
    validateExpressionRule: '/UIElement/ValidateExpressionRule?tenantId=1&formDesignID={formDesignID}&formDesignVersionId={formDesignVersionId}&ruleID={ruleID}',
    getElementList: '/UIElement/GetUIElementListForExpressionRule?tenantId=1&formDesignId={formDesignId}&effecttiveDate={effecttiveDate}',
    downloadDocumentRule: '/UIElement/DownloadDocumentRuleJson?dRulesID={docRulesID}',
}

expressionRulesDialogNew._isInitialized = false;

expressionRulesDialogNew.prototype.show = function () {
    var currentInstance = this;
    $(expressionRulesDialogNew.elementIDs.expressionDialogold).hide();
    $('.nav-tabs a[href="' + expressionRulesDialogNew.elementIDs.expressionRulePanel + '"]').tab('show');
    $(expressionRulesDialogNew.elementIDs.libtnValidateRule).show();
    $(expressionRulesDialogNew.elementIDs.libtnFormatRuleText).show();
    $(expressionRulesDialogNew.elementIDs.libtnSaveExpresssionData).show();
    $(expressionRulesDialogNew.elementIDs.libtnDeleteExpresssionRule).show();
    $(expressionRulesDialogNew.elementIDs.libtnDownloadExpresssionData).show();
    $(expressionRulesDialogNew.elementIDs.targetElementName).val(currentInstance.uiElement.Label);
    $(expressionRulesDialogNew.elementIDs.textDescription).val("Populating " + currentInstance.uiElement.Label);
    $(expressionRulesDialogNew.elementIDs.expressionDialog).dialog('option', 'title', 'Expression Builder Dialog');
    $(expressionRulesDialogNew.elementIDs.expressionDialog).dialog('open');
    currentInstance.initializCodeMirror();
    currentInstance.getRuleType();
    currentInstance.bindRuleTypeChangeEvent();
    currentInstance.getDocumentViews();
    currentInstance.loadElementGrid();
    currentInstance.fillDialogData();
    currentInstance.getTemplateData();
    currentInstance.ruleTester.registerTabEvents();
    currentInstance.bindTemplateChangeEvent();
    currentInstance.bindSaveExpressionDataEvent();
    currentInstance.bindDeleteExpresssionRuleEvent();
    currentInstance.bindValidateRuleDataEvent();
    currentInstance.bindFormatRuleTextEvent();
    currentInstance.bindDownloadExpressionDataEvent();
    currentInstance.bindJqGridInlineAfterSaveRow();
}

expressionRulesDialogNew.prototype.bindRuleTypeChangeEvent = function () {
    var currentInstance = this;
    $(expressionRulesDialogNew.elementIDs.ruleTypeDropDown).unbind().bind('change', function (e) {
        var elementData = $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('getRowData');
        if ($(expressionRulesDialogNew.elementIDs.ruleTypeDropDown + " option:selected").val() == "5") {
            $(expressionRulesDialogNew.elementIDs.templateDropDown).show();
            $(expressionRulesDialogNew.elementIDs.lbltemplateDropDown).show();
            $("#templateDropdownHelp").show();
        } else {
            $(expressionRulesDialogNew.elementIDs.templateDropDown).hide();
            $(expressionRulesDialogNew.elementIDs.lbltemplateDropDown).hide();
            $("#templateDropdownHelp").hide();
        }
        $(elementData).each(function (idx, data) {
            data.sourcedocumentfilter = $(expressionRulesDialogNew.elementIDs.ruleTypeDropDown + " option:selected").val() == "5" ? "yes" : "No";
            $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('setRowData', idx + 1, data);
        });
    });
}

expressionRulesDialogNew.prototype.bindTemplateChangeEvent = function () {
    $(expressionRulesDialogNew.elementIDs.templateDropDown).unbind().bind('change', function (e) {
        codeMirrorEditor.setValue('');
        var selectedValue = $('option:selected', this).val();
        if (selectedValue != "0") {
            codeMirrorEditor.replaceSelection(expressionRulesDialogNew.templates[selectedValue], focus);
        }
    });
}

expressionRulesDialogNew.init = function () {
    var currentInstance = this;
    if (expressionRulesDialogNew._isInitialized == false) {
        $(expressionRulesDialogNew.elementIDs.expressionDialog).dialog({
            autoOpen: false,
            height: '550',
            width: '70%',
            modal: true,
            position: {
                my: 'center',
                at: 'center'
            },
            close: function () {
                $(expressionRulesDialogNew.elementIDs.jsonData).val('');
                $(expressionRulesDialogNew.elementIDs.textDescription).val('');
                codeMirrorEditor.setValue('');
            }
        });
        expressionRulesDialogNew._isInitialized = true;

        $("#ruleTypeHelp").addClass("input-validation-error");
        $("#ruleTypeHelp").attr("data-original-title", "Rule type [Report] option is used for Collateral reports related rules.");
        $("#ruleTypeHelp").attr("data-toggle", "tooltip");
        $("#ruleTypeHelp").tooltip({
            placement: "left",
            trigger: "hover",
        });

        $("#templateDropdownHelp").addClass("input-validation-error");
        $("#templateDropdownHelp").attr("data-original-title", "User can use these predefined templates for collateral reports.");
        $("#templateDropdownHelp").attr("data-toggle", "tooltip");
        $("#templateDropdownHelp").tooltip({
            placement: "left",
            trigger: "hover",
        });
    }

}

expressionRulesDialogNew.init();

expressionRulesDialogNew.prototype.initializCodeMirror = function () {
    if (codeMirrorEditor === undefined) {
        codeMirrorEditor = CodeMirror.fromTextArea(document.getElementById("jsonDataNew"), {
            lineNumbers: true,
            lineWrapping: true,
            autoCloseBrackets: true,
            extraKeys: {
                "Ctrl-Space": "autocomplete",
                "F11": function (cm) {
                    cm.setOption("fullScreen", !cm.getOption("fullScreen"));
                },
                "Esc": function (cm) {
                    if (cm.getOption("fullScreen")) cm.setOption("fullScreen", false);
                }
            },
            mode: { name: "javascript", globalVars: true }
        });

        codeMirrorEditor.on("change", function () {
            $(expressionRulesDialogNew.elementIDs.jsonData).val(codeMirrorEditor.getValue());
        });
    }
}

expressionRulesDialogNew.prototype.fillDialogData = function () {
    var currentInstance = this;
    var url = expressionRulesDialogNew.URLs.documentRuleList.replace(/{uiElementId}/g, currentInstance.uiElement.UIElementID).replace(/{formDesignId}/g, currentInstance.formDesignId)
                                                            .replace(/{formDesignVersionId}/g, currentInstance.formDesignVersionId)
                                                            .replace(/{effecttiveDate}/g, currentInstance.effecttiveDate);
    var promise = ajaxWrapper.getJSON(url);
    promise.done(function (data) {
        if (data != null && data != undefined && data.length > 0) {
            var ruleData = data[0];

            if (ruleData != null && ruleData != undefined) {
                currentInstance.documentRuleID = ruleData.DocumentRuleID;
                currentInstance.ruleTester.init(currentInstance.documentRuleID, currentInstance.formDesignId, currentInstance.formDesignVersionId);
                if (ruleData.Description != "")
                    $(expressionRulesDialogNew.elementIDs.textDescription).val(ruleData.Description);

                $(expressionRulesDialogNew.elementIDs.ruleTypeDropDown + " option").each(function (idx, control) {
                    var optionID = $(control).val();
                    if (optionID == ruleData.DocumentRuleTypeID.toString()) {
                        $(control).prop('selected', true);
                    }
                });
                if (ruleData.DocumentRuleTypeID.toString() == "5") {
                    $(expressionRulesDialogNew.elementIDs.templateDropDown).show();
                    $(expressionRulesDialogNew.elementIDs.lbltemplateDropDown).show();
                    $("#templateDropdownHelp").show();
                } else {
                    $(expressionRulesDialogNew.elementIDs.templateDropDown).hide();
                    $(expressionRulesDialogNew.elementIDs.lbltemplateDropDown).hide();
                    $("#templateDropdownHelp").hide();
                }
                $(expressionRulesDialogNew.elementIDs.jsonData).val(ruleData.RuleJSON);
                codeMirrorEditor.setValue(ruleData.RuleJSON);
                if (ruleData.ElementData != null && ruleData.ElementData != undefined && ruleData.ElementData != "") {
                    var elementdata = JSON.parse(ruleData.ElementData);
                    $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('clearGridData');
                }

                $(elementdata).each(function (idx, data) {
                    $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('addRowData', idx + 1, {});
                    var viewName = data.sourceelement.split('[')[0];
                    var elementpath = data.sourceelement.replace(viewName, "").replace("[", "").replace("]", "");
                    var elementRow = { 'RowID': idx + 1, 'sourcename': data.sourcename, 'selectview': data.sourceformdesignid + "_" + viewName, 'formDesignId': data.sourceformdesignid, 'formDesignName': viewName, 'sourceelement': elementpath, 'sourceelementlabel': data.sourceelementlabel, 'sourcedocumentfilter': data.sourcedocumentfilter };
                    $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('setRowData', idx + 1, elementRow);
                });

            }
        }
    });
}

expressionRulesDialogNew.prototype.getSourceName = function () {
    var elementgridData = $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('getRowData');
    var sourceNameInt = 64;
    $(elementgridData).each(function (idx, row) {
        var nextSourceNameInt = row.sourcename.charCodeAt(0);
        if (!isNaN(nextSourceNameInt) && nextSourceNameInt > sourceNameInt) {
            sourceNameInt = nextSourceNameInt;
        }
    });
    return String.fromCharCode(sourceNameInt + 1);
}

expressionRulesDialogNew.prototype.loadElementGrid = function () {
    var currentInstance = this;

    $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('GridUnload');

    $(expressionRulesDialogNew.elementIDs.elementGridJQ).parent().append("<div id='p" + expressionRulesDialogNew.elementIDs.elementGrid + "'></div>");

    $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid({
        datatype: "local",
        height: '125',
        pager: '#p' + expressionRulesDialogNew.elementIDs.elementGrid,
        caption: 'Sources',
        rowheader: true,
        loadonce: true,
        autowidth: true,
        viewrecords: true,
        altRows: true,
        altclass: 'alternate',
        onSelectRow: editRow,
        hidegrid: false,
        colNames: ['RowID', 'Name', 'Document Design', 'Form Design Id', 'Form Design Name', 'Source Element JSON Path', 'Source Element', 'ElementType', 'Filter'],
        colModel: [
             { name: 'RowID', index: 'RowID', key: true, hidden: true, width: 15, align: 'center' },
             { name: 'sourcename', index: 'sourcename', width: 10, align: 'center' },
            {
                name: 'selectview', index: 'selectview', width: 30, align: 'left',
                editable: true,
                edittype: "select",
                formatter: 'select',
                editoptions: {
                    value: expressionRulesDialogNew.documentViewList,
                    dataEvents: [
								{
								    'type': 'change',
								    'fn': function (el) {
								        var formDesignId = $(el.target).val().split("_")[0];
								        var formDesignName = $(el.target).val().split("_")[1];
								        var rowid = $(el.target).closest("tr.jqgrow").attr("id");
								        var totalData = {
								            'formDesignId': formDesignId,
								            'formDesignName': formDesignName,
								            'sourceelement': '',
								        };
								        $("#" + rowid + "_sourceelementlabel").val('');
								        $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('setRowData', rowid, totalData);
								        currentInstance.fillSourceElement();
								        $("#" + rowid + "_sourceelementlabel").css('height', 'auto');
								    },
								}
                    ]

                }
            },
            { name: 'formDesignId', index: 'formDesignId', hidden: true },
            { name: 'formDesignName', index: 'formDesignName', hidden: true },
            { name: 'sourceelement', index: 'sourceelement', hidden: true },
            {
                name: 'sourceelementlabel', index: 'sourceelementlabel', align: 'left',
                editable: true,
                edittype: "text",
                editoptions: {
                    dataInit: function (element) {
                        currentInstance.fillSourceElement();
                        $(element).css('height', 'auto');
                    }
                }
            },
            { name: 'sourceelementtype', index: 'sourceelementtype', hidden: true },
            { name: 'sourcedocumentfilter', width: 10, index: 'sourcedocumentfilter', align: 'center', editable: false, formatter: currentInstance.formatCheckBoxColumn },
        ],
        gridComplete: function () {
            $(expressionRulesDialogNew.elementIDs.expressionDialog).dialog({
                position: {
                    my: 'center',
                    at: 'center'
                }
            });
        },
    });

    var pagerElement = '#p' + expressionRulesDialogNew.elementIDs.elementGrid;

    $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();
    $(pagerElement).find('#refresh_elementGrid').remove();

    //$(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('navButtonAdd', pagerElement,
    //{
    //    caption: '', buttonicon: 'ui-icon-search',
    //    onClickButton: function () {
    //        var selectedRow = $(expressionRulesDialogNew.elementIDs.elementGridJQ).getGridParam('selrow');
    //        if (selectedRow == undefined && selectedRow == null) {
    //            messageDialog.show("Please select a row to search for element");
    //        }
    //        else {
    //            var row = $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('getRowData', selectedRow);
    //            if (row.formDesignId != "") {
    //                var expDialog = new expressionRuleElementSearchDialog(1, expressionRulesDialogNew.elementIDs.elementGridJQ, row.formDesignId, currentInstance.effecttiveDate);
    //                expDialog.show();
    //            } else {
    //                messageDialog.show("Please select source document to search for element.");
    //            }
    //        }
    //    }
    //});

    $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-plus',
        onClickButton: function () {
            var rowid = $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('getRowData').length;
            var sourceName = currentInstance.getSourceName();
            var _sourcedocumentfilter = $(expressionRulesDialogNew.elementIDs.ruleTypeDropDown + " option:selected").val() == "5" ? "yes" : "No";
            var totalData = {
                'RowID': rowid + 1,
                'sourcename': sourceName,
                'sourcedocumentfilter': _sourcedocumentfilter,
            };
            $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('addRowData', rowid + 1, totalData);
        }
    });

    $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-trash',
        onClickButton: function () {
            var rowId = $(this).getGridParam('selrow');
            if (rowId == null) {
                messageDialog.show("Please select atleast a row to Delete");
            }
            else if (rowId != null) {
                confirmDialog.show(DocumentDesign.deleteElement, function () {
                    confirmDialog.hide();
                    $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('delRowData', rowId);
                    $($(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('getRowData')).each(function (idx, row) {
                        var existingId = row.RowID;
                        row.RowID = idx + 1;
                        row.sourcedocumentfilter = $(expressionRulesDialogNew.elementIDs.ruleTypeDropDown + " option:selected").val() == "5" ? "yes" : "No";
                        $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('setRowData', existingId, row);
                        $(expressionRulesDialogNew.elementIDs.elementGridJQ + " #" + existingId).attr('id', idx + 1);
                    });
                });
            }
        }
    });

    var lastSelection;
    function editRow(id) {
        var grid = $(expressionRulesDialogNew.elementIDs.elementGridJQ);
        if (id && id !== lastSelection) {
            grid.jqGrid('saveRow', lastSelection);
            grid.jqGrid('editRow', id, { keys: true });
            lastSelection = id;
        } else if (id == lastSelection) {
            grid.jqGrid('editRow', id, { keys: true });
        }
        //setInterval(function () {
        //    $("#" + id + "_selectview").focus();
        //}, 500);
    }

    //$(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid().focusout(function () {
    //    var row = $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('getGridParam', 'selrow');
    //    $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('saveRow', row);
    //});

}

expressionRulesDialogNew.prototype.fillSourceElement = function () {
    var currentInstance = this;
    currentInstance.currentElementList = [];
    var rowid = $(expressionRulesDialogNew.elementIDs.elementGridJQ).getGridParam('selrow');
    var data = $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('getRowData', rowid);
    var url = expressionRulesDialogNew.URLs.getElementList.replace(/{formDesignId}/g, data.formDesignId).replace(/{effecttiveDate}/g, currentInstance.effecttiveDate);
    var element = $("#" + rowid + "_sourceelementlabel");
    $(element).autocomplete({
        id: 'AutoComplete',
        select: function (event, ui) {
            var selectedElement = currentInstance.currentElementList.filter(function (elem) {
                return elem.label == ui.item.label
            })
            var data;
            if (selectedElement != null && selectedElement != undefined && selectedElement.length > 0) {
                data = { 'sourceelement': selectedElement[0].value };
            } else {
                data = { 'sourceelement': '' };
            }
            $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('setRowData', rowid, data);
        },
        source: function (request, response) {
            this.xhr = $.ajax({
                url: url,
                data: request,
                dataType: "json",
                global: false,
                success: function (data) {
                    var autocompleteObject = [];
                    currentInstance.currentElementList = [];
                    $(data).each(function (idx, row) {
                        var obj = { value: row.ElementFullPath, label: row.ElementFullPath }
                        autocompleteObject.push(obj);

                        currentInstance.currentElementList.push({ 'label': row.ElementFullPath, 'value': row.ElementJSONPath });
                    });
                    response(autocompleteObject);
                },
                error: function (model, response, options) {
                    response([]);
                }
            });
        },
        autoFocus: true
    });
}

expressionRulesDialogNew.prototype.formatCheckBoxColumn = function (cellValue, options, rowObject) {
    var result;
    if (cellValue == undefined) {
        result = "<span class='glyphicon glyphicon-remove text-black'></span>";
    } else {
        if (cellValue.toLowerCase() == "yes")
            result = "<span class='glyphicon glyphicon-ok text-black'></span>";
        else
            result = "<span class='glyphicon glyphicon-remove text-black'></span>";
    }
    return result;
}

expressionRulesDialogNew.prototype.groupViews = function (objectArray, property) {
    return objectArray.reduce(function (acc, obj) {
        var key = obj[property];
        if (!acc[key]) {
            acc[key] = [];
        }
        acc[key].push(obj);
        return acc;
    }, {});
}

expressionRulesDialogNew.prototype.bindSaveExpressionDataEvent = function () {
    var currentInstance = this;
    $(expressionRulesDialogNew.elementIDs.btnSaveExpresssionData).unbind().bind('click', function () {
        var rowId = $(expressionRulesDialogNew.elementIDs.elementGridJQ).getGridParam('selrow');
        if (rowId != 'undefined') {
            if ($(expressionRulesDialogNew.elementIDs.elementGridJQ + " tr#" + rowId).attr("editable") === "1") {
                $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('saveRow', rowId);
            }
        }
        var elementGridAllData = $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('getRowData');
        var description = $(expressionRulesDialogNew.elementIDs.textDescription).val();
        var ruleType = $(expressionRulesDialogNew.elementIDs.ruleTypeDropDown + " option:selected").val();
        var jsonDataStr = $(expressionRulesDialogNew.elementIDs.jsonData).val().trim();

        if (currentInstance.validateRule(description, elementGridAllData, jsonDataStr)) {
            var expList = [];
            var elementGridData = [];
            $(elementGridAllData).each(function (idx, row) {
                var filter = row.sourcedocumentfilter.includes("ok") ? "yes" : "none";
                var elementGridDataRow = { sourcename: row.sourcename, sourceelement: row.formDesignName + "[" + row.sourceelement + "]", sourceelementlabel: row.sourceelementlabel, sourcedocumentfilter: filter };
                elementGridData.push(elementGridDataRow);
            });
            expList.push({
                DocumentRuleID: currentInstance.documentRuleID,
                DisplayText: description,
                Description: description,
                DocumentRuleTypeID: ruleType,
                RuleJSON: jsonDataStr,
                FormDesignID: currentInstance.formDesignId,
                FormDesignVersionID: currentInstance.formDesignVersionId,
                TargetUIElementID: currentInstance.uiElement.UIElementID,
                ElementData: JSON.stringify(elementGridData)
            });
            currentInstance.SaveExpressionBuilderRule(currentInstance.uiElement.UIElementID, currentInstance.uiElement.ElementType, currentInstance.formDesignId, currentInstance.formDesignVersionId, expList);
        }
    })
}

expressionRulesDialogNew.prototype.validateRule = function (description, elementGridAllData, jsonDataStr) {
    var isValidRule = true;

    if (description.length > 100) {
        messageDialog.show(DocumentDesign.expDescriptionLength);
        isValidRule = false;
    } else if (elementGridAllData.length <= 0) {
        messageDialog.show(DocumentDesign.expEmptyElementGrid);
        isValidRule = false;
    } else if (jsonDataStr.length <= 0) {
        messageDialog.show(DocumentDesign.expEnterScript);
        isValidRule = false;
    } else if (elementGridAllData.length > 0) {
        $(elementGridAllData).each(function (idx, row) {
            if (row.sourceelement == undefined || row.sourceelement == "") {
                messageDialog.show("Source element does not exist.");
                isValidRule = false;
                return false;
            }
        });
    }
    return isValidRule;
}

expressionRulesDialogNew.prototype.bindValidateRuleDataEvent = function () {
    var currentInstance = this;
    $(expressionRulesDialogNew.elementIDs.btnValidateRule).unbind().bind('click', function () {
        if (currentInstance.documentRuleID > 0) {
            var url = expressionRulesDialogNew.URLs.validateExpressionRule.replace(/{formDesignID}/g, currentInstance.formDesignId)
                                                                          .replace(/{formDesignVersionId}/g, currentInstance.formDesignVersionId)
                                                                          .replace(/{ruleID}/g, currentInstance.documentRuleID);
            var promise = ajaxWrapper.getJSONSync(url);
            promise.done(function (result) {
                messageDialog.show(result);
            });
        } else {
            messageDialog.show("Save expression rule before execute the validation.");
        }
    })
}

expressionRulesDialogNew.prototype.bindFormatRuleTextEvent = function () {
    $(expressionRulesDialogNew.elementIDs.btnFormatRuleText).unbind().bind('click', function () {
        codeMirrorEditor.setValue(js_beautify(codeMirrorEditor.getValue()));
    })
}

expressionRulesDialogNew.prototype.bindDeleteExpresssionRuleEvent = function () {
    var currentInstance = this;
    $(expressionRulesDialogNew.elementIDs.btnDeleteExpresssionRule).unbind().bind('click', function () {
        if (currentInstance.documentRuleID > 0) {
            confirmDialog.show(DocumentDesign.deleteRule, function () {
                confirmDialog.hide();
                var url = expressionRulesDialog.URLs.deletedocumentRule.replace(/\{docRulesID\}/g, currentInstance.documentRuleID);
                var promise = ajaxWrapper.getJSON(url);
                promise.done(function (xhr) {
                    if (xhr.Result === ServiceResult.SUCCESS) {
                        currentInstance.documentRuleID = -1;
                        currentInstance.ruleTester.documentRuleId = -1;
                        $(expressionRulesDialogNew.elementIDs.jsonData).val('');
                        $(expressionRulesDialogNew.elementIDs.textDescription).val('');
                        codeMirrorEditor.setValue('');
                        $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('clearGridData');
                        $(expressionRulesDialogNew.elementIDs.ruleTypeDropDown).prop("selectedIndex", 0);
                        $(expressionRulesDialogNew.elementIDs.templateDropDown).prop("selectedIndex", 0);
                    }
                    else {
                        messageDialog.show(Common.errorMsg);
                    }
                });

                promise.fail(showError);
            });

        } else {
            messageDialog.show(DocumentDesign.expDeleteData);
        }
    })
}

expressionRulesDialogNew.prototype.bindDownloadExpressionDataEvent = function () {
    var currentInstance = this;
    $(expressionRulesDialogNew.elementIDs.btnDownloadExpresssionData).unbind().bind('click', function () {
        if (currentInstance.documentRuleID > 0) {
            var url = expressionRulesDialogNew.URLs.downloadDocumentRule.replace(/\{docRulesID\}/g, currentInstance.documentRuleID);
            var promise = ajaxWrapper.getJSONSync(url);
            promise.done(function (data) {
                var beautifyData = js_beautify(data);
                download(beautifyData, "ExpressionRules.json", "text/json");
            });
        }
        else {
            messageDialog.show(DocumentDesign.downloadExpressionJsonErrorMsg);
        }
    });

}

expressionRulesDialogNew.prototype.bindJqGridInlineAfterSaveRow = function () {
    var currentInstance = this;
    $(expressionRulesDialogNew.elementIDs.elementGridJQ).bind("jqGridInlineAfterSaveRow", function (e, rowid, jqXhrOrBool, postData, options) {
        var item = $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('getLocalRow', rowid);
        var sourceLabel = item.sourceelementlabel;
        var data;
        if (sourceLabel != undefined) {
            if (sourceLabel != '') {
                if (currentInstance.currentElementList.length > 0) {
                    var selectedElement = currentInstance.currentElementList.filter(function (elem) {
                        return elem.label == sourceLabel
                    })
                    if (selectedElement != null && selectedElement != undefined && selectedElement.length > 0) {
                        data = { 'sourceelement': selectedElement[0].value };
                    } else {
                        data = { 'sourceelement': '' };
                    }
                    $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('setRowData', rowid, data);
                }
            }
            else {
                data = { 'sourceelement': '' }; $(expressionRulesDialogNew.elementIDs.elementGridJQ).jqGrid('setRowData', rowid, data);
            }
        }
    });
}

expressionRulesDialogNew.prototype.SaveExpressionBuilderRule = function (uiElementId, uiElementType, formDesignId, formDesignVersionId, expRuleList) {
    var currentInstance = this;
    var rowListlen = expRuleList.length;
    var updateExpRule = new Array(rowListlen);

    for (var i = 0; i < rowListlen; i++) {
        var updateExpressionRecord = new Object();
        updateExpressionRecord.DocumentRuleID = expRuleList[i].DocumentRuleID;
        updateExpressionRecord.DisplayText = expRuleList[i].DisplayText;
        updateExpressionRecord.Description = expRuleList[i].Description;
        updateExpressionRecord.DocumentRuleTypeID = expRuleList[i].DocumentRuleTypeID;
        updateExpressionRecord.RuleJSON = expRuleList[i].RuleJSON;
        updateExpressionRecord.FormDesignID = expRuleList[i].FormDesignID;
        updateExpressionRecord.FormDesignVersionID = expRuleList[i].FormDesignVersionID;
        updateExpressionRecord.TargetUIElementID = expRuleList[i].TargetUIElementID;
        updateExpressionRecord.ElementData = expRuleList[i].ElementData;
        updateExpRule[i] = updateExpressionRecord;
    }
    var expressionRuleList = {
        uiElementId: uiElementId,
        uiElementType: uiElementType,
        formDesignId: formDesignId,
        formDesignVersionId: formDesignVersionId,
        nRules: updateExpRule
    };

    var url = expressionRulesDialogNew.URLs.savedocumentRule;
    var promise = ajaxWrapper.postJSONCustom(url, expressionRuleList);
    promise.done(function (xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            currentInstance.documentRuleID = xhr.Items[0].Messages.toString();
            currentInstance.ruleTester.init(currentInstance.documentRuleID, currentInstance.formDesignId, currentInstance.formDesignVersionId);
            messageDialog.show(DocumentDesign.expSaveElementMsg);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
    });
    promise.fail(showError);
}

expressionRulesDialogNew.prototype.getDocumentViews = function () {
    var currentInstance = this;
    var url = expressionRulesDialogNew.URLs.getdocumentviewlisturl.replace(/\{formDesignId\}/g, currentInstance.formDesignId)
    var prom = ajaxWrapper.getJSONSync(url)
    prom.done(function (result) {
        //expressionRulesDialogNew.documentViewList = result;
        var formattedResult = "SelectOne:Select One;";
        $(result).each(function (idx, val) {
            formattedResult = formattedResult + val.FormDesignID + "_" + val.FormDesignName + ":" + val.FormDesignDisplayName + ";"
        });
        formattedResult = formattedResult.slice(0, -1);
        expressionRulesDialogNew.documentViewList = formattedResult;
    });
}

expressionRulesDialogNew.prototype.getRuleType = function () {
    var currentInstance = this;
    var promise = ajaxWrapper.getJSONSync(expressionRulesDialogNew.URLs.getdocumentRuleType);
    promise.done(function (data) {
        $(expressionRulesDialogNew.elementIDs.ruleTypeDropDown).empty();
        expressionRulesDialogNew.ruleType = data;
        var result = "";
        for (var i = 0; i < data.length; i++) {
            result += "<option id="
                        + data[i].DocumentRuleTypeID
                        + " value=" + data[i].DocumentRuleTypeID
                        + ">"
                        + data[i].DisplayText
                        + "</option>";
        }
        $(expressionRulesDialogNew.elementIDs.ruleTypeDropDown).append(result);
    });
}

expressionRulesDialogNew.prototype.getTemplateData = function (focusOn) {
    var currentInstance = this;
    $(expressionRulesDialogNew.elementIDs.templateDropDown).empty();
    var result = "";
    result = "<option value=0>Select One</option>" +
             "<option value=BUILDREPORTSTRING>BUILDREPORTSTRING</option>" +
             "<option value=BUILDREPORTTABLE>BUILDREPORTTABLE</option>";

    $(expressionRulesDialogNew.elementIDs.templateDropDown).append(result);
}

function showError(xhr) {
    if (xhr.status == 999)
        this.location = '/Account/LogOn';
    else
        messageDialog.show(JSON.stringify(xhr));
}
function download(content, filename, contentType) {
    if (!contentType) contentType = 'application/octet-stream';
    var a = document.createElement('a');
    var blob = new Blob([content], { 'type': contentType });
    a.href = window.URL.createObjectURL(blob);
    a.download = filename;
    a.click();
}
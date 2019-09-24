var rulesDialog = function (uiElement, formDesignVersionId, status, elementGridData) {
    this.uiElement = uiElement;
    this.elementGridData = elementGridData;
    this.formDesignVersionId = formDesignVersionId;
    this.expressionBuilders = [];
    this.currentExpressionBuilder = undefined;
    this.elementDropDownList = undefined;
    this.rulesData = undefined;
    this.readOnlyRulesData = [];
    this.rulesRowId = 0;
    this.statustext = status;
    this.logicalOperators = [];
    this.operators = [];
    this.targetProperties = [];
    this.isExist = false;
    this.rulesDialogNewUI = undefined;
    this.isParentRepeater = false;
    this.designRulesTesterData = [];
}

rulesDialog.URLs = {
    rulesList: '/UIElement/Rules?tenantId=1&formDesignVersionId={formDesignVersionId}&uiElementId={uiElementId}',
    getLogicalOperatorList: '/UIElement/GetLogicalOperatorList?tenantId=1',
    getOperatorList: '/UIElement/GetOperatorList?tenantId=1',
    getTargetPropertyList: '/UIElement/GetTargetPropertyList?tenantId=1',
    getRuleDescription: '/FormInstanceRuleExecutionLog/GetRuleDescription?ruleID={ruleID}',
}

rulesDialog.TargetProperties = {
    Enabled: 1,
    RunValidation: 2,
    Value: 3,
    Visible: 4,
    IsRequired: 5,
    Error: 6,
    Highlight: 8,
    Dialog: 9,
    CustomRule: 10
}


rulesDialog._currentInstance = undefined;
rulesDialog._isInitialized = false;

rulesDialog.elementIDs = {
    rulesDialog: '#rulesdialog',
    rulesGrid: 'rulesgrid',
    rulesGridJQ: '#rulesgrid',
    expressionBuilder: 'expressionbuilder',
    expressionBuilderJQ: '#expressionbuilder'
}

rulesDialog.showerror = function (xhr) {
    if (xhr.status == 999)
        this.location = '/Account/LogOn';
    else
        messageDialog.show(JSON.stringify(xhr));
}

//init dialog
rulesDialog.init = function () {
    if (rulesDialog._isInitialized == false) {
        $(rulesDialog.elementIDs.rulesDialog).dialog({
            autoOpen: false,
            height: '700',
            width: '90%',
            modal: true
        });
        rulesDialog._isInitialized = true;
    }
}();

rulesDialog.prototype.getCurrentElementId = function () {
    return this.uiElement.UIElementID;
}

rulesDialog.prototype.show = function () {
    var currentInstance = this;
    rulesDialog._currentInstance = this;
    //var dialogContent = this.getHtmlTemplate();
    currentInstance.initRulesDialogNewUI();
    var dialogContent = currentInstance.rulesDialogNewUI.getHtmlTemplate();

    $(rulesDialog.elementIDs.rulesDialog).empty();
    $(rulesDialog.elementIDs.rulesDialog).append(dialogContent);

    $(rulesDialog.elementIDs.rulesDialog).dialog('option', 'title', 'Edit Rules - ' + this.uiElement.Label)
    $(rulesDialog.elementIDs.rulesDialog).dialog('open');
    $(rulesDialog.elementIDs.rulesDialog).dialog({
        beforeClose: function (event, ui) {
            var result = currentInstance.validateRulesData();
            if (result == false) {
                messageDialog.show("The rules have some invalid conditions. Please validate before closing the dialog.");
            }
            return result;
        }
    });
    var currentInstance = this;
    currentInstance.rulesDialogNewUI.registerTabEvents();
    $.when(ajaxWrapper.getJSON(rulesDialog.URLs.getLogicalOperatorList),
           ajaxWrapper.getJSON(rulesDialog.URLs.getOperatorList),
           ajaxWrapper.getJSON(rulesDialog.URLs.getTargetPropertyList)
           ).done(function (logicalOpResult, operatorResult, targetPropResult) {
               currentInstance.logicalOperators = logicalOpResult[0];
               currentInstance.operators = operatorResult[0];
               var list = targetPropResult[0];
               currentInstance.isParentRepeater = false;
               var parentElement = currentInstance.elementGridData.filter(function (elem) {
                   return elem.UIElementID == currentInstance.uiElement.parent || elem.UIElementID == currentInstance.uiElement.ParentUIElementId;
               });
               if (parentElement != null && parentElement.length > 0) {
                   if (parentElement[0].ElementType == 'Repeater') {
                       currentInstance.isParentRepeater = true;
                   }
               }
               if (currentInstance.uiElement.ElementType == 'Section' || currentInstance.uiElement.ElementType == 'Repeater') {
                   list = targetPropResult[0].filter(function (t) {
                       return t.TargetPropertyName == 'Visible';
                   });
               }
               else if (currentInstance.uiElement.ElementType == 'Label') {
                   list = targetPropResult[0].filter(function (t) {
                       return t.TargetPropertyName == 'Visible' || t.TargetPropertyName == 'Value';
                   });
               }
               if (currentInstance.isParentRepeater == true) {
                   if (currentInstance.uiElement.ElementType == 'Label') {
                       list = targetPropResult[0].filter(function (t) {
                           //return t.TargetPropertyName == 'Visible' || t.TargetPropertyName == 'Value';
                           return t.TargetPropertyName == 'Value';
                       });
                   }
                   else {
                       list = targetPropResult[0].filter(function (t) {
                           //return t.TargetPropertyName == 'Visible' || t.TargetPropertyName == 'Enabled' || t.TargetPropertyName == 'Value' || t.TargetPropertyName == 'Error' || t.TargetPropertyName == 'CustomRule';
                           return t.TargetPropertyName == 'Visible' || t.TargetPropertyName == 'Enabled' || t.TargetPropertyName == 'Value' || t.TargetPropertyName == 'Error' || t.TargetPropertyName == 'Highlight' || t.TargetPropertyName == 'Dialog';
                       });
                   }
               }
               currentInstance.targetProperties = list;
               currentInstance.loadRulesGrid();
           }).fail(rulesDialog.showerror);
}

rulesDialog.prototype.open = function () {
    var currentInstance = this;
    currentInstance.initRulesDialogNewUI();
    $(rulesDialog.elementIDs.rulesDialog).dialog('open');
}

rulesDialog.prototype.getRuleTesterData = function () {
    var currentInstance = this;
    return currentInstance.rulesDialogNewUI.getRuleTesterData();
}

rulesDialog.prototype.setRuleTesterData = function (designRulesTesterData) {
    var currentInstance = this;
    currentInstance.designRulesTesterData = designRulesTesterData;
}

rulesDialog.prototype.initRulesDialogNewUI = function () {
    var currentInstance = this;
    if (currentInstance.rulesDialogNewUI == undefined || currentInstance.rulesDialogNewUI == null) {
        currentInstance.rulesDialogNewUI = new rulesDialogNewUI(currentInstance);
    }
}

rulesDialog.prototype.getHtmlTemplate = function () {
    var rulesDialogHtml = "<div class='panel-body'>" +
                            "<div>" +
                            "<div class='row-fluid'>" +
                            "<div class='col-xs-12 col-md-12 col-lg-12 col-sm-12'>" +
                            "<div class='rulecontainer'><div id='ruleDesc'></div><div id='expressionbuilder'></div></div>" +
                            "</div>" +
                            "</div>" +
                            "<div class='row-fluid'>" +
                            "<div class='col-xs-12 col-md-12 col-lg-12 col-sm-12'></div>" +
                            "</div>" +
                            "<div class='row-fluid'>" +
                            "<div class='col-xs-12 col-md-12 col-lg-12 col-sm-12'>" +
                            "<div class='grid-wrapper'><table id='rulesgrid'></table></div>" +
                            "<span class='help-block'>Changes will be Saved locally. They will be Saved to the system only if the Save button on the Properties Grid is clicked.</span>" +
                            "</div>" +
                            "</div>" +
                            "</div>" +
                            "</div>";
    return rulesDialogHtml;
}

rulesDialog.prototype.loadRulesGrid = function () {
    var currentInstance = this;
    var showCol = !currentInstance.isParentRepeater;
    $(rulesDialog.elementIDs.expressionBuilderJQ).empty();

    //set column list
    var colArray = ['RuleId', 'PropertyRuleMapID', 'Rule Type', 'If Function is True', 'If Function is False', 'Validation Message','Is Standard', 'Run On Load', 'Key', ' Key Filter'];
    //set column models
    var colModel = [];
    colModel.push({ name: 'RuleId', index: 'RuleId', key: true, hidden: true, search: false });
    colModel.push({ name: 'PropertyRuleMapID', index: 'PropertyRuleMapID', hidden: true });
    colModel.push({ name: 'TargetPropertyId', index: 'TargetPropertyId', align: 'center', editable: false, formatter: currentInstance.formatColumns, unformat: currentInstance.unformatColumns, targetProperties: currentInstance.targetProperties, sortable: false });
    colModel.push({ name: 'ResultSuccess', index: 'ResultSuccess', align: 'center', editable: false, formatter: currentInstance.formatColumns, unformat: currentInstance.unformatColumns, sortable: false });
    colModel.push({ name: 'ResultFailure', index: 'ResultFailure', align: 'center', editable: true, formatter: currentInstance.formatColumns, unformat: currentInstance.unformatColumns, sortable: false });
    colModel.push({ name: 'Message', index: 'Message', hidden: false, editable: true, formatter: currentInstance.formatColumns, unformat: currentInstance.unformatColumns, sortable: false });
    //colModel.push({ name: 'Message', index: 'Message', hidden: true });
    colModel.push({
        name: 'IsStandard', index: 'IsStandard', hidden: false,
        formatoptions: { disabled: false, }, align: 'center', width: 60, sortable: false, formatter: currentInstance.formatColumns, unformat: currentInstance.unformatColumns
    });
    colModel.push({
        name: 'RunOnLoad', index: 'RunOnLoad', hidden: false,
        formatoptions: { disabled: false, }, align: 'center', width: 60, sortable: false, formatter: currentInstance.formatColumns, unformat: currentInstance.unformatColumns
    });
    colModel.push({ name: 'Key', index: 'Key', width: 40, align: 'center', hidden: showCol, editable: false, formatter: currentInstance.imageFormatter });
    colModel.push({ name: 'TargetKeyFilter', index: 'TargetKeyFilter', hidden: true, search: false, editable: false });
    //get URL for grid
    var rulesURL = rulesDialog.URLs.rulesList.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId).replace(/\{uiElementId\}/g, this.uiElement.UIElementID);
    //unload previous grid values
    $(rulesDialog.elementIDs.rulesGridJQ).jqGrid('GridUnload');

    $(rulesDialog.elementIDs.rulesGridJQ).parent().append("<div id='p" + rulesDialog.elementIDs.rulesGrid + "'></div>");

    $(rulesDialog.elementIDs.rulesGridJQ).jqGrid({
        datatype: 'local',
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Rules',
        pager: '#p' + rulesDialog.elementIDs.rulesGrid,
        height: '150',
        rowheader: true,
        loadonce: true,
        autowidth: true,
        viewrecords: true,
        altRows: true,
        altclass: 'alternate',
        hidegrid: false,
        afterInsertRow: function (rowId, rowData, rowElem) {
            $(rulesDialog.elementIDs.rulesGridJQ).find('#TargetProperty' + rowId).on('change', function () {
                var targetPropertyId = $(this).val();

                if (targetPropertyId != rulesDialog.TargetProperties.Error) {
                    currentInstance.isExist = false;
                }
                if (targetPropertyId == rulesDialog.TargetProperties.Dialog || targetPropertyId == rulesDialog.TargetProperties.RunValidation || targetPropertyId == rulesDialog.TargetProperties.Error
                    || targetPropertyId == rulesDialog.TargetProperties.CustomRule) {
                    $(rulesDialog.elementIDs.rulesGridJQ).showCol("Message");
                }

                var rowObject = {
                    RuleId: rowId,
                    TargetPropertyId: targetPropertyId,
                    ResultSuccess: '',
                    ResultFailure: '',
                    IsResultSuccessElement: '',
                    IsResultSuccessFailure: '',
                    Message: ''
                };
                var rowElement = $(rulesDialog.elementIDs.rulesGridJQ).find('#' + rowId);
                var resultSuccessCell = $(rowElement).find('td[aria-describedby="rulesgrid_ResultSuccess"]');
                var resultFailureCell = $(rowElement).find('td[aria-describedby="rulesgrid_ResultFailure"]');
                var messageCell = $(rowElement).find('td[aria-describedby="rulesgrid_Message"]');
                var runOnLoadCell = $(rowElement).find('td[aria-describedby="rulesgrid_RunOnLoad"]');
                var isStandardCell = $(rowElement).find('td[aria-describedby="rulesgrid_IsStandard"]');
                var options = {};
                var model = colModel.filter(function (model) {
                    return model.name == 'ResultSuccess';
                });
                options.colModel = model[0];
                resultSuccessCell.html(currentInstance.formatColumns('', options, rowObject));
                var model = colModel.filter(function (model) {
                    return model.name == 'ResultFailure';
                });
                options.colModel = model[0];
                resultFailureCell.html(currentInstance.formatColumns('', options, rowObject));
                var model = colModel.filter(function (model) {
                    return model.name == 'Message';
                });
                options.colModel = model[0];
                messageCell.html(currentInstance.formatColumns('', options, rowObject));

                var model = colModel.filter(function (model) {
                    return model.name == 'RunOnLoad';
                });
                options.colModel = model[0];
                runOnLoadCell.html(currentInstance.formatColumns('', options, rowObject));

                var model = colModel.filter(function (model) {
                    return model.name == 'IsStandard';
                });
                options.colModel = model[0];
                isStandardCell.html(currentInstance.formatColumns('', options, rowObject));

                currentInstance.registerRulesRowEvents(rowId, rowObject);
                //currentInstance.isErrorRuleExists(rowId, rowObject);
            });
            currentInstance.registerRulesRowEvents(rowId, rowData);
        },
        beforeSelectRow: function (rowid, e) {
            var result = true;
            if (currentInstance.currentExpressionBuilder != null) {
                result = currentInstance.currentExpressionBuilder.saveRule();
                if (result == true) {
                    currentInstance.currentExpressionBuilder.close();
                }
            }
            var $self = $(this), $td = $(e.target).closest("td"), iCol = $.jgrid.getCellIndex($td[0]);
            var cm = $self.jqGrid("getGridParam", "colModel");
            if (cm[iCol].name === "Key" && e.target.tagName.toUpperCase() === "I") {
                var rules = currentInstance.rulesData.filter(function (rl) {
                    return rl.RuleId == rowid;
                });
                if (rules != null && rules.length > 0) {
                    var filterGrid = new repeaterKeyFilter(currentInstance.uiElement, null, 'Target', rules[0]);
                    filterGrid.show();
                }
            }
            return result;
        },
        onSelectRow: function (rowId) {
            $("#selectedRule").text($("#TargetProperty" + rowId)[0].selectedOptions[0].label);
            var ruleDescription = undefined;
            var url = rulesDialog.URLs.getRuleDescription.replace(/\{ruleID\}/g, rowId);
            var promise = ajaxWrapper.getJSONSync(url);
            promise.done(function (result) {
                ruleDescription = result.RuleDescription;
            });
            if (currentInstance.rulesDialogNewUI.activeTab == 'ruleTester') {
                currentInstance.rulesDialogNewUI.changeCurrentRuleTestData();
            } else {
                var filteredRules = currentInstance.rulesData.filter(function (control) {
                    return control.RuleId == rowId;
                });
                var filteredRule = {};
                if (filteredRules == null || filteredRules.length == 0) {
                    filteredRule = {};
                    $.extend(filteredRule, $(rulesDialog.elementIDs.rulesGridJQ).jqGrid('getRowData', rowId));
                    currentInstance.rulesData.push(filteredRule);
                }
                else {
                    filteredRules[0].RuleDescription = ruleDescription;
                    filteredRule = filteredRules[0];
                }
                var filteredExpression = currentInstance.expressionBuilders.filter(function (control) {
                    return control.RuleID == rowId;
                });
                var expressionControl = null;
                if (filteredExpression == null || filteredExpression.length == 0) {
                    var searchGridData = currentInstance.getGridForExpression();
                    if (searchGridData.length <= 0) {
                        searchGridData = currentInstance.elementGridData;
                    }
                    if (filteredRule.hasOwnProperty('RootExpression')) {
                        expressionControl = new expressionBuilder(currentInstance.uiElement, filteredRule, rulesDialog.elementIDs.expressionBuilder,
                            searchGridData, currentInstance.logicalOperators, currentInstance.operators);
                    }
                    else {
                        filteredRule.RootExpression = null;
                        expressionControl = new expressionBuilder(currentInstance.uiElement, filteredRule, rulesDialog.elementIDs.expressionBuilder,
                            searchGridData, currentInstance.logicalOperators, currentInstance.operators);
                    }
                    currentInstance.expressionBuilders.push({ RuleID: rowId, ExpressionBuilder: expressionControl });
                }
                else {
                    expressionControl = filteredExpression[0].ExpressionBuilder;
                }
                expressionControl.setCurrent();
                expressionControl.render();
                currentInstance.currentExpressionBuilder = expressionControl;
                currentInstance.updateDataFromGrid();
            }
            if (ClientRolesForFormDesignAccess.indexOf(vbRole) >= 0 && currentInstance.uiElement.IsStandard == "true") {
                $("#expressionbuilder").find(':input').attr('disabled', 'disabled');
                $(rulesDialog.elementIDs.rulesGridJQ).find('select').attr('disabled', 'disabled');
                $(rulesDialog.elementIDs.rulesGridJQ).find('input').attr('disabled', 'disabled');
                $(rulesDialog.elementIDs.rulesGridJQ).find('textarea').attr('disabled', 'disabled');
                $(rulesDialog.elementIDs.rulesGridJQ).find('input:checkbox').attr('disabled', 'disabled');
            } else {
                $("#expressionbuilder").find(':input').removeAttr('disabled');
                $(rulesDialog.elementIDs.rulesGridJQ).find('select').removeAttr('disabled');
                $(rulesDialog.elementIDs.rulesGridJQ).find('input').removeAttr('disabled');
                $(rulesDialog.elementIDs.rulesGridJQ).find('textarea').removeAttr('disabled');
                $(rulesDialog.elementIDs.rulesGridJQ).find('input:checkbox').removeAttr('disabled');
            }
        },
        gridComplete: function () {
            var grid = this;
            var rows = $(this).getDataIDs();
            var rowdata = $(this).jqGrid('getGridParam', 'data');
            for (i = 0; i < rows.length; i++) {
                if (rowdata[i].TargetPropertyId == 9 || rowdata[i].TargetPropertyId == 2 || rowdata[i].TargetPropertyId == 6
                    || rowdata[i].TargetPropertyId == 10) {
                    $(rulesDialog.elementIDs.rulesGridJQ).showCol("Message");
                }
                else {
                    //$(rulesDialog.elementIDs.rulesGridJQ).hideCol("Message");
                }
            }

            if (ClientRolesForFormDesignAccess.indexOf(vbRole) >= 0 && currentInstance.uiElement.IsStandard == "true") {
                $('#p' + rulesDialog.elementIDs.rulesGrid).hide();
                $(rulesDialog.elementIDs.expressionBuilderJQ).addClass('divdisabled');
            } else {
                $('#p' + rulesDialog.elementIDs.rulesGrid).show();
                $(rulesDialog.elementIDs.expressionBuilderJQ).removeClass('divdisabled');
            }
        }
    });

    var pagerElement = '#p' + rulesDialog.elementIDs.rulesGrid;
    //remove default buttons
    $(rulesDialog.elementIDs.rulesGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    //remove paging
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    //if (this.statustext != 'Finalized') {
    $(rulesDialog.elementIDs.rulesGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-plus',
        onClickButton: function () {
            if (currentInstance.currentExpressionBuilder) {
                currentInstance.currentExpressionBuilder.close();
            }
            var rowId = currentInstance.rulesRowId - 1;
            currentInstance.rulesRowId = currentInstance.rulesRowId - 1;
            var targetProperty = 1;
            if (currentInstance.uiElement.ElementType == 'Section' || currentInstance.uiElement.ElementType == 'Repeater') {
                targetProperty = 4;
            }
            var rule = { RuleId: rowId, PropertyRuleMapID: 0, TargetPropertyId: targetProperty, ResultSuccess: '', ResultFailure: '', IsResultSuccessElement: '', IsResultFailureElement: '', Message: '', IsStandard:true, RunOnLoad: true };
            $(rulesDialog.elementIDs.rulesGridJQ).jqGrid('addRowData', rowId, rule);
            currentInstance.rulesData.push(rule);
            $(rulesDialog.elementIDs.rulesGridJQ).jqGrid('setSelection', rowId);
        }
    });

    $(rulesDialog.elementIDs.rulesGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-trash',
        onClickButton: function () {
            var rowId = $(rulesDialog.elementIDs.rulesGridJQ).getGridParam('selrow');
            if (rowId != null) {
                $(rulesDialog.elementIDs.rulesGridJQ).jqGrid('delRowData', rowId);
                //remove from rulesData and expressionBuilders
                for (var idx = 0; idx < currentInstance.rulesData.length; idx++) {
                    if (currentInstance.rulesData[idx].RuleId == rowId) {
                        currentInstance.rulesData.splice(idx, 1);
                        break;
                    }
                }
                for (var idxExp = 0; idxExp < currentInstance.expressionBuilders.length; idxExp++) {
                    if (currentInstance.expressionBuilders[idxExp].RuleID == rowId) {
                        currentInstance.expressionBuilders.splice(idxExp, 1);
                        break;
                    }
                }
                currentInstance.currentExpressionBuilder.close();
                currentInstance.currentExpressionBuilder = undefined;
                var rowIds = $(rulesDialog.elementIDs.rulesGridJQ).getDataIDs();
                if (rowIds.length > 0) {
                    $(rulesDialog.elementIDs.rulesGridJQ).jqGrid('setSelection', rowIds[0]);
                }
            }
        }
    });
    //}
    var promise = ajaxWrapper.getJSON(rulesURL);
    promise.done(function (xhr) {
        currentInstance.rulesData = xhr;
        var elementData = currentInstance.getGridForExpression();
        $.extend(true, currentInstance.readOnlyRulesData, currentInstance.rulesData);
        for (var index = 0; index < currentInstance.rulesData.length; index++) {
            var item = currentInstance.rulesData[index];
            var elem = elementData.filter(function (el) {
                return el.UIElementName == item.ResultSuccess;
            });
            if (elem != null && elem.length > 0) {
                item.ResultSuccessLabel = elem[0].Element;
            }
            var elem = elementData.filter(function (el) {
                return el.UIElementName == item.ResultFailure;
            });
            if (elem != null && elem.length > 0) {
                item.ResultFailureLabel = elem[0].Element;
            }

            if (elem != null && elem.length > 0) {
                item.RunOnLoad = elem[0].Element;
            }

            if (elem != null && elem.length > 0) {
                item.IsStandard = elem[0].Element;
            }

            $(rulesDialog.elementIDs.rulesGridJQ).jqGrid('addRowData', item.RuleId, item);

        }
        if (currentInstance.rulesData.length > 0) {
            var rowIds = $(rulesDialog.elementIDs.rulesGridJQ).getDataIDs();
            $(rulesDialog.elementIDs.rulesGridJQ).jqGrid('setSelection', rowIds[0]);
        }
        $(rulesDialog.elementIDs.rulesDialog).dialog({
            position: {
                my: 'center',
                at: 'center'
            },
        });
    });
    promise.fail(rulesDialog.showerror);
}

rulesDialog.prototype.registerRulesRowEvents = function (rowId, rowData) {
    if (rowData.TargetPropertyId == rulesDialog.TargetProperties.Value) {
        this.registerValueElementEvents('Success', rowId);
        this.registerValueElementEvents('Failure', rowId);
    }
}

rulesDialog.prototype.isErrorRuleExists = function (rowId, rowData) {
    var currentInstance = this;
    if (rowData.TargetPropertyId == rulesDialog.TargetProperties.Error) {
        if (currentInstance.isExist) {
            $(rulesDialog.elementIDs.rulesGridJQ).jqGrid('delRowData', rowId);
            //remove from rulesData
            for (var idx = 0; idx < currentInstance.rulesData.length; idx++) {
                if (currentInstance.rulesData[idx].RuleId == rowId) {
                    currentInstance.rulesData.splice(idx, 1);
                    break;
                }
            }
            if (currentInstance.rulesData.length > 0) {
                var rowIds = $(rulesDialog.elementIDs.rulesGridJQ).getDataIDs();
                $(rulesDialog.elementIDs.rulesGridJQ).jqGrid('setSelection', rowIds[0]);
            }
            messageDialog.show('Error Rule cannot be applied multiple times on same control.');
        }
        else if (currentInstance.rulesData.length > 1) {
            currentInstance.rulesData.filter(function (ct) {
                if (ct.TargetPropertyId == rulesDialog.TargetProperties.Error) {
                    currentInstance.isExist = true;
                }
            });
            if (currentInstance.isExist) {
                $(rulesDialog.elementIDs.rulesGridJQ).jqGrid('delRowData', rowId);
                //remove from rulesData
                for (var idx = 0; idx < currentInstance.rulesData.length; idx++) {
                    if (currentInstance.rulesData[idx].RuleId == rowId) {
                        currentInstance.rulesData.splice(idx, 1);
                        break;
                    }
                }
                if (currentInstance.rulesData.length > 0) {
                    var rowIds = $(rulesDialog.elementIDs.rulesGridJQ).getDataIDs();
                    $(rulesDialog.elementIDs.rulesGridJQ).jqGrid('setSelection', rowIds[0]);
                }
                messageDialog.show('Error Rule cannot be applied multiple times on same control.');
            }
            else {
                currentInstance.isExist = true;
            }
        }
        else {
            currentInstance.isExist = true;
        }
    }
}

rulesDialog.prototype.registerValueElementEvents = function (valueType, rowId) {
    var currentInstance = this;
    var row = $(rulesDialog.elementIDs.rulesGridJQ).find('#' + rowId);
    var resultCheckElement = row.find('#IsResult' + valueType + 'Element' + rowId);
    var resultLabelElement = row.find('#Result' + valueType + 'Label' + rowId);
    var resultElement = row.find('#Result' + valueType + rowId);
    var searchElement = row.find('#Search' + valueType + 'Element' + rowId);
    searchElement.on('click', function () {
        var searchDialog = new expressionBuilderSearchDialog(currentInstance.getGridForExpression(), resultElement, resultLabelElement, false, null);
        searchDialog.show();
    });

    resultCheckElement.on('change', function () {
        if ($(this)[0].checked == true) {
            $(searchElement).css('visibility', 'visible');
            $(resultLabelElement).attr('readonly', true);
            $(resultLabelElement).val('');
            $(resultElement).val('');
            $(resultLabelElement).on('keydown', function (event) {
                if (event.key == 'Backspace') {
                    event.stopPropagation();
                    return false;
                }
            });
        }
        else {
            $(searchElement).css('visibility', 'hidden');
            $(resultLabelElement).attr('readonly', false);
            $(resultLabelElement).val('');
            $(resultElement).val('');
            $(resultLabelElement).off('keydown');
        }
    });
    if ($(resultCheckElement)[0].checked == true) {
        $(resultLabelElement).on('keydown', function (event) {
            if (event.key == 'Backspace') {
                event.stopPropagation();
                return false;
            }
        });
    }
    else {
        $(resultLabelElement).off('keydown');

    }

}

rulesDialog.prototype.formatColumns = function (cellValue, options, rowObject) {
    var result;
    if (cellValue === undefined || cellValue === null) {
        cellValue = '';
    }
    switch (options.colModel.index) {
        case 'TargetPropertyId':
            result = "<select id='TargetProperty" + rowObject.RuleId + "'>";
            if (options.colModel.targetProperties != null && options.colModel.targetProperties.length > 0) {
                var targetProperties = options.colModel.targetProperties;
                for (var idx = 0; idx < targetProperties.length; idx++) {
                    if (cellValue == targetProperties[idx].TargetPropertyId) {
                        result = result + "<option value='" + targetProperties[idx].TargetPropertyId + "' selected>" + targetProperties[idx].TargetPropertyName + "</option>";
                    }
                    else {
                        result = result + "<option value='" + targetProperties[idx].TargetPropertyId + "'>" + targetProperties[idx].TargetPropertyName + "</option>";
                    }
                }
            }
            result = result + "</select>";
            break;
        case 'ResultSuccess':
        case 'ResultFailure':
            if (rowObject.TargetPropertyId == rulesDialog.TargetProperties.Value) {
                var resultCheckId;
                var resultLabelId;
                var resultId;
                var searchId;
                var val;
                var label;
                var isCheck;
                var visible;
                var readOnly = '';
                if (options.colModel.index == "ResultSuccess") {
                    resultId = 'ResultSuccess' + rowObject.RuleId;
                    resultLabelId = 'ResultSuccessLabel' + rowObject.RuleId;
                    resultCheckId = 'IsResultSuccessElement' + rowObject.RuleId;
                    searchId = 'SearchSuccessElement' + rowObject.RuleId;
                    val = rowObject.ResultSuccess;
                    if (rowObject.IsResultSuccessElement == true) {
                        isCheck = 'checked';
                        visible = 'visible';
                        label = rowObject.ResultSuccessLabel;
                        readOnly = "readonly='true'";
                    }
                    else {
                        label = val;
                    }
                }
                else {
                    resultId = 'ResultFailure' + rowObject.RuleId;
                    resultLabelId = 'ResultFailureLabel' + rowObject.RuleId;
                    resultCheckId = 'IsResultFailureElement' + rowObject.RuleId;
                    searchId = 'SearchFailureElement' + rowObject.RuleId;
                    val = rowObject.ResultFailure;
                    label = rowObject.ResultFailureLabel;
                    if (rowObject.IsResultFailureElement == true) {
                        isCheck = 'checked';
                        visible = 'visible';
                        label = rowObject.ResultFailureLabel;
                        readOnly = "readonly='true'";
                    }
                    else {
                        label = val;
                    }
                }
                if (val == null) {
                    val = '';
                }
                if (label == null) {
                    label = '';
                }
                if (isCheck == null) {
                    isCheck = '';
                }
                if (visible == null) {
                    visible = 'hidden';
                }
                result = "<div class='jqgridcelldiv'>";
                result = result + "<input type='text' id='" + resultLabelId + "' value='" + label + "'" + readOnly + "/>";
                result = result + "<input type='hidden' id='" + resultId + "' value='" + val + "'/>";
                result = result + "<input type='checkbox' id='" + resultCheckId + "' " + isCheck + "/>";

                result = result + "<span class='ui-icon ui-icon-search' id='" + searchId + "' style='display: inline-block;visibility:" + visible + "'></span></div>";
            }
            else if (rowObject.TargetPropertyId == rulesDialog.TargetProperties.Enabled) {
                if (options.colModel.index == "ResultSuccess") {
                    result = 'Field is enabled';
                }
                else {
                    result = 'Field is not enabled';
                }
                break;
            }
            else if (rowObject.TargetPropertyId == rulesDialog.TargetProperties.Visible) {
                if (options.colModel.index == "ResultSuccess") {
                    result = 'Field is visible';
                }
                else {
                    result = 'Field is not visible';
                }
            }
            else if (rowObject.TargetPropertyId == rulesDialog.TargetProperties.IsRequired) {
                if (options.colModel.index == "ResultSuccess") {
                    result = 'Field is required';
                }
                else {
                    result = 'Field is not required';
                }
            }
            else if (rowObject.TargetPropertyId == rulesDialog.TargetProperties.RunValidation) {
                if (options.colModel.index == "ResultSuccess") {
                    result = 'Validation will be run on Field';
                }
                else {
                    result = 'Validation will not be run on Field';
                }
            }
            else if (rowObject.TargetPropertyId == rulesDialog.TargetProperties.Error) {
                if (options.colModel.index == "ResultSuccess") {
                    result = 'Error is not returned';
                }
                else {
                    result = 'Error is returned';
                }
            }
            else if (rowObject.TargetPropertyId == rulesDialog.TargetProperties.Highlight) {
                if (options.colModel.index == "ResultSuccess") {
                    result = 'Field is highlighted';
                }
                else {
                    result = 'Field is not highlighted';
                }
                break;
            }
            else if (rowObject.TargetPropertyId == rulesDialog.TargetProperties.Dialog) {
                if (options.colModel.index == "ResultSuccess") {
                    result = 'Dialog is displayed';
                }
                else {
                    result = 'Dialog is not displayed';
                }
                break;
            }
            else if (rowObject.TargetPropertyId == rulesDialog.TargetProperties.CustomRule) {
                var resultLabelId;
                var resultId;
                var val;
                var label;
                var visible;
                var readOnly = '';
                if (options.colModel.index == "ResultSuccess") {
                    resultId = 'ResultSuccess' + rowObject.RuleId;
                    resultLabelId = 'ResultSuccessLabel' + rowObject.RuleId;
                    val = rowObject.ResultSuccess;
                    if (rowObject.IsResultSuccessElement == true) {
                        visible = 'visible';
                        label = rowObject.ResultSuccessLabel;
                        readOnly = "readonly='true'";
                    }
                    else {
                        label = val;
                    }
                    if (val == null) {
                        val = '';
                    }
                    if (label == null) {
                        label = '';
                    }
                    if (visible == null) {
                        visible = 'hidden';
                    }
                    result = "<div class='jqgridcelldiv'>";
                    result = result + "<input type='text' id='" + resultLabelId + "' value='" + label + "'" + readOnly + "/>";
                    result = result + "<input type='hidden' id='" + resultId + "' value='" + val + "'/>";
                    result = result + "</div>";
                }
                else {
                    result = 'No Call to Custom Rule.';
                }
            }
            else {
                result = cellValue;
            }
            break;
        case 'Message':
            if (rowObject.TargetPropertyId == rulesDialog.TargetProperties.IsRequired || rowObject.TargetPropertyId == rulesDialog.TargetProperties.RunValidation || rowObject.TargetPropertyId == rulesDialog.TargetProperties.Dialog || rowObject.TargetPropertyId == rulesDialog.TargetProperties.Error
                || rowObject.TargetPropertyId == rulesDialog.TargetProperties.CustomRule) {
                var resultId = "Message" + rowObject.RuleId;
                result = "<textarea type='text' id='" + resultId + "'maxlength='1000' >" + cellValue + "</textarea>";
            }
            else {
                result = "";
            }
            break;
        case 'IsStandard':
            var resultId = "IsStandard" + rowObject.RuleId;
            if (cellValue === true) {
                result = "<input type='checkbox' id='" + resultId + "' checked='" + cellValue + "'/>";
            }
            else {
                result = "<input type='checkbox' id='" + resultId + "' unchecked />";
            }
            break;
        case 'RunOnLoad':
            var resultId = "RunOnLoad" + rowObject.RuleId;
            if (cellValue === true) {
                result = "<input type='checkbox' id='" + resultId + "' checked='" + cellValue + "'/>";
            }
            else {
                result = "<input type='checkbox' id='" + resultId + "' unchecked />";
            }
            break;
        default:
            result = cellValue;
            break;
    }
    return result;
}

rulesDialog.prototype.unformatColumns = function (cellValue, options) {
    var result;
    result = $(this).find('#' + options.rowId).find('#' + options.colModel.index).val();
    return result;
}

rulesDialog.prototype.imageFormatter = function (cellValue, options, rowObject) {
    return "<span class='buttoncursor' title='Enter filter criteria'><i class='material-icons'>vpn_key</i></span>";
}

rulesDialog.prototype.validateRulesData = function () {
    var result = true;
    for (var idx = 0; idx < this.expressionBuilders.length; idx++) {
        result = this.expressionBuilders[idx].ExpressionBuilder.saveRule();
        if (result == false) {
            break;
        }
    }
    return result;
}

rulesDialog.prototype.getRulesData = function () {
    for (var idx = 0; idx < this.expressionBuilders.length; idx++) {
        this.expressionBuilders[idx].ExpressionBuilder.saveRule();
    }
    this.updateDataFromGrid();
    return this.rulesData;
}

rulesDialog.prototype.updateDataFromGrid = function () {
    var rowIds = $(rulesDialog.elementIDs.rulesGridJQ).getDataIDs();
    if (rowIds.length > 0) {
        for (var idx = 0; idx < rowIds.length; idx++) {
            var rowId = rowIds[idx];
            var row = $(rulesDialog.elementIDs.rulesGridJQ).find('#' + rowIds[idx]);
            var rules = this.rulesData.filter(function (rl) {
                return rl.RuleId == rowId;
            });
            if (rules != null && rules.length > 0) {
                var rule = rules[0];
                var elem = row.find('#TargetProperty' + rowId);
                rule.TargetPropertyId = elem[0].value;
                if (rule.TargetPropertyId == rulesDialog.TargetProperties.IsRequired || rule.TargetPropertyId == rulesDialog.TargetProperties.RunValidation
                    || rule.TargetPropertyId == rulesDialog.TargetProperties.Dialog || rule.TargetPropertyId == rulesDialog.TargetProperties.Error
                    || rule.TargetPropertyId == rulesDialog.TargetProperties.CustomRule) {
                    elem = row.find('#Message' + rowId);
                    if (elem != null && elem.length > 0) {
                        rule.Message = elem[0].value;
                    }
                }

                if (rule.TargetPropertyId == rulesDialog.TargetProperties.Value) {
                    elem = row.find('#IsResultSuccessElement' + rowId);
                    rule.IsResultSuccessElement = elem[0].checked;
                    if (rule.IsResultSuccessElement == true) {
                        elem = row.find('#ResultSuccess' + rowId);
                        rule.ResultSuccess = elem[0].value;
                    }
                    else {
                        elem = row.find('#ResultSuccessLabel' + rowId);
                        rule.ResultSuccess = elem[0].value;
                    }
                    elem = row.find('#IsResultFailureElement' + rowId);
                    rule.IsResultFailureElement = elem[0].checked;
                    if (rule.IsResultFailureElement == true) {
                        elem = row.find('#ResultFailure' + rowId);
                        rule.ResultFailure = elem[0].value;
                    }
                    else {
                        elem = row.find('#ResultFailureLabel' + rowId);
                        rule.ResultFailure = elem[0].value;
                    }
                }

                if (rule.TargetPropertyId == rulesDialog.TargetProperties.CustomRule) {
                    elem = row.find('#ResultSuccess' + rowId);
                    rule.ResultSuccess = elem[0].value;
                    if (rule.ResultSuccess == "") {
                        elem = row.find('#ResultSuccessLabel' + rowId);
                        rule.ResultSuccess = elem[0].value;
                    }
                }
                var elementIsStandard = row.find('#IsStandard' + rowId);

                var IsStandard = elementIsStandard[0].checked;
                rule.IsStandard = IsStandard;

                var elementRunOnLoad = row.find('#RunOnLoad' + rowId);

                var IsRunOnLoad = elementRunOnLoad[0].checked;
                rule.RunOnLoad = IsRunOnLoad;
            }
        }
    }
}

rulesDialog.prototype.isUpdated = function () {
    var isUpdated = false;
    //check for rule changes
    var strOriginal = JSON.stringify(this.readOnlyRulesData);
    var strModified = JSON.stringify(this.rulesData);
    if (strOriginal != strModified) {
        //add a more detailed check, if required
        isUpdated = true;
    }
    return isUpdated;
}

rulesDialog.prototype.getElementsForExpression = function () {
    //get parent element type
    var parentId = this.uiElement.parent;
    var parentElement = this.elementGridData.filter(function (element) {
        return element.parent == parentId;
    });
    var parentElementType;
    if (parentElement != null && parentElement.length > 0) {
        parentElementType = parentElement[0].ElementType;
    }
    var elements = [];
    for (var idx = 0; idx < this.elementGridData.length; idx++) {
        var elem = this.elementGridData[idx];
        if (elem.ElementType != 'Tab' && elem.ElementType != 'Section' && elem.ElementType != 'Repeater') {
            var parent = this.elementGridData.filter(function (element) {
                return element.UIElementID == elem.parent;
            });
            if (parent != null && parent.length > 0) {
                parent = parent[0];
                if (parent.ElementType == 'Section') {
                    elements.push(elem.Label);
                }
                if (parent.ElementType == 'Repeater' && elem.parent == parentId) {
                    elements.push(elem.Label);
                }
            }
        }
    }
    return elements;
}

rulesDialog.prototype.getGridForExpression = function () {
    var gridElements = [];
    var tabElements = this.elementGridData.filter(function (element) {
        return element.ElementType == 'Tab';
    });
    var tabElementId = 0;
    if (tabElements != null && tabElements.length > 0) {
        tabElementId = tabElements[0].UIElementID;
    }
    var sectionElements = this.elementGridData.filter(function (element) {
        return element.ElementType == 'Section' && element.parent == tabElementId;
    });
    for (var idx = 0; idx < sectionElements.length; idx++) {
        this.getSectionElementsForExpression('', sectionElements[idx], gridElements);
    }
    return gridElements;
}

rulesDialog.prototype.getSectionElementsForExpression = function (prependParent, section, gridElements) {
    var elements = this.elementGridData.filter(function (element) {
        return element.parent == section.UIElementID && element.ElementType != '[Blank]';
    });
    var currentParent = prependParent + section.Label;
    if (prependParent == '') {
        prependParent = section.Label + ' > ';
    }
    else {
        prependParent = prependParent + section.Label;
    }
    if (elements != null && elements.length > 0) {
        for (var idx = 0; idx < elements.length; idx++) {
            var element = elements[idx];
            if (element.ElementType == 'Repeater') {
                this.getRepeaterElementsForExpression(currentParent, element, gridElements);
            }
            else if (element.ElementType == 'Section') {
                this.getSectionElementsForExpression(prependParent, element, gridElements);
            }
            else {
                gridElements.push({ Section: currentParent, Element: element.Label, UIElementID: element.UIElementID, UIElementName: element.UIElementName, Parent: element.parent });
            }
        }
    }
}

rulesDialog.prototype.getRepeaterElementsForExpression = function (prependParent, repeater, gridElements) {
    //if (this.uiElement.parent == repeater.UIElementID) {
    var elements = this.elementGridData.filter(function (element) {
        return element.parent == repeater.UIElementID;
    });
    prependParent = prependParent + '->' + repeater.Label
    if (elements != null && elements.length > 0) {
        for (var idx = 0; idx < elements.length; idx++) {
            var element = elements[idx];
            gridElements.push({ Section: prependParent, Element: element.Label, UIElementID: element.UIElementID, UIElementName: element.UIElementName, Parent: element.parent });
        }
    }
    // }
}

//Add/Edit dialog for custom rule

//constructor for custom rule dialog.
//params:
//uiElement - uiElement of row selected of the Document Design Version UI ELements Grid
//formDesignVersionId - form design version id of the element
//status - status of the form design version
//customRule - existing custom rule for the uielement.
var customRulesDialog = function (uiElement, formDesignVersionId, status, customRule) {
    this.uiElement = uiElement;
    this.formDesignVersionId = formDesignVersionId;
    this.customRulesData = undefined;
    this.statustext = status;
    this.customRule = customRule;
}

customRulesDialog._currentInstance = undefined;

//element ID's required for custom rule
//added in Views/FormDesign/Index.cshtml
customRulesDialog.elementIDs = {
    customRulesDialog: '#customRulesdialog'
}
customRulesDialog._isInitialized = false;

//this function is called below soon after clicking on
//pencil icon for custom rule on uielement property grid.
customRulesDialog.init = function () {
    if (customRulesDialog._isInitialized == false) {
        $(customRulesDialog.elementIDs.customRulesDialog).dialog({
            autoOpen: false,
            width: 630,
            height: 350,
            modal: true
        });
        customRulesDialog._isInitialized = true;
    }
}();

//this function open  the dialog for custom rule.
customRulesDialog.prototype.show = function () {
    customRulesDialog._currentInstance = this;
    $(customRulesDialog.elementIDs.customRulesDialog).dialog('option', 'title', DocumentDesign.fieldEditCustomRules + this.uiElement.Label)
    $(customRulesDialog.elementIDs.customRulesDialog).parent().css('height', $(window).height());
    $(customRulesDialog.elementIDs.customRulesDialog).dialog('open');

    //if custom rule alreadt exists then set the text of the textbox to the custom rule o/w keep it blank.
    if (customRulesDialog._currentInstance.customRule !== undefined && customRulesDialog._currentInstance.customRule != null && customRulesDialog._currentInstance.customRule.trim() != '')
        $(customRulesDialog.elementIDs.customRulesDialog + ' textarea').val(customRulesDialog._currentInstance.customRule);
    else
        $(customRulesDialog.elementIDs.customRulesDialog + ' textarea').val('');

    //if form version is finalized then do not allow to change the custom rule.
    if (customRulesDialog._currentInstance.statustext == 'Finalized') {
        $(customRulesDialog.elementIDs.customRulesDialog + ' textarea').attr('disabled', 'disabled');
    }
    else {
        $(customRulesDialog.elementIDs.customRulesDialog + ' textarea').removeAttr('disabled');
    }

}

// this function gets the custom rule set for the uielement.
customRulesDialog.prototype.getCustomRulesData = function () {
    //get Rules
    return $(customRulesDialog.elementIDs.customRulesDialog + ' textarea').val();
}

var rulesDialogNewUI = function (currentInstance) {
    this.tenantId = 1;
    this.currentRuleDialog = currentInstance;
    this.testerDialog = undefined;
    this.activeTab = 'expressionBuilder';
}

rulesDialogNewUI.elementIDs = {
    expressionbuildertab: '#expressionbuildertab',
    rulestestergridtab: '#rulestestergridtab',
}

rulesDialogNewUI.prototype.getHtmlTemplate = function () {
    var rulesDialogNewUIHtml = "<div class='panel-body'>" +
                                    "<div><div>Currently Selected Rule Type : <b><span id='selectedRule'>Enable</span></b><span class='help-block pull-right'>Rule Test data will be saved to the system only if rule has been saved already. Data in test cases is case sensitive.</span></div><div id='ruleDesc'></div>" +
                                        "<div id='ruleTestertabs' class='parent-container ui-tabs' style='box-shadow:none !important;height: 300px;'>" +

                                            "<ul id='ruleTesttablist' class='nav nav-tabs nav-pills nav-pills-child' role='tablist' style='border-left-style: none; border-right-style: none;'>" +
                                                "<li class='active'>" +
                                                    "<a href='#expressionbuildertab' role='tab' data-toggle='tab' style='border-top-style:none !important;border-left-style:none !important;'>Rule Designs</a>" +
                                                "</li>" +
                                                "<li>" +
                                                    "<a href='#rulestestergridtab' role='tab' data-toggle='tab' style='border-top-style:none !important;' >Rule Tester</a>" +
                                                "</li>" +
	                                        "</ul>" +

                                            "<div role='tabpanel' class='tab-content'>" +
                                                "<div id='expressionbuildertab' class='tab-pane active col-xs-12 col-md-12 col-lg-12 col-sm-12'>" +
                                                    "<div class='rulecontainer'><div id='expressionbuilder'></div></div>" +
                                                "</div>" +

                                                "<div id='rulestestergridtab' class='tab-pane col-xs-12 col-md-12 col-lg-12 col-sm-12' style='float:left;'>" +
                                                    "<div class='grid-wrapper'><table id='rulestestergrid'></table></div>" +
                                                "</div>" +
                                           "</div>" +

                                        "</div>" +
                                        "<div class='row-fluid'>" +
                                            "<div class='col-xs-12 col-md-12 col-lg-12 col-sm-12'></div>" +
                                        "</div>" +
                                        "<div class='row-fluid'>" +
                                            "<div class='col-xs-12 col-md-12 col-lg-12 col-sm-12'>" +
                                                "<div class='grid-wrapper' ><table id='rulesgrid'></table></div>" +
                                                "<span class='help-block'>Changes will be Saved locally. They will be Saved to the system only if the Save button on the Properties Grid is clicked.</span>" +
                                            "</div>" +
                                        "</div>" +
                                "</div>" +
                            "</div>";
    return rulesDialogNewUIHtml;
}

rulesDialogNewUI.prototype.registerTabEvents = function () {
    var currentInstance = this;
    $('#ruleTestertabs a[data-toggle="tab"]').bind('click', function (e) {
        var ruleDialog = currentInstance.currentRuleDialog;
        var result = ruleDialog.validateRulesData();
        if (result == false) {
            messageDialog.show("The rules have some invalid conditions. Please add validate expressions before adding test cases.");
            return result;
        }
        if (ruleDialog.currentExpressionBuilder != undefined) {
            if (currentInstance.testerDialog == undefined || currentInstance.testerDialog == null) {
                currentInstance.testerDialog = new rulesTesterDialog(ruleDialog.uiElement, ruleDialog.formDesignVersionId, ruleDialog.currentExpressionBuilder, ruleDialog.elementGridData, ruleDialog.designRulesTesterData);
            }
            if (e.target.hash == rulesDialogNewUI.elementIDs.expressionbuildertab) {
                currentInstance.testerDialog.saveRow();
                currentInstance.testerDialog.updateDesignRulesTesterData();
                currentInstance.activeTab = 'expressionBuilder';
                var rowId = $(rulesDialog.elementIDs.rulesGridJQ).getGridParam('selrow');
                $(rulesDialog.elementIDs.rulesGridJQ).jqGrid('setSelection', rowId, true);
            } else if (e.target.hash == rulesDialogNewUI.elementIDs.rulestestergridtab) {
                currentInstance.updateCurrentExpressionBuilder();
                currentInstance.activeTab = 'ruleTester';
                currentInstance.testerDialog.loadDesignRulesTesterData();
            }
        }
    });
}

rulesDialogNewUI.prototype.getRuleTesterData = function () {
    var currentInstance = this;
    var designRulesTesterData = currentInstance.currentRuleDialog.designRulesTesterData;
    if (currentInstance.testerDialog != undefined || currentInstance.testerDialog != null) {
        designRulesTesterData = currentInstance.testerDialog.getRuleTesterData();
    }
    return designRulesTesterData;
}

rulesDialogNewUI.prototype.changeCurrentRuleTestData = function () {
    var currentInstance = this;
    currentInstance.testerDialog.saveRow();
    currentInstance.testerDialog.updateDesignRulesTesterData();
    currentInstance.activeTab = 'expressionBuilder';
    currentInstance.updateCurrentExpressionBuilder();
    currentInstance.testerDialog.currentExpressionBuilder = currentInstance.currentRuleDialog.currentExpressionBuilder;
    currentInstance.activeTab = 'ruleTester';
    currentInstance.testerDialog.loadDesignRulesTesterData();
}

rulesDialogNewUI.prototype.updateCurrentExpressionBuilder = function () {
    var currentInstance = this;
    if (currentInstance.currentRuleDialog.currentExpressionBuilder != null) {
        result = currentInstance.currentRuleDialog.currentExpressionBuilder.saveRule();
        if (result == true) {
            currentInstance.currentRuleDialog.currentExpressionBuilder.close();
        }
    }
    var rowId = $(rulesDialog.elementIDs.rulesGridJQ).getGridParam('selrow');
    $(rulesDialog.elementIDs.rulesGridJQ).jqGrid('setSelection', rowId, true);
}
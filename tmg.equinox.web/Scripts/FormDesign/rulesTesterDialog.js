var rulesTesterDialog = function (uiElement, formDesignVersionId, currentExpressionBuilder, elementList, designRulesTesterData, formDesignId) {
    this.tenantId = 1;
    this.uiElement = uiElement;
    this.formDesignVersionId = formDesignVersionId;
    this.elementList = elementList;
    this.currentExpressionBuilder = currentExpressionBuilder;
    this.rulesTestData = [];
    this.currentRuleTestData;
    this.lastSelectedRowId;
    this.KeyProperty = 'RowIDProperty';
    this.designRulesTesterData = designRulesTesterData;
    this.formDesignId = formDesignId;
}

rulesTesterDialog.URLs = {
    exportToExcelJqgrid: '/UIElement/ruleTesterGridExportToExcel',
    getElementsTestData: '/UIElement/getFormDesignVersionUIElementsTestData?tenantId=1&formDesignVersionId={formDesignVersionId}&elementId={elementId}',
    saveConfigRulesTesterData: '/UIElement/SaveConfigRulesTesterData',
    getTestDataFromProduct: '/ExpressionBuilder/GetDataForTest'
}

rulesTesterDialog.elementIDs = {
    rulesTesterGrid: 'rulestestergrid',
    rulesTesterGridJQ: '#rulestestergrid',
}

rulesTesterDialog.constants = {
    testPassText: "Test Pass",
    testFailText: "Test Fail",
}

rulesTesterDialog.TargetPropertyTypes = {
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

rulesTesterDialog.prototype.ruleTestData = function () {
    var ruleId;
    var testData = [];
    var columnModels = [];
}

rulesTesterDialog.prototype.designRuleTesterData = function () {
    var designRuleTesterId;
    var formDesignVersionId;
    var UIElementId;
    var ruleId;
    var testDataJson;
    var isActive;
}

rulesTesterDialog.prototype.setCurrentRuleTestData = function () {
    var currentInstance = this;
    var ruleObj = currentInstance.currentExpressionBuilder.ruleObj;
    var ruleId = ruleObj.RuleId;
    if (currentInstance.rulesTestData.length > 0) {
        var filteredRulesTestData = currentInstance.rulesTestData.filter(function (data) {
            return data.ruleId == ruleId;
        });
        if (filteredRulesTestData.length == 0) {
            if (currentInstance.checkRuleIdAllocated(ruleId)) {
                currentInstance.setCurrentRuleTestData();
                return;
            } else {
                currentInstance.addNewRulesTestData(ruleObj);
            }
        } else {
            if (!currentInstance.checkIfExpressionChanged(filteredRulesTestData[0].columnModels)) {
                var index = filteredRulesTestData[0].columnModels.map(function (ele) { return ele.index; }).indexOf("Test Result");
                filteredRulesTestData[0].columnModels.splice(index, 1);
                filteredRulesTestData[0].columnModels.push({ name: "Test Result", index: "Test Result", editable: false, formatter: currentInstance.processStatusFormmater, classes: '' });
                index = filteredRulesTestData[0].columnModels.map(function (ele) { return ele.index; }).indexOf("Is Product");
                filteredRulesTestData[0].columnModels.splice(index, 1, { name: "Is Product", index: "Is Product", align: 'center', width: 50, editable: false, formatter: currentInstance.formatCheckBoxColumn });
                currentInstance.currentRuleTestData = filteredRulesTestData[0];
            } else {
                var index = currentInstance.rulesTestData.map(function (ele) { return ele.ruleId; }).indexOf(ruleId);
                currentInstance.rulesTestData.splice(index, 1);
                currentInstance.addNewRulesTestData(ruleObj);
            }
        }
    } else {
        currentInstance.addNewRulesTestData(ruleObj);
    }
}

rulesTesterDialog.prototype.checkRuleIdAllocated = function (ruleId) {
    var currentInstance = this;
    var allocated = true;
    var filteredRulesTestData = currentInstance.rulesTestData.filter(function (data) {
        return data.ruleId < 0;
    });
    if (filteredRulesTestData.length > 0) {
        if (!currentInstance.checkIfExpressionChanged(filteredRulesTestData[0].columnModels)) {
            var index = currentInstance.rulesTestData.map(function (ele) { return ele.ruleId; }).indexOf(filteredRulesTestData[0].ruleId);
            currentInstance.rulesTestData.splice(index, 1);
            filteredRulesTestData[0].ruleId = ruleId;
            currentInstance.rulesTestData.push(filteredRulesTestData[0]);
            allocated = true;
        } else {
            allocated = false;
        }
    } else {
        allocated = false;
    }

    return allocated;
}

rulesTesterDialog.prototype.checkIfExpressionChanged = function (colModels) {
    var currentInstance = this;
    var ruleObj = currentInstance.currentExpressionBuilder.ruleObj;
    var result = false;
    var expressions = currentInstance.getExpressions();
    if (expressions != undefined) {
        for (var index = 0; index < expressions.length; index++) {
            var uiElementLabelLO, uiElementLabelRO;

            if (expressions[index].IsRightOperandElement) {
                uiElementLabelLO = currentInstance.getElementLabel(expressions[index].LeftOperand);
                if (!currentInstance.ifColumnExist(colModels, uiElementLabelLO)) { result = true; break; }
                uiElementLabelRO = currentInstance.getElementLabel(expressions[index].RightOperand);
                if (!currentInstance.ifColumnExist(colModels, uiElementLabelRO)) { result = true; break; }
            } else {
                uiElementLabelLO = currentInstance.getElementLabel(expressions[index].LeftOperand);
                if (!currentInstance.ifColumnExist(colModels, uiElementLabelLO)) { result = true; break; }
            }
        }
        if (!result) {
            var filteredColModel = colModels.filter(function (list) {
                return list.hidden == true && list.key != true;
            });
            if (filteredColModel.length != 0) {
                var cols = filteredColModel;
                for (var index = 0; index < cols.length; index++) {
                    var colName = cols[index].name;
                    colName = colName.substr(0, colName.indexOf("#!"))
                    var leftOperandIndex = expressions.map(function (ele) { return ele.LeftOperand; }).indexOf(colName);
                    var rightOperandIndex = expressions.map(function (ele) { return ele.RightOperand; }).indexOf(colName);
                    if (leftOperandIndex < 0 && rightOperandIndex < 0) {
                        result = true;
                        break;
                    }
                }
            }
        }
        if (!result) {
            var filteredColModel = colModels.filter(function (list) {
                return list.classes != "" && list.key != true && list.name != "Is Product";
            });
            if (filteredColModel.length != 0) {
                var cols = filteredColModel;
                var fieldCount = 0;
                if (ruleObj.IsResultSuccessElement) fieldCount++;
                if (ruleObj.IsResultFailureElement) fieldCount++;
                if (cols.length != fieldCount) {
                    result = true;
                } else {
                    for (var index = 0; index < cols.length; index++) {
                        if (cols[index].classes == "ResultSuccessElement") {
                            if (ruleObj.IsResultSuccessElement) {
                                var resultSuccessElementName = currentInstance.getElementLabel(ruleObj.ResultSuccess);
                                if (resultSuccessElementName != cols[index].name) {
                                    result = true;
                                    break;
                                }
                            } else {
                                result = true;
                                break;
                            }
                        }
                        if (cols[index].classes == "ResultFailureElement") {
                            if (ruleObj.IsResultFailureElement) {
                                var resultFailureElementName = currentInstance.getElementLabel(ruleObj.ResultFailure);
                                if (resultFailureElementName != cols[index].name) {
                                    result = true;
                                    break;
                                }
                            } else {
                                result = true;
                                break;
                            }
                        }
                    }
                }
            } else {
                if (ruleObj.IsResultSuccessElement) {
                    result = true;
                }
                if (ruleObj.IsResultFailureElement) {
                    result = true;
                }
            }
        }
    } else {
        result = true;
    }
    return result;
}

rulesTesterDialog.prototype.loadRulesTesterGrid = function (data) {
    var currentInstance = this;
    this.setCurrentRuleTestData();
    var colModel = currentInstance.currentRuleTestData.columnModels;
    if (data) {
        currentInstance.currentRuleTestData.testData.push(data);
    }
    if (colModel.length <= 0) {
        $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('GridUnload');
        return;
    }
    var colArray = [];
    currentInstance.lastSelectedRowId = null;
    for (var index = 0; index < colModel.length; index++) {
        colArray.push(colModel[index].index);
    }

    $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('GridUnload');

    $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).parent().append("<div id='p" + rulesTesterDialog.elementIDs.rulesTesterGrid + "'></div>");

    $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid({
        datatype: 'local',
        data: currentInstance.currentRuleTestData.testData,
        cache: false,
        colNames: colArray,
        colModel: colModel,
        pager: '#p' + rulesTesterDialog.elementIDs.rulesTesterGrid,
        caption: 'Test Cases',
        height: '200px',
        rowheader: true,
        shrinkToFit: true,
        loadonce: true,
        colwidth: '100%',
        viewrecords: true,
        altRows: true,
        altclass: 'alternate',
        hidegrid: false,
        onSelectRow: function (rowId) {
            if (rowId && rowId !== currentInstance.lastSelectedRowId) {
                if (currentInstance.lastSelectedRowId != null) {
                    $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('saveRow', currentInstance.lastSelectedRowId);
                    currentInstance.saveRow();
                }
                var rowData = $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('getRowData', rowId);
                if (!rowData["Is Product"].includes("ok")) {
                    $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('editRow', rowId, true);
                }
                currentInstance.lastSelectedRowId = rowId;
            }
            currentInstance.setCellBackgroundColor();
            return;
        }
    });

    var pagerElement = '#p' + rulesTesterDialog.elementIDs.rulesTesterGrid;

    $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false, refresh: false });
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-plus', title: 'Add Test Case',
            onClickButton: function () {
                var rowId = currentInstance.currentRuleTestData.testData.length > 0 ? (Math.max.apply(Math, currentInstance.currentRuleTestData.testData.map(function (o) { return o.RowIDProperty; })) + 1) : 1;
                var datarow = {};
                for (var idx = 0; idx < currentInstance.currentRuleTestData.columnModels.length; idx++) {
                    if (currentInstance.currentRuleTestData.columnModels[idx].index == "RowIDProperty") {
                        datarow[currentInstance.currentRuleTestData.columnModels[idx].index] = rowId.toString();
                    } else {
                        datarow[currentInstance.currentRuleTestData.columnModels[idx].index] = "";
                    }
                }
                currentInstance.currentRuleTestData.testData.push(datarow);
                $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('addRowData', rowId, datarow);
                $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('setSelection', rowId);
            }
        });

    $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-folder-open', title: 'Select Product',
            onClickButton: function () {
                var res = selectSourceDialog.show(currentInstance, currentInstance.formDesignId);
            }
        });

    $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-trash', title: 'Delete Test Case',
            onClickButton: function () {
                var rowId = $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).getGridParam('selrow');
                if (rowId != null) {
                    $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('delRowData', rowId);
                    var index = currentInstance.currentRuleTestData.testData.map(function (ele) { return ele.RowIDProperty; }).indexOf(rowId);
                    currentInstance.currentRuleTestData.testData.splice(index, 1);
                    currentInstance.lastSelectedRowId = null;
                    var rowIds = $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).getDataIDs();
                    if (rowIds.length > 0) {
                        $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('setSelection', rowIds[0]);
                    } else {
                        currentInstance.saveRow();
                    }
                } else {
                    messageDialog.show("Please select the data row to delete.");
                }
            }
        });

    $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-play', title: 'Execute Rules On Test Cases',
            onClickButton: function () {
                currentInstance.saveRow();
                currentInstance.lastSelectedRowId = null;
                var currentdata = currentInstance.currentRuleTestData;
                var currentRuleObj = currentInstance.currentExpressionBuilder.ruleObj;
                for (var index = 0; index < currentdata.testData.length; index++) {
                    var testerProcessor = new rulesTesterProcessor(currentdata.testData[index]);
                    var result = testerProcessor.processRule(currentRuleObj);
                    currentdata.testData[index] = currentInstance.updateResult(currentRuleObj, currentdata.testData[index], result);
                    $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('setGridParam', { data: currentdata.testData }).trigger('reloadGrid');
                }
                currentInstance.setCellBackgroundColor();
            }
        });

    $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('navButtonAdd', pagerElement,
     {
         caption: '', buttonicon: 'ui-icon-extlink', title: 'Save Test Cases',
         onClickButton: function () {
             if (currentInstance.currentExpressionBuilder.ruleId > 0) {
                 currentInstance.saveRow();
                 currentInstance.updateDesignRulesTesterData();
                 currentInstance.saveDesignRulesTesterData();
                 currentInstance.setCellBackgroundColor();
             } else {
                 messageDialog.show("Please save the rule details, before saving the test data.");
             }
         }
     });

    $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-arrowstop-1-s', title: 'Export To Excel',
            onClickButton: function () {
                currentInstance.saveRow();
                var jqGridtoCsv = new JQGridtoCsv(rulesTesterDialog.elementIDs.rulesTesterGridJQ, false, currentInstance);
                jqGridtoCsv.buildExportOptions();
                var stringData = "csv=" + jqGridtoCsv.csvData;
                stringData += "<&header=" + currentInstance.uiElement.Label == undefined ? "" : currentInstance.uiElement.Label + ": Rule For " + currentInstance.currentExpressionBuilder.ruleObj.TargetProperty + " (Test Data)";
                stringData += "<&noOfColInGroup=" + jqGridtoCsv.noofGroupedColumns;
                stringData += "<&repeaterName=" + rulesTesterDialog.elementIDs.rulesTesterGrid;

                $.download(rulesTesterDialog.URLs.exportToExcelJqgrid, stringData, 'post');
                currentInstance.setCellBackgroundColor();
            }
        });



}

rulesTesterDialog.prototype.setSourceDocumentValues = function (sourceDocument) {
    var currentInstance = this;
    if (sourceDocument.FormInstanceID != null && sourceDocument.FormInstanceID != undefined) {
        currentInstance.getProductData(sourceDocument.FormInstanceID);
    }
}

rulesTesterDialog.prototype.updateResult = function (ruleObj, testData, result) {
    var currentInstance = this;
    var testData;
    switch (parseInt(ruleObj.TargetPropertyId)) {
        case rulesTesterDialog.TargetPropertyTypes.Value:
            testData = currentInstance.UpdateResultForValueRule(result, ruleObj, testData);
            break;
        default:
            testData = currentInstance.UpdateResultForOtherRule(result, ruleObj, testData);
    }
    return testData;
}

rulesTesterDialog.prototype.UpdateResultForOtherRule = function (result, ruleObj, testData) {
    var currentInstance = this;
    var testData;
    var targetPropertyName = ruleObj.TargetProperty == undefined ? currentInstance.getTargetProperty(ruleObj.TargetPropertyId) : ruleObj.TargetProperty;
    if (result) {
        testData["Actual Result"] = targetPropertyName;
    } else {
        testData["Actual Result"] = "Not-" + targetPropertyName;
    }

    if (testData["Is Product"] != "yes") {
        if (testData["Actual Result"] == testData["Expected Result"]) {
            testData["Test Result"] = rulesTesterDialog.constants.testPassText;
        } else {
            testData["Test Result"] = rulesTesterDialog.constants.testFailText;
        }
    }

    return testData;
}

rulesTesterDialog.prototype.getTargetProperty = function (targetPropertyId) {
    var propertyName;
    switch (parseInt(targetPropertyId)) {
        case 1:
            propertyName = 'Enabled';
            break;
        case 2:
            propertyName = 'Run Validation';
            break;
        case 3:
            propertyName = 'Value';
            break;
        case 4:
            propertyName = 'Visible';
            break;
        case 5:
            propertyName = 'Is Required';
            break;
        case 6:
            propertyName = 'Error';
            break;
        case 8:
            propertyName = 'Highlight';
            break;
        case 9:
            propertyName = 'Dialog';
            break;
        case 10:
            propertyName = 'Custom Rule';
            break;
    }
    return propertyName;
}

rulesTesterDialog.prototype.UpdateResultForValueRule = function (result, ruleObj, testData) {
    var currentInstance = this;
    var testData;
    if (result) {
        if (ruleObj.IsResultSuccessElement) {
            var resultSuccessElementName = currentInstance.getElementLabel(ruleObj.ResultSuccess);
            testData["Actual Result"] = testData[resultSuccessElementName];
        } else {
            testData["Actual Result"] = ruleObj.ResultSuccess;
        }
    } else {
        if (ruleObj.IsResultFailureElement) {
            var resultFailureElementName = currentInstance.getElementLabel(ruleObj.ResultFailure);
            testData["Actual Result"] = testData[resultFailureElementName];
        } else {
            testData["Actual Result"] = ruleObj.ResultFailure;
        }
    }
    if (testData["Is Product"] != "yes") {
        if (testData["Actual Result"] == testData["Expected Result"]) {
            testData["Test Result"] = rulesTesterDialog.constants.testPassText;
        } else {
            testData["Test Result"] = rulesTesterDialog.constants.testFailText;
        }
    }

    return testData;
}

rulesTesterDialog.prototype.setCursorOnEditRow = function (rowId, columnIndex) {
    var ind = $(this.gridElementIdJQ).jqGrid("getInd", rowId, true);
    var child = $(" td:eq(" + columnIndex + ")", ind).children().first();
    if (child.length > 0) {
        $(child).focus();
    }
    else {
        for (var i = columnIndex + 1; i < ind.cells.length; i++) {
            var child = $(" td:eq(" + (i) + ")", ind).children().first();
            if (child.length > 0) {
                $(child).focus();
                return false;
            }
        }
    }
}

rulesTesterDialog.prototype.saveRow = function () {
    var currentInstance = this;
    var currentdata = this.currentRuleTestData;
    var saveParameters = {
        "url": 'clientArray',
        "aftersavefunc": function () {
            currentInstance.bindData();
        }
    };
    try {
        if (currentInstance.lastSelectedRowId != null) {
            $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('saveRow', currentInstance.lastSelectedRowId, saveParameters);
        }
    } catch (ex) { }

    var rowData = $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).getRowData(currentInstance.lastSelectedRowId);

    if (currentInstance.lastSelectedRowId != null) {
        var row = currentdata.testData.filter(function (dt) {
            return dt.RowIDProperty == currentInstance.lastSelectedRowId;
        });
        if (row.length == 0) {
            row = {};
            row.RowIDProperty = rowData.RowIDProperty;
            currentdata.testData.push(row);
        }
        else {
            row = row[0];
        }
        for (var prop in rowData) {
            if (prop != this.KeyProperty) {
                var value = rowData[prop];
                if (!prop.includes("#!")) {
                    if (value != "") {
                        if (prop == "Is Product") {
                            row[prop] = value.includes("glyphicon-ok") ? "yes" : "no";
                        }
                        else {
                            row[prop] = value;
                            var sameoprand = "#!" + prop + "!#";
                            for (var prop1 in rowData) {
                                if (prop1.includes(sameoprand)) {
                                    row[prop1] = value;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    var filteredRulesTestData = this.rulesTestData.filter(function (data) {
        return data.ruleId == currentdata.ruleId;
    });

    if (currentdata.testData.length > 0) {
        var filteredtestData = currentdata.testData.filter(function (data) {
            return data.RowIDProperty != undefined;
        });

        currentdata.testData = filteredtestData;
    }

    if (filteredRulesTestData.length == 0) {
        this.rulesTestData.push(currentdata)
    } else {
        var index = this.rulesTestData.map(function (ele) { return ele.ruleId; }).indexOf(currentdata.ruleId);
        this.rulesTestData.splice(index, 1);
        this.rulesTestData.push(currentdata)
    }
    $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('clearGridData');
    $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('setGridParam', { data: currentdata.testData }).trigger('reloadGrid');

}

rulesTesterDialog.prototype.addNewRulesTestData = function (ruleObj) {
    var objRuleTestData = new this.ruleTestData();
    objRuleTestData.ruleId = ruleObj.RuleId;
    objRuleTestData.testData = [];
    objRuleTestData.columnModels = this.prepareColModel();
    this.rulesTestData.push(objRuleTestData);
    this.currentRuleTestData = objRuleTestData;
}

rulesTesterDialog.prototype.prepareColModel = function () {
    var currentInstance = this;
    var colModel = [];
    var expressions = currentInstance.getExpressions();
    var ruleObj = currentInstance.currentExpressionBuilder.ruleObj;
    if (expressions != undefined) {
        colModel.push({ name: "RowIDProperty", index: "RowIDProperty", key: true, hidden: true });
        colModel.push({ name: "Is Product", index: "Is Product", align: 'center', width: 50, editable: false, formatter: currentInstance.formatCheckBoxColumn });
        if (expressions != null || expressions.length > 0) {
            for (var index = 0; index < expressions.length; index++) {
                var uiElementLabelLO, uiElementLO, uiElementLabelRO, uiElementRO;

                if (expressions[index].IsRightOperandElement) {
                    uiElementLabelLO = currentInstance.getElementLabel(expressions[index].LeftOperand);
                    uiElementLabelRO = currentInstance.getElementLabel(expressions[index].RightOperand);
                    uiElementLO = expressions[index].LeftOperand + "#!" + uiElementLabelLO + "!#";
                    uiElementRO = expressions[index].RightOperand + "#!" + uiElementLabelRO + "!#";
                    if (!currentInstance.ifColumnExist(colModel, uiElementLabelLO)) {
                        colModel.push({ name: uiElementLabelLO, index: uiElementLabelLO, editable: true, classes: '' });
                        colModel.push({ name: uiElementLO, index: uiElementLO, editable: false, hidden: true, classes: '' });
                    }
                    if (!currentInstance.ifColumnExist(colModel, uiElementLabelRO)) {
                        colModel.push({ name: uiElementLabelRO, index: uiElementLabelRO, editable: true, classes: '' });
                        colModel.push({ name: uiElementRO, index: uiElementRO, editable: false, hidden: true, classes: '' });
                    }
                } else {
                    uiElementLabelLO = currentInstance.getElementLabel(expressions[index].LeftOperand);
                    uiElementLO = expressions[index].LeftOperand + "#!" + uiElementLabelLO + "!#";
                    if (!currentInstance.ifColumnExist(colModel, uiElementLabelLO)) {
                        colModel.push({ name: uiElementLabelLO, index: uiElementLabelLO, editable: true, classes: '' });
                        colModel.push({ name: uiElementLO, index: uiElementLO, editable: false, hidden: true, classes: '' });
                    }
                }
            }
            if (parseInt(ruleObj.TargetPropertyId) != rulesTesterDialog.TargetPropertyTypes.Value) {
                var targetPropertyName = ruleObj.TargetProperty == undefined ? currentInstance.getTargetProperty(ruleObj.TargetPropertyId) : ruleObj.TargetProperty;
                optionText = targetPropertyName + ":" + targetPropertyName + ";" + "Not-" + targetPropertyName + ":" + "Not-" + targetPropertyName + ";N/A:N/A";
                colModel.push({ name: "Expected Result", index: "Expected Result", editable: true, classes: '', formatter: 'select', edittype: 'select', editoptions: { value: optionText } });
            } else {
                if (ruleObj.IsResultSuccessElement) {
                    var resultSuccessElementName = currentInstance.getElementLabel(ruleObj.ResultSuccess);
                    colModel.push({ name: resultSuccessElementName, index: resultSuccessElementName, editable: true, classes: 'ResultSuccessElement' });
                }
                if (ruleObj.IsResultFailureElement) {
                    var resultFailureElementName = currentInstance.getElementLabel(ruleObj.ResultFailure);
                    colModel.push({ name: resultFailureElementName, index: resultFailureElementName, editable: true, classes: 'ResultFailureElement' });
                }
                colModel.push({ name: "Expected Result", index: "Expected Result", editable: true, classes: '' });
            }
            colModel.push({ name: "Actual Result", index: "Actual Result", editable: false, classes: '' });
            colModel.push({ name: "Test Result", index: "Test Result", editable: false, formatter: currentInstance.processStatusFormmater, classes: '' });
        }
    }
    return colModel;
}

rulesTesterDialog.prototype.formatCheckBoxColumn = function (cellValue, options, rowObject) {
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

rulesTesterDialog.prototype.processStatusFormmater = function (cellvalue, options, rowObject) {
    switch (cellvalue) {
        case rulesTesterDialog.constants.testFailText:
            cellvalue = '<span style="color:red"><b>' + rulesTesterDialog.constants.testFailText + '</b></span>';
            break;
        case rulesTesterDialog.constants.testPassText:
            cellvalue = '<span style="color:green">' + rulesTesterDialog.constants.testPassText + '</span>';
            break;
    }
    return cellvalue;
}

rulesTesterDialog.prototype.ifColumnExist = function (colModel, element) {
    var index = colModel.map(function (ele) { return ele.name; }).indexOf(element);
    if (index >= 0) {
        return true;
    } else {
        return false;
    }
}

rulesTesterDialog.prototype.getElementLabel = function (element) {
    var currentinstance = this;
    var UIElementList = currentinstance.elementList;
    var filteredElementlist = UIElementList.filter(function (elementlist) {
        return elementlist.UIElementName == element;
    });

    uiElementLabel = element;
    if (filteredElementlist.length != 0) {
        if (filteredElementlist[0].Label) {
            uiElementLabel = filteredElementlist[0].Label;
        }
        else if (filteredElementlist[0].Element) {
            uiElementLabel = filteredElementlist[0].Element;
        }
    }
    return uiElementLabel;
}

rulesTesterDialog.prototype.getExpressions = function () {
    var currentinstance = this;
    var currentExpressionBuilder = currentinstance.currentExpressionBuilder;
    var expressions
    if (currentExpressionBuilder.expressionsList.length > 1) {
        expressions = currentExpressionBuilder.expressionsList;
    }

    if (expressions != undefined) {
        var filteredRules = expressions.filter(function (list) {
            return list.LeftOperand != "" && list.LeftOperand != null;
        });
        if (filteredRules.length != 0) {
            expressions = filteredRules;
        }
    }
    return expressions;
}

rulesTesterDialog.prototype.loadDesignRulesTesterData = function () {
    var currentInstance = this;
    currentInstance.rulesTestData = [];
    var uiElementId = currentInstance.uiElement.UIElementID;

    if (currentInstance.designRulesTesterData.length > 0) {
        var filteredData = currentInstance.designRulesTesterData.filter(function (data) {
            return data.UIElementId == uiElementId;
        });
        if (filteredData.length != 0) {
            for (var indx = 0; indx < filteredData.length; indx++) {
                var testdata = Array.isArray(filteredData[indx].testDataJson) ? filteredData[indx].testDataJson : JSON.parse(filteredData[indx].testDataJson);
                if (!Array.isArray(testdata)) currentInstance.rulesTestData.push(testdata);
            }
        } else {
            currentInstance.loadDesignRulesTesterDataFromServer();
        }
    } else {
        currentInstance.loadDesignRulesTesterDataFromServer();
    }
    currentInstance.loadRulesTesterGrid();
    currentInstance.setCellBackgroundColor();
}

rulesTesterDialog.prototype.loadDesignRulesTesterDataFromServer = function () {
    var currentInstance = this;
    var uiElementId = currentInstance.uiElement.UIElementID;

    var url = rulesTesterDialog.URLs.getElementsTestData.replace(/\{tenantId\}/g, this.tenantId).replace(/\{formDesignVersionId\}/g, currentInstance.formDesignVersionId).replace(/\{elementId\}/g, uiElementId);
    var promise = ajaxWrapper.getJSONSync(url);
    promise.done(function (result) {
        if (result != null && result.length > 0) {
            for (var index = 0; index < result.length; index++) {
                if (currentInstance.rulesTestData == undefined) {
                    currentInstance.rulesTestData = [];
                }
                currentInstance.rulesTestData.push(JSON.parse(result[index].TestData));

                if (currentInstance.designRulesTesterData.find(o => o.designRuleTesterId === result[index].RuleTersterId) == undefined) {
                    var objDesignRuleTesterData = new currentInstance.designRuleTesterData();
                    objDesignRuleTesterData.designRuleTesterId = result[index].RuleTersterId;
                    objDesignRuleTesterData.formDesignVersionId = result[index].FormDesignVersionId;
                    objDesignRuleTesterData.UIElementId = result[index].UIElementId;
                    objDesignRuleTesterData.ruleId = result[index].RuleId;
                    objDesignRuleTesterData.testDataJson = result[index].TestData;
                    objDesignRuleTesterData.isActive = result[index].IsActive;
                    currentInstance.designRulesTesterData.push(objDesignRuleTesterData);
                }
            }
        }
    });
}

rulesTesterDialog.prototype.updateDesignRulesTesterData = function () {
    var currentInstance = this;
    var uiElementId = currentInstance.uiElement.UIElementID;
    var formDesignVersionId = currentInstance.formDesignVersionId;
    var designRulesTesterData = currentInstance.designRulesTesterData;

    for (var index = 0; index < currentInstance.rulesTestData.length; index++) {
        var id = currentInstance.rulesTestData[index].ruleId;
        var filteredData = designRulesTesterData.filter(function (data) {
            return data.UIElementId == uiElementId && data.ruleId == id;
        });
        if (filteredData.length != 0) {
            for (var indx = 0; indx < designRulesTesterData.length; indx++) {
                if (designRulesTesterData[indx].UIElementId == uiElementId && designRulesTesterData[indx].ruleId == id) {
                    designRulesTesterData[indx].testDataJson = JSON.stringify(currentInstance.rulesTestData[index]);
                    if (currentInstance.rulesTestData[index].testData != undefined && currentInstance.rulesTestData[index].testData.length <= 0) {
                        designRulesTesterData[indx].isActive = "0";
                    }
                    break;
                }
            }
        } else {
            var objDesignRuleTesterData = new this.designRuleTesterData();
            objDesignRuleTesterData.designRuleTesterId = -1;
            objDesignRuleTesterData.formDesignVersionId = parseInt(formDesignVersionId);
            objDesignRuleTesterData.UIElementId = parseInt(uiElementId);
            objDesignRuleTesterData.ruleId = parseInt(id);
            if (currentInstance.rulesTestData[index].testData != undefined && currentInstance.rulesTestData[index].testData.length > 0) {
                objDesignRuleTesterData.testDataJson = JSON.stringify(currentInstance.rulesTestData[index]);
                objDesignRuleTesterData.isActive = "1";
            } else {
                objDesignRuleTesterData.testDataJson = [];
                objDesignRuleTesterData.isActive = "0";
            }
            designRulesTesterData.push(objDesignRuleTesterData);
        }
    }
}

rulesTesterDialog.prototype.saveDesignRulesTesterData = function () {
    var currentInstance = this;
    var ruleId = currentInstance.currentExpressionBuilder.ruleId;
    var designRulesTesterData = currentInstance.designRulesTesterData.filter(function (data) { return data.ruleId == ruleId });

    var configRulesTesterData = {
        tenantId: currentInstance.tenantId,
        designRulesTesterData: JSON.stringify(designRulesTesterData)
    }

    var url = rulesTesterDialog.URLs.saveConfigRulesTesterData;
    var promise = ajaxWrapper.postJSON(url, configRulesTesterData);

    promise.done(function (xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            if (xhr.Items.length > 0) {
                var index = currentInstance.designRulesTesterData.map(function (ele) { return ele.ruleId; }).indexOf(ruleId);
                currentInstance.designRulesTesterData[index].designRuleTesterId = parseInt(xhr.Items[0].Messages[0]);
            }
            //messageDialog.show("Config rule test data saved successfully.");
        }
        else {
            //messageDialog.show("Config rule test data not saved.");
        }
    });
}

rulesTesterDialog.prototype.getRuleTesterData = function () {
    var currentInstance = this;
    currentInstance.saveRow();
    currentInstance.updateDesignRulesTesterData();
    return currentInstance.designRulesTesterData;
}

rulesTesterDialog.prototype.getProductData = function (formInstanceId) {
    var currentInstance = this;
    var postData = {
        tenantId: currentInstance.tenantId,
        colModel: JSON.stringify($(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('getGridParam', 'colModel')),
        formInstanceId: formInstanceId,
        formDesignVersionId: currentInstance.formDesignVersionId
    };
    var url = rulesTesterDialog.URLs.getTestDataFromProduct;
    var promise = ajaxWrapper.postJSON(url, postData);
    promise.done(function (result) {
        var rowId = currentInstance.currentRuleTestData.testData.length > 0 ? (Math.max.apply(Math, currentInstance.currentRuleTestData.testData.map(function (o) { return o.RowIDProperty; })) + 1) : 1;
        var columnModels = $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('getGridParam', 'colModel');
        var datarow = {};
        for (var idx = 0; idx < columnModels.length; idx++) {
            if (columnModels[idx].name == "RowIDProperty") {
                datarow[columnModels[idx].name] = rowId.toString();
            } else if (columnModels[idx].name == "Is Product") {
                datarow[columnModels[idx].name] = "yes";
            } else if (columnModels[idx].name == "Test Result" || columnModels[idx].name == "Expected Result") {
                datarow[columnModels[idx].name] = "N/A";
            } else {
                var colName = columnModels[idx].name;
                datarow[colName] = currentInstance.getOperandValue(result, colName);
            }
        }
        currentInstance.loadRulesTesterGrid(datarow);
        currentInstance.setCellBackgroundColor();
    });
}
rulesTesterDialog.prototype.setCellBackgroundColor = function () {
    var gridData = $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('getGridParam', 'data');
    var columnModels = $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('getGridParam', 'colModel');
    if (gridData.length > 0) {
        for (var rowId = 1; rowId <= gridData.length; rowId++) {
            var rowData = $(rulesTesterDialog.elementIDs.rulesTesterGridJQ).jqGrid('getRowData', rowId);
            if (rowData["Is Product"].includes("ok")) {
                for (var idx = 0; idx < columnModels.length; idx++) {
                    if (columnModels[idx].name == "Test Result" || columnModels[idx].name == "Expected Result") {
                        jQuery(rulesTesterDialog.elementIDs.rulesTesterGridJQ).setCell(rowId.toString(), columnModels[idx].name, '', { 'background-color': '#eeeeee' }, '');
                    }
                }
            }
        }
    }
}


rulesTesterDialog.prototype.getOperandValue = function (data, colName) {
    var operandValue = "";
    $.each(data, function (idx, row) {
        if (colName.indexOf(row.UIElementName) >= 0) {
            operandValue = row.Value;
        }
        if (colName.indexOf(row.Label) >= 0) {
            operandValue = row.Value;
        }
    });

    return operandValue;
}
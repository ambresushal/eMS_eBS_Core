var expressionBuilder = function () {
    this.expressionBuilderExtension = new expressionBuilderExtension();
}

expressionBuilder.prototype.hasExpressionBuilderRule = function (formDesignId) {
    var result = false;
    return result = this.expressionBuilderExtension.hasExpressionBuilderRule(formDesignId);
}

expressionBuilder.prototype.hasCustomRuleColumn = function (formDesignId,repeaterFullName,columnName) {
    var result = false;
    return result = this.expressionBuilderExtension.hasCustomRuleColumn(formDesignId, repeaterFullName, columnName);
}

expressionBuilder.prototype.sectionElementOnChange = function (currentInstance, elementPath, value) {
    var currentInstance = currentInstance;
    var value = value;
    this.expressionBuilderExtension.sectionElementOnChange(currentInstance, elementPath, value);
}

expressionBuilder.prototype.onSectionLoad = function (currentInstance) {
    var currentInstance = currentInstance;
    this.expressionBuilderExtension.onSectionLoad(currentInstance);
}

expressionBuilder.prototype.repeaterElementOnChange = function (currentInstance, element, rowId,elementValue, isEnterUniqueResponse,oldValue,event) {
    var fullName = currentInstance.fullName;
    var formInstanceId = currentInstance.formInstanceId;
    this.expressionBuilderExtension.repeaterElementOnChange(currentInstance, element, rowId ,elementValue, isEnterUniqueResponse,oldValue,event);
}

expressionBuilder.prototype.repeaterElementCellSave = function (currentInstance, element, rowId) {
    this.expressionBuilderExtension.repeaterElementCellSave(currentInstance, element, rowId);
}

expressionBuilder.prototype.repeaterBeforeRowAdded = function (currentInstance, newRow) {
    this.expressionBuilderExtension.repeaterBeforeRowAdded(currentInstance, newRow);
}

expressionBuilder.prototype.hideAddButtonforRepeater = function (repeaterName) {
    var result = false;
    result = this.expressionBuilderExtension.hideAddButtonforRepeater(repeaterName);
    return result;
}

expressionBuilder.prototype.hideMinusButtonforRepeater = function (repeaterName) {
    var result = false;
    result = this.expressionBuilderExtension.hideMinusButtonforRepeater(repeaterName);
    return result;
}

expressionBuilder.prototype.hideCopyButtonforRepeater = function (repeaterName) {
    var result = false;
    result = this.expressionBuilderExtension.hideCopyButtonforRepeater(repeaterName);
    return result;
}

expressionBuilder.prototype.registerEventForButtonInRepeater = function (currentInstance) {
    this.expressionBuilderExtension.registerEventForButtonInRepeater(currentInstance);
}

expressionBuilder.prototype.manualdataSourceSaveButtonClick = function (currentInstance, data, oldData,sourceData) {
    this.expressionBuilderExtension.manualdataSourceSaveButtonClick(currentInstance, data, oldData, sourceData);
}

expressionBuilder.prototype.registerControlAndEventForSection = function (formInstancebuilder) {
    this.expressionBuilderExtension.registerControlAndEventForSection(formInstancebuilder);
}   

expressionBuilder.prototype.setDefaultValueForPlatformField = function (formInstancebuilder) {
    this.expressionBuilderExtension.setDefaultValueForPlatformField(formInstancebuilder);
}

expressionBuilder.prototype.getFormatterForGrid = function (colMod, ui, fullName) {
    return this.expressionBuilderExtension.getFormatterForGrid(colMod, ui, fullName);
}

expressionBuilder.prototype.handleBlankRowForCoverageLevel = function (currentInstance) {
    return this.expressionBuilderExtension.handleBlankRowForCoverageLevel(currentInstance);
}

expressionBuilder.prototype.addActivityLogPQ = function (currentInstance, rowData, buValueArray, buFilterCriteria) {
    var colCount = 0;
    if (buValueArray == undefined) {
        //repeater non bulk-update case
        for (var prop in rowData) {
            var type = typeof rowData[prop];
            if (type !== "object" && prop !== "pq_rowselect" && prop !== "pq_cellselect") {
                if (currentInstance.rowDataBeforeEdit[prop] != rowData[prop]) {
                    var oldValue = currentInstance.rowDataBeforeEdit[prop] == undefined ? "" : currentInstance.rowDataBeforeEdit[prop];
                    var storeOld = oldValue;
                    var newValue = rowData[prop];
                    var storeNew = newValue;
                    var colNames = $(currentInstance.gridElementIdJQ).pqGrid("getColModel");
                    //var colName = colNames[colCount].dataIndx;
                    var colName = prop;
                    if (prop.substring(0, 4) == 'INL_') {
                        var propArr = prop.split('_');
                        if (propArr.length == 4) {
                            colName = propArr[3];
                        }
                    }
                    var type = colNames[colCount].editor.type;
                    if (type == 'select') {
                        var itemLen = colNames[colCount].editor.options.length;
                        if (itemLen > 0) {
                            for (i = 0; i < itemLen; i++) {
                                if (colNames[colCount].editor.options[i][storeOld] != undefined) {
                                    oldValue = colNames[colCount].editor.options[i][storeOld];
                                }
                                if (colNames[colCount].editor.options[i][storeNew] != undefined) {
                                    newValue = colNames[colCount].editor.options[i][storeNew];

                                }
                            }

                        }
                    }
                    currentInstance.addEntryToAcitivityLogger(currentInstance.lastSelectedRow, Operation.UPDATE, colName, oldValue, newValue, undefined, undefined, prop);
                }
            }
            colCount++;
        }
    }
    else {
        for (var prop in rowData) {
            var dsName, propIdx, propName = null;
            var oldDataValue = null;
            if (currentInstance.design.RepeaterType == 'child') {
                if (prop.substring(0, 4) == 'INL_') {
                    var propArr = prop.split('_');
                    if (propArr.length == 4) {
                        dsName = propArr[1];
                        propIdx = propArr[2];
                        propName = propArr[3];
                    }
                    if (currentInstance.data.length < currentInstance.lastSelectedRow) {
                        oldDataValue = '';
                    }
                    else {
                        oldDataValue = currentInstance.data[currentInstance.lastSelectedRow][dsName][propIdx][propName];
                    }
                }
                else
                    if (currentInstance.data.length < currentInstance.lastSelectedRow) {
                        oldDataValue = '';
                    }
                    else {
                        oldDataValue = currentInstance.data[currentInstance.lastSelectedRow][prop];
                    }
            }
            else
                if (currentInstance.data.length < currentInstance.lastSelectedRow) {
                    oldDataValue = '';
                }
                else {
                    oldDataValue = currentInstance.data[currentInstance.lastSelectedRow][prop];
                }

            if (oldDataValue != rowData[prop]) {
                if (buValueArray.hasOwnProperty(prop)) {
                    var oldValue = oldDataValue;
                    var newValue = buValueArray[prop];
                    propName = propName == null ? prop : propName;
                    //var colName = currentInstance.columnNames.filter(function (dt) {
                    //    return dt.replace('<font color=red>*</font>', '').replace(/ /g, '').trim() == propName;
                    //});
                    //colName = colName[0];
                    colName = propName;
                    currentInstance.addEntryToAcitivityLogger(currentInstance.lastSelectedRow, Operation.UPDATE, colName, oldValue, newValue, buFilterCriteria, undefined, prop);
                }
            }
        }
    }
    currentInstance.formInstanceBuilder.hasChanges = true;
    currentInstance.hasChanges = true;
}

expressionBuilder.prototype.onCellClick = function (currentInstance, ui) {
    return this.expressionBuilderExtension.onCellClick(currentInstance, ui);
}

expressionBuilder.prototype.checkMLCascade = function (folderType, folderName) {
    if (folderType == 'MasterList' && (folderName == 'Premiums' || folderName == 'Formulary Information' || folderName == 'Prescription')) {
        return true;
    }
    else {
        return false;
    }
}

expressionBuilder.prototype.OpenFileUploadDialog = function (currentInstance, operationType, selectedRow) {
    this.expressionBuilderExtension.OpenFileUploadDialog(currentInstance, operationType, selectedRow);
}

expressionBuilder.prototype.DeleteProofingDocument = function (currentInstance) {
    this.expressionBuilderExtension.DeleteProofingDocument(currentInstance);
}
//VBID PreICL/GAP Repeater Cell event for Repeater Visible
expressionBuilder.prototype.repeaterVisibleOnElementChange = function (currentInstance, element, newValue, isSectionLoad) {
    this.expressionBuilderExtension.repeaterVisibleOnElementChange(currentInstance, element, newValue, isSectionLoad);
}

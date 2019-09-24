var ruleExecutionLogger = function () {
    var isLoggingActive = true;
    var isCurrentRuleExecution = true;
    this.formInstanceId = null;
    this.formName = null;
    this.currentSessionID = null;
    this.ruleExecutionLogFormInstanceData = new Object();
    this.ruleExecutionLogData = new Object();

    var elementIDs = {
        ruleExecutionLog: "ruleExecutionLog",
        ruleExecutionLogJQ: "#ruleExecutionLog",
        ruleExecutionLogGrid: "ruleExecutionLogGrid",
        ruleExecutionLogGridJQ: "#ruleExecutionLogGrid",
        btnRuleExecutionLog: "#btnRuleExecutionLog",
        headerRuleExecutionLog: "#headerRuleExecutionLog",

    }

    var URLs = {
        saveRuleExecutionLogDataURL: '/FormInstanceRuleExecutionLog/SaveFormInstanceRuleExecutionLogData',
        getRuleExecutionLogDataURL: '/FormInstanceRuleExecutionLog/GetRuleExecutionLogData?formInstnaceId={formInstnaceId}&parentRowID={parentRowID}&isParentData={isParentData}&sessionId={sessionId}',
        getRuleExecutionServerLogDataURL: '/FormInstanceRuleExecutionLog/GetRuleExecutionServerLogData?formInstnaceId={formInstnaceId}&sessionID={sessionID}&parentRowID={parentRowID}&isParentData={isParentData}',
        getRuleDescription: '/FormInstanceRuleExecutionLog/GetRuleDescription?ruleID={ruleID}',
        exportToExcel: '/FormInstanceRuleExecutionLog/ExportToExcel',
        getCurrentSessionID: '/FormInstanceRuleExecutionLog/GetCurrentHTTPSessionID',
    }

    var targetPropertyTypes = {
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

    function activateLogging(activate) {
        isLoggingActive = activate;
    }

    function init() {
        if (isLoggingActive) {
            loadRuleExecutionGrid(false);
        }
    }

    function getFormInstanceId(currentFormInstanceId) {
        this.formInstanceId = currentFormInstanceId;
    }

    function getFormName(formName) {
        this.formName = formName;
    }

    function getCurrentSessionID() {
        var promise = ajaxWrapper.getJSONSync(URLs.getCurrentSessionID);
        promise.done(function (result) {
            if (result != null) {
                currentSessionID = result;
            } else {
                messageDialog.show("Error occured while getting currrent session");
            }
        });
    }

    function loadRuleExecutionLogGrid(currentInstance) {
        $(elementIDs.btnRuleExecutionLog).text("View Rule Execution History");
        $(elementIDs.headerRuleExecutionLog).text("Current Rule Execution Log");
        $(elementIDs.headerRuleExecutionLog).append(' - <b>' + this.formName + '</b>');
        $(elementIDs.ruleExecutionLogGridJQ).jqGrid('clearGridData');
        $(elementIDs.ruleExecutionLogGridJQ).parent().append("<div id='p" + elementIDs.ruleExecutionLogGridJQ + "'></div>");
        fillRuleExecutionLogFormInstanceData(currentInstance);
        if (this.ruleExecutionLogFormInstanceData[formInstanceId] != undefined) {
            var log = {};
            log.page = 1;
            log.rows = new Array();
            if (!$.isEmptyObject(ruleExecutionLogFormInstanceData[parseInt(formInstanceId)])) {
                var ruleExecutionLogdata = ruleExecutionLogFormInstanceData[parseInt(formInstanceId)].filter(function (ct) {
                    return ct.IsNewRecord == true && ct.IsParentRow == true;
                });
                if (ruleExecutionLogdata != undefined) {
                    ruleExecutionLogdata = ruleExecutionLogdata.reverse();
                    var rowNum = $(elementIDs.ruleExecutionLogGridJQ).getGridParam('rowNum');
                    log.rows = ruleExecutionLogdata.slice((log.page * rowNum) - rowNum, log.page * rowNum);
                    log.records = ruleExecutionLogdata.length;
                    var totalPages = Math.ceil((log.records / rowNum));
                    log.total = totalPages;
                }
                var gridRows = $(elementIDs.ruleExecutionLogGridJQ)[0];
                gridRows.addJSONData(log);
            }
        }
        isCurrentRuleExecution = true;
    }

    function loadRuleExecutionGrid(viewRuleExecutionLogFlag) {
        var currentInstance = this;
        var loadDataGrid = undefined;
        if (this.ruleExecutionLogFormInstanceData != undefined) {
            if (this.ruleExecutionLogFormInstanceData[formInstanceId]) {
                var loadDataGrid = this.ruleExecutionLogFormInstanceData[formInstanceId].filter(function (ct) {
                    return ct.IsNewRecord == true && ct.IsParentRow == true;
                });
            }
        }
        $(elementIDs.ruleExecutionLogGridJQ).jqGrid('GridUnload');
        var pagerElement = "#p_" + elementIDs.ruleExecutionLogGrid;
        var url = "";
        var dataType;
        if (viewRuleExecutionLogFlag) {
            url = URLs.getRuleExecutionLogDataURL.replace(/{formInstnaceId}/g, this.formInstanceId).replace(/{parentRowID}/g, -1).replace(/{isParentData}/g, true).replace(/{sessionId}/g, '');
            dataType = "json";
            isCurrentRuleExecution = false;
        }
        else {
            dataType = "local";
            isCurrentRuleExecution = true;
            sortable: true;
        }
        $(elementIDs.ruleExecutionLogGridJQ).parent().append("<div id='p_" + elementIDs.ruleExecutionLogGrid + "'></div>");
        $(elementIDs.ruleExecutionLogGridJQ).jqGrid({
            url: url,
            datatype: dataType,
            rowList: [10, 20, 30],
            data: loadDataGrid,
            colNames: ['ID', 'Source Element Path', 'Field', 'On Event', 'Old Value', 'New Value', 'Updated By', 'Updated Date', 'ParentRowID', 'SessionID'],
            colModel: [{ name: 'ID', index: 'ID', key: true, hidden: true },
                       { name: 'ElementPath', index: 'ElementPath', width: '400' },
                       { name: 'ElementLabel', index: 'ElementLabel', width: '400' },
                       { name: 'OnEvent', index: 'OnEvent', width: '200' },
                       { name: 'OldValue', index: 'OldValue', width: '300', formatter: descriptionColumnFormatter },
                       { name: 'NewValue', index: 'NewValue', width: '300', formatter: descriptionColumnFormatter },
                       { name: 'UpdatedBy', index: 'UpdatedBy', width: '200', formatter: updatedByColumnFormatter },
                       { name: 'UpdatedDate', index: 'UpdatedDate', width: '200', formatter: dateTimeFormatter, formatoptions: JQGridSettings.DateTimeFormatterOptions, align: 'center' },
                       { name: 'ParentRowID', index: 'ParentRowID', hidden: true },
                       { name: 'SessionID', index: 'SessionID', hidden: true }
            ],
            pager: pagerElement,
            altRows: true,
            loadonce: true,
            cache: false,
            altclass: 'alternate',
            height: '160',
            hidegrid: false,
            rowNum: 50,
            autowidth: true,
            shrinkToFit: false,
            sortname: 'ID',
            viewrecords: true,
            sortorder: "desc",
            caption: '<div id="ruleExecutionLogDiv"><button id ="btnRuleExecutionLog" type="button" class="btn pull-right" style="padding:1px 5px;">View Rule Execution History</button></div> <span id="headerRuleExecutionLog" class="ui-jqgrid-title">Current Rule Execution Log</span>',
            gridComplete: function () {
                $(elementIDs.ruleExecutionLogGridJQ).jqGrid('setGridWidth', $(window).width() - (($(window).width() * 4) / 100), true);
            },
            loadComplete: function (log) {
                if (isCurrentRuleExecution) {
                    if (!$.isEmptyObject(ruleExecutionLogFormInstanceData[parseInt(formInstanceId)])) {
                        var ruleExecutionLogdata = ruleExecutionLogFormInstanceData[parseInt(formInstanceId)].filter(function (ct) {
                            return ct.IsNewRecord == true && ct.IsParentRow == true;
                        });
                        if (ruleExecutionLogdata != undefined) {
                            ruleExecutionLogdata = ruleExecutionLogdata.reverse();
                            var rowNum = $(elementIDs.ruleExecutionLogGridJQ).getGridParam('rowNum');
                            log.rows = ruleExecutionLogdata.slice((log.page * rowNum) - rowNum, log.page * rowNum);
                            log.records = ruleExecutionLogdata.length;
                            var totalPages = Math.ceil((log.records / rowNum));
                            log.total = totalPages;
                        }
                        var gridRows = $(elementIDs.ruleExecutionLogGridJQ)[0];
                        gridRows.addJSONData(log);
                    }
                }
            },
            subGrid: true,
            subGridRowExpanded: function (subgrid_id, row_id) {
                var rowData = $(elementIDs.ruleExecutionLogGridJQ).jqGrid('getRowData', row_id);
                var viewCurrentLog = true;
                if ($(elementIDs.btnRuleExecutionLog).text() == "View Current Rule Execution") {
                    viewCurrentLog = false;
                }

                ruleExecutionLogger.loadRuleExecutionSubGrid(subgrid_id, rowData.ParentRowID, viewCurrentLog, rowData.SessionID);
            },
        });
        $(elementIDs.ruleExecutionLogGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
        $(elementIDs.ruleExecutionLogGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, refresh: false, search: false }, {}, {}, {});
        $(elementIDs.btnRuleExecutionLog).bind('click', function () {
            if ($(elementIDs.btnRuleExecutionLog).text() == "View Current Rule Execution") {
                isCurrentRuleExecution = true;
                loadRuleExecutionGrid(false);
                $(elementIDs.btnRuleExecutionLog).text("View Rule Execution History");
                $(elementIDs.headerRuleExecutionLog).text("Current Rule Execution Log");
                $(elementIDs.headerRuleExecutionLog).append(' - <b>' + currentInstance.formName + '</b>');
            }
            else {
                loadRuleExecutionGrid(true);
                isCurrentRuleExecution = false;
                $(elementIDs.btnRuleExecutionLog).text("View Current Rule Execution");
                $(elementIDs.headerRuleExecutionLog).text("History Rule Execution Log");
                $(elementIDs.headerRuleExecutionLog).append(' - <b>' + currentInstance.formName + '</b>');
            }
        });

        $(pagerElement + "_left").css("width", "");

        $(elementIDs.ruleExecutionLogGridJQ).jqGrid('navButtonAdd', pagerElement,
       {
           caption: '', buttonicon: 'ui-icon-arrowstop-1-s', title: 'Download Rule Execution Log To Excel', id: 'btnRuleExecutionLogExportToExcel',
           onClickButton: function () {
               exportRuleExecutionLogToExcel();
           }
       });
    }

    function loadRuleExecutionSubGrid(ruleExecutionLogSubGrid, parentRowID, viewCurrentLog, sessionId) {
        var loadDataGrid = undefined;
        var url = "";
        var dataType;
        if (!viewCurrentLog) {
            url = URLs.getRuleExecutionLogDataURL.replace(/{formInstnaceId}/g, this.formInstanceId).replace(/{parentRowID}/g, parentRowID).replace(/{isParentData}/g, false).replace(/{sessionId}/g, sessionId);
            dataType = "json";
            isCurrentRuleExecution = false;
        }
        else {
            if (ruleExecutionLogFormInstanceData != undefined) {
                if (ruleExecutionLogFormInstanceData[formInstanceId]) {
                    loadDataGrid = ruleExecutionLogFormInstanceData[formInstanceId].filter(function (ct) {
                        return ct.IsParentRow == false && ct.ParentRowID == parentRowID;
                    });
                }
            }

            dataType = "local";
            isCurrentRuleExecution = true;
        }

        var ruleExecutionLogSubGridTable = ruleExecutionLogSubGrid + "_t";
        $("#" + ruleExecutionLogSubGrid).html("<table id='" + ruleExecutionLogSubGridTable + "' class='scroll'></table>");

        $("#" + ruleExecutionLogSubGridTable).jqGrid({
            url: url,
            datatype: dataType,
            data: loadDataGrid,
            colNames: ['ID', 'Impacted Element Path', 'Field', 'Impacted Attribute', 'Description', 'Rule'],
            colModel: [{ name: 'ID', index: 'ID', key: true, hidden: true },
                       { name: 'ImpactedElementPath', index: 'ImpactedElementPath', width: '400' },
                       { name: 'ImpactedElementLabel', index: 'ImpactedElementLabel', width: '200' },
                       { name: 'PropertyType', index: 'PropertyType', width: '150' },
                       { name: 'ImpactDescription', index: 'ImpactDescription', width: '400', formatter: descriptionColumnFormatter },
                       { name: 'RuleID', index: 'RuleID', width: '100', search: false, formatter: addRuleDescriptionLink }],
            sortname: 'UpdatedDate',
            sortorder: "desc",
            height: '100%',
            viewrecords: true,
            emptyrecords: 'No records found',
            width: '60%',
        });
    }

    function dateTimeFormatter(cellvalue, options, rowObject) {
        if (rowObject.IsNewRecord == true) {
            return "<b>" + $.fn.fmatter.call(this, "date", cellvalue, options, rowObject); + "</b>";
        }
        else {
            return $.fn.fmatter.call(this, "date", cellvalue, options, rowObject);
        }
    }

    function updatedByColumnFormatter(cellvalue, options, rowObject) {
        if (rowObject.IsNewRecord == true) {
            return cellvalue;
        }
        else {
            return cellvalue.replace(/<b>/g, " ").replace(/<\/b>/g, " ");
        }
    }

    function addRuleDescriptionLink(cellvalue, options, rowObject) {
        var btnRuleDescription = '';
        if (cellvalue != undefined && cellvalue != '' && cellvalue != -1) {
            btnRuleDescription = "<span class='ui-icon ui-icon-document' style='cursor: pointer;' onclick=\"ruleExecutionLogger.showRuleDescriptionDialog(" + rowObject.QHPQueueID + ");\"></span>";
        }
        return btnRuleDescription;
    }

    function showRuleDescriptionDialog(ruleID) {
        if (ruleID != undefined && ruleID != '' && ruleID != -1) {
            var ruleDescription = 'Rule Description is not found for this Rule[' + ruleID + ']';
            var url = URLs.getRuleDescription.replace(/\{ruleID\}/g, ruleID);
            var promise = ajaxWrapper.getJSONSync(url);
            promise.done(function (result) {
                if (result != null) {
                    $('#messagedialog').dialog({
                        title: 'Rule Description',
                        height: 125,
                        width: 500,
                        zIndex: 99999
                    });
                    messageDialog.show(result.RuleDescription)
                }
            });
        }
    }

    function descriptionColumnFormatter(cellvalue, options, rowObject) {
        if (rowObject.IsNewRecord == true) {
            return cellvalue;
        }
        else {
            return cellvalue;
        }
    }

    function logRulesExecution(elementID, oldValue, newValue, impactedElementID, ruleID, result, logFor, isParentRow, parentRowID, onEvent) {
        var rowID = !this.ruleExecutionLogData[formInstanceId] ? 1 : (this.ruleExecutionLogData[formInstanceId].length + 1);
        if (elementID == null || elementID == '') { elementID = -1; }
        var row = {
            ID: rowID,
            CurrentFormInstanceId: formInstanceId,
            LoadedElement: '',
            IsParentRow: isParentRow,
            ParentRowID: isParentRow ? rowID : parentRowID,
            OnEvent: onEvent,
            ElementID: elementID,
            OldValue: Array.isArray(oldValue) ? oldValue.join() : oldValue,
            NewValue: Array.isArray(newValue) ? newValue.join() : newValue,
            ImpactedElementID: impactedElementID,
            RuleID: ruleID,
            Result: result,
            LogFor: logFor
        }
        this.ruleExecutionLogData[formInstanceId].push(row);
        return rowID;
    }

    function logRulesExecutionOnLoad(loadedElement, logFor, onEvent, isGetLogFromDB) {

        if (!this.ruleExecutionLogData[formInstanceId]) {
            this.ruleExecutionLogData[formInstanceId] = new Object(new Array());
        }
        if (isGetLogFromDB) {
            fillRuleExecutionLogDataFromDB();
        }
        var rowID = !this.ruleExecutionLogData[formInstanceId] ? 1 : (this.ruleExecutionLogData[formInstanceId].length + 1);
        var row = {
            ID: rowID,
            CurrentFormInstanceId: formInstanceId,
            LoadedElement: loadedElement,
            IsParentRow: true,
            ParentRowID: rowID,
            OnEvent: onEvent,
            ElementID: -1,
            OldValue: '',
            NewValue: '',
            ImpactedElementID: -1,
            RuleID: -1,
            Result: null,
            LogFor: logFor
        }
        this.ruleExecutionLogData[formInstanceId].push(row);
        return rowID;
    }

    function fillRuleExecutionLogFormInstanceData(currentInstance) {
        if (this.ruleExecutionLogData != undefined) {
            if (this.currentSessionID == null) getCurrentSessionID();
            if (this.ruleExecutionLogData[formInstanceId]) {
                logData = loadDataGrid = this.ruleExecutionLogData[formInstanceId].slice();
                for (var idx = 0; idx < logData.length; idx++) {
                    var row = logData[idx];
                    if (row.IsParentRow) {
                        if (!ifChildRowExist(logData, row.ID)) {
                            continue;
                        }
                    }
                    var isAddLog = false;
                    if (this.ruleExecutionLogFormInstanceData[formInstanceId]) {
                        if (!ifLogEntryExists(row.ID))
                            isAddLog = true;
                    } else
                        isAddLog = true;

                    if (isAddLog) {
                        switch (row.LogFor) {
                            case "Activity":
                                logActivity(currentInstance, row);
                                break;
                            case "RuleForElement":
                                logRulesForElementExecution(currentInstance, row);
                                break;
                            case "OnLoad":
                                logRulesExecutionOnDocumentOrSectionLoad(currentInstance, row);
                                break;
                            case "OnSection":
                            case "OnElement":
                                logRulesExecutionOnSectionOrElements(currentInstance, row);
                                break;
                        }
                    }
                }
            }
        }
    }

    function ifChildRowExist(logData, parentRowId) {
        var childData = logData.filter(function (ct) {
            return ct.IsParentRow == false && ct.ParentRowID == parentRowId;
        });
        if (childData != null && childData.length > 0)
            return true;
        else
            return false;
    }

    function ifLogEntryExists(rowId) {
        return this.ruleExecutionLogFormInstanceData[formInstanceId].some(function (el) {
            return el.ID === rowId;
        });
    }

    function fillRuleExecutionLogDataFromDB() {
        if (this.ruleExecutionLogData[formInstanceId]) {
            urlP = URLs.getRuleExecutionServerLogDataURL.replace(/{formInstnaceId}/g, formInstanceId).replace(/{sessionID}/g, currentSessionID).replace(/{parentRowID}/g, -1).replace(/{isParentData}/g, true);
            var promise = ajaxWrapper.getJSONSync(urlP);
            promise.done(function (resultP) {
                if (resultP != null) {
                    for (iP = 0; iP < resultP.length; iP++) {
                        var parentRowId = logRulesExecutionOnLoad(resultP[iP].LoadedElement, resultP[iP].LogFor, resultP[iP].OnEvent, false);
                        url = URLs.getRuleExecutionServerLogDataURL.replace(/{formInstnaceId}/g, formInstanceId).replace(/{sessionID}/g, currentSessionID).replace(/{parentRowID}/g, resultP[iP].ID).replace(/{isParentData}/g, false);
                        var promise = ajaxWrapper.getJSONSync(url);
                        promise.done(function (result) {
                            if (result != null) {
                                for (i = 0; i < result.length; i++) {
                                    logRulesExecution(result[i].ElementID, result[i].OldValue, result[i].NewValue, result[i].ImpactedElementID, result[i].RuleID, result[i].Result, result[i].LogFor, false, parentRowId, result[i].LogFor)
                                }
                            }
                        });
                    }
                }
            });
        }
    }

    function getElementDataByElementFullName(currentInstance, sectionName, elementFullName) {
        if (currentInstance.designData != undefined) {
            var sectionDetails = currentInstance.designData.Sections.filter(function (ct) {
                return ct.Label == sectionName;
            });
        } else {
            var sectionDetails = currentInstance.formInstances[formInstanceId].FormInstanceBuilder.designData.Sections.filter(function (ct) {
                return ct.FullName == elementFullName.substr(0, elementFullName.indexOf('.'));
            });
        }
        return getElementDetails(sectionDetails[0], elementFullName);
    }

    function getElementDataByElementId(currentInstance, sectionName, elementId) {
        if (currentInstance.designData != undefined) {
            var sectionDetails = currentInstance.designData.Sections.filter(function (ct) {
                return ct.Label == sectionName;
            });
        } else {
            var sectionDetails = currentInstance.formInstances[formInstanceId].FormInstanceBuilder.designData.Sections.filter(function (ct) {
                return ct.Label == sectionName;
            });
        }

        if (sectionDetails.length > 0) {
            var elementData = sectionDetails[0].Elements.filter(function (el) {
                return el.ElementID == elementId;
            });
            return elementData[0];
        }
    }

    function getRuleObject(currentInstance, ruleID) {
        if (currentInstance.designData != undefined) {
            var rules = currentInstance.designData.Rules.filter(function (rule) {
                return rule.RuleID == ruleID;
            });
        } else {
            var rules = currentInstance.formInstances[formInstanceId].FormInstanceBuilder.designData.Rules.filter(function (rule) {
                return rule.RuleID == ruleID;
            });
        }
        return rules[0];
    }

    function logActivity(currentInstance, logRow) {
        if (logRow.ElementID != -1) {
            var elementData = getElementDataByElementId(currentInstance, currentInstance.selectedSectionName, logRow.ElementID)
            if (elementData != undefined) {
                var elementLabel = getPropertyNameLable(elementData.Label);
                var elementPath = '';

                if (currentInstance.designData != undefined) {
                    var sectionDetails = currentInstance.designData.Sections.filter(function (ct) {
                        return ct.Label == currentInstance.selectedSectionName;
                    });
                    elementPath = currentInstance.selectedSectionName + getElementHierarchy(sectionDetails[0], elementData.FullName);
                } else {
                    var sectionDetails = currentInstance.formInstances[formInstanceId].FormInstanceBuilder.designData.Sections.filter(function (ct) {
                        return ct.FullName == elementData.FullName.substr(0, elementData.FullName.indexOf('.'));
                    });
                    elementPath = elementData.FullName.substr(0, elementData.FullName.indexOf('.')) + getElementHierarchy(sectionDetails[0], elementData.FullName);
                }


                var msg = "Value of <b>" + elementLabel + "</b> is changed from <b>" + (logRow.OldValue === "" ? "null" : logRow.OldValue) + "</b> to <b>" + logRow.NewValue + "</b>.";
                var upadatedByMsg = "<b>" + currentInstance.currentUserName + "</b>";

                var rowCount = $(elementIDs.ruleExecutionLogGridJQ).getGridParam("reccount");
                var row = {
                    ID: logRow.ID,
                    SessionID: this.currentSessionID,
                    FormInstanceId: parseInt(formInstanceId),
                    IsParentRow: logRow.IsParentRow,
                    ParentRowID: logRow.ParentRowID,
                    OnEvent: logRow.OnEvent,
                    ElementID: logRow.ElementID,
                    ElementPath: elementPath,
                    ElementLabel: elementLabel,
                    OldValue: Array.isArray(logRow.OldValue) ? logRow.OldValue.join() : logRow.OldValue,
                    NewValue: Array.isArray(logRow.NewValue) ? logRow.NewValue.join() : logRow.NewValue,
                    ImpactedElementID: logRow.ImpactedElementID,
                    ImpactedElementLabel: '',
                    ImpactedElementPath: '',
                    ImpactDescription: msg,
                    PropertyType: '',
                    RuleID: logRow.RuleID,
                    FolderVersion: this.folderData.versionNumber,
                    UpdatedBy: upadatedByMsg,
                    UpdatedDate: new Date(),
                    IsNewRecord: true
                }

                updateRuleExecutionLogFormInstanceData(parseInt(formInstanceId), rowCount, row);
            }
        }
    }

    function logRulesForElementExecution(currentInstance, logRow) {
        var rule = getRuleObject(currentInstance, logRow.RuleID);
        var propertyType = getPropertyType(rule.TargetPropertyTypeId);
        if (currentInstance.designData != undefined) {
            var sectionDetails = currentInstance.designData.Sections.filter(function (ct) {
                return ct.Label == currentInstance.selectedSectionName;
            });
        } else {
            var sectionDetails = currentInstance.formInstances[formInstanceId].FormInstanceBuilder.designData.Sections.filter(function (ct) {
                return ct.Label == currentInstance.selectedSectionName;
            });
        }

        var elementLabel = '';
        var elementPath = '';
        if (logRow.ElementID != -1) {
            var elementData = getElementDataByElementId(currentInstance, currentInstance.selectedSectionName, logRow.ElementID)
            if (elementData != undefined) {
                elementLabel = getPropertyNameLable(elementData.Label);
                elementPath = currentInstance.selectedSectionName + getElementHierarchy(sectionDetails[0], elementData.FullName);
            }
        }
        var impactedElementData = getElementDetails(sectionDetails[0], rule.UIElementFullName);
        var impactedElementLabel = impactedElementData != undefined ? getPropertyNameLable(impactedElementData.Label) : rule.UIElementName;
        var impactedElementPath = currentInstance.selectedSectionName + getElementHierarchy(sectionDetails[0], rule.UIElementFullName);

        var msg = getMessageOfRulesForElementExecution(rule.TargetPropertyTypeId, logRow.Result, logRow.OldValue, logRow.NewValue, false);
        var upadatedByMsg = "<b>" + currentInstance.currentUserName + "</b>";

        var rowCount = $(elementIDs.ruleExecutionLogGridJQ).getGridParam("reccount");
        var row = {
            ID: logRow.ID,
            SessionID: this.currentSessionID,
            FormInstanceId: parseInt(formInstanceId),
            IsParentRow: logRow.IsParentRow,
            ParentRowID: logRow.ParentRowID,
            OnEvent: logRow.OnEvent,
            ElementID: logRow.ElementID,
            ElementPath: elementPath,
            ElementLabel: elementLabel,
            OldValue: Array.isArray(logRow.OldValue) ? logRow.OldValue.join() : logRow.OldValue,
            NewValue: Array.isArray(logRow.NewValue) ? logRow.NewValue.join() : logRow.NewValue,
            ImpactedElementID: logRow.ImpactedElementID,
            ImpactedElementLabel: impactedElementLabel,
            ImpactedElementPath: impactedElementPath,
            ImpactDescription: msg,
            PropertyType: propertyType,
            RuleID: logRow.RuleID,
            FolderVersion: this.folderData.versionNumber,
            UpdatedBy: upadatedByMsg,
            UpdatedDate: new Date(),
            IsNewRecord: true
        }

        updateRuleExecutionLogFormInstanceData(parseInt(formInstanceId), rowCount, row);
    }

    function logRulesExecutionOnSectionOrElements(currentInstance, logRow) {
        var upadatedByMsg = "<b>" + currentInstance.currentUserName + "</b>";
        var rule = getRuleObject(currentInstance, logRow.RuleID);
        var propertyType = getPropertyType(rule.TargetPropertyTypeId);
        var msg = '';
        if (logRow.LogFor == 'OnSection') {
            var sectionInfo = currentInstance.sections == undefined ? "" : currentInstance.sections[rule.UIElementFormName];
            var elementPath = sectionInfo != undefined || sectionInfo == "" ? sectionInfo.SectionName : rule.UIElementFormName;
            var elementLabel = ''
            msg = getMessageOfRulesForElementExecution(rule.TargetPropertyTypeId, logRow.Result, logRow.OldValue, logRow.NewValue, true);
        } else if (logRow.LogFor == 'OnElement') {
            var elementData = getElementDataByElementFullName(currentInstance, currentInstance.selectedSectionName, rule.UIElementFullName);
            var elementLabel = elementData != undefined ? getPropertyNameLable(elementData.Label) : rule.UIElementName;
            var elementPath = '';
            if (currentInstance.designData != undefined) {
                var sectionDetails = currentInstance.designData.Sections.filter(function (ct) {
                    return ct.Label == currentInstance.selectedSectionName;
                });
                elementPath = currentInstance.selectedSectionName + getElementHierarchy(sectionDetails[0], rule.UIElementFullName);
            } else {
                var sectionDetails = currentInstance.formInstances[formInstanceId].FormInstanceBuilder.designData.Sections.filter(function (ct) {
                    return ct.FullName == rule.UIElementFullName.substr(0, rule.UIElementFullName.indexOf('.'));
                });
                elementPath = rule.UIElementFullName.substr(0, rule.UIElementFullName.indexOf('.')) + getElementHierarchy(sectionDetails[0], rule.UIElementFullName);
            }


            msg = getMessageOfRulesForElementExecution(rule.TargetPropertyTypeId, logRow.Result, logRow.OldValue, logRow.NewValue, false);
        }

        var rowCount = $(elementIDs.ruleExecutionLogGridJQ).getGridParam("reccount");
        var row = {
            ID: logRow.ID,
            SessionID: this.currentSessionID,
            FormInstanceId: parseInt(formInstanceId),
            IsParentRow: logRow.IsParentRow,
            ParentRowID: logRow.ParentRowID,
            OnEvent: logRow.OnEvent,
            ElementID: logRow.ElementID,
            ElementPath: '',
            ElementLabel: '',
            OldValue: Array.isArray(logRow.OldValue) ? logRow.OldValue.join() : logRow.OldValue,
            NewValue: Array.isArray(logRow.NewValue) ? logRow.NewValue.join() : logRow.NewValue,
            ImpactedElementID: logRow.ImpactedElementID,
            ImpactedElementLabel: elementLabel,
            ImpactedElementPath: elementPath,
            ImpactDescription: msg,
            PropertyType: propertyType,
            RuleID: logRow.RuleID,
            FolderVersion: this.folderData.versionNumber,
            UpdatedBy: upadatedByMsg,
            UpdatedDate: new Date(),
            IsNewRecord: true
        }
        updateRuleExecutionLogFormInstanceData(parseInt(formInstanceId), rowCount, row);
    }

    function getMessageOfRulesForElementExecution(propertyTypeId, result, oldValue, newValue, isSection) {
        var MessageText;
        switch (propertyTypeId) {
            case targetPropertyTypes.Enabled:
                MessageText = "This " + (isSection ? "section" : "field") + " is " + (result ? "enabled" : "disabled") + " due to change in one of it's sources.";
                break;
            case targetPropertyTypes.Value:
                MessageText = "The value of the " + (isSection ? "section" : "field") + " is changed from " + oldValue + " to " + newValue;
                break;
            case targetPropertyTypes.Visible:
                MessageText = "This " + (isSection ? "section" : "field") + " is become " + (result ? "visible" : "hidden") + " due to change in one of it's sources.";
                break;
            case targetPropertyTypes.IsRequired:
                MessageText = "This " + (isSection ? "section" : "field") + " is " + (result ? "required" : "not required") + " due to change in one of it's sources.";
                break;
            case targetPropertyTypes.RunValidation:
                MessageText = "RunValidation";
                break;
            case targetPropertyTypes.Error:
                MessageText = "Error";
                break;
            case targetPropertyTypes.Highlight:
                MessageText = "Highlight";
                break;
            case targetPropertyTypes.Dialog:
                MessageText = "Dialog";
                break;
            default:
                MessageText = '';
        }
        return MessageText;
    }

    function logRulesExecutionOnDocumentOrSectionLoad(currentInstance, logRow) {
        var upadatedByMsg = "<b>" + currentInstance.currentUserName + "</b>";
        var rowCount = $(elementIDs.ruleExecutionLogGridJQ).getGridParam("reccount");
        var row = {
            ID: logRow.ID,
            SessionID: this.currentSessionID,
            FormInstanceId: parseInt(formInstanceId),
            IsParentRow: logRow.IsParentRow,
            ParentRowID: logRow.ParentRowID,
            OnEvent: logRow.OnEvent,
            ElementID: logRow.ElementID,
            ElementPath: logRow.LoadedElement,
            ElementLabel: '',
            OldValue: Array.isArray(logRow.OldValue) ? logRow.OldValue.join() : logRow.OldValue,
            NewValue: Array.isArray(logRow.NewValue) ? logRow.NewValue.join() : logRow.NewValue,
            ImpactedElementID: logRow.ImpactedElementID,
            ImpactedElementLabel: '',
            ImpactedElementPath: '',
            ImpactDescription: '',
            PropertyType: '',
            RuleID: logRow.RuleID,
            FolderVersion: this.folderData.versionNumber,
            UpdatedBy: upadatedByMsg,
            UpdatedDate: new Date(),
            IsNewRecord: true
        }
        updateRuleExecutionLogFormInstanceData(parseInt(formInstanceId), rowCount, row);
    }

    function updateRuleExecutionLogFormInstanceData(currentFormInstanceId, rowCount, row) {
        if (!this.ruleExecutionLogFormInstanceData[currentFormInstanceId]) {
            this.ruleExecutionLogFormInstanceData[currentFormInstanceId] = new Object(new Array());
            this.ruleExecutionLogFormInstanceData[currentFormInstanceId].push(row);
        } else {
            this.ruleExecutionLogFormInstanceData[currentFormInstanceId].push(row);
        }
        var allRecords = $(elementIDs.ruleExecutionLogGridJQ).getGridParam('records');
        var rowNum = $(elementIDs.ruleExecutionLogGridJQ).getGridParam('rowNum');
        var totalPages = parseInt((allRecords / rowNum) + 1);

        var currentPage = $(elementIDs.ruleExecutionLogGridJQ).getGridParam('page');
        var p;
        for (p = 1; p <= totalPages; p++) {
            if (p == totalPages) {
                $(elementIDs.ruleExecutionLogGridJQ).jqGrid('addRowData', (rowCount + 1), row, "first");
                $(elementIDs.ruleExecutionLogGridJQ).trigger("reloadGrid", [{ page: currentPage }]);
            }
        }
        isCurrentRuleExecution = true;
        $(elementIDs.btnRuleExecutionLog).text("View Rule Execution History");
        $(elementIDs.headerRuleExecutionLog).text("Current Rule Execution Log");
        $(elementIDs.headerRuleExecutionLog).append(' - <b>' + this.formName + '</b>');
    }

    function getPropertyNameLable(propertyNameLable) {
        return propertyNameLable.replace(/([a-z])([A-Z])/g, '$1 $2')
                                    .replace(/\b([A-Z]+)([A-Z])([a-z])/, '$1 $2$3')
                                    .replace(/^./, function (str) { return str.toUpperCase(); });
    }

    function getPropertyType(propertyTypeId) {
        var propertyType;
        switch (propertyTypeId) {
            case targetPropertyTypes.Enabled:
                propertyType = "Enabled";
                break;
            case targetPropertyTypes.Value:
                propertyType = "Value";
                break;
            case targetPropertyTypes.Visible:
                propertyType = "Visible";
                break;
            case targetPropertyTypes.IsRequired:
                propertyType = "IsRequired";
                break;
            case targetPropertyTypes.RunValidation:
                propertyType = "RunValidation";
                break;
            case targetPropertyTypes.Error:
                propertyType = "Error";
                break;
            case targetPropertyTypes.Highlight:
                propertyType = "Highlight";
                break;
            case targetPropertyTypes.Dialog:
                propertyType = "Dialog";
                break;
            default:
                propertyType = '';
        }
        return propertyType;
    }

    function logRepeaterActivity(fullName, field, rowNumber, operation, currentUserName, lastUpdatedDate, repeaterName, currentFormInstanceId, oldValue, newValue, buFilterCriteria, repeaterDesign, repeaterID, keyValue, customMsg) {
        //if (rowNumber) {
        if (oldValue != undefined) {
            if (!$.isArray(oldValue)) {
                if (oldValue.toString().trim() === '') {
                    oldValue = "''";
                }
            }
        }
        if (newValue != undefined) {
            if (!$.isArray(newValue)) {
                if (newValue.toString().trim() === '') {
                    newValue = "''";
                }
            }
        }

        var operationVerb = "";
        switch (operation) {
            case 0:
                operationVerb = " Added";
                break;
            case 1:
                operationVerb = " Changed";
                break;
            case 2:
                operationVerb = " Deleted";
                break;
            case 3:
                operationVerb = " Copied";
                break;
            default:

        }
        var buSearchParam = null;
        var cnt = 0;
        var msg = null;
        if (buFilterCriteria != undefined) {
            $.each(buFilterCriteria, function (ind, prop) {
                var inlColName = null;
                if (prop.field.substring(0, 4) == 'INL_') {
                    var propArr = prop.field.split('_');
                    if (propArr.length == 4) {
                        inlColName = propArr[3];
                    }
                }
                var filterField = (inlColName == null ? prop.field : inlColName);
                if (cnt == 0)
                    buSearchParam = "Bulk Update Filter Criteria[" + filterField + ": " + prop.data + "]";
                else
                    buSearchParam = buSearchParam + "[" + filterField + ": " + prop.data + "]";
                cnt++;
            });
        }
        var upadatedByMsg = "<b>" + currentUserName + "</b>";
        var rowCount = $(elementIDs.ruleExecutionLogGridJQ).getGridParam("reccount");

        //var keyValue = "";
        // If operation is delete then we get no data of selected row as row is delete before activitylog so we use oldvalue to hold repeater key data before row delete
        if (operation == 2 && oldValue != undefined) {
            keyValue = oldValue;
        }
        //else {
        //    var selectedRowId = $("#" + repeaterID).jqGrid('getGridParam', 'selrow');
        //    $.each(repeaterDesign.Elements, function (index, element) {
        //        if (element.IsKey == true) {
        //            var value = $("#" + repeaterID).jqGrid('getCell', selectedRowId, element.GeneratedName);
        //            keyValue += value + " ";
        //        }
        //    });
        //}
        if (customMsg == undefined) {
            if (keyValue !== "") {
                if (cnt == 0) {
                    if (operationVerb == " Deleted" || operationVerb == " Added")
                        msg = "<b>" + repeaterName + "</b> was" + operationVerb;
                    else
                        msg = "<b>" + repeaterName + "</b> was" + operationVerb + " from " + oldValue + " to " + newValue;
                }
                else
                    msg = buSearchParam + " <b>" + repeaterName + "</b> was" + operationVerb + " from " + oldValue + " to " + newValue;
            }
            else {
                if (cnt == 0) {
                    if (operationVerb == " Deleted" || operationVerb == " Added")
                        msg = "Row " + (parseInt(rowNumber) + 1) + " in <b>" + repeaterName + "</b> was" + operationVerb;
                    else
                        msg = "Row " + (parseInt(rowNumber) + 1) + " in <b>" + repeaterName + "</b> was" + operationVerb + " from " + oldValue + " to " + newValue;
                }
                else
                    msg = buSearchParam + " Row " + (parseInt(rowNumber) + 1) + " in <b>" + repeaterName + "</b> was" + operationVerb + " from " + oldValue + " to " + newValue;
            }
        }
        else {
            msg = customMsg;
        }

        var rowValue = "";

        if (keyValue != "")
            rowValue = keyValue;
        else {
            if (rowNumber != undefined) {
                rowValue = (parseInt(rowNumber) + 1);
            }
            else {
                rowValue = "";
            }
        }

        var row = {
            ID: (rowCount + 1),
            FormInstanceId: currentFormInstanceId,
            ElementPath: fullName,
            Field: field,
            RowNum: rowValue,
            FolderVersionName: this.folderData.versionNumber,
            Description: msg,
            UpdatedBy: upadatedByMsg,
            UpdatedLast: lastUpdatedDate,
            IsNewRecord: true
        }

        if (!this.ruleExecutionLogFormInstanceData[currentFormInstanceId]) {
            this.ruleExecutionLogFormInstanceData[currentFormInstanceId] = new Object(new Array());
            this.ruleExecutionLogFormInstanceData[currentFormInstanceId].push(row);
        } else {
            this.ruleExecutionLogFormInstanceData[currentFormInstanceId].push(row);
        }
        if (buFilterCriteria == undefined) {
            var allRecords = $(elementIDs.ruleExecutionLogGridJQ).getGridParam('records');
            var rowNum = $(elementIDs.ruleExecutionLogGridJQ).getGridParam('rowNum');
            var totalPages = parseInt((allRecords / rowNum) + 1);

            var currentPage = $(elementIDs.ruleExecutionLogGridJQ).getGridParam('page');
            var p;
            for (p = 1; p <= totalPages; p++) {
                if (p == totalPages) {
                    $(elementIDs.ruleExecutionLogGridJQ).jqGrid('addRowData', (rowCount + 1), row, "first");
                    $(elementIDs.ruleExecutionLogGridJQ).trigger("reloadGrid", [{ page: currentPage }]);
                }
            }
            isCurrentRuleExecution = true;
            $(elementIDs.btnRuleExecutionLog).text("View Rule Execution History");
            $(elementIDs.headerRuleExecutionLog).text("Current Rule Execution Log");
            $(elementIDs.headerRuleExecutionLog).append(' - <b>' + this.formName + '</b>');
        }
    }

    function logRepeaterActivityPQ(fullName, field, rowNumber, operation, currentUserName, lastUpdatedDate, repeaterName, currentFormInstanceId, oldValue, newValue, buFilterCriteria, repeaterDesign, repeaterID, keyValue, customMsg) {
        //if (rowNumber) {

        var operationVerb = "";
        switch (operation) {
            case 0:
                operationVerb = " Added";
                break;
            case 1:
                operationVerb = " Changed";
                break;
            case 2:
                operationVerb = " Deleted";
                break;
            case 3:
                operationVerb = " Copied";
                break;
            default:

        }
        var buSearchParam = null;
        var cnt = 0;
        var msg = null;
        if (buFilterCriteria != undefined) {
            $.each(buFilterCriteria[0], function (ind, prop) {
                var inlColName = null;
                if (prop.dataIndx.toString().substring(0, 4) == 'INL_') {
                    var propArr = prop.dataIndx.split('_');
                    if (propArr.length == 4) {
                        inlColName = propArr[3];
                    }
                }
                var filterField = (inlColName == null ? prop.dataIndx : inlColName);
                if (cnt == 0)
                    buSearchParam = "Bulk Update Filter Criteria[" + filterField + ": " + prop.value + "]";
                else
                    buSearchParam = buSearchParam + "[" + filterField + ": " + prop.value + "]";
                cnt++;
            });
        }
        var upadatedByMsg = "<b>" + currentUserName + "</b>";
        var rowCount = $(elementIDs.ruleExecutionLogGridJQ).getGridParam("reccount");

        //var keyValue = "";
        // If operation is delete then we get no data of selected row as row is delete before activitylog so we use oldvalue to hold repeater key data before row delete
        if (operation == 2 && oldValue != undefined) {
            keyValue = oldValue;
        }
        //else {
        //    var selectedRowId = $("#" + repeaterID).jqGrid('getGridParam', 'selrow');
        //    $.each(repeaterDesign.Elements, function (index, element) {
        //        if (element.IsKey == true) {
        //            var value = $("#" + repeaterID).jqGrid('getCell', selectedRowId, element.GeneratedName);
        //            keyValue += value + " ";
        //        }
        //    });
        //}
        if (oldValue != undefined) {
            if (!$.isArray(oldValue)) {
                if (oldValue.toString().trim() === '') {
                    oldValue = "''";
                }
            }
        }
        if (newValue != undefined) {
            if (!$.isArray(newValue)) {
                if (newValue.toString().trim() === '') {
                    newValue = "''";
                }
            }
        }
        if (customMsg == undefined) {
            if (keyValue !== "") {
                if (cnt == 0) {
                    if (operationVerb == " Deleted" || operationVerb == " Added")
                        msg = "<b>" + repeaterName + "</b> was" + operationVerb;
                    else
                        msg = "<b>" + repeaterName + "</b> was" + operationVerb + " from " + oldValue + " to " + newValue;
                }
                else
                    msg = buSearchParam + " <b>" + repeaterName + "</b> was" + operationVerb + " from " + oldValue + " to " + newValue;
            }
            else {
                if (cnt == 0) {
                    if (operationVerb == " Deleted" || operationVerb == " Added")
                        msg = "Row " + (parseInt(rowNumber) + 1) + " in <b>" + repeaterName + "</b> was" + operationVerb;
                    else
                        msg = "Row " + (parseInt(rowNumber) + 1) + " in <b>" + repeaterName + "</b> was" + operationVerb + " from " + oldValue + " to " + newValue;
                }
                else
                    msg = buSearchParam + " Row " + (parseInt(rowNumber) + 1) + " in <b>" + repeaterName + "</b> was" + operationVerb + " from " + oldValue + " to " + newValue;
            }
        }
        else {
            msg = customMsg;
        }

        var rowValue = "";

        if (keyValue != "")
            rowValue = keyValue;
        else {
            if (rowNumber != undefined) {
                rowValue = (parseInt(rowNumber) + 1);
            }
            else {
                rowValue = "";
            }
        }

        var row = {
            ID: (rowCount + 1),
            FormInstanceId: currentFormInstanceId,
            ElementPath: fullName,
            Field: field,
            RowNum: rowValue,
            FolderVersionName: this.folderData.versionNumber,
            Description: msg,
            UpdatedBy: upadatedByMsg,
            UpdatedLast: lastUpdatedDate,
            IsNewRecord: true
        }

        if (!this.ruleExecutionLogFormInstanceData[currentFormInstanceId]) {
            this.ruleExecutionLogFormInstanceData[currentFormInstanceId] = new Object(new Array());
            this.ruleExecutionLogFormInstanceData[currentFormInstanceId].push(row);
        } else {
            this.ruleExecutionLogFormInstanceData[currentFormInstanceId].push(row);
        }
        if (buFilterCriteria == undefined) {
            var allRecords = $(elementIDs.ruleExecutionLogGridJQ).getGridParam('records');
            var rowNum = $(elementIDs.ruleExecutionLogGridJQ).getGridParam('rowNum');
            var totalPages = parseInt((allRecords / rowNum) + 1);

            var currentPage = $(elementIDs.ruleExecutionLogGridJQ).getGridParam('page');
            var p;
            for (p = 1; p <= totalPages; p++) {
                if (p == totalPages) {
                    $(elementIDs.ruleExecutionLogGridJQ).jqGrid('addRowData', (rowCount + 1), row, "first");
                    $(elementIDs.ruleExecutionLogGridJQ).trigger("reloadGrid", [{ page: currentPage }]);
                }
            }
            isCurrentRuleExecution = true;
            $(elementIDs.btnRuleExecutionLog).text("View Rule Execution History");
            $(elementIDs.headerRuleExecutionLog).text("Current Rule Execution Log");
            $(elementIDs.headerRuleExecutionLog).append(' - <b>' + this.formName + '</b>');
        }
    }

    function getLabelText(targetID) {
        var text = $("label[for='" + targetID + "']").text();
        return text.replace(":", "").replace("*", "");
    }

    function clearLog() {
        if (!$.isEmptyObject(ruleExecutionLogFormInstanceData[parseInt(formInstanceId)])) {
            ruleExecutionLogFormInstanceData[this.formInstanceId] = new Object(new Array());
        }
        if (!$.isEmptyObject(ruleExecutionLogData[parseInt(formInstanceId)])) {
            ruleExecutionLogData[this.formInstanceId] = new Object(new Array());
        }
    }

    function haveChangesToForm(formName) {
        var data = $(elementIDs.ruleExecutionLogGridJQ).jqGrid('getGridParam', 'data');
        var hasFormData = false;

        if (data.length > 0) {
            for (var i = 0; i < data.length; i++) {
                if (data[i].Form === formName) {
                    hasFormData = true;
                    break;
                }
            }
        }
        return hasFormData;
    }

    function getRuleExecutionLogFormInstanceData(formId) {
        var ruleExecutionLogData = undefined;
        if (ruleExecutionLogFormInstanceData != undefined) {
            if (ruleExecutionLogFormInstanceData[formId]) {
                ruleExecutionLogData = this.ruleExecutionLogFormInstanceData[formId];
            }
        }
        return ruleExecutionLogData;
    }

    function saveFormInstanceRuleExecutionLogData(currentInstance) {
        var ruleExecutionLogDataToBeSaved = [];
        fillRuleExecutionLogFormInstanceData(currentInstance);
        var currentRuleExecutionlogData = getRuleExecutionLogFormInstanceData(parseInt(formInstanceId));
        if (currentRuleExecutionlogData != undefined && currentRuleExecutionlogData != null) {
            ruleExecutionLogDataToBeSaved = currentRuleExecutionlogData.filter(function (ct) {
                return ct.IsNewRecord == true;
            });
        }
        if (!$.isEmptyObject(ruleExecutionLogDataToBeSaved)) {
            var formInstanceRuleExecutionLogdata = {
                tenantId: currentInstance.tenantId,
                formInstanceId: parseInt(currentInstance.formInstanceId),
                folderId: parseInt(currentInstance.folderId),
                folderVersionId: parseInt(currentInstance.folderVersionId),
                formDesignId: currentInstance.formDesignId,
                formDesignVersionId: currentInstance.formDesignVersionId,
                ruleExecutionLogFormInstanceData: JSON.stringify(ruleExecutionLogDataToBeSaved)
            }
            var promise = ajaxWrapper.postJSONCustom(URLs.saveRuleExecutionLogDataURL, formInstanceRuleExecutionLogdata, false);
            promise.done(function (xhr) {
                if (xhr.Result == ServiceResult.FAILURE) {
                    messageDialog.show("Error occured while saving 'Rule Execution Log data to databases");
                }
                else if (xhr.Result == ServiceResult.SUCCESS) {
                    getCurrentSessionID();
                    if (!$.isEmptyObject(currentRuleExecutionlogData)) {
                        for (i = 0 ; i < currentRuleExecutionlogData.length; i++) {
                            currentRuleExecutionlogData[i].IsNewRecord = false;
                        }
                    }
                }
            });
        }
    }

    function exportRuleExecutionLogToExcel() {
        var currentInstance = this;
        var data = getRuleExecutionLogFormInstanceData(parseInt(formInstanceId));
        var results = data.map(function (item) {
            return {
                ID: item["ID"], ParentRowID: item["ParentRowID"], OnEvent: item["OnEvent"],
                ElementPath: item["ElementPath"], ElementLabel: item["ElementLabel"], OldValue: item["OldValue"], NewValue: item["NewValue"],
                ImpactedElementPath: item["ImpactedElementPath"], ImpactedElementLabel: item["ImpactedElementLabel"], ImpactDescription: item["ImpactDescription"], PropertyType: item["PropertyType"],
                FolderVersion: item["FolderVersion"], UpdatedBy: item["UpdatedBy"], UpdatedDate: item["UpdatedDate"]
            }
        });

        var stringData = "csv=" + GetCSVString(results, "\t").replace(/(<([^>]+)>)|\n|>/ig, "").replace(/=/g, "->");
        stringData += "<&isGroupHeader=" + false;
        stringData += "<&noOfColInGroup=0";
        stringData += "<&isChildGrid=" + false;
        stringData += "<&repeaterName=" + "Rule Execution Log";
        stringData += "<&formName=" + currentInstance.formName;
        stringData += "<&folderVersionId=" + currentInstance.folderData.folderVersionId;
        stringData += "<&folderId=" + currentInstance.folderData.folderId;
        stringData += "<&tenantId=" + 1;

        $.download(URLs.exportToExcel, stringData, 'post');
    }

    function GetCSVString(data, separator) {
        var csvstring = "";
        for (var row = 0; row < data.length; row++) {
            var totalColumns = Object.keys(data[row]).length
            var columnCounter = 0

            if (row === 0) {
                for (var key in data[row]) {
                    csvstring += key + (columnCounter + 1 < totalColumns ? separator : '\r\n')
                    columnCounter++
                }
            } else {
                for (var key in data[row - 1]) {
                    csvstring += data[row - 1][key] == null ? "" : data[row - 1][key] + (columnCounter + 1 < totalColumns ? separator : '\r\n')
                    columnCounter++
                }
            }
            columnCounter = 0
        }
        return csvstring;
    }

    return {
        init: function () {
            init();
        },
        showRuleDescriptionDialog: function (ruleID) {
            showRuleDescriptionDialog(ruleID);
        },
        loadRuleExecutionSubGrid: function (ruleExecutionLogSubGrid, parentRowID, viewCurrentLog, sessionId) {
            loadRuleExecutionSubGrid(ruleExecutionLogSubGrid, parentRowID, viewCurrentLog, sessionId);
        },
        getFormInstanceId: function (formInstanceId) {
            getFormInstanceId(formInstanceId);
        },
        getFormName: function (fromName) {
            getFormName(fromName);
        },
        logRulesExecution: function (elementID, oldValue, newValue, impactedElementID, ruleID, result, logFor, isParentRow, parentRowID, onEvent) {
            return logRulesExecution(elementID, oldValue, newValue, impactedElementID, ruleID, result, logFor, isParentRow, parentRowID, onEvent);
        },
        logRulesExecutionOnLoad: function (loadedElement, logFor, onEvent, isGetLogFromDB) {
            return logRulesExecutionOnLoad(loadedElement, logFor, onEvent, isGetLogFromDB);
        },
        logRepeater: function (fullName, field, rowNumber, operation, currentUserName, lastUpdatedBy, repeaterName, currentFormInstanceId, oldValue, newValue, buFilterCriteria, repeaterDesign, repeaterID, keyValue, customMsg) {
            logRepeaterActivity(fullName, field, rowNumber, operation, currentUserName, lastUpdatedBy, repeaterName, currentFormInstanceId, oldValue, newValue, buFilterCriteria, repeaterDesign, repeaterID, keyValue, customMsg);
        },
        logRepeaterPQ: function (fullName, field, rowNumber, operation, currentUserName, lastUpdatedBy, repeaterName, currentFormInstanceId, oldValue, newValue, buFilterCriteria, repeaterDesign, repeaterID, keyValue, customMsg) {
            logRepeaterActivityPQ(fullName, field, rowNumber, operation, currentUserName, lastUpdatedBy, repeaterName, currentFormInstanceId, oldValue, newValue, buFilterCriteria, repeaterDesign, repeaterID, keyValue, customMsg);
        },
        clear: function () {
            clearLog();
        },
        activate: function (active) {
            activateLogging(active);
        },
        haveChangesToForm: function (formName) {
            return haveChangesToForm(formName);
        },
        loadRuleExecutionLogGrid: function (currentInstance) {
            return loadRuleExecutionLogGrid(currentInstance);
        },
        saveFormInstanceRuleExecutionLogData: function (currentInstance) {
            return saveFormInstanceRuleExecutionLogData(currentInstance);
        },
        getRuleExecutionLogFormInstanceData: function (formInstanceId) {
            return getRuleExecutionLogFormInstanceData(formInstanceId);
        }
    }
}();


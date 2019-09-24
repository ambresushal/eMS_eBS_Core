var manualDataSourceMappingDialogAG = function (elementData, caption, repeaterBuilder, hasDisabledParent) {
    //Caption for the dialog box. 
    this.caption = caption;
    //All elements of the  grids elements.
    this.elementData = elementData;
    this.repeaterBuilder = repeaterBuilder;
    this.sourceDataList = [];
    this.dataSourceMapping = null;
    this.bulkUpdateCaption = 'Add/Remove Services';
    this.elementIDs = {
        repeaterManualDialogJQ: '#manualDataMappingDialog',
        repeaterManualSourceDataJQ: '#DataSourceData',
        manualGridJQ: '#manualGrid',
        manualGrid: 'manualGrid',
        manualMassageDialogJQ: '#manualMessageDialog',
        manualDialogJQ: '#manualDialog',
        manualDialogButtonJQ: '#manualDialogBtn',
        manualDialogSaveButtonJQ: "#manualDataSourceSaveBtn",
        manualDialogCancelButtonJQ: "#manualDataSourceCancelBtn",
        viewAllCheckBoxJQ: '#viewAllCheckBox',
        manualDialogAllViewCheckbox: 'input[id^=checkbox_]',
        divLTSEBulkUpdate: '#divLTSEBulkUpdate',
        btnLimitInformationDetailBulkUpdateAdd: '#btnLimitInformationDetailBulkUpdateAdd',
        btnLimitInformationDetailBulkUpdateDelete: '#btnLimitInformationDetailBulkUpdateDelete',
        limitLTSERepeater: '#FAC2Repeater608' + repeaterBuilder.formInstanceId,
    };
    this.URLs = {
        //tenantId,formInstanceId, formDesignVersionId, folderVersionId, formDesignId, fullName, sData
        getSourceFormInstanceData: "/FormInstance/GetSourceFormInstanceData?tenantId={tenantId}&targetFormInstanceId={targetFormInstanceId}&formDesignVersionId={formDesignVersionId}&folderVersionId={folderVersionId}&formDesignId={formDesignId}&fullName={fullName}",
        getAdditionalANOCChartServices: "/ExpressionBuilder/GetAdditionalANOCChartServices"
    }
    this.isIncludeChild = null;
    this.childDatasource = null;
    this.isMappedwithOtherDataSource = null;
    this.mappedWithDatasource = null;

    this.addLimitElementIDs = {
        planLevelLimitJQ: '#PlanLevelLimit',
        planLimitJQ: '#planLimit',
        planLimit: 'planLimit',
        PlanLimitSaveButtonJQ: "#PlanLimitSaveBtn",
        PlanLimitCancelButtonJQ: "#PlanLimitCancelBtn",
        limitDescriptionJQ: '#LimitDescription',
        limitDescriptionHelpBlock: '#LimitDescriptionHelpBlock',
        amountDDLJQ: '#AmountDDL',
        limitTypeDDLJQ: '#LimitTypeDDL',
        frequencyDDLJQ: '#FrequencyDDL',
    },
    this.planLimitList = [];
    this.planLimit = this.addLimitDialogMethods();
    this.isEditable = repeaterBuilder.formInstanceBuilder.folderData.isEditable;
    this.hasDisabledParent = hasDisabledParent;
    this.ebRules = new expressionBuilderRules();
    this.expressionBuilderRulesExt = new expressionBuilderRulesExt();
}
//This method for initialize Manual repeater Dialog
manualDataSourceMappingDialogAG.prototype.initializeDialog = function () {
    $(this.elementIDs.repeaterManualDialogJQ).dialog({
        autoOpen: false,
        resizable: false,
        closeOnEscape: true,
        height: 'auto',
        width: 850,
        height: 450,
        modal: true,
        position: ['middle', 100],
    });
    // $(this.elementIDs.manualDialogSaveButtonJQ).removeAttr('disabled', 'disabled');
}

//This method for popup manual repeater dialog
manualDataSourceMappingDialogAG.prototype.mapDataSourceManually = function () {
    var currentInstance = this;
    var sourceMappingData = undefined;

    //get all data source mapping.
    sourceMappingData = currentInstance.getSourceAndTargetDataSourceMapping();

    currentInstance.dataSourceMapping = sourceMappingData[0];
    if (currentInstance.dataSourceMapping.DisplayMode == "Primary" && currentInstance.repeaterBuilder.fullName == "StandardServices.StandardServiceList") {// && currentInstance.IsIncludeChild(currentInstance.dataSourceMapping.Mappings)) {
        currentInstance.isIncludeChild = true;
    }
    currentInstance.childDatasource = currentInstance.repeaterBuilder.design.ChildDataSources;

    //initialize Manual repeater Dialog
    currentInstance.initializeDialog();

    currentInstance.getSourceFormInstanceData(sourceMappingData);

    //$(currentInstance.elementIDs.repeaterManualDialogJQ).dialog('option', 'title', dialogTitle);
    //$(currentInstance.elementIDs.repeaterManualDialogJQ).dialog("open");

    if (this.caption == this.bulkUpdateCaption) {
        $(currentInstance.elementIDs.divLTSEBulkUpdate).show();
        $(currentInstance.elementIDs.manualDialogButtonJQ).hide();
    
        $(currentInstance.elementIDs.btnLimitInformationDetailBulkUpdateAdd).off("click");
        $(currentInstance.elementIDs.btnLimitInformationDetailBulkUpdateAdd).on("click", function () {
            currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.BulkUpdateLimitInformationDetailRepeaterData('ADD', currentInstance);
            $(currentInstance.elementIDs.manualGridJQ).empty();
            $(currentInstance.elementIDs.repeaterManualDialogJQ).dialog("close");
        });
    
        $(currentInstance.elementIDs.btnLimitInformationDetailBulkUpdateDelete).off("click");
        $(currentInstance.elementIDs.btnLimitInformationDetailBulkUpdateDelete).on("click", function () {
            currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.BulkUpdateLimitInformationDetailRepeaterData('DELETE', currentInstance);
            $(currentInstance.elementIDs.manualGridJQ).empty();
            $(currentInstance.elementIDs.repeaterManualDialogJQ).dialog("close");
        });
    }
    else {
        $(currentInstance.elementIDs.divLTSEBulkUpdate).hide();
        $(currentInstance.elementIDs.manualDialogButtonJQ).show();
    }

    $(currentInstance.elementIDs.manualDialogSaveButtonJQ).off("click");
    $(currentInstance.elementIDs.manualDialogSaveButtonJQ).on("click", function () {
        currentInstance.loadManualData();
    });

    $(currentInstance.elementIDs.manualDialogCancelButtonJQ).off("click");
    $(currentInstance.elementIDs.manualDialogCancelButtonJQ).on("click", function () {
        $(currentInstance.elementIDs.manualGridJQ).empty();
        $(currentInstance.elementIDs.repeaterManualDialogJQ).dialog("close");
    });

    if (currentInstance.isEditable == "False" || currentInstance.hasDisabledParent == true) {
        $(currentInstance.elementIDs.manualDialogSaveButtonJQ).attr('disabled', 'disabled');
        $(currentInstance.elementIDs.btnLTSEBulkUpdateAdd).attr('disabled', 'disabled');
        $(currentInstance.elementIDs.btnLTSEBulkUpdateDelete).attr('disabled', 'disabled');
    }
    else {
        $(currentInstance.elementIDs.manualDialogSaveButtonJQ).removeAttr('Disabled', 'disabled');
        $(currentInstance.elementIDs.btnLTSEBulkUpdateAdd).removeAttr('Disabled', 'disabled');
        $(currentInstance.elementIDs.btnLTSEBulkUpdateDelete).removeAttr('Disabled', 'disabled');
    }
}

manualDataSourceMappingDialogAG.prototype.getSourceAndTargetDataSourceMapping = function () {
    var currentInstance = this;
    var sourceMappingData;
    var currentSection = currentInstance.repeaterBuilder.formInstanceBuilder.selectedSectionName;
    var cuurentFormDesignId = currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId;

    var designData = currentInstance.repeaterBuilder.formInstanceBuilder.designData;
    if (currentInstance.repeaterBuilder.gridType == "child") {
        var parentRepeaterName = currentInstance.repeaterBuilder.fullName.split('.');
        parentRepeaterName.pop(parentRepeaterName.length - 1);
        var repeaterName = parentRepeaterName.join('.');
        sourceMappingData = designData.DataSources.filter(function (index) {
            return (index.TargetParent == repeaterName && index.DisplayMode == 'Child');
        });
    }
    else {
        sourceMappingData = designData.DataSources.filter(function (index) {
            return (index.TargetParent == currentInstance.repeaterBuilder.fullName && index.DisplayMode == 'Primary');
        });

        var mappedWithDatasource = designData.DataSources.filter(function (index) {
            return (index.SourceParent == currentInstance.repeaterBuilder.fullName && index.DataSourceModeType == "Auto");
        });
        if (mappedWithDatasource.length > 0) {

            $.each(mappedWithDatasource, function (idx, dataSource) {
                var section = dataSource.SourceParent.split('.');
                if (dataSource.FormDesignID == cuurentFormDesignId && currentSection == section[0]) {
                    currentInstance.isMappedwithOtherDataSource = true;
                    currentInstance.mappedWithDatasource = dataSource;
                }
            });
        }
    }

    return sourceMappingData;
}

manualDataSourceMappingDialogAG.prototype.getSourceFormInstanceData = function (sourceMappingData) {
    var currentInstance = this;
    var promise = undefined;

    ajaxDialog.showPleaseWait();
    var fullName = sourceMappingData[0].SourceParent;
    var sectionName = fullName.split('.');

    if (currentInstance.repeaterBuilder.formInstanceBuilder.selectedSectionName == sectionName[0]
        && currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId == sourceMappingData[0].FormDesignID && currentInstance.repeaterBuilder.formInstanceBuilder.selectedSectionName != "Limits") {
        currentInstance.getSourceDatafromCurrentInstance(fullName, sourceMappingData);
    }
    else {

        if (sourceMappingData[0].DataSourceModeType == "Manual") {
            if (currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.hasExpressionBuilderRule(currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId) && currentInstance.repeaterBuilder.fullName == "Limits.LimitsInformation.LimitList")
            {
                currentInstance.getSourceDatafromCurrentInstance("Limits.AllowableLimits.AllowableLimitsList", sourceMappingData);
                currentInstance.setTargetRepeaterData(sourceMappingData[0].SourceParent, currentInstance.sourceDataList, sourceMappingData);
                ajaxDialog.hidePleaseWait();
            }
            else if (currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.hasExpressionBuilderRule(currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId) && currentInstance.repeaterBuilder.fullName == "LimitList.LimitTextLimitDescriptionAssociation.MLLimitTextDetailsList") {
                currentInstance.getSourceDatafromCurrentInstance("LimitList.LimitTextDetailList", sourceMappingData);
                currentInstance.setTargetRepeaterData(sourceMappingData[0].SourceParent, currentInstance.sourceDataList, sourceMappingData);
                ajaxDialog.hidePleaseWait();
            }
            else if (currentInstance.repeaterBuilder.formInstanceBuilder.designData.FormDesignId != FormTypes.MEDICALFORMDESIGNID && currentInstance.repeaterBuilder.formInstanceBuilder.designData.FormDesignId != FormTypes.DENTALFORMID) {
                /** if (currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.hasExpressionBuilderRule(currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId) && currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.FACETS.altRuleServiceGroupDetailRepeater) {
                     currentInstance.sourceDataList = currentInstance.repeaterBuilder.formInstanceBuilder.formData[currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.sectionElementName.FACETS.serviceGroup][currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.repeaterElementName.FACETS.serviceGroupingDetails];
                     currentInstance.setTargetRepeaterData(sourceMappingData[0].SourceParent, currentInstance.sourceDataList, sourceMappingData);
                     ajaxDialog.hidePleaseWait();
                 }
                 else if (currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.hasExpressionBuilderRule(currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId) && currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.FACETS.altRuleAdditionalServicesDetailsRepeater) {
                     currentInstance.sourceDataList = currentInstance.repeaterBuilder.formInstanceBuilder.formData[currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.sectionElementName.FACETS.additionalServices][currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.repeaterElementName.FACETS.additionalServicesDetails];
                     currentInstance.setTargetRepeaterData(sourceMappingData[0].SourceParent, currentInstance.sourceDataList, sourceMappingData);
                     ajaxDialog.hidePleaseWait();
                 }
                 else if (currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.hasExpressionBuilderRule(currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId) && currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.FACETS.limitServicesLTSE) {
                     currentInstance.sourceDataList = currentInstance.repeaterBuilder.formInstanceBuilder.formData[currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.sectionElementName.FACETS.Limits][currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.sectionElementName.FACETS.FacetsLimits][currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.repeaterElementName.FACETS.limitRulesLTLT];
                     currentInstance.setTargetRepeaterData(sourceMappingData[0].SourceParent, currentInstance.sourceDataList, sourceMappingData);
                     ajaxDialog.hidePleaseWait();
                 }
                 else*/ {
                     url = currentInstance.URLs.getSourceFormInstanceData.replace(/\{tenantId\}/g, currentInstance.repeaterBuilder.tenantId)
                                                                         .replace(/\{targetFormInstanceId\}/g, currentInstance.repeaterBuilder.formInstanceId)
                                                                         .replace(/\{formDesignVersionId\}/g, currentInstance.repeaterBuilder.formInstanceBuilder.formDesignVersionId)
                                                                         .replace(/\{folderVersionId\}/g, currentInstance.repeaterBuilder.formInstanceBuilder.folderVersionId)
                                                                         .replace(/\{formDesignId\}/g, sourceMappingData[0].FormDesignID)
                                                                         .replace(/\{fullName\}/g, sourceMappingData[0].SourceParent);
                     promise = ajaxWrapper.getJSON(url);
                 }
            }
        }
        else {
            var repeaterData = undefined;
            if (currentInstance.repeaterBuilder.gridType == "child") {
                var gridID = currentInstance.repeaterBuilder.gridElementIdJQ.split('_');
               // repeaterData = $(gridID[0]).getRowData(currentInstance.repeaterBuilder.parentRowId);
            }
            var cuurentRepeaterName = currentInstance.repeaterBuilder.fullName;
            if (cuurentRepeaterName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.ANOCChartANOCBenefitsCompare) {
                var postData = {
                    tenantId: currentInstance.repeaterBuilder.formInstanceBuilder.tenantId,
                    nextYearFormInstanceId: currentInstance.repeaterBuilder.formInstanceBuilder.anchorFormInstanceID,
                    //previousYearFormInstanceId: currentInstance.repeaterBuilder.formInstanceBuilder.formData[currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.sectionName.ANOCChartSection][currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.sectionName.SourceDocument][currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.sectionElementName.SourecDocumentID],
                    previousYearFormInstanceId: currentInstance.repeaterBuilder.formInstanceBuilder.formData.ANOCChartPlanDetails.SourceDocument.SourceDocumentID,
                    formDesignVersionId: currentInstance.repeaterBuilder.formInstanceBuilder.formDesignVersionId,
                    folderVersionId: currentInstance.repeaterBuilder.formInstanceBuilder.folderVersionId,
                    sectionName: currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.sectionName.ANOCChartSection,
                    repeaterData: JSON.stringify(currentInstance.repeaterBuilder.data),
                    effectiveDate: currentInstance.repeaterBuilder.formInstanceBuilder.folderData.effectiveDate,
                    anocViewFormInstanceId: currentInstance.repeaterBuilder.formInstanceBuilder.formInstanceId
                }
                // To add additional services not covered in benefit comparasion chart in ANOCChart in popup
                promise = ajaxWrapper.postJSON(currentInstance.URLs.getAdditionalANOCChartServices, postData);
            }
            else {
                // To run custom datasources using expression builder
                promise = currentInstance.ebRules.runCustomDataSources(currentInstance.repeaterBuilder.tenantId, currentInstance.repeaterBuilder.formInstanceBuilder.folderVersionId, currentInstance.repeaterBuilder.formInstanceId, currentInstance.repeaterBuilder.formInstanceBuilder.formDesignVersionId, currentInstance.repeaterBuilder.design.ID, sourceMappingData[0].TargetParent);
            }
        }
        //if (url != undefined) {
        //    promise = ajaxWrapper.getJSON(url);
        //}

        if (promise != undefined) {
            promise.done(function (data) {
                ajaxDialog.hidePleaseWait();
                currentInstance.sourceDataList = JSON.parse(data);
                //if (currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId == FormTypes.BUSINESS && currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.BUSINESS.ServiceGroupDetails) {
                if (currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId == FormTypes.ANOCChartViewFormDesignID && currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.ANOCChartANOCBenefitsCompare) {
                    currentInstance.sourceDataList = currentInstance.sourceDataList.filter(function (value, idx) {
                        return value.ServiceGroupName != "";
                    });
                }
                currentInstance.setTargetRepeaterData(sourceMappingData[0].SourceParent, currentInstance.sourceDataList, sourceMappingData);
            });
            //register ajax failure callback
            promise.fail(currentInstance.showError);
        }
    }
}

manualDataSourceMappingDialogAG.prototype.showError = function (xhr) {
    if (xhr.status == 999)
        window.location.href = '/Account/LogOn';
    else
        messageDialog.show(Common.errorMsg);
}

//This method for Manual repeater Layout
manualDataSourceMappingDialogAG.prototype.applyDialogLayout = function (mappings, sourceServicesName, sourceMappingData) {
    var currentInstance = this;    
    if (jQuery("#" + currentInstance.elementIDs.manualGrid).length==0)
    {
        $(currentInstance.elementIDs.repeaterManualSourceDataJQ).append("<div class='grid-wrapper'><table id='" + currentInstance.elementIDs.manualGrid + "'> </table></div>");
    }
    currentInstance.loadGrid(sourceMappingData, sourceServicesName, mappings);

    $(currentInstance.elementIDs.viewAllCheckBoxJQ).click(function (e) {
        //$(currentInstance.elementIDs.manualGridJQ).find(".check-all").parent(".ui-jqgrid-sortable").removeClass("ui-jqgrid-sortable");
        $(".check-all").parent(".ui-jqgrid-sortable").removeClass("ui-jqgrid-sortable");
        var val = $(currentInstance.elementIDs.viewAllCheckBoxJQ).is(':checked');
        if (val) {
            $(currentInstance.elementIDs.viewAllCheckBoxJQ).attr('checked', 'checked');
            $(currentInstance.elementIDs.manualDialogAllViewCheckbox).filter(':not(:disabled)').prop('checked', true);
            currentInstance.applyCheckAll("Yes");
        }
        else {
            $(currentInstance.elementIDs.viewAllCheckBoxJQ).removeAttr('checked');
            $(currentInstance.elementIDs.manualDialogAllViewCheckbox).filter(':not(:disabled)').prop('checked', false);
            currentInstance.applyCheckAll("No");
        }
    });
}

manualDataSourceMappingDialogAG.prototype.applyCheckAll = function (value) {
    var currentInstance = this;
    var gridData = $(currentInstance.elementIDs.manualGridJQ).jqGrid('getGridParam', 'data');
    var filterCondtion = $(currentInstance.elementIDs.manualGridJQ).getGridParam("postData").filters;
    if (filterCondtion != undefined) {
        var filter = JSON.parse(filterCondtion);
        if (filter.rules.length > 0) {
            var filtergridData;
            $.each(gridData, function (idx, row) {
                if (filterCondtion != null) {

                    filtergridData = $.grep(filter.rules, function (a) {
                        return (row[a.field].toLowerCase().indexOf(a.data.toLowerCase()) >= 0);
                    });
                    if (filtergridData.length > 0 && filtergridData.length == filter.rules.length) {
                        row.IsSelect = value;
                    }
                }
            });
        }
        else {
            $.each(gridData, function (idx, row) {
                row.IsSelect = value;
            });
        }
    }
    else {
        $.each(gridData, function (idx, row) {
            row.IsSelect = value;
        });
    }
    $(currentInstance.elementIDs.manualGridJQ).jqGrid('setGridParam', { data: gridData }).trigger('reloadGrid')
}
//Load Maual Repeater data
manualDataSourceMappingDialogAG.prototype.loadGrid = function (sourceData, sourceServicesName, mappings) {
    var currentInstance = this;

    //set column list for grid
    var colArray = [];
    //set column models   
    var colModel = [];
    //add select column in grid
    var viewAllCheckbox = "Select All";


    if (currentInstance.isEditable == "False" || currentInstance.hasDisabledParent == true) {
        viewAllCheckbox = '<input type="checkbox" class="check-all"  id="viewAllCheckBox" disabled="disabled" />';
    }
    else {
        viewAllCheckbox = '<input type="checkbox" class="check-all"  id="viewAllCheckBox" />';
    }


    colArray.push(viewAllCheckbox);
    colArray.push('ID');
    colModel.push({
        name: 'IsSelect', align: 'center', width: 80, sortable: true, editable: false, edittype: "checkbox", editoptions: { value: "true:false", defaultValue: "false" },
        formatter: checkBoxFormatter, unformat: uncheckBoxFormatter, sorttype: function (value, item) {
            return item.IsSelect == "Yes" ? true : false;
        }
    });
    colModel.push({
        name: 'ID', align: 'center', width: 80, sortable: true, key: true, hidden: true
    });

    var mappedElementData = [];
    for (var j = 0; j < sourceData.length; j++) {
        var rowData = {};
        $.map(mappings, function (elementOfArray, indexInArray) {
            for (key in sourceData[j]) {
                if (elementOfArray.SourceElement == key) {
                    sourceData[j][elementOfArray.TargetElement] = sourceData[j][key];
                    rowData[[elementOfArray.TargetElement]] = sourceData[j][key];
                    if (j == 0) {
                        var elementName = currentInstance.repeaterBuilder.design.Elements.filter(function (ct) {
                            return ct.GeneratedName == elementOfArray.TargetElement
                        });
                        colArray.push(elementName[0].Label);
                        colModel.push({ name: elementOfArray.TargetElement, index: elementOfArray.TargetElement, editable: false, align: 'left' });
                    }
                }
            }
        });
        rowData["IsSelect"] = "";
        rowData["ID"] = j + 1;
        mappedElementData.push(rowData);
    }

    currentInstance.applySelectedData(mappedElementData);
    var lastgridsel;
    currentInstance.sourceDataList = sourceData;
    $(currentInstance.elementIDs.manualGridJQ).empty();
    //clean up the grid first - only table element remains after this
    $(currentInstance.elementIDs.manualGridJQ).jqGrid('GridUnload');
    //adding the pager element
    $(currentInstance.elementIDs.manualGridJQ).parent().append("<div id='p" + currentInstance.elementIDs.manualGrid + "'></div>");
    // var url = URLs.portfolioBasedAccountDetails;

    $(currentInstance.elementIDs.manualGridJQ).jqGrid({
        datatype: 'local',
        data: mappedElementData,
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: this.caption == this.bulkUpdateCaption ? this.bulkUpdateCaption : sourceServicesName,
        height: '175',
        rowNum: 20,
        ignoreCase: true,
        loadonce: true,
        autowidth: true,
        shrinkToFit: true,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        sortname: 'IsSelect',
        sortorder: 'desc',
        pager: '#p' + currentInstance.elementIDs.manualGrid,
        altclass: 'alternate',
        onSelectRow: function (id) {
        },
        gridComplete: function () {
        },
        loadComplete: function () {
            var p = this.p, data = p.data, item, $this = $(this), index = p._index, rowid;
            for (rowid in index) {
                if (index.hasOwnProperty(rowid)) {
                    item = data[index[rowid]];
                    if (item.IsSelect == "false" || item.IsSelect == "false" || item.IsSelect == "true" || item.IsSelect == false || item.IsSelect == true) {
                        $this.jqGrid('setSelection', rowid, false);
                    }
                }
            }
        },

    });

    $(currentInstance.elementIDs.manualGridJQ).on('change', 'input[name="checkbox_manualGrid"]', function (e) {
        var element = $(this).attr("Id");
        var id = element.split('_');
        var cellValue = $(this).is(":checked");
        if (cellValue) {
            cellValue = "Yes";
        }
        else {
            cellValue = "No";
        }
        $(currentInstance.elementIDs.manualGridJQ).jqGrid('setCell', id[1], 'IsSelect', cellValue);
        var selectRowData = $(currentInstance.elementIDs.manualGridJQ).getLocalRow(id[1]);
        selectRowData.IsSelect = cellValue;
        $(currentInstance.elementIDs.manualGridJQ).jqGrid("saveRow", id[1], selectRowData);
        $(currentInstance.elementIDs.manualGridJQ).editRow(id[1], true);
    });


    //This GroupHeader is used for check-all checkbox
    $(currentInstance.elementIDs.manualGridJQ).jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
             { startColumnName: 'IsSelect', numberOfColumns: 1, titleText: 'Select All' },
        ]
    });

    var pagerElement = '#p' + currentInstance.elementIDs.manualGrid;
    $(pagerElement).find('input').css('height', '20px');
    //remove default buttons
    $(currentInstance.elementIDs.manualGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    $(currentInstance.elementIDs.manualGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

    $("#viewAllCheckBox").parent('div').css('text-align', 'center');

    $(pagerElement + '_left').css('width', '');
    function checkBoxFormatter(cellValue, options, rowObject) {
        if (cellValue == "No")
            if (currentInstance.isEditable == "False" || currentInstance.hasDisabledParent == true) {
                return "<input type='checkbox'  class='staticData' id='checkbox_" + options.rowId + "' name= 'checkbox_" + options.gid + "' disabled='disabled'/>";
            }
            else {
                return "<input type='checkbox' class='staticData' id='checkbox_" + options.rowId + "' name= 'checkbox_" + options.gid + "' />";
            }
        else
            if (currentInstance.isEditable == "False" || currentInstance.hasDisabledParent == true) {
                return "<input type='checkbox' class='staticData' id='checkbox_" + options.rowId + "' name= 'checkbox_" + options.gid + "'  checked=checked disabled='disabled'/>";
            }
            else {
                return "<input type='checkbox' class='staticData' id='checkbox_" + options.rowId + "' name= 'checkbox_" + options.gid + "'  checked=checked/>";
            }
    }

    function uncheckBoxFormatter(cellvalue, options, cell) {
        var result;
        result = $(this).find('#' + options.rowId).find('input').prop('checked');

        if (result == true || result == "true")
            return 'Yes';
        else
            return 'No';
    }

    //var customrule = new customRule();
    //if (customrule.hasCustomRules(currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId) == true) {
    //    if (currentInstance.repeaterBuilder.fullName == customrule.isCustomRule(currentInstance.repeaterBuilder.fullName)) {
    //        $(currentInstance.elementIDs.manualGridJQ).jqGrid('navButtonAdd', pagerElement,
    //           {
    //               caption: 'Add Limit', title: 'Add Limit', id: 'btnRepeaterBuilderAdd',
    //               onClickButton: function () {
    //                   currentInstance.populateAddLimitDialog();
    //               }
    //           });
    //    }
    //}
}

manualDataSourceMappingDialogAG.prototype.populateAddLimitDialog = function () {
    var currentInstance = this;
    var standardlimitDesignData = undefined;
    currentInstance.repeaterBuilder.formInstanceBuilder.designData.Sections.filter(function (ct) {
        if (ct.GeneratedName === "Limits") {
            ct.Elements.filter(function (ele) {
                if (ele.GeneratedName === "StandardLimits") {
                    if (ele.Repeater.GeneratedName === "StandardLimits") {
                        standardlimitDesignData = ele.Repeater;
                        return false;
                    }
                }
            });
        }
    });

    currentInstance.planLimit.initalizeAddLimitDialog(standardlimitDesignData);
}

manualDataSourceMappingDialogAG.prototype.loadManualData = function () {
    var currentInstance = this;
    var oldData = currentInstance.repeaterBuilder.data;
    var repeaterSelectedData = [];
    var selectedData = new Object();
    selectedData.list = new Array();
    //var customrule = new customRule();
    var customrule = new customRulePQ();

    var allRowData = $(currentInstance.elementIDs.manualGridJQ).jqGrid('getGridParam', 'data');

    var k = 0;
    for (var i = 0 ; i < allRowData.length; i++) {
        if (allRowData[i].IsSelect == "Yes" && currentInstance.sourceDataList[i] != undefined) {
            selectedData.list[k] = currentInstance.sourceDataList[i];
            delete allRowData[i];
            delete selectedData.list[k]['IsSelect'];
            k++;
        }
    }
    if (k == 0) {
        //messageDialog.show(Common.pleaseSelectRowMsg);
        //return;
    }
    var prevCount = 0;
    var count = 0;
    var rowCount = 0;

    if (currentInstance.repeaterBuilder.expressionBuilder.hasExpressionBuilderRule(currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId)
        && (currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.ANOCChartANOCBenefitsCompare)) {

        if (currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.ANOCChartANOCBenefitsCompare) {
            this.updateCurrentRepeater(currentInstance, selectedData, repeaterSelectedData, oldData);
        }

        currentInstance.repeaterBuilder.expressionBuilder.manualdataSourceSaveButtonClick(currentInstance, currentInstance.repeaterBuilder.data, oldData, selectedData.list, true);
    }
    else {
        this.updateCurrentRepeater(currentInstance, selectedData, repeaterSelectedData, oldData);
    }

    currentInstance.repeaterBuilder.filterdMessagesRowId = null;
    currentInstance.repeaterBuilder.SERLMessages = [];
    currentInstance.repeaterBuilder.DisallowedMessage = [];
    currentInstance.repeaterBuilder.DisallowedMessageValue = null;
    currentInstance.repeaterBuilder.SERLMessagesValue = null;
    $(currentInstance.elementIDs.manualGridJQ).empty();
    $(currentInstance.elementIDs.repeaterManualDialogJQ).dialog("close");
    if (this.expressionBuilderRulesExt.hasRule(currentInstance.repeaterBuilder, null)) {
        this.expressionBuilderRulesExt.processRule(currentInstance.repeaterBuilder, null, currentInstance.repeaterBuilder.formInstanceBuilder.formDesignVersionId, currentInstance.repeaterBuilder.formInstanceBuilder.folderVersionId, currentInstance.repeaterBuilder.formInstanceBuilder.formInstanceId, currentInstance.repeaterBuilder.formInstanceBuilder.formData, currentInstance.repeaterBuilder);
    }
}

manualDataSourceMappingDialogAG.prototype.IsRecordExists = function (compareElement, ComparerList) {
    var currentInstance = this;
    var dataSourceLength = currentInstance.dataSourceMapping.Mappings.length

    if (currentInstance.dataSourceMapping.Mappings) {
        var isRecordExists = ComparerList.filter(function (dt) {
            var diff = currentInstance.dataSourceMapping.Mappings.filter(function (map) {
                return compareElement[map.TargetElement] == dt[map.TargetElement];
            })

            if (diff.length == dataSourceLength) {
                return dt;
            }
        });
    }
    return isRecordExists.length == 0 ? true : false;
}

manualDataSourceMappingDialogAG.prototype.synchDataSourceMapping = function (newData) {
    var currentInstance = this;

    if (currentInstance.mappedWithDatasource != null && currentInstance.mappedWithDatasource.TargetParent == "Network.NetworkInformation.NetworkClaimsMailingAddress") {
        var builders = currentInstance.repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (builder) {
            return builder.fullName == currentInstance.mappedWithDatasource.TargetParent;
        });

        if (builders.length > 0) {
            var data = [];
            var reepaterData = builders[0].data;

            $.each(newData, function (idx, data) {

                var isExitData = reepaterData.filter(function (oldData) {
                    return oldData.MasterNetworkName == data.MasterNetworkName;
                });

                if (isExitData.length > 0) {
                    data = isExitData[0];
                }
            });

            currentInstance.setRepeaterDataProperty(currentInstance.mappedWithDatasource.TargetParent, newData);
            if (builders != null && builders.length > 0) {
                var builder = builders[0];
                builder.data = newData;
                $(builder.gridElementIdJQ).jqGrid("GridUnload");
                builder.columnNames = [];
                builder.columnModels = [];
                builder.build();
            }
        }
    }
}

manualDataSourceMappingDialogAG.prototype.applySelectedData = function (gridData) {
    var currentInstance = this;
    var rowData = new Array();
    if (currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.COMMERCIALMEDICAL.LimitInformationDetailMLServiceList) {
        rowData = currentInstance.repeaterBuilder.data;
    }
    else {
        rowData = GridApi.getCurrentGridInstance(currentInstance.repeaterBuilder).gridOptions.rowData;
    }

    var IsKeyElement = currentInstance.dataSourceMapping.Mappings.filter(function (ct) {
        return ct.IsKey == true;
    });

    if (currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.hasExpressionBuilderRule(currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId) && currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.LimitSummary) {
        if (rowData.length == 1) {
            if (rowData[0].BenefitCategory1 == "" && rowData[0].BenefitCategory2 == "" && rowData[0].BenefitCategory3 == "") {
                rowData = [];
            }
        }
    }


    if (currentInstance.caption == "Add/Remove Services" || currentInstance.caption == "ANOC Benefits Compare") {
        $.each(gridData, function (idx, data) { 
            data.IsSelect = "No";            
        });
    }
    else {
        $.each(gridData, function (idx, data) {
            var selectedData = rowData.filter(function (row) {
                var count = 0;
                $.each(IsKeyElement, function (idx, mapEle) {
                    if (row[mapEle.TargetElement] != null && row[mapEle.TargetElement] != undefined && data[mapEle.TargetElement] != null && data[mapEle.TargetElement] != null) {
                        if (row[mapEle.TargetElement].toString().trim().replace("&amp;", "&") == data[mapEle.TargetElement].toString().trim().replace("&amp;", "&")) {
                            count++;
                        }
                    }
                });
                if (count == IsKeyElement.length)
                    return row;
            });
            if (selectedData.length > 0) {
                data.IsSelect = "Yes";
            }
            else {
                data.IsSelect = "No";
            }
        });
    }
}

manualDataSourceMappingDialogAG.prototype.IsIncludeChild = function (elementMapping) {

    var IsInculde = elementMapping.filter(function (ele) {
        return ele.IncludeChild == true
    });
    if (IsInculde.length > 0)
        return true
    else
        return false;
}

manualDataSourceMappingDialogAG.prototype.AddSelectedRowData = function (repeaterSelectedData, selecteData) {
    var currentInstance = this;
    var repeaterData = {};

    for (var idx = 0; idx < currentInstance.repeaterBuilder.columnModels.length; idx++) {
        if (currentInstance.repeaterBuilder.columnModels[idx].editor != undefined) {
            var defaultValue = "";
            if (currentInstance.repeaterBuilder.columnModels[idx].dataIndx.substring(0, 4) != 'INL_') {
                if (currentInstance.repeaterBuilder.columnModels[idx].editor.defaultValue != null && currentInstance.repeaterBuilder.columnModels[idx].editor.defaultValue != undefined) {
                    defaultValue = currentInstance.repeaterBuilder.columnModels[idx].editor.defaultValue;
                }
                repeaterData[currentInstance.repeaterBuilder.columnModels[idx].dataIndx] = defaultValue;
            }
        }
        else {
            repeaterData[currentInstance.repeaterBuilder.columnModels[idx].dataIndx] = "";
        }
    }


    $.each(currentInstance.dataSourceMapping.Mappings, function (idx, mapEle) {
        repeaterData[mapEle.TargetElement] = selecteData[mapEle.TargetElement];
    });

    if (currentInstance.repeaterBuilder.design.ChildDataSources != null && Array.isArray(currentInstance.repeaterBuilder.design.ChildDataSources) && currentInstance.repeaterBuilder.design.ChildDataSources.length > 0 && currentInstance.repeaterBuilder.design.ChildDataSources[0].DisplayMode == "Child") {
        var childDatasource = currentInstance.repeaterBuilder.design.ChildDataSources[0];
        repeaterData[childDatasource.DataSourceName] = [];

        //var childRow = {};
        //$.each(childDatasource.Mappings, function (idx, elm) {
        //    childRow[elm.TargetElement] = "";
        //    // row[childDatasource.DataSourceName][idx][elm.TargetElement] = "";
        //})

        //row[childDatasource.DataSourceName] = childRow;
    }

    if (currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.SlidingCostShareInformation) {
        var ebInstance = currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension;
        var data = currentInstance.repeaterBuilder.data.filter(function (dt) {
            return dt[ebInstance.KeyName.BenefitCategory1] == selecteData[ebInstance.KeyName.BenefitCategory1]
            && dt[ebInstance.KeyName.BenefitCategory2] == selecteData[ebInstance.KeyName.BenefitCategory2]
            && dt[ebInstance.KeyName.BenefitCategory3] == selecteData[ebInstance.KeyName.BenefitCategory3]
        })
        if (data.length != 0) {
            repeaterData[ebInstance.KeyName.IntervalNumber] = data[0][ebInstance.KeyName.IntervalNumber];
            repeaterData[ebInstance.repeaterElementName.BenefitPeriod] = data[0][ebInstance.repeaterElementName.BenefitPeriod];
            repeaterData[ebInstance.repeaterElementName.IsthisBenefitUnlimited] = data[0][ebInstance.repeaterElementName.IsthisBenefitUnlimited];
        }
    }

    /*if (currentInstance.dataSourceMapping.DisplayMode == "Child" && currentInstance.dataSourceMapping.SourceParent == "ServicesSection.ServicesList" && currentInstance.dataSourceMapping.TargetParent == "Limits.FacetsLimits.LimitServicesLTSE") {
        repeaterData["WeightedCounter"] = "100";
    }*/


    //for add inline element data
    if (currentInstance.childDatasource != null) {
        var inlineDataSource = currentInstance.childDatasource.filter(function (ct) { return ct.DisplayMode == "In Line" })
        if (inlineDataSource.length > 0) {
            var row = currentInstance.repeaterBuilder.data[0];
            var inLineRows = inlineDataSource[0].GroupHeader.length;
            repeaterData[inlineDataSource[0].DataSourceName] = [];
            for (var i = 0; i < inLineRows; i++) {
                var rowData = inlineDataSource[0].GroupHeader[i];
                $.each(inlineDataSource, function (idx, mapEle) {
                    var dataSource = {};
                    var titleText = '';

                    $.each(inlineDataSource[0].Mappings, function (idx, mapEle) {
                        var element = currentInstance.repeaterBuilder.getElementDesign(mapEle.TargetElement);
                        if (element.Visible == false) {
                            dataSource[mapEle.TargetElement] = rowData[mapEle.TargetElement];
                        }
                        else {
                            if ((currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.benefitReviewGrid)) {
                                dataSource[mapEle.TargetElement] = "Not Applicable"
                            }
                            else {
                                dataSource[mapEle.TargetElement] = "";
                            }
                        }
                    });
                    repeaterData[inlineDataSource[0].DataSourceName].push(dataSource);
                });
            }
        }

        if (currentInstance.repeaterBuilder.expressionBuilder.hasExpressionBuilderRule(currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId)
            && currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.benefitReviewGrid) {
            //Set default value in BRG for column 'Minimum Copay', 'Maximum Copay', Minimum Coinsurance, 'Maximum Coinsuarnce'
            if ((currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.benefitReviewGrid)
                && (selecteData.BenefitCategory1 == "Preventive and Other Defined Supplemental Services"
                && selecteData.BenefitCategory2 == "Medicare-covered Zero Dollar Preventive Services"
                && selecteData.BenefitCategory3 == "")
                || (selecteData.BenefitCategory1 == "Hospice Care" && selecteData.BenefitCategory2 == "" && selecteData.BenefitCategory3 == "")) {

                $.each(repeaterData[inlineDataSource[0].DataSourceName], function (idx, value) {
                    repeaterData.IQMedicareNetWorkList[idx].MinimumCopay = "0";
                    repeaterData.IQMedicareNetWorkList[idx].MaximumCopay = "0";
                    repeaterData.IQMedicareNetWorkList[idx].MinimumCoinsurance = "0";
                    repeaterData.IQMedicareNetWorkList[idx].MaximumCoinsuarnce = "0";
                });
            }
        }
        //Set default End
        //for add child element data
        var childDataSource = currentInstance.childDatasource.filter(function (ct) { return ct.DisplayMode == "Child" });
        if (childDataSource.length > 0 && currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.BUSINESS.ServiceGroupDetails) {
            for (var key in selecteData) {
                if (key == childDataSource[0].DataSourceName) {
                    var childRepeaterData = selecteData[key];
                    repeaterData[key] = [];
                    $.each(childRepeaterData, function (idx, mappingElement) {
                        var childDAta = {};
                        $.each(currentInstance.childDatasource[0].Mappings, function (id, mappEle) {
                            if (mappEle.TargetElement == "SelectServices")
                                childDAta[mappEle.TargetElement] = "Yes";
                            else
                                childDAta[mappEle.TargetElement] = mappingElement[mappEle.SourceElement];
                        });
                        repeaterData[key].push(childDAta);
                    });
                }
            }
        }
    }
    repeaterSelectedData.push(repeaterData);
    var length = currentInstance.repeaterBuilder.data.length;
    //$(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('addRowData', (length - 1), repeaterData);
}

manualDataSourceMappingDialogAG.prototype.setRepeaterDataProperty = function (fullName, data) {
    var currentInstance = this;
    var formData;
    var dataSourceName;
    var fullpath = fullName.split('.');
    if (currentInstance.repeaterBuilder.gridType == "child") {
        dataSourceName = fullpath[fullpath.length - 1];
        fullpath.pop();
    }
    for (var index = 0; index < fullpath.length; index++) {
        if (index == 0) {
            formData = currentInstance.repeaterBuilder.formInstanceBuilder.formData[fullpath[index]];
        }
        else if (index == (fullpath.length - 1)) {
            if (currentInstance.repeaterBuilder.gridType == "parent") {
                formData[fullpath[index]] = data;
            }
            else {
                $.each(formData[fullpath[index]], function (id, row) {
                    if (row.RowIDProperty == parseInt(currentInstance.repeaterBuilder.parentRowId)) {
                        row[dataSourceName] = data;
                    }
                });
            }
        }
        else {
            formData = formData[fullpath[index]];
        }
    }
}

manualDataSourceMappingDialogAG.prototype.getSourceDatafromCurrentInstance = function (repeaterFullName, sourceMappingData) {
    var currentInstance = this;

    var reepaterData = currentInstance.repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (ct) {
        return ct.fullName == repeaterFullName;
    });

    if (reepaterData.length > 0) {
        ajaxDialog.hidePleaseWait();
        currentInstance.sourceDataList = currentInstance.filterManualDataSource(currentInstance, reepaterData[0].data);
        currentInstance.setTargetRepeaterData(repeaterFullName, reepaterData[0].data, sourceMappingData);
    }

}

manualDataSourceMappingDialogAG.prototype.setTargetRepeaterData = function (repeaterFullName, sourceDataList, sourceMappingData) {
    var currentInstance = this;
    var fullName = sourceMappingData[0].SourceParent.split(".");
    var dialogTitle = currentInstance.caption + ' Map ' + fullName[1] + ' Data';

    $(currentInstance.elementIDs.repeaterManualDialogJQ).dialog('option', 'title', dialogTitle);
    $(currentInstance.elementIDs.repeaterManualDialogJQ).dialog("open");

    if (null != sourceDataList && sourceDataList.length > 0) {
        // currentInstance.sourceDataList = currentInstance.setCustomRuleOnNetworkList(sourceDataList);
        //Create Layout of dialog
        currentInstance.applyDialogLayout(sourceMappingData[0].Mappings, fullName[1], currentInstance.sourceDataList);
        $(currentInstance.elementIDs.manualMassageDialogJQ).addClass('hide');
        $(currentInstance.elementIDs.manualDialogButtonJQ).removeClass('hide');
        $(".panel-body").parent(currentInstance.elementIDs.manualDialogJQ).removeClass('hide');
    }
    else {
        $(currentInstance.elementIDs.manualMassageDialogJQ).removeClass('hide');
        $(currentInstance.elementIDs.manualDialogButtonJQ).addClass('hide');
        $(".panel-body").parent(currentInstance.elementIDs.manualDialogJQ).addClass('hide');
    }
}

manualDataSourceMappingDialogAG.prototype.addLimitDialogMethods = function () {
    var currentInstance = this;

    return {
        initalizeAddLimitDialog: function (standardlimitDesignData) {
            $(currentInstance.addLimitElementIDs.planLevelLimitJQ).dialog({
                autoOpen: false,
                resizable: false,
                closeOnEscape: true,
                height: 'auto',
                width: 700,
                height: 200,
                modal: true,
                position: ['middle', 100],
            });
            $(currentInstance.addLimitElementIDs.planLevelLimitJQ).dialog('option', 'title', "Add Limit");
            $(currentInstance.addLimitElementIDs.planLevelLimitJQ).dialog("open");

            $(currentInstance.addLimitElementIDs.PlanLimitSaveButtonJQ).off("click");
            $(currentInstance.addLimitElementIDs.PlanLimitSaveButtonJQ).on("click", function () {
                currentInstance.planLimit.addPlanLimit();
            });

            $(currentInstance.addLimitElementIDs.PlanLimitCancelButtonJQ).off("click");
            $(currentInstance.addLimitElementIDs.PlanLimitCancelButtonJQ).on("click", function () {
                $(currentInstance.addLimitElementIDs.planLevelLimitJQ).dialog("close");
            });

            currentInstance.planLimit.formatDropdownTextbox();
            currentInstance.planLimit.initalizeControle(standardlimitDesignData);
            if (currentInstance.isEditable == "False" || currentInstance.hasDisabledParent == true) {
                $(currentInstance.addLimitElementIDs.PlanLimitSaveButtonJQ).attr('disabled', 'disabled');
            }
            else {
                $(currentInstance.addLimitElementIDs.PlanLimitSaveButtonJQ).removeAttr('Disabled', 'disabled');
            }
        },

        addPlanLimit: function () {
            var limitDescription = $(currentInstance.addLimitElementIDs.limitDescriptionJQ).val();
            var amount = $(currentInstance.addLimitElementIDs.amountDDLJQ).val();
            var frequency = $(currentInstance.addLimitElementIDs.frequencyDDLJQ).val();
            var limitType = $(currentInstance.addLimitElementIDs.limitTypeDDLJQ).val();

            var valid = false
            if (currentInstance.planLimit.validate()) {

                var rowIds = $(currentInstance.elementIDs.manualGridJQ).getDataIDs();

                var rowId = rowIds.length + 1;
                var newLimit = {
                    LimitDescription: limitDescription,
                    IsSelect: "Yes",
                    ID: rowId,
                }

                var row = {
                    LimitDescription: limitDescription,
                    Amount: amount,
                    LimitType: limitType,
                    Frequency: frequency,
                }
                currentInstance.planLimitList.push(row);
                currentInstance.sourceDataList.push(row);
                $(currentInstance.elementIDs.manualGridJQ).jqGrid('addRowData', rowId, newLimit);
                $("#checkbox_" + rowId + "").focus();
                $(currentInstance.addLimitElementIDs.planLevelLimitJQ).dialog("close");
            }
        },

        validate: function () {
            var valid = false;
            if ($(currentInstance.addLimitElementIDs.limitDescriptionJQ).val() == "") {
                $(currentInstance.addLimitElementIDs.limitDescriptionJQ).parent().addClass('has-error');
                $(currentInstance.addLimitElementIDs.limitDescriptionHelpBlock).text("Please add Limit Description");
                valid = false;
            }
            else {
                $(currentInstance.addLimitElementIDs.limitDescriptionJQ).parent().removeClass('has-error');
                $(currentInstance.addLimitElementIDs.limitDescriptionHelpBlock).text('');
                valid = true;
            }

            //var gridData = $(currentInstance.elementIDs.manualGridJQ).jqGrid("getGridParam", "data");
            var gridData = $(currentInstance.elementIDs.manualGridJQ).pqGrid("option", "dataModel.data");

            var addLimit = $(currentInstance.addLimitElementIDs.limitDescriptionJQ).val();
            var isExist = gridData.filter(function (row, idx) {
                return row.LimitDescription == addLimit;
            });

            if (isExist.length > 0) {
                valid = false;
                messageDialog.show("Limit Description is already exists.");
            }
            return valid;
        },

        initalizeControle: function (standardlimitDesignData) {
            $(currentInstance.addLimitElementIDs.limitDescriptionJQ).val("");
            $(currentInstance.addLimitElementIDs.limitDescriptionJQ).parent().removeClass('has-error');
            $(currentInstance.addLimitElementIDs.limitDescriptionHelpBlock).text('');

            $(currentInstance.addLimitElementIDs.amountDDLJQ).empty();
            $(currentInstance.addLimitElementIDs.amountDDLJQ).append("<option value=''></option>;<option value='newItem'>Enter Unique Response</option>");

            var amountList = currentInstance.planLimit.getDropDownvalue("Amount", standardlimitDesignData);
            for (i = 0; i < amountList.length; i++) {
                $(currentInstance.addLimitElementIDs.amountDDLJQ).append("<option value='" + amountList[i].ItemValue + "'>" + amountList[i].ItemValue + "</option>");
            }

            $(currentInstance.addLimitElementIDs.limitTypeDDLJQ).empty();
            $(currentInstance.addLimitElementIDs.limitTypeDDLJQ).append("<option value=''></option>;<option value='newItem'>Enter Unique Response</option>");

            var limitTypeList = currentInstance.planLimit.getDropDownvalue("LimitType", standardlimitDesignData);
            for (i = 0; i < limitTypeList.length; i++) {
                $(currentInstance.addLimitElementIDs.limitTypeDDLJQ).append("<option value='" + limitTypeList[i].ItemValue + "'>" + limitTypeList[i].ItemValue + "</option>");
            }

            $(currentInstance.addLimitElementIDs.frequencyDDLJQ).empty();
            $(currentInstance.addLimitElementIDs.frequencyDDLJQ).append("<option value=''></option>;<option value='newItem'>Enter Unique Response</option>");

            var frequencyList = currentInstance.planLimit.getDropDownvalue("Frequency", standardlimitDesignData);
            for (i = 0; i < frequencyList.length; i++) {
                $(currentInstance.addLimitElementIDs.frequencyDDLJQ).append("<option value='" + frequencyList[i].ItemValue + "'>" + frequencyList[i].ItemValue + "</option>");
            }
        },

        getDropDownvalue: function (name, design) {
            var items = design.Elements.filter(function (ct) {
                if (ct.GeneratedName === name)
                    return ct.Items;
            });
            items = items[0].Items;
            return items;
        },

        formatDropdownTextbox: function () {
            $('.ddt-dropdown').unbind('change');
            $(".ddt-dropdown").change(function () {
                if ($(this).val() == 'newItem') {
                    $(this).val('SelectOne');
                    $(this).parent().find('.ddt-textbox').val('').toggle().focus();
                    $(this).toggle();
                }
            });
            //$('.ddt-textbox').unbind('focusout');
            $('.ddt-textbox').unbind('focusout');
            $('.ddt-textbox').on('focusout', function (e) {
                $(this).trigger("keyup");
                $(this).toggle();
                $(this).parent().find('.ddt-dropdown').toggle();
            });
            $('.ddt-textbox').unbind('keyup');
            $('.ddt-textbox').on('keyup', function (e) {
                var newValue = $(this).val();
                var dropdownTextboxControl = $(this).parent().find('.ddt-dropdown');
                var dropdownOptions = $(this).parent().find('.ddt-dropdown option');
                var stringUtility = new globalutilities();
                if (!stringUtility.stringMethods.isNullOrEmpty(newValue)) {

                    //check if unique response is the same item as Items from document design
                    var existingItem = false;
                    $.each(dropdownOptions, function (key, elem) {
                        if (elem.innerText.toUpperCase() === newValue.toUpperCase()) {
                            existingItem = true;
                            dropdownTextboxControl.val(elem.innerText);
                        }
                    });
                    if (existingItem == false) {
                        if ($(this).parent().find('.ddt-dropdown option').hasClass('non-standard-optn') == true) {
                            $(this).parent().find('.ddt-dropdown option.non-standard-optn').remove();
                        }
                        else {
                            $('<option value="' + newValue + '" class="non-standard-optn">' + newValue + '</option>').insertBefore($(this).parent().find('.ddt-dropdown option').last());
                        }
                    }
                    else {
                        $(this).parent().find('.ddt-dropdown option.non-standard-optn:not(:selected)').remove();
                    }
                    dropdownTextboxControl.val(newValue);
                }
                else {
                    dropdownTextboxControl.val('SelectOne');
                }
            });

        }

    }
}

manualDataSourceMappingDialogAG.prototype.updateCurrentRepeater = function (currentInstance, selectedData, repeaterSelectedData, oldData) {
    for (j = 0; j < selectedData.list.length ; j++) {
        if (currentInstance.repeaterBuilder.data.length > 0) {
            for (i = 0; i < currentInstance.repeaterBuilder.data.length; i++) {
                rowCount = 0;
                count = 0;
                for (var key in currentInstance.repeaterBuilder.data[i]) {
                    if (selectedData.list[j][key] != undefined && $.type(selectedData.list[j][key]) === "string") {
                        rowCount++;
                        if (selectedData.list[j][key] == currentInstance.repeaterBuilder.data[i][key]) {
                            count++;
                        }
                    }
                }
                if (rowCount == count) {
                    prevCount = i + 1;
                    var repeaterData = {};
                    $.each(currentInstance.repeaterBuilder.data[i], function (idx, ele) {
                        if (idx != 'RowIDProperty')
                            repeaterData[idx] = currentInstance.repeaterBuilder.data[i][idx];
                    });
                    repeaterSelectedData.push(repeaterData);
                    var length = currentInstance.repeaterBuilder.data.length;
                    //$(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('addRowData', (length - 1), repeaterData);
                    break;
                }
            }
            if (count < rowCount) {
                currentInstance.AddSelectedRowData(repeaterSelectedData, selectedData.list[j]);
            }
        }
        else {
            currentInstance.AddSelectedRowData(repeaterSelectedData, selectedData.list[j]);
        }
    }


   /* if (currentInstance.repeaterBuilder.expressionBuilder.hasExpressionBuilderRule(currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId)) {
        if (currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.BUSINESS.ServiceGroupDetails && repeaterSelectedData.length > 1) {
            messageDialog.show("Service Group Not Allowed more than one");
            $(currentInstance.elementIDs.repeaterManualDialogJQ).dialog("close");
            return false;
        }
        //customrule.loadCustomDataInRepeater.runCustomRuleForProduct(currentInstance.repeaterBuilder, repeaterSelectedData, currentInstance.dataSourceMapping);
    }

    if (currentInstance.repeaterBuilder.expressionBuilder.hasExpressionBuilderRule(currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId)) {
        if (currentInstance.repeaterBuilder.fullName ==
            currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.BUSINESS.ServiceGroupDetails) {
            var list = [];

            if (repeaterSelectedData.length != 0) {
                repeaterSelectedData[0]["MLServicesServicesList"].filter(function (dt) {
                    var isExist = list.filter(function (l) {
                        return dt["ServiceCategory1"] == l["ServiceCategory1"] && dt["ServiceCategory2"] == l["ServiceCategory2"] && dt["ServiceCategory3"] == l["ServiceCategory3"] &&
                       dt["BusinessPlaceofService"] == l["BusinessPlaceofService"]
                    })

                    if (isExist.length == 0) {
                        list.push(dt);
                    }

                })
                repeaterSelectedData[0]["MLServicesServicesList"] = list;
            }
        }
    }
    */
    if (repeaterSelectedData != undefined && repeaterSelectedData != null && repeaterSelectedData.length > 0) {
        for (var i = 0; i < repeaterSelectedData.length; i++) {
            repeaterSelectedData[i]["RowIDProperty"] = i;
        }
    }

    // if (repeaterSelectedData.length > 0) {
    //$(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('clearGridData');   
    //$(currentInstance.repeaterBuilder.gridElementIdJQ).pqGrid("option", "dataModel.data", repeaterSelectedData);
    //$(currentInstance.repeaterBuilder.gridElementIdJQ).pqGrid("refreshDataAndView");
    
    //$(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('setGridParam', { data: repeaterSelectedData }).trigger("reloadGrid");
    //}

    currentInstance.repeaterBuilder.data = [];
    //$(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid("GridUnload");
    $.each(repeaterSelectedData, function (idx, row) {
        currentInstance.repeaterBuilder.data.push(row);
        //Activity Log for Added Rows
        //if (!customrule.isActivityLogEntries) 
        {
            var resultRecordExits = currentInstance.IsRecordExists(row, oldData)
            if (resultRecordExits)
                currentInstance.repeaterBuilder.addEntryToAcitivityLogger(row.RowIDProperty, Operation.ADD);
        }
    });


    //Activity Log for Deleted Rows
    $.grep(oldData, function (el) {
        var resultRecordExits = currentInstance.IsRecordExists(el, currentInstance.repeaterBuilder.data)

        var OldValue = "";
        if (resultRecordExits) {
            $.each(currentInstance.repeaterBuilder.columnModels, function (index, element) {
                if (element.iskey == true) {
                    if (element.dataIndx.indexOf("_") >= 0) {
                        //if (element.dataIndx.substring(0, 4) == 'INL_') {
                        //    var keyarr = element.dataIndx.split('_');
                        //    if (keyarr.length == 4) {
                        //       // if (headercolumnname == keyarr[2]) {
                        //            var row = el;//currentInstance.data.filter(function (dt) { return dt.RowIDProperty == rowId });
                        //            if (row != undefined) {
                        //                var elementName = element.dataIndx.split("_");
                        //                var eleName = elementName[elementName.length - 1];
                        //                var value = row[currentInstance.repeaterBuilder.design.ChildDataSources[0].DataSourceName][elementName[elementName.length - 2]][eleName];
                        //                OldValue += value + " ";
                        //            }
                        //        //}
                        //    }
                        //}
                    }
                    else {
                        var row = el;//currentInstance.data.filter(function (dt) { return dt.RowIDProperty == rowId });
                        if (row != undefined) {
                            var value = row[element.dataIndx];
                            OldValue += value + " ";
                        }
                    }
                }
            });

            currentInstance.repeaterBuilder.addEntryToAcitivityLogger(el.RowIDProperty, Operation.DELETE, '', OldValue);
        }
    });


    currentInstance.setRepeaterDataProperty(currentInstance.repeaterBuilder.fullName, currentInstance.repeaterBuilder.data);

    //currentInstance.repeaterBuilder.columnNames = [];
    //currentInstance.repeaterBuilder.columnModels = [];        
    //currentInstance.repeaterBuilder.build();
    
    var gridOptions = GridApi.getCurrentGridInstance(currentInstance.repeaterBuilder).gridOptions;
    gridOptions.api.setRowData(currentInstance.repeaterBuilder.data);
    gridOptions.rowData = currentInstance.repeaterBuilder.data;
    gridOptions.api.refreshView();
    //gridOptions.api.refreshCells();

    if (currentInstance.isMappedwithOtherDataSource == true) {
        currentInstance.synchDataSourceMapping(repeaterSelectedData);
    }
}

manualDataSourceMappingDialogAG.prototype.filterManualDataSource = function (currentInstance, sourceData) {
    var sourceDataList = sourceData;
    var FilterParams = currentInstance.dataSourceMapping.Mappings.filter(function (dt) {
        if (dt.Filter != null && dt.Filter != undefined && dt.Filter != "") {
            return dt;
        }
    });
    if (FilterParams != undefined && FilterParams != null && FilterParams.length > 0) {
        sourceDataList = sourceData.filter(function (row) {
            var temp = row;
            $.each(FilterParams, function (ind, val) {
                if (temp[val.SourceElement] == val.Filter) {
                    temp = temp;
                }
                else {
                    temp = "";
                }
            });
            return temp == row;
        });
    }
    return sourceDataList;
}
var repeaterBuilderMedical = function (design, data, fullName, formInstanceId, gridType, parentRowId, formInstanceBuilder, rules, ruleProcessor) {
    this.design = design;
    this.data = data;
    this.fullName = fullName;
    this.formInstanceId = formInstanceId;
    this.rules = rules;
    this.gridType = gridType;
    this.parentRowId = parentRowId;
    this.formInstanceBuilder = formInstanceBuilder;
    this.formUtilities = new formUtilities(formInstanceId);
    this.customruleMedical = new customruleMedical();

    this.tenantId = formInstanceBuilder.tenantId;
    if (gridType == 'child') {
        this.ruleProcessor = ruleProcessor;
    }
    else {
        this.ruleProcessor = new repeaterRuleProcessor(formInstanceBuilder.formData, ruleProcessor, this);
    }
    this.columnNames = [];
    this.columnModels = [];
    this.gridElementId = null;
    this.gridElementIdJQ = null;
    this.lastSelectedRow = null;
    this.lastColumnSelected = null;
    this.groupHeaders = [];
    this.hasChildGrids = false;
    this.childGridBuilders = [];
    this.Isloaded = true;
    this.KeyProperty = 'RowIDProperty';
    this.gridEvents = this.gridMethods();
    this.URLs = {
        //tenantId,formInstanceId, formDesignVersionId, folderVersionId, formDesignId, fullName, sData
        getSourceFormInstanceData: "/FormInstance/getSourceFormInstanceData?tenantId={tenantId}&formInstanceId={formInstanceId}&formDesignVersionId={formDesignVersionId}&folderVersionId={folderVersionId}&formDesignId={formDesignId}&fullName={fullName}",
        getformInstanceRepeaterData: "/FormInstance/GetFormInstanceRepeaterData?formInstanceId={formInstanceId}&fullName={fullName}",
        checkDuplicateRowsWorker: "/Scripts/FolderVersion/journalNotify.js",
        checkDuplicateRepeaterRows: '/FormInstance/CheckDuplication'
    }

    this.rowDataBeforeEdit = undefined;
    this.hasChanges = false;
    this.isValidateDuplicate = false;
    this.loadFromServerObject = this.repeaterServerMethods();
    this.checkDuplicationRowsWorker = undefined;
    this.rowViewMode = true;
    this.isDataChangesOnTab = false;
    this.displayChildGridAsPopup = true;
    this.selectedRowId = null;
    this.tabClick = false;
    this.rowClick = false;
    this.browserInfo = getBrowserInfo();
}

repeaterBuilderMedical.prototype.build = function () {
    //set container element ID
    if (this.gridType == 'parent') {
        this.gridElementId = this.design.Name + this.formInstanceId;
        this.gridElementIdJQ = '#' + this.design.Name + this.formInstanceId;
    }
    else {
        this.gridElementId = this.design.Name;
        this.gridElementIdJQ = '#' + this.design.Name;
    }
    //generate columns
    var currentInstance = this;
    $.each(this.design.Elements, function (index, element) {
        if (element.Label != null && element.IsPrimary == true) {
            var isRequired = undefined;
            currentInstance.formInstanceBuilder.designData.Validations.filter(function (ct) {
                if (element.FullName == ct.FullName && ct.IsRequired == true) {
                    isRequired = true;
                }
            });
            var elementLabel = element.IsKey == true ? "<span class='primaryKey'/>" + element.Label : element.Label;
            
            if (isRequired == true) {
                currentInstance.columnNames.push(elementLabel + "<font color=red>*</font>");
            }
            else {
                currentInstance.columnNames.push(elementLabel);
            }
            
            currentInstance.columnModels.push(currentInstance.addColModel(element));
        }
    });

    //for manual record check for first time records didnot apply Inline or child records
    var isRepeaterData = true;
    if (this.design.PrimaryDataSource != null) {
        if (this.design.PrimaryDataSource.DataSourceModeType == "Manual") {
            isRepeaterData = currentInstance.isPrimaryDataExits();
        }
    }

    if (this.design.ChildDataSources != null) {
        $.each(currentInstance.design.ChildDataSources, function (idx, dataSource) {
            //register inline columns      
            if (dataSource.DisplayMode == 'In Line' && dataSource.Mappings != undefined && dataSource.Mappings.length > 0 && isRepeaterData) {
                if (currentInstance.data != null && currentInstance.data.length > 0) {

                    var row = currentInstance.data[0];
                    if (row != undefined && row[dataSource.DataSourceName]) {

                        var inLineRows = row[dataSource.DataSourceName].length;
                        for (var idx = 0; idx < inLineRows; idx++) {
                            var rowData = row[dataSource.DataSourceName][idx];
                            var startIndex = currentInstance.columnModels.length;
                            var titleText = '';
                            var ColModelElementArray = new Array();
                            var coveragelevelList;
                            $.each(dataSource.Mappings, function (index, mapping) {
                                var flag = true;
                                var element = currentInstance.getElementDesign(mapping.TargetElement);
                                if (element != undefined) {
                                    if (currentInstance.customruleMedical.hasCustomRules(currentInstance.formInstanceBuilder.formDesignId) && currentInstance.customruleMedical.isCustomHeaderName(element.FullName)) {
                                        if (currentInstance.formInstanceBuilder.formData["CostShare"] != undefined && currentInstance.formInstanceBuilder.formData["CostShare"] != null) {
                                            coveragelevelList = currentInstance.formInstanceBuilder.formData.CostShare.CoverageLevel.CoverageLevelList;
                                            currentInstance.customruleMedical.updateBenefitReviewGridColumnName(element, coveragelevelList, element.FullName);
                                            flag = false;
                                            if (currentInstance.fullName === "BenefitReview.BenefitReviewGrid") {
                                                ColModelElementArray.push({ CoverageType: currentInstance.customruleMedical.getCoverageType(element.FullName), Label: element.Label, ColModel: currentInstance.addColModel(element, 'INL_' + dataSource.DataSourceName, idx) });
                                            }
                                        }
                                    }
                                    else {
                                        if (element.Visible == false) {
                                            if (titleText == '') {
                                                titleText = rowData[mapping.TargetElement];
                                            }
                                            else {
                                                if (rowData[mapping.TargetElement] != null && rowData[mapping.TargetElement] != '') {
                                                    titleText = titleText + " - " + rowData[mapping.TargetElement];
                                                }
                                            }
                                        }
                                    }
                                }
                                // add other ColModel when Flag is true
                                if (flag && currentInstance.fullName == currentInstance.customruleMedical.benefitReviewGrid) {
                                    currentInstance.columnNames.push(element.Label);
                                    currentInstance.columnModels.push(currentInstance.addColModel(element, 'INL_' + dataSource.DataSourceName, idx));
                                }
                                else if (currentInstance.fullName != currentInstance.customruleMedical.benefitReviewGrid) {
                                    currentInstance.columnNames.push(element.Label);
                                    currentInstance.columnModels.push(currentInstance.addColModel(element, 'INL_' + dataSource.DataSourceName, idx));
                                }
                            });

                            //Add Deductibles & Out of Pocket as per Coverage Level List
                            if (coveragelevelList != undefined && currentInstance.fullName == "BenefitReview.BenefitReviewGrid") {
                                repeaterBuilderMedical.prototype.benefitReviewGridDeductiblesSequenceManager(coveragelevelList, ColModelElementArray, currentInstance);
                            }

                            currentInstance.groupHeaders.push({
                                startColumnName: 'INL_' + dataSource.DataSourceName + '_' + idx + '_' + dataSource.Mappings[0].TargetElement,
                                numberOfColumns: dataSource.Mappings.length,
                                startIndex: startIndex,
                                titleText: titleText
                            });
                        }
                    }
                }
            }

            if (dataSource.DisplayMode == 'Child' && dataSource.Mappings != undefined && dataSource.Mappings.length > 0 && isRepeaterData) {
                currentInstance.hasChildGrids = true;
            }
        });
    }

    //add key column
    currentInstance.columnNames.push(currentInstance.KeyProperty);
    currentInstance.columnModels.push(currentInstance.addKeyColModel());
    //render grid

    //Add extra column to show expand/collapse icon
    if (currentInstance.hasChildGrids && currentInstance.displayChildGridAsPopup) {
        currentInstance.columnNames.splice(0, 0, "");
        currentInstance.columnModels.splice(0, 0, currentInstance.addExpandCollapseColModel());
    }
    //End

    if (currentInstance.hasChildGrids && currentInstance.displayChildGridAsPopup) {
        currentInstance.registerExpandCollapseEvents();
    }

    if (currentInstance.formInstanceBuilder.formDesignId == FormTypes.HSBMASTERLISTFORMID && currentInstance.design.LoadFromServer == true) {
        currentInstance.data = new Array();
        currentInstance.checkDuplicationRowsWorker = new Worker(currentInstance.URLs.checkDuplicateRowsWorker);
    }
    //Admin Fee Include Fee Yes records Repeater 
    if (currentInstance.customruleMedical.adminFeeTotals.isAdminFeeTotals(currentInstance)) {
        currentInstance.customruleMedical.adminFeeTotals.populateIncludeFeeYesRep(currentInstance);
    }
    //Admin Fee Totals - Rate Type Wise Sum repeater
    if (currentInstance.customruleMedical.adminFeeTotals.isRateTypeSumRep(currentInstance)) {
        currentInstance.customruleMedical.adminFeeTotals.getUpdatedDataForSumRep(currentInstance);
    }

    this.render();
    if (this.gridType == 'child') {
        currentInstance.gridEvents.processRules();
    }
}

repeaterBuilderMedical.prototype.getElementDesign = function (elementName) {
    var elementDesign;
    $.each(this.design.Elements, function (index, element) {
        if (element.GeneratedName == elementName) {
            elementDesign = element;
        }
    });
    return elementDesign;
}

repeaterBuilderMedical.prototype.addColModel = function (element, dataSourceName, idx) {

    var genName = element.GeneratedName;
    if (dataSourceName != undefined) {
        genName = dataSourceName + '_' + idx + '_' + genName;
    }
    var colModel = {
        name: genName,
        index: genName,
        hidden: !element.Visible,
        sortable: true,
        sorttype: this.getSortType(element),
        editable: this.getEditable(element),
        edittype: this.getEditType(element),
        editoptions: this.getEditOptions(element),
        align: this.getColumnAlignment(element),
        width: this.getColumnWidth(element),
        iskey: element.IsKey,
        //fixed: true,
    };
    if (element.Type == 'checkbox') {
        colModel.formatter = this.chkValueImageFmatter;
        colModel.unformat = this.chkValueImageUnFormat;
    }
    else if (element.IsDropDownTextBox == true) {
        colModel.formatter = this.highlightDropdownTextboxFmatter;
        colModel.unformat = this.highlightDropdownTextboxUnformat;

    }

    if (this.customruleMedical.hasCustomRules(this.formInstanceBuilder.formDesignId) && element.FullName == "BenefitReview.BenefitReviewGrid.Limits") {
        var formatterInfo = this.customruleMedical.getFormatterForBenefitReviewGrid(colModel.index, element.Label, this.formInstanceBuilder, dataSourceName, idx);

        if (formatterInfo) {
            colModel.formatter = formatterInfo.formatter;
            colModel.editable = false;
            colModel.unformat = this.customruleMedical.getUnforamttersForBenefirReviewGrid().unformat;
        }
    }

    //Formatter for Admin Fee Totals - Rate Type Wise Sum Repeater
    if (this.customruleMedical.adminFeeTotals.isRateTypeSumRep(this) && element.FullName == "Fees.FeeTotals.FeeTotalsbyRateType.Sum") {
        var formatterInfo = this.customruleMedical.adminFeeTotals.getFormatterForRateTypeWiseSumRep(colModel.index, element.Label, this.formInstanceBuilder, dataSourceName, idx);

        if (formatterInfo) {
            colModel.align = 'center';
            colModel.formatter = formatterInfo.formatter;
            colModel.editable = false;
        }
        colModel.unformat = this.customruleMedical.adminFeeTotals.getUnforamttersForRateTypeWiseSumRep().unformat;
    }

    return colModel;
}

repeaterBuilderMedical.prototype.addKeyColModel = function () {
    var colModel = {
        name: this.KeyProperty,
        index: this.KeyProperty,
        key: true,
        hidden: true
    };
    return colModel;

}

repeaterBuilderMedical.prototype.addExpandCollapseColModel = function () {
    var colModel = {
        name: "",
        index: "expandCollapse",
        key: false,
        editable: false,
        search: false,
        width: "20px",
        formatter: this.expandCollapseFmatter
    };
    return colModel;
}

repeaterBuilderMedical.prototype.expandCollapseFmatter = function (cellvalue, options, rowObject) {
    return '<a style="cursor:pointer;"><span align="center" class="ui-icon ui-icon-extlink" style="margin:auto" ></span></a>';
}

repeaterBuilderMedical.prototype.chkValueImageFmatter = function (cellvalue, options, rowObject) {

    if (cellvalue == "Yes" || cellvalue == "True") {
        return '<span align="center" class="ui-icon ui-icon-check" style="margin:auto" ></span>';
    }
    else {
        return '<span align="center" class="ui-icon ui-icon-close" style="margin:auto" ></span>';
    }
}

repeaterBuilderMedical.prototype.chkValueImageUnFormat = function (cellvalue, options, cell) {

    var checked = $(cell).children('span').attr('class');

    if (checked == "ui-icon ui-icon-check")
        return 'Yes';
    else
        return 'No';
}

repeaterBuilderMedical.prototype.highlightDropdownTextboxFmatter = function (cellvalue, options, rowObject) {
    var stdOptions = options.colModel.editoptions.stdOptions;

    if ((cellvalue === undefined || cellvalue === null || cellvalue === "")) {
        return "";
    }
    else {
        var isCellvalueExist = false;
        if (!(stdOptions === "undefined" || stdOptions === "")) {
            for (i = 0; i < stdOptions.length; i++) {
                if (stdOptions[i].ItemValue.trim() === cellvalue.trim()) {
                    isCellvalueExist = true;
                    break;
                }
            }
        }

        if (isCellvalueExist == false) {
            return '<div class="non-standard-optn" style="hight:100%; width:100%;">' + cellvalue + '</div>';
        }
        else {
            return cellvalue;
        }
    }

}

repeaterBuilderMedical.prototype.highlightDropdownTextboxUnformat = function (cellvalue, options, cell) {

    return cellvalue;
}

repeaterBuilderMedical.prototype.sortOptions = function (items) {
    return sortDropDownElementItems(items);
}

repeaterBuilderMedical.prototype.getOptions = function (element, isSortRequired) {
    var items = element.Items;
    //sorts options in ascending order
    if (isSortRequired == true) {
        items = this.sortOptions(items);
    }

    var options = "";
    if (items != null && items.length > 0) {
        options = options + Validation.selectOne + ':' /*+ Validation.selectOne */ + ';';
        if (this.customruleMedical.hasMasterList(this.formInstanceBuilder.formDesignId) && this.customruleMedical.isMedicalServiceApplyCopayTypeDropdown(element.FullName)) {
            options = this.customruleMedical.bindApplyCopayTypeToMedicalService(options);
        }
        for (var idx = 0; idx < items.length; idx++) {
            if (items[idx].ItemValue != '') {
                options = options + items[idx].ItemValue + ':' + items[idx].ItemValue;
                if (idx < items.length - 1) {
                    options = options + ';';
                }
            }
        }
    }
    return options;
}

repeaterBuilderMedical.prototype.getColumnWidth = function (element) {
    if (element.Type === 'text') {
        return '250px';
    }
    else {
        return '150px';
    }
}

repeaterBuilderMedical.prototype.getColumnAlignment = function (element) {
    var align = 'center';

    if (element.Type == 'checkbox') {
        align = 'center';
        return align;
    }

    switch (element.DataType) {
        case 'int':
        case 'float':
            align = 'right';
            break;
        case 'date':
            align = 'center';
            break;
        case 'string':
            align = 'left';
            break;
        default:
            align = 'center';
    }
    //TODO: fix after finalization of grid layout
    return align;
}

repeaterBuilderMedical.prototype.getSortType = function (element) {
    var sortType = '';
    switch (element.DataType) {
        case 'int':
        case 'float':
            sortType = 'int';
            break;
        case 'date':
            sortType = 'date';
            break;
        case 'bool':
            sortType = 'bool';
            break;
        default:
            sortType = '';
    }
    if (element.Type == 'select') {
        sortType = '';
    }
    //TODO: fix after finalization of grid layout
    return sortType;
}

repeaterBuilderMedical.prototype.getEditType = function (element) {
    var editType = "";
    switch (element.Type) {
        case "text":
            if (element.Multiline == true) {
                editType = "textarea";
            }
            else {
                editType = "text";
            }
            break;
        case "select":
        case "SelectInput":
            editType = "select";
            break;
        case "checkbox":
            editType = "checkbox";
            break;
        case "calendar":
            editType = "text";
            break;
    }
    return editType;
}

repeaterBuilderMedical.prototype.getEditOptions = function (element) {
    var editOptions = {};
    switch (element.Type) {
        case "text":
            if (element.MaxLength > 0) {
                editOptions["maxlength"] = element.MaxLength;
            }
            if (element.Multiline == true) {
                editOptions["rows"] = 50;
                editOptions["cols"] = 10;
            }
            editOptions["defaultValue"] = element.DefaultValue;
            break;
        case "select":
        case "SelectInput":
            if (element.IsDropDownTextBox == true) {
                editOptions["value"] = this.getDropdownTextboxOptions(element.Items, element.IsSortRequired);
                editOptions["formatter"] = 'select';
                editOptions["stdOptions"] = element.Items;

            } else {
                editOptions["value"] = this.getOptions(element, element.IsSortRequired);
                editOptions["formatter"] = 'select';
            }
            editOptions["defaultValue"] = element.DefaultValue;
            break;
        case "checkbox":
            editOptions["value"] = "Yes:No";
            if (element.DefaultValue == "True") {
                editOptions["defaultValue"] = "Yes";
            }
            else {
                editOptions["defaultValue"] = "No";
            }
            break;
        case "calendar":
            editOptions["defaultValue"] = element.DefaultValue;
            break;
    }
    return editOptions;
}

repeaterBuilderMedical.prototype.getEditable = function (element) {
    var editable = true;
    if ((element.Type == 'label' && element.IsLabel == true) || element.Enabled == false) {
        editable = false;
    }
    return editable;
}

repeaterBuilderMedical.prototype.render = function () {
    var currentInstance = this;
    var url = "";
    if (currentInstance.formInstanceBuilder.formDesignId == FormTypes.HSBMASTERLISTFORMID && currentInstance.design.LoadFromServer == true) {
        url = currentInstance.URLs.getformInstanceRepeaterData.replace(/\{formInstanceId\}/g, currentInstance.formInstanceId).replace(/\{fullName\}/g, currentInstance.fullName);
    }
    $(currentInstance.gridElementIdJQ).parent().append("<div id='p" + currentInstance.gridElementId + "'></div>");
    var pagerElement = '#p' + currentInstance.gridElementId;
    var altClass = "alternate";
    if (currentInstance.formInstanceBuilder.formDesignId == FormTypes.MEDICALFORMDESIGNID && currentInstance.fullName == "BenefitReview.BenefitReviewGrid") {
        altClass="";
    }
    $(currentInstance.gridElementIdJQ).jqGrid({
        url: url,
        datatype: currentInstance.getDataTypeProperty(),
        height: 'auto',
        autoWidth: true,
        altRows: true,
        cache: false,
        altclass: altClass,
        //rownumbers: true,
        //celledit: true,
        //loadonce: true,
        viewrecords: true,
        data: currentInstance.prepareGridData(),
        rowNum: 25,
        ignoreCase: true,
        pager: pagerElement,
        headertitles: true,
        colNames: currentInstance.columnNames,
        colModel: currentInstance.columnModels,
        caption: currentInstance.design.Label,
        sortname: currentInstance.KeyProperty,
        sortorder: 'asc',
        scrollrows: true,
        toolbar: currentInstance.design.AllowBulkUpdate ? [true, "top"] : [false, "top"],
        onPaging: function (pgButton) {
            if (currentInstance.design.LoadFromServer == false) {
                if (pgButton === "user" && !IsEnteredPageExist(currentInstance.gridElementId)) {
                    return "stop";
                }
                Paging.ISPAGING = 1;
            }
        },
        beforeSelectRow: function (id, e) {
            if (id) {
                //when all column of repeater are disabled then code to skip excecute 'SaveRow' method 
                var flag = false;
                for (var i = 0 ; i < currentInstance.design.Elements.length ; i++) {
                    if (currentInstance.design.Elements[i].Enabled != false) {
                        flag = true; break;
                    }
                }
                currentInstance.selectedRowId = id;
                if (currentInstance.lastSelectedRow != null) {
                    if (!($("#repeater" + currentInstance.gridElementId).hasClass('disabled'))) {
                        var lastSelectedRow = currentInstance.lastSelectedRow;
                        if (id != lastSelectedRow && flag != false) {
                            var ind = $(currentInstance.gridElementIdJQ).jqGrid('getGridRowById', lastSelectedRow);
                            var isRowEditable = $(ind).attr("editable") || "0";
                            if (isRowEditable == 1) {
                                var rowData = $.extend(true, {}, currentInstance.getRowDataFromInstance(lastSelectedRow));
                                currentInstance.saveRow();
                                var isRowDataChanges = currentInstance.isRowDataChanges(lastSelectedRow, rowData);
                                if (isRowDataChanges == true && isRowEditable == 1) {
                                    if (!currentInstance.isValidateDuplicate) {
                                        if (currentInstance.design.LoadFromServer == false) {
                                            if (currentInstance.checkDuplicate(lastSelectedRow)) {
                                                rowData = [];
                                                currentInstance.selectedRowId = currentInstance.lastSelectedRow;
                                                return false;
                                            }
                                        }
                                    }
                                }
                                rowData = [];
                            }
                        }
                        else {
                            //in row edit mode click on td(not control) it returns html content so we need to save it
                            if ((e.target.className == "" || e.target.className == "repeater-has-error" || (e.target.className).indexOf("ui-icon") == 0) && flag != false) {
                                if (e.target.attributes['0'].value != 'option') {
                                    $(currentInstance.gridElementIdJQ).jqGrid('saveRow', currentInstance.lastSelectedRow);
                                }
                            }
                        }
                    }
                }
            }
            return true;
        },
        onSelectRow: function (id) {
            if (id) {
                if (currentInstance.lastSelectedRow != null) {
                    //get row data on lastSelectedRow
                    if (currentInstance.design.LoadFromServer == true && currentInstance.lastSelectedRow != id) {
                        var rowData = $(currentInstance.gridElementIdJQ).getRowData(currentInstance.lastSelectedRow);
                        //get row data on lastSelectedRow
                        currentInstance.loadFromServerObject.checkModifyedRowDataAndDuplication(currentInstance.lastSelectedRow, rowData, "Update");
                    }
                }
                var editParameters = {
                    "url": null,
                    "oneditfunc": function () {
                        //add jsRender here per row
                        for (var idx = 0; idx < currentInstance.columnModels.length; idx++) {

                            var idpluscol = id + "_" + currentInstance.columnModels[idx].name;

                            if (currentInstance.columnModels[idx] != null) {
                                var element = currentInstance.design.Elements.filter(function (data) {
                                    return (currentInstance.columnModels[idx].name.split('_')[currentInstance.columnModels[idx].name.split('_').length - 1] == data.GeneratedName);
                                });

                                if (element[0] != null && element[0].Type == 'calendar') {
                                    currentInstance.pickDates(idpluscol, element[0]);
                                }
                                //for dropdown textbox element
                                if (element[0] != null && element[0].Type == 'SelectInput') {
                                    currentInstance.formatDropdownTextbox(idpluscol, element[0], id);
                                }
                                //code to disable controls on chrome browser
                                if (element[0] != null) {
                                    currentInstance.setChildElementsIsDisabled(currentInstance.gridElementIdJQ, id, element[0].GeneratedName, element[0].Type);
                                }
                            }
                            /*if (currentInstance.design.Elements[idx].Type == 'checkbox') {
                                  var checked = currentInstance.data[id][currentInstance.design.Elements[idx].GeneratedName];
                                  
                                   if (checked == "Yes")
                                       $("#" + id + "_" + currentInstance.design.Elements[idx].GeneratedName, this.gridElementIdJQ).prop('checked', 'checked');
                                   else
                                       $("#" + id + "_" + currentInstance.design.Elements[idx].GeneratedName, this.gridElementIdJQ).removeAttr('checked');
                               }*/
                        }
                        //register events
                        $(currentInstance.gridElementIdJQ + ' #' + id).find('input,select,textarea').each(function (idx, elem) {
                            $(elem).on('change', function () {
                                var elem = this;
                                var rowId = id;
                                var designElem = currentInstance.design.Elements.filter(function (el) {
                                    return el.GeneratedName == elem.name;
                                });
                                if (designElem != null && designElem.length > 0) {
                                    designElem = designElem[0];
                                    var rules = currentInstance.formInstanceBuilder.rules.getRulesForElement(designElem.FullName);
                                    if (rules != null && rules.length > 0) {
                                        var row = currentInstance.data.filter(function (dt) {
                                            return dt.RowIDProperty == rowId;
                                        });
                                        if (row != null && row.length > 0) {
                                            row = row[0];
                                            for (var idx = 0; idx < rules.length; idx++) {
                                                var parentId = rowId;
                                                var rule = rules[idx];
                                                var childId;
                                                if (rule.ParentRepeaterType == 'In Line') {
                                                    var elemId = elem.id;
                                                    var idParts = elemId.split('_');
                                                    childId = idParts[idParts.length - 2];
                                                    var childName = idParts[idParts.length - 3];
                                                    row[childName][childId][designElem.GeneratedName] = currentInstance.getElementValue(designElem.Type, this);
                                                }
                                                else if (rule.ParentRepeaterType == 'Child') {
                                                    parentId = currentInstance.parentRowId;
                                                    childId = rowId;
                                                    row[designElem.GeneratedName] = currentInstance.getElementValue(designElem.Type, this);
                                                }
                                                else {
                                                    row[designElem.GeneratedName] = currentInstance.getElementValue(designElem.Type, this);
                                                }
                                                currentInstance.runRuleForRepeater(rule, parentId, childId);

                                                var rowIndex = undefined;

                                                var ID = $(currentInstance.gridElementIdJQ).jqGrid("getDataIDs");
                                                $.each(ID, function (idx, ct) {
                                                    if (ct == rowId) {
                                                        rowIndex = idx;
                                                        return false;
                                                    }
                                                });

                                                selectedRowId = $(currentInstance.gridElementIdJQ).jqGrid('getGridParam', 'selrow');
                                                currentPage = $(currentInstance.gridElementIdJQ).getGridParam('page');
                                                var rowNum = $(currentInstance.gridElementIdJQ).getGridParam('rowNum');
                                                

                                                if (currentPage != 1) {
                                                    rowIndex = ((currentPage - 1) * rowNum) + rowIndex;
                                                }

                                                var keyValue = "";
                                                var repeaterId = currentInstance.design.Name + currentInstance.formInstanceId;
                                                $.each(currentInstance.design.Elements, function (index, element) {
                                                    if (element.IsKey == true) {
                                                        var rowData = $("#" + repeaterId).jqGrid('getRowData', selectedRowId);
                                                        var value;
                                                        if (currentInstance.rowDataBeforeEdit != undefined)
                                                            value = currentInstance.rowDataBeforeEdit[element.GeneratedName];
                                                        else
                                                            value = rowData[element.GeneratedName];

                                                        keyValue += value + " ";
                                                    }
                                                });

                                                if (keyValue != "") {
                                                    rowIndex = rowIndex + "|" + keyValue;
                                                }



                                                //validation for rule
                                                var validationErrorforRule = currentInstance.formInstanceBuilder.formValidationManager.handleValidation(rule.UIElementFullName, elem.value, rowIndex, '', rowId);
                                                if (validationErrorforRule) {
                                                    currentInstance.showValidatedControlsOnRepeaterElementChange(validationErrorforRule);
                                                }
                                            }
                                        }
                                    }
                                    else {
                                        currentInstance.isDataChangesOnTab = true;
                                    }
                                }

                                if (currentInstance.formInstanceBuilder.formDesignId == FormTypes.HSBADMINFORMDESIGNID && currentInstance.formInstanceBuilder.customRules.vendorCustomrule.hasCustomRules(currentInstance.formInstanceBuilder.formDesignId)) {
                                    if (currentInstance.formInstanceBuilder.customRules.vendorCustomrule.isCustomRule(currentInstance.fullName, elem.name))
                                        currentInstance.formInstanceBuilder.customRules.vendorCustomrule.populateOutboundEligibilityAndClaimInformationData(currentInstance.gridElementIdJQ, id, elem.value, elem.name, currentInstance.formInstanceId, currentInstance);
                                }
                            });
                        });
                    }
                };

                if (!$(currentInstance.gridElementIdJQ).attr("disabled")) {
                    //get row data before editing
                    currentInstance.rowDataBeforeEdit = $(currentInstance.gridElementIdJQ).jqGrid('getRowData', id);

                    $.each(currentInstance.formInstanceBuilder.sections, function (idx, ct) {
                        if (ct.IsLoaded == true && ct.FullName == currentInstance.design.FullName.split('.')[0]) {
                            if (currentInstance.browserInfo.browser == "Firefox") {
                                currentInstance.rowClick = true;
                            }

                            var ind = $(currentInstance.gridElementIdJQ).jqGrid('getGridRowById', id);
                            $(ind).attr("editable", "0");
                            $(currentInstance.gridElementIdJQ).jqGrid('editRow', id, editParameters);
                            return false;
                        }
                    });
                }
                currentInstance.lastSelectedRow = id;
            }
        },
        subGrid: currentInstance.hasChildGrids && !currentInstance.displayChildGridAsPopup,
        subGridRowExpanded: function (subGridId, rowId) {
            try {
                var builders = currentInstance.childGridBuilders.filter(function (builder) {
                    return builder.parentRowId == rowId;
                });

                if (builders != null || builders.length > 0) {
                    for (var idx = 0; idx < currentInstance.childGridBuilders.length; idx++) {
                        if (currentInstance.childGridBuilders[idx].parentRowId == rowId) {
                            currentInstance.childGridBuilders.splice(idx, 1);
                            break;
                        }
                    }
                }
                currentInstance.loadChildGrids(subGridId, rowId);
            } catch (e) {
                console.log(e);
            }
        },
        gridComplete: function () {
            if (currentInstance.gridType === "parent") {
                var width = $($(this).parent().closest('div').first()).width();
                if (width == 0 && currentInstance.fullName == 'PrescriptionDrug.PrescriptionDrugPlanInformation.DeductibleforPharmacyBenefits')
                    width = 1281;

                $(currentInstance.gridElementIdJQ).jqGrid().setGridWidth(width, false);

                //code for adjusting width of grid coloum..               
                var columnCounter = 0;
                $.each(currentInstance.columnModels, function (colId, colProp) {
                    if (colProp.hidden === false) {
                        columnCounter++;
                    }
                });
                if (width > (150 * columnCounter)) {
                    //to check no of coloumns and its width 
                    //with the totalgrid width to be adjusted with the totalgrid width
                    var totalColumnWidth = $(currentInstance.gridElementIdJQ).jqGrid().width();
                    var gridWidth = $(currentInstance.gridElementIdJQ).getGridParam("width");

                    if (totalColumnWidth != 0 && totalColumnWidth < gridWidth) {
                        if ($(currentInstance.gridElementIdJQ).getGridParam('records') > 0) {
                            $(currentInstance.gridElementIdJQ).setGridWidth(gridWidth, true);
                        }
                    }
                }
            }
            else if (currentInstance.gridType === "child") {
                var width = $($(this).parent().closest('.tablediv').first()).width(200 * currentInstance.columnModels.length);
            }
            if (currentInstance.design.AllowBulkUpdate == true) {
                //$("#t_" + currentInstance.gridElementId).find('#chkEnable').attr('disabled', 'disabled');
            }
            $(currentInstance.gridElementIdJQ).unbind("contextmenu");
        },
        resizeStop: function (width, index) {
            if (currentInstance.groupHeaders.length > 0) {
                $(currentInstance.gridElementIdJQ).jqGrid('destroyGroupHeader');

                $(currentInstance.gridElementIdJQ).jqGrid('setGroupHeaders',
                    { useColSpanStyle: false, groupHeaders: currentInstance.groupHeaders });
            }
            autoResizing(currentInstance.gridElementIdJQ);
        },
        loadComplete: function () {
            if (currentInstance.customruleMedical.hasCustomRules(currentInstance.formInstanceBuilder.formDesignId) && currentInstance.fullName == "BenefitReview.BenefitReviewGrid") {
                $(".viewLimits").click(function () {
                    var element = $(this).attr("Id");
                    currentInstance.customruleMedical.showLimitPouUp(element, currentInstance.data, currentInstance.tenantId, currentInstance.formInstanceId, currentInstance.formInstanceBuilder.formDesignVersionId, currentInstance.formInstanceBuilder.folderVersionId);
                });
            }

            // Set BackGround color to mapped row of BenMatrix report in BRG 
            if (currentInstance.formInstanceBuilder.formDesignId == FormTypes.MEDICALFORMDESIGNID && currentInstance.fullName == "BenefitReview.BenefitReviewGrid") {
                $.each(currentInstance.data, function (index, rowData) {
                    currentInstance.setBRGRowColor(rowData)
                });
            }

            if (!($("#repeater" + currentInstance.gridElementId).hasClass('disabled'))) {
                if (Paging.ISPAGING == 1) {
                    //setTimeout(currentInstance.gridEvents.processRules(), 100);
                    Paging.ISPAGING = 0;
                }

                if (currentInstance.formInstanceBuilder.errorGridData.length > 0) {
                    var selectedIndex;
                    var sectionName = currentInstance.fullName.split('.')[0];
                    $.each(currentInstance.formInstanceBuilder.errorGridData, function (i, el) {
                        if (sectionName == el.Section.replace(/\s/g, '')) {
                            selectedIndex = i;
                            return false;
                        }
                    });

                    if (selectedIndex != undefined) {
                        var sections = currentInstance.formInstanceBuilder.errorGridData[selectedIndex].ErrorRows.filter(function (ct) {
                            return currentInstance.fullName == ct.SubSectionName.replace(/\ => /g, '.').replace(/\s/g, '');
                        });

                        for (var r = 0; r < sections.length; r++) {
                            currentInstance.applyValidation(sections[r]);
                        }
                    }
                }
            }
            if (currentInstance.gridType == "child") {
                if (currentInstance.displayChildGridAsPopup) {
                    var parentGridID = currentInstance.gridElementId.split("_")[0];
                    if ($("#repeater" + parentGridID).hasClass('disabled')) {
                        currentInstance.formUtilities.sectionManipulation.disableRepeater(currentInstance.gridElementIdJQ);
                    }
                } else {
                    if ($(currentInstance.gridElementIdJQ).closest('.repeater-grid').hasClass('disabled')) {
                        currentInstance.formUtilities.sectionManipulation.disableRepeater(currentInstance.gridElementIdJQ);
                    }
                }
            }
            else {
                if ($("#repeater" + currentInstance.gridElementId).hasClass('disabled')) {
                    currentInstance.formUtilities.sectionManipulation.disableRepeater("#repeater" + currentInstance.gridElementId);
                }
            }

            if (currentInstance.design.LoadFromServer == true) {
                var gridData = $(currentInstance.gridElementIdJQ).jqGrid('getRowData');
                currentInstance.data = new Array();
                gridData.filter(function (data) {
                    currentInstance.data.push(data);
                });
                var repeaterName = currentInstance.design.GeneratedName;
                var sectionName = currentInstance.design.FullName.split('.');
                currentInstance.formInstanceBuilder.formData[sectionName[0]][repeaterName] = currentInstance.data;
                //remove data after pagging if not save the row.
                if (currentInstance.formInstanceBuilder.repeaterRowIdList[currentInstance.design.GeneratedName])
                    currentInstance.formInstanceBuilder.repeaterRowIdList[currentInstance.design.GeneratedName] = [];

                if (currentInstance.formInstanceBuilder.loadDataFromRepeater[currentInstance.design.GeneratedName])
                    currentInstance.formInstanceBuilder.loadDataFromRepeater[currentInstance.design.GeneratedName] = [];
            }
            //Elimination of the white spaces in repeaters            
            currentInstance.setRepeaterHeight();
            if (currentInstance.design.HelpText != null || currentInstance.design.HelpText != undefined) {
                $("#repeater" + currentInstance.gridElementId).find('.ui-jqgrid-titlebar').addClass('hastooltip').attr('title', currentInstance.design.HelpText);
            }
        },
    });
    if (!this.hasChildGrids) {
        $(currentInstance.gridElementIdJQ).jqGrid('sortableRows');
    }

    var hasRows = currentInstance.data.length > 0;

    //if (hasRows == false) {
    //    this.addDefualtRow();
    //}

    if (hasRows == true && currentInstance.fullName.indexOf("Limits") == -1) {
        var ids = $(currentInstance.gridElementIdJQ).jqGrid("getDataIDs");
        if (ids && ids.length > 0) {
            $(currentInstance.gridElementIdJQ).jqGrid("setSelection", ids[0]);
            $(currentInstance.gridElementIdJQ).jqGrid("saveRow", ids[0]);
        }
    }
    $(pagerElement).find('input').css('height', '20px');

    if (currentInstance.design.LoadFromServer == true) {
        $(currentInstance.gridElementIdJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    }
    else {
        //set footer
        $(currentInstance.gridElementIdJQ).jqGrid('navGrid', pagerElement, { refreshstate: 'current', edit: false, add: false, del: false, search: false, beforeRefresh: function () { if (!($("#repeater" + currentInstance.gridElementId).hasClass('disabled'))) { Paging.ISPAGING = 1; } } });
    }

    //set filter toolbar
    $(currentInstance.gridElementIdJQ).jqGrid('filterToolbar', {
        stringResult: true, searchOnEnter: true, defaultSearch: "cn", afterSearch: function () {
            if ($(this).getRowData().length > 0) {
                var searchvalues = JSON.parse($(this).jqGrid("getGridParam", "postData").filters).rules.length;
                //check if bulk update for repeater is enabled & current formInstance is in Edit Mode then post search enable bulk update checkbox
                if (!($("#repeater" + currentInstance.gridElementId).hasClass('disabled')) && searchvalues > 0) {//
                    $("#t_" + currentInstance.gridElementId).find('#chkEnable').removeAttr('disabled');
                }
                else {
                    //$("#t_" + currentInstance.gridElementId).find('#chkEnable').attr('disabled', 'disabled');
                }
            }
        }
    });

    var isRateTypeSumRep = currentInstance.customruleMedical.adminFeeTotals.isFeeTotalsByRateTypeRep(currentInstance);
    var isIncludeFeeYesRep = currentInstance.customruleMedical.adminFeeTotals.isAdminFeeTotals(currentInstance);

    if (currentInstance.design.PrimaryDataSource == null && currentInstance.gridType != 'child' && isRateTypeSumRep == false) {
        $(currentInstance.gridElementIdJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-plus', title: 'Add', id: 'btnRepeaterBuilderAdd',
            onClickButton: function () {
                currentInstance.addNewRow();
                if (currentInstance.design.LoadFromServer == true) {
                    var gridData = $(this).jqGrid('getRowData');
                    var newRowData = gridData[gridData.length - 1];
                    currentInstance.loadFromServerObject.addRepeaterNewRowId(newRowData.RowIDProperty);
                }
                if (currentInstance.lastSelectedRow <= 0) {
                    currentInstance.setRepeaterHeight();
                }
            }
        });
        $(currentInstance.gridElementIdJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-minus', title: 'Remove', id: 'btnRepeaterBuilderDelete',
            onClickButton: function () {
                //remove selected row
                var rowId = $(this).getGridParam('selrow');
                if (rowId != null) {
                    if (currentInstance.design.LoadFromServer == true) {
                        var rowData = currentInstance.data.filter(function (row) {
                            return row.RowIDProperty == rowId;
                        });
                        //get row data on lastSelectedRow
                        currentInstance.loadFromServerObject.checkModifyedRowDataAndDuplication(currentInstance.lastSelectedRow, rowData[0], "Delete");
                    }
                    currentInstance.deleteRow(rowId);
                }
            }
        });
        //Copy button to copy data from selected folder and create new
        $(this.gridElementIdJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-copy', title: 'Copy', id: 'btnRepeaterBuilderCopy',
            onClickButton: function () {
                var rowId = $(this).getGridParam('selrow');
                var isRowReadOnly = $(this).find(".jqgrow[aria-selected='true']").prop('disabled');
                if (rowId !== undefined && rowId !== null) {
                    currentInstance.copyRow(rowId);
                }
                else {
                    messageDialog.show(Common.pleaseSelectRowMsg);
                }
            }
        });
    }

    //export to excel
    $(this.gridElementIdJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-arrowstop-1-s', title: 'Download To Excel', id: 'btnRepeaterBuilderExportToExcel',
            onClickButton: function () {

                if (currentInstance.hasChildGrids) {
                    extConfirmDialog.show("Do you want to export all child grid's data?", function (e) {
                        extConfirmDialog.hide();
                        if (e)
                            currentInstance.getExportData(true);
                        else
                            currentInstance.getExportData(false);
                    })
                }
                else {
                    currentInstance.getExportData(false);
                }

            }
        });
    if (!(isRateTypeSumRep || isIncludeFeeYesRep)) {
        $(this.gridElementIdJQ).jqGrid('navButtonAdd', pagerElement,
          {
              caption: '', buttonicon: 'ui-icon-document', title: 'View', id: 'btnRepeaterBuilderView',
              onClickButton: function () {
                  var rowId = $(this).getGridParam('selrow');
                  var isRowReadOnly = $(this).find(".jqgrow[aria-selected='true']").prop('disabled') || $(this).closest('.repeater-grid').hasClass('disabled');
                  if (rowId !== undefined && rowId !== null) {
                      ajaxDialog.showPleaseWait();
                      setTimeout(function () {
                          currentInstance.viewRowData(rowId, isRowReadOnly);
                          ajaxDialog.hidePleaseWait();
                      }, 100);
                  }
                  else {
                      messageDialog.show(Common.pleaseSelectRowMsg);
                  }
              }
          });
    }


    var primaryDataSources = currentInstance.design.PrimaryDataSource;

    if (primaryDataSources != null && currentInstance.checkCustomDataSource()) {
        if (!(currentInstance.customruleMedical.adminFeeTotals.isAdminFeeSourceChildRep(currentInstance))) {
            if (primaryDataSources.DataSourceModeType == 'Manual' || primaryDataSources.DataSourceModeType == 'Custom' && (primaryDataSources.DisplayMode == 'Primary' || primaryDataSources.DisplayMode == 'Child')) {
                //Auto Or Manual Data Source Mode
                //if (currentInstance.fullName != "BenefitReview.BenefitReviewGrid" && currentInstance.gridElementId != "VIS14Repeater" + currentInstance.design.ID + currentInstance.formInstanceId) {// 
                    $(this.gridElementIdJQ).jqGrid('navButtonAdd', pagerElement,
                       {
                           caption: '', buttonicon: 'ui-icon-plus', title: 'Add', id: 'btnManualDataPopup',
                           onClickButton: function () {
                               if (currentInstance.customruleMedical.adminFeeTotals.isAdminFeeTotals(currentInstance)) {
                                   currentInstance.customruleMedical.adminFeeTotals.setAdminFeePopUp(currentInstance);
                               }
                               else {
                                   currentInstance.addManualData();
                               }
                           }
                       });
                //}
            }
        }
    }

    //add group headers
    if (currentInstance.groupHeaders.length > 0) {
        $(currentInstance.gridElementIdJQ).jqGrid('setGroupHeaders',
            { useColSpanStyle: false, groupHeaders: this.groupHeaders });
    }

    $(pagerElement + '_left').css('width', '');

    //this code adds a hook to set focus on the control in side the cell of the jqgrid row clicked.
    $(this.gridElementIdJQ).bind("click", function (e) {
        var el = e.target;
        if (el.nodeName !== "TD") {
            el = $(el, this.rows).closest("td");
        }
        iCol = $(el).index();
        currentInstance.lastColumnSelected = iCol;

        var row = $(el, this.rows).closest("tr.jqgrow");
        if (row.length > 0) {
            var rowId = row[0].id;
            var ind = $(currentInstance.gridElementIdJQ).jqGrid("getInd", rowId, true);
            var isEdiatble = $(ind).attr("editable");
            //if (isEdiatble == 0) {
            //   // $(ind).attr("editable", "1");
            //}
            if (isEdiatble == 1 && currentInstance.browserInfo.browser == "Firefox") {
                if (currentInstance.rowClick == false) {
                    currentInstance.tabClick = true;
                }
                else {
                    currentInstance.rowClick = false;
                }
            }

            var child = $(" td:eq(" + iCol + ")", ind).children().first();
            if (child.length > 0) {
                $(child).focus();
            }
            else {
                var isfocused = false;
                for (var i = iCol - 1; i > -1; i--) {
                    var child = $(" td:eq(" + (i) + ")", ind).children().first();
                    if (child.length > 0) {
                        $(child).focus();
                        isfocused = true;
                        return false;
                    }
                }

                if (isfocused == false) {
                    for (var i = iCol + 1; i < ind.cells.length; i++) {
                        var child = $(" td:eq(" + (i) + ")", ind).children().first();
                        if (child.length > 0) {
                            $(child).focus();
                            return false;
                        }
                    }
                }
            }
        }
    });

    //this code calls the save row method whenever user clicks out of the repeater row, so as to save the row.     
    $(this.gridElementIdJQ).focusout(function (e) {
        var relatedTarget = e.relatedTarget || document.activeElement;

        if (relatedTarget) {
            var row = $(e, this.rows).closest("tr.jqgrow");
            //if condition for avoid focusout execution on date selection from ui-datepicker 
            if (((relatedTarget.className).indexOf("ui-datepicker") == -1)
                    && relatedTarget.className != " "
                    && ((relatedTarget.className).indexOf("ui-state-default") == -1)) {

                currentInstance.repeaterControlFocusOut(e);

                var $related = $(currentInstance.gridElementIdJQ).find(relatedTarget);

                if ($related.length <= 0 && currentInstance.lastSelectedRow) {
                    //avoid save row on date selection
                    if ((relatedTarget.className).indexOf("ui-dialog") == -1 && currentInstance.lastSelectedRow == currentInstance.selectedRowId) {
                        if (currentInstance.tabClick != true) {
                            currentInstance.saveRow();
                            //if ((relatedTarget.className).indexOf("ui-jqgrid-bdiv") == 0) {
                            //    $(currentInstance.gridElementIdJQ).jqGrid('setSelection', currentInstance.lastSelectedRow);
                            //}EQN-1503
                        }
                    }

                    if (currentInstance.design.LoadFromServer == true) {
                        var rowData = $(currentInstance.gridElementIdJQ).getRowData(currentInstance.lastSelectedRow);
                        //get row data on lastSelectedRow
                        currentInstance.loadFromServerObject.checkModifyedRowDataAndDuplication(currentInstance.lastSelectedRow, rowData, "Update");
                    }
                }
                if (currentInstance.formInstanceBuilder.form.hasValidationErrors()) {
                    currentInstance.formInstanceBuilder.form.showValidationErrorsOnForm();
                }
                else {
                    currentInstance.formInstanceBuilder.form.hideValidationErrorsOnForm();
                }
            }
        }
        if (currentInstance.browserInfo.browser == "Firefox") {
            currentInstance.tabClick = false;
        }
    });

    if (currentInstance.browserInfo.browser == "Firefox") {
        $(this.gridElementIdJQ).on("keydown", function (e) {
            if (e.which == 9) {
                var el = e.target;
                if (el.nodeName !== "TD") {
                    el = $(el, this.rows).closest("td");
                }
                iCol = $(el).index();
                var coltol = currentInstance.design.Elements.length - 1;
                if (iCol != coltol || e.shiftKey) {
                    currentInstance.tabClick = true;
                }
            }
        });
    }

    //if bulk update for repeater is enabled, add controls for that.
    if (this.design.AllowBulkUpdate === true) {
        currentInstance.addBulkUpdateControls();
    }
}

//hide add '+' button for sepecific selected Repeater
repeaterBuilderMedical.prototype.checkCustomDataSource = function () {
    var currentInstance = this;
    var result = true;

    //var customruleMedical = new customruleMedical();
    if (currentInstance.customruleMedical.hasCustomRules(currentInstance.formInstanceBuilder.formDesignId) == true) {
        result = currentInstance.customruleMedical.hideAddButtonforRepeater(currentInstance.fullName);
    }

    return result;
}

repeaterBuilderMedical.prototype.getDataTypeProperty = function () {
    var currentInstance = this;
    var datatype;
    if (currentInstance.formInstanceBuilder.formDesignId == FormTypes.HSBMASTERLISTFORMID && currentInstance.design.LoadFromServer == true) {
        datatype = "json";
    }
    else {
        datatype = "local";
    }
    return datatype;
}

//export to excel
repeaterBuilderMedical.prototype.getExportData = function (autoExpandChild) {
    FolderLockAction.ISREPEATERACTION = 1;
    var currentInstance = this;
    currentInstance.saveRow();
    var jqGridtoCsv = new JQGridtoCsv(currentInstance.gridElementIdJQ, autoExpandChild, currentInstance);
    jqGridtoCsv.buildExportOptions();

    var forminstancelisturl = '/FormInstance/ExportToExcel';
    jqGridtoCsv.csvData = jqGridtoCsv.csvData.replace(/"/g, '');;
    var stringData = "csv=" + jqGridtoCsv.csvData;
    stringData += "<&isGroupHeader=" + jqGridtoCsv.isGroupHeader;
    stringData += "<&noOfColInGroup=" + jqGridtoCsv.noofGroupedColumns;
    stringData += "<&isChildGrid=" + jqGridtoCsv.isSubgrid;
    stringData += "<&repeaterName=" + currentInstance.design.Label;
    stringData += "<&formName=" + currentInstance.formInstanceBuilder.formName;
    stringData += "<&folderVersionId=" + currentInstance.formInstanceBuilder.folderVersionId;
    stringData += "<&folderId=" + currentInstance.formInstanceBuilder.folderId;
    stringData += "<&tenantId=" + currentInstance.tenantId;

    if (currentInstance.customruleMedical.adminFeeTotals.isRateTypeSumRep(currentInstance)) {
        //Remove span elements from string
        var removedSpanElem = stringData.replace(/<\/?span[^>]*>/g, "");
        $.download(forminstancelisturl, removedSpanElem, 'post');
    } else {
        $.download(forminstancelisturl, stringData, 'post');
    }
}

repeaterBuilderMedical.prototype.viewRowData = function (rowId, isRowReadOnly) {
    var currentInstance = this;

    var count = $(currentInstance.gridElementIdJQ).getGridParam('reccount');
    var ids = $(currentInstance.gridElementIdJQ).getDataIDs();
    var pageNo = $(currentInstance.gridElementIdJQ).getGridParam('page');

    var rowNum = $(currentInstance.gridElementIdJQ).getGridParam('rowNum');
    var allRecords = $(currentInstance.gridElementIdJQ).getGridParam('records');
    var totalPages = parseInt((allRecords / rowNum) + 1);

    var repData = currentInstance.data;
    var filters = $(currentInstance.gridElementIdJQ).getGridParam("postData").filters;

    if (currentInstance.design.LoadFromServer == false) {
        $(currentInstance.gridElementIdJQ).setGridParam({ page: pageNo });
        $(currentInstance.gridElementIdJQ).trigger('reloadGrid');
    }
    if (isRowReadOnly) {
        for (var i = 0; i < ids.length; i++) {
            $("tr#" + ids[i], currentInstance.gridElementIdJQ).attr('disabled', 'disabled');
        }
    }
    $(currentInstance.gridElementIdJQ).jqGrid('setSelection', rowId);

    var elements = [];

    $.each(currentInstance.design.Elements, function (i, elem) {
        elements.push({
            Id: elem.GeneratedName, displayText: elem.Label, Visible: elem.Visible, Type: elem.Type, UIElementName: elem.Name,
            Items: elem.Items,
            Details: elem
        });
    });
    var ind = $(currentInstance.gridElementIdJQ).jqGrid("getInd", rowId, true);
    var child = $(" td:eq(" + 0 + ")", ind).children().first();
    if (child.length > 0) {
        $(child).focus();
    }

    //if (filters != undefined) {
    //    var filterData = JSON.parse(filters);
    //    if (filterData["rules"].length != 0)
    //currentInstance.data = repData;
    //}   
    //currentInstance.data = repData;
    currentInstance.saveRow();
    var dialog = new repeaterdialog(rowId, elements, currentInstance, isRowReadOnly, repData);
    dialog.show();
}

//Add Data on Manual Mode
repeaterBuilderMedical.prototype.addManualData = function () {
    var currentInstance = this;
    var elements = [];
    $.each(currentInstance.design.Elements, function (i, elem) {
        elements.push({
            Id: elem.GeneratedName, displayText: elem.Label, Visible: elem.Visible, Type: elem.Type, UIElementName: elem.Name,
            Items: elem.Items,
            Details: elem
        });
    });
    if (checkIfPQGridLoaded(true)) {
       var dialog = new manualDataSourceMappingDialogMedicalPQ(elements, currentInstance.design.Label, currentInstance);
    }
    else{
    var dialog = new manualDataSourceMappingDialogMedical(elements, currentInstance.design.Label, currentInstance);
    }
    dialog.mapDataSourceManually();
}

repeaterBuilderMedical.prototype.updateCallBack = function (rowData, rowID) {
    var currentInstance = this;
    $(currentInstance.gridElementIdJQ).jqGrid("setRowData", rowID, rowData);
    currentInstance.saveRow();
    //if (currentInstance.design.LoadFromServer == false)
    //    $(currentInstance.gridElementIdJQ).trigger('reloadGrid');
    //$(currentInstance.gridElementIdJQ).jqGrid('setSelection', rowID, focus());

    setTimeout(currentInstance.gridEvents.processRules(), 25);
}

repeaterBuilderMedical.prototype.prepareGridData = function () {
    var currentInstance = this;
    var gridData = [];
    var ids = [];
    var data = currentInstance.data;

    if (currentInstance.design.Elements)
        var defaultValueColumns = currentInstance.design.Elements.filter(function (ct) {
            return (ct.DefaultValue != '' && ct.DefaultValue !== undefined && ct.DefaultValue != null);
        });

    for (var index = 0; index < data.length; index++) {
        if (data[index].hasOwnProperty(this.KeyProperty) && data[index].RowIDProperty >= 0) {
            ids.push(data[index].RowIDProperty);
        }

        if (defaultValueColumns.length > 0) {
            currentInstance.setDefaultValues(data[index], defaultValueColumns);
        }

        if (data[index].MasterListVisionNetworkTier)
        {
            currentInstance.SetVisionCopayDefaultValue(data[index].MasterListVisionNetworkTier);
        }
    }
    var maxId = 0;
    if (ids.length > 0) {
        maxId = Math.max.apply(Math, ids) + 1;
    }
    for (var index = 0; index < data.length; index++) {
        if (!(data[index].hasOwnProperty(this.KeyProperty) && data[index].RowIDProperty >= 0)) {
            data[index].RowIDProperty = maxId;
            maxId++;
        }
        var row = data[index];
        var newRow = $.extend({}, row);
        $.each(currentInstance.columnModels, function (idx, col) {
            if (col.name.substring(0, 4) == 'INL_') {
                var propArr = col.name.split('_');
                if (propArr.length == 4) {
                    var dsName = propArr[1];
                    var propIdx = propArr[2];
                    var propName = propArr[3];
                    newRow[col.name] = row[dsName][propIdx][propName];
                }
            }
        });
        gridData.push(newRow);
    }
    return gridData;
}

//Set default values of columns in repeater which is mapped to a Datasource - EQN 903
repeaterBuilderMedical.prototype.setDefaultValues = function (data, defaultValueColumns) {
    $.each(defaultValueColumns, function (j, jData) {
        var colName = jData.GeneratedName;
        var defaultValue = jData.DefaultValue;

        if (data[colName] === undefined)
            data[colName] = defaultValue;
    });
}

repeaterBuilderMedical.prototype.addNewRow = function () {
    var row = {};
    this.saveRow();
    for (var idx = 0; idx < this.columnModels.length; idx++) {
        if (this.columnModels[idx].editoptions != undefined) {
            row[this.columnModels[idx].index] = this.columnModels[idx].editoptions.defaultValue;
        }
        else {
            row[this.columnModels[idx].index] = "";
        }
    }
    var ids = [-1];
    for (var idx = 0; idx < this.data.length; idx++) {
        ids.push(this.data[idx].RowIDProperty);
    }
    var rowId = Math.max.apply(Math, ids) + 1;
    row[this.KeyProperty] = rowId;

    $(this.gridElementIdJQ).jqGrid('addRowData', rowId, row);
    if (this.design.PrimaryDataSource == null) {
        this.data.push(row);
    }

    this.updateTargetDataSourceCollection(rowId, "add");

    $(this.gridElementIdJQ).jqGrid('setSelection', rowId);

    this.setCursorOnEditRow(rowId, 0);
    this.selectedRowId = rowId;

    setTimeout(this.gridEvents.processRules(), 25);
}

repeaterBuilderMedical.prototype.deleteRow = function (rowId) {
    var rowData = $(this.gridElementIdJQ).getRowData(rowId);

    var keyobject =
        {
            rowobject: rowData,
            action: "deleteRow"
        };
    var keyValue = "";
    var repeaterId = this.design.Name + this.formInstanceId;
    var selectedRowId = $("#" + repeaterId).jqGrid('getGridParam', 'selrow');
    $.each(this.design.Elements, function (index, element) {
        if (element.IsKey == true) {
            var value = $("#" + repeaterId).jqGrid('getCell', selectedRowId, element.GeneratedName);

            if (isHTML(value)) {
                var elementID = $(value).attr('id')
                if (elementID != undefined)
                    value = $('#' + elementID).val();
            }

            keyValue += value + " ";
        }
    });
    if (rowData) {
        $(this.gridElementIdJQ).delRowData(rowId);
        var index = 0;
        for (var i = 0; i < this.data.length; i++) {
            if (this.data[i].RowIDProperty == rowData.RowIDProperty) {
                index = i;
                break;
            }
        }
        this.data.splice(index, 1);
        this.lastSelectedRow = null;

        this.updateTargetDataSourceCollection(rowId, "delete");

        if (this.formInstanceBuilder.errorGridData != [] && this.formInstanceBuilder.errorGridData != null && this.formInstanceBuilder.errorGridData != undefined) {
            this.removeErrorGridRowOnRepeaterDeleteRow(rowId);
        }

        //add an entry to activity log
        this.addEntryToAcitivityLogger(rowId, Operation.DELETE, '', keyValue, '');
        this.formInstanceBuilder.hasChanges = true;
        $(this.gridElementIdJQ).jqGrid('setSelection', rowId - 1);

        // below code for set focus to an first column of selected row
        this.setCursorOnEditRow(rowId - 1, 0);
        this.selectedRowId = rowId - 1;
    }
}

repeaterBuilderMedical.prototype.copyRow = function (rowId) {
    var currentInstance = this;
    currentInstance.saveRow(true);
    //copy over primary values
    var row = currentInstance.data.filter(function (row) {
        return row.RowIDProperty == rowId;
    });
    var rowData = $.extend({}, true, row[0]);
    if (rowData) {
        var row = {};
        for (var idx = 0; idx < currentInstance.columnModels.length; idx++) {
            row[currentInstance.columnModels[idx].index] = "";
        }
        var ids = [-1];
        for (var idx = 0; idx < currentInstance.data.length; idx++) {
            ids.push(currentInstance.data[idx].RowIDProperty);
        }
        var newRowId = Math.max.apply(Math, ids) + 1;
        row[currentInstance.KeyProperty] = newRowId;

        $(currentInstance.gridElementIdJQ).jqGrid('addRowData', newRowId, row);
        rowData.RowIDProperty = newRowId;
        currentInstance.data.push(rowData);
        $(currentInstance.gridElementIdJQ).jqGrid('setRowData', newRowId, rowData);
        currentInstance.updateTargetDataSourceCollection(newRowId, "add");
        currentInstance.formInstanceBuilder.hasChanges = true;
        if (currentInstance.design.LoadFromServer == true) {
            currentInstance.loadFromServerObject.addRepeaterNewRowId(newRowId);
        }
        $(currentInstance.gridElementIdJQ).jqGrid('setSelection', newRowId, focus());
        this.setCursorOnEditRow(newRowId, 0);
    }
}

repeaterBuilderMedical.prototype.pickDates = function (idpluscol, elementDesign) {
    var currentInstance = this;
    $("#" + idpluscol).width('82%');
    var minDate = "", maxDate = "", defaultDate = "";
    minDate = elementDesign.MinDate;
    maxDate = elementDesign.MaxDate;
    defaultDate = elementDesign.DefaultValue;

    //select datapicker - jqueryui
    $("#" + idpluscol, this.gridElementIdJQ).datepicker({
        dateFormat: "mm/dd/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: 'c-61:c+20',
        showOn: "both",
        minDate: minDate == '' || minDate == null ? null : new Date(minDate),
        maxDate: maxDate == '' || maxDate == null ? null : new Date(maxDate),
        //CalenderIcon path declare in golbalvariable.js
        buttonImage: Icons.CalenderIcon,
        buttonImageOnly: true,
        disabled: (!elementDesign.Enabled),
        onSelect: function (date) {
            $("#" + idpluscol, this.gridElementIdJQ).focus();
            currentInstance.isDataChangesOnTab = true;
        },
    }).parent().find('img').css('margin-top', '-6px');

    $("#" + idpluscol, currentInstance.gridElementIdJQ).on("focusout", function (e) {
        var date = new Date($("#" + idpluscol, currentInstance.gridElementIdJQ).val());
        currentInstance.formInstanceBuilder.validation.handleDateRangeValidation(date, minDate, maxDate, $("#" + idpluscol, currentInstance.gridElementIdJQ));
    });

    $("#" + idpluscol, currentInstance.gridElementIdJQ).on("keypress", function (e) {
        var maxLength = 10;
        var val = $("#" + idpluscol, currentInstance.gridElementIdJQ).val();
        if (val.length == maxLength) {
            e.preventDefault();
        }
    });

    if ($("#" + idpluscol, currentInstance.gridElementIdJQ).parent('td').attr("disabled") === "disabled") {
        $("#" + idpluscol, currentInstance.gridElementIdJQ).siblings().attr("disabled", "disabled");
    }
}

repeaterBuilderMedical.prototype.loadChildGrids = function (subGridId, rowId) {
    var currentInstance = this;
    $.each(this.design.ChildDataSources, function (idx, dataSource) {
        if (dataSource.DisplayMode == 'Child' && dataSource.Mappings != undefined && dataSource.Mappings.length > 0) {
            //generate design
            var childDesign = $.extend({}, currentInstance.design);
            var subgridTableId = subGridId + '_' + dataSource.DataSourceName;
            if (idx == 0) {
                $('#' + subGridId).html("<table id='" + subgridTableId + "' class='scroll'></table>");
            }
            else {
                $('#' + subGridId).append("<table id='" + subgridTableId + "' class='scroll'></table>");
            }
            childDesign.Name = subgridTableId;
            childDesign.Label = dataSource.DataSourceName;
            childDesign.PrimaryDataSource = null;
            childDesign.ChildDataSources = null;
            childDesign.Elements = [];
            $.each(currentInstance.design.Elements, function (idxe, element) {
                $.each(dataSource.Mappings, function (idxm, mapping) {
                    if (mapping.TargetElement == element.GeneratedName) {
                        var childElement = $.extend({}, element);
                        childElement.IsPrimary = true;
                        childDesign.Elements.push(childElement);
                    }
                });
            });

            //get data

            var data = currentInstance.data.filter(function (dt) {
                return dt.RowIDProperty == rowId;
            });

            if (data[0][dataSource.DataSourceName] == undefined) {
                var formInstanceBuilder = currentInstance.formInstanceBuilder;

                //split sourceparent name 
                var sourceParentArray = dataSource.SourceParent.split('.');

                //select selected section name
                var targetParentSectionName = sourceParentArray[0];

                //get section data
                var sourcedata = formInstanceBuilder.formData[targetParentSectionName];

                //get repeater data
                if (sourcedata != undefined) {
                    for (var n = 1; n < sourceParentArray.length; n++) {
                        sourcedata = sourcedata[sourceParentArray[n]];
                    }

                    var sourcesynchroniser = new datasourcesync(currentInstance.formInstanceBuilder, sourcedata, dataSource, rowId);
                    sourcesynchroniser.bindChildDataSource(currentInstance.data, rowId);

                    sourcesynchroniser = null;
                }
            }

            var childData = data[0][dataSource.DataSourceName];
            var childFullName = currentInstance.fullName + '.' + dataSource.DataSourceName;


            //ReInitialize rules if undefined or blank
            currentInstance.rules = (currentInstance.rules == "" || currentInstance.rules == undefined) ? [] : currentInstance.rules;

            var rulesForRepeater = currentInstance.rules.filter(function (rule) {
                return rule.UIElementFullName.indexOf(childFullName) == 0;
            });
            if (currentInstance.design.ChildDataSources != null)
                childDesign.PrimaryDataSource = currentInstance.design.ChildDataSources[0];
            var childBuilder = new repeaterBuilderMedical(childDesign, childData, childFullName, currentInstance.formInstanceId, 'child', rowId, currentInstance.formInstanceBuilder, rulesForRepeater, currentInstance.ruleProcessor);
            currentInstance.childGridBuilders.push(childBuilder);
            childBuilder.build();
        }
    });
}

repeaterBuilderMedical.prototype.bindData = function () {
    this.getData();
}

repeaterBuilderMedical.prototype.getData = function () {
    this.saveRow(true);
    $.each(this.childGridBuilders, function (idx, gridBuilder) {
        gridBuilder.getData();
    });
}

repeaterBuilderMedical.prototype.showValidatedControls = function (isOnCellChangeEvent) {
    var currentInstance = this;
    if (currentInstance.formInstanceBuilder.errorGridData != null) {
        currentInstance.removeValidationErrors();

        //On repeater onChange SelectRow event, there is no need to restore the Grid
        if (isOnCellChangeEvent == undefined || isOnCellChangeEvent == false) {
            var ids = $(currentInstance.gridElementIdJQ).jqGrid('getDataIDs');
            for (i = 0; i < ids.length; i++) {
                $(currentInstance.gridElementIdJQ).jqGrid('restoreRow', ids[i]);
            }
        }

        for (var i = 0; i < currentInstance.formInstanceBuilder.errorGridData.length; i++) {
            var row = currentInstance.formInstanceBuilder.errorGridData[i];
            for (var j = 0; j < row.ErrorRows.length; j++) {
                if (row.ErrorRows[j].RowNum != "" && row.ErrorRows[j].RowNum != null) {
                    this.applyValidation(row.ErrorRows[j]);
                }
            }
        }
    }
}

repeaterBuilderMedical.prototype.applyValidation = function (row) {
    var currentInstance = this;
    var colindex = undefined;
    var loadedPage = undefined;

    var repeaterID = row.ElementID.substring(0, row.ElementID.indexOf("_")) + row.FormInstanceID;
    var col = $("#" + repeaterID).jqGrid("getGridParam", "colModel");

    if (col != undefined) {
        $.each(col, function (idx, ct) {
            if (ct.name == row.GeneratedName) {
                colindex = idx;
                return false;
            }
        });

        var ind = $("#" + repeaterID).jqGrid('getGridRowById', row.RowIdProperty);
        if (ind) {
            //var index = $('#' + row.RowIdProperty)[0].rowIndex;
            var tcell = $("td:eq(" + colindex + ")", ind);
            $(tcell).addClass("repeater-has-error");
        }
    }
}

// Set BackGround color to mapped row of BenMatrix report in BRG 
repeaterBuilderMedical.prototype.setBRGRowColor = function (row) {
    var currentInstance = this;
    if (null!= row && row.IsMap) {
        var repeaterID = currentInstance.gridElementIdJQ;
        var col = $(repeaterID).jqGrid("getGridParam", "colModel");
        if (col != undefined) {
            var ind = $(repeaterID).jqGrid('getGridRowById', row.RowIDProperty);
            if (ind) {
                $(ind).addClass("greyBackGround");
            }
        }
    }
}

repeaterBuilderMedical.prototype.removeValidationErrors = function () {
    var currentInstance = this;
    if (currentInstance.formInstanceBuilder.errorGridData != null) {
        for (var i = 0; i < currentInstance.formInstanceBuilder.errorGridData.length; i++) {
            var row = currentInstance.formInstanceBuilder.errorGridData[i];
            for (var j = 0; j < row.ErrorRows.length; j++) {
                if (row.ErrorRows[j].RowNum != "" && row.ErrorRows[j].RowNum != undefined) {
                    var rowNumber = row.ErrorRows[j].RowNum;
                    var td = getRepeaterColumn(row.ErrorRows[j].ElementID.split('_')[0] + currentInstance.formInstanceId, rowNumber, row.ErrorRows[j].GeneratedName, row.ErrorRows[j].ColumnNumber);
                    if (td)
                        $(td).removeClass("repeater-has-error");
                }
            }
        }
    }
}

repeaterBuilderMedical.prototype.runRuleForRepeater = function (rule, parentRowId, childRowId) {
    this.ruleProcessor.processRule(rule, parentRowId, childRowId);
}

repeaterBuilderMedical.prototype.visibleRuleResultCallBack = function (rule, result) {
    if (rule.ParentRepeaterType == 'In Line') {
        //get in line columns to hide
        var nameParts = rule.UIElementFullName.split('.');
        var dsName = nameParts[nameParts.length - 2];
        var colNamePart = 'INL_' + dsName + '_';
        var elemPart = '_' + rule.UIElementName;
        var colModels = $(this.gridElementIdJQ).jqGrid('getGridParam', 'colModel');
        var cols = colModels.filter(function (model) {
            return model.name.indexOf(colNamePart) == 0 && model.name.indexOf(elemPart) > 0;
        });
        var colNames = [];
        for (var idx = 0; idx < cols.length; idx++) {
            colNames.push(cols[idx].name);
        }
        if (result == true) {
            $(this.gridElementIdJQ).jqGrid('showCol', colNames);
        }
        else {
            $(this.gridElementIdJQ).jqGrid('hideCol', colNames);
        }

    }
    else if (rule.ParentRepeaterType == 'Child') {
        //get child grid/column to hide
    }
    else {
        //get column to hide
        if (result == true) {
            $(this.gridElementIdJQ).jqGrid('showCol', rule.UIElementName);
        }
        else {
            $(this.gridElementIdJQ).jqGrid('hideCol', rule.UIElementName);
        }
    }
    if (this.design.AllowBulkUpdate == true) {
        var width = $($(this.gridElementIdJQ).parent().closest('div').first()).width();
        $('#t_' + this.gridElementId).css('width', width);
    }
}

repeaterBuilderMedical.prototype.ruleResultCallBack = function (rule, row, retVal, childName, childIdx) {
    if (rule.ParentRepeaterType == null || rule.ParentRepeaterType == "Dropdown") {
        this.setParentRow(rule, row, retVal);
    }
    if (rule.ParentRepeaterType == 'In Line' || rule.ParentRepeaterType == 'Child') {
        this.setChildRow(rule, row, retVal, childName, childIdx);
    }
}

repeaterBuilderMedical.prototype.setParentRow = function (rule, row, retVal) {
    if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Value) {
        $(this.gridElementIdJQ).jqGrid('setCell', row.RowIDProperty, rule.UIElementName, retVal);
    }
    else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Visible) {
        if (retVal == true) {
            $(this.gridElementIdJQ).jqGrid('setCell', row.RowIDProperty, rule.UIElementName, '', { visibility: 'visible' });
            this.setChildElementsIsVisible(this.gridElementIdJQ, row.RowIDProperty, rule.UIElementName, rule.UIElementTypeID);
        }
        else {
            $(this.gridElementIdJQ).jqGrid('setCell', row.RowIDProperty, rule.UIElementName, '', { visibility: 'hidden' });
            this.setChildElementsIsVisible(this.gridElementIdJQ, row.RowIDProperty, rule.UIElementName, rule.UIElementTypeID);
            this.removeVisibleAndDisabledElementFromErrorGrid(row.RowIDProperty, rule.UIElementName);
        }
    }
    else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Enabled) {
        if (retVal == true) {
            $(this.gridElementIdJQ).jqGrid('setCell', row.RowIDProperty, rule.UIElementName, '', '', { disabled: null });
            this.setChildElementsIsDisabled(this.gridElementIdJQ, row.RowIDProperty, rule.UIElementName, rule.UIElementTypeID);
        }
        else {
            $(this.gridElementIdJQ).jqGrid('setCell', row.RowIDProperty, rule.UIElementName, '', '', { disabled: 'disabled' });
            this.setChildElementsIsDisabled(this.gridElementIdJQ, row.RowIDProperty, rule.UIElementName, rule.UIElementTypeID);
            this.removeVisibleAndDisabledElementFromErrorGrid(row.RowIDProperty, rule.UIElementName);
        }
    }
    else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Error) {
        var iCol = getColumnSrcIndexByName($(this.gridElementIdJQ), rule.UIElementName);
        var el = $("tr#" + row.RowIDProperty, this.gridElementIdJQ).find('td').eq(iCol);
        if ($(el).css('visibility') != undefined && $(el).css('visibility') == 'visible' && $(el)[0].attributes.disabled == undefined) {
            this.setElementErrorState(retVal, rule, row);
        }
    }
    else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.IsRequired) {
        this.setElementIsRequired(retVal, rule, row);
    }
}

repeaterBuilderMedical.prototype.setChildRow = function (rule, row, retVal, childName, childIdx) {
    if (rule.ParentRepeaterType == 'In Line') {
        if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Value) {
            $(this.gridElementIdJQ).jqGrid('setCell', row.RowIDProperty, 'INL_' + childName + '_' + childIdx + '_' + rule.UIElementName, retVal);
        }
        else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Visible) {
            if (retVal == true) {
                $(this.gridElementIdJQ).jqGrid('setCell', row.RowIDProperty, 'INL_' + childName + '_' + childIdx + '_' + rule.UIElementName, '', { visibility: 'visible' });
                this.setChildElementsIsVisible(this.gridElementIdJQ, row.RowIDProperty, 'INL_' + childName + '_' + childIdx + '_' + rule.UIElementName, rule.UIElementTypeID);
            }
            else {
                $(this.gridElementIdJQ).jqGrid('setCell', row.RowIDProperty, 'INL_' + childName + '_' + childIdx + '_' + rule.UIElementName, '', { visibility: 'hidden' });
                this.setChildElementsIsVisible(this.gridElementIdJQ, row.RowIDProperty, 'INL_' + childName + '_' + childIdx + '_' + rule.UIElementName, rule.UIElementTypeID);
                this.removeVisibleAndDisabledElementFromErrorGrid(row.RowIDProperty, rule.UIElementName);
            }
        }
        else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Enabled) {
            if (retVal == true) {
                $(this.gridElementIdJQ).jqGrid('setCell', row.RowIDProperty, 'INL_' + childName + '_' + childIdx + '_' + rule.UIElementName, '', '', { disabled: null });
                this.setChildElementsIsDisabled(this.gridElementIdJQ, row.RowIDProperty, 'INL_' + childName + '_' + childIdx + '_' + rule.UIElementName, rule.UIElementTypeID);
            }
            else {
                $(this.gridElementIdJQ).jqGrid('setCell', row.RowIDProperty, 'INL_' + childName + '_' + childIdx + '_' + rule.UIElementName, '', '', { disabled: 'disabled' });
                this.setChildElementsIsDisabled(this.gridElementIdJQ, row.RowIDProperty, 'INL_' + childName + '_' + childIdx + '_' + rule.UIElementName, rule.UIElementTypeID);
                this.removeVisibleAndDisabledElementFromErrorGrid(row.RowIDProperty, rule.UIElementName);
            }
        }
        else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Error) {
            var iCol = getColumnSrcIndexByName($(this.gridElementIdJQ), 'INL_' + childName + '_' + childIdx + '_' + rule.UIElementName);
            var el = $("tr#" + row.RowIDProperty, this.gridElementIdJQ).find('td').eq(iCol);
            if ($(el).css('visibility') != undefined && $(el).css('visibility') == 'visible' && $(el)[0].attributes.disabled == undefined) {
                this.setElementErrorState(retVal, rule, row);
            }
        }
        else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.IsRequired) {
            this.setElementIsRequired(retVal, rule, row);
        }
    }
    else {
        var parentRowId = row.RowIDProperty;
        var childBuilder = this.childGridBuilders.filter(function (builder) {
            return builder.parentRowId == parentRowId;
        });
        if (childBuilder != null && childBuilder.length > 0) {
            if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Value) {
                $(childBuilder[0].gridElementIdJQ).jqGrid('setCell', childBuilder[0].data[childIdx][this.KeyProperty], rule.UIElementName, retVal);
            }
            else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Visible) {
                if (retVal == true) {
                    $(childBuilder[0].gridElementIdJQ).jqGrid('setCell', childBuilder[0].data[childIdx][this.KeyProperty], rule.UIElementName, '', { visibility: 'visible' });
                    this.setChildElementsIsVisible(this.gridElementIdJQ, childBuilder[0].data[childIdx][this.KeyProperty], rule.UIElementName, rule.UIElementTypeID);
                }
                else {
                    $(childBuilder[0].gridElementIdJQ).jqGrid('setCell', childBuilder[0].data[childIdx][this.KeyProperty], rule.UIElementName, '', { visibility: 'hidden' });
                    this.setChildElementsIsVisible(this.gridElementIdJQ, childBuilder[0].data[childIdx][this.KeyProperty], rule.UIElementName, rule.UIElementTypeID);
                    this.removeVisibleAndDisabledElementFromErrorGrid(row.RowIDProperty, rule.UIElementName);
                }
            }
            else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Enabled) {
                if (retVal == true) {
                    $(childBuilder[0].gridElementIdJQ).jqGrid('setCell', childBuilder[0].data[childIdx][this.KeyProperty], rule.UIElementName, '', '', { disabled: null });
                    this.setChildElementsIsDisabled(this.gridElementIdJQ, childBuilder[0].data[childIdx][this.KeyProperty], rule.UIElementName, rule.UIElementTypeID);
                }
                else {
                    $(childBuilder[0].gridElementIdJQ).jqGrid('setCell', childBuilder[0].data[childIdx][this.KeyProperty], rule.UIElementName, '', '', { disabled: 'disabled' });
                    this.setChildElementsIsDisabled(this.gridElementIdJQ, childBuilder[0].data[childIdx][this.KeyProperty], rule.UIElementName, rule.UIElementTypeID);
                    this.removeVisibleAndDisabledElementFromErrorGrid(row.RowIDProperty, rule.UIElementName);
                }
            }
            else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Error) {
                var iCol = getColumnSrcIndexByName($(this.gridElementIdJQ), rule.UIElementName);
                var el = $("tr#" + childBuilder[0].data[childIdx][this.KeyProperty], this.gridElementIdJQ).find('td').eq(iCol);
                if ($(el).css('visibility') != undefined && $(el).css('visibility') == 'visible' && $(el)[0].attributes.disabled == undefined) {
                    this.setElementErrorState(retVal, rule, row);
                }
            }
            else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.IsRequired) {
                this.setElementIsRequired(retVal, rule, row);
            }
        }
    }
}

repeaterBuilderMedical.prototype.gridMethods = function () {
    var currentInstance = this;
    return {
        processRules: function () {
            for (var idx = 0; idx < currentInstance.rules.length; idx++) {
                if (currentInstance.rules[idx].TargetPropertyTypeId != ruleProcessor.TargetPropertyTypes.Value) {
                    if (currentInstance.gridType == 'child') {
                        currentInstance.runRuleForRepeater(currentInstance.rules[idx], currentInstance.parentRowId);
                    }
                    else {
                        currentInstance.runRuleForRepeater(currentInstance.rules[idx]);
                    }
                }
            }
        }
    }
}

repeaterBuilderMedical.prototype.saveRow = function (isDataBinded, bulkUpdateRowData) {
    var currentInstance = this;
    var isEdited = false;
    var dataBeforeChanges = null;
    var saveParameters = {
        "url": 'clientArray',
        "aftersavefunc": function () {
            currentInstance.bindData();
        }
    };
    try {
        if (currentInstance.lastSelectedRow != null) {
            $(currentInstance.gridElementIdJQ).jqGrid('saveRow', currentInstance.lastSelectedRow, saveParameters);
        }
        //$(this.gridElementIdJQ).jqGrid('restoreRow', currentInstance.lastSelectedRow);
    } catch (ex) {
    }
    var rowData = $(currentInstance.gridElementIdJQ).getRowData(currentInstance.lastSelectedRow);
    if (bulkUpdateRowData != undefined) {
        rowData = bulkUpdateRowData;
    }

    if (currentInstance.lastSelectedRow != null) {
        //copy over primary values
        var row = currentInstance.data.filter(function (dt) {
            return dt.RowIDProperty == currentInstance.lastSelectedRow;
        });
        if (row.length == 0) {
            row = {};
            row.RowIDProperty = rowData.RowIDProperty;
            currentInstance.data.push(row);
        }
        else {
            row = row[0];
        }

        if (currentInstance.fullName.split('.')[0] == "CostShare" && isDataBinded == true) {
            dataBeforeChanges = $.extend(true, {}, currentInstance);
        }

        for (var prop in rowData) {
            if (currentInstance.displayChildGridAsPopup && prop == "") {
                continue;
            }
            if (prop.substring(0, 4) != 'INL_' && prop != this.KeyProperty) {
                row[prop] = rowData[prop];
            }
        }
        for (var prop in rowData) {
            if (prop.substring(0, 4) == 'INL_') {
                var propArr = prop.split('_');
                if (propArr.length == 4) {
                    var dsName = propArr[1];
                    var propIdx = propArr[2];
                    var propName = propArr[3];
                    row[dsName][propIdx][propName] = rowData[prop];
                }
            }
        }
    }



    if (currentInstance.lastSelectedRow != null) {
        currentInstance.updateTargetDataSourceCollection(currentInstance.lastSelectedRow, "save");
    }

    if (currentInstance.rowDataBeforeEdit) {
        //check if object is changed
        if (!(JSON.stringify(currentInstance.rowDataBeforeEdit) === JSON.stringify(rowData))) {
            currentInstance.customruleMedical.addActivityLog(currentInstance, rowData);
            currentInstance.formInstanceBuilder.hasChanges = true;
            currentInstance.hasChanges = true;
        }
        currentInstance.rowDataBeforeEdit = undefined;
    }
    //check for Admin Form Repeaters
    if (currentInstance.customruleMedical.adminFeeTotals.isAdminFeeSectionRep(currentInstance)) {
        currentInstance.customruleMedical.adminFeeTotals.getUpdatedDataOnChange(currentInstance);
        currentInstance.customruleMedical.adminFeeTotals.getUpdatedDataForSumRep(currentInstance);
        var rateSumRep = currentInstance.customruleMedical.adminFeeTotals.getRateSumRepeater(currentInstance);
        if (rateSumRep != null && rateSumRep !== undefined && rateSumRep != '') {
            var rateSumRepId = rateSumRep[0].gridElementIdJQ;
        }

        $(rateSumRepId).jqGrid('GridUnload');
        var rateTypeWiseSumRep = currentInstance.formInstanceBuilder.repeaterBuilders.filter(function (ct) {
            return ct.fullName == currentInstance.customruleMedical.AdminFeeTotalsConstants.adminRateSumRep;
        });
        if (rateTypeWiseSumRep != null && rateTypeWiseSumRep !== undefined && rateTypeWiseSumRep != '' && rateTypeWiseSumRep.length > 0) {
            rateTypeWiseSumRep[0].columnModels = [];
            rateTypeWiseSumRep[0].columnNames = [];
            rateTypeWiseSumRep[0].gridElementId = null;
            rateTypeWiseSumRep[0].gridElementIdJQ = null;
            rateTypeWiseSumRep[0].groupHeaders = [];
            rateTypeWiseSumRep[0].build();
        }

    }
}

repeaterBuilderMedical.prototype.addEntryToAcitivityLogger = function (rowId, operation, colName, oldValue, newValue, buFilterCriteria, childGridPath, prop, customMsg)
{
    var currentInstance = this;
    var fields = currentInstance.fullName.split('.');
    var field = fields[fields.length - 1];
    //var colNames = $(currentInstance.gridElementIdJQ).jqGrid("getGridParam", "colNames");
    //var colName = colNames[currentInstance.lastColumnSelected];
    var updatedDate = new Date();
    var sectionDetails = currentInstance.formInstanceBuilder.designData.Sections.filter(function (ct) {
        return ct.Label == currentInstance.formInstanceBuilder.selectedSectionName;
    });

    var path = undefined;
    var fullName = "";
    if (currentInstance.fullName.indexOf("" + currentInstance.design.GeneratedName + "")) {
        path = currentInstance.formInstanceBuilder.selectedSectionName + getElementHierarchy(sectionDetails[0], currentInstance.fullName);
    }

    if (childGridPath == null)
        fullName = path + " => " + currentInstance.design.Label;
    else {
        fullName = path + " => " + childGridPath;
        currentInstance.design.Label = childGridPath;
    }
    var headercolumnname = "";
    if (prop != undefined) {
        if (prop.indexOf("_") >= 0) {
            if (prop.substring(0, 4) == 'INL_') {
                var proarr = prop.split('_');
                if (proarr.length == 4) {
                    headercolumnname = proarr[2];
                }
            }
        }
    }

    var repeaterId = currentInstance.design.Name + currentInstance.formInstanceId;
    var keyValue = "";
    if (customMsg == undefined) {
        $.each(currentInstance.columnModels, function (index, element) {
            if (element.iskey == true) {
                if (element.name.indexOf("_") >= 0) {
                    if (element.name.substring(0, 4) == 'INL_') {
                        var keyarr = element.name.split('_');
                        if (keyarr.length == 4) {
                            if (headercolumnname == keyarr[2]) {
                                var row = currentInstance.data.filter(function (dt) { return dt.RowIDProperty == rowId });
                                if (row[0] != undefined) {
                                    var elementName = element.name.split("_");
                                    var eleName = elementName[elementName.length - 1];
                                    var value = row[0][currentInstance.design.ChildDataSources[0].DataSourceName][elementName[elementName.length - 2]][eleName];
                                    keyValue += value + " ";
                                }
                            }
                        }
                    }
                }
                else {
                    var row = currentInstance.data.filter(function (dt) { return dt.RowIDProperty == rowId });
                    if (row[0] != undefined) {
                        var value = row[0][element.name];
                        keyValue += value + " ";
                    }
                }
            }
        });
    }
    

    //var index = $(currentInstance.gridElementIdJQ).jqGrid('getInd', rowId);
    //if (operation == 2)
      //  index = parseInt(rowId) + 1;

    activitylogger.logRepeater(fullName, colName, rowId, operation, currentUserName, updatedDate, currentInstance.design.Label, parseInt(currentInstance.formInstanceId), oldValue, newValue, buFilterCriteria, currentInstance.design, repeaterId, keyValue, customMsg);
    
}
repeaterBuilderMedical.prototype.getElementValue = function (elementType, element, val) {
    var retVal = $(element).val();
    if (elementType == 'checkbox') {
        if ($(element).prop('checked') == true) {
            retVal = 'Yes';
        }
        else {
            retVal = 'No';
        }
    }
    return retVal;
}

repeaterBuilderMedical.prototype.getDropdownTextboxOptions = function (items, isSortRequired) {
    //sorts options in ascending order
    if (isSortRequired == true) {
        items = this.sortOptions(items);
    }

    var options = "";
    if (items != null && items.length > 0) {
        options = options + Validation.selectOne + ':' /*+ Validation.selectOne */ + ';';
        for (var idx = 0; idx < items.length; idx++) {
            if (items[idx].ItemValue != '') {
                options = options + items[idx].ItemValue + ':' + items[idx].ItemValue;
                if (idx < items.length - 1) {
                    options = options + ';';
                }
            }
        }
    }
    //to check when there are no items attached from design document.
    if (options == null || options == 'undefined' || options == "") {
        options = Validation.selectOne + ':' /*+ Validation.selectOne */ + ";" + "newItem" + ':' + "Enter Unique Response";
    }
    else {
        if (options.charAt(options.length - 1) != ";")
            options = options + ";";
        options = options + "newItem" + ':' + "Enter Unique Response";
    }
    return options;
}

repeaterBuilderMedical.prototype.formatDropdownTextbox = function (idpluscol, elementDesign, rowId) {
    var currentInstance = this;
    var count = 0;
    //get current value of the row element
    var currentVal = $('#' + idpluscol).closest('td').attr('title');
    $('#' + idpluscol).addClass("ddt-dropdown");
    //if (currentVal === "Not Applicable" || currentVal === "No Copay") {
    //    $('#' + idpluscol).closest('td').attr("disabled", "disabled");
    //}
    if (currentVal != undefined) {
        $.each(elementDesign.Items, function (key, item) {
            if (item.ItemValue.toUpperCase() === currentVal.toUpperCase())
                count++;
        });
        if (currentVal != "" && count == 0) {
            $('<option class="standard-optn" value="' + currentVal + '">' + currentVal + '</option>').insertBefore($('#' + idpluscol).find('option').last());
            $('#' + idpluscol).val(currentVal);
        }
    }


    $("#" + idpluscol, this.gridElementIdJQ).parent().append('<input id="' + idpluscol + 'Textbox" type="text" name="' + elementDesign.GeneratedName + '" style="display:none;" class="ddt-textbox someinput ' + elementDesign.DataType + '"></input>');
    //$("#" + idpluscol).unbind('change');
    $("#" + idpluscol).change(function () {
        if ($(this).val() == 'newItem') {
            var len = $(this).parent().find('.ddt-dropdown option').length;
            var val = $(this).parent().find('.ddt-dropdown option')[len - 2].value;
            var uniqueVal = ($(this).parent().find('.ddt-dropdown option').hasClass("standard-optn")) ? val : "";

            $(this).val(uniqueVal);
            $(this).parent().find('.ddt-textbox').val(uniqueVal);
            $(this).parent().find('.ddt-textbox').toggle().focus();
            $(this).toggle();
        }
    });
    //$('.ddt-textbox').unbind('focusout');
    $('.ddt-textbox', currentInstance.gridElementIdJQ).on("focusout", function (e) {
        $(this).toggle();
        $(this).parent().find($("#" + idpluscol)).toggle();

    });

    //$('.ddt-textbox', currentInstance.gridElementIdJQ).on("mouseup", function (e) {
    //    var newValue;
    //    var $input = $(this),
    //        oldValue = $input.val();

    //    if (oldValue == "") return;

    //    if (($(this).parent().find('#' + idpluscol + ' option').hasClass("standard-optn") == true)) {
    //        var remove = true;
    //    }

    //    // When this event is fired after clicking on the clear button
    //    // the value is not cleared yet. We have to wait for it.
    //    setTimeout(function () {
    //         newValue = $input.val();

    //         if (newValue == "") {

    //            if (($input.parent().find('#' + idpluscol + ' option').hasClass("standard-optn") == true)) {
    //                $input.parent().find('#' + idpluscol + ' option.standard-optn').remove();
    //            }
    //            $input.trigger("cleared");
    //        }
    //    }, 1);
    //});


    //$('.ddt-textbox').unbind('keyup');
    $('.ddt-textbox', currentInstance.gridElementIdJQ).on("keyup", function (e) {
        if (e.which !== 16) {
            var newValue = $(this).val();
            var dropdownTextboxControl = $(this).parent().find('#' + idpluscol);
            var stringUtility = new globalutilities();
            if (!stringUtility.stringMethods.isNullOrEmpty(newValue)) {
                if ($(this).parent().find('#' + idpluscol + ' option').hasClass("standard-optn") == true) {
                    $(this).parent().find('#' + idpluscol + ' option.standard-optn').remove();
                }
                //check if unique response is the same item as Items from document design
                var existingItem = false;
                $.each(elementDesign.Items, function (key, elem) {
                    if (elem.ItemValue.toUpperCase() === newValue.toUpperCase()) {
                        existingItem = true;
                        dropdownTextboxControl.val(elem.ItemValue);
                    }
                });
                if (existingItem == false) {
                    $('<option value="' + newValue + '" class="standard-optn">' + newValue + '</option>').insertBefore($(this).parent().find('#' + idpluscol + ' option').last());
                    dropdownTextboxControl.val(newValue);
                    dropdownTextboxControl.trigger("change");
                }
            }
            else {
                dropdownTextboxControl.val('[Select One]').trigger('change');
            }
        }
    });
}

repeaterBuilderMedical.prototype.isPrimaryDataExits = function () {
    var currentInstance = this;
    var isRepeaterData = true;
    var count = 0;
    var rowCount = 0;
    if (currentInstance.data.length == 1) {
        for (var key in currentInstance.data[0]) {
            if (jQuery.type(currentInstance.data[0][key]) === "string") {
                rowCount++;
                if (currentInstance.data[0][key] == '') {
                    count++;
                }
            }
        }
        if (rowCount == count) {
            isRepeaterData = false;
        }
    }
    return isRepeaterData;
}

//TO DO : pass parameter for add update and delete row
repeaterBuilderMedical.prototype.updateTargetDataSourceCollection = function (rowID, status) {
    var currentInstance = this;
    var rowIndex = undefined;

    //index of source repeater
    var sourceindex = $(currentInstance.gridElementIdJQ).jqGrid('getGridParam', '_index');

    var rowID = rowID;

    if (sourceindex != undefined) {
        rowIndex = sourceindex[rowID];
    }

    if (rowIndex != undefined) {
        //get all datasource whose source repeater is current repeater
        var targetRepeaterMappings = currentInstance.formInstanceBuilder.designData.DataSources.filter(function (ct) {
            if (ct.SourceParent == currentInstance.fullName) {
                return ct;
            }
        });

        //update data in target repeater whose display mode is primary
        if (targetRepeaterMappings != null && targetRepeaterMappings != undefined) {
            $.each(targetRepeaterMappings, function (idx, ct) {
                var isFilter = false;

                $.each(ct.Mappings, function (idx, dt) {
                    if (dt.Filter != null) {
                        isFilter = true;
                        return false;
                    }
                })

                if (isFilter == false && ct.DataSourceModeType != "Manual") {
                    if (currentInstance.formInstanceBuilder.formDesignId == ct.FormDesignID && (currentInstance.formInstanceBuilder.selectedSectionName).replace(/ +/g, "") == ct.TargetParent.split('.')[0]) {

                        var targetSection = ct.TargetParent;

                        //get target repeater
                        var targetRepeater = currentInstance.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                            return targetSection == rb.fullName;
                        });

                        //create instance of datasourcesync
                        var sourcesynchroniser = new datasourcesync(currentInstance.formInstanceBuilder, currentInstance, ct, rowIndex, status);

                        //switch to data source 
                        switch (ct.DisplayMode) {

                            case "Primary":

                                sourcesynchroniser.syncMapPrimaryDataSource();

                                //if section is loaded rebind the data
                                if (targetRepeater.length != 0) {
                                    currentInstance.updateTargetRepeater(rowIndex, rowID, targetRepeater[0], status);

                                    if (status == 'add' && currentInstance.data.length == 1) {
                                        $(targetRepeater[0].gridElementIdJQ).jqGrid('GridUnload');
                                        targetRepeater[0].columnModels = [];
                                        targetRepeater[0].columnNames = [];
                                        targetRepeater[0].gridElementId = null;
                                        targetRepeater[0].gridElementIdJQ = null;
                                        targetRepeater[0].groupHeaders = [];
                                        targetRepeater[0].build();
                                    }
                                }
                                break;

                            case "In Line":

                                sourcesynchroniser.syncMapInLineDataSource();

                                //if section is loaded render the section
                                if (targetRepeater.length != 0) {
                                    $(targetRepeater[0].gridElementIdJQ).jqGrid('GridUnload');

                                    targetRepeater[0].columnModels = [];
                                    targetRepeater[0].columnNames = [];
                                    targetRepeater[0].gridElementId = null;
                                    targetRepeater[0].gridElementIdJQ = null;
                                    targetRepeater[0].groupHeaders = [];
                                    targetRepeater[0].build();
                                }
                                break;

                            case "Child":

                                sourcesynchroniser.syncMapChildDataSource();

                                //if section is loaded
                                if (targetRepeater.length != 0) {

                                    //loop for loaded child grid 
                                    $.each(targetRepeater[0].childGridBuilders, function (idx, childGridBuilder) {

                                        if (childGridBuilder.length != 0) {
                                            currentInstance.updateTargetRepeater(rowIndex, rowID, childGridBuilder, status);
                                        }
                                    });
                                }
                                break;

                            case "Dropdown":

                                var dropdwonelement = undefined;

                                if (targetRepeater.length == 0) {
                                    dropdwonelement = currentInstance.formInstanceBuilder.getSectionDropdownElement(currentInstance, ct);

                                    if (dropdwonelement != undefined) {
                                        //for section dropdown
                                        currentInstance.formInstanceBuilder.updateTargetSectionDropdown(currentInstance, rowIndex, ct, status, dropdwonelement, sourcesynchroniser);
                                    }
                                    else {
                                        //for repeater dropdown
                                        dropdwonelement = currentInstance.getRepeaterDropdownElement(ct);
                                        var itemsArray = sourcesynchroniser.updateRepeaterDropDown(dropdwonelement[1], status);
                                        currentInstance.updateRepeaterDropdownSelectedValue(targetRepeater, itemsArray, ct, sourcesynchroniser);
                                    }
                                }
                                else {
                                    currentInstance.updateTargetRepeaterDropdown(targetRepeater, rowIndex, ct, status, sourcesynchroniser);
                                }
                                break;
                        }
                    }
                    //clear object of synchroniser class
                    sourcesynchroniser = null
                }
            });

        }
    }
}

repeaterBuilderMedical.prototype.updateTargetRepeater = function (rowIndex, rowID, targetRepeater, status) {
    var currentInstance = this;


    //get data id of target repeater
    var targetRepeaterDataIdList = $(targetRepeater.gridElementIdJQ).jqGrid("getDataIDs");

    //get data of target repeater
    var targetData = $(targetRepeater.gridElementIdJQ).jqGrid('getGridParam', 'data');

    if (targetData != undefined) {

        // if (currentInstance.data.length == targetData.length) {
        if (status == "save") {
            var updateflag = undefined;
            if (targetData[rowIndex] != undefined) {

                //update row if rowid present targetRepeaterDataIdList of target repeater
                for (var i = 0; i < targetRepeaterDataIdList.length; i++) {
                    if (targetRepeaterDataIdList[i] == rowID) {
                        updateflag = true;
                        $(targetRepeater.gridElementIdJQ).jqGrid('setRowData', targetRepeater.data[rowIndex].RowIDProperty, targetRepeater.data[rowIndex]);
                        break;
                    }
                }

                //update row using data of target repeater if flag is not true
                if (updateflag != true) {
                    var rowItem = targetData[rowIndex];
                    if (rowItem != undefined) {
                        $.each(targetRepeater.data[rowIndex], function (el) {
                            rowItem[el] = targetRepeater.data[rowIndex][el];
                        });
                    }
                }
            }
        }
        //else if (currentInstance.data.length < targetData.length) {
        if (status == "delete") {
            var deleteflag = undefined;

            //delete row if row index is present in targetRepeaterDataIdList of target repeater
            for (var i = 0; i < targetRepeaterDataIdList.length; i++) {
                if (targetRepeaterDataIdList[i] == rowID) {
                    deleteflag = true;
                    $(targetRepeater.gridElementIdJQ).delRowData(rowID);
                    break;
                }
            }

            //if flag is not true then delete row from data of target repeater
            if (deleteflag != true) {
                //delete row using row index
                if (rowIndex != undefined) {
                    targetData.splice(rowIndex, 1);
                }
            }
        }
        if (status == "add") {
            var isRowID = false;
            if (targetData[rowIndex] == undefined) {

                $.each(targetData, function (idx, rd) {
                    if (rd.RowIDProperty == rowID) {
                        isRowID = true;
                        return false;
                    }
                });
                if (isRowID == false) {
                    targetRepeater.data[rowIndex].RowIDProperty = rowID;
                    $(targetRepeater.gridElementIdJQ).jqGrid('addRowData', targetRepeater.data[rowIndex].RowIDProperty, targetRepeater.data[rowIndex]);
                }
            }
        }
    }
}

repeaterBuilderMedical.prototype.setElementIsRequired = function (result, rule, row) {
    var currentInstance = this;
    var elem = $('#' + row.RowIDProperty + "_" + rule.UIElementName);
    if (currentInstance.formInstanceBuilder.designData.Validations == null) {
        currentInstance.formInstanceBuilder.designData.Validations = [];
    }
    var validation = currentInstance.formInstanceBuilder.designData.Validations.filter(function (val) {
        return val.UIElementName == rule.UIElementFormName;
    });
    if (result == true) {
        if (validation != null && validation.length > 0) {
            validation[0].IsRequired = true;
        }
        else {
            validation = { FullName: rule.UIElementFullName, UIElementName: rule.UIElementFormName, IsRequired: true, IsError: false, Regex: '', ValidationMessage: '', HasMaxLength: '', MaxLength: '', DataType: '', IsActive: true, ValidationType: 'Temporary' };
            currentInstance.formInstanceBuilder.designData.Validations.push(validation);
        }
    }
    else {
        if (validation != null && validation.length > 0) {
            if (validation[0].ValidationType == 'Temporary') {
                var delIdx;
                for (var idx = 0; idx < currentInstance.formInstanceBuilder.designData.Validations.length; idx++) {
                    var validation = currentInstance.formInstanceBuilder.designData.Validations[idx];
                    if (validation.UIElementName == rule.UIElementFormName) {
                        delIdx = idx;
                        break;
                    }
                }
                if (delIdx != null) {
                    currentInstance.formInstanceBuilder.designData.Validations.splice(delIdx, 1);
                }
            }
            else {
                validation[0].IsRequired = false;
            }
        }
    }

    var repeaterIndexes = $(currentInstance.gridElementIdJQ).jqGrid('getGridParam', '_index');

    var rowIndex = repeaterIndexes[row.RowIDProperty];

    //var val = currentInstance.formInstanceBuilder.ruleProcessor.getOperandValue(rule.UIElementFullName);
    var uiElementFullNameArray = rule.UIElementFullName.split(".");
    var val = row[uiElementFullNameArray[uiElementFullNameArray.length - 1]];
    var validationError = currentInstance.formInstanceBuilder.formValidationManager.handleValidation(rule.UIElementFullName, val, rowIndex, '', row.RowIDProperty);
    if (validationError) {
        currentInstance.formInstanceBuilder.validation.handleObjectChangeValidation(validationError);
        currentInstance.showValidatedControlsOnRepeaterElementChange(validationError);
    }
}

repeaterBuilderMedical.prototype.setElementErrorState = function (result, rule, row) {
    var currentInstance = this;
    var elem = $('#' + row.RowIDProperty + "_" + rule.UIElementName);
    if (currentInstance.formInstanceBuilder.designData.Validations == null) {
        currentInstance.formInstanceBuilder.designData.Validations = [];
    }
    var validation = currentInstance.formInstanceBuilder.designData.Validations.filter(function (val) {
        return val.UIElementName == rule.UIElementFormName;
    });
    if (result == true) {
        if (validation != null && validation.length > 0) {
            validation[0].IsError = false;
        }
    }
    else {
        if (validation != null && validation.length > 0) {
            validation[0].IsError = true;
        }
        else {
            validation = { FullName: rule.UIElementFullName, UIElementName: rule.UIElementFormName, IsRequired: false, IsError: true, Regex: '', ValidationMessage: '', HasMaxLength: '', MaxLength: '', DataType: '', IsActive: true, ValidationType: 'Temporary' };
            currentInstance.formInstanceBuilder.designData.Validations.push(validation);
        }
    }

    var repeaterIndexes = $(currentInstance.gridElementIdJQ).jqGrid('getGridParam', '_index');

    var rowIndex = repeaterIndexes[row.RowIDProperty];

    var keyValue = "";

    var repeaterId = currentInstance.design.Name + currentInstance.formInstanceId;
    $.each(currentInstance.design.Elements, function (index, element) {
        if (element.IsKey == true) {
            var rowData = $("#" + repeaterId).jqGrid('getRowData', rowIndex);
            var value;
            if (currentInstance.rowDataBeforeEdit != undefined)
                value = currentInstance.rowDataBeforeEdit[element.GeneratedName];
            else
                value = rowData[element.GeneratedName];

            keyValue += value + " ";
        }
    });

    if (keyValue != "") {
        rowIndex = rowIndex + "|" + keyValue;
    }

    //var val = currentInstance.formInstanceBuilder.ruleProcessor.getOperandValue(rule.UIElementFullName);
    var uiElementFullNameArray = rule.UIElementFullName.split(".");
    var val = row[uiElementFullNameArray[uiElementFullNameArray.length - 1]];
    var validationError = currentInstance.formInstanceBuilder.formValidationManager.handleValidation(rule.UIElementFullName, val, rowIndex, '', row.RowIDProperty);
    if (validationError) {
        currentInstance.formInstanceBuilder.validation.handleObjectChangeValidation(validationError);
        currentInstance.showValidatedControlsOnRepeaterElementChange(validationError);
    }
}

//if repeater is not loaded
repeaterBuilderMedical.prototype.getRepeaterDropdownElement = function (dataSource) {
    var currentInstance = this;
    var fullNameArray = dataSource.TargetParent.split('.');

    var targetsection = undefined;

    var dropdwonelement = [];

    //get parent section of dropdown
    targetsection = currentInstance.formInstanceBuilder.designData.Sections.filter(function (ts) {
        return ts.GeneratedName == fullNameArray[0];
    });

    if (fullNameArray[1] != undefined) {
        targetsection = targetsection[0].Elements.filter(function (sd) {
            return sd.GeneratedName == fullNameArray[1];
        });


        for (var l = 2; l < fullNameArray.length; l++) {
            targetsection = targetsection[0].Section.Elements.filter(function (sd) {
                return sd.GeneratedName == fullNameArray[l];
            });
        }

        //get dropdown element
        if (targetsection[0].Type == "repeater") {
            dropdwonelement[0] = targetsection[0].Repeater.Name;
            $.each(targetsection[0].Repeater.Elements, function (idx, dl) {
                if ((dl.Type == 'select' || dl.Type == "SelectInput") && dl.GeneratedName == [dataSource.Mappings[0].TargetElement]) {
                    return dropdwonelement[1] = dl;
                }
            });
        }
        else {
            //get dropdown element
            if (targetsection[0].Type == "repeater") {
                dropdwonelement[0] = targetsection[0].Repeater.Name;
                $.each(targetsection[0].Repeater.Elements, function (idx, dl) {
                    if ((dl.Type == 'select' || dl.Type == "SelectInput") && dl.GeneratedName == [dataSource.Mappings[0].TargetElement]) {
                        return dropdwonelement[1] = dl;
                    }
                });
            }
        }
    }
    return dropdwonelement;
}

repeaterBuilderMedical.prototype.updateTargetRepeaterDropdown = function (targetRepeater, rowIndex, dataSource, status, sourcesynchroniser) {
    var currentInstance = this;
    var itemIndex = undefined, items = undefined, newitems = [];

    //get dropDownElement from source repeater
    var dropDownElement = sourcesynchroniser.getRepeaterDropdownElement(targetRepeater[0]);

    var itemsArray = sourcesynchroniser.updateRepeaterDropDown(dropDownElement, status);

    items = itemsArray[0];

    //remove blank items 
    items = items.filter(function (itm) {
        return itm !== ''
    });

    for (var i = 0; i < items.length; i++) {
        newitems[i] = items[i] + ":" + items[i];
    }

    newitems.splice(0, 0, "[Select One]:");

    if (dropDownElement.Type == "SelectInput") {
        newitems.splice(newitems.length, newitems.length, "newItem:Enter Unique Response");
    }

    newitems = newitems.join(';');

    //get column name of repeater
    var colname = dataSource.Mappings[0].TargetElement;

    //get column property of repeater
    var colprop = $(targetRepeater[0].gridElementIdJQ).jqGrid('getColProp', colname);

    var dropdownitems = colprop.editoptions.value.split(';');

    var previousItems = [];

    if (dropdownitems != undefined) {

        var previousDropdownItemLength = dropdownitems.length;

        //get previous dropdown item list
        for (var i = 0; i < dropdownitems.length; i++) {
            previousItems[i] = dropdownitems[i].split(':')[1];
        }
    }

    //set dropdown item property of column
    $(targetRepeater[0].gridElementIdJQ).setColProp(colname, { editoptions: { value: newitems } });

    itemsArray[0] = items;
    itemsArray[1] = previousItems;

    //update selected value of dropdown in repeater
    currentInstance.updateRepeaterDropdownSelectedValue(targetRepeater, itemsArray, dataSource, sourcesynchroniser);
}

repeaterBuilderMedical.prototype.updateRepeaterDropdownSelectedValue = function (targetRepeater, itemsArray, dataSource, sourcesynchroniser) {

    var items = itemsArray[0], previousItems = itemsArray[1], oldItem = [], newItem = [], targetCellList = [];

    if (targetRepeater[0] != undefined) {
        items.splice(0, 0, "");

        //get data id of target repeater
        var targetRepeaterDataIdList = $(targetRepeater[0].gridElementIdJQ).jqGrid("getDataIDs");

        //get data of target repeater
        var targetData = $(targetRepeater[0].gridElementIdJQ).jqGrid('getGridParam', 'data');
    }

    //get column name of repeater
    var colname = dataSource.Mappings[0].TargetElement;

    //compare previous item list with new item list 
    $.grep(previousItems, function (el) {
        if ($.inArray(el, items) == -1) oldItem.push(el);
    });

    //compare new item list with previous item list 
    $.grep(items, function (el) {
        if ($.inArray(el, previousItems) == -1) newItem.push(el);
    });

    //get data of target repeater
    targetRepeaterData = sourcesynchroniser.getTargetRepeaterData(dataSource.TargetParent);

    //get list of row which contain dropdown selected value is oldItem
    targetCellList = targetRepeaterData.filter(function (cl) {
        return cl[colname] == oldItem[0];
    });

    //update selected value with new item in repeater
    if (targetCellList.length != 0 && oldItem[0] != undefined) {
        for (var c = 0; c < targetCellList.length; c++) {
            targetCellList[c][colname] = newItem[0];

            if (targetRepeater[0] != undefined) {
                var updateflag = undefined;
                //update row if rowid present targetRepeaterDataIdList of target repeater
                for (var i = 0; i < targetRepeaterDataIdList.length; i++) {
                    if (targetRepeaterDataIdList[i] == targetCellList[c].RowIDProperty) {
                        updateflag = true;
                        $(targetRepeater[0].gridElementIdJQ).jqGrid('setCell', targetCellList[c].RowIDProperty, colname, newItem[0]);
                        break;
                    }
                }

                //update row using data of target repeater if flag is not true
                if (updateflag != true) {
                    var data = targetData.filter(function (dt) {
                        return dt.RowIDProperty == targetCellList[c].RowIDProperty
                    });

                    data[0][colname] = newItem[0];
                }
            }
        }
    }
}

repeaterBuilderMedical.prototype.checkDuplicate = function (rowId, loadFromServer) {
    var currentInstance = this;
    var hasError = false;

    var duplicationCheckEnabledElements = currentInstance.design.Elements.filter(function (ct) {
        if (ct.CheckDuplicate == true) {
            return ct;
        }
    });

    if (duplicationCheckEnabledElements.length > 0) {
        var gridRowData = '';
        if (currentInstance.design.LoadFromServer == true) {
            gridRowData = $(currentInstance.gridElementIdJQ).jqGrid('getRowData');
        }
        else {
            gridRowData = $(currentInstance.gridElementIdJQ).jqGrid('getGridParam', 'data');
        }

        var hashcolumnArray = getRepeaterHashColumnData(gridRowData, duplicationCheckEnabledElements);

        var rowData = undefined;
        var gridData = new Array();
        hashcolumnArray.filter(function (data, ct) {
            if (ct != rowId) {
                gridData.push(data.Data);
            }
            else if (ct == rowId) {
                rowData = data.Data;
            }
        });

        var found = 0;

        gridData.filter(function (data) {
            if (data === rowData) {
                found++;
            }
        });

        if (found > 0) {
            hasError = true;
            var duplicationErrorArray = currentInstance.formInstanceBuilder.formValidationManager.handleDuplication(currentInstance.fullName, gridRowData, rowId);
            if (duplicationErrorArray != undefined && duplicationErrorArray.length > 0) {
                for (var i = 0; i < duplicationErrorArray.length; i++) {
                    var duplicationError = duplicationErrorArray[i];
                    currentInstance.formInstanceBuilder.validation.handleObjectChangeValidation(duplicationError);
                    currentInstance.showDuplicateControlsOnRepeaterElementChange(duplicationError);
                }
                currentInstance.formInstanceBuilder.bottomMenu.closeBottomMenu();
                currentInstance.formInstanceBuilder.validation.loadValidationErrorGrid();
            }
            else {
                return;
            }

            var splitRowData = rowData.split('#');
            var isBlank = splitRowData.filter(function (dt) {
                if (dt != "blank" && dt != "[selectone]" && dt != "") {
                    return dt;
                }
            });

            if (isBlank.length == 0) {
                hasError = false;
                return hasError;
            }

            if (currentInstance.design.LoadFromServer == false) {
                $(currentInstance.gridElementIdJQ).jqGrid('setSelection', rowId);
            }

            duplicateMessageDialog.show(Common.duplicateValueMessage);
            setTimeout(function () { duplicateMessageDialog.hide(); }, 3000);

            if (currentInstance.design.LoadFromServer == false)
                currentInstance.setCursorOnEditRow(rowId, 0);
        }
        else {
            var duplicationErrorArray = currentInstance.formInstanceBuilder.formValidationManager.handleDuplication(currentInstance.fullName, gridRowData, rowId);
            if (duplicationErrorArray != undefined) {
                if (duplicationErrorArray.length > 0) {
                    for (var i = 0; i < duplicationErrorArray.length; i++) {

                        var duplicationError = duplicationErrorArray[i];
                        currentInstance.formInstanceBuilder.validation.handleObjectChangeValidation(duplicationError);
                        currentInstance.showDuplicateControlsOnRepeaterElementChange(duplicationError);
                    }
                    currentInstance.formInstanceBuilder.bottomMenu.closeBottomMenu();
                    currentInstance.formInstanceBuilder.validation.loadValidationErrorGrid();
                }
            }
        }
    }
    return hasError;
}

repeaterBuilderMedical.prototype.removeErrorGridRowOnRepeaterDeleteRow = function (rowId) {
    var currentInstance = this;
    var rowNum = undefined;

    //index repeater
    var index = $(currentInstance.gridElementIdJQ).jqGrid('getGridParam', '_index');

    var fullName = currentInstance.fullName;

    if (currentInstance.formInstanceBuilder.errorGridData.length > 0) {
        var selectedIndex = undefined;
        var sectionName = fullName.split('.')[0];
        $.each(currentInstance.formInstanceBuilder.errorGridData, function (i, el) {
            if (sectionName == el.Section.replace(/\s/g, '')) {
                selectedIndex = i;
            }
        });

        if (selectedIndex != undefined) {
            var sections = currentInstance.formInstanceBuilder.errorGridData[selectedIndex].ErrorRows.filter(function (ct) {
                return fullName == ct.SubSectionName.replace(/\ => /g, '.').replace(/\s/g, '');
            });

            if (sections != null && sections.length > 0) {
                for (var i = 0; i < sections.length; i++) {
                    if (sections[i].RowIdProperty == rowId) {
                        var Id = sections[i].ID;
                        $.each(currentInstance.formInstanceBuilder.errorGridData[selectedIndex].ErrorRows, function (idx, row) {
                            if (row.ID == Id) {
                                currentInstance.formInstanceBuilder.errorGridData[selectedIndex].ErrorRows.splice(idx, 1);
                                return false;
                            }
                        });
                    }
                }

                var sections = currentInstance.formInstanceBuilder.errorGridData[selectedIndex].ErrorRows.filter(function (ct) {
                    return fullName == ct.SubSectionName.replace(/\ => /g, '.').replace(/\s/g, '') && ct.RowIdProperty > rowId;
                });

                //Updating Row number in errorGrid
                for (var i = 0; i < sections.length; i++) {
                    sections[i].RowNum = sections[i].RowNum - 1;
                }
            }
        }
        if (currentInstance.formInstanceBuilder.designData.Duplications != null || currentInstance.formInstanceBuilder.designData.Duplications != undefined) {
            //code to remove Duplication Errors from Error Grid
            var duplicationObject = currentInstance.formInstanceBuilder.designData.Duplications.filter(function (ct) {
                return ct.ParentUIElementName == currentInstance.fullName;
            });

            if (duplicationObject.length > 0) {
                var gridRowData = $(currentInstance.gridElementIdJQ).jqGrid('getGridParam', 'data');
                var duplicationErrorArray = currentInstance.formInstanceBuilder.formValidationManager.handleDuplication(currentInstance.fullName, gridRowData, rowId, undefined, true);
                if (duplicationErrorArray.length > 0) {
                    for (var j = 0; j < duplicationErrorArray.length; j++) {
                        var duplicationError = duplicationErrorArray[j];
                        currentInstance.formInstanceBuilder.validation.handleObjectChangeValidation(duplicationError);
                        currentInstance.showDuplicateControlsOnRepeaterElementChange(duplicationError);
                    }
                }
            }
        }
        currentInstance.formInstanceBuilder.bottomMenu.closeBottomMenu();
        currentInstance.formInstanceBuilder.validation.loadValidationErrorGrid();
    }
}

repeaterBuilderMedical.prototype.addDefualtRow = function () {
    var row = {};
    for (var idx = 0; idx < this.columnModels.length; idx++) {
        row[this.columnModels[idx].index] = "";
    }
    var ids = [-1];
    for (var idx = 0; idx < this.data.length; idx++) {
        ids.push(this.data[idx].RowIDProperty);
    }
    var rowId = Math.max.apply(Math, ids) + 1;
    row[this.KeyProperty] = rowId;

    $(this.gridElementIdJQ).jqGrid('addRowData', rowId, row);
    if (this.design.PrimaryDataSource == null) {
        this.data.push(row);
    }

    //this.lastSelectedRow = 0;
    this.updateTargetDataSourceCollection(0, 'save');

    $(this.gridElementIdJQ).jqGrid('setSelection', rowId, focus());
}

//code to disable controls on chrome browser
repeaterBuilderMedical.prototype.setChildElementsIsDisabled = function (grid, rowId, columnName, uiElementTypeId) {
    var iCol = getColumnSrcIndexByName($(grid), columnName);
    var el = $("tr#" + rowId, grid).find('td').eq(iCol);
    var child = $(el).children().first();
    if (child.length > 0) {
        if ($(el)[0].attributes.disabled != undefined) {
            $(child).attr('disabled', 'disabled');
            if (uiElementTypeId == 6 || uiElementTypeId == "calendar") {
                $(child).siblings().attr('disabled', 'disabled');
            }
        }
        else {
            $(child).removeAttr('disabled');
            if (uiElementTypeId == 6 || uiElementTypeId == "calendar") {
                $(child).siblings().removeAttr('disabled');
            }
        }
    }
}

//code to hide controls on Visible Rule to avoid focusout after td is hidden
repeaterBuilderMedical.prototype.setChildElementsIsVisible = function (grid, rowId, columnName, uiElementTypeId) {
    var iCol = getColumnSrcIndexByName($(grid), columnName);
    var el = $("tr#" + rowId, grid).find('td').eq(iCol);
    var child = $(el).children().first();
    if (child.length > 0) {
        if ($(el).css('visibility') != undefined) {
            if ($(el).css('visibility') == 'hidden') {
                $(child).hide();
                if (uiElementTypeId == 6 || uiElementTypeId == "calendar") {
                    $(child).siblings().hide();
                }
            }
            else if ($(el).css('visibility') == 'visible') {
                $(child).show();
                if (uiElementTypeId == 6 || uiElementTypeId == "calendar") {
                    $(child).siblings().show();
                }
            }
        }
    }
}

repeaterBuilderMedical.prototype.removeVisibleAndDisabledElementFromErrorGrid = function (rowId, UIElementName) {
    var currentInstance = this;
    if (currentInstance.formInstanceBuilder.errorGridData.length > 0) {
        var selectedIndex;
        var index = undefined;
        var sectionName = currentInstance.fullName.substring(0, currentInstance.fullName.lastIndexOf('.'));
        $.each(currentInstance.formInstanceBuilder.errorGridData, function (i, el) {
            if (sectionName == el.Section.replace(/\s/g, '')) {
                selectedIndex = i;
                return false;
            }
        });

        if (selectedIndex != undefined) {
            $.each(currentInstance.formInstanceBuilder.errorGridData[selectedIndex].ErrorRows, function (idx, row) {
                if (row.RowIdProperty == rowId && row.GeneratedName == UIElementName && currentInstance.fullName == row.SubSectionName.replace(/\ => /g, '.').replace(/\s/g, '')) {
                    currentInstance.formInstanceBuilder.errorGridData[selectedIndex].ErrorRows.splice(idx, 1);
                    return false;
                }
            });
        }
    }
}

repeaterBuilderMedical.prototype.repeaterControlFocusOut = function (e) {
    var currentInstance = this;
    var elem = e.target;
    var rowId = e.target.id.split('_')[0];
    if (rowId != "" && rowId != undefined) {
        var designElem = currentInstance.design.Elements.filter(function (el) {
            return el.GeneratedName == elem.name;
        });
        if (designElem != null && designElem.length > 0) {
            designElem = designElem[0];
            var rules = currentInstance.formInstanceBuilder.rules.getRulesForElement(designElem.FullName);
            if (rules.length == 0) {

                var rowIndex = undefined;

                var ID = $(currentInstance.gridElementIdJQ).jqGrid("getDataIDs");
                $.each(ID, function (idx, ct) {
                    if (ct == rowId) {
                        rowIndex = idx;
                        return false;
                    }
                });
                var selectedRowId = $(currentInstance.gridElementIdJQ).jqGrid('getGridParam', 'selrow');
                var currentPage = $(currentInstance.gridElementIdJQ).getGridParam('page');
                var rowNum = $(currentInstance.gridElementIdJQ).getGridParam('rowNum');

                if (currentPage != 1) {
                    rowIndex = ((currentPage - 1) * rowNum) + rowIndex;
                }
                var keyValue = "";
                var repeaterId = currentInstance.design.Name + currentInstance.formInstanceId;
                $.each(currentInstance.design.Elements, function (index, element) {
                    if (element.IsKey == true) {
                        var rowData = $("#" + repeaterId).jqGrid('getRowData', selectedRowId);
                        var value;
                        if (currentInstance.rowDataBeforeEdit != undefined)
                            value = currentInstance.rowDataBeforeEdit[element.GeneratedName];
                        else
                            value = rowData[element.GeneratedName];

                        keyValue += value + " ";
                    }
                });

                if (keyValue != "") {
                    rowIndex = rowIndex + "|" + keyValue;
                }


                if (currentInstance.formInstanceBuilder.designData.Validations.length > 0) {
                    var validationError = currentInstance.formInstanceBuilder.formValidationManager.handleValidation(designElem.FullName, elem.value, rowIndex, '', selectedRowId);
                    if (validationError) {
                        currentInstance.formInstanceBuilder.validation.handleObjectChangeValidation(validationError);
                        currentInstance.showValidatedControlsOnRepeaterElementChange(validationError);
                    }
                }

                if (designElem.DataType == 'date' && elem.value != "") {
                    var validationError = currentInstance.formInstanceBuilder.formValidationManager.handleDateValidation(designElem.FullName, elem.value, rowIndex, '', selectedRowId);
                    if (validationError) {
                        currentInstance.formInstanceBuilder.validation.handleObjectChangeValidation(validationError);
                        currentInstance.showValidatedControlsOnRepeaterElementChange(validationError);
                    }
                }
                if (currentInstance.isDataChangesOnTab == true) {
                    if (currentInstance.design.LoadFromServer == true) {
                        var gridRowData = currentInstance.data;
                    }
                    else {
                        var gridRowData = $(currentInstance.gridElementIdJQ).jqGrid('getGridParam', 'data');
                    }

                    if (gridRowData.length > 1 && (currentInstance.formInstanceBuilder.designData.Duplications != null || currentInstance.formInstanceBuilder.designData.Duplications != undefined)) {
                        //code to remove Duplication Errors from Error Grid
                        var duplicationObject = currentInstance.formInstanceBuilder.designData.Duplications.filter(function (ct) {
                            return ct.FullName == designElem.FullName;
                        });

                        if (duplicationObject.length > 0) {
                            var gridData;
                            if (currentInstance.design.LoadFromServer == true) {
                                var gridRowData = currentInstance.data;
                                //var gridRowData = $(currentInstance.gridElementIdJQ).jqGrid('getRowData');
                                gridData = gridRowData.filter(function (rowData, index) {
                                    if (rowData.RowIDProperty == rowId) {
                                        currentInstance.design.Elements.filter(function (data, idx) {
                                            gridRowData[index][data.GeneratedName] = $('#' + rowId + '_' + data.GeneratedName).val();
                                        });
                                    }
                                    return rowData;
                                });
                            }
                            else {
                                gridData = gridRowData.filter(function (ct) {
                                    if (ct.RowIDProperty == rowId) {
                                        ct[designElem.GeneratedName] = elem.value;
                                    }
                                    return ct;
                                });
                            }
                            var duplicationErrorArray = currentInstance.formInstanceBuilder.formValidationManager.handleDuplication(duplicationObject[0].ParentUIElementName, gridData, rowId);
                            if (duplicationErrorArray != undefined && duplicationErrorArray.length > 0) {
                                for (var i = 0; i < duplicationErrorArray.length; i++) {
                                    var duplicationError = duplicationErrorArray[i];
                                    //$(e.currentTarget).refreshIndex();
                                    currentInstance.formInstanceBuilder.validation.handleObjectChangeValidation(duplicationError);
                                    currentInstance.showDuplicateControlsOnRepeaterElementChange(duplicationError);
                                }
                                currentInstance.formInstanceBuilder.bottomMenu.closeBottomMenu();
                                currentInstance.formInstanceBuilder.validation.loadValidationErrorGrid();
                            }
                        }
                    }
                    currentInstance.isDataChangesOnTab = false;
                }
            }
        }
    }
}

repeaterBuilderMedical.prototype.showValidatedControlsOnRepeaterElementChange = function (validationError) {
    var currentInstance = this;
    var errorRow = currentInstance.formInstanceBuilder.validation.getErrorGridRowObject(validationError);
    if (errorRow != undefined && errorRow != null) {
        var sections = currentInstance.formInstanceBuilder.errorGridData.filter(function (ct) {
            return ct.SectionID == errorRow.SectionID && ct.Section == errorRow.Section && ct.Form == errorRow.Form;
        });

        if (sections.length > 0) {
            var row = sections[0].ErrorRows.filter(function (ct) {
                return ct.SubSectionName == errorRow.SubSectionName && ct.GeneratedName == errorRow.GeneratedName && ct.RowIdProperty == errorRow.RowIdProperty;
            });

            if (row.length != 0) {
                currentInstance.applyValidation(row[0]);
            }
            else {
                currentInstance.removeValidationErrorOfControl(errorRow, currentInstance);
            }
        }
        else {
            currentInstance.removeValidationErrorOfControl(errorRow, currentInstance);
        }

        currentInstance.formInstanceBuilder.bottomMenu.closeBottomMenu();
        currentInstance.formInstanceBuilder.validation.loadValidationErrorGrid();
    }
}

repeaterBuilderMedical.prototype.removeValidationErrorOfControl = function (row) {
    var currentInstance = this;
    var colindex = undefined;
    if (row != undefined) {
        var repeaterID = row.ElementID.substring(0, row.ElementID.indexOf("_")) + row.FormInstanceID;
        var col = $("#" + repeaterID).jqGrid("getGridParam", "colModel");

        if (col != undefined) {
            $.each(col, function (idx, ct) {
                if (ct.name == row.GeneratedName) {
                    colindex = idx;
                    return false;
                }
            });
            var ind = $("#" + repeaterID).jqGrid('getGridRowById', row.RowIdProperty);
            if (ind) {
                var tcell = $("td:eq(" + colindex + ")", ind);
                $(tcell).removeClass("repeater-has-error");
            }
        }
    }
}

repeaterBuilderMedical.prototype.repeaterServerMethods = function () {
    var currentInstance = this;
    var selectRowData;
    var rowNo;
    return {
        checkDuplicationRow: function (rowId, rowdata, existingRowIndexList, duplicationObject) {
            selectRowData = rowdata;
            rowNo = rowId;

            try {
                var repeaterData = {
                    formInstanceId: currentInstance.formInstanceId,
                    fullName: currentInstance.fullName,
                    existingRowIndexList: existingRowIndexList,
                    rowData: JSON.stringify(rowdata),
                    duplicationObject: duplicationObject
                };
                currentInstance.checkDuplicationRowsWorker.onmessage = function (e) {
                    if (e.data != undefined) {
                        var journalGridData = new Array();
                        var isDuplicateRowFound = JSON.parse(e.data);
                        if (isDuplicateRowFound) {
                            var duplicationCheckEnabledElements = currentInstance.design.Elements.filter(function (ct) {
                                if (ct.CheckDuplicate == true) {
                                    return ct;
                                }
                            });

                            var columnNum = undefined;
                            for (var j = 0; j < duplicationCheckEnabledElements.length; j++) {
                                var duplicationObject = duplicationCheckEnabledElements[j];
                                var duplicationError = {
                                    FullName: duplicationObject.FullName,
                                    IsRequiredError: true,
                                    Message: '',
                                    UIElementName: duplicationObject.Name,
                                    RowIdProperty: parseInt(rowId),
                                    RowNumber: parseInt(rowId) + 1,
                                    ColumnNumber: columnNum,
                                    GeneratedName: duplicationObject.FullName.substring(duplicationObject.FullName.lastIndexOf('.') + 1),
                                    hasValidationError: true,
                                    value: 'CheckDuplicate'
                                };

                                currentInstance.formInstanceBuilder.validation.handleObjectChangeValidation(duplicationError);
                                currentInstance.showDuplicateControlsOnRepeaterElementChange(duplicationError);
                            }
                            currentInstance.formInstanceBuilder.bottomMenu.closeBottomMenu();
                            currentInstance.formInstanceBuilder.validation.loadValidationErrorGrid();

                            //$(currentInstance.gridElementIdJQ).editRow(rowId, true);

                            //duplicateMessageDialog.show(Common.duplicateValueMessage);
                            //setTimeout(function () { duplicateMessageDialog.hide(); }, 3000);

                            //currentInstance.setCursorOnEditRow(rowId, 0);

                        }
                    }
                };
                currentInstance.checkDuplicationRowsWorker.postMessage({
                    url: currentInstance.URLs.checkDuplicateRepeaterRows,
                    formInstanceId: currentInstance.formInstanceId,
                    saveData: JSON.stringify(repeaterData)
                });
            } catch (ex) {
                console.log(ex);
            }
        },

        checkModifyedRowDataAndDuplication: function (rowId, rowData, Action) {
            var duplicationObject = new Array();
            currentInstance.design.Elements.filter(function (ct) {
                if (ct.CheckDuplicate == true)
                    duplicationObject.push(ct.GeneratedName);
            });

            var existingRepeaterRowIndex = [];
            var gridData = $(currentInstance.gridElementIdJQ).jqGrid('getRowData');
            gridData.filter(function (data) {
                existingRepeaterRowIndex.push(data.RowIDProperty);
            });

            //remove added new rowid.
            if (currentInstance.formInstanceBuilder.repeaterRowIdList[currentInstance.design.GeneratedName]) {
                currentInstance.formInstanceBuilder.repeaterRowIdList[currentInstance.design.GeneratedName].filter(function (data) {
                    existingRepeaterRowIndex.pop(data);
                });
                var newRowIDList = currentInstance.formInstanceBuilder.repeaterRowIdList[currentInstance.design.GeneratedName];
                if ($.inArray(rowId, newRowIDList) > -1 && Action != "Delete") {
                    Action = "Add";
                }
            }

            var isDuplicateRow = true;
            var isValidRow = currentInstance.checkDuplicate(rowId, true);
            if (isValidRow == false && Action != "Delete") {
                currentInstance.loadFromServerObject.checkDuplicationRow(rowId, rowData, existingRepeaterRowIndex, duplicationObject);
            }
            //check rowData if already present in Key Object
            var matchCount = 0;
            var isExitsRow = [];

            if (!$.isEmptyObject(currentInstance.formInstanceBuilder.loadDataFromRepeater)) {
                for (var ele in currentInstance.formInstanceBuilder.loadDataFromRepeater) {
                    if (ele == currentInstance.design.GeneratedName) {
                        var repeaterDaltaObjectList = currentInstance.formInstanceBuilder.loadDataFromRepeater[currentInstance.design.GeneratedName];
                        var rowIndex;
                        isExitsRow = repeaterDaltaObjectList.filter(function (data, idx) {
                            if (data.rowObject.RowIDProperty == rowData.RowIDProperty) {
                                rowIndex = idx;
                                return data;
                            }
                        });
                        if (isExitsRow.length > 0) {
                            for (i = 0; i < repeaterDaltaObjectList.length; i++) {
                                matchCount = 0;
                                duplicationObject.filter(function (key) {
                                    if (rowData[key] == repeaterDaltaObjectList[i].rowObject[key]) {
                                        matchCount++;
                                    }
                                });
                                if (duplicationObject.length == matchCount && isExitsRow[0].rowObject.RowIDProperty == repeaterDaltaObjectList[i].rowObject.RowIDProperty) {
                                    if (Action == "Delete") {
                                        currentInstance.formInstanceBuilder.loadDataFromRepeater[currentInstance.design.GeneratedName][i].rowAction = Action;
                                    }
                                    else {
                                        currentInstance.formInstanceBuilder.loadDataFromRepeater[currentInstance.design.GeneratedName][i].rowObject = rowData;
                                    }
                                    return;
                                }
                            }//if key element is changed replace all data
                            if (matchCount <= duplicationObject.length && isExitsRow.length > 0) {
                                if (Action == "Add")
                                    currentInstance.formInstanceBuilder.loadDataFromRepeater[currentInstance.design.GeneratedName][rowIndex].rowAction = Action;
                                currentInstance.formInstanceBuilder.loadDataFromRepeater[currentInstance.design.GeneratedName][rowIndex].rowObject = rowData;
                            }
                        }
                        else {//add if not exits in repeaterobjectList
                            currentInstance.loadFromServerObject.saveRowData(rowId, rowData, Action);
                        }
                    }
                    else {//if every new repeater create new object
                        currentInstance.loadFromServerObject.saveRowData(rowId, rowData, Action);
                    }
                }
            }
            else {//add if data empty object found                   
                if (isExitsRow.length == 0)
                    currentInstance.loadFromServerObject.saveRowData(rowId, rowData, Action);
            }
        },

        addRepeaterNewRowId: function (rowid) {
            if (!currentInstance.formInstanceBuilder.repeaterRowIdList[currentInstance.design.GeneratedName]) {
                currentInstance.formInstanceBuilder.repeaterRowIdList[currentInstance.design.GeneratedName] = new Object(new Array());
                currentInstance.formInstanceBuilder.repeaterRowIdList[currentInstance.design.GeneratedName].push(rowid.toString())
            } else {
                if (!($.inArray(rowid, currentInstance.formInstanceBuilder.repeaterRowIdList[currentInstance.design.GeneratedName]) > -1))
                    currentInstance.formInstanceBuilder.repeaterRowIdList[currentInstance.design.GeneratedName].push(rowid.toString())
            }
        },

        saveRowData: function (rowId, rowData, Action) {
            var deltaObjectList = [];
            //Key Object
            deltaObjectList[0] = new Object({
                rowObject: rowData,
                rowAction: Action,
            });
            //check repeater fullpath is present in keyobject
            if (!currentInstance.formInstanceBuilder.loadDataFromRepeater[currentInstance.design.GeneratedName]) {
                currentInstance.formInstanceBuilder.loadDataFromRepeater[currentInstance.design.GeneratedName] = new Object(new Array());
                currentInstance.formInstanceBuilder.loadDataFromRepeater[currentInstance.design.GeneratedName].push(deltaObjectList[0]);
            } else {
                currentInstance.formInstanceBuilder.loadDataFromRepeater[currentInstance.design.GeneratedName].push(deltaObjectList[0])
            }
        }

    }
}

repeaterBuilderMedical.prototype.setRepeaterHeight = function () {
    //Elimination of the white spaces in repeaters
    var currentInstance = this;
    var height = Repeater.MAXREPEATERHEIGHT;
    if (currentInstance.gridType == 'child' && !currentInstance.displayChildGridAsPopup) {
        height = Repeater.MINCHILDREPEATERHEIGHT;
    }
    if (currentInstance.gridType !== 'child' || currentInstance.displayChildGridAsPopup) {
        var tableHeight = parseInt($(currentInstance.gridElementIdJQ).css('height'));
        if (tableHeight < height) {
            if (tableHeight == 0 && currentInstance.data.length == 0) {
                tableHeight = 0;
            } else if (tableHeight == 0 && currentInstance.data.length > 0) {
                tableHeight = currentInstance.data.length * 22;
                tableHeight = tableHeight < Repeater.MINREPEATERHEIGHT ? Repeater.MINREPEATERHEIGHT : tableHeight;
            }
            else {
                tableHeight = tableHeight < Repeater.MINREPEATERHEIGHT ? Repeater.MINREPEATERHEIGHT : tableHeight;
            }
        }
        $(currentInstance.gridElementIdJQ).jqGrid('setGridHeight', Math.min(height, tableHeight + 18));
    }
}

repeaterBuilderMedical.prototype.isRowDataChanges = function (rowId, rowData) {
    var currentInstance = this;
    var duplicationCheckEnabledElements = currentInstance.design.Elements.filter(function (ct) {
        if (ct.CheckDuplicate == true) {
            return ct;
        }
    });

    var formDataOfRow = currentInstance.data.filter(function (ct) {
        return ct.RowIDProperty == rowId;
    });

    var hasFormGridColumnArray = getRepeaterHashColumnData(formDataOfRow, duplicationCheckEnabledElements);

    var beforeGridColumnArray = getRepeaterHashColumnData(rowData, duplicationCheckEnabledElements);

    if (hasFormGridColumnArray[0].Data == beforeGridColumnArray[0].Data) {
        return false;
    }
    else {
        return true;
    }
}

repeaterBuilderMedical.prototype.showDuplicateControlsOnRepeaterElementChange = function (validationError) {
    var currentInstance = this;
    var errorRow = currentInstance.formInstanceBuilder.validation.getErrorGridRowObject(validationError);
    var sections = currentInstance.formInstanceBuilder.errorGridData.filter(function (ct) {
        return ct.SectionID == errorRow.SectionID && ct.Section == errorRow.Section && ct.Form == errorRow.Form;
    });

    if (sections.length > 0) {
        var row = sections[0].ErrorRows.filter(function (ct) {
            return ct.SubSectionName == errorRow.SubSectionName && ct.GeneratedName == errorRow.GeneratedName && ct.RowIdProperty == errorRow.RowIdProperty;
        });

        if (row.length != 0) {
            currentInstance.applyValidation(row[0]);
        }
        else {
            currentInstance.removeValidationErrorOfControl(errorRow, currentInstance);
        }
    }
    else {
        currentInstance.removeValidationErrorOfControl(errorRow, currentInstance);
    }
}

repeaterBuilderMedical.prototype.setCursorOnEditRow = function (rowId, columnIndex) {
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

repeaterBuilderMedical.prototype.getRowDataFromInstance = function (rowId) {
    var currentInstance = this;

    var rowData = currentInstance.data.filter(function (dt) {
        return dt.RowIDProperty == rowId;
    });

    return rowData;
}

repeaterBuilderMedical.prototype.registerExpandCollapseEvents = function () {
    $(".ui-icon-extlink").off("click");
    var currentInstance = this;
    $(currentInstance.gridElementIdJQ).on("click", ".ui-icon-extlink", function (e) {
        $(this).removeClass("ui-icon-extlink").addClass("ui-icon-minus");
        var rowID = $(this).closest('tr').attr('id');
        var selRowID = $(currentInstance.gridElementIdJQ).jqGrid('getGridParam', 'selrow');
        var elementsToFind = "input,select,textarea";
        if (selRowID != rowID) {
            if (selRowID !== undefined && selRowID != null) {
                var rowHtml = $(currentInstance.gridElementIdJQ).jqGrid('getGridRowById', selRowID);
                if (rowHtml != undefined && rowHtml != null && $(rowHtml).find(elementsToFind).length > 0) {
                    $(currentInstance.gridElementIdJQ).jqGrid('saveRow', selRowID);
                    currentInstance.saveRow();
                }
            }
            $(currentInstance.gridElementIdJQ).jqGrid("setSelection", rowID);
        }
        var rowHtml = $(currentInstance.gridElementIdJQ).jqGrid('getGridRowById', rowID);
        if (rowHtml != undefined && rowHtml != null && $(rowHtml).find(elementsToFind).length > 0) {
            $(currentInstance.gridElementIdJQ).jqGrid('saveRow', rowID);
            currentInstance.saveRow();
        }
        currentInstance.displaySubGridData(rowID, this);
        e.stopPropagation();
    });
}

repeaterBuilderMedical.prototype.displaySubGridData = function (rowID, expandedRow) {
    var currentInstance = this;
    ajaxDialog.showPleaseWait();
    setTimeout(function () {
        var subGridID = currentInstance.gridElementId + "_" + rowID;

        var gridArr = $(currentInstance.gridElementIdJQ).getDataIDs();
        var detailsDiv = currentInstance.displaySelectedRowDatails(rowID);
        $("#subGridDialogData").html(detailsDiv + "<div id='" + subGridID + "'></div>");
        if (!$("#subGridDialog").is(":visible")) {
            $("#subGridDialog").dialog({
                autoOpen: true,
                height: "auto",
                width: 1010,
                modal: true,
                resizable: false,
                title: currentInstance.design.Label + ' Details',
                position: ["middle", 100],
                beforeClose: function () {
                    $("#subGridDialog").dialog("destroy");
                    $("#subGridDialogData").html("");
                    $("#subGridDialogPrevBtn").off("click");
                    $("#subGridDialogNextBtn").off("click");
                    $("#subGridPreviousBtnContainer").css("margin-top", "0");
                    $("#subGridNextBtnContainer").css("margin-top", "0");
                    $("#subGridDialogPrevBtn").attr("disabled", false);
                    $("#subGridDialogNextBtn").attr("disabled", false);
                },
                close: function () {
                    $(expandedRow).removeClass("ui-icon-minus").addClass("ui-icon-extlink");
                }
            });
            $("#subGridDialogPrevBtn").click(function () {
                var curr_index = gridArr.indexOf(rowID);
                if ((curr_index - 1) >= 0) {
                    rowID = gridArr[curr_index - 1];
                    expandedRow = currentInstance.toggleSubGridData(curr_index - 1, gridArr, expandedRow);
                }
            });

            $("#subGridDialogNextBtn").click(function () {
                var curr_index = gridArr.indexOf(rowID);
                if ((curr_index + 1) < gridArr.length) {
                    rowID = gridArr[curr_index + 1];
                    expandedRow = currentInstance.toggleSubGridData(curr_index + 1, gridArr, expandedRow);
                }
            });
        }
        currentInstance.loadChildGrids(subGridID, rowID);
        var marginTop = ($("#subGridDialogData").height() - 80) / 2;
        $("#subGridPreviousBtnContainer").css("margin-top", marginTop);
        $("#subGridNextBtnContainer").css("margin-top", marginTop);

        $("#subGridDialogPrevBtn").attr("disabled", true);
        $("#subGridDialogNextBtn").attr("disabled", true);
        if (gridArr.indexOf(rowID) > 0) {
            $("#subGridDialogPrevBtn").attr("disabled", false);
        }
        if (gridArr.indexOf(rowID) < gridArr.length - 1) {
            $("#subGridDialogNextBtn").attr("disabled", false);
        }
        ajaxDialog.hidePleaseWait();
    }, 100);
}

repeaterBuilderMedical.prototype.toggleSubGridData = function (newIndex, gridIDs, currentRow) {
    var currentInstance = this;
    var expandedRow = currentRow;

    $(expandedRow).removeClass("ui-icon-minus").addClass("ui-icon-extlink");
    $(currentInstance.gridElementIdJQ).resetSelection().setSelection(gridIDs[newIndex], true);
    $(currentInstance.gridElementIdJQ).jqGrid('saveRow', gridIDs[newIndex]);
    var rowToExpand = $(currentInstance.gridElementIdJQ).find("tr[id='" + gridIDs[newIndex] + "']").find(".ui-icon-extlink");
    $(rowToExpand).removeClass("ui-icon-extlink").addClass("ui-icon-minus");
    currentInstance.displaySubGridData(gridIDs[newIndex], rowToExpand);
    return rowToExpand;
}

repeaterBuilderMedical.prototype.displaySelectedRowDatails = function (rowID) {
    var currentInstance = this;
    var row = $(currentInstance.gridElementIdJQ).getRowData(rowID);
    var divRow = "<div class='row'>";
    var i = 0;
    for (var key in row) {
        var displayText = undefined;
        $.each(currentInstance.design.Elements, function (id, elem) {
            if (elem.GeneratedName == key && elem.Visible == true) {
                displayText = elem.Label;
                divRow = divRow + "<div class='col-xs-2'>"
                        + "<span class='staticLabel'><b>" + displayText + "</b></span></div>";
                var valueToDisplay = row[key];
                if (elem.Type === "checkbox") {
                    valueToDisplay = (row[key] === "Yes") ? row[key] : "No";
                }
                valueToDisplay = (valueToDisplay == null || valueToDisplay == undefined) ? "" : valueToDisplay;
                divRow += "<div class='col-xs-2' style='-ms-word-break: break-all;'><label style='font-weight:normal;'>" + valueToDisplay + "</label></diV>";
                return false;
            }
        });
        if ((i + 1) % 3 === 0 && (i + 1) != row.length)
            divRow = divRow + " </div>  <div class='row'> ";
        i++;
    }
    return divRow + "</div>";
}

//set Cursor to the end of text in jqgrid on IE and Firefox browser
repeaterBuilderMedical.prototype.setCursorToEndOfText = function (child) {
    var currentInstance = this;
    if (currentInstance.isRowEditable == false && child[0].type != 'checkbox') {
        if (currentInstance.browserInfo.browser == "MSIE" || currentInstance.browserInfo.browser == "Firefox") {
            if (child[0].createTextRange) {
                var FieldRange = child[0].createTextRange();
                FieldRange.moveStart('character', child[0].value.length);
                FieldRange.select();
            }
            else if (child[0].value != undefined && child[0].value != null) {
                var elemLen = child[0].value.length;
                child[0].selectionStart = elemLen;
                child[0].selectionEnd = elemLen;
                child[0].focus();
            }

        } else {
            child[0].focus();
        }

    }


}


repeaterBuilderMedical.prototype.addBulkUpdateHeader = function (colModels) {
    var es = "";
    var currentInstance = this;
    var $grid = $(currentInstance.gridElementIdJQ);
    var benefitReviewGridServiceCommentFullPath = currentInstance.customruleMedical.BenefitReviewGridServiceCommentFullPath;
    var benefitReviewLimitsPath = currentInstance.customruleMedical.fullName.benefitReviewLimits;

    $.each(colModels, function (ind, value) {
        var id = '#gs_' + value.name;
        var buElement = currentInstance.getHeaderElement(this);
        var elementFullPath = currentInstance.fullName + "." + value.name;
        if (value.name.indexOf('_') > 0) {
            var INLElementString = value.name.substring(0, value.name.indexOf('_')).toString();
            if (INLElementString == "INL") {
                var elementName = value.name.substring(value.name.lastIndexOf('_') + 1, value.name.length).toString();
                elementFullPath = currentInstance.fullName + "." + elementName;
            }
        }
        if (buElement != null && buElement != undefined && benefitReviewGridServiceCommentFullPath != elementFullPath && benefitReviewLimitsPath != elementFullPath) {
            $grid.closest("div.ui-jqgrid-view").find(id).closest('div').append(buElement);
            if ($(buElement).find('select').attr('id') != undefined) {
                for (var i = 0; i < $(buElement).find('select').find('option').length; i++) {
                    if ($(buElement).find('select').find('option')[i].text == "Enter Unique Response") {
                        currentInstance.formatBulkUpdateControlsDropdownTextbox(buElement, id);
                    }
                }
            }
        }
    });
}

repeaterBuilderMedical.prototype.getHeaderElement = function (objElement) {
    var currentInstance = this;
    var element;
    var elementTemplate = "<table class='bulkupdates' cellspacing='0'>" +
                            "<tbody>" +
                                "<tr>" +
                                    "<td class='ui-search-oper' style='display: none;' colindex='0'></td>" +
                                    "<td class='ui-search-input'>{{0}}</td>" +
                                    "<td class='ui-search-clear'><a title='Clear Search Value' class='clearsearchclass' style='padding-right: 0.3em; padding-left: 0.3em;'>x</a></td>" +
                                "</tr>" +
                            "</tbody>" +
                        "</table>";

    switch (objElement.edittype) {
        case "text":
            element = "<input type='text' id='bu_" + objElement.name + "' name='bu_" + objElement.name + "' class='editable' maxlength='" + objElement.editoptions.maxlength + "'/>";
            break;
        case "textarea":
            element = "<input type='textarea' id='bu_" + objElement.name + "' name='bu_" + objElement.name + "' class='editable' maxlength='" + objElement.editoptions.maxlength + "'/>";
            break;
        case "select":
            element = "<select name='bu_" + objElement.name + "' class='editable' id='bu_" + objElement.name + "'>";
            $.each(objElement.editoptions.value.split(';'), function (key, val) {
                var option = val.split(':');
                if (option[1] == "Enter Unique Response") {
                    element += "<option value='" + option[0] + "'>" + option[1] + "</option>";
                }
                else if (option[0] != undefined && option[1] != undefined) {
                    element += "<option value='" + option[0] + "' class='standard-optn'>" + option[1] + "</option>";
                }
            });
            element += "</select>";
            if (objElement.editoptions.stdOptions != undefined && objElement.editoptions.stdOptions.length >= 0) {
                element += "<input id='bu_" + objElement.name + "Textbox' type='text' name='bu_" + objElement.name + "' class='ddt-textbox' style='display:none;'></input>";
            }
            break;
        case "checkbox":
            element = "<select name='bu_" + objElement.name + "' class='editable' id='bu_" + objElement.name + "'>";
            element += "<option value=''></option>";
            element += "<option value='Yes'>Yes</option>";
            element += "<option value='No'>No</option>";
            break;
    }

    if (element != undefined) {
        element = elementTemplate.replace("{{0}}", element);
    }

    return element;
}

repeaterBuilderMedical.prototype.addBulkUpdateControls = function () {
    var currentInstance = this;
    var $grid = $(currentInstance.gridElementIdJQ);

    $('#t_' + this.gridElementId).append("<div class='bucontrol' />");
    $('#t_' + this.gridElementId).find('.bucontrol').append("<input type='checkbox' id='chkEnable' value='true'/> Enable Bulk Update  ");
    $grid.closest("div.ui-jqgrid-view").find(".ui-search-table").children().find(".ui-search-input").children().addClass('removeCross');
    $('#t_' + this.gridElementId).on('click', '#chkEnable', function () {
        if (this.checked) {
            $('#t_' + currentInstance.gridElementId).find('#btnUpdate').show();
            currentInstance.addBulkUpdateHeader(currentInstance.columnModels);
            //disable filter toolbar           
            $grid.closest("div.ui-jqgrid-view").find(".ui-search-table").find(".ui-search-input").attr("disabled", "disabled");
            //$grid.closest("div.ui-jqgrid-view").find(".ui-search-input").children().attr("disabled", "disabled");          
            //$grid.closest("div.ui-jqgrid-view").find(".ui-search-clear").children().attr("disabled", "disabled");        
        }
        else {
            $('#t_' + currentInstance.gridElementId).find('#btnUpdate').hide();
            $grid.closest("div.ui-jqgrid-view").find("table.bulkupdates").remove();
            $grid.closest("div.ui-jqgrid-view").find(".ui-search-table").find(".ui-search-input").removeAttr("disabled", "disabled");
            //$grid.closest("div.ui-jqgrid-view").find(".ui-search-input").children().removeAttr("disabled");
            //$grid.closest("div.ui-jqgrid-view").find(".ui-search-clear").children().removeAttr("disabled");
            //$('#t_' + currentInstance.gridElementId).find('#chkEnable').attr('disabled', 'disabled');
        }
    });

    $('#t_' + this.gridElementId).find('.bucontrol').append("<button type='button' id='btnUpdate' class='btn btn-xs' style='display:none;'>Update</button>");
    $('#t_' + this.gridElementId).on('click', '#btnUpdate', function () {
        //Get Bulk Update elements values and store in an array
        var buValueArray = [];
        var buValue = $grid.closest("div.ui-jqgrid-view").find('[id*=bu_]');
        $.each(buValue, function () {
            var elementValue = currentInstance.getBulkUpdateControlsElementValue(this);
            if (elementValue != null && elementValue != undefined) {
                buValueArray[this.id.replace('bu_', '')] = elementValue;
            }
        });

        //Get the filtered rows
        //Update filtered row with the updated value
        var dataBeforeChanges = null;
        var filteredData;
        var gridData = $grid.jqGrid('getGridParam', 'data');
        //var objFilterCriteria = JSON.parse($grid.getGridParam("postData").filters);
        //$.each(objFilterCriteria.rules, function (ind, val) {
        //    filteredData = $.grep(gridData, function (a) {
        //        return (a[val.field].toLowerCase().indexOf(val.data.toLowerCase()) >= 0);
        //    });
        //});
        var objFilterCriteria = undefined;
        if ($grid.getGridParam("postData").filters == undefined) {
            objFilterCriteria = gridData;
        }
        else {
            objFilterCriteria = JSON.parse($grid.getGridParam("postData").filters);

            $.each(objFilterCriteria.rules, function (ind, val) {
                filteredData = $.grep(gridData, function (a) {
                    return (a[val.field].toLowerCase().indexOf(val.data.toLowerCase()) >= 0);
                });
            });
        }

        if (objFilterCriteria.rules == undefined || objFilterCriteria.rules.length == 0) {
            filteredData = gridData;
        }
        $.each(filteredData, function (key, value) {
            $.each(value, function (ind, val) {
                if (buValueArray[ind] != null && buValueArray[ind] != undefined) {
                    value[ind] = buValueArray[ind];
                }
            });
            $grid.jqGrid('setRowData', value.RowIDProperty, value);
            currentInstance.lastSelectedRow = value.RowIDProperty;
            currentInstance.saveRow(undefined, value);
        });

        //Remove the additional row added for bulk updates
        $grid.closest("div.ui-jqgrid-view").find("table.bulkupdates").remove();
        //Hide update button
        $('#t_' + currentInstance.gridElementId).find('#btnUpdate').hide();
        //uncheck the enable bulk update checkbox and disable
        $('#t_' + currentInstance.gridElementId).find('#chkEnable').prop('checked', false);
        //$('#t_' + currentInstance.gridElementId).find('#chkEnable').attr('disabled', 'disabled');

        //Clear FilterToolbar search and enable

        //EQN-888HSB Bulk Load - Filter Disabled Incorrectly
        $grid.closest("div.ui-jqgrid-view").find(".ui-search-input").children().prop('disabled', false);
        $grid.closest("div.ui-jqgrid-view").find(".ui-search-table").children().find(".ui-search-clear").children().prop('disabled', false);
        $('.ui-search-input').prop('disabled', false);
        $('.ui-search-table').prop('disabled', false);

        //$grid[0].clearToolbar();
    });
}
repeaterBuilderMedical.prototype.getBulkUpdateControlsElementValue = function (element) {
    if (element.value != '' && element.value != '[Select One]') {
        return element.value;
    }
}

repeaterBuilderMedical.prototype.formatBulkUpdateControlsDropdownTextbox = function (elementDesign, colid) {
    var currentInstance = this;
    var $grid = $(currentInstance.gridElementIdJQ);
    var idpluscol = $(elementDesign).find('select').attr('id');
    var ddttextboxID = $(elementDesign).find('input').attr('id');

    //$grid.closest("div.ui-jqgrid-view").find(colid).closest('div').find("#" + idpluscol).unbind('change');
    $grid.closest("div.ui-jqgrid-view").find(colid).closest('div').find("#" + idpluscol).change(function () {
        if ($(this).val() == 'newItem') {
            $(this).val('');
            $(this).parent().find('.ddt-textbox').toggle().focus();
            $(this).toggle();
        }
    });

    //$grid.closest("div.ui-jqgrid-view").find(colid).closest('div').find("#" + ddttextboxID).unbind('focusout');
    $grid.closest("div.ui-jqgrid-view").find(colid).closest('div').find("#" + ddttextboxID).on("focusout", function (e) {
        $(this).toggle();
        $(this).parent().find($("#" + idpluscol)).toggle();

    });

    //$grid.closest("div.ui-jqgrid-view").find(colid).closest('div').find("#" + ddttextboxID).unbind('keyup');
    $grid.closest("div.ui-jqgrid-view").find(colid).closest('div').find("#" + ddttextboxID).bind("keyup", function (e) {
        if (e.which !== 16) {
            var newValue = $(this).val();
            var dropdownTextboxControl = $(this).parent().find('#' + idpluscol);
            var stringUtility = new globalutilities();
            if (!stringUtility.stringMethods.isNullOrEmpty(newValue)) {
                //check if unique response is the same item as Items from document design
                var existingItem = false;
                $.each($(this).parent().find('#' + idpluscol + ' option'), function (key, elem) {
                    if (elem.value.toUpperCase() === newValue.toUpperCase()) {
                        existingItem = true;
                        dropdownTextboxControl.val(elem.value);
                    }
                });
                if (existingItem == false) {
                    $('<option value="' + newValue + '" class="standard-optn">' + newValue + '</option>').insertBefore($(this).parent().find('#' + idpluscol + ' option').last());
                    dropdownTextboxControl.val(newValue);
                    dropdownTextboxControl.trigger("change");
                }
            }
        }
    });
}

//Manage Sequence of benefitReviewGrid Deductibles as per Coverage Level List
repeaterBuilderMedical.prototype.benefitReviewGridDeductiblesSequenceManager = function (coveragelevelList, ColModelElementArray, currentInstance) {
    if (ColModelElementArray != undefined && coveragelevelList != undefined) {
        var firstPart = ColModelElementArray.slice(0, ColModelElementArray.length / 2);
        var secondPart = ColModelElementArray.slice(firstPart.length);
        for (var z = 0; z < 2; z++) {
            var sourceArray = (z == 0 ? firstPart : secondPart);
            $.each(coveragelevelList, function (index, item) {
                var colModelObj = Enumerable.From(sourceArray).Where(function (dt) {
                    return dt.CoverageType == item.CoverageType;
                }).FirstOrDefault();
                if (colModelObj != undefined && colModelObj != null) {
                    currentInstance.columnNames.push(colModelObj.Label);
                    currentInstance.columnModels.push(colModelObj.ColModel);
                }
                var index = sourceArray.indexOf(colModelObj);
                sourceArray.splice(index, 1);
            });
            $.each(sourceArray, function (ind, model) {
                if (model != undefined && model != null) {
                    currentInstance.columnNames.push(model.Label);
                    currentInstance.columnModels.push(model.ColModel);
                }
            });
        }
    }
}

repeaterBuilderMedical.prototype.SetVisionCopayDefaultValue = function (MasterListVisionNetworkTier)
{
    if (MasterListVisionNetworkTier.length>0) {
            for (var item = 0; item < MasterListVisionNetworkTier.length; item++) {
                if (MasterListVisionNetworkTier[item]) {
                    if (MasterListVisionNetworkTier[item].Copay) {
                    }
                    else {
                        MasterListVisionNetworkTier[item].Copay= "Not Applicable";
                    }
                }
            }
        }
}


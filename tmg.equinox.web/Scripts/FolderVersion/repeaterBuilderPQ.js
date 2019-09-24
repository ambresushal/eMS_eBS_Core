var repeaterBuilderPQ = function (design, data, fullName, formInstanceId, gridType, parentRowId, formInstanceBuilder, rules, ruleProcessor) {
    this.design = design;
    if (Array.isArray(data)) {
        this.data = data;
    }
    else {
        this.data = [];
    }
    this.fullName = fullName;
    this.formInstanceId = formInstanceId;
    this.rules = rules;
    this.gridType = gridType;
    this.parentRowId = parentRowId;
    this.formInstanceBuilder = formInstanceBuilder;
    this.formUtilities = new formUtilities(formInstanceId);
    //this.customrule = new customruleHandler(FormTypes.CURRENTFORMDESIGNID, formInstanceBuilder.IsMasterList);
    this.expressionBuilder = new expressionBuilder();
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

    this.isViewRowDataClick = false;
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
    this.isGridDivClick = false;
    this.filterdMessagesRowId = null;
    this.SERLMessages = [];
    this.PDBCPrefixes = [];
    this.PDBCPrefixValue = null;
    this.DisallowedMessage = [];
    this.DisallowedMessageValue = null;
    this.SERLMessagesValue = null;
    this.DeductibleAccumulatorValue = null;
    this.bulkUpdateCopiedRowData = null;
    this.bulkUpdateSelectedRowData = null;
    this.isRowEditable = false;
    this.filterCondition = [];
    this.filterToolbar = false;
    this.hasRichTextBox = false;
    this.ProofingRepeaterName = 'ProofingAttachments.Proofing';
    this.orignalData = JSON.parse(JSON.stringify($.extend(true, [], this.prepareGridData())));
}

repeaterBuilderPQ.prototype.build = function () {
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
            currentInstance.columnNames.push(currentInstance.addColModel(element));
            currentInstance.columnModels.push(currentInstance.addColModel(element));
        }
    });


    //$.each(this.design.Elements, function (index, element) {
    //    if (element.Label != null && element.IsPrimary == true) {
    //        var isRequired = undefined;
    //        currentInstance.formInstanceBuilder.designData.Validations.filter(function (ct) {
    //            if (element.FullName == ct.FullName && ct.IsRequired == true) {
    //                isRequired = true;
    //            }
    //        });
    //        if (isRequired == true) {
    //            currentInstance.columnNames.push(element.Label + "<font color=red>*</font>");
    //        }

    //        else {
    //            currentInstance.columnNames.push(element.Label);
    //        }
    //        currentInstance.columnModels.push(currentInstance.addColModel(element));
    //    }
    //});

    //for manual record check for first time records didnot apply Inline or child records
    var isRepeaterData = true;
    if (this.design.PrimaryDataSource != null) {
        if (this.design.PrimaryDataSource.DataSourceModeType == "Manual") {
            // isRepeaterData = currentInstance.isPrimaryDataExits();
        }
    }

    if (this.design.ChildDataSources != null) {
        $.each(currentInstance.design.ChildDataSources, function (idx, dataSource) {
            //register inline columns      
            if (dataSource.DisplayMode == 'In Line' && dataSource.Mappings != undefined && dataSource.Mappings.length > 0 && isRepeaterData) {

                var currentformDataSource = currentInstance.formInstanceBuilder.designData.DataSources.filter(function (dt) {
                    return dt.TargetParent == dataSource.TargetParent && dt.DisplayMode == 'In Line';
                })

                dataSource.GroupHeader = currentformDataSource[0].GroupHeader;

                var inLineRows = dataSource.GroupHeader.length;
                for (var idx = 0; idx < inLineRows; idx++) {

                    var rowData = dataSource.GroupHeader[idx];
                    var startIndex = currentInstance.columnModels.length;
                    var titleText = '';
                    var col = [];
                    var ColModelElementArray = new Array();
                    var coveragelevelList;

                    $.each(dataSource.Mappings, function (index, mapping) {
                        var flag = true;
                        var element = currentInstance.getElementDesign(mapping.TargetElement);
                        if (element != undefined) {
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

                        col.push(currentInstance.addColModel(element, 'INL_' + dataSource.DataSourceName, idx))
                        currentInstance.columnModels.push(currentInstance.addColModel(element, 'INL_' + dataSource.DataSourceName, idx));

                    });

                    currentInstance.columnNames.push({ title: titleText, dataIndx: titleText, colModel: col });
                    currentInstance.groupHeaders.push({
                        startColumnName: 'INL_' + dataSource.DataSourceName + '_' + idx + '_' + dataSource.Mappings[0].TargetElement,
                        numberOfColumns: dataSource.Mappings.length,
                        startIndex: startIndex,
                        titleText: titleText
                    });
                }

            }

            if (dataSource.DisplayMode == 'Child' && dataSource.Mappings != undefined && dataSource.Mappings.length > 0 && isRepeaterData) {
                currentInstance.hasChildGrids = true;
            }
        });
    }

    //add key column
    currentInstance.columnNames.push(currentInstance.addKeyColModel());
    currentInstance.columnModels.push(currentInstance.addKeyColModel());
    //render grid

    //Add extra column to show expand/collapse icon
    if (currentInstance.hasChildGrids && currentInstance.displayChildGridAsPopup) {
        currentInstance.columnNames.splice(0, 0, currentInstance.addExpandCollapseColModel(currentInstance.fullName));
        currentInstance.columnModels.splice(0, 0, currentInstance.addExpandCollapseColModel(currentInstance.fullName));
        currentInstance.registerExpandCollapseEvents();
    }
    //End

    if (currentInstance.formInstanceBuilder.formDesignId == FormTypes.MASTERLISTFORMID && currentInstance.design.LoadFromServer == true) {
        currentInstance.data = new Array();
        currentInstance.checkDuplicationRowsWorker = new Worker(currentInstance.URLs.checkDuplicateRowsWorker);
    }

    this.render();
    //if (this.gridType == 'child') {
    currentInstance.gridEvents.processRules();
    //}

    if (currentInstance.design.IsEnabled == false) {
        $(currentInstance.gridElementIdJQ + " .ui-icon-plus").css("pointer-events", "none");
        $(currentInstance.gridElementIdJQ + " .ui-icon-minus").css("pointer-events", "none");
        $(currentInstance.gridElementIdJQ + " .ui-icon-copy").css("pointer-events", "none");

        $(currentInstance.gridElementIdJQ).pqGrid({
            cellClick: function (event, ui) {
                event.preventDefault();
                return false;
            }
        });
    }
}

repeaterBuilderPQ.prototype.getElementDesign = function (elementName) {
    var elementDesign;
    $.each(this.design.Elements, function (index, element) {
        if (element.GeneratedName == elementName) {
            elementDesign = element;
        }
    });
    return elementDesign;
}

repeaterBuilderPQ.prototype.addColModel = function (element, dataSourceName, idx) {
    var elementLabel = '';
    var currentInstance = this;
    if (element.Label != null && element.IsPrimary == true) {
        var isRequired = undefined;
        currentInstance.formInstanceBuilder.designData.Validations.filter(function (ct) {
            if (element.FullName == ct.FullName && ct.IsRequired == true) {
                isRequired = true;
            }
        });

        var elementLabel = element.IsKey == true ? "<span class='primaryKey'/>" + element.Label : element.Label;
        if (isRequired == true) {
            elementLabel += "<font color=red>*</font>";
        }
    }

    if (elementLabel == '')
        elementLabel = element.Label;

    var genName = element.GeneratedName;
    if (dataSourceName != undefined) {
        genName = dataSourceName + '_' + idx + '_' + genName;
    }

    var colMod = {
        title: elementLabel,
        tooltip: element.Label,
        dataIndx: genName,
        hidden: !element.Visible,
        align: this.getColumnAlignment(element),
        width: this.getColumnWidth(element),
        colModel: [],
        filter: { type: 'textbox', condition: 'contain', listeners: ['change'] },
        editor: this.getEditOptions(element, genName),
        iskey: element.IsKey,
        //type:  element.Type == 'checkbox' ? '' : this.getEditType(element),
        resizable: true,
        //name: genName,
        //index: genName,
        //hidden: !element.Visible,
        //sortable: true,
        //sorttype: this.getSortType(element),
        editable: this.getEditable(element),
        edittype: this.getEditType(element),
        editModel: this.getEditModel(element)
        //editoptions: this.getEditOptions(element, genName),
        //align: this.getColumnAlignment(element),
        //width: this.getColumnWidth(element),
        //fixed: true,
    };
    if (element.Type == 'select' || element.Type == 'SelectInput') {
        colMod.render = function (ui) {
            var options = ui.column.editor.options,
                cellData = ui.cellData;
            if (Array.isArray(cellData)) {
                var result = [];
                $.each(cellData, function () {
                    for (var i = 0; i < options.length; i++) {
                        var option = options[i];

                        if (option[this]) {
                            result.push(option[this]);
                        }
                    }
                });
                return result.join();
            }
            for (var i = 0; i < options.length; i++) {
                var option = options[i];

                if (option[cellData]) {
                    return option[cellData];
                }
            }
        }
    }
    else if (element.Type == 'checkbox') {
        colMod.render = function (ui) {
            if (ui.cellData == "Yes" || ui.cellData == "true" || ui.cellData == "True" || ui.cellData == true) {
                ui.cellData = "Yes";
                ui.rowData[ui.dataIndx] = "Yes";
            }
            else {
                ui.cellData = "No";
                ui.rowData[ui.dataIndx] = "No";
            }
            return currentInstance.chkValueImageFmatter(ui.cellData);
        }
    }

    if (element.FullName == currentInstance.ProofingRepeaterName + '.DocumentName') {
        colMod.render = function (ui) {
            return currentInstance.getFileDownloadFormatter(ui.cellData, ui.rowData);
        }
        //colMod.formatter = this.chkValueImageFmatter;
        colMod.unformat = this.fileDownloadUnFormat;
    }


    if (currentInstance.expressionBuilder.hasExpressionBuilderRule(currentInstance.formInstanceBuilder.formDesignId) && (element.FullName == "BenefitReview.BenefitReviewGrid.SlidingCostShare"
        || element.FullName == "BenefitsReview.BenefitReviewGrid.LimitsPlanOptions")) {
        colMod.render = function (ui) {
            var formatterInfo = currentInstance.expressionBuilder.getFormatterForGrid(colMod, ui, element.FullName);
            return formatterInfo;
        };
    }

    return colMod;
}

repeaterBuilderPQ.prototype.addKeyColModel = function () {
    var colMod = {
        title: this.KeyProperty,
        dataIndx: this.KeyProperty,
        key: true,
        hidden: true,
        //width: "20px",
        colModel: [],
    };
    return colMod;

}

repeaterBuilderPQ.prototype.addExpandCollapseColModel = function (repeaterName) {
    var colModel = {
        title: "",
        dataIndx: "expandCollapse",
        key: false,
        editable: false,
        search: false,
        width: "20px",
        render: function (ui) {
            //if (repeaterName == "StandardServices.StandardServiceList") {
            //    if (ui.rowData.Name != undefined && ui.rowData.Name.length > 0)
            //        return '<a style="cursor:pointer;"><span align="center" class="ui-icon ui-icon-extlink" id="' + ui.rowData.RowIDProperty + '" style="margin:auto" ></span></a>';
            //    else
            //        return '';
            //}
            //else
            return '<a style="cursor:pointer;"><span align="center" class="ui-icon ui-icon-extlink" id="' + ui.rowData.RowIDProperty + '" style="margin:auto;font-family:Material Icons;font-size: 17px;" ></span></a>';
        }
    };
    return colModel;
}

repeaterBuilderPQ.prototype.expandCollapseFmatter = function (cellvalue, options, rowObject) {
    return '<a style="cursor:pointer;"><span align="center" class="ui-icon ui-icon-extlink" style="margin:auto;font-family:Material Icons;font-size: 17px;" ></span></a>';
}

repeaterBuilderPQ.prototype.chkValueImageFmatter = function (cellvalue, options, rowObject) {

    if (cellvalue == "Yes" || cellvalue == "true" || cellvalue == "True" || cellvalue == true) {
        return '<span align="center" class="ui-icon ui-icon-check" style="margin:auto" ></span>';
    }
    else {
        return '<span align="center" class="ui-icon ui-icon-close" style="margin:auto" ></span>';
    }
}

repeaterBuilderPQ.prototype.chkValueImageUnFormat = function (cellvalue, options, cell) {

    var checked = $(cell).children('span').attr('class');

    if (checked == "ui-icon ui-icon-check")
        return 'Yes';
    else
        return 'No';
}

repeaterBuilderPQ.prototype.highlightDropdownTextboxFmatter = function (cellvalue, options, rowObject) {
    var stdOptions = options;

    if ((cellvalue === undefined || cellvalue === null || cellvalue === "")) {
        return "";
    }
    else {
        var isCellvalueExist = false;
        if (!(stdOptions === "undefined" || stdOptions === "")) {
            for (i = 0; i < stdOptions.length; i++) {
                if (stdOptions[i] === cellvalue) {
                    isCellvalueExist = true;
                    break;
                }
            }
        }

        if (isCellvalueExist == false) {
            return '<span class="non-standard-optn">' + cellvalue + '</span>';
        }
        else {
            return cellvalue;
        }
    }

}

repeaterBuilderPQ.prototype.highlightDropdownTextboxUnformat = function (cellvalue, options, cell) {

    return cellvalue;
}

repeaterBuilderPQ.prototype.sortOptions = function (items) {
    return sortDropDownElementItems(items);
}

repeaterBuilderPQ.prototype.getOptions = function (element, isSortRequired) {
    var items = element.Items;
    var currentInstance = this;
    //sorts options in ascending order
    //if (isSortRequired == true) {
    //    items = this.sortOptions(items);
    //}

    var options = "";
    var dd = [];
    if (items != undefined && items != null && items.length > 0) {
        options = options + Validation.selectOne + ':' /*+ Validation.selectOne */ + ';';

        var defaultValue = {}; defaultValue[Validation.selectOne] = "";
        dd.push(defaultValue);
        for (var idx = 0; idx < items.length; idx++) {
            if (items[idx].ItemValue != '') {
                var d = {};
                d[items[idx].ItemValue] = items[idx].ItemText == null ? items[idx].ItemValue : items[idx].ItemText;
                dd.push(d);
                options = options + items[idx].ItemValue + ':' + (items[idx].ItemText == null ? items[idx].ItemValue : items[idx].ItemText);
                if (idx < items.length - 1) {
                    options = options + ';';
                }
            }
        }
    }

    return dd;
}

repeaterBuilderPQ.prototype.getColumnWidth = function (element) {
    if (element.Type === 'text') {
        return element.IsRichTextBox ? "300px;" : '250px';
    }
    else {
        return '150px';
    }
}

repeaterBuilderPQ.prototype.getColumnAlignment = function (element) {
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

repeaterBuilderPQ.prototype.getSortType = function (element) {
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

repeaterBuilderPQ.prototype.getEditType = function (element) {
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

repeaterBuilderPQ.prototype.getEditModel = function (element) {
    var editModel = {};
    switch (element.Type) {
        case "text":
            if (element.Multiline == true) {
                editModel["keyUpDown"] = false;
                editModel["saveKey"] = '';
            }
            break;
    }
    return editModel;
}

repeaterBuilderPQ.prototype.getEditOptions = function (element, genName) {
    var editOptions = {};
    switch (element.Type) {
        case "text":
            if (element.MaxLength > 0) {
                editOptions["MaxLength"] = element.MaxLength;
            }
            if (element.Multiline == true) {
                editOptions["rows"] = 50;
                editOptions["cols"] = 10;
                editOptions["type"] = "textarea";
            }
            editOptions["defaultValue"] = element.DefaultValue;
            if (element.IsRichTextBox) {
                this.hasRichTextBox = true;
            }
            break;
        case "select":
        case "SelectInput":
            if (element.IsDropDownTextBox == true) {
                //editOptions["value"] = this.getDropdownTextboxOptionValues(element.Items, element.IsSortRequired);
                editOptions["type"] = 'select';
                editOptions["options"] = this.getDropdownTextboxOptions(element.Items, element.IsSortRequired);
                // editOptions["init"] = this.formatDropdownTextbox;
                editOptions["cls"] = "ddt-dropdown";
                //editOptions["labelIndx"] = "text";
                //editOptions["mapIndices"] = { "text": genName, "value": genName }


            } else {
                editOptions["options"] = this.getOptions(element, element.IsSortRequired);
                //editOptions["text"] = this.getDropdownTextboxOptionValues(element.Items, element.IsSortRequired);
                editOptions["type"] = 'select';
                if (element.MultiSelect == true) {
                    editOptions["attr"] = "multiple='multiple'",
                    editOptions["init"] = this.multiSelectEditor;
                    editOptions["getData"] = function (ui) {
                        return ui.$cell.find("select").val();
                    }
                }
                //editOptions["valueIndx"] = "value";
                //editOptions["labelIndx"] = "text";
                //editOptions["mapIndices"] = { "text": genName, "value": genName };

            }
            editOptions["defaultValue"] = element.DefaultValue;
            break;
        case "checkbox":
            editOptions["type"] = "checkbox";
            editOptions["init"] = this.formatCheckBox;

            editOptions["cb"] = {
                all: false,
                header: false,
                check: "true",
                uncheck: "false"
            }
            editOptions["value"] = "true:false";
            if (element.DefaultValue == "true") {
                editOptions["defaultValue"] = "true";
            }
            else {
                editOptions["defaultValue"] = "false";
            }
            break;
        case "calendar":
            editOptions["defaultValue"] = element.DefaultValue;
            break;
    }
    return editOptions;
}

repeaterBuilderPQ.prototype.multiSelectEditor = function (ui) {
    var $inp = ui.$cell.find("select");
    //    $($inp).attr('multiple', 'multiple');
    //    var noAttr = $($inp).attr('multiple', 'multiple');
    //    if(typeof noAttr !== 'undefined') { // strict equality comparison doesn't work
    //    $($inp).parent().addClass('multiple-select');
    //}

    var val = ui.cellData == null ? "" : ui.cellData;
    $inp.val(val).pqSelect({
        checkbox: true,
        //multiplePlaceholder: 'Select ' + ui.dataIndx,
        maxDisplay: 1,
        selectallText: 'Check All',
        width: '100%',
        maxSelect: 100,// maxDisplay: 5,
        displayText: "",
        deselect: true,
        search: true,
    });


    var position = $inp.pqSelect("option", "position");


    $inp.pqSelect("option", "position", { my: "right top", at: "right bottom" });

    $inp.pqSelect("open");
}

repeaterBuilderPQ.prototype.formatCheckBox = function (ui) {

    var currentInstance = this;
    var count = 0;
    var $select = ui.$cell.find("input"),
               grid = this
    if (ui.cellData == "true" || ui.cellData == "Yes" || ui.cellData == true) {
        $($select).prop('checked', "checked");
    }
    else {
        $($select).removeAttr('checked')
    }



}


repeaterBuilderPQ.prototype.getKeys = function () {
    var currentInstance = this;
    var keyNames = [];
    if (currentInstance.design.Label == 'Benefit Review Grid' && currentInstance.design.GeneratedName == "BenefitsReview.BenefitReviewGrid") {
        keyNames.push("BenefitCategory1");
        keyNames.push("BenefitCategory2");
        keyNames.push("BenefitCategory3");
    } else {
        $.each(currentInstance.design.Elements, function (int, element) {
            if (element.IsKey == true) {
                keyNames.push(element.GeneratedName);
            }
        });
    }

    return keyNames;
}

repeaterBuilderPQ.prototype.getEditable = function (element) {
    var editable = true;
    //|| element.IsKey
    if ((element.Type == 'label' && element.IsLabel == true) || element.Enabled == false) {
        editable = false;
    }

    return editable;
}

repeaterBuilderPQ.prototype.render = function () {
    var currentInstance = this;
    var obj = {
        sortable: currentInstance.fullName == "BenefitsReview.BenefitReviewGrid" || currentInstance.fullName == "Limits.StandardLimits" || currentInstance.fullName == "Limits.StandardPlanOptions" ? true : false,
        sortModel: currentInstance.fullName == "BenefitsReview.BenefitReviewGrid" ? {
            cancel: false,
            type: "local",
            multiKey: null,
            single: false,
            sorter: [{ dataIndx: "BenefitCategory1", dir: "up" }, { dataIndx: "BenefitCategory2", dir: "up" }, { dataIndx: "BenefitCategory3", dir: "up" }, { dataIndx: "NetworkTier", dir: "up" }]
        } : currentInstance.fullName == "Limits.StandardLimits" || currentInstance.fullName == "Limits.StandardPlanOptions" ?
        {
            cancel: false,
            type: "local",
            multiKey: null,
            single: false,
            sorter: [{ dataIndx: "BenefitCategory1", dir: "up" }, { dataIndx: "BenefitCategory2", dir: "up" }, { dataIndx: "BenefitCategory3", dir: "up" }]
        } :
        {},
        width: "100%",
        height: 300,
        title: currentInstance.design.Label,
        showBottom: true,
        hoverMode: 'row',
        hwrap: false,
        wrap: true,
        editable: !currentInstance.hasRichTextBox,
        selectionModel: { type: 'row' },
        editModel: { clicksToEdit: 1 },
        selectionModel: { mode: 'single' },
        virtualX: true,
        resizable: false,
        filterModel: currentInstance.getFilterModel(),
        filter: function (evt, ui) {
            currentInstance.filterhandler(evt, ui, currentInstance);
        },
        //bootstrap: { on: false },
        numberCell: { show: true }, // Number Column
        showToolbar: currentInstance.design.AllowBulkUpdate,
        showHeader: true, // Column Headers
        collapsible: { on: false, collapsed: false, toggle: true, css: { zIndex: 1305 } },
        columnBorders: true,
        rowBorders: true,
        flexHeight: false,
        scrollModel: { autoFit: false },
        //scrollModel: { pace: 'fast', autoFit: false, theme: true },// AutoFit Columns
        dragColumns: { enabled: true, acceptIcon: 'ui-icon-check', rejectIcon: 'ui-icon-closethick', topIcon: 'ui-icon-circle-arrow-s', bottomIcon: 'ui-icon-circle-arrow-n' },
        showTop: currentInstance.design.DisplayTopHeader,
        showTitle: currentInstance.design.DisplayTitle,
        freezeCols: currentInstance.design.FrozenColCount,
        freezeRows: currentInstance.design.FrozenRowCount,
        sortIndx: currentInstance.getKeys()
    };

    if (currentInstance.design.AllowPaging) {
        obj.pageModel = { type: "local", rPP: currentInstance.design.RowsPerPage, strRpp: "{0}", strDisplay: "{0} to {1} of {2}" };
    }

    if (currentInstance.design.FilterMode.toLowerCase() == "local filtering" || currentInstance.design.FilterMode.toLowerCase() == "both") {
        obj.toolbar = {
            cls: "pq-toolbar-search",
            items: [
                { type: "<span style='margin:5px;'>Filter</span>" },
                { type: 'textbox', style: "width:20%; margin:0px 5px;", attr: 'placeholder="Enter your keyword"', cls: "filterValue", listeners: [{ 'change': function (evt, ui) { currentInstance.enhancedFilterHandler(evt, ui, currentInstance) } }] },
                {
                    type: 'select', style: "width:20%; margin:0px 5px;", cls: "filterColumn",
                    listeners: [{ 'change': function (evt, ui) { currentInstance.enhancedFilterHandler(evt, ui, currentInstance) } }],
                    options: function (ui) {
                        var CM = ui.colModel;
                        var opts = [{ '': '[ All Fields ]' }];
                        var visibleCM = CM.filter(function (obj) {
                            return obj.hidden == false;
                        });
                        for (var i = 0; i < visibleCM.length; i++) {
                            var column = visibleCM[i];
                            var obj = {};
                            obj[column.dataIndx] = column.title;
                            opts.push(obj);
                        }
                        return opts;
                    }
                },
                {
                    type: 'select', style: "width:20%; margin:0px 5px;", cls: "filterCondition",
                    listeners: [{ 'change': function (evt, ui) { currentInstance.enhancedFilterHandler(evt, ui, currentInstance) } }],
                    options: [
                    { "begin": "Begins With" },
                    { "contain": "Contains" },
                    { "end": "Ends With" },
                    { "notcontain": "Does not contain" },
                    { "equal": "Equal To" },
                    { "notequal": "Not Equal To" },
                    { "empty": "Empty" },
                    { "notempty": "Not Empty" },
                    { "less": "Less Than" },
                    { "great": "Greater Than" }
                    ]
                }
            ]
        };
    }
    else {
        obj.toolbar = {}
    }
    obj.colModel = currentInstance.columnNames;

    obj.dataModel = {
        data: currentInstance.prepareGridData()
    };
    //      
    obj.columnResize = function (evt, ui) {
        //;
    };

    obj.rowSelect = function (event, ui) {
    };

    obj.create = function (event, ui) {

    }

    obj.render = function (event, ui) {
        var table = document.createElement("TABLE");
        table.className = "customPager";
        var row = document.createElement("TR");
        var td = document.createElement("TD");
        var cell = document.createElement("SPAN");


        if (currentInstance.design.PrimaryDataSource == null && currentInstance.gridType != 'child') {
            currentInstance.addFooterButtonToGrid(table, row, td, cell);
        }
        else {
            //show + button on selected repeater builder
            var primaryDataSources = currentInstance.design.PrimaryDataSource;
            if (primaryDataSources != null && !currentInstance.expressionBuilder.hideAddButtonforRepeater(currentInstance.fullName)) {
                if ((primaryDataSources.DataSourceModeType == 'Manual' || primaryDataSources.DataSourceModeType == 'Custom') && (primaryDataSources.DisplayMode == 'Primary' || primaryDataSources.DisplayMode == 'Child')) {
                    cell.className = "ui-icon ui-icon-plus";
                    cell.onclick = function () {
                        currentInstance.addManualData();
                    }
                    td.appendChild(cell);
                    row.appendChild(td);
                }
            }

        }
        // Add footer buttons for ANOCChart BenefitsCompare
        if (currentInstance.design.PrimaryDataSource != null && currentInstance.fullName == "ANOCChartPlanDetails.ANOCBenefitsCompare") {
            if (currentInstance.hasRichTextBox) {
                currentInstance.addRichTextBox(table, row, td, cell);
            }
        }
        $.extend({
            distinct: function (anArray) {
                var result = [];
                $.each(anArray, function (i, v) {
                    if ($.inArray(v, result) == -1) result.push(v);
                });
                return result;
            }
        });

        if (currentInstance.fullName != currentInstance.ProofingRepeaterName) {
            td = document.createElement("TD");
            cell = document.createElement("SPAN");
            cell.className = "ui-icon ui-icon-document";
            cell.title = 'View';
            cell.onclick = function () {
                var row = $(currentInstance.gridElementIdJQ).pqGrid("selection", { type: 'row', method: 'getSelection' });
                if (row && row.length > 0 && row[0].rowData != undefined) {
                    var rowId = row[0].rowData.RowIDProperty;
                    currentInstance.lastSelectedRow = rowId;
                    if (rowId !== undefined && rowId !== null) {
                        if (!(currentInstance.design.LayoutClass == "customLayout")) {
                            ajaxDialog.showPleaseWait();
                            setTimeout(function () {
                                var isRowReadOnly = currentInstance.design.PrimaryDataSource == null ? false : true;
                                currentInstance.viewRowDataPQ(rowId, isRowReadOnly, false);
                                ajaxDialog.hidePleaseWait();
                            }, 100);
                        }
                        else {
                            ajaxDialog.showPleaseWait();
                            setTimeout(function () {
                                currentInstance.viewRepeaterTemplatePQ(rowId, false);
                                ajaxDialog.hidePleaseWait();
                            }, 100);

                        }
                    }
                    else {
                        messageDialog.show(Common.pleaseSelectRowMsg);
                    }
                }
                else {
                    messageDialog.show(Common.pleaseSelectRowMsg);
                }
            }

            td.appendChild(cell);
            row.appendChild(td);
        }

        if (currentInstance.design.AllowExportToExcel) {
            td = document.createElement("TD");
            cell = document.createElement("SPAN");
            cell.className = "ui-icon ui-icon-arrowstop-1-s";
            cell.title = 'Export To Excel';
            cell.onclick = function () {
                //$(currentInstance.gridTableJQ).pqGrid("exportExcel", { url: "/FolderVersionReport/excel", sheetName: "pqGrid", global: true });
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

            td.appendChild(cell);
            row.appendChild(td);
        }
        if (currentInstance.fullName != currentInstance.ProofingRepeaterName) {
            td = document.createElement("TD");
            cell = document.createElement("SPAN");
            cell.className = "ui-icon ui-icon-refresh";
            cell.title = 'Refresh';
            cell.onclick = function () {
                $('#' + currentInstance.gridElementId).pqGrid("reset", { group: true, filter: true });
                $('#' + currentInstance.gridElementId).pqGrid("refreshDataAndView");
            }
            td.appendChild(cell);
            row.appendChild(td);
        }
        if (currentInstance.design.AllowExportToCSV) {
            // Export To CSV
            td = document.createElement("TD");
            cell = document.createElement("SPAN");
            cell.className = "ui-icon ui-icon-arrowstop-1-n";
            cell.title = 'Export To CSV';
            cell.onclick = function () {
                alert("Functionality Not Implemented");
            }
            td.appendChild(cell);
            row.appendChild(td);
        }

        var MLViewName = $(".ml-document-menu").val();
        if (MLViewName == "FIPS Code" || MLViewName == 'Premiums' || MLViewName == 'Formulary Information' || MLViewName == 'Benchmark Information' || MLViewName == 'Prescription') {

            td = document.createElement("TD");
            cell = document.createElement("SPAN");
            cell.className = "ui-icon ui-icon-arrowstop-1-s";
            cell.title = 'Import Excel';
            cell.onclick = function () {
                //Import ML code here
                var importDialog = new showImportMLExcelDialog(currentInstance.formInstanceId, currentInstance, currentInstance.formInstanceBuilder);
                importDialog.show();
            }

            td.appendChild(cell);
            row.appendChild(td);
        }


        table.appendChild(row);

        $(currentInstance.gridElementIdJQ).find('.pq-grid-bottom').prepend(table);
    }


    var currentInstance = this;
    var url = "";
    if (currentInstance.formInstanceBuilder.formDesignId == FormTypes.MASTERLISTFORMID && currentInstance.design.LoadFromServer == true) {
        url = currentInstance.URLs.getformInstanceRepeaterData.replace(/\{formInstanceId\}/g, currentInstance.formInstanceId).replace(/\{fullName\}/g, currentInstance.fullName);
    }
    $(currentInstance.gridElementIdJQ).parent().append("<div id='p" + currentInstance.gridElementId + "'></div>");
    var pagerElement = '#p' + currentInstance.gridElementId;

    $(currentInstance.gridElementIdJQ).pqGrid(obj);

    if (this.design.AllowBulkUpdate === true) {  //EQN-BulkUpdate
        currentInstance.addBulkUpdateControls();
    }

    //$(this.gridElementIdJQ).focusout(function (e) {
    //    if (currentInstance.lastSelectedRow != undefined && currentInstance.rowDataBeforeEdit != undefined) {
    //       // currentInstance.saveRow();
    //        //use for validate Repeater control validations
    //        currentInstance.repeaterControlFocusOut(e, currentInstance.lastSelectedRow);
    //        if (currentInstance.design.LoadFromServer == true) {
    //            var rowData = $(currentInstance.gridElementIdJQ).pqGrid("getRowData", { rowIndx: currentInstance.lastSelectedRow });
    //            currentInstance.loadFromServerObject.checkModifyedRowDataAndDuplication(currentInstance.lastSelectedRow, rowData, "Update");
    //        }
    //    }
    //    if (currentInstance.formInstanceBuilder.form.hasValidationErrors()) {
    //        currentInstance.formInstanceBuilder.form.showValidationErrorsOnForm();
    //    }
    //    else {
    //        currentInstance.formInstanceBuilder.form.hideValidationErrorsOnForm();
    //    }
    //});

    $(currentInstance.gridElementIdJQ).pqGrid({
        rowClick: function (event, ui) {
            if (ui) {
                var id = ui.rowData.RowIDProperty;
                for (var idx = 0; idx < currentInstance.columnModels.length; idx++) {

                    var idpluscol = id + "_" + currentInstance.columnModels[idx].dataIndx;

                    if (currentInstance.columnModels[idx] != null) {
                        var element = currentInstance.design.Elements.filter(function (data) {
                            return (currentInstance.columnModels[idx].dataIndx.split('_')[currentInstance.columnModels[idx].dataIndx.split('_').length - 1] == data.GeneratedName);
                        });

                        if (element[0] != null && element[0].Type == 'calendar') {
                            currentInstance.pickDates(idpluscol, element[0]);
                        }
                        //for dropdown textbox element
                        if (element[0] != null && element[0].Type == 'SelectInput') {
                            currentInstance.formatDropdownTextbox(idpluscol, element[0], id);
                        }
                        //code to disable controls on chrome browser
                        //if (element[0] != null) {
                        //    currentInstance.setChildElementsIsDisabled(currentInstance.gridElementIdJQ, id, element[0].GeneratedName, element[0].Type);
                        //}
                    }
                }
                var obj = $(currentInstance.gridElementIdJQ).pqGrid("getEditCell");

                var rowId = id;
                $(obj.$cell).find('input,select,textarea').each(function (idx, elem) {
                    $(elem).on('change', function () {
                        var elem = this;
                        var designElem = currentInstance.design.Elements.filter(function (el) {
                            var elname = elem.name.split('_');
                            return el.GeneratedName == elname[(elname.length) - 1];
                        });

                        var isEnterUniqueResponse = false;
                        if ($(this).val() == 'newItem') {
                            isEnterUniqueResponse = true;
                            $(this).val('');
                            $(this).parent().append('<input id="' + idpluscol + '_' + elem.name + 'Textbox" type="text" name="' + elem.name + '"class="pq-editor-focus pq-cell-editor"></input>');
                            var inp = $(this).parent().find('[id=' + idpluscol + '_' + elem.name + 'Textbox]');
                            $(this).parent().find('select').remove();
                            inp.on("focusout", function (e) {
                                $(this).hide();
                                $(currentInstance.gridElementIdJQ).pqGrid("saveEditCell");
                                $(currentInstance.gridElementIdJQ).pqGrid("quitEditMode");
                            });

                        }

                        if (designElem != null && designElem.length > 0) {
                            designElem = designElem[0];
                            currentInstance.lastColumnSelected = designElem;
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
                                            var elemId = currentInstance.columnModels[rowId].dataIndx;
                                            var idParts = elemId.split('_');
                                            childId = idParts[idParts.length - 2];
                                            var childName = idParts[idParts.length - 3];
                                            row[childName][childId][designElem.GeneratedName] = currentInstance.getElementValue(designElem.Type, elem);
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



                                        var ID = $(currentInstance.gridElementIdJQ).pqGrid("option", "dataModel.data");
                                        $.each(ID, function (idx, ct) {
                                            if (ct.RowIDProperty == rowId) {
                                                rowIndex = idx;
                                                return false;
                                            }
                                        });

                                        selectedRowId = rowId;
                                        currentPage = $(currentInstance.gridElementIdJQ).pqGrid("option", "pageModel.curPage");
                                        var rowNum = $(currentInstance.gridElementIdJQ).pqGrid("option", "pageModel.rPP");
                                        if (currentPage != 1) {
                                            rowIndex = ((currentPage - 1) * rowNum) + rowIndex;
                                        }
                                        var keyValue = "";
                                        var repeaterId = currentInstance.design.Name + currentInstance.formInstanceId;
                                        $.each(currentInstance.design.Elements, function (index, element) {
                                            if (element.IsKey == true) {
                                                var rowData = $("#" + repeaterId).pqGrid('getRowData', selectedRowId);
                                                var value;
                                                if (currentInstance.rowDataBeforeEdit != undefined)
                                                    value = currentInstance.rowDataBeforeEdit[element.GeneratedName];
                                                else
                                                    value = rowData[element.GeneratedName];

                                                keyValue += value + "#";
                                            }
                                        });

                                        if (keyValue != "") {
                                            rowIndex = rowIndex + "|" + keyValue.substring(0, keyValue.length - 1);
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

                            if (currentInstance.expressionBuilder.hasExpressionBuilderRule(currentInstance.formInstanceBuilder.formDesignId)) {
                                var value = $(elem).find(":selected").text();
                                currentInstance.expressionBuilder.repeaterElementOnChange(currentInstance, designElem, rowId, value, isEnterUniqueResponse);
                            }
                        }
                    });
                });

                if (!$(currentInstance.gridElementIdJQ).attr("disabled")) {
                    currentInstance.rowDataBeforeEdit = JSON.parse(JSON.stringify(ui.rowData));
                    $.each(currentInstance.formInstanceBuilder.sections, function (idx, ct) {
                        if (ct.IsLoaded == true && ct.FullName == currentInstance.design.FullName.split('.')[0]) {
                            if (currentInstance.browserInfo.browser == "Firefox") {
                                currentInstance.rowClick = true;
                            }
                            return false;
                        }
                    });

                }

                currentInstance.lastSelectedRow = ui.rowData.RowIDProperty;
            }
        },
        complete: function (event, ui) {
            if (currentInstance.fullName == currentInstance.ProofingRepeaterName) {
                $(currentInstance.gridElementIdJQ).unbind().on('click', ".downloadProofingattachment", function () {
                    FolderLockAction.ISREPEATERACTION = 1;
                    var fileName = $(this).attr("data-Id");
                    var contextName = $(this).context.textContent;
                    if (contextName == "PBP Template") {
                        window.open(fileName);
                    }
                    else {

                        var downloadFileURL = '/FolderVersion/DownloadProofingDocument?fileName=';
                        var stringData = "fileName=" + fileName;
                        $.download(downloadFileURL, stringData, 'post');
                    }
                });
            }
            //if (currentInstance.expressionBuilder.hasExpressionBuilderRule(currentInstance.formInstanceBuilder.formDesignId)) {
            //    currentInstance.expressionBuilder.registerEventForButtonInRepeater(currentInstance);
            //}
            $.each(currentInstance.columnModels, function (ind, col) {
                var colM = $(currentInstance.gridElementIdJQ).pqGrid('getColumn', { dataIndx: col.dataIndx });
                colM.title = col.tooltip;
                var $cell = $(currentInstance.gridElementIdJQ).pqGrid('getCellHeader', { colIndx: ind });
                $cell.find('div.pq-td-div').attr("title", col.tooltip);
            });
            try {
                // Add fullscreen title PQ Grid
                $(currentInstance.gridElementIdJQ).find('.ui-icon-arrow-4-diag').attr('title', 'Full Screen');
            } catch (e) { }
        },
        cellBeforeSave: function (evt, ui) {
            currentInstance.lastSelectedRow = ui.rowData.RowIDProperty;
            if (ui.newVal != undefined && ui.newVal != null) {
                if (ui.newVal == "[Select One]") {
                    ui.newVal = "";
                }
                ui.rowData[ui.dataIndx] = ui.newVal;
            }
            if (ui.dataIndx == "AdditionalCoverageLevel") {
                if (ui.newVal == "" && (ui.oldVal != "Covered at Non-Routine benefit level." || ui.oldVal != "Covered at surgical level."
                    || ui.oldVal != "Covered at the benefit level of the services billed." || ui.oldVal != "Covered at the Out of Network benefit level of the services billed"
                    || ui.oldVal != "Covered at the HCR services level. See preventive care." || ui.oldVal != "Not covered under Base"
                    || ui.oldVal != "N/A- Covered in full under Base" || ui.oldVal != "N/A")) {
                    var designBRGAddCoverLevelElement = currentInstance.design.Elements.filter(function (el) {
                        return el.GeneratedName == "AdditionalCoverageLevel";
                    });
                    if (designBRGAddCoverLevelElement != null && designBRGAddCoverLevelElement.length > 0) {
                        designBRGAddCoverLevelElement = designBRGAddCoverLevelElement[0];
                    }
                    currentInstance.expressionBuilder.repeaterElementOnChange(currentInstance, designBRGAddCoverLevelElement, ui.rowData.RowIDProperty, ui.newVal, false);
                }
            }
            if (ui.oldVal != ui.newVal) {
                if (ui.column.edittype == "checkbox") {
                    if (ui.newVal == true || ui.newVal == "true" || ui.newVal == "True") {
                        ui.newVal = "Yes";
                    }
                    else {
                        ui.newVal = "No";
                    }
                    //currentInstance.expressionBuilder.repeaterCheckBoxOnChange(currentInstance, ui.dataIndx, currentInstance.lastSelectedRow, ui.newVal);
                }
                $(currentInstance.gridElementIdJQ).pqGrid("refreshCell", { rowIndx: currentInstance.lastSelectedRow, dataIndx: ui.dataIndx });
                currentInstance.saveRow();
                currentInstance.addEntryToAcitivityLogger(currentInstance.lastSelectedRow, Operation.UPDATE, ui.column.title, ui.oldVal, ui.newVal, undefined, undefined, ui.dataIndx);
            }
        },
        cellSave: function (evt, ui) {
            if (ui) {
                var rowId = ui.rowData.RowIDProperty;
                if (currentInstance.expressionBuilder.hasExpressionBuilderRule(currentInstance.formInstanceBuilder.formDesignId)) {
                    currentInstance.expressionBuilder.repeaterElementCellSave(currentInstance, currentInstance.lastColumnSelected, rowId);
                }
                try {
                    var design = currentInstance.design.Elements.filter(function (el) {
                        var elname = ui.dataIndx.split('_');
                        return el.GeneratedName == elname[(elname.length) - 1];
                    });
                    if (design != undefined && design != null && design.length > 0) {
                        var designElem = design[0];
                        // If datatype=float make value to two decimal fixed 
                        if (designElem.DataType == "float" && ui.newVal != "" && ui.newVal != undefined && ui.newVal != null && !isNaN(ui.newVal)) {
                            ui.rowData[designElem.GeneratedName] = parseFloat(ui.newVal).toFixed(2);
                            $(currentInstance.gridElementIdJQ).pqGrid("refreshCell", { rowIndx: currentInstance.lastSelectedRow, dataIndx: ui.dataIndx });
                            currentInstance.saveRow();
                        }
                    }
                }
                catch (e) {
                }
            }
        },
        cellClick: function (evt, ui) {
            currentInstance.lastSelectedRow = ui.rowData.RowIDProperty;

            //colIndex -1 means row has been selected.
            if (ui.colIndx != undefined) {
                if (currentInstance.expressionBuilder.hasExpressionBuilderRule(currentInstance.formInstanceBuilder.formDesignId)) {
                    //  currentInstance.expressionBuilder.registerEventForButtonInRepeater(currentInstance);
                }
            }

            if (currentInstance.expressionBuilder.hasExpressionBuilderRule(currentInstance.formInstanceBuilder.formDesignId)) {
                return currentInstance.expressionBuilder.onCellClick(currentInstance, ui);
            }
        },
        editorBegin: function (event, ui) {
            if (ui) {
                var element = currentInstance.design.Elements.filter(function (data) {
                    return data.GeneratedName == ui.dataIndx;
                });

                if (element[0] != null && element[0].Type == 'SelectInput') {
                    var elementDesign = element[0];
                    var count = 0;
                    var currentVal = ui.rowData[ui.dataIndx];
                    $.each(elementDesign.Items, function (key, item) {
                        if (currentVal) {
                            if (item.ItemValue.toUpperCase() === currentVal.toUpperCase())
                                count++;
                        }
                    });
                    if (currentVal && currentVal != "" && count == 0) {
                        var obj = $(currentInstance.gridElementIdJQ).pqGrid("getEditCell");
                        $(obj.$cell).find('select').append($('<option class="non-standard-optn" value="' + currentVal + '">' + currentVal + '</option>'));
                    }
                }
            }
        },
        refreshHeader: function () {
            $.each(currentInstance.columnModels, function (ind, col) {
                var colM = $(currentInstance.gridElementIdJQ).pqGrid('getColumn', { dataIndx: col.dataIndx });
                colM.title = col.tooltip;
                var $cell = $(currentInstance.gridElementIdJQ).pqGrid('getCellHeader', { colIndx: ind });
                $cell.find('div.pq-td-div').attr("title", col.tooltip);
            });
        }
    });


}

repeaterBuilderPQ.prototype.getColumnCounts = function () {
    var currentInstance = this;
    var colNames = $.grep(currentInstance.columnModels, function (v) {
        return v.dataIndx.indexOf("INL_") < 0;
    });

    return colNames.length;
}

repeaterBuilderPQ.prototype.getPageModel = function () {
    var currentInstance = this;
    if (currentInstance.design.AllowPaging) {
        return { type: "local", rPP: currentInstance.design.RowsPerPage, strRpp: "{0}", strDisplay: "{0} to {1} of {2}" }
    } else {
        return {}
    }
}

repeaterBuilderPQ.prototype.getFilterModel = function () {
    var currentInstance = this;
    if (currentInstance.design.FilterMode.toLowerCase() == "local header filtering") {
        return { on: true, mode: "AND", header: true }
    }
    else if (currentInstance.design.FilterMode.toLowerCase() == "local filtering") {
        return { on: true, mode: "OR", header: false }
    }
    else if (currentInstance.design.FilterMode.toLowerCase() == "both") {
        return { on: true, mode: "AND", header: true }
    }
}

repeaterBuilderPQ.prototype.enhancedFilterHandler = function (evt, ui, currentInstance) {
    currentInstance.filterToolbar = true;
    var $grid = $(currentInstance.gridElementIdJQ);
    var $toolbar = $grid.find('.pq-toolbar-search'),
        $value = $toolbar.find(".filterValue"),
        value = $value.val(),
        condition = $toolbar.find(".filterCondition").val(),
        dataIndx = $toolbar.find(".filterColumn").val(),
        filterObject;

    if (dataIndx == "") {//search through all fields when no field selected.
        filterObject = [];
        var CM = $grid.pqGrid("getColModel");
        for (var i = 0, len = CM.length; i < len; i++) {
            if (!CM[i].hidden) {
                var dataIndx = CM[i].dataIndx;
                filterObject.push({ dataIndx: dataIndx, condition: condition, value: value });
            }
        }
    }
    else {//search through selected field.
        filterObject = [{ dataIndx: dataIndx, condition: condition, value: value }];
    }
    $grid.pqGrid("filter", {
        oper: 'replace',
        data: filterObject
    });
    // Initialize search Row filter condition 
    currentInstance.filterCondition = [];
    currentInstance.filterToolbar = false;
}

repeaterBuilderPQ.prototype.filterhandler = function (evt, ui, currentInstance) {
    if (!currentInstance.filterToolbar) {
        if (currentInstance.design.AllowBulkUpdate === true) {
            if (ui.filter.length > 0) {
                $('#' + evt.target.id).find('#chkEnable').removeAttr('disabled');
                $('#' + evt.target.id).find('#chkCopyRow').removeAttr('disabled');
            }
            else {
                $('#' + evt.target.id).find('#chkEnable')[0].value = "false";
                $('#' + evt.target.id).find('#chkEnable').attr('disabled', 'disabled');
                $('#' + evt.target.id).find('#chkCopyRow')[0].value = "false";
                $('#' + evt.target.id).find('#chkCopyRow').attr('disabled', 'disabled');
            }
        }
        currentInstance.filterCondition = [];
        if (ui.filter.length > 0) {
            currentInstance.filterCondition.push(ui.filter);
        }
    }
}

repeaterBuilderPQ.prototype.prepareGridData = function () {

    var currentInstance = this;
    var gridData = [];
    var ids = [];
    try {
        var data = currentInstance.data;
        //for (var index = 0; index < data.length; index++) {
        //    if (data[index].hasOwnProperty(this.KeyProperty) && data[index].RowIDProperty >= 0) {
        //        ids.push(data[index].RowIDProperty);
        //    }
        //}
        //var maxId = 0;
        //if (ids.length > 0) {
        //    maxId = Math.max.apply(Math, ids) + 1;
        //}
        for (var index = 0; index < data.length; index++) {
            //if (!(data[index].hasOwnProperty(this.KeyProperty) && data[index].RowIDProperty >= 0)) {
            data[index].RowIDProperty = index;
            //    maxId++;
            //}
            var row = data[index];
            var newRow = $.extend({}, row);
            if (currentInstance.design.ChildDataSources != null && Array.isArray(this.design.ChildDataSources) && currentInstance.design.ChildDataSources.length > 0 && currentInstance.design.ChildDataSources[0].DisplayMode == "In Line") {
                $.each(currentInstance.columnModels, function (idx, col) {

                    if (col.dataIndx.toString().substring(0, 4) == 'INL_') {
                        var propArr = col.dataIndx.toString().split('_');
                        if (propArr.length == 4) {
                            var dsName = propArr[1];
                            var propIdx = propArr[2];
                            var propName = propArr[3];
                            newRow[col.dataIndx] = row[dsName][propIdx][propName];

                        }
                    }
                });
            }
            gridData.push(newRow);
        }
    }
    catch (err) {

    }

    return gridData;
}

repeaterBuilderPQ.prototype.getDataTypeProperty = function () {
    var currentInstance = this;
    var datatype;
    if (currentInstance.formInstanceBuilder.formDesignId == FormTypes.MASTERLISTFORMID && currentInstance.design.LoadFromServer == true) {
        datatype = "json";
    }
    else {
        datatype = "local";
    }
    return datatype;
}

//export to excel
repeaterBuilderPQ.prototype.getExportData = function (autoExpandChild) {
    FolderLockAction.ISREPEATERACTION = 1;
    var currentInstance = this;
    currentInstance.saveRow();
    var jqGridtoCsv = new JQGridtoCsv(currentInstance.gridElementIdJQ, autoExpandChild, currentInstance);
    jqGridtoCsv.buildExportOptions();

    var forminstancelisturl = '/FormInstance/ExportToExcel';

    var stringData = "csv=" + encodeURIComponent(jqGridtoCsv.csvData);
    stringData += "<&isGroupHeader=" + jqGridtoCsv.isGroupHeader;
    stringData += "<&noOfColInGroup=" + jqGridtoCsv.noofGroupedColumns;
    stringData += "<&isChildGrid=" + jqGridtoCsv.isSubgrid;
    stringData += "<&repeaterName=" + currentInstance.design.Label;
    stringData += "<&formName=" + currentInstance.formInstanceBuilder.formName;
    stringData += "<&folderVersionId=" + currentInstance.formInstanceBuilder.folderVersionId;
    stringData += "<&folderId=" + currentInstance.formInstanceBuilder.folderId;
    stringData += "<&tenantId=" + currentInstance.tenantId;

    $.downloadNew(forminstancelisturl, stringData, 'post');
}

repeaterBuilderPQ.prototype.viewRowDataPQ = function (rowId, isRowReadOnly, hasSaveButton) {
    var currentInstance = this;
    currentInstance.isViewRowDataClick = true;
    var ids = $(currentInstance.gridElementIdJQ).pqGrid("option", "dataModel.data");
    var pageNo = $(currentInstance.gridElementIdJQ).pqGrid("option", "pageModel.curPage");
    var rowNum = $(currentInstance.gridElementIdJQ).pqGrid("option", "pageModel.rPP");
    //var rowNum = $(currentInstance.gridElementIdJQ).getGridParam('rowNum');
    var allRecords = $(currentInstance.gridElementIdJQ).pqGrid("option", "totalRecords");;
    var totalPages = parseInt((allRecords / rowNum) + 1);
    var totalPages = $(currentInstance.gridElementIdJQ).pqGrid("option", "pageModel.totalPages");
    var repData = currentInstance.data;
    //var filters = $(currentInstance.gridElementIdJQ).getGridParam("postData").filters;
    var filters = $(currentInstance.gridElementIdJQ).pqGrid("option", "filterModel");
    if (currentInstance.design.LoadFromServer == false) {
        $(currentInstance.gridElementIdJQ).pqGrid("option", "pageModel.curPage", pageNo);
        //   $(currentInstance.gridElementIdJQ).setGridParam({ page: pageNo });
        $(currentInstance.gridElementIdJQ).pqGrid("refreshDataAndView");
    }
    if (isRowReadOnly) {
        for (var i = 0; i < ids.length; i++) {
            // var row = $(currentInstance.gridElementIdJQ).pqGrid("getRowsByClass", { cls: 'pq-grid-row' });
            $(currentInstance.gridElementIdJQ).find("tr .pq-grid-row").attr('disabled', 'disabled');
        }
    }
    $(currentInstance.gridElementIdJQ).pqGrid("setSelection", { rowIndx: rowId });

    var elements = [];

    $.each(currentInstance.design.Elements, function (i, elem) {
        elements.push({
            Id: elem.GeneratedName, displayText: elem.Label, Visible: elem.Visible, Type: elem.Type, UIElementName: elem.Name,
            Items: elem.Items,
            Details: elem
        });
    });
    //var ind = $(currentInstance.gridElementIdJQ).jqGrid("getInd", rowId, true);
    var ind = $(currentInstance.gridElementIdJQ).pqGrid("getRow", { rowIndx: rowId });
    //var child = $(" td:eq(" + 0 + ")", ind).children().first();
    //var child = $(" td:eq(" + 1 + ")", ind);
    //if (child.length > 0) {
    //    $(child).focus();
    //}

    //if (filters != undefined) {
    //    var filterData = JSON.parse(filters);
    //    if (filterData["rules"].length != 0)
    //currentInstance.data = repData;
    //}   
    //currentInstance.data = repData;
    currentInstance.saveRow();
    var dialog = new repeaterdialog(rowId, elements, currentInstance, isRowReadOnly, repData);
    dialog.showPQ(hasSaveButton);
}

repeaterBuilderPQ.prototype.viewRepeaterTemplatePQ = function (rowId, isRowReadOnly) {
    var currentInstance = this;
    currentInstance.isViewRowDataClick = true;
    var ids = $(currentInstance.gridElementIdJQ).pqGrid("option", "dataModel.data");
    var pageNo = $(currentInstance.gridElementIdJQ).pqGrid("option", "pageModel.curPage");
    var rowNum = $(currentInstance.gridElementIdJQ).pqGrid("option", "pageModel.rPP");
    var allRecords = $(currentInstance.gridElementIdJQ).pqGrid("option", "totalRecords");;
    var totalPages = parseInt((allRecords / rowNum) + 1);
    var totalPages = $(currentInstance.gridElementIdJQ).pqGrid("option", "pageModel.totalPages");
    var repData = currentInstance.data;
    var filters = $(currentInstance.gridElementIdJQ).pqGrid("option", "filterModel");
    $(currentInstance.gridElementIdJQ).pqGrid("setSelection", { rowIndx: rowId });

    var elements = [];

    $.each(currentInstance.design.Elements, function (i, elem) {
        elements.push({
            Id: elem.GeneratedName, displayText: elem.Label, Visible: elem.Visible, Type: elem.Type, UIElementName: elem.Name,
            Items: elem.Items,
            Details: elem
        });
    });
    currentInstance.saveRow();
    var dialog = new repeaterdialog(rowId, elements, currentInstance, isRowReadOnly, repData);
    dialog.showRepeaterTemplate();
}
//Add Data on Manual Mode
repeaterBuilderPQ.prototype.addManualData = function () {
    var currentInstance = this;
    var data = $(currentInstance.gridElementIdJQ).pqGrid("option", "dataModel.data");
    if (data.length != 0) {
        currentInstance.saveRow();
    }
    var hasDisabledParent = $(currentInstance.gridElementIdJQ).hasClass('ui-jqgrid-btable disabled');
    var elements = [];
    $.each(currentInstance.design.Elements, function (i, elem) {
        elements.push({
            Id: elem.GeneratedName, displayText: elem.Label, Visible: elem.Visible, Type: elem.Type, UIElementName: elem.Name,
            Items: elem.Items,
            Details: elem
        });
    });

    ajaxDialog.showPleaseWait();
    setTimeout(function () {
        var dialog = new manualDataSourceMappingDialogPQ(elements, currentInstance.design.Label, currentInstance, hasDisabledParent);
        dialog.mapDataSourceManually();
        //ajaxDialog.hidePleaseWait();
    }, 100);
}

repeaterBuilderPQ.prototype.updateCallBack = function (rowData, rowID) {
    var currentInstance = this;
    $(currentInstance.gridElementIdJQ).jqGrid("setRowData", rowID, rowData);
    currentInstance.saveRow();
    //if (currentInstance.design.LoadFromServer == false)
    //    $(currentInstance.gridElementIdJQ).trigger('reloadGrid');
    //$(currentInstance.gridElementIdJQ).jqGrid('setSelection', rowID, focus());

    setTimeout(currentInstance.gridEvents.processRules(), 25);
}

repeaterBuilderPQ.prototype.prepareGridDataOld = function () {
    var currentInstance = this;
    var gridData = [];
    var ids = [];
    var data = currentInstance.data;
    for (var index = 0; index < data.length; index++) {
        if (data[index].hasOwnProperty(this.KeyProperty) && data[index].RowIDProperty >= 0) {
            ids.push(data[index].RowIDProperty);
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

repeaterBuilderPQ.prototype.addNewRow = function () {
    try {
        var currentInstance = this;

        if (this.restrictRowAddition() == false) {
            messageDialog.show("There is no more Coinsurance Type exist.");
            return false;
        }

        var row = {};
        //if (this.rowDataBeforeEdit != undefined) {
        //    $(this.gridElementIdJQ).pqGrid("refresh");
        //    this.saveRow();
        //}
        for (var idx = 0; idx < this.columnModels.length; idx++) {
            if (this.columnModels[idx].editor != undefined) {
                var defaultValue = "";
                if (this.columnModels[idx].dataIndx.substring(0, 4) == 'INL_') {
                    var mappElement = this.columnModels[idx].dataIndx.split('_')[3];
                    this.addChildElement(row, idx, defaultValue, mappElement);
                }
                else {
                    if (this.columnModels[idx].editor.defaultValue != null && this.columnModels[idx].editor.defaultValue != undefined) {
                        defaultValue = this.columnModels[idx].editor.defaultValue;
                    }
                    row[this.columnModels[idx].dataIndx] = defaultValue;
                }
            }
            else {
                row[this.columnModels[idx].dataIndx] = "";
            }
        }

        var ids = [-1];
        for (var idx = 0; idx < this.data.length; idx++) {
            ids.push(this.data[idx].RowIDProperty);
        }
        var rowId = Math.max.apply(Math, ids) + 1;
        row[this.KeyProperty] = rowId;

        if (this.design.PrimaryDataSource == null) {
            currentInstance.data.push(row);
        }

        if (currentInstance.expressionBuilder.hasExpressionBuilderRule(currentInstance.formInstanceBuilder.formDesignId)) {
            currentInstance.expressionBuilder.repeaterBeforeRowAdded(currentInstance, row);
        }

        $(currentInstance.gridElementIdJQ).pqGrid('addRow', { rowData: row });

        this.updateTargetDataSourceCollection(rowId, "add");

        //$(this.gridElementIdJQ).jqGrid('setSelection', rowId);

        $(this.gridElementIdJQ).pqGrid('editFirstCellInRow', { rowIndx: currentInstance.data.length });
        //this.setCursorOnEditRow(rowId, 0);
        this.selectedRowId = rowId;
        // Set selection to added row 
        $(this.gridElementIdJQ).pqGrid("setSelection", { rowIndx: currentInstance.data.length, focus: true });

        setTimeout(this.gridEvents.processRules(), 25);
    }
    catch (err) {

    }
}

repeaterBuilderPQ.prototype.restrictRowAddition = function () {
    var currentInstance = this;
    var allow = true;
    switch (currentInstance.fullName) {
        case "CostShare.Coinsurance.CoinsuranceList":
            allow = currentInstance.data.length >= 2 ? false : true;
            break;
        case "CostShare.Coinsurance.PreventiveLevelofCoinsuranceList":
            allow = currentInstance.data.length >= 2 ? false : true;
            break;

    }

    return allow;
}

repeaterBuilderPQ.prototype.deleteRow = function (rowId) {
    var dataModel = $(this.gridElementIdJQ).pqGrid("option", "dataModel");
    var gridPQData = $(this.gridElementIdJQ).pqGrid("option", "dataModel.data");

    var rowIndx;

    var rowData = undefined;
    $.each(dataModel.data, function (index, item) {
        if (item.RowIDProperty == rowId) {
            rowIndx = index;
            rowData = item;
        }
    });

    var jsonRowIndx;
    $.each(this.data, function (index, item) {
        if (item.RowIDProperty == rowId) {
            jsonRowIndx = index
        }
    });

    var keyValue = "";
    var repeaterId = this.design.Name + this.formInstanceId;
    var selectedRowId = rowId;
    if (rowData) {
        $.each(this.design.Elements, function (index, element) {
            if (element.IsKey == true) {
                var value = rowData[element.GeneratedName];
                if (isHTML(value)) {
                    var elementID = $(value).attr('id')
                    if (elementID != undefined)
                        value = $('#' + elementID).val();
                }

                keyValue += value + "#";
            }
        });
        if (keyValue != "") {
            keyValue = keyValue.substring(0, keyValue.length - 1);
        }
        // To remove from JSON that will save the form (if not specified then it will not delete row from db & is again displayed in the grid if we load the form again )

        this.data.splice(jsonRowIndx, 1);

        $(this.gridElementIdJQ).pqGrid("deleteRow", { rowIndx: rowIndx, effect: true });
        this.lastSelectedRow = null;

        if (this.formInstanceBuilder.errorGridData != [] && this.formInstanceBuilder.errorGridData != null && this.formInstanceBuilder.errorGridData != undefined) {
            this.removeErrorGridRowOnRepeaterDeleteRow(rowId);
        }
        //add an entry to activity log
        this.addEntryToAcitivityLogger(rowId, Operation.DELETE, '', keyValue, '');
        this.formInstanceBuilder.hasChanges = true;

        //$(this.gridElementIdJQ).pqGrid("setSelection", { rowIndx: rowId });

        // below code for set focus to an first column of selected row
        //this.setCursorOnEditRow(rowId - 1, 0);
        this.selectedRowId = rowId - 1;
    }
}

repeaterBuilderPQ.prototype.copyRow = function (rowId) {
    var currentInstance = this;
    currentInstance.saveRow(currentInstance.lastSelectedRow);
    //copy over primary values
    var row = currentInstance.data.filter(function (row) {
        return row.RowIDProperty == currentInstance.lastSelectedRow;
    });
    var rowData = $.extend({}, true, row[0]);
    if (rowData) {
        var row = {};
        for (var idx = 0; idx < currentInstance.columnModels.length; idx++) {
            row[currentInstance.columnModels[idx].dataIndx] = "";
        }
        var ids = [-1];
        for (var idx = 0; idx < currentInstance.data.length; idx++) {
            ids.push(currentInstance.data[idx].RowIDProperty);
        }
        var newRowId = Math.max.apply(Math, ids) + 1;
        row[currentInstance.KeyProperty] = newRowId;

        rowData.RowIDProperty = newRowId;

        currentInstance.data.push(rowData);
        $(this.gridElementIdJQ).pqGrid('option', 'dataModel.data', currentInstance.data);
        $(this.gridElementIdJQ).pqGrid("refreshDataAndView");
        $(this.gridElementIdJQ).pqGrid("commit");
        //$(this.gridElementIdJQ).pqGrid('addRow', { rowData: rowData });


        //currentInstance.updateTargetDataSourceCollection(newRowId, "add");
        currentInstance.formInstanceBuilder.hasChanges = true;
        if (currentInstance.design.LoadFromServer == true) {
            currentInstance.loadFromServerObject.addRepeaterNewRowId(newRowId);
        }
        $(this.gridElementIdJQ).pqGrid("setSelection", { rowIndx: rowId });
        this.setCursorOnEditRow(newRowId, 0);
    }
}

repeaterBuilderPQ.prototype.pickDates = function (idpluscol, elementDesign) {
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

repeaterBuilderPQ.prototype.loadChildGrids = function (subGridId, rowId, isSetDisable) {
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
            var childBuilder = new repeaterBuilderPQ(childDesign, childData, childFullName, currentInstance.formInstanceId, 'child', rowId, currentInstance.formInstanceBuilder, rulesForRepeater, currentInstance.ruleProcessor);
            currentInstance.childGridBuilders.push(childBuilder);
            childBuilder.build();
        }
        if (isSetDisable) {
            currentInstance.formUtilities.sectionManipulation.disableRepeater('#' + subgridTableId);
        }
    });
}

repeaterBuilderPQ.prototype.bindData = function () {
    this.getData();
}

repeaterBuilderPQ.prototype.getData = function () {
    this.saveRow(true);
    $.each(this.childGridBuilders, function (idx, gridBuilder) {
        gridBuilder.getData();
    });
}

repeaterBuilderPQ.prototype.showValidatedControls = function (isOnCellChangeEvent) {
    var currentInstance = this;
    if (currentInstance.formInstanceBuilder.errorGridData != null) {
        currentInstance.removeValidationErrors();

        //On repeater onChange SelectRow event, there is no need to restore the Grid
        if (isOnCellChangeEvent == undefined || isOnCellChangeEvent == false) {
            //var ids = $(currentInstance.gridElementIdJQ).jqGrid('getDataIDs');
            var data = $(currentInstance.gridElementIdJQ).pqGrid("option", "dataModel.data");
            var ids = [];
            var len = data.length;
            for (var i = 0; i < len; i++) {
                ids.push(data[i].RowIDProperty);
            }
            for (i = 0; i < ids.length; i++) {
                //$(currentInstance.gridElementIdJQ).jqGrid('restoreRow', ids[i]);
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

repeaterBuilderPQ.prototype.applyValidation = function (row) {

    var currentInstance = this;
    var colindex = undefined;
    var loadedPage = undefined;

    var repeaterID = row.ElementID.substring(0, row.ElementID.indexOf("_")) + row.FormInstanceID;

    //var col = $("#" + repeaterID).jqGrid("getGridParam", "colModel");
    var col = $("#" + repeaterID).pqGrid("option", "colModel");
    if (col != undefined) {
        $.each(col, function (idx, ct) {
            if (ct.dataIndx.toString() == row.GeneratedName) {
                colindex = idx;
                return false;
            }
        });

        var totalPages = $("#" + repeaterID).pqGrid("option", "pageModel.totalPages");
        var currentPage = $("#" + repeaterID).pqGrid("option", "pageModel.curPage");
        var data = $("#" + repeaterID).pqGrid("pageData");
        var isRowIDExist = data.filter(function (ct) {
            if (ct.RowIDProperty == row.RowIdProperty) {
                return ct;
            }
        });

        if (isRowIDExist.length == 0) {
            for (i = 1; i <= totalPages; i++) {
                if (currentPage != i) {
                    $("#" + repeaterID).pqGrid("goToPage", { page: i });
                    var data = $("#" + repeaterID).pqGrid("pageData");
                    var isRowIDExist = data.filter(function (ct) {
                        return ct.RowIDProperty == row.RowIdProperty;
                    });
                    if (isRowIDExist.length != 0) {
                        break;
                    }
                }
            }
        }
        //var data = $("#" + repeaterID).pqGrid("getRowData", { rowIndx: row.RowIdProperty });
        var ind = $("#" + repeaterID).pqGrid("getRow", { rowIndx: row.RowIdProperty });
        if (ind) {
            //var index = $('#' + row.RowIdProperty)[0].rowIndex;
            var tcell = $("td:eq(" + (colindex + 1) + ")", ind);
            $(tcell).addClass("repeater-has-error");
        }
    }
}

repeaterBuilderPQ.prototype.removeValidationErrors = function () {
    var currentInstance = this;
    if (currentInstance.formInstanceBuilder.errorGridData != null) {
        for (var i = 0; i < currentInstance.formInstanceBuilder.errorGridData.length; i++) {
            var row = currentInstance.formInstanceBuilder.errorGridData[i];
            for (var j = 0; j < row.ErrorRows.length; j++) {
                if (row.ErrorRows[j].RowNum != "" && row.ErrorRows[j].RowNum != undefined) {
                    var rowNumber = row.ErrorRows[j].RowNum;
                    var td = getRepeaterColumnPQ(row.ErrorRows[j].ElementID.split('_')[0] + currentInstance.formInstanceId, rowNumber, row.ErrorRows[j].GeneratedName, row.ErrorRows[j].ColumnNumber);
                    if (td)
                        $(td).removeClass("repeater-has-error");
                }
            }
        }
    }
}

repeaterBuilderPQ.prototype.runRuleForRepeater = function (rule, parentRowId, childRowId) {
    this.ruleProcessor.processRule(rule, parentRowId, childRowId);
}

repeaterBuilderPQ.prototype.visibleRuleResultCallBack = function (rule, result) {
    if (rule.UIElementTypeID == 7) {
        if (result) {
            $(this.gridElementIdJQ).show();
        } else {
            $(this.gridElementIdJQ).hide();
        }
    }
    else {
        if (rule.ParentRepeaterType == 'In Line') {
            //get in line columns to hide
            var nameParts = rule.UIElementFullName.split('.');
            var dsName = nameParts[nameParts.length - 2];
            var colNamePart = 'INL_' + dsName + '_';
            var elemPart = '_' + rule.UIElementName;
            var colModels = $(this.gridElementIdJQ).pqGrid("option", "colModel");

            for (var idx = 0; idx < colModels.length; idx++) {
                if (colModels[idx].dataIndx.toString().indexOf(colNamePart) == 0 && colModels[idx].dataIndx.toString().indexOf(elemPart) > 0) {
                    if (result == true) colModels[idx].hidden = false; else colModels[idx].hidden = true;
                }
                else if (colModels[idx].colModel != undefined && colModels[idx].colModel.length > 0) {
                    for (var ids = 0; ids < colModels[idx].colModel.length; ids++) {
                        if (colModels[idx].colModel[ids].dataIndx.toString().indexOf(colNamePart) == 0 && colModels[idx].colModel[ids].dataIndx.toString().indexOf(elemPart) > 0) {
                            if (result == true) colModels[idx].colModel[ids].hidden = false; else colModels[idx].colModel[ids].hidden = true;
                        }
                    }
                }
            }

            $(this.gridElementIdJQ).pqGrid("option", "colModel", colModels);
            $(this.gridElementIdJQ).pqGrid('refresh');
        }
        else if (rule.ParentRepeaterType == 'Child') {
            //get child grid/column to hide
        }
        else {
            //get column to hide
            if (result == true) {
                $(this.gridElementIdJQ).pqGrid("getColumn", { dataIndx: rule.UIElementName }).hidden = false;
                $(this.gridElementIdJQ).pqGrid("refresh");
                //$(this.gridElementIdJQ).jqGrid('showCol', rule.UIElementName);
            }
            else {
                $(this.gridElementIdJQ).pqGrid("getColumn", { dataIndx: rule.UIElementName }).hidden = true;
                $(this.gridElementIdJQ).pqGrid("refresh");
                //$(this.gridElementIdJQ).jqGrid('hideCol', rule.UIElementName);
            }
        }
    }
    if (this.design.AllowBulkUpdate == true) {
        var width = $($(this.gridElementIdJQ).parent().closest('div').first()).width();
        $('#t_' + this.gridElementId).css('width', width);
    }
}

repeaterBuilderPQ.prototype.ruleResultCallBack = function (rule, row, retVal, childName, childIdx) {
    if (rule.ParentRepeaterType == null || rule.ParentRepeaterType == "Dropdown") {
        this.setParentRow(rule, row, retVal);
    }
    if (rule.ParentRepeaterType == 'In Line' || rule.ParentRepeaterType == 'Child') {
        this.setChildRow(rule, row, retVal, childName, childIdx);
    }
}

repeaterBuilderPQ.prototype.setParentRow = function (rule, row, retVal) {
    var currentInstance = this;
    var targetRepeaterJQ = currentInstance.gridElementIdJQ;
    var targetData = currentInstance.data;
    var rowIndex = row.RowIDProperty;

    if (rule.IsParentRepeater == true) {
        names = rule.UIElementFullName.split('.');
        var section = currentInstance.formInstanceBuilder.designData.Sections.filter(function (section) { return section.FullName == names[0]; });
        var element = section[0].Elements.filter(function (element) { return element.GeneratedName == names[1]; });
        if (element.length > 0 && element[0].Repeater != null) {
            var targetRepeaterJQ = '#' + element[0].Repeater.Name + currentInstance.formInstanceId;

            var dataKeyColumn = $(targetRepeaterJQ).pqGrid('getColModel').filter(function (a) { return a.iskey == true });
            var dataKeyName = dataKeyColumn[0].dataIndx;

            var targetData = $(targetRepeaterJQ).pqGrid("option", "dataModel.data");
            if (targetData != null && targetData.length > 0) {
                for (var idx = 0; idx < targetData.length; idx++) {
                    if (targetData[idx][dataKeyName] == row[dataKeyName]) {
                        rowIndex = idx;
                    }
                }
            }
        }
    }

    if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Value) {
        $.each(row, function (index, element) {
            if (index == rule.UIElementName) {
                element = retVal;
            }
        });

        targetData[rowIndex][rule.UIElementName] = retVal;
        $(targetRepeaterJQ).pqGrid("option", "dataModel.data", targetData);
        $(targetRepeaterJQ).pqGrid("refreshCell", { rowIndx: rowIndex, dataIndx: rule.UIElementName });
    }
    else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Visible) {
        if (retVal == true) {
            $(targetRepeaterJQ).pqGrid("getCell", { rowIndx: rowIndex, dataIndx: rule.UIElementName }).css('background', 'none');
            $(targetRepeaterJQ).pqGrid("getCell", { rowIndx: rowIndex, dataIndx: rule.UIElementName }).prop("disabled", false);
            this.setChildElementsIsVisible(targetRepeaterJQ, rowIndex, rule.UIElementName, rule.UIElementTypeID);
        }
        else {
            $(targetRepeaterJQ).pqGrid("getCell", { rowIndx: rowIndex, dataIndx: rule.UIElementName }).css('background', '#ddd');
            $(targetRepeaterJQ).pqGrid("getCell", { rowIndx: rowIndex, dataIndx: rule.UIElementName }).prop("disabled", true);
            this.setChildElementsIsVisible(targetRepeaterJQ, rowIndex, rule.UIElementName, rule.UIElementTypeID);
            this.removeVisibleAndDisabledElementFromErrorGrid(rowIndex, rule.UIElementName);
        }
    }
    else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Enabled) {
        if (retVal == true) {
            $(targetRepeaterJQ).pqGrid("getCell", { rowIndx: rowIndex, dataIndx: rule.UIElementName }).css('background', 'none');
            $(targetRepeaterJQ).pqGrid("getCell", { rowIndx: rowIndex, dataIndx: rule.UIElementName }).prop("disabled", false);
            this.setChildElementsIsDisabled(targetRepeaterJQ, rowIndex, rule.UIElementName, rule.UIElementTypeID);
        }
        else {
            $(targetRepeaterJQ).pqGrid("getCell", { rowIndx: rowIndex, dataIndx: rule.UIElementName }).css('background', '#ddd');
            $(targetRepeaterJQ).pqGrid("getCell", { rowIndx: rowIndex, dataIndx: rule.UIElementName }).prop("disabled", true);
            this.removeVisibleAndDisabledElementFromErrorGrid(rowIndex, rule.UIElementName);
        }
    }
    else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Error) {
        var el = $(targetRepeaterJQ).pqGrid("getCell", { rowIndx: rowIndex, dataIndx: rule.UIElementName });
        if ($(el).css('visibility') != undefined && $(el).css('visibility') == 'visible' && $(el)[0].attributes.disabled == undefined) {
            this.setElementErrorState(retVal, rule, row, targetRepeaterJQ);
        }
    }
    else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.IsRequired) {
        this.setElementIsRequired(retVal, rule, row);
    }
}

repeaterBuilderPQ.prototype.setChildRow = function (rule, row, retVal, childName, childIdx) {
    var targetRepeaterJQ = currentInstance.gridElementIdJQ;

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
            var iCol = getColumnSrcIndexByNamePQ($(this.gridElementIdJQ), 'INL_' + childName + '_' + childIdx + '_' + rule.UIElementName);
            var el = $("tr#" + row.RowIDProperty, this.gridElementIdJQ).find('td').eq(iCol);
            if ($(el).css('visibility') != undefined && $(el).css('visibility') == 'visible' && $(el)[0].attributes.disabled == undefined) {
                this.setElementErrorState(retVal, rule, row, targetRepeaterJQ);
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
                //$(childBuilder[0].gridElementIdJQ).jqGrid('setCell', childBuilder[0].data[childIdx][this.KeyProperty], rule.UIElementName, retVal);
                $(childBuilder[0].gridElementIdJQ).pqGrid("option", "dataModel.data", childBuilder[0].data);
                $(childBuilder[0].gridElementIdJQ).pqGrid("refreshCell", { rowIndx: childIdx, dataIndx: childBuilder[0].data[childIdx][this.KeyProperty] });
            }
            else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Visible) {
                if (retVal == true) {
                    //$(childBuilder[0].gridElementIdJQ).jqGrid('setCell', childBuilder[0].data[childIdx][this.KeyProperty], rule.UIElementName, '', { visibility: 'visible' });
                    $(childBuilder[0].gridElementIdJQ).pqGrid("getCell", { rowIndx: childIdx, dataIndx: childBuilder[0].data[childIdx][this.KeyProperty] }).css('background', 'none');
                    $(childBuilder[0].gridElementIdJQ).pqGrid("getCell", { rowIndx: childIdx, dataIndx: childBuilder[0].data[childIdx][this.KeyProperty] }).prop("disabled", false);
                    this.setChildElementsIsVisible(this.gridElementIdJQ, childBuilder[0].data[childIdx][this.KeyProperty], rule.UIElementName, rule.UIElementTypeID);
                }
                else {
                    //$(childBuilder[0].gridElementIdJQ).jqGrid('setCell', childBuilder[0].data[childIdx][this.KeyProperty], rule.UIElementName, '', { visibility: 'hidden' });
                    $(childBuilder[0].gridElementIdJQ).pqGrid("getCell", { rowIndx: childIdx, dataIndx: childBuilder[0].data[childIdx][this.KeyProperty] }).css('background', '#ddd');
                    $(childBuilder[0].gridElementIdJQ).pqGrid("getCell", { rowIndx: childIdx, dataIndx: childBuilder[0].data[childIdx][this.KeyProperty] }).prop("disabled", true);
                    this.setChildElementsIsVisible(this.gridElementIdJQ, childBuilder[0].data[childIdx][this.KeyProperty], rule.UIElementName, rule.UIElementTypeID);
                    this.removeVisibleAndDisabledElementFromErrorGrid(row.RowIDProperty, rule.UIElementName);
                }
            }
            else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Enabled) {
                if (retVal == true) {
                    //$(childBuilder[0].gridElementIdJQ).jqGrid('setCell', childBuilder[0].data[childIdx][this.KeyProperty], rule.UIElementName, '', '', { disabled: null });
                    $(childBuilder[0].gridElementIdJQ).pqGrid("getCell", { rowIndx: childIdx, dataIndx: childBuilder[0].data[childIdx][this.KeyProperty] }).css('background', 'none');
                    $(childBuilder[0].gridElementIdJQ).pqGrid("getCell", { rowIndx: childIdx, dataIndx: childBuilder[0].data[childIdx][this.KeyProperty] }).prop("disabled", false);
                    this.setChildElementsIsDisabled(this.gridElementIdJQ, childBuilder[0].data[childIdx][this.KeyProperty], rule.UIElementName, rule.UIElementTypeID);
                }
                else {
                    //$(childBuilder[0].gridElementIdJQ).jqGrid('setCell', childBuilder[0].data[childIdx][this.KeyProperty], rule.UIElementName, '', '', { disabled: 'disabled' });
                    $(childBuilder[0].gridElementIdJQ).pqGrid("getCell", { rowIndx: childIdx, dataIndx: childBuilder[0].data[childIdx][this.KeyProperty] }).css('background', '#ddd');
                    $(childBuilder[0].gridElementIdJQ).pqGrid("getCell", { rowIndx: childIdx, dataIndx: childBuilder[0].data[childIdx][this.KeyProperty] }).prop("disabled", true);
                    this.setChildElementsIsDisabled(this.gridElementIdJQ, childBuilder[0].data[childIdx][this.KeyProperty], rule.UIElementName, rule.UIElementTypeID);
                    this.removeVisibleAndDisabledElementFromErrorGrid(row.RowIDProperty, rule.UIElementName);
                }
            }
            else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Error) {
                //var iCol = getColumnSrcIndexByNamePQ($(this.gridElementIdJQ), rule.UIElementName);
                //var el = $("tr#" + childBuilder[0].data[childIdx][this.KeyProperty], this.gridElementIdJQ).find('td').eq(iCol);
                var el = $(childBuilder[0].gridElementIdJQ).pqGrid("getCell", { rowIndx: childIdx, dataIndx: childBuilder[0].data[childIdx][this.KeyProperty] });
                if ($(el).css('visibility') != undefined && $(el).css('visibility') == 'visible' && $(el)[0].attributes.disabled == undefined) {
                    this.setElementErrorState(retVal, rule, row, targetRepeaterJQ);
                }
            }
            else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.IsRequired) {
                this.setElementIsRequired(retVal, rule, row);
            }
        }
    }
}

repeaterBuilderPQ.prototype.gridMethods = function () {
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

repeaterBuilderPQ.prototype.saveRow = function (isDataBinded, bulkUpdateRowData) {
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
            //var success = $(".selector").pqGrid("saveEditCell");
            //$(currentInstance.gridElementIdJQ).pqGrid("commit", { type: 'update', rows: currentInstance.lastSelectedRow });
        }
        //$(this.gridElementIdJQ).jqGrid('restoreRow', currentInstance.lastSelectedRow);
    } catch (ex) {
    }

    var rowData = currentInstance.getPQRowData();

    var colM = []
    $.each(currentInstance.columnModels, function (idx, data) {
        colM.push(data.dataIndx);
    })

    if (bulkUpdateRowData != undefined) {
        rowData = bulkUpdateRowData;
    }

    if (currentInstance.lastSelectedRow != null) {
        //copy over primary values

        var defualtRow = currentInstance.getDefualtRow();

        if (rowData != null && rowData.RowIDProperty != null) {
            defualtRow.RowIDProperty = rowData.RowIDProperty;
        }

        //if (currentInstance.fullName.split('.')[0] == "CostShare" && isDataBinded == true) {
        //    dataBeforeChanges = $.extend(true, {}, currentInstance);
        //}

        for (var prop in colM) {
            if (currentInstance.displayChildGridAsPopup && prop == "") {
                continue;
            }
            if (colM[prop].substring(0, 4) != 'INL_') {
                if (colM[prop] != undefined && colM[prop] != null && rowData[colM[prop]] != undefined && rowData[colM[prop]] != null) {
                    defualtRow[colM[prop]] = rowData[colM[prop]];
                }
            }
        }

        if (currentInstance.design.ChildDataSources != null && Array.isArray(currentInstance.design.ChildDataSources) && currentInstance.design.ChildDataSources.length > 0 && currentInstance.design.ChildDataSources[0].DisplayMode == "Child") {

            var currentdata = currentInstance.data.filter(function (dt) {
                return dt.RowIDProperty == defualtRow.RowIDProperty
            })

            var childDataSource = currentInstance.design.ChildDataSources[0];
            defualtRow[childDataSource.DataSourceName] = currentdata[0][childDataSource.DataSourceName];
        }

        for (var prop in colM) {
            if (colM[prop].substring(0, 4) == 'INL_') {
                var propArr = colM[prop].split('_');
                if (propArr.length == 4) {
                    var dsName = propArr[1];
                    var propIdx = propArr[2];
                    var propName = propArr[3];
                    if (colM[prop] !== undefined && rowData[colM[prop]] !== undefined) {
                        defualtRow[dsName][propIdx][propName] = rowData[colM[prop]];
                    }
                }
            }
        }

        var row = currentInstance.data.filter(function (dt) {
            return dt.RowIDProperty == currentInstance.lastSelectedRow;
        });

        if (row.length == 0) {
            currentInstance.data.push(defualtRow);
        }
        else {
            $.each(currentInstance.data, function (idx, dt) {
                if (dt.RowIDProperty == currentInstance.lastSelectedRow) {
                    currentInstance.data[idx] = defualtRow;
                    return;
                }
            })
        }
    }
    if (currentInstance.lastSelectedRow != null) {
        //currentInstance.updateTargetDataSourceCollection(currentInstance.lastSelectedRow, "save");
    }

    if (currentInstance.rowDataBeforeEdit) {
        currentInstance.formInstanceBuilder.hasChanges = true;
        currentInstance.hasChanges = true;
        currentInstance.rowDataBeforeEdit = undefined;
    }
    if (bulkUpdateRowData == undefined) {
        if (row != null && row.length == 1) {
            currentInstance.updateTargetDataSourceCollection(row[0].RowIDProperty, "save");
        }
    }
}

repeaterBuilderPQ.prototype.saveRowBulkUpdate = function (isDataBinded, bulkUpdateRowData) {
    var currentInstance = this;
    var isEdited = false;
    var dataBeforeChanges = null;
    //var saveParameters = {
    //    "url": 'clientArray',
    //    "aftersavefunc": function () {
    //        currentInstance.bindData();
    //    }
    //};
    try {
        if (currentInstance.lastSelectedRow != null) {
            // $(currentInstance.gridElementIdJQ).jqGrid('saveRow', currentInstance.lastSelectedRow, saveParameters);
        }
        //$(this.gridElementIdJQ).jqGrid('restoreRow', currentInstance.lastSelectedRow);
    } catch (ex) {
    }

    var rowData = currentInstance.getPQRowData();//$(currentInstance.gridElementIdJQ).pqGrid("getRowData", { rowIndx: currentInstance.lastSelectedRow });

    if (bulkUpdateRowData != undefined) {
        rowData = bulkUpdateRowData;
    }

    var colM = []
    $.each(currentInstance.columnModels, function (idx, data) {
        colM.push(data.dataIndx);
    })

    var defualtRow = currentInstance.getDefualtRow();
    if (currentInstance.lastSelectedRow != null) {
        //copy over primary values

        defualtRow.RowIDProperty = rowData.RowIDProperty;

        for (var prop in colM) {
            if (currentInstance.displayChildGridAsPopup && prop == "") {
                continue;
            }
            if (colM[prop].substring(0, 4) != 'INL_') {
                defualtRow[colM[prop]] = rowData[colM[prop]];
            }
        }
        for (var prop in colM) {
            if (colM[prop].substring(0, 4) == 'INL_') {
                var propArr = colM[prop].split('_');
                if (propArr.length == 4) {
                    var dsName = propArr[1];
                    var propIdx = propArr[2];
                    var propName = propArr[3];
                    defualtRow[dsName][propIdx][propName] = rowData[colM[prop]];
                }
            }
        }

        var row = currentInstance.data.filter(function (dt) {
            return dt.RowIDProperty == currentInstance.lastSelectedRow;
        });

        if (row.length == 0) {
            currentInstance.data.push(defualtRow);
        }
        else {
            $.each(currentInstance.data, function (idx, dt) {
                if (dt.RowIDProperty == currentInstance.lastSelectedRow) {
                    currentInstance.data[idx] = defualtRow;
                    return;
                }
            })
        }
    }
}


repeaterBuilderPQ.prototype.addEntryToAcitivityLogger = function (rowId, operation, colName, oldValue, newValue, buFilterCriteria, childGridPath, prop, customMsg) {
    var currentInstance = this;
    var fields = currentInstance.fullName.split('.');
    var field = fields[fields.length - 1];

    var colNames = $(currentInstance.gridElementIdJQ).pqGrid("option", "colModel");
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

    if (currentInstance.fullName == currentInstance.ProofingRepeaterName) {
        if (currentInstance.fullName.indexOf("" + currentInstance.design.GeneratedName + "") >= 0)
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
                if (element.dataIndx.toString().indexOf("_") >= 0) {
                    if (element.dataIndx.toString().substring(0, 4) == 'INL_') {
                        var keyarr = element.dataIndx.toString().split('_');
                        if (keyarr.length == 4) {
                            if (headercolumnname == keyarr[2]) {
                                var row = currentInstance.data.filter(function (dt) { return dt.RowIDProperty == rowId });
                                if (row[0] != undefined) {
                                    var elementName = element.dataIndx.toString().split("_");
                                    var eleName = elementName[elementName.length - 1];
                                    var value = row[0][currentInstance.design.ChildDataSources[0].DataSourceName][elementName[elementName.length - 2]][eleName];
                                    keyValue += value + "#";
                                }
                            }
                        }
                    }
                }
                else {
                    var row = currentInstance.data.filter(function (dt) { return dt.RowIDProperty == rowId });
                    if (row[0] != undefined) {
                        var value = row[0][element.dataIndx];
                        keyValue += value + "#";
                    }
                }
            }
        });
    }
    if (keyValue != "") {
        keyValue = keyValue.substring(0, keyValue.length - 1);
    }
    activitylogger.logRepeaterPQ(fullName, colName, rowId, operation, currentUserName, updatedDate, currentInstance.design.Label, parseInt(currentInstance.formInstanceId), oldValue, newValue, buFilterCriteria, currentInstance.design, repeaterId, keyValue, customMsg);
}

repeaterBuilderPQ.prototype.getElementValue = function (elementType, element, val) {
    var retVal = "";
    if (elementType == 'select') {
        retVal = $(element).find(":selected").text();
    }
    else {
        retVal = $(element).val();
        //if (elementType == 'checkbox') {
        //    if ($(element).prop('checked') == true) {
        //        retVal = 'Yes';
        //    }
        //    else {
        //        retVal = 'No';
        //    }
        //}
    }
    return retVal;
}

repeaterBuilderPQ.prototype.getDropdownTextboxOptions = function (items, isSortRequired) {
    //sorts options in ascending order
    //sorts options in ascending order
    //if (isSortRequired == true) {
    //    items = this.sortOptions(items);
    //}

    var options = "";
    var dd = [];
    if (items != null && items.length > 0) {
        var defaultValue = {};
        defaultValue[""] = "";
        dd.push(defaultValue);


        for (var idx = 0; idx < items.length; idx++) {
            if (items[idx].ItemValue != '') {
                defaultValue = {};
                defaultValue[items[idx].ItemValue] = items[idx].ItemText == null ? items[idx].ItemValue : items[idx].ItemText;
                dd.push(defaultValue);
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

    defaultValue = {};
    defaultValue["newItem"] = "Enter Unique Response";
    dd.push(defaultValue);
    return dd;
}

repeaterBuilderPQ.prototype.getDropdownTextboxOptionValues = function (items, isSortRequired) {
    //sorts options in ascending order
    if (isSortRequired == true) {
        items = this.sortOptions(items);
    }

    var options = "";
    if (items != null && items.length > 0) {
        options = options + Validation.selectOne + ':' /*+ Validation.selectOne */ + ';';
        for (var idx = 0; idx < items.length; idx++) {
            if (items[idx].ItemValue != '') {
                options = options + items[idx].ItemValue + ':' + (items[idx].ItemText == null ? items[idx].ItemValue : items[idx].ItemText);
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


repeaterBuilderPQ.prototype.formatDropdownTextbox = function (idpluscol, elementDesign, rowId) {
    var currentInstance = this;
    var count = 0;
    //get current value of the row element
    var currentVal = $('#' + idpluscol).closest('td').attr('title');
    //if (currentVal === "Not Applicable" || currentVal === "No Copay") {
    //    $('#' + idpluscol).closest('td').attr("disabled", "disabled");
    //}
    if (currentVal != undefined) {
        $.each(elementDesign.Items, function (key, item) {
            if (item.ItemValue.toUpperCase() === currentVal.toUpperCase())
                count++;
        });
        if (currentVal != "" && count == 0) {
            $('<option class="non-standard-optn" value="' + currentVal + '">' + currentVal + '</option>').insertBefore($('#' + idpluscol).find('option').last());
            $('#' + idpluscol).val(currentVal);
        }
    }


    $("#" + idpluscol, this.gridElementIdJQ).parent().append('<input id="' + idpluscol + 'Textbox" type="text" name="' + elementDesign.GeneratedName + '" style="display:none;" class="ddt-textbox"></input>');
    //$('.ddt-dropdown').unbind('change');
    $("#" + idpluscol).change(function () {
        if ($(this).val() == 'newItem') {
            $(this).val('');
            $(this).parent().find('.ddt-textbox').toggle().focus();
            $(this).toggle();
        }
    });
    //$('.ddt-textbox').unbind('focusout');
    $('.ddt-textbox', currentInstance.gridElementIdJQ).on("focusout", function (e) {
        $(this).toggle();
        $(this).parent().find($("#" + idpluscol)).toggle();

    });

    //$('.ddt-textbox').unbind('keyup');
    $('.ddt-textbox', currentInstance.gridElementIdJQ).on("keyup", function (e) {
        if (e.which !== 16) {
            var newValue = $(this).val();
            var dropdownTextboxControl = $(this).parent().find('#' + idpluscol);
            var stringUtility = new globalutilities();
            if (!stringUtility.stringMethods.isNullOrEmpty(newValue)) {
                if ($(this).parent().find('#' + idpluscol + ' option').hasClass("non-standard-optn") == true) {
                    $(this).parent().find('#' + idpluscol + ' option.non-standard-optn').remove();
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
                    $('<option value="' + newValue + '" class="non-standard-optn">' + newValue + '</option>').insertBefore($(this).parent().find('#' + idpluscol + ' option').last());
                    dropdownTextboxControl.val(newValue);
                    dropdownTextboxControl.trigger("change");
                }
            }
        }
    });
}

repeaterBuilderPQ.prototype.isStandardDropdownItem = function (elementDesign, value) {
    var isStndOption = "false";
    $.each(elementDesign.Items, function (key, item) {
        if (item.ItemValue.toUpperCase() === value.toUpperCase())
            return isStndOption = "true";
    });
    return isStndOption;
}

repeaterBuilderPQ.prototype.isPrimaryDataExits = function () {
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
repeaterBuilderPQ.prototype.updateTargetDataSourceCollection = function (rowID, status) {
    var currentInstance = this;
    var rowIDProperty = rowID;

    if (rowIDProperty != undefined) {
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

                if (isFilter == false && ct.DataSourceModeType != "Manual" && ct.DataSourceModeType != "Custom") {
                    if (currentInstance.formInstanceBuilder.formDesignId == ct.FormDesignID && (currentInstance.formInstanceBuilder.selectedSectionName).replace(/[^A-Z0-9]/ig, "") == ct.TargetParent.split('.')[0]) {

                        var targetSection = ct.TargetParent;

                        //get target repeater
                        var targetRepeater = currentInstance.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                            return targetSection == rb.fullName;
                        });

                        //create instance of datasourcesync
                        var sourcesynchroniser = new datasourcesync(currentInstance.formInstanceBuilder, currentInstance, ct, rowIDProperty, status);

                        //switch to data source 
                        switch (ct.DisplayMode) {

                            case "Primary":

                                var matchProperties = sourcesynchroniser.syncMapPrimaryDataSource();

                                //if section is loaded rebind the data
                                if (targetRepeater.length != 0 && matchProperties.RowIndex > -1) {
                                    currentInstance.updateTargetRepeater(matchProperties.RowIndex, matchProperties.RowIDProperty, targetRepeater[0], status);

                                    /*if (status == 'add' && currentInstance.data.length == 1) {
                                        $(targetRepeater[0].gridElementIdJQ).pqGrid("destroy");
                                        targetRepeater[0].columnModels = [];
                                        targetRepeater[0].columnNames = [];
                                        targetRepeater[0].gridElementId = null;
                                        targetRepeater[0].gridElementIdJQ = null;
                                        targetRepeater[0].groupHeaders = [];
                                        targetRepeater[0].build();
                                    }*/
                                }
                                break;

                            case "In Line":

                                sourcesynchroniser.syncMapInLineDataSource();

                                //if section is loaded render the section
                                if (targetRepeater.length != 0) {
                                    //$(targetRepeater[0].gridElementIdJQ).jqGrid('GridUnload');
                                    $(targetRepeater[0].gridElementIdJQ).pqGrid("destroy");

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
                                            currentInstance.updateTargetRepeater(rowIDProperty, rowID, childGridBuilder, status);
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
                                        currentInstance.formInstanceBuilder.updateTargetSectionDropdown(currentInstance, rowIDProperty, ct, status, dropdwonelement, sourcesynchroniser);
                                    }
                                    else {
                                        //for repeater dropdown
                                        dropdwonelement = currentInstance.getRepeaterDropdownElement(ct);
                                        var itemsArray = sourcesynchroniser.updateRepeaterDropDown(dropdwonelement[1], status);
                                        currentInstance.updateRepeaterDropdownSelectedValue(targetRepeater, itemsArray, ct, sourcesynchroniser);
                                    }
                                }
                                else {
                                    currentInstance.updateTargetRepeaterDropdown(targetRepeater, rowIDProperty, ct, status, sourcesynchroniser);
                                }
                                break;
                        }
                    }
                    //clear object of synchroniser class
                    sourcesynchroniser = null;
                }
            });

        }
    }
}

repeaterBuilderPQ.prototype.updateTargetRepeater = function (rowIndex, rowID, targetRepeater, status) {
    var currentInstance = this;
    //get data of target repeater
    var targetData = $(targetRepeater.gridElementIdJQ).pqGrid("option", "dataModel.data");
    //get data ids of target repeater
    var targetRepeaterDataIdList = [];
    var len = targetData.length;
    for (var i = 0; i < len; i++) {
        targetRepeaterDataIdList.push(targetData[i].RowIDProperty);
    }
    if (targetData != undefined) {

        // if (currentInstance.data.length == targetData.length) {
        if (status == "save") {
            var updateflag = undefined;
            if (targetData[rowIndex] != undefined) {

                //update row if rowid present targetRepeaterDataIdList of target repeater
                for (var i = 0; i < targetRepeaterDataIdList.length; i++) {
                    if (targetRepeaterDataIdList[i] == rowID) {
                        updateflag = true;
                        var newRow = $(targetRepeater.gridElementIdJQ).pqGrid('getRowData', { rowIndx: rowIndex });
                        //modify data to match repeater columns
                        if (newRow != null) {
                            if (targetRepeater.design.ChildDataSources != null && Array.isArray(targetRepeater.design.ChildDataSources) && targetRepeater.design.ChildDataSources.length > 0 && targetRepeater.design.ChildDataSources[0].DisplayMode == "In Line") {
                                $.each(targetRepeater.columnModels, function (idx, col) {
                                    if (col.dataIndx.toString().substring(0, 4) == 'INL_') {
                                        var propArr = col.dataIndx.toString().split('_');
                                        if (propArr.length == 4) {
                                            var dsName = propArr[1];
                                            var propIdx = propArr[2];
                                            var propName = propArr[3];
                                            if (targetRepeater.data[rowIndex] != null && targetRepeater.data[rowIndex][dsName] != null && targetRepeater.data[rowIndex][dsName][propIdx] != null && targetRepeater.data[rowIndex][dsName][propIdx][propName] != null) {
                                                newRow[col.dataIndx] = targetRepeater.data[rowIndex][dsName][propIdx][propName];
                                            }
                                        }
                                    }
                                    else {
                                        if (targetRepeater.data[rowIndex][col.dataIndx] != null) {
                                            newRow[col.dataIndx] = targetRepeater.data[rowIndex][col.dataIndx];
                                        }
                                    }
                                });
                            }
                        }
                        $(targetRepeater.gridElementIdJQ).pqGrid("refreshDataAndView");
                        //$(targetRepeater.gridElementIdJQ).pqGrid('updateRow', { rowIndx: rowIndex, row: newRow, track : false, history:false});
                        break;
                    }
                }

                //update row using data of target repeater if flag is not true
                /*if (updateflag != true) {
                    var rowItem = targetData[rowIndex];
                    if (rowItem != undefined) {
                        $.each(targetRepeater.data[rowIndex], function (el) {
                            rowItem[el] = targetRepeater.data[rowIndex][el];
                        });
                    }
                }*/
            }
        }
        //else if (currentInstance.data.length < targetData.length) {
        if (status == "delete") {
            var deleteflag = undefined;

            //delete row if row index is present in targetRepeaterDataIdList of target repeater
            for (var i = 0; i < targetRepeaterDataIdList.length; i++) {
                if (targetRepeaterDataIdList[i] == rowID) {
                    deleteflag = true;
                    //$(targetRepeater.gridElementIdJQ).delRowData(rowID);
                    $(targetRepeater.gridElementIdJQ).pqGrid("deleteRow", { rowIndx: rowIndex });
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
                    //$(targetRepeater.gridElementIdJQ).jqGrid('addRowData', targetRepeater.data[rowIndex].RowIDProperty, targetRepeater.data[rowIndex]);
                    $(targetRepeater.gridElementIdJQ).pqGrid('addRow', { rowIndx: rowID, rowData: targetRepeater.data[rowIndex] });
                }
            }
        }
    }
}

repeaterBuilderPQ.prototype.setElementIsRequired = function (result, rule, row) {
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

repeaterBuilderPQ.prototype.setElementErrorState = function (result, rule, row, targetRepeaterJQ) {
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

    //var repeaterIndexes = $(currentInstance.gridElementIdJQ).jqGrid('getGridParam', '_index');
    //var rowIndex = repeaterIndexes[row.RowIDProperty];
    var $tr = $(targetRepeaterJQ).pqGrid("getCell", { rowIndx: row.RowIDProperty, dataIndx: rule.UIElementName }).closest("tr");
    var rowIndex = $(targetRepeaterJQ).pqGrid("getRowIndx", { $tr: $tr }).rowIndx

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
repeaterBuilderPQ.prototype.getRepeaterDropdownElement = function (dataSource) {
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

repeaterBuilderPQ.prototype.updateTargetRepeaterDropdown = function (targetRepeater, rowIndex, dataSource, status, sourcesynchroniser) {
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

    if (colprop) {
        dropdownitems = colprop.editoptions.value.split(';');
    }
    else {
        var colObj = targetRepeater[0].columnModels.filter(function (itm, val) {
            return itm.dataIndx == colname;

        });

        dropdownitems = colObj[0].editor.value.split(';');
    }

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

repeaterBuilderPQ.prototype.updateRepeaterDropdownSelectedValue = function (targetRepeater, itemsArray, dataSource, sourcesynchroniser) {

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

repeaterBuilderPQ.prototype.checkDuplicate = function (rowId, loadFromServer) {
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

repeaterBuilderPQ.prototype.removeErrorGridRowOnRepeaterDeleteRow = function (rowId) {
    var currentInstance = this;
    var rowNum = undefined;

    //index repeater
    //var index = $(currentInstance.gridElementIdJQ).jqGrid('getGridParam', '_index');
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
                var gridRowData = $(currentInstance.gridElementIdJQ).pqGrid("option", "dataModel.data");
                //var gridRowData = $(currentInstance.gridElementIdJQ).jqGrid('getGridParam', 'data');
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

repeaterBuilderPQ.prototype.addDefualtRow = function () {
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
repeaterBuilderPQ.prototype.setChildElementsIsDisabled = function (grid, rowId, columnName, uiElementTypeId) {
    var iCol = getColumnSrcIndexByNamePQ($(grid), columnName);
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
repeaterBuilderPQ.prototype.setChildElementsIsVisible = function (grid, rowId, columnName, uiElementTypeId) {
    var iCol = getColumnSrcIndexByNamePQ($(grid), columnName);
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

repeaterBuilderPQ.prototype.removeVisibleAndDisabledElementFromErrorGrid = function (rowId, UIElementName) {
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

repeaterBuilderPQ.prototype.repeaterControlFocusOut = function (e, row) {
    var currentInstance = this;
    var elem = e.target;
    var rowId = row;
    if (rowId != null && rowId != undefined) {
        var designElem = currentInstance.design.Elements.filter(function (el) {
            return el.GeneratedName == elem.name;
        });
        if (designElem != null && designElem.length > 0) {
            designElem = designElem[0];
            var rules = currentInstance.formInstanceBuilder.rules.getRulesForElement(designElem.FullName);
            if (rules.length == 0) {
                var rowIndex = rowId;
                var ID = $(currentInstance.gridElementIdJQ).jqGrid("getDataIDs");
                $.each(ID, function (idx, ct) {
                    if (ct == rowId) {
                        rowIndex = idx;
                        return false;
                    }
                });
                var selectedRowId = rowId;
                var currentPage = $(currentInstance.gridElementIdJQ).pqGrid("option", "pageModel.curPage");
                var rowNum = $(currentInstance.gridElementIdJQ).pqGrid("option", "pageModel.rPP");
                if (currentPage != 1) {
                    rowIndex = ((currentPage - 1) * rowNum) + rowIndex;
                }

                var keyValue = "";
                var repeaterId = currentInstance.design.Name + currentInstance.formInstanceId;
                $.each(currentInstance.design.Elements, function (index, element) {
                    if (element.IsKey == true) {
                        var rowData = currentInstance.getPQRowData();//$("#" + repeaterId).pqGrid("getRowData", { rowIndx: selectedRowId });
                        var value;
                        if (currentInstance.rowDataBeforeEdit != undefined)
                            value = currentInstance.rowDataBeforeEdit[element.GeneratedName];
                        else
                            value = rowData[element.GeneratedName];

                        keyValue += value + "#";
                    }
                });
                if (keyValue != "") {
                    rowIndex = rowIndex + "|" + keyValue.substring(0, keyValue.length - 1);
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
                        //var gridRowData = $(currentInstance.gridElementIdJQ).jqGrid('getGridParam', 'data');
                        var gridRowData = $(currentInstance.gridElementIdJQ).pqGrid("option", "dataModel.data");
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

repeaterBuilderPQ.prototype.showValidatedControlsOnRepeaterElementChange = function (validationError) {
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

    currentInstance.formInstanceBuilder.bottomMenu.closeBottomMenu();
    currentInstance.formInstanceBuilder.validation.loadValidationErrorGrid();
}

repeaterBuilderPQ.prototype.removeValidationErrorOfControl = function (row) {
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

repeaterBuilderPQ.prototype.repeaterServerMethods = function () {
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

repeaterBuilderPQ.prototype.setRepeaterHeight = function () {
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

repeaterBuilderPQ.prototype.isRowDataChanges = function (rowId, rowData) {
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

repeaterBuilderPQ.prototype.showDuplicateControlsOnRepeaterElementChange = function (validationError) {
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

repeaterBuilderPQ.prototype.setCursorOnEditRow = function (rowId, columnIndex) {
    $(this.gridElementIdJQ).pqGrid("setSelection", { rowIndx: rowId, focus: true });
}

repeaterBuilderPQ.prototype.getRowDataFromInstance = function (rowId) {
    var currentInstance = this;

    var rowData = currentInstance.data.filter(function (dt) {
        return dt.RowIDProperty == rowId;
    });

    return rowData;
}

repeaterBuilderPQ.prototype.registerExpandCollapseEvents = function () {
    $(".ui-icon-extlink").off("click");
    var currentInstance = this;
    $(currentInstance.gridElementIdJQ).on("click", ".ui-icon-extlink", function (e) {
        var rowID = $(this).attr("Id");
        var isSetDisable = $(this).closest('.repeater-grid').hasClass('disabled');
        $(this).removeClass("ui-icon-extlink").addClass("ui-icon-minus");
        //var selRowID = $(currentInstance.gridElementIdJQ).jqGrid('getGridParam', 'selrow');
        //var elementsToFind = "input,select,textarea";
        //if (selRowID != rowID) {
        //    if (selRowID !== undefined && selRowID != null) {
        //        var rowHtml = $(currentInstance.gridElementIdJQ).jqGrid('getGridRowById', selRowID);
        //        if (rowHtml != undefined && rowHtml != null && $(rowHtml).find(elementsToFind).length > 0) {
        //            //$(currentInstance.gridElementIdJQ).jqGrid('saveRow', selRowID);
        //            currentInstance.saveRow();
        //        }
        //    }
        //    $(currentInstance.gridElementIdJQ).jqGrid("setSelection", rowID);
        //}
        //var rowHtml = $(currentInstance.gridElementIdJQ).jqGrid('getGridRowById', rowID);
        //if (rowHtml != undefined && rowHtml != null && $(rowHtml).find(elementsToFind).length > 0) {
        //    //$(currentInstance.gridElementIdJQ).jqGrid('saveRow', rowID);
        //    currentInstance.saveRow();
        //}        
        currentInstance.displaySubGridData(rowID, this, isSetDisable);
        e.stopPropagation();
    });
}

repeaterBuilderPQ.prototype.displaySubGridData = function (rowID, expandedRow, isSetDisable) {
    var currentInstance = this;
    ajaxDialog.showPleaseWait();
    setTimeout(function () {
        var subGridID = currentInstance.gridElementId + "_" + rowID;

        //var gridArr = $(currentInstance.gridElementIdJQ).getDataIDs();
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
                //beforeClose: function () {
                //    $("#subGridDialog").dialog("destroy");
                //    $("#subGridDialogData").html("");
                //    $("#subGridDialogPrevBtn").off("click");
                //    $("#subGridDialogNextBtn").off("click");
                //    $("#subGridPreviousBtnContainer").css("margin-top", "0");
                //    $("#subGridNextBtnContainer").css("margin-top", "0");
                //    $("#subGridDialogPrevBtn").attr("disabled", false);
                //    $("#subGridDialogNextBtn").attr("disabled", false);
                //},
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
        currentInstance.loadChildGrids(subGridID, rowID, isSetDisable);
        //var marginTop = ($("#subGridDialogData").height() - 80) / 2;
        //$("#subGridPreviousBtnContainer").css("margin-top", marginTop);
        //$("#subGridNextBtnContainer").css("margin-top", marginTop);

        //$("#subGridDialogPrevBtn").attr("disabled", true);
        //$("#subGridDialogNextBtn").attr("disabled", true);
        //if (gridArr.indexOf(rowID) > 0) {
        //    $("#subGridDialogPrevBtn").attr("disabled", false);
        //}
        //if (gridArr.indexOf(rowID) < gridArr.length - 1) {
        //    $("#subGridDialogNextBtn").attr("disabled", false);
        //}
        ajaxDialog.hidePleaseWait();
    }, 100);
}

repeaterBuilderPQ.prototype.toggleSubGridData = function (newIndex, gridIDs, currentRow) {
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

repeaterBuilderPQ.prototype.displaySelectedRowDatails = function (rowID) {
    var currentInstance = this;
    //var row = $(currentInstance.gridElementIdJQ).getRowData(rowID);
    var index = -1;
    var dataModel = $(currentInstance.gridElementIdJQ).pqGrid("option", "dataModel");
    $.each(dataModel.data, function (idx, data) {
        if (data.RowIDProperty == rowID) {
            index = idx;
        }
    })

    //var row = $(currentInstance.gridElementIdJQ).getRowData(rowID);
    var row = $(currentInstance.gridElementIdJQ).pqGrid("getRowData", { rowIndx: index });
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
repeaterBuilderPQ.prototype.setCursorToEndOfText = function (child) {
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


repeaterBuilderPQ.prototype.addBulkUpdateHeader = function (colModels) {
    var es = "";
    var currentInstance = this;
    var $grid = $(currentInstance.gridElementIdJQ);

    $.each(colModels, function (ind, value) {

        //pq-grid-hd-search-field

        var id = '#gs_' + value.dataIndx;
        var buElement = currentInstance.getHeaderElement(this);
        var elementFullPath = currentInstance.fullName + "." + value.dataIndx;

        if (buElement != null && buElement != undefined) {

            var filterItems = $grid.find(("input[name='" + value.dataIndx + "']"));//.hasClass('pq-grid-hd-search-field').closest('div').append(buElement);

            $.each(filterItems, function (idx, itemsValues) {

                if ($(itemsValues).hasClass('pq-grid-hd-search-field')) {
                    $(itemsValues).closest('div').append(buElement);
                }

            });


            //$grid.closest("div.repeater-grid").find(id).closest('div').append(buElement);
        }

        if (buElement != null && buElement != undefined) {
            $grid.closest("div.repeater-grid").find(id).closest('div').append(buElement);
            if ($(buElement).find('select').attr('id') != undefined) {
                for (var i = 0; i < $(buElement).find('select').find('option').length; i++) {
                    if ($(buElement).find('select').find('option')[i].text == "Enter Unique Response") {
                        currentInstance.formatBulkUpdateControlsDropdownTextbox(buElement, value.dataIndx);
                    }
                }
            }
        }

        if (currentInstance.bulkUpdateCopiedRowData) {
            var copydata = currentInstance.bulkUpdateCopiedRowData[value.dataIndx];
            var idpath = this.dataIndx;
            var buValue = $grid.find('[id*=bu_' + idpath + ']');
            $.each(buValue, function () {
                this.value = copydata;
                if (this.type == "checkbox") {
                    this.checked = copydata;
                }
            });

        }
    });

    $grid.find('[id*=bu_]').on("keydown", function (event) {
        if (event.keyCode == 9) {   //tab pressed
            event.preventDefault(); // stops its action
            event.stopImmediatePropagation();
        }
    });

}

repeaterBuilderPQ.prototype.getHeaderElement = function (objElement) {
    var element;
    var elementTemplate = "<table class='bulkupdates' cellspacing='0' style='margin-bottom:2%'>" +
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
            element = "<input type='text' id='bu_" + objElement.dataIndx + "' name='bu_" + objElement.dataIndx + "' class='pq-grid-hd-search-field' maxlength='" + objElement.editor.MaxLength + "'/>";
            break;
        case "textarea":
            element = "<input type='textarea' class='pq-grid-hd-search-field' id='bu_" + objElement.dataIndx + "' name='bu_" + objElement.dataIndx + "' value='Yes' offval='No'/>";
            break;


        case "select":
        case "selectInput":
            element = "<select  style='margin-left:-10px;width:130px'  name='bu_" + '_' + objElement.dataIndx + "' class='pq-grid-hd-search-field'  id='bu_" + objElement.dataIndx + "'>";
            $.each(objElement.editor.value.split(';'), function (key, val) {
                var option = val.split(':');
                if (option[1] == "Enter Unique Response") {
                    element += "<option value='" + option[0] + "'>" + option[1] + "</option>";
                }
                else if (option[0] != undefined && option[1] != undefined) {
                    element += "<option value='" + option[0] + "' class='standard-optn'>" + option[1] + "</option>";
                }
            });
            element += "</select>";
            if (objElement.editor.options != undefined && objElement.editor.options.length >= 0) {
                element += "<input id='bu_" + objElement.dataIndx + "Textbox' type='text' name='bu_" + objElement.dataIndx + "' class='ddt-textbox pq-grid-hd-search-field' style='margin-left:-10px;width:130px; display:none;'></input>";
            }
            break;

        case "checkbox":
            element = "<input align='center' type='checkbox' id='bu_" + objElement.dataIndx + "' name='bu_" + objElement.dataIndx + "' style='margin-left: 48px;' class='pq-grid-hd-search-field'/>";
            break;
    }

    if (element != undefined) {
        element = elementTemplate.replace("{{0}}", element);
    }

    return element;
}

repeaterBuilderPQ.prototype.addBulkUpdateControls = function () {
    var currentInstance = this;
    var $grid = $(currentInstance.gridElementIdJQ);
    $('#' + currentInstance.gridElementId).find('.bucontrol').remove();
    $('#' + currentInstance.gridElementId).find('.pq-toolbar').append("<div class='bucontrol' style='float:right;'/>");
    $('#' + currentInstance.gridElementId).find('.bucontrol').append("<input type='checkbox' id='chkEnable' disabled='disabled' value='true'/> Enable Bulk Update  " +
                                                                     "<input type='checkbox' id='chkCopyRow' disabled='disabled' value='true'> Copy Selected Row    ");
    $('#' + currentInstance.gridElementId).on('click', '#chkEnable', function () {
        if (this.checked) {
            $('#' + currentInstance.gridElementId).find('#btnUpdate').show();
            currentInstance.addBulkUpdateHeader(currentInstance.columnModels);
            //disable filter toolbar
            $grid.closest("div.repeater-grid").find(".ui-search-table").attr("disabled", "disabled");

            var $gridOuterHeader = $grid.find('.pq-header-outer');
            var headerHeight = parseInt($gridOuterHeader.css("height"), 10);
            $gridOuterHeader.css('height', headerHeight + 38);
            $grid.find('#chkCopyRow').attr('disabled', 'disabled');

            var $gridHeight = $grid.parent().closest('div');
            var gridheaderHeight = parseInt($gridHeight.css("height"), 10);
            $gridHeight.css('height', gridheaderHeight + 38);

        }
        else {
            $('#' + currentInstance.gridElementId).find('#btnUpdate').hide();
            $grid.find("table.bulkupdates").remove();

            var $gridOuterHeader = $grid.find('.pq-header-outer');
            var headerHeight = parseInt($gridOuterHeader.css("height"), 10);
            $gridOuterHeader.css('height', headerHeight - 38);

            var $gridHeight = $grid.parent().closest('div');
            var gridheaderHeight = parseInt($gridHeight.css("height"), 10);
            $gridHeight.css('height', gridheaderHeight - 38);

            $grid.find('#chkCopyRow').removeAttr('disabled');
            $grid.find('#chkCopyRow').prop('checked', false);
        }
    });

    $('#' + currentInstance.gridElementId).on('click', '#chkCopyRow', function () {

        if (this.checked) {
            currentInstance.saveRow();
            var row = $(currentInstance.gridElementIdJQ).pqGrid("selection", { type: 'row', method: 'getSelection' });
            if (row && row.length > 0) {
                var rowId = row[0].rowIndx;
                currentInstance.bulkUpdateCopiedRowData = row[0].rowData;
            }
            else {
                messageDialog.show(Common.pleaseSelectRowMsg);
                $grid.find('#chkCopyRow').prop('checked', false);
            }
        }
    });

    $('#' + currentInstance.gridElementId).find('.bucontrol').append("<button type='button' id='btnUpdate' class='btn btn-xs' style='display:none;'>Update</button>");
    $('#' + currentInstance.gridElementId).on('click', '#btnUpdate', function () {
        //Get Bulk Update elements values and store in an array
        var buValueArray = [];
        var buValue = $grid.find('[id*=bu_]');
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
        var rowid = 0;

        //Take BRG data if the current grid is BRG
        var BRGData = null;
        if (currentInstance.fullName == "BenefitsReview.BenefitReviewGrid") {
            var tenantID = currentInstance.formInstanceBuilder.tenantId;
            var formInstanceID = currentInstance.formInstanceBuilder.formInstanceId;
            var formDesignVersionID = currentInstance.formInstanceBuilder.formDesignVersionId;
            var folderVersionID = currentInstance.formInstanceBuilder.folderVersionId;

            // Get BRG Data from server for copay saved values
            var postDataForBRG = {
                tenantId: tenantID,
                formInstanceId: formInstanceID,
                formDesignVersionId: formDesignVersionID,
                folderVersionId: folderVersionID
            }
            var urlBRG = "/ExpressionBuilder/GetBenefitReviewGridData";
            var promiseForBRG = ajaxWrapper.postAsyncJSONCustom(urlBRG, postDataForBRG, false);
            promiseForBRG.done(function (result) {
                BRGData = JSON.parse(result);
            });
        }
        var filteredData = $grid.pqGrid("option", "dataModel.data");

        $.each(filteredData, function (key, value) {
            var colValueToUpdate = new Object();
            var benefitReview1 = null;
            var benefitReview2 = null;
            var benefitReview3 = null;
            var networkTier = null;
            var isINN = null;
            $.each(value, function (ind, val) {
                if (currentInstance.fullName == "BenefitsReview.BenefitReviewGrid") {
                    if (ind == "BenefitCategory1") {
                        benefitReview1 = value[ind];
                    }
                    else if (ind == "BenefitCategory2") {
                        benefitReview2 = value[ind];
                    }
                    else if (ind == "BenefitCategory3") {
                        benefitReview3 = value[ind];
                    }
                    else if (ind == "NetworkTier") {
                        networkTier = value[ind];
                    }
                    else if (ind == "IsINN") {
                        isINN = value[ind];
                    }
                    if (ind == "Coinsurance" || ind == "Copay" || ind == "Deductible") {
                        // Check AdditionalCoverageLevel is present in bulk update
                        var isAdditionalCoverageLevelForBulk = (buValueArray.AdditionalCoverageLevel != null && buValueArray.AdditionalCoverageLevel != undefined
                                                                  && buValueArray.AdditionalCoverageLevel != "");
                        // Check Covered is present in bulk update
                        var isCovered = (buValueArray.Covered != null && buValueArray.Covered != undefined && buValueArray.Covered != "");

                        // Check if AdditionalCoverageLevel and Covered is present for bulk update if not change the values with bulk update operation
                        if (isAdditionalCoverageLevelForBulk || isCovered) {
                            var rowDataInNetworkVal = BRGData.filter(function (dt) {
                                if (dt["BenefitCategory1"] == benefitReview1 && dt["BenefitCategory2"] == benefitReview2 && dt["BenefitCategory3"] == benefitReview3 && dt["IsINN"] == "Yes") {
                                    return dt;
                                }
                            });

                            var rowDataLastSavedVal = BRGData.filter(function (dt) {
                                if (dt["BenefitCategory1"] == benefitReview1 && dt["BenefitCategory2"] == benefitReview2 && dt["BenefitCategory3"] == benefitReview3 && dt["NetworkTier"] == networkTier) {
                                    return dt;
                                }
                            });
                            if (isAdditionalCoverageLevelForBulk && !isCovered) {
                                // Only AdditionalCoverageLevel is true
                                var coveredVal = value["Covered"];
                                var valueAddToEnter = buValueArray[ind];
                                if (coveredVal == "Yes" || coveredVal == "Covered at INN Benefit level") {
                                    if (buValueArray.AdditionalCoverageLevel != "") {
                                        if (buValueArray.AdditionalCoverageLevel == "N/A") {
                                            if (ind == "Coinsurance") {
                                                valueAddToEnter = rowDataLastSavedVal[0].Coinsurance;
                                            }
                                            else if (ind == "Copay") {
                                                valueAddToEnter = rowDataLastSavedVal[0].Copay;
                                            }
                                            else if (ind == "Deductible") {
                                                valueAddToEnter = rowDataLastSavedVal[0].Deductible;
                                            }
                                        }
                                        else {
                                            valueAddToEnter = "See Add'l Cov Details";
                                        }
                                    }
                                }
                                else if (coveredVal == "No" || coveredVal == "N/A") {
                                    valueAddToEnter = "N/A";
                                }
                                if (valueAddToEnter != undefined && valueAddToEnter != null) {
                                    value[ind] = valueAddToEnter;
                                    colValueToUpdate[ind] = valueAddToEnter;
                                }
                            }
                            else if (!isAdditionalCoverageLevelForBulk && isCovered) {
                                // Only Covered is true
                                if (buValueArray.Covered == "Yes") {
                                    var valueToEnter = buValueArray[ind];
                                    if (rowDataLastSavedVal != null && rowDataLastSavedVal != undefined && rowDataLastSavedVal[0] != null && rowDataLastSavedVal[0] != undefined) {
                                        if (ind == "Coinsurance") {
                                            if (buValueArray.Coinsurance == null || buValueArray.Coinsurance == undefined) {
                                                valueToEnter = rowDataLastSavedVal[0].Coinsurance;
                                            }
                                        }
                                        else if (ind == "Copay") {
                                            if (buValueArray.Copay == null || buValueArray.Copay == undefined) {
                                                valueToEnter = rowDataLastSavedVal[0].Copay;
                                            }
                                        }
                                        else if (ind == "Deductible") {
                                            if (buValueArray.Deductible == null || buValueArray.Deductible == undefined) {
                                                valueToEnter = rowDataLastSavedVal[0].Deductible;
                                            }
                                        }
                                    }
                                    if (valueToEnter != undefined && valueToEnter != null) {
                                        value[ind] = valueToEnter;
                                        colValueToUpdate[ind] = valueToEnter;
                                    }
                                }
                                else if (buValueArray.Covered == "No") {
                                    var valueToEnter = "N/A";
                                    value[ind] = valueToEnter;
                                    colValueToUpdate[ind] = valueToEnter;
                                }
                                else if (buValueArray.Covered == "Covered at INN Benefit level") {
                                    var valueToEnter = buValueArray[ind];

                                    if (rowDataInNetworkVal != null && rowDataInNetworkVal != undefined && rowDataInNetworkVal[0] != null && rowDataInNetworkVal[0] != undefined && isINN != "Yes") {
                                        if (ind == "Coinsurance") {
                                            if (buValueArray.Coinsurance == null || buValueArray.Coinsurance == undefined) {
                                                valueToEnter = rowDataInNetworkVal[0].Coinsurance;
                                            }
                                        }
                                        else if (ind == "Copay") {
                                            if (buValueArray.Copay == null || buValueArray.Copay == undefined) {
                                                valueToEnter = rowDataInNetworkVal[0].Copay;
                                            }
                                        }
                                        else if (ind == "Deductible") {
                                            if (buValueArray.Deductible == null || buValueArray.Deductible == undefined) {
                                                valueToEnter = rowDataInNetworkVal[0].Deductible;
                                            }
                                        }
                                    }
                                    if (valueToEnter != undefined && valueToEnter != null) {
                                        value[ind] = valueToEnter;
                                        colValueToUpdate[ind] = valueToEnter;
                                    }
                                }
                            }
                            else if (isAdditionalCoverageLevelForBulk && isCovered) {
                                // both are true
                                var valueToEnter = buValueArray[ind];
                                if (buValueArray.Covered == "Yes" || buValueArray.Covered == "Covered at INN Benefit level") {
                                    if (buValueArray.AdditionalCoverageLevel == "N/A") {
                                        if (ind == "Coinsurance") {
                                            valueToEnter = rowDataLastSavedVal[0].Coinsurance;
                                        }
                                        else if (ind == "Copay") {
                                            valueToEnter = rowDataLastSavedVal[0].Copay;
                                        }
                                        else if (ind == "Deductible") {
                                            valueToEnter = rowDataLastSavedVal[0].Deductible;
                                        }
                                    }
                                    else {
                                        valueToEnter = "See Add'l Cov Details";
                                    }
                                }
                                else if (buValueArray.Covered == "No" || buValueArray.Covered == "N/A") {
                                    valueToEnter = "N/A";
                                }
                                if (valueToEnter != undefined && valueToEnter != null) {
                                    value[ind] = valueToEnter;
                                    colValueToUpdate[ind] = valueToEnter;
                                }
                            }
                        }
                        else {
                            if (buValueArray[ind] != null && buValueArray[ind] != undefined) {
                                value[ind] = buValueArray[ind];
                                colValueToUpdate[ind] = buValueArray[ind];
                            }
                        }
                    }
                    else {
                        if (buValueArray[ind] != null && buValueArray[ind] != undefined) {
                            value[ind] = buValueArray[ind];
                            colValueToUpdate[ind] = buValueArray[ind];
                        }
                    }
                }
                else {
                    if (buValueArray[ind] != null && buValueArray[ind] != undefined) {
                        value[ind] = buValueArray[ind];
                        colValueToUpdate[ind] = buValueArray[ind];
                    }
                }
            });

            currentInstance.lastSelectedRow = value.RowIDProperty;
            currentInstance.expressionBuilder.addActivityLogPQ(currentInstance, colValueToUpdate, buValueArray, currentInstance.filterCondition);
            currentInstance.rowDataBeforeEdit = undefined;
            currentInstance.saveRowBulkUpdate(undefined, value);

        });

        //$.each(filteredData, function (key, value) {
        //    $grid.pqGrid("refreshRow", { rowIndx: value.RowIDProperty });
        //});

        //$grid.pqGrid("refresh");

        //Remove the additional row added for bulk updates
        $grid.find("table.bulkupdates").remove();

        var $gridOuterHeader = $grid.find('.pq-header-outer');
        var headerHeight = parseInt($gridOuterHeader.css("height"), 10);
        $gridOuterHeader.css('height', headerHeight - 38);

        var $gridHeight = $grid.parent().closest('div');
        var gridheaderHeight = parseInt($gridHeight.css("height"), 10);
        $gridHeight.css('height', gridheaderHeight - 38);

        $grid.pqGrid("refresh");

        //Hide update button
        $grid.find('#btnUpdate').hide();
        //uncheck the enable bulk update checkbox and disable
        $grid.find('#chkEnable').prop('checked', false);
        $grid.find('#chkEnable').attr('disabled', 'disabled');
        $grid.find('#chkCopyRow').prop('checked', false);
        $grid.find('#chkCopyRow').attr('disabled', 'disabled');

    });
}

repeaterBuilderPQ.prototype.getBulkUpdateControlsElementValue = function (element) {
    if (element.type == "checkbox") {
        if (element.checked) {
            return "true";
        }
        else {
            return "false";
        }
    }
    else if (element.value != '' && element.value != '[Select One]') {
        return element.value;
    }
}


repeaterBuilderPQ.prototype.getRowNumber = function () {
    var currentInstance = this;
    return Repeater.ROWNUMBER;
}

repeaterBuilderPQ.prototype.formatBulkUpdateControlsDropdownTextbox = function (elementDesign, colid) {
    var currentInstance = this;
    var $grid = $(currentInstance.gridElementIdJQ);
    var idpluscol = $(elementDesign).find('select').attr('id');
    var ddttextboxID = $grid.find('[id=bu_' + colid + 'Textbox]');

    var buValue = $grid.find('[id=bu_' + colid + ']');

    $(buValue).on('change', function () {
        if ($(this).val() == 'newItem') {
            $(this).val('');
            $(this).parent().find('.ddt-textbox').toggle().focus();
            $(this).toggle();
        }
    });

    $(ddttextboxID).on("focusout", function (e) {
        $(buValue).toggle(); //$(this).parent().find($("#" + idpluscol)).toggle();
        $(this).toggle();
    });

    $(ddttextboxID).bind("keyup", function (e) {
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

repeaterBuilderPQ.prototype.addFooterButtonToGrid = function (table, row, td, cell) {
    var currentInstance = this;

    if (currentInstance.expressionBuilder.hasExpressionBuilderRule(currentInstance.formInstanceBuilder.formDesignId)) {
        if (!currentInstance.expressionBuilder.hideAddButtonforRepeater(currentInstance.fullName)) {
            currentInstance.addPlusButton(table, row, td, cell);
        }
    }
    else {
        currentInstance.addPlusButton(table, row, td, cell);
    }

    if (currentInstance.hasRichTextBox) {
        currentInstance.addRichTextBox(table, row, td, cell);
    }

    if (currentInstance.expressionBuilder.hasExpressionBuilderRule(currentInstance.formInstanceBuilder.formDesignId)) {
        if (!currentInstance.expressionBuilder.hideMinusButtonforRepeater(currentInstance.fullName)) {
            currentInstance.addMinusButton(table, row, td, cell);
        }
    }
    else {
        currentInstance.addMinusButton(table, row, td, cell);
    }

    if (currentInstance.fullName != currentInstance.ProofingRepeaterName) {
        if (currentInstance.expressionBuilder.hasExpressionBuilderRule(currentInstance.formInstanceBuilder.formDesignId)) {
            if (!currentInstance.expressionBuilder.hideCopyButtonforRepeater(currentInstance.fullName)) {
                currentInstance.addCopyButton(table, row, td, cell);
            }
        }
        else {
            currentInstance.addCopyButton(table, row, td, cell);
        }
    }

    if (currentInstance.fullName == currentInstance.ProofingRepeaterName) {
        if (currentInstance.expressionBuilder.hasExpressionBuilderRule(currentInstance.formInstanceBuilder.formDesignId)) {
            if (!currentInstance.expressionBuilder.hideCopyButtonforRepeater(currentInstance.fullName)) {
                currentInstance.addEditButton(table, row, td, cell);
            }
        }
        else {
            currentInstance.addEditButton(table, row, td, cell);
        }
    }
}

repeaterBuilderPQ.prototype.addChildElement = function (row, idx, defaultValue, mappElement) {

    var colName = this.columnModels[idx].title.replace(/[^\w\s]/gi, '').replace(/[^\w\s]/gi, '').replace(/ /g, '');
    var dataSourceName = this.design.ChildDataSources[0].DataSourceName;
    for (var l = 0; l < this.design.ChildDataSources[0].GroupHeader.length; l++) {
        var rowData = this.design.ChildDataSources[0].GroupHeader[l];
        if (row[dataSourceName] == undefined) {
            var newRow = {};
            row[dataSourceName] = [];

            if (this.columnModels[idx].hidden == true) {
                newRow[colName] = rowData[mappElement];
            }
            else {
                newRow[colName] = defaultValue;
            }

            row[dataSourceName].push(newRow);
        }
        else {
            if (row[dataSourceName][l] == undefined) {

                var newRow = {};

                if (this.columnModels[idx].hidden == true) {
                    newRow[colName] = rowData[mappElement];
                }
                else {
                    newRow[colName] = defaultValue;
                }

                row[dataSourceName].push(newRow);
            }
            else {
                if (this.columnModels[idx].hidden == true) {
                    row[dataSourceName][l][colName] = rowData[mappElement];
                }
                else {
                    row[dataSourceName][l][colName] = defaultValue;
                }
            }
        }
    }
}

repeaterBuilderPQ.prototype.addEditButton = function (table, row, td, cell) {
    var currentInstance = this;

    td = document.createElement("TD");
    cell = document.createElement("SPAN");
    cell.className = "ui-icon ui-icon-pencil";
    cell.onclick = function () {
        var row = $(currentInstance.gridElementIdJQ).pqGrid("selection", { type: 'row', method: 'getSelection' });
        if (row && row.length > 0 && row[0].rowData != undefined) {
            var rowId = row[0].rowData.RowIDProperty;
            var round = row[0].rowData.Round;
            if (rowId !== undefined && rowId !== null && round != "") {
                currentInstance.expressionBuilder.OpenFileUploadDialog(currentInstance, 'EDIT', row[0].rowData);
            }
            else {
                messageDialog.show(Common.pleaseSelectRowMsg);
            }
        }
        else {
            messageDialog.show(Common.pleaseSelectRowMsg);
        }
    }
    if (currentInstance.fullName == currentInstance.ProofingRepeaterName) {
        if (currentInstance.formInstanceBuilder.folderData.RoleId == "23") {
            cell.onclick = null;
        }
    }
    td.appendChild(cell);
    row.appendChild(td);
}

repeaterBuilderPQ.prototype.addPlusButton = function (table, row, td, cell) {
    var currentInstance = this;

    cell.className = "ui-icon ui-icon-plus";
    cell.title = 'Add new empty row';
    cell.onclick = function () {
        var primaryDataSources = currentInstance.design.PrimaryDataSource;
        if (currentInstance.design.PrimaryDataSource == null && currentInstance.gridType != 'child') {
            //if (currentInstance.expressionBuilder.hasExpressionBuilderRule(currentInstance.formInstanceBuilder.formDesignId) && (currentInstance.fullName == "CostShare.CoverageLevel.CoverageLevelList")) {
            //    var addRow = currentInstance.expressionBuilder.handleBlankRowForCoverageLevel(currentInstance);
            //    if (addRow)
            //        currentInstance.addNewRow();
            //    else
            //        messageDialog.show(Common.blankCoverageLevel);
            //} else {
            //    currentInstance.addNewRow();
            //}
            if (currentInstance.fullName == currentInstance.ProofingRepeaterName)
                currentInstance.expressionBuilder.OpenFileUploadDialog(currentInstance, 'ADD');
            else
                currentInstance.addNewRow();

            if (currentInstance.hasRichTextBox) {
                currentInstance.richTextBoxPopup();
            }
        }
    };
    if (currentInstance.fullName == currentInstance.ProofingRepeaterName) {
        if (currentInstance.formInstanceBuilder.folderData.RoleId == "23") {
            cell.onclick = null;
        }
    }
    td.appendChild(cell);
    row.appendChild(td);
}

repeaterBuilderPQ.prototype.addMinusButton = function (table, row, td, cell) {
    var currentInstance = this;

    td = document.createElement("TD");
    cell = document.createElement("SPAN");
    cell.className = "ui-icon ui-icon-minus";
    cell.title = 'Remove row';
    cell.onclick = function () {
        if (currentInstance.fullName == currentInstance.ProofingRepeaterName)
            currentInstance.expressionBuilder.DeleteProofingDocument(currentInstance);
        else {
            var row = $(currentInstance.gridElementIdJQ).pqGrid("selection", { type: 'row', method: 'getSelection' });
            if (row && row.length > 0 && row[0].rowData != undefined) {
                var rowId = row[0].rowData.RowIDProperty;
                if (rowId !== undefined && rowId !== null) {
                    currentInstance.deleteRow(rowId);
                    currentInstance.rowDataBeforeEdit = undefined;
                }
                else {
                    messageDialog.show(Common.pleaseSelectRowMsg);
                }
            }
            else {
                messageDialog.show(Common.pleaseSelectRowMsg);
            }
        }
    }
    if (currentInstance.fullName == currentInstance.ProofingRepeaterName) {
        if (currentInstance.formInstanceBuilder.folderData.RoleId == "23") {
            cell.onclick = null;
        }
    }
    td.appendChild(cell);
    row.appendChild(td);
}

repeaterBuilderPQ.prototype.addCopyButton = function (table, row, td, cell) {
    var currentInstance = this;

    td = document.createElement("TD");
    cell = document.createElement("SPAN");
    cell.className = "ui-icon ui-icon-copy";
    cell.title = 'Copy row';
    cell.onclick = function () {
        var row = $(currentInstance.gridElementIdJQ).pqGrid("selection", { type: 'row', method: 'getSelection' });
        if (row && row.length > 0) {
            var rowId = row[0].rowIndx;
            if (rowId !== undefined && rowId !== null) {
                currentInstance.copyRow(rowId);
            }
            else {
                messageDialog.show(Common.pleaseSelectRowMsg);
            }
        }
        else {
            messageDialog.show(Common.pleaseSelectRowMsg);
        }
    }

    td.appendChild(cell);
    row.appendChild(td);
}

repeaterBuilderPQ.prototype.addRichTextBox = function (table, row, td, cell) {
    var currentInstance = this;

    td = document.createElement("TD");
    cell = document.createElement("SPAN");
    cell.className = "ui-button-icon-primary ui-icon ui-icon-pencil";
    cell.title = 'Edit';
    cell.onclick = function () {
        currentInstance.richTextBoxPopup();
    }
    td.appendChild(cell);
    row.appendChild(td);
}

repeaterBuilderPQ.prototype.richTextBoxPopup = function () {
    var currentInstance = this;
    ajaxDialog.showPleaseWait();
    setTimeout(function () {
        var row = $(currentInstance.gridElementIdJQ).pqGrid("selection", { type: 'row', method: 'getSelection' });

        if (row && row.length > 0 && row[0].rowData != undefined) {

            var rowId = row[0].rowData.RowIDProperty;
            currentInstance.lastSelectedRow = rowId;

            if (rowId !== undefined && rowId !== null) {
                var isRowReadOnly = currentInstance.design.PrimaryDataSource == null ? false : true;
                if (currentInstance.fullName == "ANOCChartPlanDetails.ANOCBenefitsCompare")
                    currentInstance.viewRowDataPQ(rowId, false, true);
                else
                    currentInstance.viewRowDataPQ(rowId, isRowReadOnly, true);
            }
            else {
                messageDialog.show(Common.pleaseSelectRowMsg);
            }
        }
        else {
            messageDialog.show(Common.pleaseSelectRowMsg);
        }
        ajaxDialog.hidePleaseWait();
    }, 100);
}

repeaterBuilderPQ.prototype.getDefualtRow = function () {
    try {
        var row = {};
        for (var idx = 0; idx < this.columnModels.length; idx++) {
            if (this.columnModels[idx].editor != undefined) {
                var defaultValue = "";
                if (this.columnModels[idx].dataIndx.substring(0, 4) == 'INL_') {
                    var mappElement = this.columnModels[idx].dataIndx.split('_')[3];
                    this.addChildElement(row, idx, defaultValue, mappElement);
                }
                else {
                    if (this.columnModels[idx].editor.defaultValue != null && this.columnModels[idx].editor.defaultValue != undefined) {
                        defaultValue = this.columnModels[idx].editor.defaultValue;
                    }
                    row[this.columnModels[idx].dataIndx] = defaultValue;
                }
            }
            else {
                row[this.columnModels[idx].dataIndx] = "";
            }
        }

        if (this.design.ChildDataSources != null && Array.isArray(this.design.ChildDataSources) && this.design.ChildDataSources.length > 0 && this.design.ChildDataSources[0].DisplayMode == "Child") {
            var childDatasource = this.design.ChildDataSources[0];
            row[childDatasource.DataSourceName] = [];

            var childRow = {};
            $.each(childDatasource.Mappings, function (idx, elm) {
                childRow[elm.TargetElement] = "";
                // row[childDatasource.DataSourceName][idx][elm.TargetElement] = "";
            })

            row[childDatasource.DataSourceName] = childRow;
        }

        return row;
    }
    catch (err) {

    }
}

repeaterBuilderPQ.prototype.getPQRowData = function () {
    var currentInstance = this;

    //Fix for ANT-704, when user filter the data then only filter data is fetched but we want all the data from the repeater grid
    var dataMData = $(currentInstance.gridElementIdJQ).pqGrid("option", "dataModel.data");
    var dataUFMData = $(currentInstance.gridElementIdJQ).pqGrid("option", "dataModel.dataUF");

    // Copying the filter rows and not filter rows (hidden) into new array
    var gridPQData = dataMData.slice();
    gridPQData.push.apply(gridPQData, dataUFMData);

    var rowData = gridPQData.filter(function (dt) {
        return dt.RowIDProperty == currentInstance.lastSelectedRow;
    })

    return rowData[0];
}

repeaterBuilderPQ.prototype.getRepeaterMatchIndex = function (repeaterData, rowIDProperty) {
    var currentInstance = this.currentInstance;
    var dataSource = this.dataSource;
    var matchRowIndex = -1;
    if (rowIDProperty != undefined) {
        $.each(repeaterData, function (idx, val) {
            if (val.RowIDProperty == rowIDProperty) {
                matchRowIndex = idx;
            }
        });
    }
    return matchRowIndex;
}

repeaterBuilderPQ.prototype.getFileDownloadFormatter = function (cellvalue, rowObject) {
    if (cellvalue != "") {
        if (rowObject.ProofingDocument == "PBP Template") {
            cellvalue = "<a class='ASSIGNMENT' href='#'><span title='Download Attachement' id='downloadProofingDoc' class='downloadProofingattachment' data-Id='" + rowObject.ClientDocumentPath + "'><u>" + cellvalue + "</u></span></a>";
        }
        else {
            cellvalue = "<a class='ASSIGNMENT' href='#'><span title='Download Attachement' id='downloadProofingDoc' class='downloadProofingattachment' data-Id='" + rowObject.ServerDocumentPath + "'><u>" + cellvalue + "</u></span></a>";
        }

        return cellvalue;
    }
}

repeaterBuilderPQ.prototype.fileDownloadUnFormat = function (cellvalue, options, cell) {
    return cellvalue;
}
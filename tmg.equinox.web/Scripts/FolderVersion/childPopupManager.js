var childPopupManager = function (formInstanceBuilder, hasDisabledParent) {
    this.uiElementID = "";
    this.uiElementDBID = "";
    //Caption for the dialog box.
    this.caption = "Select Data";
    this.mappedColumnList = [];
    this.formInstanceBuilder = formInstanceBuilder;
    this.sourceDataList = [];
    this.URLs = {
        getSourceData: "/ExpressionBuilder/RunChildPopup"
    };
    this.elementIDs = {
        childPopupDialogJQ: '#childDataMappingDialog',
        childSourceDataJQ: '#ChildDataSourceData',
        manualGridJQ: '#manualGrid',
        manualGrid: 'manualGrid',
        childMessageDialogJQ: '#childDataMessageDialog',
        childDataDialogJQ: '#childDataDialog',
        childDialogButtonJQ: '#childDataDialogBtn',
        childDialogSaveButtonJQ: "#childDataSourceSaveBtn",
        childDialogCancelButtonJQ: "#childDataSourceCancelBtn",
        viewAllCheckBoxJQ: '#viewAllCheckBox',
        childDialogAllViewCheckbox: 'input[id^=checkbox_]'
    };
    this.isEditable = formInstanceBuilder.folderData.isEditable;
    this.hasDisabledParent = this.formInstanceBuilder.isfolderReleased;
    this.isChildPopup = true;
}
childPopupManager.prototype.initializeDialog = function () {
    $(this.elementIDs.childPopupDialogJQ).dialog({
        autoOpen: false,
        resizable: false,
        closeOnEscape: true,
        height: 'auto',
        width: 600,
        height: 415,
        modal: true,
        position: ['middle', 100],
    });
}

childPopupManager.prototype.init = function (event) {
    var currentInstance = this;
    var parent = $($(event)[0].target).siblings();
    currentInstance.uiElementID = parent[0].id;
    currentInstance.uiElementDBID = currentInstance.getTargetElementDBId(currentInstance.uiElementID);
    this.hasDisabledParent = $("#" + currentInstance.uiElementID).prop("disabled");
    currentInstance.loadData();
}

childPopupManager.prototype.loadData = function () {
    var currentInstance = this;
    var sourceMappingData = undefined;
    currentInstance.initializeDialog();
    currentInstance.getSourceData();
    $(currentInstance.elementIDs.childDialogSaveButtonJQ).off("click");
    $(currentInstance.elementIDs.childDialogSaveButtonJQ).on("click", function () {
        $(currentInstance.elementIDs.childPopupDialogJQ).dialog("close");
        ajaxDialog.showPleaseWait();
        setTimeout(function () {
            currentInstance.setTargetData();
            ajaxDialog.hidePleaseWait();
        }, 100);
    });

    $(currentInstance.elementIDs.childDialogCancelButtonJQ).off("click");
    $(currentInstance.elementIDs.childDialogCancelButtonJQ).on("click", function () {
        $(currentInstance.elementIDs.manualGridJQ).empty();
        $(currentInstance.elementIDs.childPopupDialogJQ).dialog("close");
    });

    if (currentInstance.isEditable == "False" || currentInstance.hasDisabledParent == true) {
        $(currentInstance.elementIDs.childDialogSaveButtonJQ).attr('disabled', 'disabled');
    }
    else {
        if (vbRole == Role.ProductSME && currentInstance.formInstanceBuilder.formInstanceId == currentInstance.formInstanceBuilder.anchorFormInstanceID) {
            $(currentInstance.elementIDs.childDialogSaveButtonJQ).attr('disabled', 'disabled');
        }
        else {
            $(currentInstance.elementIDs.childDialogSaveButtonJQ).removeAttr('Disabled', 'disabled');
        }
    }
}

childPopupManager.prototype.getSourceData = function () {
    var currentInstance = this;
    var promise = undefined;
    ajaxDialog.showPleaseWait();
    //To run custom datasources using expression builder
    promise = currentInstance.runExpressionRule(currentInstance.formInstanceBuilder.tenantId, currentInstance.formInstanceBuilder.folderVersionId, currentInstance.formInstanceBuilder.formInstanceId, currentInstance.formInstanceBuilder.formDesignVersionId, currentInstance.uiElementDBID);
    if (promise != undefined) {
        promise.done(function (data) {
            ajaxDialog.hidePleaseWait();
            currentInstance.sourceDataList = JSON.parse(data);
            currentInstance.setRepeaterData(currentInstance.sourceDataList);
        });
        promise.fail(currentInstance.showError);
    }
}

childPopupManager.prototype.showError = function (xhr) {
    if (xhr.status == 999)
        window.location.href = '/Account/LogOn';
    else
        messageDialog.show(Common.errorMsg);
}

childPopupManager.prototype.buildRepeater = function (sourceServicesName, sourceMappingData) {
    var currentInstance = this;
    $(currentInstance.elementIDs.childSourceDataJQ).append("<table id='" + currentInstance.elementIDs.manualGrid + "'> </table>");
    currentInstance.loadGrid(sourceMappingData, sourceServicesName);
    $(currentInstance.elementIDs.viewAllCheckBoxJQ).click(function (e) {
        //$(currentInstance.elementIDs.manualGridJQ).find(".check-all").parent(".ui-jqgrid-sortable").removeClass("ui-jqgrid-sortable");
        $(".check-all").parent(".ui-jqgrid-sortable").removeClass("ui-jqgrid-sortable");
        var val = $(currentInstance.elementIDs.viewAllCheckBoxJQ).is(':checked');
        if (val) {
            $(currentInstance.elementIDs.viewAllCheckBoxJQ).attr('checked', 'checked');
            $(currentInstance.elementIDs.childDialogAllViewCheckbox).filter(':not(:disabled)').prop('checked', true);
            currentInstance.applyCheckAll("Yes");
        }
        else {
            $(currentInstance.elementIDs.viewAllCheckBoxJQ).removeAttr('checked');
            $(currentInstance.elementIDs.childDialogAllViewCheckbox).filter(':not(:disabled)').prop('checked', false);
            currentInstance.applyCheckAll("No");
        }
    });
}

childPopupManager.prototype.applyCheckAll = function (value) {
    var currentInstance = this;
    var gridData = $(currentInstance.elementIDs.manualGridJQ).jqGrid('getGridParam', 'data');
    if ($(currentInstance.elementIDs.manualGridJQ).getGridParam("postData").filters != undefined) {
        var objFilterCriteria = JSON.parse($(currentInstance.elementIDs.manualGridJQ).getGridParam("postData").filters);

        $.each(objFilterCriteria.rules, function (ind, val) {
            gridData = $.grep(gridData, function (a) {
                return (a[val.field].toLowerCase().indexOf(val.data.toLowerCase()) >= 0);
            });
        });
    }
    $.each(gridData, function (idx, row) {
        row.IsSelect = value;
        $(currentInstance.elementIDs.manualGridJQ).jqGrid('setRowData', value.RowIDProperty, value);
    });
}

childPopupManager.prototype.loadGrid = function (sourceData, sourceServicesName) {
    var currentInstance = this;
    var colArray = currentInstance.addColNames(currentInstance.mappedColumnList);
    var colModel = currentInstance.addColModel(currentInstance.mappedColumnList);
    if (currentInstance.isChildPopup) {
        var mappedElementData = [];
        for (var j = 0; j < sourceData.length; j++) {
            var rowData = {};
            rowData[currentInstance.mappedColumnList[0]] = sourceData[j][currentInstance.mappedColumnList[0]];
            rowData["IsSelect"] = "";
            rowData["ID"] = j + 1;
            mappedElementData.push(rowData);
        }
        currentInstance.setSelectedData(mappedElementData);
    }
    else {
        mappedElementData = sourceData;
    }
    var lastgridsel;
    currentInstance.sourceDataList = sourceData;
    $(currentInstance.elementIDs.manualGridJQ).empty();
    //clean up the grid first - only table element remains after this
    $(currentInstance.elementIDs.manualGridJQ).jqGrid('GridUnload');
    //adding the pager element
    $(currentInstance.elementIDs.manualGridJQ).parent().append("<div id='p" + currentInstance.elementIDs.manualGrid + "'></div>");
    $(currentInstance.elementIDs.manualGridJQ).jqGrid({
        datatype: 'local',
        data: mappedElementData,
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: sourceServicesName,
        height: '150',
        rowNum: 25,
        multiSort: true,
        ignoreCase: true,
        loadonce: true,
        autowidth: true,
        shrinkToFit: false,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        //  sortname: currentInstance.mappedColumnList[0],
        pager: '#p' + currentInstance.elementIDs.manualGrid,
        altclass: 'alternate',
        gridComplete: function () {
            var width = $($(this).parent().closest('div').first()).width();
            $(currentInstance.elementIDs.manualGridJQ).jqGrid().setGridWidth(width, false);
            var columnCounter = 0;
            $.each(colModel, function (colId, colProp) {
                if (colProp.hidden === false) {
                    columnCounter++;
                }
            });
            if (width > (150 * columnCounter)) {
                var totalColumnWidth = $(currentInstance.elementIDs.manualGridJQ).jqGrid().width();
                var gridWidth = $(currentInstance.elementIDs.manualGridJQ).getGridParam("width");

                if (totalColumnWidth != 0 && totalColumnWidth < gridWidth && (gridWidth - totalColumnWidth) > 20) {
                    $(currentInstance.elementIDs.manualGridJQ).setGridWidth((gridWidth - 20), true);
                }
            }
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

}

childPopupManager.prototype.checkBoxFormatter = function (cellValue, options, rowObject) {
    var currentInstance = this;
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

childPopupManager.prototype.uncheckBoxFormatter = function (cellvalue, options, cell) {
    var result;
    result = $(this).find('#' + options.rowId).find('input').prop('checked');

    if (result == true || result == "true")
        return 'Yes';
    else
        return 'No';
}
childPopupManager.prototype.addColNames = function (mappedCoumnList) {
    var currentInstance = this;
    var colNames = [];
    if (currentInstance.isChildPopup) {
        var viewAllCheckbox = "Select All";
        if (currentInstance.isEditable == "False" || currentInstance.hasDisabledParent == true) {
            viewAllCheckbox = '<input type="checkbox" class="check-all" style="position:relative;left:30px;" id="viewAllCheckBox" disabled="disabled"/>';
        }
        else {
            viewAllCheckbox = '<input type="checkbox" class="check-all" style="position:relative;left:30px;" id="viewAllCheckBox"/>';
        }
        colNames.push(viewAllCheckbox);
    }
    colNames.push('ID');
    for (var i = 0; i < mappedCoumnList.length; i++) {
        colNames.push(mappedCoumnList[i].toString().replace(/([A-Z])/g, ' $1').trim());
    }
    return colNames;
}

childPopupManager.prototype.addColModel = function (mappedCoumnList) {
    var currentInstance = this;
    var colModel = [];
    if (currentInstance.isChildPopup) {
        colModel.push({
            name: 'IsSelect', align: 'center', width: 30, sortable: true, editable: false, edittype: "checkbox", editoptions: { value: "true:false", defaultValue: "false" },
            formatter: function (cellValue, options, rowObject) {
                if (cellValue == "No")
                    if (currentInstance.isEditable == "False" || currentInstance.hasDisabledParent == true) {
                        return "<input type='checkbox' style='position:relative;' class='staticData' id='checkbox_" + options.rowId + "' name= 'checkbox_" + options.gid + "' disabled='disabled'/>";
                    }
                    else {
                        return "<input type='checkbox' style='position:relative;' class='staticData' id='checkbox_" + options.rowId + "' name= 'checkbox_" + options.gid + "' />";
                    }
                else
                    if (currentInstance.isEditable == "False" || currentInstance.hasDisabledParent == true) {
                        return "<input type='checkbox' style='position:relative;' class='staticData' id='checkbox_" + options.rowId + "' name= 'checkbox_" + options.gid + "'  checked=checked disabled='disabled'/>";
                    }
                    else {
                        return "<input type='checkbox' style='position:relative;' class='staticData' id='checkbox_" + options.rowId + "' name= 'checkbox_" + options.gid + "'  checked=checked/>";
                    }
            },
            unformat: function (cellvalue, options, cell) {
                var result;
                result = $(this).find('#' + options.rowId).find('input').prop('checked');

                if (result == true || result == "true")
                    return 'Yes';
                else
                    return 'No';
            },
            sorttype: function (value, item) {
                return item.IsSelect == "Yes" ? true : false;
            }
        });
    }
    colModel.push({ name: 'ID', align: 'center', width: 80, sortable: false, key: true, hidden: true });
    for (var i = 0; i < mappedCoumnList.length; i++) {
        var colMod = { name: mappedCoumnList[i], index: mappedCoumnList[i], editable: false, sortable: false };
        colModel.push(colMod);
    }
    return colModel;
}

childPopupManager.prototype.setTargetData = function () {
    var currentInstance = this; var newValue = "";
    var selectedDataList = new Array();
    var allRowData = $(currentInstance.elementIDs.manualGridJQ).jqGrid('getGridParam', 'data');
    var k = 0;
    for (var i = 0 ; i < allRowData.length; i++) {
        if (allRowData[i].IsSelect == "Yes" && currentInstance.sourceDataList[i] != undefined) {
            selectedDataList[k] = currentInstance.sourceDataList[i];
            k++;
        }
    }
    if (k == 0) {
        //messageDialog.show(Common.pleaseSelectRowMsg);
        //return;
    }
    var length = selectedDataList.length;
    $.each(selectedDataList, function (idx, val) {
        if (idx == length - 1) {
            newValue += val[currentInstance.mappedColumnList[0]];
        }
        else {
            newValue += val[currentInstance.mappedColumnList[0]] + ",";
        }
    });
    $("#" + currentInstance.uiElementID).val(newValue);
    $("#" + currentInstance.uiElementID).trigger('change');
    $(currentInstance.elementIDs.manualGridJQ).empty();
}

childPopupManager.prototype.setSelectedData = function (gridData) {
    var currentInstance = this;
    var elementData = $("#" + currentInstance.uiElementID).val();
    var elementDataArray = [];
    try {
        if (elementData !== undefined && elementData !== null) {
            elementDataArray = elementData.split(',');
        }
        $.each(gridData, function (idx, data) {
            var index = elementDataArray.indexOf(data[currentInstance.mappedColumnList[0]])
            if (index > -1) {
                data.IsSelect = "Yes";
            }
            else {
                data.IsSelect = "No";
            }
        });
    }
    catch (e) {
    }
}

childPopupManager.prototype.setRepeaterData = function (sourceDataList) {
    var currentInstance = this;
    var dialogTitle = "Select Source Data";
    $(currentInstance.elementIDs.childPopupDialogJQ).dialog('option', 'title', dialogTitle);
    $(currentInstance.elementIDs.childPopupDialogJQ).dialog("open");
    if (null != sourceDataList && sourceDataList.length > 0 && "null" != sourceDataList && undefined != sourceDataList) {
        currentInstance.mappedColumnList = Object.keys(sourceDataList[0]);
        if (currentInstance.mappedColumnList != undefined && currentInstance.mappedColumnList.length > 1) {
            currentInstance.isChildPopup = false;
        }
        else {
            currentInstance.isChildPopup = true;
        }
        currentInstance.buildRepeater("Source Data", currentInstance.sourceDataList);
        if (currentInstance.isChildPopup) {
            $(currentInstance.elementIDs.childDialogSaveButtonJQ).removeClass('hide');
            $(currentInstance.elementIDs.childDialogCancelButtonJQ).removeClass('hide');

        }
        else {
            // Increse dialog width and position to center in case of non childpopup controls.
            $(currentInstance.elementIDs.childPopupDialogJQ).dialog("option", "width", 750);
            $(currentInstance.elementIDs.childPopupDialogJQ).dialog("option", "position", "center");
            $(currentInstance.elementIDs.childDialogSaveButtonJQ).addClass('hide');
            $(currentInstance.elementIDs.childDialogCancelButtonJQ).addClass('hide');
        }
        $(currentInstance.elementIDs.childMessageDialogJQ).addClass('hide');
        $(currentInstance.elementIDs.childDialogButtonJQ).removeClass('hide');
        $(".panel-body").parent(currentInstance.elementIDs.childDataDialogJQ).removeClass('hide');
    }
    else {
        $(currentInstance.elementIDs.childMessageDialogJQ).removeClass('hide');
        $(currentInstance.elementIDs.childDialogButtonJQ).addClass('hide');
        $(".panel-body").parent(currentInstance.elementIDs.childDataDialogJQ).addClass('hide');
    }
}

childPopupManager.prototype.runExpressionRule = function (tenantId, folderVersionId, formInstanceId, formDesignVersionId, targetElementId) {
    var currentInstance = this;
    var elementPath = $('#' + currentInstance.uiElementID)[0].getAttribute('data-journal');
    var elementValue = $('#' + currentInstance.uiElementID).val();
    var elementType = $('#' + currentInstance.uiElementID)[0].type;

    var dataToPost = { tenantId: tenantId, folderVersionId: folderVersionId, formInstanceId: formInstanceId, formDesignVersionId: formDesignVersionId, targetElementId: targetElementId, targetElementPath: elementPath, targetValue: elementValue, uiElementType: elementType, uielementName : currentInstance.uiElementID };
    var promise = ajaxWrapper.postJSON(currentInstance.URLs.getSourceData, dataToPost);

    return promise;
}

childPopupManager.prototype.getTargetElementDBId = function (uiElementId) {
    var currentInstance = this;
    var frmIdIndx = uiElementId.indexOf(currentInstance.formInstanceBuilder.formInstanceId);//
    var item = uiElementId.substr(0, frmIdIndx);
    item = item.replace(/[a-zA-Z ]/g, "|");
    item = item.split('|');
    var id = item[item.length - 1];
    return id;
}


var replaceTextDialog = function () {
    var isInitialized = false;
    var objFolderInstance;
    var objFormInstance;
    var elementList = [];

    var messages = {
        replaceTextRequired: 'Replace with text is required.',
        withInRequired: 'With-in is required.',
        saveChanges: 'Please save your changes first to peform replace otherwise your changes will be lost.',
        notSupported: 'This feature is only supported for Mutiple Instances Master List.',
        errorWhileProcessing: 'There is an error while processing.',
        invalidCharacters: 'There are some invalid characters.',
        findEqualsReplace: 'Find and Replace text cannot be the same.',
        selectInstance: 'Please atleast one instance for processing.',
    };

    var URLs = {
        replaceText: '/ExpressionBuilder/ReplaceText',
        getLookIn: '/ExpressionBuilder/GetLookInList?formDesignVersionId={formDesignVersionId}',
        getInstances: '/ExpressionBuilder/GetInstances?folderVersionID={folderVersionID}',
    };

    var elementIDs = {
        replaceDialog: 'dvReplaceText',
        replaceDialogJQ: '#dvReplaceText',
        txtFindTextJQ: '#txrReplaceWhat',
        dvFindTextErrorJQ: '#dvFindTextError',
        txtReplaceWithJQ: '#txtReplaceWith',
        drpWithinJQ: '#drpWithin',
        dvWithinErrorJQ: '#dvWithinError',
        dvReplaceWithErrorJQ: '#dvReplaceWithError',
        drpLookInJQ: '#drpLookIn',
        chkMatchJQ: '#chkMatch',
        btnReplaceJQ: '#btnReplace',
        btnCancelJQ: '#btnCancel',
        dropdownLoaderJQ: '#dropLookInLoader',
        divInstancesJQ: '#divInstances',
        drpInstancesJQ: '#drpInstances',
        divInstancesErrorJQ: '#divInstancesError',
    };

    function init() {
        var currentInstance = this;
        $(document).ready(function () {
            if (isInitialized == false) {
                $(elementIDs.replaceDialogJQ).dialog({ autoOpen: false, width: 600, height: 400, modal: true });
                isInitialized = true;

                $(elementIDs.btnReplaceJQ).unbind("click");
                $(elementIDs.btnReplaceJQ).click(function () {
                    replaceText();
                });

                $(elementIDs.btnCancelJQ).unbind("click");
                $(elementIDs.btnCancelJQ).click(function () {
                    clearAll();
                    $(elementIDs.replaceDialogJQ).dialog('close');
                    return false;
                });

                $(elementIDs.drpLookInJQ).on('mousedown', function () {
                    fillLookInDropdown();
                });

                $(elementIDs.drpWithinJQ).on('change', function () {
                    if (this.value == 'Folder') {
                        fillInstances();
                        $(elementIDs.divInstancesJQ).show();
                    } else {
                        $(elementIDs.divInstancesJQ).hide()
                        $(elementIDs.drpInstancesJQ).empty();
                    }
                });
            }
        });
    }
    init();

    function fillLookInDropdown() {
        if (elementList[objFormInstance.formDesignVersionId] == null || elementList[objFormInstance.formDesignVersionId] == undefined) {
            $(elementIDs.drpLookInJQ).empty();
            $(elementIDs.drpLookInJQ).append("<option value=''>" + "Select One" + "</option>");

            var promise = ajaxWrapper.getJSON(URLs.getLookIn.replace("{formDesignVersionId}", objFormInstance.formDesignVersionId));
            promise.done(function (list) {
                elementList[objFormInstance.formDesignVersionId] = list;
                for (i = 0; i < list.length; i++) {
                    $(elementIDs.drpLookInJQ).append("<option value=" + list[i].GeneratedName + ">" + list[i].Label + "</option>");
                }
                $(elementIDs.dropdownLoaderJQ).hide();
            });

            promise.fail(showError);
        }
    }

    function fillInstances() {
        $(elementIDs.drpInstancesJQ).empty();
        if (objFormInstance.folderVersionId != null || objFormInstance.folderVersionId != undefined) {
            var promise = ajaxWrapper.getJSON(URLs.getInstances.replace("{folderVersionID}", objFormInstance.folderVersionId));
            promise.done(function (list) {
                for (i = 0; i < list.length; i++) {
                    $(elementIDs.drpInstancesJQ).append("<option value=" + list[i].FormInstanceID + " selected=true>" + list[i].Name + "</option>");
                }
                $(elementIDs.dropdownLoaderJQ).hide();
            });

            promise.fail(showError);
        }
    }

    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }


    //validate controls.
    function validate() {
        var isValid = true;

        $(elementIDs.dvFindTextErrorJQ).empty();
        var findWith = $(elementIDs.txtFindTextJQ).val();
		var rx = /^[a-z\d\-_\s\{\}\[\]\:]+$/i;
        if (findWith.trim() == '') {
            var $span = $('<small/>').addClass('help-block').appendTo(elementIDs.dvFindTextErrorJQ);
            $span.html(messages.replaceTextRequired);
            isValid = false;
        }
        else if (!rx.test(findWith)) {
            var $span = $('<small/>').addClass('help-block').appendTo(elementIDs.dvFindTextErrorJQ);
            $span.html(messages.invalidCharacters);
            isValid = false;
        }

        var replaceWith = $(elementIDs.txtReplaceWithJQ).val();
        if (replaceWith.trim() != "") {
            if (!rx.test(replaceWith)) {
                var $span = $('<small/>').addClass('help-block').appendTo(elementIDs.dvReplaceWithErrorJQ);
                $span.html(messages.invalidCharacters);
                isValid = false;
            }
        }

        if (findWith != "" && replaceWith != "") {
            if (findWith === replaceWith) {
                var $span = $('<small/>').addClass('help-block').appendTo(elementIDs.dvReplaceWithErrorJQ);
                $span.html(messages.findEqualsReplace);
                isValid = false;
            }
        }

        $(elementIDs.dvWithinErrorJQ).empty();
        var withIn = $(elementIDs.drpWithinJQ).val();
        if (withIn == '' || withIn == 'Select One') {
            var $span = $('<small/>').addClass('help-block').appendTo(elementIDs.dvWithinErrorJQ);
            $span.html(messages.withInRequired);
            isValid = false;
        }

        if ($(elementIDs.drpInstancesJQ).children().length > 0 && $(elementIDs.drpInstancesJQ).children("option:selected").length <= 0) {
            var $span = $('<small/>').addClass('help-block').appendTo(elementIDs.divInstancesErrorJQ);
            $span.html(messages.selectInstance);
            isValid = false;
        }

        if (!isValid) { $(".messageContainer").addClass('has-error'); }
        return isValid;
    }

    function clearAll() {
        $(elementIDs.txtFindTextJQ).val('');
        $(elementIDs.dvFindTextErrorJQ).empty();
        $(elementIDs.txtReplaceWithJQ).val('');
        $(elementIDs.drpWithinJQ).val('Select One');
        $(elementIDs.dvWithinErrorJQ).empty();
        $(elementIDs.drpLookInJQ).val('Select One');
        $(elementIDs.chkMatchJQ).prop('checked', false);
        $(elementIDs.divInstancesErrorJQ).empty();
        $(elementIDs.divInstancesJQ).hide()
        $(elementIDs.drpInstancesJQ).empty();
    }

    function replaceText() {
        var isValid = validate();
        if (isValid) {
            var sectionDetails = objFormInstance.form.getSectionData(objFormInstance.selectedSection);
            var sectionData = objFormInstance.formData[sectionDetails.FullName];
            var selectedInstances = [];
            $.each($(elementIDs.drpInstancesJQ).children("option:selected"), function () {
                selectedInstances.push($(this).val());
            });

            var data = {
                folderId: objFormInstance.folderId,
                folderVersionId: objFormInstance.folderVersionId,
                formInstanceId: objFormInstance.formInstanceId,
                formDesignVersionId: objFormInstance.formDesignVersionId,
                sectionName: objFormInstance.selectedSection,
                sectionData: JSON.stringify(sectionData),
                findWhat: $(elementIDs.txtFindTextJQ).val(),
                replaceWith: $(elementIDs.txtReplaceWithJQ).val(),
                withIn: $(elementIDs.drpWithinJQ).val(),
                lookIn: $(elementIDs.drpLookInJQ).val(),
                match: $(elementIDs.chkMatchJQ).prop('checked'),
                selectedInstances: selectedInstances.toString(),
            }

            var promise = ajaxWrapper.postJSON(URLs.replaceText, data);

            //callback function for ajax request success.
            promise.done(function (xhr) {
                if (xhr.Result === ServiceResult.FAILURE) {
                    messageDialog.show(messages.errorWhileProcessing);
                }
                else {
                    clearAll();
                    $(elementIDs.replaceDialogJQ).dialog('close');
                    objFormInstance.reload();
                }
            });
            promise.fail(showError);
        }
        return false;
    }

    return {
        show: function (folderInstance, formInstanceBuilder) {
            objFolderInstance = folderInstance;
            objFormInstance = formInstanceBuilder;

            var usesMasterListAliasDesign = false;
            if (folderInstance.formInstances != null && folderInstance.formInstances.length > 0) {
                for (var prop in folderInstance.formInstances) {
                    if (folderInstance.formInstances[prop].FormInstance.UsesMasterListAliasDesign == true) {
                        usesMasterListAliasDesign = true;
                        break;
                    }
                };
            }
            if (folderInstance.IsMasterList == true && usesMasterListAliasDesign == true) {
                if (objFormInstance.form.hasChanges()) {
                    messageDialog.show(messages.saveChanges);
                } else {
                    $(elementIDs.replaceDialogJQ).dialog('option', 'title', 'Find & Replace')
                    $(elementIDs.replaceDialogJQ).dialog('open');
                    clearAll();
                }
            }
            else {
                messageDialog.show(messages.notSupported);
            }
        }
    }
}();
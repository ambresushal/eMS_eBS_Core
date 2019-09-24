function formDesignViewjq(formDesignVersion, formDesignId, formDesignName, finalizedstatus) {
    this.tenantId = 1;
    this.formDesignVersion = formDesignVersion;
    this.formDesignId = formDesignId;
    this.formDesignName = formDesignName;
    this.status = finalizedstatus;
    this.data = null;
    this.objFormatter = this.formatter();
    this.prevPropGrid = null;
    this.prevPropElementId = null;

    this.elementIDs = {
        gridElementId: '#fdvuielems' + this.formDesignVersion.FormDesignVersionId,
        gridElementIdNoHash: 'fdvuielems' + this.formDesignVersion.FormDesignVersionId,
        buttonHolderJQ: '#actionContainer' + this.formDesignVersion.FormDesignVersionId,
    }

    this.URLs = {
        formDesignVersionList: '/FormDesign/FormDesignVersionList?tenantId=1&formDesignId={formDesignId}',
        formDesignUIElementList: '/UIElement/FormDesignVersionUIElementList?tenantId=1&formDesignVersionId={formDesignVersionId}',
        formDesignVersionUIElementList: '/UIElement/GetUIElementList?tenantId=1&formDesignVersionId={formDesignVersionId}',
        formDesignUIElementDelete: '/UIElement/DeleteElement?tenantId=1&formDesignVersionId={formDesignVersionId}&elementType={elementType}&uiElementId={uiElementId}',
        formDesignVersionPreview: '/FormDesignPreview/Preview?tenantId=1&formDesignVersionId={formDesignVersionId}&formName={formName}',
        CompileFormDesignVersion: '/FormDesign/CompileFormDesignVersion',
        CompileDocumentRule: '/FormDesign/CompileFormDesignVersionRule',
        CompileImpactedField: '/FormDesign/CompileImpactedField'
    }
}

formDesignViewjq.prototype.init = function () {
    var currentInstance = this;
    $(currentInstance.elementIDs.buttonHolderJQ).hide();
    $(currentInstance.elementIDs.gridElementId).closest('.grid-wrapper').show();
    currentInstance.buildGrid();

    if (ClientRolesForFormDesignAccess.indexOf(vbRole) >= 0) {
        currentInstance.SetFormDesignPermissionsToClientUser();
    }
}

formDesignViewjq.prototype.SetFormDesignPermissionsToClientUser = function () {
    var currentInstance = this;
    $('#viewTypeDDL' + currentInstance.formDesignVersion.FormDesignVersionId).prop("disabled", true);

    $(currentInstance.elementIDs.gridElementId).bind('jqGridAfterGridComplete', function (event, data) {
        var btn = $('#btn_' + currentInstance.elementIDs.gridElementIdNoHash + '_Delete');
        btn.hide();
        $(btn).unbind("click").bind("click", function () {
            var rowId = $(currentInstance.elementIDs.gridElementId).getGridParam('selrow');
            if (rowId !== undefined && rowId !== null) {
                var row = $(currentInstance.elementIDs.gridElementId).getRowData(rowId);
                if (row.IsStandard == "true") {
                    messageDialog.show(DocumentDesign.standardeMSElement);
                    return;
                } else {
                    currentInstance.deleteRow();
                }
            }
        });
    });
}

formDesignViewjq.prototype.destroy = function () {
    var currentInstance = this;
    $(currentInstance.elementIDs.gridElementId).jqGrid('GridUnload');
    $(currentInstance.elementIDs.gridElementId).closest('.grid-wrapper').hide();
}

formDesignViewjq.prototype.getData = function () {

}

formDesignViewjq.prototype.getColNames = function () {
    var colArray = ['TenantId', 'UIElementID', 'UIElementName', 'Label', 'Element Type', 'Is Mapped UIElement', 'hasLoadFromServer', 'IsRepeaterKey', 'GeneratedName', 'IsStandard'];
}

formDesignViewjq.prototype.getColModel = function () {
    var colModel = [];
    colModel.push({ name: 'TenantId', index: 'TenantId', hidden: true, search: false });
    colModel.push({ name: 'UIElementID', index: 'UIElementID', key: true, hidden: true, align: 'left', search: false });
    colModel.push({ name: 'UIElementName', index: 'UIElementName', hidden: true, align: 'left', search: false });
    colModel.push({ name: 'Label', index: 'Label', align: 'left', editable: false, width: '400px' });
    colModel.push({ name: 'ElementType', index: 'ElementType', align: 'left', editable: false, width: '150px' });
    colModel.push({ name: 'IsMappedUIElement', index: 'IsMappedUIElement', hidden: true, search: false });
    colModel.push({ name: 'hasLoadFromServer', index: 'hasLoadFromServer', hidden: true, search: false });
    colModel.push({ name: 'IsRepeaterKey', index: 'IsRepeaterKey', hidden: true, search: false });
    colModel.push({ name: 'GeneratedName', index: 'GeneratedName', hidden: true, search: false });
    colModel.push({ name: 'IsStandard', index: 'IsStandard', hidden: true, search: false });

    return colModel;
}

formDesignViewjq.prototype.formatter = function () {
    var currentInstance = this;
    return {

    }
}

formDesignViewjq.prototype.buildGrid = function (elements) {
    var currentInstance = this;

    $(currentInstance.elementIDs.gridElementId).parent().append("<div id='p" + currentInstance.elementIDs.gridElementIdNoHash + "'></div>");
    var formDesignId = currentInstance.formDesignId;
    var formDesignVersionId = currentInstance.formDesignVersion.FormDesignVersionId;
    var status = currentInstance.formDesignVersion.StatusText;

    var url = this.URLs.formDesignUIElementList.replace(/\{formDesignVersionId\}/g, currentInstance.formDesignVersion.FormDesignVersionId);

    $(currentInstance.elementIDs.gridElementId).jqGrid({
        url: url,
        datatype: 'json',
        treeGrid: true,
        treeGridModel: 'adjacency', // set this for hierarchical grid
        cache: false,
        colNames: currentInstance.getColNames(),
        colModel: currentInstance.getColModel(),
        caption: currentInstance.formDesignName + ' - ' + currentInstance.formDesignVersion.Version + ' - UI Elements',
        autowidth: false,
        shrinkToFit: false,
        forceFit: true,
        loadonce: true,
        rowNum: 10000,
        height: '350',
        expanded: true,
        ExpandColumn: 'Label',
        pager: '#p' + currentInstance.elementIDs.gridElementIdNoHash,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        altclass: 'alternate',
        //register event handler when a row is selected
        onSelectRow: function (id) {
            currentInstance.onRowSelect(id);
        },
        gridComplete: function () {
            var parentId, uiElementId;
            currentInstance.onGridRender(parentId, uiElementId);
        },
        resizeStop: function (width, index) {
            autoResizing(gridElementId);
        }
    });
    //remove paging
    var pagerElement = '#p' + currentInstance.elementIDs.gridElementIdNoHash;
    $(currentInstance.elementIDs.gridElementId).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();
    //add button in footer of grid that pops up the delete form design dialog

    $(currentInstance.elementIDs.gridElementId).jqGrid('navButtonAdd', pagerElement, { caption: '', buttonicon: 'ui-icon-trash', title: 'Delete', id: 'btn_' + currentInstance.elementIDs.gridElementIdNoHash + '_Delete', onClickButton: function () { currentInstance.deleteRow(); } });
    $(currentInstance.elementIDs.gridElementId).jqGrid('navButtonAdd', pagerElement, { caption: '', buttonicon: 'ui-icon-disk', title: 'Compile', id: 'btn_' + currentInstance.elementIDs.gridElementIdNoHash + '_Compile', onClickButton: function () { currentInstance.compileDesign(); } });
    $(currentInstance.elementIDs.gridElementId).jqGrid('navButtonAdd', pagerElement, { caption: '', buttonicon: 'ui-icon-script', title: 'CompileRule', id: 'btn_' + currentInstance.elementIDs.gridElementIdNoHash + '_CompileRule', onClickButton: function () { currentInstance.compileRules(); } });
    if (currentInstance.formDesignId == 2367) {
        $(currentInstance.elementIDs.gridElementId).jqGrid('navButtonAdd', pagerElement, { caption: '', buttonicon: 'ui-icon-gear', title: 'Compile Impacted Field', id: 'btn_' + currentInstance.elementIDs.gridElementIdNoHash + '_CompileImpact', onClickButton: function () { currentInstance.compileImpactedField(); } });
    }
    $(currentInstance.elementIDs.gridElementId).jqGrid('navButtonAdd', pagerElement, { caption: "", buttonicon: "ui-icon-triangle-2-n-s", onClickButton: function () { currentInstance.expandCollapse(); }, position: "last", title: "Expand/Collapse All", cursor: "pointer" });
}

formDesignViewjq.prototype.onRowSelect = function (id) {
    var currentInstance = this;

    if (currentInstance.prevPropGrid != null) {
        currentInstance.prevPropGrid.destroy();
    }
    var row = $(currentInstance.elementIDs.gridElementId).getRowData(id);
    if (row) {
        var propGrid = new uiElementPropertyGrid(row, currentInstance.formDesignId, currentInstance.formDesignVersion.FormDesignVersionId, currentInstance.status, $(currentInstance.elementIDs.gridElementId), currentInstance);
        propGrid.loadPropertyGrid();
        currentInstance.prevPropGrid = propGrid;
        if (row.ElementType === "[Blank]") {
            propGrid.hideGrid();
        }
        else {
            propGrid.showGrid();
        }
    }
    currentInstance.prevPropElementId = id;
}

formDesignViewjq.prototype.onColumnSelect = function () { }

formDesignViewjq.prototype.onGridRender = function (parentId, uiElementId) {
    var currentInstance = this;

    var row = $(currentInstance.elementIDs.gridElementId).jqGrid('getGridParam', 'data');

    var rowId;
    if (row.length > 0) {
        rowId = row[0].UIElementID;
    }

    var rowIdList = [];
    var selectedId = parentId;
    rowIdList.push(parentId);

    if (parentId != undefined || parentId != null) {
        getParent(parentId, rowIdList);
    }
    for (var i = 0; i < rowIdList.length; i++) {
        var rowToExpand = $(this).getLocalRow(rowIdList[i]);
        $(this).expandRow(rowToExpand);
    }

    //set first row 
    if (uiElementId === undefined) {
        $(this).jqGrid('setSelection', selectedId);
    } else {
        $(this).jqGrid('setSelection', uiElementId);
    }

    function getParent(parentId, rowIdList) {
        var selectedRowData = $(currentInstance.elementIDs.gridElementId).getRowData(parentId);
        if (selectedRowData.parent != 0) {
            rowIdList.push(selectedRowData.parent);
            getParent(selectedRowData.parent, rowIdList);
        }
    }

    //to check for UIElements grid claims..  
    //var objMap = {
    //    remove: '#btn_' + gridElementIdNoHash + '_Delete',
    //    preview: '#btn_' + gridElementIdNoHash + '_Preview',
    //    compile: '#btn_' + gridElementIdNoHash + '_Compile',
    //    compileRule: '#btn_' + gridElementIdNoHash + '_CompileRule',
    //};

    //checkApplicationClaims(claims, objMap, URLs.formDesignVersionList);
    //this function will check the permissions for Data Source.
    //authorizeUIElementsGrid($(this), URLs.formDesignVersionList, objMap);
}

formDesignViewjq.prototype.addRow = function () { }

formDesignViewjq.prototype.deleteRow = function () {
    var currentInstance = this;
    var rowId = $(currentInstance.elementIDs.gridElementId).getGridParam('selrow');
    var flag = false;
    if (rowId !== undefined && rowId !== null) {
        var row = $(currentInstance.elementIDs.gridElementId).getRowData(rowId);

        if (row.IsRepeaterKey == "true") {
            messageDialog.show(DocumentDesign.repeaterKeyElement)
            return;
        }

        for (var i = 0; i < Finalized.length ; i++) {
            if (Finalized[i].FORMDESIGNVERSIONID == currentInstance.formDesignVersion.FormDesignVersionId && Finalized[i].ISFINALIZED == 1)
                flag = true;
        }

        //assuming that "Finalized" status will have 3 as ID
        if (currentInstance.formDesignVersion.StatusId === "3" || flag == true) {
            messageDialog.show(DocumentDesign.deleteFromFinalizedValidationMsg);
        }
        else if (row.ElementType === "Tab") {
            messageDialog.show('Deleting a ' + row.ElementType + ' is not allowed.');
        }
        else if (row.IsMappedUIElement === "true") {
            messageDialog.show(DocumentDesign.deleteUIElementUsedInDataSource);
        }
        else {
            //load confirm dialogue to asset the operation
            confirmDialog.show(Common.deleteConfirmationMsg, function () {
                confirmDialog.hide();
                //adding parameters to url 
                url = currentInstance.URLs.formDesignUIElementDelete.replace(/\{formDesignVersionId\}/g, currentInstance.formDesignVersion.FormDesignVersionId).replace(/\{uiElementId\}/g, row.UIElementID);
                url = url.replace(/\{elementType\}/g, row.ElementType);

                var promise = ajaxWrapper.postJSON(url);

                promise.done(function (xhr) {
                    if (xhr.Result === ServiceResult.SUCCESS) {
                        messageDialog.show(DocumentDesign.deleteMsg);
                        currentInstance.buildGrid(currentInstance.prevPropGrid.uiElement.parent);
                    }
                    else if (xhr.Result === ServiceResult.FAILURE) {
                        messageDialog.show(xhr.Items[0].Messages[0]);
                    }
                    else {
                        messageDialog.show(Common.errorMsg);
                    }
                });

                promise.fail(currentInstance.showError);
            });
        }
    }
    else {
        messageDialog.show(DocumentDesign.inProgressDesignSelectionMsg);
    }
}

formDesignViewjq.prototype.compileImpactedField = function () {
    var currentInstance = this;
    var url = currentInstance.URLs.CompileImpactedField;
    var data = {
        tenantId: 1,
        formDesignVersionId: currentInstance.formDesignVersion.FormDesignVersionId
    };

    var promise = ajaxWrapper.postJSON(url, data);
    //success callback
    promise.done(function (xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show("Impacted Field compiled successfully.");
        }
        else {
            if (xhr.Items.length > 0) {
                var msg = xhr.Items[0].Messages + '\n \n' + "Impacted Field compiled failed."
                messageDialog.show(msg);
            }
            else {
                messageDialog.show("Impacted Field compiled failed.");
            }
        }
    });
    //failure callback
    promise.fail(currentInstance.showError);
}

formDesignViewjq.prototype.compileDesign = function () {
    var currentInstance = this;

    var url = currentInstance.URLs.CompileFormDesignVersion;
    var data = {
        tenantId: 1,
        formDesignVersionId: currentInstance.formDesignVersion.FormDesignVersionId
    };

    var promise = ajaxWrapper.postJSON(url, data);

    promise.done(function (xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(DocumentDesign.documentCompileMsg);
        }
        else {
            messageDialog.show(DocumentDesign.compileFailureMsg);
        }
    });

    promise.fail(currentInstance.showError);
}

formDesignViewjq.prototype.compileRules = function () {
    var currentInstance = this;

    var url = currentInstance.URLs.CompileDocumentRule;
    var data = {
        tenantId: 1,
        formDesignVersionId: currentInstance.formDesignVersion.FormDesignVersionId
    };

    var promise = ajaxWrapper.postJSON(url, data);

    promise.done(function (xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(DocumentDesign.documentRuleCompileMsg);
        }
        else {
            if (xhr.Items.length > 0) {
                var msg = xhr.Items[0].Messages + '\n \n' + DocumentDesign.documentRuleCompileFailureMsg
                messageDialog.show(msg);
            }
            else {
                messageDialog.show(DocumentDesign.documentRuleCompileFailureMsg);
            }
        }
    });

    promise.fail(currentInstance.showError);
}

formDesignViewjq.prototype.expandCollapse = function () {
    var currentInstance = this;

    var allRowsInGrid = $(currentInstance.elementIDs.gridElementId).jqGrid('getGridParam', 'data');
    var expandCount = 0;
    var collapseCount = 0;
    var expandableCount = 0;
    for (var i = 0; i < allRowsInGrid.length; i++) {
        if ((allRowsInGrid[i].ElementType === "Tab" || allRowsInGrid[i].ElementType === "Section" || allRowsInGrid[i].ElementType === "Repeater")) {
            expandableCount++;
            if (allRowsInGrid[i].expanded) {
                expandCount++;
            }
            else {
                collapseCount++;
            }
        }
    }
    if (expandCount == 0 && collapseCount == expandableCount) {
        $(".treeclick", currentInstance.elementIDs.gridElementId).each(function () {
            if ($(this).hasClass("tree-plus")) {
                $(this).trigger("click");
            }
        });
    } else {
        $(".treeclick", currentInstance.elementIDs.gridElementId).each(function () {
            if ($(this).hasClass("tree-minus")) {
                $(this).trigger("click");
            }
        });
    }
}

formDesignViewjq.prototype.showError = function (xhr) {
    if (xhr.status == 999)
        this.location = '/Account/LogOn';
    else

        messageDialog.show(JSON.stringify(xhr));
}
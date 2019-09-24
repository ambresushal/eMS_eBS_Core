function serviceDesignVersion(serviceDesignVersion, serviceDesignId, serviceDesignName, tabNamePrefix, tabIndex, tabCount, tabs) {
    this.formDesignId = serviceDesignVersion.FormDesignID;
    this.formDesignName = serviceDesignVersion.FormDesignName;
    this.formDesignVersionId = serviceDesignVersion.FormDesignVersionID;
    this.serviceDesignVersion = serviceDesignVersion;
    this.serviceDesignId = serviceDesignId;
    this.serviceDesignName = serviceDesignName
    this.tabNamePrefix = tabNamePrefix;
    this.tabs = tabs;
    this.tabCount = tabCount;
    this.tabIndex = tabIndex;
    this.isFinalized = serviceDesignVersion.IsFinalized;
    //URL to get elements hierarchically for a form design version
    this.URLs = {
        formDesignUIElementList: '/UIElement/FormDesignVersionUIElementList?tenantId=1&formDesignVersionId={formDesignVersionId}',
        formDesignUIElementDelete: '/UIElement/DeleteElement?tenantId=1&formDesignVersionId={formDesignVersionId}&elementType={elementType}&uiElementId={uiElementId}',
        serviceDesignElementList: '/ServiceDefinition/GetServiceDefinitionElementList?tenantId=1&serviceDesignVersionId={serviceDesignVersionId}',
        addServiceDefinition: '/ServiceDefinition/Add',
        deleteServiceDefinition: '/ServiceDefinition/Delete',
    }
    this.prevPropGrid = null;
    this.prevPropElementId = null;
    this.hasParentElementIsLoadFromServer = null;

    this.elementIDs = {
        formDesignVersionGrid: "fdvuielems" + this.serviceDesignVersion.FormDesignVersionID + this.serviceDesignVersion.ServiceDesignVersionId,
        formDesignVersionGridJQ: "#fdvuielems" + this.serviceDesignVersion.FormDesignVersionID + this.serviceDesignVersion.ServiceDesignVersionId,
        serviceDesignVersionGrid: "sdvuielems" + this.serviceDesignVersion.ServiceDesignVersionId,
        serviceDesignVersionGridJQ: "#sdvuielems" + this.serviceDesignVersion.ServiceDesignVersionId,
    }
}

//loadTabPage method - call to load the Service Design Version details in a separate tab page
//create a new instance of serviceDesignVersion and call this method
//usage: in formdesign.js
serviceDesignVersion.prototype.loadTabPage = function () {
    var tabName = this.serviceDesignName + '-' + this.serviceDesignVersion.FormDesignVersionNumber;
    //create link for the tab page
    var tabTemplate = "<li><a id= '" + this.serviceDesignName + "-" + this.serviceDesignVersion.VersionNumber + "' href='#{href}'>#{label}</a> <span class='ui-icon ui-icon-close' data-tabid=" + this.serviceDesignVersion.ServiceDesignVersionId + " role='presentation'>Remove Tab</span></li>";
    //replace based on version being loaded for this form
    var li = $(tabTemplate.replace(/#\{href\}/g, '#' + this.tabNamePrefix).replace(/#\{label\}/g, this.serviceDesignName + '-' + this.serviceDesignVersion.VersionNumber));
    //create div for the tab page content
    var tabContentTemplate = "<div class='row-fluid' style='overflow:auto;'>"
                                + "<div class='col-xs-4'>"
                                    + "<table id='" + this.elementIDs.formDesignVersionGrid + "'>"
                                    + "</table>"
                                + "</div>"
                                + "<div class='col-xs-1'>"
                                    + "<div class='row'>"
                                        + "<div class='col-xs-12'>"
                                            + "<div >"
                                                + "<div class='btn-group-vertical'>"
                                                    + "<button id='btnAddElement{serviceDesignVersionId}' class='btn btn-default'>Add >> </button>"
                                                    + "<br/>"
                                                    + "<button id='btnRemoveElement{serviceDesignVersionId}' class='btn btn-default'>Remove << </button>"
                                                + "</div>"
                                            + "</div>"
                                        + "</div>"
                                    + "</div>"
                                + "</div>"
                                + "<div class='col-xs-4'>"
                                    + "<table id='" + this.elementIDs.serviceDesignVersionGrid + "'>"
                                    + "</table>"
                                + "</div>"
                                + "<div class='col-xs-3'>"
                                    + "<div id='fdvuielemdetail{serviceDesignVersionId}container'>"
                                        + "<table id='fdvuielemdetail{serviceDesignVersionId}'>"
                                        + "</table>"
                                    + "</div>"
                                + "</div>"
                            + "</div>";
    var tabContent = tabContentTemplate.replace(/\{formDesignVersionId\}/g, this.serviceDesignVersion.FormDesignVersionID)
                                            .replace(/\{serviceDesignVersionId\}/g, this.serviceDesignVersion.ServiceDesignVersionId);
    //create tab page using jqueryui tab methods
    this.tabs.find('.ui-tabs-nav').append(li);
    this.tabs.append("<div id='" + this.tabNamePrefix + "'>" + tabContent + "</div><div style='clear:both'></div>");
    this.tabs.tabs('refresh');

    if (Finalized.length > 0) {
        this.tabs.tabs({ selected: (this.tabCount - 1) });
    }
    else
        this.tabs.tabs('option', 'active', this.tabCount);

    var currentInstance = this;
    //This function is used to nullify the instance of serviceDesignVersion on its close event.
    $(currentInstance.tabs).find('span[data-tabid=' + currentInstance.serviceDesignVersion.ServiceDesignVersionId + ']').on("click", function () {
        currentInstance = null;
    });

    tabs = $('#formdesigntabs').tabs();
    //click event of tab
    tabs.delegate('.ui-tabs-anchor', 'click', function (ctx) {
        var id = ctx.currentTarget.id;

        if (currentInstance != null && currentInstance != undefined) {
            var tabName = currentInstance.serviceDesignName + "-" + currentInstance.serviceDesignVersion.FormDesignVersionNumber;
            for (var i = 0; i < Finalized.length ; i++) {
                if (Finalized[i].FORMDESIGNVERSIONID == currentInstance.formDesignVersionId && Finalized[i].ISFINALIZED == 1) {
                    if (tabName == id && currentInstance.serviceDesignVersion != "Service Designs") {
                        if (Finalized[i].ISMESSAGEDISPLAYED != true) {
                            messageDialog.show(ServiceDesign.versionFinalizedMsg);
                            Finalized[i].ISMESSAGEDISPLAYED = true;
                            $('#fdvuielemdetail' + currentInstance.serviceDesignVersion.ServiceDesignVersionId).trigger('reloadGrid');
                            var rowID = $(currentInstance.elementIDs.formDesignVersionGridJQ).getGridParam('selrow');
                            if (rowID != null) {
                                var row = $(currentInstance.elementIDs.formDesignVersionGridJQ).getRowData(rowID);
                                //var propGrid = new uiElementPropertyGrid(row, currentInstance.formDesignId.formDesignId, currentInstance.formDesignVersionId, currentInstance.isFinalized, currentInstance.getGridData(), currentInstance);
                                //propGrid.loadPropertyGrid();
                            }
                        }
                    }
                }
            }
        }
    });

    //load the hierarchical UI Element Grid
    this.loadUIElementGrid();

    this.loadServiceDefinitionGrid();

    //regisrer Add button event 
    $("#btnAddElement" + this.serviceDesignVersion.ServiceDesignVersionId).click(function (e) {
        currentInstance.addElement();
    });

    $("#btnRemoveElement" + this.serviceDesignVersion.ServiceDesignVersionId).click(function (e) {
        currentInstance.removeElement();
    });
}

serviceDesignVersion.prototype.validate = function (formDesignVersionRow, serviceDesignVersionParentRow) {
    var message = '';
    if (formDesignVersionRow.ElementType !='Tab') {
        var formDesignVersionParentRow, serviceDesignVersionParentRow;
        var formDesignData = $(this.elementIDs.formDesignVersionGridJQ).jqGrid('getGridParam', 'data');
        if (formDesignData) {
            formDesignVersionParentRow = formDesignData.filter(function (ct) {
                if (ct.UIElementID == formDesignVersionRow.parent)
                    return ct;
            });
        }

        if (formDesignVersionParentRow && serviceDesignVersionParentRow) {
            if (serviceDesignVersionParentRow.UIElementType == 'Repeater' && formDesignVersionParentRow[0].ElementType == 'Repeater') {
                if (serviceDesignVersionParentRow.UIElementID == formDesignVersionParentRow[0].UIElementID) {
                    message = '';
                }
                else
                    message = 'Parent element can not be different';
            }
            else if (serviceDesignVersionParentRow.UIElementType == 'Repeater' || formDesignVersionParentRow[0].ElementType == 'Repeater') {
                message = 'A repeater element can only be added inside repeater.';
            }
            else
                message = '';
        }
    }
    return message;
}

//call to load the hierarchical UI Element Grid for this instance
serviceDesignVersion.prototype.loadUIElementGrid = function (parentId, uiElementId) {
    //multiple instances, so need to generate id's dynamically
    var gridElementIdJQ = this.elementIDs.formDesignVersionGridJQ;
    var gridElementId = this.elementIDs.formDesignVersionGrid;

    $(gridElementIdJQ).jqGrid('GridUnload');

    //set column list
    var colArray = ['TenantId', 'UIElementID', 'UIElementName', 'Label', 'Element Type', 'Is Mapped UIElement', 'IsKey'];

    //set column models
    var colModel = [];
    colModel.push({ name: 'TenantId', index: 'TenantId', hidden: true, search: false });
    colModel.push({ name: 'UIElementID', index: 'UIElementID', key: true, hidden: true, align: 'center', search: false });
    colModel.push({ name: 'UIElementName', index: 'UIElementName', hidden: true, align: 'center', search: false });
    colModel.push({ name: 'Label', index: 'Label', align: 'left', editable: false, width: '200px' });
    colModel.push({ name: 'ElementType', index: 'ElementType', align: 'left', editable: false, width: '200px' });
    colModel.push({ name: 'IsMappedUIElement', index: 'IsMappedUIElement', hidden: true, search: false });
    colModel.push({ name: 'IsKey', index: 'IsKey', hidden: true, search: false });

    //get URL for grid
    var url = this.URLs.formDesignUIElementList.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId);
    //the 'this' property points to the grid in grid event handlers - set this to currentInstance so that it can be used 
    //within event handlers to access this instance of serviceDesignVersion - currentInstance will be available in the 
    //context of event handlers that are registered within this function
    var currentInstance = this;

    //add footer of grid
    $(gridElementIdJQ).parent().append("<div id='p" + gridElementId + "'></div>");
    var formDesignId = this.formDesignId;
    var formDesignVersionId = this.formDesignVersionId;
    var isFinalized = this.isFinalized;
    var lastSelIdx = undefined;
    //load grid
    $(gridElementIdJQ).jqGrid({
        url: url,
        datatype: 'json',
        treeGrid: true,
        treeGridModel: 'adjacency', // set this for hierarchical grid
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: this.formDesignName + ' - ' + this.serviceDesignVersion.FormDesignVersionNumber + ' - UI Elements',
        autowidth: true,
        shrinkToFit: false,
        forceFit: true,
        loadonce: true,
        rowNum: 10000,
        height: '360',
        expanded: true,
        ExpandColumn: 'Label',
        pager: '#p' + gridElementId,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        altclass: 'alternate',
        //register event handler when a row is selected
        onSelectRow: function (id) {
            var row = $(this).getRowData(id);
            if (row) {
                if (row.ElementType == 'Tab') {
                    $("#btnAddElement" + currentInstance.serviceDesignVersion.ServiceDesignVersionId).attr("disabled", "disabled");
                }
                else {
                    $("#btnAddElement" + currentInstance.serviceDesignVersion.ServiceDesignVersionId).removeAttr("disabled");
                }
            }
        },
        gridComplete: function () {
            var row = $(this).jqGrid('getGridParam', 'data');
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
            if (uiElementId == undefined && selectedId == undefined) {
                $(this).jqGrid('setSelection', row[0].UIElementID);
            }
            else if (uiElementId === undefined) {
                $(this).jqGrid('setSelection', selectedId);
            } else {
                $(this).jqGrid('setSelection', uiElementId);
            }

            //to check for UIElements grid claims..  
            var objMap = {
                remove: '#btn_' + gridElementId + '_Delete',
                preview: '#btn_' + gridElementId + '_Preview',
                compile: '#btn_' + gridElementId + '_Compile',
            };
            /*TODO: Add code for Checking application Claims*/
            //checkApplicationClaims(claims, objMap, URLs.formDesignVersionList);
            ////this function will check the permissions for Data Source.
            //authorizeUIElementsGrid($(this), URLs.formDesignVersionList, objMap);


        },
        resizeStop: function (width, index) {
            autoResizing(gridElementIdJQ);
        },
        //beforeSelectRow: function (id, e) {
        //    var row = $("#" + id);
        //    var currSelIdx = $("#tree").getInd(id) - 1;
        //    if (e.ctrlKey) {
        //        // Ctrl was pressed - Add to selection or remove
        //        if (row.attr("aria-selected") == "true") {
        //            row.removeClass("ui-state-highlight").attr("aria-selected", "false");
        //        } else {
        //            row.addClass("ui-state-highlight").attr("aria-selected", "true");
        //        }
        //        lastSelIdx = currSelIdx;
        //    } else if (e.shiftKey) {
        //        // Shift was pressed. Select all between last selected and curently selected
        //        var rows = $(".jqgrow");
        //        // Select all rows between the last selected
        //        if (!lastSelIdx) lastSelIdx = 0;
        //        if (lastSelIdx > currSelIdx) {
        //            selmin = currSelIdx;
        //            selmax = lastSelIdx;
        //        } else {
        //            selmin = lastSelIdx;
        //            selmax = currSelIdx;
        //        }
        //        for (i = 0; i < rows.length; i++) {
        //            if (i >= selmin && i <= selmax) {
        //                $(rows[i]).addClass("ui-state-highlight").attr("aria-selected", "true");
        //            } else {
        //                $(rows[i]).removeClass("ui-state-highlight").attr("aria-selected", "false");
        //            }
        //        }
        //    } else {
        //        // Simple click
        //        $("tr[aria-selected=true]").each(function () {
        //            $(this).removeClass("ui-state-highlight").attr("aria-selected", "false");
        //        });
        //        row.addClass("ui-state-highlight").attr("aria-selected", "true");
        //        lastSelIdx = currSelIdx;
        //    }
        //    return false;
        //},
    });
    //remove paging
    var pagerElement = '#p' + gridElementId;
    $(gridElementIdJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    // Code for expand and collapse all
    $(gridElementIdJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: "",
            buttonicon: "ui-icon-triangle-2-n-s",
            onClickButton: function () {
                var allRowsInGrid = $(gridElementIdJQ).jqGrid('getGridParam', 'data');
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
                    $(".treeclick", gridElementIdJQ).each(function () {
                        if ($(this).hasClass("tree-plus")) {
                            $(this).trigger("click");
                        }
                    });
                } else {
                    $(".treeclick", gridElementIdJQ).each(function () {
                        if ($(this).hasClass("tree-minus")) {
                            $(this).trigger("click");
                        }
                    });
                }
            },

            position: "last",
            title: "Expand/Collapse All",
            cursor: "pointer"
        });

    function getParent(parentId, rowIdList) {
        var selectedRowData = $(gridElementIdJQ).getRowData(parentId);
        if (selectedRowData.parent != 0) {
            rowIdList.push(selectedRowData.parent);
            getParent(selectedRowData.parent, rowIdList);
        }
    }
}

serviceDesignVersion.prototype.loadServiceDefinitionGrid = function (parentId, serviceDefinitionId) {
    //multiple instances, so need to generate id's dynamically
    var gridElementIdJQ = '#sdvuielems' + this.serviceDesignVersion.ServiceDesignVersionId;
    var gridElementId = 'sdvuielems' + this.serviceDesignVersion.ServiceDesignVersionId;

    $(gridElementIdJQ).jqGrid('GridUnload');

    //set column list
    var colArray = ['TenantId', 'ServiceDefinitionID', 'ParentServiceDefinitionID', 'UIElementID', 'DisplayName', 'Label', 'Element Type', 'ElementTypeID', 'UIElementDataTypeID', 'IsKey'];

    //set column models
    var colModel = [];
    colModel.push({ name: 'TenantId', index: 'TenantId', hidden: true, search: false });
    colModel.push({ name: 'ServiceDefinitionID', index: 'ServiceDefinitionID', key: true, hidden: true, align: 'center', search: false });
    colModel.push({ name: 'ParentServiceDefinitionID', index: 'ParentServiceDefinitionID', key: true, hidden: true, align: 'center', search: false });
    colModel.push({ name: 'UIElementID', index: 'UIElementID', key: true, hidden: true, align: 'center', search: false });
    colModel.push({ name: 'DisplayName', index: 'DisplayName', align: 'left', editable: false, width: '200px', hidden: true });
    colModel.push({ name: 'UIElementLabel', index: 'UIElementLabel', align: 'left', editable: false, width: '200px' });
    colModel.push({ name: 'UIElementType', index: 'UIElementType', align: 'left', editable: false, width: '200px' });
    colModel.push({ name: 'UIElementTypeID', index: 'UIElementTypeID', align: 'left', editable: false, width: '200px', hidden: true });
    colModel.push({ name: 'UIElementDataTypeID', index: 'UIElementDataTypeID', align: 'left', editable: false, width: '200px', hidden: true });
    colModel.push({ name: 'IsKey', index: 'IsKey', key: true, hidden: true, align: 'center', search: false });

    //get URL for grid
    var url = this.URLs.serviceDesignElementList.replace(/\{serviceDesignVersionId\}/g, this.serviceDesignVersion.ServiceDesignVersionId);
    //the 'this' property points to the grid in grid event handlers - set this to currentInstance so that it can be used 
    //within event handlers to access this instance of serviceDesignVersion - currentInstance will be available in the 
    //context of event handlers that are registered within this function
    var currentInstance = this;

    //add footer of grid
    $(gridElementIdJQ).parent().append("<div id='p" + gridElementId + "'></div>");
    var formDesignId = this.formDesignId;
    var formDesignVersionId = this.formDesignVersionId;
    var isFinalized = this.isFinalized;
    //load grid
    $(gridElementIdJQ).jqGrid({
        url: url,
        datatype: 'json',
        treeGrid: true,
        treeGridModel: 'adjacency', // set this for hierarchical grid
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: this.serviceDesignName + ' - ' + this.serviceDesignVersion.VersionNumber + ' - Web API Definitions',
        autowidth: true,
        shrinkToFit: false,
        forceFit: true,
        rowNum: 10000,
        height: '360',
        expanded: true,
        ExpandColumn: 'UIElementLabel',
        pager: '#p' + gridElementId,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        altclass: 'alternate',
        //register event handler when a row is selected
        onSelectRow: function (id) {
            if (currentInstance.prevPropGrid != null) {
                //currentInstance.prevPropGrid.destroy();
            }
            var row = $(this).getRowData(id);
            if (row) {

                if (!row.ParentServiceDefinitionID) {
                    $("#btnRemoveElement" + currentInstance.serviceDesignVersion.ServiceDesignVersionId).attr("disabled", "disabled");
                }
                else {
                    $("#btnRemoveElement" + currentInstance.serviceDesignVersion.ServiceDesignVersionId).removeAttr("disabled");
                }
                var propGrid = new serviceDefinitionPropertyGrid(row.ServiceDefinitionID, row, currentInstance.serviceDesignId, currentInstance.serviceDesignName, currentInstance);
                propGrid.loadPropertyGrid();
                currentInstance.prevPropGrid = propGrid;
                propGrid.showGrid();
            }
            currentInstance.prevPropElementId = id;

        },
        gridComplete: function () {
            var row = $(this).jqGrid('getGridParam', 'data');
            var rowId;
            if (row.length > 0) {
                rowId = row[0].ServiceDefinitionID;
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
            if (serviceDefinitionId === undefined && selectedId === undefined) {
                $(this).jqGrid('setSelection', row[0].ServiceDefinitionID);
            } else if (serviceDefinitionId === undefined) {
                $(this).jqGrid('setSelection', selectedId);
            } else {
                $(this).jqGrid('setSelection', serviceDefinitionId);
            }

            //to check for UIElements grid claims..  
            var objMap = {
                remove: '#btn_' + gridElementId + '_Delete',
                preview: '#btn_' + gridElementId + '_Preview',
                compile: '#btn_' + gridElementId + '_Compile',
            };

            /*TODO: Add code for Checking application Claims*/
            //checkApplicationClaims(claims, objMap, URLs.formDesignVersionList);
            ////this function will check the permissions for Data Source.
            //authorizeUIElementsGrid($(this), URLs.formDesignVersionList, objMap);


        },
        resizeStop: function (width, index) {
            autoResizing(gridElementIdJQ);
        }
    });
    //remove paging
    var pagerElement = '#p' + gridElementId;
    $(gridElementIdJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    // Code for expand and collapse all
    $(gridElementIdJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: "",
            buttonicon: "ui-icon-triangle-2-n-s",
            onClickButton: function () {
                var allRowsInGrid = $(gridElementIdJQ).jqGrid('getGridParam', 'data');
                var expandCount = 0;
                var collapseCount = 0;
                var expandableCount = 0;
                for (var i = 0; i < allRowsInGrid.length; i++) {
                    if ((allRowsInGrid[i].UIElementType === "Tab" || allRowsInGrid[i].UIElementType === "Section" || allRowsInGrid[i].UIElementType === "Repeater")) {
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
                    $(".treeclick", gridElementIdJQ).each(function () {
                        if ($(this).hasClass("tree-plus")) {
                            $(this).trigger("click");
                        }
                    });
                } else {
                    $(".treeclick", gridElementIdJQ).each(function () {
                        if ($(this).hasClass("tree-minus")) {
                            $(this).trigger("click");
                        }
                    });
                }
            },

            position: "last",
            title: "Expand/Collapse All",
            cursor: "pointer"
        });

    function getParent(parentId, rowIdList) {
        var selectedRowData = $(gridElementIdJQ).getRowData(parentId);
        if (selectedRowData.parent != 0) {
            rowIdList.push(selectedRowData.parent);
            getParent(selectedRowData.parent, rowIdList);
        }
    }
}

//return data of this grid to the caller
serviceDesignVersion.prototype.getGridData = function () {
    var gridElementIdJQ = this.elementIDs.formDesignVersionGridJQ;
    return $(gridElementIdJQ);
}

//returns error box for serviceDesignVersion 
serviceDesignVersion.prototype.showError = function (xhr) {
    if (xhr.status == 999)
        this.location = '/Account/LogOn';
    else

        messageDialog.show(JSON.stringify(xhr));
}

serviceDesignVersion.prototype.addElement = function () {
    var currentInstance = this;
    var formDesignVersionRowID = $(currentInstance.elementIDs.formDesignVersionGridJQ).getGridParam('selrow');
    var serviceDesignVersionRowID = $(currentInstance.elementIDs.serviceDesignVersionGridJQ).getGridParam('selrow');
    if (formDesignVersionRowID != null && serviceDesignVersionRowID) {
        var formDesignVersionRow = $(currentInstance.elementIDs.formDesignVersionGridJQ).getRowData(formDesignVersionRowID);
        var parentServiceDesignVersionRow = $(currentInstance.elementIDs.serviceDesignVersionGridJQ).getRowData(serviceDesignVersionRowID);

        var validationMsg = currentInstance.validate(formDesignVersionRow, parentServiceDesignVersionRow);
        if (validationMsg != '') {
            messageDialog.show(validationMsg);
        }
        else {
            var addKeys = false; var addChildElements = false;
            if (formDesignVersionRow.ElementType === 'Repeater') {
                addKeys = true;
            }

            if (formDesignVersionRow.ElementType === 'Repeater' || formDesignVersionRow.ElementType === 'Section') {
                extConfirmDialog.show("Do you want to add all the elements inside this " + formDesignVersionRow.ElementType + "?", function (e) {
                    extConfirmDialog.hide();
                    if (e === true) {
                        addChildElements = true;
                        currentInstance.add(addKeys, addChildElements, formDesignVersionRow, parentServiceDesignVersionRow);
                    }
                    else {
                        addChildElements = false;
                        currentInstance.add(addKeys, addChildElements, formDesignVersionRow, parentServiceDesignVersionRow);
                    }
                });
            }
            else {
                currentInstance.add(addKeys, addChildElements, formDesignVersionRow, parentServiceDesignVersionRow);
            }
        }
    }
    else if (formDesignVersionRowID == null) {
        messageDialog.show('Please select row from Service Design Version Grid');
    }
    else if (serviceDesignVersionRowID == null) {
        messageDialog.show('Please select row from Service Definition Version Grid');
    }
}

serviceDesignVersion.prototype.removeElement = function () {
    var currentInstance = this;
    var serviceDesignVersionRowID = $(currentInstance.elementIDs.serviceDesignVersionGridJQ).getGridParam('selrow');
    if (serviceDesignVersionRowID) {
        var serviceDesignVersionRow = $(currentInstance.elementIDs.serviceDesignVersionGridJQ).getRowData(serviceDesignVersionRowID);

        if (currentInstance.serviceDesignVersion.IsFinalized == true) {
            messageDialog.show("Can not delete an element in Finalized service");
        }
        else {
            confirmDialog.show(Common.deleteConfirmationMsg, function () {
                confirmDialog.hide();
                var url = currentInstance.URLs.deleteServiceDefinition;
                var data = {
                    tenantId: 1,
                    serviceDesignVersionId: currentInstance.serviceDesignVersion.ServiceDesignVersionId,
                    serviceDefinitionId: serviceDesignVersionRow.ServiceDefinitionID,
                };

                var promise = ajaxWrapper.postJSON(url, data);
                //success callback
                promise.done(function (xhr) {
                    if (xhr.Result === ServiceResult.SUCCESS) {
                        messageDialog.show('Element Deleted Succesfully.');
                        currentInstance.loadServiceDefinitionGrid(serviceDesignVersionRow.ParentServiceDefinitionID);
                    }
                    else {
                        messageDialog.show("Unable to delete element.");
                    }
                });
                //failure callback
                promise.fail(currentInstance.showError);
            });
        }
    }
    else if (serviceDesignVersionRowID == null) {
        messageDialog.show('Please select row from Service Definition Version Grid');
    }
}

serviceDesignVersion.prototype.add = function (addKeys, addChildElements, formDesignVersionRow, parentServiceDesignVersionRow) {
    var currentInstance = this;
    var url = currentInstance.URLs.addServiceDefinition;
    var data = {
        tenantId: 1,
        formDesignVersionId: currentInstance.formDesignVersionId,
        serviceDesignVersionId: currentInstance.serviceDesignVersion.ServiceDesignVersionId,
        uielementId: formDesignVersionRow.UIElementID,
        parentServiceDefinitionId: parentServiceDesignVersionRow.ServiceDefinitionID,
        isKey: false,
        addChildKeys: addKeys,
        addChildElements: addChildElements,
    };

    var promise = ajaxWrapper.postJSON(url, data);
    //success callback
    promise.done(function (xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show('Element Added Succesfully.');
            currentInstance.loadServiceDefinitionGrid(parentServiceDesignVersionRow.ServiceDefinitionID);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
    });
    //failure callback
    promise.fail(currentInstance.showError);
}
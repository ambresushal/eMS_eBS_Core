var globalUpdates = function () {

    //variable to hold the instance
    var instance;

    function globalUpdate(effectiveDateFrom, effectiveDateTo, globalUpdateName, wizardStepsID, globalUpdateStatusID, globalUpdateID) {

        this.effectiveDateFrom = effectiveDateFrom;
        this.effectiveDateTo = effectiveDateTo;
        this.globalUpdateName = globalUpdateName;
        this.wizardStepsID = wizardStepsID;
        this.globalUpdateStatusID = globalUpdateStatusID;
        this.globalUpdateId = globalUpdateID;
        this.isEditInitialised = false;
        this.formDesignId = undefined;
        // this.confirmUpdateValueDetails = undefined;


        var currentInstance = this;
        instance = currentInstance;

        this.elementIDs = {
            //Document Design Version Grid
            documentDesignVersion: 'guDesignVersions',
            //Document Design Version jquery Grid 
            documentDesignVersionJQ: '#guDesignVersions',
            //Document Design Version Elements Grid
            documentDesignElements: 'guDesignVersionsElements',
            //Document Design Version Elements jquery Grid
            documentDesignElementsJQ: '#guDesignVersionsElements',
            //globalupdate Header
            globalUpdateNameHeaderJQ: '#globalUpdateNameHeader',
            //globalupdate Effective Period Header
            globalUpdateEffectivePeriodJQ: '#globalUpdateEffectivePeriod',
            guDesignValueUpdateJQ: '#guDesignValueUpdate',
            //globalupdate Element update Document Selection elements without JQ
            guDesignValueUpdate: 'guDesignValueUpdate',
            // document version dropdown
            documentVersionDropDownJQ: '#availableDocumentVersion',
            //get  selectedDocument version UiElements
            selectedDocumentDesignVersionElements: 'guSelectedDesignVersionsElements',
            //get  selectedDocument version UiElements
            selectedDocumentDesignVersionElementsJQ: '#guSelectedDesignVersionsElements',

            divUpdateFoldersJQ: '#divUpdateFolders',
            globalUpdateImpactedFoldersGrid: 'globalUpdateImpactedFoldersGrid',
            globalUpdateImpactedFoldersGridJQ: '#globalUpdateImpactedFoldersGrid',
            btnGenerateJQ: '#btnGenerate',
            rootwizard: '#rootwizard',
            globalUpdateName: "globalUpdateName",
            globalUpdateNameJQ: "#globalUpdateName",
            globalUpdateEffectiveFrom: "globalUpdateEffectiveFrom",
            globalUpdateEffectiveFromJQ: "#globalUpdateEffectiveFrom",
            globalUpdateEffectiveTo: "globalUpdateEffectiveTo",
            globalUpdateEffectiveToJQ: "#globalUpdateEffectiveTo",
            iasWizardJQ: '#iasWizard',
            addGlobalUpdateJQ: "#guAdd",
            clearGlobalUpdateJQ: "#guClear",
            anchorNextJQ: "#anchorNext",
            activityGridDialogJQ: "#activityGridDialog",
            activityGridJQ: "#activityGrid",
            activityGrid: "activityGridJQ",
            panelHeaderJQ: "#panelHeader",
            //globalUpdateedit Confirmation
            editGlobalUpdateconfirmDialog: "#confirmGlobalUpdateEditDialog"

        };
        this.URLs = {
            //get Document Design Version List
            getDocumentDesignVersions: '/GlobalUpdate/FormDesignVersionList?effectiveDateFrom={effectiveDateFrom}&effectiveDateTo={effectiveDateTo}',
            //get Document Design Version Elements List
            getDocumentVersionElementList: '/GlobalUpdate/FormDesignVersionUIElementList?tenantId=1&formDesignVersionId={formDesignVersionId}&globalUpdateId={globalUpdateId}&isOnlySelectedElements={isOnlySelectedElements}',
            //save Selected Element for Document Design Version
            saveElementSelection: '/GlobalUpdate/SaveElementSelection',
            //get updated document design verions
            getSelectedDocumentVersions: '/GlobalUpdate/GetUpdatedDocumentVersions?globalUpdateId={globalUpdateId}',
            getSelectedRowGlobalUpdateData: '/GlobalUpdate/GetSelectedRowGlobalUpdateData?globalUpdateId={globalUpdateId}',
            existingGUList: '/GlobalUpdate/GetExistsingGUGrid?tenantId=1',
            //iasWizard: '/GlobalUpdate/Index',
            addGlobalUpdate: '/GlobalUpdate/AddGlobalUpdate',
            //get Impacted Folders with Date Range given
            impactedfolderVersions: '/GlobalUpdate/GetIASImpactedFolderVersionList?GlobalUpdateID={GlobalUpdateID}&effectiveDateFrom={effectiveDateFrom}&effectiveDateTo={effectiveDateTo}&tenantId=1',
            //save IAS Folder data
            saveIASFolderDataValues: '/GlobalUpdate/SaveIASFolderDataValues',
            //save IAS Element Export data
            saveIASElementExportDataValues: '/GlobalUpdate/SaveIASElementExportDataValues',
            //Get Impacted Elements list
            getElementSelectionList: '/GlobalUpdate/GetElementSelection?GlobalUpdateID={GlobalUpdateID}',
            //Get Activity Grid
            getActivityGrid: '/GlobalUpdate/GetSelectedUIElementsList?globalUpdateId={globalUpdateId}&formDesignVersionId={formDesignVersionId}',
            //Get IAS Folder data
            getIASFolderDataList: '/GlobalUpdate/GetGlobalUpdatesIASFolderDataList?GlobalUpdateID={GlobalUpdateID}',
            //getUpdate Value Confirmation Info
            getConfirmationInfo: '/GlobalUpdate/UpdateValueConfirmationDetails?uiElementId={uiElementId}&formDesignVersionId={formDesignVersionId}&globalUpdateId={globalUpdateId}',
            //get UIelements for SearchGrid
            getSearchGridUIElementList: '/GlobalUpdate/GetUIElemetsForSearchGrid?tenantId=1&formDesignVersionId={formDesignVersionId}',
            //Schedule Global Update
            scheduleGlobalUpdate: '/GlobalUpdate/ScheduleGlobalUpdate'

        };

        this.globalUpdateInput = {
            globalUpdateName: '',
            effectiveDateFrom: '',
            effectiveDateTo: '',
            globalUpdateId: 0,
            iasWizardStepId: 0,
            globalUpdateStatusID: 0
        };

        this.formVersionInstance = {
            formDesignId: '',
            formDesignVersionId: 0,
            formDesignName: '',
            versionNumber: ''
        };

        this.isDesignVersionsExists = false;
        this.isGUInitialised = false;
        this.prevPropGrid = null;
        this.searchGridData = [];
        this.isInputData = true;
        this.formDesignVersionId;
        this.selectedElementsLength = 0;
        this.selectedElementsLengthPrevious = 0;

        function loadDesignVersionGrid() {
            //set url parameter
            var effectiveDateFrom = currentInstance.globalUpdateInput.effectiveDateFrom;
            //effectiveDateFromShortDate = effectiveDateFrom.toLocaleString().substring(0, effectiveDateFrom.toLocaleString().indexOf(' '));
            var effectiveDateTo = currentInstance.globalUpdateInput.effectiveDateTo;
            //effectiveDateToShortDate = effectiveDateTo.toLocaleString().substring(0, effectiveDateTo.toLocaleString().indexOf(' '));
            var documentDesignUrl = currentInstance.URLs.getDocumentDesignVersions.replace(/\{effectiveDateFrom\}/g, effectiveDateFrom).replace(/\{effectiveDateTo\}/g, effectiveDateTo);
            //set column list for grid
            var colArray = ['Document Design', 'Version', '', ''];
            //set column models
            var colModel = [];
            colModel.push({ name: 'FormDesignName', index: 'FormDesignName', editable: false, width: '220px' });
            colModel.push({ name: 'Version', index: 'Version', editable: false, align: 'right', width: '200px' });
            colModel.push({ name: 'FormDesignId', index: 'FormDesignId', hidden: true });
            colModel.push({ name: 'FormDesignVersionId', index: 'FormDesignVersionId', hidden: true, search: false, });
            //clean up the grid first - only table element remains after this
            $(currentInstance.elementIDs.documentDesignElementsJQ).jqGrid('GridUnload');
            $(currentInstance.elementIDs.documentDesignVersionJQ).jqGrid('GridUnload');

            //adding the pager element

            //adding the pager element
            $(currentInstance.elementIDs.documentDesignVersionJQ).parent().append("<div id='p" + currentInstance.elementIDs.documentDesignVersion + "'></div>");
            //load the jqGrid - refer documentation for details
            $(currentInstance.elementIDs.documentDesignVersionJQ).jqGrid({
                url: documentDesignUrl,
                datatype: 'json',
                cache: false,
                colNames: colArray,
                colModel: colModel,
                caption: 'Document Design Versions',
                height: '300',
                ignoreCase: true,
                loadonce: true,
                autowidth: true,
                viewrecords: true,
                hidegrid: true,
                altRows: true,
                shrinkToFit: false,
                pager: '#p' + currentInstance.elementIDs.documentDesignVersion,
                altclass: 'alternate',
                //load associated form design version grid on selecting a row
                onSelectRow: function (id) {
                    var mode = getQueryString("mode");
                    var row = $(this).getRowData(id);
                    loadDesignVersionElementsGrid(row);
                    if (mode == 'view') {
                        $(currentInstance.elementIDs.documentDesignElementsJQ).attr('disabled', 'disabled');
                        var grid = $(currentInstance.elementIDs.documentDesignElementsJQ);
                        $(grid[0].p.pager).find('td[id="#SelectElement"]').addClass('hide');
                    }
                },
                //on adding a new form design, reload the grid and set the row to selected
                loadComplete: function () {
                    var allRowsInGrid = $(this).getRowData();
                    if (allRowsInGrid.length > 0) {
                        $(currentInstance.elementIDs.documentDesignVersionJQ).jqGrid("setSelection", 1);
                    }
                    else {
                        $(currentInstance.elementIDs.documentDesignVersionJQ).jqGrid('GridUnload');
                    }
                    //Check if atleast one element is selected amongst all the documents
                    url = currentInstance.URLs.getSelectedDocumentVersions.replace(/\{globalUpdateId\}/g, currentInstance.globalUpdateInput.globalUpdateId);
                    var promise = ajaxWrapper.getJSON(url);
                    //register ajax success callback
                    promise.done(function (result) {
                        currentInstance.selectedElementsLengthPrevious = result.length;
                    });
                    //register ajax failure callback
                    promise.fail(showError);
                },
                resizeStop: function (width, index) {
                    autoResizing(currentInstance.elementIDs.documentDesignVersionJQ);
                }
            });
            var pagerElement = '#p' + currentInstance.elementIDs.documentDesignVersion;
            //remove default buttons
            $(currentInstance.elementIDs.documentDesignVersionJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        }
        function loadDesignVersionElementsGrid(documentVersionRow) {

            var formDesignId = documentVersionRow.FormDesignId;
            var formDesignVersionId = documentVersionRow.FormDesignVersionId;
            currentInstance.formDesignVersionId = formDesignVersionId;
            var formDesignName = documentVersionRow.FormDesignName;
            var versionNumber = documentVersionRow.Version;
            var documentDesignElementUrl = currentInstance.URLs.getDocumentVersionElementList.replace(/\{formDesignVersionId\}/g, formDesignVersionId).replace(/\{globalUpdateId\}/g, currentInstance.globalUpdateInput.globalUpdateId).replace(/\{isOnlySelectedElements\}/g, false);

            $(currentInstance.elementIDs.documentDesignElementsJQ).jqGrid('GridUnload');
            $(currentInstance.elementIDs.documentDesignElementsJQ).parent().append("<div id='p" + formDesignVersionId + "'></div>");
            //set column list
            var colArray = ['TenantId', 'UIElementID', 'UIElementName', 'Label', 'Element Type', 'Include'];

            //set column models
            var colModel = [];
            colModel.push({ name: 'TenantId', index: 'TenantId', hidden: true, search: false });
            colModel.push({ name: 'UIElementID', index: 'UIElementID', key: true, hidden: true, search: false });
            colModel.push({ name: 'UIElementName', index: 'UIElementName', hidden: true, align: 'center', search: false });
            colModel.push({ name: 'Label', index: 'Label', align: 'left', editable: false, width: '410px' });
            colModel.push({ name: 'ElementType', index: 'ElementType', align: 'left', editable: false, width: '200px' });
            colModel.push({ name: 'Include', index: 'Include', align: 'Center', width: '270px', align: 'center', editable: true, formatter: guAddCheckBoxFormatter, unformatter: guAddCheckBoxUnFormat, sortable: false });
            //load grid
            $(currentInstance.elementIDs.documentDesignElementsJQ).jqGrid({

                url: documentDesignElementUrl,
                datatype: 'json',
                treeGrid: true,
                treeGridModel: 'adjacency', // set this for hierarchical grid
                cache: false,
                colNames: colArray,
                colModel: colModel,
                caption: formDesignName + ' - ' + versionNumber + ' - UI Elements',
                autowidth: true,
                forceFit: true,
                loadonce: true,
                shrinkToFit: false,
                rowNum: 1000,
                height: '300',
                expanded: true,
                ExpandColumn: 'Label',
                pager: '#p' + formDesignVersionId,
                viewrecords: true,
                hidegrid: false,
                altRows: true,
                altclass: 'alternate',

                gridComplete: function () {
                    $(".treeclick", currentInstance.elementIDs.documentDesignElementsJQ).each(function () {
                        if ($(this).hasClass("tree-plus")) {
                            $(this).trigger("click");
                        }
                    });
                }

            });

            //set footer
            var pagerVersionElement = '#p' + documentVersionRow.FormDesignVersionId;
            $(currentInstance.elementIDs.documentDesignElementsJQ).jqGrid('navGrid', pagerVersionElement, { refresh: false, edit: false, add: false, del: false, search: false });
            $(pagerVersionElement).find(pagerVersionElement + '_center').remove();
            $(pagerVersionElement).find(pagerVersionElement + '_right').remove();
            $(currentInstance.elementIDs.documentDesignElementsJQ).jqGrid('navButtonAdd', pagerVersionElement,
                {
                    caption: 'Save', id: '#SelectElement',
                    onClickButton: function () {
                        var selectedUIElementList = new Array();
                        var allRowData = $(currentInstance.elementIDs.documentDesignElementsJQ).jqGrid('getRowData');
                        for (var i = 0 ; i < allRowData.length; i++) {
                            var UIElementID = allRowData[i].UIElementID;
                            var isChecked = $('#checkbox_' + allRowData[i].UIElementID + formDesignVersionId + '').prop('checked');
                            if (isChecked) {
                                selectedUIElementList.push(UIElementID);
                                currentInstance.selectedElementsLength = (selectedUIElementList.length);
                            }
                        }
                        if (selectedUIElementList.length == 0) {
                            currentInstance.selectedElementsLength = 0;
                        }
                        
                        //if (selectedUIElementList.length > 0) {
                            var formDesignSelectionelements = {
                                tenantID: 1,
                                formDesignId: formDesignId,
                                formDesignVersionId: formDesignVersionId,
                                globalUpdateId: currentInstance.globalUpdateInput.globalUpdateId,
                                selectedUIElementList: JSON.stringify(selectedUIElementList)
                            };
                            var formDesignUIElementDataList = formDesignSelectionelements;
                            var url = currentInstance.URLs.saveElementSelection;
                            var promise = ajaxWrapper.postJSON(url, formDesignUIElementDataList);
                            promise.done(function (xhr) {
                                if (xhr.Result === ServiceResult.SUCCESS) {
                                    appendGlyphiconCheckState('elementSelectionID');
                                    messageDialog.show(GlobalUpdateMessages.saveUIElementSuccess);
                                }
                                if (currentInstance.selectedElementsLengthPrevious > 0) {
                                    $.ajax({
                                        type: "POST", url: currentInstance.URLs.getSelectedDocumentVersions.replace(/\{globalUpdateId\}/g, currentInstance.globalUpdateInput.globalUpdateId),
                                        async: true,
                                        success: function (response) {
                                            if (response.length == 0) {
                                                currentInstance.selectedElementsLengthPrevious = 0;
                                            }
                                        }
                                    });
                                }
                            });
                        //}

                        //else {
                        //    messageDialog.show(GlobalUpdateMessages.addUIElementCondition);
                        //}
                    }
                });

            // Code for expand and collapse all
            $(currentInstance.elementIDs.documentDesignElementsJQ).jqGrid('navButtonAdd', pagerVersionElement,
                {
                    caption: "",
                    buttonicon: "ui-icon-triangle-2-n-s",
                    onClickButton: function () {
                        var allRowsInGrid = $(currentInstance.elementIDs.documentDesignElementsJQ).jqGrid('getGridParam', 'data');
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
                            $(".treeclick", currentInstance.elementIDs.documentDesignElementsJQ).each(function () {
                                if ($(this).hasClass("tree-plus")) {
                                    $(this).trigger("click");
                                }
                            });
                        } else {
                            $(".treeclick", currentInstance.elementIDs.documentDesignElementsJQ).each(function () {
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
        }
        function loadGlobalUpdateDocumentVersions() {

            var isDocumentExists = false;
            // $(currentInstance.prevPropGrid.gridElementId).jqGrid('GridUnload');
            $(currentInstance.elementIDs.selectedDocumentDesignVersionElementsJQ).jqGrid('GridUnload');
            if (currentInstance.prevPropGrid != null) {
                $(currentInstance.prevPropGrid.gridElementId).jqGrid('GridUnload');
            }
            $(currentInstance.elementIDs.documentVersionDropDownJQ).html('');
            url = currentInstance.URLs.getSelectedDocumentVersions.replace(/\{globalUpdateId\}/g, currentInstance.globalUpdateInput.globalUpdateId);
            var promise = ajaxWrapper.getJSON(url);
            //register ajax success callback
            promise.done(function (result) {
                if (result.length > 0) {
                    loadDocumentDesignVersion(result);
                }
            });
            //register ajax failure callback
            promise.fail(showError);

        }
        $(currentInstance.elementIDs.documentVersionDropDownJQ).on('change', function () {
            //added Seach Grid Information.
            var formDesignVersionId = $(currentInstance.elementIDs.documentVersionDropDownJQ).val();
            getSearchGridData(formDesignVersionId)
            // loadSelectedDesignVersionUIElementsGrid();
        });
        function loadSelectedDesignVersionUIElementsGrid() {
            var globalUpdateId = currentInstance.globalUpdateInput.globalUpdateId;
            var formDesignVersionId = $(currentInstance.elementIDs.documentVersionDropDownJQ).val();
            currentInstance.formDesignVersionId = formDesignVersionId;
            var formDesignVersionName = $(currentInstance.elementIDs.documentVersionDropDownJQ).find("option:selected").text();
            var selectedDocumentUIElementUrl = currentInstance.URLs.getDocumentVersionElementList.replace(/\{formDesignVersionId\}/g, formDesignVersionId).replace(/\{globalUpdateId\}/g, currentInstance.globalUpdateInput.globalUpdateId).replace(/\{isOnlySelectedElements\}/g, true);;
            //var currentInstance = this;
            //Removed SearchGridFrom Here
            //getSearchGridData(formDesignVersionId);
            $(currentInstance.elementIDs.selectedDocumentDesignVersionElementsJQ).jqGrid('GridUnload');
            $(currentInstance.elementIDs.selectedDocumentDesignVersionElementsJQ).parent().append("<div id='p" + globalUpdateId + "'></div>");
            //set column list
            var colArray = ['TenantId', 'UIElementID', 'UIElementName', 'Label', 'Element Type', 'Include'];
            //set column models
            var colModel = [];
            colModel.push({ name: 'TenantId', index: 'TenantId', hidden: true, search: false });
            colModel.push({ name: 'UIElementID', index: 'UIElementID', key: true, hidden: true, search: false });
            colModel.push({ name: 'UIElementName', index: 'UIElementName', hidden: true, align: 'center', search: false });
            colModel.push({ name: 'Label', index: 'Label', align: 'left', editable: false, width: '520px' });
            colModel.push({ name: 'ElementType', index: 'ElementType', align: 'left', editable: false, width: '245px' });
            colModel.push({ name: 'Include', index: 'Include', align: 'Center', hidden: true, width: '270px', align: 'center', editable: true, formatter: guAddCheckBoxFormatter, unformatter: guAddCheckBoxUnFormat, sortable: false });
            //load grid

            $(currentInstance.elementIDs.selectedDocumentDesignVersionElementsJQ).jqGrid({
                url: selectedDocumentUIElementUrl,
                datatype: 'json',
                treeGrid: true,
                treeGridModel: 'adjacency', // set this for hierarchical grid
                cache: false,
                colNames: colArray,
                colModel: colModel,
                caption: formDesignVersionName + ' - UI Elements',
                autowidth: true,
                forceFit: true,
                loadonce: true,
                shrinkToFit: false,
                rowNum: 1000,
                height: '290',
                expanded: true,
                ExpandColumn: 'Label',
                pager: '#p' + globalUpdateId,
                viewrecords: true,
                hidegrid: false,
                altRows: true,
                altclass: 'alternate',



                onSelectRow: function (id) {

                    var row = $(this).getRowData(id);
                    if (row) {
                        var isChecked = $('#checkbox_' + row.UIElementID + formDesignVersionId + '').prop('checked');
                        if (isChecked) {
                            var propGrid = new uiGuElementPropertyGrid(row, currentInstance.formDesignId, formDesignVersionId, currentInstance.searchGridData, globalUpdateId);
                            propGrid.loadPropertyGrid();
                            currentInstance.prevPropGrid = propGrid;
                        }
                        else {
                            return false;
                        }
                    }
                },

                loadComplete: function () {
                    var firstCheckedRowId;

                    var rowIds = $(currentInstance.elementIDs.selectedDocumentDesignVersionElementsJQ).jqGrid('getDataIDs');
                    $.each(rowIds, function (index, value) {
                        var firstCheckedRowData = $(currentInstance.elementIDs.selectedDocumentDesignVersionElementsJQ).jqGrid('getRowData', rowIds[index]);
                        if ($('#checkbox_' + firstCheckedRowData.UIElementID + formDesignVersionId + '').prop('checked')) {
                            firstCheckedRowId = rowIds[index];
                            return false;
                        }
                        // return firstCheckedRowId;
                    })

                    for (i = 1; i <= rowIds.length; i++) {//iterate over each row
                        rowData = $(currentInstance.elementIDs.selectedDocumentDesignVersionElementsJQ).jqGrid('getRowData', rowIds[i]);
                        //set background style if ColumnValue == true
                        var UIElementID = rowData.UIElementID;
                        var UIElementName = rowData.UIElementName;
                        var isChecked = $('#checkbox_' + UIElementID + formDesignVersionId + '').prop('checked');
                        if (!isChecked) {
                            $(currentInstance.elementIDs.selectedDocumentDesignVersionElementsJQ).jqGrid('setRowData', rowIds[i], false, 'myclass');
                        } //if
                    } //for

                    $(currentInstance.elementIDs.selectedDocumentDesignVersionElementsJQ).jqGrid("setSelection", firstCheckedRowId);
                },//loadComplete

                gridComplete: function () {
                    $(".treeclick", currentInstance.elementIDs.selectedDocumentDesignVersionElementsJQ).each(function () {
                        if ($(this).hasClass("tree-plus")) {
                            $(this).trigger("click");
                        }
                    });
                }

            });

            //set footer
            var pagerVersionElement = '#p' + globalUpdateId;
            $(currentInstance.elementIDs.selectedDocumentDesignVersionElementsJQ).jqGrid('navGrid', pagerVersionElement, { refresh: false, edit: false, add: false, del: false, search: false, save: false });
            //$(pagerVersionElement).find(pagerVersionElement + '_center').remove();
            //$(pagerVersionElement).find(pagerVersionElement + '_right').remove();

            //navbar Activity Grid Pop Up button
            $(currentInstance.elementIDs.selectedDocumentDesignVersionElementsJQ).jqGrid('navButtonAdd', pagerVersionElement,
                       {
                           caption: '', buttonicon: 'ui-icon-folder-open', title: ' Activity Grid', id: 'btnManageAccountEdit' + "view",
                           onClickButton: function () {
                               loadActivityGridPopUp(globalUpdateId, formDesignVersionId);
                           }
                       });
            // Code for expand and collapse all
            $(currentInstance.elementIDs.selectedDocumentDesignVersionElementsJQ).jqGrid('navButtonAdd', pagerVersionElement,
                {
                    caption: "",
                    buttonicon: "ui-icon-triangle-2-n-s",
                    onClickButton: function () {
                        var allRowsInGrid = $(currentInstance.elementIDs.selectedDocumentDesignVersionElementsJQ).jqGrid('getGridParam', 'data');
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
                            $(".treeclick", currentInstance.elementIDs.selectedDocumentDesignVersionElementsJQ).each(function () {
                                if ($(this).hasClass("tree-plus")) {
                                    $(this).trigger("click");
                                }
                            });
                        } else {
                            $(".treeclick", currentInstance.elementIDs.selectedDocumentDesignVersionElementsJQ).each(function () {
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
        }
        function loadActivityGridPopUp(globalUpdateId, formDesignVersionId) {
            $(currentInstance.elementIDs.activityGridDialogJQ).dialog(
                {
                    height: 520,
                    width: 800,
                    modal: true,
                    open: function () {
                        loadSelectedUIElementsGrid(globalUpdateId, formDesignVersionId);
                    }
                }
                );
        }
        function loadSelectedUIElementsGrid(globalUpdateId, formDesignVersionId) {
            //set column list for grid
            var colArray = ['GlobalUpdateID', 'FormDesignID', 'FormDesignVersionID', 'UIElementID', 'Element Name', 'Element Full Path', 'Is Value Updated'];
            var url;

            //set column models
            var colModel = [];
            colModel.push({ name: 'GlobalUpdateID', index: 'GlobalUpdateID', editable: false, hidden: true });
            colModel.push({ name: 'FormDesignID', index: 'FormDesignID', editable: false, align: 'left', hidden: true });
            colModel.push({ name: 'FormDesignVersionID', index: 'FormDesignVersionID', align: 'left', hidden: true });
            colModel.push({ name: 'UIElementID', index: 'UIElementID', align: 'left', hidden: true });
            colModel.push({ name: 'ElementHeaderName', index: 'ElementHeaderName', align: 'left' });
            colModel.push({ name: 'ElementFullPath', index: 'ElementFullPath', align: 'left' });
            colModel.push({ name: 'IsValueUpdated', index: 'IsValueUpdated', align: 'center' });
            //clean up the grid first - only table element remains after this
            $(currentInstance.elementIDs.activityGridJQ).jqGrid('GridUnload');
            //adding the pager element
            $(currentInstance.elementIDs.activityGridJQ).parent().append("<div id='p" + currentInstance.elementIDs.activityGrid + "'></div>");
            //load the jqGrid - refer documentation for details
            $(currentInstance.elementIDs.activityGridJQ).jqGrid({
                url: currentInstance.URLs.getActivityGrid.replace(/\{globalUpdateId\}/g, globalUpdateId).replace(/\{formDesignVersionId\}/g, formDesignVersionId),
                datatype: "json",
                cache: false,
                colNames: colArray,
                colModel: colModel,
                caption: 'Activity Grid',
                height: '350',
                rowNum: 20,
                rowList: [10, 20, 30],
                ignoreCase: true,
                loadonce: true,
                autowidth: true,
                viewrecords: true,
                hidegrid: true,
                altRows: true,
                onSelectRow: function (id) {
                    //if (id && id !== lastSel) {
                    //jQuery(this).restoreRow(lastSel);
                    //lastSel = id;
                    var rowid = $(currentInstance.elementIDs.activityGridJQ).jqGrid('getGridParam', 'selrow');
                    var rowData = $(currentInstance.elementIDs.activityGridJQ).getRowData(rowid);
                    //}
                    //jQuery(this).editRow(id, true);
                },
                pager: '#p' + currentInstance.elementIDs.activityGrid,
                sortname: 'FolderName',
                altclass: 'GlobalUpdateName',
                gridComplete: function () {
                }
            });

            var pagerElement = '#p' + currentInstance.elementIDs.activityGrid;
            //remove default buttons
            $(currentInstance.elementIDs.activityGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
            $(currentInstance.elementIDs.activityGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

            //navbar edit button
            $(currentInstance.elementIDs.activityGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon-pencil', title: 'Edit', id: 'btnManageAccountEdit',
                onClickButton: function () {
                    var rowId = $(this).getGridParam('selrow');
                    if (rowId !== undefined && rowId !== null) {
                        var row = $(this).getRowData(rowId);
                        $(currentInstance.elementIDs.activityGridDialogJQ).dialog('close');
                        $(currentInstance.elementIDs.selectedDocumentDesignVersionElementsJQ).jqGrid("setSelection", row.UIElementID);
                    }
                    else {
                        messageDialog.show(GlobalUpdateMessages.selectRowMessage);
                    }
                }
            });
        }
        function guAddCheckBoxFormatter(cellValue, options, rowObject) {
            var isElementSelectionEnabled = (!rowObject.AllowGlobalUpdates || rowObject.ElementType == 'Section' || rowObject.ElementType == 'Tab' || rowObject.ElementType == 'Repeater' || rowObject.ElementType == 'Label' || rowObject.ElementType == '[Blank]' || rowObject.IsKey) ? false : true;

            if (!isElementSelectionEnabled) {
                return "<input type='checkbox' disabled id='checkbox_" + rowObject.UIElementID + currentInstance.formDesignVersionId + "' name= 'checkbox_" + rowObject.UIElementID + currentInstance.formDesignVersionId + "'/>";
            }

            else {

                if (rowObject.isIncluded == false)
                    return "<input type='checkbox' id='checkbox_" + rowObject.UIElementID + currentInstance.formDesignVersionId + "' name= 'checkbox_" + rowObject.UIElementID + currentInstance.formDesignVersionId + "'/>";
                else
                    return "<input type='checkbox' id='checkbox_" + rowObject.UIElementID + currentInstance.formDesignVersionId + "' name= 'checkbox_" + rowObject.UIElementID + currentInstance.formDesignVersionId + "' checked/>";
            }
        }
        function guAddCheckBoxUnFormat(cellValue, options) {
            var result;
            result = $(this).find('#' + rowObject.UIElementID + currentInstance.formDesignVersionId).find('input').prop('checked');
            return result;
        }
        function guActionFormatter(cellValue, options, rowObject) {
            return "<span id = 'guDesignValueUpdate" + rowObject.FormDesignVersionId + "' class='ui-icon ui-icon-pencil edit' title = 'Update Element' style = 'cursor: pointer'/>";
        }
        function loadWizardElementData(globalIpdateId) {
            url = currentInstance.URLs.getSelectedRowGlobalUpdateData.replace(/\{globalUpdateId\}/g, globalIpdateId);
            var promise = ajaxWrapper.getJSON(url);
            //register ajax success callback
            promise.done(function (result) {
                var formDate = new Date(result[0].EffectiveDateFrom);
                formDate = (formDate.getMonth() + 1) + '/' + formDate.getDate() + '/' + formDate.getFullYear();
                var toDate = new Date(result[0].EffectiveDateTo);
                toDate = (toDate.getMonth() + 1) + '/' + toDate.getDate() + '/' + toDate.getFullYear();
                $(currentInstance.elementIDs.globalUpdateNameJQ).val(result[0].GlobalUpdateName);
                $(currentInstance.elementIDs.globalUpdateEffectiveFromJQ).val(formDate);
                $(currentInstance.elementIDs.globalUpdateEffectiveToJQ).val(toDate);

                currentInstance.globalUpdateInput.globalUpdateName = result[0].GlobalUpdateName;
                currentInstance.globalUpdateInput.effectiveDateFrom = formDate;
                currentInstance.globalUpdateInput.effectiveDateTo = toDate;
                //currentInstance.globalUpdateInput.globalUpdateId= ;

                elementSelectionWizardStep();
            });
            //register ajax failure callback
            promise.fail(showError);
        }
        function init() {
            $(document).ready(function () {
                var mode = getQueryString("mode");

                $("#globalUpdateNameHelpBlock").text("*");
                $("#EffectiveDateFromHelpBlock").text("*");
                $("#EffectiveDateToHelpBlock").text("*");
                if (mode == 'add') {
                    $(currentInstance.elementIDs.anchorNextJQ).css("background-color", "lightgray");
                    InitializeControls();
                }

                //Enable elements on filling value of previous element
                $(currentInstance.elementIDs.globalUpdateNameJQ).change(function () {
                    $(currentInstance.elementIDs.globalUpdateEffectiveFromJQ).removeAttr("disabled");
                    $(currentInstance.elementIDs.clearGlobalUpdateJQ).removeAttr("disabled");
                    if ($(currentInstance.elementIDs.globalUpdateNameJQ).val().trim() == "" || $(currentInstance.elementIDs.globalUpdateEffectiveFromJQ).val() == "" || $(currentInstance.elementIDs.globalUpdateEffectiveToJQ).val() == "") {
                        return $(currentInstance.elementIDs.addGlobalUpdateJQ).attr("disabled", 'disabled');
                    }
                    else {
                        return $(currentInstance.elementIDs.addGlobalUpdateJQ).removeAttr("disabled");
                    }
                });

                $(currentInstance.elementIDs.globalUpdateEffectiveFromJQ).change(function () {
                    $(currentInstance.elementIDs.globalUpdateEffectiveToJQ).removeAttr("disabled");
                    var effDateFrom = $(currentInstance.elementIDs.globalUpdateEffectiveFromJQ).val();
                    var effDatTo = $(currentInstance.elementIDs.globalUpdateEffectiveToJQ).val();
                    if (Date.parse(effDateFrom) <= Date.parse(effDatTo)) {
                        if ($(currentInstance.elementIDs.globalUpdateNameJQ).val().trim() == "" || effDateFrom == "" || effDatTo == "")
                            return $(currentInstance.elementIDs.addGlobalUpdateJQ).attr("disabled", 'disabled');
                        else
                            return $(currentInstance.elementIDs.addGlobalUpdateJQ).removeAttr("disabled");
                        }
                    else if (effDateFrom == "") {
                        $(currentInstance.elementIDs.addGlobalUpdateJQ).attr("disabled", 'disabled');
                        messageDialog.show(GlobalUpdateMessages.fromDateNotBlank);
                    }
                    else {
                        $(currentInstance.elementIDs.addGlobalUpdateJQ).attr("disabled", 'disabled');
                        if (effDatTo != "") {
                            messageDialog.show(GlobalUpdateMessages.formDateShouldLesser);
                        }
                    }
                });
                $(currentInstance.elementIDs.globalUpdateEffectiveToJQ).change(function () {
                    //Validations for GU Eff Date from and GU Eff To
                    var effDateFrom = $(currentInstance.elementIDs.globalUpdateEffectiveFromJQ).val();
                    var effDatTo = $(currentInstance.elementIDs.globalUpdateEffectiveToJQ).val();
                    if (Date.parse(effDateFrom) <= Date.parse(effDatTo)) {
                        if ($(currentInstance.elementIDs.globalUpdateNameJQ).val().trim() == "" || effDateFrom == "" || effDatTo == "")
                            return $(currentInstance.elementIDs.addGlobalUpdateJQ).attr("disabled", 'disabled');
                        else
                            return $(currentInstance.elementIDs.addGlobalUpdateJQ).removeAttr("disabled");
                    }
                    else if (effDatTo == "") {
                        $(currentInstance.elementIDs.addGlobalUpdateJQ).attr("disabled", 'disabled');
                        messageDialog.show(GlobalUpdateMessages.toDateNotBlank);
                    }
                    else {
                        $(currentInstance.elementIDs.addGlobalUpdateJQ).attr("disabled", 'disabled');
                        if (effDateFrom != "") {
                            messageDialog.show(GlobalUpdateMessages.toDateShouldGreater);
                        }
                    }
                });

                $(currentInstance.elementIDs.clearGlobalUpdateJQ).click(function () {
                    //Initialize controls on click of Clear button
                    clearAll();
                    InitializeControls();
                });

                //Call bootstrap wizard events
                bootStrapWizardEvents();

                //Set Datepicker to Input            
                setDatePickerForInputType(currentInstance.elementIDs.globalUpdateEffectiveFromJQ);
                setDatePickerForInputType(currentInstance.elementIDs.globalUpdateEffectiveToJQ);
                //Set Inputs Ehrn redirecting to Setup in case of Edit
                if (mode == 'edit') {
                    var rowId = getQueryString('rowId');
                    //loadWizardElementData(rowId);
                    setGlobalUpdateInputForEdit();
                    //Redirect to tab with corresponding Global Update Status Id in case of Edit
                    $(currentInstance.elementIDs.rootwizard).bootstrapWizard('show', ((currentInstance.globalUpdateInput.iasWizardStepId) - 1));
                    //
                    $(currentInstance.elementIDs.panelHeaderJQ).text('Edit Global Update');
                    $(currentInstance.elementIDs.addGlobalUpdateJQ).text('Edit');
                }
                else if (mode == 'view') {
                    $(currentInstance.elementIDs.panelHeaderJQ).text('View Global Update');
                }
                else {
                    $(currentInstance.elementIDs.panelHeaderJQ).text('Add Global Update');
                }
                ////load Existing Batches Grid
                //loadBatchesGrid();
            });
        }
        function clearAll() {
            $(currentInstance.elementIDs.globalUpdateNameJQ).val('');
            $(currentInstance.elementIDs.globalUpdateEffectiveFromJQ).val('');
            $(currentInstance.elementIDs.globalUpdateEffectiveToJQ).val('');
            $(currentInstance.elementIDs.globalUpdateNameHeaderJQ).val('');
            $(currentInstance.elementIDs.globalUpdateEffectivePeriodJQ).val('');
        }

        function setGlobalUpdateInputForEdit() {
            var effectiveDateFrom = currentInstance.effectiveDateFrom;
            var effectiveDateFromShortDate = effectiveDateFrom.toLocaleString().substring(0, effectiveDateFrom.toLocaleString().indexOf(' '));
            currentInstance.globalUpdateInput.effectiveDateFrom = effectiveDateFromShortDate;

            var effectiveDateTo = currentInstance.effectiveDateTo;
            var effectiveDateToShortDate = effectiveDateTo.toLocaleString().substring(0, effectiveDateTo.toLocaleString().indexOf(' '));
            currentInstance.globalUpdateInput.effectiveDateTo = effectiveDateToShortDate;

            currentInstance.globalUpdateInput.globalUpdateId = currentInstance.globalUpdateId;
            currentInstance.globalUpdateInput.globalUpdateName = currentInstance.globalUpdateName;
            currentInstance.globalUpdateInput.globalUpdateStatusID = currentInstance.globalUpdateStatusID;
            currentInstance.globalUpdateInput.iasWizardStepId = currentInstance.wizardStepsID;

            $(currentInstance.elementIDs.globalUpdateNameJQ).val(currentInstance.globalUpdateName);
            //$(currentInstance.elementIDs.globalUpdateNameJQ).attr('disabled', 'disable');
            $(currentInstance.elementIDs.globalUpdateEffectiveFromJQ).val(effectiveDateFromShortDate);
            $(currentInstance.elementIDs.globalUpdateEffectiveToJQ).val(effectiveDateToShortDate);
        }
        function showGlobalWizardTabAndNavigation(tab, navigation, index) {
            ShowGlobalWizardTab(tab, navigation, index)
            if ((index == 0)) {
                //Hide Previous button for 1st two tabs
                $(".previous").addClass('hide');
            }
            else if ((index == 1) || (index == 2) || (index == 3)) {
                $(".previous").removeClass('hide');
                $(".next").removeClass('hide');
            }
            if (index == 3) {
                $(".next").addClass('hide');
            }
        }

        //function findSelectedElementsLength(selectedElementsLength) {
        //    //var selectedElementsLength;
        //    //Check if atleast one element is selected amongst all the documents
        //    url = currentInstance.URLs.getSelectedDocumentVersions.replace(/\{globalUpdateId\}/g, currentInstance.globalUpdateInput.globalUpdateId);
        //    var promise = ajaxWrapper.getJSON(url);
        //    //register ajax success callback
        //    promise.done(function (result) {
        //        if (result.length == 0) {
        //            selectedElementsLength = result.length;
        //        }
        //    });
        //    //register ajax failure callback
        //    promise.fail(showError);
        //    //return selectedElementsLength;
        //}

        function appendGlyphiconCheckState(elementId) {
            $('#span_' + elementId).remove();
            //$('#' + elementId).append('<span id="span_'+ elementId +'" class="glyphicon glyphicon-check workflow-graphic-icon text-green"></span>');
        }

        function bootStrapWizardEvents() {
            $(currentInstance.elementIDs.rootwizard).bootstrapWizard({
                onTabShow: function (tab, navigation, index) {
                    var mode = getQueryString("mode");
                    if (mode == 'edit') {
                        if (index == 0) {
                            $(currentInstance.elementIDs.globalUpdateNameHeaderJQ).text('');
                            $(currentInstance.elementIDs.globalUpdateEffectivePeriodJQ).text('');
                        }
                        if (currentInstance.isEditInitialised == false) {
                            //Set values of Global Update Input with the values passed as params 
                            setGlobalUpdateInputForEdit();
                            currentInstance.isEditInitialised = true;
                        }
                        //Show Particaular Tab
                        showGlobalWizardTabAndNavigation(tab, navigation, index);
                    }
                    else if (mode == 'view') {
                        if (index == 0) {
                            $(currentInstance.elementIDs.globalUpdateNameHeaderJQ).text('');
                            $(currentInstance.elementIDs.globalUpdateEffectivePeriodJQ).text('');
                        }
                        DisableControlsForViewMode();
                        if (currentInstance.isEditInitialised == false) {
                            //Set values of Global Update Input with the values passed as params 
                            setGlobalUpdateInputForEdit();
                            currentInstance.isEditInitialised = true;
                        }
                        //Show Particaular Tab
                        showGlobalWizardTabAndNavigation(tab, navigation, index);
                    }
                    else {
                        showGlobalWizardTabAndNavigation(tab, navigation, index);
                    }

                },
                onNext: function (tab, navigation, index) {
                    var mode = getQueryString("mode");
                    var wzStep = $('#workflowdivJQ_GU').find('.active').text().toString().trim();
                    if (currentInstance.isInputData == false) {
                        if (wzStep == 'Set Up')
                            messageDialog.show("Please update required fields!!");
                        else if (wzStep == 'Element Selection')
                            messageDialog.show(GlobalUpdateMessages.addUIElementCondition);
                        return false;
                    }
                    if (index == 2) {
                        //Check if atleast one element is selected amongst all the documents
                        if ((currentInstance.selectedElementsLength == 0) && (currentInstance.selectedElementsLengthPrevious == 0)) {
                            messageDialog.show(GlobalUpdateMessages.addUIElementCondition);
                            return false;
                        }
                        
                    } else {
                        var stringUtility = new globalutilities();
                        if (mode == 'add') {
                            if ((index == 1)) {
                                //Disable Next button for 1st tab when Global Update is not added.
                                if (!((currentInstance.globalUpdateInput.globalUpdateName == $(currentInstance.elementIDs.globalUpdateNameJQ).val())
                                    && (!(stringUtility.stringMethods.isNullOrEmpty(currentInstance.globalUpdateInput.globalUpdateName)))
                                     && (!(stringUtility.stringMethods.isNullOrEmpty($(currentInstance.elementIDs.globalUpdateNameJQ).val())))
                                    && currentInstance.isDesignVersionsExists == true)) {
                                    return false;
                                }
                                else {
                                    //Enable Next button for 1st tab when Global Update is added successfully.
                                    return true;
                                }
                            }
                            else {
                                //Enable Next button for tabs other than 1st when Global Update is added successfully.
                                return true;
                            }
                        }
                        else {
                            if (currentInstance.isInputData == false) {
                                return false;
                            }
                            else {
                                return true;
                            }
                        }
                    }
                },
                onLast: function (tab, navigation, index) {
                    var wzStep = $('#workflowdivJQ_GU').find('.active').text().toString().trim();
                    if (currentInstance.isInputData == false) {
                        if (wzStep == 'Set Up')
                            messageDialog.show("Please update required fields!!");
                        else if (wzStep == 'Element Selection')
                            messageDialog.show(GlobalUpdateMessages.addUIElementCondition);
                        
                        return false;
                    }
                    //Check if atleast one element is selected amongst all the documents
                    if ((wzStep =='Set Up' || wzStep == 'Element Selection') && getQueryString("mode") != "view" ) {
                        if ((currentInstance.selectedElementsLength == 0) && (currentInstance.selectedElementsLengthPrevious == 0)) {
                            messageDialog.show(GlobalUpdateMessages.addUIElementCondition);
                            return false;
                        }
                    }
                },
                onPrevious: function (tab, navigation, index) {

                },
                //Avoid showing the tab on tab click event 
                onTabClick: function (tab, navigation, index) {
                    return false;
                },
                show: 3
            });
        }
        function DisableControlsForViewMode() {
            //Setup Tab
            $(currentInstance.elementIDs.globalUpdateNameJQ).attr('disabled', 'disable');
            $(currentInstance.elementIDs.globalUpdateEffectiveFromJQ).attr('disabled', 'disable');
            $(currentInstance.elementIDs.globalUpdateEffectiveToJQ).attr('disabled', 'disable');
            $(currentInstance.elementIDs.addGlobalUpdateJQ).addClass('hide');
            $(currentInstance.elementIDs.clearGlobalUpdateJQ).addClass('hide');
            //Element Selection Tab
            $(currentInstance.elementIDs.documentDesignElementsJQ).attr('disabled', 'disabled');
            //Generate IAS Tab
            $(currentInstance.elementIDs.globalUpdateImpactedFoldersGridJQ).attr('disabled', 'disabled');
            $(currentInstance.elementIDs.btnGenerateJQ).addClass('hide');
        }
        function InitializeControls() {
            $(currentInstance.elementIDs.globalUpdateNameJQ).removeAttr("disabled");
            $(currentInstance.elementIDs.globalUpdateEffectiveFromJQ).attr("disabled", 'disabled');
            $(currentInstance.elementIDs.globalUpdateEffectiveToJQ).attr("disabled", 'disabled');
            $(currentInstance.elementIDs.addGlobalUpdateJQ).attr("disabled", 'disabled');
            $(currentInstance.elementIDs.clearGlobalUpdateJQ).attr("disabled", 'disabled');
            $(currentInstance.elementIDs.anchorNext).attr("disabled", 'disabled');
            $(currentInstance.elementIDs.anchorNextJQ).css("background-color", "lightgray");
            currentInstance.isInputData = false;
        }

        function formatDateLeadingZeros(date) {
            var stringUtility = new globalutilities();
            if (!(stringUtility.stringMethods.isNullOrEmpty(date))) {
                splitedDate = date.split("/");
                for (i = 0; i < splitedDate.length; i++) {
                    if (splitedDate[i].length != 2) {
                        if (splitedDate[i].length == 1) {
                            splitedDate[i] = "0" + splitedDate[i];
                        }
                    }

                }
                leadZeroesdate = splitedDate.join('/');
                return leadZeroesdate;
            }
            else {
                return date;
            }
        }

        $(currentInstance.elementIDs.addGlobalUpdateJQ).click(function () {
            //   var addEditGlobalUpdateFlag = true;
            var globalUpdateName = $(currentInstance.elementIDs.globalUpdateNameJQ).val().trim();
            var enteredGlobalUpdateEffectiveFrom = formatDateLeadingZeros($(currentInstance.elementIDs.globalUpdateEffectiveFromJQ).val());
            var enteredGlobalUpdateEffectiveTo = formatDateLeadingZeros($(currentInstance.elementIDs.globalUpdateEffectiveToJQ).val());
            var prevEffectiveDateFrom = formatDateLeadingZeros(currentInstance.globalUpdateInput.effectiveDateFrom);
            var prevEffectiveDateTo = formatDateLeadingZeros(currentInstance.globalUpdateInput.effectiveDateTo);
            //Start
            if ((currentInstance.globalUpdateInput.globalUpdateId != 0) && currentInstance.globalUpdateInput.globalUpdateId != undefined) {
                if (((globalUpdateName != "") && (enteredGlobalUpdateEffectiveFrom != "") && (enteredGlobalUpdateEffectiveTo != "")) && (enteredGlobalUpdateEffectiveFrom != prevEffectiveDateFrom) || (enteredGlobalUpdateEffectiveTo != prevEffectiveDateTo)) {
                    if (Date.parse(enteredGlobalUpdateEffectiveFrom) <= Date.parse(enteredGlobalUpdateEffectiveTo)) {
                        $(currentInstance.elementIDs.editGlobalUpdateconfirmDialog).find('div p').text(GlobalUpdateMessages.editGlobalUpdateConfirmation);
                        $(currentInstance.elementIDs.editGlobalUpdateconfirmDialog).dialog({
                            title: 'Confirm Global Update',
                            height: 120,
                            buttons: {
                                Yes: function () {
                                    // addEditGlobalUpdateFlag = true;
                                    AddEditGlobalUpdate(globalUpdateName, enteredGlobalUpdateEffectiveFrom, enteredGlobalUpdateEffectiveTo);
                                    $(this).dialog("close");
                                },
                                No: function () {
                                    // addEditGlobalUpdateFlag = false;
                                    $(this).dialog("close");
                                }
                            }
                        });
                    }
                    else {
                        messageDialog.show(GlobalUpdateMessages.toDateShouldGreater);
                    }
                }
                else if (globalUpdateName == "") {
                    messageDialog.show(GlobalUpdateMessages.globalUpdateNameNotBlank);
                }
                else if (enteredGlobalUpdateEffectiveFrom == "") {
                    messageDialog.show(GlobalUpdateMessages.formDateNotBlank);
                }
                else if (enteredGlobalUpdateEffectiveTo == "") {
                    messageDialog.show(GlobalUpdateMessages.toDateNotBlank);
                }
                else if (globalUpdateName != currentInstance.globalUpdateInput.globalUpdateName) {
                    AddEditGlobalUpdate(globalUpdateName, enteredGlobalUpdateEffectiveFrom, enteredGlobalUpdateEffectiveTo);
                }
                else {
                    // addEditGlobalUpdateFlag = false;
                    messageDialog.show(GlobalUpdateMessages.globalUpdateEditCheck);
                }
            }//End
            else {
                AddEditGlobalUpdate(globalUpdateName, enteredGlobalUpdateEffectiveFrom, enteredGlobalUpdateEffectiveTo);
            }
        })

        function AddEditGlobalUpdate(globalUpdateName, globalUpdateEffectiveFrom, globalUpdateEffectiveTo) {
            var globalUpdateInitialValues = [];
            if (currentInstance.globalUpdateInput.globalUpdateId != 0 && currentInstance.globalUpdateInput.globalUpdateId != undefined) {
                globalUpdateInitialValues = $.extend({}, currentInstance.globalUpdateInput);// currentInstance.globalUpdateInput.slice();
            }

            currentInstance.globalUpdateInput.globalUpdateId = currentInstance.globalUpdateInput.globalUpdateId;
            currentInstance.globalUpdateInput.globalUpdateName = globalUpdateName;
            currentInstance.globalUpdateInput.effectiveDateFrom = globalUpdateEffectiveFrom;
            currentInstance.globalUpdateInput.effectiveDateTo = globalUpdateEffectiveTo;

            var url = currentInstance.URLs.addGlobalUpdate;
            var promise = ajaxWrapper.postJSON(url, currentInstance.globalUpdateInput);
            promise.done(function (xhr) {
                if (xhr.Result === ServiceResult.SUCCESS) {
                    appendGlyphiconCheckState('setUpID');
                    $(currentInstance.elementIDs.anchorNextJQ).css("background-color", "#428bca");

                    messageDialog.show(GlobalUpdateMessages.guAddSuccess);
                    currentInstance.isInputData = true;
                    if (xhr.Items.length > 0 && xhr.Items[0].Messages[0] != undefined && xhr.Items[0].Messages[0] != null) {
                        //set currentInstance.globalUpdateInput object
                        setGlobalUpdateInput(xhr);
                        currentInstance.isDesignVersionsExists = true;
                        $(currentInstance.elementIDs.rootwizard).bootstrapWizard('show', (GlobalUpdateWizard.ElementSelectionStepId - 1));
                    }
                }
                else if (xhr.Result === ServiceResult.FAILURE) {
                    messageDialog.show(xhr.Items[0].Messages);
                    var mode = getQueryString("mode");
                    if (mode == 'edit') {
                        currentInstance.globalUpdateInput = globalUpdateInitialValues;
                    }
                    if (currentInstance.globalUpdateInput.globalUpdateId != 0 && currentInstance.globalUpdateInput.globalUpdateId != undefined) {
                        $(currentInstance.elementIDs.globalUpdateNameJQ).val(currentInstance.globalUpdateInput.globalUpdateName);
                        $(currentInstance.elementIDs.globalUpdateEffectiveFromJQ).val(currentInstance.globalUpdateInput.effectiveDateFrom);
                        $(currentInstance.elementIDs.globalUpdateEffectiveToJQ).val(currentInstance.globalUpdateInput.effectiveDateTo);
                    }
                    currentInstance.isDesignVersionsExists = false;
                }
                else {
                    messageDialog.show(Common.errorMsg);
                }
            });
            promise.fail(showError);
        }

        $(currentInstance.elementIDs.clearGlobalUpdateJQ).click(function () {
            // Instruction : Clear the inputs   
            $(currentInstance.elementIDs.globalUpdateNameJQ).val('');
            $(currentInstance.elementIDs.globalUpdateEffectiveFromJQ).val('');
            $(currentInstance.elementIDs.globalUpdateEffectiveToJQ).val('');
        });
        
        $(currentInstance.elementIDs.btnGenerateJQ).click(function (e) {
            var GlobalUpdateID = currentInstance.globalUpdateInput.globalUpdateId;
            var parameters = {
                GlobalUpdateID: GlobalUpdateID
            };
            var impactedFolderCount = $(currentInstance.elementIDs.globalUpdateImpactedFoldersGridJQ).getRowData().length;
            if (impactedFolderCount == 0) {
                messageDialog.show(GlobalUpdateMessages.scheduleGlobalUpdateFailed);
                return false;
            }
            //ajax POST of element properties
            var promise = ajaxWrapper.postJSONCustom(currentInstance.URLs.scheduleGlobalUpdate, parameters);
            promise.done(function (res) {
                if (res.Result === ServiceResult.SUCCESS) {
                    appendGlyphiconCheckState('generateIASID');
                    messageDialog.show(GlobalUpdateMessages.scheduleGlobalUpdateSuccess);
                }
                else {
                    messageDialog.show(GlobalUpdateMessages.saveIASErrorMsg);
                }
            });
            //register ajax failure callback
            promise.fail(showError);
        });

        function setGlobalUpdateInput(xhr) {
            currentInstance.globalUpdateInput.globalUpdateId = xhr.Items[0].Messages[0];
            currentInstance.globalUpdateInput.globalUpdateName = xhr.Items[0].Messages[1];
            currentInstance.globalUpdateInput.effectiveDateFrom = xhr.Items[0].Messages[2];
            currentInstance.globalUpdateInput.effectiveDateTo = xhr.Items[0].Messages[3];
        }
        //Grid for Impacted Folders for IAS Generation
        function loadImpactedFoldersGrid() {
            var GlobalUpdateID = currentInstance.globalUpdateInput.globalUpdateId;
            var effectiveDateFrom = currentInstance.globalUpdateInput.effectiveDateFrom;
            var effectiveDateTo = currentInstance.globalUpdateInput.effectiveDateTo;

            //set column list for grid
            var colArray = ['Account/Portfolio', '', 'Folder Name', '', 'Folder Version Number', 'Effective Date', '', 'Document Name', 'Owner'];

            var url;
            //set column models
            var colModel = [];
            colModel.push({ name: 'AccountName', index: 'AccountName', editable: false, align: 'left' });
            colModel.push({ name: 'FolderID', index: 'FolderID', key: true, editable: false, hidden: true });
            colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left' });
            colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', editable: false, hidden: true });
            colModel.push({ name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: false, align: 'right' });
            colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, align: 'center' });
            colModel.push({ name: 'FormInstanceID', index: 'FormInstanceID', editable: false, hidden: true });
            colModel.push({ name: 'FormName', index: 'FormName', editable: false, align: 'left' });
            colModel.push({ name: 'Owner', index: 'Owner', editable: false, align: 'left' });

            //clean up the grid first - only table element remains after this
            $(currentInstance.elementIDs.globalUpdateImpactedFoldersGridJQ).jqGrid('GridUnload');
            //added to beforeProcessing must work
            $(currentInstance.elementIDs.globalUpdateImpactedFoldersGridJQ).jqGrid("destroyFilterToolbar");

            url = currentInstance.URLs.impactedfolderVersions.replace(/\{GlobalUpdateID\}/g, GlobalUpdateID).replace(/\{effectiveDateFrom\}/g, effectiveDateFrom).replace(/\{effectiveDateTo}/g, effectiveDateTo);

            //adding the pager element
            $(currentInstance.elementIDs.globalUpdateImpactedFoldersGridJQ).parent().append("<div id='p" + currentInstance.elementIDs.globalUpdateImpactedFoldersGrid + "'></div>");
            //load the jqGrid - refer documentation for details
            $(currentInstance.elementIDs.globalUpdateImpactedFoldersGridJQ).jqGrid({
                url: url,
                datatype: 'json',
                cache: false,
                colNames: colArray,
                colModel: colModel,
                caption: 'Impacted Folders',
                height: '200',
                rowNum: 20,
                rowList: [10, 20, 30],
                ignoreCase: true,
                loadonce: true,
                autowidth: true,
                viewrecords: true,
                hidegrid: false,
                altRows: true,
                pager: '#p' + currentInstance.elementIDs.globalUpdateImpactedFoldersGrid,
                sortname: 'FolderName',
                altclass: 'alternate',
                beforeProcessing: function (data) {
                    $.each(data, function (i, dt) {
                        var selectedRowData = dt;
                        //date format change logic
                        var d = new Date(selectedRowData.EffectiveDate);
                        var month = d.getMonth().toString().length > 1 ? (d.getMonth() + 1) : "0" + (d.getMonth() + 1).toString();
                        var day = d.getDate().toString().length > 1 ? d.getDate() : "0" + d.getDate().toString();
                        var year = d.getFullYear().toString();
                        var formattedDate = month + "/" + day + "/" + year;
                        selectedRowData.EffectiveDate = formattedDate;
                    });
                }
            });

            var pagerElement = '#p' + currentInstance.elementIDs.globalUpdateImpactedFoldersGrid;
            //remove default buttons
            $(currentInstance.elementIDs.globalUpdateImpactedFoldersGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
            // add filter toolbar to the top
            $(currentInstance.elementIDs.globalUpdateImpactedFoldersGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

        }

        function showError(xhr) {
            if (xhr.status == 999)
                this.location = '/Account/LogOn';
            else
                messageDialog.show(JSON.stringify(xhr));
        }

        function ShowGlobalWizardTab(tab, navigation, index) {

            var currentStep = navigation.find('li.active');
            var wizardStepId = $(currentStep).attr('id');

            if (wizardStepId == GlobalUpdateWizard.ElementSelection) {
                elementSelectionWizardStep();
            }

            if (wizardStepId == GlobalUpdateWizard.UpdateSelection) {
                updatedDocumentsVersions();
            }

            if (wizardStepId == GlobalUpdateWizard.GenerateIAS) {
                iasGenerationWizardStep();
            }

        }

        function elementSelectionWizardStep() {
            $(currentInstance.elementIDs.globalUpdateNameHeaderJQ).text(currentInstance.globalUpdateInput.globalUpdateName).css('font-weight', 'Bold');
            $(currentInstance.elementIDs.globalUpdateEffectivePeriodJQ).text('Effective Period : ' + currentInstance.globalUpdateInput.effectiveDateFrom + ' - ' + currentInstance.globalUpdateInput.effectiveDateTo);
            loadDesignVersionGrid();
        }

        function iasGenerationWizardStep() {
            $(currentInstance.elementIDs.globalUpdateNameHeaderJQ).text(currentInstance.globalUpdateInput.globalUpdateName).css('font-weight', 'Bold');
            $(currentInstance.elementIDs.globalUpdateEffectivePeriodJQ).text('Effective Period : ' + currentInstance.globalUpdateInput.effectiveDateFrom + ' - ' + currentInstance.globalUpdateInput.effectiveDateTo);
            loadImpactedFoldersGrid();
        }

        function updatedDocumentsVersions() {
            $(currentInstance.elementIDs.globalUpdateNameHeaderJQ).text(currentInstance.globalUpdateInput.globalUpdateName).css('font-weight', 'Bold');
            $(currentInstance.elementIDs.globalUpdateEffectivePeriodJQ).text('Effective Period : ' + currentInstance.globalUpdateInput.effectiveDateFrom + ' - ' + currentInstance.globalUpdateInput.effectiveDateTo);
            loadGlobalUpdateDocumentVersions();
        }

        function loadDocumentDesignVersion(data) {
            $(currentInstance.elementIDs.documentVersionDropDownJQ).html('');
            //$(currentInstance.elementIDs.documentVersionDropDownJQ).append("<option value='0'>Select Document version</option>");

            if (data != null && data.length != 0) {
                currentInstance.formDesignId = data[0].FormDesignId;
                for (var i = 0; i < data.length; i++) {
                    var key = data[i].FormDesignVersionId;
                    var Value = data[i].FormDesignName + '-' + data[i].Version;
                    $(currentInstance.elementIDs.documentVersionDropDownJQ).append("<option value=" + key + ">" + Value + "</option>");
                }
                var firstElementKey = data[0].FormDesignVersionId;
                $(currentInstance.elementIDs.documentVersionDropDownJQ).val(firstElementKey);
                getSearchGridData(firstElementKey)
                //loadSelectedDesignVersionUIElementsGrid();

            }
        }

        //return data of this grid to the caller
        //function getGridData() {
        //    var gridElementId = currentInstance.elementIDs.selectedDocumentDesignVersionElementsJQ;
        //    return $(gridElementId);
        //}


        function getSearchGridData(formDesignVersionId) {

            var searchGridUrl = currentInstance.URLs.getSearchGridUIElementList.replace(/\{formDesignVersionId\}/g, formDesignVersionId);
            var promise = ajaxWrapper.getJSON(searchGridUrl);
            //register ajax success callback
            promise.done(function (result) {
                currentInstance.searchGridData = result;
                loadSelectedDesignVersionUIElementsGrid();
            });
            //register ajax failure callback
            promise.fail(showError);
        }


        return {
            load: function () {
                init();
            }
        }
    }

    return {
        getInstance: function (effectiveDateFrom, effectiveDateTo, globalUpdateName, wizardStepsID, globalUpdateStatusID, globalUpdateID) {
            if (instance === undefined) {
                instance = new globalUpdate(effectiveDateFrom, effectiveDateTo, globalUpdateName, wizardStepsID, globalUpdateStatusID, globalUpdateID);
            }
            else if (instance.effectiveDateFrom == effectiveDateFrom && instance.effectiveDateTo == effectiveDateTo && instance.globalUpdateName == globalUpdateName) {
                return instance;
            }
            else {
                instance = undefined;
                instance = new globalUpdate(effectiveDateFrom, effectiveDateTo, globalUpdateName, wizardStepsID, globalUpdateStatusID, globalUpdateID);
            }
            return instance;
        }
    }

}();

var existingGrid = function () {
    this.elementIDs = {
        existingUpdatesGrid: "existingUpdatesGrid",
        existingUpdatesGridJQ: "#existingUpdatesGrid",
        executionStatusHelpDialogJQ: '#executionStatusHelpDialog',
        ExecutionStatusHelp: 'ExecutionStatusHelp',
        ExecutionStatusHelpJQ: '#ExecutionStatusHelp',
        btnManageGlobalUpdateAddJQ: '#btnManageGlobalUpdateAdd',
        btnManageGlobalUpdateEditJQ: '#btnManageGlobalUpdateEdit',
        btnManageGlobalUpdateUploadJQ: '#btnManageGlobalUpdateUpload',
        btnManageGlobalUpdateCopyJQ: '#btnManageGlobalUpdateCopy'
    };
    this.URLs = {
        //Get List of existing GU
        batchExe: '/GlobalUpdate/BatchExecution',
        existingGUList: '/GlobalUpdate/GetExistsingGUGrid',
        iasWizard: '/GlobalUpdate/Index',
        isValidIASUpload: '/GlobalUpdate/IsValidIASUpload?GlobalUpdateID={GlobalUpdateID}',
        getLatestGlobalUpdateStatus: '/GlobalUpdate/RefreshGlobalUpdateStatus'
    };

    this.ExecutionStatusDataDource =
        /*Symbol Indicators : 1.InProgress 2.Complete 3. Error 4.NA (Symbol is not applicable) 5.Realtime 6.Scheduled*/
        [
           { Symbol: '1', DisplayText: 'Finalization', Description: 'Global Update Finalization is pending  and It is in the process of completing Wizard Step.' },
           { Symbol: '1', DisplayText: 'IAS Generation', Description: 'IAS Generation is in process. And IAS will be generated for the impacted folders once the processing has been completed.' },
           { Symbol: '2', DisplayText: 'IAS Generation', Description: 'IAS Generation has been completed and ready for download.' },
           { Symbol: '3', DisplayText: 'IAS Generation.Contact Support Team.', Description: 'Error occurred while generating IAS.Contact technical team for further details.' },
           { Symbol: '1', DisplayText: 'Validation', Description: 'Validation is in progress for the uploaded IAS.' },
           { Symbol: '3', DisplayText: 'Validation', Description: 'Validation failed for the uploaded IAS and ready for Error Log download.' },
           { Symbol: '2', DisplayText: 'Validation / IAS Upload', Description: 'Validation is successful for uploaded IAS and ready for batch creation.' },
           { Symbol: '3', DisplayText: 'Validation.Contact Support Team.', Description: 'Error occurred while processing validation.Contact technical team for further details.' },
           { Symbol: '1', DisplayText: 'Execution', Description: 'Global Update Execution is in progress for the  uploaded IAS.' },
           { Symbol: '3', DisplayText: 'Execution.Contact Support Team.', Description: 'Error occurred while executing Global Update / IAS .Contact technical team for further details.' },
           { Symbol: '2', DisplayText: 'Execution', Description: 'Execution has been completed for the Global Update / IAS.' },
           { Symbol: '5', DisplayText: 'Execution', Description: 'Global Update has been scheduled for realtime execution, but not executed yet.' },
           { Symbol: '6', DisplayText: 'Execution(MM/DD/YYYY)', Description: 'Global Update has been scheduled for the execution on the date mentioned in the parenthesis.' },
           { Symbol: '4', DisplayText: 'Unknown Error. Contact Support Team.', Description: 'Unknown Error occurred in one of the step for Global Update. Contact technical team for further details.' },
        ]

    this.interval = 1000 * 60 * 0.5;
    this.refreshInterval;
    existingGridInstance = this;

    $(document).ready(function () {
        loadExistingGUGrid();
        $(".executionstatus-help").click(function () {
            ExecutionStatusHelpPopUp();
        });
    });
    function loadExistingGUGrid() {
        //set column list for grid
        var colArray = ['GlobalUpdateID', 'Global Update Name', 'Effective Date From', 'Effective Date To', 'Added By', 'Added Date', '','Wizard Step', 'Status', 'IsIASDownloaded', '', 'Execution Status <span class="glyphicon glyphicon-question-sign executionstatus-help" title="Status Details" style="margin-left:2%"></span>'];
        var url;

        //set column models
        var colModel = [];
        colModel.push({ name: 'GlobalUpdateID', index: 'GlobalUpdateID', editable: true, hidden: true });
        colModel.push({ name: 'GlobalUpdateName', index: 'GlobalUpdateName', editable: true, align: 'left', searchoptions: customSearchOptions });
        colModel.push({
            name: 'EffectiveDateFrom', index: 'EffectiveDateFrom', editable: false, formatter: 'date', searchoptions: customSearchOptions,  formatoptions: JQGridSettings.DateFormatterOptions
        });
        colModel.push({ name: 'EffectiveDateTo', index: 'EffectiveDateTo', editable: true,  formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, searchoptions: customSearchOptions });
        colModel.push({ name: 'AddedBy', index: 'AddedBy', editable: true, searchoptions: customSearchOptions });
        colModel.push({ name: 'GuAddedDate', index: 'GuAddedDate', editable: true,formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, searchoptions: customSearchOptions });
        colModel.push({ name: 'RowID', index: 'RowID', key: true, hidden: true, editable: false });
        colModel.push({ name: 'WizardStepName', index: 'WizardStepName', editable: true, align: 'left', hidden: false, searchoptions: customSearchOptions });
        colModel.push({ name: 'status', index: 'status', editable: true, hidden: true, align: 'center' });
        colModel.push({ name: 'IsIASDownloaded', index: 'IsIASDownloaded', editable: true, hidden: true });
        colModel.push({ name: 'ExecutionStatusText', index: 'ExecutionStatusText', editable: true, hidden: true });
        colModel.push({
            name: 'ExecutionStatusSymbol', index: 'ExecutionStatusText', editable: true, sortable: true, hidden: false, width: '220px', formatter: formatGUExecutionStatus, unformat: UnformatGUExecutionStatus
            ,searchoptions: customSearchOptions
        });

        //clean up the grid first - only table element remains after this
        $(this.elementIDs.existingUpdatesGridJQ).jqGrid('GridUnload');

        //adding the pager element
        $(this.elementIDs.existingUpdatesGridJQ).parent().append("<div id='p" + this.elementIDs.existingUpdatesGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(this.elementIDs.existingUpdatesGridJQ).jqGrid({
            url: this.URLs.existingGUList,
            datatype: "json",
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Global Updates List',
            height: '350',
            rowNum: 20,
            rowList: [10, 20, 30],
            ignoreCase: true,
            //loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            celledit: true,
            altRows: true,
            onSelectRow: function (id) {
                //if (id && id !== lastSel) {
                //jQuery(this).restoreRow(lastSel);
                //lastSel = id;
                //var rowid = $(this.elementIDs.existingUpdatesGridJQ).jqGrid('getGridParam', 'selrow');
                //var rowData = $(this.elementIDs.existingUpdatesGridJQ).getRowData(rowid);
                //}
                //jQuery(this).editRow(id, true);
                var rowData = $(this).getRowData(id);
                if (rowData.status == GlobalUpdateStatus.InProgress) {
                    $('#btnManageGlobalUpdateEdit').removeClass('ui-icon-hide');
                    $('#btnManageGlobalUpdateCopy').addClass('ui-icon-hide');
                    $('#btnManageGlobalUpdateUpload').addClass('ui-icon-hide');
                    $('#btnDownloadIAS').addClass('ui-icon-hide');
                    $('#btnDownloadErrorLog').addClass('ui-icon-hide');
                }
                else if (rowData.status == GlobalUpdateStatus.Complete) {
                    $('#btnManageGlobalUpdateEdit').addClass('ui-icon-hide');
                    $('#btnManageGlobalUpdateCopy').addClass('ui-icon-hide');
                    $('#btnManageGlobalUpdateUpload').removeClass('ui-icon-hide');
                    $('#btnDownloadIAS').removeClass('ui-icon-hide');
                    $('#btnDownloadErrorLog').addClass('ui-icon-hide');
                }
                else if (rowData.status == GlobalUpdateStatus.IASInProgress) {
                    $('#btnManageGlobalUpdateEdit').addClass('ui-icon-hide');
                    $('#btnManageGlobalUpdateCopy').addClass('ui-icon-hide');
                    $('#btnManageGlobalUpdateUpload').addClass('ui-icon-hide');
                    $('#btnDownloadIAS').addClass('ui-icon-hide');
                    $('#btnDownloadErrorLog').addClass('ui-icon-hide');
                }
                else if (rowData.status == GlobalUpdateStatus.IASComplete) {
                    $('#btnManageGlobalUpdateEdit').removeClass('ui-icon-hide');
                    $('#btnManageGlobalUpdateCopy').addClass('ui-icon-hide');
                    $('#btnManageGlobalUpdateUpload').removeClass('ui-icon-hide');
                    $('#btnDownloadIAS').removeClass('ui-icon-hide');
                    $('#btnDownloadErrorLog').addClass('ui-icon-hide');
                }
                else if (rowData.status == GlobalUpdateStatus.ValidationInProgress) {
                    $('#btnManageGlobalUpdateEdit').addClass('ui-icon-hide');
                    $('#btnManageGlobalUpdateCopy').addClass('ui-icon-hide');
                    $('#btnManageGlobalUpdateUpload').addClass('ui-icon-hide');
                    $('#btnDownloadIAS').removeClass('ui-icon-hide');
                    $('#btnDownloadErrorLog').addClass('ui-icon-hide');
                }
                else if (rowData.status == GlobalUpdateStatus.ErrorLogComplete) {
                    $('#btnManageGlobalUpdateEdit').addClass('ui-icon-hide');
                    $('#btnManageGlobalUpdateCopy').addClass('ui-icon-hide');
                    $('#btnManageGlobalUpdateUpload').removeClass('ui-icon-hide');
                    $('#btnDownloadIAS').removeClass('ui-icon-hide');
                    $('#btnDownloadErrorLog').removeClass('ui-icon-hide');
                }
                else if (rowData.status == GlobalUpdateStatus.IASFailed) {
                    $('#btnManageGlobalUpdateEdit').removeClass('ui-icon-hide');
                    $('#btnManageGlobalUpdateCopy').addClass('ui-icon-hide');
                    $('#btnManageGlobalUpdateUpload').addClass('ui-icon-hide');
                    $('#btnDownloadIAS').addClass('ui-icon-hide');
                    $('#btnDownloadErrorLog').addClass('ui-icon-hide');
                }
                else if (rowData.status == GlobalUpdateStatus.ErrorLogFailed) {
                    $('#btnManageGlobalUpdateEdit').addClass('ui-icon-hide');
                    $('#btnManageGlobalUpdateCopy').addClass('ui-icon-hide');
                    $('#btnManageGlobalUpdateUpload').removeClass('ui-icon-hide');
                    $('#btnDownloadIAS').removeClass('ui-icon-hide');
                    $('#btnDownloadErrorLog').addClass('ui-icon-hide');
                }
                else if (rowData.status == GlobalUpdateStatus.IASScheduledforExecution || rowData.status == GlobalUpdateStatus.IASExecutionFailed || rowData.status == GlobalUpdateStatus.IASExecutionInProgress || rowData.status == GlobalUpdateStatus.IASExecutionComplete) {
                    $('#btnManageGlobalUpdateEdit').addClass('ui-icon-hide');
                    $('#btnManageGlobalUpdateCopy').addClass('ui-icon-hide');
                    $('#btnManageGlobalUpdateUpload').addClass('ui-icon-hide');
                    $('#btnDownloadIAS').removeClass('ui-icon-hide');
                    $('#btnDownloadErrorLog').addClass('ui-icon-hide');
                }
                if (!($('#btnManageGlobalUpdateEdit').hasClass('ui-icon-hide'))) {
                    $('#viewGlobalUpdate').addClass('ui-icon-hide');
                }
                else {
                    $('#viewGlobalUpdate').removeClass('ui-icon-hide');
                }
            },
            pager: '#p' + this.elementIDs.existingUpdatesGrid,
            sortname: 'GlobalUpdateID',
            altclass: 'alternate',
            gridComplete: function () {
                if (currentFilterElementID != null && currentFilterElementID != undefined)
                    SetCursorAtEnd(document.getElementById(currentFilterElementID));

                checkGlobalUpdateClaims(elementIDs, claims);
            },
            loadComplete: function () {
                existingGridInstance.refreshInterval = setInterval(RefreshGlobalUpdateExecutionStatus, existingGridInstance.interval);
            }
        });
        var pagerElement = '#p' + this.elementIDs.existingUpdatesGrid;
        //remove default buttons
        $(this.elementIDs.existingUpdatesGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(this.elementIDs.existingUpdatesGridJQ).jqGrid('filterToolbar', { stringResultm: true, searchOnEnter: true, defaultSearch: "cn" });

        //$(pagerElement).find(pagerElement + '_center').remove;
        //$(pagerElement).find(pagerElement + '_right').remove();

        //navbar Add button
        $(this.elementIDs.existingUpdatesGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-plus', title: 'Add', id: 'btnManageGlobalUpdateAdd',
            onClickButton: function () {
                //Redirect to IAS wizard Setup step
                window.location.href = URLs.iasWizard + '?mode=add';

            }
        });

        //navbar edit button
        $(this.elementIDs.existingUpdatesGridJQ).jqGrid('navButtonAdd', pagerElement,
                    {
                        caption: '', buttonicon: 'ui-icon-pencil', title: 'Edit', id: 'btnManageGlobalUpdateEdit',
                        onClickButton: function () {

                            var rowId = $(this).getGridParam('selrow');
                            if (rowId !== undefined && rowId !== null) {
                                var row = $(this).getRowData(rowId);

                                /*   Redirecting to Desired Step of Wizard in edit mode*/
                                window.location.href = URLs.iasWizard + '?mode=edit&rowId=' + row.GlobalUpdateID;

                            }
                            else {
                                messageDialog.show(GlobalUpdateMessages.selectRowMessage);
                            }
                        }
                    });

        //navbar View button
        $(this.elementIDs.existingUpdatesGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-document', title: 'View', id: 'viewGlobalUpdate',
            onClickButton: function () {
                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    var row = $(this).getRowData(rowId);

                    /*   Redirecting to Desired Step of Wizard in edit mode*/
                    window.location.href = URLs.iasWizard + '?mode=view&rowId=' + row.GlobalUpdateID;

                }
                else {
                    messageDialog.show(GlobalUpdateMessages.selectRowMessage);
                }
            }
        });

        //navbar Upload IAS button
        $(this.elementIDs.existingUpdatesGridJQ).jqGrid('navButtonAdd', pagerElement,
                   {
                       caption: '', buttonicon: 'ui-icon-folder-open', title: 'Upload IAS', id: 'btnManageGlobalUpdateUpload',
                       onClickButton: function () {
                           var rowId = $(this).getGridParam('selrow');
                           if (rowId !== undefined && rowId !== null) {
                               var row = $(this).getRowData(rowId);

                               var GlobalUpdateID = row.GlobalUpdateID;
                               url = URLs.isValidIASUpload.replace(/\{GlobalUpdateID\}/g, GlobalUpdateID);
                               var promise = ajaxWrapper.getJSON(url);
                               //register ajax success callback
                               promise.done(function (xhr) {
                                   if (xhr == true || xhr == "true") {
                                       importIASDialog.show(row.GlobalUpdateID, row.GlobalUpdateName, row.EffectiveDateFrom, row.EffectiveDateTo);
                                       // SetGlobalUpdateExecutionStatusExplicitly(rowId); TODO: need to call on upload
                                   }
                                   else {
                                       messageDialog.show(GlobalUpdateMessages.uploadIASFailure);
                                   }
                               });
                               //register ajax failure callback
                               promise.fail(showError);
                           }
                           else {
                               messageDialog.show(GlobalUpdateMessages.selectRowMessage);
                           }
                       }
                   });

        //navbar Download IAS Button
        $(this.elementIDs.existingUpdatesGridJQ).jqGrid('navButtonAdd', pagerElement,
                {
                    caption: '', buttonicon: 'ui-icon-arrowthickstop-1-s', title: 'Download IAS', id: 'btnDownloadIAS',
                    onClickButton: function () {
                        var rowId = $(this).getGridParam('selrow');
                        if (rowId !== undefined && rowId !== null) {
                            var row = $(this).getRowData(rowId);

                            var GlobalUpdateID = row.GlobalUpdateID;
                            var GlobalUpdateName = row.GlobalUpdateName;
                            var GlobalUpdateEffectiveDateFrom = row.EffectiveDateFrom;
                            var GlobalUpdateEffectiveDateTo = row.EffectiveDateTo;

                            var parameters = {
                                GlobalUpdateID: GlobalUpdateID,
                                GlobalUpdateName: GlobalUpdateName
                            };
                            var url = '/GlobalUpdate/CheckForIASExcelTemplate';

                            var promise = ajaxWrapper.postJSONCustom(url, parameters);
                            //register ajax success callback
                            promise.done(function (fileExists) {
                                if (fileExists.Result === ServiceResult.SUCCESS) {
                                    var downloadurl = '/GlobalUpdate/DownloadIASExcelTemplate';

                                    var stringData = "GlobalUpdateID=" + GlobalUpdateID;
                                    stringData += "<&GlobalUpdateName=" + GlobalUpdateName;
                                    stringData += "<&GlobalUpdateEffectiveDateFrom=" + GlobalUpdateEffectiveDateFrom;
                                    stringData += "<&GlobalUpdateEffectiveDateTo=" + GlobalUpdateEffectiveDateTo;

                                    $.download(downloadurl, stringData, 'post');
                                }
                                else if (fileExists.Result === ServiceResult.FAILURE) {
                                    messageDialog.show(GlobalUpdateMessages.UnAvailableIASExcelTemplate);
                                }
                                else {
                                    messageDialog.show(Common.errorMsg);
                                }
                            });
                            //register ajax failure callback
                            promise.fail(showError);
                        }
                        else {
                            messageDialog.show(GlobalUpdateMessages.selectRowMessage);
                        }
                    }
                });

        //navbar Download Error Log Button
        $(this.elementIDs.existingUpdatesGridJQ).jqGrid('navButtonAdd', pagerElement,
                {
                    caption: '', buttonicon: 'ui-icon-circle-arrow-s', title: 'Download Error Log', id: 'btnDownloadErrorLog',
                    onClickButton: function () {
                        var rowId = $(this).getGridParam('selrow');
                        if (rowId !== undefined && rowId !== null) {
                            var row = $(this).getRowData(rowId);

                            var GlobalUpdateID = row.GlobalUpdateID;
                            var GlobalUpdateName = row.GlobalUpdateName;
                            var GlobalUpdateEffectiveDateFrom = row.EffectiveDateFrom;
                            var GlobalUpdateEffectiveDateTo = row.EffectiveDateTo;

                            var parameters = {
                                GlobalUpdateID: GlobalUpdateID,
                                GlobalUpdateName: GlobalUpdateName
                            };
                            var url = '/GlobalUpdate/CheckForErrorLogExcelTemplate';

                            var promise = ajaxWrapper.postJSONCustom(url, parameters);
                            //register ajax success callback
                            promise.done(function (fileExists) {
                                if (fileExists.Result === ServiceResult.SUCCESS) {
                                    downloadurl = '/GlobalUpdate/DownloadErrorLogExcelTemplate';

                                    var stringData = "GlobalUpdateID=" + GlobalUpdateID;
                                    stringData += "<&GlobalUpdateName=" + GlobalUpdateName;
                                    stringData += "<&GlobalUpdateEffectiveDateFrom=" + GlobalUpdateEffectiveDateFrom;
                                    stringData += "<&GlobalUpdateEffectiveDateTo=" + GlobalUpdateEffectiveDateTo;

                                    $.download(downloadurl, stringData, 'post');
                                }
                                else if (fileExists.Result === ServiceResult.FAILURE) {
                                    messageDialog.show(GlobalUpdateMessages.UnAvailableErrorLogExcelTemplate);
                                }
                                else {
                                    messageDialog.show(Common.errorMsg);
                                }
                            });
                            //register ajax failure callback
                            promise.fail(showError);
                        }
                        else {
                            messageDialog.show(GlobalUpdateMessages.selectRowMessage);
                        }
                    }
                });

        //navbar copy button
        $(this.elementIDs.existingUpdatesGridJQ).jqGrid('navButtonAdd', pagerElement,
                {
                    caption: '', buttonicon: 'ui-icon-copy', title: 'Copy', id: 'btnManageGlobalUpdateCopy',
                    onClickButton: function () {
                        //copyButtonClick(subGridTableIdJQ);
                    }
                });

    }

    var currentFilterElementID;
    var customSearchOptions = {
        dataEvents:
            [
                {
                    type: 'keypress',
                    fn: function (e) {
                        if (e.keyCode == 13) {
                            currentFilterElementID = e.target.id;
                        }
                    }
                }]
    };

    function ExecutionStatusHelpPopUp() {
        $(existingGridInstance.elementIDs.executionStatusHelpDialogJQ).dialog({
            autoOpen: true,
            height: '410',
            width: '60%',
            modal: true,
            title: 'Help'
        });
        LoadExecutionStatusHelpGrid();
    }

    function LoadExecutionStatusHelpGrid() {
        //set column list
        var colArray = ['', 'Execution Status', 'Description / Action'];
        //set column models
        var colModel = [];
        colModel.push({ name: 'Symbol', index: 'Symbol', key: true, hidden: true, search: false });
        colModel.push({ name: 'DisplayText', index: 'DisplayText', editable: false, width: '100', formatter: formatHelpText });
        colModel.push({ name: 'Description', index: 'Description', width: '250' });

        $(existingGridInstance.elementIDs.ExecutionStatusHelpJQ).jqGrid('GridUnload');
        //$(existingGridInstance.elementIDs.ExecutionStatusHelpJQ).parent().append("<div id='p" + existingGridInstance.elementIDs.ExecutionStatusHelp + "'></div>");
        $(existingGridInstance.elementIDs.ExecutionStatusHelpJQ).jqGrid({
            datatype: 'local',
            data: existingGridInstance.ExecutionStatusDataDource,
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Execution Status Matrix',
            //  pager: '#p' + existingGridInstance.elementIDs.ExecutionStatusHelp,
            height: '315',
            rowheader: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            altRows: true,
            altclass: 'alternate',
            hidegrid: false,
        });
    }

    function formatGUExecutionStatus(cellValue, options, rowObject) {
        return GetGlobalUpdateStatusHtml(rowObject.ExecutionStatusText, rowObject.ExecutionStatusSymbol);
    }

    function UnformatGUExecutionStatus(cellvalue, options, cell) {
        var ExecutionSymbolValue = $('div', cell).attr('dataval');
        return ExecutionSymbolValue;
    }

    function RefreshGlobalUpdateExecutionStatus() {
        var InProgressGlobalUpdateIdArray = [];
        var InProgressGlobalUpdateRows = [];
        var InProgressGlobalUpdates = {};
        var url = existingGridInstance.URLs.getLatestGlobalUpdateStatus;

        var rowIds = $(existingGridInstance.elementIDs.existingUpdatesGridJQ).jqGrid('getDataIDs');
        $.each(rowIds, function (index, value) {
            var globalUpdateRow = $(existingGridInstance.elementIDs.existingUpdatesGridJQ).jqGrid('getRowData', rowIds[index]);
            // if ((globalUpdateRow.ExecutionStatusSymbol == GlobalUpdateStatus.InProgressSymbol) && (globalUpdateRow.ExecutionStatusText != GlobalUpdateStatus.PendingFinzalization)) {
            InProgressGlobalUpdateRows.push({ globalUpdateId: globalUpdateRow.GlobalUpdateID, RowId: rowIds[index] });
            // } //TODO : This condition is temporarily removed but need to work on this for optimization purpose.
        })

        if (InProgressGlobalUpdateRows.length > 0) {
            $.each(InProgressGlobalUpdateRows, function (index, value) {
                InProgressGlobalUpdateIdArray.push(InProgressGlobalUpdateRows[index].globalUpdateId);
            });

            var InProgressGlobalUpdates = {
                inProgressGlobalUpdateIds: JSON.stringify(InProgressGlobalUpdateIdArray)
            };

            var promise = ajaxWrapper.postJSON(url, InProgressGlobalUpdates, false);
            promise.done(function (result) {

                $.each(result, function (index, value) {
                    var InProgressRow = InProgressGlobalUpdateRows.filter(function (InProgressRow) {
                        return InProgressRow.globalUpdateId == result[index].GlobalUpdateId;
                    });
                    var rowData = $(existingGridInstance.elementIDs.existingUpdatesGridJQ).jqGrid('getRowData', InProgressRow[0].RowId);
                    rowData.ExecutionStatusText = result[index].ExecutionStatusText;
                    rowData.ExecutionStatusSymbol = result[index].ExecutionStatusSymbol;
                    rowData.status = result[index].ExecutionStatus;
                    $(existingGridInstance.elementIDs.existingUpdatesGridJQ).jqGrid('setRowData', InProgressRow[0].RowId, rowData);

                });
            });

            promise.fail(showError);
        }
        else {
            clearInterval(existingGridInstance.refreshInterval)
        }
    }

    function GetGlobalUpdateStatusHtml(ExecutionStatusText, ExecutionStatusSymbol) {
        if (ExecutionStatusSymbol == GlobalUpdateStatus.InProgressSymbol) {
            return '<div style="float:left;width:55%" dataval=' + ExecutionStatusSymbol + '><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>' + '  ' + '<span>' + ExecutionStatusText + '</span></div>';
        }
        else if (ExecutionStatusSymbol == GlobalUpdateStatus.CompletedSymbol) {
            return '<div style="float:left;width:55%" dataval=' + ExecutionStatusSymbol + '><span class="glyphicon glyphicon-ok" style = "color:green"></span>' + '  ' + '<span>' + ExecutionStatusText + '</span></div>';
        }
        else if (ExecutionStatusSymbol == GlobalUpdateStatus.ErrorSymbol) {
            return '<div style="float:left;width:55%" dataval=' + ExecutionStatusSymbol + '><span class="glyphicon glyphicon-remove" style = "color:red"></span>' + '  ' + '<span>' + ExecutionStatusText + '</span></div>';
        }
        else if (ExecutionStatusSymbol == GlobalUpdateStatus.RealtimeSymbol) { //Realtime
            return '<div style="float:left;width:55%" dataval=' + ExecutionStatusSymbol + '><span class="glyphicon glyphicon-time" style = "color:blue"></span>' + '  ' + '<span>' + ExecutionStatusText + '</span></div>';
        }

        else if (ExecutionStatusSymbol == GlobalUpdateStatus.ScheduledSymbol) { //Scheduled
            return '<div style="float:left;width:55%" dataval=' + ExecutionStatusSymbol + '><span class="glyphicon glyphicon-calendar" style = "color:blue"></span>' + '  ' + '<span>' + ExecutionStatusText + '</span></div>';
        }
        else if (ExecutionStatusSymbol == GlobalUpdateStatus.NASymbol) {
            //return ExecutionStatusText;
            return '<div style="float:left;width:55%" dataval=' + ExecutionStatusSymbol + '><span>' + ExecutionStatusText + '</span></div>';
        }

        else {
            return '<div style="float:left;width:55%" dataval=' + ExecutionStatusSymbol + '><span>Unknown Error.Contact Support Team.</span></div>';
        }

    }

    function formatHelpText(cellValue, options, rowObject) {
        return GetGlobalUpdateStatusHtml(rowObject.DisplayText, rowObject.Symbol);
    }

    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

}();

//contains functionality for the Upload IAS add dialog
var importIASDialog = function () {
    var URLs = {
        uploadTemplate: '/GlobalUpdate/ProcessIASTemplate?GlobalUpdateID={GlobalUpdateID}',
        validateIAS: '/GlobalUpdate/ValidateIAS',
        scheduleIASUpload: '/GlobalUpdate/ScheduleIASUpload',
    }

    //see element id's in Views\GlobalUpdate\ExistsingGUGrid.cshtml
    var elementIDs = {
        //IAS Upload dialog element
        importIASDialog: "#importIASdialog",
        btnUploadJQ: '#btnUpload',
        uploadFileJQ: '#uploadFile',
        uploadFileHelpBlockJQ: "#uploadFileHelpBlock",
        uploadFileTextJQ: '#uploadFileText',
    };

    //current GlobalUpdateID
    var currentGlobalUpdateID;
    var currentGlobalUpdateName;
    var currentEffectiveDateFrom;
    var currentEffectiveDateTo;
    this.filesToUpload = [];

    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    //Upload file to folder(\\App_Data\\IAS\\)
    function uploadFile() {
        var currentInstance = this;
        //check all fields for validations.
        var isValid = false;
        isValid = validate();
        if (isValid) {
            var url = URLs.uploadTemplate.replace(/\{GlobalUpdateID\}/g, currentGlobalUpdateID);

            //ajax call to generate IAS data.
            var promise = ajaxWrapper.uploadFile(url, currentInstance.filesToUpload);
            //register ajax success callback        

            promise.done(function (xhr) {
                if (xhr.Result == ServiceResult.SUCCESS) {
                    $(elementIDs.btnUploadJQ).attr('disabled', 'disabled');
                    $(elementIDs.uploadFileJQ).attr('disabled', 'disabled');
                    currentInstance.filesToUpload = undefined;
                    $(elementIDs.uploadFileHelpBlockJQ).text("");
                    $(elementIDs.importIASDialog).dialog("close");
                    messageDialog.show(GlobalUpdateMessages.uploadIASSuccess);
                    /*
                    var GlobalUpdateID = currentGlobalUpdateID;
                    var GlobalUpdateName = currentGlobalUpdateName;
                    var GlobalUpdateEffectiveDateFrom = currentEffectiveDateFrom;
                    var GlobalUpdateEffectiveDateTo = currentEffectiveDateTo;

                    url = URLs.validateIAS;

                    var stringData = "GlobalUpdateID=" + GlobalUpdateID;
                    stringData += "<&GlobalUpdateName=" + GlobalUpdateName;
                    stringData += "<&GlobalUpdateEffectiveDateFrom=" + GlobalUpdateEffectiveDateFrom;
                    stringData += "<&GlobalUpdateEffectiveDateTo=" + GlobalUpdateEffectiveDateTo;

                    $.download(url, stringData, 'post');
                    */
                    var GlobalUpdateID = currentGlobalUpdateID;
                    var parameters = {
                        GlobalUpdateID: GlobalUpdateID
                    };

                    //ajax POST of element properties
                    var promise = ajaxWrapper.postJSONCustom(URLs.scheduleIASUpload, parameters);
                    promise.done(function (res) {
                        if (res.Result === ServiceResult.SUCCESS) {
                            messageDialog.show(GlobalUpdateMessages.scheduleIASValidationSuccess);
                        }
                        else {
                            messageDialog.show(GlobalUpdateMessages.errorLogIASMsg);
                        }
                    });
                    //register ajax failure callback
                    promise.fail(showError);
                }
                else if (xhr.Result == ServiceResult.FAILURE)
                {
                    messageDialog.show(xhr.Items[0].Messages[0]);
                }
                else {
                    messageDialog.show(Common.errorMsg);
                }
            });
            //register ajax failure callback
            promise.fail(showError);
        }
    }

    //validate controls to Upload IAS.
    function validate() {
        var isValidDocument = false;

        var file = $(elementIDs.uploadFileJQ).val();
        //Check file extension
        var ext = file.split('.').pop().toLowerCase();   //Check file extension if valid or expected
        if ($.inArray(ext, ['xlsm', 'xlsx', 'xls']) == -1) {
            $(elementIDs.uploadFileHelpBlockJQ).parent().addClass('has-error');
            $(elementIDs.uploadFileHelpBlockJQ).text(GlobalUpdateMessages.uploadValidIAS);
            isValidDocument = false;
        }
        else {
            $(elementIDs.uploadFileHelpBlockJQ).parent().removeClass('has-error');
            isValidDocument = true;
        }

        return isValidDocument;
    }

    //init dialog on load of page
    function init() {
        //register dialog for grid row upload IAS
        $(elementIDs.importIASDialog).dialog({
            autoOpen: false,
            height: 120,
            width: 450,
            modal: true,
            close: function () {
                $(elementIDs.uploadFileTextJQ).val('');
            }
        });

        $(elementIDs.importIASDialog).on('change', elementIDs.uploadFileJQ, function (e) {
            var input = $(this),
                numFiles = input.get(0).files ? input.get(0).files.length : 1,
                label = input.val().replace(/\\/g, '/').replace(/.*\//, '');

            filesToUpload = new FormData();

            if (e.target.files.length > 0) {
                for (var i = 0; i < e.target.files.length; i++) {
                    filesToUpload.append(e.target.files[i].name, e.target.files[i]);
                }
            }

            input.trigger('fileselect', [numFiles, label]);
        });

        $(elementIDs.uploadFileJQ).on('fileselect', function (event, numFiles, label) {
            var input = $(elementIDs.uploadFileTextJQ),
                log = numFiles > 1 ? numFiles + ' files selected' : label;

            if (input.length) {
                input.val(log);
            } else {
                if (log)
                    alert(log);
            }

        });
        //register event for Upload button click on dialog
        $(elementIDs.btnUploadJQ).on('click', function () {
            if ($(elementIDs.uploadFileTextJQ).val() == "") {
                $(elementIDs.uploadFileHelpBlockJQ).parent().addClass('has-error');
                $(elementIDs.uploadFileHelpBlockJQ).text("");
                $(elementIDs.uploadFileHelpBlockJQ).text(GlobalUpdateMessages.uploadValidIAS);
            }
            else {
                return uploadFile();
            }
        });
    }
    //initialize the dialog after this js are loaded
    init();

    return {
        show: function (GlobalUpdateID, GlobalUpdateName, EffectiveDateFrom, EffectiveDateTo) {
            filesToUpload = [];
            $(elementIDs.uploadFileJQ).removeAttr('disabled');
            $(elementIDs.btnUploadJQ).removeAttr('disabled');
            currentGlobalUpdateID = GlobalUpdateID;
            currentGlobalUpdateName = GlobalUpdateName;
            currentEffectiveDateFrom = EffectiveDateFrom;
            currentEffectiveDateTo = EffectiveDateTo;
            $(elementIDs.importIASDialog + ' div').removeClass('has-error');
            $(elementIDs.importIASDialog).dialog('option', 'title', "Upload IAS");
            //open the dialog - uses jqueryui dialog
            $(elementIDs.importIASDialog).dialog("open");
        }
    }
}(); //invoked soon after loading
//Contains functionality for Batch Execution
var batchExecution = function () {
    var URL = {
        existingBatchesList: '/GlobalUpdate/GetExistsingBatches',
        loadAddBatchDialog: '/GlobalUpdate/BatchExecution',
        importedNotAddedIASList: '/GlobalUpdate/GetImportedNotAddedIASListBatches',
        //saveBatch: '/GlobalUpdate/SaveBatch?batchName={batchName}&executionType={executionType}&scheduleDate={scheduleDate}&scheduledTime={scheduledTime}&globalUpdateIDArray={globalUpdateIDArray}'
        saveBatch: '/GlobalUpdate/SaveBatch',
        checkDuplicateFolderVersionExistsInSelectedBatchIAS: '/GlobalUpdate/CheckDuplicateFolderVersionExistsInSelectedBatchIAS',
        updateBatch: '/GlobalUpdate/UpdateBatch',
        approveBatch: '/GlobalUpdate/ApproveBatch?batchName={batchName}',
        executedBatchesList: '/GlobalUpdate/ExecutedBatch',
        editBatchIASListGrid: '/GlobalUpdate/EditBatchIASListGrid?batchID={batchID}',
        viewBatchIASListGrid: '/GlobalUpdate/ViewBatchIASListGrid?batchID={batchID}',
        GetGUIdsFromBatchMap: '/GlobalUpdate/GetGUIdsFromBatchMap?batchID={batchID}',
        baseLineFolderVersion: '/GlobalUpdate/GlobalUpdateBaseLineFolderVersions',
        GetFolderVersionsBaselined: '/GlobalUpdate/GetFolderVersionsBaselined?batchID={batchID}',
        //Check Audit Report
        checkAuditReport: "/GlobalUpdate/CheckForAuditReport?batchID={batchID}",
        //download Audit Report
        downloadAuditReport: "/GlobalUpdate/DownloadAuditReport?batchID={batchID}",
        rollBackBatch: "/GlobalUpdate/RollBackBatch",
        //delete Batch
        deleteGlobalUpdateBatch: "/GlobalUpdate/DeleteBatch?batchId={batchId}",
        //Save Execution Schedule
       // saveExecutionSchedule: '/GlobalUpdate/SaveExecutionSchedule'
    }

    //see element id's in Views\GlobalUpdate\BatchExecution.cshtml
    var elementIDs = {
        //Get List of existing Batches
        existinBatchesGrid: "existinBatchesGrid",
        existinBatchesGridJQ: "#existinBatchesGrid",
        addBatchGrid: "addBatchGrid",
        addBatchGridJQ: "#addBatchGrid",
        editBatchGrid: "editBatchGrid",
        editBatchGridJQ: "#editBatchGrid",
        addBatchDialogJQ: "#addBatchDialog",
        editBatchDialogJQ: "#editBatchDialog",
        scheduledDateJQ: "#scheduledDate",
        executionTypeJQ: "#executionType",
        addBatchJQ: "#addBatch",
        editBatchButtonJQ: "#editBatchButton",
        //Add
        batchNameJQ: "#batchName",
        scheduledTimeJQ: "#scheduledTime",
        batchNamerequiredJQ: "#batchNamerequired",
        executionTyperequiredJQ: "#executionTyperequired",
        scheduledDaterequiredJQ: "#scheduledDaterequired",
        scheduledTimerequiredJQ: "#scheduledTimerequired",
        scheduledDateLabelJQ: "#scheduledDateLabel",
        scheduledTimeLabelJQ: "#scheduledTimeLabel",
        //Edit
        editBatchNameJQ: "#editBatchName",
        editExecutionTypeJQ: "#editExecutionType",
        editScheduledDateJQ: "#editScheduledDate",
        editScheduledTimeJQ: "#editScheduledTime",
        btnManageAccountEditJQ: "#btnManageAccountEdit",
        approveBatchJQ: "#approveBatch",

        //View
        viewBatchGridJQ: "#viewBatchGrid",
        viewBatchGrid: "viewBatchGrid",
        viewScheduledTimeJQ: "#viewScheduledTime",
        viewScheduledDateJQ: "#viewScheduledDate",
        viewExecutionTypeJQ: "#viewExecutionType",
        viewBatchNameJQ: "#viewBatchName",
        viewBatchDialogJQ: "#viewBatchDialog",

        //Approve
        approveBatchDialogJQ: "#approveBatchDialog",
        batchNameAprJQ: "#batchNameApr",
        batchNamerequiredAprJQ: "#batchNamerequiredApr",
        executionTyperequiredAprJQ: "#executionTyperequiredApr",
        executionTypeAprJQ: "#executionTypeApr",
        scheduledDaterequiredAprJQ: "#scheduledDaterequiredApr",
        scheduledDateAprJQ: "#scheduledDateApr",
        scheduledTimerequiredAprJQ: "#scheduledTimerequiredApr",
        scheduledTimeAprJQ: "#scheduledTimeApr",
        approveBatchButtonJQ: "#approveBatchButton",
        approveBatchCheckrequiredJQ: "#approveBatchCheckrequired",
        approveBatchCheckJQ: "#approveBatchCheck",
        executedBatchesGridJQ: "#executedBatchesGrid",
        executedBatchesGrid: "executedBatchesGrid",

        //Dummy Baseline
        baselineDummyButtonJQ: "#baselineDummyButton",

        //RollBack
        confirmRollBackDialogJQ: "#confirmRollBackDialog",
        rollbackCommntsJQ: "#rollbackCommnts",
        saveRollBackButtonJQ: "#saveRollBackButton",
        rollBackDialogJQ: "#rollBackDialog",
        commentsHelpBlockJQ: "#commentsHelpBlock",

        //Delete Batch
        btnDeleteGuBatchJQ: "#btnDeleteGuBatch",
        confirmGuBatchDeleteDialogJQ: '#confirmGuBatchDeleteDialog',

        //OneTime Scheduler
        txtExecutionDateJQ: '#txtExecutionDate',
        txtSchedulerNameJQ: '#txtSchedulerName',
        txtExecutionTimeJQ: '#txtExecutionTime',
        btnScheduleBatchExecutionJQ: '#btnScheduleBatchExecution',
        btnManageAccountAddJQ: '#btnManageAccountAdd',
        btnManageAccountEditJQ: '#btnManageAccountEdit',
        btnDeleteGuBatchJQ: '#btnDeleteGuBatch',
        RealtimeBatchExecutionJQ: '#RealtimeBatchExecution',
        approveBatchJQ: '#approveBatchJQ',
        DownloadAuditReportJQ: '#DownloadAuditReport',
        RollbackBatchJQ: '#RollbackBatch'
    };

    this.globalUpdateIDArray = new Array();
    this.batchId = null;
    this.realTSchFlag = false;
    this.fldrVersionIdsForBaseline = new Array();
    this.exeType = '';
    this.rollBackRowData = null;
    $(document).ready(function () {
        loadBatchesGrid();
        loadExecutedBatchesGrid();
        setDatePickerForInputType(elementIDs.scheduledDateJQ);
        setDatePickerForInputType(elementIDs.editScheduledDateJQ);
        setDatePickerForInputType(elementIDs.txtExecutionDateJQ);
        InitializeControls();
        $(elementIDs.addBatchJQ).click(function () {
            SaveBatch();
        });
        $(elementIDs.editBatchButtonJQ).click(function () {
            updateBatch(batchId);
        });
        $(elementIDs.commentsHelpBlockJQ).text("*");
        $(elementIDs.scheduledTimeJQ).inputmask("99:99:99");
        $(elementIDs.editScheduledTimeJQ).inputmask("99:99:99");
        $(elementIDs.txtExecutionTimeJQ).inputmask("99:99:99");
        var currdate = new Date();
        var currdate = (currdate.getMonth() + 1) + '/' + currdate.getDate() + '/' + currdate.getFullYear();

        $(elementIDs.scheduledDateJQ).change(function () {
            var enteredSchDate = $(elementIDs.scheduledDateJQ).val();

            if (Date.parse(enteredSchDate) < Date.parse(currdate)) {
                messageDialog.show("Scheduled Date should be greater than today's date!");
            }
        });
        $(elementIDs.editScheduledDateJQ).change(function () {
            var enteredSchDate = $(elementIDs.editScheduledDateJQ).val();

            if (Date.parse(enteredSchDate) < Date.parse(currdate)) {
                messageDialog.show("Scheduled Date should be greater than today's date!");
            }
        });
        $(elementIDs.executionTypeJQ).change(function () {
            exeType = $(elementIDs.executionTypeJQ).val();
            if ($(elementIDs.executionTypeJQ).val() == ExecutionType.Realtime) {
                $(elementIDs.scheduledDateJQ).attr('disabled', 'disable');
                $(elementIDs.scheduledTimeJQ).attr('disabled', 'disable');
                $(elementIDs.scheduledDateJQ).val('');
                $(elementIDs.scheduledTimeJQ).val('');
            } else if ($(elementIDs.executionTypeJQ).val() == ExecutionType.Scheduled || $(elementIDs.executionTypeJQ).val() == "selectOne") {
                $(elementIDs.scheduledDateJQ).removeAttr("disabled");
                $(elementIDs.scheduledTimeJQ).removeAttr("disabled");
            }
        });
        $(elementIDs.editExecutionTypeJQ).change(function () {
            if ($(elementIDs.editExecutionTypeJQ).val() == ExecutionType.Realtime) {
                $(elementIDs.editScheduledDateJQ).attr('disabled', 'disable');
                $(elementIDs.editScheduledTimeJQ).attr('disabled', 'disable');
                $(elementIDs.editScheduledDateJQ).val('');
                $(elementIDs.editScheduledTimeJQ).val('');
            } else if ($(elementIDs.editExecutionTypeJQ).val() == ExecutionType.Scheduled || $(elementIDs.executionTypeJQ).val() == "selectOne") {
                $(elementIDs.editScheduledDateJQ).removeAttr("disabled");
                $(elementIDs.editScheduledTimeJQ).removeAttr("disabled");
            }
        });
        $(elementIDs.batchNamerequiredJQ).text("*");
        $(elementIDs.executionTyperequiredJQ).text("*");
        $(elementIDs.scheduledDaterequiredJQ).text("*");
        $(elementIDs.scheduledTimerequiredJQ).text("*");
        $(elementIDs.approveBatchCheckrequiredJQ).text("*");
    });

    function InitializeControls() {
        $(elementIDs.batchNameJQ).val('');
        //$(elementIDs.executionTypeJQ)[0].selectedIndex = 0;
        $("#executionType option:first").attr("selected", true);
        $(elementIDs.scheduledDateJQ).attr('disabled', 'disable');
        $(elementIDs.scheduledTimeJQ).attr('disabled', 'disable');

        $(elementIDs.scheduledDateJQ).val('');
        $(elementIDs.scheduledTimeJQ).val('');
    }
    function loadBatchesGrid() {
        //set column list for grid
        var colArray = ['Batch ID', 'Batch Name', 'Execution Type', 'Scheduled Date', 'Scheduled Time', 'Added By', 'Added Date', 'Is Approved', 'Approved By', 'Approved Date', 'Batch Execution Status'];
        var url;

        //set column models
        var colModel = [];
        colModel.push({ name: 'BatchID', index: 'BatchID', editable: false, hidden: true });
        colModel.push({ name: 'BatchName', index: 'BatchName', editable: false, align: 'left' });
        colModel.push({ name: 'ExecutionType', index: 'ExecutionType', editable: false, align: 'center' });
        colModel.push({ name: 'ScheduleDate', index: 'ScheduleDate', editable: false, align: 'center' });
        colModel.push({ name: 'ScheduledTime', index: 'ScheduledTime', editable: false, align: 'center' });
        colModel.push({ name: 'AddedBy', index: 'AddedBy', editable: false, align: 'center' });
        colModel.push({ name: 'AddedDate', index: 'AddedDate', editable: false, align: 'center'});
        colModel.push({ name: 'IsApprovedString', index: 'IsApprovedString', editable: false, align: 'center' });
        colModel.push({ name: 'ApprovedBy', index: 'ApprovedBy', editable: false, align: 'center' });
        colModel.push({ name: 'ApprovedDate', index: 'ApprovedDate', editable: false, align: 'center' });
        colModel.push({ name: 'BatchExecutionStatus', index: 'BatchExecutionStatus', editable: false, align: 'center' });

        //clean up the grid first - only table element remains after this
        $(elementIDs.existinBatchesGridJQ).jqGrid('GridUnload');

        //added to beforeProcessing must work
        $(elementIDs.existinBatchesGridJQ).jqGrid("destroyFilterToolbar");

        //adding the pager element
        $(elementIDs.existinBatchesGridJQ).parent().append("<div id='p" + elementIDs.existinBatchesGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.existinBatchesGridJQ).jqGrid({
            url: URL.existingBatchesList,
            datatype: "json",
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Batch List',
            height: '300',
            rowNum: 20,
            rowList: [10, 20, 30],
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            beforeProcessing: function (data) {
                $.each(data, function (i, dt) {
                    var selectedRowData = dt;
                    if (dt.ScheduleDate != null) {
                        var ScheduleDate = new Date(selectedRowData.ScheduleDate);
                        selectedRowData.ScheduleDate = formatDateForSort(ScheduleDate, false);;
                    }
                    var AddedDate = new Date(selectedRowData.AddedDate);
                    selectedRowData.AddedDate = formatDateForSort(AddedDate, false);;
                    if (selectedRowData.ApprovedDate == null) {
                        selectedRowData.ApprovedDate = null;
                    } else {
                        var ApprovedDate = new Date(selectedRowData.ApprovedDate);
                        selectedRowData.ApprovedDate = formatDateForSort(ApprovedDate, false);;
                    }
                });
            },
            onSelectRow: function (id) {
                var rowid = $(elementIDs.existinBatchesGridJQ).jqGrid('getGridParam', 'selrow');
                var rowData = $(elementIDs.existinBatchesGridJQ).getRowData(rowid);
                $(elementIDs.batchNameAprJQ).val(rowData.BatchName);
                $(elementIDs.executionTypeAprJQ).val(rowData.ExecutionType.trim());
                $(elementIDs.scheduledDateAprJQ).val(rowData.ScheduleDate);
                $(elementIDs.scheduledTimeAprJQ).val(rowData.ScheduledTime);

                var rowData = $(this).getRowData(id);
                if (rowData.ExecutionType.trim() == ExecutionType.Realtime && rowData.IsApprovedString == "Yes" && rowData.BatchExecutionStatus != "Complete" && rowData.BatchExecutionStatus != 'Rollbacked') {
                    $('#RealtimeBatchExecution').removeClass('ui-icon-hide');
                }
                else {
                    $('#RealtimeBatchExecution').addClass('ui-icon-hide');
                }

                //hide Edit, Delete, Approve grid icon if batch is Approved
                if (rowData.IsApprovedString.trim() == "No") {
                    $(elementIDs.btnManageAccountEditJQ).removeClass('ui-icon-hide');
                    $(elementIDs.btnDeleteGuBatchJQ).removeClass('ui-icon-hide');
                    $(elementIDs.approveBatchJQ).removeClass('ui-icon-hide');

                }
                else {
                    $(elementIDs.approveBatchJQ).addClass('ui-icon-hide');
                    $(elementIDs.btnManageAccountEditJQ).addClass('ui-icon-hide');
                    $(elementIDs.btnDeleteGuBatchJQ).addClass('ui-icon-hide');
                }
            },
            gridComplete: function () {
                //to check for claims.               
                var objMap = {
                    approvebatch: "#approveBatch"
                };
                checkApplicationClaims(claims, objMap, URLs.batchExe);
                checkBatchGenerationClaims(elementIDs, claims);
            },
            pager: '#p' + elementIDs.existinBatchesGrid,

        });
        var pagerElement = '#p' + elementIDs.existinBatchesGrid;
        //remove default buttons
        $(elementIDs.existinBatchesGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.existinBatchesGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

        //navbar Add button
        $(elementIDs.existinBatchesGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-plus', title: 'Add', id: 'btnManageAccountAdd',
            onClickButton: function () {
                loadAddIASPopUp();
            }
        });

        //navbar edit button
        $(elementIDs.existinBatchesGridJQ).jqGrid('navButtonAdd', pagerElement,
                    {
                        caption: '', buttonicon: 'ui-icon-pencil', title: 'Edit Batch', id: 'btnManageAccountEdit',
                        onClickButton: function () {

                            var rowId = $(this).getGridParam('selrow');
                            if (rowId !== undefined && rowId !== null) {
                                var rowData = $(this).getRowData(rowId);
                                var isApproved = rowData.IsApprovedString;
                                if (rowData.ExecutionType.trim() == ExecutionType.Realtime) {
                                    realTSchFlag = true;
                                }
                                else if (rowData.ExecutionType.trim() == ExecutionType.Scheduled) {
                                    realTSchFlag = false;
                                }
                                editBatch(isApproved, rowData, realTSchFlag);
                            }
                            else {
                                messageDialog.show(GlobalUpdateMessages.selectRowMessage);
                            }
                        }
                    });



        $(elementIDs.existinBatchesGridJQ).jqGrid('navButtonAdd', pagerElement,
                   {
                       caption: '', buttonicon: 'ui-icon-trash', title: 'Delete Batch', id: 'btnDeleteGuBatch',
                       onClickButton: function () {

                           var rowId = $(this).getGridParam('selrow');
                           if (rowId !== undefined && rowId !== null) {
                               var rowData = $(this).getRowData(rowId);
                               $(elementIDs.confirmGuBatchDeleteDialogJQ).find('div p').text("Are you sure you want to Delete Batch '" + rowData.BatchName + "'?");
                               $(elementIDs.confirmGuBatchDeleteDialogJQ).dialog({
                                   title: 'Confirm Batch Deletion',
                                   height: 120,
                                   buttons: {
                                       Yes: function () {
                                           DeleteGlobalUpdateBatch(rowData.BatchID);
                                           $(this).dialog("close");
                                       },
                                       No: function () {
                                           $(this).dialog("close");
                                       }
                                   }
                               });
                           }
                       }
                   });

        //navbar approve button
        $(elementIDs.existinBatchesGridJQ).jqGrid('navButtonAdd', pagerElement,
                {
                    caption: '', buttonicon: 'ui-icon-check', title: 'Approve Batch', id: 'approveBatch',
                    onClickButton: function () {
                        var rowId = $(this).getGridParam('selrow');
                        if (rowId !== undefined && rowId !== null) {
                            var rowData = $(elementIDs.existinBatchesGridJQ).getRowData(rowId);
                            var isApproved = rowData.IsApprovedString;
                            loadBatchApprovalDialog(isApproved);
                        }
                        else {
                            messageDialog.show(GlobalUpdateMessages.selectRowMessage);
                        }
                    }
                });
        //navbar View button
        $(elementIDs.existinBatchesGridJQ).jqGrid('navButtonAdd', pagerElement,
                {
                    caption: '', buttonicon: 'ui-icon-document', title: 'View Batch', id: 'View Batch',
                    onClickButton: function () {
                        var rowId = $(this).getGridParam('selrow');
                        if (rowId !== undefined && rowId !== null) {
                            var rowData = $(elementIDs.existinBatchesGridJQ).getRowData(rowId);
                            $(elementIDs.viewBatchDialogJQ).attr('disabled', 'disable');
                            viewBatch(rowData);
                        }
                        else {
                            messageDialog.show(GlobalUpdateMessages.selectRowMessage);
                        }
                    }
                });

        //navbar RealTime Batch Execution button
        $(elementIDs.existinBatchesGridJQ).jqGrid('navButtonAdd', pagerElement,
       {
           caption: '', buttonicon: 'ui-icon-triangle-1-e', title: 'Realtime Batch Execution', id: 'RealtimeBatchExecution',
           onClickButton: function () {
               var rowId = $(this).getGridParam('selrow');
               if (rowId !== undefined && rowId !== null) {
                   var rowData = $(elementIDs.existinBatchesGridJQ).getRowData(rowId);
                   //var rowData = $(this).getRowData(rowId);
                   $(elementIDs.confirmGuBatchDeleteDialogJQ).find('div p').text("Are you sure you want to Execute Batch '" + rowData.BatchName + "' now?");
                   $(elementIDs.confirmGuBatchDeleteDialogJQ).dialog({
                       title: 'Confirm Realtime Batch Execution',
                       height: 120,
                       buttons: {
                           Yes: function () {
                               CreateRetroFolder(rowData);
                               $(this).dialog("close");
                           },
                           No: function () {
                               $(this).dialog("close");
                           }
                       }
                   });
               }
               else {
                   messageDialog.show(GlobalUpdateMessages.selectRowMessage);
               }
           }
       });
    }
    function formatDateForSort(dt, isTimeRequired) {
        var month = dt.getMonth().toString().length > 1 ? (dt.getMonth() + 1) : "0" + (dt.getMonth() + 1).toString();
        var day = dt.getDate().toString().length > 1 ? dt.getDate() : "0" + dt.getDate().toString();
        var year = dt.getFullYear().toString();
        var thistime = "";
        if (isTimeRequired) {
            var h = dt.getHours().toString().length > 1 ? dt.getHours().toString() : "0" + dt.getHours().toString();
            var m = dt.getMinutes().toString().length > 1 ? dt.getMinutes().toString() : "0" + dt.getMinutes().toString();
            thistime = (h > 12) ? (h - 12 + ':' + m + ' PM') : (h + ':' + m + ' AM');
        }
        var formattedDate = month + "/" + day + "/" + year + " " + thistime;
        return formattedDate;
    }
    function CreateRetroFolder(rowData) {
        var parameters = {
            BatchID: rowData.BatchID,
            BatchName: rowData.BatchName,
            tenantId: 1
        };
        var url = '/GlobalUpdate/ExecuteBatch';

        var promise = ajaxWrapper.postJSONCustom(url, parameters);
        //register ajax success callback
        promise.done(function (elementExportSuccess) {
            if (elementExportSuccess.Result === ServiceResult.SUCCESS) {
                messageDialog.show(GlobalUpdateMessages.realtimeBatchExectionSuccess);
                loadBatchesGrid();
            }
            else {
                messageDialog.show(Common.errorMsg);
            }
        });
        //register ajax failure callback
        promise.fail(showError);
    }

    function GetFolderVersionsBaselinedLogsList(batchID) {
        url = URL.GetFolderVersionsBaselined.replace(/\{batchID\}/g, batchID);
        var promise = ajaxWrapper.getJSON(url);
        //register ajax success callback
        promise.done(function (xhr) {
            if (xhr.length <= 0) {
                messageDialog.show("There are no folders which are In Progress for this Batch!");
            }
            for (i = 0; i < xhr.length; i++) {
                fldrVersionIdsForBaseline.push(xhr[i]);
            }
        });
        //register ajax failure callback
        promise.fail(showError);
    }

    function viewBatch(rowData) {
        var batchName = rowData.BatchName;
        var executionType = rowData.ExecutionType;
        var scheduledDate = rowData.ScheduleDate;
        var scheduledTime = rowData.ScheduledTime;
        batchId = rowData.BatchID;

        $(elementIDs.viewBatchNameJQ).val(batchName);
        $(elementIDs.viewExecutionTypeJQ).val(executionType.trim()).prop('selected', true);
        $(elementIDs.viewScheduledDateJQ).val(scheduledDate);
        $(elementIDs.viewScheduledTimeJQ).val(scheduledTime);
        loadViewIASPopUp(batchId);
    }

    function editBatch(isApproved, rowData, realTSch) {
        if (isApproved != "Yes") {
            var batchName = rowData.BatchName;
            var executionType = rowData.ExecutionType;
            var scheduledDate = rowData.ScheduleDate;
            var scheduledTime = rowData.ScheduledTime;
            batchId = rowData.BatchID;

            $(elementIDs.editBatchNameJQ).val(batchName);
            $(elementIDs.editExecutionTypeJQ).val(executionType.trim()).prop('selected', true);
            $(elementIDs.editScheduledDateJQ).val(scheduledDate);
            $(elementIDs.editScheduledTimeJQ).val(scheduledTime);
            loadEditIASPopUp(batchId, realTSch);
        }
        else {
            messageDialog.show(GlobalUpdateMessages.alreadyApprovedBatch);
        }
    }
    function loadExecutedBatchesGrid() {
        //set column list for grid
        var colArray = ['Batch Execution ID', 'Batch ID', 'Batch Execution Status ID', 'Batch Name', 'Batch Execution Status', 'Execution Start Date Time', 'Execution End Date Time', 'RollBackComments', 'RollBackThreholdFlag'];
        var url;

        //set column models BatchExecutionStatus
        var colModel = [];
        colModel.push({ name: 'BatchExecutionID', index: 'BatchExecutionID', editable: false, hidden: true });
        colModel.push({ name: 'BatchID', index: 'BatchID', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'BatchExecutionStatusID', index: 'BatchExecutionStatusID', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'BatchName', index: 'BatchName', editable: false, align: 'left' });
        colModel.push({ name: 'BatchExecutionStatus', index: 'BatchExecutionStatus', editable: false, align: 'center' });
        colModel.push({ name: 'StartDateTime', index: 'StartDateTime', editable: false, align: 'center' });
        colModel.push({ name: 'EndDateTime', index: 'EndDateTime', editable: false, align: 'center' });
        colModel.push({ name: 'RollBackComments', index: 'RollBackComments', editable: false, align: 'left' });
        colModel.push({ name: 'RollBackThreholdFlag', index: 'RollBackComments', editable: false, align: 'left', hidden: true });

        //clean up the grid first - only table element remains after this
        $(elementIDs.executedBatchesGridJQ).jqGrid('GridUnload');

        //added to beforeProcessing must work
        $(elementIDs.executedBatchesGridJQ).jqGrid("destroyFilterToolbar");

        //adding the pager element
        $(elementIDs.executedBatchesGridJQ).parent().append("<div id='p" + elementIDs.executedBatchesGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.executedBatchesGridJQ).jqGrid({
            url: URL.executedBatchesList,
            datatype: "json",
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Executed Batch List',
            height: '350',
            rowNum: 20,
            rowList: [10, 20, 30],
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            beforeProcessing: function (data) {
                $.each(data, function (i, dt) {
                    var selectedRowData = dt;
                    var StartDateTime = new Date(selectedRowData.StartDateTime);
                    selectedRowData.StartDateTime = formatDateForSort(StartDateTime, true);;

                    var EndDateTime = new Date(selectedRowData.EndDateTime);
                    selectedRowData.EndDateTime = formatDateForSort(EndDateTime, true);;
                });
            },
            onSelectRow: function (id) {
                var rowid = $(elementIDs.executedBatchesGridJQ).jqGrid('getGridParam', 'selrow');
                var rowData = $(elementIDs.executedBatchesGridJQ).getRowData(rowid);
                if (rowData.BatchExecutionStatusID == BatchExecutionStatus.Complete) {
                    $('#DownloadAuditReport').removeClass('ui-icon-hide');
                } else {
                    $('#DownloadAuditReport').addClass('ui-icon-hide');
                }

                if (rowData.BatchExecutionStatusID != 3 && rowData.RollBackThreholdFlag == "true") {
                    $('#RollbackBatch').removeClass('ui-icon-hide');
                } else {
                    $('#RollbackBatch').addClass('ui-icon-hide');
                }
                //RollBack batch button
                //if (rowData.RollBackThreholdFlag == "true") {
                //    $('#RollbackBatch').removeClass('ui-icon-hide');
                //} else {
                //    $('#RollbackBatch').addClass('ui-icon-hide');
                //}
            },

            pager: '#p' + elementIDs.executedBatchesGrid,
            gridComplete: function () {

                checkBatchExecutionClaims(elementIDs, claims);
            }
        });
        var pagerElement = '#p' + elementIDs.executedBatchesGrid;
        //remove default buttons
        $(elementIDs.executedBatchesGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.executedBatchesGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

        //navbar copy button
        $(elementIDs.executedBatchesGridJQ).jqGrid('navButtonAdd', pagerElement,
                {
                    caption: '', buttonicon: 'ui-icon-arrowthickstop-1-s', title: 'Download Audit Report', id: 'DownloadAuditReport',
                    onClickButton: function () {
                        var rowId = $(elementIDs.executedBatchesGridJQ).getGridParam('selrow');

                        if (rowId !== undefined && rowId !== null) {
                            var rowData = $(elementIDs.executedBatchesGridJQ).getRowData(rowId);
                            var batchId = rowData.BatchID
                            //  batchId = '9f8519c0-26c6-4eb3-bcb0-9a5f81a054df';

                            var checkAuditReportUrl = URL.checkAuditReport.replace(/\{batchID\}/g, batchId);
                            var promise = ajaxWrapper.getJSON(checkAuditReportUrl);
                            //register ajax success callback
                            promise.done(function (result) {
                                if (result.Result === ServiceResult.SUCCESS) {
                                    window.location = URL.downloadAuditReport.replace(/\{batchID\}/g, batchId);
                                }
                                else if (result.Result === ServiceResult.FAILURE) {
                                    messageDialog.show(GlobalUpdateMessages.UnAvailableAuditReport);
                                }
                            });
                            //register ajax failure callback
                            promise.fail(showError);
                        }

                        else {
                            messageDialog.show(GlobalUpdateMessages.selectRowMessage);
                        }

                        //DownloadAuditReportExcel();
                        // window.location = "/GlobalUpdate/Download?BatchId=testBatch"; 
                    }
                });

        $(elementIDs.executedBatchesGridJQ).jqGrid('navButtonAdd', pagerElement,
                {
                    caption: '', buttonicon: 'ui-icon-arrowreturnthick-1-w', title: 'Rollback Batch', id: 'RollbackBatch',
                    onClickButton: function () {
                        var rowId = $(this).getGridParam('selrow');
                        if (rowId !== undefined && rowId !== null) {
                            var rowData = $(elementIDs.executedBatchesGridJQ).getRowData(rowId);
                            rollBackRowData = rowData;
                            $(elementIDs.viewBatchDialogJQ).attr('disabled', 'disable');
                            laodRollBackDialog();
                        }
                        else {
                            messageDialog.show(GlobalUpdateMessages.selectRowMessage);
                        }
                    }
                });
    }
    //Rollback Dialog
    function laodRollBackDialog() {
        $(elementIDs.rollBackDialogJQ).dialog({
            title: 'Rollback',
            height: 160,
            width: 400
        });
    }

    $(elementIDs.saveRollBackButtonJQ).click(function () {
        if ($(elementIDs.rollbackCommntsJQ).val() == "") {
            messageDialog.show("Please enter comments before you Rollback!!");
        } else {
            loadRollBackConfirmationBox(rollBackRowData);
        }
    });

    //Rollback Confirmation dialog
    function loadRollBackConfirmationBox(rollBackRowData) {
        $(elementIDs.confirmRollBackDialogJQ).find('div p').text("Are you sure you want to Rollback?");
        $(elementIDs.confirmRollBackDialogJQ).dialog({
            title: 'Confirm Rollback',
            height: 120,
            buttons: {
                Yes: function () {
                    RollBackBatchExecution(rollBackRowData);
                },
                No: function () {
                    $(this).dialog("close");
                }
            }
        });
    }
    //RollBack function
    function RollBackBatchExecution(rollBackRowData) {
        var batchID = rollBackRowData.BatchID;
        var batchExeEndTime = new Date(rollBackRowData.EndDateTime);
        var currentTime = new Date();
        var dateDiff = currentTime.getTime() - batchExeEndTime.getTime();
        var resultInMinutes = Math.round(dateDiff / 60000);
        var timeDiffInHours = Math.round(resultInMinutes / 60);
        var rollbackComments = $(elementIDs.rollbackCommntsJQ).val();
        var url = URL.rollBackBatch;
        var params = {
            batchID: batchID,
            batchExeEndTime: batchExeEndTime,
            rollbackComment: rollbackComments
        };
        var promise = ajaxWrapper.postJSONCustom(url, params);
        //register ajax success callback
        promise.done(function (xhr) {
            if (xhr.Result === ServiceResult.SUCCESS) {
                messageDialog.show("Batch Rollbacked successfully!");
                $(elementIDs.confirmRollBackDialogJQ).dialog('close');
                $(elementIDs.rollBackDialogJQ).dialog('close');
                loadExecutedBatchesGrid();
            } else if (xhr.Result === ServiceResult.FAILURE) {
                //messageDialog.show(xhr.Items[0].Messages[0]);
            }
            else {
                messageDialog.show(Common.errorMsg);
            }
        });
        //register ajax failure callback
        promise.fail(showError);
    }

    function loadImportedNotAddedIASGrid() {
        //set column list for grid
        var colArray = ['Global Update ID', 'Global Update Name', 'Effective Date From', 'Effective Date To', 'Added By', 'Added Date', 'Include'];
        var url;

        //set column models
        var colModel = [];
        colModel.push({ name: 'GlobalUpdateID', index: 'GlobalUpdateID', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'GlobalUpdateName', index: 'GlobalUpdateName', editable: false, align: 'left' });
        colModel.push({ name: 'EffectiveDateFrom', index: 'EffectiveDateFrom', editable: false, align: 'center' });
        colModel.push({ name: 'EffectiveDateTo', index: 'EffectiveDateTo', editable: false, align: 'center' });
        colModel.push({ name: 'AddedBy', index: 'AddedBy', editable: false, align: 'center' });
        colModel.push({ name: 'AddedDate', index: 'AddedDate', editable: false, align: 'center' });
        colModel.push({
            name: 'Include', index: 'Include', align: 'Center', width: '45px', align: 'center', editable: true, formatter: batchAddCheckBoxFormatter,
            unformatter: batchAddCheckBoxUnFormat, edittype: "checkbox", sortable: false,
            editoptions: { value: "Yes:No", defaultValue: "Yes" },
            stype: "select", searchoptions: {
                sopt: ["eq", "ne"],
                value: ":Any;true:Yes;false:No"
            }
        });

        //$("#existinBatchesDiv").addClass('hidden');
        //clean up the grid first - only table element remains after this
        $(elementIDs.addBatchGridJQ).jqGrid('GridUnload');
        //added to beforeProcessing must work
        $(elementIDs.addBatchGridJQ).jqGrid("destroyFilterToolbar");
        //adding the pager element
        $(elementIDs.addBatchGridJQ).parent().append("<div id='p" + elementIDs.addBatchGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.addBatchGridJQ).jqGrid({
            url: URL.importedNotAddedIASList,
            datatype: "json",
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Impact Assessment List',
            height: '350',
            rowNum: 20,
            rowList: [10, 20, 30],
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            rowList: [],        // disable page size dropdown
            pgbuttons: false,     // disable page control like next, back button
            pgtext: null,         // disable pager text like 'Page 0 of 10'
            viewrecords: false,    // disable current view record text like 'View 1-10 of 100' 
            onSelectRow: function (id) {

            },
            beforeProcessing: function (data) {
                $.each(data, function (i, dt) {
                    var selectedRowData = dt;
                    var EffectiveDateFrom = new Date(selectedRowData.EffectiveDateFrom);
                    selectedRowData.EffectiveDateFrom = formatDateForSort(EffectiveDateFrom, false);

                    var EffectiveDateTo = new Date(selectedRowData.EffectiveDateTo);
                    selectedRowData.EffectiveDateTo = formatDateForSort(EffectiveDateTo, false);

                    var AddedDate = new Date(selectedRowData.AddedDate);
                    selectedRowData.AddedDate = formatDateForSort(AddedDate, false);
                });
            },
            pager: '#p' + elementIDs.addBatchGrid,
            gridComplete: function () {
            }
        });
        var pagerElement = '#p' + elementIDs.addBatchGrid;
        //remove default buttons
        $(elementIDs.addBatchGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.addBatchGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });


    }

    function getGUIdsFromBatchMap(batchID) {
        var guIdsFromBatchMap = new Array();

        url = URL.GetGUIdsFromBatchMap.replace(/\{batchID\}/g, batchID);
        var promise = ajaxWrapper.getJSON(url);
        //register ajax success callback
        promise.done(function (xhr) {
            for (i = 0; i < xhr.length; i++) {
                guIdsFromBatchMap.push(xhr[i].GlobalUpdateID);
            }
        });
        //register ajax failure callback
        promise.fail(showError);
    }

    function editBatchGrid(batchID) {

        //set column list for grid
        var colArray = ['Global Update ID', 'Global Update Name', 'Effective Date From', 'Effective Date To', 'Added By', 'Added Date', 'Include'];
        var url;

        //set column models
        var colModel = [];
        colModel.push({ name: 'GlobalUpdateID', index: 'GlobalUpdateID', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'GlobalUpdateName', index: 'GlobalUpdateName', editable: false, align: 'left' });
        colModel.push({ name: 'EffectiveDateFrom', index: 'EffectiveDateFrom', editable: false, align: 'center' });
        colModel.push({ name: 'EffectiveDateTo', index: 'EffectiveDateTo', editable: false, align: 'center' });
        colModel.push({ name: 'AddedBy', index: 'AddedBy', editable: false, align: 'center' });
        colModel.push({ name: 'AddedDate', index: 'AddedDate', editable: false, align: 'center' });
        colModel.push({
            name: 'Include', index: 'Include', align: 'Center', width: '45px', align: 'center', editable: true, formatter: batchAddCheckBoxFormatter,
            unformatter: batchAddCheckBoxUnFormat, edittype: "checkbox", sortable: false,
            editoptions: { value: "Yes:No", defaultValue: "Yes" },
            stype: "select", searchoptions: {
                sopt: ["eq", "ne"],
                value: ":Any;true:Yes;false:No"
            }
        });
        //$("#existinBatchesDiv").addClass('hidden');
        //clean up the grid first - only table element remains after this
        $(elementIDs.editBatchGridJQ).jqGrid('GridUnload');
        //added to beforeProcessing must work
        $(elementIDs.editBatchGridJQ).jqGrid("destroyFilterToolbar");
        //adding the pager element
        $(elementIDs.editBatchGridJQ).parent().append("<div id='p" + elementIDs.editBatchGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.editBatchGridJQ).jqGrid({
            url: URL.editBatchIASListGrid.replace(/\{batchID\}/g, batchID),
            datatype: "json",
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Impact Assessment List',
            height: '350',
            rowNum: 20,
            rowList: [10, 20, 30],
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            rowList: [],        // disable page size dropdown
            pgbuttons: false,     // disable page control like next, back button
            pgtext: null,         // disable pager text like 'Page 0 of 10'
            viewrecords: false,    // disable current view record text like 'View 1-10 of 100' 
            onSelectRow: function (id) {

            },
            beforeProcessing: function (data) {
                $.each(data, function (i, dt) {
                    var selectedRowData = dt;
                    var EffectiveDateFrom = new Date(selectedRowData.EffectiveDateFrom);
                    selectedRowData.EffectiveDateFrom = formatDateForSort(EffectiveDateFrom, false);

                    var EffectiveDateTo = new Date(selectedRowData.EffectiveDateTo);
                    selectedRowData.EffectiveDateTo = formatDateForSort(EffectiveDateTo, false);

                    var AddedDate = new Date(selectedRowData.AddedDate);
                    selectedRowData.AddedDate = formatDateForSort(AddedDate, false);
                });
            },
            pager: '#p' + elementIDs.editBatchGrid
        });
        var pagerElement = '#p' + elementIDs.editBatchGrid;
        //remove default buttons
        $(elementIDs.editBatchGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.editBatchGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

    }

    function viewBatchGrid(batchID) {
        //set column list for grid
        var colArray = ['Global Update ID', 'Global Update Name', 'Effective Date From', 'Effective Date To', 'Added By', 'Added Date', 'Include'];
        var url;

        //set column models
        var colModel = [];
        colModel.push({ name: 'GlobalUpdateID', index: 'GlobalUpdateID', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'GlobalUpdateName', index: 'GlobalUpdateName', editable: false, align: 'left' });
        colModel.push({ name: 'EffectiveDateFrom', index: 'EffectiveDateFrom', editable: false, align: 'center' });
        colModel.push({ name: 'EffectiveDateTo', index: 'EffectiveDateTo', editable: false, align: 'center' });
        colModel.push({ name: 'AddedBy', index: 'AddedBy', editable: false, align: 'center' });
        colModel.push({ name: 'AddedDate', index: 'AddedDate', editable: false, align: 'center' });
        colModel.push({
            name: 'Include', index: 'Include', align: 'Center', width: '45px', align: 'center', editable: true, formatter: batchAddCheckBoxFormatter,
            unformatter: batchAddCheckBoxUnFormat, edittype: "checkbox", sortable: false,
            editoptions: { value: "Yes:No", defaultValue: "Yes" },
            stype: "select", searchoptions: {
                sopt: ["eq", "ne"],
                value: ":Any;true:Yes;false:No"
            }
        });
        //$("#existinBatchesDiv").addClass('hidden');
        //clean up the grid first - only table element remains after this
        $(elementIDs.viewBatchGridJQ).jqGrid('GridUnload');
        //added to beforeProcessing must work
        $(elementIDs.viewBatchGridJQ).jqGrid("destroyFilterToolbar");
        //adding the pager element
        $(elementIDs.viewBatchGridJQ).parent().append("<div id='p" + elementIDs.viewBatchGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.viewBatchGridJQ).jqGrid({
            url: URL.viewBatchIASListGrid.replace(/\{batchID\}/g, batchID),
            datatype: "json",
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Impact Assessment List',
            height: '350',
            rowNum: 20,
            rowList: [10, 20, 30],
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            rowList: [],        // disable page size dropdown
            pgbuttons: false,     // disable page control like next, back button
            pgtext: null,         // disable pager text like 'Page 0 of 10'
            viewrecords: false,    // disable current view record text like 'View 1-10 of 100' 
            beforeProcessing: function (data) {
                $.each(data, function (i, dt) {
                    var selectedRowData = dt;
                    var EffectiveDateFrom = new Date(selectedRowData.EffectiveDateFrom);
                    selectedRowData.EffectiveDateFrom = formatDateForSort(EffectiveDateFrom, false);

                    var EffectiveDateTo = new Date(selectedRowData.EffectiveDateTo);
                    selectedRowData.EffectiveDateTo = formatDateForSort(EffectiveDateTo, false);

                    var AddedDate = new Date(selectedRowData.AddedDate);
                    selectedRowData.AddedDate = formatDateForSort(AddedDate, false);
                });
            },
            onSelectRow: function (id) {

            },
            pager: '#p' + elementIDs.viewBatchGrid
        });
        var pagerElement = '#p' + elementIDs.viewBatchGrid;
        //remove default buttons
        $(elementIDs.viewBatchGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.viewBatchGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

    }

    function loadAddIASPopUp() {
        $(elementIDs.addBatchDialogJQ).dialog(
            {
                height: 580,
                width: 1300,
                modal: true,
                "title": "Add Batch",
                open: function () {
                    $("#executionType option:first").attr("selected", true);
                    loadImportedNotAddedIASGrid();
                },
                close: function () {
                    InitializeControls();
                }
            }
            );
    }

    function loadEditIASPopUp(batchId, realTSch) {
        $(elementIDs.editBatchDialogJQ).dialog(
            {
                height: 580,
                width: 1300,
                modal: true,
                "title": "Edit Batch",
                open: function () {
                    if (realTSch == true) {
                        $(elementIDs.editScheduledDateJQ).attr('disabled', 'disable');
                        $(elementIDs.editScheduledTimeJQ).attr('disabled', 'disable');
                        $(elementIDs.editScheduledDateJQ).val('');
                        $(elementIDs.editScheduledTimeJQ).val('');
                    }
                    else {
                        $(elementIDs.editScheduledDateJQ).removeAttr("disabled");
                        $(elementIDs.editScheduledTimeJQ).removeAttr("disabled");
                    }
                    editBatchGrid(batchId);
                },
                close: function () {
                    InitializeControls();
                }
            }
            );
    }

    function loadViewIASPopUp(batchId) {
        $(elementIDs.viewBatchDialogJQ).dialog(
            {
                height: 580,
                width: 1300,
                modal: true,
                "title": "View Batch",
                open: function () {
                    viewBatchGrid(batchId);
                },
                close: function () {
                    //InitializeControls();
                }
            }
            );
    }

    function loadBatchApprovalDialog(isApproved) {
        $(elementIDs.batchNameAprJQ).attr('disabled', 'disable');
        $(elementIDs.executionTypeAprJQ).attr('disabled', 'disable');
        $(elementIDs.scheduledDateAprJQ).attr('disabled', 'disable');
        $(elementIDs.scheduledTimeAprJQ).attr('disabled', 'disable');
        if (isApproved == "Yes") {
            messageDialog.show(GlobalUpdateMessages.alreadyApprovedBatch);
        }
        else {
            $(elementIDs.approveBatchDialogJQ).dialog(
              {
                  height: 280,
                  width: 450,
                  modal: true,
                  "title": "Approve Batch",
                  open: function () {
                  },
                  close: function () {
                      $(elementIDs.approveBatchCheckJQ).attr('checked', false);
                  }
              }
              );
        }

    }

    function SaveBatch() {
        var stringUtility = new globalutilities();

        var batchName = $(elementIDs.batchNameJQ).val();
        var executionType = $(elementIDs.executionTypeJQ).val();
        var scheduledDate = executionType == ExecutionType.Scheduled ? $(elementIDs.scheduledDateJQ).val() : '01/01/1900';
        var scheduledTime = executionType == ExecutionType.Scheduled ? $(elementIDs.scheduledTimeJQ).val() : '00:00:00';
        var rows = $(elementIDs.addBatchGridJQ).getDataIDs();
        globalUpdateIDArray = new Array();
        var splittedString = scheduledTime.split(':');
        if (splittedString[0] > 12 || splittedString[1] > 60 || splittedString[2] > 60) {
            messageDialog.show(GlobalUpdateMessages.timespanValidation);
        }
        else {
            if ((stringUtility.stringMethods.isNullOrEmpty(batchName)) || ((stringUtility.stringMethods.isNullOrEmpty(scheduledDate) || stringUtility.stringMethods.isNullOrEmpty(scheduledTime)) && executionType == 'Scheduled')) {
                messageDialog.show(GlobalUpdateMessages.enterAllValues);
            }
            else {
                if (($(elementIDs.addBatchGridJQ).find("input:checked")).length != 0) {
                    $(elementIDs.addBatchGridJQ).find("input:checked").each(function (idx, control) {
                        var GlobalUpdateID = $(this).attr('name').split('_')[1];
                        globalUpdateIDArray.push(GlobalUpdateID);
                    });
                    var params = {
                        batchName: batchName,
                        executionType: executionType,
                        scheduleDate: scheduledDate,
                        scheduledTime: scheduledTime,
                        globalUpdateIDArray: JSON.stringify(globalUpdateIDArray),
                    }
                    //check if multiple folders exist in selected IAS
                    url = URL.checkDuplicateFolderVersionExistsInSelectedBatchIAS;
                    var promise1 = ajaxWrapper.postJSON(url, params);
                    promise1.done(function (xhr1) {
                        if (xhr1.Result === ServiceResult.SUCCESS) {
                            url = URL.saveBatch;
                            var promise = ajaxWrapper.postJSON(url, params);
                            //register ajax success callback
                            promise.done(function (xhr) {
                                if (xhr.Result === ServiceResult.SUCCESS) {
                                    $(elementIDs.addBatchDialogJQ).dialog('close');
                                    InitializeControls();
                                    messageDialog.show(GlobalUpdateMessages.batchAddSuccess);
                                    loadBatchesGrid();
                                }
                                else if (xhr.Result === ServiceResult.FAILURE) {
                                    messageDialog.show(xhr.Items[0].Messages[0]);
                                }
                                else {
                                    messageDialog.show(Common.errorMsg);
                                }
                            });
                            //register ajax failure callback
                            promise.fail(showError);
                        }
                        else {
                            messageDialog.show(GlobalUpdateMessages.uniqueFloderInIASBatch);
                        }
                    });
                    promise1.fail(showError);
                }
                else {
                    messageDialog.show(GlobalUpdateMessages.selectIASMessage);
                }
            }
        }
    }

    function updateBatch(batchId) {
        var stringUtility = new globalutilities();
        var batchName = $(elementIDs.editBatchNameJQ).val();
        var executionType = $(elementIDs.editExecutionTypeJQ).val();
        var scheduledDate = executionType == ExecutionType.Scheduled ? $(elementIDs.editScheduledDateJQ).val() : '01/01/1900';
        var scheduledTime = executionType == ExecutionType.Scheduled ? $(elementIDs.editScheduledTimeJQ).val() : '00:00:00';
        var rows = $(elementIDs.editBatchGridJQ).getDataIDs();
        globalUpdateIDArray = new Array();
        var splittedString = scheduledTime.split(':');
        if (splittedString[0] > 12 || splittedString[1] > 60 || splittedString[2] > 60) {
            messageDialog.show(GlobalUpdateMessages.timespanValidation);
        } else {
            if ((stringUtility.stringMethods.isNullOrEmpty(batchName)) || ((stringUtility.stringMethods.isNullOrEmpty(scheduledDate) || stringUtility.stringMethods.isNullOrEmpty(scheduledTime)) && executionType == 'Scheduled')) {
                messageDialog.show(GlobalUpdateMessages.enterAllValues);
            }
            else {
                if (($(elementIDs.editBatchGridJQ).find("input:checked")).length != 0) {
                    var batchGlobalUpdateArray = new Array();
                    $(elementIDs.editBatchGridJQ).find("input:checked").each(function (idx, control) {
                        var GlobalUpdateID = $(this).attr('name').split('_')[1];
                        batchGlobalUpdateArray.push(GlobalUpdateID);
                    });
                    var params = {
                        batchName: batchName,
                        executionType: executionType,
                        scheduleDate: scheduledDate,
                        scheduledTime: scheduledTime,
                        globalUpdateIDArray: JSON.stringify(batchGlobalUpdateArray),
                        batchId: batchId
                    }
                    //check if multiple folders exist in selected IAS
                    url = URL.checkDuplicateFolderVersionExistsInSelectedBatchIAS;
                    var promise1 = ajaxWrapper.postJSON(url, params);
                    promise1.done(function (xhr1) {
                        if (xhr1.Result === ServiceResult.SUCCESS) {
                            url = URL.updateBatch;
                            var promise = ajaxWrapper.postJSON(url, params);
                            //register ajax success callback
                            promise.done(function (xhr) {
                                if (xhr.Result === ServiceResult.SUCCESS) {
                                    $(elementIDs.editBatchDialogJQ).dialog('close');
                                    InitializeControls();
                                    messageDialog.show(GlobalUpdateMessages.batchUpdateSuccess);
                                    loadBatchesGrid();
                                }
                                else if (xhr.Result === ServiceResult.FAILURE) {
                                    messageDialog.show(xhr.Items[0].Messages[0]);
                                }
                                else {
                                    messageDialog.show(Common.errorMsg);
                                }
                            });
                            //register ajax failure callback
                            promise.fail(showError);
                        }
                        else {
                            messageDialog.show(GlobalUpdateMessages.uniqueFloderInIASBatch);
                        }
                    });
                    promise1.fail(showError);
                }
                else {
                    messageDialog.show(GlobalUpdateMessages.selectIASMessage);
                }
            }
        }
    }

    function updateBatchWithApprovedStatus(batchName) {
        var approvedStatus = true;
        url = URL.approveBatch.replace(/\{batchName\}/g, batchName);
        var promise = ajaxWrapper.postJSON(url);
        //register ajax success callback
        promise.done(function (xhr) {
            if (xhr.Result === ServiceResult.SUCCESS) {
                $(elementIDs.approveBatchDialogJQ).dialog('close');
                $(elementIDs.approveBatchCheckJQ).attr('checked', false);
                //$(elementIDs.existinBatchesGridJQ).setGridParam({ page: 1 }).trigger("reloadGrid")
                //$(elementIDs.existinBatchesGridJQ).trigger("reloadGrid");
                messageDialog.show(GlobalUpdateMessages.approveBatchSuccess);
                loadBatchesGrid();
            }
            else if (xhr.Result === ServiceResult.FAILURE) {
                messageDialog.show(GlobalUpdateMessages.alreadyApprovedBatch);
            }
            else {
                messageDialog.show(Common.errorMsg);
            }
        });
        //register ajax failure callback
        promise.fail(showError);
    }

    function batchAddCheckBoxFormatter(cellValue, options, rowObject) {

        if (rowObject.Include == false)
            return "<input type='checkbox' id='checkbox_" + rowObject.GlobalUpdateID + "' name= 'checkbox_" + rowObject.GlobalUpdateID + "'/>";
        else
            return "<input type='checkbox' id='checkbox_" + rowObject.GlobalUpdateID + "' name= 'checkbox_" + rowObject.GlobalUpdateID + "' checked/>";

    }

    function batchAddCheckBoxUnFormat(cellValue, options) {
        var result;
        result = $(this).find('#checkbox_' + rowObject.GlobalUpdateID).find('input').prop('checked');
        return result;
    }

    function DeleteGlobalUpdateBatch(batchId) {

        var deleteBatchUrl = URL.deleteGlobalUpdateBatch.replace(/\{batchId\}/g, batchId);
        var promise = ajaxWrapper.getJSON(deleteBatchUrl);
        //register ajax success callback
        promise.done(function (result) {
            if (result.Result === ServiceResult.SUCCESS) {
                messageDialog.show(GlobalUpdateMessages.deleteBatchSuccessResult);
                loadBatchesGrid();
            }
            else
                messageDialog.show(GlobalUpdateMessages.deleteBatchErrorResult);

        });
        //register ajax failure callback
        promise.fail(showError);
    }
 
    /* Commented Temporarily : For other priority fixes.
    $(elementIDs.btnScheduleBatchExecutionJQ).click(function () {
        var hasError = ValidateBatchSchedularInputs();

        if (!hasError) {
            SaveBatchExecutionSchedule();
        }
        else {
            messageDialog.show(GlobalUpdateMessages.scheduledExecutionValidationMessage);
        }

    });

    function SaveBatchExecutionSchedule() {
        var schedularName = $(elementIDs.txtSchedulerNameJQ).val();
        var executionDate = $(elementIDs.txtExecutionDateJQ).val();
        var executionTime = $(elementIDs.txtExecutionTimeJQ).val();

        scheduleExecution = {
            schedularName: schedularName,
            executionDate: executionDate,
            executionTime: executionTime
        }

        var promise = ajaxWrapper.postJSONCustom(URLs.saveExecutionSchedule, scheduleExecution);
        promise.done(function (result) {
            if (result.Result === ServiceResult.SUCCESS) {
                messageDialog.show(GlobalUpdateMessages.scheduledExecutionSuccess);
            }
            else {
                messageDialog.show(GlobalUpdateMessages.scheduledExecutionFailed);
            }
        });
        //register ajax failure callback
        promise.fail(showError);
    }
    */

    $(elementIDs.approveBatchButtonJQ).click(function () {
        if ($(elementIDs.approveBatchCheckJQ).is(':checked')) {
            var batchName = $(elementIDs.batchNameAprJQ).val();
            updateBatchWithApprovedStatus(batchName);
        }
        else {
            messageDialog.show(GlobalUpdateMessages.selectCheckBox);
        }
    });



    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
}();

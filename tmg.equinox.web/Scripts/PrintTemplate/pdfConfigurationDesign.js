var pdfConfigurationDesign = function () {
    //variables required for tab management
    var tabs;
    var tabIndex = 1;
    var tabCount = 0;
    var tabNamePrefix = 'DesignTemplate-';
    this.elementIDs = {
        documentDesignJQ: '#designConfigtab',
        //table element for the Templte Document  Grid
        templateDesignGrid: 'templateDesign',
        //with hash for use with jQuery
        templateDesignGridJQ: '#templateDesign',
        //table element for the Templte Document  Grid
        templateUIElementGrid: 'templateUIelementMapping',
        //with hash for use with jQuery
        templateUIElementGridJQ: '#templateUIelementMapping',
        btnFormDesignAddJQ: '#btnFormDesignAdd',
        btnDocumentTemplateDeleteJQ: '#btnDocumentTemplateDelete'
    };
    this.URLs = {
        //get Document Template List
        getDocumentTemplateList: '/PrintTemplate/DocumentTemplateList?tenantId=1',
        getTemplateDesginUIElementList: '/PrintTemplate/LoadTemplateDesignUIElementList?tenantId=1&formDesignVersionId={formDesignVersionId}&templateId={templateId}',
        setTemplateDesignUIElement: '/PrintTemplate/SetTemplateDesignUIElement',
        designTemplateDelete: '/PrintTemplate/deleteDesignTemplate',
    }

    function init() {
        $(document).ready(function () {
            ////To remove style attribute so that Document Design tab is displayed after loading the  page. 
            $(elementIDs.documentDesignJQ).removeAttr("style");
            ////jqueryui tabs
            tabs = $(elementIDs.documentDesignJQ).tabs();
            ////register event for closing a tab page - refer jquery ui documentation
            ////event will be registered for each tab page loaded
            tabs.delegate('span.ui-icon-close', 'click', function () {
                var panelId = $(this).closest('li').remove().attr('aria-controls');
                $('#' + panelId).remove();
                tabCount--;
                tabIndex = 1;
                tabs.tabs('refresh');
            });
            //load the form design grid
            loadDocumentTemplateGrid();
        });
    }
    init();
    //load Document Template list in grid
    function loadDocumentTemplateGrid() {
        //set column list for grid
        var colArray = ['TemplateId', '', 'Document Design Name', '', 'Version', 'Template Name','Description'];
        //set column models
        var colModel = [];
        colModel.push({ name: 'TemplateID', index: 'TemplateID', key: true, hidden: true, editable: false });
        colModel.push({ name: 'FormDesignID', index: 'FormDesignId', hidden: true });
        colModel.push({ name: 'FormDesignName', index: 'FormDesignName', editable: false });
        colModel.push({ name: 'FormDesignVersionID', index: 'FormDesignVersionID', hidden: true, search: false, });
        colModel.push({ name: 'VersionName', index: 'VersionName', editable: false });
        colModel.push({ name: 'TemplateName', index: 'TemplateName', editable: false });
        colModel.push({ name: 'Description', index: 'Description', editable: false });

        //clean up the grid first - only table element remains after this
        $(elementIDs.templateDesignGridJQ).jqGrid('GridUnload');
        var url = URLs.getDocumentTemplateList;
        //adding the pager element
        $(elementIDs.templateDesignGridJQ).parent().append("<div id='p" + elementIDs.templateDesignGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.templateDesignGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Template List',
            height: '350',
            rowNum: 25,
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.templateDesignGrid,
            sortname: 'TemplateID',
            altclass: 'alternate',
            //load associated form design version grid on selecting a row
            onSelectRow: function (id) {
                var row = $(this).getRowData(id);
                loadTemplateUIElementGrid(row);
                $("#btnUIelementSave").hide();
            },
            //on adding a new form design, reload the grid and set the row to selected
            gridComplete: function () {
                var allRowsInGrid = $(this).getRowData();
                if (allRowsInGrid.length > 0) {
                    loadTemplateUIElementGrid(allRowsInGrid[0])
                    $("#btnUIelementSave").hide();
                }
                else {
                    $(elementIDs.templateUIElementGridJQ).jqGrid('GridUnload');
                }

                checkPDFGenerationClaims(elementIDs, claims);
            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.templateDesignGridJQ);
            }
        });
        var pagerElement = '#p' + elementIDs.templateDesignGrid;
        //remove default buttons
        $(elementIDs.templateDesignGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        //add button in footer of grid that pops up the add form design dialog
        $(elementIDs.templateDesignGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-plus',
            title: 'Add', id: 'btnFormDesignAdd',
            onClickButton: function () {               
                pdfConfigurationDialog.show();
            }
        });
        //delete button in footer of grid that pops up the delete form design dialog
        $(elementIDs.templateDesignGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-trash',
            title: 'Delete', id: 'btnDocumentTemplateDelete',
            onClickButton: function () {
                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    //get the formDesignVersion List for current form Design
                    var DocumentTemplateList = $(elementIDs.templateDesignGridJQ).jqGrid("getRowData", rowId);
                    if (DocumentTemplateList !== undefined && DocumentTemplateList.length > 0) {
                        messageDialog.show(DocumentDesign.designDeleteValidationMsg);
                    }
                    else {
                        //load confirm dialogue to asset the operation
                        confirmDialog.show(Common.deleteConfirmationMsg, function () {
                            confirmDialog.hide();
                            //delete the template design
                            var designTemplate = {
                                tenantId: 1,
                                templateId: DocumentTemplateList.TemplateID
                            };
                            var promise = ajaxWrapper.postJSON(URLs.designTemplateDelete, designTemplate);
                            //register ajax success callback
                            promise.done(templateDeleteSuccess);
                            //register ajax failure callback
                            promise.fail(pdfConfigurationDialog.showError);
                        });
                    }
                }
            }
        });
        // add filter toolbar to the top
        $(elementIDs.templateDesignGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });
    }

    function loadTemplateUIElementGrid(currentRow) {
        var currentInstance = this;

        $(elementIDs.templateUIElementGridJQ).jqGrid('GridUnload');
        //set column list
        var colArray = ['TenantID', 'TemplateUIMapID', 'TemplateID', 'UIElementID', 'Label', 'Include in PDF', 'IsActive'];
        //set column models
        var colModel = [];
        colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true, search: false });
        colModel.push({ name: 'TemplateUIMapID', index: 'TemplateUIMapID', hidden: true, search: false });
        colModel.push({ name: 'TemplateID', index: 'TemplateID', hidden: true, search: false });
        colModel.push({ name: 'UIElementID', index: 'UIElementID', key: true, hidden: true, align: 'center', search: false });
        colModel.push({ name: 'Label', index: 'Label', align: 'left', editable: false });
        colModel.push({ name: 'IncludeInPDF', index: 'IncludeInPDF', align: 'Center', editable: false, formatter: checkBoxFormatter, unformatter: unFormatIncludedColumn, sortable: false });
        colModel.push({ name: 'isActive', index: 'isActive', hidden: true, align: 'left', editable: false });
        //get URL for grid
        var url = this.URLs.getTemplateDesginUIElementList.replace(/\{formDesignVersionId\}/g, currentRow.FormDesignVersionID)
                                                       .replace(/\{templateId\}/g, currentRow.TemplateID);
        //add footer of grid
        $(elementIDs.templateUIElementGridJQ).parent().append("<div id='p" + elementIDs.templateUIElementGrid + "'></div>");
        //load grid
        $(elementIDs.templateUIElementGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            treeGrid: true,
            treeGridModel: 'adjacency', // set this for hierarchical grid
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: currentRow.FormDesignName + ' - ' + currentRow.VersionName + ' - UI Elements',
            autowidth: true,
            forceFit: true,
            loadonce: true,
            rowNum: 1000,
            height: '375',
            expanded: true,
            ExpandColumn: 'Label',
            pager: '#p' + elementIDs.templateUIElementGrid,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            altclass: 'alternate',
            gridComplete: function () {
                var allRowsInGrid = $(this).getRowData();
                var grid = this;
                for (var i = 0; i < allRowsInGrid.length; i++) {
                    if (allRowsInGrid[i].parent == 0) {
                        $(elementIDs.templateUIElementGridJQ + " tr#" + allRowsInGrid[i].UIElementID).attr("disabled", "disabled");
                        break;
                    }
                }
                var allRowsInGrid = $(elementIDs.templateUIElementGridJQ).getRowData();;
                for (var i = 0; i < allRowsInGrid.length; i++) {
                    $(elementIDs.templateUIElementGridJQ + " tr#" + allRowsInGrid[i].UIElementID).attr("disabled", "disabled");
                }
                $(grid).find('btnUIelementSave').addClass('Hide');
                //include in PDF true/false
                $(grid).find("input:checkbox").each(function (idx, control) {
                    $(control).click(function () {
                        if (idx > 0) {
                            var allrowData = $(elementIDs.templateUIElementGridJQ).jqGrid("getRowData");
                            var selectedRowData = allrowData[idx];
                            if (selectedRowData.level != 1) {
                                var uielementId = selectedRowData.parent;
                                do {
                                    if (control.checked == false) {
                                        var rowId;
                                        var isSubsection = allrowData.filter(function (rowdata, id) {
                                            rowId = id;
                                            return (rowdata.parent == uielementId && $('#checkbox_' + rowdata.UIElementID).prop('checked') == true);
                                        });
                                        if (isSubsection.length == 0)
                                            $('#checkbox_' + uielementId).prop('checked', control.checked);
                                    }
                                    else {
                                        $('#checkbox_' + uielementId).prop('checked', control.checked);
                                    }
                                    var parentRowData = allrowData.filter(function (rowdata) {
                                        return rowdata.UIElementID == uielementId;
                                    });
                                    if (parentRowData.length > 0)
                                        uielementId = parentRowData[0].parent;
                                } while (parentRowData[0].level != 1);
                            }

                            var allGridData = $(elementIDs.templateUIElementGridJQ).jqGrid('getGridParam', 'data');
                            var row = idx + 1;
                            for (var i = row; i < allRowsInGrid.length; i++) {
                                if (allRowsInGrid[idx].level < allRowsInGrid[i].level) {
                                    //   $(elementIDs.templateUIElementGridJQ).jqGrid('setCell', i, "isActive", control.checked);
                                    $('#checkbox_' + allRowsInGrid[i].UIElementID).prop('checked', control.checked);
                                }
                                else {
                                    break;
                                }
                            }
                        }
                    });
                });

                $(".treeclick", elementIDs.templateUIElementGridJQ).each(function () {
                    if ($(this).hasClass("tree-plus")) {
                        $(this).trigger("click");
                    }
                });

            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.templateUIElementGridJQ);
            }
        });
        //remove paging
        var pagerElement = '#p' + elementIDs.templateUIElementGrid;
        $(elementIDs.templateUIElementGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        $(elementIDs.templateUIElementGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-pencil',
            title: 'Edit', id: 'btnDocumnetTemplateEdit',
            onClickButton: function () {
                var allRowsInGrid = $(elementIDs.templateUIElementGridJQ).getRowData();;
                for (var i = 1; i < allRowsInGrid.length; i++) {
                    $(elementIDs.templateUIElementGridJQ + " tr#" + allRowsInGrid[i].UIElementID).removeAttr("disabled", "disabled");
                }
                $("#btnDocumnetTemplateEdit").hide();
                $("#btnUIelementSave").show();
            }
        });
        $(elementIDs.templateUIElementGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: 'Save', id: 'btnUIelementSave',
            onClickButton: function () {
                var uiElementList = new Array();
                var allRowData = $(elementIDs.templateUIElementGridJQ).jqGrid('getRowData');
                for (var i = 0 ; i < allRowData.length; i++) {
                    var TemplateUIMapID = allRowData[i].TemplateUIMapID;
                    var UIElementID = allRowData[i].UIElementID;
                    var isActive = $('#checkbox_' + allRowData[i].UIElementID + '').prop('checked');
                    uiElementList[i] = new Object({
                        UIElementID: UIElementID,
                        IncludeInPDF: isActive,
                        TemplateUIMapID: TemplateUIMapID
                    });
                }
                var templateDesignUIElementData = {
                    tenantID: 1,
                    templateID: allRowData[0].TemplateID,
                    uiElementList: JSON.stringify(uiElementList)
                };
                var templateDesignUIElementDataList = templateDesignUIElementData;
                url = currentInstance.URLs.setTemplateDesignUIElement;
                var promise = ajaxWrapper.postJSON(url, templateDesignUIElementDataList);
                //register ajax success callback
                promise.done(templateDesignSuccess);
                //register ajax failure callback
                promise.fail(pdfConfigurationDialog.showError);
                $("#btnDocumnetTemplateEdit").show();
                $("#btnUIelementSave").hide();
            }
        });
    }

    function checkBoxFormatter(cellValue, options, rowObject) {
        if (rowObject.isActive == false)
            return "<input type='checkbox' id='checkbox_" + options.rowId + "' name= 'checkbox_" + options.gid + "'/>";
        else
            return "<input type='checkbox' id='checkbox_" + options.rowId + "' name= 'checkbox_" + options.gid + "' checked />";
    }

    function unFormatIncludedColumn(cellValue, options) {
        var result;
        result = $(this).find('#' + options.rowId).find('input').prop('checked');
        return result;
    }

    function templateDesignSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(ExportToPDF.uiElementUpdatedSuccessMsg);
        }
         var allRowsInGrid = $(elementIDs.templateUIElementGridJQ).getRowData();;
                for (var i = 0; i < allRowsInGrid.length; i++) {
                    $(elementIDs.templateUIElementGridJQ + " tr#" + allRowsInGrid[i].UIElementID).attr("disabled", "disabled");
                }
    }
    function templateDeleteSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(ExportToPDF.templateDeleteSuccessMsg);
            loadDocumentTemplateGrid();
        }
    }
    return {
        loadDocumentTemplateGrid: function () {
            loadDocumentTemplateGrid();
        },
        loadTemplateUIElementGrid: function (formDesignVersionID) {
            //load document uiElement Template for currently selected row of Document template           
            loadTemplateUIElementGrid(formDesignVersionID);
        }
    }
}();
//contains functionality for the Document Template add dialog
var pdfConfigurationDialog = function () {

    var elementIDs = {
        documentDesignNameDDLJQ: '#documentDesignDDL',
        designVersionDDLJQ: '#designVersionDDL',
        templateNameJQ: '#templateName',
        descriptionJQ: '#description',

        documentDesignDDLHelpBlockJQ: '#documentDesignDDLHelpBlock',
        designVersionDDLHelpBlockJQ: '#designVersionDDLHelpBlock',
        templateNameHelpBlockJQ: '#templateNameHelpBlock',

        formDesignGridJQ: '#templateDesign',
        pdfConfigurationDialog: "#pdfConfigurationDialog"
    };
    var URLs = {
        //get Document Design List
        formDesignList: '/PrintTemplate/FormDesignList?tenantId=1',
        //get Document Design Version
        formDesignVersionList: '/PrintTemplate/FormDesignVersionList?tenantId=1&formDesignId={formDesignId}',
        //url for Add Design Template
        designTemplateAdd: '/PrintTemplate/Add?tenantId=1&formDesignId={formDesignId}&formDesignVersionId={formDesignVersionId}&templateName={templateName}&description={description}',
        //url for Update Design Template
        designTemplateUpdate: '/FormDesign/Update',
    }

    function init() {
        //register dialog for grid row add/edit
        $(elementIDs.pdfConfigurationDialog).dialog({
            autoOpen: false,
            height: 360,
            width: 280,
            modal: true
        });
        //fill Design Version list
        $(elementIDs.documentDesignNameDDLJQ).change(function (e) {
            if ($(this).val() == "0") {
                $(elementIDs.designVersionDDLJQ).empty();
                $(elementIDs.templateNameJQ).val("");
                $(elementIDs.descriptionJQ).val("");
                $(elementIDs.designVersionDDLJQ).attr('disabled', 'disabled');
                $(elementIDs.templateNameJQ).attr('disabled', 'disabled');
                $(elementIDs.descriptionJQ).attr('disabled', 'disabled');
            }
            else {
                $(elementIDs.designVersionDDLJQ).removeAttr('disabled');
                return fillDesignVersionDLL($(this).val());
            }
        });
        //Auto suggested Template name
        $(elementIDs.designVersionDDLJQ).change(function (e) {
            $(elementIDs.pdfConfigurationDialog + ' button').removeAttr('disabled');
            $(elementIDs.templateNameJQ).removeAttr('disabled');
            $(elementIDs.descriptionJQ).removeAttr('disabled');

            if ($(this).val() == "0") {
                $(elementIDs.templateNameJQ).val("");
                $(elementIDs.descriptionJQ).val("");
            }
            else {
                $(elementIDs.templateNameJQ).val("");
                $(elementIDs.descriptionJQ).val("");
                templeteName = $(elementIDs.documentDesignNameDDLJQ).find("option:selected").text().replace(/[^a-z0-9\s]/gi, ' ') + "_" + $(elementIDs.designVersionDDLJQ).find("option:selected").text() + "_PDFTemplate";
                $(elementIDs.templateNameJQ).val(templeteName);
            }
        });
        //register event for Add/Edit button click on dialog
        $(elementIDs.pdfConfigurationDialog + ' button').on('click', function () {
            var isValid = false;
            //check all input fields for validation.
            isValid = validateControls();
            if (isValid) {
                var formDesignId = $(elementIDs.documentDesignNameDDLJQ).val();
                var formDesignVersionId = $(elementIDs.designVersionDDLJQ).val();
                var templateName = $(elementIDs.templateNameJQ).val();
                var description = $(elementIDs.descriptionJQ).val();
                var documetTemplateList = $(elementIDs.formDesignGridJQ).jqGrid("getRowData");
                if (documetTemplateList.length > 0) {
                    var filterList = documetTemplateList.filter(function (ct) {
                        return (ct.TemplateName == templateName ? true : false);
                    });
                }
                //validate Template name
                if (filterList !== undefined && filterList.length > 0) {
                    $(elementIDs.templateNameJQ).parent().addClass('has-error');
                    $(elementIDs.templateNameHelpBlockJQ).text(ExportToPDF.templateAlreadyExistsMsg);
                }
                else {
                    var url;
                    url = URLs.designTemplateAdd.replace(/\{formDesignId\}/g, formDesignId)
                                                .replace(/\{formDesignVersionId\}/g, formDesignVersionId)
                                                .replace(/\{templateName\}/g, templateName)
                                                .replace(/\{description\}/g, description);


                    //ajax call to add/update
                    var promise = ajaxWrapper.postJSON(url);
                    //register ajax success callback
                    promise.done(formDesignSuccess);
                    //register ajax failure callback
                    promise.fail(showError);
                }
            }
        });
    }

    function intializeControl() {
        //intializeControl
        $(elementIDs.pdfConfigurationDialog + ' button').attr('disabled', 'disabled');
        $(elementIDs.designVersionDDLJQ).empty();
        $(elementIDs.designVersionDDLJQ).attr('disabled', 'disabled');
        $(elementIDs.templateNameJQ).attr('disabled', 'disabled');
        $(elementIDs.templateNameJQ).val("");
        $(elementIDs.templateNameJQ).parent().removeClass('has-error');
        $(elementIDs.templateNameHelpBlockJQ).removeClass('form-control');
        $(elementIDs.descriptionJQ).attr('disabled', 'disabled');
        $(elementIDs.descriptionJQ).val("");
        $(elementIDs.documentDesignNameDDLJQ).empty();
        $(elementIDs.documentDesignNameDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");
        var url = URLs.formDesignList;
        var promise = ajaxWrapper.getJSON(url);
        //fill the folder list drop down
        promise.done(function (list) {
            for (i = 0; i < list.length; i++) {
                $(elementIDs.documentDesignNameDDLJQ).append("<option value=" + list[i].FormDesignId + ">" + list[i].FormDesignName + "</option>");
            }

            $(elementIDs.pdfConfigurationDialog).dialog('option', 'title', ExportToPDF.pdfTempleteConfiguration);
            //open the dialog - uses jqueryui dialog
            $(elementIDs.pdfConfigurationDialog).dialog("open");
        });
        promise.fail(showError);
    }

    function fillDesignVersionDLL(formDesignId) {
        $(elementIDs.designVersionDDLJQ).empty();
        $(elementIDs.designVersionDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");
        var Url = URLs.formDesignVersionList.replace(/\{formDesignId\}/g, formDesignId);
        var promise = ajaxWrapper.getJSON(Url);
        //fill the folder list drop down
        promise.done(function (list) {
            for (i = 0; i < list.length; i++) {
                if (list[i].StatusText != "In Progress" || list.length == 1)
                    $(elementIDs.designVersionDDLJQ).append("<option value=" + list[i].FormDesignVersionId + ">" + list[i].Version + "</option>");
            }
        });
        promise.fail(showError);
    }

    function validateControls() {
        var isDesignNameSelected = false;
        var isDesignVersionSelected = false;
        var templateName = false;
        if ($(elementIDs.documentDesignNameDDLJQ).val() == "0") {
            $(elementIDs.documentDesignNameDDLJQ).parent().addClass('has-error');
            $(elementIDs.documentDesignDDLHelpBlockJQ).text(ExportToPDF.documentDesignNameRequiredMsg);
            isDesignNameSelected = false;
        }
        else {
            $(elementIDs.documentDesignNameDDLJQ).parent().removeClass('has-error');
            $(elementIDs.documentDesignDDLHelpBlockJQ).removeClass('form-control');
            $(elementIDs.documentDesignDDLHelpBlockJQ).text('');
            isDesignNameSelected = true;
        }
        if ($(elementIDs.designVersionDDLJQ).val() == "0") {
            $(elementIDs.designVersionDDLJQ).parent().addClass('has-error');
            $(elementIDs.designVersionDDLHelpBlockJQ).text(ExportToPDF.documentDesignVersionRequiredMsg);
            isDesignVersionSelected = false;
        }
        else {
            $(elementIDs.designVersionDDLJQ).parent().removeClass('has-error');
            $(elementIDs.designVersionDDLHelpBlockJQ).removeClass('form-control');
            $(elementIDs.designVersionDDLHelpBlockJQ).text('');
            isDesignVersionSelected = true;
        }
        if ($(elementIDs.templateNameJQ).val().length == 0) {
            $(elementIDs.templateNameJQ).parent().addClass('has-error');
            $(elementIDs.templateNameHelpBlockJQ).text(ExportToPDF.templateNameRequiredMsg);
            templateName = false;
        }
        else {
            $(elementIDs.templateNameJQ).parent().removeClass('has-error');
            $(elementIDs.templateNameHelpBlockJQ).removeClass('form-control');
            $(elementIDs.templateNameHelpBlockJQ).text('');
            templateName = true;
        }
        return (isDesignNameSelected && isDesignVersionSelected && templateName);
    }
    //ajax success callback - for add/edit
    function formDesignSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(ExportToPDF.templateAddSuccessMsg);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
        //reload Document Template grid
        pdfConfigurationDesign.loadDocumentTemplateGrid();
        $(elementIDs.pdfConfigurationDialog).dialog('close');
    }
    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
    //init dialog on load of page
    //initialize the dialog after this js are loaded
    init();
    //these are the properties that can be called by using pdfConfigurationDialog.<Property>
    //eg. pdfConfigurationDialog.show('name','add');
    return {
        show: function () {
            intializeControl();
            

        }
    }
}(); 
//contains functionality for the PDF Gebration Template dialog
var pdfGenerationDailog = function (instance) {
    this.curentInstance = instance;
    instance.exportTopdfData = null;
    var documentTemplateList = null;
    var elementIDs = {
        pdfGenerationDialogJQ: '#pdfGenerationDialog',
        templateNameDDLJQ: '#templateNameDDL',
        generatePDFJQ: '#genratePDFData',
        pdfpreviewJQ: '#pdfPreview',
        pdfGenerationJQ: '#pdfGeneration',
        pdfPreviewMessage: '#pdfPreviewMessage',
        description: '#descriptionTemplate'
    };
    var URLs = {
        //get Document Template List
        getDocumentTemplateList: '/PrintTemplate/DocumentTemplateList?tenantId=1',
    }
    this.pdfTemplateDialog = pdfMethod();

    function pdfMethod() {
        var curentInstance = this;

        return {
            showPDfDialog: function (exportToPDF) {
                var data = this;
                initializeDialog();
                instance.exportTopdfData = exportToPDF;
                $(elementIDs.pdfGenerationDialogJQ).dialog('option', 'title', ExportToPDF.pdfgenerationtitleMsg);
                $(elementIDs.pdfGenerationDialogJQ).dialog("open");
                fillDocumentTemplate(instance.exportTopdfData.formDesignId)
            }
        }
    }

    function initializeDialog() {
        $(elementIDs.pdfGenerationDialogJQ).dialog({
            autoOpen: false,
            resizable: false,
            closeOnEscape: true,
            height: 'auto',
            width: 400,
            height: 220,
            modal: true,
            position: ['middle', 100],
        });
    }

    function fillDocumentTemplate(formDesignId) {
        $(elementIDs.templateNameDDLJQ).empty();
        var url = URLs.getDocumentTemplateList;
        var promise = ajaxWrapper.getJSON(url);
        //fill the folder list drop down
        promise.done(function (list) {
            documentTemplateList = list;
            var documentTemplateData = list.filter(function (rowData) {
                return rowData.FormDesignID == formDesignId
            });
            $(elementIDs.description).addClass("Disable");
            if (documentTemplateData.length > 0) {               
                for (i = 0; i < documentTemplateData.length; i++) {
                    $(elementIDs.templateNameDDLJQ).append("<option value=" + documentTemplateData[i].TemplateID + ">" + documentTemplateData[i].TemplateName + "</option>");

                    if (instance.exportTopdfData.formDesignVersionId == documentTemplateData[i].FormDesignVersionID || documentTemplateData.length == 1)
                        $(elementIDs.templateNameDDLJQ).prop("selectedIndex", i);
                    $(elementIDs.description).val(documentTemplateData[i].Description);
                }
                $(elementIDs.pdfPreviewMessage).hide();
                $(elementIDs.pdfGenerationJQ).show();
                $(elementIDs.pdfGenerationDialogJQ).find('.pdfPreview').parent().show();
                $(elementIDs.generatePDFJQ).parent().show();
            }
            else {
                $(elementIDs.pdfPreviewMessage).removeClass('hide');
                $(elementIDs.pdfGenerationJQ).hide();
                $(elementIDs.pdfGenerationDialogJQ).find('.pdfPreview').parent().hide();
                $(elementIDs.generatePDFJQ).parent().hide();
            }
        });
        promise.fail(pdfConfigurationDialog.showError);
    }
    //for to PDf Generate
    $(elementIDs.generatePDFJQ).on("click", function () {

        var formInstanceId = instance.exportTopdfData.formInstanceId;
        var formDesignVersionId = instance.exportTopdfData.formDesignVersionId;
        var folderVersionId = instance.exportTopdfData.folderVersionId;
        var formName = instance.exportTopdfData.formName;
        var tenantId = instance.exportTopdfData.tenantId;
        var accountId = instance.exportTopdfData.accountId;
        var folderName = instance.exportTopdfData.folderName;
        var folderVersionNumber = instance.exportTopdfData.folderVersionNumber;
        var effectiveDate = instance.exportTopdfData.effectiveDate;
        var templateId = $(elementIDs.templateNameDDLJQ).val();
        var forminstancelisturl = '/FolderVersion/PrintPDF';
        var stringData = "formInstanceId=" + formInstanceId;
        stringData += "<&formDesignVersionId=" + formDesignVersionId;
        stringData += "<&folderVersionId=" + folderVersionId;
        stringData += "<&formName=" + formName;
        stringData += "<&tenantId=" + tenantId;
        stringData += "<&accountId=" + accountId;
        stringData += "<&folderName=" + folderName;
        stringData += "<&folderVersionNumber=" + folderVersionNumber;
        stringData += "<&effectiveDate=" + effectiveDate;
        stringData += "<&templateId=" + templateId;
        $.downloadNew(forminstancelisturl, stringData, 'post');
        $(elementIDs.pdfGenerationDialogJQ).dialog("close"); 
    });
    // To show PDF Preview
    $(elementIDs.pdfpreviewJQ).on("click", function () {
        //show perview
        pdfPreviewDailog.show(instance.exportTopdfData.formDesignVersionId, $(elementIDs.templateNameDDLJQ).val());
    });

    $(elementIDs.templateNameDDLJQ).change(function (e) {
        var documentTemplateData = documentTemplateList.filter(function (rowData) {
            return rowData.TemplateID == $(elementIDs.templateNameDDLJQ).val();
        });
        if (documentTemplateData.length > 0)
            $(elementIDs.description).val(documentTemplateData[0].Description);
    });       
};

//contains functionality for the PDF Preview dialog
var pdfPreviewDailog = function () {

    var elementIDs = {
        pdfPreviewDailogJQ: '#pdfPreviewDailog',
        previewPDFGrid: 'previewPDF',
        previewGridJQ: '#previewPDF'
    }
    var pdfPreviewDailogURLs = {
        getTemplateDesginUIElementList: '/PrintTemplate/LoadTemplateDesignUIElementList?tenantId=1&formDesignVersionId={formDesignVersionId}&templateId={templateId}',
    }
    function initializePreviewDialog() {
        $(elementIDs.pdfPreviewDailogJQ).dialog({
            autoOpen: false,
            resizable: false,
            closeOnEscape: true,
            height: 'auto',
            width: 400,
            height: 450,
            modal: true,
            position: ['middle', 100],
        });
    }
    function loadPDFPreviewGrid(formDesignVersionID, templateId) {
        //set column list for grid
        var colArray = ['', '', '', 'Section Name', 'Include in PDF'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'TemplateUIMapID', index: 'TemplateUIMapID', hidden: true, search: false });
        colModel.push({ name: 'TemplateID', index: 'TemplateID', hidden: true, search: false });
        colModel.push({ name: 'UIElementID', index: 'UIElementID', key: true, hidden: true, align: 'center', search: false });
        colModel.push({ name: 'Label', index: 'Label', align: 'left', editable: false, sortable: false });
        colModel.push({ name: 'IncludeInPDF', index: 'IncludeInPDF', align: 'Center', editable: false, formatter: chkValueImageFmatter, unformatter: chkValueImageUnFormat, sortable: false });
        //clean up the grid first - only table element remains after this
        $(elementIDs.previewGridJQ).jqGrid('GridUnload');

        var url = pdfPreviewDailogURLs.getTemplateDesginUIElementList.replace(/\{formDesignVersionId\}/g, formDesignVersionID)
                                                       .replace(/\{templateId\}/g, templateId);
        //adding the pager element
        $(elementIDs.previewGridJQ).parent().append("<div id='p" + elementIDs.previewPDFGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.previewGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'UIElement List',
            height: '310',
            rowNum: 25,
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.previewPDFGrid,
            sortname: 'TemplateID',
            altclass: 'alternate',
            resizeStop: function (width, index) {
                autoResizing(elementIDs.previewGridJQ);
            }
        });
        var pagerElement = '#p' + elementIDs.previewPDFGrid;
        //remove default buttons
        $(elementIDs.previewGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        $(pagerElement).find(pagerElement + '_left').remove();
    }

    function chkValueImageFmatter(cellvalue, options, rowObject) {
        if (rowObject.isActive == true) {
            return '<span align="center" class="ui-icon ui-icon-check" style="margin:auto" ></span>';
        }
        else {
            return '<span align="center" class="ui-icon ui-icon-close" style="margin:auto" ></span>';
        }
    }

    function chkValueImageUnFormat(cellvalue, options, cell) {
        var checked = $(cell).children('span').attr('class');
        if (checked == "ui-icon ui-icon-check")
            return true;
        else
            return false;
    }

    return {
        show: function (formDesignVersionID, templateId) {
            initializePreviewDialog();
            $(elementIDs.pdfPreviewDailogJQ).dialog('option', 'title', ExportToPDF.pdfPrviewtitleMsg);
            //open the dialog - uses jqueryui dialog
            $(elementIDs.pdfPreviewDailogJQ).dialog("open");
            loadPDFPreviewGrid(formDesignVersionID, templateId);
        }
    }
}();


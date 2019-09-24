//implementation of section list dialog
//resequence/add sections and edit section names
//contructor with params:
//uiElement - uiElement Object from property grid
//formDesignVersionId - form design version of the element
function sectionListDialog(uiElement, formVersionStatus, formDesignId, formDesignVersionId, formDesignVersionInstance) {
    this.uiElement = uiElement;
    this.formDesignId = formDesignId;
    this.formDesignVersionId = formDesignVersionId;
    this.tenantId = 1;
    this.status = formVersionStatus;
    this.formDesignVersionInstance = formDesignVersionInstance;
}

//class property
sectionListDialog.URLs = {
    //url to get list of Sections
    sectionList: '/UIElement/SectionList?tenantId=1&formDesignVersionId={formDesignVersionId}&uiElementId={uiElementId}',
    //url to update section sequences within the form
    updateSections: '/UIElement/UpdateSectionSequences'
}

sectionListDialog.elementIDs = {
    //container html id for this dialog -  see Views\FormDesign\Index.cshtml
    sectionListDialog: '#sectionlistdialog',
    //grid container html id
    sectionListDialogGridJQ: '#sectionlistdialoggrid',
    sectionListDialogGrid: 'sectionlistdialoggrid'
}
//property to check single initialization - since this is modal
sectionListDialog._isInitialized = false;

//init dialog
sectionListDialog.init = function () {
    //usesjquery ui dialog
    if (sectionListDialog._isInitialized == false) {
        $(sectionListDialog.elementIDs.sectionListDialog).dialog({
            autoOpen: false,
            height: '550',
            width: '60%',
            modal: true
        });
        sectionListDialog._isInitialized = true;
    }
}

//show dialog
sectionListDialog.prototype.show = function () {
    //set header
    $(sectionListDialog.elementIDs.sectionListDialog).dialog('option', 'title', DocumentDesign.manageSection + this.uiElement.Label);
    //open dialog
    $(sectionListDialog.elementIDs.sectionListDialog).dialog('open');
    //load grid
    this.loadGrid();
    $(sectionListDialog.elementIDs.sectionListDialog).dialog({
        position: {
            my: 'center',
            at: 'center'
        },
    });
}

sectionListDialog.prototype.loadGrid = function () {
    //set column list
    var colArray = ['UIElementId', 'FormDesignVersionId', 'Section Name', 'Old Sequence', 'New Sequence'];

    //set column models
    var colModel = [];
    colModel.push({ name: 'UIElementID', index: 'TenantId', key: true, hidden: true, search: false, });
    colModel.push({ name: 'FormDesignVersionId', index: 'FormDesignVersionId', hidden: true });
    colModel.push({ name: 'Label', index: 'Label', align: 'left', editable: false, sortable: false });
    colModel.push({ name: 'Sequence', index: 'Sequence', width: '50', align: 'right', hidden: true, editable: false, sortable: false });
    colModel.push({ name: 'NewSequence', index: 'NewSequence', width: '50', align: 'right', hidden: true, editable: true, sortable: false, formatter: this.formatNewSequence });

    //get URL for grid
    var sectionListUrl = sectionListDialog.URLs.sectionList.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId).replace(/\{uiElementId\}/g, this.uiElement.UIElementID);
    //unload previous grid values
    $(sectionListDialog.elementIDs.sectionListDialogGridJQ).jqGrid('GridUnload');
    //add grid footer
    $(sectionListDialog.elementIDs.sectionListDialogGridJQ).parent().append("<div id='p" + sectionListDialog.elementIDs.sectionListDialogGrid + "'></div>");

    //load grid
    $(sectionListDialog.elementIDs.sectionListDialogGridJQ).jqGrid({
        url: sectionListUrl,
        datatype: 'json',
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Section List',
        pager: '#p' + sectionListDialog.elementIDs.sectionListDialogGrid,
        height: '350',
        autowidth: true,
        ExpandColumn: 'Label',
        loadonce: true,
        rowNum: 10000,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        altclass: 'alternate'
    });
    //enable drag and drop of grid rows for resequencing
    $(sectionListDialog.elementIDs.sectionListDialogGridJQ).jqGrid('sortableRows');
    //register event handler for drag and drop stop event
    $(sectionListDialog.elementIDs.sectionListDialogGridJQ).on("sortstop", function (event, ui) {
        //reset NewSequence column
        var rows = $(sectionListDialog.elementIDs.sectionListDialogGridJQ).getDataIDs();
        for (index = 0; index < rows.length; index++) {
            row = $(sectionListDialog.elementIDs.sectionListDialogGridJQ).getRowData(rows[index]);
            row.NewSequence = index + 1;
            $(sectionListDialog.elementIDs.sectionListDialogGridJQ).setCell(rows[index], 4, index + 1);
        }
    });

    //remove paging from footer
    var pagerElement = '#p' + sectionListDialog.elementIDs.sectionListDialogGrid;
    //remove default buttons
    $(sectionListDialog.elementIDs.sectionListDialogGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    //remove paging
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    //Display Navbar only if formversion is not finalized
    //if (this.status != 'Finalized') {
    //register event handler for add button in footer
    $(sectionListDialog.elementIDs.sectionListDialogGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-plus',
        onClickButton: function () {
            //show add dialog
            addSectionDialog.show('', 'add', currentInstance.tenantId, currentInstance.formDesignId, currentInstance.formDesignVersionId, currentInstance.uiElement.UIElementID, currentInstance);
        }
    });
    //}
    var currentInstance = this;

    //Display Navbar only if formversion is not finalized
    //if (this.status != 'Finalized') {
    //register event handler for Save button in footer
    $(sectionListDialog.elementIDs.sectionListDialogGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: 'Save',
        onClickButton: function () {
            //get updates sequences
            var updatedSequences = [];
            var rows = $(this).getDataIDs();
            for (index = 0; index < rows.length; index++) {
                row = $(this).getRowData(rows[index]);
                updatedSequences.push({
                    TenantID: currentInstance.tenantId,
                    FormDesignVersionID: currentInstance.formDesignVersionId,
                    UIElementID: row.UIElementID,
                    Sequence: row.NewSequence
                });
            }
            //generate object to be POSTed
            var data = {
                TenantID: currentInstance.tenantId,
                FormDesignVersionID: currentInstance.formDesignVersionId,
                Models: updatedSequences
            };
            //ajax post
            var promise = ajaxWrapper.postJSONCustom(sectionListDialog.URLs.updateSections, { model: data });
            promise.done(function (xhr) {
                //reload grid
                currentInstance.loadGrid();
                currentInstance.formDesignVersionInstance.buildGrid(currentInstance.uiElement.UIElementID);
            });
        }
    });
    //}
}
//method used to format the NewSequence column in colmodel - see loadGrid method
//used in the formatter attribute
sectionListDialog.prototype.formatNewSequence = function (cellValue, options, rowObject) {
    if (cellValue === undefined) {
        return rowObject.Sequence;
    }
    else {
        return cellValue;
    }
}

//initialize when this function is invoked(happens only once after soon after it is loaded
sectionListDialog.init();

//implementation of add section dialog
var addSectionDialog = function () {
    var URLs = {
        //add section url
        sectionAdd: '/UIElement/AddSection',
        //update section url
        sectionUpdate: '/UIElement/UpdateSectionName'
    }

    var elementIDs = {
        //container id for dialog - see Views\FormDesign\Index.cshtml
        sectionListDialog: '#sectionlistdialogadd',
        //container id for grid
        sectionListDialogGridJQ: '#sectionlistdialoggrid',
        sectionListDialogGrid: 'sectionlistdialoggrid'
    };

    //dialog state - add or edit
    var sectionDialogState;
    var formDesignVersionId;
    var tenantId;
    var parentUIElementId;
    //parent object that loaded this dialog
    var parentObject;

    //ajax call success callback
    function sectionSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(DocumentDesign.addSectionMsg);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
        //reload grid
        parentObject.loadGrid();
        //reset dialog elements
        $(elementIDs.sectionListDialog + ' #sectionname').parent().removeClass('has-error');
        $(elementIDs.sectionListDialog + ' .help-block').text(DocumentDesign.sectionAddNewNameValidateMsg);
        $(elementIDs.sectionListDialog).dialog('close');
    }
    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else

            messageDialog.show(JSON.stringify(xhr));
    }
    //init soon after it is loaded and invoked immediately - see last line
    function init() {
        //register dialog for grid row add/edit
        $(elementIDs.sectionListDialog).dialog({
            autoOpen: false,
            height: 300,
            width: 450,
            modal: true
        });
        //register event handler when the add /edit button is clicked
        $(elementIDs.sectionListDialog + ' button').on('click', function () {
            //check if name is already used
            var newName = $(elementIDs.sectionListDialog + ' textarea').val();
            var re = new RegExp(CustomRegexValidation.STARTWITHAPLHABETS);

            var sectionList = $(elementIDs.sectionListDialogGridJQ).getRowData();

            var filterList = sectionList.filter(function (ct) {
                return compareStrings(ct.Label, newName, true);
            });

            //validate form name
            if (filterList !== undefined && filterList.length > 0) {
                $(elementIDs.sectionListDialog + ' #sectionname').parent().addClass('has-error');
                $(elementIDs.sectionListDialog + ' .help-block').text(DocumentDesign.sectionNameAlreadyExistsMsg);
            }
            else if (newName == '') {
                $(elementIDs.sectionListDialog + ' #sectionname').parent().addClass('has-error');
                $(elementIDs.sectionListDialog + ' .help-block').text(DocumentDesign.sectionNameRequired);
            }
            else if (!(newName.match(re))) {
                $(elementIDs.sectionListDialog + ' #sectionname').parent().addClass('has-error');
                $(elementIDs.sectionListDialog + ' .help-block').text(DocumentDesign.nameValidateMsg);
            }
            else {

                //check for duplicate names within FormDesign version
                if (parentObject.uiElement.Label.replace(/\s/g, "") === newName.replace(/\s/g, "")) {
                    $(elementIDs.sectionListDialog + ' #sectionname').parent().addClass('has-error');
                    $(elementIDs.sectionListDialog + ' .help-block').text(Common.fieldNameExistsMsg);
                } else {
                    //save the new form design
                    var rowId = $(elementIDs.sectionListDialogGridJQ).getGridParam('selrow');

                    var sectionAdd = {
                        TenantID: tenantId,
                        FormDesignID: formDesignId,
                        FormDesignVersionID: formDesignVersionId,
                        ParentUIElementID: parentUIElementId,
                        UIElementID: rowId,
                        Label: newName
                    };
                    var url;
                    if (sectionDialogState === 'add') {
                        url = URLs.sectionAdd;
                    }
                    else {
                        url = URLs.sectionUpdate;
                    }
                    //ajax call - POST
                    var promise = ajaxWrapper.postJSONCustom(url, sectionAdd);
                    //success callback
                    promise.done(sectionSuccess);
                    //failure callback
                    promise.fail(showError);
                }
                //});
            }
        });
    }
    init(); //init when this function is loaded and invoked

    return {
        //method is called to display the add/edit dialog - using 'add' or 'edit' in action paramater
        show: function (sectionName, action, tenantID, formDesignID, formDesignVersionID, parentUIElementID, parent) {
            tenantId = tenantID;
            formDesignVersionId = formDesignVersionID;
            formDesignId = formDesignID;
            parentUIElementId = parentUIElementID;
            sectionDialogState = action;
            parentObject = parent;
            //set Section Name in textbox
            $(elementIDs.sectionListDialog + ' textarea').each(function () {
                $(this).val(sectionName);
            });
            //set controls
            $(elementIDs.sectionListDialog + ' #sectionname').parent().removeClass('has-error');
            if (sectionDialogState == 'add') {
                $(elementIDs.sectionListDialog).dialog('option', 'title', DocumentDesign.addSection);
                $(elementIDs.sectionListDialog + ' .help-block').text(DocumentDesign.sectionAddNewNameValidateMsg);
                $(elementIDs.sectionListDialog + ' button').text('Add');
            }
            else {
                $(elementIDs.sectionListDialog).dialog('option', 'title', 'Edit Section');
                $(elementIDs.sectionListDialog + ' .help-block').text(DocumentDesign.sectionEditNameValidateMsg);
                $(elementIDs.sectionListDialog + ' button').text('Edit');
            }
            //open dialog
            $(elementIDs.sectionListDialog).dialog("open");
        }
    }
}(); // function invoked soon after it is loaded
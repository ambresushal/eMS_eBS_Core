//contains functionality for the dropdown items dialog
//constructor with params:  
//params are:
//uiElement - uiElement object
//formDesignVersionID - form design version id
function dropdownItemsDialog(uiElement, status, formDesignVersionId) {
    this.uiElement = uiElement;
    this.formDesignVersionId = formDesignVersionId;
    this.tenantId = 1;
    this.statustext = status;
}

//set class property
dropdownItemsDialog.elementIDs = {
    //ui element that contains the dialog - see FormDesign\Views\Index.cshtml
    dropdownItemsDialog: '#dropdownitemsdialog',
    //ui element that contains the dropdown items grid
    dropdownItemsDialogGridJQ: '#dropdownitemsdialoggrid',
    dropdownItemsDialogGrid: 'dropdownitemsdialoggrid'
}
//set class property - since this is a modal dialog, only one instance will be rendered at a time
// so intialize only one
dropdownItemsDialog._isInitialized = false;

//init dialog
dropdownItemsDialog.init = function () {
    var currentInstance = this;
    //initialized only once - uses jquery ui for dialog
    //since only one instance will be shown at a time - as it is modal
    if (dropdownItemsDialog._isInitialized == false) {
        $(dropdownItemsDialog.elementIDs.dropdownItemsDialog).dialog({
            autoOpen: false,
            height: '500',
            width: '60%',
            modal: true
        });
        dropdownItemsDialog._isInitialized = true;
    }
}

//show dialog
dropdownItemsDialog.prototype.show = function (isLoaded) {
    if (isLoaded == false) {
        //set header
        $(dropdownItemsDialog.elementIDs.dropdownItemsDialog).dialog('option', 'title', DocumentDesign.dropDownManageItemsMsg + this.uiElement.Label);
        //open dialog
        $(dropdownItemsDialog.elementIDs.dropdownItemsDialog).dialog('open');
        //load grid
        this.loadGrid();
    }
    else {
        //open existing dialog
        $(dropdownItemsDialog.elementIDs.dropdownItemsDialog).dialog('open');
    }
}

//load the grid of drop down items
dropdownItemsDialog.prototype.loadGrid = function () {
    var currentInstance = this;
    //set column list
    var colArray = ['Value', 'Display Text', 'Old Sequence', 'New Sequence', 'ItemID'];

    //set column models
    var colModel = [];
    colModel.push({ name: 'Value', index: 'Value', editable: false, sortable: true });   //Value
    colModel.push({ name: 'DisplayText', index: 'DisplayText', key: true, align: 'left', editable: false, sortable: true }); //DisplayText
    colModel.push({ name: 'Sequence', index: 'Sequence', width: '50', align: 'right', editable: false, sortable: false });
    colModel.push({ name: 'NewSequence', index: 'NewSequence', width: '50', align: 'right', editable: true, sortable: false, formatter: this.formatNewSequence });
    colModel.push({ name: 'ItemID', index: 'ItemID', hidden: true });


    //unload previous grid values
    $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).jqGrid('GridUnload');

    //add grid footer
    $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).parent().append("<div id='p" + dropdownItemsDialog.elementIDs.dropdownItemsDialogGrid + "'></div>");

    //load grid
    $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).jqGrid({
        datatype: 'local', //data is loaded from local array
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Item List',
        pager: '#p' + dropdownItemsDialog.elementIDs.dropdownItemsDialogGrid,
        height: '300',
        autowidth: true,
        ExpandColumn: 'Label',
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        altclass: 'alternate',
        loadComplete: function () {
            var rows = $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).getDataIDs();
            for (index = 0; index < rows.length; index++) {
                row = $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).getRowData(rows[index]);
                row.NewSequence = index + 1;
                $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).setCell(rows[index], 3, index + 1);
            }
            if (ClientRolesForFormDesignAccess.indexOf(vbRole) >= 0 && currentInstance.uiElement.IsStandard == true) {
                $("#p" + dropdownItemsDialog.elementIDs.dropdownItemsDialogGrid).hide();//.addClass('divdisabled');
            } else {
                $("#p" + dropdownItemsDialog.elementIDs.dropdownItemsDialogGrid).show();
            }
        }
    });

    if (ClientRolesForFormDesignAccess.indexOf(vbRole) < 0 && currentInstance.uiElement.IsStandard == false) {
        //enable drag and drop of rows - for sequencing
        $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).jqGrid('sortableRows');
    }
    //register event handler when a drag and drop action is completed
    $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).on("sortstop", function (event, ui) {
        //iterate through the rows and resequence
        var rows = $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).getDataIDs();
        for (index = 0; index < rows.length; index++) {
            row = $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).getRowData(rows[index]);
            row.NewSequence = index + 1;
            $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).setCell(rows[index], 3, index + 1);
        }
    });

    //remove pager from grid footer
    var pagerElement = '#p' + dropdownItemsDialog.elementIDs.dropdownItemsDialogGrid;
    //remove default buttons
    $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).jqGrid('navGrid', pagerElement, { refresh: false, edit: false, add: false, del: false, search: false });
    //remove paging
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    var currentInstance = this;

    //if (this.statustext != 'Finalized') {
    //register click event handler for Add button in grid footer
    $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-plus', title: 'Add',
        onClickButton: function () {
            //open dialog in 'add' mode
            addDropdownDialog.show(currentInstance, '', '', 'add', currentInstance.tenantId, currentInstance.formDesignVersionId, currentInstance.uiElement.UIElementID);
        }
    });

    //register click event handler for Edit button in grid footer
    $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-pencil', title: 'Edit',
        onClickButton: function () {
            var recordCount = $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).getGridParam("reccount");
            if (recordCount > 0) {
                var rowId = $(this).getGridParam('selrow');
                var rowData = $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).getRowData(rowId);
                if (rowId !== undefined && rowId !== null) {
                    //open dialog in 'edit' mode
                    addDropdownDialog.show(currentInstance, rowId, rowData.DisplayText, 'edit', currentInstance.tenantId, currentInstance.formDesignVersionId, currentInstance.uiElement.UIElementID);
                }
                else {
                    messageDialog.show(Common.pleaseSelectRowMsg);
                }
            }
            else
                messageDialog.show(Common.noRecordsToDisplay);
        }
    });

    //register click event handler for Delete button in grid footer
    $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-trash', title: 'Delete',
        onClickButton: function () {
            var recordCount = $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).getGridParam("reccount");
            if (recordCount > 0) {
                var rowId = $(this).getGridParam('selrow');
                var rowscount = $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).getDataIDs();
                var isLastRow = (rowId == rowscount.length) ? true : false;


                var rowData = $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).getRowData(rowId);
                if (rowId !== undefined && rowId !== null) {

                    //load confirm dialogue to asset the operation
                    confirmDialog.show((DocumentDesign.deleteConfirmationForDropDownItem).replace(/\{#itemName}/g, rowData.DisplayText), function () {
                        var newName = rowData.DisplayText;
                        var dropdownItems = $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).getRowData();
                        var filterList = dropdownItems.filter(function (ct) {
                            return compareStrings(ct.Value, newName, true);
                        });
                        dropdownItemsDialog.prototype.deleteRow(rowId);
                        //applying try catch if any problem occure then confirm dialog will be hidden
                        var error;
                        try {
                            //iterate through the rows and resequence
                            var rows = $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).getDataIDs();
                            for (index = 0; index < rows.length; index++) {
                                row = $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).getRowData(rows[index]);
                                row.NewSequence = index + 1;
                                if (index != rows.length - 1 || isLastRow) {
                                    $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).setCell(rows[index], 3, index + 1);
                                }
                            }
                        }
                        catch (err) {
                            error = err;
                        }
                        //Hide the confirmation dialog
                        confirmDialog.hide();
                    });
                }
                else {
                    messageDialog.show(Common.pleaseSelectRowMsg);
                }
            }
            else
                messageDialog.show(Common.noRecordsToDisplay);
        }
    });
    //}
    //add dropdown items to the grid
    for (var index = 0; index < this.uiElement.Items.length; index++) {
        var item = this.uiElement.Items[index];
        $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).jqGrid('addRowData',
            item.DisplayText, { ItemID: item.ItemID, Value: item.Value, Sequence: index + 1, NewSequence: index + 1, DisplayText: item.DisplayText });
    }
}

//method used to format the NewSequence column in colmodel - see loadGrid method
//used in the formatter attribute
dropdownItemsDialog.prototype.formatNewSequence = function (cellValue, options, rowObject) {
    if (cellValue === undefined) {
        return rowObject.Sequence;
    }
    else {
        return cellValue;
    }
}

//add row to the dropdown items grid
dropdownItemsDialog.prototype.addRow = function (itemName) {
    var index = $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).jqGrid('getGridParam', 'records');
    $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).jqGrid('addRowData',
        (index + 1), { ItemID: 0, Value: itemName, Sequence: index + 1, NewSequence: index + 1, DisplayText: itemName });
}

//change name of dropdown item in the grid
dropdownItemsDialog.prototype.editName = function (itemName, newName) {
    // $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).setCell(itemName, 4, newName);
    $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).setCell(itemName, 1, newName);
}

dropdownItemsDialog.prototype.deleteRow = function (rowId) {
    var index = $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).jqGrid('getGridParam', 'records');
    $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).jqGrid('delRowData', rowId);
}

//return data in the dropdown item grid to the caller
dropdownItemsDialog.prototype.getDialogData = function () {
    var rows = $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).getDataIDs();
    var items = [];
    for (index = 0; index < rows.length; index++) {
        var row = $(dropdownItemsDialog.elementIDs.dropdownItemsDialogGridJQ).getRowData(rows[index]);
        items.push({ ItemID: row.ItemID, Value: row.Value, DisplayText: row.DisplayText, Sequence: row.NewSequence });
    }
    return items;
}

//initialize the dropdown dialog
dropdownItemsDialog.init();

//contains the implementation of the Add/Edit dropdown item dialog
var addDropdownDialog = function () {

    var elementIDs = {
        //add/edit dialog element - see Views\FormDesign\Index.cshtml
        dropdownItemsDialog: '#dropdownitemsdialogadd',
        dropdownItemsDialogGridJQ: '#dropdownitemsdialoggrid',
        dropdownItemsDialogGrid: 'dropdownitemsdialoggrid'
    };

    //dropdown dialog that displayed this dialog
    var dropdownDialog;
    //mode it is loaded in - add or edit
    var dropdownDialogState;
    var formDesignVersionId;
    var currentName;
    var tenantId;
    var rowId;
    //element the dropdown items belong to
    var parentUIElementId;

    function init() {
        //register dialog for grid row add/edit
        //one time init sice it is modal and only one will be loaded at a time
        $(elementIDs.dropdownItemsDialog).dialog({
            autoOpen: false,
            height: 300,
            width: 450,
            modal: true
        });
        //register event for Add/Edit button click
        $(elementIDs.dropdownItemsDialog + ' button').on('click', function () {
            //check if name is already used
            var newName = $(elementIDs.dropdownItemsDialog + ' textarea').val();
            var dropdownItems = $(elementIDs.dropdownItemsDialogGridJQ).getRowData();

            var filterList = dropdownItems.filter(function (ct) {
                return compareStrings(ct.DisplayText, newName, true);
            });

            //validate form name
            if (filterList !== undefined && filterList.length > 0) {
                $(elementIDs.dropdownItemsDialog + ' div').addClass('has-error');
                $(elementIDs.dropdownItemsDialog + ' .help-block').text(DocumentDesign.dropDownItemNameExistsMsg);
            }
            else if (newName == '') {
                $(elementIDs.dropdownItemsDialog + ' div').addClass('has-error');
                $(elementIDs.dropdownItemsDialog + ' .help-block').text(DocumentDesign.dropDownItemNameRequiredMsg);
            }
            else {
                //add or edit dropdown item based on mode - add or edit
                if (dropdownDialogState === 'add') {
                    dropdownDialog.addRow(newName);
                }
                else {
                    dropdownDialog.editName(currentName, newName);
                }
                $(elementIDs.dropdownItemsDialog).dialog("close");
            }
        });

    }
    init(); //call the init function

    return {
        //show and load the dialog in add or edit mode(based on action parameter('add' or 'edit'))
        show: function (dialog, rowId, itemName, action, tenantID, formDesignVersionID, parentUIElementID) {
            tenantId = tenantID;
            currentName = rowId;
            formDesignVersionId = formDesignVersionID;
            parentUIElementId = parentUIElementID;
            dropdownDialogState = action;
            dropdownDialog = dialog;
            //get dropdown item name
            $(elementIDs.dropdownItemsDialog + ' textarea').each(function () {
                $(this).val(itemName);
            });
            //set element based on mode - add or edit
            $(elementIDs.dropdownItemsDialog + ' div').removeClass('has-error');
            if (dropdownDialogState == 'add') {
                $(elementIDs.dropdownItemsDialog).dialog('option', 'title', 'Add Item');
                $(elementIDs.dropdownItemsDialog + ' .help-block').text(DocumentDesign.dropDownAddNewItemValidateMsg);
                $(elementIDs.dropdownItemsDialog + ' button').text('Add');
            }
            else {
                $(elementIDs.dropdownItemsDialog).dialog('option', 'title', 'Edit Item');
                $(elementIDs.dropdownItemsDialog + ' .help-block').text(DocumentDesign.dropDownEditItemValidateMsg);
                $(elementIDs.dropdownItemsDialog + ' button').text('Edit');
            }
            //open the dialog
            $(elementIDs.dropdownItemsDialog).dialog("open");
        }
    }
}(); // function invoked soon after it is loaded - invokation required to initialize dialog
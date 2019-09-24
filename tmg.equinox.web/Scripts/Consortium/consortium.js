var consortium = function () {

    var URLs = {
        //Gets consortium list
        ConsortiumList: '/Consortium/GetConsortiumList?tenantID=1',
    }
    var elementIDs = {
        consortiumGrid: 'consortiumGrid',
        consortiumGridJQ: '#consortiumGrid',
        addConsortium: 'addConsortium',
        editConsortium: 'editConsortium',
        addConsortiumJQ: '#addConsortium',
        editConsortiumJQ: '#editConsortium',
    }

    function init() {
        $(document).ready(function () {
            loadConsortiumGrid();
        });
    }
    
    function loadConsortiumGrid() {
        var colArray = ['', '', 'Consortium Name'];
        colModel = []
        colModel.push({ name: 'ConsortiumID', index: 'ConsortiumID', key: true, editable: false, hidden: true });        
        colModel.push({ name: 'IsActive', index: 'IsActive', hidden: true });
        colModel.push({ name: 'ConsortiumName', index: 'ConsortiumName', editable: false });

        var url = URLs.ConsortiumList;

        //adding the pager element
        $(elementIDs.consortiumGridJQ).parent().append("<div id='p" + elementIDs.consortiumGrid + "'></div>");
        $(elementIDs.consortiumGridJQ).jqGrid(
            {
                url: url,
                datatype: 'json',
                cache: false,
                colNames: colArray,
                colModel: colModel,
                caption: 'Consortiums',
                loadonce: false,
                height: 400,
                rowList: [10, 20, 30],
                rowNum: 20,
                sortname: 'ConsortiumID',
                ignoreCase: true,
                autowidth: true,
                viewrecords: true,
                shrinktofit: true,
                hidegrid: false,
                altRows: true,
                altclass: 'alternate',
                pager: "#p" + elementIDs.consortiumGrid,
                gridComplete: function () {
                    //To set first row of grid as selected.
                    rows = $(this).getDataIDs();
                    if (rows && rows.length > 0) {
                        $(this).jqGrid("setSelection", rows[0]);
                    }
                    //to check for claims..              
                    var objMap = {
                        edit: elementIDs.editConsortiumJQ,
                        add: elementIDs.addConsortiumJQ
                    };
                    checkApplicationClaims(claims, objMap, URLs.ConsortiumList);
                }
            });
        var pagerElement = '#p' + elementIDs.consortiumGrid;
        //remove default buttons
        $(elementIDs.consortiumGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.consortiumGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

        //remove paging
        //$(pagerElement).find(pagerElement + '_center').remove();
        //$(pagerElement).find(pagerElement + '_right').remove();

        //add button in footer of grid 
        $(elementIDs.consortiumGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-plus', title: 'Add', id: elementIDs.addConsortium,
            onClickButton: function () {
                consortiumDialog.show('', 'add');
            }
        });

        //edit button in footer of grid 
        $(elementIDs.consortiumGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-pencil', title: 'Edit', id: elementIDs.editConsortium,
            onClickButton: function () {
                var consortiumID = $(elementIDs.consortiumGridJQ).jqGrid('getGridParam', 'selrow');
                var rowData = $(elementIDs.consortiumGridJQ).getRowData(consortiumID);
                if (consortiumID != null)
                    consortiumDialog.show(rowData, 'update');
                else
                    messageDialog.show(ConsortiumMessages.consortiumRowSelectionMsg);
            }
        });

    }

    init();

    return {
        loadConsortium: function () {
           loadConsortiumGrid();
        }
    }
}();

//contains functionality for the Consortium add/update dialog
var consortiumDialog = function () {
    var consortiumDialogState;
    var URLs = {
        //Add Consortium
        AddConsortium: '/Consortium/AddConsortium?tenantID=1',
        //Update Consortium
        UpdateConsortium: '/Consortium/UpdateConsortium?&tenantID=1',
    }
    var elementIDs = {
        consortiumDialog: '#consortiumDialog',
        consortiumNameJQ: '#consortiumName',
        btnsaveConsortiumJQ: '#btnsaveConsortium',
        consortiumGridJQ: '#consortiumGrid',
        consortiumNameSpanJQ: '#consortiumNameSpan'
    }

    //ajax success callback - for add/edit
    function consortiumSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            if (consortiumDialogState === "add") {
                messageDialog.show(ConsortiumMessages.addConsortiumMsg);
            }
            else {
                messageDialog.show(ConsortiumMessages.updateConsortiumMsg);
            }

            $(elementIDs.consortiumGridJQ).trigger("reloadGrid");
            $(elementIDs.consortiumDialog).dialog('close');
        }
        else {
            messageDialog.show(ConsortiumMessages.duplicateConsortiumMsg);
        }
    }
    function init() {
        //register dialog for grid row add/edit
        $(elementIDs.consortiumDialog).dialog({
            autoOpen: false,
            height: 250,
            width: 450,
            modal: true
        });

        //register event for save button click on dialog
        $(elementIDs.consortiumDialog + ' button').on('click', function () {
            var consortiumName = $(elementIDs.consortiumNameJQ).val();
            if (consortiumName == "") {
                $(elementIDs.consortiumNameJQ).parent().addClass('has-error');
                $(elementIDs.consortiumNameJQ).addClass('form-control');
                $(elementIDs.consortiumNameSpanJQ).text(ConsortiumMessages.consortiumNameRequiredMsg);
            }
            else{
            var consortiumData;
            if (consortiumDialogState == "add") {
                consortiumData = { consortiumName: consortiumName }
                var url = URLs.AddConsortium;
            }
            else {
                var consortiumID = $(elementIDs.consortiumGridJQ).jqGrid('getGridParam', 'selrow');
                consortiumData = {
                    consortiumID: consortiumID,
                    consortiumName: consortiumName
                }
                var url = URLs.UpdateConsortium;
            }

            var promise = ajaxWrapper.postJSON(url, consortiumData);

            promise.done(function (xhr) {
                consortiumSuccess(xhr)
            });
        }
        });
    }

    init();

    return {
        show: function (rowData, action) {
            $(elementIDs.consortiumDialog + ' input').val("");
            consortiumDialogState = action;

            $(elementIDs.consortiumNameSpanJQ).text('')
            $(elementIDs.consortiumNameJQ).parent().removeClass('has-error');
            $(elementIDs.consortiumNameJQ).removeClass('form-control');

            if (consortiumDialogState == "add")
                $(elementIDs.consortiumDialog).dialog('option', 'title', 'Add Consortium');
            else {
                $(elementIDs.consortiumDialog).dialog('option', 'title', 'Edit Consortium');
                $(elementIDs.consortiumNameJQ).val(rowData.ConsortiumName);
            }
            //open the dialog - uses jqueryui dialog
            $(elementIDs.consortiumDialog).dialog("open");
        }
    }
}();
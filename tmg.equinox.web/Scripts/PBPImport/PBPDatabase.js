var viewPBPDatabase = function () {

    var URLs = {
        getPBPDatabaseNameList:"/PBPImport/GetPBPDatabaselist",
    }
    var elementIDs = {
        pBPDatabaseNamegrid: "PBPDatabasegrid",
        pBPDatabaseNamegridJQ: "#PBPDatabasegrid",
        pBPDatabaseNameAddJQ: "#PBPDatabaseNameAdd",
        pBPDatabaseNameEditJQ: "#PBPDatabaseNameEdit",
        pBPDBNameJQ: "#pbpDBName",
        pBPDBDescriptionJQ: "#pbpDBDescription",
        
    };
    function init() {
        $(document).ready(function () {
            loadpBPDatabaseNamegrid();
        });
    }
    init();

    function loadpBPDatabaseNamegrid() {
        //set column list for grid
        var colArray = null;
        colArray = ['PBPDatabase1Up', 'Database Name', 'Database Description', 'Created By', 'Created Date',
                    'Updated By', 'Updated Date', 'Is Active'];

        //set column models
        var colModel = [];
        colModel.push({ key: true, hidden: true, name: 'PBPDatabase1Up', index: 'PBPDatabase1Up', align: 'right', editable: false });
        colModel.push({ key: false, name: 'DataBaseName', index: 'DataBaseName', editable: true });
        colModel.push({ key: false, name: 'DataBaseDescription',index: 'DataBaseDescription', editable: true });
        colModel.push({ key: false, name: 'CreatedBy', index: 'CreatedBy', editable: false, edittype: 'select', align: 'left' });
        colModel.push({ key: false, name: 'CreatedDate', index: 'CreatedDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' });
        colModel.push({ key: false, hidden: true, name: 'UpdatedBy', index: 'UpdatedBy', editable: false, edittype: 'select', align: 'center' });
        colModel.push({ key: false, hidden: true, name: 'UpdatedDate', index: 'UpdatedDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' });
        colModel.push({ key: false, hidden: true, name: 'IsActive', index: 'IsActive', align: 'left' });

        //clean up the grid first - only table element remains after this
        $(elementIDs.pBPDatabaseNamegrid).jqGrid('GridUnload');
        $(elementIDs.pBPDatabaseNamegridJQ).parent().append("<div id='p" + elementIDs.pBPDatabaseNamegrid + "'></div>");
        $(elementIDs.pBPDatabaseNamegridJQ).jqGrid({
            url: URLs.getPBPDatabaseNameList,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: '',
            height: 400,
            rowNum: 20,
            rowList: [20, 40, 60],
            headertitles: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortname: 'CreatedDate',
            sortorder: 'desc',
            pager: '#p' + elementIDs.pBPDatabaseNamegrid,
            gridComplete: function () {
                checkPBPDatabaseClaims(elementIDs, claims);
            }
        });
        var pagerElement = '#p' + elementIDs.pBPDatabaseNamegrid;
        //$('#p' + elementIDs.pBPDatabaseNamegrid).find('input').css('height', '22px');
        //remove default buttons
        $(elementIDs.pBPDatabaseNamegridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.pBPDatabaseNamegridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

        $(elementIDs.pBPDatabaseNamegridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-plus', title: 'Add', id: 'PBPDatabaseNameAdd',
               onClickButton: function () {
                   $(elementIDs.pBPDBNameJQ).val("");
                   $(elementIDs.pBPDBDescriptionJQ).val("");
                   $(elementIDs.pBPDBNameSpanJQ).text('');
                   var AddDBDialog = new showPBPDatabaseNameDialog();
                   AddDBDialog.show('','','','add');
                 
               }
           });
        $(elementIDs.pBPDatabaseNamegridJQ).jqGrid('navButtonAdd', pagerElement,
          {
              caption: '', buttonicon: 'ui-icon-pencil', title: 'Edit', id: 'PBPDatabaseNameEdit',
              onClickButton: function () {
                  
                  var rowId = $(this).getGridParam('selrow');
                  if (rowId !== undefined && rowId !== null) {
                      var row = $(this).getRowData(rowId);
                      var AddDBDialog = new showPBPDatabaseNameDialog();
                      //load PBPDatabase dialog for edit on click -
                      AddDBDialog.show(row.DataBaseName, row.DataBaseDescription, row.PBPDatabase1Up, 'edit');
                  }
                  else {
                      messageDialog.show(Common.pleaseSelectRowMsg);
                  }
              }
          });
    }
}
var showPBPDatabaseNameDialog = function () {
    var URLs = {
        
        addPBPDatabase: "/PBPImport/AddPBPDatabase",
        updatePBPDatabase: "/PBPImport/UpdatePBPDatabase",
    };

    var elementIDs = {
        
        addPBPDatabaseNameDialogJQ: "#addPBPDatabaseNameDialog",
        pBPDBNameJQ: "#pbpDBName",
        pBPDatabaseNamegridJQ: "#PBPDatabasegrid",
        pBPDBDescriptionJQ: "#pbpDBDescription",
        pBPDBNameSpanJQ:"#PBPDbnameSpan"
    }
    var databaseDialogState;
    var database1up;
    function databaseSuccess(xhr, actionMode) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            if (actionMode == 'add')
            { messageDialog.show(PBPDatabaseNameMessages.PBPDBadded); }
            else if (actionMode == 'edit')
            { messageDialog.show(PBPDatabaseNameMessages.PBPDBUpdated); }
            
            $(elementIDs.pBPDatabaseNamegridJQ).trigger('reloadGrid');
            $(elementIDs.addPBPDatabaseNameDialogJQ).dialog("close");
        }
    }
    function showError(xhr) {
        messageDialog.show(JSON.stringify(xhr)); 
    }
    function init() {
        //register dialog for grid row add/edit
        $(elementIDs.addPBPDatabaseNameDialogJQ).dialog({
            autoheight:true,
            autoOpen: false,
            width: 540,
            modal: true
        });
    }
   
    $(elementIDs.addPBPDatabaseNameDialogJQ + ' button').unbind().on('click', function () {

        //to check for duplicate database name
        var isExist = false;
        var PBPDatabaseName = $(elementIDs.pBPDBNameJQ).val().trim();
        var DatabaseDescription =$(elementIDs.pBPDBDescriptionJQ).val().trim();
        var DatabaseNameList = $(elementIDs.pBPDatabaseNamegridJQ).getRowData();
        var filterList = DatabaseNameList.filter(function (ct) {
            if (ct.DataBaseName.toUpperCase() === PBPDatabaseName.toUpperCase() && ct.PBPDatabase1Up != database1up) {
                isExist = true;
            }
        });
       if (isExist == true)
        {
            messageDialog.show(PBPDatabaseNameMessages.PBPDBNameExist);

        }
        else if (PBPDatabaseName == "") {
            $(elementIDs.pBPDBNameSpanJQ).text(PBPDatabaseNameMessages.PBPDBNameNull);
        }
        else {
            var NewDBData = {
                PBPDataBaseName: PBPDatabaseName,
                PBPDataBaseDescription: DatabaseDescription,
                PBPDatabase1Up: database1up,
            }

            var url;
            if (databaseDialogState === 'add') {
                NewDBData.PBPDatabase1Up = 0;
                url = URLs.addPBPDatabase;
            }
            else {
                url = URLs.updatePBPDatabase;
            }
            var promise = ajaxWrapper.postJSON(url, NewDBData);
            promise.done(function (data) { databaseSuccess(data, databaseDialogState) });
          
            promise.fail(showError);
        }
    });

    init();
    return {
        show: function (databaseName, databaseDescription,pBPDatabase1Up, action) {
            databaseDialogState = action;
            database1up = pBPDatabase1Up;
            $(elementIDs.addPBPDatabaseNameDialogJQ + ' input').each(function () {
                $(elementIDs.pBPDBNameJQ).val(databaseName);
                $(elementIDs.pBPDBDescriptionJQ).val(databaseDescription);
            });
            //set eleemnts based on whether the dialog is opened in add or edit mode
                $(elementIDs.addPBPDatabaseNameDialogJQ + ' div').removeClass('has-error');
                $(elementIDs.addPBPDatabaseNameDialogJQ + ' .help-block').text('');
                if (databaseDialogState == 'add') {
                    $(elementIDs.addPBPDatabaseNameDialogJQ).dialog('option', 'title', "Add PBP Database Name");
                }
                else {
                    $(elementIDs.addPBPDatabaseNameDialogJQ).dialog('option', 'title', "Edit PBP Database Name");

                }
            $(elementIDs.addPBPDatabaseNameDialogJQ).dialog("open");
        }
    }
}
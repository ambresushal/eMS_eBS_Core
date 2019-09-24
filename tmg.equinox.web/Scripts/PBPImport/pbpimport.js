var selectedRowData;
var IsNeededToSave = true;
var viewQueuedPBPImports = function () {
    var URLs = {
        getQueuedPBPImportList: "/PBPImport/GetQueuedPBPImportList",
        exportToExcel: '/PBPImport/ExportPBPImportQueuedToExcel',
        GetMatchAndMisMatchPlanList: "/PBPImport/GetMatchAndMisMatchPlanList",
        GetCascadeMLProcessingOrQueued: "/FolderVersion/GetCascadeMLProcessingOrQueued",
        GetQueuedOrProcessingPBPImport: "/PBPImport/GetQueuedOrProcessingPBPImport",
        GetQueuedOrProcessingPBPExport: "/PBPExport/GetQueuedOrProcessingPBPExport"
    };

    var elementIDs = {
        queuedPBPImportGrid: "queuedPBPImportGrid",
        queuedPBPImportGridJQ: "#queuedPBPImportGrid",
        importPBPbtn: "#importPBPbtn",
        addButtonJQ: "#PBPImportAdd",
        editButtonJQ: "#PBPImportEdit",
        btnImportJQ: "#btnImport"
    };

    var timer = 60;

    function init() {
        $(document).ready(function () {
            loadQueuedPBPImportGrid();
            refreshInterval = setInterval(AutoRefreshPBPImportQueued, 1000);
            $(elementIDs.btnImportJQ).click(function () {
                //check for ML Cascade and Export
                var urlML = URLs.GetCascadeMLProcessingOrQueued;
                var promiseML = ajaxWrapper.getJSON(urlML);
                var urlExport = URLs.GetQueuedOrProcessingPBPExport;
                var promiseExport = ajaxWrapper.getJSON(urlExport);
                var urlImport = URLs.GetQueuedOrProcessingPBPImport;
                var promiseImport = ajaxWrapper.getJSON(urlImport);
                $.when(promiseML, promiseExport, promiseImport).done(function (xhrML, xhrExport, xhrImport) {
                    var ml = xhrML[0];
                    var exp = xhrExport[0];
                    var imprt = xhrImport[0];
                    if (imprt == true) {
                        messageDialog.show("PBP Import is already queued or being Processed . Please try again later.");
                    }
                    else if (ml == true) {
                        messageDialog.show("A Master List Cascade is already queued or being Processed. Please try again later. Visit the Master List Cascade status screen to check for Status.");
                    }
                    else if (exp == true) {
                        messageDialog.show("A PBP Export is already queued or being Processed. Please try again later. Visit the PBP Export Screen to check the status of the Export that is queued or being Processed.");
                    }
                    if (ml == false && exp == false && imprt == false) {
                        var queueDialog = new showImportPBPQueueDialog();
                        queueDialog.show();
                    }
                });
            });
        });
    }
    init();

    function loadQueuedPBPImportGrid() {
        //set column list for grid
        var colArray = null;
        colArray = ['Id', 'Import ID', 'Description', 'Database Name', 'PBP File Name', 'PBP Plan Area File Name', 'Plan Year',
                    'Imported Date & Timestamp', 'Imported By', 'Import Status', '', ''];

        //set column models
        var colModel = [];
        colModel.push({ key: true, hidden: true, name: 'Id', index: 'Id', align: 'right', editable: true });
        colModel.push({ key: false, name: 'PBPImportQueueID', index: 'PBPImportQueueID', editable: false });
        colModel.push({ key: false, name: 'Description', index: 'Description', editable: false });
        colModel.push({ key: false, name: 'DataBaseName', index: 'DataBaseName', align: 'left' });
        colModel.push({ key: false, name: 'PBPFileDisplayName', index: 'PBPFileDisplayName', editable: true, edittype: 'select', align: 'left' });
        colModel.push({ key: false, name: 'PBPPlanAreaFileDisplayName', index: 'PBPPlanAreaFileDisplayName', editable: true, edittype: 'select', align: 'left' });
        colModel.push({ key: false, name: 'Year', index: 'Year', hidden: false, align: 'left' });
        colModel.push({ key: false, name: 'CreatedDate', index: 'CreatedDate', editable: false, align: 'left', width: 200, formatter: 'date', formatoptions: JQGridSettings.DateTimeFormatterOptions });
        colModel.push({ key: false, name: 'CreatedBy', index: 'CreatedBy', align: 'left' });
        colModel.push({
            key: false, name: 'StatusText', index: 'StatusText', editable: false, align: 'left', formatter: processStatusFormmater
        });
        colModel.push({
            key: false, hidden: true, name: 'StatusCode', index: 'StatusCode', editable: false, align: 'left'
        });
        colModel.push({
            key: false, hidden: true, name: 'ErrorMessage', index: 'ErrorMessage', editable: false, align: 'left'
        });
        //clean up the grid first - only table element remains after this
        $(elementIDs.queuedPBPImportGrid).jqGrid('GridUnload');

        //adding the pager element
        $(elementIDs.queuedPBPImportGridJQ).parent().append("<div id='p" + elementIDs.queuedPBPImportGrid + "'></div>");

        $(elementIDs.queuedPBPImportGridJQ).jqGrid({
            url: URLs.getQueuedPBPImportList,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: '',
            height: 380,
            rowNum: 20,
            rowList: [20, 40, 60],
            headertitles: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortname: 'PBPImportQueueID',
            sortorder: 'desc',
            pager: '#p' + elementIDs.queuedPBPImportGrid,
            //altclass: 'alternate'
            gridComplete: function () {
                checkPBPImportClaims(elementIDs, claims);
            },
            onCellSelect: function (rowid, index, contents, event) {
                showError(index, rowid);
            }
        });

        var pagerElement = '#p' + elementIDs.queuedPBPImportGrid;
        //$('#p' + elementIDs.queuedPBPImportGrid).find('input').css('height', '20px');
        //remove default buttons
        $(elementIDs.queuedPBPImportGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.queuedPBPImportGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

        $(elementIDs.queuedPBPImportGridJQ).jqGrid('navButtonAdd', pagerElement,
          {
              caption: '', buttonicon: 'ui-icon-pencil', title: 'Edit', id: 'PBPImportEdit',
              onClickButton: function () {
                  var selectedRowId = $(elementIDs.queuedPBPImportGridJQ).jqGrid('getGridParam', 'selrow');
                  var selectRowData;
                  if (selectedRowId != null && selectedRowId != undefined) {
                      selectRowData = $(elementIDs.queuedPBPImportGridJQ).getRowData(selectedRowId);

                      if (selectRowData.StatusCode == "8") {
                          getAllPlanConfigurationList(selectRowData.PBPImportQueueID);
                      }
                      else {
                          messageDialog.show(queuedPBPImportGridMsg.EditRowMsg);
                      }
                  }
                  else {
                      messageDialog.show(Common.pleaseSelectRowMsg);
                  }

              }
          });

        //$(elementIDs.queuedPBPImportGridJQ).jqGrid('navButtonAdd', pagerElement,
        // {
        //     caption: '', buttonicon: 'ui-icon-plus', title: 'Add', id: 'PBPImportAdd',
        //     onClickButton: function () {
        //         var queueDialog = new showImportPBPQueueDialog();
        //         queueDialog.show();
        //     },


        // });
        $(elementIDs.queuedPBPImportGridJQ).jqGrid('navButtonAdd', pagerElement,
         {
             caption: '', buttonicon: 'ui-icon-arrowstop-1-s', title: 'Download PBP Import Queue List', id: 'btnPBPImportQueueListExportToExcel',

             onClickButton: function () {
                 var currentInstance = this;
                 var jqGridtoCsv = new JQGridtoCsv(elementIDs.queuedPBPImportGridJQ, false, currentInstance);
                 jqGridtoCsv.buildExportOptions();
                 var stringData = "csv=" + jqGridtoCsv.csvData;
                 stringData += "<&isGroupHeader=" + jqGridtoCsv.isGroupHeader;
                 stringData += "<&noOfColInGroup=" + jqGridtoCsv.noofGroupedColumns;
                 stringData += "<&repeaterName=" + elementIDs.userListGrid;

                 $.download(URLs.exportToExcel, stringData, 'post');
             }
         });

        function showError(index, rowid) {
            var cm = $(elementIDs.queuedPBPImportGridJQ).jqGrid('getGridParam', 'colModel');
            if (cm[index].name == "StatusText") {
                var rowData = $(elementIDs.queuedPBPImportGridJQ).getRowData(rowid);
                if (rowData.StatusCode == "3") {
                    messageDialog.show(rowData.ErrorMessage);
                }
            }
        }
    }

    function AutoRefreshPBPImportQueued() {
        timer--;
        if (timer == 0) {
            $(elementIDs.queuedPBPImportGridJQ).trigger("reloadGrid");
            timer = 60;
        }
        $("#spnTimer").text(timer + " seconds.").css("font-weight", "Bold");
    }
    // Function to set font colour based on status using formatter 
    function processStatusFormmater(cellvalue, options, rowObject) {
        switch (cellvalue) {

            case 'Queued':
                // If status is Queued then display status text in back color 
                cellvalue = '<span style="">' + 'Queued' + '</span>';

                break;
            case 'In Progress':
                // If status is Queued then display status text in back color 
                cellvalue = '<span style="color:blue">' + 'In Progress' + '</span>';

                break;
            case 'Errored':
                // If status is Queued then display status text in back color 
                cellvalue = '<span style="color:red"><b><u>' + 'Errored' + '</u></b></span>';

                break;
            case 'Complete':
                // If status is Queued then display status text in back color 
                cellvalue = '<span style="color:green">' + 'Complete' + '</span>';

                break;
            case 'Finalized':
                // If status is Queued then display status text in back color 
                cellvalue = '<span style="color:blue">' + 'Finalized' + '</span>';

                break;
            case 'Not Scheduled':
                // If status is Queued then display status text in back color 
                cellvalue = '<span style="color:blue">' + 'Not Scheduled' + '</span>';

                break;
            case 'Scheduled':
                // If status is Queued then display status text in back color 
                cellvalue = '<span style="color:blue">' + 'Scheduled' + '</span>';

                break;
            case 'In Review':
                // If status is Queued then display status text in back color 
                cellvalue = '<span style="color:#ffb833"><b><u>' + 'In Review' + '</u></b></span>';

                break;
            case 'Cancel':
                // If status is Queued then display status text in back color 
                cellvalue = '<span style="color:blue">' + 'Cancel' + '</span>';
                break;
        }
        return cellvalue;
    } // processStatusFormmater

    function showError(msg) {
        messageDialog.show(JSON.stringify(msg));
    }

    var getAllPlanConfigurationList = function (pBPImportQueueID) {
        var postData = {
            "PBPImportQueueID": parseInt(pBPImportQueueID)
        }
        var promise = ajaxWrapper.postJSON(URLs.GetMatchAndMisMatchPlanList, postData);
        //fill the folder list drop down
        promise.done(function (result) {
            if (result.MatchPlanList.length > 0 || result.MisMatchPlanList.length > 0) {
                var Dialog = new pBPPlanMatchingConfigrationDialog();
                Dialog.show(result);
            }
        });

    }
}

var showImportPBPQueueDialog = function () {
    var URLs = {
        //Queue Collateral
        addimportPBPQueue: "/PBPImport/UploadPBPFiles",
        getDatabaseNameList: "/PBPImport/GetPBPDatabaseNameList"
    };

    var elementIDs = {
        //collateral Queue Dialog
        importPBPQueueDialog: "#importPBPQueueDialog",
        //Input radio type
        importPBPQueueTable: "#importPBPQueueTable",
        //lblcollateralQueueRunDate: "#lblCollateralQueueRunDate",
        queuedPBPImportGridJQ: "#queuedPBPImportGrid",
        description: "#description",
        uploadpbpFile: "#uploadpbpFile",
        UploadPBPFileNameJQ: "#UploadPBPFileName",
        UploadPBPPlanAreaFileNameJQ: "#UploadPBPPlanAreaFileName",
        UploadPBPFileNameSpan: "#UploadPBPFileNameSpan",
        UploadPBPPlanAreaFileNameSpan: "#UploadPBPPlanAreaFileNameSpan",
        btnPBPQueue: "#btnPBPQueue",
        btnSelectPortfolio: '#btnSelectPortfolioDetails',
        yearJQ: '#year',
        folderId: '#folderId',
        folderVersionId: '#folderVersionId',
        folderVersionEffectiveDate: '#folderVersionEffectiveDate',
        databaseNameJQ: "#databaseName",
        DatabaseNameSpan: "#databaseNameSpan",
        queuedPBPImportGridJQ: "#queuedPBPImportGrid"
    };

    function init() {
        //register dialog for grid row add/edit
        $(elementIDs.importPBPQueueDialog).dialog({
            autoOpen: false,
            height: 250,
            width: 610,
            modal: true,
            close: function (event, ui) {
                $(elementIDs.databaseNameJQ).autoCompleteDropDown("destroy");
            }
        });
        //register event for button click on dialog
        $(elementIDs.importPBPQueueDialog + ' button').unbind().on('click', function () {
            var file;
            var selectedYear = $(elementIDs.year).val(); //+ ' option: selected'
            selectedYear = 2018;
            var pBPFileName = $(elementIDs.UploadPBPFileNameJQ).val();
            var pBPFileNameUpload = $(elementIDs.UploadPBPFileNameJQ).get(0);
            var pBPFileNamefiles = pBPFileNameUpload.files;

            var pBPPlanAreaFileName = $(elementIDs.UploadPBPPlanAreaFileNameJQ).val();
            var pBPPlanAreaFileNameUpload = $(elementIDs.UploadPBPPlanAreaFileNameJQ).get(0);
            var pBPPlanAreaFileNamefiles = pBPPlanAreaFileNameUpload.files;
            var fileData = new FormData();

            // Looping over all files and add it to FormData object  
            for (var i = 0; i < pBPFileNamefiles.length; i++) {
                fileData.append(pBPFileNamefiles[i].name, pBPFileNamefiles[i]);
            }

            for (var i = 0; i < pBPPlanAreaFileNamefiles.length; i++) {
                fileData.append(pBPPlanAreaFileNamefiles[i].name, pBPPlanAreaFileNamefiles[i]);
            }

            // Adding description key to FormData object  
            var descriptionValue = $(elementIDs.description).val();
            fileData.append('description', descriptionValue);
            fileData.append('year', $(elementIDs.yearJQ).val());

            fileData.append('PBPDatabase1Up', $(elementIDs.databaseNameJQ).val());

            if (validate()) {
                $(elementIDs.databaseNameJQ).removeClass('form-control');
                $(elementIDs.databaseNameJQ).parent().removeClass('has-error');
                saveImport(fileData);
            }

        });

        $(elementIDs.btnSelectPortfolio).unbind().on('click', function () {
            var portfolioSearch = new viewPortfolioSearchDialog();
            portfolioSearch.show();
        });

        $(document).ready(function () {
            $(elementIDs.importPBPQueueTable).show();
            $(elementIDs.yearJQ).empty();
            fillYearDropDown();
        });

        validate = function () {
            var allowedExtensions = ["MDB", "LDB", "mdb"];
            var PBPFileNameIsValid = false;
            var PBPPlanAreaFileNameIsValid = false;
            var PBPFileName = $(elementIDs.UploadPBPFileNameJQ).val();
            if (PBPFileName == '') {
                $(elementIDs.UploadPBPFileNameJQ).parent().addClass('has-error');
                $(elementIDs.UploadPBPFileNameJQ).addClass('form-control');
                $(elementIDs.UploadPBPFileNameSpan).text(PBPImportMessages.PBPFileRequiredMsg);
                isValid = false
            }
            else {
                if (PBPFileName) {
                    var fileN = PBPFileName.split('.');
                    if ($.inArray(fileN[fileN.length - 1], allowedExtensions) == -1) {
                        messageDialog.show(PBPImportMessages.InvalidFileExtensions);
                        PBPFileNameIsValid = false;
                    }
                    else {
                        file = $(elementIDs.UploadPBPFileNameJQ)[0].files[0];
                        $(elementIDs.UploadPBPFileNameJQ).removeClass('form-control');
                        $(elementIDs.UploadPBPFileNameJQ).parent().addClass('has-error');
                        $(elementIDs.UploadPBPFileNameSpan).text('');
                        PBPFileNameIsValid = true;
                    }
                }
            }

            var PBPPlanAreaFileName = $(elementIDs.UploadPBPPlanAreaFileNameJQ).val();
            if (PBPPlanAreaFileName == '') {
                $(elementIDs.UploadPBPPlanAreaFileNameJQ).parent().addClass('has-error');
                $(elementIDs.UploadPBPPlanAreaFileNameJQ).addClass('form-control');
                $(elementIDs.UploadPBPPlanAreaFileNameSpan).text(PBPImportMessages.PBPPlanAreaFileRequiredMsg);
                isValid = false
            }
            else {
                if (PBPPlanAreaFileName) {
                    var fileN = PBPPlanAreaFileName.split('.');
                    if ($.inArray(fileN[fileN.length - 1], allowedExtensions) == -1) {
                        messageDialog.show(PBPImportMessages.InvalidFileExtensions);
                        PBPPlanAreaFileNameIsValid = false;
                    }
                    else {
                        file = $(elementIDs.UploadPBPPlanAreaFileNameJQ)[0].files[0];
                        $(elementIDs.UploadPBPPlanAreaFileNameJQ).removeClass('form-control');
                        $(elementIDs.UploadPBPPlanAreaFileNameJQ).parent().addClass('has-error');
                        $(elementIDs.UploadPBPPlanAreaFileNameSpan).text('');
                        PBPPlanAreaFileNameIsValid = true;
                    }
                }
            }

            var selectDDL = $("select[name='databaseName'] option:selected").index()
            if (selectDDL == '0') {
                $(elementIDs.databaseNameJQ).parent().addClass('has-error');
                $(elementIDs.databaseNameJQ).addClass('form-control');
                $(elementIDs.DatabaseNameSpan).text(PBPImportMessages.DatabaseNameRequiredMsg);
                isValid = false
            }
            else {
                $(elementIDs.databaseNameJQ).removeClass('form-control');
                $(elementIDs.databaseNameJQ).parent().addClass('has-error');
                $(elementIDs.DatabaseNameSpan).text('');
                isValid = true;
            }
            return PBPFileNameIsValid && PBPPlanAreaFileNameIsValid && isValid;
        }

        saveImport = function (fileData) {
            if (validate()) {
                $.ajax({
                    url: URLs.addimportPBPQueue,
                    type: "POST",
                    contentType: false, // Not to set any content header  
                    processData: false, // Not to process data  
                    data: fileData,
                    success: function (result) {
                        $(elementIDs.importPBPQueueDialog).dialog("close");
                        if (result.PBPImportQueueID > 0) {
                            $(elementIDs.queuedPBPImportGridJQ).trigger('reloadGrid');
                            var Dialog = new pBPPlanMatchingConfigrationDialog();
                            Dialog.show(result);
                        }
                        else if (result == false) {
                            messageDialog.show(pBPPlanMatchingConfigrationDialogMsg.FileisnotselectedMsg);
                        }
                        else {
                            if (result.ResultCode == 1) {
                                messageDialog.show(pBPPlanMatchingConfigrationDialogMsg.QIDisnotsameMsg + result.ErrorMsg);
                            }
                            else if (result.ResultCode == 2) {
                                messageDialog.show(pBPPlanMatchingConfigrationDialogMsg.FileschemaisinvalidMsg);
                            }

                            else if (result.ResultCode == 3) {
                                messageDialog.show(pBPPlanMatchingConfigrationDialogMsg.Errorinfileupload);
                            }
                        }
                    },
                    error: function (err) {
                        messageDialog.show(err.statusText);
                    }
                });
            }
        }


    }

    fillYearDropDown = function () {
        //Start********************Populate Year Dropdown
        var startYear = 2016;
        var date = new Date();
        var currentYear = date.getFullYear();
        while (currentYear - startYear >= 0) {
            startYear = startYear + 1;
            $(elementIDs.yearJQ).append("<option value=" + startYear + ">" + startYear + "</option>");
            $(elementIDs.yearJQ).val(currentYear);
        }
        //End********************Populate Year Dropdown
    }

    getDatabaseNameList = function () {
        var DatabaseNameList = "";
        $(elementIDs.databaseNameJQ).empty();
        $(elementIDs.databaseNameJQ).append("<option>" + "--Select--" + "</option>");
        var promise = ajaxWrapper.getJSON(URLs.getDatabaseNameList);
        //fill the folder list drop down
        promise.done(function (list) {
            if (list.length > 0) {
                jQuery.each(list, function (index, item) {
                    //DatabaseNameList += "<option value=" + item.PBPDatabase1Up + ">" + item.DataBaseName + "</option>";

                    $(elementIDs.databaseNameJQ).append("<option value=" + item.PBPDatabase1Up + ">" + item.DataBaseName + "</option>");
                });
                //$(elementIDs.databaseNameJQ).html(DatabaseNameList);
                $(elementIDs.databaseNameJQ).val("--Select--");
                $(elementIDs.importPBPQueueDialog).dialog('option', 'title', "Queue PBP Import");
                $(elementIDs.importPBPQueueDialog).dialog("open");
            }
        });
    }
    //initialize the dialog after this js is loaded

    validateUploadedFileAtServerSide = function () {

    }

    clientSideValidation = function () {

    }
    init();

    return {
        show: function () {
            getDatabaseNameList();
            $(elementIDs.databaseNameJQ).autoCompleteDropDown({ ID: 'databaseName' });
            $(elementIDs.databaseNameJQ).click(function () {
                $(elementIDs.databaseNameJQ).toggle();
            });
            var descriptionValue = $(elementIDs.description).val('');
            var PBPFileName = $(elementIDs.UploadPBPFileNameJQ).val('');
            var PBPPlanAreaFileName = $(elementIDs.UploadPBPPlanAreaFileNameJQ).val('');
            $(elementIDs.UploadPBPFileNameJQ).removeClass('form-control');
            $(elementIDs.UploadPBPFileNameJQ).parent().addClass('has-error');
            $(elementIDs.UploadPBPFileNameSpan).text('');
            $(elementIDs.UploadPBPPlanAreaFileNameJQ).removeClass('form-control');
            $(elementIDs.UploadPBPPlanAreaFileNameJQ).parent().addClass('has-error');
            $(elementIDs.UploadPBPPlanAreaFileNameSpan).text('');
            var DatabaseNameValue = $(elementIDs.databaseNameJQ).val('Select');
            $(elementIDs.databaseNameJQ).removeClass('form-control');
            $(elementIDs.databaseNameJQ).parent().removeClass('has-error');
            //$(elementIDs.databaseNameJQ).parent().addClass('has-error');
            $(elementIDs.DatabaseNameSpan).text('');
        }
    }
}

var viewPortfolioSearchDialog = function () {
    var URLs = {
        portfolioDetails: '/ConsumerAccount/GetPortfolioFolderListForPBPImport?tenantId=1',
    };

    var elementIDs = {
        sourceDialogJQ: "#portfolioDetailsDialog",
        portfolioSearchGrid: "portfolioSearchGrid",
        portfolioSearchGridJQ: "#portfolioSearchGrid",
        portfolioSearchGridPager: "#pportfolioSearchGrid",
        lblSelectedPortfolio: '#lblSelectedPortfolio',
        portfolioName: '#portfolioName',
        misMatchPlanGridJQ: "#misMatchPlanGrid",
    };

    function init() {
        $(elementIDs.sourceDialogJQ).dialog({
            autoOpen: false,
            height: 500,
            width: 700,
            modal: true,
            //close: function (event, ui) {
            //    var Dialog = new pBPPlanMatchingConfigrationDialog();
            //    Dialog.ClearFields(undefined,true);
            //}
        });
    }

    init();

    function loadPortfolioGrid(year) {
        //set column list for grid
        var colArray = ['', '', '', '', '', '', '', 'Portfolio Name', 'Folder Version Number', 'Effective Date', 'ProductList', 'Product Types', 'Product Names', 'Status', '', 'Uses', ''];

        //set column models
        var colModel = [];
        colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true });
        colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true, key: true });
        colModel.push({ name: 'FolderVersionCount', index: 'FolderVersionCount', hidden: true });
        colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', hidden: true });
        colModel.push({ name: 'AccountID', index: 'AccountID', hidden: true });
        colModel.push({ name: 'MarketSegmentID', index: 'MarketSegmentID', hidden: true });
        colModel.push({ name: 'PrimaryContactID', index: 'PrimaryContactID', hidden: true });
        colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left' });
        colModel.push({ name: 'VersionNumber', index: 'VersionNumber', editable: false, align: 'left' });
        colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' });
        colModel.push({ name: 'ProductList', index: 'ProductList', key: false, hidden: true, editable: false });
        colModel.push({ name: 'ProductType', index: 'ProductType', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'ProductName', index: 'ProductName', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'Status', index: 'Status', editable: false, align: 'center', hidden: true });
        colModel.push({ name: 'ApprovalStatusID', index: 'ApprovalStatusID', hidden: true });
        colModel.push({ name: 'UsesCount', index: 'UsesCount', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'IsExpanded', index: 'IsExpanded', key: true, hidden: true, editable: false });

        //clean up the grid first - only table element remains after this
        $(elementIDs.portfolioSearchGridJQ).jqGrid('GridUnload');
        //adding the pager element
        $(elementIDs.portfolioSearchGridJQ).parent().append("<div id='p" + elementIDs.portfolioSearchGrid + "'></div>");
        $(elementIDs.portfolioSearchGridJQ).jqGrid({
            url: URLs.portfolioDetails + "&year=" + parseInt(year),
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Portfolio Search',
            height: '300',
            rowNum: 20,
            rowList: [10, 20, 30],
            autowidth: true,
            shrinkToFit: true,
            viewrecords: true,
            headertitles: true,
            altRows: true,
            hidegrid: false,
            viewrecords: true,
            pager: elementIDs.portfolioSearchGridPager,
            sortname: 'FolderID',
            altclass: 'alternate',
            resizeStop: function (width, index) {
                autoResizing(elementIDs.portfolioDetailsGridJQ);
            },
            ondblClickRow: function () {
                setPortfolioDocumentSelection();
            }
        });
        var pagerElement = elementIDs.portfolioSearchGridPager;
        //remove default buttons
        $(elementIDs.portfolioSearchGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

        $(elementIDs.portfolioSearchGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
        $(elementIDs.portfolioSearchGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-check', title: 'Select',
               onClickButton: function () {
                   setSelectedFolderValuesToTarget();
               }
           });
    }

    function setSelectedFolderValuesToTarget() {
        var targetSelectedRowId = $(elementIDs.misMatchPlanGridJQ).jqGrid('getGridParam', 'selrow');
        if (targetSelectedRowId != null && targetSelectedRowId != undefined) {
            var sourceSelectedRowId = $(elementIDs.portfolioSearchGridJQ).jqGrid('getGridParam', 'selrow');
            if (sourceSelectedRowId != null && sourceSelectedRowId != undefined) {
                var validationmanaer = validationManager();
                validationmanaer.removeValidation(parseInt(targetSelectedRowId), 8);
                sourceSelectedRowData = $(elementIDs.portfolioSearchGridJQ).jqGrid('getRowData', sourceSelectedRowId);
                var targetSelectedRowData = $(elementIDs.misMatchPlanGridJQ).jqGrid('getRowData', targetSelectedRowId);
                if (sourceSelectedRowData.ProductList != "") {
                    var productlist = sourceSelectedRowData.ProductList.split(',');
                    if (productlist.indexOf(targetSelectedRowData.PlanNumber) >= 0) {
                        //applyValidation(GridData[i].RowNumber, 8);
                        messageDialog.show("Product is already exist in selected folder");
                    }
                    else {
                        $(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', targetSelectedRowId, 'FolderId', sourceSelectedRowData.FolderID);
                        $(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', targetSelectedRowId, 'FolderVersionId', sourceSelectedRowData.FolderVersionID);
                        $(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', targetSelectedRowId, 'FolderVersion', sourceSelectedRowData.FolderName + "_" + sourceSelectedRowData.VersionNumber);
                        $(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', targetSelectedRowId, 'eMSPlanNumber', targetSelectedRowData.PlanNumber);
                        $(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', targetSelectedRowId, 'ebsPlanName', targetSelectedRowData.PlanName);
                        $(elementIDs.sourceDialogJQ).dialog("close");
                    }
                }
                else {
                    $(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', targetSelectedRowId, 'FolderId', sourceSelectedRowData.FolderID);
                    $(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', targetSelectedRowId, 'FolderVersionId', sourceSelectedRowData.FolderVersionID);
                    $(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', targetSelectedRowId, 'FolderVersion', sourceSelectedRowData.FolderName + "_" + sourceSelectedRowData.VersionNumber);
                    $(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', targetSelectedRowId, 'eMSPlanNumber', targetSelectedRowData.PlanNumber);
                    $(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', targetSelectedRowId, 'ebsPlanName', targetSelectedRowData.PlanName);
                    $(elementIDs.sourceDialogJQ).dialog("close");
                }


            }
            else {
                messageDialog.show(common.pleaseSelectRowMsg);
            }
        }
        else {
            messageDialog.show(common.pleaseSelectRowMsg);
        }
    }

    validate = function () {
        var selectedRowId = $(elementIDs.portfolioSearchGridJQ).jqGrid('getGridParam', 'selrow');
        try {
            if (selectedRowId != undefined) {
                parseInt(selectedRowId);
                return true;
            }
            else {
                return false;
            }
        }
        catch (ex) {

        }
    }

    return {
        show: function (year) {
            $(elementIDs.sourceDialogJQ).dialog('option', 'title', "Select Portfolio Folder");
            $(elementIDs.sourceDialogJQ).dialog("open");
            loadPortfolioGrid(year);
        }
    }
}

var pBPPlanMatchingConfigrationDialog = function () {
    var URLs = {
        getMisMatchPlanConfigrationList: "/PBPImport/GetMisMatchPlanList",
        GetMatchPlanList: "/PBPImport/GetMatchPlanList",
        portfolioDetails: '/PBPImport/GetLatestInProgressFolderVersionList',
        exportToExcel: '/PBPImport/ExportToExcel',
        savePlanConfigurationDetail: '/PBPImport/SavePlanConfigurationDetail',
        btnSelectPortfolio: '#btnSelectPortfolioDetails',
        getUserActionList: "/PBPImport/GetUserActionList",
        updatePlanConfig: "/PBPImport/UpdatePlanConfig",
        updateMatchPlanConfig: "/PBPImport/UpdateMatchPlanConfig",
        GetCascadeMLProcessingOrQueued: "/FolderVersion/GetCascadeMLProcessingOrQueued",
        GetQueuedOrProcessingPBPImport: "/PBPImport/GetQueuedOrProcessingPBPImport",
        GetQueuedOrProcessingPBPExport: "/PBPExport/GetQueuedOrProcessingPBPExport"
    };
    var elementIDs = {
        //collateral Queue Dialog
        pBPPlanMatchingConfigrationDialog: "#pBPPlanMatchingConfigrationDialog",
        //Input radio type
        misMatchPlanGridJQ: "#misMatchPlanGrid",
        misMatchPlanGrid: "misMatchPlanGrid",
        matchPlanGridJQ: "#matchPlanGrid",
        matchPlanGrid: "matchPlanGrid",
        btnpBPPlanMatchingConfigrationDialogCloseJQ: "#btnpBPPlanMatchingConfigrationDialogClose",
        btnPreviewImport: "#btnPreviewImport",
        previewPBPConfigrationDialogJQ: "#previewPBPConfigrationDialog",
        tblPreviewPBPConfigrationJQ: "#tblPreviewPBPConfigration",
        tblPreviewPBPConfigration: "tblPreviewPBPConfigration",
        btnDownloadExcelJQ: "#btnDownloadExcel",
        btnProcessWithImportJQ: "#btnProcessWithImport",
        btnPreviewCloseJQ: "#btnPreviewClose",
        btnDocumentJQ: "ui-icon-document",
        folderId: "#folderId",
        folderVersionIdJQ: "#folderVersionId",
        folderVersionNumberJQ: "#folderVersionNumber",
        folderVersionEffectiveDateJQ: "#folderVersionEffectiveDate",
        folderNameJQ: "#folderName",
        productNameJQ: "#productName",
        ProductDetailsJQ: "#FolderVersion",
        documentIdJQ: "#documentId",
        FormInstanceIdJQ: "#FormInstanceId",
        queuedPBPImportGridJQ: "#queuedPBPImportGrid",
        previewPBPConfigrationDialogbtnDiscardJQ: "#previewPBPConfigrationDialogbtnDiscard",
        pBPPlanMatchingConfigrationDialogbtnDiscardJQ: "#pBPPlanMatchingConfigrationDialogbtnDiscard"
    };

    function init() {
        $(elementIDs.pBPPlanMatchingConfigrationDialog).dialog({
            autoOpen: false,
            height: 610,
            width: 1200,
            modal: true,
            open: function (event, ui) {
                $(this).parent().find('span.ui-dialog-title').addClass('titleClassUpper');
            },
            autoOpen: false,
            beforeClose: function (event, ui) {
                if (IsNeededToSave) {
                    UpdatePlanConfig(true);
                }
                $(elementIDs.queuedPBPImportGridJQ).trigger('reloadGrid');
                //pBPPlanMatchingConfigrationDialogClose();
            },
            onSelectRow: function (id) {
                if (id && id !== lastsel2) {
                    var rowObject = $(elementIDs.misMatchPlanGridJQ).getRowData(id);
                    {
                        $(elementIDs.misMatchPlanGridJQ).saveRow(lastsel2);
                        $(elementIDs.misMatchPlanGridJQ).editRow(id, true);
                        lastsel2 = id;
                    }
                }
            },
        });
        $(document).ready(function () {
            $(elementIDs.pBPPlanMatchingConfigrationDialog).show();
            eventRegister();
        });
    }

    eventRegister = function () {
        $(elementIDs.btnpBPPlanMatchingConfigrationDialogCloseJQ).click(function () {
            pBPPlanMatchingConfigrationDialogClose();
        });
        $(elementIDs.btnPreviewImport).unbind('click');
        $(elementIDs.btnPreviewImport).click(function () {
            if (validateGridData() == 0) {
                IsNeededToSave = true;
                UpdatePlanConfig(false);
                IsNeededToSave = false;
                //previewConfigrationDialog();
            }
        });
        $(elementIDs.previewPBPConfigrationDialogbtnDiscardJQ).click(function () {
            var pbpImportId = 0;
            var PreviewPBPPlans = $(elementIDs.tblPreviewPBPConfigrationJQ).getGridParam('data');
            if (PreviewPBPPlans.length > 0) {
                pbpImportId = parseInt(PreviewPBPPlans[0].PBPImportQueueID);
            }
            if (pbpImportId > 0) {
                var disacard = discardImport();
                disacard.Discard(pbpImportId, 2);
            }
        });
        $(elementIDs.pBPPlanMatchingConfigrationDialogbtnDiscardJQ).click(function () {
            var pbpImportId = 0;
            var MissGridData = $(elementIDs.misMatchPlanGridJQ).getGridParam('data');
            var MatchGridData = $(elementIDs.matchPlanGridJQ).getGridParam('data');

            if (MissGridData.length > 0) {
                pbpImportId = MissGridData[0].PBPImportQueueID;
            }
            else if (MatchGridData.length > 0) {
                pbpImportId = MatchGridData[0].PBPImportQueueID;
            }

            if (pbpImportId > 0) {
                var disacard = discardImport();
                disacard.Discard(pbpImportId, 1);
            }
        });
    }

    UpdatePlanConfig = function (isCloseConfigWindow) {
        //var matchGridData = $(elementIDs.matchPlanGridJQ).jqGrid('getRowData');
        var GridData = $(elementIDs.misMatchPlanGridJQ).getGridParam('data');
        ///var Data = matchGridData.concat(unMatchGridData);

        //$.each(GridData, function (i) {
        //    GridData[i].FolderId = parseInt(GridData[i].FolderId != "" ? GridData[i].FolderId : 0);
        //    GridData[i].FolderVersionId = parseInt(GridData[i].FolderVersionId != "" ? GridData[i].FolderVersionId : 0);
        //    GridData[i].FormInstanceId = parseInt(GridData[i].FormInstanceId != "" ? GridData[i].FormInstanceId : 0);
        //    GridData[i].DocumentId = parseInt(GridData[i].DocumentId != "" ? GridData[i].DocumentId : 0);
        //    GridData[i].DocId = parseInt(GridData[i].DocumentId != "" ? GridData[i].DocumentId : 0);
        //    GridData[i].UserActionId = parseInt((getUserActionId(GridData[i].UserAction) != undefined || getUserActionId(GridData[i].UserAction) != null) ? getUserActionId(GridData[i].UserAction) : 5);
        //    delete GridData[i].FormInstanceID;
        //});
        // counter= counter + 1;
        // $.each(matchGridData, function (j) {
        //     GridData[counter].PBPMatchConfig1Up = parseInt(matchGridData[j].PBPMatchConfig1Up);
        //     GridData[counter].IsIncludeInEbs = matchGridData[j].IsIncludeInEbs;
        //     GridData[counter].PBPImportQueueID = parseInt(matchGridData[j].PBPImportQueueID);
        //     GridData[counter].UserActionId = 5;
        //     counter = counter + 1;
        // });
        var data = {
            PBPMatchConfigList: JSON.stringify(GridData)
        };
        var promise = ajaxWrapper.postJSONCustom(URLs.updatePlanConfig, data);
        promise.done(function (result) {
            UpdateMatchPlanConfigList(isCloseConfigWindow);

        });
        //promise.fail(showError);

    }

    UpdateMatchPlanConfigList = function (isCloseConfigWindow) {
        var GridData = $(elementIDs.matchPlanGridJQ).getGridParam('data');
        var GridObj = $(elementIDs.matchPlanGridJQ).getGridParam('data');
        //$.each(GridData, function (j) {
        //    //GridData[j].IsIncludeInEbs = GridObj[j].IsIncludeInEbs == "true" ? true : false;
        //    //GridData[j].DocId = GridObj[j].DocumentId;
        //    //GridData[j].UserActionId = 5;
        //});
        if (GridObj == undefined || GridObj.length == 0) {
            if (isCloseConfigWindow == false) {
                previewConfigrationDialog();
            }
        }
        else {
            var data = {
                PBPMatchConfigList: JSON.stringify(GridObj)
            };
            var promise = ajaxWrapper.postJSONCustom(URLs.updateMatchPlanConfig, data);
            promise.done(function (result) {
                if (result.Result == 0 && isCloseConfigWindow == false) {

                    previewConfigrationDialog();
                }
                else if (result.Result == 2) {
                    //messageDialog.show("PBP import Queued Unsuccessfully..!");
                }
            });
            //promise.fail(showError);
        }
    }

    pBPPlanMatchingConfigrationDialogClose = function () {
        yesNoConfirmDialog.show(Common.closeConfirmationMsg, function (e) {
            if (e) {
                yesNoConfirmDialog.hide();
                $(elementIDs.pBPPlanMatchingConfigrationDialog).dialog("close");
            }
            else {
                yesNoConfirmDialog.hide();
            }
            // UpdatePlanConfig(true);
        });
    }

    generateGrid = function (userActionList, result) {

        $(elementIDs.matchPlanGridJQ).jqGrid('GridUnload');
        $(elementIDs.misMatchPlanGridJQ).jqGrid('GridUnload');
        loadMisMatchPlanGrid(userActionList, result);
        loadMatchPlanGrid(result);
        //-------

    }

    StatusFormmater = function (cellvalue, options, rowObject) {
        if (cellvalue == true) {
            cellvalue = '<span style="color:green" class="glyphicon glyphicon-ok"></span>';
        }
        else {
            cellvalue = '<span style="color:red" class="glyphicon glyphicon-remove"></span>';
        }
        return cellvalue;
    } // processStatusFormmater

    previewConfigrationDialog = function () {
        $(elementIDs.previewPBPConfigrationDialogJQ).dialog({
            autoOpen: false,
            height: "auto",
            width: 1220,
            modal: true,
            autoResize: true
        });
        $(elementIDs.previewPBPConfigrationDialogJQ).dialog('option', 'title', "Preview Import");
        $(elementIDs.previewPBPConfigrationDialogJQ).dialog("open");
        loadPreviewGrid();
    }

    loadPreviewGrid = function () {
        var previewcolArray = null;

        previewData = preparePreviewGridData();
        previewcolArray = ['PBPMatchConfig1Up', '#', 'PBP Plan Number', 'PBP Plan Name', 'eMS Plan Number',
                            'eMS Plan Name', 'User Action', 'User Action', 'Folder Version', 'Include in eMS', 'FolderVersionId',
                            'FormInstanceName', 'DocumentId', 'FolderId', 'FormInstanceId', '', 'IsTerminateVisible'];

        registerPreviewConfigrationDialogEvent();

        //set column models
        var ColModel = [];
        var lastsel2;
        ColModel.push({ key: true, hidden: true, name: 'PBPMatchConfig1Up', index: 'PBPMatchConfig1Up', editable: false, width: '60' });
        ColModel.push({ key: false, hidden: false, name: 'Index', index: 'Index', editable: false, width: '60' });
        ColModel.push({ key: false, name: 'PBPPlanNumber', index: 'PBPPlanNumber', editable: false, width: '150' });
        ColModel.push({ key: false, name: 'PBPPlanName', index: 'PBPPlanName', editable: false, width: '170' });
        GLOBAL.applicationName.toLowerCase() !== 'ebenefitsync' ? ColModel.push({ key: false, name: 'eMsPlanNumber', index: 'eMsPlanNumber', editable: false, width: '150' }) : ColModel.push({ key: false, name: 'eBsPlanNumber', index: 'eBsPlanNumber', editable: false, width: '150' });
        GLOBAL.applicationName.toLowerCase() !== 'ebenefitsync' ? ColModel.push({ key: false, name: 'eMsPlanName', index: 'eMsPlanName', editable: false, edittype: 'select', align: 'left', width: '170' }) : ColModel.push({ key: false, name: 'ebsPlanName', index: 'ebsPlanName', editable: false, edittype: 'select', align: 'left', width: '170' });
        ColModel.push({ key: false, hidden: true, name: 'UserAction', index: 'UserAction', editable: false, formatter: userActionFormatter, align: 'left', width: '170' });
        ColModel.push({ key: false, name: 'UserActionText', index: 'UserActionText', editable: false, align: 'left', width: '170' });
        ColModel.push({ key: false, name: 'FolderVersion', index: 'FolderVersion', editable: false, align: 'left', width: '170' });
        ColModel.push({
            key: false, hidden: true, name: 'IsIncludeInEbs', index: 'IsIncludeInEbs',
            editable: true, edittype: 'checkbox', align: 'center'
        });
        ColModel.push({ key: false, hidden: true, name: 'FolderVersionId', index: 'FolderVersionId', editable: false, align: 'left', width: '170' });
        ColModel.push({ key: false, hidden: true, name: 'FormInstanceName', index: 'FormInstanceName', align: 'right' });
        ColModel.push({ key: false, hidden: true, name: 'DocumentId', index: 'DocumentId', align: 'right', editable: false });
        ColModel.push({ key: false, hidden: true, name: 'FolderId', index: 'FolderId', align: 'right', editable: false });
        ColModel.push({ key: false, hidden: true, name: 'FormInstanceId', index: 'FormInstanceId', align: 'right', editable: false });
        ColModel.push({ key: false, hidden: true, name: 'PBPImportQueueID', index: 'PBPImportQueueID', align: 'right', editable: false });
        ColModel.push({ key: false, hidden: true, name: 'IsTerminateVisible', index: 'IsTerminateVisible', align: 'right', editable: false });
        $(elementIDs.tblPreviewPBPConfigrationJQ).jqGrid('GridUnload');

        //adding the pager element
        $(elementIDs.tblPreviewPBPConfigrationJQ).parent().append("<div id='p" + elementIDs.tblPreviewPBPConfigration + "'></div>");

        $(elementIDs.tblPreviewPBPConfigrationJQ).jqGrid({
            datatype: 'local',
            data: previewData,
            cache: false,
            colNames: previewcolArray,
            colModel: ColModel,
            caption: '',
            height: '200',
            rowNum: 20,
            rowList: [20, 40, 60],
            headertitles: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortname: 'CreatedDate',
            pager: '#p' + elementIDs.tblPreviewPBPConfigration,
            ignoreCase: true,
        });

        var pagerElement = '#p' + elementIDs.tblPreviewPBPConfigration;
        //$('#p' + elementIDs.tblPreviewPBPConfigrationJQ).find('input').css('height', '20px');
        //remove default buttons
        $(elementIDs.tblPreviewPBPConfigrationJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.tblPreviewPBPConfigrationJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

        function userActionFormatter(cellValue, options, rowObject) {
            var action = "";

            switch (String(cellValue)) {
                case '1':
                    action = "Add Plan In eMS";
                    break;
                case '2':
                    action = "Terminate Plan from eMS";
                    break;
                case '3':
                    action = "Map it with another eMS Plan";
                    break;
                case '4':
                    action = "No Action required";
                    break;
                case '5':
                    action = "Update Plan";
                    break;
            }
            return action;
        }
    }

    preparePreviewGridData = function () {
        var index = 0;
        var matchGridData = $(elementIDs.matchPlanGridJQ).getGridParam('data');

        matchGridData = matchGridData.filter(function (obj) {
            if (obj.IsIncludeInEbs == true || obj.IsIncludeInEbs == "true")
                return obj;
        });

        var unMatchGridData = $(elementIDs.misMatchPlanGridJQ).getGridParam('data');
        var Data = matchGridData.concat(unMatchGridData);

        $.each(Data, function (i) {
            index = parseInt(index + 1);
            Data[i].Index = index;
            Data[i].UserAction = Data[i].UserAction;
            Data[i].PBPPlanNumber = Data[i].PlanNumber;
            Data[i].PBPPlanName = Data[i].PlanName;
            Data[i].FolderVersion = Data[i].FolderVersion;
            Data[i].FormInstanceId = Data[i].FormInstanceId;
            Data[i].UserActionText = getUserActionById(Data[i].UserAction);
            //delete Data[i].FormInstanceID;
        });
        return Data;
    }

    getUserActionId = function (str) {
        var userActionId;
        switch (str) {
            case 'Add Plan In eMS':
                userActionId = 1;
                break;
            case 'Terminate Plan from eMS':
                userActionId = 2;
                break;
            case 'Map it with another eMS Plan':
                userActionId = 3;
                break;
            case 'No Action required':
                userActionId = 4;
                break;

            case 'Update Plan':
                userActionId = 5;
        }
        return userActionId;
    }

    getUserActionById = function (useractionid) {
        var userAction;
        useractionid = useractionid == null ? 5 : useractionid;
        switch (parseInt(useractionid)) {
            case 1:
                userAction = 'Add Plan In eMS';
                break;
            case 2:
                userAction = 'Terminate Plan from eMS';
                break;
            case 3:
                userAction = 'Map it with another eMS Plan';
                break;
            case 4:
                userAction = 'No Action required';
                break;

            case 5:
                userAction = 'Update Plan';
        }
        return userAction;
    }

    registerPreviewConfigrationDialogEvent = function () {

        $(elementIDs.btnPreviewCloseJQ).click(function () {
            $(elementIDs.previewPBPConfigrationDialogJQ).dialog("close");
        });

        $(elementIDs.btnProcessWithImportJQ).click(function () {
            yesNoConfirmDialog.show(PBPImportMessages.ConfirmReviewStatusImportMsg, function (e) {
                if (e) {
                    yesNoConfirmDialog.hide();
                    $(elementIDs.pBPPlanMatchingConfigrationDialog).dialog("close");
                    var urlML = URLs.GetCascadeMLProcessingOrQueued;
                    var promiseML = ajaxWrapper.getJSON(urlML);
                    var urlExport = URLs.GetQueuedOrProcessingPBPExport;
                    var promiseExport = ajaxWrapper.getJSON(urlExport);
                    var urlImport = URLs.GetQueuedOrProcessingPBPImport;
                    var promiseImport = ajaxWrapper.getJSON(urlImport);
                    $.when(promiseML, promiseExport, promiseImport).done(function (xhrML, xhrExport, xhrImport) {
                        var ml = xhrML[0];
                        var exp = xhrExport[0];
                        var imprt = xhrImport[0];
                        if (imprt == true) {
                            messageDialog.show("PBP Import is already queued or being Processed . Please try again later.");
                        }
                        else if (ml == true) {
                            messageDialog.show("A Master List Cascade is already queued or being Processed. Please try again later. Visit the Master List Cascade status screen to check for Status.");
                        }
                        else if (exp == true) {
                            messageDialog.show("A PBP Export is already queued or being Processed. Please try again later. Visit the PBP Export Screen to check the status of the Export that is queued or being Processed.");
                        }
                        if (ml == false && exp == false && imprt == false) {
                            savePlanConfigurationDetail();
                        }
                    });
                }
                else {
                    yesNoConfirmDialog.hide();
                }
            });
        });

        $(elementIDs.btnDownloadExcelJQ).click(function () {


            var Str = "", index = 0;
            var GridData = $(elementIDs.tblPreviewPBPConfigrationJQ).jqGrid('getRowData');
            var PbpImportQueueId = 0;
            $.each(GridData, function (i) {
                PbpImportQueueId = GridData[i].PBPImportQueueID;
            });

            var currentInstance = this;
            var jqGridtoCsv = new JQGridtoCsv(elementIDs.tblPreviewPBPConfigrationJQ, false, currentInstance);
            jqGridtoCsv.buildExportOptions();
            var stringData = "csv=" + jqGridtoCsv.csvData;
            stringData += "<&isGroupHeader=" + jqGridtoCsv.isGroupHeader;
            stringData += "<&noOfColInGroup=" + jqGridtoCsv.noofGroupedColumns;
            stringData += "<&repeaterName=" + elementIDs.userListGrid;
            stringData += "<&PbpImportQueueId=" + PbpImportQueueId;
            stringData += "<&PlanType=" + 3;
            $.download(URLs.exportToExcel, stringData, 'post');
        });
    }

    savePlanConfigurationDetail = function () {
        var Str = "", index = 0;
        IsNeededToSave = false;
        var GridData = $(elementIDs.tblPreviewPBPConfigrationJQ).jqGrid('getRowData');
        if (GridData != undefined) {
            if (GridData != null) {
                if (GridData.length > 0) {
                    var data = {
                        ImportID: parseInt(GridData[0].PBPImportQueueID)
                    };
                    var promise = ajaxWrapper.postJSON(URLs.savePlanConfigurationDetail, data);
                    promise.done(function (result) {
                        if (result.Result == 0) {
                            messageDialog.show(PBPImportMessages.PBPQueueImportSuccessfullyMsg);
                            $(elementIDs.previewPBPConfigrationDialogJQ).dialog("close");
                            IsNeededToSave = false;
                            $(elementIDs.queuedPBPImportGridJQ).trigger('reloadGrid');
                            $(elementIDs.pBPPlanMatchingConfigrationDialog).dialog('close');

                        }
                        else if (result.Result == 2) {
                            messageDialog.show(result.Items[0].Messages);
                        }
                    });
                }
            }
        }

    }

    loadMisMatchPlanGrid = function (userActionList, result) {

        var colArray = null, matchcolArray = null;
        misMatchcolArray = ['Id', '#', 'PBP Plan Number', 'PBP Plan Name', 'eMS Plan Number',
                            'eMS Plan Name', 'Data Base Name', 'User Action',
                            'Folder Version', 'FolderVersionId',
                            'FormInstanceName', 'DocumentId', 'FolderId', '',
                            'PBPImportQueueID', 'IsTerminateVisible', 'IsProxyUsed', 'Year'];

        //set column models
        var misMatchColModel = [];
        var lastsel2;
        misMatchColModel.push({ key: true, hidden: true, name: 'PBPMatchConfig1Up', index: 'PBPMatchConfig1Up', align: 'right', editable: false });
        misMatchColModel.push({ key: false, name: 'Index', index: 'Index', editable: false, width: '60' });
        misMatchColModel.push({ key: false, name: 'PlanNumber', index: 'PlanNumber', editable: false, width: '150' });
        misMatchColModel.push({ key: false, name: 'PlanName', index: 'PlanName', editable: false, width: '170' });
        GLOBAL.applicationName.toLowerCase() !== 'ebenefitsync' ? misMatchColModel.push({ key: false, name: 'eMsPlanNumber', index: 'eMsPlanNumber', editable: false, width: '150' }) : misMatchColModel.push({ key: false, name: 'eBsPlanNumber', index: 'eBsPlanNumber', editable: false, width: '150' });
        GLOBAL.applicationName.toLowerCase() !== 'ebenefitsync' ? misMatchColModel.push({ key: false, name: 'eMsPlanName', index: 'eMsPlanName', editable: false, edittype: 'select', align: 'left', width: '170' }) : misMatchColModel.push({ key: false, name: 'eBsPlanName', index: 'eBsPlanName', editable: false, edittype: 'select', align: 'left', width: '170' });
        misMatchColModel.push({ key: false, name: 'DataBaseFileName', index: 'DataBaseFileName', editable: false, align: 'center', width: '120', search: true });
        misMatchColModel.push({
            key: false, name: 'UserAction', index: 'UserAction', editable: true, edittype: "select", unfomatter: selectUnFormat, editoptions: {
                value: userActionList
                , dataEvents: [{ type: 'change', fn: getSelectUserAction }]
            }, align: 'left', width: '145', formatter: 'select'
        });

        //misMatchColModel.push({
        //    key: false, hidden: true, name: 'SelectFolderVersion', index: 'SelectFolderVersion', editable: false,
        //    align: 'center', width: '120'
        //});

        misMatchColModel.push({
            key: false, name: 'FolderVersion', index: 'FolderVersion', editable: false,
            align: 'center', width: '180'
        });

        misMatchColModel.push({ key: false, hidden: true, name: 'FolderVersionId', index: 'FolderVersionId', align: 'right', editable: false });
        misMatchColModel.push({ key: false, hidden: true, name: 'FormInstanceName', index: 'FormInstanceName', align: 'right', editable: false });
        misMatchColModel.push({ key: false, hidden: true, name: 'DocumentId', index: 'DocumentId', align: 'right', editable: false });
        misMatchColModel.push({ key: false, hidden: true, name: 'FolderId', index: 'FolderId', align: 'right', editable: false });
        misMatchColModel.push({ key: false, hidden: true, name: 'FormInstanceId', index: 'FormInstanceId', align: 'right', editable: false });
        misMatchColModel.push({ key: false, hidden: true, name: 'PBPImportQueueID', index: 'PBPImportQueueID', align: 'right', editable: false });
        misMatchColModel.push({ key: false, hidden: true, name: 'IsTerminateVisible', index: 'IsTerminateVisible', align: 'right', editable: false });
        misMatchColModel.push({ key: false, hidden: true, name: 'IsProxyUsed', index: 'IsProxyUsed', align: 'right', editable: false });
        misMatchColModel.push({ key: false, hidden: true, name: 'Year', index: 'Year', align: 'right', editable: false });
        $(elementIDs.misMatchPlanGrid).jqGrid('GridUnload');

        //adding the pager element
        $(elementIDs.misMatchPlanGridJQ).parent().append("<div id='p" + elementIDs.misMatchPlanGrid + "'></div>");

        $(elementIDs.misMatchPlanGridJQ).jqGrid('navButtonAdd', pagerElement,
         {
             caption: '', buttonicon: 'ui-icon-arrowstop-1-s', title: 'Download MisMatch Plan List', id: 'btnMatchPlanGrid',

             onClickButton: function () {
                 var GridData = $(elementIDs.misMatchPlanGridJQ).jqGrid('getRowData');
                 var PbpImportQueueId = 0;
                 $.each(GridData, function (i) {
                     PbpImportQueueId = GridData[i].PBPImportQueueID;
                 });
                 if (PbpImportQueueId > 0) {
                     var currentInstance = this;
                     var jqGridtoCsv = new JQGridtoCsv(elementIDs.misMatchPlanGridJQ, false, currentInstance);
                     jqGridtoCsv.buildExportOptions();
                     var stringData = "csv=" + jqGridtoCsv.csvData;
                     stringData += "<&isGroupHeader=" + jqGridtoCsv.isGroupHeader;
                     stringData += "<&noOfColInGroup=" + jqGridtoCsv.noofGroupedColumns;
                     stringData += "<&repeaterName=" + elementIDs.userListGrid;
                     stringData += "<&PbpImportQueueId=" + PbpImportQueueId;
                     stringData += "<&PlanType=" + 1;
                     $.download(URLs.exportToExcel, stringData, 'post');
                 }
             }
         });

        function getSelectUserAction(e) {
            var element = $(this).attr("id");
            var id = element.split('_');
            var userAction = $("#" + id[0] + "_UserAction").val();

            if (userAction != "" || userAction != "Select" || userAction != undefined) {
                var selectRowData = $(elementIDs.misMatchPlanGridJQ).getRowData(id[0]);
                selectRowData.UserAction = userAction;
                $(elementIDs.misMatchPlanGridJQ).jqGrid('editRow', id[0], true);
                $(elementIDs.misMatchPlanGridJQ).jqGrid("saveRow", id[0], selectRowData);
                getFolderDialog(userAction, selectRowData.Year);
                if (selectRowData.UserAction != "") {
                    removeValidation(id, 7);
                }
                if (selectRowData.UserAction != "1" || selectRowData.UserAction == "3") {
                    var validationmanaer = validationManager();
                    validationmanaer.removeValidation(parseInt(id), 8);
                }
            }
            {
                //alert("Plaese select user Action");
            }
        }

        function selectUnFormat(cellValue, options, rowObject) {
            var selected = $('#' + options.rowId + '_UserAction').val();
            return selected;
        }

        function folderVersionNameSelectUnFormat(cellValue, options, rowObject) {
            var selected = $('#' + options.rowId + '_FolderVersionName').val();
            return selected;
        }

        function getSelectedFolderVersion(e) {
            var element = $(this).attr("id");
            var id = element.split('_');
            var userAction = $("#" + id[0] + "_FolderVersionName").val();
            var selectRowData = $(elementIDs.misMatchPlanGridJQ).getRowData(parseInt(id[0]));
            //selectRowData.folderVersionName = userAction;
            $(elementIDs.misMatchPlanGridJQ).jqGrid("saveRow", id[0], selectRowData);
            $(elementIDs.misMatchPlanGridJQ).editRow(id[0], true);
            $(elementIDs.misMatchPlanGridJQ).saveRow(id[0], true);


        }

        function getFolderDialog(userAction, year) {
            if (userAction != null && userAction != undefined) {
                if (userAction == "3") {
                    var res = viewProductSelectDialog.show(year);
                }
                else if (userAction == "1") {
                    var portfolioSearch = new viewPortfolioSearchDialog();
                    portfolioSearch.show(year);
                }
                clearFields(userAction, false);
            }
        }

        $(elementIDs.misMatchPlanGridJQ).jqGrid({
            datatype: 'local',
            data: result.MisMatchPlanList,
            cache: false,
            colNames: misMatchcolArray,
            colModel: misMatchColModel,
            caption: '',
            height: '100',
            rowNum: 20,
            rowList: [20, 40, 60],
            headertitles: true,
            shrinkToFit: false,
            autowidth: true,
            viewrecords: true,
            sortname: 'CreatedDate',
            ignoreCase: true,
            pager: '#p' + elementIDs.misMatchPlanGrid,
            oneditfunc: function (id) {
                editingRowId = id;
            },
            onSelectRow: function (id) {
                //if (id && id !== lastsel2) {
                //    var rowObject = $(elementIDs.misMatchPlanGridJQ).getRowData(id);
                //    {
                //        $(elementIDs.misMatchPlanGridJQ).saveRow(lastsel2);
                //        $(elementIDs.misMatchPlanGridJQ).editRow(id, true);
                //        lastsel2 = id;
                //    }
                //}

                if (id && id !== lastsel2) {
                    $(elementIDs.misMatchPlanGridJQ).restoreRow(lastsel2);
                    lastsel2 = id;
                }
                $(elementIDs.misMatchPlanGridJQ).editRow(id, true);

                manageUserAction(id);
            },
            onCellSelect: function (rowid, index, contents, event) {
                var cm = $(elementIDs.misMatchPlanGridJQ).jqGrid('getGridParam', 'colModel');
                if (cm[index].name == "IsTerminateVisiable") {
                    ////1074_UserAction
                    //("#" + rowid + "_UserAction option[value='1']").remove();
                    //("#" + rowid + "_UserAction option[value='3']").remove();
                    //("#" + rowid + "_UserAction option[value='5']").remove();
                }
            }
        });

        var pagerElement = '#p' + elementIDs.misMatchPlanGrid;
        //$('#p' + elementIDs.misMatchPlanGrid).find('input').css('height', '20px');
        //remove default buttons
        $(elementIDs.misMatchPlanGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.misMatchPlanGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

        $(elementIDs.misMatchPlanGridJQ).jqGrid('navButtonAdd', pagerElement,
      {
          caption: '', buttonicon: 'ui-icon-arrowstop-1-s', title: 'Download To Mismatch Plan List', id: 'btmisMatchPlanGrid',

          onClickButton: function () {
              var GridData = $(elementIDs.misMatchPlanGridJQ).jqGrid('getRowData');
              var PbpImportQueueId = 0;
              $.each(GridData, function (i) {
                  PbpImportQueueId = GridData[i].PBPImportQueueID;
              });
              if (PbpImportQueueId > 0) {
                  var currentInstance = this;
                  var jqGridtoCsv = new JQGridtoCsv(elementIDs.misMatchPlanGridJQ, false, currentInstance);
                  jqGridtoCsv.buildExportOptions();
                  var stringData = "csv=" + jqGridtoCsv.csvData;
                  stringData += "<&isGroupHeader=" + jqGridtoCsv.isGroupHeader;
                  stringData += "<&noOfColInGroup=" + jqGridtoCsv.noofGroupedColumns;
                  stringData += "<&repeaterName=" + elementIDs.userListGrid;
                  stringData += "<&PbpImportQueueId=" + PbpImportQueueId;
                  stringData += "<&PlanType=" + 1;
                  $.download(URLs.exportToExcel, stringData, 'post');
              }
          }
      });
    }

    loadMatchPlanGrid = function (result) {
        matchcolArray = ['Id', '#', 'PBP Plan Number', 'PBP Plan Name', 'eMS Plan Number',
                        'eMS Plan Name', 'Data Base Name', 'Include in eMS', 'Folder Version',
                        'FolderVersionId', '', '', '', '', ''];
        var matchColModel = [];
        matchColModel.push({ key: false, hidden: true, name: 'PBPMatchConfig1Up', index: 'PBPMatchConfig1Up', align: 'right', editable: true });
        matchColModel.push({ key: true, name: 'Index', index: 'Index', editable: false, width: '60' });
        matchColModel.push({ key: false, name: 'PlanNumber', index: 'PlanNumber', editable: false, width: '150' });
        matchColModel.push({ key: false, name: 'PlanName', index: 'PlanName', editable: false, width: '170' });
        GLOBAL.applicationName.toLowerCase() !== 'ebenefitsync' ? matchColModel.push({ key: false, name: 'eMsPlanNumber', index: 'eMsPlanNumber', editable: false, width: '150' }) : matchColModel.push({ key: false, name: 'eBsPlanNumber', index: 'eBsPlanNumber', editable: false, width: '150' });
        GLOBAL.applicationName.toLowerCase() !== 'ebenefitsync' ? matchColModel.push({ key: false, name: 'eMsPlanName', index: 'eMsPlanName', editable: true, edittype: 'select', align: 'left', width: '170' }) : matchColModel.push({ key: false, name: 'ebsPlanName', index: 'ebsPlanName', editable: true, edittype: 'select', align: 'left', width: '170' });
        matchColModel.push({ key: false, name: 'DataBaseFileName', index: 'DataBaseFileName', align: 'center', width: '120', editable: false, search: true });
        //matchColModel.push({
        //    key: false, name: 'IsIncludeInEbs', index: 'IsIncludeInEbs', editable: true,
        //    formatter: CheckBoxFormatter,
        //    align: 'center', width: '150'
        //});

        matchColModel.push({
            key: false, name: 'IsIncludeInEbs', index: 'IsIncludeInEbs', editable: true,
            formatoptions: { disabled: false, }, align: 'center', sortable: false, formatter: CheckBoxFormatter
        });

        matchColModel.push({ key: false, name: 'FolderVersion', index: 'FolderVersion', editable: false, align: 'left', width: '180' });
        //matchColModel.push({ key: true, hidden: true, name: 'FolderVersionId', index: 'FolderVersionId', align: 'right', editable: false });
        matchColModel.push({ key: false, hidden: true, name: 'UserAction', index: 'UserAction', align: 'right', editable: false, unfomatter: selectUnFormat });
        matchColModel.push({ key: false, hidden: true, name: 'PBPImportQueueID', index: 'PBPImportQueueID', align: 'right', editable: false });

        matchColModel.push({ key: false, hidden: true, name: 'FolderVersionId', index: 'FolderVersionId', align: 'right', editable: false });
        matchColModel.push({ key: false, hidden: true, name: 'DocumentId', index: 'DocumentId', align: 'right', editable: false });
        matchColModel.push({ key: false, hidden: true, name: 'FolderId', index: 'FolderId', align: 'right', editable: false });
        matchColModel.push({ key: false, hidden: true, name: 'FormInstanceId', index: 'FormInstanceId', align: 'right', editable: false });

        $(elementIDs.matchPlanGrid).jqGrid('GridUnload');

        //adding the pager element
        $(elementIDs.matchPlanGridJQ).parent().append("<div id='p" + elementIDs.matchPlanGrid + "'></div>");

        $(elementIDs.matchPlanGridJQ).jqGrid({
            datatype: 'local',
            data: result.MatchPlanList,
            loadonce: true,
            cache: false,
            colNames: matchcolArray,
            colModel: matchColModel,
            caption: '',
            height: '100',
            rowNum: 20,
            rowList: [20, 40, 60],
            headertitles: true,
            shrinkToFit: false,
            autowidth: true,
            viewrecords: true,
            sortname: 'CreatedDate',
            ignoreCase: true,
            pager: '#p' + elementIDs.matchPlanGrid,
            beforeSelectRow: function (rowid, e) {
                var $self = $(this),
                    iCol = $.jgrid.getCellIndex($(e.target).closest("td")[0]),
                    cm = $self.jqGrid("getGridParam", "colModel"),
                    localData = $self.jqGrid("getLocalRow", rowid);
                if (cm[iCol].name === "IsIncludeInEbs" && e.target.tagName.toUpperCase() === "INPUT") {
                    // set local grid data
                    localData.IsIncludeInEbs = $(e.target).is(":checked");
                    //alert($(e.target).is(":checked"));
                }

                return true; // allow selection
            }
        });

        function selectUnFormat(cellValue, options, rowObject) {
            var selected = $('#' + options.rowId + '_UserAction').val();
            return selected;
        }

        function CheckBoxFormatter(cellValue, options, rowObject) {
            var result;
            var Disabled = "";
            if (rowObject.IsDisableIsIncludeIneBsFlag == true) {
                Disabled = "Disabled";
            }
            switch (options.colModel.index) {
                case 'IsIncludeInEbs':
                    var resultId = "IsIncludeInEbs" + rowObject.PBPMatchConfig1Up;
                    if (cellValue) {
                        if (rowObject.IsDisableIsIncludeIneBsFlag == false) {
                            result = "<input type='checkbox' id='" + resultId + "' checked='" + cellValue + "' " + Disabled + "/>";
                        }
                        else {
                            result = "<input type='checkbox' id='" + resultId + "' unchecked " + Disabled + "/>";
                        }
                    }
                    else {
                        result = "<input type='checkbox' id='" + resultId + "' unchecked " + Disabled + "/>";
                    }
                    break;
                default:
                    result = cellValue;
                    break;
            }
            return result;
        }

        var pagerElement = '#p' + elementIDs.matchPlanGrid;
        //$('#p' + elementIDs.matchPlanGrid).find('input').css('height', '20px');
        //remove default buttons
        $(elementIDs.matchPlanGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.matchPlanGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

        $(elementIDs.matchPlanGridJQ).jqGrid('navButtonAdd', pagerElement,
       {
           caption: '', buttonicon: 'ui-icon-arrowstop-1-s', title: 'Download Match Plan List', id: 'btnmatchPlanGrid',

           onClickButton: function () {
               var GridData = $(elementIDs.matchPlanGridJQ).jqGrid('getRowData');
               var PbpImportQueueId = 0;
               $.each(GridData, function (i) {
                   PbpImportQueueId = GridData[i].PBPImportQueueID;
               });
               if (PbpImportQueueId > 0) {
                   var currentInstance = this;
                   var jqGridtoCsv = new JQGridtoCsv(elementIDs.matchPlanGridJQ, false, currentInstance);
                   jqGridtoCsv.buildExportOptions();
                   var stringData = "csv=" + jqGridtoCsv.csvData;
                   stringData += "<&isGroupHeader=" + jqGridtoCsv.isGroupHeader;
                   stringData += "<&noOfColInGroup=" + jqGridtoCsv.noofGroupedColumns;
                   stringData += "<&repeaterName=" + elementIDs.userListGrid;
                   stringData += "<&PbpImportQueueId=" + PbpImportQueueId;
                   stringData += "<&PlanType=" + 2;
                   $.download(URLs.exportToExcel, stringData, 'post');
               }
           }
       });
    }

    loadUserAction = function (result) {
        var promise = ajaxWrapper.getJSON(URLs.getUserActionList);
        //fill the folder list drop down
        promise.done(function (list) {
            var userActionList = "0:Select;";
            if (list.length > 0) {
                jQuery.each(list, function (index, item) {
                    userActionList += item.PBPUserActionID + ":" + item.UserAction + ";";
                });
            }
            userActionList = userActionList.substr(0, userActionList.length - 1);
            generateGrid(userActionList, result);
        });

        //promise.fail(showError);
    }

    parseIntiger = function (value) {
        try {
            return parseInt(value);
        }
        catch (ex) {
            return 0;
        }
    }

    clearFields = function (userAction, isclose) {
        var selectedRowId = $(elementIDs.misMatchPlanGridJQ).jqGrid('getGridParam', 'selrow');
        if (selectedRowId != null && selectedRowId != undefined) {
            var selectRowData = $(elementIDs.misMatchPlanGridJQ).getRowData(selectedRowId);

            resetIsProxyUsed(selectRowData.FormInstanceId);

            if (selectRowData.IsTerminateVisible == "false") {
                selectRowData.FolderId = "0";
                selectRowData.FolderVersionId = "0";
                selectRowData.FolderVersionName = "";
                selectRowData.FormInstanceId = "0";
                selectRowData.DocumentId = "0";
                selectRowData.FormInstanceName = "";
                selectRowData.FolderVersion = "";
                selectRowData.eBsPlanNumber = "";
                selectRowData.ebsPlanName = "";
                $(elementIDs.misMatchPlanGridJQ).jqGrid('editRow', selectedRowId, true);
                $(elementIDs.misMatchPlanGridJQ).jqGrid('setRowData', selectedRowId, selectRowData);
                $(elementIDs.misMatchPlanGridJQ).jqGrid("saveRow", selectedRowId, selectRowData);
            }
        }
    }

    validateGridData = function () {
        var errorCode = 0;
        var formattedset = new Set();
        var ErroredUserActionList = [], ErroredFolderList = [], ErroredProductDuplicateList = [], ErroredProductList = [];
        // var errorcodelist=[];
        var GridData = $(elementIDs.misMatchPlanGridJQ).getGridParam('data');
        var allRowsOnCurrentPage = $(elementIDs.misMatchPlanGridJQ).jqGrid('getDataIDs');
        for (var i = 0; i < GridData.length; i++) {
            GridData[i].RowNumber = allRowsOnCurrentPage[i];
            if ((GridData[i].UserAction == '') || (GridData[i].UserAction == '0') || (GridData[i].UserAction == undefined)) {
                applyValidation(GridData[i].RowNumber, 7);
                ErroredUserActionList.push(GridData[i].Index);
                //messageDialog.show("Select User Action for " +/*errorMsgFormatter(ErrorRow)*/ ErrorRow + " row");
                errorCode = 1;
            }
            else {
                removeValidation(GridData[i].RowNumber, 7);
            }
            if ((GridData[i].UserAction == '1' || GridData[i].UserAction == '3') && (GridData[i].FolderId == '0' || GridData[i].FolderId == "")) {
                //messageDialog.show("Select Folder for " + /*errorMsgFormatter(ErrorRow)*/ ErrorRow + " row");
                errorCode = 1;
                applyValidation(GridData[i].RowNumber, 8);
                ErroredFolderList.push(GridData[i].Index);
            }
            else {
                removeValidation(GridData[i].RowNumber, 8);

            }
            if ((GridData[i].UserAction == '3') && (GridData[i].FolderId != '0' || GridData[i].FolderId == "")) {
                if (formattedset.has(parseInt(GridData[i].FormInstanceId))) {
                    applyValidation(GridData[i].RowNumber, 8);
                    //messageDialog.show("Product can not be duplicated for " + /*errorMsgFormatter(ErrorRow)*/ ErrorRow + " row");
                    errorCode = 1;
                    ErroredProductDuplicateList.push(GridData[i].Index);
                }
                else {
                    formattedset.add(parseInt(GridData[i].FormInstanceId));
                    if (parseInt(GridData[i].FormInstanceId) > 0) {
                        removeValidation(GridData[i].RowNumber, 8);
                    }
                    else {
                        applyValidation(GridData[i].RowNumber, 8);
                        ErroredProductList.push(GridData[i].Index);
                    }
                }
            }

            //if (errorCode > 0)
            //    break;
        }
        errorMessageBuilder(ErroredUserActionList, ErroredFolderList, ErroredProductDuplicateList, ErroredProductDuplicateList);
        return errorCode;
    }

    applyValidation = function (rowid, column) {
        $("#" + rowid).children("td:eq(" + column + ")").attr("style", "border:1px solid red !important");
    }

    removeValidation = function (rowid, column) {
        $("#" + rowid).children("td:eq(" + column + ")").attr("style", "0");
    }

    errorMessageBuilder = function (ErroredUserActionList, ErroredFolderList, ErroredProductDuplicateList, ErroredProductList) {
        var errorMessage = "";
        var errorMsgUserAction = "Select User Action for" + '</br>';
        var errorMsgFolder = '</br></br>' + "Select Folder for " + '</br>';
        var errorMsgProductDuplicate = '</br></br>' + "Product can not be duplicated for " + '</br>'
        var errorMsgSelectProduct = "</br></br>Select Product for " + '</br>';
        var errorMsg1 = "", errorMsg2 = "", errorMsg3 = "", errorMsg4 = "", errorMsg1 = "";

        $.each(ErroredUserActionList, function (index, value) {
            errorMsg1 += ordinalSuffixGenrator(parseInt(value)).toString();
        });
        if (ErroredUserActionList.length > 0) {
            errorMsg1 = errorMsg1.substring(0, errorMsg1.length - 2);
        }


        $.each(ErroredFolderList, function (index, value) {
            errorMsg2 += ordinalSuffixGenrator(parseInt(value)).toString();
        });
        if (ErroredFolderList.length > 0) {
            errorMsg2 = errorMsg2.substring(0, errorMsg2.length - 2);
        }

        $.each(ErroredProductDuplicateList, function (index, value) {
            errorMsg3 += ordinalSuffixGenrator(parseInt(value)).toString();
        });
        if (ErroredProductDuplicateList.length > 0) {
            errorMsg3 = errorMsg3.substring(0, errorMsg3.length - 2);
        }


        $.each(ErroredProductList, function (index, value) {
            errorMsg4 += ordinalSuffixGenrator(parseInt(value)).toString();
        });
        if (ErroredProductList.length > 0) {
            errorMsg4 = errorMsg4.substring(0, errorMsg4.length - 2);
        }

        if (errorMsg1 != "") {
            errorMessage += errorMsgUserAction + errorMsg1 + " row(s)";
        }

        if (errorMsg2 != "") {
            errorMessage += errorMsgFolder + errorMsg2 + " row(s)";
        }

        if (errorMsg3 != "") {
            errorMessage += errorMsgProductDuplicate + errorMsg3 + " row(s)";
        }

        if (errorMsg4 != "") {
            errorMessage += errorMsgSelectProduct + errorMsg4 + " row(s)";
        }

        if (errorMessage != "") {
            messageDialog.show(errorMessage);
        }
    }

    ordinalSuffixGenrator = function (i) {
        var j = i % 10,
         k = i % 100;
        if (j == 1 && k != 11) {
            return i + "st, ";
        }
        if (j == 2 && k != 12) {
            return i + "nd, ";
        }
        if (j == 3 && k != 13) {
            return i + "rd, ";
        }
        return i + "th, ";
    }

    manageUserAction = function (id) {
        var sourceRowData = $(elementIDs.misMatchPlanGridJQ).jqGrid('getRowData', id);
        if (sourceRowData.IsTerminateVisible == "true") {
            $("#" + id + "_UserAction option[value='1']").remove();
            $("#" + id + "_UserAction option[value='3']").remove();
            $("#" + id + "_UserAction option[value='5']").remove();
        }
        else {
            $("#" + id + "_UserAction option[value='2']").remove();
        }
        if (sourceRowData.IsProxyUsed == "true") {
            $("#" + id + "_UserAction option[value='2']").remove();
        }
    }

    resetIsProxyUsed = function (forminstanceId) {
        var MisMatchgridAllData = $(elementIDs.misMatchPlanGridJQ).getGridParam('data');

        if (MisMatchgridAllData != null) {
            SetData = MisMatchgridAllData.filter(function (obj) {
                if ((obj.FormInstanceId == forminstanceId && obj.IsTerminateVisible == "true") || obj.FormInstanceId == parseInt(forminstanceId) && obj.IsTerminateVisible == true)
                    return obj;
            });
            if (SetData != undefined) {
                if (SetData != null) {
                    if (SetData.length > 0) {
                        $(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', SetData[0].PBPMatchConfig1Up, 'IsProxyUsed', false);
                        //$(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', SetData[0].PBPMatchConfig1Up, 'UserAction', "5");

                        var rowData = $(elementIDs.misMatchPlanGridJQ).jqGrid('getRowData', SetData[0].PBPMatchConfig1Up);
                        //rowData.UserAction = '4';
                        $(elementIDs.misMatchPlanGridJQ).jqGrid('setRowData', SetData[0].PBPMatchConfig1Up, rowData);
                    }
                }
            }
        }
    }
    init();

    return {
        show: function (result) {
            $(elementIDs.pBPPlanMatchingConfigrationDialog).dialog('option', 'title', "PBP-eMS Plan Match Confirmation");
            $(elementIDs.pBPPlanMatchingConfigrationDialog).dialog("open");
            //generateGrid();
            loadUserAction(result);
        },
        DialogInitConfig: function () {
            //getFolderVersion();
        },
        ClearFields: function () {
            clearFields(undefined, true);

        }
    }
}

var viewProductSelectDialog = function () {
    var URLs = {
        docSearch: '/ConsumerAccount/GetPortfolioFoldersDocumentsList?tenantId=1'
    };
    var elementIDs = {
        //id for dialog div
        sourceDialogJQ: "#sourceDocumentDialog",
        docSearchGrid: "docSearch",
        docSearchGridJS: "#docSearch",
        docSearchGridPagerJQ: "#pdocSearch",
        misMatchPlanGridJQ: "#misMatchPlanGrid",
    }

    function init() {
        //register dialog 
        $(elementIDs.sourceDialogJQ).dialog({
            autoOpen: false,
            height: 510,
            width: 700,
            modal: true,
            open: function (event, ui) {
                $(elementIDs.sourceDialogJQ).css('overflow', 'hidden'); //this line does the actual hiding
            },
            //close: function (event, ui) {
            //    var Dialog = new pBPPlanMatchingConfigrationDialog();
            //    Dialog.ClearFields(undefined,true);
            //}
        });
    }
    //initialize the dialog after this js is loaded
    init();

    function loadSelectGrid(year) {
        var currentInstance = this;
        var colArray = ['Portfolio Name', 'Folder Version Number', 'PBP Plan Name', 'PBP Plan Number',
            'Effective Date', 'Document Name', 'eMS Plan Number', 'eMS Plan Name', '', '', '', '', '', 'DocumentId', ];

        var colModel = [];
        colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left' });
        colModel.push({ name: 'VersionNumber', index: 'VersionNumber', editable: false, align: 'left', width: 200 });
        colModel.push({ name: 'PBPPlanName', index: 'PBPPlanName', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'PBPPlanNumber', index: 'PBPPlanNumber', editable: false, align: 'left', hidden: true });
        colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' });
        colModel.push({ name: 'FormInstanceName', index: 'FormInstanceName', editable: false, align: 'left' });
        colModel.push({ name: 'eBsPlanNumber', index: 'eBsPlanNumber', editable: false, align: 'left' });
        colModel.push({ name: 'eBsPlanName', index: 'eBsPlanName', editable: false, align: 'left', hidden: false });
        colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true });
        colModel.push({ name: 'AccountID', index: 'AccountID', hidden: true });
        colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true });
        colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', hidden: true });
        colModel.push({ name: 'FormInstanceID', index: 'FormInstanceID', hidden: true });
        colModel.push({ name: 'DocumentId', index: 'DocumentId', hidden: true });
        $(elementIDs.docSearchGridJS).jqGrid('GridUnload');
        $(elementIDs.docSearchGridJS).parent().append("<div id='p" + elementIDs.docSearchGrid + "'></div>");
        var url = URLs.docSearch;
        $(elementIDs.docSearchGridJS).jqGrid({
            url: url + "&year=" + year,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Plan Mapping',
            height: '300',
            rowNum: 20,
            rowList: [10, 20, 30],
            ignoreCase: true,
            autowidth: true,
            viewrecords: true,
            headertitles: true,
            hidegrid: false,
            altRows: true,
            pager: elementIDs.docSearchGridPagerJQ,
            sortname: 'FormInstanceName',
            altclass: 'alternate',
            ondblClickRow: function () {
                setDocumentSelection();
            },
            gridComplete: function () {
                //checkPBPImportClaims(elementIDs, claims);
            }
        });
        var pagerElement = elementIDs.docSearchGridPagerJQ;
        //remove default buttons
        $(elementIDs.docSearchGridJS).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.docSearchGridJS).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
        $(elementIDs.docSearchGridJS).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-check', title: 'Select',
               onClickButton: function () {
                   setSelectedDocumentValuesToTarget();
               }
           });
    }

    function setSelectedDocumentValuesToTarget() {
        var targetSelectedRowId = $(elementIDs.misMatchPlanGridJQ).jqGrid('getGridParam', 'selrow');
        var sourceSelectedRowId = $(elementIDs.docSearchGridJS).jqGrid('getGridParam', 'selrow');
        if (targetSelectedRowId != null && targetSelectedRowId != undefined) {
            if (sourceSelectedRowId != null && sourceSelectedRowId != undefined) {
                var sourceRowData = $(elementIDs.docSearchGridJS).jqGrid('getRowData', sourceSelectedRowId);
                $(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', targetSelectedRowId, 'FolderId', sourceRowData.FolderID);
                $(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', targetSelectedRowId, 'FolderVersionId', sourceRowData.FolderVersionID);
                $(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', targetSelectedRowId, 'FolderVersion', sourceRowData.FolderName + "_" + sourceRowData.VersionNumber + "_" + sourceRowData.FormInstanceName);
                $(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', targetSelectedRowId, 'FormInstanceName', sourceRowData.FormInstanceName);
                $(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', targetSelectedRowId, 'DocumentId', sourceRowData.DocumentId);
                //$(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', selectedRowId, 'FormInstanceID', $(elementIDs.FormInstanceIdJQ).val());
                $(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', targetSelectedRowId, 'FormInstanceId', sourceRowData.FormInstanceID);
                $(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', targetSelectedRowId, 'eBsPlanNumber', sourceRowData.eBsPlanNumber);
                $(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', targetSelectedRowId, 'ebsPlanName', sourceRowData.eBsPlanName);
                setIsProxyUsed(sourceRowData.FormInstanceID);
                $(elementIDs.sourceDialogJQ).dialog("close");
                var validationmanaer = validationManager();
                validationmanaer.removeValidation(parseInt(targetSelectedRowId), 8);
            }
            else {
                messageDialog.show(Common.pleaseSelectRowMsg);
            }
        }
        else {
            messageDialog.show(Common.pleaseSelectRowMsg);
        }
    }

    setIsProxyUsed = function (forminstanceId) {
        var MisMatchgridAllData = $(elementIDs.misMatchPlanGridJQ).getGridParam('data');

        if (MisMatchgridAllData != null) {
            SetData = MisMatchgridAllData.filter(function (obj) {
                if ((obj.IsTerminateVisible == true && obj.FormInstanceId == parseInt(forminstanceId)) || (obj.IsTerminateVisible == "true" && obj.FormInstanceId == forminstanceId))
                    return obj;
            });
            if (SetData != undefined) {
                if (SetData != null) {
                    if (SetData.length > 0) {
                        //$(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', SetData[0].PBPMatchConfig1Up, 'IsProxyUsed', "true");
                        //$(elementIDs.misMatchPlanGridJQ).jqGrid('setCell', SetData[0].PBPMatchConfig1Up, 'UserAction', "5");
                        var rowData = $(elementIDs.misMatchPlanGridJQ).jqGrid('getRowData', SetData[0].PBPMatchConfig1Up);
                        rowData.UserAction = '4';
                        rowData.IsProxyUsed = 'true';
                        $(elementIDs.misMatchPlanGridJQ).jqGrid('setRowData', SetData[0].PBPMatchConfig1Up, rowData);
                    }
                }
            }
        }
    }

    validate = function () {
        var selectedRowId = $(elementIDs.docSearchGridJS).jqGrid('getGridParam', 'selrow');
        try {
            if (selectedRowId != undefined) {
                parseInt(selectedRowId);
                return true;
            }
            else {
                return false;
            }
        }
        catch (ex) {

        }
    }

    return {
        show: function (year) {
            $(elementIDs.sourceDialogJQ).dialog('option', 'title', "Plan Mapping");
            $(elementIDs.sourceDialogJQ).dialog("open");
            loadSelectGrid(year);
        }
    }
}();

var discardImport = function () {

    var URLs = {
        DiscardChanges: "/PBPImport/DiscardChanges"
    };

    var elementIDs = {
        pBPPlanMatchingConfigrationDialog: "#pBPPlanMatchingConfigrationDialog",
        previewPBPConfigrationDialogJQ: "#previewPBPConfigrationDialog",
    };
    discard = function (pbpImportId, fromdialog) {
        var postData = {
            "PBPImportQueueID": pbpImportId
        };

        yesNoConfirmDialog.show(PBPImportMessages.DiscardChanges, function (e) {
            if (e) {
                var promise = ajaxWrapper.postJSON(URLs.DiscardChanges, postData);
                promise.done(function (result) {
                    if (result.Result == 0) {
                        yesNoConfirmDialog.hide();
                        if (fromdialog == 1) {
                            $(elementIDs.pBPPlanMatchingConfigrationDialog).dialog("close");
                        } else if (fromdialog == 2) {
                            $(elementIDs.previewPBPConfigrationDialogJQ).dialog("close");
                            $(elementIDs.pBPPlanMatchingConfigrationDialog).dialog("close");
                        }
                    }
                });
            }
            else {
                yesNoConfirmDialog.hide();
            }
            // UpdatePlanConfig(true);
        });
    }
    return {
        Discard: function (pbpImportId, fromdialog) {
            discard(pbpImportId, fromdialog);
        }
    }
}

var validationManager = function () {
    applyValidation = function (rowid, column) {
        $("#" + rowid).children("td:eq(" + column + ")").attr("style", "border:1px solid red !important");
    }

    removeValidation = function (rowid, column) {
        $("#" + rowid).children("td:eq(" + column + ")").attr("style", "0");
    }
    return {
        applyValidation: function (rowid, column) {
            applyValidation(rowid, column);
        },
        removeValidation: function (rowid, column) {
            removeValidation(rowid, column);
        }
    }
}

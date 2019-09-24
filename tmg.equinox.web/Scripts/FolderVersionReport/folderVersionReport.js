
var ChangeSummaryForm = function () {

    var isInitialized = false;
    var source = '';
    var isFound = false;
    var result = '';
    var targetDataList = '';
    var sourceFolderData = '';
    var targetFolderData = '';

    //urls to be accessed for create/copy form.
    var URLs = {
        //get list of documents
        getFolderFormList: '/FolderVersionReport/GetTargetFormInstanceList?tenantId=1&folderVersionId={foldersVersionId}',
        //get list of folder version 
        getFolderVersionList: '/FolderVersionReport/GetFolderVersionList?tenantId=1&accountType={accountType}&folderId={folderId}',
        //get name of folder name
        getFolderNameList: '/FolderVersionReport/GetFolderList?tenantId=1&accountType={accountType}',
        //load jqgrid 
        getLoadFormList: '/FolderVersionReport/GetSourceFormInstanceList?tenantId=1&sourceFolderVersionId={sourceFolderVersionId}',

        exportChangeSummaryReport: '/FolderVersionReport/ExportChangeSummaryReport',
    };


    //added in Views/FolderVersionReport/FolderVersionReport.cshtml
    var elementIDs = {
        // table element of changesummartGrid      
        changeSummaryDiv: '#changeSummaryDiv',
        //element of an Account or Portfolio dropdown list
        sourceAccountDDLJQ: '#sourceAccountDDL',
        //element of an Account or Portfolio dropdown list
        targetAccountDDLJQ: '#targetAccountDDL',
        //element of folder name dropdownlist
        sourceFolderNameDDLJQ: '#sourceFolderNameDDL',
        //element of folder name dropdownlist
        targetFolderNameDDLJQ: '#targetFolderNameDDL',
        //element of folderversion list 
        sourceFolderVersionDDLJQ: '#sourceFolderVersionDDL',
        //element of folderversion list 
        targetFolderVersionDDLJQ: '#targetFolderVersionDDL',
        //element of jqgrid 
        documentChangeSummaryGrid: 'documentChangeSummary',
        //with hash for use with jQuery
        documentChangeSummaryGridJQ: '#documentChangeSummary',
        sourceAccountDDLHelpBlockJQ: '#sourceAccountDDLHelpBlock',
        targetAccountDDLHelpBlockJQ: '#targetAccountDDLHelpBlock',
        sourceFolderNameDDLHelpBlockJQ: '#sourceFolderNameDDLHelpBlock',
        targetFolderNameDDLHelpBlockJQ: '#targetFolderNameDDLHelpBlock',
        sourceFolderVersionDDLHelpBlockJQ: '#sourceFolderVersionDDLHelpBlock',
        targetFolderVersionDDLHelpBlockJQ: '#targetFolderVersionDDLHelpBlock',

        //element of button to view report
        btnViewReportFormJQ: '#btnViewReportForm',
        btnCancelJQ: '#btnCancel',
        sourceFolderNameAutoComplDDLJQ: '#sourceFolderNameAutoComplDDL',
        targetFolderNameAutoComplDDLJQ: '#targetFolderNameAutoComplDDL', 
    };
    //this function is called below soon after this JS file is loaded
    function init() {
        $(document).ready(function () {
            if (isInitialized == false) {
                initilizeControls();
                //// add on change event for  folder drop down
              //  $(elementIDs.sourceAccountDDLJQ).change(function (e) { return fillSourceFolderDDL($(this).val()); });
              //  $(elementIDs.targetAccountDDLJQ).change(function (e) { return fillTargetFolderDDL($(this).val()); });
                //// add on change event for  folder drop down
               
                //Auto complete Start
                $(function () {
                    $(elementIDs.sourceFolderNameDDLJQ).autoCompleteDropDown({ value: ChangeSummaryForm.fillSourceDropdown, ID: 'sourceFolderNameAutoComplDDL' });
                 });
               
                //Auto complete Start
                $(function () {
                    $(elementIDs.targetFolderNameDDLJQ).autoCompleteDropDown({ value: ChangeSummaryForm.fillTargetDropDown, ID: 'targetFolderNameAutoComplDDL' });                    
                });
                //// add on change event for form drop down
                $(elementIDs.sourceFolderVersionDDLJQ).change(function (e) {
                    $(elementIDs.sourceFolderVersionDDLHelpBlockJQ).text('');
                    $(elementIDs.sourceFolderVersionDDLJQ).parent().removeClass('has-error');
                    //check the both drop down has to selected
                    dropDownfunctionValidate();
                });

                $(elementIDs.targetFolderVersionDDLJQ).change(function (e) {
                    $(elementIDs.targetFolderVersionDDLHelpBlockJQ).text('');
                    $(elementIDs.targetFolderVersionDDLJQ).parent().removeClass('has-error');
                    //check the both drop down has to selected
                    dropDownfunctionValidate();
                });
                //// add click event for Done button
                $(elementIDs.btnViewReportFormJQ).click(function (e) { return saveChangeSummery(); });
                $(elementIDs.btnCancelJQ).click(function (e) { return clearAll(); });
                isInitialized = true;
            }
        });

    }
    //set default setting
    function initilizeControls() {
        $(elementIDs.btnViewReportFormJQ).attr('disabled', 'disabled');
        $(elementIDs.sourceAccountDDLJQ).removeAttr("disabled");
        $(elementIDs.targetAccountDDLJQ).removeAttr("disabled");
        fillSourceFolderDDL(2);
        fillTargetFolderDDL(2);
    }
    //fill folder drop down depending upon account type selected
    function fillSourceFolderDDL(accountType) {

        $(elementIDs.sourceAccountDDLHelpBlockJQ).text('');
        $(elementIDs.sourceAccountDDLHelpBlockJQ).removeClass('form-control');
        $(elementIDs.sourceAccountDDLJQ).parent().removeClass('has-error');
        //enable folder drop down list.
        $(elementIDs.sourceFolderNameDDLJQ).removeAttr('disabled');
        //empty the folder drop down list
        $(elementIDs.sourceFolderNameDDLJQ).empty();
        $(elementIDs.sourceFolderNameDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");
        $(elementIDs.sourceFolderNameDDLJQ).val("");
        //$(elementIDs.sourceFolderNameDDLJQ).append("<option value='0'>" + "" + "</option>");
        $(elementIDs.sourceFolderVersionDDLJQ).empty();
        $(elementIDs.sourceFolderVersionDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");        
        if (accountType != "0") {
            //ajax call for drop down list of folder list.       
            var url = URLs.getFolderNameList.replace(/\{accountType\}/g, accountType);
            var promise = ajaxWrapper.getJSON(url);
            //fill the folder list drop down
            promise.done(function (list) {
                for (i = 0; i < list.length; i++) {
                    if (list[i].AccountName != null) {
                        $(elementIDs.sourceFolderNameDDLJQ).append("<option value=" + list[i].FolderId + ">" + list[i].AccountName + "  - " + list[i].FolderName + "</option>");
                    }
                    else {
                        $(elementIDs.sourceFolderNameDDLJQ).append("<option value=" + list[i].FolderId + ">" + list[i].FolderName + "</option>");
                    }
                    sourceFolderData = list;
                }
            });

            promise.fail(showError);
        }
        else {
            // disable the folder and form ddl.
            $(elementIDs.sourceFolderNameDDLJQ).attr('disabled', 'disabled');
            //disable the folder version drop down list.
            $(elementIDs.sourceFolderVersionDDLJQ).attr('disabled', 'disabled');
            //empty the folder version drop down list
            $(elementIDs.sourceFolderVersionDDLJQ).empty();
            $(elementIDs.sourceFolderVersionDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");            
            //disable the form instance drop down list.
            $(elementIDs.sourceFormInstanceDDLJQ).attr('disabled', 'disabled');
            //empty the form instance drop down list
            $(elementIDs.sourceFormInstanceDDLJQ).empty();
            //$(elementIDs.sourceFormInstanceDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");
            $(elementIDs.sourceFormInstanceDDLJQ).append("<option value='0'>" + "" + "</option>");
        }
    }

    //fill folder drop down depending upon account type selected
    function fillTargetFolderDDL(accountType) {

        $(elementIDs.targetAccountDDLHelpBlockJQ).text('');
        $(elementIDs.targetAccountDDLHelpBlockJQ).removeClass('form-control');
        $(elementIDs.targetAccountDDLJQ).parent().removeClass('has-error');

        //enable folder drop down list.
        $(elementIDs.targetFolderNameDDLJQ).removeAttr('disabled');
        //empty the folder drop down list
        $(elementIDs.targetFolderNameDDLJQ).empty();
        $(elementIDs.targetFolderNameDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");        
        $(elementIDs.targetFolderNameDDLJQ).val("");
        $(elementIDs.targetFolderVersionDDLJQ).empty();
        $(elementIDs.targetFolderVersionDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");

        if (accountType != "0") {
            //ajax call for drop down list of folder list.       
            var url = URLs.getFolderNameList.replace(/\{accountType\}/g, accountType);
            var promise = ajaxWrapper.getJSON(url);
            //fill the folder list drop down
            promise.done(function (list) {
                for (i = 0; i < list.length; i++) {
                    if (list[i].AccountName != null) {
                        $(elementIDs.targetFolderNameDDLJQ).append("<option value=" + list[i].FolderId + ">" + list[i].AccountName + "  - " + list[i].FolderName + "</option>");
                    }
                    else {
                        $(elementIDs.targetFolderNameDDLJQ).append("<option value=" + list[i].FolderId + ">" + list[i].FolderName + "</option>");
                    }
                    targetFolderData = list;
                }
            });

            promise.fail(showError);
        }
        else {
            // disable the folder and form ddl.
            $(elementIDs.targetFolderNameDDLJQ).attr('disabled', 'disabled');
            //disable the folder version drop down list.
            $(elementIDs.targetFolderVersionDDLJQ).attr('disabled', 'disabled');
            //empty the folder version drop down list
            $(elementIDs.targetFolderVersionDDLJQ).empty();
            $(elementIDs.targetFolderVersionDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");
            //disable the form instance drop down list.
            $(elementIDs.targetFormInstanceDDLJQ).attr('disabled', 'disabled');
            //empty the form instance drop down list
            $(elementIDs.targetFormInstanceDDLJQ).empty();
            $(elementIDs.targetFormInstanceDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");
        }
    }

    //fill source folder version list
    function fillSouceFolderVarsionDLL(accountType, folderId) {

        $(elementIDs.sourceFolderNameDDLHelpBlockJQ).text('');
        $(elementIDs.sourceFolderNameDDLHelpBlockJQ).removeClass('form-control');
        $(elementIDs.sourceFolderNameDDLJQ).parent().removeClass('has-error');

        $(elementIDs.sourceFolderVersionDDLJQ).removeAttr('disabled');
        //empty the folder drop down list
        $(elementIDs.sourceFolderVersionDDLJQ).empty();
        $(elementIDs.sourceFolderVersionDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");

        if (accountType != "0") {
            //ajax call for drop down list of folder list.       
            var url = URLs.getFolderVersionList.replace(/\{accountType\}/g, accountType).replace(/\{folderId\}/g, folderId);
            //url = URLs.getFolderVersionList.replace(/\{FolderId\}/g, FolderId);
            var promise = ajaxWrapper.getJSON(url);
            //fill the folder list drop down
            promise.done(function (list) {
                for (i = 0; i < list.length; i++) {
                    $(elementIDs.sourceFolderVersionDDLJQ).append("<option value=" + list[i].FolderVersionId + ">" + list[i].FolderVersionNumber + "</option>");
                }
            });


            promise.fail(showError);
        }
        else {
            // disable the folder and form ddl.
            $(elementIDs.sourceFolderVersionDDLJQ).attr('disabled', 'disabled');
            //disable the form instance drop down list.
            $(elementIDs.sourceFormInstanceDDLJQ).attr('disabled', 'disabled');
            //empty the form instance drop down list
            $(elementIDs.sourceFormInstanceDDLJQ).empty();
            $(elementIDs.sourceFormInstanceDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");
        }
    }

    //fill target folder version list
    function fillTargetFolderVarsionDLL(accountType, folderId) {
        $(elementIDs.targetFolderNameDDLHelpBlockJQ).text('');
        $(elementIDs.targetFolderNameDDLHelpBlockJQ).removeClass('form-control');
        $(elementIDs.targetFolderNameDDLJQ).parent().removeClass('has-error');
        //enable folder drop down list.
        $(elementIDs.targetFolderVersionDDLJQ).removeAttr('disabled');
        //empty the folder drop down list

        $(elementIDs.targetFolderVersionDDLJQ).empty();
        $(elementIDs.targetFolderVersionDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");

        if (accountType != "0") {
            //ajax call for drop down list of folder list.       
            var url = URLs.getFolderVersionList.replace(/\{accountType\}/g, accountType).replace(/\{folderId\}/g, folderId);

            var promise = ajaxWrapper.getJSON(url);
            //fill the folder list drop down
            promise.done(function (list) {
                for (i = 0; i < list.length; i++) {
                    $(elementIDs.targetFolderVersionDDLJQ).append("<option value=" + list[i].FolderVersionId + ">" + list[i].FolderVersionNumber + "</option>");
                }
            });



            promise.fail(showError);
        }
        else {
            // disable the folder and form ddl.
            $(elementIDs.targetFolderVersionDDLJQ).attr('disabled', 'disabled');
            //disable the form instance drop down list.
            $(elementIDs.targetFormInstanceDDLJQ).attr('disabled', 'disabled');
            //empty the form instance drop down list
            $(elementIDs.targetFormInstanceDDLJQ).empty();
            $(elementIDs.targetFormInstanceDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");
        }
    }

    //load jqgrid 
    function loadChangeSummaryGrid(sourceFolderVersionId) {
        //set column list for grid

        var currentInstance = this;
        var colArray = ['FormDesignId', 'FormInstanceId', 'SourceForm', 'TargetForm', '', ''];

        //set column models
        var colModel = [];
        colModel.push({ name: 'FormDesignId', search: false, width: 10, align: 'left', hidden: true });
        colModel.push({ name: 'FormInstanceId', search: false, width: 10, align: 'left', hidden: true });
        colModel.push({ name: 'SourceForm', search: false, width: 70, align: 'left' });
        colModel.push({ name: 'FormTargetInstanceId', search: false, width: 10, align: 'left', hidden: true });
        colModel.push({ name: 'TargetForm', search: false, width: 70, align: 'left', formatter: targetFormFormatter, unformatter: targetFormFormatter });
        colModel.push({ name: 'IsIdentity', align: 'left', width: 30, editable: false, formatter: checkBoxFormatter, unformatter: unFormatIncludedColumn, sortable: false });

        //clean up the grid first - only table element remains after this
        $(elementIDs.documentChangeSummaryGridJQ).jqGrid('GridUnload');
        var url = URLs.getLoadFormList.replace(/\{sourceFolderVersionId\}/g, sourceFolderVersionId);
        $(elementIDs.documentChangeSummaryGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Select the Documents to be included in the Change Summary',
            height: '240',
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: true,
            altRows: true,
            pager: '#p' + elementIDs.documentChangeSummaryGrid,
            sortname: 'FormDesignName',
            altclass: 'alternate',
            loadComplete: function () {
                var rows = '';
                rows = $(this).getDataIDs();
                var sourceFolderName = $(elementIDs.sourceFolderNameDDLJQ).find("option:selected").text();
                var targetFolderName = $(elementIDs.targetFolderNameDDLJQ).find("option:selected").text();

                //set column name of grid as per select folder name & no. of form
                sourceFolderName = sourceFolderName + "  ( Documents# -" + rows.length + " )";
                targetFolderName = targetFolderName + "  ( Documents# -" + targetDataList.length + " )";
                $(elementIDs.documentChangeSummaryGridJQ).jqGrid('setLabel', 'SourceForm', sourceFolderName);
                $(elementIDs.documentChangeSummaryGridJQ).jqGrid('setLabel', 'TargetForm', targetFolderName);
            },
            gridComplete: function () {

                var rows = '';
                rows = $(this).getDataIDs();
                for (index = 0; index < rows.length; index++) {
                    row = $(this).getRowData(rows[index]);
                    isFound = false;
                    for (var i = 0; i < targetDataList.length; i++) {
                        if (targetDataList[i].FormDesignId == row.FormDesignId) {
                            $('#Select_' + rows[index] + '').append("<option value=" + targetDataList[i].FormInstanceId + ">" + targetDataList[i].TargetForm + "</option>");
                            isFound = true;
                        }
                    }
                    if (isFound != true) {
                        {
                            $('#checkbox_' + rows[index] + '').attr('disabled', 'disabled');
                            $('#Select_' + rows[index] + '').attr('disabled', 'disabled');
                        }
                    }
                    if (row.SourceForm == 'NA') {
                        for (var i = 0; i < targetDataList.length; i++) {
                            if (targetDataList[i].FormDesignId != row.FormDesignId) {
                                $('#Select_' + rows[index] + '').append("<option value=" + targetDataList[i].FormInstanceId + ">" + targetDataList[i].TargetForm + "</option>");
                                $('#Select_' + rows[index] + '').attr('disabled', 'disabled');
                                $('#checkbox_' + rows[index] + '').attr('disabled', 'disabled');
                            }
                        }
                    }
                    var cout = $('#Select_' + rows[index] + '').find("option").length;
                    if (cout == 1) {
                        $('#Select_' + rows[index] + '').hide();
                        var rowData = $('#my-jqgrid-table').jqGrid('getRowData', rows[index]);
                        row.TargetForm = $('#Select_' + rows[index] + '').find(":selected").text();
                        row.FormTargetInstanceId = $('#Select_' + rows[index] + '').val();
                        $(elementIDs.documentChangeSummaryGridJQ).jqGrid('setRowData', rows[index], row);
                    }
                }

                $.each(rows, function (i, item) {

                    if (item.FormDesignName == FolderVersion.inProgress && item.VersionType == FolderVersion.new) {
                        isReleased = false;
                    }
                });
            },
            onSelectRow: function (id) {
                var row = $(this).getRowData(id);
                if (row.FormDesignName == FolderVersion.inProgress) {
                    if (row.VersionType == FolderVersion.new) {
                        $('#btnMinorVersionRollback').addClass('ui-state-disabled');
                        $('#btnMinorVersionDelete').removeClass('ui-state-disabled');
                    }
                    $('#btnMinorVersionEdit').removeClass('ui-state-disabled');
                }
                else if (row.FormDesignName == FolderVersion.inProgressBlocked) {
                    $('#btnMinorVersionRollback').addClass('ui-state-disabled');
                    $('#btnMinorVersionDelete').addClass('ui-state-disabled');
                }
                else {
                    $('#btnMinorVersionDelete').addClass('ui-state-disabled');
                    $('#btnMinorVersionRollback').removeClass('ui-state-disabled');
                    $('#btnMinorVersionEdit').addClass('ui-state-disabled');
                }
            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.documentChangeSummaryGridJQ);
            }
        });

    }

    // check Valid for both dropdown has to select for load grid
    function dropDownfunctionValidate() {
        var folderSource = null, folderTarget = null
        folderSource = $(elementIDs.sourceFolderVersionDDLJQ).val();
        folderTarget = $(elementIDs.targetFolderVersionDDLJQ).val();

        if ((folderTarget != 0 && folderTarget != null) && (folderSource != 0 && folderSource != null)) {
            $(elementIDs.btnViewReportFormJQ).removeAttr("disabled");
            //load first target data 
            getTargetList(folderTarget);
            //load grid
            loadChangeSummaryGrid(folderSource, folderTarget);
        }
    }

    //get list of target form  
    function getTargetList(targetFolderVersionId) {
        //ajax call to add/update       
        url = URLs.getFolderFormList.replace(/\{foldersVersionId\}/g, targetFolderVersionId);
        var promise = ajaxWrapper.getJSON(url);
        //register ajax success callback
        promise.done(loadTargetList);
        //register ajax failure callback
        promise.fail(showError);
    }

    //get data of target documents
    function loadTargetList(data) {
        targetDataList = data;
    }
    // formatter use to get dropdown list for target form
    function targetFormFormatter(cellValue, options, rowObject) {

        if (cellValue == null) {
            result = "<Div id= Div_" + options.rowId + " class = Class_" + options.rowId + "><Select name = 'Target' data-rowID=" + options.rowId + " id = Select_" + options.rowId + " class = 'form-control'></Div";
        }
        else {
            result = cellValue;
        }
        return result;
    }
    //
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
    //after click Done  
    function saveChangeSummery() {
        var isValid = false;
        //check all input fields for validation.
        isValid = validateControls();

        if (isValid) {

            var allRowData = $(elementIDs.documentChangeSummaryGridJQ).jqGrid('getRowData');
           // var sourceAcountName = $(elementIDs.sourceAccountDDLJQ).find("option:selected").text();
           // var targetAcountName = $(elementIDs.targetAccountDDLJQ).find("option:selected").text();
            var sourceFolderId = $(elementIDs.sourceFolderNameDDLJQ).val();
            var sourceFolderName = '';
            var sourceFolder = sourceFolderData.filter(function (rowData) {
                return rowData.FolderId == sourceFolderId;
            });
            if (sourceFolder.length > 0)
                sourceFolderName = sourceFolder[0].FolderName;
            var targetFolderId = $(elementIDs.targetFolderNameDDLJQ).val();
            var targetFolderName = '';
            var targetFolder = targetFolderData.filter(function (rowData) {
                return rowData.FolderId == targetFolderId;
            });
            if (targetFolder.length > 0)
                targetFolderName = targetFolder[0].FolderName;
            var sourceFolderVersion = $(elementIDs.sourceFolderVersionDDLJQ).find(":selected").text();
            var targetFolderVersion = $(elementIDs.targetFolderVersionDDLJQ).find(":selected").text();
            var formSourceInstanceId, formTargetInstanceId;
            var formSourceInstanceName, formTargetInstanceName;

            lineItems = new Array();
            var saveData = new Object();
            saveData.Keys = new Array();
            var index = 1
            var j = 0;
            var found = 0;
            var csvData = '';
            for (var i = 0 ; i < allRowData.length; i++) {

                if ($('#checkbox_' + index + '').prop('checked') == true) {
                    if (allRowData[i].FormTargetInstanceId == "") {
                        var FormTargetInstanceId = $('#Select_' + index + '').val();
                        var TargetForm = $('#Select_' + index + '').find(":selected").text();
                    }
                    else {
                        FormTargetInstanceId = allRowData[i].FormTargetInstanceId;
                        TargetForm = allRowData[i].TargetForm;
                    }
                    lineItems[j] = new Object({
                        sourceInstanceId: allRowData[i].FormInstanceId, sourceInstanceName: allRowData[i].SourceForm,
                        targetInstanceId: FormTargetInstanceId, targetInstanceName: TargetForm

                    });
                    j++;
                }
                if ($('#checkbox_' + index + '').prop('disabled') == false) {
                    found++;
                }
                index++;
            }

            if (j == 0 && found == 0) {
                messageDialog.show("No Document to Compare ");
                return;
            }
            else {
                if (found != 0 && j == 0 || found == 0 && j != 0) {
                    messageDialog.show("Select Document to Compare ");
                    return;
                }
            }

            var jdata = JSON.stringify(lineItems);
            var encodeString = encodeURIComponent(jdata);
            //'/FolderVersionReport/ExportChangeSummaryReport'
            url = URLs.exportChangeSummaryReport;
            var stringData = "tenantId=" + 1;
            stringData += "<&sourceFolderId=" + sourceFolderId;
            stringData += "<&targetFolderId=" + targetFolderId;
            stringData += "<&sourceFolderName=" + sourceFolderName;
            stringData += "<&targetFolderName=" + targetFolderName;
            stringData += "<&sourceFolderVersion=" + sourceFolderVersion;
            stringData += "<&targetFolderVersion=" + targetFolderVersion;
            stringData += "<&encodeString=" + encodeString;

            $.downloadNew(url, stringData, 'post');
        }
    }

    //validate controls.
    function validateControls() {

       // var isSourceAccountNameSelected = false;
       // var isTargetAccountNameSelected = false;
        var isSourceFolderNameSelected = false;
        var isTargetFolderNameSelected = false;
        var isSourceFolderVersionSelected = false;
        var isTargetFolderVersionSelected = false;


        //if ($(elementIDs.sourceAccountDDLJQ).val() == "0") {
        //    $(elementIDs.sourceAccountDDLJQ).parent().addClass('has-error');
        //    $(elementIDs.sourceAccountDDLHelpBlockJQ).text(ChangeSummary.accountNameRequiredMsg);
        //    isSourceAccountNameSelected = false;
        //}
        //else {
        //    $(elementIDs.sourceAccountDDLHelpBlockJQ).removeClass('form-control');
        //    $(elementIDs.sourceAccountDDLJQ).parent().removeClass('has-error');
        //    $(elementIDs.sourceAccountDDLHelpBlockJQ).text('');
        //    isSourceAccountNameSelected = true;
        //}
        //if ($(elementIDs.targetAccountDDLJQ).val() == "0") {
        //    $(elementIDs.targetAccountDDLJQ).parent().addClass('has-error');
        //    $(elementIDs.targetAccountDDLHelpBlockJQ).text(ChangeSummary.accountNameRequiredMsg);
        //    isTargetAccountNameSelected = false;
        //}
        //else {

        //    $(elementIDs.targetAccountDDLHelpBlockJQ).removeClass('form-control');
        //    $(elementIDs.targetAccountDDLJQ).parent().removeClass('has-error');
        //    $(elementIDs.targetAccountDDLHelpBlockJQ).text('');
        //    isTargetAccountNameSelected = true;
        //}
        if ($(elementIDs.sourceFolderNameAutoComplDDLJQ).val() == "") {
            $(elementIDs.sourceFolderNameDDLJQ).parent().addClass('has-error');
            $(elementIDs.sourceFolderNameDDLHelpBlockJQ).text(ChangeSummary.folderNameRequiredMsg);
            isSourceFolderNameSelected = false;
        }
        else {

            $(elementIDs.sourceFolderNameDDLHelpBlockJQ).removeClass('form-control');
            $(elementIDs.sourceFolderNameDDLJQ).parent().removeClass('has-error');
            $(elementIDs.sourceFolderNameDDLHelpBlockJQ).text('');
            isSourceFolderNameSelected = true;
        }
        if ($(elementIDs.targetFolderNameAutoComplDDLJQ).val() == "") {
            $(elementIDs.targetFolderNameDDLJQ).parent().addClass('has-error');
            $(elementIDs.targetFolderNameDDLHelpBlockJQ).text(ChangeSummary.folderNameRequiredMsg);
            isTargetFolderNameSelected = false;
        }
        else {

            $(elementIDs.targetFolderNameDDLHelpBlockJQ).removeClass('form-control');
            $(elementIDs.targetFolderNameDDLJQ).parent().removeClass('has-error');
            $(elementIDs.targetFolderNameDDLHelpBlockJQ).text('');
            isTargetFolderNameSelected = true;
        }
        if ($(elementIDs.sourceFolderVersionDDLJQ).val() == "0") {
            $(elementIDs.sourceFolderVersionDDLJQ).parent().addClass('has-error');
            $(elementIDs.sourceFolderVersionDDLHelpBlockJQ).text(ChangeSummary.folderVarsionRequiredMsg);
            isSourceFolderVersionSelected = false;
        }
        else {

            $(elementIDs.sourceFolderVersionDDLHelpBlockJQ).removeClass('form-control');
            $(elementIDs.sourceFolderVersionDDLJQ).parent().removeClass('has-error');
            $(elementIDs.sourceFolderVersionDDLHelpBlockJQ).text('');
            isSourceFolderVersionSelected = true;
        }
        if ($(elementIDs.targetFolderVersionDDLJQ).val() == "0") {
            $(elementIDs.targetFolderVersionDDLJQ).parent().addClass('has-error');
            $(elementIDs.targetFolderVersionDDLHelpBlockJQ).text(ChangeSummary.folderVarsionRequiredMsg);
            isTargetFolderVersionSelected = false;
        }
        else {

            $(elementIDs.targetFolderVersionDDLHelpBlockJQ).removeClass('form-control');
            $(elementIDs.targetFolderVersionDDLJQ).parent().removeClass('has-error');
            $(elementIDs.targetFolderVersionDDLHelpBlockJQ).text('');
            isTargetFolderVersionSelected = true;
        }

        return (isSourceFolderNameSelected && isTargetFolderNameSelected && isSourceFolderVersionSelected && isTargetFolderVersionSelected)
    }

    function clearAll() {
        // $(elementIDs.sourceAccountDDLJQ)[0].selectedIndex = 0;


      //  $(elementIDs.targetAccountDDLJQ)[0].selectedIndex = 0;


        $(elementIDs.sourceFolderNameDDLJQ).empty();
        $(elementIDs.sourceFolderNameDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");       
        $(elementIDs.sourceFolderNameDDLJQ).attr('disabled', 'disabled');

        $(elementIDs.sourceFolderNameAutoComplDDLJQ).empty();       
        $(elementIDs.sourceFolderNameAutoComplDDLJQ).val("");

        $(elementIDs.targetFolderNameDDLJQ).empty();
        $(elementIDs.targetFolderNameDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");
        //$(elementIDs.targetFolderNameAutoComplDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");
        $(elementIDs.targetFolderNameDDLJQ).attr('disabled', 'disabled');

        $(elementIDs.targetFolderNameAutoComplDDLJQ).empty();
        $(elementIDs.targetFolderNameAutoComplDDLJQ).val("");
        

        $(elementIDs.sourceFolderVersionDDLJQ).empty();
        $(elementIDs.sourceFolderVersionDDLJQ).append("<option value='0'>" + " --Select--" + "</option>");
        $(elementIDs.sourceFolderVersionDDLJQ).attr('disabled', 'disabled');

        $(elementIDs.targetFolderVersionDDLJQ).empty();
        $(elementIDs.targetFolderVersionDDLJQ).append("<option value='0'>" + " --Select--" + "</option>");
        $(elementIDs.targetFolderVersionDDLJQ).attr('disabled', 'disabled');

        // $(elementIDs.documentChangeSummaryGridJQ).hide();
        $(elementIDs.documentChangeSummaryGridJQ).jqGrid("GridUnload");
        // location.reload();
        initilizeControls();

    }


    //formatter use to get checkbox
    function checkBoxFormatter(cellValue, options, rowObject) {


        if (rowObject.IsIdentity == "false")
            return "<input type='checkbox' id='checkbox_" + options.rowId + "' name= 'checkbox_" + options.gid + "' disabled='disabled' />";
        else
            return "<input type='checkbox' id='checkbox_" + options.rowId + "' name= 'checkbox_" + options.gid + "'/>";
    }

    function unFormatIncludedColumn(cellValue, options) {
        var result;
        result = $(this).find('#' + options.rowId).find('input').prop('checked');
        return result;
    }
    init();

    return {
        fillSourceDropdown: function (folderVersionId) {
            fillSouceFolderVarsionDLL(2, folderVersionId);           
        },
        fillTargetDropDown: function (folderVersionId) {
            fillTargetFolderVarsionDLL(2, folderVersionId);            
        },
    }
}();
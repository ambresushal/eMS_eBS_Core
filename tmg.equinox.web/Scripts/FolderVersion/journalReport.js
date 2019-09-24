var JournalReportForm = function () {

    var isInitialized = false;
    
    var URLs = {
        //get list of folder version 
        getFolderVersionList: '/FolderVersionReport/GetFolderVersionList?tenantId=1&accountType={accountType}&folderId={folderId}',
        //get name of folder name
        getFolderNameList: '/FolderVersionReport/GetFolderList?tenantId=1&accountType={accountType}',
        //get All Journal Responses
        getAllJournalResponsesList: '/JournalReport/GetAllJournalResponsesList?journalId={journalId}',
        getFormInstanceDesignData: "/FormInstance/GetFormInstanceData?tenantId={tenantId}&formInstanceId={formInstanceId}&formDesignVersionId={formDesignVersionId}&folderVersionId={folderVersionId}",
        //load jqgrid 
        getLoadFormList: '/FolderVersionReport/GetSourceFormInstanceList?tenantId=1&sourceFolderVersionId={sourceFolderVersionId}'
    };

    //added in Views/JournalReport/JournalEntryReport.cshtml
    var elementIDs = {
        // table element of journalReportGrid      
        journalReportDiv: '#journalReportDiv',
        //element of an Account or Portfolio dropdown list
        //sourceAccountDDLJQ: '#sourceAccountDDL',
        //element of folder name dropdownlist
        sourceFolderNameDDLJQ: '#sourceFolderNameDDL',
        //element of folderversion list 
        sourceFolderVersionDDLJQ: '#sourceFolderVersionDDL',
        //element of forminstance list
        sourceFormInstanceDDLJQ: '#sourceFormInstanceDDL',
        //element of checkbox for previos version
        isPreviousVersionCheckJQ: '#isPreviousVersionCheck',
        //with hash for use with jQuery
        journalGridJQ: "#journalReportGrid",
        sourceAccountDDLHelpBlockJQ: '#sourceAccountDDLHelpBlock',
        sourceFolderNameDDLHelpBlockJQ: '#sourceFolderNameDDLHelpBlock',
        sourceFolderVersionDDLHelpBlockJQ: '#sourceFolderVersionDDLHelpBlock',
        sourceFormInstanceDDLHelpBlockJQ: '#sourceFormInstanceDDLHelpBlock',

        //element of button to view report
        btnViewReportFormJQ: '#btnViewReportForm',
        btnCancelJQ: '#btnCancel',
        sourceFolderNameAutoCompltDDLJQ: '#sourceFolderNameAutoCompltDDL',
        sourceFormInstanceAutoCompltDDLJQ: '#sourceFormInstanceAutoCompltDDL', 
    };
    //this function is called below soon after this JS file is loaded
    function init() {
        $(document).ready(function () {
            if (isInitialized == false) {
                initilizeControls();
                // add on change event for  folder drop down
          //      $(elementIDs.sourceAccountDDLJQ).change(function (e) { return fillSourceFolderDDL($(this).val()); });
                // add on change event for  folder drop down
                $(elementIDs.sourceFolderNameDDLJQ).change(function (e) { return fillSourceFolderVersionDDL(2, $(this).val()); });
                
				//Auto complete Start
                $(function () {
                    $(elementIDs.sourceFolderNameDDLJQ).autoCompleteDropDown({ value: JournalReportForm.fillSourceFolderVerDropDown, ID: "sourceFolderNameAutoCompltDDL"}); //"ChangeSummaryForm.fillTargetDropDown"
                    $(elementIDs.sourceFolderNameDDLJQ).click(function () {
                        $(elementIDs.sourceFolderNameDDLJQ).toggle();
                    });
                });

                // add on change event for form version drop down
                $(elementIDs.sourceFolderVersionDDLJQ).change(function (e) { return fillSourceFormInstanceDDL($(elementIDs.sourceFolderNameDDLJQ).val(), $(this).val()); });
                // add on change event for form drop down
                $(elementIDs.sourceFormInstanceDDLJQ).change(function (e) {
                    $(elementIDs.sourceFormInstanceDDLHelpBlockJQ).text('');
                    $(elementIDs.sourceFormInstanceDDLJQ).parent().removeClass('has-error');
                    //load Grid
                    loadJournalGrid();
                });

                //Auto complete Start
                $(function () {
                    $(elementIDs.sourceFormInstanceDDLJQ).autoCompleteDropDown({ value: JournalReportForm.loadJournalGridFunc, ID: "sourceFormInstanceAutoCompltDDL", isDisabled : true });
                    $(elementIDs.sourceFormInstanceDDLJQ).click(function () {
                        $(elementIDs.sourceFormInstanceDDLJQ).toggle();
                    });
                });

                $(elementIDs.isPreviousVersionCheckJQ).on("change", function () {
                    loadJournalGrid();
                });

                //// add click event for Done button
                $(elementIDs.btnViewReportFormJQ).click(function (e) { return saveJournalReport(); });
                $(elementIDs.btnCancelJQ).click(function (e) { return clearAll(); });
                isInitialized = true;
            }
        });

    }
    //set default setting
    function initilizeControls() {
        $(elementIDs.btnViewReportFormJQ).attr('disabled', 'disabled');
        //$(elementIDs.sourceAccountDDLJQ).removeAttr("disabled");
        fillSourceFolderDDL(2);
    }

    //fill folder drop down depending upon account type selected
    function fillSourceFolderDDL(accountType) {
        $(elementIDs.sourceAccountDDLHelpBlockJQ).text('');
        $(elementIDs.sourceAccountDDLHelpBlockJQ).removeClass('form-control');
        //$(elementIDs.sourceAccountDDLJQ).parent().removeClass('has-error');
        //enable folder drop down list.
        $(elementIDs.sourceFolderNameDDLJQ).removeAttr('disabled');
        //empty the folder drop down list
        $(elementIDs.sourceFolderNameDDLJQ).empty();
        $(elementIDs.sourceFolderNameDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");
        $(elementIDs.sourceFolderNameDDLJQ).val("");
        //empty the folder version drop down list
        $(elementIDs.sourceFolderVersionDDLJQ).empty();
        $(elementIDs.sourceFolderVersionDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");        
        //empty the form instance drop down list
        $(elementIDs.sourceFormInstanceDDLJQ).empty();
        $(elementIDs.sourceFormInstanceDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");
        $(elementIDs.sourceFormInstanceDDLJQ).val("");
       
        $(elementIDs.sourceFormInstanceAutoCompltDDLJQ).empty();
        
        $(elementIDs.sourceFormInstanceAutoCompltDDLJQ).prop("disabled", true);
        if (accountType != "0") {
            //ajax call for drop down list of folder list.       
            var url = URLs.getFolderNameList.replace(/\{accountType\}/g, accountType);
            var promise = ajaxWrapper.getJSON(url);
            //fill the folder list drop down
            promise.done(function (list) {
                for (i = 0; i < list.length; i++) {
                    $(elementIDs.sourceFolderNameDDLJQ).append("<option value=" + list[i].FolderId + ">" + list[i].FolderName + "</option>");
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
            $(elementIDs.sourceFormInstanceDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");

        }
    }

    //fill source folder version list
    function fillSourceFolderVersionDDL(accountType, folderId) {

        $(elementIDs.sourceFolderNameDDLHelpBlockJQ).text('');
        $(elementIDs.sourceFolderNameDDLHelpBlockJQ).removeClass('form-control');
        $(elementIDs.sourceFolderNameDDLJQ).parent().removeClass('has-error');

        $(elementIDs.sourceFolderVersionDDLJQ).removeAttr('disabled');
        //empty the folder drop down list
        $(elementIDs.sourceFolderVersionDDLJQ).empty();
        $(elementIDs.sourceFolderVersionDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");

        //empty the form instance drop down list
        $(elementIDs.sourceFormInstanceDDLJQ).empty();
        $(elementIDs.sourceFormInstanceDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");

        $(elementIDs.sourceFormInstanceAutoCompltDDLJQ).empty();
        $(elementIDs.sourceFormInstanceAutoCompltDDLJQ).append("--Select--");
        $(elementIDs.sourceFormInstanceAutoCompltDDLJQ).val("");

        if (accountType != "0" && folderId != "0") {
            //ajax call for drop down list of folder list.       
            var url = URLs.getFolderVersionList.replace(/\{accountType\}/g, accountType).replace(/\{folderId\}/g, folderId);
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

    //fill source form instance list
    function fillSourceFormInstanceDDL(folderId,sourceFolderVersionId) 
    {
        $(elementIDs.sourceFolderVersionDDLHelpBlockJQ).text('');
        $(elementIDs.sourceFolderVersionDDLHelpBlockJQ).removeClass('form-control');
        $(elementIDs.sourceFolderVersionDDLJQ).parent().removeClass('has-error');

        $(elementIDs.sourceFormInstanceDDLJQ).removeAttr('disabled');
        //empty the form instance drop down list
        $(elementIDs.sourceFormInstanceDDLJQ).empty();
        $(elementIDs.sourceFormInstanceDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");
        $(elementIDs.sourceFormInstanceDDLJQ).val("");
        $(elementIDs.sourceFormInstanceAutoCompltDDLJQ).empty();        
        $(elementIDs.sourceFormInstanceAutoCompltDDLJQ).val("");

        $(elementIDs.sourceFormInstanceAutoCompltDDLJQ).attr('disabled', false);
        $(elementIDs.sourceFormInstanceAutoCompltDDLJQ).removeClass('disabledField');
        $(elementIDs.sourceFormInstanceAutoCompltDDLJQ).siblings('a').removeClass('disabledField');

        if (sourceFolderVersionId != "0") {
            //ajax call for drop down list of form instance list.       
            var url = URLs.getLoadFormList.replace(/\{sourceFolderVersionId\}/g, sourceFolderVersionId);
            var promise = ajaxWrapper.getJSON(url);
            //fill the form instance drop down
            promise.done(function (list) {
                for (i = 0; i < list.length; i++) {
                    $(elementIDs.sourceFormInstanceDDLJQ).append("<option value=" + list[i].FormInstanceId + "#" + list[i].FormDesignVersionId + ">" + list[i].SourceForm + "</option>");
                }
            });
            
            promise.fail(showError);
        }
        else {
            // disable the folder and form ddl.
            $(elementIDs.sourceFormInstanceDDLJQ).attr('disabled', 'disabled');
        }
        $(elementIDs.journalGridJQ).jqGrid('GridUnload');
    }

    //load jqgrid 
    function loadJournalGrid() {
        var isReport = true;
        var tenantId = 1;
        //var accountId = $(elementIDs.sourceAccountDDLJQ).val();
        var accountId = 1;
        var formInstanceId;
        var formDesignVersionId;
        var folderId = $(elementIDs.sourceFolderNameDDLJQ).val();
        var folderVersionId = $(elementIDs.sourceFolderVersionDDLJQ).val();
        if ($(elementIDs.sourceFormInstanceDDLJQ).val() != null)
            formInstanceId = $(elementIDs.sourceFormInstanceDDLJQ).val().split('#')[0];
        if ($(elementIDs.sourceFormInstanceDDLJQ).val() != null)
            formDesignVersionId = $(elementIDs.sourceFormInstanceDDLJQ).val().split('#')[1];
        var formDesignName = $(elementIDs.sourceFormInstanceDDLJQ).find(':selected').text();

        if (formInstanceId != 0 && formInstanceId != null) {
            $(elementIDs.btnViewReportFormJQ).removeAttr("disabled");
            var isPreviousVersion = $(elementIDs.isPreviousVersionCheckJQ).is(":checked");
            //invoke formInstanceManager instance, save to array
            var formInstancebuilder = new formInstanceBuilder(tenantId, accountId, folderVersionId, folderId, formInstanceId,
                                            formDesignVersionId, formDesignName, undefined, false);
            //formInstancebuilder.journal.loadJournalEntry(isPreviousVersion);
            try {

                var myWorker = new Worker('/Scripts/FolderVersion/journalNotify.js');

                if (isPreviousVersion) {
                    var url = '/JournalReport/GetAllJournalsList';

                    var saveData = JSON.stringify({
                        formInstanceId: formInstanceId,
                        folderVersionId: folderVersionId,
                        folderId: folderId,
                        formDesignVersionId: formDesignVersionId,
                        tenantId: tenantId
                    });
                }
                else {
                    var url = '/JournalReport/GetCurrentVersionJournalsList';

                    var saveData = JSON.stringify({
                        formInstanceId: formInstanceId,
                        folderVersionId: folderVersionId,
                        formDesignVersionId: formDesignVersionId,
                        tenantId: tenantId
                    });
                }

                if (typeof (Worker) !== "undefined") {
                    myWorker.onmessage = function (e) {
                        if (e.data != undefined) {
                            var journalGridData = new Array();
                            var list = JSON.parse(e.data);
                            for (var i = 0; i < list.length; i++) {
                                var listObject = list[i];
                                journalGridData.push(listObject);
                            }
                            groupJournalReportGrid(journalGridData, tenantId, folderId, folderVersionId, formInstanceId, formDesignVersionId, formDesignName, isPreviousVersion);
                        }
                    };

                    myWorker.onerror = function (err) {
                        console.log('Worker is suffering!', err);
                    }

                    myWorker.postMessage({
                        url: url,
                        saveData: saveData
                    });
                }
                else {
                    console.log("Sorry, your browser does not support Web Workers...");
                    messageDialog.show("Sorry, your browser does not support Web Workers...");
                }
            } catch (ex) {
                console.log(ex);
            }
        }

    }

    function groupJournalReportGrid(journalGridData, tenantId, folderId, folderVersionId, formInstanceId, formDesignVersionId, formName, isPreviousVersion) {
        var journalSortedGridData = new Array();

        var url = URLs.getFormInstanceDesignData.replace(/\{tenantId\}/g, tenantId).replace(/\{formInstanceId\}/g, formInstanceId).replace(/\{formDesignVersionId\}/g, formDesignVersionId).replace(/\{folderVersionId\}/g, folderVersionId);
        var promise = ajaxWrapper.getJSON(url);
        //register ajax success callback
        promise.done(function (xhr) {
            var designData = xhr;
            var data = designData.JSONData.replace(/\[Select One]/g, '').replace(/\Select One/g, '');
            var formData = JSON.parse(data);
            //Tab Journal
            for (var i = 0; i < journalGridData.length; i++) {
                var journalObject = journalGridData[i];
                if (journalGridData[i].FieldName == JournalEntry.attributeNotApplicableMsg && journalGridData[i].FieldPath == formName) {
                    journalSortedGridData.push(journalObject);
                }
            }
            //Section & other elements Journal
            var obje = flattenSortObject(formData);
            for (prop in obje) {
                if (prop.indexOf("jQuery") == -1) {
                    var path = prop;

                    var sectionPath = prop.replace(/[^a-zA-Z0-9]/g, '');
                        
                    //for adding section entries
                    var sectionObjects = journalGridData.filter(function (ct) {
                        var fieldPath = ct.FieldPath.replace(/[^a-zA-Z0-9]/g, '');
                        var fieldName = ct.FieldName;
                        return fieldName == JournalEntry.attributeNotApplicableMsg && fieldPath == sectionPath;
                    });
                    if (sectionObjects != null && sectionObjects.length > 0) {
                        for (var i = 0; i < sectionObjects.length; i++) {
                            var sectionObject = sectionObjects[i];
                            var sectionName1 = sectionObject.FieldPath.replace(/[^a-zA-Z0-9]/g, '');
                            var formInstanceId1 = sectionObject.FormInstanceID;
                            var journalAddedDate = sectionObject.AddedDate;
                            var sectionObjects1 = journalSortedGridData.filter(function (ct) {
                                var fieldPath = ct.FieldPath.replace(/[^a-zA-Z0-9]/g, '');
                                var fieldName = ct.FieldName;
                                var formInstanceId2 = ct.FormInstanceID;
                                var addedDate = ct.AddedDate;
                                return fieldName == JournalEntry.attributeNotApplicableMsg && addedDate == journalAddedDate
                                    && fieldPath == sectionName1 && formInstanceId2 == formInstanceId1;
                            });
                            if (sectionObjects1.length == 0) {
                                journalSortedGridData.push(sectionObject);
                            }
                        }
                    }
                        
                    var journalObjects = journalGridData.filter(function (ct) {
                        var fieldPath = ct.FieldPath.replace(/[^a-zA-Z0-9]/g, '');
                        var fieldName = ct.FieldName.replace(/[^a-zA-Z0-9]/g, '');
                        var fieldPath1 = fieldPath + fieldName;
                        return fieldPath1 == sectionPath;
                    });
                    if (journalObjects != null && journalObjects.length > 0) {
                        for (var i = 0; i < journalObjects.length; i++) {
                            var journalObject = journalObjects[i];
                            var fieldPath1 = journalObject.FieldPath.replace(/[^a-zA-Z0-9]/g, '');
                            var fieldName1 = journalObject.FieldName.replace(/[^a-zA-Z0-9]/g, '');
                            var formInstanceId1 = journalObject.FormInstanceID;
                            var journalAddedDate = journalObject.AddedDate;
                            var journalObjects1 = journalSortedGridData.filter(function (ct) {
                                var fieldName = ct.FieldName.replace(/[^a-zA-Z0-9]/g, '');
                                var fieldPath = ct.FieldPath.replace(/[^a-zA-Z0-9]/g, '');
                                var formInstanceId2 = ct.FormInstanceID;
                                var addedDate = ct.AddedDate;
                                return fieldName == fieldName1 && addedDate == journalAddedDate
                                    && fieldPath == fieldPath1 && formInstanceId2 == formInstanceId1;
                            });
                            if (journalObjects1.length == 0) {
                                journalSortedGridData.push(journalObject);
                            }
                        }
                    }
                }
            }
            buildJournalReportGrid();
            loadJournalReportGridData(journalSortedGridData);
        });
        promise.fail(showError);
    }

    function loadJournalReportResponseGridData(data, subGridId) {
        $(subGridId).jqGrid('clearGridData');

        if (data != undefined) {
            for (var i = 0; i < data.length; i++) {
                var item = data[i];
                $(subGridId).jqGrid('addRowData', i, data[i]);
            }
        }
    }

    function loadJournalReportResponse(journalId, subGridId) {

        //ajax call for journal list of forminstance.       
        var url = URLs.getAllJournalResponsesList.replace(/\{journalId\}/g, journalId);
        var promise = ajaxWrapper.getJSON(url);

        promise.done(function (responseList) {
            var journalRows = new Array();
            for (var i = 0; i < responseList.length; i++) {
                var responseListObject = responseList[i];
                journalRows.push(responseListObject);
            }
            loadJournalReportResponseGridData(journalRows, subGridId);
        });
        promise.fail(showError);
    }

    function loadJournalReportGridData(data) {
        $(elementIDs.journalGridJQ).jqGrid('clearGridData');

        if (data != undefined) {
            for (var i = 0; i < data.length; i++) {
                var item = data[i];
                $(elementIDs.journalGridJQ).jqGrid('addRowData', i, data[i]);
            }
        }
    }

    function buildJournalReportGrid() {
        $(elementIDs.journalGridJQ).jqGrid('GridUnload');

        $(elementIDs.journalGridJQ).jqGrid({
            datatype: "local",
            colNames: ['JournalID', 'FormInstanceID', 'FolderVersionID', 'ActionID', 'Action Required', 'Field Path', 'Field Name', 'Description', 'Added Workflow State', 'Closed Workflow State', 'Added By', 'Added Date', 'Updated By', 'Updated Date', 'Version Number'],
            colModel: [{ name: 'JournalID', index: 'JournalID', key: true, hidden: true },
                       { name: 'FormInstanceID', index: 'FormInstanceID', hidden: true },
                       { name: 'FolderVersionID', index: 'FolderVersionID', hidden: true },
                       { name: 'ActionID', index: 'ActionID', hidden: true },
                       { name: 'ActionName', index: 'ActionName', width: '60' },
                       { name: 'FieldPath', index: 'FieldPath', width: '220' },
                       { name: 'FieldName', index: 'FieldName', width: '110' },
                       { name: 'Description', index: 'Description', width: '205' },
                       { name: 'AddedWFStateName', index: 'AddedWFStateName', width: '120' },
                       { name: 'ClosedWFStateName', index: 'ClosedWFStateName', width: '120' },
                       { name: 'AddedBy', index: 'AddedBy', width: '80' },
                       { name: 'AddedDate', index: 'AddedDate', width: '70', formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' },
                       { name: 'UpdatedBy', index: 'UpdatedBy', width: '80' },
                       { name: 'UpdatedDate', index: 'UpdatedDate', width: '80', formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' },
                       { name: 'FolderVersionNumber', index: 'FolderVersionNumber', width: '100' }],
            pager: '#pjournalReportGrid',
            autowidth: true,
            height: '150',
            altRows: true,
            altclass: 'alternate',
            rownumbers: true,
            rownumWidth: 40,
            pgbuttons: false,
            pgtext: null,
            hiddengrid: false,
            hidegrid: false,
            rowNum: 500000,
            caption: 'Journal List',
            subGrid: true,
            subGridRowExpanded: function (subgrid_id, rowid) {
                var subgrid_table_id, pager_id;
                subgrid_table_id = subgrid_id + "_t";
                pager_id = "p_" + subgrid_table_id;
                $("#" + subgrid_id).html("<table id='" + subgrid_table_id + "' class='scroll'></table><div id='" + pager_id + "' class='scroll'></div>");
                $("#" + subgrid_table_id).jqGrid({
                    datatype: "local",
                    colNames: ['JournalResponseID', 'JournalID', 'Response', 'Added By', 'Added Date'],
                    colModel: [{ name: 'JournalResponseID', index: 'JournalResponseID', key: true, hidden: true },
                               { name: 'JournalID', index: 'JournalID', hidden: true },
                               { name: 'Description', index: 'Description', width: '1000' },
                               { name: 'AddedBy', index: 'AddedBy', width: '80' },
                               { name: 'AddedDate', index: 'AddedDate', width: '70', formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' }],
                    rowNum: 500000,
                    height: '100%',
                    autowidth: false,
                    caption: 'Responses List',
                    shrinkToFit: true,
                    altRows: true,
                    altclass: 'alternate',
                    rownumWidth: 40,
                    pgbuttons: false,
                    hiddengrid: false,
                    hidegrid: false
                });

                var row = $(elementIDs.journalGridJQ).jqGrid("getLocalRow", rowid);
                if (row) {
                    loadJournalReportResponse(row.JournalID, "#" + subgrid_table_id);
                }
            },
            afterInsertRow: function (rowId, rowData) {
                if (rowData.ActionID === JournalEntryAction.YESOPEN) {
                    $("#" + rowId, "#journalReportGrid").css({ color: 'red' })
                }
            },
            gridComplete: function () {
                if ($(elementIDs.journalGridJQ).getGridParam('records') == 0) {
                    $(elementIDs.journalGridJQ).css({ height: '1px' })
                }
            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.journalGridJQ);
            }
        });
        $(elementIDs.journalGridJQ).jqGrid('navGrid', "#pjournalGrid", { edit: false, add: false, del: false, refresh: true, search: false }, {}, {}, {});
        $(elementIDs.journalGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

    }

    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
    //after click Done  
    function saveJournalReport() {
        var isValid = false;
        //check all input fields for validation.
        isValid = validateControls();

        if (isValid) {
            var accountName = $(elementIDs.sourceAccountDDLJQ).find("option:selected").text();
            var formInstanceId = $(elementIDs.sourceFormInstanceDDLJQ).val().split('#')[0];
            var folderId = $(elementIDs.sourceFolderNameDDLJQ).val();
            var folderName = $(elementIDs.sourceFolderNameDDLJQ).find(":selected").text();
            var folderVersionId = $(elementIDs.sourceFolderVersionDDLJQ).val();
            var formDesignName = $(elementIDs.sourceFormInstanceDDLJQ).find(':selected').text();
            var isPreviousVersion = $(elementIDs.isPreviousVersionCheckJQ)[0].checked;
            var formDesignVersionId = $(elementIDs.sourceFormInstanceDDLJQ).val().split('#')[1];

            url = '/JournalReport/ExportJournalReport';

            var stringData = "tenantId=" + 1;
            stringData += "<&accountName=" + accountName;
            stringData += "<&formInstanceId=" + formInstanceId;
            stringData += "<&folderId=" + folderId;
            stringData += "<&folderName=" + folderName;
            stringData += "<&folderVersionId=" + folderVersionId;
            stringData += "<&formDesignName=" + formDesignName;
            stringData += "<&isPreviousVersion=" + isPreviousVersion;
            stringData += "<&formDesignVersionId=" + formDesignVersionId;

            $.download(url, stringData, 'post');
        }
    }

    //validate controls.
    function validateControls() {

       // var isSourceAccountNameSelected = false;
        var isSourceFolderNameSelected = false;
        var isSourceFolderVersionSelected = false;

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
        
        if ($(elementIDs.sourceFolderNameDDLJQ).val() == "0") {
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

        if ($(elementIDs.sourceFormInstanceDDLJQ).val() == "0") {
            $(elementIDs.sourceFormInstanceDDLJQ).parent().addClass('has-error');
            $(elementIDs.sourceFormInstanceDDLHelpBlockJQ).text(ChangeSummary.formInstanceRequiredMsg);
            $(elementIDs.sourceFormInstanceAutoCompltDDL).addClass('highlightBorder');
            $(elementIDs.sourceFormInstanceAutoCompltDDL).siblings('a').addClass('highlightBorder');
            isSourceFormInstanceSelected = false;
        }
        else {
            $(elementIDs.sourceFormInstanceDDLHelpBlockJQ).removeClass('form-control');
            $(elementIDs.sourceFormInstanceDDLJQ).parent().removeClass('has-error');
            $(elementIDs.sourceFormInstanceDDLHelpBlockJQ).text('');
            isSourceFormInstanceSelected = true;
        }
        
        return (isSourceFolderNameSelected && isSourceFolderVersionSelected && isSourceFormInstanceSelected)
    }

    function clearAll() {
        //$(elementIDs.sourceAccountDDLJQ)[0].selectedIndex = 0;

        $(elementIDs.sourceFolderNameDDLJQ).empty();
        $(elementIDs.sourceFolderNameDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");
        $(elementIDs.sourceFolderNameDDLJQ).val("");
        $(elementIDs.sourceFolderNameDDLJQ).attr('disabled', 'disabled');

        $(elementIDs.sourceFolderNameAutoCompltDDLJQ).empty();
        $(elementIDs.sourceFolderNameAutoCompltDDLJQ).val("");
        
        $(elementIDs.sourceFolderVersionDDLJQ).empty();
        $(elementIDs.sourceFolderVersionDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");
        $(elementIDs.sourceFolderVersionDDLJQ).attr('disabled', 'disabled');

        $(elementIDs.sourceFormInstanceDDLJQ).empty(); 
        $(elementIDs.sourceFormInstanceDDLJQ).append("<option value='0'>" + "--Select--" + "</option>");
        $(elementIDs.sourceFormInstanceDDLJQ).val("");
        $(elementIDs.sourceFormInstanceDDLJQ).attr('disabled', 'disabled');

        $(elementIDs.sourceFormInstanceAutoCompltDDLJQ).empty();
        $(elementIDs.sourceFormInstanceAutoCompltDDLJQ).val("");

        $(elementIDs.sourceFormInstanceAutoCompltDDLJQ).addClass('disabledField');
        $(elementIDs.sourceFormInstanceAutoCompltDDLJQ).siblings('a').addClass('disabledField');

        $(elementIDs.isPreviousVersionCheckJQ)[0].checked = false;

        $(elementIDs.journalGridJQ).jqGrid("GridUnload");
        initilizeControls();
    }

    init();

    return {
        fillSourceFolderVerDropDown: function (folderVersionID)
        {
            return fillSourceFolderVersionDDL(2, folderVersionID);
        },
        loadJournalGridFunc: function (folderVersionID)
        {
            return loadJournalGrid();
        }
    }
}();
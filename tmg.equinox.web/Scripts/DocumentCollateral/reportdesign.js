/* Report Design Template Grid*/
var selectedReportName = '';
var selectedRowData;
var selectedSourceDocuments = [];

var reportTemplate = function () {

    //variables required for tab management
    var tabsReportVersion;
    var tabIndex = 0;
    var tabCount = 0;
    var tabNamePrefix = 'report-';
    var selectedReportID = 0;
    var selectedGenReportID = 0;
    var selectedReportTemplateVersionID = 0;
    var selectedReportTemplateLocation = '';
    var webConfigCollateralOption = '';
    var ReportsResults;


    //urls to be accessed for form design
    var URLs = {
        //get Report Names
        reportList: '/DocumentCollateral/GetReportNames',

        //Delete Report Design
        reportDesignDelete: '/DocumentCollateral/DeleteCollateralDesign',

        //get Report Template Versions List
        reportVersionList: '/DocumentCollateral/GetReportTemplateVersions?TenantID={TenantID}&ReportTemplateID={ReportTemplateID}',
        //Edit Template Version for Report
        reportEditTmplVrsn: '/DocumentCollateral/UpdateReportTemplateVersion?TenantID={TenantID}&ReportTemplateVersionID={ReportTemplateVersionID}',

        // gets Report Names for Report Generation
        reportListGen: '/DocumentCollateral/GetReportNamesForGeneration?ReportLocation=InMenu',
        // gets the applicable Accounts, related Folders and related Folder Versions for Report Genaration
        getAccFolderdocumentsForReport: '/DocumentCollateral/GetAccountFolderFolderVersionDocuments?ReportTemplateID={ReportTemplateID}&AccountID={AccountID}&FolderID={FolderID}&FolderVersionID={FolderVersionID}',
        // generates the Report based on selected parameters   
        viewReport: "/DocumentCollateral/ViewReport",

        getInputsForGeneratingReport: "/DocumentCollateral/ViewReport/?accountID={AccountID}&folderID={FolderID}&formInstanceID={FormInstanceID}&folderVersionID={FolderVersionID}&reportTemplateName={ReportTemplateName}&reportTemplateID={ReportTemplateID}&folderVersionEffDt={FolderVersionEffDt}&description={Description}&formDesignVersionID={FormDesignVersionID}&collateralOptions={CollateralOptions}",

        //Checks whether user has access to report
        checkPermission: '/DocumentCollateral/CheckRolePermission?reportTemplateID={ReportTemplateID}&folderVersionEffDt={FolderVersionEffDt}',
        //get Document Design Version
        formDesignVersionList: '/DocumentCollateral/FormDesignVersionList?tenantId=1&formDesignId={formDesignId}',
        //get Template Document Mapping
        formTemplateDocumentMapping: '/DocumentCollateral/GetDocumentDesignVersion',
        //get Accounts Folders and Folder Versions for Report generation
        //getAccountsFolder: '/DocumentCollateral/GetAccountsFolders?reportTemplateVersionID={reportTemplateVersionID}&AccountID={AccountID}&FolderID={FolderID}'
        reportTemplateVrsnDelete: '/DocumentCollateral/DeleteReportTemplateVersion',
        IsTemplateUploaded: '/DocumentCollateral/IsTemplateUploaded?reportTemplateVersionId={ReportTemplateVersionId}',
        queueCollateral: "/DocumentCollateral/QueueCollateral?accountID={accountID}&accountName={accountName}&folderID={folderID}&folderName={folderName}&formInstanceIDs={formInstanceIDs}&folderVersionID={folderVersionID}&folderVersionNumbers={folderVersionNumbers}&reportTemplateID={reportTemplateID}&productIds={productIds}&folderVersionEffDt={folderVersionEffDt}&runDate={runDate}&reportName={ReportName}",
        getAccountsAndFolders: '/DocumentCollateral/GetAccountAndFolders?reportTemplateID={ReportTemplateID}&accountID={AccountID}&folderID={FolderID}&folderVersionID={FolderVersionID}&&reportName={reportName}&planFamilyName={planFamilyName}&shouldSelectFormInstance=true',
        checkIfCollateralIsOfSBDesignType: '/DocumentCollateral/CheckIfCollateralIsOfSBDesignType?templateReportID=',
        generateCollateralSBDesign: '/DocumentCollateral/GenerateCollateral',
        queueSBDesignCollateral: "/DocumentCollateral/QueueSBDesignCollateral?accountIDs={accountID}&accountNames={accountName}&folderIDs={folderID}&folderNames={folderName}&formInstanceIDs={formInstanceIDs}&folderVersionIDs={folderVersionID}&folderVersionNumbers={folderVersionNumbers}&productIds={productIds}&folderVersionEffDts={folderVersionEffDt}&formDesignIds={formDesignID}&formDesignVersionIds={formDesignVersionID}&reportTemplateID={reportTemplateID}&reportName={ReportName}",
        getPlanFamilyDropdownList: '/DocumentCollateral/GetPlanFamilyDropdownList?templateReportID={templateReportID}&reportName={reportName}',
    };


    var elementIDs = {
        // Create Report Template and Version Tab
        viewReportTemplateTab: '#viewReportTemplate',
        // Create Report Template Grid - Report Names
        reportTemplateGrid: 'rptTmplNames',
        // Create Report Template GridID - Report Names
        reportTemplateGridJQ: '#rptTmplNames',
        // Create Report Template Version Grid - Report Versions for the Report selected on the Report Grid.
        reportTemplateVersionGrid: 'rptTmplVersions',
        // Create Report Template Version GridID - Report Versions for the Report selected on the Report Grid.
        reportTemplateVersionGridJQ: '#rptTmplVersions',



        // generate Report Grid - Tab
        reportGenerateTab: '#reportGenerate',
        // generate Report Grid - Report Names.
        reportGrid: 'fdgReportgen',
        // generate Report GridID - Report Names.
        reportGridJQ: '#fdgReportgen',
        // Available Forms Grid for Report generation
        relatedDocumentGrid: 'documentsInFolder',
        // Available Forms GridID for Report generation
        relatedDocumentGridJQ: '#documentsInFolder',
        // Available Forms Grid for Report generation while queueing
        collateralQueueDocumentsGrid: "collaterlDialogDocumentsInFolder",
        collateralQueueDocumentsGridJQ: "#collaterlDialogDocumentsInFolder",

        // Account, Folder, FolderVersion Dropdown IDs - Generate Report screen
        genReportAccountID: '#accountDropDown',
        genReportFolderID: '#folderDropDown',
        genReportFolderVersionID: '#folderVersionDrpDown',
        selectDocumentDrpDown: '#selectDocumentDrpDown',

        // parent table encapsulating the three elements mentioned above
        genReportDropDownOptions: '#accountFolderOpt',
        genReportButton: '#generateReportbtn',
        reportTmplVrsnDialog: "#reportTmplVrsnDialog",
        queueCollateralBtn: '#QueueCollateralBtn',
        uploadCollateralBtn: '#UploadCollateralBtn',
        viewQueueCollateralBtn: '#viewQueueCollateralBtn',
        divCollateralOptions: '#divCollateralOptions',
        hiddenCollateralOptionVal: '#webConfigCollateralOption',
        selectDocumentLabel: '#selectDocumentLabel',
        selectDocumentOptTable: '#selectDocumentOptTable',
        divAccountDetails: '#divAccountDetails',
        viewAccDetailsDialogButn: '#cmdSelectAccountDetails',
        sourceDocument: "#sourceDocument",
        sourceDocumentGrid: 'sourceDocumentList',
        sourceDocumentGridJQ: '#sourceDocumentList',
        selectedSourceDocumentGrid: 'selectedSourceDocumentList',
        selectedSourceDocumentGridJQ: '#selectedSourceDocumentList',
        divGenerateReport: '#generateReportDiv',
        divGenerateSBReport: '#generateSBReportDiv',
        generateCollateralBtn: '#generateCollateralBtn',
        queueSBDesignCollateralBtn: '#queueSBDesignCollateralBtn',
        uploadSBDesignCollateralBtn: '#uploadSBDesignCollateralBtn',
        btnReportTemplateAddJQ: '#btnReportTemplateAdd',
        btnReportTemplateEditJQ: '#btnReportTemplateEdit',
        btnReportDesignDeleteJQ: '#btnReportDesignDelete',
        btnReportTemplateVersionAddJQ: '#btnReportTemplateVersionAdd',
        btnReportTemplateVersionEditJQ: '#btnReportTemplateVersionEdit',
        btnReportTemplateVersionDeleteJQ: '#btnReportTemplateVersionDelete',
        btnReportTemplateVersionFinalizedJQ: '#btnReportTemplateVersionFinalized',
        planFamilyJQ: '#planFamily',
        planFamilyDivJQ: '#planFamilyDiv'

    };

    var currentFilterElementID;
    var customSearchOptions = {
        dataEvents:
            [{
                type: 'keypress',
                fn: function (e) {
                    if (e.keyCode != undefined) {
                        currentFilterElementID = e.target.id;
                    }
                }
            }]
    };



    //the function below is called as soon as the JS file is loaded
    //generates the tabs and loads the Document Design Grid
    function init() {
        var gh = '';
        // Set Cancel Date to EOT.
        $(function () {
            $("#effDate").datepicker();
            //$("#canDate").datepicker();
            var date = "01/01/2099";

            $("#canDate").val(date);
        });

        $(document).ready(function () {

            setValue = function (gridName, rowID, eleId, colName, prop) {
                var rowid = parseInt(eleId.slice(4));
                $(gridName).jqGrid('setCell', rowID, colName, $('#' + eleId).prop(prop));
                var griddataa = $(gridName).jqGrid('getRowData');
            }

            editTemplateVersion = function (TenantID, ReportTemplateVersionID) {
                selectedReportTemplateVersionID = ReportTemplateVersionID;
                var url = URLs.reportEditTmplVrsn.replace(/\{TenantID\}/g, TenantID).replace(/\{ReportTemplateVersionID\}/g, ReportTemplateVersionID);
                window.open(url, "popupWindow", "width=1200,height=700,scrollbars=yes,resizable=yes");
                reportVersionConfig.reportConfigurationGrid(reportTemplate.selectedReportTemplateVersionID());
            }

            DownLoadDocument = function (reportVersionID) {
                var checkUrl = '/DocumentCollateral/CheckTemplateDownloadFilePossibility?ReportTemplateVersionID=' + reportVersionID;
                var downloadUrl = '/DocumentCollateral/DownloadDocument?ReportTemplateVersionID=' + reportVersionID;
                var promise = ajaxWrapper.postJSON(checkUrl); //DownloadDocument2
                promise.done(function (xhr) {
                    if (xhr != 'Success')
                        messageDialog.show('Collateral Template is not available.');
                    else {
                        //this.elementIDs.uploadDocMapForReportTableIDJQ
                        var stringData = 'ReportTemplateVersionID=' + reportVersionID;
                        var result = $.downloadNew(downloadUrl, stringData, 'post');
                    }
                });
            }

            //To remove style attribute so that Document Design tab is displayed after loading the page.
            $(elementIDs.reportGenerateTab).removeAttr("style");
            //$(elementIDs.reportTmplViewTab).removeAttr("style");
            $(elementIDs.viewReportTemplateTab).removeAttr("style");

            //jqueryui tabs
            tabsReportVersion = $(elementIDs.viewReportTemplateTab).tabs();
            tabsGenerateReport = $(elementIDs.reportGenerateTab).tabs();

            //register event for closing a tab page - refer jquery ui documentation
            //event will be registered for each tab page loaded
            tabsReportVersion.delegate('span.ui-icon-close', 'click', function () {
                var panelId = $(this).closest('li').remove().attr('aria-controls');
                $('#' + panelId).remove();
                tabCount--;
                //tabIndex = tabIndex - 1;
                tabsReportVersion.tabs('refresh');
            });

            // loading Data 
            generateReports();
            viewReportTemplates();
        });

        // generate Report DropDown Events and Generate button event.
        $(function () {



            $(elementIDs.genReportAccountID).on('change', function () {
                var accountID = $(elementIDs.genReportAccountID).val();
                $(elementIDs.relatedDocumentGridJQ).jqGrid('GridUnload');
                $(elementIDs.selectDocumentDrpDown).hide();
                $(elementIDs.selectDocumentLabel).hide();
                $(elementIDs.selectDocumentOptTable).hide();
                $(elementIDs.genReportButton).hide();
                if (accountID)
                    getAccountsFolders(selectedGenReportID, accountID, 0, 0);
                else {
                    $(elementIDs.genReportFolderID).val('');
                    $(elementIDs.genReportFolderVersionID).val('');
                    $(elementIDs.selectDocumentDrpDown).val('');
                }
            });

            $(elementIDs.genReportFolderID).on('change', function () {
                var accountID = $(elementIDs.genReportAccountID).val();
                var folderID = $(elementIDs.genReportFolderID).val();
                $(elementIDs.relatedDocumentGridJQ).jqGrid('GridUnload');
                $(elementIDs.selectDocumentDrpDown).hide();
                $(elementIDs.selectDocumentLabel).hide();
                $(elementIDs.selectDocumentOptTable).hide();
                $(elementIDs.genReportButton).hide();
                if (!accountID) {
                    messageDialog.show("Please select the Account to proceed with this.");
                    $(elementIDs.genReportFolderID).val('');
                }
                else if (folderID) {
                    getAccountsFolders(selectedGenReportID, accountID, folderID, 0);
                }
                else
                    $(elementIDs.genReportFolderVersionID).val('');
                $(elementIDs.selectDocumentDrpDown).val('');
            });

            $(elementIDs.genReportFolderVersionID).on('change', function () {
                var accountID = $(elementIDs.genReportAccountID).val();
                var folderID = $(elementIDs.genReportFolderID).val();
                var folderVersionID = $(elementIDs.genReportFolderVersionID).val();
                $(elementIDs.queueCollateralBtn).show();
                $(elementIDs.uploadCollateralBtn).show();
                webConfigCollateralOption = $(elementIDs.hiddenCollateralOptionVal).val();
                if (webConfigCollateralOption.toLowerCase() == "both")
                    $(elementIDs.divCollateralOptions).show();
                if (!accountID || !folderID) {
                    messageDialog.show("Please select the Account and Folder to proceed with this.");
                    $(elementIDs.genReportFolderVersionID).val('');
                }
                else if (folderVersionID) {

                    //Added DropDown to show report document, and hiding the grid. The grid Function is commented 'selectDocumentsForReportGeneration'
                    populateDocumentsForReportGeneration(selectedGenReportID, accountID, folderID, folderVersionID, elementIDs.relatedDocumentGrid, elementIDs.relatedDocumentGridJQ);
                    //selectDocumentsForReportGeneration(selectedGenReportID, accountID, folderID, folderVersionID, elementIDs.relatedDocumentGrid, elementIDs.relatedDocumentGridJQ);
                }
                else
                    $(elementIDs.relatedDocumentGridJQ).jqGrid('GridUnload');
            });

            $(elementIDs.genReportButton).on('click', function () {
                var selectedCollateralOptions = [];
                if (webConfigCollateralOption.toLowerCase() == "both") {
                    selectedCollateralOptions = [];
                    $('input:checkbox[name=collateralOption]').each(function () {
                        if ($(this).is(':checked'))
                            selectedCollateralOptions.push($(this).val());
                    });
                }
                else
                    selectedCollateralOptions.push(webConfigCollateralOption);

                if (webConfigCollateralOption.toLowerCase() == "both" && selectedCollateralOptions.length == 0) {
                    messageDialog.show('Please select atleast one Collateral Option.');
                    return false;
                }
                else {
                    var url = URLs.viewReport;

                    var description = "";
                    var grid = $(elementIDs.relatedDocumentGridJQ);
                    var gridRows = grid.jqGrid('getRowData');
                    var arr = [];
                    var indexestodelete = [];
                    arr = gridRows;

                    var FormInstanceID = "", formDesignVersionID = "";
                    //var DataSourceNames = "";

                    var account = $(elementIDs.genReportAccountID + ' :selected').text();
                    var folder = $(elementIDs.genReportFolderID + ' :selected').text();
                    var folderVersion = $(elementIDs.genReportFolderVersionID + ' :selected').text();
                    var effDate = $(elementIDs.genReportFolderVersionID + ' :selected').text()

                    var selectedFormInstanceID = $(elementIDs.selectDocumentDrpDown + ' :selected').val();

                    var selectedFormName = $(elementIDs.selectDocumentDrpDown + ' :selected').text();

                    var objJsonFormResults;

                    objJsonFormResults = $(ReportsResults).filter(function (i, n) {
                        return n.FormName === selectedFormName;
                    });

                    var objFormDesignVersionID;
                    var objFolderVersionNumber;
                    var objDocumentType;

                    if (objJsonFormResults != null && objJsonFormResults.length > 0) {
                        objFormDesignVersionID = objJsonFormResults[0].FormDesignVersionID;
                        objFolderVersionNumber = objJsonFormResults[0].FolderVersionNumber;
                        objDocumentType = objJsonFormResults[0].DocumentType;
                    }

                    FormInstanceID = selectedFormInstanceID + ",";
                    formDesignVersionID = objFormDesignVersionID;


                    var effdateformt = new Date(effDate).toLocaleDateString();

                    description = 'Genrated Collateral for account ' + account + ' having Folder ' + folder + ' With Effective Date ' + effdateformt;
                    description += ' and Version ' + objFolderVersionNumber + ' has document ' + selectedFormName + ' of Type ' + objDocumentType;

                    //$.each(arr, function (index) {
                    //    if (arr[index]['IsSelected']) {
                    //        formDesignVersionID = arr[index]['FormDesignVersionID'];
                    //        FormInstanceID += arr[index]['FormInstanceID'] + ",";
                    //        //DataSourceNames += arr[index]['DataSourceNames'] + ",";
                    //        description += ' and Version ' + arr[index]["FolderVersionNumber"] + ' has document ' + arr[index]["FormName"] + ' of Type ' + arr[index]["DocumentType"];

                    //    }
                    //});

                    if (selectedFormInstanceID == "") {
                        messageDialog.show('Please select atleast one Document to generate the Collateral.');
                        return false;
                    }

                    var URL = URLs.checkPermission.replace(/\{ReportTemplateID\}/, selectedGenReportID).replace(/\{FolderVersionEffDt\}/, effDate);

                    var promise = ajaxWrapper.getJSON(URL);
                    promise.done(function (hasAccess) {
                        if (hasAccess == true) {
                            var url = URLs.viewReport;
                            var jsonObject = JSON.stringify(arr);
                            var accountID = $(elementIDs.genReportAccountID).val();
                            var folderID = $(elementIDs.genReportFolderID).val();
                            var folderVersionID = $(elementIDs.genReportFolderVersionID).val();
                            var effDate = $(elementIDs.genReportFolderVersionID + " option:selected").text();

                            var stringData = "accountID=" + accountID;
                            stringData += "<&folderID=" + folderID;
                            stringData += "<&formInstanceID=" + FormInstanceID;
                            //stringData += "<&dataSourceNames=" + DataSourceNames;
                            stringData += "<&folderVersionID=" + folderVersionID;
                            stringData += "<&reportTemplateName=" + selectedReportName;
                            //stringData += "<&reportTemplateLocation=" + selectedReportTemplateLocation;
                            stringData += "<&reportTemplateID=" + selectedGenReportID;
                            stringData += "<&folderVersionEffDt=" + effDate;
                            stringData += "<&description=" + description;
                            stringData += "<&formDesignVersionID=" + formDesignVersionID
                            stringData += "<&collateralOptions=" + selectedCollateralOptions;

                            $.download(url, stringData, 'post');
                        }
                        else {
                            messageDialog.show('You do not have permission to access this Collateral template.');
                        }
                    });
                }
            });

            $(elementIDs.queueCollateralBtn).on('click', function () {
                var accountID = selectedRowData.AccountID;
                var accountName = selectedRowData.AccountName;
                var folderID = selectedRowData.FolderID;
                var folderName = selectedRowData.FolderName;
                var versionNumber = selectedRowData.FolderVersionNumber;
                var folderVersionID = selectedRowData.FolderVersionID;
                var effDate = selectedRowData.FolderVersionEffectiveDate;


                var grid = $(elementIDs.relatedDocumentGridJQ);
                var gridRows = grid.jqGrid('getRowData');
                var arr = [];
                arr = gridRows;

                var formInstanceIDs = "", productIds = "", folderVersionNumber = "";

                $.each(arr, function (index) {
                    if (arr[index]['IsSelected']) {
                        formInstanceIDs += arr[index]['FormInstanceID'] + ",";
                        productIds += arr[index]['FormName'] + ",";
                        folderVersionNumber += arr[index]['FolderVersionNumber'] + ",";
                    }
                });

                var runDate = null;

                var url = URLs.queueCollateral.replace(/\{accountID\}/, accountID).replace(/\{accountName\}/, accountName).replace(/\{folderID\}/, folderID)
                          .replace(/\{folderName\}/, folderName).replace(/\{formInstanceIDs\}/, formInstanceIDs).replace(/\{folderVersionID\}/, folderVersionID)
                          .replace(/\{folderVersionNumbers\}/, folderVersionNumber).replace(/\{reportTemplateID\}/, selectedGenReportID)
                          .replace(/\{productIds\}/, productIds).replace(/\{folderVersionEffDt\}/, effDate).replace(/\{runDate\}/, runDate).replace(/\{ReportName\}/, selectedReportName);


                if (formInstanceIDs.length == 0) {
                    messageDialog.show('Please select atleast one Document to queue the Collateral.');
                    return false;
                }
                else {
                    var promise = ajaxWrapper.getJSON(url);
                    promise.done(function (xhr) {

                        if (xhr.Result === ServiceResult.SUCCESS) {
                            messageDialog.show(CollateralQueue.enQueuecollateral);
                            $('input[type=checkbox]').each(function () {
                                this.checked = false;
                            });
                        }
                        else
                            messageDialog.show(CollateralQueue.collateralQueueErrorMsg);
                    });
                }
            });

            $(elementIDs.uploadCollateralBtn).on('click', function () {
                complianceDialog1.show();
            });

            $(elementIDs.uploadSBDesignCollateralBtn).on('click', function () {
                complianceDialog1.show();
            });

            $(elementIDs.queueSBDesignCollateralBtn).on('click', function () {
                var formInstanceIDs = "", formNames = "", folderVersionNumbers = "", accountIDs = "", accountNames = "", folderIDs = ""; folderNames = "", folderVersionIDs = ""
                var folderVersionEffDts = "", formDesignVersionIDs = "", folderVersionNumbers = "", formDesignIds = "", formDesignVersionIds = "";
                var effectiveDate = null;
                var rows = $(elementIDs.selectedSourceDocumentGridJQ).jqGrid('getRowData');

                if (rows.length != 0) {
                    $.each(rows, function (index) {
                        formInstanceIDs += rows[index]['FormInstanceID'] + ",";
                        formNames += rows[index]['FormName'] + ",";
                        formDesignVersionIDs += rows[index]['FormDesignVersionID'] + ",";
                        accountIDs += rows[index]['AccountID'] + ",";
                        accountNames += rows[index]['AccountName'] + ",";
                        folderIDs += rows[index]['FolderID'] + ",";
                        folderNames += rows[index]['FolderName'] + ",";
                        folderVersionIDs += rows[index]['FolderVersionID'] + ",";
                        folderVersionEffDts += rows[index]['FolderVersionEffectiveDate'] + ",";
                        folderVersionNumbers += rows[index]['FolderVersionNumber'] + ",";
                        formDesignIds += rows[index]['FormDesignID'] + ",";
                        formDesignVersionIds += rows[index]['FormDesignVersionID'] + ",";
                    });
                    var selectedCollateralOptions = [];
                    if (webConfigCollateralOption.toLowerCase() == "both") {
                        selectedCollateralOptions = [];
                        $('input:checkbox[name=collateralOption]').each(function () {
                            if ($(this).is(':checked'))
                                selectedCollateralOptions.push($(this).val());
                        });
                    }
                    else
                        selectedCollateralOptions.push(webConfigCollateralOption);

                    if (webConfigCollateralOption.toLowerCase() == "both" && selectedCollateralOptions.length == 0) {
                        messageDialog.show('Please select atleast one Collateral Option.');
                        return false;
                    }

                    var url = URLs.queueSBDesignCollateral.replace(/\{accountID\}/, accountIDs).replace(/\{accountName\}/, accountNames).replace(/\{folderID\}/, folderIDs)
                              .replace(/\{folderName\}/, folderNames).replace(/\{formInstanceIDs\}/, formInstanceIDs).replace(/\{folderVersionID\}/, folderVersionIDs)
                              .replace(/\{folderVersionNumbers\}/, folderVersionNumbers).replace(/\{reportTemplateID\}/, selectedGenReportID)
                              .replace(/\{productIds\}/, formNames).replace(/\{folderVersionEffDt\}/, folderVersionEffDts).replace(/\{ReportName\}/, selectedReportName).replace(/\{formDesignID\}/, formDesignIds).replace(/\{formDesignVersionID\}/, formDesignVersionIds);

                    var promise = ajaxWrapper.getJSON(url);
                    promise.done(function (xhr) {
                        if (xhr.Result === ServiceResult.SUCCESS) {
                            messageDialog.show(CollateralQueue.enQueuecollateral);
                            $('input[type=checkbox]').each(function () {
                                this.checked = false;
                            });
                        }
                        else
                            messageDialog.show(CollateralQueue.collateralQueueErrorMsg);
                    });
                }
                else {
                    messageDialog.show('Please select atleast one Document to queue the Collateral.');
                    return false;
                }
            });

            $(elementIDs.viewAccDetailsDialogButn).on('click', function () {
                var accDetails = new viewAccountDetailsDialog(selectedGenReportID);
                accDetails.show();
            });

            $(elementIDs.generateCollateralBtn).on('click', function () {
                var formInstanceIDs = "", formNames = "", folderVersionNumbers = "", accountIDs = "", folderIDs = "", folderVersionIDs = "", folderVersionEffDts = "", formDesignVersionIDs = "", documentName = "";
                var effectiveDate = null;
                var rows = $(elementIDs.selectedSourceDocumentGridJQ).jqGrid('getRowData');

                if (rows.length != 0) {
                    $.each(rows, function (index) {
                        formInstanceIDs += rows[index]['FormInstanceID'] + ",";
                        formNames += rows[index]['FormName'] + ",";
                        formDesignVersionIDs += rows[index]['FormDesignVersionID'] + ",";
                        accountIDs += rows[index]['AccountID'] + ",";
                        folderIDs += rows[index]['FolderID'] + ",";
                        folderVersionIDs += rows[index]['FolderVersionID'] + ",";
                        folderVersionEffDts += rows[index]['FolderVersionEffectiveDate'] + ",";
                        effectiveDate = rows[index]['FolderVersionEffectiveDate'];
                    });
                    var selectedCollateralOptions = [];
                    if (webConfigCollateralOption.toLowerCase() == "both") {
                        selectedCollateralOptions = [];
                        $('input:checkbox[name=collateralOption]').each(function () {
                            if ($(this).is(':checked'))
                                selectedCollateralOptions.push($(this).val());
                        });
                    }
                    else
                        selectedCollateralOptions.push(webConfigCollateralOption);

                    if (webConfigCollateralOption.toLowerCase() == "both" && selectedCollateralOptions.length == 0) {
                        messageDialog.show('Please select atleast one Collateral Option.');
                        return false;
                    }

                    var URL = URLs.checkPermission.replace(/\{ReportTemplateID\}/, selectedGenReportID).replace(/\{FolderVersionEffDt\}/, effectiveDate);

                    var promise = ajaxWrapper.getJSON(URL, true);
                    promise.done(function (hasAccess) {
                        var sbDesignURL = URLs.generateCollateralSBDesign;                      
                        if (formInstanceIDs.trim().split(',').length <= 2)
                        {
                            var stringData = "accountIDs=" + accountIDs;
                            stringData += "<&folderIDs=" + folderIDs;
                            stringData += "<&formInstanceIDs=" + formInstanceIDs;
                            stringData += "<&formNames=" + formNames;
                            stringData += "<&folderVersionIDs=" + folderVersionIDs;
                            stringData += "<&reportTemplateName=" + selectedReportName;
                            stringData += "<&reportTemplateID=" + selectedGenReportID;
                            stringData += "<&folderVersionEffDts=" + folderVersionEffDts;
                            stringData += "<&formDesignVersionIDs=" + formDesignVersionIDs;
                            stringData += "<&collateralOptions=" + selectedCollateralOptions;
                            stringData += "<&rowCount=" + rows.length;
                            stringData += "<&isPDF=" + false;
                            $.download(sbDesignURL, stringData, 'post');
                        }
                        else
                        {
                            messageDialog.show("Only one SB report can be generated realtime. To generate multiple reports please use Collateral queue");
                        }

                    });

                }
                else
                    messageDialog.show("Please select at least one document");
            });

        });
    }

    // View Report Templates - Add and Edit Templates
    function viewReportTemplates() {
        //set column list for grid
        var colArray = ['Report Id', 'Collateral Name'];
        var currentInstance = this;

        //set column models
        var colModel = [];
        colModel.push({ name: 'ReportId', index: 'ReportId', key: true, hidden: true, search: false });
        colModel.push({ name: 'ReportName', index: 'ReportName', width: 220, search: true });
        //colModel.push({ name: 'Action', index: 'Action', width: '100px', align: 'center', sortable: false, search: false, editable: false, formatter: formatReportgen });

        //clean up the grid first - only table element remains after this
        $(elementIDs.reportTemplateGridJQ).jqGrid('GridUnload');

        var url = URLs.reportList;
        //adding the pager element
        $(elementIDs.reportTemplateGridJQ).parent().append("<div id='p" + elementIDs.reportTemplateGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.reportTemplateGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Collateral Template',
            height: '350',
            rowNum: 10000,
            ignoreCase: true,
            loadonce: false,
            autowidth: true,

            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.reportTemplateGrid,
            sortname: 'ReportId',
            altclass: 'alternate',
            onSelectRow: function (index) {
                var data = $(this).getRowData(index);
                selectedReportID = data.ReportId;
                selectedReportName = data.ReportName;
                viewReportTemplateVersion(selectedReportID, selectedReportName);
            },
            gridComplete: function () {
                //As per bug logged under EQN-1787, selection should reset to default after grid reload

                //if (selectedReportID)
                //    $(this).jqGrid('setSelection', selectedReportID);
                //else {
                rowIDs = $(this).getDataIDs();
                $(this).jqGrid('setSelection', rowIDs[0]);
                //}

                checkDocumentCollateralClaims(elementIDs, claims);
            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.reportTemplateGridJQ);
            }
        });

        var pagerElement = '#p' + elementIDs.reportTemplateGrid;
        //remove default buttons
        $(elementIDs.reportTemplateGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        //add button in footer of grid that pops up the add Report Template dialog
        $(elementIDs.reportTemplateGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-plus',
            title: 'Add', id: 'btnReportTemplateAdd',
            onClickButton: function () {
                //load Add Report dialog on click - see formDesignDialog function below
                reportTmplAddDialog.show('', 'add');
            }
        });
        //edit button in footer of grid that pops up the edit Report Template dialog
        $(elementIDs.reportTemplateGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-pencil',
            title: 'Edit', id: 'btnReportTemplateEdit',
            onClickButton: function () {

                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    var row = $(this).getRowData(rowId);
                    //load Document Design dialog for edit on click - see formDesignDialog function below
                    reportTmplAddDialog.show(row.ReportName, 'edit', currentInstance);
                }
            }
        });

        //delete button in footer of grid that pops up the delete form design dialog
        $(elementIDs.reportTemplateGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-trash',
            title: 'Delete', id: 'btnReportDesignDelete',
            onClickButton: function () {
                var rowId = $(this).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    //get the Report Design Version List for current report Design
                    var collatralVersionList = $(elementIDs.reportTemplateVersionGridJQ).getRowData();

                    if (collatralVersionList !== undefined && collatralVersionList.length > 0) {
                        messageDialog.show(ReportTemplate.repoertDeleteValidationMsg);
                    }
                    else {
                        //load confirm dialogue to asset the operation
                        confirmDialog.show(Common.deleteConfirmationMsg, function () {
                            confirmDialog.hide();

                            //delete the report design
                            var reportDesignDelete = {
                                tenantId: 1,
                                reportDesignId: rowId
                            };
                            var promise = ajaxWrapper.postJSON(URLs.reportDesignDelete, reportDesignDelete);
                            //register ajax success callback
                            promise.done(reportDesignDeleteSuccess);
                            //register ajax failure callback
                            promise.fail(showError);
                        });
                    }
                }

                else {
                    messageDialog.show(ReportTemplate.inProgressDesignSelectionMsg);
                }
            }
        });


        // add filter toolbar to the top
        $(elementIDs.reportTemplateGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: true,
            defaultSearch: "cn"
        });
    }

    //ajax callback success - reload Form Desing  grid
    function reportDesignDeleteSuccess(xhr) {

        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(ReportTemplate.deletereportDesign);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }
        viewReportTemplates();
    }

    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else

            messageDialog.show(JSON.stringify(xhr));
    }

    var reportTmplVersionDialogState = '';
    // View Report Template Version - Add and Edit Template Versions
    function viewReportTemplateVersion(ReportId, ReportName) {
        //set column list
        var colArray = ['ReportTemplateVersionID', 'Effective Date', 'Status', 'Version Number', 'Action', 'TemplateLocation', 'TemplateName'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'ReportTemplateVersionID', index: 'ReportTemplateVersionID', key: true, hidden: true, search: false, });
        colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', width: '315px', align: 'left', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions });
        colModel.push({ name: 'Status', index: 'Status', width: '315px', editable: false });
        colModel.push({ name: 'VersionNumber', index: 'VersionNumber', width: '315px', align: 'left', editable: false });
        colModel.push({ name: 'Action', index: 'Action', align: 'left', sortable: false, search: false, editable: false, formatter: editTmplVrsnFormatter });
        colModel.push({ name: 'TemplateLocation', TemplateLocation: 'IsReleased', width: '315px', align: 'left', editable: false, hidden: true });
        colModel.push({ name: 'TemplateName', index: 'TemplateName', width: '315px', align: 'left', editable: false, hidden: true });


        //get URL for grid
        var reportVersionListUrl = URLs.reportVersionList.replace(/\{TenantID\}/g, 1).replace(/\{ReportTemplateID\}/g, ReportId);

        //unload previous grid values
        $(elementIDs.reportTemplateVersionGridJQ).jqGrid('GridUnload');
        //add pager element
        $(elementIDs.reportTemplateVersionGridJQ).parent().append("<div id='p" + elementIDs.reportTemplateVersionGrid + "'></div>");
        //load grid - refer jqGrid documentation for details
        $(elementIDs.reportTemplateVersionGridJQ).jqGrid({
            url: reportVersionListUrl,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Collateral Template Version - ' + ReportName,
            forceFit: true,
            height: '350',
            autowidth: true,
            rowNum: 10000,
            ignoreCase: true,
            loadonce: false,
            ExpandColumn: 'Label',
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.reportTemplateVersionGrid,
            sortname: 'ReportTemplateVersionID',
            sortorder: 'desc',
            altclass: 'alternate',
            //register events to load the form design version in a new tab page
            //this is on click of the span which has the image in the last column
            gridComplete: function () {
                var rows = $(this).getDataIDs();
                var reverse = rows.reverse();
                for (index = 0; index < rows.length; index++) {
                    row = $(this).getRowData(rows[index]);
                    //if (row.Status != 'Finalized') {
                    $('#rptvrsn_' + row.ReportTemplateVersionID).click(function () {
                        var reportTemplateVersionID = this.id.replace(/rptvrsn_/g, '');
                        //launch a new tab here for this report version
                        //TODO: pass TenantId too
                        var currentRow = $(elementIDs.reportTemplateVersionGridJQ).getRowData(reportTemplateVersionID);
                        //increment to manage adding /deletesof tab pages
                        var tabName = ReportName + '-' + currentRow.VersionNumber;
                        $(elementIDs.btnReportTemplateEdit).addClass('ui-state-disabled');
                        $(elementIDs.btnReportTemplateDelete).addClass('ui-state-disabled');
                        //if report version is already loaded, do not load again but make it active
                        var foundIndex;
                        if (tabsReportVersion != undefined) {
                            tabsReportVersion.find('.ui-tabs-anchor').each(function (index, element) {
                                if ($(this).text() == tabName) {
                                    foundIndex = index;
                                }
                            });
                        }
                        else {
                            foundIndex = 0;
                        }
                        if (foundIndex > 0) {
                            $(".ui-tabs-anchor").trigger("click");
                        }
                        if (foundIndex > 0) {
                            tabsReportVersion.tabs('option', 'active', foundIndex);
                        }
                        else {
                            tabIndex++;
                            tabCount++;

                            var gridData = $(elementIDs.reportTemplateVersionGridJQ).getRowData();
                            var effDateForCurrentVersion = Date.parse(currentRow.EffectiveDate);
                            currentRow['canbeFinalized'] = true;           // A Report version cannot be finalized if an earlier version is in progress state.
                            for (var obj = 0; obj < gridData.length; obj++) {
                                if (gridData[obj].ReportTemplateVersionID != reportTemplateVersionID && Date.parse(gridData[obj].EffectiveDate) < effDateForCurrentVersion && gridData[obj].Status != 'Finalized') {
                                    currentRow['canbeFinalized'] = false;
                                    break;
                                }
                            };

                            var rptVersion = new reportConfigurationGrid(currentRow, ReportId, reportTemplateVersionID, currentRow.Status, tabNamePrefix + tabIndex, tabIndex - 1, tabCount, tabsReportVersion, ReportName);
                            rptVersion.loadTabPage();
                        }
                    });
                    //}
                }
                var rowId;
                if (rows.length > 0) {
                    rowId = rows[0];
                }

                //    //set first row 
                $(this).jqGrid('setSelection', rowId);
            },
            onSelectRow: function (rowID) {
                if (rowID !== undefined && rowID !== null) {
                    var row = $(this).getRowData(rowID);
                    if (row.Status == 'Finalized') {
                        $('#btnReportTemplateVersionEdit').addClass('ui-icon-hide');
                        $('#btnReportTemplateVersionDelete').addClass('ui-icon-hide');
                        $('#btnReportTemplateVersionFinalized').addClass('ui-icon-hide');
                    }
                    else {
                        $('#btnReportTemplateVersionEdit').removeClass('ui-icon-hide');
                        $('#btnReportTemplateVersionDelete').removeClass('ui-icon-hide');
                        $('#btnReportTemplateVersionFinalized').removeClass('ui-icon-hide');
                    }
                }
            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.reportTemplateVersionGridJQ);
            }
        });
        var pagerElement = '#p' + elementIDs.reportTemplateVersionGrid;
        //remove default buttons
        $(elementIDs.reportTemplateVersionGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        //add button in footer of grid that pops up the add Report Template dialog
        $(elementIDs.reportTemplateVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-plus',
            title: 'Add', id: 'btnReportTemplateVersionAdd',
            onClickButton: function () {
                var rows = $(this).getDataIDs();
                $(elementIDs.reportTmplVrsnDialog + ' select').empty();
                var showDialog = true;
                for (index = 0; index < rows.length; index++) {
                    row = $(this).getRowData(rows[index]);
                    $(elementIDs.reportTmplVrsnDialog + ' select').append(new Option(row.EffectiveDate + " - " + row.VersionNumber, row.ReportTemplateVersionID));
                    if (row.Status == "In Progress") {
                        showDialog = false;
                        break;
                    }
                }
                if (showDialog === true) {
                    //load Add Report version dialog on click - see formDesignDialog function below                
                    reportTmplVersionAddDialog.show(selectedReportID, 'add');
                }
                else {
                    //show message dialog
                    messageDialog.show(ReportTemplateVersion.reportTmplVrsnInProgress);
                }
            }
        });

        $(elementIDs.reportTemplateVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
       {
           caption: '', buttonicon: 'ui-icon-pencil',
           title: 'Edit', id: 'btnReportTemplateVersionEdit',
           onClickButton: function () {
               var recordCount = $(elementIDs.reportTemplateVersionGridJQ).getGridParam("reccount");
               if (recordCount > 0) {
                   //load Add Report dialog on click - see formDesignDialog function below   
                   var rowId = $(this).getGridParam('selrow');
                   if (rowId !== undefined && rowId !== null) {
                       var row = $(this).getRowData(rowId);
                       if (row.StatusId != 3)
                           reportTmplVersionAddDialog.show(selectedReportID, 'edit');
                   }
               }
               else
                   messageDialog.show(Common.noRecordsToDisplay);
           }
       });


        $(elementIDs.reportTemplateVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
      {
          caption: '', buttonicon: 'ui-icon-trash', title: 'Delete', id: 'btnReportTemplateVersionDelete',
          onClickButton: function () {
              var recordCount = $(elementIDs.reportTemplateVersionGridJQ).getGridParam("reccount");
              if (recordCount > 0) {
                  var rowId = $(this).getGridParam('selrow');
                  if (rowId !== undefined && rowId !== null) {
                      var row = $(this).getRowData(rowId);
                      var row = $(elementIDs.reportTemplateVersionGridJQ).getRowData(rowId);
                      if (row.Status == "In Progress") {
                          confirmDialog.show(Common.deleteConfirmationMsg, function () {
                              confirmDialog.hide();
                              var reportTemplateVersionDelete = {
                                  TenantId: 1,
                                  ReportTemplateVersionID: rowId,
                              };
                              var promise = ajaxWrapper.postJSON(URLs.reportTemplateVrsnDelete, reportTemplateVersionDelete);
                              //register ajax success callback
                              promise.done(reportTemplateVrsnDeleteSuccess);
                              //register ajax failure callback
                              promise.fail(showError);
                          });
                      }
                      else {
                          messageDialog.show(ReportTemplateVersion.reportTmplVrsnCannotDelete);
                      }
                  }
              }
              else
                  messageDialog.show(Common.noRecordsToDisplay);
          }
      });

        //add filter toolbar at the top of grid
        $(elementIDs.reportTemplateVersionGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: true,
            defaultSearch: "cn"
        });

        //button in footer of grid for finalizing a report Template Version
        $(elementIDs.reportTemplateVersionGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-locked', title: 'Finalized', id: 'btnReportTemplateVersionFinalized',
            onClickButton: function () {
                var rowId = $(elementIDs.reportTemplateVersionGridJQ).getGridParam('selrow');
                if (rowId != null) {
                    var row = $(elementIDs.reportTemplateVersionGridJQ).getRowData(rowId);
                    if (row.Status != 'Finalized') {
                        var url = URLs.IsTemplateUploaded.replace(/\{ReportTemplateVersionId\}/, row.ReportTemplateVersionID)
                        var promise = ajaxWrapper.getJSON(url);
                        promise.done(function (result) {
                            if (result == true) {
                                //show confirm dialog
                                confirmDialog.show(DocumentDesign.confirmFinalizationMsg, function () {
                                    confirmDialog.hide();
                                    commentsDialog.show(row.VersionNumber);
                                })
                            }
                            else
                                messageDialog.show(ReportTemplateVersion.reportTmplVrsnCanNotFinalize);
                        });
                    }
                    else {
                        messageDialog.show(ReportTemplate.inProgressDesignSelectionMsg);
                    }
                }
                else {
                    messageDialog.show(ReportTemplate.inProgressDesignSelectionMsg);
                }
            }
        });

    }

    function reportTemplateVrsnDeleteSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(ReportTemplateVersion.reportTmplVrsnDeleted);
        }
        else {
            messageDialog.show(ReportTemplateVersion.reportTmplVrsnErrorMsg);
        }
        var rowId = $(elementIDs.reportTemplateGridJQ).getGridParam('selrow');
        var row = $(elementIDs.reportTemplateGridJQ).getRowData(rowId);
        viewReportTemplateVersion(rowId, row.ReportName);
    }

    function populateAccFldrFldrVrsnDrpDwn(result, accountID, folderID, folderVersionID) {
        //var accures = '';
        //var accounts = [];   
        try {
            var acc = $(elementIDs.genReportAccountID);
            var account = $(elementIDs.genReportAccountID);
            var folder = $(elementIDs.genReportFolderID);
            var folderVersion = $(elementIDs.genReportFolderVersionID);
            var defaultOption = $('<option></option>').val('').html('Select');


            if (folderVersionID) {                                // If FolderVersionID comes in then display the documents and do nothing with the dropdowns.
                // Do Nothing
                var doNothing = '';
            }
            else if (folderID) {                                  // If FolderID comes in that means FolderVersion DropDown is to be populated with values.
                var _selectFolderVersion = $('<select>');
                folderVersion.empty();
                _selectFolderVersion.append(defaultOption);

                $.each(result, function (i, obj) {
                    if (_selectFolderVersion.find('option[value="' + obj.FolderVersionID + '"]').length == 0) {
                        _optFldrVrsn = $('<option></option>').val(obj.FolderVersionID).html(obj.FolderVersionEffectiveDate);
                        _selectFolderVersion.append(_optFldrVrsn);
                    }
                });
                folderVersion.html(_selectFolderVersion.html());
            }
            else if (accountID) {                                // If AccountID comes in that means FolderID DropDown is to be populated.
                var _selectFolder = $('<select>');
                folder.empty();
                folderVersion.empty();
                _selectFolder.append(defaultOption);

                $.each(result, function (i, obj) {
                    if (_selectFolder.find('option[value="' + obj.FolderID + '"]').length == 0) {
                        _optFldr = $('<option></option>').val(obj.FolderID).html(obj.FolderName);
                        _selectFolder.append(_optFldr);
                    }
                });
                folder.html(_selectFolder.html());
            }
            else {                                              // If Nothing comes in that means AccountID DropDown is to be populated.
                var _selectAccount = $('<select>');
                account.empty();
                folder.empty();
                folderVersion.empty();
                _selectAccount.append(defaultOption);

                $.each(result, function (i, obj) {
                    if (_selectAccount.find('option[value="' + obj.AccountID + '"]').length == 0) {
                        _optAcc = $('<option></option>').val(obj.AccountID).html(obj.AccountName);
                        _selectAccount.append(_optAcc);
                    }
                });
                account.html(_selectAccount.html());
            }
        }
        catch (ex) {
            var exception = ex;
        }
    }

    var AccFldrFldrVrsn = [];

    // Populates the Dropdown values, used for Report generation argument selection.
    function getAccountsFolders(selectedReportTemplateID, accountID, folderID, folderVersionID) {
        //var url = URLs.getAccFolderdocumentsForReport.replace(/\{ReportTemplateID\}/, selectedReportTemplateID).replace(/\{AccountID\}/, accountID).replace(/\{FolderID\}/, folderID).replace(/\{FolderVersionID\}/, folderVersionID);//  '/DocumentCollateral/GetAccountsFolders?reportTemplateVersionID=' + selectedReportTemplateVersionID + "&AccountID=" + account;
        ////var promise = ajaxWrapper.getJSON(url);

        ////promise.done(function (result) {
        ////    AccFldrFldrVrsn = result;
        ////    populateAccFldrFldrVrsnDrpDwn(result, accountID, folderID, folderVersionID);
        ////});

        if (!accountID) {

            var url = URLs.getAccFolderdocumentsForReport.replace(/\{ReportTemplateID\}/, selectedReportTemplateID).replace(/\{AccountID\}/, accountID).replace(/\{FolderID\}/, folderID).replace(/\{FolderVersionID\}/, folderVersionID);//  '/DocumentCollateral/GetAccountsFolders?reportTemplateVersionID=' + selectedReportTemplateVersionID + "&AccountID=" + account;
            var promise = ajaxWrapper.getJSON(url);

            promise.done(function (result) {
                AccFldrFldrVrsn = result;
                populateAccFldrFldrVrsnDrpDwn(result);
            });
        }
        else {
            var filteredResult = [];
            if (folderVersionID)
                filteredResult = AccFldrFldrVrsn.filter(function (obj, index) { return obj.FolderVersionID == folderVersionID });
            else if (folderID)
                filteredResult = AccFldrFldrVrsn.filter(function (obj, index) { return obj.FolderID == folderID });
            else if (accountID)
                filteredResult = AccFldrFldrVrsn.filter(function (obj, index) { return obj.AccountID == accountID });


            //if (folderVersionID)
            //    filteredResult = getFilteredList('FolderVersionID', folderVersionID); //AccFldrFldrVrsn.filter(function (obj, index) { return obj.FolderVersionID == folderVersionID });
            //else if (folderID)
            //    filteredResult = getFilteredList('FolderID', folderID); //AccFldrFldrVrsn.filter(function (obj, index) { return obj.FolderID == folderID });
            //else if (accountID)
            //    filteredResult = getFilteredList('AccountID', accountID);  //AccFldrFldrVrsn.filter(function (obj, index) { return obj.AccountID == accountID });

            populateAccFldrFldrVrsnDrpDwn(filteredResult, accountID, folderID, folderVersionID);
        }
    };

    //Loads grid data for the available Form Instances for the Report generation based on selected Report, account & folderVersion.
    function selectDocumentsForReportGeneration(ReportTemplateID, AccountID, FolderID, FolderVersionID, gridName, gridId) {
        $(elementIDs.genReportButton).show();
        //set column list for grid
        var colArray = ['AccountID', 'Account Name', 'Document Type', 'FolderID', 'Folder Name', 'Document Name', 'FolderVersionID', 'Version Number', 'Effective Date', 'FormInstanceID', 'Data Source Name', 'FormDesignVersionID', 'Action'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'AccountID', index: 'AccountID', hidden: true, search: false, });
        colModel.push({ name: 'AccountName', index: 'AccountName', search: true, });
        colModel.push({ name: 'DocumentType', index: 'DocumentType', search: true, });
        colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true, search: false, });
        colModel.push({ name: 'FolderName', index: 'FolderName', search: true, hidden: true });
        colModel.push({ name: 'FormName', index: 'FormName', search: true, });
        colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', search: false, hidden: true });
        colModel.push({ name: 'FolderVersionNumber', index: 'FolderVersionNumber', search: true, });
        colModel.push({ name: 'FolderVersionEffectiveDate', index: 'FolderVersionEffectiveDate', search: true, hidden: true });
        colModel.push({ name: 'FormInstanceID', index: 'FormInstanceID', key: true, hidden: true, search: true, });
        colModel.push({ name: 'DataSourceName', index: 'DataSourceName', hidden: true, search: false, });
        colModel.push({ name: 'FormDesignVersionID', index: 'FormDesignVersionID', hidden: true, search: false, });

        colModel.push({
            name: 'IsSelected', index: 'IsSelected', width: '100px', align: 'left', sortable: false, search: false, editable: false, formatter: selectDocumentFormatter, unformat: function selectDocumentUnFormatter(cellValue, options) {
                // As we are using same function to render this grid at different locations, use different IDs for checkbox column
                var checkBoxId;
                if (gridId == elementIDs.collateralQueueDocumentsGridJQ)
                    checkBoxId = "chkBox_";
                else
                    checkBoxId = "sel_";
                var elementId = '#' + checkBoxId + options.rowId;
                return $(elementId).prop('checked');
            }
        });

        //clean up the grid first - only table element remains after this
        $(gridId).jqGrid('GridUnload');

        var url = URLs.getAccFolderdocumentsForReport.replace(/\{ReportTemplateID\}/g, ReportTemplateID).replace(/\{AccountID\}/g, AccountID).replace(/\{FolderID\}/g, FolderID).replace(/\{FolderVersionID\}/g, FolderVersionID);

        //adding the pager element
        $(gridId).parent().append("<div id='p" + gridName + "'></div>");
        //load the jqGrid - refer documentation for details
        $(gridId).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Select Documents',
            height: '180',
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,                    // for Grid Refresh ( setting [loadonce: true] means Grid Refresh isn't needed)
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + gridName,
            sortname: 'FormInstanceID',
            altclass: 'alternate',
            gridComplete: function () {
                $(elementIDs.genReportButton).show();
                $(gridId).show();
            },

            resizeStop: function (width, index) {
                autoResizing(gridId);
            }
        });

        var pagerElement = '#p' + gridName;
        //remove default buttons
        $(gridId).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();

        // add filter toolbar to the top
        $(gridId).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });
    }

    //Load the DropDown for the available Form Instance for the report geberation bases on selected Teport, account & Folderversion
    function populateDocumentsForReportGeneration(ReportTemplateID, AccountID, FolderID, FolderVersionID, gridName, gridId) {
        $(elementIDs.genReportButton).show();

        //Hiding the Grid
        $(elementIDs.relatedDocumentGrid).hide();
        //Showing the drop down
        $(elementIDs.selectDocumentOptTable).show();
        $(elementIDs.selectDocumentDrpDown).show();
        $(elementIDs.selectDocumentLabel).show();
        try {

            var defaultOption = $('<option></option>').val('').html('Select');
            var _selectDocument = $('<select>');
            _selectDocument.append(defaultOption);

            var documentDropDown = $(elementIDs.selectDocumentDrpDown);

            var url = URLs.getAccFolderdocumentsForReport.replace(/\{ReportTemplateID\}/g, ReportTemplateID).replace(/\{AccountID\}/g, AccountID).replace(/\{FolderID\}/g, FolderID).replace(/\{FolderVersionID\}/g, FolderVersionID);
            var promise = ajaxWrapper.getJSON(url);
            promise.done(function (result) {
                ReportsResults = result;
                $.each(result, function (i, obj) {
                    if (_selectDocument.find('option[value="' + obj.FormInstanceID + '"]').length == 0) {
                        _optFldrVrsn = $('<option></option>').val(obj.FormInstanceID).html(obj.FormName);
                        _selectDocument.append(_optFldrVrsn);
                    }
                });

                documentDropDown.html(_selectDocument.html());
            });
        }
        catch (ex) {
            var exception = ex;
        }
    }

    //generate Report for the specified datasources
    function generateReports() {
        //set column list for grid
        var colArray = ['Report ID', 'Collateral Name']; //, 'Report Location', 'ReportTemplateVersionID', 'Version No'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'ReportId', index: 'ReportId', key: true, hidden: true, search: false });
        colModel.push({ name: 'ReportName', index: 'ReportName', width: 220, search: true });
        //colModel.push({ name: 'ReportTemplateLocation', index: 'ReportTemplateLocation', hidden: true, search: true }); 
        //colModel.push({ name: 'ReportTemplateVersionID', index: 'ReportTemplateVersionID', hidden: true, search: true });
        //colModel.push({ name: 'VersionNumber', index: 'VersionNumber', search: true, hidden: true });
        //colModel.push({ name: 'Action', index: 'Action', width: '100px', align: 'center', sortable: false, search: false, editable: false, formatter: formatReportgen });

        //clean up the grid first - only table element remains after this
        $(elementIDs.reportGridJQ).jqGrid('GridUnload');

        var url = URLs.reportListGen;
        //adding the pager element
        $(elementIDs.reportGridJQ).parent().append("<div id='p" + elementIDs.reportGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.reportGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Collateral',
            height: '350',
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
            autowidth: true,

            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.reportGrid,
            sortname: 'ReportId',
            altclass: 'alternate',
            onSelectRow: function (index) {
                var data = $(this).getRowData(index);
                //$(elementIDs.genReportDropDownOptions).show();
                $(elementIDs.divAccountDetails).show();
                $(elementIDs.sourceDocument).hide();
                $(elementIDs.queueCollateralBtn).hide();
                $(elementIDs.uploadCollateralBtn).hide();
                $(elementIDs.divCollateralOptions).hide();
                selectedReportName = data.ReportName;
                selectedGenReportID = data.ReportId;
                //selectedReportTemplateVersionID = data.ReportTemplateVersionID;
                //selectedReportTemplateLocation = data.ReportTemplateLocation;
                //$('#accountDropDown').empty();
                $(elementIDs.relatedDocumentGridJQ).jqGrid('GridUnload');
                $(elementIDs.selectDocumentDrpDown).hide();
                $(elementIDs.selectDocumentLabel).hide();
                $(elementIDs.selectDocumentOptTable).hide();
                $(elementIDs.genReportButton).hide();

                var url = URLs.checkIfCollateralIsOfSBDesignType + selectedGenReportID;
                var promise = ajaxWrapper.getJSON(url);
                promise.done(function (isSBDesignProduct) {
                    $(elementIDs.planFamilyJQ).empty();
                    $(elementIDs.planFamilyJQ).append('<option value="">Select One</option>');                    
                    if (isSBDesignProduct=="EOC") {
                        $(elementIDs.divGenerateReport).hide();
                        $(elementIDs.divGenerateSBReport).show();
                        $(elementIDs.planFamilyDivJQ).show();

                        if ($(elementIDs.selectedSourceDocumentGridJQ)[0].grid) {
                            $(elementIDs.selectedSourceDocumentGridJQ).jqGrid('clearGridData');
                            selectedSourceDocuments = [];
                        }

                        var url = URLs.getPlanFamilyDropdownList.replace(/\{templateReportID\}/, selectedGenReportID).replace(/\{reportName\}/, selectedReportName);

                        var promise = ajaxWrapper.getJSON(url);
                        promise.done(function (options) {                            
                            if (options.length > 0) {                                
                                $.each(options, function (idx, dt) {
                                    $(elementIDs.planFamilyJQ).append('<option value="' + dt + '">' + dt + '</option>');
                                })
                                reportTemplate.loadSourceDocumentList();                               
                            }                            
                        });
                    }
                    else if (isSBDesignProduct == "SB") {
                        if ($(elementIDs.selectedSourceDocumentGridJQ)[0].grid) {
                            $(elementIDs.selectedSourceDocumentGridJQ).jqGrid('clearGridData');
                            selectedSourceDocuments = [];
                        }                                            
                        $(elementIDs.planFamilyDivJQ).hide();
                        $(elementIDs.divGenerateReport).hide();
                        $(elementIDs.divGenerateSBReport).show();
                        reportTemplate.loadSourceDocumentList();
                    }
                    else {                                              
                        $(elementIDs.planFamilyDivJQ).hide();
                        $(elementIDs.divGenerateReport).show();
                        $(elementIDs.divGenerateSBReport).hide();
                    }
                });
                //getAccountsFolders(selectedGenReportID, 0, 0, 0);
                //selectDocumentsForReportGeneration(data.ReportName,0,0);
            },

            resizeStop: function (width, index) {
                autoResizing(elementIDs.reportGridJQ);
            }
        });

        var pagerElement = '#p' + elementIDs.reportGrid;
        //remove default buttons
        $(elementIDs.reportGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();

        // add filter toolbar to the top
        $(elementIDs.reportGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });

        $(elementIDs.planFamilyJQ).on('change', function () {
            if ($(elementIDs.selectedSourceDocumentGridJQ)[0].grid) {
                $(elementIDs.selectedSourceDocumentGridJQ).jqGrid('clearGridData');
                selectedSourceDocuments = [];
            }
            reportTemplate.loadSourceDocumentList();
        });
    }

    function loadSourceDocumentGrid() {
        var colArray = ['Account', 'Folder', 'Folder Version Number', 'Effective Date', 'Document Name', '', '', '', '', '', '', ''];
        var colModel = [];
        colModel.push({ name: 'AccountName', index: 'AccountName', editable: false, align: 'left', width: 200 });
        colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left' });
        colModel.push({ name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: false, align: 'left' });
        colModel.push({ name: 'FolderVersionEffectiveDate', index: 'FolderVersionEffectiveDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'left' });
        colModel.push({ name: 'FormName', index: 'FormName', editable: false, align: 'left' });
        colModel.push({ name: 'AccountID', index: 'AccountID', hidden: true });
        colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true });
        colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', hidden: true });
        colModel.push({ key: true, name: 'FormInstanceID', index: 'FormInstanceID', hidden: true });
        colModel.push({ key: true, name: 'AnchorDocumentName', index: 'AnchorDocumentName', hidden: true });
        colModel.push({ name: 'FormDesignID', index: 'FormDesignID', hidden: true, search: false, });
        colModel.push({ name: 'FormDesignVersionID', index: 'FormDesignVersionID', hidden: true, search: false, });

        $(elementIDs.sourceDocumentGridJQ).jqGrid('GridUnload');
        $(elementIDs.sourceDocumentGridJQ).parent().append("<div id='p" + elementIDs.sourceDocumentGrid + "'></div>");
        var planFamilyValue = $(elementIDs.planFamilyJQ).val();
        var url = URLs.getAccountsAndFolders.replace(/\{ReportTemplateID\}/, selectedGenReportID).replace(/\{AccountID\}/, 0).replace(/\{FolderID\}/, 0).replace(/\{FolderVersionID\}/, 0).replace(/\{reportName}/, selectedReportName).replace(/\{planFamilyName}/, planFamilyValue);;
        var pagerElement = '#p' + elementIDs.sourceDocumentGrid;
        var anchorDocumentName = 'Medicare';

        $(elementIDs.sourceDocumentGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            postData: {
                filters: '{"groupOp":"AND","rules":[' +
                        '{"field":"AnchorDocumentName","op":"eq","data":"' + anchorDocumentName + '"}]}' // Apply filter while loading grid
            },
            mtype: 'POST',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Select Documents',
            height: '150',
            rowNum: 20,
            rowList: [10, 20, 30],
            ignoreCase: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: pagerElement,
            sortname: 'AccountName',
            altclass: 'alternate'
        });
        //remove default buttons
        $(elementIDs.sourceDocumentGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.sourceDocumentGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
        $(elementIDs.sourceDocumentGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-plus', title: 'Add',
               onClickButton: function () {
                   var selectedRowId = $(elementIDs.sourceDocumentGridJQ).jqGrid('getGridParam', 'selrow');
                   if (selectedRowId == null) {
                       messageDialog.show("Please select a document.");
                       return;
                   }
                   var rowData = $(elementIDs.sourceDocumentGridJQ).jqGrid('getRowData', selectedRowId);
                   var rowIDs = $(elementIDs.selectedSourceDocumentGridJQ).jqGrid("getDataIDs");
                   if (rowIDs.length >= 3) {
                       messageDialog.show("Max 3 documents can be added at a time.");
                       return;
                   }
                   var matchID = rowIDs.filter(function (rowID) {
                       return selectedRowId == rowID;
                   });
                   if (matchID.length > 0) {
                       messageDialog.show(rowData.FormName + " for Folder Version Number " + rowData.FolderVersionNumber + " is already added for queuing.");
                       return;
                   }
                   else {
                       selectedSourceDocuments.push(rowData);
                       loadSelectedDocumentsGrid();
                       webConfigCollateralOption = $(elementIDs.divGenerateSBReport + ' ' + elementIDs.hiddenCollateralOptionVal).val();
                       if (webConfigCollateralOption.toLowerCase() == "both") {
                           $(elementIDs.divGenerateSBReport + ' ' + elementIDs.divCollateralOptions).show();
                           $(elementIDs.uploadSBDesignCollateralBtn).css('margin-left', '320px');
                           $(elementIDs.generateCollateralBtn).css('margin-left', '250px');
                           $(elementIDs.queueSBDesignCollateralBtn).css('margin-left', '190px');
                       }
                       $(elementIDs.generateCollateralBtn).show();
                       $(elementIDs.queueSBDesignCollateralBtn).show();
                       $(elementIDs.uploadSBDesignCollateralBtn).show();
                   }
               }
           });
    }

    function loadSelectedDocumentsGrid() {
        if (selectedSourceDocuments != null) {
            var colArray = ['Account', 'Folder', 'Folder Version Number', 'Effective Date', 'Document Name', 'AccountID', 'FolderID', 'FolderVersionID', 'FormInstanceID', 'FormDesignID', 'FormDesignVersionID'];
            var colModel = [];
            colModel.push({ name: 'AccountName', index: 'AccountName', editable: false, align: 'left' });
            colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left' });
            colModel.push({ name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: false, align: 'left' });
            colModel.push({ name: 'FolderVersionEffectiveDate', index: 'FolderVersionEffectiveDate', editable: false, align: 'left' });
            colModel.push({ name: 'FormName', index: 'FormName', editable: false, align: 'left' });
            colModel.push({ name: 'AccountID', index: 'AccountID', hidden: true });
            colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true });
            colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', hidden: true });
            colModel.push({ key: true, name: 'FormInstanceID', index: 'FormInstanceID', hidden: true });
            colModel.push({ name: 'FormDesignID', index: 'FormDesignID', hidden: true, search: false, });
            colModel.push({ name: 'FormDesignVersionID', index: 'FormDesignVersionID', hidden: true, search: false, });

            $(elementIDs.selectedSourceDocumentGridJQ).jqGrid('GridUnload');
            $(elementIDs.selectedSourceDocumentGridJQ).parent().append("<div id='p" + elementIDs.selectedSourceDocumentGrid + "'></div>");
            var pagerElement = '#p' + elementIDs.selectedSourceDocumentGrid;

            $(elementIDs.selectedSourceDocumentGridJQ).jqGrid({
                datatype: 'local',
                cache: false,
                colNames: colArray,
                colModel: colModel,
                caption: 'Selected Documents',
                height: '100',
                data: selectedSourceDocuments,
                autowidth: true,
                rowNum: 20,
                rowList: [10, 20, 50],
                ignoreCase: true,
                autowidth: true,
                viewrecords: true,
                hidegrid: false,
                altRows: true,
                pager: pagerElement,
                sortname: 'FormInstanceName',
                altclass: 'alternate'
            });
            //remove default buttons
            $(elementIDs.selectedSourceDocumentGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
            $(elementIDs.selectedSourceDocumentGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
            $(elementIDs.selectedSourceDocumentGridJQ).jqGrid('navButtonAdd', pagerElement,
                {
                    caption: '', buttonicon: 'ui-icon-minus', title: 'Remove',
                    onClickButton: function () {
                        var selectedRowId = $(elementIDs.selectedSourceDocumentGridJQ).jqGrid('getGridParam', 'selrow');
                        if (selectedRowId == null) {
                            messageDialog.show("Please select a document to remove");
                            return;
                        }
                        var rowData = $(elementIDs.selectedSourceDocumentGridJQ).jqGrid('getRowData', selectedRowId);
                        var rowIDs = $(elementIDs.sourceDocumentGridJQ).jqGrid("getDataIDs");
                        var matchID = rowIDs.filter(function (rowID) {
                            return selectedRowId == rowID;
                        });
                        $(elementIDs.selectedSourceDocumentGridJQ).delRowData(selectedRowId);
                        if (matchID.length == 0) {
                            $(elementIDs.sourceDocumentGridJQ).jqGrid('addRowData', selectedRowId, rowData);
                        }
                        selectedSourceDocuments = $(elementIDs.selectedSourceDocumentGridJQ).jqGrid('getRowData');
                        $(elementIDs.selectedSourceDocumentGridJQ).trigger("reloadGrid");
                    }
                });
        }
    }
    init();


    // Column Formatters
    function editTmplVrsnFormatter(cellValue, options, rowObject) {
        var idRptVrsn = "rptvrsn_" + rowObject.ReportTemplateVersionID;
        var id = $(this).attr('id');
        var rows = $('#rptTmplVersions').jqGrid('getRowData');
        if (rowObject.Status == 'Finalized') {
            return "<span id = '" + idRptVrsn + "' class='ui-icon ui-icon-document view' title = 'View Template' style ='cursor: pointer'/>";
        }
        else {
            return "<span id = '" + idRptVrsn + "' class='ui-icon ui-icon-pencil edit' title = 'Edit Template' style = 'cursor: pointer'/>";
        }
    }

    function selectDocumentFormatter(cellValue, options, rowObject) {
        // As we are using same function to render this grid at different locations, use different IDs for checkbox column.
        var checkBoxId;
        if (options.gid == elementIDs.collateralQueueDocumentsGrid)
            checkBoxId = "chkBox_";
        else
            checkBoxId = "sel_";
        var id = checkBoxId + rowObject.FormInstanceID;
        return "<input id = '" + id + '\'' + (rowObject.IsSelected == 'true' ? ' checked="checked" readonly=\'true\' class=\'ui-state-disabled\' ' : '') + " type='checkbox' title = 'Select' />";   //  style = 'cursor: pointer'
    }

    return {
        viewReportTemplates: function () {
            viewReportTemplates();
        },
        viewReportTemplateVersion: function () {
            var rowId = $(elementIDs.reportTemplateGridJQ).getGridParam('selrow');
            var row = $(elementIDs.reportTemplateGridJQ).getRowData(rowId);
            viewReportTemplateVersion(rowId, row.ReportName);
        },
        selectedReportID: function (valueToSet) {
            if (valueToSet)
                selectedReportID = valueToSet;
            return selectedReportID;
        },
        selectedReportName: function () {
            return selectedReportName;
        },
        selectedReportTemplateVersionID: function () {
            return selectedReportTemplateVersionID;
        },
        loadReportDesignTemplategrid: function () {
            loadReportDesignTemplategrid();
        },
        loadSourceDocumentList: function () {
            loadSourceDocumentGrid();
        }
    }
}();

var reportTmplAddDialog = function () {
    var URLs = {
        //url for Add Report Template
        reportTemplateAdd: '/DocumentCollateral/AddReportTemplate',
        //url for Update Report Template
        reportTemplateUpdate: '/DocumentCollateral/UpdateReportTemplate'
    }

    //see element id's in Views\FormDesign\Index.cshtml
    var elementIDs = {
        //form design grid element
        reportTemplateGrid: 'rptTmplNames',
        //with hash for jquery
        reportTemplateGridJQ: '#rptTmplNames',
        //form design dialog element
        reportTmplDialog: "#reportTmplDialog"
    };

    //maintains dialog state - add or edit
    var reportTemplateDialogState;

    //ajax success callback - for add/edit
    function reportTemplateSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            if (reportTemplateDialogState === 'edit')
                messageDialog.show(ReportTemplate.reportTmplUpdated);
            else {
                reportTemplate.selectedReportID(xhr.Items[0].Messages[0]);
                messageDialog.show(ReportTemplate.reportTmplAdded);
            }

        }
        else {
            messageDialog.show(ReportTemplate.reportTmplErrorMsg);
        }
        //reload the Report Template grid

        reportTemplate.viewReportTemplates();

        //reset dialog elements
        $(elementIDs.reportTmplDialog + ' div').removeClass('has-error');
        $(elementIDs.reportTmplDialog + ' .help-block').text(ReportTemplate.reportTemplateEditNameValidationMsg);
        $(elementIDs.reportTmplDialog).dialog('close');
    }
    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/DocumentCollateral/ViewReportTemplate';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
    //init dialog on load of page
    function init() {
        //register dialog for grid row add/edit
        $(elementIDs.reportTmplDialog).dialog({
            autoOpen: false,
            height: 250,
            width: 450,
            modal: true
        });
        //register event for Add/Edit button click on dialog
        $(elementIDs.reportTmplDialog + ' button').on('click', function () {
            //check if name is already used
            var newName = $(elementIDs.reportTmplDialog + ' input').val();
            var reportTemplateList = $(elementIDs.reportTemplateGridJQ).getRowData();

            var filterList = reportTemplateList.filter(function (ct) {
                return compareStrings($.trim(ct.ReportName), $.trim(newName), true);
            });

            //validate form name
            if (filterList !== undefined && filterList.length > 0) {
                $(elementIDs.reportTmplDialog + ' div').addClass('has-error');
                $(elementIDs.reportTmplDialog + ' .help-block').text(ReportTemplate.reportTemplateNameDuplicationMsg);
            }
            else if (newName == '') {
                $(elementIDs.reportTmplDialog + ' div').addClass('has-error');
                $(elementIDs.reportTmplDialog + ' .help-block').text(ReportTemplate.reportTemplateNameRequiresValidationMsg);
            }
            else {
                //save the new report Template
                var rowId = $(elementIDs.reportTemplateGridJQ).getGridParam('selrow');
                var reportTemplateAdd = {
                    TenantID: 1,
                    ReportTemplateID: (reportTemplateDialogState === 'add' ? -1 : rowId),
                    ReportName: newName
                };
                var url;
                if (reportTemplateDialogState === 'add') {
                    url = URLs.reportTemplateAdd;
                }
                else {
                    url = URLs.reportTemplateUpdate;
                }
                //ajax call to add/update
                var promise = ajaxWrapper.postJSON(url, reportTemplateAdd);
                //register ajax success callback
                promise.done(reportTemplateSuccess);
                //register ajax failure callback
                promise.fail(showError);
            }
        });
    }
    //initialize the dialog after this js are loaded
    init();

    //these are the properties that can be called by using reportTmplDialog.<Property>
    //eg. reportTmplDialog.show('name','add');
    return {
        show: function (ReportName, action) {

            reportTemplateDialogState = action;

            $(elementIDs.reportTmplDialog + ' input').each(function () {
                $(this).val(ReportName);
            });
            //set eleemnts based on whether the dialog is opened in add or edit mode
            $(elementIDs.reportTmplDialog + ' div').removeClass('has-error');
            if (reportTemplateDialogState == 'add') {
                $(elementIDs.reportTmplDialog).dialog('option', 'title', ReportTemplate.addReportTemplate);
                $(elementIDs.reportTmplDialog + ' .help-block').text(ReportTemplate.reportTmplAddNewNameValidateMsg);
            }
            else {
                $(elementIDs.reportTmplDialog).dialog('option', 'title', ReportTemplate.editReportTemplate);
                $(elementIDs.reportTmplDialog + ' .help-block').text(ReportTemplate.reportTmplEditNameValidateMsg);
            }

            //open the dialog - uses jqueryui dialog
            $(elementIDs.reportTmplDialog).dialog("open");

        }
    }
}(); //invoked soon after loading

var reportTmplVersionAddDialog = function () {
    var URLs = {
        //url for Adding Report Template Version
        reportTemplateVrsnAdd: '/DocumentCollateral/AddReportTemplateVersion',
        reportTemplateVrsnUpdate: '/DocumentCollateral/EditReportTemplateVersion'
    }


    var reportTemplateID;
    //see element id's in Views\FormDesign\Index.cshtml
    var elementIDs = {
        reportTemplateGrid: 'rptTmplNames',
        reportTemplateGridJQ: '#rptTmplNames',
        //form design grid element
        reportTemplateVersionGrid: 'rptTmplVersions',
        //with hash for jquery
        reportTemplateVersionGridJQ: '#rptTmplVersions',
        //form design dialog element
        reportTmplVrsnDialog: "#reportTmplVrsnDialog",

        reportTmplVrsnEffDate: "#reportTmplVrsnEffDate",
        copyCollateralTemplateVersionCheckboxHelpblock: "#copyCollateralTemplateVersionCheckboxHelpblock",
        chkbox: 'createCollateralTemplateVersion',
        divReportTmplVrsnAdd: "#divReportTmplVrsnAdd",
        //dropDown: "#copyPreviousTemplateVersion",
    };

    //ajax success callback - for add
    function reportTemplateVrsnSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            if (reportTmplVersionDialogState === 'add')
                messageDialog.show(ReportTemplateVersion.reportTmplVrsnAdded);
            else
                messageDialog.show(ReportTemplateVersion.reportTmplVrsnUpdated);
        }
        else {
            messageDialog.show(ReportTemplateVersion.reportTmplVrsnErrorMsg);
        }
        //reload the Report Template grid 
        //reportTemplate.viewReportTemplateVersion();
        $(elementIDs.reportTemplateGridJQ).jqGrid('setSelection', reportTemplateID);

        //reset dialog elements
        $(elementIDs.reportTmplVrsnDialog + ' div').removeClass('has-error');
        $(elementIDs.reportTmplVrsnDialog + ' .help-block').text(ReportTemplateVersion.reportTmplVrsnEffDateRequiredValidationMsg);
        $(elementIDs.reportTmplVrsnDialog).dialog('close');
    }
    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/DocumentCollateral/ViewReportTemplate';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
    //init dialog on load of page
    function init() {
        //register dialog for grid row add/edit
        $(elementIDs.reportTmplVrsnDialog).dialog({
            autoOpen: false,
            height: 320,
            width: 400,
            modal: true
        });

        $(elementIDs.reportTmplVrsnEffDate).datepicker({
            dateFormat: 'mm/dd/yy',
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-61:c+20',
            showOn: "both",
            //CalenderIcon path declare in golbalvariable.js
            buttonImage: Icons.CalenderIcon,
            buttonImageOnly: true
        });

        //register event for Add/Edit button click on dialog
        $(elementIDs.reportTmplVrsnDialog + ' button').on('click', function () {

            var effDate = $(elementIDs.reportTmplVrsnEffDate).val();
            var reportTemplateVersionID = 0;

            if ($("input[name='" + elementIDs.chkbox + "']").is(":checked")) {
                var templateVersionList = $(elementIDs.reportTemplateVersionGridJQ).getRowData();

                if (templateVersionList.length != 0) {
                    reportTemplateVersionID = $(elementIDs.reportTmplVrsnDialog + ' select').val();
                }
            }


            if (Date.parse(effDate)) {

                //Removed validation as per defect logged under EQN-1787

                //var rptTemplateVersionsList = $(elementIDs.reportTemplateVersionGridJQ).getRowData();

                //var checkEffectiveDate = rptTemplateVersionsList.length > 0 &&
                //                       new Date(effDate) <= new Date(getMinimumEffectiveDate(rptTemplateVersionsList));

                //if (checkEffectiveDate === true && rptTemplateVersionsList.length > 0) {
                //    //$(elementIDs.reportTmplVrsnDialog).parent('div').addClass('has-error');
                //    //$(elementIDs.reportTmplVrsnDialog).text(ReportTemplateVersion.effectiveDateGreaterThanVersionMsg);
                //    //return;
                //    messageDialog.show(ReportTemplateVersion.effectiveDateGreaterThanVersionMsg);
                //    return;
                //}
                ////save the new report Template
                //var effectiveDateMessage = isValidEffectiveDate(effDate);
                //if (effectiveDateMessage == "") {

                var rowId = $(elementIDs.reportTemplateVersionGridJQ).getGridParam('selrow');
                //var 
                var url, reportTemplateVrsn;
                if (reportTmplVersionDialogState === 'add') {
                    url = URLs.reportTemplateVrsnAdd;

                    reportTemplateVrsn = {
                        TenantID: 1,
                        ReportTemplateID: reportTemplateID,
                        EffectiveDate: effDate,
                        reportTemplateVersionID: reportTemplateVersionID
                    };
                }
                else {
                    url = URLs.reportTemplateVrsnUpdate

                    reportTemplateVrsn = {
                        TenantID: 1,
                        ReportTemplateVersionID: rowId,
                        EffectiveDate: effDate
                    };
                }

                //ajax call to add/update
                var promise = ajaxWrapper.postJSON(url, reportTemplateVrsn);
                //register ajax success callback
                promise.done(reportTemplateVrsnSuccess);
                //}
            }
            else {                       //register ajax failure callback                
                messageDialog.show("Please enter a Valid Effective Date.");
            }
        });

        //register event for Check box change to add a new version
        $("input[name='" + elementIDs.chkbox + "']").on('change', function () {
            if ($(this).is(":checked")) {
                var templateVersionList = $(elementIDs.reportTemplateVersionGridJQ).getRowData();

                if (templateVersionList.length == 0) {
                    $(elementIDs.reportTmplVrsnDialog + ' select').attr('disabled', 'disabled');
                }
                else {
                    //enable copy version drop down list
                    $(elementIDs.reportTmplVrsnDialog + ' select').removeAttr("disabled");

                    // Set first element in the dropdown list as selected
                    $(elementIDs.reportTmplVrsnDialog + ' select option:first').attr('selected', 'selected');
                }
            }
            else {
                //disable copy version drop down list
                $(elementIDs.reportTmplVrsnDialog + ' select').attr('disabled', 'disabled');
            }
        });


        function getMinimumEffectiveDate(rptTemplateVersionsList) {
            var min = rptTemplateVersionsList.sort(function (a, b) {
                return new Date(a.EffectiveDate) - new Date(b.EffectiveDate);
            });
            return min[0].EffectiveDate;
        }
    }
    //initialize the dialog after this js is loaded
    init();

    //these are the properties that can be called by using reportTmplDialog.<Property>
    //eg. reportTmplDialog.show('name','add');
    return {
        show: function (ReportID, action) {
            reportTmplVersionDialogState = action;
            reportTemplateID = ReportID;
            $(elementIDs.reportTmplVrsnEffDate).datepicker();

            $(elementIDs.reportTmplVrsnDialog + ' input').each(function () {
                $(elementIDs.reportTmplVrsnDialog + ' div').removeClass('has-error');
            });

            $("input[name='" + elementIDs.chkbox + "']").removeAttr("checked");
            var dialogTital;
            if (reportTmplVersionDialogState === 'add') {
                $(elementIDs.divReportTmplVrsnAdd).show();
                $(elementIDs.reportTmplVrsnDialog + ' select').attr('disabled', 'disabled');
                dialogTital = ReportTemplateVersion.addReportTemplateVrsn;
            }
            else {
                dialogTital = ReportTemplateVersion.editReportTemplateVrsn;
                $(elementIDs.divReportTmplVrsnAdd).hide();
            }

            $(elementIDs.reportTmplVrsnDialog).dialog('option', 'title', dialogTital);
            $(elementIDs.reportTmplVrsnDialog + ' .help-block').text(ReportTemplateVersion.reportTmplVrsnNewVersionEffectiveDateMsg);
            $(elementIDs.copyCollateralTemplateVersionCheckboxHelpblock).text(ReportTemplateVersion.reportTmplVrsnCheckBoxMsg);

            //open the dialog - uses jqueryui dialog
            $(elementIDs.reportTmplVrsnDialog).dialog("open");


        }
    }
}(); //invoked soon after loading

var commentsDialog = function () {
    var formDesignVersion;
    var URLs = {
        //Finalize report Design
        reportTemplateVersionFinalize: '/DocumentCollateral/FinalizeReportVersion',
    };
    var elementIDs = {
        //id for save button
        saveButton: "saveButton",
        //id for textarea of comments
        comments: "#comments",
        //id for dialog
        commentsDialog: "#commentsDialog",

        reportTemplateVersionGrid: 'rptTmplVersions',

        reportTemplateVersionGridJQ: '#rptTmplVersions',

        WordUploadPrintX: "#WordUploadPrintX"

    }

    //ajax callback success - reload Form Desing Version grid
    function VersionFinalizeSuccess(xhr) {
        if (xhr.Items[0]) {
            var message = '';
            for (var i = 0; i < xhr.Items[0].Messages.length; i++) {
                message += xhr.Items[0].Messages[i] + ", ";
            }
            messageDialog.show(ReportTemplate.finalizationCannotPerformMsg + message);
        }
        else if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(ReportTemplate.finalizationSuccessMsg);
        }
        else {
            messageDialog.show(Common.errorMsg);
        }

        reportTemplate.viewReportTemplateVersion();
    }
    //handler for ajax errors
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    function init() {
        //register dialog 
        $(elementIDs.commentsDialog).dialog({
            autoOpen: false,
            height: 200,
            width: 400,
            modal: true
        });
        $(elementIDs.commentsDialog + ' button').on('click', function () {
            var rowId = $(elementIDs.reportTemplateVersionGridJQ).getGridParam('selrow');
            var finalizingComments;
            //Comments added by user
            finalizingComments = $(elementIDs.comments).val();

            var reportTemplateFinalize = {
                tenantId: 1,
                reportVersionId: rowId,
                comments: finalizingComments
            };
            //submit using ajax for finalization
            var promise = ajaxWrapper.postJSON(URLs.reportTemplateVersionFinalize, reportTemplateFinalize);
            //To close comments dialog.
            $(elementIDs.commentsDialog).dialog("close");
            //success callback
            promise.done(VersionFinalizeSuccess);
            //failure callback
            promise.fail(showError);
        });
    }
    //initialize the dialog after this js is loaded
    init();
    return {
        show: function (version) {
            formDesignVersion = version;
            $(elementIDs.commentsDialog).dialog('option', 'title', DocumentDesign.designAddComments);
            //To refresh textarea of comments
            $(elementIDs.comments).val("");
            //open the dialog - uses jqueryui dialog
            $(elementIDs.commentsDialog).dialog("open");
        }
    }
}();

//var collateralQueueDialog = function (accountID, accountName, folderID, folderName, selectedGenReportID, folderVersionID, effDate, collateralName) {
//    var URLs = {
//        //Queue Collateral
//        queueCollateral: "/DocumentCollateral/QueueCollateral?accountID={accountID}&accountName={accountName}&folderID={folderID}&folderName={folderName}&formInstanceIDs={formInstanceIDs}&folderVersionID={folderVersionID}&folderVersionNumbers={folderVersionNumbers}&reportTemplateID={reportTemplateID}&productIds={productIds}&folderVersionEffDt={folderVersionEffDt}&runDate={runDate}&reportName={ReportName}",
//    };

//    var elementIDs = {
//        //collateral Queue Dialog
//        collateralQueueDialog: "#collateralQueueDialog",
//        //Input radio type
//        collateralQueueType: "collateralQueueType",
//        //lblcollateralQueueRunDate: "#lblCollateralQueueRunDate",
//        collateralQueueRunDate: "#collateralQueueRunDate",
//        datePicker: "#divDatePicker",
//        lblAccountID: "#lblAccountID",
//        lblAccountName: "#lblAccountName",
//        lblFolderID: "#folderID",
//        lblFolderName: "#folderName",
//        lblFolderVersionID: "#folderVersionID",
//        lblFolderVersionEffDt: "#folderVersionEffDt",
//        lblReportTemplateName: "#reportTemplateName",
//        collateralQueueDocumentsGridJQ: "#collaterlDialogDocumentsInFolder",
//    };


//    function collateralQueueSuccess(xhr) {
//        if (xhr.Result === ServiceResult.SUCCESS)
//            messageDialog.show(CollateralQueue.enQueuecollateral);
//        else
//            messageDialog.show(CollateralQueue.collateralQueueErrorMsg);
//        $(elementIDs.collateralQueueDialog).dialog("close");
//    }

//    function init() {
//        //register dialog for grid row add/edit
//        $(elementIDs.collateralQueueDialog).dialog({
//            autoOpen: false,
//            height: 600,
//            width: 950,
//            modal: true
//        });

//        $(elementIDs.datePicker + ' input').val('');

//        //register event for button click on dialog
//        $(elementIDs.collateralQueueDialog + ' button').unbind().on('click', function () {
//            var grid = $(elementIDs.collateralQueueDocumentsGridJQ);
//            var gridRows = grid.jqGrid('getRowData');
//            var arr = [];
//            arr = gridRows;

//            var formInstanceIDs = "", productIds = "", folderVersionNumber = "";

//            $.each(arr, function (index) {
//                if (arr[index]['IsSelected']) {
//                    formInstanceIDs += arr[index]['FormInstanceID'] + ",";
//                    productIds += arr[index]['FormName'] + ",";
//                    folderVersionNumber += arr[index]['FolderVersionNumber'] + ",";
//                }
//            });

//            var runDate = $(elementIDs.datePicker + ' input').val();

//            var url = URLs.queueCollateral.replace(/\{accountID\}/, accountID).replace(/\{accountName\}/, accountName).replace(/\{folderID\}/, folderID)
//                      .replace(/\{folderName\}/, folderName).replace(/\{formInstanceIDs\}/, formInstanceIDs).replace(/\{folderVersionID\}/, folderVersionID)
//                      .replace(/\{folderVersionNumbers\}/, folderVersionNumber).replace(/\{reportTemplateID\}/, selectedGenReportID)
//                      .replace(/\{productIds\}/, productIds).replace(/\{folderVersionEffDt\}/, effDate).replace(/\{runDate\}/, runDate).replace(/\{ReportName\}/, collateralName);


//            if (formInstanceIDs.length == 0) {
//                messageDialog.show('Please select atleast one Document to queue the Collateral.');
//                return false;
//            }
//            else {
//                var promise = ajaxWrapper.getJSON(url);
//                promise.done(collateralQueueSuccess);
//            }
//        });

//        $(document).ready(function () {
//            $("input[name=" + elementIDs.collateralQueueType + "]:radio").unbind().on('change', function () {
//                if ($("input[name=" + elementIDs.collateralQueueType + "]:checked").val() == "Schedule") {
//                    $(elementIDs.datePicker).show();
//                    $(elementIDs.datePicker + ' input').datepicker({
//                        minDate: 0
//                    });
//                }
//                else {
//                    $(elementIDs.datePicker).hide();
//                }
//            });
//        });
//    }

//    //initialize the dialog after this js is loaded
//    init();

//    return {
//        show: function () {
//            $(elementIDs.collateralQueueDialog + ' input').each(function () {
//                $(elementIDs.collateralQueueDialog + ' div').removeClass('has-error');
//            });

//            $(elementIDs.lblAccountID).text("Account ID : " + accountID);
//            $(elementIDs.lblAccountName).text("Account Name : " + accountName);
//            $(elementIDs.lblFolderID).text("Folder ID : " + folderID);
//            $(elementIDs.lblFolderName).text("Folder Name : " + folderName);
//            $(elementIDs.lblFolderVersionID).text("Folder Version ID : " + folderVersionID);
//            $(elementIDs.lblFolderVersionEffDt).text("Folder Version Effective Date : " + effDate);
//            $(elementIDs.lblReportTemplateName).text("Collateral Template Name :" + collateralName);

//            $(elementIDs.collateralQueueDialog).dialog('option', 'title', "Queue Collateral");
//            //open the dialog - uses jqueryui dialog
//            $(elementIDs.collateralQueueDialog).dialog("open");
//        }
//    }
//}

var viewQueuedCollaterals = function () {
    var URLs = {
        getQueuedCollateralsList: "/DocumentCollateral/GetQueuedCollateralsList",
        getCollateralFile: "/DocumentCollateral/DownloadCollateralFile",
        importFile: "/DocumentCollateral/UploadPrintXPDFFile?processQueue1Up={processQueue1Up}&fileFormat={fileFormat}"

    };
    var SETTING = { isComplianceEnable: false, printX: false };
    var SETTINGTYPE = { C508: "tmg.equinox.net.PdfConformance.OnOff", printX: "tmg.equinox.net.printx.OnOff" };

    var elementIDs = {
        queuedCollateralsGrid: "queuedCollateralsGrid",
        queuedCollateralsGridJQ: "#queuedCollateralsGrid",
        WordUploadPrintX: "#WordUploadPrintX",
        UploadMLFileName: "#UploadMLFileName",
        UploadMLFileNameSpan: "#UploadMLFileNameSpan",
        PDFFileUploadError: '#pdffileuploadwerror'
    };

    var timer = 60;
    this.interval = 1000 * 60 * 1;
    this.refreshInterval;

    function init() {
        $(document).ready(function () {
            getSetting(SETTINGTYPE.C508);
            getSetting(SETTINGTYPE.printX);

            loadQueuedCollateralsGrid();

            $('#btnWordFile').click(function () {
                var selectedRowId = $(elementIDs.queuedCollateralsGridJQ).jqGrid('getGridParam', 'selrow');

                var file;
                var MLFileName = $(elementIDs.UploadMLFileName).val();
                var MLFileNameUpload = $(elementIDs.UploadMLFileName).get(0);
                var MLFileNamefiles = MLFileNameUpload.files;

                var fileData = new FormData();

                // Adding Comment key to FormData object  

                // Adding ML Import to FormData object
                if (MLFileNamefiles.length > 0) {
                    // fileData = MLFileNamefiles;
                    fileData.append(MLFileNamefiles[0].name, MLFileNamefiles[0]);
                    var fileN = MLFileNamefiles[0].name.split('.');
                    var fileFormatFlag = "false";
                    var fileFormat = fileN[fileN.length - 1];

                    if (fileFormat == "Pdf" || fileFormat == "pdf") {
                        fileFormatFlag = "true";
                    }
                }
                if (fileFormatFlag == "true") {

                    var urlData = URLs.importFile.replace("{processQueue1Up}", selectedRowId);
                    urlData = urlData.replace("{fileFormat}", fileFormat);
                    $.ajax({
                        url: urlData,
                        type: "POST",
                        contentType: false, // Not to set any content header  
                        processData: false, // Not to process data  
                        data: fileData,
                        success: function (result) {
                            if (result == "true") {
                                messageDialog.show("Print X PDF File Uploaded Sucessfully.");
                                $(elementIDs.WordUploadPrintX).dialog("close");
                            }
                        },
                        error: function (err) {
                            messageDialog.show(err);
                        }
                    });
                }
                else {
                    $(elementIDs.PDFFileUploadError).text("File Not in Correct Format");
                }
            });


        });


        $(elementIDs.WordUploadPrintX).dialog({
            autoOpen: false,
            height: 500,
            width: 700,
            modal: true
        });

    }
    init();

    function getSetting(name) {
        var url = '/DocumentCollateral/GetSettingInfo?name=' + name;
        var promise = ajaxWrapper.getJSONSync(url);
        promise.done(function (data) {
            if (data != null) {
                if (SETTINGTYPE.C508 == name)
                    SETTING.isComplianceEnable = data == "ON" ? true : false;
                else (SETTINGTYPE.printX == name)
                SETTING.printX = data == "ON" ? true : false;
            }
        });
    }
    function loadQueuedCollateralsGrid() {
        //set column list for grid
        var colArray = null;
        colArray = ['Id', 'Collateral Name', 'Product Name', 'Folder Name', 'Version Number', 'Status', 'Processed Date', 'Download PDF', 'Download Word', 'TemplateReportVersionID'];// 'Product Json','image',

        //set column models
        var colModel = [];
        colModel.push({ key: true, hidden: true, name: 'CollateralProcessQueue1Up', index: 'CollateralProcessQueue1Up', align: 'right', editable: true });
        colModel.push({ key: false, name: 'CollateralName', index: 'CollateralName', editable: false, width: '120' });
        colModel.push({ key: false, name: 'ProductID', index: 'ProductID', editable: false, width: '150' });
        //colModel.push({ key: false, name: 'AccountName', index: 'AccountName', editable: true, edittype: 'select', align: 'left', width: '150' });
        colModel.push({ key: false, name: 'FolderName', index: 'FolderName', editable: true, edittype: 'select', align: 'left', width: '200' });
        colModel.push({ key: false, name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: true, edittype: 'select', align: 'left', width: '130' });
        //colModel.push({ key: false, name: 'imageName', index: 'imageName', editable: true, edittype: 'select', align: 'left', width: '200' });
        colModel.push({ key: false, name: 'Status', index: 'Status', editable: false, align: 'left', width: '100', formatter: processStatusFormmater });
        colModel.push({ key: false, name: 'ProcessedDate', index: 'ProcessedDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'left' });
        colModel.push({ key: false, name: 'CollateralProcessQueue1Up', index: 'CollateralProcessQueue1Up', search: false, editable: false, width: '80', formatter: downloadPDFFileFormmater });
        colModel.push({ key: false, name: 'CollateralProcessQueue1Up', index: 'CollateralProcessQueue1Up', search: false, editable: false, width: '90', formatter: downloadWordFileFormmater });
        colModel.push({ key: false, name: 'TemplateReportVersionID', index: 'TemplateReportVersionID', hidden: true, search: false });

        //clean up the grid first - only table element remains after this
        $(elementIDs.queuedCollateralsGridJQ).jqGrid('GridUnload');

        //adding the pager element
        $(elementIDs.queuedCollateralsGridJQ).parent().append("<div id='p" + elementIDs.queuedCollateralsGrid + "'></div>");

        $(elementIDs.queuedCollateralsGridJQ).jqGrid({
            url: URLs.getQueuedCollateralsList,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Collaterals Queued',
            height: '400',
            rowNum: 20,
            rowList: [20, 40, 60],
            headertitles: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            //altRows: true,
            //multiselect: false,
            //multiboxonly: true,
            sortname: 'ProcessedDate',
            sortorder: 'desc',
            pager: '#p' + elementIDs.queuedCollateralsGrid,
            gridComplete: function () {
                $(".DownloadQueuedCollateral").on('click', function (e) {

                    if ($(this).attr("data-Id") !== undefined) {
                        var parameters = $(this).attr("data-Id").split('ö');
                        var processQueue1Up = parameters[0];
                        var reportName = parameters[1];
                        var templateReportVersionID = parameters[2];
                        var fileFormat = parameters[3];
                        var url = '/DocumentCollateral/CheckIfFileExistsToDownload?processQueue1Up=' + processQueue1Up + "&fileFormat=" + fileFormat;
                        var promise = ajaxWrapper.getJSON(url);
                        promise.done(function (doesFileExist) {
                            if (doesFileExist == true)
                                window.location.href = "/DocumentCollateral/DownloadCollateralFile?processQueue1Up=" + processQueue1Up + "&reportName=" + reportName + "&templateReportVersionID=" + templateReportVersionID + "&fileFormat=" + fileFormat;
                            else
                                messageDialog.show(CollateralQueue.fileDoesNotExist.replace(/\{FileFormat\}/g, fileFormat.toUpperCase()));
                        });
                    }
                });

                $(".DownloadQueuedCollateralImageUpload").on('click', function (e) {

                    $(elementIDs.WordUploadPrintX).dialog('option', 'title', 'Word File Upload');

                    //open the dialog - uses jqueryui dialog
                    $(elementIDs.WordUploadPrintX).dialog("open");


                });
            },
            //altclass: 'alternate'
        });

        var pagerElement = '#p' + elementIDs.queuedCollateralsGrid;
        $('#p' + elementIDs.queuedCollateralsGrid).find('input').css('height', '20px');
        //remove default buttons
        $(elementIDs.queuedCollateralsGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.queuedCollateralsGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

        //Custom Delete button
        //$.ajaxSettings.traditional = true;

        refreshInterval = setInterval(AutorefreshQueuedReports, 1000);
    }

    function AutorefreshQueuedReports() {
        timer--;
        if (timer == 0) {
            $(elementIDs.queuedCollateralsGridJQ).trigger("reloadGrid", [{ current: true }]);
            //clearInterval(refreshInterval);
            timer = 60;
        }
        //else {
        $("#spnTimer").text(timer + " seconds.");
        //}
    }

    // Function to set font colour based on status using formatter 
    function processStatusFormmater(cellvalue, options, rowObject) {
        switch (cellvalue) {
            case "Queued":
                // If status is Queued then display status text in back color 
                cellvalue = '<span style="color:blue">' + 'Queued' + '</span>';
                break;
            case "In Progress":
                // If status is Inprogress then display status text in black color 
                cellvalue = '<span style="color:black">' + cellvalue + '</span>';
                break;
            case "Complete":
                // If status is Complete then display status text in green color
                cellvalue = '<span style="color:green">' + cellvalue + '</span>';
                break;
            case "Errored":
                // If status is Errored then display status text in red color 
                cellvalue = '<span style="color:red">' + cellvalue + '</span>';
                break;
        }
        return cellvalue;
    } // processStatusFormmater

    function showError(msg) {
        messageDialog.show(JSON.stringify(msg));
    }

    function downloadPDFFileFormmater(cellvalue, options, rowObject) {


        var value = rowObject.FormInstanceID;
        cellvalue = "<img src='/Content/css/custom-theme/images/pdf.png' class='DownloadQueuedCollateral' alt='PDFCollateral' style='margin-left: 5px;'  title = 'PDF Collateral' data-Id='" + rowObject.CollateralProcessQueue1Up + 'ö' + rowObject.CollateralName + 'ö' + rowObject.TemplateReportVersionID + 'ö' + 'pdf' + "'/>";

        if (SETTING.printX == true) {
            cellvalue = cellvalue + "<img src='/Content/css/custom-theme/images/pdf-printer-icon.png' class='DownloadQueuedCollateral' alt='' style='margin-left: 5px;'  title = 'Print X PDFCollateral' data-Id='" + rowObject.CollateralProcessQueue1Up + 'ö' + rowObject.CollateralName + 'ö' + rowObject.TemplateReportVersionID + 'ö' + 'pdfx' + "'/>";
            if (rowObject.PrintXManallyUploaded == true)
            {
                cellvalue = cellvalue + "<img src='/Content/css/custom-theme/images/pdf-printer-icon.png' class='DownloadQueuedCollateral' alt='' style='margin-left: 5px;'  title = 'Print X PDFCollateral' data-Id='" + rowObject.CollateralProcessQueue1Up + 'ö' + rowObject.CollateralName + 'ö' + rowObject.TemplateReportVersionID + 'ö' + 'pdfx' + "'/>";
            }
        }
        if (SETTING.isComplianceEnable == true) {
            cellvalue = cellvalue + "<img src='/Content/css/custom-theme/images/check.png'  alt='Compliance Validations' style='margin-left: 5px;'  title = 'PDF Collateral Compliance Validations'  onclick='javascript:viewComplianceIssues(" + value + "," + rowObject.CollateralProcessQueue1Up + ");'/>";
        }

        return cellvalue;
    }
    viewComplianceIssues = function (formInstanceId, collateralQueueId) {
        var viewCompianceIssue = new viewComplianceErrorDialog(formInstanceId, collateralQueueId);
        viewCompianceIssue.show();
    }
    function showCompliancevalidation(img) {
        var accDetails = new viewAccountDetailsDialog(1);
        accDetails.show();
    }


    function downloadWordFileFormmater(cellvalue, options, rowObject) {
        cellvalue = "<img src='/Content/css/custom-theme/images/word.png' class='DownloadQueuedCollateral' alt='WordsCollateral' style='margin-left: 5px;'  title = 'Word Collateral' data-Id='" + rowObject.CollateralProcessQueue1Up + 'ö' + rowObject.CollateralName + 'ö' + rowObject.TemplateReportVersionID + 'ö' + 'word' + "'/>";
        // if (Role.TMGSuperUser == "24" && window.location.search == "?true")
        if (window.location.search == "?true")
        {
            cellvalue = cellvalue + "<img src='/Content/css/custom-theme/images/UploadPDF.jfif' class='DownloadQueuedCollateralImageUpload' alt='Print X File Upload' style='margin-left: 5px; height:16px;width:16px;'  title = 'Print X File Upload' data-Id='" + rowObject.CollateralProcessQueue1Up + 'ö' + rowObject.CollateralName + 'ö' + rowObject.TemplateReportVersionID + 'ö' + 'word' + "'/>";
        }
        return cellvalue;
    }
}

var viewUploadedCollaterals = function () {
    var URLs = {
        getUploadedCollateralLogsList: "/DocumentCollateral/GetUploadedCollateralLogsList",
        getCollateralFile: "/DocumentCollateral/DownloadCollateralFile",
        importFile: "/DocumentCollateral/UploadPrintXPDFFile?processQueue1Up={processQueue1Up}&fileFormat={fileFormat}"
      
    };
    var SETTING = { isComplianceEnable: false, printX: false };
    var SETTINGTYPE = { C508: "tmg.equinox.net.PdfConformance.OnOff", printX: "tmg.equinox.net.printx.OnOff" };

    var elementIDs = {
        uploadedCollateralsGrid: "uploadedCollateralsGrid",
        uploadedCollateralsGridJQ: "#uploadedCollateralsGrid",
        WordUploadPrintX: "#WordUploadPrintX",
        UploadMLFileName: "#UploadMLFileName",
        UploadMLFileNameSpan: "#UploadMLFileNameSpan",
        PDFFileUploadError: '#pdffileuploadwerror'
    };

    var timer = 60;
    this.interval = 1000 * 60 * 1;
    this.refreshInterval;

    function init() {
        $(document).ready(function () {
            getSetting(SETTINGTYPE.C508);
            getSetting(SETTINGTYPE.printX);

            loadUploadedCollateralsGrid();

            $('#btnWordFile').click(function () {
                var selectedRowId = $(elementIDs.queuedCollateralsGridJQ).jqGrid('getGridParam', 'selrow');

                    var file;
                    var MLFileName = $(elementIDs.UploadMLFileName).val();
                    var MLFileNameUpload = $(elementIDs.UploadMLFileName).get(0);
                    var MLFileNamefiles = MLFileNameUpload.files;

                    var fileData = new FormData();

                    // Adding Comment key to FormData object  

                    // Adding ML Import to FormData object
                    if (MLFileNamefiles.length > 0) {
                      // fileData = MLFileNamefiles;
                        fileData.append(MLFileNamefiles[0].name, MLFileNamefiles[0]);
                        var fileN = MLFileNamefiles[0].name.split('.');
                        var  fileFormatFlag="false";
                        var fileFormat=fileN[fileN.length - 1];

                        if (fileFormat == "Pdf" || fileFormat == "pdf")
                        {
                            fileFormatFlag = "true";
                        }
                    }
                    if (fileFormatFlag == "true") {
                       
                        var urlData = URLs.importFile.replace("{processQueue1Up}", selectedRowId);
                        urlData = urlData.replace("{fileFormat}", fileFormat);
                        $.ajax({
                            url: urlData,
                            type: "POST",
                            contentType: false, // Not to set any content header  
                            processData: false, // Not to process data  
                            data: fileData,
                            success: function (result) {
                                if (result == "true")
                                {
                                    messageDialog.show("Print X PDF File Uploaded Sucessfully.");
                                    $(elementIDs.WordUploadPrintX).dialog("close");
                                }                                
                            },
                            error: function (err) {
                                messageDialog.show(err);
                            }
                        });
                    }
                    else
                    {
                        $(elementIDs.PDFFileUploadError).text("File Not in Correct Format");
                    }
                });

           
            });
                 

        $(elementIDs.WordUploadPrintX).dialog({
            autoOpen: false,
            height: 500,
            width: 700,
            modal: true
        });

    }
    init();
   

    function getSetting(name) {
        var url = '/DocumentCollateral/GetSettingInfo?name=' + name;
        var promise = ajaxWrapper.getJSONSync(url);
        promise.done(function (data) {
            if (data != null) {
                if (SETTINGTYPE.C508 == name)
                    SETTING.isComplianceEnable = data == "ON" ? true : false;
                else (SETTINGTYPE.printX == name)
                SETTING.printX = data == "ON" ? true : false;
            }
        });
    }

    function loadUploadedCollateralsGrid() {
        //set column list for grid
        var colArray = null;
        colArray = ['ID', 'Collateral Name', 'Product Name', 'Folder Name', 'Version Number', 'Created Date', 'Download PDF'];// 'Product Json','image',

        //set column models
        var colModel = [];
        colModel.push({ key: true, name: 'ID', index: 'ID', align: 'right', editable: false, width: '60' });
        colModel.push({ key: false, name: 'CollateralName', index: 'CollateralName', editable: false, width: '120' });
        colModel.push({ key: false, name: 'ProductID', index: 'ProductID', editable: false, width: '150' });
        //colModel.push({ key: false, name: 'AccountName', index: 'AccountName', editable: true, edittype: 'select', align: 'left', width: '150' });
        colModel.push({ key: false, name: 'FolderName', index: 'FolderName', editable: true, edittype: 'select', align: 'left', width: '200' });
        colModel.push({ key: false, name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: true, edittype: 'select', align: 'left', width: '130' });
        colModel.push({ key: false, name: 'CreatedDate', index: 'CreatedDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'left' });
        colModel.push({ key: false, name: 'ID', index: 'ID', search: false, editable: false, width: '80', formatter: downloadPDFFileFormmater });
       
        //clean up the grid first - only table element remains after this
        $(elementIDs.uploadedCollateralsGridJQ).jqGrid('GridUnload');

        //adding the pager element
        $(elementIDs.uploadedCollateralsGridJQ).parent().append("<div id='p" + elementIDs.uploadedCollateralsGrid + "'></div>");

        $(elementIDs.uploadedCollateralsGridJQ).jqGrid({
            url: URLs.getUploadedCollateralLogsList,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Collaterals Uploaded',
            height: '400',
            rowNum: 20,
            rowList: [20, 40, 60],
            headertitles: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            //altRows: true,
            //multiselect: false,
            //multiboxonly: true,
            sortname: 'ID',
            sortorder: 'desc',
            pager: '#p' + elementIDs.uploadedCollateralsGrid,
            gridComplete: function () {
                $(".DownloadQueuedCollateral").on('click', function (e) {

                    if ($(this).attr("data-Id") !== undefined) {
                        var parameters = $(this).attr("data-Id").split('ö');
                        var processQueue1Up = parameters[0];
                        var reportName = parameters[1];
                        //var templateReportVersionID = parameters[2];
                        var fileFormat = parameters[2];
                        window.location.href = "/DocumentCollateral/DownloadManualFile?id=" + processQueue1Up + "&fileFormat=" + fileFormat;
                        //var url = '/DocumentCollateral/CheckIfFileExistsToDownload?processQueue1Up=' + id + "&fileFormat=" + fileFormat;
                        //var promise = ajaxWrapper.getJSON(url);
                        //promise.done(function (doesFileExist) {
                        //    if (doesFileExist == true)
                        //        window.location.href = "/DocumentCollateral/DownloadCollateralFile?processQueue1Up=" + processQueue1Up + "&reportName=" + reportName + "&templateReportVersionID=" + templateReportVersionID + "&fileFormat=" + fileFormat;
                        //    else
                        //        messageDialog.show(CollateralQueue.fileDoesNotExist.replace(/\{FileFormat\}/g, fileFormat.toUpperCase()));
                        //});
                    }
                });

                $(".DownloadQueuedCollateralImageUpload").on('click', function (e) {

                    $(elementIDs.WordUploadPrintX).dialog('option', 'title', 'Word File Upload');

                    //open the dialog - uses jqueryui dialog
                    $(elementIDs.WordUploadPrintX).dialog("open");


                });

            
            },
            //altclass: 'alternate'
        });



        var pagerElement = '#p' + elementIDs.uploadedCollateralsGrid;
        $('#p' + elementIDs.uploadedCollateralsGrid).find('input').css('height', '20px');
        //remove default buttons
        $(elementIDs.uploadedCollateralsGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.uploadedCollateralsGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

        //Custom Delete button
        //$.ajaxSettings.traditional = true;

        //refreshInterval = setInterval(AutorefreshQueuedReports, 1000);
    }


    // Function to set font colour based on status using formatter 
    function processStatusFormmater(cellvalue, options, rowObject) {
        switch (cellvalue) {
            case "Complete":
                // If status is Complete then display status text in green color
                cellvalue = '<span style="color:green">' + cellvalue + '</span>';
                break;
            case "Errored":
                // If status is Errored then display status text in red color 
                cellvalue = '<span style="color:red">' + cellvalue + '</span>';
                break;
        }
        return cellvalue;
    } // processStatusFormmater

    function showError(msg) {
        messageDialog.show(JSON.stringify(msg));
    }

    function downloadPDFFileFormmater(cellvalue, options, rowObject) {
        var value = rowObject.FormInstanceID;
        cellvalue = "<img src='/Content/css/custom-theme/images/pdf.png' class='DownloadQueuedCollateral' alt='PDFCollateral' style='margin-left: 5px;'  title = 'PDF Collateral' data-Id='" + rowObject.ID + 'ö' + rowObject.CollateralName + 'ö' + 'pdf' + "'/>";
        cellvalue = cellvalue + "<img src='/Content/css/custom-theme/images/pdf-printer-icon.png' class='DownloadQueuedCollateral' alt='' style='margin-left: 5px;'  title = 'Print X PDFCollateral' data-Id='" + rowObject.ID + 'ö' + rowObject.CollateralName + 'ö' + 'pdfx' + "'/>";
        cellvalue = cellvalue + "<img src='/Content/css/custom-theme/images/word.png' class='DownloadQueuedCollateral' alt='WordsCollateral' style='margin-left: 5px;'  title = 'Word Collateral' data-Id='" + rowObject.ID + 'ö' + rowObject.CollateralName + 'ö' + 'word' + "'/>";
        return cellvalue;
    }
    viewComplianceIssues = function (formInstanceId, collateralQueueId) {
        var viewCompianceIssue = new viewComplianceErrorDialog(formInstanceId, collateralQueueId);
        viewCompianceIssue.show();
    }
    function showCompliancevalidation(img) {
        var accDetails = new viewAccountDetailsDialog(1);
        accDetails.show();
    }



    function downloadWordFileFormmater(cellvalue, options, rowObject)
    {
        cellvalue = "<img src='/Content/css/custom-theme/images/word.png' class='DownloadQueuedCollateral' alt='WordsCollateral' style='margin-left: 5px;'  title = 'Word Collateral' data-Id='" + rowObject.CollateralProcessQueue1Up + 'ö' + rowObject.CollateralName + 'ö' + rowObject.TemplateReportVersionID + 'ö' + 'word' + "'/>";
        if (Role.TMGSuperUser == "24" && window.location.search == "?true")
        {
            cellvalue = cellvalue + "<img src='/Content/css/custom-theme/images/UploadPDF.jfif' class='DownloadQueuedCollateralImageUpload' alt='Print X File Upload' style='margin-left: 5px; height:16px;width:16px;'  title = 'Print X File Upload' data-Id='" + rowObject.CollateralProcessQueue1Up + 'ö' + rowObject.CollateralName + 'ö' + rowObject.TemplateReportVersionID + 'ö' + 'word' + "'/>";
        }        
        return cellvalue;
    }
}

var viewAccountDetailsDialog = function (reportTemplateID) {
    var URLs = {
        docSearch: '/ConsumerAccount/GetDocumentsList?tenantId=1',
        getAccountsAndFolders: '/DocumentCollateral/GetAccountAndFolders?reportTemplateID={ReportTemplateID}&accountID={AccountID}&folderID={FolderID}&folderVersionID={FolderVersionID}&reportName={reportName}&planFamilyName={planFamilyName}',
        getAccountFolderDocuments: '/DocumentCollateral/GetAccountFolderFolderVersionDocuments?ReportTemplateID={ReportTemplateID}&AccountID={AccountID}&FolderID={FolderID}&FolderVersionID={FolderVersionID}',
        generateCollateral: "/DocumentCollateral/ViewReport",
        checkPermission: '/DocumentCollateral/CheckRolePermission?reportTemplateID={ReportTemplateID}&folderVersionEffDt={FolderVersionEffDt}',
    };

    var elementIDs = {
        sourceDialogJQ: "#accountDetailsDialog",
        docSearchGrid: "docSearch",
        docSearchGridJS: "#docSearch",
        docSearchGridPagerJQ: "#pdocSearch",
        sourceDocumentDiv: "#sourceDocument",
        //thSourceDocumentJQ: "#thSourceDcument",
        tdAccountNameJQ: "#tdAccountName",
        tdFolderNameJQ: "#tdFolderName",
        tdFolderVersionNumberJQ: "#tdFolderVersionNumber",
        tdEffectiveDateJQ: "#tdEffectiveDate",
        relatedDocumentGrid: 'documentsInFolder',
        relatedDocumentGridJQ: '#documentsInFolder',
        queueCollateralButton: '#QueueCollateralBtn',
        uploadCollateralButton: '#UploadCollateralBtn',
        divCollateralOptions: '#divCollateralOptions',
        hiddenCollateralOptionVal: '#webConfigCollateralOption',
        planFamilyJQ: '#planFamily'
    };

    var selRowId;

    function init() {
        $(elementIDs.sourceDialogJQ).dialog({
            autoOpen: false,
            height: 500,
            width: 700,
            modal: true
        });
    }
    init();

    function loadSelectGrid() {
        var currentInstance = this;
        var colArray = ['Folder', 'Folder Version Number', 'Effective Date', '', '', ''];
        var colModel = [];
        //colModel.push({ name: 'AccountName', index: 'AccountName', editable: false, align: 'left', width: 200 });
        colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left' });
        colModel.push({ name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: false, align: 'left' });
        colModel.push({ name: 'FolderVersionEffectiveDate', index: 'FolderVersionEffectiveDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'left' });
        colModel.push({ name: 'AccountID', index: 'AccountID', hidden: true });
        colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true });
        colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', hidden: true });

        $(elementIDs.docSearchGridJS).jqGrid('GridUnload');
        $(elementIDs.docSearchGridJS).parent().append("<div id='p" + elementIDs.docSearchGrid + "'></div>");
        var planFamilyValue = $(elementIDs.planFamilyJQ).val();
        var url = URLs.getAccountsAndFolders.replace(/\{ReportTemplateID\}/, reportTemplateID).replace(/\{AccountID\}/, 0).replace(/\{FolderID\}/, 0).replace(/\{FolderVersionID\}/, 0).replace(/\{reportName}/, selectedReportName).replace(/\{planFamilyName}/, planFamilyValue);

        $(elementIDs.docSearchGridJS).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            //caption: 'Select Documents',
            height: '300',
            rowNum: 20,
            rowList: [10, 20, 30],
            ignoreCase: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: elementIDs.docSearchGridPagerJQ,
            sortname: 'AccountName',
            altclass: 'alternate',
            ondblClickRow: function () {
                setDocumentSelection();
            },
            gridComplete: function () {
                //To set first row of grid as selected.
                console.log("grid complete");
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
                   setDocumentSelection();
               }
           });
    }

    function setSourceDocumentValues(rowData) {
        $(elementIDs.tdAccountNameJQ).text(rowData.AccountName);
        $(elementIDs.tdFolderNameJQ).text(rowData.FolderName);
        $(elementIDs.tdFolderVersionNumberJQ).text(rowData.FolderVersionNumber);
        $(elementIDs.tdEffectiveDateJQ).text(rowData.FolderVersionEffectiveDate);
        $(elementIDs.sourceDocumentDiv).show();
        webConfigCollateralOption = $(elementIDs.hiddenCollateralOptionVal).val();
        if (webConfigCollateralOption.toLowerCase() == "both")
            $(elementIDs.divCollateralOptions).show();
        $(elementIDs.queueCollateralButton).show();
        $(elementIDs.uploadCollateralButton).show();
    }

    function setDocumentSelection(dialogParent) {
        var selectedRowId = $(elementIDs.docSearchGridJS).jqGrid('getGridParam', 'selrow');
        if (selectedRowId != null && selectedRowId != undefined) {
            var rowData = $(elementIDs.docSearchGridJS).jqGrid('getRowData', selectedRowId);
            setSourceDocumentValues(rowData);
            //selectedRow variable will be used to retrive values at the time of generating reports
            selectedRowData = rowData;
            $(elementIDs.sourceDialogJQ).dialog("close");
            if (rowData.AccountID == "")
                rowData.AccountID = 0;
            populateDocumentsInRepeater(reportTemplateID, rowData.AccountID, rowData.FolderID, rowData.FolderVersionID, elementIDs.relatedDocumentGrid, elementIDs.relatedDocumentGridJQ);
        }
        else {
            messageDialog.show("Please select a row.");
        }
    }

    function populateDocumentsInRepeater(ReportTemplateID, AccountID, FolderID, FolderVersionID, gridName, gridId) {
        $(elementIDs.genReportButton).show();
        //set column list for grid
        var colArray = ['AccountID', 'Document Type', 'FolderID', 'Folder Name', 'Document Name', 'FolderVersionID', 'Version Number', 'Effective Date', 'FormInstanceID', 'Data Source Name', 'FormDesignVersionID', 'PDF Collateral', 'Word Collateral', 'Queue product'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'AccountID', index: 'AccountID', hidden: true, search: false, });
        //colModel.push({ name: 'AccountName', index: 'AccountName', search: true, width: '120' });
        colModel.push({ name: 'DocumentType', index: 'DocumentType', search: true, width: '80' });
        colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true, search: false, });
        colModel.push({ name: 'FolderName', index: 'FolderName', search: true, hidden: true });
        colModel.push({ name: 'FormName', index: 'FormName', search: true, width: '80' });
        colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', search: false, hidden: true });
        colModel.push({ name: 'FolderVersionNumber', index: 'FolderVersionNumber', width: '80', search: true, });
        colModel.push({ name: 'FolderVersionEffectiveDate', index: 'FolderVersionEffectiveDate', search: true, hidden: true });
        colModel.push({ name: 'FormInstanceID', index: 'FormInstanceID', key: true, hidden: true, search: true, });
        colModel.push({ name: 'DataSourceName', index: 'DataSourceName', hidden: true, search: false, });
        colModel.push({ name: 'FormDesignVersionID', index: 'FormDesignVersionID', hidden: true, search: false, });
        colModel.push({ name: 'FormInstanceIdForReportGeneration', index: 'FormInstanceIdForReportGeneration', search: false, editable: false, width: '100', align: 'left', formatter: generatePDFReportFormmater });
        colModel.push({ name: 'FormInstanceIdForReportGeneration', index: 'FormInstanceIdForReportGeneration', search: false, editable: false, width: '100', align: 'left', formatter: generateWordReportFormmater });

        colModel.push({
            name: 'IsSelected', index: 'IsSelected', width: '100', align: 'left', sortable: false, search: false, editable: false, formatter: selectDocumentFormatter, unformat: function selectDocumentUnFormatter(cellValue, options) {
                // As we are using same function to render this grid at different locations, use different IDs for checkbox column
                var checkBoxId;
                if (gridId == elementIDs.collateralQueueDocumentsGridJQ)
                    checkBoxId = "chkBox_";
                else
                    checkBoxId = "sel_";
                var elementId = '#' + checkBoxId + options.rowId;
                return $(elementId).prop('checked');
            }
        });

        $(gridId).jqGrid('GridUnload');

        var url = URLs.getAccountFolderDocuments.replace(/\{ReportTemplateID\}/g, ReportTemplateID).replace(/\{AccountID\}/g, AccountID).replace(/\{FolderID\}/g, FolderID).replace(/\{FolderVersionID\}/g, FolderVersionID);

        //adding the pager element
        $(gridId).parent().append("<div id='p" + gridName + "'></div>");
        $(gridId).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Select Documents',
            height: '140',
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,                    // for Grid Refresh ( setting [loadonce: true] means Grid Refresh isn't needed)
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + gridName,
            sortname: 'FormInstanceID',
            altclass: 'alternate',
            gridComplete: function () {
                $(elementIDs.genReportButton).show();
                $(gridId).show();
            },
            resizeStop: function (width, index) {
                autoResizing(gridId);
            }
        });

        var pagerElement = '#p' + gridName;
        //remove default buttons
        $(gridId).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();

        // add filter toolbar to the top
        $(gridId).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });
    }

    function selectDocumentFormatter(cellValue, options, rowObject) {
        // As we are using same function to render this grid at different locations, use different IDs for checkbox column.
        var checkBoxId;
        if (options.gid == elementIDs.collateralQueueDocumentsGrid)
            checkBoxId = "chkBox_";
        else
            checkBoxId = "sel_";
        var id = checkBoxId + rowObject.FormInstanceID;
        return "<input id = '" + id + '\'' + (rowObject.IsSelected == 'true' ? ' checked="checked" readonly=\'true\' class=\'ui-state-disabled\' ' : '') + " type='checkbox' title = 'Select' />";   //  style = 'cursor: pointer'
    }

    function generatePDFReportFormmater(cellvalue, options, rowObject) {
        //height='50px' width='50px'
        var value = rowObject.FormInstanceID;

        cellvalue = "<img src='/Content/css/custom-theme/images/pdf.png' alt='PDFCollateral' style='margin-left: 5px;' title = 'PDF Collateral' onclick='javascript:generateCollateral(" + cellvalue + "," + true + ");'/>";

        //not allowed frmom UI
        //cellvalue = cellvalue + "<img src='/Content/css/custom-theme/images/check.png'  alt='Compliance Validations' style='margin-left: 5px;'  title = 'PDF Collateral Compliance Validations' onclick='javascript:viewComplianceIssues(" + value + ",0);'/>";

        return cellvalue;
    }

    function generateWordReportFormmater(cellvalue, options, rowObject) {
        //height='50px' width='50px'
        cellvalue = "<img src='/Content/css/custom-theme/images/word.png' alt='WordsCollateral' style='margin-left: 5px;'  title = 'Word Collateral' onclick='generateCollateral(" + cellvalue + "," + false + ")'/>";
        return cellvalue;
    }

    $(document).ready(function () {
        viewComplianceIssues = function (formInstanceId) {
            var viewCompianceIssue = new viewComplianceErrorDialog(formInstanceId, 0);
            viewCompianceIssue.show();
        }
        generateCollateral = function (formInstanceId, isPDF) {
            var selectedCollateralOptions = [];
            if (webConfigCollateralOption.toLowerCase() == "both") {
                selectedCollateralOptions = [];
                $('input:checkbox[name=collateralOption]').each(function () {
                    if ($(this).is(':checked'))
                        selectedCollateralOptions.push($(this).val());
                });
            }
            else
                selectedCollateralOptions.push(webConfigCollateralOption);

            if (webConfigCollateralOption.toLowerCase() == "both" && selectedCollateralOptions.length == 0) {
                messageDialog.show('Please select atleast one Collateral Option.');
                return false;
            }
            var description = "";

            var accountID = selectedRowData.AccountID;
            var accountName = selectedRowData.AccountName;
            var folderID = selectedRowData.FolderID;
            var folderName = selectedRowData.FolderName;
            var versionNumber = selectedRowData.FolderVersionNumber;
            var folderVersionID = selectedRowData.FolderVersionID;
            var effDate = selectedRowData.FolderVersionEffectiveDate;

            var rowData = $(elementIDs.relatedDocumentGridJQ).jqGrid('getRowData', formInstanceId);
            var selectedFormName = rowData.FormName;
            var documentType = rowData.DocumentType;
            var formDesignVersionID = rowData.FormDesignVersionID;

            var effDate = selectedRowData.FolderVersionEffectiveDate;
            //var effdateformt = new Date(effDate).toLocaleDateString();

            description = 'Genrated Collateral for account ' + accountName + ' having Folder ' + folderName + ' With Effective Date ' + effDate;
            description += ' and Version ' + versionNumber + ' has document ' + selectedFormName + ' of Type ' + documentType;

            var URL = URLs.checkPermission.replace(/\{ReportTemplateID\}/, reportTemplateID).replace(/\{FolderVersionEffDt\}/, effDate);

            var promise = ajaxWrapper.getJSON(URL);
            promise.done(function (hasAccess) {
                if (hasAccess == true) {
                    var url = URLs.generateCollateral;
                    var stringData = "accountID=" + accountID;
                    stringData += "<&folderID=" + folderID;
                    stringData += "<&formInstanceID=" + formInstanceId;
                    stringData += "<&folderVersionID=" + folderVersionID;
                    stringData += "<&reportTemplateName=" + selectedReportName;
                    stringData += "<&reportTemplateID=" + reportTemplateID;
                    stringData += "<&folderVersionEffDt=" + effDate;
                    stringData += "<&description=" + description;
                    stringData += "<&formDesignVersionID=" + formDesignVersionID
                    stringData += "<&collateralOptions=" + selectedCollateralOptions;
                    stringData += "<&isPDF=" + isPDF;
                    stringData += "<&documentName=" + selectedFormName;

                    $.download(url, stringData, 'post');
                }
                else {
                    messageDialog.show('You do not have permission to access this Collateral template.');
                }
            });
        }
    });


    return {
        show: function () {
            $(elementIDs.sourceDialogJQ).dialog('option', 'title', "Select Source Document");
            $(elementIDs.sourceDialogJQ).dialog("open");
            loadSelectGrid();
            $(elementIDs.sourceDialogJQ).dialog({
                position: {
                    my: 'center',
                    at: 'center'
                },
            });
        }
    }
}

var viewComplianceErrorDialog = function (formInstanceId, collateralQueueId) {
    this.URL = {
        getComplianceValidationlog: '/DocumentCollateral/GetComplianceValidationlog?formInstanceId=' + formInstanceId + '&collateralQueueId=' + collateralQueueId
    };
    this.formInstanceId = formInstanceId;

    var elementIDs = {
        sourceDialogJQ: "#complianceErrorDialog",
        viewErrors: "viewErrors",
        viewErrorsJS: "#viewErrors",
    };

    var selRowId;

    function init() {
        $(elementIDs.sourceDialogJQ).dialog({
            autoOpen: false,
            height: 700,
            width: 900,
            modal: true
        });
    }
    init();

    viewComplianceErrorDialog.prototype.loadSelectGrid = function () {
        var currentInstance = this;
        var url = currentInstance.URL.getComplianceValidationlog;
        var promise = ajaxWrapper.getJSON(url);
        //register ajax success callback
        promise.done(function (result) {
            currentInstance.bindGrid(result);
        });
        promise.fail(currentInstance.showError);
    };

    viewComplianceErrorDialog.prototype.bindGrid = function (rowData) {
        var gridGlobalSettingOption = new GridApi.gridGlobalSetting();

        var columnDefs = [
             { headerName: 'ComplianceType', field: 'ComplianceType', width: 40 },
             { headerName: 'ValidationType', field: 'ValidationType', width: 40 },
             { headerName: 'Error', field: 'Error', width: 300, filter: 'agTextColumnFilter' }
        ];

        var gridOptions = gridGlobalSettingOption.defaultGridOption();
        gridOptions.pivotPanelShow = false;
        gridOptions.pivotTotals = false;
        gridOptions.showToolPanel = false;
        gridOptions.defaultColDef.enablePivot = false;
        gridOptions.defaultColDef.enableValue = false;
        gridOptions.defaultColDef.enableRowGroup = false;
        gridOptions.defaultColDef.headerComponentParams.enableMenu = false;
        gridOptions.toolPanelSuppressSideButtons = true;
        gridOptions.columnDefs = columnDefs;
        gridOptions.rowData = rowData;
        gridOptions.onGridReady = function (e) {
            //  autoSizeAll(e);
            //e.api.sizeColumnsToFit();
            var currentInstance = this;
            var allColumnIds = [];
            gridOptions.columnApi.getAllColumns().forEach(function (column) {
                allColumnIds.push(column.colId);
            });
            gridOptions.columnApi.autoSizeColumns(allColumnIds);
            gridOptions.api.resetRowHeights();
        };

        agGrid.LicenseManager.setLicenseKey(license.agGrid);

        $(elementIDs.viewErrorsJS).html('');

        //var gridDivList = $(elementIDs.viewErrorsSJ);//document.querySelector('#myGrid');
        //var gridDiv = gridDivList[0];
        // gridDiv.gridOptions = gridOptions;
        var gridDiv = document.querySelector(elementIDs.viewErrorsJS);

        new agGrid.Grid(gridDiv, gridOptions);


        GridApi.renderGridHeaderRow(elementIDs.viewErrorsJ, "View", gridOptions);
    }
    viewComplianceErrorDialog.prototype.show = function () {
        $(elementIDs.sourceDialogJQ).dialog('option', 'title', "View Compliance");
        $(elementIDs.sourceDialogJQ).dialog("open");
        this.loadSelectGrid();
    }
}

var settingImages = function () {
    isInitialized = false;
    tenantId = 1;

    var URLs = {
        getCollateralImageList: "/DocumentCollateral/GetCollateralImageList",
        deleteCollateralImage: "/DocumentCollateral/DeleteImage",
    };

    var elementIDs = {
        collateralImageListGridJQ: "#collateralImageListGrid",
        collateralImageListGrid: "collateralImageListGrid",
        addCollateralImageListGridJQ: "addCollateralImageListGridJQ",
        deleteCollateralImageListGridJQ: "deleteCollateralImageListGridJQ",
        uploadSpan: '#imageDespSpan',
        imageDespSpan: '#uploadSpan',
        btnUploadCom: '#btnUploadCom'
    };

    //this function is called below soon after this JS file is loaded
    function init() {


        $(document).ready(function () {
            if (isInitialized == false) {



                isInitialized = true;

            }
            loadcollateralImageListGrid();

        });
    }

    function loadcollateralImageListGrid() {
        var colArray = ['', 'Images', 'Name', 'Path', 'Description'];
        //set column models
        var colModel = [];
        colModel.push({ name: 'ImageID', index: 'ImageID', key: true, editable: false, hidden: true });
        colModel.push({
            name: 'URL', index: 'URL', editable: false, width: '250px', align: 'center',
            formatter: function (cellvalue, options, rowobject) {
                // return "<img alt='Smiley face' height='42' width='42' src='"+cellvalue+"' >";
                return "<a href='" + cellvalue + "' download><img alt='Smiley face' height='42' width='42' src='" + cellvalue + "' ></a>"
            }
        });
        colModel.push({ name: 'Name', index: 'Name', editable: false, width: '150px' });
        colModel.push({ name: 'URL', index: 'URL', editable: false, width: '200px' });
        colModel.push({ name: 'Description', index: 'Description', editable: false, width: '400px' });
        //adding the pager element
        $(elementIDs.collateralImageListGridJQ).parent().append("<div id='p" + elementIDs.collateralImageListGrid + "'></div>");


        //clean up the grid first - only table element remains after this
        $(elementIDs.collateralImageListGridJQ).jqGrid('GridUnload');
        $(elementIDs.collateralImageListGridJQ).jqGrid({
            datatype: 'json',
            url: URLs.getCollateralImageList,
            cache: false,
            altclass: 'alternate',
            colNames: colArray,
            colModel: colModel,
            caption: '',
            height: '200',
            rowNum: 10,
            //loadonce: true,
            //autowidth: true,
            viewrecords: true,
            hidegrid: false,
            hiddengrid: false,
            ignoreCase: true,
            caption: "List of Images(CMYK format) used in Collateral Documents",
            sortname: 'Description',
            pager: '#p' + elementIDs.collateralImageListGrid,
            altRows: true,
            loadComplete: function () {
                //if (roleId == Role.HNESuperUser) {
                //    $("#" + elementIDs.addcollateralImageListGri3265JQ).addClass('disabled-button');
                //    $("#" + elementIDs.editcollateralImageListGridJQ).addClass('disabled-button');
                //    $("#" + elementIDs.deletecollateralImageListGridJQ).addClass('disabled-button');
                //}
                return true;
            }
        });

        var pagerElement = '#p' + elementIDs.collateralImageListGrid;
        $(pagerElement).find('input').css('height', '20px');

        $(pagerElement).find('#first_p' + elementIDs.collateralImageListGrid).remove();
        $(pagerElement).find('#last_p' + elementIDs.collateralImageListGrid).remove();

        //remove default buttons
        $(elementIDs.collateralImageListGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

        $(elementIDs.collateralImageListGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

        $(elementIDs.collateralImageListGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon-plus', title: 'Add', id: elementIDs.addcollateralImageListGridJQ,
                onClickButton: function () {
                    imageDialog.show('', 'add');
                }
            });

        $(elementIDs.btnUploadCom).on('click', function () {
            complianceDialog.show();
        });


        $(elementIDs.collateralImageListGridJQ).jqGrid('navButtonAdd', pagerElement,
           {
               caption: '', buttonicon: 'ui-icon-trash', title: 'Remove', id: elementIDs.deletecollateralImageListGridJQ,
               onClickButton: function () {
                   //remove selected row
                   var ImageID = $(elementIDs.collateralImageListGridJQ).jqGrid('getGridParam', 'selrow');
                   var rowData = $(elementIDs.collateralImageListGridJQ).getRowData(ImageID);
                   if (ImageID != null)
                       //imageDialog.show(rowData, 'delete');
                       confirmDialog.show('Are you sure want to delete?', function () {
                           confirmDialog.hide();
                           var data = {
                               id: rowData.ImageID
                           }
                           var promise = ajaxWrapper.postJSON(URLs.deleteCollateralImage, data);
                           //callback function for ajax request success.
                           promise.done(function (xhr) {
                               if (xhr.Result === ServiceResult.FAILURE) {
                                   if (xhr.Items[0].Messages.length > 0) messageDialog.show(xhr.Items[0].Messages[0]);
                                   else messageDialog.show("Error while Delete");
                               }
                               else {
                                   messageDialog.show("Successfully deleted");
                                   loadcollateralImageListGrid();
                               }
                           });
                       })
                   else
                       messageDialog.show("Not found");
               }
           });


    }




    init();


}();

var complianceDialog = function () {
    var URLs = {
        make508: '/DocumentCollateral/make508?',
    };
    var elementIDs = {
        comDialog: "#comDialog",
        btnUploadJQ: '#btnUpload',
        uploadSpanJQ: '#uploadSpan',
        docFileJQ: '#docFile',
        inkDownload: '#inkDownload'
    };
    function init() {
        $(elementIDs.comDialog).dialog({
            autoOpen: false,
            height: 250,
            width: 450,
            modal: true
        });
        //register event for save button click on dialog
        $(elementIDs.btnUploadJQ).on('click', function () {

            var docFileValue = $(elementIDs.docFileJQ).val().trim();
            if (docFileValue == "" || validate(docFileValue) == false) {
                $(elementIDs.uploadSpanJQ).parent().addClass('has-error');
                $(elementIDs.btnUploadJQ).addClass('form-control');
                $(elementIDs.uploadSpanJQ).text("Please upload docx file");
                return false;
            }
            else {
                var formData = undefined;
                //call is from generate reports use form[1]
                    formData = new FormData($('form')[0]);

                    $.ajax({
                    url: 'MakeCompliance',
                    type: 'POST',
                    success: imageSuccess,
                    data: formData,
                    cache: false,
                    contentType: false,
                    processData: false
                });
               
            }

        });
        function validate(fileName) {

            var extension = fileName.replace(/^.*\./, '');
            switch (extension.toLowerCase()) {
                case 'docx':
                    return true;
                    break;
                default:
                    return false;
            }
        }
        function imageSuccess(xhr) {
            if (xhr.Result === ServiceResult.SUCCESS) {
                $(elementIDs.inkDownload).empty();
                $(elementIDs.inkDownload).append(
                             " <a style='color:blue;' href='" + xhr.Items[0].Messages[0] + "'  target='_blank'>508 Compliance</a><a style='color:blue; margin-left: 15px; border-left: 1px solid #ccc; padding-left: 15px;' href='" + xhr.Items[0].Messages[1] + "' download target='_blank'>Print X Ready</a></br>");
            }
            else {

                //if (imageDialogState === "add") {
                if (xhr.Items.length > 0) {
                    messageDialog.show('Error occur in conversion');
                    console.log(xhr.Items[0].Messages);
                }
            }
            $(elementIDs.uploadSpanJQ).parent().removeClass('has-error');
            $(elementIDs.imageFileJQ).removeClass('form-control');
            $(elementIDs.uploadSpanJQ).text('');
        }


    }

    init();

    return {
        show: function () {
            $(elementIDs.uploadSpanJQ).parent().removeClass('has-error');
            $(elementIDs.imageFileJQ).removeClass('form-control');
            $(elementIDs.uploadSpanJQ).text('')
            $(elementIDs.docFileJQ).val('')
            $(elementIDs.inkDownload).empty();
            $(elementIDs.comDialog).dialog('option', 'title', '508 Compliance PDF and Print x Ready');

            //open the dialog - uses jqueryui dialog
            $(elementIDs.comDialog).dialog("open");
        }
    }
}();


var complianceDialog1 = function () {
    var URLs = {
        make508: '/DocumentCollateral/make508?',
    };
    var element1IDs = {
        comDialog: "#comDialog1",
        btnUploadJQ: '#btnUpload1',
        uploadSpanJQ: '#uploadSpan1',
        docFileJQ: '#docFile1',
        docFileJQ1: '#docFile2',
        inkDownload: '#inkDownload1',
        selectedSourceDocumentList: '#selectedSourceDocumentList',
        relatedDocumentGridJQ: '#documentsInFolder',
        alreadyConverted508 :'#alreadyConverted508',
        lblUploadDoc :'#lblUploadDoc',
    };
    function init() {
        $(element1IDs.comDialog).dialog({
            autoOpen: false,
            height: 250,
            width: 450,
            modal: true
        });

        $(element1IDs.alreadyConverted508).on('click', function () {
            if ($(this).is(':checked')) {
                $(element1IDs.lblUploadDoc).text('Upload Converted 508 Pdf');
            }
            else {
                $(element1IDs.lblUploadDoc).text('Upload docx file');
            }
        });

        //register event for save button click on dialog
        $(element1IDs.btnUploadJQ).on('click', function () {

            var docFileValue = $(element1IDs.docFileJQ).val().trim();
            if (docFileValue == "" || validate(docFileValue) == false) {
                $(element1IDs.uploadSpanJQ).parent().addClass('has-error');
                $(element1IDs.btnUploadJQ).addClass('form-control');
                $(element1IDs.uploadSpanJQ).text("Please upload docx file");
                return false;
            }
            docFileValue = $(element1IDs.docFileJQ1).val().trim();
            if (docFileValue == "" || validatePDF(docFileValue) == false) {
                $(element1IDs.uploadSpanJQ).parent().addClass('has-error');
                $(element1IDs.btnUploadJQ).addClass('form-control');
                $(element1IDs.uploadSpanJQ).text("Please upload printx file");
                return false;
            }
            else {
                var formData ="", rows ="", accountID = "", accountName = "", folderID = "", folderName = "", versionNumber = "", folderVersionID = "", effDate = "", 
                    collateralName= selectedReportName, FormDesignID="", FormDesignVersionID="",FormInstanceID="",FormName=""

                var selSBRowId = $(element1IDs.selectedSourceDocumentList).jqGrid("getGridParam", "selrow");
                var selOtherRowId = $(element1IDs.relatedDocumentGridJQ).jqGrid("getGridParam", "selrow");

                var isSB = false;
                if (selSBRowId == null && selOtherRowId == null) {
                    messageDialog.show('Please select a row.');
                    return;
                }
                if (selSBRowId != null && selOtherRowId == null) {
                    isSB = true;
                }


                //if(selectedReportName  == "Medicare SB"){
                if (isSB == true) {


                    var selRowId = $(element1IDs.selectedSourceDocumentList).jqGrid("getGridParam", "selrow");
                    if(selRowId == null){
                        messageDialog.show('Please select a row.');
                        return;
                    }
                    row = $(element1IDs.selectedSourceDocumentList).jqGrid('getRowData', selRowId);
                    accountID = row['AccountID']
                    accountName = row['AccountName']
                    folderID = row['FolderID']
                    folderName = row['FolderName']
                    versionNumber = row['FolderVersionNumber']
                    folderVersionID = row['FolderVersionID']
                    effDate = row['FolderVersionEffectiveDate']
                    FormDesignID = row['FormDesignID']
                    FormDesignVersionID = row['FormDesignVersionID']
                    FormInstanceID= row['FormInstanceID']
                    FormName= row['FormName']
                }
                else{    
                    var selRowId = $(element1IDs.relatedDocumentGridJQ).jqGrid("getGridParam", "selrow");
                    if(selRowId == null){
                        messageDialog.show('Please select a row.');
                        return;
                    }
                    row = $(element1IDs.relatedDocumentGridJQ).jqGrid('getRowData', selRowId);
                    accountID = row['AccountID']
                    accountName = row['AccountName']
                    folderID = row['FolderID']
                    folderName = row['FolderName']
                    versionNumber = row['FolderVersionNumber']
                    folderVersionID = row['FolderVersionID']
                    effDate = row['FolderVersionEffectiveDate']
                    FormDesignID = row['FormDesignID']
                    FormDesignVersionID = row['FormDesignVersionID']
                    FormInstanceID= row['FormInstanceID']
                    FormName= row['FormName']
                }
                formData = new FormData($('form')[2]);
                formData.append("accountID", accountID);
                formData.append("accountName", accountName);
                formData.append("folderID", folderID);
                formData.append("versionNumber", versionNumber);
                formData.append("folderName", folderName);
                formData.append("folderVersionID", folderVersionID);
                formData.append("effDate", effDate);
                formData.append("FormDesignID", FormDesignID);
                formData.append("FormDesignVersionID", FormDesignVersionID);
                formData.append("FormInstanceID", FormInstanceID);
                formData.append("FormName", FormName);
                formData.append("collateralName", collateralName);
                var alreadyConverted508 = false;
                alreadyConverted508 = $(element1IDs.alreadyConverted508).is(":checked");
                formData.append("alreadyConverted508", alreadyConverted508);

                $.ajax({
                url: 'MakeComplianceWithData',
                type: 'POST',
                success: imageSuccess,
                data: formData,
                cache: false,
                contentType: false,
                processData: false
            });
            }

        });
        function validate(fileName) {

            var extension = fileName.replace(/^.*\./, '');
            switch (extension.toLowerCase()) {
                case 'docx':
                    {
                        if ($(element1IDs.alreadyConverted508).is(':checked'))
                            return false;
                        else
                            return true;
                    }
                    break;
                case 'pdf':
                    {
                        if ($(element1IDs.alreadyConverted508).is(':checked'))
                            return true;
                         else
                             return false;                        
                    }
                default:
                    return false;
            }
        }function validatePDF(fileName) {

            var extension = fileName.replace(/^.*\./, '');
            switch (extension.toLowerCase()) {
                case 'pdf':
                    return true;
                    break;
                default:
                    return false;
            }
        }
        function imageSuccess(xhr) {
            if (xhr.Result === ServiceResult.SUCCESS) {
                $(element1IDs.inkDownload).empty();
                $(element1IDs.inkDownload).append("Sucessfully Uploaded");
               // $(element1IDs.inkDownload).append(
                 //            " <a style='color:blue;' href='" + xhr.Items[0].Messages[0] + "'  target='_blank'>508 Compliance</a><a style='color:blue; margin-left: 15px; border-left: 1px solid #ccc; padding-left: 15px;' href='" + xhr.Items[0].Messages[1] + "' download target='_blank'>Print X Ready</a></br>");*/
            }
            else {

                //if (imageDialogState === "add") {
                if (xhr.Items.length > 0) {
                    messageDialog.show('Error occur in conversion');
                    console.log(xhr.Items[0].Messages);
                }
            }
            $(element1IDs.uploadSpanJQ).parent().removeClass('has-error');
            $(element1IDs.imageFileJQ).removeClass('form-control');
            $(element1IDs.uploadSpanJQ).text('');
        }


    }

    init();

    return {
        show: function () {
            $(element1IDs.uploadSpanJQ).parent().removeClass('has-error');
            $(element1IDs.imageFileJQ).removeClass('form-control');
            $(element1IDs.uploadSpanJQ).text('')
            $(element1IDs.docFileJQ).val('')
            $(element1IDs.docFileJQ1).val('')
            $(element1IDs.inkDownload).empty();
            $(element1IDs.comDialog).dialog('option', 'title', '508 Compliance PDF and Print x Ready');

            //open the dialog - uses jqueryui dialog
            $(element1IDs.comDialog).dialog("open");
        }
    }
}();

//contains functionality for the image add/update dialog
var imageDialog = function () {
    var imageDialogState;
    var URLs = {

        DeleteImage: '/DocumentCollateral/DeleteImage?',
    }
    var elementIDs = {
        imageDialog: '#imageDialog',
        imageNameJQ: '#imageDesp',
        imagNameJQ: '#imagName',
        btnsaveimageJQ: '#btnsaveimage',
        collateralImageListGridJQ: "#collateralImageListGrid",
        imageDespSpanJQ: '#imageDespSpan',
        imagNameSpanJQ: '#imagNameSpan',
        uploadSpanJQ: '#uploadSpan',
        imageFileJQ: '#imageFile',
        groupNamesAutoCompltDDLJQ: '#groupNamesAutoCompltDDL'
    }


    function init() {
        //register dialog for grid row add/edit
        $(elementIDs.imageDialog).dialog({
            autoOpen: false,
            height: 250,
            width: 450,
            modal: true
        });

        //register event for save button click on dialog
        $(elementIDs.btnsaveimageJQ).on('click', function () {

            var categoryName = $(elementIDs.imageNameJQ).val().trim();
            var imageFileValue = $(elementIDs.imageFileJQ).val().trim();
            var imagNameValue = $(elementIDs.imagNameJQ).val().trim();
            if (categoryName == "") {
                $(elementIDs.imageDespSpanJQ).parent().addClass('has-error');
                $(elementIDs.imageNameJQ).addClass('form-control');
                $(elementIDs.imageDespSpanJQ).text("Description is required");
                return false;
            }
            else if (imagNameValue == "") {
                $(elementIDs.imagNameSpanJQ).parent().addClass('has-error');
                $(elementIDs.imagNameJQ).addClass('form-control');
                $(elementIDs.imagNameSpanJQ).text("Name is required");
                return false;
            }
            else if (imageFileValue == "" || validate(imageFileValue) == false) {
                $(elementIDs.uploadSpanJQ).parent().addClass('has-error');
                $(elementIDs.imageFileJQ).addClass('form-control');
                $(elementIDs.uploadSpanJQ).text("Please upload image(jpg) file");
                return false;
            }
            else {

                var formData = new FormData($('form')[1]);
                $.ajax({
                    url: 'SaveImage',
                    type: 'POST',
                    success: imageSuccess,
                    data: formData,
                    cache: false,
                    contentType: false,
                    processData: false
                });
            }

        });
        function validate(fileName) {

            var extension = fileName.replace(/^.*\./, '');
            switch (extension.toLowerCase()) {
                case 'jpg':
                    return true;
                    break;
                default:
                    return false;
            }
        }
        //ajax success callback - for add/edit
        function imageSuccess(xhr) {
            if (xhr.Result === ServiceResult.SUCCESS) {
                /*if (imageDialogState === "add") {
                    messageDialog.show(FolderCategoryMessages.addCategoryMsg);
                }
                else {
                    messageDialog.show(FolderCategoryMessages.updateCategoryMsg);
                }*/

                $(elementIDs.collateralImageListGridJQ).setGridParam({ datatype: 'json', page: 1 }).trigger('reloadGrid');;  //.trigger("reloadGrid");
                $(elementIDs.imageDialog).dialog('close');
            }
            else {

                //if (imageDialogState === "add") {
                if (xhr.Items.length > 0) {
                    messageDialog.show(xhr.Items[0].Messages);
                }
                else {
                    messageDialog.show("Image uploaded sucessfully");
                }
                //  }
                /* else if (imageDialogState === "update") {
                     if (xhr.Items.length > 0) {
                         messageDialog.show(xhr.Items[0].Messages);
                     }
                     else {
                         messageDialog.show(FolderCategoryMessages.updateCategoryErrorMsg);
                     }
                 }
                 else {
                     //messageDialog.show(FolderCategoryMessages.updateCategoryErrorMsg);
                 }*/
                $(elementIDs.imageNameJQ).parent().removeClass('has-error');
                $(elementIDs.imageNameJQ).removeClass('form-control');
                $(elementIDs.groupDDLJQ).parent().removeClass('has-error');
                $(elementIDs.groupDDLJQ).removeClass('form-control');
                $(elementIDs.imageDespSpanJQ).text('')
                $(elementIDs.imagNameSpanJQ).text('')
                $(elementIDs.groupNameSpanJQ).text('')
            }
        }
        /*   function imageSuccess(data) {
               if (data!=undefined)
               {
                   if (data.Result ==2)//failure
                   {
                       if (data.Items != undefined) {
                           if (data.Items.length >=1)
                           {
                               if (data.Items.length > 0) {
                                   if (data.Items[0].Messages.length > 0) {
                                       var message = data.Items[0].Messages[0];
                                       $(elementIDs.imageDespSpanJQ).parent().addClass('has-error');
                                       $(elementIDs.imageFileJQ).addClass('form-control');
                                       $(elementIDs.imageDespSpanJQ).text(message);
                                   }
                               }
                           }
                       }
                   }
               }
           }*/

    }



    init();

    //to fill drop down list of Group names


    return {
        show: function (rowData, action) {
            $(elementIDs.imageDialog + ' input').val("");
            imageDialogState = action;

            $(elementIDs.imageNameJQ).parent().removeClass('has-error');
            $(elementIDs.imageNameJQ).removeClass('form-control');

            $(elementIDs.groupDDLJQ).parent().removeClass('has-error');
            $(elementIDs.groupDDLJQ).removeClass('form-control');

            if (imageDialogState == "add") {
                $(elementIDs.imageDialog).dialog('option', 'title', 'Add Image');
            }
            else if (imageDialogState == "update") {

                //var imageID = $(elementIDs.collateralImageListGridJQ).jqGrid('getGridParam', 'selrow');
                //categoryData = {
                //    imageID: imageID,
                //    categoryName: categoryName,
                //}
                //var url = URLs.DeleteCategory;

                //updatecategoryData(url, imageData, imageDialogState)

            }
            else {
                $(elementIDs.imageDialog).dialog('option', 'title', 'Delete Image');
                $(elementIDs.imageNameJQ).val(rowData.FolderVersionCategoryName);
            }
            //open the dialog - uses jqueryui dialog
            $(elementIDs.imageDialog).dialog("open");
        }
    }
}();





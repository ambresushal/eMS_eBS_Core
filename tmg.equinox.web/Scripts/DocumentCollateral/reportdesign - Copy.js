/* Report Design Template Grid*/
var reportTemplate = function () {

    var indexdrp = 0;
    var indextxt = 0;
    var indexchk = 0;
    var indexdoc = 0;
    var indexdocvrsn = 0;
    var indexdocedit = 0;
    //variables required for tab management
    var tabsReportVersion;
    var tabIndex = 1;
    var tabCount = 0;
    var tabNamePrefix = 'reportDesign-';
    var selectedReportID = 0;
    var selectedReportName = '';
    var selectedReportTemplateVersionID = 0;
    var selectedReportTemplateLocation = '';

    //urls to be accessed for form design
    var URLs = {
        //get Document Design List
        formDesignList: '/DocumentCollateral/FormDesignList?tenantId=1',
        //get Report Names
        reportList: '/DocumentCollateral/GetReportNames',
        //get Report Names for Report Generation
        reportListGen: '/DocumentCollateral/GetReportNamesForGeneration',
        //Edit Template Version for Report
        reportEditTmplVrsn: '/DocumentCollateral/UpdateReportTemplateVersion?TenantID={TenantID}&ReportTemplateVersionID={ReportTemplateVersionID}',
        //get Report Template Versions List
        reportVersionList: '/DocumentCollateral/GetReportTemplateVersions?TenantID={TenantID}&ReportTemplateID={ReportTemplateID}',
        //get related Documents For Report Generation
        documentsForReport: '/DocumentCollateral/GetDocumentsForReportGeneration?ReportTemplateVersionID={ReportTemplateVersionID}&AccountID={AccountID}&FolderVersionID={FolderVersionID}',
        //get Document Design Version
        formDesignVersionList: '/DocumentCollateral/FormDesignVersionList?tenantId=1&formDesignId={formDesignId}',
        //get Template Document Mapping
        formTemplateDocumentMapping: '/DocumentCollateral/GetDocumentDesignVersion'
    };


    var elementIDs = {
        //table element for the Document Design Grid
        formDesignGrid: 'fdgReport',
        //with hash for use with jQuery
        formDesignGridJQ: '#fdgReport',
        //container element for the form design tabs
        formDesignTabs: '#formdesigntabs',
        //Edit Report and Report Version Tab
        viewReportTemplateTab: '#viewReportTemplate',

        reportUploadTab: '#uploadTab',
        reportDownloadTab: '#downloadTab',
        reportGenerateTab: '#reportGenerate',
        //Create Report Template and Version Tab
        reportTmplViewTab: '#viewReportTemplate',
        
        reportUploadTable:'tempDocMapdiv',
        reportUploadTableID: '#tempDocMapdiv',
        //table element for the Document Design Version Grid
        formDesignVersionGrid: 'fdvgReport',
        //with hash for use with jQuery
        formDesignVersionGridJQ: '#fdvgReport',
        //Fill the table representing [Report Template - Document Version] Mapping
        formTemplateDocumentMappingGrid: 'tempDocMap',
        formTemplateDocumentMappingGridJQ: '#tempDocMap',
        reportGrid: 'fdgReportgen',
        //with hash for use with jQuery
        reportGridJQ: '#fdgReportgen', 
        relatedDocumentGrid: 'documentsInFolder',
        relatedDocumentGridJQ: '#documentsInFolder',
        reportTemplateGrid: 'rptTmplNames',
        reportTemplateGridJQ: '#rptTmplNames',
        reportTemplateVersionGrid: 'rptTmplVersions',
        reportTemplateVersionGridJQ: '#rptTmplVersions'
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

    

    //this function is called below soon after this JS file is loaded 
    //generates the tabs and loads the Document Design Grid
    function init() {

        // Set Cancel Date to EOT.
        $(function () {
            $("#effDate").datepicker();
            //$("#canDate").datepicker();
            var date = "01/01/2099";

            $("#canDate").val(date);
        });

        // Downloads the XML for the selected version of the Document.
        $(function () {
            downloadDocumentDesignXML = function (TenantID, idDwn, DocumentDesignId) {
                if (typeof TenantID === 'undefined')
                    TenantID = 1;
                var FormDesignVersionId = $(idDwn).val();                
                if(FormDesignVersionId){
                    FormDesignVersionId = FormDesignVersionId.split('-')[0];

                    window.location.href = "/DocumentCollateral/DownloadDocumentDesignVersionXML?TenantID=" + TenantID + "&FormDesignVersionId=" + FormDesignVersionId;
                }
                else
                    messageDialog.show('Please select Version to Download the XML for.');
            }
        });

        // Prepares JSON from grid data to be sent over to controller on Form Submit.
        $(function () {
            $("#saveTemplateButton").click(function () {
                var effDate = $('#effDate').val();
                var error = true;
                $('#reportTemplateID').val(reportTemplate.selectedReportID());
                $('#reportTemplateName').val(reportTemplate.selectedReportName());
                //var reportName = $('#reportName').val();
                if (effDate) {
                    if ($('#uploadTemplate').val()) {
                        
                        var grid = $(elementIDs.formTemplateDocumentMappingGridJQ);
                        var gridRows = grid.jqGrid('getRowData');
                        //var arr = [];
                        //var indexestodelete = [];
                        //arr = gridRows;
                        //$.each(arr, function (index) {
                        //    delete arr[index]['DocumentVersions'];
                        //    delete arr[index]['DataSourceName'];
                        //    delete arr[index]['Action'];
                        //});

                        if (typeof gridRows != undefined) {
                            var i = gridRows.length - 1;

                            for (; i > -1; i--) {
                                if (!gridRows[i]['IsSelected'])
                                    gridRows.splice(i, 1);
                                else {
                                    delete gridRows[i]['DocumentVersions'];
                                    delete gridRows[i]['DataSourceName'];
                                    delete gridRows[i]['Action'];
                                }
                            }
                        }
                        var dataToSend = JSON.stringify(arr);

                        if (dataToSend.indexOf('"IsSelected":"true"') == -1) {
                            messageDialog.show('Please select atleast one Document to map the Report Template to.');
                            return false;
                        }                        
                        $('#jsonData').val(dataToSend);
                        $('#uploadedFileName').val($('#uploadTemplate').val());
<<<<<<< .mine
                        error = true;
=======

                        return true;
>>>>>>> .r6116
                    }
                    else {
                        messageDialog.show('Please select the File to be Uploaded.');
                        error= false;
                    }
                }
                else {
                    messageDialog.show("Effective Date is Required Input.");
                    error= false;
                }

                if (error) {                    
                    var formData = new FormData();
                    var totalFiles = document.getElementById("uploadTemplate").files.length;
                    for (var i = 0; i < totalFiles; i++) {
                        var file = document.getElementById("uploadTemplate").files[i];

                        formData.append("FileUpload", file);
                    }
                    formData.append("reportTemplateID", $('#reportTemplateID').val());
                    formData.append("reportTemplateName", $('#reportTemplateName').val());
                    formData.append("reportDesc", $('#desc').val());
                    formData.append("jsonData", dataToSend);
                    formData.append("effDate", $('#effDate').val());
                    formData.append("canDate", $('#canDate').val());
                    formData.append("uploadedFileName", $('#uploadedFileName').val());
                    var reportid = $('#reportTemplateID').val();
                    // kick off AJAX
                    $.ajax({
                        url: '/DocumentCollateral/SaveReportTemplateVersion',
                        type: 'post',                        
                        data: formData,                        
                        dataType: "json",
                        contentType: false,
                        processData: false,
                        success: function (data) {                            
                            reportTemplate.viewReportTemplates(reportid);
                            //reset dialog elements
                            $(elementIDs.reportTmplDialog + ' div').removeClass('has-error');
                            $(elementIDs.reportTmplDialog).dialog('close');
                        },
                        error: function () {                            
                        alert("Error in file processing");
                    }
                    });
                    return true;
                }
            });
        });

        $(document).ready(function () {

            setValue = function (gridName, rowID, eleId, colName, prop) {
                //alert('pouio');
                var rowid = parseInt(eleId.slice(4));
                $(gridName).jqGrid('setCell', rowID, colName, $('#' + eleId).prop(prop));
                var griddataa = $(gridName).jqGrid('getRowData');
            }            

            editTemplateVersion = function (TenantID, ReportTemplateVersionID) {
                selectedReportTemplateVersionID = ReportTemplateVersionID;
                alert(TenantID + "_" + ReportTemplateVersionID);
                var url = URLs.reportEditTmplVrsn.replace(/\{TenantID\}/g, TenantID).replace(/\{ReportTemplateVersionID\}/g, ReportTemplateVersionID);
                window.open(url, "popupWindow", "width=1200,height=700,scrollbars=yes,resizable=yes");
                reportVersionConfig.reportConfigurationGrid(reportTemplate.selectedReportTemplateVersionID());
                alert(ReportTemplateVersionID);
            }            

            function showMessage(message, status, urlToRedirectTo) {
                if (status === 'Success') {
                    alert(message);
                    windows.location.href = urlToRedirectTo;
                }
                else
                    alert(message);
            };

            $('#folderDropDown').on('change', function () {
                var accountID = $('#accountDropDown').val();
                var folderVersionID = $('#folderDropDown').val();
                
                if (accountID && folderVersionID) {
                    selectDocumentsForReportGeneration(selectedReportTemplateVersionID, accountID, folderVersionID);
                    indexdoc = 0;
                }
            })

            $('#accountDropDown').on('change', function () {
                var accountID = $('#accountDropDown').val();
                $(elementIDs.relatedDocumentGridJQ).jqGrid('GridUnload');
                $("#generateReportbtn").hide();
                getAccountsFolders(selectedReportTemplateVersionID, accountID);
                indexdoc = 0;
            })
            
            $('#generateReportbtn').on('click', function () {
                //alert('gh');
                var url = "/DocumentCollateral/ViewReport"

                var grid = $(elementIDs.relatedDocumentGridJQ);
                var gridRows = grid.jqGrid('getRowData');
                var arr = [];
                var indexestodelete = [];
                arr = gridRows;

                var FormInstanceID = "";

                $.each(arr, function (index) {
                    if (arr[index]['IsSelected'] == 'true') {
                        FormInstanceID += arr[index]['FormInstanceID'] + ",";
                    }
                });               

                var jsonObject = JSON.stringify(arr);
                var accountID = $('#accountDropDown').val();
                var folderVersionID = $('#folderDropDown').val();

                var stringData = "accountID=" + accountID;
                stringData += "<&formInstanceID=" + FormInstanceID;
                stringData += "<&folderVersionID=" + folderVersionID;
                stringData += "<&reportTemplateName=" + selectedReportName;
                stringData += "<&reportTemplateLocation=" + selectedReportTemplateLocation;
                stringData += "<&reportTemplateVersionID=" + selectedReportTemplateVersionID;


                if (FormInstanceID == "") {
                    messageDialog.show('Please select atleast one Document to generate the Report.');
                    return false;
                }

                $.download(url, stringData, 'post');

            });

            
            //To remove style attribute so that Document Design tab is displayed after loading the page. 
            $(elementIDs.formDesignTabs).removeAttr("style");
            $(elementIDs.reportUploadTab).removeAttr("style");
            $(elementIDs.reportDownloadTab).removeAttr("style");
            $(elementIDs.reportGenerateTab).removeAttr("style");
            $(elementIDs.reportTmplViewTab).removeAttr("style");
            $(elementIDs.viewReportTemplateTab).removeAttr("style");
            


            //  $(elementIDs.reportUploadTableID).removeAttr("style");


            //jqueryui tabs
            tabsDesign = $(elementIDs.formDesignTabs).tabs();
            tabsUpl = $(elementIDs.reportUploadTab).tabs();
            tabsDld = $(elementIDs.reportDownloadTab).tabs();
            tabsGen = $(elementIDs.reportGenerateTab).tabs();
            tabsTmpl = $(elementIDs.reportTmplViewTab).tabs();
            tabsReportVersion = $(elementIDs.viewReportTemplateTab).tabs();
            //register event for closing a tab page - refer jquery ui documentation
            //event will be registered for each tab page loaded
            tabsDesign.delegate('span.ui-icon-close', 'click', function () {
                var panelId = $(this).closest('li').remove().attr('aria-controls');
                $('#' + panelId).remove();                
            });
            tabsUpl.delegate('span.ui-icon-close', 'click', function () {
                var panelId = $(this).closest('li').remove().attr('aria-controls');
                $('#' + panelId).remove();
            });
            tabsDld.delegate('span.ui-icon-close', 'click', function () {
                var panelId = $(this).closest('li').remove().attr('aria-controls');
                $('#' + panelId).remove();
            });
            tabsGen.delegate('span.ui-icon-close', 'click', function () {
                var panelId = $(this).closest('li').remove().attr('aria-controls');
                $('#' + panelId).remove();
            });
            tabsTmpl.delegate('span.ui-icon-close', 'click', function () {
                var panelId = $(this).closest('li').remove().attr('aria-controls');
                $('#' + panelId).remove();
            });
            tabsReportVersion.delegate('span.ui-icon-close', 'click', function () {
                var panelId = $(this).closest('li').remove().attr('aria-controls');
                $('#' + panelId).remove();
                tabCount--;
                tabIndex = 1;
                tabs.tabs('refresh');
            });

            //load the form design grid
            loadFormDesignGrid();
            //loads the form design version grid
            loadReportDesignTemplategrid();
            //loads the Report names grid
            generateReports();
            viewReportTemplates(0);
        });


    }  

    // View Report Templates - Add and Edit Templates
    function viewReportTemplates(reportid) {
        
        //set column list for grid
        var colArray = ['Report Id', 'Report Name'];
        var currentInstance=this;
        var rowindex = 1;
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
            caption: 'Reports',
            height: '350',
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
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
<<<<<<< .mine
            afterInsertRow: function (index) {
                var data = $(this).getRowData(index);                
                if (reportid === data.ReportId) {                    
                    rowindex = index;
                }
=======
            gridComplete: function () {
                if (selectedReportID)
                    $(this).jqGrid('setSelection', selectedReportID);
                else {
                    rowIDs = $(this).getDataIDs();
                    $(this).jqGrid('setSelection',rowIDs[0]);
                }
                    
>>>>>>> .r6116
            },
            gridComplete: function () {                
                $(this).jqGrid('setSelection', rowindex);
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

        // add filter toolbar to the top
        $(elementIDs.reportTemplateGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });


    }

    // View Report Template Version - Add and Edit Template Versions
    function viewReportTemplateVersion(ReportId, ReportName) {
        //set column list
        var colArray = ['ReportTemplateVersionID', 'Effective Date', 'Status', 'Version Number', 'Action', 'TemplateLocation', 'TemplateName'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'ReportTemplateVersionID', index: 'ReportTemplateVersionID', key:true, hidden: true, search: false, });
        colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', width: '315px', align: 'left', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions });
        colModel.push({ name: 'Status', index: 'Status', width: '315px', editable: false });
        colModel.push({ name: 'VersionNumber', index: 'VersionNumber', width: '315px', align: 'left', editable: false });
        colModel.push({ name: 'Action', index: 'Action', align: 'left', editable: false, formatter: editTmplVrsnFormatter });
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
            caption: 'Report Template Version List - ' + ReportName,
            forceFit: true,
            height: '350',
            autowidth: true,
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
            ExpandColumn: 'Label',
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.reportTemplateVersionGrid,
            sortname: 'ReportTemplateVersionID',
            altclass: 'alternate',
            //register events to load the form design version in a new tab page
            //this is on click of the span which has the image in the last column
            gridComplete: function () {
                var rows = $(this).getDataIDs();
                var reverse = rows.reverse();
                for (index = 0; index < rows.length; index++) {
                    row = $(this).getRowData(rows[index]);
                    $('#rptvrsn' + row.ReportTemplateVersionID).click(function () {
                        var reportTemplateVersionID = this.id.replace(/rptvrsn/g, '');
                        //launch a new tab here for this form design version
                        //TODO: pass TenantId too
                        var currentRow = $(elementIDs.reportTemplateVersionGridJQ).getRowData(reportTemplateVersionID);
                        //increment to manage adding /deletesof tab pages
                        var tabName = ReportName + '-' + currentRow.VersionNumber;
                        $(elementIDs.btnReportTemplateEdit).addClass('ui-state-disabled');
                        $(elementIDs.btnReportTemplateDelete).addClass('ui-state-disabled');
                        //if form design version is already loaded, do not load again but make it active
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
                        //if (foundIndex > 0) {
                        //    for (var i = 0; i < Finalized.length ; i++) {
                        //        if (Finalized[i].FORMDESIGNVERSIONID == formDesignVersionId && Finalized[i].ISFINALIZED == 1) {
                        //            $(".ui-tabs-anchor").trigger("click");
                        //        }
                        //    }
                        //}
                        if (foundIndex > 0) {
                            tabsReportVersion.tabs('option', 'active', foundIndex);
                        }
                        else {
                            tabIndex++;
                            tabCount++;
                            //create formDesignVersion instance - load tab     

                            //this function will check the user permission for form design(eg. portfolio designer).
                            //var hasPermission = checkUserPermissionForEditable(URLs.formDesignVersionList);
                            //if (hasPermission) {
                            var rptVersion = new reportConfigurationGrid(currentRow, reportTemplateVersionID, row.Status, tabNamePrefix + tabIndex, tabIndex - 1, tabCount, tabsReportVersion, ReportName);
                            rptVersion.loadTabPage();
                            //}
                        }
                    });                
                }
                //disableEditDeleteButtons(formDesignName);
                var rowId;
                if (rows.length > 0) {
                    rowId = rows[0];
                }
                //get the newly added id for the form design version 
                //var newName = $(elementIDs.formDesignVersionDialog + ' input').val();
                //var newId;
                //for (index = 0; index < rows.length; index++) {
                //    row = $(this).getRowData(rows[index]);
                //    if (newName === row.DisplayText) {
                //        newId = row.FormDesignId;
                //        break;
                //    }
                //}
                //if (newId !== undefined) {
                //    //set newly added row 
                //    $(this).jqGrid('setSelection', newId);
                //}
                //else {
                //    //set first row 
                $(this).jqGrid('setSelection', rowId);
                //}

                //to check for claims..  
                //var objMap = {
                //    edit: "#btnFormDesignVersionEdit",
                //    add: "#btnFormDesignVersionAdd",
                //    remove: "#btnFormDesignVersionDelete",
                //    finalized: "#btnFormDesignVersionFinalized",
                //    preview: "#btnFormDesignVersionPreview",
                //};
                //checkApplicationClaims(claims, objMap, URLs.formDesignVersionList);
                //authorizeDocumentDesignVersionList($(this), URLs.formDesignVersionList);

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
                //load Add Report dialog on click - see formDesignDialog function below                
                reportTmplVersionAddDialog.show(selectedReportID);
            }
        });

        //add filter toolbar at the top of grid
        $(elementIDs.reportTemplateVersionGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });

    }    

    function getAccountsFolders(selectedReportTemplateVersionID, account) {

        var url = '/DocumentCollateral/GetAccountsFolders?reportTemplateVersionID=' + selectedReportTemplateVersionID + "&AccountID=" + account;
        var promise = ajaxWrapper.getJSON(url);

        promise.done(function (result) {
            var accures = '';
            var accounts = [];
            $('#folderDropDown').empty();
            var _selectAccount = $('<select>');
            var _selectFolder = $('<select>');

            $.each(result, function (i, obj) {
                if (accounts.indexOf())
                    accounts.push({ AccountID: obj.AccountID, AccountName: obj.AccountName });
            });

            
            var _optionsFdr = $('<option></option>').val("").html("Select");
            _selectFolder.append(_optionsFdr);

            if (account == 0) {
                var _optionsAcc = $('<option></option>').val("").html("Select");
                _selectAccount.append(_optionsAcc);
            }

            $.each(result, function (i, obj) {
                accures += obj.AccountID + '_' + obj.AccountName + ';';
                var _optionsAcc = $('<option></option>').val(obj.AccountID).html(obj.AccountName);
                _optionsFdr = $('<option></option>').val(obj.FolderVersionID).html(obj.FolderName + '_' + obj.FolderVersionNumber);
                if (account == 0) {
                    if (_selectAccount.find('option[value="' + obj.AccountID + '"]').length == 0) {
                        _selectAccount.append(_optionsAcc);
                    }
                }
                _selectFolder.append(_optionsFdr);
            });

            if (account == 0) {
                $('#accountDropDown').append(_selectAccount.html());
            }
            $('#folderDropDown').append(_selectFolder.html());
        });

    };    

    //generate Report for the specified datasources
    function selectDocumentsForReportGeneration(ReportTemplateVersionID, AccountID, FolderVersionID) {
        $("#generateReportbtn").show();
        //alert(ReportTemplateVersionID + '_' + AccountID + '_' + FolderVersionID);
        //set column list for grid
        var colArray = ['AccountID', 'Account Name', 'Document Type', 'FolderID', 'Folder Name', 'Form Name', 'FolderVersionID', 'Version Number', 'Effective Date', 'FormInstanceID', 'Data Source Name', 'IsSelected', 'Action'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'AccountID', index: 'AccountID', hidden: true, search: false, });
        colModel.push({ name: 'AccountName', index: 'AccountName', search: true, });
        colModel.push({ name: 'DocumentType', index: 'DocumentType', search: true, });
        colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true, search: false, });
        colModel.push({ name: 'FolderName', index: 'FolderName', search: true, });
        colModel.push({ name: 'FormName', index: 'FormName', search: true, });
        colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', search: false, hidden: true });
        colModel.push({ name: 'FolderVersionNumber', index: 'FolderVersionNumber', search: true, });
        colModel.push({ name: 'FolderVersionEffectiveDate', index: 'FolderVersionEffectiveDate', search: true, });
        colModel.push({ name: 'FormInstanceID', index: 'FormInstanceID', hidden:true, search: true, });
        colModel.push({ name: 'DataSourceName', index: 'DataSourceName', hidden: true, search: false, }); 
        colModel.push({ name: 'IsSelected', index: 'IsSelected', hidden: true, search: false, });

        colModel.push({ name: 'Action', index: 'Action', width: '100px', align: 'left', sortable: false, search: false, editable: false, formatter: selectDocumentFormatter });

        //clean up the grid first - only table element remains after this
        $(elementIDs.relatedDocumentGridJQ).jqGrid('GridUnload');

        var url = URLs.documentsForReport.replace(/\{ReportTemplateVersionID\}/g, ReportTemplateVersionID).replace(/\{AccountID\}/g, AccountID).replace(/\{FolderVersionID\}/g, FolderVersionID);
        
        //adding the pager element
        $(elementIDs.relatedDocumentGridJQ).parent().append("<div id='p" + elementIDs.relatedDocumentGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.relatedDocumentGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Select Documents',
            height: '230',
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.relatedDocumentGrid,
            sortname: 'ReportId',
            altclass: 'alternate',
            gridComplete: function () {
                $('#generateReportbtn').show();
                $('#documentsInFolder').show();
            },

            resizeStop: function (width, index) {
                autoResizing(elementIDs.relatedDocumentGridJQ);
            }
        });

        var pagerElement = '#p' + elementIDs.relatedDocumentGrid;
        //remove default buttons
        $(elementIDs.relatedDocumentGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();

        // add filter toolbar to the top
        $(elementIDs.relatedDocumentGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });
    }

    //generate Report for the specified datasources
    function generateReports() {
        //set column list for grid
        var colArray = ['Report Id', 'Report Name', 'Report Location', 'ReportTemplateVersionID', 'Version No'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'ReportId', index: 'ReportId', hidden: true, search: false });
        colModel.push({ name: 'ReportName', index: 'ReportName', width:220, search: true });
        colModel.push({ name: 'ReportTemplateLocation', index: 'ReportTemplateLocation', hidden: true, search: true }); 
        colModel.push({ name: 'ReportTemplateVersionID', index: 'ReportTemplateVersionID', hidden: true, search: true });
        colModel.push({ name: 'VersionNumber', index: 'VersionNumber', search: true });
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
            caption: 'Reports',
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
                $('#accountFolderOpt').show();
                selectedReportName = data.ReportName;
                selectedReportTemplateVersionID = data.ReportTemplateVersionID;
                selectedReportTemplateLocation = data.ReportTemplateLocation;
                $('#accountDropDown').empty();
                $(elementIDs.relatedDocumentGridJQ).jqGrid('GridUnload');
                $("#generateReportbtn").hide();
                getAccountsFolders(selectedReportTemplateVersionID, 0);
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
    }    

    //load form design list in grid
    function loadFormDesignGrid() {        
        if ($('#downloadStatus').val() == 'DoesNotExist')
            messageDialog.show('DataSource for the selected Document Version does not Exist.');
        if ($('#downloadStatus').val() == 'Failure')
            messageDialog.show('There was a Failure downloading the File.');
        $('#downloadStatus').val('');

        //set column list for grid
        var colArray = ['', '', 'Document Design'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true, search: false, });
        colModel.push({ name: 'FormDesignId', index: 'FormDesignId', key: true, hidden: true });
        colModel.push({ name: 'DisplayText', index: 'DisplayText', editable: false, width: '305px', searchoptions: customSearchOptions });

        //clean up the grid first - only table element remains after this
        $(elementIDs.formDesignGridJQ).jqGrid('GridUnload');

        var url = URLs.formDesignList;
        //adding the pager element
        $(elementIDs.formDesignGridJQ).parent().append("<div id='p" + elementIDs.formDesignGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.formDesignGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Document Design List',
            height: '350',
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.formDesignGrid,
            sortname: 'FormDesignId',
            altclass: 'alternate',
            //load associated form design version grid on selecting a row
            onSelectRow: function (id) {
                var row = $(this).getRowData(id);
                loadFormDesignVersionGrid(row.FormDesignId, row.DisplayText);
            },
            //on adding a new form design, reload the grid and set the row to selected
            gridComplete: function () {
                var rows = $(this).getDataIDs();
                var rowId;
                if (rows.length > 0) {
                    rowId = rows[0];
                }
                var newName = $(elementIDs.formDesignDialog + ' input').val();
                var newId;
                for (index = 0; index < rows.length; index++) {
                    row = $(this).getRowData(rows[index]);
                    if (newName === row.DisplayText) {
                        newId = row.FormDesignId;
                        break;
                    }
                }
                if (newId !== undefined) {
                    $(this).jqGrid('setSelection', newId);
                }
                else {
                    $(this).jqGrid('setSelection', rowId);
                }

            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.formDesignGridJQ);
            }
        });

        var pagerElement = '#p' + elementIDs.formDesignGrid;
        //remove default buttons
        $(elementIDs.formDesignGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();

        // add filter toolbar to the top
        $(elementIDs.formDesignGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });
    }

    //load form design verion in grid
    function loadFormDesignVersionGrid(formDesignId, formDesignName) {
        //set column list
        var colArray = ['TenantId', 'FormDesignId', 'Effective Date', 'Version', 'StatusId', 'Status', 'Download'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'TenantId', index: 'TenantId', hidden: true, search: false, });
        colModel.push({ name: 'FormDesignVersionId', index: 'FormDesignVersionId', key: true, hidden: true, width: 100, align: 'left', search: false, });
        colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', width: '315px', align: 'left', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions });
        colModel.push({ name: 'Version', index: 'Version', width: '315px', align: 'left', editable: false });
        colModel.push({ name: 'StatusId', index: 'StatusId', hidden: true, editable: false });
        colModel.push({ name: 'StatusText', index: 'StatusText', width: '315px', align: 'left', editable: false });
        colModel.push({ name: 'Action', index: 'Action', width: '100px', align: 'left', sortable: false, search: false, editable: false, formatter: actionFormatterfordesign });
            

        //get URL for grid
        var formDesignVersionListUrl = URLs.formDesignVersionList.replace(/\{formDesignId\}/g, formDesignId);

        //unload previous grid values
        $(elementIDs.formDesignVersionGridJQ).jqGrid('GridUnload');
        //add pager element
        $(elementIDs.formDesignVersionGridJQ).parent().append("<div id='p" + elementIDs.formDesignVersionGrid + "'></div>");
        //load grid - refer jqGrid documentation for details
        $(elementIDs.formDesignVersionGridJQ).jqGrid({
            url: formDesignVersionListUrl,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Document Design Version List - ' + formDesignName,
            forceFit: true,
            height: '350',
            autowidth: true,
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
            ExpandColumn: 'Label',
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.formDesignVersionGrid,
            sortname: 'FormDesignVersionId',
            altclass: 'alternate',
            //register events to load the form design version in a new tab page
            //this is on click of the span which has the image in the last column

            resizeStop: function (width, index) {
                autoResizing(elementIDs.formDesignGridJQ);
            }
        });
        var pagerElement = '#p' + elementIDs.formDesignVersionGrid;
        //remove default buttons
        $(elementIDs.formDesignVersionGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();

        //add filter toolbar at the top of grid
        $(elementIDs.formDesignVersionGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });

    }

    function loadReportDesignTemplategrid() {
        //alert($('#statusMessage').html()); 
        if ($('#statusMessage').val() == 'Success')
            messageDialog.show('File Uploaded Successfully');
        else if ($('#statusMessage').val() == 'Failure') {
            messageDialog.show('There was a Failure uploading File.');
        }
        
        $('#statusMessage').html('');

        var lastSel;
        //set column list for grid
        var colArray = ['Document Design Id', 'Document Design', 'Document Version', 'Select', 'Data Source Name', 'DSourceName', 'IsSelected', 'DVersion', 'Action'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'DocumentDesignId', index: 'DocumentDesignId', width: '250px', hidden:true, search: true, align: 'left' });
        colModel.push({ name: 'DocumentDesignName', index: 'DocumentDesignName', editable: true, width: '250px', search: true, align: 'left' });
        //colModel.push({ name: 'DocumentVersionId', index: 'DocumentVersionId', width: '220px', hidden:true, search: true, align: 'center' });
        colModel.push({ name: 'DocumentVersions', index: 'DocumentVersions', width: '250px', align: 'left', editable: true, edittype: "select", formatter: actionFormatterfortemplate3 });    //  , unformat: actionUnFormatterfortemplate3
        colModel.push({ name: 'Action', index: 'Action', editable: true, edittype: "checkbox", width: '250px', searchoptions: customSearchOptions, align: 'center' , formatter: actionFormatterfortemplate1 });  //
        colModel.push({ name: 'DataSourceName', index: 'DataSourceName', editable: true, edittype: "text", width: '250px', align: 'left' , formatter: actionFormatterfortemplate2 });

        colModel.push({ name: 'DSourceName', index: 'DSourceName', width: '250px', hidden: true, search: true, align: 'left' });
        colModel.push({ name: 'IsSelected', index: 'IsSelected', width: '250px', hidden: true, search: true, align: 'left' });
        colModel.push({ name: 'DVersion', index: 'DVersion', width: '250px', hidden: true, search: true, align: 'left' });
        colModel.push({ name: 'Action', index: 'Action', width: '100px', align: 'left', sortable: false, search: false, editable: false, formatter: actionFormatterForFormDesignVersionDownload });



        //clean up the grid first - only table element remains after this
        $(elementIDs.formTemplateDocumentMappingGridJQ).jqGrid('GridUnload');

        var url = URLs.formTemplateDocumentMapping;
        //adding the pager element
        $(elementIDs.formTemplateDocumentMappingGridJQ).parent().append("<div id='p" + elementIDs.formTemplateDocumentMappingGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.formTemplateDocumentMappingGridJQ).jqGrid({
            url: url,
            name:'templatejqGrid',
            datatype: 'json',
            cache: false,
            colNames: colArray,
            cellEdit:false,
            colModel: colModel,
            caption: 'Select Document Design',
            height: '160',
            rowNum: 10000,
            ignoreCase: true,
            loadonce: false,
            width: 750,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.formTemplateDocumentMappingGrid,
            sortname: 'FormDesignId',
            altclass: 'alternate',
            onSelectRow: function (id) {
                if (lastSel != undefined || lastSel != null) {
                    $(elementIDs.formTemplateDocumentMappingGridJQ).jqGrid('saveRow', lastSel);
                }
                lastSel = id;
               
            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.formTemplateDocumentMappingGridJQ);
            }
        });

        var pagerElement = '#p' + elementIDs.formTemplateDocumentMappingGrid;
        //remove default buttons
        $(elementIDs.formTemplateDocumentMappingGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();

        // add filter toolbar to the top
        $(elementIDs.formTemplateDocumentMappingGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: false,
            defaultSearch: "cn"
        });
    }

    init();

    



    function editTmplVrsnFormatter(cellValue, options, rowObject) {
        //reportConfigurationGrid.prototype.testmethod();
        var idRptVrsn = "rptvrsn" + rowObject.ReportTemplateVersionID;
        var id = $(this).attr('id');
        var rows = $('#rptTmplVersions').jqGrid('getRowData');
        if (rowObject.Status == 'Finalized') {
            return "<span id = '" + idRptVrsn + "' class='ui-icon ui-icon-document view' title = 'Non-Editable Template Version' onclick='reportConfigurationGrid.prototype.DownLoadDocument(" + rowObject.ReportTemplateVersionID + ")' style = 'margin-left:55px;cursor: pointer'/>";
        }
        else {   //   rowObject.ReportTemplateVersionID
            return "<span id = '" + idRptVrsn + "' class='ui-icon ui-icon-pencil edit' title = 'Editable Template Version' style = 'margin-left:55px;cursor: pointer'/>";
            //return "<span id = '" + idDwn + "' class='ui-icon ui-icon-pencil edit' onclick='editTemplateVersion(\"" + 1 + "\",\"" + rowObject.ReportTemplateVersionID + "\");' title = 'Editable Template Version' style = 'cursor: pointer'/>";

        } // editTemplateVersion('"+rowObject.TenantID+"', '"+rowObject.ReportTemplateVersionID+"');  '$(function(){editghgh();});'
    }  

    function selectDocumentFormatter(cellValue, options, rowObject) {                 // checkbox 
        var id = "sel_" + indexdoc++;
        return "<input onchange='setValue(\"" + elementIDs.relatedDocumentGridJQ + "\",\"" + indexdoc + "\",\"" + id + "\",\"IsSelected\",\"checked\")' id = '" + id + "' type='checkbox' title = 'Select' />";   //  style = 'cursor: pointer'
    }

    function actionFormatterfordesign(cellValue, options, rowObject) {
        return "<span id = 'fvds" + rowObject.FormDesignVersionId + "' class='ui-icon ui-icon-circle-arrow-s' onclick='downloadDocumentDesignXML(" + rowObject.TenantID + ", " + rowObject.FormDesignVersionId + ");' title = 'Download Xml' style = 'cursor: pointer; text-align:center;'/>";
    }

    function formatReportgen(cellValue, options, rowObject) {
        return "<span id = 'fvdsgen" + rowObject.ReportId + "' class='ui-icon-document view' onclick='generateReport(" + rowObject.ReportId + ", " + rowObject.ReportName + ")' title = 'View Report' style = 'cursor: pointer; text-align:center;'/>";
    }

    function actionFormatterForFormDesignVersionDownload(cellValue, options, rowObject) {
        if (typeof rowObject.DocumentVersions != 'undefined' && rowObject.DocumentVersions != null) {
            var idDwn = "#drp_" + indexdocvrsn++;
            if ($(idDwn).val())
                return "<span id = 'desDwn" + rowObject.DocumentDesignId + "' class='ui-icon ui-icon-circle-arrow-s' onclick='downloadDocumentDesignXML(" + rowObject.TenantID + ", '" + idDwn + "')' title = 'Download Xml' style = 'cursor: pointer; text-align:center; width:100px;'/>";
        }
    }

    function actionFormatterfortemplate1(cellValue, options, rowObject) {                 // checkbox 
        var id1 = "chk_" + indexchk++;
        return "<input onchange='setValue(\"" + elementIDs.formTemplateDocumentMappingGridJQ + "\",\"" + id1 + "\",\"IsSelected\",\"checked\")' id = '" + id1 + "' type='checkbox' title = 'Select' />";   //  style = 'cursor: pointer'
    }

    function actionFormatterfortemplate2(cellValue, options, rowObject) {                 //  textbox
        var id2 = "txt_" + indextxt++;
        return "<input onchange='setValue(\"" + elementIDs.formTemplateDocumentMappingGridJQ + "\",\"" + id2 + "\",\"DSourceName\",\"value\")' id = '" + id2 + "' type='text' title = 'Enter' />";        //  style = 'cursor: pointer'
    }

    function actionFormatterfortemplate3(cellValue, options, rowObject) {                 //   dropdown 
        if (typeof rowObject.DocumentVersions != 'undefined' && rowObject.DocumentVersions != null) {
                var id3 = "drp_" + indexdrp++;
                var result = "<select onchange='setValue(\"" + elementIDs.formTemplateDocumentMappingGridJQ + "\",\"" + id3 + "\",\"DVersion\",\"value\")' id = '" + id3 + "' class='form-control hastooltip' >";
                result = result + '<option value="">Select</option>'
                $.each(rowObject.DocumentVersions, function (i, obj) {
                    result += "<option value='" + obj.VersionId + "-" + obj.VersionNo + "' >" + obj.VersionNo + "</option>";
                });
                result += "</select>";
                return result;
        }
    }

    return {
        viewReportTemplates: function (reportid) {
            viewReportTemplates(reportid);
        },
        viewReportTemplateVersion:function(){
            viewReportTemplateVersion();
        },
        selectedReportID: function () {
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
            else
                messageDialog.show(ReportTemplate.reportTmplAdded);

        }
        else {
            messageDialog.show(ReportTemplate.reportTmplErrorMsg);
        }
        //reload the Report Template grid 
        reportTemplate.viewReportTemplates(0); 

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
                return compareStrings(ct.ReportName, newName, true);
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
                    ReportTemplateID: (reportTemplateDialogState === 'add' ? -1 : rowId.ReportId),
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
        reportTemplateVrsnAdd: '/DocumentCollateral/AddReportTemplateVersion'
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
        reportTmplVrsnDialog: "#reportTmplVrsnDialog"
    };

    //ajax success callback - for add
    function reportTemplateVrsnSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {
            messageDialog.show(ReportTemplateVersion.reportTmplVrsnAdded);
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
            height: 250,
            width: 450,
            modal: true
        });
        //register event for Add/Edit button click on dialog
        $(elementIDs.reportTmplVrsnDialog + ' button').on('click', function () {            
            
            var effDate = $(elementIDs.reportTmplVrsnDialog + ' input').val();

            if(Date.parse(effDate)){
                //save the new report Template
                var rowId = $(elementIDs.reportTemplateVersionGridJQ).getGridParam('selrow');
                var reportTemplateVrsnAdd = {
                    TenantID: 1,
                    ReportTemplateID: reportTemplateID,
                    EffectiveDate: effDate
                };
                var url = URLs.reportTemplateVrsnAdd;
                //ajax call to add/update
                var promise = ajaxWrapper.postJSON(url, reportTemplateVrsnAdd);
                //register ajax success callback
                promise.done(reportTemplateVrsnSuccess);
            }
            else{                       //register ajax failure callback                
                messageDialog.show("Please enter a Valid Effective Date.");
            }
        });
    }
    //initialize the dialog after this js is loaded
    init();

    //these are the properties that can be called by using reportTmplDialog.<Property>
    //eg. reportTmplDialog.show('name','add');
    return {
        show: function (ReportID) {
            reportTemplateID = ReportID;
            $(elementIDs.reportTmplVrsnDialog + ' input').datepicker();

            $(elementIDs.reportTmplVrsnDialog + ' input').each(function () {
                
                $(elementIDs.reportTmplVrsnDialog + ' div').removeClass('has-error');
                $(elementIDs.reportTmplVrsnDialog).dialog('option', 'title', ReportTemplate.addReportTemplate);
                $(elementIDs.reportTmplVrsnDialog + ' .help-block').text(ReportTemplate.reportTmplVrsnEffDateRequiredValidationMsg);

                //open the dialog - uses jqueryui dialog
                $(elementIDs.reportTmplVrsnDialog).dialog("open");
                
            });
        }
    }
}(); //invoked soon after loading

var reportTmplVrsnAddDialog = function () {

    var reportTemplateVersiondiv = '#reportTmplVrsnDialog';
    //init dialog on load of page
    function init() {

        //register dialog for grid row add/edit
        $(reportTemplateVersiondiv).dialog({
            autoOpen: false,
            height: 250,
            width: 450,
            modal: true
        });
        
        
    }
    //initialize the dialog after this js are loaded
    init();

    //these are the properties that can be called by using reportTmplDialog.<Property>
    //eg. reportTmplDialog.show('name','add');
    return {
        show: function (ReportName) {
            $(reportTemplateVersiondiv).dialog('option', 'title', ReportName);

            //open the dialog - uses jqueryui dialog
            $(reportTemplateVersiondiv).dialog("open");

            
            

        }
    }
}(); //invoked soon after loading

var reportVersionConfig = function (ReportTemplateVersionID) {
    function reportConfigurationGrid() {
        this.ReportTemplateVersionID = ReportTemplateVersionID;//$('#ReportTmplVersionID').val();
        this.tenantId = 1;
        this.uiElementDetail = null;
        this.statustext = status;

        alert(this.ReportTemplateVersionID);
        //alert(reportTemplate.selectedReportTemplateVersionID());
        $('#uploadTab').removeAttr('style');

        var isFinalized = false;
        if (Finalized.length > 0) {
            $.each(Finalized, function (index, value) {
                if (value.FORMDESIGNVERSIONID == formDesignVersionId && value.ISFINALIZED == 1) {
                    isFinalized = true;
                }
            });
        }
        if (isFinalized == true)
            this.statustext = "Finalized";

        this.UIElementIDs = {
            //multiple instances for different tabs -generate id dynamically for each form design version id
            propertyGridContainerElement: 'fdvuielemdetail{formDesignVersionId}',
            propertyGridContainerElementContainer: 'fdvuielemdetail{formDesignVersionId}container'
        }
        this.URLs = {
            updateProperties: '/DocumentCollateral/UpdateReportProperties',
            reportPropertiesDetail: '/DocumentCollateral/GetProperties?tenantId=1&TemplateReportVersionID={TemplateReportVersionID}&uiElementId={uiElementId}',
            getTemplateNameUrl: '/DocumentCollateral/GetTemplateNameById?templateReportID={templateReportID}',
        }

        //generate dynamic grid element id
        this.gridElementId = '#reportMapping';
        this.gridElementIdNoHash = 'reportMapping';
        this.gridContainerElementId = '#' + this.UIElementIDs.propertyGridContainerElementContainer.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId);
        this.roleAccessPermissionDialog = undefined;
        //variable for dropDownItemsDialog - required only for Drop Downs
        this.dropDownItemsDialog = undefined;
    }

    //load property grid
    reportConfigurationGrid.prototype.loadPropertyGrid = function () {
        url = this.URLs.reportPropertiesDetail.replace(/\{TemplateReportVersionID\}/g, this.ReportTemplateVersionID);

        if (url !== undefined) {
            var promise = ajaxWrapper.getJSON(url);
            var currentInstance = this;
            promise.done(function (xhr) {
                currentInstance.uiElementDetail = xhr;
                //generate grid data
                var reportProperties = currentInstance.generateGridData(xhr);
                currentInstance.bindToPropertyGrid(reportProperties);
            });
            //register callback for ajax request failure
            promise.fail(this.showError);
        }
        else {
            //get the url to fetch data for the property grid - for the tab element 
            url = this.URLs.tabDetail.replace(/\{uiElementId\}/g, this.uiElement.UIElementID);
            //make the ajax call to get the data
            if (url !== undefined) {
                var promise = ajaxWrapper.getJSON(url);
                var currentInstance = this;
                //callback for ajax request success
                promise.done(function (xhr) {
                    currentInstance.uiElementDetail = xhr;
                    var tabUIElementProperties = currentInstance.getUIElementProperties(currentInstance.uiElement.ElementType);
                    //Adding custom rule fetched from DB to tabUIElementProperties.
                    tabUIElementProperties[1].Value = xhr.CustomRule;
                    currentInstance.customRule = xhr.CustomRule;
                    currentInstance.bindToPropertyGrid(tabUIElementProperties);
                });
                //register callback for ajax request failure
                promise.fail(this.showError);
            }
        }
    }

    reportConfigurationGrid.prototype.generateGridData = function (xhr) {
        //get properties that need to be displayed for the element based on type
        var uiElementProperties = this.getUIElementProperties(this.reportItem.ReportName);
        //populate the Value for each property of the element with the data received from the ajax call
        for (var index = 0; index < uiElementProperties.length; index++) {
            switch (uiElementProperties[index].IntProperty) {
                case 'ReportDescription':
                    uiElementProperties[index].Value = xhr.ReportDescription;
                    break;
                case 'Visible':

                    uiElementProperties[index].Value = xhr.Visible;
                    break;
                case 'Location':
                    uiElementProperties[index].Value = xhr.Location;
                    break;
                case 'RoleAccessPermission':
                    uiElementProperties[index].Value = xhr.RoleAccessPermission;
                    break;
                case 'Parameters':
                    uiElementProperties[index].Value = xhr.Parameters;
                    break;
                case 'ReportType':
                    uiElementProperties[index].Value = xhr.ReportType;
                    break;
                case 'HelpText':
                    uiElementProperties[index].Value = xhr.HelpText;
                    break;
                case 'IsRelease':
                    uiElementProperties[index].Value = xhr.IsRelease;
                    break;
                case 'Template':
                    uiElementProperties[index].Value = xhr.Template;
                    break;
            }
        }
        return uiElementProperties;
    }

    reportConfigurationGrid.prototype.getUIElementProperties = function (elementType) {
        var currentInstance = this;
        var elementProperties = [
                 { IntProperty: 'ReportDescription', Property: 'Report Description', Value: '' },
                 { IntProperty: 'Visible', Property: 'Visible', Value: '' },
                 { IntProperty: 'Location', Property: 'Location', Value: '' },
                 { IntProperty: 'RoleAccessPermission', Property: 'Role Access Permissions' },
                 { IntProperty: 'Parameters', Property: 'Parameters', Value: '' },
                 { IntProperty: 'ReportType', Property: 'Report Type', Value: '' },
                 { IntProperty: 'HelpText', Property: 'Help Text', Value: '' },
                 { IntProperty: 'IsRelease', Property: 'Is Release', Value: '' },
                 { IntProperty: 'Template', Property: 'Template', Value: '' },
        ];
        return elementProperties;
    }

    //bind data to the element property grid
    reportConfigurationGrid.prototype.bindToPropertyGrid = function (uiElementProperties) {
        var URLs = {
            //get Document Design List
            formDesignList: '/FormDesign/FormDesignList?tenantId=1',
            formDesignVersionList: '/FormDesign/FormDesignVersionList?tenantId=1'
        }
        //unload previous grid values
        $('#reportMapping').jqGrid('GridUnload');  // this should be mapping Grid
        //set column list
        var colArray = ['IntProperty', 'Property', 'Value'];
        //set column  models
        var colModel = [];
        colModel.push({ name: 'IntProperty', index: 'IntProperty', key: true, hidden: true, search: false });
        colModel.push({ name: 'Property', index: 'Property', align: 'left', editable: false });
        colModel.push({ name: 'Value', index: 'Value', align: 'left', editable: false, formatter: this.formatColumn, unformat: this.unFormatColumn });

        $(this.gridElementId).parent().append("<div id='p" + this.gridElementIdNoHash + "'></div>");

        var currentInstance = this;

        $(this.gridElementId).jqGrid({
            datatype: 'local',
            colNames: colArray,
            colModel: colModel,
            autowidth: true,
            caption: currentInstance.reportItem.ReportName,
            pager: '#p' + currentInstance.gridElementIdNoHash,
            hidegrid: false,
            height: '374',
            altRows: true,
            altclass: 'alternate',
            //register event handler for row insert
            afterInsertRow: function (rowId, rowData, rowElem) {
            },
            gridComplete: function () {

            }
        });

        //insert rows in the grid
        for (var index = 0; index < uiElementProperties.length; index++) {
            $('#reportMapping').jqGrid('addRowData', uiElementProperties[index].IntProperty, uiElementProperties[index]);
        }

        var pagerElement = '#p' + currentInstance.gridElementIdNoHash;
        //remove default buttons
        $(currentInstance.gridElementId).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();
        $(pagerElement + '_left').find("tr").children().last().find('div').css('padding-top', '5px');

        $(currentInstance.gridElementId).jqGrid('navButtonAdd', pagerElement,
        {
            caption: 'Save', id: 'templatePropSaveButton',
            onClickButton: function () {
                //var templatePropertiesList = $(this).getRowData();
                var grid = this;
                var data = currentInstance.readGridData();

                currentInstance.postFormInstanceData(data, currentInstance);

            }
        });

        $(this.gridElementId).find('#Template').find('.ui-icon-document').click(function (e) {
            var url1 = currentInstance.URLs.getTemplateNameUrl.replace(/\{templateReportID\}/g, currentInstance.reportItem.ReportId);
            //ajax POST of element properties
            var promise = ajaxWrapper.postJSON(url1);
            promise.done(function (xhr) {
                if (xhr != "") {
                    var currentInstance = this;
                    //var reportID = currentInstance.reportItem.ReportId;
                    var url1 = '/DocumentCollateral/DownloadDocument';
                    var stringData = 'fileName=' + 'App_Data\\' + xhr;
                    $.downloadNew(url1, stringData, 'post');
                }
                else {
                    messageDialog.show(winwardReportMessages.templaleNotAvailableMsg);
                    return;
                }
            });
            //register ajax failure callback
            promise.fail(currentInstance.showError);
        });
        //register event handler to display fields dialog when edit icon of fields property is clicked
        $(this.gridElementId).find('#Parameters').find('.ui-icon-pencil').click(function () {
            currentInstance.parametersDialog = new parametersDialog(currentInstance.reportItem, currentInstance.formDesignVersionId, currentInstance.statustext);
            currentInstance.parametersDialog.show();
        });

        $(this.gridElementId).find('#RoleAccessPermission').find('.ui-icon-pencil').click(function () {
            currentInstance.roleAccessPermissionDialog = new roleAccessPermissionDialog(currentInstance.reportItem, currentInstance.ReportTemplateVersionID, currentInstance.formDesignVersionId);
            currentInstance.roleAccessPermissionDialog.show();
        });
    }

    reportConfigurationGrid.prototype.allowCustomLayoutForElement = function (currentInstance, data) {
        var isSubSectionOrRepeaterInsideSection = false;
        var childElements = currentInstance.elementGridData.filter(function (elem) {
            return elem.parent == currentInstance.reportItem.ReportId;
        });
        for (i = 0; i < childElements.length; i++) {
            if (childElements[i].ElementType == "Repeater" || childElements[i].ElementType == "Section") {
                isSubSectionOrRepeaterInsideSection = true;
            }
        }
        if (!isSubSectionOrRepeaterInsideSection) {
            currentInstance.postFormInstanceData(data, currentInstance);
        }
        else {
            messageDialog.show(DocumentDesign.restrictExistingLayoutToChangeMsg);
            return false;
        }
    }

    reportConfigurationGrid.prototype.postFormInstanceData = function (data, currentInstance) {
        //ajax POST of element properties
        var promise = ajaxWrapper.postJSON(currentInstance.URLs.updateProperties, data);
        promise.done(function (xhr) {
            if (xhr.Result === ServiceResult.SUCCESS) {
                if (xhr.Items.length > 0 && xhr.Items[0].Messages[0] != undefined && xhr.Items[0].Messages[0] != null) {
                    messageDialog.show(DocumentDesign.saveMsg);
                }
            } else {
                if (xhr.Items.length > 0 && xhr.Items[0].Messages[0] != undefined && xhr.Items[0].Messages[0] != null) {
                    messageDialog.show(xhr.Items[0].Messages[0]);
                    if (xhr.Items[0].Messages[1] != undefined && xhr.Items[0].Messages[1] != null) {
                        currentInstance.loadPropertyGrid();
                    }
                }
            }
        });
        //register ajax failure callback
        promise.fail(currentInstance.showError);

    }

    reportConfigurationGrid.prototype.hasValidationError = function (grid) {
        //first remove all the validations on the grid
        $(grid).find('.input-validation-error').each(function (val, idx) {
            $(this).removeClass("input-validation-error");
            $(this).attr("data-original-title", "");
        });
        //trigger the validations
        //as all validations are applied on focusout event of control, we need to trigger the focusout event explicitly 
        $(grid).find('input').trigger('focusout');
        $(grid).find('textarea').trigger('focusout');
        $(grid).find('select').trigger('focusout');
        return $(grid).find('.input-validation-error').length > 0;
    }

    reportConfigurationGrid.prototype.validateMaxLength = function (rowId, control, length) {
        var controlLength = $(control).val().length;
        if (controlLength > length) {
            $(control).addClass("input-validation-error");
            $(control).attr("data-original-title", DocumentDesign.maxLengthMsg + length + " chars.");
            $(control).attr("data-toggle", "tooltip");
            $(control).tooltip({
                placement: "left",
                trigger: "hover",
            });
        }
    }


    //read the data in the property grid
    reportConfigurationGrid.prototype.readGridData = function () {
        var currentInst = this;
        var uiElementProperties = this.getUIElementProperties(this.reportItem.ReportId);
        var updateElement = {};
        updateElement.ElementType = this.reportItem.ReportName;
        //updateElement.UIElementID = this.uiElement.UIElementID;
        //updateElement.FormDesignID = this.formDesignId;
        //updateElement.FormDesignVersionID = this.formDesignVersionId;
        //updateElement.ParentUIElementID = this.uiElement.ParentUIElementID;
        updateElement.ReportDesignId = this.reportItem.ReportId;
        //updateElement.ReportDesignId = this.uiElementDetail.ReportDesignId;
        //iterate through each element and set the values in the updateElement object
        for (var index = 0; index < uiElementProperties.length; index++) {
            updateElement[uiElementProperties[index].IntProperty] = $(this.gridElementId).getRowData(uiElementProperties[index].IntProperty).Value;
        }
        if (currentInst.parametersDialog != undefined)
            updateElement.Parameters = currentInst.parametersDialog.parameter;

        updateElement.ReportVersionID = this.reportItem.ReportTemplateVersionID;
        return updateElement;
    }

    reportConfigurationGrid.prototype.formatColumn = function (cellValue, options, rowObject) {
        var result;
        switch (rowObject.IntProperty) {
            case 'Location':
                result = '<select style="width:100%" class="form-control"><option value="InFolder">In Folder</option><option value="InMenu">In Menu</option></select>';
                break;
            case 'ReportType':
                result = '<select style="width:100%" class="form-control"><option value="Folder">Folder</option><option value="Account">Account</option></select>';
                break;
            case 'Visible':
            case 'IsLibraryRegex':
            case 'IsRelease':
                if (cellValue == true) {
                    result = '<input type="checkbox" checked/>';
                }
                else {
                    result = '<input type="checkbox"/>';
                }
                break;
            case 'RoleAccessPermission':
            case 'Parameters':
                if (cellValue === null || cellValue === undefined) {
                    cellValue = '';
                }
                result = '<div style="float:right;width:55%"><span class="ui-icon ui-icon-pencil" title = "Manage ' + rowObject.Property + '" style = "cursor: pointer;"></span></div>';
                break;
            case 'ReportDescription':
            case 'HelpText':
                result = '<textarea style="width:100%;" >' + cellValue + '</textarea>';
                break;
            case 'Template':
                if (cellValue === null || cellValue === undefined) {
                    cellValue = '';
                }
                return '<div style="float:right;width:55%"><span class="ui-icon ui-icon-document view" title = "Download ' + rowObject.Property + '" style = "cursor: pointer;"></span></div>';
        }
        return result;
    }

    //unformat the grid column based on element property
    //used in unformat in colModel for the Value column : bindToPropertyGrid method
    reportConfigurationGrid.prototype.unFormatColumn = function (cellValue, options) {
        var result;
        switch (options.rowId) {
            case 'ReportDescription':
            case 'HelpText':
                result = $(this).find('#' + options.rowId).find('textarea').val();
                break;
            case 'Location':
            case 'ReportType':
                //extract value from the drop down
                result = $(this).find('#' + options.rowId).find('select').val();
                break;
            case 'DataSourceName':
                result = $(this).find('#' + options.rowId).find('input').val();
                break;
            case 'Visible':
            case 'IsRelease':
                result = $(this).find('#' + options.rowId).find('input').prop('checked');
                break;
            default:
                result = '';
                break;
        }
        return result;
    }

    //handler for ajax errors
    reportConfigurationGrid.prototype.showError = function (xhr) {
        if (xhr.status == 999)
            window.location.href = '/Account/LogOn';
        else
            alert(JSON.stringify(xhr));
    }

    reportConfigurationGrid.prototype.destroy = function () {
        this.dropDownItemsDialog = null;
        this.rulesDialog = null;
        this.dataSourceDialog = null;
        this.customRulesDialog = null;
        this.customRule = null;
        this.sectionListDialog = null;
        $(this.gridElementId).jqGrid('GridUnload');
        this.duplicationCheck = null;
        this.duplicationCheckDialog = null
    }

    reportConfigurationGrid.prototype.showGrid = function () {
        $(this.gridContainerElementId).show();
    }

    reportConfigurationGrid.prototype.hideGrid = function () {
        $(this.gridContainerElementId).hide();
    }

    return {
        reportConfigurationGrid: function () {
            reportConfigurationGrid();
        }
    };
}();

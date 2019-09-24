
var indexdoc = 0;
var indexdocvrsn = 0;
var indexdocedit = 0;
var disableRowID = -1;
var RptVrsnInstances = {};
var templateLbl;
var dropDownDocumentVersions;
var dropDownDocumentDesignName;
var dataSourceNameColumn;

function reportConfigurationGrid(reportVersion, ReportId, ReportTemplateVersionID, Status, tabNamePrefix, tabIndex, tabCount, tabs, ReportName) {
    this.ReportId = ReportId;
    this.ReportTemplateVersionID = ReportTemplateVersionID;
    this.tenantId = 1;
    this.uiElementDetail = null;
    this.statustext = Status;
    this.tabNamePrefix = tabNamePrefix;
    this.tabs = tabs;
    this.tabCount = tabCount;
    this.tabIndex = tabIndex;
    this.reportName = ReportName;
    this.VersionNo = reportVersion.VersionNumber;
    this.reportUploadMappedDoc = '#reportUploadMappedDoc';
    this.tempDocMapdiv = '#tempDocMapdiv';
    this.TemplateName = (reportVersion.TemplateName == null || reportVersion.TemplateName == '') ? undefined : reportVersion.TemplateName;
    this.uploadTemplateformID = '#uploadRptTmplt-' + ReportTemplateVersionID
    this.parameterElementID = '#parameters-' + ReportTemplateVersionID
    this.Location = reportVersion.TemplateLocation;
    this.fileInputElement = '#uploadTemplateFile-' + ReportTemplateVersionID;
    this.fileInputLabelElement = '#uploadTemplateFileLabel-' + ReportTemplateVersionID;
    this.fileInputChangeElement = '#uploadTemplateFileChange-' + ReportTemplateVersionID;
    this.fileInputChangeLabelElement = '#uploadTemplateFileChangeLabel-' + ReportTemplateVersionID;
    this.cancelTemplateFileChange = '#cancelTemplateFileChange-' + ReportTemplateVersionID;
    templateLbl = '#uploadTemplateFileChangeLabel-' + ReportTemplateVersionID;
    this.SaveButtonID = '#saveTemplateButton-' + ReportTemplateVersionID;
    this.canbeFinalized = reportVersion.canbeFinalized;
    RptVrsnInstances["RptVrsn_" + ReportTemplateVersionID] = this;

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

    this.elementIDs = {
        uploadDocMapForReportTableIDJQ: '#rptDocMap-' + this.ReportTemplateVersionID,
        uploadDocMapForReportTableID: 'rptDocMap-' + this.ReportTemplateVersionID
    };


    this.UIElementIDs = {
        //multiple instances for different tabs -generate id dynamically for each form design version id
        propertyGridContainerElement: 'fdvuielemdetail{formDesignVersionId}',
        propertyGridContainerElementContainer: 'fdvuielemdetail{formDesignVersionId}container'
    }
    this.URLs = {
        updateProperties: '/DocumentCollateral/UpdateReportProperties',
        reportPropertiesDetail: '/DocumentCollateral/GetProperties?tenantId=1&TemplateReportVersionID={TemplateReportVersionID}',
        // Update Report Template Version
        SaveReportTemplateVersionUrl: '/DocumentCollateral/UpdateReportTemplateVersion?ReportVersionID={ReportVersionID}' +
                                      '&reportTemplateName={reportTemplateName}&VersionNumber={VersionNumber}' +
                                      '&templateDocMappings={templateDocMappings}&templateProperties={templateProperties}&Parameters={Parameters}',
        //get Template Document Mapping
        formTemplateDocumentMapping: '/DocumentCollateral/GetDocumentDesignVersion?ReportTemplateVersionID={ReportTemplateVersionID}',
        getTemplateNameUrl: '/DocumentCollateral/GetTemplateName?TemplateReportVersionID={templateReportVersionID}',
    }

    //generate dynamic grid element id
    this.gridElementId = '#reportProperties-' + this.ReportTemplateVersionID;
    this.gridElementIdNoHash = 'reportProperties-' + this.ReportTemplateVersionID;
    this.gridDocMapElementId = '#reportDocMappings-' + this.ReportTemplateVersionID;
    this.gridDocMapElementIdNoHash = 'reportDocMappings-' + this.ReportTemplateVersionID;
    this.gridContainerElementId = '#' + this.UIElementIDs.propertyGridContainerElementContainer.replace(/\{formDesignVersionId\}/g, this.formDesignVersionId);
    this.roleAccessPermissionDialog = undefined;
    //variable for dropDownItemsDialog - required only for Drop Downs
    this.dropDownItemsDialog = undefined;

}

// loads the main page
reportConfigurationGrid.prototype.loadTabPage = function () {
    var tabName = this.reportName + '-' + this.VersionNo;
    var uploadedTemplateName = this.TemplateName;
    //var reportUploadMappedDocContent = $(this.reportUploadMappedDoc).html();
    var reportVersionID = this.ReportTemplateVersionID;
    var reportUploadMappedDocDivContent = $('<div>' + $(this.reportUploadMappedDoc).html().replace(/##ID/gi, reportVersionID) + '</div>');
    var disableInput = "";
    if (RptVrsnInstances["RptVrsn_" + reportVersionID].statustext == 'Finalized') {
        disableInput = "disabled='disabled'"
        reportUploadMappedDocDivContent.find(this.fileInputElement).attr('disabled', 'disabled');
        reportUploadMappedDocDivContent.find(this.fileInputChangeElement).attr('disabled', 'disabled');
        //$(this.gridElementId).find('#Parameters_' + reportVersionID).find('.ui-icon-pencil').unbind("click");
        //$("#reportPropertiesdetail1024container").find('.ui-icon-pencil').attr('disabled', 'disabled').addClass('ui-state-disabled');
    }
    /**** START: Hide or Show the File Upload element based on whether the Template has already been uploaded or it hasn't been.****/

    //In case template is uploaded and again collater template version is opened without refreshing page ,then get template from DB
    if (!uploadedTemplateName) {
        var url = this.URLs.getTemplateNameUrl.replace(/\{templateReportVersionID\}/g, this.ReportTemplateVersionID);
        var promise = ajaxWrapper.getJSONSync(url);
        promise.done(function (result) {
            uploadedTemplateName = result;
        });
    }

    if (uploadedTemplateName) {
        //var labelvale = reportUploadMappedDocDivContent.find(this.fileInputChangeLabelElement)[0].innerText;
        reportUploadMappedDocDivContent.find(this.fileInputChangeLabelElement)[0].innerText = uploadedTemplateName;
        reportUploadMappedDocDivContent.find(this.fileInputChangeLabelElement).css('display', 'normal');
        $(document).on('click', this.fileInputChangeElement, function () {
            var temp = $(this).attr('id');
            var reportVersionID = temp.split('-')[1];
            var currentInstance = RptVrsnInstances["RptVrsn_" + reportVersionID];
            var displll = $(currentInstance.fileInputChangeLabelElement).css('float');

            var propvalue = $(currentInstance.fileInputChangeLabelElement).attr('style');
            $(currentInstance.fileInputChangeLabelElement).removeClass('form-control');
            $(currentInstance.fileInputChangeLabelElement).css('display', 'none');

            $(currentInstance.fileInputChangeElement).css('display', 'none');
            $(currentInstance.fileInputLabelElement).css('display', 'normal');
            $(currentInstance.fileInputElement).css('display', 'normal');
            $(currentInstance.cancelTemplateFileChange).css('display', 'normal');
        });

        reportUploadMappedDocDivContent.find(this.fileInputChangeElement).css('display', 'normal');
        reportUploadMappedDocDivContent.find(this.fileInputLabelElement).css('display', 'none');
        reportUploadMappedDocDivContent.find(this.fileInputElement).css('display', 'none');

        $(document).on('click', this.cancelTemplateFileChange, function () {//.unbind()
            var temp = $(this).attr('id');
            var reportVersionID = temp.split('-')[1];
            var currentInstance = RptVrsnInstances["RptVrsn_" + reportVersionID];
            //var displll = $(currentInstance.fileInputChangeLabelElement).css('float');

            //var propvalue = $(currentInstance.fileInputChangeLabelElement).attr('style');
            $(currentInstance.fileInputChangeLabelElement).addClass('form-control');
            $(currentInstance.fileInputChangeLabelElement).css('display', 'normal');

            $(currentInstance.fileInputChangeElement).css('display', 'normal');
            $(currentInstance.fileInputLabelElement).css('display', 'none');
            $(currentInstance.fileInputElement).css('display', 'none');
            $(currentInstance.cancelTemplateFileChange).css('display', 'none');
        });
    }
    /**** END: Hide or Show the File Upload element based on whether the Template has already been uploaded or it hasn't been.****/
    reportUploadMappedDocDiv = "<div class='col-xs-8 col-md-8 col-lg-8 col-sm-8'><div class='left-sidebar'>" + reportUploadMappedDocDivContent.html() + "</div></div>";

    //$('#VersionNumber-' + reportVersionID).val(this.VersionNo);
    //$('#reportTemplateName-' + reportVersionID).val(this.reportName);

    //create link for the tab page
    var tabTemplate = "<li><a id= '" + this.reportName + "-" + this.VersionNo + "' href='#{href}'>#{label}</a> <span class='ui-icon ui-icon-close' data-tabid=" + this.ReportTemplateVersionID + " role='presentation'>Remove Tab</span></li>";
    //replace based on version being loaded for this report
    var li = $(tabTemplate.replace(/#\{href\}/g, '#' + this.tabNamePrefix).replace(/#\{label\}/g, this.reportName + '-' + this.VersionNo));
    //create div for the tab page content
    var tabContentTemplate = "<div class='row'>" + reportUploadMappedDocDiv + "<div class='col-xs-4 col-md-4 col-lg-4 col-sm-4'><div id='reportPropertiesdetail{ReportTemplateVersionID}container'><div class='grid-wrapper right-sidebar'><table " + disableInput + " id='reportProperties-{ReportTemplateVersionID}'></table></div><button " + disableInput + " id='saveTemplateButton-{ReportTemplateVersionID}' type='button'class='pull-right' style='margin-top:-7px;clear:both;'>Save</button></div></div></div>";
    var tabContent = tabContentTemplate.replace(/\{ReportTemplateVersionID\}/gi, this.ReportTemplateVersionID);
    //create tab page using jqueryui tab methods
    this.tabs.find('.ui-tabs-nav').append(li);
    this.tabs.append("<div id='" + this.tabNamePrefix + "'>" + tabContent + "</div><div style='clear:both'></div>");
    this.tabs.tabs('refresh');

    if (Finalized.length > 0) {
        this.tabs.tabs({ selected: (this.tabCount - 1) });
    }
    else
        this.tabs.tabs('option', 'active', this.tabCount);

    var currentInstance = this;
    //This function is used to nullify the instance of formdesignversion on its close event.
    $(currentInstance.tabs).find('span[data-tabid=' + currentInstance.ReportTemplateVersionID + ']').on("click", function () {
        currentInstance = null;
    });

    tabs = $('#viewReportTemplate').tabs();

    //load the hierarchical UI Element Grid
    this.loadPropertyGrid();
    this.loadDocMapGrid();
    this.PostFormData();

    $(templateLbl).click(function (e) {
        var currentInstance = RptVrsnInstances["RptVrsn_" + reportVersionID];
        //if (RptVrsnInstances["RptVrsn_" + currentInstance.ReportTemplateVersionID].statustext != 'Finalized') {
            var id = $(this).attr('id');
            reportVersionID = id.split('-')[1];
            currentInstance.DownLoadDocument(currentInstance.ReportTemplateVersionID);
        //}
    });


}

//load property grid
reportConfigurationGrid.prototype.loadPropertyGrid = function () {
    url = this.URLs.reportPropertiesDetail.replace(/\{TemplateReportVersionID\}/g, this.ReportTemplateVersionID);

    if (url !== undefined) {
        var promise = ajaxWrapper.getJSON(url);
        var currentInstance = this;
        var xhrdata;
        promise.done(function (xhr) {
            //alert('success');
            xhrdata = xhr;
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

//loads Document Grid on Report Template Version edit screen.
reportConfigurationGrid.prototype.loadDocMapGrid = function () {
    disableRowID = -1;
    var templateDocumentMappingData;
    var currentInstance = this;
    var url = currentInstance.URLs.formTemplateDocumentMapping.replace(/{ReportTemplateVersionID}/, currentInstance.ReportTemplateVersionID);
    var promise = ajaxWrapper.getJSONSync(url);
    var collateralDocDesignLst, collateralDesignVersionsLst;
    var documentVersions;
    var lastSel = 1;
    dropDownDocumentVersions = "#" + lastSel + "_DocumentVersions_" + currentInstance.ReportTemplateVersionID;
    dropDownDocumentDesignName = "#" + lastSel + "_DocumentDesignName_" + currentInstance.ReportTemplateVersionID;
    dataSourceNameColumn = "#" + lastSel + "_DataSourceName_" + currentInstance.ReportTemplateVersionID;

    promise.done(function (response) {
        //questionList = '', questionNameTextList = '', templateList = '';
        if (response.length != 0) {
            templateDocumentMappingData = response;
            var docDesignLst = "{";
            docDesignLst = docDesignLst + '"0":"Select One",';
            $(response).each(function () {
                docDesignLst = docDesignLst + "\"" + this.DocumentDesignId + "\":\"" + this.DocumentDesignName + "\",";
            });
            docDesignLst = docDesignLst.substr(0, docDesignLst.lastIndexOf(','));  // removes comma at the end.
            docDesignLst = docDesignLst + "}";
            collateralDocDesignLst = $.parseJSON(docDesignLst);
        }
    });

    var colArray = ['Document Design', 'Document Design Version', 'Data Source Name', 'Download'] //, 'SelectedVersion', 'Download'];
    var colModel = [];
    colModel.push({
        name: 'DocumentDesignName_' + currentInstance.ReportTemplateVersionID, index: 'DocumentDesignName_' + currentInstance.ReportTemplateVersionID, editable: true, width: '150', edittype: 'select', align: 'left', //formatter: 'select',
        editoptions: {
            value: collateralDocDesignLst,
            dataInit: function (elem) {
                $(elem).width(190);
                $.each(templateDocumentMappingData, function (value, key) {
                    if (key.TemplateReportFormDesignVersionMapID != null) {
                        $("#" + elem.id + " option[value='" + key.DocumentDesignId + "']").prop("selected", 1);
                        var selectDocData = $.grep(templateDocumentMappingData, function (n, i) {
                            return n.DocumentDesignId == key.DocumentDesignId;
                        });
                        if (selectDocData.length > 0) {
                            $(dropDownDocumentVersions).empty();
                            var docVersions = selectDocData[0].DocumentVersions;
                            var docVersionLst = "{";
                            docVersionLst = docVersionLst + '"0":"Select One",';
                            $(docVersions).each(function () {
                                var date = new Date(this.EffctiveDate);
                                var month = date.getMonth() + 1;
                                var day = date.getDate();
                                var year = date.getFullYear();
                                docVersionLst = docVersionLst + "\"" + this.VersionId + "\":\"" + this.VersionNo + " - " + month + "/" + day + "/" + year + "\",";
                            });
                            docVersionLst = docVersionLst.substr(0, docVersionLst.lastIndexOf(','));  // removes comma at the end.
                            docVersionLst = docVersionLst + "}";

                            collateralDesignVersionsLst = $.parseJSON(docVersionLst);

                            $.each(collateralDesignVersionsLst, function (val, txt) {
                                $(dropDownDocumentVersions).append('<option value="' + val + '">' + txt + '</option>');
                            });
                        }
                    }
                });
            },
            dataEvents: [
                                               {
                                                   type: 'change',
                                                   fn: function (e) {
                                                       var formDesignId = $(dropDownDocumentDesignName + " :selected")[0].value;

                                                       var selectDocData = $.grep(templateDocumentMappingData, function (n, i) {
                                                           return n.DocumentDesignId == formDesignId;
                                                       });
                                                       if (selectDocData.length > 0) {
                                                           $(dropDownDocumentVersions).empty();
                                                           var docVersions = selectDocData[0].DocumentVersions;
                                                           var docVersionLst = "{";
                                                           docVersionLst = docVersionLst + '"0":"Select One",';
                                                           $(docVersions).each(function () {
                                                               var date = new Date(this.EffctiveDate);
                                                               var month = date.getMonth() + 1;
                                                               var day = date.getDate();
                                                               var year = date.getFullYear();
                                                               docVersionLst = docVersionLst + "\"" + this.VersionId + "\":\"" + this.VersionNo + " - " + month + "/" + day + "/" + year + "\",";
                                                           });
                                                           docVersionLst = docVersionLst.substr(0, docVersionLst.lastIndexOf(','));  // removes comma at the end.
                                                           docVersionLst = docVersionLst + "}";

                                                           collateralDesignVersionsLst = $.parseJSON(docVersionLst);

                                                           $.each(collateralDesignVersionsLst, function (val, txt) {
                                                               $(dropDownDocumentVersions).append('<option value="' + val + '">' + txt + '</option>');
                                                           });
                                                           $(dataSourceNameColumn).val(selectDocData[0].DataSourceName);
                                                       }
                                                   }
                                               }
            ]
        }
    });
    colModel.push({
        name: 'DocumentVersions_' + currentInstance.ReportTemplateVersionID, index: 'DocumentVersions_' + currentInstance.ReportTemplateVersionID, width: '100', align: 'left', editable: true, edittype: "select",//formatter: 'select',
        editoptions: {
            value: initializeGridFilterValue = function () { },
            dataInit: function (elem) {
                $(elem).width(150);
                var $el = $(elem);
                $el.empty(); // remove old options 

                if (collateralDesignVersionsLst != null) {
                    $.each(collateralDesignVersionsLst, function (value, key) {
                        $el.append($("<option></option>").attr("value", value).text(key));
                    });
                }
                $.each(templateDocumentMappingData, function (value, key) {
                    if (key.SelectedVersion != null && key.TemplateReportFormDesignVersionMapID != null) {
                        $("#" + $el[0].id + " option[value='" + key.SelectedVersion.VersionId + "']").prop("selected", 1);
                    }
                });
            }
        }
    });
    colModel.push({ name: 'DataSourceName', index: 'DataSourceName', editable: true, hidden: true, edittype: "text", align: 'left' });  //, formatter: this.formatDocDesGridColumns, unformat: this.unformatDocDesGridColumns });
    colModel.push({ name: 'Download', index: 'Download', width: '60', align: 'left', sortable: false, search: false, editable: false, formatter: this.formatColumnForDownloadBtn });

    $(this.elementIDs.uploadDocMapForReportTableIDJQ).jqGrid('GridUnload');

    var disableInput = false;
    if (currentInstance.statustext == 'Finalized')
        disableInput = true;

    var designName = "Select One";
    var selectedDocVersionNumber = "Select One";
    if (templateDocumentMappingData != undefined || templateDocumentMappingData != null) {
        $.each(templateDocumentMappingData, function (value, key) {
            if (key.SelectedVersion != null && key.TemplateReportFormDesignVersionMapID != null) {
                designName = key.DocumentDesignName;
                var selectedDocVersionId = key.SelectedVersion.VersionId;
                var selectDocVersion = $.grep(templateDocumentMappingData, function (n, i) {
                    return n.TemplateReportFormDesignVersionMapID == key.TemplateReportFormDesignVersionMapID;
                });
                var selectDocVersionNum = $.grep(selectDocVersion[0].DocumentVersions, function (n, i) {
                    return n.VersionId == selectedDocVersionId;
                });
                selectedDocVersionNumber = selectDocVersionNum[0].VersionNo;
            }
        });
    }

    //As there will be always single row in grid, dynamically create row to be added to grid.
    var designDocumentNameColumn = 'DocumentDesignName_' + currentInstance.ReportTemplateVersionID;
    var designDocumentVersionsColumn = 'DocumentVersions_' + currentInstance.ReportTemplateVersionID
    var dataSourceNameColumn = 'DataSourceName_' + currentInstance.ReportTemplateVersionID

    var docMappingData = [
                 { designDocumentNameColumn: designName, designDocumentVersionsColumn: selectedDocVersionNumber, dataSourceNameColumn: 'DataSourceName', Download: 'Download' } //, SelectedVersion: response[0].SelectedVersion }
    ];

    var url = currentInstance.URLs.formTemplateDocumentMapping.replace(/{ReportTemplateVersionID}/, currentInstance.ReportTemplateVersionID);
    //adding the pager element
    $(currentInstance.elementIDs.uploadDocMapForReportTableIDJQ).parent().append("<div id='p" + currentInstance.elementIDs.uploadDocMapForReportTableID + "'></div>");
    //load the jqGrid - refer documentation for details
    $(currentInstance.elementIDs.uploadDocMapForReportTableIDJQ).jqGrid({
        url: '',
        name: 'templatejqGrid',
        mtype: 'POST',
        data: docMappingData,
        datatype: 'local',
        cache: false,
        colNames: colArray,
        cellEdit: false,
        colModel: colModel,
        caption: 'Template Document Mapping',
        height: '250',
        rowNum: 10000,
        ignoreCase: true,
        //loadonce: false,
        width: 750,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        pager: '#p' + currentInstance.elementIDs.uploadDocMapForReportTableID,
        sortname: 'DocumentDesignId',
        altclass: 'alternate',
        onSelectRow: function (id) {
            if (lastSel != undefined || lastSel != null) {
                $(currentInstance.elementIDs.uploadDocMapForReportTableIDJQ).jqGrid('saveRow', lastSel);
            }
            if (RptVrsnInstances["RptVrsn_" + currentInstance.ReportTemplateVersionID].statustext != 'Finalized') { //disableInput = "disabled='disabled'"
                $(currentInstance.elementIDs.uploadDocMapForReportTableIDJQ).editRow(id, true, null, null, 'clientArray', null,
                                     function (rowid, response) {
                                         $(currentInstance.elementIDs.uploadDocMapForReportTableIDJQ).setColProp('DocumentDesignName', { editoptions: { value: collateralDocDesignLst } });
                                         $(currentInstance.elementIDs.uploadDocMapForReportTableIDJQ).setColProp('DocumentVersions', { editoptions: { value: collateralDesignVersionsLst } });
                                     });
            }
            lastSel = id;
        },
        resizeStop: function (width, index) {
            autoResizing(currentInstance.elementIDs.uploadDocMapForReportTableIDJQ);
        },
        gridComplete: function (rowId) {
            var data = $(currentInstance.elementIDs.uploadDocMapForReportTableIDJQ).html();
            if (disableRowID > -1) {
                $(this).find('tr[id="' + disableRowID + '"]').each(function () {
                    //alert($(this).id);
                    $(this).addClass('ui-state-disabled');
                });
            }
            if (RptVrsnInstances["RptVrsn_" + currentInstance.ReportTemplateVersionID].statustext == 'Finalized') { //disableInput = "disabled='disabled'"
                $(currentInstance.elementIDs.uploadDocMapForReportTableIDJQ).editRow(lastSel, false, null, null, 'clientArray', null,
                                     function (rowid, response) {
                                         $(currentInstance.elementIDs.uploadDocMapForReportTableIDJQ).setColProp('DocumentDesignName', { editoptions: { value: collateralDocDesignLst } });
                                         $(currentInstance.elementIDs.uploadDocMapForReportTableIDJQ).setColProp('DocumentVersions', { editoptions: { value: collateralDesignVersionsLst } });
                                     });
            }
            else{
                $(currentInstance.elementIDs.uploadDocMapForReportTableIDJQ).editRow(lastSel, true, null, null, 'clientArray', null,
                                     function (rowid, response) {
                                         $(currentInstance.elementIDs.uploadDocMapForReportTableIDJQ).setColProp('DocumentDesignName', { editoptions: { value: collateralDocDesignLst } });
                                         $(currentInstance.elementIDs.uploadDocMapForReportTableIDJQ).setColProp('DocumentVersions', { editoptions: { value: collateralDesignVersionsLst } });
                                     });
            }
        },
        autowidth: true,
        shrinkToFit: true,
        afterInsertRow: function (rowId, rowData, rowElem) {
            if (disableInput)
                $(this).find('.ui-icon-circle-arrow-s').attr("disabled", "disabled").addClass("ui-state-disabled");
        }
    });

    var pagerElement = '#p' + currentInstance.elementIDs.uploadDocMapForReportTableID;
    //remove default buttons
    $(currentInstance.elementIDs.uploadDocMapForReportTableIDJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    //remove paging
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();
}

reportConfigurationGrid.prototype.generateGridData = function (xhr) {

    //get properties that need to be displayed for the element based on type
    var uiElementProperties = this.getUIElementProperties(this.reportName);
    //this.Location = xhr.Location;
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
                //case 'IsRelease':
                //    uiElementProperties[index].Value = xhr.IsRelease;
                //  break;
            case 'Template':
                uiElementProperties[index].Value = xhr.Template;
                break;
        }
    }
    return uiElementProperties;
}

reportConfigurationGrid.prototype.getUIElementProperties = function (currentInstance) {
    var elementProperties = [
             { IntProperty: 'ReportDescription', Property: 'Collateral Description', Value: '' },
             { IntProperty: 'Visible', Property: 'Visible', Value: '' },
             { IntProperty: 'Location', Property: 'Location', Value: '' },
             { IntProperty: 'RoleAccessPermission', Property: 'Role Access Permissions' },
             //{ IntProperty: 'Parameters', Property: 'Parameters', Value: '' },
             //{ IntProperty: 'ReportType', Property: 'Collateral Type', Value: '' },
             { IntProperty: 'HelpText', Property: 'Help Text', Value: '' },
             //{ IntProperty: 'IsRelease', Property: 'Is Release', Value: '' },
             //{ IntProperty: 'Template', Property: 'Template', Value: '' },
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
    $(this.gridElementId).jqGrid('GridUnload');  // this should be mapping Grid
    //set column list
    var colArray = ['IntProperty', 'Property', 'Value'];
    //set column  models
    var colModel = [];
    colModel.push({ name: 'IntProperty', index: 'IntProperty', key: true, hidden: true, search: false });
    colModel.push({ name: 'Property', index: 'Property', align: 'left', editable: false });
    colModel.push({ name: 'Value', index: 'Value', align: 'left', editable: false, formatter: this.formatColumn, unformat: this.unFormatColumn });

    $(this.gridElementId).parent().append("<div id='p" + this.gridElementIdNoHash + "'></div>");

    var currentInstance = this;
    var disableInput = false;
    if (currentInstance.statustext == 'Finalized')
        disableInput = true;

    $(this.gridElementId).jqGrid({
        datatype: 'local',
        colNames: colArray,
        colModel: colModel,
        autowidth: true,
        caption: currentInstance.reportName + "-" + currentInstance.VersionNo + " Properties",
        pager: '#p' + currentInstance.gridElementIdNoHash,
        hidegrid: false,
        height: '250',
        altRows: true,
        altclass: 'alternate',
        //register event handler for row insert
        afterInsertRow: function (rowId, rowData, rowElem) {
            if (disableInput) {
                if (rowId == 'RoleAccessPermission')
                    $(this).find('#RoleAccessPermission').find('.ui-icon-pencil').attr("disabled", "disabled").addClass("ui-state-disabled");
                if (rowId == 'Parameters')
                    $(this).find('#Parameters').find('.ui-icon-pencil').attr("disabled", "disabled").addClass("ui-state-disabled");
                if (rowId == 'Template')
                    $(this).find('#Template').find('.ui-icon-document').attr("disabled", "disabled").addClass("ui-state-disabled");
            }
        },
        gridComplete: function () {

        }
    });



    //insert rows in the grid
    for (var index = 0; index < uiElementProperties.length; index++) {
        $(this.gridElementId).jqGrid('addRowData', uiElementProperties[index].IntProperty, uiElementProperties[index]);
    }

    var pagerElement = '#p' + currentInstance.gridElementIdNoHash;
    //remove default buttons
    $(currentInstance.gridElementId).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();
    $(pagerElement + '_left').find("tr").children().last().find('div').css('padding-top', '5px');

    $(this.gridElementId).find('#Template').find('.ui-icon-document').click(function (e) {
        var id = $(this).attr('id');
        reportVersionID = id.split('_')[1];
        var currentInstance = RptVrsnInstances["RptVrsn_" + reportVersionID];
        currentInstance.DownLoadDocument(currentInstance.ReportTemplateVersionID);
    });

    //register event handler to display fields dialog when edit icon of fields property is clicked
    $(this.gridElementId).find('#Parameters').find('.ui-icon-pencil').click(function () {
        var id = $(this).attr('id');
        reportVersionID = id.split('_')[1];
        var currentInstance = RptVrsnInstances["RptVrsn_" + reportVersionID];   // currentInst.parametersDialog.parameter
        //currentInstance.parametersDialog = new parametersDialog(currentInstance.reportItem, currentInstance.formDesignVersionId, currentInstance.statustext);
        currentInstance.parametersDialog = new parametersDialog(currentInstance.parameterElementID, reportVersionID);
        currentInstance.parametersDialog.show();
        var dfg = '';
    });

    $(this.gridElementId).find('#RoleAccessPermission').find('.ui-icon-pencil').click(function () {
        var id = $(this).attr('id');
        reportVersionID = id.split('_')[1];
        var currentInstance = RptVrsnInstances["RptVrsn_" + reportVersionID];   // currentInst.parametersDialog.parameter
        currentInstance.roleAccessPermissionDialog = new roleAccessPermissionDialog(0, 0, currentInstance.ReportTemplateVersionID);
        currentInstance.roleAccessPermissionDialog.show();
    });
}

reportConfigurationGrid.prototype.DownLoadDocument = function (reportVersionID) {
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

// Prepares JSON from grid data to be sent over to controller on Form Submit.
reportConfigurationGrid.prototype.PostFormData = function () {
    var cuuIns = this;
    //alert(this.ReportTemplateVersionID);
    $(cuuIns.SaveButtonID).on('click', function () {
        var reportVersionID = $(this).attr('id').split('-')[1];
        var currentInstance = RptVrsnInstances["RptVrsn_" + reportVersionID];  //uploadDocMapForReportTableIDJQ
        var mappingGridID = $(currentInstance.elementIDs.uploadDocMapForReportTableIDJQ);
        var Url = currentInstance.URLs.SaveReportTemplateVersionUrl;

        var file;
        var fileName = $(currentInstance.fileInputElement).val();
        var allowedExtensions = ["docx", "doc", "docm"];
        if (fileName) {
            var fileN = fileName.split('.');
            if ($.inArray(fileN[fileN.length - 1], allowedExtensions) == -1)
                messageDialog.show('Please try uploading File with allowed extensions. Allowed extensions are .doc or .docx.');
            else
                file = $(currentInstance.fileInputElement)[0].files[0];//[0]["uploadTemplateFile"].files[0];
        }
        //else {
        // stringifying the mapping grid data
        //var mappingGridRows = mappingGridID.jqGrid('getRowData');

        //if (typeof mappingGridRows != undefined) {
        //    var i = mappingGridRows.length - 1;

        //    for (; i > -1; i--) {
        //        if (!mappingGridRows[i]['Action'] || !mappingGridRows[i]['DocumentVersions'])
        //            mappingGridRows.splice(i, 1);
        //        else {
        //            delete mappingGridRows[i]['DocumentVersions'];
        //            delete mappingGridRows[i]['Action'];
        //            delete mappingGridRows[i]['Download'];

        //            var selectedVersion = mappingGridRows[i].SelectedVersion;
        //            var parsedValue = $.parseJSON(selectedVersion);
        //            mappingGridRows[i].SelectedVersion = parsedValue;
        //        }
        //    }
        //}
        var jsonArr = [];
        var documentDesignId = $(dropDownDocumentDesignName + " :selected")[0].value;
        if (documentDesignId != "0") {
            var formDesignVersionID = $(dropDownDocumentVersions + " :selected")[0].value;
            if (formDesignVersionID == "0") {
                messageDialog.show("Please select document design version.");
                return false;
            }
            var dataSourceName = $(dataSourceNameColumn).val();

            var obj = {};
            obj["DocumentDesignId"] = documentDesignId;
            obj["FormDesignVersionID"] = formDesignVersionID;
            obj["DataSourceName"] = dataSourceName == undefined ? "" : dataSourceName;
            jsonArr.push(obj);
        }

        var templateDocMappings = JSON.stringify(jsonArr);
        //var templateDocMappings = JSON.stringify($.parseJSON(mappingGridRows));
        var Updatedproperties = currentInstance.readGridData(currentInstance);
        var templateProperties = JSON.stringify(Updatedproperties);
        var Parameters = '';
        if (currentInstance.parametersDialog) {
            var Paramtrs = currentInstance.parametersDialog.parameter;
            if (Paramtrs) {
                var parametersLength = Paramtrs.length - 1;
                for (; parametersLength > -1; parametersLength--) {
                    if (!Paramtrs[parametersLength].IsSelected)
                        Paramtrs.splice(parametersLength, 1);
                }
                Parameters = JSON.stringify(Paramtrs);
            }
        }
        // int ReportVersionID, string reportTemplateName, string VersionNumber, string templateDocMappings, string templateProperties, string Parameters
        Url = Url.replace(/{ReportVersionID}/, reportVersionID).replace(/{VersionNumber}/, currentInstance.VersionNo)
               .replace(/{reportTemplateName}/, currentInstance.reportName).replace(/{Parameters}/, Parameters).replace(/{templateDocMappings}/, templateDocMappings)
               .replace(/{templateProperties}/, templateProperties);

        var formData = new FormData();
        formData.append('reportTemplateVersionID', reportVersionID);
        if (fileName) {
            formData.append('uploadedFileName', file.name);
            formData.append('File', file);
        }
        //formData.append('reportTemplateName', currentInstance.reportName);
        //formData.append('VersionNumber', currentInstance.VersionNo);
        //formData.append('templateDocMappings', templateDocMappings);
        //formData.append('templateProperties', propertiesMappings);
        //formData.append('Parameters', Parameters);

        $.ajax({
            type: 'POST',
            url: Url,
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                var message = 'There was a Failure processing the Request.';
                try {
                    if (result.Result == ServiceResult.SUCCESS) {
                        //currentInstance.loadDocMapGrid();
                        //currentInstance.loadPropertyGrid();
                        //reportTemplate.viewReportTemplates();
                        //var reportTab = $(document).find('li a[id="reportNameTab"]').click();
                        message = 'Collateral Template Version Updated Successfully.' + ((result.Items.length > 1) ? "\n Template File Uploaded Successfully." : "");

                        if (result.Items.length > 1) {
                            var reportUploadMappedDoc = '#reportUploadMappedDoc';
                            var fileInputElement = '#uploadTemplateFile-' + reportVersionID;
                            var fileInputLabelElement = '#uploadTemplateFileLabel-' + reportVersionID;
                            var fileInputChangeElement = '#uploadTemplateFileChange-' + reportVersionID;
                            var reportUploadMappedDocDivContent = $('<div>' + $(reportUploadMappedDoc).html().replace(/##ID/gi, reportVersionID) + '</div>');
                            reportUploadMappedDocDivContent.find(fileInputChangeElement).css('display', 'normal');
                            reportUploadMappedDocDivContent.find(fileInputLabelElement).css('display', 'none');
                            reportUploadMappedDocDivContent.find(fileInputElement).css('display', 'none');
                        }
                        messageDialog.show(message);
                    }
                    else
                        messageDialog.show(message);
                }
                catch (ex) {
                    messageDialog.show(message);
                }
            }
        });
        return false;
        //}
        //return true;
    });
};

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
reportConfigurationGrid.prototype.readGridData = function (currentInstance) {
    var uiElementProperties = currentInstance.getUIElementProperties(currentInstance);
    var parameterValue = $(currentInstance.parameterElementID).val();
    var updateElement = {};
    var updateElement2 = {};
    //updateElement.ElementType = currentInst.reportName;
    //updateElement.UIElementID = this.uiElement.UIElementID;
    //updateElement.FormDesignID = this.formDesignId;
    //updateElement.FormDesignVersionID = this.formDesignVersionId;
    //updateElement.ParentUIElementID = this.uiElement.ParentUIElementID;
    updateElement.ReportVersionID = currentInstance.ReportTemplateVersionID;
    updateElement.ReportDesignId = currentInstance.ReportId;

    //updateElement.ReportDesignId = this.uiElementDetail.ReportDesignId;
    //iterate through each element and set the values in the updateElement object
    var RowIDs = $(currentInstance.gridElementId).getDataIDs();
    var dataRows = $(currentInstance.gridElementId).getRowData();
    $.each(RowIDs, function (i, obj) {
        updateElement2[obj.IntProperty] = obj.Value;
    });

    for (var index = 0; index < uiElementProperties.length; index++) {
        updateElement[uiElementProperties[index].IntProperty] = $(currentInstance.gridElementId).getRowData(uiElementProperties[index].IntProperty).Value;
    }
    if (parameterValue != undefined)
        //updateElement.Parameters = currentInstance.parametersDialog.parameter;

        updateElement.ReportVersionID = currentInstance.ReportTemplateVersionID;
    return updateElement;
}

reportConfigurationGrid.prototype.formatColumn = function (cellValue, options, rowObject) {

    var temp = $(this).attr('id');
    var reportVersionID = temp.split('-')[1];
    var currentInstance = RptVrsnInstances["RptVrsn_" + reportVersionID];
    //   canbeFinalized
    var result;
    switch (rowObject.IntProperty) {
        case 'Location':
            result = '<select style="width:100%" class="form-control"><option value="All"  ' + (rowObject.Value == "All" ? "selected='selected'" : "") + ' >All</option><option value="InFolder" ' + (rowObject.Value == "InFolder" ? "selected='selected'" : "") + ' >In Folder</option><option value="InMenu"  ' + (rowObject.Value == "InMenu" ? "selected='selected'" : "") + ' >In Menu</option></select>';
            break;
        case 'ReportType':
            result = '<select style="width:100%" class="form-control"><option value="Folder"  ' + (rowObject.Value == "Folder" ? "selected='selected'" : "") + ' >Folder</option><option value="Account" ' + (rowObject.Value == "Account" ? "selected='selected'" : "") + ' >Account</option></select>';
            break;
        case 'Visible':
            if (cellValue == true) {
                result = '<input type="checkbox" checked/>';
            }
            else {
                result = '<input type="checkbox" />';
            }
            break;
        case 'IsLibraryRegex':
            //case 'IsRelease':
            //    if (cellValue == true) {
            //        result = '<input type="checkbox" checked/>';
            //    }
            //    else {
            //        result = (currentInstance.canbeFinalized ? '<input type="checkbox" />' : ' ');  //'<input type="checkbox" ' + (currentInstance.canbeFinalized ? '' : ' class="ui-state-disabled" ') + ' />';
            //    }
            //    break;
        case 'RoleAccessPermission':
        case 'Parameters':
            if (cellValue === null || cellValue === undefined) {
                cellValue = '';
            }
            result = '<div><span id=Parameters_' + reportVersionID + ' class="ui-icon ui-icon-pencil" title = "Manage ' + rowObject.Property + '" style = "cursor: pointer;"></span></div>';
            break;
        case 'ReportDescription':
        case 'HelpText':
            result = '<textarea style="width:100%;" >' + cellValue + '</textarea>';
            break;
        case 'Template':
            if (cellValue === null || cellValue === undefined) {
                cellValue = '';
            }
            if (currentInstance.Location != null && currentInstance.Location != undefined && currentInstance.Location != '')
                return '<div style="float:right;width:55%"><span id=Template_' + reportVersionID + ' class="ui-icon ui-icon-document view" title = "Download ' + rowObject.Property + '" style = "cursor: pointer;"></span></div>';
            else
                return '<div style="float:right;width:55%"><span title = "Download ' + rowObject.Property + '" style = "cursor: pointer;"></span></div>';
    }
    return result;
}

reportConfigurationGrid.prototype.formatDocDesGridColumns = function (cellValue, options, rowObject) {
    var reportVersionID = options.gid.split('-')[1];     //$(this).attr('id').split('-')[1];
    var Id = reportVersionID + "_" + options.rowId;
    var defaultOption = '<option value="">Select</option>';
    var disableInput = "";
    if (RptVrsnInstances["RptVrsn_" + reportVersionID].statustext == 'Finalized')
        disableInput = "disabled='disabled'"

    switch (options.pos.toString()) {
        case '2': {
            Id = "drp_" + Id;
            var result = "<select " + disableInput + " id='" + Id + "' class='form-control hastooltip'>";
            result += defaultOption;
            if (typeof rowObject.DocumentVersions != 'undefined' && rowObject.DocumentVersions != null) {
                $.each(rowObject.DocumentVersions, function (i, obj) {
                    result += "<option value='" + obj.VersionId + "-" + ((obj.VersionNo == null || obj.VersionNo == undefined) ? "0" : obj.VersionNo) + "' " + ((rowObject.SelectedVersion.VersionId == obj.VersionId) ? " selected='selected'" : "") + " >" + obj.VersionNo + "</option>";
                });
                result += '</select>';
            }
            return result;
        }
            //case '3': {
            //    Id = "chk_" + Id;
            //    return "<input " + disableInput + " id = '" + Id + "' type='checkbox' " + (rowObject.SelectedVersion.VersionId > 0 ? "checked='checked'" : "") + " title = 'Select' />";   //  style = 'cursor: pointer'
            //}
        case '4': {
            Id = "txt_" + Id;
            return "<input " + disableInput + " id = '" + Id + "' type='text' style='padding-bottom:5px;' " + ((rowObject.DataSourceName != undefined || rowObject.DataSourceName != null) ? "value='" + rowObject.DataSourceName + "'" : "") + " title = 'Enter' />";
        }
    }
    var rtrt = '';
}

reportConfigurationGrid.prototype.unformatDocDesGridColumns = function (cellValue, options, rowObject) {
    var reportVersionID = $(this).attr('id').split('-')[1];
    var Id = reportVersionID + "_" + options.rowId;
    var switchValue = options.colModel.index;

    switch (switchValue) {
        case 'DocumentVersions': {
            Id = "#drp_" + Id;
            var Value = $(Id).val().split('-');
            if (Value.length > 1) {
                //var selectedVersion = { "VersionId": Value[0], "VersionNo": Value[1] };
                var selectedVersion = { VersionId: Value[0], VersionNo: Value[1] };
                var objh = rowObject.SelectedVersion;

                var stringifiedObject = JSON.stringify(selectedVersion);
                $('#' + $(this).attr('id')).jqGrid('setCell', options.rowId, 'SelectedVersion', stringifiedObject);              // To Set a Grid Cell Value Format is :  $('#GridID').jqGrid('setCell',RowID,ColumnName,value);
                $('#' + $(this).attr('id')).jqGrid('getCell', options.rowId, 'SelectedVersion').VersionId;
            }
            return $(Id).val();
        }
        case 'Action': {
            Id = "#chk_" + Id;
            return $(Id).prop('checked');
        }
        case 'DataSourceName': {
            Id = "#txt_" + Id;
            return $(Id).val();
        }
    }
}

reportConfigurationGrid.prototype.formatColumnForDownloadBtn = function (cellValue, options, rowObject) {
    if (typeof rowObject.designDocumentVersionsColumn != 'undefined') {
        var reportVersionID = $(this).attr('id').split('-')[1];
        if (typeof TenantID === 'undefined')
            TenantID = 1;
        var idDwn = dropDownDocumentDesignName; // + reportVersionID + "_" + rowObject.DocumentDesignId;

        if (idDwn) {
            return "<span id = 'desDwn" + rowObject.designDocumentNameColumn + "' class='ui-icon ui-icon-circle-arrow-s' onclick='downloadDocumentDesignXML(" + TenantID + ", \"" + idDwn + "\")' title = 'Download Xml' style = 'cursor: pointer;margin-left:60px;' ></span></div>";
        }
        else {
            var ret = '';
        }
    }
    else {
        return "<span id = 'desDwn" + rowObject.DocumentDesignId + "' class='ui-icon ui-icon-circle-arrow-s' title = 'Download Xml' style = 'cursor: pointer;margin-left:60px;'/>";
    }
}

// Downloads the XML for the selected version of the Document.
function downloadDocumentDesignXML(TenantID, idDwn) {
    if (typeof TenantID === 'undefined')
        TenantID = 1;
    var FormDesignVersionId = $(idDwn).val();
    if (FormDesignVersionId) {
        FormDesignVersionId = FormDesignVersionId.split('-')[0];

        var promise = ajaxWrapper.getJSON("/DocumentCollateral/CheckDSFileDownloadPossibility?TenantID=" + TenantID + "&FormDesignVersionId=" + FormDesignVersionId);

        promise.done(function (result) {
            if (result == "Success")
                window.location.href = "/DocumentCollateral/DownloadDocumentDesignVersionXML?TenantID=" + TenantID + "&FormDesignVersionId=" + FormDesignVersionId;
            else
                messageDialog.show('There was an Error Downloading the File for the Selected Version.');
        });

        //window.location.href = "/DocumentCollateral/DownloadDocumentDesignVersionXML?TenantID=" + TenantID + "&FormDesignVersionId=" + FormDesignVersionId;
    }
    else
        messageDialog.show('Please select Document Version to Download the XML for.');
}

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

//unformat the grid column based on element property
//used in unformat in colModel for the Value column : bindToPropertyGrid method
reportConfigurationGrid.prototype.unFormatColumn = function (cellValue, options) {
    var temp = $(this).attr('id');
    var reportVersionID = temp.split('-')[1];
    var currentInstance = RptVrsnInstances["RptVrsn_" + reportVersionID];

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
            //case 'IsRelease':
            result = currentInstance.canbeFinalized ? $(this).find('#' + options.rowId).find('input').prop('checked') : false;
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

//return methods which can be used to get master lists
var masterListManager = function () {
    var URLs = {
        libraryRegex: '/MasterList/LibraryRegexList?tenantId={tenantId}'
    }

    return {
        //get library regexes
        getLibraryRegexes: function (tenantId) {
            var url = URLs.libraryRegex.replace(/\{tenantId\}/g, tenantId);
            var promise = ajaxWrapper.getJSONCache(url);
            return promise;
        }
    }

}();

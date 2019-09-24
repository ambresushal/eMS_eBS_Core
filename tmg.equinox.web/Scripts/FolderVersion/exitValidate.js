var exitValidate = function () {
    var isInitialized = false;
    var formInstanceData = undefined;
    var defaultSections = undefined;
    var isSetAsDefault = undefined;
    //var rowID = undefined;

    //urls to be accessed for create/copy form.
    var URLs = {
        getIsExitValidate: '/ExitValidate/GetIsExitValidate',
        postExitValidate: '/ExitValidate/Validate',
        getExitValidateErrors: '/ExitValidate/GetExitValidateErrors',
        isExitValidateInProgress: '/ExitValidate/IsExitValidateInProgress',
        exportExitValidate: '/ExitValidate/ExportExitValidate',
    };

    //element ID's required for create/copy form.
    var elementIDs = {
        exitValidateDialogJQ: '#exitValidateDialog',
        exitValidationGridJQ: '#exitValidationGrid',
        exitValidationGrid: 'exitValidationGrid',
        exitValidateSelectAllCheckboxJQ: '#selectAllCheckBox',
        exitValidateAllIsIncludeCheckboxJQ: '[id^=cbSection_]',
        btnExitValidateJQ: '#btnExitValidate',
        btnValidateJQ: '#btnTextValidate',
        cbDefaultSelectJQ: '#cbDefaultSelect',

        exitValidateLogGrid: 'exitValidateLogGrid',
        exitValidateLogGridJQ: '#exitValidateLogGrid',
        btnExitValidateResultsJQ: '#btnExitValidateResults',

    };

    //this function is called below soon after this JS file is loaded

    function init() {
        $(document).ready(function () {
            if (isInitialized == false) {
                $(elementIDs.exitValidateDialogJQ).dialog({ autoOpen: false, width: 500, height: 400, modal: true });
                isInitialized = true;
            }
            $(elementIDs.btnExitValidateJQ).click(function () {
                //return exitValidate.show();
                var isQueued = exitValidate.isExitValidateInProgress(formInstanceId);
                if (isQueued == true) {
                    messageDialog.show("Exit Validate is already queued for processing");
                }
                else {
                    loadExitValidate();
                    checkForExitValidateCompletion(formInstanceId);
                }
            });
        });

    }

    init();

    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    function loadExitValidate() {

        var currentInstance = this;
        var secList = ["Section A", "Section B1", "Section B2", "Section B3", "Section B4", "Section B5", "Section B6", "Section B7",
        "Section B8", "Section B9", "Section B10", "Section B11", "Section B12", "Section B13", "Section B14", "Section B15", "Section B16",
        "Section B17", "Section B18", "Section B19", "Section B20", "Section C", "Section D", "Section RX"];

        var exitvalidateData = {
            FormInstanceID: formInstanceData.formInstanceId,
            FolderVersionID: formInstanceData.folderVersionId,
            FolderID: formInstanceData.folderId,
            ProductID: "",
            UserID: formInstanceData.currentUserId,
            Sections: secList,
            SetAsDefault: $(elementIDs.cbDefaultSelectJQ).is(':checked'),
            FormDesignVersionID: formInstanceData.formDesignVersionId,
            FormName: formInstanceData.designData.FormName,
            Name: formInstanceData.formName,
        };

        var promiseValidateTask = ajaxWrapper.postJSONCustom(URLs.postExitValidate, exitvalidateData);
        promiseValidateTask.done(function (xhr) {
            if (xhr.Result == ServiceResult.FAILURE) {
                messageDialog.show("Exit Validate request could not be queued");
            }
            else if (xhr.Result == ServiceResult.SUCCESS) {
                messageDialog.show("Exit Validate is queued for processing, Please do not make any changes to the document.");
                //start loop to load 
            }
        });
    }

    function checkForExitValidateCompletion(formInstanceId) {
        setTimeout(function () {
            if (exitValidate.isExitValidateInProgress(formInstanceId) == true) {
                checkForExitValidateCompletion(formInstanceId);
            }
            else {
                exitValidate.loadExitValidateResults(formInstanceId);
                messageDialog.show("Exit Validate completed. Please view results in the Exit Validate tab.");
            }
        }, 15000);
    }

    function loadExitValidateGrid() {

        var selectAllCheckbox = '<input type="checkbox" class="check-all" id="selectAllCheckBox" style="margin-left: 5px;" />';
        var currentInstance = this;
        var dataJson = [
            {
                Section: 'Section A',
                Validate: '',
            }, {
                Section: 'Section B1',
                Validate: '',
            }, {
                Section: 'Section B2',
                Validate: '',
            }, {
                Section: 'Section B3',
                Validate: '',
            }, {
                Section: 'Section B4',
                Validate: '',
            }, {
                Section: 'Section B5',
                Validate: '',
            }, {
                Section: 'Section B6',
                Validate: '',
            }, {
                Section: 'Section B7',
                Validate: '',
            }, {
                Section: 'Section B8',
                Validate: '',
            }, {
                Section: 'Section B9',
                Validate: '',
            }, {
                Section: 'Section B10',
                Validate: '',
            }, {
                Section: 'Section B11',
                Validate: '',
            }, {
                Section: 'Section B12',
                Validate: '',
            }, {
                Section: 'Section B13',
                Validate: '',
            }, {
                Section: 'Section B14',
                Validate: '',
            }, {
                Section: 'Section B15',
                Validate: '',
            }, {
                Section: 'Section B16',
                Validate: '',
            }, {
                Section: 'Section B17',
                Validate: '',
            }, {
                Section: 'Section B18',
                Validate: '',
            }, {
                Section: 'Section B19',
                Validate: '',
            }, {
                Section: 'Section B20',
                Validate: '',
            }, {
                Section: 'Section C',
                Validate: '',
            }, {
                Section: 'Section D',
                Validate: '',
            }, {
                Section: 'Section RX',
                Validate: '',
            },
        ];

        var colArray = ['Section', selectAllCheckbox + '<span style="margin-left: 5px; position: absolute;">Validate</span>'];
        var colModel = [];
        colModel.push({ name: 'Section', index: 'Section', editable: false, align: 'left', search: true, searchoptions: { sopt: ["cn"] } });
        colModel.push({ name: 'Validate', index: 'Validate', align: 'center', formatter: exitValidateFormatter, search: false, sortable: false });

        //set column models
        $(elementIDs.exitValidationGridJQ).jqGrid('GridUnload');

        $(elementIDs.exitValidationGridJQ).jqGrid({
            data: dataJson,
            datatype: 'local',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: "Exit Validate Sections List",
            pager: '#p' + elementIDs.exitValidationGrid,
            height: '300',
            rowheader: false,
            loadonce: true,
            rowNum: 10000,
            autowidth: true,
            gridview: true,
            viewrecords: true,
            altRows: true,
            altclass: 'alternate',
            gridComplete: function () {

                initializeCheckAll();

                $(elementIDs.exitValidateSelectAllCheckboxJQ).off('change');
                $(elementIDs.exitValidateSelectAllCheckboxJQ).change(function () {
                    var val = $(this).is(':checked');
                    $(elementIDs.exitValidateAllIsIncludeCheckboxJQ).filter(':not(:disabled)').prop('checked', val);
                    setSelectAllCheckbox();
                });

                $(elementIDs.exitValidateAllIsIncludeCheckboxJQ).off('change');
                $(elementIDs.exitValidateAllIsIncludeCheckboxJQ).change(function () {
                    setSelectAllCheckbox();
                });

                $(elementIDs.btnValidateJQ).off('click');
                $(elementIDs.btnValidateJQ).on('click', function () {
                    var secList = [];
                    var objSecList = $(elementIDs.exitValidateAllIsIncludeCheckboxJQ).filter(':checked');
                    if (objSecList.length > 0) {
                        $.each(objSecList, function (index) {
                            //secList += objSecList[index].name;
                            //if (objSecList.length != index + 1)
                            //    secList += ',';
                            secList[index] = objSecList[index].name;
                        });
                    }
                    var exitvalidateData = {
                        FormInstanceID: formInstanceData.formInstanceId,
                        FolderVersionID: formInstanceData.folderVersionId,
                        FolderID: formInstanceData.folderId,
                        ProductID: "",
                        UserID: formInstanceData.currentUserId,
                        Sections: secList,
                        SetAsDefault: $(elementIDs.cbDefaultSelectJQ).is(':checked'),
                        FormDesignVersionID: formInstanceData.formDesignVersionId,
                        FormName: formInstanceData.designData.FormName,
                        Name: formInstanceData.formName,
                    };

                    var promiseValidateTask = ajaxWrapper.postJSONCustom(URLs.postExitValidate, exitvalidateData);
                    promiseValidateTask.done(function (xhr) {
                        if (xhr.Result == ServiceResult.FAILURE) {
                            messageDialog.show("Error occured while queueing Exit Validate");
                        }
                        else if (xhr.Result == ServiceResult.SUCCESS) {
                            $(elementIDs.exitValidateDialogJQ).dialog("close");
                        }

                    });
                });
            }
        });

        $(elementIDs.exitValidationGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
        //To initialize all check-all checkboxes
        function initializeCheckAll() {
            $(elementIDs.exitValidateSelectAllCheckboxJQ).closest(".ui-jqgrid-sortable").removeClass("ui-jqgrid-sortable");
            setSelectAllCheckbox();
        }

        function setSelectAllCheckbox() {
            if ($(elementIDs.exitValidateAllIsIncludeCheckboxJQ).filter(':not(:checked):not(:disabled)').length > 0 ||
                $(elementIDs.exitValidateAllIsIncludeCheckboxJQ).filter(':checked').length == 0)
                $(elementIDs.exitValidateSelectAllCheckboxJQ).prop('checked', false);
            else
                $(elementIDs.exitValidateSelectAllCheckboxJQ).prop('checked', true);
        }

    }

    function loadExitValidateErrorGrid(formInstanceId) {

        var colArray = ['ExitValidateResultID', 'ExitValidateQueueID', 'FormInstanceID', 'ContractNumber', 'PlanName', 'Section', 'Status', 'Result', 'Screen', 'Question', 'Error', 'PBPViewSection'];//, 'SectionID', 'Section', 'GeneratedName', 'RowNum', 'FormInstance', 'ElementID', 'RowIdProperty'];
        var colModel = [];
        colModel.push({ name: 'ExitValidateResultID', index: 'ExitValidateResultID', hidden: true, editable: false, align: 'left', search: true, searchoptions: { sopt: ["cn"] } });
        colModel.push({ name: 'ExitValidateQueueID', index: 'ExitValidateQueueID', hidden: true, editable: false, align: 'left', search: true, searchoptions: { sopt: ["cn"] } });
        colModel.push({ name: 'FormInstanceID', index: 'FormInstanceID', hidden: true, editable: false, align: 'left', search: true, searchoptions: { sopt: ["cn"] } });
        colModel.push({ name: 'ContractNumber', index: 'ContractNumber', hidden: true, editable: false, align: 'left', search: true, searchoptions: { sopt: ["cn"] } });
        colModel.push({ name: 'PlanName', index: 'PlanName', hidden: true, editable: false, align: 'left', search: true, searchoptions: { sopt: ["cn"] } });
        colModel.push({ name: 'Section', index: 'Section', editable: false, align: 'left', search: true, searchoptions: { sopt: ["cn"] } });
        colModel.push({ name: 'Status', index: 'Status', hidden: true, editable: false, align: 'left', search: true, searchoptions: { sopt: ["cn"] } });
        colModel.push({ name: 'Result', index: 'Result', hidden: true, editable: false, align: 'left', search: true, searchoptions: { sopt: ["cn"] } });
        colModel.push({ name: 'Screen', index: 'Screen', editable: false, align: 'left', search: true, searchoptions: { sopt: ["cn"] } });
        colModel.push({ name: 'Question', index: 'Question', editable: false, align: 'left', search: true, searchoptions: { sopt: ["cn"] } });
        colModel.push({ name: 'Error', index: 'Error', editable: false, align: 'left', search: true, searchoptions: { sopt: ["cn"] } });
        colModel.push({ name: 'PBPViewSection', index: 'PBPViewSection', hidden: true, editable: false, align: 'left', search: true, searchoptions: { sopt: ["cn"] } });
        
        //colModel.push({ name: 'SectionID', index: 'SectionID', hidden: true, editable: false, align: 'left' });
        //colModel.push({ name: 'Section', index: 'Section', hidden: true, editable: false, align: 'left' });
        //colModel.push({ name: 'GeneratedName', index: 'GeneratedName', hidden: true, editable: false, align: 'left' });
        //colModel.push({ name: 'RowNum', index: 'RowNum', hidden: true, editable: false, align: 'left' });
        //colModel.push({ name: 'FormInstance', index: 'FormInstance', hidden: true, editable: false, align: 'left' });
        //colModel.push({ name: 'ElementID', index: 'ElementID', hidden: true, editable: false, align: 'left' });
        //colModel.push({ name: 'RowIdProperty', index: 'RowIdProperty', hidden: true, editable: false, align: 'left' });

        //set column models
        $(elementIDs.exitValidateLogGridJQ).jqGrid('GridUnload');
        var URL = URLs.getExitValidateErrors + "?formInstanceId=" + formInstanceId
        $(elementIDs.exitValidateLogGridJQ).parent().append("<div id='p_" + elementIDs.exitValidateLogGrid + "'></div>");
        $(elementIDs.exitValidateLogGridJQ).jqGrid({
            //data: dataJson,
            rowList: [10, 20, 30],
            url: URL,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: "Exit Validate Result",
            pager: '#p_' + elementIDs.exitValidateLogGrid,
            rowheader: false,
            loadonce: true,
            rowNum: 30,
            autowidth: true,
            gridview: true,
            viewrecords: true,
            altRows: true,
            altclass: 'alternate',
            gridComplete: function () {
                var pagerElement = '#p_' + elementIDs.exitValidateLogGrid;
                $(pagerElement + "_right").hide();
                var rowCount = $(elementIDs.exitValidateLogGridJQ).getRowData().length;
                if (rowCount == 0) {
                    $(elementIDs.exitValidateLogGridJQ).jqGrid('setCaption', 'Exit Validate Result - <b>no Errors</b>');
                }
                else {
                    $(elementIDs.exitValidateLogGridJQ).jqGrid('setCaption', 'Exit Validate Result');
                }
            }
        });
        var pagerElement = '#p_' + elementIDs.exitValidateLogGrid;
        $(elementIDs.exitValidateLogGridJQ).jqGrid('navGrid', pagerElement, { edit: false, view: false, add: false, del: false, search: false, refresh: false });
        $(elementIDs.exitValidateLogGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
        $(elementIDs.exitValidateLogGridJQ).jqGrid('navButtonAdd', pagerElement,
          {
              caption: '', buttonicon: 'ui-icon-pencil', title: 'Edit', id: 'EVEdit',
              onClickButton: function () {
                  var rowID = $(elementIDs.exitValidateLogGridJQ).getGridParam('selrow');
                  if (rowID !== undefined && rowID !== null) {
                      var row = $(elementIDs.exitValidateLogGridJQ).getRowData(rowID);
                      var idx = 0;

                      $.each($("#sectionMenuContainerBottom select option"), function () {
                          if ($(this).val().trim() == row.PBPViewSection) {
                              console.log($(this).val());
                              $("#sectionMenuContainerBottom select").val($(this).val());
                              $("#sectionMenuContainerBottom select").trigger('change');
                              return;
                          }
                          idx++;
                      });

                      //var _errorManager = new errorManager(formInstanceData);
                      //_errorManager.evSubgridRowSelect(rowID, elementIDs.exitValidateLogGridJQ);

                  }
              }
          });
        $(elementIDs.exitValidateLogGridJQ).jqGrid('navButtonAdd', pagerElement,
         {
             caption: '', buttonicon: 'ui-icon-arrowstop-1-s', title: 'Export to Excel', id: 'btnExitValidateExportToExcel',
             onClickButton: function () {
                 var countRecord = $(elementIDs.exitValidateLogGridJQ).jqGrid('getGridParam', 'records');
                 if (countRecord > 0) {
                     $(window).off('beforeunload');
                     var currentInstance = this;
                     var jqGridtoCsv = new JQGridtoCsv(elementIDs.exitValidateLogGridJQ, false, currentInstance);
                     jqGridtoCsv.buildExportOptions();
                     var stringData = "csv=" + jqGridtoCsv.csvData;
                     stringData += "<&isGroupHeader=" + jqGridtoCsv.isGroupHeader;
                     stringData += "<&noOfColInGroup=" + jqGridtoCsv.noofGroupedColumns;
                     stringData += "<&repeaterName=" + elementIDs.userListGrid;
                     success: $.download(URLs.exportExitValidate + "?formInstanceId=" + formInstanceId, stringData, 'post');
                 }
                 else {
                     messageDialog.show("No exit validate log found for this product.");
                 }
             }
         });
    }

    function applyValidation() {
        var formInstanceId = formInstancebuilder.formInstanceId;
        var row = $(elementIDs.exitValidateLogGridJQ).getRowData(rowID);
        if (row.ElementID.indexOf("Repeater") != -1) {
            var repeaterID = row.ElementID.substring(0, row.ElementID.indexOf("_")) + row.FormInstanceID;
            $(formInstancebuilder.formInstanceDivJQ).find("#" + repeaterID).find("table.pq-grid-table").parent().css({ "border": "solid 1px #a94442" });

            $(window).scrollTop($(formInstancebuilder.formInstanceDivJQ).find("#" + repeaterID).offset().top);
        }
        else if (row.RowNum == "") {
            $(formInstancebuilder.formInstanceDivJQ).find("#" + row.ElementID).parent().addClass("has-error");
            //$(".container-fluid " + formInstancebuilder.formInstanceDivJQ).find("#" + row.ElementID).parent().addClass("has-error");
            //$(window).scrollTop($(".container-fluid " + formInstancebuilder.formInstanceDivJQ).find("#" + row.ElementID).offset().top - 50);
            //$(".container-fluid " + formInstancebuilder.formInstanceDivJQ).find("#" + row.ElementID).focus();
        }
    }

    return {
        show: function () {
            $(elementIDs.exitValidateDialogJQ + ' div').removeClass('has-error');
            $(elementIDs.exitValidateDialogJQ + ' .help-block').text('');

            $(elementIDs.exitValidateDialogJQ).dialog('option', 'title', 'Exit Validate');
            $(elementIDs.exitValidateDialogJQ).dialog({
                position: {
                    my: 'center',
                    at: 'center'
                },
            });
            $(elementIDs.exitValidateDialogJQ).dialog('open');
            loadExitValidateGrid();

        },

        exitValidateMenu: function (result) {
            formInstanceData = result;
            if (result.designData.FormName == "PBPView" || result.designData.FormName == "VBIDView") {
                var promise = ajaxWrapper.getJSON(URLs.getIsExitValidate);
                promise.done(function (evresult) {
                    if (evresult.Sections != null) {
                        defaultSections = evresult.Sections.length > 0 ? evresult.Sections : null;
                    }
                    isSetAsDefault = evresult.SetAsDefault;
                    if (isSetAsDefault == true) {
                        $(elementIDs.cbDefaultSelectJQ).attr('checked', true);
                    }
                    else {
                        $(elementIDs.cbDefaultSelectJQ).attr('checked', false);
                    }
                    //if (evresult.IsExitValidate == "YES") {
                    if (result.designData.FormName == "PBPView" || result.designData.FormName == "VBIDView") {
                        $(elementIDs.btnExitValidateJQ).show();
                    }
                    else {
                        $(elementIDs.btnExitValidateJQ).hide();
                    }
                    $(elementIDs.btnExitValidateResultsJQ).show();
                    //}
                });
                promise.fail(showError);
            }
            else {
                $(elementIDs.btnExitValidateJQ).hide();
                $(elementIDs.btnExitValidateResultsJQ).hide();
            }
        },
        loadExitValidateResults: function (formInstanceId) {
            if (this.isExitValidateInProgress(formInstanceId)) {
                messageDialog.show("Exit Validate is queued for processing, Please do not make any changes to the document.");
            }
            else {
                loadExitValidateErrorGrid(formInstanceId);
            }
        },
        clearExitValidateResults: function(){
            $(elementIDs.exitValidateLogGridJQ).jqGrid('GridUnload');
        },
        isExitValidateInProgress: function (formInstanceId) {
            var inProcess = false;
            var URL = URLs.isExitValidateInProgress + "?formInstanceId=" + formInstanceId + "&folderVersionId=" + formInstanceData.folderVersionId;
            var promise = ajaxWrapper.getJSONSync(URL);
            //register ajax success callback
            promise.done(function (result) {
                inProcess = result;
            });
            return inProcess;
        }, 
        applyValidation: function () {
            if (rowID != undefined) {
                applyValidation();
            }
        },
        folderInstance: undefined
    }

    function exitValidateFormatter(cellvalue, options, rowObject) {
        if (cellvalue == "Yes" || cellvalue == "True" || cellvalue == "Y-True" || cellvalue == true) {
            result = "<input type='checkbox' checked='checked' id='cbSection_" + options.rowId + "' name= '" + rowObject.Section + "' />";
        }
        else {
            var isChecked = undefined;
            if (defaultSections != undefined) {
                isChecked = defaultSections.includes(rowObject.Section) ? true : false;
            }
            if (isChecked == true && isSetAsDefault == true) {
                result = "<input type='checkbox' checked='checked' id='cbSection_" + options.rowId + "' name= '" + rowObject.Section + "' />";
            }
            else {
                result = "<input type='checkbox' id='cbSection_" + options.rowId + "' name= '" + rowObject.Section + "' />";
            }
        }
        return result;
    }


}();

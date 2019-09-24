var searchParameterNameList = [];

var serviceParameterDialog = function (serviceDesingId, serviceDesignName, serviceDesignVersionId, isFinalized, formDesignId, formDesignVersionId) {
    this.serviceDesignId = serviceDesingId;
    this.serviceDesignName = serviceDesignName;
    this.serviceDesignVersionId = serviceDesignVersionId;
    this.isServiceVersionFinalized = isFinalized;
    this.formDesignId = formDesignId;
    this.formDesignVersionId = formDesignVersionId;
    

    this.URLs = {
        getServiceParameterList: '/ServiceDefinition/GetServiceParameterList?tenantId=1&serviceDesignId={serviceDesignId}&serviceDesignVersionId={serviceDesignVersionId}',
        deleteServiceParameter: '/ServiceDefinition/DeleteServiceParameter'
    }

    this.elementIDs = {
        serviceParameterDialogJQ: '#serviceparameterdialog',
        serviceParameterDialogGridJQ: '#serviceparameterdialoggrid',
        serviceParameterDialogGrid: 'serviceparameterdialoggrid',
    };

    var currentInstance = this;

    function init() {
        $(currentInstance.elementIDs.serviceParameterDialogJQ).dialog({
            autoOpen: false,
            height: '550',
            width: '70%',
            modal: true,
            title: currentInstance.serviceDesignName + ' - Web API Parameters'
        });
    }

    init();

    return {
        show: function () {
            $(currentInstance.elementIDs.serviceParameterDialogJQ).dialog("open");
            currentInstance.loadServiceParameterGrid();
        },
        loadServiceParameter: function () {
            loadServicePrameterGrid();
        }
    }
};

serviceParameterDialog.prototype.loadServiceParameterGrid = function () {
    var currentInstance = this;

    //set column list
    var colArray = ['ServiceParameterID', 'Parameter Name', 'DataTypeID', 'DataType', '', 'Is Required'];

    //set column models
    var colModel = [];
    colModel.push({ name: 'ServiceParameterID', index: 'ServiceParameterID', key: true, hidden: true, search: false, });
    colModel.push({ name: 'Name', index: 'Name', align: 'left', editable: false, sortable: false });
    colModel.push({ name: 'DataTypeID', index: 'DataTypeID', hidden: true, editable: false, sortable: false });
    colModel.push({ name: 'DataTypeName', index: 'DataTypeName', align: 'left', editable: false, sortable: false });
    colModel.push({ name: 'UIElementFullPath', index: 'UIElementFullPath', align: 'left', hidden: true, editable: false, sortable: false });
    colModel.push({
        name: 'IsRequired', index: 'IsRequired', width: '30', align: 'center', editable: false, edittype: 'checkbox',
        formatter: chkValueImageFmatter, editoptions: { value: 'true:false' },
        unformat: chkValueImageUnFormat
    });

    //get URL for grid
    var url = currentInstance.URLs.getServiceParameterList.replace(/\{serviceDesignVersionId\}/g, currentInstance.serviceDesignVersionId).replace(/\{serviceDesignId\}/g, currentInstance.serviceDesignId);
    //unload previous grid values
    $(currentInstance.elementIDs.serviceParameterDialogGridJQ).jqGrid('GridUnload');

    $(currentInstance.elementIDs.serviceParameterDialogGridJQ).parent().append("<div id='p" + currentInstance.elementIDs.serviceParameterDialogGrid + "'></div>");

    //load grid
    $(currentInstance.elementIDs.serviceParameterDialogGridJQ).jqGrid({
        url: url,
        datatype: 'json',
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Parameter List - ' + currentInstance.serviceDesignName,
        pager: '#p' + currentInstance.elementIDs.serviceParameterDialogGrid,
        height: '350',
        rowNum: 10000,
        loadonce: true,
        autowidth: true,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        altclass: 'alternate',
        gridComplete: function () {
            var rows = $(this).getDataIDs();
            var rowId;
            if (rows.length > 0) {
                rowId = rows[0];
                $(this).jqGrid('setSelection', rowId);
                var i = 0;
                var j = 0;
                for (i = 0; i < rows.length; i++) {
                    var rowcurrId = rows[i];
                    var rowData = $(this).getRowData(rowcurrId);
                    if (rowData.Name == "FormInstanceID") {
                        j = 'Present';
                    }
                }
                if (j == 'Present') {
                    $('#btnServiceParameterAddDefaultFormInstance').addClass('ui-state-disabled');
                }
                else {
                    $('#btnServiceParameterAddDefaultFormInstance').removeClass('ui-state-disabled');
                }


            }
            if (rows.length == 0) {
                $('#btnServiceParameterEdit').addClass('ui-state-disabled');
                $('#btnServiceParameterDelete').addClass('ui-state-disabled');
            }

            searchParameterNameList = $(currentInstance.elementIDs.serviceParameterDialogGridJQ).jqGrid("getCol", "Name");
        },
        onSelectRow: function (id, e) {
            if (id) {
                var rowData = $(currentInstance.elementIDs.serviceParameterDialogGridJQ).jqGrid('getRowData', id);
                if (rowData) {
                    if (rowData.Name === "FormInstanceID") {
                        $('#btnServiceParameterEdit').addClass('ui-state-disabled');
                    }
                    else {
                        $('#btnServiceParameterEdit').removeClass('ui-state-disabled');
                    }
                }
            }
        }
    });

    var pagerElement = '#p' + currentInstance.elementIDs.serviceParameterDialogGrid;
    //remove default buttons
    $(currentInstance.elementIDs.serviceParameterDialogGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    //remove paging
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    if (currentInstance.isServiceVersionFinalized == 'false') {
        $(currentInstance.elementIDs.serviceParameterDialogGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-plus',
            title: 'Add', id: 'btnServiceParameterAdd',
            onClickButton: function () {
                addEditServiceParameterDialog.show('add', row, currentInstance, currentInstance.serviceDesignId, currentInstance.serviceDesignVersionId);
            }
        });
        $(currentInstance.elementIDs.serviceParameterDialogGridJQ).jqGrid('navButtonAdd', pagerElement,
       {
           caption: '', buttonicon: 'ui-icon-key',
           title: 'Key', id: 'btnServiceParameterAddDefaultFormInstance',
           onClickButton: function () {
               var serviceParameter = {
                   tenantId: 1,
                   serviceDesignId: currentInstance.serviceDesignId,
                   serviceDesignVersionId: currentInstance.serviceDesignVersionId,
                   serviceParameterId: 0,
                   name: 'FormInstanceID',
                   dataTypeId: 1, // 1 in integer data type in database
                   isRequired: true,
               };

               var rows = $(this).getDataIDs();

               var promise = ajaxWrapper.postJSON('/ServiceDefinition/AddDefaultFormInstanceIDServiceParameter', serviceParameter);
               //register success callback
               promise.done(function (xhr) {
                   if (xhr.Result === ServiceResult.SUCCESS) {
                       messageDialog.show(ServiceDesign.parameterAddedSuccessfullyMsg);
                   }
                   else {
                       messageDialog.show(Common.errorMsg);
                   }

                   //close the dialog - jqueryui dialog used
                   currentInstance.loadServiceParameterGrid();
               });
               //register failure callback
               promise.fail(showError);


           }
       });
        $(currentInstance.elementIDs.serviceParameterDialogGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-pencil',
            title: 'Edit', id: 'btnServiceParameterEdit',
            onClickButton: function () {
                var rowId = $(currentInstance.elementIDs.serviceParameterDialogGridJQ).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    var row = $(currentInstance.elementIDs.serviceParameterDialogGridJQ).getRowData(rowId);
                    //load Document Design dialog for edit on click - see serviceDesignDialog function below
                    addEditServiceParameterDialog.show('edit', row, currentInstance, currentInstance.serviceDesignId, currentInstance.serviceDesignVersionId);
                }
                else {
                    messageDialog.show('Please select a row to edit');
                }
            }
        });
        $(currentInstance.elementIDs.serviceParameterDialogGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-trash',
            title: 'Delete', id: 'btnServiceParameterDelete',
            onClickButton: function () {
                var rowId = $(currentInstance.elementIDs.serviceParameterDialogGridJQ).getGridParam('selrow');
                if (rowId !== undefined && rowId !== null) {
                    var row = $(currentInstance.elementIDs.serviceParameterDialogGridJQ).getRowData(rowId);
                    //load confirm dialogue to asset the operation
                    confirmDialog.show(Common.deleteConfirmationMsg, function () {
                        confirmDialog.hide();

                        //delete the service design
                        var serviceParameterToDelete = {
                            tenantId: 1,
                            serviceParameterId: row.ServiceParameterID,
                            serviceDesignId: currentInstance.serviceDesignId,
                            serviceDesignVersionId: currentInstance.serviceDesignVersionId,
                        };
                        var promise = ajaxWrapper.postJSON(currentInstance.URLs.deleteServiceParameter, serviceParameterToDelete);
                        //register ajax success callback
                        promise.done(function (xhr) {
                            if (xhr.Result === ServiceResult.SUCCESS) {
                                messageDialog.show(ServiceDesign.parameterDeletedSuccessfullyMsg);
                            }
                            else {
                                messageDialog.show(Common.errorMsg);
                            }
                            currentInstance.loadServiceParameterGrid();
                        });
                        //register ajax failure callback
                        promise.fail(function (xhr) {
                            if (xhr.status == 999)
                                this.location = '/Account/LogOn';
                            else
                                messageDialog.show(JSON.stringify(xhr));
                        });
                    });
                }
                else {
                    messageDialog.show('Please select a row to edit');
                }
            }
        });
    }

    function chkValueImageFmatter(cellvalue, options, rowObject) {
        if (cellvalue == true || cellvalue == "true") {
            return '<span align="center" class="ui-icon ui-icon-check" style="margin:auto" ></span>';
        }
        else {
            return '<span align="center" class="ui-icon ui-icon-close" style="margin:auto" ></span>';
        }
    }

    function chkValueImageUnFormat(cellvalue, options, cell) {
        var checked = $(cell).children('span').attr('class');
        if (checked == "ui-icon ui-icon-check")
            return 'true';
        else
            return 'false';
    }

    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
}

var addEditServiceParameterDialog = function () {
    var URLs = {
        parameterAdd: '/ServiceDefinition/AddServiceParameter',
        parameterUpdate: '/ServiceDefinition/UpdateServiceParameter'
    }

    var elementIDs = {
        serviceParameterDialogJQ: '#serviceparameterdialog',
        serviceParameterDialogGridJQ: '#serviceparameterdialoggrid',
        serviceParameterDialogGrid: 'serviceparameterdialoggrid',
        manageServiceParameterDialogJQ: '#manageserviceparameterdialog',
        paramternameJQ: '#parametername',
        paramternameHelpBlockJQ: '#parameternameHelpBlock',
        parameterDataTypeJQ: '#paramterdatatype',
        parameterDataTypeHelpBlockJQ: '#paramterdatatypeHelpBlock',
        parameterIsRequiredJQ: '#parameterisrequired',
        parameterIsRequiredHelpBlockJQ: '#parameterisrequiredHelpBlock',
        parameteruielementfullpathJQ: "#parameteruielementfullpath",
        parameteruielementfullpathHelpBlockJQ: "#parameteruielementfullpathHelpBlock",
        parameterselectelementBtnJQ: "#parameterselectelement",
        btnsaveserviceparameterJQ: '#btnsaveserviceparameter',
    };

    var dialogState;
    var serviceParameterDialogInstance;
    var serviceParameterId;
    var serviceDesignId;
    var serviceDesignVersionId;
    var elementid;
    var fullPath;

    function init() {
        $(elementIDs.manageServiceParameterDialogJQ).dialog({
            autoOpen: false,
            height: 380,
            width: 380,
            modal: true
        });

        $(elementIDs.parameterDataTypeJQ).empty();
        $(elementIDs.parameterDataTypeJQ).append('<option value="0">Select</option>')
                                            .append('<option value="1">Integer</option>')
                                            .append('<option value="2">String</option>')
                                            .append('<option value="5">Float</option>')
                                            .append('<option value="3">Date</option>');

        $(elementIDs.parameterDataTypeJQ).val(0);

        //register event for button click to add 
        $(elementIDs.btnsaveserviceparameterJQ).on('click', function () {
            var name = $(elementIDs.paramternameJQ).val();
            var dataTypeId = $(elementIDs.parameterDataTypeJQ).val();
            var isRequired = $(elementIDs.parameterIsRequiredJQ).is(":checked");

            if (validate(serviceParameterId, name, dataTypeId, isRequired, elementid)) {
                save(serviceParameterId, name, dataTypeId, isRequired, elementid);
            }
        });
    }

    function validate(serviceParameterId, name, dataTypeId, isRequired, uielementId) {
        var isValid = false;
        if (name === "FormInstanceID") {
            if (uielementId == 0 || uielementId == '' || uielementId == null) {
                isValid = false;
                $(elementIDs.parameteruielementfullpathJQ).parent('div').addClass('has-error');
                $(elementIDs.parameteruielementfullpathHelpBlockJQ).text("Data type is required.");

                return isValid;
            }
            else {
                isValid = true;
                $(elementIDs.parameteruielementfullpathJQ).parent('div').removeClass('has-error');
                $(elementIDs.parameteruielementfullpathHelpBlockJQ).text('Please select the element.');
            }
        }
        if (name == '' || name == undefined || name == null) {
            isValid = false;
            $(elementIDs.paramternameJQ).parent('div').addClass('has-error');
            $(elementIDs.paramternameJQHelpBlockJQ).text("Parameter Name is required.");
            return isValid;
        }
        else {
            isValid = true;
            $(elementIDs.paramternameJQ).parent('div').removeClass('has-error');
            $(elementIDs.paramternameHelpBlockJQ).text(ServiceDesign.parameterNameHelpBlockMsg);
        }
        if (dataTypeId == 0 || dataTypeId == '' || dataTypeId == null) {
            isValid = false;
            $(elementIDs.parameterDataTypeJQ).parent('div').addClass('has-error');
            $(elementIDs.parameterDataTypeHelpBlockJQ).text("Data type is required.");

            return isValid;
        }
        else {
            isValid = true;
            $(elementIDs.parameterDataTypeJQ).parent('div').removeClass('has-error');
            $(elementIDs.parameterDataTypeHelpBlockJQ).text(ServiceDesign.parameterDataTypeHelpBlockMsg);
        }

        return isValid;
    }

    function save(serviceParameterId, name, dataTypeId, isRequired, uielementid) {
        var serviceParameter = {
            tenantId: 1,
            serviceDesignId: serviceDesignId,
            serviceDesignVersionId: serviceDesignVersionId,
            serviceParameterId: serviceParameterId,
            name: name,
            dataTypeId: dataTypeId,
            isRequired: isRequired,
            uielementId: uielementid,
        };
        var url;
        //add or edit based on the current mode the dialog is opened in
        if (dialogState === 'add') {
            url = URLs.parameterAdd;
        }
        else {
            url = URLs.parameterUpdate;
        }
        //ajax call for post
        var promise = ajaxWrapper.postJSON(url, serviceParameter);
        //register success callback
        promise.done(function (xhr) {
            if (xhr.Result === ServiceResult.SUCCESS) {
                if (dialogState === 'edit')
                    messageDialog.show(ServiceDesign.parameterUpdatedSuccessfullyMsg);
                else
                    messageDialog.show(ServiceDesign.parameterAddedSuccessfullyMsg);
            }
            else {
                messageDialog.show(Common.errorMsg);
            }

            //close the dialog - jqueryui dialog used
            $(elementIDs.manageServiceParameterDialogJQ).dialog('close');
            serviceParameterDialogInstance.loadServiceParameterGrid();
        });
        //register failure callback
        promise.fail(showError);
    }

    //ajax error callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    init();

    return {
        show: function (action, row, instance, currentServiceDesignId, currentServiceDesignVersionId) {
            dialogState = action;
            serviceParameterDialogInstance = instance;
            serviceDesignId = currentServiceDesignId;
            serviceDesignVersionId = currentServiceDesignVersionId;

            $(elementIDs.serviceDesignDialog + ' div').removeClass('has-error');
            if (dialogState == 'add') {
                $(elementIDs.manageServiceParameterDialogJQ).dialog('option', 'title', "Add Web API Parameter");

                $(elementIDs.paramternameJQ).val('');
                $(elementIDs.parameterDataTypeJQ).val(0);
                $(elementIDs.parameterIsRequiredJQ).removeAttr('checked', 'checked');
                $(elementIDs.parameteruielementfullpathJQ).val('');
                serviceParameterId = 0;
            }
            else {
                $(elementIDs.manageServiceParameterDialogJQ).dialog('option', 'title', "Edit Web API Parameter");

                serviceParameterId = row.ServiceParameterID;
                $(elementIDs.paramternameJQ).val(row.Name);
                $(elementIDs.parameterDataTypeJQ).val(row.DataTypeID);
                if (row.IsRequired)
                    $(elementIDs.parameterIsRequiredJQ).attr('checked', 'checked');
                else
                    $(elementIDs.parameterIsRequiredJQ).removeAttr('checked', 'checked');
                $(elementIDs.parameteruielementfullpathJQ).val(row.UIElementFullPath);

                if (row.Name === "FormInstanceID") {
                    $(elementIDs.paramternameJQ).attr('disabled', 'disabled');
                    $(elementIDs.paramternameJQ).addClass('disabled');
                }
                else {
                    $(elementIDs.paramternameJQ).removeAttr('disabled', 'disabled');
                    $(elementIDs.paramternameJQ).removeClass('disabled');
                }
            }
            $(elementIDs.paramternameJQ).next(' .help-block').text(ServiceDesign.parameterNameHelpBlockMsg);
            $(elementIDs.parameterDataTypeJQ).next(' .help-block').text(ServiceDesign.parameterDataTypeHelpBlockMsg);
            $(elementIDs.parameterIsRequiredJQ).next(' .help-block').text(ServiceDesign.parameterIsRequiredHelpBlockMsg);
            $(elementIDs.manageServiceParameterDialogJQ).dialog('open');

            $(elementIDs.parameterselectelementBtnJQ).off('click');
            $(elementIDs.parameterselectelementBtnJQ).on('click', function () {
                var dialog = new serviceParameterUIelementDialog(instance.formDesignId, instance.formDesignVersionId);
                dialog.show(serviceDesignId, serviceDesignVersionId, instance.isFinalized, instance.formDesignId, instance.formDesignVersionId);
            });
        },
        setUIElement: function (elemid, fullpath, label) {            

            var filterList = searchParameterNameList.filter(function (ct) {
                return compareStrings(ct, label, false, false);
            });

            if (filterList.length > 0)
            {
                messageDialog.show(ServiceDesign.searchParameterexistMsg);
            }
            else
            {
                elementid = elemid;
                fullPath = fullpath;
                $(elementIDs.parameteruielementfullpathJQ).val(fullpath ? fullpath : '');
                $(elementIDs.paramternameJQ).val(label ? label : '');
            }
        }
    }
}();

var serviceParameterUIelementDialog = function (formDesignId, formDesignVersionId) {

    this.formDesignId = formDesignId;
    this.formDesignVersionId = formDesignVersionId;
    this.data = [];

    var Urls = {
        getUIelementList: "/ServiceDefinition/GetUIElementList?tenantId=1&formDesignVersionId={formDesignVersionID}&formDesignId={formDesignID}"
    };

    var elementIDs = {
        selectUIElementJQ: "#selectUIElementServiceParameter",
        uielementGrid: "serviceparameteruielementgrid",
        uielementGridJQ: "#serviceparameteruielementgrid",
        selectuielementserviceparameterdialog: 'selectuielementserviceparameterdialog',
        selectuielementserviceparameterdialogJQ: '#selectuielementserviceparameterdialog',
        selectuielementJQ: '#selectuielement',
        serviceparameteruielementgridcontainerJQ: '#serviceparameteruielementgridcontainer',
    };

    function init() {
        $(elementIDs.selectuielementserviceparameterdialogJQ).dialog({
            autoOpen: false,
            height: 450,
            width: 580,
            modal: true,
            title: 'Select Element',
            close: function (event, ui) {
                destroy();
            }
        });
    }

    function destroy() {
        $(elementIDs.selectuielementserviceparameterdialogJQ).dialog('destroy');
        $(elementIDs.serviceparameteruielementgridcontainerJQ).empty();
        $(elementIDs.serviceparameteruielementgridcontainerJQ).append('<table id="serviceparameteruielementgrid"></table>');
        $(elementIDs.uielementGridJQ).jqGrid('GridUnload');
    }

    function loadGrid(formDesignId, formDesignVersionId) {
        var lastSel;
        //set column list
        var colArray = ['', 'Field Name', 'Element Type', 'Full Path'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'UIElementID', index: 'UIElementID', key: true, hidden: true, search: false, });
        colModel.push({ name: 'Label', index: 'Label', align: 'left', width: '100', editable: false, sortable: false, searchable: true });
        colModel.push({ name: 'ElementType', index: 'ElementType', align: 'left', width: '60', editable: false, sortable: false, searchable: true });
        colModel.push({ name: 'UIelementFullPath', index: 'UIelementFullPath', align: 'left', editable: false, sortable: false, searchable: true });

        //get URL for grid
        var fieldListUrl = Urls.getUIelementList.replace(/\{formDesignVersionID\}/g, formDesignVersionId).replace(/\{formDesignID\}/g, formDesignId);
        //unload previous grid values
        $(elementIDs.uielementGridJQ).jqGrid('GridUnload');

        $(elementIDs.uielementGridJQ).parent().append("<div id='p" + elementIDs.uielementGrid + "'></div>");

        //load grid
        $(elementIDs.uielementGridJQ).jqGrid({
            url: fieldListUrl,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Field List',
            pager: '#p' + elementIDs.uielementGrid,
            height: '300',
            rowNum: 10000,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            altRows: true,
            altclass: 'alternate',
            gridComplete: function () {

            },
            onSelectRow: function (id) {
                if (id && id !== lastSel) {

                }
            }
        });

        var pagerElement = '#p' + elementIDs.uielementGrid;
        //remove default buttons
        $(elementIDs.uielementGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();

        $(elementIDs.uielementGridJQ).jqGrid('navButtonAdd', pagerElement,
        {
            caption: '', buttonicon: 'ui-icon-check', title: 'Add', id: 'selectuielement',
            onClickButton: function () {
                var gridrowId = $(elementIDs.uielementGridJQ).jqGrid('getGridParam', 'selrow');
                if (gridrowId) {
                    var gridData = $(elementIDs.uielementGridJQ).jqGrid('getRowData', gridrowId);
                    if (gridData) {
                        addEditServiceParameterDialog.setUIElement(gridData.UIElementID, gridData.UIelementFullPath, gridData.Label);
                        $(elementIDs.selectuielementserviceparameterdialogJQ).dialog('close');
                    }
                }
            }
        });
    }

    init();
    return {
        show: function (serviceDesignId, serviceDesignVersionID, isFinalized, formDesignId, formDesignVersionId) {
            $(elementIDs.selectuielementserviceparameterdialogJQ).dialog('open');
            loadGrid(formDesignId, formDesignVersionId);
        }
    }
};
var prefixReuse = function () {
    var elementIDs = {
        productQueueGridJQ: "#productQueueGrid",
        createProductQueueJQ: "#createProductQueueGrid",
        confirmProductList: "#confirmProductList",
        cancelProductList: "#cancelProductList",
        productGridJQ: "#productGrid"
    };

    var URLs = {
        getProductQueueList: '/Translator/GetProductDetailsForTranslation?tenantID={tenantID}&formInstanceIDList={formInstanceIDList}&state={state}'
    }

    function fillConfirmationPopupData(tenantID, formInstanceIDList, state, updateWorkflowData,currentFormInstanceID) {
        //set column list for grid
        var colArray = ['FormInstance ID', 'Product Name', 'Product ID', 'PDBC Type', 'Create New Prefix', 'Benefit Set Name', 'Create SEPY Prefix', 'BenefitSetCount'];

        //set column models
        var colModel = [];
        colModel.push({ key: true, name: 'FormInstanceID', index: 'FormInstanceID', align: 'center', hidden: true });
        colModel.push({ key: false, name: 'FormInstanceName', index: 'FormInstanceName', align: 'center', width: 90 })
        colModel.push({ key: false, name: 'ProductName', index: 'Product', header: 'Product Id', align: 'center', hidden: false, width: 90 });
        colModel.push({ key: false, name: 'PDBCType', index: 'PDBCType', align: 'center', width: 90 });
        colModel.push({ key: false, name: 'CreateNewPrefix', index: 'CreateNewPrefix', align: 'center', formatter: chkValueImageFmatter, width: 110 });
        colModel.push({ key: false, name: 'BenefitSetName', index: 'BenefitSetName', align: 'left', width: 260 });
        colModel.push({ key: false, name: 'CreateSEPYPrefix', index: 'CreateSEPYPrefix', align: 'center', formatter: chkValueImageFmatter, width: 110 });
        colModel.push({ key: false, name: 'BenefitSetCount', index: 'BenefitSetCount', align: 'center', hidden: true });

        //clean up the grid first - only table element remains after this
        $(elementIDs.productQueueGridJQ).jqGrid('GridUnload');

        jQuery.ajaxSettings.traditional = true;

        $(elementIDs.productQueueGridJQ).jqGrid({
            url: URLs.getProductQueueList.replace(/\{tenantID\}/g, parseInt(tenantID)).replace(/\{formInstanceIDList\}/g, formInstanceIDList).replace(/\{state\}/g, state),
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Product Details',
            rowNum: 10000,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: false,
            hiddengrid: false,
            width: '100%',
            grouping: true,
            groupingView: {
                groupField: ['FormInstanceName'],
                groupCollapse: false
            },
            altRows: true,
            shrinkToFit: false,
            pager: '#p' + elementIDs.productQueueGridJQ,
            altclass: 'alternate',
            gridComplete: function () {
                var count = $(elementIDs.productQueueGridJQ).jqGrid('getGridParam', 'records');
                if (count > 0) {
                    $(elementIDs.createProductQueueJQ).dialog('option', 'title', 'Prefix Reuse Check-box Pop-up')
                    $(elementIDs.createProductQueueJQ).dialog("open");
                }
                else {
                    if (state == 0)
                        QueueProductsForTranslation();
                    else if (state == 1)
                        updateWorkflowStatus(updateWorkflowData, formInstanceIDList[0], state);
                    else {
                    }
                }
            }
        });

        $(elementIDs.productQueueGridJQ).jqGrid('setGroupHeaders', {
            useColSpanStyle: true,
            groupHeaders: [
              { startColumnName: 'PDBCType', numberOfColumns: 2, titleText: 'PDBCPrefixList' },
              { startColumnName: 'BenefitSetName', numberOfColumns: 2, titleText: 'NetworkList' }
            ]
        });
        var pagerElement = '#p' + elementIDs.productQueueGridJQ;
        $('#p' + elementIDs.productQueueGridJQ).find('input').css('height', '20px');
        //remove default buttons
        $(elementIDs.productQueueGridJQ).jqGrid('navGrid', pagerElement, { edit: true, add: true, del: true, search: false, refresh: true });
        //remove paging
        $(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find(pagerElement + '_right').remove();

        $(elementIDs.confirmProductList).off('click');
        $(elementIDs.confirmProductList).on('click', function () {
            if (state == 0)
                QueueProductsForTranslation();
            if (state == 1)
                updateWorkflowStatus(updateWorkflowData, currentFormInstanceID, 0);
        });

        $(elementIDs.cancelProductList).off('click');
        $(elementIDs.cancelProductList).on('click', function () {
            $(elementIDs.createProductQueueJQ).dialog('close');
            $(elementIDs.productQueueGridJQ).jqGrid('GridUnload');
            $(elementIDs.productGridJQ).jqGrid('GridUnload');
        });
    }

    function QueueProductsForTranslation() {
        var productList = "";
        productList = $(elementIDs.productGridJQ).jqGrid('getGridParam', 'data');
        productList = productList.filter(function (ct) {
            return ct.IsIncluded == true && ct.IsTranslated == false;
        });
        if (productList != null) {
            var rowListlen = productList.length;
            var productRows = new Array(rowListlen);
        }
        var folderInstance = folderManager.getInstance().getFolderInstance();
        if (productList.length > 0) {
            for (i = 0; i < productList.length; i++) {
                var row = new Object();
                var currentFormInstaceBuilder = folderInstance.formInstances[productList[i].FormInstanceID].FormInstance;
                row.ID = productList[i].FormInstanceID;
                row.Product = productList[i].ID;
                row.FolderVersionNumber = productList[i].FolderVersionNumber;
                row.FormInstanceName = productList[i].ProductName;
                row.FolderName = productList[i].FolderName;
                row.FormDesignID = currentFormInstaceBuilder.FormDesignID;
                row.FormDesignVersionID = currentFormInstaceBuilder.FormDesignVersionID;
                row.ConsortiumID = productList[i].ConsortiumID;
                row.AccountID = folderData.accountId;
                row.FormInstanceID = productList[i].FormInstanceID;
                row.EffectiveDate = productList[i].EffectiveDate;
                row.AccountName = productList[i].AccountName;
                row.ConsortiumName = productList[i].ConsortiumName;
                productRows[i] = row;
            }
            $(elementIDs.createProductQueueJQ).dialog('close');
            $(elementIDs.productQueueGridJQ).jqGrid('GridUnload');
            $(elementIDs.productGridJQ).jqGrid('GridUnload');
            folderStatus.queueProductToPluginVersionProcessQueue(productRows, true);
        }
    }

    function updateWorkflowStatus(updateWorkflowData, formInstanceID, state) {
        var folderInstance = folderManager.getInstance().getFolderInstance();
        var currentFormInstaceBuilder = undefined;
        if (folderInstance.formInstances[formInstanceID] != undefined) {
            currentFormInstaceBuilder = folderInstance.formInstances[formInstanceID].FormInstanceBuilder;
            var callbackMetaDeta = {
                callback: folderStatus.validateAndUpdateWorkFlowData,
                args: [updateWorkflowData, folderInstance]
            };
            //check form Validation & duplication   
            currentFormInstaceBuilder.validation.validateFormInstance(true, callbackMetaDeta);
            if (state !== 1) {
                $(elementIDs.createProductQueueJQ).dialog('close');
                $(elementIDs.productQueueGridJQ).jqGrid('GridUnload');
                $(elementIDs.productGridJQ).jqGrid('GridUnload');
            }
        }        
    }

    function chkValueImageFmatter(cellvalue, options, rowObject) {
        if (cellvalue == "Yes" || cellvalue == "True")
            return '<span align="center" class="ui-icon ui-icon-check" style="margin:auto" ></span>';
        else if (cellvalue == "No" || cellvalue == "False")
            return '<span align="center" class="ui-icon ui-icon-close" style="margin:auto" ></span>';
        else
            return '<span align="center"></span>';
    }

    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    function init() {
        $(elementIDs.createProductQueueJQ).dialog({
            autoOpen: false,
            width: 800,
            modal: true
        });
    }

    $(document).ready(function () {
        //initialize the dialog after this js are loaded
        init();
    });

    return {
        fillConfirmationPopupData: function (tenantID, formInstanceIDList, state, updateWorkflowData, currentFormInstanceID) {
            fillConfirmationPopupData(tenantID, formInstanceIDList, state, updateWorkflowData, currentFormInstanceID);
        },
        QueueProductsForTranslation: function () {
            QueueProductsForTranslation();
        },
        updateWorkflowStatus: function (updateWorkflowData, formInstanceID, state) {
            updateWorkflowStatus(updateWorkflowData, formInstanceID, state);
        }
    }
}();

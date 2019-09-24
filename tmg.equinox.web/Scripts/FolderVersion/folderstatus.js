var folderStatus = function () {
    this.workflowData = undefined;
    this.btnStatusUpdateJQ = "#statusupdatebutton";
    //urls to be accessed  
    var URLs = {
        //Get WorkFlowState List        
        workFlowStateList: '/FolderVersionWorkFlow/GetCurrentWorkFlowState?tenantId=1&folderVersionId={folderVersionId}',
        //Get ApprovalStatus List        
        approvalStatusTypeList: '/FolderVersion/GetApprovalStatusTypeList?tenantId=1&folderVersionId={folderVersionId}&wfVersionStateId={wfVersionStateId}',

        updateWorkflowStatus: '/FolderVersionWorkFlow/UpdateWorkflowStatus',
        //Get TeamManager
        folderVersionIndex: '/FolderVersion/Index?tenantId=1&folderVersionId={folderVersionId}&folderId={folderId}&foldeViewMode={foldeViewMode}&mode={mode}',
        getProductList: '/Translator/GetProductList?folderversionId={folderversionId}',
        addMasterListProductToTranslate: '/Translator/AddMasterListToTranslateTransmit?folderversionId={folderversionId}',
        addProductsToTranslate: '/Translator/AddProductToTranslate',
        isAllJournalEntryIsClosed: '/JournalReport/IsAllJournalEntryIsClosed?tenantId={tenantId}&folderVersionId={folderVersionId}',
        getFormInstaceIDList: '/FolderVersion/getFormInstanceIDList?tenantId={tenantId}&folderVersionId={folderVersionId}',
        getAccelaratedMessage: '/FolderVersion/GetAcceleratedConfirmationMsg?wfversionstateId={wfversionstateId}',
        validateTaskCompleted: '/PlanTaskUserMapping/ValidateTaskCompletedForWorkFlow?FolderVersionID={FolderVersionID}&WorkFlowVersionStateID={WorkFlowVersionStateID}',
        validateExitValidationCompleted: '/ExitValidate/ValidateExitValidationForFolderversion?FolderVersionID={FolderVersionID}'
    };

    //element ID's required 
    //added in Views/FolderVersion/Index.cshtml
    var elementIDs = {
        //for dialog for status update
        statusUpdateButton: '#statusupdatebutton',
        statusUpdateDialogue: "#statusUpdateDialogue",
        workFlowStateListId: "#workflowstatelist",
        approvalTypeListId: "#approvaltypelist",
        ownerListId: "#ownerlist",
        comments: "#comment",
        doneButtonID: "#updatestatus",
        workflowstatediv: "#workflowstatediv",
        approvaldiv: "#approvaldiv",
        ownerdiv: "#ownerdiv",
        btnQueueFacetsTranslationJQ: "#facetsTranslate",
        btnQueueFacetsTransmission: "#facetsTransmit",
        createProductJQ: "#createProductGrid",
        btnCreateFormJQ: "#btnCreateForm",
        commentsSpan: "#commentSpan"
    };

    function getWorkFlowStateList(folderVersionId) {
        //ajax call to add/update
        var url = URLs.workFlowStateList.replace(/\{folderVersionId\}/g, folderVersionId);
        var promise = ajaxWrapper.getJSON(url);
        //register ajax success callback
        promise.done(loadworkflowlist);
        //register ajax failure callback
        promise.fail(showError);
    }

    function getApprovalStatusTypeList(folderVersionId, wfVersionStateId) {
        //ajax call to add/update
        wfVersionStateId = wfVersionStateId == undefined ? 0 : wfVersionStateId;

        var url = URLs.approvalStatusTypeList.replace(/\{folderVersionId\}/g, folderVersionId).replace(/\{wfVersionStateId\}/g, wfVersionStateId);
        var promise = ajaxWrapper.getJSONSync(url);
        //register ajax success callback
        promise.done(loadapprovallist);
        //register ajax failure callback
        promise.fail(showError);
    }

    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    function loadworkflowlist(data) {
        $(elementIDs.workFlowStateListId).empty();
        for (var i = 0; i < data.length; i++) {
            $(elementIDs.workFlowStateListId).append("<option value=" + data[i].Key + ">" + data[i].Value + "</option>");
        }
        var wfVersionStateId = parseInt($(elementIDs.workFlowStateListId).val());
        getApprovalStatusTypeList(folderData.folderVersionId, wfVersionStateId);
    }

    function loadapprovallist(data) {
        var len = data.length;

        $(elementIDs.approvalTypeListId).empty();
        for (var i = 0; i < len; i++) {
            $(elementIDs.approvalTypeListId).append("<option value=" + data[i].Key + ">" + data[i].Value + "</option>");
        }
        var workflowstate = parseInt($(elementIDs.workFlowStateListId).val());
        var approvalstatus = parseInt($(elementIDs.approvalTypeListId).val());
    }

    function workflowSuccess(xhr) {
        if (xhr.Result === ServiceResult.SUCCESS) {

            $(elementIDs.statusUpdateDialogue).dialog("close");
            if (xhr.Items[0].Messages[1] == 'Baseline') {
                var url = URLs.folderVersionIndex.replace(/\{folderVersionId\}/g, parseInt(xhr.Items[0].Messages[0])).replace(/\{folderId\}/g, folderData.folderId).replace(/{foldeViewMode}/g, folderData.FolderViewMode).replace(/\{mode\}/g, true);
                window.location.href = url;
            }
            else if (xhr.Items[0].Messages[1] === 'Release') {
                var url = URLs.folderVersionIndex.replace(/\{folderVersionId\}/g, parseInt(xhr.Items[0].Messages[0])).replace(/\{folderId\}/g, folderData.folderId).replace(/{foldeViewMode}/g, folderData.FolderViewMode).replace(/\{mode\}/g, false);
                window.location.href = url;
            }
            else {
                var folderVersionWorkflow = new folderVersionWorkflowInstance.getInstance(folderData.accountId, folderData.folderVersionId, folderData.folderId);
                folderVersionWorkflow.load();
            }
            //if (xhr.Items[0].Messages[0] === 'false') {

            //        var folderVersionWorkflow = new folderVersionWorkflowInstance.getInstance(folderData.accountId, folderData.folderVersionId, folderData.folderId);
            //        folderVersionWorkflow.load();
            //    }
        }
        //   messageDialog.show(Common.errorMsg);
    }

    function queueProductsToMasterListTranslator(folderversionId, updateWorkflowData) {

        dataToAdd = {
            folderVersionId: folderversionId,
            updateWorkflowData: updateWorkflowData
        };
        var url = URLs.addMasterListProductToTranslate;
        var promise = ajaxWrapper.postJSONCustom(url, dataToAdd);
        promise.done(function (xhr) {
            if (xhr.Result === ServiceResult.FAILURE) {
                messageDialog.show(FolderVersion.productQueueError);
            }
            else if (xhr.Items.length > 0 && xhr.Items[0].Messages[0] == FolderVersion.translatorEmptyMsg) {
                if (folderData.WFStateName == WorkFlowState.FACETSProductbuild || folderData.WFStateName == WorkFlowState.FACETSPROD || folderData.WFStateName == WorkFlowState.FACETSProductReview) {//AP
                    $(elementIDs.statusUpdateDialogue).dialog("close");
                    if (updateWorkFlowData != undefined) {
                        postWorkFlowData(updateWorkFlowData); // post workflow data(release folder) if transmitter list is empty.
                    }
                    else
                        window.location.reload();
                }
            }
            else
                productQueuedDialog.show(FolderVersion.masterListQueuedMsg);
        });
    }

    function queueProductsToTranslator(folderVersionId, updateWorkflowData) {
        if (folderData.folderType == 'MasterList') {
            queueProductsToMasterListTranslator(folderVersionId, updateWorkflowData);
        }
        else {
            var url = URLs.getProductList.replace(/\{folderversionId\}/g, parseInt(folderVersionId));
            var promise = ajaxWrapper.getJSON(url);
            //register ajax success callback
            promise.done(function (xhr) {
                var productsToBeQueued = xhr;
                var folderInstance = folderManager.getInstance().getFolderInstance();
                var rowListlen = productsToBeQueued.length;
                if (rowListlen > 0) {
                    var productRows = new Array(rowListlen);
                    for (i = 0; i < productsToBeQueued.length; i++) {
                        var row = new Object();
                        //var currentFormInstaceBuilder = folderInstance.formInstances[productsToBeQueued[i].Id].FormInstanceBuilder;
                        row.FormInstanceID = productsToBeQueued[i].Id;
                        row.Product = productsToBeQueued[i].Product;
                        row.FolderVersionNumber = productsToBeQueued[i].FolderVersionNumber;
                        row.FormInstanceName = productsToBeQueued[i].FormInstanceName;
                        row.FolderName = productsToBeQueued[i].FolderName;
                        //Passing FormDesignId and FormDesignVersionID as blank as we are not getting this values due all formInstances are not loaded
                        //it will updated on server side 
                        row.FormDesignID = productsToBeQueued[i].FormDesignID;//currentFormInstaceBuilder.designData.FormDesignId;
                        row.FormDesignVersionID = productsToBeQueued[i].FormDesignVersionID;//currentFormInstaceBuilder.designData.FormDesignVersionId;
                        row.ConsortiumID = productsToBeQueued[i].ConsortiumID;
                        row.AccountID = folderData.accountId;
                        row.ID = productsToBeQueued[i].Id;
                        row.AccountName = productsToBeQueued[i].AccountName;
                        row.EffectiveDate = productsToBeQueued[i].EffectiveDate;
                        row.ConsortiumName = productsToBeQueued[i].ConsortiumName;
                        productRows[i] = row;
                    }
                    // here all the products present in the folder version will be queued for translation.
                    queueProductToPluginVersionProcessQueue(productRows, false, updateWorkflowData);
                }
                else {
                    $(elementIDs.statusUpdateDialogue).dialog("close");
                    if (updateWorkflowData != undefined) {
                        postWorkFlowData(updateWorkflowData); // post workflow data(release folder) if folder version has all reference documents.
                    }
                    else
                        window.location.reload();
                }
            });
            //register ajax failure callback
            promise.fail(showError);
        }
    }

    function queueProductToPluginVersionProcessQueue(productSelectionList, isManualTranslation, updateWorkFlowData) {
        if (productSelectionList.length > 0) {
            var folderInstance = folderManager.getInstance().getFolderInstance();
            var currentFormInstaceBuilder = folderInstance.formInstances[folderInstance.currentFormInstanceID].FormInstanceBuilder;
            var rowListlen = productSelectionList.length;
            var productRows = new Array(rowListlen);

            for (var i = 0; i < rowListlen; i++) {
                var productRow = new Object();
                productRow.Id = productSelectionList[i].ID;
                productRow.Plugin = "Facets";
                productRow.Version = "5.2";
                productRow.Product = productSelectionList[i].Product;
                productRow.FolderVersionNumber = productSelectionList[i].FolderVersionNumber;
                productRow.FormInstanceName = productSelectionList[i].FormInstanceName;
                productRow.FolderName = productSelectionList[i].FolderName;
                productRow.FormDesignID = parseInt(productSelectionList[i].FormDesignID);
                productRow.FormDesignVersionID = parseInt(productSelectionList[i].FormDesignVersionID);
                productRow.ConsortiumID = parseInt(productSelectionList[i].ConsortiumID);
                productRow.AccountID = parseInt(productSelectionList[i].AccountID);
                productRow.FormInstanceID = parseInt(productSelectionList[i].FormInstanceID);
                productRow.CurrentUser = folderData.currentUserName;
                productRow.FolderID = parseInt(folderData.folderId);
                productRow.AccountName = productSelectionList[i].AccountName;
                productRow.EffectiveDate = productSelectionList[i].EffectiveDate;
                productRow.ConsortiumName = productSelectionList[i].ConsortiumName;
                productRow.IsTranslated = false;
                productRow.FolderVersionID = parseInt(folderData.folderVersionId);
                productRow.IsRetro = parseInt(folderData.versionType == 2 ? 1 : 0);
                productRows[i] = productRow;
            }
            productsToAdd = {
                translatorModel: productRows,
                isManual: isManualTranslation,
                tenantId: currentFormInstaceBuilder.tenantId,
                folderVersionId: currentFormInstaceBuilder.folderVersionId,
                updateWorkflowData: updateWorkFlowData
            };

            //return productsToAdd;
            var url = URLs.addProductsToTranslate;
            var promise = ajaxWrapper.postJSONCustom(url, productsToAdd);
            promise.done(function (xhr) {
                if (xhr.Result === ServiceResult.FAILURE) {
                    messageDialog.show(FolderVersion.productQueueError);
                }
                else if (folderData.WFStateName == WorkFlowState.FACETSPROD) { //AP
                    if (xhr.Items.length > 0 && xhr.Items[0].Messages[0] == FolderVersion.transmitterEmptyMsg || xhr.Items.length > 0 && xhr.Items[1] != undefined && xhr.Items[1].Messages[0] == FolderVersion.transmitterEmptyMsg) {
                        if (updateWorkFlowData != undefined) {
                            postWorkFlowData(updateWorkFlowData); // post workflow data(to release folder) if transmitter list is empty.
                        }
                    }
                    if (updateWorkFlowData != null) {
                        $(elementIDs.createProductJQ).dialog('close');
                        window.location.reload();
                    }
                }
                else if (xhr.Items.length > 0 && xhr.Items[0].Messages[0] == FolderVersion.translatorEmptyMsg) {
                    if (folderData.WFStateName == WorkFlowState.FACETSProductbuild || folderData.WFStateName == WorkFlowState.FACETSPROD || folderData.WFStateName == WorkFlowState.FACETSProductReview) {//AP
                        $(elementIDs.statusUpdateDialogue).dialog("close");
                        if (updateWorkFlowData != undefined) {
                            postWorkFlowData(updateWorkFlowData); // post workflow data(release folder) if transmitter list is empty.
                        }
                        else {
                            window.location.reload();
                        }
                    }
                }
                else {
                    productQueuedDialog.show(FolderVersion.productQueuedMsg);
                    if (isManualTranslation) {
                        $(elementIDs.createProductJQ).dialog('close');
                    }
                }
            });
            promise.fail(showError);
        }
    }

    function validOwnerData() {
        valid = false;
        if ($(elementIDs.ownerListId)[0].selectedIndex == 0) {
            $(elementIDs.ownerdiv).addClass('has-error');
            $(elementIDs.ownerdiv + ' .help-block').text(FolderVersion.ownerRequiredMsg);
            valid = false;
        } else {
            $(elementIDs.ownerdiv).removeClass('has-error');
            $(elementIDs.ownerdiv + ' .help-block').text('');
            valid = true;
        }
        return valid;
    }

    function ValidateWorkflowData() {
        validOwner = validOwnerData();

        if (validOwner != true) {
            return false;
        }
        return true;
    }

    function ValidateMilestoneChecklistData(checkListFormInstanceBuilder, updateWorkflowData) {
        var callbackMetaData = {
            callback: folderStatus.updateWorkFlowStatus,
            args: [checkListFormInstanceBuilder, updateWorkflowData]
        };
        if (checkListFormInstanceBuilder === undefined || checkListFormInstanceBuilder === null) {
            messageDialog.show(WorkFlowStateMessages.validateMilestoneChecklistDocument);
            return;
        } else
            checkListFormInstanceBuilder.validation.validateFormInstance(false, callbackMetaData);
    }

    function updateWorkFlowStatus(args) {
        if (args == undefined || args == null || args.length == 0) {
            return;
        }
        var isValid;
        var checkListFormInstanceBuilder = args[0];
        var updateWorkflowData = args[1];
        if (checkListFormInstanceBuilder != undefined && checkListFormInstanceBuilder != null && checkListFormInstanceBuilder.errorGridData.length > 0) {
            isValid = false;
        }
        else {
            isValid = true;
        }
        if (isValid) {
            var workflowstate = $(elementIDs.workFlowStateListId).val();
            var approvalstatus = $(elementIDs.approvalTypeListId).val();
            if (workflowstate == WorkFlowState.Development && approvalstatus == ApprovalStatus.APPROVED) {
                var hasChanges = false;
                if (checkListFormInstanceBuilder.hasChanges == true) {
                    hasChanges = true;
                }
                //load confirm dialogue before release save data
                if (hasChanges) {
                    confirmDialog.show(Common.saveConfirmationMsg, function () {
                        confirmDialog.hide();
                        checkListFormInstanceBuilder.form.saveFormInstanceData(true);
                    });
                    return;
                }
            }
            postWorkFlowData(updateWorkflowData);
        }
        else {
            $(elementIDs.statusUpdateDialogue).dialog("close");
            messageDialog.show(WorkFlowStateMessages.fillCheckListRequiredFields);
            //folderManager.getInstance().getFolderInstance().validateCurrentForm(formInstance);
            return;
        }
    }

    function validateAndUpdateWorkFlowData(args, hasValidationErrors) {
        var updateWorkflowData = args[0];
        var currentInstance = args[1];
        if (hasValidationErrors) {
            messageDialog.show(Folder.folderValidationErrorMsg);
            return false;
        }
        else {
            if (updateWorkflowData != undefined) {
                if (GLOBAL.applicationName.toLowerCase() != 'ebenefitsync') {
                    //if (GLOBAL.applicationName.toLowerCase() == 'Test') {//Temporary Disabled EV till get full testing
                    //Check IF Exit Validation Is completed
                    var url = URLs.validateExitValidationCompleted.replace('{FolderVersionID}', updateWorkflowData.folderVersionId)
                    var promise = ajaxWrapper.postJSON(url);
                    promise.done(function (xhr) {
                        if (xhr.Result === ServiceResult.SUCCESS) {
                            postWorkFlowData(updateWorkflowData);
                        } else if (xhr.Result === ServiceResult.FAILURE) {
                            $(elementIDs.statusUpdateDialogue).dialog("close");
                            messageDialog.show(Folder.exitValidationErrorMsg);
                            return false;
                        } else if (xhr.Result === ServiceResult.WARNING) {
                            $(elementIDs.statusUpdateDialogue).dialog("close");
                            messageDialog.show(Folder.exitValidationPlansCompletedMsg);
                            return false;
                        }
                    });
                    refreshInterval = setInterval(function () { AutoRefreshMigrationQueue("", ""); }, 1000);
                } else {
                    postWorkFlowData(updateWorkflowData);
                }
            }
        }
    }

    function postWorkFlowData(updateWorkflowData) {
        var promise = ajaxWrapper.postJSON(URLs.updateWorkflowStatus, updateWorkflowData);
        promise.done(workflowSuccess);
        promise.fail(showError);
    }

    //this function is called below soon after this JS file is loaded 
    function init() {

        //register dialog for grid row add/edit
        $(elementIDs.statusUpdateDialogue).dialog({
            autoOpen: false,
            resizable: false,
            closeOnEscape: true,
            height: 'auto',
            width: 380,
            modal: true,
            position: 'center'
        });

        $(elementIDs.statusUpdateButton).on('click', function () {
            folderStatus.show();
        });
        var isRelease = false;
        $(elementIDs.doneButtonID).on('click', function (event, isRelease, majorVersionComment, majorFolderVersionNumber) {
            //var majorVersionNumber = "";
            var workflowstate = parseInt($(elementIDs.workFlowStateListId).val());
            //var workflowText = $(elementIDs.workFlowStateListId + " option:selected").text();;
            //var approvalstatus = parseInt($(elementIDs.approvalTypeListId).val());
            var approvalstatusText = $(elementIDs.approvalTypeListId + " option:selected").text();
            //var commenttext = $(elementIDs.comments).val();
            //var checkJournalIsOpen = undefined;
            //var folderId = "";
            //if (isRelease) {
            //    majorVersionNumber = majorFolderVersionNumber;
            //    commenttext = majorVersionComment;
            //}

            //var updateWorkflowData = {
            //    tenantId: folderData.tenantId,
            //    folderVersionId: folderData.folderVersionId,
            //    workflowStateId: workflowstate,
            //    approvalStatusId: approvalstatus,
            //    commenttext: commenttext,
            //    userId: folderData.currentUserId,
            //    majorFolderVersionNumber: majorVersionNumber
            //};

            //this.workflowData = updateWorkflowData;

            //var folderInstance = folderManager.getInstance().getFolderInstance();

            //var currentFormInstaceBuilder = folderInstance.formInstances[folderInstance.currentFormInstanceID].FormInstanceBuilder;
            //var accleratedmessage = "";

            //var folderVersionWorkflow = folderVersionWorkflowInstance.getInstance();
            //var workFlowStates = folderVersionWorkflow.getWorkFlowStates();
            //var workFlowList = folderVersionWorkflow.getWorkFlowList();

            //var workFlowMarketApprovalState = workFlowList.filter(function (value) {
            //    return value.WFStateName == WorkFlowState.MarketApproval;
            //});

            //var workFlowMarketApprovalStateStatus = workFlowStates.filter(function (value) {
            //    return value.WorkflowStateID == workFlowMarketApprovalState[0].WorkFlowVersionStateID;
            //});

            //var workFlowTBCOOPCApprovalState = workFlowList.filter(function (value) {
            //    return value.WFStateName == WorkFlowState.TBCOOPCApproval;
            //});

            //var workFlowTBCOOPCApprovalStateStatus = workFlowStates.filter(function (value) {
            //    return value.WorkflowStateID == workFlowTBCOOPCApprovalState[0].WorkFlowVersionStateID;
            //});

            //var workFlowPartDApprovalState = workFlowList.filter(function (value) {
            //    return value.WFStateName == WorkFlowState.PartDApproval;
            //});

            //var workFlowPartDApprovalStateStatus = workFlowStates.filter(function (value) {
            //    return value.WorkflowStateID == workFlowPartDApprovalState[0].WorkFlowVersionStateID;
            //});


            //var workFlowMarketApprovalState = workFlowList.filter(function (value) {
            //    return value.WFStateName == WorkFlowState.MarketApproval;
            //});

            //var workFlowMarketApprovalStateStatus = workFlowStates.filter(function (value) {
            //    return value.WorkflowStateID == workFlowMarketApprovalState[0].WorkFlowVersionStateID;
            //});

            //Check for current folder verison has assigned tasks completed or not
            var isAssignmentTasksCompleted = false;
            if (approvalstatusText == 'Approved' || approvalstatusText == 'Complete') {
                var promiseValidateTask = ajaxWrapper.postJSON(URLs.validateTaskCompleted.replace(/{FolderVersionID}/g, folderData.folderVersionId).replace(/{WorkFlowVersionStateID}/g, workflowstate));
                promiseValidateTask.done(function (xhr) {

                    if (xhr.Items != null && xhr.Items != undefined) {
                        if (xhr.Items.length > 0) {
                            if (xhr.Items[0].Status == 0) {
                                updateFolderVersionStatus();
                            }
                            else {

                            }
                        } else {
                            messageDialog.show(WorkFlowStateMessages.PlanTaskHasNotCompletedMsg);
                        }
                    }
                });
            } else {
                //isAssignmentTasksCompleted = true;
                updateFolderVersionStatus();
            }
            promiseValidateTask.fail(showError);
            //if (isAssignmentTasksCompleted) {
            //    if (folderData.folderType == "MasterList") {
            //        // TODO: if required for WellCare             
            //    }
            //    else if ((workflowText == WorkFlowState.PSoTPreparation && approvalstatus == ApprovalStatus.COMPLETED)) {
            //        postWorkFlowData(updateWorkflowData);
            //    }
            //    else if ((workflowText == WorkFlowState.MarketMeetingUpdates && approvalstatus == ApprovalStatus.COMPLETED)) {
            //        //currentFormInstaceBuilder.form.saveFormInstanceData();
            //        var callbackMetaDeta = {
            //            callback: validateAndUpdateWorkFlowData,
            //            args: [updateWorkflowData, folderInstance]
            //        };
            //        //check form Validation & duplication   
            //        currentFormInstaceBuilder.validation.validateFormInstance(true, callbackMetaDeta);
            //        //postWorkFlowData(updateWorkflowData);
            //    }
            //    else if ((workflowText == WorkFlowState.MarketApproval && approvalstatus == ApprovalStatus.APPROVED)) {
            //        if (workFlowMarketApprovalStateStatus[0].ApprovalStatusID == ApprovalStatus.APPROVED) {
            //            messageDialog.show(WorkFlowStateMessages.marketApprovalAlreadyApproved);
            //        }
            //        else {
            //            postWorkFlowData(updateWorkflowData);
            //        }
            //    }
            //    else if ((workflowText == WorkFlowState.MarketApproval && approvalstatus == ApprovalStatus.NOTAPPROVED)) {
            //        if (workFlowMarketApprovalStateStatus[0].ApprovalStatusID == ApprovalStatus.APPROVED) {
            //            messageDialog.show(WorkFlowStateMessages.marketApprovalAlreadyApproved);
            //        }
            //        else {
            //            postWorkFlowData(updateWorkflowData);
            //        }
            //    }
            //    else if ((workflowText == WorkFlowState.PBPValidation && approvalstatus == ApprovalStatus.COMPLETED)) {
            //        if (workFlowMarketApprovalStateStatus[0].ApprovalStatusID == ApprovalStatus.NOTAPPROVED) {
            //            messageDialog.show(WorkFlowStateMessages.marketApprovalStateStatus);
            //        }
            //        else {
            //            postWorkFlowData(updateWorkflowData);
            //        }
            //    }
            //    else if ((workflowText == WorkFlowState.TBCOOPCApproval && approvalstatus == ApprovalStatus.COMPLETED)) {

            //        if (workFlowTBCOOPCApprovalStateStatus[0].ApprovalStatusID == ApprovalStatus.COMPLETED) {
            //            messageDialog.show(WorkFlowStateMessages.TBCOOPCApprovalAlreadyCompleted);
            //        }
            //        else {
            //            postWorkFlowData(updateWorkflowData);
            //        }
            //    }
            //    else if ((workflowText == WorkFlowState.PartDApproval && approvalstatus == ApprovalStatus.COMPLETED)) {
            //        if (workFlowPartDApprovalStateStatus[0].ApprovalStatusID == ApprovalStatus.COMPLETED) {
            //            messageDialog.show(WorkFlowStateMessages.PartDApprovalAlreadyCompleted);
            //        }
            //        else {
            //            postWorkFlowData(updateWorkflowData);
            //        }
            //    }
            //    else if ((workflowText == WorkFlowState.PreBenchmarkApproval && approvalstatus == ApprovalStatus.COMPLETED)) {
            //        postWorkFlowData(updateWorkflowData);
            //    }
            //    else if ((workflowText == WorkFlowState.DeskReviewInternalReviewI && approvalstatus == ApprovalStatus.COMPLETED)) {
            //        postWorkFlowData(updateWorkflowData);
            //    }
            //    else if ((workflowText == WorkFlowState.BenchmarkInternalReviewII && approvalstatus == ApprovalStatus.COMPLETED)) {
            //        //currentFormInstaceBuilder.form.saveFormInstanceData();
            //        var callbackMetaDeta = {
            //            callback: validateAndUpdateWorkFlowData,
            //            args: [updateWorkflowData, folderInstance]
            //        };
            //        //check form Validation & duplication   
            //        currentFormInstaceBuilder.validation.validateFormInstance(true, callbackMetaDeta);
            //        //postWorkFlowData(updateWorkflowData);
            //    }
            //}
            //else {
            //    messageDialog.show(WorkFlowStateMessages.PlanTaskHasNotCompletedMsg);
            //}

            //currentFormInstaceBuilder.form.saveFormInstanceData();
            //var callbackMetaDeta = {
            //    callback: validateAndUpdateWorkFlowData,
            //    args: [updateWorkflowData, folderInstance]
            //};
            //check form Validation & duplication   
            //currentFormInstaceBuilder.validation.validateFormInstance(true, callbackMetaDeta);

            //postWorkFlowData(updateWorkflowData);
        });

        function updateFolderVersionStatus() {
            var majorVersionNumber = "";
            var workflowstate = parseInt($(elementIDs.workFlowStateListId).val());
            var workflowText = $(elementIDs.workFlowStateListId + " option:selected").text();;
            var approvalstatus = parseInt($(elementIDs.approvalTypeListId).val());
            var approvalstatusText = $(elementIDs.approvalTypeListId + " option:selected").text();
            var commenttext = $(elementIDs.comments).val();
            var checkJournalIsOpen = undefined;
            var folderId = "";
            if (isRelease) {
                majorVersionNumber = majorFolderVersionNumber;
                commenttext = majorVersionComment;
            }

            var updateWorkflowData = {
                tenantId: folderData.tenantId,
                folderVersionId: folderData.folderVersionId,
                workflowStateId: workflowstate,
                approvalStatusId: approvalstatus,
                commenttext: commenttext,
                userId: folderData.currentUserId,
                majorFolderVersionNumber: majorVersionNumber
            };

            this.workflowData = updateWorkflowData;

            var folderInstance = folderManager.getInstance().getFolderInstance();

            var currentFormInstaceBuilder = folderInstance.formInstances.length > 0 ? folderInstance.formInstances[folderInstance.currentFormInstanceID].FormInstanceBuilder : null;
            var accleratedmessage = "";

            var folderVersionWorkflow = folderVersionWorkflowInstance.getInstance();
            var workFlowStates = folderVersionWorkflow.getWorkFlowStates();
            var workFlowList = folderVersionWorkflow.getWorkFlowList();

            var workFlowMarketApprovalState = workFlowList.filter(function (value) {
                return value.WFStateName == WorkFlowState.MarketApproval;
            });

            var workFlowMarketApprovalStateStatus = workFlowStates.filter(function (value) {
                if (workFlowMarketApprovalState != undefined && workFlowMarketApprovalState.length > 0)
                    return value.WorkflowStateID == workFlowMarketApprovalState[0].WorkFlowVersionStateID;
            });

            var workFlowTBCOOPCApprovalState = workFlowList.filter(function (value) {
                return value.WFStateName == WorkFlowState.TBCOOPCApproval;
            });

            var workFlowTBCOOPCApprovalStateStatus = workFlowStates.filter(function (value) {
                if (workFlowTBCOOPCApprovalState != undefined && workFlowTBCOOPCApprovalState.length > 0)
                    return value.WorkflowStateID == workFlowTBCOOPCApprovalState[0].WorkFlowVersionStateID;
            });

            var workFlowPartDApprovalState = workFlowList.filter(function (value) {
                return value.WFStateName == WorkFlowState.PartDApproval;
            });

            var workFlowPartDApprovalStateStatus = workFlowStates.filter(function (value) {
                if (workFlowPartDApprovalState != undefined && workFlowPartDApprovalState.length > 0)
                    return value.WorkflowStateID == workFlowPartDApprovalState[0].WorkFlowVersionStateID;
            });

            if (folderData.folderType == "MasterList") {
                // TODO: if required for WellCare             
            }
            else if ((workflowText == WorkFlowState.PSoTPreparation && approvalstatus == ApprovalStatus.COMPLETED)) {
                postWorkFlowData(updateWorkflowData);
            }
                //1. Check if client name is eBenefitSysnc
                //2. Check if folder is Account type
                //3. Check the state and completion sate
            else if (folderData.isPortfolio == "False" && GLOBAL.applicationName.toLowerCase() == "ebenefitsync" && workflowText == WorkFlowState.ProductConfig && approvalstatus == ApprovalStatus.COMPLETED) {
                //currentFormInstaceBuilder.form.saveFormInstanceData();
                var callbackMetaDeta = {
                    callback: validateAndUpdateWorkFlowData,
                    args: [updateWorkflowData, folderInstance]
                };
                //check form Validation & duplication  
                if (currentFormInstaceBuilder != null) {
                    currentFormInstaceBuilder.validation.validateFormInstance(true, callbackMetaDeta);
                }
                //postWorkFlowData(updateWorkflowData);
            }
            else if ((workflowText == WorkFlowState.MarketMeetingUpdates && approvalstatus == ApprovalStatus.COMPLETED)) {
                //currentFormInstaceBuilder.form.saveFormInstanceData();
                var callbackMetaDeta = {
                    callback: validateAndUpdateWorkFlowData,
                    args: [updateWorkflowData, folderInstance]
                };
                //check form Validation & duplication   
                currentFormInstaceBuilder.validation.validateFormInstance(true, callbackMetaDeta);
                //postWorkFlowData(updateWorkflowData);
            }
            else if ((workflowText == WorkFlowState.MarketApproval && approvalstatus == ApprovalStatus.APPROVED)) {
                if (workFlowMarketApprovalStateStatus[0].ApprovalStatusID == ApprovalStatus.APPROVED) {
                    messageDialog.show(WorkFlowStateMessages.marketApprovalAlreadyApproved);
                }
                else {
                    postWorkFlowData(updateWorkflowData);
                }
            }
            else if ((workflowText == WorkFlowState.MarketApproval && approvalstatus == ApprovalStatus.NOTAPPROVED)) {
                if (workFlowMarketApprovalStateStatus[0].ApprovalStatusID == ApprovalStatus.APPROVED) {
                    messageDialog.show(WorkFlowStateMessages.marketApprovalAlreadyApproved);
                }
                else {
                    postWorkFlowData(updateWorkflowData);
                }
            }
            else if ((workflowText == WorkFlowState.PBPValidation && approvalstatus == ApprovalStatus.COMPLETED)) {
                if (folderData.CategoryName == "Medicare") {
                    var callbackMetaDeta = {
                        callback: validateAndUpdateWorkFlowData,
                        args: [updateWorkflowData, folderInstance]
                    };
                    if (currentFormInstaceBuilder != null) {
                        currentFormInstaceBuilder.validation.validateFormInstance(true, callbackMetaDeta);
                    }
                } else if (workFlowMarketApprovalStateStatus[0].ApprovalStatusID == ApprovalStatus.NOTAPPROVED) {
                    messageDialog.show(WorkFlowStateMessages.marketApprovalStateStatus);
                } else {
                    postWorkFlowData(updateWorkflowData);
                }
            }
            else if ((workflowText == WorkFlowState.TBCOOPCApproval && approvalstatus == ApprovalStatus.COMPLETED)) {

                if (workFlowTBCOOPCApprovalStateStatus[0].ApprovalStatusID == ApprovalStatus.COMPLETED) {
                    messageDialog.show(WorkFlowStateMessages.TBCOOPCApprovalAlreadyCompleted);
                }
                else {
                    postWorkFlowData(updateWorkflowData);
                }
            }
            else if ((workflowText == WorkFlowState.PartDApproval && approvalstatus == ApprovalStatus.COMPLETED)) {
                if (workFlowPartDApprovalStateStatus[0].ApprovalStatusID == ApprovalStatus.COMPLETED) {
                    messageDialog.show(WorkFlowStateMessages.PartDApprovalAlreadyCompleted);
                }
                else {
                    postWorkFlowData(updateWorkflowData);
                }
            }
            else if ((workflowText == WorkFlowState.PreBenchmarkApproval && approvalstatus == ApprovalStatus.COMPLETED)) {
                if (folderData.CategoryName == "Medicare") {
                    var callbackMetaDeta = {
                        callback: validateAndUpdateWorkFlowData,
                        args: [updateWorkflowData, folderInstance]
                    };
                    if (currentFormInstaceBuilder != null) {
                        currentFormInstaceBuilder.validation.validateFormInstance(true, callbackMetaDeta);
                    }
                } else
                    postWorkFlowData(updateWorkflowData);
            }
            else if ((workflowText == WorkFlowState.PostBenchmarkApproval && approvalstatus == ApprovalStatus.COMPLETED)) {
                if (folderData.CategoryName == "Medicare" && GLOBAL.clientName.toLowerCase() != 'floridablue') {
                    var callbackMetaDeta = {
                        callback: validateAndUpdateWorkFlowData,
                        args: [updateWorkflowData, folderInstance]
                    };
                    if (currentFormInstaceBuilder != null) {
                        currentFormInstaceBuilder.validation.validateFormInstance(true, callbackMetaDeta);
                    }
                } else
                    postWorkFlowData(updateWorkflowData);
            }
            else if ((workflowText == WorkFlowState.DeskReviewInternalReviewI && approvalstatus == ApprovalStatus.COMPLETED)) {
                postWorkFlowData(updateWorkflowData);
            }
            else if ((workflowText == WorkFlowState.BenchmarkInternalReviewII && approvalstatus == ApprovalStatus.COMPLETED)) {
                //currentFormInstaceBuilder.form.saveFormInstanceData();
                var callbackMetaDeta = {
                    callback: validateAndUpdateWorkFlowData,
                    args: [updateWorkflowData, folderInstance]
                };
                //check form Validation & duplication   
                currentFormInstaceBuilder.validation.validateFormInstance(true, callbackMetaDeta);
                //postWorkFlowData(updateWorkflowData);
            }
            else if ((approvalstatus == ApprovalStatus.COMPLETED || approvalstatus == ApprovalStatus.APPROVED)) {
                postWorkFlowData(updateWorkflowData);
            }
            else {
                postWorkFlowData(updateWorkflowData);
            }
        }
    }


    $(document).ready(function () {
        //initialize the dialog after this js are loaded
        init();

        $(elementIDs.workFlowStateListId).change(function () {
            var wfVersionStateId = parseInt($(elementIDs.workFlowStateListId).val());
            getApprovalStatusTypeList(folderData.folderVersionId, wfVersionStateId);
        });
    });

    //these are the properties that can be called by using AccountDialog.<Property>
    //eg. accountDialog.show('name','add');
    return {
        show: function () {

            getWorkFlowStateList(folderData.folderVersionId);
            //open the dialog - uses jqueryui dialog:
            $(elementIDs.workflowstatediv).removeClass('has-error');
            $(elementIDs.comments).removeClass('comment-has-error');
            $(elementIDs.workflowstatediv + ' .help-block').text(FolderVersion.selectWorkFlowStateMsg);
            $(elementIDs.workFlowStateListId).prop('selectedIndex', 0);
            $(elementIDs.approvaldiv).removeClass('has-error');
            $(elementIDs.approvaldiv + ' .help-block').text(FolderVersion.selectApprovalStatusMsg);
            $(elementIDs.approvalTypeListId).prop('selectedIndex', 0);
            $(elementIDs.ownerdiv).removeClass('has-error');
            $(elementIDs.ownerdiv + ' .help-block').text(FolderVersion.selectOwnerMsg);
            $(elementIDs.ownerListId).prop('selectedIndex', 0);
            $(elementIDs.comments).val('');
            $(elementIDs.statusUpdateDialogue).dialog('option', 'title', FolderVersion.statusUpdateMsg);
            $(elementIDs.statusUpdateDialogue).css('overflow', 'hidden');
            $(elementIDs.statusUpdateDialogue).dialog("open");
        },
        updateWorkFlowStatus: function (args) {
            updateWorkFlowStatus(args);
        },
        postWorkFlowData: function (args) {
            postWorkFlowData(args);
        },
        queueProductToPluginVersionProcessQueue: function (productSelectionList, isManual) {
            queueProductToPluginVersionProcessQueue(productSelectionList, isManual);
        },
        queueProductsToTranslator: function (folderVersionId, updateWFData) {
            queueProductsToTranslator(folderVersionId, updateWFData);
        },
        validateAndUpdateWorkFlowData: function (args, hasValidationErrors) {
            validateAndUpdateWorkFlowData(args, hasValidationErrors);
        }
    }
}();//invoke immediately 
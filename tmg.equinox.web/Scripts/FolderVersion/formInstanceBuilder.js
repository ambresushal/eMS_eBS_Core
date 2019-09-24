function formInstanceBuilder(tenantId, accountId, folderVersionId, folderId, formInstanceId, formDesignVersionID, formName,
    autoSaveWorker, autoSaveTimer, isfolderReleased, folderData, folderType, anchorFormInstanceID, IsFormInstanceEditable, effectiveDate, currentUserId, currentUserName, lockSectionName) {

    this.heighlightValidationError = undefined;
    this.tenantId = tenantId == undefined ? 1 : tenantId;
    this.accountId = accountId;
    this.folderVersionId = folderVersionId;
    this.folderId = folderId;
    this.effectiveDate = effectiveDate;
    this.formInstanceId = formInstanceId;
    this.currentUserId = currentUserId;
    this.currentUserName = currentUserName;
    this.formInstanceDivJQ = "#tab" + anchorFormInstanceID;
    this.anchorFormInstanceID = anchorFormInstanceID;
    this.formDesignVersionId = formDesignVersionID;
    this.designData = undefined;
    this.formData = undefined;
    this.impactList = undefined;
    this.gridDesigns = [];
    this.repeaterBuilders = [];
    this.selectedSection = undefined;
    this.selectedSectionName = undefined;
    this.folderVersionWorkFlow = undefined;
    this.sections = {};
    this.errorGridData = [];
    this.pbpViewAction = new pbpViewAction(this);
    this.errorManager = new errorManager(this);
    this.annotationMgr = new annotationManager(this);
    this.formName = formName;
    this.formDesignId = undefined;
    this.formValidationManager = undefined;
    this.ruleProcessor = undefined;
    this.repeaterRuleProcessor = undefined;
    this.autoSaveWorker = autoSaveWorker;
    this.autoSaveTimer = autoSaveTimer;
    this.folderData = folderData;
    this.isfolderReleased = isfolderReleased || false;
    this.fieldMaskValidator = new fieldMaskValidator(this);
    var oldformInstanceData = undefined;
    this.autoSaveDuration = undefined;
    this.autoSaveEnabled = undefined;
    this.currentActivitylogData = new Object();
    this.isProductInTranslation = false;
    this.isProductInTransmission = false;
    this.IsProductInMigration = false;
    this.folderType = folderType;
    this.IsMasterList = folderType == "MasterList" ? true : false;
    this.IsJQLoaded = true;
    this.UserMasterListInstanceInit = false;
    this.repeaterUserSettingData = [];
    this.stateAccessRoles = [];
    this.hasDocumentLockOverriden = undefined;
    this.documentLockWorker = undefined;
    this.isDocumentEditable = folderData.isEditable;
    this.documentLock = documentLockMethods(this);
    this.IsFormInstanceEditable = IsFormInstanceEditable;
    this.lockSectionName = lockSectionName;
    this.commentData = {};
    this.hasManualOverideElements = false;
    this.isSectionEditable = false;
    this.vbidext = null;
    this.currentSectionRepeaterIDs = [];
    this.isSectionLevelMO = false;

    if (typeof vbIsFolderLockEnable === "undefined") { this.isFolderLockEnable = false } else { this.isFolderLockEnable = vbIsFolderLockEnable };
    this.isFolderLocked = this.isFolderLockEnable && this.folderData.isLocked == "True";
    this.URLs = {
        getMultipleFormInstancesData: '/FolderVersion/GetMultipleFormInstancesData?tenantId=1&formInstanceIDs={formInstanceIDs}',
        getFormInstanceDesignData: "/FormInstance/GetFormInstanceDesignData?tenantId={tenantId}&formInstanceId={formInstanceId}&formDesignVersionId={formDesignVersionId}&folderVersionId={folderVersionId}&reloadData={reloadData}&sectionName={sectionName}&rulesPreloaded={rulesPreloaded}&ancillaryProductName={ancillaryProductName}",
        saveFormInstanceData: "/FormInstance/SaveFormInstanceData",
        loadCustomRules: "/FormInstance/LoadCustomRules?tenantId={tenantId}&formInstanceId={formInstanceId}&formDesignVersionId={formDesignVersionId}&folderVersionId={folderVersionId}",
        getAutoSaveDurationurl: '/Settings/GetAutoSaveDuration?tenantId={tenantId}',
        getPropertiesData: '/FormInstance/GetPropertiesData?formInstanceID={formInstanceID}',
        checkFolderLockUrl: '/FormInstance/HasFolderLockByCurrentUser?folderId={folderId}',
        getSectionDataFromServer: '/FormInstance/GetFormInstanceSectionData?sectionName={sectionName}&formInstanceId={formInstanceId}&formDesignId={formDesignId}&folderVersionId={folderVersionId}&formDesignVersionId={formDesignVersionId}',
        getRepeateUserSettings: "/Settings/GetFormDesignUserSetting?formDesignVersionId={formDesignVersionId}",
        validateFormInstanceAtServer: '/FormInstance/ValidateFormInstance',
        saveFormInstanceSectionData: "/FormInstance/UpdateFormInstanceSectionData",
        getPropertiesData: '/FormInstance/GetPropertiesData?formInstanceID={formInstanceID}',
        saveActivityLogData: '/FormInstance/SaveFormInstanceActivityLogData',
        saveDocumentViewImpactLog: '/PBPViewAction/SaveDocumentViewImpactLog',
        folderVersionDetailsPortfolioBasedAccount: '/FolderVersion/Index?tenantId={tenantId}&folderVersionId={folderVersionId}&folderId={folderId}&foldeViewMode={foldeViewMode}&mode={mode}',
        getProductState: '/Translator/GetProductState?formInstanceId={formInstanceId}',
        isProductInTransmission: '/Translator/IsProductInTransmission?formInstanceId={formInstanceId}',
        getdocumentviewlisturl: '/FormInstance/GetDocumentViewList?tenantId={tenantId}&formInstanceId={formInstanceId}&mode={Mode}',
        getMasterListDocuments: '/FolderVersion/GetMasterListDocuments?formInstanceId={formInstanceId}',
        masterlisturl: "/FolderVersion/IndexML?tenantId=1&folderType={folderType}",
        isProductInTransmission: '/Translator/IsProductInTransmission?formInstanceId={formInstanceId}',
        //getdocumentviewlisturl: '/FormInstance/GetDocumentViewList?tenantId={tenantId}&formInstanceId={formInstanceId}',
        forminstancelisturl: '/FolderVersion/GetFormInstanceList?tenantId=1&folderVersionId={folderVersionId}&folderId={folderId}',
        getFolderVersionViewModel: '/FolderVersion/GetFolderVersionViewModel?tenantId=1&folderType={folderType}',
        getStateAccessRoles: '/FolderVersion/GetStateAccessRoles?tenantId=1&folderVersionId={folderVersionId}',
        isDocumentLockedByUser: '/FormInstance/IsDocumentLockedByCurrentUser?formInstanceId={formInstanceId}',
        folderLockWorker: '/Scripts/FolderVersion/folderLock.js',
        getDocumentLockOverridenStatus: '/FolderVersion/CheckDocumentLockIsOverriden',
        manageSectionLock: '/FormInstance/ManageSectionLock?folderID={folderID}&formInstanceID={formInstanceID}&displayViewName={displayViewName}&displaySectionName={displaySectionName}&sectionName={sectionName}&formDesignID={formDesignID}&formName={formName}&isMasterList={isMasterList}',
        releaseSectionLock: '/FormInstance/ReleaseSectionLockonDocumentClose?formInstanceID={formInstanceID}&previousSectionName={sectionName}',
        releaseViewLockUrl: '/FolderVersion/ReleaseDocumentLockonOnViewChange?formInstanceId={formInstanceID}&newFormInstanceId={newFormInstanceId}',
        getFormInstanceCommentData: '/FormInstance/GetFormInstanceCommentData?formInstanceId={formInstanceId}',
    }

    this.elementIDs = {
        btnSaveFormInstanceDataJQ: "#btnSaveFormData",
        btnBottomSaveFormDataJQ: '#btnBottomSaveFormData',
        sectionMenuTemplate: "SectionMenuTemplate",
        sectionMenuTemplateJQ: "#SectionMenuTemplate",
        mlDocumentMenuTemplateJQ: '#MLDocumentMenuTemplate',
        sectionMenuContainer: "sectionMenuContainer",
        sectionMenuContainerJQ: "#sectionMenuContainer",
        sectionMenu: "sectionMenu",
        sectionMenuJQ: "#sectionMenu",
        sectionMenuContainerBottom: "sectionMenuContainerBottom",
        sectionMenuContainerBottomJQ: "#sectionMenuContainerBottom",
        sectionMenuBottom: "sectionMenuBottom",
        sectionMenuBottomJQ: "#sectionMenuBottom",
        masterListDocumentMenu: "masterListDocumentMenu",
        masterListDocumentMenuJQ: "#masterListDocumentMenu",
        foldertabsJQ: "#foldertabs",
        errorGridContainer: "errorGridContainer",
        errorGridContainerJQ: "#errorGridContainer",
        btnManualDataPopup: "#btnManualDataPopup",
        serviceGroupingRepeaterPath: "ServiceGroup.ServiceGrouping",
        //JSRENDER elements 
        formTemplateJQ: "#FormActionTemplate",
        sectionTemplateJQ: "#SectionTemplate",
        bottomMenuJQ: "#bottom-menu",
        bottomMenuTabsJQ: "#bottom-menu-tabs",
        btnViewFaxBackReportJQ: "#btnViewFaxBackReportJQ",
        folderAutoSaveAlertJQ: '#folderAutoSaveAlert',
        folderVersionAlertJQ: "#folderVersionAlert",
        btnPropertiesJQ: "#btnProperties",
        btnFacetPropertiesJQ: "#btnFacetProperties",
        folderVersionPropertiesDialogJQ: "#folderVersionPropertiesDialog",
        folderVersionPropertiesGridJQ: "#folderVersionPropertiesGrid",
        folderVersionPropertiesGrid: "folderVersionPropertiesGrid",
        activityLogGridJQ: "#activityLogGrid",
        ruleExecutionLogGridJQ: "#ruleExecutionLogGrid",
        btnActivityLog: "#btnActivityLog",
        copyFromAuditTrailGridJQ: "#copyFromAuditTrailGrid",
        copyFromAuditTrailGrid: "copyFromAuditTrailGrid",
        facetPropertiesGridJQ: "#facetPropertiesGrid",
        facetPropertiesGrid: "facetPropertiesGrid",
        copyFromAuditTrailGrid: "copyFromAuditTrailGrid",
        shadowPrefixSource: "#PRO3DropDown2527",
        serviceGroupName: "Service Group",
        btnDeleteFormInstance: "#btnDeleteForm",
        btnRealoadFormDataJQ: '#btnReloadFormData',
        btnValidateJQ: "#btnValidate",
        btnValidateSectionJQ: "#btnValidateSection",
        btnJournalEntryJQ: "#btnJournalEntry",
        btnStatusUpdateJQ: "#statusupdatebutton",
        btnSaveFormDataJQ: '#btnSaveFormData',
        btnDeleteVBIDForm: '#btnDeleteVBIDForm',
        btnCreateFormJQ: '#btnCreateForm',
        versionHistoryButtonMLJQ: '#versionHistoryButtonML',
        deductiblesSection: "PRO3Section419",
        limitSection: "PRO3Section451",
        folderVersionPropertiesversionIDJQ: '#folderVersion',
        folderVersionPropertiescategoryIDJQ: '#categoryID',
        folderVersionPropertiescategoryNameJQ: '#categoryName',
        folderIconsSeparatorJQ: '#folderIcons .separator',
        viewdropdown: '#viewDropdown',
        assignFolderMemberbuttonJQ: '#assignFolderMemberbutton',
        facetsTranslateJQ: '#facetsTranslate',
        //generateProductShareReportButtonJQ: '#generateProductShareReportButton',
        btnView1ReportJQ: '#btnView-1-Report',
        btnView2ReportJQ: '#btnView-2-Report',
        btnValidateAllJQ: '#btnValidateAll',
        btnBaselineMLJQ: '#btnBaselineML',
        mLView_accountName: '#formTitle_accountName',
        mLView_folderName: '#formTitle_folderName',
        mLView_effectiveDate: '#formTitle_effectiveDate',
        mLView_vesrionHistoryDialogML: '#vesrionHistoryDialogML_folderName',
        IsJQLoaded: '#IsJQLoaded',
        btnBottomCreateNewVersion: '#btnBottomCreateNewVersion',
        documentODMDisableAlertJQ: '#documentODMDisableAlert',
        btnBaselineJQ: "#btnBaseline",
        btnNewVersionJQ: "#newVersionHistory",
        btnRetroJQ: "#retroVersionHistory",
        btnAssignFolderMember: '#assignFolderMemberbutton',
        btnExitValidateResultsJQ: "#btnExitValidateResults",
        btnShowAllProducts: "#btnShowAllProducts",
    }

    this.menu = this.menuMethods();
    this.form = this.formMethods();
    this.validation = this.validationMethods();
    this.bottomMenu = this.bottomMenuMethods();
    this.loadDocumentHandlerMethod = this.loadDocumentMethods();
    this.rules = this.rulesMethods();
    this.accessPermissions = this.accessPermissionsMethods();
    this.dropDownTextBox = this.dropDownTextBox();
    this.journal = this.journalMethods();
    this.autosave = this.autoSaveMethods();
    this.loadDataFromRepeater = {};
    this.repeaterFullPathList = [];
    this.repeaterRowIdList = [];
    this.hasOpenJournals = undefined;
    this.hasChanges = false;
    this.customRules = new customruleHandler(FormTypes.CURRENTFORMDESIGNID, this.IsMasterList);
    this.expressionBuilder = new expressionBuilder();
    this.expressionBuilderExten = new expressionBuilderRulesExt();
    this.objManualOverride = this.manualOverride();
    //set flag to Apply Access Permissions for section
    if (typeof vbIsHiddenOrDisableSections === "undefined") { this.isHiddenOrDisableSections = false } else { this.isHiddenOrDisableSections = vbIsHiddenOrDisableSections };
    //set flag to Stop Scroll with Floating Headers
    if (typeof vbIsStopScrollFloatingHeaders === "undefined") { this.isStopScrollFloatingHeaders = false } else { this.isStopScrollFloatingHeaders = vbIsStopScrollFloatingHeaders }
    // set flag to apply field masking for regex
    this.applyMaskingFlag = true;
    //if (typeof applyMaskFlag === "undefined") { this.applyMaskingFlag = false } else { this.applyMaskingFlag = applyMaskFlag };
    //isPeodutIdConfirmClickNo added to handle form instance level dicision of uncheck isproductnew checkbox
    this.isPeodutIdConfirmClickedNo = false;
    this.documentLock.resetCurrentUserLockSession();
}

formInstanceBuilder.prototype.loadFormInstanceDesignData = function (reloadData, sectionName, isBackgroundProcess, callbackMethod, ancillaryProductName) {
    var currentInstance = this;

    reloadData = reloadData != undefined ? reloadData : true;
    sectionName = sectionName != undefined && sectionName != null ? sectionName : "";
    if (ancillaryProductName != undefined && sectionName != undefined) { reloadData = false; }
    $(currentInstance.elementIDs.btnPropertiesJQ).unbind();
    $(currentInstance.elementIDs.btnPropertiesJQ).bind('click', function () {
        currentInstance.GetfolderVersionPropertiesDialog(currentInstance.folderVersionId);
    });

    $(currentInstance.elementIDs.btnFacetPropertiesJQ).bind('click', function () {
        currentInstance.GetFacetsPropertiesDialog(currentInstance.folderVersionId);
    });

    var fdvPreload = formDesignVersionRulesPreLoadManager.getInstance();
    var rulesPreloaded = false;
    if (this.IsMasterList == false && fdvPreload.hasRule(this.formDesignVersionId) == true) {
        rulesPreloaded = true;
    }
    var url = this.URLs.getFormInstanceDesignData.replace(/\{tenantId\}/g, currentInstance.tenantId).replace(/\{formInstanceId\}/g, currentInstance.formInstanceId).replace(/\{formDesignVersionId\}/g, currentInstance.formDesignVersionId).replace(/\{folderVersionId\}/g, currentInstance.folderVersionId);
    url = url.replace(/\{reloadData\}/g, reloadData).replace(/\{sectionName\}/g, sectionName);
    url = url.replace(/\{rulesPreloaded\}/g, rulesPreloaded)
    url = url.replace(/\{ancillaryProductName}/g, ancillaryProductName);
    var promiseFormInstanceData = ajaxWrapper.getJSON(url);
    var promiseAccessRoles = ajaxWrapper.getJSON(this.URLs.getStateAccessRoles.replace(/\{tenantId\}/g, currentInstance.tenantId).replace(/\{folderVersionId\}/g, currentInstance.folderVersionId));
    var promiseRepeaterUserSettingData = ajaxWrapper.getJSON(this.URLs.getRepeateUserSettings.replace(/\{formDesignVersionId\}/g, currentInstance.formDesignVersionId));
    var promiseAntCommentsData = ajaxWrapper.getJSON(currentInstance.URLs.getFormInstanceCommentData.replace(/\{formInstanceId\}/g, currentInstance.formInstanceId));
    //register ajax success callback
    if (callbackMethod != undefined || callbackMethod != null) {
        $.when(promiseFormInstanceData, promiseAccessRoles, promiseRepeaterUserSettingData, promiseAntCommentsData)
            .then(currentInstance.loadDocumentHandlerMethod.loadDocument, currentInstance.showError).then(function () { callbackMethod.callback(callbackMethod.callbackArgs); });
    } else {
        $.when(promiseFormInstanceData, promiseAccessRoles, promiseRepeaterUserSettingData, promiseAntCommentsData).then(currentInstance.loadDocumentHandlerMethod.loadDocument, currentInstance.showError);
    }



    //$.when(promiseFormInstanceData, promiseAccessRoles, promiseRepeaterUserSettingData).then(currentInstance.loadDocumentHandlerMethod.loadDocument, currentInstance.showError);
}
formInstanceBuilder.prototype.getRepeaterUserSettingData = function (key) {
    var currentInstance = this;
    var userSetting = [];
    $.each(currentInstance.repeaterUserSettingData, function (index, item) {
        if (item.Key === key) {
            userSetting.push(item);
        }
    });
    return userSetting;
}

formInstanceBuilder.prototype.loadDocumentMethods = function () {
    var currentInstance = this;
    return {
        loadDocument: function (xhrFormInstanceData, xhrAccessRoles, xhrRepeaterUserSettingData, xhrAntCommentsData) {

            if (xhrRepeaterUserSettingData && xhrRepeaterUserSettingData.length > 0) {
                currentInstance.repeaterUserSettingData = xhrRepeaterUserSettingData[0];
            }

            stateAccessRoles = xhrAccessRoles[0];

            if (xhrAntCommentsData != null) {
                if (xhrAntCommentsData[0] != "") {
                    currentInstance.commentData = JSON.parse(xhrAntCommentsData[0]);
                }
                else {
                    currentInstance.commentData = {};
                }
            }

            activitylogger.getFormInstanceId(currentInstance.formInstanceId);
            activitylogger.init(currentInstance.formInstanceId);
            activitylogger.getFormName(currentInstance.formName);

            if (ActiveRuleExecutionLogger == "True") {
                ruleExecutionLogger.getFormInstanceId(currentInstance.formInstanceId);
                ruleExecutionLogger.getFormName(currentInstance.formName);
                ruleExecutionLogger.init();
            } else {
                for (var i = 0; i < $(currentInstance.elementIDs.bottomMenuTabsJQ + " li").length; i++) {
                    if ($(currentInstance.elementIDs.bottomMenuTabsJQ + " li")[i].innerText == 'Rule Execution Log') {
                        $(currentInstance.elementIDs.bottomMenuTabsJQ + " li")[i].style.display = 'none';
                    }
                }
            }

            var formInstanceDesignData = xhrFormInstanceData[0];
            currentInstance.designData = formInstanceDesignData;
            var fdvPreload = formDesignVersionRulesPreLoadManager.getInstance();
            if (currentInstance.IsMasterList == false && fdvPreload.hasRule(currentInstance.designData.FormDesignVersionId) == true) {
                currentInstance.designData.Rules = fdvPreload.getRule(currentInstance.designData.FormDesignVersionId);
                currentInstance.designData.Validations = fdvPreload.getValidation(currentInstance.designData.FormDesignVersionId);
            }
            var data = currentInstance.designData.JSONData.replace(/\[Select One]/g, '').replace(/\Select One/g, '');
            currentInstance.formData = JSON.parse(data);
            currentInstance.formDesignId = formInstanceDesignData.FormDesignId;
            currentInstance.errorGridData = formInstanceDesignData.errorGridData == "" ? [] : JSON.parse(formInstanceDesignData.errorGridData);
            currentInstance.hasChanges = false;

            //observe all data right from the time it is been loaded.
            currentInstance.observeAllNonArrayFields(currentInstance.formData, "");
            currentInstance.formValidationManager = new formValidationManager(currentInstance);
            currentInstance.ruleProcessor = new ruleProcessor(currentInstance.formData);
            currentInstance.repeaterRuleProcessor = new repeaterRuleProcessor(currentInstance.formData, currentInstance.ruleProcessor, null);

            currentInstance.form.initSections();
            currentInstance.menu.renderMenu();
            currentInstance.bottomMenu.renderMenu();
            try {
                exitValidate.clearExitValidateResults();
                $(currentInstance.elementIDs.btnExitValidateResultsJQ).unbind('click');
                $(currentInstance.elementIDs.btnExitValidateResultsJQ).bind('click', function () {
                    exitValidate.loadExitValidateResults(currentInstance.formInstanceId);
                });
            }
            catch (e) {
            }
            if (currentInstance.form.hasValidationErrors()) {
                currentInstance.form.showValidationErrorsOnForm();
            }
            else {
                currentInstance.form.hideValidationErrorsOnForm();
            };
            //process rules targeting values
            //if (currentInstance.isfolderReleased == false) {
            //    currentInstance.rules.processValueRulesOnData('DOCUMENTROOT');
            //}

            //currentInstance.form.renderSection(currentInstance.selectedSection, true);
            currentInstance.autosave.InitializeAutoSave();
            currentInstance.journal.InitializeOpenJournals();

            //Initialize PBP View Actions
            if (GLOBAL.applicationName.toLowerCase() == 'emedicaresync') {
                currentInstance.pbpViewAction.buildImpactGrid();
                if (currentInstance.impactList != undefined && currentInstance.impactList.length > 0) {
                    currentInstance.pbpViewAction.fillImpactGrid(currentInstance.impactList);
                }
            }
            //display alert message 
            if (currentInstance.designData.IsNewLoadedVersionIsMajorVersion == true) {

                $(currentInstance.elementIDs.folderVersionAlertJQ).show();
                $(currentInstance.elementIDs.folderVersionAlertJQ).fadeOut(7000);
                //Reload the folderVersionWorkFlow
                this.folderVersionWorkFlow = folderVersionWorkflowInstance.getInstance(currentInstance.accountId, currentInstance.folderVersionId, currentInstance.folderId);
                this.folderVersionWorkFlow.load();
                IsNewLoadedVersionIsMajorVersion = false;
            }

            if (currentInstance.folderData.isPortfolio.toLowerCase() == "true" && currentInstance.isDocumentEditable.toLowerCase() == "true" && currentInstance.IsFormInstanceEditable == true) {
                initDocumentLockWorker(currentInstance);
            }
            if (isFromExitValidate != null) {
                if (isFromExitValidate == true) {
                    currentInstance.bottomMenu.showExitValidateTab();
                    isFromExitValidate = false;
                }
            }
            currentInstance.SetValidateMenuVisibility();

            //initialize VBID
            if (GLOBAL.applicationName.toLowerCase() == 'emedicaresync') {
                currentInstance.vbidext = new vbidextension(currentInstance.anchorFormInstanceID, currentInstance.folderVersionId, currentInstance.folderId);
                currentInstance.vbidext.setVBIDMenu();

                if (currentInstance.formDesignId == 2409) {
                    console.log('Check for ' + currentInstance.formInstanceId);
                    var elem = $(currentInstance.elementIDs.btnDeleteVBIDForm);
                    if (elem == null || elem.length == 0) {
                        var deleteBtn = $("<button class='btn btn-sm but-align' title='Delete' id='btnDeleteVBIDForm' type='button' aria-label='Left Align'><i class='material-icons'>&#xE872;</i></button>").insertAfter($(currentInstance.elementIDs.btnSaveFormDataJQ));
                        deleteBtn.on('click', function () {
                            currentInstance.vbidext.deleteFormInstance(currentInstance.formInstanceId, currentInstance.formName);
                        });
                    }
                    else {
                        elem.off('click');
                        elem.on('click', function () {
                            currentInstance.vbidext.deleteFormInstance(currentInstance.formInstanceId, currentInstance.formName);
                        });
                    }
                }
                else {
                    var elem = $(currentInstance.elementIDs.btnDeleteVBIDForm);
                    elem.remove();
                }
            }
            this.isSectionEditable = false;
        }
    }
}

formInstanceBuilder.prototype.SetValidateMenuVisibility = function () {
    try {
        var currentInstance = this;
        if (nonValidateFormDesignID.indexOf(currentInstance.formDesignId) != -1) {
            // Hide Validate And Section Valid Menu button
            $(currentInstance.elementIDs.btnValidateJQ).css('display', 'none');
            $(currentInstance.elementIDs.btnValidateSectionJQ).css('display', 'none');
        }
        else {
            $(currentInstance.elementIDs.btnValidateJQ).css('display', '');
            $(currentInstance.elementIDs.btnValidateSectionJQ).css('display', '');
        }
    } catch (e) {
    }
}

formInstanceBuilder.prototype.observeAllNonArrayFields = function (data, rootPath) {
    for (var prop in data) {
        if (typeof data[prop] == "object") {
            if (!(data[prop] instanceof Array)) {
                if (rootPath == "") {
                    this.observeAllNonArrayFields(data[prop], prop);
                }
                else {
                    this.observeAllNonArrayFields(data[prop], rootPath + '.' + prop);
                }
            }
            else {
                $.observe(this.formData, rootPath + '.' + prop, this.form.objectChangeHandler);
            }
        }
        else {
            $.observe(this.formData, rootPath + '.' + prop, this.form.objectChangeHandler);
        }
    }
}

function checkServiceIsSelected(left, right) {
    var breakLoop = false;
    try {
        if (left != "" && right != "") {
            for (var i = 0; i < right.length; i++) {
                if (left.indexOf(right[i]) == -1) {
                    breakLoop = true;
                    break;
                }
            }
            if (!breakLoop)
                return true;
        }
    }

    catch (ex) {

        return false;

    }

}

formInstanceBuilder.prototype.loadPreview = function (formDesignData) {
    var currentInstance = this;
    currentInstance.designData = formDesignData;
    currentInstance.formData = JSON.parse(currentInstance.designData.JSONData.replace(/\[Select One]/g, ''));
    //initialize the instance validation Manager
    currentInstance.formValidationManager = new formValidationManager(currentInstance);

    currentInstance.form.initSections();
    currentInstance.menu.renderMenu();
    currentInstance.bottomMenu.renderMenu();
    if (currentInstance.form.hasValidationErrors()) {
        currentInstance.form.showValidationErrorsOnForm();
    }
    else {
        currentInstance.form.hideValidationErrorsOnForm();
    };
    currentInstance.form.renderSection(currentInstance.selectedSection, true);
}

formInstanceBuilder.prototype.showError = function (xhr) {
    if (xhr.status == 999)
        window.location.href = '/Account/LogOn';
    else
        messageDialog.show(Common.errorMsg);
}

formInstanceBuilder.prototype.menuMethods = function () {
    var currentInstance = this;
    return {
        groupViews: function (objectArray, property) {
            return objectArray.reduce(function (acc, obj) {
                var key = obj[property];
                if (!acc[key]) {
                    acc[key] = [];
                }
                acc[key].push(obj);
                return acc;
            }, {});
        },
        //renders the left side menu containing section list
        renderMenu: function () {
            $(currentInstance.elementIDs.sectionMenuContainerJQ + " select:eq(0)").find('option').remove()
            $(currentInstance.elementIDs.sectionMenuContainerBottomJQ + " select").find('option').remove()
            exitValidate.exitValidateMenu(currentInstance);
            if (currentInstance.designData.FormName == "CommercialMedicalAnchor") {
                $(currentInstance.elementIDs.btnShowAllProducts).show();
            }
            else {
                $(currentInstance.elementIDs.btnShowAllProducts).hide();
            }
            //loads the section data in jsrender template to render the menu            
            $.templates({ sectionMenuTmplt: { markup: currentInstance.elementIDs.sectionMenuTemplateJQ } });
            $(currentInstance.elementIDs.sectionMenuJQ).html($.render.sectionMenuTmplt(currentInstance.designData));
            $(currentInstance.elementIDs.sectionMenuBottomJQ).html($.render.sectionMenuTmplt(currentInstance.designData));
            if (currentInstance.IsMasterList == false) {
                var url = currentInstance.URLs.getdocumentviewlisturl.replace(/\{tenantId\}/g, currentInstance.tenantId).replace(/\{formInstanceId\}/g, currentInstance.formInstanceId);
                var promise = ajaxWrapper.getJSON(url);
                //register ajax success callback
                promise.done(function (result) {
                    //loads the view dropdown data                 
                    $(currentInstance.elementIDs.sectionMenuContainerJQ).css('display', 'block');
                    $(currentInstance.elementIDs.viewdropdown).find('optgroup').remove();
                    $(currentInstance.elementIDs.viewdropdown).find('option').remove();

                    var groupedDocs = currentInstance.menu.groupViews(result, 'FormDesignTypeID');
                    $.each(groupedDocs, function (ind, val) {
                        var group = $('<optgroup label="' + val[0].FormDesignTypeName + '" />');
                        $.each(val, function (index, item) {
                            // Disabled returned true to disable access permission against Design documents.
                            // var isOptionDisable = true;
                            // var isOptionDisable = false;
                            //isOptionDisable = !currentInstance.checkForUserRoleAccessPermissions(item, currentInstance);
                            $("<option class='section-menu-item nav-pills' value= " + item.FormInstanceId + '_' + item.FormDesignVersionID + "/>").html(item.FormDesignDisplayName).appendTo(group);
                            //$(currentInstance.elementIDs.viewdropdown).find('option:last').prop('disabled', isOptionDisable)
                        });
                        group.appendTo($(currentInstance.elementIDs.viewdropdown));
                    });
                    $(currentInstance.elementIDs.viewdropdown + " option").each(function (idx, control) {
                        var forminstid = $(control)[0].value.split('_');
                        if (forminstid[0] == currentInstance.formInstanceId) {
                            $(control).prop('selected', true);
                        }
                    });
                });
            }

            var url = currentInstance.URLs.getdocumentviewlisturl.replace(/\{tenantId\}/g, currentInstance.tenantId).replace(/\{formInstanceId\}/g, currentInstance.formInstanceId).replace(/\{Mode\}/g, currentInstance.isDocumentEditable);
            var promise = ajaxWrapper.getJSON(url);
            //register ajax success callback
            promise.done(function (result) {
                //loads the view dropdown data                 
                $(currentInstance.elementIDs.sectionMenuContainerJQ).css('display', 'block');
                $(currentInstance.elementIDs.viewdropdown).find('optgroup').remove();
                $(currentInstance.elementIDs.viewdropdown).find('option').remove();

                var groupedDocs = currentInstance.menu.groupViews(result, 'FormDesignTypeID');
                $.each(groupedDocs, function (ind, val) {
                    var group = $('<optgroup label="' + val[0].FormDesignTypeName + '" />');
                    $.each(val, function (index, item) {
                        // Disabled returned true to disable access permission against Design documents.
                        // var isOptionDisable = true;
                        // var isOptionDisable = false;
                        //isOptionDisable = !currentInstance.checkForUserRoleAccessPermissions(item, currentInstance);
                        $("<option class='section-menu-item nav-pills' value= " + item.FormInstanceId + '_' + item.FormDesignVersionID + "/>").html(item.FormDesignDisplayName).appendTo(group);
                        //$(currentInstance.elementIDs.viewdropdown).find('option:last').prop('disabled', isOptionDisable)
                    });
                    group.appendTo($(currentInstance.elementIDs.viewdropdown));
                });
                $(currentInstance.elementIDs.viewdropdown + " option").each(function (idx, control) {
                    var forminstid = $(control)[0].value.split('_');
                    if (forminstid[0] == currentInstance.formInstanceId) {
                        $(control).prop('selected', true);
                    }
                });

            });

            //Added this for hiding Invisible section as display:none doesn't work in IE
            $(currentInstance.elementIDs.sectionMenuJQ + " option").each(function (idx, control) {
                var display = $(this).css('display');
                if (display == 'none') {
                    var optionid = $(this).attr("id")
                    $(currentInstance.elementIDs.sectionMenuJQ + " option[id=" + optionid + "]").attr('disabled', 'disabled').remove();
                }
            });

            $(currentInstance.elementIDs.sectionMenuBottomJQ + " option").each(function (idx, control) {
                var display = $(this).css('display');
                if (display == 'none') {
                    var optionid = $(this).attr("id")
                    $(currentInstance.elementIDs.sectionMenuBottomJQ + " option[id=" + optionid + "]").attr('disabled', 'disabled').remove();
                }
            });


            if (currentInstance.IsMasterList) {
                var mlURL = currentInstance.URLs.getMasterListDocuments.replace(/\{formInstanceId\}/, currentInstance.formInstanceId);
                var promise = ajaxWrapper.getJSONSync(mlURL);
                var dataDDL = [];

                promise.done(function (docs) {
                    var masterListDocumentMenu = $(currentInstance.elementIDs.masterListDocumentMenuJQ);
                    var loadedDocumentName = currentInstance.designData.FormName;
                    $.templates({ documentMenuTmplt: { markup: currentInstance.elementIDs.mlDocumentMenuTemplateJQ } });
                    masterListDocumentMenu.html($.render.documentMenuTmplt(docs));
                    dataDDL = docs.Sections;
                    //Added this for hiding Invisible section as display:none doesn't work in IE
                    $(currentInstance.elementIDs.masterListDocumentMenuJQ + " option").each(function (idx, control) {
                        var display = $(this).css('display');
                        control.innerHTML = $(control).val().replace(/\n/g, '').trim();
                        var value = control.innerHTML;
                        //dataDDL.push(value);
                        if (loadedDocumentName && value) {
                            if (loadedDocumentName.replace(/\n/g, '').replace(/ /g, '') == control.id)
                                $(control).attr('selected', 'selected');
                        }
                        if (display == 'none') {
                            var optionid = $(this).attr("id")
                            $(currentInstance.elementIDs.masterListDocumentMenuJQ + " option[id=" + optionid + "]").attr('disabled', 'disabled').remove();
                        }
                    });

                    currentInstance.menu.registerMenuEvents();
                });

                $(currentInstance.elementIDs.sectionMenuContainerJQ + " select:eq(0)").autoCompleteDropDown({
                    select: function (event, ui) {
                        var inputEle = $(currentInstance.elementIDs.masterListDocumentMenuJQ).find('input');
                        if (inputEle != null) {
                            $(inputEle).off("focusout");
                        }
                        currentInstance.UserMasterListInstanceInit = false;
                        $('#masterListInstanceMenu').off("change");
                        $('#masterListInstanceMenuContainer').hide();
                        //var releaseSectionLock = '/FormInstance/ReleaseSectionLockonDocumentClose?formInstanceID={formInstanceID}&previousSectionName={sectionName}';
                        var releaseSectionLockUrl = currentInstance.URLs.releaseSectionLock;
                        //var sectioName = currentInstance + '.sections.' + currentInstance.selectedSection + '. FullName';
                        var sectioName = currentInstance.sections[currentInstance.selectedSection].FullName; //eval(sectioName);

                        releaseSectionLockUrl = releaseSectionLockUrl.replace("{sectionName}", sectioName);
                        releaseSectionLockUrl = releaseSectionLockUrl.replace("{formInstanceID}", currentInstance.formInstanceId);
                        var promise = ajaxWrapper.postJSON(releaseSectionLockUrl);
                        promise.done(function (result) {
                            if (result != null && result.length > 0) {
                            }
                        });

                        currentInstance.menu.confirmSaveForCurrentData(ui.item.id);
                    }
                });

                currentInstance.menu.formatViewDropDownML(dataDDL);

                if (currentInstance.designData.UsesAliasDesignMasterList == true) {
                    if (currentInstance.UserMasterListInstanceInit == false) {
                        //get list of document names and id's
                        var folderId = currentInstance.folderId;
                        var folderVersionId = currentInstance.folderVersionId;
                        var url = currentInstance.URLs.forminstancelisturl.replace(/\{folderVersionId\}/g, folderVersionId).replace(/\{folderId\}/g, folderId);
                        var promise = ajaxWrapper.getJSON(url);
                        var idx = 0;
                        promise.done(function (result) {
                            if (result != null && result.length > 0) {
                                //also match dropdown to current instance
                                currentInstance.UserMasterListInstanceInit = true;
                                $('#masterListInstanceMenu').find('option').remove();
                                $.each(result, function (idx, val) {
                                    $('#masterListInstanceMenu')
                                        .append($("<option></option>")
                                        .attr("value", val.FormInstanceID)
                                        .text(val.FormInstanceName));
                                });
                                $('#masterListInstanceMenu').find('option[value="' + currentInstance.formInstanceId + '"]').attr('selected', 'selected');
                                if (result.length > 1) {
                                    $("#masterListInstanceMenuContainer").show();
                                }
                                //register event for dropdown
                                $('#masterListInstanceMenu').on("change", function (ret) {
                                    //var sectioName = currentInstance + '.sections.' + currentInstance.selectedSection + '. FullName';
                                    var sectioName = currentInstance.sections[currentInstance.selectedSection].FullName; //eval(sectioName);
                                    var releaseSectionLockUrl = currentInstance.URLs.releaseSectionLock;
                                    releaseSectionLockUrl = releaseSectionLockUrl.replace("{sectionName}", sectioName);
                                    releaseSectionLockUrl = releaseSectionLockUrl.replace("{formInstanceID}", currentInstance.formInstanceId);
                                    var promise = ajaxWrapper.postJSON(releaseSectionLockUrl);
                                    promise.done(function (result) {
                                        if (result != null && result.length > 0) {
                                        }
                                    });
                                    currentInstance.formInstanceId = ret.target.value;
                                    var sectionName = "";
                                    currentInstance.formName = $("#masterListInstanceMenu option:selected").text();
                                    $.each(currentInstance.sections, function (idx, val) {
                                        if (sectionName == "") {
                                            sectionName = val.FullName;
                                        }
                                    });
                                    currentInstance.selectedSection = sectionName;
                                    currentInstance.loadFormInstanceDesignData(true, sectionName, false);
                                });
                            }
                        });
                    }
                }
            }
            else {
                currentInstance.menu.registerMenuEvents();
            }



            //trigger rules for Sections
            currentInstance.rules.processRulesForRootSections();
            currentInstance.menu.clearMenuSelection();
            //set the currently selected section
            if (currentInstance.formDesignId == 2409) {
                currentInstance.selectedSection = undefined;
            }
            if (currentInstance.selectedSection === undefined) {
                currentInstance.menu.selectFirstSection();
                currentInstance.form.renderSection(currentInstance.selectedSection, true);
            }
            else {
                currentInstance.menu.setSectionMenuSelection(currentInstance.selectedSection);
            }
            currentInstance.errorManager.loadErrorGridData(currentInstance.errorGridData);
            try {
                exitValidate.clearExitValidateResults();
                $(currentInstance.elementIDs.btnExitValidateResultsJQ).unbind('click');
                $(currentInstance.elementIDs.btnExitValidateResultsJQ).bind('click', function () {
                    exitValidate.loadExitValidateResults(currentInstance.formInstanceId);
                });
            }
            catch (e) {
            }
        },
        confirmSaveForCurrentData: function (value) {
            var isEditableML = false;
            isEditableML = checkUserPermissionForEditable(currentInstance.URLs.masterlisturl);

            var message = '';
            if (currentInstance != null && currentInstance.form.hasChanges() && isEditableML) {
                message = message + Common.changeMasterListView;
                yesNoConfirmDialog.show(message, function (e) {
                    yesNoConfirmDialog.hide();

                    if (e) {
                        if (message == Common.changeMasterListView) {
                            currentInstance.menu.loadViewML(value, true);
                        }
                    }
                    else
                        currentInstance.menu.loadViewML(value, false);
                });
            }
            else
                currentInstance.menu.loadViewML(value, false);
        },
        formatViewDropDownML: function (dataDDL) {
            var spanCustomBox = $(currentInstance.elementIDs.masterListDocumentMenuJQ).find('span[class="custom-combobox"]');
            $(spanCustomBox).removeClass('custom-combobox').addClass('custom-combobox');
            //if (spanCustomBox.length > 0)
            //    $(spanCustomBox[0]).css('width', '180px !important')

            var inputEle = $(currentInstance.elementIDs.masterListDocumentMenuJQ).find('input');

            $(inputEle).removeClass('custom-combobox-input ui-widget ui-widget-content ui-state-default ui-corner-left ui-autocomplete-input').addClass('section-menu');

            $(inputEle).css('width', '100%');

            if (inputEle.length > 0) {
                $(inputEle).on('focusout', function () {
                    var value = $(inputEle).val();
                    var valueFromArray = dataDDL.filter(function (obj, i) { return value == obj.Label && currentInstance.designData.FormName != obj.Name })
                    if (valueFromArray.length > 0)
                        currentInstance.menu.confirmSaveForCurrentData(valueFromArray[0].Name);
                });
            }

            var anchorHousingTriangleIcon = $(currentInstance.elementIDs.masterListDocumentMenuJQ).find('a');
            //$(inputEle).css('display', 'inline');                                //  doesnot work for ggogle chrome so using the code line given below
            $(anchorHousingTriangleIcon).css('margin-left', $(inputEle).css('width'));
            $(anchorHousingTriangleIcon).css({ 'border': '1px solid gray', 'width': '18px', 'border-left-width': '0px !important;' });

            var triangleIcon = $(currentInstance.elementIDs.masterListDocumentMenuJQ).find('span[class="ui-button-icon-primary ui-icon ui-icon-triangle-1-s"]');
            var IsJQLoadedEle = $(currentInstance.elementIDs.IsJQLoaded);
            if (IsJQLoadedEle) {
                if (IsJQLoadedEle.html() != null) {
                    var IsJQLoadedText = IsJQLoadedEle.html().toUpperCase();
                    var IsJQLoaded = IsJQLoadedText === "TRUE" || !IsJQLoadedText ? true : false;
                    currentInstance.IsJQLoaded = IsJQLoaded;
                    if (triangleIcon.length > 0 && !IsJQLoaded)
                        $(triangleIcon[0]).css('margin-top', '-6px !important').css('margin-left', '-4px !important');
                }
            }
        },

        // Save current View(Folder) Data and load new View(Folder).
        loadViewML: function (value, shouldSaveCurrentData) {
            var folderIconsSeparatorJQ = $(currentInstance.elementIDs.folderIconsSeparatorJQ);
            var btnStatusUpdateJQ = folderIconsSeparatorJQ.find(currentInstance.elementIDs.btnStatusUpdateJQ);
            btnStatusUpdateJQ.remove();

            //  save the data for current view and move forward with loading the new View by calling loadNewViewML method from within saveFormInstanceData.
            if (currentInstance.isfolderReleased == false && currentInstance.IsFormInstanceEditable == true && shouldSaveCurrentData) {
                currentInstance.form.saveFormInstanceData(false, true, value);
            }
                //  saving not required for current view, just load the new View by calling loadNewViewML method.
            else {
                if ((value != null && value != undefined))
                    currentInstance.menu.loadNewViewML(value);
            }

        },

        // load the new View as per the new View selected from ML View DrpDwn.
        loadNewViewML: function (value) {

            if (value) {
                if (value !== null && value !== undefined) {
                    var url = currentInstance.URLs.getFolderVersionViewModel.replace(/{folderType}/, encodeURIComponent(value));
                    var promise = ajaxWrapper.getJSONSync(url);

                    promise.done(function (result) {

                        var folderData_NEW = {
                            tenantId: result.TenantID,
                            folderId: result.FolderId,
                            folderVersionId: result.FolderVersionId,
                            accountId: result.AccountId,
                            effectiveDate: result.EffectiveDate,
                            versionNumber: result.FolderVersionNumber,
                            isPortfolio: result.IsPortfolio == true ? 'True' : 'False',
                            isEditable: result.IsEditable == true ? 'True' : 'False',
                            folderName: result.FolderName,
                            folderType: result.FolderType,
                            referenceProductFormInstanceID: result.ReferenceProductFormInstanceID,
                            versionType: result.VersionTypeID,
                            folderVersionState: result.FolderVersionStateID,
                            isReleased: result.IsReleased == true ? 'True' : 'False',
                            formDesignVersionType: result.FormDesignVersionType,
                            isAutoSaveEnabled: result.IsAutoSaveEnabled == true ? 'True' : 'False',
                            duration: result.Duration,
                            autoSaveSettingsPropertites: result.AutoSaveSettingsProperties,
                            isLocked: result.IsLocked == true ? 'True' : 'False',
                            lockedBy: result.LockedBy,
                            isNewVersionLoaded: result.IsNewVersionLoaded == true ? 'True' : 'False',
                            isNewLoadedVersionIsMajorVersion: result.IsNewLoadedVersionIsMajorVersion == true ? 'True' : 'False',
                            currentUserName: result.CurrentUserName,
                            WFStateID: result.WFStateID,
                            currentUserId: result.CurrentUserId,
                            RoleId: result.RoleId,
                            CategoryName: result.CategoryName,
                            CatID: result.CatID,
                            WFStateName: result.WFStateName,
                            currentUserId: result.CurrentUserId,
                            currentUserName: result.CurrentUserName
                        };

                        // replace the existing folderData object created in IndexML with the folderData_NEW for the new View(Folder). folderData is referreed by most of the dialogs in the Menu.
                        folderData = folderData_NEW;

                        // creating singleton folderVersion instance for the new View(Folder).
                        $(document).ready(function () {
                            var singleTonfolderVersionInstance = new folderVersionInstance.getInstance(folderData_NEW.accountId, folderData_NEW.folderVersionId, folderData_NEW.versionNumber, folderData_NEW.effectiveDate, folderData_NEW.folderId, folderData_NEW.referenceProductFormInstanceID, folderData_NEW.isEditable, folderData_NEW.folderType, 'Default', folderData_NEW.currentUserId, folderData_NEW.currentUserName);
                            singleTonfolderVersionInstance.load();

                            var date = new Date(result.EffectiveDate);
                            var shortDate = date.getMonth() + 1 + '/' + date.getDate() + '/' + date.getFullYear();
                            shortDate = !shortDate || shortDate == null ? '' : shortDate;
                            $(currentInstance.elementIDs.mLView_accountName).html('<b>Account:</b>' + result.AccountName);
                            $(currentInstance.elementIDs.mLView_folderName).html(result.FolderName + ' <span class="form-subtitle new-subtitle" id="formTitle_effectiveDate">' + shortDate + ', Version No. ' + result.FolderVersionNumber + '</span>');
                            //$(currentInstance.elementIDs.mLView_effectiveDate).html(shortDate + ', Version No. ' + result.FolderVersionNumber);
                            $(currentInstance.elementIDs.mLView_vesrionHistoryDialogML).html(result.FolderName);

                        });
                    });
                }
            }

        },

        //registers events for the menu. 
        registerMenuEvents: function () {
            $(currentInstance.elementIDs.sectionMenuContainerJQ).unbind('click');
            $(currentInstance.elementIDs.sectionMenuContainerJQ).on("click", function () {
                currentInstance.menu.slideInSectionMenu();
            });

            if (currentInstance.IsMasterList) {
                var folderIconsSeparatorJQ = $(currentInstance.elementIDs.folderIconsSeparatorJQ);
                var btnStatusUpdateJQ = folderIconsSeparatorJQ.find(currentInstance.elementIDs.btnStatusUpdateJQ);
                btnStatusUpdateJQ.remove();

                //$(currentInstance.elementIDs.sectionMenuContainerJQ + " select:eq(0)").on('change', function () {
                //    if (currentInstance.isfolderReleased == false && currentInstance.isFolderLocked == false) {
                //        currentInstance.form.saveFormInstanceData(false, true);
                //    }

                //    var formDesignName = $(this).children(":selected").attr("id");
                //    if (formDesignName) {
                //        if (formDesignName !== null && formDesignName !== undefined) {
                //            var url = currentInstance.URLs.masterlisturl.replace(/{folderType}/, formDesignName);
                //            //currentInstance = null;
                //            window.location.href = url;
                //            //currentInstance = null;
                //        }
                //    }
                //});


                $(currentInstance.elementIDs.sectionMenuContainerJQ + " select:eq(2)").on('change', function () {
                    if (currentInstance.isfolderReleased == false && currentInstance.isFolderLocked == false) {
                        currentInstance.form.saveSectionData(currentInstance.selectedSection, $(this).children(":selected").attr("id"));
                    }
                    else {
                        currentInstance.selectedSection = $(this).children(":selected").attr("id");
                        currentInstance.form.getSectionDataFromServer();
                    }
                });
            }
            else {
                $(currentInstance.elementIDs.sectionMenuContainerJQ + " select.section-menu").on('change', function () {
                    if (currentInstance.isfolderReleased == false && currentInstance.IsFormInstanceEditable == true) {
                        currentInstance.form.saveSectionData(currentInstance.selectedSection, $(this).children(":selected").attr("id"));
                    }
                    else {
                        currentInstance.selectedSection = $(this).children(":selected").attr("id");
                        currentInstance.form.getSectionDataFromServer();
                    }
                });

            }
            //folderIconsSeparatorJQ.find(currentInstance.elementIDs.btnStatusUpdateJQ).remove();

            $(currentInstance.elementIDs.sectionMenuContainerJQ).css("left", "-247px");
            //For bottom menu
            $(currentInstance.elementIDs.sectionMenuContainerBottomJQ).unbind('click');
            $(currentInstance.elementIDs.sectionMenuContainerBottomJQ).on("click", function () {
                currentInstance.menu.slideInSectionMenu();
            });

            $(currentInstance.elementIDs.sectionMenuContainerBottomJQ + " select").on('change', function () {
                if (currentInstance.isfolderReleased == false && currentInstance.IsFormInstanceEditable == true) {
                    currentInstance.form.saveSectionData(currentInstance.selectedSection, $(this).children(":selected").attr("id"));
                }
                else {
                    currentInstance.selectedSection = $(this).children(":selected").attr("id");
                    currentInstance.form.getSectionDataFromServer();
                }
            });

        },

        //sets the specified sectionName 
        setSectionMenuSelection: function (sectionName, callbackMetaData) {
            try {

                currentInstance.menu.clearMenuSelection();
                currentInstance.selectedSection = sectionName;
                if (currentInstance.sections[sectionName] == undefined) {
                    currentInstance.menu.selectFirstSection();
                    currentInstance.form.renderSection(currentInstance.selectedSection, true);
                }
                else if (currentInstance.sections[sectionName] != undefined && currentInstance.sections[sectionName].IsLoaded == false) {
                    currentInstance.form.renderSection(sectionName, true, callbackMetaData);
                }
                else {
                    currentInstance.form.renderSection(sectionName, true);
                }
                $(currentInstance.elementIDs.sectionMenuJQ + " option").each(function (idx, control) {
                    if ($(control).attr("id") == sectionName) {
                        $(control).prop('selected', true);
                    }
                });
                $(currentInstance.elementIDs.sectionMenuBottomJQ + " option").each(function (idx, control) {
                    if ($(control).attr("id") == sectionName) {
                        $(control).prop('selected', true);
                    }
                });
            } catch (e) {
                console.log(e + " function : setSectionMenuSelection()");
            }
        },

        setSectionMenuSelectionScroll: function () {
            var scrollTo = $(currentInstance.elementIDs.sectionMenuJQ + " li.menu-item-active");
            var container = $(currentInstance.elementIDs.sectionMenuJQ);

            if (scrollTo != null && scrollTo.length > 0) {
                container.animate({
                    scrollTop: scrollTo.offset().active
                });
            }

            //Bottom
            var scrollTo1 = $(currentInstance.elementIDs.sectionMenuBottomJQ + " li.menu-item-active");
            var container1 = $(currentInstance.elementIDs.sectionMenuBottomJQ);

            if (scrollTo1 != null && scrollTo1.length > 0) {
                container1.animate({
                    scrollTop: scrollTo1.offset().active
                });
            }
        },

        //selects the first section in menu
        selectFirstSection: function () {
            currentInstance.menu.clearMenuSelection();
            var firstSelected = false;
            $(currentInstance.elementIDs.sectionMenuJQ + " option").each(function (idx, control) {
                var display = $(this).css('display');
                if (display == 'block' && firstSelected == false) {
                    if (currentInstance.IsMasterList) {
                        currentInstance.selectedSection = $(currentInstance.elementIDs.sectionMenuContainerJQ + " select:eq(2)").children(":selected").attr("id");
                    }
                    else {
                        currentInstance.selectedSection = $(currentInstance.elementIDs.sectionMenuContainerJQ + " select").children(":selected").attr("id");
                    }
                    $(this).addClass("menu-item-active");
                    currentInstance.menu.setSectionMenuSelectionScroll();
                    firstSelected = true;
                }

            });

            //bottom menu
            $(currentInstance.elementIDs.sectionMenuBottomJQ + " option").each(function (idx, control) {
                var display = $(this).css('display');
                if (display == 'block' && firstSelected == false) {
                    currentInstance.selectedSection = $(currentInstance.elementIDs.sectionMenuContainerBottomJQ + " select").children(":selected").attr("id");
                    $(this).addClass("menu-item-active");
                    currentInstance.menu.setSectionMenuSelectionScroll();
                    firstSelected = true;
                }
            });
        },

        //clears all the selection in section menu
        clearMenuSelection: function () {
            $(currentInstance.elementIDs.sectionMenuJQ + " option").each(function (idx, control) {
                $(this).removeClass("menu-item-active");
                $(this).removeClass("active");
            });
            $(currentInstance.elementIDs.sectionMenuBottomJQ + " option").each(function (idx, control) {
                $(this).removeClass("menu-item-active");
                $(this).removeClass("active");
            });
        },

        //handles the slide in effect for the menu
        slideInSectionMenu: function () {
            if ($(currentInstance.elementIDs.sectionMenuContainerJQ).position().left == -247) {
                $(currentInstance.elementIDs.sectionMenuContainerJQ).css("left", "-21px");
                $(currentInstance.elementIDs.sectionMenuContainerJQ).css("z-index", "1001");
            } else {
                $(currentInstance.elementIDs.sectionMenuContainerJQ).css("left", "-247px");
                $(currentInstance.elementIDs.sectionMenuContainerJQ).css("z-index", "999");
            }

            if ($(currentInstance.elementIDs.sectionMenuContainerBottomJQ).position().left == -247) {
                $(currentInstance.elementIDs.sectionMenuContainerBottomJQ).css("left", "-21px");
                $(currentInstance.elementIDs.sectionMenuContainerBottomJQ).css("z-index", "1001");
            } else {
                $(currentInstance.elementIDs.sectionMenuContainerBottomJQ).css("left", "-247px");
                $(currentInstance.elementIDs.sectionMenuContainerBottomJQ).css("z-index", "999");
            }
        },
        hideSection: function (sectionName) {
            $(currentInstance.elementIDs.sectionMenuJQ + " option").each(function (idx, control) {
                var menuId = $(this).attr('id');
                if (menuId == sectionName) {
                    $(this).css('display', 'none');
                }
            });
            $(currentInstance.elementIDs.sectionMenuBottomJQ + " option").each(function (idx, control) {
                var menuId = $(this).attr('id');
                if (menuId == sectionName) {
                    $(this).css('display', 'none');
                }
            });
        },
        showSection: function (sectionName) {
            $(currentInstance.elementIDs.sectionMenuJQ + " option").each(function (idx, control) {
                var menuId = $(this).attr('id');
                if (menuId == sectionName && $(this).css('display') != 'block') {
                    $(this).css('display', 'block');
                }
            });
            $(currentInstance.elementIDs.sectionMenuBottomJQ + " option").each(function (idx, control) {
                var menuId = $(this).attr('id');
                if (menuId == sectionName) {
                    $(this).css('display', 'block');
                }
            });
        },

    }
}

formInstanceBuilder.prototype.autoSaveMethods = function () {
    var currentInstance = this;
    return {

        InitializeAutoSave: function () {
            try {
                if (typeof (Worker) !== "undefined" && currentInstance.autoSaveWorker != undefined) {
                    currentInstance.autoSaveWorker.onmessage = function (e) {
                        if (e.data.formInstanceId == currentInstance.formInstanceId) {
                            // console.log('setTimeoutforAutoSave called');
                            currentInstance.autosave.setAutoSaveTimeout();
                        }
                    };
                    currentInstance.autoSaveWorker.onerror = function (err) {
                        console.log(AutoSave.workerStatus, err);
                    }
                    currentInstance.autosave.setAutoSaveTimeout();
                }
                else {
                    console.log(AutoSave.checkWebWorkerStatus);
                }
            } catch (ex) {
                console.log(ex);
            }
        },
        //Data saved at a given specific duration automatically
        setAutoSaveTimeout: function () {
            if (currentInstance.folderData.isAutoSaveEnabled == "True") {
                if (currentInstance.autoSaveTimer != undefined) {
                    currentInstance.autosave.clearAutoSaveTimeOut();
                }
                currentInstance.autoSaveTimer = setTimeout(function () {
                    //save data
                    currentInstance.autosave.postMessageAndSaveData();
                }, currentInstance.folderData.duration * 1000 * 60);
            }
        },
        //Data saved After switching from one tab to another after confirmation
        postMessageAndSaveData: function () {
            // Focus out element having focus, so that its value can be saved.
            var elementHavingFocus = $(':focus');
            if ($(elementHavingFocus).is("input") || $(elementHavingFocus).is("select") || $(elementHavingFocus).is("textarea")) {
                $("#" + $(elementHavingFocus).attr("ID")).trigger('change');
            }

            if (currentInstance.folderData.isAutoSaveEnabled == "True") {
                if (currentInstance != null && currentInstance.form.hasChanges()) {
                    var formInstanceData = currentInstance.form.getFormInstanceData();
                    currentInstance.autoSaveWorker.postMessage({
                        url: currentInstance.URLs.saveFormInstanceData,
                        formInstanceId: currentInstance.formInstanceId,
                        saveData: JSON.stringify(formInstanceData),
                    });

                    currentInstance.autoSaveWorker.onmessage = function (e) {
                        if (e.data != undefined) {
                            //call to save activity log data method.
                            currentInstance.form.saveFormInstanceActivityLogData();
                            if (ActiveRuleExecutionLogger == "True") {
                                ruleExecutionLogger.saveFormInstanceRuleExecutionLogData(currentInstance);
                            }
                        }
                    }


                    //show alert
                    currentInstance.autosave.showAutoSaveAlert();
                    //reset hasChanges flag
                    currentInstance.hasChanges = false;
                }
                else if (currentInstance != null) {
                    currentInstance.autosave.setAutoSaveTimeout();
                }
            }
        },



        showAutoSaveAlert: function () {
            $(currentInstance.elementIDs.folderAutoSaveAlertJQ).show();
            $(currentInstance.elementIDs.folderAutoSaveAlertJQ).fadeOut(6000);
            if (!$(currentInstance.elementIDs.folderAutoSaveAlertJQ).is('.fin')) {
                $(currentInstance.elementIDs.folderAutoSaveAlertJQ).addClass('fin');
                setTimeout(function () {
                    $(currentInstance.elementIDs.folderAutoSaveAlertJQ).removeClass('fin');
                }, 6000);
            }
        },
        clearAutoSaveTimeOut: function () {
            clearTimeout(currentInstance.autoSaveTimer);
        },
    }
}

formInstanceBuilder.prototype.registerViewddevent = function () {
    var currentInstance = this, lastValue;
    $(currentInstance.elementIDs.sectionMenuContainerJQ + " select.view-menu").off('change');
    $(currentInstance.elementIDs.sectionMenuContainerJQ + " select.view-menu").focus(function (e) {
        lastValue = $(this).val();
    }).on('change', function (e, data) {
        //currentInstance.changeView(lastValue);        
        var message = "", message1 = "", message2 = "";
        if (currentInstance != null && currentInstance.form.hasChanges()) {
            message = message + Common.changeView;
        }
        else {
            message = message + Common.confirmchangeView;
        }
        yesNoConfirmDialog.show(message, function (e) {
            yesNoConfirmDialog.hide();
            if (e) {
                if (message == Common.changeView) {
                    currentInstance.form.saveFormInstanceData(false, false, undefined, true, lastValue);
                } else {
                    reloadData = false;
                    sectionName = "";
                    //----
                    var releaseViewLockUrl = currentInstance.URLs.releaseViewLockUrl;
                    var formInstanceIDs = $(currentInstance.elementIDs.sectionMenuContainerJQ + " select.view-menu")[0].value.split('_');

                    releaseViewLockUrl = releaseViewLockUrl.replace("{formInstanceID}", currentInstance.formInstanceId);
                    releaseViewLockUrl = releaseViewLockUrl.replace("{newFormInstanceId}", formInstanceIDs[0]);
                    var promise = ajaxWrapper.postJSON(releaseViewLockUrl);
                    promise.done(function (result) {
                        if (result != null && result.length > 0) {
                        }
                    });
                    //----
                    if (data != undefined && data.sectionName != undefined) {
                        currentInstance.selectedSection = data.sectionName;
                        sectionName = data.sectionGeneratedName;
                    }

                    currentInstance.cleanup();
                    currentInstance.formDesignVersionId = formInstanceIDs[1];
                    currentInstance.formInstanceId = formInstanceIDs[0];
                    currentInstance.accessPermissions = currentInstance.accessPermissionsMethods();
                    var callbackfunc = null;
                    if (data != null) {
                        callbackfunc = data.callback;
                    }
                    currentInstance.loadFormInstanceDesignData(reloadData, sectionName, false, callbackfunc);
                    var folder = folderManager.getInstance().getFolderInstance();
                    folder.loadAllFormsStatus(currentInstance.folderVersionId);
                }
            }
            else {
                if (message == Common.confirmchangeView) {
                    $(currentInstance.elementIDs.sectionMenuContainerJQ + " select.view-menu").val(lastValue);
                }
                else {
                    reloadData = false,
                    sectionName = "";
                    var formInstanceIDs = $(currentInstance.elementIDs.sectionMenuContainerJQ + " select.view-menu")[0].value.split('_');
                    currentInstance.cleanup();
                    currentInstance.formDesignVersionId = formInstanceIDs[1];
                    currentInstance.formInstanceId = formInstanceIDs[0];
                    currentInstance.loadFormInstanceDesignData(reloadData, sectionName, false)
                    var folder = folderManager.getInstance().getFolderInstance();
                    folder.loadAllFormsStatus(currentInstance.folderVersionId);
                }
            }

        });

    });
}

formInstanceBuilder.prototype.formMethods = function () {
    var currentInstance = this;

    return {
        initSections: function () {
            if (currentInstance.designData != null && currentInstance.designData.Sections != null && currentInstance.designData.Sections.length > 0) {
                $.each(currentInstance.designData.Sections, function (index, item) {
                    currentInstance.sections[item.Name] = { IsLoaded: false, SectionName: item.Label, FullName: item.FullName, SectionID: item.Name };
                });
            }
        },
        renderAllSections: function () {
            for (var prop in currentInstance.sections) {
                if (currentInstance.sections[prop].IsLoaded == false) {
                    currentInstance.form.renderSection(prop, false);
                }
            }
        },
        renderSectionAfterLock: function (selectedSection, makeVisible, callbackMetaData) {
            try {
                if (selectedSection != null) {
                    if (makeVisible == true) {
                        currentInstance.selectedSectionName = currentInstance.sections[selectedSection].SectionName;
                    }
                    var sectionData = currentInstance.form.getSectionData(selectedSection);
                    if (currentInstance.sections[selectedSection].IsLoaded == false) {
                        if (sectionData != null) {
                            //render template for section
                            $.templates({ sectionTmpl: { markup: currentInstance.elementIDs.sectionTemplateJQ, layout: true } });
                            // add unique response as a option in DropDownTextBox element
                            currentInstance.form.setDropDownTextBoxOption(sectionData.Elements, currentInstance.formData, sectionData.FullName);

                            //first run to generate html
                            var sectionHtml = $.render.sectionTmpl(sectionData, { repeaterCallback: currentInstance.form.renderRepeater, hasRequiredValidation: currentInstance.formValidationManager.hasRequiredValidation, getFormInstanceId: currentInstance.form.getFormInstanceId, setCheckBoxDataLink: currentInstance.setCheckBoxDataLink, setRadioButtonDataLink: currentInstance.setRadioButtonDataLink, renderCustomHtml: currentInstance.form.renderCustomHtml, isLabelValue: currentInstance.rules.isLabelValue });
                            //second run to render with data binding
                            var tmpl = $.templates(sectionHtml);
                            var data = {};
                            var wrapperSectionId = selectedSection + currentInstance.formInstanceId + "wrapper";
                            var sectionDiv = "<div id='" + wrapperSectionId + "'>";
                            $(currentInstance.formInstanceDivJQ).append(sectionDiv);
                            data[sectionData.FullName] = currentInstance.formData[sectionData.FullName];
                            tmpl.link("#" + wrapperSectionId, data);

                            currentInstance.form.setAutoCompleteToFilterDropdown(selectedSection, sectionData.Elements);

                            currentInstance.dropDownTextBox.updateAndToggleDropDownTextBox();
                            if (folderData.isEditable == "True") {
                                currentInstance.form.sectionControlFocusOut();
                            }
                            //load grids

                            currentInstance.currentSectionRepeaterIDs = [];
                            if (currentInstance.gridDesigns != null && currentInstance.gridDesigns.length > 0) {
                                currentInstance.repeaterBuilders = currentInstance.repeaterBuilders.filter(function (item) {
                                    return item.fullName.split('.')[0] != sectionData.FullName;
                                });
                                var ctr = 1;

                                var userSettingInstance = new userSettings(currentInstance, currentInstance.getRepeaterUserSettingData(currentInstance.selectedSection));
                                var userSettingRepeaterChoice = userSettingInstance.getUserSectionChoiceData(userSettingInstance.commonConst.repeaterRender);

                                //  console.log(userSettingRepeaterChoice);

                                $.each(currentInstance.gridDesigns, function (idx, item) {
                                    //if (currentInstance.customRules.hasProduct(currentInstance.formDesignId) && currentInstance.customRules.isProductRepeaterNotToBeRender(item.Repeater.FullName)) {
                                    //    return false;
                                    //}

                                    if (item.Visible == true) {

                                        var rulesForRepeater = currentInstance.rules.getRulesInContainerElement(item.Repeater.FullName);

                                        // if (checkIfPQGridLoaded()) {
                                        var isEditable = currentInstance.isHiddenOrDisableSections;

                                        if (isEditable == true) {
                                            isEditable = currentInstance.accessPermissions.checkEditPermission(sectionData);
                                        }
                                        var repeater = {};
                                        /*    if (currentInstance.IsMasterList == false && GridDisplayMode.PQ == CurrentGridDisplayMode) {
                                                repeater = new repeaterBuilderPQ(item.Repeater, getDataProperty(item.Repeater.FullName, currentInstance.formData),
                                                    item.Repeater.FullName, currentInstance.formInstanceId, 'parent', '', currentInstance, rulesForRepeater,
                                                    currentInstance.ruleProcessor);
                                            }
                                            else {*/
                                        //if (currentInstance.designData.FormDesignId != FormTypes.MEDICALFORMDESIGNID) {
                                        repeater = new repeaterBuilderAG(item.Repeater, getDataProperty(item.Repeater.FullName, currentInstance.formData),
                                            item.Repeater.FullName, currentInstance.formInstanceId, 'parent', '', currentInstance, rulesForRepeater,
                                            currentInstance.ruleProcessor, isEditable, currentInstance.currentUserId, currentInstance.currentUserName);

                                        //  }
                                        //}
                                        //else {
                                        //    var repeater = new repeaterBuilderMedicalPQ(item.Repeater, getDataProperty(item.Repeater.FullName, currentInstance.formData),
                                        //    item.Repeater.FullName, currentInstance.formInstanceId, 'parent', '', currentInstance, rulesForRepeater,
                                        //    currentInstance.ruleProcessor);
                                        //}
                                        //}

                                        currentInstance.repeaterBuilders.push(repeater);
                                        var btn = '#btn' + item.Repeater.Name + currentInstance.formInstanceId;

                                        if (userSettingInstance.commonConst.RenderAll != userSettingRepeaterChoice && currentInstance.IsMasterList == true) {

                                            if (ctr <= 1) {
                                                $(btn).css('display', 'none');
                                                $(btn).removeClass('btn btn-secondary btn-lg btn-block');
                                                $(btn).hide();
                                                repeater.build();
                                            }
                                            else {
                                                $(btn).val(item.Repeater.Label);
                                                $(document).off('click', btn);
                                                $(document).on('click', btn, function () {
                                                    //remove the HTML 
                                                    var repeaterId = '#' + item.Repeater.Name + currentInstance.formInstanceId;
                                                    $(repeaterId).empty();
                                                    repeater.build();
                                                    $(this).removeClass('btn btn-secondary btn-lg btn-block');
                                                    $(this).hide();
                                                });
                                            }
                                        }
                                        else {
                                            $(btn).css('display', 'none');
                                            $(btn).removeClass('btn btn-secondary btn-lg btn-block');
                                            $(btn).hide();
                                            repeater.build();
                                        }
                                        ctr++;

                                        currentInstance.currentSectionRepeaterIDs.push(item.Repeater.Name + currentInstance.formInstanceId);
                                    }
                                    else {
                                        var btn = '#btn' + item.Repeater.Name + currentInstance.formInstanceId;
                                        $(btn).css('display', 'none');
                                        $(btn).removeClass('btn btn-secondary btn-lg btn-block text-left');
                                        $(btn).hide();
                                    }
                                });
                                currentInstance.gridDesigns = [];
                            }

                            if (makeVisible == true) {
                                //set the current selected section as visible
                                currentInstance.form.setSectionSelection(currentInstance.selectedSection);
                            }
                            //method to register the datepicker events.
                            currentInstance.form.setDatePickers(selectedSection);
                            currentInstance.form.setChildPopup(currentInstance);
                            currentInstance.form.setRichTextFormat(selectedSection);

                            //currentInstance.sections[selectedSection].IsLoaded = true;

                            //set selected elements as [Select One] where no value is selected for dropdowns.
                            currentInstance.form.setValueToEmptyDropdown(selectedSection);


                            var sectionDesignData = currentInstance.form.getSectionData(selectedSection);
                            currentInstance.form.setSectionVisibleProperty(sectionDesignData);
                            currentInstance.form.setSectionEnableProperty(sectionDesignData);

                            if (currentInstance.expressionBuilder.hasExpressionBuilderRule(currentInstance.designData.FormDesignId)) {
                                currentInstance.expressionBuilder.onSectionLoad(currentInstance);
                            }
                        }

                        //registers ToolTip events
                        currentInstance.form.registerToolTip(selectedSection);
                        //run rules for the section - values first
                        //currentInstance.rules.processRulesOnElements(sectionData.FullName, 'VALUE');
                        currentInstance.rules.processRulesOnElements(sectionData.FullName, 'NONVALUE');
                        //Reset Grid Top Pager Width
                        $(currentInstance.formInstanceDivJQ + " .ui-jqgrid").each(function (index, grid) {
                            var gridID = $(grid).attr("ID").substring(5);
                            if ($(grid).find('#t_' + gridID).length > 0) {
                                //var width = $($(grid).parent()).innerWidth() - 20;
                                var width = $($("#" + gridID).parent().closest('div').first()).width();
                                $('#t_' + gridID).css('width', width);
                            }
                        });
                        //End

                        if (currentInstance.isHiddenOrDisableSections) {
                            currentInstance.accessPermissions.applyAccessPermission(sectionData);
                        }
                        currentInstance.accessPermissions.handleReleasedState(currentInstance.isfolderReleased, sectionData);
                        if (currentInstance.folderData.isPortfolio.toLowerCase() == "true" && currentInstance.isDocumentEditable.toLowerCase() == "true") {
                            currentInstance.accessPermissions.handleFolderLockState(!currentInstance.IsFormInstanceEditable, sectionData);
                            if (currentInstance.IsFormInstanceEditable == false) {
                                currentInstance.accessPermissions.disableMenuLevelButtons();
                                //$(currentInstance.elementIDs.btnCreateFormJQ).addClass('disabled-button');
                            }
                            else {
                                currentInstance.accessPermissions.enableMenuLevelButtons();
                                //$(currentInstance.elementIDs.btnCreateFormJQ).removeClass('disabled-button');
                                if (vbRole == Role.ProductSME && currentInstance.formInstanceId == currentInstance.anchorFormInstanceID) {
                                    $(currentInstance.elementIDs.btnSaveFormDataJQ).addClass('disabled-button');
                                    $(currentInstance.elementIDs.btnBottomSaveFormDataJQ).addClass('disabled-button');
                                    $(currentInstance.elementIDs.btnBottomCreateNewVersion).addClass('disabled-button');
                                    $(currentInstance.elementIDs.btnValidateJQ).addClass('disabled-button');
                                    $(currentInstance.elementIDs.btnValidateSectionJQ).addClass('disabled-button');
                                    $(currentInstance.elementIDs.btnJournalEntryJQ).addClass('disabled-button');
                                }
                            }
                        }

                        if (folderData.folderType == 'MasterList') {
                            var folder = folderManager.getInstance().getFolderInstance();

                            //if (!(folderData.WFStateID == WorkFlowState.SetUp || folderData.WFStateID == WorkFlowState.ProductBuild)) {
                            //    currentInstance.accessPermissions.handleDocumentTraslationState(true, sectionData);
                            //}
                            var isEnable = false;
                            if (folder.allProductStatusInFolder[currentInstance.formInstanceId] != undefined && ((folder.allProductStatusInFolder[currentInstance.formInstanceId].IsProductInTranslation != undefined && folder.allProductStatusInFolder[currentInstance.formInstanceId].IsProductInTranslation)
                                || (folder.allProductStatusInFolder[currentInstance.formInstanceId].IsProductInTransmission != undefined && folder.allProductStatusInFolder[currentInstance.formInstanceId].IsProductInTransmission))) {
                                currentInstance.accessPermissions.handleDocumentTraslationState(true, sectionData);
                                currentInstance.accessPermissions.disableDocumentLevelButtons();
                                isEnable = true;
                            }
                            if (currentInstance.folderData.isLocked == 'False' && currentInstance.folderData.isReleased == 'False' && isEnable == false)
                                isEnable = true;
                            else
                                isEnable = false;
                            currentInstance.accessPermissions.disableDocumentLevelMenuML(isEnable);
                        }
                        else {
                            var folder = folderManager.getInstance().getFolderInstance();
                            if (folder.allProductStatusInFolder[currentInstance.formInstanceId] != undefined) {

                                if (folder.allProductStatusInFolder[currentInstance.formInstanceId].IsProductInTranslation != undefined && folder.allProductStatusInFolder[currentInstance.formInstanceId].IsProductInTranslation) {
                                    currentInstance.accessPermissions.handleDocumentTraslationState(true, sectionData);
                                    currentInstance.accessPermissions.disableDocumentLevelButtons();
                                }
                                else if (folder.allProductStatusInFolder[currentInstance.formInstanceId].IsProductInTransmission != undefined &&
                                    folder.allProductStatusInFolder[currentInstance.formInstanceId].IsProductInTransmission) {
                                    currentInstance.accessPermissions.handleDocumentTraslationState(true, sectionData);
                                    currentInstance.accessPermissions.disableDocumentLevelButtons();

                                }
                                else if (folder.allProductStatusInFolder[currentInstance.formInstanceId].IsFolderVersionReleased != undefined &&
                                    folder.allProductStatusInFolder[currentInstance.formInstanceId].IsFolderVersionReleased) {
                                    currentInstance.accessPermissions.handleDocumentTraslationState(true, sectionData);
                                    currentInstance.accessPermissions.disableDocumentLevelButtons();

                                }
                                else if (folder.allProductStatusInFolder[currentInstance.formInstanceId].IsFolderVersionBaselined != undefined &&
                                    folder.allProductStatusInFolder[currentInstance.formInstanceId].IsFolderVersionBaselined) {
                                    currentInstance.accessPermissions.handleDocumentTraslationState(true, sectionData);
                                    currentInstance.accessPermissions.disableDocumentLevelButtons();
                                }
                                else if (currentInstance.isfolderReleased == true) {
                                    currentInstance.accessPermissions.disableDocumentLevelButtons();
                                }
                                if (folder.allProductStatusInFolder[currentInstance.formInstanceId].IsProductInMigration != undefined && folder.allProductStatusInFolder[currentInstance.formInstanceId].IsProductInMigration) {
                                    currentInstance.accessPermissions.handleReleasedState(true, sectionData);
                                    currentInstance.accessPermissions.disableDocumentLevelButtons();
                                }
                            }
                        }
                        //if (currentInstance.applyMaskingFlag) {
                        //    currentInstance.fieldMaskValidator.applyMask(currentInstance.designData.Validations, currentInstance.formInstanceId);
                        //    //currentInstance.masking.applyFieldMasking(currentInstance.designData, currentInstance.formInstanceId);
                        //}

                        var currentSectionRepeaterBuilders = currentInstance.repeaterBuilders.filter(function (item) {
                            if (currentInstance.currentSectionRepeaterIDs.indexOf(item.gridElementId) > -1) { return item; }
                        });

                        currentInstance.form.showValidations(currentSectionRepeaterBuilders);
                        if (callbackMetaData !== null && callbackMetaData !== undefined) {
                            callbackMetaData.callback(callbackMetaData.callbackArgs);
                        }
                    }
                    else {
                        //set the current selected section as visible                                                     
                        currentInstance.form.setSectionSelection(selectedSection);
                        if (sectionData != null) {
                            if (currentInstance.isHiddenOrDisableSections) {
                                currentInstance.accessPermissions.applyAccessPermission(sectionData);
                            }
                            currentInstance.rules.processRulesOnElements(sectionData.FullName, 'NONVALUE');
                        }

                        var currentSectionRepeaterBuilders = currentInstance.repeaterBuilders.filter(function (item) {
                            if (currentInstance.currentSectionRepeaterIDs.indexOf(item.gridElementId) > -1) { return item; }
                        });

                        currentInstance.form.showValidations(currentSectionRepeaterBuilders);
                        if (callbackMetaData !== null && callbackMetaData !== undefined) {
                            callbackMetaData.callback(callbackMetaData.callbackArgs);
                        }
                    }


                }

                var sectionId = "section" + selectedSection + currentInstance.formInstanceId;
                if (currentInstance.isStopScrollFloatingHeaders) {
                    floatingsectionheaders.float(sectionId);
                }

                if (currentInstance.selectedSectionName == "Section C: OON - Groups") {
                    if (GridDisplayMode.AG == CurrentGridDisplayMode) {
                        eventextensionsAG.registerEvent(currentInstance, currentInstance.formData, "OONGroups.ManualOverride");
                    }
                    if (GridDisplayMode.PQ == CurrentGridDisplayMode) {
                        eventextensions.registerEvent(currentInstance, currentInstance.formData, "OONGroups.ManualOverride");
                    }
                }
                var manualOverrideSectionObj = ManualOverrideSectionList.filter(function (obj) {
                    return obj.sectionName == currentInstance.selectedSectionName;
                });
                if (manualOverrideSectionObj != null && manualOverrideSectionObj != undefined && manualOverrideSectionObj.length != 0) {
                    eventextensions.registerEvent(currentInstance, currentInstance.formData, manualOverrideSectionObj[0].fieldPath);
                }
                //if (currentInstance.selectedSectionName == "Section C: OON - Number of Groups") {
                //    eventextensions.registerEvent(currentInstance, currentInstance.formData, "OONNumberofGroups.IndicatethenumberofOutofNetworkgroupingsofferedexcludingInpatientHospi");
                //}
                if (currentInstance.heighlightValidationError != undefined) {
                    currentInstance.errorManager.highlightErrorElement(currentInstance.heighlightValidationError);
                    currentInstance.heighlightValidationError = undefined;
                }
            } catch (e) {
                messageDialog.show(Common.errorMsg + 'stack ' + e.stack);
                console.log(e + "  " + e.stack + " function:renderSection");
            }
        },
        renderSection: function (selectedSection, makeVisible, callbackMetaData) {
            currentInstance.hasManualOverideElements = false;
            var sectionDesignData = currentInstance.form.getSectionData(selectedSection);
            currentInstance.form.getSectionManualOverrideData(sectionDesignData);
            //manage section locking conditionally
            currentInstance.form.manageSectionLevelLocking(selectedSection, true, callbackMetaData);

            //currentInstance.form.renderSectionAfterLock(selectedSection, true);
            //try {
            //    var currentSectionRepeaterBuilders = currentInstance.repeaterBuilders.filter(function (item) {
            //        if (currentSectionRepeaterIDs.indexOf(item.gridElementId) > -1) { return item; }
            //    });
            //    currentInstance.form.showValidations(currentSectionRepeaterBuilders);
            //}
            //catch (e) { }
        },
        showValidations: function (currentSectionRepeaters) {
            currentInstance.bottomMenu.closeBottomMenu();
            currentInstance.validation.loadValidationErrorGrid();

            if (currentInstance.form.hasValidationErrors()) {
                currentInstance.form.showValidationErrorsOnForm();
                currentInstance.formValidationManager.showValidatedControls();
                $.each(currentSectionRepeaters, function (idx, repeater) {
                    repeater.showValidatedElements();
                });
            }
            else {
                currentInstance.form.hideValidationErrorsOnForm();
            }
        },
        getSectionData: function (selectedSection) {
            var sectionData = null;
            $.each(currentInstance.designData.Sections, function (index, item) {
                if (item.Name == selectedSection) {
                    sectionData = item;
                }
            });
            return sectionData;
        },
        setDropDownTextBoxOption: function (element, item, sectionName) {
            var ItemID = "0001";
            var flag = false;
            var currentInstance = this;
            $.each(element, function (key, value) {
                if (element[key].IsDropDownTextBox == true) {
                    var count = 0;
                    var stringUtility = new globalutilities();
                    if (!stringUtility.stringMethods.isNullOrEmpty(item[sectionName][element[key].GeneratedName])) {
                        var ddlElement = item[sectionName][element[key].GeneratedName];
                        if (element[key].MultiSelect) {
                            // If dropdownTextbox has only single value (It will not give array so need to handle using single string value code)
                            if (!Array.isArray(ddlElement)) {
                                $.each(element[key].Items, function (index, value) {
                                    if (value.ItemValue.toUpperCase() === (item[sectionName][element[key].GeneratedName]).toUpperCase())
                                        count++;
                                });
                                if (count == 0) {
                                    var newItem = { 'Enabled': null, 'ItemID': ItemID++, 'ItemValue': item[sectionName][element[key].GeneratedName], 'cssclass': 'non-standard-optn' };
                                    element[key].Items.push(newItem);
                                }
                            }
                            else {
                                $.each(item[sectionName][element[key].GeneratedName], function (index, value) {
                                    $(element[key].Items).filter(function (idx, val) {
                                        if (value.toUpperCase() === val.ItemValue.toUpperCase())
                                            count++;
                                    });
                                    if (count == 0) {
                                        if (value !== "newItem") {
                                            var newItem = { 'Enabled': null, 'ItemID': ItemID++, 'ItemValue': value, 'cssclass': 'non-standard-optn' };
                                            element[key].Items.push(newItem);
                                        }
                                    }
                                    count = 0;
                                });
                            }
                        }
                        else {
                            // If Multiselect DDL is converted to normal DDL and if multiselect has more than one options selected 
                            // Then set first item as selecetd 
                            if (Array.isArray(ddlElement)) {
                                var selectedItem = ddlElement[0];
                                $.each(element[key].Items, function (index, value) {
                                    if (value.ItemValue.toUpperCase() === (selectedItem).toUpperCase())
                                        count++;
                                });
                                if (count == 0) {
                                    var newItem = { 'Enabled': null, 'ItemID': ItemID++, 'ItemValue': selectedItem, 'cssclass': 'non-standard-optn' };
                                    element[key].Items.push(newItem);
                                }
                            }
                            else {
                                $.each(element[key].Items, function (index, value) {
                                    if (value.ItemValue.toUpperCase() === (item[sectionName][element[key].GeneratedName]).toUpperCase())
                                        count++;
                                });
                                if (count == 0) {
                                    var newItem = { 'Enabled': null, 'ItemID': ItemID++, 'ItemValue': item[sectionName][element[key].GeneratedName], 'cssclass': 'non-standard-optn' };
                                    element[key].Items.push(newItem);
                                }
                            }
                        }
                    }
                } else if (element[key].Section != null && element[key].Section != undefined) {
                    currentInstance.setDropDownTextBoxOption(element[key].Section.Elements, item[sectionName], element[key].GeneratedName);
                }
            });
        },
        renderRepeater: function (design) {
            currentInstance.gridDesigns.push(design);
        },
        getFormInstanceId: function () {
            return currentInstance.formInstanceId;
        },
        //sets section selection
        setSectionSelection: function (sectionName) {
            var sectionIdJQ = '#section' + sectionName + currentInstance.formInstanceId;
            currentInstance.menu.setSectionMenuSelectionScroll();
            $(currentInstance.formInstanceDivJQ + " .tab-pane").hide();
            $(currentInstance.formInstanceDivJQ + " .tab-pane").removeClass("in active");
            $(currentInstance.formInstanceDivJQ + ' ' + sectionIdJQ).show();
            $(currentInstance.formInstanceDivJQ + ' ' + sectionIdJQ).addClass("in active");

            $(currentInstance.formInstanceDivJQ + ' ' + sectionIdJQ + " input").first().focus();
            currentInstance.form.setGridWidth();
            var folder = folderManager.getInstance().getFolderInstance();
            if (folder.allProductStatusInFolder[currentInstance.formInstanceId] != undefined) {
                if (currentInstance.folderData.isEditable == "True" && currentInstance.folderData.isLocked == "False" && currentInstance.folderData.isReleased == "False") {
                    if (!folder.allProductStatusInFolder[currentInstance.formInstanceId].IsFolderVersionReleased &&
                        !folder.allProductStatusInFolder[currentInstance.formInstanceId].IsFolderVersionBaselined
                        //&& (currentInstance.IsMasterList && folderData.RoleId == Role.SuperUser) ||
                        //(!currentInstance.IsMasterList && (folderData.RoleId != Role.EBAAnalyst || folderData.RoleId != Role.TPAAnalyst || folderData.RoleId != Role.SuperUser))
                        ) {

                        currentInstance.accessPermissions.enableDocumentLevelButtons();
                    }
                    else {
                        currentInstance.accessPermissions.disableDocumentLevelButtons();
                    }

                    if (folder.allProductStatusInFolder[currentInstance.formInstanceId].IsProductInMigration != undefined && folder.allProductStatusInFolder[currentInstance.formInstanceId].IsProductInMigration) {
                        $(currentInstance.elementIDs.documentODMDisableAlertJQ).show();
                    }
                    else {
                        $(currentInstance.elementIDs.documentODMDisableAlertJQ).hide();
                    }
                }
            }
        },
        //register datepickers
        setDatePickers: function (selectedSection) {
            var sectionIdJQ = '#section' + selectedSection + currentInstance.formInstanceId;
            $(currentInstance.formInstanceDivJQ + ' ' + sectionIdJQ + " .datepicker").each(function (indx, control) {
                $(control).datepicker({
                    dateFormat: 'mm/dd/yy',
                    onClose: function (dateText, inst) {

                    },
                    minDate: $(this).data("min-date") == "" ? null : new Date($(this).data("min-date")),
                    maxDate: $(this).data("max-date") == "" ? null : new Date($(this).data("max-date")),
                    changeMonth: true,
                    changeYear: true,
                    yearRange: 'c-61:c+20',
                    showOn: "both",
                    //Please refer to globalvariables.js for more information on Icons.CalendarIcon
                    buttonImage: Icons.CalenderIcon,
                    buttonImageOnly: true,
                    disabled: $(control).is(":disabled"),
                });
                if ($(control).is(":visible") == false) {
                    $(control).next().hide();
                }
                $(currentInstance.formInstanceDivJQ).find(sectionIdJQ).find(".datepicker").parent().css('white-space', 'nowrap');
                $(currentInstance.formInstanceDivJQ).find(sectionIdJQ).find(".datepicker").width('85%');
                $(control).on("focusout", function (e) {
                    var minDate = $(control).data("min-date") == "" ? null : new Date($(control).data("min-date"));
                    var maxDate = $(control).data("max-date") == "" ? null : new Date($(control).data("max-date"));
                    var date = new Date($(control).val());
                    currentInstance.validation.handleDateRangeValidation(date, minDate, maxDate, control);
                });
            });
        },
        setChildPopup: function (currentInstance) {
            var childPopupMgr = new childPopupManager(currentInstance, false);
            $(".ui-search-trigger").on("click", function (event) {
                childPopupMgr.init(event);
            });
        },
        setRichTextFormat: function (selectedSection) {
            if (tinymce != null) {
                tinymce.remove();
            }
            var sectionIdJQ = '#section' + selectedSection + currentInstance.formInstanceId;
            // Set z-index for dropdown click inside tinymce dialog as .mce window has z-index 999999!important 
            // If not set then menu inside dropdown i.e menuitems are displayed behind dialog
            $(document).on('click', '.mce-listbox', function () {
                $('.mce-menu').css('z-index', '999999');
            });
            $(currentInstance.formInstanceDivJQ + ' ' + sectionIdJQ + " .richtext-editorformat").each(function (indx, control) {
                currentInstance.form.registerTinyMCE('#' + control.id);
            });

        },
        registerTinyMCE: function (id) {
            tinymce.initialized = false;
            tinymce.remove(id);
            var editor = tinymce.init({
                selector: id,
                statusbar: false,
                //height: 500,
                forced_root_block: "",
                force_br_newlines: true,
                force_p_newlines: false,
                theme: 'modern',
                plugins: [
                  'advlist autolink lists charmap print preview hr pagebreak',
                  'searchreplace wordcount visualblocks visualchars code fullscreen',
                  'insertdatetime save table contextmenu directionality',
                  'emoticons template textcolor colorpicker textpattern imagetools codesample toc autoresize',
                  'image',
                  'powerpaste',
                  'tma_annotate',
                  'flite',
                  'tinycomments',
                ],
                powerpaste_word_import: 'prompt',
                powerpaste_html_import: 'prompt',
                menubar: "file edit insert view format table tools",
                toolbar1: 'undo redo | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | fontselect |  fontsizeselect',
                toolbar2: 'preview fullscreen forecolor backcolor | override | tma_annotate tma_annotatedelete | flite-toggletracking flite-toggleshow flite-acceptall flite-rejectall flite-acceptone flite-rejectone tinycomments',
                autoresize_bottom_margin: 50,
                autoresize_max_height: 600,
                removed_menuitems: 'pastetext bold italic',
                extended_valid_elements: "p[*],span[*],delete[*],insert[*],mlstart[contenteditable<false]",
                contextmenu: 'link openlink image inserttable | tma_annotate tma_annotatedelete | cell row column deletetable',
                custom_user_name: currentInstance.currentUserName,
                custom_user_id: currentInstance.currentUserId,
                style_formats: [
                               { title: 'Heading 1', block: 'h1' },
                               { title: 'Heading 2', block: 'h2' },
                               { title: 'Heading 3', block: 'h3' },
                               { title: 'Heading 4', block: 'h4' },
                               { title: 'Heading 5', block: 'h5' },
                               { title: 'Heading 6', block: 'h6' },
                ],

                image_advtab: true,
                templates: [
                  { title: 'Test template 1', content: 'Test 1' },
                  { title: 'Test template 2', content: 'Test 2' }
                ],
                image_list: [
                    { title: 'Apple', value: '/Content/tinyMce/Apple.png' },
                    { title: 'Question Mark', value: '/Content/tinyMce/Question Mark.png' },
                    { title: 'Tick Mark', value: '/Content/tinyMce/Tick Mark.png' },
                    { title: 'Square Bullet', value: '/Content/tinyMce/Square Bullet.png' },
                    { title: 'ID Card No Rx - Back', value: '/Content/tinyMce/ID Card No Rx - Back.png' },
                    { title: 'ID Card No Rx - Front', value: '/Content/tinyMce/ID Card No Rx - Front.png' },
                    { title: 'Medicare Rx Membership Card', value: '/Content/tinyMce/Medicare Rx Membership Card.png' },
                    { title: 'Back Membership Card', value: '/Content/tinyMce/Back Membership Card.png' },
                    { title: 'Tick [Large]', value: '/Content/tinyMce/Tick [Large].png' },
                    { title: 'Tick Bullets', value: '/Content/tinyMce/Tick Bullets.png' },
                    { title: 'Tick Not Valid', value: '/Content/tinyMce/Tick Not Valid.png' }

                ],
                content_css: '/Content/css/tmaannotation.css',
                tinycomments_create: create,
                tinycomments_reply: reply,
                tinycomments_delete: del,
                tinycomments_lookup: lookup,
                setup: function (editor) {

                    editor.on('focusin', function (e) {
                        var content = editor.getContent();
                        try {
                            var flitePlugin = editor.plugins.flite;
                            if (flitePlugin != undefined) {
                                flitePlugin.setUserInfo({ id: currentInstance.currentUserId, name: currentInstance.currentUserName });
                            }
                        } catch (e) {
                            console.log('Error while adding wrapper.');
                            editor.setContent(content);
                        }

                    });

                    editor.on('focusout', function (e) {
                        tinymce.triggerSave();
                        $(id).trigger('change');
                    });

                    editor.on("init", function () {
                        if ($(id).hasClass("enableMo") == true) {
                            //ignore
                        }
                        else {
                            this.setMode('readonly');
                            enableTinyMceEditorPlugin(id, 'preview', 'mcePreview');
                            enableTinyMceEditorPlugin(id, 'fullscreen', 'mceFullScreen');
                        }
                    });
                },
            });

            function enableTinyMceEditorPlugin(editorId, pluginName, commandName) {
                editorId = editorId.replace('#', '');
                var htmlEditorDiv = document.getElementById(editorId).previousSibling;
                var editor = tinymce.get(editorId);
                var buttonDiv = htmlEditorDiv.querySelectorAll('.mce-i-' + pluginName.toLowerCase())[0].parentElement.parentElement;
                buttonDiv.className = buttonDiv.className.replace(' mce-disabled', '');
                buttonDiv.removeAttribute('aria-disabled');
                buttonDiv.firstChild.onclick = function () {
                    editor.execCommand(commandName);
                };
            }

            function randomString() {
                return Math.random().toString(36).substring(2, 14);
            }

            function getConversation(uid) {
                var el = currentInstance.commentData;
                var conv = el[uid];
                if (!conv) {
                    el[uid] = [];
                }
                return el[uid];
            }

            function setConversation(uid, conversation) {
                var el = currentInstance.commentData;
                el[uid] = conversation;
            }

            function deleteConversation(uid) {
                var el = currentInstance.commentData;
                delete el[uid];
            }

            function create(content, done, fail) {
                var uid = 'ant-' + randomString();
                try {
                    setConversation(
                        uid, [{
                            userName: currentInstance.currentUserName,
                            userID: currentInstance.currentUserId,
                            richTextBoxId: id,
                            comment: content
                        }]
                    );
                    done(uid);
                } catch (ex) {
                    fail(new Error('Error creating conversation...'));
                }
            }

            function reply(uid, content, done, fail) {
                try {
                    var comments = getConversation(uid);
                    comments.push({
                        userName: currentInstance.currentUserName,
                        userID: currentInstance.currentUserId,
                        richTextBoxId: id,
                        comment: content
                    });
                    setConversation(uid, comments);
                    done();
                } catch (ex) {
                    fail(new Error('Error replying to conversation...'));
                }
            }

            function del(uid, done, fail) {
                deleteConversation(uid);
                done(true);
            }

            function lookup(uid, done, fail) {
                try {
                    var comments = getConversation(uid).map(function (item) {
                        return {
                            author: item.userName, //getAuthorDisplayName(item.user),
                            content: item.comment
                        };
                    });
                    done({
                        comments: comments
                    });
                } catch (ex) {
                    fail(new Error('Error looking up conversation...'));
                }
            }
        },
        //resize grid width
        setGridWidth: function () {
            $(currentInstance.formInstanceDivJQ + " .ui-jqgrid").each(function (index, grid) {
                var gridID = $(grid).attr("ID").substring(5);
                var width = $("#" + gridID).jqGrid('getGridParam', 'width');
                if ($($(grid).parent()).innerWidth() > width)
                    width = $($(grid).parent()).innerWidth() - 20;
                $("#" + gridID).jqGrid().setGridWidth(width, false);
            });
        },

        //save form instance data
        saveFormInstanceData: function (isFromStatusUpdate, isBackGroundSave, NewSectionForML, saveOnViewChange, previousView) {
            var HasError = false;
            if (currentInstance.isFolderLockEnable) { // && currentInstance.folderId != 7459) {
                //var sectionName = currentInstance.selectedSectionName;
                //var sectioName = currentInstance + '.sections.' + currentInstance.selectedSection + '. FullName';
                var sectioName = currentInstance.sections[currentInstance.selectedSection].FullName; //eval(sectioName);
                var lockUrl = currentInstance.URLs.isDocumentLockedByUser.replace(/\{formInstanceId\}/g, currentInstance.formInstanceId) + "&sectionName=" + sectioName + "&formDesignId=" + currentInstance.formDesignId;

                var promise = ajaxWrapper.postAsyncJSONCustom(lockUrl);
                promise.done(function (xhr) {

                    try {
                        if (xhr == true) {// || (xhr == false && currentInstance.IsMasterList)) {
                            var formInstanceData = currentInstance.form.getFormInstanceData();
                            formInstanceData.formInstanceData = formInstanceData.formInstanceData.replace(/\[Select One]/g, '').replace(/\Select One/g, '');
                            var global = isBackGroundSave == true ? false : true;
                            var url = currentInstance.URLs.saveFormInstanceData;
                            var promise = ajaxWrapper.postJSON(url, formInstanceData, global);
                            //register ajax success callback
                            promise.done(function (xhr) {
                                //returning sections updated instead of ServiceResult
                                if ((typeof (xhr) === "object" && xhr.length != undefined) || (typeof (xhr) === "object" && xhr.Result == ServiceResult.SUCCESS)) {
                                    if ((typeof (xhr) === "object" && xhr.length > 0)) {
                                        $.each(xhr, function () {
                                            var that = this.SectionName;
                                            var thatData = this.SectionData;
                                            $.each(currentInstance.sections, function () {
                                                if (this.FullName === that) {
                                                    this.IsLoaded = false;
                                                    currentInstance.formData[that] = JSON.parse(thatData);
                                                }
                                            });
                                        });
                                    }
                                    if (isFromStatusUpdate == true) {
                                        $(currentInstance.repeaterBuilders).each(function (index) {
                                            currentInstance.repeaterBuilders[index].orignalData = currentInstance.repeaterBuilders[index].data;
                                        });
                                        messageDialog.show(FolderVersion.saveDocumentInstanceMsg);
                                    }
                                    //save activity log data.
                                    currentInstance.form.saveFormInstanceActivityLogData();

                                    if (ActiveRuleExecutionLogger == "True") {
                                        //save Rule Execution Log Data
                                        ruleExecutionLogger.saveFormInstanceRuleExecutionLogData(currentInstance);
                                    }

                                    //save the PBP View Action log data
                                    if (currentInstance.formInstanceId == currentInstance.anchorFormInstanceID && GLOBAL.applicationName.toLowerCase() !== 'ebenefitsync') {
                                        currentInstance.form.saveDocumentViewImpactLogData();
                                    }

                                    currentInstance.autosave.setAutoSaveTimeout();
                                    currentInstance.hasChanges = false;
                                    currentInstance.loadDataFromRepeater = {};
                                    currentInstance.repeaterRowIdList = [];
                                    //add Data in repeater in instance
                                    currentInstance.form.spliceOrMargeFormInstnaceRepeaterData(false, currentInstance);

                                    if ((NewSectionForML != null && NewSectionForML != undefined) && currentInstance.IsMasterList)
                                        currentInstance.menu.loadNewViewML(NewSectionForML);
                                    //wellcare
                                    if (saveOnViewChange == true) {
                                        reloadData = false;
                                        sectionName = "";
                                        //if (data != undefined && data.sectionName != undefined) {
                                        //    currentInstance.selectedSection = data.sectionName;
                                        //    sectionName = data.sectionGeneratedName;
                                        //}
                                        var formInstanceIDs = $(currentInstance.elementIDs.sectionMenuContainerJQ + " select.view-menu")[0].value.split('_');
                                        currentInstance.cleanup();
                                        currentInstance.formDesignVersionId = formInstanceIDs[1];
                                        currentInstance.formInstanceId = formInstanceIDs[0];
                                        currentInstance.accessPermissions = currentInstance.accessPermissionsMethods();
                                        var callbackfunc = null;
                                        //if (data != null) {
                                        //    callbackfunc = data.callback;
                                        //}
                                        currentInstance.loadFormInstanceDesignData(reloadData, sectionName, false, callbackfunc);
                                        var folder = folderManager.getInstance().getFolderInstance();
                                        folder.loadAllFormsStatus(currentInstance.folderVersionId);
                                    }
                                }
                                else {
                                    if (xhr.Result == ServiceResult.FAILURE) {
                                        messageDialog.show(Common.errorMsg);
                                    }
                                    else if (xhr.Result == ServiceResult.WARNING) {
                                        if (previousView) {
                                            $(currentInstance.elementIDs.sectionMenuContainerJQ + " select.view-menu").val(previousView);
                                        }
                                        messageDialog.show("Exit Validate is in process for this Product, changes will not be saved.");
                                    }
                                }
                            });
                            //register ajax failure callback
                            promise.fail(this.showError);
                        }
                            //else if (!currentInstance.IsMasterList) {
                        else {
                            documentLockOverridenDialog.show(FolderLock.unlockedDocumentMsg, function (e) {
                                if (e == true) {
                                    var sectionName = currentInstance.selectedSection;
                                    if (sectionName == null || sectionName == undefined) {
                                        window.location.reload();
                                    } else {
                                        var sectionDetails = currentInstance.form.getSectionData(sectionName);
                                        sectionName = sectionDetails.FullName;
                                        currentInstance.cleanup();
                                        currentInstance.loadFormInstanceDesignData(true, sectionName, false);
                                        var folder = folderManager.getInstance().getFolderInstance();
                                        folder.loadAllFormsStatus(currentInstance.folderVersionId);
                                        documentLockOverridenDialog.hide();
                                    }
                                } else {
                                    console.log("Page does not refresh...");
                                }
                            });
                        }
                    }
                    catch (ex) {
                        HasError = true;
                    }
                });
                promise.fail(this.showError);
            }
            else {
                //try{
                var formInstanceData = currentInstance.form.getFormInstanceData();
                var url = currentInstance.URLs.saveFormInstanceData;
                var promise = ajaxWrapper.postJSON(url, formInstanceData);
                //register ajax success callback
                promise.done(function (xhr) {
                    try {
                        //show appropriate message
                        if (xhr.Result == ServiceResult.SUCCESS) {

                            messageDialog.show(FolderVersion.saveDocumentInstanceMsg);
                            currentInstance.autosave.setAutoSaveTimeout();

                            //save form instance activity log.
                            currentInstance.form.saveFormInstanceActivityLogData();

                            if (ActiveRuleExecutionLogger == "True") {
                                //save Rule Execution Log Data
                                ruleExecutionLogger.saveFormInstanceRuleExecutionLogData(currentInstance);
                            }

                            currentInstance.hasChanges = false;
                            currentInstance.loadDataFromRepeater = {};
                            currentInstance.repeaterRowIdList = [];
                            //add Data in repeater in instance
                            currentInstance.form.spliceOrMargeFormInstnaceRepeaterData(false, currentInstance);

                            if ((NewSectionForML != null && NewSectionForML != undefined) && currentInstance.IsMasterList)
                                currentInstance.menu.loadNewViewML(NewSectionForML);
                        }
                        else {
                            messageDialog.show(Common.errorMsg);
                        }
                    }
                    catch (ex) {
                        HasError = true;
                    }
                });
                //register ajax failure callback
                promise.fail(this.showError);

            }
        },



        getFormInstanceData: function () {

            //initilize repeater object if found in MasterList 
            //check the design element LoadFromServer is true which is selected to save server side           
            currentInstance.form.spliceOrMargeFormInstnaceRepeaterData(true, currentInstance);
            var sectionDetails = currentInstance.form.getSectionData(currentInstance.selectedSection);
            var sectionData = currentInstance.formData[sectionDetails.FullName];

            var formInstanceData = {
                tenantId: currentInstance.tenantId,
                formInstanceId: currentInstance.formInstanceId,
                folderId: currentInstance.folderId,
                folderVersionId: currentInstance.folderVersionId,
                formDesignId: currentInstance.formDesignId,
                formDesignVersionId: currentInstance.formDesignVersionId,
                formInstanceData: JSON.stringify(sectionData),
                repeaterFullNameList: JSON.stringify(currentInstance.repeaterFullPathList),
                repeaterData: JSON.stringify(currentInstance.loadDataFromRepeater),
                sectionName: sectionDetails.FullName,
                commentData: JSON.stringify(currentInstance.commentData)
            }
            return formInstanceData;
        },

        registerToolTip: function (sectionName) {
            $(currentInstance.formInstanceDivJQ + " #section" + sectionName + currentInstance.formInstanceId + " .hastooltip").tooltip({
                placement: 'bottom'
            });
        },

        objectChangeHandler: function (ev, eventArgs) {
            var path = ev.data.fullPath;

            var validationError = currentInstance.formValidationManager.handleValidation(path, eventArgs.value);
            if (validationError) {
                currentInstance.validation.handleObjectChangeValidation(validationError);
                currentInstance.validation.showValidatedControlsOnSectionElementChange(validationError);
            }

            var sectionDetails = currentInstance.designData.Sections.filter(function (ct) {
                return ct.Label == currentInstance.selectedSectionName;
            });
            var elementDetails = getElementDetails(sectionDetails[0], path);
            if (elementDetails.DataType === 'date') {
                if (eventArgs.value != "") {
                    var validationError = currentInstance.formValidationManager.handleDateValidation(path, eventArgs.value);
                    if (validationError) {
                        currentInstance.validation.handleObjectChangeValidation(validationError);
                        currentInstance.validation.showValidatedControlsOnSectionElementChange(validationError);
                    }
                }
            }
            var updatedDate = new Date();
            var sectionDetails = currentInstance.designData.Sections.filter(function (ct) {
                return ct.Label == currentInstance.selectedSectionName;
            });

            var elementPath = currentInstance.selectedSectionName + getElementHierarchy(sectionDetails[0], path);
            var SelectedSectionFullName = elementPath.replace(/=>/g, ".").replace(/ /g, '').replace(/&/, '');

            var element = null;
            var elementData = getElementDetails(sectionDetails[0], ev.data.fullPath);

            if (elementData.Type == "select" || elementData.Type == "SelectInput") {
                var finstanceId = currentInstance.formInstanceId;
                var elementName = '#' + elementData.Name + finstanceId;
                if (eventArgs.oldValue != null && eventArgs.oldValue != "[Select One]") {
                    if ($(elementName).find("option[value =' + eventArgs.oldValue + ']").length > 0) {
                        eventArgs.oldValue = $(elementName).find("option[value =' + eventArgs.oldValue + ']").text();
                    }
                }
                if (eventArgs.value != null && eventArgs.value != "[Select One]") {
                    if ($(elementName).find("option[value =' + eventArgs.value + ']").length > 0) {
                        eventArgs.value = $(elementName).find("option[value =' + eventArgs.value + ']").text();
                    }
                }

            }

            if (elementData.Type == "checkbox") {
                isCollateralMO = true;
                var manualOverrideSectionObj = ManualOverrideSectionList.filter(function (obj) {
                    return obj.sectionName == currentInstance.selectedSectionName;
                });
                if (manualOverrideSectionObj != null && manualOverrideSectionObj != undefined && manualOverrideSectionObj.length != 0)
                    isCollateralMO = false;
                if (elementData.Label.indexOf("Manual Override") >= 0 && elementData.FullName != "OONGroups.ManualOverride" && isCollateralMO) {
                    if (elementData.ignoreObjectHandler == undefined || elementData.ignoreObjectHandler == false) {
                        currentInstance.objManualOverride.process(elementData, eventArgs.value, path);
                        if (eventArgs.value) currentInstance.form.manageSectionLevelLocking(sectionDetails[0].Name, false);
                    }
                    else {
                        elementData.ignoreObjectHandler = false;
                    }
                }
            }

            var isRichTextTrigger = false, divElement = document.createElement('div'), oldValue, newValue;
            if (elementData.IsRichTextBox == true) {
                //doing for removing red color of italic string in activity log
                if (eventArgs.value.indexOf('<em>') > -1) {
                    eventArgs.value = eventArgs.value.replace(/<em>/g, "<i>");
                    eventArgs.value = eventArgs.value.replace(/<\/em>/g, "<\i>");
                }
                if (eventArgs.oldValue.indexOf('<em>') > -1) {
                    eventArgs.oldValue = eventArgs.oldValue.replace(/<em>/g, "<i>");
                    eventArgs.oldValue = eventArgs.oldValue.replace(/<\/em>/g, "<\i>");
                }
                divElement.innerHTML = eventArgs.oldValue;
                oldValue = $(divElement).text();
                divElement.innerHTML = eventArgs.value;
                newValue = $(divElement).text();
                isRichTextTrigger = (oldValue == newValue) ? true : false;
            }
            var executionLogParentRowId = -1;
            if (!isRichTextTrigger) {
                //log operations performed by logged-in user as activity log.
                activitylogger.log(elementData.Label, eventArgs.oldValue, eventArgs.value, elementPath, currentUserName, updatedDate, parseInt(currentInstance.formInstanceId));
                if (ActiveRuleExecutionLogger == "True") {
                    executionLogParentRowId = ruleExecutionLogger.logRulesExecution(elementData.ElementID, eventArgs.oldValue, eventArgs.value, null, null, null, 'Activity', true, null, 'On Change');
                }
            }
            //currentInstance.ruleProcessor.setPropertyValue
            //fire the rules
            currentInstance.rules.processRulesForElement(path, elementData, eventArgs, elementPath, executionLogParentRowId);
            //remove error rows from Error Grid for Visible/Disable controls
            currentInstance.validation.removeVisibleAndDisabledControls();
            //form object is changed.
            if ((eventArgs.oldValue == "False" && (eventArgs.value == false || eventArgs.value == "false")) || (eventArgs.oldValue == "True" && (eventArgs.value == true || eventArgs.value == "true"))) {
                currentInstance.hasChanges = false;
            }
            else {
                currentInstance.hasChanges = true;
            }
            if (currentInstance.form.hasValidationErrors()) {
                currentInstance.form.showValidationErrorsOnForm();
            }
            else {
                currentInstance.form.hideValidationErrorsOnForm();
            }

            if (currentInstance.expressionBuilder.hasExpressionBuilderRule(currentInstance.designData.FormDesignId)) {
                currentInstance.expressionBuilder.sectionElementOnChange(currentInstance, path, eventArgs.value);
            }
            if (currentInstance.expressionBuilderExten.hasRule(elementData, eventArgs)) {
                if (elementData.Type != "repeater")
                    currentInstance.expressionBuilderExten.processRule(elementData, eventArgs, currentInstance.formDesignVersionId, currentInstance.folderVersionId, currentInstance.formInstanceId, currentInstance.formData, currentInstance);
            }
        },

        hasValidationErrors: function () {
            return (currentInstance.errorGridData.length > 0);
        },

        showValidationErrorsOnForm: function (formInstanceId) {
            if (formInstanceId == undefined) {
                formInstanceId = currentInstance.formInstanceId;
            }
            $("#form-tab-" + formInstanceId).addClass('document-validation-error');
        },

        hideValidationErrorsOnForm: function (formInstanceId) {
            if (formInstanceId == undefined) {
                formInstanceId = currentInstance.formInstanceId;
            }
            $("#form-tab-" + formInstanceId).removeClass('document-validation-error');
        },

        setValueToEmptyDropdown: function (selectedSection) {
            var sectionIdJQ = '#section' + selectedSection;
            $(currentInstance.formInstanceDivJQ + ' ' + sectionIdJQ).find('select').each(function () {
                if ($(this).val() == '' || $(this).val() == null)
                    $(this).val(Validation.selectOne);
            });
        },
        setAutoCompleteToFilterDropdown: function (selectedSection, elements) {
            var filterDropDownList = new Array();
            var sectionIdJQ = '#section' + selectedSection;
            $(elements).each(function (index, obj) {
                if (obj.Type == "section") {
                    var section = obj.Section;
                    if (section !== undefined && section !== null) {
                        var secElements = section.Elements;
                        if (secElements !== undefined && secElements !== null && secElements.length > 0) {
                            processSection(secElements);
                        }
                    }
                }
                else if ((obj.Type == 'select' || obj.Type == 'SelectInput') && obj.IsDropDownFilterable == true) {
                    filterDropDownList.push(obj);
                }
            });

            var item = undefined;
            if (filterDropDownList.length > 0) {
                autoCompleteDropDown(jQuery);
                $(filterDropDownList).each(function (index, obj) {
                    item = $(currentInstance.formInstanceDivJQ + ' ' + sectionIdJQ + currentInstance.formInstanceId).find('#' + obj.Name + currentInstance.formInstanceId);
                    if (item !== undefined && item !== null) {
                        item.filterDropDown({
                            ID: obj.Name + currentInstance.formInstanceId + 'DDL',
                        });
                    }
                });
            }

            function processSection(sectionElements) {
                $(sectionElements).each(function (index, obj) {
                    if (obj.Type == "section") {
                        var section = obj.Section;
                        if (section !== undefined && section !== null) {
                            var secElements = section.Elements;
                            if (secElements !== undefined && secElements !== null && secElements.length > 0) {
                                processSection(secElements);
                            }
                        }
                    }
                    else if ((obj.Type == 'select' || obj.Type == 'SelectInput') && obj.IsDropDownFilterable == true) {
                        filterDropDownList.push(obj);
                    }
                });
            }
        },

        setSectionEnableProperty: function (sectionData) {
            var currentFormInstanceUtilities = new formUtilities(currentInstance.formInstanceId);
            if (sectionData !== undefined || sectionData !== null) {
                if (!sectionData.Enabled) {
                    currentFormInstanceUtilities.sectionManipulation.SetSectionDisable(sectionData);
                }
                else {
                    $.each(sectionData.Elements, function (index, element) {
                        var elem;
                        if (element.Type == "section") {
                            currentInstance.form.setSectionEnableProperty(element.Section);
                        }
                    });
                }
            }
        },

        setSectionVisibleProperty: function (sectionData) {
            var currentFormInstanceUtilities = new formUtilities(currentInstance.formInstanceId);
            if (sectionData != undefined && sectionData != null) {
                if (!sectionData.Visible) {
                    currentFormInstanceUtilities.sectionManipulation.SetSectionHidden(sectionData);
                }
                else {
                    $.each(sectionData.Elements, function (index, element) {
                        var elem;
                        if (element.Type == "section") {
                            currentInstance.form.setSectionVisibleProperty(element.Section);
                        }
                    });
                }
            }
        },
        getSectionManualOverrideData: function (sectionData) {
            var targetSectionElements, manualOverrideData;
            if (sectionData != undefined && sectionData != null) {
                if (sectionData.Visible) {
                    targetSectionElements = sectionData.Elements.filter(function (ts) {
                        return (ts.GeneratedName.search('ManualOverride') != -1) && ts.Type == "checkbox";
                    });
                    if (targetSectionElements.length > 0) {
                        $(targetSectionElements).each(function (i, v) {
                            if (!currentInstance.hasManualOverideElements) {
                                var data = currentInstance.formData[sectionData.FullName][v.GeneratedName];
                                var value = data != "" ? data.toString().toLowerCase() == "true" ? true : false : false;
                                if (value) { currentInstance.hasManualOverideElements = true; }
                            }
                        });
                    }
                    else
                        currentInstance.hasManualOverideElements = true;
                }
            }
        },
        manageSectionLevelLocking: function (selectedSection, isLoadSection, callbackMetaData) {
            this.isDocumentEditable = folderData.isEditable;
            if ((currentInstance.isDocumentEditable.toLowerCase() == "true" && currentInstance.hasManualOverideElements) || (currentInstance.isDocumentEditable.toLowerCase() == "true" && !isLoadSection)) {
                var displaySectionName = currentInstance.sections[selectedSection].SectionName;
                var displayViewName = currentInstance.designData.FormName;

                var url = currentInstance.URLs.manageSectionLock.replace(/\{formInstanceID\}/g, currentInstance.formInstanceId);
                url = url.replace(/\{displayViewName\}/g, displayViewName);
                url = url.replace(/\{displaySectionName\}/g, encodeURIComponent(displaySectionName));
                url = url.replace(/\{sectionName\}/g, encodeURIComponent(currentInstance.sections[selectedSection].FullName));
                url = url.replace(/\{folderID\}/g, currentInstance.folderId);
                url = url.replace(/\{formDesignID\}/g, currentInstance.formDesignId);
                url = url.replace(/\{formName\}/g, encodeURIComponent(currentInstance.formName));
                url = url.replace(/\{isMasterList\}/g, currentInstance.IsMasterList);

                var promise = ajaxWrapper.getJSON(url);
                //register ajax success callback
                promise.done(function (result) {
                    //loads the view dropdown data  
                    if (isLoadSection) {
                        if (result != null && result.MarkReadOnly != null) {
                            currentInstance.isSectionEditable = !(result.MarkReadOnly);
                            if (result.LockedByUser != null) {
                                currentInstance.folderData.isLocked = "True";

                                currentInstance.accessPermissions.disableMenuLevelButtons();
                                currentInstance.accessPermissions.disableDocumentLevelButtons();
                                // $(currentInstance.elementIDs.btnSaveFormDataJQ).addClass('disabled-button');
                                currentInstance.IsFormInstanceEditable = false;
                                messageDialog.show(result.LockedByUser);
                            }
                            else {

                                //$(currentInstance.elementIDs.btnSaveFormDataJQ).removeClass('disabled-button');
                                currentInstance.IsFormInstanceEditable = true;
                                currentInstance.accessPermissions.enableMenuLevelButtons();
                                currentInstance.accessPermissions.enableDocumentLevelButtons();
                            }
                            currentInstance.form.renderSectionAfterLock(selectedSection, true, callbackMetaData);
                        }
                    } else {
                        if (result != null && result.MarkReadOnly != null) {
                            currentInstance.isSectionEditable = !(result.MarkReadOnly);
                            if (result.LockedByUser != null) {
                                currentInstance.accessPermissions.disableMenuLevelButtons()
                                currentInstance.accessPermissions.disableDocumentLevelButtons();
                                // $(currentInstance.elementIDs.btnSaveFormDataJQ).addClass('disabled-button');
                                currentInstance.IsFormInstanceEditable = false;
                                messageDialog.show(result.LockedByUser);
                            }
                        }
                    }
                });
            }
            else {
                if (currentInstance.isfolderReleased == false && currentInstance.isDocumentEditable.toLowerCase() == "true") {
                    currentInstance.isSectionEditable = true;
                    currentInstance.IsFormInstanceEditable = true;
                    currentInstance.accessPermissions.enableMenuLevelButtons();
                    currentInstance.accessPermissions.enableDocumentLevelButtons();
                }

                if (isLoadSection)
                    currentInstance.form.renderSectionAfterLock(selectedSection, true, callbackMetaData);

                //if (currentInstance.isDocumentEditable.toLowerCase() == "false") {
                //    currentInstance.accessPermissions.disableMenuLevelButtons();
                //    currentInstance.accessPermissions.disableDocumentLevelButtons();
                //    currentInstance.IsFormInstanceEditable = false;
                //}
            }
        },
        getPreviousFormInstance: function (previousInstance) {

            var formInstanceData = {
                tenantId: previousInstance.tenantId,
                formInstanceId: previousInstance.formInstanceId,
                folderId: previousInstance.folderId,
                folderVersionId: previousInstance.folderVersionId,
                formDesignId: previousInstance.formDesignId,
                formDesignVersionId: previousInstance.formDesignVersionId,
                formInstanceData: JSON.stringify(previousInstance.formData)
            }

            return formInstanceData;
        },

        hasChanges: function () {
            if (currentInstance.hasChanges) {
                return true;
            }
            else
                return false;
        },

        sectionControlFocusOut: function () {
            $.each(currentInstance.designData.Validations, function (idx, ct) {
                if ((currentInstance.selectedSectionName).replace(/\s/g, '') == ct.FullName.split('.')[0] && ct.IsRequired == true) {
                    if (ct.UIElementName.indexOf("Repeater") == -1) {
                        $("#" + ct.UIElementName + currentInstance.formInstanceId).focusout(function (e) {
                            var path = $(this).data("journal");
                            if (this.type == "radio") {
                                if (this.checked) {
                                    var value = this.checked;
                                }
                                else {
                                    var value = "";
                                }
                            }
                            else {
                                var value = this.value;
                            }
                            var validationError = currentInstance.formValidationManager.handleValidation(path, value);
                            if (validationError) {
                                if (validationError.IsRequiredError == true) {
                                    currentInstance.validation.handleObjectChangeValidation(validationError);
                                    $(currentInstance.formInstanceDivJQ).find("#" + validationError.UIElementName + currentInstance.formInstanceId).parent().addClass("has-error");
                                    currentInstance.bottomMenu.closeBottomMenu();
                                    currentInstance.validation.loadValidationErrorGrid();
                                }
                            }
                        });
                    }
                }
            });
        },

        spliceOrMargeFormInstnaceRepeaterData: function (flag, currentInstance) {
            if (currentInstance.formDesignId == FormTypes.MASTERLISTFORMID) {
                currentInstance.repeaterFullPathList = [];
                for (var idx = 0; idx < currentInstance.repeaterBuilders.length; idx++) {
                    if (currentInstance.repeaterBuilders[idx].design.LoadFromServer == true) {
                        //code for splice repeater data from current instance
                        var fullpath = currentInstance.repeaterBuilders[idx].design.FullName.split('.');
                        var repeaterName = currentInstance.repeaterBuilders[idx].design.GeneratedName;
                        if (currentInstance.loadDataFromRepeater.hasOwnProperty(repeaterName) && flag == true) {
                            currentInstance.repeaterFullPathList.push(currentInstance.repeaterBuilders[idx].design.FullName);
                        }
                        var formData;
                        for (var index = 0; index < fullpath.length; index++) {
                            if (index == 0) {
                                formData = currentInstance.formData[fullpath[index]];
                            }
                            else if (index == (fullpath.length - 1)) {
                                if (flag == true) {
                                    formData[fullpath[index]] = new Array();
                                }
                                else {
                                    var repeaterData = currentInstance.repeaterBuilders[idx].data;
                                    currentInstance.repeaterBuilders[idx].data = new Array();
                                    $.each(repeaterData, function (idx, row) {
                                        formData[fullpath[index]].push(row);
                                    });
                                }
                            }
                            else {
                                formData = formData[fullpath[index]];
                            }
                        }
                    }
                }
            }
        },
        saveSectionData: function (selectedSection, newSection, callbackMetaData) {
            var sectionDetails = currentInstance.form.getSectionData(selectedSection);
            var sectionData = currentInstance.formData[sectionDetails.FullName];
            var messageToPost = { newSection: newSection, tenantId: currentInstance.tenantId, formInstanceId: currentInstance.formInstanceId, folderVersionId: currentInstance.folderVersionId, formDesignId: currentInstance.formDesignId, formDesignVersionId: currentInstance.formDesignVersionId, sectionName: sectionDetails.FullName, sectionData: JSON.stringify(sectionData).replace(/\[Select One]/g, '').replace(/\Select One/g, '') };
            var promise = ajaxWrapper.postJSON(currentInstance.URLs.saveFormInstanceSectionData, messageToPost);
            promise.done(function (xhr) {
                //Check sections updated and set reload true;
                if (typeof (xhr) === "object" && xhr.length > 0) {
                    $.each(xhr, function () {
                        var that = this.SectionName;
                        var thatData = this.SectionData;
                        $.each(currentInstance.sections, function () {
                            if (this.FullName === that) {
                                this.IsLoaded = false;
                                currentInstance.formData[that] = JSON.parse(thatData);
                            }
                        });
                    });
                }

                if (newSection !== null && newSection !== undefined) {
                    currentInstance.selectedSection = newSection;
                    currentInstance.form.getSectionDataFromServer(callbackMetaData);
                }
            });
            promise.fail(this.showError);
            //}
        },
        getSectionDataFromServer: function (callbackMetaData) {
            currentInstance.documentLock.resetCurrentUserLockSession();
            if (currentInstance.sections[currentInstance.selectedSection].IsLoaded) {
                currentInstance.form.loadSection();
                if (callbackMetaData !== null && callbackMetaData !== undefined) {
                    callbackMetaData.callback(callbackMetaData.callbackArgs);
                }
            } else {
                var sectionDetails = currentInstance.form.getSectionData(currentInstance.selectedSection);
                var sectionName = sectionDetails.FullName;
                var url = currentInstance.URLs.getSectionDataFromServer.replace(/\{sectionName\}/g, sectionName).replace(/\{formInstanceId\}/g, currentInstance.formInstanceId).replace(/\{formDesignId\}/g, currentInstance.formDesignId).replace(/\{folderVersionId\}/g, currentInstance.folderVersionId).replace(/\{formDesignVersionId\}/g, currentInstance.formDesignVersionId);
                var promise = ajaxWrapper.getJSON(url);
                promise.done(function (xhr) {
                    if (typeof (xhr) === "object" && xhr.length > 0) {
                        $.each(xhr, function () {

                            var data = this.SectionData.replace(/\[Select One]/g, '').replace(/\Select One/g, '');
                            var parseData = JSON.parse(data);
                            var sectionKeys = Object.keys(parseData);
                            var sectionKey = sectionKeys[0];
                            var sectionInfo = null;
                            $.each(currentInstance.sections, function () {
                                if (this.FullName == sectionKey) {
                                    sectionInfo = this; return false;
                                };
                            });
                            if (sectionKey == sectionName) {
                                currentInstance.formData[sectionKey] = parseData[sectionKey];

                            } else if (sectionInfo != null && sectionInfo.IsLoaded == false) {
                                currentInstance.formData[sectionKey] = parseData[sectionKey];
                            }
                            $.each(sectionKeys, function (idx, key) {
                                if (key != sectionKey) {
                                    currentInstance.formData[key] = parseData[key];
                                }
                            });
                            var detail = this.SectionDetail;
                            if (this.DataSource != null) {
                                currentInstance.designData.DataSources = this.DataSource;
                            }

                            $.each(currentInstance.designData.Sections, function (idx, dt) {
                                if (dt.GeneratedName == sectionKey) {
                                    currentInstance.designData.Sections[idx] = detail;
                                }
                            })
                        });
                        currentInstance.form.loadSection(callbackMetaData);
                        currentInstance.observeAllNonArrayFields(currentInstance.formData[sectionName], sectionName);
                    }
                    //if (callbackMetaData !== null && callbackMetaData !== undefined) {
                    //    callbackMetaData.callback(callbackMetaData.callbackArgs);
                    //}
                });
                promise.fail(currentInstance.showError);
            }
        },
        loadSection: function (callbackMetaData) {
            var selectedSection = currentInstance.selectedSection;
            var selectedSectionElementId = 'section' + currentInstance.selectedSection;
            //currentInstance.form.renderSection(selectedSection, true);
            currentInstance.menu.setSectionMenuSelection(selectedSection, callbackMetaData);
            $("#" + selectedSectionElementId).find(":input").first().focus();
            currentInstance.bottomMenu.checkSectionSelection(selectedSection);
        },
        renderCustomHtml: function (customHtml, elementCount, name) {
            var html = "<script id=\"" + name.replace("#", "") + "\" type=\"text/x-jsrender\">";
            html = html + customHtml;

            for (i = 1; i <= elementCount; i++) {
                //class="repeater-grid {{>~getCssClass(~layout)}}"
                html = html.replace("{{" + i + "}}", "{{for Elements tmpl=\"#CustomElementTemplate\" ~cnt = " + i + " ~layout = LayoutColumn/}}");
            }

            html = html + "</script>";
            $("body").append(html);
        },

        saveFormInstanceActivityLogData: function () {
            var activityLogDataURL = currentInstance.URLs.saveActivityLogData;

            var activityLogDataToBeSaved = [];

            currentInstance.currentActivitylogData = activitylogger.getActivityLogFormInstanceData(parseInt(currentInstance.formInstanceId));
            if (currentInstance.currentActivitylogData != undefined && currentInstance.currentActivitylogData != null) {
                activityLogDataToBeSaved = currentInstance.currentActivitylogData.filter(function (ct) {
                    return ct.IsNewRecord == true;
                });
            }
            if (!$.isEmptyObject(activityLogDataToBeSaved)) {
                var formInstanceActivityLogdata = {
                    tenantId: currentInstance.tenantId,
                    formInstanceId: parseInt(currentInstance.formInstanceId),
                    folderId: parseInt(currentInstance.folderId),
                    folderVersionId: parseInt(currentInstance.folderVersionId),
                    formDesignId: currentInstance.formDesignId,
                    formDesignVersionId: currentInstance.formDesignVersionId,

                    activityLogFormInstanceData: JSON.stringify(activityLogDataToBeSaved)
                }
                var promise = ajaxWrapper.postJSONCustom(activityLogDataURL, formInstanceActivityLogdata, false);
                promise.done(function (xhr) {
                    if (xhr.Result == ServiceResult.FAILURE) {
                        messageDialog.show("Error occured while saving 'Activity Log data to databases");

                    }
                    else if (xhr.Result == ServiceResult.SUCCESS) {
                        if (!$.isEmptyObject(currentInstance.currentActivitylogData)) {
                            for (i = 0 ; i < currentInstance.currentActivitylogData.length; i++) {
                                currentInstance.currentActivitylogData[i].IsNewRecord = false;
                            }
                        }
                    }
                });
            }
        },

        saveDocumentViewImpactLogData: function () {
            var impactLogDataURL = currentInstance.URLs.saveDocumentViewImpactLog;

            var activityLogDataToBeSaved = [];

            currentInstance.currentActivitylogData = activitylogger.getActivityLogFormInstanceData(parseInt(currentInstance.formInstanceId));
            if (currentInstance.currentActivitylogData != undefined && currentInstance.currentActivitylogData != null) {
                activityLogDataToBeSaved = currentInstance.currentActivitylogData.filter(function (ct) {
                    return ct.IsNewRecord == true;
                });
            }
            if (!$.isEmptyObject(activityLogDataToBeSaved)) {
                var formInstanceActivityLogdata = {
                    tenantId: currentInstance.tenantId,
                    formInstanceId: parseInt(currentInstance.formInstanceId),
                    folderId: parseInt(currentInstance.folderId),
                    folderVersionId: parseInt(currentInstance.folderVersionId),
                    formDesignId: currentInstance.formDesignId,
                    formDesignVersionId: currentInstance.formDesignVersionId,

                    activityLogFormInstanceData: JSON.stringify(activityLogDataToBeSaved)
                }
                var promise = ajaxWrapper.postJSONCustom(impactLogDataURL, formInstanceActivityLogdata, false);
                promise.done(function (xhr) {
                    if (xhr.Result == ServiceResult.FAILURE) {
                        messageDialog.show("Error occured while saving 'View Impact Log data to databases");

                    }
                    else {
                        currentInstance.impactList = xhr;
                        currentInstance.pbpViewAction.fillImpactGrid(xhr);
                    }
                });
            }
        },

        initRadioButtonEvent: function (element, formData, sectionName) {

            function setJSRenderProperty(elementFullName, value) {
                var obj = {};
                if (value == 'No') {
                    value = "";
                }
                obj[elementFullName] = value;
                $.observable(currentInstance.formData).setProperty(obj);
            }
            $(':radio').mousedown(function (e) {
                var $self = $(this);
                if ($self.is(':checked')) {
                    var uncheck = function () {
                        var value = 'No';
                        var path = $self.attr('data-link');
                        if (path != null || path !== undefined) {
                            var dataLinkPath = $self.attr('data-link').split(':');
                            setTimeout(function () { $self.removeAttr('checked'); }, 0);
                            setJSRenderProperty(dataLinkPath[1], value);
                        }
                    };
                    var unbind = function () {
                        $self.unbind('mouseup', up);
                    };
                    var up = function () {
                        uncheck();
                        unbind();
                    };
                    $self.bind('mouseup', up);
                    $self.one('mouseout', unbind);
                }
            });
        },
    }
}

formInstanceBuilder.prototype.validationMethods = function () {
    var currentInstance = this;

    function getSectionInfo(sectionName) {
        var sectionInfo;
        for (var section in currentInstance.sections) {
            if (currentInstance.sections[section].FullName == sectionName) {
                sectionInfo = currentInstance.sections[section];
                break;
            }
        }
        return sectionInfo;
    }

    function getSectionDesignDetails(sectionName) {
        var sectionDetails = currentInstance.designData.Sections.filter(function (ct) {
            return ct.FullName == sectionName;
        });
        return sectionDetails[0];
    }

    function getErrorGridRowObject(validationErrorObject) {
        var sectionNamePath = validationErrorObject.FullName.substring(0, validationErrorObject.FullName.lastIndexOf('.'));
        var sectionName = validationErrorObject.FullName.substring(0, sectionNamePath.indexOf('.'));

        if (sectionName == "")
            sectionName = sectionNamePath;

        var sectionInfo = getSectionInfo(sectionName);
        if (sectionInfo) {
            var sectionDetails = getSectionDesignDetails(sectionName);
            var subSectionDetails = getElementDetails(sectionDetails, sectionNamePath);
            if (sectionDetails) {
                try {
                    var elementData = getElementDetails(sectionDetails, validationErrorObject.FullName);
                    if (currentInstance.errorGridData.length > 0)
                        var selectedSectionErrorGridData = currentInstance.errorGridData.filter(function (errorData) {
                            return errorData.Section == currentInstance.selectedSectionName;
                        });
                    var newRowID = currentInstance.errorGridData.length + 1;;
                    if (selectedSectionErrorGridData && selectedSectionErrorGridData.length > 0) {
                        newRowID = Enumerable.From(selectedSectionErrorGridData[0].ErrorRows).Select(function (x) { return x.ID }).Max(function (e) { return e; }) + 1;
                    }
                    if (elementData) {
                        if (elementData.Visible == true) {
                            var errorRow = {};
                            errorRow.FormInstanceBuilder = currentInstance;
                            //errorRow.ID = selectedSectionErrorGridData && selectedSectionErrorGridData.length > 0 ? selectedSectionErrorGridData[0].ErrorRows.length + 1 : currentInstance.errorGridData.length + 1;
                            errorRow.ID = newRowID;
                            errorRow.Form = currentInstance.formName;
                            errorRow.FormInstance = currentInstance.formInstanceDivJQ.replace('#', '');
                            errorRow.SectionID = sectionInfo.SectionID;
                            errorRow.Section = sectionInfo.SectionName;
                            errorRow.FormInstanceID = currentInstance.formInstanceId;
                            errorRow.SubSectionName = subSectionDetails ? subSectionDetails.Label : "";
                            errorRow.SubSectionName = sectionInfo.SectionName + getElementHierarchy(sectionDetails, validationErrorObject.FullName);
                            if (elementData.Name.indexOf("Repeater") != -1) {
                                errorRow.SubSectionName = validationErrorObject.parentElementFullPath.split('.').join(" => ");
                            }
                            errorRow.Field = elementData.Label;
                            if (elementData["DataSourceGeneratedName"]) {
                                errorRow.GeneratedName = elementData["DataSourceGeneratedName"];
                                errorRow.ColumnNumber = validationErrorObject.ColumnNumber;
                            }
                            else {
                                errorRow.GeneratedName = validationErrorObject.GeneratedName;
                                errorRow.ColumnNumber = 0;
                            }

                            var rowNum;
                            var keyValue;
                            if (validationErrorObject.RowNumber != null && validationErrorObject.RowNumber != "" && validationErrorObject.RowNumber != "0") {
                                if (validationErrorObject.RowNumber.toString().indexOf("|") >= 0) {
                                    rowNum = validationErrorObject.RowNumber.substring(0, validationErrorObject.RowNumber.indexOf("|"));
                                    keyValue = validationErrorObject.RowNumber.substring(validationErrorObject.RowNumber.indexOf("|") + 1, validationErrorObject.RowNumber.length + 1);
                                }
                                else {
                                    rowNum = validationErrorObject.RowNumber;
                                    keyValue = rowNum;
                                }
                            }
                            errorRow.RowIdProperty = validationErrorObject.RowIdProperty;
                            errorRow.RowNum = rowNum;
                            errorRow.KeyValue = keyValue;
                            errorRow.Description = validationErrorObject.Message.replace('{0}', errorRow.Field);
                            errorRow.Value = validationErrorObject.value;

                            if (validationErrorObject.UIElementName.indexOf('_') > 1) {
                                var elementElementID = validationErrorObject.UIElementName.split('_');
                                var elementNameID = elementElementID[elementElementID.length - 1];
                                var parentelementID = elementElementID[elementElementID.length - elementElementID.length];
                                rowNumber = validationErrorObject.RowIdProperty;
                                if (validationErrorObject.RowNumber.indexOf("|") != -1) {
                                    var rowNumber = validationErrorObject.RowNumber.toString().split('|');
                                    if (rowNumber != undefined) {
                                        rowNumber = rowNumber[0];
                                    }
                                }
                                errorRow.ElementID = parentelementID + "_" + rowNumber + "_" + elementNameID + currentInstance.formInstanceId;
                            }
                            else {
                                errorRow.ElementID = validationErrorObject.UIElementName + currentInstance.formInstanceId;
                            }
                        }
                    }
                } catch (e) {
                    console.log(e);
                }

                return errorRow;
            }
        }
    }

    function populateErrorGrid(validationErrorList) {
        currentInstance.errorGridData = new Array();
        for (var i = 0; i < validationErrorList.length; i++) {
            var designObject = validationErrorList[i];
            if (designObject) {
                if (designObject.hasValidationError) {
                    var errorGridRow = getErrorGridRowObject(designObject);
                    if (errorGridRow) {
                        var sectionRow = undefined;
                        var sections = currentInstance.errorGridData.filter(function (ct) {
                            return ct.SectionID == errorGridRow.SectionID
                                && ct.Section == errorGridRow.Section
                                && ct.Form == errorGridRow.Form;
                        });
                        if (sections != null && sections.length > 0) {
                            isExists = true;
                            sectionRow = sections[0];
                        }
                        if (!sectionRow) {
                            var errorRows = new Array();
                            errorRows.push(errorGridRow);

                            sectionRow = {
                                SectionID: errorGridRow.SectionID,
                                Section: errorGridRow.Section,
                                Form: errorGridRow.Form,
                                FormInstanceID: currentInstance.formInstanceId,
                                ID: currentInstance.errorGridData.length + 1,
                                ErrorRows: errorRows,
                                FormInstanceBuilder: currentInstance,
                            };

                            currentInstance.errorGridData.push(sectionRow);
                        }
                        else {
                            var sections = currentInstance.errorGridData.filter(function (ct) {
                                return ct.SectionID == errorGridRow.SectionID
                                    && ct.Section == errorGridRow.Section
                                    && ct.Form == errorGridRow.Form;
                            });
                            if (sections != null && sections.length > 0) {
                                var section = sections[0];
                                section.ErrorRows.push(errorGridRow);
                            }
                        }
                    }
                }
            }
        }
        var array = [];
        for (var m = 0; m < currentInstance.errorGridData.length; m++) {
            for (var t = 0; t < currentInstance.errorGridData[m].ErrorRows.length; t++) {
                var errorGridRow = currentInstance.errorGridData[m].ErrorRows[t];
                if (errorGridRow.RowNum != "") {
                    var repeaterID = errorGridRow.ElementID.substring(0, errorGridRow.ElementID.indexOf("_")) + errorGridRow.FormInstanceID;


                    var isrepeaterExist = false;
                    for (var k = 0; k < array.length; k++) {
                        if (array[k] == repeaterID) {
                            isrepeaterExist = true
                        }
                    }

                    if (isrepeaterExist == false) {
                        var repeaterIndex = new Array();
                        var rowNum = $("#" + repeaterID).getGridParam('rowNum');
                        var allRecords = $("#" + repeaterID).getGridParam('records');
                        var rowNum = $("#" + repeaterID).getGridParam('rowNum');
                        var totalPages = parseInt((allRecords / rowNum) + 1);

                        if (totalPages == 1)
                            return false;
                        currentPage = $("#" + repeaterID).getGridParam('page');

                        var ID = $("#" + repeaterID).jqGrid("getDataIDs");

                        for (p = 1; p <= totalPages; p++) {
                            if (p == 1) {
                                Paging.ISPAGING = 1;
                            }
                            else {
                                Paging.ISPAGING = 0;
                            }
                            $("#" + repeaterID).trigger("reloadGrid", [{
                                page: p
                            }]);

                            var ID = $("#" + repeaterID).jqGrid("getDataIDs");
                            repeaterIndex.push(ID);

                            if (p == totalPages) {
                                $("#" + repeaterID).trigger("reloadGrid", [{
                                    page: currentPage
                                }]);
                            }
                        }
                        array[array.length] = repeaterID;
                    }

                    var index = undefined;
                    var page = undefined;

                    if (repeaterIndex != undefined) {
                        $.each(repeaterIndex, function (p, indx) {
                            $.each(indx, function (idx, ct) {
                                if (ct == errorGridRow.RowIdProperty) {
                                    index = idx + 1;
                                    page = p;
                                    return false;
                                }
                            });
                        });

                        if (index != undefined)
                            var rowNum = $("#" + repeaterID).getGridParam('rowNum');
                        errorGridRow.RowNum = ((page) * rowNum) + index;
                    }
                }
            }
        }
        currentInstance.validation.removeVisibleAndDisabledControls();
    }

    function checkErrorRowExistsInErrorGridData(errorRow) {
        var isExists = false;
        var sections = currentInstance.errorGridData.filter(function (ct) {
            return ct.SectionID == errorRow.SectionID && ct.Section == errorRow.Section && ct.Form == errorRow.Form;
        });
        if (sections != null && sections.length > 0) {
            isExists = true;
        }
        return isExists;
    }

    function checkChildErrorRowExistsInErrorGridData(errorRow) {
        try {
            var isExists = false;
            var sections = currentInstance.errorGridData.filter(function (ct) {
                return ct.SectionID == errorRow.SectionID && ct.Section == errorRow.Section && ct.Form == errorRow.Form;
            });
            if (sections != null && sections.length > 0) {
                var section = sections[0];
                var rows = section.ErrorRows.filter(function (d) {
                    return d.ElementID == errorRow.ElementID && d.Form == errorRow.Form && d.FormInstance == errorRow.FormInstance
                    && d.SectionID == errorRow.SectionID && d.Section == errorRow.Section && d.Field == errorRow.Field
                    && d.Description == errorRow.Description && d.RowIdProperty == errorRow.RowIdProperty;
                });
                if (rows != null && rows.length > 0) {
                    isExists = true;
                }
            }
        } catch (e) {
            //alert(e);
        }
        return isExists;
    }

    function handleObjectChangeValidation(validationError) {
        var errorRow = currentInstance.validation.getErrorGridRowObject(validationError);
        if (!errorRow)
            return false;
        if (validationError.hasValidationError && !currentInstance.validation.checkErrorRowExistsInErrorGridData(errorRow)) {
            var errorRowsArray = new Array();
            errorRowsArray.push(errorRow);
            var errorGridRow = {
                SectionID: errorRow.SectionID,
                Section: errorRow.Section,
                FormInstanceID: currentInstance.formInstanceId,
                Form: errorRow.Form,
                ErrorRows: errorRowsArray,
                FormInstanceBuilder: currentInstance
            };

            currentInstance.errorGridData.push(errorGridRow);
        }
        else if (validationError.hasValidationError && currentInstance.validation.checkErrorRowExistsInErrorGridData(errorRow)) {
            if (currentInstance.validation.checkChildErrorRowExistsInErrorGridData(errorRow)) {
                //remove previously existing row
                var isDuplicateRow = false;
                var sections = currentInstance.errorGridData.filter(function (ct) {
                    return ct.SectionID == errorRow.SectionID;
                });
                if (sections != null && sections.length > 0) {
                    var section = sections[0];
                    if (validationError.CheckDuplicate) {
                        for (var i = 0; i < section.ErrorRows.length; i++) {
                            if (section.ErrorRows[i].ElementID == errorRow.ElementID && errorRow.RowIdProperty == section.ErrorRows[i].RowIdProperty && section.ErrorRows[i].Value == "CheckDuplicate") {
                                isDuplicateRow = true;
                            }
                        }
                        //add child row
                        if (!isDuplicateRow) {
                            currentInstance.validation.pushValidationErrorRow(errorRow);
                        }
                    }
                }
            }
            else {
                var sections = currentInstance.errorGridData.filter(function (ct) {
                    return ct.SectionID == errorRow.SectionID && ct.Section == errorRow.Section && ct.Form == errorRow.Form;
                });
                if (sections != null && sections.length > 0) {
                    var section = sections[0];
                    for (var i = 0; i < section.ErrorRows.length; i++) {
                        if ((errorRow.RowIdProperty == 0) || (errorRow.RowIdProperty != "" && errorRow.RowIdProperty != undefined)) {
                            if (section.ErrorRows[i].ElementID == errorRow.ElementID && errorRow.RowIdProperty == section.ErrorRows[i].RowIdProperty && errorRow.Value != "CheckDuplicate" && section.ErrorRows[i].Value != "CheckDuplicate"
                               && section.ErrorRows[i].Description == errorRow.Description) {
                                section.ErrorRows.splice(i, 1);
                            }
                        }
                        else {
                            if (section.ErrorRows[i].ElementID == errorRow.ElementID && errorRow.RowNum == section.ErrorRows[i].RowNum && errorRow.Value != "CheckDuplicate") {
                                section.ErrorRows.splice(i, 1);
                            }
                        }
                    }
                }
                currentInstance.validation.pushValidationErrorRow(errorRow);
            }
        }
        else if (!validationError.hasValidationError) {
            var sections = currentInstance.errorGridData.filter(function (ct) {
                return ct.SectionID == errorRow.SectionID;
            });
            if (sections != null && sections.length > 0) {
                var section = sections[0];
                if (validationError.CheckDuplicate) {
                    for (var i = 0; i < section.ErrorRows.length; i++) {
                        if (section.ErrorRows[i].ElementID == errorRow.ElementID && errorRow.RowIdProperty == section.ErrorRows[i].RowIdProperty && section.ErrorRows[i].Value == "CheckDuplicate") {
                            section.ErrorRows.splice(i, 1);
                        }
                    }
                }
                else {
                    for (var i = 0; i < section.ErrorRows.length; i++) {
                        if ((errorRow.RowIdProperty == 0) || (errorRow.RowIdProperty != "" && errorRow.RowIdProperty != undefined)) {
                            if (section.ErrorRows[i].ElementID == errorRow.ElementID && errorRow.RowIdProperty == section.ErrorRows[i].RowIdProperty && errorRow.Value != "CheckDuplicate" && section.ErrorRows[i].Value != "CheckDuplicate"
                               && section.ErrorRows[i].Description == errorRow.Description) {
                                section.ErrorRows.splice(i, 1);
                            }
                        }
                        else {
                            if (section.ErrorRows[i].ElementID == errorRow.ElementID && errorRow.RowIdProperty == section.ErrorRows[i].RowIdProperty && errorRow.Value != "CheckDuplicate" && section.ErrorRows[i].Value != "CheckDuplicate") {
                                section.ErrorRows.splice(i, 1);
                            }
                        }
                    }
                }
            }

            for (var i = 0; i < currentInstance.errorGridData.length; i++) {
                if (currentInstance.errorGridData[i].SectionID == errorRow.SectionID) {
                    if (currentInstance.errorGridData[i].ErrorRows.length == 0) {
                        currentInstance.errorGridData.splice(i, 1);
                    }
                }
            }
        }
    }

    function showValidatedControlsOnSectionElementChange(validationError) {
        var errorRow = currentInstance.validation.getErrorGridRowObject(validationError);
        if (!errorRow)
            return false;
        var sections = currentInstance.errorGridData.filter(function (ct) {
            return ct.SectionID == errorRow.SectionID && ct.Section == errorRow.Section && ct.Form == errorRow.Form;
        });

        var isDropDownFilter = false;
        var sectionDetails = currentInstance.designData.Sections.filter(function (ct) {
            return ct.Label == errorRow.Section;
        });
        if (sectionDetails !== undefined && sectionDetails !== null && sectionDetails.length > 0) {
            var elementDetails = getElementDetails(sectionDetails[0], sectionDetails[0].FullName + '.' + errorRow.GeneratedName);
            if (elementDetails !== undefined && elementDetails !== null) {
                if ((elementDetails.Type == 'select' || elementDetails.Type == 'SelectInput') && elementDetails.IsDropDownFilterable) {
                    isDropDownFilter = true;
                }
            }
        }
        if (sections.length > 0) {
            var row = sections[0].ErrorRows.filter(function (ct) {
                return ct.SubSectionName == errorRow.SubSectionName && ct.GeneratedName == errorRow.GeneratedName && ct.ElementID == errorRow.ElementID;
            });

            if (row.length != 0) {
                if (isDropDownFilter) {
                    $(currentInstance.formInstanceDivJQ).find("#" + validationError.UIElementName + currentInstance.formInstanceId + 'DDL').css('border-color', 'red');
                    $(currentInstance.formInstanceDivJQ).find("#" + validationError.UIElementName + currentInstance.formInstanceId + 'DDL').parent().find('a').css('border-color', 'red');

                } else {
                    $(currentInstance.formInstanceDivJQ).find("#" + validationError.UIElementName + currentInstance.formInstanceId).parent().addClass("has-error");
                    //addAsterisk(row, validationError);
                }
            }
            else {
                if (isDropDownFilter) {
                    $(currentInstance.formInstanceDivJQ).find("#" + validationError.UIElementName + currentInstance.formInstanceId + 'DDL').css('border-color', '');
                    $(currentInstance.formInstanceDivJQ).find("#" + validationError.UIElementName + currentInstance.formInstanceId + 'DDL').parent().find('a').css('border-color', '');
                } else {
                    $(currentInstance.formInstanceDivJQ).find("#" + validationError.UIElementName + currentInstance.formInstanceId).parent().removeClass("has-error");
                    //removeAsterisk(validationError);
                }
            }
        }
        else {
            if (isDropDownFilter) {
                $(currentInstance.formInstanceDivJQ).find("#" + validationError.UIElementName + currentInstance.formInstanceId + 'DDL').css('border-color', '');
                $(currentInstance.formInstanceDivJQ).find("#" + validationError.UIElementName + currentInstance.formInstanceId + 'DDL').parent().find('a').css('border-color', '');
            } else {
                $(currentInstance.formInstanceDivJQ).find("#" + validationError.UIElementName + currentInstance.formInstanceId).parent().removeClass("has-error");
                //removeAsterisk(validationError);
            }
        }

        currentInstance.bottomMenu.closeBottomMenu();
        currentInstance.validation.loadValidationErrorGrid();
    }

    function loadValidationErrorGrid() {
        currentInstance.errorManager.buildErrorGrid();
        currentInstance.errorManager.loadErrorGridData(currentInstance.errorGridData);
    }

    function removeValidationErrors() {
        $(currentInstance.formInstanceDivJQ + " .has-error").removeClass("has-error");
        $(currentInstance.formInstanceDivJQ + " .repeater-has-error").removeClass("repeater-has-error");
    }

    function removeDuplicationMessage() {
        $.each(currentInstance.repeaterBuilders, function (idx, ct) {
            ct.isValidateDuplicate = true;
        });
    }
    function getRepeaterInstanceById(currentInstance, generetedName) {
        var repToResizeColumn = currentInstance.repeaterBuilders.filter(function (elemName) {
            return elemName.design.Name == generetedName;
        });
    }
    function handleDateRangeValidation(currentValue, minValue, maxValue, control) {
        if (currentValue != undefined || currentInstance != "") {
            if ((minValue != undefined || minValue != "") && (maxValue === undefined || maxValue == "")) {
                if (!validationManager.applyMinValueValidation(currentValue, minValue, control)) {
                    $(control).val('');
                }
            }
            else if ((minValue === undefined || minValue == "") && (maxValue != undefined || maxValue != "")) {
                if (!validationManager.applyMaxValueValidation(currentValue, maxValue, control)) {
                    $(control).val('');
                }
            }
            else if ((minValue != undefined || minValue != "") && (maxValue != undefined || maxValue != "")) {
                if (!validationManager.applyRangeValidation(currentValue, minValue, maxValue, control)) {
                    $(control).val('');
                }
            }
        }
    }

    function getValidationDataFromServer(isValidateAllDocs, callbackMetaDeta, onlyCurrentSection) {
        if (onlyCurrentSection == undefined) {
            onlyCurrentSection = false
        }
        var sectionDetails = currentInstance.form.getSectionData(currentInstance.selectedSection);
        var sectionData = currentInstance.formData[sectionDetails.FullName];
        var formInstanceData = {
            tenantId: currentInstance.tenantId,
            formInstanceId: currentInstance.formInstanceId,
            folderVersionId: currentInstance.folderVersionId,
            folderId: currentInstance.folderId,
            sectionData: JSON.stringify(sectionData),
            sectionName: sectionDetails.FullName,
            isValidateAllDocs: isValidateAllDocs,
            formDesignVersionId: currentInstance.formDesignVersionId,
            validateOnlyCurrentSection: onlyCurrentSection
        }

        var promise = ajaxWrapper.postJSON(currentInstance.URLs.validateFormInstanceAtServer, formInstanceData);
        promise.done(function (xhr) {
            if (xhr != null && xhr != undefined) {
                var currentFolderInstance = folderManager.getInstance();
                //currentFolderInstance.fillErrorGridData(xhr);

                $(currentInstance.formInstanceDivJQ + " #section" + currentInstance.selectedSection + " input").first().focus();
                var hasValidationErrors = false;
                for (var i = 0; i < xhr.length; i++) {
                    if (xhr[i].ErrorList.length != 0) {
                        hasValidationErrors = true;
                        currentInstance.form.showValidationErrorsOnForm(xhr[i].FormInstanceID);
                    }
                    else {
                        currentInstance.form.hideValidationErrorsOnForm(xhr[i].FormInstanceID);
                    }
                }

                var currentSelectedSection = currentInstance.selectedSection;
                if (currentInstance.formInstanceId != folderManager.getInstance().getFolderInstance().currentFormInstanceID) {
                    var ParentViewFormInstance = folderManager.getInstance().getFolderInstance();
                    var childViewFormInstance = ParentViewFormInstance.formInstances[ParentViewFormInstance.currentFormInstanceID].FormInstanceBuilder;
                    if (ParentViewFormInstance != undefined) {
                        currentInstance.formInstanceId = childViewFormInstance.formInstanceId;
                    }
                }
                if (currentInstance.formInstanceId != undefined) {
                    currentFolderInstance.fillErrorGridData(xhr, currentInstance.formInstanceId);
                    currentInstance.menu.clearMenuSelection();
                    currentInstance.menu.selectFirstSection();
                    if (currentInstance.selectedSection) {
                        if (currentInstance.sections[currentInstance.selectedSection].IsLoaded == false) {
                            currentInstance.selectedSection = currentSelectedSection;
                        }
                    }
                    else {
                        currentInstance.selectedSection = currentSelectedSection;
                    }
                    //currentInstance.menu.setSectionMenuSelection(currentInstance.selectedSection);
                    currentInstance.validation.loadValidationErrorGrid();
                    currentInstance.formValidationManager.showValidatedControls();
                    var currentSectionRepeaterBuilders = currentInstance.repeaterBuilders.filter(function (item) {
                        if (currentInstance.currentSectionRepeaterIDs.indexOf(item.gridElementId) > -1) { return item; }
                    });
                    $.each(currentSectionRepeaterBuilders, function (idx, repeater) {
                        repeater.showValidatedElements();
                    });
                    currentInstance.bottomMenu.openBottomMenu();
                    currentInstance.bottomMenu.showJournalErrorGrid();
                }
                if (callbackMetaDeta != null && callbackMetaDeta != undefined) {
                    callbackMetaDeta.callback(callbackMetaDeta.args, hasValidationErrors);
                }
            }
        });
        promise.fail(this.showerror);
    }

    function addAsterisk(row, validationError) {
        if (row.length > 0) {
            if (row[0].Description.indexOf("is required.") > -1) {
                if (validationError.UIElementName != undefined) {
                    var text = $("label[for=" + validationError.UIElementName + "]").text().replace("*", "");
                    text = text + " <em>*</em>";
                    $("label[for=" + validationError.UIElementName + "]").html(text);
                }
            }
        }
    }

    function removeAsterisk(validationError) {
        if (validationError != undefined) {
            if (validationError.IsRequiredError == false) {
                var text = $("label[for=" + validationError.UIElementName + "]").text().replace("*", "");
                $("label[for=" + validationError.UIElementName + "]").text(text);
            }
        }
    }

    return {
        validateFormInstance: function (isValidateAllDocs, callbackMetaDeta, onlyCurrentSection) {
            try {
                removeValidationErrors();
                removeDuplicationMessage();
                currentInstance.errorGridData = new Array();

                for (var idx = 0; idx < currentInstance.repeaterBuilders.length; idx++) {
                    currentInstance.repeaterBuilders[idx].getData();
                }

                getValidationDataFromServer(isValidateAllDocs, callbackMetaDeta, onlyCurrentSection);

            } catch (e) {
                alert(e);
                messageDialog.show(Common.errorMsg);
            }
        },
        loadValidationErrorGrid: function () {
            for (var i = 0; i < currentInstance.errorGridData.length; i++) {
                if (currentInstance.errorGridData[i].ErrorRows.length == 0) {
                    currentInstance.errorGridData.splice(i, 1);
                }
            }
            loadValidationErrorGrid();
        },
        getErrorGridRowObject: function (validationErrorObject) {
            return getErrorGridRowObject(validationErrorObject);
        },
        checkErrorRowExistsInErrorGridData: function (errrorRow) {
            return checkErrorRowExistsInErrorGridData(errrorRow);
        },
        checkChildErrorRowExistsInErrorGridData: function (errorRow) {
            return checkChildErrorRowExistsInErrorGridData(errorRow);
        },
        removeVisibleAndDisabledControls: function () {
            currentInstance.formValidationManager.removeVisibleAndDisabledControls();
        },
        removeInvalidValidationErrorRows: function (counter, selectedSectionID, elementID, rowNum) {
            var sections = currentInstance.errorGridData.filter(function (ct) {
                return ct.SectionID == selectedSectionID;
            });
            if (sections != null && sections.length > 0) {
                var section = sections[0];
                for (var i = 0; i < section.ErrorRows.length; i++) {
                    if (section.ErrorRows[i].ElementID == elementID && section.ErrorRows[i].RowNum == rowNum) {
                        section.ErrorRows.splice(i, 1);
                    }
                }

            }
        },
        pushValidationErrorRow: function (errorRow) {
            var sections = currentInstance.errorGridData.filter(function (ct) {
                return ct.SectionID == errorRow.SectionID && ct.Section == errorRow.Section && ct.Form == errorRow.Form;
            });
            if (sections != null && sections.length > 0) {
                sections[0].ErrorRows.push(errorRow);
            }
        },
        handleDateRangeValidation: function (currentValue, minValue, maxValue, control) {
            return handleDateRangeValidation(currentValue, minValue, maxValue, control);
        },
        handleObjectChangeValidation: function (validationError) {
            handleObjectChangeValidation(validationError);
        },
        showValidatedControlsOnSectionElementChange: function (validationError) {
            showValidatedControlsOnSectionElementChange(validationError);
        },
        populateErrorGrid: function (validationErrorList) {
            populateErrorGrid(validationErrorList);
        }
    }
}

formInstanceBuilder.prototype.rulesMethods = function () {
    var currentInstance = this;
    function getElementRules(elementFullName) {
        var elemRuleMap;
        if (currentInstance.designData != undefined) {
            var designData = currentInstance.designData;
            var elemRuleMaps = designData.ElementRuleMaps.filter(function (elem) {
                return elem.FullName == elementFullName;
            });
            if (elemRuleMaps != null && elemRuleMaps.length > 0) {
                elemRuleMap = elemRuleMaps[0];
            }
        }
        return elemRuleMap;
    }

    function getRule(ruleId) {
        var rule;
        var designData = currentInstance.designData;
        var UIElementGeneratedName = designData.Sections.filter(function (section) {
            return (section.Label == currentInstance.selectedSectionName);
        });
        var rules = designData.Rules.filter(function (rule) {
            return rule.RuleID == ruleId && (rule.UIElementFullName.split('.')[0] == UIElementGeneratedName[0].GeneratedName || rule.UIElementFullName.indexOf('.') < 0);
        });
        if (rules != null && rules.length > 0) {
            rule = rules[0];
        }
        return rule;
    }

    function setJSRenderProperty(rule, elementFullName, value, uiElementId) {
        var obj = {};
        if (rule.UIElementTypeID == 1 || rule.UIElementTypeID == 2) {
            if (value == 'Yes') {
                value = true;
            }
            else if (value == 'No') {
                value = false;
            }
        }
        if (rule.UIElementTypeID == 10 && rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Value) {
            $("#" + uiElementId).text(value);
        }
        // If UIElement Is DropDownTextbox and has Elementvalue Rule
        if (rule.UIElementTypeID == 12 && rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Value) {
            var stringUtility = new globalutilities();
            if (!stringUtility.stringMethods.isNullOrEmpty(value)) {
                if ($("#" + uiElementId).find("option[value='" + value + "']").length == 0) {
                    $('<option value="' + value + '" class="non-standard-optn">' + value + '</option>').insertBefore($("#" + uiElementId).find('option').last());
                }
            }
        }
        obj[elementFullName] = value;
        $.observable(currentInstance.formData).setProperty(obj);
    }

    function isLabelValue(fullname) {
        var data = getDataProperty(fullname, currentInstance.formData);
        if (data != "") {
            return data;
        }
        else {
            return true;
        }
    }

    function setElementDialogState(result, rule) {
        var uiElementId = rule.UIElementFormName + currentInstance.formInstanceId;
        if (result == true) {
            messageDialog.show(rule.Message);
        }
    }

    function setElementCustomRuleState(result, rule) {
        var leftoperandpath;
        var rightoperandpath;
        var leftValue;
        var rightValue;
        var leftArray = [];
        $.each(rule.Expressions, function (index, item) {
            if (item.OperatorTypeId == 9) {
                leftoperandpath = item.LeftOperandName;
                leftValue = currentInstance.ruleProcessor.getOperandValue(leftoperandpath);
                if (item.RightOperandName == null) {
                    rightValue = item.RightOperand
                }
                else {
                    rightoperandpath = item.RightOperandName;
                    rightValue = currentInstance.ruleProcessor.getOperandValue(rightoperandpath);
                }
            }
            if (item.OperatorTypeId == 5) {
                leftoperandpath = item.LeftOperandName;
                leftValue = currentInstance.ruleProcessor.getOperandValue(leftoperandpath);
                leftArray.push(leftValue);
                if (item.RightOperandName == null) {
                    rightValue = item.RightOperand
                }
            }

        });

        var uiElementId = rule.UIElementFormName + currentInstance.formInstanceId;
        var result = "";
        if (rule.SuccessValue == "SumOf") {
            result = sumOfElementValues(leftArray);
            $("#" + uiElementId).val(result);
            $("#" + uiElementId).trigger('change');
        } else if (rule.SuccessValue == "percentageOf") {
            result = percentageOf(leftArray);
            $("#" + uiElementId).val(result.toString() + "%");
        }
        else if (rule.SuccessValue != "") {
            //var functionToExecute = "currentInstance.ruleProcessor." + rule.SuccessValue; //"currentInstance.ruleProcessor." has to be replaced with the file which could be containing rule function;
            var functionToExecute = rule.SuccessValue;
            fn = eval(functionToExecute);
            var functionoutput = fn(leftValue, rightValue);
            var elem = $('#' + rule.UIElementFormName + currentInstance.formInstanceId);
            if (currentInstance.designData.Validations == null) {
                currentInstance.designData.Validations = [];
            }
            var validation = currentInstance.designData.Validations.filter(function (val) {
                return val.UIElementName == rule.UIElementFormName;
            });
            if (functionoutput) {
                if (validation != null && validation.length > 0) {
                    validation[0].ValidationMessage = "";
                    validation[0].IsError = false;
                }
                else {
                    validation = { FullName: rule.UIElementFullName, UIElementName: rule.UIElementFormName, IsRequired: true, IsError: false, Regex: '', ValidationMessage: rule.Message, HasMaxLength: '', MaxLength: '', DataType: '', IsActive: true, ValidationType: 'Temporary' };
                    currentInstance.designData.Validations.push(validation);
                }
            }
            else {
                if (validation != null && validation.length > 0) {
                    validation[0].IsError = true;
                    validation[0].ValidationMessage = rule.Message == "" ? "Custome rule failed" : rule.Message;
                }
                else {
                    validation = { FullName: rule.UIElementFullName, UIElementName: rule.UIElementFormName, IsRequired: false, IsError: true, Regex: '', ValidationMessage: '', HasMaxLength: '', MaxLength: '', DataType: '', IsActive: true, ValidationType: 'Temporary' };
                    currentInstance.designData.Validations.push(validation);
                }
            }
            var val = currentInstance.ruleProcessor.getOperandValue(rule.UIElementFullName);
            var validationError = currentInstance.formValidationManager.handleValidation(rule.UIElementFullName, val);
            if (validationError) {
                currentInstance.validation.handleObjectChangeValidation(validationError);
                currentInstance.validation.showValidatedControlsOnSectionElementChange(validationError);
            }
        }
    }

    function CheckInterval(left, right) {
        var result = true;
        try {
            if (left.indexOf('-') >= 0 && right.indexOf('-') >= 0) {
                var endDay = left.split('-')[1], beginDay = right.split('-')[0].split(' ')[1];
                result = (parseFloat(endDay) + 1) == parseFloat(beginDay);
            }
            else
                result = (parseFloat(left) + 1) == parseFloat(right);
        }
        catch (ex) {
            return true;
        }
        return result;
    }

    function checkServiceIsSelected(left, right) {
        var breakLoop = false;
        try {

            if (left != "" && right != "") {
                for (var i = 0; i < right.length; i++) {
                    if (left.indexOf(right[i]) == -1) {
                        breakLoop = true;
                        break;
                    }
                }
                if (!breakLoop)
                    return true;
            }

        }
        catch (ex) {
            return false;
        }
    }
    function AdditionalDaysInterval(left, right) {
        var result = true;
        try {
        }
        catch (ex) {
            return true;
        }

        return result;
    }
    function checkIfOptionalBenefit(left, right) {
        var result = true;
        try {
        }
        catch (ex) {
            return true;
        }

        return result;
    }
    function AdditionalDaysMaxTierEndDay(left, right) {
        var result = true;
        try {
        }
        catch (ex) {
            return true;
        }

        return result;
    }

    function setElementValue(result, rule) {
        var uiElementId = rule.UIElementFormName + currentInstance.formInstanceId;
        if (result == true) {

            if (rule.IsResultSuccessElement == true) {
                if (rule.SuccessValueFullName != rule.UIElementFullName) {
                    var successValue = currentInstance.ruleProcessor.getOperandValue(rule.SuccessValueFullName);
                    setJSRenderProperty(rule, rule.UIElementFullName, successValue, uiElementId);
                }
            }
            else {
                if (rule.SuccessValue !== null && rule.SuccessValue !== undefined) {
                    setJSRenderProperty(rule, rule.UIElementFullName, rule.SuccessValue, uiElementId);
                }
            }
        }
        else {
            if (rule.IsResultFailureElement == true) {
                if (rule.FailureValueFullName != rule.UIElementFullName) {
                    var failureValue = currentInstance.ruleProcessor.getOperandValue(rule.FailureValueFullName);
                    setJSRenderProperty(rule, rule.UIElementFullName, failureValue, uiElementId);
                }
            }
            else {
                if (rule.FailureValue !== null && rule.FailureValue !== undefined) {
                    setJSRenderProperty(rule, rule.UIElementFullName, rule.FailureValue, uiElementId);
                }
            }
        }
    }

    function setElementVisibility(result, rule) {
        var elem;
        if (rule.UIElementTypeID == 7) {
            if (GridDisplayMode.AG == CurrentGridDisplayMode) {
                elem = $('#repeater' + rule.UIElementFormName + currentInstance.formInstanceId);
            }
            else if (GridDisplayMode.PQ == CurrentGridDisplayMode) {
                elem = $('#' + rule.UIElementFormName + currentInstance.formInstanceId);
            }
        }
        else if (rule.UIElementTypeID == 9) {
            elem = $('#section' + rule.UIElementFormName + currentInstance.formInstanceId);
        }
        else {
            elem = $('#' + rule.UIElementFormName + currentInstance.formInstanceId);
        }
        if (result == true) {
            if (rule.UIElementTypeID == 7 || rule.UIElementTypeID == 9) {
                if ((rule.UIElementTypeID == 9 && rule.UIElementFormName == currentInstance.selectedSection && rule.UIElementFullName.indexOf('.') < 0)
                    || (rule.UIElementTypeID == 7)
                    || (rule.UIElementTypeID == 9 && rule.UIElementFullName.indexOf('.') > 0)) {
                    elem.css('display', '');
                }
                if (rule.UIElementTypeID == 7) {
                    var repToResizeColumn = currentInstance.repeaterBuilders.filter(function (elemName) {
                        return elemName.design.Name == rule.UIElementFormName;
                    });
                    if (repToResizeColumn != undefined && GridDisplayMode.AG == CurrentGridDisplayMode) {
                        if (repToResizeColumn.length > 0) {
                            var gridOptions = GridApi.getCurrentGridInstance(repToResizeColumn[0]).gridOptions;
                            gridOptions.api.sizeColumnsToFit();
                        }
                    }
                }
            }
            else {
                elem.parent('div').css('display', '');
                elem.parent('div').prev('div').css('display', '');
                //elem.parent('div').next('div').css('display', '');
            }
            if (rule.UIElementTypeID == 9 && rule.UIElementFullName.indexOf('.') < 0) {
                currentInstance.menu.showSection(rule.UIElementFormName);
            }
        }
        else {
            if (rule.UIElementTypeID == 7 || rule.UIElementTypeID == 9) {
                elem.css('display', 'none');
                try {
                    var childs = elem.find(':input');
                    $.each(childs, function (key, value) {
                        var fullName = $(this).attr('data-link');
                        setJSRenderProperty(rule, fullName, '', this.id);
                    });
                } catch (e) {

                }

            }
            else {
                var uiElementId = rule.UIElementFormName + currentInstance.formInstanceId;
                setJSRenderProperty(rule, rule.UIElementFullName, '', uiElementId);
                elem.parent('div').css('display', 'none');
                elem.parent('div').prev('div').css('display', 'none');
                //elem.parent('div').next('div').css('display', 'none');
            }
            if (rule.UIElementTypeID == 9 && rule.UIElementFullName.indexOf('.') < 0) {
                currentInstance.menu.hideSection(rule.UIElementFormName);
            }
        }
    }

    function setElementEnabledState(result, rule) {
        var elem = $('#' + rule.UIElementFormName + currentInstance.formInstanceId);
        if (result == true) {
            elem.prop('disabled', '');
            if (rule.UIElementTypeID == 1 || rule.UIElementTypeID == 6) {
                elem.siblings().prop('disabled', '');
            }
            else if (rule.UIElementTypeID == 9) {
                elem = $('#section' + rule.UIElementFormName + currentInstance.formInstanceId);
                elem.removeAttr('disabled');
                elem.find(':input').removeAttr('disabled');
            } else if (rule.UIElementTypeID == 13) {
                try {
                    elem.addClass("enableMo");
                    var rte = rule.UIElementFormName + currentInstance.formInstanceId;
                    tinyMCE.get(rte).setMode('code');
                } catch (e) { }
            }

        }
        else {
            elem.prop('disabled', 'disabled');
            if (rule.UIElementTypeID == 1 || rule.UIElementTypeID == 6) {
                elem.siblings().prop('disabled', 'disabled');
            }
            else if (rule.UIElementTypeID == 9) {
                elem = $('#section' + rule.UIElementFormName + currentInstance.formInstanceId);
                elem.attr('disabled', 'disabled');
                elem.find(':input').attr('disabled', 'disabled');
            } else if (rule.UIElementTypeID == 13) {
                var rte = rule.UIElementFormName + currentInstance.formInstanceId;
                try {
                    elem.removeClass("enableMo");
                    tinyMCE.get(rte).setMode('readonly');
                    enableTinyMceEditorPlugin(rte, 'preview', 'mcePreview');
                    enableTinyMceEditorPlugin(rte, 'fullscreen', 'mceFullScreen');
                } catch (e) { }
            }
        }
    }

    function enableTinyMceEditorPlugin(editorId, pluginName, commandName) {
        editorId = editorId.replace('#', '');
        var htmlEditorDiv = document.getElementById(editorId).previousSibling;
        if (htmlEditorDiv.nodeName == "DIV") {
            var editor = tinymce.get(editorId);
            var buttonDiv = htmlEditorDiv.querySelectorAll('.mce-i-' + pluginName.toLowerCase())[0].parentElement.parentElement;
            buttonDiv.className = buttonDiv.className.replace(' mce-disabled', '');
            buttonDiv.removeAttribute('aria-disabled');
            buttonDiv.firstChild.onclick = function () {
                editor.execCommand(commandName);
            };
        }
    }

    function setElementIsRequired(result, rule) {
        var elem = $('#' + rule.UIElementFormName + currentInstance.formInstanceId);
        if (currentInstance.designData.Validations == null) {
            currentInstance.designData.Validations = [];
        }
        var validation = currentInstance.designData.Validations.filter(function (val) {
            return val.UIElementName == rule.UIElementFormName;
        });
        if (result == true) {
            if (validation != null && validation.length > 0) {
                validation[0].IsRequired = true;
            }
            else {
                validation = { FullName: rule.UIElementFullName, UIElementName: rule.UIElementFormName, IsRequired: true, IsError: false, Regex: '', ValidationMessage: '', HasMaxLength: '', MaxLength: '', DataType: '', IsActive: true, ValidationType: 'Temporary' };
                currentInstance.designData.Validations.push(validation);
            }
        }
        else {
            if (validation != null && validation.length > 0) {
                //code commented to work IsRequired Rule properly
                /*
                if (validation[0].ValidationType == 'Temporary') {
                    var delIdx;
                    for (var idx = 0; idx < currentInstance.designData.Validations.length; idx++) {
                        var validation = currentInstance.designData.Validations[idx];
                        if (validation.UIElementName == rule.UIElementFormName) {
                            delIdx = idx;
                            break;
                        }
                    }
                    if (delIdx != null) {
                        //currentInstance.formValidationManager.removeValidation(uiElementName);
                        currentInstance.designData.Validations.splice(delIdx, 1);
                    }
                }
                else {
                    validation[0].IsRequired = false;
                }
                */
                validation[0].IsRequired = false;
            }
        }
        var val = currentInstance.ruleProcessor.getOperandValue(rule.UIElementFullName);
        var validationError = currentInstance.formValidationManager.handleValidation(rule.UIElementFullName, val);
        if (validationError) {
            currentInstance.validation.handleObjectChangeValidation(validationError);
            currentInstance.validation.showValidatedControlsOnSectionElementChange(validationError);
        }

    }

    function setElementErrorState(result, rule) {
        var elem = $('#' + rule.UIElementFormName + currentInstance.formInstanceId);
        if (currentInstance.designData.Validations == null) {
            currentInstance.designData.Validations = [];
        }
        var validation = currentInstance.designData.Validations.filter(function (val) {
            return val.UIElementName == rule.UIElementFormName;
        });
        if (result == true) {
            if (validation != null && validation.length > 0) {
                validation[0].IsError = false;
            }
        }
        else {
            if (validation != null && validation.length > 0) {
                validation[0].IsError = true;
            }
            else {
                validation = { FullName: rule.UIElementFullName, UIElementName: rule.UIElementFormName, IsRequired: false, IsError: true, Regex: '', ValidationMessage: '', HasMaxLength: '', MaxLength: '', DataType: '', IsActive: true, ValidationType: 'Temporary' };
                currentInstance.designData.Validations.push(validation);
            }
        }
        var val = currentInstance.ruleProcessor.getOperandValue(rule.UIElementFullName);
        var validationError = currentInstance.formValidationManager.handleValidation(rule.UIElementFullName, val, undefined, undefined, undefined, rule.Message);
        if (validationError) {
            currentInstance.validation.handleObjectChangeValidation(validationError);
            currentInstance.validation.showValidatedControlsOnSectionElementChange(validationError);
        }
    }

    function setElementValidation(result, rule) {
        if (currentInstance.designData.Validations != null && currentInstance.designData.Validations.length > 0) {
            var validation = currentInstance.designData.Validations.filter(function (val) {
                return val.UIElementName == rule.UIElementFormName;
            });
            if (result == true) {
                if (validation != null && validation.length > 0) {
                    validation[0].IsActive = true;
                }
            }
            else {
                if (validation != null && validation.length > 0) {
                    validation[0].IsActive = false;
                }
            }
            var val = currentInstance.ruleProcessor.getOperandValue(rule.UIElementFullName);
            var validationError = currentInstance.formValidationManager.handleValidation(rule.UIElementFullName, val, undefined, undefined, undefined, rule.Message);
            if (validationError) {
                currentInstance.validation.handleObjectChangeValidation(validationError);
                currentInstance.validation.showValidatedControlsOnSectionElementChange(validationError);
            }
        }
    }

    function setElementHighlightState(result, rule) {
        var elem = $('#' + rule.UIElementFormName + currentInstance.formInstanceId);
        if (result == true) {
            if (rule.UIElementTypeID == 1 || rule.UIElementTypeID == 2 || rule.UIElementTypeID == 13) {
                elem.parent('div').addClass('non-standard-optn');
            }
            else {
                elem.addClass('non-standard-optn');
            }
        }
        else {
            if (rule.UIElementTypeID == 1 || rule.UIElementTypeID == 2 || rule.UIElementTypeID == 13) {
                elem.parent('div').removeClass('non-standard-optn');
            }
            else {
                elem.removeClass('non-standard-optn');
            }
        }
    }

    function setCustomJSRenderProperty(elementFullName, value) {
        var obj = {};
        obj[elementFullName] = value;
        $.observable(currentInstance.formData).setProperty(obj);
    }

    function sumOfElementValues(values) {
        var sum = 0;
        $.each(values, function (index, item) {
            if (item != "" && item != "NA" && item != "Not Applicable") {
                if (!isNaN(item)) {
                    sum += parseInt(item);
                }
            }
        });
        return sum;
    }

    function percentageOf(values) {
        var percent = 0.00;
        try {
            if (values.length) {
                var value = parseInt(values[1]);
                var totalValue = parseInt(values[0]);
                percent = (value / totalValue) * 100;
                percent = percent.toFixed(2);
            }
        }
        catch (ex) {

        }
        return percent;
    }
    return {
        processValueRulesOnData: function (containerFullName) {
            if (currentInstance.isfolderReleased == false) {
                if (currentInstance.designData.Rules != null) {
                    var rules = currentInstance.designData.Rules.filter(function (rule) {
                        return rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Value &&
                            (rule.UIElementFullName.indexOf(containerFullName) == 0 || containerFullName == 'DOCUMENTROOT');
                    });
                    for (var idx = 0; idx < rules.length; idx++) {
                        var rule = rules[idx];
                        if (rule.IsParentRepeater == true) {
                            currentInstance.rules.processRepeaterRuleOnData(rule);
                        }
                        else {
                            currentInstance.rules.processRuleOnData(rule);
                        }
                    }
                }
            }
        },
        processRuleOnData: function (rule) {
            var result = currentInstance.ruleProcessor.processRule(rule);
            if (result == true) {
                if (rule.IsResultSuccessElement == true) {
                    var successValue = currentInstance.ruleProcessor.getOperandValue(rule.SuccessValueFullName);
                    currentInstance.ruleProcessor.setPropertyValue(rule.UIElementFullName, successValue);
                }
                else {
                    currentInstance.ruleProcessor.setPropertyValue(rule.UIElementFullName, rule.SuccessValue);
                }
            }
            else {
                if (rule.IsResultFailureElement == true) {
                    var successValue = currentInstance.ruleProcessor.getOperandValue(rule.FailureValueFullName);
                    currentInstance.ruleProcessor.setPropertyValue(rule.UIElementFullName, successValue);
                }
                else {
                    currentInstance.ruleProcessor.setPropertyValue(rule.UIElementFullName, rule.FailureValue);
                }
            }
        },
        processRepeaterRuleOnData: function (rule) {
            currentInstance.repeaterRuleProcessor.processRule(rule);
        },
        processRulesForElement: function (elementFullName, elementData, eventArgs, elementPath, executionLogParentRowId) {
            var elemRuleMap = getElementRules(elementFullName);
            if (elemRuleMap != null && elemRuleMap.Rules != null && elemRuleMap.Rules.length > 0) {
                for (var idx = 0; idx < elemRuleMap.Rules.length; idx++) {
                    var rule = getRule(elemRuleMap.Rules[idx]);
                    if (rule != undefined) {
                        currentInstance.rules.processRule(rule, elementData, eventArgs, elementPath, true, null, executionLogParentRowId);
                    }
                }
            }
        },
        processRulesForRootSections: function () {
            var rules = currentInstance.designData.Rules.filter(function (rule) {
                return rule.UIElementTypeID == 9 && rule.UIElementFullName.indexOf('.') < 0;
            });
            if (rules != null && rules.length > 0) {
                var executionLogParentRowId = -1;
                if (ActiveRuleExecutionLogger == "True") {
                    executionLogParentRowId = ruleExecutionLogger.logRulesExecutionOnLoad('DOCUMENT LOADED', 'OnLoad', 'On Load Client', false);
                }
                for (var idx = 0; idx < rules.length; idx++) {
                    currentInstance.rules.processRule(rules[idx], null, null, null, null, 'OnSection', executionLogParentRowId);
                }
            }

        },
        processRule: function (rule, elementData, eventArgs, elementPath, isRulesForElement, executeRuleOn, executionLogParentRowId) {
            if (rule.IsParentRepeater == true) {
                //determine repeaterbuilder
                var nameParts = rule.UIElementFullName.split('.');
                var parentName = '';
                for (var nameIdx = 0; nameIdx < nameParts.length - 1; nameIdx++) {
                    if (parentName == '') {
                        parentName = nameParts[nameIdx]
                    }
                    else {
                        parentName = parentName + '.' + nameParts[nameIdx];
                    }
                }
                var repeaterBuilder = currentInstance.repeaterBuilders.filter(function (builder) {
                    return parentName.indexOf(builder.fullName) == 0;
                });
                if (repeaterBuilder != null && repeaterBuilder.length > 0) {
                    //TODO: to be removed after unity testing of Rules
                    repeaterBuilder[0].runRuleForRepeater(rule);
                }
            }
            else {
                var result = currentInstance.ruleProcessor.processRule(rule);
                if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Enabled) {
                    setElementEnabledState(result, rule);
                }
                else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Value) {
                    setElementValue(result, rule);
                }
                else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Visible) {
                    setElementVisibility(result, rule);
                }
                else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.IsRequired) {
                    setElementIsRequired(result, rule);
                }
                else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.RunValidation) {
                    setElementValidation(result, rule);
                }
                else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Error) {
                    setElementErrorState(result, rule);
                }
                else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Highlight) {
                    setElementHighlightState(result, rule);
                }
                else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Dialog) {
                    setElementDialogState(result, rule);
                }
                else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.CustomRule) {
                    try {
                        //setElementCustomRuleState(result, rule);
                    }
                    catch (ex) {

                    }
                }

                if (ActiveRuleExecutionLogger == "True") {
                    if (isRulesForElement)
                        ruleExecutionLogger.logRulesExecution(elementData.ElementID, eventArgs.oldValue, eventArgs.value, rule.UIELementID, rule.RuleID, result, 'RuleForElement', false, executionLogParentRowId, 'On Change');

                    if (executeRuleOn != undefined && executeRuleOn != null)
                        ruleExecutionLogger.logRulesExecution(null, null, null, rule.UIELementID, rule.RuleID, result, executeRuleOn, false, executionLogParentRowId, 'On Load');
                }
            }
        },
        processRulesOnElements: function (containerFullName, ruleTargetType) {
            if (currentInstance.designData.Rules != null) {
                var rules = [];
                if (ruleTargetType == 'VALUE') {
                    rules = currentInstance.designData.Rules.filter(function (rule) {
                        return rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Value && rule.UIElementFullName.indexOf(containerFullName) == 0;
                    });
                }
                else if (ruleTargetType == 'NONVALUE') {
                    rules = currentInstance.designData.Rules.filter(function (rule) {
                        return (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Enabled || rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Visible || rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Highlight || rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.CustomRule) && rule.UIElementFullName.indexOf(containerFullName) == 0;
                    });
                }

                var sectionDetails = currentInstance.designData.Sections.filter(function (ct) {
                    return ct.FullName == containerFullName;
                });
                var sectionName = containerFullName;
                if (sectionDetails.length > 0) {
                    sectionName = sectionDetails[0].Label;
                }

                var executionLogParentRowId = -1;
                if (ActiveRuleExecutionLogger == "True") {
                    executionLogParentRowId = ruleExecutionLogger.logRulesExecutionOnLoad(sectionName, 'OnLoad', 'On Load Client', true);
                }

                for (var idx = 0; idx < rules.length; idx++) {
                    var rule = rules[idx];
                    if (currentInstance.isfolderReleased == false || rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Visible) {
                        if (rule.IsParentRepeater == true) {
                            var builders = currentInstance.repeaterBuilders.filter(function (builder) {
                                return rule.UIElementFullName.indexOf(builder.fullName) == 0;
                            });
                            if (builders != null && builders.length > 0) {
                                builders[0].ruleProcessor.processRule(rule, null, null, null, null, 'OnElement', executionLogParentRowId);
                            }
                        }
                        else {
                            currentInstance.rules.processRule(rule, null, null, null, null, 'OnElement', executionLogParentRowId);
                        }
                    }
                }
            }
        },
        getRulesForElement: function (elementFullName) {
            var ruleMaps = getElementRules(elementFullName);
            var rules = [];
            if (ruleMaps != null && ruleMaps.Rules != null) {
                for (var idx = 0; idx < ruleMaps.Rules.length; idx++) {
                    rules.push(getRule(ruleMaps.Rules[idx]));
                }
            }
            return rules;
        },
        getRulesInContainerElement: function (containerFulName) {
            var rules = currentInstance.designData.Rules.filter(function (rule) {
                return rule.UIElementFullName.indexOf(containerFulName) == 0;
            });
            return rules;
        },
        isLabelValue: function (fullname) {
            return isLabelValue(fullname);
        },
        setCustomJSRenderProperty: function (fullName, value) {
            setCustomJSRenderProperty(fullName, value);
        }
    }
}

formInstanceBuilder.prototype.cleanup = function () {
    $.observable(this.formData).unobserveAll(this.form.objectChangeHandler);
    this.designData = undefined;
    this.formData = undefined;
    this.gridDesigns = [];
    this.repeaterBuilders = [];
    this.sections = {};
    $(this.formInstanceDivJQ).empty();
    activitylogger.clear();
    if (ActiveRuleExecutionLogger == "True") {
        ruleExecutionLogger.clear();
    }
}

formInstanceBuilder.prototype.reload = function (isBackgroundProcess, ancillaryProductName) {
    var sectionDetails = this.form.getSectionData(this.selectedSection);
    var sectionName = sectionDetails.FullName;
    this.cleanup();
    this.loadFormInstanceDesignData(true, sectionName, isBackgroundProcess, undefined, ancillaryProductName);
    var folder = folderManager.getInstance().getFolderInstance();
    folder.loadAllFormsStatus(this.folderVersionId);
}

formInstanceBuilder.prototype.bottomMenuMethods = function () {
    var currentInstance = this;

    return {
        renderMenu: function () {
            $('#bottom-menu').show();
            currentInstance.errorManager.loadErrorGridData(currentInstance.errorGridData);
            $(currentInstance.elementIDs.bottomMenuTabsJQ).tabs();
            //$(".bottom-tab-content").hide();
            currentInstance.bottomMenu.registerEvents();
            $(currentInstance.elementIDs.bottomMenuTabsJQ + " .ui-jqgrid").show();
        },
        registerEvents: function () {
            $(currentInstance.elementIDs.bottomMenuJQ).css('height', '44px');

            $(currentInstance.elementIDs.bottomMenuTabsJQ + " .bottom-menu-button").unbind("click");
            $(currentInstance.elementIDs.bottomMenuTabsJQ + " .bottom-menu-button").bind("click", function () {
                if ($(this).hasClass("glyphicon-plus-sign")) {
                    currentInstance.bottomMenu.openBottomMenu();
                }
                else {
                    currentInstance.bottomMenu.closeBottomMenu();
                }
            });

            $(currentInstance.elementIDs.bottomMenuTabsJQ).tabs({
                activate: function (event, ui) {
                    if (ui.newTab[0].innerText == "Journal") {
                        currentInstance.journal.loadJournalEntry(false);
                    }
                    else if (ui.newTab[0].innerText == "Activity Log") {
                        activitylogger.loadActivityLogGrid();
                    }
                    else if (ui.newTab[0].innerText == "PBP View Actions") {
                        currentInstance.pbpViewAction.resizeGrid();
                    }
                    else if (ui.newTab[0].innerText == "Annotations") {
                        currentInstance.annotationMgr.buildAnnotationGrid();
                    }
                    else if (ui.newTab[0].innerText == "Rule Execution Log") {
                        if (ActiveRuleExecutionLogger == "True") {
                            ruleExecutionLogger.loadRuleExecutionLogGrid(currentInstance);
                        }
                    }
                    $(currentInstance.errorManager.errorGridElementID.errorGridJQ).jqGrid("setGridWidth", $(currentInstance.errorManager.errorGridElementID.errorGridJQ).parent().width(), true);
                }
            });
        },
        openBottomMenu: function () {
            $(currentInstance.elementIDs.bottomMenuJQ).css('height', '330px');

            $(".bottom-menu-button").removeClass("glyphicon-plus-sign");
            $(".bottom-menu-button").addClass("glyphicon-minus-sign");

            if (ActiveRuleExecutionLogger == "True") {
                for (var i = 0; i < $(currentInstance.elementIDs.bottomMenuTabsJQ + " li").length; i++) {
                    if ($(currentInstance.elementIDs.bottomMenuTabsJQ + " li")[i].innerText == 'Rule Execution Log') {
                        var className = $(currentInstance.elementIDs.bottomMenuTabsJQ + " li")[i].className;
                        if (className == "ui-state-default ui-corner-top ui-tabs-active ui-state-active") {
                            ruleExecutionLogger.loadRuleExecutionLogGrid(currentInstance);
                        }
                    }
                }
            }
        },
        closeBottomMenu: function () {
            if ($(currentInstance.elementIDs.bottomMenuJQ).is(":visible")) {
                $(currentInstance.elementIDs.bottomMenuJQ).css('height', '46px');

                $(".bottom-menu-button").removeClass("glyphicon-minus-sign");
                $(".bottom-menu-button").addClass("glyphicon-plus-sign");
            }
        },
        showErrorGridTab: function () {
            currentInstance.bottomMenu.openBottomMenu();
            $(currentInstance.elementIDs.bottomMenuTabsJQ).tabs("option", "active", 0);
        },
        showJournalGridTab: function (isPreviousVersion) {
            currentInstance.journal.loadJournalEntry(isPreviousVersion);
            currentInstance.bottomMenu.openBottomMenu();
            $(currentInstance.elementIDs.bottomMenuTabsJQ).tabs("option", "active", 2);
        },
        showPBPViewActionTab: function () {
            //currentInstance.bottomMenu.openBottomMenu();
            $(currentInstance.elementIDs.bottomMenuTabsJQ).tabs("option", "active", 3);
        },
        showExitValidateTab: function () {
            currentInstance.bottomMenu.openBottomMenu();
            $(currentInstance.elementIDs.bottomMenuTabsJQ).tabs("option", "active", 5);
            exitValidate.loadExitValidateResults(currentInstance.formInstanceId);
        },
        checkTabSelection: function (oldFormInstanceID) {
            currentInstance.bottomMenu.checkSectionSelection(currentInstance.selectedSection, oldFormInstanceID);

            //VBID Set anchorFormInstanceID
            if (GLOBAL.applicationName.toLowerCase() == 'emedicaresync') {
                currentInstance.vbidext = new vbidextension(currentInstance.anchorFormInstanceID, currentInstance.folderVersionId, currentInstance.folderId);
                currentInstance.vbidext.setVBIDMenu();
            }

            var className = $(currentInstance.elementIDs.bottomMenuTabsJQ + " li")[2].className;
            if (className == "ui-state-default ui-corner-top ui-tabs-active ui-state-active") {
                currentInstance.journal.loadJournalEntry(false);
            }
        },
        checkSectionSelection: function (sectionName, oldFormInstanceID) {
            journalInstance.getInstance().getJournalInstance().highlightButton(false, $('#btnJournalEntry'));
            if (sectionName != undefined) {
                journalInstance.getInstance().getJournalInstance().disableJournalAddMode(sectionName, currentInstance.formInstanceId, oldFormInstanceID);
            }
            else {
                journalInstance.getInstance().getJournalInstance().disableJournalAddMode("", currentInstance.formInstanceId, oldFormInstanceID);
            }
        },
        showJournalErrorGrid: function () {
            if (currentInstance.errorGridData.length > 0) {
                $(currentInstance.elementIDs.bottomMenuTabsJQ).tabs("option", "active", 0);
            }
            else {
                if (currentInstance.hasOpenJournals) {
                    currentInstance.bottomMenu.showJournalGridTab(false);
                }
            }
        }
    }
}

formInstanceBuilder.prototype.manualOverride = function () {
    var currentInstance = this;
    var _elementData;
    var _path;
    var _value;

    function turnOn() {
        messageDialog.show("Once manual override is turned ON, the field will not pull standard ML language or anchor values.");
    }

    function turnOff() {
        yesNoConfirmDialog.show('Are you sure! All your manual changes will be lost.', processTurnOffResult);
    }

    function processTurnOffResult(result) {
        var tgtElementId = getTargetField();
        if (tgtElementId.UIELementID) {
            if (result) {
                yesNoConfirmDialog.hide();
                getOriginalValue(tgtElementId);
            } else {
                _elementData.ignoreObjectHandler = true;
                $.observable(currentInstance.formData).setProperty(_elementData.FullName, true);
            }
        }
        else {
            yesNoConfirmDialog.hide();
        }
    }

    function getTargetField() {
        var targetElement = {};
        var elemRuleMap = {};
        var designData = currentInstance.designData;
        var elemRuleMaps = designData.ElementRuleMaps.filter(function (elem) {
            return elem.FullName == _path;
        });
        if (elemRuleMaps != null && elemRuleMaps.length > 0) {
            elemRuleMap = elemRuleMaps[0];
        }

        if (elemRuleMap != null && elemRuleMap.Rules != null && elemRuleMap.Rules.length > 0) {
            for (var idx = 0; idx < elemRuleMap.Rules.length; idx++) {
                var rules = designData.Rules.filter(function (rule) {
                    return rule.RuleID == elemRuleMap.Rules[idx];
                });
                if (rules != null && rules.length > 0) {
                    var rule = rules[0];
                    if (rule.TargetPropertyTypeId == 1 && rule.UIElementTypeID == 13) {
                        targetElement.UIELementID = rule.UIELementID;
                        targetElement.UIElementName = rule.UIElementFullName;
                        targetElement.UIElementFormName = rule.UIElementFormName;
                        break;
                    }
                }
            }
        }

        return targetElement;
    }

    function getOriginalValue(tgtElementId) {
        var url = "/ExpressionBuilder/ProcessRuleForManualOverride";
        var promise = ajaxWrapper.postJSON(url, {
            folderVersionId: currentInstance.folderVersionId,
            formInstanceId: currentInstance.formInstanceId,
            formDesignVersionId: currentInstance.formDesignVersionId,
            targetElementId: tgtElementId.UIELementID,
            targetElementPath: tgtElementId.UIElementName,
            sourceElementPath: _path
        });

        promise.done(function (result) {
            result = JSON.parse(result);
            if (result) {
                var ele = tgtElementId.UIElementFormName + currentInstance.formInstanceId;
                var obj = {};
                obj[tgtElementId.UIElementName] = result;
                $.observable(currentInstance.formData).setProperty(obj);
                tinymce.get(ele).setContent(result);
            }
        });
    }

    function execute(element, value, path) {
        _elementData = element;
        _value = value;
        _path = path;
        if (value) {
            turnOn();
        } else {
            turnOff();
        }
    }

    return {
        process: function (elementData, value, path) {
            execute(elementData, value, path);
        }
    }
}

formInstanceBuilder.prototype.setCheckBoxDataLink = function (fullName) {
    return 'data-link="{getCheck:' + fullName + ':setCheck}"';
}

formInstanceBuilder.prototype.setRadioButtonDataLink = function (fullName) {
    return 'data-link="{getRadioValue:' + fullName + ':setRadioValue}"';
}
formInstanceBuilder.prototype.checkForUserRoleAccessPermissions = function (formInstance, currentInstance) {
    var isAccessible = false;
    $.each(formInstanceClaims, function (i, claim) {
        if (claim.ResourceID == formInstance.FormDesignID && claim.RoleID == currentInstance.folderData.RoleId) {
            isAccessible = true;
            return false;
        }
    });

    return isAccessible;
}
//section access permission for user role
formInstanceBuilder.prototype.accessPermissionsMethods = function () {
    var currentInstance = this;
    var currentFormUtilities = new formUtilities(currentInstance.formInstanceId);
    function checkViewPermission(Permissions, roleId) {
        var rtnVal = false;
        $.each(Permissions, function (index, item) {
            if (item.RoleID == roleId && item.IsVisible === true) rtnVal = true;
        });
        return rtnVal;
    }

    function checkEditPermission(Permissions, roleId) {
        var rtnVal = true;
        $.each(Permissions, function (index, item) {
            if (item.RoleID == roleId && item.IsEditable === true) rtnVal = true;
        });
        //var WfStatePermission = false;
        //$.each(stateAccessRoles, function (index, item) { if (item.RoleID == roleId) WfStatePermission = true; });
        //Fixed for ANT-13, If TPA Analyst user is logged in then he should edit the document but he cannot update the status
        //The condition is changed for role id TPA analyst 
        //if (rtnVal != true || (WfStatePermission != true && roleId != Role.TPAAnalyst) || folderData.WFStateName == AntWorkFlowState.BPDDistribution) {
        //    rtnVal = false;
        //}
        //Check if Section is locked for editing
        if (currentInstance.isSectionEditable == false) {
            rtnVal = false;
        }
        return rtnVal;
    }

    //member functions for View Permissions
    function SetViewPermissionToSection(isVisible, section) {
        if (isVisible == false) {
            currentFormUtilities.sectionManipulation.SetSectionHidden(section);
        }
        else {
            if (section.Elements != null) {
                $.each(section.Elements, function (index, element) {
                    SetViewForElements(element);
                });
            }
        }
    }

    function SetViewForElements(Element) {
        if (Element.Section != null) {
            var section = Element.Section;
            var isSectionVisible = checkViewPermission(section.AccessPermissions, role);
            SetViewPermissionToSection(isSectionVisible, section);
        }
    }

    function disableManualDataSourceButton(currentInstance) {
        //if (vbRole == Role.Audit || vbRole == Role.Product || vbRole == Role.ProductAudit || vbRole == Role.WFSpecialist) {
        //    for (var i = 0; i < currentInstance.repeaterBuilders.length ; i++) {
        //        if (currentInstance.repeaterBuilders[i].fullName == currentInstance.elementIDs.serviceGroupingRepeaterPath) {
        //            var repeaterId = '#repeater' + currentInstance.repeaterBuilders[i].gridElementId;
        //            $(repeaterId).find(currentInstance.elementIDs.btnManualDataPopup).addClass('ui-state-disabled');
        //        }
        //    }
        //}
    }

    //member functions for Edit Permissions
    function SetEditPermissionToSection(isEnable, section) {
        var currentUrl = window.location.href;
        var mode = currentUrl.substring(currentUrl.lastIndexOf('=') + 1, window.location.href.length);

        if (isEnable == false) {
            currentFormUtilities.sectionManipulation.SetSectionDisable(section);

        }
        else if (mode == "false") {
            currentFormUtilities.sectionManipulation.SetSectionDisable(section);
        }
        else if (section.Elements != null) {
            $.each(section.Elements, function (index, element) {
                SetEditForElements(element);
            });
        }
    }

    function disableMenuLevelButtons() {
        //$(currentInstance.elementIDs.btnCreateFormJQ).addClass('disabled-button');
        //$(currentInstance.elementIDs.btnSOTAddJQ).addClass('disabled-button');
        $(currentInstance.elementIDs.btnSaveFormDataJQ).addClass('disabled-button');
        $(currentInstance.elementIDs.btnBottomSaveFormDataJQ).addClass('disabled-button');
        $(currentInstance.elementIDs.btnBottomCreateNewVersion).addClass('disabled-button');
        //$(currentInstance.elementIDs.btnBottomGenerateSOTFile).addClass('disabled-button');
        //$(currentInstance.elementIDs.btnSOTSaveJQ).addClass('disabled-button');
        $(currentInstance.elementIDs.btnValidateJQ).addClass('disabled-button');
        $(currentInstance.elementIDs.btnValidateSectionJQ).addClass('disabled-button');
        $(currentInstance.elementIDs.btnValidateAllJQ).addClass('document-disabled-button');
        $(currentInstance.elementIDs.btnStatusUpdateJQ).addClass('disabled-button');
        $(currentInstance.elementIDs.btnBaselineJQ).addClass('disabled-button');
        $(currentInstance.elementIDs.btnNewVersionJQ).attr("disabled", "disabled");
        //$(currentInstance.elementIDs.btnNewVersionMLJQ).attr("disabled", "disabled");
        $(currentInstance.elementIDs.btnRetroJQ).attr("disabled", "disabled");
        //$(currentInstance.elementIDs.btnRetroMLJQ).attr("disabled", "disabled");
        $(currentInstance.elementIDs.btnJournalEntryJQ).addClass('disabled-button');
        $(currentInstance.elementIDs.btnDeleteFormInstance).addClass('disabled-button');
        $(currentInstance.elementIDs.btnAssignFolderMember).addClass('disabled-button');
    }

    function disableDocumentLevelButtons() {
        $(currentInstance.elementIDs.btnDeleteFormInstance).addClass('document-disabled-button');
        $(currentInstance.elementIDs.btnValidateJQ).addClass('document-disabled-button');
        $(currentInstance.elementIDs.btnValidateSectionJQ).addClass('document-disabled-button');
        $(currentInstance.elementIDs.btnJournalEntryJQ).addClass('document-disabled-button');
        $(currentInstance.elementIDs.btnSaveFormDataJQ).addClass('document-disabled-button');
        $(currentInstance.elementIDs.btnBaselineMLJQ).addClass('document-disabled-button');
        $(currentInstance.elementIDs.btnBottomSaveFormDataJQ).addClass('document-disabled-button');
    }
    function disableDocumentLevelMenuML(isEditable) {
        var currentUrl = window.location.href;
        var mode = currentUrl.substring(currentUrl.lastIndexOf('=') + 1, window.location.href.length);
        $(currentInstance.elementIDs.btnDeleteFormInstance).parent('div').hide();
        $(currentInstance.elementIDs.btnView1ReportJQ).parent('div').hide();
        $(currentInstance.elementIDs.btnView2ReportJQ).parent('div').hide();
        $(currentInstance.elementIDs.assignFolderMemberbuttonJQ).parent('div').hide();
        $(currentInstance.elementIDs.facetsTranslateJQ).parent('div').hide();
        //$(currentInstance.elementIDs.generateProductShareReportButtonJQ).parent('div').hide();
        if (!isEditable || mode == "false" || ((vbRole == Role.ProductDesignerLevel2 || vbRole == Role.ProductSME) && GLOBAL.clientName != 'HMHS') || (vbRole == Role.ProductCoreAdminDesigner && GLOBAL.clientName == 'HMHS') || ((vbRole == Role.ReViewer || vbRole == Role.ProductCoreAdminDesigner) && GLOBAL.clientName == 'HMHS')) {
            $(currentInstance.elementIDs.btnSaveFormDataJQ).addClass('document-disabled-button');
            $(currentInstance.elementIDs.btnCreateFormJQ).addClass('document-disabled-button');
            $(currentInstance.elementIDs.btnValidateJQ).addClass('document-disabled-button');
            $(currentInstance.elementIDs.btnValidateSectionJQ).addClass('document-disabled-button');
            $(currentInstance.elementIDs.btnJournalEntryJQ).addClass('document-disabled-button');
            $(currentInstance.elementIDs.btnValidateAllJQ).addClass('document-disabled-button');
            $(currentInstance.elementIDs.btnBaselineMLJQ).addClass('document-disabled-button');
            $(currentInstance.elementIDs.btnBottomSaveFormDataJQ).addClass('document-disabled-button');
        }
        else if (currentInstance.IsFormInstanceEditable = true) {
            $(currentInstance.elementIDs.btnSaveFormDataJQ).removeClass('disabled-button');
            $(currentInstance.elementIDs.btnBottomSaveFormDataJQ).removeClass('disabled-button');
            $(currentInstance.elementIDs.btnCreateFormJQ).removeClass('disabled-button');
        }
    }

    function enableDocumentLevelButtons() {

        var currentUrl = window.location.href;
        var mode = currentUrl.substring(currentUrl.lastIndexOf('=') + 1, window.location.href.length);

        $(currentInstance.elementIDs.btnDeleteFormInstance).removeClass('document-disabled-button');
        $(currentInstance.elementIDs.btnValidateJQ).removeClass('document-disabled-button');
        $(currentInstance.elementIDs.btnValidateSectionJQ).removeClass('document-disabled-button');
        $(currentInstance.elementIDs.btnJournalEntryJQ).removeClass('document-disabled-button');

        if (vbRole == Role.ProductSME && currentInstance.formInstanceId != currentInstance.anchorFormInstanceID && (vbRole != Role.ProductCoreAdminDesigner && GLOBAL.clientName != 'HMHS')) {
            $(currentInstance.elementIDs.btnSaveFormDataJQ).removeClass('disabled-button');
            $(currentInstance.elementIDs.btnSaveFormDataJQ).removeClass('document-disabled-button');
        }
        if ((!currentInstance.IsMasterList || (currentInstance.IsMasterList && currentInstance.designData.UsesAliasDesignMasterList == true)) && vbRole != Role.Viewer) {
            $(currentInstance.elementIDs.btnCreateFormJQ).removeClass('disabled-button');
            $(currentInstance.elementIDs.btnCreateFormJQ).removeAttr("disabled");
            $(currentInstance.elementIDs.btnSaveFormDataJQ).removeClass('disabled-button');
            $(currentInstance.elementIDs.btnSaveFormDataJQ).removeClass('document-disabled-button');
            $(currentInstance.elementIDs.btnBottomSaveFormDataJQ).removeClass('disabled-button');
            $(currentInstance.elementIDs.btnBottomSaveFormDataJQ).removeClass('document-disabled-button');
        }
        else {
            $(currentInstance.elementIDs.btnCreateFormJQ).addClass('disabled-button');
            $(currentInstance.elementIDs.btnCreateFormJQ).attr("disabled", "disabled");
        }

        if ((!currentInstance.IsMasterList || (currentInstance.IsMasterList && currentInstance.designData.UsesAliasDesignMasterList == true)) && (vbRole == Role.Viewer || (vbRole == Role.ReViewer && GLOBAL.clientName == 'HMHS') || (vbRole == Role.ProductCoreAdminDesigner && GLOBAL.clientName == 'HMHS' && currentInstance.accountId <= 0))) {
            $(currentInstance.elementIDs.btnCreateFormJQ).addClass('disabled-button');
            $(currentInstance.elementIDs.btnCreateFormJQ).attr("disabled", "disabled");
        }
        if (vbRole != Role.Viewer && vbRole != Role.ProductSME && currentInstance.isSectionEditable && (vbRole != Role.ProductCoreAdminDesigner && GLOBAL.clientName != 'HMHS')) {
            $(currentInstance.elementIDs.btnSaveFormDataJQ).removeClass('disabled-button');
            $(currentInstance.elementIDs.btnSaveFormDataJQ).removeClass('document-disabled-button');
            $(currentInstance.elementIDs.btnBottomSaveFormDataJQ).removeClass('disabled-button');
            $(currentInstance.elementIDs.btnBottomSaveFormDataJQ).removeClass('document-disabled-button');
        }
    }

    function enableMenuLevelButtons() {
        //$(currentInstance.elementIDs.btnCreateFormJQ).removeClass('disabled-button');
        //$(currentInstance.elementIDs.btnSOTremoveJQ).removeClass('disabled-button');
        $(currentInstance.elementIDs.btnSaveFormDataJQ).removeClass('disabled-button');
        $(currentInstance.elementIDs.btnBottomSaveFormDataJQ).removeClass('disabled-button');
        $(currentInstance.elementIDs.btnBottomCreateNewVersion).removeClass('disabled-button');
        //$(currentInstance.elementIDs.btnBottomGenerateSOTFile).removeClass('disabled-button');
        //$(currentInstance.elementIDs.btnSOTSaveJQ).removeClass('disabled-button');
        $(currentInstance.elementIDs.btnValidateJQ).removeClass('disabled-button');
        $(currentInstance.elementIDs.btnValidateSectionJQ).removeClass('disabled-button');
        $(currentInstance.elementIDs.btnValidateAllJQ).removeClass('document-disabled-button');
        $(currentInstance.elementIDs.btnStatusUpdateJQ).removeClass('disabled-button');
        $(currentInstance.elementIDs.btnBaselineJQ).removeClass('disabled-button');
        $(currentInstance.elementIDs.btnNewVersionJQ).removeAttr("disabled", "disabled");
        //$(currentInstance.elementIDs.btnNewVersionMLJQ).removeAttr("disabled", "disabled");
        $(currentInstance.elementIDs.btnRetroJQ).removeAttr("disabled", "disabled");
        //$(currentInstance.elementIDs.btnRetroMLJQ).removeAttr("disabled", "disabled");
        $(currentInstance.elementIDs.btnJournalEntryJQ).removeClass('disabled-button');
        $(currentInstance.elementIDs.btnDeleteFormInstance).removeClass('disabled-button');
        $(currentInstance.elementIDs.btnAssignFolderMember).removeClass('disabled-button');
    }

    function SetEditForElements(Element) {
        if (Element.Section != null) {
            var section = Element.Section;
            var isSectionEnable = checkEditPermission(section.AccessPermissions, role);
            SetEditPermissionToSection(isSectionEnable, section);
        }
    }

    return {
        applyAccessPermission: function (section) {
            if (section.AccessPermissions != null) {
                var isVisible = checkViewPermission(section.AccessPermissions, role);
                SetViewPermissionToSection(isVisible, section);
                var isEnable = checkEditPermission(section.AccessPermissions, role);
                SetEditPermissionToSection(isEnable, section);
                //add editable condition for custom rule here
            }
            else {
                //add editable condition for custom rule here
            }
        },
        handleReleasedState: function (isReleased, section) {
            SetEditPermissionToSection(!isReleased, section);;
        },
        handleFolderLockState: function (isLocked, section) {
            SetEditPermissionToSection(!isLocked, section);
        },
        SetViewPermissionToSection: function (isVisible, Section) {
            SetViewPermissionToSection(isVisible, Section);
        },
        handleDocumentTraslationState: function (isDocumentLocked, section) {
            SetEditPermissionToSection(!isDocumentLocked, section);
        },
        disableDocumentLevelButtons: function () {
            disableDocumentLevelButtons();
        },
        enableDocumentLevelButtons: function () {
            enableDocumentLevelButtons();
        },
        disableMenuLevelButtons: function () {
            disableMenuLevelButtons();
        },
        enableMenuLevelButtons: function () {
            enableMenuLevelButtons();
        },
        disableDocumentLevelMenuML: function (isEditable) {
            disableDocumentLevelMenuML(isEditable);
        },
        checkEditPermission: function (section) {
            var currentUrl = window.location.href;
            var mode = currentUrl.substring(currentUrl.lastIndexOf('=') + 1, window.location.href.length);
            if (mode == 'false') {
                return false;
            }
            return checkEditPermission(section.AccessPermissions, role);
        }
    }
}

formInstanceBuilder.prototype.dropDownTextBox = function () {
    function highlightDropdownTextBox(dropDownTextBoxElement) {
        dropDownTextBoxElement.removeClass('standard-optn non-standard-optn');
        dropDownTextBoxElement.addClass(dropDownTextBoxElement.find(":selected").attr('class'));
        // For highlighting autocomplete textbox
        var id = dropDownTextBoxElement[0].id;
        var combobox = $('#' + id + 'DDL');
        if (combobox !== undefined && combobox !== null && combobox.length > 0) {
            $(combobox).removeClass('standard-optn non-standard-optn');
            $(combobox).addClass(dropDownTextBoxElement.find(":selected").attr('class'));
        }
    }
    return {
        updateAndToggleDropDownTextBox: function () {

            $('.ddt-dropdown').unbind('change');
            $('.ddt-dropdown').change(function () {
                if ($(this).val() == 'newItem') {
                    $(this).val('');
                    $(this).parent().find('.ddt-textbox').toggle().focus();
                    //$(this).toggle();
                    var id = $(this)[0].id;
                    var combobox = $('#' + id + 'DDL');
                    if (combobox !== undefined && combobox !== null && combobox.length > 0) {
                        $(this).parent().find('.ddt-textbox').css({ 'position': 'relative', 'top': '-20px' });
                        $(combobox).parent().toggle();
                    }
                    else {
                        $(this).toggle();
                    }
                }
                highlightDropdownTextBox($(this));
            });

            $('.ddt-textbox').unbind('focusout');
            $('.ddt-textbox').on('focusout', function () {
                var newValue = $(this).val();
                var isItemExists = false;
                var stringUtility = new globalutilities();
                if (!stringUtility.stringMethods.isNullOrEmpty(newValue)) {
                    if ($(this).parent().find('.ddt-dropdown option').hasClass("non-standard-optn") == true) {
                        $(this).parent().find('.ddt-dropdown option.non-standard-optn').remove();
                    }
                    $(this).parent().find('.ddt-dropdown option').each(function () {
                        if (this.value.toUpperCase() === newValue.toUpperCase()) {
                            isItemExists = true;
                        }
                    });
                    if (isItemExists == false) {
                        $('<option value="' + newValue + '" class="non-standard-optn">' + newValue + '</option>').insertBefore($(this).parent().find('.ddt-dropdown option').last());
                        $(this).parent().find('.ddt-dropdown')[0].selectedIndex = $(this).parent().find('.ddt-dropdown option.non-standard-optn').index();
                    }
                }
                else {
                    $(this).siblings('.ddt-dropdown').val('[Select One]').trigger('change');
                }
                $(this).toggle();
                var id = $(this).parent().find('.ddt-dropdown')[0].id;
                var autoCompletSpan = $('#' + id + 'DDL');
                if (autoCompletSpan !== undefined && autoCompletSpan !== null && autoCompletSpan.length > 0) {
                    $('#' + id + 'DDL').parent().toggle();
                    $('#' + id).filterDropDown("refresh");
                }
                else {
                    $(this).parent().find('.ddt-dropdown').toggle();
                }
                highlightDropdownTextBox($(this).parent().find('.ddt-dropdown'));
            });
            $('.ddt-dropdown').each(function () {
                highlightDropdownTextBox($(this));
            });
        }
    }
}

formInstanceBuilder.prototype.journalMethods = function () {
    var currentInstance = this;
    return {
        InitializeOpenJournals: function () {
            try {

                var myWorker = new Worker('/Scripts/FolderVersion/journalNotify.js');

                var url = '/JournalReport/CheckAllJournalEntryIsClosed';

                var saveData = JSON.stringify({
                    folderVersionId: currentInstance.folderVersionId,
                    formInstanceId: currentInstance.formInstanceId
                });

                if (typeof (Worker) !== "undefined") {

                    myWorker.onmessage = function (e) {
                        if (e.data != undefined) {
                            if (e.data == "true") {
                                currentInstance.hasOpenJournals = true;
                            }
                            else if (e.data == "false") {
                                currentInstance.hasOpenJournals = false;
                            }
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
        },
        loadJournalEntry: function (isPreviousVersion) {
            try {

                var myWorker = new Worker('/Scripts/FolderVersion/journalNotify.js');

                if (isPreviousVersion) {
                    var url = '/JournalReport/GetAllJournalsList';

                    var saveData = JSON.stringify({
                        formInstanceId: currentInstance.formInstanceId,
                        folderVersionId: currentInstance.folderVersionId,
                        folderId: currentInstance.folderId,
                        formDesignVersionId: currentInstance.formDesignVersionId,
                        tenantId: currentInstance.tenantId
                    });
                }
                else {
                    var url = '/JournalReport/GetCurrentVersionJournalsList';

                    var saveData = JSON.stringify({
                        formInstanceId: currentInstance.formInstanceId,
                        folderVersionId: currentInstance.folderVersionId,
                        formDesignVersionId: currentInstance.formDesignVersionId,
                        tenantId: currentInstance.tenantId
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
                            journalManager.buildJournalGrid(currentInstance.formInstanceId, isPreviousVersion);
                            journalManager.loadJournalGridData(journalGridData);
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
        },
        accessJournalEntry: function (event) {
            var sectionDesignData = currentInstance.form.getSectionData(currentInstance.selectedSection);

            try {

                var myWorker = new Worker('/Scripts/FolderVersion/journalNotify.js');

                var saveData = JSON.stringify({
                    formInstanceId: currentInstance.formInstanceId,
                    folderVersionId: currentInstance.folderVersionId,
                    formDesignVersionId: currentInstance.formDesignVersionId,
                    tenantId: currentInstance.tenantId
                });

                if (typeof (Worker) !== "undefined") {

                    myWorker.onmessage = function (e) {

                        if (e.data != undefined) {
                            var journalGridData = new Array();
                            var list = JSON.parse(e.data);
                            for (var i = 0; i < list.length; i++) {
                                var listObject = list[i];
                                journalGridData.push(listObject);
                            }
                            var singleTonjournalInstance = new journalInstance.getInstance(sectionDesignData, currentInstance.formInstanceId, currentInstance.folderVersionId, currentInstance.tenantId, journalGridData, event);
                            singleTonjournalInstance.load();
                        }
                    };

                    myWorker.onerror = function (err) {
                        console.log('Worker is suffering!', err);
                    }

                    myWorker.postMessage({
                        url: '/JournalReport/GetCurrentVersionJournalsList',
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

        },
    }
}

formInstanceBuilder.prototype.getSectionDropdownElement = function (currentInstance, dataSource) {
    var fullNameArray = dataSource.TargetParent.split('.');

    var targetsection = undefined;

    var dropdwonelement = undefined;

    //gett parent section of dropdown
    targetsection = currentInstance.formInstanceBuilder.designData.Sections.filter(function (ts) {
        return ts.GeneratedName == fullNameArray[0];
    });

    if (fullNameArray[1] != undefined) {
        targetsection = targetsection[0].Elements.filter(function (sd) {
            return sd.GeneratedName == fullNameArray[1];
        });


        for (var l = 2; l < fullNameArray.length; l++) {
            targetsection = targetsection[0].Section.Elements.filter(function (sd) {
                return sd.GeneratedName == fullNameArray[l];
            });
        }

        //get dropdown element
        if (targetsection[0].Type != "repeater") {
            dropdwonelement = targetsection[0].Section.Elements.filter(function (dl) {
                if ((dl.Type == 'select' || dl.Type == "SelectInput") && dl.GeneratedName == [dataSource.Mappings[0].TargetElement]) {
                    return dl;
                }
            });
        }
    }
    else {
        //get dropdown element
        if (targetsection[0].Type != "repeater") {
            dropdwonelement = targetsection[0].Elements.filter(function (dl) {
                if ((dl.Type == 'select' || dl.Type == "SelectInput") && dl.GeneratedName == [dataSource.Mappings[0].TargetElement]) {
                    return dl;
                }
            });
        }
    }

    return dropdwonelement;
}

formInstanceBuilder.prototype.updateTargetSectionDropdown = function (currentInstance, rowIndex, dataSource, status, dropdwonelement, sourcesynchroniser) {
    var oldItem = [], newItem = [], options = [], enterUniqueResponse = undefined;

    //get item list from source repeater also update design element in datasourcesync
    var items = sourcesynchroniser.updateSectionDropDown(dropdwonelement[0], status);

    items = items.filter(function (itm) {
        return itm !== ''
    })

    items.splice(0, 0, "[Select One]");


    var previousItems = [];

    var dropdownId = '#' + dropdwonelement[0].Name + currentInstance.formInstanceId;

    var dropdownItems = $(dropdownId + " option");

    if (dropdownItems.length != 0) {
        for (var i = 0; i < dropdownItems.length; i++) {
            previousItems[i] = dropdownItems[i].text;
        }
        if (dropdwonelement[0].Type == "SelectInput") {
            $.each(dropdownItems, function (idx, dt) {
                if (dt.className == "non-standard-optn") {
                    enterUniqueResponse = items.filter(function (ct) {
                        return ct == previousItems[idx];
                    });

                    if (enterUniqueResponse != undefined) {
                        if (enterUniqueResponse.length != 0) {
                            var isenterUniqueResponseExistInSourceRepeater = items.filter(function (itm) {
                                return itm == enterUniqueResponse;
                            });

                            if (isenterUniqueResponseExistInSourceRepeater == undefined && isenterUniqueResponseExistInSourceRepeater.length == 0) {
                                items.splice(items.length, items.length, previousItems[idx]);
                                return false;
                            }
                        }
                        else {
                            items.splice(items.length, items.length, previousItems[idx]);
                            return false;
                        }
                    }
                }
            });
            items.splice(items.length, items.length, "Enter Unique Response");
        }

        $.grep(items, function (el) {
            if ($.inArray(el, previousItems) == -1)
                newItem.push(el);
        });

        $.grep(previousItems, function (el) {
            if ($.inArray(el, items) == -1)
                oldItem.push(el);
        });

        //var newItem = currentInstance.data[rowIndex][dataSource.Mappings[0].SourceElement];        
        for (var i = 0; i < items.length; i++) {

            if (dropdwonelement[0].Type == "SelectInput") {

                if (items[i] != "Enter Unique Response") {

                    var isItemEnterUniqueResponse = undefined;

                    $.each(dropdownItems, function (idx, dt) {
                        if (dt.className == "non-standard-optn") {
                            if (dt.innerText == items[i]) {
                                isItemEnterUniqueResponse = items[i];
                                return false;
                            }
                        }
                    });

                    if (isItemEnterUniqueResponse != undefined) {
                        newitems = sourcesynchroniser.getSourceDropdownItems();

                        var isExistInSource = undefined;

                        for (var k = 0; k < newitems.length; k++) {
                            if (newitems[k] == items[i]) {
                                isExistInSource = items[i];
                                break;
                            }
                        }

                        if (isExistInSource == undefined || isExistInSource.length == 0) {
                            options[i] = $('<option value="' + items[i] + '" class="non-standard-optn">' + items[i] + '</option>');
                        }
                        else
                            options[i] = $('<option></option>').attr("value", items[i]).text(items[i]);
                    }
                    else
                        options[i] = $('<option></option>').attr("value", items[i]).text(items[i]);
                }
                else {
                    options[i] = $('<option></option>').attr("value", "newItem").text("Enter Unique Response");
                }
            }
            else {
                options[i] = $('<option></option>').attr("value", items[i]).text(items[i]);
            }
        }

        var selectedval = $(dropdownId + " option:selected").val();

        //add new item to dropdownlist
        $(dropdownId).empty().append(options);


        if (selectedval != undefined && selectedval != "[Select One]") {
            if (selectedval == oldItem[0] && newItem[0] != undefined) {
                $(dropdownId).val(newItem[0]);
            }
            else {
                $(dropdownId).val(selectedval);

                var newSelectedval = $(dropdownId + " option:selected").val();

                if (newSelectedval != selectedval) {
                    if (enterUniqueResponse != undefined)
                        $(dropdownId).val(enterUniqueResponse);
                }
            }
        }
    }
}

function showError(xhr) {
    if (xhr.status == 999)
        this.location = '/Account/LogOn';
    else
        messageDialog.show(JSON.stringify(xhr));
}

//Get the FolderVersion Properties Dialog
formInstanceBuilder.prototype.GetfolderVersionPropertiesDialog = function (folderVersionId) {
    var currentInstance = this;

    $(currentInstance.elementIDs.folderVersionPropertiesDialogJQ).dialog({
        autoOpen: true,
        height: 'auto',
        closeOnEscape: false,
        position: ["middle", 100],
        width: '70%',
        modal: true,
        title: 'Properties'
        //close: function (ev, ui) {
        //    $(this).dialog("close");
        //    // $(currentInstance.elementIDs.folderVersionPropertiesDialogJQ).html('');
        //}
    });

    $(currentInstance.elementIDs.folderVersionPropertiesversionIDJQ).text(currentInstance.folderData.versionNumber);
    //$(currentInstance.elementIDs.folderVersionPropertiescategoryIDJQ).text(currentInstance.folderData.CatID);
    $(currentInstance.elementIDs.folderVersionPropertiescategoryNameJQ).text(currentInstance.folderData.CategoryName);
    loadFolderVersionPropertiesGrid(currentInstance, folderVersionId);
    loadCopyFromAuditTrailGrid(currentInstance, folderVersionId);
    //currentInstance.loadFacetPropertiesGrid(currentInstance, folderVersionId);
}

//formInstanceBuilder.prototype.loadFacetPropertiesGrid = function (currentInstance, folderVersionId) {
//    var colArray = ['Document Name', 'Product ID', 'Folder.01 Draft Creation', 'Last Updated', 'Last Translation', 'Last Transmission'];

//    //set column models
//    var colModel = [];
//    colModel.push({ name: 'DocumentName', index: 'DocumentName', search: false, editable: true, align: 'left', formatter: this.formatColumn });
//    colModel.push({ name: 'ProductID', index: 'ProductID', search: false, editable: false, align: 'left', classes: 'word-wrap', formatter: this.formatColumn });
//    colModel.push({ name: 'DraftCreationDate', index: 'DraftCreationDate', search: false, editable: false, align: 'left', classes: 'word-wrap', formatter: 'date', align: 'center', formatoptions: JQGridSettings.DateTimeFormatterOptions, cellattr: function (rowId, val, rawObject) { return currentInstance.cellAttrformatColumn(rowId, val, rawObject); } });
//    colModel.push({ name: 'LastUpdated', index: 'LastUpdated', search: false, editable: false, align: 'left', classes: 'word-wrap', formatter: 'date', align: 'center', formatoptions: JQGridSettings.DateTimeFormatterOptions, cellattr: function (rowId, val, rawObject) { return currentInstance.cellAttrformatColumn(rowId, val, rawObject); } });
//    colModel.push({ name: 'LastTranslation', index: 'LastTranslation', search: false, editable: false, align: 'left', classes: 'word-wrap', formatter: 'date', align: 'center', formatoptions: JQGridSettings.DateTimeFormatterOptions, cellattr: function (rowId, val, rawObject) { return currentInstance.cellAttrformatColumn(rowId, val, rawObject); } });
//    colModel.push({ name: 'LastTransmission', index: 'LastTransmission', search: false, editable: false, align: 'left', classes: 'word-wrap', formatter: 'date', align: 'center', formatoptions: JQGridSettings.DateTimeFormatterOptions, cellattr: function (rowId, val, rawObject) { return currentInstance.cellAttrformatColumn(rowId, val, rawObject); } });

//    //clean up the grid first - only table element remains after this
//    $(currentInstance.elementIDs.facetPropertiesGridJQ).jqGrid('GridUnload');
//    //adding the pager element
//    $(currentInstance.elementIDs.facetPropertiesGridJQ).parent().append("<div id='p" + currentInstance.elementIDs.facetPropertiesGrid + "'></div>");

//    var url = '/Translator/GetFacetPropertiesData?folderVersionId=' + folderVersionId;

//    $(currentInstance.elementIDs.facetPropertiesGridJQ).jqGrid({
//        url: url,
//        datatype: 'json',
//        cache: false,
//        colNames: colArray,
//        colModel: colModel,
//        height: '200px',
//        rowNum: 10,
//        loadonce: true,
//        width: '70%',
//        autowidth: true,
//        viewrecords: true,
//        caption: "Translator/Transmission Information",
//        altRows: true,
//        pager: '#p' + currentInstance.elementIDs.facetPropertiesGrid,
//        altclass: 'alternate',
//        onPaging: function (pgButton) {
//            //To disable next button on Pager of grid when there are no more rows.
//            if (pgButton === "user" && !IsEnteredPageExist(currentInstance.elementIDs.facetPropertiesGrid)) {
//                return "stop";
//            }
//        },
//        gridComplete: function () {
//            var rows = $(this).getDataIDs();
//        },
//        resizeStop: function (width, index) {
//            autoResizing(currentInstance.elementIDs.facetPropertiesGridJQ);
//        }
//    });
//    var pagerElement = '#p' + currentInstance.elementIDs.facetPropertiesGrid;
//    //Increase height of "Number Of rows" Input TextBox 
//    $(pagerElement).find('input.ui-pg-input').css('height', '20px');
//    $(currentInstance.elementIDs.facetPropertiesGrid).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
//}

formInstanceBuilder.prototype.cellAttrformatColumn = function (rowId, val, rawObject) {
    if (val == null)
        return "";
    if (rawObject.TranslationStatus == FacetStatus.Queued && rawObject.TransmissionStatus == FacetStatus.Queued)
        return 'class="highlight-blue"';
    else if (rawObject.TranslationStatus == FacetStatus.Queued)
        return 'class="highlight-blue"';
    else if (rawObject.TransmissionStatus == FacetStatus.Queued)
        return 'class="highlight-green"';
    else
        return val;
}

formInstanceBuilder.prototype.formatColumn = function (cellValue, options, rowObject) {
    var result;
    if (cellValue == null)
        return "";
    if (rowObject.TranslationStatus == FacetStatus.Queued && rowObject.TransmissionStatus == FacetStatus.Queued)
        return "<font color='3693d0'>" + cellValue + "</font>";
    else if (rowObject.TranslationStatus == FacetStatus.Queued)
        return "<font color='3693d0'>" + cellValue + "</font>";
    else if (rowObject.TransmissionStatus == FacetStatus.Queued)
        return "<font color='3693d0'>" + cellValue + "</font>";
    else
        return cellValue;
}

//load the FolderVersion PropertiesDialog Grid
function loadFolderVersionPropertiesGrid(currentInstance, folderVersionId) {
    var colArray = ['Document Name', 'Design Document Name', 'Version Number'];

    //set column models
    var colModel = [];
    colModel.push({ name: 'FormInstanceName', index: 'FormInstanceName', search: false, editable: false, align: 'left', classes: 'word-wrap' });
    colModel.push({ name: 'FormDesignName', index: 'FormDesignName', search: false, editable: false, align: 'left' });
    colModel.push({ name: 'VersionNumber', index: 'VersionNumber', editable: false, align: 'center', sortable: false, width: '100' });

    //clean up the grid first - only table element remains after this
    $(currentInstance.elementIDs.folderVersionPropertiesGridJQ).jqGrid('GridUnload');
    //adding the pager element
    $(currentInstance.elementIDs.folderVersionPropertiesGridJQ).parent().append("<div id='p" + currentInstance.elementIDs.folderVersionPropertiesGrid + "'></div>");

    var url = '/FormInstance/GetPropertiesData?folderVersionId=' + folderVersionId;

    $(currentInstance.elementIDs.folderVersionPropertiesGridJQ).jqGrid({
        url: url,
        datatype: 'json',
        cache: false,
        colNames: colArray,
        colModel: colModel,
        height: '200px',
        rowNum: 10,
        loadonce: true,
        width: '70%',
        autowidth: true,
        viewrecords: true,
        caption: "Document Design Information",
        hidegrid: true,
        altRows: true,
        pager: '#p' + currentInstance.elementIDs.folderVersionPropertiesGrid,
        altclass: 'alternate',
        onPaging: function (pgButton) {
            //To disable next button on Pager of grid when there are no more rows.
            if (pgButton === "user" && !IsEnteredPageExist(currentInstance.elementIDs.folderVersionPropertiesGrid)) {
                return "stop";
            }
        },

        gridComplete: function () {
            var rows = $(this).getDataIDs();
        },
        resizeStop: function (width, index) {
            autoResizing(currentInstance.elementIDs.folderVersionPropertiesGridJQ);
        }
    });
    var pagerElement = '#p' + currentInstance.elementIDs.folderVersionPropertiesGrid;
    //Increase height of "Number Of rows" Input TextBox 
    $(pagerElement).find('input.ui-pg-input').css('height', '20px');
    $(currentInstance.elementIDs.folderVersionPropertiesGridJQ).jqGrid('navGrid', pagerElement, {
        edit: false, add: false, del: false, search: false, refresh: false
    });
}

function loadCopyFromAuditTrailGrid(currentInstance, folderVersionId) {
    var colArray = ['CopyFromAuditTrailID', 'Document Name', 'Copy On Date', 'Folder Name', 'FolderID', 'FolderVersionID', 'Folder Effective Date', 'Folder Version', 'Document Name'];

    //set column models
    var colModel = [];
    colModel.push({ name: 'CopyFromAuditTrailID', index: 'CopyFromAuditTrailID', hidden: true, search: false, editable: false, align: 'left', classes: 'word-wrap', width: '100' });
    colModel.push({
        name: 'DestinationDocumentName', index: 'DestinationDocumentName', search: false, editable: false, align: 'center', classes: 'word-wrap', width: '100'
    });
    colModel.push({
        name: 'AddedDate', index: 'AddedDate', search: false, editable: false, formatter: 'date', align: 'center', formatoptions: JQGridSettings.DateFormatterOptions, width: '100'
    });
    //colModel.push({ name: 'AccountName', index: 'AccountName', editable: false, align: 'center', sortable: false, width: '100' });
    colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'center', sortable: false, width: '100' });
    colModel.push({ name: 'FolderID', index: 'FolderID', editable: false, hidden: true, align: 'center', sortable: false, width: '100' });
    colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', editable: false, hidden: true, align: 'center', sortable: false, width: '100' });
    colModel.push({ name: 'FolderEffectiveDate', index: 'FolderEffectiveDate', search: false, align: 'center', editable: false, sortable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, width: '100' });
    colModel.push({ name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: false, align: 'center', sortable: false, width: '100' });
    colModel.push({ name: 'SourceDocumentName', index: 'SourceDocumentName', editable: false, align: 'center', sortable: false, width: '100', formatter: returnHyperLink });

    //clean up the grid first - only table element remains after this
    $(currentInstance.elementIDs.copyFromAuditTrailGridJQ).jqGrid('GridUnload');
    //adding the pager element
    $(currentInstance.elementIDs.copyFromAuditTrailGridJQ).parent().append("<div id='p" + currentInstance.elementIDs.copyFromAuditTrailGrid + "'></div>");

    var url = '/FormInstance/GetCopyAuditTrailData?folderVersionId=' + folderVersionId;

    $(currentInstance.elementIDs.copyFromAuditTrailGridJQ).jqGrid({
        url: url,
        datatype: 'json',
        cache: false,
        colNames: colArray,
        colModel: colModel,
        height: '200px',
        rowNum: 10,
        loadonce: true,
        width: '70%',
        autowidth: true,
        viewrecords: true,
        caption: "Document ‘Copy From’ Audit Trail",
        hidegrid: true,
        altRows: true,
        pager: '#p' + currentInstance.elementIDs.copyFromAuditTrailGrid,
        altclass: 'alternate',
        onPaging: function (pgButton) {
            //To disable next button on Pager of grid when there are no more rows.
            if (pgButton === "user" && !IsEnteredPageExist(currentInstance.elementIDs.copyFromAuditTrailGrid)) {
                return "stop";
            }
        },

        gridComplete: function () {
            var rows = $(this).getDataIDs();
            var rowdata = $(this).jqGrid('getGridParam', 'data');
            for (var i = 1; i <= rows.length; i++) {
                var rowData1 = $(this).jqGrid('getRowData', i);
                if (rowData1.AddedDate == " " || rowData1.AddedDate == null || rowData1.AddedDate == "NaN/NaN/NaN") {
                    rowData1.AddedDate = "Not Copied";
                }
                if (rowData1.FolderEffectiveDate == " " || rowData1.FolderEffectiveDate == "NaN/NaN/NaN" || rowData1.FolderEffectiveDate == null) {
                    rowData1.FolderEffectiveDate = "Not Copied";
                }
                $(this).jqGrid('setRowData', i, rowData1);
            }
        },
        resizeStop: function (width, index) {
            autoResizing(currentInstance.elementIDs.copyFromAuditTrailGridJQ);
        }
    });
    var pagerElement = '#p' + currentInstance.elementIDs.copyFromAuditTrailGrid;
    //Increase height of "Number Of rows" Input TextBox 
    $(pagerElement).find('input.ui-pg-input').css('height', '20px');
    $(currentInstance.elementIDs.copyFromAuditTrailGridJQ).jqGrid('navGrid', pagerElement, {
        edit: false, add: false, del: false, search: false, refresh: false
    });

    $(currentInstance.elementIDs.copyFromAuditTrailGridJQ).jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
      { startColumnName: 'CopyFromAuditTrailID', numberOfColumns: 1, titleText: '.' },
      {
          startColumnName: 'FolderName', numberOfColumns: 7, titleText: "Source Document Details"
      }
        ]
    });

    function returnHyperLink(cellValue, options, rowdata, action) {
        var url = currentInstance.URLs.folderVersionDetailsPortfolioBasedAccount.replace(/{tenantId}/g, currentInstance.tenantId).replace(/{folderVersionId}/g, rowdata.FolderVersionID).replace(/{folderId}/g, rowdata.FolderID).replace(/{foldeViewMode}/g, currentInstance.folderData.FolderViewMode);
        url = url.replace(/{mode}/g, true);
        if (cellValue != "Not Copied") {
            return "<a href='" + url + "' class='documentLink' >" + cellValue + "</a>";
        }
        else {
            return cellValue;
        }

    }
}


function autoCompleteDropDown($) {
    $.widget("custom.filterDropDown", {
        _create: function () {
            this.wrapper = $("<span>")
              .addClass("custom-combobox")
              .insertAfter(this.element);
            var callback = this.options.value;
            var ID = this.options.ID;
            var disableField = this.options.isDisabled;
            $(this.element).css('visibility', 'hidden');
            this._createAutocomplete(callback, ID);
            this._createShowAllButton();
            this._disableFields(disableField, ID);
        },
        _createAutocomplete: function (callback, ID) {
            var selected = this.element.children(":selected"),
              value = selected.val() ? selected.text() : "";
            this.input = $("<input style='width:90% !important;height:20px; background-color: #fff;'>")
              .appendTo(this.wrapper)
              .val(value)
              .attr("title", "")
              .attr("id", ID)
              .addClass("custom-combobox-input ui-widget ui-widget-content ui-state-default ui-corner-left")
              .autocomplete({
                  delay: 0,
                  minLength: 0,
                  source: $.proxy(this, "_source")
              })
              .tooltip({
                  tooltipClass: "ui-state-highlight"
              });

            $(this.wrapper).css("position", "relative");
            $(this.wrapper).css("top", "-20px");

            this._on(this.input, {
                autocompleteselect: function (event, ui) {
                    ui.item.option.selected = true;
                    this._trigger("select", event, {
                        item: ui.item.option
                    });
                    if (callback != undefined)
                        callback(ui.item.option.value);
                },
                autocompletechange: "_removeIfInvalid"
            });
        },
        _createShowAllButton: function () {
            var input = this.input,

              wasOpen = false;
            $("<a> <span class='ui-button-icon-primary ui-icon ui-icon-search'><span class='ui-button-text'>")
              .attr("tabIndex", -1)
              .attr("title", "Show All Items")
              //.attr("type", "text")
              .tooltip()
              .appendTo(this.wrapper)
              .button({
                  icons: {
                      primary: "ui-icon-triangle-1-s"
                  },
                  text: false
              })
              .removeClass("ui-corner-all")
              .addClass("ui-button ui-widget ui-state-default ui-button-icon-only custom-combobox-toggle ui-corner-right")
              .mousedown(function () {
                  wasOpen = input.autocomplete("widget").is(":visible");
              })
              .click(function () {
                  input.focus();
                  // Close if already visible
                  if (wasOpen) {
                      return;
                  }
                  // Pass empty string as value to search for, displaying all results
                  input.autocomplete("search", "");

                  // To apply class to dropDown items
                  var ul = $('ul.ui-autocomplete').filter(function () {
                      return $(this).css('display') == 'block';
                  });
                  if (ul !== undefined && ul !== null && ul.length > 0) {
                      // Actual dropDown elements
                      var elements = $(input[0]).parent().prev().children();
                      if (elements !== undefined && elements != null && elements.length > 0) {
                          // autocomplete dropDown elements
                          var listItems = $(ul).find('li');
                          if (listItems !== undefined && listItems != null && listItems.length > 0) {
                              var selectedItem = elements.filter(function (item, obj) {
                                  return obj.selected == true;
                              });
                              var selectedIndex = -1;
                              var isUniqueResponse = false;
                              if (selectedItem !== undefined && selectedItem != null && selectedItem.length > 0) {
                                  selectedIndex = selectedItem[0].index;
                                  if (selectedItem[0].className == "non-standard-optn") {
                                      isUniqueResponse = true;
                                  }
                              }
                              // Add standared/ non-stanadared class to dropdown textbox
                              elements.each(function (index, obj) {
                                  $(listItems[index]).addClass(obj.className);
                              });
                              if (selectedIndex != -1) {
                                  if (isUniqueResponse) {
                                      $(listItems[selectedIndex]).removeClass('non-standard-optn');
                                  }
                                  // make selected item highlight in dropdown
                                  $(listItems[selectedIndex]).css('background-color', '#0099cc');
                                  $(listItems[selectedIndex]).find("a").css('color', '#ffffff');
                              }
                          }
                      }
                  }
              });
        },
        _disableFields: function (disabled, Id) {
            if (disabled == true) {

                $('#' + Id).addClass('disabledField');
                $('#' + Id).siblings('a').addClass('disabledField');
            }
        },
        _source: function (request, response) {
            var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
            response(this.element.children("option").map(function () {
                var text = $(this).text();
                if (this.value && (!request.term || matcher.test(text)))
                    return {
                        label: text,
                        value: text,
                        option: this
                    };
            }));
        },
        _removeIfInvalid: function (event, ui) {
            // Selected an item, nothing to do
            if (ui.item) {
                $('#' + this.element[0].id).trigger('change');
                return;
            }
            // Search for a match (case-insensitive)
            var value = this.input.val(),
              valueLowerCase = value.toLowerCase(),
              valid = false;
            this.element.children("option").each(function () {
                if ($(this).text().toLowerCase() === valueLowerCase) {
                    this.selected = valid = true;
                    return false;
                }
            });
            // Found a match, nothing to do
            if (valid) {
                $('#' + this.element[0].id).trigger('change');
                return;
            }
            // Remove invalid value
            this.input.val("[Select One]");
            this.element.val("[Select One]");
            $('#' + this.element[0].id).trigger('change');
        },
        // added- inserted refresh function -BD
        refresh: function () {
            selected = this.element.children(":selected");
            this.input.val(selected.text());
        },
        _destroy: function () {
            this.wrapper.remove();
            this.element.show();
        },

        select: function (event, ui) {
            ui.item.option.selected = true;
            self._trigger("selected", event, {
                item: ui.item.option
            });
            select.trigger("change");
        },

        change: function (event, ui) {
            if (!ui.item) {
                var matcher = new RegExp("^" + $.ui.autocomplete.escapeRegex($(this).val()) + "$", "i"),
                    valid = false;
                select.children("option").each(function () {
                    if ($(this).text().match(matcher)) {
                        this.selected = valid = true;
                        return false;
                    };
                });
                if (!valid) {
                    // remove invalid value, as it didn't match anything
                    $(this).val("");
                    select.val("");
                    input.data("autocomplete").term = "";
                    return false;
                };
            };
        }
    });
}

function initDocumentLockWorker(currentInstance) {
    if (currentInstance.isDocumentEditable) {
        $(currentInstance.elementIDs.btnUnlockJQ).show();

        //Start a new Worker Thread for Folder Lock functionality
        currentInstance.documentLockWorker = new Worker(currentInstance.URLs.folderLockWorker);
        console.log('Document Lock Worker created');
        currentInstance.documentLock.InitializeDocumentLock();
    }
}
var currInstance = null;
function documentLockMethods(currentInstance) {
    currInstance = currentInstance;
    //Document Lock checked at a specific duration automatically.
    setDocumentOverrideCheckTimeout = function () {
        //if (currentInstance.folderData.isFolderLockEnable) {
        if (currentInstance.checkDocumentLockOverrideTimer != undefined) {
            currentInstance.documentLock.clearDocumentLockLockTimeOut();
        }
        currentInstance.checkDocumentLockOverrideTimer = setTimeout(function () {
            currentInstance.documentLock.postMessageAndGetData(currInstance);
        }, 120000);
        //}
    }

    return {

        InitializeDocumentLock: function () {
            try {
                if (typeof (Worker) !== "undefined" && currentInstance.documentLockWorker != undefined) {
                    currentInstance.documentLockWorker.onmessage = function (e) {

                        if (e.data != undefined) {
                            if (e.data == "true") {
                                currentInstance.hasDocumentLockOverriden = true;
                            }
                            else if (e.data == "false") {
                                currentInstance.hasDocumentLockOverriden = false;
                                currentInstance.documentLock.showDocumentLockAlert();
                                setDocumentOverrideCheckTimeout();
                            }
                        }
                    };
                    currentInstance.documentLockWorker.onerror = function (err) {
                        console.log('Worker is suffering!', err);
                    }

                    setDocumentOverrideCheckTimeout();
                }
                else {
                    console.log("Sorry, your browser does not support WEB WORKERS..For Folder LOCK");
                }
            } catch (ex) {
                console.log(ex);
            }
        },

        postMessageAndGetData: function (currInstance) {
            //if (currentInstance.folderData.isFolderLockEnable) {
            if (currInstance != null) {
                //var sectioName = currentInstance + '.sections.' + currentInstance.selectedSection + '. FullName';
                var sectioName = currInstance.sections[currInstance.selectedSection].FullName; //eval(sectioName);//sectioName = eval(sectioName);
                var folderData = {
                    FormInstanceId: currInstance.formInstanceId,
                    formDesignId: currInstance.formDesignId,
                    sectionName: sectioName
                }
                currInstance.documentLockWorker.postMessage({
                    url: currInstance.URLs.getDocumentLockOverridenStatus,
                    formInstanceId: currInstance.formInstanceId,
                    formDesignId: currInstance.formDesignId,
                    sectionName: sectioName,
                    saveData: JSON.stringify(folderData)
                });
            }
            else if (currInstance != null) {
                setDocumentOverrideCheckTimeout();
            }
            currInstance.checkDocumentLockOverrideTimer = setTimeout(function () {
                currInstance.documentLock.postMessageAndGetData(currInstance);
            }, 120000);
            //}
        },

        showDocumentLockAlert: function () {
            //$(currentInstance.elementIDs.folderLockAlertJQ).show();
            documentLockOverridenDialog.show(FolderLock.unlockedDocumentMsg, function (e) {
                if (e == true) {
                    window.location.reload();
                } else {
                    console.log("Page does not refresh...");
                }
                documentLockOverridenDialog.hide(function () {
                    $(currentInstance.elementIDs.folderLockAlertJQ).hide();
                    window.location.reload();
                });
            });
        },

        clearDocumentLockLockTimeOut: function () {
            clearTimeout(currentInstance.checkDocumentLockOverrideTimer);
        },

        resetCurrentUserLockSession: function () {
            InitialiseUserLockSession(); //global function 
        },

        resetCurrentInstance: function (currentInstance) {
            currInstance = currentInstance; //global function 
        }


    }
}
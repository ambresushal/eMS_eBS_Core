var timer = 60;
var refreshInterval;

var folderManager = function () {
    //variable to hold the instance
    var instance;

    function folder(accountId, folderVersionId, folderId, referenceformInstanceId, isEditable, folderType, viewMode, currentUserId, currentUserName) {
        this.accountId = accountId;
        this.folderVersionId = folderVersionId;
        this.folderId = folderId;
        this.currentUserId = currentUserId;
        this.currentUserName = currentUserName;
        this.data = undefined;
        this.formInstances = undefined;
        this.tabs = undefined;
        this.isEditable = isEditable;
        this.isReleased = folderData.isReleased == 'True' || false;
        this.currentFormInstanceID = undefined;
        this.autoSaveTimer = undefined;
        this.autoSaveWorker = undefined;
        this.folderLockWorker = undefined;
        this.folderLock = this.folderLockMethods();
        this.allProductStatusInFolder = new Object();
        this.hasLockOverriden = undefined;
        this.checkFolderLockOverrideTimer = undefined;
        if (typeof vbIsHiddenContainer === "undefined") { this.isHiddenContainer = false } else { this.isHiddenContainer = vbIsHiddenContainer };
        if (typeof vbIsHiddenOrDisableSections === "undefined") { this.isSectionPermissionsApplicable = false } else { this.isSectionPermissionsApplicable = vbIsHiddenOrDisableSections };
        this.pdfgeneration = pdfGenerationMethod();
        if (typeof vbIsFolderLockEnable === "undefined") { this.isFolderLockEnable = false } else { this.isFolderLockEnable = vbIsFolderLockEnable };
        var currentInstance = this;
        this.isfolderInTranslation = undefined;
        this.folderType = folderType;
        this.folderData = folderData;
        this.IsMasterList = folderType == "MasterList" ? true : false;
        // this.openDocumentList = [];
        this.openDocuments = [];
        this.iscollateralReportMenuItemLoaded = false;
        this.formInstanceFactory = new Factory();
        this.folderViewMode = viewMode;
        this.viewManager = this.folderViewMode == FolderViewMode.DefaultView ? new tabManager(currentInstance) : new sotManager(currentInstance);
        instance = currentInstance;
        this.isFormInstanceLodedFromOutSide = false;
        this.URLs = {
            //get Folder Version Workflow
            forminstancelisturl: '/FolderVersion/GetFormInstanceList?tenantId=1&folderVersionId={folderVersionId}&folderId={folderId}',
            //get Folder Version Workflow
            previewAllInstancesUrl: '/FolderVersion/PreviewAllInstances?tenantId=1&folderVersionId={folderVersionId}&folderId={folderId}',
            previewFormInstance: '/FolderVersion/PreviewFormInstance?formInstanceId={formInstanceId}&formDesignVersionId={formDesignVersionId}&folderVersionId={folderVersionId}&roleID={roleID}',
            deleteFormInstance: '/FormInstance/DeleteFormInstance?folderId={folderId}&tenantId=1&folderVersionId={folderVersionId}&formInstanceId={formInstanceId}',
            isDataSource: '/FormInstance/IsDataSource?&formDesignID={formDesignID}&formDesignVersionID={formDesignVersionID}',
            autoSaveWorker: '/Scripts/FolderVersion/autoSave.js',
            releaseLock: '/UnAuthenticated/ReleaseFolderLock?tenantId=1&folderId={folderId}&isNavigate=false',
            overrideFolderLock: '/FolderVersion/OverrideFolderLock?tenantId=1&folderId={folderId}',
            folderLockWorker: '/Scripts/FolderVersion/folderLock.js',
            getFolderLockOverridenStatus: '/FolderVersion/CheckFolderLockIsOverriden',
            checkUserIsTeamManager: '/FolderVersion/CheckUserIsTeamManager',
            getAllFormsStatus: '/FolderVersion/GetAllFormsStatus?folderVersionId={folderVersionId}&folderType={folderType}',
            folderVersionIndex: '/FolderVersion/Index?tenantId=1&folderVersionId={folderVersionId}&folderId={folderId}&mode={mode}',
            getMasterListFormDesignID: '/FolderVersion/GetMasterListFormDesignID?folderVersionId={folderVersionId}',
            getFormInstanceList: '/FolderVersion/GetFormInstancesList',
            cascadeMasterList: '/MasterList/CascadeMasterList',
            addToReportQueue: "/ReportingCenter/AddSOTQueue",
            getDefaultViewFormInstanceList: '/FolderVersion/GetFormInstancesList',
            isReportAlreadyGenerated: '/DocumentCollateral/IsReportAlreadyGenerated?formInstanceId={formInstanceId}&reportTemplateId={reportTemplateId}',
            queueCollateral: "/DocumentCollateral/QueueCollateral?accountID={accountID}&accountName={accountName}&folderID={folderID}&folderName={folderName}&formInstanceIDs={formInstanceIDs}&folderVersionID={folderVersionID}&folderVersionNumbers={folderVersionNumbers}&reportTemplateID={reportTemplateID}&productIds={productIds}&folderVersionEffDt={folderVersionEffDt}&runDate={runDate}&reportName={ReportName}",
            getQueuedCollateralsList: "/DocumentCollateral/GetQueuedCollateralsList",
            getCollateralTemplatesForReportGeneration: "/DocumentCollateral/GetCollateralTemplatesForReportGeneration?formInstanceId={formInstanceId}",
            ViewCollateralsAtFolder: "/DocumentCollateral/ViewCollateralsAtFolder?formInstanceId={formInstanceId}",
            ShowAllProducts: "/FormInstance/ShowAllProducts?formInstanceId={formInstanceId}&formDesignVersionId={formDesignVersionId}",
        };

        this.elementIDs = {
            //container element for form instances
            //forminstancelistjq: '#foldertabs',
            //documentContainerDivJQ: "#documentContainer",
            //documentHeaderContainerJQ: "#documentheadercontainer",
            documentheadercontainer: "#documentheadercontainer",
            sectionMenuContainer: "#sectionMenuContainer",
            btnSOTAddJQ: '#btnAddSOTView',
            btnSOTOpenJQ: '#btnOpenSOTView',
            btnSOTSaveJQ: '#btnSaveSOTView',
            btnCreateFormJQ: '#btnCreateForm',
            btnSaveFormDataJQ: '#btnSaveFormData',
            btnBottomSaveFormDataJQ: '#btnBottomSaveFormData',
            btnBottomCreateNewVersion: '#btnBottomCreateNewVersion',
            btnBottomGenerateSOTFile: '#btnBottomGenerateSOTFile',
            btnRealoadFormDataJQ: '#btnReloadFormData',
            btnValidateJQ: "#btnValidate",
            btnValidateSectionJQ: "#btnValidateSection",
            btnExportToPDf: "#btnExportToPDf",
            btnCascadeML: "#btnCascadeML",
            btnExportFolderToPDf: "#btnExportFolderToPDf",
            btnStatusUpdateJQ: "#statusupdatebutton",
            btnVersionHistoryJQ: "#versionHistoryButton",
            btnBaselineJQ: "#btnBaseline",
            btnValidateAllJQ: "#btnValidateAll",
            btnNewVersionJQ: "#newVersionHistory",
            btnNewVersionMLJQ: "#newVersionHistoryML",
            btnRetroJQ: "#retroVersionHistory",
            btnRetroMLJQ: "#retroVersionHistoryML",
            btnJournalEntryJQ: "#btnJournalEntry",
            btnReplaceTextJQ: "#btnReplaceText",
            btnDeleteFormInstance: "#btnDeleteForm",
            btnShowAllProducts: "#btnShowAllProducts",
            btnFolderLockJQ: "#folderLockAlert",
            btnLockJQ: "#btnLock",
            btnUnlockOverrideJQ: "#btnUnlockOverride",
            scrollTop: "#scrollTop",
            folderLockAlertJQ: '#folderLockAlert',
            btnMenuOptionsJQ: '#btnMenuOptions',
            btnAssignFolderMember: '#assignFolderMemberbutton',
            btnPropertiesJQ: "#btnProperties",
            collateralMenuItemTemplate: '#dropDownMenu #collateralMenuItemTemplate',
            documentMenuIcons: '#dropDownMenu #documentIcons',
            btnOpenFormJQ: '#btnOpenForm',
            retroNote: '#retroNote',
            openDocumentGridJQ: '#openDocumentGrid',
            viewDropdown: '#viewDropdown',
            retroNoteMLJQ: '#retroNoteML',
            queuedCollateralsGrid: "downloadQueuedCollateralsGrid",
            queuedCollateralsGridJQ: "#downloadQueuedCollateralsGrid",
            showAllProductsGrid: "#showAllProductsGrid",
            QueuedCollateralDialog: "#downloadCollateralDialog",
            showAllProductsDialog: "#showAllProductsDialog",
            btnCloseCollateralDialog: "#closeCollateralDialog",
            btnViewCollateral: "#btnViewCollateral",
            downloadQueueCollateralDialog: "#dwnQueueCnlConfirmDialog",
            btnExitValidateResultsJQ: "#btnExitValidateResults",
            bottommenu: "#bottom-menu",
            btnViewFaxBackReport: '#btnViewFaxBackReport',
            btnViewBenefitMatrixReport: '#btnViewBenefitMatrixReport',
            btnExitValidate: '#btnExitValidate',
            btnShowAllProducts: '#btnShowAllProducts',
            collateralReportMenuItems: '.collateralReportMenuItem'
        };


        function loadFolderTabs(formInstances, isFormInstanceLodedFromOutSide) {
            if (currentInstance.IsMasterList == false && isFormInstanceLodedFromOutSide != false) {
                currentInstance.isFormInstanceLodedFromOutSide = true;
            }
            else {
                currentInstance.isFormInstanceLodedFromOutSide = false;
            }

            if (formInstances == null || formInstances.length < 1) {
                currentInstance.disableButton();
                formInstances = [];
            }
            else {
                currentInstance.enableButton();
            }

            if (!currentInstance.IsMasterList)
                currentInstance.checkUserIsTeamManager();

            if (!currentInstance.IsMasterList) {
                var fibr = formDesignVersionRulesPreLoadManager.getInstance();
                fibr.init();
            }
            currentInstance.loadAllFormsStatus(currentInstance.folderVersionId);

            //initialize auto save
            initAutoSaveWorker();

            //initialize folder lock
            //initFolderLockWorker();

            //release Folder Lock.
            initReleaseFolderLock();

            if (currentInstance.formInstances == undefined) {
                currentInstance.formInstances = [];
            }

            manageView(formInstances);
            setWindowScroll();

        }

        function manageView(formInstances) {
            currentInstance.viewManager.initialize();

            if (formInstances.length != undefined) {
                $.each(formInstances, function (i, item) {
                    manageFormInstances(item);
                    currentInstance.viewManager.addTab(item);
                });
            }
            else {
                manageFormInstances(formInstances);
                currentInstance.viewManager.addTab(formInstances);
            }

            currentInstance.viewManager.setAttributes();
            currentInstance.viewManager.registerEvent();
            currentInstance.viewManager.setActiveTab();
        }

        function loadActiveFormInstancce(documentId, oldFormInstanceID, sibling) {

            if (currentInstance.openDocuments.length == 0) {
                return;
            }

            var formInstanceID = currentInstance.openDocuments[documentId].SelectedViewID;
            var anchorFormInstanceID = currentInstance.openDocuments[documentId].AnchorDocumentID;
            currentInstance.currentFormInstanceID = formInstanceID;

            //get form instance data from the array.
            var formInstanceData = currentInstance.formInstances[formInstanceID];

            if (currentInstance.isFormInstanceLodedFromOutSide == true) {
                var formInstanceIds = new Array();
                formInstanceIds.push(formInstanceID);
                var getDocStatusUrl = URLs.getDocumentLockStatus.replace(/{folderId}/g, currentInstance.folderId);
                var promise = ajaxWrapper.postAsyncJSONCustom(getDocStatusUrl, formInstanceIds);
                promise.done(function (documentLock) {
                    if (documentLock == "true") {
                        formInstanceData.FormInstance.IsFormInstanceEditable = false;
                    }
                });
            }

            activitylogger.getFormInstanceId(parseInt(formInstanceID));
            activitylogger.getFormName(formInstanceData.FormInstance.FormDesignName);
            activitylogger.loadActivityLogGrid(parseInt(formInstanceID));

            if (ActiveRuleExecutionLogger == "True") {
                ruleExecutionLogger.getFormInstanceId(parseInt(formInstanceID));
                ruleExecutionLogger.getFormName(formInstanceData.FormInstance.FormDesignName);
                ruleExecutionLogger.init();

                if (currentInstance.designData == null) {
                    ruleExecutionLogger.loadRuleExecutionLogGrid(formInstanceData.FormInstanceBuilder);
                }
                else {
                    ruleExecutionLogger.loadRuleExecutionLogGrid(currentInstance);
                }
            }

            //Show Documents like (Benefit Matrix, Fax-Back and SBC) Links only for Medical Document
            setDocumentMenuVisibility(formInstanceData);
            SetValidateMenuVisibility(formInstanceData);
            if (!currentInstance.IsMasterList) {
                //prepareMenuItemsForReport();
                currentInstance.PopulateReportGenerationMenu();
            }
            FormTypes.CURRENTFORMDESIGNID = formInstanceData.FormInstance.FormDesignID;
            if (formInstanceData.FormInstanceBuilder == null) {
                //invoke formInstanceManager instance, save to array
                //var formInstanceBuilderObj = new formInstanceBuilder(formInstanceData.FormInstance.TenantID, currentInstance.accountId,
                //                                currentInstance.folderVersionId, currentInstance.folderId, formInstanceID,
                //                                formInstanceData.FormInstance.FormDesignVersionID, formInstanceData.FormInstance.FormDesignName,
                //                                 currentInstance.autoSaveWorker, currentInstance.autoSaveTimer, currentInstance.isReleased, folderData, folderType, anchorFormInstanceID, currentInstance.folderViewMode);

                var params = {
                    TenantId: formInstanceData.FormInstance.TenantID,
                    AccountId: currentInstance.accountId,
                    //FolderVersionId: currentInstance.folderVersionId,
                    FolderVersionId: formInstanceData.FormInstance.FolderVersionID,
                    FolderId: currentInstance.folderId,
                    FormInstanceId: formInstanceID,
                    FormDesignVersionId: formInstanceData.FormInstance.FormDesignVersionID,
                    FormDesignName: formInstanceData.FormInstance.FormDesignName,
                    AutoSaveWorkder: currentInstance.autoSaveWorker,
                    AutoSaveTimer: currentInstance.autoSaveTimer,
                    IsReleased: formInstanceData.FormInstance.FolderVersionID != currentInstance.folderVersionId ? true : currentInstance.isReleased,
                    FolderData: folderData,
                    FolderType: folderType,
                    AnchorFormInstanceId: anchorFormInstanceID,
                    IsFormInstanceEditable: formInstanceData.FormInstance.IsFormInstanceEditable,
                    EffectiveDate: formatDate(formInstanceData.FormInstance.EffectiveDate),
                    currentUserId: currentInstance.currentUserId,
                    currentUserName: currentInstance.currentUserName
                }
                var formInstanceBuilderObj = currentInstance.formInstanceFactory.createFormInstance(currentInstance.folderViewMode, params);
                // Check if action is SOTView To Classic View Impact Field Navigation.
                if (currentInstance.folderData != undefined && currentInstance.folderData.SectionGeneratedName != undefined
                    && currentInstance.folderData.SectionGeneratedName != null && currentInstance.folderData.SectionGeneratedName != ""
                    && currentInstance.folderData.SectionName != undefined && currentInstance.folderData.SectionName != null
                    && currentInstance.folderData.SectionName != "") {
                    formInstanceBuilderObj.selectedSection = currentInstance.folderData.SectionName;
                    formInstanceBuilderObj.loadFormInstanceDesignData(undefined, currentInstance.folderData.SectionGeneratedName, undefined, sibling);
                    currentInstance.folderData.SectionGeneratedName = "";
                    currentInstance.folderData.SectionName = "";
                }
                else {
                    formInstanceBuilderObj.loadFormInstanceDesignData(undefined, undefined, undefined, sibling);
                }
                formInstanceBuilderObj.bottomMenu.checkTabSelection(oldFormInstanceID);
                formInstanceBuilderObj.registerViewddevent();
                //add the form instance builder object to the form instances array for future use.
                currentInstance.formInstances[formInstanceID].FormInstanceBuilder = formInstanceBuilderObj;
            }
            else {
                if (currentInstance.folderViewMode == FolderViewMode.DefaultView) {
                    formInstanceData.FormInstanceBuilder.menu.renderMenu();
                }
                if (formInstanceData.FormInstanceBuilder.documentLock != null) {
                    formInstanceData.FormInstanceBuilder.documentLock.resetCurrentInstance(formInstanceData.FormInstanceBuilder);
                }
                formInstanceData.FormInstanceBuilder.bottomMenu.checkTabSelection(oldFormInstanceID);

                //already loaded form instance & tab switch
                formInstanceData.FormInstanceBuilder.autosave.InitializeAutoSave();

                //Load ErrorGrid for current tab
                formInstanceData.FormInstanceBuilder.form.showValidations(formInstanceData.FormInstanceBuilder.repeaterBuilders);
                var anchorFormInstanceData = currentInstance.formInstances[anchorFormInstanceID];
                if (anchorFormInstanceData != null && anchorFormInstanceData.FormInstanceBuilder != null) {
                    anchorFormInstanceData.FormInstanceBuilder.registerViewddevent();
                }
                else {
                    formInstanceData.FormInstanceBuilder.registerViewddevent();
                }
            }
            if (GLOBAL.applicationName.toLowerCase() == 'emedicaresync') {
                // Set FormInstanceBuilder for PBPViewAction
                formInstanceData.FormInstanceBuilder.pbpViewAction.setFormInstanceBuilder(formInstanceData.FormInstanceBuilder);
                formInstanceData.FormInstanceBuilder.pbpViewAction.buildImpactGrid();
            }
            bindUnbindFolderMenuEvents(formInstanceID);
        }

        function formatDate(value) {
            value = new Date(value);
            return value.getMonth() + 1 + "/" + value.getDate() + "/" + value.getFullYear();
        }

        function createViewModel(formInstanceIdArr) {
            var modelData = {
                FormInstanceId: formInstanceIdArr
            }
            return modelData;
        }

        function saveReportFolderRow(model) {
            var viewModel = model;
            var promise = ajaxWrapper.postJSON(currentInstance.URLs.addToReportQueue, viewModel);
            //callback function for ajax request success.
            promise.done(function (xhr) {
                if (xhr.Result === ServiceResult.SUCCESS) {
                    messageDialog.show('Report added to Queue, Please wait while page redirects');
                    location.href = "/ReportingCenter/ReportQueue";
                }
                else {
                    messageDialog.show(Common.errorMsg);
                }
            });
        }

        function bindUnbindFolderMenuEvents(formInstanceID) {
            $(currentInstance.elementIDs.btnSaveFormDataJQ).unbind('click');
            $(currentInstance.elementIDs.btnSaveFormDataJQ).bind('click', function () {
                currentInstance.formInstances[formInstanceID].FormInstanceBuilder.form.saveFormInstanceData(true);
            });

            $(currentInstance.elementIDs.btnBottomSaveFormDataJQ).unbind('click');
            $(currentInstance.elementIDs.btnBottomSaveFormDataJQ).bind('click', function () {
                currentInstance.formInstances[formInstanceID].FormInstanceBuilder.form.saveFormInstanceData(true);
            });

            $(currentInstance.elementIDs.btnBottomCreateNewVersion).unbind('click');
            $(currentInstance.elementIDs.btnBottomCreateNewVersion).bind('click', function () {
                folderVersionBaseline.show(true, folderData.currentUserName);
            });

            $(currentInstance.elementIDs.btnBottomGenerateSOTFile).unbind('click');
            $(currentInstance.elementIDs.btnBottomGenerateSOTFile).bind('click', function () {
                var QueueFormInstaces = [];
                currentInstance.formInstances.forEach(function (index, val) {
                    QueueFormInstaces.push(val);
                });
                var model = createViewModel(QueueFormInstaces);
                saveReportFolderRow(model);
                //messageDialog.show("Not Implemented..");
            });

            $(currentInstance.elementIDs.btnSOTSaveJQ).unbind('click');
            $(currentInstance.elementIDs.btnSOTSaveJQ).bind('click', function () {
                currentInstance.formInstances[formInstanceID].FormInstanceBuilder.form.saveFormInstanceData(true);
            });
            $(currentInstance.elementIDs.btnDeleteFormInstance).unbind('click');
            $(currentInstance.elementIDs.btnDeleteFormInstance).bind('click', function () {
                currentInstance.deleteFormInstance();
            });
            $(currentInstance.elementIDs.btnShowAllProducts).unbind('click');
            $(currentInstance.elementIDs.btnShowAllProducts).bind('click', function () {
                currentInstance.ShowAllProducts();
            });
            $(currentInstance.elementIDs.scrollTop).unbind('click');
            $(currentInstance.elementIDs.scrollTop).bind('click', function () {
                currentInstance.scrollToTop();
            });
            $(currentInstance.elementIDs.btnRealoadFormDataJQ).unbind('click');
            $(currentInstance.elementIDs.btnRealoadFormDataJQ).bind('click', function () {
                currentInstance.formInstances[formInstanceID].FormInstanceBuilder.reload();
            });
            $(currentInstance.elementIDs.btnValidateJQ).unbind('click');
            $(currentInstance.elementIDs.btnValidateJQ).bind('click', function () {
                currentInstance.validateCurrentForm(formInstanceID);
            });
            $(currentInstance.elementIDs.btnValidateSectionJQ).unbind('click');
            $(currentInstance.elementIDs.btnValidateSectionJQ).bind('click', function () {
                currentInstance.validateCurrentForm(formInstanceID, true);
            });
            $(currentInstance.elementIDs.btnValidateAllJQ).unbind('click');
            $(currentInstance.elementIDs.btnValidateAllJQ).bind('click', function () {
                currentInstance.validate(true, formInstanceID);
            });

            $(currentInstance.elementIDs.btnExportToPDf).unbind('click');
            $(currentInstance.elementIDs.btnExportToPDf).bind('click', function () {
                var exportToPDF = {
                    formInstanceId: formInstanceID,
                    formDesignVersionId: currentInstance.formInstances[formInstanceID].FormInstanceBuilder.formDesignVersionId,
                    folderVersionId: currentInstance.folderVersionId,
                    formDesignId: currentInstance.formInstances[formInstanceID].FormInstanceBuilder.formDesignId,
                    formName: currentInstance.formInstances[formInstanceID].FormInstanceBuilder.formName,
                    tenantId: folderData.tenantId,
                    accountId: folderData.accountId,
                    folderName: folderData.folderName,

                    folderVersionNumber: folderData.versionNumber,
                    effectiveDate: folderData.effectiveDate
                }

                currentInstance.pdfgeneration.showPDfDialog(exportToPDF);
            });

            $(currentInstance.elementIDs.btnCascadeML).unbind('click');
            $(currentInstance.elementIDs.btnCascadeML).bind('click', function () {

                window.location.href = currentInstance.URLs.cascadeMasterList;
                //var promise = ajaxWrapper.getJSON(url);

                //promise.done(function (result) {

                //});

                ////register ajax failure callback
                //promise.fail(showError);

            });

            $(currentInstance.elementIDs.btnReplaceTextJQ).unbind('click');
            $(currentInstance.elementIDs.btnReplaceTextJQ).click(function (e) {
                var objFormInstanceBuilder = currentInstance.formInstances[currentInstance.currentFormInstanceID].FormInstanceBuilder;
                replaceTextDialog.show(currentInstance, objFormInstanceBuilder);
            });


            //register
            if (folderData.isAutoSaveEnabled === "True") {
                var showMsgTimer;
                //de-register the previous event
                $(window).off('webapp:page:closing');
                //register new event
                $(window).on('beforeunload', function () {
                    var e = $.Event('webapp:page:closing');
                    $(window).trigger(e); // let other modules determine whether to prevent closing
                    if (e.isDefaultPrevented()) {
                        showMsgTimer = window.setTimeout(console.log("you have been trolled."), 500);
                        // e.message is optional
                        return e.message || Common.documentChangesAreUnsavedDefaultMsg;
                    }
                });

                $(window).on('webapp:page:closing', function (e) {
                    if (currentInstance.formInstances[formInstanceID].FormInstanceBuilder.form.hasChanges()) {
                        e.preventDefault();
                        FolderLockAction.ISAUTOSAVEACTION = 1;
                        e.message = Common.documentChangesAreUnsavedMsg;
                        window.onunload = function () {
                            currentInstance.releaseFolderLockOnNavigate(currentInstance.URLs.releaseLock);
                            clearTimeout(showMsgTimer);
                        }
                    }
                });
            }

            $(currentInstance.elementIDs.btnUnlockOverrideJQ).unbind('click');
            $(currentInstance.elementIDs.btnUnlockOverrideJQ).bind('click', function () {
                currentInstance.overrideFolderLock(currentInstance.URLs.overrideFolderLock);
            });

            $(currentInstance.elementIDs.btnViewCollateral).unbind('click');
            $(currentInstance.elementIDs.btnViewCollateral).bind('click', function () {
                ViewCollateralsAtFolder(currentInstance);
            });
        }

        function manageFormInstances(formInstance) {
            currentInstance.formInstances[formInstance.FormInstanceID] = { 'ID': formInstance.FormInstanceID, 'FormInstanceBuilder': undefined, 'FormInstance': formInstance, 'AnchorDocumentID': formInstance.AnchorDocumentID };
            currentInstance.openDocuments[formInstance.AnchorDocumentID] = { 'AnchorDocumentID': formInstance.AnchorDocumentID, 'SelectedViewID': formInstance.FormInstanceID, 'Status': OpenDocumentStatus.Open, 'FormInstance': formInstance };
        }

        function initReleaseFolderLock() {
            if (folderData.isEditable.toString() == "True" && folderData.isLocked == "False" && currentInstance.isFolderLockEnable) {

                //de-register the previous event
                $(window).off('webapp:page:closing');
                //register new event
                $(window).off('beforeunload');
                $(window).on('beforeunload', function () {
                    var e = $.Event('webapp:page:closing');
                    $(window).trigger(e);
                    if (FolderLockAction.ISREPEATERACTION !== 1 && FolderLockAction.ISAUTOSAVEACTION !== 1 && FolderLockAction.ISOVERRIDEDIALOGACTION !== 1) {
                        currentInstance.releaseFolderLockOnNavigate(currentInstance.URLs.releaseLock);
                    } else {
                        FolderLockAction.ISREPEATERACTION = 0;
                        FolderLockAction.ISAUTOSAVEACTION = 0;
                        FolderLockAction.ISOVERRIDEDIALOGACTION = 0;
                    }
                });
            }
        }

        function setWindowScroll() {
            $(window).scroll(function () {
                var offset = 100;
                var duration = 500;
                if ($(this).scrollTop() > offset) {
                    $('.scroll-to-top').fadeIn(duration);
                    $('.scroll-to-top-after-expanding-bottom-menu').fadeIn(duration);
                } else {
                    $('.scroll-to-top').fadeOut(duration);
                    $('.scroll-to-top-after-expanding-bottom-menu').fadeOut(duration);
                }
            });
        }

        function initAutoSaveWorker() {
            if (currentInstance.isEditable && folderData.isAutoSaveEnabled == "True") {

                //Start a new Worker Thread for AutoSave functionality
                currentInstance.autoSaveWorker = new Worker(currentInstance.URLs.autoSaveWorker);
                console.log('Worker created');
            }
        }

        function initFolderLockWorker() {
            if (currentInstance.isEditable && currentInstance.isFolderLockEnable && !currentInstance.IsMasterList) {
                $(currentInstance.elementIDs.btnUnlockJQ).show();

                //Start a new Worker Thread for Folder Lock functionality
                currentInstance.folderLockWorker = new Worker(currentInstance.URLs.folderLockWorker);
                console.log('Folder Lock Worker created');
                currentInstance.folderLock.InitializeFolderLock();
            }
        }

        function setDocumentMenuVisibility(formInstanceData) {
            if (formInstanceData !== null && formInstanceData !== undefined) {
                //The Product generation option should not be available when the folder is in the Audit/Prod status i.e. when WFStateID > 3 and when form in in VIEW mode
                if (formInstanceData.FormInstance.FormDesignID == 2 || (this.folderData.WFStateName != WorkFlowState.FACETSTestAudit && this.folderData.WFStateName != WorkFlowState.FACETSPROD) || currentInstance.isEditable == false) {
                    $(currentInstance.elementIDs.btnJournalEntryJQ).parent('div').removeClass("separator");
                }
                else if (formInstanceData.FormInstance.FormDesignID == 3) {
                    $(currentInstance.elementIDs.btnJournalEntryJQ).parent('div').addClass("separator");
                }
            }
        }
        function SetValidateMenuVisibility(formInstanceData) {
            try {
                if (formInstanceData !== null && formInstanceData !== undefined) {
                    if (nonValidateFormDesignID.indexOf(formInstanceData.FormInstance.FormDesignID) != -1) {
                        // Hide Validate And Section Valid Menu button
                        $(currentInstance.elementIDs.btnValidateJQ).css('display', 'none');
                        $(currentInstance.elementIDs.btnValidateSectionJQ).css('display', 'none');
                    }
                    else {
                        $(currentInstance.elementIDs.btnValidateJQ).css('display', '');
                        if (folderData.FolderViewMode == FolderViewMode.DefaultView) {
                            $(currentInstance.elementIDs.btnValidateSectionJQ).css('display', '');
                        }
                        else { $(currentInstance.elementIDs.btnValidateSectionJQ).css('display', 'none'); }
                    }
                }
            } catch (e) {
            }
        }

       
        //sets the display mode for the folder
        function setDisplayMode() {          
            //except for the VersionHistory button, disable all the buttons,
            //since user can always view the VersionHistory
            if (currentInstance.IsMasterList) {
                if (folderData.versionType == VersionType.Retro && (folderData.folderVersionState == FolderVersionState.INPROGRESS_BLOCKED || folderData.folderVersionState == FolderVersionState.INPROGRESS)) {
                    $(currentInstance.elementIDs.retroNoteMLJQ).css("display", "block");
                }
                $(currentInstance.elementIDs.btnSaveFormDataJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnBottomSaveFormDataJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnBottomCreateNewVersion).addClass('disabled-button');
                $(currentInstance.elementIDs.btnBottomGenerateSOTFile).addClass('disabled-button');
                $(currentInstance.elementIDs.btnCreateFormJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnSOTAddJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnValidateAllJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnAssignFolderMember).addClass('disabled-button');
                $(currentInstance.elementIDs.btnPropertiesJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnDeleteFormInstance).addClass('disabled-button');
                //$(currentInstance.elementIDs.btnJournalEntry).css('display', 'none');
                //$(currentInstance.elementIDs.btnReplaceText).css('display', 'none');
                //$(currentInstance.elementIDs.btnExportToPDf).css('display', 'none');
                //$(currentInstance.elementIDs.btnViewFaxBackReport).css('display', 'none');
                //$(currentInstance.elementIDs.btnViewBenefitMatrixReport).css('display', 'none');


            }
            else if (isEditable) {
                checkFolderVersionClaims(currentInstance.elementIDs, claims);
            }

            else if (folderData.isLocked == "True") {
                checkFolderLockClaims(currentInstance.elementIDs, claims);
                $(currentInstance.elementIDs.btnCreateFormJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnSOTAddJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnSaveFormDataJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnBottomSaveFormDataJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnBottomCreateNewVersion).addClass('disabled-button');
                $(currentInstance.elementIDs.btnBottomGenerateSOTFile).addClass('disabled-button');
                $(currentInstance.elementIDs.btnSOTSaveJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnValidateJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnValidateAllJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnStatusUpdateJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnBaselineJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnNewVersionJQ).attr("disabled", "disabled");
                $(currentInstance.elementIDs.btnNewVersionMLJQ).attr("disabled", "disabled");
                $(currentInstance.elementIDs.btnRetroJQ).attr("disabled", "disabled");
                $(currentInstance.elementIDs.btnRetroMLJQ).attr("disabled", "disabled");
                 $(currentInstance.elementIDs.btnJournalEntryJQ).addClass('disabled-button');               
                $(currentInstance.elementIDs.btnDeleteFormInstance).addClass('disabled-button');
                $(currentInstance.elementIDs.btnAssignFolderMember).addClass('disabled-button');
                $(currentInstance.elementIDs.btnViewCollateral).addClass('disabled-button');
            }
            else {
                $(currentInstance.elementIDs.btnCreateFormJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnSOTAddJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnSaveFormDataJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnBottomSaveFormDataJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnBottomCreateNewVersion).addClass('disabled-button');
                $(currentInstance.elementIDs.btnBottomGenerateSOTFile).addClass('disabled-button');
                $(currentInstance.elementIDs.btnSOTSaveJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnValidateJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnValidateAllJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnStatusUpdateJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnBaselineJQ).addClass('disabled-button');
                $(currentInstance.elementIDs.btnNewVersionJQ).attr("disabled", "disabled");
                $(currentInstance.elementIDs.btnNewVersionMLJQ).attr("disabled", "disabled");
                $(currentInstance.elementIDs.btnRetroJQ).attr("disabled", "disabled");
                $(currentInstance.elementIDs.btnRetroMLJQ).attr("disabled", "disabled");
                $(currentInstance.elementIDs.btnJournalEntryJQ).addClass('disabled-button');            
                $(currentInstance.elementIDs.btnDeleteFormInstance).addClass('disabled-button');
                $(currentInstance.elementIDs.btnAssignFolderMember).addClass('disabled-button');
                $(currentInstance.elementIDs.btnViewCollateral).addClass('disabled-button');
            }

            //ANT-289, Assign Folder Member should be disable if folder is released
            if (folderData.isReleased == "True") {
                $(currentInstance.elementIDs.btnAssignFolderMember).addClass('disabled-button');
            }
        }

        
        function pdfGenerationMethod() {
            var currentInstance = this;

            var pdfDialog = new pdfGenerationDailog(currentInstance);

            return {
                showPDfDialog: function (folderVersionId) {
                    if (pdfDialog != undefined) {
                        pdfDialog.pdfTemplateDialog.showPDfDialog(folderVersionId);
                    }
                }
            }
        }

        return {
            load: function () {
                //set the display mode for the buttons
                setDisplayMode();
                if (currentInstance.IsMasterList == true) {
                    var url = currentInstance.URLs.forminstancelisturl.replace(/\{folderVersionId\}/g, folderVersionId).replace(/\{folderId\}/g, folderId);
                    var promise = ajaxWrapper.getJSON(url);
                    //register ajax success callback
                    promise.done(loadFolderTabs);
                    //Comment out the following line to show landing page
                    //register ajax failure callback
                    promise.fail(showError);
                    //create folder instance
                }
                else {
                    // Without SOTView To Classic View Navigaton for PBPViewImpact Grid
                    if (currentInstance.folderData.ViewFormInstanceID == 0 || currentInstance.folderData.ViewFormInstanceID == undefined
                        || currentInstance.folderData.ViewFormInstanceID == null || currentInstance.folderData.ViewFormInstanceID == "") {
                        loadFolderTabs(null);
                    }
                    else if (currentInstance.folderData.ViewFormInstanceID > 0) {
                        var url = currentInstance.URLs.getDefaultViewFormInstanceList;
                        var requestData = {};
                        var formInstanceIds = new Array();
                        var unlockDocumentSelections = new Array();
                        var unlockDocumentStatus = { FormInstanceId: parseInt(currentInstance.folderData.ViewFormInstanceID), IsOverrideDocument: false, IsDocumentLocked: "false" }
                        unlockDocumentSelections.push(unlockDocumentStatus);
                        formInstanceIds.push(parseInt(currentInstance.folderData.ViewFormInstanceID));
                        if (formInstanceIds.length != 0) {
                            requestData.tenantId = 1;
                            requestData.formInstanceIDs = formInstanceIds;
                            requestData.unlockDocumentSelections = unlockDocumentSelections;
                        }
                        // Clear currentInstance.folderData.ViewFormInstanceID as there is no use further after navigation
                        currentInstance.folderData.ViewFormInstanceID = 0;
                        var promise = ajaxWrapper.postAsyncJSONCustom(url, requestData);
                        //var promise = ajaxWrapper.postJSON(url, requestData);
                        promise.done(loadFolderTabs);
                        promise.fail(showError);
                    }
                }
                $(currentInstance.elementIDs.sectionMenuContainer).css('display', 'none');
                $(currentInstance.elementIDs.documentheadercontainer).css('display', 'none');
                createFormDialog.setFolder(currentInstance);
                // add click event for save button
                $(currentInstance.elementIDs.btnCreateFormJQ).click(function (e) {
                    var isPortfolio = false;
                    if (folderData.isPortfolio == "True") {
                        isPortfolio = true;
                    }
                    return createFormDialog.show(isPortfolio);
                });
                $(currentInstance.elementIDs.btnSOTAddJQ).click(function (e) {
                    var isPortfolio = false;
                    if (folderData.isPortfolio == "True") {
                        isPortfolio = true;
                    }
                    return createFormDialog.show(isPortfolio);
                });
                $(currentInstance.elementIDs.btnOpenFormJQ).click(function (e) {
                    return openDocumentDialog.show(folderVersionId, currentInstance);
                });
                $(currentInstance.elementIDs.btnSOTOpenJQ).click(function (e) {
                    return openDocumentDialog.show(folderVersionId, currentInstance);
                });
                $(currentInstance.elementIDs.btnJournalEntryJQ).click(function (e) {
                    currentInstance.formInstances[currentInstance.currentFormInstanceID].FormInstanceBuilder.journal.accessJournalEntry(e);
                });
            },
            loadFolderTabs: function (formInstances, isFormInstanceLodedFromOutSide) {
                loadFolderTabs(formInstances, isFormInstanceLodedFromOutSide);
            },
            loadFormInstance: function (formInstanceID, oldFormInstanceID, sibling) {
                loadActiveFormInstancce(formInstanceID, oldFormInstanceID, sibling);
            },
            loadFolderView: function (formDesignID, formInstanceID) {
                loadFolderView(formDesignID, formInstanceID)
            },
            enableButton: function () {
                currentInstance.enableButton();
            },
            hasValidationErrors: function () {
                return currentInstance.hasValidationErrors();
            },
            getFolderInstance: function () {
                return currentInstance;
            },
            checkJournalEntryIsOpen: function () {
                return currentInstance.checkJournalEntryIsOpen();
            },
            fillErrorGridData: function (allErrorGridDataList, currentFormInstanceID) {
                var formInstanceList = [];
                var errorGridDataList = "";
                if (currentFormInstanceID != undefined) {
                    errorGridDataList = allErrorGridDataList.filter(function (item) {
                        if (item.FormInstanceID == parseInt(currentFormInstanceID)) {
                            return item;
                        }
                    });
                }
                else {
                    errorGridDataList = allErrorGridDataList;
                }

                $.each(errorGridDataList, function () {
                    if (this.FormInstanceID != undefined) {
                        var formInstance = currentInstance.getFormInstanceBuilder(this.FormInstanceID);
                        if (formInstance != undefined) {
                            formInstance.errorGridData = this.ErrorList;
                            formInstance.validation.removeVisibleAndDisabledControls();
                        }
                        else {
                            // In case of View Validate forminstance will be undefined as we are keeping AnchorForminstanceIds(Not views forminstanceId) in folder's formInstances List, 
                            // and replacing forminstancebuilder object with anchor/views of folders formInstance list. 
                            if (folderData.FolderViewMode == FolderViewMode.DefaultView) {
                                formInstance = currentInstance.formInstances[currentInstance.currentFormInstanceID].FormInstanceBuilder;
                                formInstance.errorGridData = this.ErrorList;
                                formInstance.validation.removeVisibleAndDisabledControls();
                            }
                        }
                    }
                });
                $.each(errorGridDataList, function () {
                    if (this.ErrorList.length > 0 && currentInstance.formInstances[this.FormInstanceID] == undefined) {
                        formInstanceList.push(this.FormInstanceID);
                    }
                });
                if (formInstanceList.length > 0) {
                    var data = {
                        tenantId: 1,
                        formInstanceIDs: formInstanceList
                    };

                    //ajax call to add folder workflow states
                    //var promise = ajaxWrapper.postAsyncJSONCustom(currentInstance.URLs.getFormInstanceList, data);
                    //
                    //promise.done(function (list) {
                    //    if (list.length != 0) {
                    //        var errorDocumentNames = '';
                    //        $.each(list, function () {
                    //            errorDocumentNames = errorDocumentNames + this.FormDesignName.replace(/\@@/g, ' View: ') + '<br>'
                    //        });
                    //        messageDialog.show((Folder.folderValidationErrorDocumentsMsg).replace(/\{#documentList}/g, errorDocumentNames));
                    //
                    //    }
                    //});
                }
            },
            currentFormInstanceID: this.currentFormInstanceID
        }
    }

    folder.prototype.PopulateReportGenerationMenu = function () {
        var currentInstance = this;
        //$(currentInstance.elementIDs.documentMenuIcons).find('div[class="separator"]:eq(1)').remove();

        var collateralMenuItems = $(currentInstance.elementIDs.documentMenuIcons).find('div[class="separator collateralMenuItem"]');
        if (collateralMenuItems.length) {
            if (currentInstance.formInstances && currentInstance.formInstances[currentInstance.currentFormInstanceID]) {
                collateralMenuItems.each(function (i) {
                    $(this).css('display', 'none');
                });
            }
            else {
                collateralMenuItems.each(function (i) {
                    $(this).css('display', '');
                });
            }
        }
        else {
            if (currentInstance.formInstances && currentInstance.formInstances[currentInstance.currentFormInstanceID] && !currentInstance.iscollateralReportMenuItemLoaded) {
                var reportUrl = currentInstance.URLs.getCollateralTemplatesForReportGeneration.replace(/\{formInstanceId\}/g, currentInstance.currentFormInstanceID);
                var promise = ajaxWrapper.getJSON(reportUrl);
                var newMenuItems = '';
                promise.done(function (result) {
                    $.each(result, function (i, obj) {
                        var buttonID = 'btnGenerate-' + obj.ReportId + '-Report';

                        var newItem = '<div class="separator collateralReportMenuItem"><div title="Generate ' + obj.ReportName + '" class="button folderVersionButton" id="' + buttonID + '" data-="' + obj.ReportName + '" ><p><span class="folderversion-buttons-text">Generate ' + obj.ReportName + '</span></p></div></div';
                        $(currentInstance.elementIDs.documentMenuIcons).find('div[class="separator"]:eq(2)').after($(newItem));

                        //// creating click event for the buttons created above
                        $(document).on('click', '#' + buttonID, function () {
                            currentInstance.generateReportForCollateralTemplate(this, obj.ReportId, obj.ReportName);
                        });
                        //currentInstance.iscollateralReportMenuItemLoaded = true;
                    });
                });
                currentInstance.iscollateralReportMenuItemLoaded = true;
            }
        }
    }
   
    folder.prototype.enableButton = function () {        
        var currentInstance = this;
        if (currentInstance.IsMasterList == true) {
            $(currentInstance.elementIDs.btnJournalEntryJQ).css('display', 'none');
            $(currentInstance.elementIDs.btnReplaceTextJQ).removeClass('disabled-button');
            $(currentInstance.elementIDs.btnExportToPDf).css('display', 'none');
            $(currentInstance.elementIDs.btnViewFaxBackReport).css('display', 'none');
            $(currentInstance.elementIDs.btnViewBenefitMatrixReport).css('display', 'none');
        }
        else {
            $(currentInstance.elementIDs.btnMenuOptionsJQ).show();
            $(currentInstance.elementIDs.btnSaveFormDataJQ).show();
            $(currentInstance.elementIDs.btnSOTSaveJQ).show();
            $(currentInstance.elementIDs.btnBottomSaveFormDataJQ).show();
            $(currentInstance.elementIDs.btnBottomCreateNewVersion).show();
            $(currentInstance.elementIDs.btnBottomGenerateSOTFile).show();
            $(currentInstance.elementIDs.btnViewCollateral).removeClass('disabled-button');
            $(currentInstance.elementIDs.btnRealoadFormDataJQ).removeClass('disabled-button');
            $(currentInstance.elementIDs.btnValidateJQ).removeClass('disabled-button');
            $(currentInstance.elementIDs.btnReplaceTextJQ).removeClass('disabled-button');
            $(currentInstance.elementIDs.btnExportToPDf).removeClass('disabled-button');
            $(currentInstance.elementIDs.btnViewFaxBackReport).removeClass('disabled-button');
            $(currentInstance.elementIDs.btnViewBenefitMatrixReport).removeClass('disabled-button');
            $(currentInstance.elementIDs.btnCascadeML).removeClass('disabled-button');
            $(currentInstance.elementIDs.btnExitValidate).removeClass('disabled-button');
            $(currentInstance.elementIDs.btnShowAllProducts).removeClass('disabled-button');
            $(currentInstance.elementIDs.btnValidateSectionJQ).removeClass('disabled-button');
            $(currentInstance.elementIDs.collateralReportMenuItems).children().removeClass('disabled-button');
        }
    }

    folder.prototype.disableButton = function () {
        
        var currentInstance = this;
        //if (parseInt(folderData.RoleId) != parseInt(Role.WFSpecialist)) {
        if (currentInstance.IsMasterList == true) {
            $(currentInstance.elementIDs.btnJournalEntryJQ).css('display', 'none');
            $(currentInstance.elementIDs.btnReplaceTextJQ).addClass('disabled-button');
            $(currentInstance.elementIDs.btnExportToPDf).css('display', 'none');
            $(currentInstance.elementIDs.btnViewFaxBackReport).css('display', 'none');
            $(currentInstance.elementIDs.btnViewBenefitMatrixReport).css('display', 'none');
        }
        else {
            $(currentInstance.elementIDs.btnMenuOptionsJQ).hide();
            //}
            $(currentInstance.elementIDs.btnSaveFormDataJQ).hide();
            $(currentInstance.elementIDs.btnSOTSaveJQ).hide();
            $(currentInstance.elementIDs.btnBottomSaveFormDataJQ).hide();
            $(currentInstance.elementIDs.btnBottomCreateNewVersion).hide();
            $(currentInstance.elementIDs.btnBottomGenerateSOTFile).hide();
            $(currentInstance.elementIDs.btnViewCollateral).addClass('disabled-button');
            $(currentInstance.elementIDs.btnRealoadFormDataJQ).addClass('disabled-button');
            $(currentInstance.elementIDs.btnValidateJQ).addClass('disabled-button');
            $(currentInstance.elementIDs.btnJournalEntryJQ).addClass('disabled-button');            
            $(currentInstance.elementIDs.btnReplaceTextJQ).addClass('disabled-button');            
            $(currentInstance.elementIDs.btnExportToPDf).addClass('disabled-button');            
            $(currentInstance.elementIDs.btnViewFaxBackReport).addClass('disabled-button');            
            $(currentInstance.elementIDs.btnViewBenefitMatrixReport).addClass('disabled-button');            
            $(currentInstance.elementIDs.btnCascadeML).addClass('disabled-button');
            $(currentInstance.elementIDs.btnExitValidate).addClass('disabled-button');
            $(currentInstance.elementIDs.btnShowAllProducts).addClass('disabled-button');
            $(currentInstance.elementIDs.btnValidateSectionJQ).addClass('disabled-button');
            $(currentInstance.elementIDs.collateralReportMenuItems).children().addClass('disabled-button');
        }
    }

    folder.prototype.getFormInstanceBuilder = function (formInstanceID) {

        var formInstanceBuilder = undefined;
        if (this.formInstances[formInstanceID] != undefined) {
            formInstanceBuilder = this.formInstances[formInstanceID].FormInstanceBuilder;
        }
        return formInstanceBuilder;
    }

    folder.prototype.addFormInstance = function (formInstance) {
        var currentInstance = this;
        if (this.IsMasterList == false) {
            this.formInstances[formInstance.FormInstanceID] = { 'ID': formInstance.FormInstanceID, 'FormInstanceBuilder': undefined, 'FormInstance': formInstance, 'AnchorDocumentID': formInstance.AnchorDocumentID };
            this.openDocuments[formInstance.AnchorDocumentID] = { 'AnchorDocumentID': formInstance.AnchorDocumentID, 'SelectedViewID': formInstance.FormInstanceID, 'Status': OpenDocumentStatus.Open };
            currentInstance.viewManager.addTab(formInstance);
            currentInstance.viewManager.setActiveTab();
            //this.addTab(formInstance);

            //this.tabs.tabs({ active: -1 });

            //TODO: Write code to activate current form
            //TODO: This code needs to be replaced by code to activate the currently added tab
            this.enableButton();
        }
        else {
            $("#masterListInstanceMenuContainer").show();
            $('#masterListInstanceMenu').append($("<option></option>")
                                        .attr("value", formInstance.FormInstanceID)
                                        .text(formInstance.FormDesignName));
            $('#masterListInstanceMenu').find('option[value="' + formInstance.FormInstanceID + '"]').attr('selected', 'selected');
            $('#masterListInstanceMenu').val(formInstance.FormInstanceID).change();
        }
    }

    folder.prototype.deleteFormInstance = function () {
        var currentInstance = this;
        var formInstance = currentInstance.formInstances[currentInstance.currentFormInstanceID].FormInstance;
        //url to check  document to be deleted is used as data source for other documents in the folder
        var url = currentInstance.URLs.isDataSource.replace(/\{formDesignID\}/g, formInstance.FormDesignID)
                                                   .replace(/\{formDesignVersionID\}/g, formInstance.FormDesignVersionID);
        var promise = ajaxWrapper.getJSON(url);
        var message = "";
        //register ajax success callback
        promise.done(function (isDataSource) {
            if (isDataSource) {
                message = Common.useDataSourceConfirmationMsg;
            }
            message = message + FolderVersion.deleteConfirmationMsg.replace(/\{#documnentName}/g, formInstance.FormDesignName);

            formInstanceDeleteDialog.show(message, function () {
                formInstanceDeleteDialog.hide();
                var url = currentInstance.URLs.deleteFormInstance.replace(/\{folderId\}/g, currentInstance.folderId)
                                                                 .replace(/\{tenantId\}/g, formInstance.TenantID)
                                                                 .replace(/\{formInstanceId\}/g, formInstance.FormInstanceID)
                                                                 .replace(/\{folderVersionId\}/g, formInstance.FolderVersionID);
                var promise = ajaxWrapper.getJSON(url);
                //register ajax success callback
                promise.done(function (xhr) {
                    if (xhr.Result === ServiceResult.SUCCESS) {
                        window.location.reload();
                    }
                    else {
                        messageDialog.show(Common.errorMsg);
                    }
                });
                //register ajax failure callback
                promise.fail(showError);
            });
        });
        //register ajax failure callback
        promise.fail(showError);
    }

    folder.prototype.ShowAllProducts = function () {
        //alert('Show Related products Popup');
        var currentInstance = this;
        var formInstance = currentInstance.formInstances[currentInstance.currentFormInstanceID].FormInstance;
        var formInstanceId = formInstance.FormInstanceID;
        var folderVersionId = formInstance.FolderVersionID;
        var formDesignVersionId = formInstance.FormDesignVersionID;
        //Get the list fo related products
        $(currentInstance.elementIDs.showAllProductsDialog).dialog({
            autoOpen: false,
            height: 300,
            width: 800,
            modal: true,
            close: function () {
            }
        });
        $(currentInstance.elementIDs.showAllProductsDialog).dialog('option', 'title', "All Products");
        $(currentInstance.elementIDs.showAllProductsDialog).dialog("open");

        var colArray = null;
        colArray = ['FormInstanceID', 'Product Name', 'Product ID', 'Product Type'];

        var colModel = [];
        colModel.push({ key: true, hidden: true, name: 'FormInstanceID', index: 'FormInstanceID', editable: false });
        colModel.push({ key: false, name: 'ProductName', index: 'ProductName', editable: false });
        colModel.push({ key: false, name: 'ProductID', index: 'ProductID', editable: false });
        colModel.push({ key: false, name: 'ProductType', index: 'ProductType', editable: false });

        $(currentInstance.elementIDs.showAllProductsGrid).jqGrid('GridUnload');

        var url = currentInstance.URLs.ShowAllProducts.replace(/\{formInstanceId\}/g, currentInstance.currentFormInstanceID);
        url = url.replace(/\{formDesignVersionId\}/g, formInstance.FormDesignVersionID) + "&formName=" +  formInstance.FormDesignName;
        $(currentInstance.elementIDs.showAllProductsGrid).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'All Products List',
            height: '200',
            rowheader: true,
            loadonce: true,
            autowidth: true,
            gridview: true,
            viewrecords: true,
            altRows: true,
            altclass: 'alternate',
            ignoreCase: true,
            gridComplete: function () {
                $(currentInstance.elementIDs.showAllProductsDialog).dialog({
                    position: {
                        my: 'center',
                        at: 'center'
                    },
                });

            },
        });
    }

    folder.prototype.scrollToTop = function () {
        $('html, body').animate({ scrollTop: 0 }, 1000);
    }

    folder.prototype.validate = function (flag, formInstanceID) {
        try {
            var currentInstance = this;
            var isAllFormsLoaded = true;
            var hasChanges = false;

            currentInstance.formInstances.filter(function (form) {
                if (form.FormInstanceBuilder === undefined) {
                    isAllFormsLoaded = false;
                }
                else if (form.FormInstanceBuilder.hasChanges === true) {
                    hasChanges = true;
                }
            });

            //check if all forms are loaded
            // if (isAllFormsLoaded) {
            currentInstance.documentValidation(currentInstance.formInstances[formInstanceID].FormInstanceBuilder, formInstanceID, flag);
            //}
            //else {
            //    //ask user to load all the forms
            // messageDialog.show(FolderVersion.loadDocumentsBeforeValidatingMsg);
            //}
        }
        catch (e) {
        }
    }

    folder.prototype.formValidation = function (allFormInstance, formInstanceID, onlyCurrentSection) {
        var currentInstance = this;
        currentInstance.formInstances[formInstanceID].FormInstanceBuilder.validation.validateFormInstance(allFormInstance, undefined, onlyCurrentSection);
    }

    folder.prototype.previewAllInstances = function (previewurl) {
        var currentInstance = this;
        window.open(previewurl);
    }

    folder.prototype.validateCurrentForm = function (formInstanceID, onlyCurrentSection) {
        var currentInstance = this;
        var flag = false;
        var hasChanges = false;

        currentInstance.documentValidation(currentInstance.formInstances[formInstanceID].FormInstanceBuilder, formInstanceID, flag, undefined, onlyCurrentSection);
    }

    folder.prototype.documentValidation = function (currentInstanceData, formInstanceID, flag, instanceCount, onlyCurrentSection) {
        var currentInstance = this;
        var totalInstanceCount = 0;
        currentInstance.formInstances.filter(function (form) {
            totalInstanceCount++;
        });

        var fullpath = [];
        for (var idx = 0; idx < currentInstanceData.repeaterBuilders.length; idx++) {
            if (currentInstanceData.repeaterBuilders[idx].design.LoadFromServer == true) {
                fullpath.push(currentInstanceData.repeaterBuilders[idx].design.FullName);
            }
        }
        // if load from server is true then checks duplicate records
        if (fullpath.length > 0) {
            var getFormInstanceRepeaterDesignData = "/FormInstance/GetCheckDuplicationList?tenantId={tenantId}&formInstanceId={formInstanceId}&formDesignVersionId={formDesignVersionId}&repeaterFullNameList={repeaterFullNameList}";
            var checkDuplicateUrl = getFormInstanceRepeaterDesignData.replace(/\{tenantId\}/g, 1)
                                .replace(/\{formInstanceId\}/g, currentInstanceData.formInstanceId)
                                .replace(/\{formDesignVersionId\}/g, currentInstanceData.formDesignVersionId)
                                .replace(/\{repeaterFullNameList\}/g, JSON.stringify(fullpath));
            var promise = ajaxWrapper.getJSON(checkDuplicateUrl);
            promise.done(function (duplicateData) {
                for (var idx = 0; idx < currentInstanceData.repeaterBuilders.length; idx++) {
                    if (currentInstanceData.repeaterBuilders[idx].design.LoadFromServer == true) {
                        //code for splice repeater data from current instance
                        var fullpath = currentInstanceData.repeaterBuilders[idx].design.FullName.split('.');
                        var fullName = currentInstanceData.repeaterBuilders[idx].design.FullName;

                        var formData;
                        for (var index = 0; index < fullpath.length; index++) {
                            if (index == 0) {
                                formData = currentInstanceData.formData[fullpath[index]];
                            }
                            else if (index == (fullpath.length - 1)) {
                                var repeaterInstanceData = formData[fullpath[index]];
                                if (!$.isEmptyObject(duplicateData) && duplicateData[fullName] && repeaterInstanceData.length > 0) {
                                    var duplicateData = duplicateData[fullName];
                                    $.each(duplicateData, function (idx, rowData) {
                                        var duplicateRow = repeaterInstanceData.filter(function (row) {
                                            return parseInt(row.RowIDProperty) == parseInt(rowData.RowIDProperty);
                                        });
                                        if (duplicateRow.length == 0) {
                                            formData[fullpath[index]].push(rowData);
                                        }
                                    });
                                }
                            }
                        }
                    }
                }
                //Form Validation
                if (flag == true || flag == false) {
                    currentInstance.formValidation(flag, formInstanceID, onlyCurrentSection);
                }
            });
            //register ajax failure callback
            promise.fail(showError);
        }
        else {
            //Form Validation
            if (flag == true || flag == false) {
                currentInstance.formValidation(flag, formInstanceID, onlyCurrentSection);
            }
        }
    }

    folder.prototype.checkAllFormsAreLoaded = function () {
        try {
            var currentInstance = this;
            var isAllFormsLoaded = true;
            currentInstance.formInstances.filter(function (form) {
                if (form.FormInstanceBuilder === undefined) {
                    isAllFormsLoaded = false;
                }
            });
            return isAllFormsLoaded;
        }
        catch (e) {
        }
    }

    folder.prototype.getNonLoadedForms = function () {
        try {
            var currentInstance = this;
            var nonLoadedFormList = new Array();

            currentInstance.formInstances.filter(function (form) {
                if (form.FormInstanceBuilder === undefined) {
                    nonLoadedFormList.push(form.FormInstance);
                }
            });
            return nonLoadedFormList;
        } catch (e) {

        }
        return null;
    }

    folder.prototype.loadAllForms = function () {
        try {
            var currentInstance = this;
            var notLoadedFormList = currentInstance.getNonLoadedForms();
            if (notLoadedFormList.length > 0) {
                for (var i = 0; i < notLoadedFormList.length; i++) {
                    var effDate = new Date(notLoadedFormList.FormInstance.EffectiveDate);
                    var effectiveDate = effDate.getMonth() + 1 + "/" + effDate.getDate() + "/" + effDate.getYear();

                    var params = {
                        TenantId: notLoadedFormList[i].TenantID,
                        AccountId: currentInstance.accountId,
                        FolderVersionId: currentInstance.folderVersionId,
                        FolderId: currentInstance.folderId,
                        FormInstanceId: notLoadedFormList[i].FormInstanceID,
                        FormDesignVersionId: notLoadedFormList[i].FormDesignVersionID,
                        FormDesignName: notLoadedFormList[i].FormDesignName,
                        AutoSaveWorkder: undefined,
                        AutoSaveTimer: undefined,
                        IsReleased: currentInstance.isReleased,
                        FolderData: undefined,
                        FolderType: folderType,
                        AnchorFormInstanceId: undefined,
                        EffectiveDate: effectiveDate,
                        currentUserId: currentInstance.currentUserId,
                        currentUserName: currentInstance.currentUserName
                    }
                    var formInstanceBuilderObj = currentInstance.formInstanceFactory.createFormInstance(currentInstance.folderViewMode, params);
                    //var formInstanceBuilderObj = new formInstanceBuilder(notLoadedFormList[i].TenantID, currentInstance.accountId, currentInstance.folderVersionId, currentInstance.folderId, notLoadedFormList[i].FormInstanceID, notLoadedFormList[i].FormDesignVersionID, notLoadedFormList[i].FormDesignName, undefined, undefined, currentInstance.isReleased, undefined, folderType);
                    formInstanceBuilderObj.loadFormInstanceDesignData();
                    //add the form instance builder object to the form instances array for future use.
                    currentInstance.formInstances[notLoadedFormList[i].FormInstanceID].FormInstanceBuilder = formInstanceBuilderObj;
                }
            }
        } catch (e) {

        }
    }

    folder.prototype.checkForAccessPermissions = function (formInstance) {
        var isAccessible = false;
        $.each(formInstanceClaims, function (i, claim) {
            if (claim.ResourceID == formInstance.FormDesignVersionID) {
                isAccessible = true;
                return false;
            }
        });

        return isAccessible;
    }


    folder.prototype.hasValidationErrors = function () {
        var hasValidationErrors = false;

        try {
            var currentInstance = this;
            var isAllFormsLoaded = true;
            if (currentInstance.formInstances == undefined)
                isAllFormsLoaded = false;
            currentInstance.formInstances.filter(function (form) {
                if (form.FormInstanceBuilder === undefined) {
                    isAllFormsLoaded = false;
                }
            });
            //check if all forms are loaded
            if (isAllFormsLoaded) {
                try {
                    uiblocker.showPleaseWait(FolderVersion.validatingMsg);

                    var firstFormInstanceID = undefined;
                    //run validation on each of the form
                    for (var formInstance in currentInstance.formInstances) {
                        currentInstance.formInstances[formInstance].FormInstanceBuilder.validation.validateFormInstance();
                    }
                    //show first section of currently loaded form & show validated controls & load validation grid for currently loaded form
                    currentInstance.formInstances[currentInstance.currentFormInstanceID].FormInstanceBuilder.menu.selectFirstSection();
                    currentInstance.formInstances[currentInstance.currentFormInstanceID].FormInstanceBuilder.formValidationManager.showValidatedControls();
                    currentInstance.formInstances[currentInstance.currentFormInstanceID].FormInstanceBuilder.validation.loadValidationErrorGrid();

                    //loop through each of the form to check if there are validation errors.
                    for (var formInstance in currentInstance.formInstances) {
                        hasValidationErrors = currentInstance.formInstances[formInstance].FormInstanceBuilder.form.hasValidationErrors();
                        if (hasValidationErrors)
                            break;
                    }

                    if (hasValidationErrors) {
                        //show message to user that there are validation errors in forms
                        messageDialog.show(Folder.folderHasValidationError);
                    }

                    uiblocker.hidePleaseWait();
                }
                catch (e) {
                    hasValidationErrors = true;
                    messageDialog.show(Common.errorMsg);
                }
            }
            else {
                //ask user to load all the forms
                messageDialog.show(Folder.loadDocumentsBeforeReleasingMsg);
                hasValidationErrors = true;
            }
        }
        catch (e) {
        }

        return hasValidationErrors;
    }

    folder.prototype.checkOpenJournals = function () {
        var isOpenJournalEntry = false;

        try {
            var currentInstance = this;

            currentInstance.formInstances[currentInstance.currentFormInstanceID].FormInstanceBuilder.journal.InitializeOpenJournals();

            for (var formInstance in currentInstance.formInstances) {
                if (isOpenJournalEntry = currentInstance.formInstances[formInstance].FormInstanceBuilder != undefined) {
                    isOpenJournalEntry = currentInstance.formInstances[formInstance].FormInstanceBuilder.hasOpenJournals;
                    if (isOpenJournalEntry) {
                        break;
                    }
                }
            }

            if (isOpenJournalEntry) {
                messageDialog.show(JournalEntry.checkJournalEntryIsOpen);
            }

        } catch (e) {
            isOpenJournalEntry = true;
            messageDialog.show(Common.errorMsg);
        }
        return isOpenJournalEntry;
    }

    folder.prototype.overrideFolderLock = function (releaseLockUrl) {
        var currentInstance = this;
        if (currentInstance) {
            var message = "";
            message = message + FolderLock.releaseFolderLockMsg
            confirmDialog.show(message, function () {
                confirmDialog.hide();

                releaseLockUrl = releaseLockUrl.replace(/\{folderId\}/g, currentInstance.folderId);
                var promise = ajaxWrapper.getJSON(releaseLockUrl);
                //register ajax success callback
                promise.done(function (xhr) {

                    if (xhr.Result === ServiceResult.SUCCESS) {
                        window.location.reload();
                    }
                    else if (xhr.Result === ServiceResult.FAILURE) {
                        //messageDialog.show(xhr.Items[0].Messages);
                        window.location.reload();
                    }
                });
                //register ajax failure callback
                promise.fail(showError);
            });
        }

    }

    folder.prototype.releaseFolderLockOnNavigate = function (releaseLockUrl) {
        var currentInstance = this;
        if (currentInstance) {
            try {
                releaseLockUrl = releaseLockUrl.replace(/\{folderId\}/g, currentInstance.folderId).replace("isNavigate=false", "isNavigate='true'");
                var promise = ajaxWrapper.getJSONAsync(releaseLockUrl);
                promise.done(function (xhr) {
                    if (xhr.Result === ServiceResult.SUCCESS) {
                        console.log("delete Success..");
                    }
                    else if (xhr.Result === ServiceResult.FAILURE) {
                        console.log("entry not in Folder Lock..");
                        //folderLockOverridenDialog.hide();
                        //window.location.reload();
                    }
                });
                //promise.fail(showError);
            } catch (ex) {
                console.log(ex);
            }
            //register ajax failure callback

        }
    }

    folder.prototype.folderLockMethods = function () {
        var currentInstance = this;
        //Folder Lock checked at a specific duration automatically.
        setFolderOverrideCheckTimeout = function () {
            if (currentInstance.isFolderLockEnable) {

                if (currentInstance.checkFolderLockOverrideTimer != undefined) {
                    currentInstance.folderLock.clearFolderLockTimeOut();
                }
                currentInstance.checkFolderLockOverrideTimer = setTimeout(function () {
                    //get data
                    currentInstance.folderLock.postMessageAndGetData();
                }, 120000);
            }
        }

        return {

            InitializeFolderLock: function () {
                try {
                    if (typeof (Worker) !== "undefined" && currentInstance.folderLockWorker != undefined) {
                        currentInstance.folderLockWorker.onmessage = function (e) {

                            if (e.data != undefined) {
                                var result = JSON.parse(e.data);
                                if (result == "true") {
                                    currentInstance.hasLockOverriden = true;
                                }
                                else if (result == "false") {
                                    currentInstance.hasLockOverriden = false;
                                    currentInstance.folderLock.showFolderLockAlert();
                                    setFolderOverrideCheckTimeout();
                                }
                                else if (result == "ODMInProgress") {
                                    currentInstance.hasLockOverriden = true;
                                    messageDialog.show(FolderLock.odmlockMsg);
                                }
                            }
                        };
                        currentInstance.folderLockWorker.onerror = function (err) {
                            console.log('Worker is suffering!', err);
                        }

                        setFolderOverrideCheckTimeout();
                    }
                    else {
                        console.log("Sorry, your browser does not support WEB WORKERS..For Folder LOCK");
                    }
                } catch (ex) {
                    console.log(ex);
                }
            },

            postMessageAndGetData: function () {
                if (currentInstance.isFolderLockEnable) {
                    if (currentInstance != null) {
                        var folderData = {
                            folderId: currentInstance.folderId,
                            folderVersionId: currentInstance.folderVersionId,
                        }
                        currentInstance.folderLockWorker.postMessage({
                            url: currentInstance.URLs.getFolderLockOverridenStatus,
                            folderId: currentInstance.folderId,
                            folderVersionId: currentInstance.folderVersionId,
                            saveData: JSON.stringify(folderData)
                        });
                    }
                    else if (currentInstance != null) {
                        setFolderOverrideCheckTimeout();
                    }
                    currentInstance.checkFolderLockOverrideTimer = setTimeout(function () {
                        currentInstance.folderLock.postMessageAndGetData();
                    }, 120000);
                }
            },

            showFolderLockAlert: function () {
                //$(currentInstance.elementIDs.folderLockAlertJQ).show();
                folderLockOverridenDialog.show(FolderLock.unlockedFolderVersionMsg, function (e) {
                    if (e == true) {
                        window.location.reload();
                    } else {
                        console.log("Page does not refresh...");
                    }
                    folderLockOverridenDialog.hide(function () {
                        $(currentInstance.elementIDs.folderLockAlertJQ).hide();
                        window.location.reload();
                    });
                });
            },

            clearFolderLockTimeOut: function () {
                clearTimeout(currentInstance.checkFolderLockOverrideTimer);
            },
        }
    }

    folder.prototype.checkUserIsTeamManager = function () {
        var currentInstance = this;
        var promise = ajaxWrapper.getJSON(currentInstance.URLs.checkUserIsTeamManager);

        //register ajax success callback
        promise.done(function (result) {
            if (!result) {
                $(currentInstance.elementIDs.btnAssignFolderMember).closest('.separator').hide();
            }
        });

        //register ajax failure callback
        promise.fail(showError);
    }

    folder.prototype.loadAllFormsStatus = function (folderVersionId) {

        var currentInstance = this;
        var url = currentInstance.URLs.getAllFormsStatus.replace(/\{folderVersionId\}/g, folderVersionId).replace(/\{folderType\}/g, folderData.folderType);

        var promise = ajaxWrapper.getJSON(url);

        //register ajax success callback
        promise.done(function (list) {
            var result = list;
            currentInstance.allProductStatusInFolder = list;
            currentInstance.applyPermissionToStatusUpdate();
        });

        //register ajax failure callback
        promise.fail(showError);
    }

    folder.prototype.applyPermissionToStatusUpdate = function () {
        var data = this.allProductStatusInFolder;
        var disableButton = false;
        var isfolderInTranslationcompleted = this.isfolderInTranslation;
        if (this.checkForStatusUpdateDisableButton()) {
            $(this.elementIDs.btnStatusUpdateJQ).addClass('document-disabled-button');
            $(this.elementIDs.btnBaselineJQ).addClass('document-disabled-button');
            $(this.elementIDs.btnQueueFacetsTranslationJQ).addClass('document-disabled-button');
            this.isfolderInTranslation = true;
        }
        else {
            $(this.elementIDs.btnStatusUpdateJQ).removeClass('document-disabled-button');
            //$(this.elementIDs.btnBaselineJQ).removeClass('document-disabled-button');
            //$(this.elementIDs.btnBaselineJQ).removeClass('disabled-button');
            $(this.elementIDs.btnQueueFacetsTranslationJQ).removeClass('document-disabled-button');
            this.isfolderInTranslation = false;
        }

        //ANT-3, here if the logged in user is TPA Analyst then the folder workflow state is BPDPending Approval then Status update menu item will be disabled
        //if (folderData.WFStateName == WorkFlowState.BPDPendingApproval && folderData.RoleId == Role.TPAAnalyst) {
        //    $(this.elementIDs.btnStatusUpdateJQ).addClass('document-disabled-button');
        //}
        if (isfolderInTranslationcompleted == true && this.isfolderInTranslation == false) {

            var url = this.URLs.folderVersionIndex.replace(/\{folderVersionId\}/g, this.folderVersionId).replace(/\{folderId\}/g, folderData.folderId).replace(/\{mode\}/g, true);
            window.location.href = url;
        }
    }

    folder.prototype.checkForStatusUpdateDisableButton = function () {
        var disableButton = false;
        for (var formInstance in this.formInstances) {
            var form = this.formInstances[formInstance];
            if (this.allProductStatusInFolder[form.ID] != undefined && this.allProductStatusInFolder[form.ID] != null) {
                if (this.allProductStatusInFolder[form.ID].IsFolderVersionReleased != undefined && this.allProductStatusInFolder[form.ID].IsFolderVersionReleased ||
                this.allProductStatusInFolder[form.ID].IsFolderVersionBaselined != undefined && this.allProductStatusInFolder[form.ID].IsFolderVersionBaselined ||
                this.allProductStatusInFolder[form.ID].IsProductInTranslation != undefined && this.allProductStatusInFolder[form.ID].IsProductInTranslation ||
                this.allProductStatusInFolder[form.ID].IsProductInTransmission != undefined && this.allProductStatusInFolder[form.ID].IsProductInTransmission) {
                    disableButton = true;
                    return disableButton;
                }
            }
        }
        return disableButton;
    }

    folder.prototype.releaseFolder = function (updateWorkflowData) {
        var totalInstanceCount = 0;
        var hasChanges = false;
        var currentInstance = this;

        var currentFormInstaceBuilder = currentInstance.formInstances[currentInstance.currentFormInstanceID].FormInstanceBuilder;

        var callbackMetaDeta = {
            callback: folderManager.getInstance().getFolderInstance().queueProductForTransmission,
            args: [currentFormInstaceBuilder, currentInstance, updateWorkflowData]
        };

        //check form Validation & duplication
        currentFormInstaceBuilder.validation.validateFormInstance(true, callbackMetaDeta);

        //uiblocker.showPleaseWait();
        if (currentInstance.checkOpenJournals()) {
            //uiblocker.hidePleaseWait();
            return;
        }
    }

    folder.prototype.showReleasePopup = function (args, hasValidationErrors) {
        var formInstanceBuilder = args[0];
        var currentInstance = args[1];

        if (hasValidationErrors) {
            messageDialog.show(Folder.folderHasValidationError);
        } else {
            currentFormInstaceBuilder.form.saveFormInstanceData();
            folderVersionBaseline.show(false, folderData.currentUserName);
        }
    }

    folder.prototype.isAllDocumentsLoaded = function () {
        var currentInstance = this;
        var isAllFormsLoaded = true;
        for (var formInstanceId in currentInstance.formInstances) {
            var formInstance = currentInstance.formInstances[formInstanceId];
            if (formInstance.FormInstanceBuilder === undefined) {
                isAllFormsLoaded = false;
                break;
            }
        }
        return isAllFormsLoaded;
    }

    folder.prototype.checkForViewAccessPermissions = function (formInstance) {
        var isAccessible = false;
        if (formInstance) {
            $.each(formInstanceClaims, function (i, claim) {
                if (claim.ResourceID == formInstance.FormDesignID && claim.RoleID == folderData.RoleId) {
                    isAccessible = true;
                    return false;
                }
            });
        }

        return isAccessible;
    }
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    function isSaveFormInstance(currentInstance, formInstanceID, flag) {
        var isSave = undefined;
        var count = 0;
        var totalInstance = 0;
        var saveCount = 0;
        if (flag == false) {
            totalInstance++;
            if (currentInstance.formInstances[formInstanceID].FormInstanceBuilder.hasChanges == true)
                count++;
            else if (currentInstance.formInstances[formInstanceID].FormInstanceBuilder.hasChanges == false)
                saveCount++;
        }
        else {
            for (var formInstance in currentInstance.formInstances) {
                totalInstance++;
                if (currentInstance.formInstances[formInstance].FormInstanceBuilder.hasChanges == true)
                    count++;
                else if (currentInstance.formInstances[formInstance].FormInstanceBuilder.hasChanges == false)
                    saveCount++;
            }
        }
        if (totalInstance == count)
            isSave = false;
        else if (totalInstance == saveCount) {
            isSave = true;
        }
        return isSave;
    }

    function getFolderInstanceLength() {

    }


    function queueProduct(currentInstance, TemplateReportID, TemplateReportName) {
        var currentFormInstaceBuilder = currentInstance.formInstances[currentInstance.currentFormInstanceID].FormInstanceBuilder;

        //blank accountName is sent to controller and its retrieved from DB in SP before inserting in table
        var url = currentInstance.URLs.queueCollateral.replace(/\{accountID\}/, currentInstance.accountId).replace(/\{accountName\}/, "").replace(/\{folderID\}/, currentFormInstaceBuilder.folderId)
                                                 .replace(/\{folderName\}/, "").replace(/\{formInstanceIDs\}/, currentInstance.currentFormInstanceID).replace(/\{folderVersionID\}/, currentFormInstaceBuilder.folderData.folderVersionId)
                                                 .replace(/\{folderVersionNumbers\}/, currentFormInstaceBuilder.folderData.versionNumber).replace(/\{reportTemplateID\}/, TemplateReportID)
                                                 .replace(/\{productIds\}/, currentFormInstaceBuilder.formName).replace(/\{folderVersionEffDt\}/, currentFormInstaceBuilder.folderData.effectiveDate).replace(/\{runDate\}/, null).replace(/\{ReportName\}/, TemplateReportName);

        var promise = ajaxWrapper.getJSON(url);
        promise.done(function (xhr) {
            if (xhr.Result === ServiceResult.SUCCESS)
                messageDialog.show(CollateralQueue.enQueuecollateral);
            else
                messageDialog.show(CollateralQueue.collateralQueueErrorMsg);
        });
    }

    function downloadQueuedProductReport(currentInstance, formInstanceId, collateralProcessQueue1Up) {
        $(currentInstance.elementIDs.QueuedCollateralDialog).dialog({
            autoOpen: false,
            height: 300,
            width: 950,
            modal: true,
            close: function () {  // When dialog is closed then dispose autorefresh of grid so that it wont keep executing at backround.
                timer = 60;
                clearInterval(refreshInterval);
            }
        });

        $("#spnTimer").text(timer + " seconds.");
        $(currentInstance.elementIDs.QueuedCollateralDialog).dialog('option', 'title', "Queued Collateral");
        $(currentInstance.elementIDs.QueuedCollateralDialog).dialog("open");


        var colArray = null;
        colArray = ['Id', 'FormInstanceID', 'Collateral Name', 'Product Name', 'Account Name', 'Folder Name', 'Version Number', 'Status', 'Processed Date', 'Download', 'Download', 'TemplateReportVersionID'];// 'Product Json','Consortium',

        var colModel = [];
        colModel.push({ key: true, hidden: true, name: 'CollateralProcessQueue1Up', index: 'CollateralProcessQueue1Up', align: 'right', editable: false });
        colModel.push({ key: false, hidden: true, name: 'FormInstanceID', index: 'FormInstanceID', align: 'right', editable: false });
        colModel.push({ key: false, name: 'CollateralName', index: 'CollateralName', editable: false, width: '120' });
        colModel.push({ key: false, name: 'ProductID', index: 'ProductID', editable: false, width: '150' });
        colModel.push({ key: false, name: 'AccountName', index: 'AccountName', editable: true, edittype: 'select', align: 'left', width: '150' });
        colModel.push({ key: false, name: 'FolderName', index: 'FolderName', editable: true, edittype: 'select', align: 'left', width: '200' });
        colModel.push({ key: false, name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: true, edittype: 'select', align: 'left', width: '130' });
        colModel.push({ key: false, name: 'Status', index: 'Status', editable: false, align: 'left', width: '100' });
        colModel.push({ key: false, name: 'ProcessedDate', index: 'ProcessedDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' });
        colModel.push({ key: false, name: 'CollateralProcessQueue1Up', index: 'CollateralProcessQueue1Up', search: false, editable: false, width: '80', formatter: downloadPDFFileFormmater });
        colModel.push({ key: false, name: 'CollateralProcessQueue1Up', index: 'CollateralProcessQueue1Up', search: false, editable: false, width: '90', formatter: downloadWordFileFormmater });
        colModel.push({ key: false, name: 'TemplateReportVersionID', index: 'TemplateReportVersionID', hidden: true, search: false });

        //clean up the grid first - only table element remains after this
        $(currentInstance.elementIDs.queuedCollateralsGridJQ).jqGrid('GridUnload');

        $(currentInstance.elementIDs.queuedCollateralsGridJQ).jqGrid({
            url: currentInstance.URLs.getQueuedCollateralsList,
            datatype: 'json',
            postData: {
                filters: '{"groupOp":"AND","rules":[' +
                        '{"field":"FormInstanceID","op":"eq","data":"' + formInstanceId + '"}]}' // Apply filter while loading grid
            },
            mtype: 'POST',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Collaterals Queued',
            height: '200',
            headertitles: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortname: 'ProcessedDate',
            sortorder: 'desc',
            gridComplete: function () {
                $(".DownloadQueuedReport").on('click', function (e) {
                    var parameters = $(this).attr("data-Id").split('ö');
                    var processQueue1Up = parameters[0];
                    var reportName = parameters[1];
                    var templateReportVersionID = parameters[2];
                    var fileFormat = parameters[3];
                    var url = '/DocumentCollateral/CheckIfFileExistsToDownload?processQueue1Up=' + processQueue1Up + "&fileFormat=" + fileFormat;
                    var promise = ajaxWrapper.getJSON(url);
                    promise.done(function (doesFileExist) {
                        if (doesFileExist == true) {
                            FolderLockAction.ISOVERRIDEDIALOGACTION = 1;
                            window.location.href = "/DocumentCollateral/DownloadCollateralFile?processQueue1Up=" + processQueue1Up + "&reportName=" + reportName + "&templateReportVersionID=" + templateReportVersionID + "&fileFormat=" + fileFormat;
                        }
                        else
                            messageDialog.show(CollateralQueue.fileDoesNotExist.replace(/\{FileFormat\}/g, fileFormat.toUpperCase()));
                    });
                });
            },
            search: true
        });

        $(currentInstance.elementIDs.queuedCollateralsGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

        refreshInterval = setInterval(function () { AutorefreshQueuedReports(currentInstance.elementIDs.queuedCollateralsGridJQ); }, 1000);
    }

    function AutorefreshQueuedReports(grid) {
        timer--;
        if (timer == 0) {
            $(grid).trigger("reloadGrid");
            timer = 60;
        }
        $("#spnTimer").text(timer + " seconds.");
    }

    function ViewCollateralsAtFolder(currentInstance) {
        $(currentInstance.elementIDs.QueuedCollateralDialog).dialog({
            autoOpen: false,
            height: 300,
            width: 950,
            modal: true,
            close: function () { // When dialog is closed then dispose autorefresh of grid so that it wont keep executing at backround.
                timer = 60;
                clearInterval(refreshInterval);
            }
        });

        $("#spnTimer").text(timer + " seconds.");
        $(currentInstance.elementIDs.QueuedCollateralDialog).dialog('option', 'title', "Queued Collateral");
        $(currentInstance.elementIDs.QueuedCollateralDialog).dialog("open");


        var colArray = null;
        colArray = ['Id', 'FormInstanceID', 'Collateral Name', 'Product Name', 'Account Name', 'Folder Name', 'Version Number', 'Status', 'Processed Date', 'Download', 'Download', 'TemplateReportVersionID'];// 'Product Json','Consortium',

        var colModel = [];
        colModel.push({ key: true, hidden: true, name: 'CollateralProcessQueue1Up', index: 'CollateralProcessQueue1Up', align: 'right', editable: false });
        colModel.push({ key: false, hidden: true, name: 'FormInstanceID', index: 'FormInstanceID', align: 'right', editable: false });
        colModel.push({ key: false, name: 'CollateralName', index: 'CollateralName', editable: false, width: '120' });
        colModel.push({ key: false, name: 'ProductID', index: 'ProductID', editable: false, width: '150' });
        colModel.push({ key: false, name: 'AccountName', index: 'AccountName', editable: true, edittype: 'select', align: 'left', width: '150' });
        colModel.push({ key: false, name: 'FolderName', index: 'FolderName', editable: true, edittype: 'select', align: 'left', width: '200' });
        colModel.push({ key: false, name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: true, edittype: 'select', align: 'left', width: '130' });
        colModel.push({ key: false, name: 'Status', index: 'Status', editable: false, align: 'left', width: '100' });
        colModel.push({ key: false, name: 'ProcessedDate', index: 'ProcessedDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' });
        colModel.push({ key: false, name: 'CollateralProcessQueue1Up', index: 'CollateralProcessQueue1Up', search: false, editable: false, width: '80', formatter: downloadPDFFileFormmater });
        colModel.push({ key: false, name: 'CollateralProcessQueue1Up', index: 'CollateralProcessQueue1Up', search: false, editable: false, width: '90', formatter: downloadWordFileFormmater });
        colModel.push({ key: false, name: 'TemplateReportVersionID', index: 'TemplateReportVersionID', hidden: true, search: false });

        //clean up the grid first - only table element remains after this
        $(currentInstance.elementIDs.queuedCollateralsGridJQ).jqGrid('GridUnload');

        var url = currentInstance.URLs.ViewCollateralsAtFolder.replace(/\{formInstanceId\}/g, currentInstance.currentFormInstanceID);

        $(currentInstance.elementIDs.queuedCollateralsGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            mtype: 'POST',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Collaterals Queued',
            height: '200',
            headertitles: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortname: 'ProcessedDate',
            sortorder: 'desc',
            search: true,
            gridComplete: function () {
                $(".DownloadQueuedReport").on('click', function (e) {
                    var parameters = $(this).attr("data-Id").split('ö');
                    var processQueue1Up = parameters[0];
                    var reportName = parameters[1];
                    var templateReportVersionID = parameters[2];
                    var fileFormat = parameters[3];
                    var url = '/DocumentCollateral/CheckIfFileExistsToDownload?processQueue1Up=' + processQueue1Up + "&fileFormat=" + fileFormat;
                    var promise = ajaxWrapper.getJSON(url);
                    promise.done(function (doesFileExist) {
                        if (doesFileExist == true) {
                            FolderLockAction.ISOVERRIDEDIALOGACTION = 1;
                            window.location.href = "/DocumentCollateral/DownloadCollateralFile?processQueue1Up=" + processQueue1Up + "&reportName=" + reportName + "&templateReportVersionID=" + templateReportVersionID + "&fileFormat=" + fileFormat;
                        }
                        else
                            messageDialog.show(CollateralQueue.fileDoesNotExist.replace(/\{FileFormat\}/g, fileFormat.toUpperCase()));
                    });
                });
            },
        });

        $(currentInstance.elementIDs.queuedCollateralsGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

        refreshInterval = setInterval(function () { AutorefreshQueuedReports(currentInstance.elementIDs.queuedCollateralsGridJQ); }, 1000);
    }

    function ViewCollateralsAtFolder(currentInstance) {
        $(currentInstance.elementIDs.QueuedCollateralDialog).dialog({
            autoOpen: false,
            height: 300,
            width: 950,
            modal: true,
            close: function () { // When dialog is closed then dispose autorefresh of grid so that it wont keep executing at backround.
                timer = 60;
                clearInterval(refreshInterval);
            }
        });

        $("#spnTimer").text(timer + " seconds.");
        $(currentInstance.elementIDs.QueuedCollateralDialog).dialog('option', 'title', "Queued Collateral");
        $(currentInstance.elementIDs.QueuedCollateralDialog).dialog("open");



        var colArray = null;
        colArray = ['Id', 'FormInstanceID', 'Collateral Name', 'Product Name', 'Account Name', 'Folder Name', 'Version Number', 'Status', 'Processed Date', 'Download', 'Download', 'TemplateReportVersionID'];// 'Product Json','Consortium',

        var colModel = [];
        colModel.push({ key: true, hidden: true, name: 'CollateralProcessQueue1Up', index: 'CollateralProcessQueue1Up', align: 'right', editable: false });
        colModel.push({ key: false, hidden: true, name: 'FormInstanceID', index: 'FormInstanceID', align: 'right', editable: false });
        colModel.push({ key: false, name: 'CollateralName', index: 'CollateralName', editable: false, width: '120' });
        colModel.push({ key: false, name: 'ProductID', index: 'ProductID', editable: false, width: '150' });
        colModel.push({ key: false, name: 'AccountName', index: 'AccountName', editable: true, edittype: 'select', align: 'left', width: '150' });
        colModel.push({ key: false, name: 'FolderName', index: 'FolderName', editable: true, edittype: 'select', align: 'left', width: '200' });
        colModel.push({ key: false, name: 'FolderVersionNumber', index: 'FolderVersionNumber', editable: true, edittype: 'select', align: 'left', width: '130' });
        colModel.push({ key: false, name: 'Status', index: 'Status', editable: false, align: 'left', width: '100' });
        colModel.push({ key: false, name: 'ProcessedDate', index: 'ProcessedDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' });
        colModel.push({ key: false, name: 'CollateralProcessQueue1Up', index: 'CollateralProcessQueue1Up', search: false, editable: false, width: '80', formatter: downloadPDFFileFormmater });
        colModel.push({ key: false, name: 'CollateralProcessQueue1Up', index: 'CollateralProcessQueue1Up', search: false, editable: false, width: '90', formatter: downloadWordFileFormmater });
        colModel.push({ key: false, name: 'TemplateReportVersionID', index: 'TemplateReportVersionID', hidden: true, search: false });

        //clean up the grid first - only table element remains after this
        $(currentInstance.elementIDs.queuedCollateralsGridJQ).jqGrid('GridUnload');

        var url = currentInstance.URLs.ViewCollateralsAtFolder.replace(/\{formInstanceId\}/g, currentInstance.currentFormInstanceID);

        $(currentInstance.elementIDs.queuedCollateralsGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            mtype: 'POST',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Collaterals Queued',
            height: '200',
            headertitles: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortname: 'ProcessedDate',
            sortorder: 'desc',
            search: true,
            gridComplete: function () {
                $(".DownloadQueuedReport").on('click', function (e) {
                    var parameters = $(this).attr("data-Id").split('ö');
                    var processQueue1Up = parameters[0];
                    var reportName = parameters[1];
                    var templateReportVersionID = parameters[2];
                    var fileFormat = parameters[3];
                    var url = '/DocumentCollateral/CheckIfFileExistsToDownload?processQueue1Up=' + processQueue1Up + "&fileFormat=" + fileFormat;
                    var promise = ajaxWrapper.getJSON(url);
                    promise.done(function (doesFileExist) {
                        if (doesFileExist == true) {
                            FolderLockAction.ISOVERRIDEDIALOGACTION = 1;
                            window.location.href = "/DocumentCollateral/DownloadCollateralFile?processQueue1Up=" + processQueue1Up + "&reportName=" + reportName + "&templateReportVersionID=" + templateReportVersionID + "&fileFormat=" + fileFormat;
                        }
                        else
                            messageDialog.show(CollateralQueue.fileDoesNotExist.replace(/\{FileFormat\}/g, fileFormat.toUpperCase()));
                    });
                });
                $(currentInstance.elementIDs.QueuedCollateralDialog).dialog({
                    position: {
                        my: 'center',
                        at: 'center'
                    },
                });
            },
        });

        $(currentInstance.elementIDs.queuedCollateralsGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

        refreshInterval = setInterval(function () { AutorefreshQueuedReports(currentInstance.elementIDs.queuedCollateralsGridJQ); }, 1000);
    }

    function downloadPDFFileFormmater(cellvalue, options, rowObject) {
        cellvalue = "<img src='/Content/css/custom-theme/images/pdf.png' class='DownloadQueuedReport' alt='PDFCollateral' style='margin-left: 5px;'  title = 'PDF Collateral' data-Id='" + rowObject.CollateralProcessQueue1Up + 'ö' + rowObject.CollateralName + 'ö' + rowObject.TemplateReportVersionID + 'ö' + 'pdf' + "'/>";
        return cellvalue;
    }

    function downloadWordFileFormmater(cellvalue, options, rowObject) {
        cellvalue = "<img src='/Content/css/custom-theme/images/word.png' class='DownloadQueuedReport' alt='WordsCollateral' style='margin-left: 5px;'  title = 'Word Collateral' data-Id='" + rowObject.CollateralProcessQueue1Up + 'ö' + rowObject.CollateralName + 'ö' + rowObject.TemplateReportVersionID + 'ö' + 'word' + "'/>";
        return cellvalue;
    }
    folder.prototype.generateReportForCollateralTemplate = function (currentIns, TemplateReportID, TemplateReportName) {
        try {
            var currentInstance = this;
            var url = currentInstance.URLs.isReportAlreadyGenerated.replace(/\{formInstanceId\}/g, currentInstance.currentFormInstanceID)
            url = url.replace(/\{reportTemplateId\}/g, TemplateReportID)

            var promise = ajaxWrapper.postJSON(url);
            promise.done(function (result) {
                if (result != null && result.ProcessedDate != null) {
                    var confirmationDialog;
                    confirmationDialog = confirmationDialog || (function (currentIns) {
                        $(function (currentIns) {
                            $('#dwnQueueCnlConfirmDialog').dialog({
                                modal: true,
                                autoOpen: false,
                                draggable: true,
                                resizable: true,
                                zIndex: 1005,
                                closeOnEscape: false,
                                title: 'Download/Queue',
                                open: function (event, ui) {
                                },
                                buttons: {
                                    Download: function () {
                                        downloadQueuedProductReport(currentInstance, result.FormInstanceID, result.CollateralProcessQueue1Up);
                                        $(this).dialog("close");
                                    },
                                    Queue: function () {
                                        queueProduct(currentInstance, TemplateReportID, TemplateReportName);
                                        $(this).dialog("close");
                                    },
                                    Cancel: function () {
                                        $(this).dialog("close");
                                    }
                                }
                            }).parent().find('.ui-dialog-titlebar-close').hide();
                        });
                        return {
                            show: function (message) {
                                $('#dwnQueueCnlConfirmDialog').find('div p').text(message);
                                $('#dwnQueueCnlConfirmDialog').dialog('open');
                            },
                            hide: function () {
                                $('#dwnQueueCnlConfirmDialog').dialog('close');
                            },
                        };
                    })();

                    var message = "Collateral is already generated/queued for this Document on " + result.ProcessedDate + ". Please choose appropriate action :";
                    confirmationDialog.show(message);
                }
                else {
                    queueProduct(currentInstance, TemplateReportID, TemplateReportName);
                }
            });
        }
        catch (ex) {
            console.log(ex);
        }
    }


    return {
        getInstance: function (accountId, folderVersionId, folderId, referenceformInstanceId, isEditable, folderType, viewMode, currentUserId, currentUserName) {
            if (instance === undefined) {
                instance = new folder(accountId, folderVersionId, folderId, referenceformInstanceId, isEditable, folderType, viewMode, currentUserId, currentUserName);
            }
            else if (instance.accountId == accountId && instance.folderVersionId == folderVersionId && instance.folderId == folderId && instance.referenceformInstanceId == referenceformInstanceId) {
                return instance;
            }
            else {
                instance = undefined;
                instance = new folder(accountId, folderVersionId, folderId, referenceformInstanceId, isEditable, folderType, viewMode, currentUserId, currentUserName);
            }
            return instance;
        }
    }
}();

function Factory() {
    this.createFormInstance = function (viewType, params) {
        var formInstanceBuilderObj;

        if (viewType == FolderViewMode.DefaultView) {
            formInstanceBuilderObj = new formInstanceBuilder(params.TenantId, params.AccountId, params.FolderVersionId, params.FolderId, params.FormInstanceId,
                                        params.FormDesignVersionId, params.FormDesignName, params.AutoSaveWorkder, params.AutoSaveTimer, params.IsReleased,
                                        params.FolderData, params.FolderType, params.AnchorFormInstanceId, params.IsFormInstanceEditable, params.EffectiveDate,
                                        params.currentUserId, params.currentUserName);
        }
        else {
            formInstanceBuilderObj = new sotFormInstanceBuilder(params.TenantId, params.AccountId, params.FolderVersionId, params.FolderId, params.FormInstanceId,
                                        params.FormDesignVersionId, params.FormDesignName, params.AutoSaveWorkder, params.AutoSaveTimer, params.IsReleased,
                                        params.FolderData, params.FolderType, params.AnchorFormInstanceId, params.IsFormInstanceEditable, params.EffectiveDate,
                                        params.currentUserId, params.currentUserName);
        }

        return formInstanceBuilderObj;
    }
}



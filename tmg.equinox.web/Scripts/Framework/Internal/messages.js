var Common = {
    errorMsg: "An error occurred while processing your request.",
    useDataSourceConfirmationMsg: "This document is used as data source for other documents in the folder.",
    deleteConfirmationMsg: 'Please confirm if you want to delete?',
    closeConfirmationMsg: 'Please confirm if you want to close?',
    selectRowMsg: "Select a row to modify.",
    validationErrorMsg: "Please correct the validation errors.",
    pleaseSelectRowMsg: "Please select a row.",
    effectiveDateRequiredMsg: "Effective Date required.",
    categoryRequiredMsg: "Category is required.",
    effectiveDateValidMsg: "Please enter a valid Effective Date.",
    effectiveDateValidateMonthMsg: "Please enter a valid month",
    effectiveDateValidateDayMsg: "Please enter a valid day",
    effectiveDateValidateYearMsg: "Please enter a valid 4 digit year",
    fieldNameRequiredMsg: "Field Name required.",
    fieldNameExistsMsg: "Field Name already exists.",
    fieldEditNameValidateMsg: "Edit Field Name. Existing names are not allowed.",
    fieldUniqueNameMsg: "Add a new Field Name. Existing names are not allowed.",
    folderEffectiveDateRequiredMsg: "Folder effective date required.",
    documentChangesAreUnsavedMsg: "There are some changes in the document which are unsaved.",
    documentChangesAreUnsavedDefaultMsg: "You have unsaved stuff. Are you sure to leave?",
    duplicateValueMessage: "Entered Value already exists in the list. Please enter Unique Value",
    notApplicable: "Not Applicable",
    saveConfirmationMsg: 'Please confirm if you want to update the changes?',
    acceleratedConfirmationMsg: 'You have selected that the Accelerated workflow applies to this product build.  This will skip the Product Review and Test/Audit steps and will push the product directly to the Facets Production environment.',
    saveFormMsg: 'Please save the newly added form to see impacted documents list.',
    pleaseEnableBulk: "Please enable Bulk Update.",
    closeView: "Do you want to close this document?",
    changeMasterListView: "Do you want to save the current Master List changes?",
    changeView: "Do you want to save the current View changes?",
    confirmchangeView: "Are you sure you want to change the view?",
    noRecordsToDisplay: "No records to display.",
    pleaseSelectSourceMsg: "Please select source document to compare.",
    blankCoverageLevel: "Please add Coverage level to empty rows!",
    userGridSetting: "Do you want to continue with page refresh otherwise new settings will applied on next page load.",
    CanNotBeDuplicated:"{0} can not be duplicated",
    pleaseselectrowformultidelete:"Please select the row(s) to delete multiple records."
};

var ConsumerAccount = {
    folderAbsentMsg: "Account does not have folder",
    saveFolderMsg: 'Folder saved successfully.',
    addAccountMsg: 'Account added successfully',
    updateAccountMsg: 'Account updated successfully',
    duplicateAccountMsg: 'Account name already exists.',
    accountNotExistMsg: 'Account Does Not Exist',
    folderNameExistsMsg: "This Folder Name already exists for this Account. Please enter a different Folder Name.",
    accountNameRequiredMsg: "Account name is required.",
    marketSegmentRequiredMsg: "Market Segment is required.",
    primaryContactRequiredMsg: "Primary Contact is required.",
    folderNameRequiredMsg: "Folder Name required.",
    doNotDisplayPortFolioQuestionMsg: "do not display portfolio folder question",
    diplayPortFolioQuesMsg: "display portfolio folder question",
    deleteConfirmationForAccountMsg: 'Please confirm that would you like to delete {#accountName}.   This action cannnot be undone',
    categoryRequiredMsg: "Category is required.",
    deleteConfirmationForFolderMsg: 'Please confirm that would you like to delete {#FolderName}.   This action cannnot be undone',
    userWarning: "<b>Warning:</b> This Folder cannot be deleted. User <b>\"{0}\"</b> currently has this Folder Version locked for editing.",
    folderDeleteSuccess: "Folder Deleted successfully."

};
var FolderVersion = {
    duplicateDocumentInstanceMsg: "Document Instance is already available for selected folder version.",
    versionEditValidationMsg: "Major version can not be edited",
    saveDocumentInstanceMsg: "Document Instance Data saved successfully.",
    validatingMsg: "Validating.....",
    loadDocumentsBeforeValidatingMsg: "Please select and view each document before completing the Folder Validation.",
    folderVersionCreatedSuccess: "Folder Version created successfully.",
    selectRowToView: "Please select a row to View",
    selectRowToEdit: "Please select a row to Edit",
    selectRowToDelete: "Please select a row to Delete",
    selectRowToRelease: "Please select a row to Release",
    selectRowToTranslateQueue: "Please select a row to send ML in Translation Queue!",
    selectRowToTransmitTESTQueue: "Please select a row to send ML in Transmit TEST Queue!",
    selectRowToTransmitPRODQueue: "Please select a row to send ML in Transmit PROD Queue!",
    MLTranslateQueueSuccess: "MasterList Folder Version sent to Translation queue successfully!",
    MLTransmitterQueueSuccess: "MasterList Folder Version sent to Transmitter queue successfully!",
    folderVersionReleaseSuccess: "Folder Version released successfully.",
    folderVersionCascadeReleaseSuccess: "Master List Released and Plan Updates queued. Please see the Status of processing on the Plan Updates Status screen.",
    folderVersionRetroSuccess: "Folder Version Retro changes created successfully.",
    selectInProgress: 'Please select In Progress Minor Version',
    inProgress: 'In Progress',
    new: 'New',
    baselined: 'Baselined',
    inProgressBlocked: 'In Progress-Blocked',
    folderVersionDeletedSuccess: "Folder Version Deleted successfully.",
    folderVersionRollbackSuccess: "Folder Version Rollback done successfully.",
    confirmDeleteFolderVersion: "Are you sure want to Delete the FolderVersion ?",
    confirmRollbackFolderVersion: "Are you sure you want to Rollback the FolderVersion ?",
    confirmReleaseFolderVersion: "Are you sure you want to Release the FolderVersion ?",
    confirmCascadeReleaseFolderVersion: "Are you sure you want to proceed with the Plan Update? The action cannot be undone.",
    confirmSendTranslateQueueFolderVersion: "Are you sure you want to send the ML to Translation Queue?",
    confirmSendTransmitTESTQueueFolderVersion: "Are you sure you want to send the ML to Transmit TEST Queue?",
    confirmSendTransmitPRODQueueFolderVersion: "Are you sure you want to send the ML to Transmit PROD Queue?",
    cannotPerformDelete: "Cannot Delete Folder Version",
    documentNameRequiredMsg: "Document Name is required.",
    documentTypeRequiredMsg: "Document Type is required.",
    selectDocumentRequiredMsg: "Document  is required.",
    selectFolderTypeRequiredMsg: "Folder Type is required.",
    selectFolderRequiredMsg: "Folder is required.",
    selectFoldeVersionRequiredMsg: "Folder Version is required.",
    selectAccountRequiredMsg: "Account is required.",
    copyFromPortfolioOrAccountRequiredMsg: "Copy from Portfolio or Account Folder is required.",
    ownerRequiredMsg: "Owner is required.",
    addMinorVersionWithEffectiveDateValiadteMsg: "Select the Effective Date for the new Folder Version",
    selectWorkFlowStateMsg: "Select Workflow State",
    selectApprovalStatusMsg: "Select Approval Status",
    statusUpdateMsg: "Status Update",
    selectOwnerMsg: "Select Owner",
    retroisNotEffective: "Retro cannot perform for this Effective Date",
    statusWebWorkerMsg: "Sorry, your browser does not support Web Workers..",
    deleteConfirmationMsg: 'Please confirm that you would like to delete <b>{#documnentName}</b>.<br>This action cannot be undone.',
    documentTypeEffectiveDateMsg: 'The Document Designs first version effective date must be equal to, or prior to, the effective date of a Folder.',
    documentNotPresentMsg: 'This Folder does not contain any document to copy.',
    adminDoesNotExists: 'This folder does not contain Admin Document. Please add Admin document to view report.',
    folderVersionRenewalMsg: 'Folder Version created successfully. This is the first folder renewal post-migration to ' + GLOBAL.applicationName +'. Please review and validate that the correct Benefit Categories have been applied to the product for Types of Service that can fall under multiple categories.',
    productInTranslationMsg: 'Product is disabled as the product is in Translation. To Edit the product form, please reload it after completion of translation.',
    productQueuedMsg: 'Product successfully queued',
    productQueueError: 'Error in queuing products for translation.',
    transmitterEmptyMsg: 'Transmitter List Empty',
    translatorEmptyMsg: 'Translator list empty',
    masterListQueuedMsg: 'Master List successfully queued',
    cascadeCommentEmptyMsg: 'Cascade comment can not be empty',
};
var DocumentDesign = {
    saveMsg: "Saved successfully.",
    saveElementMsg: "Data Source Element saved successfully",
    saveMappingMsg: "Data Source Mapping saved successfully.",
    saveFieldMsg: "Field saved successfully.",
    designVersionDeleteMsg: "Document Design Version deleted successfully.",
    designAddSuccessMsg: "Document Design Added Successfully.",
    designUpdateSuccessMsg: "Document Design Saved Successfully.",
    designVersionAddSuccessMsg: "Document Design Version Added Successfully.",
    designVersionUpdateSuccessMsg: "Document Design Version Saved Successfully.",
    inProgressDesignSelectionMsg: "Please select the In Progress Document Design Version.",
    deleteMsg: "Deleted successfully.",
    documentCompileMsg: "Document Design has been compiled.",
    compileFailureMsg: "Document Design failed to compile.",
    addSectionMsg: "Section added successfully.",
    designDeleteValidationMsg: "Please delete all DocumentDesignVersions before deleting Document Design.",
    inProgressDesignValidationMsg: "Only one Document Design Version can be In Progress. New Version can be added only after it is Finalized.",
    designModificationValidationMsg: "Finalized Document Design cannot be modified.",
    deleteDesignValidationMsg: "A Finalized Version of a Document Design cannot be deleted.",
    deleteFromFinalizedValidationMsg: "Deleting elements from Finalized Document Design Version is not allowed.",
    deleteUIElementUsedInDataSource: "Cannot delete UIElement, It is used as source element in DataSourceMapping.",
    datasourceElementSaveConfirmMsg: "Please confirm if you want to save the Data source elements?",
    dataSourceRowSelectionMsg: "Please Select Row First.",
    mapElementSelectionMsg: "Please select map element.",
    mapElementAlreadySavedMsg: "Mapping is Already Saved.",
    dataSourcemodeSelectionMsg: "Please Select Data Source Mode. ",
    filterElementSelectionMsg: "Please Enter Filter Value.",
    mapIsKeyWithLabelType: "Please map with Label Type for IsKey element.",
    primarySelectionMsg: "Please select Is Primary.",
    isKeyMsg: "Please select Is Key.",
    confirmFinalizationMsg: "Please confirm if you want to finalize?",
    previewWarningMessage: "This feature is under construction.",
    finalizationSuccessMsg: "Document Design version finalized successfully.",
    finalizationCannotPerformMsg: "Document Design version cannot be finalized,because DataSources that we use from other documents need to be finalized before, those documents are \n",
    nameValidateMsg: "Name should start with an alphabet or number.",
    specialCharNotAllowedMsg: "Name should not contain special character.",
    sameEffectiveDateValidateMsg: "Document Design Version with the same effective date already exists.",
    effectiveDateGreaterThanVersionMsg: "Effective Date should be greater than Version 1.0 Effective Date.",
    designAddNewNameValidateMsg: "Add a new Document Design Name. Existing names are not allowed.",
    designEditNameValidateMsg: "Edit Document Design Name. Existing names are not allowed.",
    designNameRequiredMsg: "Document Design Name required.",
    designNameAlreadyExistsMsg: "Document Design Name already exists.",
    addDocumentDesign: "Add Document Design",
    editDocumentDesign: "Edit Document Design",
    designAddVersionMsg: "Add Document Design Version",
    editDocumentDesignVersion: "Edit Document Design Version",
    designAddNewVersionValidateEffectiveDateMsg: "Select an Effective Date for new Document Design Version.",
    designAddNewVerionValidateCheckBoxMsg: "Select a Check Box to copy an earlier Document Design Version.",
    designEditVersionValidateEffectiveDateMsg: "Edit Document Design Version Effective Date.",
    designAddComments: "Add Comments",
    fieldEditCustomRules: "Edit Custom Rules -",
    customRegexDialog: "Custom Regex - ",
    invalidRegexMsg: "Invalid regular expression. Please enter a valid regular expression",
    manageSection: "Manage Sections -",
    sectionNameRequired: "Section Name required.",
    sectionNameAlreadyExistsMsg: "Section Name already exists.",
    addSection: "Add Section",
    sectionAddNewNameValidateMsg: "Add a new Section Name. Existing names are not allowed.",
    sectionEditNameValidateMsg: "Edit Section Name. Existing names are not allowed.",
    MinMaxDateValidateMsg: "Min Date can not be greater than Max Date.",
    dataSourceNameUniqueMsg: "The Data Source name should be unique.",
    mdmUniqueMsg: "The MDM name should be unique.",
    maxLengthMsg: "Length can not be more than ",
    dropDownManageItemsMsg: "Manage Dropdown Items -",
    dropDownItemNameRequiredMsg: "Item Name required.",
    dropDownItemNameExistsMsg: "Item Name already exists.",
    dropDownAddNewItemValidateMsg: "Add a new Item Name. Existing names are not allowed.",
    dropDownEditItemValidateMsg: "Edit Item Name. Existing names are not allowed.",
    roleAccessPermissionDialog: "Role Access Permissions",
    roleAccessPermissionMapSuccessMsg: "Role Access Permissions applied successfully. Please compile the form to see effect.",
    deleteDocumentDesign: "Document Design Deleted Successfully",
    roleAccessPermissionValidationMsg: "Please select at least one user role for 'Edit'",
    roleAccessPermissionMapErrorMsg: "Error to save Role Access permission set",
    allowLoadFromServerValidationMessage: "To enable LoadFromServer, please Remove the functionality of:",
    isDataSource: " IsDataSource",
    rules: " Rules",
    allowRepeaterLoadFromServerConfirmSaveMsg: "Once LoadFromServer is checked, it can not be undone. Are you sure you want to save?",
    versionFinalizedMsg: "Document design version has been finalized.It is no longer in edit mode.",
    deleteConfirmationForDropDownItem: 'Please confirm that you would like to delete {#itemName}. This action cannot be undone.',
    restrictExistingLayoutToChangeMsg: "Existing Layout can not be changed to custom layout as it has sub section or repeater inside it.",
    AreYouSureYouWantToGenerateANewProductID: "Are you sure you want to generate a New Product ID ?",
    repeaterKeyValidationMessage: "Please select at least one key for the repeater.",
    repeaterKeyElement: 'Unable to delete the element, as it is associated with datasource mapping key element.',
    standardeMSElement: 'Unable to delete the element, as it is associated with standard eMS System.',
    docdesignAddSuccessMsg: "Document Design Type Added Successfully.",
    addDocumentDesignType: "Add Document Design Type",
    documentdesignAddNewNameValidateMsg: "Add a new Document Design Type Name. Existing names are not allowed.",
    documentdesignNameAlreadyExistsMsg: "Document Design Type Name already exists.",
    anchortotargetlinkMsg: "Please select Anchor Design to keep it linked with created Document Design.",
    documentdesignNameRequiredMsg: "Document Design Type Name required.",
    openDocumentValidationMsg: "Please select Document with correct view",
    deleteMappingOnChangeControl: "Are you sure you want to delete Mapping for the control",
    controlChangeSuccessMsg: "Change Control Type done Successfully.",
    dropDowncontrolChangeReqMsg: "Please select an option in Change Control Type.",
    expSaveElementMsg: "Expression Builder Rule for element saved successfully",
    expEnterJsonMsg: "Please enter valid JSON data",
    expDuplicateMsg: "Please do not save duplicate data",
    documentRuleCompileMsg: "Document Rule has been compiled.",
    documentRuleCompileFailureMsg: "Document Rule failed to compile.",
    rowsPerPageRequiredMsg: "Rows Per Page Required.",
    rowsPerPageGreaterThanZeroMsg: "Rows per page should be greater than 0.",
    isAliasDesignAdd: "Is Design Alias: for setting aliases for Collateral",
    usesAliasDesignAdd: "Uses Alias Design - for Collateral Master Lists",
    expEmptyElementGrid: "Please enter a source element details.",
    expDuplicateElementSources: "Duplicate sources elements found.",
    expEnterScript: "Please enter valid script.",
    expDescriptionLength: "Desiption length should not be more than 100 charactors.",
    expDeleteData: "Rule doesn't exist to delete.",
    deleteRule: "Do you want to delete current rule?",
    deleteElement: "Do you want to delete current element?",
    downloadExpressionJsonErrorMsg: "Rule doesn't exist to download."
};
var DocumentGroupDesign = {
    designMapSuccessMsg: "Document Design mapped successfully.",
    addFolderMsg: "Add Document Folder",
    editFolderMsg: "Edit Document Folder",
    addFolderSuccessMsg: "Document Folder added successfully.",
    addFolderNameMsg: "Add a new Document Folder Name. Existing names are not allowed.",
    editFolderNameMsg: "Edit Document Folder Name. Existing names are not allowed."

};

var DashBoard =
{
    selectRowToViewFolderMsg: "Select a row to view the Folder",
    selectRowToShowTaskAssignmentEditMsg: "Select a row to edit the task assignment.",
    selectRowToShowTaskAssignmentViewMsg: "Select a row to view the task assignment.",
    selectRowToShowUpdateTaskMsg: "Select a row to update the task.",
    selectRowToShowViewTaskMsg: "Select a row to view the task.",
    planTaskUserMapSaveMsg: "Task saved successfully.",
    planTaskUserMapUpdateMsg: "Task updated successfully.",
    startDuedateValidationMsg: "Start date cannot be greater than due date",
    selectAccountRequiredMsg: "Please select account.",
    selectFolderRequiredMsg: "Please select folder.",
    selectFoldeVersionRequiredMsg: "Folder Version is required.",
    selectWorkFlowStateRequiredMsg: "Select Workflow State",
    selectPlanRequiredMsg: "Please select plan.",
    selectTaskRequiredMsg: "Please select task",
    selectAssignUserRequiredMsg: "Please select assign user",
    selectTaskStatusRequiredMsg: "Please select task status",
    selectStartDateRequiredMsg: "Please select start date",
    selectDueDateRequiredMsg: "Please select due date",
    confirmTaskAssignmentSaveMsg: "Do you want to save task assignment changes?",
    confirmTaskCompleteMsg: "Are you sure task has been completed?",
    entertaskdescriptionMsg: "Please enter task description",
    UseraddrestrictedMsg: "Only Team Manager can add Task Assignment.",
    UsereditrestrictedMsg: "Only Team Manager can edit Task Assignment.",
    selectDueDateGreaterThanCurrentMsg: "Due date should be greater than current date.",
    currentDateValidationMsg: "Start date cannot be less than current date",
    estimateTimeValidationMsg: "Estimated Time must be numeric",
    actualTimeValidationMsg: "Actual Time must be numeric",
    durationValidationMsg: "Duration must be positive numeric value",
    PBPSharePointLink: "https://wellcareportal.wellcare.com/SalesandMarketing/ProductMgmt/Medicare%20Products/CCPTeamSite/Proofing/PBP%20Templates%202019/Forms/AllItems.aspx",
    selectPlanValidationMsg: '"ALL Plans" option cannot be selected with other options',
    selectViewValidationMsg: '"ALL Views" option cannot be selected with other options',
    selectSectionValidationMsg: '"ALL Sections" option cannot be selected with other options',
    PBPSharePointLink: "https://themostg.sharepoint.com/:x:/s/wellcare/Deliverable/EaDA_UQbZepHsx3dvDisiTsBp_uAJzSMm1afHdXVxWBWfw",
};

var LogOff =
{
    logOffMsg: "Are you sure you want to Log off?",
};

var Validation = {
    requiredMsg: '{0} is required.',
    regexMsg: '{0} does not have correct format.',
    incorrectValueMsg: 'Select One is an incorrect value for {0}.',
    selectOne: '[Select One]',
    invalidIntMsg: '{0} is not a valid number.',
    invalidFloatMsg: '{0} is not a valid decimal number.',
    duplicateMsg: 'Value in {0} already exists.',
    ruleErrorMsg: '{0} rule failed.',
    dataRequiredMsg: '{0} Repeater Data is required',
    invalidDateMsg: '{0} does not have a valid date format.',
    exceedMaxLen: '{0} Exceeds its Max Length'
};

var Folder = {
    folderHasValidationError: "This folder can not be moved to Facets Prod State as it has some validation errors. Please correct the validation Errors.",
    loadDocumentsBeforeReleasingMsg: "Please select and view each Document before  moving the Product to Facets Prod.",
    folderValidationErrorMsg: "Workflow step can not be updated as there are validation errors. Please correct the validation Errors.",
    exitValidationErrorMsg: "Plans have been queued for processing. Please refer Exit Validate screen or Notifications bell for Exit Validate status.",
    exitValidationPlansCompletedMsg: "Please refer Exit Validate screen or Notifications bell for Exit Validate status for individual plans.",
    viewEachDocumentMsg: "Please select and view each Document",
    folderValidationErrorDocumentsMsg: "Workflow step can not be updated as there are validation errors. Please correct the validation Errors of below documents: <br>{#documentList}"

}


var ChangeSummary = {
    accountNameRequiredMsg: "Account name is required",
    folderNameRequiredMsg: "Folder name is required",
    folderVarsionRequiredMsg: "Folder version is Required",
    formInstanceRequiredMsg: "Form instance is Required",
}

var QhpTemplate = {
    accountNameRequiredMsgNew: "Account or Portfolio name is required.",
    folderNameRequiredMsg: "Folder name is required",
    folderVarsionRequiredMsg: "Folder version is Required",
    qhpErrorGenerationMsg: "Error in generating Qhp Files!",
    qhpValidateSuccessMsg: "QHP XML Validate successfully",
};

var JournalEntry = {
    addJournalEntryMsg: "Journal Add",
    updateJournalEntryMsg: "Journal Update",
    viewJournalEntryMsg: "Journal View",
    journalErrorMsg: "This Journal is Closed by a Different User. You cannot add a response to a closed Journal. Please reload the folder.",
    journalCharacterMaxMsg: " characters left",
    journalCharacterLimitMsg: "You have reached the limit",
    attributeNotApplicableMsg: "N/A",
    checkJournalEntryIsOpen: "This folder can not be released as it has some journal errors. Please correct the journal Errors by updating all journal errors status to Closed before releasing.",
    insertDescriptionMsg: "Insert Description",
    insertResponseMsg: "Insert Resonse",
};

var AutoSave = {
    confirmMsg: "Do you want to save {#documnentName} document?",
    checkWebWorkerStatus: "Sorry, your browser does not support Web Workers...",
    workerStatus: "'Worker is suffering!'",
    autoSaveDurationRequiredMsg: "Please enter Auto Save Duration.",
};
var DuplicationCheck = {
    duplicationCheckDialogMsg: "Duplication Check Setup - ",
    saveFieldMsg: "Duplication check field saved successfully.",
    existsValueMsg: "Entered value already exists in the list. Please enter unique value.",
    duplicationCheckErrorMsg: "Error to save duplication check setup",
    addAtleastOneFieldMsg: "Please select at least one Duplicate Check field if repeter selected for Load From Server."
};

var ExportToPDF = {
    pdfTempleteConfiguration: "PDF Templete Configuration",
    templateAlreadyExistsMsg: "Template Name already exists.",
    documentDesignNameRequiredMsg: "Document Design name is required.",
    documentDesignVersionRequiredMsg: "Document Design Version is required.",
    templateNameRequiredMsg: "Template Name is required.",
    uiElementUpdatedSuccessMsg: "Design UIElement mapped Successfully.",
    templateAddSuccessMsg: "Document Template added Successfully. ",
    templateDeleteSuccessMsg: "Document Template Deleted Successfully. ",
    pdfgenerationtitleMsg: "PDF Generation",
    pdfPrviewtitleMsg: "Template Preview"
};

var FolderLock = {
    unlockedFolderVersionMsg: "<b> Warning: </b> Your Edit session has been overridden. You may re-open this Folder Version to complete changes as long as it is not locked by another user.",
    unlockedDocumentMsg: "<b> Warning: </b> Your Edit session has been overridden. You may re-open this document to complete changes as long as it is not locked by another user.",
    userWarning: "<b>Warning:</b> This Folder Version cannot be edited. User <b>\"{0}\"</b> currently locked this Folder for processing.",
    releaseFolderLockMsg: "Please Confirm if you want to Unlock Folder?",
    odmlockMsg: "Folder will start for migration within 10 min.Please save your data immediately.",
    odmMigrationInProgressMsg: "Migration process has started on this folder. Folder will be locked for editing until migration process is complete. ODM screen can be referenced for status of migration."
};

var DocumentSectionLockMsg = {
    autoUnLock :"You have been idle for too long and your session has ended. You may continue working by reopening the View and Section."
};

var DocumentMatch = {
    matchRatePercentageRequired: "Match rate percentage is required",
    atleastOneSearchCriteriaSelection: "At least one search criteria field should be selected"
};

var WorkFlowStateMessages = {
    userSelectionToAddMsg: "Please select at least one user to Add.",
    userSelectionToRemoveMsg: "Please select at least one user for Removal.",
    atleastOneTeamManagerMsg: 'Please select a Manager',
    selectedUserListEmptyMsg: 'Selected User List can not be empty.',
    selectedTaskListEmptyMsg: 'Selected Task List can not be empty.',
    selectApplicableTeamMsg: 'Please select Applicable Team',
    selectWorkflowStateMsg: 'Please select Workflow State',
    saveSuccessMsg: 'Applicable Team and users saved successfully.',
    validateMilestoneChecklistDocument: "Please select and view Milestone Checklist Document before approving the Development State.",
    addMilestoneChecklistDocument: "Please Add Milestone Checklist Document before approving the current Workflow State.",
    addTeamManager: "TeamManager is not assigned for next Workflow State. Please assign it first.",
    emailStatusMsg: "Email sending information is insufficient , hence unable deliver an email.Please contact support team.",
    atleastOneApplicableTeamMsg: "Please select at least one Applicable Team.",
    fillCheckListRequiredFields: "The Milestone CheckList Document is having some validation errors. Please resolve those errors before updating the status.",
    marketApprovalStateStatus: "Please approve " + WorkFlowState.MarketApproval + " state before approving " + WorkFlowState.PBPValidation + " state.",
    marketApprovalAlreadyApproved: WorkFlowState.MarketApproval + " state is already approved.",
    TBCOOPCApprovalAlreadyCompleted: WorkFlowState.TBCOOPCApproval + " state is already completed.",
    PartDApprovalAlreadyCompleted: WorkFlowState.PartDApproval + " state is already completed.",
    PlanTaskHasNotCompletedMsg: "Task(s) has not completed for current Workflow State. Please complete those before updating the work flow state."
};

var WorkFlowTaskMapMessages = {
    taskSelectionToAddMsg: "Please select at least one task to Add.",
    taskSelectionToRemoveMsg: "Please select at least one task for Removal.",
    selectedTaskListEmptyMsg: 'Selected Task List can not be empty.',
    selectApplicableWorkflowMsg: 'Please select Applicable Workflow.',
    WorkFlowTaskMapsaveSuccessMsg: 'Applicable workflow and task saved successfully.',
};

var ConsortiumMessages = {
    addConsortiumMsg: 'Consortium added successfully',
    updateConsortiumMsg: 'Consortium updated successfully',
    consortiumRowSelectionMsg: 'Please select row to update',
    duplicateConsortiumMsg: 'Consortium name already exists.',
    consortiumNameRequiredMsg: "Consortium Name required.",
}

var ReportTemplate = {
    addReportTemplate: "Add Collateral Template",
    editReportTemplate: "Edit Collateral Template",
    reportTemplateEditNameValidationMsg: "Please enter a Name that does not already Exist for the Collateral Templates.",
    reportTemplateNameRequiresValidationMsg: "Collateral Template Name is Required Parameter.",
    reportTemplateNameDuplicationMsg: "Collateral Template with the specified Name Exists.",
    reportTmplAdded: "Collateral Template Added Successfully",
    reportTmplUpdated: "Collateral Template Updated Successfully",
    reportTmplErrorMsg: "There was some Error serving the Request.",
    reportTmplAddNewNameValidateMsg: "Add a new Collateral Template. Existing names are not allowed.",
    reportTmplEditNameValidateMsg: "Edit Collateral Template. Existing names are not allowed.",
    repoertDeleteValidationMsg: "Please delete all CollateralDesignVersions before deleting Collateral Design.",
    inProgressDesignSelectionMsg: "Please select the In Progress Collateral Design Version.",
    deletereportDesign: "Collateral Template Deleted Successfully",
    finalizationCannotPerformMsg: "Collateral Template version cannot be finalized \n",
    finalizationSuccessMsg: "Collateral Template version finalized successfully.",
};

var ReportTemplateVersion = {
    addReportTemplateVrsn: "Add Collateral Template Version",
    editReportTemplateVrsn: "Edit Collateral Template Version",
    reportTmplVrsnEffDateRequiredValidationMsg: "Effective Date is Required Parameter.",
    reportTmplVrsnAdded: "Collateral Template Version Added Successfully",
    reportTmplVrsnUpdated: "Collateral Template Version Updated Successfully",
    reportTmplVrsnErrorMsg: "There was some Error serving the Request.",
    reportTmplVrsnDeleted: "Collateral Template Version deleted Successfully",
    reportTmplVrsnCannotDelete: "A Finalized Collateral Template Version cannot be deleted.",
    reportTmplVrsnInProgress: "Only one Collateral Template Version can be In Progress. New Version can be added only after it is Finalized.",
    effectiveDateGreaterThanVersionMsg: "Effective Date should be greater than Version 1.0 Effective Date.",
    reportTmplVrsnNewVersionEffectiveDateMsg: "Select an Effective Date for new Document Design Version.",
    reportTmplVrsnCheckBoxMsg: "Select a Check Box to copy an earlier Collateral Template Version.",
    reportTmplVrsnCanNotFinalize: "This Collateral Template Version can not be finalized as template is not uploaded",
};

var CollateralQueue = {
    enQueuecollateral: "Collateral Queued Successfully",
    collateralQueueErrorMsg: "There was some Error serving the Request.",
    fileDoesNotExist: 'Collateral you are trying to download is either not generated yet or {FileFormat} file does not exists.'
}

var UserManagementMessages = {
    userNameRequiredMsg: "User Name Required",
    userRoleRequiredMsg: "User Role Required",
    emailRequiredMsg: "User Email Address Required",
    userFirstNameRequiredMsg: "User First Name Required",
    userLastNameRequiredMsg: "User Last Name Required",
    resetpasswordConfirmationMsg: 'Please confirm that would you like to reset {#userName} password',
    deleteConfirmationuserMsg: 'Please confirm that would you like to delete {#userName} This action cannnot be undone',
    userNameSelectMsg: 'Please select UserName',
    emailFormatMsg: 'Please enter valid Email ID',
    createUserSucessMsg: 'New user created successfully',
    changeRoleSucessMsg: 'User role changed successfully',
    changeRoleErrorMsg: 'Change user role failed',
    createUserErrorMsg: 'New User creation failed',
    deleteUserSucessMsg: 'User deleted sucessfully',
    unlockUserSucessMsg: 'User unlocked sucessfully',
    resetPasswordSuccessMsg: 'Default password is reset',
    userAlreadyExistMsg: 'User Already Exist',
    userActivateConfirmationMsg: "The User Name '{#UserName}' already exists. Do you want to activate it?",
    userActivateSuccessMsg: "User activated successfully",
    sentEmailNotifcationSucess: 'Credentials are sent in E-mail.',
    sentEmailNotifcationrequiredMsg: 'Please select checkbox to send e-mail Notification',
    sentEmailNotifcationFailedMsg: 'SentEmailNotifcationrequiredMsg',
    emailAlreadyExistMsg: 'EmailID already exist'

}

var AlartRemoveSESEID = {
    userRemoveSESEID: "User should have remove service from Service List and Service group definition section"
}
var TransmissionMsgs = {
    transmissionProdQueueSucceessMsg: 'Products has successfully Queued to Trasmission prod',
    transmissionProdQueueFailedtoSelectMsg: 'Please select 1Up which has completed Transmission to Test',
    ProcessGovernanceRemovedFailedtoSelectMsg: 'Please select a Process Governance number with a Queued status.',
    transmissionProdFlagChangeFailedtoSelectMsg: 'Please select the Process Governance to be transmitted to PROD with status "Queued / Errored" and recovery mode as OFF',
    transmissionTestQueueFailedtoSelectMsg: 'Please select 1Up which has completed Translation'
}

var AuditChecklist = {
    fromDateRequiredMsg: "From Date is required",
    toDateRequiredMsg: "To Date is required",
    fromDateCompareMsg: "From Date should not be greater than To Date"
}

var QManager = {
    selectbeforeafterMsg: "Please add select statement before and after.",
    selectatleastoneobjectMsg: "Please select at least one object from the list."
}

var FolderCategoryMessages = {
    addCategoryMsg: 'Category added successfully',
    addCategoryErrorMsg: 'New Added Category Failed',
    updateCategoryMsg: 'Category updated successfully',
    updateCategoryErrorMsg: 'Failed to update Category',
    categoryRowSelectionMsg: 'Please select row to update',
    duplicateCategoryMsg: 'Category name already exists.',
    categoryNameRequiredMsg: "Category Name required.",
    deleteCategoryMsg: 'Category deleted successfully',
    deleteCategoryErrorMsg: 'Category delete failed',

    addGroupMsg: 'Group added successfully',
    addGroupErrorMsg: 'New Added Group Failed',
    updateGroupMsg: 'Group updated successfully',
    updateGroupErrorMsg: 'Failed to update Group',
    duplicateGroupMsg: 'Group name already exists.',
    groupNameRequiredMsg: "Group Name required.",
    deleteGroupMsg: 'Group deleted successfully',
    deleteGroupErrorMsg: 'Group delete failed',
    groupRowDeleteSelectionMsg: 'Please select row to delete'
}

var ServiceDesign = {
    addNewDesignNameMsg: "Add a new Service Design Name. Existing names are not allowed.",
    addNewMethodNameMsg: "Add a new Service Method Name. Existing names are not allowed.",
    doesReturnAListMsg: "Specify if service returns a list.",
    editNewDesignNameMsg: "Edit a new Service Design Name. Existing names are not allowed.",
    editNewMethodNameMsg: "Edit a new Service Method Name. Existing names are not allowed.",
    addServiceDesign: "Add Service Design",
    editServiceDesign: "Edit Service Design",
    designAddSuccessMsg: "Service Design Added Successfully.",
    designUpdateSuccessMsg: "Service Design Saved Successfully.",
    designNameRequiredMsg: "Service Design Name required.",
    designNameAlreadyExistsMsg: "Service Design Name already exists.",
    methodNameRequiredMsg: "Service Method Name required.",
    methodNameAlreadyExistsMsg: "Service Method Name already exists.",
    addVersionDialogTitle: 'Add Service Design Version',
    editVersionDialogTitle: 'Edit Service Design Version',
    effectiveDateIsRequiredMsg: 'Effective Date is required.',
    formDesignIDIsRequiredMsg: 'Document Design is required.',
    formDesignVersionIDIsRequiredMsg: 'Document Design Version is required.',
    formDesignHelpBlockMsg: "Select the Document Design to be associated with the service.",
    formDesignVersionHelpBlockMsg: "Select the Document Design Version to be associated with the service.",
    effectiveDateHelpblockMsg: 'Select an Effective Date for new Service Design Version.',
    effectiveDateGreaterThanVersionMsg: "Effective Date should be greater than Version 1.0 Effective Date.",
    designVersionAddSuccessMsg: "Service Design Version Added Successfully.",
    designVersionUpdateSuccessMsg: "Service Design Version Saved Successfully.",
    inProgressDesignSelectionMsg: "Please select the In Progress Service Design Version.",
    confirmFinalizationMsg: "Please confirm if you want to finalize?",
    finalizationSuccessMsg: "Service Design version finalized successfully.",
    designModificationValidationMsg: "Finalized Service Design cannot be modified.",
    versionFinalizedMsg: "Service design version has been finalized.It is no longer in edit mode.",
    parameterNameHelpBlockMsg: "Enter the Parameter Name",
    parameterDataTypeHelpBlockMsg: "Select the required DataType",
    parameterIsRequiredHelpBlockMsg: "Check if the parameter is required.",
    parameterAddedSuccessfullyMsg: "Service Parameter added successfully.",
    parameterUpdatedSuccessfullyMsg: "Service Parameter updated successfully.",
    parameterDeletedSuccessfullyMsg: "Service Parameter deleted successfully.",
    deleteServiceDesign: "Web API Design deleted successfully.",
    designDeleteValidationMsg: "Please delete all Web API  Design Versions before deleting Web API Design.",
    deleteDesignValidationMsg: "A Finalized Version of a Web API Design cannot be deleted.",
    searchParameterexistMsg: "Search Parameter already exists. Please select another parameter."
};

var GlobalUpdateMessages = {
    guAddSuccess: "Global Update saved successfully!",
    addUIElementCondition: "Please select at least one UIElement!",
    saveUIElementSuccess: "Elements saved successfully!",
    selectRowMessage: "Please select a row!",
    uploadValidIAS: "Please upload valid Impact Assessment file",
    uploadIASSuccess: "File uploaded successfully!",
    importIASSuccess: "Impact Assessment file data save/imported successfully!",
    errorLogIASMsg: "Errors occurred while saving/importing IAS File data.",
    batchAddSuccess: "Batch added successfully!",
    duplicateBatchName: "There is already an entry of batch with provided Batch Name!",
    saveIASErrorMsg: "Please select at least one element in Element Selection stage to save Impact Assessment file.",
    selectIASMessage: "Please select atleast one Impact Assessment!",
    enterAllValues: "Please enter values for all inputs!",
    selectCheckBox: "Please select Approve Batch checkbox!",
    approveBatchSuccess: "Batch approved successfully!",
    alreadyApprovedBatch: "Selected batch is already approved!",
    batchUpdateSuccess: "Batch updated successfully!",
    timespanValidation: "Please enter Scheduled Time in valid format!",
    batchUpdateSuccess: "Batch updated successfully!",
    editGlobalUpdateConfirmation: "Editing Global Update will revert all the previous changes. Do you want to continue?",
    globalUpdateEditCheck: "Global Update can not be edited. Please select different values and try again.",
    ruleSuccessValuevalidation: " for New Value. Please enter valid value and try again.",
    dataTypeValidation: "DataType is not valid",
    maxLengthvalidation: "Maxlength exceeded",
    UnAvailableAuditReport: 'Audit report is Unavailable for the selected batch.Please contact Support Team.',
    scheduleGlobalUpdateSuccess: "IAS Generation Scheduled successfully.",
    scheduleGlobalUpdateFailed: "Unable to schedule IAS Generation!! There should be at least one impacted folder.",
    uploadIASFailure: "Selected IAS is either scheduled for Batch Execution or Executed already. So you can't upload it again.",
    scheduleIASValidationSuccess: "IAS file Validation Scheduled successfully.",
    realtimeBatchExectionSuccess: "Realtime Batch Executed SuccessFully.",
    UnAvailableIASExcelTemplate: 'IAS Excel Report is Unavailable for the selected Global Update. Please contact Support Team.',
    UnAvailableErrorLogExcelTemplate: 'Error Log Excel Report is Unavailable for the selected Global Update. Please contact Support Team.',
    deleteBatchSuccessResult: 'Batch has been deleted successfully!',
    deleteBatchErrorResult: 'Unable to delete the batch. Please contact Support Team.',
    //scheduledExecutionSuccess: 'Execution Scheduled Successfully!',
    //scheduledExecutionFailed: 'Error occured while saving execution scheduled.Please contact Support Team.',
    //scheduledExecutionValidationMessage: 'Please fill the highlighted inputs and try again!',

    toDateShouldGreater: "Effective Date To should be greater than Effective Date From!!",
    formDateShouldLesser: "Effective Date From should be lesser than Effective Date To!!",
    globalUpdateNameNotBlank: "Global Update Name should not be blank!!",
    fromDateNotBlank: "Effective Date From should not be blank!!",
    toDateNotBlank: "Effective Date To should not be blank!!",
    uniqueFloderInIASBatch: 'For selected IAS impacted folder should be different. Please select IAS which having unique folder!!',
};

var WorkFlowSettingsMessages = {
    wfMasterStateAddTitle: "Add Workflow State Name",
    wfMasterStateLabel: "Workflow State Name",
    wfMasterStateUpdateTitle: "Update Workflow State Name",
    wfMasterAPPTypeAddTitle: "Add Workflow State Approval Type Name",
    wfMasterAPPTypeUpdateTitle: "Update Workflow State Approval Type Name",
    wfMasterAPPTypeLabel: "Workflow State Approval Type Name",
    wfMasterStateAddHelpText: "Add a new Workflow State Name. Existing names are not allowed.",
    wfMasterStateUpdateHelpText: "Update Workflow State Name. Existing names are not allowed.",
    wfMasterAPPTypeAddHelpText: "Add a new Workflow State Approval Type Name. Existing names are not allowed.",
    wfMasterAPPTypeUpdateHelpText: "Update Workflow State Approval Type Name. Existing names are not allowed.",
    wfMasterStateRequiredError: "Workflow State Name is Required.",
    wfMasterStateExistsError: "Workflow State Name already exists.",
    wfMasterAPPTypeRequiredError: "Workflow State Approval Type Name is Required.",
    wfMasterAPPTypeExistsError: "Workflow State Approval Type Name already exists.",
    wfMasterStateUpdateSuccess: "Workflow State updated Successfully!",
    wfMasterAPPTypeUpdateSuccess: "Workflow State Approval Type updated Successfully!",//Sprint5-Bug72
    wfMasterStateDeleteSuccess: "Workflow State deleted Successfully!",
    wfMasterAPPTypeDeleteSuccess: "Workflow State Approval Type deleted Successfully!",

    wfCatAddSuccess: "Workflow Category saved successfully!",
    wfCatUpdateSuccess: "Workflow Category update saved successfully!",
    wfCatDeleteSuccess: "Workflow Category deleted successfully!",
    wfCatCopySuccess: "Workflow Category Copied successfully!",
    wfCatNameRequiredError: "Category Name required.",
    wfAccTypeRequiredError: "Account Type required.",
    wfWFTypeRequiredError: 'Workflow Type required.',
    wfVersionStateAddSuccess: "Workflow State saved Successfully!",
    wfVersionStateDeleteSuccess: "Workflow State deleted Successfully!",
    wfVersionOrderUpdateSuccess: "Workflow State Order saved Successfully!",
    wfVersionStateRequiredError: "Workflow State required.",
    wfVersionStateOrderRequiredError: "Workflow State Order required.",
    wfVersionStateOrderRepeatError: "Sequence cannot be repeated.",
    wfVerStateAccessAddSuccess: "User role saved Successfully!",//Sprint5-Bug72
    wfVerStateAccessUpdateSuccess: "User role update saved Successfully!",//Sprint5-Bug72
    wfVerStateAccessDeleteSuccess: "User role deleted Successfully!",//Sprint5-Bug72
    wfVerStateUserRoleRequiredError: "User Role required.",
    wfVerStateAPStatusAddSuccess: "Approval type saved successfully!",//Sprint5-Bug72
    wfVerStateAPStatusUpdateSuccess: "Approval type update saved successfully!",//Sprint5-Bug72
    wfVerStateAPStatusDeleteSuccess: "Approval type deleted Successfully!",
    wfVersionStateAPStatusRequiredError: "Approval Type required.",
    wfVersionStateAPStatusActionADDSuccess: "Action Details Saved Successfully",//Sprint5-Bug72
    wfVersionActionEmailHelpText: 'Enter comma separated values for multiple email ids.',
    wfVersionActionEmailRequiredError: "Email required.",
    wfVersionActionValidEmailError: "Enter Valid email required.",
    wfVersionActionStateRequiredError: "State required.",
    wfVersionActionLabelForSuccessState: "Move to State on Success",
    wfVersionActionLabelForFailureState: "Move to State on Failure",

    wfCategoryAddTitle: "Add Category Account Workflow Mapping",
    wfCategoryUpdateTitle: "Update Category Account Workflow Mapping",
    wfStateAddTitle: "Add Workflow State",
    wfStateUpdateTitle: "Update Workflow State Sequence",
    wfStateAccessAddTitle: "Add User Role",
    wfStateAccessUpdateTitle: "Update User Role",
    wfStateAPPTypeAddTitle: "Add State Approval Type",
    wfStateAPPTypeUpdateTitle: "Update State Approval Type",
    wfStateAPPActionEmailTitle: "Update Action Details for Email",
    wfStateAPPActionStateTitle: "Update Action Details for Move To State",

    wfCategoryFinalizedDone: "Category Workflow already Finalized!",
    wfCategoryMappingDeleteError: "Finalized Category Workflow cannot be Deleted."
}

var TasksListMessages = {
    taskAddTitle: "Add task description",
    taskLabel: "Task Description",
    taskUpdateTitle: "Update task description",
    taskAddSuccess: "Task saved Successfully!",
    taskDeleteSuccess: "Task deleted Successfully!",
    taskUpdateSuccess: "Task updated Successfully!",
    taskRequiredError: "Task description required.",
    taskExistsError: "Task description already exists.",
    taskAddHelpText: "Add a new task description. Existing names are not allowed.",
    taskUpdateHelpText: "Update task description. Existing names are not allowed.",

    taskIsstandardCheckboxlabel: "Is Standard State",
}

var DashboardMessages = {
    userNameRequired: "User Required.",
    userNameSaved: "User saved successfully!",
    userAssignedSuccess: "User(s) assigned successfully!",
    userUnAssignedSuccess: "User(s) unassigned successfully!",
    userAssignedValidation: "User(s) already assigned.",
    userDialogAssignment: "Assign Users",
    userDialogUnAssignment: "Unassign Users",
}

var MLImportMessages = {
    InvalidFileExtensions: "Please try uploading File with allowed extensions. Allowed extensions are .xls or .xlsx",
    MLFileRequiredMsg: "Please upload a file and try again.",
    MLCommentRequiredMsg: "Please enter a comment to proceed.",
    MLFailedUploadFileMsg1: "Connection Timeout. Please try again.",
    MLFailedUploadFileMsg2: "Incorrect template. Please try again by using a correct template.",
    MLFailedUploadFileMsg3: "The file doesn’t exist. Please try again by choosing a correct file location.",
    MLImportSuccessMsg: "Master List data imported successfully.",
    MLImportFailedMsg: "Master List import failed.",
}

var PBPImportMessages = {
    yearNotMatching: "Selected year does not match year of folder version's effective date.",
    InvalidFileExtensions: "Please try uploading File with allowed extensions. Allowed extensions are .MDB or .ldb",
    PBPFileRequiredMsg: " PBP File is required.",
    PBPPlanAreaFileRequiredMsg: "PBP Plan Area File is required.",
    DatabaseNameRequiredMsg: "Database Name is required.",
    ConfirmReviewStatusImportMsg: "Make sure you have reviewed the Match / Mismatch status of the plan(s).\n Are you sure you want to continue with the import?",
    PBPQueueImportSuccessfullyMsg: "PBP Import Queued successfully.",
    PBPQueueImportUnsuccessfullyMsg: "PBP Import Queued unsuccessfully.",
    DiscardChanges: "Are you sure want to discard changes?"
}
var pBPPlanMatchingConfigrationDialogMsg = {
    FileisnotselectedMsg: "No files selected.",
    QIDisnotsameMsg: "File contains mismatch QID(s)</br>",
    FileschemaisinvalidMsg: "File structure is invalid.",
    Errorinfileupload: "Error in file upload.",
}
var queuedPBPImportGridMsg = {
    EditRowMsg: "Only In Review status can be edited.",
}
var misMatchPlanGridMsg = {
    SelectUserActionMsg: "Please select user action.",
    selectFolderRequiredMsg: "Please select folder.",
    ProductDuplicationMsg: "Product can not be duplicated.",
}
var PBPDatabaseNameMessages = {
    PBPDBadded: "PBP Database Added successfully.",
    PBPDBNameNull: "PBP Database name cannot be empty",
    PBPDBNameExist: "PBP Database name already exist",
    PBPDBUpdated: "PBP Database Updated successfully.",
    SelectRow: "Please Select row to edit"
}
var SectionLockpopUpCommonMessage={
    SelectRowCheckbox: 'Please select at least one Document',
    OpenDocumentInViewOnlyMode: 'Please select one Document',
    NotifyDoneMessage: 'System will notify you on document release'
}

var LimitBulkUpdate={
    Forserviceupdatenorecordfound:"For service update no record found",
    Forserviceupdatepleaseapplyfilter: "For service update please apply filter",
    PleasefilterLimit:"Please filter Limit",
    Recordsaddedsuccessfully:"Records added successfully.",
    Norecordsareaddedfortheselectedservice:"No records are added for the selected service.",
    Recordsdeletedsuccessfully: "Records deleted successfully.",
    Norecordsareavailabletodeletefortheselectedservice:"No records are available to delete for the selected service."
}
var SBCCalculator={
NoChangeFound:"No data change found."
}
var Guardrails = {
OnlyNumber:"Please enter valid number in {0}",
Increment: "{0} should be greater then 0",
HighShouldBeGreaterThenLow: "{0} should be greater then {1}",
LowShouldBeLessThenHigh: "{0} should be less then {1}",
limitDescriptionAlert: "Please remove {0} from Standard Selection (Limit List) before deleting this from Allowable Selection."
}

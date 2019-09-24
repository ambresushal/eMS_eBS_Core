//this function will check the claims passed in viewbag from controller.
function checkApplicationClaims(claims, objMap, sectionName) {
    var claimsData = claims;
    var sectionData = [];

    if (claimsData !== null && claimsData !== undefined)
        claimsData.forEach(function (section) {
            var temp = sectionName.split("?");
            sectionName = temp[0];

            var tempString = "/" + section.Resource;
            if (tempString.toLowerCase() === sectionName.toLowerCase()) {
                sectionData.push(section);
            }
            else if ((tempString + "/").toLocaleLowerCase() === sectionName.toLocaleLowerCase()) {
                sectionData.push(section);
            }
        });

    var releaseFlag=false,editFlag = false, viewFlag = false, retroFlag = false, copyFlag = false, addFlag = false, deleteFlag = false, finalizedFlag = false, previewFlag = false, saveFlag = false, rollbackFlag = false, compileFlag = false, approveBatchFlag = false;

    if (sectionData !== null && sectionData !== undefined) {
        sectionData.forEach(function (claim) {
            if (claim.Action.toLowerCase() === "edit") {
                editFlag = true;
            }
            if (claim.Action.toLowerCase() === "view") {
                viewFlag = true;
            }
            if (claim.Action.toLowerCase() === "retro") {
                retroFlag = true;
            }
            if (claim.Action.toLowerCase() === "copy") {
                copyFlag = true;
            }
            if (claim.Action.toLowerCase() === "new") {
                addFlag = true;
            }
            if (claim.Action.toLowerCase() === "delete") {
                deleteFlag = true;
            }
            if (claim.Action.toLowerCase() === "finalized") {
                finalizedFlag = true;
            }
            if (claim.Action.toLowerCase() === "preview") {
                previewFlag = true;
            }
            if (claim.Action.toLowerCase() === "save") {
                saveFlag = true;
            }
            if (claim.Action.toLowerCase() === "rollback") {
                rollbackFlag = true;
            }
            if (claim.Action.toLowerCase() === "compile") {
                compileFlag = true;
            }
            if (claim.Action.toLowerCase() === "approvebatch") {
                approveBatchFlag = true;
            }
            if (claim.Action.toLowerCase() === "release") {
                releaseFlag = true;
            }
        }); 

        if (!editFlag) {
            $(objMap["edit"]).addClass('ui-state-disabled');
        }
        if (!viewFlag) {
            $(objMap["view"]).addClass('ui-state-disabled');
        }
        if (!retroFlag) {
            $(objMap["retro"]).addClass('ui-state-disabled');
        }
        if (!addFlag) {
            $(objMap["add"]).addClass('ui-state-disabled');
        }
        if (!copyFlag) {
            $(objMap["copy"]).addClass('ui-state-disabled');
        }
        if (!deleteFlag) {
            $(objMap["remove"]).addClass('ui-state-disabled');
        }
        if (!finalizedFlag) {
            $(objMap["finalized"]).addClass('ui-state-disabled');
        }
        if (!previewFlag) {
            $(objMap["preview"]).addClass('ui-state-disabled');
        }
        if (!saveFlag) {
            $(objMap["save"]).addClass('ui-state-disabled');
        }
        if (!rollbackFlag) {
            $(objMap["rollback"]).addClass('ui-state-disabled');
        }
        if (!compileFlag) {
            $(objMap["compile"]).addClass('ui-state-disabled');
        }
        if (!approveBatchFlag) {
            $(objMap["approvebatch"]).addClass('ui-state-disabled');
        }
        if (!releaseFlag) {
            $(objMap["release"]).addClass('ui-state-disabled');
        }

    }
    else {
        $(objMap["edit"]).addClass('ui-state-disabled');
        $(objMap["view"]).addClass('ui-state-disabled');
        $(objMap["retro"]).addClass('ui-state-disabled');
        $(objMap["copy"]).addClass('ui-state-disabled');
        $(objMap["add"]).addClass('ui-state-disabled');
        $(objMap["remove"]).addClass('ui-state-disabled');
        $(objMap["finalized"]).addClass('ui-state-disabled');
        $(objMap["preview"]).addClass('ui-state-disabled');
        $(objMap["save"]).addClass('ui-state-disabled');
        $(objMap["rollback"]).addClass('ui-state-disabled');
        $(objMap["compile"]).addClass('ui-state-disabled');
    }
}

function checkSubGridPortfolioClaims(claims, objMap) {
    
    viewFlag = false, copyFlag = false;

    claims.forEach(function (claim) {

        if (claim.Action.toLowerCase() === "view") {
            viewFlag = true;
        }
        if (claim.Action.toLowerCase() === "copy") {
            copyFlag = true;
        }
    });

    if (!viewFlag) {
        $(objMap["view"]).addClass('ui-state-disabled');
    }
    if (!copyFlag) {
        $(objMap["copy"]).addClass('ui-state-disabled');
    }
}

function checkUserPermissionForEditable(sectionName) {
    var sectionData = [];
    var editFlag = false;
    //to check for user permissions.       

    if (claims !== null && claims !== undefined)
        claims.forEach(function (section) {
            var temp = sectionName.split("?");
            sectionName = temp[0];
            var tempString = "/" + section.Resource;
            if (tempString.toLowerCase() === sectionName.toLowerCase()) {
                sectionData.push(section);
            }
        });

    sectionData.forEach(function (claim) {
        if (claim.Action.toLowerCase() === "edit") {
            editFlag = true;
        }
    });
    return editFlag;
}

//function to authorize access to Dynamic Grids(Document Design Version Grid)
function authorizeDocumentDesignVersionList(grid, sectionURL) {
    var editFlag = checkUserPermissionForEditable(sectionURL);
    if (!editFlag) {
        grid.find('input,select,textarea,.ui-icon-pencil').addClass('ui-state-disabled');
        grid.find('.ui-state-disabled').attr("disabled", "disabled").off('click');
        grid.find('img.ui-datepicker-trigger').css("display", "none");
        //portfolio designer will have view access for Data Source.
        grid.find('#UseDataSource').find('.ui-icon-pencil').removeAttr("disabled").removeClass('ui-state-disabled');
        grid.find('#CustomRule').find('.ui-icon-pencil').removeAttr("disabled").removeClass('ui-state-disabled');
        grid.find('#Rules').find('.ui-icon-pencil').removeAttr("disabled").removeClass('ui-state-disabled');
        grid.find('#DuplicationCheck').find('.ui-icon-pencil').removeAttr("disabled").removeClass('ui-state-disabled');
        grid.find('#LoadFromServer').find('input:checkbox').removeAttr("disabled");
    }
}

//function to authorize access to Dynamic Grids(Data Source Grids accessible only by Super User.)
function authorizePropertyGrid(grid, sectionURL) {
    var editFlag = checkUserPermissionForEditAndDataSource(sectionURL);
    if (!editFlag) {
        grid.find('input,select,textarea,.ui-icon-pencil').addClass('ui-state-disabled');
        grid.find('.ui-state-disabled').attr("disabled", "disabled");
        grid.find('img.ui-datepicker-trigger').css("display", "none");
        grid.find('.ui-state-disabled').addClass("notactive");

        //portfolio designer will have view access for Data Source.
        grid.find('#UseDataSource').find('.ui-icon-pencil').removeAttr("disabled").removeClass('ui-state-disabled notactive');
        grid.find('#CustomRule').find('.ui-icon-pencil').removeAttr("disabled").removeClass('ui-state-disabled notactive');
        grid.find('#Rules').find('.ui-icon-pencil').removeAttr("disabled").removeClass('ui-state-disabled notactive');
        grid.find('#DuplicationCheck').find('.ui-icon-pencil').removeAttr("disabled").removeClass('ui-state-disabled notactive');
        grid.find('#LoadFromServer').find('input:checkbox').removeAttr("disabled");
        grid.find('#LoadFromServer').find('input:checkbox').removeAttr("disabled notactive");
    }
}

//permissions for UIElements FormDesign Version grid.(only super-user should have data source edit permissions.)
function authorizeUIElementsGrid(grid, sectionURL, objMap) {
    var editFlag = checkUserPermissionForEditAndDataSource(sectionURL);
    if (!editFlag) {
        //this will disable the delete button on -UIElements grid.
        $(objMap["remove"]).addClass('ui-state-disabled');
    }
}

function checkUserPermissionForEditAndDataSource(sectionName) {
    var sectionData = [];
    var editFlag = false;
    //to check for user permissions.       

    if (claims !== null && claims !== undefined)
        claims.forEach(function (section) {
            var temp = sectionName.split("?");
            sectionName = temp[0];
            var tempString = "/" + section.Resource;
            if (tempString.toLowerCase() === sectionName.toLowerCase()) {
                sectionData.push(section);
            }
        });

    sectionData.forEach(function (claim) {
        if (claim.Action.toLowerCase() === "datasource") {
            editFlag = true;
        }
    });
    return editFlag;
}

function checkPBPImportClaims(elementIDs, claims) {
    var PBPImportAdd = false;
    var PBPImportEdit = false;

    claims.forEach(function (claims) {
        if (claims.Action.toLowerCase() === "new") {
            PBPImportAdd = true;
        }
        if (claims.Action.toLowerCase() === "edit") {
            PBPImportEdit = true;
        }
    });

    if (!PBPImportAdd) {
        $(elementIDs["addButtonJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["addButtonJQ"]).removeClass('ui-state-disabled');
    }

    if (!PBPImportEdit) {
        $(elementIDs["editButtonJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["editButtonJQ"]).removeClass('ui-state-disabled');
    }

}

function checkPBPDatabaseClaims(elementIDs, claims) {
    var PBPDatabaseNameAdd = false;
    var PBPDatabaseNameEdit = false;
    
    claims.forEach(function (claims) {
        if (claims.Action.toLowerCase() === "new") {
            PBPDatabaseNameAdd = true;
        }
        if (claims.Action.toLowerCase() === "edit") {
            PBPDatabaseNameEdit = true;
        }
    });

    if (!PBPDatabaseNameAdd) {
        $(elementIDs["pBPDatabaseNameAddJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["pBPDatabaseNameAddJQ"]).removeClass('ui-state-disabled');
    }

    if (!PBPDatabaseNameEdit) {
        $(elementIDs["pBPDatabaseNameEditJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["pBPDatabaseNameEditJQ"]).removeClass('ui-state-disabled');
    }

}
//this function will check claims for FolderVersion
function checkFolderVersionClaims(elementIDs, claims) {
    var baseline = false;
    var statusUpdate = false;
    var versionHistory = false;
    var newFlag = false;
    var saveFlag = false;
    var deleteFlag = false;
    var validateFlag = false;
    var editFlag = false;
    var newVersionFlag = false;
    var retroFlag = false;

    claims.forEach(function (FolderClaims) {
        if (FolderClaims.Action.toLowerCase() === "baseline") {
            baseline = true;
        }
        if (FolderClaims.Action.toLowerCase() === "statusupdate") {
            statusUpdate = true;
        }
        if (FolderClaims.Action.toLowerCase() === "versionhistory") {
            versionHistory = true;
        }
        if (FolderClaims.Action.toLowerCase() === "new") {
            newFlag = true;
        }
        if (FolderClaims.Action.toLowerCase() === "save") {
            saveFlag = true;
        }
        if (FolderClaims.Action.toLowerCase() === "deleteinstance") {
            deleteFlag = true;
        }
        if (FolderClaims.Action.toLowerCase() === "validate") {
            validateFlag = true;
        }
        if (FolderClaims.Action.toLowerCase() === "newversion") {
            newVersionFlag = true;
        }
        if (FolderClaims.Action.toLowerCase() === "retro") {
            retroFlag = true;
        }

    });
    if (!baseline) {
        $(elementIDs["btnBaselineJQ"]).addClass('disabled-button');
    }
    else {
        $(elementIDs["btnBaselineJQ"]).removeClass('disabled-button');
    }
    if (!statusUpdate) {
        $(elementIDs["btnStatusUpdateJQ"]).addClass('disabled-button');
    }
    else {
        $(elementIDs["btnStatusUpdateJQ"]).removeClass('disabled-button');

    }
    if (!versionHistory) {
        $(elementIDs["btnVersionHistoryJQ"]).addClass('disabled-button');
    }
    else {
        $(elementIDs["btnVersionHistoryJQ"]).removeClass('disabled-button');
    }
    if (!newFlag) {
        $(elementIDs["btnCreateFormJQ"]).addClass('disabled-button');
        $(elementIDs["btnSOTAddJQ"]).addClass('disabled-button');
    }
    else {
        $(elementIDs["btnCreateFormJQ"]).removeClass('disabled-button');
        $(elementIDs["btnSOTAddJQ"]).removeClass('disabled-button');
    }
    if (!saveFlag) {
        $(elementIDs["btnSaveFormDataJQ"]).addClass('disabled-button');
    }
    else {
        $(elementIDs["btnSaveFormDataJQ"]).removeClass('disabled-button');
    }
    if (!saveFlag) {
        $(elementIDs["btnBottomSaveFormDataJQ"]).addClass('disabled-button');
        $(elementIDs["btnSOTSaveJQ"]).addClass('disabled-button');
    }
    else {
        $(elementIDs["btnBottomSaveFormDataJQ"]).removeClass('disabled-button');
        $(elementIDs["btnSOTSaveJQ"]).removeClass('disabled-button');
    }
    if (!deleteFlag) {
        $(elementIDs["btnDeleteFormInstance"]).addClass('disabled-button');
    }
    else {
        $(elementIDs["btnDeleteFormInstance"]).removeClass('disabled-button');
    }
    if (!validateFlag) {
        $(elementIDs["btnValidateAllJQ"]).addClass('disabled-button');
    }
    else {
        $(elementIDs["btnValidateAllJQ"]).removeClass('disabled-button');
    }
    if (!newVersionFlag) {
        $(elementIDs["btnNewVersionJQ"]).attr('disabled', 'disabled');
    }
    else {
        $(elementIDs["btnNewVersionJQ"]).removeAttr('disabled');
    }
    if (!retroFlag) {
        $(elementIDs["btnRetroJQ"]).attr('disabled', 'disabled');
    }
    else {
        $(elementIDs["btnRetroJQ"]).removeAttr('disabled');
    }
}

//check for folder lock permissions.
function checkFolderLockClaims(elementIDs, claims) {
    var unlockFolderFlag = false;

    claims.forEach(function (FolderClaims) {
        if (FolderClaims.Action.toLowerCase() === "unlockfolder") {
            unlockFolderFlag = true;
        }
    });
    if (!unlockFolderFlag) {
        $(elementIDs["btnUnlockOverrideJQ"]).attr("disabled", "disabled");
    }
    else {
        $(elementIDs["btnUnlockOverrideJQ"]).removeAttr("disabled", "disabled");
    }
}

function checkUnlockFolderClaims(elementIDs, claims) {
    var unlockFolderFlag = false;

    claims.forEach(function (FolderClaims) {
        if (FolderClaims.Action.toLowerCase() === "overridelock") {
            unlockFolderFlag = true;
        }
    });
    if (!unlockFolderFlag) {
        $(elementIDs["btnUnlockJQ"]).attr("hidden", "true");
    }
    else {
        $(elementIDs["btnUnlockJQ"]).removeAttr("hidden", "false");
    }
}

//settings permissions.
function checkSettingsClaims(elementIDs, claims) {
    var checkboxAutoSaveFlag = false;

    claims.forEach(function (autoSaveClaims) {
        if (autoSaveClaims.Action.toLowerCase() === "autosave") {
            checkboxAutoSaveFlag = true;
        }
    });
    if (!checkboxAutoSaveFlag) {
        $(elementIDs["btnSaveJQ"]).attr("disabled", "disabled");
        $(elementIDs["enableAutoSaveChkJQ"]).attr("disabled", "disabled");
        $(elementIDs["duationDDLAutoSaveJQ"]).attr("disabled", "disabled");
    }
    else {
        $(elementIDs["btnSaveJQ"]).removeAttr("disabled", "disabled");
        $(elementIDs["enableAutoSaveChkJQ"]).removeAttr("disabled", "disabled");
    }

}

//DocumentCollateral permissions.
function checkDocumentCollateralClaims(elementIDs, claims) {
    var createCollateralTemplate = false;
    var editCollateralTemplate = false;
    var deleteCollateralTemplate = false;
    var createCollateralTemplateVersion = false;
    var editCollateralTemplateVersion = false;
    var deleteCollateralTemplateVersion = false;
    var finalizedCollateralTemplateVersion = false;
    

    claims.forEach(function (claim) {
        if (claim.Action.toLowerCase() === "createcollateraltemplate") {
            createCollateralTemplate = true;
        }

        if (claim.Action.toLowerCase() === "editcollateraltemplate") {
            editCollateralTemplate = true;
        }

        if (claim.Action.toLowerCase() === "deletecollateraltemplate") {
            deleteCollateralTemplate = true;
        }
        if (claim.Action.toLowerCase() === "createcollateraltemplateversion") {
            createCollateralTemplateVersion = true;
        }

        if (claim.Action.toLowerCase() === "editcollateraltemplateversion") {
            editCollateralTemplateVersion = true;
        }

        if (claim.Action.toLowerCase() === "deletecollateraltemplateversion") {
            deleteCollateralTemplateVersion = true;
        }
        if (claim.Action.toLowerCase() === "finalizedcollateraltemplateversion") {
            finalizedCollateralTemplateVersion = true;
        }
    });

    if (!createCollateralTemplate) {
        $(elementIDs["btnReportTemplateAddJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnReportTemplateAddJQ"]).removeClass("disabled", "disabled");
    }

    if (!editCollateralTemplate) {
        $(elementIDs["btnReportTemplateEditJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnReportTemplateEditJQ"]).removeClass("disabled", "disabled");
    }

    if (!deleteCollateralTemplate) {
        $(elementIDs["btnReportDesignDeleteJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnReportDesignDeleteJQ"]).removeClass("disabled", "disabled");
    }

    if (!createCollateralTemplateVersion) {
        $(elementIDs["btnReportTemplateVersionAddJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnReportTemplateVersionAddJQ"]).removeClass("disabled", "disabled");
    }

    if (!editCollateralTemplateVersion) {
        $(elementIDs["btnReportTemplateVersionEditJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnReportTemplateVersionEditJQ"]).removeClass("disabled", "disabled");
    }

    if (!deleteCollateralTemplateVersion) {
        $(elementIDs["btnReportTemplateVersionDeleteJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnReportTemplateVersionDeleteJQ"]).removeClass("disabled", "disabled");
    }
    if (!finalizedCollateralTemplateVersion) {
        $(elementIDs["btnReportTemplateVersionFinalizedJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnReportTemplateVersionFinalizedJQ"]).removeClass("disabled", "disabled");
    }
}

function checkCollateralGenerationClaims(elementIDs, claims) {
    var queueCollateralTemplate = false;


    claims.forEach(function (claim) {

        if (claim.Action.toLowerCase() === "queuecollateraltemplate") {
            queueCollateralTemplate = true;
        }
    });

    if (!queueCollateralTemplate) {
        $(elementIDs["queueCollateralBtnJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["queueCollateralBtnJQ"]).removeClass("disabled", "disabled");
        $(elementIDs["uploadCollateralBtnJQ"]).removeClass("disabled", "disabled");
    }

}

function checkPDFGenerationClaims(elementIDs, claims) {
    var pdfGenerationAdd = false;
    var pdfGenerationDelete = false;


    claims.forEach(function (claim) {

        if (claim.Action.toLowerCase() === "pdfconfigurationadd") {
            pdfGenerationAdd = true;
        }
        if (claim.Action.toLowerCase() === "pdfconfigurationdelete") {
            pdfGenerationDelete = true;
        }
    });

    if (!pdfGenerationAdd) {
        $(elementIDs["btnFormDesignAddJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnFormDesignAddJQ"]).removeClass("disabled", "disabled");
    }

    if (!pdfGenerationDelete) {
        $(elementIDs["btnDocumentTemplateDeleteJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnDocumentTemplateDeleteJQ"]).removeClass("disabled", "disabled");
    }
}

function checkGlobalUpdateClaims(elementIDs, claims) {
    var globalUpdateAdd = false;
    var globalUpdateEdit = false;
    var globalUpdateUploadIAS = false;
    var globalUpdateCopy = false;


    claims.forEach(function (claim) {

        if (claim.Action.toLowerCase() === "globalupdateadd") {
            globalUpdateAdd = true;
        }
        if (claim.Action.toLowerCase() === "globalupdateedit") {
            globalUpdateEdit = true;
        }
        if (claim.Action.toLowerCase() === "globalupdateuploadias") {
            globalUpdateUploadIAS = true;
        }
        if (claim.Action.toLowerCase() === "globalupdatecopy") {
            globalUpdateCopy = true;
        }
    });

    if (!globalUpdateAdd) {
        $(elementIDs["btnManageGlobalUpdateAddJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnManageGlobalUpdateAddJQ"]).removeClass("disabled", "disabled");
    }

    if (!globalUpdateEdit) {
        $(elementIDs["btnManageGlobalUpdateEditJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnManageGlobalUpdateEditJQ"]).removeClass("disabled", "disabled");
    }

    if (!globalUpdateUploadIAS) {
        $(elementIDs["btnManageGlobalUpdateUploadJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnManageGlobalUpdateUploadJQ"]).removeClass("disabled", "disabled");
    }

    if (!globalUpdateCopy) {
        $(elementIDs["btnManageGlobalUpdateCopyJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnManageGlobalUpdateCopyJQ"]).removeClass("disabled", "disabled");
    }
}

function checkBatchGenerationClaims(elementIDs, claims) {
    var batchAdd = false;
    var batchEdit = false;
    var batchDelete = false;
    var approveBatch = false;
    var realtimeBatchExecution = false;

    claims.forEach(function (claim) {

        if (claim.Action.toLowerCase() === "batchadd") {
            batchAdd = true;
        }
        if (claim.Action.toLowerCase() === "batchedit") {
            batchEdit = true;
        }
        if (claim.Action.toLowerCase() === "batchdelete") {
            batchDelete = true;
        }
        if (claim.Action.toLowerCase() === "approvebatch") {
            approveBatch = true;
        }
        if (claim.Action.toLowerCase() === "realtimebatchexecution") {
            realtimeBatchExecution = true;
        }
    });

    if (!batchAdd) {
        $(elementIDs["btnManageAccountAddJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnManageAccountAddJQ"]).removeClass("disabled", "disabled");
    }

    if (!batchEdit) {
        $(elementIDs["btnManageAccountEditJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnManageAccountEditJQ"]).removeClass("disabled", "disabled");
    }

    if (!batchDelete) {
        $(elementIDs["btnDeleteGuBatchJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnDeleteGuBatchJQ"]).removeClass("disabled", "disabled");
    }

    if (!approveBatch) {
        $(elementIDs["approveBatchJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["approveBatchJQ"]).removeClass("disabled", "disabled");
    }

    if (!realtimeBatchExecution) {
        $(elementIDs["RealtimeBatchExecutionJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["RealtimeBatchExecutionJQ"]).removeClass("disabled", "disabled");
    }
}

function checkBatchExecutionClaims(elementIDs, claims) {
    var downloadAuditReport = false;
    var rollbackBatch = false;

    claims.forEach(function (claim) {

        if (claim.Action.toLowerCase() === "downloadauditreport") {
            downloadAuditReport = true;
        }
        if (claim.Action.toLowerCase() === "rollbackbatch") {
            rollbackBatch = true;
        }
    });

    if (!downloadAuditReport) {
        $(elementIDs["DownloadAuditReportJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["DownloadAuditReportJQ"]).removeClass("disabled", "disabled");
    }

    if (!rollbackBatch) {
        $(elementIDs["RollbackBatchJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["RollbackBatchJQ"]).removeClass("disabled", "disabled");
    }
}

function checkAccountSearchClaims(elementIDs, claims) {
    var btnNonPortfolioAdd = false;
    var btnNonPortfolioEdit = false;
    var btnNonPortfolioCopy = false;
    var btnNonPortfolioRemoveFolder = false;
    
    claims.forEach(function (claim) {

        if (claim.Action.toLowerCase() === "btnnonportfolioadd") {
            btnNonPortfolioAdd = true;
        }
        if (claim.Action.toLowerCase() === "btnnonportfolioedit") {
            btnNonPortfolioEdit = true;
        }
        if (claim.Action.toLowerCase() === "btnnonportfoliocopy") {
            btnNonPortfolioCopy = true; 
        }
        if (claim.Action.toLowerCase() === "btnnonportfolioremovefolder") {
            btnNonPortfolioRemoveFolder = true;
        }
    });

    if (!btnNonPortfolioAdd) {
        $(elementIDs["btnNonPortfolioAddJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnNonPortfolioAddJQ"]).removeClass("disabled", "disabled");
    }

    if (!btnNonPortfolioEdit) {
        $(elementIDs["btnNonPortfolioEditJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnNonPortfolioEditJQ"]).removeClass("disabled", "disabled");
    }

    if (!btnNonPortfolioCopy) {
        $(elementIDs["btnNonPortfolioCopyJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnNonPortfolioCopyJQ"]).removeClass("disabled", "disabled");
    }
    if (!btnNonPortfolioRemoveFolder) {
        $(elementIDs["btnNonPortfolioRemoveFolderJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnNonPortfolioRemoveFolderJQ"]).removeClass("disabled", "disabled");
    }
}

function checkAccountManageClaims(elementIDs, claims) {
    var btnManageAccountAdd = false;
    var btnManageAccountEdit = false;
    var btnManageAccountDelete = false;

    claims.forEach(function (claim) {

        if (claim.Action.toLowerCase() === "btnmanageaccountadd") {
            btnManageAccountAdd = true;
        }
        if (claim.Action.toLowerCase() === "btnmanageaccountedit") {
            btnManageAccountEdit = true;
        }
        if (claim.Action.toLowerCase() === "btnmanageaccountdelete") {
            btnManageAccountDelete = true;
        }
    });

    if (!btnManageAccountAdd) {
        $(elementIDs["btnManageAccountAddJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnManageAccountAddJQ"]).removeClass("disabled", "disabled");
        $(elementIDs["btnManageAccountAddJQ"]).removeClass("ui-state-disabled", "ui-state-disabled");
    }

    if (!btnManageAccountEdit) {
        $(elementIDs["btnManageAccountEditJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnManageAccountEditJQ"]).removeClass("disabled", "disabled");
        $(elementIDs["btnManageAccountEditJQ"]).removeClass("ui-state-disabled", "ui-state-disabled");
    }

    if (!btnManageAccountDelete) {
        $(elementIDs["btnManageAccountDeleteJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnManageAccountDeleteJQ"]).removeClass("disabled", "disabled");
        $(elementIDs["btnManageAccountDeleteJQ"]).removeClass("ui-state-disabled", "ui-state-disabled");
    }
}

function checkReportClaims(elementIDs, claims) {
    var btnGenerateReport = false;
    
    claims.forEach(function (claim) {
        if (claim.Action.toLowerCase() === "btngeneratereport") {
            btnGenerateReport = true;
        }
    });

    if (!btnGenerateReport) {
        $(elementIDs["btnGenerateReport"]).addClass('disabled-button');
    }
    else {
        $(elementIDs["btnGenerateReport"]).removeClass('disabled-button');
    }
}

function checkPortfolioClaims(elementIDs, claims) {
    var btnPortfolioSearchNew = false;
    var btnPortfolioSearchEdit = false;
    var btnPortfolioSearchCopy = false;
    var btnPortfolioSearchView = false;

    claims.forEach(function (claim) {

        if (claim.Action.toLowerCase() === "btnportfoliosearchnew") {
            btnPortfolioSearchNew = true;
        }
        if (claim.Action.toLowerCase() === "btnportfoliosearchedit") {
            btnPortfolioSearchEdit = true;
        }
        if (claim.Action.toLowerCase() === "btnportfoliosearchcopy") {
            btnPortfolioSearchCopy = true;
        }
        if (claim.Action.toLowerCase() === "btnportfoliosearchview") {
            btnPortfolioSearchView = true;
        }
    });

    if (!btnPortfolioSearchNew) {
        $(elementIDs["btnPortfolioSearchNewJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnPortfolioSearchNewJQ"]).removeClass('ui-state-disabled');
    }

    if (!btnPortfolioSearchEdit) {
        $(elementIDs["btnPortfolioSearchEditJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnPortfolioSearchEditJQ"]).removeClass('ui-state-disabled');
    }

    if (!btnPortfolioSearchCopy) {
        $(elementIDs["btnPortfolioSearchCopyJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnPortfolioSearchCopyJQ"]).removeClass('ui-state-disabled');
    }
    if (!btnPortfolioSearchView) {
        $(elementIDs["btnPortfolioSearchViewJQ"]).addClass('ui-state-disabled');
    }
    else {
        $(elementIDs["btnPortfolioSearchViewJQ"]).removeClass('ui-state-disabled');
    }
}

function checkWorkflowSettingsClaims(elementIDs, claims) {
    var workFlowSettinsFlag = false;
    claims.forEach(function (workFlowClaim) {
        if (workFlowClaim.Action.toLowerCase() == "workflowsettings") {
            workFlowSettinsFlag = true;
        }
    });
    if (!workFlowSettinsFlag) {
        $(elementIDs["addUserButton"]).addClass('disabled-button');
        $(elementIDs["removeUserButton"]).addClass('disabled-button');
        $(elementIDs["btnSaveApplicableTeamUsers"]).addClass('disabled-button');
    }
    else {
        $(elementIDs["addUserButton"]).removeClass('disabled-button');
        $(elementIDs["removeUserButton"]).removeClass('disabled-button');
        $(elementIDs["btnSaveApplicableTeamUsers"]).removeClass('disabled-button');
    }
}

function checkUserPermissionNewVersionRetro(sectionName) {
    var sectionData = [];
    var editFlag = false;
    //to check for user permissions.       

    if (claims !== null && claims !== undefined)
        claims.forEach(function (section) {
            var temp = sectionName.split("?");
            sectionName = temp[0];
            var tempString = "/" + section.Resource;
            if (tempString.toLowerCase() === sectionName.toLowerCase()) {
                sectionData.push(section);
            }
        });

    sectionData.forEach(function (claim) {
        if (claim.Action.toLowerCase() === "newversion" || claim.Action.toLowerCase() === "retro") {
            editFlag = true;
        }
    });
    return editFlag;
}


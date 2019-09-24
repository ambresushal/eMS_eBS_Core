var folderVersionInstance = function () {

    //variable to hold the instance
    var instance;
    var URLs = {
        checkIfEVCompletedForFolderversion: '/ExitValidate/CheckExitValidationCompletedForFolderVersion?folderversionId={folderversionId}',
    };
    return {
        //this method will return a singleton object for folderVersion at any point of time.
        getInstance: function (accountId, folderVersionId, folderVersionNo, effectiveDate, folderId, referenceformInstanceId, isEditable, folderType, viewMode, currentUserId, currentUserName) {
            isEditable = isEditable === "True" ? true : false;

            if (instance === undefined)
            {
                WorkFlowState.TaskFolderVersionId = folderVersionId;
                instance = new folderVersion(accountId, folderVersionId, folderVersionNo, effectiveDate, folderId, referenceformInstanceId, isEditable, folderType, viewMode, currentUserId, currentUserName);
            }
            else if (instance.accountId == accountId && instance.folderVersionId == folderVersionId && instance.folderVersionNo === folderVersionNo && instance.effectiveDate === effectiveDate && instance.folderId === folderId && instance.referenceformInstanceId === referenceformInstanceId) {
                return instance;
            }
            else
            {
                instance = undefined;
                instance = new folderVersion(accountId, folderVersionId, folderVersionNo, effectiveDate, folderId, referenceformInstanceId, isEditable, folderType, viewMode, currentUserId, currentUserName);
            }
            return instance;
           
            
        },

        checkIfEVCompletedForFolderversion: function (folderVersionId, folderId, isEditable, folderType, viewMode, currentUserId, currentUserName) {
            isEditable = isEditable === "True" ? true : false;
            var url = URLs.checkIfEVCompletedForFolderversion.replace(/\{folderversionId\}/g, folderVersionId) 
            var promise = ajaxWrapper.getJSONAsync(url);
            //register ajax success callback
            promise.done(function (result) {
                if (result.Result === ServiceResult.SUCCESS)
                {
                    messageDialog.show("Exit Validate for all plans is complete without errors. The folder is ready to be moved to the next workflow step.");
                } else if (result.Result === ServiceResult.FAILURE) {
                    messageDialog.show("Exit Validate for all plans is complete with errors. Please refer Notifications bell for individual plan status to resolve errors.");
                } 
                });
           
            
        }

    }
}();

function folderVersion(accountId, folderVersionId, folderVersionNo, effectiveDate, folderId, referenceformInstanceId, isEditable, folderType, viewMode, currentUserId, currentUserName) {
    this.accountId = accountId;
    this.folderVersionId = folderVersionId;
    this.folderVersionNo = folderVersionNo;
    this.effectiveDate = effectiveDate;
    this.folderId = folderId;
    this.folderType = folderType;
    this.viewMode = viewMode;
    this.referenceformInstanceId = referenceformInstanceId;
    this.currentUserId = currentUserId;
    this.currentUserName = currentUserName;
    this.folderVersionWorkflow = undefined;
    this.folderManager = undefined;

    this.isEditable = isEditable;

    return {
        load: function () {
            //TODO : add code to load the folder version instance data here.
            //TODO: add method calls to load Header, Workflow & tabs sections in the page.

            //load folder version workflow state here
            if (folderType != "MasterList") {
                this.folderVersionWorkflow = folderVersionWorkflowInstance.getInstance(accountId, folderVersionId, folderId);
                this.folderVersionWorkflow.load();
            }

          
            
            //manage form instances here 

            //if ((folderType != "Account") && ((folderType == "MasterType" && folderData.RoleId == Role.SuperUser) || (folderType != "MasterType" && (folderData.RoleId != Role.HNESuperUser || folderData.RoleId != Role.ProductDesigner || folderData.RoleId != Role.ProductSME))))
            //    isEditable = false;
            this.folderManager = folderManager.getInstance(accountId, folderVersionId, folderId, referenceformInstanceId, isEditable, folderType, viewMode, currentUserId, currentUserName);
            this.folderManager.load();
        }
    }
}

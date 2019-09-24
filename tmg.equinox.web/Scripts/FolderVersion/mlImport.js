
var showImportMLExcelDialog = function (formInstanceId, currentInstance, FormInstanceBuilder) {

    //alert(formInstanceId);
    var URLs = {
        //Queue Collateral
        importML: "/MasterList/ImportMasterList",
        getFormInstanceData: "/MasterList/GetFormInstanceData",
        newInstanceML: "/FolderVersion/IndexReloadML"
    };

    var elementIDs = {
        importMLDialog: "#importMLDialog",
        MLcomment: "#MLcomment",
        uploadMLImportFile: "#uploadMLImportFile",
        UploadMLFileName: "#UploadMLFileName",
        UploadMLFileNameSpan: "#UploadMLFileNameSpan",
        CommentMLSpan: "#CommentMLSpan",
        btnMLImport: "#btnMLImport",
        MLImportConfirmDialog: "#MLImportConfirmDialog",
        MLDocumentMenu: ".ml-document-menu",
        SectionMenu: ".section-menu",
        FolderName: "#FolderName",
        SectionName: "#SectionName",
        FormInstanceId: "#FormInstanceId"
    };

    function init() {
        //register dialog for grid row add/edit        
        $(elementIDs.importMLDialog).dialog({
            autoOpen: false,
            height: 300,
            width: 500,
            modal: true,
            close: function (event, ui) {
            }
        });

        $(elementIDs.MLImportConfirmDialog).dialog({
            autoOpen: false,
            modal: true
        });

        //register event for button click on dialog
        $(elementIDs.importMLDialog + ' button').unbind().on('click', function () {
            var file;
            var MLFileName = $(elementIDs.UploadMLFileName).val();
            var MLFileNameUpload = $(elementIDs.UploadMLFileName).get(0);
            var MLFileNamefiles = MLFileNameUpload.files;

            var fileData = new FormData();

            // Adding Comment key to FormData object  
            var MLcommentValue = $(elementIDs.MLcomment).val();
            fileData.append('MLcomment', MLcommentValue);

            // Adding ML Import to FormData object
            if (MLFileNamefiles.length > 0) {
                fileData.append(MLFileNamefiles[0].name, MLFileNamefiles[0]);
            }
           
            if (validate()) {
                $(elementIDs.MLImportConfirmDialog).dialog({
                    buttons: {
                        "Confirm": function () {
                            $(this).dialog("close");
                            saveImport(fileData);
                        },
                        "Close": function () {
                            $(this).dialog("close");
                        }
                    }
                });
                $(elementIDs.MLImportConfirmDialog).dialog("open");
                
            }

            var MLFolderName = $(elementIDs.MLDocumentMenu).val();
            fileData.append('MLFolderName', MLFolderName);
            //alert($(".ml-document-menu").val());
            var MLSectionName = "";
            $(elementIDs.SectionMenu).each(function () {
                MLSectionName = $(this).val();
                //alert($(this).val());

            })
            //alert($(".section-menu").val());
            fileData.append('MLSectionName', MLSectionName);
            fileData.append('MLFormInstanceId', formInstanceId);

        });





        validate = function () {

            var file;
            var allowedExtensions = ["xls", "xlsx","XLS","XLSX"];
            var MLImportFileNameIsValid = false;
            var MLImportCommentIsValid = false;

            var MLImportComment = $(elementIDs.MLcomment).val();

            if (MLImportComment == '') {
                $(elementIDs.MLcomment).parent().addClass('has-error');
                $(elementIDs.MLcomment).addClass('form-control');
                $(elementIDs.CommentMLSpan).text(MLImportMessages.MLCommentRequiredMsg);
                MLImportCommentIsValid = false
            }
            else {
                $(elementIDs.MLcomment).removeClass('form-control');
                $(elementIDs.MLcomment).parent().addClass('has-error');
                $(elementIDs.CommentMLSpan).text('');
                MLImportCommentIsValid = true;
            }

           
            var MLImportFileName = $(elementIDs.UploadMLFileName).val();
            if (MLImportFileName == '') {
                $(elementIDs.UploadMLFileName).parent().addClass('has-error');
                $(elementIDs.UploadMLFileName).addClass('form-control');
                $(elementIDs.UploadMLFileNameSpan).text(MLImportMessages.MLFileRequiredMsg);
                isValid = false;
            }
            else {
                if (MLImportFileName) {
                    var fileN = MLImportFileName.split('.');
                    if ($.inArray(fileN[fileN.length - 1], allowedExtensions) == -1) {
                        messageDialog.show(MLImportMessages.InvalidFileExtensions);
                        MLImportFileNameIsValid = false;
                    }
                    else {
                        file = $(elementIDs.UploadMLFileName)[0].files[0];
                        $(elementIDs.UploadMLFileNameSpan).removeClass('form-control');
                        $(elementIDs.UploadMLFileNameSpan).parent().addClass('has-error');
                        $(elementIDs.UploadMLFileNameSpan).text('');
                        MLImportFileNameIsValid = true;
                        isValid = true;
                    }
                }
            }
            
            return MLImportCommentIsValid && MLImportFileNameIsValid && isValid;
        }

        saveImport = function (fileData) {
            if (validate()) {

                $.ajax({
                    url: URLs.importML,
                    type: "POST",
                    contentType: false, // Not to set any content header  
                    processData: false, // Not to process data  
                    data: fileData,
                    success: function (result) {
                        if (result.hasOwnProperty('FormInstanceId')) {
                            if (result.FormInstanceId > 0) {
                                $(elementIDs.importMLDialog).dialog("close");
                                var Message = result.Message;
                                var SectionName = result.SectionName;
                                var ViewName = result.ViewName;
                                var FormInstanceId = result.FormInstanceId;
                                var FolderId = result.FolderId;
                                var FolderVersionId = result.FolderVersionId;
                                messageDialog.show(MLImportMessages.MLImportSuccessMsg);
                                location.href = '/FolderVersion/GetNonPortfolioBasedAccountFoldersML?tenantId=1&folderVersionId=' + FolderVersionId + '&folderId=' + FolderId + '&folderType=MasterList&mode=true';
                                //location.href = '/FolderVersion/IndexML?tenantId=1&folderVersionId=' + FolderVersionId + '&folderId=' + FolderId + '&FormInstanceId=' + FormInstanceId;
                            }
                            else {
                                messageDialog.show(result.Message);
                            }
                        }
                        else {
                            messageDialog.show(MLImportMessages.MLImportFailedMsg);
                        }
                    },
                    error: function (err) {
                        messageDialog.show(err);
                    }
                });
                
               

            }
        }
    }

    $('#messageDialog').on('hidden.bs.modal', function () {
        window.alert('hidden event fired!');
        location.reload(true);
    });


    init();

    return {
        show: function () {
            var commentValue = $(elementIDs.MLcomment).val('');
            var MLFileName = $(elementIDs.UploadMLFileName).val('');
            $(elementIDs.UploadMLFileName).removeClass('form-control');
            $(elementIDs.UploadMLFileName).parent().addClass('has-error');
            $(elementIDs.UploadMLFileNameSpan).text('');
            $(elementIDs.importMLDialog).dialog('option', 'title', "Master List Import");
            $(elementIDs.importMLDialog).dialog("open");
        }
    }
}

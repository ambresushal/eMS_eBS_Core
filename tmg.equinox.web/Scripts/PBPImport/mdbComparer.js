var mdbComparer = function () {
    var URLs = {
        compareMDB: "/mdbComparer/CompareMDB",
        uploadPlanList: "/mdbComparer/UploadPBPFiles",
        validateMDBfiles: "/mdbComparer/ValidateMDBfiles?importedFileName={importedFileName}&exportedFileName={exportedFileName}"
    };

    var elementIDs = {

        btnMDBCompareJQ: "#btnMDBCompare",

        fileNameJQ: "#fileName",

        importMDBFile: "#importMDBFile",
        importMDBFileName: "#importMDBFileName",
        importMDBFileNameSpan: "#importMDBFileNameSpan",

        exportMDBFile: "#exportMDBFile",
        exportMDBFileName: "#exportMDBFileName",
        exportMDBFileNameSpan: "#exportMDBFileNameSpan",


    };

    var importFileName = "";
    var importedFilePath = "";
    var exportedFilePath = "";
    var exportFileName = "";

    function init() {
        $(document).ready(function () {

            $(elementIDs.btnMDBCompareJQ).click(function () {
                comparemdb();
            });

        });
    }
    init();

    function comparemdb() {
        var importmdbFileNameUpload = $(elementIDs.importMDBFileName).get(0);
        var importMDBFileNamefiles = importmdbFileNameUpload.files;
        if (importMDBFileNamefiles.length <= 0) {
            messageDialog.show('Please choose the imported file.');
            return;
        }
        importedFilePath = importmdbFileNameUpload.value;
        importFileName = importMDBFileNamefiles[0].name;
        var filedata = new FormData();

        // Looping over all files and add it to FormData object  
        for (var i = 0; i < importMDBFileNamefiles.length; i++) {
            filedata.append(importMDBFileNamefiles[i].name, importMDBFileNamefiles[i]);
        }


        var exportmdbFileNameUpload = $(elementIDs.exportMDBFileName).get(0);
        var exportMDBFileNamefiles = exportmdbFileNameUpload.files;
        if (exportMDBFileNamefiles.length <= 0) {
            messageDialog.show('Please choose the exported file.');
            return;
        }
        exportedFilePath = exportmdbFileNameUpload.value;
        exportFileName = exportMDBFileNamefiles[0].name;
        var exportfileData = new FormData();

        // Looping over all files and add it to FormData object  
        for (var i = 0; i < exportMDBFileNamefiles.length; i++) {
            filedata.append(exportMDBFileNamefiles[i].name, exportMDBFileNamefiles[i]);
        }

        if (validate()) {
            var stringData = undefined;
            $.ajax({
                url: URLs.uploadPlanList,
                type: "POST",
                contentType: false, // Not to set any content header  
                processData: false, // Not to process data  
                data: filedata,
                async: false,
                success: function (result) {

                    stringData = "importGenratedFileName=" + result[0];
                    stringData += "<&exportGenratedFileName=" + result[2];
                    stringData += "<&importFileName=" + importFileName;
                    stringData += "<&importedFilePath=" + importedFilePath;
                    stringData += "<&exportFileName=" + exportFileName;
                    stringData += "<&exportedFilePath=" + exportedFilePath;

                    var validationURL = URLs.validateMDBfiles.replace('{importedFileName}', result[0]).replace('{exportedFileName}', result[2]);
                    $.ajax({
                        url: validationURL,
                        type: "POST",
                        contentType: false,
                        processData: false,
                        async: false,
                        success: function (data) {
                            if (data.Result != ServiceResult.SUCCESS) {
                                stringData = undefined;
                                messageDialog.show('MDB files are not valid MDBs.');
                            }
                        },
                        error: function (err) {
                            stringData = undefined;
                            messageDialog.show(err.statusText);
                        }
                    });
                },
                error: function (err) {
                    messageDialog.show(err.statusText);
                }
            });

            if (stringData != undefined) {
                $.downloadNew(URLs.compareMDB, stringData, 'post');
            }
        }
    }

    validate = function () {
        var allowedExtensions = ["MDB", "LDB", "mdb"];
        var mdbFileNameIsValid = false;
        var importedMDBFileName = $(elementIDs.importMDBFileName).val();
        var exportedMDBFileName = $(elementIDs.exportMDBFileName).val();

        if (importedMDBFileName == '') {
            $(elementIDs.importMDBFileName).parent().addClass('has-error');
            $(elementIDs.importMDBFileNameSpan).text("Imported MDB file required.");
            isValid = false
        }
        else {
            if (importedMDBFileName) {
                var fileN = importedMDBFileName.split('.');
                if ($.inArray(fileN[fileN.length - 1], allowedExtensions) == -1) {
                    mdbFileNameIsValid = false;
                    messageDialog.show(PBPImportMessages.InvalidFileExtensions);
                }
                else {
                    file = $(elementIDs.importMDBFileName)[0].files[0];
                    $(elementIDs.importMDBFileName).parent().removeClass('has-error');
                    $(elementIDs.importMDBFileNameSpan).text('');
                    mdbFileNameIsValid = true;
                }
            }
        }

        if (exportedMDBFileName == '') {
            $(elementIDs.exportMDBFileName).parent().addClass('has-error');
            $(elementIDs.exportMDBFileNameSpan).text("Exported MDB file required.");
            isValid = false
        }
        else {
            if (exportedMDBFileName) {
                var fileN = exportedMDBFileName.split('.');
                if ($.inArray(fileN[fileN.length - 1], allowedExtensions) == -1) {
                    mdbFileNameIsValid = false;
                    messageDialog.show(PBPImportMessages.InvalidFileExtensions);
                }
                else {
                    file = $(elementIDs.exportMDBFileName)[0].files[0];
                    $(elementIDs.exportMDBFileName).parent().removeClass('has-error');
                    $(elementIDs.exportMDBFileNameSpan).text('');
                    mdbFileNameIsValid = true;
                }
            }
        }

        if (importedMDBFileName == '' || exportedMDBFileName == '') {
            mdbFileNameIsValid = false;
        }

        return mdbFileNameIsValid;
    }
}



var ProductShareReportForm = function () {

    var isInitialized = false;

    //added in Views/JournalReport/ProductShareReport.cshtml
    var elementIDs = {            
        productShareReportDiv: '#productShareReportDiv',
        productIDJQ: '#productID',        
        SEPYJQ: '#SEPY',        
        DEDEJQ: '#DEDE',        
        LTLTJQ: '#LTLT',        
        productIDHelpBlockJQ: '#productIDHelpBlock',
        SEPYHelpBlockJQ: '#SEPYHelpBlock',
        DEDEHelpBlockJQ: '#DEDEHelpBlock',
        LTLTHelpBlockJQ: '#LTLTHelpBlock',

        //element of button to view report
        btnDownloadProductShareReportJQ: '#btnDownloadProductShareReport',
        btnResetJQ: '#btnReset'
    };
    //this function is called below soon after this JS file is loaded
    function init() {
        $(document).ready(function () {
            if (isInitialized == false) {
                initilizeControls();
                
                $(elementIDs.productIDJQ).on("change", function () {
                    if ($(this).val() != null && $(this).val() != undefined && $(this).val() != "") {
                        $(elementIDs.SEPYJQ).attr('disabled', 'disabled');
                        $(elementIDs.DEDEJQ).attr('disabled', 'disabled');
                        $(elementIDs.LTLTJQ).attr('disabled', 'disabled');

                        if ($(this).val().length == 8) {
                            $(elementIDs.btnDownloadProductShareReportJQ).removeAttr('disabled');
                        }
                        else {
                            $(elementIDs.btnDownloadProductShareReportJQ).attr('disabled', 'disabled');
                        }
                    }
                });
                $(elementIDs.productIDJQ).on("focusout", function () {
                    if ($(this).val().length != 8) {
                        $(elementIDs.btnDownloadProductShareReportJQ).attr('disabled', 'disabled');
                    }
                });
                $(elementIDs.SEPYJQ).on("change", function () {
                    if ($(this).val() != null && $(this).val() != undefined && $(this).val() != "") {
                        $(elementIDs.productIDJQ).attr('disabled', 'disabled');
                        $(elementIDs.DEDEJQ).attr('disabled', 'disabled');
                        $(elementIDs.LTLTJQ).attr('disabled', 'disabled');

                        if ($(this).val().length == 4) {
                            $(elementIDs.btnDownloadProductShareReportJQ).removeAttr('disabled');
                        }
                        else {
                            $(elementIDs.btnDownloadProductShareReportJQ).attr('disabled', 'disabled');
                        }
                    }
                });
                $(elementIDs.SEPYJQ).on("focusout", function () {
                    if ($(this).val().length != 4) {
                        $(elementIDs.btnDownloadProductShareReportJQ).attr('disabled', 'disabled');
                    }
                });
                $(elementIDs.DEDEJQ).on("change", function () {
                    if ($(this).val() != null && $(this).val() != undefined && $(this).val() != "") {
                        $(elementIDs.SEPYJQ).attr('disabled', 'disabled');
                        $(elementIDs.productIDJQ).attr('disabled', 'disabled');
                        $(elementIDs.LTLTJQ).attr('disabled', 'disabled');

                        if ($(this).val().length == 4) {
                            $(elementIDs.btnDownloadProductShareReportJQ).removeAttr('disabled');
                        }
                        else {
                            $(elementIDs.btnDownloadProductShareReportJQ).attr('disabled', 'disabled');
                        }
                    }
                });
                $(elementIDs.DEDEJQ).on("focusout", function () {
                    if ($(this).val().length != 4) {
                        $(elementIDs.btnDownloadProductShareReportJQ).attr('disabled', 'disabled');
                    }
                });
                $(elementIDs.LTLTJQ).on("change", function () {
                    if ($(this).val() != null && $(this).val() != undefined && $(this).val() != "") {
                        $(elementIDs.SEPYJQ).attr('disabled', 'disabled');
                        $(elementIDs.DEDEJQ).attr('disabled', 'disabled');
                        $(elementIDs.productIDJQ).attr('disabled', 'disabled');

                        if ($(this).val().length == 4) {
                            $(elementIDs.btnDownloadProductShareReportJQ).removeAttr('disabled');
                        }
                        else {
                            $(elementIDs.btnDownloadProductShareReportJQ).attr('disabled', 'disabled');
                        }
                    }
                });
                $(elementIDs.LTLTJQ).on("focusout", function () {
                    if ($(this).val().length != 4) {
                        $(elementIDs.btnDownloadProductShareReportJQ).attr('disabled', 'disabled');
                    }
                });
                $(elementIDs.btnDownloadProductShareReportJQ).click(function (e) { return saveProductShareReport(); });
                $(elementIDs.btnResetJQ).click(function (e) { return clearAll(); });
                isInitialized = true;
            }
        });

    }
    //set default setting
    function initilizeControls() {
        $(elementIDs.productIDJQ).removeAttr('disabled');
        $(elementIDs.SEPYJQ).removeAttr('disabled');
        $(elementIDs.DEDEJQ).removeAttr('disabled');
        $(elementIDs.LTLTJQ).removeAttr('disabled');
        $(elementIDs.btnDownloadProductShareReportJQ).attr('disabled', 'disabled');
    }

    //after click Done  
    function saveProductShareReport() {
        var ProductID = $(elementIDs.productIDJQ).val();
        var SEPY = $(elementIDs.SEPYJQ).val();
        var DEDE = $(elementIDs.DEDEJQ).val();
        var LTLT = $(elementIDs.LTLTJQ).val();

        url = '/FolderVersion/GenerateProductShareReport';

        var stringData = "tenantId=" + 1;
        stringData += "<&ProductID=" + ProductID;
        stringData += "<&SEPY=" + SEPY;
        stringData += "<&DEDE=" + DEDE;
        stringData += "<&LTLT=" + LTLT;

        $.download(url, stringData, 'post');
    }

    function clearAll() {
        $(elementIDs.productIDJQ).val("");
        $(elementIDs.SEPYJQ).val("");
        $(elementIDs.DEDEJQ).val("");        
        $(elementIDs.LTLTJQ).val("");
        initilizeControls();
    }

    init();

}();
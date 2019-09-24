var AuditChecklistForm = function () {
    var isInitialized = false;

    //urls to be accessed for create/copy form.
    var URLs = {
        getAuditChecklistDataList: '/Report/GetAuditChecklistDataList?tenantID=1&fromDate={sourceDate}&toDate={targetDate}',
        exportAuditChecklistReport: '/Report/ExportAuditChecklistReport',
    };

    var elementIDs = {
        // table element of Audit checklist Grid      
        auditChecklistDiv: '#auditChecklistDiv',
        //element of an From date Datepicker
        fromDateJQ: '#fromDate',
        //element of an To date Datepicker
        toDateJQ: '#toDate',
        //element of jqgrid 
        documentAuditChecklistGrid: 'documentAuditChecklist',
        //with hash for use with jQuery
        documentAuditChecklistGridJQ: '#documentAuditChecklist',
        fromDateHelpBlockJQ: '#fromDateHelpBlock',
        toDateHelpBlockJQ: '#toDateHelpBlock',
        btnACViewReportFormJQ: '#btnACViewReportForm',
        btnACCancelJQ: '#btnACCancel'
    };

    //this function is called below soon after this JS file is loaded
    function init() {
        $(document).ready(function () {
            if (isInitialized == false) {
                initilizeControls();
                //To display datepicker for folder effective date.
                $(elementIDs.fromDateJQ).datepicker({
                    dateFormat: 'mm/dd/yy',
                    changeMonth: true,
                    changeYear: true,
                    yearRange: 'c-61:c+20',
                    showOn: "both",
                    //CalenderIcon path declare in golbalvariable.js
                    buttonImage: Icons.CalenderIcon,
                    buttonImageOnly: true,
                }).parent().find('img').css('margin-bottom', '7px');

                $(elementIDs.toDateJQ).datepicker({
                    dateFormat: 'mm/dd/yy',
                    changeMonth: true,
                    changeYear: true,
                    yearRange: 'c-61:c+20',
                    showOn: "both",
                    //CalenderIcon path declare in golbalvariable.js
                    buttonImage: Icons.CalenderIcon,
                    buttonImageOnly: true,
                }).parent().find('img').css('margin-bottom', '7px');

                // add on change event for form drop down
                $(elementIDs.fromDateJQ).change(function (e) {
                    $(elementIDs.fromDateHelpBlockJQ).text('');
                    validateDateRange();
                });

                $(elementIDs.toDateJQ).change(function (e) {
                    $(elementIDs.toDateHelpBlockJQ).text('');
                    //check the both drop down has to selected
                    validateDateRange();
                });
                // add click event for Done button
                $(elementIDs.btnACViewReportFormJQ).click(function (e) { return saveAuditChecklistReport(); });
                $(elementIDs.btnACCancelJQ).click(function (e) { return clearAll(); });
                isInitialized = true;
            }
        });
    }

    //set default setting
    function initilizeControls() {
        $(elementIDs.btnACViewReportFormJQ).attr('disabled', 'disabled');
        $(elementIDs.fromDateJQ).removeAttr("disabled");
        $(elementIDs.toDateJQ).removeAttr("disabled");
    }

    //load jqgrid 
    function loadAuditChecklistGrid() {
        var sourceDate = null, targetDate = null
        sourceDate = $(elementIDs.fromDateJQ).val();
        targetDate = $(elementIDs.toDateJQ).val();

        sourceDate = sourceDate.replace(/\//g, '-');
        targetDate = targetDate.replace(/\//g, '-');
        //set column list for grid
        var currentInstance = this;
        var colArray = ['FormDesignID', 'FormInstanceID', 'Document Name', 'Account', 'Folder Name', 'Folder Version', 'Release Date', 'FormData'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'FormDesignID', search: false, width: 10, align: 'left', hidden: true });
        colModel.push({ name: 'FormInstanceID', search: false, width: 10, align: 'left', hidden: true });
        colModel.push({ name: 'DocumentName', search: false, width: 40, align: 'left' });
        colModel.push({ name: 'AccountName', search: false, width: 60, align: 'left' });
        colModel.push({ name: 'FolderName', search: false, width: 70, align: 'left' });
        colModel.push({ name: 'FolderVersionNumber', search: false, width: 40, align: 'left' });
        colModel.push({ name: 'ReleaseDate', search: false, width: 60, align: 'left', formatter: 'date', formatoptions: JQGridSettings.DateTimeFormatterOptions });
        colModel.push({ name: 'FormData', search: false, width: 60, align: 'left', hidden: true });

        //clean up the grid first - only table element remains after this
        $(elementIDs.documentAuditChecklistGridJQ).jqGrid('GridUnload');
        var url = URLs.getAuditChecklistDataList.replace(/\{sourceDate\}/g, sourceDate).replace(/\{targetDate\}/g, targetDate);
        $(elementIDs.documentAuditChecklistGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            height: '240',
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            hidegrid: true,
            altRows: true,
            pager: '#p' + elementIDs.documentAuditChecklistGrid,
            altclass: 'alternate',
            resizeStop: function (width, index) {
                autoResizing(elementIDs.documentAuditChecklistGridJQ);
            }
        });
    }

    //validate controls.
    function validateControls() {
        var isFromDateSelected = false;
        var isToDateSelected = false;

        if ($(elementIDs.fromDateJQ).val() == "") {
            $(elementIDs.fromDateJQ).parent().addClass('has-error');
            $(elementIDs.fromDateHelpBlockJQ).text(AuditChecklist.fromDateRequiredMsg);
            isFromDateSelected = false;
        }
        else {
            $(elementIDs.fromDateHelpBlockJQ).removeClass('form-control');
            $(elementIDs.fromDateJQ).parent().removeClass('has-error');
            $(elementIDs.fromDateHelpBlockJQ).text('');
            isFromDateSelected = true;
        }

        if ($(elementIDs.toDateJQ).val() == "") {
            $(elementIDs.toDateJQ).parent().addClass('has-error');
            $(elementIDs.toDateHelpBlockJQ).text(AuditChecklist.toDateRequiredMsg);
            isToDateSelected = false;
        }
        else {
            $(elementIDs.toDateHelpBlockJQ).removeClass('form-control');
            $(elementIDs.toDateJQ).parent().removeClass('has-error');
            $(elementIDs.toDateHelpBlockJQ).text('');
            isToDateSelected = true;
        }
        return (isFromDateSelected && isToDateSelected)
    }

    //after click Done  
    function saveAuditChecklistReport() {
        var isValid = false;
        //check all input fields for validation.
        isValid = validateControls();

        if (isValid) {
            var allRowData = $(elementIDs.documentAuditChecklistGridJQ).jqGrid('getRowData');
            var sourceDate = $(elementIDs.fromDateJQ).val();
            var targetDate = $(elementIDs.toDateJQ).val();
            var jdata = JSON.stringify(allRowData);
            var encodeString = encodeURIComponent(jdata);
            url = URLs.exportAuditChecklistReport;
            var stringData = "tenantId=" + 1;
            stringData += "<&fromDate=" + sourceDate;
            stringData += "<&toDate=" + targetDate;
            stringData += "<&encodeString=" + encodeString;

            $.downloadNew(url, stringData, 'post');
        }
    }

    function validateDateRange() {
        var sourceDate = null, targetDate = null
        sourceDate = $(elementIDs.fromDateJQ).val();
        targetDate = $(elementIDs.toDateJQ).val();
        var d1 = Date.parse(sourceDate);
        var d2 = Date.parse(targetDate);

        if ((sourceDate == "") && (targetDate != null && targetDate != "")) {
            $(elementIDs.fromDateJQ).parent().addClass('has-error');
            $(elementIDs.fromDateHelpBlockJQ).text(AuditChecklist.fromDateRequiredMsg);
            isFromDateSelected = false;
        }
        else {
            $(elementIDs.fromDateHelpBlockJQ).removeClass('form-control');
            $(elementIDs.fromDateJQ).parent().removeClass('has-error');
            $(elementIDs.fromDateHelpBlockJQ).text('');
        }
        if ((sourceDate != null && sourceDate != "") && (targetDate != null && targetDate != "")) {
            if (d1 > d2) {
                $(elementIDs.fromDateJQ).parent().addClass('has-error');
                $(elementIDs.fromDateHelpBlockJQ).text(AuditChecklist.fromDateCompareMsg);
                isFromDateSelected = false;
            }
            else {
                $(elementIDs.fromDateHelpBlockJQ).removeClass('form-control');
                $(elementIDs.fromDateJQ).parent().removeClass('has-error');
                $(elementIDs.fromDateHelpBlockJQ).text('');
                $(elementIDs.btnACViewReportFormJQ).removeAttr("disabled");
                loadAuditChecklistGrid();
            }
        }
    }

    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    function clearAll() {
        $(elementIDs.fromDateJQ).datepicker('setDate', null);
        $(elementIDs.toDateJQ).datepicker('setDate', null);
        $(elementIDs.btnACViewReportFormJQ).attr('disabled', 'disabled');
        $(elementIDs.documentAuditChecklistGridJQ).jqGrid("GridUnload");
        initilizeControls();
    }

    init();
}();
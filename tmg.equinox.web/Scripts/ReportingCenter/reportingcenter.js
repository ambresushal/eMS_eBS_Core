var reportList = function () {

    var URLs = {
        reportList: '/ReportingCenter/GetReportList',
    };

    var elementIDs = {
        //table element for the Work Queue Grid 
        reportListGrid: 'reportList',
        //with hash for use with jQuery
        reportListGridJQ: '#reportList',
        btnUnlockJQ: '#btnUnlock',
        btnGenerateReport: '#btnGenerateReport',
        btnPopulateData:'#btnPopulateData'
    };

    if (typeof vbIsFolderLockEnable === "undefined") { this.isFolderLockEnable = false } else { this.isFolderLockEnable = vbIsFolderLockEnable };

    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    function init() {
        $(document).ready(function () {
            loadreportListGrid();
        });
    }

    function loadreportListGrid() {
        //set column list for grid
        var colArray = ['ReportId', 'Report Name'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'ReportId', index: 'ReportId', hidden: true, align: 'left', search: false });
        colModel.push({ name: 'ReportName', index: 'ReportName', hidden: false, search: true, width: 320 });



        //clean up the grid first - only table element remains after this
        $(elementIDs.reportListGridJQ).jqGrid('GridUnload');

        //adding the pager element
        $(elementIDs.reportListGridJQ).parent().append("<div id='p" + elementIDs.reportListGrid + "'></div>");

        var url = URLs.reportList;

        $(elementIDs.reportListGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Select Report',
            height: '290',
            rowNum: 30,
            ignoreCase: true,
            loadonce: true,
            viewrecords: true,
            hidegrid: true,
            altRows: true,
            //pager: '#p' + elementIDs.reportListGrid,
            altclass: 'alternate',
            rowList: [10, 20, 30],
            pgbuttons: false,
            pgtext: "",
            pginput: false,
            multiselect: true,
            onPaging: function (pgButton) {
                if (pgButton === "user" && !IsEnteredPageExist(elementIDs.reportListGrid)) {
                    return "stop";
                }
            },
            gridComplete: function () {
                //to check for claims.             
                var objMap = {
                    edit: "#btnreportListEdit",
                    view: "#btnreportListView"
                };
                checkApplicationClaims(claims, objMap, URLs.reportList);
                checkReportClaims(elementIDs, claims);
            },
            onSelectRow: function (rowId) {

            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.reportListGridJQ);
            },
            jsonReader: {
                page: function (obj) { return obj.records == 0 ? "0" : obj.page; },
            }
        });
        //$(".ui-jqgrid-htable").hide();
        $("#reportList_cb").css("width", "35px")
        var pagerElement = '#p' + elementIDs.reportListGrid;
        $('#p' + elementIDs.reportListGrid).find('input').css('height', '20px');

        $(elementIDs.reportListGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: true,
            defaultSearch: "cn",
        });
        //remove default buttons
        $(elementIDs.reportListGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false, refresh: false });


        // add filter toolbar to the top
        //$(elementIDs.formDesignGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
    }

    //initialization of the DashBoard Grid when the YourreportList function is loaded in browser and invoked
    init();

    return {
        loadreportList: function () {
            loadreportListGrid();
        }
    }
}();

var folderList = function () {

    var URLs = {
        folderList: '/FolderVersion/GetAllFoldersList',
        folderVersionList: '/FolderVersion/GetFolderVersionList'
    };

    var elementIDs = {
        //table element for the Form Update Grid 
        folderListGrid: 'folderList',
        //with hash for use with jQuery
        folderListGridJQ: '#folderList',
    };

    function init() {
        $(document).ready(function () {
            //load the Document update grid
            loadfolderListGrid();

            getFoldersData();

            $('#SelectAll').click(function () {
                var gridData = $(elementIDs.folderListGridJQ).getGridParam('data');
                for (i = 0; i < gridData.length; i++) {
                    if ($('#SelectAll').is(':checked')) {
                        gridData[i].Select = true;
                    }
                    else {
                        gridData[i].Select = false;
                    }
                    $(elementIDs.folderListGridJQ).jqGrid('editRow', gridData[i]._id_, true);
                    $(elementIDs.folderListGridJQ).jqGrid('setRowData', gridData[i]._id_, gridData[i]);
                    $(elementIDs.folderListGridJQ).jqGrid("saveRow", gridData[i]._id_, gridData[i]);
                }
            });
        });
    }

    function VersionList(cellvalue, options, rowObject) {
        //var folderId = $(elementIDs.folderListGridJQ).jqGrid('getCell', rowObject, 'FolderId');
        selectList = loadFolderVersionList(options.rowId);
        return selectList;
    }


    var getFoldersData = function () {
        var promise = ajaxWrapper.postAsyncJSONCustom(URLs.folderList);
        promise.done(function (list) {
            if (list.length > 0) {
                localStorage.setItem('added-items', JSON.stringify(list));
            }
        });
    }

    //Comparer Function  //Sort by id desc
    function GetSortOrder(prop) {
        return function (a, b) {
            if (a[prop] < b[prop]) {
                return 1;
            } else if (a[prop] > b[prop]) {
                return -1;
            }
            return 0;
        }
    }

    var loadFolderVersionListByAjax = function (folderId) {
        var tenantId = "1";
        var jsonData = {
            tenantId: tenantId,
            folderId: folderId
        };
        var FolderVersionList = "<select id='fv_" + folderId + "'>";
        var promise = ajaxWrapper.postAsyncJSONCustom(URLs.folderVersionList, jsonData);
        //fill the folder list drop down
        promise.done(function (list) {

            if (list.length > 0) {
                jQuery.each(list, function (index, item) {
                    FolderVersionList += "<option value='" + item.FolderVersionId + "'>" + item.FolderVersionNumber + "</option>";
                });
            }
            FolderVersionList = FolderVersionList.substr(0, FolderVersionList.length - 1);
            FolderVersionList = FolderVersionList + "</select>";

        });
        return FolderVersionList;
    }



    var loadFolderVersionList = function (folderId) {

        // READ STRING FROM LOCAL STORAGE
        var retrievedObject = localStorage.getItem('added-items');
        var FolderVersionList = "";

        // CONVERT STRING TO REGULAR JS OBJECT
        var parsedObject = JSON.parse(retrievedObject);
        FolderVersionList = "<select id='fv_" + folderId + "' class='selectChange'>";
        var folderVersions;
        var searchField = "FolderId";
        var searchVal = folderId;
        if (parsedObject.length > 0) {
            for (var i = 0 ; i < parsedObject.length ; i++) {
                if (parsedObject[i][searchField] == searchVal) {
                    folderVersions = parsedObject[i].FolderVersions
                }
            }

            folderVersions.sort(GetSortOrder("FolderVersionId"))

            if (folderVersions.length > 0) {
                jQuery.each(folderVersions, function (index, item) {
                    FolderVersionList += "<option value='" + item.FolderVersionId + "'>" + item.FolderVersionNumber + "</option>";
                });
            }
            FolderVersionList = FolderVersionList.substr(0, FolderVersionList.length - 1);
            FolderVersionList = FolderVersionList + "</select>";
        }
        else {
            FolderVersionList = loadFolderVersionListByAjax(folderId);
        }

        return FolderVersionList;
    }





    function loadfolderListGrid() {

        //set column list for grid
        var colArray = ['Folder Id', '<input type="Checkbox" id="SelectAll" alt="Select All">', 'Folder Name', 'FolderVersionId', 'Version Number'];

        //set column models
        var colModel = [];
        colModel.push({ key: true, name: 'FolderId', index: 'FolderId', hidden: true, align: 'left', search: false });
        colModel.push({
            key: false, name: 'Select', index: 'Select', editable: true, search: false, hidden: true,
            formatter: 'checkbox', editoptions: { value: 'true:false' }, sortable: false,
            formatoptions: { disabled: false, }, align: 'center', width: '10'
        });
        colModel.push({ key: false, name: 'FolderName', index: 'FolderName', editable: false, align: 'left', search: true });
        colModel.push({ key: false, name: 'FolderVersionId', index: 'FolderVersionId', hidden: true, align: 'left', search: false });
        colModel.push({
            name: 'FolderVersionNumber',
            search: false,
            index: 'FolderVersionNumber', width: '50', align: 'left', edittype: "select", formatter: VersionList
        });

        //clean up the grid first - only table element remains after this
        $(elementIDs.folderListGridJQ).jqGrid('GridUnload');

        //adding the pager element
        $(elementIDs.folderListGridJQ).parent().append("<div id='p" + elementIDs.folderListGrid + "'></div>");

        var url = URLs.folderList;
        var selectedRows = {};
        var selectedFolderVersions = {};

        $(document).on('change', 'select', function () {
            var id = $(this).attr('id');
            id = id.replace('fv_', '');
            selectedFolderVersions[id] = $(this).val();
        });

        $(elementIDs.folderListGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Select Folder',
            rowNum: 300,
            rowList: [300, 600, 900],
            viewrecords: true,
            pager: '#p' + elementIDs.folderListGrid,
            height: '280',
            autowidth: true,
            loadonce: true,
            ignoreCase: true,
            hidegrid: true,
            hiddengrid: false,
            altRows: true,
            altclass: 'alternate',
            searchOnEnter: false,
            //this is added for pagination.
            multiselect: true,
            // to save selection state
            onSelectAll: function (rowIds, status) {
                if (status === true) {
                    for (var i = 0; i < rowIds.length; i++) {
                        selectedRows[rowIds[i]] = true;
                        selectedFolderVersions[rowIds[i]] = $("#fv_" + rowId).val();
                    }
                } else {
                    for (var i = 0; i < rowIds.length; i++) {
                        delete selectedRows[rowIds[i]];
                        delete selectedFolderVersions[rowIds[i]];
                    }
                }
            },
            onSelectRow: function (rowId, status, e) {
                if (status === false) {
                    delete selectedRows[rowId];
                    delete selectedFolderVersions[rowId];
                } else {
                    selectedRows[rowId] = status;
                    if (typeof selectedFolderVersions[rowId] == "undefined")
                        selectedFolderVersions[rowId] = $("#fv_" + rowId).val();
                }

            },
            gridComplete: function () {
                for (var rowId in selectedRows) {
                    $(elementIDs.folderListGridJQ).setSelection(rowId, true);
                    $("#fv_" + rowId).val(selectedFolderVersions[rowId]);
                }
            },

            beforeSelectRow: function (rowid, e) {
                var $self = $(this),
                    iCol = $.jgrid.getCellIndex($(e.target).closest("td")[0]),
                    cm = $self.jqGrid("getGridParam", "colModel"),
                    localData = $self.jqGrid("getLocalRow", rowid);
                localData.FolderVersionId = $("#fv_" + rowid).val();
                if (cm[iCol].name === "Select" && e.target.tagName.toUpperCase() === "INPUT") {
                    // set local grid data
                    localData.Select = $(e.target).is(":checked");
                }

                return true; // allow selection
            }

        });

        //remove default buttons
        $(elementIDs.folderListGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false, refresh: true });



        $("#folderList_cb").css("width", "35px");
        var pagerElement = '#p' + elementIDs.folderListGrid;
        $('#p' + elementIDs.folderListGrid).find('input').css('height', '20px');

        $(elementIDs.folderListGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: true,
            defaultSearch: "cn",
        });

        //remove default buttons
        $(elementIDs.folderListGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false, refresh: true });
        $(elementIDs.folderListGridJQ).jqGrid('navGrid', pagerElement, {
            beforeRefresh: function () {
                selectedRows = {};
                selectedFolderVersions = {};
            }
        });

    }

    //initialization of the DashBoard Grid when the Form Update function is loaded in browser and invoked
    init();

    return {
        loadfolderList: function () {
            loadfolderListGrid();
        }
    }
}();

var generateReport = function () {

    var elementIDs = {
        btnGenerateReport: "#btnGenerateReport",
        btnPopulateData:'#btnPopulateData',
        btnGenerateSchema: "#btnGenerateSchema",
        btnExecuteODMBatch: "#btnExecuteODMBatch",
        errorReport: "#errorReport",
        errorFolder: "#errorFolder",
        //table element for the Form Update Grid 
        reportQueueGrid: 'reportQueue',
        //with hash for use with jQuery
        reportQueueGridJQ: '#reportQueue',
        reportListGridJQ: '#reportList',
        folderListGridJQ: '#folderList'
    }

    var URL = {
        addToReportQueue: "/ReportingCenter/AddReportQueue",
        getReportQueue: "/ReportingCenter/GetReportQueueList",
        DownloadFile: "/ReportingCenter/Download",
        generateSchema: "/ReportingCenter/GenerateSchema",
        populateData: "/ReportingCenter/PopulateData",
    }

    var timer = 60;
    this.interval = 1000 * 60 * 1;
    this.refreshInterval;



    $(elementIDs.btnGenerateReport).click(function (e) {

        var elementData = {
            reportData: getSelectedReports(),
            folderData: getSelectedFolders(),
            folderVersionData: getselectedFolderVersions()
        }

        //Report grid data
        function getSelectedReports() {
            var $grid = $(elementIDs.reportListGridJQ), selIds = $grid.jqGrid("getGridParam", "selarrrow"), i, n,
            reportIds = [];
            for (i = 0, n = selIds.length; i < n; i++) {
                reportIds.push($grid.jqGrid("getCell", selIds[i], "ReportId"));
            }
            return reportIds;
        }

        //Folder grid data
        function getSelectedFolders() {
            var $grid = $(elementIDs.folderListGridJQ), selIds = $grid.jqGrid("getGridParam", "selarrrow"), i, n,
            folderIds = [];
            for (i = 0, n = selIds.length; i < n; i++) {
                folderIds.push($grid.jqGrid("getCell", selIds[i], "FolderId"));
            }
            return folderIds;
        }


        //FolderVersion grid data
        function getselectedFolderVersions() {
            var $grid = $(elementIDs.folderListGridJQ), selIds = $grid.jqGrid("getGridParam", "selarrrow"), i, n,
            folderVersionIds = [];
            for (i = 0, n = selIds.length; i < n; i++) {
                folderVersionIds.push(getSelectedFolderVersionValue($grid.jqGrid("getCell", selIds[i], "FolderId")));
            }
            return folderVersionIds;
        }


        function getSelectedFolderVersionValue(selId) {
            var folderVersionId = $("#fv_" + selId).val();
            return folderVersionId;
        }


        createDataForProcessing();

        function validateSettings() {
            var isValid = false;

            var folders = elementData.folderData;
            var reports = elementData.reportData;

            if (reports.length === 0) {
                $(elementIDs.errorReport).text('Please select a Report.');
                isValid = false;
            }
            else {
                $(elementIDs.errorReport).text('');
            }
            if (folders.length === 0) {
                $(elementIDs.errorFolder).text('Please select a Folder and Folder Version.');
                isValid = false;
            }
            else {
                $(elementIDs.errorFolder).text('');
            }

            if (reports != "" && folders != "") {
                isValid = true;
            }

            return isValid;
        }

        function createDataForProcessing() {
            if (validateSettings()) {
                var reportArr = elementData.reportData;
                var formInstanceArr = [];
                for (var i = 0; i < reportArr.length; i++) {
                    var folderArr = elementData.folderData;
                    var folderVersionArr = elementData.folderVersionData;
                    var model = createViewModel(reportArr[i], folderArr, folderVersionArr);
                    saveReportFolderRow(model);
                }
            }
        }

        function saveReportFolderRow(model) {
            var viewModel = model;
            var promise = ajaxWrapper.postJSON(URL.addToReportQueue, viewModel);
            //callback function for ajax request success.
            promise.done(function (xhr) {
                if (xhr.Result === ServiceResult.SUCCESS) {
                    messageDialog.show('Report added to Queue');
                    jQuery("#reportQueue").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
                }
                else {
                    messageDialog.show(Common.errorMsg);
                }
            });
        }

        function createViewModel(reportId, folderIdArr, folderVersionIdArr) {
            var modelData = {
                ReportId: reportId,
                FolderId: folderIdArr,
                FolderVersionId: folderVersionIdArr
            }
            return modelData;
        }

    });

    function init() {
        $(document).ready(function () {
            //load the Document update grid
            loadreportQueueGrid();
            registerEvents()
        });
    }
    //initialization of the DashBoard Grid when the Form Update function is loaded in browser and invoked
    init();
    function registerEvents() {
        //load grid on the tab switch event
        $(".WFCATLIST").unbind('click');
        $(".WFCATLIST").click(function () {
            loadreportQueueGrid();
            //show the selected tab
            $('#reportingCentTab').tabs({
                selected: 1
            });
        });
    }
    function loadreportQueueGrid() {
        //set column list for grid
        var colArray = ['Folder Name', 'Report Name', 'Requestor', 'Processed Date & Time', 'Status', 'Download Report', 'ErrorMessage'];

        //set column models
        var colModel = [];
        colModel.push({ key: true, name: 'ReportQueueId', index: 'ReportQueueId', hidden: false, align: 'center', width: '60', search: false, formatter: queueDetailPopupLinkFormatter });
        colModel.push({ key: false, name: 'ReportName', index: 'ReportName', align: 'left', search: true });
        colModel.push({ key: false, name: 'AddedBy', index: 'AddedBy', align: 'left', search: true });
        colModel.push({ key: false, name: 'AddedDate', index: 'AddedDate', align: 'left', search: true, formatter: 'date', formatoptions: JQGridSettings.DateTimeFormatterOptions, align: 'center' });
        colModel.push({ key: false, name: 'Status', index: 'Status', align: 'left', search: true, formatter: processStatusFormmater });
        colModel.push({ key: false, name: 'FileName', index: 'FileName', search: false, editable: false, width: '80', formatter: downloadExcelFileFormmater });
        colModel.push({ key: false, name: 'ErrorMessage', index: 'ErrorMessage', hidden: true, align: 'left', search: false });



        //{ key: false, name: 'Gender', index: 'Gender', editable: true, edittype: 'select', editoptions: { value: { 'M': 'Male', 'F': 'Female', 'N': 'None' } } },
        //clean up the grid first - only table element remains after this
        $(elementIDs.reportQueueGridJQ).jqGrid('GridUnload');

        //adding the pager element
        $(elementIDs.reportQueueGridJQ).parent().append("<div id='p" + elementIDs.reportQueueGrid + "'></div>");

        var url = URL.getReportQueue;

        $(elementIDs.reportQueueGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Recently Generated Reports',
            rowNum: 30,
            height: '280',
            autowidth: true,
            loadonce: false,
            viewrecords: true,
            ignoreCase: true,
            hidegrid: true,
            hiddengrid: false,
            altRows: true,
            pager: '#p' + elementIDs.reportQueueGrid,
            altclass: 'alternate',
            //this is added for pagination.
            rowList: [10, 20, 30],
            sortname: 'ReportQueueId',
            sortorder: "desc"
        });

        var pagerElement = '#p' + elementIDs.reportQueueGrid;
        $('#p' + elementIDs.reportQueueGrid).find('input').css('height', '20px');

        $(elementIDs.reportQueueGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: true,
            defaultSearch: "cn",
        });



        //remove default buttons
        $(elementIDs.reportQueueGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false, refresh: true });

    }

    refreshInterval = setInterval(AutorefreshQueuedReports, 1000);

    function AutorefreshQueuedReports() {
        timer--;
        if (timer == 0) {
            //loadreportQueueGrid();
            jQuery("#reportQueue").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            timer = 60;
        }
        //else {
        $("#spnTimer").text(timer + " seconds.");
        //}
    }

    function showDateTime(cellvalue, options, rowObject) {
        if (cellvalue.length > 0)
            cellvalue = cellvalue.slice(0, -4).replace('T', ' ');
        return cellvalue;
    }

    String.prototype.replaceAll = function (search, replacement) {
        var target = this;
        return target.split(search).join(replacement);
    };

    function downloadExcelFileFormmater(cellvalue, options, rowObject) {
        if (rowObject.Status === "Failed") {
            if (rowObject.ErrorMessage != null) {
                //var error = rowObject.ErrorMessage.replaceAll("(", "[");
                //error = error.replaceAll(")", "]");
                //if (error.length > 200) error = error.substr(0, 150) + '...';
                //errorMsg += error.replace(/["']/g, "&quot");
                var error = "There was an error in generating the report. Please queue the report again for generation.";
                error = error.replaceAll(" ", "&nbsp;");
                var errorMsg = "onclick=javascript:errorLogDialog.show('";
                errorMsg += error
                errorMsg += "');";

                cellvalue = "<a href='javascript:void(0);' " + errorMsg + " style='cursor:hand'><img src='/Content/css/custom-theme/images/error.png' class='DownloadQueuedCollateral' alt='Error' style='margin-left: 5px;'  title = 'Error Message' /></a>";
            }
            else
                cellvalue = "<a href='javascript:void(0);' onclick=javascript:messageDialog.show('Some_error_occured.'); style='cursor:hand'><img src='/Content/css/custom-theme/images/error.png' class='DownloadQueuedCollateral' alt='Error' style='margin-left: 5px;'  title = 'Error Message' /></a>";
        }
        else
            if (rowObject.FileName != null)
                cellvalue = "<a href='/ReportingCenter/Download?ReportQueueId=" + rowObject.ReportQueueId + "'><img src='/Content/css/custom-theme/images/excel.png' class='DownloadQueuedCollateral' alt='Excel' style='margin-left: 5px;'  title = 'Excel Download' data-Id='" + rowObject.ReportQueueId + '.xlsx' + "'/></a>";
            else
                cellvalue = "<img src='/Content/css/custom-theme/images/excel.png' class='DownloadQueuedCollateral' alt='Excel' style='margin-left: 5px;'  title = 'Excel Download' data-Id='" + rowObject.ReportQueueId + '.xlsx' + "'/>";
        return cellvalue;
    }
    
    function queueDetailPopupLinkFormatter(cellvalue, options, rowObject) {
        cellvalue = "<span title='Folder Details' style='align:center; width:auto !important;' class='DownloadAccessFile ui-icon ui-icon-extlink' onclick=\"queueDetailPopup.show(" + rowObject.ReportQueueId + ");\"></span>";
        return cellvalue;
    }

    $(elementIDs.btnGenerateSchema).click(function () {
        var promise = ajaxWrapper.postJSON(URL.generateSchema);
        //callback function for ajax request success.
        promise.done(function (xhr) {
            if (xhr.Result === ServiceResult.SUCCESS) {
                messageDialog.show('Schema has been generated successfully!');
            }
            else {
                messageDialog.show("Something went wrong!");
            }
        });
    });
    $(elementIDs.btnPopulateData).click(function () {
        var promise = ajaxWrapper.postJSON(URL.populateData);
        //callback function for ajax request success.
        promise.done(function (xhr) {
            if (xhr.Result === ServiceResult.SUCCESS) {
                messageDialog.show('Data populated successfully!');
            }
            else {
                messageDialog.show("Something went wrong!");
            }
        });
    });

    $(elementIDs.btnExecuteODMBatch).click(function () {
        var batchLink = "/ReportingCenter/ExecuteODMBatch?batchId=1";
        var promise = ajaxWrapper.postJSON(batchLink);
        //callback function for ajax request success.
        promise.done(function (xhr) {
            messageDialog.show('Schema has been generated successfully!');
        });
    });

    // Function to set font colour based on status using formatter 
    function processStatusFormmater(cellvalue, options, rowObject) {
        switch (cellvalue) {
            case "Enqueued":
                // If status is Inprogress then display status text in black color 
                cellvalue = '<span style="color:blue">' + cellvalue + '</span>';
                break;
            case "Processing":
                // If status is Complete then display status text in green color
                cellvalue = '<span style="color:orange">' + cellvalue + '</span>';
                break;
            case "Succeeded":
                // If status is Complete then display status text in green color
                cellvalue = '<span style="color:green">' + cellvalue + '</span>';
                break;
            case "Failed":
                // If status is Errored then display status text in red color 
                cellvalue = '<span style="color:red">' + cellvalue + '</span>';
                break;
        }
        return cellvalue;
    } // processStatusFormmater


    return {
        loadreportQueue: function () {
            loadreportQueueGrid();
        }
    }

}();

var queueDetailPopup;
queueDetailPopup = queueDetailPopup || (function () {

    var elementIDs = {
        queueDetailDialog: "#queueDetailDialog",
        queueDetails: 'queueDetails',
        queueDetailsJQ: '#queueDetails',
    }

    var URLs = {
        getFolderList: '/ReportingCenter/GetReportQueueFolderDetailsList?queueId={queueId}',
    }

    $(function () {
        $(elementIDs.queueDetailDialog).dialog({
            modal: true,
            autoOpen: false,
            draggable: true,
            resizable: true,
            height: 400,
            width: 900,
            zIndex: 1005,
            closeOnEscape: false,
            title: 'Queue Details',
            buttons: {
                Close: function () {
                    $(this).dialog("close");
                }
            }
        });
    });

    function loadQueueDetailGrid(queueId) {
        var colArray = ['Folder Id', 'Folder Name', 'Version Number'];

        var colModel = [];
        colModel.push({ key: true, name: 'FolderId', index: 'FolderId', hidden: true });
        colModel.push({ key: false, name: 'FolderName', index: 'FolderName', align: 'left', search: true });
        colModel.push({ key: false, name: 'FolderVersionNumber', index: 'FolderVersionNumber', align: 'left', search: true });

        $(elementIDs.queueDetailsJQ).jqGrid('GridUnload');

        $(elementIDs.queueDetailsJQ).parent().append("<div id='p" + elementIDs.queueDetails + "'></div>");

        var url = URLs.getFolderList.replace('{queueId}', queueId);

        $(elementIDs.queueDetailsJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Folder Details',
            rowNum: 30,
            height: '300',
            autowidth: true,
            loadonce: true,
            viewrecords: true,
            ignoreCase: true,
            hidegrid: true,
            hiddengrid: false,
            altRows: true,
            pager: '#p' + elementIDs.queueDetails,
            altclass: 'alternate',
            rowList: [10, 20, 30],
            sortname: 'FolderName'
        });

        var pagerElement = '#p' + elementIDs.queueDetails;
        $('#p' + elementIDs.queueDetails).find('input').css('height', '20px');

        $(elementIDs.queueDetailsJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: true,
            defaultSearch: "cn",
        });

        $(elementIDs.queueDetailsJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false, refresh: true });

    }

    return {
        show: function (queueId) {
            loadQueueDetailGrid(queueId);
            $(elementIDs.queueDetailDialog).dialog('open');
        },
        hide: function () {
            $(elementIDs.queueDetailDialog).dialog('close');
        },
    };
})();

var errorLogDialog;
errorLogDialog = errorLogDialog || (function () {
    $(function () {
        $('#errorLogDialog').dialog({
            modal: true,
            autoOpen: false,
            draggable: true,
            resizable: true,
            height: 200,
            zIndex: 1005,
            closeOnEscape: false,
            title: 'Error Details',
            buttons: {
                Close: function () {
                    $(this).dialog("close");
                }
            }
        });
    });
    return {
        show: function (message) {
            $('#errorLogDialog').html(message);
            $('#errorLogDialog').dialog('open');
        },
        hide: function () {
            $('#errorLogDialog').dialog('close');
        },
    };
})();

$(document).ready(function () {
    $("#jqgh_folderList_Select").removeClass("ui-jqgrid-sortable");
});

var reportSync = function () {

    var elementIDs = {
        //table element for the Form Update Grid 
        reportSyncLogGrid: 'reportSyncLog',
        //with hash for use with jQuery
        reportSyncLogGridJQ: '#reportSyncLog',
        reportLogDialog: '#ReportLogDialog',
        lblerror: '#lblerror',
        lblerrordesc: '#lblerrordesc'
    }

    var URL = {
        getReportSyncData: "/ReportingCenter/GetReportSyncDataLog",
        mDMErrorData: '/ReportingCenter/GetMDMErrorData',
    }

    var timer = 60;
    this.interval = 1000 * 60 * 1;
    this.refreshInterval;




    function init() {
        //  $(document).ready(function () {
        //load the Document update grid
        loadDataSyncGrid();
        //       registerEvents()
        //  });
    }
    init();

    function registerEvents() {
        //load grid on the tab switch event
        $(".WFCATLIST").unbind('click');
        $(".WFCATLIST").click(function () {
            loadDataSyncGrid();
            //show the selected tab
            $('#reportingCentTab').tabs({
                selected: 1
            });
        });
    }

    function loadDataSyncGrid() {
        //set column list for grid
        var colArray = ['FormInstanceId', 'Name', 'Folder Name', 'View Name','Version', 'Effective Date', 'Status', 'Processed Date & Time', 'Process Type'];

        //set column models
        var colModel = [];
        colModel.push({ key: true, name: 'FormInstanceId', index: 'FormInstanceId', hidden: true, align: 'left', search: false });
        colModel.push({ key: false, name: 'Name', index: 'Name', align: 'left', search: true });
        colModel.push({ key: false, name: 'FolderName', index: 'FolderName', align: 'left', search: true });
        colModel.push({ key: false, name: 'ViewType', index: 'ViewType', align: 'left', search: true });
        colModel.push({ key: false, name: 'Version', index: 'Version', search: true, editable: false, width: '80' });
        colModel.push({ key: false, name: 'EffectiveDate', index: 'EffectiveDate', align: 'left', search: true, formatter: 'date', formatoptions: JQGridSettings.DateTimeFormatterOptions, align: 'center' });
        colModel.push({ key: false, name: 'Status', index: 'Status', align: 'left', search: true, formatter: processStatusFormmater });
        colModel.push({ key: false, name: 'UpdateDate', index: 'UpdateDate', align: 'left', search: true, formatter: 'date', formatoptions: JQGridSettings.DateTimeFormatterOptions, align: 'center' });
        colModel.push({ key: false, name: 'ProcessType', index: 'ProcessType', align: 'left', search: true });




        //{ key: false, name: 'Gender', index: 'Gender', editable: true, edittype: 'select', editoptions: { value: { 'M': 'Male', 'F': 'Female', 'N': 'None' } } },
        //clean up the grid first - only table element remains after this
        $(elementIDs.reportSyncLogGridJQ).jqGrid('GridUnload');

        //adding the pager element
        $(elementIDs.reportSyncLogGridJQ).parent().append("<div id='p" + elementIDs.reportSyncLogGrid + "'></div>");

        var url = URL.getReportSyncData;

        $(elementIDs.reportSyncLogGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Reporting Center Syncronization Status',
            rowNum: 30,
            height: '280',
            autowidth: true,
            //loadonce: false,
            viewrecords: true,
            ignoreCase: true,
            hidegrid: false,
            hiddengrid: false,
            altRows: true,
            pager: '#p' + elementIDs.reportSyncLogGrid,
            altclass: 'alternate',
            //this is added for pagination.
            rowList: [10, 20, 30],
            sortname: 'UpdateDate',
            sortorder: "desc"
        });

        var pagerElement = '#p' + elementIDs.reportSyncLogGrid;
        $('#p' + elementIDs.reportSyncLogGrid).find('input').css('height', '20px');

        $(elementIDs.reportSyncLogGridJQ).jqGrid('filterToolbar', {
            stringResult: true, searchOnEnter: true,
            defaultSearch: "cn",
        });

        //remove default buttons
        $(elementIDs.reportSyncLogGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false, refresh: true });
        //remove paging
        //$(pagerElement).find(pagerElement + '_center').remove();
        $(pagerElement).find('#first_p' + elementIDs.reportSyncLogGrid).remove();
        $(pagerElement).find('#last_p' + elementIDs.reportSyncLogGrid).remove();

        $(pagerElement).find(pagerElement + '_right').remove();
    }

    refreshInterval = setInterval(AutorefreshDataSyncReports, 1000);

    function AutorefreshDataSyncReports() {
        timer--;
        if (timer == 0) {
            //loadreportQueueGrid();
            jQuery(elementIDs.reportSyncLogGridJQ).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            timer = 60;
        }
        //else {
        $("#spnTimerSync").text(timer + " seconds.");
        //}
    }

    function showDateTime(cellvalue, options, rowObject) {
        if (cellvalue.length > 0)
            cellvalue = cellvalue.slice(0, -4).replace('T', ' ');
        return cellvalue;
    }

    String.prototype.replaceAll = function (search, replacement) {
        var target = this;
        return target.split(search).join(replacement);
    };




    // Function to set font colour based on status using formatter 
    function processStatusFormmater(cellvalue, options, rowObject) {
        switch (cellvalue) {
            case "ReadyForUpdate":
                // If status is Inprogress then display status text in black color 
                cellvalue = '<span style="color:blue">' + cellvalue + '</span>';
                break;
            case "Inprogress":
                // If status is Complete then display status text in green color
                cellvalue = '<span style="color:orange">' + cellvalue + '</span>';
                break;
            case "Completed":
                // If status is Complete then display status text in green color
                cellvalue = '<span style="color:green">' + cellvalue + '</span>';
                break;
            case "Errored":
                // If status is Errored then display status text in red color 
                cellvalue = "<img src='/Content/css/custom-theme/images/error.png'  alt='MDM Error' style='margin-left: 5px;'  title = 'MDM Error'  onclick='javascript:showMDMErrorMessage(" + rowObject.FormInstanceId + "," + rowObject.FormDesignVersionId + ");'/>";
                break;
        }
        return cellvalue;
    } // processStatusFormmater

    showMDMErrorMessage = function (formInstanceId, FormDesignVersionId) {
        var urlMDMError = URL.mDMErrorData + "?formInstanceId=" + formInstanceId + "&&formDesignVersionId=" + FormDesignVersionId;
        var promiseMDMError = ajaxWrapper.getJSON(urlMDMError);
        $.when(promiseMDMError).done(function (xhrML) {
            if (xhrML != '') {
                var res = xhrML.split('|');
                function init() {
                    $(elementIDs.reportLogDialog).dialog({
                        autoOpen: false,
                        height: 700,
                        width: 900,
                        modal: true
                    });
                }
                init();
                $(elementIDs.reportLogDialog).dialog('option', 'title', "Error Details");
                $(elementIDs.reportLogDialog).dialog("open");
                $(elementIDs.lblerror).html("");
                $(elementIDs.lblerrordesc).html("");
                $(elementIDs.lblerror).html(res[0]);
                if (res.length == 2) {
                    $(elementIDs.lblerrordesc).html(res[1]);
                }
            }

        });
    }

    return {
        loadDataSync: function () {
            loadDataSyncGrid();
        }
    }

}();

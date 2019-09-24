var odm = function () {
    var URLs = {
        formDesignUIParentSectionList: '/ODM/FormDesignParentSectionList?tenantId=1&formName={formName}&year={year}',
        GetMigrationQueue: "/ODM/GetMigrationQueue",
        QueueForMigration: "/ODM/QueueForMigration",
        BaselineAndCreateNewMinorVersion: "/ODM/BaselineAndCreateNewMinorVersion"
    };

    var elementIDs = {
        selectPlanGrid: "selectPlanGrid",
        selectPlanGridJQ: "#selectPlanGrid",

        misMatchGrid: "misMatchGrid",
        misMatchGridJQ: "#misMatchGrid",

        selectPBPSectionGrid: "selectPBPSectionGrid",
        selectPBPSectionGridJQ: "#selectPBPSectionGrid",

        selectSOTSectionGrid: "selectSOTSectionGrid",
        selectSOTSectionGridJQ: "#selectSOTSectionGrid",

        migrationQueueGrid: "migrationQueueGrid",
        migrationQueueGridJQ: "#migrationQueueGrid",

        btnUploadMDBFileJQ: "#btnUploadMDBFile",

        descriptionToDisplayJQ: "#descriptionToDisplay",
        yearToDisplayJQ: "#yearToDisplay",

        btnStartMigration: "#btnStartMigration",

        fileNameJQ: "#fileName",
        originalFileNameJQ: "#originalFileName",

        chkBeforeBaselineJQ: "#chkBeforeBaseline",
        chkAfterBaselineJQ: "#chkAfterBaseline",
        chkRunManualUpdateOnlyJQ: "#chkRunManualUpdateOnly"

    };

    var data = {
        selectPlanGridData: [],
        misMatchGridData: [],
    }

    var timer = 60;

    function init() {
        $(document).ready(function () {
            loadSelectPlanGrid();
            loadMisMatchGrid();
            loadselectPBPSectionGrid();
            loadselectSOTSectionGrid();
            loadmigrationQueueGrid();

            refreshInterval = setInterval(AutoRefreshMigrationQueue, 1000);

            $(elementIDs.btnUploadMDBFileJQ).click(function () {
                var queueDialog = new showMDBUploadDialog(elementIDs, data, URLs);
            });

            $(elementIDs.btnStartMigration).click(function () {
                yesNoConfirmDialog.show("Are you sure to migrate the plan?", function (e) {
                    yesNoConfirmDialog.hide();
                    if (e) {
                        var planData = $(elementIDs.selectPlanGridJQ).jqGrid('getGridParam', 'data');

                        planData = planData.filter(function (dt) {
                            return dt.SelectPlan == "Yes";
                        })

                        var pbpSections = $(elementIDs.selectPBPSectionGridJQ).jqGrid('getGridParam', 'data');

                        pbpSections = pbpSections.filter(function (dt) {
                            return dt.SelectSection == "Yes";
                        })

                        var sotSections = $(elementIDs.selectSOTSectionGridJQ).jqGrid('getGridParam', 'data');

                        sotSections = sotSections.filter(function (dt) {
                            return dt.SelectSection == "Yes";
                        })

                        if (planData.length == 0) {
                            messageDialog.show("Please select at least one Plan");
                        }
                        else if (pbpSections.length == 0 && sotSections.length == 0) {
                            messageDialog.show("Please select at least one Section");
                        }
                        else {
                            var description = $(elementIDs.descriptionToDisplayJQ).text();
                            var year = $(elementIDs.yearToDisplayJQ).text();
                            var fileName = $(elementIDs.fileNameJQ).text();
                            var originalFileName = $(elementIDs.originalFileNameJQ).text();
                            var isBeforeBaseline = $(elementIDs.chkBeforeBaselineJQ).is(":checked");
                            var isAfterBaseline = $(elementIDs.chkAfterBaselineJQ).is(":checked");
                            var runManualUpdateOnly = $(elementIDs.chkRunManualUpdateOnlyJQ).is(":checked");
                            var postData = {
                                planData: JSON.stringify(planData),
                                pbpSections: JSON.stringify(pbpSections),
                                sotSections: JSON.stringify(sotSections),
                                description: description,
                                year: year,
                                fileName: fileName,
                                originalFileName: originalFileName,
                            }
                            var promise = ajaxWrapper.postJSON(URLs.QueueForMigration, postData);
                            promise.done(function (xhr) {
                                if (xhr.Result == ServiceResult.SUCCESS) {
                                    var batchId = xhr.Items[0].Messages[0];
                                    messageDialog.show("Plan Queued for migration.");
                                    $(elementIDs.migrationQueueGridJQ).trigger("reloadGrid");
                                    var postData = {
                                        batchId: batchId,
                                        isBeforeBaseline: isBeforeBaseline,
                                        isAfterBaseline: isAfterBaseline,
                                        runManualUpdateOnly: runManualUpdateOnly
                                    }
                                    // setTimeout(function () {
                                    var promise = ajaxWrapper.postJSON(URLs.BaselineAndCreateNewMinorVersion, postData, false);
                                    //},300000)
                                }
                                else {
                                    messageDialog.show("error occured while Queuing Plan to migration.");
                                }
                            });
                            promise.fail();
                        }
                    }
                });
            });
        });
    }
    init();

    function loadSelectPlanGrid() {
        //set column list for grid
        var colArray = null;
        var planSelectAllcheckbox = '<input type="checkbox" class="check-all" id="planSelectAllcheckbox"  />' + '  ' + 'Select Plan';
        colArray = ['SOTFormInstanceId', 'PBPFormInstanceId', 'FormDesignVersionId', 'FolderId', 'FolderversionID', 'QID', 'Folder', 'Folder Version', 'View', planSelectAllcheckbox];

        //set column models
        var colModel = [];
        colModel.push({ key: true, name: 'SOTFormInstanceId', index: 'SOTFormInstanceId', editable: true, hidden: true });
        colModel.push({ key: false, name: 'PBPFormInstanceId', index: 'PBPFormInstanceId', editable: true, hidden: true });
        colModel.push({ key: false, name: 'FormDesignVersionId', index: 'FormDesignVersionId', editable: true, hidden: true });
        colModel.push({ key: false, name: 'FolderId', index: 'FolderId', editable: false, hidden: true });
        colModel.push({ key: false, name: 'FolderversionID', index: 'FolderversionID', editable: false, hidden: true });
        colModel.push({ key: false, name: 'QID', index: 'QID', editable: false });
        colModel.push({ key: false, name: 'Folder', index: 'Folder', align: 'left' });
        colModel.push({ key: false, name: 'FolderVersion', index: 'FolderVersion', editable: false, align: 'left' });
        colModel.push({ key: false, name: 'View', index: 'View', editable: false, edittype: 'false', align: 'left', hidden: true });
        colModel.push({
            key: false, name: 'SelectPlan', index: 'SelectPlan', editable: true, edittype: 'checkbox', align: 'center',
            editoptions: { value: "Yes:No" }, formatter: "checkbox", formatoptions: { disabled: false }
        });
        //clean up the grid first - only table element remains after this
        $(elementIDs.selectPlanGrid).jqGrid('GridUnload');

        //adding the pager element
        $(elementIDs.selectPlanGridJQ).parent().append("<div id='p" + elementIDs.selectPlanGrid + "'></div>");

        $(elementIDs.selectPlanGridJQ).jqGrid({
            //url: URLs.getQueuedPBPImportList,
            datatype: 'local',
            cache: false,
            data: data.selectPlanGridData,
            colNames: colArray,
            colModel: colModel,
            caption: 'Match Plan',
            height: 380,
            rowNum: 2000,
            loadonce: true,
            rowList: [20, 40, 60],
            headertitles: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortname: 'CreatedDate',
            sortorder: 'desc',
            //pager: '#p' + elementIDs.selectPlanGrid,
            beforeSelectRow: function (rowid, e) {
                var value = $(e.target).is(":checked");

                var data = $(elementIDs.selectPlanGridJQ).jqGrid('getGridParam', 'data');

                var rowData = data.filter(function (dt) {
                    return dt["SOTFormInstanceId"].toString() == rowid;
                })

                rowData = rowData[0];
                if (value == true) {
                    rowData["SelectPlan"] = "Yes";
                    var data = $(elementIDs.selectPlanGridJQ).jqGrid('getGridParam', 'data');
                    var isallSelected = data.filter(function (dt) {
                        return dt["SelectPlan"] == "No"
                    })

                    if (isallSelected == undefined || isallSelected.length == 0) {
                        $("#planSelectAllcheckbox").prop('checked', true);
                    }
                }
                else {
                    $("#planSelectAllcheckbox").prop('checked', false);
                    rowData["SelectPlan"] = "No"
                }
            },
            //altclass: 'alternate'
            gridComplete: function () {
                initializeCheckAll();
            },


        });

        var pagerElement = '#p' + elementIDs.selectPlanGrid;



        //remove default buttons
        $(elementIDs.selectPlanGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false, refresh: false });
        $(elementIDs.selectPlanGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

        $('#planSelectAllcheckbox').change(function () {
            var val = $("#planSelectAllcheckbox").prop('checked');
            if (val == true) {
                val = "Yes";
            }
            else {
                val = "No";
            }

            var gridData = $(elementIDs.selectPlanGridJQ).jqGrid('getGridParam', 'data');
            try {
                var objFilterCriteria = JSON.parse($(elementIDs.selectPlanGridJQ).getGridParam("postData").filters);
                $.each(objFilterCriteria.rules, function (ind, val) {
                    gridData = $.grep(gridData, function (a) {
                        return (a[val.field].toLowerCase().indexOf(val.data.toLowerCase()) >= 0);
                    });
                });
            }
            catch (err) {
                var gridData = $(elementIDs.selectPlanGridJQ).jqGrid('getGridParam', 'data');
            }

            filteredData = gridData;

            $.each(filteredData, function (idx, dt) {
                dt.SelectPlan = val
            })

            $(elementIDs.selectPlanGridJQ).trigger('reloadGrid');
        });
    }

    function loadMisMatchGrid() {
        //set column list for grid
        var colArray = null;
        colArray = ['FormInstance', 'FolderversionID', 'QID', 'Folder', 'Folder Version', 'Select Plan'];

        //set column models
        var colModel = [];
        colModel.push({ key: true, name: 'FormInstance', index: 'FormInstance', align: 'right', editable: true, hidden: true });
        colModel.push({ key: false, name: 'FolderversionID', index: 'FolderversionID', editable: false, hidden: true });
        colModel.push({ key: false, name: 'QID', index: 'QID', editable: false });
        colModel.push({ key: false, name: 'Folder', index: 'Folder', align: 'left', hidden: true });
        colModel.push({ key: false, name: 'Folder Version', index: 'FolderVersion', editable: true, edittype: 'select', align: 'left', hidden: true });
        colModel.push({ key: false, name: 'Select Plan', index: 'SelectPlan', editable: true, edittype: 'select', align: 'left', hidden: true });

        //clean up the grid first - only table element remains after this
        $(elementIDs.misMatchGrid).jqGrid('GridUnload');

        //adding the pager element
        $(elementIDs.misMatchGridJQ).parent().append("<div id='p" + elementIDs.misMatchGrid + "'></div>");

        $(elementIDs.misMatchGridJQ).jqGrid({
            //url: URLs.getQueuedPBPImportList,
            datatype: 'local',
            cache: false,
            data: data.misMatchGridData,
            colNames: colArray,
            colModel: colModel,
            loadonce: true,
            caption: 'MisMatch Plan',
            height: 380,
            rowNum: 2000,
            rowList: [20, 40, 60],
            headertitles: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortname: 'CreatedDate',
            sortorder: 'desc',
            //pager: '#p' + elementIDs.misMatchGrid,
            beforeSelectRow: function (rowid, e) {
                var value = $(e.target).is(":checked");
                var rowData = $(elementIDs.selectPBPSectionGridJQ).jqGrid("getLocalRow", rowid);
                if (value == true) {
                    rowData["SelectPlan"] = "Yes";
                }
                else { rowData["SelectPlan"] = "No" }
            },
            //altclass: 'alternate'
            gridComplete: function () {

            },

        });

        var pagerElement = '#p' + elementIDs.misMatchGrid;

        //remove default buttons
        $(elementIDs.misMatchGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false, refresh: false });
        $(elementIDs.misMatchGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
        $(pagerElement + '_right').width('37px');

    }

    function loadselectPBPSectionGrid() {
        //set column list for grid
        var colArray = null;
        var pbpSelectAllcheckbox = '<input type="checkbox" class="check-all" id="pbpSelectAllcheckbox"  />' + '  ' + 'Select Section';
        colArray = ['UIElementID', 'GeneratedName', 'Section Name', pbpSelectAllcheckbox];

        //set column models
        var colModel = [];
        colModel.push({ key: true, hidden: true, name: 'UIElementID', index: 'UIElementID', align: 'right', hidden: true });
        colModel.push({ key: false, hidden: true, name: 'GeneratedName', index: 'GeneratedName', align: 'right', hidden: true });
        colModel.push({ key: false, name: 'SectionName', index: 'SectionName', editable: false, hidden: false });
        colModel.push({
            key: false, name: 'SelectSection', index: 'SelectSection', editable: true, edittype: 'select', align: 'center',
            editoptions: { value: "Yes:No" }, formatter: "checkbox", formatoptions: { disabled: false }
        });


        var url = URLs.formDesignUIParentSectionList.replace(/\{formName\}/g, 'PBPView').replace(/\{year\}/g, $(elementIDs.yearToDisplayJQ).text());
        //clean up the grid first - only table element remains after this
        $(elementIDs.selectPBPSectionGrid).jqGrid('GridUnload');

        //adding the pager element
        $(elementIDs.selectPBPSectionGridJQ).parent().append("<div id='p" + elementIDs.selectPBPSectionGrid + "'></div>");

        $(elementIDs.selectPBPSectionGridJQ).jqGrid({
            url: url,
            datatype: 'local',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'PBP Sections',
            height: 380,
            rowNum: 20,
            rowList: [20, 40, 60],
            headertitles: true,
            loadonce: false,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortorder: 'desc',
            pager: '#p' + elementIDs.selectPBPSectionGrid,
            beforeSelectRow: function (rowid, e) {
                var value = $(e.target).is(":checked");
                var rowData = $(elementIDs.selectPBPSectionGridJQ).jqGrid("getLocalRow", rowid);
                if (value == true) {
                    rowData["SelectSection"] = "Yes";
                    var data = $(elementIDs.selectPBPSectionGridJQ).jqGrid('getGridParam', 'data');
                    var isallSelected = data.filter(function (dt) {
                        return dt["SelectSection"] == "No"
                    })

                    if (isallSelected == undefined || isallSelected.length == 0) {
                        $("#pbpSelectAllcheckbox").prop('checked', true);
                    }
                }
                else {
                    $("#pbpSelectAllcheckbox").prop('checked', false);
                    rowData["SelectSection"] = "No"
                }
            },
            //altclass: 'alternate'
            gridComplete: function () {
                initializeCheckAll();
            }

        });

        var pagerElement = '#p' + elementIDs.selectPBPSectionGrid;

        //remove default buttons
        $(elementIDs.selectPBPSectionGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false, refresh: false });
        $(elementIDs.selectPBPSectionGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
        $(pagerElement).css('left', '110px');
        $('.ui-paging-info').width('130px');

        $('#pbpSelectAllcheckbox').change(function () {
            var val = $("#pbpSelectAllcheckbox").prop('checked');
            if (val == true) {
                val = "Yes";
            }
            else {
                val = "No";
            }

            var gridData = $(elementIDs.selectPBPSectionGridJQ).jqGrid('getGridParam', 'data');
            try {
                var objFilterCriteria = JSON.parse($(elementIDs.selectPBPSectionGridJQ).getGridParam("postData").filters);
                $.each(objFilterCriteria.rules, function (ind, val) {
                    gridData = $.grep(gridData, function (a) {
                        return (a[val.field].toLowerCase().indexOf(val.data.toLowerCase()) >= 0);
                    });
                });
            }
            catch (err) {
                var gridData = $(elementIDs.selectPBPSectionGridJQ).jqGrid('getGridParam', 'data');
            }

            filteredData = gridData;
            $.each(filteredData, function (idx, dt) {
                dt.SelectSection = val
            })

            $(elementIDs.selectPBPSectionGridJQ).trigger('reloadGrid');
        });
    }

    function loadselectSOTSectionGrid() {
        //set column list for grid
        var colArray = null;
        var sotSelectAllcheckbox = '<input type="checkbox" class="check-all" id="sotSelectAllcheckbox"  />' + '  ' + 'Select Section';
        colArray = ['UIElementID', 'GeneratedName', 'Section Name', sotSelectAllcheckbox];

        //set column models
        var colModel = [];
        colModel.push({ key: true, hidden: true, name: 'UIElementID', index: 'UIElementID', align: 'right', hidden: true });
        colModel.push({ key: false, hidden: true, name: 'GeneratedName', index: 'GeneratedName', align: 'right', hidden: true });
        colModel.push({ key: false, name: 'SectionName', index: 'SectionName', editable: false, hidden: false });
        colModel.push({
            key: false, name: 'SelectSection', index: 'SelectSection', editable: true, edittype: 'select', align: 'center',
            editoptions: { value: "Yes:No" }, formatter: "checkbox", formatoptions: { disabled: false }
        });

        var url = URLs.formDesignUIParentSectionList.replace(/\{formName\}/g, 'Medicare').replace(/\{year\}/g, $(elementIDs.yearToDisplayJQ).text());
        //clean up the grid first - only table element remains after this
        $(elementIDs.selectSOTSectionGrid).jqGrid('GridUnload');

        //adding the pager element
        $(elementIDs.selectSOTSectionGridJQ).parent().append("<div id='p" + elementIDs.selectSOTSectionGrid + "'></div>");

        $(elementIDs.selectSOTSectionGridJQ).jqGrid({
            url: url,
            datatype: 'local',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'SOT Sections',
            height: 380,
            rowNum: 20,
            rowList: [20, 40, 60],
            headertitles: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            loadonce: false,
            sortorder: 'desc',
            pager: '#p' + elementIDs.selectSOTSectionGrid,
            beforeSelectRow: function (rowid, e) {
                var value = $(e.target).is(":checked");
                var rowData = $(elementIDs.selectSOTSectionGridJQ).jqGrid("getLocalRow", rowid);
                if (value == true) {
                    rowData["SelectSection"] = "Yes";
                    var data = $(elementIDs.selectSOTSectionGridJQ).jqGrid('getGridParam', 'data');
                    var isallSelected = data.filter(function (dt) {
                        return dt["SelectSection"] == "No"
                    })

                    if (isallSelected == undefined || isallSelected.length == 0) {
                        $("#sotSelectAllcheckbox").prop('checked', true);
                    }
                }
                else {
                    $("#sotSelectAllcheckbox").prop('checked', false);
                    rowData["SelectSection"] = "No"
                }
            },
            altclass: 'alternate',
            gridComplete: function () {
                initializeCheckAll();
            },

        });

        var pagerElement = '#p' + elementIDs.selectSOTSectionGrid;

        //remove default buttons
        $(elementIDs.selectSOTSectionGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false, refresh: false });
        $(elementIDs.selectSOTSectionGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
        $(pagerElement).css('left', '110px');
        $('.ui-paging-info').width('130px');

        $('#sotSelectAllcheckbox').change(function () {
            var val = $("#sotSelectAllcheckbox").prop('checked');
            if (val == true) {
                val = "Yes";
            }
            else {
                val = "No";
            }

            var gridData = $(elementIDs.selectSOTSectionGridJQ).jqGrid('getGridParam', 'data');

            try {
                var objFilterCriteria = JSON.parse($(elementIDs.selectSOTSectionGridJQ).getGridParam("postData").filters);
                $.each(objFilterCriteria.rules, function (ind, val) {
                    gridData = $.grep(gridData, function (a) {
                        return (a[val.field].toLowerCase().indexOf(val.data.toLowerCase()) >= 0);
                    });
                });
            }
            catch (err) {
                var gridData = $(elementIDs.selectSOTSectionGridJQ).jqGrid('getGridParam', 'data');
            }

            filteredData = gridData;
            $.each(filteredData, function (idx, dt) {
                dt.SelectSection = val
            })

            $(elementIDs.selectSOTSectionGridJQ).trigger('reloadGrid');

        });
    }

    function initializeCheckAll() {
        $(".check-all").parent(".ui-jqgrid-sortable").removeClass("ui-jqgrid-sortable");
    }

    function loadmigrationQueueGrid() {
        //set column list for grid
        var colArray = null;
        colArray = ['BatchID', 'File Name', 'Description', 'Migrated Date', 'Migrated By', 'Status'];

        //set column models
        var colModel = [];
        colModel.push({ key: true, hidden: false, name: 'BatchID', index: 'BatchID', align: 'left', hidden: true, editable: false });
        colModel.push({ key: false, name: 'MDBOriginalFileName', index: 'MDBOriginalFileName', editable: false });
        colModel.push({ key: false, name: 'Description', index: 'Description', editable: false });
        colModel.push({ key: false, name: 'MigratedDate', index: 'MigratedDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateTimeFormatterOptions });
        colModel.push({ key: false, name: 'MigratedBy', index: 'MigratedBy', editable: false });
        colModel.push({ key: false, name: 'Status', index: 'Status', editable: false, formatter: processStatusFormmater, search: false });

        //clean up the grid first - only table element remains after this
        $(elementIDs.migrationQueueGridJQ).jqGrid('GridUnload');

        //adding the pager element
        $(elementIDs.migrationQueueGridJQ).parent().append("<div id='p" + elementIDs.migrationQueueGrid + "'></div>");

        $(elementIDs.migrationQueueGridJQ).jqGrid({
            url: URLs.GetMigrationQueue,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Migration Status',
            height: 380,
            rowNum: 20,
            loadonce: false,
            ignoreCase: true,
            rowList: [20, 40, 60],
            headertitles: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortname: 'BatchID',
            sortorder: 'desc',
            pager: '#p' + elementIDs.migrationQueueGrid,
            gridComplete: function () {

            },

        });

        var pagerElement = '#p' + elementIDs.migrationQueueGrid;

        //remove default buttons
        $(elementIDs.migrationQueueGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.migrationQueueGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

    }

    function AutoRefreshMigrationQueue() {
        timer--;
        if (timer == 0) {
            $(elementIDs.migrationQueueGridJQ).trigger("reloadGrid");
            timer = 60;
        }
        $("#spnTimer").text(timer + " seconds.").css("font-weight", "Bold");
    }

    function processStatusFormmater(cellvalue, options, rowObject) {
        switch (cellvalue) {
            case 'Queued':
                cellvalue = '<span style="">' + 'Queued' + '</span>';
                break;
            case 'In Progress':
                cellvalue = '<span style="color:blue">' + 'In Progress' + '</span>';
                break;
            case 'Fail':
                cellvalue = '<span style="color:red"><b>' + 'Errored' + '</b></span>';
                break;
            case 'Completed':
                cellvalue = '<span style="color:green">' + 'Complete' + '</span>';
                break;
        }
        return cellvalue;
    }
}

var showMDBUploadDialog = function (odmElementIDs, data, odmURLs) {

    var odmElementIDs = odmElementIDs;
    var odmData = data;
    var URLs = {
        uploadPlanList: "/ODM/UploadPBPFiles",
    };

    var elementIDs = {

        uploadMDBDialog: "#uploadMDBDialog",

        description: "#description",

        uploadMDBFile: "#uploadMDBFile",
        UploadMDBFileName: "#UploadMDBFileName",
        uploadMDBFileNameSpan: "#uploadMDBFileNameSpan",


        btnMDBUpload: "#btnMDBUpload",

        btnSelectPortfolio: '#btnSelectPortfolioDetails',
        yearJQ: '#year',
        folderId: '#folderId',
        folderVersionId: '#folderVersionId',
        folderVersionEffectiveDate: '#folderVersionEffectiveDate',
    };

    function init() {
        //register dialog for grid row add/edit
        $(elementIDs.uploadMDBDialog).dialog({
            autoOpen: true,
            title: "Upload MDB File",
            height: 250,
            width: 610,
            modal: true,
            close: function (event, ui) {
                $(elementIDs.UploadMDBFileName).val("");
                $(elementIDs.description).val("");
            }
        });
        //register event for button click on dialog
        $(elementIDs.uploadMDBDialog + ' button').unbind().on('click', function () {
            var mdbFileName = $(elementIDs.UploadMDBFileName).val();
            var mdbFileNameUpload = $(elementIDs.UploadMDBFileName).get(0);
            var mdbFileNamefiles = mdbFileNameUpload.files;

            var fileData = new FormData();

            // Looping over all files and add it to FormData object  
            for (var i = 0; i < mdbFileNamefiles.length; i++) {
                fileData.append(mdbFileNamefiles[i].name, mdbFileNamefiles[i]);
            }

            // Adding description key to FormData object  
            var descriptionValue = $(elementIDs.description).val();
            fileData.append('description', descriptionValue);

            if (validate()) {
                saveImport(fileData);
            }

        });

        validate = function () {
            var allowedExtensions = ["MDB", "LDB", "mdb"];
            var mdbFileNameIsValid = false;
            var mdbFileName = $(elementIDs.UploadMDBFileName).val();
            if (mdbFileName == '') {
                $(elementIDs.UploadMDBFileName).parent().addClass('has-error');
                $(elementIDs.UploadMDBFileName).addClass('form-control');
                $(elementIDs.uploadMDBFileNameSpan).text(PBPImportMessages.PBPFileRequiredMsg);
                isValid = false
            }
            else {
                if (mdbFileName) {
                    var fileN = mdbFileName.split('.');
                    if ($.inArray(fileN[fileN.length - 1], allowedExtensions) == -1) {
                        messageDialog.show(PBPImportMessages.InvalidFileExtensions);
                        mdbFileNameIsValid = false;
                    }
                    else {
                        file = $(elementIDs.UploadMDBFileName)[0].files[0];
                        $(elementIDs.UploadMDBFileName).removeClass('form-control');
                        $(elementIDs.UploadMDBFileName).parent().addClass('has-error');
                        $(elementIDs.uploadMDBFileNameSpan).text('');
                        mdbFileNameIsValid = true;
                    }
                }
            }

            return mdbFileNameIsValid;
        }

        saveImport = function (fileData) {

            $.ajax({
                url: URLs.uploadPlanList,
                type: "POST",
                contentType: false, // Not to set any content header  
                processData: false, // Not to process data  
                data: fileData,
                success: function (result) {
                    $(odmElementIDs.selectPlanGridJQ).jqGrid('clearGridData');
                    $(odmElementIDs.misMatchGridJQ).jqGrid('clearGridData');
                    if (result == 0) {
                        messageDialog.show("Error occurred while uploading file");
                    }
                    else {
                        if (result.MatchPlanList.length > 0) {
                            odmData.selectPlanGridData = result.MatchPlanList;
                            $(odmElementIDs.selectPlanGridJQ).jqGrid('setGridParam', { data: odmData.selectPlanGridData }).trigger('reloadGrid');
                        }

                        if (result.MisMatchPlanList.length > 0) {
                            odmData.misMatchGridData = result.MisMatchPlanList;
                            $(odmElementIDs.misMatchGridJQ).jqGrid('setGridParam', { data: odmData.misMatchGridData }).trigger('reloadGrid');
                        }

                        $(odmElementIDs.originalFileNameJQ).text(result.OriginalFileName);
                        $(odmElementIDs.fileNameJQ).text(result.FileName);
                        $(odmElementIDs.descriptionToDisplayJQ).text(result.Description);
                        $(odmElementIDs.yearToDisplayJQ).text(result.Year);


                        var pbpURl = odmURLs.formDesignUIParentSectionList.replace(/\{formName\}/g, 'PBPView').replace(/\{year\}/g, $(odmElementIDs.yearToDisplayJQ).text());
                        $(odmElementIDs.selectPBPSectionGridJQ).jqGrid('setGridParam', { datatype: 'json', url: pbpURl }).trigger('reloadGrid');


                        var sotURl = odmURLs.formDesignUIParentSectionList.replace(/\{formName\}/g, 'Medicare').replace(/\{year\}/g, $(odmElementIDs.yearToDisplayJQ).text());
                        $(odmElementIDs.selectSOTSectionGridJQ).jqGrid('setGridParam', { datatype: 'json', url: sotURl }).trigger('reloadGrid');

                        $(elementIDs.uploadMDBDialog).dialog("close");

                        $(odmElementIDs.selectPBPSectionGridJQ).jqGrid('setGridParam', { loadonce: true });
                        $(odmElementIDs.selectSOTSectionGridJQ).jqGrid('setGridParam', { loadonce: true });
                    }
                },
                error: function (err) {
                    messageDialog.show(err.statusText);
                }
            });
        }
    }

    init();
}

var accountSearch = function () {
    var URLs = {
        //Gets the document list
        documentList: '/DocumentCollateral/GetDocumentList?tenantId=1',
        generateDocument: '/DocumentCollateral/GenerateReport?formInstanceId={formInstanceId}'
    };
    var elementIDs = {
        documentGrid: 'grdDocument',
        documentGridJQ: '#grdDocument',
        queueCollateralBtnJQ: '#QueueCollateralBtn',
        uploadCollateralBtnJQ: '#UploadCollateralBtn'
    };

    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    function loadDocumentListGrid() {
        //set column list for grid
        var colArray = ['', '', '', '', 'Folder', 'Folder Version Number', 'Effective Date', 'Product Names', 'Status', 'Report'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true });
        colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true });
        colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', hidden: true });
        colModel.push({ name: 'FormInstanceID', index: 'FormInstanceID', key: true, hidden: true });
        colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left' });
        colModel.push({ name: 'VersionNumber', index: 'VersionNumber', editable: false, align: 'left' });
        colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'left' });
        colModel.push({ name: 'ProductName', index: 'ProductName', editable: false, align: 'left' });
        colModel.push({ name: 'Status', index: 'Status', editable: false, align: 'left' });
        colModel.push({ name: 'Action', index: 'Action', width: '40px', align: 'left', sortable: false, search: false, editable: false, formatter: actionFormatter });

        //clean up the grid first - only table element remains after this
        $(elementIDs.documentGridJQ).jqGrid('GridUnload');
        //adding the pager element
        $(elementIDs.documentGridJQ).parent().append("<div id='p" + elementIDs.documentGrid + "'></div>");

        var url = URLs.documentList;
        $(elementIDs.documentGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Document List',
            height: '350',
            rowNum: 10000,
            ignoreCase: true,
            loadonce: true,
            autowidth: true,
            viewrecords: true,
            rownumbers: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.documentGrid,
            sortname: 'FolderName',
            altclass: 'alternate',
            gridComplete: function () {
                rows = $(this).getDataIDs();
                if (rows && rows.length > 0) {
                    $(this).jqGrid("setSelection", rows[0]);

                    for (index = 0; index < rows.length; index++) {
                        row = $(this).getRowData(rows[index]);
                        $('#fis' + row.FormInstanceID).click(function () {
                            var formInstanceid = this.id.replace(/fis/g, '');

                            var currentRow = $(elementIDs.documentGridJQ).getRowData(formInstanceid);
                            DownloadDocument(currentRow, URLs.generateDocument);
                        });
                    }
                }
            },
            resizeStop: function (width, index) {
                autoResizing(elementIDs.documentGridJQ);
            }
        });
        var pagerElement = '#p' + elementIDs.documentGrid;
        //remove default buttons
        $(elementIDs.documentGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        //remove paging
        //$(pagerElement).find(pagerElement + '_center').remove();
        //$(pagerElement).find(pagerElement + '_right').remove();
        $(elementIDs.documentGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

    }

    function DownloadDocument(currentRow, url) {
        var getReportUrl = url.replace(/\{formInstanceId\}/g, currentRow.FormInstanceID);
        var promise = ajaxWrapper.getJSON(getReportUrl);

        promise.done(function (result) {
            alert('hello');
        });
    }

    function actionFormatter(cellValue, options, rowObject) {
        return "<span id = 'fis" + rowObject.FormInstanceID + "' class='ui-icon ui-icon-document view' title = 'Generate Report' style = 'cursor: pointer'/>";
    }

    function init() {
        checkCollateralGenerationClaims(elementIDs, claims);
        $(document).ready(function () {
            loadDocumentListGrid();
        });

    }
    init();

    return {
        loadDocumentList: function () {
            loadDocumentListGrid();
        }
    }
}();

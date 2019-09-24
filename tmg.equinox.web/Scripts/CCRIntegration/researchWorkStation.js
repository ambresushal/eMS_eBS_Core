var researchWorkStation = function () {

    var URLs = {
        getTranslationQueueList:"",
    }
    var elementIDs = {
        translatedListGrid: "translatedListGrid",
        translatedListGridJQ: "#translatedListGrid"
        
    };
    function init() {
        $(document).ready(function () {
           loadTranslatedgrid();
        });
    }
    init();

    function loadTranslatedgrid() {
        //set column list for grid
        var colArray = null;
        colArray = ['Product ID', 'Plan Name', 'Account Name', 'Folder Name', 'Product Type', 'Effective Date', 'Folder version number', 'Processed on'];

        //set column models
        var colModel = [];        
        colModel.push({ key: true, name: 'ProductID', index: 'ProductID', align: 'right', editable: false });
        colModel.push({ key: true, name: 'PlanName', index: 'PlanName', align: 'right', editable: false });
        colModel.push({ key: false, name: 'AccountName', index: 'AccountName', editable: true });
        colModel.push({ key: false, name: 'FolderName', index: 'FolderName', editable: true });
        colModel.push({ key: true, name: 'ProductType', index: 'ProductType', align: 'right', editable: false });
        colModel.push({ key: false, name: 'EffectiveDate', index: 'EffectiveDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' });
        colModel.push({ key: false, name: 'Folderversionnumber', index: 'Folderversionnumber', editable: true });        
        colModel.push({ key: false, name: 'Processedon', index: 'Processedon', editable: false });
       

        //clean up the grid first - only table element remains after this
        $(elementIDs.translatedListGrid).jqGrid('GridUnload');
       //s $(elementIDs.translatedListGridJQ).parent().append("<div id='p" + elementIDs.translatedListGrid + "'></div>");
        $(elementIDs.translatedListGridJQ).jqGrid({
            url: URLs.getPBPDatabaseNameList,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: '',
            height: 400,
            rowNum: 20,
            rowList: [20, 40, 60],
            headertitles: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortname: 'CreatedDate',
            sortorder: 'desc',
            pager: '#p' + elementIDs.translatedListGrid,
            gridComplete: function () {
            }
        });
        var pagerElement = '#p' + elementIDs.translatedListGrid;        
       // $(elementIDs.translatedListGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.translatedListGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
    }

    function loadTableDetails(){



    }
}
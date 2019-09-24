var useractivity = function () {

    var URLs = {

        // gets Report Names for Report Generation
        userActivityList: '/DocumentCollateral/GetReportUserActivity'
    }

    var elementIDs = {
        userActivityTab: '#userActivity',
        userActivityGrid: 'userActivityGrid',
        userActivityGridJQ: '#userActivityGrid',
    }

    function userActivity() {
        //set column list for grid
        var colArray = ['Collateral Name', 'Collateral Version', 'Description', 'User Name', 'Date'];

        //set column models
        var colModel = [];
        colModel.push({ name: 'ReportName', index: 'ReportName', search: true, width: 60, });
        colModel.push({ name: 'ReportVersion', index: 'ReportVersion', width: 20, search: true });
        colModel.push({ name: 'Description', index: 'Description', search: true, width: 100 });
        colModel.push({ name: 'UserName', index: 'UserName', width: 30, search: true });
        colModel.push({ name: 'Date', index: 'Date', width: 60, search: true, formatter: 'date', formatoptions: JQGridSettings.DateTimeFormatterOptions, align: 'left' });


        var url = URLs.userActivityList;
        var pagerElement = '#p' + elementIDs.userActivityGrid;
        //adding the pager element
        $(elementIDs.userActivityGridJQ).parent().append("<div id='p" + elementIDs.userActivityGrid + "'></div>");
        //load the jqGrid - refer documentation for details
        $(elementIDs.userActivityGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Collateral',
            height: '400',
            rowNum: 20,
            rowList: [20, 40, 60],
            ignoreCase: true,
            pager: pagerElement,
            headertitles: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            //altRows: true,
            //altclass: 'alternate',
            sortname: 'Date',
            sortorder: 'desc'
        });


        //remove default buttons
        $(elementIDs.userActivityGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

        $('#p' + elementIDs.userActivityGrid).find('input').css('height', '20px');
        // add filter toolbar to the top
        $(elementIDs.userActivityGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });
    }

    function init() {
        $(document).ready(function () {

            $(elementIDs.userActivityTab).removeAttr("style");

            tabsGenerateReport = $(elementIDs.userActivityTab).tabs();

            userActivity();
        });
    }
    init();
}();

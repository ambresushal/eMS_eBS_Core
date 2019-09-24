var NotificationFunction = function () {
    var URLs = {
        formUpdatesList: '/DashBoard/NotificationstatusgridData?viewMode={viewMode}',
        forDataclrafterClosebtn: '/DashBoard/NotificationisreadDataclr?viewMode={viewMode}',
    };

    var elementIDs = {
        NotificationDialog: '#NotificationDialog',
        NotificationDialogButton: '#NotificationStatusbtn',
        notificationStatusGridJQ: '#notificationstatusGrid',
        formUpdatesGrid: 'formUpdates',
        isreadRadiobtn: '#radioUnRead',
        viewallRadiobtn: '#radioViewall',
        notificationPanel: '#notificationPanel'
    };
    var data = false;
    var CountData = 0;
    function init() {

        $(document).ready(function () {
            $(elementIDs.NotificationDialog).dialog({
                autoOpen: false,
                height: 600,
                width: 500,
                modal: true,
                left: 90,
                top: 50,
                beforeClose: function (event, ui) {
                    //after close dialog box Data clear
                    isreadOnlyclrdata(false);
                    $(elementIDs.queuedPBPImportGridJQ).trigger('reloadGrid');

                }

            });
        });
    }
    init();
    function viewAll()
    {
        loadFormGrid(true);
    }
    function isreadOnlyclrdata(data) {
        data = false;
        var url = URLs.forDataclrafterClosebtn.replace('{viewMode}', data);
        var promise = ajaxWrapper.postJSON(url, data);
        promise.done(function (data) {

        });

    }

    function loadFormGrid(data) {
        //set column list for grid
        var colArray = ['ID', ''];

        //set column models
        var colModel = [];
        colModel.push({ name: 'ID', index: 'ID', editable: false, align: 'left', hidden: true });
        colModel.push({
            name: 'Message', index: 'Message', editable: false, align: 'left'
        });

        //clean up the grid first - only table element remains after this
        $(elementIDs.notificationStatusGridJQ).jqGrid('GridUnload');

        //adding the pager element

        if (data == '') {
            data = false;
        }
        var url = URLs.formUpdatesList.replace('{viewMode}', data);

        $(elementIDs.notificationStatusGridJQ).jqGrid({
            url: url,
            datatype: 'json',
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: '',
            height: 380,
            rowNum: 20,
            rowList: [20, 40, 60],
            headertitles: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortname: 'ID',
            sortorder: 'desc',
            pager: '#p' + elementIDs.formUpdatesGrid,
            gridView: true,
            pageable: true,
            shrinkToFit: true,
            autowidth: true,
            gridComplete: function () {
                //for row count shows at header            
                var conntData = 0;
                conntData = jQuery(elementIDs.notificationStatusGridJQ).jqGrid('getGridParam', 'records');

                if (conntData == 0) {
                    $(elementIDs.NotificationDialog).dialog('option', 'title', 'Notification');
                }
                else if (data == false) {
                    $(elementIDs.NotificationDialog).dialog('option', 'title', conntData + '  unread notification');
                }
                else {
                    $(elementIDs.NotificationDialog).dialog('option', 'title', 'Notification');
                }
                jqgridCss();

            },


        });

        var pagerElement = '#p' + elementIDs.formUpdatesGrid;

        //remove default buttons
        $(elementIDs.notificationStatusGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        // $(elementIDs.notificationStatusGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
        $('#p' + elementIDs.formUpdatesGrid).find('input').css('height', '20px');


    }

    function jqgridCss() {
        $("#pformUpdates").css("top", "425px");
        $("#pformUpdates").css("padding", "0px");
        // $('#NotificationDialog .custom-combobox .ui-autocomplete-input,.ui-widget.ui-dialog input[type="text"]').css("display", "inline-block");
        //$('#NotificationDialog .custom-combobox .ui-autocomplete-input,.ui-widget.ui-dialog input[type="text"]').css("width", "25px");
        $('#NotificationDialog .grid-wrapper > .ui-jqgrid tr.jqgrow td').css("border", "0px");
        $('#NotificationDialog .grid-wrapper > .ui-jqgrid tr.jqgrow td').css("border-top", "5px solid white");
        $('#NotificationDialog .grid-wrapper > .ui-jqgrid tr.jqgrow td').css("padding", "20px 5px");
        $('#NotificationDialog .grid-wrapper > .ui-jqgrid tr.jqgrow td').css("border-radius", "15px");
        $('#NotificationDialog .grid-wrapper > .ui-jqgrid tr.jqgrow td').css("background-color", "gainsboro");
        //$('.ui-dialog .ui-dialog-content').css('padding', '0px 5px');
        //$('.ui-jqgrid .ui-jqgrid-bdiv').css("background-color", "white");
        $('#NotificationDialog .grid-wrapper > .ui-jqgrid tr.jqgrow td').css("white-space", "normal");
        $('#NotificationDialog .grid-wrapper > .ui-jqgrid tr.jqgrow td').css("word-wrap", "break-word");
        //$('.ui-dialog .grid-wrapper .ui-jqgrid-view').css("margin-top", "-65px");
        //$('.ui-dialog').css("top", "80px");
        //$('.ui-jqdialog .ui-jqdialog-titlebar, .ui-dialog .ui-dialog-titlebar').css("background-color", "rgba(65,126,172,1)");
        //$('.ui-jqdialog .ui-jqdialog-titlebar, .ui-dialog .ui-dialog-titlebar').css("border-bottom", "0px solid black");


    }

    return {
        showNotification: function ()
        {
            $(elementIDs.NotificationDialog).dialog({

                autoOpen: false,

                height: 600,

                width: 500,

                modal: true,

                left: 90,

                top: 50,

                beforeClose: function (event, ui) {

                    //after close dialog box Data clear

                    isreadOnlyclrdata(false);

                    $(elementIDs.queuedPBPImportGridJQ).trigger('reloadGrid');



                }



            });

            $(elementIDs.NotificationDialog).dialog("open");

            jqgridCss();
            loadFormGrid(data);
            $(elementIDs.viewallRadiobtn).unbind('click');
            $(elementIDs.NotificationDialog).dialog({
                position: {
                    my: 'center',
                    at: 'center'
                },
            });
            $(elementIDs.viewallRadiobtn).click(function () {               
                loadFormGrid(true);
            })
        }

    }

}();
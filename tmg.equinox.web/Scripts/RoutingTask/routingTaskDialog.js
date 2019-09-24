var routingTaskDialog = function () {
    var elementIDs = {
        //folder dialog element   routingTaskDialog
        routingTaskDialog: '#dashTaskDetails',
        dashTaskDetails: '#dashTaskDetails',
        btnRoutingTask: '#btnRoutingTask'
    };
    function init() {
        $(elementIDs.routingTaskDialog).dialog({
            autoOpen: false,
            height: 'auto',
            width: 500,
            modal: true,
            close: function (event, ui) {
            }
        });
    }
    init();

    //$(elementIDs.btnRoutingTask).click(function () {
    //    $(elementIDs.routingTaskDialog).dialog({
    //        autoOpen: false,
    //        height: 1000,
    //        width: 1500,
    //        modal: true,
    //        close: function (event, ui) {

    //        }
    //    });

    //    //open the dialog - uses jqueryui dialog
    //    $(elementIDs.routingTaskDialog).dialog('option', 'title', 'Routing Tasks');
    //    $(elementIDs.routingTaskDialog).dialog("open");
    //});

    return {
        show: function () {
            var url = window.location.href;
            if (url.includes("DashBoard"))
            {
                return;
            }
            $(elementIDs.routingTaskDialog).dialog({
                autoOpen: false,
                height: 750,
                width: 1250,
                modal: true,
                close: function (event, ui) {
                   
                }
            });
            initTabs.loadFormUpdates();
            //open the dialog - uses jqueryui dialog
            $(elementIDs.routingTaskDialog).dialog('option', 'title', 'My Tasks');
            $(elementIDs.routingTaskDialog).dialog("open");
            
        }
    }

}();

var ajaxDialog;
ajaxDialog = ajaxDialog || (function () {
    $(function () {
        $(document).ajaxStart(function () {
            ajaxDialog.showPleaseWait();
        });
        $(document).ajaxStop(function () {
            ajaxDialog.hidePleaseWait();
        });
        $(document).ajaxError(function () {
            ajaxDialog.hidePleaseWait();
        });
        $('#pleaseWaitDialog').dialog({
            modal: true,
            autoOpen: false,
            draggable: false,
            resizable: false,
            zIndex: 99999,
            width: 150,
            hegith: 125,
            closeOnEscape: false,
            open: function (event, ui) {
                $('#pleaseWaitDialog').parent().find('.ui-dialog-titlebar-close').parent().hide();
            }
        });
    });
    return {
        showPleaseWait: function () {
            $('#pleaseWaitDialog').dialog('open');
        },
        hidePleaseWait: function () {
            $('#pleaseWaitDialog').dialog('close');
        },
    };
})();
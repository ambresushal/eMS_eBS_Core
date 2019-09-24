var uiblocker;
uiblocker = uiblocker || (function () {
    var elementIDs = {
        blockUI: "blockui",
        blockUIJQ: "#blockui",
        messageJQ: "#block-message"
    }

    $(function () {
        $(elementIDs.blockUIJQ).dialog({
            modal: true,
            autoOpen: false,
            draggable: false,
            resizable: false,
            zIndex: 1005,
            width: 150,
            hegith: 150,
            closeOnEscape: false,
            open: function (event, ui) {
                $(elementIDs.blockUIJQ).parent().find('.ui-dialog-titlebar-close').parent().hide();
            }
        });
    });
    return {
        showPleaseWait: function (message) {
            var startDate = new Date();

            $(elementIDs.blockUIJQ).dialog('open');
            $(elementIDs.messageJQ).text(message);
        },
        hidePleaseWait: function () {
            $(elementIDs.blockUIJQ).dialog('close');

            var completionDate = new Date();
        },
    };
})();
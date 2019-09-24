var messageDialog;
messageDialog = messageDialog || (function () {
    $(function () {
        $('#messagedialog').dialog({
            modal: true,
            autoOpen: false,
            draggable: true,
            resizable: true,
            zIndex: 1005,
            closeOnEscape: false,
            title: 'Message',
            buttons: {
                Close: function () {
                    $(this).dialog("close");
                }
            }
        });
    });
    return {
        show: function (message) {
            $('#messagedialog').find('div p').html(message);
            $('#messagedialog').dialog('open');
        },
        hide: function () {
            $('#messagedialog').dialog('close');
        },
    };
})();

var productQueuedDialog;
productQueuedDialog = productQueuedDialog || (function () {
    $(function () {
        $('#productQueuedDialog').dialog({
            modal: true,
            autoOpen: false,
            draggable: true,
            resizable: true,
            zIndex: 1005,
            closeOnEscape: false,
            title: 'Message',
            buttons: {
                Close: function () {
                    $(this).dialog("close");
                    window.location.reload();
                }
            },
            close: function () {
                $(this).dialog("close");
                window.location.reload();
            }
        });
    });
    return {
        show: function (message) {
            $('#productQueuedDialog').find('div p').text(message);
            $('#productQueuedDialog').dialog('open');
        },
        hide: function () {
            $('#productQueuedDialog').dialog('close');
        }
    };
})();

var confirmDialog;
confirmDialog = confirmDialog || (function () {
    $(function () {
        $('#confirmdialog').dialog({
            modal: true,
            autoOpen: false,
            draggable: true,
            resizable: true,
            zIndex: 1005,
            closeOnEscape: false,
            title: 'Confirm',
            buttons: {
                Confirm: function () {
                    confirm();
                },
                Close: function () {
                    $(this).dialog("close");
                }
            }
        });

        function confirm() {
            confirmDialog.callbackroutine();
        }
    });
    return {
        show: function (message, callback) {
            confirmDialog.callbackroutine = callback;
            $('#confirmdialog').find('div p').text(message);
            $('#confirmdialog').dialog('open');
        },
        hide: function () {
            $('#confirmdialog').dialog('close');
        },
    };
})();


var extConfirmDialog;
extConfirmDialog = extConfirmDialog || (function () {
    $(function () {
        $('#extConfirmdialog').dialog({
            modal: true,
            autoOpen: false,
            draggable: true,
            resizable: true,
            zIndex: 1005,
            closeOnEscape: false,
            title: 'Confirm',
            buttons: {
                Confirm: function () {
                    confirm(true);
                },
                Close: function () {
                    confirm(false);
                    $(this).dialog("close");
                }
            }
        });

        function confirm(e) {
            extConfirmDialog.callbackroutine(e);
        }
    });
    return {
        show: function (message, callback) {
            extConfirmDialog.callbackroutine = callback;
            $('#extConfirmdialog').find('div p').text(message);
            $('#extConfirmdialog').dialog('open');
        },
        hide: function () {
            $('#extConfirmdialog').dialog('close');
        },
    };
})();


var autoConfirmDialog;
autoConfirmDialog = autoConfirmDialog || (function () {
    $(function () {
        $('#autoSavedialog').dialog({
            modal: true,
            autoOpen: false,
            draggable: true,
            resizable: true,
            zIndex: 1005,
            closeOnEscape: false,
            title: 'Save Document',
            buttons: {
                Yes: function () {
                    yes(true);
                },
                No: function () {
                    yes(false);
                    $(this).dialog("close");
                }
            }
        });

        function yes(e) {
            autoConfirmDialog.callbackroutine(e);
        }
    });
    return {
        show: function (message, callback) {
            autoConfirmDialog.callbackroutine = callback;
            $('#autoSavedialog').find('div p').text(message);
            $('#autoSavedialog').dialog('open');
        },
        hide: function () {
            $('#autoSavedialog').dialog('close');
        },
    };
})();


var folderLockOverridenDialog;
folderLockOverridenDialog = folderLockOverridenDialog || (function () {
    $(function () {
        $('#folderLockOverridenDialog').dialog({
            modal: true,
            autoOpen: false,
            draggable: true,
            resizable: true,
            zIndex: 1005,
            closeOnEscape: false,
            close: function () {
                window.location.reload();
            },
            title: 'Unlocked Folder Version',
            buttons: {
                OK: function () {
                    yes(true);
                    FolderLockAction.ISOVERRIDEDIALOGACTION = 1;
                },
            }
        });

        function yes(e) {
            folderLockOverridenDialog.callbackroutine(e);
        }
    });
    return {
        show: function (message, callback) {
            folderLockOverridenDialog.callbackroutine = callback;
            $('#folderLockOverridenDialog').find('div p').html(message);
            $('#folderLockOverridenDialog').dialog('open');
        },
        hide: function () {
            $('#folderLockOverridenDialog').dialog('close');
        },
    };
})();

//Folder Lock Warning Dialog
var folderLockWarningDialog;
folderLockWarningDialog = folderLockWarningDialog || (function () {
    $(function () {
        $('#folderLockDialog').dialog({
            modal: true,
            autoOpen: false,
            draggable: true,
            resizable: true,
            zIndex: 200,
            closeOnEscape: false,
            title: 'Locked Folder Version',
            buttons: [
            //    {
            //    id: "btnUnlock",
            //    text: "Unlock Folder Version",
            //    click: function () {
            //        confirmUnlock();
            //    }
            //},
            {
                id: "FolderLockDialogView",
                text: "View Folder Version",
                click: function () {
                    confirm();
                }
            }]
        })
        function confirmUnlock() {
            folderLockWarningDialog.callbackroutine1();
        }

        function confirm() {
            folderLockWarningDialog.callbackroutine2();
        }


    });
    return {
        show: function (message, callback1, callback2) {
            //checkFolderLockClaims();
            folderLockWarningDialog.callbackroutine1 = callback1;
            folderLockWarningDialog.callbackroutine2 = callback2;
            $('#folderLockDialog').find('div p').html(message);
            $('#folderLockDialog').dialog('open');
        },
        hide: function () {
            $('#folderLockDialog').dialog('close');
        },
    };
})();

//FormInstance Delete Dialog.
var formInstanceDeleteDialog;
formInstanceDeleteDialog = formInstanceDeleteDialog || (function () {
    $(function () {
        $('#formInsatanceDialog').dialog({
            modal: true,
            autoOpen: false,
            draggable: true,
            resizable: true,
            zIndex: 1005,
            closeOnEscape: false,
            title: 'Confirm',
            buttons: [{
                id: "btnConfirm",
                text: "Confirm",
                "class": 'confirmButtonClass',
                click: function () {
                    confirm();
                }
            },
           {
               id: "btnClose",
               text: "Close",
               click: function () {
                   $(this).dialog("close");
               }
           }]
        });

        function confirm() {
            confirmDialog.callbackroutine();
        }
    });
    return {
        show: function (message, callback) {
            confirmDialog.callbackroutine = callback;
            $('#formInsatanceDialog').find('div p').html(message);
            $('#formInsatanceDialog').dialog('open');
        },
        hide: function () {
            $('#formInsatanceDialog').dialog('close');
        },
    };
})();

var duplicateMessageDialog;
duplicateMessageDialog = duplicateMessageDialog || (function () {
    $(function () {
        $('#duplicateMessagedialog').dialog({
            modal: true,
            autoOpen: false,
            draggable: true,
            resizable: true,
            zIndex: 1005,
            closeOnEscape: false,
            title: 'Message',
            open: function (event, ui) {
            }
        }).parent().find('.ui-dialog-titlebar-close').hide();
    });
    return {
        show: function (message) {
            $('#duplicateMessagedialog').find('div p').text(message);
            $('#duplicateMessagedialog').dialog('open');
        },
        hide: function () {
            $('#duplicateMessagedialog').dialog('close');
        },
    };
})();

//FormInstance Delete Dialog.
var acceleratedConfirmDialog;
acceleratedConfirmDialog = acceleratedConfirmDialog || (function () {
    $(function () {
        $('#acceleratedConfirmDialog').dialog({
            modal: true,
            autoOpen: false,
            draggable: true,
            resizable: true,
            zIndex: 1005,
            closeOnEscape: false,
            title: 'Confirm',
            buttons: [{
                id: "btnConfirm",
                text: "Proceed",
                click: function () {
                    confirm();
                }
            },
           {
               id: "btnClose",
               text: "Remove Accelerated",
               click: function () {
                   $(this).dialog("close");
                   removeAccelerated();
               }
           }]
        });

        function confirm() {
            confirmDialog.callbackroutine();
        }
        function removeAccelerated() {
            $('#approvaltypelist option').removeAttr('selected');
        }
    });
    return {
        show: function (message, callback) {
            confirmDialog.callbackroutine = callback;
            $('#acceleratedConfirmDialog').find('div p').html(message);
            $('#acceleratedConfirmDialog').dialog('open');
        },
        hide: function () {
            $('#acceleratedConfirmDialog').dialog('close');
        },
    };
})();


var yesNoConfirmDialog;
yesNoConfirmDialog = yesNoConfirmDialog || (function () {
    $(function () {
        $('#yesNoConfirmDialog').dialog({
            modal: true,
            autoOpen: false,
            draggable: true,
            resizable: true,
            zIndex: 1005,
            closeOnEscape: false,
            title: 'Confirm',
            open: function (event, ui) {
            },
            buttons: {
                Yes: function () {
                    confirm(true);
                },
                No: function () {
                    confirm(false);
                    $(this).dialog("close");
                }
            }
        }).parent().find('.ui-dialog-titlebar-close').hide();

        function confirm(e) {
            yesNoConfirmDialog.callbackroutine(e);
        }
    });
    return {
        show: function (message, callback) {
            yesNoConfirmDialog.callbackroutine = callback;
            $('#yesNoConfirmDialog').find('div p').text(message);
            $('#yesNoConfirmDialog').dialog('open');
        },
        showFormatted: function (message, callback) {
            yesNoConfirmDialog.callbackroutine = callback;
            $('#yesNoConfirmDialog').find('div p').html(message);
            $('#yesNoConfirmDialog').dialog('open');
        },
        hide: function () {
            $('#yesNoConfirmDialog').dialog('close');
        },
    };
})();

var documentLockOverridenDialog;
documentLockOverridenDialog = documentLockOverridenDialog || (function () {
    $(function () {
        $('#documentLockOverridenDialog').dialog({
            modal: true,
            autoOpen: false,
            draggable: true,
            resizable: true,
            zIndex: 1005,
            closeOnEscape: false,
            close: function () {
                //window.location.reload();
            },
            title: 'Unlocked Document',
            buttons: {
                Ok: function () {
                    yes(true);
                    //FolderLockAction.ISOVERRIDEDIALOGACTION = 1;
                },
            }
        });

        function yes(e) {
            documentLockOverridenDialog.callbackroutine(e);
        }
    });
    return {
        show: function (message, callback) {
            documentLockOverridenDialog.callbackroutine = callback;
            $('#documentLockOverridenDialog').find('div p').html(message);
            $('#documentLockOverridenDialog').dialog('open');
        },
        hide: function () {
            $('#documentLockOverridenDialog').dialog('close');
        },
    };
})();




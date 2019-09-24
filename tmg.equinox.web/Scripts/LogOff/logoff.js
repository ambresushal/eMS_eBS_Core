var logOff = function () {
    var URLs = {
        logOff: "/Account/LogOff"
    };
    function init() {
        $(document).ready(function () {
            logOff();
        });
    }
    function logOff() {
        $('.logofflink').click(function () {
            $('#confirmdialog').dialog({
                title: 'Confirm Log Off',
                height: 120,
                buttons: {
                    Yes: function () {
                        confirm();
                    },
                    No: function () {
                        $(this).dialog("close");
                    }
                }


            });
            function confirm() {
                confirmDialog.callbackroutine();
            }
            confirmDialog.show(LogOff.logOffMsg, function () {
                window.location.href = URLs.logOff;
            });
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
    }
    init();
    return {
        LogOff: function () {
            logOff();
        }
    };
}();


//var idleTimeOut = function () {
//    var URLs = {
//        LogOffs: "/Account/LogOn/"
//    };
//    var sessionWarningTimer = null;
//    var redirectToWelcomePageTimer = null;
//    var flag = false;

//    function init() {
//        $(document).ready(function () {
//            $(this).mousemove(function (e) {
//                if (!flag) {
//                    clearTimeout(sessionWarningTimer);
//                    clearTimeout(redirectToWelcomePageTimer);
//                    start();
//                }
//            });
//            start();
//        });
//    }

//    function start() {
//        sessionWarningTimer = setTimeout(function () {
//            flag = true;
//            //minutes left for expiry
//            var minutesForExpiry = (parseInt(sessionTimeout) - parseInt(sessionTimeoutWarning));
//            var message = "Your session will expire in another " + minutesForExpiry + " mins. Do you want to extend the session?";
//            //Confirm the user if he wants to extend the session
//            $('#confirmdialog').dialog({
//                title: 'Session Expiration Alert',
//                height: 140,
//                buttons: {
//                    Yes: function () {
//                           if (redirectToWelcomePageTimer != null) {
//                                clearTimeout(redirectToWelcomePageTimer);
//                            }
//                            start();
//                            $(this).dialog("close");
//                    },
//                    No: function () {
//                        $(this).dialog("close");
//                    }
//                }
//            });
//            confirmDialog.show(message);
//        }, parseInt(sessionTimeoutWarning) * 60 * 1000); 

//       //To redirect to the welcome page
//        redirectToWelcomePageTimer = setTimeout(function () {
//            $('#messagedialog').dialog({
//                title: 'Session Expired',
//                height: 140,
//                buttons: {
//                    'OK': function () {
//                        window.location.href = URLs.LogOffs;
//                        $(this).dialog("close");
//                    }
//                }
//            });
//            messageDialog.show("Session expired. You will be redirected to Login page");
//          }, parseInt(sessionTimeout) * 60 * 1000); 
//    }

//    init();

//    return {
//        LogOff: function () {
//            logOff();
//        }
//    };
//}();



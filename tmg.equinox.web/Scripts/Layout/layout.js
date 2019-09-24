
$(window).bind('resize', function () {
    $(".ui-jqgrid").each(function (index, grid) {
        var gridID = $(grid).attr("ID").substring(5);
        var width = $($(grid).parent().closest('div').first()).width();
        $("#" + gridID).jqGrid().setGridWidth(width);

        var headers = $("#" + gridID).jqGrid("getGridParam", "groupHeader");
        if (headers) {
            $("#" + gridID).jqGrid('destroyGroupHeader', true);

            $("#" + gridID).jqGrid('setGroupHeaders',
                { useColSpanStyle: false, groupHeaders: headers.groupHeaders });
        }
    });
}).trigger('resize');

function setMenu(menuID) {
    $("ul.list-sidebar > li > a").each(function (val, index) {
        var childID = $(this).attr("ID").toLowerCase();
        if (childID == menuID.toLowerCase()) {
            //$(this).addClass('active');
            $(this).parent().attr("class", "active");
        }
        else {
            $(this).removeAttr("class", "active");
        }
    });
}

var historyManager = (function () {
    var variables = {
        refreshState: 'setRefreshState'
    }
    var URLs = {
        loginUrl: '/Account/LogOff'
    }



    function loadEvent() {
        window.onload = function (e) {
            if (typeof history.pushState === BrowserNavigationSetting.pushStateType) {
                if (history.state == variables.refreshState) {
                    this.location = URLs.loginUrl;
                }
                else {
                    history.pushState(BrowserNavigationSetting.forwardState, null, null);
                    window.onpopstate = function () {
                        this.location = URLs.loginUrl;
                    }
                }
            }
        }
    }

    function keyEvent() {
        document.onkeydown = function (e) {
            var keycode = e.keyCode;
            if (keycode == 116 || (window.event.ctrlKey && keycode == 82)) {
                history.pushState(variables.refreshState, null, null);

            }
        }
    }

    return {
        init: function (isEnable) {
            if (isEnable) {
                if (history.state != null && history.state != variables.refreshState) {
                    this.location = URLs.loginUrl;
                }
                loadEvent();
                keyEvent();
            }
        }
    }
}());

historyManager.init(false);
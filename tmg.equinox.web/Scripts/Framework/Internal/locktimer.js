

var SingletonService = (function () {
    var unLockTimeoutValue;
    
    function createInstanceAndGetDate() {
        var unLockTimeoutSer = new unLockTimeoutService();
        return unLockTimeoutSer.getUnlockTimeoutSettingsData();
    }
    
    return {
        getUnLockTimeoutValue: function () {
            if (unLockTimeoutValue===undefined) {
                unLockTimeoutValue = createInstanceAndGetDate();
                GLOBAL.timeInMinAfterCurrentUserSessionLockOut = unLockTimeoutValue;
            }
            return unLockTimeoutValue;
        }
    };
})();



function showError(xhr) {
    if (xhr.status == 999)
        this.location = '/Account/LogOn';
    else
        messageDialog.show(JSON.stringify(xhr));
}

var unLockTimeoutService = function () {
 
    var URLs = {
        GetUnLockTimeOutSetting: "/Settings/GetUnLockTimeOutSetting",
        ReleaseDocumentAndSectionLockOnTimeOut: "/FormInstance/ReleaseDocumentAndSectionLockOnTimeOut"

    };
    tenantId = 1;


    function getUnlockTimeoutSettingsData() {
        var currentInstance = this;
        var url = URLs.GetUnLockTimeOutSetting.replace(/\{tenantId\}/g, currentInstance.tenantId);
        var promise = ajaxWrapper.getJSON(url);
        promise.done(function (data) {
            if (data != null) {
                GLOBAL.timeInMinAfterCurrentUserSessionLockOut = parseInt(data);
            }
        });
        promise.fail(showError);
    }

    function ReleaseAlllockOnTimeOut() {
        var currentInstance = this;
        var url = URLs.ReleaseDocumentAndSectionLockOnTimeOut;
        var promise = ajaxWrapper.postJSON(url);
        promise.done(function (data) {
        });
        promise.fail(showError);
    }

    return {
        getUnlockTimeoutSettingsData: function () {
            return getUnlockTimeoutSettingsData();
        },
        ReleaseAlllockOnTimeOut: function () {
            return ReleaseAlllockOnTimeOut();
        }
    }
}

$(function () {

    $("body").on('click keypress', function () {
        InitialiseUserLockSession();
    });
});

function ResetCurrentUserLockSession() {
    GLOBAL.minuteTick = 0;
    GLOBAL.clearAllTimeout();
}

function StartCurrentUserLockSessionTimer() {
    //GLOBAL.minuteTick = GLOBAL.minuteTick + GLOBAL.waitIntervalPeriod ;// increment by so many seconds
    GLOBAL.minuteTick++;
    
    if (GLOBAL.minuteTick >= GLOBAL.timeInMinAfterCurrentUserSessionLockOut) {
        GLOBAL.clearAllTimeout();
        var unLockTimeoutSer = new unLockTimeoutService();
        unLockTimeoutSer.ReleaseAlllockOnTimeOut();
        messageDialog.show(DocumentSectionLockMsg.autoUnLock);
        return; 
    }
    GLOBAL.setTimeout("StartCurrentUserLockSessionTimer()", GLOBAL.waitIntervalPeriod); //1000 == 1 sec, , 1 min = 60000, so for 5 min ,3,00,000
}

function InitialiseUserLockSession()
{
    ResetCurrentUserLockSession();
    GLOBAL.setTimeout("StartCurrentUserLockSessionTimer()", GLOBAL.waitIntervalPeriod); //1000 == 1 sec, , 1 min = 60000, so for 5 min ,3,00,000
}

/*
SessionLockManager = function()
{
    this.timeInMinAfterSessionLockOut = 30; // change this to change session time out (in seconds).
    this.minuteTick = 0;
    this.waitIntervalPeriod = 1000;
    this.tick;
}

SessionLockManager.prototype.startThisLockSessionTimer = function ()
{

    this.minuteTick++;


    if (this.minuteTick > this.timeInMinAfterSessionLockOut) {
        clearTimeout(this.tick);
        // window.location = "/Logout.aspx";
        return;
    }
    if (this.minuteTick == 1)
        this.tick = setTimeout(this.startThisLockSessionTimer(), this.waitIntervalPeriod); //1000 == 1 sec, , 1 min = 60000, so for 5 min ,3,00,000
}
SessionLockManager.prototype.getWaitIntervalPeriod = function () {

    return this.waitIntervalPeriod;
}

SessionLockManager.prototype.resetThisLockSession = function () {

    this.minuteTick = 0;
}




var SingletonLockTimer = (function () {
    var instance;
   
    function createInstance() {
            var sessionLockManager = new SessionLockManager();
            return sessionLockManager;
        }
    
    return {
        getInstance: function () {
            if (!instance) {
                debugger;
                instance = createInstance();
                instance.startThisLockSessionTimer();
            }
            return instance;
        }
    };
})();




/*
    var instance1 = Singleton.getInstance();
    var instance2 = Singleton.getInstance();
*/



if (GLOBAL.timeInMinAfterCurrentUserSessionLockOut === undefined)
{
    GLOBAL.timeInMinAfterCurrentUserSessionLockOut=SingletonService.getUnLockTimeoutValue();
}

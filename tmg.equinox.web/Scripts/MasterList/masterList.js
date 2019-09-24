var masterList = function () {
    this.masterList = [];

    var currentInstance = this;
    var URLs = {
        getMasterList: "/FormDesign/MasterListFormDesignList?tenantId=1",
        getGetMasterFolderVersion: "/FolderVersion/GetMasterFolderVersion"
    };
    var elementIDs = {
        dopMasterListJQ: "#dopMasterList",
        btnsubmitJQ: "#btnsubmit",
        masterListEffectiveDateFilter: "#ml_EffDat_fltr",
        dopMasterListHelpBlockJQ: "#dopMasterList-help-block"
    };

    function init() {
        $(document).ready(function () {
            loadMasterList();
        });
    }

    function registerButtonEvents() {
        $(elementIDs.btnsubmitJQ).unbind("click");
        $(elementIDs.btnsubmitJQ).click(function () {
            var formDesignType = $(elementIDs.dopMasterListJQ).val();
            if (validate(formDesignType)) {
                renderMasterList(formDesignType);
            }
        });

        $(elementIDs.masterListEffectiveDateFilter).click(function () {
            $(this).datepicker();
        });

        $(elementIDs.dopMasterListJQ).unbind("change");
        $(elementIDs.dopMasterListJQ).change(function () {
            $(elementIDs.dopMasterListHelpBlockJQ).parent('div').removeClass("has-error");
            $(elementIDs.dopMasterListHelpBlockJQ).text("");
        });
    }

    function validate(formDesignType) {
        var valid = false;
        if (formDesignType == 0) {
            $(elementIDs.dopMasterListHelpBlockJQ).parent('div').addClass("has-error");
            $(elementIDs.dopMasterListHelpBlockJQ).text("Select Master List");
            valid = false;
        }
        else {
            valid = true;

        }
        return valid;
    }

    function loadMasterList() {
        var currentInstance = this;
        var promise = ajaxWrapper.getJSON(URLs.getMasterList);

        //register ajax success callback
        promise.done(function (items) {
            if (items) {
                $.each(items, function (i, item) {
                    $(elementIDs.dopMasterListJQ).append(
                        "<option value=" + item.FormDesignName + ">" + item.DisplayText + "</option>"
                    );
                });
            }
        });

        //register ajax failure callback
        promise.fail(showError);
        registerButtonEvents();
    }

    function renderMasterList(Data) {
        var masterlisturl = "/FolderVersion/IndexML?tenantId=1&folderType=" + Data + "&folderId=1";
        window.location.href = masterlisturl;
    }
    init();
    return {
        loadMasterList: function () {
            loadMasterList();
        }
    }
}();




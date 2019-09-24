var servicedesignversionoutputdialog = function () {

    var elementIDs = {
        dialog: 'servicedesignversionoutputdialog',
        dialogJQ: '#servicedesignversionoutputdialog',
        dialogTab: 'servicedesignversionoutputtabs',
        dialogTabJQ: '#servicedesignversionoutputtabs',
        jsonoutput: 'jsonoutput',
        jsonoutputJQ: '#jsonoutput',
        xmloutput: 'xmloutput',
        xmloutputJQ: '#xmloutput',
    }

    var Urls = {
        loadOutput: '/ServiceDesign/GetPreview?tenantId=1&serviceDesignVersionID={serviceDesignVersionID}&serviceDesignID={serviceDesignID}'
    }

    function init() {
        //jqueryui tabs

        //register dialog for grid row add/edit
        $(elementIDs.dialogJQ).dialog({
            autoOpen: false,
            height: 450,
            width: 650,
            modal: true
        });

        $(elementIDs.dialogTabJQ).tabs();

    }

    function loadOutput(serviceDesignID, serviceDesignVersionID) {
        var data = {
            tenantID: 1,
            serviceDesignID: serviceDesignID,
            serviceDesignVersionID: serviceDesignVersionID
        };

        //ajax call to add/update
        var promise = ajaxWrapper.postJSON(Urls.loadOutput, data);
        //register ajax success callback
        promise.done(function (result) {
            if (result) {
                $(elementIDs.jsonoutputJQ).val(result.JsonOutput);
                $(elementIDs.xmloutputJQ).val(result.XmlOutput);
                $(elementIDs.dialogJQ).dialog('open');
            }
            else {
                $(elementIDs.dialogJQ).dialog('hide');
                messageDialog.show(Common.errorMsg);
            }
        });
        //register ajax failure callback
        promise.fail(showError);
    }

    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    init();

    return {
        show: function (serviceDesignID, serviceDesignVersionID, serviceDesignName, versionNumber) {
            var title = 'Output for ' + serviceDesignName + ' Version - ' + versionNumber;
            $(elementIDs.jsonoutputJQ).val('');
            $(elementIDs.xmloutputJQ).val('');
            loadOutput(serviceDesignID, serviceDesignVersionID);
            $(elementIDs.dialogJQ).dialog('option', 'title', title);
            $(elementIDs.dialogJQ).dialog('open');
        }
    }
}();
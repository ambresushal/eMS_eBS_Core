function formDesignPreview(formDesignVersionId, formName, tenantId) {
    this.formDesignVersionId = formDesignVersionId;
    this.formName = formName;
    this.tenantId = tenantId;
    this.formDesignData = null;

    this.elementIDs = {
        //container element for form instances
        forminstancelistjq: '#foldertabs',
    };

    this.URLs = {
        //get Folder Version Workflow
        getFormDesignDataForPreview: '/FormDesignPreview/GetFormDesignDataForPreview?tenantId={tenantId}&formDesignVersionId={formDesignVersionId}'
    };
}

formDesignPreview.prototype.addForm = function () {
    var currentInstance = this;
    //create tabs
    currentInstance.tabs = $(currentInstance.elementIDs.forminstancelistjq).tabs();

    var tabName = currentInstance.formName;
    var tabTemplate = "<li><a href='#{href}' id='form-tab-" + currentInstance.formDesignVersionId + "'>{label}</a></li>";
    //replace based on version being loaded for this form
    var li = $(tabTemplate.replace(/#\{href\}/g, '#tab' + currentInstance.formDesignVersionId).replace(/\{label\}/g, tabName));
    //create div for the tab page content
    currentInstance.tabs.find('.ui-tabs-nav').append(li);
    currentInstance.tabs.append("<div id='tab" + +currentInstance.formDesignVersionId + "'></div>");
    $(currentInstance.elementIDs.forminstancelistjq).tabs('refresh');
    $(currentInstance.elementIDs.forminstancelistjq).tabs({ active: -1 });
}

formDesignPreview.prototype.loadPreview = function () {
    var currentInstance = this;
    currentInstance.addForm();

    var url = currentInstance.URLs.getFormDesignDataForPreview.replace(/\{formDesignVersionId\}/g, currentInstance.formDesignVersionId).replace(/\{tenantId\}/g, currentInstance.tenantId);
    var promise = ajaxWrapper.getJSON(url);
    //register ajax success callback
    promise.done(function (xhr) {
        var formInstanceBuilderObj = new formInstanceBuilder(currentInstance.tenantId, 0, 0, 0, currentInstance.formDesignVersionId, currentInstance.formDesignVersionId, currentInstance.formName);
        formInstanceBuilderObj.loadPreview(xhr);
    });
    //register ajax failure callback
    promise.fail(currentInstance.showError);

}

formDesignPreview.prototype.showError = function (xhr) {
    if (xhr.status == 999)
        window.location.href = '/Account/LogOn';
    else
        messageDialog.show(JSON.stringify(xhr));
}
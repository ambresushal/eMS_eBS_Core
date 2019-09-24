var expressionBuilderRules = function () {
    this.URLs = {
        getSourceFormInstanceData: "/ExpressionBuilder/RunCustomDataSources?tenantId={tenantId}&folderVersionId={folderVersionId}&formInstanceId={formInstanceId}&formDesignVersionId={formDesignVersionId}&targetElementId={targetElementId}&targetElementPath={targetElementPath}"
    }
}

expressionBuilderRules.prototype.runCustomDataSources = function (tenantId, folderVersionId, formInstanceId, formDesignVersionId, targetElementId, targetElementPath) {
    var currentInstance = this;
    var url = currentInstance.URLs.getSourceFormInstanceData.replace(/{tenantId}/g, tenantId).replace(/{folderVersionId}/g, folderVersionId).replace(/{formInstanceId}/g, formInstanceId).replace(/{formDesignVersionId}/g, formDesignVersionId).replace(/{targetElementId}/g, targetElementId).replace(/{targetElementPath}/g, targetElementPath);
    var promise = ajaxWrapper.getJSON(url);
    return promise;
}


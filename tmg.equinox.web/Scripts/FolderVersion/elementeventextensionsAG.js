var eventextensionsAG = function () {
    return {
        registerEvent: function (formInstanceBuilder, data, path) {
            //if (path == "OONGroups.ManualOverride") {
            //    var evHandler = new eventhandlergrp(formInstanceBuilder, data, path);
            //    evHandler.registerevent(data, path);
            //}
            if (path == "OONNumberofGroups.IndicatethenumberofOutofNetworkgroupingsofferedexcludingInpatientHospi") {
                var evHandler = new eventhandlernog(formInstanceBuilder, data, path);
                evHandler.init(data, path);
            }
            else {
                var evHandler = new eventhandlergrp(formInstanceBuilder, data, path);
                evHandler.registerevent(data, path);
            }
        }
    }
}();

var eventhandlergrp = function (formInstanceBuilder) {
    this.formInstanceBuilder = formInstanceBuilder;
    this.allowCellClick = false;
    this.eventHandlers = this.eventhandlermethods();
    this.btnRealoadFormDataJQ = '#btnReloadFormData';
}

eventhandlergrp.prototype.registerevent = function (data, path) {
    if (path == "OONGroups.ManualOverride") {
        var res = getDataProperty(path, data);
        if (res !== undefined) {
            $.observe(data, path, this.eventHandlers.clickHandler);
            var currentInstance = this;
            if (res == true || res == "True") {
                currentInstance.allowCellClick = true;
            }
            else {
                currentInstance.allowCellClick = false;
            }
            var repeaterBuilders = currentInstance.formInstanceBuilder.repeaterBuilders;
            $.each(repeaterBuilders, function (idx, val) {
                var repeaterId = '#repeater' + val.design.Name + val.formInstanceId;
                if (currentInstance.allowCellClick == false) {
                    $(repeaterId + " .ui-icon-plus").css("pointer-events", "none");
                    $(repeaterId + " .ui-icon-minus").css("pointer-events", "none");
                    $(repeaterId + " .ui-icon-copy").css("pointer-events", "none");
                }
                else {
                    $(repeaterId + " .ui-icon-plus").css("pointer-events", "auto");
                    $(repeaterId + " .ui-icon-minus").css("pointer-events", "auto");
                    $(repeaterId + " .ui-icon-copy").css("pointer-events", "auto");
                }
                this.gridOptions.onCellClicked = function (event) {
                    if (currentInstance.allowCellClick == false) {
                        event.api.stopEditing();
                        return false;
                    }
                    else {
                        return true;
                    }
                }
            });
        }
    }
    else {
        var res = getDataProperty(path, data);
        if (res !== undefined) {
            $.observe(data, path, this.eventHandlers.clickHandler);
        }
    }
}

eventhandlergrp.prototype.eventhandlermethods = function () {
    var currentInstance = this;
    return {
        clickHandler: function (ev, eventArgs) {
            if (eventArgs.value == true) {
                var dialogMessageYes = eventArgs.path == "ManualOverride" ? 'Do you want to enable manual override. If manual override is enabled, the system will not auto generate values for the OON Groups.' :
                'Do you want to enable manual override. If manual override is enabled, the system will not auto generate values from Medicare.';
                var dialogMessageNo = eventArgs.path == "ManualOverride" ? 'All changes made to OON Groups will be lost. The system will auto generate values for the OON Groups.' :
                    'All changes made will be lost. The system will auto generate values from Medicare.';
                //display message
                yesNoConfirmDialog.show(dialogMessageYes, function (e) {
                    yesNoConfirmDialog.hide();
                    if (e) {
                        //set to checked and refresh
                        currentInstance.allowCellClick = true;
                        if (eventArgs.path == "ManualOverride") {
                            var repeaterBuilders = currentInstance.formInstanceBuilder.repeaterBuilders;
                            $.each(repeaterBuilders, function (idx, val) {
                                var repeaterId = '#repeater' + val.design.Name + val.formInstanceId;
                                $(repeaterId + " .ui-icon-plus").css("pointer-events", "auto");
                                $(repeaterId + " .ui-icon-minus").css("pointer-events", "auto");
                                $(repeaterId + " .ui-icon-copy").css("pointer-events", "auto");
                            });
                        }
                    }
                });

            }
            if (eventArgs.value == false) {
                //display message
                yesNoConfirmDialog.show(dialogMessageNo, function (e) {
                    yesNoConfirmDialog.hide();
                    if (e) {
                        //run OON Groups
                        currentInstance.allowCellClick = false;
                        if (eventArgs.path == "ManualOverride") {
                            var url = "/FormInstance/ProcessOONGroups?tenantId={tenantId}&anchorFormInstanceId={anchorFormInstanceId}&formInstanceId={formInstanceId}";
                            url = url.replace(/\{tenantId\}/g, currentInstance.formInstanceBuilder.tenantId).replace(/\{anchorFormInstanceId\}/g, currentInstance.formInstanceBuilder.anchorFormInstanceID).replace(/\{formInstanceId\}/g, currentInstance.formInstanceBuilder.formInstanceId);
                            var promise = ajaxWrapper.getJSON(url);
                            //register ajax success callback
                            promise.done(function (xhr) {
                                var repeaterBuilders = currentInstance.formInstanceBuilder.repeaterBuilders;
                                $.each(repeaterBuilders, function (idx, val) {
                                    var repeaterId = '#repeater' + val.design.Name + val.formInstanceId;
                                    val.gridOptions.api.destroy();
                                });
                                $(currentInstance.btnRealoadFormDataJQ).trigger("click");
                            });
                        }
                        else {
                            var formInstanceData = currentInstance.formInstanceBuilder.form.getFormInstanceData();

                            formInstanceData.formInstanceData = formInstanceData.formInstanceData.replace(/\[Select One]/g, '').replace(/\Select One/g, '');
                            var global = true;
                            var url = "/FormInstance/SaveFormInstanceData";
                            var promise = ajaxWrapper.postJSON(url, formInstanceData, global);
                            //register ajax success callback
                            promise.done(function (xhr) {
                                //returning sections updated instead of ServiceResult
                                if ((typeof (xhr) === "object" && xhr.length != undefined) || (typeof (xhr) === "object" && xhr.Result == ServiceResult.SUCCESS)) {
                                    $(currentInstance.btnRealoadFormDataJQ).trigger("click");
                                }
                            });
                            //register ajax failure callback
                            promise.fail(this.showError);
                        }
                    }
                });
            }
        }
    }
}

var eventhandlernog = function (formInstanceBuilder) {
    this.formInstanceBuilder = formInstanceBuilder;
    this.allowCellClick = false;
    this.eventHandlers = this.eventhandlermethods();
    this.btnRealoadFormDataJQ = '#btnReloadFormData';
}

eventhandlernog.prototype.init = function (data, path) {
    $.observe(data, path, this.eventHandlers.changeHandler);
    var res = getDataProperty(path, data);
    var currentInstance = this;
}

eventhandlernog.prototype.eventhandlermethods = function () {
    var currentInstance = this;
    return {
        changeHandler: function (ev, eventArgs) {
        }
    }
}

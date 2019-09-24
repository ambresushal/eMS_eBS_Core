var vbidextension = function (anchorId, folderVersionId, folderId) {
    this.URLs = {
        GetVBIDSettings: "/VBID/GetVBIDSettings",
        GetPackages: "/VBID/GetPackages",
        DeleteFormInstance: '/FormInstance/DeleteFormInstance?folderId={folderId}&tenantId=1&folderVersionId={folderVersionId}&formInstanceId={formInstanceId}',
        UpdateForDeletedPackage: "/VBID/UpdateForDeletedPackage"
    };
    this.elementIDs = {
        btnManageVBIDPackagesJQ: '#btnManageVBIDPackages'
    };
    this.anchorId = anchorId;
    this.folderVersionId = folderVersionId;
    this.folderId = folderId;
    this.settings = null;
}

vbidextension.prototype.getVBIDSettings = function () {
    //check if VBID is allowed
    //ajax call to determine if PBP View fields are set appropriately
    var currentInstance = this;
    var input = {
        anchorId: currentInstance.anchorId,
        folderVersionId: currentInstance.folderVersionId
    }
    var promise = ajaxWrapper.postJSON(this.URLs.GetVBIDSettings, input);
    return promise;
}

vbidextension.prototype.setVBIDMenu = function () {
    var currentInstance = this;
    var URLs = {
        AddPackages: "/VBID/ManagePackages"
    };
    $(currentInstance.elementIDs.btnManageVBIDPackagesJQ).hide();
    $(currentInstance.elementIDs.btnManageVBIDPackagesJQ).off('click');
    var promise = this.getVBIDSettings();
    promise.done(function (xhr) {
        if (xhr != null) {
            if (xhr.DoesyourplanincludeMAUniformityFlexibilitywithreductionsincostoradditi == "1" ||
                xhr.ValueBasedInsuranceDesignAttestationIattestthat1thebenefitsenteredcomp == "true" ||
                xhr.DoyouofferSpecialSupplementalBenefitsforChronicallyIII == "1" ||
                (xhr.IndicatenumberofTiersinyourPartDbenefit >= 2 && xhr.IsthisaVBIDPlan == "YES")) {
                $(currentInstance.elementIDs.btnManageVBIDPackagesJQ).show();
                $(currentInstance.elementIDs.btnManageVBIDPackagesJQ).on('click', function () {
                    //get packages
                    var promiseSettings = currentInstance.getVBIDSettings();
                    promiseSettings.done(function (xhr) {
                        var input = {
                            anchorId: currentInstance.anchorId,
                            folderVersionId: currentInstance.folderVersionId
                        }
                        currentInstance.settings = xhr;
                        var promiseGetPackages = ajaxWrapper.postJSON(currentInstance.URLs.GetPackages, input);
                        promiseGetPackages.done(function (xhr) {
                            manageVBIDPackagesDialog.show(currentInstance, xhr);
                        });
                    });
                });
            }
            else {
                var input = {
                    anchorId: currentInstance.anchorId,
                    folderVersionId: currentInstance.folderVersionId
                }
                var promiseGetPackagesCount = ajaxWrapper.postJSON(currentInstance.URLs.GetPackages, input);
                promiseGetPackagesCount.done(function (xhr) {
                    if (xhr.CostSharingCount > 0 || xhr.AdditionalBenefitsCount > 0 || xhr.RxCount > 0) {
                        var input = {
                            ReducedCostSharePackageRequired: 0,
                            ReducedCostSharePackageCurrent: xhr.costSharingCount,
                            AdditionalBenefitPackageRequired: 0,
                            AdditionalBenefitPackageCurrent: xhr.additionalBenefitsCount,
                            RxPackageRequired: 0,
                            RxPackageCurrent: xhr.rxCount,
                            anchorId: currentInstance.anchorId,
                            folderVersionId: currentInstance.folderVersionId
                        };
                        var promiseRemovePackages = ajaxWrapper.postJSON(URLs.AddPackages, input);
                        promiseRemovePackages.done(function (xhr) {
                            messageDialog.show("The VBID View has been deleted. The folder will now be reloaded.");
                            window.location.reload();
                        });
                    }
                });
            }
        }
    });
}

vbidextension.prototype.deleteFormInstance = function (formInstanceId, formName) {
    var currentInstance = this;
    var message = FolderVersion.deleteConfirmationMsg.replace(/\{#documnentName}/g, 'the document');

    formInstanceDeleteDialog.show(message, function () {
        formInstanceDeleteDialog.hide();
        var url = currentInstance.URLs.DeleteFormInstance.replace(/\{folderId\}/g, currentInstance.folderId)
                                                         .replace(/\{tenantId\}/g, 1)
                                                         .replace(/\{formInstanceId\}/g, formInstanceId)
                                                         .replace(/\{folderVersionId\}/g, currentInstance.folderVersionId);
        var promise = ajaxWrapper.getJSON(url);
        //register ajax success callback
        promise.done(function (xhr) {
            if (xhr.Result === ServiceResult.SUCCESS) {
                var input = {
                    anchorId: currentInstance.anchorId,
                    folderVersionId: currentInstance.folderVersionId
                };
                var url = currentInstance.URLs.UpdateForDeletedPackage;
                var promiseVBID = ajaxWrapper.postJSON(url, input);
                promiseVBID.done(function (xhr) {
                    messageDialog.show("The VBID View has been deleted. The folder will now be reloaded.");
                    window.location.reload();
                });
            }
            else {
                messageDialog.show(Common.errorMsg);
            }
        });
    });
}

//package dialog
//get packages
var manageVBIDPackagesDialog = function () {
    var vbidExt;
    var URLs = {
        AddPackages: "/VBID/ManagePackages"
    }
    var elementIDs = {
        manageVBIDPackagesDialog: 'manageVBIDPackagesDialog',
        manageVBIDPackagesDialogJQ: '#manageVBIDPackagesDialog',
        btnVBIDPackages: '#btnVBIDPackages',
        packagesCS: '#packagesCS',
        packagesCSCurrent: '#packagesCSCurrent',
        packagesAB: '#packagesAB',
        packagesABCurrent: '#packagesABCurrent',
        packagesPartD: '#packagesPartD',
        packagesPartDCurrent: '#packagesPartDCurrent'
    }

    function init() {
        $(document).ready(function () {
            $(elementIDs.manageVBIDPackagesDialogJQ).dialog({
                autoOpen: false,
                height: 500,
                width: 900,
                modal: true,
                title: 'Manage VBID Packages'
            });
        });
    }
    init();

    return {
        show: function (vbidExtension, xhr) {
            $(elementIDs.manageVBIDPackagesDialogJQ).dialog("open");
            //register click event to submit changes
            vbidExt = vbidExtension;
            $(elementIDs.packagesCS).val(xhr.CostSharingCount);
            $(elementIDs.packagesCSCurrent).val(xhr.CostSharingCount);
            $(elementIDs.packagesAB).val(xhr.AdditionalBenefitsCount);
            $(elementIDs.packagesABCurrent).val(xhr.AdditionalBenefitsCount);
            $(elementIDs.packagesPartD).val(xhr.RxCount);
            $(elementIDs.packagesPartDCurrent).val(xhr.RxCount);
            $(elementIDs.packagesCS).off('focusout');
            $(elementIDs.packagesCS).on('focusout', function () {
                var val = parseInt($(this).val());
                var valCurrent = parseInt($(elementIDs.packagesCSCurrent).val());

                var success = true;
                if (val > 15) {
                    messageDialog.show("Number of Packages is limited to 15.");
                    success = false;
                }
                if (val < 0) {
                    messageDialog.show("Number of Packages cannot be lower than 0.");
                    success = false;
                }
                if (success == false) {
                    $(this).val(valCurrent);
                }
            });
            $(elementIDs.packagesAB).off('focusout');
            $(elementIDs.packagesAB).on('focusout', function () {
                var val = parseInt($(this).val());
                var valCurrent = parseInt($(elementIDs.packagesABCurrent).val());
                var success = true;
                if (val > 15) {
                    messageDialog.show("Number of Packages is limited to 15.");
                    success = false;
                }
                if (val < 0) {
                    messageDialog.show("Number of Packages cannot be lower than 0.");
                    success = false;
                }
                if (success == false) {
                    $(this).val(valCurrent);
                }
            });

            //19A
            if (vbidExtension.settings != null &&
                vbidExtension.settings.IsthisaVBIDPlan == "NO" &&
                vbidExtension.settings.DoesyourplanincludeMAUniformityFlexibilitywithreductionsincostoradditi == "2" &&
                vbidExtension.settings.SelectwhattypeofbenefityourSSBCIincludes != "true") {
                $(elementIDs.packagesCS).attr('disabled', 'disabled');
            }
            else if (vbidExtension.settings != null &&
                vbidExtension.settings.IsthisaVBIDPlan == "YES" &&
                vbidExtension.settings.DoesyourplanincludeMAUniformityFlexibilitywithreductionsincostoradditi == "2" &&
                vbidExtension.settings.SelectwhattypeofbenefityourSSBCIincludes != "true" &&
                vbidExtension.settings.DoesyourVBIDbenefitofferPartCreductionsincostoradditionalbenefits == "2") {
                $(elementIDs.packagesCS).attr('disabled', 'disabled');
            }
            else {
                $(elementIDs.packagesCS).removeAttr('disabled');
            }

            //19B
            if (vbidExtension.settings != null &&
                vbidExtension.settings.IsthisaVBIDPlan == "NO" &&
                vbidExtension.settings.DoesyourplanincludeMAUniformityFlexibilitywithreductionsincostoradditi == "2" &&
                vbidExtension.settings.SelectwhattypeofbenefityourSSBCIincludesAB != "true") {
                $(elementIDs.packagesAB).attr('disabled', 'disabled');
            }
            else if (vbidExtension.settings != null &&
                vbidExtension.settings.IsthisaVBIDPlan == "YES" &&
                vbidExtension.settings.DoesyourplanincludeMAUniformityFlexibilitywithreductionsincostoradditi == "2" &&
                vbidExtension.settings.SelectwhattypeofbenefityourSSBCIincludesAB != "true" &&
                vbidExtension.settings.DoesyourVBIDbenefitofferPartCreductionsincostoradditionalbenefits == "2") {
                $(elementIDs.packagesAB).attr('disabled', 'disabled');
            }
            else {
                $(elementIDs.packagesAB).removeAttr('disabled');
            }

            //RX
            if (vbidExtension.settings != null &&
                ((vbidExtension.settings.IsthisaVBIDPlan != null && vbidExtension.settings.IsthisaVBIDPlan == "NO") ||
                (vbidExtension.settings.DoesyourPlanofferaPrescriptionPartDbenefit != null && vbidExtension.settings.DoesyourPlanofferaPrescriptionPartDbenefit == "NO"))) {

                $(elementIDs.packagesPartD).attr('disabled', 'disabled');
            }
            else {
                $(elementIDs.packagesPartD).removeAttr('disabled');
                $(elementIDs.packagesPartD).off('focusout');
                $(elementIDs.packagesPartD).on('focusout', function () {
                    var val = parseInt($(this).val());
                    var valCurrent = parseInt($(elementIDs.packagesPartDCurrent).val());
                    var success = true;
                    if (val > 15) {
                        messageDialog.show("Number of Packages is limited to 15.");
                        success = false;
                    }
                    if (val < 0) {
                        messageDialog.show("Number of Packages cannot be lower than 0.");
                        success = false;
                    }
                    if (success == false) {
                        $(this).val(valCurrent);
                    }
                });
            }
            $(elementIDs.btnVBIDPackages).off('click');
            $(elementIDs.btnVBIDPackages).on('click', function () {
                var input = {
                    ReducedCostSharePackageRequired: $(elementIDs.packagesCS).val(),
                    ReducedCostSharePackageCurrent: $(elementIDs.packagesCSCurrent).val(),
                    AdditionalBenefitPackageRequired: $(elementIDs.packagesAB).val(),
                    AdditionalBenefitPackageCurrent: $(elementIDs.packagesABCurrent).val(),
                    RxPackageRequired: $(elementIDs.packagesPartD).val(),
                    RxPackageCurrent: $(elementIDs.packagesPartDCurrent).val(),
                    anchorId: vbidExtension.anchorId,
                    folderVersionId: vbidExtension.folderVersionId
                }
                if (input.ReducedCostSharePackageRequired != input.ReducedCostSharePackageCurrent
                    || input.AdditionalBenefitPackageRequired != input.AdditionalBenefitPackageCurrent
                    || input.RxPackageRequired != input.RxPackageCurrent) {
                    var promiseAddPackages = ajaxWrapper.postJSON(URLs.AddPackages, input);
                    promiseAddPackages.done(function (xhr) {
                        messageDialog.show("The VBID Packages have been created. The folder will now be reloaded.");
                        window.location.reload();
                    });
                }
                else {
                    messageDialog.show("No VBID Packages have been added.");
                }

            });
        }
    }
}();
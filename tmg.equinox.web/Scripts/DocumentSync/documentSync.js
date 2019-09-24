var documentSync = function () {
    function init() {
        $(document).ready(function () {
            $("#documentSync").steps({
                headerTag: "h1",
                bodyTag: "div",
                contentContainerTag: "div",
                cssClass: "wizard compareSyncWizards",
                autoFocus: true,
                transitionEffect: $.fn.steps.transitionEffect.none,
                onStepChanging: function (e, currentIndex, newIndex) {
                    return stepChanging(e, currentIndex, newIndex);
                },
                onStepChanged: function (e, currentIndex, priorIndex) {
                    adjustIframeHeight();
                    return stepChanged(e, currentIndex, priorIndex);
                },
                onFinished: function (event, currentIndex) {
                    window.location.href = "/DocumentSync/Index";
                }
            });
            $("#documentSync").css('display', 'block');
            loadSelectSourceDocument();
        });
    }
    init();
    var documentSyncSteps = {
        sourceDocuments: null,
        setupCompare: null,
        setupRepeater: null,
        selectTargetDocuments: null,
        compareDocuments: null,
        syncDocument: null
    };

    var documentSyncData = {
        SourceDocument: null,
        SelectedTargetDocuments: null,
        SelectedMacro: null,
        SelectedRepeater: { repeaterList: null },
        IsSyncInLastVisit : false
    };

    function stepChanging(e, currentIndex, newIndex) {
        switch (currentIndex) {
            case 0:
                if (documentSyncData.SourceDocument == null) {
                    messageDialog.show("Please select a Source Document.");
                    return false;
                }
                break;
            case 1:
                if (newIndex > currentIndex && documentSyncData.SelectedMacro == null) {
                    messageDialog.show("Please select a Macro.");
                    return false;
                }
                else {
                    var setup = new csSetupCompare(currentIndex, newIndex, documentSyncData);
                    if (documentSyncData.SelectedMacro != null && documentSyncData.SelectedMacro.Template != null) {
                        if (setup.vallidateMacro(JSON.stringify(documentSyncData.SelectedMacro.Template))) {
                            setup.updateMacro();
                        }
                        else {
                            messageDialog.show("Please select atleast one element for compare.");
                            return false;
                        }
                    }
                }
                break;
            case 2:
                if (newIndex > currentIndex) {
                    var isAllSetupComplete = true;
                    if (documentSyncData.SelectedRepeater.repeaterList != null) {
                        $.each(documentSyncData.SelectedRepeater.repeaterList, function (ind, rpt) {
                            if (!rpt.IsSet) {
                                isAllSetupComplete = false;
                                return false;
                            }
                        });
                    }
                   // if (isAllSetupComplete) {
                        var setup = new csSetRepeaterCriteria(currentIndex, newIndex, documentSyncData);
                        setup.updateMacro();
                   // }
                }
                break;
            case 3:
                if ((newIndex > currentIndex) && (documentSyncData.SelectedTargetDocuments == null || documentSyncData.SelectedTargetDocuments.length == 0)) {
                    messageDialog.show("Please select Target Documents for Compare and Sync.");
                    return false;
                }
                break;
            case 4:
                break;
            case 5:
                break;
        }
        return true;
    }

    function stepChanged(event, currentIndex, priorIndex) {
        if (currentIndex > priorIndex) {
            switch (currentIndex) {
                case 1:
                    loadSetupCompare(currentIndex, priorIndex);
                    break;
                case 2:
                    loadSetRepeaterCriteria(currentIndex, priorIndex);
                    break;
                case 3:
                    loadSelectTargetDocuments(currentIndex, priorIndex);
                    break;
                case 4:
                    loadCompareDocuments(currentIndex, priorIndex);
                    break;
                case 5:
                    loadSyncDocuments(currentIndex, priorIndex);
                    break;
            }
        }
        else {
            switch (currentIndex) {
                case 2:
                    documentSyncSteps.setupRepeater.skipStep('previous');
                    break;
                case 3:
                    if (documentSyncSteps.selectTargetDocuments != null) {
                        documentSyncSteps.selectTargetDocuments.loadTargetDocumentsGrid();
                        documentSyncSteps.selectTargetDocuments.loadTargetSelectedDocumentsGrid();
                    }
                    break;
                case 4:
                    if (documentSyncSteps.compareDocuments != null && documentSyncData.IsSyncInLastVisit == true) {
                        documentSyncSteps.compareDocuments.loadSelectedDocumentsGrid();
                    }
                    break;
            }
        }
        return true;
    }

    function loadSelectSourceDocument() {
        //add additional parameters that need to be passed
        var nextStep = new csSelectSourceDocument(documentSyncData);
        documentSyncSteps.sourceDocuments = nextStep;
        nextStep.process();
    }

    function loadSetupCompare(currentIndex, priorIndex) {
        //add additional parameters that need to be passed
        var nextStep = new csSetupCompare(currentIndex, priorIndex, documentSyncData);
        documentSyncSteps.setupCompare = nextStep;
        nextStep.process();
    }

    function loadSetRepeaterCriteria(currentIndex, priorIndex) {
        //add additional parameters that need to be passed
        var nextStep = new csSetRepeaterCriteria(currentIndex, priorIndex, documentSyncData);
        documentSyncSteps.setupRepeater = nextStep;
        nextStep.process();
    }

    function loadSelectTargetDocuments(currentIndex, priorIndex) {
        //add additional parameters that need to be passed
        var nextStep = new csSelectTargetDocuments(currentIndex, priorIndex, documentSyncData);
        documentSyncSteps.selectTargetDocuments = nextStep;
        nextStep.process();
    }

    function loadCompareDocuments(currentIndex, priorIndex) {
        //add additional parameters that need to be passed
        var nextStep = new csCompareDocuments(currentIndex, priorIndex, documentSyncData);
        documentSyncSteps.compareDocuments = nextStep;
        nextStep.process();
    }

    function loadSyncDocuments(currentIndex, priorIndex) {
        //add additional parameters that need to be passed
        var nextStep = new csSyncDocuments(currentIndex, priorIndex, documentSync, documentSyncData);
        documentSyncSteps.syncDocument = nextStep;
        nextStep.process();
    }

    function adjustIframeHeight() {
        var $body = $('body'),
            $iframe = $body.data('iframe.fv');
        if ($iframe) {
            // Adjust the height of iframe
            $iframe.height($body.height());
        }
    }

}();




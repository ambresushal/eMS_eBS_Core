var journalInstance = function () {
    //variable to hold the instance
    var instance;
    var oldFormInstanceID = undefined;

    function enableJournalAddMode(sectionDesignData, formInstanceId, folderVersionId, tenantId, journalGridData, buttonEvent, isSotView) {
        this.formDesignData = sectionDesignData;
        this.sectionDesignData = sectionDesignData;
        this.formInstanceId = formInstanceId;
        this.folderVersionId = folderVersionId;
        this.tenantId = tenantId;
        this.journalGridData = journalGridData;
        this.buttonEvent = buttonEvent;
        this.isSot = isSotView;
        var currentInstance = this;
        instance = currentInstance;

        return {
            load: function () {
                var elementIDs = {};
                var that = this;

                if (currentInstance.isSot == true) {
                    currentInstance.sectionDesignData = currentInstance.formDesignData.Sections[0];
                    elementIDs = {
                        btnJournalEntry: "#btnJournalEntry",
                        formTab: '#tab' + currentInstance.formInstanceId,
                        section: "#tab" + currentInstance.formInstanceId,
                    };
                }
                else {
                    elementIDs = {
                        btnJournalEntry: "#btnJournalEntry",
                        formTab: "#form-tab-" + currentInstance.formInstanceId,
                        section: "#section" + currentInstance.sectionDesignData.Name + currentInstance.formInstanceId,
                    };
                }
                if ($(elementIDs.btnJournalEntry).hasClass("clicked")) {
                    this.highlightButton(false, $(elementIDs.btnJournalEntry));
                    this.disableJournalAddMode(currentInstance.sectionDesignData.Name, currentInstance.formInstanceId, oldFormInstanceID);
                    currentInstance.buttonEvent.preventDefault();
                    currentInstance.buttonEvent.stopPropagation();
                }
                else {
                    var elementListBind = "input, select, textarea, div.panel-heading-gray, div.repeater-grid, table";
                    var elementListUnbind = "input, select, textarea, img.ui-datepicker-trigger, div.panel-heading-gray, div.repeater-grid, table";
                    this.highlightButton(true, $(elementIDs.btnJournalEntry));
                    //tab Start
                    $(elementIDs.formTab).addClass("cursor-crosshair");
                    $(elementIDs.formTab).bind("click", function (tabEvent) {
                        tabEvent.preventDefault();
                        tabEvent.stopPropagation();
                        $(elementIDs.section).find(elementListUnbind).each(function (idx, control) {
                            if ($(control).hasClass("selected-control"))
                                $(control).removeClass("selected-control");
                        });
                        if ($(elementIDs.formTab)) {
                            if ($(elementIDs.btnJournalEntry).hasClass("clicked")) {
                                var sectionTrail = $(this).data("journal");
                                currentInstance.isExistJournal(sectionTrail, undefined, currentInstance.formInstanceId, currentInstance.folderVersionId, currentInstance.sectionDesignData.Name, currentInstance.tenantId, currentInstance.journalGridData);
                            }
                        }
                    });
                    //tab end
                    $(elementIDs.btnJournalEntry).addClass("clicked");
                    $(elementIDs.section).find(elementListUnbind).unbind("click");
                    $(elementIDs.section).addClass("cursor-crosshair");
                    $(elementIDs.section).find(elementListBind).addClass("cursor-crosshair");
                    //bind click event to elements
                    $(elementIDs.section).find(elementListBind).bind("click", function (sectionEvent) {
                        sectionEvent.preventDefault();
                        sectionEvent.stopPropagation();
                        $(elementIDs.section).find(elementListUnbind).each(function (idx, control) {
                            if ($(control).hasClass("selected-control"))
                                $(control).removeClass("selected-control");
                        });
                        if ($(this).is("input") || $(this).is("select") || $(this).is("textarea") || $(this).is("div.panel-heading-gray") || $(this).is("div.repeater-grid")) {
                            $(this).addClass("selected-control");
                            $(elementIDs.section).removeClass("cursor-crosshair");
                            var link = $(this).data("journal");
                            if (currentInstance.isSot) { that.setCurrentSection(link); }
                            var sectionTrail = currentInstance.sectionDesignData.Label + getElementHierarchy(currentInstance.sectionDesignData, link);
                            var attr = getElementDetails(currentInstance.sectionDesignData, link);
                            if (sectionTrail != undefined && link != undefined) {
                                currentInstance.isExistJournal(sectionTrail, attr, currentInstance.formInstanceId, currentInstance.folderVersionId, currentInstance.sectionDesignData.Name, currentInstance.tenantId, currentInstance.journalGridData);
                            }
                        }
                    });
                }
            },
            setCurrentSection: function (path) {
                var currentSection = path.split('.');
                if (currentSection.length > 1) {
                    sectionDetails = currentInstance.formDesignData.Sections.filter(function (ct) {
                        return ct.FullName == currentSection[0];
                    });

                    currentInstance.sectionDesignData = sectionDetails[0];
                } else {
                    currentInstance.sectionDesignData = currentInstance.formDesignData.Sections[0];
                }
            },
            highlightButton: function (mode, element) {
                currentInstance.highlightButton(mode, element);
            },
            disableJournalAddMode: function (sectionName, formInstanceId, oldFormInstanceID) {
                currentInstance.disableJournalAddMode(sectionName, formInstanceId, oldFormInstanceID);
            },
            isExistJournal: function (sectionTrail, elementName, formInstanceId, folderVersionId, sectionName, tenantId, journalGridData) {
                currentInstance.isExistJournal(sectionTrail, elementName, formInstanceId, folderVersionId, sectionName, tenantId, journalGridData);
            },
            getJournalInstance: function () {
                return currentInstance;
            },
            isSot: function () {
                return currentInstance.isSot;
            }
        }
    }
    enableJournalAddMode.prototype.highlightButton = function (mode, element) {
        if (mode) {
            $(element).addClass("selected-button");
        } else {
            $(element).removeClass("selected-button");
        }
    }
    enableJournalAddMode.prototype.disableJournalAddMode = function (sectionName, formInstanceId, oldFormInstanceID) {
        var currentInstance = this;
        var disableElementIDs = {};

        if (currentInstance.isSot == true) {
            disableElementIDs = {
                btnJournalEntry: "#btnJournalEntry",
                formTab: "#tab" + formInstanceId,
                section: "#tab" + formInstanceId,
                elementList: "input, select, textarea, img.ui-datepicker-trigger, div.panel-heading-gray, div.repeater-grid, table",
            };
        } else {
            disableElementIDs = {
                btnJournalEntry: "#btnJournalEntry",
                formTab: "#form-tab-" + formInstanceId,
                section: "#section" + sectionName + formInstanceId,
                elementList: "input, select, textarea, img.ui-datepicker-trigger, div.panel-heading-gray, div.repeater-grid, table",
            };
        }

        $(disableElementIDs.btnJournalEntry).removeClass("clicked");
        $(disableElementIDs.section).find(disableElementIDs.elementList).each(function (idx, control) {
            if ($(control).hasClass("selected-control"))
                $(control).removeClass("selected-control");
        });
        $(disableElementIDs.section).removeClass("cursor-crosshair");
        $(disableElementIDs.section).find(disableElementIDs.elementList).unbind("click");
        $(disableElementIDs.section).find(disableElementIDs.elementList).removeClass("cursor-crosshair");
        //tab
        $(disableElementIDs.formTab).removeClass("cursor-crosshair");
        $(disableElementIDs.formTab).removeClass("selected-control");
        //old tab
        $("#form-tab-" + oldFormInstanceID).removeClass("cursor-crosshair");
        $("#form-tab-" + oldFormInstanceID).removeClass("selected-control");
    }

    enableJournalAddMode.prototype.isExistJournal = function (sectionTrail, elementName, formInstanceId, folderVersionId, sectionName, tenantId, journalGridData) {
        var existJournalGridData = new Array();
        for (var i = 0; i < journalGridData.length; i++) {
            var listObject = journalGridData[i];

            var fieldName = listObject.FieldName;
            var fieldPath = listObject.FieldPath;
            var FormInstanceID = listObject.FormInstanceID;
            var actionID = listObject.ActionID;

            if (elementName != undefined) {
                if (fieldPath == sectionTrail && fieldName == elementName.Label && FormInstanceID == formInstanceId && actionID == JournalEntryAction.YESOPEN.toString()) {
                    existJournalGridData.push(listObject);
                }
            }
            else {
                if (fieldPath == sectionTrail && fieldName == JournalEntry.attributeNotApplicableMsg && FormInstanceID == formInstanceId && actionID == JournalEntryAction.YESOPEN.toString()) {
                    existJournalGridData.push(listObject);
                }
            }
        }
        if (existJournalGridData.length > 0) {
            var journalID = existJournalGridData[0].JournalID;
            var field = existJournalGridData[0].FieldName;
            var description = existJournalGridData[0].Description;
            var actionid = existJournalGridData[0].ActionID;
            journalEntryDialog.updateJournal(field, sectionTrail, actionid, formInstanceId, folderVersionId, sectionName, description, journalID, formInstanceId);
        }
        else {
            journalEntryDialog.addJournal(sectionTrail, elementName, formInstanceId, folderVersionId, sectionName, tenantId, formInstanceId);
        }
    }
    return {
        //this method will return a singleton object for folderVersion at any point of time.
        getInstance: function (sectionDesignData, formInstanceId, folderVersionId, tenantId, journalGridData, buttonEvent, isSourceSot) {
            if (instance === undefined) {
                instance = new enableJournalAddMode(sectionDesignData, formInstanceId, folderVersionId, tenantId, journalGridData, buttonEvent, isSourceSot);
            }
            else if (instance.sectionDesignData == sectionDesignData && instance.formInstanceId == formInstanceId && instance.folderVersionId == folderVersionId && instance.tenantId == tenantId && instance.journalGridData == journalGridData && instance.buttonEvent == buttonEvent) {
                return instance;
            }
            else {
                instance = undefined;
                instance = new enableJournalAddMode(sectionDesignData, formInstanceId, folderVersionId, tenantId, journalGridData, buttonEvent, isSourceSot);
            }
            return instance;
        }
    }
}();
var FolderViewMode = {
    DefaultView: 'Default',
    SOTView: 'SOT'
};

var RichTextBoxRenderingFrom = {
    Cell: 'Cell',
    Column: 'Column'
};

var ServiceResult = {
    SUCCESS: 0,
    WARNING: 1,
    FAILURE: 2
};

var WorkFlowState = { //Modified No-1: Needs updation when master states modify   
    PSoTPreparation: 'PSoT Preparation',
    MarketMeetingUpdates: 'Market Meeting Updates',
    MarketApproval: 'Market Approval',
    PBPValidation: 'PBP Validation',
    TBCOOPCApproval: 'TBC / OOPC Approval',
    PartDApproval: 'Part D Approval',
    PreBenchmarkApproval: 'Pre-Benchmark Approval',
    DeskReviewInternalReviewI: 'Desk Review / Internal Review I',
    BenchmarkInternalReviewII: 'Benchmark / Internal Review II',
    TaskFolderVersionId: 0,
    ProductConfig: "Product Config",
    InitialDocumentReview: "Initial Document Review",
    PostBenchmarkApproval: "Post - Benchmark Approval",
    FinalDocumentReview: "Final Document Review",
    SBGeneration: "SB Generation",
    ANOCGeneration: "ANOC Generation",
    EOCGeneration: "EOC Generation",
    ProductReview: "Product Review"
};

var ApprovalStatus = {
    APPROVED: 1,
    NOTAPPROVED: 2,
    COMPLETED: 6
};

var CustomRegexValidation = {
    STARTWITHAPLHABETS: '^[a-zA-Z0-9][ a-zA-Z0-9!@#$%^&*()_\\`.:;=?+,/\'"\n-]*$',
    AVOIDSPECIALCHARACTER: '^[a-zA-Z0-9- ]*$'
};

var Icons = {
    //Calender icon path
    CalenderIcon: '/Content/css/custom-theme/images/calender-icon.svg',
};


var FolderVersionState =
{
    INPROGRESS: 1,
    BASELINED: 2,
    RELEASED: 3,
    INPROGRESS_BLOCKED: 4
};

var VersionType =
{
    New: 1,
    Retro: 2
};

var FormDesignName =
{
    FormDesignMedicare: "Medicare",
};

var FormDesignVersionType =
    {
        Minor: 1,
        Major: 2
    };

var FormTypes = {
    CURRENTFORMDESIGNID: 0,
    MASTERLISTFORMID: 1,
    PRODUCTFORMDESIGNID: 3,
    HSBMASTERLISTFORMID: 1082,
    HSBADMINFORMDESIGNID: 1083,
    MEDICALFORMDESIGNID: 1084,
    //Please note these variables are hard coded for QHP sites & needs to be updated on http://192.168.100.4:5770/. 
    //PRODUCTFORMID: new Array(18/*For Product-QHP*/, 35/*For Product-FACETS*/, 41/*For Product*/),
    // PRODUCTFACETSQHPFORMID: [43/*For Product+Facets+QHP*/],
    ADMINFORMID: 2,
    DOCUMENTREFERENCEFORMDESIGNID: 8,
    DENTALFORMID: 1101,
    HSBVISIONFORMID: 1100,
    HSBBENADMIN: 1103,
    MEDICAREFORMDESIGNID: 2359,//1276,
    COMMERCIALMEDICAL: 2405,
    ANTHEMANCHORDESIGN: 1316,
    LIMITSMASTERLIST: 1350,
    COMMERCIALMEDICAL: 2405,
    ANTHEMANCHORDESIGN: 1316,
    LIMITSMASTERLIST: 1350,
    ANOCChartViewFormDesignID: 2385,
    SBCDESIGN: 2444,
    VBIDDESIGN: 2409,
    RX: 2447,
    DENTAL: 2448,
    PlansandBenefitTemplate: 2424,
    SystemConfiguration: 2392, 
    LIMITSML: 2433
}


//Global variable to hold JQ
var JQGridSettings = {
    DateFormatterOptions: { newformat: "m/d/Y" },
    DateTimeFormatterOptions: { srcformat: "ISO8601Long", newformat: "m/d/Y h:i A" }
}

//ag grid licence
var license = {
    agGrid: 'The_Most_Group,_Inc_(aka_Simplify_Healthcare)_eBenefitSync-Core_1Devs27_March_2019__MTU1MzY0NDgwMDAwMA==ca77b0b30a0220cddd06048569240b30'
}

//Journal Entry
var JournalEntryAction = {
    SELECT: 0,
    YESOPEN: 1,
    NO: 2,
}

var Operation = {
    ADD: 0,
    UPDATE: 1,
    DELETE: 2,
    COPY: 3
}
var GridDisplayMode = {
    PQ: 'PQ',
    AG: 'AG'
}
var CurrentGridDisplayMode = 'AG';
//Folder Lock
var FolderLockAction = {
    ISREPEATERACTION: 0,
    ISAUTOSAVEACTION: 0,
    ISOVERRIDEDIALOGACTION: 0,

}

var MasterList = {
    MASTERLISTFOLDERID: 1,
    WMASTERLISTFOLDERID: 7459
}

var MilestoneChecklist = {
    MilestoneChecklistFormDesignID: 1104
}

var WorkFlowDiv = {
    MAXWIDTH: 40,
    MinWidthClass: 'doc-min-length',
    MaxWidthClass: 'doc-max-length',
    AverageTextLength: 17
}

var Paging = {
    ISPAGING: 0
}

var Repeater = {
    MINREPEATERHEIGHT: 100,
    MINCHILDREPEATERHEIGHT: 150,
    MAXREPEATERHEIGHT: 250,
    ROWNUMBER: 25,
}

var LayoutType = {
    CUSTOMLAYOUT: 7
}

var BrowserNavigationSetting =
    {
        forwardState: 'setEmptyForwardState',
        pushStateType: 'function'
    }

var GlobalUpdateWizard =
    {
        SetUp: 'setupID',
        ElementSelection: 'elementSelectionID',
        UpdateSelection: 'updateSelectionID',
        GenerateIAS: 'generateIASID',
        SetUpStepId: 1,
        ElementSelectionStepId: 2,
        UpdateSelectionStepId: 3,
        GenerateIASStepId: 4
    }

var GlobalUpdateStatus =
{
    InProgress: "In Progress",
    Complete: "Complete",
    IASInProgress: "IAS Generation In Progress",
    IASComplete: "IAS Generation Complete",
    ValidationInProgress: "Validation In Progress",
    ErrorLogComplete: "Error Log Generation Complete",
    IASFailed: "IAS Generation Failed",
    ErrorLogFailed: "Error Log Generation Failed",
    IASExecutionFailed: "IAS Execution Failed",
    IASExecutionInProgress: "IAS Execution In Progress",
    IASExecutionComplete: "IAS Execution Complete",
    PendingFinzalization: "Finalization",
    ValidationStatus: 'Validation',
    IASScheduledforExecution: 'IAS Scheduled for Execution',
    InProgressSymbol: "1",
    CompletedSymbol: "2",
    ErrorSymbol: "3",
    NASymbol: "4",
    RealtimeSymbol: "5",
    ScheduledSymbol: "6"

}
var BatchExecutionStatus =
{
    Complete: 1,
    Incomplete: 2
}
var ExecutionType = {
    Realtime: "Realtime",
    Scheduled: "Scheduled"
}

var Finalized = []

var Role = {
    ProductSME: 31,
    ProductDesignerLevel1: 26,
    Viewer: 23,
    TMGSuperUser: 24,
    ClientSuperUser: 25,
    ProductDesignerLevel2: 26,
    ProductCoreAdminDesigner: 32,
    ReViewer: 29,

}

var ClientRolesForFormDesignAccess = [Role.ClientSuperUser];
var ActiveRuleExecutionLogger = "False";

var Category = {
    General: 1
}
var FacetStatus = {
    Queued: 1,
    InProgress: 2,
    Complete: 4
}

var ResetPassword = {
    DefaultPassword: 123
}

var MasterListFolder = {
    MASTERLIST: "MasterList",
    MASTERLISTHSB: "MasterList_HSB"

}

var ReportFormDesignID = {
    Admin_HSB: 1083,
    Medical_HSB: 1084,
    Vision_HSB: 1100,
    Dental_HSB: 1101,
    STD_HSB: 1102,
    BenAdmin_HSB: 1122

}

var FormDesignTypeID = {
    ANCHOR: 1,
    MASTERLIST: 2,
    REPORT: 3,
    ADMIN: 4,
    FACETS: 5
}

var OpenDocumentStatus = {
    Open: "open",
    Close: "close"
}

var WFCategoriesGroups = {
    General: "General"
}
var AntWorkFlowState = {
    NewSalesNotification: "New Sales Notification",
    BPDCreation: "BPD Creation",
    BPDPendingApproval: "BPD Pending Approval",
    BPDDistribution: "BPD Distribution",
    ChangeNotification: "Change Notification",
    BPDUpdate: "BPD Update"
}

var TravelandLodgingConstants = {
    BoneMarrowDoner: "Bone Marrow Donor Search/Travel and Lodging",
    Comments: "Comments"
}

var AppealsDefaultValues = {
    OneEighty: "180",
    Sixty: "60",
    SevetyTwo: "72",
    Thirty: "30",
    FourString: "four",
    FourtyFive: "45",
    ExpeditedConcurrent: "For pre-service claims involving urgent/concurrent care, the member may proceed with an Expedited External Review without filing an internal appeal or while simultaneously pursuing an expedited appeal through our internal appeal process."
}

var nonValidateFormDesignID = [2367];


GLOBAL = {
    timeouts: [],//global timeout id arrays
    timeInMinAfterCurrentUserSessionLockOut: undefined, // change this to change session time out (in seconds).
    minuteTick: 0,
    waitIntervalPeriod: 60000, //evry 1 min method to execute 
    applicationName: '',
    clientName: '',
    const_emedicaresync: 'emedicaresync',
    setTimeout: function (code, waitIntervalPeriod) {
        this.timeouts.push(setTimeout(code, waitIntervalPeriod));
    },
    clearAllTimeout: function () {
        for (var i = 0; i < this.timeouts.length; i++) {
            window.clearTimeout(this.timeouts[i]); // clear all the timeouts
        }
        this.timeouts = [];//empty the id array
    }
};

function setTotalNotification(totalNotif) {
    console.log(totalNotif);
    $("#messages").html(totalNotif.total);
    if (totalNotif !== undefined) {
        if (totalNotif.message !== undefined) {
            if (totalNotif.message != null) {
                Notify(totalNotif.message, null, null, "success");
            }
        }
    }
}
var timer = 60; var hub = $.connection.notificationHub;


function AutoRefreshMigrationQueue(gridName, spnTimer) {
    timer--;
    if (timer == 0) {
        //"#reportQueue"
        if (gridName != '')
            $(gridName).trigger("reloadGrid");

        var hub = $.connection.notificationHub;
        //Client Call
        hub.client.broadcaastNotif = function (totalNotif) {
            setTotalNotification(totalNotif)
        };


        //$.connection.hub.start().done(function () { });

        $.connection.hub.start()
            .done(function () {
                console.log("Hub Connected!");

                //Server Call
                hub.server.getNotification();

            })
            .fail(function () {
                console.log("Could not Connect!");
            });
        timer = 60;
    }
    if (spnTimer != '')
        $(spnTimer).text(timer + " seconds.").css("font-weight", "Bold");
}

var ManualOverrideSectionList = [
    { sectionName: 'Section B: #1a Inpatient Hospital - Acute', fieldPath: 'aInpatientHospitalServicesAcute.ManualOverrideAInpatientHospitalServicesAcute' },
    { sectionName: 'Section B: #1b Inpatient Hospital - Psychiatric', fieldPath: 'InpatientHospitalPsychiatric.ManualOverrideInpatientHospitalPsychiatric' },
    { sectionName: 'Section B: #2 Skilled Nursing Facility (SNF)', fieldPath: 'SkilledNursingFacilitySNF.ManualOverrideSkilledNursingFacilitySNF' }
];

var customruleHandler = function (FormDesignId, IsMasterList) {
    var rulehandler;
    var medicalRule;
    var rule;
    if (checkIfPQGridLoaded(IsMasterList)) {
        if (FormTypes.MEDICALFORMDESIGNID == FormDesignId || FormTypes.HSBADMINFORMDESIGNID == FormDesignId || FormTypes.DENTALFORMID == FormDesignId
            || FormTypes.HSBVISIONFORMID == FormDesignId || FormTypes.HSBBENADMIN == FormDesignId) {
            medicalRule = new customruleMedicalPQ();
        }
        else 

            rule = new customRulePQ();
    }
    else {
        if (FormTypes.MEDICALFORMDESIGNID == FormDesignId || FormTypes.HSBADMINFORMDESIGNID == FormDesignId || FormTypes.DENTALFORMID == FormDesignId
            || FormTypes.HSBVISIONFORMID == FormDesignId || FormTypes.HSBBENADMIN == FormDesignId) {
            medicalRule = new customruleMedical();
        }
        //else
        //rule = new customRule();
    }
    
if (FormDesignId != FormTypes.DOCUMENTREFERENCEFORMDESIGNID) {
    if (checkIfPQGridLoaded(IsMasterList)) {
            rule = new customRulePQ();
        }
        //else {
        //    rule = new customRule();
        //}
    }
//    else {
//        rule = new customRule();
//}

if (IsMasterList && IsMasterList == true) FormDesignId = FormTypes.MASTERLISTFORMID
    switch (FormDesignId) {
        case FormTypes.MASTERLISTFORMID:     //MasterList 1
        case FormTypes.PRODUCTFORMDESIGNID://Product 3
        case FormTypes.DOCUMENTREFERENCEFORMDESIGNID://Reference 8
            this.rulehandler = rule;
            break;
        case FormTypes.DOCUMENTREFERENCEFORMDESIGNID:     //ReferenceDocument 8
            this.rulehandler = rule;
            break;
        case FormTypes.HSBMASTERLISTFORMID:  //MasterList HSB 1082
        case FormTypes.HSBADMINFORMDESIGNID:  //Admin HSB 1083
        case FormTypes.MEDICALFORMDESIGNID: //Medical HSB 1084
        case FormTypes.DENTALFORMID:    //Dental HSB 1101
        case FormTypes.HSBVISIONFORMID: //Vision HSB 1100
        case FormTypes.HSBBENADMIN:   //Ben Admin 1103
             this.rulehandler = medicalRule;
            //this.rulehandler = new customruleMedical();
        default:
            this.rulehandler = rule;
            break;
    }
    return this.rulehandler;
};
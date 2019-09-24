var masterListDataValues = function () {

    function removeDuplicateMandateDetailServices(masterListData) {
        try {
            for (var i = 0; i < masterListData.MandateDetail.MandateDetailList.length; i++) {
                var mandateMasterListServices = new Array();
                for (var j = 0; j < masterListData.MandateDetail.MandateDetailList[i].MasterListServices.length; j++) {
                    var serviceInfo = masterListData.MandateDetail.MandateDetailList[i].MasterListServices[j];

                    //if (masterListData.MandateDetail.MandateDetailList[i].MandateName === "Preventive 100%") {
                    if (isDuplicate(masterListData.MandateDetail.MandateDetailList[i].MasterListServices, serviceInfo)) {
                        console.log("removed " + JSON.stringify(serviceInfo) + " from " + JSON.stringify(masterListData.MandateDetail.MandateDetailList[i]));
                        masterListData.MandateDetail.MandateDetailList[i].MasterListServices.pop(serviceInfo);
                    }
                    else {
                        mandateMasterListServices.push(serviceInfo);
                    }
                    //}
                }
                masterListData.MandateDetail.MandateDetailList[i].MasterListServices = mandateMasterListServices;
            }
        } catch (e) {

        }

        return masterListData;
    }

    function isDuplicate(masterListServices, service) {
        try {
            var items = masterListServices.filter(function (item) {
                return item.BenefitCategory1 === service.BenefitCategory1 && item.BenefitCategory2 === service.BenefitCategory2 && item.BenefitCategory3 === service.BenefitCategory3 && item.PlaceofService === service.PlaceofService;
            });
            if (items.length > 1)
                return true;
            else
                return false;
        } catch (e) {

        }
        return false;
    }

    return {
        removeDuplicateMandateDetailServices: function (masterListData) {
            return removeDuplicateMandateDetailServices(masterListData);
        }
    }
}();
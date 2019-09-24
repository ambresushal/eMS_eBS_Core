using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.integration.qhplite.DocumentMerger
{
    public class DocumentMerger
    {
        List<QhpDocumentViewModel> _qhpDocumentList;
        List<string> _mappings;
        private static readonly ILog _logger = LogProvider.For<QHPReportQueueManager>();
        public DocumentMerger(List<QhpDocumentViewModel> qhpDocumentList)
        {
            this._qhpDocumentList = qhpDocumentList;
            this._mappings = this.GetListOfArrayToMerge();
        }

        public List<QhpDocumentViewModel> GroupAndMerge(List<QhpPackageGroup> groupedInstances, bool offExchangeOnly)
        {
            List<QhpDocumentViewModel> packages = new List<QhpDocumentViewModel>();
            try
            {
                var groups = from grp in groupedInstances
                             group grp by grp.GroupID into newGroup
                             select newGroup;

                foreach (var group in groups)
                {
                    List<QhpDocumentGroupModel> benefitPackage = new List<QhpDocumentGroupModel>();
                    foreach (var grp in group)
                    {
                        var document = _qhpDocumentList.Where(s => s.DocumentID == grp.ForminstanceID).FirstOrDefault();
                        if (document != null)
                        {
                            benefitPackage.Add(new QhpDocumentGroupModel() { DocumentID = document.DocumentID, DocumentData = JObject.Parse(document.DocumentData) });
                        }
                    }

                    packages.Add(Merge(benefitPackage, offExchangeOnly));
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while grouping documents for a Benefit Packages.";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
                _logger.ErrorException(customMsg, customException);
                throw customException;
            }
            return packages;
        }

        public QhpDocumentViewModel Merge(List<QhpDocumentGroupModel> benefitPackage, bool offExchangeOnly)
        {
            QhpDocumentViewModel package = new QhpDocumentViewModel();
            try
            {
                if (offExchangeOnly) { benefitPackage = this.KeepOffExchangeVariation(benefitPackage); }

                package.DocumentID = benefitPackage[0].DocumentID;
                JObject currentObject = benefitPackage[0].DocumentData;

                for (int i = 1; i < benefitPackage.Count; i++)
                {
                    JObject latestObj = benefitPackage[i].DocumentData;

                    foreach (string item in this._mappings)
                    {
                        List<JToken> currArray = currentObject.SelectToken(item).ToList();
                        List<JToken> lstArray = latestObj.SelectToken(item).ToList();
                        if (currArray[0]["HIOSPlanIDStandardComponent"] != null)
                        {
                            var duplicate = currArray.Where(s => (string)s["HIOSPlanIDStandardComponent"] == (string)lstArray[0]["HIOSPlanIDStandardComponent"]).FirstOrDefault();
                            if (duplicate == null)
                            {
                                currArray.AddRange(lstArray);
                            }
                        }
                        else
                        {
                            currArray.AddRange(lstArray);
                        }
                        latestObj.SelectToken(item).Replace(JToken.FromObject(currArray));
                    }
                    currentObject = latestObj;
                }

                package.DocumentData = JsonConvert.SerializeObject(currentObject);

            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred wile merging documents for a Benefit Package.";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
                _logger.ErrorException(customMsg, customException);
                throw customException;
            }
            return package;
        }

        private List<QhpDocumentGroupModel> KeepOffExchangeVariation(List<QhpDocumentGroupModel> benefitPackage)
        {
            for (int i = 0; i < benefitPackage.Count; i++)
            {
                JObject currentObject = benefitPackage[i].DocumentData;

                foreach (string item in this._mappings)
                {
                    List<JToken> currArray = currentObject.SelectToken(item).ToList();
                    List<JToken> onExchangeRows = null;
                    if (currArray[0]["HIOSPlanIDStandardComponentVariant"] != null)
                    {
                        onExchangeRows = currArray.Where(s => ((string)s["HIOSPlanIDStandardComponentVariant"]).Contains("-01")).ToList();
                    }

                    if (currArray[0]["HIOSPlanIDStandardComponent"] != null)
                    {
                        onExchangeRows = currArray.Where(s => ((string)s["HIOSPlanIDStandardComponent"]).Contains("-01")).ToList();
                    }

                    if (onExchangeRows != null && onExchangeRows.Count > 0)
                    {
                        foreach (var row in onExchangeRows)
                        {
                            currArray.Remove(row);
                        }
                        currentObject.SelectToken(item).Replace(JToken.FromObject(currArray));
                    }
                }
                benefitPackage[i].DocumentData = currentObject;
            }

            return benefitPackage;
        }

        private List<string> GetListOfArrayToMerge()
        {
            List<string> list = new List<string>();
            list.Add("ProductDetails.PlanIdentifiers.PlanIdentifierList");
            list.Add("ProductDetails.PlanAttributes.PlanAttributesList");
            list.Add("ProductDetails.StandardAloneDentalOnly.StandardAloneDentalOnlyList");
            list.Add("ProductDetails.AVCalculatorAdditionalBenefitDesign.AVCalculatorAdditionalBenefitDesignList");
            list.Add("ProductDetails.PlanDates.PlanDatesList");
            list.Add("ProductDetails.GeographicCoverage.GeographicCoverageList");
            list.Add("ProductDetails.AdditionalInformation.PlanLevelURLs");
            list.Add("CostShareVariance.PlanCostSharingAttributes");
            list.Add("SBCScenario.PlanSBCScenarioDetailsList");
            list.Add("Deductibles.DrugDeductibles.PlanDrugDeductibleDetails");
            list.Add("MaximumOutofPocket.PlanMaximumOutofPocketDetails");
            list.Add("HSAHRADetail.HSAHRADetailList");
            list.Add("PlanVariantLevelURLs.PlanVariantLevelURLsList");
            list.Add("PlanBenefitInformation.PlanBenefitDetails");

            return list;
        }
    }
}

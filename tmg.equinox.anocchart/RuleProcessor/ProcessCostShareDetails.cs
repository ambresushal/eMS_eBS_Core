using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.anocchart.GlobalUtilities;
using tmg.equinox.anocchart.Model;
using tmg.equinox.ruleinterpreter.executor;
using tmg.equinox.ruleinterpretercollateral;

namespace tmg.equinox.anocchart.RuleProcessor
{
  public  class ProcessCostShareDetails
    {
        ANOCChartSource _source = null;
        public ProcessCostShareDetails(ANOCChartSource source)
        {
            this._source = source;
        }
        public List<CostShareDetails> CostShareDetailsChange()
        {
            List<CostShareDetails> CostShareDetailsList = BuildCostShareDetailsRepeater();
            
            foreach (var item in CostShareDetailsList)
            {
                if (item.Question.Contains("Copayment"))
                {
                    item.ThisYearValue = ConvertYesNoValue(_source.PreviousPBPViewJsonData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.Copayment.IsthereanenrolleeCopayment").ToString()??string.Empty);
                    item.NextYearValue = ConvertYesNoValue(_source.NextPBPViewJsonData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.Copayment.IsthereanenrolleeCopayment").ToString() ?? string.Empty);
                }
                else if (item.Question.Contains("Coinsurance"))
                {
                    item.ThisYearValue = ConvertYesNoValue(_source.PreviousPBPViewJsonData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase2.IsthereanenrolleeCoinsurance").ToString() ?? string.Empty);
                    item.NextYearValue = ConvertYesNoValue(_source.NextPBPViewJsonData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase2.IsthereanenrolleeCoinsurance").ToString() ?? string.Empty);
                }
                else if (item.Question.Contains("Do you charge the Medicare defined cost shares"))
                {
                    item.ThisYearValue = ConvertYesNoValue(_source.PreviousPBPViewJsonData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.DoyouchargetheMedicaredefinedcostsharesThesearethetotalchargesforallse").ToString() ?? string.Empty);
                    item.NextYearValue = ConvertYesNoValue(_source.NextPBPViewJsonData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.DoyouchargetheMedicaredefinedcostsharesThesearethetotalchargesforallse").ToString() ?? string.Empty);
                }
                else if (item.Question.Contains("Enhanced Benefits"))
                {
                    item.ThisYearValue = ConvertServiceCode(_source.PreviousPBPViewJsonData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase1.Selectenhancedbenefits").ToString() ?? string.Empty);
                    item.NextYearValue = ConvertServiceCode(_source.NextPBPViewJsonData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase1.Selectenhancedbenefits").ToString() ?? string.Empty);

                }
                else if (item.Question.Contains("Is this benefit unlimited for Additional Days"))
                {
                    item.ThisYearValue = ConvertYesNoValue(_source.PreviousPBPViewJsonData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase1.IsthisbenefitunlimitedforAdditionalDays").ToString() ?? string.Empty);
                    item.NextYearValue = ConvertYesNoValue(_source.NextPBPViewJsonData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase1.IsthisbenefitunlimitedforAdditionalDays").ToString() ?? string.Empty);
                }
            }

            return CostShareDetailsList;

        }

        private List<CostShareDetails> BuildCostShareDetailsRepeater()
        {
            List<CostShareDetails> CostShareDetailsObj = new List<CostShareDetails>();
            CostShareDetailsObj.Add(new CostShareDetails() {
                ServiceCode = "1a",
                Question = "Do you charge the Medicare defined cost shares",
                NextYearValue = string.Empty,
                ThisYearValue = string.Empty
            });


            CostShareDetailsObj.Add(new CostShareDetails()
            {
                ServiceCode = "1a",
                Question = "Is there an enrollee Copayment",
                NextYearValue = string.Empty,
                ThisYearValue = string.Empty
            });

            CostShareDetailsObj.Add(new CostShareDetails()
            {
                ServiceCode = "1a",
                Question = "Is there an enrollee Coinsurance",
                NextYearValue = string.Empty,
                ThisYearValue = string.Empty
            });
            CostShareDetailsObj.Add(new CostShareDetails()
            {
                ServiceCode = "1a",
                Question = "Enhanced Benefits",
                NextYearValue = string.Empty,
                ThisYearValue = string.Empty
            });
            CostShareDetailsObj.Add(new CostShareDetails()
            {
                ServiceCode = "1a",
                Question = "Is this benefit unlimited for Additional Days",
                NextYearValue = string.Empty,
                ThisYearValue = string.Empty
            });
            return CostShareDetailsObj;
        }

        private string ConvertYesNoValue(string value)
        {

            if (!string.IsNullOrEmpty(value))
            {
                return value.Equals("1") ? "Yes" : "No";
            }
            else
            {
                return value;

            }
        }

        private string ConvertServiceCode(string service)
        {
            string serviceText = string.Empty;

            Dictionary<string, string> ServiceCodePair = new Dictionary<string, string>();
            ServiceCodePair.Add("010", "Additional Days");
            ServiceCodePair.Add("001", "Non-Medicare-covered Stay");
            ServiceCodePair.Add("100", "Upgrades");
            try
            {
                if (!string.IsNullOrEmpty(service))
                {
                    var itemList = (JArray)JsonConvert.DeserializeObject(service);

                    foreach (var item in itemList)
                    {
                        string serviceName = ServiceCodePair.Where(s => s.Key.Equals(item.ToString())).Select(s => s.Value).FirstOrDefault();
                        if (!string.IsNullOrEmpty(serviceName))
                        {
                            serviceText += serviceName + ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(serviceText))
                    {
                        serviceText = serviceText.TrimEnd(',');
                    }

                }
            }
            catch (Exception ex)
            {
                return service;
            }
            return serviceText;
        }
    }
}

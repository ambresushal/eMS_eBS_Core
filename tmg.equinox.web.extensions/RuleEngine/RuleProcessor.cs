using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.infrastructure.util;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.FormInstanceProcessor.SourceTargetDataManager.SourceHandler;
using tmg.equinox.web.Framework.Caching;

namespace tmg.equinox.web.RuleEngine
{
    public abstract class RuleProcessor
    {
        protected JObject _formData;
        protected string _containerName;
        protected FormInstanceDataManager _formDataInstanceManager;
        protected int _formInstanceId;
        protected FormDesignVersionDetail _detail;
        protected string _sectionName;
        protected int _folderVersionId;
        protected IFormInstanceService _formInstanceService;
        protected IFormDesignService _formDesignServices;
        protected IFolderVersionServices _folderVersionServices;

        protected enum OperatorTypes
        {
            Equals = 1,
            GreaterThan = 2,
            LessThan = 3,
            Contains = 4,
            NotEquals = 5,
            GreaterThanOrEqualTo = 6,
            LessThanOrEqualTo = 7,
            IsNull = 8,
            NotContains = 10,
        }

        public enum LogicalOperatorTypes
        {
            AND = 1,
            OR = 2
        }

        public enum ExpressionTypes
        {
            NODE = 1,
            LEAF = 2
        }

        public enum TargetPropertyTypes
        {
            Enabled = 1,
            RunValidation = 2,
            Value = 3,
            Visible = 4,
            IsRequired = 5,
            Error = 6,
            CustomRule = 10,
        }

        //Process a rule object
        public abstract bool ProcessRule(RuleDesign rule);

        public bool ProcessNode(RuleDesign rule, ExpressionDesign rootExpression, JObject _rowData)
        {
            var exp = rootExpression.Expressions.Where(e => e.ExpressionTypeId != (int)ExpressionTypes.NODE).FirstOrDefault();
            string leftOperand = GetOperandValue(exp.LeftOperandName, exp.LeftOperand, _rowData);
            string rightOperand = exp.RightOperand;

            if (exp.IsRightOperandElement) { rightOperand = GetOperandValue(exp.RightOperandName, exp.RightOperand, _rowData); }

            if (rule.SuccessValue == "CheckInterval")
                return CheckInterval(leftOperand, rightOperand);
            if (rule.SuccessValue == "CheckCoinsurance")
                return CheckCoinsurance(leftOperand, rightOperand, _rowData, rule);
            if (rule.SuccessValue == "AdditionalDaysInterval")
                return AdditionalDaysInterval(leftOperand, rightOperand, _rowData, rule);
            if (rule.SuccessValue == "AdditionalDaysMaxTierEndDay")
                return AdditionalDaysMaxTierEndDay(leftOperand, rightOperand, _rowData, rule);
            if (rule.SuccessValue == "MOOPAmountValidationAcuteInterval1")
                return MOOPAmountValidationAcuteInterval1(leftOperand, rightOperand, _rowData);
            if (rule.SuccessValue == "MOOPAmountValidationAcuteInterval2")
                return MOOPAmountValidationAcuteInterval2(leftOperand, rightOperand, _rowData);
            if (rule.SuccessValue == "MOOPAmountValidationAcuteInterval3")
                return MOOPAmountValidationAcuteInterval3(leftOperand, rightOperand, _rowData);
            if (rule.SuccessValue == "MOOPAmountValidationPsychiatricInterval1")
                return MOOPAmountValidationPsychiatricInterval1(leftOperand, rightOperand, _rowData);
            if (rule.SuccessValue == "MOOPAmountValidationPsychiatricInterval2")
                return MOOPAmountValidationPsychiatricInterval2(leftOperand, rightOperand, _rowData);
            if (rule.SuccessValue == "MOOPAmountValidationPsychiatricInterval3")
                return MOOPAmountValidationPsychiatricInterval3(leftOperand, rightOperand, _rowData);
            if (rule.SuccessValue == "MOOPAmountValidationSNF")
                return MOOPAmountValidationSNF(leftOperand, rightOperand, _rowData, rule);

            if (rule.SuccessValue == "MOOPAmountValidationAcuteInterval1_2019")
                return MOOPAmountValidationAcuteInterval1_2019(leftOperand, rightOperand, _rowData);
            if (rule.SuccessValue == "MOOPAmountValidationAcuteInterval2_2019")
                return MOOPAmountValidationAcuteInterval2_2019(leftOperand, rightOperand, _rowData);
            if (rule.SuccessValue == "MOOPAmountValidationAcuteInterval3_2019")
                return MOOPAmountValidationAcuteInterval3_2019(leftOperand, rightOperand, _rowData);
            if (rule.SuccessValue == "MOOPAmountValidationPsychiatricInterval1_2019")
                return MOOPAmountValidationPsychiatricInterval1_2019(leftOperand, rightOperand, _rowData);
            if (rule.SuccessValue == "MOOPAmountValidationPsychiatricInterval2_2019")
                return MOOPAmountValidationPsychiatricInterval2_2019(leftOperand, rightOperand, _rowData);
            if (rule.SuccessValue == "MOOPAmountValidationPsychiatricInterval3_2019")
                return MOOPAmountValidationPsychiatricInterval3_2019(leftOperand, rightOperand, _rowData);
            if (rule.SuccessValue == "MOOPAmountValidationSNF_2019")
                return MOOPAmountValidationSNF_2019(leftOperand, rightOperand, _rowData);
            if (rule.SuccessValue == "MOOPAmountValidationSNF2019")
                return MOOPAmountValidationSNF2019(leftOperand, rightOperand, _rowData);
            if (rule.SuccessValue == "checkServiceIsSelected")
                return checkServiceIsSelected(leftOperand, rightOperand);
            if (rule.SuccessValue == "checkIfMandetoryForMedicare")
                return checkIfMandetoryForMedicare(leftOperand, rightOperand, _rowData, rule);
            if (rule.SuccessValue == "checkIfMandetoryForMedicare2019")
                return checkIfMandetoryForMedicare(leftOperand, rightOperand, _rowData, rule);
            if (rule.SuccessValue == "checkIfMandetoryForNonMedicare")
                return checkIfMandetoryForNonMedicare(leftOperand, rightOperand, _rowData, rule);
            if (rule.SuccessValue == "checkIfMandetoryForNonMedicare2019")
                return checkIfMandetoryForNonMedicare(leftOperand, rightOperand, _rowData, rule);
            if (rule.SuccessValue == "slidingCostShare")
                return slidingCostShareDeductibleCheck(leftOperand, rightOperand, _rowData);
            if (rule.SuccessValue == "checkIfOptionalBenefit")
                return checkIfOptionalBenefit(leftOperand, rightOperand, _rowData);
            return true;
        }

        private bool AdditionalDaysInterval(string leftOperand, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            int unlimitedAdditionalDays, endDay = 0, intervalNo, beginDay = 0, isThisBenefitUnlimited = 0, tierNo = 0;
            string serviceName = rightOperand.Split(',')[0], interval = string.Empty;
            bool doCheck = false;
            try
            {
                unlimitedAdditionalDays = String.IsNullOrEmpty(leftOperand) ? 0 : Convert.ToInt32(leftOperand);
                tierNo = Convert.ToInt32(rightOperand.Split(',')[2]);

                if (serviceName == "Acute")
                {
                    isThisBenefitUnlimited = Convert.ToInt32(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase1.IsthisbenefitunlimitedforAdditionalDays") ?? String.Empty);
                }
                else if (serviceName == "Psychiatric")
                {
                    isThisBenefitUnlimited = Convert.ToInt32(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase1.IsthisbenefitunlimitedforAdditionalDays") ?? String.Empty);
                }
                else if (serviceName == "SNF")
                {
                    isThisBenefitUnlimited = Convert.ToInt32(_rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase1.IsthisbenefitunlimitedforAdditionalDays") ?? String.Empty);
                }

                if (isThisBenefitUnlimited == 2)
                {
                    switch (rightOperand)
                    {
                        case "Acute,Coins,1":
                            {
                                beginDay = 90;

                                JToken acuteCoinsTier1Section = _rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase5.AdditionalDaysCoinsuranceCostSharingforTier1");
                                intervalNo = Convert.ToInt32(acuteCoinsTier1Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDays") ?? String.Empty);
                                string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                if (currentInterval.Contains(Convert.ToString(intervalNo - 1)))
                                    doCheck = true;
                                if (doCheck)
                                {
                                    if (intervalNo == 2)
                                        endDay = Convert.ToInt32(acuteCoinsTier1Section.SelectToken("EndDayInterval1") ?? String.Empty);
                                    else if (intervalNo == 3)
                                        endDay = Convert.ToInt32(acuteCoinsTier1Section.SelectToken("EndDayInterval2") ?? String.Empty);
                                    else if (intervalNo == 4)
                                        endDay = Convert.ToInt32(acuteCoinsTier1Section.SelectToken("EndDayInterval3") ?? String.Empty);
                                }
                                break;
                            }
                        case "Acute,Coins,2":
                            {
                                beginDay = 90;
                                JToken acuteCoinsTier2Section = _rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase5.AdditionalDaysCoinsuranceCostSharingforTier2");
                                intervalNo = Convert.ToInt32(acuteCoinsTier2Section.SelectToken("AdditionalDaysCoinsuranceCostSharingforTier2Indicatethenumberofdayinte") ?? String.Empty);
                                string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                if (currentInterval.Contains(Convert.ToString(intervalNo - 1)))
                                    doCheck = true;
                                if (doCheck)
                                {
                                    if (intervalNo == 2)
                                        endDay = Convert.ToInt32(acuteCoinsTier2Section.SelectToken("EndDayInterval1forTier2") ?? String.Empty);
                                    else if (intervalNo == 3)
                                        endDay = Convert.ToInt32(acuteCoinsTier2Section.SelectToken("EndDayInterval2forTier2") ?? String.Empty);
                                    else if (intervalNo == 4)
                                        endDay = Convert.ToInt32(acuteCoinsTier2Section.SelectToken("EndDayInterval3forTier2") ?? String.Empty);
                                }
                                break;
                            }
                        case "Acute,Coins,3":
                            {
                                beginDay = 90;
                                JToken acuteCoinsTier3Section = _rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase6.AdditionalDaysCoinsuranceCostSharingforTier3");
                                intervalNo = Convert.ToInt32(acuteCoinsTier3Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier3") ?? String.Empty);
                                string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                if (currentInterval.Contains(Convert.ToString(intervalNo - 1)))
                                    doCheck = true;
                                if (doCheck)
                                {
                                    if (intervalNo == 2)
                                        endDay = Convert.ToInt32(acuteCoinsTier3Section.SelectToken("EndDayInterval1forTier3") ?? String.Empty);
                                    else if (intervalNo == 3)
                                        endDay = Convert.ToInt32(acuteCoinsTier3Section.SelectToken("EndDayInterval2forTier3") ?? String.Empty);
                                    else if (intervalNo == 4)
                                        endDay = Convert.ToInt32(acuteCoinsTier3Section.SelectToken("EndDayInterval3forTier3") ?? String.Empty);
                                }
                                break;
                            }
                        case "Acute,Copay,1":
                            {
                                beginDay = 90;
                                JToken acuteCopayTier1Section = _rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase10.AdditionalDaysCopaymentCostSharingforTier1");
                                intervalNo = Convert.ToInt32(acuteCopayTier1Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDays") ?? String.Empty);
                                string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                if (currentInterval.Contains(Convert.ToString(intervalNo - 1)))
                                    doCheck = true;
                                if (doCheck)
                                {
                                    if (intervalNo == 2)
                                        endDay = Convert.ToInt32(acuteCopayTier1Section.SelectToken("EndDayInterval1") ?? String.Empty);
                                    else if (intervalNo == 3)
                                        endDay = Convert.ToInt32(acuteCopayTier1Section.SelectToken("EndDayInterval2") ?? String.Empty);
                                    else if (intervalNo == 4)
                                        endDay = Convert.ToInt32(acuteCopayTier1Section.SelectToken("EndDayInterval3") ?? String.Empty);
                                }
                                break;
                            }
                        case "Acute,Copay,2":
                            {
                                beginDay = 90;
                                JToken acuteCopayTier2Section = _rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase10.AdditionalDaysCopaymentCostSharingforTier2");
                                intervalNo = Convert.ToInt32(acuteCopayTier2Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier2") ?? String.Empty);
                                string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                if (currentInterval.Contains(Convert.ToString(intervalNo - 1)))
                                    doCheck = true;
                                if (doCheck)
                                {
                                    if (intervalNo == 2)
                                        endDay = Convert.ToInt32(acuteCopayTier2Section.SelectToken("EndDayInterval1forTier2") ?? String.Empty);
                                    else if (intervalNo == 3)
                                        endDay = Convert.ToInt32(acuteCopayTier2Section.SelectToken("EndDayInterval2forTier2") ?? String.Empty);
                                    else if (intervalNo == 4)
                                        endDay = Convert.ToInt32(acuteCopayTier2Section.SelectToken("EndDayInterval3forTier2") ?? String.Empty);
                                }
                                break;
                            }
                        case "Acute,Copay,3":
                            {
                                beginDay = 90;
                                JToken acuteCopayTier3Section = _rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase11.AdditionalDaysCopaymentCostSharingforTier3");
                                intervalNo = Convert.ToInt32(acuteCopayTier3Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier3") ?? String.Empty);
                                string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                if (currentInterval.Contains(Convert.ToString(intervalNo - 1)))
                                    doCheck = true;
                                if (doCheck)
                                {
                                    if (intervalNo == 2)
                                        endDay = Convert.ToInt32(acuteCopayTier3Section.SelectToken("EndDayInterval1forTier3") ?? String.Empty);
                                    else if (intervalNo == 3)
                                        endDay = Convert.ToInt32(acuteCopayTier3Section.SelectToken("EndDayInterval2forTier3") ?? String.Empty);
                                    else if (intervalNo == 4)
                                        endDay = Convert.ToInt32(acuteCopayTier3Section.SelectToken("EndDayInterval3forTier3") ?? String.Empty);
                                }
                                break;
                            }

                        case "Psychiatric,Coins,1":
                            {
                                beginDay = 90;
                                //wellcare   JToken psychiatricCoinsTier1Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase5.AdditionalDaysCoinsuranceCostSharingforTier1");
                                JToken psychiatricCoinsTier1Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase5.IP5AdditionalDaysCoinsuranceCostSharingforTier1");
                                intervalNo = Convert.ToInt32(psychiatricCoinsTier1Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDays") ?? String.Empty);
                                string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                if (currentInterval.Contains(Convert.ToString(intervalNo - 1)))
                                    doCheck = true;
                                if (doCheck)
                                {
                                    if (intervalNo == 2)
                                        endDay = Convert.ToInt32(psychiatricCoinsTier1Section.SelectToken("EndDayInterval1") ?? String.Empty);
                                    else if (intervalNo == 3)
                                        endDay = Convert.ToInt32(psychiatricCoinsTier1Section.SelectToken("EndDayInterval2") ?? String.Empty);
                                    else if (intervalNo == 4)
                                        endDay = Convert.ToInt32(psychiatricCoinsTier1Section.SelectToken("EndDayInterval3") ?? String.Empty);
                                }
                                break;
                            }
                        case "Psychiatric,Coins,2":
                            {
                                beginDay = 90;
                                //wellcare  JToken psychiatricCoinsTier2Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase5.AdditionalDaysCoinsuranceCostSharingforTier2");
                                JToken psychiatricCoinsTier2Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase5.IP5AdditionalDaysCoinsuranceCostSharingforTier2");
                                intervalNo = Convert.ToInt32(psychiatricCoinsTier2Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier2") ?? String.Empty);
                                string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                if (currentInterval.Contains(Convert.ToString(intervalNo - 1)))
                                    doCheck = true;
                                if (doCheck)
                                {
                                    if (intervalNo == 2)
                                        endDay = Convert.ToInt32(psychiatricCoinsTier2Section.SelectToken("EndDayInterval1forTier2") ?? String.Empty);
                                    else if (intervalNo == 3)
                                        endDay = Convert.ToInt32(psychiatricCoinsTier2Section.SelectToken("EndDayInterval2forTier2") ?? String.Empty);
                                    else if (intervalNo == 4)
                                        endDay = Convert.ToInt32(psychiatricCoinsTier2Section.SelectToken("EndDayInterval3forTier2") ?? String.Empty);
                                }
                                break;
                            }
                        case "Psychiatric,Coins,3":
                            {
                                beginDay = 90;
                                //wellcare JToken psychiatricCoinsTier3Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase6.AdditionalDaysCoinsuranceCostSharingforTier3");
                                JToken psychiatricCoinsTier3Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase6.IP6AdditionalDaysCoinsuranceCostSharingforTier3");
                                intervalNo = Convert.ToInt32(psychiatricCoinsTier3Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier3") ?? String.Empty);
                                string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                if (currentInterval.Contains(Convert.ToString(intervalNo - 1)))
                                    doCheck = true;
                                if (doCheck)
                                {
                                    if (intervalNo == 2)
                                        endDay = Convert.ToInt32(psychiatricCoinsTier3Section.SelectToken("EndDayInterval1forTier3") ?? String.Empty);
                                    else if (intervalNo == 3)
                                        endDay = Convert.ToInt32(psychiatricCoinsTier3Section.SelectToken("EndDayInterval2forTier3") ?? String.Empty);
                                    else if (intervalNo == 4)
                                        endDay = Convert.ToInt32(psychiatricCoinsTier3Section.SelectToken("EndDayInterval3forTier3") ?? String.Empty);
                                }
                                break;
                            }
                        case "Psychiatric,Copay,1":
                            {
                                beginDay = 90;
                                //wellcare                                 JToken psychiatricCopayTier1Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase10.AdditionalDaysCopaymentCostSharingforTier1");
                                JToken psychiatricCopayTier1Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase10.IP10AdditionalDaysCopaymentCostSharingforTier1");
                                intervalNo = Convert.ToInt32(psychiatricCopayTier1Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDays") ?? String.Empty);
                                string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                if (currentInterval.Contains(Convert.ToString(intervalNo - 1)))
                                    doCheck = true;
                                if (doCheck)
                                {
                                    if (intervalNo == 2)
                                        endDay = Convert.ToInt32(psychiatricCopayTier1Section.SelectToken("EndDayInterval1") ?? String.Empty);
                                    else if (intervalNo == 3)
                                        endDay = Convert.ToInt32(psychiatricCopayTier1Section.SelectToken("EndDayInterval2") ?? String.Empty);
                                    else if (intervalNo == 4)
                                        endDay = Convert.ToInt32(psychiatricCopayTier1Section.SelectToken("EndDayInterval3") ?? String.Empty);
                                }
                                break;
                            }
                        case "Psychiatric,Copay,2":
                            {
                                beginDay = 90;
                                //wellcare    JToken psychiatricCopayTier2Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase10.AdditionalDaysCopaymentCostSharingforTier2");
                                JToken psychiatricCopayTier2Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase10.IP10AdditionalDaysCopaymentCostSharingforTier2");
                                intervalNo = Convert.ToInt32(psychiatricCopayTier2Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier2") ?? String.Empty);
                                string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                if (currentInterval.Contains(Convert.ToString(intervalNo - 1)))
                                    doCheck = true;
                                if (doCheck)
                                {
                                    if (intervalNo == 2)
                                        endDay = Convert.ToInt32(psychiatricCopayTier2Section.SelectToken("EndDayInterval1forTier2") ?? String.Empty);
                                    else if (intervalNo == 3)
                                        endDay = Convert.ToInt32(psychiatricCopayTier2Section.SelectToken("EndDayInterval2forTier2") ?? String.Empty);
                                    else if (intervalNo == 4)
                                        endDay = Convert.ToInt32(psychiatricCopayTier2Section.SelectToken("EndDayInterval3forTier2") ?? String.Empty);
                                }
                                break;
                            }
                        case "Psychiatric,Copay,3":
                            {
                                beginDay = 90;
                                //wellcare    JToken psychiatricCopayTier3Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase11.AdditionalDaysCopaymentCostSharingforTier3");
                                JToken psychiatricCopayTier3Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase11.IP11AdditionalDaysCopaymentCostSharingforTier3");
                                intervalNo = Convert.ToInt32(psychiatricCopayTier3Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier3") ?? String.Empty);
                                string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                if (currentInterval.Contains(Convert.ToString(intervalNo - 1)))
                                    doCheck = true;
                                if (doCheck)
                                {
                                    if (intervalNo == 2)
                                        endDay = Convert.ToInt32(psychiatricCopayTier3Section.SelectToken("EndDayInterval1forTier3") ?? String.Empty);
                                    else if (intervalNo == 3)
                                        endDay = Convert.ToInt32(psychiatricCopayTier3Section.SelectToken("EndDayInterval2forTier3") ?? String.Empty);
                                    else if (intervalNo == 4)
                                        endDay = Convert.ToInt32(psychiatricCopayTier3Section.SelectToken("EndDayInterval3forTier3") ?? String.Empty);
                                }
                                break;
                            }

                        case "SNF,Coins,1":
                            {
                                beginDay = 100;
                                //wellcare    JToken snfCoinsTier1Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase4.AdditionalDaysCoinsuranceCostSharingforTier1");
                                JToken snfCoinsTier1Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase4.SNF4AdditionalDaysCoinsuranceCostSharingforTier1");
                                intervalNo = Convert.ToInt32(snfCoinsTier1Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDays") ?? String.Empty);
                                string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                if (currentInterval.Contains(Convert.ToString(intervalNo - 1)))
                                    doCheck = true;
                                if (doCheck)
                                {
                                    if (intervalNo == 2)
                                        endDay = Convert.ToInt32(snfCoinsTier1Section.SelectToken("EndDayInterval1") ?? String.Empty);
                                    else if (intervalNo == 3)
                                        endDay = Convert.ToInt32(snfCoinsTier1Section.SelectToken("EndDayInterval2") ?? String.Empty);
                                    else if (intervalNo == 4)
                                        endDay = Convert.ToInt32(snfCoinsTier1Section.SelectToken("EndDayInterval3") ?? String.Empty);
                                }
                                break;
                            }
                        case "SNF,Coins,2":
                            {
                                beginDay = 100;
                                JToken snfCoinsTier2Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase4.SNF4AdditionalDaysCoinsuranceCostSharingforTier2");
                                intervalNo = Convert.ToInt32(snfCoinsTier2Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier2") ?? String.Empty);
                                string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                if (currentInterval.Contains(Convert.ToString(intervalNo - 1)))
                                    doCheck = true;
                                if (doCheck)
                                {
                                    if (intervalNo == 2)
                                        endDay = Convert.ToInt32(snfCoinsTier2Section.SelectToken("EndDayInterval1forTier2") ?? String.Empty);
                                    else if (intervalNo == 3)
                                        endDay = Convert.ToInt32(snfCoinsTier2Section.SelectToken("EndDayInterval2forTier2") ?? String.Empty);
                                    else if (intervalNo == 4)
                                        endDay = Convert.ToInt32(snfCoinsTier2Section.SelectToken("EndDayInterval3forTier2") ?? String.Empty);
                                }
                                break;
                            }
                        case "SNF,Coins,3":
                            {
                                beginDay = 100;
                                //wellcare    JToken snfCoinsTier3Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase5.AdditionalDaysCoinsuranceCostSharingforTier3");
                                JToken snfCoinsTier3Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase5.SNF5AdditionalDaysCoinsuranceCostSharingforTier3");
                                intervalNo = Convert.ToInt32(snfCoinsTier3Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier3") ?? String.Empty);
                                string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                if (currentInterval.Contains(Convert.ToString(intervalNo - 1)))
                                    doCheck = true;
                                if (doCheck)
                                {
                                    if (intervalNo == 2)
                                        endDay = Convert.ToInt32(snfCoinsTier3Section.SelectToken("EndDayInterval1forTier3") ?? String.Empty);
                                    else if (intervalNo == 3)
                                        endDay = Convert.ToInt32(snfCoinsTier3Section.SelectToken("EndDayInterval2forTier3") ?? String.Empty);
                                    else if (intervalNo == 4)
                                        endDay = Convert.ToInt32(snfCoinsTier3Section.SelectToken("EndDayInterval3forTier3") ?? String.Empty);
                                }
                                break;
                            }
                        case "SNF,Copay,1":
                            {
                                beginDay = 100;
                                JToken snfCopayTier1Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase8.SNF8AdditionalDaysCopaymentCostSharingforTier1");
                                intervalNo = Convert.ToInt32(snfCopayTier1Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDays") ?? String.Empty);
                                string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                if (currentInterval.Contains(Convert.ToString(intervalNo - 1)))
                                    doCheck = true;
                                if (doCheck)
                                {
                                    if (intervalNo == 2)
                                        endDay = Convert.ToInt32(snfCopayTier1Section.SelectToken("EndDayInterval1") ?? String.Empty);
                                    else if (intervalNo == 3)
                                        endDay = Convert.ToInt32(snfCopayTier1Section.SelectToken("EndDayInterval2") ?? String.Empty);
                                    else if (intervalNo == 4)
                                        endDay = Convert.ToInt32(snfCopayTier1Section.SelectToken("EndDayInterval3") ?? String.Empty);
                                }
                                break;
                            }
                        case "SNF,Copay,2":
                            {
                                beginDay = 100;
                                JToken snfCopayTier2Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase8.SNF8AdditionalDaysCopaymentCostSharingforTier2");
                                intervalNo = Convert.ToInt32(snfCopayTier2Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier2") ?? String.Empty);
                                string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                if (currentInterval.Contains(Convert.ToString(intervalNo - 1)))
                                    doCheck = true;
                                if (doCheck)
                                {
                                    if (intervalNo == 2)
                                        endDay = Convert.ToInt32(snfCopayTier2Section.SelectToken("EndDayInterval1forTier2") ?? String.Empty);
                                    else if (intervalNo == 3)
                                        endDay = Convert.ToInt32(snfCopayTier2Section.SelectToken("EndDayInterval2forTier2") ?? String.Empty);
                                    else if (intervalNo == 4)
                                        endDay = Convert.ToInt32(snfCopayTier2Section.SelectToken("EndDayInterval3forTier2") ?? String.Empty);
                                }
                                break;
                            }
                        case "SNF,Copay,3":
                            {
                                beginDay = 100;
                                //wellcare     JToken snfCopayTier3Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase9.AdditionalDaysCopaymentCostSharingforTier3");
                                JToken snfCopayTier3Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase9.SNF9AdditionalDaysCopaymentCostSharingforTier3");
                                intervalNo = Convert.ToInt32(snfCopayTier3Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier3") ?? String.Empty);
                                string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                if (currentInterval.Contains(Convert.ToString(intervalNo - 1)))
                                    doCheck = true;
                                if (doCheck)
                                {
                                    if (intervalNo == 2)
                                        endDay = Convert.ToInt32(snfCopayTier3Section.SelectToken("EndDayInterval1forTier3") ?? String.Empty);
                                    else if (intervalNo == 3)
                                        endDay = Convert.ToInt32(snfCopayTier3Section.SelectToken("EndDayInterval2forTier3") ?? String.Empty);
                                    else if (intervalNo == 4)
                                        endDay = Convert.ToInt32(snfCopayTier3Section.SelectToken("EndDayInterval3forTier3") ?? String.Empty);
                                }
                                break;
                            }
                    }
                    if (doCheck)
                    {
                        return endDay == unlimitedAdditionalDays + beginDay;
                    }
                }
                else
                {
                    return AdditionalDaysMaxTierEndDay(leftOperand, rightOperand, _rowData, rule);
                }
            }
            catch (Exception ex)
            {
                var e = ex;
            }
            return true;
        }

        private bool AdditionalDaysMaxTierEndDay(string leftOperand, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            int endDayTier1 = 0, endDayTier2 = 0, endDayTier3 = 0, tierNo = 0, isThisBenefitUnlimited = 0, intervalNoTier1 = 0, intervalNoTier2 = 0, intervalNoTier3 = 0;
            string serviceName = rightOperand.Split(',')[0], interval = string.Empty, switchCondition = string.Empty;
            bool doCheck = false;

            try
            {
                if (!String.IsNullOrEmpty(rightOperand))
                {
                    tierNo = Convert.ToInt32(rightOperand.Split(',')[2]);
                    switchCondition = rightOperand.Split(',')[0] + "," + rightOperand.Split(',')[1];

                    if (serviceName == "Acute")
                    {
                        isThisBenefitUnlimited = Convert.ToInt32(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase1.IsthisbenefitunlimitedforAdditionalDays") ?? String.Empty);
                    }
                    else if (serviceName == "Psychiatric")
                    {
                        isThisBenefitUnlimited = Convert.ToInt32(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase1.IsthisbenefitunlimitedforAdditionalDays") ?? String.Empty);
                    }
                    else if (serviceName == "SNF")
                    {
                        isThisBenefitUnlimited = Convert.ToInt32(_rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase1.IsthisbenefitunlimitedforAdditionalDays") ?? String.Empty);
                    }

                    if (isThisBenefitUnlimited == 1)
                    {
                        switch (switchCondition)
                        {
                            case "Acute,Coins":
                                {
                                    JToken acuteCoinsTier1Section = _rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase5.AdditionalDaysCoinsuranceCostSharingforTier1");
                                    JToken acuteCoinsTier2Section = _rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase5.AdditionalDaysCoinsuranceCostSharingforTier2");
                                    JToken acuteCoinsTier3Section = _rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase6.AdditionalDaysCoinsuranceCostSharingforTier3");
                                    string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                    intervalNoTier1 = Convert.ToString(acuteCoinsTier1Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDays") ?? "") != String.Empty ?
                                        Convert.ToInt32(acuteCoinsTier1Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDays")) : 0;
                                    intervalNoTier2 = Convert.ToString(acuteCoinsTier2Section.SelectToken("AdditionalDaysCoinsuranceCostSharingforTier2Indicatethenumberofdayinte") ?? "") != String.Empty ?
                                        Convert.ToInt32(acuteCoinsTier2Section.SelectToken("AdditionalDaysCoinsuranceCostSharingforTier2Indicatethenumberofdayinte")) : 0;
                                    intervalNoTier3 = Convert.ToString(acuteCoinsTier3Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier3") ?? "") != String.Empty ?
                                      Convert.ToInt32(acuteCoinsTier3Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier3")) : 0;

                                    if (tierNo == 1)
                                        interval = Convert.ToString(intervalNoTier1 - 1);
                                    if (tierNo == 2)
                                        interval = Convert.ToString(intervalNoTier2 - 1);
                                    if (tierNo == 3)
                                        interval = Convert.ToString(intervalNoTier3 - 1);

                                    if (currentInterval.Contains(interval))
                                        doCheck = true;

                                    if (doCheck)
                                    {
                                        if (intervalNoTier1 == 2)
                                            endDayTier1 = Convert.ToInt32(acuteCoinsTier1Section.SelectToken("EndDayInterval1") ?? String.Empty);
                                        else if (intervalNoTier1 == 3)
                                            endDayTier1 = Convert.ToInt32(acuteCoinsTier1Section.SelectToken("EndDayInterval2") ?? String.Empty);
                                        else if (intervalNoTier1 == 4)
                                            endDayTier1 = Convert.ToInt32(acuteCoinsTier1Section.SelectToken("EndDayInterval3") ?? String.Empty);

                                        if (intervalNoTier2 == 2)
                                            endDayTier2 = Convert.ToInt32(acuteCoinsTier2Section.SelectToken("EndDayInterval1forTier2") ?? String.Empty);
                                        else if (intervalNoTier2 == 3)
                                            endDayTier2 = Convert.ToInt32(acuteCoinsTier2Section.SelectToken("EndDayInterval2forTier2") ?? String.Empty);
                                        else if (intervalNoTier2 == 4)
                                            endDayTier2 = Convert.ToInt32(acuteCoinsTier2Section.SelectToken("EndDayInterval3forTier2") ?? String.Empty);

                                        if (intervalNoTier3 == 2)
                                            endDayTier3 = Convert.ToInt32(acuteCoinsTier3Section.SelectToken("EndDayInterval1forTier3") ?? String.Empty);
                                        else if (intervalNoTier3 == 3)
                                            endDayTier3 = Convert.ToInt32(acuteCoinsTier3Section.SelectToken("EndDayInterval2forTier3") ?? String.Empty);
                                        else if (intervalNoTier3 == 4)
                                            endDayTier3 = Convert.ToInt32(acuteCoinsTier3Section.SelectToken("EndDayInterval3forTier3") ?? String.Empty);
                                    }
                                    break;
                                }
                            case "Acute,Copay":
                                {
                                    JToken acuteCopayTier1Section = _rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase10.AdditionalDaysCopaymentCostSharingforTier1");
                                    JToken acuteCopayTier2Section = _rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase10.AdditionalDaysCopaymentCostSharingforTier2");
                                    JToken acuteCopayTier3Section = _rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase11.AdditionalDaysCopaymentCostSharingforTier3");
                                    string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                    intervalNoTier1 = Convert.ToString(acuteCopayTier1Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDays") ?? "") != String.Empty ?
                                       Convert.ToInt32(acuteCopayTier1Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDays")) : 0;
                                    intervalNoTier2 = Convert.ToString(acuteCopayTier2Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier2") ?? "") != String.Empty ?
                                        Convert.ToInt32(acuteCopayTier2Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier2")) : 0;
                                    intervalNoTier3 = Convert.ToString(acuteCopayTier3Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier3") ?? "") != String.Empty ?
                                      Convert.ToInt32(acuteCopayTier3Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier3")) : 0;

                                    if (tierNo == 1)
                                        interval = Convert.ToString(intervalNoTier1 - 1);
                                    if (tierNo == 2)
                                        interval = Convert.ToString(intervalNoTier2 - 1);
                                    if (tierNo == 3)
                                        interval = Convert.ToString(intervalNoTier3 - 1);

                                    if (currentInterval.Contains(interval))
                                        doCheck = true;

                                    if (doCheck)
                                    {
                                        if (intervalNoTier1 == 2)
                                            endDayTier1 = Convert.ToInt32(acuteCopayTier1Section.SelectToken("EndDayInterval1") ?? String.Empty);
                                        else if (intervalNoTier1 == 3)
                                            endDayTier1 = Convert.ToInt32(acuteCopayTier1Section.SelectToken("EndDayInterval2") ?? String.Empty);
                                        else if (intervalNoTier1 == 4)
                                            endDayTier1 = Convert.ToInt32(acuteCopayTier1Section.SelectToken("EndDayInterval3") ?? String.Empty);

                                        if (intervalNoTier2 == 2)
                                            endDayTier2 = Convert.ToInt32(acuteCopayTier2Section.SelectToken("EndDayInterval1forTier2") ?? String.Empty);
                                        else if (intervalNoTier2 == 3)
                                            endDayTier2 = Convert.ToInt32(acuteCopayTier2Section.SelectToken("EndDayInterval2forTier2") ?? String.Empty);
                                        else if (intervalNoTier2 == 4)
                                            endDayTier2 = Convert.ToInt32(acuteCopayTier2Section.SelectToken("EndDayInterval3forTier2") ?? String.Empty);

                                        if (intervalNoTier3 == 2)
                                            endDayTier3 = Convert.ToInt32(_rowData.SelectToken("EndDayInterval1forTier3") ?? String.Empty);
                                        else if (intervalNoTier3 == 3)
                                            endDayTier3 = Convert.ToInt32(_rowData.SelectToken("EndDayInterval2forTier3") ?? String.Empty);
                                        else if (intervalNoTier3 == 4)
                                            endDayTier3 = Convert.ToInt32(_rowData.SelectToken("EndDayInterval3forTier3") ?? String.Empty);
                                    }
                                    break;
                                }
                            case "Psychiatric,Coins":
                                {
                                    /*
                                     JToken psychiatricCoinsTier1Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase5.AdditionalDaysCoinsuranceCostSharingforTier1");
                                        JToken psychiatricCoinsTier2Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase5.AdditionalDaysCoinsuranceCostSharingforTier2");
                                        JToken psychiatricCoinsTier3Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase6.AdditionalDaysCoinsuranceCostSharingforTier3");                                 
                                    */
                                    JToken psychiatricCoinsTier1Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase5.IP5AdditionalDaysCoinsuranceCostSharingforTier1");
                                    JToken psychiatricCoinsTier2Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase5.IP5AdditionalDaysCoinsuranceCostSharingforTier2");
                                    JToken psychiatricCoinsTier3Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase6.IP6AdditionalDaysCoinsuranceCostSharingforTier3");
                                    string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                    intervalNoTier1 = Convert.ToString(psychiatricCoinsTier1Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDays") ?? "") != String.Empty ?
                                      Convert.ToInt32(psychiatricCoinsTier1Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDays")) : 0;
                                    intervalNoTier2 = Convert.ToString(psychiatricCoinsTier2Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier2") ?? "") != String.Empty ?
                                        Convert.ToInt32(psychiatricCoinsTier2Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier2")) : 0;
                                    intervalNoTier3 = Convert.ToString(psychiatricCoinsTier3Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier3") ?? "") != String.Empty ?
                                      Convert.ToInt32(psychiatricCoinsTier3Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier3")) : 0;

                                    if (tierNo == 1)
                                        interval = Convert.ToString(intervalNoTier1 - 1);
                                    if (tierNo == 2)
                                        interval = Convert.ToString(intervalNoTier2 - 1);
                                    if (tierNo == 3)
                                        interval = Convert.ToString(intervalNoTier3 - 1);

                                    if (currentInterval.Contains(interval))
                                        doCheck = true;

                                    if (doCheck)
                                    {
                                        if (intervalNoTier1 == 2)
                                            endDayTier1 = Convert.ToInt32(psychiatricCoinsTier1Section.SelectToken("EndDayInterval1") ?? String.Empty);
                                        else if (intervalNoTier1 == 3)
                                            endDayTier1 = Convert.ToInt32(psychiatricCoinsTier1Section.SelectToken("EndDayInterval2") ?? String.Empty);
                                        else if (intervalNoTier1 == 4)
                                            endDayTier1 = Convert.ToInt32(psychiatricCoinsTier1Section.SelectToken("EndDayInterval3") ?? String.Empty);

                                        if (intervalNoTier2 == 2)
                                            endDayTier2 = Convert.ToInt32(psychiatricCoinsTier2Section.SelectToken("EndDayInterval1forTier2") ?? String.Empty);
                                        else if (intervalNoTier2 == 3)
                                            endDayTier2 = Convert.ToInt32(psychiatricCoinsTier2Section.SelectToken("EndDayInterval2forTier2") ?? String.Empty);
                                        else if (intervalNoTier2 == 4)
                                            endDayTier2 = Convert.ToInt32(psychiatricCoinsTier2Section.SelectToken("EndDayInterval3forTier2") ?? String.Empty);

                                        if (intervalNoTier3 == 2)
                                            endDayTier3 = Convert.ToInt32(psychiatricCoinsTier3Section.SelectToken("EndDayInterval1forTier3") ?? String.Empty);
                                        else if (intervalNoTier3 == 3)
                                            endDayTier3 = Convert.ToInt32(psychiatricCoinsTier3Section.SelectToken("EndDayInterval2forTier3") ?? String.Empty);
                                        else if (intervalNoTier3 == 4)
                                            endDayTier3 = Convert.ToInt32(psychiatricCoinsTier3Section.SelectToken("EndDayInterval3forTier3") ?? String.Empty);
                                    }
                                    break;
                                }
                            case "Psychiatric,Copay":
                                {
                                    /*
                                        JToken psychiatricCopayTier1Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase10.AdditionalDaysCopaymentCostSharingforTier1");
                                        JToken psychiatricCopayTier2Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase10.AdditionalDaysCopaymentCostSharingforTier2");
                                        JToken psychiatricCopayTier3Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase11.AdditionalDaysCopaymentCostSharingforTier3");                               
                                    */
                                    JToken psychiatricCopayTier1Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase10.IP10AdditionalDaysCopaymentCostSharingforTier1");
                                    JToken psychiatricCopayTier2Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase10.IP10AdditionalDaysCopaymentCostSharingforTier2");
                                    JToken psychiatricCopayTier3Section = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase11.IP11AdditionalDaysCopaymentCostSharingforTier3");
                                    string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                    intervalNoTier1 = Convert.ToString(psychiatricCopayTier1Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDays") ?? "") != String.Empty ?
                                     Convert.ToInt32(psychiatricCopayTier1Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDays")) : 0;
                                    intervalNoTier2 = Convert.ToString(psychiatricCopayTier2Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier2") ?? "") != String.Empty ?
                                        Convert.ToInt32(psychiatricCopayTier2Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier2")) : 0;
                                    intervalNoTier3 = Convert.ToString(psychiatricCopayTier3Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier3") ?? "") != String.Empty ?
                                      Convert.ToInt32(psychiatricCopayTier3Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier3")) : 0;

                                    if (tierNo == 1)
                                        interval = Convert.ToString(intervalNoTier1 - 1);
                                    if (tierNo == 2)
                                        interval = Convert.ToString(intervalNoTier2 - 1);
                                    if (tierNo == 3)
                                        interval = Convert.ToString(intervalNoTier3 - 1);

                                    if (currentInterval.Contains(interval))
                                        doCheck = true;

                                    if (doCheck)
                                    {
                                        if (intervalNoTier1 == 2)
                                            endDayTier1 = Convert.ToInt32(psychiatricCopayTier1Section.SelectToken("EndDayInterval1") ?? String.Empty);
                                        else if (intervalNoTier1 == 3)
                                            endDayTier1 = Convert.ToInt32(psychiatricCopayTier1Section.SelectToken("EndDayInterval2") ?? String.Empty);
                                        else if (intervalNoTier1 == 4)
                                            endDayTier1 = Convert.ToInt32(psychiatricCopayTier1Section.SelectToken("EndDayInterval3") ?? String.Empty);

                                        if (intervalNoTier2 == 2)
                                            endDayTier2 = Convert.ToInt32(psychiatricCopayTier2Section.SelectToken("EndDayInterval1forTier2") ?? String.Empty);
                                        else if (intervalNoTier2 == 3)
                                            endDayTier2 = Convert.ToInt32(psychiatricCopayTier2Section.SelectToken("EndDayInterval2forTier2") ?? String.Empty);
                                        else if (intervalNoTier2 == 4)
                                            endDayTier2 = Convert.ToInt32(psychiatricCopayTier2Section.SelectToken("EndDayInterval3forTier2") ?? String.Empty);

                                        if (intervalNoTier3 == 2)
                                            endDayTier3 = Convert.ToInt32(psychiatricCopayTier3Section.SelectToken("EndDayInterval1forTier3") ?? String.Empty);
                                        else if (intervalNoTier3 == 3)
                                            endDayTier3 = Convert.ToInt32(psychiatricCopayTier3Section.SelectToken("EndDayInterval2forTier3") ?? String.Empty);
                                        else if (intervalNoTier3 == 4)
                                            endDayTier3 = Convert.ToInt32(psychiatricCopayTier3Section.SelectToken("EndDayInterval3forTier3") ?? String.Empty);
                                    }
                                    break;
                                }
                            case "SNF,Coins":
                                {
                                    /*
                                      JToken snfCoinsTier1Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase4.AdditionalDaysCoinsuranceCostSharingforTier1");
                                        JToken snfCoinsTier2Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase4.AdditionalDaysCoinsuranceCostSharingforTier2");
                                        JToken snfCoinsTier3Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase5.AdditionalDaysCoinsuranceCostSharingforTier3");
                                    */
                                    JToken snfCoinsTier1Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase4.SNF4AdditionalDaysCoinsuranceCostSharingforTier1");
                                    JToken snfCoinsTier2Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase4.SNF4AdditionalDaysCoinsuranceCostSharingforTier2");
                                    JToken snfCoinsTier3Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase5.SNF5AdditionalDaysCoinsuranceCostSharingforTier3");
                                    string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                    intervalNoTier1 = Convert.ToString(snfCoinsTier1Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDays") ?? "") != String.Empty ?
                                     Convert.ToInt32(snfCoinsTier1Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDays")) : 0;
                                    intervalNoTier2 = Convert.ToString(snfCoinsTier2Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier2") ?? "") != String.Empty ?
                                        Convert.ToInt32(snfCoinsTier2Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier2")) : 0;
                                    intervalNoTier3 = Convert.ToString(snfCoinsTier3Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier3") ?? "") != String.Empty ?
                                      Convert.ToInt32(snfCoinsTier3Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier3")) : 0;


                                    if (tierNo == 1)
                                        interval = Convert.ToString(intervalNoTier1 - 1);
                                    if (tierNo == 2)
                                        interval = Convert.ToString(intervalNoTier2 - 1);
                                    if (tierNo == 3)
                                        interval = Convert.ToString(intervalNoTier3 - 1);

                                    if (currentInterval.Contains(interval))
                                        doCheck = true;

                                    if (doCheck)
                                    {

                                        if (intervalNoTier1 == 2)
                                            endDayTier1 = Convert.ToInt32(snfCoinsTier1Section.SelectToken("EndDayInterval1") ?? String.Empty);
                                        else if (intervalNoTier1 == 3)
                                            endDayTier1 = Convert.ToInt32(snfCoinsTier1Section.SelectToken("EndDayInterval2") ?? String.Empty);
                                        else if (intervalNoTier1 == 4)
                                            endDayTier1 = Convert.ToInt32(snfCoinsTier1Section.SelectToken("EndDayInterval3") ?? String.Empty);


                                        if (intervalNoTier2 == 2)
                                            endDayTier2 = Convert.ToInt32(snfCoinsTier2Section.SelectToken("EndDayInterval1forTier2") ?? String.Empty);
                                        else if (intervalNoTier2 == 3)
                                            endDayTier2 = Convert.ToInt32(snfCoinsTier2Section.SelectToken("EndDayInterval2forTier2") ?? String.Empty);
                                        else if (intervalNoTier2 == 4)
                                            endDayTier2 = Convert.ToInt32(snfCoinsTier2Section.SelectToken("EndDayInterval3forTier2") ?? String.Empty);

                                        if (intervalNoTier3 == 2)
                                            endDayTier3 = Convert.ToInt32(snfCoinsTier3Section.SelectToken("EndDayInterval1forTier3") ?? String.Empty);
                                        else if (intervalNoTier3 == 3)
                                            endDayTier3 = Convert.ToInt32(snfCoinsTier3Section.SelectToken("EndDayInterval2forTier3") ?? String.Empty);
                                        else if (intervalNoTier3 == 4)
                                            endDayTier3 = Convert.ToInt32(snfCoinsTier3Section.SelectToken("EndDayInterval3forTier3") ?? String.Empty);
                                    }
                                    break;
                                }
                            case "SNF,Copay":
                                {
                                    /*
                                        JToken snfCopayTier1Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase8.AdditionalDaysCopaymentCostSharingforTier1");
                                        JToken snfCopayTier2Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase8.AdditionalDaysCopaymentCostSharingforTier2");
                                        JToken snfCopayTier3Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase9.AdditionalDaysCopaymentCostSharingforTier3");

                                    */
                                    JToken snfCopayTier1Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase8.SNF8AdditionalDaysCopaymentCostSharingforTier1");
                                    JToken snfCopayTier2Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase8.SNF8AdditionalDaysCopaymentCostSharingforTier2");
                                    JToken snfCopayTier3Section = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase9.SNF9AdditionalDaysCopaymentCostSharingforTier3");
                                    string currentInterval = Convert.ToString(rule.UIElementName).Replace("Tier" + Convert.ToString(tierNo), "");

                                    intervalNoTier1 = Convert.ToString(snfCopayTier1Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDays") ?? "") != String.Empty ?
                                     Convert.ToInt32(snfCopayTier1Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDays")) : 0;
                                    intervalNoTier2 = Convert.ToString(snfCopayTier2Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier2") ?? "") != String.Empty ?
                                        Convert.ToInt32(snfCopayTier2Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier2")) : 0;
                                    intervalNoTier3 = Convert.ToString(snfCopayTier3Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier3") ?? "") != String.Empty ?
                                      Convert.ToInt32(snfCopayTier3Section.SelectToken("IndicatethenumberofdayintervalsforAdditionalDaysforTier3")) : 0;


                                    if (tierNo == 1)
                                        interval = Convert.ToString(intervalNoTier1 - 1);
                                    if (tierNo == 2)
                                        interval = Convert.ToString(intervalNoTier2 - 1);
                                    if (tierNo == 3)
                                        interval = Convert.ToString(intervalNoTier3 - 1);

                                    if (currentInterval.Contains(interval))
                                        doCheck = true;

                                    if (doCheck)
                                    {
                                        if (intervalNoTier1 == 2)
                                            endDayTier1 = Convert.ToInt32(snfCopayTier1Section.SelectToken("EndDayInterval1") ?? String.Empty);
                                        else if (intervalNoTier1 == 3)
                                            endDayTier1 = Convert.ToInt32(snfCopayTier1Section.SelectToken("EndDayInterval2") ?? String.Empty);
                                        else if (intervalNoTier1 == 4)
                                            endDayTier1 = Convert.ToInt32(snfCopayTier1Section.SelectToken("EndDayInterval3") ?? String.Empty);


                                        if (intervalNoTier2 == 2)
                                            endDayTier2 = Convert.ToInt32(snfCopayTier2Section.SelectToken("EndDayInterval1forTier2") ?? String.Empty);
                                        else if (intervalNoTier2 == 3)
                                            endDayTier2 = Convert.ToInt32(snfCopayTier2Section.SelectToken("EndDayInterval2forTier2") ?? String.Empty);
                                        else if (intervalNoTier2 == 4)
                                            endDayTier2 = Convert.ToInt32(snfCopayTier2Section.SelectToken("EndDayInterval3forTier2") ?? String.Empty);


                                        if (intervalNoTier3 == 2)
                                            endDayTier3 = Convert.ToInt32(snfCopayTier3Section.SelectToken("EndDayInterval1forTier3") ?? String.Empty);
                                        else if (intervalNoTier3 == 3)
                                            endDayTier3 = Convert.ToInt32(snfCopayTier3Section.SelectToken("EndDayInterval2forTier3") ?? String.Empty);
                                        else if (intervalNoTier3 == 4)
                                            endDayTier3 = Convert.ToInt32(snfCopayTier3Section.SelectToken("EndDayInterval3forTier3") ?? String.Empty);
                                    }
                                    break;
                                }
                        }
                        if (doCheck)
                        {
                            if (tierNo == 1)
                            {
                                if (intervalNoTier2 <= 1 && intervalNoTier3 <= 1)
                                    return true;
                                if (intervalNoTier3 <= 1)
                                {
                                    if (endDayTier1 != endDayTier2)
                                        return false;
                                }
                                if (intervalNoTier3 > 1 && intervalNoTier2 > 1)
                                {
                                    if (endDayTier1 != endDayTier2 || endDayTier1 != endDayTier3)
                                        return false;

                                }
                            }
                            if (tierNo == 2)
                            {
                                if (intervalNoTier2 <= 1 && intervalNoTier3 <= 1)
                                    return true;
                                if (intervalNoTier3 <= 1)
                                {
                                    if (endDayTier1 != endDayTier2)
                                        return false;
                                }
                                if (intervalNoTier3 > 1 && intervalNoTier2 > 1)
                                {
                                    if (endDayTier1 != endDayTier2 || endDayTier1 != endDayTier3)
                                        return false;

                                }
                            }
                            if (tierNo == 3)
                            {
                                if (intervalNoTier2 <= 1 && intervalNoTier3 <= 1)
                                    return true;
                                if (intervalNoTier3 <= 1)
                                {
                                    if (endDayTier1 != endDayTier2)
                                        return false;
                                }
                                if (intervalNoTier3 > 1 && intervalNoTier2 > 1)
                                {
                                    if (endDayTier1 != endDayTier2 || endDayTier1 != endDayTier3)
                                        return false;

                                }
                            }
                        }
                    }
                    else
                    {
                        return AdditionalDaysInterval(leftOperand, rightOperand, _rowData, rule);
                    }
                }
            }
            catch (Exception ex)
            {
                var e = ex;
            }
            return true;
        }

        private bool CheckInterval(string leftOperand, string rightOperand)
        {
            decimal n1, n2;
            try
            {
                if (!string.IsNullOrEmpty(leftOperand) && !string.IsNullOrEmpty(rightOperand))
                {
                    if (leftOperand.Contains('-') && rightOperand.Contains('-'))
                    {
                        string endDay = leftOperand.Split('-')[1], beginDay = rightOperand.Split('-')[0].Split(' ')[1];
                        if (decimal.TryParse(endDay, out n1) && decimal.TryParse(beginDay, out n2))
                            return n2 == n1 + 1;
                    }
                    else if (decimal.TryParse(leftOperand, out n1) && decimal.TryParse(rightOperand, out n2))
                        return n2 == n1 + 1;
                }
            }
            catch (Exception) { }
            return true;
        }

        private bool CheckCoinsurance(string leftOperand, string rightOperand, JToken _rowData, RuleDesign rule)
        {
            decimal n1, n2, n3;
            String targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementFullName) ?? String.Empty);
            try
            {
                if (!string.IsNullOrEmpty(leftOperand) && !string.IsNullOrEmpty(rightOperand))
                {
                    if (decimal.TryParse(leftOperand, out n1) && decimal.TryParse(rightOperand, out n2) && decimal.TryParse(targetValue, out n3))
                        return (n1 + n2 + n3) <= 100;
                }
            }
            catch (Exception) { }
            return true;
        }

        private bool MOOPAmountValidationAcuteInterval1(string leftOperand, string rightOperand, JObject _rowData)
        {
            decimal n1; int n2;
            string moopValue = String.Empty, endDay1 = String.Empty, tierValue = String.Empty;

            try
            {
                if (_rowData != null)
                {
                    //if (rightOperand.IndexOf(',') != -1)
                    //{
                    //    tierValue = rightOperand.Split(',')[0];
                    //    designYear = rightOperand.Split(',')[1];
                    //    if (String.IsNullOrEmpty(designYear) && designYear.Equals("2019"))
                    //    {
                    //        mandatory60 = 4314; mandatory10 = 2042; mandatory6 = 1860; voluntary10 = 2552; voluntary6 = 2325;
                    //    }
                    //}
                    //else
                    //    tierValue = rightOperand;
                    moopValue = Convert.ToString(_rowData.SelectToken("SectionDPlanLevel.MaxEnrolleCostLimitInNetwork.IsyourInNetworkMaximumEnrolleeOutofPocketMOOPCostattheVoluntaryorManda") ?? String.Empty);
                    tierValue = rightOperand;
                    if (tierValue.Equals("Tier1"))
                    {
                        endDay1 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval1") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier2"))
                    {
                        endDay1 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.EndDayInterval1forTier2") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier3"))
                    {
                        endDay1 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval1forTier3") ?? String.Empty);
                    }


                    if (decimal.TryParse(leftOperand, out n1) && int.TryParse(endDay1, out n2) && !string.IsNullOrEmpty(moopValue))
                    {
                        if (MOOPAmountValidationAcuteInterval3(leftOperand, tierValue, _rowData))
                        {
                            if (MOOPAmountValidationAcuteInterval2(leftOperand, tierValue, _rowData))
                            {
                                if (moopValue == "1")
                                {
                                    if (n2 <= 6)
                                    {
                                        if (n1 * n2 > 2271) return false;
                                    }
                                    else if (n2 > 6 && n2 <= 10)
                                    {
                                        if (n1 * n2 > 2495) return false;
                                    }
                                    else if (n1 * n2 > 2495) return false;
                                }
                                else
                                {
                                    if (n2 <= 6)
                                    {
                                        if (n1 * n2 > 1817) return false;
                                    }
                                    else if (n2 > 6 && n2 <= 10)
                                    {
                                        if (n1 * n2 > 1996) return false;
                                    }
                                    else if (n2 > 10 && n2 <= 60)
                                    {
                                        if (n1 * n2 > 4235) return false;
                                    }
                                    else if (n1 * n2 > 4235) return false;
                                }
                            }
                            else return false;
                        }
                        else return false;
                    }
                }
            }
            catch (Exception)
            { }
            return true;
        }

        private bool MOOPAmountValidationAcuteInterval2(string leftOperand, string rightOperand, JObject _rowData)
        {
            string moopValue = String.Empty, tierValue = String.Empty, copay1 = String.Empty, endDay1 = String.Empty, copay2 = String.Empty, beginDay2 = String.Empty, endDay2 = String.Empty;
            decimal c1, c2;
            int ed1, bd2, ed2;

            try
            {
                if (_rowData != null)
                {
                    //if (rightOperand.IndexOf(',') != -1)
                    //{
                    //    tierValue = rightOperand.Split(',')[0];
                    //    designYear = rightOperand.Split(',')[1];
                    //    if (String.IsNullOrEmpty(designYear) && designYear.Equals("2019"))
                    //    {
                    //        mandatory60 = 4314; mandatory10 = 2042; mandatory6 = 1860; voluntary10 = 2552; voluntary6 = 2325;
                    //    }
                    //}
                    //else
                    //    tierValue = rightOperand;
                    moopValue = Convert.ToString(_rowData.SelectToken("SectionDPlanLevel.MaxEnrolleCostLimitInNetwork.IsyourInNetworkMaximumEnrolleeOutofPocketMOOPCostattheVoluntaryorManda") ?? String.Empty);
                    tierValue = rightOperand;
                    if (tierValue.Equals("Tier1"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval1") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval1") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval2") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.BeginDayInterval2") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval2") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier2"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.CopaymentAmtInterval1forTier2") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.EndDayInterval1forTier2") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.CopaymentAmtInterval2forTier2") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.BeginDayInterval2forTier2") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.EndDayInterval2forTier2") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier3"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval1forTier3") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval1forTier3") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval2forTier3") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.BeginDayInterval2forTier3") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval2forTier3") ?? String.Empty);
                    }


                    if (moopValue == "1")
                    {
                        if (decimal.TryParse(copay1, out c1) && int.TryParse(endDay1, out ed1) && decimal.TryParse(copay2, out c2) && int.TryParse(beginDay2, out bd2) && int.TryParse(endDay2, out ed2))
                        {
                            bd2 = --bd2;
                            var totalDays = ed1 + (ed2 - bd2);
                            if (totalDays <= 60)
                            {
                                decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2));

                                if ((totalDays <= 6) && (totalCopay > 1817)) return false;
                                if ((totalDays <= 10) && (totalCopay > 1996)) return false;
                                if ((totalDays <= 60) && (totalCopay > 4235)) return false;
                            }
                            else
                            {
                                if (ed1 >= 60)
                                {
                                    decimal totalCopay = c1 * 60;
                                    if (totalCopay > 4235) return false;
                                }
                                else
                                {
                                    var extraDays = 60 - ed1;
                                    decimal totalCopay = (c1 * ed1) + (c2 * extraDays);
                                    if (totalCopay > 4235) return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (decimal.TryParse(copay1, out c1) && int.TryParse(endDay1, out ed1) && decimal.TryParse(copay2, out c2) && int.TryParse(beginDay2, out bd2) && int.TryParse(endDay2, out ed2))
                        {
                            bd2 = --bd2;
                            var totalDays = ed1 + (ed2 - bd2);
                            if (totalDays <= 60)
                            {
                                decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2));

                                if ((totalDays <= 10) && (totalCopay > 2495)) return false;
                                if ((totalDays <= 60) && (totalCopay > 2271)) return false;
                            }
                            else
                            {
                                if (ed1 >= 60)
                                {
                                    decimal totalCopay = c1 * 60;
                                    if (totalCopay > 2271) return false;

                                }
                                else
                                {
                                    var extraDays = 60 - ed1;
                                    decimal totalCopay = (c1 * ed1) + (c2 * extraDays);
                                    if (totalCopay > 2271) return false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            { }
            return true;
        }

        private bool MOOPAmountValidationAcuteInterval3(string leftOperand, string rightOperand, JObject _rowData)
        {
            string moopValue = String.Empty, tierValue = String.Empty, copay1 = String.Empty, endDay1 = String.Empty, copay2 = String.Empty, beginDay2 = String.Empty, endDay2 = String.Empty, copay3 = String.Empty, beginDay3 = String.Empty, endDay3 = String.Empty;
            decimal c1, c2, c3;
            int ed1, bd2, ed2, bd3, ed3;

            try
            {
                if (_rowData != null)
                {
                    //if (rightOperand.IndexOf(',') != -1)
                    //{
                    //    tierValue = rightOperand.Split(',')[0];
                    //    designYear = rightOperand.Split(',')[1];
                    //    if (String.IsNullOrEmpty(designYear) && designYear.Equals("2019"))
                    //    {
                    //        mandatory60 = 4314; mandatory10 = 2042; mandatory6 = 1860; voluntary10 = 2552; voluntary6 = 2325;
                    //    }
                    //}
                    //else
                    //    tierValue = rightOperand;
                    moopValue = Convert.ToString(_rowData.SelectToken("SectionDPlanLevel.MaxEnrolleCostLimitInNetwork.IsyourInNetworkMaximumEnrolleeOutofPocketMOOPCostattheVoluntaryorManda") ?? String.Empty);
                    tierValue = rightOperand;
                    if (tierValue.Equals("Tier1"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval1") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval1") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval2") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.BeginDayInterval2") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval2") ?? String.Empty);
                        copay3 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval3") ?? String.Empty);
                        beginDay3 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.BeginDayInterval3") ?? String.Empty);
                        endDay3 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval3") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier2"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.CopaymentAmtInterval1forTier2") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.EndDayInterval1forTier2") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.CopaymentAmtInterval2forTier2") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.BeginDayInterval2forTier2") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.EndDayInterval2forTier2") ?? String.Empty);
                        copay3 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.CopaymentAmtInterval3forTier2") ?? String.Empty);
                        beginDay3 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.BeginDayInterval3forTier2") ?? String.Empty);
                        endDay3 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.EndDayInterval3forTier2") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier3"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval1forTier3") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval1forTier3") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval2forTier3") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.BeginDayInterval2forTier3") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval2forTier3") ?? String.Empty);
                        copay3 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval3forTier3") ?? String.Empty);
                        beginDay3 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.BeginDayInterval3forTier3") ?? String.Empty);
                        endDay3 = Convert.ToString(_rowData.SelectToken("AInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval3forTier3") ?? String.Empty);
                    }


                    if (moopValue == "1")
                    {
                        if (decimal.TryParse(copay1, out c1) && int.TryParse(endDay1, out ed1) && decimal.TryParse(copay2, out c2) && int.TryParse(beginDay2, out bd2) && int.TryParse(endDay2, out ed2) && decimal.TryParse(copay3, out c3) && int.TryParse(beginDay3, out bd3) && int.TryParse(endDay3, out ed3))
                        {
                            bd2 = --bd2; bd3 = --bd3;
                            var totalDays = ed1 + (ed2 - bd2) + (ed3 - bd3);
                            if (totalDays <= 60)
                            {
                                decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2)) + (c3 * (ed3 - bd3));
                                if ((totalDays <= 6) && (totalCopay > 1817)) return false;
                                if ((totalDays <= 10) && (totalCopay > 1996)) return false;
                                if ((totalDays <= 60) && (totalCopay > 4235)) return false;
                            }
                            else
                            {
                                if (ed1 >= 60)
                                {
                                    decimal totalCopay = (c1 * 60);
                                    if (totalCopay > 4235) return false;
                                }
                                else if (ed1 + ed2 >= 60)
                                {
                                    var extraDays = 60 - ed1;

                                    decimal totalCopay = (c1 * ed1) + (c2 * extraDays);
                                    if (totalCopay > 4235) return false;
                                }
                                else
                                {
                                    var extraDays = 60 - (ed1 + ed2);

                                    decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2)) + (c3 * extraDays);
                                    if (totalCopay > 4235) return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (decimal.TryParse(copay1, out c1) && int.TryParse(endDay1, out ed1) && decimal.TryParse(copay2, out c2) && int.TryParse(beginDay2, out bd2) && int.TryParse(endDay2, out ed2) && decimal.TryParse(copay3, out c3) && int.TryParse(beginDay3, out bd3) && int.TryParse(endDay3, out ed3))
                        {
                            bd2 = --bd2; bd3 = --bd3;
                            var totalDays = ed1 + (ed2 - bd2) + (ed3 - bd3);

                            if (totalDays <= 60)
                            {
                                decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2)) + (c3 * (ed3 - bd3));
                                if ((totalDays <= 10) && (totalCopay > 2495)) return false;
                                if ((totalDays <= 60) && (totalCopay > 2271)) return false;
                            }
                            else
                            {
                                if (ed1 >= 60)
                                {
                                    decimal totalCopay = (c1 * 60);
                                    if (totalCopay > 2271) return false;
                                }
                                else if (ed1 + ed2 >= 60)
                                {
                                    var extraDays = 60 - ed1;

                                    decimal totalCopay = (c1 * ed1) + (c2 * extraDays);
                                    if (totalCopay > 2271) return false;
                                }
                                else
                                {
                                    var extraDays = 60 - (ed1 + ed2);

                                    decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2)) + (c3 * extraDays);
                                    if (totalCopay > 2271) return false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            { }
            return true;
        }

        private bool MOOPAmountValidationPsychiatricInterval1(string leftOperand, string rightOperand, JObject _rowData)
        {
            decimal n1; int n2;
            string moopValue = String.Empty, endDay1 = String.Empty, tierValue = String.Empty; ;

            try
            {
                if (_rowData != null)
                {
                    //if (rightOperand.IndexOf(',') != -1)
                    //{
                    //    tierValue = rightOperand.Split(',')[0];
                    //    designYear = rightOperand.Split(',')[1];
                    //    if (String.IsNullOrEmpty(designYear) && designYear.Equals("2019"))
                    //    {
                    //        mandatory60 = 2190; mandatory15 = 1660; voluntary60 = 2737; voluntary15 = 2075;
                    //    }
                    //}
                    //else
                    //    tierValue = rightOperand;
                    moopValue = Convert.ToString(_rowData.SelectToken("SectionDPlanLevel.MaxEnrolleCostLimitInNetwork.IsyourInNetworkMaximumEnrolleeOutofPocketMOOPCostattheVoluntaryorManda") ?? String.Empty);
                    tierValue = rightOperand;
                    if (tierValue.Equals("Tier1"))
                    {
                        endDay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.IP7MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval1") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier2"))
                    {
                        endDay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier2.EndDayInterval1forTier3") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier3"))
                    {
                        endDay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval1forTier3") ?? String.Empty);
                    }

                    if (decimal.TryParse(leftOperand, out n1) && int.TryParse(endDay1, out n2) && string.IsNullOrEmpty(moopValue))
                    {
                        if (MOOPAmountValidationPsychiatricInterval3(leftOperand, tierValue, _rowData))
                        {
                            if (MOOPAmountValidationPsychiatricInterval2(leftOperand, tierValue, _rowData))
                            {
                                if (moopValue == "1")
                                {
                                    if (n2 <= 15)
                                    {
                                        if (n1 * n2 > 2025) return false;
                                    }
                                    else if (n2 > 15 && n2 <= 60)
                                    {
                                        if (n1 * n2 > 2677) return false;
                                    }
                                    else if (n1 * n2 > 2677) return false;
                                }
                                else
                                {
                                    if (n2 <= 15)
                                    {
                                        if (n1 * n2 > 1620) return false;
                                    }
                                    else if (n2 > 15 && n2 <= 60)
                                    {
                                        if (n1 * n2 > 2142) return false;
                                    }
                                    else if (n1 * n2 > 2142) return false;
                                }
                            }
                            else return false;
                        }
                        else return false;
                    }
                }
            }
            catch (Exception)
            { }
            return true;
        }

        private bool MOOPAmountValidationPsychiatricInterval2(string leftOperand, string rightOperand, JObject _rowData)
        {
            string moopValue = String.Empty, tierValue = String.Empty, copay1 = String.Empty, endDay1 = String.Empty, copay2 = String.Empty, beginDay2 = String.Empty, endDay2 = String.Empty;
            decimal c1, c2;
            int ed1, bd2, ed2;

            try
            {
                if (_rowData != null)
                {
                    //if (rightOperand.IndexOf(',') != -1)
                    //{
                    //    tierValue = rightOperand.Split(',')[0];
                    //    designYear = rightOperand.Split(',')[1];
                    //    if (String.IsNullOrEmpty(designYear) && designYear.Equals("2019"))
                    //    {
                    //        mandatory60 = 2190; mandatory15 = 1660; voluntary60 = 2737; voluntary15 = 2075;
                    //    }
                    //}
                    //else
                    //    tierValue = rightOperand;
                    moopValue = Convert.ToString(_rowData.SelectToken("SectionDPlanLevel.MaxEnrolleCostLimitInNetwork.IsyourInNetworkMaximumEnrolleeOutofPocketMOOPCostattheVoluntaryorManda") ?? String.Empty);
                    tierValue = rightOperand;
                    if (tierValue.Equals("Tier1"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.IP7MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval1") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.IP7MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval1") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.IP7MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval2") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.IP7MedicarecoveredCopaymentCostSharingforTier1.BeginDayInterval2") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.IP7MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval2") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier2"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier2.CopaymentAmtInterval1forTier2") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier2.EndDayInterval1forTier2") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier2.CopaymentAmtInterval2forTier2") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier2.BeginDayInterval2forTier2") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier2.EndDayInterval2forTier2") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier3"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval1forTier3") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval1forTier3") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval2forTier3") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier3.BeginDayInterval2forTier3") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval2forTier3") ?? String.Empty);
                    }

                    if (moopValue == "1")
                    {
                        if (decimal.TryParse(copay1, out c1) && int.TryParse(endDay1, out ed1) && decimal.TryParse(copay2, out c2) && int.TryParse(beginDay2, out bd2) && int.TryParse(endDay2, out ed2))
                        {
                            bd2 = --bd2;
                            var totalDays = ed1 + (ed2 - bd2);
                            if (totalDays <= 60)
                            {
                                decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2));

                                if ((totalDays <= 15) && (totalCopay > 2025)) return false;
                                if ((totalDays <= 60) && (totalCopay > 2677)) return false;
                            }
                            else
                            {
                                if (ed1 >= 60)
                                {
                                    decimal totalCopay = c1 * 60;
                                    if (totalCopay > 2677) return false;

                                }
                                else
                                {
                                    var extraDays = 60 - ed1;
                                    decimal totalCopay = (c1 * ed1) + (c2 * extraDays);
                                    if (totalCopay > 2677) return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (decimal.TryParse(copay1, out c1) && int.TryParse(endDay1, out ed1) && decimal.TryParse(copay2, out c2) && int.TryParse(beginDay2, out bd2) && int.TryParse(endDay2, out ed2))
                        {
                            bd2 = --bd2;
                            var totalDays = ed1 + (ed2 - (--bd2));
                            if (totalDays <= 60)
                            {
                                decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2));

                                if ((totalDays <= 15) && (totalCopay > 1620)) return false;
                                if ((totalDays <= 60) && (totalCopay > 2142)) return false;
                            }
                            else
                            {
                                if (ed1 >= 60)
                                {
                                    decimal totalCopay = c1 * 60;
                                    if (totalCopay > 2142) return false;

                                }
                                else
                                {
                                    var extraDays = 60 - ed1;
                                    decimal totalCopay = (c1 * ed1) + (c2 * extraDays);
                                    if (totalCopay > 2142) return false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            { }
            return true;
        }

        private bool MOOPAmountValidationPsychiatricInterval3(string leftOperand, string rightOperand, JObject _rowData)
        {
            string moopValue = String.Empty, tierValue = String.Empty, copay1 = String.Empty, endDay1 = String.Empty, copay2 = String.Empty, beginDay2 = String.Empty, endDay2 = String.Empty, copay3 = String.Empty, beginDay3 = String.Empty, endDay3 = String.Empty;
            decimal c1, c2, c3;
            int ed1, bd2, ed2, bd3, ed3;

            try
            {

                if (_rowData != null)
                {
                    //if (rightOperand.IndexOf(',') != -1)
                    //{
                    //    tierValue = rightOperand.Split(',')[0];
                    //    designYear = rightOperand.Split(',')[1];
                    //    if (String.IsNullOrEmpty(designYear) && designYear.Equals("2019"))
                    //    {
                    //        mandatory60 = 2190; mandatory15 = 1660; voluntary60 = 2737; voluntary15 = 2075;
                    //    }
                    //}
                    //else
                    //    tierValue = rightOperand;
                    moopValue = Convert.ToString(_rowData.SelectToken("SectionDPlanLevel.MaxEnrolleCostLimitInNetwork.IsyourInNetworkMaximumEnrolleeOutofPocketMOOPCostattheVoluntaryorManda") ?? String.Empty);
                    tierValue = rightOperand;
                    if (tierValue.Equals("Tier1"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.IP7MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval1") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.IP7MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval1") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.IP7MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval2") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.IP7MedicarecoveredCopaymentCostSharingforTier1.BeginDayInterval2") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.IP7MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval2") ?? String.Empty);
                        copay3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.IP7MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval3") ?? String.Empty);
                        beginDay3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.IP7MedicarecoveredCopaymentCostSharingforTier1.BeginDayInterval3") ?? String.Empty);
                        endDay3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.IP7MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval3") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier2"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier2.CopaymentAmtInterval1forTier2") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier2.BeginDayInterval1forTier2") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier2.CopaymentAmtInterval2forTier2") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier2.BeginDayInterval2forTier2") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier2.EndDayInterval2forTier2") ?? String.Empty);
                        copay3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier2.CopaymentAmtInterval3forTier2") ?? String.Empty);
                        beginDay3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier2.BeginDayInterval3forTier2") ?? String.Empty);
                        endDay3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier2.EndDayInterval3forTier2") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier3"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval1forTier3") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval1forTier3") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval2forTier3") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier3.BeginDayInterval2forTier3") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval2forTier3") ?? String.Empty);
                        copay3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval3forTier3") ?? String.Empty);
                        beginDay3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier3.BeginDayInterval3forTier3") ?? String.Empty);
                        endDay3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.IP8MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval3forTier3") ?? String.Empty);
                    }

                    if (moopValue == "1")
                    {
                        if (decimal.TryParse(copay1, out c1) && int.TryParse(endDay1, out ed1) && decimal.TryParse(copay2, out c2) && int.TryParse(beginDay2, out bd2) && int.TryParse(endDay2, out ed2) && decimal.TryParse(copay3, out c3) && int.TryParse(beginDay3, out bd3) && int.TryParse(endDay3, out ed3))
                        {
                            bd2 = --bd2; bd3 = --bd3;
                            var totalDays = ed1 + (ed2 - bd2) + (ed3 - bd3);
                            if (totalDays <= 60)
                            {
                                decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2)) + (c3 * (ed3 - bd3));
                                if ((totalDays <= 15) && (totalCopay > 2025)) return false;
                                if ((totalDays <= 60) && (totalCopay > 2677)) return false;
                            }
                            else
                            {
                                if (ed1 >= 60)
                                {
                                    decimal totalCopay = (c1 * 60);
                                    if (totalCopay > 2677) return false;
                                }
                                else if (ed1 + ed2 >= 60)
                                {
                                    var extraDays = 60 - ed1;

                                    decimal totalCopay = (c1 * ed1) + (c2 * extraDays);
                                    if (totalCopay > 2677) return false;
                                }
                                else
                                {
                                    var extraDays = 60 - (ed1 + ed2);

                                    decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2)) + (c3 * extraDays);
                                    if (totalCopay > 2677) return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (decimal.TryParse(copay1, out c1) && int.TryParse(endDay1, out ed1) && decimal.TryParse(copay2, out c2) && int.TryParse(beginDay2, out bd2) && int.TryParse(endDay2, out ed2) && decimal.TryParse(copay3, out c3) && int.TryParse(beginDay3, out bd3) && int.TryParse(endDay3, out ed3))
                        {
                            bd2 = --bd2; bd3 = --bd3;
                            var totalDays = ed1 + (ed2 - bd2) + (ed3 - bd3);
                            if (totalDays <= 60)
                            {
                                decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2)) + (c3 * (ed3 - bd3));
                                if ((totalDays <= 15) && (totalCopay > 1620)) return false;
                                if ((totalDays <= 60) && (totalCopay > 2142)) return false;
                            }
                            else
                            {
                                if (ed1 >= 60)
                                {
                                    decimal totalCopay = (c1 * 60);
                                    if (totalCopay > 2142) return false;
                                }
                                else if (ed1 + ed2 >= 60)
                                {
                                    var extraDays = 60 - ed1;

                                    decimal totalCopay = (c1 * ed1) + (c2 * extraDays);
                                    if (totalCopay > 2142) return false;
                                }
                                else
                                {
                                    var extraDays = 60 - (ed1 + ed2);

                                    decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2)) + (c3 * extraDays);
                                    if (totalCopay > 2142) return false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return true;
        }

        private bool MOOPAmountValidationSNF(string leftOperand, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            decimal n1, target; int n2, beginDay;
            string moopValue = String.Empty, designYear = String.Empty;
            bool isCoins = true;
            int voluntary20Value = 20, mandatory20Value = 0;
            decimal voluntary100Value = Convert.ToDecimal(167.50), mandaroty100Value = Convert.ToDecimal(167.50);
            int.TryParse(leftOperand, out beginDay);
            String targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementFullName) ?? String.Empty);
            decimal.TryParse(targetValue, out target);
            try
            {

                if (_rowData != null)
                {
                    moopValue = Convert.ToString(_rowData.SelectToken("SectionDPlanLevel.MaxEnrolleCostLimitInNetwork.IsyourInNetworkMaximumEnrolleeOutofPocketMOOPCostattheVoluntaryorManda") ?? String.Empty);

                    if (!string.IsNullOrEmpty(targetValue) && targetValue.Contains("."))
                        isCoins = false;
                    if (!isCoins && beginDay <= 20)
                    {
                        if (moopValue == "1")
                        {
                            if (target <= 20)
                                return false;
                        }
                        else
                        {
                            if (moopValue == "2")
                            {
                                if (target != 0)
                                    return false;
                            }
                            else
                                return true;
                        }

                    }
                    else
                    {


                        if (decimal.TryParse(targetValue, out n1) && int.TryParse(rightOperand, out n2) && !string.IsNullOrEmpty(moopValue))
                        {
                            if (moopValue == "1")
                            {
                                if (n2 <= 20)
                                {
                                    if (isCoins && ((n1 / 100) * 518) > voluntary20Value) return false;
                                    else if (!isCoins && (n1 > voluntary20Value)) return false;
                                }
                                else
                                {
                                    if (isCoins && ((n1 / 100) * 518) > voluntary100Value) return false;
                                    else if (!isCoins && (n1 > voluntary100Value)) return false;
                                }
                            }
                            else
                            {
                                if (n2 <= 20)
                                {
                                    if (isCoins && ((n1 / 100) * 518) > mandatory20Value) return false;
                                    else if (!isCoins && (n1 > mandatory20Value)) return false;
                                }
                                else
                                {
                                    if (isCoins && ((n1 / 100) * 518) > mandaroty100Value) return false;
                                    else if (!isCoins && (n1 > mandaroty100Value)) return false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            { }
            return true;
        }
        private bool MOOPAmountValidationSNF(string leftOperand, string rightOperand, JObject _rowData)
        {
            decimal n1; int n2;
            string moopValue = String.Empty;
            bool isCoins = true;
            int voluntary20Value = 20, mandatory20Value = 0;
            decimal voluntary100Value = Convert.ToDecimal(167.50), mandaroty100Value = Convert.ToDecimal(167.50);

            try
            {
                if (_rowData != null)
                {
                    moopValue = Convert.ToString(_rowData.SelectToken("SectionDPlanLevel.MaxEnrolleCostLimitInNetwork.IsyourInNetworkMaximumEnrolleeOutofPocketMOOPCostattheVoluntaryorManda") ?? String.Empty);

                    if (!string.IsNullOrEmpty(leftOperand) && leftOperand.Contains("."))
                        isCoins = false;

                    if (decimal.TryParse(leftOperand, out n1) && int.TryParse(rightOperand, out n2) && !string.IsNullOrEmpty(moopValue))
                    {
                        if (moopValue == "1")
                        {
                            if (n2 <= 20)
                            {
                                if (isCoins && ((n1 / 100) * 518) > voluntary20Value) return false;
                                else if (!isCoins && (n1 > voluntary20Value)) return false;
                            }
                            else
                            {
                                if (isCoins && ((n1 / 100) * 518) > voluntary100Value) return false;
                                else if (!isCoins && (n1 > voluntary100Value)) return false;
                            }
                        }
                        else
                        {
                            if (n2 <= 20)
                            {
                                if (isCoins && ((n1 / 100) * 518) > mandatory20Value) return false;
                                else if (!isCoins && (n1 > mandatory20Value)) return false;
                            }
                            else
                            {
                                if (isCoins && ((n1 / 100) * 518) > mandaroty100Value) return false;
                                else if (!isCoins && (n1 > mandaroty100Value)) return false;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            { }
            return true;
        }
        private bool MOOPAmountValidationSNF2019(string leftOperand, string rightOperand, JObject _rowData)
        {
            decimal n1; int n2;
            string moopValue = String.Empty, designYear = String.Empty;
            bool isCoins = true;
            int voluntary20Value = 20, mandatory20Value = 0;
            decimal voluntary100Value = Convert.ToDecimal(172.00), mandaroty100Value = Convert.ToDecimal(172.00);

            try
            {
                if (_rowData != null)
                {
                    moopValue = Convert.ToString(_rowData.SelectToken("SectionDPlanLevel.MaxEnrolleCostLimitInNetwork.IsyourInNetworkMaximumEnrolleeOutofPocketMOOPCostattheVoluntaryorManda") ?? String.Empty);

                    if (!string.IsNullOrEmpty(leftOperand) && leftOperand.Contains("."))
                        isCoins = false;

                    if (decimal.TryParse(leftOperand, out n1) && int.TryParse(rightOperand, out n2) && !string.IsNullOrEmpty(moopValue))
                    {
                        if (moopValue == "1")
                        {
                            if (n2 <= 20)
                            {
                                if (isCoins && ((n1 / 100) * 518) > voluntary20Value) return false;
                                else if (!isCoins && (n1 > voluntary20Value)) return false;
                            }
                            else
                            {
                                if (isCoins && ((n1 / 100) * 518) > voluntary100Value) return false;
                                else if (!isCoins && (n1 > voluntary100Value)) return false;
                            }
                        }
                        else
                        {
                            if (n2 <= 20)
                            {
                                if (isCoins && ((n1 / 100) * 518) > mandatory20Value) return false;
                                else if (!isCoins && (n1 > mandatory20Value)) return false;
                            }
                            else
                            {
                                if (isCoins && ((n1 / 100) * 518) > mandaroty100Value) return false;
                                else if (!isCoins && (n1 > mandaroty100Value)) return false;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            { }
            return true;
        }
        private bool RepeaterCoinsuranceCheck(string leftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            decimal n1, n2;
            String targetValue, groupID, isMedicareCovered, isOONGroup, checkMaxValue, iscoinsurance;
            try
            {
                isOONGroup = rightOperand != null ? rightOperand.Split(',')[1] : string.Empty;
                checkMaxValue = rightOperand != null ? rightOperand.Split(',')[0] : string.Empty;
                targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);

                if (isOONGroup.Equals("OON"))
                {
                    //var result = true;
                    groupID = Convert.ToString(_rowData.SelectToken("OONGroupID") ?? string.Empty);
                    var oonGroupRepeater = _formData.SelectToken("OONGroups.OONGroupsBase1").Where(x => Convert.ToString(x["OONGroupID"] ?? string.Empty) == groupID).FirstOrDefault();
                    var oONGroupRepeaterBase2 = _formData.SelectToken("OONGroups.OONGroupsBase2").Where(x => Convert.ToString(x["OONGroupID"] ?? string.Empty) == groupID).FirstOrDefault();

                    iscoinsurance = oONGroupRepeaterBase2 != null ? Convert.ToString(oONGroupRepeaterBase2.SelectToken("IsthereanOONCoinsuranceforthisGroup")) : string.Empty;
                    if (iscoinsurance == "1")
                    {
                        if (targetValue != "")
                        {
                            isMedicareCovered = oonGroupRepeater != null ? Convert.ToString(oonGroupRepeater.SelectToken("SelectthebenefitsthatapplytotheOONGroups")) : string.Empty;

                            if (decimal.TryParse(targetValue, out n1) && decimal.TryParse(checkMaxValue, out n2))
                            {
                                if (isMedicareCovered.Contains("01"))
                                    return n2 >= n1;
                                //if (result)
                                //{
                                //    rightOperand = rightOperand.Split(',')[2];
                                //    return RepeaterMINMAXCheck(leftOperandName, rightOperand, _rowData, rule);
                                //}
                                //else
                                //    return result;
                            }
                        }
                        else
                            return false;
                    }
                }
                else
                {
                    var result = true;
                    groupID = Convert.ToString(_rowData.SelectToken("POSGroupID") ?? string.Empty);
                    var pOSGroupRepeater = _formData.SelectToken("POSGroups.POSGroupsBase1").Where(x => Convert.ToString(x["POSGroupID"] ?? string.Empty) == groupID).FirstOrDefault();

                    iscoinsurance = pOSGroupRepeater != null ? Convert.ToString(pOSGroupRepeater.SelectToken("IsthereaPOSCoinsuranceforthisGroup")) : string.Empty;
                    if (iscoinsurance == "YES")
                    {
                        if (targetValue != "")
                        {
                            if (decimal.TryParse(targetValue, out n1) && decimal.TryParse(checkMaxValue, out n2))
                                result = n2 >= n1;

                            if (result)
                            {
                                rightOperand = rightOperand.Split(',')[1];
                                return RepeaterMINMAXCheck(leftOperandName, rightOperand, _rowData, rule);
                            }
                            else
                                return result;
                        }
                        else
                            return false;
                    }
                }

            }
            catch (Exception) { }
            return true;
        }
        private bool RepeaterDeductibleCheck(string leftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            decimal n1, n2;
            String targetValue, groupID, isMedicareCovered, isNOFlag;
            try
            {
                isNOFlag = rightOperand != null ? rightOperand : string.Empty;
                targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);

                groupID = Convert.ToString(_rowData.SelectToken("OONGroupID") ?? string.Empty);
                var oonGroupRepeater = _formData.SelectToken("OONGroups.OONGroupsBase1").Where(x => Convert.ToString(x["OONGroupID"] ?? string.Empty) == groupID).FirstOrDefault();
                isMedicareCovered = oonGroupRepeater != null ? Convert.ToString(oonGroupRepeater.SelectToken("SelectthebenefitsthatapplytotheOONGroups")) : string.Empty;

                if (decimal.TryParse(targetValue, out n1) && decimal.TryParse(isNOFlag, out n2))
                {
                    if (isMedicareCovered.Contains("01"))
                        return targetValue == isNOFlag;
                }

            }
            catch (Exception) { }
            return true;
        }

        private bool RepeaterPlanMaxCoverageCheck(string leftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            decimal n1, n2;
            String targetValue, groupID, isMedicareCovered, isNOFlag;
            try
            {
                isNOFlag = rightOperand != null ? rightOperand : string.Empty;
                targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);
                string PlanType = _formData.SelectToken("SectionA.SectionA1.PlanType").ToString();
                groupID = Convert.ToString(_rowData.SelectToken("OONGroupID") ?? string.Empty);
                var oonGroupRepeater = _formData.SelectToken("OONGroups.OONGroupsBase1").Where(x => Convert.ToString(x["OONGroupID"] ?? string.Empty) == groupID).FirstOrDefault();
                isMedicareCovered = oonGroupRepeater != null ? Convert.ToString(oonGroupRepeater.SelectToken("SelectthebenefitsthatapplytotheOONGroups")) : string.Empty;
                if (PlanType != "9")
                {
                    if (decimal.TryParse(targetValue, out n1) && decimal.TryParse(isNOFlag, out n2))
                    {
                        if (isMedicareCovered.Contains("01"))
                            return targetValue == isNOFlag;
                    }
                }

            }
            catch (Exception) { }
            return true;
        }

        private bool RepeaterMINMAXCheck(string leftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            decimal n1, n2;
            bool isCopay = false;
            String targetValue, groupID, maxValue, isMinValue, minValue, isCopayment;
            try
            {
                isMinValue = rightOperand != null ? rightOperand : string.Empty;
                targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);
                isCopay = rule.UIElementName.IndexOf("Copayment") != -1 ? true : false;


                groupID = Convert.ToString(_rowData.SelectToken("POSGroupID") ?? string.Empty);
                var posGroupRepeater = _formData.SelectToken("POSGroups.POSGroupsBase1").Where(x => Convert.ToString(x["POSGroupID"] ?? string.Empty) == groupID).FirstOrDefault();

                if (!isCopay)
                {
                    if (isMinValue.Equals("MIN"))
                    {
                        maxValue = posGroupRepeater != null ? Convert.ToString(posGroupRepeater.SelectToken("EnterMaximumCoinsurancePercentageforthisGroup")) : string.Empty;
                        if (decimal.TryParse(targetValue, out n1) && decimal.TryParse(maxValue, out n2))
                            return n1 <= n2;
                    }
                    else
                    {
                        minValue = posGroupRepeater != null ? Convert.ToString(posGroupRepeater.SelectToken("EnterMinimumCoinsurancePercentageforthisGroup")) : string.Empty;
                        if (decimal.TryParse(targetValue, out n1) && decimal.TryParse(minValue, out n2))
                            return n1 >= n2;
                    }
                }
                else
                {
                    isCopayment = posGroupRepeater != null ? Convert.ToString(posGroupRepeater.SelectToken("IsthereaPOSCopaymentforthisGroup")) : string.Empty;
                    if (isCopayment == "YES")
                    {
                        if (targetValue != "")
                        {
                            if (isMinValue.Equals("MIN"))
                            {
                                maxValue = posGroupRepeater != null ? Convert.ToString(posGroupRepeater.SelectToken("EnterMaximumCopaymentAmountforthisGroup")) : string.Empty;
                                if (decimal.TryParse(targetValue, out n1) && decimal.TryParse(maxValue, out n2))
                                    return n1 <= n2;
                            }
                            else
                            {
                                minValue = posGroupRepeater != null ? Convert.ToString(posGroupRepeater.SelectToken("EnterMinimumCopaymentAmountforthisGroup")) : string.Empty;
                                if (decimal.TryParse(targetValue, out n1) && decimal.TryParse(minValue, out n2))
                                    return n1 >= n2;

                            }
                        }
                        return false;
                    }
                    else if (targetValue == "")
                        return true;
                }
            }
            catch (Exception) { }
            return true;
        }
        private bool RepeaterIsRequired(string leftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            //wellcare   string targetValue, groupID, isCopayment, pOSgroupID, isMedicare, posgroupIDGroup2, medicareisCopaymentgroup2, tierID, isDrugBenefitSelected;         
            String targetValue, groupID, isCopayment, posisCopayment, medicareisCopayment, posgroupID, posgroupID1, medicareisCopaymentgroup2, posgroupIDGroup2;
            targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);
            groupID = Convert.ToString(_rowData.SelectToken("OONGroupID") ?? string.Empty);
            var oONGroupRepeater = _formData.SelectToken("OONGroups.OONGroupsBase2").Where(x => Convert.ToString(x["OONGroupID"] ?? string.Empty) == groupID).FirstOrDefault();
            isCopayment = oONGroupRepeater != null ? Convert.ToString(oONGroupRepeater.SelectToken("IsthereanOONCopaymentforthisGroup")) : string.Empty;

            if (isCopayment == "1")
            {
                if (targetValue != "")
                {
                    return true;
                }
                return false;
            }


            /* wellcare
              pOSgroupID = Convert.ToString(_rowData.SelectToken("POSGroupID") ?? string.Empty);
                        var pONGroupRepeater = _formData.SelectToken("POSGroups.POSGroupsBase1").Where(x => Convert.ToString(x["POSGroupID"] ?? string.Empty) == pOSgroupID).FirstOrDefault();
            */
            posgroupID = Convert.ToString(_rowData.SelectToken("POSGroupID") ?? string.Empty);
            var posGroupRepeater = _formData.SelectToken("POSGroups.POSGroupsBase2").Where(x => Convert.ToString(x["POSGroupID"] ?? string.Empty) == posgroupID).FirstOrDefault();
            posisCopayment = posGroupRepeater != null ? Convert.ToString(posGroupRepeater.SelectToken("IsthereaPOSMaximumPlanBenefitCoverageamountforthisgroup")) : string.Empty;

            if (posisCopayment == "1")
            {
                if (targetValue != "")
                {
                    return true;
                }
                return false;
            }



            /* wellcare
               isMedicare = pONGroupRepeater != null ? Convert.ToString(pONGroupRepeater.SelectToken("SelectthebenefitsthatapplytothePOSBenefitsforthisGroup")) : string.Empty;
                        if (isMedicare.Contains(rightOperand))
                        {
                            if (targetValue != "")
                            {
                                return true;
                            }
                            return false;
                        }
            */
            posgroupID1 = Convert.ToString(_rowData.SelectToken("POSGroupID") ?? string.Empty);
            var posGroupRepeater1 = _formData.SelectToken("POSGroups.POSGroupsBase1").Where(x => Convert.ToString(x["POSGroupID"] ?? string.Empty) == posgroupID1).FirstOrDefault();
            medicareisCopayment = posGroupRepeater1 != null ? Convert.ToString(posGroupRepeater1.SelectToken("SelectthebenefitsthatapplytothePOSBenefitsforthisGroup")) : string.Empty;

            if (medicareisCopayment.Contains(rightOperand))
            {
                if (targetValue != "")
                {
                    return true;
                }
                return false;
            }

            posgroupIDGroup2 = Convert.ToString(_rowData.SelectToken("POSGroupID") ?? string.Empty);
            var posGroup2Repeater = _formData.SelectToken("POSGroups.POSGroupsBase2").Where(x => Convert.ToString(x["POSGroupID"] ?? string.Empty) == posgroupIDGroup2).FirstOrDefault();
            medicareisCopaymentgroup2 = posGroup2Repeater != null ? Convert.ToString(posGroup2Repeater.SelectToken("IsthereaPOSDeductibleforthisgroup")) : string.Empty;

            if (medicareisCopaymentgroup2 == "1")
            {
                if (targetValue != "")
                {
                    return true;
                }
                return false;
            }


            return true;




        }

        private bool checkIfMandetoryForMedicare(string leftOperand, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            string targetValue, groupID;
            List<string> list;
            List<string> services;
            if (rule.IsParentRepeater)
            {
                groupID = Convert.ToString(_rowData.SelectToken("OONGroupID") ?? string.Empty);
                var oONGroup1Repeater = _formData.SelectToken("OONGroups.OONGroupsBase1").Where(x => Convert.ToString(x["OONGroupID"] ?? string.Empty) == groupID).FirstOrDefault();
                targetValue = oONGroup1Repeater != null ? Convert.ToString(oONGroup1Repeater.SelectToken("SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis")) : string.Empty;

            }
            else
            {
                targetValue = Convert.ToString(_formData.SelectToken(rule.UIElementFullName) ?? String.Empty);
            }
            string sOTServices = targetValue.Replace("\r\n", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty).Replace("\"", string.Empty).Replace(" ", string.Empty);
            string[] Sservices = sOTServices.Split(',');
            if (rule.SuccessValue == "checkIfMandetoryForMedicare")
            {
                services = new List<string>() { "10a", "11a", "11b", "11c", "12", "14a", "14d", "14e", "5", "6", "7a", "7c", "7d", "7e", "7g", "7h", "7i", "8a1", "8a2", "8b1", "8b2", "8b3", "9a", "9b", "9c" };
                list = new List<string>() { "10a", "11a", "11b", "11c", "12", "14a", "14d", "14e", "5", "6", "7a", "7c", "7d", "7e", "7g", "7h", "7i", "8a1", "8a2", "8b1", "8b2", "8b3", "9a", "9b", "9c" };

            }
            else
            {
                services = new List<string>() { "10a1", "10a2", "11a", "11b", "11c", "12", "14a", "14d", "14e1", "14e2", "14e3", "14e4", "14e5", "14e6", "5", "6", "7a", "7c", "7d", "7e", "7g", "7h", "7i", "8a1", "8a2", "8b1", "8b2", "8b3", "9a1", "9a2", "9b", "9c" };
                list = new List<string>() { "10a1", "10a2", "11a", "11b", "11c", "12", "14a", "14d", "14e1", "14e2", "14e3", "14e4", "14e5", "14e6", "5", "6", "7a", "7c", "7d", "7e", "7g", "7h", "7i", "8a1", "8a2", "8b1", "8b2", "8b3", "9a1", "9a2", "9b", "9c" };

            }

            foreach (string field in Sservices)
            {
                if (field != "")
                    services.Add(field);
            }
            string PlanType = _formData.SelectToken("SectionA.SectionA1.PlanType").ToString();
            string IsThisNWPlan = _formData.SelectToken("SectionA.SectionA1.Isthisanetworkplan").ToString();

            if (PlanType == "9" && IsThisNWPlan == "3")
            {
                string InpatientAcute1 = _formData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase1.SelecttypeofbenefitforAdditionalDays").ToString();
                string InpatientAcute2 = _formData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase1.SelecttypeofbenefitforNonMedicarecoveredstay").ToString();
                string InpatientAcute3 = _formData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase1.SelecttypeofbenefitforUpgrades").ToString();
                if (InpatientAcute1 == "2" || InpatientAcute2 == "2" || InpatientAcute3 == "2")
                    list.Add("1a");

                string InpatientPsychiatric1 = _formData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase1.SelecttypeofbenefitforAdditionalDays").ToString();
                string InpatientPsychiatric2 = _formData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase1.SelecttypeofbenefitforNonMedicarecoveredstay").ToString();
                if (InpatientPsychiatric1 == "2" || InpatientPsychiatric2 == "2")
                    list.Add("1b");

                string SNF1 = _formData.SelectToken("SkilledNursingFacilitySNF.SNFBase1.SelecttypeofbenefitforAdditionalDaysbeyondMedicarecovered").ToString();
                string SNF2 = _formData.SelectToken("SkilledNursingFacilitySNF.SNFBase1.SelecttypeofbenefitfortheNonMedicarecoveredstay").ToString();
                if (SNF1 == "2" || SNF2 == "2")
                    list.Add("2");

                string CardiacandPulmonaryRehabilitation = _formData.SelectToken("CardiacandPulmonaryRehabilitationServices.CardiacandPulmonaryRehabilitationServicesBase1.SelecttypeofbenefitforAdditionalCardiacRehabilitationServices").ToString();
                if (CardiacandPulmonaryRehabilitation == "2")
                    list.Add("3-1");

                string IntensiveCardiacRehabilitation = _formData.SelectToken("CardiacandPulmonaryRehabilitationServices.CardiacandPulmonaryRehabilitationServicesBase1.SelecttypeofbenefitforAdditionalIntensiveCardiacRehabilitationServices").ToString();
                if (IntensiveCardiacRehabilitation == "2")
                    list.Add("3-2");

                string AdditionalPulmonaryRehabilitation = _formData.SelectToken("CardiacandPulmonaryRehabilitationServices.CardiacandPulmonaryRehabilitationServicesBase1.SelecttypeofbenefitforAdditionalPulmonaryRehabilitationServices").ToString();
                if (AdditionalPulmonaryRehabilitation == "2")
                    list.Add("3-3");

                string RoutineCare = _formData.SelectToken("HealthCareProfessionalServices.ChiropracticServices.ChiropracticServicesBase1.SelecttypeofbenefitforRoutineCare").ToString();
                string Other = _formData.SelectToken("HealthCareProfessionalServices.ChiropracticServices.ChiropracticServicesBase1.SelecttypeofbenefitforOtherService").ToString();
                if (RoutineCare == "2" || Other == "2")
                    list.Add("7b");

                string RoutineFootCare = _formData.SelectToken("HealthCareProfessionalServices.PodiatryServices.PodiatryServicesBase1.SelecttypeofbenefitforRoutineFootCare").ToString();
                if (RoutineFootCare == "2")
                    list.Add("7f");

                string OutpatientBloodServices = _formData.SelectToken("OutpatientServices.OutpatientBloodServices.OutpatientBloodServicesBase1.SelecttypeofbenefitforThree3PintDeductibleWaived").ToString();
                if (OutpatientBloodServices == "2")
                    list.Add("9d");

                string MedicarePartBRxDrugsGeneral = _formData.SelectToken("MedicarePartBRxDrugsGeneral.HomeInfusionBundlesServices.DoestheplanprovidePartDhomeinfusiondrugsaspartofabundledserviceasamand").ToString();
                if (MedicarePartBRxDrugsGeneral == "1")
                    list.Add("15");

                string ComprehensiveDental1 = _formData.SelectToken("ComprehensiveDental.ComprehensiveDentalBase1.SelecttypeofbenefitforNonroutineServices").ToString();
                string ComprehensiveDental2 = _formData.SelectToken("ComprehensiveDental.ComprehensiveDentalBase1.SelecttypeofbenefitforDiagnosticServices").ToString();
                string ComprehensiveDental3 = _formData.SelectToken("ComprehensiveDental.ComprehensiveDentalBase1.SelecttypeofbenefitforRestorativeServices").ToString();
                string ComprehensiveDental4 = _formData.SelectToken("ComprehensiveDental.ComprehensiveDentalBase2.SelecttypeofbenefitforEndodontics").ToString();
                string ComprehensiveDental5 = _formData.SelectToken("ComprehensiveDental.ComprehensiveDentalBase2.SelecttypeofbenefitforPeriodontics").ToString();
                string ComprehensiveDental6 = _formData.SelectToken("ComprehensiveDental.ComprehensiveDentalBase2.SelecttypeofbenefitforExtractions").ToString();
                if (ComprehensiveDental1 == "2" || ComprehensiveDental2 == "2" || ComprehensiveDental3 == "2" || ComprehensiveDental4 == "2" || ComprehensiveDental5 == "2" || ComprehensiveDental6 == "2")
                    list.Add("16b");

                string EyeExams1 = _formData.SelectToken("EyeExams.EyeExamsBase1.SelecttypeofbenefitforRoutineEyeExams").ToString();
                string EyeExams2 = _formData.SelectToken("EyeExams.EyeExamsBase1.SelecttypeofbenefitforOtherService").ToString();
                if (EyeExams1 == "2" || EyeExams2 == "2")
                    list.Add("17a");

                string Eyewear1 = _formData.SelectToken("Eyewear.EyewearBase1.SelecttypeofbenefitforContactlenses").ToString();
                string Eyewear2 = _formData.SelectToken("Eyewear.EyewearBase1.SelecttypeofbenefitforEyeglasseslensesandframes").ToString();
                string Eyewear3 = _formData.SelectToken("Eyewear.EyewearBase2.SelecttypeofbenefitforEyeglasslenses").ToString();
                string Eyewear4 = _formData.SelectToken("Eyewear.EyewearBase2.SelecttypeofbenefitforEyeglassframes").ToString();
                string Eyewear5 = _formData.SelectToken("Eyewear.EyewearBase2.SelecttypeofbenefitforUpgrades").ToString();
                if (Eyewear1 == "2" || Eyewear2 == "2" || Eyewear3 == "2" || Eyewear4 == "2" || Eyewear5 == "2")
                    list.Add("17b");

                string HearingExams1 = _formData.SelectToken("HearingExams.HearingExamsBase1.SelecttypeofbenefitforFittingEvaluationforHearingAid").ToString();
                string HearingExams2 = _formData.SelectToken("HearingExams.HearingExamsBase1.SelecttypeofbenefitforRoutineHearingExams").ToString();
                if (HearingExams1 == "2" || HearingExams2 == "2")
                    list.Add("18a");

                string OPDrupg = _formData.SelectToken("OutpatientDrugs.OutpatientDrugsBase1.Selecttypeofbenefit").ToString();
                if (OPDrupg == rightOperand)
                    list.Add("20");

                foreach (string service in list)
                {
                    if (!services.Contains(service))
                    {
                        return false;
                    }
                }
                foreach (string service in services)
                {
                    if (!list.Contains(service))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool checkIfMandetoryForNonMedicare(string leftOperand, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            string targetValue, groupID;
            if (rule.IsParentRepeater)
            {
                groupID = Convert.ToString(_rowData.SelectToken("OONGroupID") ?? string.Empty);
                var oONGroup1Repeater = _formData.SelectToken("OONGroups.OONGroupsBase1").Where(x => Convert.ToString(x["OONGroupID"] ?? string.Empty) == groupID).FirstOrDefault();
                targetValue = oONGroup1Repeater != null ? Convert.ToString(oONGroup1Repeater.SelectToken("SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort")) : string.Empty;

            }
            else
            {
                targetValue = Convert.ToString(_formData.SelectToken(rule.UIElementFullName) ?? String.Empty);
            }
            string sOTServices = targetValue.Replace("\r\n", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty).Replace("\"", string.Empty).Replace(" ", string.Empty);
            string[] Sservices = sOTServices.Split(',');
            List<string> services = new List<string>();
            foreach (string field in Sservices)
            {
                if (field != "")
                    services.Add(field);
            }
            List<string> list = new List<string>();

            string CardiacandPulmonaryRehabilitation = _formData.SelectToken("CardiacandPulmonaryRehabilitationServices.CardiacandPulmonaryRehabilitationServicesBase1.SelecttypeofbenefitforAdditionalCardiacRehabilitationServices").ToString();
            if (CardiacandPulmonaryRehabilitation == "2")
                list.Add("3-1");

            string IntensiveCardiacRehabilitation = _formData.SelectToken("CardiacandPulmonaryRehabilitationServices.CardiacandPulmonaryRehabilitationServicesBase1.SelecttypeofbenefitforAdditionalIntensiveCardiacRehabilitationServices").ToString();
            if (IntensiveCardiacRehabilitation == "2")
                list.Add("3-2");

            string AdditionalPulmonaryRehabilitation = _formData.SelectToken("CardiacandPulmonaryRehabilitationServices.CardiacandPulmonaryRehabilitationServicesBase1.SelecttypeofbenefitforAdditionalPulmonaryRehabilitationServices").ToString();
            if (AdditionalPulmonaryRehabilitation == "2")
                list.Add("3-3");

            string RoutineCare = _formData.SelectToken("HealthCareProfessionalServices.ChiropracticServices.ChiropracticServicesBase1.SelecttypeofbenefitforRoutineCare").ToString();
            string Other = _formData.SelectToken("HealthCareProfessionalServices.ChiropracticServices.ChiropracticServicesBase1.SelecttypeofbenefitforOtherService").ToString();
            if (RoutineCare == "2" || Other == "2")
                list.Add("7b");

            string RoutineFootCare = _formData.SelectToken("HealthCareProfessionalServices.PodiatryServices.PodiatryServicesBase1.SelecttypeofbenefitforRoutineFootCare").ToString();
            if (RoutineFootCare == "2")
                list.Add("7f");

            string OutpatientBloodServices = _formData.SelectToken("OutpatientServices.OutpatientBloodServices.OutpatientBloodServicesBase1.SelecttypeofbenefitforThree3PintDeductibleWaived").ToString();
            if (OutpatientBloodServices == "2")
                list.Add("9d");

            string MedicarePartBRxDrugsGeneral = _formData.SelectToken("MedicarePartBRxDrugsGeneral.HomeInfusionBundlesServices.DoestheplanprovidePartDhomeinfusiondrugsaspartofabundledserviceasamand").ToString();
            if (MedicarePartBRxDrugsGeneral == "1")
                list.Add("15");

            string TransportationServices = _formData.SelectToken("AmbulanceTransportationServices.TransportationServices.TransportationServicesBase1.SelecttypeofbenefitforPlanapprovedLocation").ToString();
            if (TransportationServices == "2")
                list.Add("10b");

            string Acupuncture = _formData.SelectToken("Acupuncture.AcupunctureBase1.SelecttypeofbenefitforNumberofTreatments").ToString();
            if (Acupuncture == "2")
                list.Add("13a");

            string OTCItems = _formData.SelectToken("OTCItems.OTCItemsBase1.SelecttypeofbenefitforOTCItems").ToString();
            if (OTCItems == "2")
                list.Add("13b");

            string MealBenefit = _formData.SelectToken("MealBenefit.MealBenefitBase1.SelecttypeofbenefitforMeals").ToString();
            if (MealBenefit == "2")
                list.Add("13c");

            string OtherOne = _formData.SelectToken("OtherOne.Other1Base1.SelecttypeofbenefitforOther1").ToString();
            if (OtherOne == "2")
                list.Add("13d");

            string OtherTwo = _formData.SelectToken("OtherTwo.Other2Base1.SelecttypeofbenefitforOther2").ToString();
            if (OtherTwo == "2")
                list.Add("13e");

            string OtherThree = _formData.SelectToken("OtherThree.Other3Base1.SelecttypeofbenefitforOther3").ToString();
            if (OtherThree == "2")
                list.Add("13f");

            string DualEligibleSNP = _formData.SelectToken("DualEligibleSNPswithHighlyIntegratedServices.DualEligibleSNPswithHighlyIntegratedServicesBase1.SelecttypeofbenefitforDualEligibleSNPswithHighlyIntegratedServices").ToString();
            if (DualEligibleSNP == "2")
                list.Add("13g");

            string AnnualPhysicalExam = _formData.SelectToken("AnnualPhysicalExam.AnnualPhysicalExamBase1.SelecttypeofbenefitfortheAnnualPhysicalExam").ToString();
            if (AnnualPhysicalExam == "2")
                list.Add("14b");
            if (rule.SuccessValue == "checkIfMandetoryForNonMedicare")
            {
                string EligibleSupplementalBenefits1 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforHealthEducation").ToString();
                string EligibleSupplementalBenefits2 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforNutritionalDietaryBenefit").ToString();
                string EligibleSupplementalBenefits3 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforAdditionalsessionsofSmokingandTobaccoCessationCo").ToString();
                string EligibleSupplementalBenefits4 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforFitnessBenefit").ToString();
                string EligibleSupplementalBenefits5 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforEnhancedDiseaseManagement").ToString();
                string EligibleSupplementalBenefits6 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforTelemonitoringServices").ToString();
                string EligibleSupplementalBenefits7 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforRemoteAccessTechnologiesincludingWebPhonebasedte").ToString();
                string EligibleSupplementalBenefits8 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforBathroomSafetyDevices").ToString();
                string EligibleSupplementalBenefits9 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforCounselingServices").ToString();
                string EligibleSupplementalBenefits10 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforInHomeSafetyAssessment").ToString();
                string EligibleSupplementalBenefits11 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforPersonalEmergencyResponseSystemPERS").ToString();
                string EligibleSupplementalBenefits12 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforMedicalNutritionTherapyMNT").ToString();
                string EligibleSupplementalBenefits13 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforPostdischargeInhomeMedicationReconciliation").ToString();
                string EligibleSupplementalBenefits14 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforReadmissionPrevention").ToString();
                string EligibleSupplementalBenefits15 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforWigsforHairLossRelatedtoChemotherapy").ToString();
                string EligibleSupplementalBenefits16 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforWeightManagementPrograms").ToString();
                string EligibleSupplementalBenefits17 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforAlternativeTherapies").ToString();

                if (EligibleSupplementalBenefits1 == "2" || EligibleSupplementalBenefits2 == "2" || EligibleSupplementalBenefits3 == "2" || EligibleSupplementalBenefits4 == "2" || EligibleSupplementalBenefits5 == "2" || EligibleSupplementalBenefits6 == "2" || EligibleSupplementalBenefits7 == "2" || EligibleSupplementalBenefits8 == "2" || EligibleSupplementalBenefits9 == "2" || EligibleSupplementalBenefits10 == "2" || EligibleSupplementalBenefits11 == "2" || EligibleSupplementalBenefits12 == "2" || EligibleSupplementalBenefits13 == "2" || EligibleSupplementalBenefits14 == "2" || EligibleSupplementalBenefits15 == "2" || EligibleSupplementalBenefits16 == "2" || EligibleSupplementalBenefits17 == "2")
                    list.Add("14c");
            }
            else
            {
                string EligibleSupplementalBenefits1 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforHealthEducation").ToString();
                if (EligibleSupplementalBenefits1 == "2")
                    list.Add("14c1");
                string EligibleSupplementalBenefits2 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforNutritionalDietaryBenefit").ToString();
                if (EligibleSupplementalBenefits2 == "2")
                    list.Add("14c2");
                string EligibleSupplementalBenefits3 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforAdditionalsessionsofSmokingandTobaccoCessationCo").ToString();
                if (EligibleSupplementalBenefits3 == "2")
                    list.Add("14c3");
                string EligibleSupplementalBenefits4 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforFitnessBenefit").ToString();
                if (EligibleSupplementalBenefits4 == "2")
                    list.Add("14c4");
                string EligibleSupplementalBenefits5 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforEnhancedDiseaseManagement").ToString();
                if (EligibleSupplementalBenefits5 == "2")
                    list.Add("14c5");
                string EligibleSupplementalBenefits6 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforTelemonitoringServices").ToString();
                if (EligibleSupplementalBenefits6 == "2")
                    list.Add("14c6");
                string EligibleSupplementalBenefits7 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforRemoteAccessTechnologiesincludingWebPhonebasedte").ToString();
                if (EligibleSupplementalBenefits7 == "2")
                    list.Add("14c7");
                string EligibleSupplementalBenefits8 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforBathroomSafetyDevices").ToString();
                if (EligibleSupplementalBenefits8 == "2")
                    list.Add("14c8");
                string EligibleSupplementalBenefits9 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforCounselingServices").ToString();
                if (EligibleSupplementalBenefits9 == "2")
                    list.Add("14c9");
                string EligibleSupplementalBenefits10 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforInHomeSafetyAssessment").ToString();
                if (EligibleSupplementalBenefits10 == "2")
                    list.Add("14c10");
                string EligibleSupplementalBenefits11 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforPersonalEmergencyResponseSystemPERS").ToString();
                if (EligibleSupplementalBenefits11 == "2")
                    list.Add("14c11");
                string EligibleSupplementalBenefits12 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforMedicalNutritionTherapyMNT").ToString();
                if (EligibleSupplementalBenefits12 == "2")
                    list.Add("14c12");
                string EligibleSupplementalBenefits13 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforPostdischargeInhomeMedicationReconciliation").ToString();
                if (EligibleSupplementalBenefits13 == "2")
                    list.Add("14c13");
                string EligibleSupplementalBenefits14 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforReadmissionPrevention").ToString();
                if (EligibleSupplementalBenefits14 == "2")
                    list.Add("14c14");
                string EligibleSupplementalBenefits15 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforWigsforHairLossRelatedtoChemotherapy").ToString();
                if (EligibleSupplementalBenefits15 == "2")
                    list.Add("14c15");
                string EligibleSupplementalBenefits16 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforWeightManagementPrograms").ToString();
                if (EligibleSupplementalBenefits16 == "2")
                    list.Add("14c16");
                string EligibleSupplementalBenefits17 = _formData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforAlternativeTherapies").ToString();
                if (EligibleSupplementalBenefits17 == "2")
                    list.Add("14c17");
            }
            string DentalPreventive1 = _formData.SelectToken("DentalPreventive.PreventiveDentalBase1.SelecttypeofbenefitforOralExams").ToString();
            string DentalPreventive2 = _formData.SelectToken("DentalPreventive.PreventiveDentalBase1.SelecttypeofbenefitforProphylaxisCleaning").ToString();
            string DentalPreventive3 = _formData.SelectToken("DentalPreventive.PreventiveDentalBase1.SelecttypeofbenefitforFluorideTreatment").ToString();
            string DentalPreventive4 = _formData.SelectToken("DentalPreventive.PreventiveDentalBase2.SelecttypeofbenefitforDentalXRays").ToString();
            if (DentalPreventive1 == "2" || DentalPreventive2 == "2" || DentalPreventive3 == "2" || DentalPreventive4 == "2")
                list.Add("16a");
            string ComprehensiveDental1 = _formData.SelectToken("ComprehensiveDental.ComprehensiveDentalBase1.SelecttypeofbenefitforNonroutineServices").ToString();
            string ComprehensiveDental2 = _formData.SelectToken("ComprehensiveDental.ComprehensiveDentalBase1.SelecttypeofbenefitforDiagnosticServices").ToString();
            string ComprehensiveDental3 = _formData.SelectToken("ComprehensiveDental.ComprehensiveDentalBase1.SelecttypeofbenefitforRestorativeServices").ToString();
            string ComprehensiveDental4 = _formData.SelectToken("ComprehensiveDental.ComprehensiveDentalBase2.SelecttypeofbenefitforEndodontics").ToString();
            string ComprehensiveDental5 = _formData.SelectToken("ComprehensiveDental.ComprehensiveDentalBase2.SelecttypeofbenefitforPeriodontics").ToString();
            string ComprehensiveDental6 = _formData.SelectToken("ComprehensiveDental.ComprehensiveDentalBase2.SelecttypeofbenefitforExtractions").ToString();
            if (ComprehensiveDental1 == "2" || ComprehensiveDental2 == "2" || ComprehensiveDental3 == "2" || ComprehensiveDental4 == "2" || ComprehensiveDental5 == "2" || ComprehensiveDental6 == "2")
                list.Add("16b");

            string EyeExams1 = _formData.SelectToken("EyeExams.EyeExamsBase1.SelecttypeofbenefitforRoutineEyeExams").ToString();
            string EyeExams2 = _formData.SelectToken("EyeExams.EyeExamsBase1.SelecttypeofbenefitforOtherService").ToString();
            if (EyeExams1 == "2" || EyeExams2 == "2")
                list.Add("17a");

            string Eyewear1 = _formData.SelectToken("Eyewear.EyewearBase1.SelecttypeofbenefitforContactlenses").ToString();
            string Eyewear2 = _formData.SelectToken("Eyewear.EyewearBase1.SelecttypeofbenefitforEyeglasseslensesandframes").ToString();
            string Eyewear3 = _formData.SelectToken("Eyewear.EyewearBase2.SelecttypeofbenefitforEyeglasslenses").ToString();
            string Eyewear4 = _formData.SelectToken("Eyewear.EyewearBase2.SelecttypeofbenefitforEyeglassframes").ToString();
            string Eyewear5 = _formData.SelectToken("Eyewear.EyewearBase2.SelecttypeofbenefitforUpgrades").ToString();
            if (Eyewear1 == "2" || Eyewear2 == "2" || Eyewear3 == "2" || Eyewear4 == "2" || Eyewear5 == "2")
                list.Add("17b");

            string HearingExams1 = _formData.SelectToken("HearingExams.HearingExamsBase1.SelecttypeofbenefitforFittingEvaluationforHearingAid").ToString();
            string HearingExams2 = _formData.SelectToken("HearingExams.HearingExamsBase1.SelecttypeofbenefitforRoutineHearingExams").ToString();
            if (HearingExams1 == "2" || HearingExams2 == "2")
                list.Add("18a");

            string HearingAids1 = _formData.SelectToken("HearingAids.HearingAidsBase1.SelecttypeofbenefitforHearingAidsalltypes").ToString();
            string HearingAids2 = _formData.SelectToken("HearingAids.HearingAidsBase1.SelecttypeofbenefitforHearingAidsInnerEar").ToString();
            string HearingAids3 = _formData.SelectToken("HearingAids.HearingAidsBase1.SelecttypeofbenefitforHearingAidsOuterEar").ToString();
            string HearingAids4 = _formData.SelectToken("HearingAids.HearingAidsBase2.SelecttypeofbenefitforHearingAidsOvertheEar").ToString();
            if (HearingAids1 == "2" || HearingAids2 == "2" || HearingAids3 == "2" || HearingAids4 == "2")
                list.Add("18b");

            string OPDrupg = _formData.SelectToken("OutpatientDrugs.OutpatientDrugsBase1.Selecttypeofbenefit").ToString();
            if (OPDrupg == rightOperand)
                list.Add("20");

            foreach (string service in list)
            {
                if (!services.Contains(service))
                {
                    return false;
                }
            }
            foreach (string service in services)
            {
                if (!list.Contains(service))
                {
                    return false;
                }

            }
            return true;
        }

        private bool slidingCostShareDeductibleCheck(string leftOperand, string rightOperand, JObject _rowData)
        {
            try
            {
                //if (decimal.TryParse(targetValue, out n1) && decimal.TryParse(maxValue, out n2))
                decimal n1, n2;
                decimal.TryParse(leftOperand, out n1); decimal.TryParse(rightOperand, out n2);
                //bool result = false;
                int isAcuteCoinseTier1, isAcuteCoinseTier2, isAcuteCoinseTier3 = 0;
                isAcuteCoinseTier1 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase2.MedicarecoveredCoinsuranceCostSharingforTier1.DoyouchargetheMedicaredefinedcostsharesThesearethetotalchargesforallse") ?? "") != String.Empty ?
                                    Convert.ToInt32(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase2.MedicarecoveredCoinsuranceCostSharingforTier1.DoyouchargetheMedicaredefinedcostsharesThesearethetotalchargesforallse")) : 0;
                isAcuteCoinseTier2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase3.MedicarecoveredCoinsuranceCostSharingforTier2.DoyouchargetheMedicaredefinedcostsharesforTier2Thesearethetotalcharges") ?? "") != String.Empty ?
                                    Convert.ToInt32(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase3.MedicarecoveredCoinsuranceCostSharingforTier2.DoyouchargetheMedicaredefinedcostsharesforTier2Thesearethetotalcharges")) : 0;
                isAcuteCoinseTier3 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase3.MedicarecoveredCoinsuranceCostSharingforTier3.DoyouchargetheMedicaredefinedcostsharesforTier3Thesearethetotalcharges") ?? "") != String.Empty ?
                                    Convert.ToInt32(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase3.MedicarecoveredCoinsuranceCostSharingforTier3.DoyouchargetheMedicaredefinedcostsharesforTier3Thesearethetotalcharges")) : 0;

                if (isAcuteCoinseTier1 == 1 || isAcuteCoinseTier2 == 1 || isAcuteCoinseTier3 == 1)
                {
                    if (n1 >= n2)
                    {
                        return true;
                    }
                    return false;
                }
                int isAcuteCopayTier1, isAcuteCopayTier2, isAcuteCopayTier3 = 0;
                isAcuteCopayTier1 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.DoyouchargetheMedicaredefinedcostsharesThesearethetotalchargesforallse") ?? "") != String.Empty ?
                    Convert.ToInt32(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.DoyouchargetheMedicaredefinedcostsharesThesearethetotalchargesforallse")) : 0;
                isAcuteCopayTier2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.DoyouchargetheMedicaredefinedcostsharesforTier2Thesearethetotalcharges") ?? "") != String.Empty ?
                    Convert.ToInt32(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.DoyouchargetheMedicaredefinedcostsharesforTier2Thesearethetotalcharges")) : 0;
                isAcuteCopayTier3 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.DoyouchargetheMedicaredefinedcostsharesforTier3Thesearethetotalcharges") ?? "") != String.Empty ?
                    Convert.ToInt32(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.DoyouchargetheMedicaredefinedcostsharesforTier3Thesearethetotalcharges")) : 0;

                if (isAcuteCopayTier1 == 1 || isAcuteCopayTier2 == 1 || isAcuteCopayTier3 == 1)
                {
                    if (n1 >= n2)
                    {
                        return true;
                    }
                    return false;
                }
                int isPsychCoinseTier1, isPsychCoinseTier2, isPsychCoinseTier3 = 0;
                isPsychCoinseTier1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase2.MedicarecoveredCoinsuranceCostSharingforTier1.DoyouchargetheMedicaredefinedcostsharesThesearethetotalchargesforallse") ?? "") != String.Empty ?
                   Convert.ToInt32(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase2.MedicarecoveredCoinsuranceCostSharingforTier1.DoyouchargetheMedicaredefinedcostsharesThesearethetotalchargesforallse")) : 0;
                isPsychCoinseTier2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase3.MedicarecoveredCoinsuranceCostSharingforTier2.DoyouchargetheMedicaredefinedcostsharesforTier2Thesearethetotalcharges") ?? "") != String.Empty ?
                   Convert.ToInt32(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase3.MedicarecoveredCoinsuranceCostSharingforTier2.DoyouchargetheMedicaredefinedcostsharesforTier2Thesearethetotalcharges")) : 0;
                isPsychCoinseTier3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase3.MedicarecoveredCoinsuranceCostSharingforTier3.DoyouchargetheMedicaredefinedcostsharesforTier3Thesearethetotalcharges") ?? "") != String.Empty ?
                   Convert.ToInt32(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase3.MedicarecoveredCoinsuranceCostSharingforTier3.DoyouchargetheMedicaredefinedcostsharesforTier3Thesearethetotalcharges")) : 0;

                if (isPsychCoinseTier1 == 1 || isPsychCoinseTier2 == 1 || isPsychCoinseTier3 == 1)
                {
                    if (n1 >= n2)
                    {
                        return true;
                    }
                    return false;
                }
                int isPsychCopayTier1, isPsychCopayTier2, isPsychCopayTier3 = 0;

                isPsychCopayTier1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.MedicarecoveredCopaymentCostSharingforTier1.DoyouchargetheMedicaredefinedcostsharesThesearethetotalchargesforallse") ?? "") != String.Empty ?
                   Convert.ToInt32(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.MedicarecoveredCopaymentCostSharingforTier1.DoyouchargetheMedicaredefinedcostsharesThesearethetotalchargesforallse")) : 0;
                isPsychCopayTier2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier2.DoyouchargetheMedicaredefinedcostsharesThesearethetotalchargesforallse") ?? "") != String.Empty ?
                   Convert.ToInt32(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier2.DoyouchargetheMedicaredefinedcostsharesThesearethetotalchargesforallse")) : 0;
                isPsychCopayTier3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.DoyouchargetheMedicaredefinedcostsharesforTier3Thesearethetotalcharges") ?? "") != String.Empty ?
                   Convert.ToInt32(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.DoyouchargetheMedicaredefinedcostsharesforTier3Thesearethetotalcharges")) : 0;

                if (isPsychCopayTier1 == 1 || isPsychCopayTier2 == 1 || isPsychCopayTier3 == 1)
                {
                    if (n1 >= n2)
                    {
                        return true;
                    }
                    return false;
                }
                int isSNFCoinseTier1, isSNFCoinseTier2, isSNFCoinseTier3 = 0;
                isSNFCoinseTier1 = Convert.ToString(_rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase2.MedicareCoveredCoinsuranceCostSharingforTier1.DoyouchargetheMedicaredefinedcostsharesThesearethetotalchargesforallse") ?? "") != String.Empty ?
                  Convert.ToInt32(_rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase2.MedicareCoveredCoinsuranceCostSharingforTier1.DoyouchargetheMedicaredefinedcostsharesThesearethetotalchargesforallse")) : 0;
                isSNFCoinseTier2 = Convert.ToString(_rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase3.MedicareCoveredCoinsuranceCostSharingforTier2.DoyouchargetheMedicaredefinedcostsharesforTier2Thesearethetotalcharges") ?? "") != String.Empty ?
                  Convert.ToInt32(_rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase3.MedicareCoveredCoinsuranceCostSharingforTier2.DoyouchargetheMedicaredefinedcostsharesforTier2Thesearethetotalcharges")) : 0;
                isSNFCoinseTier3 = Convert.ToString(_rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase3.MedicareCoveredCoinsuranceCostSharingforTier3.DoyouchargetheMedicaredefinedcostsharesforTier3Thesearethetotalcharges") ?? "") != String.Empty ?
                  Convert.ToInt32(_rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase3.MedicareCoveredCoinsuranceCostSharingforTier3.DoyouchargetheMedicaredefinedcostsharesforTier3Thesearethetotalcharges")) : 0;

                if (isSNFCoinseTier1 == 1 || isSNFCoinseTier2 == 1 || isSNFCoinseTier3 == 1)
                {
                    if (n1 >= n2)
                    {
                        return true;
                    }
                    return false;
                }
                int isSNFCopayTier1, isSNFCopayTier2, isSNFCopayTier3 = 0;
                isSNFCopayTier1 = Convert.ToString(_rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase6.MedicarecoveredCopaymentCostSharingforTier1.DoyouchargetheMedicaredefinedcostsharesThesearethetotalchargesforallse") ?? "") != String.Empty ?
                  Convert.ToInt32(_rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase6.MedicarecoveredCopaymentCostSharingforTier1.DoyouchargetheMedicaredefinedcostsharesThesearethetotalchargesforallse")) : 0;
                isSNFCopayTier2 = Convert.ToString(_rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase7.MedicarecoveredCopaymentCostSharingforTier2.DoyouchargetheMedicaredefinedcostsharesforTier2Thesearethetotalcharges") ?? "") != String.Empty ?
                  Convert.ToInt32(_rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase7.MedicarecoveredCopaymentCostSharingforTier2.DoyouchargetheMedicaredefinedcostsharesforTier2Thesearethetotalcharges")) : 0;
                isSNFCopayTier3 = Convert.ToString(_rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase7.MedicarecoveredCopaymentCostSharingforTier3.DoyouchargetheMedicaredefinedcostsharesforTier3Thesearethetotalcharges") ?? "") != String.Empty ?
                  Convert.ToInt32(_rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase7.MedicarecoveredCopaymentCostSharingforTier3.DoyouchargetheMedicaredefinedcostsharesforTier3Thesearethetotalcharges")) : 0;

                if (isSNFCopayTier1 == 1 || isSNFCopayTier2 == 1 || isSNFCopayTier3 == 1)
                {
                    if (n1 >= n2)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch
            {

            }
            return true;
        }

        private bool checkIfOptionalBenefit(string leftOperand, string rightOperand, JObject _rowData)
        {
            string sOTServices = leftOperand.Replace("\r\n", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty).Replace("\"", string.Empty).Replace(" ", string.Empty);
            string[] Sservices = sOTServices.Split(',');
            List<string> services = new List<string>() { "10a", "11a", "11b", "11c", "12", "14a", "14d", "14e", "15", "5", "6", "7a", "7c", "7d", "7e", "7g", "7h", "7i", "8a", "8b", "9a", "9b", "9c", "4a", "4b" };
            foreach (string field in Sservices)
            {
                if (field != "")
                    services.Add(field);
            }
            List<string> list = new List<string>() { "10a", "11a", "11b", "11c", "12", "14a", "14d", "14e", "15", "5", "6", "7a", "7c", "7d", "7e", "7g", "7h", "7i", "8a", "8b", "9a", "9b", "9c", "4a", "4b" };

            string InpatientAcute1 = _rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase1.SelecttypeofbenefitforAdditionalDays").ToString();
            string InpatientAcute2 = _rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase1.SelecttypeofbenefitforNonMedicarecoveredstay").ToString();
            string InpatientAcute3 = _rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase1.SelecttypeofbenefitforUpgrades").ToString();
            if (InpatientAcute1 == rightOperand || InpatientAcute2 == rightOperand || InpatientAcute3 == rightOperand)
                list.Add("1a");
            string InpatientPsychiatric1 = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase1.SelecttypeofbenefitforAdditionalDays").ToString();
            string InpatientPsychiatric2 = _rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase1.SelecttypeofbenefitforNonMedicarecoveredstay").ToString();
            if (InpatientPsychiatric1 == rightOperand || InpatientPsychiatric2 == rightOperand)
                list.Add("1b");

            string SNF1 = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase1.SelecttypeofbenefitforAdditionalDaysbeyondMedicarecovered").ToString();
            string SNF2 = _rowData.SelectToken("SkilledNursingFacilitySNF.SNFBase1.SelecttypeofbenefitfortheNonMedicarecoveredstay").ToString();
            if (SNF1 == rightOperand || SNF2 == rightOperand)
                list.Add("2");

            string CardiacandPulmonaryRehabilitation = _rowData.SelectToken("CardiacandPulmonaryRehabilitationServices.CardiacandPulmonaryRehabilitationServicesBase1.SelecttypeofbenefitforAdditionalCardiacRehabilitationServices").ToString();
            if (CardiacandPulmonaryRehabilitation == rightOperand)
                list.Add("3-1");

            string IntensiveCardiacRehabilitation = _rowData.SelectToken("CardiacandPulmonaryRehabilitationServices.CardiacandPulmonaryRehabilitationServicesBase1.SelecttypeofbenefitforAdditionalIntensiveCardiacRehabilitationServices").ToString();
            if (IntensiveCardiacRehabilitation == rightOperand)
                list.Add("3-2");

            string AdditionalPulmonaryRehabilitation = _rowData.SelectToken("CardiacandPulmonaryRehabilitationServices.CardiacandPulmonaryRehabilitationServicesBase1.SelecttypeofbenefitforAdditionalPulmonaryRehabilitationServices").ToString();
            if (AdditionalPulmonaryRehabilitation == rightOperand)
                list.Add("3-3");

            string WWEmergancyCov = _rowData.SelectToken("WorldwideEmergencyUrgentCoverage.WorldwideEmergencyUrgentCoverageBase1.SelecttypeofbenefitforWorldwideEmergencyCoverage").ToString();
            string WurgentCoverage = _rowData.SelectToken("WorldwideEmergencyUrgentCoverage.WorldwideEmergencyUrgentCoverageBase1.SelecttypeofbenefitforWorldwideUrgentCoverage").ToString();
            string WemergencyCoveage = _rowData.SelectToken("WorldwideEmergencyUrgentCoverage.WorldwideEmergencyUrgentCoverageBase1.SelecttypeofbenefitforWorldwideEmergencyTransportation").ToString();
            if (WWEmergancyCov == rightOperand || WurgentCoverage == rightOperand || WemergencyCoveage == rightOperand)
                list.Add("4c");

            string RoutineCare = _rowData.SelectToken("HealthCareProfessionalServices.ChiropracticServices.ChiropracticServicesBase1.SelecttypeofbenefitforRoutineCare").ToString();
            string Other = _rowData.SelectToken("HealthCareProfessionalServices.ChiropracticServices.ChiropracticServicesBase1.SelecttypeofbenefitforOtherService").ToString();
            if (RoutineCare == rightOperand || Other == rightOperand)
                list.Add("7b");

            string RoutineFootCare = _rowData.SelectToken("HealthCareProfessionalServices.PodiatryServices.PodiatryServicesBase1.SelecttypeofbenefitforRoutineFootCare").ToString();
            if (RoutineFootCare == rightOperand)
                list.Add("7f");

            string OutpatientBloodServices = _rowData.SelectToken("OutpatientServices.OutpatientBloodServices.OutpatientBloodServicesBase1.SelecttypeofbenefitforThree3PintDeductibleWaived").ToString();
            if (OutpatientBloodServices == rightOperand)
                list.Add("9d");

            string ComprehensiveDental1 = _rowData.SelectToken("ComprehensiveDental.ComprehensiveDentalBase1.SelecttypeofbenefitforNonroutineServices").ToString();
            string ComprehensiveDental2 = _rowData.SelectToken("ComprehensiveDental.ComprehensiveDentalBase1.SelecttypeofbenefitforDiagnosticServices").ToString();
            string ComprehensiveDental3 = _rowData.SelectToken("ComprehensiveDental.ComprehensiveDentalBase1.SelecttypeofbenefitforRestorativeServices").ToString();
            string ComprehensiveDental4 = _rowData.SelectToken("ComprehensiveDental.ComprehensiveDentalBase2.SelecttypeofbenefitforEndodontics").ToString();
            string ComprehensiveDental5 = _rowData.SelectToken("ComprehensiveDental.ComprehensiveDentalBase2.SelecttypeofbenefitforPeriodontics").ToString();
            string ComprehensiveDental6 = _rowData.SelectToken("ComprehensiveDental.ComprehensiveDentalBase2.SelecttypeofbenefitforExtractions").ToString();
            if (ComprehensiveDental1 == rightOperand || ComprehensiveDental2 == rightOperand || ComprehensiveDental3 == rightOperand || ComprehensiveDental4 == rightOperand || ComprehensiveDental5 == rightOperand || ComprehensiveDental6 == rightOperand)
                list.Add("16b");

            string EyeExams1 = _rowData.SelectToken("EyeExams.EyeExamsBase1.SelecttypeofbenefitforRoutineEyeExams").ToString();
            string EyeExams2 = _rowData.SelectToken("EyeExams.EyeExamsBase1.SelecttypeofbenefitforOtherService").ToString();
            if (EyeExams1 == rightOperand || EyeExams2 == rightOperand)
                list.Add("17a");

            string Eyewear1 = _rowData.SelectToken("Eyewear.EyewearBase1.SelecttypeofbenefitforContactlenses").ToString();
            string Eyewear2 = _rowData.SelectToken("Eyewear.EyewearBase1.SelecttypeofbenefitforEyeglasseslensesandframes").ToString();
            string Eyewear3 = _rowData.SelectToken("Eyewear.EyewearBase2.SelecttypeofbenefitforEyeglasslenses").ToString();
            string Eyewear4 = _rowData.SelectToken("Eyewear.EyewearBase2.SelecttypeofbenefitforEyeglassframes").ToString();
            string Eyewear5 = _rowData.SelectToken("Eyewear.EyewearBase2.SelecttypeofbenefitforUpgrades").ToString();
            if (Eyewear1 == rightOperand || Eyewear2 == rightOperand || Eyewear3 == rightOperand || Eyewear4 == rightOperand || Eyewear5 == rightOperand)
                list.Add("17b");

            string HearingExams1 = _rowData.SelectToken("HearingExams.HearingExamsBase1.SelecttypeofbenefitforFittingEvaluationforHearingAid").ToString();
            string HearingExams2 = _rowData.SelectToken("HearingExams.HearingExamsBase1.SelecttypeofbenefitforRoutineHearingExams").ToString();
            if (HearingExams1 == rightOperand || HearingExams2 == rightOperand)
                list.Add("18a");
            string TransportationServices = _rowData.SelectToken("AmbulanceTransportationServices.TransportationServices.TransportationServicesBase1.SelecttypeofbenefitforPlanapprovedLocation").ToString();
            if (TransportationServices == rightOperand)
                list.Add("10b");

            string Acupuncture = _rowData.SelectToken("Acupuncture.AcupunctureBase1.SelecttypeofbenefitforNumberofTreatments").ToString();
            if (Acupuncture == rightOperand)
                list.Add("13a");

            string OTCItems = _rowData.SelectToken("OTCItems.OTCItemsBase1.SelecttypeofbenefitforOTCItems").ToString();
            if (OTCItems == rightOperand)
                list.Add("13b");

            string MealBenefit = _rowData.SelectToken("MealBenefit.MealBenefitBase1.SelecttypeofbenefitforMeals").ToString();
            if (MealBenefit == rightOperand)
                list.Add("13c");

            string OtherOne = _rowData.SelectToken("OtherOne.Other1Base1.SelecttypeofbenefitforOther1").ToString();
            if (OtherOne == rightOperand)
                list.Add("13d");

            string OtherTwo = _rowData.SelectToken("OtherTwo.Other2Base1.SelecttypeofbenefitforOther2").ToString();
            if (OtherTwo == rightOperand)
                list.Add("13e");

            string OtherThree = _rowData.SelectToken("OtherThree.Other3Base1.SelecttypeofbenefitforOther3").ToString();
            if (OtherThree == rightOperand)
                list.Add("13f");

            string DualEligibleSNP = _rowData.SelectToken("DualEligibleSNPswithHighlyIntegratedServices.DualEligibleSNPswithHighlyIntegratedServicesBase1.SelecttypeofbenefitforDualEligibleSNPswithHighlyIntegratedServices").ToString();
            if (DualEligibleSNP == rightOperand)
                list.Add("13g");

            string AnnualPhysicalExam = _rowData.SelectToken("AnnualPhysicalExam.AnnualPhysicalExamBase1.SelecttypeofbenefitfortheAnnualPhysicalExam").ToString();
            if (AnnualPhysicalExam == rightOperand)
                list.Add("14b");

            string EligibleSupplementalBenefits1 = _rowData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforHealthEducation").ToString();
            string EligibleSupplementalBenefits2 = _rowData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforNutritionalDietaryBenefit").ToString();
            string EligibleSupplementalBenefits3 = _rowData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforAdditionalsessionsofSmokingandTobaccoCessationCo").ToString();
            string EligibleSupplementalBenefits4 = _rowData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforFitnessBenefit").ToString();
            string EligibleSupplementalBenefits5 = _rowData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforEnhancedDiseaseManagement").ToString();
            string EligibleSupplementalBenefits6 = _rowData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforTelemonitoringServices").ToString();
            string EligibleSupplementalBenefits7 = _rowData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforRemoteAccessTechnologiesincludingWebPhonebasedte").ToString();
            string EligibleSupplementalBenefits8 = _rowData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforBathroomSafetyDevices").ToString();
            string EligibleSupplementalBenefits9 = _rowData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforCounselingServices").ToString();
            string EligibleSupplementalBenefits10 = _rowData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base1.SelecttypeofbenefitforInHomeSafetyAssessment").ToString();
            string EligibleSupplementalBenefits11 = _rowData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforPersonalEmergencyResponseSystemPERS").ToString();
            string EligibleSupplementalBenefits12 = _rowData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforMedicalNutritionTherapyMNT").ToString();
            string EligibleSupplementalBenefits13 = _rowData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforPostdischargeInhomeMedicationReconciliation").ToString();
            string EligibleSupplementalBenefits14 = _rowData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforReadmissionPrevention").ToString();
            string EligibleSupplementalBenefits15 = _rowData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforWigsforHairLossRelatedtoChemotherapy").ToString();
            string EligibleSupplementalBenefits16 = _rowData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforWeightManagementPrograms").ToString();
            string EligibleSupplementalBenefits17 = _rowData.SelectToken("EligibleSupplementalBenefitsasDefinedinChapterFour.EligibleSupplementalBenefitsasDefinedinChapter4Base2.SelecttypeofbenefitforAlternativeTherapies").ToString();

            if (EligibleSupplementalBenefits1 == rightOperand || EligibleSupplementalBenefits2 == rightOperand || EligibleSupplementalBenefits3 == rightOperand || EligibleSupplementalBenefits4 == rightOperand || EligibleSupplementalBenefits5 == rightOperand || EligibleSupplementalBenefits6 == rightOperand || EligibleSupplementalBenefits7 == rightOperand || EligibleSupplementalBenefits8 == rightOperand || EligibleSupplementalBenefits9 == rightOperand || EligibleSupplementalBenefits10 == rightOperand || EligibleSupplementalBenefits11 == rightOperand || EligibleSupplementalBenefits12 == rightOperand || EligibleSupplementalBenefits13 == rightOperand || EligibleSupplementalBenefits14 == rightOperand || EligibleSupplementalBenefits15 == rightOperand || EligibleSupplementalBenefits16 == rightOperand || EligibleSupplementalBenefits17 == rightOperand)
                list.Add("14c");

            string DentalPreventive1 = _rowData.SelectToken("DentalPreventive.PreventiveDentalBase1.SelecttypeofbenefitforOralExams").ToString();
            string DentalPreventive2 = _rowData.SelectToken("DentalPreventive.PreventiveDentalBase1.SelecttypeofbenefitforProphylaxisCleaning").ToString();
            string DentalPreventive3 = _rowData.SelectToken("DentalPreventive.PreventiveDentalBase1.SelecttypeofbenefitforFluorideTreatment").ToString();
            string DentalPreventive4 = _rowData.SelectToken("DentalPreventive.PreventiveDentalBase2.SelecttypeofbenefitforDentalXRays").ToString();
            if (DentalPreventive1 == rightOperand || DentalPreventive2 == rightOperand || DentalPreventive3 == rightOperand || DentalPreventive4 == rightOperand)
                list.Add("16a");

            string HearingAids1 = _rowData.SelectToken("HearingAids.HearingAidsBase1.SelecttypeofbenefitforHearingAidsalltypes").ToString();
            string HearingAids2 = _rowData.SelectToken("HearingAids.HearingAidsBase1.SelecttypeofbenefitforHearingAidsInnerEar").ToString();
            string HearingAids3 = _rowData.SelectToken("HearingAids.HearingAidsBase1.SelecttypeofbenefitforHearingAidsOuterEar").ToString();
            string HearingAids4 = _rowData.SelectToken("HearingAids.HearingAidsBase2.SelecttypeofbenefitforHearingAidsOvertheEar").ToString();
            if (HearingAids1 == rightOperand || HearingAids2 == rightOperand || HearingAids3 == rightOperand || HearingAids4 == rightOperand)
                list.Add("18b");
            string OPDrupg = _rowData.SelectToken("OutpatientDrugs.OutpatientDrugsBase1.Selecttypeofbenefit").ToString();
            if (OPDrupg == rightOperand)
                list.Add("20");

            string Pos = _rowData.SelectToken("POSGeneral.POSGeneralBase1.SelecttypeofbenefitforthePOSoption").ToString();
            if (Pos == rightOperand)
                list.Add("POS");
            string VT = _rowData.SelectToken("VT.SelecttypeofbenefitfortheUSVisitorTravelprogram").ToString();
            if (VT == rightOperand)
                list.Add("VT");

            foreach (string service in list)
            {
                if (!services.Contains(service))
                {
                    return false;
                }
            }
            foreach (string service in services)
            {
                if (!list.Contains(service))
                {
                    return false;
                }

            }
            return true;
        }
        private bool repeaterPreICLTierTypeCostShareStructure(string leftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            //Pre ICL - Tier Type and Cost Share Structure
            string targetValue, tierID, isDrugBenefitSelected, tierLabelDescriptions;
            targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);


            tierID = Convert.ToString(_rowData.SelectToken("TierID") ?? string.Empty);
            var tierTypeInfo = _formData.SelectToken("PreICL.PreICLGeneral").Where(x => Convert.ToString(x["TierID"] ?? string.Empty) == tierID).FirstOrDefault();
            isDrugBenefitSelected = tierTypeInfo != null ? Convert.ToString(tierTypeInfo.SelectToken("Selectthetypeofdrugbenefit")) : string.Empty;
            tierLabelDescriptions = tierTypeInfo != null ? Convert.ToString(tierTypeInfo.SelectToken("TierLabelDescriptions")) : string.Empty;
            if (isDrugBenefitSelected != "")
            {
                if (targetValue != "")
                {
                    if (tierLabelDescriptions.Contains("Brand") == true && targetValue.Contains("10"))
                    {
                        return true;
                    }
                    else if (targetValue != "")
                    {
                        return true;
                    }
                    else
                        return false;
                }
                return false;
            }

            return true;
        }

        private bool repeaterTypeOfCostSharingStructure(string leftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            string targetValue;
            targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);
            string costsharingbeforetheInitialCoverageLimit = _rowData.Root.SelectToken("BasicEnhancedAlternative.AlternativePreICL.HowdoyouapplyyourcostsharingbeforetheInitialCoverageLimitICLisreached").ToString();

            if (costsharingbeforetheInitialCoverageLimit == "1")
            {
                if (targetValue == "2")
                    return true;
                else
                    return false;
            }
            else
            if (costsharingbeforetheInitialCoverageLimit == "2")
            {
                if (targetValue == "1")
                    return true;
                else
                    return false;
            }
            return true;
        }
        private bool repeaterStdRetailcostsharingMonthlySupply(string leftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            string targetValue, tierID, STDretailLocationSupply, PrefRetaillocationsupply;
            decimal n1;
            targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);
            tierID = Convert.ToString(_rowData.SelectToken("TierID") ?? string.Empty);
            var tierTypeInfo = _formData.SelectToken("PreICL.PreICLTierLocations").Where(x => Convert.ToString(x["TierID"] ?? string.Empty) == tierID).FirstOrDefault();
            STDretailLocationSupply = tierTypeInfo != null ? Convert.ToString(tierTypeInfo.SelectToken("SelectallStandardRetailCostSharingLocationsupplyamountsthatapplyforthi")) : string.Empty;
            PrefRetaillocationsupply = tierTypeInfo != null ? Convert.ToString(tierTypeInfo.SelectToken("SelectallStandardRetailPreferredRetailCostSharingPharmacyLocationsuppl")) : string.Empty;
            decimal.TryParse(targetValue, out n1);
            if (STDretailLocationSupply.Contains("010") && rightOperand == "One")
            {
                if (targetValue != "")
                {
                    if (n1 >= 30 && n1 <= 34)
                    {
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;

            }
            if (STDretailLocationSupply.Contains("001") && rightOperand == "Two")
            {
                if (targetValue != "")
                {
                    if (n1 >= 60 && n1 <= 66)
                    {
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;

            }

            if (STDretailLocationSupply.Contains("100") && rightOperand == "Three")
            {
                if (targetValue != "")
                {
                    if (n1 >= 90 && n1 <= 102)
                    {
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            if (PrefRetaillocationsupply.Contains("010") && rightOperand == "OnePR")
            {
                if (targetValue != "")
                    return true;
                else
                    return false;

            }
            if (PrefRetaillocationsupply.Contains("001") && rightOperand == "TwoPR")
            {
                if (targetValue != "")
                    return true;
                else
                    return false;
            }

            if (PrefRetaillocationsupply.Contains("100") && rightOperand == "ThreePR")
            {
                if (targetValue != "")
                    return true;
                else
                    return false;
            }
            return true;
        }
        private bool repeaterMailOrderCostSharingMonthlySupply(string leftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            string targetValue, tierID, mailOrderCSMonthly, mailPRCSMonthly;
            decimal n1;
            targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);
            tierID = Convert.ToString(_rowData.SelectToken("TierID") ?? string.Empty);
            var tierTypeInfo = _formData.SelectToken("PreICL.PreICLTierLocations").Where(x => Convert.ToString(x["TierID"] ?? string.Empty) == tierID).FirstOrDefault();
            mailOrderCSMonthly = tierTypeInfo != null ? Convert.ToString(tierTypeInfo.SelectToken("SelectallStandardMailOrderCostSharingLocationsupplyamountsthatapplyfor")) : string.Empty;
            mailPRCSMonthly = tierTypeInfo != null ? Convert.ToString(tierTypeInfo.SelectToken("SelectallMailOrderStandardPreferredLocationsupplyamountsthatapplyforth")) : string.Empty;
            string costsharingbeforetheInitialCoverageLimit = _rowData.Root.SelectToken("BasicEnhancedAlternative.AlternativePreICL.HowdoyouapplyyourcostsharingbeforetheInitialCoverageLimitICLisreached").ToString();
            decimal.TryParse(targetValue, out n1);
            if (mailOrderCSMonthly.Contains("010") && rightOperand == "One")
            {
                if (targetValue != "")
                {
                    if (n1 >= 30 && n1 <= 34)
                    {
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;

            }
            if (mailOrderCSMonthly.Contains("001") && rightOperand == "Two")
            {
                if (targetValue != "")
                {
                    if (n1 >= 60 && n1 <= 66)
                    {
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;

            }

            if (mailOrderCSMonthly.Contains("100") && rightOperand == "Three")
            {
                if (targetValue != "")
                {
                    if (n1 >= 90 && n1 <= 102)
                    {
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            if (mailPRCSMonthly.Contains("010") && rightOperand == "OnePR")
            {
                if (targetValue != "")
                    return true;
                else
                    return false;

            }
            if (mailPRCSMonthly.Contains("001") && rightOperand == "TwoPR")
            {
                if (targetValue != "")
                    return true;
                else
                    return false;
            }

            if (mailPRCSMonthly.Contains("100") && rightOperand == "ThreePR")
            {
                if (targetValue != "")
                    return true;
                else
                    return false;
            }
            return true;


        }
        private bool rXPreICLIsRequired(string LeftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            string targetValue, tierID, twoMonthSupply, threeMonthSupply, outOfNetworkSupply, longTermCare;
            targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);
            tierID = Convert.ToString(_rowData.SelectToken("TierID") ?? string.Empty);
            var tierTypeInfo = _formData.SelectToken("PreICL.PreICLLocationSupply").Where(x => Convert.ToString(x["TierID"] ?? string.Empty) == tierID).FirstOrDefault();
            twoMonthSupply = tierTypeInfo != null ? Convert.ToString(tierTypeInfo.SelectToken("EnternumberofdaysforStandardRetailCostSharinginyourtwomonthsupply")) : string.Empty;
            threeMonthSupply = tierTypeInfo != null ? Convert.ToString(tierTypeInfo.SelectToken("EnternumberofdaysforStandardRetailCostSharinginyourthreemonthsupply")) : string.Empty;
            if ((twoMonthSupply != "" || threeMonthSupply != "") && rightOperand == "")
            {
                if (targetValue != "")
                    return true;
                else
                    return false;

            }
            var tierTypeInfoOON = _formData.SelectToken("PreICL.PreICLTierLocations").Where(x => Convert.ToString(x["TierID"] ?? string.Empty) == tierID).FirstOrDefault();
            outOfNetworkSupply = tierTypeInfoOON != null ? Convert.ToString(tierTypeInfoOON.SelectToken("SelectallOutofNetworkPharmacyLocationsupplyamountsthatapplyforthisTier")) : string.Empty;
            if (rightOperand == "OON1" && outOfNetworkSupply.Contains("01"))
            {
                if (targetValue != "")
                    return true;
                else
                    return false;
            }
            if (rightOperand == "OON2" && outOfNetworkSupply.Contains("10"))
            {
                if (targetValue != "")
                    return true;
                else
                    return false;
            }
            longTermCare = tierTypeInfoOON != null ? Convert.ToString(tierTypeInfoOON.SelectToken("SelectallLongTermCarePharmacyLocationsupplyamountsthatapplyforthisTier")) : string.Empty;
            if (rightOperand == "Long" && outOfNetworkSupply.Contains("1"))
            {
                if (targetValue != "")
                    return true;
                else
                    return false;
            }
            return true;
        }
        private bool repeaterPreICLStandardRetailCostSharing(string LeftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            string targetValue, tierID, STDretailLocationSupply, PrefRetaillocationsupply;
            decimal n1;

            targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);
            tierID = Convert.ToString(_rowData.SelectToken("TierID") ?? string.Empty);
            var tierTypeInfo = _formData.SelectToken("PreICL.PreICLTierLocations").Where(x => Convert.ToString(x["TierID"] ?? string.Empty) == tierID).FirstOrDefault();
            STDretailLocationSupply = tierTypeInfo != null ? Convert.ToString(tierTypeInfo.SelectToken("SelectallStandardRetailCostSharingLocationsupplyamountsthatapplyforthi")) : string.Empty;
            PrefRetaillocationsupply = tierTypeInfo != null ? Convert.ToString(tierTypeInfo.SelectToken("SelectallStandardRetailPreferredRetailCostSharingPharmacyLocationsuppl")) : string.Empty;
            string costsharingbeforetheInitialCoverageLimit = _rowData.Root.SelectToken("BasicEnhancedAlternative.AlternativePreICL.HowdoyouapplyyourcostsharingbeforetheInitialCoverageLimitICLisreached").ToString();
            if (costsharingbeforetheInitialCoverageLimit == "2" || costsharingbeforetheInitialCoverageLimit == "1")
            {
                decimal.TryParse(targetValue, out n1);
                if (STDretailLocationSupply.Contains("010") && rightOperand.Split(',')[0] == "One")
                {
                    if (targetValue != "")
                    {
                        if (costsharingbeforetheInitialCoverageLimit == "1" && rightOperand.Split(',')[1] == "NCS")
                        {
                            return n1 == 0;
                        }
                        else
                        if (costsharingbeforetheInitialCoverageLimit == "2" && rightOperand.Split(',')[1] == "PartD")
                        {
                            return n1 == 25;
                        }
                        else
                            return true;
                    }
                    else
                        return false;

                }
                if (STDretailLocationSupply.Contains("001") && rightOperand.Split(',')[0] == "Two")
                {
                    if (targetValue != "")
                    {
                        if (costsharingbeforetheInitialCoverageLimit == "1" && rightOperand.Split(',')[1] == "NCS")
                        {
                            return n1 == 0;
                        }
                        else
                         if (costsharingbeforetheInitialCoverageLimit == "2" && rightOperand.Split(',')[1] == "PartD")
                        {
                            return n1 == 25;
                        }
                        else
                            return true;
                    }
                    else
                        return false;

                }

                if (STDretailLocationSupply.Contains("100") && rightOperand.Split(',')[0] == "Three")
                {
                    if (targetValue != "")
                    {
                        if (costsharingbeforetheInitialCoverageLimit == "1" && rightOperand.Split(',')[1] == "NCS")
                        {
                            return n1 == 0;
                        }
                        else
                         if (costsharingbeforetheInitialCoverageLimit == "2" && rightOperand.Split(',')[1] == "PartD")
                        {
                            return n1 == 25;
                        }
                        else
                            return true;
                    }
                    else
                        return false;
                }
                if (PrefRetaillocationsupply.Contains("010") && rightOperand.Split(',')[0] == "OnePR")
                {
                    if (targetValue != "")
                    {
                        if (costsharingbeforetheInitialCoverageLimit == "1" && rightOperand.Split(',')[1] == "NCS")
                        {
                            return n1 == 0;
                        }
                        else
                        if (costsharingbeforetheInitialCoverageLimit == "2" && rightOperand.Split(',')[1] == "PartD")
                        {
                            return n1 == 25;
                        }
                        else
                            return true;
                    }
                    else
                        return false;

                }
                if (PrefRetaillocationsupply.Contains("001") && rightOperand.Split(',')[0] == "TwoPR")
                {
                    if (targetValue != "")
                    {
                        if (costsharingbeforetheInitialCoverageLimit == "1" && rightOperand.Split(',')[1] == "NCS")
                        {
                            return n1 == 0;
                        }
                        else
                        if (costsharingbeforetheInitialCoverageLimit == "2" && rightOperand.Split(',')[1] == "PartD")
                        {
                            return n1 == 25;
                        }
                        else
                            return true;
                    }
                    else
                        return false;
                }

                if (PrefRetaillocationsupply.Contains("100") && rightOperand.Split(',')[0] == "ThreePR")
                {
                    if (targetValue != "")
                    {
                        if (costsharingbeforetheInitialCoverageLimit == "1" && rightOperand.Split(',')[1] == "NCS")
                        {
                            return n1 == 0;
                        }
                        else
                        if (costsharingbeforetheInitialCoverageLimit == "2" && rightOperand.Split(',')[1] == "PartD")
                        {
                            return n1 == 25;
                        }
                        else
                            return true;
                    }
                    else
                        return false;
                }
                return true;
            }
            return true;
        }
        private bool repeaterPreICLStandardMailCostSharing(string LeftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            //Pre ICL -Standard Mail Cost Sharing

            string targetValue, tierID, mailOrderCSMonthly, mailPRCSMonthly;
            decimal n1;
            targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);
            tierID = Convert.ToString(_rowData.SelectToken("TierID") ?? string.Empty);
            var tierTypeInfo = _formData.SelectToken("PreICL.PreICLTierLocations").Where(x => Convert.ToString(x["TierID"] ?? string.Empty) == tierID).FirstOrDefault();
            mailOrderCSMonthly = tierTypeInfo != null ? Convert.ToString(tierTypeInfo.SelectToken("SelectallStandardMailOrderCostSharingLocationsupplyamountsthatapplyfor")) : string.Empty;
            mailPRCSMonthly = tierTypeInfo != null ? Convert.ToString(tierTypeInfo.SelectToken("SelectallMailOrderStandardPreferredLocationsupplyamountsthatapplyforth")) : string.Empty;
            string costsharingbeforetheInitialCoverageLimit = _rowData.Root.SelectToken("BasicEnhancedAlternative.AlternativePreICL.HowdoyouapplyyourcostsharingbeforetheInitialCoverageLimitICLisreached").ToString();
            if (costsharingbeforetheInitialCoverageLimit == "2" || costsharingbeforetheInitialCoverageLimit == "1")
            {
                decimal.TryParse(targetValue, out n1);
                if (mailOrderCSMonthly.Contains("010") && rightOperand.Split(',')[0] == "One")
                {
                    if (targetValue != "")
                    {
                        if (costsharingbeforetheInitialCoverageLimit == "1" && rightOperand.Split(',')[1] == "NCS")
                        {
                            return n1 == 0;
                        }
                        else
                        if (costsharingbeforetheInitialCoverageLimit == "2" && rightOperand.Split(',')[1] == "PartD")
                        {
                            return n1 == 25;
                        }
                        else
                            return true;
                    }
                    else
                        return false;

                }
                if (mailOrderCSMonthly.Contains("001") && rightOperand.Split(',')[0] == "Two")
                {
                    if (targetValue != "")
                    {
                        if (costsharingbeforetheInitialCoverageLimit == "1" && rightOperand.Split(',')[1] == "NCS")
                        {
                            return n1 == 0;
                        }
                        else
                         if (costsharingbeforetheInitialCoverageLimit == "2" && rightOperand.Split(',')[1] == "PartD")
                        {
                            return n1 == 25;
                        }
                        else
                            return true;
                    }
                    else
                        return false;

                }

                if (mailOrderCSMonthly.Contains("100") && rightOperand.Split(',')[0] == "Three")
                {
                    if (targetValue != "")
                    {
                        if (costsharingbeforetheInitialCoverageLimit == "1" && rightOperand.Split(',')[1] == "NCS")
                        {
                            return n1 == 0;
                        }
                        else
                         if (costsharingbeforetheInitialCoverageLimit == "2" && rightOperand.Split(',')[1] == "PartD")
                        {
                            return n1 == 25;
                        }
                        else
                            return true;
                    }
                    else
                        return false;
                }
                if (mailPRCSMonthly.Contains("010") && rightOperand.Split(',')[0] == "OnePR")
                {
                    if (targetValue != "")
                    {
                        if (costsharingbeforetheInitialCoverageLimit == "1" && rightOperand.Split(',')[1] == "NCS")
                        {
                            return n1 == 0;
                        }
                        else
                        if (costsharingbeforetheInitialCoverageLimit == "2" && rightOperand.Split(',')[1] == "PartD")
                        {
                            return n1 == 25;
                        }
                        else
                            return true;
                    }
                    else
                        return false;

                }
                if (mailPRCSMonthly.Contains("001") && rightOperand.Split(',')[0] == "TwoPR")
                {
                    if (targetValue != "")
                    {
                        if (costsharingbeforetheInitialCoverageLimit == "1" && rightOperand.Split(',')[1] == "NCS")
                        {
                            return n1 == 0;
                        }
                        else
                        if (costsharingbeforetheInitialCoverageLimit == "2" && rightOperand.Split(',')[1] == "PartD")
                        {
                            return n1 == 25;
                        }
                        else
                            return true;
                    }
                    else
                        return false;
                }

                if (mailPRCSMonthly.Contains("100") && rightOperand.Split(',')[0] == "ThreePR")
                {
                    if (targetValue != "")
                    {
                        if (costsharingbeforetheInitialCoverageLimit == "1" && rightOperand.Split(',')[1] == "NCS")
                        {
                            return n1 == 0;
                        }
                        else
                        if (costsharingbeforetheInitialCoverageLimit == "2" && rightOperand.Split(',')[1] == "PartD")
                        {
                            return n1 == 25;
                        }
                        else
                            return true;
                    }
                    else
                        return false;
                }
                return true;
            }
            return true;


        }
        private bool repeaterPreICLOONandLTC(string LeftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            string targetValue, tierID, outOfNetworkSupply, longTermCare;
            decimal n1;
            targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);
            tierID = Convert.ToString(_rowData.SelectToken("TierID") ?? string.Empty);
            var tierTypeInfoOON = _formData.SelectToken("PreICL.PreICLTierLocations").Where(x => Convert.ToString(x["TierID"] ?? string.Empty) == tierID).FirstOrDefault();
            outOfNetworkSupply = tierTypeInfoOON != null ? Convert.ToString(tierTypeInfoOON.SelectToken("SelectallOutofNetworkPharmacyLocationsupplyamountsthatapplyforthisTier")) : string.Empty;
            longTermCare = tierTypeInfoOON != null ? Convert.ToString(tierTypeInfoOON.SelectToken("SelectallLongTermCarePharmacyLocationsupplyamountsthatapplyforthisTier")) : string.Empty;
            string costsharingbeforetheInitialCoverageLimit = _rowData.Root.SelectToken("BasicEnhancedAlternative.AlternativePreICL.HowdoyouapplyyourcostsharingbeforetheInitialCoverageLimitICLisreached").ToString();
            if (costsharingbeforetheInitialCoverageLimit == "2" || costsharingbeforetheInitialCoverageLimit == "1")
            {
                decimal.TryParse(targetValue, out n1);

                if (outOfNetworkSupply.Contains("01") && rightOperand.Split(',')[0] == "One")
                {
                    if (targetValue != "")
                    {
                        if (costsharingbeforetheInitialCoverageLimit == "1" && rightOperand.Split(',')[1] == "NCS")
                        {
                            return n1 == 0;
                        }
                        else
                        if (costsharingbeforetheInitialCoverageLimit == "2" && rightOperand.Split(',')[1] == "PartD")
                        {
                            return n1 == 25;
                        }
                        else
                            return true;
                    }
                    else
                        return false;

                }
                if (outOfNetworkSupply.Contains("10") && rightOperand.Split(',')[0] == "Two")
                {
                    if (targetValue != "")
                    {
                        if (costsharingbeforetheInitialCoverageLimit == "1" && rightOperand.Split(',')[1] == "NCS")
                        {
                            return n1 == 0;
                        }
                        else
                         if (costsharingbeforetheInitialCoverageLimit == "2" && rightOperand.Split(',')[1] == "PartD")
                        {
                            return n1 == 25;
                        }
                        else
                            return true;
                    }
                    else
                        return false;

                }
                if (longTermCare.Contains("1") && rightOperand.Split(',')[0] == "Long")
                {
                    if (targetValue != "")
                    {
                        if (costsharingbeforetheInitialCoverageLimit == "1" && rightOperand.Split(',')[1] == "NCS")
                        {
                            return n1 == 0;
                        }
                        else
                        if (costsharingbeforetheInitialCoverageLimit == "2" && rightOperand.Split(',')[1] == "PartD")
                        {
                            return n1 == 25;
                        }
                        else
                            return true;
                    }
                    else
                        return false;

                }
                return true;
            }
            return true;
        }

        private bool repeaterGapTierTypeCostShareStructure(string leftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            //Pre ICL - Tier Type and Cost Share Structure
            string targetValue, tierID, tierLabelDescriptions;
            targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);
            tierID = Convert.ToString(_rowData.SelectToken("TierID") ?? string.Empty);
            var tierTypeInfo = _formData.SelectToken("GapCoverage.GapCoverageGeneral").Where(x => Convert.ToString(x["TierID"] ?? string.Empty) == tierID).FirstOrDefault();
            tierLabelDescriptions = tierTypeInfo != null ? Convert.ToString(tierTypeInfo.SelectToken("TierLabelDescriptions")) : string.Empty;
            if (tierLabelDescriptions.Contains("Brand") == true)
            {
                if (targetValue.Contains("10"))
                {
                    return true;
                }
                else
                    return false;
            }

            return true;

        }
        private bool repeaterGapCoverageTierCoverage(string leftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            string targetValue, tierID, preICLCoverageDrugsthroughGap, ICLDrugType;
            targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);
            tierID = Convert.ToString(_rowData.SelectToken("TierID") ?? string.Empty);
            var tierTypeInfogap = _formData.SelectToken("GapCoverage.GapCoverageTierTypeandCostShareStructure").Where(x => Convert.ToString(x["TierID"] ?? string.Empty) == tierID).FirstOrDefault();
            preICLCoverageDrugsthroughGap = tierTypeInfogap != null ? Convert.ToString(tierTypeInfogap.SelectToken("TowhatextentareanyPreICLcovereddrugsonthistiercoveredthroughthegap")) : string.Empty;
            var tierTypeInfoICL = _formData.SelectToken("GapCoverage.DGapCoverageTierTypeandCostShareStructure").Where(x => Convert.ToString(x["TierID"] ?? string.Empty) == tierID).FirstOrDefault();
            ICLDrugType = tierTypeInfoICL != null ? Convert.ToString(tierTypeInfoICL.SelectToken("TierDrugtypesselectallthatapply")) : string.Empty;

            if (preICLCoverageDrugsthroughGap == "2" && ICLDrugType.Contains("10") && ICLDrugType.Contains("01"))
            {
                if (targetValue != "")
                {
                    return true;
                }
                else
                    return false;
            }
            return true;
        }
        private bool repeaterGapCoverageStandardRetailCostSharing(string leftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            string targetValue, tierID, ICLDrugType;
            decimal n1;
            targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);
            tierID = Convert.ToString(_rowData.SelectToken("TierID") ?? string.Empty);
            var tierTypeInfoICL = _formData.SelectToken("GapCoverage.DGapCoverageTierTypeandCostShareStructure").Where(x => Convert.ToString(x["TierID"] ?? string.Empty) == tierID).FirstOrDefault();
            ICLDrugType = tierTypeInfoICL != null ? Convert.ToString(tierTypeInfoICL.SelectToken("TierDrugtypesselectallthatapply")) : string.Empty;
            decimal.TryParse(targetValue, out n1);
            if (ICLDrugType.Contains("01"))
            {
                if (n1 <= 24)
                    return true;
                else
                    return false;
            }
            if (ICLDrugType.Contains("10") && !ICLDrugType.Contains("01"))
            {
                if (n1 <= 28)
                    return true;
                else
                    return false;
            }
            return true;
        }
        private bool repeaterPreICLMedicareMedicaidTierTypeandCostShareStructure(string leftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            string targetValue, tierID, tierInclude;
            targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);
            tierID = Convert.ToString(_rowData.SelectToken("TierID") ?? string.Empty);
            var tierTypeInfo = _formData.SelectToken("MedicareMedicaidPreICL.MedicareMedicaidCostSharingPreICL").Where(x => Convert.ToString(x["TierID"] ?? string.Empty) == tierID).FirstOrDefault();
            tierInclude = tierTypeInfo != null ? Convert.ToString(tierTypeInfo.SelectToken("Tierincludesselectonlyoneforeachtier")) : string.Empty;
            if (tierInclude == "1" || tierInclude == "3")
            {
                return true;
            }
            return true;
        }
        private bool repeatercheckServiceIsSelected(string leftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            string targetValue, groupID, sourceServices = "", maxCoinsuranceValue = string.Empty, maxCopayValue = string.Empty;
            string[] Sservices;
            bool breakLoop = false;
            groupID = Convert.ToString(_rowData.SelectToken("OONGroupID") ?? string.Empty);
            var oONGroup1Repeater = _formData.SelectToken("OONGroups.OONGroupsBase1").Where(x => Convert.ToString(x["OONGroupID"] ?? string.Empty) == groupID).FirstOrDefault();
            if (rightOperand == "2")
                targetValue = oONGroup1Repeater != null ? Convert.ToString(oONGroup1Repeater.SelectToken("SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis")) : string.Empty;
            else
                targetValue = oONGroup1Repeater != null ? Convert.ToString(oONGroup1Repeater.SelectToken("SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort")) : string.Empty;

            string targetServices = targetValue.Replace("\r\n", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty).Replace("\"", string.Empty).Replace(" ", string.Empty);
            string[] Tservices = targetServices.Split(',');
            if (rightOperand == "2")
            {
                //check for medicare services range for > 50% group
                var oONGroup2Repeater = _formData.SelectToken("OONGroups.OONGroupsBase2").Where(x => Convert.ToString(x["OONGroupID"] ?? string.Empty) == groupID).FirstOrDefault();
                maxCoinsuranceValue = oONGroup2Repeater != null ? Convert.ToString(oONGroup2Repeater.SelectToken("EnterMaximumCoinsurancePercentageforthisGroup")) : string.Empty;
                if (!String.IsNullOrEmpty(targetServices))
                {
                    if (String.IsNullOrEmpty(maxCoinsuranceValue) || Convert.ToInt32(maxCoinsuranceValue) <= 50)
                        breakLoop = false;
                    else
                        return false;
                }
            }
            string PlanType = _formData.SelectToken("SectionA.SectionA1.PlanType").ToString();
            string IsThisNWPlan = _formData.SelectToken("SectionA.SectionA1.Isthisanetworkplan").ToString();
            if ((PlanType == "4" || PlanType == "31" || (PlanType == "9" && IsThisNWPlan == "1")) && rightOperand == "2")
            {
                Sservices = new string[] { "1a", "1b", "2", "3-1", "3-2", "3-3", "5", "6", "7a", "7b", "7c", "7d", "7e", "7f", "7g", "7h", "7i", "8a1", "8a2", "8b1", "8b2", "8b3", "9a1", "9a2", "9b", "9c", "9d", "10a1", "10a2", "11a", "11b", "11c", "12", "14a", "14d", "14e1", "14e2", "14e3", "14e4", "14e5", "14e6", "15", "16b", "17a", "17b", "18a", "20" };
            }
            else
            {
                string sourceValue = _formData.SelectToken(leftOperandName).ToString();
                sourceServices = sourceValue.Replace("\r\n", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty).Replace("\"", string.Empty).Replace(" ", string.Empty);
                Sservices = sourceServices.Split(',');
            }
            if (Sservices.Length > 0 && !String.IsNullOrEmpty(targetServices))
            {

                foreach (string selservice in Tservices)
                {
                    if (!Sservices.Contains(selservice))
                    {
                        breakLoop = true;
                        break;
                    }
                }
                if (!breakLoop)
                {
                    decimal n1;
                    decimal.TryParse(groupID, out n1);
                    string MedicareServices = "";
                    for (var i = 1; i < n1; i++)
                    {
                        string id = i.ToString();
                        var Data = _formData.SelectToken("OONGroups.OONGroupsBase1").Where(x => Convert.ToString(x["OONGroupID"] ?? string.Empty) == id).FirstOrDefault();
                        if (rightOperand == "2")
                        {
                            MedicareServices = Data != null ? Convert.ToString(Data.SelectToken("SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis")) : string.Empty;
                        }
                        else
                            MedicareServices = Data != null ? Convert.ToString(Data.SelectToken("SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort")) : string.Empty;

                        string CheckServices = MedicareServices.Replace("\r\n", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty).Replace("\"", string.Empty).Replace(" ", string.Empty);
                        string[] Cservices = CheckServices.Split(',');

                        if (!String.IsNullOrEmpty(targetServices))
                        {
                            foreach (string selservice in Tservices)
                            {
                                if (Cservices.Contains(selservice))
                                {
                                    breakLoop = true;
                                    break;
                                }
                            }
                        }
                    }

                }
                if (!breakLoop)
                {
                    return true;
                }
                else
                    return false;
            }
            if (String.IsNullOrEmpty(targetServices))
                return true;
            else
                return false;
        }

        //Process list of expressions
        public bool ProcessNode(ExpressionDesign expression, JObject _rowData, RuleDesign rule)
        {
            if (rule != null && rule.IsParentRepeater)
            {

                //var exp = expression.Expressions.Where(e => e.ExpressionTypeId != (int)ExpressionTypes.NODE).FirstOrDefault();
                //JObject targetData = this.GetTargetDataByFilters(exp.LeftKeyFilters, _rowData);
                //string leftOperand = GetOperandValue(exp.LeftOperandName, exp.LeftOperand, targetData);
                //string rightOperand = exp.RightOperand;

                //if (rule.SuccessValue == "RepeaterCoinsuranceCheck")
                //    return RepeaterCoinsuranceCheck(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "RepeaterDeductibleCheck")
                //    return RepeaterDeductibleCheck(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "RepeaterPlanMaxCoverageCheck")
                //    return RepeaterPlanMaxCoverageCheck(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "RepeaterMINMAXCheck")
                //    return RepeaterMINMAXCheck(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "RepeaterIsRequired")
                //    return RepeaterIsRequired(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "RepeaterDeductibleIsRequired")
                //    return RepeaterDeductibleIsRequired(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "repeatercheckServiceIsSelected")
                //    return repeatercheckServiceIsSelected(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "RepeaterOONDeductible")
                //    return RepeaterOONDeductible(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "repeaterPreICLTierTypeCostShareStructure")
                //    return repeaterPreICLTierTypeCostShareStructure(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "repeaterTypeOfCostSharingStructure")
                //    return repeaterTypeOfCostSharingStructure(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "repeaterStdRetailcostsharingMonthlySupply")
                //    return repeaterStdRetailcostsharingMonthlySupply(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "repeaterMailOrderCostSharingMonthlySupply")
                //    return repeaterMailOrderCostSharingMonthlySupply(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "rXPreICLIsRequired")
                //    return rXPreICLIsRequired(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "repeaterPreICLStandardRetailCostSharing")
                //    return repeaterPreICLStandardRetailCostSharing(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "repeaterPreICLStandardMailCostSharing")
                //    return repeaterPreICLStandardMailCostSharing(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "repeaterPreICLOONandLTC")
                //    return repeaterPreICLOONandLTC(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "repeaterGapTierTypeCostShareStructure")
                //    return repeaterGapTierTypeCostShareStructure(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "repeaterGapCoverageTierCoverage")
                //    return repeaterGapCoverageTierCoverage(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "repeaterGapCoverageStandardRetailCostSharing")
                //    return repeaterGapCoverageStandardRetailCostSharing(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "repeaterPreICLMedicareMedicaidTierTypeandCostShareStructure")
                //    return repeaterPreICLMedicareMedicaidTierTypeandCostShareStructure(exp.LeftOperandName, rightOperand, _rowData, rule);
                //if (rule.SuccessValue == "RepeaterPlanMaxAmountCheck")
                //    return RepeaterPlanMaxAmountCheck(exp.LeftOperandName, rightOperand, _rowData, rule);
            }
            //loop through all the expression
            //Call ProcessLeaf to evaluate single expression                    
            bool isSuccess = expression.LogicalOperatorTypeId == (int)LogicalOperatorTypes.AND ? true : false;
            if (expression.Expressions != null && expression.Expressions.Count > 0)
            {
                for (var idx = 0; idx < expression.Expressions.Count; idx++)
                {
                    string sourceRepeaterName = "";
                    string leftKeyContainerName = "";
                    string rightKeyContainerName = "";
                    JArray sourceDataRows = null;
                    JArray leftRepeaterRowData = null;
                    JArray rightRepeaterRowData = null;
                    JObject sourceDataRow = null;
                    if (rule.IsParentRepeater == true && expression.Expressions[idx].LeftOperand != null && expression.Expressions[idx].LeftOperand != "")
                    {
                        if (rule.TargetKeyFilters != null && rule.TargetKeyFilters.Count > 0)
                        {
                            var leftRepeater = this.GetRepeaterDataFromElementName(expression.Expressions[idx].LeftOperandName);
                            leftKeyContainerName = leftRepeater.repeaterName;
                            leftRepeaterRowData = this.getFilteredRowData(expression.Expressions[idx].LeftKeyFilters, (JArray)leftRepeater.data);
                            if (expression.Expressions[idx].IsRightOperandElement == true)
                            {
                                var rightRepeater = this.GetRepeaterDataFromElementName(expression.Expressions[idx].RightOperandName);
                                rightKeyContainerName = rightRepeater.repeaterName;
                                rightRepeaterRowData = this.getFilteredRowData(expression.Expressions[idx].RightKeyFilters, (JArray)rightRepeater.data);
                            }
                        }
                        else
                        {
                            string[] nameParts = rule.UIElementFullName.Split('.');
                            RepeaterDesign targetRepearter = _detail.Sections.Find(x => x.FullName == nameParts[0]).Elements.Find(y => y.GeneratedName == nameParts[1]).Repeater;
                            List<ElementDesign> targetKeyColumns = targetRepearter.Elements.FindAll(x => x.IsKey == true);
                            if (targetKeyColumns.Count > 0)
                            {
                                List<string> targerDataKeys = new List<string>();
                                foreach (ElementDesign key in targetKeyColumns)
                                    targerDataKeys.Add(key.GeneratedName);


                                var sourceRepeaterData = this.GetRepeaterDataFromElementName(expression.Expressions[idx].LeftOperandName);
                                sourceDataRows = (JArray)sourceRepeaterData.data;
                                if (sourceDataRows.Count > 0)
                                {
                                    if (sourceRepeaterData != null) { sourceRepeaterName = sourceRepeaterData.repeaterName; }

                                    nameParts = expression.Expressions[idx].LeftOperandName.Split('.');
                                    RepeaterDesign sourceRepearter = _detail.Sections.Find(x => x.FullName == nameParts[0]).Elements.Find(y => y.GeneratedName == nameParts[1]).Repeater;
                                    List<ElementDesign> sourceKeyColumns = sourceRepearter != null ? sourceRepearter.Elements.FindAll(x => x.IsKey == true) : null;
                                    List<string> sourceDataKeys = new List<string>();
                                    foreach (ElementDesign key in sourceKeyColumns)
                                        sourceDataKeys.Add(key.GeneratedName);

                                    foreach (string key in sourceDataKeys)
                                    {
                                        int targetValue;
                                        if (Int32.TryParse(Convert.ToString(_rowData[key]), out targetValue))
                                            sourceDataRows = new JArray(sourceDataRows.Where(v => (int)v[key] == targetValue));
                                        else
                                            sourceDataRows = new JArray(sourceDataRows.Where(v => v[key].ToString() == _rowData[key].ToString()));
                                    }

                                    sourceDataRow = (JObject)sourceDataRows.FirstOrDefault();
                                }
                            }
                        }
                    }

                    bool result = false;
                    if (expression.Expressions[idx].ExpressionTypeId == (int)ExpressionTypes.NODE)
                        result = this.ProcessNode(expression.Expressions[idx], _rowData, rule);
                    else
                    {
                        if (rule.TargetKeyFilters != null && rule.TargetKeyFilters.Count > 0)
                            result = leftRepeaterRowData != null ? this.processComplexLeaf(expression.Expressions[idx], _rowData, leftKeyContainerName, JObject.Parse(leftRepeaterRowData[0].ToString()), rightKeyContainerName, (rightRepeaterRowData != null ? JObject.Parse(rightRepeaterRowData[0].ToString()) : null)) : false;
                        else
                            result = this.ProcessLeaf(expression.Expressions[idx], _rowData, sourceRepeaterName != "" ? sourceRepeaterName : null, sourceDataRow != null ? sourceDataRow : null);
                    }

                    if (expression.LogicalOperatorTypeId == (int)LogicalOperatorTypes.AND)
                    {
                        if (result == false)
                        {
                            isSuccess = false;
                            break;
                        }
                    }
                    else
                    {
                        if (result == true)
                        {
                            isSuccess = true;
                            break;
                        }
                    }
                }
            }

            return isSuccess;
        }

        private JArray getFilteredRowData(List<RepeaterKeyFilterDesign> filterKeys, JArray repeaterData)
        {
            JArray filteredRow = null;
            foreach (var filter in filterKeys)
            {
                string key = filter.RepeaterKeyName.Split('.')[filter.RepeaterKeyName.Split('.').Length - 1];
                string _value = filter.RepeaterKeyValue.ToString();
                repeaterData = new JArray(repeaterData.Where(v => v[key].ToString() == _value.ToString()));
            }
            if (repeaterData != null)
                filteredRow = repeaterData;

            return filteredRow;
        }
        public bool processComplexLeaf(ExpressionDesign expression, JObject targetData, string leftRepearterName, JObject leftRowData, string rightRepearterName, JObject rightRowData)
        {
            targetData = this.GetTargetDataByFilters(expression.LeftKeyFilters, targetData);
            string leftOperand = GetOperandValue(expression.LeftOperandName, expression.LeftOperand, targetData, leftRepearterName, leftRowData);
            string rightOperand = expression.RightOperand;

            if (expression.IsRightOperandElement)
            {
                targetData = this.GetTargetDataByFilters(expression.RightKeyFilters, targetData);
                rightOperand = GetOperandValue(expression.RightOperandName, expression.RightOperand, targetData, rightRepearterName, rightRowData);
                rightOperand = this.GetRightOperandByComplextOp(expression, rightOperand);
            }
            return this.EvaluateExpression(leftOperand, expression.OperatorTypeId, rightOperand, expression.IsRightOperandElement, expression.LeftOperand);
        }
        public bool ProcessLeaf(ExpressionDesign expression, JObject targetData, string sourceRepearterName, JObject sourceData)
        {
            targetData = this.GetTargetDataByFilters(expression.LeftKeyFilters, targetData);
            string leftOperand = GetOperandValue(expression.LeftOperandName, expression.LeftOperand, targetData, sourceRepearterName, sourceData);
            string rightOperand = expression.RightOperand;

            if (expression.IsRightOperandElement)
            {
                targetData = this.GetTargetDataByFilters(expression.RightKeyFilters, targetData);
                rightOperand = GetOperandValue(expression.RightOperandName, expression.RightOperand, targetData, sourceRepearterName, sourceData);
                rightOperand = this.GetRightOperandByComplextOp(expression, rightOperand);
            }
            return this.EvaluateExpression(leftOperand, expression.OperatorTypeId, rightOperand, expression.IsRightOperandElement, expression.LeftOperand);
        }

        //Evaluate single expression
        public bool EvaluateExpression(string leftOperand, int op, string rightOperand, bool isRightOperandElement, string uiElementId)
        {
            var result = false;

            if (op == (int)OperatorTypes.Equals)
            {
                result = ExpressionHelper.Equal(leftOperand, rightOperand);
            }
            else if (op == (int)OperatorTypes.GreaterThan)
            {
                result = ExpressionHelper.GreaterThan(leftOperand, rightOperand, uiElementId.IndexOf("Calendar") > 0);
            }
            else if (op == (int)OperatorTypes.GreaterThanOrEqualTo)
            {
                result = ExpressionHelper.GreaterThanOrEqual(leftOperand, rightOperand, uiElementId.IndexOf("Calendar") > 0);
            }
            else if (op == (int)OperatorTypes.LessThan)
            {
                result = ExpressionHelper.LessThan(leftOperand, rightOperand, uiElementId.IndexOf("Calendar") > 0);
            }
            else if (op == (int)OperatorTypes.LessThanOrEqualTo)
            {
                result = ExpressionHelper.LessThanOrEqual(leftOperand, rightOperand, uiElementId.IndexOf("Calendar") > 0);
            }
            else if (op == (int)OperatorTypes.Contains)
            {
                if (uiElementId.IndexOf("DropDown") > -1)
                {
                    if (leftOperand == "Select One")
                    {
                        leftOperand = "";
                    }
                }
                result = ExpressionHelper.Contains(leftOperand, rightOperand);
            }
            else if (op == (int)OperatorTypes.NotContains)
            {
                if (uiElementId.IndexOf("DropDown") > -1)
                {
                    if (leftOperand == "Select One")
                    {
                        leftOperand = "";
                    }
                }
                result = ExpressionHelper.NotContains(leftOperand, rightOperand);
            }
            else if (op == (int)OperatorTypes.NotEquals)
            {
                result = ExpressionHelper.NotEqual(leftOperand, rightOperand);
            }
            else if (op == (int)OperatorTypes.IsNull)
            {
                result = ExpressionHelper.IsNull(leftOperand);
            }
            return result;
        }

        public string GetOperandValue(string operandElementFullName, string operandElementName, JObject targetData, string sourceRepearterName = "", JObject sourceData = null)
        {
            if (_sectionName != null && operandElementFullName.Split('.')[0] != _sectionName)
            {
                targetData = JObject.Parse(_formDataInstanceManager.GetSectionData(_formInstanceId, operandElementFullName.Split('.')[0], false, _detail, false, false));
            }

            string value = null;
            if (!string.IsNullOrEmpty(_containerName) && operandElementFullName.Contains(_containerName) && targetData != null)
            {
                string elementName = operandElementFullName.Replace(_containerName + ".", "");
                string[] nameParts = elementName.Split('.');
                JToken dataPart = null;
                for (var idx = 0; idx < nameParts.Length; idx++)
                {
                    if (idx == 0)
                    {
                        if (targetData[nameParts[idx]] != null)
                        {
                            dataPart = targetData[nameParts[idx]];
                        }
                    }
                    else if (idx == (nameParts.Length - 1))
                    {
                        if (targetData[nameParts[idx]] != null)
                        {
                            dataPart = targetData[nameParts[idx]];
                        }
                    }
                    else
                    {
                        dataPart = dataPart[nameParts[idx]];
                    }
                }
                value = dataPart == null ? "" : Convert.ToString(dataPart);
            }
            else if (!string.IsNullOrEmpty(sourceRepearterName) && operandElementFullName.Contains(sourceRepearterName) && sourceData != null)
            {
                string elementName = operandElementFullName.Replace(sourceRepearterName + ".", "");
                string[] nameParts = elementName.Split('.');
                JToken dataPart = null;
                for (var idx = 0; idx < nameParts.Length; idx++)
                {
                    if (idx == 0)
                    {
                        if (sourceData[nameParts[idx]] != null)
                        {
                            dataPart = sourceData[nameParts[idx]];
                        }
                    }
                    else if (idx == (nameParts.Length - 1))
                    {
                        if (sourceData[nameParts[idx]] != null)
                        {
                            dataPart = sourceData[nameParts[idx]];
                        }
                    }
                    else
                    {
                        dataPart = dataPart[nameParts[idx]];
                    }
                }
                value = dataPart == null ? "" : Convert.ToString(dataPart);
            }
            else
            {
                JToken dataPart = null;
                var nameParts = operandElementFullName.Split('.');
                for (var idx = 0; idx < nameParts.Length; idx++)
                {
                    if (idx == 0)
                    {
                        dataPart = targetData[nameParts[idx]];
                    }
                    else
                    {
                        if (dataPart != null && dataPart.SelectToken(nameParts[idx]) != null)
                        {
                            dataPart = dataPart[nameParts[idx]];
                        }
                    }
                }
                value = dataPart == null ? "" : Convert.ToString(dataPart);
            }

            if (operandElementName != null)
            {
                if (operandElementName.IndexOf("CheckBox") > 0 || operandElementName.IndexOf("Radio") > 0)
                {
                    value = value == "true" || value == "True" || value == "Yes" || value == "yes" ? "Yes" : "No";
                }
            }

            if (!string.IsNullOrEmpty(value) && HtmlContentHelper.IsHTML(value))
                value = HtmlContentHelper.GetFreeFromHtmlText(value);

            return value;
        }

        private string GetRightOperandByComplextOp(ExpressionDesign expression, string rightOperand)
        {
            try
            {
                bool prefix = rightOperand.Contains("$");
                bool suffix = rightOperand.Contains("%");
                if (expression.complexOp != null)
                {
                    rightOperand = rightOperand.Replace("$", "").Replace("%", "");
                    switch (expression.complexOp.Factor)
                    {
                        case "%":
                            rightOperand = (Convert.ToDecimal(rightOperand) - ((Convert.ToDecimal(rightOperand) * expression.complexOp.FactorValue) / 100)).ToString();
                            break;
                        case "*":
                            rightOperand = (Convert.ToDecimal(rightOperand) * expression.complexOp.FactorValue).ToString();
                            break;
                        case "+":
                            rightOperand = (Convert.ToDecimal(rightOperand) + expression.complexOp.FactorValue).ToString();
                            break;
                        case "-":
                            rightOperand = (Convert.ToDecimal(rightOperand) - expression.complexOp.FactorValue).ToString();
                            break;
                        default:
                            rightOperand = this.GetRightOperandByOperator(expression.OperatorTypeId, rightOperand, expression.complexOp.FactorValue);
                            break;
                    }
                    if (prefix) { rightOperand = "$" + rightOperand; }
                    if (suffix) { rightOperand = rightOperand + "%"; }
                }
            }
            catch (Exception)
            {
            }
            return rightOperand;
        }

        private string GetRightOperandByOperator(int operatorTypeId, string rightOperand, decimal factorAmount)
        {
            if (operatorTypeId == 3 || operatorTypeId == 7)
            {
                rightOperand = (Convert.ToDecimal(rightOperand) - factorAmount).ToString();
            }

            if (operatorTypeId == 2 || operatorTypeId == 6)
            {
                rightOperand = (Convert.ToDecimal(rightOperand) + factorAmount).ToString();
            }

            return rightOperand;
        }

        private JObject GetTargetDataByFilters(List<RepeaterKeyFilterDesign> keyFilters, JObject _rowData)
        {
            JObject targetData = _rowData;
            if (keyFilters != null && keyFilters.Count > 0)
            {
                string[] path = keyFilters[0].RepeaterKeyName.Split('.');
                JToken rptData = _formData.SelectToken(string.Join(".", path.Take(path.Count() - 1).ToArray()));
                List<JToken> filterData = rptData.ToList();

                foreach (var filter in keyFilters)
                {
                    path = filter.RepeaterKeyName.Split('.');
                    string keyName = path[path.Length - 1];
                    filterData = filterData.Where(s => (string)s[keyName] == filter.RepeaterKeyValue).ToList();
                }

                if (filterData.Count > 0)
                {
                    targetData = filterData[0] as JObject;
                }
            }

            return targetData;
        }

        private bool checkServiceIsSelected(string leftOperand, string rightOperand)
        {

            string sOTServices = leftOperand.Replace("\r\n", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty);
            string[] services = sOTServices.Split(',');
            string pBPServices = rightOperand.Replace("\r\n", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty);
            string[] selectedServices = pBPServices.Split(',');
            bool breakLoop = false;
            if (!String.IsNullOrEmpty(rightOperand) && !String.IsNullOrEmpty(leftOperand))
            {

                foreach (string selservice in selectedServices)
                {
                    if (!services.Contains(selservice))
                    {
                        breakLoop = true;
                        break;
                    }
                }

                if (!breakLoop)
                {
                    return true;
                }
            }
            return false;
        }

        private bool MOOPAmountValidationAcuteInterval1_2019(string leftOperand, string rightOperand, JObject _rowData)
        {
            decimal n1; int n2;
            string moopValue = String.Empty, endDay1 = String.Empty, tierValue = String.Empty;

            try
            {
                if (_rowData != null)
                {
                    moopValue = Convert.ToString(_rowData.SelectToken("SectionDPlanLevel.MaxEnrolleCostLimitInNetwork.IsyourInNetworkMaximumEnrolleeOutofPocketMOOPCostattheVoluntaryorManda") ?? String.Empty);
                    tierValue = rightOperand;
                    if (tierValue.Equals("Tier1"))
                    {
                        endDay1 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval1") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier2"))
                    {
                        endDay1 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.EndDayInterval1forTier2") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier3"))
                    {
                        endDay1 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval1forTier3") ?? String.Empty);
                    }


                    if (decimal.TryParse(leftOperand, out n1) && int.TryParse(endDay1, out n2) && !string.IsNullOrEmpty(moopValue))
                    {
                        if (MOOPAmountValidationAcuteInterval3_2019(leftOperand, tierValue, _rowData))
                        {
                            if (MOOPAmountValidationAcuteInterval2_2019(leftOperand, tierValue, _rowData))
                            {
                                if (moopValue == "1")
                                {
                                    if (n2 <= 6)
                                    {
                                        if (n1 * n2 > 2325) return false;
                                    }
                                    else if (n2 > 6 && n2 <= 10)
                                    {
                                        if (n1 * n2 > 2552) return false;
                                    }
                                    else if (n1 * n2 > 2552) return false;
                                }
                                else
                                {
                                    if (n2 <= 6)
                                    {
                                        if (n1 * n2 > 1860) return false;
                                    }
                                    else if (n2 > 6 && n2 <= 10)
                                    {
                                        if (n1 * n2 > 2042) return false;
                                    }
                                    else if (n2 > 10 && n2 <= 60)
                                    {
                                        if (n1 * n2 > 4314) return false;
                                    }
                                    else if (n1 * n2 > 4314) return false;
                                }
                            }
                            else return false;
                        }
                        else return false;
                    }
                }
            }
            catch (Exception)
            { }
            return true;
        }

        private bool MOOPAmountValidationAcuteInterval2_2019(string leftOperand, string rightOperand, JObject _rowData)
        {
            string moopValue = String.Empty, tierValue = String.Empty, copay1 = String.Empty, endDay1 = String.Empty, copay2 = String.Empty, beginDay2 = String.Empty, endDay2 = String.Empty;
            decimal c1, c2;
            int ed1, bd2, ed2;

            try
            {
                if (_rowData != null)
                {
                    moopValue = Convert.ToString(_rowData.SelectToken("SectionDPlanLevel.MaxEnrolleCostLimitInNetwork.IsyourInNetworkMaximumEnrolleeOutofPocketMOOPCostattheVoluntaryorManda") ?? String.Empty);
                    tierValue = rightOperand;
                    if (tierValue.Equals("Tier1"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval1") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval1") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval2") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.BeginDayInterval2") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval2") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier2"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.CopaymentAmtInterval1forTier2") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.EndDayInterval1forTier2") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.CopaymentAmtInterval2forTier2") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.BeginDayInterval2forTier2") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.EndDayInterval2forTier2") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier3"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval1forTier3") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval1forTier3") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval2forTier3") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.BeginDayInterval2forTier3") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval2forTier3") ?? String.Empty);
                    }


                    if (moopValue == "1")
                    {
                        if (decimal.TryParse(copay1, out c1) && int.TryParse(endDay1, out ed1) && decimal.TryParse(copay2, out c2) && int.TryParse(beginDay2, out bd2) && int.TryParse(endDay2, out ed2))
                        {
                            bd2 = --bd2;
                            var totalDays = ed1 + (ed2 - bd2);
                            if (totalDays <= 60)
                            {
                                decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2));

                                if ((totalDays <= 6) && (totalCopay > 1860)) return false;
                                if ((totalDays <= 10) && (totalCopay > 2042)) return false;
                                if ((totalDays <= 60) && (totalCopay > 4314)) return false;
                            }
                            else
                            {
                                if (ed1 >= 60)
                                {
                                    decimal totalCopay = c1 * 60;
                                    if (totalCopay > 4314) return false;
                                }
                                else
                                {
                                    var extraDays = 60 - ed1;
                                    decimal totalCopay = (c1 * ed1) + (c2 * extraDays);
                                    if (totalCopay > 4314) return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (decimal.TryParse(copay1, out c1) && int.TryParse(endDay1, out ed1) && decimal.TryParse(copay2, out c2) && int.TryParse(beginDay2, out bd2) && int.TryParse(endDay2, out ed2))
                        {
                            bd2 = --bd2;
                            var totalDays = ed1 + (ed2 - bd2);
                            if (totalDays <= 60)
                            {
                                decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2));

                                if ((totalDays <= 10) && (totalCopay > 2552)) return false;
                                if ((totalDays <= 60) && (totalCopay > 2325)) return false;
                            }
                            else
                            {
                                if (ed1 >= 60)
                                {
                                    decimal totalCopay = c1 * 60;
                                    if (totalCopay > 2325) return false;

                                }
                                else
                                {
                                    var extraDays = 60 - ed1;
                                    decimal totalCopay = (c1 * ed1) + (c2 * extraDays);
                                    if (totalCopay > 2325) return false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            { }
            return true;
        }

        private bool MOOPAmountValidationAcuteInterval3_2019(string leftOperand, string rightOperand, JObject _rowData)
        {
            string moopValue = String.Empty, tierValue = String.Empty, copay1 = String.Empty, endDay1 = String.Empty, copay2 = String.Empty, beginDay2 = String.Empty, endDay2 = String.Empty, copay3 = String.Empty, beginDay3 = String.Empty, endDay3 = String.Empty;
            decimal c1, c2, c3;
            int ed1, bd2, ed2, bd3, ed3;

            try
            {
                if (_rowData != null)
                {
                    moopValue = Convert.ToString(_rowData.SelectToken("SectionDPlanLevel.MaxEnrolleCostLimitInNetwork.IsyourInNetworkMaximumEnrolleeOutofPocketMOOPCostattheVoluntaryorManda") ?? String.Empty);
                    tierValue = rightOperand;
                    if (tierValue.Equals("Tier1"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval1") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval1") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval2") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.BeginDayInterval2") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval2") ?? String.Empty);
                        copay3 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval3") ?? String.Empty);
                        beginDay3 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.BeginDayInterval3") ?? String.Empty);
                        endDay3 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase7.MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval3") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier2"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.CopaymentAmtInterval1forTier2") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.EndDayInterval1forTier2") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.CopaymentAmtInterval2forTier2") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.BeginDayInterval2forTier2") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.EndDayInterval2forTier2") ?? String.Empty);
                        copay3 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.CopaymentAmtInterval3forTier2") ?? String.Empty);
                        beginDay3 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.BeginDayInterval3forTier2") ?? String.Empty);
                        endDay3 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier2.EndDayInterval3forTier2") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier3"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval1forTier3") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval1forTier3") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval2forTier3") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.BeginDayInterval2forTier3") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval2forTier3") ?? String.Empty);
                        copay3 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval3forTier3") ?? String.Empty);
                        beginDay3 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.BeginDayInterval3forTier3") ?? String.Empty);
                        endDay3 = Convert.ToString(_rowData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval3forTier3") ?? String.Empty);
                    }


                    if (moopValue == "1")
                    {
                        if (decimal.TryParse(copay1, out c1) && int.TryParse(endDay1, out ed1) && decimal.TryParse(copay2, out c2) && int.TryParse(beginDay2, out bd2) && int.TryParse(endDay2, out ed2) && decimal.TryParse(copay3, out c3) && int.TryParse(beginDay3, out bd3) && int.TryParse(endDay3, out ed3))
                        {
                            bd2 = --bd2; bd3 = --bd3;
                            var totalDays = ed1 + (ed2 - bd2) + (ed3 - bd3);
                            if (totalDays <= 60)
                            {
                                decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2)) + (c3 * (ed3 - bd3));
                                if ((totalDays <= 6) && (totalCopay > 1860)) return false;
                                if ((totalDays <= 10) && (totalCopay > 2042)) return false;
                                if ((totalDays <= 60) && (totalCopay > 4314)) return false;
                            }
                            else
                            {
                                if (ed1 >= 60)
                                {
                                    decimal totalCopay = (c1 * 60);
                                    if (totalCopay > 4314) return false;
                                }
                                else if (ed1 + ed2 >= 60)
                                {
                                    var extraDays = 60 - ed1;

                                    decimal totalCopay = (c1 * ed1) + (c2 * extraDays);
                                    if (totalCopay > 4314) return false;
                                }
                                else
                                {
                                    var extraDays = 60 - (ed1 + ed2);

                                    decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2)) + (c3 * extraDays);
                                    if (totalCopay > 4314) return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (decimal.TryParse(copay1, out c1) && int.TryParse(endDay1, out ed1) && decimal.TryParse(copay2, out c2) && int.TryParse(beginDay2, out bd2) && int.TryParse(endDay2, out ed2) && decimal.TryParse(copay3, out c3) && int.TryParse(beginDay3, out bd3) && int.TryParse(endDay3, out ed3))
                        {
                            bd2 = --bd2; bd3 = --bd3;
                            var totalDays = ed1 + (ed2 - bd2) + (ed3 - bd3);

                            if (totalDays <= 60)
                            {
                                decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2)) + (c3 * (ed3 - bd3));
                                if ((totalDays <= 10) && (totalCopay > 2552)) return false;
                                if ((totalDays <= 60) && (totalCopay > 2325)) return false;
                            }
                            else
                            {
                                if (ed1 >= 60)
                                {
                                    decimal totalCopay = (c1 * 60);
                                    if (totalCopay > 2325) return false;
                                }
                                else if (ed1 + ed2 >= 60)
                                {
                                    var extraDays = 60 - ed1;

                                    decimal totalCopay = (c1 * ed1) + (c2 * extraDays);
                                    if (totalCopay > 2325) return false;
                                }
                                else
                                {
                                    var extraDays = 60 - (ed1 + ed2);

                                    decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2)) + (c3 * extraDays);
                                    if (totalCopay > 2325) return false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            { }
            return true;
        }

        private bool MOOPAmountValidationPsychiatricInterval1_2019(string leftOperand, string rightOperand, JObject _rowData)
        {
            decimal n1; int n2;
            string moopValue = String.Empty, endDay1 = String.Empty, tierValue = String.Empty; ;

            try
            {
                if (_rowData != null)
                {
                    moopValue = Convert.ToString(_rowData.SelectToken("SectionDPlanLevel.MaxEnrolleCostLimitInNetwork.IsyourInNetworkMaximumEnrolleeOutofPocketMOOPCostattheVoluntaryorManda") ?? String.Empty);
                    tierValue = rightOperand;
                    if (tierValue.Equals("Tier1"))
                    {
                        endDay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval1") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier2"))
                    {
                        endDay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval1forTier3") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier3"))
                    {
                        endDay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval1forTier3") ?? String.Empty);
                    }

                    if (decimal.TryParse(leftOperand, out n1) && int.TryParse(endDay1, out n2) && string.IsNullOrEmpty(moopValue))
                    {
                        if (MOOPAmountValidationPsychiatricInterval3_2019(leftOperand, tierValue, _rowData))
                        {
                            if (MOOPAmountValidationPsychiatricInterval2_2019(leftOperand, tierValue, _rowData))
                            {
                                if (moopValue == "1")
                                {
                                    if (n2 <= 15)
                                    {
                                        if (n1 * n2 > 2075) return false;
                                    }
                                    else if (n2 > 15 && n2 <= 60)
                                    {
                                        if (n1 * n2 > 2737) return false;
                                    }
                                    else if (n1 * n2 > 2737) return false;
                                }
                                else
                                {
                                    if (n2 <= 15)
                                    {
                                        if (n1 * n2 > 1660) return false;
                                    }
                                    else if (n2 > 15 && n2 <= 60)
                                    {
                                        if (n1 * n2 > 2190) return false;
                                    }
                                    else if (n1 * n2 > 2190) return false;
                                }
                            }
                            else return false;
                        }
                        else return false;
                    }
                }
            }
            catch (Exception)
            { }
            return true;
        }

        private bool MOOPAmountValidationPsychiatricInterval2_2019(string leftOperand, string rightOperand, JObject _rowData)
        {
            string moopValue = String.Empty, tierValue = String.Empty, copay1 = String.Empty, endDay1 = String.Empty, copay2 = String.Empty, beginDay2 = String.Empty, endDay2 = String.Empty;
            decimal c1, c2;
            int ed1, bd2, ed2;

            try
            {
                if (_rowData != null)
                {
                    moopValue = Convert.ToString(_rowData.SelectToken("SectionDPlanLevel.MaxEnrolleCostLimitInNetwork.IsyourInNetworkMaximumEnrolleeOutofPocketMOOPCostattheVoluntaryorManda") ?? String.Empty);
                    tierValue = rightOperand;
                    if (tierValue.Equals("Tier1"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval1") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval1") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval2") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.MedicarecoveredCopaymentCostSharingforTier1.BeginDayInterval2") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval2") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier2"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval1forTier3") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval1forTier3") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval2forTier3") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.BeginDayInterval2forTier3") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval2forTier3") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier3"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval1forTier3") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval1forTier3") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval2forTier3") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.BeginDayInterval2forTier3") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval2forTier3") ?? String.Empty);
                    }

                    if (moopValue == "1")
                    {
                        if (decimal.TryParse(copay1, out c1) && int.TryParse(endDay1, out ed1) && decimal.TryParse(copay2, out c2) && int.TryParse(beginDay2, out bd2) && int.TryParse(endDay2, out ed2))
                        {
                            bd2 = --bd2;
                            var totalDays = ed1 + (ed2 - bd2);
                            if (totalDays <= 60)
                            {
                                decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2));

                                if ((totalDays <= 15) && (totalCopay > 2075)) return false;
                                if ((totalDays <= 60) && (totalCopay > 2737)) return false;
                            }
                            else
                            {
                                if (ed1 >= 60)
                                {
                                    decimal totalCopay = c1 * 60;
                                    if (totalCopay > 2737) return false;

                                }
                                else
                                {
                                    var extraDays = 60 - ed1;
                                    decimal totalCopay = (c1 * ed1) + (c2 * extraDays);
                                    if (totalCopay > 2737) return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (decimal.TryParse(copay1, out c1) && int.TryParse(endDay1, out ed1) && decimal.TryParse(copay2, out c2) && int.TryParse(beginDay2, out bd2) && int.TryParse(endDay2, out ed2))
                        {
                            bd2 = --bd2;
                            var totalDays = ed1 + (ed2 - (--bd2));
                            if (totalDays <= 60)
                            {
                                decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2));

                                if ((totalDays <= 15) && (totalCopay > 1660)) return false;
                                if ((totalDays <= 60) && (totalCopay > 2190)) return false;
                            }
                            else
                            {
                                if (ed1 >= 60)
                                {
                                    decimal totalCopay = c1 * 60;
                                    if (totalCopay > 2190) return false;

                                }
                                else
                                {
                                    var extraDays = 60 - ed1;
                                    decimal totalCopay = (c1 * ed1) + (c2 * extraDays);
                                    if (totalCopay > 2190) return false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            { }
            return true;
        }

        private bool MOOPAmountValidationPsychiatricInterval3_2019(string leftOperand, string rightOperand, JObject _rowData)
        {
            string moopValue = String.Empty, tierValue = String.Empty, copay1 = String.Empty, endDay1 = String.Empty, copay2 = String.Empty, beginDay2 = String.Empty, endDay2 = String.Empty, copay3 = String.Empty, beginDay3 = String.Empty, endDay3 = String.Empty;
            decimal c1, c2, c3;
            int ed1, bd2, ed2, bd3, ed3;

            try
            {

                if (_rowData != null)
                {
                    moopValue = Convert.ToString(_rowData.SelectToken("SectionDPlanLevel.MaxEnrolleCostLimitInNetwork.IsyourInNetworkMaximumEnrolleeOutofPocketMOOPCostattheVoluntaryorManda") ?? String.Empty);
                    tierValue = rightOperand;
                    if (tierValue.Equals("Tier1"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval1") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval1") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval2") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.MedicarecoveredCopaymentCostSharingforTier1.BeginDayInterval2") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval2") ?? String.Empty);
                        copay3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.MedicarecoveredCopaymentCostSharingforTier1.CopaymentAmtInterval3") ?? String.Empty);
                        beginDay3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.MedicarecoveredCopaymentCostSharingforTier1.BeginDayInterval3") ?? String.Empty);
                        endDay3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase7.MedicarecoveredCopaymentCostSharingforTier1.EndDayInterval3") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier2"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier2.CopaymentAmtInterval1forTier2") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier2.BeginDayInterval1forTier2") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier2.CopaymentAmtInterval2forTier2") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier2.BeginDayInterval2forTier2") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier2.EndDayInterval2forTier2") ?? String.Empty);
                        copay3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier2.CopaymentAmtInterval3forTier2") ?? String.Empty);
                        beginDay3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier2.BeginDayInterval3forTier2") ?? String.Empty);
                        endDay3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier2.EndDayInterval3forTier2") ?? String.Empty);
                    }
                    if (tierValue.Equals("Tier3"))
                    {
                        copay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval1forTier3") ?? String.Empty);
                        endDay1 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval1forTier3") ?? String.Empty);
                        copay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval2forTier3") ?? String.Empty);
                        beginDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.BeginDayInterval2forTier3") ?? String.Empty);
                        endDay2 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval2forTier3") ?? String.Empty);
                        copay3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.CopaymentAmtInterval3forTier3") ?? String.Empty);
                        beginDay3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.BeginDayInterval3forTier3") ?? String.Empty);
                        endDay3 = Convert.ToString(_rowData.SelectToken("InpatientHospitalPsychiatric.InpatientPsychiatricBase8.MedicarecoveredCopaymentCostSharingforTier3.EndDayInterval3forTier3") ?? String.Empty);
                    }

                    if (moopValue == "1")
                    {
                        if (decimal.TryParse(copay1, out c1) && int.TryParse(endDay1, out ed1) && decimal.TryParse(copay2, out c2) && int.TryParse(beginDay2, out bd2) && int.TryParse(endDay2, out ed2) && decimal.TryParse(copay3, out c3) && int.TryParse(beginDay3, out bd3) && int.TryParse(endDay3, out ed3))
                        {
                            bd2 = --bd2; bd3 = --bd3;
                            var totalDays = ed1 + (ed2 - bd2) + (ed3 - bd3);
                            if (totalDays <= 60)
                            {
                                decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2)) + (c3 * (ed3 - bd3));
                                if ((totalDays <= 15) && (totalCopay > 2075)) return false;
                                if ((totalDays <= 60) && (totalCopay > 2737)) return false;
                            }
                            else
                            {
                                if (ed1 >= 60)
                                {
                                    decimal totalCopay = (c1 * 60);
                                    if (totalCopay > 2737) return false;
                                }
                                else if (ed1 + ed2 >= 60)
                                {
                                    var extraDays = 60 - ed1;

                                    decimal totalCopay = (c1 * ed1) + (c2 * extraDays);
                                    if (totalCopay > 2737) return false;
                                }
                                else
                                {
                                    var extraDays = 60 - (ed1 + ed2);

                                    decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2)) + (c3 * extraDays);
                                    if (totalCopay > 2737) return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (decimal.TryParse(copay1, out c1) && int.TryParse(endDay1, out ed1) && decimal.TryParse(copay2, out c2) && int.TryParse(beginDay2, out bd2) && int.TryParse(endDay2, out ed2) && decimal.TryParse(copay3, out c3) && int.TryParse(beginDay3, out bd3) && int.TryParse(endDay3, out ed3))
                        {
                            bd2 = --bd2; bd3 = --bd3;
                            var totalDays = ed1 + (ed2 - bd2) + (ed3 - bd3);
                            if (totalDays <= 60)
                            {
                                decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2)) + (c3 * (ed3 - bd3));
                                if ((totalDays <= 15) && (totalCopay > 1660)) return false;
                                if ((totalDays <= 60) && (totalCopay > 2190)) return false;
                            }
                            else
                            {
                                if (ed1 >= 60)
                                {
                                    decimal totalCopay = (c1 * 60);
                                    if (totalCopay > 2190) return false;
                                }
                                else if (ed1 + ed2 >= 60)
                                {
                                    var extraDays = 60 - ed1;

                                    decimal totalCopay = (c1 * ed1) + (c2 * extraDays);
                                    if (totalCopay > 2190) return false;
                                }
                                else
                                {
                                    var extraDays = 60 - (ed1 + ed2);

                                    decimal totalCopay = (c1 * ed1) + (c2 * (ed2 - bd2)) + (c3 * extraDays);
                                    if (totalCopay > 2190) return false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return true;
        }

        private bool MOOPAmountValidationSNF_2019(string leftOperand, string rightOperand, JObject _rowData)
        {
            decimal n1; int n2;
            string moopValue = String.Empty;
            bool isCoins = true;
            int voluntary20Value = 20, mandatory20Value = 0;
            decimal voluntary100Value = Convert.ToDecimal(172), mandaroty100Value = Convert.ToDecimal(172);

            try
            {
                if (_rowData != null)
                {
                    moopValue = Convert.ToString(_rowData.SelectToken("SectionDPlanLevel.MaxEnrolleCostLimitInNetwork.IsyourInNetworkMaximumEnrolleeOutofPocketMOOPCostattheVoluntaryorManda") ?? String.Empty);

                    if (!string.IsNullOrEmpty(leftOperand) && leftOperand.Contains("."))
                        isCoins = false;

                    if (decimal.TryParse(leftOperand, out n1) && int.TryParse(rightOperand, out n2) && !string.IsNullOrEmpty(moopValue))
                    {
                        if (moopValue == "1")
                        {
                            if (n2 <= 20)
                            {
                                if (isCoins && ((n1 / 100) * 518) > voluntary20Value) return false;
                                else if (!isCoins && (n1 > voluntary20Value)) return false;
                            }
                            else
                            {
                                if (isCoins && ((n1 / 100) * 518) > voluntary100Value) return false;
                                else if (!isCoins && (n1 > voluntary100Value)) return false;
                            }
                        }
                        else
                        {
                            if (n2 <= 20)
                            {
                                if (isCoins && ((n1 / 100) * 518) > mandatory20Value) return false;
                                else if (!isCoins && (n1 > mandatory20Value)) return false;
                            }
                            else
                            {
                                if (isCoins && ((n1 / 100) * 518) > mandaroty100Value) return false;
                                else if (!isCoins && (n1 > mandaroty100Value)) return false;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            { }
            return true;
        }
        private bool RepeaterDeductibleIsRequired(string leftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            //wellcare   string targetValue, groupID, isCopayment, pOSgroupID, isMedicare, posgroupIDGroup2, medicareisCopaymentgroup2, tierID, isDrugBenefitSelected;         
            String targetValue, groupID, isDeductible;
            targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);
            groupID = Convert.ToString(_rowData.SelectToken("OONGroupID") ?? string.Empty);
            var oONGroupRepeater = _formData.SelectToken("OONGroups.OONGroupsBase2").Where(x => Convert.ToString(x["OONGroupID"] ?? string.Empty) == groupID).FirstOrDefault();
            isDeductible = oONGroupRepeater != null ? Convert.ToString(oONGroupRepeater.SelectToken("IsthereanOONDeductibleforthisgroup")) : string.Empty;

            if (isDeductible == "1")
            {
                if (targetValue != "")
                {
                    return true;
                }
                return false;
            }

            return true;
        }
        private bool RepeaterOONDeductible(string leftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            String targetValue, groupID, isMedicare;
            targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);
            groupID = Convert.ToString(_rowData.SelectToken("OONGroupID") ?? string.Empty);
            var oONGroupRepeater = _formData.SelectToken("OONGroups.OONGroupsBase1").Where(x => Convert.ToString(x["OONGroupID"] ?? string.Empty) == groupID).FirstOrDefault();
            isMedicare = oONGroupRepeater != null ? Convert.ToString(oONGroupRepeater.SelectToken("SelectthebenefitsthatapplytotheOONGroups")) : string.Empty;
            string PlanType = _formData.SelectToken("SectionA.SectionA1.PlanType").ToString();
            if (PlanType == "4" || PlanType == "31")
            {
                if (isMedicare.Contains("01"))
                {
                    if (targetValue.Contains("2"))
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
            return true;
        }
        private bool RepeaterPlanMaxAmountCheck(string leftOperandName, string rightOperand, JObject _rowData, RuleDesign rule)
        {
            String targetValue, groupID, medicareCoveredServices = string.Empty, maxPlanBenefitCoverageValue = string.Empty, nonMedicareCoveredServices = string.Empty, serviceName = string.Empty;
            bool hasService = false;
            string[] selectedServices, dvhMedicareServicesList = { "16b", "17a", "17b", "18a" },
                dvhNonMedicareServicesList = { "16a", "16b", "17a", "17b", "18a", "18b" };
            try
            {
                targetValue = Convert.ToString(_rowData.SelectToken(rule.UIElementName) ?? String.Empty);

                int anchorformInstanceId = _formInstanceService.GetAnchorDocumentID(_formInstanceId);
                int anchorformDesignVersionId = _folderVersionServices.GetSourceFormDesignVersionId(Convert.ToInt32(anchorformInstanceId));
                FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(1, anchorformDesignVersionId, _formDesignServices);
                FormDesignVersionDetail anchorDetail = formDesignVersionMgr.GetFormDesignVersion(false);


                string sectionData = _formDataInstanceManager.GetSectionData(anchorformInstanceId, "SECTIONBSUMMARYOFPACKAGES", false, anchorDetail, true, false);
                JToken sectionDVH = JToken.Parse(sectionData);
                string dental = Convert.ToString(sectionDVH.SelectToken("SECTIONBSUMMARYOFPACKAGES.DentalBenefits.MaximumPlanbenefitcoverageappliesINonlyorbothINandOONPreventive") ?? String.Empty);
                string vision = Convert.ToString(sectionDVH.SelectToken("SECTIONBSUMMARYOFPACKAGES.VisionBenefits.MaximumPlanbenefitcoverageappliesINonlyorbothINandOONEyeExam") ?? String.Empty);
                string hearing = Convert.ToString(sectionDVH.SelectToken("SECTIONBSUMMARYOFPACKAGES.HearingBenefits.MaximumPlanbenefitcoverageappliesINonlyorbothINandOON") ?? String.Empty);

                groupID = Convert.ToString(_rowData.SelectToken("OONGroupID") ?? string.Empty);
                var oonGroupRepeater = _formData.SelectToken("OONGroups.OONGroupsBase1").Where(x => Convert.ToString(x["OONGroupID"] ?? string.Empty) == groupID).FirstOrDefault();
                medicareCoveredServices = oonGroupRepeater != null ? Convert.ToString(oonGroupRepeater.SelectToken("SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis")) : string.Empty;
                medicareCoveredServices = medicareCoveredServices.Replace("\r\n", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty).Replace("\"", string.Empty).Replace(" ", string.Empty);
                nonMedicareCoveredServices = oonGroupRepeater != null ? Convert.ToString(oonGroupRepeater.SelectToken("SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort")) : string.Empty;
                nonMedicareCoveredServices = nonMedicareCoveredServices.Replace("\r\n", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty).Replace("\"", string.Empty).Replace(" ", string.Empty);
                if (medicareCoveredServices.Length > 0)
                {
                    selectedServices = medicareCoveredServices.Split(',');
                    foreach (string serviceCode in dvhMedicareServicesList)
                    {
                        if (selectedServices.Contains(serviceCode))
                        {
                            hasService = true;
                            serviceName = serviceCode;
                            break;
                        }
                    }
                }
                else if (nonMedicareCoveredServices.Length > 0)
                {
                    selectedServices = nonMedicareCoveredServices.Split(',');
                    foreach (string serviceCode in dvhNonMedicareServicesList)
                    {
                        if (selectedServices.Contains(serviceCode))
                        {
                            hasService = true;
                            serviceName = serviceCode;
                            break;
                        }
                    }
                }
                if (hasService)
                {
                    if (serviceName == "16a" || serviceName == "16b")
                        maxPlanBenefitCoverageValue = dental; // get dental max plan benefit
                    if (serviceName == "17a" || serviceName == "17b")
                        maxPlanBenefitCoverageValue = vision; // get vision max plan benefit
                    if (serviceName == "18a" || serviceName == "18b")
                        maxPlanBenefitCoverageValue = hearing; // get hearing max plan benefit

                    if (maxPlanBenefitCoverageValue == "Both In-network and Out-of-network services" && targetValue != "")
                        return false;
                    else if (maxPlanBenefitCoverageValue == "In-network services only" && targetValue == "")
                        return false;
                }
                else
                    return true;
            }
            catch (Exception) { }
            return true;
        }

        private dynamic GetRepeaterDataFromElementName(string elementName)
        {
            string repeaterName = "";
            string[] nameParts = elementName.Split('.');
            JObject dataPart = _formData;
            JArray dataToReturn = null;
            foreach (string partName in nameParts)
            {
                var objData = dataPart[partName];
                if (objData != null)
                {
                    repeaterName = string.IsNullOrEmpty(repeaterName) ? partName : repeaterName + "." + partName;
                    if (objData is JArray)
                    {
                        dataToReturn = (JArray)objData;
                        break;
                    }
                    dataPart = (JObject)objData;
                }
            }
            return new { data = dataToReturn, repeaterName = repeaterName };
        }
    }

}
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.web.FormInstance;
using tmg.equinox.web.Framework.Caching;

namespace tmg.equinox.web.ExpresionBuilder
{
    public class ExpressionBuilderGetSetUtilities
    {

    }

    public static class COMMERCIALMEDICALGETSET
    {
        public static List<JToken> GetAdditionalServicesGrid(JObject data)
        {
            List<JToken> services = ((JArray)data[COMMERCIALMEDICAL.AdditionalServices][COMMERCIALMEDICAL.AdditionalServiceList]).ToList();
            return services;
        }

        public static void SetAdditionalServicesGrid(JObject sectionData, List<JToken> data)
        {
            sectionData[COMMERCIALMEDICAL.AdditionalServices][COMMERCIALMEDICAL.AdditionalServiceList] = JArray.FromObject(data);
        }


        public static List<JToken> GetBenefitReviewGrid(JObject data)
        {
            List<JToken> services = ((JArray)data[COMMERCIALMEDICAL.BenefitReview][COMMERCIALMEDICAL.BenefitReviewGrid]).ToList();
            return services;
        }

        public static void SetBenefitReviewGrid(JObject sectionData, List<JToken> data)
        {
            sectionData[COMMERCIALMEDICAL.BenefitReview][COMMERCIALMEDICAL.BenefitReviewGrid] = JArray.FromObject(data);
        }

        public static List<JToken> GetStandardServicesGrid(JObject data)
        {
            List<JToken> services = ((JArray)data[COMMERCIALMEDICAL.StandardServices][COMMERCIALMEDICAL.StandardServiceList]).ToList();
            return services;
        }

        public static List<JToken> GetNetworkTiers(JObject data)
        {
            List<JToken> list = ((JArray)data[COMMERCIALMEDICAL.Network][COMMERCIALMEDICAL.NetworkTierList]).ToList();
            return list;
        }

        public static List<JToken> GetCopayGrid(JObject data)
        {
            List<JToken> list = ((JArray)data[COMMERCIALMEDICAL.CostShare][COMMERCIALMEDICAL.CopaySec][COMMERCIALMEDICAL.CopayList]).ToList();
            return list;
        }

        public static List<JToken> GetCounsuranceGrid(JObject data)
        {
            List<JToken> list = ((JArray)data[COMMERCIALMEDICAL.CostShare][COMMERCIALMEDICAL.CoinsuranceSec][COMMERCIALMEDICAL.CoinsuranceList]).ToList();
            return list;
        }

        public static List<JToken> GetDeductibleGrid(JObject data)
        {
            List<JToken> list = ((JArray)data[COMMERCIALMEDICAL.CostShare][COMMERCIALMEDICAL.DeductibleSec][COMMERCIALMEDICAL.DeductibleList]).ToList();
            return list;
        }

        public static List<JToken> GetOOPMGrid(JObject data)
        {
            List<JToken> list = ((JArray)data[COMMERCIALMEDICAL.CostShare][COMMERCIALMEDICAL.OutofPocketMaximum][COMMERCIALMEDICAL.OutofPocketMaximumList]).ToList();
            return list;
        }

        public static List<JToken> GetPreventCostShareGrid(JObject data)
        {
            List<JToken> list = ((JArray)data[COMMERCIALMEDICAL.CostShare][COMMERCIALMEDICAL.PreventCostShare][COMMERCIALMEDICAL.PreventServicescovered]).ToList();
            return list;
        }

        public static List<JToken> GetReductionofBenefitGrid(JObject data)
        {
            List<JToken> list = ((JArray)data[COMMERCIALMEDICAL.CostShare][COMMERCIALMEDICAL.ReductionofBenefitsSec][COMMERCIALMEDICAL.WhatisthePlansReductionofBenefitAmount]).ToList();

            return list;
        }

        public static List<JToken> GetlimitsInformation(JObject data)
        {
            List<JToken> list = ((JArray)data[COMMERCIALMEDICAL.LimitSec][COMMERCIALMEDICAL.LimitInformationDetail]).ToList();
            return list;
        }

        public static List<JToken> GetlimitsSummary(JObject data)
        {
            List<JToken> list = ((JArray)data[COMMERCIALMEDICAL.LimitSec][COMMERCIALMEDICAL.LimitSummary]).ToList();
            return list;
        }

        public static string GetDoesthisplanhaveaReductionofBenefits(JObject data)
        {
            string list = data[COMMERCIALMEDICAL.CostShare][COMMERCIALMEDICAL.ReductionofBenefitsSec][COMMERCIALMEDICAL.DoesthisplanhaveaReductionofBenefits].ToString();
            return list;
        }

        public static JToken GetBasePlan(JObject data)
        {
            return (data[COMMERCIALMEDICAL.ProductRules][COMMERCIALMEDICAL.PlanInformation][COMMERCIALMEDICAL.BasePlanType]);
        }

        public static void SetRxRiderData(JObject sectionData, List<JToken> data)
        {
            sectionData[COMMERCIALMEDICAL.RxSelection][COMMERCIALMEDICAL.RxRiderSelection][COMMERCIALMEDICAL.RxRiderDetails] = "";
            sectionData[COMMERCIALMEDICAL.RxSelection][COMMERCIALMEDICAL.RxRiderSelection][COMMERCIALMEDICAL.SelectRxRider] = "";
        }

        public static void SetChiroData(JObject sectionData, List<JToken> data)
        {
            sectionData[COMMERCIALMEDICAL.RxSelection][COMMERCIALMEDICAL.ChiroRiderSelection][COMMERCIALMEDICAL.ChiroRiderDetails] = "";
            sectionData[COMMERCIALMEDICAL.RxSelection][COMMERCIALMEDICAL.ChiroRiderSelection][COMMERCIALMEDICAL.SelectChiroRider] = "";
        }

    }
    public static class MASTERLISTMEDICALRIDERGETSET
    {

        public static List<JToken> GetRXRider(JObject data)
        {
            List<JToken> list = ((JArray)data[MASTERLISTRIDEROPTION.RxRiderOption][MASTERLISTRIDEROPTION.RxRiderOptionList]).ToList();
            return list;
        }

        public static List<JToken> GetChiroRider(JObject data)
        {
            List<JToken> list = ((JArray)data[MASTERLISTRIDEROPTION.ChiroRiderOption][MASTERLISTRIDEROPTION.ChiroRiderOptionList]).ToList();
            return list;
        }

        public static List<JToken> GetDVHPackage(JObject data, string sectionName, string basePlanAssociationName)
        {
            List<JToken> list = ((JArray)data[sectionName][basePlanAssociationName]).ToList();
            return list;
        }
    }
}
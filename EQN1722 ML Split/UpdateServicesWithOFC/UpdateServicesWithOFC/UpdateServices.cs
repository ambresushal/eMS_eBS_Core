using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Xml;

namespace UpdateServicesWithOVC
{
    class UpdateServices
    {
        #region Variable Declaration
        public static string MedicalServices = "MedicalServices.Services";
        public static string Limits = "Limits.LimitList";
        public static string StandardLimits = "Limits.StandardLimits";
        public static string OfficeVisitCategory = "OfficeVisitCategory";
        public static string OfficeVisitCategoryValue = "Office Visit Charge";
        public static string OfficeVisitChargePCP = "Office Visit Charge - PCP";
        public static string OfficeVisitChargeOnly = "OfficeVisitChargeOnly";
        public static string OfficeVisitChargeSpecialist = "Office Visit Charge - Specialist";
        public static string ApplyCopayType = "ApplyCopayType";
        public static string PCP = "PCP";
        public static string Specialist = "Specialist";
        public static string Json = string.Empty;
        public static string connectionString = string.Empty;
        public static int ToBeUpdatedCount = 0;
        public static int UpdatedCount = 0; // BenefitReviewGrid AdditionalServices

        public static string AdditionalServices = "AdditionalServices.AdditionalServicesList";
        public static string BenefitCategory1 = "BenefitCategory1";  // "Physicians Office Visit";
        public static string BenefitCategory2 = "BenefitCategory2"; //  "PCP";
        public static string BenefitCategory3 = "BenefitCategory3";    //string.Empty;
        public static string PlaceOfService = "PlaceOfService";             //  string.Empty;

        public static string BenefitReviewGrid = "BenefitReview.BenefitReviewGrid";
        public static List<JToken> BenefitReviewGridData = new List<JToken>();


        public static string MasterList = "MasterList";
        public static string Product = "Product";
        public static int MasterListFormInstanceID = 0;
        public static int ProductFormInstanceID = 0;

        public static string StandardServices_MasterList = "StandardServiceDetails.StandardServiceDetailsList";    // Standard Services in Master List
        public static string StandardServices_Product = "StandardServices.StandardServiceList";    // Standard Services in Product
        public static string MandateName = "MandateName";
        public static string ServiceList = "ServiceList";
        public static string GrandFathered = "Grandfathered";
        public static string NonGrandFathered = "Non-grandfathered";
        public static List<JToken> GrandFatheredList = new List<JToken>();
        public static List<JToken> NonGrandFatheredList = new List<JToken>();
        public static string MasterListMedicalServices = "MasterListMedicalServices";

        public static string Copay = "Copay";
        public static string Copays = "CostShare.Copay";
        public static string Doesthisplanhaveacopay = "Doesthisplanhaveacopay";
        public static string CopayList = "CopayList";
        public static string CopayType = "CopayType";  //  IQMedicalPlanNetwork
        public static string IQMedicalPlanNetwork = "IQMedicalPlanNetwork";
        public static string Amount = "Amount";
        public static string CostShareTier = "CostShareTier";
        public static List<JToken> CopayValues = new List<JToken>();

        public static JToken MEDICALSERVICES = null;
        public static JToken STANDARDSERVICES_GF = null;
        public static JToken STANDARDSERVICES_NonGF = null;
        public static JToken STANDARDLIMITS = null;
        public static JToken ADDITIONALSERVICES = null;
        public static JToken BenefitReviewData = null;

        public static string StandardServiceToBeDeletedFromMasterList = "Psychiatric  (Inpatient)";
        public static string StandardServiceToBeAddedToMasterList = "Psychiatric (Inpatient)";

        public static string PlanDocumentsDetails = "GroupInformation.GeneralGroupInformation.PlanDocumentDetails";
        public static string cobretirecoverage = "CoordinationofBenefits.COBMedicare";
        public static string injurymalformationpath = "PlanSpecificServicesAH.CosmeticSurgery";
        public static string precertificationddvalues = "Precertification.InpatientHospitalPreCertificationReview.InpatientHospitalPreCertificationPenalty";
        public static string ClaimInterventionDetails = "Fees.FeeInformation.ClaimInterventionFees";

        #region EQN-1622
        public static string AdminNetworks_ML = "AdminNetwork.AdminNetworkList";
        public static string MedicalNetworks_ML = "MedicalNetwork.MedicalNetworkList"; // 
        public static string MedicalSubNetworks_ML = "MedicalNetwork.MedicalSubNetworkList";
        public static string MedicalNetworkDetailList_ML = "MedicalNetwork.MedicalNetworkDetailList";
        public static string AdminVendorTypes_ML = "AdminVendor.VendorTypeList";
        public static string AdminVendorOptions_ML = "AdminVendor.VendorOptionList";
        public static string AdminVendorTypeOptionMappings_ML = "AdminVendor.VendorMappingList";
        public static string NewAdminVendorType_ML = "Networks";
        #endregion


        public static StringBuilder Result = new StringBuilder("");



        #endregion



        static UpdateServices()
        {
            string conn = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
            connectionString = conn;   // @"Data Source=.;Initial Catalog=HSBPROD2;Persist Security Info=True;User ID=sa;Password=sa@123;MultipleActiveResultSets=True";
            EQN1557_Fill_MasterNetwork_AdminNetwork_AdminVendor_Lists();
            //connectionString = @"Data Source=TMGDESK037\SQLSERVER2012;Initial Catalog=HSB_Prod;Persist Security Info=True;User ID=sa;Password=sa@123;MultipleActiveResultSets=True"; 
        }

        #region EQN-1557 AdminNetworks AdminVendors MedicalNetworks JSONS Update in Master List and impacted Products

        #region Reference Variable Declaration.
        static string currentForm = string.Empty;
        static List<dynamic> AdminNetworks;
        static List<dynamic> AdminVendors;
        static List<dynamic> MedicalNetworks;

        static List<dynamic> MedicalNetworks_ToAdd;
        static List<dynamic> MedicalNetworks_ToDelete;
        static List<dynamic> MedicalNetworks_ToUpdate;
        static List<dynamic> AdminNetworks_ToAdd;
        static List<dynamic> AdminNetworks_ToDelete;
        static List<dynamic> AdminNetworks_ToUpdate;
        static List<dynamic> AdminVendorType_ToAdd;
        static List<dynamic> AdminVendorType_ToUpdate;
        static List<dynamic> AdminVendorOption_ToAdd;
        static List<dynamic> AdminVendorOption_ToUpdate;
        #endregion

        // MedicalNetworks_ToAdd  MedicalNetworks_ToDelete  MedicalNetworks_ToUpdate AdminNetworks_ToAdd  AdminNetworks_ToDelete  AdminNetworks_ToUpdate
        //  AdminVendorType_ToAdd   AdminVendorType_ToUpdate  AdminVendorOption_ToAdd  AdminVendorOption_ToUpdate

        public static void EQN1557_Update_Product()
        {
            Result.Append("\n\n-------------------------------Products----------------------------");
            List<string> FormsToUpdate = new List<string>() { "Medical", "Admin" };


            foreach (string Form in FormsToUpdate)
            {
                //if (Form == "Medical")
                //    continue;

                currentForm = Form;
                List<int> Products = new List<int>();
                string allProducts = @"select ins.FormInstanceID from ui.formdesign dess
                                 inner join fldr.FormInstance ins on dess.FormID=ins.FormDesignID
                                 inner join fldr.FormInstanceDataMap insdata on insdata.FormInstanceID=ins.FormInstanceID
                                 where FormName = '" + Form + "' and dess.IsActive=1 and ins.IsActive=1";

                //allProducts += " and ins.FormInstanceID in (931, 935)";

                string currentProduct = "select FormInstanceID,formdata from  fldr.forminstancedatamap where forminstanceid={0}";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand(allProducts, conn))
                    {
                        conn.Open();
                        using (SqlDataReader rd = comm.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                Products.Add((int)rd[0]);
                            }
                        }
                    }
                }

                foreach (var product in Products)
                {
                    if (product == 869)
                    {
                    }

                    Result.Append("\n");
                    GetJsonFromDB(string.Format(currentProduct, product), Product);
                    JObject RootData = JObject.Parse(Json);
                    string JSON_String = RootData.ToString(Newtonsoft.Json.Formatting.Indented);

                    if (currentForm == "Medical")
                        EQN1557_Update_MedicalProducts(RootData);
                    else
                        EQN1557_Update_AdminProducts(RootData);

                    string updatedData = "'" + RootData.ToString(Newtonsoft.Json.Formatting.None).Replace("'", "''") + "'";

                    updatedData = "update  fldr.forminstancedatamap set formdata=" + updatedData + " where forminstanceid= " + ProductFormInstanceID;
                    UpdateJsonInDB(updatedData);
                    string finsihed = string.Empty;
                }
            }
        }

        public static void EQN1557_Update_MasterList()
        {
            List<int> Products = new List<int>();
            string allProducts = @"select top 1 ins.FormInstanceID from ui.formdesign deg
                                    inner join ui.FormDesignVersion degver on deg.FormID = degver.FormDesignID
                                    inner join fldr.FormInstance ins on ins.FormDesignID = deg.FormID and degver.FormDesignVersionID= ins.FormDesignVersionID
                                    inner join fldr.FormInstanceDataMap dat on ins.FormInstanceID = dat.FormInstanceID
                                    inner join [UI].[Status] sta on sta.StatusID =degver.StatusID
                                    where deg.FormName = 'MasterList' and ins.IsActive=1
                                    order by ins.FormInstanceID desc";

            //allProducts += " and ins.FormInstanceID in (931, 935)";

            string currentProduct = "select FormInstanceID,formdata from  fldr.forminstancedatamap where forminstanceid={0}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = new SqlCommand(allProducts, conn))
                {
                    conn.Open();
                    using (SqlDataReader rd = comm.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            Products.Add((int)rd[0]);
                        }
                    }
                }
            }

            foreach (var product in Products)
            {
                Result.Append("\n");
                GetJsonFromDB(string.Format(currentProduct, product), MasterList);
                JObject RootData = JObject.Parse(Json);
                string JSON_String = RootData.ToString(Newtonsoft.Json.Formatting.Indented);

                EQN1557_UpdateMasterList_MedicalNetworkSection(RootData);
                EQN1557_UpdateMasterList_AdminNetworkSection(RootData);
                EQN1557_UpdateMasterList_AdminVendorSection(RootData);

                string updatedData = "'" + RootData.ToString(Newtonsoft.Json.Formatting.None).Replace("'", "''") + "'";

                updatedData = "update  fldr.forminstancedatamap set formdata=" + updatedData + " where forminstanceid= " + MasterListFormInstanceID;
                UpdateJsonInDB(updatedData);
                string finsihed = string.Empty;
            }

        }

        public static void EQN1557_Update_MedicalProducts(JObject RootData)
        {
            string SelectthePlansNetworksPath = "Network.NetworkInformation.SelectthePlansNetworks";
            string NetworkClaimsMailingAddressPath = "Network.NetworkInformation.NetworkClaimsMailingAddress";
            string SelectthePlansCostShareTiersPath = "Network.NetworkInformation.SelectthePlansCostShareTiers";
            string StandardlyAddedNetworksListPath = "Network.StandardlyAddedNetworks.StandardlyAddedNetworksList";

            List<string> NetworkRepeatersToUpdateFromMedicalDoc = new List<string>();
            NetworkRepeatersToUpdateFromMedicalDoc.Add(SelectthePlansNetworksPath);
            NetworkRepeatersToUpdateFromMedicalDoc.Add(NetworkClaimsMailingAddressPath);
            NetworkRepeatersToUpdateFromMedicalDoc.Add(StandardlyAddedNetworksListPath);

            #region Updates and Deletes To { SelectthePlansNetworks, NetworkClaimsMailingAddress, StandardlyAddedNetworksList} Repeaters in Medical Products.
            foreach (var repeaterPath in NetworkRepeatersToUpdateFromMedicalDoc)
            {
                var RepeaterData = RootData.SelectToken(repeaterPath);
                if (null != RepeaterData && RepeaterData.Any())
                {
                    Result.Append("\n ======     " + repeaterPath + "      ======\n");
                    Result.Append("\n UPDATES: ");
                    foreach (var rec in MedicalNetworks_ToUpdate)
                    {
                        var RecordsToUpdate = RepeaterData.Where(i => Convert.ToString(i.SelectToken("MasterNetworkName")) == rec.ChangeRequest).FirstOrDefault();
                        if (null != RecordsToUpdate)
                        {
                            Result.Append(" \n {" + RecordsToUpdate["MasterNetworkName"] + "} Updated To {" + rec.NetworkName + "}");
                            RecordsToUpdate["MasterNetworkName"] = rec.NetworkName;
                        }
                    }

                    Result.Append("\n\n DELETES: ");
                    foreach (var rec in MedicalNetworks_ToDelete)
                    {
                        var RecordsToDelete = RepeaterData.Where(i => Convert.ToString(i.SelectToken("MasterNetworkName")) == rec.NetworkName).FirstOrDefault();
                        if (null != RecordsToDelete)
                        {
                            Result.Append(" \n Record with MasterNetworkName =  {" + RecordsToDelete["MasterNetworkName"] + "}  Deleted.");
                            RecordsToDelete.Remove();
                        }
                    }
                }
            }
            #endregion

            #region SelectthePlansCostShareTiers Updates and Deletes To Products.
            var SelectthePlansCostShareTiers = RootData.SelectToken(SelectthePlansCostShareTiersPath);

            if (null != SelectthePlansCostShareTiers && SelectthePlansCostShareTiers.Any())
            {
                Result.Append("\n\n ======     " + SelectthePlansCostShareTiersPath + "      ======\n");
                Result.Append("\n UPDATES: ");
                foreach (var rec in MedicalNetworks_ToUpdate)
                {
                    foreach (var networks in SelectthePlansCostShareTiers)
                    {
                        var IQMedicalNetWorkList = networks["IQMedicalNetWorkList"];
                        if (IQMedicalNetWorkList != null)
                        {
                            foreach (var ntwk in IQMedicalNetWorkList)
                            {
                                if (Convert.ToString(ntwk["MasterNetworkName"]) == rec.ChangeRequest)
                                {
                                    Result.Append(" \n {" + ntwk["MasterNetworkName"] + "} Updated To {" + rec.NetworkName + "}");
                                    ntwk["MasterNetworkName"] = rec.NetworkName;
                                }
                                //var ntwkRec = ntwk.Where(i => Convert.ToString(i.SelectToken("MasterNetworkName")) == rec.ChangeRequest).FirstOrDefault();
                                //if (null != ntwkRec)
                                //{
                                //    Result.Append(" \n {" + ntwkRec["MasterNetworkName"] + "} Updated To {" + rec.NetworkName + "}");
                                //    ntwkRec["MasterNetworkName"] = rec.NetworkName;
                                //}
                            }
                        }
                    }
                }

                Result.Append("\n\n DELETES: ");
                foreach (var rec in MedicalNetworks_ToDelete)
                {
                    foreach (var networks in SelectthePlansCostShareTiers)
                    {
                        var IQMedicalNetWorkList = networks["IQMedicalNetWorkList"];
                        if (IQMedicalNetWorkList != null)
                        {
                            var ntwkRec = IQMedicalNetWorkList.Where(i => Convert.ToString(i.SelectToken("MasterNetworkName")) == rec.NetworkName).FirstOrDefault();
                            if (null != ntwkRec)
                            {
                                Result.Append(" \n Record with MasterNetworkName =  {" + ntwkRec["MasterNetworkName"] + "}  Deleted.");
                                ntwkRec.Remove();
                            }
                        }
                    }
                }
            }
            #endregion

            //#region StandardlyAddedNetworksList Updates and Deletes To Products.

            //var StandardlyAddedNetworksList = RootData.SelectToken(StandardlyAddedNetworksListPath);
            //if (null != StandardlyAddedNetworksList && StandardlyAddedNetworksList.Any())
            //{
            //    foreach (var rec in MedicalNetworks_ToUpdate)
            //    {
            //        var RecordsToUpdate = StandardlyAddedNetworksList.Where(i => Convert.ToString(i.SelectToken("MasterNetworkName")) == rec.ChangeRequest).FirstOrDefault();
            //        if (null != RecordsToUpdate)
            //            RecordsToUpdate["MasterNetworkName"] = rec.NetworkName;
            //    }

            //    foreach (var rec in MedicalNetworks_ToDelete)
            //    {
            //        var RecordsToDelete = StandardlyAddedNetworksList.Where(i => Convert.ToString(i.SelectToken("MasterNetworkName")) == rec.ChangeRequest).FirstOrDefault();
            //        if (null != RecordsToDelete)
            //            RecordsToDelete.Remove();
            //    }
            //}
            //#endregion



            //#region NetworkClaimsMailingAddress Updates and Deletes To Products.
            //var NetworkClaimsMailingAddress = RootData.SelectToken(NetworkClaimsMailingAddressPath);
            //if (null != NetworkClaimsMailingAddress && NetworkClaimsMailingAddress.Any())
            //{
            //    foreach (var rec in MedicalNetworks_ToUpdate)
            //    {
            //        var RecordsToUpdate = NetworkClaimsMailingAddress.Where(i => Convert.ToString(i.SelectToken("MasterNetworkName")) == rec.ChangeRequest).FirstOrDefault();
            //        if (null != RecordsToUpdate)
            //            RecordsToUpdate["MasterNetworkName"] = rec.NetworkName;
            //    }

            //    foreach (var rec in MedicalNetworks_ToDelete)
            //    {
            //        var RecordsToDelete = NetworkClaimsMailingAddress.Where(i => Convert.ToString(i.SelectToken("MasterNetworkName")) == rec.ChangeRequest).FirstOrDefault();
            //        if (null != RecordsToDelete)
            //            RecordsToDelete.Remove();
            //    }
            //}
            //#endregion

            //#region SelectthePlansNetworks Updates and Deletes To Products.
            //var SelectthePlansNetworks = RootData.SelectToken(SelectthePlansNetworksPath);
            //if (null != SelectthePlansNetworks && SelectthePlansNetworks.Any())
            //{
            //    foreach (var rec in MedicalNetworks_ToUpdate)
            //    {
            //        var RecordsToUpdate = SelectthePlansNetworks.Where(i => Convert.ToString(i.SelectToken("MasterNetworkName")) == rec.ChangeRequest).FirstOrDefault();
            //        if (null != RecordsToUpdate)
            //            RecordsToUpdate["MasterNetworkName"] = rec.NetworkName;
            //    }

            //    foreach (var rec in MedicalNetworks_ToDelete)
            //    {
            //        var RecordsToDelete = SelectthePlansNetworks.Where(i => Convert.ToString(i.SelectToken("MasterNetworkName")) == rec.ChangeRequest).FirstOrDefault();
            //        if (null != RecordsToDelete)
            //            RecordsToDelete.Remove();
            //    }
            //}
            //#endregion
        }

        public static void EQN1557_Update_AdminProducts(JObject RootData)
        {
            string VendorListPath = "Vendors.Vendor.VendorList";
            string NetworkFeesPath = "Fees.FeeInformation.NetworkFees";

            var VendorList = RootData.SelectToken(VendorListPath);
            var NetworkFees = RootData.SelectToken(NetworkFeesPath);

            #region NetworkFees Updates and Deletes in the Product.
            if (NetworkFees != null && NetworkFees.Any())
            {
                //MasterNetworkName          VendorName        NetworkName     ChangeRequest
                Result.Append("\n ======     " + NetworkFeesPath + "      ======\n");
                Result.Append("\n UPDATES: ");
                foreach (var rec in AdminNetworks_ToUpdate)
                {
                    var NetworkToBeUpdated = NetworkFees.Where(i => Convert.ToString(i.SelectToken("MasterNetworkName")) == rec.ChangeRequest).FirstOrDefault(); //AdminVendorType_ToUpdate
                    if (NetworkToBeUpdated != null)
                    {
                        Result.Append(" \n {" + NetworkToBeUpdated["MasterNetworkName"] + "} Updated To {" + rec.NetworkName + "}");
                        NetworkToBeUpdated["MasterNetworkName"] = rec.NetworkName;
                    }
                }

                Result.Append("\n\n DELETES: ");
                foreach (var rec in AdminNetworks_ToDelete)
                {
                    var NetworkToBeDeleted = NetworkFees.Where(i => Convert.ToString(i.SelectToken("MasterNetworkName")) == rec.NetworkName).FirstOrDefault(); //AdminVendorType_ToUpdate
                    if (NetworkToBeDeleted != null)
                    {
                        Result.Append(" \n Record with MasterNetworkName =  {" + NetworkToBeDeleted["MasterNetworkName"] + "}  Deleted.");
                        NetworkToBeDeleted.Remove();
                    }
                }

            }
            #endregion

            #region VendorList Updates in the Product.
            if (VendorList != null && VendorList.Any())
            {
                Result.Append("\n ======     " + VendorListPath + "      ======\n");
                Result.Append("\n UPDATES: ");
                // VendorOption
                foreach (var rec in AdminVendorOption_ToUpdate)
                {
                    string VendorType = "";
                    string VendorOption = "";
                    if (rec.ChangeRequest.Contains(";;"))
                    {
                        string[] tt = rec.ChangeRequest.Split(new string[] { ";;" }, StringSplitOptions.None);
                        VendorType = tt.Count() > 1 ? tt[0] : rec.VendorType;//tt.Count() > 1 ? tt[0] : rec.VendorOption;
                        VendorOption = tt.Count() > 1 ? tt[1] : rec.ChangeRequest;// rec.VendorOption;
                    }
                    else
                    {
                        VendorType = rec.VendorType;
                        VendorOption = rec.ChangeRequest;
                    }

                    var RecordsWithVendorOptionToBeUpdated = VendorList.Where(i => (Convert.ToString(i.SelectToken("VendorName")) ?? "").Trim() == VendorOption.Trim() && (Convert.ToString(i.SelectToken("VendorType")) ?? "").Trim() == VendorType.Trim()).FirstOrDefault(); //AdminVendorType_ToUpdate
                    if (RecordsWithVendorOptionToBeUpdated != null)
                    {
                        Result.Append(" \n {" + RecordsWithVendorOptionToBeUpdated["VendorName"] + "}   VendorOption  Updated To  {" + rec.VendorOption + "}");
                        RecordsWithVendorOptionToBeUpdated["VendorName"] = rec.VendorOption;
                    }
                }

                // VendorType
                foreach (var rec in AdminVendorType_ToUpdate)
                {

                    string VendorType = "";
                    string VendorOption = "";
                    if (rec.ChangeRequest.Contains(";;"))
                    {
                        string[] tt = rec.ChangeRequest.Split(new string[] { ";;" }, StringSplitOptions.None);
                        VendorType = tt.Count() > 1 ? tt[0] : rec.ChangeRequest;//tt.Count() > 1 ? tt[0] : rec.VendorOption;
                        VendorOption = rec.VendorOption;  //  tt.Count() > 1 ? tt[1] : rec.VendorOption;
                    }
                    else
                    {
                        VendorType = rec.VendorType;
                        VendorOption = rec.ChangeRequest;
                    }

                    var RecordsWithVendorTypeToBeUpdated = VendorList.Where(i => (Convert.ToString(i.SelectToken("VendorName")) ?? "").Trim() == VendorOption.Trim() && (Convert.ToString(i.SelectToken("VendorType")) ?? "").Trim() == VendorType.Trim()).FirstOrDefault(); //AdminVendorType_ToUpdate
                    if (RecordsWithVendorTypeToBeUpdated != null)
                    {
                        Result.Append(" \n {" + RecordsWithVendorTypeToBeUpdated["VendorType"] + "}   VendorType  Updated To  {" + rec.VendorType + "}");
                        RecordsWithVendorTypeToBeUpdated["VendorType"] = rec.VendorType;
                    }
                }
            }
            #endregion
        }

        public static void EQN1557_Fill_MasterNetwork_AdminNetwork_AdminVendor_Lists()
        {
            #region AdminNetworks
            AdminNetworks = new List<dynamic>();
            AdminNetworks.Add(new { NetworkName = "AHA", ChangeRequest = "Add new" });
            AdminNetworks.Add(new { NetworkName = "AMCO - Full", ChangeRequest = "AMCO 1" });
            AdminNetworks.Add(new { NetworkName = "AMCO - Baptist Exclusive", ChangeRequest = "AMCO 2" });
            AdminNetworks.Add(new { NetworkName = "AMCO - Select", ChangeRequest = "AMCO 3" });
            AdminNetworks.Add(new { NetworkName = "APPO (Arkansas PPO)", ChangeRequest = "Add new" });
            AdminNetworks.Add(new { NetworkName = "BCBS 2", ChangeRequest = "Delete" });
            AdminNetworks.Add(new { NetworkName = "Cigna (Available at GHS)", ChangeRequest = "Delete" });
            AdminNetworks.Add(new { NetworkName = "Cigna (Not available at GHS)", ChangeRequest = "Delete" });
            AdminNetworks.Add(new { NetworkName = "Claim Doc (Ref Based Service)", ChangeRequest = "Claim Doc for facilities" });
            AdminNetworks.Add(new { NetworkName = "EHC (Employer's Health Choice)", ChangeRequest = "EHC 1" });
            AdminNetworks.Add(new { NetworkName = "EHC 2", ChangeRequest = "Delete" });
            AdminNetworks.Add(new { NetworkName = "FIRST HEALTH  STRATOSE NETWORK 2", ChangeRequest = "Delete" });
            AdminNetworks.Add(new { NetworkName = "FirstHealth (NC or SC)", ChangeRequest = "Delete" });
            AdminNetworks.Add(new { NetworkName = "HEALTHLINK", ChangeRequest = "HEALTHLINK 1" });
            AdminNetworks.Add(new { NetworkName = "HEALTHLINK 2", ChangeRequest = "Delete" });
            AdminNetworks.Add(new { NetworkName = "HEALTHREACH PREFERRED", ChangeRequest = "HEALTHREACH PREFERRED 1" });
            AdminNetworks.Add(new { NetworkName = "HEALTHREACH PREFERRED 2", ChangeRequest = "Delete" });
            AdminNetworks.Add(new { NetworkName = "HEALTHREACH PREFERRED 3", ChangeRequest = "Delete" });
            AdminNetworks.Add(new { NetworkName = "HST (Ref Based Service)", ChangeRequest = "Add new" });
            AdminNetworks.Add(new { NetworkName = "JRMC Hospital", ChangeRequest = "Delete" });
            AdminNetworks.Add(new { NetworkName = "Medcost - First Health (outside NC or SC)", ChangeRequest = "Delete" });
            AdminNetworks.Add(new { NetworkName = "Mississippi Physicians Care Network", ChangeRequest = "Add new" });
            AdminNetworks.Add(new { NetworkName = "OneNet", ChangeRequest = "Add new" });
            AdminNetworks.Add(new { NetworkName = "PPO Plus", ChangeRequest = "Add new" });
            AdminNetworks.Add(new { NetworkName = "Preferred One", ChangeRequest = "Add new" });
            AdminNetworks.Add(new { NetworkName = "Randolph Hospital", ChangeRequest = "Delete" });
            AdminNetworks.Add(new { NetworkName = "Reference Based Pricing", ChangeRequest = "Delete" });
            AdminNetworks.Add(new { NetworkName = "San Ysidro Health Care, Inc.", ChangeRequest = "Delete" });
            AdminNetworks.Add(new { NetworkName = "Self Regional (billed by)", ChangeRequest = "Delete" });
            AdminNetworks.Add(new { NetworkName = "SLVH Providers", ChangeRequest = "Delete" });
            AdminNetworks.Add(new { NetworkName = "SRHS-RHP-Greenville Pediatric-Medcost Subset", ChangeRequest = "Delete" });
            AdminNetworks.Add(new { NetworkName = "SuperMed PPO (MMO)", ChangeRequest = "Add new" });
            AdminNetworks.Add(new { NetworkName = "Univ of Toledo Med Center & Physicians", ChangeRequest = "Delete" });
            AdminNetworks.Add(new { NetworkName = "University Medical Center (NV)", ChangeRequest = "Delete" });
            AdminNetworks.Add(new { NetworkName = "Vanguard", ChangeRequest = "Add new" });
            AdminNetworks.Add(new { NetworkName = "Verity Health (Healthcare Highways)", ChangeRequest = "Add new" });
            AdminNetworks.Add(new { NetworkName = "Community Eye Care (Vision)", ChangeRequest = "Add new" });
            AdminNetworks.Add(new { NetworkName = "Cigna Dental", ChangeRequest = "Add new" });
            AdminNetworks.Add(new { NetworkName = "Aetna Dental", ChangeRequest = "Add new" });
            AdminNetworks.Add(new { NetworkName = "Guardian", ChangeRequest = "Add new" });
            AdminNetworks.Add(new { NetworkName = "NPPN", ChangeRequest = "Add new" });
            AdminNetworks.Add(new { NetworkName = "First Dental Health", ChangeRequest = "Add new" });
            AdminNetworks.Add(new { NetworkName = "Diversified Dental Network", ChangeRequest = "Add new" });
            #endregion

            #region AdminVendors
            AdminVendors = new List<dynamic>();
            AdminVendors.Add(new { VendorType = "Dental  (Fully Insured)", VendorOption = "Aetna Dental (fully insured)", ChangeRequest = "Aetna Dental", ChangeNeededFor = "VendorOption" });
            AdminVendors.Add(new { VendorType = "Dental  (Fully Insured)", VendorOption = "Cigna (fully insured)", ChangeRequest = "Cigna", ChangeNeededFor = "VendorOption" });
            AdminVendors.Add(new { VendorType = "Dental  (Fully Insured)", VendorOption = "Guardian (fully insured)", ChangeRequest = "Guardian", ChangeNeededFor = "VendorOption" });
            AdminVendors.Add(new { VendorType = "Vision (Fully Insured)", VendorOption = "Community Care Network  (fully insured)", ChangeRequest = "Vision Fully Insured", ChangeNeededFor = "VendorType" });
            AdminVendors.Add(new { VendorType = "Vision (Fully Insured)", VendorOption = "VSP (fully insured)", ChangeRequest = "Vision Fully Insured", ChangeNeededFor = "VendorType" });
            AdminVendors.Add(new { VendorType = "Pharmacy Benefit Management", VendorOption = "EcRx", ChangeRequest = "Add new", ChangeNeededFor = "VendorOption" });

            AdminVendors.Add(new { VendorType = "Dental  (Fully Insured)", VendorOption = "Aetna Dental (fully insured)", ChangeRequest = "Add new", ChangeNeededFor = "VendorOption" });
            AdminVendors.Add(new { VendorType = "Dental  (Fully Insured)", VendorOption = "Cigna (fully insured)", ChangeRequest = "Add new", ChangeNeededFor = "VendorOption" });
            AdminVendors.Add(new { VendorType = "Dental  (Fully Insured)", VendorOption = "Guardian (fully insured)", ChangeRequest = "Add new", ChangeNeededFor = "VendorOption" });



            //



            AdminVendors.Add(new { VendorType = "Pharmacy Benefit Management", VendorOption = "RxEdo", ChangeRequest = "Add new", ChangeNeededFor = "VendorOption" });
            AdminVendors.Add(new { VendorType = "Telehealth", VendorOption = "MDLive", ChangeRequest = "Add new", ChangeNeededFor = "VendorOption" });
            AdminVendors.Add(new { VendorType = "Vision Network (self funded)", VendorOption = "Community Eye Care Network (self-funded)", ChangeRequest = "Vision Network;;Community Care Network", ChangeNeededFor = "Both" });
            AdminVendors.Add(new { VendorType = "Vision Network (self-funded)", VendorOption = "VSP (self funded)", ChangeRequest = "Vision Network;;VSP", ChangeNeededFor = "Both" });
            AdminVendors.Add(new { VendorType = "Specialty Health Program", VendorOption = "Amplifon", ChangeRequest = "Add new", ChangeNeededFor = "Both" });
            #endregion

            #region MedicalNetworks
            MedicalNetworks = new List<dynamic>();
            MedicalNetworks.Add(new { NetworkName = "Claim Doc (facilitiy & physician)", ChangeRequest = "Claim Doc for facilities" });
            MedicalNetworks.Add(new { NetworkName = "Claim Doc (facilitiy only)", ChangeRequest = "Add new" });
            MedicalNetworks.Add(new { NetworkName = "Community Eye Care (Vision)", ChangeRequest = "Add new" });
            MedicalNetworks.Add(new { NetworkName = "EHC (Employers Health Choice)", ChangeRequest = "EHC" });
            MedicalNetworks.Add(new { NetworkName = "Employer Health Choice", ChangeRequest = "Delete" });
            MedicalNetworks.Add(new { NetworkName = "HST  (facility & physician)", ChangeRequest = "Add new" });
            MedicalNetworks.Add(new { NetworkName = "HST (facility only)", ChangeRequest = "Add new" });
            MedicalNetworks.Add(new { NetworkName = "Reference Based Pricing", ChangeRequest = "Delete" });
            MedicalNetworks.Add(new { NetworkName = "VSP (Vision)", ChangeRequest = "Add new" });
            #endregion

            MedicalNetworks_ToAdd = MedicalNetworks.Where(i => i.ChangeRequest == "Add new").ToList();
            MedicalNetworks_ToDelete = MedicalNetworks.Where(i => i.ChangeRequest == "Delete").ToList();
            MedicalNetworks_ToUpdate = MedicalNetworks.Where(i => i.ChangeRequest != "Add new" && i.ChangeRequest != "Delete").ToList();

            AdminNetworks_ToAdd = AdminNetworks.Where(i => i.ChangeRequest == "Add new").ToList();
            AdminNetworks_ToDelete = AdminNetworks.Where(i => i.ChangeRequest == "Delete").ToList();
            AdminNetworks_ToUpdate = AdminNetworks.Where(i => i.ChangeRequest != "Add new" && i.ChangeRequest != "Delete").ToList();

            AdminVendorType_ToAdd = AdminVendors.Where(i => i.ChangeRequest == "Add new" && (i.ChangeNeededFor == "VendorType" || i.ChangeNeededFor == "Both")).ToList();
            AdminVendorType_ToUpdate = AdminVendors.Where(i => i.ChangeRequest != "Add new" && i.ChangeRequest != "Delete" && (i.ChangeNeededFor == "VendorType" || i.ChangeNeededFor == "Both")).ToList();

            AdminVendorOption_ToAdd = AdminVendors.Where(i => i.ChangeRequest == "Add new").ToList();
            AdminVendorOption_ToUpdate = AdminVendors.Where(i => i.ChangeRequest != "Add new" && i.ChangeRequest != "Delete" && (i.ChangeNeededFor == "VendorOption" || i.ChangeNeededFor == "Both")).ToList();

            //#region AdminNetworks
            //AdminNetworks = new List<List<string>>();
            //AdminNetworks.Add(new List<string> { "AHA", "Add new" });
            //AdminNetworks.Add(new List<string> { "AMCO - Full", "AMCO 1" });
            //AdminNetworks.Add(new List<string> { "AMCO - Baptist Exclusive", "AMCO 2" });
            //AdminNetworks.Add(new List<string> { "AMCO - Select", "AMCO 3" });
            //AdminNetworks.Add(new List<string> { "APPO (Arkansas PPO)", "Add new" });
            //AdminNetworks.Add(new List<string> { "BCBS 2", "Delete" });
            //AdminNetworks.Add(new List<string> { "Cigna (Available at GHS)", "Delete" });
            //AdminNetworks.Add(new List<string> { "Cigna (Not available at GHS)", "Delete" });
            //AdminNetworks.Add(new List<string> { "Claim Doc (Ref Based Service)", "Claim Doc for facilities" });
            //AdminNetworks.Add(new List<string> { "EHC (Employer's Health Choice)", "EHC 1" });
            //AdminNetworks.Add(new List<string> { "EHC 2", "Delete" });
            //AdminNetworks.Add(new List<string> { "FIRST HEALTH  STRATOSE NETWORK 2", "Delete" });
            //AdminNetworks.Add(new List<string> { "FirstHealth (NC or SC)", "Delete" });
            //AdminNetworks.Add(new List<string> { "HEALTHLINK", "HEALTHLINK 1" });
            //AdminNetworks.Add(new List<string> { "HEALTHLINK 2", "Delete" });
            //AdminNetworks.Add(new List<string> { "HEALTHREACH PREFERRED", "HEALTHREACH PREFERRED 1" });
            //AdminNetworks.Add(new List<string> { "HEALTHREACH PREFERRED 2", "Delete" });
            //AdminNetworks.Add(new List<string> { "HEALTHREACH PREFERRED 3", "Delete" });
            //AdminNetworks.Add(new List<string> { "HST (Ref Based Service)", "Add new" });
            //AdminNetworks.Add(new List<string> { "JRMC Hospital", "Delete" });
            //AdminNetworks.Add(new List<string> { "Medcost - First Health (outside NC or SC)", "Delete" });
            //AdminNetworks.Add(new List<string> { "Mississippi Physicians Care Network", "Add new" });
            //AdminNetworks.Add(new List<string> { "OneNet", "Add new" });
            //AdminNetworks.Add(new List<string> { "PPO Plus", "Add new" });
            //AdminNetworks.Add(new List<string> { "Preferred One", "Add new" });
            //AdminNetworks.Add(new List<string> { "Randolph Hospital", "Delete" });
            //AdminNetworks.Add(new List<string> { "Reference Based Pricing", "Delete" });
            //AdminNetworks.Add(new List<string> { "San Ysidro Health Care, Inc.", "Delete" });
            //AdminNetworks.Add(new List<string> { "Self Regional (billed by)", "Delete" });
            //AdminNetworks.Add(new List<string> { "SLVH Providers", "Delete" });
            //AdminNetworks.Add(new List<string> { "SRHS-RHP-Greenville Pediatric-Medcost Subset", "Delete" });
            //AdminNetworks.Add(new List<string> { "SuperMed PPO (MMO)", "Add new" });
            //AdminNetworks.Add(new List<string> { "Univ of Toledo Med Center & Physicians", "Delete" });
            //AdminNetworks.Add(new List<string> { "University Medical Center (NV)", "Delete" });
            //AdminNetworks.Add(new List<string> { "Vanguard", "Add new" });
            //AdminNetworks.Add(new List<string> { "Verity Health (Healthcare Highways)", "Add new" });
            //AdminNetworks.Add(new List<string> { "Community Eye Care (Vision)", "Add new" });
            //AdminNetworks.Add(new List<string> { "Cigna Dental", "Add new" });
            //AdminNetworks.Add(new List<string> { "Aetna Dental", "Add new" });
            //AdminNetworks.Add(new List<string> { "Guardian", "Add new" });
            //AdminNetworks.Add(new List<string> { "NPPN", "Add new" });
            //AdminNetworks.Add(new List<string> { "First Dental Health", "Add new" });
            //AdminNetworks.Add(new List<string> { "Diversified Dental Network", "Add new" });
            //#endregion

            //#region AdminVendors
            //AdminVendors = new List<List<string>>();
            //AdminVendors.Add(new List<string> { "Dental  (Fully Insured)", "Aetna Dental (fully insured)", "Aetna Dental", "VendorOption" });
            //AdminVendors.Add(new List<string> { "Dental  (Fully Insured)", "Cigna (fully insured)", "Cigna", "VendorOption" });
            //AdminVendors.Add(new List<string> { "Dental  (Fully Insured)", "Guardian (fully insured)", "Guardian", "VendorOption" });
            //AdminVendors.Add(new List<string> { "Vision (Fully Insured)", "Community Care Network  (fully insured)", "Vision Fully Insured", "VendorType" });
            //AdminVendors.Add(new List<string> { "Vision (Fully Insured)", "VSP (fully insured)", "Vision Fully Insured", "VendorType" });
            //AdminVendors.Add(new List<string> { "Pharmacy Benefit Management", "EcRx", "Add new", "" });
            //AdminVendors.Add(new List<string> { "Pharmacy Benefit Management", "RxEdo", "Add new", "" });
            //AdminVendors.Add(new List<string> { "Telehealth", "MDLive", "Add new", "" });
            //AdminVendors.Add(new List<string> { "Vision Network (self funded)", "Community Eye Care Network (self-funded)", "Vision Network & Community Care Network", "Both" });
            //AdminVendors.Add(new List<string> { "Vision Network (self-funded)", "VSP (self funded)", "Vision Network & VSP", "Both" });
            //AdminVendors.Add(new List<string> { "Specialty Health Program", "Amplifon", "Add new", "" });
            //#endregion

            //#region MedicalNetworks
            //MedicalNetworks = new List<dynamic>();
            //MedicalNetworks.Add(new List<string> { "Claim Doc (facilitiy & physician)", "Claim Doc for facilities" });
            //MedicalNetworks.Add(new List<string> { "Claim Doc (facilitiy only)", "Add new" });
            //MedicalNetworks.Add(new List<string> { "Community Eye Care (Vision)", "Add new" });
            //MedicalNetworks.Add(new List<string> { "EHC (Employers Health Choice)", "EHC" });
            //MedicalNetworks.Add(new List<string> { "Employer Health Choice", "Delete" });
            //MedicalNetworks.Add(new List<string> { "HST  (facility & physician)", "Add new" });
            //MedicalNetworks.Add(new List<string> { "HST (facility only)", "Add new" });
            //MedicalNetworks.Add(new List<string> { "Reference Based Pricing", "Delete" });
            //MedicalNetworks.Add(new List<string> { "VSP (Vision)", "Add new" });
            //#endregion
        }

        public static void EQN1557_UpdateMasterList_MedicalNetworkSection(JObject RootData)
        {
            string MedicalNetworkListPath = "MedicalNetwork.MedicalNetworkList";  // MedicalSubNetworkList
            string MedicalSubNetworkListPath = "MedicalNetwork.MedicalSubNetworkList";
            string MedicalNetworkDetailListPath = "MedicalNetwork.MedicalNetworkDetailList";

            string templateMedicalNetworkDet = @"			{
				""MasterNetworkName"": """",
				""MasterNumber"": """",
				""IsManualSelect"": ""No"",
				""IsStandardlyaddedNetwork"": ""No"",
				""SelectSubNetwork"": ""No"",
				""Address"": """",
				""City"": """",
				""State"": """",
				""Zip"": """",
				""EDINumber"": """",
				""Website"": """",
				""MasterListSubNetworkList"": [],
				""RowIDProperty"": 0
			    }";



            var MedicalNetworkList = RootData.SelectToken(MedicalNetworkListPath);
            var MedicalSubNetworkList = RootData.SelectToken(MedicalSubNetworkListPath);
            var MedicalNetworkDetailList = RootData.SelectToken(MedicalNetworkDetailListPath);



            List<int> MedNetL_RowIDProperties = MedicalNetworkList.Select(i => string.IsNullOrEmpty(Convert.ToString(i.SelectToken("RowIDProperty"))) ? 0 : Convert.ToInt32(i.SelectToken("RowIDProperty"))).ToList();
            var MedNetL_RowID = MedNetL_RowIDProperties.OrderByDescending(o => o).ToList().FirstOrDefault();

            List<int> MedNetDetL_RowIDProperties = MedicalNetworkDetailList.Select(i => string.IsNullOrEmpty(Convert.ToString(i.SelectToken("RowIDProperty"))) ? 0 : Convert.ToInt32(i.SelectToken("RowIDProperty"))).ToList();
            var MedNetDetL_RowID = MedNetDetL_RowIDProperties.OrderByDescending(o => o).ToList().FirstOrDefault();

            Result.Append("\n ======     " + MedicalNetworkListPath + "; " + MedicalNetworkDetailListPath + "      ======\n");
            Result.Append("\n ADDITIONS : ");

            #region MedicalNetworks_ToAdd
            foreach (var network in MedicalNetworks_ToAdd)
            {
                string RowIDProperty = "";

                if (null != MedicalNetworkList && MedicalNetworkList.Any())
                {
                    if (!MedicalNetworkList.Where(i => (Convert.ToString(i.SelectToken("MasterNetworkName")) ?? "").Trim() == network.NetworkName.Trim()).Any())
                    {
                        if (MedNetL_RowID != null)
                        {
                            MedNetL_RowID++;
                            RowIDProperty = Convert.ToString(MedNetL_RowID);
                        }
                        MedicalNetworkList[0].AddAfterSelf(JObject.Parse("{'MasterNetworkName': '" + network.NetworkName + "', 'MasterNumber': '', 'RowIDProperty': '" + RowIDProperty + "'}"));
                        Result.Append(" \n New Record with {" + network.NetworkName + "}  added to Medical Network List.");
                    }
                    else
                    {
                        Result.Append(" \n Record already Exists.");
                    }


                }


                if (null != MedicalNetworkDetailList && MedicalNetworkDetailList.Any())
                {
                    if (!MedicalNetworkDetailList.Where(i => (Convert.ToString(i.SelectToken("MasterNetworkName")) ?? "").Trim() == network.NetworkName.Trim()).Any())
                    {
                        JObject templateMedicalNetworkDetOBJ = JObject.Parse(templateMedicalNetworkDet);

                        if (MedNetDetL_RowID != null)
                        {
                            MedNetDetL_RowID++;
                            RowIDProperty = string.Empty;
                            RowIDProperty = Convert.ToString(MedNetDetL_RowID);
                        }

                        templateMedicalNetworkDetOBJ["MasterNetworkName"] = network.NetworkName;
                        templateMedicalNetworkDetOBJ["RowIDProperty"] = RowIDProperty;

                        MedicalNetworkDetailList[0].AddAfterSelf(templateMedicalNetworkDetOBJ);

                        Result.Append(" \n New Record with {" + network.NetworkName + "}  added to Medical Network Detail List.");
                    }
                    else
                        Result.Append(" \n Record already Exists.");
                }
            }
            #endregion

            #region MedicalNetworks_ToDelete
            Result.Append("\n DELETES : ");
            foreach (var servc in MedicalNetworks_ToDelete)
            {
                var SrvcToRemove = MedicalNetworkList.Where(p => (Convert.ToString(p.SelectToken("MasterNetworkName")) ?? "").Trim() == servc.NetworkName.Trim()).FirstOrDefault();
                if (null != SrvcToRemove)
                {
                    SrvcToRemove.Remove();
                    Result.Append(" \nRecord with MasterNetworkName =  {" + servc.NetworkName + "}   is Deleted from Medical Network List.");
                }

                var SrvcDetToRemove = MedicalNetworkDetailList.Where(p => (Convert.ToString(p.SelectToken("MasterNetworkName")) ?? "").Trim() == servc.NetworkName.Trim()).FirstOrDefault();
                if (null != SrvcDetToRemove)
                {
                    SrvcDetToRemove.Remove();
                    Result.Append(" \nRecord with MasterNetworkName =  {" + servc.NetworkName + "}   is Deleted from Medical Network Detail List.");
                }
            }
            #endregion

            #region MedicalNetworks_ToUpdate
            Result.Append("\n UPDATES : ");
            foreach (var servc in MedicalNetworks_ToUpdate)
            {
                var SrvcToUpdate = MedicalNetworkList.Where(p => (Convert.ToString(p.SelectToken("MasterNetworkName")) ?? "").Trim() == servc.ChangeRequest.Trim()).FirstOrDefault();
                if (null != SrvcToUpdate)
                {
                    Result.Append("{" + SrvcToUpdate["MasterNetworkName"] + "}   is Updated to " + servc.NetworkName + " in Medical Network List.");
                    SrvcToUpdate["MasterNetworkName"] = servc.NetworkName;
                }

                var SrvcDetToUpdate = MedicalNetworkDetailList.Where(p => (Convert.ToString(p.SelectToken("MasterNetworkName")) ?? "").Trim() == servc.ChangeRequest.Trim()).FirstOrDefault();
                if (null != SrvcDetToUpdate)
                {
                    Result.Append("{" + SrvcDetToUpdate["MasterNetworkName"] + "}   is  Updated to " + servc.NetworkName + " in Medical Network Detail List.");
                    SrvcDetToUpdate["MasterNetworkName"] = servc.NetworkName;
                }
            }
            #endregion
        }

        public static void EQN1557_UpdateMasterList_AdminNetworkSection(JObject RootData)
        {
            string AdminNetworkListPath = "AdminNetwork.AdminNetworkList";  // MedicalSubNetworkList
            string AdminSubNetworkListPath = "AdminNetwork.AdminSubNetworkList";
            string AdminNetworkDetailListPath = "AdminNetwork.AdminNetworkDetailList";

            string templateAdminNetworkDet = @"{
				""MasterNetworkName"": """",
				""MasterListAdminSubNetworkList"": []
			    }";

            var AdminNetworkList = RootData.SelectToken(AdminNetworkListPath);
            var AdminSubNetworkList = RootData.SelectToken(AdminSubNetworkListPath);
            var AdminNetworkDetailList = RootData.SelectToken(AdminNetworkDetailListPath);






            Result.Append("\n ======     " + AdminNetworkListPath + "; " + AdminNetworkDetailListPath + "      ======\n");
            Result.Append("\n ADDITIONS : ");

            foreach (var network in AdminNetworks_ToAdd)
            {
                if (!AdminNetworkList.Where(i => (Convert.ToString(i.SelectToken("MasterNetworkName")) ?? "").Trim() == network.NetworkName.Trim()).Any())
                {
                    AdminNetworkList[0].AddAfterSelf(JObject.Parse("{'MasterNetworkName': '" + network.NetworkName + "'}"));
                    Result.Append(" \n New Record with {" + network.NetworkName + "}  added to Admin Network List.");
                }
                else
                    Result.Append(" \n Record already Exists.");

                if (!AdminNetworkDetailList.Where(i => (Convert.ToString(i.SelectToken("MasterNetworkName")) ?? "").Trim() == network.NetworkName.Trim()).Any())
                {
                    JObject templateMedicalNetworkDetOBJ = JObject.Parse(templateAdminNetworkDet);
                    templateMedicalNetworkDetOBJ["MasterNetworkName"] = network.NetworkName;

                    AdminNetworkDetailList[0].AddAfterSelf(templateMedicalNetworkDetOBJ);
                    Result.Append(" \n New Record with {" + network.NetworkName + "}  added to Admin Network Detail List.");
                }
                else
                    Result.Append(" \n Record already Exists.");
            }


            foreach (var servc in AdminNetworks_ToDelete)
            {
                var SrvcToRemove = AdminNetworkList.Where(p => (Convert.ToString(p.SelectToken("MasterNetworkName")) ?? "").Trim() == servc.NetworkName.Trim()).FirstOrDefault();
                if (null != SrvcToRemove)
                {
                    SrvcToRemove.Remove();
                    Result.Append(" \nRecord with MasterNetworkName =  {" + servc.NetworkName + "}  is Deleted from Admin Network List.");
                }

                var SrvcDetToRemove = AdminNetworkDetailList.Where(p => (Convert.ToString(p.SelectToken("MasterNetworkName")) ?? "").Trim() == servc.NetworkName.Trim()).FirstOrDefault();
                if (null != SrvcDetToRemove)
                {
                    SrvcDetToRemove.Remove();
                    Result.Append(" \nRecord with MasterNetworkName =  {" + servc.NetworkName + "}  is Deleted from Admin Network Detail List.");
                }
            }

            foreach (var servc in AdminNetworks_ToUpdate)
            {
                var SrvcToUpdate = AdminNetworkList.Where(p => (Convert.ToString(p.SelectToken("MasterNetworkName")) ?? "").Trim() == servc.ChangeRequest.Trim()).FirstOrDefault();
                if (null != SrvcToUpdate)
                {
                    Result.Append("\n{" + SrvcToUpdate["MasterNetworkName"] + "}   is  Updated to " + servc.NetworkName + " in Admin Network List.");
                    SrvcToUpdate["MasterNetworkName"] = servc.NetworkName;
                }

                var SrvcDetToUpdate = AdminNetworkDetailList.Where(p => (Convert.ToString(p.SelectToken("MasterNetworkName")) ?? "").Trim() == servc.ChangeRequest.Trim()).FirstOrDefault();
                if (null != SrvcDetToUpdate)
                {
                    Result.Append("\n{" + SrvcToUpdate["MasterNetworkName"] + "}   is  Updated to " + servc.NetworkName + " in Admin Network Detail List.");
                    SrvcDetToUpdate["MasterNetworkName"] = servc.NetworkName;
                }
            }
        }

        public static void EQN1557_UpdateMasterList_AdminVendorSection(JObject RootData)
        {
            string AdminVendorTypeListPath = "AdminVendor.VendorTypeList";  // MedicalSubNetworkList
            string AdminVendorOptionListPath = "AdminVendor.VendorOptionList";
            string AdminVendorMappingListPath = "AdminVendor.VendorMappingList";

            var AdminVendorTypeList = RootData.SelectToken(AdminVendorTypeListPath);
            var AdminVendorOptionList = RootData.SelectToken(AdminVendorOptionListPath);
            var AdminVendorMappingList = RootData.SelectToken(AdminVendorMappingListPath);


            Result.Append("\n ======     " + AdminVendorTypeListPath + "      ======\n");
            Result.Append("\n ADDITIONS : ");

            foreach (var network in AdminVendorType_ToAdd)
            {
                if (!AdminVendorTypeList.Where(h => (Convert.ToString(h.SelectToken("VendorType")) ?? "").Trim() == network.VendorType.Trim()).Any())
                {
                    AdminVendorTypeList[0].AddAfterSelf(JObject.Parse("{'VendorType': '" + network.VendorType + "'}"));
                    Result.Append(" \n New Record with {" + network.VendorType + "}  added to Admin Vendor Type List.");
                }
                else
                    Result.Append(" \n Record already Exists.");

                if (!AdminVendorMappingList.Where(h => (Convert.ToString(h.SelectToken("VendorType")) ?? "").Trim() == network.VendorType.Trim() && (Convert.ToString(h.SelectToken("VendorOption")) ?? "").Trim() == network.VendorOption.Trim()).Any())
                {
                    AdminVendorMappingList[0].AddAfterSelf(JObject.Parse("{'VendorType': '" + network.VendorType + "', 'VendorOption': '" + network.VendorOption + "'}"));
                    Result.Append(" \n New Record with {" + network.VendorType + "}  added to Admin Vendor Type List.");
                }
                else
                    Result.Append(" \n Record already Exists.");


            }

            Result.Append("\n UPDATES : ");
            foreach (var servc in AdminVendorType_ToUpdate)
            {
                string VendorType = "";
                string VendorOption = "";
                if (servc.ChangeRequest.Contains(";;"))
                {
                    string[] temmp = servc.ChangeRequest.Split(new string[] { ";;" }, StringSplitOptions.None);
                    VendorType = temmp[0];
                    VendorOption = temmp[1];
                }
                else
                {
                    VendorType = servc.ChangeRequest;
                    VendorOption = servc.VendorOption;
                }

                var SrvcToUpdate = AdminVendorTypeList.Where(p => (Convert.ToString(p.SelectToken("VendorType")) ?? "").Trim() == VendorType.Trim()).FirstOrDefault();
                if (null != SrvcToUpdate)
                {
                    Result.Append("\n{" + SrvcToUpdate["VendorType"] + "}  updated to  {" + servc.VendorType + "}");
                    SrvcToUpdate["VendorType"] = servc.VendorType;
                }


                if (servc.ChangeNeededFor == "Both")
                {
                    var SrvcMapToUpdate = AdminVendorMappingList.Where(p => (Convert.ToString(p.SelectToken("VendorType")) ?? "").Trim() == VendorType.Trim() && (Convert.ToString(p.SelectToken("VendorOption")) ?? "").Trim() == VendorOption.Trim()).FirstOrDefault();
                    if (null != SrvcMapToUpdate)
                    {
                        Result.Append("\n{" + SrvcMapToUpdate["VendorType"] + "}  updated to  {" + servc.VendorType + "}");
                        SrvcMapToUpdate["VendorType"] = servc.VendorType;
                    }
                }
                else
                {
                    var SrvcMapToUpdate = AdminVendorMappingList.Where(p => (Convert.ToString(p.SelectToken("VendorType")) ?? "").Trim() == VendorType.Trim() && (Convert.ToString(p.SelectToken("VendorOption")) ?? "").Trim() == servc.VendorOption.Trim()).FirstOrDefault();
                    if (null != SrvcMapToUpdate)
                    {
                        Result.Append("\n{" + SrvcMapToUpdate["VendorType"] + "}  updated to  {" + servc.VendorType + "}");
                        SrvcMapToUpdate["VendorType"] = servc.VendorType;
                    }
                }

            }

            Result.Append("\n ======     " + AdminVendorOptionListPath + "      ======\n");
            Result.Append("\n ADDITIONS : ");
            foreach (var network in AdminVendorOption_ToAdd)
            {
                //Aetna Dental (fully insured)  Cigna (fully insured)   Guardian (fully insured)


                if (!AdminVendorOptionList.Where(h => (Convert.ToString(h.SelectToken("VendorOption")) ?? "").Trim() == network.VendorOption.Trim()).Any())
                {
                    AdminVendorOptionList[0].AddAfterSelf(JObject.Parse("{'VendorOption': '" + network.VendorOption + "'}"));
                    Result.Append(" \n New Record with {" + network.VendorOption + "}  added to Admin Vendor Option List.");
                }
                else
                    Result.Append(" \n Record already Exists.");

                if (network.ChangeNeededFor == "Both")
                    continue;

                if (!network.VendorOption.Contains("Aetna Dental (fully insured)")
                && !network.VendorOption.Contains("Cigna (fully insured)")
                && !network.VendorOption.Contains("Guardian (fully insured)"))
                {
                    if (!AdminVendorMappingList.Where(h => (Convert.ToString(h.SelectToken("VendorType")) ?? "").Trim() == network.VendorType.Trim() && (Convert.ToString(h.SelectToken("VendorOption")) ?? "").Trim() == network.VendorOption.Trim()).Any())
                    {
                        AdminVendorMappingList[0].AddAfterSelf(JObject.Parse("{'VendorType': '" + network.VendorType + "', 'VendorOption': '" + network.VendorOption + "'}"));
                        Result.Append(" \n New Record with {" + network.VendorType + "}  added to Admin Vendor Type List.");
                    }
                    else
                        Result.Append(" \n Record already Exists.");
                }
            }

            Result.Append("\n UPDATES : ");
            foreach (var servc in AdminVendorOption_ToUpdate)
            {
                //Aetna Dental (fully insured)  Cigna (fully insured)   Guardian (fully insured)


                string temp = "";
                if (servc.ChangeRequest.Contains(";;"))
                    temp = servc.ChangeRequest.Split(new string[] { ";;" }, StringSplitOptions.None)[1];
                else
                    temp = servc.ChangeRequest;

                if (!servc.VendorOption.Contains("Aetna Dental (fully insured)")
                && !servc.VendorOption.Contains("Cigna (fully insured)")
                && !servc.VendorOption.Contains("Guardian (fully insured)"))
                {
                    var SrvcToUpdate = AdminVendorOptionList.Where(p => (Convert.ToString(p.SelectToken("VendorOption")) ?? "").Trim() == temp.Trim()).FirstOrDefault();
                    if (SrvcToUpdate != null)
                    {
                        Result.Append("\n{" + SrvcToUpdate["VendorOption"] + "}  updated to  {" + servc.VendorOption + "}");
                        SrvcToUpdate["VendorOption"] = servc.VendorOption;
                    }
                }

                var SrvcMapToUpdate = AdminVendorMappingList.Where(p => (Convert.ToString(p.SelectToken("VendorOption")) ?? "").Trim() == temp.Trim() && (Convert.ToString(p.SelectToken("VendorType")) ?? "").Trim() == servc.VendorType.Trim()).FirstOrDefault();
                if (null != SrvcMapToUpdate)
                {
                    Result.Append("\n{" + SrvcMapToUpdate["VendorType"] + "}  updated to  {" + servc.VendorType + "}");
                    SrvcMapToUpdate["VendorOption"] = servc.VendorOption;
                }

            }
        }

        #endregion


        #region EQN-1622 EQN-1641(Partially)

        public static List<dynamic> NewVendorsForExistingVendorTypes = new List<dynamic>();


        public static void EQN_1622_Update_MasterList()
        {
            Result.Append("\n-------------------------------Master List----------------------------\n");
            string command = @"select top 1 ins.FormInstanceID,FormData from ui.formdesign dess
                               inner join fldr.FormInstance ins on dess.FormID=ins.FormDesignID
                               inner join fldr.FormInstanceDataMap insdata on insdata.FormInstanceID=ins.FormInstanceID
                               where FormName='MasterList' and dess.IsActive=1 and ins.IsActive=1 order by ins.FormInstanceID DESC";



            GetJsonFromDB(command, MasterList);

            NewVendorsForExistingVendorTypes.Add(new { VendorType = "Case Management - Specialty Case", VendorOption = "Rx Benefits Preferred" });
            NewVendorsForExistingVendorTypes.Add(new { VendorType = "Pharmacy Benefit Management", VendorOption = "RN Cancer Guides" });
            NewVendorsForExistingVendorTypes.Add(new { VendorType = "Preference Sensitive Care (Small Case Management)", VendorOption = "Evolution (by PremierSource)" });
            NewVendorsForExistingVendorTypes.Add(new { VendorType = "Networks", VendorOption = "Evolution Elite Specialty Services" }); // EQN-1641
            

            JObject RootData = JObject.Parse(Json);
            string JSON_String = RootData.ToString(Newtonsoft.Json.Formatting.Indented);

            EQN_1622_Update_MasterList_AdminVendor_Section(RootData);
            EQN_1633_1641_Add_HealthCareStrategies_As_Network_To_Admin_And_Medical(RootData);

            string updatedData = "'" + RootData.ToString(Newtonsoft.Json.Formatting.None).Replace("'", "''") + "'";
            updatedData = "update  fldr.forminstancedatamap set formdata=" + updatedData + " where forminstanceid= " + MasterListFormInstanceID;
            UpdateJsonInDB(updatedData);
        }

        public static void EQN_1622_Update_MasterList_AdminVendor_Section(JObject RootData)
        {
            var AdminNetworks = RootData.SelectToken(AdminNetworks_ML);  // 75
            var AdminVendorTypes = RootData.SelectToken(AdminVendorTypes_ML);  // 55
            var AdminVendorOptions = RootData.SelectToken(AdminVendorOptions_ML);  // 73
            var AdminVendorTypeOptionMappings = RootData.SelectToken(AdminVendorTypeOptionMappings_ML);   // 257
            

            if (AdminNetworks != null && AdminNetworks.Any())
            {
                if (AdminVendorTypes != null && AdminVendorTypes.Any())
                {
                    JObject NewVendorType = JObject.Parse("{\"VendorType\": \"" + NewAdminVendorType_ML + "\"}");
                    AdminVendorTypes.ToList()[0].AddAfterSelf(NewVendorType);


                    foreach (JObject network in AdminNetworks)
                    {
                        string MasterNetworkName = Convert.ToString(network["MasterNetworkName"]) ?? "";
                        JObject NewVendorOption = JObject.Parse("{\"VendorOption\": \"" + MasterNetworkName + "\"}");
                        JObject NewVendorTypeOptionMapping = JObject.Parse(@"{""VendorType"": """ + NewAdminVendorType_ML + @""", ""VendorOption"": """ + MasterNetworkName + @"""}");

                        if (AdminVendorOptions != null)
                            AdminVendorOptions.ToList()[0].AddAfterSelf(NewVendorOption);

                        if (AdminVendorTypeOptionMappings != null)
                            AdminVendorTypeOptionMappings.ToList()[0].AddAfterSelf(NewVendorTypeOptionMapping);
                    }

                    foreach (var record in NewVendorsForExistingVendorTypes)
                    {
                        string VendorType = record.VendorType;
                        string VendorOption = record.VendorOption;

                        JObject NewVendorOption = JObject.Parse("{\"VendorOption\": \"" + VendorOption + "\"}");
                        JObject NewVendorTypeOptionMapping = JObject.Parse(@"{""VendorType"": """ + VendorType + @""", ""VendorOption"": """ + VendorOption + @"""}");

                        if (AdminVendorOptions != null)
                            AdminVendorOptions.ToList()[0].AddAfterSelf(NewVendorOption);

                        if (AdminVendorTypeOptionMappings != null)
                            AdminVendorTypeOptionMappings.ToList()[0].AddAfterSelf(NewVendorTypeOptionMapping);
                    }
                }
            }
        }
        #endregion

        #region EQN-1633, EQN-1641(Partially)   Add 'HealthCare Strategies' as Master Network in Admin & Medical Networks - Support Ticket #1122 
        public static void EQN_1633_1641_Add_HealthCareStrategies_As_Network_To_Admin_And_Medical(JObject RootData)
        {
            string NewNetworkName = "HealthCare Strategies";
            string EQN1641_AdminMedicalNetwork = "Evolution Elite Specialty Services";
            string EQN1641_MedicalNetworknumber = "269";
            string EQN1641_MedicalSubNetworknumber = "1570";
            var AdminNetworks = RootData.SelectToken(AdminNetworks_ML);  // 75
            var MedicalNetworks = RootData.SelectToken(MedicalNetworks_ML);  // 75
            var MedicalSubNetworks = RootData.SelectToken(MedicalSubNetworks_ML);  // 75  
            var MedicalNetworkDetailList = RootData.SelectToken(MedicalNetworkDetailList_ML);

            
            int Current_MedicalSubNetworkRowIDProperty = 0;
            var RowIDProperties_In_MedicalSubNetworks = MedicalSubNetworks.ToList().Select(k => Convert.ToInt32(Convert.ToString(k.SelectToken("RowIDProperty")) ?? ""));
            if (RowIDProperties_In_MedicalSubNetworks != null && RowIDProperties_In_MedicalSubNetworks.Any())
            {
                Current_MedicalSubNetworkRowIDProperty = RowIDProperties_In_MedicalSubNetworks.OrderByDescending(i => i).FirstOrDefault();
                Current_MedicalSubNetworkRowIDProperty=Current_MedicalSubNetworkRowIDProperty == 0 ? 0 : Current_MedicalSubNetworkRowIDProperty + 1;
            }

            int Current_MedicalNetworkRowIDProperty = 0;
            var RowIDProperties_In_MedicalNetworks = MedicalNetworks.ToList().Select(k => Convert.ToInt32(Convert.ToString(k.SelectToken("RowIDProperty")) ?? ""));
            if (RowIDProperties_In_MedicalNetworks != null && RowIDProperties_In_MedicalNetworks.Any())
            {
                Current_MedicalNetworkRowIDProperty = RowIDProperties_In_MedicalNetworks.OrderByDescending(i => i).FirstOrDefault();
                Current_MedicalNetworkRowIDProperty=Current_MedicalNetworkRowIDProperty==0?0:Current_MedicalNetworkRowIDProperty+1;
            }


            #region EQN-1641
            JObject EQN1641_AdminNetworkNew = JObject.Parse("{\"MasterNetworkName\": \"" + EQN1641_AdminMedicalNetwork + "\"}");
            if (AdminNetworks != null && AdminNetworks.Any())
                AdminNetworks.ToList()[0].AddAfterSelf(EQN1641_AdminNetworkNew);

            string EQN1641_NewMedicalNetworks_String = "{\"MasterNetworkName\": \"" + EQN1641_AdminMedicalNetwork + "\", \"MasterNumber\": \"" + EQN1641_MedicalNetworknumber + "\"" + (Current_MedicalNetworkRowIDProperty == 0 ? "}" : ", \"RowIDProperty\": " + Current_MedicalNetworkRowIDProperty + "}");
            JObject EQN1641_NewMedicalNetworks = JObject.Parse(EQN1641_NewMedicalNetworks_String);
            if (MedicalNetworks != null && MedicalNetworks.Any())
                MedicalNetworks.ToList()[0].AddAfterSelf(EQN1641_NewMedicalNetworks);

            string EQN1641_NewMedicalSubNetwork_String = "{\"SubNetworkName\": \"" + EQN1641_AdminMedicalNetwork + "\", \"SubNetworkNumber\": \"" + EQN1641_MedicalSubNetworknumber + "\", \"PPOComment\": \"\", \"PricingMethod\": \"\"" + (Current_MedicalSubNetworkRowIDProperty == 0 ? " }" : ", \"RowIDProperty\": " + Current_MedicalSubNetworkRowIDProperty + " }");
            JObject EQN1641_NewMedicalSubNetwork = JObject.Parse(EQN1641_NewMedicalSubNetwork_String);
            if (MedicalSubNetworks != null && MedicalSubNetworks.Any())
                MedicalSubNetworks.ToList()[0].AddAfterSelf(EQN1641_NewMedicalSubNetwork);


            string MedicalNetworkDetailList_Template = @"{""MasterNetworkName"": """ + EQN1641_AdminMedicalNetwork + @""",
				""MasterNumber"": """ + EQN1641_MedicalNetworknumber + @""",
				""IsManualSelect"": ""No"",
				""IsStandardlyaddedNetwork"": ""No"",
				""SelectSubNetwork"": ""No"",
				""Address"": """",
				""City"": """",
				""State"": """",
				""Zip"": """",
				""EDINumber"": """",
				""Website"": """",
				""MasterListSubNetworkList"": [
					{
						""SubNetworkName"": """ + EQN1641_AdminMedicalNetwork + @""",
						""SubNetworkNumber"": """ + EQN1641_MedicalSubNetworknumber + @""",
						""PPOComment"": """",
						""PricingMethod"": """"
					}]}";
            JObject NewMedicalNetworkDetailList = JObject.Parse(MedicalNetworkDetailList_Template);
            if (MedicalNetworkDetailList != null && MedicalNetworkDetailList.Any())
                MedicalNetworkDetailList.ToList()[0].AddAfterSelf(NewMedicalNetworkDetailList);
#endregion

            #region EQN-1633
            JObject NewAdminNetworks = JObject.Parse("{\"MasterNetworkName\": \"" + NewNetworkName + "\"}");
            if (AdminNetworks != null && AdminNetworks.Any())
                AdminNetworks.ToList()[0].AddAfterSelf(NewAdminNetworks);

            if (MedicalNetworks != null && MedicalNetworks.Any())
            {
                Current_MedicalNetworkRowIDProperty = Current_MedicalNetworkRowIDProperty == 0 ? 0 : Current_MedicalNetworkRowIDProperty + 1;
                string NewMedicalNetworks_String = "{\"MasterNetworkName\": \"" + NewNetworkName + "\", \"MasterNumber\": \"\"" + (Current_MedicalNetworkRowIDProperty == 0 ? "" : (", \"RowIDProperty\": " + Current_MedicalNetworkRowIDProperty)) + "}";
                JObject NewMedicalNetworks = JObject.Parse(NewMedicalNetworks_String);
                
                MedicalNetworks.ToList()[0].AddAfterSelf(NewMedicalNetworks);
            }
            #endregion

        }

        #endregion






        #region EQN-1582 Typos and Formatting Issues - Backlog from EQN-1467
        public static void EQN1582_Update_Product()
        {
        Result.Append("\n\n-------------------------------Products----------------------------");


            List<int> Products = new List<int>();
            string allProducts = @"select ins.FormInstanceID from ui.formdesign dess
                                 inner join fldr.FormInstance ins on dess.FormID=ins.FormDesignID
                                 inner join fldr.FormInstanceDataMap insdata on insdata.FormInstanceID=ins.FormInstanceID
                                 where FormName='Medical' and dess.IsActive=1 and ins.IsActive=1";

            //allProducts += " and ins.FormInstanceID in (997)";

            string currentProduct = "select FormInstanceID,formdata from  fldr.forminstancedatamap where forminstanceid={0}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = new SqlCommand(allProducts, conn))
                {
                    conn.Open();
                    using (SqlDataReader rd = comm.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            Products.Add((int)rd[0]);
                        }
                    }
                }
            }

            foreach (var product in Products)
            {
                if (product == 869)
                {
                }

                Result.Append("\n");
                GetJsonFromDB(string.Format(currentProduct, product), Product);
                JObject RootData = JObject.Parse(Json);
                string JSON_String = RootData.ToString(Newtonsoft.Json.Formatting.Indented);

                EQN_1582_UpdateBRG(RootData);

                string updatedData = "'" + RootData.ToString(Newtonsoft.Json.Formatting.None).Replace("'", "''") + "'";

                updatedData = "update  fldr.forminstancedatamap set formdata=" + updatedData + " where forminstanceid= " + ProductFormInstanceID;
                UpdateJsonInDB(updatedData);
                string finsihed = string.Empty;
            }
            
        }


        public static void EQN_1582_UpdateBRG(JObject RootData)
        {
            var BRGData = RootData.SelectToken(BenefitReviewGrid);
            CopayValues = new List<JToken>();
            GetCostShareValues(RootData);
            Regex numeric = new Regex(@"\d+");

            string BC1_2 = "Diagnostic Radiology (Outpatient)";
            string BC1 = "Diagnostic Radiology";
            string BC2 = "High Tech DX";
            string OVC1 = "Services with office visit charge";
            string OVC2 = "Services NO office visit charge";
            string HiTechCopay="Hi-Tech Imaging Copay";
            //string NoCopay = "Not Applicable";


            var HiTechServices = BRGData.Where(i => (Convert.ToString(i.SelectToken(BenefitCategory1)) ?? "").Contains(BC1) && (Convert.ToString(i.SelectToken(BenefitCategory2)) ?? "") == BC2);
            if (HiTechServices != null && HiTechServices.Any())
            {
                foreach (var HiTechSrvcs in HiTechServices)
                {
                    if (HiTechSrvcs != null)
                    {
                        HiTechSrvcs[ApplyCopayType] = HiTechCopay;
                        if (CopayValues != null && CopayValues.Any())
                        {
                            var HiTechCopayValues = CopayValues.Where(i => (Convert.ToString(i.SelectToken(CopayType)) ?? "") == HiTechCopay).FirstOrDefault(); //"CopayType": "PCP"
                            if (HiTechCopayValues != null)
                            {
                                var IQMedicalPlanNetwork_ = HiTechCopayValues[IQMedicalPlanNetwork];
                                if (IQMedicalPlanNetwork_ != null && IQMedicalPlanNetwork_.Any())
                                {
                                    foreach (var network in IQMedicalPlanNetwork_)
                                    {
                                        string Copay_Amount = Convert.ToString(network[Amount]) ?? "";
                                        string Copay_NTier = Convert.ToString(network[CostShareTier]) ?? "";
                                        string Srvc_CopayAmount = string.Empty;
                                        string Srvc_ApplyCopayType = string.Empty;
                                        var BRG_Servc_IQMedicalPlanNetwork = HiTechSrvcs[IQMedicalPlanNetwork];
                                        Srvc_ApplyCopayType = Convert.ToString(HiTechSrvcs[ApplyCopayType]) ?? "";
                                        //HiTechSrvcs[ApplyCopayType] = HiTechCopay;

                                        if (BRG_Servc_IQMedicalPlanNetwork != null && BRG_Servc_IQMedicalPlanNetwork.Any())
                                        {
                                            var TData = BRG_Servc_IQMedicalPlanNetwork.Where(i => (Convert.ToString(i.SelectToken(CostShareTier)) ?? "") == Copay_NTier).FirstOrDefault();
                                            if (TData != null)
                                            {
                                                Srvc_CopayAmount = Convert.ToString(TData[Copay]) ?? "";

                                                //if ((Srvc_CopayAmount == NotApplicable || Srvc_CopayAmount == NA || string.IsNullOrEmpty(Srvc_CopayAmount)) && string.IsNullOrEmpty(Copay_Amount))
                                                //    TData[Copay] = NoCopay;
                                                //else if ((Srvc_CopayAmount == NotApplicable || Srvc_CopayAmount == NA || string.IsNullOrEmpty(Srvc_CopayAmount)) && !string.IsNullOrEmpty(Copay_Amount))
                                                //    TData[Copay] = Copay_Amount;

                                                if (!numeric.Match(Srvc_CopayAmount).Success && string.IsNullOrEmpty(Copay_Amount))
                                                    TData[Copay] = NoCopay;
                                                else if (!numeric.Match(Srvc_CopayAmount).Success && !string.IsNullOrEmpty(Copay_Amount))
                                                    TData[Copay] = Copay_Amount;

                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var BRG_Servc_IQMedicalPlanNetwork = HiTechSrvcs[IQMedicalPlanNetwork];
                                
                                if (BRG_Servc_IQMedicalPlanNetwork != null && BRG_Servc_IQMedicalPlanNetwork.Any())
                                {
                                    foreach (var networkData in BRG_Servc_IQMedicalPlanNetwork)
                                    {
                                        string Srvc_CopayAmount = Convert.ToString(networkData[Copay]) ?? "";
                                        if (!numeric.Match(Srvc_CopayAmount).Success)
                                            networkData[Copay] = NoCopay;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var BRG_Servc_IQMedicalPlanNetwork = HiTechSrvcs[IQMedicalPlanNetwork];

                            if (BRG_Servc_IQMedicalPlanNetwork != null && BRG_Servc_IQMedicalPlanNetwork.Any())
                            {
                                foreach (var networkData in BRG_Servc_IQMedicalPlanNetwork)
                                {
                                    string Srvc_CopayAmount = Convert.ToString(networkData[Copay]) ?? "";
                                    if (!numeric.Match(Srvc_CopayAmount).Success)
                                        networkData[Copay] = NoCopay;
                                }
                            }
                        }
                    }

                }
            }
        
        }

        //public static void EQN_1582_UpdateBRG(JObject RootData)
        //{
        //    var BRGData = RootData.SelectToken(BenefitReviewGrid);
        //    CopayValues = new List<JToken>();
        //    GetCostShareValues(RootData);
        //    Regex numeric = new Regex(@"\d+");

        //    string BC1 = "Diagnostic Radiology";
        //    string BC2 = "High Tech DX";
        //    string OVC1 = "Services with office visit charge";
        //    string OVC2 = "Services NO office visit charge";
        //    string HiTechCopay = "Hi-Tech Imaging Copay";
        //    //string NoCopay = "Not Applicable";


        //    var HiTechServices = BRGData.Where(i => (Convert.ToString(i.SelectToken(BenefitCategory1)) ?? "") == BC1 && (Convert.ToString(i.SelectToken(BenefitCategory2)) ?? "") == BC2);
        //    if (HiTechServices != null && HiTechServices.Any())
        //    {
        //        foreach (var HiTechSrvcs in HiTechServices)
        //        {
        //            if (HiTechSrvcs != null)
        //            {
        //                HiTechSrvcs[ApplyCopayType] = HiTechCopay;
        //                if (CopayValues != null && CopayValues.Any())
        //                {
        //                    var HiTechCopayValues = CopayValues.Where(i => (Convert.ToString(i.SelectToken(CopayType)) ?? "") == HiTechCopay).FirstOrDefault(); //"CopayType": "PCP"
        //                    if (HiTechCopayValues != null)
        //                    {
        //                        var IQMedicalPlanNetwork_ = HiTechCopayValues[IQMedicalPlanNetwork];
        //                        if (IQMedicalPlanNetwork_ != null && IQMedicalPlanNetwork_.Any())
        //                        {
        //                            foreach (var network in IQMedicalPlanNetwork_)
        //                            {
        //                                string Copay_Amount = Convert.ToString(network[Amount]) ?? "";
        //                                string Copay_NTier = Convert.ToString(network[CostShareTier]) ?? "";
        //                                string Srvc_CopayAmount = string.Empty;
        //                                string Srvc_ApplyCopayType = string.Empty;
        //                                var BRG_Servc_IQMedicalPlanNetwork = HiTechSrvcs[IQMedicalPlanNetwork];
        //                                Srvc_ApplyCopayType = Convert.ToString(HiTechSrvcs[ApplyCopayType]) ?? "";
        //                                //HiTechSrvcs[ApplyCopayType] = HiTechCopay;

        //                                if (BRG_Servc_IQMedicalPlanNetwork != null && BRG_Servc_IQMedicalPlanNetwork.Any())
        //                                {
        //                                    var TData = BRG_Servc_IQMedicalPlanNetwork.Where(i => (Convert.ToString(i.SelectToken(CostShareTier)) ?? "") == Copay_NTier).FirstOrDefault();
        //                                    if (TData != null)
        //                                    {
        //                                        Srvc_CopayAmount = Convert.ToString(TData[Copay]) ?? "";

        //                                        //if ((Srvc_CopayAmount == NotApplicable || Srvc_CopayAmount == NA || string.IsNullOrEmpty(Srvc_CopayAmount)) && string.IsNullOrEmpty(Copay_Amount))
        //                                        //    TData[Copay] = NoCopay;
        //                                        //else if ((Srvc_CopayAmount == NotApplicable || Srvc_CopayAmount == NA || string.IsNullOrEmpty(Srvc_CopayAmount)) && !string.IsNullOrEmpty(Copay_Amount))
        //                                        //    TData[Copay] = Copay_Amount;

        //                                        if (!numeric.Match(Srvc_CopayAmount).Success && string.IsNullOrEmpty(Copay_Amount))
        //                                            TData[Copay] = NoCopay;
        //                                        else if (!numeric.Match(Srvc_CopayAmount).Success && !string.IsNullOrEmpty(Copay_Amount))
        //                                            TData[Copay] = Copay_Amount;

        //                                    }
        //                                }




        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        var BRG_Servc_IQMedicalPlanNetwork = HiTechSrvcs[IQMedicalPlanNetwork];

        //                        if (BRG_Servc_IQMedicalPlanNetwork != null && BRG_Servc_IQMedicalPlanNetwork.Any())
        //                        {
        //                            foreach (var networkData in BRG_Servc_IQMedicalPlanNetwork)
        //                            {
        //                                string Srvc_CopayAmount = Convert.ToString(networkData[Copay]) ?? "";
        //                                if (!numeric.Match(Srvc_CopayAmount).Success)
        //                                    networkData[Copay] = NoCopay;
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    var BRG_Servc_IQMedicalPlanNetwork = HiTechSrvcs[IQMedicalPlanNetwork];

        //                    if (BRG_Servc_IQMedicalPlanNetwork != null && BRG_Servc_IQMedicalPlanNetwork.Any())
        //                    {
        //                        foreach (var networkData in BRG_Servc_IQMedicalPlanNetwork)
        //                        {
        //                            string Srvc_CopayAmount = Convert.ToString(networkData[Copay]) ?? "";
        //                            if (!numeric.Match(Srvc_CopayAmount).Success)
        //                                networkData[Copay] = NoCopay;
        //                        }
        //                    }
        //                }
        //            }

        //        }
        //    }

        //}

        #endregion


        #region EQN-1043

        public static string InPatient = "Inpatient Copay";
        public static string OutPatient = "Outpatient Surgical Copay";
        public static string NotApplicable = "Not Applicable";
        public static string NA = "NA";
        public static string NoCopay = "No Copay";


        public static void UpdateProductForServiceshaving_InpatientANDOutPatientSurgical()
        {
            List<int> Products = new List<int>();
            string allProducts = @"select ins.FormInstanceID from ui.formdesign dess
                                 inner join fldr.FormInstance ins on dess.FormID=ins.FormDesignID
                                 inner join fldr.FormInstanceDataMap insdata on insdata.FormInstanceID=ins.FormInstanceID
                                 where FormName='Medical' and dess.IsActive=1 and ins.IsActive=1";

            //allProducts += " and ins.FormInstanceID in (931)";//(869,990)";

            string currentProduct = "select FormInstanceID,formdata from  fldr.forminstancedatamap where forminstanceid={0}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = new SqlCommand(allProducts, conn))
                {
                    conn.Open();
                    using (SqlDataReader rd = comm.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            Products.Add((int)rd[0]);
                        }
                    }
                }
            }

            foreach (var product in Products)
            {
                GetJsonFromDB(string.Format(currentProduct, product), Product);
                JObject RootData = JObject.Parse(Json);
                string JSON_String = RootData.ToString(Newtonsoft.Json.Formatting.Indented);

                UpdateBRG(RootData);
                UpdateAdditionalServices(RootData);

                string updatedData = "'" + RootData.ToString(Newtonsoft.Json.Formatting.None).Replace("'", "''") + "'";

                updatedData = "update  fldr.forminstancedatamap set formdata=" + updatedData + " where forminstanceid= " + ProductFormInstanceID;
                UpdateJsonInDB(updatedData);
                string finsihed = string.Empty;
            }
        }

        public static string IsInPatientOROutPatientSurgical(JToken token)
        {
            string BC1 = ((string)token[BenefitCategory1]) ?? "";
            string BC2 = ((string)token[BenefitCategory2]) ?? "";

            if (BC1.ToLower().Contains("inpatient") || BC2.ToLower().Contains("inpatient"))
                return InPatient;
            if (BC1.ToLower().Contains("outpatient") && BC2.ToLower().Contains("surgical"))
                return OutPatient;

            return "";
        }

        public static void UpdateBRG(JObject RootData)
        {
            UpdatedCount = 0;
            string InPatientOROutPatientSurgical = string.Empty;
            var BRGData = RootData.SelectToken(BenefitReviewGrid);

            BenefitReviewGridData = GetServicesWithInpatientANDOutPatientSurgicalServices(BRGData);
            Result.Append(" --------------------------------------------------------------------------- " + "\n");
            Result.Append("     [ FormInstanceID: " + ProductFormInstanceID + " ]" + "\n\n\n");
            Result.Append(" --------- Benefit Review Grid ---------  " + "\n\n");

            if (BenefitReviewGridData.Any())
            {
                foreach (var data in BenefitReviewGridData)
                {
                    Result.Append(data.ToString(Newtonsoft.Json.Formatting.None));
                    InPatientOROutPatientSurgical = IsInPatientOROutPatientSurgical(data);
                    data[ApplyCopayType] = InPatientOROutPatientSurgical;

                    var NetworkData = data[IQMedicalPlanNetwork];
                    foreach (JToken token in NetworkData)
                    {
                        if (string.IsNullOrEmpty((string)token[Copay]) || (string)token[Copay] == NotApplicable || (string)token[Copay] == NA)
                            token[Copay] = NoCopay;
                    }

                    Result.Append("  --->   " + data.ToString(Newtonsoft.Json.Formatting.None) + "\n");
                }

            }
        }

        public static List<JToken> GetServicesWithInpatientANDOutPatientSurgicalServices(JToken serviceList, string service = "")
        {
            //var MedicalServicesToBeChanged = Benefits.GetBenefitsToApplyChangeTo();

            var services = serviceList.Where(i => ((string)i.SelectToken(BenefitCategory1) ?? "").ToLower().Contains("inpatient")
                                               || ((string)i.SelectToken(BenefitCategory2) ?? "").ToLower().Contains("inpatient")
                                               || (((string)i.SelectToken(BenefitCategory1) ?? "").ToLower().Contains("outpatient")
                                                 && ((string)i.SelectToken(BenefitCategory2) ?? "").ToLower().Contains("surgical"))
                ).ToList();

            return services;
        }

        public static void UpdateAdditionalServices(JObject RootData)
        {
            UpdatedCount = 0; ToBeUpdatedCount = 0;
            ADDITIONALSERVICES = (RootData.SelectToken(AdditionalServices));
            string InPatientOROutPatientSurgical = string.Empty;
            Result.Append("\n-----------------  Additional Services   -----------------" + "\n\n");
            List<JToken> services = GetServicesWithInpatientANDOutPatientSurgicalServices(ADDITIONALSERVICES);

            if (services.Any())
            {
                ToBeUpdatedCount = services.Count();
                foreach (JToken service in services)
                {
                    Result.Append(service.ToString(Newtonsoft.Json.Formatting.None));
                    InPatientOROutPatientSurgical = IsInPatientOROutPatientSurgical(service);
                    service[OfficeVisitChargeOnly] = "Copay";
                    service[ApplyCopayType] = InPatientOROutPatientSurgical;
                    Result.Append("    --->   " + service.ToString(Newtonsoft.Json.Formatting.None) + "\n");
                }
            }
            Result.Append(" --------------------------------------------------------------------------- " + "\n");
        }

        #endregion

        public class Benefits
        {
            public string BenefitCategory1 { get; set; }
            public string BenefitCategory2 { get; set; }
            public string BenefitCategory3 { get; set; }
            public string OfficeVisitCategory { get; set; }
            public string Category { get; set; }
            public string ServiceCode { get; set; }
            public string OfficeVisitChargeOnly { get; set; }
            public string ApplyCopayType { get; set; }
            public string ApplyDeductible { get; set; }
            public string ApplyCoinsurance { get; set; }
            public string Grandfathered { get; set; }
            public string Non_Grandfathered { get; set; }
            public string OnlyPCPorSpecialist { get; set; }
            static List<Benefits> benefits_AFTERCHANGE = new List<Benefits>();
            static List<Benefits> benefits_BEFORECHANGE = new List<Benefits>();
            public static List<Benefits> benefits_WithOfficeVisitChargeOnlyAsNAandApplyCopayTypeAsPCP = new List<Benefits>();

            static Benefits()
            {
                benefits_WithOfficeVisitChargeOnlyAsNAandApplyCopayTypeAsPCP.Add(new Benefits { BenefitCategory1 = "Routine Preventive - Exams", BenefitCategory2 = "Child", BenefitCategory3 = "Physical Exam", OfficeVisitCategory = "", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "NA", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_WithOfficeVisitChargeOnlyAsNAandApplyCopayTypeAsPCP.Add(new Benefits { BenefitCategory1 = "Routine Preventive - Exams", BenefitCategory2 = "Baby", BenefitCategory3 = "Physical Exam", OfficeVisitCategory = "", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "NA", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_WithOfficeVisitChargeOnlyAsNAandApplyCopayTypeAsPCP.Add(new Benefits { BenefitCategory1 = "Routine Preventive - Exams", BenefitCategory2 = "Baby", BenefitCategory3 = "Routine Physical Services", OfficeVisitCategory = "", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "NA", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_WithOfficeVisitChargeOnlyAsNAandApplyCopayTypeAsPCP.Add(new Benefits { BenefitCategory1 = "Routine Preventive - Exams", BenefitCategory2 = "Adult", BenefitCategory3 = "Physical Exam", OfficeVisitCategory = "", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "NA", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_WithOfficeVisitChargeOnlyAsNAandApplyCopayTypeAsPCP.Add(new Benefits { BenefitCategory1 = "Routine Preventive - Exams", BenefitCategory2 = "Women (any age)", BenefitCategory3 = "Gynecological Exam", OfficeVisitCategory = "", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "NA", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
            }

            public static List<Benefits> GetBenefitsToApplyChangeTo()
            {
                #region Medical Services To be effected, AFTER CHANGE
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Physicians Office Visit", BenefitCategory2 = "PCP", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Physicians Office Visit", BenefitCategory2 = "Specialist", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Dialysis or Hemodialysis", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Dialysis or Hemodialysis", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Durable Medical Equipment", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Durable Medical Equipment", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Education", BenefitCategory2 = "Instruction", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "ES", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Education", BenefitCategory2 = "Instruction", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "ES", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Hearing", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "HA", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Hearing", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "HA", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Infertility Services Diagnostic", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "IL", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Infertility Services Diagnostic", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "IL", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Infertility Services  Treatment", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "IF", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Infertility Services  Treatment", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "IF", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Massage Therapy", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Massage Therapy", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Maternity", BenefitCategory2 = "Not Prenatal", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Maternity", BenefitCategory2 = "Not Prenatal", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Musculoskeletal", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "MC", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Musculoskeletal", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "MC", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Obesity Weight Control", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "WE", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Obesity Weight Control", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "WE", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Obesity Surgical", BenefitCategory2 = "Surgical", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "GB", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Obesity Surgical", BenefitCategory2 = "Surgical", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "GB", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "OB-GYN", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "GB", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "OB-GYN", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "GB", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Orthotics", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "OH", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Orthotics", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "OH", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Foot Orthotics", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "FO", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Foot Orthotics", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "FO", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Orthopedic Shoes", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "ON", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Orthopedic Shoes", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "ON", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Podiatry", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NOA", ApplyCoinsurance = "NOA", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Podiatry", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NOA", ApplyCoinsurance = "NOA", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Prosthetic Appliances", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "PR", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Prosthetic Appliances", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "PR", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Pulmonary Rehab", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Pulmonary Rehab", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Psychiatric", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Psychiatric", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Biofeedback", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "AU", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Biofeedback", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "AU", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Substance Abuse", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Substance Abuse", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Female Birth Control", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "B5", OfficeVisitChargeOnly = "NA", ApplyCopayType = "", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Female Birth Control", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "B5", OfficeVisitChargeOnly = "NA", ApplyCopayType = "", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Routine Preventive - Exams", BenefitCategory2 = "Health Maintenance", BenefitCategory3 = "Routine Physical Exam Only", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "NA", ApplyCopayType = "", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Routine Preventive - Exams", BenefitCategory2 = "Health Maintenance", BenefitCategory3 = "Routine OB/GYN Exam Only", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "NA", ApplyCopayType = "", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Second Surgical Opinion", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Second Surgical Opinion", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Sleep Disorders", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "SD", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Sleep Disorders", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "SD", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Sleep Disorders", BenefitCategory2 = "Sleep Study", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "SD", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Sleep Disorders", BenefitCategory2 = "Sleep Study", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "SD", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Smoking Cessation", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "SC", ServiceCode = "", OfficeVisitChargeOnly = "NA", ApplyCopayType = "", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Smoking Cessation", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "SC", ServiceCode = "", OfficeVisitChargeOnly = "NA", ApplyCopayType = "", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Therapy Services", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "TY", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Therapy Services", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "TY", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "TMJ Services", BenefitCategory2 = "Non Surgical", BenefitCategory3 = "Pediatric", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "TMJ Services", BenefitCategory2 = "Non Surgical", BenefitCategory3 = "Pediatric", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "TMJ Services", BenefitCategory2 = "Surgical", BenefitCategory3 = "Pediatric", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "TMJ Services", BenefitCategory2 = "Surgical", BenefitCategory3 = "Pediatric", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "TMJ Services", BenefitCategory2 = "Non Surgical", BenefitCategory3 = "Adult", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "TMJ Services", BenefitCategory2 = "Non Surgical", BenefitCategory3 = "Adult", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "TMJ Services", BenefitCategory2 = "Surgical", BenefitCategory3 = "Adult", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "TMJ Services", BenefitCategory2 = "Surgical", BenefitCategory3 = "Adult", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Transplant", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Transplant", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Vision Non Routine", BenefitCategory2 = "Medical Condition", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Vision Non Routine", BenefitCategory2 = "Medical Condition", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Vision Non Routine", BenefitCategory2 = "Cataracts", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Vision Non Routine", BenefitCategory2 = "Cataracts", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Wigs", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Wigs", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Acupuncture", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "AU", ServiceCode = "", OfficeVisitChargeOnly = "NA", ApplyCopayType = "", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Acupuncture", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "AU", ServiceCode = "", OfficeVisitChargeOnly = "NA", ApplyCopayType = "", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Male Birth Control", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "B5", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Male Birth Control", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "B5", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Chemotherapy & Radiation", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Chemotherapy & Radiation", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Dental Accident", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "DS", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Dental Accident", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "DS", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Allergy", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "AT", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Allergy", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "AT", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Dental Non Accident", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - PCP", Category = "", ServiceCode = "DS", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Dental Non Accident", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "DS", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_AFTERCHANGE.Add(new Benefits { BenefitCategory1 = "Physicians Office Visit", BenefitCategory2 = "SPECIALIST", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge - Specialist", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                #endregion

                #region Medical Services To be effected, BEFORE CHANGE
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Physicians Office Visit", BenefitCategory2 = "PCP", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Dialysis or Hemodialysis", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Durable Medical Equipment", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Education", BenefitCategory2 = "Instruction", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "ES", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Hearing", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "HA", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Infertility Services Diagnostic", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "IL", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Infertility Services  Treatment", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "IF", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Massage Therapy", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Maternity", BenefitCategory2 = "Not Prenatal", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Musculoskeletal", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "MC", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Obesity Weight Control", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "WE", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Obesity Surgical", BenefitCategory2 = "Surgical", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "GB", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "OB-GYN", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "GB", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Orthotics", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "OH", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Foot Orthotics", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "FO", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Orthopedic Shoes", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "ON", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Podiatry", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NOA", ApplyCoinsurance = "NOA", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Prosthetic Appliances", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "PR", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Pulmonary Rehab", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Psychiatric", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Biofeedback", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "AU", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Substance Abuse", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Female Birth Control", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "B5", OfficeVisitChargeOnly = "NA", ApplyCopayType = "", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Routine Preventive - Exams", BenefitCategory2 = "Health Maintenance", BenefitCategory3 = "Routine Physical Exam Only", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "NA", ApplyCopayType = "", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes" });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Routine Preventive - Exams", BenefitCategory2 = "Health Maintenance", BenefitCategory3 = "Routine OB/GYN Exam Only", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "NA", ApplyCopayType = "", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes" });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Second Surgical Opinion", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Sleep Disorders", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "SD", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Sleep Disorders", BenefitCategory2 = "Sleep Study", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "SD", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Smoking Cessation", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "SC", ServiceCode = "", OfficeVisitChargeOnly = "NA", ApplyCopayType = "", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Therapy Services", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "TY", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "TMJ Services", BenefitCategory2 = "Non Surgical", BenefitCategory3 = "Pediatric", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "TMJ Services", BenefitCategory2 = "Surgical", BenefitCategory3 = "Pediatric", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "TMJ Services", BenefitCategory2 = "Non Surgical", BenefitCategory3 = "Adult", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "TMJ Services", BenefitCategory2 = "Surgical", BenefitCategory3 = "Adult", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Transplant", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Vision Non Routine", BenefitCategory2 = "Medical Condition", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Vision Non Routine", BenefitCategory2 = "Cataracts", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Wigs", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Acupuncture", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "AU", ServiceCode = "", OfficeVisitChargeOnly = "NA", ApplyCopayType = "", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Male Birth Control", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "B5", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Chemotherapy & Radiation", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Dental Accident", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "DS", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Allergy", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "AT", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "", Non_Grandfathered = "Yes", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Dental Non Accident", BenefitCategory2 = "", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "DS", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "PCP", ApplyDeductible = "YES", ApplyCoinsurance = "YES", Grandfathered = "", Non_Grandfathered = "", });
                benefits_BEFORECHANGE.Add(new Benefits { BenefitCategory1 = "Physicians Office Visit", BenefitCategory2 = "SPECIALIST", BenefitCategory3 = "", OfficeVisitCategory = "Office Visit Charge", Category = "", ServiceCode = "", OfficeVisitChargeOnly = "Copay", ApplyCopayType = "Specialist", ApplyDeductible = "NO", ApplyCoinsurance = "NO", Grandfathered = "Yes", Non_Grandfathered = "Yes" });
                #endregion

                return benefits_BEFORECHANGE;
            }
        }

        public class FieldsInRepo
        {
            public string BM_NtwInfo_Ntwrk { get; set; }
            public string BM_NtwInfo_OutNtwrk1 { get; set; }
            public string BM_NtwInfo_OutNtwrk2 { get; set; }
            public string BM_NtwInfo_UCRval { get; set; }
            public string BM_PreBen_34DSRSplMedi { get; set; }
        }

        public static void ApplyMarkupAsHTMLForRelevantElements(XmlDocument doc)
        {
            List<FieldsInRepo> FieldsInRepodata = new List<FieldsInRepo>();
            FieldsInRepodata.Add(new FieldsInRepo());
            FieldsInRepodata[0].BM_NtwInfo_Ntwrk = "<span>Add</span><a href='www.google.co.in'>www.google2.co.in</a>";
            FieldsInRepodata[0].BM_NtwInfo_OutNtwrk1 = "<span>Add</span><a href='www.google2.co.in'>www.google2.co.in</a>";
            FieldsInRepodata[0].BM_NtwInfo_OutNtwrk2 = "This is not Html.";
            FieldsInRepodata[0].BM_NtwInfo_UCRval = "<span>Add</span><a href='www.google4.co.in'>www.google4.co.in</a>";
            FieldsInRepodata[0].BM_PreBen_34DSRSplMedi = "<span>Add</span><a href='www.google5.co.in'>www.google5.co.in</a>";


                                
            //      <Value>=Fields!PropName.Value</Value>
            XmlNodeList nodeList;
            XmlElement root = doc.DocumentElement;
            var NamespaceURI = root.NamespaceURI;
            var NamespaceURI_Prefix = root.GetPrefixOfNamespace(NamespaceURI);
            NamespaceURI_Prefix = string.IsNullOrEmpty(NamespaceURI_Prefix) ? "rpp" : NamespaceURI_Prefix;

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace(NamespaceURI_Prefix, NamespaceURI);
            
            //nsmgr.AddNamespace("rpt", "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
            //nsmgr.AddNamespace("rd", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");

            string xPath = "//" + NamespaceURI_Prefix + ":Report/" + NamespaceURI_Prefix + ":Body/" + NamespaceURI_Prefix + ":ReportItems";
            nodeList = root.SelectNodes(xPath, nsmgr);

            foreach (var prop in FieldsInRepodata[0].GetType().GetProperties())
            {
                var propValue = prop.GetValue(FieldsInRepodata[0]);
                List<string> HTMLTags=new List<string>();
                HTMLTags.Add("</a>");   HTMLTags.Add("</span>");
                var TextRunList = root.GetElementsByTagName("TextRun");

                var gfasha = new XmlNamespaceManager(doc.NameTable).DefaultNamespace;
                var gSghdgcgsd = doc.DocumentElement.GetNamespaceOfPrefix("rd");

               // var DefaultNameList = root.GetElementsByTagName("DefaultName",);

                //DefaultName

                if (HTMLTags.Where(i => propValue.ToString().Contains(i)).Any())
                {
                    string PropName = prop.Name;
                    string TextToFind = @"Fields!"+PropName+".Value";
                    var TextRun = TextRunList.Cast<XmlNode>().ToList().Where(i => i.InnerXml.ToString().Contains(TextToFind)).FirstOrDefault();
                    if (TextRun != null)
                    {
                        string TextRun_Content = TextRun.InnerXml;
                        XmlNode MarkupType = doc.CreateNode(XmlNodeType.Element, "MarkupType", NamespaceURI);
                        MarkupType.InnerText = "HTML";
                        //MarkupType.InnerXml = "<MarkupType>HTML</MarkupType>";
                        TextRun.AppendChild(MarkupType);
                        doc.Save("BenefitMatrixReportNT" + DateTime.Now.ToString("_yyyyMMddhhmmss") + ".rdlc");
                    }

                }
            }
            //doc.Save("BenefitMatrixReportNT" + DateTime.Now.ToString("_yyyyMMddhhmmss") + ".rdlc");

        }






        public static void Update_MasterList()
        {
            Result.Append("\n-------------------------------Master List----------------------------\n");
            string command = @"select top 1 ins.FormInstanceID,FormData from ui.formdesign dess
                               inner join fldr.FormInstance ins on dess.FormID=ins.FormDesignID
                               inner join fldr.FormInstanceDataMap insdata on insdata.FormInstanceID=ins.FormInstanceID
                               where FormName='MasterList' and dess.IsActive=1 and ins.IsActive=1";

            //"select formdata from  fldr.forminstancedatamap where forminstanceid=870";
            //"select formdata from fldr.forminstancedatamap where FormInstanceDataMapID=9"; 

            #region "Psychiatric  (Inpatient)"   -->  BRG has one less record than Additional Services + Standard Services.

            //command = "select FormInstanceID,formdata from  fldr.forminstancedatamap where forminstanceid=935";
            //GetJsonFromDB(command, Product);

            //JObject RootData2 = JObject.Parse(Json);
            //string JSON_String2 = RootData2.ToString(Newtonsoft.Json.Formatting.Indented);
            //var BenefitReviewGridList = RootData2.SelectToken(BenefitReviewGrid).ToList();
            //var count = BenefitReviewGridList.ToList().Count();


            //var AdditionalServicesList = RootData2.SelectToken(AdditionalServices).ToList();
            ////JToken StandardServices_NonGF = StandardServices.Where(i => (string)i.SelectToken(MandateName) == NonGrandFathered).FirstOrDefault();
            //var NonGFDList = RootData2["StandardServices"]["StandardServiceList"].ToList()[0]["MasterListMedicalServices"].ToList();//StandardServices_NonGF[MasterListMedicalServices].ToList();

            //var Additional_plus_Standarad = AdditionalServicesList.Union(NonGFDList).ToList();

            //GetServicesWithOfficeVisitCharge(BenefitReviewGridList, Additional_plus_Standarad);
            #endregion





            GetJsonFromDB(command, MasterList);

            JObject RootData = JObject.Parse(Json);
            string JSON_String = RootData.ToString(Newtonsoft.Json.Formatting.Indented);

            Update_MedicalServices(RootData);
            Update_StandardServices(RootData, StandardServices_MasterList);
            Update_StandardLimits(RootData, MasterList);
            DeleteServiceFromStandardServicesInMasterList();

            string updatedData = "'" + RootData.ToString(Newtonsoft.Json.Formatting.None).Replace("'", "''") + "'";
            updatedData = "update  fldr.forminstancedatamap set formdata=" + updatedData + " where forminstanceid= " + MasterListFormInstanceID;
            UpdateJsonInDB(updatedData);
        }

        public static void Update_Product()
        {
            Result.Append("\n\n-------------------------------Products----------------------------");
            List<int> Products = new List<int>();
            string allProducts = @"select ins.FormInstanceID from ui.formdesign dess
                                 inner join fldr.FormInstance ins on dess.FormID=ins.FormDesignID
                                 inner join fldr.FormInstanceDataMap insdata on insdata.FormInstanceID=ins.FormInstanceID
                                 where FormName='Medical' and dess.IsActive=1 and ins.IsActive=1";

            //allProducts += " and ins.FormInstanceID in (931, 935)";

            string currentProduct = "select FormInstanceID,formdata from  fldr.forminstancedatamap where forminstanceid={0}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = new SqlCommand(allProducts, conn))
                {
                    conn.Open();
                    using (SqlDataReader rd = comm.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            Products.Add((int)rd[0]);
                        }
                    }
                }
            }

            foreach (var product in Products)
            {
                Result.Append("\n");
                GetJsonFromDB(string.Format(currentProduct, product), Product);
                JObject RootData = JObject.Parse(Json);
                string JSON_String = RootData.ToString(Newtonsoft.Json.Formatting.Indented);

                Update_AdditionalServices(RootData);
                //Update_StandardServices(RootData, StandardServices_Product);
                //Update_StandardLimits(RootData, Product);
                //Update_BRG(RootData);

                // EQN-1339 Request #8
                // ProductFormInstanceID = product  for add forminstanceid in text file
                //ProductFormInstanceID = product;
                //Add_StandardServicesNonGF(RootData, StandardServices_Product);
                // EQN-1339 Request #10
                //Update_NetWorkPriceLink_ASAAetna(RootData, StandardServices_Product);

                string updatedData = "'" + RootData.ToString(Newtonsoft.Json.Formatting.None).Replace("'", "''") + "'";

                updatedData = "update  fldr.forminstancedatamap set formdata=" + updatedData + " where forminstanceid= " + ProductFormInstanceID;
                UpdateJsonInDB(updatedData);
                string finsihed = string.Empty;
            }
        }

        public static void Update_VisionProducts()
        {
            Result.Append("\n\n-------------------------------Products----------------------------");
            List<int> Products = new List<int>();
            string allProducts = @"select ins.FormInstanceID from ui.formdesign dess
                                 inner join fldr.FormInstance ins on dess.FormID=ins.FormDesignID
                                 inner join fldr.FormInstanceDataMap insdata on insdata.FormInstanceID=ins.FormInstanceID
                                 where FormName='Vision' and dess.IsActive=1 and ins.IsActive=1";

            //allProducts += " and ins.FormInstanceID in (931, 935)";

            string currentProduct = "select FormInstanceID,formdata from  fldr.forminstancedatamap where forminstanceid={0}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = new SqlCommand(allProducts, conn))
                {
                    conn.Open();
                    using (SqlDataReader rd = comm.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            Products.Add((int)rd[0]);
                        }
                    }
                }
            }

            foreach (var product in Products)
            {
                Result.Append("\n");
                GetJsonFromDB(string.Format(currentProduct, product), Product);
                JObject RootData = JObject.Parse(Json);
                string JSON_String = RootData.ToString(Newtonsoft.Json.Formatting.Indented);

                Update_BRGCostTierLabels(RootData);
                //Update_StandardServices(RootData, StandardServices_Product);
                //Update_StandardLimits(RootData, Product);
                //Update_BRG(RootData);

                // EQN-1339 Request #8
                // ProductFormInstanceID = product  for add forminstanceid in text file
                //ProductFormInstanceID = product;
                //Add_StandardServicesNonGF(RootData, StandardServices_Product);
                // EQN-1339 Request #10
                //Update_NetWorkPriceLink_ASAAetna(RootData, StandardServices_Product);

                string updatedData = "'" + RootData.ToString(Newtonsoft.Json.Formatting.None).Replace("'", "''") + "'";

                updatedData = "update  fldr.forminstancedatamap set formdata=" + updatedData + " where forminstanceid= " + ProductFormInstanceID;
                UpdateJsonInDB(updatedData);
                string finsihed = string.Empty;
            }
        }


        public static void Update_BRGCostTierLabels(JObject RootData)
        {
            var BRGData = RootData.SelectToken(BenefitReviewGrid);
            if (BRGData != null && BRGData.Any())
            {
                foreach (JToken service in BRGData)
                {
                    var MasterListVisionNetworkTier = service["MasterListVisionNetworkTier"];
                    if (MasterListVisionNetworkTier != null && MasterListVisionNetworkTier.Any())
                    {
                        var NetworkTiers = MasterListVisionNetworkTier;
                        foreach (JToken Tier in NetworkTiers)
                        {
                            string Network = Convert.ToString(Tier["Network"]) ?? "";

                            if (!string.IsNullOrEmpty(Network))
                            {
                                switch (Network)
                                {
                                    case "NonNetwork":
                                        Tier["Network"] = "Non Network";
                                        break;
                                    case "OutofArea":
                                        Tier["Network"] = "Out of Area";
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                    }
                }
            }
        }

        public static void Update_ServicesWithOfficeVisitChargeAsNAandApplyCopayTypeAsPCP_ToHaveApplyCopayTypeAsBlank(JToken data)
        {
            // If change is needed for Products too then comment out below two lines
            var dataToUpdate = data.Where(i => (((string)i.SelectToken(OfficeVisitChargeOnly)) ?? "").ToUpper() == "NA"
                                            && (((string)i.SelectToken(ApplyCopayType)) ?? "").ToUpper() == "PCP");

            // If change is needed for Products too then uncomment out below code.
            //var dataToUpdate = data.Where(i => Benefits.benefits_WithOfficeVisitChargeOnlyAsNAandApplyCopayTypeAsPCP
            //                             .Where(j => j.BenefitCategory1 == (string)i.SelectToken(BenefitCategory1)
            //                                      && j.BenefitCategory2 == (string)i.SelectToken(BenefitCategory2)
            //                                      && j.BenefitCategory3 == (string)i.SelectToken(BenefitCategory3)
            //                                      && j.OfficeVisitCategory == (string)i.SelectToken(OfficeVisitCategory)
            //                                      ).Any()
            //                          );

            foreach (var item in dataToUpdate)
            {
                item[ApplyCopayType] = "";
            }
        }

        public static void Update_MedicalServices(JObject RootData)
        {
            MEDICALSERVICES = (RootData.SelectToken(MedicalServices));
            Update_ServicesWithOfficeVisitChargeAsNAandApplyCopayTypeAsPCP_ToHaveApplyCopayTypeAsBlank(MEDICALSERVICES);

            List<JToken> services = GetServicesWithOfficeVisitCharge(MEDICALSERVICES);

            if (services.Any())
            {
                ToBeUpdatedCount = services.Count();

                UpdatedCount = UpdateExistingRecordToPCPandCreateSpecialistRecord(services, MEDICALSERVICES, MedicalServices);

                Result.Append("There were [" + UpdatedCount.ToString() + "] Medical Services found and Updated in the Master List." + "\n");
            }
        }

        public static void Update_StandardServices(JObject RootData, string For_Product_OR_MasterList = "")
        {
            List<JToken> StandardServices = RootData.SelectToken(For_Product_OR_MasterList).ToList();
            JToken StandardServices_GF = null;
            JToken StandardServices_NonGF = null;

            StandardServices_GF = StandardServices.Where(i => (string)i.SelectToken(For_Product_OR_MasterList == StandardServices_MasterList ? MandateName : ServiceList) == GrandFathered).FirstOrDefault();
            StandardServices_NonGF = StandardServices.Where(i => (string)i.SelectToken(For_Product_OR_MasterList == StandardServices_MasterList ? MandateName : ServiceList) == NonGrandFathered).FirstOrDefault();

            if (StandardServices_GF != null)
            {
                STANDARDSERVICES_GF = StandardServices_GF.SelectToken(MasterListMedicalServices); //[MasterListMedicalServices]

                // Uncomment the below if condition, if needed
                if (For_Product_OR_MasterList == StandardServices_MasterList)
                    Update_ServicesWithOfficeVisitChargeAsNAandApplyCopayTypeAsPCP_ToHaveApplyCopayTypeAsBlank(STANDARDSERVICES_GF);

                if (STANDARDSERVICES_GF.Any())
                {
                    GrandFatheredList = GetServicesWithOfficeVisitCharge(STANDARDSERVICES_GF, GrandFathered);
                    ToBeUpdatedCount = GrandFatheredList.Count();
                    if (GrandFatheredList.Any())
                    {
                        UpdatedCount = UpdateExistingRecordToPCPandCreateSpecialistRecord(GrandFatheredList, STANDARDSERVICES_GF);

                        Result.Append("There were [" + UpdatedCount.ToString() + "]  Grandfathered Services found and Updated in the " + (For_Product_OR_MasterList == StandardServices_MasterList ? "Master List" : "Product { " + ProductFormInstanceID + " }") + "\n");
                    }
                }
            }

            if (StandardServices_NonGF != null)
            {
                STANDARDSERVICES_NonGF = StandardServices_NonGF[MasterListMedicalServices];

                // Uncomment the below if condition, if needed
                if (For_Product_OR_MasterList == StandardServices_MasterList)
                    Update_ServicesWithOfficeVisitChargeAsNAandApplyCopayTypeAsPCP_ToHaveApplyCopayTypeAsBlank(STANDARDSERVICES_NonGF);

                if (STANDARDSERVICES_NonGF.Any())
                {
                    NonGrandFatheredList = GetServicesWithOfficeVisitCharge(STANDARDSERVICES_NonGF, NonGrandFathered);
                    if (NonGrandFatheredList.Any())
                    {
                        UpdateExistingRecordToPCPandCreateSpecialistRecord(NonGrandFatheredList, STANDARDSERVICES_NonGF);

                        Result.Append("There were [" + UpdatedCount.ToString() + "]  Non-Grandfathered Services found and Updated in the " + (For_Product_OR_MasterList == StandardServices_MasterList ? "Master List" : "Product { " + ProductFormInstanceID + " }") + "\n");
                    }
                }
            }
        }

        public static void Add_StandardServicesNonGF(JObject RootData, string For_Product_OR_MasterList = "")
        {
            List<JToken> StandardServices = RootData.SelectToken(For_Product_OR_MasterList).ToList();
            JToken StandardServices_NonGF = null;
            StandardServices_NonGF = StandardServices.Where(i => (string)i.SelectToken(For_Product_OR_MasterList == StandardServices_MasterList ? MandateName : ServiceList) == NonGrandFathered).FirstOrDefault();

            if (StandardServices_NonGF != null)
            {
                STANDARDSERVICES_NonGF = StandardServices_NonGF[MasterListMedicalServices];
                if (STANDARDSERVICES_NonGF.Any())
                {

                    JObject newService = JObject.Parse(@"{
            						""SelectServices"": ""Yes"",
            						""BenefitCategory1"": ""Preadmission Testing"",
            						""BenefitCategory2"": """",
            						""BenefitCategory3"": """",
            						""PlaceofService"": """",
            						""OfficeVisitCategory"": """",
            						""Category"": """",
            						""ServiceCode"": """",
            						""OfficeVisitChargeOnly"": """",
            						""ApplyCopayType"": """",
            						""ApplyDeductible"": """",
            						""ApplyCoinsurance"": """"
            					}");

                    STANDARDSERVICES_NonGF[0].AddAfterSelf(newService);
                }
            }
        }

        public static void Update_NetWorkPriceLink_ASAAetna(JObject RootData, string For_Product_OR_MasterList = "")
        {
            List<JToken> selectPlanNetwork = RootData.SelectToken("Network.NetworkInformation.SelectthePlansNetworks").ToList();
            foreach (var item in selectPlanNetwork)
            {
                if (Convert.ToString(item["MasterNetworkName"]) == "ASA AETNA")
                {
                    var subNet = item["MasterListSubNetworkList"] ?? null;
                    if (null != subNet)
                    {
                        foreach (var data in subNet)
                        {
                            data["NetworkNumber"] = "1432";
                            data["NetworkName"] = "ASA AETNA";
                            data["PPOComment"] = "1432";
                            data["PricingMethod"] = "REPRICED ";
                        }
                        Result.Append(ProductFormInstanceID + "\n");
                    }
                }
            }
        }

        public static void DeleteServiceFromStandardServicesInMasterList()
        {
            var ServiceToBeDeleted = STANDARDSERVICES_NonGF.ToList().Where(i => (string)i.SelectToken(BenefitCategory1) == StandardServiceToBeDeletedFromMasterList).FirstOrDefault();
            if (ServiceToBeDeleted != null)
                ServiceToBeDeleted.Remove();
        }

        public static void Update_AdditionalServices(JObject RootData)
        {

            UpdatedCount = 0; ToBeUpdatedCount = 0;
            ADDITIONALSERVICES = (RootData.SelectToken(AdditionalServices));

            string structure = ADDITIONALSERVICES.ToList()[0].ToString();
            JObject structureObject = JObject.Parse(structure);
            structureObject["Selecttheapplicableservice"] = "";
            structureObject["BenefitCategory1"] = "Post Surgical Bras";
            structureObject["BenefitCategory2"] = "";
            structureObject["BenefitCategory3"] = "";
            structureObject["PlaceofService"] = "";
            structureObject["OfficeVisitCategory"] = "";
            structureObject["Category"] = "";
            structureObject["ServiceCode"] = "";
            structureObject["OfficeVisitChargeOnly"] = "";
            structureObject["ApplyCopayType"] = "";
            structureObject["ApplyDeductible"] = "";
            structureObject["ApplyCoinsurance"] = "";
            ADDITIONALSERVICES[0].AddAfterSelf(structureObject);

            //Update BRG
            var BRGData = RootData.SelectToken(BenefitReviewGrid);
            if (BRGData.Count() > 0)
            {
                structure = BRGData.ToList()[0].ToString();
                structureObject = JObject.Parse(structure);

                structureObject["BenefitCategory1"] = "Post Surgical Bras";
                structureObject["BenefitCategory2"] = "";
                structureObject["BenefitCategory3"] = "";
                structureObject["PlaceofService"] = "";
                structureObject["OfficeVisitCategory"] = "";
                structureObject["Category"] = "";
                structureObject["ServiceCode"] = "";
                structureObject["ApplyCopayType"] = "";
                structureObject["ApplyDeductible"] = "";
                structureObject["ApplyCoinsurance"] = "";
                structureObject["Limits"] = "";

                var IQMedicalPlanNetwork = structureObject["IQMedicalPlanNetwork"].ToList();
                foreach (var NetworkData in IQMedicalPlanNetwork)
                {
                    NetworkData["Copay"] = "Not Applicable";
                    NetworkData["Coinsurance"] = "Not Applicable";
                    NetworkData["IndividualDeductible"] = "Not Applicable";
                    NetworkData["FamilyDeductible"] = "Not Applicable";
                    NetworkData["Other1Deductible"] = "";
                    NetworkData["Other2Deductible"] = "";
                    NetworkData["IndividualOOPM"] = "";
                    NetworkData["FamilyOOPM"] = "";
                    NetworkData["Other1OOPM"] = "";
                    NetworkData["Other2OOPM"] = "";
                }
                BRGData[0].AddAfterSelf(structureObject);
            }
            // Uncomment the below line, if needed
            //Update_ServicesWithOfficeVisitChargeAsNAandApplyCopayTypeAsPCP_ToHaveApplyCopayTypeAsBlank(ADDITIONALSERVICES);

            //List<JToken> services = GetServicesWithOfficeVisitCharge(ADDITIONALSERVICES);

            //if (services.Any())
            //{
            //    ToBeUpdatedCount = services.Count();
            //    UpdatedCount = UpdateExistingRecordToPCPandCreateSpecialistRecord(services, ADDITIONALSERVICES, MedicalServices);

            //    Result.Append("There were [" + UpdatedCount.ToString() + "] Additional Services found and Updated in the Product { "+ ProductFormInstanceID + " }\n");
            //}
        }

        /// <summary>
        /// Updates Standard Limits for the Product as well as the Master List depending on the argument specified for For_Product_OR_MasterList parameter.
        /// </summary>
        /// <param name="RootData"></param>
        /// <param name="For_Product_OR_MasterList"></param>
        public static void Update_StandardLimits(JObject RootData, string For_Product_OR_MasterList = "")
        {
            UpdatedCount = 0;
            var SLimit = RootData.SelectToken(StandardLimits).ToList();
            List<JToken> StandardLimit = new List<JToken>();
            if (SLimit.Any())
            {
                foreach (var limit in SLimit)
                {
                    var MasterListMedicalService = limit.SelectToken(MasterListMedicalServices);
                    StandardLimit = GetServicesWithOfficeVisitCharge(MasterListMedicalService).ToList();

                    if (StandardLimit.Any())
                        UpdatedCount += UpdateExistingRecordToPCPandCreateSpecialistRecord(StandardLimit, MasterListMedicalService);
                }

                Result.Append("There were [" + UpdatedCount.ToString() + "] Standard Limit services found and Updated in the " + (For_Product_OR_MasterList == Product ? "Product { " + ProductFormInstanceID + " }" : "Master List") + "\n");
            }
        }

        /// <summary>
        /// Gets the Services that needs to be updated, listed down in the spreadsheet provided by Mandy for this Requirement.
        /// </summary>
        /// <param name="serviceList"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public static List<JToken> GetServicesWithOfficeVisitCharge(JToken serviceList, string service = "")
        {
            var MedicalServicesToBeChanged = Benefits.GetBenefitsToApplyChangeTo();

            var services = serviceList.Where(i => MedicalServicesToBeChanged
                                                     .Where(j => j.BenefitCategory1 == (string)i.SelectToken(BenefitCategory1)
                                                              && j.BenefitCategory2 == (string)i.SelectToken(BenefitCategory2)
                                                              && j.BenefitCategory3 == (string)i.SelectToken(BenefitCategory3)
                                                              && j.OfficeVisitCategory == (string)i.SelectToken(OfficeVisitCategory)
                                                              && ((service == GrandFathered && (!string.IsNullOrEmpty(j.Grandfathered))) ||            // GrandFathered Services
                                                                   (service == NonGrandFathered && (!string.IsNullOrEmpty(j.Non_Grandfathered))) ||     // Non-GrandFathered Services
                                                                   string.IsNullOrEmpty(service))
                                                              ).Any()
                                                  ).ToList();

            var Services_with_OfficeVisitCharge = serviceList.Where(i => (string)i.SelectToken(OfficeVisitCategory) == "Office Visit Charge").ToList();

            var exceptionsToUpdate = services.Except(Services_with_OfficeVisitCharge);

            return services;
        }

        public static void GetCostShareValues(JObject RootData)
        {
            var Copay = RootData.SelectToken(UpdateServices.Copays);
            bool Doesthisplanhaveacopay = (Convert.ToString(Copay.SelectToken(UpdateServices.Doesthisplanhaveacopay)) ?? "").ToLower() == "true";
            if (Doesthisplanhaveacopay)
            {
                var CopayList = Copay.SelectToken(UpdateServices.CopayList).ToList();
                if (CopayList.Any())
                {
                    CopayValues = CopayList;

                }
            }
        }

        /// <summary>
        /// Splits, Benefit Review Grid Services with [Office Visit Category]= "Office Visit Charge" to have the PCP and Specialist pair.
        /// </summary>
        /// <param name="RootData"></param>
        public static void Update_BRG(JObject RootData)
        {
            UpdatedCount = 0;
            string ExistingApplyCopayTypeinBRG = string.Empty;

            var BRGData = RootData.SelectToken(BenefitReviewGrid);

            // Uncomment the below line, if needed
            // Update_ServicesWithOfficeVisitChargeAsNAandApplyCopayTypeAsPCP_ToHaveApplyCopayTypeAsBlank(BRGData);

            BenefitReviewGridData = GetServicesWithOfficeVisitCharge(BRGData);
            if (BenefitReviewGridData.Any())
            {
                CopayValues = new List<JToken>();
                GetCostShareValues(RootData);

                foreach (var data in BenefitReviewGridData)
                {
                    var temp = data.ToString();
                    JObject RecordToBeAdded = JObject.Parse(temp);
                    ExistingApplyCopayTypeinBRG = (((string)data.SelectToken(ApplyCopayType)) ?? "");
                    UpdateRecordsinBRG(data, RecordToBeAdded, BRGData, ExistingApplyCopayTypeinBRG);
                }

                Result.Append("There were [" + UpdatedCount.ToString() + "] BRG services found and Updated in the Product (" + ProductFormInstanceID + ")" + "\n");
            }
        }

        /// <summary>
        /// Recieves service record, one at a time, from BRG and based on the ApplyCopayType of the recieved record, the [OfficeVisitCategory] for the existing record 
        /// is set either to "Office Visit Charge - PCP" or to "Office Visit Charge - Specialist" (For a PCP recived record it's set to "Office Visit Charge - PCP").
        /// This is followed by the creation of the other record to have the pair of records present in BRG for this service.(i.e. same service with "Office Visit Charge - PCP" and "Office Visit Charge - Specialist")
        /// if the Copay value for the Existing service record was one from the Cost Share section then the created record inherits the value corresponding to it from Cost Share Section itself, in all other cases the created record has same copay value as that of the existing record.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="RecordToBeAdded"></param>
        /// <param name="ExistingApplyCopayTypeinBRG"></param>
        public static void UpdateRecordsinBRG(JToken existingRecord, JObject RecordToBeAdded, JToken BenefitReviewGridData, string ExistingApplyCopayTypeinBRG = "")
        {
            string ExistingCopayValueForCurrentNetworkinBRG = string.Empty;
            string PCPValueForCurrentNetworkFromCostShare = string.Empty;
            string SpecialistValueForCurrentNetworkFromCostShare = string.Empty;
            Regex numbers = new System.Text.RegularExpressions.Regex(@"[\d]+");

            string BC1 = (string)existingRecord[BenefitCategory1];
            string BC2 = (string)existingRecord[BenefitCategory2];
            string BC3 = (string)existingRecord[BenefitCategory3];
            string OVC = (string)existingRecord[OfficeVisitCategory];

            if (BC1 == "Psychiatric")
            {
            }

            #region Exceptional services
            if (BC1 == "Physicians Office Visit" && BC2 == "SPECIALIST")
            {
                existingRecord[OfficeVisitCategory] = OfficeVisitChargeSpecialist;
                existingRecord[ApplyCopayType] = (ExistingApplyCopayTypeinBRG == Specialist || ExistingApplyCopayTypeinBRG == PCP) ? Specialist : ExistingApplyCopayTypeinBRG;
                UpdatedCount++;
            }

            else if ((BC1 == "Physicians Office Visit" && BC2 == "PCP") || BC1 == "Routine Preventive - Exams")
            {
                existingRecord[OfficeVisitCategory] = OfficeVisitChargePCP;
                existingRecord[ApplyCopayType] = (ExistingApplyCopayTypeinBRG == Specialist || ExistingApplyCopayTypeinBRG == PCP) ? PCP : ExistingApplyCopayTypeinBRG;
                UpdatedCount++;
            }
            #endregion

            else
            {
                existingRecord[OfficeVisitCategory] = ExistingApplyCopayTypeinBRG == Specialist ? OfficeVisitChargeSpecialist : OfficeVisitChargePCP;
                RecordToBeAdded[OfficeVisitCategory] = (string)existingRecord[OfficeVisitCategory] == OfficeVisitChargeSpecialist ? OfficeVisitChargePCP : OfficeVisitChargeSpecialist;

                if ((ExistingApplyCopayTypeinBRG == Specialist || ExistingApplyCopayTypeinBRG == PCP) && CopayValues.Any())
                {
                    RecordToBeAdded[ApplyCopayType] = ExistingApplyCopayTypeinBRG == Specialist ? PCP : Specialist;
                    var networks = existingRecord[IQMedicalPlanNetwork].ToList();
                    var PCPRecordFromCostShare = CopayValues.Where(i => (((string)i.SelectToken(CopayType)) ?? "").ToLower() == PCP.ToLower()).FirstOrDefault()[IQMedicalPlanNetwork].ToList();
                    var SpecialistRecordFromCostShare = CopayValues.Where(i => (((string)i.SelectToken(CopayType)) ?? "").ToLower() == Specialist.ToLower()).FirstOrDefault()[IQMedicalPlanNetwork].ToList();

                    foreach (var networkRecord in networks)
                    {
                        string Network = ((string)networkRecord.SelectToken(CostShareTier)) ?? "";
                        var PCPRecordForCurrentNetwork = PCPRecordFromCostShare.Where(i => (string)i.SelectToken(CostShareTier) == Network).FirstOrDefault(); //CostShareTier   Where((string)i.SelectToken(CopayType) == Network).FirstOrDefault();
                        PCPValueForCurrentNetworkFromCostShare = ((string)PCPRecordForCurrentNetwork[Amount]) ?? "";

                        var SpecialistRecordForCurrentNetwork = SpecialistRecordFromCostShare.Where(i => (string)i.SelectToken(CostShareTier) == Network).FirstOrDefault(); //CostShareTier   Where((string)i.SelectToken(CopayType) == Network).FirstOrDefault();
                        SpecialistValueForCurrentNetworkFromCostShare = ((string)SpecialistRecordForCurrentNetwork[Amount]) ?? "";

                        ExistingCopayValueForCurrentNetworkinBRG = ((string)(networkRecord[Copay])) ?? "";

                        string ApplicableCostShareValue = ExistingApplyCopayTypeinBRG == Specialist ? SpecialistValueForCurrentNetworkFromCostShare : PCPValueForCurrentNetworkFromCostShare;
                        if (ExistingCopayValueForCurrentNetworkinBRG == ApplicableCostShareValue)
                        {
                            JToken copayForCurrentNetwork = RecordToBeAdded[IQMedicalPlanNetwork].ToList().Where(i => (string)i.SelectToken(CostShareTier) == Network).FirstOrDefault();
                            if (copayForCurrentNetwork != null)
                                copayForCurrentNetwork[Copay] = ExistingApplyCopayTypeinBRG == Specialist ? PCPValueForCurrentNetworkFromCostShare : SpecialistValueForCurrentNetworkFromCostShare;
                        }
                    }
                }
                BenefitReviewGridData.ToList()[0].AddAfterSelf(RecordToBeAdded);
                UpdatedCount++;
            }
        }

        /// <summary>
        /// Splits, existing Services with [Office Visit Category]= "Office Visit Charge" to have the PCP and Specialist pair for    [Additional Services] , [Standard Services] in Product and Master List , [Standard Limits] in Product and Master List
        /// </summary>
        /// <param name="services"></param>
        /// <param name="section"></param>
        /// <param name="Service"></param>
        /// <returns></returns>
        public static int UpdateExistingRecordToPCPandCreateSpecialistRecord(List<JToken> services, JToken section, string Service = "")
        {
            UpdatedCount = 0;
            for (int i = 0; i < services.Count; i++)
            {
                string BC1 = (string)services[i][BenefitCategory1];
                string BC2 = (string)services[i][BenefitCategory2];
                string BC3 = (string)services[i][BenefitCategory3];
                string OVC = (string)services[i][OfficeVisitCategory];

                bool NeedsUpdating_ApplyCopayType = false;
                string OVCOnly = string.Empty;

                if (Service == MedicalServices)
                {
                    OVCOnly = ((string)services[i][OfficeVisitChargeOnly]) ?? "";
                    if (OVCOnly != null)
                        NeedsUpdating_ApplyCopayType = OVCOnly.ToLower() == "copay";
                }
                // For service with BenefitCategory1 = "Physicians Office Visit", BenefitCategory2 = "SPECIALIST"  update the record to be Specialist but don't create the PCP part
                if (BC1 == "Physicians Office Visit" && BC2 == "SPECIALIST")
                {
                    services[i][OfficeVisitCategory] = OfficeVisitChargeSpecialist;
                    if (NeedsUpdating_ApplyCopayType)
                        services[i][ApplyCopayType] = Specialist;
                    UpdatedCount++;
                    continue;
                }

                if ((BC1 == "Physicians Office Visit" && BC2 == "PCP") || BC1 == "Routine Preventive - Exams")
                {
                    services[i][OfficeVisitCategory] = OfficeVisitChargePCP;
                    if (NeedsUpdating_ApplyCopayType)
                        services[i][ApplyCopayType] = PCP;
                    UpdatedCount++;
                    continue;
                }

                // For Existing Record - PCP
                services[i][OfficeVisitCategory] = OfficeVisitChargePCP;
                if (NeedsUpdating_ApplyCopayType)
                    services[i][ApplyCopayType] = PCP;

                // For New Record - Specialist                          
                string temp = services[i].ToString();
                JObject CreateSpecialistRecord = JObject.Parse(temp);
                CreateSpecialistRecord[OfficeVisitCategory] = OfficeVisitChargeSpecialist;
                if (NeedsUpdating_ApplyCopayType)
                    CreateSpecialistRecord[ApplyCopayType] = Specialist;
                //if (BC1 == "Physicians Office Visit" && BC2 == "PCP")
                //    CreateSpecialistRecord[BenefitCategory2] = Specialist;
                section.ToList()[0].AddAfterSelf(CreateSpecialistRecord);

                UpdatedCount++;
            }

            return UpdatedCount;
        }

        /// <summary>
        /// Gets JSON for the specified FormInstanceId from DB.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="Is_MasterList_OR_Product"></param>
        /// <returns></returns>
        public static string GetJsonFromDB(string command, string Is_MasterList_OR_Product)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand comm = new SqlCommand(command, con);
                SqlDataReader rd = comm.ExecuteReader();
                if (!rd.HasRows)
                    Console.WriteLine("No Data Found");
                else
                {
                    while (rd.Read())
                    {

                        if (Is_MasterList_OR_Product == Product)
                            ProductFormInstanceID = (int)(rd[0]);
                        else
                            MasterListFormInstanceID = (int)(rd[0]);
                        Json = rd[1].ToString();
                    }
                }
                rd.Close();
                comm.Cancel();
                con.Close();
            }
            return Json;
        }

        public static int GetFormInstanceIDFromFormNameDB(string FormName)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand comm = new SqlCommand(FormName, con);
                SqlDataReader rd = comm.ExecuteReader();
                if (!rd.HasRows)
                    Console.WriteLine("No Data Found");
                else
                {
                    while (rd.Read())
                    {
                        return Convert.ToInt32(rd[0]);
                    }
                }
                rd.Close();
                comm.Cancel();
                con.Close();
            }
            return 200;
        }

        /// <summary>
        /// Updates the Instance for the specified FormInstanceId in DB.
        /// </summary>
        /// <param name="command"></param>
        public static void UpdateJsonInDB(string command)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand comm = new SqlCommand(command, con);
                comm.ExecuteNonQuery();

                comm.Cancel();
                con.Close();
            }
        }

        /// <summary>
        /// This method is not in use for the requirement
        /// </summary>
        /// <param name="serviceList1"></param>
        /// <param name="serviceList2"></param>
        public static void GetServicesWithOfficeVisitCharge(List<JToken> serviceList1, List<JToken> serviceList2)
        {
            var MedicalServicesToBeChanged = serviceList2;

            var res1 = serviceList1.Select(j => new
            {
                BC1 = (string)j.SelectToken(BenefitCategory1),
                BC2 = (string)j.SelectToken(BenefitCategory2),
                BC3 = (string)j.SelectToken(BenefitCategory3),
                OVC = (string)j.SelectToken(OfficeVisitCategory)
            });

            var res2 = serviceList2.Select(j => new
            {
                BC1 = (string)j.SelectToken(BenefitCategory1),
                BC2 = (string)j.SelectToken(BenefitCategory2),
                BC3 = (string)j.SelectToken(BenefitCategory3),
                OVC = (string)j.SelectToken(OfficeVisitCategory)
            });

            var result = res2.Except(res1);


            //var services = serviceList1.Where(i => MedicalServicesToBeChanged
            //                                         .Where(j => (string)j.SelectToken(BenefitCategory1) == (string)i.SelectToken(BenefitCategory1)
            //                                                  && (string)j.SelectToken(BenefitCategory2) == (string)i.SelectToken(BenefitCategory2)
            //                                                  && (string)j.SelectToken(BenefitCategory3) == (string)i.SelectToken(BenefitCategory3)
            //                                                  && (string)j.SelectToken(OfficeVisitCategory) == (string)i.SelectToken(OfficeVisitCategory)

            //                                                  ).Any()
            //                                      ).ToList();
        }

        #region EQN_1466:RequestNo38,39,40

        public static void Update_MedicalBRGProducts()
        {
            Result.Append("\n\n-------------------------------Products----------------------------");
            List<int> Products = new List<int>();
            string allProducts = @"select ins.FormInstanceID from ui.formdesign dess
                                 inner join fldr.FormInstance ins on dess.FormID=ins.FormDesignID
                                 inner join fldr.FormInstanceDataMap insdata on insdata.FormInstanceID=ins.FormInstanceID
                                 where FormName='Medical' and dess.IsActive=1 and ins.IsActive=1";


            string currentProduct = "select FormInstanceID,formdata from  fldr.forminstancedatamap where forminstanceid={0}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = new SqlCommand(allProducts, conn))
                {
                    conn.Open();
                    using (SqlDataReader rd = comm.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            Products.Add((int)rd[0]);
                        }
                    }
                }
            }

            foreach (var product in Products)
            {
                Result.Append("\n");
                GetJsonFromDB(string.Format(currentProduct, product), Product);
                JObject RootData = JObject.Parse(Json);
                string JSON_String = RootData.ToString(Newtonsoft.Json.Formatting.Indented);

                //Update_MedicalBRGdata(RootData);
                ////DeleteServiceFromBRGInMedical1(RootData);
                //DeleteServiceFromBRGInMedical2(RootData);
               // AddData_MedicalBRGdata(RootData);
                Update_MedicalCOBRetireCoverage(RootData);
                Update_InjuryMalformation(RootData);
                Update_precertificationddvalues(RootData);
                Update_BRGServiceHighTechDX(RootData);
                string updatedData = "'" + RootData.ToString(Newtonsoft.Json.Formatting.None).Replace("'", "''") + "'";
                updatedData = "update  fldr.forminstancedatamap set formdata=" + updatedData + " where forminstanceid= " + ProductFormInstanceID;
                UpdateJsonInDB(updatedData);
                string finsihed = string.Empty;
            }
        }
        public static void Update_MedicalBRGdata(JObject RootData)
        {
            var BRGData = RootData.SelectToken(BenefitReviewGrid);
            if (BRGData != null && BRGData.Any())
            {
                foreach (JToken service in BRGData)
                {
                    string BC1 = Convert.ToString(service["BenefitCategory1"]) ?? "";
                    string BC3 = Convert.ToString(service["BenefitCategory3"]) ?? "";
                    if (!string.IsNullOrEmpty(BC1) && !string.IsNullOrEmpty(BC3))
                    {
                        if (BC1 == "Emergency Room Treatment Illness & Accident HSB Emergency List")
                        {
                            service["BenefitCategory1"] = "Emergency Room Treatment Illness & Accident";

                            switch (BC3)
                            {
                                case "True Emergency HSB List":
                                    service["BenefitCategory3"] = "True Emergency";
                                    break;
                                case "Non True Emergency HSB List":
                                    service["BenefitCategory3"] = "Non True Emergency";
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                }
            }
        }

        public static void Update_BRGServiceHighTechDX(JObject RootData)
        {
            var BRGData = RootData.SelectToken(BenefitReviewGrid);
            if (BRGData != null && BRGData.Any())
            {
                var ServiceWithHighTechDX = BRGData.Where(i => (Convert.ToString(i.SelectToken("BenefitCategory1")) ?? "").ToLower().StartsWith("emer") && Convert.ToString(i.SelectToken("BenefitCategory2")) == "High Tech DX").FirstOrDefault();
                var ServiceWithHighTechDXL = BRGData.Where(i => (Convert.ToString(i.SelectToken("BenefitCategory1")) ?? "").ToLower().StartsWith("emer") && Convert.ToString(i.SelectToken("BenefitCategory2")) == "High Tech DXL" && (Convert.ToString(i.SelectToken("BenefitCategory3")) ?? "").Contains("diag")).FirstOrDefault();
                var ServiceWithPCP = BRGData.Where(i => (Convert.ToString(i.SelectToken("BenefitCategory1")) ?? "").ToLower().StartsWith("emer") && Convert.ToString(i.SelectToken("BenefitCategory2")) == "PCP").FirstOrDefault();

                if (ServiceWithHighTechDXL != null)
                {
                    if (null != ServiceWithHighTechDX)
                        ServiceWithHighTechDX.Remove();
                }
                else
                {
                    if (null != ServiceWithHighTechDX)
                        ServiceWithHighTechDX["BenefitCategory2"] = "High Tech DXL";
                }

                if (null != ServiceWithPCP)
                    ServiceWithPCP.Remove();
            }
        }

        //public static void Update_BRGServiceHighTechDX(JObject RootData)
        //{
        //    var BRGData = RootData.SelectToken(BenefitReviewGrid);
        //    if (BRGData != null && BRGData.Any())
        //    {
        //        foreach (JToken service in BRGData)
        //        {
        //            string BC2 = Convert.ToString(service["BenefitCategory2"]) ?? "";

        //            if (!string.IsNullOrEmpty(BC2))
        //            {
        //                //if (BC2 == "High Tech DX")
        //                //{
        //                //    service["BenefitCategory2"] = "High Tech DXL";

        //                //}
        //                switch (BC2)
        //                {
        //                    case "High Tech DX":
        //                        service["BenefitCategory2"] = "High Tech DXL";
        //                        break;
        //                    case "PCP":
        //                        service["BenefitCategory2"] = string.Empty;
        //                        break;
        //                    default:
        //                        break;
        //                }
        //            }

        //        }
        //    }
        //}

        public static void DeleteServiceFromBRGInMedical1(JObject RootData)
        {
            var BRGData = RootData.SelectToken(BenefitReviewGrid);

            if (BRGData != null && BRGData.Any())
            {
                BRGData.Where(i => i != null)
                       .Where(i => (string)i.SelectToken(BenefitCategory1) == "Emergency Room Treatment Illness & Accident"
                                && (string)i.SelectToken(BenefitCategory3) == "All Diagnosis")
                       .ToList()
                       .ForEach(i => i.Remove());
            }
        }
        public static void DeleteServiceFromBRGInMedical2(JObject RootData)
        {
            var BRGData = RootData.SelectToken(BenefitReviewGrid);

            if (BRGData != null && BRGData.Any())
            {
                BRGData.Where(i => i != null)
                       .Where(i => (string)i.SelectToken(BenefitCategory1) == "Male Birth Control"
                                && (string)i.SelectToken(BenefitCategory2) == "Elective Sterilization"
                                && (string)i.SelectToken(BenefitCategory3) != "Vasectomy")
                       .ToList()
                       .ForEach(i => i.Remove());
            }
        }
        public static void AddData_MedicalBRGdata(JObject RootData)
        {
            var BRGData = RootData.SelectToken(BenefitReviewGrid);
            if (BRGData != null && BRGData.Any())
            {
                foreach (JToken service in BRGData)
                {
                    string BC1 = Convert.ToString(service["BenefitCategory1"]) ?? "";
                    string BC2 = Convert.ToString(service["BenefitCategory2"]) ?? "";
                    string BC3 = Convert.ToString(service["BenefitCategory3"]) ?? "";
                    if (!string.IsNullOrEmpty(BC1) && !string.IsNullOrEmpty(BC2))
                    {
                        if (BC1 == "Male Birth Control (Outpatient)" && BC2 == "Elective Sterilization" && BC3 == "")
                        {
                            service["BenefitCategory3"] = "Vasectomy";
                        }
                    }

                }
            }
        }

        #endregion


        #region EQN_1502 :Request # 15
        public static void Update_AdminData()
        {
            Result.Append("\n\n-------------------------------Products----------------------------");
            List<int> Products = new List<int>();
            string allProducts = @"select ins.FormInstanceID from ui.formdesign dess
                                 inner join fldr.FormInstance ins on dess.FormID=ins.FormDesignID
                                 inner join fldr.FormInstanceDataMap insdata on insdata.FormInstanceID=ins.FormInstanceID
                                 where FormName='Admin' and dess.IsActive=1 and ins.IsActive=1";


            string currentProduct = "select FormInstanceID,formdata from  fldr.forminstancedatamap where forminstanceid={0}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = new SqlCommand(allProducts, conn))
                {
                    conn.Open();
                    using (SqlDataReader rd = comm.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            Products.Add((int)rd[0]);
                        }
                    }
                }
            }

            foreach (var product in Products)
            {
                Result.Append("\n");
                GetJsonFromDB(string.Format(currentProduct, product), Product);
                JObject RootData = JObject.Parse(Json);
                string JSON_String = RootData.ToString(Newtonsoft.Json.Formatting.Indented);
                Update_AdminPlanDetailsCalenders(RootData);
                Update_ClaimInterventionIncludeFee(RootData);
                string updatedData = "'" + RootData.ToString(Newtonsoft.Json.Formatting.None).Replace("'", "''") + "'";
                updatedData = "update  fldr.forminstancedatamap set formdata=" + updatedData + " where forminstanceid= " + ProductFormInstanceID;
                UpdateJsonInDB(updatedData);
                string finsihed = string.Empty;
            }
        }
        public static void Update_AdminPlanDetailsCalenders(JObject RootData)
        {
            var gidetails = RootData.SelectToken(PlanDocumentsDetails);
            if (gidetails != null && gidetails.Any())
            {

                string begindate = Convert.ToString(gidetails["PlanYearMondayDayreferenceforSPDBeginningDate"]) ?? "";
                string enddate = Convert.ToString(gidetails["PlanYearMondayDayreferenceforSPDEndDate"]) ?? "";
                if (!string.IsNullOrEmpty(begindate))
                {

                    gidetails["PlanYearMonthandDayreferenceforSPDBeginDate"] = begindate;


                }
                if (!string.IsNullOrEmpty(enddate))
                {
                    gidetails["PlanYearMonthandDayreferenceforSPDEndDate"] = enddate;

                }


            }
        }
        //EQN-1502 Request #12
        public static void Update_ClaimInterventionIncludeFee(JObject RootData)
        {
            var gidetails = RootData.SelectToken(ClaimInterventionDetails);
            if (gidetails != null && gidetails.Any())
            {

                //string includefee = Convert.ToString(gidetails["IncludeFee"]) ?? "";
                foreach (JToken service in gidetails)
                {
                    string serviceDetail = Convert.ToString(service["ServiceDetail"]) ?? "";
                    if (!string.IsNullOrEmpty(serviceDetail))
                    {
                        if (serviceDetail == "Bill Audits")
                        {
                            service["IncludeFee"] = "Yes";
                        }


                    }
                }


            }
        }
        public static void Update_MedicalCOBRetireCoverage(JObject RootData)
        {
            var gidetails = RootData.SelectToken(cobretirecoverage);
            if (gidetails != null && gidetails.Any())
            {

                string retirecoverage = Convert.ToString(gidetails["WhenaretiredpersonorcovereddependentofaretireeiseligibleforMedicaredoe"]) ?? "";

                if (!string.IsNullOrEmpty(retirecoverage))
                {
                    if (retirecoverage.ToLower() == "true")
                    {
                        gidetails["WhenaretiredpersonorcovereddependentofaretireeiseligibleforMedicaredoe"] = "Yes";

                    }
                    else if (retirecoverage.ToLower() == "false")
                    {
                        gidetails["WhenaretiredpersonorcovereddependentofaretireeiseligibleforMedicaredoe"] = "No";

                    }

                }
                else
                {
                    gidetails["WhenaretiredpersonorcovereddependentofaretireeiseligibleforMedicaredoe"] = "Not Applicable";

                }

            }
        }
        public static void Update_InjuryMalformation(JObject RootData)
        {
            var gidetails = RootData.SelectToken(injurymalformationpath);
            if (gidetails != null && gidetails.Any())
            {

                string injurymalformation = Convert.ToString(gidetails["InjuryMalformation"]) ?? "";

                if (!string.IsNullOrEmpty(injurymalformation))
                {
                    gidetails["MalformationBirthDefect"] = injurymalformation;


                }


            }
        }
        // New Request #113
        public static void Update_precertificationddvalues(JObject RootData)
        {
            var gidetails = RootData.SelectToken(precertificationddvalues);
            if (gidetails != null && gidetails.Any())
            {

                string ddvalues = Convert.ToString(gidetails["SelecthowInpatientHospitalPreCertificationPenaltyisapplied"]) ?? "";

                if (!string.IsNullOrEmpty(ddvalues))
                {
                    if (ddvalues == "All Related Charges (includes all ancillary services)" || ddvalues == "All related charges (includes all ancillary services)")
                    {
                        gidetails["SelecthowInpatientHospitalPreCertificationPenaltyisapplied"] = "All Related Charges denied (includes all ancillary services)";
                    }
                    else if (ddvalues == "Facility Charges Only")
                    {
                        gidetails["SelecthowInpatientHospitalPreCertificationPenaltyisapplied"] = "Facility Charges only denied";
                    }
                    else if (ddvalues == "Hospital Ancillary charges only")
                    {
                        gidetails["SelecthowInpatientHospitalPreCertificationPenaltyisapplied"] = "Hospital Ancillary charges only denied";
                    }

                }

            }
        }
        #endregion
    }
}



// RootData["AdminVendor"]["VendorOptionList"] = JArray.Parse(@"[
//  {
//    ""VendorOption"": ""Sentinel Air Medical Alliance""
//  },
//  {
//    ""VendorOption"": ""Amplifon""
//  },
//  {
//    ""VendorOption"": ""MDLive""
//  },
//  {
//    ""VendorOption"": ""RxEdo""
//  },
//  {
//    ""VendorOption"": ""Guardian (fully insured)""
//  },
//  {
//    ""VendorOption"": ""Cigna (fully insured)""
//  },
//  {
//    ""VendorOption"": ""Aetna Dental (fully insured)""
//  },
//  {
//    ""VendorOption"": ""EcRx""
//  },
//  {
//    ""VendorOption"": ""Cerner ""
//  },
//  {
//    ""VendorOption"": ""BravoHealth""
//  },
//  {
//    ""VendorOption"": ""AHH""
//  },
//  {
//    ""VendorOption"": ""MCM""
//  },
//  {
//    ""VendorOption"": ""Cigna""
//  },
//  {
//    ""VendorOption"": ""Optum""
//  },
//  {
//    ""VendorOption"": ""Renalogic""
//  },
//  {
//    ""VendorOption"": ""LabCard (Quest)""
//  },
//  {
//    ""VendorOption"": ""One Call""
//  },
//  {
//    ""VendorOption"": ""US Imaging""
//  },
//  {
//    ""VendorOption"": ""AIG""
//  },
//  {
//    ""VendorOption"": ""HCC""
//  },
//  {
//    ""VendorOption"": ""Interlink""
//  },
//  {
//    ""VendorOption"": ""Hines""
//  },
//  {
//    ""VendorOption"": ""HealthLink""
//  },
//  {
//    ""VendorOption"": ""Inetico""
//  },
//  {
//    ""VendorOption"": ""SHARP""
//  },
//  {
//    ""VendorOption"": ""MAP""
//  },
//  {
//    ""VendorOption"": ""Compass""
//  },
//  {
//    ""VendorOption"": ""Aetna Dental""
//  },
//  {
//    ""VendorOption"": ""Guardian""
//  },
//  {
//    ""VendorOption"": ""Cigna Dental""
//  },
//  {
//    ""VendorOption"": ""NPPN""
//  },
//  {
//    ""VendorOption"": ""First Dental Health""
//  },
//  {
//    ""VendorOption"": ""Diversified Dental Network""
//  },
//  {
//    ""VendorOption"": ""Livongo""
//  },
//  {
//    ""VendorOption"": ""Omada Health""
//  },
//  {
//    ""VendorOption"": ""Cigna EAP""
//  },
//  {
//    ""VendorOption"": ""bswift""
//  },
//  {
//    ""VendorOption"": ""AccessHR""
//  },
//  {
//    ""VendorOption"": ""Healthwatch""
//  },
//  {
//    ""VendorOption"": ""Worlddoc""
//  },
//  {
//    ""VendorOption"": ""IMWell""
//  },
//  {
//    ""VendorOption"": ""Precision Diagnostic Labs""
//  },
//  {
//    ""VendorOption"": ""CVS/Caremark""
//  },
//  {
//    ""VendorOption"": ""Precisions Diagnostics""
//  },
//  {
//    ""VendorOption"": ""WiserTogether""
//  },
//  {
//    ""VendorOption"": ""HealthcareBlueBook""
//  },
//  {
//    ""VendorOption"": ""HealthSParq""
//  },
//  {
//    ""VendorOption"": ""HealthWatch (including WorldDoc)""
//  },
//  {
//    ""VendorOption"": ""WorldDoc Only (not HealthWatch)""
//  },
//  {
//    ""VendorOption"": ""Aetna ASA""
//  },
//  {
//    ""VendorOption"": ""LNL""
//  },
//  {
//    ""VendorOption"": ""Doctors on Demand""
//  },
//  {
//    ""VendorOption"": ""Teladoc""
//  },
//  {
//    ""VendorOption"": ""MyIdealDoc""
//  },
//  {
//    ""VendorOption"": ""ActiveHealth""
//  },
//  {
//    ""VendorOption"": ""Community Care Network  (fully insured)""
//  },
//  {
//    ""VendorOption"": ""VSP (fully insured)""
//  },
//  {
//    ""VendorOption"": ""Community Eye Care Network (self-funded)""
//  },
//  {
//    ""VendorOption"": ""VSP (self funded)""
//  },
//  {
//    ""VendorOption"": ""Optum Rx""
//  },
//  {
//    ""VendorOption"": ""Procare""
//  },
//  {
//    ""VendorOption"": ""Script Care""
//  },
//  {
//    ""VendorOption"": ""Phoenix""
//  },
//  {
//    ""VendorOption"": ""Express Scripts""
//  },
//  {
//    ""VendorOption"": ""LDI""
//  },
//  {
//    ""VendorOption"": ""Medco""
//  },
//  {
//    ""VendorOption"": ""Global Excel Management""
//  },
//  {
//    ""VendorOption"": ""Envision Rx""
//  },
//  {
//    ""VendorOption"": ""CVS/Caremark via RxBenefits""
//  },
//  {
//    ""VendorOption"": ""Magellan Rx""
//  },
//  {
//    ""VendorOption"": ""HealthCare Strategies""
//  },
//  {
//    ""VendorOption"": ""ERS""
//  },
//  {
//    ""VendorOption"": ""ERS EAP""
//  }
//]");



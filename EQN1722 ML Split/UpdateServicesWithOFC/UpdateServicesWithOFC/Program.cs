using System;
using System.IO;
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
    class Program
    {
        public static string conn = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
        public static int MasterListFormInstanceID;

        public static string GetJsonFromDB(string command, string Is_MasterList_OR_Product)
        {
            using (SqlConnection con = new SqlConnection(conn))
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
                            MasterListFormInstanceID = (int)(rd[0]);
                            return rd[1].ToString();
                    }
                }
                rd.Close();
                comm.Cancel();
                con.Close();
            }
            return "";
        }

        public static int GetFormInstanceIDFromFormNameDB(string FormName)
        {
            using (SqlConnection con = new SqlConnection(conn))
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

        public static void UpdateJsonInDB(string command)
        {
            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                SqlCommand comm = new SqlCommand(command, con);
                comm.ExecuteNonQuery();

                comm.Cancel();
                con.Close();
            }
        }

        static void Main(string[] args)
        {
            string Json = string.Empty;

            //Json = GetJsonFromDB("select FormInstanceID,formdata from Fldr.FormInstanceDataMap where FormInstanceID = 59658", UpdateServices.MasterList); //QA MasterList_HSB FormInstanceID = 59658
            Json = GetJsonFromDB("select FormInstanceID,formdata from Fldr.FormInstanceDataMap where FormInstanceID = 53998", UpdateServices.MasterList); //CBC

            string JSONString = Json;
            JObject JSON = JObject.Parse(JSONString);
            
            var sections = JSON.Children();
            foreach (var section in sections)
            {
                if (((JProperty)section).Name.Contains("ProductCategoryLOB"))
                { 
                }
                if (((JProperty)section).Name != "Copay" && ((JProperty)section).Name != "Deductible" && ((JProperty)section).Name != "Coinsurance")
                //if (((JProperty)section).Name.Contains("Limits")) 
                {
                
                    var FormNameQuery = @"select top 1 forminstanceid 
                 from fldr.forminstance ins 
                 inner join fldr.folderversion fldver on fldver.folderversionid =ins.folderversionid 
                 inner join fldr.folder fld on fld.folderid=fldver.folderid where replace(fld.Name,' ','') ='" + ((JProperty)section).Name + "'";  // GetFormInstanceIDFromFormNameDB

                    var data = "{" + section.ToString(Newtonsoft.Json.Formatting.None) + "}";

                    int FormInstanceID = GetFormInstanceIDFromFormNameDB(FormNameQuery);

                    string UpdateQuery = string.Format("update fldr.forminstancedatamap set formdata = {0} where forminstanceid= {1}", "'" + data.Replace("'", "''") + "'", FormInstanceID);

                    UpdateJsonInDB(UpdateQuery);
                }
            }
        }
    }
}







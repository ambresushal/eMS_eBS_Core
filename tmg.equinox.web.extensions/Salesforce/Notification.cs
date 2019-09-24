using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.web.Salesforce
{
    public class Notification
    {
        public string Id { get; set; }
        public string Folder_Name__c { get; set; }
        public string Group_Name__c { get; set; }
        public string Products__c { get; set; }
        public string Contracting_Entity_s_Legal_Name__c { get; set; }
        public string Group_Effective_Date__c { get; set; }
        public string Group_Renewal_Date__c { get; set; }
        public string Initial_Term_of_Contract__c { get; set; }
        public string Number_of_Covered_Employees__c { get; set; }
        public string Tax_ID__c { get; set; }
        public string HAXS_Group_ID__c { get; set; }
        public string Select_Group_Entity__c { get; set; }
        public string Does_the_employer_have_50_or_more_employ__c { get; set; }
        public string Are_there_affiliated_subsidiaries_or_aff__c { get; set; }
        public string BRC_Additional_Information__c { get; set; }

        public static Notification GetNotification(System.Xml.Linq.XElement document)
        {
            Notification objNotification = new Notification();

            List<System.Xml.Linq.XElement> fields = document.Descendants().ToList();
            foreach (var item in fields)
            {
                switch (item.Name.LocalName)
                {
                    case "Group_Name__c": objNotification.Group_Name__c = item.Value; break;
                    case "Products__c": objNotification.Products__c = item.Value; break;
                    case "Contracting_Entity_s_Legal_Name__c": objNotification.Contracting_Entity_s_Legal_Name__c = item.Value; break;
                    case "Group_Effective_Date__c": objNotification.Group_Effective_Date__c = item.Value; break;
                    case "Group_Renewal_Date__c": objNotification.Group_Renewal_Date__c = item.Value; break;
                    case "Initial_Term_of_Contract__c": objNotification.Initial_Term_of_Contract__c = item.Value; break;
                    case "Number_of_Covered_Employees__c": objNotification.Number_of_Covered_Employees__c = item.Value; break;
                    case "Tax_ID__c": objNotification.Tax_ID__c = item.Value; break;
                    case "HAXS_Group_ID__c": objNotification.HAXS_Group_ID__c = item.Value; break;
                    case "Select_Group_Entity__c": objNotification.Select_Group_Entity__c = item.Value; break;
                    case "Does_the_employer_have_50_or_more_employ__c": objNotification.Does_the_employer_have_50_or_more_employ__c = item.Value; break;
                    case "Are_there_affiliated_subsidiaries_or_aff__c": objNotification.Are_there_affiliated_subsidiaries_or_aff__c = item.Value; break;
                    case "BRC_Additional_Information__c": objNotification.BRC_Additional_Information__c = item.Value; break;
                    case "Id": objNotification.Id = item.Value; break;
                    case "Folder_Name__c": objNotification.Folder_Name__c = item.Value; break;
                }
            }

            return objNotification;
        }
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using tmg.equinox.document.rulebuilder.evaluator;
using tmg.equinox.document.rulebuilder.executor;
using tmg.equinox.document.rulebuilder.globalUtility;


namespace tmg.equinox.document.rulebuilder.UnitTesting
{

    public class SourceJSON
    {

        public List<JToken> AdditionalServiceDetailsJSONToken()
        {
            string additionalServicesJSON = @"
                 {'AdditionalServicesDetails' :
                    [
                       {
                         'SESEID':'*RG',
                         'BenefitCategory1' : 'INPATIENT HOSPITAL ROOM AND BOARD',
                         'BenefitCategory3': 'PROFESSIONAL',
                         'PlaceofService': 'IP',
                         'AllowedAmount' : '200',
                         'SpecialCopay' : '30',
                         'PCPCopay' :'40',
                         'DisallowedMessage' :'This is *RG Demo Text Message' 
                       },
            
                          {
                        'SESEID':'9PR',
                        'BenefitCategory1' : 'SURGERY',
                        'BenefitCategory3': 'PROFESSIONAL',
                        'PlaceofService': 'OP',
                        'AllowedAmount' : '900',
                         'SpecialCopay' : '30',
                         'PCPCopay' :'35',
                         'DisallowedMessage' :'This is 9PR Demo Text Message' 
                       },

                          {
                         'SESEID':'110',
                         'BenefitCategory1' :'SURGERY',
                         'BenefitCategory3': 'PROFESSIONAL',
                         'PlaceofService': 'IP',
                         'AllowedAmount' : '200',
                         'SpecialCopay' : '30',
                         'PCPCopay' :'40',
                         'DisallowedMessage' :'This is 110 Demo Text Message' 
                       }
                    ]}";

            JToken jsonObjectForAddServices = JObject.Parse(additionalServicesJSON);
            List<JToken> result = jsonObjectForAddServices["AdditionalServicesDetails"].ToList();

            //string propName = "IQMedicalPlanNetwork";
            //JToken token = JToken.Parse("['CostShareTier','Copay','IndividualDeductible','FamilyDeductible','Other1Deductible','Other2Deductible','EmployeePlusOneChildDeductible','FamilyDeductible','Coinsurance','IndividualOOPM','FamilyOOPM','Other1OOPM','Other2OOPM','EmployeePlusOneChildOOPM']");
            //JArray jArray = new JArray();
            //jArray.Add(token);
            //JProperty jproperty = new JProperty(propName, jArray);
            //result.ForEach(x => x.Last().AddAfterSelf(jproperty));


            return result;
        }

        public List<JToken> ServiceGroupDetailsJSONToken()
        {

            string serviceDetailsJSON = @"
                    {'ServiceGroupDetails' :
                      [{
                         'SESEID':'*RG',
                         'BenefitCategory1' : 'INPATIENT HOSPITAL ROOM AND BOARD',
                         'BenefitCategory3': 'PROFESSIONAL',
                         'PlaceofService': 'IP',
                         'Covered' : 'Yes',
                         'Coinsurance' : '100.00',
                         'Copay' :'20',
                         'Deductible' :'30' 
                       },
            
                          {
                        'SESEID':'9PR',
                        'BenefitCategory1' : 'SURGERY',
                        'BenefitCategory3': 'PROFESSIONAL',
                        'PlaceofService': 'OP',
                        'Covered' : 'No',
                        'Coinsurance' : '80.00',
                        'Copay' :'10',
                        'Deductible' :'40'
                       },

                          {
                         'SESEID':'110',
                         'BenefitCategory1' :'SURGERY',
                         'BenefitCategory3': 'PROFESSIONAL',
                         'PlaceofService': 'IP',
                         'Covered' : 'No',
                         'Coinsurance' : '50.00',
                         'Copay' :'15',
                         'Deductible' :'85' 
                       }]
                     }";



            //JObject jObject = JObject.Parse(serviceDetailsJSON);
            //List<string> columns = new List<string>();
            //columns.Add("BenefitCategory1");
            //columns.Add("BenefitCategory2");
            //columns.Add("BenefitCategory3");

            //List<JToken> services =((JArray)jObject["ServiceGroupDetails"]).ToList();
            //List<JToken> selectedColumnDetails = RuleEngineGlobalUtility.GetAdHocColumnDetails(services, columns);

            JToken jsonObjectForserviceDetails = JObject.Parse(serviceDetailsJSON);
            return jsonObjectForserviceDetails["ServiceGroupDetails"].ToList();
        }
    }


    public class ArithmaticOperationTesting
    {

        SourceJSON jsonOutput = new SourceJSON();

        //
        public string mergeJSONTesting()
        {
            string json = "";
            Dictionary<string, string> keys = new Dictionary<string, string>();
            keys.Add("SESEID", "SESEID");
            keys.Add("BenefitCategory1", "BenefitCategory1");
            keys.Add("BenefitCategory3", "BenefitCategory3");
            keys.Add("PlaceofService", "PlaceofService");


            List<JToken> ServiceDetails = jsonOutput.ServiceGroupDetailsJSONToken();
            List<JToken> AdditionalServices = jsonOutput.AdditionalServiceDetailsJSONToken();

            CollectionExecutionComparer operatorProcessor = new CollectionExecutionComparer(AdditionalServices, ServiceDetails, keys);
            CollectionExecutionComparer operatorProcessor1 = new CollectionExecutionComparer(ServiceDetails, AdditionalServices, keys);

            List<JToken> resultforAddServiceIntersect = operatorProcessor.Intersection();
            List<JToken> resultforServiceGroupIntersect = operatorProcessor1.Intersection();

            JArray Obj1 = new JArray();
            Obj1.Add(resultforAddServiceIntersect);

            JArray Obj2 = new JArray();
            Obj2.Add(resultforServiceGroupIntersect);

            var mergeSettingsMerge = new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Merge

            };

            //var mergeSettingsUnion = new JsonMergeSettings
            //{
            //    MergeArrayHandling = MergeArrayHandling.Union           
            //};

            Obj1.Merge(Obj2, mergeSettingsMerge);            

            json = Obj1.ToString();

            return json;
        }
    }
}

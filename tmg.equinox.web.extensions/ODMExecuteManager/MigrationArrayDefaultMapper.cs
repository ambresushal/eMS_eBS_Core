using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tmg.equinox.domain.entities;
using tmg.equinox.web.ODMExecuteManager.Interface;
using tmg.equinox.web.ODMExecuteManager.Model;

namespace tmg.equinox.web.ODMExecuteManager
{
    public class MigrationArrayDefaultMapper : IMigrationArrayMapper
    {
        public void MapArray(ref JObject source, ref JObject target, List<MigrationFieldItem> items)
        {
            string targetParentPath = items[0].DocumentPath.Split('[')[0];

            if (!String.IsNullOrEmpty(targetParentPath))
            {
                //get source token 
                string sourcePath = items[0].TableName;
                var sourceToken = source.SelectToken(sourcePath);
                //get target array
                var targetToken = target.SelectToken(targetParentPath);
                if (targetToken == null)
                {
                    if (targetParentPath == "PreICL.DailyCopaymentAmounts")
                    {
                        var t = target.SelectToken("PreICL");
                        string s = t.ToString();
                        s = s.Replace("GCDailyCopaymentAmounts", "DailyCopaymentAmounts");
                        t = JToken.Parse(s);
                        target.SelectToken("PreICL").Replace(t);
                        targetToken = target.SelectToken(targetParentPath);
                    }
                    if (targetParentPath == "GapCoverage.GCDailyCopaymentAmounts")
                    {
                        var t = target.SelectToken("GapCoverage");
                        string s = t.ToString();
                        s = s.Replace("DailyCopaymentAmounts", "GCDailyCopaymentAmounts");
                        t = JToken.Parse(s);
                        target.SelectToken("GapCoverage").Replace(t);
                        targetToken = target.SelectToken(targetParentPath);
                    }


                }
                //for each source row, clear target rows and add new rows
                if (sourceToken.Type == JTokenType.Array && targetToken.Type == JTokenType.Array)
                {
                    int _count = targetToken.Children().Count();
                    for (int i = 0; i < _count - 1; i++)
                        if (targetToken.Children().Count() > 0)
                            targetToken[0].Remove();

                    if (targetToken.Children().Count() > 0)
                        foreach (JProperty jp in targetToken[0].Children())
                            jp.Value = "";

                    JArray sourceRows = (JArray)sourceToken;
                    int idx = 0;
                    while (idx < sourceRows.Count - 1)
                    {
                        if (targetToken.Children().Count() > 0)
                        {
                            JObject clone = (JObject)targetToken[0].DeepClone();
                            ((JArray)targetToken).Add(clone);
                        }
                        idx++;
                    }
                    MigrationItemMapperFactory mapperFactory = new MigrationItemMapperFactory();
                    for (idx = 0; idx < sourceRows.Count; idx++)
                    {
                        foreach (var item in items)
                        {
                            MigrationFieldItem itemClone = item.Clone();
                            string sourceField = itemClone.ColumnName;
                            string targetField = itemClone.DocumentPath.Split('.').Last();
                            if (targetToken.Children().Count() > 0)
                            {
                                if (sourceRows[idx][sourceField] != null && targetToken[idx][targetField] != null)
                                {
                                    itemClone.TableName = itemClone.TableName + "[" + idx + "]";
                                    itemClone.DocumentPath = itemClone.DocumentPath.Replace("[0]", "[" + idx + "]");
                                    IMigrationItemMapper itemMapper = mapperFactory.GetMapper(itemClone.MappingType);
                                    itemMapper.MapItem(ref source, ref target, itemClone);
                                }
                            }
                        }
                    }
                }
                else if (sourceToken != null)
                {
                    if (sourceToken.Type == JTokenType.Object && targetToken.Type == JTokenType.Object
                       && source["pbp_d_opt_identifier"] != null && source["pbp_d_opt_identifier"].ToString() == string.Empty && items[0].DocumentPath.StartsWith("OptionalSupplementalBenefitPackages."))
                    {
                        JArray jarr = new JArray();
                        jarr.Add((JToken)sourceToken);
                        sourceToken = jarr;
                    }

                    if ((sourceToken.Type == JTokenType.Array && targetToken.Type == JTokenType.Object))
                    {
                        JArray sourceRows = (JArray)sourceToken;
                        int idx = 0;

                        List<JObject> sourceObject = sourceRows.ToObject<List<JObject>>();
                        MigrationItemMapperFactory mapperFactory = new MigrationItemMapperFactory();

                        for (idx = 0; idx < sourceObject.Count; idx++)
                        {
                            foreach (var item in items)
                            {
                                MigrationFieldItem itemClone = item.Clone();
                                string sourceField = itemClone.ColumnName;
                                string targetField = itemClone.DocumentPath.Split('.').Last();
                                if (targetToken.Children().Count() > 0)
                                {
                                    if (sourceRows[idx][sourceField] != null && targetToken[targetField] != null)
                                    {
                                        //modify item source and target paths to use the Field Mappers
                                        //source - change table name to add index of array
                                        itemClone.TableName = itemClone.TableName;
                                        //target - change document path
                                        itemClone.DocumentPath = itemClone.DocumentPath.Replace("[0]", "");
                                        IMigrationItemMapper itemMapper = mapperFactory.GetMapper(itemClone.MappingType);

                                        string parentSectionID = itemClone.DocumentPath.Split('.').First();

                                        if (parentSectionID == "OptionalSupplementalBenefitPackages")
                                        {
                                            parentSectionID = itemClone.DocumentPath.Split('.').Skip(1).First();

                                            var targetToken1 = target.SelectToken("OptionalSupplementalBenefitPackages.OptionalSupplementalPackages");
                                            var prop = targetToken1.Parent as JProperty;
                                            prop.Value = sourceObject.Count;
                                        }

                                        string lastCharacter = parentSectionID[parentSectionID.Length - 1].ToString();
                                        int lastDigit = Convert.ToInt32(lastCharacter);

                                        bool optOonSectionUpdate = false;

                                        if (item.TableName.ToString() == "PBPD_OON")
                                        {
                                            string optOonIdentifier = JObject.Parse(sourceObject[idx].ToString())["pbp_d_opt_oon_identifier"].ToString();
                                            string optOonCatId = JObject.Parse(sourceObject[idx].ToString())["pbp_d_opt_oon_cat_id"].ToString();
                                            string optOonTypeId = JObject.Parse(sourceObject[idx].ToString())["pbp_d_opt_oon_type_id"].ToString();
                                            string sectionNumber = Regex.Match(parentSectionID, @"\d+").Value;
                                            string ooNCatNumber = Regex.Match(optOonCatId, @"\d+").Value;

                                            if (lastDigit == Convert.ToInt32(optOonIdentifier) && parentSectionID.Contains(optOonCatId) && sectionNumber == ooNCatNumber && ((parentSectionID.Contains("OONOptional") && optOonTypeId == "1") || (parentSectionID.Contains("OONStepup") && optOonTypeId == "2")))
                                            {
                                                optOonSectionUpdate = true;
                                            }
                                        }

                                        if (GlobalVariables.OptionalSupplementalPackageTables.Contains(item.TableName.ToString()))
                                        {
                                            if (idx == lastDigit - 1)
                                            {
                                                itemClone.TableName = item.TableName + "[" + idx + "]";
                                                itemMapper.MapItem(ref source, ref target, itemClone);
                                            }
                                        }
                                        else if (item.TableName.ToString() == "PBPD_OON")
                                        {
                                            if (optOonSectionUpdate)
                                            {
                                                itemClone.TableName = item.TableName + "[" + idx + "]";
                                                itemMapper.MapItem(ref source, ref target, itemClone);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var item in items)
                    {
                        if (targetToken.Children().Count() > 0)
                        {
                            if (targetToken[0] != null)
                            {
                                if (item.MappingType == "MULTIVALUECSV")
                                {
                                    var token = targetToken[0][item.DocumentPath.Split('.').Last()];
                                    var prop = token.Parent as JProperty;
                                    JArray arr = new JArray();
                                    arr.Add("");
                                    prop.Value = arr;
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}

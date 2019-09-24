using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.pbp
{
    public class MigrationArrayDefaultMapper : IMigrationArrayMapper
    {
        public void MapArray(ref JObject source, ref JObject target, List<MigrationFieldItem> items,  MigrationPlanItem plan)
        {
            string targetParentPath = items[0].DocumentPath.Split('[')[0];

            if (!String.IsNullOrEmpty(targetParentPath))
            {
                //get source token 
                string sourcePath = items[0].TableName;
                var sourceToken = source.SelectToken(sourcePath);
                //get target array
                var targetToken = target.SelectToken(targetParentPath);
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
                                    itemMapper.MapItem(ref source, ref target, itemClone,plan);
                                }
                            }
                        }
                    }
                }
                else if (sourceToken != null)
                {
                    if (sourceToken.Type == JTokenType.Array && targetToken.Type == JTokenType.Object)
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

                                            if (lastDigit == Convert.ToInt32(optOonIdentifier) && parentSectionID.Contains(optOonCatId))
                                            {
                                                optOonSectionUpdate = true;
                                            }
                                        }

                                        if (item.TableName.ToString() == "STEP16B" || item.TableName.ToString() == "STEP16A" || item.TableName.ToString() == "PBPD_OPT")
                                        {
                                            if (idx ==lastDigit - 1  )
                                            {
                                                itemClone.TableName = item.TableName + "[" + idx + "]";
                                                itemMapper.MapItem(ref source, ref target, itemClone, plan);
                                            }
                                        }
                                        else if(item.TableName.ToString() == "PBPD_OON")
                                        {
                                            if (optOonSectionUpdate)
                                            {
                                                itemClone.TableName = item.TableName + "[" + idx + "]";
                                                itemMapper.MapItem(ref source, ref target, itemClone, plan);
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

                                    string logMessage = item.TableName + "," + item.ColumnName + "," + item.DocumentPath + "," + prop.Value.ToString().Replace(",",";");
                                    logMessage = Regex.Replace(logMessage, @"\t|\n|\r", "");
                                    DMUtilityLog.WriteDMUtilityLog(plan.QID, item.ViewType, logMessage);
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}

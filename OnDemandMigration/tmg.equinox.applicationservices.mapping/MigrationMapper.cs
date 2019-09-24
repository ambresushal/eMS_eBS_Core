using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.pbp
{
    public class MigrationMapper
    {
        private List<MigrationFieldItem> _mapItems;

        public MigrationMapper(List<MigrationFieldItem> mapItems)
        {
            _mapItems = mapItems;
        }

        public List<PBPTable> GetPBPTables()
        {
            List<PBPTable> tables = new List<PBPTable>();
            var groups = _mapItems.GroupBy(a => a.TableName);
            foreach (var group in groups)
            {
                PBPTable table = new PBPTable();
                table.TableName = group.Key;
                table.ColumnNames = new List<string>();
                foreach (var item in group)
                {
                    table.IsArray = item.IsArrayElement;
                    table.ColumnNames.Add(item.ColumnName);
                }
                tables.Add(table);
            }
            return tables;
        }

        public void ProcessMappings(JObject source, JObject target, MigrationPlanItem plan)
        {
            ProcessNonArrayItems(source, target,plan);
            ProcessArrayItems(source, target,plan);
        }

        private void ProcessNonArrayItems(JObject source, JObject target, MigrationPlanItem plan)
        {
            var items = from mapItem in _mapItems
                        where mapItem.DocumentPath != "TBD" && mapItem.IsArrayElement == false
                        select mapItem;
            if (items != null && items.Count() > 0)
            {
                List<MigrationFieldItem> nonArrayItems = new List<MigrationFieldItem>();
                nonArrayItems = items.ToList();
                var groups = nonArrayItems.GroupBy(b => b.TableName);
                MigrationItemMapperFactory mapperFactory = new MigrationItemMapperFactory();
                IMigrationItemMapper itemMapper;
                foreach (var group in groups)
                {
                    try
                    {
                        foreach (var item in group)
                        {
                            try
                            {
                                //this if is temporary
                                itemMapper = mapperFactory.GetMapper(item.MappingType);
                                itemMapper.MapItem(ref source, ref target, item,plan);
                            }
                            catch (Exception ex)
                            {
                                Console.Write("got it");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write("got it");
                    }
                }
            }
        }

        private void ProcessArrayItems(JObject source, JObject target, MigrationPlanItem plan)
        {
            var items = from mapItem in _mapItems
                        where mapItem.DocumentPath != "TBD" && mapItem.IsArrayElement == true
                        && mapItem.DocumentPath.Contains('[')
                        select mapItem;
            if (items != null && items.Count() > 0)
            {

                List<MigrationFieldItem> nonArrayItems = new List<MigrationFieldItem>();
                nonArrayItems = items.ToList();
                var groups = nonArrayItems.GroupBy(b => b.TableName);
                MigrationArrayMapperFactory mapperFactory = new MigrationArrayMapperFactory();
                IMigrationArrayMapper arrayMapper;
                JObject tempsource = (JObject)source.DeepClone();
                foreach (var grp in groups)
                {
                    var arrayGroups = from grpItem in grp.ToList()
                                      group grpItem by grpItem.DocumentPath.Split('[')[0]
                                      into g
                                      select g;
                    foreach (var subGrp in arrayGroups)
                    {

                        source = (JObject)tempsource.DeepClone();
                        var sourceToken = source.SelectToken(subGrp.ToList()[0].TableName);
                        if (sourceToken.Type == JTokenType.Array)
                        {
                            JArray sourceRows = (JArray)sourceToken;
                            int _counter = sourceRows.Count;
                            if (subGrp.Key.Contains("PreICL."))
                            {
                            STARTAGAIN:
                                foreach (JToken jt in sourceRows)
                                {
                                    if (jt["mrx_tier_type_id"].ToString() == "3")
                                    {
                                        jt.Remove();
                                        goto STARTAGAIN;
                                    }
                                }
                                arrayMapper = mapperFactory.GetMapper("");
                                arrayMapper.MapArray(ref source, ref target, subGrp.ToList(),plan);
                            }
                            else if (subGrp.Key.Contains("GapCoverage."))
                            {
                            STARTAGAIN1:
                                foreach (JToken jt in sourceRows)
                                {
                                    if (jt["mrx_tier_type_id"].ToString() == "1")
                                    {
                                        jt.Remove();
                                        goto STARTAGAIN1;
                                    }
                                }
                                arrayMapper = mapperFactory.GetMapper("");
                                arrayMapper.MapArray(ref source, ref target, subGrp.ToList(),plan);
                            }
                            else
                            {
                                arrayMapper = mapperFactory.GetMapper("");
                                arrayMapper.MapArray(ref source, ref target, subGrp.ToList(), plan);
                            }
                        }
                        else
                        {
                            arrayMapper = mapperFactory.GetMapper("");
                            arrayMapper.MapArray(ref source, ref target, subGrp.ToList(), plan);
                        }
                    }
                    source = (JObject)tempsource.DeepClone();
                }

            }
        }
    }
}

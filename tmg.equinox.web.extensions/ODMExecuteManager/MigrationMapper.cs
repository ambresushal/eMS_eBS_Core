using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.web.ODMExecuteManager.Interface;
using tmg.equinox.web.ODMExecuteManager.Model;

namespace tmg.equinox.web.ODMExecuteManager
{
    public class MigrationMapper
    {
        private List<MigrationFieldItem> _mapItems;
        private static readonly ILog _logger = LogProvider.For<OnDemandMigrationExecutionManager>();

        public MigrationMapper(List<MigrationFieldItem> mapItems)
        {
            _mapItems = mapItems;
        }

        public List<TableInfo> GetPBPTables()
        {
            List<TableInfo> tables = new List<TableInfo>();
            var groups = _mapItems.GroupBy(a => a.TableName);
            foreach (var group in groups)
            {
                TableInfo table = new TableInfo();
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

        public void ProcessMappings(JObject source, JObject target)
        {
            ProcessNonArrayItems(source, target);
            ProcessArrayItems(source, target);
        }
        public void ProcessManualDataUpdates(List<ManualDataUpdates> manualDataUpdates, ref JObject target)
        {
            foreach (ManualDataUpdates mdu in manualDataUpdates)
            {
                var targetToken = target.SelectToken(mdu.DocumentPath);
                var prop = targetToken.Parent as JProperty;
                if (mdu.IsArray == true)
                    prop.Value = JArray.Parse(mdu.DataValue);
                else
                    prop.Value = mdu.DataValue;
            }
        }
        private void ProcessNonArrayItems(JObject source, JObject target)
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
                    foreach (var item in group)
                    {
                        try
                        {
                            itemMapper = mapperFactory.GetMapper(item.MappingType);
                            itemMapper.MapItem(ref source, ref target, item);
                        }
                        catch (Exception ex)
                        {
                            if(item.ViewType == "")
                            {

                            }
                            _logger.Error("[" + item.ViewType + "] Exception occures in migration process. Due to error [" + ex.Message + "] for item [" + item.DocumentPath + "]");
                        }
                    }
                }
            }
        }

        private void ProcessArrayItems(JObject source, JObject target)
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
                        try
                        {
                            //if (subGrp.Key.Contains("OptionalSupplementalBenefitPackages"))
                              //  continue;//WellCare is not using this section

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
                                    arrayMapper.MapArray(ref source, ref target, subGrp.ToList());
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
                                    arrayMapper.MapArray(ref source, ref target, subGrp.ToList());
                                }
                                else
                                {
                                    arrayMapper = mapperFactory.GetMapper("");
                                    arrayMapper.MapArray(ref source, ref target, subGrp.ToList());
                                }
                            }
                            else
                            {
                                arrayMapper = mapperFactory.GetMapper("");
                                arrayMapper.MapArray(ref source, ref target, subGrp.ToList());
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("[" + subGrp.ToList()[0].ViewType + "] Exception occures in migration process. Due to error [" + ex.Message + "] for sub group key [" + subGrp.Key + "]");
                        }
                    }
                    source = (JObject)tempsource.DeepClone();
                }

            }
        }

    }

}

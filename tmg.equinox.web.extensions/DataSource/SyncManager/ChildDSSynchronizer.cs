using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.Framework.Caching;

namespace tmg.equinox.web.DataSource.SyncManager
{
    public class ChildInlineDSSynchroniser : DataSourceSychronizer
    {

        public ChildInlineDSSynchroniser(JArray sourceData, JArray targetData, DataSourceDesign dataSource, RepeaterDesign targetRepeaterDesign, Dictionary<string, List<JToken>> sourceDataDifference) :
            base(sourceData, targetData, dataSource, targetRepeaterDesign, sourceDataDifference)
        {
        }

        public override void addNewDataSourceRow(JToken sourceRow)
        {
            foreach (JObject rowData in _targetData)
            {

                if (rowData[_dataSource.DataSourceName] == null)
                {
                    bindChildDataSourceObjectToParentRow(rowData);
                    isTargetDataUpdated = true;
                }
                else if (rowData[_dataSource.DataSourceName] != null)
                {
                    addNewChildRowToParent(sourceRow, rowData);
                }
            }
        }

        public override void updateDataSourceRow(JToken sourceRow)
        {
            foreach (JObject rowData in _targetData)
            {
                if (rowData[_dataSource.DataSourceName] == null || ((JArray)rowData[_dataSource.DataSourceName]).Count == 0)
                {
                    bindChildDataSourceObjectToParentRow(rowData);
                    isTargetDataUpdated = true;
                }
                else
                {
                    var targetChildRow = getRepeaterRow(sourceRow, (JArray)rowData[_dataSource.DataSourceName]);
                    if (targetChildRow != null)
                    {
                        var elementsToCompare = _dataSource.Mappings.ToDictionary(s => s.SourceElement, s => s.TargetElement);
                        var rowComparer = new JTokenEqualityComparer(elementsToCompare);
                        if (!rowComparer.Equals(sourceRow, targetChildRow))
                        {
                            updateTargetRepeaterRow(sourceRow, targetChildRow, true);
                            isTargetDataUpdated = true;
                        }
                    }
                    else
                    {
                        addNewChildRowToParent(sourceRow, rowData);
                    }
                }
            }
        }

        public override void removeDataSourceRow(JToken sourceRow)
        {
            for (int i = 0; i < _targetData.Count; i++)
            {
                var dataSourceRows = (JArray)_targetData[i][_dataSource.DataSourceName];
                var targetChildRow = getRepeaterRow(sourceRow, dataSourceRows);
                dataSourceRows.Remove(targetChildRow);
            }
            isTargetDataUpdated = true;
        }

        public override void cleanBlankRows()
        {
            JObject blankRow = new JObject();
            foreach (var map in _dataSource.Mappings)
            {
                blankRow.Add(map.TargetElement, "");
            }
            var keysToComapre = _dataSource.Mappings.Where(m => m.IsKey).ToDictionary(s => s.TargetElement, s => s.TargetElement);
            var targetRowComparer = new JTokenEqualityComparer(keysToComapre);
            foreach (var rowData in _targetData)
            {
                if (rowData[_dataSource.DataSourceName] != null)
                {
                    var childData = ((JArray)rowData[_dataSource.DataSourceName]);
                    if (childData.Count > 1)
                    {
                        var rowToRemove = childData.Where(r => targetRowComparer.Equals(r, blankRow)).FirstOrDefault();
                        if (rowToRemove != null)
                        {
                            childData.Remove(rowToRemove);
                        }
                    }
                }
            }
        }

        private void addNewChildRowToParent(JToken sourceRow, JToken paretRow)
        {
            JObject newRow = new JObject();
            foreach (DataSourceElementMapping ds in _dataSource.Mappings)
            {
                newRow.Add(ds.TargetElement, "");
            }
            newRow = (JObject)updateTargetRepeaterRow(sourceRow, newRow, false);
            var dataSourceData = (JArray)paretRow[_dataSource.DataSourceName];
            dataSourceData.Add(newRow);
            paretRow[_dataSource.DataSourceName] = dataSourceData;

            isTargetDataUpdated = true;
        }

        private void bindChildDataSourceObjectToParentRow(JToken parentRow)
        {
            if (parentRow[_dataSource.DataSourceName] == null)
            {
                parentRow[_dataSource.DataSourceName] = new JArray();
                foreach (JObject rowData in _sourceData)
                {
                    JObject newRow = new JObject();
                    foreach (DataSourceElementMapping ds in _dataSource.Mappings)
                    {
                        newRow.Add(ds.TargetElement, "");
                    }
                    newRow = (JObject)updateTargetRepeaterRow(rowData, newRow, false);
                    ((JArray)parentRow[_dataSource.DataSourceName]).Add(newRow);
                }
            }
        }
    }
}
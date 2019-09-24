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
    public class PrimaryDSSynchroniser : DataSourceSychronizer
    {
        public PrimaryDSSynchroniser(JArray sourceData, JArray targetData, DataSourceDesign dataSource, RepeaterDesign targetRepeaterDesign, Dictionary<string, List<JToken>> sourceDataDifference) :
            base(sourceData, targetData, dataSource, targetRepeaterDesign, sourceDataDifference)
        {
        }

        public override void addNewDataSourceRow(JToken sourceRow)
        {
            JObject newRow = new JObject();
            foreach (ElementDesign element in _targetRepeaterDesign.Elements.Where(e => e.IsPrimary))
            {
                newRow.Add(element.GeneratedName, "");
            }
            _targetData.Add(updateTargetRepeaterRow(sourceRow, newRow, false));
            isTargetDataUpdated = true;
        }
        public override void updateDataSourceRow(JToken sourceRow)
        {
            var targetRow = getRepeaterRow(sourceRow, _targetData);
            if (targetRow != null)
            {
                var elementsToCompare = _dataSource.Mappings.ToDictionary(s => s.SourceElement, s => s.TargetElement);
                var rowComparer = new JTokenEqualityComparer(elementsToCompare);
                if (!rowComparer.Equals(sourceRow, targetRow))
                {
                    updateTargetRepeaterRow(sourceRow, targetRow, true);
                    isTargetDataUpdated = true;
                }
            }
            else
            {
                addNewDataSourceRow(sourceRow);
            }
        }
        public override void removeDataSourceRow(JToken sourceRow)
        {
            var targetRow = getRepeaterRow(sourceRow, _targetData);
            _targetData.Remove(targetRow);
            isTargetDataUpdated = true;
        }
        public override void cleanBlankRows()
        {
            JObject blankRow = new JObject();
            foreach (var element in _targetRepeaterDesign.Elements.Where(e => e.IsPrimary))
            {
                blankRow.Add(element.GeneratedName, "");
            }
            var keysToComapre = _dataSource.Mappings.Where(m => m.IsKey).ToDictionary(s => s.TargetElement, s => s.TargetElement);
            var targetRowComparer = new JTokenEqualityComparer(keysToComapre);
            if (_targetData != null && _targetData.Count > 1)
            {
                var rowToRemove = _targetData.Where(r => targetRowComparer.Equals(r, blankRow)).FirstOrDefault();
                if (rowToRemove != null)
                {
                    _targetData.Remove(rowToRemove);
                }
            }
        }

    }
}
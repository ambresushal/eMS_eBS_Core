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
    public abstract class DataSourceSychronizer
    {
        public DataSourceSychronizer(JArray sourceData, JArray targetData, DataSourceDesign dataSource, RepeaterDesign targetRepeaterDesign, Dictionary<string, List<JToken>> sourceDataDifference)
        {
            this._sourceData = sourceData;
            this._targetData = targetData;
            this._dataSource = dataSource;
            this._sourceDataDifference = sourceDataDifference;
            this._keysToCompare = _dataSource.Mappings.Where(m => m.IsKey).ToDictionary(s => s.SourceElement, s => s.TargetElement);
            this.jTokenEqualityComparer = new JTokenEqualityComparer(_keysToCompare);
            this._targetRepeaterDesign = targetRepeaterDesign;
        }

        protected JArray _sourceData { get; set; }
        protected JArray _targetData { get; set; }
        protected DataSourceDesign _dataSource { get; set; }
        protected RepeaterDesign _targetRepeaterDesign { get; set; }
        protected Dictionary<string, List<JToken>> _sourceDataDifference { get; set; }
        protected Dictionary<string, string> _keysToCompare { get; set; }
        protected JTokenEqualityComparer jTokenEqualityComparer { get; set; }
        public bool isTargetDataUpdated { get; set; }

        public void syncMapDataSource()
        {
            List<JToken> deletedRows = _sourceDataDifference["Delete"];
            foreach (JToken row in deletedRows)
            {
                removeDataSourceRow(row);
            }

            List<JToken> updatedRows = _sourceDataDifference["Update"];
            foreach (JToken row in updatedRows)
            {
                updateDataSourceRow(row);
            }

            List<JToken> addedRows = _sourceDataDifference["Add"];
            foreach (JToken row in addedRows)
            {
                addNewDataSourceRow(row);
            }
            //cleanBlankRows();
        }

        #region abstract methods
        public abstract void addNewDataSourceRow(JToken sourceRow);

        public abstract void updateDataSourceRow(JToken sourceRow);

        public abstract void removeDataSourceRow(JToken sourceRow);

        public abstract void cleanBlankRows();

        #endregion

        #region Helper methods

        public JToken getRepeaterRow(JToken rowToCompare, JArray data)
        {
            JToken row = null;
            if (data != null)
            {
                row = data.Where(o => jTokenEqualityComparer.Equals(rowToCompare, o)).FirstOrDefault();
            }
            return row;
        }

        public JToken updateTargetRepeaterRow(JToken sourceRow, JToken targetRow, bool isForRowUpdate)
        {
            if (sourceRow != null && targetRow != null)
            {
                var mappings = isForRowUpdate ? _dataSource.Mappings.Where(m => m.CopyModeID == 1) : _dataSource.Mappings;

                foreach (var map in mappings)
                {
                    targetRow[map.TargetElement] = sourceRow[map.SourceElement];
                }
            }
            return targetRow;
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;

namespace tmg.equinox.web.DataSource
{
    public class SectionDataSourceMapper
    {
        private ExpandoObject source;
        private ExpandoObject target;
        private DataSourceDesign design;

        public SectionDataSourceMapper(DataSourceDesign design,ExpandoObject source,ExpandoObject target) {
            this.source = source;
            this.target = target;
            this.design = design;
        }

        public void MergeDataSource() 
        {
            //get data from data source and populate the target object
            ExpandoObject sourceParent = GetParentObject(source, design.SourceParent);
            ExpandoObject targetParent = GetParentObject(target, design.TargetParent);
            MoveData(design, sourceParent, targetParent);
        }

        private ExpandoObject GetParentObject(ExpandoObject sourceObject, string fullParentName)
        {
            ExpandoObject parent = new ExpandoObject();
            string[] parentElements = fullParentName.Split('.');
            IDictionary<string, object> vals = sourceObject as IDictionary<string, object>;
            foreach (var parentElement in parentElements)
            {
                if (vals is ExpandoObject)
                {
                    vals = vals[parentElement] as IDictionary<string, object>;
                }
            }
            if (vals is ExpandoObject)
            {
                parent = (ExpandoObject)vals;
            }
            return parent;
        }

        private void MoveData(DataSourceDesign design, ExpandoObject source, ExpandoObject target)
        {
            IDictionary<string, object> sourceVals = source as IDictionary<string, object>;
            IDictionary<string, object> targetVals = target as IDictionary<string, object>;

            foreach (var mapping in design.Mappings)
            {
                if (targetVals.ContainsKey(mapping.TargetElement) && sourceVals.ContainsKey(mapping.SourceElement))
                {
                    if (targetVals[mapping.TargetElement] == null || (targetVals[mapping.TargetElement] is String && String.IsNullOrEmpty(targetVals[mapping.TargetElement] as String)))
                    {
                        targetVals[mapping.TargetElement] = sourceVals[mapping.SourceElement];
                    }
                }
            }
        }
    }
}
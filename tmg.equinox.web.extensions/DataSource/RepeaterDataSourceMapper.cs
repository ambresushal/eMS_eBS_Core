using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;

namespace tmg.equinox.web.DataSource
{
    public class RepeaterDataSourceMapper
    {
        #region Private Members
        private ExpandoObject source;
        private ExpandoObject target;
        private DataSourceDesign design;
        private FormDesignVersionDetail targetDesign;
        enum DataCopyMode
        {
            None,
            Always,
            Once
        };
        #endregion Private Members

        #region Constructor
        public RepeaterDataSourceMapper(DataSourceDesign design,FormDesignVersionDetail targetDesign, ExpandoObject source, ExpandoObject target)
        {
            this.source = source;
            this.target = target;
            this.design = design;
            this.targetDesign = targetDesign;
        }
        #endregion Constructor

        #region Public Methods
        public void MergeDataSource()
        {
            //get data from data source and populate the target object
            IList<Object> sourceParent = GetParentObject(source, design.SourceParent);
            IList<Object> targetParent = GetParentObject(target, design.TargetParent);
            IList<DataSourceElementMapping> filterMaps = GetFilters(design);
            if (design.DisplayMode == "Primary")
            {
                if (design.DataSourceModeType == "Auto" || design.DataSourceModeType == null)
                {
                    MoveData(design, filterMaps, sourceParent, targetParent);
                }
            }
            else
            {
                if (design.DataSourceModeType == "Auto" || design.DataSourceModeType == null)
                {
                    if (design.FormDesignID != 1276 || design.TargetParent != "BenefitReview.BenefitReviewGrid")
                    {
                        MoveChildData(design, filterMaps, sourceParent, targetParent);
                    }
                }

                if (design.DisplayMode == "In Line")
                {
                    GroupHeader(design, sourceParent);
                }
            }
        }
        #endregion Public Methods

        #region Private Methods
        private void MoveChildData(DataSourceDesign design, IList<DataSourceElementMapping> filterMaps, IList<Object> source, IList<Object> target)
        {
            if (target.Count() > 0)
            {
                foreach (var elem in target)
                {
                    if (elem is ExpandoObject)
                    {
                        IDictionary<string, object> vals = elem as IDictionary<string, object>;
                        if (!vals.ContainsKey(design.DataSourceName))
                        {
                            vals.Add(design.DataSourceName, new System.Collections.Generic.List<Object>());
                        }
                        if (vals.ContainsKey(design.DataSourceName))
                        {
                            if (vals[design.DataSourceName] is System.Collections.Generic.IList<Object>)
                            {
                                MoveData(design, filterMaps, source, vals[design.DataSourceName] as System.Collections.Generic.IList<Object>);
                            }
                        }
                    }
                }
            }
        }

        private IList<Object> GetParentObject(ExpandoObject sourceObject, string fullParentName)
        {
            IList<Object> parent = new List<Object>();
            string[] parentElements = fullParentName.Split('.');
            IDictionary<string, object> vals = sourceObject as IDictionary<string, object>;
            foreach (var parentElement in parentElements)
            {
                if (vals is ExpandoObject && vals.ContainsKey(parentElement))
                {
                    if (vals[parentElement] is System.Collections.Generic.IList<Object>)
                    {
                        parent = vals[parentElement] as System.Collections.Generic.IList<Object>;
                        break;
                    }
                    vals = vals[parentElement] as IDictionary<string, object>;
                }
            }
            return parent;
        }

        private void MoveData(DataSourceDesign design, IList<DataSourceElementMapping> filterMaps, IList<Object> source, IList<Object> target)
        {
            if (source != null && target != null)
            {
                //List of key elements in  
                var keyElementMappingListCount = (from k in design.Mappings
                                                  where k.IsKey == true
                                                  select k).Count();
                if (keyElementMappingListCount != null && keyElementMappingListCount > 0)
                {
                    target = MoveKeyElementData(design.Mappings, keyElementMappingListCount, filterMaps, source, target);
                }
                else
                {
                    target = MoveNonKeyElementData(filterMaps, source, target);
                }
            }
        }

        private IList<object> MoveKeyElementData(IList<DataSourceElementMapping> mappingList, int keyElmentCount, IList<DataSourceElementMapping> filterMaps, IList<object> source, IList<object> target)
        {
            if (target.Count() == 1)
            {
                IDictionary<string, object> targetDyn = target[0] as IDictionary<string, object>;
                short found = 0;
                foreach (var element in targetDyn)
                {
                    if ((element.Value != null && element.Value is System.Collections.Generic.IList<Object>) && String.IsNullOrWhiteSpace(element.Value.ToString()))
                    {
                        found++;
                    }
                }
                if (found == targetDyn.Count())
                {
                    target.RemoveAt(0);
                }
            }
            //get list of DatacopyMode is Once mode
            //if no record in DataCopymode is consider Always Mode other wise it will Once
            var copyElementOnceList = (from k in design.Mappings
                                       where k.CopyModeID == (int)DataCopyMode.Once
                                       select k).ToList();
            IList<int> indexesToFilter = new List<int>();

            IList<Object> newList = new List<Object>();

            List<string> nonKeyElements = new List<string>();
            if (design.DisplayMode == "Primary")
            {
                var mapElements = from key in design.Mappings select key.TargetElement;
                List<string> mapElems = new List<string>();
                if (mapElements != null && mapElements.Count() > 0)
                {
                    mapElems = mapElements.ToList();
                }
                RepeaterDesign targetDesign = GetTargetRepeaterDesign(design);
                var elementNames = from elem in targetDesign.Elements where elem.IsPrimary == true && !mapElems.Contains(elem.GeneratedName) select elem.GeneratedName;
                if (elementNames != null && elementNames.Count() > 0)
                {
                    nonKeyElements = elementNames.ToList();
                }
            }
            
            //if (copyElementOnceList.Count() == 0 || (copyElementOnceList.Count() > 0 && target.Count() == 0))
            //{
            for (int idx = 0; idx < source.Count(); idx++)
            {
                IDictionary<string, object> sourceDyn = source[idx] as IDictionary<string, object>;
                //if ((filterMaps.Count() == 0) || (filterMaps.Count() > 0 && Filter(filterMaps, sourceDyn, "Source")))
                //{
                short keyCount = 0;
                short matchCount = 0;
                for (int i = 0; i < target.Count(); i++)
                {
                    matchCount = 0;
                    keyCount = 0;
                    IDictionary<string, object> targetDyn = target[i] as IDictionary<string, object>;
                    foreach (var mapping in mappingList)
                    {
                        if (sourceDyn.ContainsKey(mapping.SourceElement) && targetDyn.ContainsKey(mapping.TargetElement))
                        {
                            if (sourceDyn[mapping.SourceElement] == null) 
                            {
                                sourceDyn[mapping.SourceElement] = "";
                            }
                            if (targetDyn[mapping.TargetElement] == null)
                            {
                                targetDyn[mapping.TargetElement] = "";
                            }
                            if (sourceDyn[mapping.SourceElement].ToString() == targetDyn[mapping.TargetElement].ToString())
                            {
                                matchCount++;
                                //check if this is part of key columns
                                if (mapping.IsKey)
                                    keyCount++;
                            }
                        }
                    }//if match the all element then use break to avoid next loop search
                    //add remaining fields
                    if (matchCount == mappingList.Count() && keyCount == keyElmentCount)
                    {
                        newList.Add(targetDyn);
                        foreach (string nonKeyElement in nonKeyElements)
                        {
                            if (!targetDyn.Keys.Contains(nonKeyElement))
                            {
                                targetDyn.Add(nonKeyElement, "");
                            }
                        }
                        break;
                    }

                    if (matchCount < mappingList.Count() && keyCount == keyElmentCount)
                    {
                        foreach (var mapping in mappingList)
                        {
                            if (sourceDyn.ContainsKey(mapping.SourceElement))
                            {
                                if (mapping.CopyModeID == 1)
                                {
                                    targetDyn[mapping.TargetElement] = sourceDyn[mapping.SourceElement].ToString();
                                }
                            }
                        }
                        foreach (string nonKeyElement in nonKeyElements)
                        {
                            if (!targetDyn.Keys.Contains(nonKeyElement))
                            {
                                targetDyn.Add(nonKeyElement, "");
                            }
                        }
                        newList.Add(targetDyn);
                        break;
                    }
                }
                if (keyCount < keyElmentCount)
                {
                    ExpandoObject newTarget = new ExpandoObject();
                    IDictionary<string, object> targetList = newTarget as IDictionary<string, object>;
                    foreach (var mapping in design.Mappings)
                    {
                        if (sourceDyn.ContainsKey(mapping.SourceElement))
                        {
                            if (!targetList.ContainsKey(mapping.SourceElement))
                            {
                                targetList.Add(mapping.TargetElement, sourceDyn[mapping.SourceElement]);
                            }
                        }
                        else
                        {
                            if (!targetList.ContainsKey(mapping.SourceElement))
                            {
                                targetList.Add(mapping.TargetElement, "");
                            }
                        }
                    }
                    if (targetList.Count() == mappingList.Count())
                    {
                        foreach (string nonKeyElement in nonKeyElements)
                        {
                            if (!targetList.Keys.Contains(nonKeyElement))
                            {
                                targetList.Add(nonKeyElement, "");
                            }
                        }
                        target.Add(targetList);
                        newList.Add(targetList);
                    }
                }
                //}
            }

            if (filterMaps.Count() > 0)
            {
                for (int i = 0; i < newList.Count(); i++)
                {
                    IDictionary<string, object> targetDyn = newList[i] as IDictionary<string, object>;
                    if (!Filter(filterMaps, targetDyn, "Target"))
                    {
                        indexesToFilter.Add(i);
                    }
                }
                if (indexesToFilter.Count() > 0)
                {
                    IOrderedEnumerable<int> deleteOrder = indexesToFilter.OrderByDescending(d => d);
                    foreach (int delIndex in deleteOrder)
                    {
                        newList.RemoveAt(delIndex);
                    }
                }
            }
            //}
            if (source.Count() == 0 && target.Count() > 0)
            {
                foreach (var tList in target.ToList())
                {
                    target.Remove(tList);
                }
            }
            int count = target.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                target.RemoveAt(i);
            }

            foreach (var value in newList)
            {
                target.Add(value);
            }
            return target;
        }

        private IList<object> MoveNonKeyElementData(IList<DataSourceElementMapping> filterMaps, IList<object> source, IList<object> target)
        {
            int sourceCount = source.Count();
            int targetCount = target.Count();
            IList<int> indexesToFilter = new List<int>();
            for (int idx = 0; idx < sourceCount; idx++)
            {
                IDictionary<string, object> sourceDyn = source[idx] as IDictionary<string, object>;
                if ((filterMaps.Count() == 0) || (filterMaps.Count() > 0 && Filter(filterMaps, sourceDyn, "Source")))
                {
                    if (targetCount > idx)
                    {
                        IDictionary<string, object> targetDyn = target[idx] as IDictionary<string, object>;
                        foreach (var mapping in design.Mappings)
                        {
                            if (sourceDyn.ContainsKey(mapping.SourceElement) && targetDyn.ContainsKey(mapping.TargetElement))
                            {
                                if (targetDyn[mapping.TargetElement] == null || (targetDyn[mapping.TargetElement] is String && String.IsNullOrEmpty(targetDyn[mapping.TargetElement] as String)))
                                {
                                    targetDyn[mapping.TargetElement] = sourceDyn[mapping.SourceElement];
                                }
                            }
                        }
                    }
                    else
                    {
                        ExpandoObject newTarget = new ExpandoObject();
                        IDictionary<string, object> targetDyn = newTarget as IDictionary<string, object>;
                        foreach (var mapping in design.Mappings)
                        {
                            if (sourceDyn.ContainsKey(mapping.SourceElement))
                            {
                                if (!targetDyn.ContainsKey(mapping.TargetElement))
                                {
                                    targetDyn.Add(mapping.TargetElement, sourceDyn[mapping.SourceElement]);
                                }
                            }
                            else
                            {
                                if ((!targetDyn.ContainsKey(mapping.TargetElement)) && design.DisplayMode != "Primary" &&
                                            target != null)
                                {
                                    targetDyn.Add(mapping.TargetElement, null);
                                }
                            }
                        }
                        if (targetDyn.Count() > 0)
                            target.Add(targetDyn);
                    }
                }
                else
                {
                    if (idx < target.Count())
                    {
                        indexesToFilter.Add(idx);
                    }
                }
            }
            if (indexesToFilter.Count() > 0)
            {
                IOrderedEnumerable<int> deleteOrder = indexesToFilter.OrderByDescending(d => d);
                foreach (int delIndex in deleteOrder)
                {
                    target.RemoveAt(delIndex);
                }
            }
            if (sourceCount == 0 && target.Count() > 0)
            {
                foreach (object tList in target.ToList())
                {
                    target.Remove(tList);
                }
            }
            return target;
        }

        private IList<DataSourceElementMapping> GetFilters(DataSourceDesign design)
        {
            var maps = from map in design.Mappings where map.Filter != null && map.OperatorID > 0 select map;
            return maps.ToList();
        }

        private bool Filter(IList<DataSourceElementMapping> filterMaps, IDictionary<string, object> elementList, string mode)
        {
            bool isIncluded = false;
            short found = 0;
            foreach (var mapping in filterMaps)
            {
                string mapElement = String.Empty;
                mapElement = Convert.ToString(mode == "Source" ? mapping.SourceElement : mapping.TargetElement);
                if (elementList.ContainsKey(mapElement))
                {
                    var val = elementList[mapElement];
                    int fValue = 0;
                    int sValue = 0;
                    switch (mapping.OperatorID)
                    {
                        case 1: //equals
                            if (val != null && mapping.Filter != null && Convert.ToString(val).ToUpper() == Convert.ToString(mapping.Filter).ToUpper())
                            {
                                found++;
                            }
                            break;
                        case 2: //greater than                           
                            if (int.TryParse(Convert.ToString(val), out fValue) && int.TryParse(Convert.ToString(mapping.Filter), out sValue) && fValue > sValue)
                            {
                                found++;
                            }
                            break;
                        case 3: //less than
                            if (int.TryParse(Convert.ToString(val), out fValue) && int.TryParse(Convert.ToString(mapping.Filter), out sValue) && fValue < sValue)
                            {
                                found++;
                            }
                            break;
                        case 4: //greater than or equals
                            if (int.TryParse(Convert.ToString(val), out fValue) && int.TryParse(Convert.ToString(mapping.Filter), out sValue) && fValue >= sValue)
                            {
                                found++;
                            }
                            break;
                        case 5: //less than or equals
                            if (int.TryParse(Convert.ToString(val), out fValue) && int.TryParse(Convert.ToString(mapping.Filter), out sValue) && fValue <= sValue)
                            {
                                found++;
                            }
                            break;
                        case 6: //contains
                            if (Convert.ToString(val).ToUpper().Contains(Convert.ToString(mapping.Filter).ToUpper()))
                            {
                                found++;
                            }
                            break;
                        case 7: //is null
                            if ((Convert.ToString(val).ToUpper() == Convert.ToString(mapping.Filter).ToUpper()) || (string.IsNullOrEmpty(Convert.ToString(val))))
                            {
                                found++;
                            }
                            break;
                        case 8: //not equals
                            if (val != null && mapping.Filter != null && Convert.ToString(val).ToUpper() != Convert.ToString(mapping.Filter).ToUpper())
                            {
                                found++;
                            }
                            break;
                    }
                }
            }
            //if match  all filter value 
            if (found == filterMaps.Count())
            {
                isIncluded = true;
            }
            return isIncluded;
        }

        private void GroupHeader(DataSourceDesign design, IList<object> sourceParent)
        {
            design.GroupHeader = new List<Object>();
            for (int i = 0; i < sourceParent.Count; i++)
            {
                IDictionary<string, object> sourceData = sourceParent[i] as IDictionary<string, object>;
                Dictionary<string, string> obj = new  Dictionary<string, string>();
               // string titleText = "";
                foreach (DataSourceElementMapping mapp in design.Mappings)
                {
                    if (sourceData.ContainsKey(mapp.SourceElement) && mapp.IsKey)
                    {
                        obj.Add(mapp.SourceElement.ToString(), sourceData[mapp.SourceElement].ToString());                       
                    }
                }
                design.GroupHeader.Add(obj);
            }
        }

        private RepeaterDesign GetTargetRepeaterDesign(DataSourceDesign dataSource)
        {
            string[] fullNameArray = dataSource.TargetParent.Split('.');
            SectionDesign targetParentSection = targetDesign.Sections.Where(s => s.GeneratedName == fullNameArray[0]).FirstOrDefault();
            ElementDesign targetRepeaterDesignElement = null;
            if (fullNameArray.Length > 1)
            {
                for (int i = 1; i < fullNameArray.Length; i++)
                {
                    if (targetRepeaterDesignElement == null)
                    {
                        targetRepeaterDesignElement = targetParentSection.Elements.Where(e => e.GeneratedName == fullNameArray[i]).FirstOrDefault();
                    }
                    else
                    {
                        targetRepeaterDesignElement = targetRepeaterDesignElement.Section.Elements.Where(e => e.GeneratedName == fullNameArray[i]).FirstOrDefault();
                    }
                }
            }
            return targetRepeaterDesignElement == null ? null : targetRepeaterDesignElement.Repeater;
        }
        #endregion Private Methods
    }


}
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;

namespace tmg.equinox.web.DataSource
{
    public class DropDownDataSourceMapper
    {
        private ExpandoObject source;
        private DataSourceDesign design;
        private FormDesignVersionDetail targetDesign;

        public DropDownDataSourceMapper(DataSourceDesign design, ExpandoObject source, FormDesignVersionDetail targetDesign)
        {
            this.source = source;
            this.design = design;
            this.targetDesign = targetDesign;
        }

        public void MergeDataSource()
        {
            //get data from data source and populate the dropdown items into the target design
            IList<Object> sourceParent = GetParentObject(source, design.SourceParent);
            ElementDesign dropdownDesign = GetDropDownDesign();
            string sourceElement = "";
            if (design.Mappings != null && design.Mappings.Count() == 1)
            {
                sourceElement = design.Mappings[0].SourceElement;
                FillDropDownItems(sourceParent, sourceElement, dropdownDesign);
            }
        }

        private ElementDesign GetDropDownDesign()
        {
            ElementDesign dropdownDesign = null;
            string parentElement = design.TargetParent;
            string[] parentElements = parentElement.Split('.');
            //first get top level section
            SectionDesign sectionDesign = (from secn in targetDesign.Sections where secn.GeneratedName == parentElements[0] select secn).FirstOrDefault();
            RepeaterDesign repeaterDesign = null;
            ElementDesign elementDesign = null;
            for (int idx = 1; idx < parentElements.Count(); idx++)
            {
                if (sectionDesign != null)
                {
                    elementDesign = (from elem in sectionDesign.Elements where elem.GeneratedName == parentElements[idx] select elem).FirstOrDefault();
                }
                if (repeaterDesign != null)
                {
                    elementDesign = (from elem in repeaterDesign.Elements where elem.GeneratedName == parentElements[idx] select elem).FirstOrDefault();
                }
                if (elementDesign != null)
                {
                    if (elementDesign.Repeater != null)
                    {
                        repeaterDesign = elementDesign.Repeater;
                        sectionDesign = null;
                    }
                    if (elementDesign.Section != null)
                    {
                        sectionDesign = elementDesign.Section;
                        repeaterDesign = null;
                    }
                }
            }
            if (design.Mappings.Count() > 0)
            {
                string targetElement = design.Mappings[0].TargetElement;
                ElementDesign element = null;
                if (repeaterDesign != null)
                {
                    element = (from el in repeaterDesign.Elements where el.GeneratedName == targetElement select el).FirstOrDefault();
                }
                if (sectionDesign != null)
                {
                    element = (from el in sectionDesign.Elements where el.GeneratedName == targetElement select el).FirstOrDefault();
                }
                if (element != null)
                {
                    dropdownDesign = element;
                }

            }
            return dropdownDesign;
        }

        private IList<Object> GetParentObject(ExpandoObject sourceObject, string fullParentName)
        {
            IList<Object> parent = new List<Object>();
            string[] parentElements = fullParentName.Split('.');
            IDictionary<string, object> vals = sourceObject as IDictionary<string, object>;
            foreach (var parentElement in parentElements)
            {
                try
                {
                    
                    if (vals is ExpandoObject)
                    {
                        if (vals.ContainsKey(parentElement))
                        {
                            if (vals[parentElement] is System.Collections.Generic.IList<Object>)
                            {
                                parent = vals[parentElement] as System.Collections.Generic.IList<Object>;
                                break;
                            }
                            vals = vals[parentElement] as IDictionary<string, object>;
                        }
                    }
                }
                catch (Exception ex)
                { 
                
                
                }
            }
            return parent;
        }

        private void FillDropDownItems(IList<Object> source, string sourceElement, ElementDesign dropdownDesign)
        {
            if (source != null && dropdownDesign != null)
            {
                dropdownDesign.Items.Clear();
                int sourceCount = source.Count();
                for (int idx = 0; idx < sourceCount; idx++)
                {
                    IDictionary<string, object> sourceDyn = source[idx] as IDictionary<string, object>;
                    if (sourceDyn.ContainsKey(sourceElement))
                    {
                        Item ddItem = new Item();
                        ddItem.ItemValue = sourceDyn[sourceElement].ToString() != null ? sourceDyn[sourceElement].ToString().Trim() : null;
                        ddItem.ItemText = sourceDyn[sourceElement].ToString() != null ? sourceDyn[sourceElement].ToString().Trim() : null;
                        if (!string.IsNullOrEmpty(ddItem.ItemValue))
                        {
                            dropdownDesign.Items.Add(ddItem);
                        }
                    }
                }
                //this code needs to be uncommented when, we would like to sort items for dropdowns
                //in a section on server side itself. 
                //dropdownDesign.Items.Sort(new SortDropDownElementItemsComparer());
            }
        }
    }

    //class to sort items in ascending order for the drop down list in section.
    internal class SortDropDownElementItemsComparer : IComparer<Item>
    {

        public int Compare(Item itemA, Item itemB)
        {
            var itemAVal = Regex.Replace(itemA.ItemValue.ToLower(), "[^a-zA-Z0-9_-]", "", RegexOptions.IgnoreCase);
            var itemBVal = Regex.Replace(itemB.ItemValue.ToLower(), "[^a-zA-Z0-9_-]", "", RegexOptions.IgnoreCase);

            double itemAValue = 0, itemBValue = 0;
            bool isItemADouble = double.TryParse(itemAVal, out itemAValue);
            bool isItemBDouble = double.TryParse(itemBVal, out itemBValue);

            if (isItemADouble && isItemBDouble)
            {
                return itemAValue.CompareTo(itemBValue);
            }
            else if (isItemADouble)
                return 1;
            else if (isItemBDouble)
                return -1;
            else
                return itemAVal.CompareTo(itemBVal);
        }
    }
}
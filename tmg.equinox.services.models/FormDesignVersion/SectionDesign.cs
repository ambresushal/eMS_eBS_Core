using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesign;

namespace tmg.equinox.applicationservices.viewmodels.FormDesignVersion
{

    public class SectionDesign
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string GeneratedName { get; set; }
        public string FullName { get; set; }
        public string Label { get; set; }
        public string HelpText { get; set; }
        public int Sequence { get; set; }
        public int ChildCount { get; set; }
        public bool Visible { get; set; }
        public bool Enabled { get; set; }
        public int? LayoutColumn { get; set; }
        public string LayoutClass { get; set; }
        public string CustomHtml { get; set; }
        public List<ElementDesign> Elements { get; set; }
        public List<ElementAccessViewModel> AccessPermissions { get; set; }
        public string SectionNameTemplate { get; set; }
        public string EffDt { get; set; }
        public string Op { get; set; }
        public string MDMName { get; set; }

        internal void BuildDefaultJSONDataObject(ref StringBuilder jsonBuilder, bool isNewFormInstance)
        {
            jsonBuilder.Append("\"" + this.GeneratedName + "\": { ");
            for (int index = 0; index < this.Elements.Count; index++)
            {
                if (!String.IsNullOrEmpty(this.Elements[index].GeneratedName) && this.Elements[index].Op != "I")
                {
                    this.Elements[index].BuildDefaultJSONDataObject(ref jsonBuilder, isNewFormInstance);
                    if (index < (this.Elements.Count - 1))
                    {
                        jsonBuilder.Append(" , ");
                    }
                }
            }
            jsonBuilder.Append('}');
        }

        internal void BuildSyncMacro(ref StringBuilder jsonBuilder, bool isNewFormInstance)
        {
            jsonBuilder.Append("\"" + this.GeneratedName + "\": { ");
            var repeaters = from elem in this.Elements where elem.Repeater != null select elem;
            var sections = from elem in this.Elements where elem.Section != null select elem;
            var fields = from elem in this.Elements where elem.Repeater == null && elem.Section == null select elem;
            int index = 0;
            foreach (var elem in repeaters)
            {
                if (!String.IsNullOrEmpty(elem.GeneratedName))
                {
                    elem.Repeater.BuildSyncMacro(ref jsonBuilder, isNewFormInstance);
                    if (index < (repeaters.Count() - 1))
                    {
                        jsonBuilder.Append(" , ");
                    }
                }
                index++;
            }
            index = 0;
            if (repeaters.Count() > 0 && sections.Count() > 0)
            {
                jsonBuilder.Append(" , ");
            }

            foreach (var elem in sections)
            {
                if (!String.IsNullOrEmpty(elem.GeneratedName))
                {
                    elem.Section.BuildSyncMacro(ref jsonBuilder, isNewFormInstance);
                    if (index < (sections.Count() - 1))
                    {
                        jsonBuilder.Append(" , ");
                    }
                }
                index++;
            }
            if (repeaters.Count() > 0 || sections.Count() > 0)
            {
                jsonBuilder.Append(" , ");
            }
            jsonBuilder.Append(" \"Label\" : \"" + this.Label + "\" ,");
            jsonBuilder.Append(" \"Fields\" : [ ");
            //if (fields != null)
            //{
            //    index = 0;
            //    foreach (var elem in fields)
            //    {
            //        if (!String.IsNullOrEmpty(elem.GeneratedName))
            //        {
            //            jsonBuilder.Append(" { \"Field\" : \"");
            //            jsonBuilder.Append(elem.GeneratedName);
            //            jsonBuilder.Append("\" }");
            //        }
            //        if (index < fields.Count() - 1)
            //        {
            //            jsonBuilder.Append(" , ");
            //        }
            //        index++;
            //    }
            //}
            jsonBuilder.Append(" ] } ");
        }
    }
}

using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class SectionUIElement : UIElement
    {
        public int UIElementTypeID { get; set; }
        public int ChildCount { get; set; }
        public int LayoutTypeID { get; set; }
        public Nullable<int> DataSourceID { get; set; }
        public virtual LayoutType LayoutType { get; set; }
        //public virtual UIElement UIElement { get; set; }
        public virtual UIElementType UIElementType { get; set; }
        public virtual DataSource DataSource { get; set; }
        public string CustomHtml { get; set; }
    }
}

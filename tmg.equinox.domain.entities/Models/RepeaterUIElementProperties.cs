using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public class RepeaterUIElementProperties : Entity
    {
        public int RepeaterUIElementPropertyID { get; set; }
        public int RepeaterUIElementID { get; set; }
        public string RowTemplate { get; set; }
        public string HeaderTemplate { get; set; }
        public string FooterTemplate { get; set; }
    }
}

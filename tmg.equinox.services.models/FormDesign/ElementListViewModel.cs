﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FormDesign
{
    public class ElementListViewModel : ViewModelBase
    {
        public int UIElementID { get; set; }
        public string UIElementName { get; set; }
        public string ElementLabel { get; set; }
        public string ElementFullPath { get; set; }
        public string ElementJSONPath { get; set; }
        public int? Parent { get; set; }
        public bool IsContainer { get; set; }
    }
}

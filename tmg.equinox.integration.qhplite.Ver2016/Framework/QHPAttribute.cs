using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    internal class QHPSetting : Attribute
    {
        public QHPSheetType SheetType { get; set; }
        public int Row { get; set; }
        public string Column { get; set; }
        public int IncrementStep { get; set; }
        public IncrementDirection IncrementDirection { get; set; }
        public bool IsContainer { get; set; }
        public bool IsList { get; set; }
    }
}

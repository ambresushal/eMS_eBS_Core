using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class QhpToExcelMap
    {
        public string ParentPropertyName { get; set; }
        public string FormPropertyName { get; set; }
        public string DomainPropertyName { get; set; }
        public string ColumnName { get; set; }
        public int RowIndex { get; set; }
        public int IncrementStep { get; set; }
        public QHPSheetType QhpSheetType { get; set; }
        public bool IsHeader { get; set; }
        public QhpCellFormat CellFormat { get; set; }

        public QhpToExcelMap()
        {
            CellFormat = new QhpCellFormat();
        }
    }
}

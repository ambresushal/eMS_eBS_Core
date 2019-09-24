using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class BenefitMapping : Entity
    {
        
        public int MappingID { get; set; }
        public string PBPFile { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public int Length { get; set; }
        public string FieldTitle { get; set; }
        public string Title { get; set; }
        public string Codes { get; set; }
        public string Code_Values { get; set; }
        public int FormDesignVersionID { get; set; }
        public int FormDesignID { get; set; }
        public string TableName { get; set; }
        public string MappingType { get; set; }
        public string DocumentPath { get; set; }
        public string ElementType { get; set; }
        public bool IsArrayElement { get; set; }
        public bool IsActive { get; set; }
        public string ViewType { get; set; }
        public string SOTDocumentPath { get; set; }
        public string SOTPrefix { get; set; }
        public string SOTSuffix { get; set; }
        public string IfBlankThenValue { get; set; }
        public bool IsYesNoField { get; set; }
        public bool IsCheckBothFields { get; set; }
        public string SetSimilarValues { get; set; }
        public string SectionGeneratedName { get; set; }
        public virtual ICollection<BenefitsDictionary> BenefitsDictionaries { get; set; }

    }
}

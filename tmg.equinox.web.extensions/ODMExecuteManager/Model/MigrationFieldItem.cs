using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.web.ODMExecuteManager.Model
{
    public class MigrationFieldItem
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
        public string ViewType { get; set; }
        public string SOTDocumentPath { get; set; }
        public string SOTPrefix { get; set; }
        public string SOTSuffix { get; set; }
        public string IfBlankThenValue { get; set; }
        public bool IsYesNoField { get; set; }
        public bool IsCheckBothFields { get; set; }
        public string SetSimilarValues { get; set; }
        public string SectionGeneratedName { get; set; }
        public List<FieldItem> Dictionaryitems { get; set; }

        public MigrationFieldItem Clone()
        {
            MigrationFieldItem item = new MigrationFieldItem();
            item.Codes = this.Codes;
            item.Code_Values = this.Code_Values;
            item.ColumnName = this.ColumnName;
            item.DataType = this.DataType;
            item.Dictionaryitems = this.Dictionaryitems;
            item.DocumentPath = this.DocumentPath;
            item.ElementType = this.ElementType;
            item.FieldTitle = this.FieldTitle;
            item.FormDesignID = this.FormDesignID;
            item.FormDesignVersionID = this.FormDesignVersionID;
            item.IsArrayElement = this.IsArrayElement;
            item.Length = this.Length;
            item.MappingID = this.MappingID;
            item.MappingType = this.MappingType;
            item.PBPFile = this.PBPFile;
            item.TableName = this.TableName;
            item.Title = this.Title;
            item.ViewType = this.ViewType;
            return item;
        }
    }
}

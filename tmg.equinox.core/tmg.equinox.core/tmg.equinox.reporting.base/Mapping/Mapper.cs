using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.repository.models;

namespace tmg.equinox.reporting.Base
{
    public class Mapper : IMapper<IList<ReportMappingField>>
    {
        IList<ReportMappingField> _map;
        ReportSetting _reportConfig;
       

        
        private void Read()
        {

            _map = new List<ReportMappingField>();

            //

            FileInfo existingFile = new FileInfo(_reportConfig.ReportTemplatePath);

            using (var package = new ExcelPackage(existingFile))
            {

                ExcelWorksheet sheet = package.Workbook.Worksheets["SheetName"];

                var totalRows = sheet.Dimension.End.Row;

                for (int i = 2; i <= totalRows; i++)
                {
                    if (sheet.Cells[i, 4].Text != "")
                    {
                        bool isrule = false;
                        if (sheet.Cells[i, 4].Text.StartsWith("Rule"))
                            isrule = true;

                        _map.Add(new ReportMappingField
                        {
                            DBFieldame = sheet.Cells[i, 6].Text,
                            DisplayName = sheet.Cells[i, 7].Text,
                            Expression = sheet.Cells[i, 8].Text,
                            DataFieldName= sheet.Cells[i, 9].Text,
                            isRule = isrule,
                            RuleName = sheet.Cells[i, 4].Text
                        });
                    }
                }
            }
        }
        public IList<ReportMappingField> GetMapping(ReportSetting reportConfig)
        {
            _reportConfig = reportConfig;
            if (_map == null)
                Read();
            return _map;
        }

       
    }
}

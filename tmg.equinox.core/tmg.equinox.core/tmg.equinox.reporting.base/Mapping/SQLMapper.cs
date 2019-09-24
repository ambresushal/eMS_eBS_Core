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

namespace tmg.equinox.reporting.Base.Mapping
{
    public class SQLMapper : IMapper<string>
    {


        public string GetMapping(ReportSetting reportConfig)
        {
      
            FileInfo existingFile = new FileInfo(reportConfig.ReportTemplatePath);

            using (var package = new ExcelPackage(existingFile))
            {

                ExcelWorksheet sheet = package.Workbook.Worksheets["SheetName"];
                return sheet.Cells[1, 1].Text;
                //to do write logic to fill map

            }
            }
        }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using tmg.equinox.integration.qhplite.Ver2016.QHPModel;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class QHPFileReader : IQHPFileReader
    {
        private XSSFWorkbook workbook;
        public List<PlanBenefitPackage> GetPackagesFromFile(string fileName)
        {
            IExcelFileReader fileReader = new ExcelFileReader();
            MemoryStream qhpStream = fileReader.GetExcelFile(fileName);
            workbook = new XSSFWorkbook(qhpStream);
            return BuildPackages();
        }

        private List<PlanBenefitPackage> BuildPackages()
        {
            List<PlanBenefitPackage> packages = new List<PlanBenefitPackage>();
            Dictionary<int, ISheet> bpSheets = GetBenefitPackageSheets();
            foreach (var bpSheet in bpSheets) 
            {
                ISheet cvSheet = GetMatchingCostVarianceSheet(bpSheet.Key, bpSheet.Value);
                if (cvSheet != null)
                {
                    ExcelToQHPModelMapper mapper = new ExcelToQHPModelMapper(workbook, bpSheet.Value, cvSheet);
                    packages.Add(mapper.GetBenefitPackage());
                }
            }
            return packages;
        }

        private Dictionary<int,ISheet> GetBenefitPackageSheets() 
        {
            Dictionary<int, ISheet> sheets = new Dictionary<int,ISheet>();
            for (int index = 0; index < workbook.NumberOfSheets; index++) 
            {
                ISheet sheet = workbook.GetSheetAt(index);
                try
                {
                    if (sheet.SheetName != "DefaultBP" && sheet.GetRow(1).GetCell(0).StringCellValue == "HIOS Issuer ID*")
                    {
                        sheets.Add(index,sheet);
                    }
                }
                catch 
                { 
                }
            }
            return sheets;
        }

        private ISheet GetMatchingCostVarianceSheet(int benefitPackageSheetIndex, ISheet benefitPackageSheet)
        {
            ISheet costShareVariancesheet = null;
            int costShareVarianceSheetIndex = benefitPackageSheetIndex + 1;
            /*TODO: need to add more specific check to confirm that the Cost Share Variance Sheet is 
             * matching the Benefit Package Sheet in terms of content
             */
            if (workbook.NumberOfSheets > costShareVarianceSheetIndex)
            {
                ISheet probableSheet = workbook.GetSheetAt(costShareVarianceSheetIndex);
                if (probableSheet.GetRow(1).GetCell(0).StringCellValue == "Plan Cost Sharing Attributes" || probableSheet.GetRow(1).GetCell(0).StringCellValue == "Cost Sharing Reduction Information") 
                {
                    costShareVariancesheet = probableSheet;
                }
             
            }
            return costShareVariancesheet;
        }
    }
}

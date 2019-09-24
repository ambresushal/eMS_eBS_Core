using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IMLImportHelperService
    {
        string FormatCurrency(decimal Value);

        string JsonColumnNameFormatting(string JColumn);

        string JsonValueFormatting(string JValue);

        string PremiumMLJsonFormatting(DataTable table);

        string FormularyInfoMLJsonFormatting(DataTable table);

        string FIPSMLJsonFormatting(DataTable table);

        string BenchmarkInfoMLJsonFormatting(DataTable table);

        string PrescriptionMLJsonFormatting(DataTable table, DateTime effectiveDate);

        string FIPSCreateJson(DataTable table);

        string PremiumMLCreateJson(DataTable table);

        string FormularyInfoMLCreateJson(DataTable table);

        string BenchmarkInfoMLCreateJson(DataTable table);

        string PrescriptionMLCreateJson(DataTable table);

        bool ProcessMasterListImportTemplateValidation(string MLSectionName, DataTable SourceTable,string filePath);

    }
}

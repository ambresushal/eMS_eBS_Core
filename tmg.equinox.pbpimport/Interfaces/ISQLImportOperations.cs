using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.PBPImport;

namespace tmg.equinox.pbpimport.Interfaces
{
    public interface ISQLImportOperations
    {
        void InitializeVariables(string connectingString);
        bool CreateTableStructure(string strCreateTableScript);
        bool ImportDataToSqlServer(DataTable sourceDataTable);
        bool ImportDataToSqlServer(DataTable sourceDataTable, string destinationTableName, List<PBPImportTableColumnsViewModel> PBPImportTableColumnsViewModel);
    }
}

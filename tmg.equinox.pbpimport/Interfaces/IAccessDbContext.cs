using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.pbpimport.Interfaces
{
    public interface IAccessDbContext
    {
        DataTable ExecuteSelectQuery(string query, OleDbParameter[] parameters);
        string ExecuteReader(string query, OleDbParameter[] parameters);
        DataTable GetUsedTables();
        void InitializeVariables(string mdfDbPath);
        string GetConnectingString();
    }
}

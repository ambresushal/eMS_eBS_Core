using System;
using System.Threading.Tasks;
using tmg.equinox.schema.Base.Interface;
using tmg.equinox.schema.Base.Model;

namespace tmg.equinox.schema.Base
{
    public class SchemeManager
    {
        IPrepareSchema _prepareSchema;
        IGenerateSchema _generateSchema;
        ISQLStatement _sqlStatement;
        private static readonly Object _lockObject = new Object();
        public SchemeManager(IPrepareSchema prepareSchema, IGenerateSchema generateSchema)
        {
            _prepareSchema = prepareSchema;
            _generateSchema = generateSchema;
        }
        public SchemeManager(ISQLStatement sqlStatement)
        {
            _sqlStatement = sqlStatement;
        }
        public virtual void Execute()
        {
            _prepareSchema.PrepareSchema();
            _generateSchema.CreateSchema(_prepareSchema.GetSchema());
        }

        public virtual void UpdateDatabase(JData jData)
        {
            //lock (_lockObject)
            //{
              _sqlStatement.UpdateReportingDatabase(jData);
            //}
        }

    }
}

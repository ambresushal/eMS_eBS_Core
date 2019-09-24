using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.domain.Impl
{
    public class MultipleResultSetWrapper
    {
        private readonly DbContext _dbContext;
        private readonly string _storedProcedure;
        public List<Func<IObjectContextAdapter, DbDataReader, IEnumerable>> _resultSets;
        object[] _parameters;
        private List<IEnumerable> results { get; set; }

        public MultipleResultSetWrapper(DbContext dbContext, string storedProcedure, params object[] parameters)
        {
            _dbContext = dbContext;
            _storedProcedure = storedProcedure;
            _parameters = parameters;
            _resultSets = new List<Func<IObjectContextAdapter, DbDataReader, IEnumerable>>();
        }

        public MultipleResultSetWrapper With<TResult>()
        {
            _resultSets.Add((adapter, reader) => adapter
                .ObjectContext
                .Translate<TResult>(reader)
                .ToList());

            return this;
        }

        public List<IEnumerable> Execute()
        {
            results = new List<IEnumerable>();
            DbConnection connection = _dbContext.Database.Connection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = _storedProcedure;
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddRange(_parameters.ToArray());
            using (var reader = command.ExecuteReader())
            {
                var adapter = ((IObjectContextAdapter)_dbContext);
                foreach (var resultSet in _resultSets)
                {
                    results.Add(resultSet(adapter, reader));
                    reader.NextResult();
                }
            }
            return results;
        }
    }
}

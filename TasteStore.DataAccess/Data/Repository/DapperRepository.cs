using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using TasteStore.DataAccess.Data.Repository.IRepository;

namespace TasteStore.DataAccess.Data.Repository
{
    public class DapperRepository : IDapperRepository
    {
        private readonly ApplicationDbContext _context;

        private readonly string _connectionString;

        public DapperRepository(ApplicationDbContext context)
        {
            _context = context;
            _connectionString = _context.Database.GetDbConnection().ConnectionString;
        }

        public void Execute(string procedureName, DynamicParameters parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(procedureName, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public T Get<T>(string procedureName, DynamicParameters parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return (T)Convert.ChangeType(connection.ExecuteScalar<T>(procedureName, parameters, commandType: CommandType.StoredProcedure), typeof(T));
            }
        }

        public IEnumerable<T> GetAll<T>(string procedureName, DynamicParameters parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

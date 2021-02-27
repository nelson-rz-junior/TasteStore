using Dapper;
using System;
using System.Collections.Generic;

namespace TasteStore.DataAccess.Data.Repository.IRepository
{
    public interface IDapperRepository : IDisposable
    {
        void Execute(string procedureName, DynamicParameters parameters = null);

        T Get<T>(string procedureName, DynamicParameters parameters = null);

        IEnumerable<T> GetAll<T>(string procedureName, DynamicParameters parameters = null);
    }
}

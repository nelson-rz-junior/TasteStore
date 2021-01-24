using System;
using System.Collections.Generic;
using System.Text;

namespace TasteStore.DataAccess.Data.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository CategoryRepository { get; }

        void Save();
    }
}

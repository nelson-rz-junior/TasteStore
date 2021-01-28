using System;
using System.Collections.Generic;
using System.Text;
using TasteStore.DataAccess.Data.Repository.IRepository;

namespace TasteStore.DataAccess.Data.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository CategoryRepository { get; }

        IFoodTypeRepository FoodTypeRepository { get; }

        IMenuItemRepository MenuItemRepository { get; }

        void Save();
    }
}

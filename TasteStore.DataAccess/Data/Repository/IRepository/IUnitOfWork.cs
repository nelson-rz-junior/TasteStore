﻿using System;
using TasteStore.DataAccess.Data.Repository.IRepository;

namespace TasteStore.DataAccess.Data.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository CategoryRepository { get; }

        IFoodTypeRepository FoodTypeRepository { get; }

        IMenuItemRepository MenuItemRepository { get; }

        IApplicationUserRepository ApplicationUserRepository { get; }

        void Save();
    }
}

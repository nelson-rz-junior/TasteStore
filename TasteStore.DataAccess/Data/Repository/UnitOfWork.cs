using System;
using System.Collections.Generic;
using System.Text;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.DataAccess.Data.Repository.IRepository;

namespace TasteStore.DataAccess.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            CategoryRepository = new CategoryRepository(_context);
            FoodTypeRepository = new FoodTypeRepository(_context);
            MenuItemRepository = new MenuItemRepository(_context);
            ApplicationUserRepository = new ApplicationUserRepository(_context);
        }

        public ICategoryRepository CategoryRepository { get; private set; }

        public IFoodTypeRepository FoodTypeRepository { get; private set; }

        public IMenuItemRepository MenuItemRepository { get; private set; }

        public IApplicationUserRepository ApplicationUserRepository { get; private set; }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}

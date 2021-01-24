using System;
using System.Collections.Generic;
using System.Text;
using TasteStore.DataAccess.Data.Repository.Interfaces;

namespace TasteStore.DataAccess.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            CategoryRepository = new CategoryRepository(_context);
        }

        public ICategoryRepository CategoryRepository { get; private set; }

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

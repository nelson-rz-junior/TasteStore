using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models;
using System.Linq;

namespace TasteStore.DataAccess.Data.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

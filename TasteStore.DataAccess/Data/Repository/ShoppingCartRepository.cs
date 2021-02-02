using TasteStore.DataAccess.Data.Repository.IRepository;
using TasteStore.Models;

namespace TasteStore.DataAccess.Data.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

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

        public int IncrementQuantity(ShoppingCart shoppingCart, int quantity)
        {
            shoppingCart.Quantity += quantity;
            return shoppingCart.Quantity;
        }

        public int DecrementQuantity(ShoppingCart shoppingCart, int quantity)
        {
            shoppingCart.Quantity -= quantity;
            return shoppingCart.Quantity;
        }
    }
}

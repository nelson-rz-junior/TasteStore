using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models;

namespace TasteStore.DataAccess.Data.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        int IncrementQuantity(ShoppingCart shoppingCart, int quantity);

        int DecrementQuantity(ShoppingCart shoppingCart, int quantity);
    }
}

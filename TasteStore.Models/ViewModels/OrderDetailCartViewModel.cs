using System.Collections.Generic;

namespace TasteStore.Models.ViewModels
{
    public class OrderDetailCartViewModel
    {
        public List<ShoppingCart> ShoppingCartItems { get; set; }

        public OrderHeader OrderHeader { get; set; }
    }
}

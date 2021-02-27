using System.Collections.Generic;

namespace TasteStore.Models.ViewModels
{
    public class OrderPickupViewModel
    {
        public OrderHeader OrderHeader { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }
    }
}

using System.Linq;
using TasteStore.DataAccess.Data.Repository.IRepository;
using TasteStore.Models;

namespace TasteStore.DataAccess.Data.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderDetail orderDetail)
        {
            var currentOrderDetail = _context.OrderDetails.FirstOrDefault(mi => mi.Id == orderDetail.Id);
            _context.Update(currentOrderDetail);

            _context.SaveChanges();
        }
    }
}

using System.Linq;
using TasteStore.DataAccess.Data.Repository.IRepository;
using TasteStore.Models;

namespace TasteStore.DataAccess.Data.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderHeader orderHeader)
        {
            var currentOrderHeader = _context.OrderHeaders.FirstOrDefault(mi => mi.Id == orderHeader.Id);
            _context.Update(currentOrderHeader);
            
            _context.SaveChanges();
        }
    }
}

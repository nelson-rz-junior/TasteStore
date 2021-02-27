using System.Threading.Tasks;

namespace TasteStore.DataAccess.Data.Initializer
{
    public interface IDbInitializer
    {
        Task Initialize();
    }
}

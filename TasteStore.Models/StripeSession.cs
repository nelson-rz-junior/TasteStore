using System.ComponentModel.DataAnnotations;

namespace TasteStore.Models
{
    public class StripeSession
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public int OrderId { get; set; }
    }
}

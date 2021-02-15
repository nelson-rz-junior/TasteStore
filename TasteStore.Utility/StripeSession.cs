using System.ComponentModel.DataAnnotations;

namespace TasteStore.Utility
{
    public class StripeSession
    {
        [Required]
        public string UserId { get; set; }
    }
}

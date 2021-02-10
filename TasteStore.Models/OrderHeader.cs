using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasteStore.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Order total")]
        public decimal OrderTotal { get; set; }

        [Required]
        [Display(Name = "Pick up date")]
        [DataType(DataType.Date)]
        public DateTime PickupDate { get; set; }

        [Required]
        [Display(Name = "Pick up time")]
        [DataType(DataType.Time)]
        public DateTime PickupTime { get; set; }

        public string Status { get; set; }

        public string PaymentStatus { get; set; }

        public string Comments { get; set; }

        [Display(Name = "Pick up name")]
        public string PickupName { get; set; }

        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public string TransactionId { get; set; }
    }
}

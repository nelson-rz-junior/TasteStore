using System;
using System.ComponentModel.DataAnnotations;

namespace TasteStore.Utility
{
    public class SummaryOrder
    {
        [Required(ErrorMessage = "{0} is a mandatory field")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "{0} is a mandatory field")]
        public string PickupName { get; set; }

        [Required(ErrorMessage = "{0} is a mandatory field")]
        public string PickupPhoneNumber { get; set; }

        [Required(ErrorMessage = "{0} is a mandatory field")]
        public DateTime PickupDate { get; set; }

        [Required(ErrorMessage = "{0} is a mandatory field")]
        public DateTime PickupTime { get; set; }

        public string PickupComments { get; set; }
    }
}

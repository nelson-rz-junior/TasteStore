using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TasteStore.Models
{
    public class FoodType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Food type name")]
        public string Name { get; set; }
    }
}

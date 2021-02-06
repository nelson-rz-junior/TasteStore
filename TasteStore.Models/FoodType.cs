using System.ComponentModel.DataAnnotations;

namespace TasteStore.Models
{
    public class FoodType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Background color")]
        public string BackgroundColor { get; set; }
    }
}

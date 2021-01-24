using System.ComponentModel.DataAnnotations;

namespace TasteStore.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Display order")]
        public int DisplayOrder { get; set; }
    }
}

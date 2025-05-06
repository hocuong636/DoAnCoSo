using System.ComponentModel.DataAnnotations;

namespace CT_Fas.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        // Navigation property
        public ICollection<Product> Products { get; set; }
    }
}
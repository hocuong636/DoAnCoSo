using System.ComponentModel.DataAnnotations;

namespace CT_Fas.Models
{
    public class ProductSize
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Kích thước")]
        public string Size { get; set; }

        [Required]
        [Display(Name = "Số lượng trong kho")]
        public int StockQuantity { get; set; }

        // Foreign key
        public int ProductId { get; set; }

        // Navigation property
        public Product Product { get; set; }
    }
}
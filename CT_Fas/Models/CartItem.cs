using System.ComponentModel.DataAnnotations;

namespace CT_Fas.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        
        [Required]
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        public int SizeId { get; set; }
        public ProductSize Size { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
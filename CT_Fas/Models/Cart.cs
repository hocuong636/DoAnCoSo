using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CT_Fas.Models
{
    public class Cart
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public decimal TotalAmount => CartItems?.Sum(item => item.Product.Price * item.Quantity) ?? 0;
    }
}
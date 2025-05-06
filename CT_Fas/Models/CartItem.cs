using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CT_Fas.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Display(Name = "Ngày thêm vào giỏ")]
        public DateTime DateCreated { get; set; }

        // Navigation properties
        public Cart Cart { get; set; }
        public Product Product { get; set; }

        public CartItem()
        {
            DateCreated = DateTime.Now;
        }
    }
}
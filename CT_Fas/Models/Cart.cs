using System.ComponentModel.DataAnnotations;

namespace CT_Fas.Models
{
    public class Cart
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Mã người dùng")]
        public string UserId { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public ApplicationUser User { get; set; }
        public ICollection<CartItem> CartItems { get; set; }

        public Cart()
        {
            CreatedDate = DateTime.Now;
            CartItems = new List<CartItem>();
        }
    }
}
using Microsoft.AspNetCore.Identity;

namespace CT_Fas.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public ICollection<Order> Orders { get; set; }
        public ICollection<Cart> Carts { get; set; }
        public ICollection<ProductReview> ProductReviews { get; set; }

        public ApplicationUser()
        {
            CreatedDate = DateTime.Now;
            IsActive = true;
            Orders = new List<Order>();
            Carts = new List<Cart>();
            ProductReviews = new List<ProductReview>();
        }
    }
}

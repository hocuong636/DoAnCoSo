using System;
using System.Collections.Generic;

namespace CT_Fas.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public bool IsOrdered { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
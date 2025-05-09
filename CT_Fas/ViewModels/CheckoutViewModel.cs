using System.Collections.Generic;
using CT_Fas.Models;

namespace CT_Fas.ViewModels
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}
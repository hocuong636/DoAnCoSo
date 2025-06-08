using CT_Fas.Models;
using System;
using System.Collections.Generic;

namespace CT_Fas.ViewModels
{
    public class OrderSuccessViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
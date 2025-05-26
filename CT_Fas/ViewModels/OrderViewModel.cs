using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CT_Fas.Models;

namespace CT_Fas.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }

        [Display(Name = "Ngày đặt hàng")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Tổng tiền")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Trạng thái")]
        public string Status { get; set; }

        [Display(Name = "Địa chỉ giao hàng")]
        public string ShippingAddress { get; set; }

        public List<OrderItemViewModel> Items { get; set; }

        public bool CanBeCancelled => Status == OrderStatus.Pending.ToString();
    }

    public class OrderItemViewModel
    {
        public int ProductId { get; set; }
        
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }

        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Display(Name = "Đơn giá")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Thành tiền")]
        public decimal Total { get; set; }

        public string ImageUrl { get; set; }
    }
}
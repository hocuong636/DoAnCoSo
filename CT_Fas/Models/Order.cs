using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CT_Fas.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ngày đặt hàng")]
        public DateTime OrderDate { get; set; }

        [Required]
        [Display(Name = "Tên khách hàng")]
        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Trạng thái")]
        public OrderStatus Status { get; set; }

        [Display(Name = "Tổng tiền")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        // Foreign key cho ApplicationUser
        public string UserId { get; set; }

        // Navigation properties
        public ApplicationUser User { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }

        public Order()
        {
            OrderDate = DateTime.Now;
            Status = OrderStatus.Pending;
            OrderDetails = new List<OrderDetail>();
        }
    }

    public enum OrderStatus
    {
        [Display(Name = "Chờ xác nhận")]
        Pending,
        
        [Display(Name = "Đã xác nhận")]
        Confirmed,
        
        [Display(Name = "Đang giao hàng")]
        Shipping,
        
        [Display(Name = "Đã giao hàng")]
        Delivered,
        
        [Display(Name = "Đã hủy")]
        Cancelled
    }
}
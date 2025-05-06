using System.ComponentModel.DataAnnotations;

namespace CT_Fas.Models
{
    public class ProductReview
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Tên người đánh giá")]
        public string ReviewerName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string ReviewerEmail { get; set; }

        [Required]
        [Range(1, 5)]
        [Display(Name = "Số sao")]
        public int Rating { get; set; }

        [Required]
        [Display(Name = "Nội dung đánh giá")]
        public string Comment { get; set; }

        [Display(Name = "Ngày đánh giá")]
        public DateTime ReviewDate { get; set; }

        [Display(Name = "Trạng thái")]
        public ReviewStatus Status { get; set; }

        // Navigation properties
        public Product Product { get; set; }
        public ApplicationUser User { get; set; }

        public ProductReview()
        {
            ReviewDate = DateTime.Now;
            Status = ReviewStatus.Pending;
        }
    }

    public enum ReviewStatus
    {
        [Display(Name = "Chờ duyệt")]
        Pending,

        [Display(Name = "Đã duyệt")]
        Approved,

        [Display(Name = "Đã từ chối")]
        Rejected
    }
}
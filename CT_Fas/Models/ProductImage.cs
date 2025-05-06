using System.ComponentModel.DataAnnotations;

namespace CT_Fas.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Display(Name = "Đường dẫn hình ảnh")]
        public string ImageUrl { get; set; }

        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Display(Name = "Alt Text")]
        public string AltText { get; set; }

        [Display(Name = "Thứ tự hiển thị")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Ảnh chính")]
        public bool IsMainImage { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; }

        // Navigation property
        public Product Product { get; set; }

        public ProductImage()
        {
            CreatedDate = DateTime.Now;
            DisplayOrder = 999; // Giá trị mặc định cho thứ tự hiển thị
            IsMainImage = false;
        }
    }
}
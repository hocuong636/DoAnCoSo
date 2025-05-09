using CT_Fas.Models;
using System.ComponentModel.DataAnnotations;

namespace CT_Fas.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Giá không được để trống")]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Display(Name = "Thương hiệu")]
        public string Brand { get; set; }

        [Display(Name = "Danh mục")]
        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryId { get; set; }

        public string ImageUrl { get; set; }

        [Display(Name = "Sizes và số lượng")]
        public List<ProductSizeViewModel> ProductSizes { get; set; } = new List<ProductSizeViewModel>();

        public IFormFile MainImage { get; set; }
        public List<IFormFile> AdditionalImages { get; set; }

        // Thêm thuộc tính cho ảnh bổ sung
        public List<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }

    public class ProductSizeViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn size")]
        [Display(Name = "Size")]
        public string Size { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Display(Name = "Số lượng")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không được âm")]
        public int StockQuantity { get; set; }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CT_Fas.Models;
using CT_Fas.ViewModels;

namespace CT_Fas.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Product
        public async Task<IActionResult> Index(string category, decimal? minPrice, decimal? maxPrice, string stockStatus)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductSizes)
                .AsQueryable();

            // Lọc theo danh mục
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.Name == category);
            }

            // Lọc theo khoảng giá
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Lọc theo tình trạng tồn kho
            if (!string.IsNullOrEmpty(stockStatus))
            {
                switch (stockStatus)
                {
                    case "out":
                        query = query.Where(p => !p.ProductSizes.Any() || p.ProductSizes.Sum(ps => ps.StockQuantity) == 0);
                        break;
                    case "low":
                        query = query.Where(p => p.ProductSizes.Sum(ps => ps.StockQuantity) > 0 && p.ProductSizes.Sum(ps => ps.StockQuantity) <= 10);
                        break;
                    case "in":
                        query = query.Where(p => p.ProductSizes.Sum(ps => ps.StockQuantity) > 10);
                        break;
                }
            }

            var products = await query.ToListAsync();

            // Chuẩn bị dữ liệu cho dropdowns
            ViewBag.Categories = await _context.Categories.Select(c => c.Name).Distinct().ToListAsync();
            ViewBag.SelectedCategory = category;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.StockStatus = stockStatus;

            return View(products);
        }

        // GET: Admin/Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Product/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            var viewModel = new ProductViewModel
            {
                ProductSizes = new List<ProductSizeViewModel>
                {
                    new ProductSizeViewModel() // Tạo một dòng trống mặc định
                }
            };
            return View(viewModel);
        }

        // POST: Admin/Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = viewModel.Name,
                    Price = viewModel.Price,
                    Description = viewModel.Description,
                    Brand = viewModel.Brand,
                    CategoryId = viewModel.CategoryId
                };

                // Xử lý ảnh chính
                if (viewModel.MainImage != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.MainImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.MainImage.CopyToAsync(fileStream);
                    }

                    product.ImageUrl = "/images/products/" + uniqueFileName;
                }

                // Thêm sản phẩm vào database
                _context.Add(product);
                await _context.SaveChangesAsync();

                // Thêm sizes và số lượng
                foreach (var sizeVM in viewModel.ProductSizes.Where(ps => !string.IsNullOrEmpty(ps.Size)))
                {
                    var productSize = new ProductSize
                    {
                        ProductId = product.Id,
                        Size = sizeVM.Size,
                        StockQuantity = sizeVM.StockQuantity
                    };
                    _context.ProductSizes.Add(productSize);
                }

                // Xử lý các ảnh bổ sung
                if (viewModel.AdditionalImages != null && viewModel.AdditionalImages.Count > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");

                    foreach (var image in viewModel.AdditionalImages)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                        }

                        var productImage = new ProductImage
                        {
                            ProductId = product.Id,
                            ImageUrl = "/images/products/" + uniqueFileName
                        };

                        _context.ProductImages.Add(productImage);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Sản phẩm đã được tạo thành công";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", viewModel.CategoryId);
            return View(viewModel);
        }

        // GET: Admin/Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Brand = product.Brand,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl,
                ProductSizes = product.ProductSizes.Select(ps => new ProductSizeViewModel
                {
                    Size = ps.Size,
                    StockQuantity = ps.StockQuantity
                }).ToList()
            };

            if (!viewModel.ProductSizes.Any())
            {
                viewModel.ProductSizes.Add(new ProductSizeViewModel()); // Thêm một dòng trống nếu không có sizes
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(viewModel);
        }

        // POST: Admin/Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var product = await _context.Products
                        .Include(p => p.ProductSizes)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (product == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin cơ bản
                    product.Name = viewModel.Name;
                    product.Price = viewModel.Price;
                    product.Description = viewModel.Description;
                    product.Brand = viewModel.Brand;
                    product.CategoryId = viewModel.CategoryId;

                    // Xử lý ảnh chính nếu có
                    if (viewModel.MainImage != null)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.MainImage.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await viewModel.MainImage.CopyToAsync(fileStream);
                        }

                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(product.ImageUrl))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        product.ImageUrl = "/images/products/" + uniqueFileName;
                    }
                    // Xóa các CartItem tham chiếu đến ProductSize trước
                    var cartItems = await _context.CartItems
                        .Where(ci => ci.ProductId == product.Id)
                        .ToListAsync();
                    _context.CartItems.RemoveRange(cartItems);

                    // Cập nhật sizes và số lượng
                    _context.ProductSizes.RemoveRange(product.ProductSizes);
                    foreach (var sizeVM in viewModel.ProductSizes.Where(ps => !string.IsNullOrEmpty(ps.Size)))
                    {
                        var productSize = new ProductSize
                        {
                            ProductId = product.Id,
                            Size = sizeVM.Size,
                            StockQuantity = sizeVM.StockQuantity
                        };
                        _context.ProductSizes.Add(productSize);
                    }

                    // Xử lý ảnh bổ sung
                    if (viewModel.AdditionalImages != null && viewModel.AdditionalImages.Count > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");

                        foreach (var image in viewModel.AdditionalImages)
                        {
                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(fileStream);
                            }

                            var productImage = new ProductImage
                            {
                                ProductId = product.Id,
                                ImageUrl = "/images/products/" + uniqueFileName
                            };

                            _context.ProductImages.Add(productImage);
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Sản phẩm đã được cập nhật thành công";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", viewModel.CategoryId);
            return View(viewModel);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product != null)
            {
                // Xóa ảnh chính
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var mainImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(mainImagePath))
                    {
                        System.IO.File.Delete(mainImagePath);
                    }
                }

                // Xóa ảnh bổ sung
                foreach (var image in product.ProductImages)
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Sản phẩm đã được xóa thành công";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage != null)
            {
                // Xóa file ảnh
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, productImage.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                _context.ProductImages.Remove(productImage);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMultiple([FromBody] int[] ids)
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductSizes)
                    .Where(p => ids.Contains(p.Id))
                    .ToListAsync();

                foreach (var product in products)
                {
                    // Xóa ảnh chính
                    if (!string.IsNullOrEmpty(product.ImageUrl))
                    {
                        var mainImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(mainImagePath))
                        {
                            System.IO.File.Delete(mainImagePath);
                        }
                    }

                    // Xóa ảnh bổ sung
                    foreach (var image in product.ProductImages)
                    {
                        var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                }

                _context.Products.RemoveRange(products);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa thành công " + products.Count + " sản phẩm";

                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
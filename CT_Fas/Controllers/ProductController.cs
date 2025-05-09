using System.Threading.Tasks;
using CT_Fas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CT_Fas.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes)
                .Where(p => p.IsActive)
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductReviews)
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public async Task<IActionResult> Category(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes)
                .Where(p => p.CategoryId == id && p.IsActive)
                .ToListAsync();

            ViewBag.Category = category;
            return View(products);
        }

        public async Task<IActionResult> Search(string searchString)
        {
            var products = from p in _context.Products
                          select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString) 
                                       || p.Description.Contains(searchString));
            }

            return View(await products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes)
                .Where(p => p.IsActive)
                .ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int sizeId, int quantity)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng" });
            }

            try
            {
                Console.WriteLine($"Thêm vào giỏ - ProductId: {productId}, SizeId: {sizeId}, Quantity: {quantity}");
                
                // Kiểm tra sản phẩm và size tồn tại
                var productSize = await _context.ProductSizes
                    .Include(ps => ps.Product)
                    .FirstOrDefaultAsync(ps => ps.Id == sizeId && ps.ProductId == productId);
                
                if (productSize == null)
                {
                    Console.WriteLine("Không tìm thấy ProductSize");
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm hoặc kích thước" });
                }

                Console.WriteLine($"Tìm thấy ProductSize - Product: {productSize.Product.Name}, Size: {productSize.Size}");

                if (productSize.StockQuantity < quantity)
                {
                    return Json(new { success = false, message = "Số lượng sản phẩm trong kho không đủ" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                var cart = await _context.Carts
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId
                    };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Đã tạo giỏ hàng mới - CartId: {cart.Id}");
                }
                else
                {
                    Console.WriteLine($"Đã tìm thấy giỏ hàng - CartId: {cart.Id}");
                }

                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartId == cart.Id &&
                                            ci.ProductId == productId &&
                                            ci.SizeId == sizeId);

                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                }
                else
                {
                    cartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = productId,
                        SizeId = sizeId,
                        Quantity = quantity
                    };
                    _context.CartItems.Add(cartItem);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã thêm sản phẩm vào giỏ hàng", redirect = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm vào giỏ hàng: {ex.Message}");
                Console.WriteLine($"Chi tiết: {ex.StackTrace}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { items = new List<object>(), total = 0 });
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                            .ThenInclude(p => p.ProductImages)
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Size)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    return Json(new { items = new List<object>(), total = 0 });
                }

                var cartItems = cart.CartItems.Select(ci => new
                {
                    id = ci.Id,
                    productId = ci.ProductId,
                    productName = ci.Product.Name,
                    imageUrl = ci.Product.ProductImages.FirstOrDefault(pi => pi.IsMainImage)?.ImageUrl
                              ?? ci.Product.ProductImages.FirstOrDefault()?.ImageUrl,
                    size = ci.Size.Size,
                    price = ci.Product.Price,
                    quantity = ci.Quantity,
                    total = ci.Product.Price * ci.Quantity
                }).ToList();

                var total = cartItems.Sum(item => item.total);

                return Json(new { items = cartItems, total = total });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy giỏ hàng: {ex.Message}");
                return Json(new { items = new List<object>(), total = 0 });
            }
        }
    }
}
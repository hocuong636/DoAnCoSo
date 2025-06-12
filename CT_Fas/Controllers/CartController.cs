using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CT_Fas.Models;
using CT_Fas.ViewModels;
using CT_Fas.Services;

namespace CT_Fas.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public CartController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return View(new List<CartItem>());
            }

            var cartItems = await _context.CartItems
                .Include(c => c.Cart)
                .Include(c => c.Product)
                .Include(c => c.Size)
                .Where(c => c.Cart.UserId == user.Id && !c.Cart.IsOrdered)
                .ToListAsync();
            return View(cartItems);
        }

        public async Task<IActionResult> GetCartItems()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return PartialView("_CartPartial", new List<CartItem>());
            }

            var cartItems = await _context.CartItems
                .Include(c => c.Cart)
                .Include(c => c.Product)
                .Include(c => c.Size)
                .Where(c => c.Cart.UserId == user.Id && !c.Cart.IsOrdered)
                .ToListAsync();
            return PartialView("_CartPartial", cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity, int sizeId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Bạn cần đăng nhập để thêm sản phẩm vào giỏ hàng" });
                }

                // Kiểm tra sản phẩm tồn tại
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại" });
                }

                // Kiểm tra size tồn tại
                var size = await _context.ProductSizes.FindAsync(sizeId);
                if (size == null)
                {
                    return Json(new { success = false, message = "Size không hợp lệ" });
                }

                // Tìm giỏ hàng chưa đặt hàng của user

                var cart = await _context.Carts
                    .FirstOrDefaultAsync(c => c.UserId == user.Id && !c.IsOrdered);
                // Nếu không có giỏ hàng hoặc giỏ hàng đã đặt -> tạo giỏ hàng mới
                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = user.Id,
                        IsOrdered = false,
                        CreatedAt = DateTime.Now
                    };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartId == cart.Id &&
                                             ci.ProductId == productId &&
                                             ci.SizeId == sizeId);

                if (cartItem != null)
                {
                    // Cập nhật số lượng nếu sản phẩm đã có trong giỏ hàng
                    cartItem.Quantity += quantity;
                }
                else
                {
                    // Thêm sản phẩm mới vào giỏ hàng
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
                
                var currentCartItemCount = await _context.CartItems
                    .Where(ci => ci.Cart.UserId == user.Id && !ci.Cart.IsOrdered)
                    .SumAsync(ci => ci.Quantity);

                return Json(new {
                    success = true,
                    message = "Đã thêm sản phẩm vào giỏ hàng",
                    cartItemCount = currentCartItemCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Bạn cần đăng nhập để cập nhật giỏ hàng" });
                }

                var cartItem = await _context.CartItems
                    .Include(c => c.Cart)
                    .FirstOrDefaultAsync(c => c.Id == id && c.Cart.UserId == user.Id && !c.Cart.IsOrdered);

                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng" });
                }

                if (quantity < 1)
                {
                    return Json(new { success = false, message = "Số lượng không hợp lệ" });
                }

                cartItem.Quantity = quantity;
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật số lượng" });
            }
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            // TODO: Sẽ cập nhật để xóa sản phẩm khỏi cart
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string CustomerName, string Email, string Phone, string Address)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để đặt hàng";
                return RedirectToAction(nameof(Checkout));
            }

            var cartItems = await _context.CartItems
                .Include(c => c.Cart)
                .Include(c => c.Product)
                .Include(c => c.Size)
                .Where(c => c.Cart.UserId == user.Id && !c.Cart.IsOrdered)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống";
                return RedirectToAction(nameof(Checkout));
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var order = new Order
                    {
                        UserId = user.Id,
                        CustomerName = CustomerName,
                        Email = Email,
                        Phone = Phone,
                        Address = Address,
                        TotalAmount = cartItems.Sum(i => i.Product.Price * i.Quantity),
                        Status = OrderStatus.Pending
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    foreach (var item in cartItems)
                    {
                        var orderDetail = new OrderDetail
                        {
                            Order = order,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            UnitPrice = item.Product.Price,
                            Size = item.Size.Size,
                            TotalPrice = item.Product.Price * item.Quantity
                        };
                        item.Size.StockQuantity -= item.Quantity;
                        _context.OrderDetails.Add(orderDetail);
                    }

                    if (cartItems.Any())
                    {
                        // Đánh dấu giỏ hàng đã được đặt và xóa cartItems
                        var cartId = cartItems.First().CartId;
                        var cart = await _context.Carts.FindAsync(cartId);
                        if (cart != null)
                        {
                            _context.Carts.Update(cart);
                            
                            // Xóa tất cả CartItems
                            _context.CartItems.RemoveRange(cartItems);
                        }
                    }
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    // Gửi email xác nhận đơn hàng
                    await _emailService.SendOrderConfirmationEmailAsync(
                        order.Email,
                        order.CustomerName,
                        order.OrderId.ToString()
                    );
                    return RedirectToAction(nameof(OrderSuccess), new { orderId = order.OrderId });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = $"Có lỗi xảy ra khi đặt hàng: {ex.Message}";
                    return RedirectToAction(nameof(Checkout));
                }
            }
        }

        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để đặt hàng";
                return RedirectToAction(nameof(Index));
            }

            var cartItems = await _context.CartItems
                .Include(c => c.Cart)
                .Include(c => c.Product)
                .Include(c => c.Size)
                .Where(c => c.Cart.UserId == user.Id && !c.Cart.IsOrdered)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new CheckoutViewModel
            {
                CartItems = cartItems,
                CustomerName = user.FullName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Address = user.Address
            };

            return View(viewModel);
        }

        public async Task<IActionResult> OrderSuccess(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }
            var viewModel = new OrderSuccessViewModel
            {
                OrderId = order.OrderId,
                CustomerName = order.CustomerName,
                Email = order.Email,
                Phone = order.Phone,
                Address = order.Address,
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                OrderDetails = order.OrderDetails.ToList()
            };

            return View(viewModel);
        }
    }
}
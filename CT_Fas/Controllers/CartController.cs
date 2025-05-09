using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CT_Fas.Models;
using CT_Fas.ViewModels;

namespace CT_Fas.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
        public IActionResult AddToCart(int productId, int quantity)
        {
            // TODO: Sẽ cập nhật để thêm sản phẩm vào cart
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int cartItemId, int quantity)
        {
            // TODO: Sẽ cập nhật số lượng sản phẩm trong cart
            return Json(new { success = true });
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
                TempData["Error"] = "Bạn cần đăng nhập để đặt hàng";
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
                TempData["Error"] = "Giỏ hàng của bạn đang trống";
                return RedirectToAction(nameof(Index));
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
                    TempData["Success"] = "Đặt hàng thành công!";
                    return RedirectToAction("Index", "Home");
                }
                catch
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = "Có lỗi xảy ra khi đặt hàng. Vui lòng thử lại sau.";
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để đặt hàng";
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
                TempData["Error"] = "Giỏ hàng của bạn đang trống";
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
    }
}
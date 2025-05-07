using Microsoft.AspNetCore.Mvc;
using CT_Fas.Models;

namespace CT_Fas.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult GetCartItems()
        {
            // TODO: Sẽ cập nhật để lấy cart của user hiện tại
            var cartItems = new List<CartItem>();
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
    }
}
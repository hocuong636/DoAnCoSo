using Microsoft.AspNetCore.Mvc;
using CT_Fas.Models;

namespace CT_Fas.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CartViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // TODO: Sẽ cập nhật để lấy cart của user hiện tại
            var cartItems = new List<CartItem>();
            return View(cartItems);
        }
    }
}
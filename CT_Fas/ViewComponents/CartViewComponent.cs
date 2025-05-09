using System.Security.Claims;
using System.Threading.Tasks;
using System;
using CT_Fas.Models;
using CT_Fas.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            var model = new CartViewModel();

            if (User.Identity.IsAuthenticated)
            {
                var userId = (User as ClaimsPrincipal)?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return View(model);
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                            .ThenInclude(p => p.ProductImages)
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Size)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart != null)
                {
                    model.Items = cart.CartItems.Select(ci => new CartItemViewModel
                    {
                        Id = ci.Id,
                        ProductId = ci.ProductId,
                        ProductName = ci.Product.Name,
                        ImageUrl = !string.IsNullOrEmpty(ci.Product.ImageUrl)
                                  ? ci.Product.ImageUrl
                                  : ci.Product.ProductImages.FirstOrDefault(pi => pi.IsMainImage)?.ImageUrl
                                    ?? ci.Product.ProductImages.FirstOrDefault()?.ImageUrl,
                        Size = ci.Size.Size,
                        Price = ci.Product.Price,
                        Quantity = ci.Quantity
                    }).ToList();
                }
            }

            return View(model);
        }
    }
}
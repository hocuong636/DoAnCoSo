using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CT_Fas.Models;
using CT_Fas.Services;
using Microsoft.AspNetCore.Identity;

namespace CT_Fas.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public OrderController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Order order)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                order.UserId = user.Id;
                order.OrderDate = DateTime.Now;
                order.Status = OrderStatus.Pending;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Gửi email xác nhận đơn hàng
                await _emailService.SendOrderConfirmationEmailAsync(
                    order.Email,
                    order.CustomerName,
                    order.OrderId.ToString()
                );

                TempData["SuccessMessage"] = "Đặt hàng thành công!";
                return RedirectToAction("OrderConfirmation", new { id = order.OrderId });
            }

            return View(order);
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            
            if (order == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (order.UserId != user.Id)
            {
                return Forbid();
            }

            if (order.Status != OrderStatus.Pending)
            {
                TempData["ErrorMessage"] = "Chỉ có thể hủy đơn hàng ở trạng thái chờ xác nhận!";
                return RedirectToAction("Details", new { id = order.OrderId });
            }

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();

            // Gửi email thông báo hủy đơn hàng
            await _emailService.SendOrderCancelledEmailAsync(
                order.Email,
                order.CustomerName,
                order.OrderId.ToString()
            );

            TempData["SuccessMessage"] = "Hủy đơn hàng thành công!";
            return RedirectToAction("Details", new { id = order.OrderId });
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (order.UserId != user.Id)
            {
                return Forbid();
            }

            return View(order);
        }

        // GET: Order/OrderConfirmation/5
        public async Task<IActionResult> OrderConfirmation(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (order.UserId != user.Id)
            {
                return Forbid();
            }

            return View(order);
        }
    }
}
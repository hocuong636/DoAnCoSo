using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CT_Fas.Models;

namespace ACC_Tour.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Đếm tổng số đơn hàng
            ViewBag.TotalOrders = await _context.Orders.CountAsync();

            // Tính tổng doanh thu
            ViewBag.TotalRevenue = await _context.Orders
                .Where(o => o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.TotalAmount);

            // Đếm số lượng sản phẩm
            ViewBag.TotalProducts = await _context.Products.CountAsync();

            // Đếm số lượng khách hàng
            ViewBag.TotalCustomers = await _context.Users.CountAsync();

            // Lấy 5 đơn hàng mới nhất
            ViewBag.RecentOrders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }
} 
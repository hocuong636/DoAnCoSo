using System.Threading.Tasks;
using CT_Fas.Models;

namespace CT_Fas.Services
{
    public interface IEmailService
    {
        Task SendOrderConfirmationEmailAsync(string to, string customerName, string orderNumber);
        Task SendOrderCancelledEmailAsync(string to, string customerName, string orderNumber);
        Task SendOrderStatusUpdateEmailAsync(string to, string customerName, string orderNumber, string newStatus);
    }
}
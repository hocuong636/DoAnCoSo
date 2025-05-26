using System.Threading.Tasks;
using CT_Fas.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CT_Fas.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailService(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        private async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailConfig.From));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = body;
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendOrderConfirmationEmailAsync(string to, string customerName, string orderNumber)
        {
            string subject = $"Xác nhận đơn hàng #{orderNumber}";
            string body = $@"
                <h2>Xin chào {customerName},</h2>
                <p>Cảm ơn bạn đã đặt hàng tại CT Fashion. Đơn hàng của bạn đã được xác nhận.</p>
                <p>Mã đơn hàng: <strong>{orderNumber}</strong></p>
                <p>Chúng tôi sẽ thông báo cho bạn khi đơn hàng được gửi đi.</p>
                <p>Trân trọng,<br>CT Fashion</p>";

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendOrderCancelledEmailAsync(string to, string customerName, string orderNumber)
        {
            string subject = $"Đơn hàng #{orderNumber} đã bị hủy";
            string body = $@"
                <h2>Xin chào {customerName},</h2>
                <p>Đơn hàng #{orderNumber} của bạn đã bị hủy.</p>
                <p>Nếu bạn đã thanh toán, số tiền sẽ được hoàn trả trong vòng 5-7 ngày làm việc.</p>
                <p>Nếu bạn có bất kỳ thắc mắc nào, vui lòng liên hệ với chúng tôi.</p>
                <p>Trân trọng,<br>CT Fashion</p>";

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendOrderStatusUpdateEmailAsync(string to, string customerName, string orderNumber, string newStatus)
        {
            string subject = $"Cập nhật trạng thái đơn hàng #{orderNumber}";
            string body = $@"
                <h2>Xin chào {customerName},</h2>
                <p>Đơn hàng #{orderNumber} của bạn đã được cập nhật trạng thái mới: <strong>{newStatus}</strong></p>
                <p>Bạn có thể theo dõi đơn hàng của mình trên website của chúng tôi.</p>
                <p>Trân trọng,<br>CT Fashion</p>";

            await SendEmailAsync(to, subject, body);
        }
    }
}
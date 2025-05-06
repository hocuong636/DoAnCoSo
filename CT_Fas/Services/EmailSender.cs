using Microsoft.AspNetCore.Identity.UI.Services;

namespace CT_Fas.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // TODO: Implement real email sending logic here
            return Task.CompletedTask;
        }
    }
}
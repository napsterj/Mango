using Mango.Services.Email.API.Models.Dto;
using Mango.Services.EmailAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly DbContextOptions<EmailDbContext> _emailDbContext;

        public EmailService(DbContextOptions<EmailDbContext> emailDbContext)
        {
            _emailDbContext = emailDbContext;
        }

        public async Task EmailCart(CartDto cartDto)
        {
            
            StringBuilder message = new StringBuilder();
            
            message.AppendLine("<br>Cart Email Requested ");
            message.AppendLine($"<br>Total {cartDto.CartHeaderDto.CartTotal}");
            message.Append("<br>");
            message.Append("<ul>");

            foreach(var cartDetail in cartDto.CartDetailsDto)
            {
                message.Append("<li>");
                message.Append(cartDetail.ProductDto.Name + " x " + cartDetail.ProductDto.Count);
                message.Append("</li>");
            }

            message.Append("</ul>");

            await SendEmail(message.ToString(), cartDto.CartHeaderDto.Email);
        }

        public async Task<bool> SendEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLogger = new()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message
                };

                await using var db = new EmailDbContext(_emailDbContext);
                await db.EmailLoggers.AddAsync(emailLogger);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }
    }
}

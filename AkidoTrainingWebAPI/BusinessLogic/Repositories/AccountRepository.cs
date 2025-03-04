using AkidoTrainingWebAPI.DataAccess.Data;
using AkidoTrainingWebAPI.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using AkidoTrainingWebAPI.BusinessLogic.DTOs.AccountsDTO;

namespace AkidoTrainingWebAPI.BusinessLogic.Repositories
{
    public class AccountRepository
    {
        private readonly AkidoTrainingWebAPIContext _context;
        public AccountRepository(AkidoTrainingWebAPIContext context)
        {
            _context = context;
        }
        
        public async Task<ActionResult<IEnumerable<Accounts>>> GetAccounts()
        {
            return await _context.Accounts.ToListAsync();
        }
        
        public async Task<Accounts> GetAccountsByIdAsync(int id)
        {
            return await _context.Accounts.FindAsync(id);
        }

        public async Task<Accounts> GetAccountsByPhoneAsync(string? phone)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.PhoneNumber == phone);
        }

        public async Task<bool> IsPhoneExistsAsync(string? phone)
        {
            return await _context.Accounts.AnyAsync(a => a.PhoneNumber == phone);
        }

        public async Task AddAccountsAsync(AccountsDTO account)
        {
            var newAccount = new Accounts
            {
                Id = account.Id,
                Name = account.Name,
                Password = account.Password,
                PhoneNumber = account.PhoneNumber,
                Role = account.Role,
                Level = account.Level,
                Belt = account.Belt,
                ImagePath = account.ImagePath
            };
            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateUserAsync(Accounts account)
        {
            _context.Entry(account).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAccountAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }

        public bool IsAccountExisting(int id)
        {
            return _context.Accounts.Any(a => a.Id == id);
        }

        public void SendEmail(string receiveEmail, string subject, string body, bool htmlBody = false)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("conglinhoct2003@gmail.com"));
            email.To.Add(MailboxAddress.Parse(receiveEmail));
            email.Subject = subject;

            if (htmlBody)
            {
                email.Body = new TextPart(TextFormat.Html) { Text = body };
            }
            else
            {
                email.Body = new TextPart(TextFormat.Plain) { Text = body };
            }

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("conglinhoct2003@gmail.com", "adcyvzgxcdyzcrwc");
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using AkidoTrainingWebAPI.BusinessLogic.Repositories;
using AkidoTrainingWebAPI.BusinessLogic.DTOs.AccountsDTO;

namespace AkidoTrainingWebAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly AccountRepository _repository;
        public AccountsController(AccountRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Accounts
        [HttpGet]
        public async Task<ActionResult> GetAccounts()
        {
            var accounts = await _repository.GetAccounts();
            return Ok(accounts);
        }

        // GET: api/Accounts/ViewAccount/5
        [HttpGet("ViewAccounts/{id}")]
        public async Task<ActionResult> GetAccounts(int id)
        {
            var accounts = await _repository.GetAccountsByIdAsync(id);

            if (accounts == null)
            {
                return NotFound();
            }

            return Ok(accounts);
        }

        // PUT: api/Accounts/Update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateAccount(int id, AccountsDTOPut accounts)
        {
            var accountToUpdate = await _repository.GetAccountsByIdAsync(id);
            if (accountToUpdate == null)
            {
                return NotFound();
            }

            if (await _repository.IsPhoneExistsAsync(accounts.PhoneNumber) && accountToUpdate.PhoneNumber != accounts.PhoneNumber)
            {
                return Conflict("This phone number is already used for other accounts");
            }

            if (accounts.Role.ToString() == "Admin" || accounts.Role.ToString() == "User")
            {
                accountToUpdate.PhoneNumber = accounts.PhoneNumber;
                accountToUpdate.Role = accounts.Role;
                accountToUpdate.Name = accounts.Name;
                accountToUpdate.Password = accounts.Password;
                accountToUpdate.Level = accounts.Level;
                accountToUpdate.Belt = accounts.Belt;

                await _repository.UpdateUserAsync(accountToUpdate);
                return NoContent();
            }
            else
            {
                return BadRequest("Must be Admin or User");
            }
        }

        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccounts(int id)
        {
            if (!_repository.IsAccountExisting(id))
            {
                return NotFound();
            }
            var accountToDelete = await _repository.GetAccountsByIdAsync(id);
            if (accountToDelete.ImagePath != "Default.png")
            {
                DeleteAvatar(accountToDelete.ImagePath);
            }
            await _repository.DeleteAccountAsync(id);
            return NoContent();
        }

        // POST: api/Accounts/login
        [HttpPost("Login")]
        public async Task<ActionResult> Login(AccountsDTOLogin login)
        {
            var existingPhoneNumber = await _repository.GetAccountsByPhoneAsync(login.PhoneNumber);

            if (existingPhoneNumber == null || existingPhoneNumber.Password != login.Password)
            {
                return NotFound("Số điện thoại hoặc mật khẩu không hợp lệ.");
            }

            return Ok(existingPhoneNumber.Role);
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(AccountsDTORegister account)
        {
            if (await _repository.IsPhoneExistsAsync(account.PhoneNumber))
            {
                return Conflict("Đã có tài khoản khác sử dụng số điện thoại này.");
            }
            var newAccount = new AccountsDTO
            {
                Name = account.Name,
                Password = account.Password,
                PhoneNumber = account.PhoneNumber,
                Role = "User",
                Level = 5,
                Belt = "Black",
                ImagePath = "Default.png"
            };
            await _repository.AddAccountsAsync(newAccount);
            return CreatedAtAction(nameof(GetAccounts), new { id = newAccount.Id}, newAccount);
        }

        [HttpGet("Image/{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            var account = await _repository.GetAccountsByIdAsync(id);
            if (account == null || string.IsNullOrEmpty(account.ImagePath))
            {
                return NotFound("User or image not found.");
            }

            var uploadsDirectory = Path.Combine("C:", "API", "Avatar");
            var imagePath = Path.Combine(uploadsDirectory, account.ImagePath);

            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound("Image not found.");
            }
            try
            {
                var memory = new MemoryStream();
                using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, "image/jpeg");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving image: {ex.Message}");
            }
        }

        [HttpPut("UploadImages/{id}")]
        public async Task<IActionResult> EditProfilePicture(int id, IFormFile avatar)
        {
            var accountToUpdate = await _repository.GetAccountsByIdAsync(id);

            if (accountToUpdate == null)
            {
                return NotFound("User not found");
            }

            if (accountToUpdate.ImagePath != "Default.png")
            {
                DeleteAvatar(accountToUpdate.ImagePath);
            }
            accountToUpdate.ImagePath = await WriteFile(avatar, accountToUpdate.PhoneNumber);
            await _repository.UpdateUserAsync(accountToUpdate);

            return Ok(accountToUpdate.ImagePath);
        }

        private async Task<string> WriteFile(IFormFile image, string? phone)
        {
            string filename = "";
            try
            {
                var extension = Path.GetExtension(image.FileName);
                filename = phone + extension;

                var filepath = Path.Combine("C:", "API", "Avatar");
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                var exactpath = Path.Combine(filepath, filename);

                using (var stream = new FileStream(exactpath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error writing file: {ex.Message}");
            }
            return filename;
        }

        private void DeleteAvatar(string avatarPath)
        {
            try
            {
                var deleteFile = Path.Combine("C:", "API", "Avatar", avatarPath);
                if (System.IO.File.Exists(deleteFile))
                {
                    System.IO.File.Delete(deleteFile);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting avatar: {ex.Message}");
            }
        }

    }
}

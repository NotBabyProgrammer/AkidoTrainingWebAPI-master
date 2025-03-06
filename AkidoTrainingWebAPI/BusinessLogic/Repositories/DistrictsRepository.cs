using AkidoTrainingWebAPI.DataAccess.Data;
using AkidoTrainingWebAPI.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using AkidoTrainingWebAPI.BusinessLogic.DTOs.DistrictsDTO;

namespace AkidoTrainingWebAPI.BusinessLogic.Repositories
{
    public class DistrictsRepository
    {
        private readonly AkidoTrainingWebAPIContext _context;
        public DistrictsRepository(AkidoTrainingWebAPIContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<Districts>>> GetDistricts()
        {
            return await _context.Districts.ToListAsync();
        }
        public async Task<Districts> GetDistrictByIdAsync(int id)
        {
            return await _context.Districts.FindAsync(id);
        }
        public async Task UpdateDistrictAsync(Districts dis)
        {
            _context.Entry(dis).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task AddDistrictAsync(DistrictsDTOAll dis)
        {
            var newDis = new Districts
            {
                Id = dis.Id,
                Name = dis.Name
            };
            _context.Districts.Add(newDis);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDistrictAsync(Districts dis)
        {
            _context.Districts.Remove(dis);
            await _context.SaveChangesAsync();
        }

        public bool DistrictsExists(int id)
        {
            return _context.Districts.Any(e => e.Id == id);
        }
    }
}

using AkidoTrainingWebAPI.DataAccess.Data;
using AkidoTrainingWebAPI.DataAccess.Models;
using AkidoTrainingWebAPI.BusinessLogic.DTOs.AreasDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace AkidoTrainingWebAPI.BusinessLogic.Repositories
{
    public class AreasRepository
    {
        private readonly AkidoTrainingWebAPIContext _context;
        public AreasRepository(AkidoTrainingWebAPIContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<Areas>>> GetAreas()
        {
            return await _context.Areas.ToListAsync();
        }

        public async Task<Areas> GetAreaByIdAsync(int id)
        {
            return await _context.Areas.FindAsync(id);
        }

        public async Task AddAreasAsync(AreasDTO area)
        {
            var newArea = new Areas
            {
                Id = area.Id,
                Name = area.Name,
                District = area.District,
                Schedule = area.Schedule,
                Address = area.Address,
                Description = area.Description,
                ImagePath = area.ImagePath
            };
            _context.Areas.Add(newArea);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAreaAsync(Areas area)
        {
            _context.Entry(area).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAreaAsync(Areas area)
        {
            _context.Areas.Remove(area);
            await _context.SaveChangesAsync();
        }

        public bool AreasExists(int id)
        {
            return _context.Areas.Any(e => e.Id == id);
        }
    }
}

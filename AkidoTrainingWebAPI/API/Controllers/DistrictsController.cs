using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AkidoTrainingWebAPI.BusinessLogic.DTOs.DistrictsDTO;
using AkidoTrainingWebAPI.BusinessLogic.Repositories;
using AkidoTrainingWebAPI.BusinessLogic.DTOs.AccountsDTO;
using AkidoTrainingWebAPI.DataAccess.Models;

namespace AkidoTrainingWebAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistrictsController : ControllerBase
    {
        private readonly DistrictsRepository _repository;

        public DistrictsController( DistrictsRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Districts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Districts>>> GetDistricts()
        {
            var districts = await _repository.GetDistricts();
            return districts;
        }

        // GET: api/Districts/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetDistricts(int id)
        {
            var districts = await _repository.GetDistrictByIdAsync(id);

            if (districts == null)
            {
                return NotFound();
            }

            return Ok(districts);
        }

        // PUT: api/Districts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDistricts(int id,DistrictsDTO districts)
        {
            var districtsToUpdate = await _repository.GetDistrictByIdAsync(id);
            if (districtsToUpdate == null)
            {
                return BadRequest();
            }
            districtsToUpdate.Name = districts.Name;
            try
            {
                await _repository.UpdateDistrictAsync(districtsToUpdate);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_repository.DistrictsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Districts
        [HttpPost]
        public async Task<ActionResult> PostDistricts(DistrictsDTO districts)
        {
            var newDis = new DistrictsDTOAll
            {
                Name = districts.Name
            };
            await _repository.AddDistrictAsync(newDis);
            return CreatedAtAction(nameof(GetDistricts), new { id = newDis.Id }, newDis);
        }

        // DELETE: api/Districts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDistricts(int id)
        {
            var districts = await _repository.GetDistrictByIdAsync(id);
            if (districts == null)
            {
                return NotFound();
            }

            await _repository.DeleteDistrictAsync(districts);
            return NoContent();
        }
    }
}

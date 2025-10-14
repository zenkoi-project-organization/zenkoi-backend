using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VarietyController : BaseAPIController
    {
        private readonly IVarietyService _varietyService;

        public VarietyController(IVarietyService varietyService)
        {
            _varietyService = varietyService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var varieties = await _varietyService.GetAllAsync();
            return Ok(varieties);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var variety = await _varietyService.GetByIdAsync(id);
            if (variety == null)
                return NotFound(new { message = "Variety not found." });

            return Ok(variety);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VarietyRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _varietyService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] VarietyRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _varietyService.UpdateAsync(id, dto);
            if (!success)
                return NotFound(new { message = "Variety not found or update failed." });

            return Ok(new { message = "Variety updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _varietyService.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = "Variety not found or delete failed." });

            return Ok(new { message = "Variety deleted successfully." });
        }
    }
}

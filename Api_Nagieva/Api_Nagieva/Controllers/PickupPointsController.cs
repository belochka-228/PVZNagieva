using Api_Nagieva.DTO;
using Api_Nagieva.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api_Nagieva.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PickupPointsController : ControllerBase
    {
        private readonly PickupPointService _service;

        public PickupPointsController(PickupPointService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreatePickupPointRequest request)
        {
            return Ok(await _service.CreateAsync(request));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CreatePickupPointRequest request)
        {
            var success = await _service.UpdateAsync(id, request);
            if (!success) return NotFound();
            return Ok("ПВЗ обновлен");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok("ПВЗ удален");
        }
    }
}

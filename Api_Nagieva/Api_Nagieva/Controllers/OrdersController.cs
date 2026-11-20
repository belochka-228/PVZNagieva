using Api_Nagieva.DTO;
using Api_Nagieva.Models;
using Api_Nagieva.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_Nagieva.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetUserId()
        {
            int.TryParse(User.FindFirst("sub")?.Value, out int userId);
            return userId;
        }
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderRequest request)
        {
            try
            {
                var order = await _orderService.CreateOrderAsync(GetUserId(), request);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrders([FromQuery] bool newestFirst = true)
        {
            var orders = await _orderService.GetUserOrdersAsync(GetUserId(), newestFirst);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(GetUserId(), id);
            if (order == null) return NotFound("Заказ не найден");
            return Ok(order);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus)
        {
            var success = await _orderService.UpdateStatusAsync(id, newStatus);
            if (!success) return BadRequest("Не удалось изменить статус (заказ или статус не найден)");
            return Ok("Статус обновлен");
        }

        [HttpGet("statuses")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStatuses()
        {
            var statuses = await _orderService.GetStatusesAsync();
            return Ok(statuses);
        }
    }
}

using Api_Nagieva.DTO;
using Api_Nagieva.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api_Nagieva.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var success = await _authService.RegisterAsync(request);
        if (!success) 
            return BadRequest("Логин занят или роль не найдена");
        return Ok (new 
        { 
            message = "Пользователь успешно зарегистрирован" 
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        if (response == null) 
            return Unauthorized("Неверный логин или пароль");
        return Ok(response);
    }
}
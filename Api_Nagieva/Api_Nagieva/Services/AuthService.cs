using Api_Nagieva.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api_Nagieva.DTO;

using LoginRequest = Api_Nagieva.DTO.LoginRequest;
using RegisterRequest = Api_Nagieva.DTO.RegisterRequest;

namespace Api_Nagieva.Services
{
    public class AuthService
    {
        private readonly MarketplaceDBContext _context;
        private readonly PasswordHasher<User> _hasher;
        private readonly IConfiguration _config;
        

        public AuthService(MarketplaceDBContext context, PasswordHasher<User> hasher, IConfiguration config)
        {
            _context = context;
            _hasher = hasher;
            _config = config;
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Login == request.Login))
                return false;

            var user = new User { Login = request.Login };
            user.PasswordHash = _hasher.HashPassword(user, request.Password);
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (userRole == null) 
                return false;
            user.RoleId = userRole.Id;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Login == request.Login);

            if (user == null) return null;

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed) return null;

            var token = CreateToken(user);
            return new AuthResponse(token, user.Role.Name, user.Login);
        }

        private string CreateToken(User user)
        {
            var claims = new[]
            {
            new Claim("sub", user.Id.ToString()),
            new Claim("name", user.Login),
            new Claim("role", user.Role.Name)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    } }
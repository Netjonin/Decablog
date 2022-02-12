using DecaBlog.Models;
using DecaBlog.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DecaBlog.Services.Implementations
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        public JwtService(IConfiguration configuration)
        {
            _config = configuration;
        }

        public string GenerateJWTToken(User user, IEnumerable<string> userRoles)
        {
            //Adding user claims
            var Claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            };
            foreach (var role in userRoles)
                Claims.Add(new Claim(ClaimTypes.Role, role));
            //Set up system security
            var SymmetricSecurity = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JWT:Key").Value));
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(Claims),
                Expires = DateTime.Today.AddDays(1),
                SigningCredentials = new SigningCredentials(SymmetricSecurity, SecurityAlgorithms.HmacSha256)
            };
            //Create token
            var SecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = SecurityTokenHandler.CreateToken(securityTokenDescriptor);
            return SecurityTokenHandler.WriteToken(token);
        }

        public string GenerateInvitationToken(string userId, string email)
        {
            var Claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email)
            };
            var SymmetricSecurity = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JWT:Key").Value));
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(Claims),
                Expires = DateTime.Today.AddDays(1),
                SigningCredentials = new SigningCredentials(SymmetricSecurity, SecurityAlgorithms.HmacSha256)
            };
            var SecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = SecurityTokenHandler.CreateToken(securityTokenDescriptor);
            var returnedToken = "decablog-" + SecurityTokenHandler.WriteToken(token);
            return returnedToken;
        }
    }
}

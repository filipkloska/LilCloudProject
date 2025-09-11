using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LilCloudServerConsole.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly ILogger<JwtTokenService> _logger;

        public JwtTokenService(IConfiguration config, ILogger<JwtTokenService> logger)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            _tokenHandler = new JwtSecurityTokenHandler();
            _logger = logger;
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User"),
                new Claim(ClaimTypes.Name, user.Name.ToString())
            };

            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],       
                audience: _config["Jwt:Audience"], 
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return _tokenHandler.WriteToken(token);
        }
        //return a usersessionclass with these three fields
        public (int? userId, bool? isAdmin, string? name) ValidateToken(string token)
        {
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = _key,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = _tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = principal.FindFirst(ClaimTypes.Role)?.Value;
                var userNameClaim = principal.FindFirst(ClaimTypes.Name)?.Value;

                //there has to be a better way than this dogshit
                int? userId = userIdClaim != null ? int.Parse(userIdClaim) : null;
                if (userIdClaim == null)
                {
                    _logger.LogError("useridclaim null");
                }
                string? username = userNameClaim != null ? userNameClaim.ToString() : null;
                if (userNameClaim == null)
                {
                    _logger.LogError("usernameclaim null");
                }
                bool isAdmin = false;
                if(roleClaim != null && roleClaim == "Admin")
                {
                    isAdmin = true;
                }

                return (userId, isAdmin, username);
            }
            catch
            {
                return (null, false, null);
            }
        }
    }
}

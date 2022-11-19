using ADMIN.Getways.Interface;
using ADMIN.Models.CustomModels.Login;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ADMIN.Controllers
{
    public class LoginController : Controller
    {
        private readonly IResponseGetaway _responseGetaway;
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration, IResponseGetaway responseGetaway)
        {
            _configuration = configuration;
            _responseGetaway = responseGetaway;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<JsonResult> Login([FromBody] LoginModel model)
        {
            var responseModel = new LoginResponseModel();
            if (!ModelState.IsValid)
            {
                responseModel.message = "Model Is Invalid";
                return Json(new { status = 200, responseModel = responseModel });
            }

            var response = _responseGetaway.SendTAsync(model, "api/Login/authenticate", HttpMethod.Post).Result;

            var tokens = JsonConvert.DeserializeObject<JWTModel>(response);

            if (!string.IsNullOrEmpty(tokens.Token) && !string.IsNullOrEmpty(tokens.RefreshToken))
                responseModel.isAuth = true;
            else
                responseModel.message = "Wrong Password or Username";

            var principal = GetPrincipalFromExpiredToken(tokens.Token);
            var username = principal.Identity?.Name;

			foreach (Claim claim in principal.Claims)
			{
               
			}


            return Json(new { status = 200, responseModel = responseModel });
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var Key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // on production make it true
                    ValidateAudience = false, // on production make it true
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["JWT:Issuer"],
                    ValidAudience = _configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Key),
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

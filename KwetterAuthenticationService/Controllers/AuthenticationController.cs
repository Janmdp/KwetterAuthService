using DataAccesLayer;
using Logic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KwetterAuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationLogic logic;

        public AuthenticationController(UserDbContext context, ITokenBuilder _tokenBuilder)
        {
            logic = new AuthenticationLogic(context, _tokenBuilder);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            if (!logic.CheckUserExist(user))
                return NotFound("User not found.");

            if (!logic.AuthenticateUser(user))
                return BadRequest("Could not authenticate user");


            return Ok(logic.GenerateToken(user));

        }

        [HttpGet("verify")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> VerifyToken()
        {
            //Check if the received token contains a username
            var username = User
                .Claims
                .SingleOrDefault();

            if (username == null)
            {
                return Unauthorized();
            }

            //Check if the username matches a existing account
            User user = new User() { Username = username.Value };

            if (!logic.CheckUserExist(user))
            {
                return Unauthorized();
            }

            return NoContent();
        }
    }
}

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
using RabbitMQ.Client;
using System.Text;
using KwetterAuthenticationService.RabbitMQ;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace KwetterAuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationLogic logic;
        private readonly RabbitMqMessenger client;

        public AuthenticationController(UserDbContext context, ITokenBuilder _tokenBuilder)
        {
            logic = new AuthenticationLogic(context, _tokenBuilder);
            client = new RabbitMqMessenger();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            if (!logic.CheckUserExist(user))
                return NotFound("User not found.");

            if (!logic.AuthenticateUser(user))
                return BadRequest("Could not authenticate user");

            user = logic.GetData(user);
            var token = logic.GenerateToken(user);
            Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true});
            return Ok(token);

        }

        [HttpGet("verify")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> VerifyToken()
        {
            //Check if the received token contains a username
            var userId = User
                .Claims
                .SingleOrDefault();


            if (userId == null)
            {
                return Unauthorized();
            }

            //Check if the username matches a existing account
            User user = new User() { Id = Convert.ToInt32(userId.Value) };

            if (!logic.CheckUserExistById(user))
            {
                return Unauthorized();
            }

            return NoContent();
        }

        [HttpPost("create")]

        public async Task<IActionResult> CreateCredentials([FromBody] User user)
        {
            
            if (!logic.CheckUserExist(user))
            {
                var results = logic.UploadCredentials(user);
                client.SendRabbitMessage(JsonConvert.SerializeObject(user));
                return Ok();
            }
            else
            {
                return BadRequest("Username taken");
            }

            
        }

        //[HttpDelete("delete")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> DeleteCredentials(int id)
        //{
        //    var userId = User
        //        .Claims
        //        .SingleOrDefault();

        //    if(Convert.ToInt32(userId.Value) == id)
        //    {
        //        logic.DeleteCredentials(id);
        //        return Ok();
        //    }
        //    return Unauthorized();
        //}

        [HttpPost("test")]
        public async Task<IActionResult> Rabbit([FromBody] User user)
        {
            client.SendRabbitMessage(JsonConvert.SerializeObject(user));
            return Ok();
        }
    }
}

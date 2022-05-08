using DataAccesLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class AuthenticationLogic
    {
        private readonly UserRepository repo;
        private readonly ITokenBuilder tokenBuilder;

        public AuthenticationLogic(UserDbContext context, ITokenBuilder tBuilder)
        {
            repo = new UserRepository(context);
            tokenBuilder = tBuilder;
        }

        public bool CheckUserExist(User user)
        {
            return repo.CheckUserExist(user).Result;
        }

        public bool AuthenticateUser(User user)
        {
            return repo.AuthenticateUser(user).Result;
        }

        public string GenerateToken(User user)
        {
            var token = tokenBuilder.BuildToken(user.Username);
            return token;
        }
    }
}

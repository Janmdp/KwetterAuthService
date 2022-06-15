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
            var dbUser = repo.AuthenticateUser(user).Result;
            return Hasher.ValidatePassword(user.Password, dbUser.Password);
        }

        public string GenerateToken(User user)
        {
            var token = tokenBuilder.BuildToken(user);
            return token;
        }

        public string UploadCredentials(User user)
        {
            user.Password = Hasher.HashPassword(user.Password);
            var us = repo.UploadCredentials(user).Result;
            var token = tokenBuilder.BuildToken(us);
            return token;
        }

        public User GetData(User user)
        {
            return repo.GetData(user).Result;
        }

        public bool CheckUserExistById(User user)
        {
            return repo.CheckUserExistById(user).Result;
        }

        public async void DeleteCredentials(int id)
        {
            repo.DeleteCredentials(id);
        }
    }
}

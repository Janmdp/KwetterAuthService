using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccesLayer
{
    public class UserRepository
    {
        private readonly UserDbContext context;

        public UserRepository(UserDbContext cont)
        {
            context = cont;
        }

        public async Task<bool> CheckUserExist(User user)
        {
            var dbUser = await context
                .Users
                .SingleOrDefaultAsync(u => u.Username == user.Username);
            if (dbUser == null)
                return false;

            return true;
        }

        public async Task<User> AuthenticateUser(User user)
        {
            var dbUser = await context
                .Users
                .SingleOrDefaultAsync(u => u.Username == user.Username);

            return dbUser;
        }

        public async Task<User> GetData(User user)
        {
            return await context
                .Users
                .SingleOrDefaultAsync(u => u.Username == user.Username);
        }

        public async Task<User> UploadCredentials(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
            return  await context
                    .Users
                    .SingleOrDefaultAsync(u => u.Username == user.Username);
        }

        public async Task<bool> CheckUserExistById(User user)
        {
            var dbUser = await context
                .Users
                .SingleOrDefaultAsync(u => u.Id == user.Id);
            if (dbUser == null)
                return false;

            return true;
        }

        public async void DeleteCredentials(int id)
        {
            var dbUser =  context
                .Users
                .SingleOrDefault(u => u.Id == id);

            if (dbUser != null)
                context.Users.Remove(dbUser);
            context.SaveChanges();
        }
    }
}

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

        public async Task<bool> AuthenticateUser(User user)
        {
            var dbUser = await context
                .Users
                .SingleOrDefaultAsync(u => u.Username == user.Username);

            return dbUser.Password == user.Password;
        }
    }
}

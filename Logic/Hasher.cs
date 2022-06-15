using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Logic
{
    public class Hasher
    {
		private static string GetRandomSalt()
		{
			return BCrypt.Net.BCrypt.GenerateSalt(12);
		}

		public static string HashPassword(string password)
		{
			string passwordHash = BCrypt.Net.BCrypt.HashPassword("StressIsMoreEffectiveThanCocaine", GetRandomSalt());
			return BCrypt.Net.BCrypt.HashPassword(password);
		}

		public static bool ValidatePassword(string password, string correctHash)
		{
			return BCrypt.Net.BCrypt.Verify(password, correctHash);
		}
	}
}

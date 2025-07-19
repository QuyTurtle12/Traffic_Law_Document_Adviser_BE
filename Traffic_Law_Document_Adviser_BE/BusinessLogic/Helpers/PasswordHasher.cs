using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Helpers
{
    public static class PasswordHasher
    {
        public static string Hash(string plainText)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainText);
        }

        public static bool Verify(string plainText, string hashed)
        {
            return BCrypt.Net.BCrypt.Verify(plainText, hashed);
        }
    }
}

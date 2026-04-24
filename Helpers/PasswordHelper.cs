using System;
using System.Security.Cryptography;
using System.Text;

namespace HospitalMedSystem.Helpers
{
    public static class PasswordHelper
    {
        public static string Hash(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
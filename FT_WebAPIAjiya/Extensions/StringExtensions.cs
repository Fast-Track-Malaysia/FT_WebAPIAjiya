using System.Text;
using System.Security.Cryptography;

namespace FT_WebAPIAjiya.Extensions
{
    public static class StringExtensions
    {
        public static string SHA256(this string value)
        {
            using (var crypt = new SHA256Managed())
            {
                string hash = String.Empty;
                byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes($"{value}mmtg"));

                foreach (byte theByte in crypto)
                {
                    hash += theByte.ToString("x3");
                }

                return hash;
            }
        }
    }
}

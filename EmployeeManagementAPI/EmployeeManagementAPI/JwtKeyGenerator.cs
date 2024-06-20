using System.Security.Cryptography;

namespace EmployeeManagementAPI
{
    public class JwtKeyGenerator
    {
        public static string GenerateJwtKey()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] keyBytes = new byte[32];
                rng.GetBytes(keyBytes);
                return Convert.ToBase64String(keyBytes);
            }
        }
    }
}

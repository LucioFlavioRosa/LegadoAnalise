using System;
using System.Security.Cryptography;
using System.Text;

namespace Services.Auth
{
    public static class PkceHelper
    {
        public static string GenerateCodeVerifier()
        {
            var bytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Base64UrlEncode(bytes).Substring(0, 128);
        }

        public static string GenerateCodeChallenge(string codeVerifier)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier));
                return Base64UrlEncode(bytes);
            }
        }

        private static string Base64UrlEncode(byte[] arg)
        {
            return Convert.ToBase64String(arg)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}
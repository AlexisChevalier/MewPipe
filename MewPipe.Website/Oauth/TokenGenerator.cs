using System;
using System.Security.Cryptography;

namespace MewPipe.Website.Oauth
{
    public static class TokenGenerator
    {
        private static readonly RNGCryptoServiceProvider RngCryptoServiceProvider = new RNGCryptoServiceProvider();
        
        public static string GenerateRandomString(int size)
        {
            var bytes = new byte[size/2];
            RngCryptoServiceProvider.GetBytes(bytes);

            var hex = BitConverter.ToString(bytes);
            return hex.Replace("-", "").ToLower();
        }
    }
}
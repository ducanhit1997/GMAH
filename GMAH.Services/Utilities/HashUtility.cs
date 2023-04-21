using System.Text;

namespace GMAH.Services.Utilities
{
    public static class HashUtility
    {
        /// <summary>
        /// Convert plain text về dạng hashed text
        /// Tham khảo: https://auth0.com/blog/hashing-passwords-one-way-road-to-security/
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToHashedString(string input)
        {
            // Code tham khảo tại 
            // https://stackoverflow.com/questions/11454004/calculate-a-md5-hash-from-a-string
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}

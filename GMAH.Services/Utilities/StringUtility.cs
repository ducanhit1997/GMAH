using System;
using System.Linq;

namespace GMAH.Services.Utilities
{
    /// <summary>
    /// Hàm hỗ trợ xử lý chuỗi
    /// </summary>
    public class StringUtility
    {
        /// <summary>
        /// Random 1 chuỗi bất kỳ
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(int length)
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$%^&*/=`!+-*";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}

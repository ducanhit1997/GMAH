using System.Collections.Generic;

namespace GMAH.Models.Models
{
    public class UserIdentity
    {
        public int IdUser { get; set; }
        public int IdRole { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string[] Permission { get; set; }

        /// <summary>
        /// Convert identity obj to claims using for jwt
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> ConvertToClaims()
        {
            var claims = new Dictionary<string, object>();

            foreach (var item in GetType().GetProperties())
            {
                claims.Add(item.Name, item.GetValue(this, null));
            }

            return claims;
        }
    }
}

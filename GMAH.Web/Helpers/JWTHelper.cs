using GMAH.Models.Models;
using GMAH.Services.Interfaces;
using JWT.Algorithms;
using JWT.Serializers;
using JWT;
using System.Collections.Generic;
using JWT.Builder;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using Newtonsoft.Json;

namespace GMAH.Web.Helpers
{
    /// <summary>
    /// Jwt using HMACSHA256Algorithm and secret key
    /// https://github.com/jwt-dotnet/jwt
    /// </summary>
    public class JWTHelper : IAuthencationService
    {
        // Timeout cho mỗi token được sinh ra (in hour)
        private int timeOut = 24;

        // Đọc secret key từ Web.Config
        private string secretKey = ConfigurationManager.AppSettings["JWT_SECRET"];

        public string CreateTokenByIdentity(UserIdentity user)
        {
            // Not allow null user
            if (user is null)
            {
                throw new Exception("Không tồn tại người dùng đăng nhập");
            }

            var token = JwtBuilder.Create()
                                  .WithAlgorithm(new HMACSHA256Algorithm())
                                  .WithSecret(secretKey)

                                  // Add expired time (in hour)
                                  .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(timeOut).ToUnixTimeSeconds())

                                  // Add user claims
                                  .AddClaims(user.ConvertToClaims())

                                  // Encode to jwt token
                                  .Encode();

            return token;
        }

        // Decode token từ string token thành user identity object
        public UserIdentity DecodeToken(string token)
        {
            // Decode thành json string
            var json = JwtBuilder.Create()
                                 .WithAlgorithm(new HMACSHA256Algorithm())
                                 .WithSecret(secretKey)
                                 .MustVerifySignature()
                                 .Decode(token);

            // Parse thành object
            return JsonConvert.DeserializeObject<UserIdentity>(json);
        }
    }
}
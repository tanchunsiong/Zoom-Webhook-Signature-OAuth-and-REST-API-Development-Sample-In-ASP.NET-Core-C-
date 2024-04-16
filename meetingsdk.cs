
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
public static class meetingsdk
{

    public static Task MeetingSDK(HttpContext context)
    {
        return Task.Run(async () =>
        {

            string MSDK_CLIENT_ID = Environment.GetEnvironmentVariable("MSDK_CLIENT_ID");
            string MSDK_CLIENT_SECRET = Environment.GetEnvironmentVariable("MSDK_CLIENT_SECRET");


            var authorizationToken = context.Request.Query["code"].ToString();
            
            var currentDateOffset = DateTimeOffset.UtcNow;
            var epochTime = (int)currentDateOffset.ToUnixTimeSeconds();

            var epochTime48HoursLater = epochTime + 172800; // 172800 seconds = 2 days
            DateTime epochDateTimeExpiry = DateTimeOffset.FromUnixTimeSeconds(epochTime48HoursLater).UtcDateTime;

            var sdkSecret = MSDK_CLIENT_SECRET;
            var appKey = MSDK_CLIENT_ID;
            var data = new Dictionary<string, object>
        {
            { "appKey", appKey },
            { "iat", epochTime },
            { "exp", epochTime48HoursLater },
            { "tokenExp", epochTime48HoursLater },
            { "mn", 9898533313 },
            { "role", 1 }
        };
            var meetingSdkKey = GenerateSignature(data, sdkSecret, epochDateTimeExpiry);


            await context.Response.WriteAsync(JsonConvert.SerializeObject(meetingSdkKey));
        });

        static string GenerateSignature(Dictionary<string, object> data, string secret, DateTime epochDateTimeExpiry)
        {
       
            var claims = data.Select(kv => new Claim(kv.Key, ConvertValueToString(kv.Value)));

            var identity = new ClaimsIdentity(claims, "Custom");
            //var handler = new JwtSecurityTokenHandler();
            //var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            //{
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)), SecurityAlgorithms.HmacSha256),
            //    Subject = identity,
            //    Expires = epochDateTimeExpiry, 
            //    NotBefore= null
            //});
            //return handler.WriteToken(securityToken);
            // Create JWT token without setting the "nbf" claim
            var securityToken = new JwtSecurityToken(
                claims: identity.Claims,
                expires: epochDateTimeExpiry,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)), SecurityAlgorithms.HmacSha256),
                notBefore: null  // Exclude the "nbf" claim
            );

            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(securityToken);
        }
         static string ConvertValueToString(object value)
        {
            // Check if the value is a DateTime and format it accordingly
            if (value is DateTime dateTimeValue)
            {
                return dateTimeValue.ToString("yyyy-MM-ddTHH:mm:ssZ");
            }

            // Convert other types to string
            return value.ToString();
        }

    }
}

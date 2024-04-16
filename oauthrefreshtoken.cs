
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
public static class oauthrefreshtoken
{

    public static Task OauthRefreshToken(HttpContext context)
    {
        return Task.Run(async () =>
        {


            var oauthClientId = Environment.GetEnvironmentVariable("OAUTH_CLIENT_ID");
            var oauthClientSecret = Environment.GetEnvironmentVariable("OAUTH_CLIENT_SECRET");
            var refreshToken = context.Request.Query["code"].ToString();

            var url = "https://zoom.us/oauth/token";

            var credentials = $"{oauthClientId}:{oauthClientSecret}";
            var credentialsEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

            var data = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };

            var content = new FormUrlEncodedContent(data);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentialsEncoded);

        

            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                var accessToken = JsonDocument.Parse(jsonResponse).RootElement.GetProperty("access_token").GetString();

                // Set the "Content-Type" header to "application/json"
                await context.Response.WriteAsync(JsonConvert.SerializeObject(accessToken));
            }
            else
            {
                await context.Response.WriteAsync($" Request Failed with Status Code: {response.StatusCode}");

            }
        });
    }

}


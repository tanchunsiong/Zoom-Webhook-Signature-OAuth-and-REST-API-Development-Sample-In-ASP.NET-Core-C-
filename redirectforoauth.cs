
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
using System.Threading.Tasks;
public static class redirectforoauth
{
 
    public static Task RedirectForOauth(HttpContext context)
    {
        return Task.Run(async () =>
        {

          
            String AppClientID = Environment.GetEnvironmentVariable("OAUTH_CLIENT_ID"); ;
            String AppClientSecret = Environment.GetEnvironmentVariable("OAUTH_CLIENT_SECRET"); ;
            String redirectURL = "https://csharp.asdc.cc/redirectforoauth";

            var authorizationToken = context.Request.Query["code"].ToString();

            try
            {
                using (HttpClient _httpClient = new HttpClient())
                {
                    var url = "https://zoom.us/oauth/token";

                    using var client = new HttpClient();

                    // Encode the client ID and client secret
                    var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{AppClientID}:{AppClientSecret}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);


                    var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "redirect_uri", redirectURL },
                    { "code", authorizationToken }
                });

                    var response = await client.PostAsync(url, content);


                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(responseContent));

                    }
                    else
                    {
                        // Handle error response
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(response.StatusCode));

                    }
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exceptions
                Console.WriteLine($"HTTP Request Exception: {ex.Message}");
                //throw new Exception($"HTTP Request Exception: {ex.Message}");
            }
            catch (System.Text.Json.JsonException ex)
            {
                // Handle JSON parsing exceptions
                Console.WriteLine($"JSON Parsing Exception: {ex.Message}");
                throw new Exception($"JSON Parsing Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Exception occurred: {ex.Message}");
                throw new Exception($"Exception occurred: {ex.Message}");
            }
        });
    }

    internal static RequestDelegate RedirectForOauth(string? oAUTH_CLIENT_ID, string? oAUTH_CLIENT_SECRET)
    {
        throw new NotImplementedException();
    }
}


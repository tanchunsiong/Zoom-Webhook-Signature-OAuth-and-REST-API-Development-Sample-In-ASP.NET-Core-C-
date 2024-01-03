using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

var app = builder.Build();
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.MapGet("/webhook", () =>
{
    // Handle GET request logic here
    return "GET request received.";
});


app.MapPost("/webhook", async context =>
{
    // Get the request content as a string asynchronously
    string requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

    try
    {
        JObject jsonObject = JObject.Parse(requestBody);
        JObject payload = (JObject)jsonObject["payload"];
        string plainToken = payload["plainToken"].ToString();
        string encryptedToken = GetHash(plainToken, "");

        var response = new
        {
            plainToken,
            encryptedToken
        };
        context.Response.ContentType = "application/json";

        // Write the JSON response
        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));

    }
    catch (Exception ex)
    {
        await context.Response.WriteAsync("Error: " + ex.ToString());
    }

});




app.MapGet("/redirectforoauth", async context =>
{

    String AppClientID = "redacted";
    String AppClientSecret = "redacted";
    String redirectURL = "https://18cc-58-182-81-118.ngrok-free.app/redirectforoauth";

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

string GetHash(string text, string key)
{
    using (HMACSHA256 sha256 = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
    {
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}


app.Run();

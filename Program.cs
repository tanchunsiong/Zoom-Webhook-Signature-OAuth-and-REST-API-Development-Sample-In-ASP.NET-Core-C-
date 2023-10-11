using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
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

 string GetHash(string text, string key)
{
    using (HMACSHA256 sha256 = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
    {
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}


app.Run();

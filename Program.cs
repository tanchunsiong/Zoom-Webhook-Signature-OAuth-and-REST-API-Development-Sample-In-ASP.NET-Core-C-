using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection.Emit;

using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using dotenv.net;
using System;
using Microsoft.AspNetCore.Hosting;

DotEnv.Load();

string MSDK_CLIENT_ID = Environment.GetEnvironmentVariable("MSDK_CLIENT_ID");
string MSDK_CLIENT_SECRET = Environment.GetEnvironmentVariable("MSDK_CLIENT_SECRET");
string OAUTH_SECRET_TOKEN = Environment.GetEnvironmentVariable("OAUTH_SECRET_TOKEN");
string OAUTH_CLIENT_ID = Environment.GetEnvironmentVariable("OAUTH_CLIENT_ID");
string OAUTH_CLIENT_SECRET = Environment.GetEnvironmentVariable("OAUTH_CLIENT_SECRET");
string S2S_OAUTH_CLIENT_ID = Environment.GetEnvironmentVariable("S2S_OAUTH_CLIENT_ID");
string S2S_OAUTH_CLIENT_SECRET = Environment.GetEnvironmentVariable("S2S_OAUTH_CLIENT_SECRET");
string S2S_OAUTH_ACCOUNT_ID = Environment.GetEnvironmentVariable("S2S_OAUTH_ACCOUNT_ID");

var builder = WebApplication.CreateBuilder(args);




var app = builder.Build();
// Configure the HTTP request pipeline.

// Create and configure the host
var host = builder.WebHost;

// Set the URL and port to listen on
app.Urls.Add("http://127.0.0.1:7014");
//don't use https
//app.UseHttpsRedirection();


app.MapGet("/", HandleRoot);
app.MapGet("/webhook", webhook.GetWebhook);

app.MapPost("/webhook", webhook.PostWebhook);

app.MapGet("/redirectforoauth", redirectforoauth.RedirectForOauth);



static async Task HandleRoot(HttpContext context)
{
    // Read the HTML file content
    string htmlContent = await File.ReadAllTextAsync("index.html");

    // Set the content type of the response
    context.Response.ContentType = "text/html";

    // Write the HTML content to the response
    await context.Response.WriteAsync(htmlContent);
}

app.Run();

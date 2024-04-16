using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
public class callapi
{
    public static Task CallAPI(HttpContext context)
    {
        return Task.Run(async () =>
        {
            // Fetch access_token from query string
            var access_token = context.Request.Query["access_token"];

            // Meeting data
            var meeting_data = new
            {
                topic = "hello world",
                type = 2,
                start_time = "2023-10-01T10:00:00Z",
                duration = 120,
                password = "12345678",
                agenda = "40 mins limit demonstration",
                pre_schedule = false,
                timezone = "Asia/Singapore",
                default_password = false
            };

            // Zoom API endpoint
            var api_url = "https://api.zoom.us/v2/users/me/meetings";

            // Headers for the request
            var headers = new
            {
                Authorization = $"Bearer {access_token}",
                Content_Type = "application/json",
                Accept = "application/json"
            };

            // Send POST request to create meeting
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, api_url);
            request.Content = new StringContent(JsonSerializer.Serialize(meeting_data), Encoding.UTF8, "application/json");

            foreach (var header in headers.GetType().GetProperties())
            {
                request.Headers.Add(header.Name, header.GetValue(headers).ToString());
            }

            var response = await client.SendAsync(request);

            // Return response
            await context.Response.WriteAsync("Meeting Details: " + await response.Content.ReadAsStringAsync());
        });


    }
}


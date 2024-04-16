using Newtonsoft.Json;
using System.Text;
using System.Text.Json;



    public class s2soauth
    {

        public static Task S2SOauth(HttpContext context)
        {
            return Task.Run(async () =>
            {

                string s2s_oauth_client_id = Environment.GetEnvironmentVariable("S2S_OAUTH_CLIENT_ID"); 
                string s2s_oauth_client_secret = Environment.GetEnvironmentVariable("S2S_OAUTH_CLIENT_SECRET"); 
                string s2s_oauth_account_id = Environment.GetEnvironmentVariable("S2S_OAUTH_ACCOUNT_ID");


                // Access the environment variables
                var clientId = s2s_oauth_client_id;
                var clientSecret = s2s_oauth_client_secret;
                var accountId = s2s_oauth_account_id;
                var oauthUrl = $"https://zoom.us/oauth/token?grant_type=account_credentials&account_id={accountId}";

                try
                {
                    // Create the Basic Authentication header
                    var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

                    // Initialize HttpClient
                    using var client = new HttpClient();

                    // Create request message
                    var request = new HttpRequestMessage(HttpMethod.Post, oauthUrl);
                    request.Headers.Add("Authorization", $"Basic {authHeader}");

                    // Send request and get response
                    var response = await client.SendAsync(request);

                    // Check if the request was successful (status code 200)
                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the JSON response to get the access token
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var accessToken = JsonDocument.Parse(jsonResponse).RootElement.GetProperty("access_token").GetString();

                        // Set the "Content-Type" header to "application/json"
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(accessToken));
                      
                    }
                    else
                    {
                        await context.Response.WriteAsync($"OAuth Request Failed with Status Code: {response.StatusCode}");
                      
                    }
                }
                catch (Exception e)
                {
                    await context.Response.WriteAsync($"An error occurred: {e.Message}");
                   
                }


            });
        }

   
    }


using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;

class Program
{
    static async System.Threading.Tasks.Task Main(string[] args)
    {
        // Step 1: Authenticate and get the token
        string token = await GetAuthToken("cloud_auth.json", "https://xray.cloud.getxray.app/api/v1/authenticate");

        // Step 2: Make the API call with the obtained token
        await ImportExecution("data.xml", "https://xray.cloud.getxray.app/api/v1/import/execution/nunit?projectKey=XTP&testExecKey=XNP-23", token);
    }

    static async System.Threading.Tasks.Task<string> GetAuthToken(string authFile, string authUrl)
    {
        using (HttpClient client = new HttpClient())
        {
            // Read the content of the JSON file
            string jsonContent = System.IO.File.ReadAllText(authFile);

            // Send a POST request to authenticate and get the token
            var response = await client.PostAsync(authUrl, new StringContent(jsonContent, Encoding.UTF8, "application/json"));

            // Check if the request was successful
            response.EnsureSuccessStatusCode();

            // Parse the JSON response and extract the token
            string jsonResponse = await response.Content.ReadAsStringAsync();
            string token = JObject.Parse(jsonResponse)["token"].ToString();

            return token;
        }
    }

    static async System.Threading.Tasks.Task ImportExecution(string dataFile, string importUrl, string token)
    {
        using (HttpClient client = new HttpClient())
        {
            // Read the content of the XML file
            string xmlContent = System.IO.File.ReadAllText(dataFile);

            // Set up the request with headers
            var request = new HttpRequestMessage(HttpMethod.Post, importUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml");

            // Send the POST request to import execution
            var response = await client.SendAsync(request);

            // Check if the request was successful
            response.EnsureSuccessStatusCode();

            // Print the response if needed
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }
}

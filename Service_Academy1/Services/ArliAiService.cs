using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

public class ArliAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    // Constructor to inject HttpClient and Configuration
    public ArliAIService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["ArliAI:ApiKey"];  // Ensure this is in your appsettings.json
    }

    // Generate a recommendation based on analytics data
    public async Task<string> GetRecommendation(string prompt)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.arliai.com/v1/chat/completions")
        {
            Headers =
        {
            { "Authorization", $"Bearer {_apiKey}" }
        },
            Content = new StringContent(JsonConvert.SerializeObject(new
            {
                model = "Meta-Llama-3.1-8B-Instruct", // Replace with the desired model
                messages = new[]
                {
                new { role = "system", content = "You are a helpful business analyst." },
                new { role = "user", content = prompt }
            },
                max_tokens = 1024,
                temperature = 0.7
            }), Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<dynamic>(responseContent);
            return responseJson?.choices[0]?.message?.content?.ToString() ?? "No insights generated.";
        }
        else
        {
            return $"Error: {response.StatusCode}, {response.ReasonPhrase}";
        }
    }
    public async Task<string> GetAnalysis(string prompt)
    {
        // Define the request to the ArliAI API
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.arliai.com/v1/chat/completions")
        {
            Headers =
        {
            { "Authorization", $"Bearer {_apiKey}" }
        },
            Content = new StringContent(JsonConvert.SerializeObject(new
            {
                model = "Meta-Llama-3.1-8B-Instruct", // Replace with the desired model
                messages = new[]
                {
                new { role = "system", content = "You are an expert data analyst specializing in educational program evaluations." },
                new { role = "user", content = prompt }
            },
                max_tokens = 1024,
                temperature = 0.7
            }), Encoding.UTF8, "application/json")
        };

        // Send the request and get the response
        var response = await _httpClient.SendAsync(request);

        // Check the response status
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<dynamic>(responseContent);
            return responseJson?.choices[0]?.message?.content?.ToString() ?? "No analysis generated.";
        }
        else
        {
            return $"Error: {response.StatusCode}, {response.ReasonPhrase}";
        }
    }
    public async Task<string> GetImpact(string prompt)
    {
        // Define the request to the ArliAI API
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.arliai.com/v1/chat/completions")
        {
            Headers =
        {
            { "Authorization", $"Bearer {_apiKey}" }
        },
            Content = new StringContent(JsonConvert.SerializeObject(new
            {
                model = "Meta-Llama-3.1-8B-Instruct", // Replace with the desired model
                messages = new[]
                {
                new { role = "system", content = "You are an expert data analyst specializing in impact assessments using descriptive analytics. You analyze past data to identify trends, summarize key metrics, and provide insights into the effectiveness of programs based on historical performance." },
                new { role = "user", content = prompt }
            },
                max_tokens = 1024,
                temperature = 0.7
            }), Encoding.UTF8, "application/json")
        };

        // Send the request and get the response
        var response = await _httpClient.SendAsync(request);

        // Check the response status
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<dynamic>(responseContent);
            return responseJson?.choices[0]?.message?.content?.ToString() ?? "No analysis generated.";
        }
        else
        {
            return $"Error: {response.StatusCode}, {response.ReasonPhrase}";
        }
    }

}

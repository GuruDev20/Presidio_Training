using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

public class FAQService(IConfiguration configuration) : IFAQService
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly string _apiKey = configuration["GeminiAPIKey"]!;
    private readonly string _endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

    public async Task<string> GetAnswerAsync(string question)
    {
        var prompt = $@"
        You are a banking chatbot trained to answer customer FAQs clearly, accurately, and briefly. 
        Do not say 'please visit the website', give disclaimers, or say you're an AI.

        Format:
        Q: What is the minimum balance in HDFC Savings account?
        A: ₹10,000 in metro areas, ₹5,000 in semi-urban, ₹2,500 in rural areas.

        Now answer:
        Q: {question}
        A:";

        var request = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_endpoint}?key={_apiKey}", content);

        if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
        {
            await Task.Delay(2000);
            response = await _httpClient.PostAsync($"{_endpoint}?key={_apiKey}", content);
        }

        if (!response.IsSuccessStatusCode)
        {
            return $"Error: {response.StatusCode}";
        }

        var responseString = await response.Content.ReadAsStringAsync();
        var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseString);

        var answer = geminiResponse?.candidates?[0]?.content?.parts?[0]?.text?.Trim() ?? "No response from AI.";

        if (answer.StartsWith("A:"))
        {
            answer = answer.Substring(2).Trim();
        }
        return answer;
    }
}


using Microsoft.AspNetCore.Mvc;
using OPENAI_API.Models;
using System.Text.Json;
using OPENAI_API.Models;

public class WeatherController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public WeatherController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index(string city = "Kochi")
    {
        try
        {
            string apiKey = _configuration["WeatherApi:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                ViewBag.Error = "API key is missing.";
                return View();
            }

            string url =
                $"https://api.weatherapi.com/v1/current.json?key={Uri.EscapeDataString(apiKey)}&q={Uri.EscapeDataString(city)}&aqi=no";

            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Unable to get weather. Check the city or API key.";
                return View();
            }

            using var stream =
                await response.Content.ReadAsStreamAsync();

            using JsonDocument document =
                await JsonDocument.ParseAsync(stream);

            var root = document.RootElement;

            var location = root.GetProperty("location");
            var current = root.GetProperty("current");
            var condition = current.GetProperty("condition");

            var weather = new WeatherViewModel
            {
                City = location.GetProperty("name").GetString() ?? "",
                Temperature = current.GetProperty("temp_c").GetDouble(),
                Condition = condition.GetProperty("text").GetString() ?? "",
                Icon = "https:" + condition.GetProperty("icon").GetString(),
                Humidity = current.GetProperty("humidity").GetDouble(),
                WindSpeed = current.GetProperty("wind_kph").GetDouble(),
                LastUpdated = current.GetProperty("last_updated").GetString() ?? ""
            };

            return View(weather);
        }
        catch (Exception)
        {
            ViewBag.Error = "Something went wrong while fetching weather.";
            return View();
        }
    }
}

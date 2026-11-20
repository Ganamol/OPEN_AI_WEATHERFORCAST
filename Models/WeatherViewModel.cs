namespace OPENAI_API.Models
{
    public class WeatherViewModel
    {
        public string City { get; set; }

        public double Temperature { get; set; }

        public string Condition { get; set; }

        public string Icon { get; set; }

        public double Humidity { get; set; }

        public double WindSpeed { get; set; }

        public string LastUpdated { get; set; }
    }
}

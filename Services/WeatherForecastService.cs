using OpenTracing;

namespace Jaeger.Services
{
    public class WeatherForecastService
    {
        private readonly ITracer _tracer;

        public WeatherForecastService(ITracer tracer)
        {
            _tracer = tracer;
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        public IEnumerable<WeatherForecast> GetWeatherForecasts()
        {
            using var scope = _tracer.BuildSpan("GetWeatherForecasts").StartActive(true);
            {
                return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();
            }
        }
    }
}
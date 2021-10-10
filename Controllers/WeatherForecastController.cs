using Jaeger.Services;
using Microsoft.AspNetCore.Mvc;
using OpenTracing;

namespace Jaeger.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ITracer _tracer;
    private readonly WeatherForecastService _weatherForecastService;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ITracer tracer, WeatherForecastService weatherForecastService)
    {
        _logger = logger;
        _tracer = tracer;
        _weatherForecastService = weatherForecastService;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast>? Get()
    {
        try
        {
            var serviceName = ControllerContext.ActionDescriptor.DisplayName;
            using var scope = _tracer.BuildSpan(serviceName).StartActive(true);
            {
                return _weatherForecastService.GetWeatherForecasts();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        return null;
    }
}

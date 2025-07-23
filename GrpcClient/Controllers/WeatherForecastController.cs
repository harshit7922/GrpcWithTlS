using GrpcServer;
using Microsoft.AspNetCore.Mvc;

namespace GrpcClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly Greeter.GreeterClient _greeterClient;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, Greeter.GreeterClient greeterClient)
        {
            _logger = logger;
            _greeterClient = greeterClient; 
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("test-grpc")]
        public async Task<IActionResult> TestGrpc()
        {
            var reply = await _greeterClient.SayHelloAsync(new HelloRequest { Name = "World" });
            return Ok(reply.Message);
        }
    }
}

using combined;
using Grpc.Core;
using GrpcServer;
using Microsoft.AspNetCore.Mvc;

namespace GrpcClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly Greeter.GreeterClient _greeterClient;
        private readonly FullEmployeeService.FullEmployeeServiceClient _fullEmployeeServiceClient;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, Greeter.GreeterClient greeterClient, FullEmployeeService.FullEmployeeServiceClient fullEmployeeServiceClient)
        {
            _logger = logger;
            _greeterClient = greeterClient;
            _fullEmployeeServiceClient = fullEmployeeServiceClient;
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

        [HttpGet("test-grpc-combined-getall")]
        public async Task<IActionResult> TestGrpcCombinedGetAll()
        {
            var employees = new List<EmployeeReply>();
            using var call = _fullEmployeeServiceClient.StreamAllEmployees(new Empty());

            while (await call.ResponseStream.MoveNext())
            {
                employees.Add(call.ResponseStream.Current);
            }

            if (!employees.Any())
            {
                return NotFound("No employees found.");
            }
            return Ok(employees);
        }

        [HttpGet("get-employee/{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var request = new EmployeeRequest { Id = id };
            try
            {
                var reply = await _fullEmployeeServiceClient.GetEmployeeAsync(request);
                if (reply == null)
                {
                    return NotFound($"Employee with ID {id} not found.");
                }
                return Ok(reply);
            }
            catch (RpcException ex)
            {
                return NotFound($"Employee with ID {id} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving employee: {ex.Message}");
            }
        }

        [HttpGet("chat")]
        public async Task<IActionResult> Chat([FromQuery] string message)
        {
            var responses = new List<chatMessage>();

            using var call = _fullEmployeeServiceClient.Chat();

            // Send the message to the server
            await call.RequestStream.WriteAsync(new chatMessage { Message = message });

            // Complete the request stream to signal no more messages will be sent
            await call.RequestStream.CompleteAsync();

            // Read all responses from the server
            while (await call.ResponseStream.MoveNext())
            {
                responses.Add(call.ResponseStream.Current);
            }

            return Ok(responses);
        }
    }
}

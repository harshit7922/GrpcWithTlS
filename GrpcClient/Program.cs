using Grpc.Core;
using GrpcServer;
using GrpcServer.Services;
using System.IO;
using System.Net;
using System.Net.Security;

var builder = WebApplication.CreateBuilder(args);

//var rootCert = File.ReadAllText(@"../../certs/localhost.crt");
//var sslCred = new SslCredentials(rootCert);

//var channel = new Channel("localhost:7163", sslCred);
//var client = new Greeter.GreeterClient(channel);
//var reply = client.SayHello(new HelloRequest { Name = "World" });
//Console.WriteLine("Greeting: " + reply.Message);
// Add services to the container.

builder.Services.AddGrpcClient<Greeter.GreeterClient>(o =>
{
    o.Address = new Uri("https://localhost:7163");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    // This callback allows you to validate the server certificate.
    handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, chain, errors) =>
    {
        // Load your root certificate
        var rootCert = File.ReadAllText(@"../../certs/localhost.crt");
        // Compare the server certificate with your root certificate
        return cert != null && cert.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Cert)
            .SequenceEqual(System.Text.Encoding.ASCII.GetBytes(rootCert));
        // For development, you can use: return true;
    };
    return handler;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

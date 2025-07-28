using combined;
using Grpc.Core;
using GrpcServer;
using GrpcServer.Services;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

//var rootCert = File.ReadAllText(@"../../certs/localhost.crt");
//var sslCred = new SslCredentials(rootCert);

//var channel = new Channel("localhost:7163", sslCred);
//var client = new Greeter.GreeterClient(channel);
//var reply = client.SayHello(new HelloRequest { Name = "World" });
//Console.WriteLine("Greeting: " + reply.Message);
// Add services to the container.
var pfxPath = @"../../certs/localhostnew.pfx";//@"../../certs/newlocalhostcert.pfx";
var pfxPassword = "password123";//"localhost@123"; // Replace with your actual password

builder.Services.AddGrpcClient<Greeter.GreeterClient>(o =>
{
    o.Address = new Uri("https://localhost:7163");
}).ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    // Load the certificate from the PFX file
    var clientCert = new X509Certificate2(pfxPath, pfxPassword);

    handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, chain, errors) =>
    {
        // Compare the server certificate with the certificate from the PFX file
        return cert != null && cert.Thumbprint == clientCert.Thumbprint;
        // For development only: return true;
    };
    return handler;
});

// Register FullEmployeeService client
builder.Services.AddGrpcClient<FullEmployeeService.FullEmployeeServiceClient>(o =>
{
    o.Address = new Uri("https://localhost:7163");
}).ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    var clientCert = new X509Certificate2(pfxPath, pfxPassword);
    handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, chain, errors) =>
    {
        return cert != null && cert.Thumbprint == clientCert.Thumbprint;
    };
    return handler;
});




//.ConfigurePrimaryHttpMessageHandler(() =>
//{
//    var handler = new HttpClientHandler();
//    // This callback allows you to validate the server certificate.
//    handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, chain, errors) =>
//    {
//        // Load your root certificate
//        var rootCert = File.ReadAllText(@"../../certs/localhost.crt");
//        // Compare the server certificate with your root certificate
//        return cert != null && cert.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Cert)
//            .SequenceEqual(System.Text.Encoding.ASCII.GetBytes(rootCert));
//        // For development, you can use: return true;
//    };
//    return handler;
//});

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

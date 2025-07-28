using GrpcServer;
using GrpcServer.Repository;
using GrpcServer.Services;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Configure Kestrel to use HTTPS with a self-signed certificate.
    serverOptions.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
        httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
        //httpsOptions.ServerCertificate = new X509Certificate2("path/to/your/certificate.pfx", "your_certificate_password");
        //httpsOptions.ServerCertificate = new X509Certificate2(@"../../certs/newlocalhostcert.pfx", "localhost@123");
        httpsOptions.ServerCertificate = new X509Certificate2(@"../../certs/localhostnew.pfx", "password123");

    });
});

// Add services to the container.
builder.Services.AddGrpc();

// With this line:
builder.Services.AddAutoMapper(cfg => { }, typeof(GrpcMappingProfile));
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<FullEmployeeServiceImpl>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

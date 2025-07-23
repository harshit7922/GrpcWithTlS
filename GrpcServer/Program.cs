using GrpcServer.Services;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using Microsoft.AspNetCore.Server.Kestrel.Https;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Configure Kestrel to use HTTPS with a self-signed certificate.
    serverOptions.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
        httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
        //httpsOptions.ServerCertificate = new X509Certificate2("path/to/your/certificate.pfx", "your_certificate_password");
        httpsOptions.ServerCertificate = new X509Certificate2(@"../../certs/newlocalhostcert.pfx", "localhost@123");

    });
});

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

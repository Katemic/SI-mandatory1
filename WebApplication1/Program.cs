
using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Library.Data.Context;
using Library.Data.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Get database connection from User Secrets
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Database connection string not configured"
    );

// 2. Register MySQL database
builder.Services.AddDbContext<LibraryDbContext>(
    options => options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

// 3. Register repositories from Library.Data
builder.Services.AddLibraryRepositories();

// 4. Configure SOAP services
builder.Services
    .AddServiceModelServices()
    .AddServiceModelMetadata();

builder.Services.AddSingleton<
    IServiceBehavior,
    UseRequestHeadersForMetadataAddressBehavior>();

// 5. Register SOAP service
builder.Services.AddScoped<LibraryService>();

var app = builder.Build();

// 6. Configure SOAP endpoint
app.UseServiceModel(services =>
{
    services
        .AddService<LibraryService>()
        .AddServiceEndpoint<LibraryService, ILibraryService>(
            new BasicHttpBinding(),
            "/soap/library");
});

// 7. Enable WSDL
var metadata = app.Services
    .GetRequiredService<ServiceMetadataBehavior>();

metadata.HttpGetEnabled = true;

app.Run();

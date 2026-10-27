
using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddServiceModelServices()
    .AddServiceModelMetadata();

builder.Services.AddSingleton<
    IServiceBehavior,
    UseRequestHeadersForMetadataAddressBehavior>();

builder.Services.AddSingleton<LibraryService>();

var app = builder.Build();

app.UseServiceModel(services =>
{
    services
        .AddService<LibraryService>()
        .AddServiceEndpoint<LibraryService, ILibraryService>(
            new BasicHttpBinding(),
            "/soap/library");
});

var metadata = app.Services
    .GetRequiredService<ServiceMetadataBehavior>();

metadata.HttpGetEnabled = true;

app.Run();

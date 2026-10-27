using Library.Data.Context;
using Library.Data.Repositories;
using Library.gRPCApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Database connection string is not configured.");

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)));

builder.Services.AddLibraryRepositories();
builder.Services.AddSingleton<IBookNotificationService, BookNotificationService>();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<LibraryGrpcService>();

app.Run();

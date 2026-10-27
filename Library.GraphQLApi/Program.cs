using Library.Data.Context;
using Library.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Library.GraphQLApi.GraphQL;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

var builder = WebApplication.CreateBuilder(args);

// database stuff
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Database connection string not configured");

// Register EF Core with MySQL
builder.Services.AddDbContext<LibraryDbContext>(
    options => options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

// Register shared repositories
builder.Services.AddLibraryRepositories();

// Register GraphQL services
builder.Services
    .AddGraphQLServer()
    .AddQueryType<LibraryQuery>()
    .AddMutationType<LibraryMutation>();



// Add services to the container.

var app = builder.Build();

app.MapGraphQL("/graphql")
    .WithOptions(options => options.EnableGetRequests = false);

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();



app.Run();


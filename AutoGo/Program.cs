using Agoda.IoC.NetCore;
using AutoGo.BotHandlers;
using AutoGo.Data.DbContexts;
using AutoGo.Services.Workers;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment.EnvironmentName;
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{env}.json", true)
    .AddEnvironmentVariables()
    .Build();

// Add services to the container.

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // Read settings from appsettings.json
    .Enrich.FromLogContext() // Add contextual information to logs
    .WriteTo.Console() // Write logs to the console
    .WriteTo.Seq("http://localhost:5341") // Write logs to Seq (default Seq URL)
    .CreateLogger();

// Use Serilog as the logging provider
builder.Host.UseSerilog();

builder.Services.AutoWireAssembly([typeof(Program).Assembly], false);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<TelegramWorker>();
builder.Services.AddHostedService<BookingCreationWorker>();
builder.Services.AddHostedService<BookingAcknowledgmentWorker>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
    .UseSnakeCaseNamingConvention()
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate(); // Applies any pending migrations and creates the database if it doesn't exist
    }
    catch (Exception ex)
    {
        // Log the exception or handle it as needed
        Log.Error(ex, "An error occurred while migrating the database.");
        throw ex;
    }
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

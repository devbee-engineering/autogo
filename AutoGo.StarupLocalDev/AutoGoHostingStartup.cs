using AutoGo.StartupLocalDev;
using Microsoft.AspNetCore.Hosting;
using Testcontainers.PostgreSql;

[assembly: HostingStartup(typeof(AutoGoHostingStartup))]
namespace AutoGo.StartupLocalDev;

public class AutoGoHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
        {
            return;
        }

        var result = StartContainers().GetAwaiter().GetResult();
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", result.DbConnectionString);
    }

    private async Task<ContainerResult> StartContainers()
    {
        var containerResult = new ContainerResult();
        var postgreSqlContainer = new PostgreSqlBuilder()
           .WithName("autogo")
           .WithReuse(true)
           .Build();

        await postgreSqlContainer.StartAsync();

        containerResult.DbConnectionString = postgreSqlContainer.GetConnectionString();

        return containerResult;

    }
}

public class ContainerResult
{
    public string? DbConnectionString { get; set; }
}

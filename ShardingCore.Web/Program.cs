using Microsoft.EntityFrameworkCore;
using Serilog;
using ShardingCore;
using ShardingCore.Domain;
using ShardingCore.EntityFrameworkCore;
using ShardingCore.Web;
using ShardingCore.Web.Controllers;
using System.Text.Json;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    var assemblies = new[]
    {
        typeof(Order).Assembly,
        typeof(Program).Assembly,
    };

    // Add services to the container.
    builder.Services.AddControllers().AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.MaxDepth = 10;
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(o =>
    {
        o.AddStronglyTypedId(assemblies);
    });

    builder.Services.AddShardingCoreDbContext(o =>
    {
        //use your data base connection string
        o.AddDefaultDataSource("default", builder.Configuration.GetConnectionString("DefaultConnection"));
    });

    builder.Host.UseSerilog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //先执行迁移，然后执行分片补偿表
    using (var scope = app.Services.CreateScope())
    {
        scope.ServiceProvider
            .GetRequiredService<ShardingCoreDbContext>()
            .Database
            .Migrate();

        scope.ServiceProvider.UseAutoTryCompensateTable();
    }

    app.UseAuthorization();

    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


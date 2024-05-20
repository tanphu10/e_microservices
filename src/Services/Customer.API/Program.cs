using Serilog;
using Common.Logging;
using Customer.API.Persistence;
using Microsoft.EntityFrameworkCore;
using Customer.API.Repositories.Interfaces;
using Customer.API.Repositories;
using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Customer.API.Services.Interfaces;
using Customer.API.Services;
using Shared.Dtos.Customer;
using AutoMapper;
using Customer.API.Controllers;


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);
Log.Information($"Start Customer API  up");

try
{
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
    builder.Services.AddDbContext<CustomerContext>(
        options => options.UseNpgsql(connectionString));
    //builder.Services.AddScoped<ICustomerRepository, CustomerRepository>().AddScoped(typeof(IRepositoryBaseAsync<,,>), typeof(RepositoryBase<,,>)).AddScoped<ICustomerService, CustomerService>();

    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>().AddScoped(typeof(IRepositoryQueryBase<,,>), typeof(RepositoryQueryBase<,,>)).AddScoped<ICustomerService, CustomerService>();

    var app = builder.Build();
    app.MapGet("/", () => "Welcome to Customer API");
    app.MapCustomerAPI();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
                $"swagger API v1"));
        });
    }
    // app.UseHttpsRedirection(); //production only

    app.UseAuthorization();

    app.MapControllers();

    app.SeedCustomerData()
        .Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information($"Shut down Customer API complete");
    Log.CloseAndFlush();
}
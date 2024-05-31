using Serilog;
using Common.Logging;
using Inventory.Product.API.Extensions;


var builder = WebApplication.CreateBuilder(args);

Log.Information("Starting Iventory API up");


try
{
    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddInfrastructureServices();
    builder.Services.ConfigureMongoDbClient();
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{builder.Environment.ApplicationName} v1"));
    }

    //app.UseHttpsRedirection();


    app.UseAuthorization();

    //app.MapControllers();
    app.MapDefaultControllerRoute();

    app.MigrateDatabase().Run();

}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, messageTemplate: "Unhandeld expection");
}

finally
{
    Log.Information(messageTemplate: "Shut down Product API complete");
    Log.CloseAndFlush();
}
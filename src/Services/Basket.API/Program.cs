using Serilog;
using Common.Logging;
using Basket.API.Extensitons;
using Basket.API;


Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

Log.Information("Starting Basket API up");


try
{
    builder.Host.UseSerilog(Serilogger.Configure);
    builder.Host.AddAppConfigurations();
    //builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
    // Add services to the container.
    builder.Services.ConfigureServices();
    builder.Services.ConfigureRedis(builder.Configuration);
    builder.Services.ConfigureGrpcServices();
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);



    // configure MassTransit
    builder.Services.ConfigureMassTransit();
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();





    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();


    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, messageTemplate: "Unhandeld expection");
}

finally
{
    Log.Information(messageTemplate: "Shut down Basket API complete");
    Log.CloseAndFlush();
}
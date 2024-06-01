using Common.Logging;
using Infrastructure.MiddleWares;
using Ocelot.Middleware;
using OcelotApiGw.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
var builder = WebApplication.CreateBuilder(args);

Log.Information("Starting Ocelot API up");


// Add services to the container.
try
{
    builder.Host.AddAppConfigurations();
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.ConfigureOcelot(builder.Configuration);
    builder.Services.ConfigureCors(builder.Configuration);
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        //app.UseSwagger();
        //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{builder.Environment.ApplicationName} v1"));
    }

    app.UseCors("CorsPolicy");

    app.UseMiddleware<ErrorWrappingMiddleware>();
    app.UseAuthentication();
    //app.UseHttpsRedirection();

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/",context =>
        {
            //await context.Response.WriteAsync($"Hello TanPhu");
            context.Response.Redirect("swagger/index.html");
            return Task.CompletedTask;
        });
    });


    app.UseSwaggerForOcelotUI(
        opt => { opt.PathToSwaggerGenerator = "/swagger/docs"; });

    await app.UseOcelot();

    app.Run();

}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;
    Log.Fatal(ex, messageTemplate: "Unhandeld expection");
}

finally
{
    Log.Information(messageTemplate: "Shut down Ocelot API complete");
    Log.CloseAndFlush();
}
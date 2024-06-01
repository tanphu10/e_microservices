﻿using Infrastructure.MiddleWares;

namespace Product.API.Extensions;

public static class ApplicationExtension
{
    public static void UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseMiddleware<ErrorWrappingMiddleware>();
        app.UseAuthentication();
        app.UseRouting();
        //app.UseHttpsRedirection(); // for production only
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
    }
}

using Customer.API.Repositories.Interfaces;
using Customer.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos.Customer;

namespace Customer.API.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public static class CustomerController
    {
        public static void MapCustomerAPI(this WebApplication app)
        {
            app.MapGet("/api/customers", async (ICustomerService customerService) => await customerService.GetCustomersAsync());
            app.MapGet("/api/customers/{username}", async (string username, ICustomerService customerService) =>
            {
                var result = await customerService.GetCustomerByUsernameAsync(username);
                return result != null ? Results.Ok(result) : Results.NotFound();
            });
            //app.MapPost("/api/customers", async (Customer.API.Entities.Customer customer, ICustomerRepository customerRepository) =>
            //{
            //    customerRepository.CreateAsync(customer);
            //    customerRepository.SaveChangesAsync();

            //});
            //    app.MapPut("/api/customers/put/{id}", async (int id, CustomerDto updatecustomer, ICustomerRepository
            //customerRepository) =>
            //    {
            //        var customer = await customerRepository.FindByCondition(x => x.Id.Equals(id)).SingleOrDefaultAsync();
            //        if (customer == null) return Results.NotFound();



            //        return Results.Ok();
            //    });
            //app.MapDelete("/api/customers/{id}", async (int id, ICustomerRepository customerRepository) =>
            //{

            //    var customer = await customerRepository.FindByCondition(x => x.Id.Equals(id)).SingleOrDefaultAsync();
            //    if (customer == null) return Results.NotFound();
            //    await customerRepository.DeleteAsync(customer);
            //    await customerRepository.SaveChangesAsync();
            //    return Results.NoContent();
            //});
        }
    }
}

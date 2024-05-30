using Customer.API.Persistence;
using Customer.API.Repositories.Interfaces;
using Infrastructure.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Customer.API.Repositories
{
    public class CustomerRepository : RepositoryQueryBase<Entities.Customer, int, CustomerContext>, ICustomerRepository
    {
        public CustomerRepository(CustomerContext dbContext) : base(dbContext)
        { }
        
        public async Task<Entities.Customer> GetCustomerByUserNameAsync(string username) =>
        await     FindByCondition(x => x.UserName.Equals(username))
                .SingleOrDefaultAsync();


        public async Task<IEnumerable<Entities.Customer>> GetCustomersAsync()
       => await FindAll().ToListAsync();
    }
}

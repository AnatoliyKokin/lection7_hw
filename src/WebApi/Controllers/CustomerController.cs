using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace WebApi.Controllers
{
    [Route("customers")]
    public class CustomerController : Controller
    {
        private readonly Models.CustomerDbContext mCustomerDbContext;
        public CustomerController(Models.CustomerDbContext context)
        {
            mCustomerDbContext = context;
        }

        [HttpGet("{id:long}")]
        public async Task<Customer> GetCustomerAsync([FromRoute] long id)
        {
            var customer = await mCustomerDbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
            }

            return customer;
        }

        [HttpPost("")]
        public async Task<long> CreateCustomerAsync([FromBody] Customer customer)
        {
            var entity1 = await mCustomerDbContext.Customers.FirstOrDefaultAsync(c =>
                (c.Id == customer.Id));
            if (entity1!=null)
            {
                Response.StatusCode = StatusCodes.Status409Conflict;
                return entity1.Id;
            }

            await mCustomerDbContext.Customers.AddAsync(customer);
            await mCustomerDbContext.SaveChangesAsync();
            Response.StatusCode = StatusCodes.Status201Created;
            return customer.Id;

        }
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace WebClient
{
    static class Program
    {
        static IConfigurationRoot ConfigurationRoot;

        static Program()
        {
            ConfigurationRoot = new ConfigurationBuilder().
            SetBasePath(Directory.GetCurrentDirectory()).
            AddJsonFile("appsettings.json").
            Build();
        }

        static async Task Main(string[] args)
        {
            string? uriStr = ConfigurationRoot.GetConnectionString("DefaultConnection");

            if (uriStr == null)
            {
                Console.WriteLine("Wrong connection string (appsettings.json)");
                return;
            }

            using (var client = new CustomerClient(new Uri((string)uriStr)))
            {
                bool exitFlag = false;
                while (!exitFlag)
                {
                    PrintHelp();
                    char key = Console.ReadKey().KeyChar;
                    if (key == 'g' || key == 'G')
                    {
                        Console.WriteLine();
                        await ExecuteGetCustomerAsync(client);
                    }
                    else if (key == 'p' || key == 'P')
                    {
                        Console.WriteLine();
                        await ExecutePostCustomerAsync(client, RandomCustomer());
                    }
                    else if (key == 'x' || key == 'X')
                    {
                        exitFlag = true;
                    }
                    else 
                    {
                        Console.WriteLine();
                        Console.WriteLine("Wrong key entered");
                    }
                }

            }

        }

        private static CustomerCreateRequest RandomCustomer()
        {
            return new CustomerCreateRequest(Faker.Name.First(), Faker.Name.Last());
        }

        static async Task ExecuteGetCustomerAsync(CustomerClient client)
        {
            bool entered = false;
            long id = 0;
            while (!entered)
            {
                Console.WriteLine("Enter customer id:");
                entered = long.TryParse(Console.ReadLine(), out id);
            }
            await GetCustomerAsync(client, id);
        }


        static async Task GetCustomerAsync(CustomerClient client, long id)
        {
            Customer? customer = await client.GetAsync(id);
            if (customer != null)
            {
                Console.WriteLine("Result: "+customer.Firstname + " " + customer.Lastname);
            }
            else
            {
                Console.WriteLine("Id " + id + " not found");
            }
        }

        static async Task ExecutePostCustomerAsync(CustomerClient client, CustomerCreateRequest request)
        {
            long id = await client.PostAsync(request);
            Console.WriteLine("Added new customer with id " + id);
            await GetCustomerAsync(client, id);
        }

        static void PrintHelp()
        {
            Console.WriteLine("g - get customer with specified id");
            Console.WriteLine("p - post random generated customer");
            Console.WriteLine("x - exit");
            Console.WriteLine("Press selected key:");
        }

    }
}
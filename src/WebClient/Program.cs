using System;
using System.Threading.Tasks;
using CommandLine;
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

            var parseResult = CommandLine.Parser.Default.ParseArguments<Options>(args);

            await parseResult.WithParsedAsync<Options>(RunAsync);

        }

        private static CustomerCreateRequest RandomCustomer()
        {
            return new CustomerCreateRequest(Faker.Name.First(), Faker.Name.Last());
        }

        static async Task RunAsync(Options options)
        {
            string? uriStr = ConfigurationRoot.GetConnectionString("DefaultConnection");

            if (uriStr == null)
            {
                Console.WriteLine("Неверно задан адрес подключения (appsettings.json)");
                return;
            }

            using (var client = new CustomerClient(new Uri((string)uriStr)))
            {

                if (options.Id != null)
                {
                    await GetCustomerAsync(client, (long)options.Id);
                }

                if (options.PostRequired)
                {
                    await PostCustomerAsync(client, RandomCustomer());
                }

            }

        }

        static async Task GetCustomerAsync(CustomerClient client, long id)
        {
            Customer? customer = await client.GetAsync(id);
            if (customer != null)
            {
                Console.WriteLine(customer.Firstname + " " + customer.Lastname);
            }
            else
            {
                Console.WriteLine("Id " + id + " not found");
            }
        }

        static async Task PostCustomerAsync(CustomerClient client, CustomerCreateRequest request)
        {
            long id = await client.PostAsync(request);
            Console.WriteLine("Added new customer with id " + id);
            await GetCustomerAsync(client, id);
        }

    }
}
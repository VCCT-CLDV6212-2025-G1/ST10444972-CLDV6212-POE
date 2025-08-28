using Azure.Data.Tables;
using CLDV6212_ST10444972_POE.Models;

namespace CLDV6212_ST10444972_POE.Services
{
    public class TableStorageService
    {
        private readonly TableServiceClient _tableServiceClient;

        public TableStorageService(string connectionString)
        {
            _tableServiceClient = new TableServiceClient(connectionString);
        }

        // Customer CRUD Operations
        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            var tableClient = _tableServiceClient.GetTableClient("customers");
            await tableClient.CreateIfNotExistsAsync();
            await tableClient.AddEntityAsync(customer);
            return customer;
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            var tableClient = _tableServiceClient.GetTableClient("customers");
            await tableClient.CreateIfNotExistsAsync();
            var customers = new List<Customer>();
            await foreach (var customer in tableClient.QueryAsync<Customer>())
            {
                customers.Add(customer);
            }
            return customers;
        }

        public async Task<Customer?> GetCustomerAsync(string id)
        {
            var tableClient = _tableServiceClient.GetTableClient("customers");
            try
            {
                var response = await tableClient.GetEntityAsync<Customer>("Customer", id);
                return response.Value;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            var tableClient = _tableServiceClient.GetTableClient("customers");
            await tableClient.UpdateEntityAsync(customer, customer.ETag);
            return customer;
        }

        public async Task DeleteCustomerAsync(string id)
        {
            var tableClient = _tableServiceClient.GetTableClient("customers");
            await tableClient.DeleteEntityAsync("Customer", id);
        }

        // Product CRUD Operations
        public async Task<Product> CreateProductAsync(Product product)
        {
            var tableClient = _tableServiceClient.GetTableClient("products");
            await tableClient.CreateIfNotExistsAsync();
            await tableClient.AddEntityAsync(product);
            return product;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            var tableClient = _tableServiceClient.GetTableClient("products");
            await tableClient.CreateIfNotExistsAsync();
            var products = new List<Product>();
            await foreach (var product in tableClient.QueryAsync<Product>())
            {
                products.Add(product);
            }
            return products;
        }

        public async Task<Product?> GetProductAsync(string id)
        {
            var tableClient = _tableServiceClient.GetTableClient("products");
            try
            {
                var response = await tableClient.GetEntityAsync<Product>("Product", id);
                return response.Value;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            var tableClient = _tableServiceClient.GetTableClient("products");
            await tableClient.UpdateEntityAsync(product, product.ETag);
            return product;
        }

        public async Task DeleteProductAsync(string id)
        {
            var tableClient = _tableServiceClient.GetTableClient("products");
            await tableClient.DeleteEntityAsync("Product", id);
        }
    }
}
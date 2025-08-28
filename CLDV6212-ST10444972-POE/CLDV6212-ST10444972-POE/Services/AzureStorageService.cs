using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Files.Shares;
using Azure.Storage.Sas;
using CLDV6212_ST10444972_POE.Models;

namespace CLDV6212_ST10444972_POE.Services
{
    public class AzureStorageService
    {
        private readonly TableServiceClient _tableServiceClient;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly QueueServiceClient _queueServiceClient;
        private readonly ShareServiceClient _shareServiceClient;

        public AzureStorageService(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Azure Storage connection string is required");
                
            _tableServiceClient = new TableServiceClient(connectionString);
            _blobServiceClient = new BlobServiceClient(connectionString);
            _queueServiceClient = new QueueServiceClient(connectionString);
            _shareServiceClient = new ShareServiceClient(connectionString);
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            var tableClient = _tableServiceClient.GetTableClient("customers");
            await tableClient.CreateIfNotExistsAsync();
            await tableClient.AddEntityAsync(customer);
        }

        public async Task AddProductAsync(Product product)
        {
            var tableClient = _tableServiceClient.GetTableClient("products");
            await tableClient.CreateIfNotExistsAsync();
            await tableClient.AddEntityAsync(product);
        }

        public async Task<List<Customer>> GetCustomersAsync()
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

        public async Task<List<Product>> GetProductsAsync()
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

        public async Task<ProductMedia> UploadProductMediaAsync(IFormFile file, string productId, string productName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("product-media");
            await containerClient.CreateIfNotExistsAsync();
            
            var blobName = $"{productId}_{Guid.NewGuid()}_{file.FileName}";
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(file.OpenReadStream(), true);
            
            return new ProductMedia
            {
                ProductId = productId,
                ProductName = productName,
                FileName = file.FileName,
                BlobUrl = GenerateSasUrl(blobClient)
            };
        }

        public async Task<List<ProductMedia>> GetProductMediaAsync()
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("product-media");
            await containerClient.CreateIfNotExistsAsync();
            var mediaList = new List<ProductMedia>();
            
            await foreach (var blob in containerClient.GetBlobsAsync())
            {
                var parts = blob.Name.Split('_');
                if (parts.Length >= 3)
                {
                    var blobClient = containerClient.GetBlobClient(blob.Name);
                    mediaList.Add(new ProductMedia
                    {
                        ProductId = parts[0],
                        FileName = string.Join("_", parts.Skip(2)),
                        BlobUrl = GenerateSasUrl(blobClient)
                    });
                }
            }
            return mediaList;
        }

        private string GenerateSasUrl(BlobClient blobClient)
        {
            if (blobClient.CanGenerateSasUri)
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = blobClient.BlobContainerName,
                    BlobName = blobClient.Name,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(24)
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);
                return blobClient.GenerateSasUri(sasBuilder).ToString();
            }
            return blobClient.Uri.ToString();
        }

        public async Task SendMessageAsync(string message)
        {
            var queueClient = _queueServiceClient.GetQueueClient("orders");
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(message);
        }

        public async Task<List<string>> GetMessagesAsync()
        {
            var queueClient = _queueServiceClient.GetQueueClient("orders");
            await queueClient.CreateIfNotExistsAsync();
            var messages = new List<string>();
            var receivedMessages = await queueClient.PeekMessagesAsync(maxMessages: 10);
            foreach (var message in receivedMessages.Value)
            {
                messages.Add(message.MessageText);
            }
            return messages;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var shareClient = _shareServiceClient.GetShareClient("contracts");
            await shareClient.CreateIfNotExistsAsync();
            var directoryClient = shareClient.GetRootDirectoryClient();
            var fileClient = directoryClient.GetFileClient(file.FileName);
            await fileClient.CreateAsync(file.Length);
            await fileClient.UploadAsync(file.OpenReadStream());
            return fileClient.Uri.ToString();
        }

        public async Task<List<string>> GetFilesAsync()
        {
            var shareClient = _shareServiceClient.GetShareClient("contracts");
            await shareClient.CreateIfNotExistsAsync();
            var directoryClient = shareClient.GetRootDirectoryClient();
            var files = new List<string>();
            await foreach (var item in directoryClient.GetFilesAndDirectoriesAsync())
            {
                if (!item.IsDirectory)
                {
                    files.Add(item.Name);
                }
            }
            return files;
        }
    }
}
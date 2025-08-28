using Azure;
using Azure.Data.Tables;

namespace CLDV6212_ST10444972_POE.Models
{
    public class Customer : ITableEntity
    {
        public string PartitionKey { get; set; } = "Customer";
        public string RowKey { get; set; } = Guid.NewGuid().ToString();
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string Id => RowKey;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
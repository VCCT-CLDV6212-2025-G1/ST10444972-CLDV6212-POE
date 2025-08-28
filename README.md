# ABC Retailers - Azure Cloud Management System

A comprehensive retail management web application built with ASP.NET Core MVC and Azure Storage Services. This application provides end-to-end management of customers, products, media, orders, and contracts through Azure cloud services.

## Features

- 🛍️ **Customer & Product Management**: CRUD operations using Azure Tables
- 📸 **Product Media Gallery**: Image management with Azure Blobs and SAS security
- 📦 **Order Processing**: Asynchronous queue system using Azure Queues
- 📄 **Contract Management**: Document storage and management with Azure Files

## Prerequisites

- .NET 8.0 SDK
- Azure Storage Account
- Visual Studio 2022 or Visual Studio Code
- Azure Storage Emulator (for local development)

## Quick Start

1. Clone the repository:
```powershell
git clone https://github.com/ST10444972-Ethan-Algeo/abc-retailers.git
cd abc-retailers
```

2. Update connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "AzureStorage": "your_connection_string_here"
  }
}
```

3. Run the application:
```powershell
dotnet restore
dotnet run
```

## Architecture

- **Frontend**: ASP.NET Core MVC with Bootstrap
- **Backend**: C# .NET 8 with Azure Storage SDKs
- **Storage**: Azure Tables, Blobs, Queues, and Files
- **Security**: SAS token authentication

## Auto-Provisioning

The system automatically creates required Azure resources on first use:
- Tables: `customers`, `products`
- Blob Container: `product-media`
- Queue: `orders`
- File Share: `contracts`

## Documentation

For detailed documentation, see:
- [Technical Documentation](Documentation/ST10444972-CLDV6212-POE-P1.md)
- [Azure Setup Guide](https://learn.microsoft.com/en-us/azure/storage/common/storage-account-create)

## Screenshots

![Web Application](Screenshots/WebApp.png)
*Main application interface*

## License

This project is licensed under the MIT License - see the LICENSE file for details.

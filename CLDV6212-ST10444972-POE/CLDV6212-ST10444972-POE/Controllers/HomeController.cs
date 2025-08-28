using System.Diagnostics;
using CLDV6212_ST10444972_POE.Models;
using CLDV6212_ST10444972_POE.Services;
using Microsoft.AspNetCore.Mvc;

namespace CLDV6212_ST10444972_POE.Controllers
{
    public class HomeController : Controller
    {
        private readonly AzureStorageService _storageService;
        private readonly TableStorageService _tableService;

        public HomeController(AzureStorageService storageService, TableStorageService tableService)
        {
            _storageService = storageService;
            _tableService = tableService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Customers = await _tableService.GetAllCustomersAsync();
            ViewBag.Products = await _tableService.GetAllProductsAsync();
            ViewBag.ProductMedia = await _storageService.GetProductMediaAsync();
            ViewBag.Messages = await _storageService.GetMessagesAsync();
            ViewBag.Files = await _storageService.GetFilesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(string name, string email)
        {
            await _tableService.CreateCustomerAsync(new Customer { Name = name, Email = email });
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(string name, decimal price)
        {
            await _tableService.CreateProductAsync(new Product { Name = name, Price = price });
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            await _tableService.DeleteCustomerAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            await _tableService.DeleteProductAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadProductMedia(IFormFile file, string productId)
        {
            if (file != null && !string.IsNullOrEmpty(productId))
            {
                var product = await _tableService.GetProductAsync(productId);
                if (product != null)
                {
                    await _storageService.UploadProductMediaAsync(file, productId, product.Name);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string message)
        {
            await _storageService.SendMessageAsync(message);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadContract(IFormFile file)
        {
            if (file != null)
                await _storageService.UploadFileAsync(file);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

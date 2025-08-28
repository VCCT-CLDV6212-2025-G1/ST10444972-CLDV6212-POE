using CLDV6212_ST10444972_POE.Services;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton(provider =>
{
    var connectionString = builder.Configuration["AzureStorage:ConnectionString"];
    return new AzureStorageService(connectionString);
});
builder.Services.AddSingleton(provider =>
{
    var connectionString = builder.Configuration["AzureStorage:ConnectionString"];
    return new TableStorageService(connectionString);
});
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["AzureStorage:ConnectionString1:blobServiceUri"]!).WithName("AzureStorage:ConnectionString1");
    clientBuilder.AddQueueServiceClient(builder.Configuration["AzureStorage:ConnectionString1:queueServiceUri"]!).WithName("AzureStorage:ConnectionString1");
    clientBuilder.AddTableServiceClient(builder.Configuration["AzureStorage:ConnectionString1:tableServiceUri"]!).WithName("AzureStorage:ConnectionString1");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

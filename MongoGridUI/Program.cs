using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MongoGridUI;
using MongoGridUI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient to point to the API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5001/") });

// Register API services
builder.Services.AddScoped<MongoGridUI.Features.Categories.CategoryService>();
builder.Services.AddScoped<MongoGridUI.Features.Products.ProductService>();

await builder.Build().RunAsync();

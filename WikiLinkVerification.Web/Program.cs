using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WikiLinkVerification.Web;
using WikiLinkVerification.Web.Services;
using WikiLinkVerification.Web.Services.Interfaces;
using AngleSharp;
using WikiLinkVerification.Web.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IUrlVerificationService, UrlVerificationService>();
builder.Services.AddSingleton(BrowsingContext.New(Configuration.Default));

await builder.Build().RunAsync();
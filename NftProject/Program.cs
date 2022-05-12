using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Nethereum.Erc20.Blazor;
using Nethereum.Metamask;
using Nethereum.Metamask.Blazor;
using Nethereum.UI;
using NftContractHandler;
using NftProject;
using NftProject.Data;

//mumbai testnet
//https://polygon-mumbai.infura.io/v3/57ac44fec97144dbb3589a9c2e41bd8c

//polygon mainnet
//https://polygon-mainnet.infura.io/v3/57ac44fec97144dbb3589a9c2e41bd8c
//private ethereum key 
//1e0e84f3ed2da30e1c274aae90a9859fdfaa4cb7b86cac4efde77476f1b94b90
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

//Dependency injections

builder.Services.AddSingleton<TestExample>();

builder.Services.AddServerSideBlazor();

// builder.Services.AddTransient<IJSRuntime>(serviceProvider =>
// {
//     return serviceProvider.GetService<JSRuntime>();
// });
//
builder.Services.AddScoped<IMetamaskInterop, MetamaskBlazorInterop>();
builder.Services.AddScoped<MetamaskInterceptor>();
builder.Services.AddScoped<MetamaskHostProvider>();
builder.Services.AddScoped<IEthereumHostProvider>(serviceProvider => 
{
    return serviceProvider.GetService<MetamaskHostProvider>();
}); 
builder.Services.AddScoped<IEthereumHostProvider, MetamaskHostProvider>();
builder.Services.AddScoped<NethereumSiweAuthenticatorService>();
builder.Services.AddValidatorsFromAssemblyContaining<Erc20Transfer>();   

// builder.Services.AddSingleton<IMetamaskInterop, MetamaskBlazorInterop>();
// builder.Services.AddSingleton<MetamaskInterceptor>();
// builder.Services.AddSingleton<MetamaskHostProvider>();
// builder.Services.AddSingleton<IEthereumHostProvider>(serviceProvider => 
// {
//     return serviceProvider.GetService<MetamaskHostProvider>();
// }); 
// builder.Services.AddSingleton<IEthereumHostProvider, MetamaskHostProvider>();
// builder.Services.AddSingleton<NethereumSiweAuthenticatorService>();
// builder.Services.AddValidatorsFromAssemblyContaining<Erc20Transfer>();   

builder.Services.AddControllersWithViews();
                                                                               


    // <PropertyGroup>
    // <UseRazorSourceGenerator>false</UseRazorSourceGenerator>
    // </PropertyGroup>

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute("default", "{controller=Nft}/{action=Index}/{id?}");
    endpoints.MapBlazorHub();
});

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Nethereum.Erc20.Blazor;
using Nethereum.Metamask;
using Nethereum.Metamask.Blazor;
using Nethereum.UI;
using NftContractHandler;
using NftProject.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddSingleton<TestExample>();

builder.Services.AddServerSideBlazor();
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
builder.Services.AddControllersWithViews();

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

app.MapRazorPages();

app.Run();
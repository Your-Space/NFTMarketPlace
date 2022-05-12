using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NftProject.Models;

namespace NftProject.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<AuctionSaleModel> AuctionSales { get; set; }
    
    public DbSet<AuctionInfoModel> AuctionInfo { get; set; }
}
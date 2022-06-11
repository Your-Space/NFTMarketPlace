using NftProject.Data;
using NftProject.Models;

namespace NftProject.Services;

public class AuctionService : IAuctionService
{
    private readonly ApplicationDbContext _context;


    public AuctionService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    //what do i have to get!
    public IEnumerable<AuctionSaleModel> GetAllSalesByIdAsync(string TokenId)
    {
        List<AuctionSaleModel> sales = _context.AuctionSales.
            Where(m => m.TokenId == TokenId).ToList();

        sales.RemoveAt(0);
        
        return sales;
    }

}
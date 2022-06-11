using System.Composition.Convention;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.CopyAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NftProject.Data;
using NftProject.Models;
using Org.BouncyCastle.Math;

namespace NftProject.AuctionHandler;

public class AuctionHandler
{
    private readonly ApplicationDbContext _context;
    private string tokenId;
    public string TokenId {
        get
        {
            return tokenId;
        }
        set
        {
            tokenId = value;
            // find all the bids
            // find the owner
            // find days left to the end of auction
        }
    }

    public string? Owner { get; set; }

    public void GetOwner()
    {
        Owner = _context.AuctionSales.FirstOrDefault(m => m.TokenId == tokenId)?.Address;
        
        
    }

    public void GetBids()
    {
        List<AuctionSaleModel> a = _context.AuctionSales.Where(m=> m.TokenId == tokenId).ToList();
        a.RemoveAt(0);
        
    }

    public void EstimateTimeLeft()
    {
        //run the timer. on frontend
    }
    //time left
    // owner
    // 

    AuctionHandler(ApplicationDbContext context)
    {
        _context = context;
    }
    
    
    //bids
    //auction owner
    //time
    //auction winner : null
    //context goes in here \DI
}
using System.Net;
using System.Numerics;
using System.Text.RegularExpressions;
using Contracts.Contracts.NFTMarketplace.ContractDefinition;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Nethereum.ABI.Util;
using Nethereum.Web3;
using Newtonsoft.Json;
using NftContractHandler;
using NftProject.Data;
using NftProject.Models;
using NftProject.Models.ViewModels;

namespace NftProject.Controllers;

public class SalesController : Controller
{
    private readonly TestExample _testExample;

    private readonly ApplicationDbContext _dbContext;
    
    // GET
    public SalesController(TestExample testExample, ApplicationDbContext dbContext)
    {
        _testExample = testExample;
        _dbContext = dbContext;
    }
    
    public async Task<IActionResult> Index(string? id)
    {
        if(id==null)
        {
            return NotFound();
        }

        NftViewItemModel res = (await GetNft(id))!;
         
        return View(res);
    }

    public async Task<IActionResult> AuctionSale(string? id)
    {
        if(id==null)
        {
            return NotFound();
        }

        NftViewItemModel res = (await GetNft(id))!;

        AuctionCreateModel view = new AuctionCreateModel();
        view.NftViewModel = res;
        view.AuctionInfo = new AuctionInfoModel();
        view.AuctionInfo.StartDate = DateTime.Now;

        return View(view);
    }

    [HttpPost]
    public async Task<IActionResult> AuctionSale(AuctionCreateModel auctionViewModel, string? id)
    {
        
        if(id==null)
        {
            return NotFound();
        }
        
        auctionViewModel.NftViewModel = (await GetNft(id))!;
        ModelState.Remove("NftViewModel");
        
        //start date cannot be >= final date
        //check it
        auctionViewModel.AuctionInfo.StartDate = DateTime.Now;
        if(auctionViewModel.AuctionInfo.StartDate >= auctionViewModel.AuctionInfo.FinalDate)
            ModelState.AddModelError("AuctionInfo.FinalDate", "Final date cannot be earlier than start date");

        auctionViewModel.AuctionInfo.TokenId = id;
        ModelState.Remove("AuctionInfo.TokenId");
        auctionViewModel.AuctionInfo.OwnerWalletAddress = _testExample.CurrentUserAddress;
        ModelState.Remove("AuctionInfo.OwnerWalletAddress");
        
        //push to database

        foreach (var modelState in ModelState.Values) {
            foreach (ModelError error in modelState.Errors) {
                Console.WriteLine(error.ErrorMessage);
            }
        }
        
        if (ModelState.IsValid)
        {
            _dbContext.Add(auctionViewModel.AuctionInfo);
            await _dbContext.SaveChangesAsync();
            await _testExample.ResellToken(BigInteger.Parse(id), BigInteger.Parse(auctionViewModel.AuctionInfo.startPrice));
            await _testExample.UpdatePriceAsync(BigInteger.Parse(id), 
                BigInteger.Parse(auctionViewModel.AuctionInfo.startPrice));
            
            return RedirectToAction(nameof(Index), "Nft");
        }

        return View(auctionViewModel);
    }

    public async Task<IActionResult> ClassicSale(string? id)
    {
        if(id==null)
        {
            return NotFound();
        }

        NftViewItemModel res = (await GetNft(id))!;
        
        return View(res);
    }
    
    [HttpPost]
    public async Task<IActionResult> ClassicSale(string? id, string? price)
    {
        if (id == null)
            return NotFound();

        if (string.IsNullOrEmpty(price)) // add model error
        {
            ViewBag.ErrorMessage("Price is required field.");
            return RedirectToAction("ClassicSale");
        }

        if (Regex.IsMatch(price, @"^\d+$")) // is number
        {
           //Console.WriteLine("I am Number");
           await _testExample.ResellToken(BigInteger.Parse(id), BigInteger.Parse(price));
           return RedirectToAction(nameof(Index), "Nft");
        }
        
        ViewBag.ErrorMessage("Price should be number.");

        return RedirectToAction("ClassicSale");
    }
    
    private async Task<NftMetadataModel> ReadJsonFromWeb(BigInteger id)
    {
        string uri = await _testExample.Service.TokenURIQueryAsync(id);

        //reading metadata json from web 
        HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
        WebResponse resp = request.GetResponse();
        StreamReader reader = new StreamReader(resp.GetResponseStream());
        string jsonText = reader.ReadToEnd();

        NftMetadataModel NftMetadata = JsonConvert.DeserializeObject<NftMetadataModel>(jsonText);
        // NftMetadata.Price = Web3.Convert.FromWei(BigInteger.Parse(NftMetadata.Price)).ToString();
        return NftMetadata;
    }
    
    private async Task<NftViewItemModel?> GetNft(string id)
    {
        List<MarketItem> marketItems = (await _testExample.Service.FetchMyNFTsQueryAsync()).ReturnValue1;
        
        MarketItem? marketItem = null;
        foreach (var item in marketItems)
        {
            
            if(item.TokenId != BigInteger.Parse(id))
                continue;
            marketItem = item;
        }

        if (marketItem == null)
            return null;
         
        NftViewItemModel res = new NftViewItemModel();
        res.NftMetadata = await ReadJsonFromWeb(marketItem.TokenId);
        res.MarketData = marketItem;

        return res;
    }
}
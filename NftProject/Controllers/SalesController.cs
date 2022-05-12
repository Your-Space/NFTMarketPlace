using System.Net;
using System.Numerics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Nethereum.ABI.Util;
using Newtonsoft.Json;
using NftContractHandler;
using NftProject.Contracts.NFTMarketplace.ContractDefinition;
using NftProject.Models;

namespace NftProject.Controllers;

public class SalesController : Controller
{
    private readonly TestExample _testExample;
    // GET
    public SalesController(TestExample testExample)
    {
        _testExample = testExample;
    }
    
    public async Task<IActionResult> Index(string? id)
    {
        if(id==null)
        {
            return NotFound();
        }

        List<MarketItem> marketItems = (await _testExample.Service.FetchMyNFTsQueryAsync()).ReturnValue1;
       

        MarketItem? marketItem = null;
        foreach (var item in marketItems)
        {
            //deserialize token uri
            if(item.TokenId != BigInteger.Parse(id))
                continue;
            marketItem = item;
        }

        if (marketItem == null)
            return NotFound();
         
        NftViewItemModel res = new NftViewItemModel();
        res.NftMetadata = await ReadJsonFromWeb(marketItem.TokenId);
        res.MarketData = marketItem;
         
        return View(res);
    }

    public async Task<IActionResult> AuctionSale(string? id)
    {
        if(id==null)
        {
            return NotFound();
        }

        List<MarketItem> marketItems = (await _testExample.Service.FetchMyNFTsQueryAsync()).ReturnValue1;
       

        MarketItem? marketItem = null;
        foreach (var item in marketItems)
        {
            //deserialize token uri
            if(item.TokenId != BigInteger.Parse(id))
                continue;
            marketItem = item;
        }

        if (marketItem == null)
            return NotFound();
         
        NftViewItemModel res = new NftViewItemModel();
        res.NftMetadata = await ReadJsonFromWeb(marketItem.TokenId);
        res.MarketData = marketItem;

        AuctionViewModel view = new AuctionViewModel();
        view.NftViewModel = res;
        
        view.AuctionInfo = new AuctionInfoModel();
        view.AuctionSale = new AuctionSaleModel();
            
        view.AuctionInfo.StartDate = DateTime.Today;
        view.AuctionInfo.TokenId = id;
        view.AuctionSale.TokenId = id;
        view.AuctionSale.Address = _testExample.CurrentUserAddress;
         
        return View(view);
    }

    [HttpPost]
    public async Task<IActionResult> AuctionSale(AuctionViewModel auctionViewModel)
    {
        return NotFound();
    }

    public async Task<IActionResult> ClassicSale(string? id)
    {
        if(id==null)
        {
            return NotFound();
        }

        List<MarketItem> marketItems = (await _testExample.Service.FetchMyNFTsQueryAsync()).ReturnValue1;
       

        MarketItem? marketItem = null;
        foreach (var item in marketItems)
        {
            //deserialize token uri
            if(item.TokenId != BigInteger.Parse(id))
                continue;
            marketItem = item;
        }

        if (marketItem == null)
            return NotFound();
         
        NftViewItemModel res = new NftViewItemModel();
        res.NftMetadata = await ReadJsonFromWeb(marketItem.TokenId);
        res.MarketData = marketItem;
        
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
        return NftMetadata;
    }
}
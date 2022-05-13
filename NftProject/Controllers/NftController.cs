using System.Net;
using System.Numerics;
using Ipfs.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NftContractHandler;
using NftProject.Contracts.NFTMarketplace.ContractDefinition;
using NftProject.Data;
using NftProject.Models;

namespace NftProject.Controllers;

public class NftController : Controller
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IpfsClient _client;
    private readonly TestExample _testExample;
    private readonly ApplicationDbContext _dbContext;

    public NftController(IWebHostEnvironment webHostEnvironment, TestExample testExample, ApplicationDbContext dbContext)
    {
        _webHostEnvironment = webHostEnvironment;
        _client = new IpfsClient("https://ipfs.infura.io:5001/api/v0");
        _testExample = testExample;
        _dbContext = dbContext;
    }

    //GET
    public async Task<IActionResult> Index()
    {
        var marketItems = (await _testExample.Service.FetchMarketItemsQueryAsync()).ReturnValue1;

        List<NftViewItemModel> metaSet = new List<NftViewItemModel>(marketItems.Count);
        
        foreach (var item in marketItems)
        {
            NftViewItemModel tmp = new NftViewItemModel();
            tmp.NftMetadata = await ReadJsonFromWeb(item.TokenId);
            tmp.MarketData = item;

            //check if it is on auction
            AuctionInfoModel? dbResult = _dbContext.AuctionInfo.FirstOrDefault(m => m.TokenId.Equals(item.TokenId.ToString()));
            
            if (dbResult != null)
                tmp.AuctionSale = true;

            metaSet.Add(tmp); 
        }
        
        
        return View(metaSet);
    }
    
    //Post
    //TODO: buy token || make it to redirect to details
    [HttpPost]
    public async Task<ActionResult> Index(string id, string price)
    {
        await _testExample.CreateMarketSale(BigInteger.Parse(id), BigInteger.Parse(price));
        return RedirectToAction(nameof(Index));
    }

    //GET
    public IActionResult Create()
    {
        return View();
    }
    
    //Post
    [HttpPost]
    [ValidateAntiForgeryToken]
     public async Task<IActionResult> Create([Bind("Name, Description, Price, FileUrl")] NftMetadataModel nftMetadata, IFormFile? fileUrl)
     {
            if (fileUrl != null)
                ModelState.Remove("FileUrl");
            
            
            if (ModelState.IsValid)
            {
                //add metadata to ipfs
                string url = await UploadToIpfs(nftMetadata, fileUrl);
                
                //create nft
                await _testExample.CreateToken(url, BigInteger.Parse(nftMetadata.Price));
                
                return RedirectToAction(nameof(Index));
            }
            
            return View(nftMetadata);
        }

     public async Task<IActionResult> Detail(string? id, string? nftaction)
     {

         if(id==null)
         {
             return NotFound();
         }

         List<MarketItem> marketItems;
         
         if(nftaction!.Equals("MyNfts"))
             marketItems = (await _testExample.Service.FetchMyNFTsQueryAsync()).ReturnValue1;
         else 
             marketItems = (await _testExample.Service.FetchMarketItemsQueryAsync()).ReturnValue1;

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
     
     //Get
     public async Task<IActionResult> MyNfts()
     {
        
         var marketItems = (await _testExample.Service.FetchMyNFTsQueryAsync()).ReturnValue1;
         List<NftViewItemModel> metaSet = new List<NftViewItemModel>(marketItems.Count);
         
         foreach (var item in marketItems)
         {
             NftViewItemModel tmp = new NftViewItemModel();
             tmp.NftMetadata = await ReadJsonFromWeb(item.TokenId);
             tmp.MarketData = item;
             
             metaSet.Add(tmp); 
         }

         return View(metaSet);
     }
     
     
     //Post
     [HttpPost]
     public async Task<ActionResult> MyNfts(string id, string price)
     {
         await _testExample.ResellToken(BigInteger.Parse(id), BigInteger.Parse(price));
         return RedirectToAction(nameof(Index));
     }
     
     public async Task<IActionResult> NftListed()
     {
         var marketItems = (await _testExample.Service.FetchItemsListedQueryAsync()).ReturnValue1;
         List<NftViewItemModel> metaSet = new List<NftViewItemModel>(marketItems.Count);
         
         foreach (var item in marketItems)
         {
             NftViewItemModel tmp = new NftViewItemModel();
             tmp.NftMetadata = await ReadJsonFromWeb(item.TokenId);
             tmp.MarketData = item;
             
             metaSet.Add(tmp); 
         }

         return View(metaSet);
     }
     
     
     //TODO: WORK WITH WEB MAYBE TO OTHER CLASS
     public async Task<string> UploadToIpfs([Bind("Name, Description, Price, FileUrl")] NftMetadataModel nftMetadata, IFormFile? image)
     {
         var name = Path.Combine(_webHostEnvironment.WebRootPath + "\\Images", Path.GetFileName(image.FileName));
         FileStream f = new FileStream(name, FileMode.Create);
         await image.CopyToAsync(f);
         f.Close();
         
         string path = "Images\\" + image.FileName;
         

         var hash = await _client.FileSystem.AddFileAsync($"wwwroot/{path}");
         nftMetadata.FileUrl = $"https://ipfs.infura.io/ipfs/{hash.Id}";
         System.IO.File.Delete(name);
         
         //Store other 
         string metadata = JsonConvert.SerializeObject(nftMetadata);
         hash = await _client.FileSystem.AddTextAsync(metadata);
         
         string url = $"https://ipfs.infura.io/ipfs/{hash.Id}";
         
         return url;
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
using System.Net;
using System.Numerics;
using Ipfs.Http;
using Microsoft.AspNetCore.Mvc;
using Nethereum.UI;
using Nethereum.Web3;
using Newtonsoft.Json;
using NftContractHandler;
using NftProject.Contracts.NFTMarketplace.ContractDefinition;
using NftProject.Models;

namespace NftProject.Controllers;

public class NftController : Controller
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IpfsClient client;
    private readonly TestExample _testExample;
    
    //have to have client to make everything done

    // GET
    public NftController(IWebHostEnvironment webHostEnvironment, TestExample testExample)
    {
        _webHostEnvironment = webHostEnvironment;
        client = new IpfsClient("https://ipfs.infura.io:5001/api/v0");
        _testExample = testExample;
    }

    public async Task<IActionResult> Index()
    {
        var marketItems = (await _testExample.Service.FetchMarketItemsQueryAsync()).ReturnValue1;
        // List<NftMetadataModel> metaSet = new List<NftMetadataModel>(marketItems.Count); //I NEED TOKEN ID
        List<NftViewModel> metaSet = new List<NftViewModel>(marketItems.Count); //I NEED TOKEN ID
        foreach (var item in marketItems)
        {
            NftViewModel tmp = new NftViewModel();
            tmp.NftMetadata = await ReadJsonFromWeb(item.TokenId);
            tmp.TokenId = item.TokenId;
            metaSet.Add(tmp); 
        }
        // List<NftViewModel> metaSet = new List<NftViewModel>(0);

        return View(metaSet);
    }
    
    public async Task<IActionResult> NftListed()
    {
        var marketItems = (await _testExample.Service.FetchItemsListedQueryAsync()).ReturnValue1;
        // List<NftMetadataModel> metaSet = new List<NftMetadataModel>(marketItems.Count); //I NEED TOKEN ID
        List<NftViewModel> metaSet = new List<NftViewModel>(marketItems.Count); //I NEED TOKEN ID
        foreach (var item in marketItems)
        {
            NftViewModel tmp = new NftViewModel();
            tmp.NftMetadata = await ReadJsonFromWeb(item.TokenId);
            tmp.TokenId = item.TokenId;
            metaSet.Add(tmp); 
        }

        return View(metaSet);
    }
    
    public async Task<IActionResult> MyNfts()
    {
        
         var marketItems = (await _testExample.Service.FetchMyNFTsQueryAsync()).ReturnValue1;
         List<NftViewModel> metaSet = new List<NftViewModel>(marketItems.Count); //I NEED TOKEN ID
         foreach (var item in marketItems)
         {
             NftViewModel tmp = new NftViewModel();
             tmp.NftMetadata = await ReadJsonFromWeb(item.TokenId);
             tmp.TokenId = item.TokenId;
             metaSet.Add(tmp); 
         }

        return View(metaSet);
        //return View(null);
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

    [HttpPost]
    public async Task<ActionResult> Index(int? id, string price)
    {
        //Console.WriteLine("I AM INSIDE " + id);
        await _testExample.CreateMarketSale(id, BigInteger.Parse(price));
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

     public async Task<string> UploadToIpfs([Bind("Name, Description, Price, FileUrl")] NftMetadataModel nftMetadata, IFormFile? image)
     {
         var name = Path.Combine(_webHostEnvironment.WebRootPath + "\\Images", Path.GetFileName(image.FileName));
         FileStream f = new FileStream(name, FileMode.Create);
         await image.CopyToAsync(f);
         f.Close();
         
         string path = "Images\\" + image.FileName;
         

         var hash = await client.FileSystem.AddFileAsync($"wwwroot/{path}");
         nftMetadata.FileUrl = $"https://ipfs.infura.io/ipfs/{hash.Id}";
         System.IO.File.Delete(name);
         //Store other 
         string metadata = JsonConvert.SerializeObject(nftMetadata);
         hash = await client.FileSystem.AddTextAsync(metadata);
         
         string url = $"https://ipfs.infura.io/ipfs/{hash.Id}";
         
         return url;
     }

     public async Task<IActionResult> Detail(int? id)
     {
         if(id==null)
         {
             return NotFound();
         }
    
         var marketItems = (await _testExample.Service.FetchMarketItemsQueryAsync()).ReturnValue1;

         MarketItem marketItem = null;
         foreach (var item in marketItems)
         {
             //deserialize token uri
             if(item.TokenId != id)
                 continue;
             marketItem = item;
         }

         if (marketItem == null)
             return NotFound();
         
         string uri = await _testExample.Service.TokenURIQueryAsync(marketItem.TokenId);
             
         //reading metadata json from web 
         HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
         WebResponse resp = request.GetResponse();
         StreamReader reader = new StreamReader(resp.GetResponseStream());
         string jsonText = reader.ReadToEnd();
             
         NftViewItemModel res = new NftViewItemModel();
         res.NftMetadata = JsonConvert.DeserializeObject<NftMetadataModel>(jsonText);
         res.MarketData = marketItem;
         
         return View(res);
     }
     
     [HttpPost]
     public async Task<ActionResult> MyNfts(int? id, string price)
     {
         //Console.WriteLine("I AM INSIDE " + id);
         await _testExample.ResellToken(id, BigInteger.Parse(price));
         return RedirectToAction(nameof(Index));
     }
}
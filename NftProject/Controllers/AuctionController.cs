using System.Net;
using System.Numerics;
using Contracts.Contracts.NFTMarketplace.ContractDefinition;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NftContractHandler;
using NftProject.Data;
using NftProject.Models;

namespace NftProject.Controllers
{
    public class AuctionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly TestExample _testExample;

        public AuctionController(ApplicationDbContext context, TestExample testExample)
        {
            _context = context;
            _testExample = testExample;
        }

        // GET: Auction
        public async Task<IActionResult> Index()
        {
            return View(await _context.AuctionInfo.ToListAsync());
        }

        // GET: Auction/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auctionInfoModel = await _context.AuctionInfo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (auctionInfoModel == null)
            {
                return NotFound();
            }

            return View(auctionInfoModel);
        }

        public async Task<IActionResult> SaleDetails(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }
    
            var marketItems = (await _testExample.Service.FetchMyNFTsQueryAsync()).ReturnValue1;

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
        
        // GET: Auction/Create
        public async Task<IActionResult> Create(int? id)
        {
            
            if(id==null)
            {
                return NotFound();
            }
    
            var marketItems = (await _testExample.Service.FetchMyNFTsQueryAsync()).ReturnValue1;

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
            
            AuctionViewModel result = new AuctionViewModel();
            
            result.NftViewModel = res;

            result.AuctionInfo = new AuctionInfoModel();
            result.AuctionSale = new AuctionSaleModel();
            
            result.AuctionInfo.StartDate = DateTime.Today;
            result.AuctionInfo.TokenId = id.ToString();
            result.AuctionSale.TokenId = id.ToString();
            result.AuctionSale.Address = _testExample.CurrentUserAddress;

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuctionViewModel auctionInfoModel)
        {

            if (ModelState.IsValid)
            {
                _context.Add(auctionInfoModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(auctionInfoModel); //change
        }
        
        // GET: Auction/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auctionInfoModel = await _context.AuctionInfo.FindAsync(id);
            if (auctionInfoModel == null)
            {
                return NotFound();
            }
            return View(auctionInfoModel);
        }

        // POST: Auction/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TokenId,MinimalBid,MinimalBidStep,StartDate,FinalDate")] AuctionInfoModel auctionInfoModel)
        {
            if (id != auctionInfoModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(auctionInfoModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuctionInfoModelExists(auctionInfoModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(auctionInfoModel);
        }

        // GET: Auction/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auctionInfoModel = await _context.AuctionInfo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (auctionInfoModel == null)
            {
                return NotFound();
            }

            return View(auctionInfoModel);
        }

        // POST: Auction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var auctionInfoModel = await _context.AuctionInfo.FindAsync(id);
            _context.AuctionInfo.Remove(auctionInfoModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuctionInfoModelExists(int id)
        {
            return _context.AuctionInfo.Any(e => e.Id == id);
        }
        
        //get
        public async Task<IActionResult> MakeBid(string? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            NftViewItemModel res = (await GetNft(id));

            AuctionViewModel model = new AuctionViewModel();
            model.NftViewModel = res;

            model.AuctionInfo = await _context.AuctionInfo.FirstOrDefaultAsync(m => m.TokenId == id)!;
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MakeBid(AuctionViewModel model, string? id)
        {
            if (String.IsNullOrEmpty(id))
                return NotFound();

            AuctionInfoModel info = _context.AuctionInfo.FirstOrDefault(m => m.TokenId == id)!;
            string minimalBidStep = info.MinimalBidStep;
            string lastPrice = _context.AuctionSales.OrderBy(m => m.Price).LastOrDefault(m => m.TokenId == id)?.Price ?? info.startPrice;

            ModelState.Remove("NftViewModel");
            ModelState.Remove("AuctionInfo");
            //bid cannot be < lastPrice + minimalBidStep 
            Console.WriteLine($"Sum  {BigInteger.Parse(lastPrice) + BigInteger.Parse(minimalBidStep)}");
            //check it
            if(BigInteger.Parse(model.AuctionSale.Price) < BigInteger.Parse(lastPrice) + BigInteger.Parse(minimalBidStep))
                ModelState.AddModelError("AuctionSale.Price", "Current bid is too small.");

            model.AuctionSale.TokenId = id;
            ModelState.Remove("AuctionSale.TokenId");
            model.AuctionSale.Address = _testExample.CurrentUserAddress;
            ModelState.Remove("AuctionSale.Address");
        
            //push to database

            foreach (var modelState in ModelState.Values) {
                foreach (ModelError error in modelState.Errors) {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
        
            if (ModelState.IsValid)
            {
                _context.AuctionSales.Add(model.AuctionSale);
                await _context.SaveChangesAsync();
                // await _testExample.UpdatePriceAsync(BigInteger.Parse(id), 
                //     BigInteger.Parse(model.AuctionSale.Price));
            
                return RedirectToAction(nameof(Index), "Nft");
            }

            NftViewItemModel nftViewModel = (await GetNft(id))!;
            model.AuctionInfo = info;
            model.NftViewModel = nftViewModel;
            //make bid, retrieve info. 
            
            // _testExample.Service.UpdateNftPriceRequestAndWaitForReceiptAsync(model., );
            return View(model);
        }


        private async Task<NftViewItemModel?> GetNft(string id)
        {
            //var marketItems = (await _testExample.Service.FetchMarketItemsQueryAsync()).ReturnValue1;
            List<MarketItem> marketItems = (await _testExample.Service.FetchMarketItemsQueryAsync()).ReturnValue1;

            MarketItem? marketItem = marketItems.FirstOrDefault(m => m.TokenId == BigInteger.Parse(id));

            if (marketItem == null)
                return null;
            
            NftViewItemModel res = new NftViewItemModel();
            res.NftMetadata = await ReadJsonFromWeb(BigInteger.Parse(id));
            res.MarketData = marketItem;

            return res;
        }
        
        private async Task<NftMetadataModel> ReadJsonFromWeb(BigInteger id)
        {
            string uri = await _testExample.Service.TokenURIQueryAsync(id);

            //reading metadata json from web 
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            WebResponse resp = request.GetResponse();
            StreamReader reader = new StreamReader(resp.GetResponseStream());
            
            string jsonText = reader.ReadToEnd();

            NftMetadataModel nftMetadata = JsonConvert.DeserializeObject<NftMetadataModel>(jsonText);
            return nftMetadata;
        }
    }
}

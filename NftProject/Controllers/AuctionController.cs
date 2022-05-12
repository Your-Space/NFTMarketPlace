#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NftContractHandler;
using NftProject.Contracts.NFTMarketplace.ContractDefinition;
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

        // POST: Auction/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create([Bind("Id,TokenId,MinimalBid,MinimalBidStep,StartDate,FinalDate")] AuctionInfoModel auctionInfoModel)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         _context.Add(auctionInfoModel);
        //         await _context.SaveChangesAsync();
        //         return RedirectToAction(nameof(Index));
        //     }
        //     return View(null); //change
        // }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuctionViewModel auctionInfoModel)
        {

            if (ModelState.IsValid)
            {
                //_context.Add(auctionInfoModel);
                //await _context.SaveChangesAsync();
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
    }
}

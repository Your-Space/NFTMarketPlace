
using System.ComponentModel.DataAnnotations;

namespace NftProject.Models;

public class AuctionViewModel
{
    [Required]
    public AuctionInfoModel AuctionInfo { get; set; }
    
    [Required]
    public AuctionSaleModel AuctionSale { get; set; }

    [Required]
    public NftViewItemModel NftViewModel { get; set; }
}
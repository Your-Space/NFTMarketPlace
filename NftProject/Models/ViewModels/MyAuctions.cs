using System.ComponentModel.DataAnnotations;

namespace NftProject.Models.ViewModels;

public class MyAuctions
{
    [Required]
    public List<AuctionInfoModel> AuctionInfoList { get; set; }

    
    [Required]
    public List<AuctionSaleModel> AuctionSalesList { get; set; }
}
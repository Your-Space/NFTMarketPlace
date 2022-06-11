using System.ComponentModel.DataAnnotations;

namespace NftProject.Models.ViewModels;

public class AuctionCreateModel
{
    [Required]
    public AuctionInfoModel AuctionInfo { get; set; }

    [Required]
    public NftViewItemModel NftViewModel { get; set; }
}
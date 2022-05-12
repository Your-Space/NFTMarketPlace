using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Build.Framework;

namespace NftProject.Models;

public class AuctionInfoModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string TokenId { get; set; } //actually big integer

    [Required]
    public string MinimalBid { get; set; }

    [Required]
    public string MinimalBidStep { get; set; }
    
    [Required]
    [Column(TypeName="Date")]
    public DateTime StartDate { get; set; }
    
    [Required]
    [Column(TypeName="Date")]
    public DateTime FinalDate { get; set; }
}
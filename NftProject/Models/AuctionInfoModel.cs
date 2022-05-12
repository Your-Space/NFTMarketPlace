using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NftProject.Models;

public class AuctionInfoModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string TokenId { get; set; } //actually big integer

    [Required]
    [DisplayName("Minimal Bid Step")]
    public string MinimalBidStep { get; set; }
    
    [Required]
    [DisplayName("Start date")]
    [Column(TypeName="Date")]
    public DateTime StartDate { get; set; }
    
    [Required]
    [DisplayName("Final date")]
    [Column(TypeName="Date")]
    public DateTime FinalDate { get; set; }
}
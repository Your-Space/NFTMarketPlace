using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace NftProject.Models;

public class AuctionSaleModel
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    public string Address { get; set; }

    [Required]
    [EmailAddress]
    [DisplayName("Email")]
    public string EmailAddress { get; set; }

    [Required]
    public string TokenId { get; set; } //actually big integer

    [Required]
    public string Price { get; set; }
}
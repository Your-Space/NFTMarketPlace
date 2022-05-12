using System.ComponentModel.DataAnnotations;

namespace NftProject.Models;

public class NftMetadataModel
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string Price { get; set; }
    [Required]
    public string FileUrl { get; set; }
}
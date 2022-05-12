using System.Numerics;
using NftProject.Contracts.NFTMarketplace.ContractDefinition;

namespace NftProject.Models;

public class NftViewModel
{
    public BigInteger TokenId { get; set; }
    public NftMetadataModel NftMetadata { get; set; }
}
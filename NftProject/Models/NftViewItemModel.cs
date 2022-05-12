using NftProject.Contracts.NFTMarketplace.ContractDefinition;

namespace NftProject.Models;

public class NftViewItemModel
{
    public MarketItem MarketData { get; set; }
    public NftMetadataModel NftMetadata { get; set; }
}
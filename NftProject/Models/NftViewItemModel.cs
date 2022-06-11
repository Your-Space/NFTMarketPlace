using Contracts.Contracts.NFTMarketplace.ContractDefinition;

namespace NftProject.Models;

public class NftViewItemModel
{
    public bool AuctionSale { get; set; } = false;
    public MarketItem MarketData { get; set; }
    public NftMetadataModel NftMetadata { get; set; }
}
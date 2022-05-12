using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace NftProject.Contracts.NFTMarketplace.ContractDefinition
{
    public partial class MarketItem : MarketItemBase { }

    public class MarketItemBase 
    {
        [Parameter("uint256", "tokenId", 1)]
        public virtual BigInteger TokenId { get; set; }
        [Parameter("address", "seller", 2)]
        public virtual string Seller { get; set; }
        [Parameter("address", "owner", 3)]
        public virtual string Owner { get; set; }
        [Parameter("uint256", "price", 4)]
        public virtual BigInteger Price { get; set; }
        [Parameter("bool", "sold", 5)]
        public virtual bool Sold { get; set; }
    }
}

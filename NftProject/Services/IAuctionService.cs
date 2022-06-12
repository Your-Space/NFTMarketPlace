using NftProject.Models;

namespace NftProject.Services
{
    public interface IAuctionService
    {
        void CheckWinningAuctions();
        IEnumerable<AuctionSaleModel> GetAllSalesByIdAsync(string TokenId);
        Task SendMessage(AuctionInfoModel owner);
        void SendMessage(AuctionInfoModel owner, AuctionSaleModel winner);
        void SendMessage(string receiver, string subject, string message);
    }
}
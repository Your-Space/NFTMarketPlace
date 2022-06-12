using System.Drawing;
using System.Net.Mail;
using Hangfire;
using NftProject.Data;
using NftProject.Models;

namespace NftProject.Services;

public class AuctionService : IAuctionService
{
    private readonly ApplicationDbContext _context;

    public AuctionService(ApplicationDbContext context)
    {
        _context = context;
    }

    //what do i have to get!
    public IEnumerable<AuctionSaleModel> GetAllSalesByIdAsync(string TokenId)
    {
        List<AuctionSaleModel> sales = _context.AuctionSales.Where(m => m.TokenId == TokenId).ToList();

        sales.RemoveAt(0);

        return sales;
    }

    public void CheckWinningAuctions()
    {
        List<AuctionInfoModel> finishedAuctions =
            _context.AuctionInfo.Where(m => m.FinalDate <= DateTime.Today).ToList();

        List<string> tokenIdsInfo = finishedAuctions.Select(m => m.TokenId).Distinct().ToList();

        List<AuctionSaleModel> auctionSaleModels = _context.AuctionSales
            .Where(m => tokenIdsInfo.Contains(m.TokenId)).ToList();

        List<string> tokenIdsBids = auctionSaleModels.Select(m => m.TokenId).Distinct().ToList();

        List<string> tokensWithoutBids = tokenIdsInfo.Except(tokenIdsBids).ToList();

        foreach (var item in tokensWithoutBids)
        {
            SendMessage(finishedAuctions.FirstOrDefault(m => m.TokenId.Equals(item))!);
        }

        //send message to token owner about failed auction and delete record from db.

        var tokensWithBids = tokenIdsInfo.Intersect(tokenIdsBids).ToList();
        foreach (var item in tokensWithBids)
        {
            AuctionSaleModel winner = auctionSaleModels
                .Where(m => m.TokenId == item)
                .OrderByDescending(m => m.Price).ToList()[0];
            SendMessage(finishedAuctions.FirstOrDefault(m => m.TokenId.Equals(item))!, winner);
        }
        //find the winner
    }

    public async Task SendMessage(AuctionInfoModel owner)
    {
        string ownerEmailAddress = owner.EmailAddress;
        //send message to owner
        SendMessage(ownerEmailAddress, "Auction NFT ended",
            $"We are sorry to inform that your NFT with id {owner.TokenId} has not been bid by anyone." +
            $" We have listed nft on market on classic sale with price {owner.startPrice} eth. Take care!");


        _context.AuctionInfo.Remove(owner);
        await _context.SaveChangesAsync();
    }

    public void SendMessage(AuctionInfoModel owner, AuctionSaleModel winner)
    {
        string ownerEmailAddress = owner.EmailAddress;
        string buyerEmailAddress = winner.EmailAddress;

        //send messages to both of them
        SendMessage(ownerEmailAddress, "Auction NFT ended successfully",
            $"Your NFT with id {owner.TokenId} has been purchased by {winner.Address} for {winner.Price} eth. Congrats!");

        SendMessage(buyerEmailAddress, "Your bid has won",
            $"Your bid for NFT with id {owner.TokenId} can be purchased for {winner.Price} eth. Congrats!");
    }

    public void SendMessage(string receiver, string subject, string message)
    {
        SmtpClient client = new SmtpClient();
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.EnableSsl = true;
        client.Host = "smtp.gmail.com";
        client.Port = 587;

        // setup Smtp authentication
        System.Net.NetworkCredential credentials =
            new System.Net.NetworkCredential("your_account@gmail.com", "yourpassword");
        client.UseDefaultCredentials = false;
        client.Credentials = credentials;

        MailMessage msg = new MailMessage();
        msg.From = new MailAddress("your_account@gmail.com");
        msg.To.Add(new MailAddress(receiver));

        msg.Subject = subject;
        msg.IsBodyHtml = false;
        msg.Body = message;

        try
        {
            client.Send(msg);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error {ex}");
        }
    }
}
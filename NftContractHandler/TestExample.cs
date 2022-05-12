using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Numerics;
using System.Resources;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3.Accounts;
using NftProject.Contracts.NFTMarketplace;
using NftProject.Contracts.NFTMarketplace.ContractDefinition;

namespace NftContractHandler;
using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;

public class TestExample
{
    public bool ConnectButtonClick { get; set; } = false;
    public NFTMarketplaceService Service { get; private set; }
    public BigInteger ListingPrice { get; }
    public string ContractAddress { get; set; }
    public string CurrentUserAddress { get; set; }
    private Web3 web3;
    public Web3 Web3
    {
        get
        {
            return web3;
        }
        set
        {
            web3 = value;
            Service = new NFTMarketplaceService(web3, ContractAddress);
        }
    }


    public TestExample()
    {
        //to type none into res
        // using (ResourceWriter res = new ResourceWriter(@".\NftResources.resources"))
        // {
        //     res.AddResource("contractAddress", "none");
        // }
        
        //----------------------//
        
         ResourceReader reader = new ResourceReader(@".\NftResources.resources");
         IDictionaryEnumerator dict = reader.GetEnumerator();
        
         dict.MoveNext(); // contractAddress
        
         //ManagedAccount account = new ManagedAccount("0x12890d2cce102216644c59dae5baed380d84830c", "password");
        // var privateKey = "0xb5b1870957d373ef0eeffecc6e4812c0fd08f554b37b233526acc331bf1544f7";
         //0x13F022d72158410433CBd66f5DD8bF6D2D129924
        // web3 = new Web3();
        //var account = new Account(privateKey);
         web3 = new Web3();
          // ManagedAccount account = new ManagedAccount("0x12890d2cce102216644c59dae5baed380d84830c", "password");
          // web3 = new Web3(account);
          // web3 = new Web3(account, "https://mainnet.infura.io/v3/57ac44fec97144dbb3589a9c2e41bd8c");
         if (dict.Value!.Equals("none"))
         {
             NFTMarketplaceDeployment deployment = new NFTMarketplaceDeployment();
             TransactionReceipt receipt = NFTMarketplaceService.DeployContractAndWaitForReceiptAsync(web3, deployment).Result;
             Service = new NFTMarketplaceService(web3, receipt.ContractAddress);
             reader.Close();
             using (ResourceWriter res = new ResourceWriter(@".\NftResources.resources"))
             {
                 res.AddResource("contractAddress", receipt.ContractAddress);
             }
         }
         else
         {
             Service = new NFTMarketplaceService(web3, (string) dict.Value);
         }
        
        ContractAddress = Service.ContractHandler.ContractAddress;
        Console.WriteLine($"Contract Address: {Service.ContractHandler.ContractAddress}");
        ListingPrice = Service.GetListingPriceQueryAsync().Result;
    }

    // public void setTest(Web3 web3)
    // {
    //     ResourceReader reader = new ResourceReader(@".\NftResources.resources");
    //     IDictionaryEnumerator dict = reader.GetEnumerator();
    //     dict.MoveNext();
    //     
    //     this.web3 = web3;
    //     Service = new NFTMarketplaceService(web3, (string) dict.Value);
    // }

    public async Task CreateToken(string uri, BigInteger price, BigInteger value)
    {
        var createTokenFunctionMessage = new CreateTokenFunction()
        {
            TokenURI = uri,
            Price = price,
            AmountToSend = value
        };

        await Service.ContractHandler.SendRequestAndWaitForReceiptAsync(createTokenFunctionMessage);
    }
    
    public async Task CreateToken(string uri, BigInteger price)
    {
        var createTokenFunctionMessage = new CreateTokenFunction()
        {
            TokenURI = uri,
            // Price = Web3.Convert.ToWei(price),
            Price = price,
            AmountToSend = ListingPrice
        };

        await Service.ContractHandler.SendRequestAndWaitForReceiptAsync(createTokenFunctionMessage);
    }

    public async Task CreateMarketSale(BigInteger? tokenId, BigInteger price)
    {
        if (tokenId == null) return;
        var createMarkerSaleMessage = new CreateMarketSaleFunction()
        {
            TokenId =  (BigInteger) tokenId,
            AmountToSend = price
        };

        await Service.ContractHandler.SendRequestAndWaitForReceiptAsync(createMarkerSaleMessage);
    }
    
    public async Task ResellToken(int? tokenId, BigInteger price)
    {
        if (tokenId == null) return;
        var resellTokenMessage = new ResellTokenFunction()
        {
            TokenId =  (BigInteger) tokenId,
            Price = price,
            AmountToSend = ListingPrice
        };

        await Service.ContractHandler.SendRequestAndWaitForReceiptAsync(resellTokenMessage);
    }

    public async Task FetchMarketItems(string uri, BigInteger price)
    {
        var createTokenFunctionMessage = new CreateTokenFunction()
        {
            TokenURI = uri,
            Price = price,
            AmountToSend = ListingPrice
        };

        await Service.ContractHandler.SendRequestAndWaitForReceiptAsync(createTokenFunctionMessage);
    }

    public async Task ContractDeployment()
    {
        // Console.WriteLine($"Contract Deployment Tx Status: {Receipt.Status.Value}");
        // Console.WriteLine($"Contract Address: {Service.ContractHandler.ContractAddress}");
        // Console.WriteLine($"Contract Address: {Receipt.ContractAddress}");
        // Console.WriteLine("");


        //var Upd = new UpdateListingPriceFunction() { ListingPrice = 1234};

        //await service.UpdateListingPriceRequestAndWaitForReceiptAsync(132413454332);

        //await web3.Eth.GetContractQueryHandler<UpdateListingPriceFunction>().QueryAsync<object>(receipt.ContractAddress, Upd);
        //await service.ContractHandler.SendRequestAsync(Upd);

        var listingPrice = await Service.GetListingPriceQueryAsync();


        var createTokenFunctionMessage = new CreateTokenFunction()
        {
            TokenURI = "https://www.mytokenlocation.com",
            Price = listingPrice,
            AmountToSend = listingPrice
        };
        //connect to one person 

        await Service.ContractHandler.SendRequestAndWaitForReceiptAsync(createTokenFunctionMessage);
        createTokenFunctionMessage.TokenURI = "https://www.mytokenlocation2.com";
        await Service.ContractHandler.SendRequestAndWaitForReceiptAsync(createTokenFunctionMessage);

        var createMarkerSaleMessage = new CreateMarketSaleFunction()
        {
            TokenId = 1,
            AmountToSend = listingPrice
        };

        await Service.ContractHandler.SendRequestAndWaitForReceiptAsync(createMarkerSaleMessage);

        var resellTokenMessage = new ResellTokenFunction()
        {
            TokenId = 1,
            Price = listingPrice,
            AmountToSend = listingPrice
        };

        await Service.ContractHandler.SendRequestAndWaitForReceiptAsync(resellTokenMessage);


        ///////////////

        FetchMarketItemsOutputDTO items = await Service.FetchMarketItemsQueryAsync();

        Console.WriteLine("\n\n");
        Console.WriteLine("Items 1");
        foreach (MarketItem item in items.ReturnValue1) 
        {
            printMarketItem(item);
            Console.WriteLine(await Service.TokenURIQueryAsync(item.TokenId));
        }

        FetchItemsListedOutputDTO itemsanother = await Service.FetchItemsListedQueryAsync();

        Console.WriteLine("\n\n");
        Console.WriteLine("Items 2");
        foreach (var item in itemsanother.ReturnValue1)
        {
            printMarketItem(item);
            Console.WriteLine(await Service.TokenURIQueryAsync(item.TokenId));
        }
        // FetchMyNFTsOutputDTO itemsanother1 = await Service.FetchMyNFTsQueryAsync();
        //
        // Console.WriteLine("\n\n");
        // Console.WriteLine("Items 3");
        // foreach (var item in itemsanother1.ReturnValue1)
        // {
        //     printMarketItem(item);
        //     Console.WriteLine(await Service.TokenURIQueryAsync(item.TokenId));
        // }
    }

    private void printMarketItem(MarketItem item) {
        Console.WriteLine();
        Console.WriteLine($"Price: {item.Price}");
        Console.WriteLine($"TokenId: {item.TokenId}");
        Console.WriteLine($"Seller: {item.Seller}");
        Console.WriteLine($"Owner: {item.Owner}");
    }
}

// See https://aka.ms/new-console-template for more information
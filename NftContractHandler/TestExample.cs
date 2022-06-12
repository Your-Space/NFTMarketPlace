using System.Collections;
using System.Numerics;
using System.Resources;
using Contracts.Contracts.NFTMarketplace;
using Contracts.Contracts.NFTMarketplace.ContractDefinition;
using Nethereum.RPC.Eth.DTOs;

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
    private Web3 _web3;
    public Web3 Web3
    {
        get
        {
            return _web3;
        }
        set
        {
            _web3 = value;
            Service = new NFTMarketplaceService(_web3, ContractAddress);
        }
    }

    //TODO: Read contract 
    public TestExample()
    {
        //When private node was only started
         CreateResourceFile();

         ResourceReader reader = new ResourceReader(@".\NftResources.resources");
         IDictionaryEnumerator dict = reader.GetEnumerator();
        
         dict.MoveNext(); // contractAddress
         
          _web3 = new Web3();
          
          //to deploy contract
          if (dict.Value!.Equals("none")) 
          {
              //TODO: get account from metamask or connect it to metamask | or deploy contract from null address
              ManagedAccount account = new ManagedAccount("0x12890d2cce102216644c59dae5baed380d84830c", "password");
             _web3 = new Web3(account);
             
             NFTMarketplaceDeployment deployment = new NFTMarketplaceDeployment();
             TransactionReceipt receipt = NFTMarketplaceService.DeployContractAndWaitForReceiptAsync(_web3, deployment).Result;
             Service = new NFTMarketplaceService(_web3, receipt.ContractAddress);
             reader.Close();
             using (ResourceWriter res = new ResourceWriter(@".\NftResources.resources"))
             {
                 res.AddResource("contractAddress", receipt.ContractAddress);
             }
         }
         else
         {
             Service = new NFTMarketplaceService(_web3, (string) dict.Value);
         }
        
        ContractAddress = Service.ContractHandler.ContractAddress;
        ListingPrice = Service.GetListingPriceQueryAsync().Result;
    }

    public void CreateResourceFile()
    {
        //to type none into res
        using (ResourceWriter res = new ResourceWriter(@".\NftResources.resources"))
        {
            res.AddResource("contractAddress", "none");
        }
    }


    //TODO: Change price as ethereum
    public async Task CreateToken(string uri, BigInteger price)
    {
        var createTokenFunctionMessage = new CreateTokenFunction()
        {
            TokenURI = uri,
            Price = Web3.Convert.ToWei(price),
            //Price = price,
            AmountToSend = ListingPrice
        };

        Console.WriteLine($"PRICE : {Web3.Convert.ToWei(price)}");
        
        await Service.ContractHandler.SendRequestAndWaitForReceiptAsync(createTokenFunctionMessage);
    }

    public async Task CreateMarketSale(BigInteger tokenId, BigInteger price)
    {
        
        var createMarkerSaleMessage = new CreateMarketSaleFunction()
        {
            TokenId = tokenId,  
            AmountToSend = Web3.Convert.ToWei(price),
        };

        await Service.ContractHandler.SendRequestAndWaitForReceiptAsync(createMarkerSaleMessage);
    }

    public async Task ResellToken(BigInteger tokenId, BigInteger price)
    {
        
        var resellTokenMessage = new ResellTokenFunction()
        {
            TokenId = tokenId,
            Price = Web3.Convert.ToWei(price),
            AmountToSend = ListingPrice
        };

        await Service.ContractHandler.SendRequestAndWaitForReceiptAsync(resellTokenMessage);
    }
    
    public async Task UpdatePriceAsync(BigInteger tokenId, BigInteger price)
    {
        
        var updatePrice = new UpdateNftPriceFunction()
        {
            TokenId = tokenId,
            Price = Web3.Convert.ToWei(price),
        };

        await Service.ContractHandler.SendRequestAndWaitForReceiptAsync(updatePrice);
    }
}

// See https://aka.ms/new-console-template for more information
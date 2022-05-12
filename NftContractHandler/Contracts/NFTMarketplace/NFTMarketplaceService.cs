using System.Numerics;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.RPC.Eth.DTOs;
using NftProject.Contracts.NFTMarketplace.ContractDefinition;

namespace NftProject.Contracts.NFTMarketplace
{
    public partial class NFTMarketplaceService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, NFTMarketplaceDeployment? nFTMarketplaceDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<NFTMarketplaceDeployment>().SendRequestAndWaitForReceiptAsync(nFTMarketplaceDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, NFTMarketplaceDeployment nFTMarketplaceDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<NFTMarketplaceDeployment>().SendRequestAsync(nFTMarketplaceDeployment);
        }

        public static async Task<NFTMarketplaceService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, NFTMarketplaceDeployment? nFTMarketplaceDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, nFTMarketplaceDeployment, cancellationTokenSource);
            return new NFTMarketplaceService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public NFTMarketplaceService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<string> ApproveRequestAsync(ApproveFunction approveFunction)
        {
             return ContractHandler.SendRequestAsync(approveFunction);
        }

        public Task<TransactionReceipt> ApproveRequestAndWaitForReceiptAsync(ApproveFunction approveFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(approveFunction, cancellationToken);
        }

        public Task<string> ApproveRequestAsync(string to, BigInteger tokenId)
        {
            var approveFunction = new ApproveFunction();
                approveFunction.To = to;
                approveFunction.TokenId = tokenId;
            
             return ContractHandler.SendRequestAsync(approveFunction);
        }

        public Task<TransactionReceipt> ApproveRequestAndWaitForReceiptAsync(string to, BigInteger tokenId, CancellationTokenSource cancellationToken = null)
        {
            var approveFunction = new ApproveFunction();
                approveFunction.To = to;
                approveFunction.TokenId = tokenId;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(approveFunction, cancellationToken);
        }

        public Task<BigInteger> BalanceOfQueryAsync(BalanceOfFunction balanceOfFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BalanceOfFunction, BigInteger>(balanceOfFunction, blockParameter);
        }

        
        public Task<BigInteger> BalanceOfQueryAsync(string owner, BlockParameter blockParameter = null)
        {
            var balanceOfFunction = new BalanceOfFunction();
                balanceOfFunction.Owner = owner;
            
            return ContractHandler.QueryAsync<BalanceOfFunction, BigInteger>(balanceOfFunction, blockParameter);
        }

        public Task<string> CreateMarketSaleRequestAsync(CreateMarketSaleFunction createMarketSaleFunction)
        {
             return ContractHandler.SendRequestAsync(createMarketSaleFunction);
        }

        public Task<TransactionReceipt> CreateMarketSaleRequestAndWaitForReceiptAsync(CreateMarketSaleFunction createMarketSaleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(createMarketSaleFunction, cancellationToken);
        }

        public Task<string> CreateMarketSaleRequestAsync(BigInteger tokenId)
        {
            var createMarketSaleFunction = new CreateMarketSaleFunction();
                createMarketSaleFunction.TokenId = tokenId;
            
             return ContractHandler.SendRequestAsync(createMarketSaleFunction);
        }

        public Task<TransactionReceipt> CreateMarketSaleRequestAndWaitForReceiptAsync(BigInteger tokenId, CancellationTokenSource cancellationToken = null)
        {
            var createMarketSaleFunction = new CreateMarketSaleFunction();
                createMarketSaleFunction.TokenId = tokenId;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(createMarketSaleFunction, cancellationToken);
        }

        public Task<string> CreateTokenRequestAsync(CreateTokenFunction createTokenFunction)
        {
             return ContractHandler.SendRequestAsync(createTokenFunction);
        }

        public Task<TransactionReceipt> CreateTokenRequestAndWaitForReceiptAsync(CreateTokenFunction createTokenFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(createTokenFunction, cancellationToken);
        }

        public Task<string> CreateTokenRequestAsync(string tokenURI, BigInteger price)
        {
            var createTokenFunction = new CreateTokenFunction();
                createTokenFunction.TokenURI = tokenURI;
                createTokenFunction.Price = price;
            
             return ContractHandler.SendRequestAsync(createTokenFunction);
        }

        public Task<TransactionReceipt> CreateTokenRequestAndWaitForReceiptAsync(string tokenURI, BigInteger price, CancellationTokenSource cancellationToken = null)
        {
            var createTokenFunction = new CreateTokenFunction();
                createTokenFunction.TokenURI = tokenURI;
                createTokenFunction.Price = price;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(createTokenFunction, cancellationToken);
        }

        public Task<FetchItemsListedOutputDTO> FetchItemsListedQueryAsync(FetchItemsListedFunction fetchItemsListedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<FetchItemsListedFunction, FetchItemsListedOutputDTO>(fetchItemsListedFunction, blockParameter);
        }

        public Task<FetchItemsListedOutputDTO> FetchItemsListedQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<FetchItemsListedFunction, FetchItemsListedOutputDTO>(null, blockParameter);
        }

        public Task<FetchMarketItemsOutputDTO> FetchMarketItemsQueryAsync(FetchMarketItemsFunction fetchMarketItemsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<FetchMarketItemsFunction, FetchMarketItemsOutputDTO>(fetchMarketItemsFunction, blockParameter);
        }

        public Task<FetchMarketItemsOutputDTO> FetchMarketItemsQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<FetchMarketItemsFunction, FetchMarketItemsOutputDTO>(null, blockParameter);
        }

        public Task<FetchMyNFTsOutputDTO> FetchMyNFTsQueryAsync(FetchMyNFTsFunction fetchMyNFTsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<FetchMyNFTsFunction, FetchMyNFTsOutputDTO>(fetchMyNFTsFunction, blockParameter);
        }

        public Task<FetchMyNFTsOutputDTO> FetchMyNFTsQueryAsync(BlockParameter blockParameter = null)
        // public Task<string> FetchMyNFTsQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<FetchMyNFTsFunction, FetchMyNFTsOutputDTO>(null, blockParameter);
            // return ContractHandler.QueryAsync<FetchMyNFTsFunction, string>(null, blockParameter);
        }

        public Task<string> GetApprovedQueryAsync(GetApprovedFunction getApprovedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetApprovedFunction, string>(getApprovedFunction, blockParameter);
        }

        
        public Task<string> GetApprovedQueryAsync(BigInteger tokenId, BlockParameter blockParameter = null)
        {
            var getApprovedFunction = new GetApprovedFunction();
                getApprovedFunction.TokenId = tokenId;
            
            return ContractHandler.QueryAsync<GetApprovedFunction, string>(getApprovedFunction, blockParameter);
        }

        public Task<BigInteger> GetListingPriceQueryAsync(GetListingPriceFunction getListingPriceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetListingPriceFunction, BigInteger>(getListingPriceFunction, blockParameter);
        }

        
        public Task<BigInteger> GetListingPriceQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetListingPriceFunction, BigInteger>(null, blockParameter);
        }

        public Task<bool> IsApprovedForAllQueryAsync(IsApprovedForAllFunction isApprovedForAllFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsApprovedForAllFunction, bool>(isApprovedForAllFunction, blockParameter);
        }

        
        public Task<bool> IsApprovedForAllQueryAsync(string owner, string @operator, BlockParameter blockParameter = null)
        {
            var isApprovedForAllFunction = new IsApprovedForAllFunction();
                isApprovedForAllFunction.Owner = owner;
                isApprovedForAllFunction.Operator = @operator;
            
            return ContractHandler.QueryAsync<IsApprovedForAllFunction, bool>(isApprovedForAllFunction, blockParameter);
        }

        public Task<string> NameQueryAsync(NameFunction nameFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NameFunction, string>(nameFunction, blockParameter);
        }

        
        public Task<string> NameQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NameFunction, string>(null, blockParameter);
        }

        public Task<string> OwnerOfQueryAsync(OwnerOfFunction ownerOfFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerOfFunction, string>(ownerOfFunction, blockParameter);
        }

        
        public Task<string> OwnerOfQueryAsync(BigInteger tokenId, BlockParameter blockParameter = null)
        {
            var ownerOfFunction = new OwnerOfFunction();
                ownerOfFunction.TokenId = tokenId;
            
            return ContractHandler.QueryAsync<OwnerOfFunction, string>(ownerOfFunction, blockParameter);
        }

        public Task<string> ResellTokenRequestAsync(ResellTokenFunction resellTokenFunction)
        {
             return ContractHandler.SendRequestAsync(resellTokenFunction);
        }

        public Task<TransactionReceipt> ResellTokenRequestAndWaitForReceiptAsync(ResellTokenFunction resellTokenFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(resellTokenFunction, cancellationToken);
        }

        public Task<string> ResellTokenRequestAsync(BigInteger tokenId, BigInteger price)
        {
            var resellTokenFunction = new ResellTokenFunction();
                resellTokenFunction.TokenId = tokenId;
                resellTokenFunction.Price = price;
            
             return ContractHandler.SendRequestAsync(resellTokenFunction);
        }

        public Task<TransactionReceipt> ResellTokenRequestAndWaitForReceiptAsync(BigInteger tokenId, BigInteger price, CancellationTokenSource cancellationToken = null)
        {
            var resellTokenFunction = new ResellTokenFunction();
                resellTokenFunction.TokenId = tokenId;
                resellTokenFunction.Price = price;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(resellTokenFunction, cancellationToken);
        }

        public Task<string> SafeTransferFromRequestAsync(SafeTransferFromFunction safeTransferFromFunction)
        {
             return ContractHandler.SendRequestAsync(safeTransferFromFunction);
        }

        public Task<TransactionReceipt> SafeTransferFromRequestAndWaitForReceiptAsync(SafeTransferFromFunction safeTransferFromFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(safeTransferFromFunction, cancellationToken);
        }

        public Task<string> SafeTransferFromRequestAsync(string from, string to, BigInteger tokenId)
        {
            var safeTransferFromFunction = new SafeTransferFromFunction();
                safeTransferFromFunction.From = from;
                safeTransferFromFunction.To = to;
                safeTransferFromFunction.TokenId = tokenId;
            
             return ContractHandler.SendRequestAsync(safeTransferFromFunction);
        }

        public Task<TransactionReceipt> SafeTransferFromRequestAndWaitForReceiptAsync(string from, string to, BigInteger tokenId, CancellationTokenSource cancellationToken = null)
        {
            var safeTransferFromFunction = new SafeTransferFromFunction();
                safeTransferFromFunction.From = from;
                safeTransferFromFunction.To = to;
                safeTransferFromFunction.TokenId = tokenId;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(safeTransferFromFunction, cancellationToken);
        }

        public Task<string> SafeTransferFromRequestAsync(SafeTransferFrom1Function safeTransferFrom1Function)
        {
             return ContractHandler.SendRequestAsync(safeTransferFrom1Function);
        }

        public Task<TransactionReceipt> SafeTransferFromRequestAndWaitForReceiptAsync(SafeTransferFrom1Function safeTransferFrom1Function, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(safeTransferFrom1Function, cancellationToken);
        }

        public Task<string> SafeTransferFromRequestAsync(string from, string to, BigInteger tokenId, byte[] data)
        {
            var safeTransferFrom1Function = new SafeTransferFrom1Function();
                safeTransferFrom1Function.From = from;
                safeTransferFrom1Function.To = to;
                safeTransferFrom1Function.TokenId = tokenId;
                safeTransferFrom1Function.Data = data;
            
             return ContractHandler.SendRequestAsync(safeTransferFrom1Function);
        }

        public Task<TransactionReceipt> SafeTransferFromRequestAndWaitForReceiptAsync(string from, string to, BigInteger tokenId, byte[] data, CancellationTokenSource cancellationToken = null)
        {
            var safeTransferFrom1Function = new SafeTransferFrom1Function();
                safeTransferFrom1Function.From = from;
                safeTransferFrom1Function.To = to;
                safeTransferFrom1Function.TokenId = tokenId;
                safeTransferFrom1Function.Data = data;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(safeTransferFrom1Function, cancellationToken);
        }

        public Task<string> SetApprovalForAllRequestAsync(SetApprovalForAllFunction setApprovalForAllFunction)
        {
             return ContractHandler.SendRequestAsync(setApprovalForAllFunction);
        }

        public Task<TransactionReceipt> SetApprovalForAllRequestAndWaitForReceiptAsync(SetApprovalForAllFunction setApprovalForAllFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setApprovalForAllFunction, cancellationToken);
        }

        public Task<string> SetApprovalForAllRequestAsync(string @operator, bool approved)
        {
            var setApprovalForAllFunction = new SetApprovalForAllFunction();
                setApprovalForAllFunction.Operator = @operator;
                setApprovalForAllFunction.Approved = approved;
            
             return ContractHandler.SendRequestAsync(setApprovalForAllFunction);
        }

        public Task<TransactionReceipt> SetApprovalForAllRequestAndWaitForReceiptAsync(string @operator, bool approved, CancellationTokenSource cancellationToken = null)
        {
            var setApprovalForAllFunction = new SetApprovalForAllFunction();
                setApprovalForAllFunction.Operator = @operator;
                setApprovalForAllFunction.Approved = approved;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setApprovalForAllFunction, cancellationToken);
        }

        public Task<bool> SupportsInterfaceQueryAsync(SupportsInterfaceFunction supportsInterfaceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SupportsInterfaceFunction, bool>(supportsInterfaceFunction, blockParameter);
        }

        
        public Task<bool> SupportsInterfaceQueryAsync(byte[] interfaceId, BlockParameter blockParameter = null)
        {
            var supportsInterfaceFunction = new SupportsInterfaceFunction();
                supportsInterfaceFunction.InterfaceId = interfaceId;
            
            return ContractHandler.QueryAsync<SupportsInterfaceFunction, bool>(supportsInterfaceFunction, blockParameter);
        }

        public Task<string> SymbolQueryAsync(SymbolFunction symbolFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SymbolFunction, string>(symbolFunction, blockParameter);
        }

        
        public Task<string> SymbolQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SymbolFunction, string>(null, blockParameter);
        }

        public Task<string> TokenURIQueryAsync(TokenURIFunction tokenURIFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TokenURIFunction, string>(tokenURIFunction, blockParameter);
        }

        
        public Task<string> TokenURIQueryAsync(BigInteger tokenId, BlockParameter blockParameter = null)
        {
            var tokenURIFunction = new TokenURIFunction();
                tokenURIFunction.TokenId = tokenId;
            
            return ContractHandler.QueryAsync<TokenURIFunction, string>(tokenURIFunction, blockParameter);
        }

        public Task<string> TransferFromRequestAsync(TransferFromFunction transferFromFunction)
        {
             return ContractHandler.SendRequestAsync(transferFromFunction);
        }

        public Task<TransactionReceipt> TransferFromRequestAndWaitForReceiptAsync(TransferFromFunction transferFromFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFromFunction, cancellationToken);
        }

        public Task<string> TransferFromRequestAsync(string from, string to, BigInteger tokenId)
        {
            var transferFromFunction = new TransferFromFunction();
                transferFromFunction.From = from;
                transferFromFunction.To = to;
                transferFromFunction.TokenId = tokenId;
            
             return ContractHandler.SendRequestAsync(transferFromFunction);
        }

        public Task<TransactionReceipt> TransferFromRequestAndWaitForReceiptAsync(string from, string to, BigInteger tokenId, CancellationTokenSource cancellationToken = null)
        {
            var transferFromFunction = new TransferFromFunction();
                transferFromFunction.From = from;
                transferFromFunction.To = to;
                transferFromFunction.TokenId = tokenId;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFromFunction, cancellationToken);
        }

        public Task<string> UpdateListingPriceRequestAsync(UpdateListingPriceFunction updateListingPriceFunction)
        {
             return ContractHandler.SendRequestAsync(updateListingPriceFunction);
        }

        public Task<TransactionReceipt> UpdateListingPriceRequestAndWaitForReceiptAsync(UpdateListingPriceFunction updateListingPriceFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(updateListingPriceFunction, cancellationToken);
        }

        public Task<string> UpdateListingPriceRequestAsync(BigInteger listingPrice)
        {
            var updateListingPriceFunction = new UpdateListingPriceFunction();
                updateListingPriceFunction.ListingPrice = listingPrice;
            
             return ContractHandler.SendRequestAsync(updateListingPriceFunction);
        }

        public Task<TransactionReceipt> UpdateListingPriceRequestAndWaitForReceiptAsync(BigInteger listingPrice, CancellationTokenSource cancellationToken = null)
        {
            var updateListingPriceFunction = new UpdateListingPriceFunction();
                updateListingPriceFunction.ListingPrice = listingPrice;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(updateListingPriceFunction, cancellationToken);
        }
    }
}

using System.Runtime.CompilerServices;
using AutoMapper;
using Azure.Storage.Blobs;
using Contoso.Api.Data;
using Contoso.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Contoso.Api.Services;

public class ProductsService : IProductsService
{
    private readonly ContosoDbContext _context;
    private readonly IMapper _mapper;
    private readonly string _sasUrl;
    private readonly string _containerName;
    private readonly string _storageUrl;
    private readonly string _sasToken;

    public ProductsService(ContosoDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _sasUrl = "https://sasmartwebshopteam1.blob.core.windows.net/?sv=2022-11-02&ss=bfqt&srt=sco&sp=rwdlacupiytfx&se=2025-03-27T21:53:50Z&st=2025-03-18T13:53:50Z&spr=https,http&sig=SVT3rK38Kn4dXdZv%2FEk79cswik2H0Z3ZrCqotvpUPQE%3D";
        _containerName = "products";
        _storageUrl = "https://sasmartwebshopteam1.blob.core.windows.net";
        _sasToken = "sv=2022-11-02&ss=bfqt&srt=sco&sp=rwdlacupiytfx&se=2025-03-27T21:53:50Z&st=2025-03-18T13:53:50Z&spr=https,http&sig=SVT3rK38Kn4dXdZv%2FEk79cswik2H0Z3ZrCqotvpUPQE%3D";
    }

    public async Task<string> GetBlobMetadataAsync(string blobName)
    {
        if (string.IsNullOrEmpty(blobName))
            return string.Empty;

        // Create a BlobServiceClient
        var blobServiceClient = new BlobServiceClient(new Uri(_sasUrl));

        // Get the container client
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        var blobsList = containerClient.GetBlobsAsync().ToBlockingEnumerable().ToList();
        // Get the blob client
        var blobClient = containerClient.GetBlobClient(Path.GetFileName((new Uri(blobName)).AbsolutePath));

        if(!await blobClient.ExistsAsync())
            return DateTime.Now.ToString();
        try
        {
            var key = "ReleaseDate";
            // Fetch the blob's metadata
            var blobProperties = await blobClient.GetPropertiesAsync();

            // Check if the metadata contains the specified key
            if (blobProperties.Value.Metadata.ContainsKey(key))
            {
                string value = blobProperties.Value.Metadata[key];
                Console.WriteLine($"Metadata for '{key}': {value}");
                return value;
            }
            else
            {
                Console.WriteLine($"Metadata key '{key}' not found.");
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching metadata: {ex.Message}");
            return string.Empty;
        }
    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(QueryParameters queryParameters)
    {

        var products = await _context.Products
                            .Where(p =>  p.Category == queryParameters.filterText || string.IsNullOrEmpty(queryParameters.filterText))
                            .Skip(queryParameters.StartIndex) 
                            .Take(queryParameters.PageSize)
                            .ToListAsync();

        foreach(var item in products)
        {
            item.ImageUrl = $"{_storageUrl}{item.ImageUrl}?{_sasToken}";
            var date = await GetBlobMetadataAsync(item.ImageUrl);
            if (!string.IsNullOrEmpty(date))
            {
                if (DateTime.TryParse(date, out DateTime parsedDate))
                {
                    Console.WriteLine($"Parsed Date: {parsedDate}");

                    // Get the current date and time
                    DateTime currentDate = DateTime.Now;

                    // Compare the parsed date with the current date and time
                    if (parsedDate > currentDate)
                    {
                        item.ImageUrl = string.Empty;
                    }
                }
            }
        }

        var totalCount = await _context.Products
                                        .Where(p =>  p.Category == queryParameters.filterText || string.IsNullOrEmpty(queryParameters.filterText))
                                        .CountAsync();

        var pagedProducts = new PagedResult<ProductDto>
        {
            Items = _mapper.Map<List<ProductDto>>(products),
            TotalCount = totalCount,
            PageSize = queryParameters.PageSize,
            PageNumber = queryParameters.PageNumber
        };


        return pagedProducts;
    }

    public async Task<ProductDto> GetProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateProductAsync(ProductDto product)
    {
        var productModel = _mapper.Map<Product>(product);

        _context.Products.Add(productModel);

        await _context.SaveChangesAsync();

        return _mapper.Map<ProductDto>(productModel);
    }

    public async Task<ProductDto> UpdateProductAsync(ProductDto product)
    {
        var existingProduct = await _context.Products.AsNoTracking().FirstAsync(x => x.Id == product.Id);

        if  (existingProduct == null)
        {
            return null;
        }

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        
        if (existingProduct.ImageUrl != product.ImageUrl)
        {
            existingProduct.ImageUrl = product.ImageUrl;
        }


        _context.Entry(existingProduct).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return _mapper.Map<ProductDto>(existingProduct);
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _context.Products.AsNoTracking().FirstAsync(x => x.Id == id);

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();
    }

    public async Task<List<string>> GetProductCategories()
    {
        return await _context.Products.Select(x => x.Category).Distinct().ToListAsync();
    }
}
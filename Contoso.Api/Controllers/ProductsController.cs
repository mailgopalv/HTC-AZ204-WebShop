using Azure.Storage.Blobs;
using Contoso.Api.Data;
using Contoso.Api.Models;
using Contoso.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductsService _productService;
    private readonly string _containerName = "Products";
    private readonly string _sasUrl;

    public ProductsController(IProductsService productService)
    {
        _sasUrl = "https://sasmartwebshopteam1.blob.core.windows.net/?sv=2022-11-02&ss=bfqt&srt=sco&sp=rwdlacupiytfx&se=2025-03-27T21:53:50Z&st=2025-03-18T13:53:50Z&spr=https,http&sig=SVT3rK38Kn4dXdZv%2FEk79cswik2H0Z3ZrCqotvpUPQE%3D";
        _containerName = "products";
    }

    [HttpPost]
    public async Task<PagedResult<ProductDto>> GetProductsAsync(QueryParameters queryParameters)
    {
        return await _productService.GetProductsAsync(queryParameters);
    }

    [HttpGet("categories")]
    public async Task<List<string>> GetProductCategories()
    {
        return await _productService.GetProductCategories();
    }
    
    [HttpGet("{id}")]
    public async Task<ProductDto> GetProductAsync(int id)
    {
        return await _productService.GetProductAsync(id);
    }


    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateProductAsync([FromForm] ProductDto product, [FromForm] IFormFile Image)
    {
        try
        {
            if (Image != null && Image.Length > 0)
            {
                // Step 1: Upload the image to Azure Blob Storage
                var blobServiceClient = new BlobServiceClient(new Uri(_sasUrl));
                var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

                // Generate a unique name for the image to avoid collisions
                var blobName = Path.GetFileNameWithoutExtension(Image.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(Image.FileName);
                var blobClient = containerClient.GetBlobClient(blobName);

                // Step 2: Upload the image to the Azure Blob
                using (var stream = Image.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                // Step 3: Set the image URL for the product
                var imageUrl = blobClient.Uri.AbsolutePath.ToString();
                product.ImageUrl = imageUrl; // Assuming ImageUrl is a property of the ProductDto
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Exception occured: {ex.Message}");
        }
    

        // Step 4: Save the product to the database (or any other business logic)
        var result = await _productService.CreateProductAsync(product);

        if (result != null)
        {
            return Ok(new { message = "Product created successfully!" });
        }
        else
        {
            return BadRequest("An error occurred while creating the product.");
        }
    }


    /*[HttpPost("create")]
    [Authorize]
    public async Task<ProductDto> CreateProductAsync(ProductDto product)
    {
        return await _productService.CreateProductAsync(product);
    }*/


    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateProductAsync(ProductDto product)
    {
        var updatedProduct = await _productService.UpdateProductAsync(product);

        if (updatedProduct == null)
        {
            return BadRequest("Product not found");
        }

        return Ok(updatedProduct);
    }


    [HttpPost("upload/images")]
    [Authorize]
    public async Task<IActionResult> GetUploadBlobUrl([FromBody] List<ProductImageDto> productImage)
    {
        
         ///////////////////////
        //// YOUR CODE HERE ///
       ///////////////////////
       
       return  BadRequest();
    }

    [HttpPost("create/bulk")]
    [Authorize]
    public async Task<IActionResult> CreateProductsAsync()
    {
         ///////////////////////
        //// YOUR CODE HERE ///
       ///////////////////////
       
       return  BadRequest();
    }


    [HttpDelete("{id}")]
    [Authorize]
    public async Task DeleteProductAsync(int id)
    {
        await _productService.DeleteProductAsync(id);
    }
}
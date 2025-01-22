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

    public ProductsController(IProductsService productService)
    {
        _productService = productService;
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
    public async Task<ProductDto> CreateProductAsync(ProductDto product)
    {
        return await _productService.CreateProductAsync(product);
    }


    [HttpPut("{id}")]
    public async Task<ProductDto> UpdateProductAsync(int id, ProductDto product)
    {
        product.Id = id;
        return await _productService.UpdateProductAsync(product);
    }


    // Make check if product with id exists
    [HttpDelete("{id}")]
    public async Task DeleteProductAsync(int id)
    {
        await _productService.DeleteProductAsync(id);
    }
}
using System.Threading.Tasks;
using Contoso.WebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Contoso.WebApp.Extensions;
public class ProductModel : PageModel
{
    private readonly IContosoAPI _contosoAPI;

    public ProductDto Product { get; set; }

    public string ErrorMessage { get; set; }

    public string SuccessMessage { get; set; }


    public ProductModel(IContosoAPI contosoAPI)
    {
        _contosoAPI = contosoAPI;
    }
   
    public async Task OnGetAsync(int id)
    {
        var response = await _contosoAPI.GetProductAsync(id);

        if (!response.IsSuccessStatusCode)
        {
            ErrorMessage = "Failed to retrieve product";
        }

        if (HttpContext.Session.Get<string>("SuccessMessage") != null)
        {
            SuccessMessage = HttpContext.Session.Get<string>("SuccessMessage") ?? string.Empty;
            HttpContext.Session.Remove("SuccessMessage");
        }

        Product = response.Content;
    }

    public async Task<IActionResult> OnPostAddToCart(int id)
    {
        // Add logic to add product to cart (session storage or something similar)

        var response = await _contosoAPI.GetProductAsync(id);

        if (!response.IsSuccessStatusCode)
        {
            ErrorMessage = "Failed to retrieve product";
            return Page();
        }

        Product = response.Content;

        List<OrderItemDto> orderItems = HttpContext.Session.Get<List<OrderItemDto>>("OrderItems") ?? new List<OrderItemDto>();

        Console.WriteLine("OrderItems: " + orderItems.Count);
       
        var existingOrderItem = orderItems.FirstOrDefault(oi => oi.ProductId == id);

        if (existingOrderItem != null)
        {
            existingOrderItem.Quantity++;
        }
        else
        {
            orderItems.Add(new OrderItemDto
            {
                ProductId = id,
                Quantity = 1,
                Price = Product.Price,
                Product = Product
            });
        }

        HttpContext.Session.Set("OrderItems", orderItems);

        HttpContext.Session.Set("SuccessMessage", "Product added to cart");

        return RedirectToPage(new { id });
    }
}
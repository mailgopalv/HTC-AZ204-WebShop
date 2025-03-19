using Contoso.WebApp.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Contoso.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc;

public class CartModel : PageModel
{
    private readonly IContosoAPI _contosoAPI;

    public List<OrderItemDto> OrderItems { get; set; }

    public string ErrorMessage { get; set; }

    public string SuccessMessage { get; set; }

    private readonly string _sasToken;

    public CartModel(IContosoAPI contosoAPI)
    {
        _contosoAPI = contosoAPI;
        _sasToken = "sp=r&st=2025-03-19T15:15:01Z&se=2025-03-28T23:15:01Z&sv=2022-11-02&sr=c&sig=h9FIOlVV34mxyB5Nr3L0QcH1DFquW1QHQbSaB%2Bf67SU%3D";
    }
   
    public async Task OnGetAsync()
    {
        LoadOrderItemsFromSession();

        if (HttpContext.Session.Get<string>("SuccessMessage") != null)
        {
            SuccessMessage = HttpContext.Session.Get<string>("SuccessMessage") ?? string.Empty;
            HttpContext.Session.Remove("SuccessMessage");
        }

        if (HttpContext.Session.Get<string>("ErrorMessage") != null)
        {
            ErrorMessage = HttpContext.Session.Get<string>("ErrorMessage") ?? string.Empty;
            HttpContext.Session.Remove("ErrorMessage");
        }
    }

    public async Task<IActionResult> OnPostCheckoutAsync(int id)
    {
        LoadOrderItemsFromSession();

        var order = new OrderDto
        {
            Items = OrderItems,
            Total = OrderItems.Sum(oi => oi.Quantity * oi.Price)

        };

        if (HttpContext.Session.GetString("AuthToken") == null)
        {
            HttpContext.Session.Set("ErrorMessage","You must be logged in to submit an order.");
            return RedirectToPage();
        }

        var response = await _contosoAPI.SubmitOrderAsync(order);

        if (!response.IsSuccessStatusCode)
        {
            ErrorMessage = "Failed to submit order";
            return Page();
        }

        HttpContext.Session.Remove("OrderItems");

        HttpContext.Session.Set("CartCount", 0);

        HttpContext.Session.Set("SuccessMessage", "Order submitted successfully. Thank you!");

        return RedirectToPage();
    }


    // Helper method to get the Product Thumbnail URL FROM the image URL
    // Use this method in the challenge 3 with Azure Functions
    public string GetProductThumbnaillUrl(string imageUrl)
    {
        // Parse the URL
        Uri uri = new Uri(imageUrl);
        string[] segments = uri.Segments;

        // Change the path to include "resized" folder

        if (segments.Length > 1)
        {
            segments[1] =  "resized-" + segments[1];
        }

        // Replace the image name with "_thumb" suffix
        string lastSegment = segments[segments.Length - 1];

        string extension = System.IO.Path.GetExtension(lastSegment);
        if (!string.IsNullOrEmpty(extension))
        {
            segments[segments.Length - 1] = lastSegment.Replace(extension, $"_thumb{extension}");
        }

        // Reconstruct the URL
        string newPath = string.Join("", segments);
        UriBuilder uriBuilder = new UriBuilder(uri)
        {
            Path = newPath
        };

        return uriBuilder.Uri.ToString();
    }

    private void LoadOrderItemsFromSession()
    {
        OrderItems = HttpContext.Session.Get<List<OrderItemDto>>("OrderItems");
        foreach(var item in OrderItems)
        {
            if(item.Product != null && !string.IsNullOrEmpty(item.Product.ImageUrl))
            {
                var currUrl = GetProductThumbnaillUrl(item.Product.ImageUrl);
                item.Product.ImageUrl = $"{currUrl}?{_sasToken}";
            }
        }

        if (OrderItems == null)
        {
            OrderItems = new List<OrderItemDto>();
            ErrorMessage = "Your cart is empty.";
        }
    }     
}
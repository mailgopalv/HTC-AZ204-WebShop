using Contoso.WebApp.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Contoso.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc;

public class CartModel : PageModel
{
    private readonly IContosoAPI _contosoAPI;

    public List<OrderItemDto> OrderItems { get; set; }

    public string ErrorMessage { get; set; }


    public CartModel(IContosoAPI contosoAPI)
    {
        _contosoAPI = contosoAPI;
    }
   
    public async Task OnGetAsync()
    {
        LoadOrderItemsFromSession();
    }

    public async Task<IActionResult> OnPostCheckoutAsync(int id)
    {
        LoadOrderItemsFromSession();

        var order = new OrderDto
        {
            Items = OrderItems,
            Total = OrderItems.Sum(oi => oi.Quantity * oi.Price)

        };

        var response = await _contosoAPI.SubmitOrderAsync(order);

        if (!response.IsSuccessStatusCode)
        {
            ErrorMessage = "Failed to submit order";
            return Page();
        }

        HttpContext.Session.Remove("OrderItems");

        return RedirectToPage("/Home/Home");
    }

    private void LoadOrderItemsFromSession()
    {
        OrderItems = HttpContext.Session.Get<List<OrderItemDto>>("OrderItems");
        if (OrderItems == null)
        {
            OrderItems = new List<OrderItemDto>();
            ErrorMessage = "Your cart is empty.";
        }
    }

        
}
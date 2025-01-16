using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Contoso.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class HomeModel : PageModel
{
    public List<ProductDto> Products { get; set; }

    public string ErrorMessage { get; set; }

    private readonly IContosoAPI _contosoAPI;

    public HomeModel(IContosoAPI contosoAPI)
    {
        _contosoAPI = contosoAPI;
    }
   
    public async Task OnGetAsync()
    {
        var response = await _contosoAPI.GetProductsAsync();

        if (!response.IsSuccessStatusCode)
        {
            ErrorMessage = "Failed to retrieve products";
        }

        Products = response.Content;
    }

    public IActionResult OnPostImageClick(int productId)
    {
        return RedirectToPage("/Product/Product", new { id = productId });
    }
}
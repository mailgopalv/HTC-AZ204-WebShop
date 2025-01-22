using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Contoso.Api.Data;
using Contoso.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Contoso.WebApp.Extensions;
using Microsoft.AspNetCore.Server.HttpSys;


public class HomeModel : PageModel
{
    public List<ProductDto> Products { get; set; }

    public List<string> Categories { get; set; }

    public int CurrentPage  { get; set; } = 1;

    public int TotalPages { get; set; }

    public int PageSize { get; set; } = 2;

    public string CategorySelected { get; set; } = "";

    public string ErrorMessage { get; set; }

    private readonly IContosoAPI _contosoAPI;


    public HomeModel(IContosoAPI contosoAPI)
    {
        _contosoAPI = contosoAPI;
    }
   
    public async Task OnGetAsync()
    {

        if (HttpContext.Session.Get("CartCount") == null) 
        {
            HttpContext.Session.Set("CartCount", 0);
        }
        
        if (HttpContext.Session.Get<List<string>>("Categories") == null)
        {
            var category_response = await _contosoAPI.GetCategoriesAsync();
            Categories = category_response.Content;
            HttpContext.Session.Set("Categories", Categories);
        }
        else
        {
            Categories = HttpContext.Session.Get<List<string>>("Categories");
        }

        bool isCategorySelected = HttpContext.Session.Get<string>("CategorySelected") != null;
        bool isPageSelected = HttpContext.Session.Get<int>("CurrentPage") > 0;

        if (isCategorySelected)
        {
            CategorySelected = HttpContext.Session.Get<string>("CategorySelected");
        }

        if (isPageSelected)
        {
            CurrentPage = HttpContext.Session.Get<int>("CurrentPage");
        }


        var pagedProducts = GetPagedFilteredProduct(CurrentPage, CategorySelected);

        Products = pagedProducts.Result.Items;
        TotalPages = pagedProducts.Result.TotalCount / pagedProducts.Result.PageSize;

    }

    public IActionResult OnPostImageClick(int productId)
    {
        return RedirectToPage("/Product/Product", new { id = productId });
    }

    public IActionResult OnGetPage(int pageNumber)
    {
        HttpContext.Session.Set("CurrentPage", pageNumber);

        return RedirectToPage();
    }

    public IActionResult OnGetFilterByCategory(string category)
    {

        HttpContext.Session.Set("CategorySelected", category);
        HttpContext.Session.Set("CurrentPage", 1);

        Console.WriteLine("CategorySelected: " + category);

        return RedirectToPage();
    }

    private async Task<PagedResult<ProductDto>> GetPagedFilteredProduct(int pageNumber, string category)
    {
        var productResponse = await _contosoAPI.GetProductsPagedAsync(new QueryParameters
        {
            filterText = category,
            PageNumber = pageNumber,
            PageSize = PageSize,
            StartIndex = (pageNumber - 1) * PageSize
        });

        return productResponse.Content;
    }

}
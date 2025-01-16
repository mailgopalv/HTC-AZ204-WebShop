using Refit;
using static Contoso.WebApp.Pages.Account.LoginModel;

namespace Contoso.WebApp.Services
{
    public interface IContosoAPI
    {
        [Post("/api/Account/Login")]
        Task<ApiResponse<LoginDto>> LoginAsync(LoginInputModel loginInputModel);

        [Get("/api/Products")]
        Task<ApiResponse<List<ProductDto>>> GetProductsAsync();

        [Get("/api/Products/{id}")]
        Task<ApiResponse<ProductDto>> GetProductAsync(int id);

        [Post("/api/Order")]
        Task<ApiResponse<OrderDto>> SubmitOrderAsync(OrderDto orderDto);
    }
}
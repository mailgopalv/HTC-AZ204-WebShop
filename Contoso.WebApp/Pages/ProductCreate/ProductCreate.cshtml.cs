using Contoso.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class ProductCreateModel : PageModel
{

        public bool IsAdmin { get; set; }

        public string ImageUrl { get; set; }

        [BindProperty]
        public ProductDto Product { get; set; }


        [BindProperty]
        public IFormFile Image { get; set; }


        public string SuccessMessage { get; set; }


        private readonly IContosoAPI _contosoApi;


        public ProductCreateModel(IContosoAPI contosoApi)
        {
            _contosoApi = contosoApi;
        }


        public void OnGet()
        {
            IsAdmin = true;
        }

        public async Task<IActionResult> OnPostCreateProductAsync()
        {
            if (Image != null && Image.Length > 0)
            {
                // Create FormData object and append the file
                var formData = new MultipartFormDataContent();
                var fileContent = new StreamContent(Image.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg"); // Adjust MIME type as needed
                formData.Add(fileContent, "Image", Image.FileName); // 'Image' is the field name in the API for the file

                // Add other form data, e.g. product details
                formData.Add(new StringContent(Product.Name), "Name");
                formData.Add(new StringContent(Product.Description), "Description");
                formData.Add(new StringContent(Product.Category), "Category");
                formData.Add(new StringContent(Product.Price.ToString()), "Price");

                // Call the API to create the product
                var response = await _contosoApi.CreateProductAsync(formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Product created successfully!";
                    return RedirectToPage("/ProductCreate/ProductCreate");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the product.");
                    return Page();
                }
            }

            ModelState.AddModelError(string.Empty, "Please select an image for the product.");
            return Page();
        }

    /*public async Task<IActionResult> OnPostCreateProductAsync()
    {
        if (Image != null && Image.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                Image.CopyTo(memoryStream);
                Product.Image = memoryStream.ToArray();
            }

            var fileName = Path.GetFileName(Image.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                Image.CopyTo(stream);
            }

            Product.ImageUrl = fileName;
        }

        var response = await _contosoApi.CreateProductAsync(Product);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Product created successfully!";
            return RedirectToPage("/ProductCreate/ProductCreate");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "An error occurred while creating the product.");
            return Page();
        }
    }*/
}

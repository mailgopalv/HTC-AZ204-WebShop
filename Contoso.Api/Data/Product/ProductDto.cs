using System.ComponentModel.DataAnnotations;

namespace Contoso.Api.Data
{
    public class ProductDto
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Category { get; set; }

        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }
    }
}
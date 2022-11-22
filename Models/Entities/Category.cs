using System.ComponentModel.DataAnnotations;

namespace test.Models.Entities
{
    public class Category : Entity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<Product> Products { get; set; }
    }
}

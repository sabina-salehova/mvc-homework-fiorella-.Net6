using test.Models.Entities;

namespace test.Models.ViewModels
{
    public class BasketViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int Count { get; set; }
    }
}

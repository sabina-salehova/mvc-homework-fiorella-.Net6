using test.Models.Entities;

namespace test.ViewModels
{
    public class HomeViewModel
    {
        public Slider Slider = new Slider();
        public List<SliderImage> SliderImages = new List<SliderImage>();
        public List<Category> Categories = new List<Category>();
        public List<Product> Products = new List<Product>();
    }
}

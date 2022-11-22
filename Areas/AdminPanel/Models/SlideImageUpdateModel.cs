namespace test.Areas.AdminPanel.Models
{
    public class SlideImageUpdateModel
    {
        public string ImageUrl { get; set; }=String.Empty;
        public IFormFile Image { get; set; }
    }
}

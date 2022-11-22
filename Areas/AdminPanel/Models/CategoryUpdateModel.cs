using System.ComponentModel.DataAnnotations;

namespace test.Areas.AdminPanel.Models
{
    public class CategoryUpdateModel
    {
        [Required, MaxLength(20)]
        public string? Name { get; set; }
        [MaxLength(150)]
        public string? Description { get; set; }
    }
}

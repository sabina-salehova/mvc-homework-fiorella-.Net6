using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.DataAccessLayer;
using test.Models.Entities;

namespace test.ViewComponents
{
    public class DiscountProductViewComponent : ViewComponent
    {
        private readonly AppDbContext _dbContext;
        public DiscountProductViewComponent(AppDbContext dbContext) 
        { 
            _dbContext = dbContext; 
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Product> products = await _dbContext.Products.Include(p => p.Category).Where(p => p.Discount != null).ToListAsync();

            return View(products);
        }
    }
}

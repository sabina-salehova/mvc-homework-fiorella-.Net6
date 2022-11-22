using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.DataAccessLayer;
using test.Models.Entities;

namespace test.ViewComponents
{
    public class ProductViewComponent : ViewComponent
    {
        private readonly AppDbContext _dbContext;
        public ProductViewComponent(AppDbContext dbContext) 
        { 
            _dbContext = dbContext; 
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Product> products = await _dbContext.Products.Include(p=>p.Category).ToListAsync();

            return View(products);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.DataAccessLayer;
using test.Models.Entities;
using test.Models.ViewModels;

namespace test.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly AppDbContext _dbContext;
        private int _productCount;

        public ProductController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _productCount = dbContext.Products.Count();

        }
        public IActionResult Index()
        {
            ViewBag.productCount = _productCount;
            List<Product> Products = _dbContext.Products.Take(4).Include(product => product.Category).ToList();

            ProductViewModel pvm = new ProductViewModel
            {                
                Products = Products,
            };
            return View(pvm);
        }
        public IActionResult Details(int? id)
        {
            if (id is null) return NotFound();

            Product product = _dbContext.Products.SingleOrDefault(p => p.Id == id);

            if (product is null) return NotFound();

            return View(product);
        }

        public IActionResult Partial(int skip)
        {
            if (skip >= _productCount) return BadRequest();

            List<Product> Products = _dbContext.Products.Include(product => product.Category).Skip(skip).Take(4).ToList();
            
            return PartialView("_ProductPartial", Products);
        }

        public IActionResult ViewComponent()
        {
            return ViewComponent();
        }
    }
}

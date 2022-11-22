using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using test.DataAccessLayer;
using test.Models.Entities;
using test.ViewModels;

namespace test.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;

        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            HttpContext.Session.SetString("session","hello");
            Response.Cookies.Append("cookie","p324",new CookieOptions { Expires=DateTimeOffset.Now.AddHours(1)});

            Slider Slider = _dbContext.Sliders.SingleOrDefault();
            List<SliderImage> SliderImages = _dbContext.SliderImages.ToList();
            List<Category> Categories = _dbContext.Categories.ToList();
            List<Product> Products = _dbContext.Products.Include(product=>product.Category).ToList();

            HomeViewModel hvm = new HomeViewModel
            {
                Slider = Slider,
                SliderImages = SliderImages,
                Categories = Categories,
                Products = Products,
            };
            return View(hvm);
        }

        public IActionResult Search(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return NoContent();
            var products = _dbContext.Products.Where(p => p.Name.ToLower().Contains(searchText.Trim().ToLower())).ToList();
            return PartialView("_SearchedProductPartial",products);
        }

        public async Task<IActionResult>  Basket()
        {
            //var session = HttpContext.Session.GetString("session");
            //var cookie = Request.Cookies["cookie"];
            //return Content(session+"-"+cookie);

            var basket = Request.Cookies["basket"];

            if (basket is null) return NotFound();

            var basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);

            foreach(var basketViewModel in basketViewModels)
            {
                var product = await _dbContext.Products.Include(p=>p.Category).SingleOrDefaultAsync(p => p.Id == basketViewModel.Id);

                if (product != null)
                {
                    basketViewModel.Price = product.Price;
                    basketViewModel.Category = product.Category;
                }

            }            

            return View(basketViewModels);
        }

        public IActionResult AddToBasket(int? id)
        {
            //throw new Exception();
            var product = _dbContext.Products.Include(p=>p.Category).SingleOrDefault(x=>x.Id==id);

            if (product is null) return NotFound();            

            BasketViewModel newBvm = new BasketViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                Category = product.Category,
                Count = 1,
            };

            var basket = Request.Cookies["basket"];

            List<BasketViewModel> existBasketViewModels = null;

            if (basket is not null)
            {
                existBasketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);
                var existBvm = existBasketViewModels.Where(p => p.Id == product.Id).SingleOrDefault();
                if (existBvm is null) existBasketViewModels.Add(newBvm);
                else existBvm.Count++;
            }
            else
            {
                existBasketViewModels = new List<BasketViewModel>();
                existBasketViewModels.Add(newBvm);
            } 

            var productsJson = JsonConvert.SerializeObject(existBasketViewModels, new JsonSerializerSettings { ReferenceLoopHandling=ReferenceLoopHandling.Ignore});
            Response.Cookies.Append("basket",productsJson);

            return NoContent();
        
        }

        public IActionResult e_404()
        {
            ViewBag.ErrorCount = 404;
            return View(viewName: "forErrors");
        }
        public IActionResult e_403()
        {
            ViewBag.ErrorCount = 403;
            return View(viewName: "forErrors");
        }



    }
}

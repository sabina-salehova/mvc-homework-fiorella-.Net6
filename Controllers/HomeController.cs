using MailKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using test.Data;
using test.DataAccessLayer;
using test.Models.Entities;
using test.Models.ViewModels;
using IMailService = test.Services.IMailService;

namespace test.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IMailService _mailService;

        public HomeController(AppDbContext dbContext, IMailService mailService)
        {
            _dbContext = dbContext;
            _mailService = mailService;
        }
        public async Task<IActionResult> Index()
        {
            //await _mailService.SendEmailAsync(new RequestEmail { Body = "Hello" , ToEmail="sebine93@gmail.com", Subject="From lessondd"});

           //HttpContext.Session.SetString("session","hello");
           // Response.Cookies.Append("cookie","p324",new CookieOptions { Expires=DateTimeOffset.Now.AddHours(1)});

            Slider Slider =await _dbContext.Sliders.SingleOrDefaultAsync();
            List<SliderImage> SliderImages =await _dbContext.SliderImages.ToListAsync();
            List<Category> Categories = await _dbContext.Categories.ToListAsync();
            List<Product> Products =await _dbContext.Products.Include(product=>product.Category).ToListAsync();

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

        public async Task<IActionResult> Basket()
        {
            //var session = HttpContext.Session.GetString("session");
            //var cookie = Request.Cookies["cookie"];
            //return Content(session+"-"+cookie);

            var basket = Request.Cookies["basket"];

            List<BasketViewModel> basketViewModels = null;

            if (basket is not null)
            {
                basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);

                foreach (var basketViewModel in basketViewModels)
                {
                    var product = await _dbContext.Products.SingleOrDefaultAsync(p => p.Id == basketViewModel.Id);

                    if (product != null)
                    {
                        basketViewModel.Price = product.Price;
                    }
                }
            }

            return View(basketViewModels);

        }

        public async Task<IActionResult> AddToBasket(int? id)
        {
            if (id is null ) return BadRequest();
            //throw new Exception();
            var product = await _dbContext.Products.Include(p=>p.Category).SingleOrDefaultAsync(x=>x.Id==id);

            if (product is null) return NotFound();            

            BasketViewModel newBvm = new BasketViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Count = 1,
            };

            var basket = Request.Cookies["basket"];

            List<BasketViewModel> existBasketViewModels = new List<BasketViewModel>(); ;

            if (!string.IsNullOrEmpty(basket))
            {
                existBasketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);
                var existBvm = existBasketViewModels.Where(p => p.Id == id).SingleOrDefault();
                if (existBvm is null) existBasketViewModels.Add(newBvm);
                else existBvm.Count++;
            }
            else
            {
                existBasketViewModels.Add(newBvm);
            } 

            var productsJson = JsonConvert.SerializeObject(existBasketViewModels, new JsonSerializerSettings { ReferenceLoopHandling=ReferenceLoopHandling.Ignore});
            Response.Cookies.Append("basket",productsJson);

            return NoContent();
        
        }
        public async Task<IActionResult> RemoveItemFromBasket(int? id)
        {

            if (id is null) return BadRequest();
            //throw new Exception();
            var product = await _dbContext.Products.Include(p => p.Category).SingleOrDefaultAsync(x => x.Id == id);

            if (product is null) return NotFound();

            var basket = Request.Cookies["basket"];

            List<BasketViewModel> existBasketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);

            if (existBasketViewModels.Count>0)
            {
                var existBvm = existBasketViewModels.Find(p=>p.Id==id);
                if (existBvm is not null) existBasketViewModels.Remove(existBvm);

                var productsJson = JsonConvert.SerializeObject(existBasketViewModels, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                Response.Cookies.Append("basket", productsJson);
            }

                return RedirectToAction(nameof(Basket));
        }

        public async Task<IActionResult> MinusCountItemFromBasket(int? id)
        {

            if (id is null) return BadRequest();
            //throw new Exception();
            var product = await _dbContext.Products.Include(p => p.Category).SingleOrDefaultAsync(x => x.Id == id);

            if (product is null) return NotFound();

            var basket = Request.Cookies["basket"];

            List<BasketViewModel> existBasketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);

            if (existBasketViewModels.Count > 0)
            {
                var existBvm = existBasketViewModels.Find(p => p.Id == id);
                if (existBvm is not null)
                {
                    if (existBvm.Count > 1) existBvm.Count--;
                } 

                var productsJson = JsonConvert.SerializeObject(existBasketViewModels, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                Response.Cookies.Append("basket", productsJson);
            }

            return RedirectToAction(nameof(Basket));
        }

        public async Task<IActionResult> PlusCountItemFromBasket(int? id)
        {

            if (id is null) return BadRequest();
            //throw new Exception();
            var product = await _dbContext.Products.Include(p => p.Category).SingleOrDefaultAsync(x => x.Id == id);

            if (product is null) return NotFound();

            var basket = Request.Cookies["basket"];

            List<BasketViewModel> existBasketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);

            if (existBasketViewModels.Count > 0)
            {
                var existBvm = existBasketViewModels.Find(p => p.Id == id);
                if (existBvm is not null)
                {
                    existBvm.Count++;
                }

                var productsJson = JsonConvert.SerializeObject(existBasketViewModels, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                Response.Cookies.Append("basket", productsJson);
            }

            return RedirectToAction(nameof(Basket));
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

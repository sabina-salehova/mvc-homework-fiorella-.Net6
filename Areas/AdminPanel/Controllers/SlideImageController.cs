using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using test.Areas.AdminPanel.Data;
using test.Areas.AdminPanel.Models;
using test.DataAccessLayer;
using test.Models.Entities;

namespace test.Areas.AdminPanel.Controllers
{
    public class SlideImageController : BaseController
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;

        public SlideImageController(AppDbContext dbContext, IWebHostEnvironment environment) 
        { 
            _dbContext= dbContext;
            _environment = environment;
        }
        public async Task<IActionResult> Index()
        {
            List<SliderImage> slideImages =await _dbContext.SliderImages.ToListAsync();
            return View(slideImages);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SlideImageCreateModel model)
        {
            if(!ModelState.IsValid)
                return View();

            if (!model.Image.IsImage())
            {
                ModelState.AddModelError("Image","Shekil formati secilmelidir");
                return View();
            }
            int imageMbCount = 10;
            if (!model.Image.IsAllowedSize(imageMbCount))
            {
                ModelState.AddModelError("Image", $"Shekilin olcusu {imageMbCount} mb-dan boyuk ola bilmez");
                return View();
            }

            string unicalName = await model.Image.GenerateFile(Constants.RootPath);

            await _dbContext.SliderImages.AddAsync(new SliderImage
            {
                Name=unicalName
            });

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id )
        {
            if (id is null)
                return NotFound();

            SliderImage currentSlideImage = await _dbContext.SliderImages.FindAsync(id);

            if (currentSlideImage is null)
                return NotFound();

            return View(new SlideImageUpdateModel
            {
                ImageUrl = currentSlideImage.Name
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, SlideImageUpdateModel model)
        {
            if (id is null)
                return NotFound();

            var slideImage = await _dbContext.SliderImages.FindAsync(id);

            if (slideImage is null)
                return NotFound();

            if (slideImage.Id != id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(new SlideImageUpdateModel
                {
                    ImageUrl = slideImage.Name
                });
            }

            var newImage = model.Image;

            if (!newImage.IsImage())
            {
                ModelState.AddModelError("Image", "Shekil formati secilmelidir");

                return View(new SlideImageUpdateModel
                {
                    ImageUrl = slideImage.Name
                });
            }
            int imageMbCount = 10;
            if (!newImage.IsAllowedSize(imageMbCount))
            {
                ModelState.AddModelError("Image", $"Shekilin olcusu {imageMbCount} mb-dan boyuk ola bilmez");

                return View(new SlideImageUpdateModel
                {
                    ImageUrl = slideImage.Name
                });
            }

            var path = Path.Combine(Constants.RootPath,"img",slideImage.Name);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            var unicalFileName = await model.Image.GenerateFile(Constants.RootPath);

            slideImage.Name=unicalFileName;

            await _dbContext.SaveChangesAsync(); 

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
                return NotFound();

            var slideImage = await _dbContext.SliderImages.FindAsync(id);

            if (slideImage is null)
                return NotFound();

            if (slideImage.Id != id)
                return BadRequest();

            var path = Path.Combine(Constants.RootPath, "img", slideImage.Name);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            _dbContext.SliderImages.Remove(slideImage);

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MultipleCreate()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MultipleCreate(SlideImageCreateMultipleModel model)
        {

            if (!ModelState.IsValid)
                return View();

            foreach (var image in model.Images)
            {
                if (!image.IsImage())
                {
                    //ModelState.AddModelError("Images", "Shekil formati secilmelidir");
                    //return View();
                    continue;
                }
                int imageMbCount = 10;
                if (!image.IsAllowedSize(imageMbCount))
                {
                    //ModelState.AddModelError("Images", $"Shekilin olcusu {imageMbCount} mb-dan boyuk ola bilmez");
                    //return View();
                    continue;
                }

                string unicalName = await image.GenerateFile(Constants.RootPath);

                await _dbContext.SliderImages.AddAsync(new SliderImage
                {
                    Name = unicalName
                });
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

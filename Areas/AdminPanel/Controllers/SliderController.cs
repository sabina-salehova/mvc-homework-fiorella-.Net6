using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.Areas.AdminPanel.Data;
using test.Areas.AdminPanel.Models;
using test.DataAccessLayer;
using test.Models.Entities;

namespace test.Areas.AdminPanel.Controllers
{
    public class SliderController : BaseController
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;

        public SliderController(AppDbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _environment = environment;
        }
        public async Task<IActionResult> Index()
        {
            List<Slider> sliders = await _dbContext.Sliders.ToListAsync();

            return View(sliders);
        }

        public async Task<IActionResult> Create()
        {
            List<Slider> sliders = await _dbContext.Sliders.ToListAsync();

            if (sliders.Count>0)
            {
                ModelState.AddModelError("", "1-den artiq slider yaratmaq olmaz");
                return RedirectToAction(nameof(Index), sliders);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderCreateViewModel model)
        {

            if (!ModelState.IsValid)
                return View();

            if (!model.Image.IsImage())
            {
                ModelState.AddModelError("", "Sekil secmelisiz");
                return View(model);
            }

            if (!model.Image.IsAllowedSize(10))
            {
                ModelState.AddModelError("", "Sekil 10mb-den cox ola bilmez");
                return View(model);
            }

            string unicalName = await model.Image.GenerateFile(Constants.RootPath);

            await _dbContext.Sliders.AddAsync(new Slider
            {
                Title = model.Title,
                Subtitle = model.Subtitle,
                SignatureImageUrl = unicalName
            });

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();

            var slider = await _dbContext.Sliders.FindAsync(id);

            if (slider is null) return NotFound();

            return View(slider);
        }


        public async Task<IActionResult> Update(int? id)
        {
            if (id is null) return NotFound();

            var slider = await _dbContext.Sliders.FindAsync(id);

            if (slider is null) return NotFound();

            return View(new SliderUpdateViewModel
            {
                Title = slider.Title,
                Subtitle=slider.Subtitle,
                ImageUrl = slider.SignatureImageUrl
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, SliderUpdateViewModel model)
        {
            if (id is null)
                return NotFound();

            var existSlider = await _dbContext.Sliders.FindAsync(id);

            if (existSlider is null)
                return NotFound();

            if (existSlider.Id != id)
                return BadRequest();

            SliderUpdateViewModel sliderModel = new SliderUpdateViewModel
            {
                Title = existSlider.Title,
                Subtitle = existSlider.Subtitle,
                ImageUrl = existSlider.SignatureImageUrl
            };

            if (!ModelState.IsValid)
            {
                return View(sliderModel);
            }

            var updateImage = existSlider.SignatureImageUrl;

            if (model.Image is not null)
            {
                var newImage = model.Image;

                if (!newImage.IsImage())
                {
                    ModelState.AddModelError("Image", "Shekil formati secilmelidir");

                    return View(sliderModel);
                }
                int imageMbCount = 10;
                if (!newImage.IsAllowedSize(imageMbCount))
                {
                    ModelState.AddModelError("Image", $"Shekilin olcusu {imageMbCount} mb-dan boyuk ola bilmez");

                    return View(sliderModel);
                }

                var path = Path.Combine(Constants.RootPath, existSlider.SignatureImageUrl);

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                var unicalFileName = await model.Image.GenerateFile(Constants.RootPath);

                updateImage = unicalFileName;
            }

            existSlider.Title = model.Title;
            existSlider.Subtitle = model.Subtitle;
            existSlider.SignatureImageUrl = updateImage;

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var existSlider = await _dbContext.Sliders.FindAsync(id);

            if (existSlider is null) return NotFound();

            var path = Path.Combine(Constants.RootPath, existSlider.SignatureImageUrl);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            _dbContext.Sliders.Remove(existSlider);

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

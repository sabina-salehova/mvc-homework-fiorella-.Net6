using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using test.Areas.AdminPanel.Models;
using test.DataAccessLayer;
using test.Models.Entities;

namespace test.Areas.AdminPanel.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly AppDbContext _dbContext;

        public CategoryController(AppDbContext dbContext) 
        {
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _dbContext.Categories.ToListAsync();
            return View(categories);
        }

        public async Task<IActionResult> Details(int? id) 
        {
            if (id is null) return NotFound();

            var category = await _dbContext.Categories.FindAsync(id);

            if (category is null) return NotFound();

            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateModel category)
        {
            if (!ModelState.IsValid) return View();

            var existName = await _dbContext.Categories.AnyAsync(x=>x.Name.ToLower().Equals(category.Name.ToLower()));

            if (existName)
            {
                ModelState.AddModelError("name","Eyni ad tekrarlana bilmez");
                return View();
            }

            var categoryEntity = new Category 
            {
                Name=category.Name,
                Description=category.Description,
            };

           await _dbContext.Categories.AddAsync(categoryEntity);
           await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int? id)
        {
            if (id is null) return NotFound();

            var category = await _dbContext.Categories.FindAsync(id);

            if (category is null) return NotFound();

            return View(new CategoryUpdateModel
            {
                Name=category.Name,
                Description=category.Description,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, CategoryUpdateModel categoryModel)
        {
            if (id is null) return NotFound();

            if (!ModelState.IsValid) return View();

            var category = await _dbContext.Categories.FindAsync(id);

            if (category is null) return NotFound();

            if (category.Id != id)
                return BadRequest();

            var isExistName = await _dbContext.Categories.AnyAsync(c=>c.Name.ToLower() == categoryModel.Name.ToLower() && c.Id!=id);

            if (isExistName)
            {
                ModelState.AddModelError("Name", "Ad tekrarlana bilmez");
                return View();
            }
            
            category.Name = categoryModel.Name;

            category.Description = categoryModel.Description;

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var existCategory = await _dbContext.Categories.FindAsync(id);

            if(existCategory is null) return NotFound();

            _dbContext.Categories.Remove(existCategory);

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

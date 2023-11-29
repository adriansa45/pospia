using Microsoft.AspNetCore.Mvc;
using POS.Models;
using POS.Services;

namespace POS.Controllers
{
    public class CategoriesController: Controller
    {
        private readonly ICategoriesRepository categoriesRepository;

        public CategoriesController(ICategoriesRepository categoriesRepository)
        {
            this.categoriesRepository = categoriesRepository;
        }

        public IActionResult Create(Category category)
        {
            categoriesRepository.CreateCategory(category);
            return RedirectToAction("Home", "Home");
        }

        public IActionResult Edit(Category category)
        {
            categoriesRepository.CreateCategory(category);
            return RedirectToAction("Home", "Home");
        }
    }
}

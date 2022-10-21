using Microsoft.AspNetCore.Mvc;

namespace POS.Controllers
{
    public class CategoriesController: Controller
    {
        public CategoriesController()
        {

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }
        public IActionResult Delete()
        {
            return View();
        }
    }
}

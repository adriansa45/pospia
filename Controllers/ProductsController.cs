using Microsoft.AspNetCore.Mvc;
using POS.Models;
using POS.Services;

namespace POS.Controllers
{
    public class ProductsController: Controller
    {
        private readonly IProductsRepository productsRepository;

        public ProductsController(IProductsRepository productsRepository)
        {
            this.productsRepository = productsRepository;
        }
        
        public async Task<IActionResult> Index(string? code = null)
        {
            IEnumerable<Product> model;
            if (code == null)
            {
                model = await productsRepository.GetProducts();
            }
            else
            {
                model = await productsRepository.GetProducts(code);
            }
            
            return View(model);
        }

        public async Task<IActionResult> Crear()
        {
            return View();
        }
        public async Task<IActionResult> Editar()
        {
            return View();
        }
        public async Task<IActionResult> Eliminar()
        {
            return View();
        }
    }
}

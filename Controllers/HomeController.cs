using Microsoft.AspNetCore.Mvc;
using POS.Models;
using POS.Services;
using System.Diagnostics;

namespace POS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOrdersRepository ordersRepository;
        private readonly IServiceUser serviceUser;
        private readonly IProductsRepository productsRepository;

        public HomeController(ILogger<HomeController> logger, IOrdersRepository ordersRepository, 
            IServiceUser serviceUser, IProductsRepository productsRepository)
        {
            _logger = logger;
            this.ordersRepository = ordersRepository;
            this.serviceUser = serviceUser;
            this.productsRepository = productsRepository;
        }

        public async Task<IActionResult> Index(Category category)
        {
            var products = await productsRepository.GetProducts();
            var filterProduct = products.Where(e => e.CategoryId == (int)category);
            return View(filterProduct);
        }
        public async Task<IActionResult> Historic()
        {
            int userId = serviceUser.GetUserId();
            var model = await ordersRepository.GetOrders(userId);
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
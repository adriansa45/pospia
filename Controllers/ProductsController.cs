using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POS.Models;
using POS.Services;
using System.Data.Common;
using System.Transactions;

namespace POS.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductsRepository productsRepository;

        public ProductsController(IProductsRepository productsRepository)
        {
            this.productsRepository = productsRepository;
        }

        public async Task<IActionResult> Index(string? category = null)
        {
            IEnumerable<Product> model;
            if (category == null)
            {
                model = await productsRepository.GetProducts();
            }
            else
            {
                model = await productsRepository.GetProducts(category);
            }
            
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {

            if (!ModelState.IsValid || product.FormFile is null)
            {
                return View(product);
            }

            product.Image = await SaveImage(product.FormFile);

            product.CategoryId = (int)product.Category;
            await productsRepository.AddProduct(product);


            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await productsRepository.GetProduct(id);

            if (product is null)
            {
                return RedirectToAction();
            }

            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction();
            }
            if (product.FormFile is not null)
            {
                product.Image = await SaveImage(product.FormFile);
            }

            product.CategoryId = (int)product.Category;
            await productsRepository.EditProduct(product);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await productsRepository.GetProduct(id);
            if (product is null)
            {
                return RedirectToAction();
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(Product product)
        {
            if (product is null)
            {
                return RedirectToAction();
            }

            await productsRepository.Delete(product.ProductId);
            return RedirectToAction("Index");
        }

        public async Task<string> SaveImage (IFormFile file)
        {
            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                bytes = ms.ToArray();
            }
            var name = String.Concat(Guid.NewGuid().ToString(), ".webp");
            var path = Path.Combine(Path.GetFullPath("wwwroot/imgs/"), name);

            await System.IO.File.WriteAllBytesAsync(path, bytes);
            return name;
        }
    }
}

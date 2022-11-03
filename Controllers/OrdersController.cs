using Microsoft.AspNetCore.Mvc;
using POS.Models;
using POS.Services;

namespace POS.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IProductsRepository productsRepository;
        private readonly IServiceUser serviceUser;
        private readonly IOrdersRepository ordersRepository;

        public OrdersController(IProductsRepository productsRepository, IServiceUser serviceUser,IOrdersRepository ordersRepository)
        {
            this.productsRepository = productsRepository;
            this.serviceUser = serviceUser;
            this.ordersRepository = ordersRepository;
        }
        //
        [HttpPost]
        async public Task<JsonResult> BuyProducts([FromBody] IEnumerable<Product> products)
        {
            if (products.Count() > 0)
            {
                int userId = serviceUser.GetUserId();
                var productsList = await productsRepository.GetProducts(products);

                decimal total = productsList.Sum(p => p.Amount * p.Price);

                int orderId = await ordersRepository.CreateOrder(total, userId);

                var orderLines = new List<OrderLine>();
                foreach (var product in productsList)
                {
                    var ol = new OrderLine
                    {
                        ProductId = product.ProductId,
                        OrderId = orderId,
                        Price = product.Price,
                        Amount = product.Amount,
                        Created = DateTime.Now
                    };
                    orderLines.Add(ol);
                }

                await ordersRepository.CreateOrderLines(orderLines);
            }
            return Json(products);
        }
    }
}

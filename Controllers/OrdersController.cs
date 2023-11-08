using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using POS.Models;
using POS.Services;
using System.Data;

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
                        Total = product.Price * product.Amount,
                        Amount = product.Amount,
                        Created = DateTime.Now
                    };
                    orderLines.Add(ol);
                }

                await productsRepository.EditProducts(products);
                await ordersRepository.CreateOrderLines(orderLines);
            }
            return Json(products);
        }

        [HttpPost]
        async public Task<FileResult> GetReport(DateTime start, DateTime final)
        {
            int userId = serviceUser.GetUserId();

            start = new DateTime(start.Year, start.Day, start.Month);
            final = new DateTime(final.Year, final.Day, final.Month);

            start = start == DateTime.MinValue ? DateTime.Now.AddDays(-7) : start;
            final = final == DateTime.MinValue ? DateTime.Now : final;

            if (start > final)
            {
                throw new Exception("Fechas invalidas");
            }


            var data = await ordersRepository.GetOrderLines(start, final.AddDays(1), userId);

            DataTable dataTable = new DataTable();
            dataTable.TableName = "Report";//$"Reporte {start.ToShortDateString()} - {final.ToShortDateString()}";
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Fecha de Creación"),
                new DataColumn("Producto"),
                new DataColumn("Cantidad"),
                new DataColumn("Precio"),
                new DataColumn("Total"),

            });

            foreach (var item in data)
            {
                dataTable.Rows.Add(
                    item.Created,
                    item.Name,
                    item.Amount,
                    item.Price,
                    item.Total
                    );
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte.xlsx");
                }
            }

        }
    }
}

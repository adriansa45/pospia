using Dapper;
using MySql.Data.MySqlClient;
using POS.Models;

namespace POS.Services
{
    public interface IProductsRepository
    {
        Task<IEnumerable<Product>> GetProducts(IEnumerable<Product> products);
    }
    public class ProductsRepository: IProductsRepository
    {
        private readonly string connectionString;
        public ProductsRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Product>> GetProducts(IEnumerable<Product> products)
        {
            List<Product> productsList = new List<Product>();
            using var connection = new MySqlConnection(connectionString);
            
            foreach (var p in products)
            {
                var product = await connection.QuerySingleAsync<Product>("select * from products where product_id = @ProductId", new {p.ProductId});
                product.Amount = p.Amount;
                productsList.Add(product);
            }
            return productsList;
        }
        public async Task<IEnumerable<Product>> GetProducts()
        {
            using var connection = new MySqlConnection(connectionString);
            return await connection.QuerySingleAsync<IEnumerable<Product>>("select * from products");
        }

        public async Task<Product> GetProduct(int ProductId)
        {
            using var connection = new MySqlConnection(connectionString);
            var product = await connection.QuerySingleAsync<Product>("select * from products where product_id = @ProductId", new { ProductId });

            return product;
        }
    }
}

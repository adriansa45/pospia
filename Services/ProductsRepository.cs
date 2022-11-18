using Dapper;
using MySql.Data.MySqlClient;
using POS.Models;

namespace POS.Services
{
    public interface IProductsRepository
    {
        Task AddProduct(Product product);
        Task Delete(int id);
        Task EditProduct(Product product);
        Task<Product> GetProduct(int ProductId);
        Task<IEnumerable<Product>> GetProducts(IEnumerable<Product> products);
        Task<IEnumerable<Product>> GetProducts();
        Task<IEnumerable<Product>> GetProducts(string code);
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
                var product = await connection.QuerySingleAsync<Product>("select * from products where ProductId = @ProductId", new {p.ProductId});
                product.Amount = p.Amount;
                productsList.Add(product);
            }
            return productsList;
        }
        public async Task<IEnumerable<Product>> GetProducts()
        {
            using var connection = new MySqlConnection(connectionString);
            return await connection.QueryAsync<Product>("select * from products");
        }


        public async Task<IEnumerable<Product>> GetProducts(string code)
        {
            using var connection = new MySqlConnection(connectionString);
            return await connection.QueryAsync<Product>(String.Format("select * from products where cast(productId as CHAR) LIKE '%{0}%' or name LIKE '%{0}%';", code));
        }

        public async Task<Product> GetProduct(int ProductId)
        {
            using var connection = new MySqlConnection(connectionString);
            var product = await connection.QuerySingleAsync<Product>("select * from products where ProductId = @ProductId", new { ProductId });

            return product;
        }

        public async Task EditProduct(Product product)
        {
            using var connection = new MySqlConnection(connectionString);

            await connection.ExecuteAsync(@"UPDATE products SET
                                            name = @Name,
                                            amount = @Amount,
                                            price = @Price,
                                            image = @Image
                                            where ProductId = @ProductId", product);
        }

        public async Task AddProduct(Product product)
        {
            using var connection = new MySqlConnection(connectionString);
            int id = await connection.QuerySingleAsync<int>(@"INSERT INTO products 
                                                            (name, amount, price, image)
                                                            VALUES 
                                                            (@Name, @Amount, @Price, @Image);
                                                            SELECT LAST_INSERT_ID();", product);

            product.ProductId = id;
        }

        public async Task Delete(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE from products WHERE ProductId = @id", new { id });
        }
    }
}

using Dapper;
using POS.Models;
using MySql.Data.MySqlClient;

namespace POS.Services
{
    public interface IOrdersRepository
    {
        Task<int> CreateOrder(decimal Total, int UserId);
        Task CreateOrderLines(IEnumerable<OrderLine> OrderLines);
    }
    public class OrdersRepository: IOrdersRepository
    {
        private readonly string connectionString;
        public OrdersRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<int> CreateOrder(decimal Total, int UserId)
        {
            using var connection = new MySqlConnection(connectionString);
            return await connection.QuerySingleAsync<int>(@"INSERT INTO orders 
                                                            (total, created_by)
                                                            VALUES 
                                                            (@Total, @UserId);
                                                            SELECT LAST_INSERT_ID()", new { Total, UserId });
        }

        public async Task CreateOrderLines(IEnumerable<OrderLine> OrderLines)
        {
            using var connection = new MySqlConnection(connectionString);
            foreach (var orderline in OrderLines)
            {
                int id = await connection.QuerySingleAsync<int>(@"INSERT INTO order_lines 
                                                            (order_id, product_id, amount, total,created)
                                                            VALUES 
                                                            (@OrderId, @ProductId, @Amount, @Price, @Created);
                                                            SELECT LAST_INSERT_ID()", new { orderline.OrderId, orderline.ProductId, orderline.Amount, orderline.Price, orderline.Created });
            }
            
        }

    }
}

using Dapper;
using POS.Models;
using MySql.Data.MySqlClient;

namespace POS.Services
{
    public interface IOrdersRepository
    {
        Task<int> CreateOrder(decimal Total, int UserId);
        Task CreateOrderLines(IEnumerable<OrderLine> OrderLines);
        Task<IEnumerable<Order>> GetOrders(int UserId);
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
            DateTime Created = DateTime.Now;
            using var connection = new MySqlConnection(connectionString);
            return await connection.QuerySingleAsync<int>(@"INSERT INTO orders 
                                                            (total, created, created_by)
                                                            VALUES 
                                                            (@Total, @Created, @UserId);
                                                            SELECT LAST_INSERT_ID()", new { Total, Created, UserId });
        }

        public async Task CreateOrderLines(IEnumerable<OrderLine> OrderLines)
        {
            using var connection = new MySqlConnection(connectionString);
            foreach (var orderline in OrderLines)
            {
                int id = await connection.QuerySingleAsync<int>(@"INSERT INTO order_lines 
                                                            (OrderId, ProductId, amount, price, total,created)
                                                            VALUES 
                                                            (@OrderId, @ProductId, @Amount, @Price,@Total, @Created);
                                                            SELECT LAST_INSERT_ID()", new { orderline.OrderId, orderline.ProductId, orderline.Amount, orderline.Price,orderline.Total, orderline.Created });
            }
            
        }

        public async Task<IEnumerable<Order>> GetOrders(int UserId)
        {
            var orders = new  List<Order>();
            using var connection = new MySqlConnection(connectionString);
            var orderlines = await connection.QueryAsync<OrderLine>(@"select ol.OrderId, ol.OrderLineId,ol.ProductId, p.Name, ol.Total,ol.Price,ol.Amount,ol.Created 
                                                                    from order_lines ol 
                                                                    join orders o on ol.orderId = o.OrderId
                                                                    left join products p on ol.ProductId = p.ProductId
                                                                    where created_by = @UserId
                                                                    order by ol.created desc", new {UserId});
            if(orderlines is not null)
            {
                int? lastid = null;
                foreach (var orderline in orderlines)
                {
                    if (lastid is null || lastid != orderline.OrderId)
                    {
                        lastid = orderline.OrderId;
                        var o = new Order()
                        {
                            OrderId = orderline.OrderId,
                            Total = orderline.Total,
                            Created = orderline.Created,
                            orderLines = new List<OrderLine>()
                        };
                        o.orderLines.Add(orderline);
                        orders.Add(o);
                    }
                    else
                    {
                        var order = orders.SingleOrDefault(o => o.OrderId == lastid);
                        if (order is not null)
                        {
                            order.Total += orderline.Total;
                            order.orderLines.Add(orderline);
                        }
                    }
                }
            }
            
            return orders;
        }

    }
}

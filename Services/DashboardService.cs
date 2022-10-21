using POS.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Linq;

namespace POS.Services
{
    public interface IDashboardService
    {
        Task CreateDashboard(int userId);
        Task<Dashboard> GetDashboard(int userId, int accountId);
        Task UpdateDashboard(int userId, decimal amount, decimal oldAmount);
        Task UpdateDashboardBillSubscriptions(int userId, decimal amount, decimal oldAmount);
    }
    public class DashboardService: IDashboardService
    {
        private readonly ITransactionsRepository transactionsRepository;
        private readonly ISubscriptionsRepository subscriptionsRepository;
        private readonly string connectionString;

        public DashboardService(ITransactionsRepository transactionsRepository, 
                                IConfiguration configuration,
                                ISubscriptionsRepository subscriptionsRepository)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
            this.transactionsRepository = transactionsRepository;
            this.subscriptionsRepository = subscriptionsRepository;
        }

        public async Task<Dashboard> GetDashboard(int userId, int accountId)
        {
            using var connection = new MySqlConnection(connectionString);
            var dashboard = await connection.QuerySingleAsync<Dashboard>("select * from dashboard where userId = @userId", new { userId });

            var transactions = await transactionsRepository.GetTransactions(accountId);
            dashboard.LastTrasactions = transactions.Take(5);

            var subs = await subscriptionsRepository.GetSubscriptions(accountId);
            dashboard.Subscriptions = subs.Take(5);

            return dashboard;
        }

        public async Task CreateDashboard(int userId)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("Insert into dashboard (userId, balance, bills, billsSubscriptions, nextMonth) values (@userId, 0, 0, 0, 0);", new { userId });
        }

        public async Task UpdateDashboard(int userId, decimal amount, decimal oldAmount)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync(@"update dashboard as d,
                                            (select * from dashboard where userId = @userId) as r
                                            join accounts a on a.userId = r.userId
                                            set
                                            d.balance = a.balance,
                                            d.bills = r.bills - @oldAmount + @amount,
                                            d.nextMonth = r.nextMonth + @oldAmount - @amount
                                            where d.userId = @userId; ", new { userId, oldAmount, amount });

        }

        public async Task UpdateDashboardBillSubscriptions(int userId, decimal amount, decimal oldAmount)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync(@"update dashboard as d,
                                            (select * from dashboard where userId = @userId) as r
                                            set
                                            d.billsSubscriptions = r.billsSubscriptions - @oldAmount + @amount
                                            where d.userId = @userId; ", new { userId, oldAmount, amount });

        }

    }
}

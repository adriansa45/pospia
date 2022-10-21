using POS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace POS.Services
{
    public interface ISubscriptionsRepository
    {
        Task ChargeSubscriptions();
        Task<int> CreateSubscription(Subscription sub);
        Task Delete(int id);
        Task EditSubscription(Subscription subscription);
        Task<IEnumerable<Subscription>> GetSubscriptions(int accountId);
        Task<Subscription> GetSubscriptionById(int accountId, int id);
    }
    public class SubscriptionsRepository: ISubscriptionsRepository
    {
        private readonly string connectionString;
        public SubscriptionsRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreateSubscription(Subscription sub)
        {
            using var connection = new MySqlConnection(connectionString);
            int id = await connection.QuerySingleAsync<int>(@"INSERT INTO subscriptions 
                                                            (accountId, name, amount, date,nextPay)
                                                            VALUES 
                                                            (@AccountId, @Name, @Amount, @Date, @NextPay);
                                                            SELECT LAST_INSERT_ID();", sub);

            sub.Id = id;
            return id;
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptions(int accountId)
        {
            using var connection = new MySqlConnection(connectionString);
            return await connection.QueryAsync<Subscription>(@"SELECT *
                                                             FROM subscriptions
                                                             WHERE AccountId = @accountId
                                                             ORDER BY Date DESC, name", new { accountId });
        }


        public async Task<Subscription> GetSubscriptionById(int accountId, int id)
        {
            using var connection = new MySqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Subscription>(@"SELECT *
                                                             FROM subscriptions
                                                             WHERE AccountId = @accountId AND Id = @id", new { accountId, id });
        }


        public async Task EditSubscription(Subscription subscription)
        {
            using var connection = new MySqlConnection(connectionString);

            await connection.ExecuteAsync(@"UPDATE subscriptions SET
                                            name = @Name,
                                            amount = @Amount,
                                            date = @Date,
                                            nextPay = @nextPay
                                            where Id = @Id", subscription);
        }

        public async Task Delete(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE from subscriptions WHERE Id = @Id", new { id });
        }



        public async Task ChargeSubscriptions()
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync(@"create temporary table subs as
                                        select s.id, s.accountId, s.name, s.amount, s.lastPay, s.nextPay, a.userId, a.balance, s.interval_days 
                                        from subscriptions s 
                                        inner join accounts a on s.accountId = a.id  
                                        where nextPay < now();

                                        UPDATE subscriptions, subs SET subscriptions.lastPay = now(), subscriptions.nextPay = DATE_ADD(now(), INTERVAL s.interval_days  DAY) WHERE subscriptions.id =  subs.id;
                                        INSERT INTO transactions 
                                        (accountId, name, amount, date, category)
	                                        SELECT accountId, name, amount, now(), 4 FROM subs;
                                        UPDATE accounts AS a JOIN subs s ON a.id = s.accountId SET a.balance = a.balance - s.amount WHERE (a.id = s.accountId);
                                        UPDATE dashboard AS d, subs s SET  
                                        d.balance = d.balance - s.amount,
                                        d.bills = d.bills + s.amount
                                        WHERE d.userId = s.userId; 
                                        DROP TABLE subs;");

            
        }
    }
}

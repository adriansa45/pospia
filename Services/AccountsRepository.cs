using Dapper;
using POS.Models;
using MySql.Data.MySqlClient;

namespace POS.Services
{
	public interface IAccountsRepository
	{
        Task CreateAccount(int userId);
        Task DeleteTransactionToAccount(int accountId, decimal amount);
        Task UpdateBalance(int accountId, decimal amount, decimal oldAmount);
        Task<int> GetAccountIdByUserId(int userId);
        Task<Account> GetAccountByUserId(int userId);
        Task UpdateProfile(Account accountModel);
        Task ChargeSalary();
    }

	public class AccountsRepository: IAccountsRepository
    {
        private readonly string connectionString;

        public AccountsRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task CreateAccount(int userId)
		{
            using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("Insert into accounts (userId, balance, name) values (@userId, 0, 'main');", new { userId });

        }
        public async Task<Account> GetAccountByUserId(int userId)
        {
            using var connection = new MySqlConnection(connectionString);
            var account = await connection.QuerySingleAsync<Account>("select * from accounts where userId = @userId", new { userId });
            return account;
        }

        public async Task<int> GetAccountIdByUserId(int userId)
        {
            using var connection = new MySqlConnection(connectionString);
            var account = await connection.QuerySingleAsync<int>("select id from accounts where userId = @userId", new { userId });

            return account;
        }

        public async Task UpdateBalance(int accountId, decimal amount, decimal oldAmount)
        {
            using var connection = new MySqlConnection(connectionString);
            var account = await connection.ExecuteAsync(@"update accounts as a,
                                                        (select * from accounts where id = @accountId) as account
                                                        set a.balance = account.balance - @amount + @oldAmount
                                                        where a.id = @accountId; ", new { accountId, amount, oldAmount });

         }
        public async Task UpdateProfile(Account accountModel)
        {
            using var connection = new MySqlConnection(connectionString);
            var account = await connection.ExecuteAsync(@"update accounts as a
                                                    set a.salary = @Salary,
                                                    a.payDay = @PayDay,
                                                    a.balance = @Balance,
                                                    a.interval_days = @Interval_Days
                                                    where a.id = @Id; ", accountModel);
        }

        public async Task DeleteTransactionToAccount(int accountId, decimal amount)
        {
            using var connection = new MySqlConnection(connectionString);
            var account = await connection.ExecuteAsync(@"update accounts as a,
                                                        (select * from accounts where id = @accountId) as account
                                                        set a.balance = account.balance + @amount
                                                        where a.id = @accountId; ", new { accountId, amount });
        }


        public async Task ChargeSalary()
        {
            using var connection = new MySqlConnection(connectionString);
            var charge = await connection.ExecuteAsync(@"create temporary table acts as
                                                        select id, userId, balance, salary, payDay, interval_days
                                                        from accounts s
                                                        where payDay < now();
			                                                        UPDATE accounts, acts SET accounts.balance = accounts.balance + acts.salary, 
			                                                        accounts.payDay = DATE_ADD(now(), INTERVAL acts.interval_days DAY) WHERE accounts.id = acts.id;
			                                                        INSERT INTO transactions
			                                                        (accountId, name, amount, date, category)

	                                                        SELECT id, 'Salary', -1 * salary, now(), 3 FROM acts;
			                                                        UPDATE dashboard AS d, acts a SET
                                                        d.balance = d.balance + a.salary
                                                        WHERE d.userId = a.userId;
                                                        DROP TABLE acts;");
        }
    }
}

using Dapper;
using POS.Models;
using MySql.Data.MySqlClient;

namespace POS.Services
{
    public interface ITransactionsRepository
    {
        Task AddTransaction(Transaction transactionViewModel);
        Task Delete(int id);
        Task EditTransaction(Transaction transactionViewModel);
        Task<IEnumerable<Transaction>> GetTransactions(int accountId);
        Task<Transaction> GetTransactionById(int accountId, int id);
    }
    public class TransactionsRepository: ITransactionsRepository
    {
        private readonly string connectionString;

        public TransactionsRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public async Task<IEnumerable<Transaction>> GetTransactions(int accountId)
        {
            using var connection = new MySqlConnection(connectionString);
            return await connection.QueryAsync<Transaction>(@"SELECT *
                                                             FROM transactions
                                                             WHERE AccountId = @accountId
                                                             ORDER BY Date DESC, name", new {accountId});
        }

        public async Task<Transaction> GetTransactionById(int accountId, int id)
        {
            using var connection = new MySqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaction>(@"SELECT *
                                                             FROM transactions
                                                             WHERE AccountId = @accountId AND Id = @id", new { accountId, id });
        }

        public async Task AddTransaction(Transaction transaction)
        {
            using var connection = new MySqlConnection(connectionString);
            int id = await connection.QuerySingleAsync<int>(@"INSERT INTO transactions 
                                                            (accountId, name, amount, date, category)
                                                            VALUES 
                                                            (@AccountId, @Name, @Amount, @Date, @Category);
                                                            SELECT LAST_INSERT_ID();", transaction);

            transaction.Id = id;
        }

        public async Task EditTransaction(Transaction transaction)
        {
            using var connection = new MySqlConnection(connectionString);

            await connection.ExecuteAsync(@"UPDATE transactions SET
                                            name = @Name,
                                            amount = @Amount,
                                            date = @Date,
                                            category = @Category
                                            where Id = @Id", transaction);
        }

        public async Task Delete(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE from transactions WHERE Id = @Id", new { id });
        }

        
    }
}

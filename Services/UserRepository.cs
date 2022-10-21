using POS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace POS.Services
{
    public interface IUserRepository
    {
        Task<int> CreateUser(User user);
        Task<User> GetUserByEmail(string normalizedEmail);
        Task<User> GetUserById(int id);
    }
    public class UserRepository: IUserRepository
    {
        private readonly string connectionString;
		private readonly IAccountsRepository accountsRepository;
        private readonly IDashboardService dashboardService;

        public UserRepository(IConfiguration configuration, IAccountsRepository accountsRepository, IDashboardService dashboardService)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
			this.accountsRepository = accountsRepository;
            this.dashboardService = dashboardService;
        }

        public async Task<int> CreateUser(User user)
        {
            using var connection = new MySqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"Insert into users 
                                                            (name, Email, NormalizedEmail, Password) VALUES
                                                            (@Name, @Email, @NormalizedEmail, @Password);
                                                            SELECT LAST_INSERT_ID();", user);
            return id;
        }

        public async Task<User> GetUserByEmail(string normalizedEmail)
        {
            using var connection = new MySqlConnection(connectionString);
            return await connection.QuerySingleOrDefaultAsync<User>(@"SELECT * FROM users WHERE NormalizedEmail = @normalizedEmail", 
                                                            new {normalizedEmail});

        }

        public async Task<User> GetUserById(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            return await connection.QuerySingleOrDefaultAsync<User>(@"SELECT id, name, email FROM users WHERE id = @id",
                                                            new { id });

        }

    }
}

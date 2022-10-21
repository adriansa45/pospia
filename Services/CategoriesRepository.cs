using POS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace POS.Services
{

    public interface ICategoriesRepository
    {
        Task<IEnumerable<Category>> GetCategories(int userId);
    }

    public class CategoriesRepository: ICategoriesRepository
    {
        private readonly string connectionString;

        public CategoriesRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Category>>  GetCategories(int userId)
        {
            using var connection = new MySqlConnection(connectionString);
            return await connection.QueryAsync<Category>(@"select * from categories where userId in (0, @userId)", new {userId});
        }

        public async Task CreateCategories(Category category, int userId)
        {
            using var connection = new MySqlConnection(connectionString);
            int id = await connection.QuerySingleAsync<int>(@"INSERT INTO categories 
                                                            (userId, name, color)
                                                            VALUES 
                                                            (@userId, @Name, @Color);
                                                            SELECT LAST_INSERT_ID()", new { category, userId });
        }

    }
}

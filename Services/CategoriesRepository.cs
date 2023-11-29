using POS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace POS.Services
{

    public interface ICategoriesRepository
    {
        Task<IEnumerable<Category>> GetCategories();
        Task<Category> CreateCategory(Category category);
    }

    public class CategoriesRepository: ICategoriesRepository
    {
        private readonly string connectionString;

        public CategoriesRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Category>>  GetCategories()
        {
            using var connection = new MySqlConnection(connectionString);
            return await connection.QueryAsync<Category>(@"select * from categories");
        }

        public async Task<Category> CreateCategory(Category category)
        {
            using var connection = new MySqlConnection(connectionString);
            int id = await connection.QuerySingleAsync<int>(@"INSERT INTO categories 
                                                            (name, color)
                                                            VALUES 
                                                            (@Name, @Color);
                                                            SELECT LAST_INSERT_ID()", new { category});
            //category.CategoryId = id;
            return category;
        }

    }
}

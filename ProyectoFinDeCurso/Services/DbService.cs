using ProyectoFinDeCurso.Models;
using SQLite;

namespace ProyectoFinDeCurso.Services
{
    public class DbService
    {
        private const string DB_NAME = "Users.db3";
        private readonly SQLiteAsyncConnection _connection;

        public DbService()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
            _connection.CreateTableAsync<User>();
        }
        public async Task<List<User>> GetUsers()
        {
            return await _connection.Table<User>().ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _connection.Table<User>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task Create(User user)
        {
            await _connection.InsertAsync(user);
        }

        public async Task Update(User user)
        {
            await _connection.UpdateAsync(user);
        }

        public async Task Delete(User user)
        {
            await _connection.DeleteAsync(user);
        }
    }
}

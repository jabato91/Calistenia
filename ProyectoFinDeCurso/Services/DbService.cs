using ProyectoFinDeCurso.Models;
using SQLite;

namespace ProyectoFinDeCurso.Services
{
    public class DbService
    {
        private const string DB_NAME = "Users.db3"; //nombre de la base de datos
        private readonly SQLiteAsyncConnection _connection;

        public DbService()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME)); //crea la conexión a la base de datos
            _connection.CreateTableAsync<User>(); //crea la base de datos si no existe
        }
        public async Task<List<User>> GetUsers() //obtiene todos los usuarios
        {
            return await _connection.Table<User>().ToListAsync();
        }

        public async Task<User> GetById(int id) //obtiene usuario por id
        {
            return await _connection.Table<User>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task Create(User user) //crea usuario
        {
            await _connection.InsertAsync(user);
        }

        public async Task Update(User user) //actualiza usuario
        {
            await _connection.UpdateAsync(user);
        }

        public async Task Delete(User user) //elimina usuario
        {
            await _connection.DeleteAsync(user);
        }
    }
}

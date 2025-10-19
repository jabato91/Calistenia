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
            string path = Path.Combine(FileSystem.AppDataDirectory, DB_NAME);
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME)); //crea la conexión a la base de datos
            InitTablesAsync();
            
        }
        private async void InitTablesAsync()
        {
            await _connection.CreateTableAsync<User>();
            await _connection.CreateTableAsync<Exercise>();
        }
        public async Task<List<User>> GetUsers() //obtiene todos los usuarios
        {
            return await _connection.Table<User>().ToListAsync();
        }
        public async Task<List<Exercise>> GetEercises() //obtiene todos los usuarios
        {
            return await _connection.Table<Exercise>().ToListAsync();
        }
        public async Task<User> GetUserById(int id) //obtiene usuario por id
        {
            return await _connection.Table<User>().Where(x => x.UserID == id).FirstOrDefaultAsync();
        }
        public async Task<Exercise> GetExerciseById(int id) //obtiene ejercicio por id
        {
            return await _connection.Table<Exercise>().Where(x => x.execiseID == id).FirstOrDefaultAsync();
        }
        public async Task Create(object create) //elimina objeto
        {
            await _connection.InsertAsync(create);
        }

        public async Task Update(object update) //elimna objeto
        {
            await _connection.UpdateAsync(update);
        }

        public async Task Delete(object delete) //elimina objeto
        {
            await _connection.DeleteAsync(delete);
        }
        public async Task DeleteExerciseById(int id)
        {
            var exercise = await GetExerciseById(id);
            if (exercise != null)
            {
                await _connection.DeleteAsync(exercise);
            }
        }
    }
}

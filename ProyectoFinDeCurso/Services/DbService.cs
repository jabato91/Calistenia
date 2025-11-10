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
            await _connection.CreateTableAsync<Routines>();
            await _connection.CreateTableAsync<RoutinesExercises>();
        }
        public async Task<List<User>> GetUsers() => await _connection.Table<User>().ToListAsync(); //obtiene todos los usuarios

        public async Task<List<Exercise>> GetEercises() => await _connection.Table<Exercise>().ToListAsync(); //obtiene todos los ejercicios

        public async Task<List<Routines>> GetRoutines() => await _connection.Table<Routines>().ToListAsync(); //obtiene todos las rutinas
        public async Task<List<SetsAndRepetitions>> GetSetsAndRepetitions() => await _connection.Table<SetsAndRepetitions>().ToListAsync(); //obtiene todos las rutinas

        public async Task<List<RoutinesExercises>> GetRoutinesExercises() => await _connection.Table<RoutinesExercises>().ToListAsync(); //obtiene todos los ejercicios de las rutinas

        public async Task<User> GetUserById(int id) //obtiene usuario por id
        {
            return await _connection.Table<User>().Where(x => x.UserID == id).FirstOrDefaultAsync();
        }
        public async Task<Exercise> GetExerciseById(int id) //obtiene ejercicio por id
        {
            return await _connection.Table<Exercise>().Where(x => x.execiseID == id).FirstOrDefaultAsync();
        }
        public async Task<Routines> GetRoutineById(int id) //obtiene rutina por id
        {
            return await _connection.Table<Routines>().Where(x => x.routineID == id).FirstOrDefaultAsync();
        }
        public async Task<RoutinesExercises> GetRoutinesExercisesByIdRoutine(int idRoutines) //obtiene rutina por id
        {
            return await _connection.Table<RoutinesExercises>().Where(x => x.Id == idRoutines).FirstOrDefaultAsync();
        }
        public async Task<SetsAndRepetitions> GetRoutinesExercisesByIdSetAndRepetitions(int idSetsAndReps) //obtiene rutina por id
        {
            return await _connection.Table<SetsAndRepetitions>().Where(x => x.setsAndRepsID == idSetsAndReps).FirstOrDefaultAsync();
        }
        public async Task<User> GetUserByEmail(string email) //obtiene usuario por email
        {
            return await _connection.Table<User>().Where(x => x.Email == email).FirstOrDefaultAsync();
        }
        public async Task Create(object create) //inserta objeto en cualquier tabla
        {
            await _connection.InsertAsync(create);
        }

        public async Task Update(object update) //actualiza objeto de cualquier tabla
        {
            await _connection.UpdateAsync(update);
        }

        public async Task Delete(object delete) //elimina objeto de cualquier tabla
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
        public async Task DeleteUserById(int id)
        {
            var exercise = await GetUserById(id);
            if (exercise != null)
            {
                await _connection.DeleteAsync(exercise);
            }
        }
    }
}

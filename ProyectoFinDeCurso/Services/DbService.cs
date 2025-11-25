using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using SQLite;

namespace ProyectoFinDeCurso.Services
{
    public class DbService
    {
        private const string DB_NAME = "DbService.db3";
        private readonly SQLiteAsyncConnection _connection;

        public DbService()
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, DB_NAME);
            _connection = new SQLiteAsyncConnection(path);
        }

        public async Task InitTablesAsync()
        {
            await _connection.CreateTableAsync<User>();
            await _connection.CreateTableAsync<Exercise>();
            await _connection.CreateTableAsync<Routines>();
            await _connection.CreateTableAsync<RoutinesExercises>();
        }

        // -----------------------------
        //     GETTERS SIN CACHÉ
        // -----------------------------

        public Task<List<User>> GetUsersAsync() =>
            _connection.Table<User>().ToListAsync();

        public Task<List<Exercise>> GetExercisesAsync() =>
            _connection.Table<Exercise>().ToListAsync();

        public Task<List<Routines>> GetRoutinesAsync() =>
            _connection.Table<Routines>().ToListAsync();

        public Task<List<RoutinesExercises>> GetRoutinesExercisesAsync() =>
            _connection.Table<RoutinesExercises>().ToListAsync();


        // -----------------------------
        //      GETTERS INDIVIDUALES
        // -----------------------------

        public Task<User> GetUserById(int id) =>
            _connection.Table<User>().Where(x => x.UserID == id).FirstOrDefaultAsync();

        public Task<User> GetUserByEmail(string email) =>
            _connection.Table<User>().Where(x => x.Email == email).FirstOrDefaultAsync();

        public Task<Exercise> GetExerciseById(int id) =>
            _connection.Table<Exercise>().Where(x => x.execiseID == id).FirstOrDefaultAsync();

        public Task<Routines> GetRoutineById(int id) =>
            _connection.Table<Routines>().Where(x => x.routineID == id).FirstOrDefaultAsync();

        public Task<RoutinesExercises> GetRoutinesExercisesByIdRoutine(int id) =>
            _connection.Table<RoutinesExercises>().Where(x => x.Id == id).FirstOrDefaultAsync();


        // -----------------------------
        //      CRUD
        // -----------------------------

        public Task Create(object obj) =>
            _connection.InsertAsync(obj);

        public Task Update(object obj) =>
            _connection.UpdateAsync(obj);

        public Task Delete(object obj) =>
            _connection.DeleteAsync(obj);


        public async Task DeleteExerciseById(int id)
        {
            var exercise = await GetExerciseById(id);
            if (exercise != null)
                await _connection.DeleteAsync(exercise);
        }

        public async Task DeleteUserById(int id)
        {
            var user = await GetUserById(id);
            if (user != null)
                await _connection.DeleteAsync(user);
        }


        // -----------------------------
        //  Crear admin si no existe
        // -----------------------------

        public async Task CreateUserAdmin()
        {
            var admin = new User
            {
                Name = "Admin",
                FirstSurname = "Admin",
                SecondSurname = "Admin",
                Email = "admin@admin.com",
                Password = "100000.cmGiRcXAazYJWv9YY7xDtw==.JXSvT+5gqMy6c9tyvGJ6RQIDA1GQxBqQrauSC3Hr1nA=",
                Phone = "123456789",
                userType = userTypeEnum.admin
            };

            var users = await GetUsersAsync();

            if (!users.Any(u => u.Email == admin.Email))
                await Create(admin);
        }
    }
}

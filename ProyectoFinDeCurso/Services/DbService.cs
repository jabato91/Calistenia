using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using SQLite;

namespace ProyectoFinDeCurso.Services
{
    public class DbService
    {
        private const string DB_NAME = "DbService.db3";
        private readonly SQLiteAsyncConnection _connection;

        private List<User>? _cachedUsers;
        private List<Exercise>? _cachedExercises;
        private List<Routines>? _cachedRoutines;
        private List<RoutinesExercises>? _cachedRoutinesExercises;
        private List<SetsAndRepetitions>? _cachedSetsAndReps;

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
            await _connection.CreateTableAsync<SetsAndRepetitions>();
        }

        public void ClearAllCache()
        {
            _cachedUsers = null;
            _cachedExercises = null;
            _cachedRoutines = null;
            _cachedRoutinesExercises = null;
            _cachedSetsAndReps = null;
        }


        public async Task<List<User>> GetUsersCached()
        {
            #if WINDOWS
                return await _connection.Table<User>().ToListAsync();
            #endif
            if (_cachedUsers != null) return _cachedUsers;
            _cachedUsers = await _connection.Table<User>().ToListAsync();
            return _cachedUsers;
        }

        public async Task<List<Exercise>> GetExercisesCached()
        {
            #if WINDOWS
                return await _connection.Table<Exercise>().ToListAsync();
            #endif
            if (_cachedExercises != null) return _cachedExercises;
            _cachedExercises = await _connection.Table<Exercise>().ToListAsync();
            return _cachedExercises;
        }

        public async Task<List<Routines>> GetRoutinesCached()
        {
            #if WINDOWS
                return await _connection.Table<Routines>().ToListAsync();
            #endif
            if (_cachedRoutines != null) return _cachedRoutines;
            _cachedRoutines = await _connection.Table<Routines>().ToListAsync();
            return _cachedRoutines;
        }

        public async Task<List<RoutinesExercises>> GetRoutinesExercisesCached()
        {
            #if WINDOWS
                return await _connection.Table<RoutinesExercises>().ToListAsync();
            #endif
            if (_cachedRoutinesExercises != null) return _cachedRoutinesExercises;
            _cachedRoutinesExercises = await _connection.Table<RoutinesExercises>().ToListAsync();
            return _cachedRoutinesExercises;
        }

        public async Task<List<SetsAndRepetitions>> GetSetsAndRepetitionsCached()
        {
            #if WINDOWS
                return await _connection.Table<SetsAndRepetitions>().ToListAsync();
            #endif
            if (_cachedSetsAndReps != null) return _cachedSetsAndReps;
            _cachedSetsAndReps = await _connection.Table<SetsAndRepetitions>().ToListAsync();
            return _cachedSetsAndReps;
        }

        public async Task<User> GetUserById(int id) =>
            await _connection.Table<User>().Where(x => x.UserID == id).FirstOrDefaultAsync();

        public async Task<User> GetUserByEmail(string email) =>
            await _connection.Table<User>().Where(x => x.Email == email).FirstOrDefaultAsync();

        public async Task<Exercise> GetExerciseById(int id) =>
            await _connection.Table<Exercise>().Where(x => x.execiseID == id).FirstOrDefaultAsync();

        public async Task<Routines> GetRoutineById(int id) =>
            await _connection.Table<Routines>().Where(x => x.routineID == id).FirstOrDefaultAsync();

        public async Task<RoutinesExercises> GetRoutinesExercisesByIdRoutine(int idRoutines) =>
            await _connection.Table<RoutinesExercises>().Where(x => x.Id == idRoutines).FirstOrDefaultAsync();

        public async Task<SetsAndRepetitions> GetSetsAndRepsById(int idSetsAndReps) =>
            await _connection.Table<SetsAndRepetitions>().Where(x => x.setsAndRepsID == idSetsAndReps).FirstOrDefaultAsync();


        public async Task Create(object create)
        {
            await _connection.InsertAsync(create);
            ClearAllCache(); // refrescar datos
        }

        public async Task Update(object update)
        {
            await _connection.UpdateAsync(update);
            ClearAllCache();
        }

        public async Task Delete(object delete)
        {
            await _connection.DeleteAsync(delete);
            ClearAllCache();
        }

        public async Task DeleteExerciseById(int id)
        {
            var exercise = await GetExerciseById(id);
            if (exercise != null)
            {
                await _connection.DeleteAsync(exercise);
                ClearAllCache();
            }
        }

        public async Task DeleteUserById(int id)
        {
            var user = await GetUserById(id);
            if (user != null)
            {
                await _connection.DeleteAsync(user);
                ClearAllCache();
            }
        }

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

            var users = await GetUsersCached();

            if (!users.Any(u => u.Email == admin.Email))
                await Create(admin);
        }
    }
}

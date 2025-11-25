using ProyectoFinDeCurso.Enums;
using ProyectoFinDeCurso.Models;
using SQLite;

namespace ProyectoFinDeCurso.Services
{
    public class DbService
    {
        private const string DB_NAME = "DbService.db3"; // Nombre de la base de datos
        private readonly SQLiteAsyncConnection _connection; // Conexión asíncrona a la base de datos SQLite

        public DbService() 
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, DB_NAME); // Ruta completa al archivo de la base de datos
            _connection = new SQLiteAsyncConnection(path); // Inicializa la conexión a la base de datos
        }

        public async Task InitTablesAsync() // Inicializa o crea las tablas en la base de datos
        {
            await _connection.CreateTableAsync<User>();
            await _connection.CreateTableAsync<Exercise>();
            await _connection.CreateTableAsync<Routines>();
            await _connection.CreateTableAsync<RoutinesExercises>();
        }


        public Task<List<User>> GetUsersAsync() => // Obtiene todos los usuarios de la tabla User
            _connection.Table<User>().ToListAsync();

        public Task<List<Exercise>> GetExercisesAsync() => // Obtiene todos los ejercicios de la tabla Exercise
            _connection.Table<Exercise>().ToListAsync();

        public Task<List<Routines>> GetRoutinesAsync() => // Obtiene todas las rutinas de la tabla Routines
            _connection.Table<Routines>().ToListAsync();

        public Task<List<RoutinesExercises>> GetRoutinesExercisesAsync() => // Obtiene todas las asociaciones de rutinas y ejercicios de la tabla RoutinesExercises
            _connection.Table<RoutinesExercises>().ToListAsync();


        public Task<User> GetUserById(int id) => // Obtiene un usuario por su ID
            _connection.Table<User>().Where(x => x.UserID == id).FirstOrDefaultAsync();


        public Task<Exercise> GetExerciseById(int id) => // obtiene un ejercicio por su ID
            _connection.Table<Exercise>().Where(x => x.execiseID == id).FirstOrDefaultAsync();

        public Task<Routines> GetRoutineById(int id) => // obtiene una rutina por su ID
            _connection.Table<Routines>().Where(x => x.routineID == id).FirstOrDefaultAsync();

        public Task<RoutinesExercises> GetRoutinesExercisesByIdRoutine(int id) => // obtiene una rutina-ejercicio por su ID
            _connection.Table<RoutinesExercises>().Where(x => x.Id == id).FirstOrDefaultAsync();



        public Task Create(object obj) => // Crea un nuevo registro en la tabla correspondiente, independientemente del tipo de objeto
            _connection.InsertAsync(obj);

        public Task Update(object obj) => // Actualiza un registro existente en la tabla correspondiente, independientemente del tipo de objeto
            _connection.UpdateAsync(obj);

        public Task Delete(object obj) => // Elimina un registro de la tabla correspondiente, independientemente del tipo de objeto
            _connection.DeleteAsync(obj);


        public async Task DeleteExerciseById(int id) // Elimina un ejercicio por su ID
        {
            var exercise = await GetExerciseById(id);
            if (exercise != null)
                await _connection.DeleteAsync(exercise);
        }

        public async Task DeleteUserById(int id) // Elimina un usuario por su ID
        {
            var user = await GetUserById(id);
            if (user != null)
                await _connection.DeleteAsync(user);
        }



        public async Task CreateUserAdmin() // Crea un usuario administrador si no existe ya uno
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

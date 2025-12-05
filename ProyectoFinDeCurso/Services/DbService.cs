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
            try
            {
                string path = Path.Combine(FileSystem.AppDataDirectory, DB_NAME); // Ruta completa al archivo de la base de datos
                _connection = new SQLiteAsyncConnection(path); // Inicializa la conexión a la base de datos
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] No se pudo crear la conexión: {ex.Message}");
            }
        }

        public async Task InitTablesAsync() // Inicializa o crea las tablas en la base de datos
        {
            try
            {
                await _connection.CreateTableAsync<User>();
                await _connection.CreateTableAsync<Exercise>();
                await _connection.CreateTableAsync<Routines>();
                await _connection.CreateTableAsync<RoutinesExercises>();
                await _connection.CreateTableAsync<RegisterLogging>();
                await _connection.CreateTableAsync<Alarm>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] No se pudieron crear las tablas: {ex.Message}");
            }
        }


        public async Task<List<User>> GetUsersAsync() //obtiene los usuarios de la base de datos
        {
            try
            {
                return await _connection.Table<User>().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] GetUsersAsync: {ex.Message}");
                return new List<User>();
            }
        }

        public async Task<List<Exercise>> GetExercisesAsync() //obtiene los ejercicios de la base de datos
        {
            try
            {
                return await _connection.Table<Exercise>().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] GetExercisesAsync: {ex.Message}");
                return new List<Exercise>();
            }
        }

        public async Task<List<Routines>> GetRoutinesAsync() //obtiene las rutinas de la base de datos
        {
            try
            {
                return await _connection.Table<Routines>().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] GetRoutinesAsync: {ex.Message}");
                return new List<Routines>();
            }
        }

        public async Task<List<RoutinesExercises>> GetRoutinesExercisesAsync() //obtiene los ejercicios de la rutina de la base de datos
        {
            try
            {
                return await _connection.Table<RoutinesExercises>().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] GetRoutinesExercisesAsync: {ex.Message}");
                return new List<RoutinesExercises>();
            }
        }

        public async Task<List<RegisterLogging>> GetRegisterLoggingAsync() //obtiene los registros de las rutinas realizadas
        {
            try
            {
                return await _connection.Table<RegisterLogging>().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] GetRegisterLoggingAsync: {ex.Message}");
                return new List<RegisterLogging>();
            }
        }

        public async Task<List<Alarm>> GetAlarmsAsync() //obtiene las alarmas
        {
            try
            {
                return await _connection.Table<Alarm>().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] GetAlarmsAsync: {ex.Message}");
                return new List<Alarm>();
            }
        }

        public async Task<User> GetUserById(int id) //obtiene el usuario a partir de la id de la base de datos
        {
            try
            {
                return await _connection.Table<User>().Where(x => x.UserID == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] GetUserById: {ex.Message}");
                return null;
            }
        }

        public async Task<Exercise> GetExerciseById(int id) //obtieene un ejercico a partir de la id de la base de datos
        {
            try
            {
                return await _connection.Table<Exercise>().Where(x => x.execiseID == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] GetExerciseById: {ex.Message}");
                return null;
            }
        }

        public async Task<Routines> GetRoutineById(int id) //obtiene la rutina a partir de la id de la base de datos
        {
            try
            {
                return await _connection.Table<Routines>().Where(x => x.routineID == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] GetRoutineById: {ex.Message}");
                return null;
            }
        }

        public async Task<RoutinesExercises> GetRoutinesExercisesByIdRoutine(int id) //obtiene los ejercicios de la rutina a partir de la id
        {
            try
            {
                return await _connection.Table<RoutinesExercises>().Where(x => x.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] GetRoutinesExercisesByIdRoutine: {ex.Message}");
                return null;
            }
        }

        public async Task<RegisterLogging> GetRegisterLoggingByIdRegister(int id) //obtiene los registros de la rutina a partir de la ip de la base de datos
        {
            try
            {
                return await _connection.Table<RegisterLogging>().Where(x => x.registerID == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] GetRegisterLoggingByIdRegister: {ex.Message}");
                return null;
            }
        }

        public async Task<Alarm> GetAlarmByIdAlarm(int id) //obtiene la alarma a partir de la id de la base de datos
        {
            try
            {
                return await _connection.Table<Alarm>().Where(x => x.AlarmID == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] GetAlarmByIdAlarm: {ex.Message}");
                return null;
            }
        }

        public async Task Create(object obj) //crea una fila de cualquier tabla
        {
            try
            {
                await _connection.InsertAsync(obj);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] Create: {ex.Message}");
            }
        }

        public async Task Update(object obj)//modifica una fila de cualquier tabla
        {
            try
            {
                await _connection.UpdateAsync(obj);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] Update: {ex.Message}");
            }
        }

        public async Task Delete(object obj)//elimina una fila de cualquier tabla
        {
            try
            {
                await _connection.DeleteAsync(obj);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] Delete: {ex.Message}");
            }
        }

        public async Task DeleteExerciseById(int id) //elimina el ejercicio a partir de la id
        {
            try
            {
                var exercise = await GetExerciseById(id);
                if (exercise != null)
                    await _connection.DeleteAsync(exercise);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] DeleteExerciseById: {ex.Message}");
            }
        }


        public async Task DeleteUserById(int id) //elimina usuario a partir de la id
        {
            try
            {
                var user = await GetUserById(id);
                if (user != null)
                    await _connection.DeleteAsync(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] DeleteUserById: {ex.Message}");
            }
        }

        public async Task CreateUserAdmin() //crea el usuario administrador
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] CreateUserAdmin: {ex.Message}");
            }
        }
    }
}

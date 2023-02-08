using Core.Models;
using System.Data.SqlClient;

namespace Data.MSSQLRepository
{
    public class UserRepository : MainRepository<User>
    {
        private TrophyRepository _trophyRepository;
        public UserRepository(string connectionString, string tableName, string createQuery, string updateQuery,
            string getQuery, string getAllQuery, TrophyRepository trophyRepository)
            : base(connectionString, tableName, createQuery, updateQuery, getQuery, getAllQuery)
        {
            _trophyRepository = trophyRepository;
        }

        public UserRepository(string connectionString, TrophyRepository trophyRepository)
            : this(connectionString,
                  "Users",
                  @"INSERT INTO Users(FirstName,MiddleName, LastName, Age, TrophyId) 
                    VALUES (@firstName,@middleName, @lastName, @age, @trophy)",
                  @"UPDATE Users a SET a.FirstName = @firstName, a.MiddleName = @middleName, a.LastName = @lastName, 
                    a.Age = @age, a.TrophyId = @trophy
                    WHERE a.Id = @id",
                  "SELECT * FROM Users a WHERE Id=@id",
                  "SELECT * FROM Users",
                  trophyRepository)
        {
        }

        public override async Task CreateAsync(User entity)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync().ConfigureAwait(false);

                using (var sqlDataAdapter = new SqlDataAdapter(createQuery, sqlConnection))
                {
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@firstName", entity.FirstName);
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@middleName", entity.MiddleName);
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@lastName", entity.LastName);
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@age", entity.Age);
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@trophy", entity.Trophy.Id);

                    sqlDataAdapter.SelectCommand.ExecuteNonQuery();
                }
            }
        }

        public override async Task<List<User>> GetAllAsync()
        {
            var users = new List<User>();

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync().ConfigureAwait(false);

                using (var sqlCommand = new SqlCommand(getAllQuery, sqlConnection))
                {
                    using (SqlDataReader sqlReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        while (await sqlReader.ReadAsync())
                        {
                            int userId = sqlReader.GetInt32(0);
                            string firstName = sqlReader.GetString(1);
                            string middleName = sqlReader.GetString(2);
                            string lastName = sqlReader.GetString(3);
                            int age = sqlReader.GetInt32(4);
                            int trophyId = sqlReader.GetInt32(5);
                            Trophy trophy = await _trophyRepository.GetAsync(trophyId);

                            var user = new User()
                            {
                                Id = userId,
                                FirstName = firstName,
                                MiddleName = middleName,
                                LastName = lastName,
                                Age = age,
                                Trophy = trophy
                            };

                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }

        public override async Task<User> GetAsync(int id)
        {
            var user = new User();

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync().ConfigureAwait(false);

                using (var sqlCommand = new SqlCommand(getQuery, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader sqlReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        while (await sqlReader.ReadAsync())
                        {
                            int userId = sqlReader.GetInt32(0);
                            string firstName = sqlReader.GetString(1);
                            string middleName = sqlReader.GetString(2);
                            string lastName = sqlReader.GetString(3);
                            int age = sqlReader.GetInt32(4);
                            int trophyId = sqlReader.GetInt32(5);
                            Trophy trophy = await _trophyRepository.GetAsync(trophyId);


                            user.Id = userId;
                            user.FirstName = firstName;
                            user.MiddleName = middleName;
                            user.LastName = lastName;
                            user.Age = age;
                            user.Trophy = trophy;
                        }
                    }
                }
            }

            return user;
        }

        public override async Task UpdateAsync(User entity)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync().ConfigureAwait(false);

                using (var sqlDataAdapter = new SqlDataAdapter(updateQuery, sqlConnection))
                {
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@id", entity.Id);
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@firstName", entity.FirstName);
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@middleName", entity.MiddleName);
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@lastName", entity.LastName);
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@age", entity.Age);
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@trophy", entity.Trophy);

                    sqlDataAdapter.SelectCommand.ExecuteNonQuery();
                }
            }
        }
    }
}

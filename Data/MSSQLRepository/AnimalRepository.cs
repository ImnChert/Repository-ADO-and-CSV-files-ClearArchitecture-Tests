using Core.Models;
using System.Data.SqlClient;

namespace Data.MSSQLRepository
{
    public class AnimalRepository : MainRepository<Animal>
    {
        private TypeRepository _typeRepository;
        public AnimalRepository(string connectionString, string tableName, string createQuery, string updateQuery,
            string getQuery, string getAllQuery, TypeRepository typeRepository)
            : base(connectionString, tableName, createQuery, updateQuery, getQuery, getAllQuery)
        {
            _typeRepository = typeRepository;
        }

        public AnimalRepository(string connectionString, TypeRepository typeRepository)
            : this(connectionString,
                  "Animals",
                  "INSERT INTO Animals(Age,TypeId) VALUES (@age,@type)",
                  "UPDATE Animals a SET a.Age = @age, a.TypeId = @type WHERE a.Id = @id",
                  "SELECT * FROM Animals a WHERE Id=@id",
                  "SELECT * FROM Animals",
                  typeRepository)
        {
        }

        public override async Task CreateAsync(Animal entity)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();

                using (var sqlDataAdapter = new SqlDataAdapter(createQuery, sqlConnection))
                {
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@age", entity.Age);
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@type", entity.Type.Id);

                    sqlDataAdapter.SelectCommand.ExecuteNonQuery();
                }
            }
        }

        public override async Task<List<Animal>> GetAllAsync()
        {
            var animals = new List<Animal>();

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();

                using (var sqlCommand = new SqlCommand(getAllQuery, sqlConnection))
                {
                    using (SqlDataReader sqlReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlReader.Read())
                        {
                            int animalId = sqlReader.GetInt16(0);
                            int typeId = sqlReader.GetInt16(1);
                            TypeAnimal type = await _typeRepository.GetAsync(typeId);
                            int age = sqlReader.GetInt16(2);

                            var animal = new Animal()
                            {
                                Id = animalId,
                                Type = type,
                                Age = age
                            };

                            animals.Add(animal);
                        }
                    }
                }
            }

            return animals;
        }

        public override async Task<Animal> GetAsync(int id)
        {
            var animal = new Animal();

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();

                using (var sqlCommand = new SqlCommand(getQuery, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader sqlReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlReader.Read())
                        {
                            int animalId = sqlReader.GetInt16(0);
                            int typeId = sqlReader.GetInt16(1);
                            TypeAnimal type = await _typeRepository.GetAsync(typeId);
                            int age = sqlReader.GetInt16(2);

                            animal.Id = animalId;
                            animal.Type = type;
                            animal.Age = age;
                        }
                    }
                }
            }

            return animal;
        }

        public override async Task UpdateAsync(Animal entity)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();

                using (var sqlDataAdapter = new SqlDataAdapter(updateQuery, sqlConnection))
                {
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@id", entity.Id);
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@type", entity.Type.Id);
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@age", entity.Age);

                    sqlDataAdapter.SelectCommand.ExecuteNonQuery();
                }
            }
        }
    }
}

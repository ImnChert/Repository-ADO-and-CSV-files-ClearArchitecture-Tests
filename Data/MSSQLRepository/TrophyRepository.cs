using Core.Models;
using System.Data.SqlClient;

namespace Data.MSSQLRepository
{
    public class TrophyRepository : MainRepository<Trophy>
    {
        private AnimalRepository _animalRepository;

        public TrophyRepository(string connectionString, string tableName, string createQuery, string updateQuery,
            string getQuery, string getAllQuery, AnimalRepository animalRepository)
            : base(connectionString, tableName, createQuery, updateQuery, getQuery, getAllQuery)
        {
            _animalRepository = animalRepository;
        }

        public override async Task CreateAsync(Trophy entity)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();

                using (var sqlDataAdapter = new SqlDataAdapter(createQuery, sqlConnection))
                {
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@animalId", entity.Animal.Id);
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@date", entity.DateOfMurder);

                    sqlDataAdapter.SelectCommand.ExecuteNonQuery();
                }
            }
        }

        public override async Task<List<Trophy>> GetAllAsync()
        {
            var trophes = new List<Trophy>();

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();

                using (var sqlCommand = new SqlCommand(getAllQuery, sqlConnection))
                {
                    using (SqlDataReader sqlReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlReader.Read())
                        {
                            int trophyId = sqlReader.GetInt16(0);
                            int animalId = sqlReader.GetInt16(1);
                            Animal animal = await _animalRepository.GetAsync(animalId);
                            DateTime dateOfMurder = sqlReader.GetDateTime(2);


                            var trophy = new Trophy()
                            {
                                Id = trophyId,
                                Animal = animal,
                                DateOfMurder = dateOfMurder
                            };

                            trophes.Add(trophy);
                        }
                    }
                }
            }

            return trophes;
        }

        public override async Task<Trophy> GetAsync(int id)
        {
            var trophy = new Trophy();

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();

                using (var sqlCommand = new SqlCommand(getQuery, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader sqlReader = sqlCommand.ExecuteReader())
                    {
                        while (await sqlReader.ReadAsync())
                        {
                            int trophyId = sqlReader.GetInt16(0);
                            int animalId = sqlReader.GetInt16(1);
                            Animal animal = await _animalRepository.GetAsync(animalId);
                            DateTime dateOfMurder = sqlReader.GetDateTime(2);

                            trophy.Id = trophyId;
                            trophy.Animal = animal;
                            trophy.DateOfMurder = dateOfMurder;
                        }
                    }
                }
            }

            return trophy;
        }

        public override async Task UpdateAsync(Trophy entity)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();

                using (var sqlDataAdapter = new SqlDataAdapter(updateQuery, sqlConnection))
                {
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@id", entity.Id);
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@animalId", entity.Animal.Id);
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@dateOfMurder", entity.DateOfMurder);

                    sqlDataAdapter.SelectCommand.ExecuteNonQuery();
                }
            }
        }
    }
}

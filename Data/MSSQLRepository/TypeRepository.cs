using Core.Models;
using System.Data.SqlClient;

namespace Data.MSSQLRepository
{
    public class TypeRepository : MainRepository<TypeAnimal>
    {
        public TypeRepository(string connectionString, string tableName, string createQuery, string updateQuery, string getQuery, string getAllQuery)
            : base(connectionString, tableName, createQuery, updateQuery, getQuery, getAllQuery)
        {
        }

        public override async Task CreateAsync(TypeAnimal entity)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();

                using (var sqlDataAdapter = new SqlDataAdapter(createQuery, sqlConnection))
                {
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@name", entity.Name);

                    sqlDataAdapter.SelectCommand.ExecuteNonQuery();
                }
            }
        }

        public override async Task<List<TypeAnimal>> GetAllAsync()
        {
            var types = new List<TypeAnimal>();

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();

                using (var sqlCommand = new SqlCommand(getAllQuery, sqlConnection))
                {
                    using (SqlDataReader sqlReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlReader.Read())
                        {
                            int typeId = sqlReader.GetInt16(0);
                            string name = sqlReader.GetString(1);


                            var type = new TypeAnimal()
                            {
                                Id = typeId,
                                Name = name
                            };

                            types.Add(type);
                        }
                    }
                }
            }

            return types;
        }

        public override async Task<TypeAnimal> GetAsync(int id)
        {
            var type = new TypeAnimal();

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
                            int typeId = sqlReader.GetInt16(0);
                            string name = sqlReader.GetString(1);

                            type.Id = typeId;
                            type.Name = name;
                        }
                    }
                }
            }

            return type;
        }

        public override async Task UpdateAsync(TypeAnimal entity)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();

                using (var sqlDataAdapter = new SqlDataAdapter(updateQuery, sqlConnection))
                {
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@id", entity.Id);
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@name", entity.Name);

                    sqlDataAdapter.SelectCommand.ExecuteNonQuery();
                }
            }
        }
    }
}

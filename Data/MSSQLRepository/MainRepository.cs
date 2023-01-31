using Core;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.MSSQLRepository
{
    public abstract class MainRepository<T> : IRepository<T> where T : Entity
    {
        protected string connectionString;
        protected string tableName;
        protected string createQuery;
        protected string updateQuery;
        protected string deleteQuery;
        protected string getQuery;
        protected string getAllQuery;

        protected MainRepository(string connectionString, string tableName, string createQuery, string updateQuery,
             string getQuery, string getAllQuery)
        {
            this.connectionString = connectionString;
            this.tableName = tableName;
            this.createQuery = createQuery;
            this.updateQuery = updateQuery;
            this.deleteQuery = $"DELETE FROM {tableName} a WHERE a.ID = @id";
            this.getQuery = getQuery;
            this.getAllQuery = getAllQuery;
        }

        public abstract Task CreateAsync(T entity);

        public async Task DeleteAsync(T entity)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();

                using (var sqlDataAdapter = new SqlDataAdapter(deleteQuery, sqlConnection))
                {
                    sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@id", entity.Id);
                    sqlDataAdapter.SelectCommand.ExecuteNonQuery();
                }
            }
        }

        public abstract Task<List<T>> GetAllAsync();
        public abstract Task<T> GetAsync(int id);
        public abstract Task UpdateAsync(T entity);
    }
}

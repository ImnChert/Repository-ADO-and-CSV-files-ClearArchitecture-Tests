using Core.Models;
using Interfaces;
using System.Text;
using System.Text.RegularExpressions;

namespace Data.CSVRepository
{
    internal class TypeRepository : IRepository<TypeAnimal>
    {
        private string _filename;

        public TypeRepository(string filename)
        {
            _filename = filename;
        }

        public async Task CreateAsync(TypeAnimal entity)
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append(entity.Id).Append(";")
                .Append(entity.Name);

            string str = strBuilder.ToString();

            using (var streamWriter = new StreamWriter(_filename, true))
            {
                await streamWriter.WriteAsync(str);
            }
        }

        public async Task DeleteAsync(TypeAnimal entity)
        {
            List<TypeAnimal> typeAnimals = (await GetAllAsync())
                .Where(a => a.Id != entity.Id)
                .ToList();

            using (var writer = new StreamWriter(_filename, false))
            {
                typeAnimals.ForEach(async item => await CreateAsync(item));
            }
        }

        public async Task<List<TypeAnimal>> GetAllAsync()
        {
            int i = 0;
            var types = new List<TypeAnimal>();
            var regex = new Regex(";");

            using (var reader = new StreamReader(_filename))
            {
                string line;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    string[] arr = regex.Split(line);

                    int typeId = Convert.ToInt16(arr[0]);
                    string name = Convert.ToString(arr[1]);

                    var trophy = new TypeAnimal()
                    {
                        Id = typeId,
                        Name = name
                    };

                    types.Add(trophy);

                    i++;
                }
            }

            return types;
        }

        public async Task<TypeAnimal> GetAsync(int id)
             => (await GetAllAsync()).First(x => x.Id == id);

        public async Task UpdateAsync(TypeAnimal entity)
        {
            await DeleteAsync(entity);
            await CreateAsync(entity);
        }
    }
}

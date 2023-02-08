using Core.Models;
using Interfaces;
using System.Text;
using System.Text.RegularExpressions;

namespace Data.CSVRepository
{
    public class AnimalRepository : IRepository<Animal>
    {
        private string _filename;
        private TypeRepository _typeRepository;

        public AnimalRepository(string filename, TypeRepository typeRepository)
        {
            _filename = filename;
            _typeRepository = typeRepository;
        }

        public async Task CreateAsync(Animal entity)
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append(entity.Id).Append(";")
                .Append(entity.Age).Append(";")
                .Append(entity.Type.Id);

            string str = strBuilder.ToString();

            using (var streamWriter = new StreamWriter(_filename, true))
            {
                await streamWriter.WriteAsync(str);
            }
        }

        public async Task DeleteAsync(Animal entity)
        {
            List<Animal> animals = (await GetAllAsync())
                .Where(a => a.Id != entity.Id)
                .ToList();

            using (var writer = new StreamWriter(_filename, false))
            {
                animals.ForEach(async item => await CreateAsync(item));
            }
        }

        public async Task<List<Animal>> GetAllAsync()
        {
            int i = 0;
            var animals = new List<Animal>();
            var regex = new Regex(";");

            using (var reader = new StreamReader(_filename))
            {
                string line;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    string[] arr = regex.Split(line);

                    int animalId = Convert.ToInt16(arr[0]);
                    int age = Convert.ToInt16(arr[2]);
                    int typeId = Convert.ToInt16(arr[3]);
                    TypeAnimal type = await _typeRepository.GetAsync(typeId);

                    var animal = new Animal()
                    {
                        Id = animalId,
                        Age = age,
                        Type = type
                    };

                    animals.Add(animal);

                    i++;
                }
            }

            return animals;
        }

        public async Task<Animal> GetAsync(int id)
            => (await GetAllAsync()).First(x => x.Id == id);

        public async Task UpdateAsync(Animal entity)
        {
            await DeleteAsync(entity);
            await CreateAsync(entity);
        }
    }
}

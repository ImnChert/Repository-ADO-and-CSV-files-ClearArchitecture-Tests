using Core.Models;
using Data.MSSQLRepository;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Data.CSVRepository
{
    internal class TrophyRepository : IRepository<Trophy>
    {
        private string _filename;
        private AnimalRepository _animalRepository;

        public TrophyRepository(string filename, AnimalRepository animalRepository)
        {
            _filename = filename;
            _animalRepository = animalRepository;
        }

        public async Task CreateAsync(Trophy entity)
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append(entity.Id).Append(";")
                .Append(entity.Animal.Id).Append(";")
                .Append(entity.DateOfMurder);

            string str = strBuilder.ToString();

            using (var streamWriter = new StreamWriter(_filename, true))
            {
                await streamWriter.WriteAsync(str);
            }
        }

        public async Task DeleteAsync(Trophy entity)
        {
            List<Trophy> animals = (await GetAllAsync())
                .Where(a => a.Id != entity.Id)
                .ToList();

            using (var writer = new StreamWriter(_filename, false))
            {
                animals.ForEach(async item => await CreateAsync(item));
            }
        }

        public async Task<List<Trophy>> GetAllAsync()
        {
            int i = 0;
            var trophes = new List<Trophy>();
            var regex = new Regex(";");

            using (var reader = new StreamReader(_filename))
            {
                string line;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    string[] arr = regex.Split(line);

                    int trophyId = Convert.ToInt16(arr[0]);
                    int animalId = Convert.ToInt16(arr[1]);
                    Animal animal = await _animalRepository.GetAsync(animalId);
                    DateTime dateOfMurder = Convert.ToDateTime(arr[2]);

                    var trophy = new Trophy()
                    {
                        Id = trophyId,
                        Animal = animal,
                        DateOfMurder = dateOfMurder
                    };

                    trophes.Add(trophy);

                    i++;
                }
            }

            return trophes;
        }

        public async Task<Trophy> GetAsync(int id)
             => (await GetAllAsync()).First(x => x.Id == id);

        public async Task UpdateAsync(Trophy entity)
        {
            await DeleteAsync(entity);
            await CreateAsync(entity);
        }
    }
}

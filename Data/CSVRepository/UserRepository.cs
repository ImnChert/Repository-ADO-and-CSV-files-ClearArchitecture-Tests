using Core.Models;
using Interfaces;
using System.Text;
using System.Text.RegularExpressions;

namespace Data.CSVRepository
{
    public class UserRepository : IRepository<User>
    {
        private string _filename;
        private TrophyRepository _trophyRepository;

        public UserRepository(string filename, TrophyRepository trophyRepository)
        {
            _filename = filename;
            _trophyRepository = trophyRepository;
        }

        public async Task CreateAsync(User entity)
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append(entity.Id).Append(";")
                .Append(entity.FirstName).Append(";")
                .Append(entity.MiddleName).Append(";")
                .Append(entity.LastName).Append(";")
                .Append(entity.Age).Append(";")
                .Append(entity.Trophy.Id);

            string str = strBuilder.ToString();

            using (var streamWriter = new StreamWriter(_filename, true))
            {
                await streamWriter.WriteAsync(str);
            }
        }

        public async Task DeleteAsync(User entity)
        {
            List<User> users = (await GetAllAsync())
                .Where(a => a.Id != entity.Id)
                .ToList();

            using (var writer = new StreamWriter(_filename, false))
            {
                users.ForEach(async item => await CreateAsync(item));
            }
        }

        public async Task<List<User>> GetAllAsync()
        {
            int i = 0;
            var users = new List<User>();
            var regex = new Regex(";");

            using (var reader = new StreamReader(_filename))
            {
                string line;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    string[] arr = regex.Split(line);

                    int userId = Convert.ToInt16(arr[0]);
                    string firstName = Convert.ToString(arr[1]);
                    string middleName = Convert.ToString(arr[2]);
                    string lastName = Convert.ToString(arr[3]);
                    int age = Convert.ToInt16(arr[4]);
                    int trophyId = Convert.ToInt16(arr[5]);
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

                    i++;
                }
            }

            return users;
        }

        public async Task<User> GetAsync(int id)
            => (await GetAllAsync()).First(x => x.Id == id);

        public async Task UpdateAsync(User entity)
        {
            await DeleteAsync(entity);
            await CreateAsync(entity);
        }
    }
}

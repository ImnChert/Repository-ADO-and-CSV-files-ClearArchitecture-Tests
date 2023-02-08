using Core.Models;
using Data.MSSQLRepository;
using Data.CSVRepository;
using Interfaces;

namespace HunterWinForms
{
    public partial class Form1 : Form
    {
        private IRepository<User> _repository;
        private List<User> _users = new List<User>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Ó·ÌÓ‚ËÚ¸ToolStripMenuItem_ClickAsync(object sender, EventArgs e)
        {
            foreach (User user in _users)
            {
                bool flag = true;

                for (int i = 0; i < dataGridView1.RowCount && flag; i++)
                {
                    var row = dataGridView1.Rows[i];
                    int id = (int)row.Cells[4].Value;

                    if (user.Id == id)
                    {
                        flag = false;

                        string firstName = (string)row.Cells[0].Value;
                        string middleName = (string)row.Cells[1].Value;
                        string lastName = (string)row.Cells[2].Value;
                        string animalName = (string)row.Cells[3].Value;

                        user.FirstName = firstName;
                        user.MiddleName = middleName;
                        user.LastName = lastName;
                        user.Trophy.Animal.Type.Name = animalName;

                        _repository.UpdateAsync(user);
                    }
                }
            }
        }

        private void mSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string connectionString = @"Server=DESKTOP-CTBUCT0\SQLEXPRESS;Database=laba1;Trusted_Connection=True;";


            var typeRepository = new Data.MSSQLRepository.TypeRepository(connectionString);
            var animalRepository = new Data.MSSQLRepository.AnimalRepository(connectionString, typeRepository);
            var trophyRepository = new Data.MSSQLRepository.TrophyRepository(connectionString, animalRepository);
            _repository = new Data.MSSQLRepository.UserRepository(connectionString, trophyRepository);
        }

        private void cSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string typeFilename = "type.csv";
            var typeRepository = new Data.CSVRepository.TypeRepository(typeFilename);

            string animalFilename = "animal.csv";
            var animalRepository = new Data.CSVRepository.AnimalRepository(animalFilename, typeRepository);

            string trophyFilename = "trophy.csv";
            var trophyRepository = new Data.CSVRepository.TrophyRepository(trophyFilename, animalRepository);

            string userFilename = "user.csv";
            _repository = new Data.CSVRepository.UserRepository(userFilename, trophyRepository);
        }

        private void Á‡ÔËÒ‡Ú¸ToolStripMenuItem_ClickAsync(object sender, EventArgs e)
        {
            _users.ForEach(async u => await _repository.DeleteAsync(u));
            _users.ForEach(async u => await _repository.CreateAsync(u));
        }

        private void Û‰‡ÎËÚ¸ToolStripMenuItem_ClickAsync(object sender, EventArgs e)
        {
            int selectedRow = dataGridView1.CurrentCell.RowIndex;
            int id = (int)dataGridView1[4, selectedRow].Value;
            User user = _users.Find(x => x.Id == id);
            _users.Remove(user);
            _repository.DeleteAsync(user);
        }

        private void Ò˜ËÚ‡Ú¸ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var _users = _repository.GetAllAsync().Result;

            for (int row = 0; row < _users.Count; row++)
            {
                var user = _users[row];
                dataGridView1[0, row].Value = user.FirstName;
                dataGridView1[1, row].Value = user.MiddleName;
                dataGridView1[2, row].Value = user.LastName;
                dataGridView1[3, row].Value = user.Trophy.Animal.Type.Name;
                dataGridView1[4, row].Value = user.Id;
            }
        }
    }
}
namespace Core.Models
{
    public class User : Entity
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public Trophy Trophy { get; set; }
    }
}

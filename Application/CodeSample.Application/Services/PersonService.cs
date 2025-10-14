using CodeSample.Domain.Entities;

namespace CodeSample.Application.Services
{
    public class PersonService
    {
        private readonly List<Person> _people = new();

        public Person Create(string firstName, string lastName, string nationalCode, DateTime birthDate)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required.");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required.");

            if (nationalCode is null || nationalCode.Length != 10)
                throw new ArgumentException("National code must be exactly 10 digits.");

            if (birthDate > DateTime.Now)
                throw new ArgumentException("Birth date cannot be in the future.");

            var person = new Person
            {
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                NationalCode = nationalCode.Trim(),
                BirthDate = birthDate
            };

            _people.Add(person);
            return person;
        }

        public Person? Get(Guid id)
            => _people.FirstOrDefault(p => p.Id == id);

        public List<Person> GetAll()
            => _people.ToList();

        public bool Delete(Guid id)
        {
            var person = Get(id);
            if (person == null) return false;

            _people.Remove(person);
            return true;
        }

        public bool Update(Guid id, string firstName, string lastName, string nationalCode, DateTime birthDate)
        {
            var person = Get(id);
            if (person == null) return false;

            person.FirstName = firstName;
            person.LastName = lastName;
            person.NationalCode = nationalCode;
            person.BirthDate = birthDate;
            return true;
        }
    }
}

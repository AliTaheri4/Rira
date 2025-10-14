using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSample.Domain.Entities
{
    public class Person
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string NationalCode { get; set; } = "";
        public DateTime BirthDate { get; set; }
    }

    public class PersonService
    {
        private readonly List<Person> _people = new();

        public Person Create(string firstName, string lastName, string nationalCode, DateTime birthDate)
        {
            var person = new Person
            {
                FirstName = firstName,
                LastName = lastName,
                NationalCode = nationalCode,
                BirthDate = birthDate
            };

            _people.Add(person);
            return person;
        }

        public Person? Get(Guid id) => _people.FirstOrDefault(p => p.Id == id);
    }

}

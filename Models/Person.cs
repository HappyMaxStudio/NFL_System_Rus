using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using NFLFantasySystem.Interfaces;

namespace NFLFantasySystem.Models
{
    public class Person : IDateCounter
    {
        public int Id { get; set; }
        [Column("Имя")]
        public string ?Name { get; private set; }
        [Column("Фамилия")]
        public string ?Surname { get; private set; }
        [Column("Почта")]
        public string ?Email { get; private set; }
        [Column("Дата регистрации")]
        public DateOnly RegistrationDate { get; private set; }
        public Person(string name, string surname, string email)
        {
            Name = name;
            Surname = surname;
            RegistrationDate = DateOnly.FromDateTime(DateTime.Now);
            Email = email;
        }


        public void Deconstruct(out string personName, out string personSurname)
        {
            personName = Name!;
            personSurname = Surname!;
        }

        public virtual void ChangeData(Person objectInfo)
        {
            Id = objectInfo.Id;
            Name = objectInfo.Name;
            Surname = objectInfo.Surname;
            Email = objectInfo.Email;
        }
    }
}

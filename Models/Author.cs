using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace NFLFantasySystem.Models
{
    public class Author : Person
    {
        [Column("Статус")]
        public string ?Status { get; private set; }
        [Column("Рейтинг статьи")]
        public double CurrentRating {  get; private set; } 

        public Author(string currentStatus, string name, string surname, string email) : base(name, surname, email)
        {
            Status = currentStatus;
        }

        public void ChangeCurrentRating(int rating)
        {
            CurrentRating = rating;
        }

        public void ChangeStatus (string status)
        {
            Status = status;
        }

        public override void ChangeData(Person AuthorData)
        {
            base.ChangeData(AuthorData);
            Author author = (Author) AuthorData;
            Status = author.Status;
        }
    }
}

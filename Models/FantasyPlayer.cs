using System.ComponentModel.DataAnnotations.Schema;

namespace NFLFantasySystem.Models
{
    public class FantasyPlayer : Person
    {
        [Column("Текущая лига")]
        public string ?CurrentLeague {  get; set; }

        public FantasyPlayer(string currentLeague, string name, string surname, string email) : base(name, surname, email)
        {
            CurrentLeague = currentLeague;
        }
        public override void ChangeData(Person fantasyPlayerData)
        {
            base.ChangeData(fantasyPlayerData);
            FantasyPlayer player = (FantasyPlayer)fantasyPlayerData;
            CurrentLeague = player.CurrentLeague;
        }
    }
}

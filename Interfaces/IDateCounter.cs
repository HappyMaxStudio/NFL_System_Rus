namespace NFLFantasySystem.Interfaces
{
    public interface IDateCounter
    {
        DateOnly RegistrationDate { get; }
        int ReturnDaysRegistered()
        {
            return DateOnly.FromDateTime(DateTime.Now).DayNumber - RegistrationDate.DayNumber;
        }
    }
}

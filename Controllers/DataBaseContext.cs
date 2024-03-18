namespace NFLFantasySystem.Controllers
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using NFLFantasySystem.Models;
    using System.Reflection;
    using System.Reflection.Emit;

    public class DataBaseContext : DbContext
    {
        readonly StreamWriter TextLogStream = new StreamWriter("LogFiles/databaselog.txt");
        public DbSet<Article> Articles { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<FantasyPlayer> FantasyPlayers { get; set;}
        public DbSet<Person> People { get; set; }

        public DataBaseContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=nflRusDb.db");
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Trace);
            optionsBuilder.LogTo(TextLogStream.WriteLine, LogLevel.Information);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>().ToTable("Пользователи");
            modelBuilder.Entity<Author>().ToTable("Авторы статей");
            modelBuilder.Entity<FantasyPlayer>().ToTable("Фентези-игроки");
            modelBuilder.Entity<Person>().HasIndex(person => person.Email).IsUnique();
            modelBuilder.Entity<Author>().HasData(new Author("admin", "Максим", "Мальцев", "test_maltsev@mail.ru"));

            SetRequiredForAllPropertiesOfAllEntities(modelBuilder, typeof(Author), typeof(Article), typeof(FantasyPlayer));
        }
        private void SetRequiredForAllPropertiesOfAllEntities(ModelBuilder modelBuilder,params Type[] entityTypes)
        {
            ///var entitiesList = Assembly.GetExecutingAssembly().GetTypes().Where(x => entityTypes.Contains(x)).ToList(); можно было брать тип сущности прямо из входного массива params, просто хотел "поиграть" со сборкой.
            //foreach(var entityType in entitiesList)
            //{
            //    modelBuilder.Entity(entityType).GetType().GetProperties().ToList().ForEach(prop => modelBuilder.Entity(entityType).Property(prop.Name).IsRequired());
            //}
            foreach(var entity in modelBuilder.Model.GetEntityTypes().ToList())
            {
                modelBuilder.Entity(entity.GetType()).GetType().GetProperties().ToList().ForEach(prop => modelBuilder.Entity(entity.GetType()).Property(prop.Name).IsRequired());
            }
        }
    }
}

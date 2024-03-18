using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NFLFantasySystem.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace NFLFantasySystem.Models
{
    public class Article : IDateCounter
    {
        public int Id { get; set; }
        [Column("Тип статьи")]
        public string Type { get; set; }
        [Column("Заголовок")]
        public string Title { get; set; }
        [Column("Описание")]
        public string Description { get; set; }
        [Column("Автор")]
        public string Author { get; set; }
        [Column("Содержание")]
        public string Content { get; set; }
        public DateOnly RegistrationDate { get; private set; }

        public Article(string type, string title, string description, string author, string content) 
        {
            Type = type;
            Title = title;
            Description = description;
            Author = author;
            Content = content;
            RegistrationDate = DateOnly.FromDateTime(DateTime.Now);
        }

        public void ChangeData(Article article) //дублирование логики поведения с Person.ChangeData(), создать интерфейс????
        {
            Id = article.Id;
            Type = article.Type;
            Title = article.Title;
            Description = article.Description;
            Author = article.Author;
            Content = article.Content;
        }
    }
}

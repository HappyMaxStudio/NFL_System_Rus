using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using NFLFantasySystem.Controllers;
using NFLFantasySystem.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataBaseContext>();
builder.Configuration.AddJsonFile("config.json");

var app = builder.Build();

app.UseStatusCodePages(HandleHTTPException);
app.UseExceptionHandler(app => app.Run(HandleDeveloperException));
app.UseCors(builder => builder.WithOrigins("htpp://sleeper.com").AllowAnyHeader().AllowAnyMethod());

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"staticfiles/html")),
    RequestPath = new PathString("/")
}) ;
//??????????????????????????
// возможно для всех адресов сделать по одному методу, внутри которого проводить проверку на тип запроса.. Во избежание такого дублирования кода

//Возврат всех экземпляров конкретной сущности из БД
app.MapGet("/access/authors", async (DataBaseContext db) => await db.Authors.ToListAsync());
app.MapGet("/access/articles", async (DataBaseContext db) => await db.Articles.ToListAsync());
app.MapGet("/access/fantasyPlayers", async (DataBaseContext db) => await db.FantasyPlayers.ToListAsync());
app.MapGet("/access/seasonWinners", (IConfiguration config) => Results.Json(config.AsEnumerable().ToDictionary()));

//Возврат конкретного элемента
app.MapGet("/search/authors/{id: int}", async (DataBaseContext db, HttpContext context, int id) =>
{
    LogInformation(context, LogLevel.Information);
    var author = await db.Authors.FirstOrDefaultAsync(author => author.Id == id);
    return author == null ? Results.Json(new { message = $"Пользователь с идентификатором {id} не найден" }) : Results.Json(author);
});
app.MapGet("/search/articles/{id: int}", async (DataBaseContext db, HttpContext context, int id) =>
{
    LogInformation(context, LogLevel.Information);
    var article = await db.Articles.FirstOrDefaultAsync(article => article.Id == id);
    return article == null ? Results.Json(new { message = $"Статья с идентификатором {id} не найдена" }) : Results.Json(article);
});
app.MapGet("/search/fantasyPlayers/{id: int}", async (DataBaseContext db, HttpContext context, int id) =>
{
    LogInformation(context, LogLevel.Information);
    var fantasyPlayer = await db.FantasyPlayers.FirstOrDefaultAsync(fPlayer => fPlayer.Id == id);
    return fantasyPlayer == null ? Results.Json(new { message = $"Игрок с идентификатором {id} не найден" }) : Results.Json(fantasyPlayer);
});

//Добавление
app.MapPost("/admin/articles/", async (DataBaseContext db, HttpContext context, Article articleData) =>
{
    LogInformation(context, LogLevel.Information);
    await db.Articles.AddAsync(articleData);
    await db.SaveChangesAsync();
    Results.Json(new { messege = $"Статья успешно добавлена" });
});
app.MapPost("/admin/authors/", async (DataBaseContext db, HttpContext context, Author authorData) =>
{
    LogInformation(context, LogLevel.Information);
    await db.Authors.AddAsync(authorData);
    await db.SaveChangesAsync();
    Results.Json(new { messege = $"Автор успешно добавлен" });
});
app.MapPost("/admin/fantasyPlayer", async (DataBaseContext db, HttpContext context, FantasyPlayer playerData) =>
{
    LogInformation(context, LogLevel.Information);
    await db.FantasyPlayers.AddAsync(playerData);
    await db.SaveChangesAsync();
    Results.Json(new { messege = $"Игрок успешно добавлен" });
});

//Изменение
app.MapPut("/admin/articles/", async (DataBaseContext db, HttpContext context, Article articleData) =>
{
    LogInformation(context, LogLevel.Information);
    var article = await db.Articles.FirstOrDefaultAsync(article => article.Id == articleData.Id);
    if(article == null)
    {
        Results.Json(new {message = "Статья с данным Id не найдена" });    
    }
    else
    {
        article.ChangeData(articleData);
        await db.SaveChangesAsync();
        Results.Json(new { message = $"Статья с {articleData.Id} успешно изменена" });
    }
});
app.MapPut("/admin/authors/", async (DataBaseContext db, HttpContext context, Person authorData) =>
{
    LogInformation(context, LogLevel.Information);
    var author = await db.Authors.FirstOrDefaultAsync(author => author.Id == authorData.Id);
    if (author == null)
    {
        Results.Json(new { message = "Автор с данным Id не найден" });
    }
    else
    {
        author.ChangeData(authorData);
        await db.SaveChangesAsync();
        Results.Json(new { message = $"Данные автора с {authorData.Id} успешно изменены" });
    }
});
app.MapPut("/admin/fantasyPlayers/", async (DataBaseContext db, HttpContext context, FantasyPlayer playerData) =>
{
    LogInformation(context, LogLevel.Information);
    var player = await db.FantasyPlayers.FirstOrDefaultAsync(player => player.Id == playerData.Id);
    if (player == null)
    {
        Results.Json(new { message = $"Игрок с данным Id не найден" });
    }
    else
    {
        player.ChangeData(playerData);
        await db.SaveChangesAsync();
        Results.Json(new { message = $"Игрок с {playerData.Id} успешно изменён" });
    }
});

app.Map("/admin/", async (IConfiguration config, HttpContext context) =>
{
    var form = context.Request.Form;
    if (form["login"] == config["adminLogin"] && form["password"] == config["adminPassword"])
    {
        await context.Response.SendFileAsync("/staticfiles/html/admin.html");
    }
    else
    {
        Results.Content("Введены неверные данные!");
    }
});


//Удаление
app.MapDelete("/admin/articles/", async (DataBaseContext db, HttpContext context, Article articleData) =>
{
    LogInformation(context, LogLevel.Information);
    var article = await db.Articles.FirstOrDefaultAsync(article => article.Id == articleData.Id);
    if (article == null)
    {
        Results.Json(new { message = "Статья с данным Id не найдена" });
    }
    else
    {
        db.Articles.Remove(article);
        await db.SaveChangesAsync();
        Results.Json(new { message = $"Статья с {articleData.Id} успешно удалена" });
    }
});
app.MapDelete("/admin/authors/", async (DataBaseContext db, HttpContext context, Author authorData) =>
{
    LogInformation(context, LogLevel.Information);
    var author = await db.Authors.FirstOrDefaultAsync(author => author.Id == authorData.Id);
    if (author == null)
    {
        Results.Json(new { message = "Автор с данным Id не найден" });
    }
    else
    {
        db.Authors.Remove(author);
        await db.SaveChangesAsync();
        Results.Json(new { message = $"Автор с {authorData.Id} успешно удалён" });
    }
});
app.MapDelete("/admin/fantasyPlayers/", async (DataBaseContext db, HttpContext context, FantasyPlayer playerData) =>
{
    LogInformation(context, LogLevel.Information);
    var player = await db.FantasyPlayers.FirstOrDefaultAsync(player => player.Id == playerData.Id);
    if (player == null)
    {
        Results.Json(new { message = $"Игрок с данным Id не найден" });
    }
    else
    {
        db.FantasyPlayers.Remove(player);
        await db.SaveChangesAsync();
        Results.Json(new { message = $"Игрок с {playerData.Id} успешно удалён" });
    }
});

app.Run();

async Task HandleHTTPException(StatusCodeContext statusCodeContext)
{
    HttpResponse response = statusCodeContext.HttpContext.Response;
    string path = statusCodeContext.HttpContext.Request.Path;
    switch (response.StatusCode)
    {
        case 403: await response.WriteAsync($"Access to the following path: {path} is denied");
            break;
        case 404: await response.WriteAsync($"The following path: {path} is not found");
            break;  
    }
}
async Task HandleDeveloperException(HttpContext context)
{
    context.Response.StatusCode = 500;
    await context.Response.WriteAsync("A developer exception has occured. Unable to load the page!");
}
void LogInformation(HttpContext context, LogLevel logLevel)
{
    if(logLevel == LogLevel.Information)
    {
        app.Logger.LogInformation($"Обработка запроса типа {context.Request.Method} по адресу {context.Request.Path}");
    }
    else
    {
        app.Logger.LogWarning("Запрошенный объект не найден!!!");
    }
}
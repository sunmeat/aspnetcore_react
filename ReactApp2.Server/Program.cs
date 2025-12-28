namespace ReactApp2.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddOpenApi(); // https://aka.ms/aspnet/openapi
            // OpenAPI - це стандарт для опису RESTful API, який дозволяє розробникам легко зрозуміти та взаємодіяти з веб-сервісами

            builder.Services.AddHttpClient();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.MapStaticAssets();

            if (app.Environment.IsDevelopment())
            { 
                app.MapOpenApi(); // тут додається OpenAPI middleware, це дозволяє автоматично генерувати документацію для API на основі OpenAPI специфікації
            }

            app.UseHttpsRedirection(); // перенаправляє всі HTTP-запити на HTTPS для забезпечення безпеки

            app.UseAuthorization(); // додає middleware для обробки авторизації, перевіряючи, чи має користувач необхідні права доступу до ресурсів, хоча аутентифікація поки і не налаштована

            app.MapControllers(); // налаштовує маршрутизацію для контролерів, дозволяючи їм обробляти вхідні HTTP-запити відповідно до визначених маршрутів

            app.MapFallbackToFile("/index.html"); // налаштовує fallback маршрут, який перенаправляє всі невизначені запити до файлу index.html

            app.Run();
        }
    }
}

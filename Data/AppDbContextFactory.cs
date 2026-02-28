using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MontageAPI.Data;

/// <summary>
/// Фабрика для EF Core Tools (design-time).
/// Лежит в проекте Data, рядом с AppDbContext.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // === ХАРДКОД СТРОКИ ПОДКЛЮЧЕНИЯ ===
        // Для локальной разработки
        var connectionString = "Server=localhost;Port=3306;Database=MontageDb;User=webtracker;Password=1234;";

        // === НАСТРОЙКА КОНТЕКСТА ===
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Версия MySQL сервера (укажите свою, если отличается)
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 32));

        optionsBuilder.UseMySql(connectionString, serverVersion);

        return new AppDbContext(optionsBuilder.Options);
    }
}
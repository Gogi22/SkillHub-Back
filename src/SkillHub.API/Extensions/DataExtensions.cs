namespace SkillHub.API.Extensions;

public static class DataExtensions
{
    public static WebApplication
        ApplyMigrations<T>(this WebApplication app, bool inMemory) where T : DbContext
    {
        if (inMemory)
        {
            return app;
        }

        using var scope = app.Services.CreateScope();
        var connection = scope.ServiceProvider.GetRequiredService<T>();
        connection.Database.Migrate();
      
        return app;
    }
}
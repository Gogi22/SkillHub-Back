using Microsoft.EntityFrameworkCore;

namespace Identity.API.Extensions;

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

        var secondsPassed = 0;
        var retryDelay = TimeSpan.FromSeconds(10);
        const int maxSeconds = 60;

        while (true)
        {
            try
            {
                var connection = scope.ServiceProvider.GetService<T>();
                connection?.Database.Migrate();
                break;
            }
            catch (Exception)
            {
                if (secondsPassed > maxSeconds)
                {
                    throw;
                }

                retryDelay += TimeSpan.FromSeconds(10);
                Thread.Sleep(retryDelay);
                secondsPassed += retryDelay.Seconds;
            }
        }

        return app;
    }
}
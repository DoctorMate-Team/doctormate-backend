
using doctor.Repository.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace doctor.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            #region Connect DataBase

            builder.Services.AddDbContext<DoctorMateDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            #endregion
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            #region Auto Database Migration
            // تطبيق الـ migrations تلقائياً عند تشغيل الـ application
            await ApplyDatabaseMigrations(app);
            #endregion

            #region Kestral Pipliens
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            #endregion
            app.Run();
        }

        /// <summary>
        /// تطبيق الـ Database Migrations تلقائياً
        /// </summary>
        private static async Task ApplyDatabaseMigrations(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            try
            {
                var context = scope.ServiceProvider.GetRequiredService<DoctorMateDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("🔄 Checking for pending database migrations...");

                // تحقق من وجود migrations معلقة
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                
                if (pendingMigrations.Any())
                {
                    logger.LogInformation($"📊 Found {pendingMigrations.Count()} pending migrations. Applying...");
                    
                    // تطبيق الـ migrations
                    await context.Database.MigrateAsync();
                    
                    logger.LogInformation("✅ Database migrations applied successfully!");
                }
                else
                {
                    logger.LogInformation("✅ Database is up to date. No pending migrations.");
                }
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "❌ Error applying database migrations: {ErrorMessage}", ex.Message);
                
                // في حالة الخطأ، يمكنك اختيار إما:
                // 1. إيقاف الـ application (uncomment التالي)
                // throw;
                
                // 2. أو الاستمرار مع تحذير (الحالي)
                logger.LogWarning("⚠️ Application will continue without applying migrations.");
            }
        }
    }
}

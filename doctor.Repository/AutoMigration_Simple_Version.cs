// نسخة مبسطة - لو عايز النسخة البسيطة بدل المعقدة
/*
            #region Auto Database Migration - Simple Version
            // تطبيق الـ migrations تلقائياً عند تشغيل الـ application
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DoctorMateDbContext>();
                context.Database.Migrate();
                Console.WriteLine("✅ Database updated successfully!");
            }
            #endregion
*/
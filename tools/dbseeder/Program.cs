using doctor.Repository.Data.Contexts;
using doctor.Repository.Data.Data_Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        // copy connection from repository's appsettings via environment if needed
        services.AddDbContext<DoctorMateDbContext>(options =>
        {
            var conn = ctx.Configuration["ConnectionStrings:DefaultConnection"] ?? ctx.Configuration["ConnectionStrings:DoctorMate"];
            if (string.IsNullOrWhiteSpace(conn)) throw new InvalidOperationException("Connection string not found in configuration.");
            options.UseSqlServer(conn);
        });
    })
    .Build();

var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<DoctorMateDbContext>();

try
{
    Console.WriteLine("Running seeder against: " + db.Database.GetConnectionString());

    // report counts before
    Console.WriteLine("Counts before seeding:");
    Console.WriteLine($"Users: {await db.Users.CountAsync()}");
    Console.WriteLine($"Doctors: {await db.Doctors.CountAsync()}");
    Console.WriteLine($"Patients: {await db.Patients.CountAsync()}");
    Console.WriteLine($"Appointments: {await db.Appointments.CountAsync()}");

    Console.WriteLine("Starting seeder...");
    await doctor.Repository.Data.Data_Seed.DoctorMateDbContextSeed.SeedAsync(db);

    Console.WriteLine("Counts after seeding:");
    Console.WriteLine($"Users: {await db.Users.CountAsync()}");
    Console.WriteLine($"Doctors: {await db.Doctors.CountAsync()}");
    Console.WriteLine($"Patients: {await db.Patients.CountAsync()}");
    Console.WriteLine($"Appointments: {await db.Appointments.CountAsync()}");
    Console.WriteLine("Seeding complete.");
}
catch (Exception ex)
{
    Console.WriteLine("Seeder failed: " + ex);
}

return 0;
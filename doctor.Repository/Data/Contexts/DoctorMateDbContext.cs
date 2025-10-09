using doctor.Core.Entities;
using doctor.Repository.Data.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doctor.Repository.Data.Contexts
{
    public class DoctorMateDbContext : DbContext
    {
        #region DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Diagnosis> Diagnosis { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<IntegrationLog> IntegrationLogs { get; set; }
        #endregion

        #region Constructor
        public DoctorMateDbContext(DbContextOptions<DoctorMateDbContext> options) : base(options)
        {
        }
        #endregion

        #region Model Configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تطبيق جميع الـ Configurations الموجودة في نفس الـ Assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DoctorMateDbContext).Assembly);
        }
        #endregion
    }
}

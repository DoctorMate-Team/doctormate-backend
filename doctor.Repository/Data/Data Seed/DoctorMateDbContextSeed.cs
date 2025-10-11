using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using doctor.Core.Entities;
using doctor.Core.Entities.Identity;
using doctor.Repository.Data.Contexts;
using Microsoft.AspNetCore.Identity;

namespace doctor.Repository.Data.Data_Seed
{
    public static class DoctorMateDbContextSeed
    {
        public static async Task SeedAsync(DoctorMateDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var seedDir = FindSeedDataDirectory();
            Console.WriteLine("Using seed directory: " + seedDir);

            // Users
            try
            {
                var usersPath = Path.Combine(seedDir, "users.json");
                if (File.Exists(usersPath) && !context.Users.Any())
                {
                    var usersJson = await File.ReadAllTextAsync(usersPath);
                    var users = JsonSerializer.Deserialize<List<UserSeed>>(usersJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (users != null)
                    {
                        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
                        foreach (var u in users)
                        {
                            var userId = TryParseGuidOrNew(u.Id);
                            var user = new User
                            {
                                Id = userId,
                                UserName = u.Phone ?? u.Id ?? userId.ToString(),
                                Email = u.Phone ?? $"user{userId}@example.com",
                                EmailConfirmed = true,
                                PasswordHash = hasher.HashPassword(null!, "TempPassword123!"),
                                FullName = u.FullName,
                                Phone = u.Phone,
                                Role = u.Role ?? "patient",
                                IsActive = u.IsActive,
                                CreatedAt = u.CreatedAt ?? DateTime.UtcNow
                            };
                            context.Users.Add(user);
                        }
                        await context.SaveChangesAsync();
                        Console.WriteLine($"Seeded {users.Count} users.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Users seeding failed: " + ex.Message);
            }

            // Doctors
            try
            {
                var doctorsPath = Path.Combine(seedDir, "doctors.json");
                if (File.Exists(doctorsPath) && !context.Doctors.Any())
                {
                    var doctorsJson = await File.ReadAllTextAsync(doctorsPath);
                    var doctors = JsonSerializer.Deserialize<List<DoctorSeed>>(doctorsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var added = 0;
                    if (doctors != null)
                    {
                        foreach (var d in doctors)
                        {
                            // parse required GUIDs
                            if (!TryParseGuidOrWarn(d.UserId, "Doctor.UserId", out var userGuid))
                                continue; // skip if Doctor references invalid user

                            var doc = new Doctor
                            {
                                Id = TryParseGuidOrNew(d.Id),
                                UserId = userGuid,
                                Specialty = d.Specialty,
                                Qualifications = d.Qualifications,
                                LicenseNumber = d.LicenseNumber,
                                ConsultationFee = d.ConsultationFee ?? 0m,
                                OpenmrsProviderUuid = d.OpenmrsProviderUuid,
                                CreatedAt = d.CreatedAt ?? DateTime.UtcNow
                            };
                            context.Doctors.Add(doc);
                            added++;
                        }
                        if (added > 0) await context.SaveChangesAsync();
                        Console.WriteLine($"Seeded {added} doctors.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Doctors seeding failed: " + ex.Message);
            }

            // Patients
            try
            {
                var patientsPath = Path.Combine(seedDir, "patients.json");
                if (File.Exists(patientsPath) && !context.Patients.Any())
                {
                    var patientsJson = await File.ReadAllTextAsync(patientsPath);
                    var patients = JsonSerializer.Deserialize<List<PatientSeed>>(patientsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var added = 0;
                    if (patients != null)
                    {
                        foreach (var p in patients)
                        {
                            if (!TryParseGuidOrWarn(p.UserId, "Patient.UserId", out var userGuid))
                                continue;

                            var pat = new Patient
                            {
                                Id = TryParseGuidOrNew(p.Id),
                                UserId = userGuid,
                                BirthDate = p.BirthDate,
                                Gender = p.Gender,
                                Address = p.Address,
                                BloodType = p.BloodType,
                                EmergencyContact = p.EmergencyContact,
                                OpenmrsPatientUuid = p.OpenmrsPatientUuid,
                                CreatedAt = p.CreatedAt ?? DateTime.UtcNow
                            };
                            context.Patients.Add(pat);
                            added++;
                        }
                        if (added > 0) await context.SaveChangesAsync();
                        Console.WriteLine($"Seeded {added} patients.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Patients seeding failed: " + ex.Message);
            }

            // Appointments
            try
            {
                var appointmentsPath = Path.Combine(seedDir, "appointments.json");
                if (File.Exists(appointmentsPath) && !context.Appointments.Any())
                {
                    var appointmentsJson = await File.ReadAllTextAsync(appointmentsPath);
                    var appointments = JsonSerializer.Deserialize<List<AppointmentSeed>>(appointmentsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var added = 0;
                    if (appointments != null)
                    {
                        foreach (var a in appointments)
                        {
                            // required references: PatientId, DoctorId
                            if (!TryParseGuidOrWarn(a.PatientId, "Appointment.PatientId", out var patientGuid))
                                continue;
                            if (!TryParseGuidOrWarn(a.DoctorId, "Appointment.DoctorId", out var doctorGuid))
                                continue;

                            var ap = new Appointment
                            {
                                Id = TryParseGuidOrNew(a.Id),
                                PatientId = patientGuid,
                                DoctorId = doctorGuid,
                                ScheduledStart = a.ScheduledStart ?? DateTime.UtcNow,
                                ScheduledEnd = a.ScheduledEnd,
                                Status = a.Status ?? "pending",
                                Reason = a.Reason,
                                AppointmentType = a.AppointmentType,
                                CanceledAt = a.CanceledAt,
                                CanceledBy = TryParseNullableGuid(a.CanceledBy),
                                OpenmrsAppointmentUuid = a.OpenmrsAppointmentUuid,
                                CreatedAt = a.CreatedAt ?? DateTime.UtcNow
                            };
                            context.Appointments.Add(ap);
                            added++;
                        }
                        if (added > 0) await context.SaveChangesAsync();
                        Console.WriteLine($"Seeded {added} appointments.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Appointments seeding failed: " + ex.Message);
            }

            // MedicalRecords, Diagnoses, Prescriptions, Payments, Notifications, ChatMessages, AuditLogs, IntegrationLogs
            // For brevity seed only files that exist and map obvious GUIDs; follow same TryParse pattern.

            await SeedAdditionalAsync(context, seedDir);
        }

        private static async Task SeedAdditionalAsync(DoctorMateDbContext context, string seedDir)
        {
            // MedicalRecords
            try
            {
                var path = Path.Combine(seedDir, "medicalRecords.json");
                if (File.Exists(path) && !context.MedicalRecords.Any())
                {
                    var json = await File.ReadAllTextAsync(path);
                    var items = JsonSerializer.Deserialize<List<MedicalRecordSeed>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var added = 0;
                    if (items != null)
                    {
                        foreach (var it in items)
                        {
                            if (!TryParseGuidOrWarn(it.PatientId, "MedicalRecord.PatientId", out var patGuid))
                                continue;

                            var mr = new MedicalRecord
                            {
                                Id = TryParseGuidOrNew(it.Id),
                                PatientId = patGuid,
                                Title = it.Title,
                                Description = it.Description,
                                RecordType = it.RecordType,
                                Status = it.Status,
                                RecordedBy = TryParseNullableGuid(it.RecordedBy),
                                RecordedAt = it.RecordedAt ?? DateTime.UtcNow,
                                OpenmrsEncounterUuid = it.OpenmrsEncounterUuid,
                                CreatedAt = it.CreatedAt ?? DateTime.UtcNow
                            };
                            context.MedicalRecords.Add(mr);
                            added++;
                        }
                        if (added > 0) await context.SaveChangesAsync();
                        Console.WriteLine($"Seeded {added} medical records.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MedicalRecords seeding failed: " + ex.Message);
            }

            // Diagnoses
            try
            {
                var path = Path.Combine(seedDir, "diagnoses.json");
                if (File.Exists(path) && !context.Diagnosis.Any())
                {
                    var json = await File.ReadAllTextAsync(path);
                    var items = JsonSerializer.Deserialize<List<DiagnosisSeed>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var added = 0;
                    if (items != null)
                    {
                        foreach (var it in items)
                        {
                            if (!TryParseGuidOrWarn(it.MedicalRecordId, "Diagnosis.MedicalRecordId", out var mrGuid))
                                continue;
                            if (!TryParseGuidOrWarn(it.DiagnosedBy, "Diagnosis.DiagnosedBy", out var userGuid))
                                continue;

                            var d = new Diagnosis
                            {
                                Id = TryParseGuidOrNew(it.Id),
                                MedicalRecordId = mrGuid,
                                AppointmentId = TryParseNullableGuid(it.AppointmentId),
                                DiagnosedBy = userGuid,
                                DiagnosisText = it.DiagnosisText,
                                IcdCode = it.IcdCode,
                                Severity = it.Severity,
                                OpenmrsObsUuid = it.OpenmrsObsUuid,
                                CreatedAt = it.CreatedAt ?? DateTime.UtcNow
                            };
                            context.Diagnosis.Add(d);
                            added++;
                        }
                        if (added > 0) await context.SaveChangesAsync();
                        Console.WriteLine($"Seeded {added} diagnoses.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Diagnoses seeding failed: " + ex.Message);
            }

            // Prescriptions
            try
            {
                var path = Path.Combine(seedDir, "prescriptions.json");
                if (File.Exists(path) && !context.Prescriptions.Any())
                {
                    var json = await File.ReadAllTextAsync(path);
                    var items = JsonSerializer.Deserialize<List<PrescriptionSeed>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var added = 0;
                    if (items != null)
                    {
                        foreach (var it in items)
                        {
                            if (!TryParseGuidOrWarn(it.DiagnosisId, "Prescription.DiagnosisId", out var diagGuid))
                                continue;
                            if (!TryParseGuidOrWarn(it.PatientId, "Prescription.PatientId", out var patGuid))
                                continue;
                            if (!TryParseGuidOrWarn(it.DoctorId, "Prescription.DoctorId", out var docGuid))
                                continue;

                            var p = new Prescription
                            {
                                Id = TryParseGuidOrNew(it.Id),
                                DiagnosisId = diagGuid,
                                PatientId = patGuid,
                                DoctorId = docGuid,
                                DrugName = it.DrugName ?? "",
                                Dosage = it.Dosage,
                                Instructions = it.Instructions,
                                DurationDays = it.DurationDays,
                                OpenmrsOrderUuid = it.OpenmrsOrderUuid,
                                CreatedAt = it.CreatedAt ?? DateTime.UtcNow
                            };
                            context.Prescriptions.Add(p);
                            added++;
                        }
                        if (added > 0) await context.SaveChangesAsync();
                        Console.WriteLine($"Seeded {added} prescriptions.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Prescriptions seeding failed: " + ex.Message);
            }

            // Payments
            try
            {
                var path = Path.Combine(seedDir, "payments.json");
                if (File.Exists(path) && !context.Payments.Any())
                {
                    var json = await File.ReadAllTextAsync(path);
                    var items = JsonSerializer.Deserialize<List<PaymentSeed>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var added = 0;
                    if (items != null)
                    {
                        foreach (var it in items)
                        {
                            if (!TryParseGuidOrWarn(it.AppointmentId, "Payment.AppointmentId", out var apGuid))
                                continue;
                            if (!TryParseGuidOrWarn(it.PatientId, "Payment.PatientId", out var patGuid))
                                continue;

                            var p = new Payment
                            {
                                Id = TryParseGuidOrNew(it.Id),
                                AppointmentId = apGuid,
                                PatientId = patGuid,
                                Amount = it.Amount,
                                Currency = it.Currency ?? "USD",
                                Method = it.Method ?? "unknown",
                                Status = it.Status ?? "pending",
                                TransactionRef = it.TransactionRef,
                                PaidAt = it.PaidAt,
                                CreatedAt = it.CreatedAt ?? DateTime.UtcNow
                            };
                            context.Payments.Add(p);
                            added++;
                        }
                        if (added > 0) await context.SaveChangesAsync();
                        Console.WriteLine($"Seeded {added} payments.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Payments seeding failed: " + ex.Message);
            }

            // Notifications
            try
            {
                var path = Path.Combine(seedDir, "notifications.json");
                if (File.Exists(path) && !context.Notifications.Any())
                {
                    var json = await File.ReadAllTextAsync(path);
                    var items = JsonSerializer.Deserialize<List<NotificationSeed>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var added = 0;
                    if (items != null)
                    {
                        foreach (var it in items)
                        {
                            if (!TryParseGuidOrWarn(it.UserId, "Notification.UserId", out var userGuid))
                                continue;

                            var n = new Notification
                            {
                                Id = TryParseGuidOrNew(it.Id),
                                UserId = userGuid,
                                Message = it.Message ?? string.Empty,
                                Type = it.Type,
                                IsRead = it.IsRead,
                                CreatedAt = it.CreatedAt ?? DateTime.UtcNow
                            };
                            context.Notifications.Add(n);
                            added++;
                        }
                        if (added > 0) await context.SaveChangesAsync();
                        Console.WriteLine($"Seeded {added} notifications.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Notifications seeding failed: " + ex.Message);
            }

            // ChatMessages, AuditLogs, IntegrationLogs can follow same pattern if present
            try
            {
                var chatPath = Path.Combine(seedDir, "chatMessages.json");
                if (File.Exists(chatPath) && !context.ChatMessages.Any())
                {
                    var json = await File.ReadAllTextAsync(chatPath);
                    var items = JsonSerializer.Deserialize<List<ChatMessageSeed>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var added = 0;
                    if (items != null)
                    {
                        foreach (var it in items)
                        {
                            if (!TryParseGuidOrWarn(it.SenderId, "ChatMessage.SenderId", out var senderGuid))
                                continue;
                            if (!TryParseGuidOrWarn(it.ReceiverId, "ChatMessage.ReceiverId", out var receiverGuid))
                                continue;

                            var cm = new ChatMessage
                            {
                                Id = TryParseGuidOrNew(it.Id),
                                SenderId = senderGuid,
                                ReceiverId = receiverGuid,
                                Message = it.Message ?? string.Empty,
                                SentAt = it.CreatedAt ?? DateTime.UtcNow
                            };
                            context.ChatMessages.Add(cm);
                            added++;
                        }
                        if (added > 0) await context.SaveChangesAsync();
                        Console.WriteLine($"Seeded {added} chat messages.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ChatMessages seeding failed: " + ex.Message);
            }

            // AuditLogs
            try
            {
                var path = Path.Combine(seedDir, "auditLogs.json");
                if (File.Exists(path) && !context.AuditLogs.Any())
                {
                    var json = await File.ReadAllTextAsync(path);
                    var items = JsonSerializer.Deserialize<List<AuditLogSeed>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var added = 0;
                    if (items != null)
                    {
                        foreach (var it in items)
                        {
                            if (!TryParseGuidOrWarn(it.UserId, "AuditLog.UserId", out var auditUser))
                                continue;

                            var al = new AuditLog
                            {
                                Id = TryParseGuidOrNew(it.Id),
                                Action = it.Action ?? string.Empty,
                                Entity = it.TableName ?? string.Empty,
                                EntityId = TryParseNullableGuid(it.PrimaryKey),
                                Status = it.NewValues ?? string.Empty,
                                LogTime = it.CreatedAt ?? DateTime.UtcNow,
                                Response = it.OldValues,
                                UserId = auditUser
                            };
                            context.AuditLogs.Add(al);
                            added++;
                        }
                        if (added > 0) await context.SaveChangesAsync();
                        Console.WriteLine($"Seeded {added} audit logs.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AuditLogs seeding failed: " + ex.Message);
            }

            try
            {
                var path = Path.Combine(seedDir, "integrationLogs.json");
                if (File.Exists(path) && !context.IntegrationLogs.Any())
                {
                    var json = await File.ReadAllTextAsync(path);
                    var items = JsonSerializer.Deserialize<List<IntegrationLogSeed>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var added = 0;
                    if (items != null)
                    {
                        foreach (var it in items)
                        {
                            var il = new IntegrationLog
                            {
                                Id = TryParseGuidOrNew(it.Id),
                                Endpoint = it.Event ?? string.Empty,
                                Method = "POST",
                                EntityType = null,
                                EntityId = null,
                                RequestPayload = it.Payload,
                                ResponsePayload = null,
                                StatusCode = 200,
                                Success = true,
                                CreatedAt = it.CreatedAt ?? DateTime.UtcNow
                            };
                            context.IntegrationLogs.Add(il);
                            added++;
                        }
                        if (added > 0) await context.SaveChangesAsync();
                        Console.WriteLine($"Seeded {added} integration logs.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("IntegrationLogs seeding failed: " + ex.Message);
            }
        }

        #region Helpers
        private static Guid TryParseGuidOrNew(string? value)
        {
            if (Guid.TryParse(value, out var g)) return g;
            return Guid.NewGuid();
        }

        private static Guid? TryParseNullableGuid(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (Guid.TryParse(value, out var g)) return g;
            Console.WriteLine($"Warning: could not parse GUID value '{value}' to nullable Guid.");
            return null;
        }

        private static bool TryParseGuidOrWarn(string? value, string fieldName, out Guid guid)
        {
            guid = Guid.Empty;
            if (string.IsNullOrWhiteSpace(value))
            {
                Console.WriteLine($"Warning: missing required GUID for {fieldName}. Skipping record.");
                return false;
            }
            if (!Guid.TryParse(value, out guid))
            {
                Console.WriteLine($"Warning: invalid GUID value '{value}' for {fieldName}. Skipping record.");
                return false;
            }
            return true;
        }

        private static string FindSeedDataDirectory()
        {
            // Search upwards from base directory and current directory for the data seed folder
            var start = AppContext.BaseDirectory;
            var dir = new DirectoryInfo(start);
            for (int i = 0; i < 8 && dir != null; i++)
            {
                var candidate = Path.Combine(dir.FullName, "doctor.Repository", "Data", "Data Seed", "Data");
                if (Directory.Exists(candidate)) return candidate;
                var alt = Path.Combine(dir.FullName, "Data Seed", "Data");
                if (Directory.Exists(alt)) return alt;
                dir = dir.Parent;
            }
            // fallback to repository relative path from current directory
            var fallback = Path.Combine(Directory.GetCurrentDirectory(), "doctor.Repository", "Data", "Data Seed", "Data");
            return Directory.Exists(fallback) ? fallback : Path.Combine(Directory.GetCurrentDirectory(), "Data Seed", "Data");
        }
        #endregion

        #region DTOs
        private class UserSeed { public string? Id { get; set; } public string? FullName { get; set; } public string? Phone { get; set; } public string? Role { get; set; } public bool IsActive { get; set; } = true; public DateTime? CreatedAt { get; set; } }
        private class DoctorSeed { public string? Id { get; set; } public string? UserId { get; set; } public string? Specialty { get; set; } public string? Qualifications { get; set; } public string? LicenseNumber { get; set; } public decimal? ConsultationFee { get; set; } public string? OpenmrsProviderUuid { get; set; } public DateTime? CreatedAt { get; set; } }
        private class PatientSeed { public string? Id { get; set; } public string? UserId { get; set; } public DateTime? BirthDate { get; set; } public string? Gender { get; set; } public string? Address { get; set; } public string? BloodType { get; set; } public string? EmergencyContact { get; set; } public string? OpenmrsPatientUuid { get; set; } public DateTime? CreatedAt { get; set; } }
        private class AppointmentSeed { public string? Id { get; set; } public string? PatientId { get; set; } public string? DoctorId { get; set; } public DateTime? ScheduledStart { get; set; } public DateTime? ScheduledEnd { get; set; } public string? Status { get; set; } public string? Reason { get; set; } public string? AppointmentType { get; set; } public DateTime? CanceledAt { get; set; } public string? CanceledBy { get; set; } public string? OpenmrsAppointmentUuid { get; set; } public DateTime? CreatedAt { get; set; } }
        private class MedicalRecordSeed { public string? Id { get; set; } public string? PatientId { get; set; } public string? Title { get; set; } public string? Description { get; set; } public string? RecordType { get; set; } public string? Status { get; set; } public string? RecordedBy { get; set; } public DateTime? RecordedAt { get; set; } public string? OpenmrsEncounterUuid { get; set; } public DateTime? CreatedAt { get; set; } }
        private class DiagnosisSeed { public string? Id { get; set; } public string? MedicalRecordId { get; set; } public string? AppointmentId { get; set; } public string? DiagnosedBy { get; set; } public string? DiagnosisText { get; set; } public string? IcdCode { get; set; } public string? Severity { get; set; } public string? OpenmrsObsUuid { get; set; } public DateTime? CreatedAt { get; set; } }
        private class PrescriptionSeed { public string? Id { get; set; } public string? DiagnosisId { get; set; } public string? PatientId { get; set; } public string? DoctorId { get; set; } public string? DrugName { get; set; } public string? Dosage { get; set; } public string? Instructions { get; set; } public int? DurationDays { get; set; } public string? OpenmrsOrderUuid { get; set; } public DateTime? CreatedAt { get; set; } }
        private class PaymentSeed { public string? Id { get; set; } public string? AppointmentId { get; set; } public string? PatientId { get; set; } public decimal Amount { get; set; } public string? Currency { get; set; } public string? Method { get; set; } public string? Status { get; set; } public string? TransactionRef { get; set; } public DateTime? PaidAt { get; set; } public DateTime? CreatedAt { get; set; } }
        private class NotificationSeed { public string? Id { get; set; } public string? UserId { get; set; } public string? Message { get; set; } public string? Type { get; set; } public bool IsRead { get; set; } = false; public DateTime? CreatedAt { get; set; } }
        private class ChatMessageSeed { public string? Id { get; set; } public string? SenderId { get; set; } public string? ReceiverId { get; set; } public string? Message { get; set; } public DateTime? CreatedAt { get; set; } }
        private class AuditLogSeed { public string? Id { get; set; } public string? Action { get; set; } public string? TableName { get; set; } public string? PrimaryKey { get; set; } public string? OldValues { get; set; } public string? NewValues { get; set; } public DateTime? CreatedAt { get; set; } public string? UserId { get; set; } }
        private class IntegrationLogSeed { public string? Id { get; set; } public string? Event { get; set; } public string? Payload { get; set; } public DateTime? CreatedAt { get; set; } }
        #endregion
    }
}

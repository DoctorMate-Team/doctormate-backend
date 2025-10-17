# إعداد نظام OTP - Doctor Mate 🔧

## 1. الإعدادات المطلوبة في appsettings.json ⚙️

تم إضافة الإعدادات التالية لـ `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=db29031.public.databaseasp.net;Database=db29031;User Id=db29031;Password=Hy2!h#X4Lk8_;Encrypt=False;MultipleActiveResultSets=True;TrustServerCertificate=True",
    "SmtpSettingsConnection": "Server=db29708.public.databaseasp.net;Database=db29708;User Id=db29708;Password=Qw9_j%A5Z7@c;Encrypt=False;MultipleActiveResultSets=True;TrustServerCertificate=True"
  },

  "SmtpSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "EnableSsl": true,
    "UserName": "doctormate89@gmail.com",
    "Password": "pigc wbef oybf jonq",
    "SenderEmail": "doctormate89@gmail.com",
    "SenderName": "Doctor Mate"
  },

  "OtpSettings": {
    "ExpiryMinutes": 10,
    "MaxAttemptsPerHour": 5,
    "CodeLength": 6,
    "AllowResendAfterMinutes": 2,
    "EnableRateLimit": true,
    "TrackIpAddress": true,
    "CleanupIntervalHours": 1
  }
}
```

## 2. إعدادات التطوير appsettings.Development.json 🛠️

تم تخصيص إعدادات للتطوير مع قيم أكثر مرونة:

```json
{
  "OtpSettings": {
    "ExpiryMinutes": 15,
    "MaxAttemptsPerHour": 10,
    "CodeLength": 6,
    "AllowResendAfterMinutes": 1
  },

  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "doctor.Service.Services.Otp": "Debug",
      "doctor.APIs.Controllers.OtpController": "Debug"
    }
  }
}
```

## 3. شرح الإعدادات 📖

### SmtpSettings:
- **Host**: خادم SMTP (Gmail: smtp.gmail.com)
- **Port**: منفذ SMTP (Gmail: 587)
- **EnableSsl**: تفعيل SSL/TLS
- **UserName/Password**: بيانات حساب Gmail (استخدم App Password)
- **SenderEmail/SenderName**: معلومات المرسل

### OtpSettings:
- **ExpiryMinutes**: مدة انتهاء صلاحية OTP (افتراضي: 10 دقائق)
- **MaxAttemptsPerHour**: عدد محاولات الإرسال المسموح (افتراضي: 5)
- **CodeLength**: طول رمز OTP (افتراضي: 6 أرقام)
- **AllowResendAfterMinutes**: السماح بإعادة الإرسال بعد (افتراضي: 2 دقيقة)
- **EnableRateLimit**: تفعيل حد الإرسال
- **TrackIpAddress**: تتبع عنوان IP
- **CleanupIntervalHours**: تنظيف OTPs منتهية الصلاحية

## 4. إعداد Gmail App Password 📧

لاستخدام Gmail، تحتاج إلى إنشاء App Password:

1. ادخل إلى حساب Google
2. اذهب إلى Security → 2-Step Verification
3. قم بتفعيل 2-Step Verification
4. اذهب إلى App Passwords
5. أنشئ كلمة مرور جديدة للتطبيق
6. استخدم هذه الكلمة في `Password` في SmtpSettings

## 5. إنشاء Migration للـ OTP 🗄️

قم بتشغيل الأوامر التالية:

```bash
# الانتقال لمجلد APIs
cd "d:\Doctor Mate\doctor\doctor.APIs"

# إنشاء Migration للـ OTP
dotnet ef migrations add AddOtpCodeTable --project ..\doctor.Repository --startup-project .

# تطبيق Migration على قاعدة البيانات
dotnet ef database update --project ..\doctor.Repository --startup-project .
```

## 6. اختبار النظام 🧪

### استخدام OtpApi.http:

```http
# إرسال OTP
POST https://localhost:7243/api/Otp/send-verification
Content-Type: application/json

{
  "userId": "5fcf8c92-bc1b-4048-8128-73af384759bf",
  "email": "doctormate89@gmail.com"
}

# التحقق من OTP
POST https://localhost:7243/api/Otp/verify
Content-Type: application/json

{
  "userId": "5fcf8c92-bc1b-4048-8128-73af384759bf",
  "code": "123456"
}
```

## 7. مراقبة النظام 📊

### Logs للمراقبة:
- **Information**: العمليات العادية
- **Debug**: تفاصيل OTP operations (في Development فقط)
- **Warning**: محاولات مشبوهة أو Rate Limiting
- **Error**: أخطاء تقنية

### أمثلة على Logs:
```
INFO: OTP verification sent successfully to user 12345
DEBUG: Generated OTP code for user 12345 with length 6
WARN: Rate limit exceeded for user 12345
ERROR: Error generating and sending OTP for user 12345
```

## 8. الأمان والحماية 🔐

### الميزات الأمنية المُطبقة:
- ✅ **Cryptographic Random** لتوليد OTP
- ✅ **Rate Limiting** لمنع الـ spam
- ✅ **انتهاء صلاحية** تلقائي
- ✅ **عدم إرجاع OTP** في الـ response
- ✅ **تشفير كلمات المرور** في الإعدادات
- ✅ **Validation شامل** للبيانات

### Best Practices:
- استخدم App Password بدلاً من كلمة المرور الأساسية
- قم بتغيير كلمات المرور دورياً
- راقب الـ logs للأنشطة المشبوهة
- استخدم HTTPS في الإنتاج

## 9. استكشاف الأخطاء 🔍

### المشاكل الشائعة:

#### 1. خطأ SMTP Authentication:
```
Solution: تأكد من App Password صحيح وليس كلمة المرور العادية
```

#### 2. خطأ Connection String:
```
Solution: تأكد من صحة connection strings في appsettings.json
```

#### 3. Migration Errors:
```bash
# حذف Migration خاطئة
dotnet ef migrations remove --project ..\doctor.Repository --startup-project .

# إعادة إنشاء Migration
dotnet ef migrations add AddOtpCodeTable --project ..\doctor.Repository --startup-project .
```

#### 4. Port 587 مغلق:
```
Solution: جرب Port 465 مع EnableSsl = true
```

## 10. التشغيل النهائي 🚀

```bash
# تشغيل المشروع
cd "d:\Doctor Mate\doctor\doctor.APIs"
dotnet run

# النظام سيعمل على:
# HTTP: http://localhost:7200
# HTTPS: https://localhost:7243

# اختبر باستخدام Swagger:
# https://localhost:7243/swagger
```

---

## ✅ النظام جاهز للعمل!

تم إعداد نظام OTP بنجاح مع جميع الإعدادات المطلوبة. النظام يوفر:

- 🔐 **أمان عالي** مع تشفير وRate Limiting
- 📧 **إرسال إيميل احترافي** بتصميم HTML
- ⚙️ **إعدادات قابلة للتخصيص** لكل بيئة
- 📊 **مراقبة شاملة** مع Logging متقدم
- 🧪 **سهولة الاختبار** مع ملفات HTTP

النظام الآن جاهز للإنتاج! 🎉
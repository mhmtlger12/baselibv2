namespace Baselib.Core.Messages;

public static class Messages
{
    public static class General
    {
        public const string NotFound = "Kayıt bulunamadı";
        public const string Saved = "Kayıt başarıyla kaydedildi";
        public const string Updated = "Kayıt başarıyla güncellendi";
        public const string Deleted = "Kayıt başarıyla silindi";
        public const string Error = "Bir hata oluştu";
        public const string Required = "Bu alan zorunludur";
        public const string SessionNotFound = "Oturum bulunamadı.";
        public const string SelfReferenceNotAllowed = "Bir kayıt kendisine üst olamaz";
    }

    public static class User
    {
        public const string UsernameAlreadyExists = "Bu kullanıcı adı zaten kullanılıyor";
        public const string EmailAlreadyExists = "Bu e-posta adresi zaten kullanılıyor";
        public const string InvalidCredentials = "Kullanıcı adı veya şifre hatalı";
        public const string NotFound = "Kullanıcı bulunamadı";
        public const string WrongPassword = "Mevcut şifreniz yanlış.";
        public const string PasswordChanged = "Şifreniz başarıyla güncellendi.";
        public const string RolesAssigned = "Roller başarıyla atandı";
    }

    public static class Role
    {
        public const string NameAlreadyExists = "Bu rol adı zaten kullanılıyor";
        public const string NotFound = "Rol bulunamadı";
        public const string NoSwitchAccess = "Bu role geçiş yetkiniz yok.";
        public const string PermissionsAssigned = "İzinler başarıyla atandı";
    }

    public static class Permission
    {
        public const string CodeAlreadyExists = "Bu izin kodu zaten kullanılıyor";
        public const string NotFound = "İzin bulunamadı";
        public const string AlreadyExistsForAction = "Bu controller/action için izin zaten mevcut";
    }

    public static class Department
    {
        public const string CodeAlreadyExists = "Bu departman kodu zaten kullanılıyor";
        public const string NotFound = "Departman bulunamadı";
    }

    public static class Menu
    {
        public const string NotFound = "Menü bulunamadı";
    }

    public static class Auth
    {
        public const string InvalidRefreshToken = "Geçersiz yenileme token'ı";
        public const string TokenExpired = "Token süresi dolmuş";
        public const string LoggedOut = "Başarıyla çıkış yapıldı";
    }

    public static class RecycleBin
    {
        public const string Restored = "Kayıt başarıyla geri yüklendi.";
        public const string InvalidType = "Geçersiz tür";
    }

    public static class Settings
    {
        public const string NotFound = "Ayar bulunamadı.";
    }
}
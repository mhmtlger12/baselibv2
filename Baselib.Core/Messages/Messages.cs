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
    }

    public static class User
    {
        public const string UsernameAlreadyExists = "Bu kullanıcı adı zaten kullanılıyor";
        public const string EmailAlreadyExists = "Bu e-posta adresi zaten kullanılıyor";
        public const string InvalidCredentials = "Kullanıcı adı veya şifre hatalı";
        public const string NotFound = "Kullanıcı bulunamadı";
    }

    public static class Role
    {
        public const string NameAlreadyExists = "Bu rol adı zaten kullanılıyor";
        public const string NotFound = "Rol bulunamadı";
    }

    public static class Permission
    {
        public const string CodeAlreadyExists = "Bu izin kodu zaten kullanılıyor";
        public const string NotFound = "İzin bulunamadı";
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
}
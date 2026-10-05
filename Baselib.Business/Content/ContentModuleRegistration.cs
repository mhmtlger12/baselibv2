using System.Linq.Expressions;
using System.Reflection;
using Baselib.Core.Interfaces;
using Baselib.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Baselib.Business.Content;

public static class ContentModuleRegistration
{
    public static IServiceCollection AddSiteContentModules(this IServiceCollection services)
    {
        services.AddScoped<ContentRules>();
        Add<SiteNavigation>(services, "navigation", "Site Menüsü", [
            Field<SiteNavigation>("label", "Başlık", x => x.Label, "text", null, false),
            Field<SiteNavigation>("url", "Bağlantı", x => x.Url, "text", null, false),
            Field<SiteNavigation>("order", "Sıra", x => x.SortOrder, "number", null, false),
            Field<SiteNavigation>("active", "Aktif", x => x.IsActive, "checkbox", null, false)
        ]);
        Add<SiteAdvertisement>(services, "ads", "Reklam Alanları", [
            Field<SiteAdvertisement>("name", "Alan adı", x => x.Name, "text", null, false),
            Field<SiteAdvertisement>("position", "Konum", x => x.Position, "select", "positions", false),
            Field<SiteAdvertisement>("image", "Görsel URL", x => x.ImageUrl, "image", null, false),
            Field<SiteAdvertisement>("link", "Yönlendirme", x => x.LinkUrl, "text", null, false),
            Field<SiteAdvertisement>("active", "Aktif", x => x.IsActive, "checkbox", null, false)
        ]);
        Add<ExamCountdown>(services, "countdowns", "Sınav Sayaçları", [
            Field<ExamCountdown>("label", "Sınav adı", x => x.Label, "text", null, false),
            Field<ExamCountdown>("target", "Sınav tarihi (Türkiye saati)", x => x.Target, "datetime-local", null, false),
            Field<ExamCountdown>("order", "Sıra", x => x.SortOrder, "number", null, false),
            Field<ExamCountdown>("active", "Aktif", x => x.IsActive, "checkbox", null, false)
        ]);
        Add<SiteNews>(services, "sidebar-recent", "Son Eklenenler", [
            Field<SiteNews>("title", "Başlık", x => x.Title, "text", null, false),
            Field<SiteNews>("category", "Kategori", x => x.Category, "select", "news-categories", false),
            Field<SiteNews>("link", "Bağlantı", x => x.Link, "text", null, false),
            Field<SiteNews>("date", "Yayın tarihi", x => x.PublishedAt, "date", null, false),
            Field<SiteNews>("order", "Sıra", x => x.SortOrder, "number", null, false),
            Field<SiteNews>("active", "Aktif", x => x.IsActive, "checkbox", null, false)
        ], x => x.Section == "recent", x => x.Section = "recent");
        Add<SiteNews>(services, "sidebar-osym", "ÖSYM Duyuruları", [
            Field<SiteNews>("title", "Başlık", x => x.Title, "text", null, false),
            Field<SiteNews>("link", "Bağlantı", x => x.Link, "text", null, false),
            Field<SiteNews>("date", "Yayın tarihi", x => x.PublishedAt, "date", null, false),
            Field<SiteNews>("order", "Sıra", x => x.SortOrder, "number", null, false),
            Field<SiteNews>("active", "Aktif", x => x.IsActive, "checkbox", null, false)
        ], x => x.Section == "osym", x => x.Section = "osym");
        Add<ScoreCategory>(services, "score-cards", "Puan Kategorileri", [
            Field<ScoreCategory>("href", "Kategori anahtarı", x => x.Key, "text", null, true),
            Field<ScoreCategory>("title", "Kart başlığı", x => x.Title, "text", null, false),
            Field<ScoreCategory>("category", "Üst etiket", x => x.Label, "text", null, false),
            Field<ScoreCategory>("order", "Sıra", x => x.SortOrder, "number", null, false),
            Field<ScoreCategory>("active", "Aktif", x => x.IsActive, "checkbox", null, false)
        ]);
        Add<StudyLevel>(services, "levels", "Öğrenim Düzeyleri", [
            Field<StudyLevel>("key", "Anahtar", x => x.Key, "text", null, true),
            Field<StudyLevel>("label", "Başlık", x => x.Label, "text", null, false),
            Field<StudyLevel>("order", "Sıra", x => x.SortOrder, "number", null, false),
            Field<StudyLevel>("active", "Aktif", x => x.IsActive, "checkbox", null, false)
        ]);
        Add<StudyProgram>(services, "departments", "Bölümler", [
            Field<StudyProgram>("name", "Bölüm adı", x => x.Name, "text", null, false),
            Field<StudyProgram>("level", "Öğrenim düzeyi", x => x.StudyLevelId, "select", "levels", false),
            Field<StudyProgram>("active", "Aktif", x => x.IsActive, "checkbox", null, false)
        ]);
        Add<ScorePeriod>(services, "periods", "Puan Dönemleri", [
            Field<ScorePeriod>("name", "Dönem", x => x.Name, "text", null, false),
            Field<ScorePeriod>("order", "Sıra", x => x.SortOrder, "number", null, false),
            Field<ScorePeriod>("active", "Aktif", x => x.IsActive, "checkbox", null, false)
        ]);
        Add<JobCategory>(services, "job-categories", "İlan Kategorileri", [
            Field<JobCategory>("key", "Kategori anahtarı", x => x.Key, "text", null, true),
            Field<JobCategory>("label", "Kategori adı", x => x.Label, "text", null, false),
            Field<JobCategory>("parent", "Üst kategori", x => x.ParentKey, "select", "job-parents", false),
            Field<JobCategory>("selectable", "İlanlarda seçilebilir", x => x.IsSelectable, "checkbox", null, false),
            Field<JobCategory>("order", "Sıra", x => x.SortOrder, "number", null, false),
            Field<JobCategory>("active", "Aktif", x => x.IsActive, "checkbox", null, false)
        ]);
        Add<PublicComment>(services, "comments", "Yorumlar", [
            Field<PublicComment>("author", "Yazar", x => x.Author, "text", null, false),
            Field<PublicComment>("status", "Durum", x => x.Status, "select", "comment-status", false),
            Field<PublicComment>("body", "Yorum", x => x.Body, "textarea", null, false),
            Field<PublicComment>("page", "Puan sayfası adresi", x => x.Page, "text", null, false),
            Field<PublicComment>("hideName", "Adı gizle", x => x.HideName, "checkbox", null, false),
            Field<PublicComment>("likes", "Beğeni sayısı", x => x.Likes, "number", null, false),
            Field<PublicComment>("active", "Aktif", x => x.IsActive, "checkbox", null, false)
        ]);
        Add<ContactMessage>(services, "messages", "İletişim Mesajları", [
            Field<ContactMessage>("name", "Ad soyad", x => x.Name, "text", null, false),
            Field<ContactMessage>("email", "E-posta", x => x.Email, "text", null, false),
            Field<ContactMessage>("subject", "Konu", x => x.Subject, "text", null, false),
            Field<ContactMessage>("message", "Mesaj", x => x.Message, "textarea", null, false),
            Field<ContactMessage>("status", "Durum", x => x.Status, "select", "message-status", false),
            Field<ContactMessage>("active", "Aktif", x => x.IsActive, "checkbox", null, false)
        ]);
        Add<InformationPage>(services, "pages", "Bilgi Sayfaları", [
            Field<InformationPage>("slug", "Sayfa anahtarı", x => x.Slug, "text", null, true),
            Field<InformationPage>("title", "Başlık", x => x.Title, "text", null, false),
            Field<InformationPage>("body", "İçerik", x => x.Body, "textarea", null, false),
            Field<InformationPage>("active", "Aktif", x => x.IsActive, "checkbox", null, false)
        ]);
        foreach (var exam in new[] { "kpss", "dgs", "yks" })
        {
            Add<ScoreEntry>(services, "taban-" + exam, exam.ToUpperInvariant() + " Taban Puanları", [
                Field<ScoreEntry>("category", "Kategori", x => x.ScoreCategoryId, "select", "score-" + exam, false),
                Field<ScoreEntry>("program", "Bölüm", x => x.StudyProgramId, "select", "programs", false),
                Field<ScoreEntry>("period", "Dönem", x => x.ScorePeriodId, "select", "periods", false),
                Field<ScoreEntry>("institution", "Kurum / Üniversite", x => x.Institution, "text", null, false),
                Field<ScoreEntry>("city", "Şehir", x => x.City, "text", null, false),
                Field<ScoreEntry>("title", "Unvan", x => x.Title, "text", null, false),
                Field<ScoreEntry>("quota", "Kontenjan", x => x.Quota, "number", null, false),
                Field<ScoreEntry>("vacant", "Boş kontenjan", x => x.Vacant, "number", null, false),
                Field<ScoreEntry>("score", "En düşük puan", x => x.MinScore, "number", null, false),
                Field<ScoreEntry>("maxScore", "En yüksek puan", x => x.MaxScore, "number", null, false),
                Field<ScoreEntry>("rank", "Başarı sırası", x => x.Rank, "number", null, false),
                Field<ScoreEntry>("qualification", "Nitelik", x => x.Qualification, "text", null, false),
                Field<ScoreEntry>("active", "Aktif", x => x.IsActive, "checkbox", null, false)
            ], x => exam == "kpss" ? x.ScoreCategory!.Key.StartsWith("kpss") : x.ScoreCategory!.Key == exam);
        }
        return services;
    }

    private static ContentBinding Field<T>(string key, string label, Expression<Func<T, object?>> property,
        string type, string? lookup, bool immutable)
    {
        var member = property.Body is UnaryExpression unary ? (MemberExpression)unary.Operand : (MemberExpression)property.Body;
        return new(key, label, (PropertyInfo)member.Member, type, lookup, immutable);
    }

    private static void Add<T>(IServiceCollection services, string slug, string title, ContentBinding[] fields,
        Expression<Func<T, bool>>? scope = null, Action<T>? initialize = null) where T : SoftDeleteEntity, new() =>
        services.AddScoped<IContentModule>(provider => new ContentModule<T>(slug, title, fields,
            provider.GetRequiredService<IEntityRepository<T>>(), provider.GetRequiredService<ContentRules>(),
            provider.GetRequiredService<IUnitOfWork>(), provider.GetRequiredService<TimeProvider>(), scope, initialize));
}

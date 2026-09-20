using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
namespace BaseLib.Presentation.Services.Content;

// Mock data lives for the application lifetime and is intentionally independent of the public site.
public sealed class MockContentService : IContentService
{
    private readonly Dictionary<string, ContentDefinition> definitions;
    private readonly object gate = new();
    public MockContentService(IWebHostEnvironment environment)
    {
        definitions = JsonSerializer.Deserialize<Dictionary<string, ContentDefinition>>(
            File.ReadAllText(Path.Combine(environment.ContentRootPath, "Data", "content.json")), new JsonSerializerOptions(JsonSerializerDefaults.Web))!
            .ToDictionary(x => x.Key.Replace("content/", ""), x => x.Value);
    }
    public ContentPage? List(string slug)
    {
        lock (gate)
        {
            return definitions.TryGetValue(slug, out var definition)
                ? new ContentPage(slug, definition, definition.Rows.Select(x => (JsonObject)x.DeepClone()).ToList()) : null;
        }
    }
    public ContentEditModel? Edit(string slug, int id)
    {
        lock (gate)
        {
            if (!definitions.TryGetValue(slug, out var definition)) return null;
            var row = definition.Rows.Find(x => x["id"]!.GetValue<int>() == id);
            if (id != 0 && row is null) return null;
            return new ContentEditModel
            {
                Slug = slug, Id = id, Definition = definition,
                Values = definition.Fields.ToDictionary(x => x.Key, x => row?[x.Key]?.ToString() ?? (x.Type == "checkbox" ? "true" : x.Type == "number" ? "0" : ""))!
            };
        }
    }
    public IReadOnlyDictionary<string, string> Save(ContentEditModel model)
    {
        lock (gate)
        {
            var errors = new Dictionary<string, string>();
            if (!definitions.TryGetValue(model.Slug, out var definition)) return new Dictionary<string, string> { [""] = "İçerik türü bulunamadı." };
            var row = new JsonObject();
            foreach (var field in definition.Fields)
            {
                var value = model.Values.GetValueOrDefault(field.Key)?.Trim() ?? "";
                var key = $"Values[{field.Key}]";
                if (value.Length > 8000) { errors[key] = "Alan en fazla 8000 karakter olabilir."; continue; }
                if (field.Type == "checkbox") row[field.Key] = string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
                else if (field.Type == "number")
                {
                    if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var number) && number >= 0) row[field.Key] = number;
                    else errors[key] = "Sıfır veya daha büyük bir sayı girin.";
                }
                else if (field.Type == "date")
                {
                    if (DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)) row[field.Key] = value;
                    else errors[key] = "Geçerli bir tarih girin.";
                }
                else if (field.Type == "select" && !(field.Options?.Contains(value) ?? false)) errors[key] = "Listeden bir seçenek seçin.";
                else if ((field.Type == "image" || field.Key == "link") && value.Length > 0 && !SafeUrl(value)) errors[key] = "Yerel bir yol veya http/https adresi girin.";
                else if (field.Key == "email" && !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(value)) errors[key] = "Geçerli bir e-posta adresi girin.";
                else row[field.Key] = value;
                if (field.Key is "title" or "name" or "label" or "institution" or "program" or "author" && value.Length == 0) errors[key] = "Bu alan zorunludur.";
            }
            if (errors.Count > 0) return errors;
            var index = definition.Rows.FindIndex(x => x["id"]!.GetValue<int>() == model.Id);
            if (model.Id != 0 && index < 0) return new Dictionary<string, string> { [""] = "Kayıt bulunamadı; başka bir oturum tarafından silinmiş olabilir." };
            row["id"] = model.Id == 0 ? definition.Rows.Select(x => x["id"]!.GetValue<int>()).DefaultIfEmpty().Max() + 1 : model.Id;
            if (index >= 0) definition.Rows[index] = row;
            else definition.Rows.Add(row);
            return errors;
        }
    }
    public bool Delete(string slug, int id)
    {
        lock (gate) return definitions.TryGetValue(slug, out var definition) && definition.Rows.RemoveAll(x => x["id"]!.GetValue<int>() == id) > 0;
    }
    private static bool SafeUrl(string value) => (value.StartsWith('/') && !value.StartsWith("//") && !value.Contains('\\')) ||
        (Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme is "http" or "https");
}

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Baselib.Business.DTOs;
using Baselib.Core.Interfaces;
using Baselib.Core.Results;
using Baselib.Entities;

namespace Baselib.Business.Content;

public sealed record ContentBinding(string Key, string Label, PropertyInfo Property, string Type, string? Lookup = null, bool Immutable = false);

public interface IContentModule
{
    string Slug { get; }
    string Title { get; }
    Task<ContentModuleDto> ReadAsync(CancellationToken ct);
    Task<IDataResult<int>> SaveAsync(int id, SaveContentDto input, CancellationToken ct);
    Task<IResult> DeleteAsync(int id, CancellationToken ct);
    Task<IEnumerable<RecycleBinItemDto>> DeletedAsync(CancellationToken ct);
    Task<IResult> RestoreAsync(int id, CancellationToken ct);
}

// Form metadata describes allowed fields; actual values are stored in typed entity columns.
public sealed class ContentModule<T>(string slug, string title, ContentBinding[] bindings,
    IEntityRepository<T> repository, ContentRules rules, IUnitOfWork unitOfWork, TimeProvider clock,
    Expression<Func<T, bool>>? scope = null, Action<T>? initialize = null) : IContentModule where T : SoftDeleteEntity, new()
{
    public string Slug => slug;
    public string Title => title;

    public async Task<ContentModuleDto> ReadAsync(CancellationToken ct)
    {
        var fields = new List<ContentFieldDto>();
        foreach (var binding in bindings)
        {
            var options = binding.Lookup is null ? null : await rules.OptionsAsync(binding.Lookup, ct);
            fields.Add(new(binding.Key, binding.Label, binding.Type,
                binding.Property.IsDefined(typeof(RequiredAttribute)),
                binding.Property.GetCustomAttribute<StringLengthAttribute>()?.MaximumLength ?? 8000,
                options, binding.Immutable ? "Kaydedildikten sonra anahtar değiştirilemez." : null, binding.Immutable));
        }
        var rows = await repository.GetAllAsync(scope, ignoreQueryFilters: true, asNoTracking: true, cancellationToken: ct);
        return new(slug, title, fields, rows.Where(x => !x.IsDeleted).OrderBy(x => x.Id).Select(Row).ToList());
    }

    public async Task<IDataResult<int>> SaveAsync(int id, SaveContentDto input, CancellationToken ct)
    {
        if (input.Values is null || input.Values.Count > 64 || input.Values.Keys.Any(key => !bindings.Any(b => b.Key == key)))
            return DataResult<int>.BadRequest("Geçersiz içerik alanı.");
        var previous = id == 0 ? null : await FindAsync(id, ct);
        if (id != 0 && (previous is null || previous.IsDeleted)) return DataResult<int>.NotFound();
        var item = new T();
        // Validate a detached candidate, so a rejected update cannot alter tracked state.
        if (previous is not null)
            foreach (var property in typeof(T).GetProperties().Where(p => p.CanRead && p.CanWrite))
                property.SetValue(item, property.GetValue(previous));
        initialize?.Invoke(item);
        foreach (var binding in bindings)
        {
            var value = input.Values.GetValueOrDefault(binding.Key)?.Trim() ?? "";
            if (value.Length > 8000) return DataResult<int>.BadRequest($"{binding.Label}: Alan çok uzun.");
            if (binding.Immutable && previous is not null && value != Format(binding.Property.GetValue(previous), binding.Type))
                return DataResult<int>.BadRequest($"{binding.Label}: Kaydedilmiş anahtar değiştirilemez.");
            if (binding.Lookup is not null && !(await rules.OptionsAsync(binding.Lookup, ct)).Any(x => x.Value == value))
                return DataResult<int>.BadRequest($"{binding.Label}: Geçerli bir seçenek seçin.");
            try { binding.Property.SetValue(item, Parse(value, binding.Property.PropertyType, binding.Type)); }
            catch (Exception e) when (e is FormatException or OverflowException or ArgumentException)
            { return DataResult<int>.BadRequest($"{binding.Label}: Geçerli bir değer girin."); }
        }
        var errors = new List<ValidationResult>();
        if (!Validator.TryValidateObject(item, new(item), errors, true))
            return DataResult<int>.BadRequest(string.Join(" ", errors.Select(x => x.ErrorMessage)));
        var validation = await rules.ValidateAsync(item, ct);
        if (validation is not null) return DataResult<int>.BadRequest(validation);
        if (id == 0)
        {
            item.CreatedDate = clock.GetUtcNow().UtcDateTime;
            await repository.AddAsync(item, ct);
        }
        else
        {
            item.UpdatedDate = clock.GetUtcNow().UtcDateTime;
            repository.Update(item);
        }
        await unitOfWork.SaveChangesAsync(ct);
        return DataResult<int>.Ok(item.Id, "İçerik kaydedildi.");
    }

    public async Task<IResult> DeleteAsync(int id, CancellationToken ct)
    {
        var item = await FindAsync(id, ct);
        if (item is null || item.IsDeleted) return Result.NotFound();
        item.IsDeleted = true;
        item.IsActive = false;
        item.UpdatedDate = clock.GetUtcNow().UtcDateTime;
        repository.Update(item);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Ok("İçerik çöp kutusuna taşındı.");
    }

    public async Task<IEnumerable<RecycleBinItemDto>> DeletedAsync(CancellationToken ct)
    {
        var rows = await repository.GetAllAsync(scope, ignoreQueryFilters: true, asNoTracking: true, cancellationToken: ct);
        return rows.Where(x => x.IsDeleted).Select(x => new RecycleBinItemDto
        { Id = x.Id, Type = "Content:" + slug, TypeName = title, Name = Row(x).GetValueOrDefault(bindings[0].Key) ?? title, DeletedDate = x.UpdatedDate });
    }

    public async Task<IResult> RestoreAsync(int id, CancellationToken ct)
    {
        var item = await FindAsync(id, ct);
        if (item is null || !item.IsDeleted) return Result.NotFound();
        item.IsDeleted = false;
        item.IsActive = false;
        item.UpdatedDate = clock.GetUtcNow().UtcDateTime;
        repository.Update(item);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Ok("İçerik pasif olarak geri yüklendi.");
    }

    private async Task<T?> FindAsync(int id, CancellationToken ct)
    {
        var parameter = scope?.Parameters[0] ?? Expression.Parameter(typeof(T), "item");
        var idMatch = Expression.Equal(Expression.Property(parameter, nameof(BaseEntity.Id)), Expression.Constant(id));
        var predicate = Expression.Lambda<Func<T, bool>>(scope is null ? idMatch : Expression.AndAlso(scope.Body, idMatch), parameter);
        return (await repository.GetAllAsync(predicate, ignoreQueryFilters: true, asNoTracking: true, cancellationToken: ct)).SingleOrDefault();
    }

    private Dictionary<string, string?> Row(T item)
    {
        var row = bindings.ToDictionary(b => b.Key, b => (string?)Format(b.Property.GetValue(item), b.Type));
        row["id"] = item.Id.ToString(CultureInfo.InvariantCulture);
        return row;
    }

    private static string Format(object? value, string type) => value switch
    {
        DateTime date => date.ToString(type == "datetime-local" ? "yyyy-MM-ddTHH:mm" : "yyyy-MM-dd", CultureInfo.InvariantCulture),
        bool boolean => boolean ? "true" : "false",
        IFormattable formatted => formatted.ToString(null, CultureInfo.InvariantCulture),
        _ => value?.ToString() ?? ""
    };

    private static object? Parse(string value, Type type, string fieldType)
    {
        if (Nullable.GetUnderlyingType(type) is Type underlying)
        {
            if (value.Length == 0) return null;
            type = underlying;
        }
        if (type == typeof(string)) return value;
        if (type == typeof(bool)) return value == "" ? false : bool.Parse(value);
        if (type == typeof(int)) return int.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
        if (type == typeof(decimal)) return decimal.Parse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
        if (type == typeof(DateTime)) return DateTime.ParseExact(value,
            fieldType == "datetime-local" ? ["yyyy-MM-ddTHH:mm", "yyyy-MM-ddTHH:mm:ss"] : ["yyyy-MM-dd"], CultureInfo.InvariantCulture, DateTimeStyles.None);
        throw new ArgumentException("Desteklenmeyen alan türü.");
    }
}

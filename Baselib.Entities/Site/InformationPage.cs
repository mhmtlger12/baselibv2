using System.ComponentModel.DataAnnotations;
using Baselib.Entities.Validation;

namespace Baselib.Entities;

public sealed class InformationPage : SoftDeleteEntity
{
    [Required, StringLength(80), RegularExpression("^[a-z0-9]+(?:-[a-z0-9]+)*$")] public string Slug { get; set; } = "";
    [Required, StringLength(200)] public string Title { get; set; } = "";
    [Required, StringLength(8000)] public string Body { get; set; } = "";
}

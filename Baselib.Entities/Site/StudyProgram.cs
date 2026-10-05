using System.ComponentModel.DataAnnotations;
using Baselib.Entities.Validation;

namespace Baselib.Entities;

public sealed class StudyProgram : SoftDeleteEntity
{
    [Required, StringLength(200)] public string Name { get; set; } = "";
    [Range(1,int.MaxValue)] public int StudyLevelId { get; set; }
    public StudyLevel? StudyLevel { get; set; }
}

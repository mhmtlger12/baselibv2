using System.ComponentModel.DataAnnotations;
using Baselib.Entities.Validation;

namespace Baselib.Entities;

public sealed class ScoreEntry : SoftDeleteEntity
{
    [Range(1,int.MaxValue)] public int ScoreCategoryId { get; set; }
    public ScoreCategory? ScoreCategory { get; set; }
    [Range(1,int.MaxValue)] public int StudyProgramId { get; set; }
    public StudyProgram? StudyProgram { get; set; }
    [Range(1,int.MaxValue)] public int ScorePeriodId { get; set; }
    public ScorePeriod? ScorePeriod { get; set; }
    [Required, StringLength(200)] public string Institution { get; set; } = "";
    [StringLength(100)] public string City { get; set; } = "";
    [StringLength(200)] public string Title { get; set; } = "";
    [Range(0,int.MaxValue)] public int Quota { get; set; }
    [Range(0,int.MaxValue)] public int Vacant { get; set; }
    [Range(0,1000)] public decimal MinScore { get; set; }
    [Range(0,1000)] public decimal MaxScore { get; set; }
    [Range(0,int.MaxValue)] public int Rank { get; set; }
    [StringLength(300)] public string Qualification { get; set; } = "";
}

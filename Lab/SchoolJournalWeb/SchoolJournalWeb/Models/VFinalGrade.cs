namespace SchoolJournalWeb.Models;

/// <summary>Представление dbo.v_FinalGrades (итоговые отметки) — сущность без ключа.</summary>
public partial class VFinalGrade
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = null!;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = null!;
    public string AcademicYear { get; set; } = null!;
    public string PeriodType { get; set; } = null!;
    public double? FinalGrade { get; set; }
}

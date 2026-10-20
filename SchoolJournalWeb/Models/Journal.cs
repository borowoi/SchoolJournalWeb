namespace SchoolJournalWeb.Models;

public partial class Journal
{
    public int JournalId { get; set; }
    public int StudentId { get; set; }
    public int LessonId { get; set; }
    public bool IsPresent { get; set; }
    public string? AbsenceReason { get; set; }
    public int? GradeValue { get; set; }
    public string? WorkType { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
}

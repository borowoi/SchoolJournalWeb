namespace SchoolJournalWeb.Models;

public partial class SubjectHour
{
    public int SubjectId { get; set; }
    public int GradeLevel { get; set; }
    public int HoursPerWeek { get; set; }

    public virtual Subject Subject { get; set; } = null!;
}

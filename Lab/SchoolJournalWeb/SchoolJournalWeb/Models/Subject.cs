namespace SchoolJournalWeb.Models;

public partial class Subject
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = null!;
    public string ExamType { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public virtual ICollection<SubjectHour> SubjectHours { get; set; } = new List<SubjectHour>();

    public override string ToString() => $"{SubjectId}. {SubjectName} ({ExamType})";
}

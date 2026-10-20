namespace SchoolJournalWeb.Models;

public partial class Lesson
{
    public int LessonId { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public int? TeacherId { get; set; }
    public DateOnly LessonDate { get; set; }
    public int LessonNumber { get; set; }
    public string Topic { get; set; } = null!;

    public virtual Class Class { get; set; } = null!;
    public virtual Subject Subject { get; set; } = null!;
    public virtual Teacher? Teacher { get; set; }
    public virtual ICollection<Journal> Journals { get; set; } = new List<Journal>();
}

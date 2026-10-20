namespace SchoolJournalWeb.Models;

public partial class Teacher
{
    public int TeacherId { get; set; }
    public string FullName { get; set; } = null!;
    public string QualificationCategory { get; set; } = null!;
    public int WorkloadHours { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}

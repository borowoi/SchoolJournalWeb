namespace SchoolJournalWeb.Models;

public partial class Class
{
    public int ClassId { get; set; }
    public int GradeLevel { get; set; }
    public string ClassLetter { get; set; } = null!;
    public int? HomeroomTeacherId { get; set; }
    public string AcademicYear { get; set; } = null!;

    public virtual Teacher? HomeroomTeacher { get; set; }
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}

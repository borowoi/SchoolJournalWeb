namespace SchoolJournalWeb.Models;

public partial class Student
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = null!;
    public DateOnly BirthDate { get; set; }
    public int CurrentClassId { get; set; }
    public string? ContactInfo { get; set; }
    public DateOnly EnrollmentDate { get; set; }

    public virtual Class CurrentClass { get; set; } = null!;
    public virtual ICollection<Journal> Journals { get; set; } = new List<Journal>();
}

using Microsoft.EntityFrameworkCore;
using SchoolJournalWeb.Models;

namespace SchoolJournalWeb.Data;

public partial class SchoolJournalDbContext : DbContext
{
    public SchoolJournalDbContext()
    {
    }

    public SchoolJournalDbContext(DbContextOptions<SchoolJournalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; }
    public virtual DbSet<Journal> Journals { get; set; }
    public virtual DbSet<Lesson> Lessons { get; set; }
    public virtual DbSet<Student> Students { get; set; }
    public virtual DbSet<Subject> Subjects { get; set; }
    public virtual DbSet<SubjectHour> SubjectHours { get; set; }
    public virtual DbSet<Teacher> Teachers { get; set; }
    public virtual DbSet<VFinalGrade> VFinalGrades { get; set; }

    // Строка подключения берётся из appsettings.json (см. Program.cs),
    // поэтому OnConfiguring здесь не переопределяется.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.ToTable("Teachers");
            entity.HasKey(e => e.TeacherId).HasName("PK_Teachers");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.QualificationCategory).HasMaxLength(50);
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.ToTable("Subjects");
            entity.HasKey(e => e.SubjectId).HasName("PK_Subjects");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.SubjectName).HasMaxLength(100);
            entity.Property(e => e.ExamType).HasMaxLength(50);
        });

        modelBuilder.Entity<SubjectHour>(entity =>
        {
            entity.ToTable("SubjectHours");
            entity.HasKey(e => new { e.SubjectId, e.GradeLevel }).HasName("PK_SubjectHours");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");

            entity.HasOne(d => d.Subject).WithMany(p => p.SubjectHours)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SubjectHours_Subjects");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.ToTable("Classes");
            entity.HasKey(e => e.ClassId).HasName("PK_Classes");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.AcademicYear).HasMaxLength(9);
            entity.Property(e => e.ClassLetter).HasMaxLength(1).IsFixedLength();
            entity.Property(e => e.HomeroomTeacherId).HasColumnName("HomeroomTeacherID");

            entity.HasOne(d => d.HomeroomTeacher).WithMany(p => p.Classes)
                .HasForeignKey(d => d.HomeroomTeacherId)
                .HasConstraintName("FK_Classes_Teachers");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students");
            entity.HasKey(e => e.StudentId).HasName("PK_Students");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.ContactInfo).HasMaxLength(250);
            entity.Property(e => e.CurrentClassId).HasColumnName("CurrentClassID");
            entity.Property(e => e.FullName).HasMaxLength(150);

            entity.HasOne(d => d.CurrentClass).WithMany(p => p.Students)
                .HasForeignKey(d => d.CurrentClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Students_Classes");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.ToTable("Lessons");
            entity.HasKey(e => e.LessonId).HasName("PK_Lessons");
            entity.Property(e => e.LessonId).HasColumnName("LessonID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.Topic).HasMaxLength(250);

            entity.HasOne(d => d.Class).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lessons_Classes");

            entity.HasOne(d => d.Subject).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lessons_Subjects");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_Lessons_Teachers");
        });

        modelBuilder.Entity<Journal>(entity =>
        {
            entity.ToTable("Journal");
            entity.HasKey(e => e.JournalId).HasName("PK_Journal");
            entity.Property(e => e.JournalId).HasColumnName("JournalID");
            entity.Property(e => e.AbsenceReason).HasMaxLength(50);
            entity.Property(e => e.LessonId).HasColumnName("LessonID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.WorkType).HasMaxLength(50);

            entity.HasOne(d => d.Lesson).WithMany(p => p.Journals)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Journal_Lessons");

            entity.HasOne(d => d.Student).WithMany(p => p.Journals)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Journal_Students");
        });

        modelBuilder.Entity<VFinalGrade>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("v_FinalGrades");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName).HasMaxLength(150);
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.SubjectName).HasMaxLength(100);
            entity.Property(e => e.AcademicYear).HasMaxLength(9);
            entity.Property(e => e.PeriodType).HasMaxLength(10);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

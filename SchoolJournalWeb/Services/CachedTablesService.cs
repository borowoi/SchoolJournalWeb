using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SchoolJournalWeb.Data;
using SchoolJournalWeb.Models;

namespace SchoolJournalWeb.Services;

public class CachedTablesService
{
    private const int VariantNumber = 3;   // номер варианта N
    private const int RowsNumber = 20;     // сколько записей кэшировать из каждой таблицы
    private static readonly TimeSpan CacheTime = TimeSpan.FromSeconds(2 * VariantNumber + 240); // 246 с

    private readonly SchoolJournalDbContext _db;
    private readonly IMemoryCache _cache;

    public CachedTablesService(SchoolJournalDbContext context, IMemoryCache memoryCache)
    {
        _db = context;
        _cache = memoryCache;
    }

    public IEnumerable<Teacher> GetTeachers() =>
        GetOrAdd("Teachers20", () =>
            _db.Teachers.AsNoTracking().OrderBy(t => t.TeacherId).Take(RowsNumber).ToList());

    public IEnumerable<Subject> GetSubjects() =>
        GetOrAdd("Subjects20", () =>
            _db.Subjects.AsNoTracking().OrderBy(s => s.SubjectId).Take(RowsNumber).ToList());

    public IEnumerable<SubjectHour> GetSubjectHours() =>
        GetOrAdd("SubjectHours20", () =>
            _db.SubjectHours.AsNoTracking().OrderBy(h => h.SubjectId).ThenBy(h => h.GradeLevel).Take(RowsNumber).ToList());

    public IEnumerable<Class> GetClasses() =>
        GetOrAdd("Classes20", () =>
            _db.Classes.AsNoTracking().OrderBy(c => c.ClassId).Take(RowsNumber).ToList());

    public IEnumerable<Student> GetStudents() =>
        GetOrAdd("Students20", () =>
            _db.Students.AsNoTracking().OrderBy(s => s.StudentId).Take(RowsNumber).ToList());

    public IEnumerable<Lesson> GetLessons() =>
        GetOrAdd("Lessons20", () =>
            _db.Lessons.AsNoTracking().OrderBy(l => l.LessonId).Take(RowsNumber).ToList());

    public IEnumerable<Journal> GetJournal() =>
        GetOrAdd("Journal20", () =>
            _db.Journals.AsNoTracking().OrderBy(j => j.JournalId).Take(RowsNumber).ToList());

    // Возвращает данные из кэша, а если их там нет — загружает из БД и кладёт в кэш на 246 секунд
    private List<T> GetOrAdd<T>(string cacheKey, Func<List<T>> load)
    {
        if (!_cache.TryGetValue(cacheKey, out List<T>? rows) || rows == null)
        {
            rows = load();
            _cache.Set(cacheKey, rows, new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(CacheTime));
            Console.WriteLine($"[Кэш] {cacheKey}: загружено из БД, храним {CacheTime.TotalSeconds} с");
        }
        else
        {
            Console.WriteLine($"[Кэш] {cacheKey}: получено из кэша");
        }
        return rows;
    }
}

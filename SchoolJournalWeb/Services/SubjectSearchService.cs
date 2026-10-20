using Microsoft.EntityFrameworkCore;
using SchoolJournalWeb.Data;
using SchoolJournalWeb.Infrastructure;
using SchoolJournalWeb.Models;

namespace SchoolJournalWeb.Services;

public class SubjectSearchService
{
    private readonly SchoolJournalDbContext _db;

    public SubjectSearchService(SchoolJournalDbContext db)
    {
        _db = db;
    }

    public List<Subject> Search(SearchFormState state)
    {
        IQueryable<Subject> query = _db.Subjects.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(state.NameFilter))
        {
            string name = state.NameFilter.Trim();
            query = query.Where(s => s.SubjectName.Contains(name));
        }

        if (state.ExamType != SearchFormState.AnyExamType)
        {
            query = query.Where(s => s.ExamType == state.ExamType);
        }

        query = state.SortBy == "id"
            ? query.OrderBy(s => s.SubjectId)
            : query.OrderBy(s => s.SubjectName);

        return query.Take(20).ToList();
    }
}

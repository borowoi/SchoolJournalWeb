using Microsoft.AspNetCore.Http;

namespace SchoolJournalWeb.Infrastructure;

public class SearchFormState
{
    public const string AnyExamType = "Любой";

    public string NameFilter { get; set; } = "";          // текстовое поле
    public string ExamType { get; set; } = AnyExamType;   // выпадающий список
    public string SortBy { get; set; } = "name";          // переключатели: name / id

    // Создаёт объект из параметров GET-запроса формы
    public static SearchFormState FromQuery(IQueryCollection query)
    {
        string exam = query["exam"].ToString();
        return new SearchFormState
        {
            NameFilter = query["name"].ToString(),
            ExamType = string.IsNullOrEmpty(exam) ? AnyExamType : exam,
            SortBy = query["sort"].ToString() == "id" ? "id" : "name"
        };
    }
}

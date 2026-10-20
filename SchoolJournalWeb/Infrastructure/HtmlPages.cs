using System.Net;
using System.Text;
using SchoolJournalWeb.Models;

namespace SchoolJournalWeb.Infrastructure;

public static class HtmlPages
{
    public const string ContentType = "text/html; charset=utf-8";

    private static readonly string[] Links =
        { "info", "teachers", "subjects", "subjecthours", "classes", "students", "lessons", "journal", "searchform1", "searchform2" };

    public static string Encode(object? value) => WebUtility.HtmlEncode(value?.ToString() ?? "");

    public static string PageStart(string title)
    {
        var sb = new StringBuilder();
        sb.Append("<!DOCTYPE html><html><head><meta charset='utf-8'>");
        sb.Append($"<title>{Encode(title)}</title>");
        sb.Append("<style>body{font-family:Arial,sans-serif;margin:20px;}");
        sb.Append("table{border-collapse:collapse;}td,th{border:1px solid #888;padding:4px 8px;}");
        sb.Append("th{background:#eee;}nav a{margin-right:12px;}</style></head><body><nav>");
        foreach (string link in Links)
            sb.Append($"<a href='/{link}'>{link}</a>");
        sb.Append("</nav>");
        return sb.ToString();
    }

    public static string PageEnd() => "</body></html>";

    // ----- Таблицы: заголовок, затем строки по одной, затем конец -----
    public static string TableStart(string title, IEnumerable<string> headers)
    {
        var sb = new StringBuilder(PageStart(title));
        sb.Append($"<h1>{Encode(title)}</h1><table><tr>");
        foreach (string header in headers)
            sb.Append($"<th>{Encode(header)}</th>");
        sb.Append("</tr>");
        return sb.ToString();
    }

    public static string TableRow(IEnumerable<string> cells)
    {
        var sb = new StringBuilder("<tr>");
        foreach (string cell in cells)
            sb.Append($"<td>{Encode(cell)}</td>");
        sb.Append("</tr>");
        return sb.ToString();
    }

    public static string TableEnd() => "</table>" + PageEnd();

    public static string Message(string title, string text) =>
        PageStart(title) + $"<h1>{Encode(title)}</h1><p>{Encode(text)}</p>" + PageEnd();

    // ----- Форма поиска: значения элементов берутся из сохранённого состояния -----
    public static string SearchPage(string title, string action, string storage,
                                    SearchFormState state, List<Subject>? results)
    {
        var sb = new StringBuilder(PageStart(title));
        sb.Append($"<h1>{Encode(title)}</h1>");
        sb.Append($"<p>Состояние формы хранится в: <b>{Encode(storage)}</b></p>");

        sb.Append($"<form action='{action}' method='get'>");

        // текстовое поле
        sb.Append("<p>Название предмета содержит:<br>");
        sb.Append($"<input type='text' name='name' value='{Encode(state.NameFilter)}'></p>");

        // выпадающий список
        sb.Append("<p>Вид контроля:<br><select name='exam'>");
        string[] exams = { SearchFormState.AnyExamType, "Экзамен", "Зачет", "Дифференцированный зачет" };
        foreach (string exam in exams)
        {
            string selected = exam == state.ExamType ? " selected" : "";
            sb.Append($"<option value='{Encode(exam)}'{selected}>{Encode(exam)}</option>");
        }
        sb.Append("</select></p>");

        // переключатели
        string byName = state.SortBy == "name" ? " checked" : "";
        string byId = state.SortBy == "id" ? " checked" : "";
        sb.Append("<p>Сортировка:<br>");
        sb.Append($"<label><input type='radio' name='sort' value='name'{byName}> по названию</label> ");
        sb.Append($"<label><input type='radio' name='sort' value='id'{byId}> по коду</label></p>");

        // кнопка отправки
        sb.Append("<input type='submit' value='Найти'></form>");

        if (results != null)
        {
            sb.Append("<h2>Результаты поиска</h2>");
            if (results.Count == 0)
            {
                sb.Append("<p>Ничего не найдено.</p>");
            }
            else
            {
                sb.Append("<table><tr><th>Код</th><th>Название</th><th>Вид контроля</th></tr>");
                foreach (Subject s in results)
                {
                    sb.Append("<tr>");
                    sb.Append($"<td>{s.SubjectId}</td><td>{Encode(s.SubjectName)}</td><td>{Encode(s.ExamType)}</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</table>");
            }
        }

        sb.Append(PageEnd());
        return sb.ToString();
    }
}

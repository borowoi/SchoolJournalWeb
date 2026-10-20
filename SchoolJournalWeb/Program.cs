using Microsoft.EntityFrameworkCore;
using SchoolJournalWeb.Data;
using SchoolJournalWeb.Infrastructure;
using SchoolJournalWeb.Models;
using SchoolJournalWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------- Регистрация сервисов ----------
builder.Services.AddDbContext<SchoolJournalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMemoryCache();              // IMemoryCache
builder.Services.AddDistributedMemoryCache();   // хранилище для Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<CachedTablesService>();
builder.Services.AddScoped<SubjectSearchService>();

var app = builder.Build();

app.UseSession();

// ---------- Вспомогательный метод: ветка Map для вывода кэшированной таблицы ----------
void MapTable<T>(string path, string title, string[] headers,
                 Func<CachedTablesService, IEnumerable<T>> getRows, Func<T, string[]> toCells)
{
    app.Map(path, appBuilder =>
    {
        appBuilder.Run(async context =>
        {
            // получаем сервис кэширования из контейнера зависимостей
            CachedTablesService service = context.RequestServices.GetRequiredService<CachedTablesService>();
            IEnumerable<T> rows = getRows(service);

            context.Response.ContentType = HtmlPages.ContentType;
            await context.Response.WriteAsync(HtmlPages.TableStart(title, headers));
            foreach (T row in rows)
            {
                await context.Response.WriteAsync(HtmlPages.TableRow(toCells(row)));
            }
            await context.Response.WriteAsync(HtmlPages.TableEnd());
        });
    });
}

// ---------- /info : информация о клиенте ----------
app.Map("/info", appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var request = context.Request;
        var rows = new List<string[]>
        {
            new[] { "IP-адрес клиента", context.Connection.RemoteIpAddress?.ToString() ?? "" },
            new[] { "Порт клиента", context.Connection.RemotePort.ToString() },
            new[] { "Метод запроса", request.Method },
            new[] { "Протокол", request.Protocol },
            new[] { "Схема", request.Scheme },
            new[] { "Хост", request.Host.ToString() },
            new[] { "Путь", request.Path.ToString() },
            new[] { "Браузер (User-Agent)", request.Headers["User-Agent"].ToString() },
            new[] { "Язык (Accept-Language)", request.Headers["Accept-Language"].ToString() }
        };

        context.Response.ContentType = HtmlPages.ContentType;
        await context.Response.WriteAsync(HtmlPages.TableStart("Информация о клиенте", new[] { "Параметр", "Значение" }));
        foreach (string[] row in rows)
        {
            await context.Response.WriteAsync(HtmlPages.TableRow(row));
        }
        await context.Response.WriteAsync(HtmlPages.TableEnd());
    });
});

// ---------- Таблицы базы данных (по 20 записей из кэша) ----------
MapTable("/teachers", "Учителя",
    new[] { "Код", "ФИО", "Категория", "Нагрузка (ч)" },
    s => s.GetTeachers(),
    t => new[] { t.TeacherId.ToString(), t.FullName, t.QualificationCategory, t.WorkloadHours.ToString() });

MapTable("/subjects", "Учебные предметы",
    new[] { "Код", "Название", "Вид контроля" },
    s => s.GetSubjects(),
    x => new[] { x.SubjectId.ToString(), x.SubjectName, x.ExamType });

MapTable("/subjecthours", "Нагрузка по часам",
    new[] { "Код предмета", "Класс", "Часов в неделю" },
    s => s.GetSubjectHours(),
    h => new[] { h.SubjectId.ToString(), h.GradeLevel.ToString(), h.HoursPerWeek.ToString() });

MapTable("/classes", "Классы",
    new[] { "Код", "Параллель", "Буква", "Классный руководитель", "Учебный год" },
    s => s.GetClasses(),
    c => new[] { c.ClassId.ToString(), c.GradeLevel.ToString(), c.ClassLetter,
                 c.HomeroomTeacherId?.ToString() ?? "", c.AcademicYear });

MapTable("/students", "Учащиеся",
    new[] { "Код", "ФИО", "Дата рождения", "Код класса", "Контакты", "Дата зачисления" },
    s => s.GetStudents(),
    x => new[] { x.StudentId.ToString(), x.FullName, x.BirthDate.ToString("dd.MM.yyyy"),
                 x.CurrentClassId.ToString(), x.ContactInfo ?? "", x.EnrollmentDate.ToString("dd.MM.yyyy") });

MapTable("/lessons", "Уроки",
    new[] { "Код", "Класс", "Предмет", "Учитель", "Дата", "№ урока", "Тема" },
    s => s.GetLessons(),
    l => new[] { l.LessonId.ToString(), l.ClassId.ToString(), l.SubjectId.ToString(),
                 l.TeacherId?.ToString() ?? "", l.LessonDate.ToString("dd.MM.yyyy"),
                 l.LessonNumber.ToString(), l.Topic });

MapTable("/journal", "Журнал",
    new[] { "Код", "Учащийся", "Урок", "Присутствовал", "Причина", "Оценка", "Вид работы" },
    s => s.GetJournal(),
    j => new[] { j.JournalId.ToString(), j.StudentId.ToString(), j.LessonId.ToString(),
                 j.IsPresent ? "Да" : "Нет", j.AbsenceReason ?? "",
                 j.GradeValue?.ToString() ?? "", j.WorkType ?? "" });

// ---------- /searchform1 : состояние формы хранится в КУКИ ----------
app.Use(async (context, next) =>
{
    if (!context.Request.Path.StartsWithSegments("/searchform1"))
    {
        await next.Invoke();   // не наш адрес — передаём дальше по конвейеру
        return;
    }

    SubjectSearchService search = context.RequestServices.GetRequiredService<SubjectSearchService>();
    SearchFormState state;
    List<Subject>? results = null;

    if (context.Request.Query.ContainsKey("exam"))
    {
        // форма отправлена: читаем значения, сохраняем в куки, ищем в БД
        state = SearchFormState.FromQuery(context.Request.Query);
        context.Response.SetObjectCookie("SearchForm1", state);
        results = search.Search(state);
    }
    else
    {
        // страница открыта впервые: заполняем элементы формы данными из куки
        state = context.Request.GetObjectCookie<SearchFormState>("SearchForm1") ?? new SearchFormState();
    }

    context.Response.ContentType = HtmlPages.ContentType;
    await context.Response.WriteAsync(
        HtmlPages.SearchPage("Форма поиска 1", "/searchform1", "куки (Cookies)", state, results));
});

// ---------- /searchform2 : состояние формы хранится в SESSION ----------
app.Use(async (context, next) =>
{
    if (!context.Request.Path.StartsWithSegments("/searchform2"))
    {
        await next.Invoke();
        return;
    }

    SubjectSearchService search = context.RequestServices.GetRequiredService<SubjectSearchService>();
    SearchFormState state;
    List<Subject>? results = null;

    if (context.Request.Query.ContainsKey("exam"))
    {
        state = SearchFormState.FromQuery(context.Request.Query);
        context.Session.SetObject("SearchForm2", state);
        results = search.Search(state);
    }
    else
    {
        // заполняем элементы формы данными из Session
        state = context.Session.GetObject<SearchFormState>("SearchForm2") ?? new SearchFormState();
    }

    context.Response.ContentType = HtmlPages.ContentType;
    await context.Response.WriteAsync(
        HtmlPages.SearchPage("Форма поиска 2", "/searchform2", "сессии (Session)", state, results));
});

// ---------- Всё остальное: Page Not Found ----------
app.Run(async context =>
{
    context.Response.StatusCode = 404;
    context.Response.ContentType = HtmlPages.ContentType;
    await context.Response.WriteAsync(HtmlPages.Message("404", "Page Not Found"));
});

app.Run();

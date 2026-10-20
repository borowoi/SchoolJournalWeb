using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace SchoolJournalWeb.Infrastructure;

public static class StateExtensions
{
    // ----- Session: объект сохраняется как JSON-строка -----
    public static void SetObject<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static T? GetObject<T>(this ISession session, string key)
    {
        string? json = session.GetString(key);
        return json == null ? default : JsonSerializer.Deserialize<T>(json);
    }

    // ----- Куки: тот же JSON, но в куки браузера -----
    public static void SetObjectCookie<T>(this HttpResponse response, string key, T value)
    {
        response.Cookies.Append(key, JsonSerializer.Serialize(value), new CookieOptions
        {
            Expires = DateTimeOffset.Now.AddDays(1),
            HttpOnly = true,
            IsEssential = true
        });
    }

    public static T? GetObjectCookie<T>(this HttpRequest request, string key)
    {
        if (!request.Cookies.TryGetValue(key, out string? json) || string.IsNullOrEmpty(json))
            return default;
        try
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (JsonException)
        {
            return default;
        }
    }
}

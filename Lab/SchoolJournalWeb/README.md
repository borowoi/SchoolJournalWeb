# SchoolJournalWeb

[![Build](https://github.com/borowoi/SchoolJournalWeb/actions/workflows/build.yml/badge.svg)](https://github.com/borowoi/SchoolJournalWeb/actions/workflows/build.yml)

Лабораторная работа №3: обработка HTTP-запросов средствами ASP.NET Core, сохранение состояния, кэширование.
Предметная область: электронный журнал успеваемости школы (MS SQL Server, база данных SchoolJournalLabDB).

## Что реализовано
- Платформа: .NET 10, ASP.NET Core (без MVC), Entity Framework Core.
- Сервис `CachedTablesService`: кэширование 20 записей из каждой таблицы в `IMemoryCache` на 246 секунд.
- Middleware на основе `Use`, `Map`, `Run`: `/info`, страницы таблиц, `/searchform1`, `/searchform2`, ответ 404.
- Сохранение состояния формы: куки (`/searchform1`) и Session (`/searchform2`).
- GitHub Actions: сборка под Ubuntu и Windows при любом push и pull request.

## Запуск
1. Создать БД скриптом из лабораторной работы №1 и заполнить её данными.
2. При необходимости изменить `DefaultConnection` в `SchoolJournalWeb/appsettings.json`.
3. `dotnet run --project SchoolJournalWeb`

# Novskiy Web Application

Пример многослойного веб-приложения на .NET 9.

## Структура решения

- **Novskiy.Domain** — Общая библиотека классов (доменные модели).
- **Novskiy.API** — Сервис предоставления данных (REST API / ASP.NET Core Web API). Использует SQLite.
- **Novskiy.UI** — Клиентское приложение на основе архитектуры MVC / Razor Pages.
- **Novskiy.Blazor** — Клиентское приложение на основе Blazor Interactive Server.
- **Novskiy.Tests** — Проект с модульными тестами (xUnit + NSubstitute).

## Стек технологий

- C#, .NET 9
- ASP.NET Core MVC & Razor Pages
- Blazor Server
- Entity Framework Core, SQLite
- xUnit, NSubstitute
- Serilog (в MVC-части)
- Identity Framework

## Запуск проекта

Проекты `API`, `UI` и `Blazor` спроектированы для совместной работы. `API` должен быть запущен для обеспечения клиентов данными:

1. Запустите API-проект (обычно на `https://localhost:7002`).
2. Запустите UI или Blazor проект для взаимодействия с API.

# CrossApp 
Наскрізний проєкт з крос-платформного програмування. 
Предметна область: Бібліотека. Сутності: Book, BookCopy, Reader, Loan. 
Призначення: облік видач примірників книг читачам. 
# CrossApp

# Структура рішення (Solution Structure)
```text
CrossApp/ 
├── CrossApp.sln 
├── README.md 
├── .gitignore 
└── src/ 
    ├── Core/ 
    │   ├── Core.csproj 
    │   ├── EnvironmentInfo.cs       
    └── Cli/ 
        ├── Cli.csproj                  
        └── Program.cs
```

# Запуск 
dotnet build 
dotnet run --project src/Cli 
# Публікація (Publish) framework-dependent:
dotnet publish src/Cli -c Release -r win-x64 --self-contained false
# Публікація (Publish) self-contained:
dotnet publish src/Cli -c Release -r win-x64 --self-contained true
# Середовище 
.NET SDK 10.0, Windows 11 x64

RID         Режим                   Розмір publish      Потрібен runtime
win-x64     self-contained          ~76 МБ              ні 
win-x64     framework-dependent     ~195 МБ             так (.NET 10) - встановлений 

# Режими публікації
Framework-dependent: Публікація містить лише безпосередньо код застосунку та його залежності, але не містить .NET Runtime. Займає дуже мало місця, проте для її запуску на комп’ютері користувача має бути обов'язково встановлений .NET Runtime відповідної версії (.NET 10).

Self-contained: Публікація містить і код застосунку, і повне середовище виконання .NET Runtime для визначеної платформи (RID). Такий застосунок може працювати на "чистому" комп’ютері без встановленого .NET, але папка з публікацією займає значно більше місця на диску.

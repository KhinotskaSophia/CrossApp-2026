# CrossApp 
Наскрізний проєкт з крос-платформного програмування. 
Предметна область: Бібліотека. Сутності: Book, BookCopy, Reader, Loan. 
Призначення: облік видач примірників книг читачам. 

# Структура рішення (Solution Structure)
```text
CrossApp/
├── CrossApp.sln
├── README.md
├── .gitignore
├── data/
│   └── sample.csv
└── src/
    ├── Core/
    │   ├── Core.csproj
    │   ├── EnvironmentInfo.cs
    │   ├── Domain/
    │   │   ├── BookCopy.cs
    │   │   └── Loan.cs
    │   ├── Dto/
    │   │   ├── BookCopyDto.cs
    │   │   ├── BookDto.cs
    │   │   ├── ImportResult.cs
    │   │   ├── LoanDto.cs
    │   │   └── ReaderDto.cs
    │   └── Import/
    │       └── BookCsvImporter.cs
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
win-x64     framework-dependent     ~195 KБ             так (.NET 10) - встановлений 

# Режими публікації
Framework-dependent: Публікація містить лише безпосередньо код застосунку та його залежності, але не містить .NET Runtime. Займає дуже мало місця, проте для її запуску на комп’ютері користувача має бути обов'язково встановлений .NET Runtime відповідної версії (.NET 10).

Self-contained: Публікація містить і код застосунку, і повне середовище виконання .NET Runtime для визначеної платформи (RID). Такий застосунок може працювати на "чистому" комп’ютері без встановленого .NET, але папка з публікацією займає значно більше місця на диску.

## Лабораторна робота 4: Доменна модель, інваріанти та інкапсуляція

### Реалізовані інваріанти

1. **Обов'язковість ідентифікаторів та текстових полів (`ArgumentException`)**:
   - При створенні `BookCopy`: поля `Id`, `Isbn` та `Title` не можуть бути порожніми або складеними лише з пробілів (`BookCopy.Create`).
   - При відкритті видачі `Loan`: поля `Id` та `ReaderId` є обов'язковими (`Loan.Open`).

2. **Захист від некоректного стану примірника (`InvalidOperationException`)**:
   - Неможливо видати примірник, який уже має статус виданого (`BookCopy.Issue`).
   - Неможливо оформити повернення примірника, який не був виданий (`BookCopy.Return`).

3. **Коректність хронології операцій (`ArgumentOutOfRangeException`)**:
   - Дата повернення не може передувати даті оформлення видачі (`Loan.Close`: умова `returnedOn < IssuedOn`).

4. **Цілісність процесу видачі (`InvalidOperationException`)**:
   - Неможливо повторно закрити видачу, яка вже була закрита (`Loan.Close`: перевірка `IsClosed`).
   - Неможливо закрити видачу примірником, ідентифікатор якого не збігається з виданим (`Loan.Close`: перевірка `copy.Id == BookCopyId`).

5. **Інкапсуляція стану та фабричні методи**:
   - Публічні сетери відсутні, зміна стану здійснюється лише методами `Issue()`, `Return()` та `Close()`.
   - Конструктори сутностей закриті (`private`), створення можливе лише через фабричні методи `BookCopy.Create`, `Loan.Open`, або через мапінг відновлення `FromDto`.
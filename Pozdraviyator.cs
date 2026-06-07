using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

class BirthdayPerson
{
    public string Name { get; set; }
    public DateTime Birthday { get; set; }
}

class BirthdayManager
{
    private List<BirthdayPerson> people = new List<BirthdayPerson>();
    private const string FileName = "birthdays.json";

    public void Add(string name, DateTime birthday)
    {
        people.Add(new BirthdayPerson { Name = name, Birthday = birthday });
    }

    public void ShowAll()
    {
        if (people.Count == 0)
        {
            Console.WriteLine("Список пуст.");
            return;
        }

        var sorted = people.OrderBy(p => p.Birthday.Month).ThenBy(p => p.Birthday.Day);

        int i = 1;
        foreach (var p in sorted)
        {
            if (IsToday(p))
                Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"{i}. {p.Name} — {p.Birthday:dd.MM}");
            Console.ResetColor();
            i++;
        }
    }

    public void ShowUpcoming()
    {
        if (people.Count == 0)
        {
            Console.WriteLine("Список пуст.");
            return;
        }

        var upcoming = people
            .Select(p => new { Person = p, Days = DaysToBirthday(p) })
            .OrderBy(x => x.Days)
            .ToList();

        Console.WriteLine("Ближайшие дни рождения:");

        foreach (var x in upcoming)
        {
            if (x.Days == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  {x.Person.Name} — сегодня!");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"  {x.Person.Name} — через {x.Days} дн. ({x.Person.Birthday:dd.MM})");
            }
        }
    }

    public void Delete(int index)
    {
        if (index < 0 || index >= people.Count)
        {
            Console.WriteLine("Неверный номер.");
            return;
        }
        people.RemoveAt(index);
        Console.WriteLine("Удалено.");
    }

    public void Edit(int index, string name, DateTime birthday)
    {
        if (index < 0 || index >= people.Count)
        {
            Console.WriteLine("Неверный номер.");
            return;
        }
        people[index].Name = name;
        people[index].Birthday = birthday;
        Console.WriteLine("Изменено.");
    }

    public int Count => people.Count;

    public BirthdayPerson Get(int index)
    {
        return people[index];
    }

    public void Save()
    {
        try
        {
            string json = JsonSerializer.Serialize(people);
            File.WriteAllText(FileName, json);
        }
        catch
        {
            Console.WriteLine("Ошибка сохранения.");
        }
    }

    public void Load()
    {
        if (!File.Exists(FileName))
            return;

        try
        {
            string json = File.ReadAllText(FileName);
            people = JsonSerializer.Deserialize<List<BirthdayPerson>>(json);
        }
        catch
        {
            Console.WriteLine("Ошибка загрузки.");
        }
    }

    private bool IsToday(BirthdayPerson p)
    {
        var today = DateTime.Today;
        return p.Birthday.Day == today.Day && p.Birthday.Month == today.Month;
    }

    private int DaysToBirthday(BirthdayPerson p)
    {
        var today = DateTime.Today;
        var next = new DateTime(today.Year, p.Birthday.Month, p.Birthday.Day);

        if (next < today)
            next = next.AddYears(1);

        return (next - today).Days;
    }
}

class Program
{
    static BirthdayManager manager = new BirthdayManager();

    static void Main()
    {
        manager.Load();

        Console.WriteLine("=== Поздравлятор ===\n");
        manager.ShowUpcoming();

        while (true)
        {
            Console.WriteLine("\n1 — Все ДР");
            Console.WriteLine("2 — Ближайшие");
            Console.WriteLine("3 — Добавить");
            Console.WriteLine("4 — Удалить");
            Console.WriteLine("5 — Изменить");
            Console.WriteLine("6 — Сохранить");
            Console.WriteLine("7 — Загрузить");
            Console.WriteLine("0 — Выход");
            Console.Write("> ");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1": manager.ShowAll(); break;
                case "2": manager.ShowUpcoming(); break;
                case "3": AddPerson(); break;
                case "4": DeletePerson(); break;
                case "5": EditPerson(); break;
                case "6": manager.Save(); Console.WriteLine("Сохранено."); break;
                case "7": manager.Load(); Console.WriteLine("Загружено."); break;
                case "0": manager.Save(); return;
                default: Console.WriteLine("Неизвестная команда."); break;
            }
        }
    }

    static void AddPerson()
    {
        Console.Write("Имя: ");
        string name = Console.ReadLine();

        Console.Write("Дата (дд.мм.гггг): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
        {
            Console.WriteLine("Неверная дата.");
            return;
        }

        manager.Add(name, date);
        Console.WriteLine("Добавлено.");
    }

    static void DeletePerson()
    {
        manager.ShowAll();
        if (manager.Count == 0) return;

        Console.Write("Номер для удаления: ");
        if (!int.TryParse(Console.ReadLine(), out int num))
        {
            Console.WriteLine("Нужно ввести число.");
            return;
        }

        manager.Delete(num - 1);
    }

    static void EditPerson()
    {
        manager.ShowAll();
        if (manager.Count == 0) return;

        Console.Write("Номер для изменения: ");
        if (!int.TryParse(Console.ReadLine(), out int num))
        {
            Console.WriteLine("Нужно ввести число.");
            return;
        }

        int index = num - 1;
        if (index < 0 || index >= manager.Count)
        {
            Console.WriteLine("Неверный номер.");
            return;
        }

        Console.Write("Новое имя: ");
        string name = Console.ReadLine();

        Console.Write("Новая дата (дд.мм.гггг): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
        {
            Console.WriteLine("Неверная дата.");
            return;
        }

        manager.Edit(index, name, date);
    }
}

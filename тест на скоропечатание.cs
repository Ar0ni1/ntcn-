using Newtonsoft.Json;

class Record
{
    public string Name { get; set; }
    public int CharactersPerMinute { get; set; }
    public double CharactersPerSecond { get; set; }
}

 class RecordsTable
{
    public List<Record> Records { get; set; }

    public RecordsTable()
    {
        Records = new List<Record>();
    }

    public void AddRecord(string name, int charactersPerMinute, double charactersPerSecond)
    {
        Records.Add(new Record
        {
            Name = name,
            CharactersPerMinute = charactersPerMinute,
            CharactersPerSecond = charactersPerSecond
        });
    }

    public void SaveRecords(string fileName)
    {
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(fileName, json);
    }

    public static RecordsTable LoadRecords(string fileName)
    {
        if (File.Exists(fileName))
        {
            string json = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<RecordsTable>(json);
        }
        return new RecordsTable();
    }
}

class TypingSpeedTest
{
    private const string RecordsFileName = "records.json"; // Имя файла для хранения рекордов

    static void Main()
    {
        bool exitProgram = false;

        do
        {
            RecordsTable recordsTable = RecordsTable.LoadRecords(RecordsFileName);

            Console.WriteLine("Добро пожаловать в тест на скоропечатание!");
            Console.Write("Пожалуйста, введите ваше имя: ");
            string name = Console.ReadLine();

            RunTypingTest(name, recordsTable);

            Console.Write("\nХотите пройти тест еще раз? (+/-): ");
            char response = Console.ReadKey().KeyChar;

            if (response != '+')
                exitProgram = true;

            Console.Clear(); // Очистить консоль для следующего запуска или завершения программы
        } while (!exitProgram);
    }

    static void RunTypingTest(string name, RecordsTable recordsTable)
    {
        string textToType = @"Быстрый корабль прошел через бурю, оставив за собой лишь слабое эхо своего пути в бескрайнем океане.";

        Console.WriteLine("\nПечатайте следующий текст:");
        Console.WriteLine(textToType);
        Console.WriteLine("\nНачинайте печатать текст:");
        Console.ForegroundColor = ConsoleColor.White;

        int currentIndex = 0;
        int correctChars = 0;
        int incorrectChars = 0;
        bool inputFinished = false;

        DateTime testStartTime = DateTime.Now;

        while (!inputFinished)
        {
            if ((DateTime.Now - testStartTime).TotalSeconds >= 60 || currentIndex >= textToType.Length)
            {
                inputFinished = true;
                break;
            }

            Console.SetCursorPosition(currentIndex, Console.CursorTop);

            Console.ForegroundColor = Console.BackgroundColor;
            string userInput = Console.ReadKey(true).KeyChar.ToString();

            if (userInput == textToType.Substring(currentIndex, 1))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(userInput);
                currentIndex++;
                correctChars++;
            }
            else if (userInput == "\r") // Если пользователь нажал Enter
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n\nВы решили прервать тест. До свидания!");
                return;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(textToType.Substring(currentIndex, 1));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(textToType.Substring(currentIndex + 1, 1));
                Console.ForegroundColor = ConsoleColor.White;
                currentIndex++;
                incorrectChars++;
            }
        }

        double totalTimeInSeconds = (DateTime.Now - testStartTime).TotalSeconds;
        int charactersTyped = correctChars + incorrectChars;
        int charactersPerMinute = (int)(charactersTyped / (totalTimeInSeconds / 60.0));
        double charactersPerSecond = charactersTyped / totalTimeInSeconds;

        recordsTable.AddRecord(name, charactersPerMinute, charactersPerSecond);
        recordsTable.SaveRecords(RecordsFileName);

        Console.WriteLine($"\n\nПоздравляем, {name}!");
        Console.WriteLine($"Ваша скорость печати: {charactersPerMinute} символов в минуту");
        Console.WriteLine($"Символов в секунду: {charactersPerSecond:F2}");

        Console.WriteLine("\nТаблица рекордов:");
        foreach (var record in recordsTable.Records)
        {
            Console.WriteLine($"Имя: {record.Name}, Символов в минуту: {record.CharactersPerMinute}, Символов в секунду: {record.CharactersPerSecond:F2}");
        }
    }
}

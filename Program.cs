using System;
using System.IO;
using System.Media; // Dùng để phát âm thanh
using System.Threading.Tasks; // Dùng để chạy âm thanh trên luồng khác
using NAudio.Wave;

class MathChallengeGame
{
    static Question[,] gameData = new Question[5, 10]; // Mang 2D luu 5 vong choi, moi vong 10 cau hoi
    static int currentRound = 0; // Vong choi hien tai
    static int currentQuestionIndex = 0; // Chi so cau hoi hien tai trong vong
    static int score = 0; // Diem so tong
    static Random random = new Random();

    static void Main()
    {
        DisplayTitle();

        while (true)
        {
            DisplayMenu();
            string choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        AskQuestion();
                        break;
                    case "2":
                        ShowHistory();
                        break;
                    case "3":
                        SaveGame();
                        break;
                    case "4":
                        LoadGame();
                        break;
                    case "5":
                        PrintMessage("Cam on ban da choi! Tam biet.", true);
                        return;
                    default:
                        PrintMessage("Lua chon khong hop le. Vui long chon lai.", true);
                        break;
                }
            }
            catch (Exception ex)
            {
                PrintMessage($"Da xay ra loi: {ex.Message}", true);
            }
        }
    }

    static void DisplayTitle()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════╗");
        Console.WriteLine("║            *** GIAI TOAN ***       ║");
        Console.WriteLine("╚════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine("        Hay san sang chinh phuc toan hoc!");
        Console.WriteLine();
    }

    static void DisplayMenu()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n╔═══════════════════════════════════╗");
        Console.WriteLine("║               MENU                ║");
        Console.WriteLine("╠═══════════════════════════════════╣");
        Console.WriteLine("║  1. Tra loi cau hoi               ║");
        Console.WriteLine("║  2. Xem lich su cau hoi           ║");
        Console.WriteLine("║  3. Luu game                      ║");
        Console.WriteLine("║  4. Nap game                      ║");
        Console.WriteLine("║  5. Thoat                         ║");
        Console.WriteLine("╚═══════════════════════════════════╝");
        Console.ResetColor();
        Console.Write("Chon mot tuy chon: ");
    }

    static void AskQuestion()
    {
        if (currentRound >= gameData.GetLength(0))
        {
            PrintMessage("Ban da hoan thanh tat ca cac vong choi.", true);
            return;
        }

        if (currentQuestionIndex >= gameData.GetLength(1))
        {
            PrintMessage($"Vong {currentRound + 1} da hoan thanh. Hay chuyen sang vong tiep theo.", true);
            currentRound++;
            currentQuestionIndex = 0;
            return;
        }

        int a = random.Next(1, 11);
        int b = random.Next(1, 11);
        string[] operators = { "+", "-", "*", "/" };
        string op = operators[random.Next(operators.Length)];
        {
            "+" => a + b,
            "-" => a - b,
            "*" => a * b,
            "/" => b != 0 ? a / b : 0,
            _ => 0
        };

        string question = $"{a} {op} {b}";
        PrintMessage($"Cau hoi: {question}");
        Console.Write("Dap an cua ban: ");

        try
        {
            gameData[currentRound, currentQuestionIndex] = new Question(question, playerAnswer, correctAnswer);

            if (playerAnswer == correctAnswer)
            {
                score += 10;
                PrintMessage("Chinh xac! Ban duoc cong 10 diem.", true);
            }
            else
            {
                PrintMessage($"Sai! Dap an dung la {correctAnswer}.", true);
            }
            currentQuestionIndex++;
        }
        catch (FormatException)
        {
            PrintMessage("Vui long nhap mot so hop le.", true);
        }
    }

    static void ShowHistory()
    {
        Console.WriteLine("\n╔═════════════════ LICH SU ═════════════════╗");
        for (int round = 0; round < gameData.GetLength(0); round++)
        {
            for (int question = 0; question < gameData.GetLength(1); question++)
            {
                var entry = gameData[round, question];
                if (entry != null)
                {
                    string result = entry.IsCorrect ? "Dung" : "Sai";
                    Console.WriteLine($"Vong {round + 1}, Cau {question + 1}: {entry.QuestionText} | Ban tra loi: {entry.PlayerAnswer} | Ket qua: {result}");
                }
            }
        }
        Console.WriteLine($"Diem so hien tai: {score}");
    }

    static void SaveGame()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter("math_game_save.txt"))
            {
                writer.WriteLine(score);
                writer.WriteLine(currentRound);
                writer.WriteLine(currentQuestionIndex);
                for (int round = 0; round < gameData.GetLength(0); round++)
                {
                    for (int question = 0; question < gameData.GetLength(1); question++)
                    {
                        var entry = gameData[round, question];
                        if (entry != null)
                        {
                            writer.WriteLine($"{round},{question},{entry.QuestionText},{entry.PlayerAnswer},{entry.CorrectAnswer}");
                        }
                    }
                }
            }
            PrintMessage("Game da duoc luu thanh cong.", true);
        }
        catch (Exception ex)
        {
            PrintMessage($"Loi khi luu game: {ex.Message}", true);
        }
    }

    static void LoadGame()
    {
        try
        {
            using (StreamReader reader = new StreamReader("math_game_save.txt"))
            {
                score = int.Parse(reader.ReadLine());
                currentRound = int.Parse(reader.ReadLine());
                currentQuestionIndex = int.Parse(reader.ReadLine());

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    int round = int.Parse(parts[0]);
                    int question = int.Parse(parts[1]);
                    gameData[round, question] = new Question(parts[2], int.Parse(parts[3]), int.Parse(parts[4]));
                }
            }
            PrintMessage("Game da duoc nap thanh cong.", true);
        }
        catch (FileNotFoundException)
        {
            PrintMessage("File luu game khong ton tai.", true);
        }
        catch (Exception ex)
        {
            PrintMessage($"Loi khi nap game: {ex.Message}", true);
        }
    }

    static void PrintMessage(string message, bool addSeparator = false)
    {
        Console.WriteLine(message);
        if (addSeparator)
            Console.WriteLine(new string('-', 40));
    }

    static void PlaySound(string filePath)
    {
        {
        }
    
    }
}

class Question
{
    public string QuestionText { get; }
    public int PlayerAnswer { get; }
    public int CorrectAnswer { get; }
    public bool IsCorrect => PlayerAnswer == CorrectAnswer;

    public Question(string questionText, int playerAnswer, int correctAnswer)
    {
        QuestionText = questionText;
        PlayerAnswer = playerAnswer;
        CorrectAnswer = correctAnswer;
    }
}

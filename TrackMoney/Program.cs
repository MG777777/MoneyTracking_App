using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

class TrackingMoney
{
    static List<Transaction> transactions = new List<Transaction>();
    static string dataFilePath = @"C:\MoneyTracking\transactions.txt"; // File path where the file is saved
    static CultureInfo swedishCulture = new CultureInfo("sv-SE"); // To display amount with Swedish currency
    static decimal currentAccount;
    static void Main(string[] args)
    {
        string directoryPath = Path.GetDirectoryName(dataFilePath); // Check if the directory exists, and if not, create it
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        LoadTransactions();
        Console.ForegroundColor = ConsoleColor.Magenta;
        string str = "--------Welcome To TrackMoney--------\n";
        Console.SetCursorPosition((Console.WindowWidth - str.Length) / 2, Console.CursorTop);
        Console.WriteLine(str);
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Please enter your currently amount on your account:");
        Console.ResetColor();
        decimal.TryParse(Console.ReadLine(), out currentAccount);

        if (currentAccount == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Plaese enter your currently amount on your account");
            Console.ResetColor();
        }
        else if (currentAccount >= 1)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("You have currently" + " " + currentAccount.ToString("C", swedishCulture) + " " + "on your account\n");
            Console.ResetColor();
        }
        while (true)// Here we go, this to make choice with swich statements.
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Please chice the number to follow steps | to Exit before you continue please enter exit \n");
            Console.ResetColor();
            Console.ForegroundColor= ConsoleColor.Red;
            Console.WriteLine("Attention: When you choose number 9 the app will be exit and save the file to folder C:\\MoneyTracking.\n");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("1. Add Income");
            Console.WriteLine("2. Add Expense");
            Console.WriteLine("3. Edit Transaction");
            Console.WriteLine("4. Remove Transaction");
            Console.WriteLine("5. Display Transactions");
            Console.WriteLine("6. Display Expenses");
            Console.WriteLine("7. Display Incomes");
            Console.WriteLine("8. Sort Transactions");
            Console.WriteLine("9. Save and Exit");
            Console.ResetColor();

            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();
            if (choice.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                SaveTransactions();
                break;
            }
            int choiceNumber;
            if (!int.TryParse(choice, out choiceNumber))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid choice. Please enter a number or 'exit'.");
                Console.ResetColor();
                continue;
            }
            switch (choiceNumber)
            {
                case 1:
                    AddTransaction(false);
                    break;
                case 2:
                    AddTransaction(true);
                    break;
                case 3:
                    EditTransaction();
                    break;
                case 4:
                    RemoveTransaction();
                    break;
                case 5:
                    DisplayTransactions(transactions);
                    break;
                case 6:
                    DisplayTransactions(transactions.Where(t => t.IsExpense).ToList());
                    break;
                case 7:
                    DisplayTransactions(transactions.Where(t => !t.IsExpense).ToList());
                    break;
                case 8:
                    SortTransactions();
                    break;
                case 9:
                    SaveTransactions();
                    return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice. Please try again.");
                    Console.ResetColor();
                    break;
            }
        }
    }
    static void AddTransaction(bool isExpense) // To add transactions to saved file
    {
        Console.Write("Enter title: ");
        string title = Console.ReadLine();

        Console.Write("Enter amount: ");
        decimal amount = decimal.Parse(Console.ReadLine());

        Console.Write("Enter month: ");
        int month = int.Parse(Console.ReadLine());
        transactions.Add(new Transaction
        {
            Title = title,
            Amount = amount,
            Month = month,
            IsExpense = isExpense
        });
        Console.ForegroundColor= ConsoleColor.Green;
        Console.WriteLine("Transaction added successfully.");
        Console.ResetColor();
    }
    static void EditTransaction() //To Edit transactions
    {
        DisplayTransactions(transactions);

        Console.Write("Enter the index of the transaction to edit: ");
        int index = int.Parse(Console.ReadLine());

        if (index >= 0 && index < transactions.Count)
        {
            Transaction transaction = transactions[index];

            Console.Write("Enter new title: ");
            transaction.Title = Console.ReadLine();

            Console.Write("Enter new amount: ");
            transaction.Amount = decimal.Parse(Console.ReadLine());

            Console.Write("Enter new month: ");
            transaction.Month = int.Parse(Console.ReadLine());

            Console.WriteLine("Transaction edited successfully.");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid index.");
            Console.ResetColor();
        }
    }
    static void RemoveTransaction() // To Remove transactions
    {
        DisplayTransactions(transactions);

        Console.Write("Enter the index of the transaction to remove: ");
        int index = int.Parse(Console.ReadLine());

        if (index >= 0 && index < transactions.Count)
        {
            transactions.RemoveAt(index);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Transaction removed successfully.");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid index.");
            Console.ResetColor();
        }
    }
    static void DisplayTransactions(List<Transaction> transactionsToDisplay) // Display transactions 
    {
        Console.WriteLine("\nTransactions:");
        Console.WriteLine("Index".PadRight(10) + "Title".PadRight(15) + "type".PadRight(10) + "Amount".PadRight(20) + "Month ".PadRight(10));
        Console.WriteLine("-----".PadRight(10) + "-----".PadRight(15) + "----".PadRight(10) + "------".PadRight(20) + "-----".PadRight(10));
        for (int i = 0; i < transactionsToDisplay.Count; i++)
        {
            Transaction transaction = transactionsToDisplay[i];
            string type = transaction.IsExpense ? "Expense" : "Income";
            Console.Write($"{i.ToString().PadRight(7)} {transaction.Title.PadRight(15)} {type.PadRight(10)} {transaction.Amount.ToString("C", swedishCulture).PadRight(20)} {transaction.Month.ToString().PadRight(10)}\n");
        }
        Console.WriteLine();
    }
    static void SortTransactions() // To sort transactions also with switch statements to make a specific sorting choice
    {
        Console.WriteLine("Sort By:");
        Console.WriteLine("1. Title");
        Console.WriteLine("2. Amount");
        Console.WriteLine("3. Month");

        Console.Write("Enter your choice: ");
        int sortChoice = int.Parse(Console.ReadLine());

        List<Transaction> sortedTransactions = new List<Transaction>();

        switch (sortChoice)
        {
            case 1:
                sortedTransactions = transactions.OrderBy(t => t.Title).ToList();
                break;
            case 2:
                sortedTransactions = transactions.OrderBy(t => t.Amount).ToList();
                break;
            case 3:
                sortedTransactions = transactions.OrderBy(t => t.Month).ToList();
                break;
            default:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid choice.");
                Console.ResetColor();
                return;
        }
        Console.WriteLine("Sort Order:");
        Console.WriteLine("1. Ascending");
        Console.WriteLine("2. Descending");

        Console.Write("Enter your choice: ");
        int sortOrderChoice = int.Parse(Console.ReadLine());

        if (sortOrderChoice == 2)
        {
            sortedTransactions.Reverse();
        }

        DisplayTransactions(sortedTransactions);
    }
    static void SaveTransactions() // Save transactions to the file
    {
        using (StreamWriter writer = new StreamWriter(dataFilePath)) //To write a table in text file i used (writer instead console)
        {
            writer.WriteLine("Title".PadRight(15) + "type".PadRight(10) + "Amount".PadRight(20) + "Month ".PadRight(10));
            writer.WriteLine("-----".PadRight(15) + "-----".PadRight(10) + "------".PadRight(20) + "------".PadRight(10));
            foreach (Transaction transaction in transactions)
            {
                string type = transaction.IsExpense ? "Expense" : "Income";
                writer.WriteLine(transaction.Title.PadRight(15) + type.PadRight(10) + transaction.Amount.ToString("C", swedishCulture).PadRight(20) + transaction.Month.ToString().PadRight(10));
                
            }
            writer.WriteLine("\n--------------------------------------------------------------------------------------------");
            writer.WriteLine("\nYou have currently" + " " + currentAccount.ToString("C", swedishCulture) + " " + "on your account");
        }
        Console.WriteLine("Transactions saved to file.");
    }
    static void LoadTransactions()
    {
        if (File.Exists(dataFilePath))
        {
            transactions.Clear();
            using (StreamReader reader = new StreamReader(dataFilePath))
            {
                string line;
                bool isFirstLine = true;
                while ((line = reader.ReadLine()) != null)
                {
                    if (isFirstLine)
                    {
                        string[] parts = line.Split(':');
                        if (parts.Length == 2 && decimal.TryParse(parts[1], NumberStyles.Currency, swedishCulture, out decimal amount))
                        {
                            currentAccount = amount;
                        }
                        isFirstLine = false;
                    }
                    else
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length == 4 &&
                            decimal.TryParse(parts[1], NumberStyles.Currency, swedishCulture, out decimal amount) &&
                            int.TryParse(parts[2], out int month) &&
                            bool.TryParse(parts[3], out bool isExpense))
                        {
                            transactions.Add(new Transaction
                            {
                                Title = parts[0],
                                Amount = amount,
                                Month = month,
                                IsExpense = isExpense
                            });
                        }
                    }
                }
            }
            Console.WriteLine("Transactions loaded from file.");
        }
    }
}
class Transaction
{

    public string Title { get; set; }
    public decimal Amount { get; set; }
    public int Month { get; set; }
    public bool IsExpense { get; set; }
}

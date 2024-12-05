using System.Net.Mime;
using Microsoft.Data.Sqlite;

namespace BookBase.raynna.application;

public class MainInterface {


    public static bool CanCreate(string newUsername) {
        SqliteConnection connection = Sql.GetConnection();
        
        string query = "SELECT Username, Password, privileges, Name FROM users";

        using (var command = connection.CreateCommand()) {
            command.CommandText = query;

            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    string username = reader.GetString(1);
                    if (username.ToLower().Equals(newUsername.ToLower())) {
                        MessageBox.show($"Username {newUsername} already exists");
                        return false;
                    }
                }
            }
        }
        return true;
    }
        public static void Welcome() {
        Console.Clear();
        Console.WriteLine("Welcome to the bookstore!");
        Console.WriteLine("\nHere are the available books:");

        SqliteConnection connection = Sql.GetConnection();
        
        string query = "SELECT Id, Title, Lent FROM Books";
        var books = new List<(int Id, string Title, bool Lent)>();

        using (var command = connection.CreateCommand()) {
            command.CommandText = query;

            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    int id = reader.GetInt32(0);
                    string title = reader.GetString(1);
                    bool lent = reader.GetInt32(2) == 1;
                    books.Add((id, title, lent));
                    
                    Console.WriteLine($"[{id}] {title} (Lent: {(lent ? "Yes" : "No")})");
                }
            }
        }

        Console.WriteLine("\nPress [key] to view book from list above");
        Console.WriteLine("Press [x] to add book");

    char key = Console.ReadKey().KeyChar;
    Console.Clear();
    if (key == 'x') {
        Console.Write("What is the title of the book you want to add?");
        string? title = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(title)) {
            Book book = new Book(title, false);
            Sql.StoreBook(book);
            Console.Clear();
            Console.WriteLine($"Added book with title: {title}");
            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
            Welcome();
        } else {
            Console.Clear();
            Console.WriteLine("\nPlease enter a valid title");
            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
            Welcome();
        }
        
    }
    if (char.IsDigit(key)) {
        int selectedId = int.Parse(key.ToString());
        var selectedBook = books.FirstOrDefault(book => book.Id == selectedId);

        if (selectedBook != default) {
            Console.WriteLine("Now viewing book:");
            Console.WriteLine($"Title: {selectedBook.Title}");
            Console.WriteLine($"Lent: {(selectedBook.Lent ? "Yes" : "No")}");
            Console.WriteLine("\nPress [1] to change status\nPress [2] to go back to the main menu.");

            key = Console.ReadKey().KeyChar;
            if (key == '1') {
                selectedBook.Lent = !selectedBook.Lent; // Toggle the Lent status
                Sql.UpdateBook(new Book {
                    Title = selectedBook.Title,
                    Lent = selectedBook.Lent
                });
                Console.Clear();
                Console.WriteLine($"Status updated. Book '{selectedBook.Title}' is now {(selectedBook.Lent ? "Lent" : "Available")}.");
                Console.WriteLine("\nPress any key to continue");
                Console.ReadKey();
                Welcome();
            } else if (key == '2') {
                Welcome();
            }
        } else {
            Console.WriteLine("\nInvalid selection. Returning to main menu...");
        }
    }

    Console.WriteLine("Closing program...");
    Sql.Disconnect();
    Console.WriteLine("\nPress any key to exit...");
    Console.ReadKey();
}



}
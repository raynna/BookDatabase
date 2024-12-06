using System.Net;
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
                    var hasSameUsername = username.ToLower().Equals(newUsername.ToLower());
                    if (hasSameUsername) {
                        return false;
                    }
                }
            }
        }

        return true;
    }


    public static void IsValidRemove(int Id, string? Title, bool Lent) {
        try {
            List<string> booksToRemove = new List<string>();

            using (SqliteConnection connection = Sql.GetConnection()) {
                string query = "SELECT Id, Title, Lent FROM Books";

                using (var command = connection.CreateCommand()) {
                    command.CommandText = query;

                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            int id = reader.GetInt32(0);
                            string title = reader.GetString(1);
                            bool lent = reader.GetBoolean(2);

                            Console.WriteLine($"Checking conditions for book: {title}");
                            var containsId = id.Equals(Id);
                            var containsTitle = title.ToLower().Contains(Title.ToLower());
                            var containsLent = lent == Lent && Lent;
                            Console.WriteLine(
                                $"same Id? {containsId}, containsTitle? {title.ToLower()}/{Title.ToLower()}({containsTitle}), containsLent? ({containsLent})");
                            if (containsId || containsTitle || containsLent) {
                                booksToRemove.Add(title);
                            }
                        }
                    }
                }

                foreach (string bookToRemove in booksToRemove) {
                    try {
                        Sql.RemoveBook(connection, bookToRemove);
                        Console.WriteLine($"Removed book from database {bookToRemove}");
                    } catch (InvalidOperationException ex) {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        } catch (Exception e) {
            Console.WriteLine(e);
        }
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
        Console.WriteLine("Press [v] to add book");
        Console.WriteLine("Press [x] to remove a book.");

        char key = Console.ReadKey().KeyChar;
        Console.Clear();
        if (key == 'v') {
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

        if (key == 'x') {
            Console.WriteLine("Pick an option: ");
            Console.WriteLine("[1] Remove by id");
            Console.WriteLine("[2] Remove by title");
            key = Console.ReadKey().KeyChar;
            if (char.IsDigit(key)) {
                if (key == '1') {
                    Console.Clear();
                    Console.Write("Enter id of book you want to remove: ");
                    int id = int.Parse(Console.ReadLine());
                    IsValidRemove(id, "", false);
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                    Welcome();
                }

                if (key == '2') {
                    Console.Clear();
                    Console.Write("Enter title of book you want to remove: ");
                    string? title = Console.ReadLine();
                    IsValidRemove(-1, title, false);
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                    Welcome();
                }
                
            }
        }

        if (char.IsDigit(key)) {
            int selectedId = int.Parse(key.ToString());
            var selectedBook = books.FirstOrDefault(book => book.Id == selectedId);

            if (selectedBook != default) {
                Console.WriteLine("Now viewing book:");
                Console.WriteLine($"Title: {selectedBook.Title}");
                Console.WriteLine($"Lent: {(selectedBook.Lent ? "Yes" : "No")}");
                Console.WriteLine("\nPress [1] to change status\nPress [2] to delete book\nPress [3] to go back to main menu.");

                key = Console.ReadKey().KeyChar;
                if (key == '1') {
                    selectedBook.Lent = !selectedBook.Lent; // Toggle the Lent status
                    Sql.UpdateBook(new Book {
                        Title = selectedBook.Title,
                        Lent = selectedBook.Lent
                    });
                    Console.Clear();
                    Console.WriteLine(
                        $"Status updated. Book '{selectedBook.Title}' is now {(selectedBook.Lent ? "Lent" : "Available")}.");
                    Console.WriteLine("\nPress any key to continue");
                    Console.ReadKey();
                    Welcome();
                } else if (key == '2') {
                    Console.Clear();
                    Sql.RemoveBook(connection, selectedBook.Title);
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadKey();
                    Welcome();
                } else if (key == '3') {
                    Welcome();
                }
            } else {
                Console.WriteLine("\nInvalid selection. Press any key to go back to main menu.");
                Console.ReadKey();
                Welcome();
            }
        }

        Console.WriteLine("Closing program...");
        Sql.Disconnect();
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
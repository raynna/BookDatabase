using Microsoft.Data.Sqlite;

namespace ConsoleApp;

public class Sql {
    private static SqliteConnection? _connection;

    // Method to initialize and connect to the database
    public static void Connect() {
        string databasePath = "MyDatabase.db"; // Specify the database file name.

        // Connection string
        string connectionString = $"Data Source={databasePath}";

        // Initialize and open the connection
        _connection = new SqliteConnection(connectionString);
        _connection.Open();
        Console.WriteLine("Connected to the database.");

        // Create the required tables
        string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Books (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Lent INTEGER NOT NULL
            );";

        using (var command = _connection.CreateCommand()) {
            command.CommandText = createTableQuery;
            command.ExecuteNonQuery();
            Console.WriteLine("Table 'Books' created successfully!");
            Console.WriteLine("\nPress any key to start program...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    // Method to get the current database connection
    public static SqliteConnection GetConnection() {
        if (_connection == null) {
            throw new InvalidOperationException("Database connection is not initialized. Call Connect() first.");
        }

        return _connection;
    }

    // Method to store a Book object
    public static void StoreBook(Book book) {
        if (_connection == null) {
            throw new InvalidOperationException("Database connection is not initialized. Call Connect() first.");
        }

        string insertQuery = "INSERT INTO Books (Title, Lent) VALUES (@Title, @Lent);";

        using (var command = _connection.CreateCommand()) {
            command.CommandText = insertQuery;
            command.Parameters.AddWithValue("@Title", book.Title);
            command.Parameters.AddWithValue("@Lent", book.Lent ? 1 : 0); // Convert boolean to integer (0 or 1)
            command.ExecuteNonQuery();
            Console.WriteLine($"Book '{book.Title}' (Lent: {book.Lent}) stored successfully.");
        }
    }

    public static void UpdateBook(Book book) {
        if (_connection == null) {
            throw new InvalidOperationException("Database connection is not initialized. Call Connect() first.");
        }

        string updateQuery = "UPDATE Books SET Title = @Title, Lent = @Lent WHERE Title = @Title;";

        using (var command = _connection.CreateCommand()) {
            command.CommandText = updateQuery;

            // Bind parameters
            command.Parameters.AddWithValue("@Title", book.Title);
            command.Parameters.AddWithValue("@Lent", book.Lent ? 1 : 0);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0) {
                Console.WriteLine($"Book '{book.Title}' status updated successfully.");
            } else {
                Console.WriteLine($"No book found. Update failed.");
            }
        }
    }


    // Optional: Cleanup and close the connection when done
    public static void Disconnect() {
        if (_connection != null) {
            _connection.Close();
            Console.WriteLine("Database connection closed.");
        }
    }
}

// Book class definition
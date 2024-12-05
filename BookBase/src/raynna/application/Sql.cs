using Microsoft.Data.Sqlite;

namespace BookBase.raynna.application;

public class Sql {
    
    private static SqliteConnection? _connection;

    public static void Connect() {
        string? projectRoot = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName;
        if (projectRoot != null) {
            string databasePath = Path.Combine(projectRoot, "MyDatabase.db");
            string connectionString = $"Data Source={databasePath}";
            _connection = new SqliteConnection(connectionString);
        }
        _connection?.Open();
        Console.WriteLine("Connected to the database.");
        
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

    public static SqliteConnection GetConnection() {
        if (_connection == null) {
            throw new InvalidOperationException("Database connection is not initialized. Call Connect() first.");
        }

        return _connection;
    }

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

    public static void Disconnect() {
        if (_connection != null) {
            _connection.Close();
            Console.WriteLine("Database connection closed.");
        }
    }
}
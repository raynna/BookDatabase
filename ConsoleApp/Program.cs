
using System.Collections;
using Microsoft.Data.Sqlite;
namespace ConsoleApp {
    
    class Program {
    
        static void Main(string[] args) {
            try {
                Sql.Connect();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            MainInterface.Welcome();
        }
    }
}
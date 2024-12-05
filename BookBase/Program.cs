
using BookBase.raynna.application;
namespace BookBase {
    
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

using BookBase.raynna.application;
namespace BookBase {
    internal abstract class Program {
        private static void Main(string[] args) {
            try {
                Sql.Connect();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            MainInterface.Welcome();
        }
    }
}
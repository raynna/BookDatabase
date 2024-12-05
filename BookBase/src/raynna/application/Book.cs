namespace BookBase.raynna.application;

public class Book {
    public int Id { get; set; }
    public string Title { get; set; }
    public bool Lent { get; set; }

    // Standard Constructor
    public Book(string title, bool lent) {
        Title = title;
        Lent = lent;
    }

    public Book() {
    }
}
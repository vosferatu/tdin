package bookstore.commons;

public class EventHandlers {
    public interface AlterBookEvent {
        void run(String book_title);
    }

    public interface ClickedButton {
        void clicked();
    }
}
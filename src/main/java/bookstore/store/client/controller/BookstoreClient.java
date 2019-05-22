package bookstore.store.client.controller;

import java.rmi.RemoteException;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.concurrent.ConcurrentHashMap;

import bookstore.store.client.gui.ClientWindow;
import bookstore.store.commons.BaseRMI;
import bookstore.store.server.ServerInterface;
import bookstore.store.server.responses.Book;
import bookstore.store.commons.EventHandlers.ClickedButton;
import bookstore.store.commons.EventHandlers.AlterBookEvent;

public class BookstoreClient extends BaseRMI implements ClientInterface {
    private static final String SERVER_NAME = "BookstoreServer";
    private static final int SERVER_PORT = 8005;

    private ServerInterface server_obj;
    private ClientWindow client_gui;

    private ConcurrentHashMap<String, Book> available_books;
    private HashMap<String, Integer> books_order;

    public static void main(String[] args) {
        System.out.println("Starting Bookstore client...");
        try {
            ServerInterface server = (ServerInterface) fetchObject(SERVER_NAME, SERVER_PORT);
            if (server != null) {
                BookstoreClient obj = new BookstoreClient(server);
                System.out.println("Started Bookstore client!");
                obj.startClient();
            }
        } catch (Exception e) {
            System.err.println("Failed to start Bookstore client!\n - " + e.getMessage());
        }
    }

    public BookstoreClient(ServerInterface server) {
        this.server_obj = server;
        this.client_gui = ClientWindow.newClient (
            (AlterBookEvent)(String title) -> this.addBookToOrder(title),
            (AlterBookEvent)(String title) -> this.remBookFromOrder(title),
            (ClickedButton)() -> this.submitOrder(),
            (ClickedButton)() -> this.resetOrder()
        );
        this.available_books = new ConcurrentHashMap<>();
        this.books_order = new HashMap<>();
    }

    private void startClient() {
        // Starts executing the Client GUI thread
        this.client_gui.start();
        try {
            LinkedList<Book> books = this.server_obj.getAllBooks();
            for (Book book : books) {
                this.available_books.put(book.getTitle(), book);
            }
            this.client_gui.setAvailableBooks(this.server_obj.getAllBooks());
        }
        catch (Exception e) {
            System.err.println("Error!\n - " + e.getMessage());
        }
    }

    void addBookToOrder(String title) {
        if (this.available_books.containsKey(title)) {
            Integer new_amount = this.books_order.computeIfPresent(title, (String key, Integer value) -> value+1);
            if (new_amount == null) { //Book Entry not present
                this.books_order.put(title, 1);
            }
            Book book = this.available_books.get(title);
            this.client_gui.addBookUnit(book.getTitle(), book.getPrice());
        }
        else {
            System.err.println("Warning: No book named '" + title + "'");
        }
    }

    void remBookFromOrder(String title) {
        if (this.available_books.containsKey(title) && this.books_order.containsKey(title)) {
            Integer new_amount = this.books_order.computeIfPresent(title, (String key, Integer value) -> value - 1);
            if (new_amount != null) {
                if (new_amount <= 0) {
                    this.books_order.remove(title);
                }
                Book book = this.available_books.get(title);
                this.client_gui.remBookUnit(title, book.getPrice());
            }
        }
    }

    void submitOrder() {
        System.out.println("Submitting order!");
        // TODO: Send order to remote
        
        this.client_gui.clearOrder();
    }

    void resetOrder() {
        this.client_gui.clearOrder();
    }

    @Override
    public void receiveBook(Book new_book) throws RemoteException {

    }
}
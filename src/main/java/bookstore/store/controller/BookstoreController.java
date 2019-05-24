package bookstore.store.controller;

import java.util.HashMap;
import java.util.LinkedList;
import javax.mail.internet.InternetAddress;
import java.util.concurrent.ConcurrentHashMap;

import bookstore.commons.BaseRMI;
import bookstore.server.responses.Book;
import bookstore.server.responses.BookOrder;
import bookstore.server.responses.Request;
import bookstore.server.ServerInterface;
import bookstore.store.gui.ClientPopup;
import bookstore.store.gui.ClientWindow;
import bookstore.commons.EventHandlers.ClickedButton;
import bookstore.commons.EventHandlers.AlterBookEvent;

public class BookstoreController extends BaseRMI {
    private static final String SERVER_NAME = "BookstoreServer";
    private static final int SERVER_PORT = 8005;

    private ServerInterface server_obj;
    private ClientWindow client_gui;

    private ConcurrentHashMap<String, Book> available_books;
    private HashMap<String, Integer> books_order;

    public static void main(String[] args) {
        System.out.println("Starting Bookstore client...");
        try {
            ServerInterface server = (ServerInterface)fetchObject(SERVER_NAME, SERVER_PORT);
            if (server != null) {
                BookstoreController obj = new BookstoreController(server);
                System.out.println("Started Bookstore client!");
                obj.startClient();
            }
        } catch (Exception e) {
            System.err.println("Failed to start Bookstore client!\n - " + e.getMessage());
        }
    }

    public BookstoreController(ServerInterface server) {
        this.server_obj = server;
        this.client_gui = ClientWindow.newClient (
            new AlterBookEvent[] {
                (String title) -> this.addBookToOrder(title),
                (String title) -> this.remBookFromOrder(title)
            },
            new ClickedButton[] {
                () -> this.submitOrder(),
                () -> this.resetOrder(),
                () -> this.finishOrder(),
                () -> this.resetDetails()
            }
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

    void finishOrder() {
        ClientPopup popup = this.client_gui.getPopup();
        popup.clearAllErrors();
        if (detailsValid(popup.getInputName(), popup.getInputAddr(), popup.getInputEmail())) {
            LinkedList<BookOrder> books = new LinkedList<>();
            String name = popup.getInputName(), email = popup.getInputEmail(),
                addr = popup.getInputAddr();
            books_order.forEach((String title, Integer amount) -> {
                Book book = this.available_books.get(title);
                books.add(new BookOrder(book, amount, null, null));
            });
            Request req = Request.fromClientData(name, email, addr, books);
            
            try {
                this.server_obj.putRequest(req);
            } catch (Exception e) {
                System.err.println("Failed to finish order!\n - " + e);
            }
            this.client_gui.clearOrder();
            this.client_gui.getPopup().closeWindow();
        }
    }
    
    private boolean detailsValid(String name, String addr, String email) {
        boolean valid = true;
        ClientPopup popup = this.client_gui.getPopup();
        if (name.trim().length() < 3) {
            popup.setNameError("Name field must have at least 3 characters!");
            valid = false;
        } 
        if (addr.trim().length() < 10) {
            popup.setAddrError("Address field must have at least 10 characters!");
            valid = false;
        }

        boolean valid_email = true;
        try {
            InternetAddress email_addr = new InternetAddress(email);
            email_addr.validate();
        }
        catch (Exception err) {
            valid_email = false;
        }
        if (!valid_email) {
            popup.setEmailError("Provided email is not valid!");
        }
        return valid && valid_email;
    }

    void submitOrder() {
        if (this.books_order.size() > 0) {
            this.client_gui.getPopup().show();
        }
        else {
            this.client_gui.setErrorMessage("At least one book must be added!");
        }
    }

    void resetOrder() {
        this.client_gui.clearOrder();
    }

    void resetDetails() {
        this.client_gui.getPopup().resetDetails();
    }
}
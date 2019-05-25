package bookstore.store.controller;

import java.util.HashMap;
import java.util.LinkedList;
import javax.mail.internet.InternetAddress;

import org.gnome.gtk.Gtk;
import org.gnome.gtk.Widget;
import org.gnome.gtk.Window;

import java.util.concurrent.ConcurrentHashMap;

import bookstore.commons.BaseRMI;
import bookstore.server.responses.Book;
import bookstore.server.responses.BookOrder;
import bookstore.server.responses.Request;
import bookstore.server.ServerInterface;
import bookstore.store.gui.ArrivingBooksWindow;
import bookstore.store.gui.ClientDetailsPopup;
import bookstore.store.gui.OrderCreatorWindow;

public class BookstoreController extends BaseRMI {
    private static final String SERVER_NAME = "BookstoreServer";
    private static final int SERVER_PORT = 8005;
    private ServerInterface server_obj;

    private Window window;
    private Widget[] prev_window;
    private OrderCreatorWindow creator_window;
    private ClientDetailsPopup details_window;
    private ArrivingBooksWindow arriving_window;

    private ConcurrentHashMap<String, Book> books;
    private HashMap<String, Integer> order;

    public static void main(String[] args) {
        System.out.println("Starting Bookstore client...");
        try {
            ServerInterface server = (ServerInterface)fetchObject(SERVER_NAME, SERVER_PORT);
            if (server != null) {
                Gtk.init(new String[] {});
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
        this.books = new ConcurrentHashMap<>();
        this.order = new HashMap<>();

        this.creator_window = OrderCreatorWindow.newWindow (
            (String title) -> this.addBookToOrder(title),
            (String title) -> this.remBookFromOrder(title)
        );
        this.window = this.creator_window.getWindow();
        this.creator_window.connectHandler("SubmitButton", () -> this.submitOrder());
        this.creator_window.connectHandler("ArrivedButton", () -> this.switchToArriving());
        this.creator_window.connectHandler("ResetButton", () -> this.resetOrder());
        this.details_window = ClientDetailsPopup.newWindow();
        this.details_window.connectHandler("FinishButton", () -> this.finishOrder());
        this.details_window.connectHandler("ResetButton", () -> this.resetDetails());
        this.arriving_window = ArrivingBooksWindow.newWindow((String title) -> this.bookAccepted(title));
        this.arriving_window.connectHandler("AcceptAllButton", () -> this.acceptAllBooks());
        this.arriving_window.connectHandler("GoBackButton", () -> this.switchToNormalView());
    }

    private void startClient() {
        Thread gui_thr = new Thread() {
            public void run() {
                Gtk.main();
            }
        };
        gui_thr.start();
        this.window.showAll();

        try {
            LinkedList<Book> books = this.server_obj.getAllBooks();
            for (Book book : books) {
                this.books.put(book.getTitle(), book);
            }
            this.creator_window.setAvailableBooks(this.server_obj.getAllBooks());
        }
        catch (Exception e) {
            System.err.println("Error!\n - " + e.getMessage());
        }
    }

    void addBookToOrder(String title) {
        if (this.books.containsKey(title)) {
            Integer new_amount = this.order.computeIfPresent(title, (String key, Integer value) -> value+1);
            if (new_amount == null) { //Book Entry not present
                this.order.put(title, 1);
            }
            Book book = this.books.get(title);
            this.creator_window.addBookUnit(book.getTitle(), book.getPrice());
        }
        else {
            System.err.println("Warning: No book named '" + title + "'");
        }
    }

    void remBookFromOrder(String title) {
        if (this.books.containsKey(title) && this.order.containsKey(title)) {
            Integer new_amount = this.order.computeIfPresent(title, (String key, Integer value) -> value - 1);
            if (new_amount != null) {
                if (new_amount <= 0) {
                    this.order.remove(title);
                }
                Book book = this.books.get(title);
                this.creator_window.remBookUnit(title, book.getPrice());
            }
        }
    }

    void finishOrder() {
        this.details_window.clearAllErrors();
        String name = this.details_window.getInputName(), email = this.details_window.getInputEmail(),
            addr = this.details_window.getInputAddr();
        if (detailsValid(name, addr, email)) {
            LinkedList<BookOrder> books = new LinkedList<>();
            order.forEach((String title, Integer amount) -> {
                Book book = this.books.get(title);
                books.add(new BookOrder(book, amount, null, null));
            });

            Request req = Request.fromClientData(name, email, addr, books);
            req.assignID();
            try {
                this.server_obj.putRequest(req);
            } catch (Exception e) {
                System.err.println("Failed to finish order!\n - " + e);
            }
            this.creator_window.clearOrder();
            this.details_window.getWindow().hide();
        }
    }
    
    void bookAccepted(String title) {

    }

    void acceptAllBooks() {
        
    }

    private boolean detailsValid(String name, String addr, String email) {
        boolean valid = true;
        if (name.trim().length() < 3) {
            this.details_window.setNameError("Name field must have at least 3 characters!");
            valid = false;
        } 
        if (addr.trim().length() < 10) {
            this.details_window.setAddrError("Address field must have at least 10 characters!");
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
            this.details_window.setEmailError("Provided email is not valid!");
        }
        return valid && valid_email;
    }

    void submitOrder() {
        if (this.order.size() > 0) {
            this.details_window.getWindow().showAll();
        }
        else {
            this.creator_window.setErrorMessage("At least one book must be added!");
        }
    }

    void resetOrder() {
        this.creator_window.clearOrder();
    }

    void resetDetails() {
        this.details_window.resetDetails();
    }

    void switchToNormalView() {
        this.arriving_window.removeAllBooks();
        for (Widget widget : this.window.getChildren()) {
            this.window.remove(widget);
            this.arriving_window.getWindow().add(widget);
        }
        for (Widget widget : this.prev_window)
            this.window.add(widget);
    }

    void switchToArriving() {
        this.prev_window = this.window.getChildren();
        for (Widget widget : this.window.getChildren()) {
            this.window.remove(widget);
        }
        
        try {
            HashMap<String, Integer> books = this.server_obj.getArrivedBooks();
            System.out.println(books.toString());
            this.arriving_window.setArrivingBooks(books);
            for (Widget widget : this.arriving_window.getWindow().getChildren()) {
                this.arriving_window.getWindow().remove(widget);
                this.window.add(widget);
            }
        }
        catch (Exception e) {
            System.err.println("Failed to fetch arriving books from server!\n - " + e);
            e.printStackTrace();
        }
    }
}
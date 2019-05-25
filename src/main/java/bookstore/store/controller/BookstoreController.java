package bookstore.store.controller;

import java.rmi.Remote;
import java.rmi.RemoteException;
import java.util.HashMap;
import java.util.LinkedList;
import javax.mail.internet.InternetAddress;

import org.gnome.gtk.Gtk;
import org.gnome.gtk.Widget;
import org.gnome.gtk.Window;

import java.util.concurrent.ConcurrentHashMap;

import bookstore.commons.BaseRMI;
import bookstore.server.responses.Book;
import bookstore.server.responses.BookRequests;
import bookstore.server.ServerInterface;
import bookstore.store.gui.ArrivingBooksWindow;
import bookstore.store.gui.ClientDetailsPopup;
import bookstore.store.gui.OrderCreatorWindow;

public class BookstoreController extends BaseRMI implements BookstoreInterface {
    private ServerInterface server_obj;

    private boolean on_creator = true;
    private Boolean refreshing = new Boolean(false); //Value irrelevant, only want the mutex ability

    private Window window;
    private Widget[] prev_window;
    private OrderCreatorWindow creator_window;
    private ClientDetailsPopup details_window;
    private ArrivingBooksWindow arriving_window;

    private ConcurrentHashMap<String, Book> books;
    private HashMap<String, Integer> order;

    private LinkedList<BookRequests> book_requests;

    public static void main(String[] args) {
        System.out.println("Starting Bookstore client...");
        try {
            ServerInterface server = (ServerInterface) fetchObject(BS_SERVER_OBJ_NAME, BS_SERVER_OBJ_PORT);
            if (server != null) {
                Gtk.init(new String[] {});
                BookstoreController obj = new BookstoreController(server);
                BookstoreInterface stub = (BookstoreInterface)registerObject((Remote)obj, BS_CLIENT_OBJ_NAME, BS_CLIENT_OBJ_PORT);
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
        this.book_requests = new LinkedList<>();

        this.creator_window = OrderCreatorWindow.newWindow((String title) -> this.addBookToOrder(title),
                (String title) -> this.remBookFromOrder(title));
        this.window = this.creator_window.getWindow();
        this.creator_window.connectHandler("SubmitButton", () -> this.submitOrder());
        this.creator_window.connectHandler("ArrivedButton", () -> this.switchToArriving());
        this.creator_window.connectHandler("ResetButton", () -> this.resetOrder());
        this.details_window = ClientDetailsPopup.newWindow();
        this.details_window.connectHandler("FinishButton", () -> this.finishOrder());
        this.details_window.connectHandler("ResetButton", () -> this.resetDetails());
        this.arriving_window = ArrivingBooksWindow.newWindow((String title) -> this.bookStored(title));
        this.arriving_window.connectHandler("AcceptAllButton", () -> this.storeAllBooks());
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
        } catch (Exception e) {
            System.err.println("Error!\n - " + e.getMessage());
        }
    }

    void addBookToOrder(String title) {
        if (this.books.containsKey(title)) {
            Integer new_amount = this.order.computeIfPresent(title, (String key, Integer value) -> value + 1);
            if (new_amount == null) { // Book Entry not present
                this.order.put(title, 1);
            }
            Book book = this.books.get(title);
            this.creator_window.addBookUnit(book.getTitle(), book.getPrice());
        } else {
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
            try {
                this.server_obj.putRequest(name, email, addr, order);
            } catch (Exception e) {
                System.err.println("Failed to finish order!\n - " + e.getMessage());
                e.printStackTrace();
            }
            this.order.clear();
            this.creator_window.clearOrder();
            this.details_window.getWindow().hide();
        }
    }

    void bookStored(String title) {
        BookRequests reqs = this.getBookRequests(title);
        if (reqs != null) {
            LinkedList<BookRequests> book_reqs = new LinkedList<>();
            book_reqs.add(reqs);
            try {
                this.server_obj.booksStored(book_reqs);
                this.arriving_window.removeArrivingBook(title);
                this.book_requests.remove(reqs);
            } catch (Exception e) {
                System.err.println("Failed to store book '" + title + "' in server!\n - " + e);
            }
        } else {
            System.err.println("No book '" + title + "' in book_requests :o");
        }
    }

    void storeAllBooks() {
        try {
            this.server_obj.booksStored(this.book_requests);
            this.arriving_window.removeAllBooks();
            this.book_requests.clear();
        } catch (Exception e) {
            System.err.println("Failed to store all books in server!\n - " + e);
        }
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
        } catch (Exception err) {
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
        } else {
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
        synchronized (this.refreshing) {
            this.arriving_window.removeAllBooks();
            for (Widget widget : this.window.getChildren()) {
                this.window.remove(widget);
                this.arriving_window.getWindow().add(widget);
            }
            this.on_creator = true;
        }
        try {
            this.creator_window.setAvailableBooks(this.server_obj.getAllBooks());
            for (Widget widget : this.prev_window)
                this.window.add(widget);
        }
        catch (Exception e) {
            System.err.println("Failed to fetch all books from server when switching to normal view!\n - " + e);
        }
    }

    void switchToArriving() {
        this.prev_window = this.window.getChildren();
        for (Widget widget : this.window.getChildren()) {
            this.window.remove(widget);
        }

        synchronized (this.refreshing) {
            this.refreshArrivingWindow();
            this.creator_window.cancelAnimation();
            for (Widget widget : this.arriving_window.getWindow().getChildren()) {
                this.arriving_window.getWindow().remove(widget);
                this.window.add(widget);
            }
            this.on_creator = false;
        }
    }

    private HashMap<String, Integer> toBookAmounts(LinkedList<BookRequests> book_reqs) {
        HashMap<String, Integer> amounts = new HashMap<>();
        for (BookRequests book_req : book_reqs) {
            amounts.put(book_req.getTitle(), book_req.getAmount());
        }

        return amounts;
    }

    private BookRequests getBookRequests(String title) {
        for (BookRequests req : this.book_requests) {
            if (req.getTitle().equals(title)) {
                return req;
            }
        }

        return null;
    }

    private void refreshArrivingWindow() {
        try {
            LinkedList<BookRequests> arrived = this.server_obj.getArrivedBooks();
            synchronized (this.book_requests) {
                synchronized (arrived) {
                    this.book_requests = arrived;
                    this.arriving_window.setArrivingBooks(this.toBookAmounts(arrived));
                }
            } 
        }
        catch (Exception e) {
            System.err.println("Failed to fetch arriving books from server!\n - " + e);
            e.printStackTrace();
        }
    }

    @Override
    public void booksArriving() throws RemoteException {
        synchronized (this.refreshing) {
            if (this.on_creator) {
                this.refreshArrivingWindow();
                this.creator_window.startAnimation();
            }
            else {
                this.arriving_window.removeAllBooks();
                for (Widget widget : this.window.getChildren()) {
                    this.window.remove(widget);
                    this.arriving_window.getWindow().add(widget);
                }
                this.refreshArrivingWindow();
                Widget[] children = this.arriving_window.getWindow().getChildren();
                System.out.println("Children len = " + children.length + ", childs = " + children.toString());
                for (Widget widget : children) {
                    System.out.println("Adding arriving widget!");
                    this.arriving_window.getWindow().remove(widget);
                    this.window.add(widget);
                }
                System.out.println("Finished adding widgets!");
            }
        }
    }
}
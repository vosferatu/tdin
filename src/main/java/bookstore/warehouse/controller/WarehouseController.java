package bookstore.warehouse.controller;

import java.util.HashMap;
import java.util.LinkedList;

import org.gnome.gtk.Gtk;

import bookstore.commons.BaseRMI;
import bookstore.server.ServerInterface;
import bookstore.server.responses.BookOrder;
import bookstore.server.responses.Request;
import bookstore.warehouse.gui.WarehouseWindow;

public class WarehouseController extends BaseRMI {
    private static final String SERVER_NAME = "WarehouseServer";
    private static final int    SERVER_PORT = 8007;
    private static final String BOOKSTORE_NAME = "BookstoreServer";
    private static final int    BOOKSTORE_PORT = 8005;

    private ServerInterface server_obj;
    private ServerInterface bookstore_obj;
    private WarehouseWindow window;

    private LinkedList<Request> reqs;

    public static void main(String[] args) {
        System.out.println("Starting Warehouse client...");
        try {
            ServerInterface server = (ServerInterface)fetchObject(SERVER_NAME, SERVER_PORT);
            ServerInterface bookstore_obj = (ServerInterface)fetchObject(BOOKSTORE_NAME, BOOKSTORE_PORT);
            Gtk.init(new String[] {});
            WarehouseController obj = new WarehouseController(server, bookstore_obj);
            System.out.println("Started Bookstore client!");
            obj.startClient();
        }
        catch (Exception e) {
            System.err.println("Failed to start warehouse client!\n - " + e);
        }
    }

    WarehouseController(ServerInterface server, ServerInterface bs_obj) {
        this.server_obj = server;
        this.bookstore_obj = bs_obj;
        this.window = WarehouseWindow.newWindow((String title) -> this.sendBook(title));
        this.window.connectHandler("RefreshButton", () -> this.refreshInterface());
        this.window.connectHandler("DispatchAllButton", () -> this.sendAllBooks());
    }

    public void startClient() {
        Thread gui_thr = new Thread() {
            public void run() {
                Gtk.main();
            }
        };
        gui_thr.start();
        try {
            this.reqs = server_obj.getWaitingRequests();
            this.window.setOrderList(this.mergeRequests(this.reqs));
            this.window.getWindow().showAll();
        }
        catch (Exception e) {
            System.err.println("Failed to fetch all requests!\n - " + e);
        }
    }

    private HashMap<String, Integer> mergeRequests(LinkedList<Request> reqs) {
        HashMap<String, Integer> books = new HashMap<>();
        synchronized (reqs) {
            for (Request req : reqs) {
                for (BookOrder book : req.getRequestBooks()) {
                    if (book.isWaiting()) {
                        books.compute(book.getTitle(), (String title, Integer amount) -> {
                            if (amount == null) return book.getAmount();
                            else return amount + book.getAmount();
                        });
                    }
                }
            }
        }

        return books;
    }

    void sendBook(String title) {
        LinkedList<Long> req_uuids = new LinkedList<Long>();
        int amount = 10;
        try {
            synchronized (this.reqs) {
                for (Request req : this.reqs) {
                    if (req.hasWaitingBook(title)) {
                        req_uuids.add(req.getID());
                        amount += req.getBookAmount(title);
                    }
                }
            }
            this.server_obj.bookDispatched(title, amount, req_uuids);
            this.bookstore_obj.bookDispatched(title, amount, req_uuids);
            this.window.clearOrder(title);
        }
        catch (Exception e) {
            System.err.println("Failed to send book dispatched msg!\n - " + e);
        }
    }

    void sendAllBooks() {
        LinkedList<Long> req_uuids = new LinkedList<Long>();
        HashMap<String, Integer> books_amount;
        try {
            synchronized (this.reqs) {
                for (Request req : this.reqs) {
                    req_uuids.add(req.getID());
                }
                books_amount = this.mergeRequests(this.reqs);
            }
            this.server_obj.allBooksDispatched(books_amount, req_uuids);
            this.bookstore_obj.allBooksDispatched(books_amount, req_uuids);
            this.window.clearOrders();
        }
        catch (Exception e) {
            System.err.println("Failed to dispatch all books to bookstore!\n - " + e);
        }
    }

    void refreshInterface() {
        synchronized (this.reqs) {
            try {
                this.reqs = server_obj.getWaitingRequests();
                this.window.clearOrders();
                this.window.setOrderList(this.mergeRequests(this.reqs));
            }
            catch (Exception e) {
                System.err.println("Failed to fetch all requests at refresh!\n - " + e);
            }
        }
    }
}
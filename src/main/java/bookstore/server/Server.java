package bookstore.server;

import java.io.IOException;
import java.rmi.Remote;
import java.rmi.RemoteException;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.Map;
import java.util.concurrent.TimeoutException;

import com.rabbitmq.client.Delivery;

import bookstore.commons.BaseRMI;
import bookstore.commons.BaseQueue;
import bookstore.server.responses.Book;
import bookstore.server.responses.BookOrder;
import bookstore.server.responses.BookRequests;
import bookstore.server.responses.Request;
import bookstore.store.PrinterInterface;

public class Server extends BaseRMI implements ServerInterface {
    private static final String PR_OBJ_NAME="BookstorePrinter";
    private static final int PR_OBJ_PORT = 8001;
    private static final String BS_OBJ_NAME="BookstoreServer";
    private static final int    BS_OBJ_PORT=8005;
    private static final String WH_OBJ_NAME="WarehouseServer";
    private static final int    WH_OBJ_PORT=8007;

    private boolean is_bookstore;
    private BaseQueue queue;
    private Database db;
    private PrinterInterface printer;

    public static void main(String[] args) {
        if (args.length == 1) {
            try {
                boolean is_bookstore = args[0].equals("Bookstore");
                Server server = new Server(is_bookstore);
                ServerInterface stub;
                if (is_bookstore) {
                    stub = (ServerInterface)registerObject((Remote) server, BS_OBJ_NAME, BS_OBJ_PORT);
                    server.printer = (PrinterInterface)fetchObject(PR_OBJ_NAME, PR_OBJ_PORT);
                }
                else
                    stub = (ServerInterface)registerObject((Remote) server, WH_OBJ_NAME, WH_OBJ_PORT);

                if (stub != null) {
                    System.out.println(args[0] + " server is up!");
                    System.in.read();
                }
            } catch (Exception e) {
                System.err.println("Bookstore server exception:");
                e.printStackTrace();
            }
        }
    }

    Server(boolean is_bookstore) throws IOException, TimeoutException {
        this.is_bookstore = is_bookstore;
        this.db = new Database(is_bookstore);
        if (is_bookstore) {
            this.queue = new BaseQueue("BookstoreQueue");
        }
        else {
            this.queue = new BaseQueue("BookstoreQueue");
            this.queue.registerCallbacks(
                (String tag, Delivery msg) -> this.newMessageCallback(tag, msg),
                (String tag) -> this.cancelCallback(tag)
            );
        }
    }

    @Override
    public void putRequest(String name, String addr, String email, HashMap<String, Integer> books) throws RemoteException {
        Request new_request = this.createRequest(name, addr, email, books);
        printer.printRequest(new_request);
        HashMap<String, Request> orders = this.db.putRequest(new_request);
        Request unfinished_req = orders.get("unfinished"), finished_req = orders.get("finished");
        if (unfinished_req.getRequestBooks().size() > 0) {
            this.queue.sendObject(unfinished_req);
        }
        if (finished_req.hasBooks()) { // Only send email if a request can be satisfied
            String to = new_request.getEmail();
            long id = new_request.getID();
            EmailDispatcher.sendEmail(to, "Order #" + id + " will be dispatched", finished_req.toEmailString());
        }
    }

    private Request createRequest(String name, String addr, String email, HashMap<String, Integer> books_amount) {
        LinkedList<BookOrder> books_order = new LinkedList<>();
        Map<String, Book> books = this.db.getBooks();

        books_amount.forEach((String title, Integer amount) -> {
            Book book = books.get(title);
            books_order.add(new BookOrder(book, amount, null, null));
        });
        Request req = Request.fromClientData(name, email, addr, books_order);
        req.assignID();
        return req;
    }

    @Override
    public LinkedList<Request> getUserRequests(String username) throws RemoteException {
        try {
            return this.db.getUserRequests(username);
        }
        catch (Exception e) {
            System.err.println("Failed to get user '" + username + "' requests!\n - " + e);
            return null;
        }
    }

    @Override
    public LinkedList<Request> getWaitingRequests() throws RemoteException {
        return this.db.getWaitingRequests();
    }

    @Override
    public LinkedList<Book> getAllBooks() throws RemoteException {
        try {
            return this.db.getAllBooks();
        } catch (Exception e) {
            System.err.println("Failed to get all database books!\n - " + e);
            return null;
        }
    }

    void newMessageCallback(String consumer_tag, Delivery msg) {
        System.out.println("Got a new request!");
        Request req = (Request)this.queue.objFromBytes(msg.getBody());
        this.db.putRequest(req);
        //TODO: Warn Warehouse client GUI it got new request!
    }

    void cancelCallback(String consumer_tag) {
        System.out.println("Consumer cancelled queue");
    }

    @Override
    public void bookDispatched(String title, int amount, LinkedList<Long> req_uuids) throws RemoteException {
        this.db.bookDispatched(title, req_uuids);
        System.out.println("Book " + title + " will be dispatched");
        if (this.is_bookstore) {
            this.warnClientGUI();
        }
    }

    @Override
    public void allBooksDispatched(HashMap<String, Integer> book_amount, LinkedList<Long> req_uuids)
            throws RemoteException 
    {
        System.out.println("All requested books will be dispatched");
        book_amount.forEach((String title, Integer amount) -> {
            System.out.println("    Book " + title);
            this.db.bookDispatched(title, req_uuids);
        });
        if (this.is_bookstore) {
            this.warnClientGUI();
        }
    }

    private void warnClientGUI() {
        //TODO: Warn Bookstore client GUI that there are arriving books
        System.out.println("There are arriving books!");
    }
    
    @Override
    public LinkedList<BookRequests> getArrivedBooks() throws RemoteException {
        return this.db.getArrivedBooks();
        
    }

    @Override
    public void booksStored(LinkedList<BookRequests> books) throws RemoteException {
        this.db.booksStored(books);
    }
}
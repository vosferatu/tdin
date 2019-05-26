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
import bookstore.store.controller.BookstoreInterface;
import bookstore.warehouse.controller.WarehouseInterface;

public class Server extends BaseRMI implements ServerInterface {
    private boolean is_bookstore;
    private BaseQueue queue;
    private Database db;
    private PrinterInterface printer;

    public static void main(String[] args) {
        if (args.length == 1) {
            ServerInterface stub = null;
            try {
                boolean is_bookstore = args[0].equals("Bookstore");
                Server server = new Server(is_bookstore);
                if (is_bookstore) {
                    stub = (ServerInterface)registerObject((Remote) server, BS_SERVER_OBJ_NAME, BS_SERVER_OBJ_PORT);
                    server.printer = (PrinterInterface)fetchObject(PRINTER_OBJ_NAME, PRINTER_OBJ_PORT);
                }
                else
                    stub = (ServerInterface)registerObject((Remote) server, WH_SERVER_OBJ_NAME, WH_SERVER_OBJ_PORT);

            } catch (Exception e) {
                System.err.println("Bookstore server exception:");
                e.printStackTrace();
            }
            if (stub != null) {
                System.out.println(args[0] + " server is up!");
                try {System.in.read();} catch (Exception e) {}
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
    public void putRequest(String name, String email, String addr, Map<String, Integer> books) throws RemoteException {
        Request new_request = this.createRequest(name, email, addr, books);
        if (this.printer != null) {
            printer.printRequest(new_request);
        }
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

    private Request createRequest(String name, String email, String addr, Map<String, Integer> books_amount) {
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
        try {
            WarehouseInterface gui_obj = (WarehouseInterface)fetchObject(WH_CLIENT_OBJ_NAME, WH_CLIENT_OBJ_PORT);
            gui_obj.newBookRequest();
            System.out.println("Gui warned");
        }
        catch (Exception e) {
            System.err.println("Failed to warn Warehouse client GUI of arriving books!\n - " + e);
        }
    }

    void cancelCallback(String consumer_tag) {
        System.out.println("Consumer cancelled queue");
    }

    @Override
    public void booksDispatched(Map<String, Integer> book_amount, LinkedList<Long> req_uuids)
            throws RemoteException 
    {
        book_amount.forEach((String title, Integer amount) -> {
            this.db.bookDispatched(title, req_uuids);
        });
        if (this.is_bookstore) {
            this.warnClientGUI();
        }
    }

    private void warnClientGUI() {
        try {
            BookstoreInterface gui_obj = (BookstoreInterface)fetchObject(BS_CLIENT_OBJ_NAME, BS_CLIENT_OBJ_PORT);
            gui_obj.booksArriving();
            System.out.println("GUI warned!");
        }
        catch (Exception e) {
            System.err.println("Failed to warn Bookstore client GUI of arriving books!\n - " + e);
        }
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
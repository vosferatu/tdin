package bookstore.server;

import java.io.IOException;
import java.rmi.Remote;
import java.rmi.RemoteException;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.concurrent.TimeoutException;

import com.rabbitmq.client.Delivery;

import bookstore.commons.BaseRMI;
import bookstore.commons.BaseQueue;
import bookstore.server.responses.Book;
import bookstore.server.responses.Request;

public class Server extends BaseRMI implements ServerInterface {
    private static final int    BS_OBJ_PORT=8005;
    private static final String BS_OBJ_NAME="BookstoreServer";
    private static final int    WH_OBJ_PORT=8007;
    private static final String WH_OBJ_NAME="WarehouseServer";

    private boolean is_bookstore;
    private BaseQueue queue;
    private Database db;

    public static void main(String[] args) {
        if (args.length == 1) {
            try {
                boolean is_bookstore = args[0].equals("Bookstore");
                Server server = new Server(is_bookstore);
                ServerInterface stub;
                if (is_bookstore)
                    stub = (ServerInterface)registerObject((Remote) server, BS_OBJ_NAME, BS_OBJ_PORT);
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
    public String putRequest(Request new_request) throws RemoteException {
        System.out.println("Putting request on db!");
        HashMap<String, Request> orders = this.db.putRequest(new_request);
        Request unfinished_req = orders.get("unfinished"), finished_req = orders.get("finished");
        if (unfinished_req.getRequestBooks().size() > 0) {
            System.out.println("Sending request to warehouse");
            this.queue.sendObject(unfinished_req);
        }
        
        System.out.println(finished_req.toEmailString());
        // TODO: Send email to user
        return null;
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
        // TODO: Warn client that book has been dispatched
    }
}
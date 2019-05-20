package bookstore.store.client;

import bookstore.BaseRMI;
import bookstore.store.server.ServerInterface;

public class BookstoreClient extends BaseRMI {
    private static final String SERVER_NAME = "BookstoreServer";
    private static final int SERVER_PORT = 8005;

    public static void main(String[] args) {
        System.out.println("Starting Bookstore client...");
        try {
            ServerInterface server = (ServerInterface)fetchObject(SERVER_NAME, SERVER_PORT);
            if (server != null) {
                System.out.println(server.getAllBooks());
            }
        }
        catch (Exception e) {
            System.err.println("Failed to start Bookstore client!\n - " + e.getMessage());
        }
    }
}
package bookstore.store.client.controller;

import java.rmi.RemoteException;

import bookstore.store.client.gui.ClientWindow;
import bookstore.BaseRMI;
import bookstore.store.server.ServerInterface;
import bookstore.store.server.responses.Book;

public class BookstoreClient extends BaseRMI implements ClientInterface {
    private static final String SERVER_NAME = "BookstoreServer";
    private static final int SERVER_PORT = 8005;

    private ServerInterface server_obj;
    private ClientWindow client_gui;

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
        this.client_gui = new ClientWindow();
    }

    private void startClient() {
        // Starts executing the Client GUI thread
        this.client_gui.start();
        try {
            this.client_gui.setAvailableBooks(this.server_obj.getAllBooks());
        }
        catch (Exception e) {
            System.err.println("Error!\n - " + e.getMessage());
        }
    }

    @Override
    public void receiveBook(Book new_book) throws RemoteException {

    }
}
package bookstore.store.server;

import java.rmi.Remote;
import java.rmi.RemoteException;
import java.sql.SQLException;
import java.util.LinkedList;

import bookstore.store.server.responses.Request;
import bookstore.store.server.responses.Book;

public interface ServerInterface extends Remote {

    Request getUserRequests(String username) throws RemoteException, SQLException;
    LinkedList<Book> getAllBooks() throws RemoteException, SQLException;
    String putRequest(Request new_request) throws RemoteException, SQLException;
}
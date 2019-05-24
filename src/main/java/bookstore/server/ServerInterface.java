package bookstore.server;

import java.rmi.Remote;
import java.rmi.RemoteException;
import java.util.LinkedList;

import bookstore.server.responses.Request;
import bookstore.server.responses.Book;

public interface ServerInterface extends Remote {

    LinkedList<Request> getUserRequests(String username) throws RemoteException;
    LinkedList<Book> getAllBooks() throws RemoteException;
    String putRequest(Request new_request) throws RemoteException;
}
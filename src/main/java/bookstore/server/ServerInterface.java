package bookstore.server;

import java.rmi.Remote;
import java.rmi.RemoteException;
import java.util.LinkedList;
import java.util.Map;

import bookstore.server.responses.Request;
import bookstore.server.responses.Book;
import bookstore.server.responses.BookRequests;

public interface ServerInterface extends Remote {
    //Bookstore functions
    LinkedList<Request> getUserRequests(String username) throws RemoteException;
    LinkedList<Book> getAllBooks() throws RemoteException;
    void putRequest(String name, String addr, String email, Map<String, Integer> books) throws RemoteException;
    LinkedList<BookRequests> getArrivedBooks() throws RemoteException;
    void booksStored(LinkedList<BookRequests> books) throws RemoteException;
    
    //Warehouse functions
    LinkedList<Request> getWaitingRequests() throws RemoteException;
    void booksDispatched(Map<String, Integer> books_amount, LinkedList<Long> req_uuids) throws RemoteException;

}
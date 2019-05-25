package bookstore.server;

import java.rmi.Remote;
import java.rmi.RemoteException;
import java.util.HashMap;
import java.util.LinkedList;

import bookstore.server.responses.Request;
import bookstore.server.responses.Book;
import bookstore.server.responses.BookRequests;

public interface ServerInterface extends Remote {
    //Bookstore functions
    LinkedList<Request> getUserRequests(String username) throws RemoteException;
    LinkedList<Book> getAllBooks() throws RemoteException;
    void putRequest(Request new_request) throws RemoteException;
    LinkedList<BookRequests> getArrivedBooks() throws RemoteException;
    void booksStored(HashMap<String, Integer> book_amounts, LinkedList<Long> req_ids) throws RemoteException;
    
    //Warehouse functions
    LinkedList<Request> getWaitingRequests() throws RemoteException;
    void bookDispatched(String title, int amount, LinkedList<Long> req_uuids) throws RemoteException;
    void allBooksDispatched(HashMap<String, Integer> book_amount, LinkedList<Long> req_uuids) throws RemoteException;


}
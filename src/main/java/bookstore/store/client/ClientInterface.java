package bookstore.store.client;

import java.rmi.Remote;
import java.rmi.RemoteException;

import bookstore.store.server.responses.Book;

public interface ClientInterface extends Remote {
    void receiveBook(Book new_book) throws RemoteException;
}
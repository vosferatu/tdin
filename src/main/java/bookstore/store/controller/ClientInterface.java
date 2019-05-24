package bookstore.store.controller;

import java.rmi.Remote;
import java.rmi.RemoteException;

import bookstore.server.responses.Book;

public interface ClientInterface extends Remote {
    void receiveBook(Book new_book) throws RemoteException;
}
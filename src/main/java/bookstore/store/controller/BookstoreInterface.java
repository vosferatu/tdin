package bookstore.store.controller;

import java.rmi.Remote;
import java.rmi.RemoteException;

public interface BookstoreInterface extends Remote {
    void booksArriving() throws RemoteException;
}
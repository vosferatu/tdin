package bookstore.store;

import java.rmi.Remote;
import java.rmi.RemoteException;

import bookstore.server.responses.Request;

public interface PrinterInterface extends Remote {
    void printRequest(Request req) throws RemoteException;
}
package bookstore.warehouse.controller;

import java.rmi.Remote;
import java.rmi.RemoteException;

public interface WarehouseInterface extends Remote {
    void newBookRequest() throws RemoteException;
}
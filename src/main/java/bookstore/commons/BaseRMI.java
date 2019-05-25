package bookstore.commons;

import java.rmi.Remote;
import java.rmi.RemoteException;
import java.rmi.registry.LocateRegistry;
import java.rmi.registry.Registry;
import java.rmi.server.UnicastRemoteObject;

public class BaseRMI {
    protected static final String PRINTER_OBJ_NAME = "BookstorePrinter";
    protected static final int PRINTER_OBJ_PORT = 8001;
    protected static final String BS_SERVER_OBJ_NAME = "BookstoreServer";
    protected static final int BS_SERVER_OBJ_PORT = 8005;
    protected static final String BS_CLIENT_OBJ_NAME = "BookstoreClient";
    protected static final int BS_CLIENT_OBJ_PORT = 8006;
    protected static final String WH_SERVER_OBJ_NAME = "WarehouseServer";
    protected static final int WH_SERVER_OBJ_PORT = 8007;
    protected static final String WH_CLIENT_OBJ_NAME = "WarehouseClient";
    protected static final int WH_CLIENT_OBJ_PORT = 8008;
    private static final int DEFAULT_PORT = 1099;

    /**
     * Registers the RMI object in the Registry
     * 
     * @param obj  Object to register
     * @param name Name to associate with the object
     * @param port Port to link the object to
     * @return The registered object, null on error
     */
    public static Remote registerObject(Remote obj, String name, int port) {
        Remote stub;
        Registry reg;

        try {
            stub = (Remote)UnicastRemoteObject.exportObject(obj, port);
            reg = LocateRegistry.getRegistry();
        }
        catch (RemoteException err) {
            System.err.println("Failed to register object '" + name + "'!\n - " + err.getMessage());
            return null;
        }

        if (!tryBinding(reg, name, stub)) {
            System.out.println("Creating registry...");
            try {
                reg = LocateRegistry.createRegistry(DEFAULT_PORT);
            }
            catch (RemoteException err) {
                System.err.println("Failed to create registry!\n - " + err.getMessage());
                return null;
            }
        }
        else {
            return stub;
        }

        if (tryBinding(reg, name, stub)) {
            return stub;
        }
        return null;
    }

    /***
     * Fetches the specified remote object
     * @param name  Name of the remote object to search for
     * @param port  Port where the object is hosted
     * @return Remote object or null on error
     */
    public static Remote fetchObject(String name, int port) {
        try {
            Registry registry = LocateRegistry.getRegistry(DEFAULT_PORT);
            Remote obj = registry.lookup(name);
            return obj;
        }
        catch (Exception e) {
            System.err.println("Failed to fetch '" + name + "' object!\n - " + e.getMessage());
            return null;
        }
    }

    /**
     * Tries to bind the ID to the registry
     * 
     * @param reg  {@link Registry} to bind object to
     * @param id   ID of object to be binded
     * @param stub Object to be binded
     * @return Whether it was successfully binded or not
     */
    private static boolean tryBinding(Registry reg, String id, Remote stub) {
        try {
            reg.rebind(id, stub);
            return true;
        } catch (RemoteException err) {
            return false;
        }
    }
}
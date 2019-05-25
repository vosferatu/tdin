package bookstore.store;

import java.rmi.Remote;
import java.rmi.RemoteException;

import bookstore.commons.BaseRMI;
import bookstore.server.responses.Request;

class BookstorePrinter extends BaseRMI implements PrinterInterface {
    private static final String OBJ_NAME = "BookstorePrinter";
    private static final int OBJ_PORT = 8001;
    public static void main(String[] args) {
        try {
            BookstorePrinter printer = new BookstorePrinter();
            PrinterInterface stub = (PrinterInterface)registerObject((Remote)printer, OBJ_NAME, OBJ_PORT);
            if (stub != null) {
                System.out.println("Printer is up! Press <return> to exit");
                System.in.read();
            }
        }
        catch (Exception e) {
            System.err.println("Failed to start the printer!\n - " + e.getMessage());
        }
    }

    @Override
    public void printRequest(Request req) throws RemoteException {
        System.out.println(req.toPrinterString());
    }
    
}
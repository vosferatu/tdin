package bookstore.store;

import java.rmi.Remote;
import java.rmi.RemoteException;

import bookstore.commons.BaseRMI;
import bookstore.server.responses.Request;

class BookstorePrinter extends BaseRMI implements PrinterInterface {
    public static void main(String[] args) {
        try {
            BookstorePrinter printer = new BookstorePrinter();
            PrinterInterface stub = (PrinterInterface)registerObject((Remote)printer, PRINTER_OBJ_NAME, PRINTER_OBJ_PORT);
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
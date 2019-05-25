package bookstore.server.responses;

import java.util.LinkedList;
import java.io.Serializable;

// State = ["waiting", "dispatched", "dispatching"]

public class Request implements Serializable {
    private static final long serialVersionUID = -593874237645144342L;
    private static long ID = 0;
    private LinkedList<BookOrder> books;
    private String client_name;
    private String address;
    private String email;
    private long uuid;

    private Request() {
        this.books = new LinkedList<>();
        this.client_name = null;
        this.address = null;
        this.email = null;
    }

    public static Request fromClientData(String client_name, String email, String addr, LinkedList<BookOrder> books) {
        Request req = new Request();
        req.client_name = client_name;
        req.email = email;
        req.address = addr;
        req.books = books;

        return req;
    }

    public void assignID() {
        this.uuid = ID;
        ID+=1;
    }

    public LinkedList<BookOrder> getRequestBooks() {
        return this.books;
    }

    public LinkedList<String> getBooksName() {
        LinkedList<String> books_names = new LinkedList<>();

        for (BookOrder order : this.books) {
            books_names.add(order.getTitle());
        }

        return books_names;
    }

    public boolean isWaiting() {
        for (BookOrder order : this.books) {
            if (order.isWaiting()) 
                return true;
        }

        return false;
    }

    public String getClientName() {
        return this.client_name;
    }
    
    public String getAddress() {
        return this.address;
    }

    public String getEmail() {
        return this.email;
    }

    public String toEmailString() {
        double total_price = 0.0;
        String email = "Dear Mr. or Mrs. " + this.client_name + "\n\n";
        email += "You have made an order at our store for the following items:\n";
        for (BookOrder book : this.books) {
            int amount = book.getAmount();
            total_price += amount * book.getPrice();
            email += "  - " + amount + (amount > 1 ? " copies" : " copy") + 
                " of " + book.getTitle() + " (" + String.format("%1$,.2f", book.getPrice()) + "€ each)\n";
        }
        email += "\n   -> Total Price = " + String.format("%1$,.2f", total_price) + "€\n";
        email += "\n" + "They will all be dispatched at " + this.books.getFirst().getDisp_date().toString() + "\n\n";
        email += "Best Regards,\n João Almeida\n João Mendes";

        return email;
    }

    public boolean hasWaitingBook(String title) {
        for (BookOrder order : this.books) {
            if (order.isWaiting() && order.getTitle().equals(title))
                return true;
        }
        return false;
    }

    public int getBookAmount(String title) {
        for (BookOrder order : this.books) {
            if (order.getTitle().equals(title)) {
                return order.getAmount();
            }
        }

        return 0;
    }

    public void bookDispatched(String title) {
        for (BookOrder order : this.books) {
            if (order.getTitle().equals(title)) {
                order.setDispatchedAt(false, 2);
                return;
            }
        }
    }

    public long getID() {
        return this.uuid;
    }

    @Override
    public boolean equals(Object obj) {
        if (obj instanceof Request) {
            Request req = (Request)obj;
            return this.uuid == req.uuid;
        }
        return false;
    }
}
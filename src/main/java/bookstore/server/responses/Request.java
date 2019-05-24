package bookstore.server.responses;

import java.util.Date;
import java.text.DateFormat;
import java.util.LinkedList;
import java.io.Serializable;
import java.sql.ResultSet;
import java.sql.SQLException;

// State = ["waiting", "dispatched", "dispatching"]

public class Request implements Serializable {
    private static final long serialVersionUID = -593874237645144342L;
    private LinkedList<BookOrder> books;
    private String client_name;
    private String address;
    private String email;

    private Request() {
        this.books = new LinkedList<>();
        this.client_name = null;
        this.address = null;
        this.email = null;
    }

    public static Request fromServerData(ResultSet res) throws SQLException {
        Request req = new Request();
        while (res.next()) {
            String title = res.getString("title");
            int amount = res.getInt("amount");
            String state = res.getString("req_state");
            Date date;

            try {
                date = DateFormat.getDateInstance().parse(res.getString("date"));
            } catch (Exception e) {
                date = null;
            }

            req.books.add(new BookOrder(title, amount, date, state));
        }
        return req;
    }
    
    public static Request fromClientData(String client_name, String email, String addr, LinkedList<BookOrder> books) {
        Request req = new Request();
        req.client_name = client_name;
        req.email = email;
        req.address = addr;
        req.books = books;

        return req;
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
        String email = "Dear Mr. or Mrs. " + this.client_name + "\n\n";
        email += "You have made an order at our store for the following items:\n";
        for (BookOrder book : this.books) {
            int amount = book.getAmount();
            email += "  - " + amount + (amount > 1 ? "copies" : "copy") + " of " + book.getTitle() + "\n";
        }
        email += "\n" + "They will all be dispatched at " + this.books.getFirst().getDisp_date().toString() + "\n\n";
        email += "Best Regards,\n João Almeida\n João Mendes";

        return email;
    }
}
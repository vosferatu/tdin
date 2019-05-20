package bookstore.store.server.responses;

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

    public Request(ResultSet res) throws SQLException {
        this.books = new LinkedList<BookOrder>();
        while (res.next()) {
            String title = res.getString("title");
            int amount = res.getInt("amount");
            String state = res.getString("req_state");
            Date date;

            try {
                date = DateFormat.getDateInstance().parse(res.getString("date"));
            }
            catch (Exception e) {
                date = null;
            }

            this.books.add(new BookOrder(title, amount, date, state));
        }
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

    public String toHTMLTable() {
        String html = "";
        for (BookOrder book : this.books) {
            html += "<tr>\n";
            html += "<td>" + book.getTitle() + "</td>\n";
            html += "<td>" + book.getAmount() + "</td>\n";
            html += "<td>" + book.getState() + "</td>\n";
            html += "<td>" + book.getDisp_date() + "</td>\n";
            html += "</tr>\n";
        }

        return html;
    }
}
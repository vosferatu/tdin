package bookstore.store.server.responses;

import java.io.Serializable;

public class Book implements Serializable {
    private static final long serialVersionUID = 5729053599256218529L;
    private String title;
    private double price;
    private int stock;

    public Book(String title, double price, int stock) {
        this.title = title;
        this.price = price;
        this.stock = stock;
    }

    public int getStock() {
        return stock;
    }

    public double getPrice() {
        return price;
    }

    public String getTitle() {
        return title;
    }

    public String toHTML() {
        String html = "<tr>\n";
        html += "<td>" + this.title + "</td>\n";
        html += "<td>" + this.price + "€</td>\n";
        html += "<td>" + this.stock + "</td>\n";
        html += "<td><input tag=\"request_amount\" id=\"" 
            + this.title + "_amount\" type=\"number\" value=\"0\" min=\"0\"></input></td>\n";
        html += "</tr>\n";

        return html;
    }
}
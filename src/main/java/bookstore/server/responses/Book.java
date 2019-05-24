package bookstore.server.responses;

import java.io.Serializable;

public class Book implements Serializable {
    private static final long serialVersionUID = 5729053599256218529L;
    private String title;
    private double price;

    public Book(String title, double price) {
        this.title = title;
        this.price = price;
    }

    public double getPrice() {
        return price;
    }

    public String getTitle() {
        return title;
    }
}
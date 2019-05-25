package bookstore.server.responses;

import java.io.Serializable;

public class Book implements Serializable {
    private static final long serialVersionUID = 5729053599256218529L;
    private String title;
    private double price;
    private int stock;

    public Book(String title, double price, int amount) {
        this.title = title;
        this.price = price;
        this.stock = amount;
    }

    public double getPrice() {
        return price;
    }

    public String getTitle() {
        return title;
    }

    public int getStock() {
        return this.stock;
    }

    public void setStock(int stock) {
        this.stock = stock;
    }
}
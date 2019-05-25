package bookstore.store.gui;

import org.gnome.gtk.Grid;

import java.util.HashMap;

import org.gnome.gtk.Widget;

import bookstore.commons.GenericWindow;
import bookstore.commons.EventHandlers.AlterBookEvent;

public class ArrivingBooksWindow extends GenericWindow {
    private static final String FILE_NAME = "assets/windows/BookstoreWindow.glade";
    private static final String ROOT = "DispatchedWindow";

    Grid arrived_list;
    AlterBookEvent arrived_handler;
    HashMap<String, Integer> books;

    private ArrivingBooksWindow(AlterBookEvent arrived_handler) {
        super(FILE_NAME, ROOT);
        this.arrived_handler = arrived_handler;
    }

    public static ArrivingBooksWindow newWindow(AlterBookEvent arrived_handler) {
        ArrivingBooksWindow obj = new ArrivingBooksWindow(arrived_handler);
        try {
            obj.arrived_list = (Grid)obj.builder.getObject("ArrivingBooksList");
            return obj;
        }
        catch (Exception e) {
            System.err.println("Failed to create Arriving Books window!\n - " + e);
            return null;
        }
    }

    public Widget[] getWindowChildren() {
        return this.window.getChildren();
    }

    public void setArrivingBooks(HashMap<String, Integer> books) {
        this.books = books;
        books.forEach((String title, Integer amount) -> {
            BookEntry entry = new BookEntry(title, amount, this.arrived_handler);
            this.arrived_list.attach(entry, 0, this.arrived_list.getChildren().length, 1, 1);
        });
    }

    public void removeArrivingBook(String title) {
        for (Widget widget : this.arrived_list.getChildren()) {
            BookEntry entry = (BookEntry)widget;
            if (entry.getBookTitle().equals(title)) {
                this.arrived_list.remove(widget);
            }
        }
    }

    public void removeAllBooks() {
        for (Widget widget : this.arrived_list.getChildren()) {
            this.arrived_list.remove(widget);
        }
    }

    public HashMap<String, Integer> getAllBooks() {
        return this.books;
    }
}
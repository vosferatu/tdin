package bookstore.store.gui;

import java.util.LinkedList;

import org.gnome.gtk.Grid;
import org.gnome.gtk.Label;
import org.gnome.gtk.StateFlags;
import org.gnome.gtk.Widget;
import org.gnome.gdk.RGBA;

import bookstore.store.gui.BookEntry;
import bookstore.server.responses.Book;
import bookstore.commons.GenericWindow;
import bookstore.commons.EventHandlers.AlterBookEvent;

public class OrderCreatorWindow extends GenericWindow {
    private static final String FILE_NAME = "assets/windows/BookstoreWindow.glade";
    private static final String ROOT = "root";
    private boolean error = false;

    Grid book_list;
    Grid book_order;
    Label order_total;
    Label error_label;

    AlterBookEvent add_handler;
    AlterBookEvent rem_handler;

    public static OrderCreatorWindow newWindow(AlterBookEvent add_handler, AlterBookEvent rem_handler) {
        OrderCreatorWindow obj = new OrderCreatorWindow(add_handler, rem_handler);
        try {
            obj.book_list = (Grid) obj.builder.getObject("BookListTable");
            obj.book_order = (Grid) obj.builder.getObject("BookOrderTable");
            obj.order_total = (Label) obj.builder.getObject("OrderTotalPrice");
            obj.error_label = (Label) obj.builder.getObject("ErrorLabel");
            return obj;
        }
        catch (Exception e) {
            System.err.println("Failed to create Order Creator window!\n - " + e);
            return null;
        }
    }

    private OrderCreatorWindow(AlterBookEvent add_handler, AlterBookEvent rem_handler) {
        super(FILE_NAME, ROOT, true);
        this.add_handler = add_handler;
        this.rem_handler = rem_handler;
    }

    public void setAvailableBooks(LinkedList<Book> books) {
        for (int i = 0; i < books.size(); i++) {
            Book book = books.get(i);

            BookEntry entry = new BookEntry(book.getTitle(), book.getPrice(), this.add_handler);
            this.book_list.attach(entry, 0, this.book_list.getChildren().length, 1, 1);
        }
    }

    BookEntry getBookEntry(String book_title) {
        for (Widget widget : this.book_order.getChildren()) {
            BookEntry entry = (BookEntry)widget;
            if (entry.getBookTitle() ==  book_title) {
                return entry;
            }
        }

        return null;
    }

    public void addBookUnit(String book_title, double price) {
        BookEntry entry = this.getBookEntry(book_title);
        if (entry != null) {
            entry.updateBookAmount(+1);
        }
        else {
            entry = new BookEntry(book_title, price, 1, this.rem_handler);
            this.book_order.add(entry);
        }
        this.updateOrderTotal(price);
        if (error) {
            this.clearErrorMessage();
        }
    }

    public void remBookUnit(String title, double price) {
        BookEntry entry = this.getBookEntry(title);
        if (entry.updateBookAmount(-1)) {
            this.book_order.remove(entry);
        }
        this.updateOrderTotal(-price);
    }

    public void clearOrder() {
        for (Widget widget : this.book_order.getChildren()) {
            this.book_order.remove(widget);
        }
        this.updateOrderTotal(0);
        this.clearErrorMessage();
    }

    private void updateOrderTotal(double inc) {
        String curr_money_raw = this.order_total.getText();
        double curr_money = Double.valueOf(curr_money_raw.substring(0, curr_money_raw.length()-1));
        if (inc == 0) {
            this.order_total.setLabel("0.0 €");
        }
        else {
            this.order_total.setLabel(String.format("%1$,.2f €", curr_money + inc));
        }
    }

    public void setErrorMessage(String err) {
        this.error_label.setLabel(err);
        this.error_label.overrideColor(StateFlags.NORMAL, RGBA.RED);
        error = true;
    }

    public void clearErrorMessage() { 
        error = false;
        this.error_label.setLabel("");
        this.error_label.overrideColor(StateFlags.NORMAL, RGBA.BLACK);
    }
}
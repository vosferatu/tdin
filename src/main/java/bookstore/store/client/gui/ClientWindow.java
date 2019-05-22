package bookstore.store.client.gui;

import java.util.LinkedList;

import org.gnome.gdk.Event;
import org.gnome.gtk.Builder;
import org.gnome.gtk.Grid;
import org.gnome.gtk.Gtk;
import org.gnome.gtk.Widget;
import org.gnome.gtk.Label;
import org.gnome.gtk.Window;

import bookstore.store.client.gui.BookEntry.AlterBookEvent;
import bookstore.store.server.responses.Book;

public class ClientWindow extends Thread {

    Window window;
    Grid book_list;
    Grid book_order;
    Label order_total;
    AlterBookEvent add_handler;
    AlterBookEvent rem_handler;


    public ClientWindow(AlterBookEvent add_handler, AlterBookEvent rem_handler) {
        Gtk.init(new String[] {});
        this.add_handler = add_handler;
        this.rem_handler = rem_handler;
        Builder b = new Builder();
        try {
            b.addFromFile("assets/windows/BookstoreWindow.glade");
            this.window = (Window)b.getObject("root");
            this.book_list = (Grid)b.getObject("BookListTable");
            this.book_order = (Grid)b.getObject("BookOrderTable");
            this.order_total = (Label)b.getObject("OrderTotalPrice");
            this.window.connect(new Window.DeleteEvent() {
                @Override
                public boolean onDeleteEvent(Widget arg0, Event arg1) {
                    Gtk.mainQuit();
                    return false;
                }
            });
        }
        catch (Exception e) {
            System.err.println(e);
        }
    }

    /***
     * Starts the GTK3 rendering thread
     */
    @Override
    public void run() {
        this.window.showAll();
        Gtk.main();
    }

    public void setAvailableBooks(LinkedList<Book> books) {
        for (int i = 0; i < books.size(); i++) {
            Book book = books.get(i);

            BookEntry entry = new BookEntry(book.getTitle(), book.getPrice(), book.getStock(), this.add_handler);
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
            entry = new BookEntry(book_title, price, this.rem_handler);
            this.book_order.add(entry);
        }
        this.updateOrderTotal(price);
    }

    public void remBookUnit(String title, double price) {
        BookEntry entry = this.getBookEntry(title);
        if (entry.updateBookAmount(-1)) {
            this.book_order.remove(entry);
        }
        this.updateOrderTotal(-price);
    }

    private void updateOrderTotal(double inc) {
        String curr_money_raw = this.order_total.getText();
        double curr_money = Double.valueOf(curr_money_raw.substring(0, curr_money_raw.length()-1));
        this.order_total.setLabel(String.format("%1$,.2f €", curr_money + inc));
    }
}
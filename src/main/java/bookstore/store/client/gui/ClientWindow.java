package bookstore.store.client.gui;

import java.util.LinkedList;

import org.gnome.gtk.Gtk;
import org.gnome.gtk.Grid;
import org.gnome.gdk.Event;
import org.gnome.gtk.Label;
import org.gnome.gtk.Button;
import org.gnome.gtk.Widget;
import org.gnome.gtk.Window;
import org.gnome.gtk.Builder;
import org.gnome.gdk.EventButton;

import bookstore.store.commons.BookEntry;
import bookstore.store.server.responses.Book;
import bookstore.store.commons.EventHandlers.ClickedButton;
import bookstore.store.commons.EventHandlers.AlterBookEvent;

public class ClientWindow extends Thread {

    Window window;
    Grid book_list;
    Grid book_order;
    Label order_total;
    Button reset_button;
    Button submit_button;
    AlterBookEvent add_handler;
    AlterBookEvent rem_handler;
    ClickedButton submit_handler;
    ClickedButton reset_handler;

    @Override
    public void run() {
        this.window.showAll();
        Gtk.main();
    }

    private ClientWindow() {}

    public static ClientWindow newClient(AlterBookEvent add_handler, AlterBookEvent rem_handler, 
        ClickedButton submit_handler, ClickedButton reset_handler)
    {
        Gtk.init(new String[] {});
        ClientWindow window = new ClientWindow();
        window.add_handler = add_handler;
        window.rem_handler = rem_handler;
        window.submit_handler = submit_handler;
        window.reset_handler = reset_handler;
        Builder b = new Builder();
        try {
            b.addFromFile("assets/windows/BookstoreWindow.glade");
            window.window = (Window)b.getObject("root");
            window.book_list = (Grid)b.getObject("BookListTable");
            window.book_order = (Grid)b.getObject("BookOrderTable");
            window.order_total = (Label)b.getObject("OrderTotalPrice");
            window.reset_button = (Button)b.getObject("ResetButton");
            window.submit_button = (Button)b.getObject("SubmitButton");
            window.finishConnections();
        }
        catch (Exception e) {
            System.err.println(e);
            return null;
        }
        
        return window;
    }

    private void finishConnections() {
        this.window.connect((Window.DeleteEvent)(Widget arg0, Event arg1) -> {
            Gtk.mainQuit();
            return false;
        });
        this.submit_button.connect((Widget.ButtonReleaseEvent)(Widget arg0, EventButton arg1) -> {
            this.submit_handler.clicked();
            return true;
        });
        this.reset_button.connect((Widget.ButtonReleaseEvent) (Widget arg0, EventButton arg1) -> {
            this.reset_handler.clicked();
            return true;
        });
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

    public void clearOrder() {
        for (Widget widget : this.book_order.getChildren()) {
            this.book_order.remove(widget);
        }
        this.updateOrderTotal(0);
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
}
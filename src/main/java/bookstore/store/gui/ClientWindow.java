package bookstore.store.gui;

import java.util.LinkedList;

import org.gnome.gtk.Gtk;
import org.gnome.gtk.Grid;
import org.gnome.gdk.Event;
import org.gnome.gtk.Label;
import org.gnome.gtk.StateFlags;
import org.gnome.gtk.Button;
import org.gnome.gtk.Widget;
import org.gnome.gtk.Window;
import org.gnome.gtk.Builder;
import org.gnome.gdk.EventButton;
import org.gnome.gdk.RGBA;

import bookstore.store.gui.BookEntry;
import bookstore.server.responses.Book;
import bookstore.commons.EventHandlers.ClickedButton;
import bookstore.commons.EventHandlers.AlterBookEvent;

public class ClientWindow extends Thread {
    Window window;
    ClientPopup popup;
    Grid book_list;
    Grid book_order;
    Label order_total;
    Label error_label;
    Button reset_button;
    Button submit_button;
    AlterBookEvent add_handler;
    AlterBookEvent rem_handler;
    ClickedButton submit_handler;
    ClickedButton reset_handler;

    private boolean error = false;

    @Override
    public void run() {
        this.window.showAll();
        Gtk.main();
    }

    private ClientWindow() {}

    /**
     * Creates a new bookstore client window
     * @param book_handlers     Contains all the handlers of the book buttons [AddBookHandler, RemBookHandler]
     * @param button_handlers   Contains all the handlers of the buttons [SubmitHandler, ResetBooksHandler, FinishOrderHandler, ResetDetailsOrder]
     * @return
     */
    public static ClientWindow newClient(AlterBookEvent[] book_handlers, ClickedButton[] button_handlers) {
        Gtk.init(new String[] {});
        ClientWindow window = new ClientWindow();
        window.popup = new ClientPopup(button_handlers[2], button_handlers[3]);
        window.add_handler = book_handlers[0];
        window.rem_handler = book_handlers[1];
        window.submit_handler = button_handlers[0];
        window.reset_handler = button_handlers[1];
        Builder b = new Builder();
        try {
            b.addFromFile("assets/windows/BookstoreWindow.glade");
            window.window = (Window)b.getObject("root");
            window.book_list = (Grid)b.getObject("BookListTable");
            window.book_order = (Grid)b.getObject("BookOrderTable");
            window.order_total = (Label)b.getObject("OrderTotalPrice");
            window.reset_button = (Button)b.getObject("ResetButton");
            window.submit_button = (Button)b.getObject("SubmitButton");
            window.error_label = (Label)b.getObject("ErrorLabel");
            window.finishConnections();
            return window;
        }
        catch (Exception e) {
            System.err.println(e);
            return null;
        }
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

    public ClientPopup getPopup() {
        return this.popup;
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
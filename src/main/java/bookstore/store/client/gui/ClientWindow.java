package bookstore.store.client.gui;

import java.util.LinkedList;

import org.gnome.gdk.Event;
import org.gnome.gtk.Builder;
import org.gnome.gtk.Grid;
import org.gnome.gtk.Gtk;
import org.gnome.gtk.Widget;
import org.gnome.gtk.Window;

import bookstore.store.server.responses.Book;

public class ClientWindow extends Thread {

    Window window;
    Grid book_list;
    Grid book_order;


    public ClientWindow() {
        Gtk.init(new String[] {});
        Builder b = new Builder();
        try {
            b.addFromFile("assets/windows/BookstoreWindow.glade");
            this.window = (Window)b.getObject("root");
            this.book_list = (Grid)b.getObject("BookListTable");
            this.book_order = (Grid)b.getObject("BookOrderTable");
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

            BookEntry entry = new BookEntry(book.getTitle(), book.getPrice(), book.getStock(), (String title) -> {
                return this.addBookUnit(title);
            });
            System.out.println("Book_list childs = " + i);
            this.book_list.attach(entry, 0, this.book_list.getChildren().length, 1, 1);
        }
    }

    boolean addBookUnit(String book_title) {
        System.out.println("Adding a single unit of '" + book_title + "'");
        return true;
    }
}
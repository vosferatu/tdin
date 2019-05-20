package bookstore.store;

import org.gnome.gdk.Event;
import org.gnome.gtk.Builder;
import org.gnome.gtk.Gtk;
import org.gnome.gtk.Widget;
import org.gnome.gtk.Window;

public class ExampleWindow {

    public static void main(String[] args) {
        Gtk.init(args);
        Builder b = new Builder();
        try {
            b.addFromFile("assets/windows/BookstoreWindow.glade");
        }
        catch (Exception e) {
            System.err.println(e);
            return;
        }
        Window window = (Window)b.getObject("root");
        window.connect(new Window.DeleteEvent(){
        
            @Override
            public boolean onDeleteEvent(Widget arg0, Event arg1) {
                Gtk.mainQuit();
                return false;
            }
        });

        window.showAll();
        Gtk.main();
    }
}
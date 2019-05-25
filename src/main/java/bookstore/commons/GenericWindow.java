package bookstore.commons;

import org.gnome.gtk.Gtk;
import org.gnome.gdk.Event;
import org.gnome.gtk.Widget;
import org.gnome.gtk.Window;
import org.gnome.gtk.Builder;
import org.gnome.gdk.EventButton;

import bookstore.commons.EventHandlers.ClickedButton;

public abstract class GenericWindow {
    protected Window window;
    protected Builder builder;

    protected GenericWindow(String file_name, String root) {
        this.builder = new Builder();
        try {
            this.builder.addFromFile(file_name);
            this.window = (Window)this.builder.getObject(root);
            this.window.connect((Window.DeleteEvent) (Widget arg0, Event arg1) -> {
                Gtk.mainQuit();
                return false;
            });
        }
        catch (Exception e) {
            System.err.println("Failed to read '" + file_name + "' file!\n - " + e);
        }
    }

    public void connectHandler(String widget_name, ClickedButton handler) {
        try {
            Widget obj = (Widget) this.builder.getObject(widget_name);
            obj.connect((Widget.ButtonReleaseEvent) (Widget w, EventButton b) -> {
                handler.clicked();
                return true;
            });
        } catch (Exception e) {
            System.err.println("Failed to connect to widget '" + widget_name + "'!\n - " + e);
        }
    }

    public Window getWindow() {
        return this.window;
    }
}
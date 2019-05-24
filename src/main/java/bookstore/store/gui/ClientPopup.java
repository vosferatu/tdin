package bookstore.store.gui;

import org.gnome.gtk.Window;
import org.gnome.gtk.Entry;
import org.gnome.gtk.Widget;
import org.gnome.gtk.Button;
import org.gnome.gtk.Builder;
import org.gnome.gdk.EventButton;

import bookstore.commons.EventHandlers.ClickedButton;

class ClientPopup {
    Window window;
    Entry name_field;
    Entry email_field;
    Entry addr_field;

    ClickedButton finish_handler;
    ClickedButton reset_handler;

    public ClientPopup(ClickedButton finish_handler, ClickedButton reset_handler) {
        this.finish_handler = finish_handler;
        this.reset_handler = reset_handler;
        Builder b = new Builder();
        try {
            b.addFromFile("assets/windows/BookstorePopup.glade");
            this.window = (Window)b.getObject("root");
            this.name_field = (Entry)b.getObject("ClientNameField");
            this.email_field = (Entry)b.getObject("ClientEmailField");
            this.addr_field = (Entry)b.getObject("ClientAddressField");
            Button finish_button = (Button)b.getObject("FinishButton");
            finish_button.connect((Widget.ButtonReleaseEvent)(Widget arg0, EventButton arg1) -> {
                this.finish_handler.clicked();
                return true;
            });
            Button reset_button = (Button)b.getObject("ResetButton");
            reset_button.connect((Widget.ButtonReleaseEvent)(Widget arg0, EventButton arg1) -> {
                this.finish_handler.clicked();
                return true;
            });
        }
        catch (Exception e) {
            System.err.println(e);
        }
    }

    public void show() {
        this.window.showAll();
    }

    public void closeWindow() {
        this.name_field.setText("");
        this.email_field.setText("");
        this.addr_field.setText("");
        this.window.hide();
    }

}
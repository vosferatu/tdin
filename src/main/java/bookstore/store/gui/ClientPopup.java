package bookstore.store.gui;

import org.gnome.gtk.Window;
import org.gnome.gtk.Entry;
import org.gnome.gtk.Label;
import org.gnome.gtk.StateFlags;
import org.gnome.gtk.Widget;
import org.gnome.gtk.Button;
import org.gnome.gtk.Editable;
import org.gnome.gtk.Builder;
import org.gnome.gdk.EventButton;
import org.gnome.gdk.RGBA;

import bookstore.commons.EventHandlers.ClickedButton;

public class ClientPopup {
    Window window;
    Entry name_field;
    Entry email_field;
    Entry addr_field;
    Label name_label;
    Label email_label;
    Label addr_label;

    Button reset_button;
    Button finish_button;

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
            this.finish_button = (Button)b.getObject("FinishButton");
            this.reset_button = (Button)b.getObject("ResetButton");
            this.name_label = (Label)b.getObject("ClientNameLabel");
            this.email_label = (Label) b.getObject("ClientEmailLabel");
            this.addr_label = (Label) b.getObject("ClientAddressLabel");
            this.finishConnections();
        }
        catch (Exception e) {
            System.err.println(e);
        }
    }
    
    private void finishConnections() {
        this.finish_button.connect((Widget.ButtonReleaseEvent)(Widget arg0, EventButton arg1) -> {
            this.finish_handler.clicked();
            return true;
        });
        this.reset_button.connect((Widget.ButtonReleaseEvent)(Widget arg0, EventButton arg1) -> {
            this.reset_handler.clicked();
            return true;
        });
        this.name_field.connect((Editable.Changed)(Editable widget) -> {
            this.clearErrorMessage(this.name_label);
        });
        this.email_field.connect((Editable.Changed) (Editable widget) -> {
            this.clearErrorMessage(this.email_label);
        });
        this.addr_field.connect((Editable.Changed) (Editable widget) -> {
            this.clearErrorMessage(this.addr_label);
        });
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

    public String getInputName() {
        return this.name_field.getText();
    }

    public String getInputEmail() {
        return this.email_field.getText();
    }

    public String getInputAddr() {
        return this.addr_field.getText();
    }

    public void setNameError(String err) {
        this.setErrorMessage(this.name_label, this.name_label.getLabel() + " : " + err);
    }

    public void setEmailError(String err) {
        this.setErrorMessage(this.email_label, this.email_label.getLabel() + " : " + err);
    }

    public void setAddrError(String err) {
        this.setErrorMessage(this.addr_label, this.addr_label.getLabel() + " : " + err);
    }
    
    public void setErrorMessage(Label label, String new_text) {
        label.setLabel(new_text);
        label.overrideColor(StateFlags.NORMAL, RGBA.RED);
    }

    public void clearErrorMessage(Label label) {
        String curr_text = label.getLabel();
        int prev_i = curr_text.indexOf(" : ");
        System.err.println("Curr = " + curr_text);
        System.err.println("':' -> " + curr_text.indexOf(':'));
        if (prev_i != -1) {
            label.setLabel(curr_text.substring(0, curr_text.indexOf(" : ")));
            label.overrideColor(StateFlags.NORMAL, RGBA.BLACK);
        }
    }

    public void clearAllErrors() {
        this.clearErrorMessage(this.name_label);
        this.clearErrorMessage(this.email_label);
        this.clearErrorMessage(this.addr_label);
    }

    public void resetDetails() {
        this.clearAllErrors();
        this.name_field.setText("");
        this.email_field.setText("");
        this.addr_field.setText("");
    }
}
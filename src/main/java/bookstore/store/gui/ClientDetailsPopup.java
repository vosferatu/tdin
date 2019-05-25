package bookstore.store.gui;

import org.gnome.gtk.Entry;
import org.gnome.gtk.Label;
import org.gnome.gtk.StateFlags;
import org.gnome.gtk.Editable;
import org.gnome.gdk.RGBA;

import bookstore.commons.GenericWindow;

public class ClientDetailsPopup extends GenericWindow {
    private static final String FILE_NAME = "assets/windows/BookstorePopup.glade";
    private static final String ROOT = "root";

    Entry name_field;
    Entry email_field;
    Entry addr_field;
    Label name_label;
    Label email_label;
    Label addr_label;

    private ClientDetailsPopup() {
        super(FILE_NAME, ROOT, false);
    }

    public static ClientDetailsPopup newWindow() {
        ClientDetailsPopup obj = new ClientDetailsPopup();
        try {
            obj.name_field = (Entry)obj.builder.getObject("ClientNameField");
            obj.email_field = (Entry)obj.builder.getObject("ClientEmailField");
            obj.addr_field = (Entry)obj.builder.getObject("ClientAddressField");
            obj.name_label = (Label)obj.builder.getObject("ClientNameLabel");
            obj.email_label = (Label)obj.builder.getObject("ClientEmailLabel");
            obj.addr_label = (Label)obj.builder.getObject("ClientAddressLabel");
            obj.finishConnections();
            return obj;
        }
        catch (Exception e) {
            System.err.println("Failed to create Client Details window!\n - " + e);
            return null;
        }
    }

    private void finishConnections() {
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
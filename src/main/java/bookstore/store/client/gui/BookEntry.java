package bookstore.store.client.gui;

import org.gnome.gtk.Grid;
import org.gnome.gtk.Label;
import org.gnome.gtk.Widget;
import org.gnome.gdk.EventButton;
import org.gnome.gtk.EventBox;
import org.gnome.gtk.IconSize;
import org.gnome.gtk.Image;
import org.freedesktop.icons.ActionIcon;
import org.freedesktop.icons.Icon;

public class BookEntry extends Grid {
    Label title_label;
    Label price_label;
    Label amount_label;
    ImageButton button;
    String book_title;
    AlterBookEvent handler;
   
    public interface AlterBookEvent {
        void run(String book_title);
    }
    class ImageButton extends EventBox {
        String title;
        AlterBookEvent handler;
        Image img;

        ImageButton(Icon icon, IconSize size, String book_title, AlterBookEvent handler) {
            this.title = book_title;
            this.handler = handler;

            this.img = new Image(icon, size);
            this.add(this.img);
            this.connect((Widget.ButtonReleaseEvent)(Widget arg0, EventButton arg1) -> {
                this.handler.run(this.title);   
                return true;
            });
            this.showAll();
        }
    }


    BookEntry(String title, double price, int amount, AlterBookEvent handler) {
        this.handler = handler;
        this.book_title = title;
        this.title_label = new Label(title);
        this.title_label.setSizeRequest(300, 10);
        this.title_label.setExpandHorizontal(true);
        this.title_label.setAlignment(0.01f, 0.5f);
        this.attach(this.title_label, 0, 0, 1, 1);
        this.price_label = new Label(String.valueOf(price) + "€");
        this.price_label.setSizeRequest(120, 10);
        this.attach(this.price_label, 1, 0, 1, 1);
        this.amount_label = new Label(String.valueOf(amount));
        this.amount_label.setSizeRequest(80, 10);
        this.attach(this.amount_label, 2, 0, 1, 1);
        this.button = new ImageButton(ActionIcon.LIST_ADD, IconSize.BUTTON, title, 
            (String book_title) -> this.handler.run(book_title));
        this.button.setSizeRequest(80, 10);
        this.attach(this.button, 3, 0, 1, 1);
        this.showAll();
    }

    BookEntry(String title, double price, AlterBookEvent handler) {
        this.handler = handler;
        this.book_title = title;
        this.title_label = new Label(title);
        this.title_label.setSizeRequest(300, 10);
        this.title_label.setExpandHorizontal(true);
        this.title_label.setAlignment(0.01f, 0.5f);
        this.attach(this.title_label, 0, 0, 1, 1);
        this.price_label = new Label(String.valueOf(price) + "€");
        this.price_label.setSizeRequest(120, 10);
        this.attach(this.price_label, 1, 0, 1, 1);
        this.amount_label = new Label(String.valueOf(1));
        this.amount_label.setSizeRequest(80, 10);
        this.attach(this.amount_label, 2, 0, 1, 1);
        this.button = new ImageButton(ActionIcon.LIST_REMOVE, IconSize.BUTTON, title, 
            (String book_title) -> this.handler.run(book_title));
        this.button.setSizeRequest(80, 10);
        this.attach(this.button, 3, 0, 1, 1);
        this.showAll();
    }

    String getBookTitle() {
        return this.book_title;
    }

    /**
     * Updates the amount of this given book
     * @param inc Increment to add to the amount
     * @return Whether amount has reached 0 or not
     */
    boolean updateBookAmount(int inc) {
        int new_value = (Integer.parseInt(amount_label.getText()) + inc);
        if (new_value > 0) {
            this.amount_label.setLabel(String.valueOf(new_value));
        }

        return new_value <= 0;

    }
}
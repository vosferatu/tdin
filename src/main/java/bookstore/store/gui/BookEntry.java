package bookstore.store.gui;

import org.gnome.gtk.Grid;
import org.gnome.gtk.Label;
import org.gnome.gtk.IconSize;
import org.freedesktop.icons.ActionIcon;
import org.freedesktop.icons.DeviceIcon;

import bookstore.commons.ImageButton;
import bookstore.commons.EventHandlers.AlterBookEvent;

public class BookEntry extends Grid {
    Label title_label;
    Label price_label;
    Label amount_label;
    ImageButton button;
    String book_title;
    AlterBookEvent handler;

    public BookEntry(String title, double price, int amount, AlterBookEvent handler) {
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
        this.button = new ImageButton(ActionIcon.LIST_REMOVE, IconSize.LARGE_TOOLBAR, title,
                (String book_title) -> this.handler.run(book_title));
        this.button.setSizeRequest(80, 10);
        this.attach(this.button, 3, 0, 1, 1);
        this.showAll();
    }

    public BookEntry(String title, double price, AlterBookEvent handler) {
        this.handler = handler;
        this.book_title = title;
        this.title_label = new Label("<big>" + title + "</big>");
        this.title_label.setUseMarkup(true);
        this.title_label.setSizeRequest(300, 10);
        this.title_label.setExpandHorizontal(true);
        this.title_label.setAlignment(0.01f, 0.5f);
        this.attach(this.title_label, 0, 0, 1, 1);
        this.price_label = new Label(String.valueOf(price) + "€");
        this.price_label.setSizeRequest(120, 10);
        this.attach(this.price_label, 1, 0, 1, 1);
        this.button = new ImageButton(ActionIcon.LIST_ADD, IconSize.LARGE_TOOLBAR, title,
                (String book_title) -> this.handler.run(book_title));
        this.button.setSizeRequest(80, 10);
        this.attach(this.button, 2, 0, 1, 1);
        this.showAll();
    }

    public BookEntry(String title, int amount, AlterBookEvent handler) {
        this.handler = handler;
        this.book_title = title;
        this.title_label = new Label("<big>" + title + "</big>");
        this.title_label.setUseMarkup(true);
        this.title_label.setSizeRequest(300, 10);
        this.title_label.setExpandHorizontal(true);
        this.title_label.setAlignment(0.01f, 0.5f);
        this.attach(this.title_label, 0, 0, 1, 1);
        this.amount_label = new Label(String.valueOf(amount));
        this.amount_label.setSizeRequest(80, 10);
        this.attach(this.amount_label, 1, 0, 1, 1);
        this.button = new ImageButton(DeviceIcon.MEDIA_FLOPPY, IconSize.LARGE_TOOLBAR, title,
                (String book_title) -> this.handler.run(book_title));
        this.button.setSizeRequest(80, 10);
        this.attach(this.button, 2, 0, 1, 1);
        this.showAll();
    }

    public String getBookTitle() {
        return this.book_title;
    }

    /**
     * Updates the amount of this given book
     * 
     * @param inc Increment to add to the amount
     * @return Whether amount has reached 0 or not
     */
    public boolean updateBookAmount(int inc) {
        int new_value = (Integer.parseInt(amount_label.getText()) + inc);
        if (new_value > 0) {
            this.amount_label.setLabel(String.valueOf(new_value));
        }

        return new_value <= 0;

    }
}
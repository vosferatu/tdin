package bookstore.commons;

import org.gnome.gtk.Image;
import org.gnome.gtk.Widget;
import org.gnome.gtk.EventBox;
import org.gnome.gtk.IconSize;
import org.gnome.gdk.EventButton;
import org.freedesktop.icons.Icon;

import bookstore.commons.EventHandlers.AlterBookEvent;

public class ImageButton extends EventBox {
    String title;
    AlterBookEvent handler;
    Image img;

    public ImageButton(Icon icon, IconSize size, String book_title, AlterBookEvent handler) {
        this.title = book_title;
        this.handler = handler;

        this.img = new Image(icon, size);
        this.add(this.img);
        this.connect((Widget.ButtonReleaseEvent) (Widget arg0, EventButton arg1) -> {
            this.handler.run(this.title);
            return true;
        });
        this.showAll();
    }
}
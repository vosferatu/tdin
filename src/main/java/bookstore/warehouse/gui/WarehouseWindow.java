package bookstore.warehouse.gui;

import org.gnome.gtk.Gtk;
import org.gnome.gtk.Grid;

import java.util.HashMap;

import org.gnome.gdk.Event;
import org.gnome.gtk.Widget;
import org.gnome.gtk.Button;
import org.gnome.gtk.Window;
import org.gnome.gtk.Builder;
import org.gnome.gdk.EventButton;

import bookstore.commons.EventHandlers.ClickedButton;
import bookstore.commons.EventHandlers.AlterBookEvent;

public class WarehouseWindow extends Thread {
    private Window window;
    private Button send_all_button;
    private Button refresh_button;
    private Grid order_list;

    private AlterBookEvent send_handler;
    private ClickedButton send_all_handler;
    private ClickedButton refresh_handler;

    @Override
    public void run() {
        this.window.showAll();
        Gtk.main();
    }

    public static WarehouseWindow newWindow(
        AlterBookEvent send_handler, 
        ClickedButton send_all_handler,
        ClickedButton refresh_handler) 
    {
        Gtk.init(new String[] {});
        WarehouseWindow window = new WarehouseWindow();
        window.send_handler = send_handler;
        window.send_all_handler = send_all_handler;
        window.refresh_handler = refresh_handler;
        Builder b = new Builder();
        try {
            b.addFromFile("assets/windows/WarehouseWindow.glade");
            window.window = (Window)b.getObject("root");
            window.order_list = (Grid)b.getObject("BookOrderList");
            window.send_all_button = (Button)b.getObject("DispatchAllButton");
            window.refresh_button = (Button)b.getObject("RefreshButton");
            window.finishConnections();
            return window;
        }
        catch (Exception e) {
            return null;
        }
    }

    private void finishConnections() {
        this.window.connect((Window.DeleteEvent) (Widget arg0, Event arg1) -> {
            Gtk.mainQuit();
            return false;
        });
        this.send_all_button.connect((Widget.ButtonReleaseEvent) (Widget arg0, EventButton arg1) -> {
            this.send_all_handler.clicked();
            return true;
        });
        this.refresh_button.connect((Widget.ButtonReleaseEvent) (Widget a, EventButton b) -> {
            this.refresh_handler.clicked();
            return true;
        });
    }

    public void setOrderList(HashMap<String, Integer> orders) {
        orders.forEach((String title, Integer amount) -> {
            OrderEntry entry = new OrderEntry(title, amount, this.send_handler);
            this.order_list.attach(entry, 0, this.order_list.getChildren().length, 1, 1);
        });
    }

    public void clearOrders() {
        for (Widget widget : this.order_list.getChildren()) {
            this.order_list.remove(widget);
        }
    }

    public void clearOrder(String title) {
        for (Widget widget : this.order_list.getChildren()) {
            OrderEntry entry = (OrderEntry)widget;
            if (entry.getTitle().equals(title)) {
                this.order_list.remove(widget);
            }
        }
    }

    public void incrementAmount(String title, int amount) {
        for (Widget widget : this.order_list.getChildren()) {
            OrderEntry entry = (OrderEntry)widget;
            if (entry.getTitle().equals(title)) {
                entry.updateBookAmount(amount);
            }
        }
    }
}
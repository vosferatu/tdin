package bookstore.warehouse.gui;

import org.gnome.gtk.Grid;

import java.util.HashMap;

import org.gnome.gtk.Widget;

import bookstore.commons.GenericWindow;
import bookstore.commons.EventHandlers.AlterBookEvent;

public class WarehouseWindow extends GenericWindow {
    private static final String FILE_NAME = "assets/windows/WarehouseWindow.glade";
    private static final String ROOT = "root";

    private Grid order_list;

    private AlterBookEvent send_handler;

    private WarehouseWindow(AlterBookEvent send_handler) {
        super(FILE_NAME, ROOT, true);
        this.send_handler = send_handler;
    }

    public static WarehouseWindow newWindow(AlterBookEvent send_handler) {
        WarehouseWindow window = new WarehouseWindow(send_handler);
        try {
            window.order_list = (Grid)window.builder.getObject("BookOrderList");
            return window;
        }
        catch (Exception e) {
            return null;
        }
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
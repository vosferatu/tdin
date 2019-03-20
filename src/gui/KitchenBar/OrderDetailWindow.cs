using Gtk;
using Glade;
using System;
using System.Collections.Generic;

namespace Restaurant {
    public class OrderDetailWindow {
        private const string WINDOW_FILE = GuiConstants.WINDOWS_DIR + "OrderDetail.glade";
        private const string WINDOW_NAME = "root";

        [Glade.Widget]
        public Gtk.Window root;
        [Glade.Widget]
        Gtk.Button BackButton;
        [Glade.Widget]
        Gtk.Label OrderDescriptionLabel;
        [Glade.Widget]
        Gtk.Label TableNLabel;
        [Glade.Widget]
        Gtk.VBox DishesBox;
        [Glade.Widget]
        Gtk.VBox DrinksBox;

        public OrderDetailWindow(EventHandler back_handler, long order_id, string desc, int table_n) {
            Glade.XML gxml = new Glade.XML(WINDOW_FILE, WINDOW_NAME, null);
            gxml.Autoconnect(this);
            root.Title = "Order #" + order_id;
            OrderDescriptionLabel.Text = desc;
            TableNLabel.Text = "Table #" + table_n;
            BackButton.Clicked += back_handler;
        }

        public void AddProducts(Dictionary<string, uint> products, bool is_dish) {
            if (is_dish) {
                this.AddProductsToVBox(products, this.DishesBox);
            }
            else {
                this.AddProductsToVBox(products, DrinksBox);
            }
        }
        
        private void AddProductsToVBox(Dictionary<string, uint> products, Gtk.VBox box) {
            foreach(KeyValuePair<string, uint> product in products) {
                box.PackStart(this.CreateBox(product.Key, product.Value));
            }
        }

        private HBox CreateBox(string name, uint amount) {
            Gtk.HBox p_entry = new Gtk.HBox(false, 2);
            Gtk.Label p_name = new Gtk.Label(name);
            Gtk.Label p_amount = new Gtk.Label(String.Format("{0}", amount));
            p_entry.Add(p_name);
            p_entry.Add(p_amount);

            return p_entry;
        }
    }
}
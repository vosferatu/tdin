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
        Gtk.Label TableNLabel;
        [Glade.Widget]
        Gtk.Table ProductsBox;

        public OrderDetailWindow(EventHandler back_handler, long order_id, string desc, uint table_n) {
            Glade.XML gxml = new Glade.XML(WINDOW_FILE, WINDOW_NAME, null);
            gxml.Autoconnect(this);
            root.Title = desc;
            TableNLabel.Text = String.Format("Table #{0}",  table_n);
            BackButton.Clicked += back_handler;
        }

        public void AddProducts(Dictionary<string, uint> products, bool is_dish) {
            foreach(KeyValuePair<string, uint> product in products) {
                uint child_n = (uint)this.ProductsBox.Children.Length;
                this.ProductsBox.Attach(this.CreateBox(product.Key, product.Value),
                0, 1, 0 + child_n, 1 + child_n, 
                Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink, 0, 3);
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
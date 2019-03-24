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

        public OrderDetailWindow(EventHandler back_handler, Dictionary<string, uint> prods, long order_id, uint table_n) {
            Glade.XML gxml = new Glade.XML(WINDOW_FILE, WINDOW_NAME, null);
            gxml.Autoconnect(this);
            root.Title = String.Format("Order #{0} Details", order_id);
            TableNLabel.Text = String.Format("Table #{0}",  table_n);
            BackButton.Clicked += back_handler;
            foreach(KeyValuePair<string, uint> product in prods) {
                uint child_n = (uint)this.ProductsBox.Children.Length;
                this.ProductsBox.Attach(
                    this.CreateBox(product.Key, product.Value),
                    0, 1, 0 + child_n, 1 + child_n,
                    Gtk.AttachOptions.Expand, Gtk.AttachOptions.Shrink,
                    0, 0
                );
            }
        }

        // TODO: Center this freaking labels
        private HBox CreateBox(string name, uint amount) {
            Gtk.HBox p_entry = new Gtk.HBox(false, 0);
            Gtk.Label p_name = new Gtk.Label(name);
            Gtk.Label p_amount = new Gtk.Label(String.Format("{0}", amount));
            p_entry.Homogeneous = false;
            p_name.SetSizeRequest(80, 30);
            p_amount.SetSizeRequest(250, 30);
            p_entry.Add(p_name);
            p_entry.Add(p_amount);
            p_entry.ShowAll();
            return p_entry;
        }

        public void OnDelete(object o, DeleteEventArgs e) {
            Application.Quit();
        }
    }
}
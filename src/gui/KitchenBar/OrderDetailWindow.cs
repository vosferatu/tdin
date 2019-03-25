using Gdk;
using Gtk;
using Glade;
using System;
using System.Collections.Generic;

namespace Restaurant {
    /// <summary>
    /// Responsible for communicating directly with Gtk on the Order Detail Window
    /// </summary>
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

        /// <summary>
        /// Creates the window
        /// </summary>
        /// <param name="back_handler"></param>
        /// <param name="prods"></param>
        /// <param name="order_id"></param>
        /// <param name="table_n"></param>
        public OrderDetailWindow(EventHandler back_handler, Dictionary<string, uint> prods, long order_id, uint table_n) {
            Glade.XML gxml = new Glade.XML(WINDOW_FILE, WINDOW_NAME, null);
            gxml.Autoconnect(this);
            this.root.SetIconFromFile(GuiConstants.APP_ICON);
            this.root.Title = String.Format("Order #{0} Details", order_id);
            TableNLabel.Text = String.Format("Table #{0}",  table_n);
            BackButton.Clicked += back_handler;
            foreach(KeyValuePair<string, uint> product in prods) {
                uint child_n = (uint)this.ProductsBox.Children.Length;
                this.ProductsBox.Attach(
                    this.CreateBox(product.Key, product.Value),
                    0, 1, 0 + child_n, 1 + child_n,
                    Gtk.AttachOptions.Expand | Gtk.AttachOptions.Fill, Gtk.AttachOptions.Shrink,
                    0, 0
                );
            }
        }

        /// <summary>
        /// Creates the box that will hold a single product
        /// </summary>
        /// <param name="name">Name of the product</param>
        /// <param name="amount">Amount of the product</param>
        /// <returns></returns>
        private Gtk.Table CreateBox(string name, uint amount) {
            Gtk.Table p_entry = new Gtk.Table(1, 1, false);
            Gtk.Label p_name = new Gtk.Label(name);
            Gtk.Label p_amount = new Gtk.Label(amount.ToString());
            p_name.Xalign = 0.05f;
            p_amount.UseMarkup = true;
            p_amount.Markup = String.Format("<b><big>{0}</big></b>", p_amount.Text);
            p_name.SetSizeRequest(210, 30);
            p_entry.Attach(p_name,
                0, 1, 0, 1,
                Gtk.AttachOptions.Expand | Gtk.AttachOptions.Fill, Gtk.AttachOptions.Shrink,
                0, 0
            );
            p_entry.Attach(p_amount,
                1, 2, 0, 1,
                Gtk.AttachOptions.Expand | Gtk.AttachOptions.Fill, Gtk.AttachOptions.Shrink,
                0, 0
            );
            p_entry.ShowAll();
            return p_entry;
        }
        
        /// <summary>
        /// Function called when the window is destroyed
        /// </summary>
        /// <param name="o">Object</param>
        /// <param name="e">Delete event arguments</param>
        public void OnDelete(object o, DeleteEventArgs e) {
            Application.Quit();
        }
    }
}
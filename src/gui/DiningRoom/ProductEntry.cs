using Gtk;
using System;

namespace Restaurant {
    internal class ProductEntry: Gtk.HBox {
        internal string p_name {get; private set;}
        Gtk.Label name_label;
        Gtk.Label amount_label = null;
        ImageAction image;

        ProductListFunc add_product;
        ProductListFunc rem_product;

        internal ProductEntry(string p_name, ProductListFunc add_handler): base(false, 0) {
            this.p_name = p_name;
            this.add_product = add_handler;
            this.name_label = new Gtk.Label(p_name);
            this.name_label.SetSizeRequest(200, 25);
            this.image = new ImageAction(Gtk.Stock.Add, Gtk.IconSize.Button);
            this.image.SetSizeRequest(50, 25);

            this.Add(this.name_label);
            this.Add(this.image);
            this.image.ButtonReleaseEvent += this.AddProductToOrder;
        }

        internal ProductEntry(string p_name, uint amount, ProductListFunc rem_handler): base(false, 0) {
            this.p_name = p_name;
            this.rem_product = rem_handler;
            this.name_label = new Gtk.Label(p_name);
            this.amount_label = new Gtk.Label(String.Format("{0}", amount));
            this.image = new ImageAction(Gtk.Stock.Remove, Gtk.IconSize.Button);

            this.Add(this.name_label);
            this.Add(this.amount_label);
            this.Add(this.image);
            this.image.ButtonReleaseEvent += this.RemProductFromOrder;
        }

        internal void ChangeAmount(int change) {
            if (this.amount_label != null) {
                this.amount_label.Text = String.Format("{0}", uint.Parse(this.amount_label.Text) + change);
            }
        }

        internal void AddProductToOrder(object e, Gtk.ButtonReleaseEventArgs args) {
            this.add_product(this.p_name);
        }
    
        internal void RemProductFromOrder(object e, Gtk.ButtonReleaseEventArgs args) {
            this.rem_product(this.p_name);
        }
    }
}
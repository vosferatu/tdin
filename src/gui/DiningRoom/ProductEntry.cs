using Gtk;
using System;

namespace Restaurant {
    /// <summary>
    /// Represents a single product entry in the Dining Room window
    /// </summary>
    internal class ProductEntry: Gtk.Table {
        internal string p_name {get; private set;}
        Gtk.Label price_label;
        Gtk.Label name_label;
        Gtk.Label amount_label = null;
        ImageAction image;

        ProductListFunc add_product;
        ProductListFunc rem_product;

        /// <summary>
        /// Creates a new Product Entry
        /// </summary>
        /// <param name="p_name">Name of the product</param>
        /// <param name="price">Price of the product</param>
        /// <param name="add_handler">Handler to be called when the button is clicked</param>
        /// <returns></returns>
        internal ProductEntry(string p_name, double price, ProductListFunc add_handler): base(1, 3, false) {
            this.p_name = p_name;
            this.add_product = add_handler;
            this.price_label = new Gtk.Label(String.Format("{0}€", price));
            this.price_label.SetSizeRequest(80, 25);
            this.name_label = new Gtk.Label(p_name);
            this.name_label.SetSizeRequest(150, 25);
            this.image = new ImageAction(Gtk.Stock.Add, Gtk.IconSize.Button);
            this.image.SetSizeRequest(30, 25);
            this.name_label.Xalign = 0f;
            this.price_label.Xalign = 0.5f;
            this.price_label.Markup = String.Format("<small>{0}</small>", this.price_label.Text);
            this.price_label.UseMarkup = true;
            this.Attach(this.name_label,
                0, 1, 0, 1,
                Gtk.AttachOptions.Expand | Gtk.AttachOptions.Fill, Gtk.AttachOptions.Shrink,
                0, 0    
            );
            this.Attach(this.price_label,
                1, 2, 0, 1,
                Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink,
                0, 0    
            );
            this.Attach(this.image,
                2, 3, 0, 1,
                Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink,
                0, 0    
            );
            this.image.ButtonReleaseEvent += this.AddProductToOrder;
            this.ShowAll();
        }

        /// <summary>
        /// Creates a new ProductEntry but with a remove handler
        /// </summary>
        /// <param name="p_name">Name of the product</param>
        /// <param name="amount">Amount of the product</param>
        /// <param name="rem_handler">Handler to be called when the 'Remove' button is clicked</param>
        /// <returns></returns>
        internal ProductEntry(string p_name, uint amount, ProductListFunc rem_handler): base(1, 3, false) {
            this.p_name = p_name;
            this.rem_product = rem_handler;
            this.name_label = new Gtk.Label(p_name);
            this.name_label.SetSizeRequest(140, 25);
            this.name_label.Xalign = 0f;
            this.amount_label = new Gtk.Label(amount.ToString());
            this.amount_label.SetSizeRequest(90, 25);
            this.amount_label.Xalign = 0.5f;
            this.amount_label.Markup = String.Format("<small>{0}</small>", this.amount_label.Text);
            this.image = new ImageAction(Gtk.Stock.Remove, Gtk.IconSize.Button);
            this.image.SetSizeRequest(35, 25);

            this.Attach(this.name_label,
                0, 1, 0, 1,
                Gtk.AttachOptions.Expand | Gtk.AttachOptions.Fill, Gtk.AttachOptions.Shrink,
                0, 0    
            );
            this.Attach(this.amount_label,
                1, 2, 0, 1,
                Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink,
                0, 0    
            );
            this.Attach(this.image,
                2, 3, 0, 1,
                Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink,
                0, 0    
            );
            this.image.ButtonReleaseEvent += this.RemProductFromOrder;
            this.ShowAll();
        }

        /// <summary>
        /// Changes the amount of the product
        /// </summary>
        /// <param name="change">Actual change (either 1 or -1)</param>
        internal void ChangeAmount(int change) {
            if (this.amount_label != null) {
                this.amount_label.Markup = String.Format("<small>{0}</small>", uint.Parse(this.amount_label.Text) + change);
            }
        }

        /// <summary>
        /// Function linked with 'Add' Gtk EventBox event
        /// </summary>
        /// <param name="e">Object that called the method</param>
        /// <param name="args">Arguments of the event</param>
        internal void AddProductToOrder(object e, Gtk.ButtonReleaseEventArgs args) {
            this.add_product(this.p_name, true);
        }

        /// <summary>
        /// Function linked with 'Remove' Gtk EventBox event
        /// </summary>
        /// <param name="e">Object that called the method</param>
        /// <param name="args">Arguments of the event</param>
        internal void RemProductFromOrder(object e, Gtk.ButtonReleaseEventArgs args) {
            this.rem_product(this.p_name, true);
        }
    }
}
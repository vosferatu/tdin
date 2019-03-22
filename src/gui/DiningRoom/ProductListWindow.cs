using Gtk;
using Gdk;
using Glade;
using System;
using System.Collections.Generic;

namespace Restaurant {
    public delegate void ProductListFunc(string p_name);
    public delegate void SimpleFunction();
    
    public class ProductListWindow {
        private Gdk.Color GREEN = new Gdk.Color(153, 255, 153);
        private const string WINDOW_FILE = GuiConstants.WINDOWS_DIR + "ProductPicker.glade";
        private const string WINDOW_NAME = "root";
        
        [Glade.Widget]
        Gtk.Window root;

        [Glade.Widget]
        Gtk.Table DishProductList;
        [Glade.Widget]
        Gtk.Table DrinkProductList;
        [Glade.Widget]
        Gtk.Table DishOrderList;
        [Glade.Widget]
        Gtk.Table DrinkOrderList;


        ProductListFunc add_p;
        ProductListFunc rem_p;
        SimpleFunction submit;
        SimpleFunction undo;

        public ProductListWindow(ProductListFunc add_product, ProductListFunc remove_product, 
            SimpleFunction submit, SimpleFunction undo) 
        {
            this.add_p = add_product;
            this.rem_p = remove_product;
            this.submit = submit;
            this.undo = undo;
        }
        
        public void StartThread() {
            Glade.XML gxml = new Glade.XML(WINDOW_FILE, WINDOW_NAME, null);
            gxml.Autoconnect(this);
            this.root.ShowAll();
        }
    
        public void SetProducts(List<string> dishes, List<string> drinks) {
            foreach(string dish in dishes) {
                uint child_n = (uint)this.DishOrderList.Children.Length;
                ProductEntry new_entry = new ProductEntry(dish, this.add_p);
                this.DishProductList.Attach(new_entry, 0, 1, 0 + child_n, 1 + child_n,
                    Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink, 0, 0);
            }
            foreach(string drink in drinks) {
                uint child_n = (uint)this.DishOrderList.Children.Length;
                ProductEntry new_entry = new ProductEntry(drink, this.add_p);
                this.DrinkProductList.Attach(new_entry, 0, 1, 0 + child_n, 1 + child_n,
                    Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink, 0, 0);
            }
        }

        public void AddProduct(string p_name) {
            Console.WriteLine("Adding product '{0}'", p_name);
        }

        public void RemProduct(string p_name) {
            Console.WriteLine("Removing product '{1}'", p_name);
        }

        public void OnUndo(object o, EventArgs args) {
            this.undo();
        }

        public void OnSubmit(object o, EventArgs args) {
            this.submit();
        }
    }
}
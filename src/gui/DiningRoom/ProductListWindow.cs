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
        public Gtk.Window root;

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
            Application.Init();
            Glade.XML gxml = new Glade.XML(WINDOW_FILE, WINDOW_NAME, null);
            gxml.Autoconnect(this);
            Application.Run();
        }
    
        public void SetProducts(List<string> dishes, List<string> drinks) {
            Console.WriteLine("Adding products");
            uint child_n = (uint)this.DishOrderList.Children.Length;
            foreach(string dish in dishes) {
                ProductEntry new_entry = new ProductEntry(dish, this.add_p);
                this.DishProductList.Attach(new_entry, 0, 1, 0 + child_n, 1 + child_n,
                    Gtk.AttachOptions.Expand, Gtk.AttachOptions.Shrink, 0, 2);
                child_n++;
            }
            child_n = (uint)this.DishOrderList.Children.Length;
            foreach(string drink in drinks) {
                ProductEntry new_entry = new ProductEntry(drink, this.add_p);
                this.DrinkProductList.Attach(new_entry, 0, 1, 0 + child_n, 1 + child_n,
                    Gtk.AttachOptions.Expand, Gtk.AttachOptions.Shrink, 0, 2);
                child_n++;
            }
            this.root.ShowAll();
        }

        public void AddProduct(string name, bool is_dish) {
            Gtk.Table table = (is_dish ? this.DishOrderList : this.DrinkOrderList);
            uint child_n = (uint)table.Children.Length;
            ProductEntry entry = new ProductEntry(name, 1, this.rem_p);
            table.Attach(entry, 0, 1, 0 + child_n, 1 + child_n,
                Gtk.AttachOptions.Expand, Gtk.AttachOptions.Shrink, 0, 2);
            this.root.ShowAll();
        }

        public void RemoveProduct(string name, bool is_dish) {
            Gtk.Table table = (is_dish ? this.DishOrderList : this.DrinkOrderList);
            table.Foreach((Gtk.Widget widget) => {
                ProductEntry entry = (ProductEntry)widget;
                if (entry.p_name == name) {
                    table.Remove(widget);
                }
            });
            this.root.ShowAll();
        } 
        
        public void ChangeAmount(string name, int change, bool is_dish) {
            Gtk.Table table = (is_dish ? this.DishOrderList : this.DrinkOrderList);
            table.Foreach((Gtk.Widget widget) => {
                ProductEntry entry = (ProductEntry)widget;
                if (entry.p_name == name) {
                    entry.ChangeAmount(change);
                }
            });
            this.root.ShowAll();
        }

        public void OnUndo(object o, EventArgs args) {
            this.undo();
            this.root.ShowAll();
        }

        public void OnSubmit(object o, EventArgs args) {
            this.submit();
            this.root.ShowAll();
        }
    
        public void OnDelete(object o, DeleteEventArgs e) {
            Console.WriteLine("Quitting!!");
            Application.Quit();
        }
    }
}
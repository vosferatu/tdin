using Gtk;
using Gdk;
using Glade;
using System;
using System.Collections.Generic;

namespace Restaurant {
    public delegate void ProductListFunc(string p_name, bool add_history);
    public delegate void SimpleFunction();
    
    public class ProductListWindow {
    #region FIELDS
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
        [Glade.Widget]
        public Gtk.SpinButton TableNumber;
        [Glade.Widget]
        Gtk.Table OrderReadyBox;


        ProductListFunc add_p;
        ProductListFunc rem_p;
        SimpleFunction submit;
        SimpleFunction undo;
    #endregion FIELDS

    #region METHODS
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
            this.root.SetIconFromFile(GuiConstants.APP_ICON);
            Application.Run();
        }
    
        public void SetProducts(List<Tuple<string, double>> dishes, List<Tuple<string, double>> drinks) {
            uint child_n = (uint)this.DishOrderList.Children.Length;
            foreach(Tuple<string, double> dish in dishes) {
                ProductEntry new_entry = new ProductEntry(dish.Item1, dish.Item2, this.add_p);
                this.DishProductList.Attach(new_entry, 
                    0, 1, 0 + child_n, 1 + child_n,
                    Gtk.AttachOptions.Expand, Gtk.AttachOptions.Shrink,
                    0, 0
                );
                child_n++;
            }
            child_n = (uint)this.DishOrderList.Children.Length;
            foreach(Tuple<string, double> drink in drinks) {
                ProductEntry new_entry = new ProductEntry(drink.Item1, drink.Item2, this.add_p);
                this.DrinkProductList.Attach(new_entry, 
                    0, 1, 0 + child_n, 1 + child_n,
                    Gtk.AttachOptions.Expand, Gtk.AttachOptions.Shrink, 
                    0, 0
                );
                child_n++;
            }
            this.root.ShowAll();
        }

        // TODO: 
        // Add at least 2 products to your order
        // Completelly delete one of the products from your order
        // Add a new product to your order
        // EXPECTED: New product added below existing one
        // HAPPENS:  New product added on top of existing one
        public void AddProduct(string name, bool is_dish) {
            Gtk.Table table = (is_dish ? this.DishOrderList : this.DrinkOrderList);
            uint child_n = (uint)table.Children.Length;
            ProductEntry entry = new ProductEntry(name, 1, this.rem_p);
            table.Attach(entry, 
                0, 1, 0 + child_n, 1 + child_n,
                Gtk.AttachOptions.Expand | Gtk.AttachOptions.Fill, Gtk.AttachOptions.Shrink,
                0, 0
            );
            entry.ShowAll();
        }

        public void RemoveProduct(string name, bool is_dish) {
            Gtk.Table table = (is_dish ? this.DishOrderList : this.DrinkOrderList);
            foreach(Gtk.Widget widget in table) {
                if (((ProductEntry)widget).p_name == name) {
                    widget.HideAll();
                    table.Remove(widget);
                    break;
                }
            }
        } 
        
        public void ChangeAmount(string name, int change, bool is_dish) {
            Gtk.Table table = (is_dish ? this.DishOrderList : this.DrinkOrderList);
            foreach(Gtk.Widget widget in table) {
                ProductEntry entry = (ProductEntry)widget;
                if (entry.p_name == name) {
                    entry.ChangeAmount(change);
                    break;
                }
            }
        }

        public void OnUndo(object o, EventArgs args) {
            this.undo();
            this.root.ShowAll();
        }

        public void OnSubmit(object o, EventArgs args) {
            this.submit();
            this.root.ShowAll();
        }

        public void ResetOrder() {
            this.DishOrderList.Foreach((Gtk.Widget widget) => {
                this.DishOrderList.Remove(widget);
            });

            this.DrinkOrderList.Foreach((Gtk.Widget widget) => {
                this.DrinkOrderList.Remove(widget);
            });
        }

        public void OrderReady(long order_id, OrderDelivered handler) {
            OrderReadyEntry new_entry = new OrderReadyEntry(order_id, handler);
            uint child_n = (uint)this.OrderReadyBox.Children.Length;
            this.OrderReadyBox.Attach(new_entry,
                0, 1, 0+child_n, 1+child_n,
                Gtk.AttachOptions.Expand, Gtk.AttachOptions.Shrink,
                0, 10
            );
        }

        public void FinishOrder(long order_id) {
            foreach(Widget widget in this.OrderReadyBox) {
                OrderReadyEntry entry = (OrderReadyEntry)widget;
                if (entry.order_id == order_id) {
                    this.OrderReadyBox.Remove(widget);
                    break;
                }
            }
        }

        public void OnDelete(object o, DeleteEventArgs e) {
            Application.Quit();
        }
    
    #endregion METHODS
    }
}

namespace Restaurant {
    public delegate void OrderDelivered(long order_id);

    public class OrderReadyEntry: Gtk.HBox {
        internal long order_id {get; private set;}
        OrderDelivered handler;
        Gtk.Label order_name;
        ImageAction finish;


        public OrderReadyEntry(long order_id, OrderDelivered func) {
            this.order_id = order_id;
            this.handler = func;
            this.order_name = new Gtk.Label(String.Format("<big>{0}</big>", order_id));
            this.order_name.UseMarkup = true;
            this.finish = new ImageAction(Gtk.Stock.Apply, Gtk.IconSize.SmallToolbar);
            this.finish.ButtonReleaseEvent += this.Finished;

            this.order_name.SetSizeRequest(35, 20);

            this.Add(this.order_name);
            this.Add(this.finish);
            this.ShowAll();
        }

        private void Finished(object e, Gtk.ButtonReleaseEventArgs args) {
            this.handler(this.order_id);
        }
    }
}
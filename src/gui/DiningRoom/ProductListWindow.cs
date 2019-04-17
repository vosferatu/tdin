using Gtk;
using Gdk;
using Base;
using Glade;
using System;
using System.Collections.Generic;

namespace Restaurant {
    public delegate void ProductListFunc(string p_name, bool add_history);
    
    /// <summary>
    /// Responsible for communicating directly with Gtk and handling its events for the Dining Room window
    /// </summary>
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
        public Gtk.ComboBox TableNumber;
        [Glade.Widget]
        Gtk.Table OrderReadyBox;


        ProductListFunc add_p;
        ProductListFunc rem_p;
        SimpleFunction submit;
        SimpleFunction undo;
    #endregion FIELDS

    #region METHODS
        /// <summary>
        /// Creates a new Product List Window
        /// </summary>
        /// <param name="add_product">Handler to be called when a new product is added to the order product list</param>
        /// <param name="remove_product">Handler to be called when a product is removed from the order product list</param>
        /// <param name="submit">Handler to be called when 'Submit' button is pressed</param>
        /// <param name="undo">Handler to be called when the 'Undo' button is pressed</param>
        public ProductListWindow(ProductListFunc add_product, ProductListFunc remove_product, 
            SimpleFunction submit, SimpleFunction undo) 
        {
            this.add_p = add_product;
            this.rem_p = remove_product;
            this.submit = submit;
            this.undo = undo;
        }
        
        /// <summary>
        /// Starts the actual thread of the Gtk Application
        /// </summary>
        public void StartThread() {
            Glade.XML gxml = new Glade.XML(WINDOW_FILE, WINDOW_NAME, null);
            gxml.Autoconnect(this);
            this.root.SetIconFromFile(GuiConstants.APP_ICON);
            this.TableNumber.Active = 0;
            Application.Run();
        }

        /// <summary>
        /// Sets the products to be displayed to the user
        /// </summary>
        /// <param name="dishes">List of available dishes</param>
        /// <param name="drinks">List of available drinks</param>
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
        /// <summary>
        /// Adds a new product to the product order list frame
        /// </summary>
        /// <param name="name">Name of product to be added</param>
        /// <param name="is_dish">Whether the product is a dish or not</param>
        public void AddProduct(string name, bool is_dish) {
            Gtk.Table table = (is_dish ? this.DishOrderList : this.DrinkOrderList);
            uint child_n = (uint)table.Children.Length;
            ProductEntry entry = new ProductEntry(name, 1, this.rem_p);

            table.Attach(entry, 
                0, 1, 0 + child_n, 1 + child_n,
                Gtk.AttachOptions.Expand | Gtk.AttachOptions.Fill, Gtk.AttachOptions.Shrink,
                0, 0
            );
        }

        /// <summary>
        /// Removes a product from the products list of the order
        /// </summary>
        /// <param name="name">Name of product</param>
        /// <param name="is_dish">Whether the product is a dish or drink</param>
        public void RemoveProduct(string name, bool is_dish) {
            Gtk.Table table = (is_dish ? this.DishOrderList : this.DrinkOrderList);
            foreach(Gtk.Widget widget in table)
                if (widget is ProductEntry && ((ProductEntry)widget).p_name == name)
                    widget.Destroy();
        } 
        
        /// <summary>
        /// Changes the amount of a product in the order
        /// </summary>
        /// <param name="name">Name of the product</param>
        /// <param name="change">Amount to be added to the product amount, can be 1 or -1</param>
        /// <param name="is_dish">Whether the product is a dish or not</param>
        public void ChangeAmount(string name, int change, bool is_dish) {
            Gtk.Table table = (is_dish ? this.DishOrderList : this.DrinkOrderList);
            foreach(Gtk.Widget widget in table) {
                if (widget is ProductEntry) {
                    ProductEntry entry = (ProductEntry)widget;
                    if (entry.p_name == name)
                        entry.ChangeAmount(change);
                }
            }
        }

        /// <summary>
        /// Function called when button 'Undo' is clicked
        /// </summary>
        /// <param name="o">Object that called the function</param>
        /// <param name="args">Arguments of the event</param>
        public void OnUndo(object o, EventArgs args) {
            this.undo();
            this.root.ShowAll();
        }

        /// <summary>
        /// Function called when button 'Submit' was clicked
        /// </summary>
        /// <param name="o">Object that called the function</param>
        /// <param name="args">Arguments of the event</param>
        public void OnSubmit(object o, EventArgs args) {
            this.submit();
            this.root.ShowAll();
        }

        /// <summary>
        /// Resets the product order list back to its default state (empty)
        /// </summary>
        public void ResetOrder() {
            this.DishOrderList.Foreach((Gtk.Widget widget) => {
                this.DishOrderList.Remove(widget);
            });

            this.DrinkOrderList.Foreach((Gtk.Widget widget) => {
                this.DrinkOrderList.Remove(widget);
            });
        }

        /// <summary>
        /// Signals that the order is ready
        /// </summary>
        /// <param name="order_id">ID of the order that is ready</param>
        /// <param name="handler">Function to be called when the order is delivered to the table</param>
        public void OrderReady(long order_id, OrderDelivered handler) {
            OrderReadyEntry new_entry = new OrderReadyEntry(order_id, handler);
            uint child_n = (uint)this.OrderReadyBox.Children.Length;
            this.OrderReadyBox.Attach(new_entry,
                0, 1, 0+child_n, 1+child_n,
                Gtk.AttachOptions.Expand, Gtk.AttachOptions.Shrink,
                0, 10
            );
        }

        /// <summary>
        /// Finishes the order, removing it from the window
        /// </summary>
        /// <param name="order_id">ID of order to be finished</param>
        public void FinishOrder(long order_id) {
            foreach(Widget widget in this.OrderReadyBox) {
                OrderReadyEntry entry = (OrderReadyEntry)widget;
                if (entry.order_id == order_id) {
                    this.OrderReadyBox.Remove(widget);
                    break;
                }
            }
        }

        /// <summary>
        /// Function called when the window is destroyed
        /// </summary>
        /// <param name="o">Object</param>
        /// <param name="e">Delete event arguments</param>
        public void OnDelete(object o, DeleteEventArgs e) {
            Application.Quit();
        }
    
    #endregion METHODS
    }
}

namespace Restaurant {
    /// <summary>
    /// Function that represents when an order is delivered to a table
    /// </summary>
    /// <param name="order_id">ID of the delivered order</param>
    public delegate void OrderDelivered(long order_id);

    /// <summary>
    /// Represents an entry in the Order Ready frame
    /// </summary>
    public class OrderReadyEntry: Gtk.Table {
        internal long order_id {get; private set;}
        OrderDelivered handler;
        Gtk.Label order_name;
        ImageAction finish;

        /// <summary>
        /// Creates a new entry
        /// </summary>
        /// <param name="order_id">ID of the ready order</param>
        /// <param name="func">Function to be called when the 'ready' button is pressed</param>
        public OrderReadyEntry(long order_id, OrderDelivered func): base(1, 2, false) {
            this.order_id = order_id;
            this.handler = func;
            this.order_name = new Gtk.Label(String.Format("<big>{0}</big>", order_id));
            this.order_name.UseMarkup = true;
            this.finish = new ImageAction(Gtk.Stock.Apply, Gtk.IconSize.SmallToolbar);
            this.finish.ButtonReleaseEvent += this.Finished;

            this.order_name.SetSizeRequest(35, 20);

            this.Attach(this.order_name, 0, 1, 0, 1,
                Gtk.AttachOptions.Expand | Gtk.AttachOptions.Fill, Gtk.AttachOptions.Shrink, 0, 0
            );
            this.Attach(this.finish, 0, 1, 0, 1,
                Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink, 0, 0
            );
            this.ShowAll();
        }

        /// <summary>
        /// Function called when the order is finished
        /// </summary>
        /// <param name="o">Object</param>
        /// <param name="e">Delete event arguments</param>
        private void Finished(object e, Gtk.ButtonReleaseEventArgs args) {
            this.handler(this.order_id);
        }
    }
}
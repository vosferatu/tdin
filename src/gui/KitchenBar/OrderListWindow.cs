using Gtk;
using System;
using System.Collections.Generic;

namespace Restaurant {
    public delegate void OrderListFunc(long order_id);

    public class OrderListWindow {
        private const string WINDOW_FILE = GuiConstants.WINDOWS_DIR + "KitchenBar.glade";
        private const string WINDOW_NAME = "root";

        [Glade.Widget]
        public Gtk.Window root;
        [Glade.Widget]
        Gtk.Table NotPickedBox;
        [Glade.Widget]
        Gtk.Table PreparingBox;

        OrderListFunc view_handler;
        OrderListFunc prepare_handler;
        OrderListFunc done_handler;

        public OrderListWindow(OrderListFunc view_handler,  OrderListFunc prepare_handler,  OrderListFunc done_handler) {
            this.view_handler = view_handler;
            this.prepare_handler = prepare_handler;
            this.done_handler = done_handler;
        }

        public void StartThread() {
            Glade.XML gxml = new Glade.XML(WINDOW_FILE, WINDOW_NAME, null);
            gxml.Autoconnect(this);
            this.root.SetIconFromFile(GuiConstants.APP_ICON);
            Application.Run();
        }

        public void AddOrders(List<long> orders_id, bool picked) {
            Gtk.Table box = (picked ? this.PreparingBox : this.NotPickedBox);
            foreach(long order_id in orders_id) {
                this.AddOrderToBox(order_id, box, picked);
            }
            this.root.ShowAll();
        }

        public void AddOrder(long order_id) {
            this.AddOrderToBox(order_id, this.NotPickedBox, false);
            this.root.ShowAll();
        }

        public bool RemoveOrder(long order_id) {
            bool removed = false;
            PreparingBox.Foreach((child) => {
                OrderEntry widget = (OrderEntry)child;
                if (widget.order_id == order_id) {
                    PreparingBox.Remove(child);
                    removed = true;
                }
            });

            return removed;
        }

        public bool PickOrder(long order_id) {
            bool picked = false;
            NotPickedBox.Foreach((child) => {
                OrderEntry widget = (OrderEntry)child;
                if (widget.order_id == order_id) {
                    widget.OrderMoved();
                    widget.SetHandlers(this.view_handler, this.done_handler);
                    NotPickedBox.Remove(child);
                    uint child_n = (uint)this.PreparingBox.Children.Length;
                    PreparingBox.Attach(child, 0, 1, 0+child_n, 1+child_n, Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink, 0, 3);
                    picked = true;
                }
            });

            return picked;
        }

        private void AddOrderToBox(long order_id, Gtk.Table box, bool picked) {
            uint child_n = (uint)box.Children.Length;
            OrderEntry new_entry = new OrderEntry(order_id, picked);
            new_entry.SetHandlers(this.view_handler, (picked ? this.done_handler : this.prepare_handler));
            box.Attach(new_entry, 
                0, 1, 0+child_n, 1+child_n, 
                Gtk.AttachOptions.Expand | Gtk.AttachOptions.Fill, Gtk.AttachOptions.Shrink,
                0, 3
            );
            box.ShowAll();
        }
    
        public void OnDelete(object o, DeleteEventArgs e) {
            Application.Quit();
        }
    }
}

namespace Restaurant {
    /// <summary>
    /// Represents a single order entry in the Kitchen Bar window.
    /// </summary>
    internal class OrderEntry: Gtk.HBox {
        internal long order_id {get; private set;}
        OrderListFunc label_handler;
        OrderListFunc img_handler;
        OrderLabel label;
        ImageAction img;

        /// <summary>
        /// Creates a new OrderEntry
        /// </summary>
        /// <param name="order_id">ID of the order</param>
        /// <param name="picked">Whether the order has been picked or not</param>
        /// <returns></returns>
        internal OrderEntry(long order_id, bool picked): base(false, 0) {
            this.order_id = order_id;
            this.label = new OrderLabel(String.Format("Order #{0}", order_id));
            if (!picked) {
                this.img = new ImageAction(Gtk.Stock.GoForward, Gtk.IconSize.Button);
            }
            else {
                this.img = new ImageAction(Gtk.Stock.Apply, Gtk.IconSize.Button);
            }
            this.SetSizeRequest(200, 40);
            this.Add(this.label);
            this.Add(this.img);
            this.label.ButtonReleaseEvent += this.LabelReleaseFunc;
            this.img.ButtonReleaseEvent += this.ImgReleaseFunc;
        }

        /// <summary>
        /// Sets the handlers of the label
        /// These are functions to be called on each clicked event
        /// </summary>
        /// <param name="label_handler">Handler to be called when clicking the label</param>
        /// <param name="img_handler">Handler to be called when clicking the image</param>
        internal void SetHandlers(OrderListFunc label_handler, OrderListFunc img_handler) {
            this.label_handler = label_handler;
            this.img_handler = img_handler;
        }

        /// <summary>
        /// Signals that this order has been moved
        /// </summary>
        internal void OrderMoved() {
            this.img.SetImage(Gtk.Stock.Apply, Gtk.IconSize.Button);
        }

        /// <summary>
        /// Function called when the label of the entry is clicked
        /// </summary>
        /// <param name="e">Object that called the function</param>
        /// <param name="args">Arguments of the event</param>
        internal void LabelReleaseFunc(object e, Gtk.ButtonReleaseEventArgs args) {
            this.label_handler(this.order_id);
        }

        /// <summary>
        /// Function called when the image of the entry is clicked
        /// </summary>
        /// <param name="e">Object that called the function</param>
        /// <param name="args">Arguments of the event</param>
        internal void ImgReleaseFunc(object e, Gtk.ButtonReleaseEventArgs args) {
            this.img_handler(this.order_id);
        }

    }
    
    /// <summary>
    /// Represents a simple label that has an associated event to be fired on click
    /// </summary>
    internal class OrderLabel: Gtk.EventBox {
        private Gtk.Label label;

        /// <summary>
        /// Creates the label
        /// </summary>
        /// <param name="text">Text label</param>
        internal OrderLabel(string text) {
            this.label = new Gtk.Label(text);
            this.label.Justify = Gtk.Justification.Left;
            this.label.LineWrap = true;
            this.Add(this.label);
            this.SetSizeRequest(120, 0);
        }
    }
}
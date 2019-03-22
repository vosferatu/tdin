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
            this.root.ShowAll();
        }

        public void AddOrders(List<Tuple<long, string>> orders, bool picked) {
            Gtk.Table box = (picked ? this.PreparingBox : this.NotPickedBox);
            foreach(Tuple<long, string> order in orders) {
                this.AddOrderToBox(order, box, picked);
            }
            this.root.ShowNow();
        }

        public void AddOrder(Tuple<long, string> order) {
            this.AddOrderToBox(order, this.NotPickedBox, false);
            this.root.ShowNow();
            Application.RunIteration(false);
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

        private void AddOrderToBox(Tuple<long, string> order, Gtk.Table box, bool picked) {
            uint child_n = (uint)box.Children.Length;
            OrderEntry new_entry = new OrderEntry(order.Item1, order.Item2, picked);
            new_entry.SetHandlers(this.view_handler, (picked ? this.done_handler : this.prepare_handler));
            box.Attach(new_entry, 0, 1, 0+child_n, 1+child_n, Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink, 0, 3);
            box.ShowAll();
        }
    
        public void OnDelete(object o, DeleteEventArgs e) {
            Console.WriteLine("Deleting window!");
            Application.Quit();
        }
    }
}

namespace Restaurant {
    internal class OrderEntry: Gtk.HBox {
        internal long order_id {get; private set;}
        OrderListFunc label_handler;
        OrderListFunc img_handler;
        OrderLabel label;
        ImageAction img;

        internal OrderEntry(long order_id, string desc, bool picked): base(false, 0) {
            this.order_id = order_id;
            this.label = new OrderLabel(desc);
            if (!picked) {
                this.img = new ImageAction(Gtk.Stock.GoForward, Gtk.IconSize.Button);
            }
            else {
                this.img = new ImageAction(Gtk.Stock.Apply, Gtk.IconSize.Button);
            }
            this.SetSizeRequest(150, 50);
            this.Add(this.label);
            this.Add(this.img);
            this.label.ButtonReleaseEvent += this.LabelReleaseFunc;
            this.img.ButtonReleaseEvent += this.ImgReleaseFunc;
        }

        internal void SetHandlers(OrderListFunc label_handler, OrderListFunc img_handler) {
            this.label_handler = label_handler;
            this.img_handler = img_handler;
        }

        internal void OrderMoved() {
            this.img.SetImage(Gtk.Stock.Apply, Gtk.IconSize.Button);
        }

        internal void LabelReleaseFunc(object e, Gtk.ButtonReleaseEventArgs args) {
            this.label_handler(this.order_id);
        }

        internal void ImgReleaseFunc(object e, Gtk.ButtonReleaseEventArgs args) {
            this.img_handler(this.order_id);
        }

    }
    
    internal class OrderLabel: Gtk.EventBox {
        private Gtk.Label label;

        internal OrderLabel(string text) {
            this.label = new Gtk.Label(text);
            this.label.Justify = Gtk.Justification.Left;
            this.label.LineWrap = true;
            this.Add(this.label);
            this.SetSizeRequest(120, 0);
        }
    }
}
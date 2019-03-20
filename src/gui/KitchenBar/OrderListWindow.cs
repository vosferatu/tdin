using Gtk;
using Glade;
using System;
using System.Collections.Generic;

namespace Restaurant {
    public delegate void ButtonFunc(long order_id);

    public class OrderListWindow {
        private const string WINDOW_FILE = GuiConstants.WINDOWS_DIR + "KitchenBar.glade";
        private const string WINDOW_NAME = "root";

        [Glade.Widget]
        public Gtk.Window root;
        [Glade.Widget]
        VBox NotPickedBox;
        [Glade.Widget]
        VBox PreparingBox;

        ButtonFunc view_handler;
        ButtonFunc prepare_handler;
        ButtonFunc done_handler;

        public OrderListWindow(ButtonFunc view_handler,  ButtonFunc prepare_handler,  ButtonFunc done_handler) {
            this.view_handler = view_handler;
            this.prepare_handler = prepare_handler;
            this.done_handler = done_handler;
        }

        public void StartThread() {
            Console.WriteLine("Starting OrderListWindow Thread");
            Glade.XML gxml = new Glade.XML(WINDOW_FILE, WINDOW_NAME, null);
            gxml.Autoconnect(this);
            this.root.ShowAll();
        }

        public void AddOrders(List<Tuple<long, string>> orders, bool picked) {
            VBox box = (picked ? this.PreparingBox : this.NotPickedBox);
            foreach(Tuple<long, string> order in orders) {
                this.AddOrderToBox(order, box, picked);
            }
            this.root.ShowNow();
        }

        public void AddOrder(Tuple<long, string> order) {
            Console.WriteLine("Adding Order");
            this.AddOrderToBox(order, this.NotPickedBox, false);
            this.root.ShowNow();
            Application.RunIteration(false);
            Console.WriteLine("Ran iteration");
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
                    NotPickedBox.Remove(child);
                    PreparingBox.PackEnd(child);
                    picked = true;
                }
            });

            return picked;
        }

        private void AddOrderToBox(Tuple<long, string> order, Gtk.VBox box, bool picked) {
            OrderEntry new_entry = new OrderEntry(order.Item1, order.Item2, picked);
            new_entry.SetHandlers(this.view_handler, (picked ? this.done_handler : this.prepare_handler));
            box.PackStart(new_entry);
            new_entry.ShowAll();
        }
    
        public void OnDelete(object o, DeleteEventArgs e) {
            Console.WriteLine("Deleting window!");
            Application.Quit();
        }
    }
}

namespace Restaurant {
    public class OrderEntry: Gtk.HBox {
        public long order_id {get; private set;}
        ButtonFunc label_handler;
        ButtonFunc img_handler;
        Gtk.EventBox label;
        Gtk.EventBox img;

        public OrderEntry(long order_id, string order_desc, bool picked): base(false, 0) {
            this.order_id = order_id;
            this.label = new OrderLabel(order_desc);
            if (!picked) {
                this.img = new OrderImage(Gtk.Stock.GoForward, Gtk.IconSize.Button);
            }
            else {
                this.img = new OrderImage(Gtk.Stock.Apply, Gtk.IconSize.Button);
            }

            this.Add(this.label);
            this.Add(this.img);
            this.label.ButtonReleaseEvent += this.LabelReleaseFunc;
            this.label.ButtonReleaseEvent += this.ImgReleaseFunc;
        }
    
        public void SetHandlers(ButtonFunc label_handler, ButtonFunc img_handler) {
            this.label_handler = label_handler;
            this.img_handler = img_handler;
        }

        public void LabelReleaseFunc(object e, Gtk.ButtonReleaseEventArgs args) {
            this.label_handler(this.order_id);
        }

        public void ImgReleaseFunc(object e, Gtk.ButtonReleaseEventArgs args) {
            this.img_handler(this.order_id);
        }

    }
    
    internal class OrderLabel: Gtk.EventBox {
        private Gtk.Label label;

        internal OrderLabel(string text) {
            this.label = new Gtk.Label(text);
            this.Add(this.label);
        }
    }

    internal class OrderImage: Gtk.EventBox {
        private Gtk.Image image;

        internal OrderImage(string stock, Gtk.IconSize type) {
            this.image = new Gtk.Image(stock, type);
            this.Add(this.image);
        }
    }
}
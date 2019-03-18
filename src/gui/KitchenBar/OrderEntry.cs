using System;
using Gtk;
using Gdk;

namespace Restaurant {
    public class OrderEntry: Gtk.HBox {
        long order_id;
        Gtk.EventBox label;
        Gtk.EventBox img;

        public OrderEntry(long order_id, string order_desc, bool picked): base(false, 0) {
            this.order_id = order_id;
            this.label = new OrderLabel(order_desc);
            if (!picked) {
                this.img = new PrepareOrder();
            }
            else {
                this.img = new OrderDone();
            }

            this.Add(this.label);
            this.Add(this.img);
            this.Show();
        }
    
        public void SetHandlers(Gtk.ButtonReleaseEventHandler label_handler, Gtk.ButtonReleaseEventHandler img_handler) {
            this.label.ButtonReleaseEvent += label_handler;
            this.img.ButtonReleaseEvent += img_handler;
        }
    }

    internal class OrderDone: Gtk.EventBox {
        private Gtk.Image image;

        internal OrderDone() {
            this.image = new Gtk.Image(Gtk.Stock.Apply, Gtk.IconSize.Button);
            this.Add(this.image);
        }
    }

    internal class OrderLabel: Gtk.EventBox {
        private Gtk.Label label;

        internal OrderLabel(string text) {
            this.label = new Gtk.Label(text);
            this.Add(this.label);
        }
    }

    internal class PrepareOrder: Gtk.EventBox {
        private Gtk.Image image;

        internal PrepareOrder() {
            this.image = new Gtk.Image(Gtk.Stock.GoForward, Gtk.IconSize.Button);
            this.Add(this.image);
        }
    }
}
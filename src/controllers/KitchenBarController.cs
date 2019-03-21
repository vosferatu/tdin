using Gtk;
using Glade;
using System;
using System.Threading;
using System.Collections.Generic;

namespace Restaurant {
    public class KitchenBarController {
        OrderDetailWindow detail_window;
        OrderListWindow list_window;

        List<Order> not_picked;
        List<Order> preparing;

        public KitchenBarController() {
            this.not_picked = new List<Order>();
            this.preparing = new List<Order>();
            this.list_window = new OrderListWindow(this.ViewOrderDetails, this.MoveOrder, this.FinishOrder);
            Thread thr = new Thread(new ThreadStart(this.list_window.StartThread));
            thr.Start();
            // TODO: Initialize network listening/speaking tasks
        }

        public void NewOrder(Order order) {
            this.not_picked.Add(order);
            Application.Invoke(delegate {
                this.list_window.AddOrder(new Tuple<long, string>(order.id, order.desc));
            });
        }

        public void ViewOrderDetails(long order_id) {
            Console.WriteLine("Cliked on order #{0}, ViewOrderDetails", order_id);
            Order order = this.FindOrder(order_id);
            if (order != null) {
                this.detail_window = new OrderDetailWindow(this.GoBack, order.id, order.desc, order.table_n);
                this.list_window.root.HideAll();
                this.detail_window.root.ShowAll();
            }
        }

        public void MoveOrder(long order_id) {
            Console.WriteLine("Cliked on order #{0}, MoveOrder", order_id);
            Order order = this.FindOrder(order_id);
            if (order != null && this.list_window.PickOrder(order_id)) {
                this.preparing.Add(order);
                this.not_picked.Remove(order);
                this.list_window.root.ShowAll();
                this.list_window.root.QueueDraw();
            }
        }

        public void FinishOrder(long order_id) {
            Console.WriteLine("Cliked on order #{0}, FinishOrder", order_id);
            Order order = this.FindOrder(order_id);
            if (order != null && this.list_window.RemoveOrder(order_id)) {
                this.preparing.Remove(order);
                this.list_window.root.ShowAll();
                this.list_window.root.QueueDraw();
            }
        }

        public void GoBack(object o, EventArgs args) {
            this.detail_window.root.HideAll();
            this.list_window.root.ShowAll();
        }

        private Order FindOrder(long order_id) {
            foreach(Order order in this.not_picked) {
                if (order.id == order_id) {
                    return order;
                }
            }
            foreach(Order order in this.preparing) {
                if (order.id == order_id) {
                    return order;
                }
            }

            return null;
        }
    }
}
using Gtk;
using Glade;
using System;
using System.Threading;
using System.Collections.Generic;

namespace Restaurant {
    public class KitchenBarController: IController {
        OrderDetailWindow detail_window;
        OrderListWindow list_window;

        List<Order> not_picked;
        List<Order> preparing;

        public KitchenBarController(bool is_kitchen) {
            this.not_picked = new List<Order>();
            this.preparing = new List<Order>();
        }

        public bool InitializeNetwork() {
            return true;
        }

        public bool TryAndJoinNetwork() {
            return true;
        }

        public bool StartController() {
            this.list_window = new OrderListWindow(this.ViewOrderDetails, this.MoveOrder, this.FinishOrder);
            Thread thr = new Thread(new ThreadStart(this.list_window.StartThread));
            try {
                thr.Start();
                return true;
            }
            catch (Exception e) {
                Console.WriteLine("Failed to start KitchenBarController!\n - {0}", e);
                return false;
            }
        }

        public void NewOrder(Order order) {
            // TODO: Call this only on new order from network
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
            Order order = this.FindOrder(order_id);
            if (order != null && this.list_window.PickOrder(order_id)) {
                this.preparing.Add(order);
                this.not_picked.Remove(order);
                this.list_window.root.ShowAll();
                this.list_window.root.QueueDraw();
                // TODO: Warn network that order has moved from 'Not Picked' to 'Preparing'
            }
        }

        public void FinishOrder(long order_id) {
            Console.WriteLine("Cliked on order #{0}, FinishOrder", order_id);
            Order order = this.FindOrder(order_id);
            if (order != null && this.list_window.RemoveOrder(order_id)) {
                this.preparing.Remove(order);
                this.list_window.root.ShowAll();
                this.list_window.root.QueueDraw();
                // TODO: Warn network that order is ready to be picked!
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
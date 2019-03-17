using Gtk;
using Glade;
using System;
using System.Collections.Generic;

namespace Restaurant {
    public class OrderClickedArgs: Gtk.ButtonReleaseEventArgs {
        public Order order{get; private set;}

        public OrderClickedArgs(Order order): base() {
            this.order = order;
        }
    }
    
    public class KitchenBarWindow {
        private const string WINDOW_FILE = "KitchenBar.glade";
        [Glade.Widget]
        VBox NotPickedBox;
        [Glade.Widget]
        VBox PreparingBox;

        List<Order> not_picked;
        List<Order> preparing;

        public Window() {
           this.not_picked = new List<Order>(8);
           this.preparing = new List<Order>(8); 
        }
    
        public void ViewOrderInformation(object sender, OrderClickedArgs args) {

        }

        public void StartPreparingOrder(object sender, OrderClickedArgs args) {

        }

        public void FinishedPreparingOrder(object sender, OrderClickedArgs args) {

        }
    }


}
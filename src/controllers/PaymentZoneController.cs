using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Restaurant {
    public class PaymentZoneController: MarshalByRefObject, ICentralController {
        public event OrderReadyEventHandler OrderReadyEvent;
        public event NewOrderEventHandler NewOrderEvent;
        
        double total_money;
        Dictionary<string, Product> products;
        ConcurrentDictionary<long, Order> orders;

        public PaymentZoneController(List<Product> dishes, List<Product> drinks) {
            this.total_money = 0;
        }

        public bool InitializeNetwork() {
            return true;
        }

        public bool TryAndJoinNetwork() {
            return true;
        }

        public bool StartController() {
            return true;
        }

        public void OrderReady(long order_id) {

        }

        public void NewOrder(Dictionary<string, uint> products, int table_n) {

        }
    }
}
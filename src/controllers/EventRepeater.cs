using System;

namespace Restaurant {
    public abstract class EventRepeaterDelegateObject: MarshalByRefObject {
        event OrderReadyEventHandler OrderReadyEventRepeater;
        event NewOrderEventHandler NewOrderEventRepeater;

        public override object InitializeLifetimeService() {return null;}

        public void NewOrderCallback(Order order) {
            this.NewOrderCallbackCore(order);
        }

        public void OrderReadyCallback(long order_id, uint table_n) {
            this.OrderReadyCallbackCore(order_id, table_n);
        }

        protected abstract void NewOrderCallbackCore(Order order);
        protected abstract void OrderReadyCallbackCore(long order_id, uint table_n);
    }
}
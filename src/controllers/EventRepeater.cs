using System;

namespace Restaurant {
    public abstract class EventRepeaterDelegateObject: MarshalByRefObject {
    #region METHODS
        public override object InitializeLifetimeService() {return null;}

        public void NewOrderCallback(Order order) {
            this.NewOrderCallbackCore(order);
        }

        public void OrderReadyCallback(long order_id, uint table_n) {
            this.OrderReadyCallbackCore(order_id, table_n);
        }

        protected abstract void NewOrderCallbackCore(Order order);
        protected abstract void OrderReadyCallbackCore(long order_id, uint table_n);
    #endregion METHODS
    }

    public class EventRepeaterDelegate: EventRepeaterDelegateObject {
    #region FIELDS
        OrderReadyEventHandler ready_handler;
        NewOrderEventHandler new_handler;
    #endregion FIELDS

    #region METHODS
        public EventRepeaterDelegate (OrderReadyEventHandler ready_handler, NewOrderEventHandler new_handler) {
            this.ready_handler = ready_handler;
            this.new_handler = new_handler;
        }

        protected override void NewOrderCallbackCore(Order order) {
            if (this.new_handler != null) {
                this.new_handler(order);
            }
        }
        
        protected override void OrderReadyCallbackCore(long order_id, uint table_n) {
            if (this.ready_handler != null) {
                this.ready_handler(order_id, table_n);
            }
        }
    #endregion METHODS
    }
}
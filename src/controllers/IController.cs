using System;
using System.Collections.Generic;

namespace Restaurant {
    public delegate void OrderReadyEventHandler(long order_id, uint table_n);
    public delegate void NewOrderEventHandler(Order order);

    public interface IController {
        bool InitializeNetwork();
        bool TryAndJoinNetwork();
        bool StartController();
    }

    public interface ICentralController: IController {
        event OrderReadyEventHandler OrderReadyEvent;
        event NewOrderEventHandler NewOrderEvent;
        
        ICentralController getObj();
        void OrderReady(long order_id, uint table_n);
        void NewOrder(Dictionary<string, uint> products, uint table_n);
    }
}
using System;
using System.Collections.Generic;

namespace Restaurant {
    public delegate void OrderReadyEventHandler(long order_id);
    public delegate void NewOrderEventHandler(Order order);

    public interface IController {
        bool InitializeNetwork();
        bool TryAndJoinNetwork();
        bool StartController();
    }

    public interface ICentralController: IController {
        event OrderReadyEventHandler OrderReadyEvent;
        event NewOrderEventHandler NewOrderEvent;

        void OrderReady(long order_id);
        void NewOrder(Dictionary<string, uint> products, int table_n);
    }
}
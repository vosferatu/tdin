using System;

namespace Restaurant {
    public class Product {
        public enum Type {Dish, Drink};
        public string name {get; private set;}
        public double price {get; private set;}
        public double time {get; private set;}
        public Product.Type type{get; private set;}

        public Product(string name, double price, double time, bool is_dish) {
            this.name = name;
            this.price = price;
            this.time = time;
            this.type = (is_dish) ? Product.Type.Dish : Product.Type.Drink;
        }

        public string toString() {
            return String.Format("{0} ({1}€) - {2}min", this.name, this.price, this.time);
        }
    }
}
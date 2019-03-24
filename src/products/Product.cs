using Base;
using System;

namespace Restaurant {
    [Serializable]
    public class Product: IEquatable<Product>{
        public string name {get; private set;}
        public double price {get; private set;}
        public double time {get; private set;}
        public ProductType type{get; private set;}

        public Product(string name, double price, double time, bool is_dish) {
            this.name = name;
            this.price = price;
            this.time = time;
            this.type = (is_dish) ? ProductType.Dish : ProductType.Drink;
        }

        public string toString() {
            return String.Format("{0} ({1}€) - {2}min", this.name, this.price, this.time);
        }

        public override int GetHashCode() {
            return this.name.GetHashCode();
        }
        
        public bool Equals(Product other) {
            return this.name == other.name;
        }

        public override bool Equals(object obj) {
            return ((obj is Product) && Equals((Product)obj, this));
        }

    }
}
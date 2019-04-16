using Gtk;
using Gdk;
using Base;
using Glade;
using System;
using System.Collections.Generic;

namespace Restaurant {
    public class StatisticsWindow {
        private const string WINDOW_FILE = GuiConstants.WINDOWS_DIR + "Statistics.glade";
        private const string WINDOW_NAME = "root";
        private enum Sorting {Name, Price, Amount, Total};

        private static Gtk.Image DOWN = new Gtk.Image(Gtk.Stock.GoDown, Gtk.IconSize.SmallToolbar);
        private static Gtk.Image UP = new Gtk.Image(Gtk.Stock.GoUp, Gtk.IconSize.SmallToolbar);
        private static Gtk.Image BACK = new Gtk.Image(Gtk.Stock.GoBack, Gtk.IconSize.SmallToolbar);
    #region WIDGETS
        [Glade.Widget]
        public Gtk.Window root;
        [Glade.Widget]
        Gtk.Button NameButton;
        [Glade.Widget]
        Gtk.Button PriceButton;
        [Glade.Widget]
        Gtk.Button AmountButton;
        [Glade.Widget]
        Gtk.Button TotalButton;
        [Glade.Widget]
        Gtk.Button GoBackButton;
        [Glade.Widget]
        Gtk.Table ProductTable;
    #endregion WIDGETS

    #region FIELDS
        SortedSet<StatisticLine> lines;
        SimpleFunction back_handler;
        bool ascending = false;
        Sorting curr_type = Sorting.Name;
    #endregion FIELDS

    #region METHODS
        public StatisticsWindow(SimpleFunction back_handler) {
            this.back_handler = back_handler;
            Glade.XML gxml = new Glade.XML(WINDOW_FILE, WINDOW_NAME, null);
            gxml.Autoconnect(this);
            this.root.SetIconFromFile(GuiConstants.APP_ICON);
            this.GoBackButton.Image = BACK;
            this.GoBackButton.ImagePosition = Gtk.PositionType.Left;
            this.NameButton.Image = UP;
            this.NameButton.ImagePosition = Gtk.PositionType.Right;
        }

        public void UpdateProducts(List<StatisticLine> prods) {
            this.lines = new SortedSet<StatisticLine>(prods, new StatisticLine.Comparer(StatisticLine.NameDescOrder));
            this.SetProducts(this.lines);
        }

        private void ToggleName(object e, EventArgs args) {
            this.ascending = !this.ascending;
            if (this.curr_type != Sorting.Name)
                this.curr_type = Sorting.Name;

            if (ascending) this.ChangeSorting(this.NameButton, StatisticLine.NameAscOrder);
            else this.ChangeSorting(this.NameButton, StatisticLine.NameDescOrder);

            this.PriceButton.Image = null;
            this.AmountButton.Image = null;
            this.TotalButton.Image = null;
        }

        private void TogglePrice(object e, EventArgs args) {
            this.ascending = !this.ascending;
            if (this.curr_type != Sorting.Price)
                this.curr_type = Sorting.Price;

            if (ascending) this.ChangeSorting(this.PriceButton, StatisticLine.PriceAscOrder);
            else this.ChangeSorting(this.PriceButton, StatisticLine.PriceDescOrder);

            this.NameButton.Image = null;
            this.AmountButton.Image = null;
            this.TotalButton.Image = null;
        }

        private void ToggleAmount(object e, EventArgs args) {
            this.ascending = !this.ascending;
            if (this.curr_type != Sorting.Amount)
                this.curr_type = Sorting.Amount;

            if (ascending) this.ChangeSorting(this.AmountButton, StatisticLine.AmountAscOrder);
            else this.ChangeSorting(this.AmountButton, StatisticLine.AmountDescOrder);

            this.NameButton.Image = null;
            this.PriceButton.Image = null;
            this.TotalButton.Image = null;
        }

        private void ToggleTotal(object e, EventArgs args) {
            this.ascending = !this.ascending;
            if (this.curr_type != Sorting.Total)
                this.curr_type = Sorting.Total;

            if (ascending) this.ChangeSorting(this.TotalButton, StatisticLine.TotalAscOrder);
            else this.ChangeSorting(this.TotalButton, StatisticLine.TotalDescOrder);

            this.NameButton.Image = null;
            this.PriceButton.Image = null;
            this.AmountButton.Image = null;
        }

        private void ChangeSorting(Gtk.Button button, Comparison<StatisticLine> comparison) {
            button.Image = (this.ascending ? DOWN : UP);
            button.ImagePosition = Gtk.PositionType.Right;
            this.lines = new SortedSet<StatisticLine>(this.lines, new StatisticLine.Comparer(comparison));
            this.SetProducts(this.lines);
        }

        private void SetProducts(SortedSet<StatisticLine> lines) {
            foreach(Gtk.Widget widget in this.root) {
                if (widget.Parent == null) {
                    widget.Parent = this.root;
                }
            }
            foreach (Gtk.Widget widget in this.ProductTable)
                this.ProductTable.Remove(widget);

            foreach (Gtk.Widget line in lines) {
                uint child_n = (uint)this.ProductTable.Children.Length;
                this.ProductTable.Attach(line, 0, 1, 0 + child_n, 1 + child_n,
                    Gtk.AttachOptions.Expand | Gtk.AttachOptions.Fill, Gtk.AttachOptions.Shrink,
                    0, 0
                );
            }
        }

        private void GoBack(object e, EventArgs args) {
            this.back_handler();
        }
    #endregion METHODS
    }

    public class StatisticLine: Gtk.Table {
        public string name {get; private set;}
        public double price {get; private set;}
        public uint amount {get; private set;}
        public double total {get; private set;}

        public StatisticLine(string name, double price, uint amount): base(1, 4, false) {
            this.name = name;
            this.price = price;
            this.amount = amount;
            this.total = price*amount;
            this.SetLabels();
        }

        private void SetLabels() {
            Gtk.Label name_label = new Gtk.Label(String.Format("<big>{0}</big>", this.name));
            Gtk.Label price_label = new Gtk.Label(String.Format("{0}€", this.price));
            Gtk.Label amount_label = new Gtk.Label(this.amount.ToString());
            Gtk.Label total_label = new Gtk.Label(String.Format("<big><b>{0} €</b></big>", this.total));
            total_label.UseMarkup = true;
            name_label.UseMarkup = true;

            name_label.Xalign = 0.005f;
            this.Attach(name_label, 0, 1, 0, 1, 
                Gtk.AttachOptions.Expand | Gtk.AttachOptions.Fill, Gtk.AttachOptions.Shrink, 0, 0
            );
            price_label.Xalign = 0.5f;
            price_label.SetSizeRequest(100, 30);
            this.Attach(price_label, 1, 2, 0, 1,
                Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink, 0, 0
            );
            amount_label.Xalign = 0.5f;
            amount_label.SetSizeRequest(100, 30);
            this.Attach(amount_label, 2, 3, 0, 1,
                Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink, 0, 0
            );

            total_label.Xalign = 1f;
            total_label.SetSizeRequest(120, 30);
            this.Attach(total_label, 3, 4, 0, 1,
                Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink, 0, 0
            );
        }

        internal class Comparer: IComparer<StatisticLine> {
            private Comparison<StatisticLine> comparer;

            public Comparer(Comparison<StatisticLine> comparer) {
                this.comparer = comparer;
            }

            public int Compare(StatisticLine x, StatisticLine y) {
                return this.comparer(x, y);
            }
        }

        internal static int NameAscOrder(StatisticLine x, StatisticLine y) {
            return x.name.CompareTo(y.name);
        }

        internal static int NameDescOrder(StatisticLine x, StatisticLine y) {
            return -x.name.CompareTo(y.name);
        }

        internal static int PriceAscOrder(StatisticLine x, StatisticLine y) {
            return x.price.CompareTo(y.price);
        }

        internal static int PriceDescOrder(StatisticLine x, StatisticLine y) {
            return -x.price.CompareTo(y.price);
        }

        internal static int AmountAscOrder(StatisticLine x, StatisticLine y) {
            return x.amount.CompareTo(y.amount);
        }

        internal static int AmountDescOrder(StatisticLine x, StatisticLine y) {
            return -x.amount.CompareTo(y.amount);
        }

        internal static int TotalAscOrder(StatisticLine x, StatisticLine y) {
            return x.total.CompareTo(y.total);
        }

        internal static int TotalDescOrder(StatisticLine x, StatisticLine y) {
            return -x.total.CompareTo(y.total);
        }
    }
}
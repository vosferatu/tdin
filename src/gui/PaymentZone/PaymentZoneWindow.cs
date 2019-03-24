using Gtk;
using Gdk;
using Glade;
using System;
using System.Collections.Generic;

namespace Restaurant {
    public delegate void TableSelected(uint table_n);

    public class PaymentZoneWindow {
        private const string WINDOW_FILE = GuiConstants.WINDOWS_DIR + "PaymentZone.glade";
        private const string WINDOW_NAME = "root";
    #region WIDGETS
        [Glade.Widget]
        Gtk.Window root;
        [Glade.Widget]
        Gtk.Table ButtonsTable;
        [Glade.Widget]
        Gtk.ToggleButton Table1;
        [Glade.Widget]
        Gtk.ToggleButton Table2;
        [Glade.Widget]
        Gtk.ToggleButton Table3;
        [Glade.Widget]
        Gtk.ToggleButton Table4;
        [Glade.Widget]
        Gtk.ToggleButton Table5;
        [Glade.Widget]
        Gtk.ToggleButton Table6;
        [Glade.Widget]
        Gtk.ToggleButton Table7;
        [Glade.Widget]
        Gtk.ToggleButton Table8;
        [Glade.Widget]
        Gtk.ToggleButton Table9;
        [Glade.Widget]
        Gtk.Label TotalMoneyLabel;
        [Glade.Widget]
        Gtk.Label OrderPriceLabel;
        [Glade.Widget]
        Gtk.Button PaidButton;
        [Glade.Widget]
        Gtk.Table TableProductsList;

    #endregion WIDGETS

    #region FIELDS
        TableSelected table_handler;
        Dictionary<uint, Gtk.ToggleButton> buttons;
    #endregion FIELDS

    #region METHODS
        public PaymentZoneWindow(TableSelected table_handler) {
            this.table_handler = table_handler;
        }

        public void StartThread() {
            Glade.XML gxml = new Glade.XML(WINDOW_FILE, WINDOW_NAME, null);
            gxml.Autoconnect(this);
            this.buttons = new Dictionary<uint, Gtk.ToggleButton>(9);
            this.buttons.Add(1, this.Table1);
            this.buttons.Add(2, this.Table2);
            this.buttons.Add(3, this.Table3);
            this.buttons.Add(4, this.Table4);
            this.buttons.Add(5, this.Table5);
            this.buttons.Add(6, this.Table6);
            this.buttons.Add(7, this.Table7);
            this.buttons.Add(8, this.Table8);
            this.buttons.Add(9, this.Table9);
            Application.Run();
        }

        internal void OnTableSelect(object e, EventArgs args) {
            Gtk.ToggleButton button = (Gtk.ToggleButton)e;
            Gtk.Label label = (Gtk.Label)button.Child;
            if (this.table_handler != null) {
                uint table_n = UInt32.Parse(label.Text);
                this.table_handler(table_n);
            }
        }

        public void UntoggleButton(uint table_n) {
            if (this.buttons.ContainsKey(table_n)) {
                this.buttons[table_n].Active = false;
                this.buttons[table_n].DrawIndicator = false;
                this.ButtonsTable.ShowAll();
            }
        }

        public void ClearProductsList() {
            this.OrderPriceLabel.Text = "0.0";
            foreach(Gtk.Widget widget in this.TableProductsList) {
                this.TableProductsList.Remove(widget);
            }
        }

        public void SetProductsList(List<Gtk.Widget> prod_info, double total_price) {
            this.OrderPriceLabel.Text = total_price.ToString();
            foreach (Gtk.Widget widget in prod_info) {
                uint child_n = (uint)this.TableProductsList.Children.Length;
                this.TableProductsList.Attach(widget,
                    0, 1, 0 + child_n, 1 + child_n,
                    Gtk.AttachOptions.Expand, Gtk.AttachOptions.Shrink,
                    0, 0
                );
            }
        }

        public void OnDelete(object o, DeleteEventArgs e) {
            Application.Quit();
        }
    #endregion METHODS
    }
}

namespace Restaurant {
    public class TableProductEntry: Gtk.Table {
        Gtk.Label name_label;
        Gtk.Label price_label;
        Gtk.Label amount_label;
        Gtk.Label total_label;
        Gtk.VSeparator sep1;
        Gtk.VSeparator sep2;
        Gtk.VSeparator sep3;

        public TableProductEntry(string name, double price, uint amount): base(1, 7, false) {
            double total = price * amount;
            this.name_label = new Gtk.Label(name);
            this.price_label = new Gtk.Label(price.ToString());
            this.amount_label = new Gtk.Label(amount.ToString());
            this.total_label = new Gtk.Label(total.ToString());
            this.sep1 = new Gtk.VSeparator();
            this.sep2 = new Gtk.VSeparator();
            this.sep3 = new Gtk.VSeparator();
            this.ConfigureLabels();
            this.AttachEverything();
            this.ShowAll();
        }

        private void ConfigureLabels() {
            this.name_label.SetSizeRequest(150, 20);
        }

        private void AttachEverything() {
            this.Attach(this.name_label,
                0, 1, 0, 1,
                Gtk.AttachOptions.Expand, Gtk.AttachOptions.Shrink,
                0, 3
            );
            this.Attach(this.sep1,
                1, 2, 0, 1,
                Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink,
                0, 3
            );
            this.Attach(this.price_label,
                2, 3, 0, 1,
                Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink,
                0, 3
            );
            this.Attach(this.sep2,
                3, 4, 0, 1,
                Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink,
                0, 3
            );
            this.Attach(this.amount_label,
                4, 5, 0, 1,
                Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink,
                0, 3
            );
            this.Attach(this.sep3,
                5, 6, 0, 1,
                Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink,
                0, 3
            );
            this.Attach(this.total_label,
                6, 7, 0, 1,
                Gtk.AttachOptions.Expand, Gtk.AttachOptions.Shrink,
                0, 3
            );
        }
    }
}
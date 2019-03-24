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
            Console.WriteLine("UntoggleButton({0})", table_n);
            if (this.buttons.ContainsKey(table_n)) {
                Console.WriteLine("Toggling button #{0}", table_n);
                // this.buttons[table_n].Toggle();
                this.buttons[table_n].Active = false;
                this.buttons[table_n].DrawIndicator = false;
                this.ButtonsTable.ShowAll();
            }
        }

        public void OnDelete(object o, DeleteEventArgs e) {
            Application.Quit();
        }
    #endregion METHODS
    }
}
using Gtk;
using Gdk;
using Base;
using Glade;
using System;
using System.Timers;
using System.Collections.Generic;

namespace Restaurant {
    /// <summary>
    /// Function to be called when a table is selected
    /// </summary>
    /// <param name="table_n">Number of the selected table</param>
    public delegate void TableSelected(uint table_n);
    /// <summary>
    /// Function to be called when the order has been paid
    /// </summary>
    public delegate void PaidOrder();

    /// <summary>
    /// Responsible for communicating directly with Gtk on the Payment Zone Window
    /// </summary>
    public class PaymentZoneWindow {
        private const string WINDOW_FILE = GuiConstants.WINDOWS_DIR + "PaymentZone.glade";
        private const string WINDOW_NAME = "root";
    #region WIDGETS
        [Glade.Widget]
        public Gtk.Window root;
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
        PaidOrder paid_handler;
        SimpleFunction see_stat;
        Dictionary<uint, Gtk.ToggleButton> buttons;
    #endregion FIELDS

    #region METHODS
        /// <summary>
        /// Constructor of this window
        /// </summary>
        /// <param name="table_handler">Handler when selecting a table</param>
        /// <param name="paid_handler">Handler when paying the table orders</param>
        public PaymentZoneWindow(TableSelected table_handler, PaidOrder paid_handler, SimpleFunction see_stat) {
            this.table_handler = table_handler;
            this.paid_handler = paid_handler;
            this.see_stat = see_stat;
        }

        /// <summary>
        /// Starts the actual Gtk thread of this window
        /// </summary>
        public void StartThread() {
            Glade.XML gxml = new Glade.XML(WINDOW_FILE, WINDOW_NAME, null);
            gxml.Autoconnect(this);
            this.root.SetIconFromFile(GuiConstants.APP_ICON);
            this.buttons = new Dictionary<uint, Gtk.ToggleButton>(9);
            this.FinishSetup();
            Application.Run();
        }

        /// <summary>
        /// Finishes setting up the different widgets of this window
        /// </summary>
        private void FinishSetup() {
            this.buttons.Add(1, this.Table1);
            this.buttons.Add(2, this.Table2);
            this.buttons.Add(3, this.Table3);
            this.buttons.Add(4, this.Table4);
            this.buttons.Add(5, this.Table5);
            this.buttons.Add(6, this.Table6);
            this.buttons.Add(7, this.Table7);
            this.buttons.Add(8, this.Table8);
            this.buttons.Add(9, this.Table9);
            this.PaidButton.Label = "Paid";
            this.PaidButton.ImagePosition = Gtk.PositionType.Right;
            this.PaidButton.Image = new Gtk.Image(GuiConstants.DOLLAR_SIGN);
        }

        /// <summary>
        /// Function called when a table is selected
        /// </summary>
        /// <param name="e">Object that called the function</param>
        /// <param name="args">Event arguments of the callback</param>
        internal void OnTableSelect(object e, EventArgs args) {
            Gtk.ToggleButton button = (Gtk.ToggleButton)e;
            Gtk.Label label = (Gtk.Label)button.Child;
            if (this.table_handler != null) {
                uint table_n = UInt32.Parse(label.Text);
                this.table_handler(table_n);
            }
        }

        /// <summary>
        /// Untoggles the specified button
        /// </summary>
        /// <param name="table_n">Table number associated with a button</param>
        public void UntoggleButton(uint table_n) {
            if (this.buttons.ContainsKey(table_n)) {
                this.buttons[table_n].Active = false;
                this.buttons[table_n].DrawIndicator = false;
                this.ButtonsTable.ShowAll();
            }
        }

        /// <summary>
        /// Clears the table products list
        /// </summary>
        public void ClearProductsList() {
            this.OrderPriceLabel.Text = "0.0";
            this.PaidButton.HideAll();
            foreach(Gtk.Widget widget in this.TableProductsList) {
                this.TableProductsList.Remove(widget);
            }
        }

        /// <summary>
        /// Sets the prodcts shown on the Table Products List window
        /// </summary>
        /// <param name="prod_info">List with the various products</param>
        /// <param name="total_price">Total price of the products</param>
        public void SetProductsList(List<Gtk.Widget> prod_info, double total_price) {
            if (prod_info.Count > 0) {
                this.OrderPriceLabel.Text = total_price.ToString();
                this.PaidButton.ShowAll();
                foreach (Gtk.Widget widget in prod_info) {
                    uint child_n = (uint)this.TableProductsList.Children.Length;
                    this.TableProductsList.Attach(widget,
                        0, 1, 0 + child_n, 1 + child_n,
                        Gtk.AttachOptions.Expand | Gtk.AttachOptions.Fill, Gtk.AttachOptions.Shrink,
                        0, 0
                    );
                    this.TableProductsList.ShowAll();
                }
            }
        }

        /// <summary>
        /// Function called when the paid button is clicked
        /// </summary>
        /// <param name="e">Object that called the function</param>
        /// <param name="args">Arguments of the event</param>
        public void OnPaidClicked(object e, EventArgs args) {
            this.paid_handler();
        }

        /// <summary>
        /// Sets the total money of the restaurant
        /// Fires up a simple animation
        /// </summary>
        /// <param name="money">Total money of the restaurant</param>
        public void SetTotalMoney(double money) {
            Timer price_timer = new Timer(GuiConstants.PRICE_ANIMATION);
            price_timer.Elapsed += (obj, args) => {
                this.TotalMoneyLabel.Text = money.ToString() + " €";
                this.TotalMoneyLabel.Markup = money.ToString() + " €";
                this.TotalMoneyLabel.UseMarkup = false;
                this.TotalMoneyLabel.ModifyFg(Gtk.StateType.Normal, GuiConstants.BLACK);
                price_timer.Close();
            };
            this.TotalMoneyLabel.Text = money.ToString() + " €";
            this.TotalMoneyLabel.Markup = String.Format("<big><b>{0}</b></big>", this.TotalMoneyLabel.Text);
            this.TotalMoneyLabel.UseMarkup = true;
            this.TotalMoneyLabel.ModifyFg(Gtk.StateType.Normal, GuiConstants.GREEN);
            price_timer.Start();
            this.TotalMoneyLabel.Show();
        }

        /// <summary>
        /// Fires up an animation on new order on a table
        /// </summary>
        /// <param name="table_n">Number of table associated with order</param>
        public void NewOrderOnTable(uint table_n) {
            Gtk.Button table = this.buttons[table_n];
            this.TableAnimation(table, GuiConstants.YELLOW);
        }

        /// <summary>
        /// Fires up an animation on order delivered to table
        /// </summary>
        /// <param name="table_n">Number of table associated with order</param>
        public void OrderDeliveredOnTable(uint table_n) {
            Gtk.Button table = this.buttons[table_n];
            this.TableAnimation(table, GuiConstants.GREEN);
        }

        /// <summary>
        /// Simple table animation that is made on new orders or orders delivered to a table
        /// </summary>
        /// <param name="table">Button to be used on animation</param>
        /// <param name="color">Color to be used for animation</param>
        private void TableAnimation(Gtk.Button table, Gdk.Color color) {
            Timer timer = new Timer(GuiConstants.TABLE_ANIMATION);
            timer.Elapsed += (obj, args) => {
                table.Parent.ModifyBg(Gtk.StateType.Normal);
                timer.Enabled = false;
                timer.Close();
            };
            table.Parent.ModifyBg(Gtk.StateType.Normal, color);
            timer.Enabled = true;
            timer.Start();
        }

        /// <summary>
        /// Function to be called when user clicks 'See Statistics' button
        /// </summary>
        /// <param name="e">Object that triggered the event</param>
        /// <param name="args">Event parameters</param>
        internal void SeeStatistics(object e, EventArgs args) {
            this.see_stat();
        } 

        /// <summary>
        /// Function called when the window is destroyed
        /// </summary>
        /// <param name="o">Object</param>
        /// <param name="e">Delete event arguments</param>
        public void OnDelete(object o, DeleteEventArgs e) {
            Application.Quit();
        }
    #endregion METHODS
    }
}

namespace Restaurant {
    /// <summary>
    /// Represents a single entry in the Payment Zone Window table products list
    /// </summary>
    public class TableProductEntry: Gtk.Table {
        Gtk.Label name_label;
        Gtk.Label price_label;
        Gtk.Label amount_label;
        Gtk.Label total_label;
        Gtk.VSeparator sep1;
        Gtk.VSeparator sep2;
        Gtk.VSeparator sep3;

        /// <summary>
        /// Creates a new TableProductEntry
        /// </summary>
        /// <param name="name">Name of the product</param>
        /// <param name="price">Price of the product</param>
        /// <param name="amount">Amount of product ordered</param>
        /// <returns></returns>
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

        /// <summary>
        /// Configures the various labels of the  widget
        /// </summary>
        private void ConfigureLabels() {
            this.name_label.SetSizeRequest(145, 20);
            this.name_label.Xalign = 0f;
            this.price_label.SetSizeRequest(60, 20);
            this.price_label.Markup = String.Format("<small>{0}</small>", this.price_label.Text);
            this.price_label.UseMarkup = true;
            this.amount_label.SetSizeRequest(50, 20);
            this.amount_label.Markup = String.Format("<small>{0}</small>", this.amount_label.Text);
            this.amount_label.UseMarkup = true;
            this.total_label.SetSizeRequest(65, 20);
            this.total_label.Markup = String.Format("<b>{0}€</b>", this.total_label.Text);
            this.total_label.UseMarkup = true;
            this.total_label.Xalign = 0.95f;
        }

        /// <summary>
        /// Finalizes the setup by attaching every widget to the table
        /// </summary>
        private void AttachEverything() {
            this.Attach(this.name_label,
                0, 1, 0, 1,
                Gtk.AttachOptions.Expand | Gtk.AttachOptions.Fill, Gtk.AttachOptions.Shrink,
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
                Gtk.AttachOptions.Expand | Gtk.AttachOptions.Fill, Gtk.AttachOptions.Shrink,
                0, 3
            );
        }
    }
}
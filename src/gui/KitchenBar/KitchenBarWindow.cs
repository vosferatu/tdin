using Gtk;
using Glade;
using System;
using System.Collections.Generic;

namespace Restaurant {
    

    public class ClickedOrderArgs: Gtk.ButtonReleaseEventArgs {
        public long order_id{get; private set;}

        public ClickedOrderArgs(long order_id): base() {
            this.order_id = order_id;
        }
    }
    
    public class KitchenBarWindow {
        private const string WINDOW_FILE = "KitchenBar.glade";
        private const string WINDOW_NAME = "root";

        [Widget]
        VBox NotPickedBox;
        [Widget]
        VBox PreparingBox;

        List<Tuple<long, string>> not_picked;
        List<Tuple<long, string>> preparing;

        public KitchenBarWindow(List<Tuple<long, string>> not_picked, List<Tuple<long, string>> preparing) {
            Application.Init();
            this.not_picked = not_picked;
            this.preparing = preparing;
            Glade.XML gxml = new Glade.XML(GuiConstants.WINDOWS_DIR + WINDOW_FILE, WINDOW_NAME, null);
            gxml.Autoconnect(this);

            foreach(Tuple<long, string> info in not_picked) {
                NotPickedBox.PackStart(new OrderEntry(info.Item1, info.Item2, false), true, true, 1);
            }
            Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
            foreach(Tuple<long, string> info in preparing) {
                PreparingBox.PackStart(new OrderEntry(info.Item1, info.Item2, true), true, true, 1);
            }

        }

        public void Show() {
            this.NotPickedBox.ShowAll();
            this.PreparingBox.ShowAll();
            Application.Run();
        }

        public void ViewOrderInformation(object sender, ClickedOrderArgs args) {

        }

        public void StartPreparingOrder(object sender, ClickedOrderArgs args) {

        }

        public void FinishedPreparingOrder(object sender, ClickedOrderArgs args) {

        }
    
        public void OnDelete(object o, DeleteEventArgs e) {
            Application.Quit();
        }
    }
}
using Gtk;
using Glade;
using System;
using System.Threading;

namespace Restaurant {
    public class GladeWindow {
        [Glade.Widget]
        VBox NotPickedBox;
        [Glade.Widget]
        VBox PreparingBox;

        public GladeWindow(string[] args) {
            Application.Init();
            Glade.XML gxml = new Glade.XML("windows/test.glade", "kitchen", null);
            gxml.Autoconnect(this);

            HBox prod_box = new HBox(false, 0);
            Label label = new Label("WJNEAENIANEI");
            Image img = new Image(Gtk.Stock.Apply, Gtk.IconSize.Button);

            prod_box.Add(label);
            prod_box.Add(img);

            NotPickedBox.Add(prod_box);
            img.Show();
            prod_box.Show();
            label.Show();

            Application.Run();
        }

        public void onImageClicked(object o, EventArgs e) {
            Console.WriteLine("Image pressed!\n - {0}\n - {1}", o, e);
        }

        public void onNameClicked(object o, EventArgs e) {
            Console.WriteLine("Label pressed!\n - {0}\n - {1}", o, e);
        }
    }
}
using Gtk;

namespace Restaurant {
    internal static class GuiConstants {
        public const string WINDOWS_DIR = "./assets/windows/";
        public const string ICONS_DIR = "./assets/icons/";
        
        public const string APP_ICON = ICONS_DIR + "icon.png";
        public const string DOLLAR_SIGN = ICONS_DIR + "dollar.png";

        public const double PRICE_ANIMATION = 400;

        public static readonly Gdk.Color GREEN = new Gdk.Color(0, 128, 0);
        public static readonly Gdk.Color BLACK = new Gdk.Color(0, 0, 0);
    }

    internal class ImageAction: Gtk.EventBox {
        private Gtk.Image image;

        internal ImageAction(string stock, Gtk.IconSize type) {
            this.image = new Gtk.Image(stock, type);
            this.Add(this.image);
        }

        internal void SetImage(string stock, Gtk.IconSize type) {
            this.Remove(this.image);
            this.image = new Gtk.Image(stock, type);
            this.Add(this.image);
            this.ShowAll();
        }
    }
}
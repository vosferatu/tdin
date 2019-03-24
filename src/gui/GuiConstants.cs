using Gtk;

namespace Restaurant {
    internal static class GuiConstants {
        public const string WINDOWS_DIR = "./assets/windows/";
        public const string ICONS_DIR = "./assets/icons/";
        
        public const string APP_ICON = ICONS_DIR + "icon.png";
        public const string DOLLAR_SIGN = ICONS_DIR + "dollar.png";
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
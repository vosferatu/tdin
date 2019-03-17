using System;
using Gtk;

namespace Restaurant
{
    class MainWindow {
        public static void Main(string[] args) {
            Application.Init();
            MainWindow win = new MainWindow();
            win.Show();
            Application.Run();
        }

        public void Show() {

        }
    }
}

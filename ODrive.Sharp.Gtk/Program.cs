using System;
using Gtk;

namespace ODrive.Sharp.Gtk
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();

            // ReSharper disable once StringLiteralTypo
            var app = new Application("com.knuxbbs.odrive", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new MainWindow();
            app.AddWindow(win);

            win.Show();
            Application.Run();
        }
    }
}

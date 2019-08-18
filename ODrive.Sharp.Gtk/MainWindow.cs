using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace ODrive.Sharp.Gtk
{
    class MainWindow : Window
    {
        [UI] private Label _label1 = null;
        [UI] private Button _button1 = null;

        public MainWindow() : this(new Builder("MainWindow.glade"))
        {
        }

        private MainWindow(Builder builder) : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);

            DeleteEvent += Window_DeleteEvent;
            _button1.Clicked += Button1_Clicked;
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void Button1_Clicked(object sender, EventArgs a)
        {
            var quickstart = new GoogleDriveQuickStart();
            quickstart.Run();
            
            _label1.Text = "Sync data.";
        }
    }
}
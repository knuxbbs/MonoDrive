using System;
using GLib;
using Gtk;

namespace ODrive.Sharp.Gtk
{
    class Program
    {
        private static MainWindow _window;

        [STAThread]
        public static void Main(string[] args)
        {
            global::Gtk.Application.Init();

            // ReSharper disable once StringLiteralTypo
            var app = new global::Gtk.Application("com.knuxbbs.odrive", ApplicationFlags.None);
            app.Register(Cancellable.Current);

            _window = new MainWindow();
            app.AddWindow(_window);

            // Bind any unhandled exceptions in the GTK UI so that they are logged.
            ExceptionManager.UnhandledException += OnGLibUnhandledException;

            _window.Show();
            global::Gtk.Application.Run();
        }

        /// <summary>
        /// Passes any unhandled exceptions from the GTK UI to the generic handler.
        /// </summary>
        /// <param name="args">The event object containing the information about the exception.</param>
        private static void OnGLibUnhandledException(UnhandledExceptionArgs args)
        {
            if (args.ExceptionObject is Exception unhandledException)
            {
                var dialog = new Dialog("Error", _window,
                    DialogFlags.Modal | DialogFlags.DestroyWithParent,
                    "Okay", ResponseType.Ok);

                ((Box) dialog.Child).Add(new Label(unhandledException.Message));

                dialog.ShowAll();
                dialog.Run();
                
                dialog.Destroy();
                dialog.Dispose();
            }
        }
    }
}
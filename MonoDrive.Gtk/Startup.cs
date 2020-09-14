using System;
using GLib;
using Gtk;
using Microsoft.Extensions.DependencyInjection;

namespace MonoDrive.Gtk
{
    public class Startup
    {
        private static IServiceProvider ServiceProvider { get; set; }
        private readonly MainWindow _mainWindow;

        public Startup(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public void Run()
        {
            // ReSharper disable once StringLiteralTypo
            var app = new global::Gtk.Application("com.knuxbbs.odrive", ApplicationFlags.None);
            app.Register(Cancellable.Current);

            // Bind any unhandled exceptions in the GTK UI so that they are logged.
            ExceptionManager.UnhandledException += OnGLibUnhandledException;

            app.AddWindow(_mainWindow);
            _mainWindow.Show();

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
                var dialog = new Dialog("Error", ServiceProvider.GetService<MainWindow>(),
                    DialogFlags.Modal | DialogFlags.DestroyWithParent,
                    "Okay", ResponseType.Ok);

                ((Box)dialog.Child).Add(new Label(unhandledException.Message));

                dialog.ShowAll();
                dialog.Run();

                //dialog.Destroy();
                dialog.Dispose();
            }
        }
    }
}
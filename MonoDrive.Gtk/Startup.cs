using System;
using GLib;
using Gtk;
using Microsoft.Extensions.Logging;

namespace MonoDrive.Gtk
{
    public class Startup
    {
        private readonly MainWindow _mainWindow;
        private readonly ILogger<Startup> _logger;

        public Startup(MainWindow mainWindow, ILogger<Startup> logger)
        {
            _mainWindow = mainWindow;
            _logger = logger;
        }

        public void Run()
        {
            // ReSharper disable once StringLiteralTypo
            var app = new global::Gtk.Application("com.knuxbbs.monodrive", ApplicationFlags.None);

            // Bind any unhandled exceptions in the GTK UI so that they are logged.
            ExceptionManager.UnhandledException += OnGLibUnhandledException;

            app.Startup += delegate
            {
                app.AddWindow(_mainWindow);
                _mainWindow.ShowAll();
                //TODO: O aplicativo deve verificar se existem arquivos novos ou apagados
            };

            app.Activated += delegate
            {
                _mainWindow.Present();
            };
            
            ((GLib.Application) app).Run();
        }

        /// <summary>
        /// Passes any unhandled exceptions from the GTK UI to the generic handler.
        /// </summary>
        /// <param name="args">The event object containing the information about the exception.</param>
        private void OnGLibUnhandledException(UnhandledExceptionArgs args)
        {
            if (args.ExceptionObject is Exception unhandledException)
            {
                _logger.LogError(unhandledException, unhandledException.Message);
                
                var dialog = new Dialog("Error", _mainWindow, DialogFlags.Modal | DialogFlags.DestroyWithParent,
                    "Okay", ResponseType.Ok);

                ((Box) dialog.Child).Add(new Label(unhandledException.Message));

                dialog.ShowAll();
                dialog.Run();

                //dialog.Destroy();
                dialog.Dispose();
            }
        }
    }
}
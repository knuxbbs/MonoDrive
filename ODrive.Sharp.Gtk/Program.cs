using System;
using System.Linq;
using GLib;
using Gtk;
using Microsoft.Extensions.DependencyInjection;
using ODrive.Sharp.Infra.IoC;

namespace ODrive.Sharp.Gtk
{
    class Program
    {
        private static IServiceProvider ServiceProvider { get; set; }

        [STAThread]
        static void Main(string[] args)
        {
            global::Gtk.Application.Init();

            // ReSharper disable once StringLiteralTypo
            var app = new global::Gtk.Application("com.knuxbbs.odrive", ApplicationFlags.None);
            app.Register(Cancellable.Current);

            // Bind any unhandled exceptions in the GTK UI so that they are logged.
            ExceptionManager.UnhandledException += OnGLibUnhandledException;

            var services = new ServiceCollection();
            RegisterServices(services);
            ServiceProvider = services.BuildServiceProvider();
            
            var mainWindow = ServiceProvider.GetService<MainWindow>();
            app.AddWindow(mainWindow);
            mainWindow.Show();
            
            global::Gtk.Application.Run();
        }

        private static void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<MainWindow>();

            NativeInjectorBootstrapper.RegisterServices(serviceCollection);
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

                ((Box) dialog.Child).Add(new Label(unhandledException.Message));

                dialog.ShowAll();
                dialog.Run();

                //dialog.Destroy();
                dialog.Dispose();
            }
        }
    }
}